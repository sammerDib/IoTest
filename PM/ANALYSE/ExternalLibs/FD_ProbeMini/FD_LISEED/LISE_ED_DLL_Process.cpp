/*
 * $Id: LISE_ED_DLL_Process.cpp 8257 2009-02-16 17:52:50Z S-PETITGRAND $
 */

#include <windows.h>
#include <stdio.h>
#include <string.h>

// ## probe-common headers ##
#include "..\SrcC\SPG.h"
#include "../FD_FogaleProbe/NIDAQmxConfig.h"
#include "../FD_FogaleProbe/FogaleProbeReturnValues.h"
// ## probe-common headers ##

// ## probe-specific headers ##
#include "../FD_LISE_General/LISE_Consts.h"
#include "../FD_LISE_General/PeakMatch.h"
#include "../FD_LISE_General/LISE_Struct_Process.h"
#include "../FD_LISE_General/LISE_Struct.h"
#include "../FD_LISE_General/LISE_Process.h"

// #include "../FD_LISELS/LISE_LSLI_DLL_Internal.h"
// ## probe-specific headers ##

#include "LISE_ED_DLL_UI_Struct.h"
#include "LISE_ED_DLL_Internal.h"
#include "LISE_ED_DLL_Acquisition.h"
#include "LISE_ED_DLL_Config.h"
#include "LISE_ED_DLL_Create.h"
#include "LISE_ED_DLL_General.h"
#include "LISE_ED_DLL_Process.h"
#include "LISE_ED_DLL_Log.h"
#include "LISE_ED_DLL_Reglages.h"

#include <crtdbg.h>
#include <time.h>

