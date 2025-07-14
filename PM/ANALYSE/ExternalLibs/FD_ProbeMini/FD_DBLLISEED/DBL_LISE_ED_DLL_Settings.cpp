
#include <windows.h>
#include <stdio.h>
#include <shlwapi.h>
#include <string.h>

// ## probe-common headers ##
#include "..\SrcC\SPG.h"
#include "..\SrcC\BreakHook.h"
#include "../FD_FogaleProbe/NIDAQmxConfig.h"
#include "../FD_FogaleProbe/FogaleProbeReturnValues.h"
// ## probe-common headers ##

// ## probe-specific headers ##
#include "../FD_LISE_General/LISE_Consts.h"
#include "../FD_LISE_General/PeakMatch.h"
#include "../FD_LISE_General/LISE_Struct_Process.h"
#include "../FD_LISE_General/LISE_Struct.h"

#include "../FD_LISEED/LISE_ED_DLL_UI_Struct.h"
#include "../FD_LISEED/LISE_ED_DLL_Internal.h"
#include "../FD_LISEED/LISE_ED_DLL_Log.h"

// ## probe-specific headers ##
#include "DBL_LISE_ED_DLL_Internal.h"
#include "DBL_LISE_ED_DLL_Log.h"
#include "DBL_LISE_ED_DLL_Settings.h"



#define MAX_NUM_TESTS 20

// fonction pour faire une calibration dbl lise
int DBL_CalibrateDblLise(void* s,double* CalibrationArray)
{
#pragma region New_Code_07_2010

	//_________________________________________________ Preconditions, préparation du Lise ED ___________________________________________________________//
	if(!s)
	{
		return FP_FAIL;
	}

	SPG_MemFastCheck();
	// On récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;
    
	// Initialisation de la calibration comme étant non annulé
	DblLiseEd->bCalibrationCancel = false;

	// On attend que la probe soit liberee
	/*int iTimeOut = 0;
	while(DblLiseEd->bProbeRessourceReserved && (iTimeOut < 10)){
		Sleep(100);
		iTimeOut ++;
	}
	if(iTimeOut >= 10){
		return FP_BUSY;
	}
	DblLiseEd->bProbeRessourceReserved = true;*/
	/*if(WaitForSingleObject(DblLiseEd->ProbeMutex,1000)==0){
		return FP_BUSY;
	}*/

	LogDblED(*DblLiseEd,PRIO_INFO,"[DBL_CalibrateDblLise] Enter Double Lise Calibration Function");
	
	// on taggue la probe en calibration
	DblLiseEd->bProbeInCalibration = true;

	// Entier pour un état de probe 0 = stopped / 1 = Upper probe started / 2 = Lower probe started
	int ProbeNum = 0;
	ACQUISITIONMODE ModeDblEdBeforeCalib = SingleShot;
	MODE ModeProbeBeforeCalib = Measurement;
	bool bAutoGainState0 = DblLiseEd->LiseEd[0].bUseAutoGain;
	bool bAutoGainState1 = DblLiseEd->LiseEd[1].bUseAutoGain;
	DblLiseEd->LiseEd[0].bUseAutoGain = false;
	DblLiseEd->LiseEd[1].bUseAutoGain = false;

	// On récupère l'état de la probe courante pour pouvoir l'appliquer à nouveau en fin de mesure
	// modif MP/MA du 09/09/2010
	ProbeNum = DblLiseEd->iCurrentProbe;	// Sauvegarde de la probe courante
	ModeDblEdBeforeCalib = DblLiseEd->LiseEd[DblLiseEd->iCurrentProbe].Lise.AcqMode;
	ModeProbeBeforeCalib = DblLiseEd->LiseEd[DblLiseEd->iCurrentProbe].AcquisitionMode;

	// On memorise le total th
	double TempCalib = DblLiseEd->dTotalThickness;
	DblLiseEd->dTotalThickness = 10000;

	// Ainsi que les autres params de calibration
	double TempLower = DblLiseEd->dCalibrateLowerAirgap;// Airgap de dessous mesuré
	double TempUpper = DblLiseEd->dCalibrateUpperAirgap;// Airgap de dessus mesuré
	double TempThUsed = DblLiseEd->dCalibrateThicknessUsed;// Epaisseur mesurée durant calibration

	// Init des tableux
	double CalibThickness[MAX_THICKNESS];	memset(CalibThickness,0,MAX_THICKNESS*sizeof(double));
	double CalibTolerance[MAX_THICKNESS];	memset(CalibTolerance,0,MAX_THICKNESS*sizeof(double));
	double CalibIndex[MAX_THICKNESS];		memset(CalibIndex,0,MAX_THICKNESS*sizeof(double));
	double CalibIntensity[MAX_THICKNESS];	memset(CalibIntensity,0,MAX_THICKNESS*sizeof(double));

	// Définition des couches
	int NbThickness = 0;
	int IndexLowerAG = 4;
	if(DblLiseEd->iCalibrationMode == 3)	// Calibration de l'etalon
	{
		NbThickness = 3;
		IndexLowerAG = 2;
		CalibThickness[0] = 3000.0;					CalibTolerance[0] = 3000.0;					CalibIndex[0] = 1.0;					CalibIntensity[0] = 0.0;
		CalibThickness[1] = 3000.0;					CalibTolerance[1] =	3000.0;					CalibIndex[1] = 0.0;					CalibIntensity[1] = 0.0;
		CalibThickness[IndexLowerAG] = 3000.0;		CalibTolerance[IndexLowerAG] = 3000.0;		CalibIndex[IndexLowerAG] = 1.0;			CalibIntensity[IndexLowerAG] = 0.0;
	}
	else
	{
		NbThickness = 5;
		IndexLowerAG = 4;
		CalibThickness[0] = 3000.0;					CalibTolerance[0] = 3000.0;					CalibIndex[0] = 1.0;					CalibIntensity[0] = 0.0;
		CalibThickness[1] = CalibrationArray[0];	CalibTolerance[1] =	CalibrationArray[1];	CalibIndex[1] = CalibrationArray[2];	CalibIntensity[1] = 0.0;
		CalibThickness[2] = 3000.0;					CalibTolerance[2] = 3000.0;					CalibIndex[2] = 0.0;					CalibIntensity[2] = 0.0;
		CalibThickness[3] = CalibrationArray[0];	CalibTolerance[3] =	CalibrationArray[1];	CalibIndex[3] = CalibrationArray[2];	CalibIntensity[3] = 0.0;
		CalibThickness[IndexLowerAG] = 3000.0;		CalibTolerance[IndexLowerAG] = 3000.0;		CalibIndex[IndexLowerAG] = 1.0;			CalibIntensity[IndexLowerAG] = 0.0;
	}

	// init du tableau de gain
	double GainValue[2]; memset(GainValue,0,2*sizeof(double));
	GainValue[0] = CalibrationArray[3];
	GainValue[1] = CalibrationArray[4];

	

	double Qualitythreshold[2] ; memset(Qualitythreshold,0,2*sizeof(double));
	try{
		Qualitythreshold[0] = CalibrationArray[5];
		Qualitythreshold[1] = CalibrationArray[6];
	}
	catch(...){
		LogDblED(*DblLiseEd,PRIO_ERROR,"[DBL_CalibrateDblLise] Calibration array is false, you probably use a wrong version of Fogale Probe");
	}
	// Define sample 
	int RetDefine = FP_OK;
	int nbDefineSampleRepeat = 0;

	// Modif MP 09/09/10: Reflechir a effectuer le test en amont
	do
	{
		RetDefine = DBL_LEDIDefineSampleDouble(s,"Calib_sample","",CalibThickness,CalibTolerance,CalibIndex,CalibIntensity,NbThickness,GainValue,Qualitythreshold);
		nbDefineSampleRepeat ++;
	}
	while((RetDefine != FP_OK)&&(nbDefineSampleRepeat < 3));	// On effectue 3 essais au maximum

	if(nbDefineSampleRepeat > 2)
	{
		DblLiseEd->dTotalThickness = TempCalib;
		LogDblED(*DblLiseEd,PRIO_ERROR,"[DBL_CalibrateDblLise] Failed to define sample: exit the calibration");
		SetSystemMode(s,ModeProbeBeforeCalib,ProbeNum ,ModeDblEdBeforeCalib);
		DblLiseEd->LiseEd[0].bUseAutoGain = bAutoGainState0;
		DblLiseEd->LiseEd[1].bUseAutoGain = bAutoGainState1;
		return FP_CALIB_FAILED_DEFINE_SAMPLE;
	}
	
	SPG_MemFastCheck();

	// Start de la probe dans son bon mode probe
	if(ModeProbeBeforeCalib==Stopped)
	{
		if(DBL_LEDIStartSingleShotAcq(s) != FP_OK)
		{
			DblLiseEd->dTotalThickness = TempCalib;
			LogDblED(*DblLiseEd,PRIO_ERROR,"[DBL_CalibrateDblLise] Failed to start probe: exit the calibration");
			
			// on se remet dans le mode dans lequel on etait entre
			SetSystemMode(s,ModeProbeBeforeCalib,ProbeNum ,ModeDblEdBeforeCalib);
			DblLiseEd->LiseEd[0].bUseAutoGain = bAutoGainState0;
			DblLiseEd->LiseEd[1].bUseAutoGain = bAutoGainState1;
			return FP_FAILED_START_PROBE;
		}
	}
	
	// On se place sur probe zéro si ce n'est pas le cas
	LogDblED(*DblLiseEd,PRIO_INFO,"[DBL_CalibrateDblLise] Select first probe");
	if(DBL_SetCurrentProbe(s,0,false) != FP_OK)
	{ 
		DblLiseEd->dTotalThickness = TempCalib;
		LogDblED(*DblLiseEd,PRIO_ERROR,"[DBL_CalibrateDblLise] Failed to select probe: exit the calibration");
		
		// on se remet dans le mode dans lequel on etait entre
		SetSystemMode(s,ModeProbeBeforeCalib,ProbeNum ,ModeDblEdBeforeCalib);
		DblLiseEd->LiseEd[0].bUseAutoGain = bAutoGainState0;
		DblLiseEd->LiseEd[1].bUseAutoGain = bAutoGainState1;

		return FP_SELECTCHANNEL_FAILED;
	}

	int maxFailed = 5;
	int nbFailed = 0;
	// nbRepeat GetThickness en double probe
	LogDblED(*DblLiseEd,PRIO_INFO,"[DBL_CalibrateDblLise] Start %i get thickness",DblLiseEd->iNbCalibrationRepeat);
	
	// Init des variables
	double ThicknessMeasured[MAX_NUM_TESTS][MAX_THICKNESS];		// Contiendra toutes les acquisitions de la calibration
	double QualityMeasured[MAX_NUM_TESTS][MAX_THICKNESS];
	memset(ThicknessMeasured,0.0, sizeof(double) * MAX_NUM_TESTS * MAX_THICKNESS);
	memset(QualityMeasured,0.0,sizeof(double) * MAX_NUM_TESTS * MAX_THICKNESS);

	if(DblLiseEd->iNbCalibrationRepeat >= MAX_NUM_TESTS)
		DblLiseEd->iNbCalibrationRepeat = MAX_NUM_TESTS-1;

	//_________________________________________________ Début de l'acquisition ___________________________________________________________//

	int RetGetTh = FP_FAIL;

	double dAG1_tab[MAX_NUM_TESTS];memset(dAG1_tab,0,MAX_NUM_TESTS*sizeof(double));
	double dAG2_tab[MAX_NUM_TESTS];memset(dAG2_tab,0,MAX_NUM_TESTS*sizeof(double));
	
	for(int index = 0 ; index < DblLiseEd->iNbCalibrationRepeat;index ++)
	{	
		// Verification si on a annulé la calibration
		if(DblLiseEd->bCalibrationCancel == true)
		{
			LogDblED(*DblLiseEd,PRIO_ERROR,"[DBL_CalibrateDblLise] Calibration canceled -> Exit calibration");
			return FP_CALIB_CANCELED;
		}

		// On va moyenner l'acquisition sur nbRepeats
		RetGetTh = DBL_LEDIGetThickness(s,ThicknessMeasured[index],QualityMeasured[index],NbThickness);
      	if((RetGetTh != FP_OK)||(QualityMeasured[index][0] < Qualitythreshold[0])) 
		{	
			// Fail get thickness ou quality trop faible
			LogDblED(*DblLiseEd,PRIO_ERROR,"[DBL_CalibrateDblLise] Get thickness failed: Function return = %i, Quality = %f",RetGetTh,QualityMeasured);
			nbFailed ++;			// On incrémente le nombre d'essais ratés
			index--;				// On recommence à l'index précédent
		}

		// trop d'erreur on sort ressort
		if(nbFailed > maxFailed)
		{	
			// Trop d'erreurs
			LogDblED(*DblLiseEd,PRIO_ERROR,"[DBL_CalibrateDblLise] Exit: Get thickness failed %i times!",maxFailed);
			DBL_LEDIAcqStop(s);

			// on se remet dans le mode dans lequel on est entre et l'ancienne valeur de calib
			DblLiseEd->dTotalThickness = TempCalib;
			SetSystemMode(s,ModeProbeBeforeCalib,ProbeNum ,ModeDblEdBeforeCalib);

			DblLiseEd->LiseEd[0].bUseAutoGain = bAutoGainState0;
			DblLiseEd->LiseEd[1].bUseAutoGain = bAutoGainState1;

			return FP_CALIB_GET_THICK_FAILED;
		}

		// On calcule les moyennes d'airgap de façon à pouvoir discriminer les airgaps defaillants
		if((index >= 0)&&(index < MAX_NUM_TESTS))
		{
			dAG1_tab[index] = (double)ThicknessMeasured[index][0];
			dAG2_tab[index] = (double)ThicknessMeasured[index][IndexLowerAG];
		}
	}

	double Tolerance_AG1 = 10;
	double Tolerance_AG2 = 10;
	double AG1_Med = 0.0;
	double AG2_Med = 0.0;

	// On calcule les tolerances en triant les tableaux de maniere a recuperer la mediane
	int pi, mi;
	for(mi=0;mi<DblLiseEd->iNbCalibrationRepeat;++mi)
	{
		for(pi=0;pi<DblLiseEd->iNbCalibrationRepeat-1;pi++)
		{
			double temp = 0;
			if(dAG1_tab[pi] > dAG1_tab[pi+1])
			{
				temp = dAG1_tab[pi];
				dAG1_tab[pi] = dAG1_tab[pi+1];
				dAG1_tab[pi+1] = temp;
			}
			if(dAG2_tab[pi] > dAG2_tab[pi+1])
			{
				
				temp = dAG2_tab[pi];
				dAG2_tab[pi] = dAG2_tab[pi+1];
				dAG2_tab[pi+1] = temp;
			}
		}
	}
	// Tableaux tries par ordre croissant, on prend la mediane
	AG1_Med = dAG1_tab[(int)(DblLiseEd->iNbCalibrationRepeat/2)];
	AG2_Med = dAG2_tab[(int)(DblLiseEd->iNbCalibrationRepeat/2)];

	// On a effectué nbRepeats
	LogDblED(*DblLiseEd,PRIO_INFO,"[DBL_CalibrateDblLise] Get thickness successed %i times!", DblLiseEd->iNbCalibrationRepeat);

	// On force l'arrêt de la current probe
	LogDblED(*DblLiseEd,PRIO_INFO,"[DBL_CalibrateDblLise] Stop current probe");
	if(DBL_LEDIAcqStop(s) != FP_OK)	LogDblED(*DblLiseEd,PRIO_ERROR,"[DBL_CalibrateDblLise] Stop current probe failed");

	// On se remet dans l'état dans lequel on avait trouvé la probe à son entrée
	SetSystemMode(s,ModeProbeBeforeCalib,ProbeNum ,ModeDblEdBeforeCalib);
	DblLiseEd->LiseEd[0].bUseAutoGain = bAutoGainState0;
	DblLiseEd->LiseEd[1].bUseAutoGain = bAutoGainState1;
	
	//_________________________________________________ Post traitement ___________________________________________________________//


	int nbErrors = 0;  
	int iIndex = 0;											// index != i car on peut avoir des résultats faux
	// Variables de post traitement
	double Min = 10000000.0;
	double Max = -Min; 
	double dTotalThicknessMoy = 0.0;
	// Moyenne des résultats de toutes les acquisitions
	double ThicknessMeasuredMoy[MAX_THICKNESS];	memset(ThicknessMeasuredMoy,0,MAX_THICKNESS*sizeof(double));
	double QualityMeasuredMoy[MAX_THICKNESS];		memset(QualityMeasuredMoy,0,MAX_THICKNESS*sizeof(double));

	double dTotalThickness_tab[MAX_NUM_TESTS];						// Contiendra tous les résultats de calibration
	memset(dTotalThickness_tab,0,MAX_NUM_TESTS*sizeof(double));
	double ThicknessUsed[MAX_NUM_TESTS];							// Variable temporaire
	memset(ThicknessUsed,0,MAX_NUM_TESTS*sizeof(double));
	double ThicknessUsedMoy = 0.0;									// Variable renvoyee

	iIndex = 0;
	for(int i = 0 ; i < DblLiseEd->iNbCalibrationRepeat;i ++)
	{		
		// On va mesurer toutes les TotalThicknesses possibles
		dTotalThickness_tab[iIndex] = 0.0;
		
		switch(DblLiseEd->iCalibrationMode)
		{
			case 0:				// Mode 0: Valeur théorique de calibration
				ThicknessUsed[iIndex] = CalibrationArray[0];
				dTotalThickness_tab[iIndex] = ThicknessMeasured[i][0] + ThicknessMeasured[i][IndexLowerAG] + ThicknessUsed[iIndex];
				break;
			case 1:				// Mode 1: Valeur Mesurée
				ThicknessUsed[iIndex] = (ThicknessMeasured[i][1] + ThicknessMeasured[i][3])/2.0;
				dTotalThickness_tab[iIndex] = ThicknessMeasured[i][0] + ThicknessMeasured[i][IndexLowerAG] + ThicknessUsed[iIndex];
				break;
			case 2:				// Mode 2: Moyenne des 3 valeurs
				ThicknessUsed[iIndex] = (ThicknessMeasured[i][1] + ThicknessMeasured[i][3] + CalibrationArray[0])/3.0;
				dTotalThickness_tab[iIndex] = ThicknessMeasured[i][0] + ThicknessMeasured[i][IndexLowerAG] + ThicknessUsed[iIndex];
				break;
			case 3:				// Mode calibration de l'étalon plan
				ThicknessUsed[iIndex] = CalibrationArray[0];
				dTotalThickness_tab[iIndex] = ThicknessMeasured[i][0] + ThicknessMeasured[i][IndexLowerAG] + ThicknessUsed[iIndex];
				break;
			default:			// Mode par defaut
				ThicknessUsed[iIndex] = 0;
				dTotalThickness_tab[iIndex] = 0;
			break;
		}

		// Detection d'une mauvaise mesure: Cas ou la thickness mesurée est différentes de la tolérance
		if((DblLiseEd->iCalibrationMode != 3) &&
			((abs(ThicknessMeasured[i][1] - CalibThickness[1]) >  CalibTolerance[1]) ||
		(abs(ThicknessMeasured[i][3] - CalibThickness[3]) >  CalibTolerance[3])) )
		{
			// On logue la fausse valeur
			LogDblED(*DblLiseEd,PRIO_WARNING,"[DBL_CalibrateDblLise] thickness measure under tolerance, Probe 1: %f, Probe 2: %f", ThicknessMeasured[i][1],ThicknessMeasured[i][3]);
			nbErrors ++;
			iIndex --;								// On ecrasera la fausse valeur au prochain coup
		}
		else if(((abs(ThicknessMeasured[i][0] - AG1_Med) >  Tolerance_AG1)|| 
		(abs(ThicknessMeasured[i][IndexLowerAG] - AG2_Med) >  Tolerance_AG2)) )
		{
			// On logue la fausse valeur
			LogDblED(*DblLiseEd,PRIO_WARNING,"[DBL_CalibrateDblLise] Airgap tolerance crossed AG1: %f, AG1_Med: %f, AG1_Tol: %f, AG2: %f, AG2_Med: %f, AG2_Tol: %f", ThicknessMeasured[i][0],AG1_Med,Tolerance_AG1,ThicknessMeasured[i][4],AG2_Med,Tolerance_AG2);
			nbErrors ++;
			iIndex --;								// On ecrasera la fausse valeur au prochain coup
		
		}
		else
		{
			// On fait la moyenne des mesures
			for(int t = 0 ; t < MAX_THICKNESS;t++)
			{
				ThicknessMeasuredMoy[t] += ThicknessMeasured[i][t];
				QualityMeasuredMoy[t] += QualityMeasured[i][t];
			}

			// On recupere le thickness used
			ThicknessUsedMoy += ThicknessUsed[iIndex];
			dTotalThicknessMoy += dTotalThickness_tab[iIndex];

			// On récupère le max et le min
			if(dTotalThickness_tab[iIndex]>Max) Max = dTotalThickness_tab[iIndex];
			if(dTotalThickness_tab[iIndex]<Min) Min = dTotalThickness_tab[iIndex];
		}

		iIndex ++;
	}
	iIndex --;

	// Calcul des moyennes
	if(iIndex >= 0)
	{
		dTotalThicknessMoy = dTotalThicknessMoy/((double)iIndex +1);
		ThicknessUsedMoy = ThicknessUsedMoy/((double)iIndex +1);

		for(int t = 0 ; t < MAX_THICKNESS;t++)
		{
			ThicknessMeasuredMoy[t] = ThicknessMeasuredMoy[t]/((double)iIndex +1);
			QualityMeasuredMoy[t] = QualityMeasuredMoy[t]/((double)iIndex +1);
		}
	}
	else
	{
		ThicknessUsedMoy = 0.0;
		dTotalThicknessMoy = 0.0;
		for(int t = 0 ; t < MAX_THICKNESS;t++)
		{
			ThicknessMeasuredMoy[t] = 0.0;
			QualityMeasuredMoy[t] = 0.0;
		}
	}

	// Detection toutes les mesures sont hors tolerance -> return fail
	if(DblLiseEd->iNbCalibrationRepeat <= nbErrors)
	{
		LogDblED(*DblLiseEd,PRIO_ERROR,"[DBL_CalibrateDblLise] All measures are under tolerance -> Exit calibration");
		DBL_LEDIAcqStop(s);
		DblLiseEd->dTotalThickness = TempCalib;	// On remet l'ancienne calibration
		LogCalibrationFile(*DblLiseEd,CalibThickness,ThicknessMeasured[0],QualityMeasured[0],DblLiseEd->dTotalThickness,FP_CALIB_UNDER_TOLERANCE,IndexLowerAG);
		
		//ReleaseMutex(DblLiseEd->ProbeMutex);//DblLiseEd->bProbeRessourceReserved = false;
		
		return FP_CALIB_UNDER_TOLERANCE;
	}

	// Detection le range de total thickness est trop élevé, 
	if((Max - Min) > 3.0)
	{
		LogDblED(*DblLiseEd,PRIO_ERROR,"[DBL_CalibrateDblLise] Range too high %d -> Exit calibration",(Max - Min));
		DBL_LEDIAcqStop(s);
		DblLiseEd->dTotalThickness = TempCalib;	// On remet l'ancienne calibration
		LogCalibrationFile(*DblLiseEd,CalibThickness,ThicknessMeasured[0],QualityMeasured[0],DblLiseEd->dTotalThickness,FP_CALIB_UNDER_TOLERANCE,IndexLowerAG);

		//ReleaseMutex(DblLiseEd->ProbeMutex);//DblLiseEd->bProbeRessourceReserved = false;

		return FP_CALIB_UNDER_TOLERANCE;
	}

	// end of post traitement

	DblLiseEd->dTotalThickness = dTotalThicknessMoy;

	// Sauvegarde des resultats
	DblLiseEd->dCalibrateLowerAirgap = ThicknessMeasuredMoy[IndexLowerAG];
	DblLiseEd->dCalibrateUpperAirgap = ThicknessMeasuredMoy[0];
	DblLiseEd->dCalibrateThicknessUsed = ThicknessUsedMoy;

	// On loggue les résultats
	LogDblED(*DblLiseEd,PRIO_INFO,"[DBL_CalibrateDblLise] Calibration mode = %i",DblLiseEd->iCalibrationMode);
	LogDblED(*DblLiseEd,PRIO_INFO,"[DBL_CalibrateDblLise] Number of out of tolerance measures %i", nbErrors);
	LogDblED(*DblLiseEd,PRIO_INFO,"[DBL_CalibrateDblLise] Total thickness Min: %f µm, Max: %f µm, range: %f µm", Min,Max,(Max - Min));
	LogDblED(*DblLiseEd,PRIO_INFO,"[DBL_CalibrateDblLise] Total thickness Moy: %f", dTotalThicknessMoy);

	LogCalibrationFile(*DblLiseEd,CalibThickness,ThicknessMeasuredMoy,QualityMeasuredMoy,DblLiseEd->dTotalThickness,FP_OK,IndexLowerAG);

    if(DblLiseEd->dTotalThickness >= 0)
	{
		LogDblED(*DblLiseEd,PRIO_INFO,"[DBL_CalibrateDblLise] Calibration OK, total thickness: %f", DblLiseEd->dTotalThickness);
		//ReleaseMutex(DblLiseEd->ProbeMutex);//DblLiseEd->bProbeRessourceReserved = false;
		return FP_OK;
	}
	else
	{
		LogDblED(*DblLiseEd,PRIO_ERROR,"[DBL_CalibrateDblLise] Total thickness negative: %f", DblLiseEd->dTotalThickness);
		//ReleaseMutex(DblLiseEd->ProbeMutex);//DblLiseEd->bProbeRessourceReserved = false;
		return FP_CALIB_TOTALTH_NEGATIVE;
	}

#pragma endregion // New_Code
}