// fonction pour le process des deux différentes méthodes de GetThickness
int LEDGetContinousThickness(LISE_ED& LiseEd, double* _Thickness, double* _Quality,int NbThickness)
{
	SC_CACHED_ARRAY( double, Thickness, _Thickness, NbThickness );
	SC_CACHED_ARRAY( double, Quality, _Quality, NbThickness );
	// log du mode continu dans le fichier de log
	logF(&LiseEd,"[LISEED]\t---- Continous Mode define, return last valid value ----");

	// on récupère le dernier indice de période à l'instant ou l'on a appelé la fonction GetThickness
	RING_BUFFER_POS IndexMoyenne = LiseEd.Lise.IndicePeriod;
	
	// Condition de démarrage du Lise ED
	if(LiseEd.Lise.IndicePeriod.AbsN <= 20)
	{
		logF(&LiseEd,"[LISEED]\tNot enough measurement available to average the measurement with the actual parameters.");
		logF(&LiseEd,"[LISEED]\tRBI: %I64d",LiseEd.Lise.IndicePeriod.AbsN);
		LiseEd.Lise.bReentrance = false;
		return FP_NOT_ENOUGHT_MEAS_TO_AVERAGE;
	}

	// on recherche une première fois quelle mesure on va renvoyer, un best peak ou MatchingSucess
	//int ValeursATester = 36;
	int ValeursATester = (LiseEd.Lise.Moyenne * 2) - 1;
	int ValeurCourante = 0;
	int NbBestPeak = 0;
	int NbMatchingSucess = 0;
	MATCHINGMODE ModeToUse;

	// boucle sur les dernière valeur à tester
	while(ValeurCourante < ValeursATester)
	{
		RBP_Dec(IndexMoyenne);
		PERIOD_RESULT LastMeasure = LiseEd.Lise.Resultats[IndexMoyenne.N];

		// test sur les dernières mesures pour savoir si on a plus de BestPeak ou Matching Sucess
		if(LastMeasure.MatchMode == BestPeak)
		{
			NbBestPeak++;
		}
		else if(LastMeasure.MatchMode == MatchingSucess)
		{
			NbMatchingSucess++;
		}

		// on incrémente la valeur courante de boucle
		ValeurCourante++;
	}

	// cas ou on est sorti de la boucle et que l'on a aucun des deux modes qui s'est détaché, cas Impossible
	if(NbBestPeak < LiseEd.Lise.Moyenne && NbMatchingSucess < LiseEd.Lise.Moyenne)
	{
		if(LiseEd.Lise.bLimitedTime){
			if(NbMatchingSucess >= NbBestPeak){ 
				ModeToUse = MatchingSucess;
				logF(&LiseEd, "[LISEED]\tMoyenne, Limited Time");
			}
			else {
				logF(&LiseEd, "[LISEED]\tBestPeak, Limited Time");
				ModeToUse = BestPeak;
			}
		}
		else{
			// cas ou l'on n'a eu ni assez de BestPeak, ni de matching peak, alors on retourne une erreur (normalement se produit jamais)
			logF(&LiseEd,"[LISEED]\tNot enough measures in CONTINOUS MODE available to average the measurement with the actual parameters");
			LiseEd.Lise.bReentrance = false;
			return FP_NOT_ENOUGHT_MEAS_TO_AVERAGE;
		}
	}
	else if(NbBestPeak >= LiseEd.Lise.Moyenne)
	{
		logF(&LiseEd, "[LISEED]\tBestPeak");
		// on définit le mode à utiliser comme le bestPeak
		ModeToUse = BestPeak;
	}
	else if(NbMatchingSucess >= LiseEd.Lise.Moyenne)
	{
		logF(&LiseEd, "[LISEED]\tMoyenne");
		// on définit le mode à utiliser comme Matching Success
		ModeToUse = MatchingSucess;
	}

	// on positionne à nouveaux l'index sur la valeur de départ
	IndexMoyenne = LiseEd.Lise.IndicePeriod;

	// on va chercher les dernière mesures valide, en se limitant à un nombre définit de mesure pour ne pas rester bloquer dans la fonction
	SC_FIXED_ARRAY(double, ThicknessMoyenne,MAX_THICKNESS); // nombre de thickness max possible à mesurer
	SC_FIXED_ARRAY(double, QualMoyenne,MAX_THICKNESS); // nombre de thickness max possible à mesurer
	for(int k = 0; k< MAX_THICKNESS ;k++)	{ThicknessMoyenne[k] = 0.0;QualMoyenne[k] = 0.0;}	// on initialise le tableau à zéro

	//double QualityMoyenne = 0.0;	// Moyenne de la qualité
	int IndexError = 0;	// Paramètre permettant de mesures testées, bonne ou mauvaise
	int ValeursBonnes = 0;	// nombre de valeurs OK
	int NbThicknessInternal = 0;	// nombre d'épaisseurs

	// condition de sortie de la mesure, soit on a les 16 bonnes valeurs, soit un "TimeOut" de 20 mesures non valides ou pas
	while((ValeursBonnes+1) <= LiseEd.Lise.Moyenne)
	{
		// on commence par décrémenter l'index, bon pour la première mesure car la première mesure valide est a GetNMinusOne 
		RBP_Dec(IndexMoyenne);

		// on récupère la dernière mesure valide
		PERIOD_RESULT LastNMeasure = LiseEd.Lise.Resultats[IndexMoyenne.N];

		// on récupère le nombre d'épaisseurs
		NbThicknessInternal = V_Min(LastNMeasure.iNbThickness,NbThickness);
		
		// On va tester si la mesure est bonne => supérieure à la qualité
		if(NbThicknessInternal > 0 && ModeToUse == LastNMeasure.MatchMode)
		{ 
			// seulement si la valeur est strictement supérieure à zéro
			if(LastNMeasure.fThickness[0] > 0.0)
			{
				// si la mesure est valide alors on l'ajoute au tableau de résultats
				for(int i = 0;i<NbThicknessInternal;i++)
				{			
					//LogfileF(*LiseEd.Lise.Log, "[LISEED]\tAcqMode Continuous i=%i ThicknessMoyenne=%.3f QualMoyenne=%.3f",LastNMeasure.fThickness[i],LastNMeasure.fQuality);
					ThicknessMoyenne[i] += LastNMeasure.fThickness[i];
					QualMoyenne[i] += LastNMeasure.fQuality;//approximation tous les pics de même qualité
				}

				// on incrémente le compteur de valeurs bonnes
				ValeursBonnes++;

				// log dans le fichier peak moyenne si activé
				LogPeakUsed(LiseEd.Lise,&LastNMeasure,ValeursBonnes,1);

			}
			else
			{
				// index survant à surveiller le nombre de mesures max à laisser passer
				IndexError++;
			}
		}
		else
		{
			// index suivant à surveiller le nombre de mesures max à laisser passer
			IndexError++;
		}

		// cas ou l'on a pas trouvé 2*Moyenne résultats valide, il y a un problème et on sort
		if(IndexError >= 2*LiseEd.Lise.Moyenne)
		{
			// log de l'erreur dans peakmoyenne
			char Message[LONG_STR]; 
			sprintf(Message,"Not all thickness valid found in Continous GetThickness, can't find %d measures with the same matching mode",LiseEd.Lise.Moyenne);
			LogPeakCommentResult(LiseEd.Lise,Message);

			logF(&LiseEd,"[LISEED]\tNot all thickness valid found in Continous GetThickness, can't find %d measure with the same matching mode",LiseEd.Lise.Moyenne);
			LiseEd.Lise.bReentrance = false;
			return FP_NOT_ALL_VALUE_FOUND;
		}
	}

	// on teste et vérifie que le nombre de mesure valide est en accords avec la moyenne
	if(ValeursBonnes != LiseEd.Lise.Moyenne)
	{
		// log de l'erreur dans peakmoyenne
		LogPeakCommentResult(LiseEd.Lise,"Number of good value found is different of average parameter");

		logF(&LiseEd,"[LISEED]\tNumber of good value found is different of average parameter");
		LiseEd.Lise.bReentrance = false;
		return FP_GOOD_VALUE_DIFF_OF_AVERAGE;
	}
	int i = 0;

	// on copie les n valeurs de thickness trouvé
	for(i=0;i<NbThicknessInternal;i++)
	{
		Thickness[i] = ThicknessMoyenne[i] / LiseEd.Lise.Moyenne;
		Quality[i] = QualMoyenne[i] / LiseEd.Lise.Moyenne;
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\t[CONTINUOUS]\tQualMoyenne %i: %f",i,Thickness[i]);
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\t[CONTINUOUS]\tThicknessMoyenne %i: %f",i,QualMoyenne[i]);
	}

	// log des résultat dans PeakMoyenne
	LogPeakResult(LiseEd.Lise,Thickness,Quality);

	// on loggue les infos
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\t[CONTINUOUS]\t-------------------");
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\t[CONTINUOUS]\tNumber of thickness: %i",NbThicknessInternal);
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\t[CONTINUOUS]\tQuality: %f",Quality[0]);
		for(int i =0;i<NbThickness;i++) LogfileF(*LiseEd.Lise.Log,"[LISEED]\t[CONTINUOUS]\tThickness %i: %f",i,Thickness[i]);
	}

	logF(&LiseEd,"[LISEED]\tQuality value = %f",Quality[0]);
	LiseEd.Lise.bReentrance = false;

	// retour OK sans erreur
	return FP_OK;
}
int LEDGetSingleShotThicknessWithTimeout(LISE_ED& LiseEd, double* _Thickness, double* _Quality,int NbThickness)
{
	SC_CACHED_ARRAY( double, Thickness, _Thickness, NbThickness );
	SC_CACHED_ARRAY( double, Quality, _Quality, NbThickness );

	Quality[0] = 0;	// on retourne une qualité de Zéro
	LiseEd.Lise.bNeedRead = false;	// On change la valeur de bNeedRead
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log, "[LISEED]\t[RET]\tReturning Gethickness with Last Good Thickness and DLL_OK - No Synchronisation with thread");
	}
	LiseEd.Lise.bReentrance = false;
	LiseEd.Lise.bGetThickness = false;
	
	// on initialise le tableau d'épaisseur
	{
		for(int k=0;k<NbThickness;k++)	
		{
			Thickness[k] = 0.0;
			Quality[k] = 0;
		}
		for(int k=0;k<NB_PEAK_MAX;k++)	
		{
			LiseEd.Lise.LastAmplitudePics[k] = 0.0;
		}
	}
	
	// on réinitialise le tableau d'épaisseur
	RING_BUFFER_POS IndiceCalculMoyenne = LiseEd.Lise.WaitNThicknessStop;
	
	// on calcule les n valeurs d'épaisseurs
	int CompteurMoyenneBestPeak = 0;
	
	// tableau des épaisseurs des bestPeak
	SC_FIXED_ARRAY(double, BestPeakThickness,MAX_PEAKS);
	{for(int i = 0;i<MAX_PEAKS;i++) BestPeakThickness[i] = 0.0;}
	
	// compteur des n valeurs de thcikness
	int CompteurMoyenneMatchingPeak = 0;
	
	// tableau des épaisseurs des MatchingPeak
	SC_FIXED_ARRAY(double, MatchingSucessThickness,MAX_THICKNESS);
	{for(int i = 0;i<MAX_THICKNESS;i++) MatchingSucessThickness[i] = 0.0;}

	int CompteurMoyenne = 1;
	// indice qui va pointer sur le buffer de résultat entre le ThicknessStart et Stop
	while(IndiceCalculMoyenne.AbsN > LiseEd.Lise.WaitNThicknessStart.AbsN)
	{
		// on décrémente l'indice de décimation
		RBP_Dec(IndiceCalculMoyenne);
		if(LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].MatchMode == BestPeak)
		{
			// seulement si la valeur est strictement suppérieure à zéro
			if(LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].fThickness[0] > 0.0)
			{
//FDE				LogfileF(*LiseEd.Lise.Log, "[LISEED]\tMode best peak");
				// on ajoute l'épaisseur courante aux épaisseur moyenne
				{for (int cptThick=0; cptThick<LiseEd.Lise.sample.NbThickness; cptThick++)
				{
					BestPeakThickness[cptThick] += LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].fThickness[cptThick];
				}}

				{for (int cptThick=0; cptThick<LiseEd.Lise.sample.NbThickness+1; cptThick++)
				{
					// mise à jour des valeurs d'amplitude cptThick
					LiseEd.Lise.LastAmplitudePics[cptThick] += (LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexPlus[cptThick]].Intensite + LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexMoins[cptThick]].Intensite) / 2.0;
					LiseEd.Lise.LastPositionPicsPlus[cptThick] += LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexPlus[cptThick]].Position;
					LiseEd.Lise.LastPositionPicsMoins[cptThick] += LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexMoins[cptThick]].Position;
				}}

				// on incrémente le compteur de valeur bonne pour faire une moyenne
				CompteurMoyenneBestPeak++;
			}
		}
		else if(LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].MatchMode == MatchingSucess)
		{
			// seulement si la valeur est strictement suppérieure à zéro
			if(LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].fThickness[0] > 0.0)
			{
				LogfileF(*LiseEd.Lise.Log, "[LISEED]\tMode Matching success");
				// on ajoute l'épaisseur courante aux épaisseur moyenne
				{for (int cptThick=0; cptThick<LiseEd.Lise.sample.NbThickness; cptThick++)
				{
					MatchingSucessThickness[cptThick] += LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].fThickness[cptThick];
				}}

				// maj des amplitude peaks
				{for (int cptThick=0; cptThick<LiseEd.Lise.sample.NbThickness+1; cptThick++)
				{
					// mise à jour des valeurs d'amplitude cptThick + 100
					LiseEd.Lise.LastAmplitudePics[cptThick] += (LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexPlus[cptThick]].Intensite + LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexMoins[cptThick]].Intensite) / 2.0;
					LiseEd.Lise.LastPositionPicsPlus[cptThick] += LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexPlus[cptThick]].Position;
					LiseEd.Lise.LastPositionPicsMoins[cptThick] += LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexMoins[cptThick]].Position;
					// log du pic
					//LogfileF(*LiseEd.Lise.Log, "[LISEED]\t[AMPLITUDEPEAK]\tA%i :\t%.3f\t%.3f\t%.3f\t%.3f",cptThick,
					//	LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexPlus[cptThick]].Intensite,
					//	LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexMoins[cptThick]].Intensite,
					//	LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexPlus[cptThick]].Position,
					//	LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexMoins[cptThick]].Position );
				}}

				// on incrémente le compteur de valeur bonne pour faire une moyenne
				CompteurMoyenneMatchingPeak++;
			}

			// log des peaks dans le fichier peak moyenne
			LogPeakUsed(LiseEd.Lise,&LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)],CompteurMoyenne,0);
		}
	}
	int nb_errs = 0;
	for(int k=0;k<LiseEd.Lise.sample.NbThickness;k++)
	{
		if(CompteurMoyenneBestPeak >= CompteurMoyenneMatchingPeak && CompteurMoyenneBestPeak > 0)
		{
			Thickness[k] = BestPeakThickness[k]/(double)CompteurMoyenneBestPeak;
			LiseEd.Lise.LastAmplitudePics[k]/=(double)CompteurMoyenneBestPeak;
		}
		else if(CompteurMoyenneBestPeak < CompteurMoyenneMatchingPeak && CompteurMoyenneMatchingPeak > 0)
		{
			Thickness[k] = MatchingSucessThickness[k]/(double)CompteurMoyenneMatchingPeak;
			LiseEd.Lise.LastAmplitudePics[k]/=(double)CompteurMoyenneMatchingPeak;
		}
		else
		{
			Thickness[k] = 0.0;
			LiseEd.Lise.LastAmplitudePics[k] =0.0;
			nb_errs++;
		}
	}

	char Message[LONG_STR];
	sprintf(Message,"[TIMEOUT_SINGLE_SHOT]%i\t%f\t%f\t%f\t\r\n",LiseEd.Lise.Moyenne,Thickness[0],Thickness[1],Quality[0]);
	LogPeakCommentResult(LiseEd.Lise,Message);

	LiseEd.Lise.bReentrance = false;

	DefRestore(LiseEd.Lise.sample);
	LiseEd.Lise.iFirstMatchingSucces = false;

	if (nb_errs>0)
	{
		LogfileF(*LiseEd.Lise.Log, "[LISEED] No good data.");
		return FP_NO_VALUE_FOUND;
	}
	else
	{
		// retour OK sans erreur
		return FP_OK;
	}
}
int LEDGetSingleShotThickness(LISE_ED& LiseEd, double* _Thickness, double* _Quality,int NbThickness)
{
	SC_CACHED_ARRAY( double, Thickness, _Thickness, NbThickness );
	SC_CACHED_ARRAY( double, Quality, _Quality, NbThickness );
	LiseEd.Lise.bNeedRead = true;	
	// on teste si la thread est active
	if(LiseEd.Lise.bThreadActive == true)
	{
		if(LiseEd.Lise.bDebug)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\t Waiting Synchronisation with thread");
		}	
		int c=0;
		clock_t TimeStart = clock();
		
		// parametre pour la thread, on attend la permission de lire
		while(!LiseEd.Lise.bReadAllowed)	
		{
			if(clock() - TimeStart > LEDI_TIMEOUT_GETTH)
			{
				LiseEd.Lise.bNeedRead = false;
				if(LiseEd.Lise.bDebug)
				{
					LogfileF(*LiseEd.Lise.Log,"[LISEED]\tTimeout of thread, returning of GetThickness with DLL_OK and all thickness = 0.0");
				}
				LiseEd.Lise.bReentrance = false;
				LiseEd.Lise.bGetThickness = false;
				// on ne valide pas les valeur d'épaisseur dans le cas présent
				for(int k = 0;k<NbThickness;k++)	
				{
					Thickness[k] = 0.0;
				}
                for(int k = 0;k<NB_PEAK_MAX;k++)	
				{
					LiseEd.Lise.LastAmplitudePics[k] = 0.0;
                }
				DefRestore(LiseEd.Lise.sample);
				LiseEd.Lise.iFirstMatchingSucces = false;
				return FP_OK;
			}
			Sleep(1);
		}
	}

	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log, "[LISEED]\tFind N thickness with good quality");
	}
	// début de modif
	// MA 20/02/2009 on calcule la valeur de qualité par rapport au mode de matching
	// on recherche une première fois quelle mesure on va renvoyer, un best peak ou MatchingSucess
	int ValeursATester = (LiseEd.Lise.Moyenne * 2) - 1;
	int ValeurCourante = 0;
	int NbBestPeak = 0;
	int NbMatchingSucess = 0;
	int NbUnknown = 0;
	MATCHINGMODE ModeToUse;
	RING_BUFFER_POS IndexMoyenne = LiseEd.Lise.WaitNThicknessStop;
	
	LogPeakComment(LiseEd.Lise,"--------------------------------\r\n");

	//while(ValeurCourante < ValeursATester)
	while(LiseEd.Lise.WaitNThicknessStart.AbsN < IndexMoyenne.AbsN)
	{
		RBP_Dec(IndexMoyenne);
		PERIOD_RESULT LastMeasure = LiseEd.Lise.Resultats[IndexMoyenne.N];

		// test sur les dernière mesures pour savoir si on a plus de BestPeak ou Matching Sucess
		if(LastMeasure.MatchMode == BestPeak)
		{
			NbBestPeak++;
			LogPeakRBIInfo(LiseEd.Lise,IndexMoyenne.N,LastMeasure.fThickness[0],0);
		}
		else if(LastMeasure.MatchMode == MatchingSucess)
		{
			NbMatchingSucess++;
			LogPeakRBIInfo(LiseEd.Lise,IndexMoyenne.N,LastMeasure.fThickness[0],1);
		}
		else
		{
			NbUnknown++;
			LogPeakRBIInfo(LiseEd.Lise,IndexMoyenne.N,LastMeasure.fThickness[0],2);
#ifdef _DEBUG
			//BreakHook();
#endif
		}

		// on incrémente la valeur courante de boucle
		ValeurCourante++;
	}

	if(NbBestPeak < LiseEd.Lise.Moyenne && NbMatchingSucess < LiseEd.Lise.Moyenne)
	{
		if(LiseEd.Lise.bLimitedTime){
			/*if(NbMatchingSucess >= LiseEd.Lise.Moyenne) ModeToUse = MatchingSucess;
			else if(NbBestPeak >= LiseEd.Lise.Moyenne) ModeToUse = BestPeak;*/

			if(NbMatchingSucess >= NbBestPeak){ 
				ModeToUse = MatchingSucess;
			}
			else {
				ModeToUse = BestPeak;
			}
		}
		else{
			// cas ou l'on n'a eu ni assez de BestPeak, ni de matching peak, alors on retourne une erreur (normalement se produit jamais)
			if(LiseEd.Lise.bDebug == true)
			{
				LogfileF(*LiseEd.Lise.Log,"[LISEED]\tNot enough measures available in SINGLE SHOT MEASUREMENT to average the measurement with the actual parameters");
				LogfileF(*LiseEd.Lise.Log,"[LISEED]\tNumber of peak found (Matching Success or Best peak) is less than number average required.\r\nNbBestPeak = %i\r\nNbMatchingSuccess = %i\r\nUnknown = %i\r\nStart RB  = %i\r\nStop RN = %i",NbBestPeak,NbMatchingSucess,NbUnknown,LiseEd.Lise.WaitNThicknessStart.N,LiseEd.Lise.WaitNThicknessStop.N);
			}
			LiseEd.Lise.bReentrance = false;

			// on ne valide pas les valeurs d'épaisseur dans le cas présent
			for(int k = 0;k<NbThickness;k++)	
			{
				Thickness[k] = 0.0;
				Quality[k] = 0;	// on retourne une qualité de Zéro
			}
			for(int k = 0;k<NB_PEAK_MAX;k++)	
			{
				LiseEd.Lise.LastAmplitudePics[k] = 0.0;
			}

			DefRestore(LiseEd.Lise.sample);
			LiseEd.Lise.iFirstMatchingSucces = false;

			char Message[LONG_STR];
			sprintf(Message,"Number of peak found (Matching Success or Best peak) is less than number average required.\r\nNbBestPeak = %i\r\nNbMatchingSuccess = %i\r\nUnknown = %i\r\nStart RB  = %i\r\nStop RN = %i",NbBestPeak,NbMatchingSucess,NbUnknown,LiseEd.Lise.WaitNThicknessStart.N,LiseEd.Lise.WaitNThicknessStop.N);
			LogPeakCommentResult(LiseEd.Lise,Message);

			// qualité nulle
			return FP_NOT_ENOUGHT_MEAS_TO_AVERAGE;
		}
	}
	else if(NbMatchingSucess >= LiseEd.Lise.Moyenne) ModeToUse = MatchingSucess;
	else if(NbBestPeak >= LiseEd.Lise.Moyenne) ModeToUse = BestPeak;

	// on initialise le tableau des thickness moyenne
	SC_FIXED_ARRAY(double, Moyenne, NB_PEAK_MAX);
	for (int i=0; i<NB_PEAK_MAX; i++)
	{
		Moyenne[i] = 0.0;
		LiseEd.Lise.LastAmplitudePics[i] = 0.0;
	}
	for (int i=0; i<NbThickness; i++)
	{
		Quality[i] = 0;	// on retourne une qualité de Zéro
	}

	// on initilise la valeur de qualité moyenne
	double moyQual = 0.0;
	RING_BUFFER_POS IndiceCalculMoyenne;
	IndiceCalculMoyenne = LiseEd.Lise.WaitNThicknessStop;
	if(LiseEd.Lise.bDebug)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tStart index :%d Stop Index : %d, Average Value = %d ",LiseEd.Lise.WaitNThicknessStart.N,LiseEd.Lise.WaitNThicknessStop.N,LiseEd.Lise.Moyenne);
	}
	
	int cptThick;

	int CompteurMoyenne = 0;
	int CompteurError = 0;
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tQuality Threshold in Sample Definition %f",LiseEd.Lise.sample.QualityThreshold);
	}

	// modif de la condition while MA 22/01/2009
	// RBP_Inc(IndiceCalculMoyenne);

	// booléen poru surveiller si on a les n valeurs au dessus du seuil de qualité
	bool bAllValueAboveQTh = true;

	// boucle permettant d'aller tester les n épaisseurs contenues entre Start et Stop
	while(IndiceCalculMoyenne.AbsN > LiseEd.Lise.WaitNThicknessStart.AbsN)
	{
		// on décrémente l'indice de décimation
		//RBP_Dec(IndiceCalculMoyenne);
		if(LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].MatchMode == ModeToUse && CompteurMoyenne < 16)
		{
			// seulement si la première valeur n'est pas de zéro
			if(LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].fThickness[0] > 0.0)
			{
				// on ajoute l'épaisseur courante aux épaisseur moyenne
				for (cptThick=0; cptThick<NbThickness; cptThick++)
				{
					Moyenne[cptThick] += LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].fThickness[cptThick];
				}

				for (cptThick=0; cptThick<NbThickness+1; cptThick++)
				{
					// mise à jour des valeurs d'amplitude cptThick
					LiseEd.Lise.LastAmplitudePics[cptThick] += (LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexPlus[cptThick]].Intensite + LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexMoins[cptThick]].Intensite) / 2.0;
					LiseEd.Lise.LastPositionPicsPlus[cptThick] += LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsPlusVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexPlus[cptThick]].Position;
					LiseEd.Lise.LastPositionPicsMoins[cptThick] += LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PicsMoinsVoie1[LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].PkIndexMoins[cptThick]].Position;
				}

				// on incrémente le compteur de valeur bonne pour faire une moyenne
				CompteurMoyenne++;

				// On ajoute la valeur à la moyenne de Qualité
				moyQual += LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].fQuality;

				// Si une valeur du seuil de qualité n'est pas bonne alors on change le booléen de surveillance
				if(LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)].fQuality <= LiseEd.Lise.sample.QualityThreshold)
				{
					bAllValueAboveQTh = false;
				}				
			}

			// on loggue les pics utilisés
			LogPeakUsed(LiseEd.Lise,&LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)],CompteurMoyenne,0);
		
			// Pour sortir de la boucle, si les valeurs ont ete trouvees
			if(CompteurMoyenne >= LiseEd.Lise.Moyenne ) 
			{
				break;
			}
		}

		RBP_Dec(IndiceCalculMoyenne);
		// on incrémente le compteur d'erreur
		CompteurError++;
	}

	// modif MA 22/01/2009
	if(LiseEd.Lise.Moyenne > CompteurMoyenne) // Pour sortir de la boucle
	{ // Cas ou l'on sort de la fonction pour ne pas rester bloquer dans le while
		LiseEd.Lise.bNeedRead = false;
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log, "Start in buffer resultats : %i",LiseEd.Lise.WaitNThicknessStart.AbsN);
			LogfileF(*LiseEd.Lise.Log, "Stop in buffer resultats : %i",LiseEd.Lise.WaitNThicknessStop.AbsN);
			LogfileF(*LiseEd.Lise.Log, "[LISEED]\tCan't find N thickness with quality superior or equivalent to Quality threshold");
			for(int k = 0;k<(LiseEd.Lise.WaitNThicknessStop.AbsN - LiseEd.Lise.WaitNThicknessStart.AbsN);k++)
			{
				int indice = LiseEd.Lise.WaitNThicknessStart.N + k;
				if (indice > LiseEd.Lise.WaitNThicknessStart.Len)	indice = k;
				//LogfileF(*LiseEd->Lise.Log, "[LISEED]\tQuality Value n%i : %f",k,LiseEd->Lise.Resultats[indice].fQuality);	
			}
		}
		LiseEd.Lise.bReentrance = false;
		LiseEd.Lise.bGetThickness = false;
		// on ne valide pas les valeur d'épaisseur dans le cas présent
		for(int k = 0;k<NbThickness;k++)
		{
			Thickness[k] = 0.0;
			Quality[k] = 0;	// on retourne une qualité de Zéro
		}

		DefRestore(LiseEd.Lise.sample);
		LiseEd.Lise.iFirstMatchingSucces = false;

		char Message[LONG_STR];
		sprintf(Message,"Not All Valid thickness found\r\n");
		LogPeakCommentResult(LiseEd.Lise,Message);

		return FP_NOT_ALL_VALUE_FOUND;
	}
	// fin de modif Ma

	// On print dans le fichier les peaks utilisés pour sortir l'épaisseur
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log, "[LISEED]\tN Thickness find for quality average");
	}

	// MA 22/01/2009 pour debug
	if(LiseEd.Lise.bDebug == true)
	{
		LogfileF(*LiseEd.Lise.Log, "Start in buffer resultats : %i",LiseEd.Lise.WaitNThicknessStart.AbsN);
		LogfileF(*LiseEd.Lise.Log, "Stop in buffer resultats : %i",LiseEd.Lise.WaitNThicknessStop.AbsN);
		LogfileF(*LiseEd.Lise.Log, "[LISEED]\tFind N thickness with quality superior or equivalent to Quality threshold");
		for(int k = 0;k<(LiseEd.Lise.WaitNThicknessStop.AbsN - LiseEd.Lise.WaitNThicknessStart.AbsN);k++)
		{
			int indice = LiseEd.Lise.WaitNThicknessStart.N + k;
			if (indice > LiseEd.Lise.WaitNThicknessStart.Len)	indice = k;
		}
	}

	// MA 22/01/2009 fin de pour debug
	for (cptThick=0; cptThick<NbThickness; cptThick++)
	{
		Thickness[cptThick] = Moyenne[cptThick] / (double)CompteurMoyenne;
		char temp[LONG_STR];
		sprintf(temp, "[LISEED]\tThickness[%i] = %f", cptThick, Thickness[cptThick]);
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,temp);
		}
	}

	// mise à jour des amplitude peaks
	for (cptThick=0; cptThick<NbThickness+1; cptThick++)
	{
		LiseEd.Lise.LastAmplitudePics[cptThick] = LiseEd.Lise.LastAmplitudePics[cptThick] / (double)CompteurMoyenne;
		// log du pic
		//LogfileF(*LiseEd.Lise.Log, "[LISEED]\t[AMPLITUDEPEAK]\tA%i :\t%.3f\t%.3f\t%.3f",cptThick,
		//	LiseEd.Lise.LastAmplitudePics[cptThick],
		//	LiseEd.Lise.LastPositionPicsPlus[cptThick],
		//	LiseEd.Lise.LastPositionPicsMoins[cptThick] );
	}

	if(CompteurMoyenne == 0)
	{
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tGetThickness Fail - Impossible to divide by Zero");
		}
		LiseEd.Lise.bNeedRead = false;
		LiseEd.Lise.bReentrance = false;
		LiseEd.Lise.bGetThickness = false;
		DefRestore(LiseEd.Lise.sample);
		LiseEd.Lise.iFirstMatchingSucces = false;
		
		// log du comment dans le fichier pic
		LogPeakCommentResult(LiseEd.Lise,"No value found\r\n");

		return FP_NO_VALUE_FOUND;
	}	
	for (cptThick=0; cptThick<NbThickness; cptThick++)
	{
		//approximation: tous les pics de même qualite
		Thickness[cptThick] = Moyenne[cptThick] / (double)CompteurMoyenne;
		Quality[cptThick] = moyQual / (double)CompteurMoyenne;

		// on teste le cas ou on a pas eu n valeurs au dessus du seuil de qualité et que la valeur moyenne soit supérieure au seuil de qualité
		if(bAllValueAboveQTh == false && Quality[cptThick] >= LiseEd.Lise.sample.QualityThreshold)
		{
			Quality[cptThick] = LiseEd.Lise.sample.QualityThreshold;
		}
	}

	LiseEd.Lise.bNeedRead = false;

	char Message[LONG_STR];
	sprintf(Message,"%i\t%f\t%f\t%f\t\r\n",LiseEd.Lise.Moyenne,Thickness[0],Thickness[1],Quality[0]);
	LogPeakCommentResult(LiseEd.Lise,Message);

	if(LiseEd.Lise.bDebug == true)
	{
		// chaine de caractère temporaire pour écrire le message
		char messTemp[LONG_STR];

		sprintf(messTemp, "[LISEED]\tNumber of thickness returned : %i", NbThickness);
		LogfileF(*LiseEd.Lise.Log, messTemp);
		LogfileF(*LiseEd.Lise.Log, "[LISEED]\tQuality Value %f",Quality[0]);
		LogfileF(*LiseEd.Lise.Log, "[LISEED]\tLEDIGetThickness - End");
	}

	// on relache la réentrance
	LiseEd.Lise.bReentrance = false;

	// retour OK sans erreur
	return FP_OK;
}
// fonction pour la détection d'une période comprenant un pulse plus et moins
int LEDPulseDetection(LISE& Lise,LISE_ED& LiseEd,PICRESULT* BufferResultat,double* Buffer,RING_BUFFER_POS &WriteResultChannelProcess, int Voie)
{
	if( BufferResultat == NULL)
	{
		if(Lise.bDebug == true)
		{
			if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tError in Pulse Detection - BufferResultat NULL");
		}
		return STATUS_FAIL;
	}
	if( BufferResultat == NULL)
	{
		if(Lise.bDebug == true)
		{
			if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tError in Pulse Detection - Buffer Signal NULL");
		}	
		return STATUS_FAIL;
	}
	int i = Lise.Read.N;
        // Detection du sens
        if( ((LiseEd.fNiveauHaut + LiseEd.fTolerance) >= Buffer[i]) && ((LiseEd.fNiveauHaut - LiseEd.fTolerance) <= Buffer[i]))
        { // Detection d'un pulse plus, on ajoute à la condition la voie (les pulses ne sont présent que sur la voie 1)
			if (LiseEd.bFirstPulseSample)
			{
				if (Lise.SensPositif == false)
				{
					LiseEd.PossiblePulsePlusLeft = Lise.Read;
					LiseEd.cptSensDetect = 1;
					LiseEd.bFirstPulseSample = false;
					if(Lise.bDebug == true)
					{		
						if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tFirst sample detected for Pulse Plus Detection");
					}
				}
			}
			else
			{
				LiseEd.cptSensDetect++;
				if (LiseEd.cptSensDetect >= Lise.iNombreEchantillons && LiseEd.bPulseDetection == false)
				{
					if (!Lise.FirstPass)
					{
						if (LiseEd.PossiblePulsePlusLeft.N >= Lise.PulsePlusLeft.N)
						{
							Lise.NbSamplesLastPeriod = LiseEd.PossiblePulsePlusLeft.N - Lise.PulsePlusLeft.N;
						}
						else
						{
							Lise.NbSamplesLastPeriod = LiseEd.PossiblePulsePlusLeft.N - Lise.PulsePlusLeft.N + Lise.BufferLen;
						}
					}
					// MA 22/01/2009 : Modif pour connaitre la valeur du pulse plus sur la dernière période
					LiseEd.PulsePlusWidthLastPeriod = Lise.PulsePlusRight.AbsN - Lise.PulsePlusLeft.AbsN;
					// MA 22/01/2009 : fin de modif
					Lise.PulsePlusLeft = LiseEd.PossiblePulsePlusLeft;
					LiseEd.PossiblePulsePlusLeft.AbsN = 0;
					LiseEd.PossiblePulsePlusLeft.N = 0;
					if(Lise.bDebug == true)
					{		
						if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tPulse Plus Left Detected");
					}

					LiseEd.cptSensDetect = 0;
					LiseEd.bPulseDetection = true;
					if(!Lise.FirstPass && Lise.Indice != 0)
					{
						if(Lise.bDebug == true)
						{		
							if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tErase Peaks in Pulse Plus in process");
						}
						// On retire les pics trop proches de pulse plus.
						while ((BufferResultat[RBP_GetNMinusOne(WriteResultChannelProcess)].XAbsN > Lise.PulsePlusLeft.AbsN - (__int64)Lise.iNombreEchantillons) && WriteResultChannelProcess.AbsN >= 0)
						{
 							RBP_Dec(WriteResultChannelProcess);
							Lise.Indice--;
						}
						if(Lise.bDebug == true)
						{		
							if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tErase Peaks in pulse plus Success");
						}
					}
					
					// Mise à jour des XRel pour le sens négatif.

					int j = WriteResultChannelProcess.N-1;					
					if (j<0) j += Lise.PicResultLen;
					int CompteurPics = 0;
					int CompteurPicSature = 0;
					while (BufferResultat[j].XAbsN > LiseEd.PulseMoinsRight.AbsN && BufferResultat[j].Sens == false)
					{
						if (Lise.PulsePlusLeft.N > BufferResultat[j].Position)
						{
							BufferResultat[j].XRel = Lise.PulsePlusLeft.N - BufferResultat[j].Position;
						}
						else
						{
							BufferResultat[j].XRel = Lise.PulsePlusLeft.N - BufferResultat[j].Position + Lise.BufferLen;
						}
						if(Lise.NombredeVoie == 2 && BufferResultat[j].Sens == false)
						{
							BufferResultat[j].Position /= 2;
							BufferResultat[j].XAbsN /= 2;
							BufferResultat[j].Qualite *= 2;
							BufferResultat[j].XRel /= 2;
						}
						CompteurPics++;
						j--;
						if (j<0) j += Lise.PicResultLen;
					}
					if(Lise.NombredeVoie == 2)
					{ // Traitement des Xrel pour la voie 2
						int j = Lise.WriteResultSecondChannel.N-1;					
						if (j<0) j += Lise.PicResultLen;
						int CompteurPics = 0;
						int CompteurPicSature = 0;
						while (Lise.BufferResultat[1][j].XAbsN > LiseEd.PulseMoinsRight.AbsN && Lise.BufferResultat[1][j].Sens == false)
						{
							if (Lise.PulsePlusLeft.N > Lise.BufferResultat[1][j].Position)
							{
								Lise.BufferResultat[1][j].XRel = Lise.PulsePlusLeft.N - Lise.BufferResultat[1][j].Position;
							}
							else
							{
								Lise.BufferResultat[1][j].XRel = Lise.PulsePlusLeft.N - Lise.BufferResultat[1][j].Position + Lise.BufferLen;
							}
							if(Lise.NombredeVoie == 2 && Lise.BufferResultat[1][j].Sens == false)
							{
								Lise.BufferResultat[1][j].Position /= 2.0;
								Lise.BufferResultat[1][j].XAbsN /= 2;
								Lise.BufferResultat[1][j].Qualite *= 2.0;
								Lise.BufferResultat[1][j].XRel /= 2.0;
							}
							CompteurPics++;
							j--;
							if (j<0) j += Lise.PicResultLen;
						}
					}
					Lise.SensPositif = true;				
					LiseEd.bPulse = true;
					LiseEd.LtMoins = Lise.PulsePlusLeft.AbsN - LiseEd.PulseMoinsRight.AbsN;
					Lise.NbPicPlusProcessVoie1 = LiseEd.NbrPics;
					Lise.NbPicMoinsProcessVoie1 = Lise.Indice - LiseEd.NbrPics;
					Lise.NbPicPlusProcessVoie2 = LiseEd.iNbrPicsVoie2;
					Lise.NbPicMoinsProcessVoie2 = Lise.iIndiceVoie2 - LiseEd.iNbrPicsVoie2;
 					Lise.Indice = 0;
					Lise.iIndiceVoie2 = 0;

					if (!Lise.FirstPass)
					{
						int RetValue = 0;
						RetValue = ProcessPic(Lise,Lise.BufferResultat[0],WriteResultChannelProcess,0);
						if(RetValue == STATUS_FAIL)
						{
							LogfileF(*Lise.Log,"[LISEED]\tError in ProcessPic - Acquisition Restarted");
							return STATUS_FAIL;
						}
						if(Lise.NombredeVoie == 2)
						{
							RetValue = ProcessPic(Lise,Lise.BufferResultat[1],Lise.WriteResultSecondChannel,1);
							if(RetValue == STATUS_FAIL)
							{
								LogfileF(*Lise.Log,"[LISEED]\tError in ProcessPic - Acquisition Restarted");
								return STATUS_FAIL;
							}
						}
						PicMoyenne(Lise,Lise.BufferResultat[0],WriteResultChannelProcess,0);
						if(Lise.NombredeVoie == 2)
						{
							PicMoyenne(Lise,Lise.BufferResultat[1],Lise.WriteResultSecondChannel,1);
						}

						FindPicSature(Lise,Lise.BufferResultat[0],WriteResultChannelProcess);
						if(Lise.WritePeaksForCalibration == 1)
						{
							if(WriteResultChannelProcess.AbsN > 0)
							{
								WritePeaks(Lise,0,0,Lise.FichierSavePics);
							}
						}
						FindNBestPeak(Lise,WriteResultChannelProcess,LiseEd.bTheoOptRef,LiseEd.fPositionRefOpt,LiseEd.fToleranceRefOpt);

						MemorisationResultatsPeriode(Lise);
						if(Lise.WritePeaksForCalibration == 0 || Lise.WritePeaksForCalibration > 1)
						{
							if(WriteResultChannelProcess.AbsN > 0)
							{
								// fonction pour écrire les pics dans le fichier de save
								WritePeaks(Lise,0,0,Lise.FichierSavePics);

								// toutes les épaisseurs sur une moyenne ne sont pas prêtes
								if(Lise.bNThicknessNotReady == true && Lise.FlagSavePeakMeasured)
								{
									// controler la qualité pour ne pas afficher les pics non valides
									double QualityValue = 0.0;
									int indRes = Lise.IndicePeriod.N;

									// Sens positif
									for(int l = 0;l< NB_PEAK_MAX ;l++)
									{ // on vérifie la qualité des n premiers pics
										QualityValue += Lise.Resultats[indRes].PicsPlusVoie1[l].Qualite;
									}
								}
							}
						}
					}

					// ici pour les incrément de période
					Lise.Periode++;
					RBP_Inc(Lise.IndicePeriod);

					if (!Lise.FirstPass)
					{
						// Calcul des épaisseurs recherchées à partir des pics traouvés et de l'échantillon défini.
						FindThickness(Lise, LiseEd.fPositionRefOpt, LiseEd.fToleranceRefOpt);

						// Ecriture des épaisseurs dans un fichier.
						WriteThickness(Lise);
					}
					// A partir de là on est en début de période, le premier passage est fini.
					Lise.FirstPass = false;
					if(Lise.CompteurDecimation == Lise.IndiceDecimation) Lise.CompteurDecimation = 0;
					Lise.CompteurDecimation++;
				}
			}
		}
        else if( ((LiseEd.fNiveauBas - LiseEd.fTolerance) <= Buffer[i]) && ((LiseEd.fNiveauBas + LiseEd.fTolerance) >= Buffer[i]))
        { // Cas ou l'on est sur le Pulse Moins (present que sur la voie 1)
			if (LiseEd.bFirstPulseSample == true)
			{
				if (Lise.SensPositif == true)
				{
					LiseEd.PossiblePulseMoinsLeft = Lise.Read;
					LiseEd.cptSensDetect = 1;
					LiseEd.bFirstPulseSample = false;
					if(Lise.bDebug == true)
					{		
						if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tFirst sample detected for Pulse Moins Detection");
					}
				}
			}
			else
			{
				LiseEd.cptSensDetect++;
				if (LiseEd.cptSensDetect >= Lise.iNombreEchantillons && LiseEd.bPulseDetection == false)
				{
					if(Lise.bDebug == true)
					{		
						if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tPulse Moins Left Detected");
					}
					LiseEd.PulseMoinsLeft = LiseEd.PossiblePulseMoinsLeft;
					LiseEd.bPulse = true;
					LiseEd.cptSensDetect = 0;
					LiseEd.bPulseDetection = true;
					Lise.FirstPass = false;
					Lise.SensPositif = false;
					LiseEd.iNbrPicsVoie2 = Lise.iIndiceVoie2;
					LiseEd.NbrPics = Lise.Indice;				
					LiseEd.LtPlus = LiseEd.PulseMoinsLeft.AbsN - Lise.PulsePlusRight.AbsN;
				}
			}
		}
		else
		{
			if (LiseEd.bPulseDetection)
			{
				if (Lise.SensPositif)
				{ // Memorisation de Pulse Plus Right
					Lise.PulsePlusRight = Lise.Read;
					if(Lise.bDebug == true)
					{		
						if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tPulse Plus Right Detected");
					}

				}
				else
				{ // Memorisation de Pulse Moins Right
					LiseEd.PulseMoinsRight = Lise.Read;
					if(Lise.bDebug == true)
					{		
						if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tPulse Moins Right Detected");
					}
				}
				LiseEd.bPulseDetection = false;
			}
			LiseEd.bFirstPulseSample = true;
			LiseEd.bPulse = false;
			// Mise à jour de la ligne de base.
			if (Buffer[i] < Lise.LigneDeBase + LiseEd.fTolerance && Buffer[i] > Lise.LigneDeBase - LiseEd.fTolerance) 
			{
				Lise.LigneDeBase = (Lise.LigneDeBase * 255.0 + Buffer[i]) / 256.0;
			}
		}

	return STATUS_OK;
}