// Fonction pour remettre le systeme dans un etat connu
int SetSystemMode(void* s,MODE ModeProbeBeforeCalib,int ProbeNum ,ACQUISITIONMODE ModeDblEdBeforeCalib)
{
	// on recupere la structure du pointeur
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;
	int returnValue = FP_OK;

	// on detaggue la probe de calibration
	DblLiseEd->bProbeInCalibration = false;

	if((ProbeNum >= 0)&&(ProbeNum <= 1))
	{
		// Selection de la probe
		LogDblED(*DblLiseEd,PRIO_INFO,"[SetSystemMode] Select probe %i", ProbeNum);
		if(DBL_SetCurrentProbe(s,ProbeNum,false) != FP_OK)
		{ 
			LogDblED(*DblLiseEd,PRIO_ERROR,"[SetSystemMode] Set current probe failed");	
			returnValue = FP_SELECTCHANNEL_FAILED;
		}

		// On remet le double ED dans l'état dans lequel on l'a trouvé en entrant
		if(ModeProbeBeforeCalib == Measurement)
		{
			LogDblED(*DblLiseEd,PRIO_INFO,"[SetSystemMode] Mode for double lise was MEASUREMENT");	
			if(ModeDblEdBeforeCalib == SingleShot)
			{
				LogDblED(*DblLiseEd,PRIO_INFO,"[SetSystemMode] Set current probe in single shot mode");
				DBL_LEDIStartSingleShotAcq(s);
			}
			else
			{
				LogDblED(*DblLiseEd,PRIO_INFO,"[SetSystemMode] Set current probe in continuous mode");
				DBL_LEDIStartContinuousAcq(s);
			}
		}
		else if(ModeProbeBeforeCalib == RecoupPower)
		{
			LogDblED(*DblLiseEd,PRIO_INFO,"[SetSystemMode] Mode for double lise was READ COUPLED POWER");		
		}
		else
		{
			LogDblED(*DblLiseEd,PRIO_INFO,"[SetSystemMode] Double Lise was STOPPED before entering in calibrate function");	
		}
	}
	else
	{
		LogDblED(*DblLiseEd,PRIO_ERROR,"[SetSystemMode] Probe number invalid : %i",ProbeNum);	
		return FP_CALIB_WRONG_PROBE_NUM;
	}

	return returnValue;
}


void hSleep(int ms)
{
	int T=GetTickCount();
	while(T==GetTickCount()) Sleep(1);
	Sleep(ms);
	while((T+ms-(int)GetTickCount())>0)  Sleep(1);
}


// fonction pour faire un check de l'erreur
void CheckError(int32 Error)
{
#ifdef _DEBUG
	if(Error < 0)
	{
		BreakHook();
	}
#endif
}
// pour definir la probe visible
int DBL_SetVisibleProbe(void* s,int Channel)
{
	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;


	LogDblED(*DblLiseEd,PRIO_INFO,"[DBL_SetVisibleProbe] Enter function. Current channel = %i, Channel wanted = %i",DblLiseEd->iVisibleProbe,Channel);

	// mise à jour seulement si elle est demandée
	DblLiseEd->iVisibleProbe = Channel;

	return FP_OK;
}
// fonction pour faire un set de la probe courante
int DBL_SetCurrentProbe(void* s,int Channel, bool bUpdateProbeVisible)
{
	// préconditions
	if(!s)
	{
		return FP_FAIL;
	}

	// on récupère le pointeur sur la structure
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	LogDblED(*DblLiseEd,PRIO_INFO,"[DBL_SetCurrentProbe] Enter function. Current channel = %i, Channel wanted = %i",DblLiseEd->iCurrentProbe,Channel);

	// cas ou la channel est differente de la courante
	if(Channel != DblLiseEd->iCurrentProbe)
	{
		// on recupere l'etat courant de la probe (started/Stopped) et le mode d'acquisition (SingleShot/continous)
		DBL_PROBE_STATE EnteringStateProbe = DblLiseEd->ProbeState;
		ACQUISITIONMODE EnteringAcquisitionMode = DblLiseEd->LiseEd[DblLiseEd->iCurrentProbe].Lise.AcqMode;

		// si la probe est started, on arrete la courante
		if(DblLiseEd->ProbeState == DblEdStarted)
		{
			// Stop de la probe
			DBL_LEDIAcqStop(s);	
		}
		
		// opération pour le changement de switch
		int32 ErrSwitch;
		//uInt32 AlarmLr;
		int32 written;

		// constante pour les différents états du switch
		uInt32 WriteAdressSwitch0 = 0;
		if(Channel == 1) WriteAdressSwitch0 = -1;
		uInt32 WriteAdressSwitch1 = 0;
		uInt32 WriteAdressSwitch2High = -1;
		uInt32 WriteAdressSwitch2Low = 0;

#ifndef NOHARDWARE
		// on écrit l'adresse sur le port
		ErrSwitch =  DAQmxWriteDigitalU32(DblLiseEd->LiseEd[0].Lise.T_Switch0,1,1,DblLiseEd->LiseEd[0].Lise.Timeout,DAQmx_Val_GroupByChannel,&WriteAdressSwitch0,&written,NULL);	if(DblEdDisplayDAQmxError(DblLiseEd->LiseEd[0],ErrSwitch,"EnableScanOn Error Alarm Not Valid")!=FP_OK) return FP_SELECTCHANNEL_FAILED;
		CheckError(ErrSwitch);
		ErrSwitch =  DAQmxWriteDigitalU32(DblLiseEd->LiseEd[0].Lise.T_Switch1,1,1,DblLiseEd->LiseEd[0].Lise.Timeout,DAQmx_Val_GroupByChannel,&WriteAdressSwitch1,&written,NULL);	if(DblEdDisplayDAQmxError(DblLiseEd->LiseEd[0],ErrSwitch,"EnableScanOn Error Alarm Not Valid")!=FP_OK) return FP_SELECTCHANNEL_FAILED;
		CheckError(ErrSwitch);

		hSleep(1);

		ErrSwitch =  DAQmxWriteDigitalU32(DblLiseEd->LiseEd[0].Lise.T_AlarmLr,1,1,DblLiseEd->LiseEd[0].Lise.Timeout,DAQmx_Val_GroupByChannel,&WriteAdressSwitch2High,&written,NULL);	if(DblEdDisplayDAQmxError(DblLiseEd->LiseEd[0],ErrSwitch,"EnableScanOn Error Alarm Not Valid")!=FP_OK) return FP_SELECTCHANNEL_FAILED;
		CheckError(ErrSwitch);
		// 1 ms de sleep, les temps de commutation étant de 45µs selon la doc du switch
		hSleep(1);

		ErrSwitch =  DAQmxWriteDigitalU32(DblLiseEd->LiseEd[0].Lise.T_AlarmLr,1,1,DblLiseEd->LiseEd[0].Lise.Timeout,DAQmx_Val_GroupByChannel,&WriteAdressSwitch2Low,&written,NULL);	if(DblEdDisplayDAQmxError(DblLiseEd->LiseEd[0],ErrSwitch,"EnableScanOn Error Alarm Not Valid")!=FP_OK) return FP_SELECTCHANNEL_FAILED;
		CheckError(ErrSwitch);
#endif
		// temps d'attente de stabilisation de 65µs selon doc du switch
		hSleep(1);

		// on définit la probe courante
		DblLiseEd->iCurrentProbe = Channel;

		// si l'etat etait demarre, on redemarre la probe
		if(EnteringStateProbe == DblEdStarted)
		{
			// start de la probe en fonction du dernier mode de fonctionnement
			if(EnteringAcquisitionMode == Continous)
			{
				DBL_LEDIStartContinuousAcq(s);
			}
			else
			{
				DBL_LEDIStartSingleShotAcq(s);
			}
		}

		// Attente de stabilisation
		Sleep(DblLiseEd->iWaitAfterSwitch);
	}

	// mise à jour seulement si elle est demandée
	if(bUpdateProbeVisible)
	{
		DblLiseEd->iVisibleProbe = Channel;
	}

	return FP_OK;
}