// fonction pour la détection de pics
int GetPic(LISE_ED& LiseEd)
{
	DbgCHECK(_CrtCheckMemory()!=TRUE,"GetPic Enter");

	SPG_MemFastCheck();

	BufferMoyenne(LiseEd.Lise,LiseEd.Lise.BufferIntensity,LiseEd.Lise.WriteResult);

	while(LiseEd.Lise.Read.AbsN<=LiseEd.Lise.Write.AbsN-LiseEd.Lise.fitLen)
    {
		int Voie = 0;
		double* Buffer;
		PICRESULT* BufferResultat;
		RING_BUFFER_POS WriteResultChannelProcess;

		if(LiseEd.Lise.NombredeVoie == 2)
		{ // Cas ou l'on a deux voie - A revoir

			if(LiseEd.Lise.Read.N % 2 == 0)
			{
				Voie = 0;
			}
			if(LiseEd.Lise.Read.N % 2 == 1)
			{
				Voie = 1;
			}
			// On crée un index de Buffer circulaire qui va prendre en compte le Buffer de resultat de la voie un ou la voie deux
	
			if(Voie == 0)
			{ // Si on traite la voie un, alors on y associe son buffer circulaire
				WriteResultChannelProcess = LiseEd.Lise.WriteResult;
			}
			else if(Voie == 1)
			{ // Si on traite la voie deux, alors on y associe son buffer circulaire
				WriteResultChannelProcess = LiseEd.Lise.WriteResultSecondChannel;
			}
			// On recupère les différents Buffer contenant le signal
			Buffer = LiseEd.Lise.BufferIntensity;
			// Buffer de resultat en fonction de la voie
			BufferResultat = LiseEd.Lise.BufferResultat[Voie];
		}
		else
		{ // Cas ou l'on a une voie et tous les autres cas
			// Buffer de resultat en fonction de la voie.
			WriteResultChannelProcess = LiseEd.Lise.WriteResult;
			Buffer = LiseEd.Lise.BufferIntensity;
			BufferResultat = LiseEd.Lise.BufferResultat[0];
			Voie = 0;
		}
		// Detection des débuts et fin d'acquisition 
		if(Voie == 0)
		{
			int RetValue = 0;
			RetValue = LEDPulseDetection(LiseEd.Lise,LiseEd,BufferResultat,Buffer,LiseEd.Lise.WriteResult,0);
			if(RetValue == STATUS_FAIL)
			{
				LogfileF(*LiseEd.Lise.Log,"[LISEED]\tError in Pulse detection - Acquisition Retarted");
				return STATUS_FAIL;
			}

			RetValue = 0;
			if(!LiseEd.bPulse)
			{
				if (!LiseEd.bPulseDetection)
				{
					RetValue = PicDetection(LiseEd.Lise,BufferResultat,Buffer,LiseEd.Lise.WriteResult,0, 0);
				}
				// faire tout le process picdetection et pic moyenne ...

			}
			if(RetValue == STATUS_FAIL)
			{
				LogfileF(*LiseEd.Lise.Log,"[LISEED]\tError in PicDetection - Acquisition Restarting");
				return STATUS_FAIL;
			}
			if(LiseEd.Lise.NombredeVoie == 2)
			{
				if(!LiseEd.bPulse && !LiseEd.bPulseDetection)
				{
					RetValue = PicDetection(LiseEd.Lise,LiseEd.Lise.BufferResultat[1],Buffer,LiseEd.Lise.WriteResultSecondChannel,1,1);
				}
				if(RetValue == STATUS_FAIL)
				{
					LogfileF(*LiseEd.Lise.Log,"[LISEED]\tError in PicDetection - Acquisition Retarting");
					return STATUS_FAIL;
				}
			}
		}
		/*else if (Voie == 1)
		{
			PicDetection(Lise,BufferResultat,Buffer,Lise.WriteResultSecondChannel,Voie,1);
		}*/

		RBP_Add(LiseEd.Lise.Read,1);
		if(LiseEd.Lise.NombredeVoie == 2)
		{
			if(LiseEd.Lise.Read.N % 2 == 0 && LiseEd.Lise.Read.AbsN !=0)
			{
				RBP_Inc(LiseEd.Lise.ReadCrossBuffer);
			}
		}
	} // Fin de la boucle

	/*
	Lise.Trailer=Lise.Read;
	if(Lise.NombredeVoie == 2)
	{
		if(Lise.Read.AbsN % 2 == 1)
		{
			RING_BUFFER_POS Indice = Lise.Read;
			RBP_Dec(Indice);
			Lise.Trailer = Indice;
		}
	}
	CHECK(Lise.Trailer.AbsN<=Lise.Read.AbsN-Lise.BufferLen,"LiseEDAcqProcessSamples: Peak larger than ring buffer",Lise.Trailer=Lise.Read);
	*/

	RING_BUFFER_POS IndiceTemp = LiseEd.Lise.PulsePlusLeft;
	RBP_Sub(IndiceTemp,LiseEd.Lise.NbSamplesLastPeriod);
	LiseEd.Lise.Trailer = IndiceTemp;

	if(LiseEd.Lise.NombredeVoie == 2)
	{
		if(LiseEd.Lise.Read.AbsN % 2 == 1)
		{
			//RING_BUFFER_POS Indice = Lise.PulsePlusLeft - Lise.NbSamplesLastPeriod;
			RING_BUFFER_POS Indice = LiseEd.Lise.PulsePlusLeft ;
			RBP_Sub(LiseEd.Lise.PulsePlusLeft,LiseEd.Lise.NbSamplesLastPeriod);
			RBP_Dec(Indice);
			LiseEd.Lise.Trailer = Indice;
		}
	}
	CHECK(LiseEd.Lise.Trailer.AbsN<=LiseEd.Lise.Read.AbsN-LiseEd.Lise.BufferLen,"LiseEDAcqProcessSamples: big problem, read is beyond trailer", return STATUS_FAIL);

	SPG_MemFastCheck();

	DbgCHECK(_CrtCheckMemory()!=TRUE,"GetPic Exit");

	return STATUS_OK;
}