// fonction pour faire une association des taches
int DBL_SetMasterDevice(DBL_LISE_ED& DblLiseEd)
{
	// on récupère le device Master
	int DeviceMaster = 0;
	int DeviceSlave = 0;

	if(DblLiseEd.LiseEd[0].DblEdMode == Master)
	{
		DeviceMaster = 0;
		DeviceSlave = 1;
	}
	else if(DblLiseEd.LiseEd[1].DblEdMode == Master)
	{
		DeviceMaster = 1;
		DeviceSlave = 0;
	}
	else
	{
		// erreur, configuration pas OK
		LogDblED(DblLiseEd,PRIO_ERROR,"Configuration Master/slave Wrong. Can't find Master device.");
		return FP_SET_MASTER_DEV_FAIL;
	}

	// on récupère le taskHandle du master: source, acquisition
	DblLiseEd.LiseEd[DeviceSlave].Lise.T_VoieAnalogIn = DblLiseEd.LiseEd[DeviceMaster].Lise.T_VoieAnalogIn;
	DblLiseEd.LiseEd[DeviceSlave].Lise.T_ControlSource1 = DblLiseEd.LiseEd[DeviceMaster].Lise.T_ControlSource1;
	DblLiseEd.LiseEd[DeviceSlave].Lise.T_PuissanceREcouplee = DblLiseEd.LiseEd[DeviceMaster].Lise.T_PuissanceREcouplee;
	// Note: s'il le faut, faire également le lien avec les autres taches NI

	return FP_OK;
}

// Pour la création du fichier de log de calibration
void CreateCalibrationFile(DBL_LISE_ED& DblLiseEd)
{
	// Ouverture pour sqvoir si le fichier existe
	bool Exists = false;
	FILE* TempFile = fopen(DblLiseEd.strLogCalibrationFile,"r");
	if(TempFile){
		Exists = true;
		fclose(TempFile);
	}
	// Ouverture et concaténation des infos qui vont être logguée dans le fichier
	DblLiseEd.FileLogCalib = fopen(DblLiseEd.strLogCalibrationFile,"ab");

	if((DblLiseEd.FileLogCalib)&&(Exists == false)){
		// On récupère l'heure et date du fichier pour ouverture
		SYSTEMTIME lpSystime;
		GetLocalTime(&lpSystime);
		fprintf(DblLiseEd.FileLogCalib,"-------------- Start calibration File, run from %i/%i/%i %ih%im%is --------------\r\n",lpSystime.wDay,lpSystime.wMonth,lpSystime.wYear,lpSystime.wHour,lpSystime.wMinute,lpSystime.wSecond);
		fprintf(DblLiseEd.FileLogCalib,"DD/MM/YY HH/MM/SS\tUpAGDef\tThicknessSample\tLowAGDef\tX\tY\tZ\tModeCalib\tUpPrUPAGMeas\tUpPrThMeas\tUpPrQual\tLOWPrAGMeas\tLOWPrThMeas\tLOWPrQual\tTotlaTh\tError\r\n");
	}
}