// fonction pour trouver les pics dans le buffer de résultats
int FindPicInBuffer(LISE_ED& LiseEd,int voie)
{
	return GetPic(LiseEd);
}

//_______________________________________________________________________________________________ AutoGain Mode
// Cette fonction permet d'effectuer un AutoGain en mode SingleShot
int ProcessAutoGain_SingleShot_Mode(LISE_ED* LiseEd){

	if (!LiseEd) { return DLL_FAIL; }

	// En mode SingleShot, on teste au préalable si un autoGain est nécessaire, on ne touche pas au booléen
	if(LiseEd->Lise.bDebug)logF(LiseEd, "[ProcessAutoGain_SingleShot_Mode]\t Enter function");

	// On teste dans un premier temps le gain actuel
	bool bQualityGoodEnough = false;
	// on récupère la dernière mesure valide
	RING_BUFFER_POS IndexMoyenne = LiseEd->Lise.IndicePeriod;
	RBP_Dec(IndexMoyenne);
	PERIOD_RESULT LastNMeasure = LiseEd->Lise.Resultats[IndexMoyenne.N];

	if(LastNMeasure.fQuality >= LiseEd->Lise.fQualityThreshold){
		bQualityGoodEnough = true;
	}

	if(!bQualityGoodEnough){
		// On procède à l'autoGain
		double dGainValue = ProcessAutoGain(LiseEd);

		if(dGainValue >= 0.0){
			logF(LiseEd, "[ProcessAutoGain_SingleShot_Mode]\t ProcessAutoGain result = %f",(float)dGainValue);
		}
		else
		{
			// Erreur dans la fonction ProcessAutoGain
			logF(LiseEd, "[ProcessAutoGain_SingleShot_Mode]\t Error in ProcessAutoGain!");
			if(LiseEd->Lise.bDebug)logF(LiseEd, "[ProcessAutoGain_SingleShot_Mode]\t Exit function");
			return DLL_FAIL;
		}
	}
	else{
		logF(LiseEd, "[ProcessAutoGain_SingleShot_Mode]\t Gain test succeed. ProcessAutoGain has not been performed");
	}

	if(LiseEd->Lise.bDebug)logF(LiseEd, "[ProcessAutoGain_SingleShot_Mode]\t Exit function");
	return FP_OK;
}
// Cette fonction permet d'effectuer un AutoGain en mode continuous
int ProcessAutoGain_Continuous_Mode(LISE_ED* LiseEd){

	if (!LiseEd) { return DLL_FAIL; }
	// En mode continu ou stop, on effectue systématiquement l'autoGain
	if(LiseEd->Lise.bDebug)logF(LiseEd, "[ProcessAutoGain_Continuous_Mode]\t Enter function");

	// Si la probe n'est pas started, on la passe en continuous
	ACQUISITIONMODE ModeEdBeforeAutoGain = SingleShot;
	MODE ModeProbeBeforeAutoGain = Measurement;
	
	// On récupère l'état de la probe courante pour pouvoir l'appliquer à nouveau en fin de mesure
	ModeEdBeforeAutoGain = LiseEd->Lise.AcqMode;
	ModeProbeBeforeAutoGain = LiseEd->AcquisitionMode;

	if(ModeProbeBeforeAutoGain == Stopped){
		StartAcquisition(*LiseEd);
		Sleep(100);
	}

	// On procède à l'autoGain
	double dGainValue = ProcessAutoGain(LiseEd);

	// On repasse le booleen à False
	LiseEd->bUseAutoGain = false;

	// On se remet dans l'état précédent

	if(dGainValue >= 0.0){
		logF(LiseEd, "[ProcessAutoGain_Continuous_Mode]\t ProcessAutoGain result = %f",(float)dGainValue);
	}
	else
	{
		// Erreur dans la fonction ProcessAutoGain
		logF(LiseEd, "[ProcessAutoGain_Continuous_Mode]\t Error in ProcessAutoGain!");
		if(LiseEd->Lise.bDebug)logF(LiseEd, "[ProcessAutoGain_Continuous_Mode]\t Exit function");
		return DLL_FAIL;
	}
	if(LiseEd->Lise.bDebug)logF(LiseEd, "[ProcessAutoGain_Continuous_Mode]\t Exit function");

	return FP_OK;
	
	// Old_Code
	/*if (!LiseEd) { return DLL_FAIL; }

	// En mode continu, on effectue systématiquement l'autoGain et on repasse le booléen à False une fois terminé
	if(LiseEd->Lise.bDebug)logF(LiseEd, "[ProcessAutoGain_Continuous_Mode]\t Enter function");

	// On procède à l'autoGain
	double dGainValue = ProcessAutoGain(LiseEd,Thickness, Quality,NbThickness);

	// On repasse le booleen à False
	LiseEd->bUseAutoGain = false;
	
	if(dGainValue >= 0.0){
		logF(LiseEd, "[ProcessAutoGain_Continuous_Mode]\t ProcessAutoGain result = %f",(float)dGainValue);
	}
	else
	{
		// Erreur dans la fonction ProcessAutoGain
		logF(LiseEd, "[ProcessAutoGain_Continuous_Mode]\t Error in ProcessAutoGain!");
		if(LiseEd->Lise.bDebug)logF(LiseEd, "[ProcessAutoGain_Continuous_Mode]\t Exit function");
		return DLL_FAIL;
	}
	if(LiseEd->Lise.bDebug)logF(LiseEd, "[ProcessAutoGain_Continuous_Mode]\t Exit function");

	return FP_OK;*/
}
// Fonction de recherche du meilleur gain
double ProcessAutoGain(LISE_ED* LiseEd){

	// Precondition
	if (!LiseEd) { return -1.0; }

	if(LiseEd->Lise.bDebug)	LogfileF(*LiseEd->Lise.Log,"[ProcessAutoGain]\tEnter Function");
	// Inits
	double dBackUpGainValue = LiseEd->Lise.fSourceValue;
	double dGainValue = 0.0;
	double StepSize = LiseEd->Lise.AutoGainStep;	// En volts a definir dans les params
	int TotalSteps = (int)((LiseEd->Lise.GainMax - LiseEd->Lise.GainMin) / StepSize);
	double* QualityTab = (double*)malloc(TotalSteps*sizeof(double));;
	memset(QualityTab,0.0,TotalSteps*sizeof(double));
	LogfileF(*LiseEd->Lise.Log,"[ProcessAutoGain]\tNum Steps = %i, Step Value %f, Gain Max %f, Gain Min %f",TotalSteps,(float)StepSize,(float)LiseEd->Lise.GainMax,(float)LiseEd->Lise.GainMin);

	// Changement de la fréquence d'acquisition

	// Recherche sur toute la plage
	double MaxQuality = 0.0;
	for(int iNumStep = 0; iNumStep < TotalSteps; iNumStep++){
		double dGain = iNumStep * StepSize;
		
		// On applique le Gain
		LiseEd->Lise.fSourceValue = dGain;
		LEDISetSourcePower(LiseEd,LiseEd->Lise.fSourceValue);

		// Tempo pour stabilisation du gain
		Sleep(37);	// Temps de 2 allers/retours chariots 55 Hz 

		// On lit la valeur de la qualité courante
		//double CurrentQuality = 0.0;

		// on récupère la dernière mesure valide
		RING_BUFFER_POS IndexMoyenne = LiseEd->Lise.IndicePeriod;
		// on commence par décrémenter l'index, bon pour la première mesure car la première mesure valide est a GetNMinusOne 
		RBP_Dec(IndexMoyenne);
		PERIOD_RESULT LastNMeasure = LiseEd->Lise.Resultats[IndexMoyenne.N];

		QualityTab[iNumStep] = LastNMeasure.fQuality;

		// On logue les valeurs
		LogfileF(*LiseEd->Lise.Log,"[ProcessAutoGain]\tStep number %i : Gain Value = %f, Quality Value = %f",iNumStep,(float)(dGain),(float)QualityTab[iNumStep]);
	
		// On récupère la valeur du Max et l'indice associé
		if(QualityTab[iNumStep] > MaxQuality)
		{
			MaxQuality = QualityTab[iNumStep];
			dGainValue = dGain;
		}
	}

	// Detection du meilleur gain
	/*double MaxQuality = 0.0;
	for(int iNumStep = 0; iNumStep < TotalSteps; iNumStep++){
		
		// On récupère la valeur du Max et l'indice associé
		if(QualityTab[iNumStep] > MaxQuality)
		{
			MaxQuality = QualityTab[iNumStep];
			dGainValue = iNumStep*StepSize;
		}
	}*/
	LogfileF(*LiseEd->Lise.Log,"[ProcessAutoGain]\tMax Quality = %f, Associated Gain = %f",(float)MaxQuality,(float)dGainValue);

	// Desallocations
	free(QualityTab);

	if(dGainValue == 0){
		// Si l'on n'a pas trouvé de meilleur gain, on retourne un message d'erreur
		LogfileF(*LiseEd->Lise.Log,"[ProcessAutoGain]\tNo best gain detected");
	}
	else{
		// Changement du Gain
		LiseEd->Lise.fSourceValue = dGainValue;
		LEDISetSourcePower(LiseEd,LiseEd->Lise.fSourceValue);
	}

	// Remise de la fréquence d'acquisition précédente

	// Retour de la valeur du gain
	if(LiseEd->Lise.bDebug)	LogfileF(*LiseEd->Lise.Log,"[ProcessAutoGain]\tLeave Function");
	return dGainValue;
}