// Fonction pour logguer les message
void LogCalibrationFile(DBL_LISE_ED& DblLiseEd,double* CalibArray,double* ThicknessMeasured,double* QualityArray,double CalculateTh,int error,int _iIndexLowerAG)
{
	// On récupère la date et l'heure de la calib
	SYSTEMTIME lpSystime;
	GetLocalTime(&lpSystime);

	if (_iIndexLowerAG < 1)
	{
		fprintf(DblLiseEd.FileLogCalib,"%i/%i/%i %i:%i:%i\t",lpSystime.wDay,lpSystime.wMonth,lpSystime.wYear,lpSystime.wHour,lpSystime.wMinute,lpSystime.wSecond);
		fprintf(DblLiseEd.FileLogCalib,"Programming error: invalid lower air gap index\r\n");
	}

	// log seulement si le fichier n'est pas NULL
	if(DblLiseEd.FileLogCalib != NULL)
	{
		// date et heure dans le fichier
		fprintf(DblLiseEd.FileLogCalib,"%i/%i/%i %i:%i:%i\t",lpSystime.wDay,lpSystime.wMonth,lpSystime.wYear,lpSystime.wHour,lpSystime.wMinute,lpSystime.wSecond);
		fprintf(DblLiseEd.FileLogCalib,"%f\t%f\t%f\t",CalibArray[0],CalibArray[1],CalibArray[2]);
		fprintf(DblLiseEd.FileLogCalib,"%f\t%f\t%f\t",DblLiseEd.XCalibrationValue,DblLiseEd.YCalibrationValue,DblLiseEd.ZCalibrationValue);
		fprintf(DblLiseEd.FileLogCalib,"%i\t",DblLiseEd.iCalibrationMode);
		fprintf(DblLiseEd.FileLogCalib,"%f\t%f\t%f\t",ThicknessMeasured[0],ThicknessMeasured[1],QualityArray[1]);
		fprintf(DblLiseEd.FileLogCalib,"%f\t%f\t%f\t",ThicknessMeasured[_iIndexLowerAG],ThicknessMeasured[_iIndexLowerAG-1],QualityArray[3]);
		fprintf(DblLiseEd.FileLogCalib,"%f\t%i\t",CalculateTh,error);
		fprintf(DblLiseEd.FileLogCalib,"\r\n");
	}
}

// pour la fermeure du fichier de log de calibration
void CloseCalibrationFile(DBL_LISE_ED& DblLiseEd)
{
	if(DblLiseEd.FileLogCalib == NULL)
		return;

	fclose(DblLiseEd.FileLogCalib);
}
// pour faire un clear des fichier pic moyenne
void ClearPicMoyenne(void* s)
{
	// on recupere la structure du pointeur
	DBL_LISE_ED* DblLiseEd = (DBL_LISE_ED*)s;

	// restart des fichiers pour les deux sondes
	RestartPicMoyenne(DblLiseEd->LiseEd[0].Lise);
	RestartPicMoyenne(DblLiseEd->LiseEd[1].Lise);
}