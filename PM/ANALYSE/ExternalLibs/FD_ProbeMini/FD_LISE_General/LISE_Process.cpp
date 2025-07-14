/*
 * $Id: LISE_ED_DLL_Process.cpp 7501 2008-09-08 07:46:21Z m-abet $
 */
#include <windows.h>
#include <stdio.h>
#include <string.h>

// ## probe-common headers ##
#include "..\SrcC\SPG.h"
#include "../FD_FogaleProbe/NIDAQmxConfig.h"
// ## probe-common headers ##

// ## probe-specific headers ##
#include "../FD_LISE_General/LISE_Consts.h"
#include "../FD_LISE_General/PeakMatch.h"
#include "../FD_LISE_General/LISE_Struct_Process.h"
#include "../FD_LISE_General/LISE_Struct.h"

// #include "../FD_LISELS/LISE_LSLI_DLL_Internal.h"

#include "../FD_LISEED/LISE_ED_DLL_UI_Struct.h"
#include "../FD_LISEED/LISE_ED_DLL_Internal.h"
// ## probe-specific headers ##

#include "../FD_LISEED/LISE_ED_DLL_Internal.h"
#include "../FD_LISEED/LISE_ED_DLL_Acquisition.h"
#include "../FD_LISEED/LISE_ED_DLL_Config.h"
#include "../FD_LISEED/LISE_ED_DLL_Create.h"
#include "../FD_LISEED/LISE_ED_DLL_General.h"
#include "../FD_LISEED/LISE_ED_DLL_Process.h"
#include "../FD_LISEED/LISE_ED_DLL_Log.h"
#include "../FD_LISEED/LISE_ED_DLL_Reglages.h"

// ProcessPic qui va permettre de marquer les pics similaires quel que soit le buffer de Resultats
int ProcessPic(LISE& Lise,PICRESULT* BufferResultat,RING_BUFFER_POS& WriteResult,int Voie)
{ // Rajouter la voie pour que chaque traitement dépende du buffer
	// On balaie tout le buffer pour mettre a zéro l'indice de Regroupement
	if(BufferResultat == NULL)
	{ // test sur le buffer de resultat
		if(Lise.bDebug == true)
		{
			if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tError in Process Pic - BufferResultat NULL");
		}
		return STATUS_FAIL;
	}
	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tEntering Process Peaks");
	}

	RING_BUFFER_POS IndiceTraitement;
	IndiceTraitement = WriteResult;
	// On se place à l'indice en début de période sur le buffer de résultats pour traiter tous les derniers pics acquis
	
	// Attention à faire le cas pour la voie 2
	int NombreDePicsATraiter = 0;
	if(Voie == 0)
	{
		NombreDePicsATraiter = Lise.NbPicPlusProcessVoie1 + Lise.NbPicMoinsProcessVoie1;
	}
	else if(Voie == 1)
	{
		NombreDePicsATraiter = Lise.NbPicPlusProcessVoie2 + Lise.NbPicMoinsProcessVoie2;
	}
    if (NombreDePicsATraiter < 0 || NombreDePicsATraiter >= IndiceTraitement.Len)
    {
		LogfileF(*Lise.Log,"[LISEED]\tError - Number of peaks to process in ProcessPic = %i", NombreDePicsATraiter);
		if(Global.EnableList>=1) MessageBox(0,L"Number of peaks to process in ProcessPic",L"ERROR",0);
		//SaveLastWaveformError(Lise,1);
		return STATUS_FAIL;
	}

	RBP_Sub(IndiceTraitement,NombreDePicsATraiter)
	RING_BUFFER_POS IndiceTraitementSecondaire = IndiceTraitement;
	// Détection des pics, on leur attribue un nombre en fonction du pic courant
	int TempVar = 1;
	double vartemp;

	while (IndiceTraitement.AbsN < WriteResult.AbsN)
	{ // On traite tous les pics jusqu'à WriteResult.AbsN
		BufferResultat[IndiceTraitement.N].IndiceRegroupement = TempVar;

		while (IndiceTraitementSecondaire.AbsN < WriteResult.AbsN)
		{
			vartemp = fabs(BufferResultat[IndiceTraitement.N].XRel-BufferResultat[IndiceTraitementSecondaire.N].XRel);
			double coeff = 2.0;
			if(Lise.NombredeVoie == 2)
			{
				coeff = 8.0;
			}
			if(fabs(BufferResultat[IndiceTraitement.N].XRel-BufferResultat[IndiceTraitementSecondaire.N].XRel) < (double)Lise.fitLen / coeff && BufferResultat[IndiceTraitement.N].Sens == BufferResultat[IndiceTraitementSecondaire.N].Sens)
			{
				BufferResultat[IndiceTraitementSecondaire.N].IndiceRegroupement = TempVar;
				RBP_Inc(IndiceTraitementSecondaire);
			}
			else break;
		}
		TempVar++;
		IndiceTraitement = IndiceTraitementSecondaire;
	}

	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tProcess Peaks Sucess");
	}

	return STATUS_OK;
}

int PicMoyenne(LISE& Lise,PICRESULT* BufferResultat,RING_BUFFER_POS& WriteResult,int Voie)
{
// rassemblement et moyenne des pics du buffer de résultat passé en paramètre
	if(BufferResultat == NULL)
	{ // test si on passe un bien un buffer de résultats
		if(Lise.bDebug == true)
		{
			if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tError in Pic Moyenne - BufferResultat NULL");
		}
		return STATUS_FAIL;
	}

	if(Lise.bDebug == true)
	{ // instruction de debug pour le log
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tEntering Average Peak ");
	}

	int compteur = 0;
	double PositionMoyenne = 0;
	double IntensiteMoyenne = 0;
	double QualiteMoyenne = 0;
	double XAbsNMoyenne = 0;
	double XRelMoyenne = 0;
	int NombreDePicsATraiter=0;//ajout init à zero par SPG sans chercher à comprendre (warning C4701: variable locale 'NombreDePicsATraiter' potentiellement non initialisée utilisée)
	
	// L'indice du dernier résultat est l'indice max
	int NombreIndicesATraiter = 0; 
	if(WriteResult.N == 0 && Lise.NbPicPlusProcessVoie1 == 0 && Lise.NbPicMoinsProcessVoie1 ==0)
	{
		NombreIndicesATraiter = 0;
	}
	else
	{
		NombreIndicesATraiter = BufferResultat[RBP_GetNMinusOne(WriteResult)].IndiceRegroupement;
	}
	if(Voie == 0)
	{
		NombreDePicsATraiter = Lise.NbPicPlusProcessVoie1 + Lise.NbPicMoinsProcessVoie1;
	}
	else if(Voie == 1)
	{
		NombreDePicsATraiter = Lise.NbPicPlusProcessVoie2 + Lise.NbPicMoinsProcessVoie2;
	}
	RING_BUFFER_POS IndiceTraitement;
	IndiceTraitement = WriteResult;
	RBP_Sub(IndiceTraitement,NombreDePicsATraiter);
	RING_BUFFER_POS IndiceTraitementSecondaire = IndiceTraitement;
	RING_BUFFER_POS IndEcrit = IndiceTraitement;
	int CompteurPicsPlus = 0;
	int CompteurPicsMoins = 0;

	for(int i=1;i<=NombreIndicesATraiter;i++)
	{
		while(IndiceTraitementSecondaire.AbsN < WriteResult.AbsN)
		{
			if(BufferResultat[IndiceTraitement.N].IndiceRegroupement == BufferResultat[IndiceTraitementSecondaire.N].IndiceRegroupement)
			{
				XRelMoyenne += BufferResultat[IndiceTraitementSecondaire.N].XRel;
				PositionMoyenne += BufferResultat[IndiceTraitementSecondaire.N].Position;
				IntensiteMoyenne += BufferResultat[IndiceTraitementSecondaire.N].Intensite;
				QualiteMoyenne += BufferResultat[IndiceTraitementSecondaire.N].Qualite;
				XAbsNMoyenne += BufferResultat[IndiceTraitementSecondaire.N].XAbsN;
				compteur++;
				RBP_Inc(IndiceTraitementSecondaire);
			}
			else break;
		}
		BufferResultat[IndEcrit.N].XRel = XRelMoyenne / (double)compteur;
		BufferResultat[IndEcrit.N].Position = PositionMoyenne / (double)compteur; 
		BufferResultat[IndEcrit.N].Intensite = IntensiteMoyenne / (double)compteur;
		BufferResultat[IndEcrit.N].Qualite = QualiteMoyenne / (double)compteur;
		BufferResultat[IndEcrit.N].XAbsN = XAbsNMoyenne / (double)compteur;
		BufferResultat[IndEcrit.N].Sens = BufferResultat[IndiceTraitement.N].Sens;
		BufferResultat[IndEcrit.N].IndiceRegroupement = BufferResultat[IndiceTraitement.N].IndiceRegroupement;
		if(BufferResultat[IndEcrit.N].Sens == true)
		{
			CompteurPicsPlus++;
		}
		if(BufferResultat[IndEcrit.N].Sens == false)
		{
			CompteurPicsMoins++;
		}
		compteur = 0;
		XRelMoyenne = 0;
		PositionMoyenne = 0;
		IntensiteMoyenne = 0;
		QualiteMoyenne = 0;
		XAbsNMoyenne = 0;
		IndiceTraitement = IndiceTraitementSecondaire;
		RBP_Inc(IndEcrit);
	}

	if(Voie == 0)
	{
		Lise.NbPicPlusProcessVoie1 = CompteurPicsPlus;
		Lise.NbPicMoinsProcessVoie1 = CompteurPicsMoins;
	}
	else if(Voie == 1)
	{
		Lise.NbPicPlusProcessVoie2 = CompteurPicsPlus;
		Lise.NbPicMoinsProcessVoie2 = CompteurPicsMoins;
	}
	WriteResult = IndEcrit;

	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tAverage Peak Success");
	}

	return STATUS_OK;
}

int FindPicSature(LISE& Lise,PICRESULT* BufferResultat,RING_BUFFER_POS &WriteResultChannelProcess)
{
	RING_BUFFER_POS PicTraitementSaturation = WriteResultChannelProcess;
	int PicATraiter = Lise.NbPicPlusProcessVoie1 + Lise.NbPicMoinsProcessVoie1;
	RBP_Sub(PicTraitementSaturation,PicATraiter);

	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tEntering Function Peak Sature");
	}

	for(int i = 0;i<PicATraiter;i++)
	{
		// Condition pour traiter un pic saturé
		if(BufferResultat[WriteResultChannelProcess.N].Intensite > Lise.fThresholdSaturation - Lise.LigneDeBase)
		{
			BufferResultat[PicTraitementSaturation.N].bSaturation = true;
		}
		else
		{
			BufferResultat[PicTraitementSaturation.N].bSaturation = false;
		}
		RBP_Inc(PicTraitementSaturation);
	}

	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tFunction Peak Sature Success");
	}

	return STATUS_OK;
}

int PicDetection(LISE& Lise,PICRESULT* BufferResultat,double* Buffer,RING_BUFFER_POS &WriteResultChannelProcess, int Voie, bool bChan1BeforeChan2)
{
	int i = Lise.Read.N;
	// Avant modif de structure
	//if(Lise.led.bPulse)
	//{
	//	return STATUS_OK;
	//}
	if(Buffer[i] >= 0)
	{
		int a = 0;
	}
	// TODO : attention au fonctionnement quand il y a 2 voies de mesure. Utiliser habilement LISE.fitStep.

	if(Lise.NombredeVoie == 2)
	{
		//int fitStepCount = (int)(LISE.fitStep / 2) - 1;
		//if(LISE.Read.N % fitStepCount != 0)
		if(Lise.ReadCrossBuffer.AbsN % Lise.fitStep != 0)
		{
			return STATUS_OK;
		}
	}
	else
	{	// On ne traite que pour 1 point sur fitStep points
		if(Lise.Read.AbsN % Lise.fitStep != 0)
		{
			return STATUS_OK;
		}
	}

	if(Lise.NombredeVoie == 2 && Voie == 1)
	{ // On calcule le pic pour la 2eme voie
		RBP_Inc(Lise.Read);
	}

	if( BufferResultat == NULL)
	{
		if(Lise.bDebug == true)
		{
			if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tError int Pulse Detection - Buffer Signal NULL");
		}
		return STATUS_FAIL;
	}

	int fitStep = Lise.fitStep;
	int fitLen = Lise.fitLen;

	K_FIT2_ELT fitRes;
	if (Lise.fitLen + Lise.Read.N > Lise.BufferLen)
	{
		/* Ajout voie 2*/
		int Len1 = Lise.BufferLen - Lise.Read.N;
		int Len2 = Lise.fitLen - Lise.BufferLen + Lise.Read.N;

		if(Lise.NombredeVoie == 2)
		{
			fitStep = Lise.fitStep * 2;
			Len1 = 2 * Len1;
			Len2 = 2 * Len2;
		}
		// Attention à modifier les fitLen en fonction de ce que l'on a dans le code de la fonction
//		K_FIT2_Double(Buffer+Lise.Read.N, Lise.BufferLen - Lise.Read.N, Buffer, Lise.fitLen - Lise.BufferLen + Lise.Read.N, fitStep, fitRes, Lise.fitDef);
		K_FIT2_Double(Buffer+Lise.Read.N, Len1, Buffer, Len2, fitStep, fitRes, Lise.fitDef);
	}
	else
	{
		/* Ajout voie 2*/
		if(Lise.NombredeVoie == 2)
		{
			fitStep = Lise.fitStep * 2;
			fitLen = Lise.fitLen * 2;
		}
		// Modif ok, on va deux fois plus loin
		K_FIT2_Double(Buffer+Lise.Read.N, fitLen, NULL, 0, fitStep, fitRes, Lise.fitDef);
	}

	if(fitRes._a < 0)
	{ //cas du coeff d'ordre 2 negatif
		double top = - (fitRes._b / (2*fitRes._a)) * fitStep + (Lise.fitLen-1) / 2.0;
		double topFit = - (fitRes._b / (2*fitRes._a));
		if (topFit > -(double)Lise.fitLen / (4.0 * (double)fitStep) && topFit < (double)Lise.fitLen / (4.0 * (double)fitStep))
		{
			// Recherche du maximum
			double max = Lise.ValMin;
			RING_BUFFER_POS maxPos;
			RING_BUFFER_POS indBuf = Lise.Read;
			RING_BUFFER_POS indEnd = indBuf;
			RBP_Add(indEnd, Lise.fitLen);
			/* Ajout voie 2*/
			if(Lise.NombredeVoie == 2)
			{ // On ajoute deux fois la distance pour avoir la bonne position de stop pour la deuxième voie
				RBP_Add(indEnd, Lise.fitLen);
			}
			while (indBuf.AbsN < indEnd.AbsN)
			{
				if (Buffer[indBuf.N] > max)
				{
					max = Buffer[indBuf.N];
					maxPos = indBuf;
				}
				RBP_Inc(indBuf);
				/* Ajout voie 2*/
				if(Lise.NombredeVoie == 2)
				{
					RBP_Inc(indBuf);
				}
			}

			double coef1 = 1.0;
			double coef2 = 3.0 / 4.0;
			if(Lise.NombredeVoie == 2)
			{ // on agrandi l'intervalle de recherche des pics lorsqu'on traite les deux voies.
				//coef1 = 0;
				//coef2 = 2.0;
				coef1 = 0.0;
				coef2 = 2.0;
			}
			if (maxPos.AbsN > Lise.Read.AbsN + coef1 * (int)((double)Lise.fitLen / 4.0) && maxPos.AbsN < Lise.Read.AbsN + (int)((double)Lise.fitLen * coef2))
			{
				// Recherche de l'intervalle de fit précis
				RING_BUFFER_POS Start = maxPos;
				RING_BUFFER_POS Stop = maxPos;
				int lFit=0;
				//while((Start.AbsN>Lise.Read.AbsN)&&(Stop.AbsN<Lise.Read.AbsN + Lise.fitLen))
				while(2*lFit+1 < Lise.fitLen / 2.0)
				{
					/* Ajout voie 2*/
					if(Lise.NombredeVoie == 2)
					{ // on incrémente de deux pour tomber sur l'échantillon de la voie correspondante.
						RBP_Sub(Start,2);
						RBP_Add(Stop,2);
						lFit++;
					}
					else
					{
						//trouve la largeur
						RBP_Sub(Start,1);
						RBP_Add(Stop,1);
						lFit++;
					}
				}

				int preciseFitLen=2*lFit+1;
				if(Lise.NombredeVoie == 2)
				{
					preciseFitLen = 2 * preciseFitLen; 
				}

				// On a trouvé un pic, on fit par une parabole moins grossière centrée sur le maximum.
				if (preciseFitLen + Start.N > Lise.BufferLen)
				{
					int Len1 = Lise.BufferLen - Start.N;
					int Len2 = preciseFitLen - Lise.BufferLen + Start.N;
					/* Ajout voie 2*/
					if(Lise.NombredeVoie == 2)
					{ // on incrémente de deux à chaque fois
						Len1 = 2 * Len1;
						Len2 = 2 * Len2;
//						K_FIT2_Double(Buffer+Start.N, Lise.BufferLen - Start.N, Buffer, preciseFitLen - Lise.BufferLen + Start.N, 2, fitRes, Lise.fitDef);
						/*if(Lise.bDebug == true)
						{
							if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\t K_Fit2_Double: D1= %d , Len1= %i , D2= %d , Len2= %i",(Buffer+Start.N), Len1, Buffer, Len2);
						}*/
						K_FIT2_Double(Buffer+Start.N, Len1, Buffer, Len2, 2, fitRes, Lise.fitDef);
					}
					else
					{ // cas d'une seule voie
						K_FIT2_Double(Buffer+Start.N, Len1, Buffer, Len2, 1, fitRes, Lise.fitDef);
					}
				}
				else
				{
					/* Ajout voie 2*/
					if(Lise.NombredeVoie == 2)
					{ // on incrémente de deux à chaque fois
						K_FIT2_Double(Buffer+Start.N, preciseFitLen, NULL, 0, 2, fitRes, Lise.fitDef);
					}
					else
					{
						K_FIT2_Double(Buffer+Start.N, preciseFitLen, NULL, 0, 1, fitRes, Lise.fitDef);
					}
				}
				top = - (fitRes._b / (2*fitRes._a)) + (preciseFitLen-1) / 2.0 + (double)(Start.AbsN - Lise.Read.AbsN);
				topFit = - (fitRes._b / (2*fitRes._a));

				// Si le sommet est placé dans la moitié centrale de la parabole précise.
				if (fitRes._a < 0 && topFit > -(double)Lise.fitLen / 4.0 && topFit < (double)Lise.fitLen / 4.0)
				{
					double I = fitRes._c + topFit * (fitRes._b + topFit * fitRes._a) - Lise.LigneDeBase;
					double Q = 0;
					double discriminant = 0;	// on calcule le discriminant
					double x1 = 0;	// et les racines
					double x2 = 0;
					double largeur = 0;	// pour le calcul de la largeur d'un pic
					discriminant = fitRes._b * fitRes._b - 4 * fitRes._a * (fitRes._c - Lise.LigneDeBase);
					if(discriminant > 0)
					{
						x1 = (-fitRes._b - sqrt(discriminant)) / (2*fitRes._a);
						x2 = (-fitRes._b + sqrt(discriminant)) / (2*fitRes._a);
						largeur = fabs(x2 - x1);
					}
					if(discriminant == 0)	largeur = 0;
					if(discriminant < 0)
					{
						largeur = 0;
					}
					RING_BUFFER_POS StartPosition = Start;
					int iCount = preciseFitLen;
					if(Lise.NombredeVoie == 2)iCount /= 2;
					for(int j = 0; j < iCount;j++)
					{
						//Erreur sur la qualité, ne pas faire Buffer[i+j]!! Pic à la fin du buffer?
						if(Lise.NombredeVoie == 2){
							Q = Q + fabs(Buffer[StartPosition.N] - (fitRes._c + (j - (preciseFitLen/2-1) / 2.0) * (fitRes._b + (j - (preciseFitLen/2-1) / 2.0) * fitRes._a)));
							RBP_Inc(StartPosition);
							RBP_Inc(StartPosition);
						}
						else{
							Q = Q + fabs(Buffer[StartPosition.N] - (fitRes._c + (j - (preciseFitLen-1) / 2.0) * (fitRes._b + (j - (preciseFitLen-1) / 2.0) * fitRes._a)));
							RBP_Inc(StartPosition);
						}
					}
					// Formule de qualité permettant de donner moins d'importance aux pics bien fittés mais très faibles.

// Début modif YR du 28 Février 2008 : Modification du calcul de la qualité des pics et des épaisseurs.
					//Q = I * preciseFitLen / (Lise.EcartTypiqueFit + Q);
					Q = I / (Lise.EcartTypiqueFit + Q);
// Fin modif YR.

					// Modif MP 17/03/11 Si le pic est saturé, la qualité est quasi-nulle
					if((Lise.NombredeVoie == 2)&&(I >  Lise.fThresholdSaturation - Lise.LigneDeBase)){
						Q = 0.1;
					}

					/*if(Lise.bDebug == true)
					{
					LogfileF(*Lise.Log,"[LISEED]\t Quality = %f , I = %f , FitRes a = %f b %f c %f ",Q,I,fitRes._a,fitRes._b,fitRes._c);
					}*/

					// Fin modif

					if(Q >= 1.0)
					{
						int a =0;
					}
					BufferResultat[WriteResultChannelProcess.N].Position = (Lise.Read.N + top) < Lise.BufferLen ? (Lise.Read.N + top) : (Lise.Read.N + top - Lise.BufferLen);
					BufferResultat[WriteResultChannelProcess.N].XAbsN = Lise.Read.AbsN + (int)top;
					BufferResultat[WriteResultChannelProcess.N].Intensite = I;
					BufferResultat[WriteResultChannelProcess.N].Sens = Lise.SensPositif;
					BufferResultat[WriteResultChannelProcess.N].Qualite = Q;
					// on initialise le XRel pour le lise LS
					BufferResultat[WriteResultChannelProcess.N].XRel = BufferResultat[WriteResultChannelProcess.N].Position;

					// Fin de detection du Pic de reference
					if (Lise.SensPositif == true)
					{
						if (BufferResultat[WriteResultChannelProcess.N].Position > (double)Lise.PulsePlusRight.N)
						{ // Cas normal
							BufferResultat[WriteResultChannelProcess.N].XRel = BufferResultat[WriteResultChannelProcess.N].Position - Lise.PulsePlusRight.N;
						}
						else
						{ // Cas ou l'on depasse la longeur de buffer
							BufferResultat[WriteResultChannelProcess.N].XRel = BufferResultat[WriteResultChannelProcess.N].Position - Lise.PulsePlusRight.N + Lise.BufferLen;
						}
					}
					// On calcule les valeurs réelles si on a les deux voies de mesures
					if(Lise.NombredeVoie == 2 && BufferResultat[WriteResultChannelProcess.N].Sens == true)
					{
						BufferResultat[WriteResultChannelProcess.N].Position /= 2;
						BufferResultat[WriteResultChannelProcess.N].XAbsN /= 2;
						BufferResultat[WriteResultChannelProcess.N].Qualite *= 2;
						BufferResultat[WriteResultChannelProcess.N].XRel /= 2;
					}
					// Pas d'ajout de pic au premier tour
					//if (!Lise.FirstPass)
					{
						// avant modif de la structure
						//if (!Lise.led.bPulseDetection)
						{
							// Permet de ne pas ajouter un pic vu dans un pulse plus avant bPulseDetection.
							if (BufferResultat[WriteResultChannelProcess.N].XAbsN < Lise.PulsePlusLeft.AbsN - (__int64)Lise.iNombreEchantillons || BufferResultat[WriteResultChannelProcess.N].XAbsN > Lise.PulsePlusRight.AbsN + (__int64)Lise.iNombreEchantillons)
							{
								//DbgCHECK(_CrtCheckMemory()!=TRUE,"Before GetPic Increment Buffer");
								RBP_Inc(WriteResultChannelProcess);
								if(Voie == 0)
								{
									// incrément du compteur de pics
									Lise.Indice++;
									if(Lise.Indice > (int)(Lise.PicResultLen * 0.9))
									{
										LogfileF(*Lise.Log,"[LiseED]\tToo many peaks found in waveform= %i", Lise.Indice);
										if(Global.EnableList>=1) MessageBox(0,L"Too many Peaks found in waveform. Restart acquisition",L"Error",0);
										//SaveLastWaveformError(LiseEd,1);
										return STATUS_FAIL;
									}
									Lise.WriteResult = WriteResultChannelProcess;
								}
								else if(Voie == 1)
								{
									Lise.iIndiceVoie2++;
									if(Lise.iIndiceVoie2 > (int)(Lise.PicResultLen  * 0.9))
									{
										LogfileF(*Lise.Log,"[LISEED]\tToo many peaks found in waveform= %i", Lise.iIndiceVoie2);
										if(Global.EnableList>=1) MessageBox(0,L"Too many Peaks found in waveform. Restart acquisition",L"Error",0);
										//SaveLastWaveformError(LiseEd,1);
										return STATUS_FAIL;
									}
									Lise.WriteResultSecondChannel = WriteResultChannelProcess;
								}
							}
						}
					}
				}
			}
		}
	}
	return STATUS_OK;
}


int WriteThickness(LISE& Lise)
{ // On va écrire dans le fichier de sauvegarde des épaisseurs les n dernières épaisseurs trouvées.
	// A ce moment, on a déjà incrémenté la période et l'indice sur le buffer circulaire de résultats.
	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tEntering Write Thickness");
	}

	if(Lise.iConfigMoyenneEpaisseurs == 1)
	{ // Configuration pour afficher la moyenne des épaisseurs aller-retour
		if(Lise.CompteurDecimation == Lise.IndiceDecimation)
		{
			if(Lise.FlagThickness == true && Lise.Periode > 1)
			{
				fprintf(Lise.FichierSaveThickness,"\r\n%i\t",Lise.Periode-1);
				int i;
				for(i =0;i<Lise.Resultats[RBP_GetNMinusOne(Lise.IndicePeriod)].iNbThickness;i++)
				{
					fprintf(Lise.FichierSaveThickness,"%f\t",Lise.Resultats[RBP_GetNMinusOne(Lise.IndicePeriod)].fThickness[i]);
				}
				fprintf(Lise.FichierSaveThickness,"%f\t",Lise.Resultats[RBP_GetNMinusOne(Lise.IndicePeriod)].fQuality);
				// Indices des pics choisis
				for(i =0;i<Lise.sample.NumPks;i++)
				{
					fprintf(Lise.FichierSaveThickness,"%i ",Lise.Resultats[RBP_GetNMinusOne(Lise.IndicePeriod)].PkIndexPlus[i]);
				}
				fprintf(Lise.FichierSaveThickness,"\t");
				for(i =0;i<Lise.sample.NumPks;i++)
				{
					fprintf(Lise.FichierSaveThickness,"%i ",Lise.Resultats[RBP_GetNMinusOne(Lise.IndicePeriod)].PkIndexMoins[i]);
				}
			}
		}
	}
	else
	{ // Sinon on écrit les épaisseurs dans le sens + ou - selon la config
		if(Lise.CompteurDecimation == Lise.IndiceDecimation)
		{
			if(Lise.FlagThickness == true && Lise.Periode > 1)
			{
				fprintf(Lise.FichierSaveThickness,"\r\n%i\t",Lise.Periode-1);
				int i;
				fprintf(Lise.FichierSaveThickness,"Dir1\t");
				for(i =0;i<Lise.Resultats[RBP_GetNMinusOne(Lise.IndicePeriod)].iNbThickness;i++)
				{
					fprintf(Lise.FichierSaveThickness,"%f\t",Lise.Resultats[RBP_GetNMinusOne(Lise.IndicePeriod)].fThicknessPlus[i]);
				}
				fprintf(Lise.FichierSaveThickness,"%f\t",Lise.Resultats[RBP_GetNMinusOne(Lise.IndicePeriod)].fQualityPlus);
				// Indices des pics choisis
				for(i =0;i<Lise.sample.NumPks;i++)
				{
					fprintf(Lise.FichierSaveThickness,"%i ",Lise.Resultats[RBP_GetNMinusOne(Lise.IndicePeriod)].PkIndexPlus[i]);
				}
				if(Lise.iConfigSensUnique == 0)
				{
					fprintf(Lise.FichierSaveThickness,"\tDir0\t");
					for(i =0;i<Lise.Resultats[RBP_GetNMinusOne(Lise.IndicePeriod)].iNbThickness;i++)
					{
						fprintf(Lise.FichierSaveThickness,"%f\t",Lise.Resultats[RBP_GetNMinusOne(Lise.IndicePeriod)].fThicknessMoins[i]);
					}
					fprintf(Lise.FichierSaveThickness,"%f\t",Lise.Resultats[RBP_GetNMinusOne(Lise.IndicePeriod)].fQualityMoins);
					// Indices des pics choisis
					for(i =0;i<Lise.sample.NumPks;i++)
					{
						fprintf(Lise.FichierSaveThickness,"%i ",Lise.Resultats[RBP_GetNMinusOne(Lise.IndicePeriod)].PkIndexMoins[i]);
					}
				}
			}
		}
	}

	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tFunction Write Thickness Success");
	}

	fflush(Lise.FichierSaveThickness);
	return STATUS_OK;
}

// fonction pour aller éc
int WritePeaksPeriod(LISE& Lise,PERIOD_RESULT PeriodResult,FILE* FileSavePeak)
{
	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tEntering Write Peaks");
	}

	// on écrit l'indice de période
	fprintf(FileSavePeak,"\r\n%i\t",Lise.Periode);

	// Sens positif
	fprintf(FileSavePeak,"Channel1\t");
	fprintf(FileSavePeak,"Dir1\t");

	// on déclare et boucle sur les n pics de la voie 1
	int j;
	for(j = 0;j<PeriodResult.NbPicsPlusVoie1;j++)
	{
		// on écrit le numéro de pic, sont XRel, sa qualité, son Intensité
		fprintf(FileSavePeak,"Pic%i\t",j);
		fprintf(FileSavePeak,"%f\t",PeriodResult.PicsPlusVoie1[j].XRel);
		fprintf(FileSavePeak,"%f\t",PeriodResult.PicsPlusVoie1[j].Qualite);
		fprintf(FileSavePeak,"%f\t",PeriodResult.PicsPlusVoie1[j].Intensite);
	}
	fflush(FileSavePeak);
	// Sens négatif

	// si on est en config sens unique
	if(Lise.iConfigSensUnique == 0)
	{
		fprintf(FileSavePeak,"Dir0\t");
		for(j = 0;j<PeriodResult.NbPicsMoinsVoie1;j++)
		{
			fprintf(FileSavePeak,"Pic%i\t",j);
			fprintf(FileSavePeak,"%f\t",PeriodResult.PicsMoinsVoie1[j].XRel);
			fprintf(FileSavePeak,"%f\t",PeriodResult.PicsMoinsVoie1[j].Qualite);
			fprintf(FileSavePeak,"%f\t",PeriodResult.PicsMoinsVoie1[j].Intensite);
		}
		fflush(FileSavePeak);
		if(Lise.NombredeVoie == 2)
		{ // Voie 2, on affiche sur la ligne suivante les même infos que pour la voie1. On ne le met pas à la suite car le fichier texte va automatiquement à la ligne après un certain nombre de caractère
			fprintf(FileSavePeak,"\r\n%i\t",Lise.Periode);
			fprintf(FileSavePeak,"Channel2\t");
			// Sens positif
			fprintf(FileSavePeak,"Dir1\t");
			for(j = 0;j<PeriodResult.NbPicsPlusVoie2;j++)
			{
				fprintf(FileSavePeak,"Pic%i\t",j+PeriodResult.NbPicsPlusVoie1);
				fprintf(FileSavePeak,"%f\t",PeriodResult.PicsPlusVoie2[j].XRel);
				fprintf(FileSavePeak,"%f\t",PeriodResult.PicsPlusVoie2[j].Qualite);
				fprintf(FileSavePeak,"%f\t",PeriodResult.PicsPlusVoie2[j].Intensite);
			}
			// Sens négatif
			fprintf(FileSavePeak,"Dir0\t");
			for(j = 0;j<PeriodResult.NbPicsMoinsVoie2;j++)
			{
				fprintf(FileSavePeak,"Pic%i\t",j+PeriodResult.NbPicsMoinsVoie1);
				fprintf(FileSavePeak,"%f\t",PeriodResult.PicsMoinsVoie2[j].XRel);
				fprintf(FileSavePeak,"%f\t",PeriodResult.PicsMoinsVoie2[j].Qualite);
				fprintf(FileSavePeak,"%f\t",PeriodResult.PicsMoinsVoie2[j].Intensite);
			}
			fflush(FileSavePeak);
		}
	}

	// on retourne le status OK
	return STATUS_OK;
}

int WritePeaks(LISE& Lise,int NombreDePics,int Voie,FILE* FileSavePeak)
{ // On écrit les pics lorsque l'on a détecté une période de signal
	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tEntering Write Peaks");
	}

	if(Lise.WritePeaksForCalibration == 1)
	{// On seuille la qualité et l'intensité pour écrire les pics.
		if(Lise.FlagSavePics == true && !Lise.FirstPass)
		{ // Sauvegarde des pics dans les différents fichiers de résultats
			if(Lise.CompteurDecimation == Lise.IndiceDecimation) 
			{
				int NombreDePicsVoie1 = Lise.NbPicPlusProcessVoie1 + Lise.NbPicMoinsProcessVoie1;
				int SensPositif = 1;
				fprintf(FileSavePeak,"\r\n%i\t",Lise.Periode);
				fprintf(FileSavePeak,"%i\t",SensPositif);
				for(int j = 0;j<NombreDePicsVoie1;j++)
				{
					if(j == Lise.NbPicPlusProcessVoie1)
					{
						SensPositif = 0;
						fprintf(FileSavePeak,"%i\t",SensPositif);
					}
					if(Lise.WriteResult.N + j - NombreDePicsVoie1 < 0)
					{
						if(Lise.BufferResultat[0][Lise.WriteResult.N  + j - NombreDePicsVoie1 + Lise.WriteResult.Len].Qualite > Lise.fQualityThreshold && Lise.BufferResultat[0][Lise.WriteResult.N  + j - NombreDePicsVoie1 + Lise.WriteResult.Len].Intensite > Lise.fIntensityThreshold)
						{ // Seuillage sur la qualité et l'intensité
							if(Lise.iConfigSensUnique == 0 || (Lise.iConfigSensUnique == 1 && Lise.BufferResultat[0][Lise.WriteResult.N  + j - NombreDePicsVoie1 + Lise.WriteResult.Len].Sens == true))
							{
								fprintf(FileSavePeak,"%f\t",Lise.BufferResultat[0][Lise.WriteResult.N  + j - NombreDePicsVoie1 + Lise.WriteResult.Len].XRel);
								fprintf(FileSavePeak,"%f\t",Lise.BufferResultat[0][Lise.WriteResult.N  + j - NombreDePicsVoie1 + Lise.WriteResult.Len].Qualite);
							}
						}
					}
					else
					{
						if(Lise.BufferResultat[0][Lise.WriteResult.N  + j - NombreDePicsVoie1].Intensite > Lise.fIntensityThreshold && Lise.BufferResultat[0][Lise.WriteResult.N  + j - NombreDePicsVoie1].Qualite > Lise.fQualityThreshold)
						{ // Seuillage sur l'intensité et la qualité
							if(Lise.iConfigSensUnique == 0 || (Lise.iConfigSensUnique == 1 && Lise.BufferResultat[0][Lise.WriteResult.N  + j - NombreDePicsVoie1].Sens == true))
							{
								fprintf(FileSavePeak,"%f\t",Lise.BufferResultat[0][Lise.WriteResult.N  + j - NombreDePicsVoie1].XRel);
								fprintf(FileSavePeak,"%f\t",Lise.BufferResultat[0][Lise.WriteResult.N  + j - NombreDePicsVoie1].Qualite);				
							}
						}
					}
				}
				fflush(FileSavePeak);
			}
		}
	}
	else
	{ // On écrit les meilleurs pics conservés pour PeakMatch
		if(Lise.FlagSavePics == true && !Lise.FirstPass)
		{ 
			if(Lise.CompteurDecimation == Lise.IndiceDecimation) 
			{
				//int indRes = RBP_GetNMinusOne(Lise.IndicePeriod);
				int indRes = Lise.IndicePeriod.N;
				int j;
				fprintf(FileSavePeak,"\r\n%i\t",Lise.Periode);
				// Sens positif
				fprintf(FileSavePeak,"Channel1\t");
				fprintf(FileSavePeak,"Dir1\t");
				for(j = 0;j<Lise.Resultats[indRes].NbPicsPlusVoie1;j++)
				{
					fprintf(FileSavePeak,"Pic%i\t",j);
					fprintf(FileSavePeak,"%f\t",Lise.Resultats[indRes].PicsPlusVoie1[j].XRel);
					fprintf(FileSavePeak,"%f\t",Lise.Resultats[indRes].PicsPlusVoie1[j].Qualite);
					fprintf(FileSavePeak,"%f\t",Lise.Resultats[indRes].PicsPlusVoie1[j].Intensite);
				}
				fflush(FileSavePeak);
				// Sens négatif

				if(Lise.iConfigSensUnique == 0)
				{
					fprintf(FileSavePeak,"Dir0\t");
					for(j = 0;j<Lise.Resultats[indRes].NbPicsMoinsVoie1;j++)
					{
						fprintf(FileSavePeak,"Pic%i\t",j);
						fprintf(FileSavePeak,"%f\t",Lise.Resultats[indRes].PicsMoinsVoie1[j].XRel);
						fprintf(FileSavePeak,"%f\t",Lise.Resultats[indRes].PicsMoinsVoie1[j].Qualite);
						fprintf(FileSavePeak,"%f\t",Lise.Resultats[indRes].PicsMoinsVoie1[j].Intensite);
					}
					fflush(FileSavePeak);
					if(Lise.NombredeVoie == 2)
					{ // Voie 2, on affiche sur la ligne suivante les même infos que pour la voie1. On ne le met pas à la suite car le fichier texte va automatiquement à la ligne après un certain nombre de caractère
						fprintf(FileSavePeak,"\r\n%i\t",Lise.Periode);
						fprintf(FileSavePeak,"Channel2\t");
						// Sens positif
						fprintf(FileSavePeak,"Dir1\t");
						for(j = 0;j<Lise.Resultats[indRes].NbPicsPlusVoie2;j++)
						{
							fprintf(FileSavePeak,"Pic%i\t",j+Lise.Resultats[indRes].NbPicsPlusVoie1);
							fprintf(FileSavePeak,"%f\t",Lise.Resultats[indRes].PicsPlusVoie2[j].XRel);
							fprintf(FileSavePeak,"%f\t",Lise.Resultats[indRes].PicsPlusVoie2[j].Qualite);
							fprintf(FileSavePeak,"%f\t",Lise.Resultats[indRes].PicsPlusVoie2[j].Intensite);
						}
						// Sens négatif
						fprintf(FileSavePeak,"Dir0\t");
						for(j = 0;j<Lise.Resultats[indRes].NbPicsMoinsVoie2;j++)
						{
							fprintf(FileSavePeak,"Pic%i\t",j+Lise.Resultats[indRes].NbPicsMoinsVoie1);
							fprintf(FileSavePeak,"%f\t",Lise.Resultats[indRes].PicsMoinsVoie2[j].XRel);
							fprintf(FileSavePeak,"%f\t",Lise.Resultats[indRes].PicsMoinsVoie2[j].Qualite);
							fprintf(FileSavePeak,"%f\t",Lise.Resultats[indRes].PicsMoinsVoie2[j].Intensite);
						}
						fflush(FileSavePeak);
					}
				}
			}
		}
	}

	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tFunction Write Peaks Success");
	}

	return STATUS_OK;
}

int CalculateIndexBestQualityPeak(INDEXPEAK* matchingPeak, int* IndexTab, int NumPeakToSearch, double* Quality, double QualityThreshold)
{
	// tri pour sortir les index des meilleur pics
	{for(int i = 0;i<NB_PEAK_MAX;i++)
	{
		for(int j = i;j<NB_PEAK_MAX;j++)
		{
			if(matchingPeak[i].Quality < matchingPeak[j].Quality)
			{
				INDEXPEAK temp;
				temp = matchingPeak[i];
				matchingPeak[i] = matchingPeak[j];
				matchingPeak[j] = temp;
			}
		}
	}}

	int i;
	for(i = 0;i<NumPeakToSearch;i++)
	{
		if( matchingPeak[i].Quality < QualityThreshold ) break;
		IndexTab[i] = matchingPeak[i].index;
	}
	int NumPeaksAboveThreshold=i;

	// on réordonne les pics sélectionnés par d'index croissant
	{for(int i = 0;i<NumPeaksAboveThreshold;i++)
	{
		for(int j = i;j<NumPeaksAboveThreshold;j++)
		{
			if(IndexTab[i] > IndexTab[j])
			{
				int temp;
				temp = IndexTab[i];
				IndexTab[i] = IndexTab[j];
				IndexTab[j] = temp;
			}
		}
	}}
	{for(;i<NB_PEAK_MAX;i++)
	{
		IndexTab[i] = IndexTab[ ((i-1)>=0)?(i-1):0 ];
	}}
	// on va chercher l'index pour récupérer la qualité
	//int Index = NumPeakToSearch-1;
	//if(NumPeakToSearch > 0)
	//{
	//	*Quality = matchingPeak[Index].Quality;
	//}
	//else
	//{
	//	*Quality = 0.0;
	//}
	*Quality = 0.0;
	return NumPeaksAboveThreshold;
}

void MatMatDisplayResult(PERIOD_RESULT* result,LISE& Lise,bool bEmulated)
{
// seulement en debug
#ifdef _DEBUG
	
	// seulement en mode emulé
	if(bEmulated)
	{
		// on va créer le fichier dans le répertoire qui va bien
		char AbsModulePath[512]; char FileNamePath[512]; 
#ifdef FDE
		GetModuleFileName(GetModuleHandle("FogaleProbe.dll"),AbsModulePath,512);
		SPG_PathOnly(AbsModulePath);
#else
		wchar_t path[512];
		GetModuleFileName(GetModuleHandle(NULL), path, 512);
		strcpy_s(AbsModulePath, fdstring(&path[0]));
#endif
		SPG_ConcatPath(FileNamePath,AbsModulePath,"MatmatResultPeriod.txt");

		// on crée le fichier
		FILE* fichier = fopen(FileNamePath,"wb");

		// infos qualité
		fprintf(fichier,"Quality Infos -------------------------------\r\n");
		fprintf(fichier,"Quality: %f \r\n",result->fQuality);
		fprintf(fichier,"Quality Plus: %f \r\n",result->fQualityPlus);
		fprintf(fichier,"Quality Moins: %f \r\n",result->fQualityMoins);
		
		// infos sur les pics retenus
		fprintf(fichier,"Peaks matching ------------------------------\r\n");
		for(int i =0;i<8;i++)
		{
			fprintf(fichier,"Num %i:\tPlus: %i\tMoins: %i\r\n",i,result->PkIndexPlus[i],result->PkIndexMoins[i]);
		}

		// infos sur les pics retenus
		fprintf(fichier,"Thickness -----------------------------------\r\n");
		{for(int i =0;i<8;i++)
		{
			fprintf(fichier,"Num %i:\tTh: %f\r\n",i,result->fThickness[i]);
		}}

		// infos sur le matchmode
		fprintf(fichier,"Matching Mode -----------------------------------\r\n");
		if(result->MatchMode == MatchingSucess) fprintf(fichier,"General: MATCHING SUCCESS\r\n");
		else if(result->MatchMode == BestPeak) fprintf(fichier,"General: BEST PEAKS\r\n");
		else fprintf(fichier,"General: ERROR\r\n");

		// plus
		if(result->MatchModePlus == MatchingSucess) fprintf(fichier,"Plus: MATCHING SUCCESS\r\n");
		else if(result->MatchModePlus == BestPeak) fprintf(fichier,"Plus: BEST PEAKS\r\n");
		else fprintf(fichier,"Plus: ERROR\r\n");

		//moins
		if(result->MatchModePlus == MatchingSucess) fprintf(fichier,"Moins: MATCHING SUCCESS\r\n");
		else if(result->MatchModePlus == BestPeak) fprintf(fichier,"Moins: BEST PEAKS\r\n");
		else fprintf(fichier,"Moins: ERROR\r\n");

		// sample information
		fprintf(fichier,"Sample Infos -------------------------------\r\n");
		fprintf(fichier,"Sample Number layer: %i \r\n",Lise.sample.NbThickness);
		fprintf(fichier,"Sample Quality Threshold: %f \r\n",Lise.sample.QualityThreshold);

		// on ferme le fichier
		fclose(fichier);
	}
#endif
}

// fonction pour forcer une résultat en bestpeaks
int ForceResultToBestPeak(LISE& Lise,PERIOD_RESULT* result)
{
	// on définit le nombre de thickness et on va rechercher parmis les pics les n meilleurs
	result->iNbThickness = Lise.sample.NbThickness;
	INDEXPEAK matchingPeakPlus[NB_PEAK_MAX];
	{for(int i = 0; i<NB_PEAK_MAX;i++)
	{
		 matchingPeakPlus[i].index = i;
		 matchingPeakPlus[i].Quality = result->PicsPlusVoie1[i].Qualite;
	}}

	int NumPeaksAboveThreshold = CalculateIndexBestQualityPeak(matchingPeakPlus,result->PkIndexPlus,result->iNbThickness+1,&result->fQualityPlus, Lise.sample.QualityThreshold);
	
	// ici le matching n'a pas marché, mais on retourne une valeur sur les meilleurs pics
	result->fQualityPlus = 0.0;
	
	// on définit le mode de matching comme celui des n meilleurs pics
	result->MatchModePlus = BestPeak;

	// on logue le mode
	if(Lise.bDebugProcess == true)
	{		
		LogfileF(*Lise.Log,"[LISEED]\t[MATCHING MODE]\tBest Peak Mode, PLUS WAY");
	}

	// on définit le nombre de thickness et on va rechercher parmis les pics les n meilleurs
	result->iNbThickness = Lise.sample.NbThickness;
	INDEXPEAK matchingPeakMoins[NB_PEAK_MAX];
	{for(int i = 0; i<NB_PEAK_MAX;i++)
	{
		 matchingPeakMoins[i].index = i;
		 matchingPeakMoins[i].Quality = result->PicsMoinsVoie1[i].Qualite;
	}}
	NumPeaksAboveThreshold = CalculateIndexBestQualityPeak(matchingPeakMoins,result->PkIndexMoins,result->iNbThickness+1,&result->fQualityMoins, Lise.sample.QualityThreshold);
	
	// ici le matching n'a pas marché mais on retourne une valeur avec quality = 0
	result->fQualityMoins = 0.0;
	
	// mode du matching
	result->MatchModeMoins = BestPeak;
	
	// on logue le mode utilisé
	if(Lise.bDebugProcess == true)
	{		
		LogfileF(*Lise.Log,"[LISEED]\t[MATCHING MODE]\tBest Peak Mode, MINUS WAY");
	}
			
	// on force le result en bestpeaks
	result->MatchMode = BestPeak;

	return 0;
}

// Calibration : moyennes des sens + et -.
int FindThickness(LISE& Lise, float fPositionRefOpt, float fToleranceRefOpt)
{
	// Utilisation de PeakMatch

	SAMPLEDEFINITION sample = Lise.sample;
	PERIOD_RESULT* result = &Lise.Resultats[RBP_GetNMinusOne(Lise.IndicePeriod)];

	if (result->NbPicsMoinsVoie1>NB_PEAK_MAX || result->NbPicsMoinsVoie2>NB_PEAK_MAX || result->NbPicsPlusVoie1>NB_PEAK_MAX || result->NbPicsPlusVoie2>NB_PEAK_MAX)
	{
		if(Lise.bDebug == true)
		{
			if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tToo many peaks in FindThickness.");
		}
		return STATUS_FAIL;
	}

	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tEntering Find Thickness");
	}

	// si pas d'échantillon définit
	if (!sample.bDefined)
	{
		int j;
		
		// Attention, on en prend pas en compte la référence optique mais le pic est quand même dans le signal.
		j = 1;

		// on loggue le fait que l'échantillon ne soit pas définit
		if(Lise.bDebug == true)	LogfileF(*Lise.Log,"[LISEED]\t[FindThickness]\tWarning - Sample not define in function find thickness");

		int i = 0;
		result->iNbThickness = 0;
		result->fQuality = 1.0;
		// On vérifie qu'on a au moins 2 pics pour calculer une épaisseur.
		bool bPicsDispo = false;
		if (result->NbPicsPlusVoie1 > j+1 && result->NbPicsMoinsVoie1 > j+1)
		{
			bPicsDispo = true;
		}
		while (bPicsDispo && result->iNbThickness < NB_PEAK_MAX)
		{
			// Moyennes des sens + et -.
			result->fThickness[i] = (result->PicsPlusVoie1[j+1].XRel + result->PicsMoinsVoie1[j+1].XRel) / 2.0 - (result->PicsPlusVoie1[j].XRel + result->PicsMoinsVoie1[j].XRel) / 2.0;
			result->iNbThickness++;
			i++;
			j++;
			// On vérifie qu'on a au moins 2 pics pour calculer une épaisseur.
			bPicsDispo = false;
			if (result->NbPicsPlusVoie1 > j+1 && result->NbPicsMoinsVoie1 > j+1)
			{
				bPicsDispo = true;
			}
		}
	}
	else
	{ // On définit un tableau assez grand pour contenir les pics des deux voies... On va envoyer les pics des deux voies les uns à la suite des autres, et faire un tri dans l'ordre des XRels Croissants
		MEASPEAK MeasPositif[2 * NB_PEAK_MAX];
		// On enregistre aussi les intensités des pics
		double MeasPositifIntensite[2 * NB_PEAK_MAX];
	
		int k = 0;
		for (k=0; k<NB_PEAK_MAX*2; k++)
		{
			MeasPositif[k].Position = 20000.0;
			MeasPositif[k].Quality = 0.0;
			MeasPositifIntensite[k] = 0.0;
		}
		int i;

		// Sens positif
		for(i=0;i<result->NbPicsPlusVoie1;i++)
		{
			MeasPositif[i].Quality = result->PicsPlusVoie1[i].Qualite;
			MeasPositif[i].Position = result->PicsPlusVoie1[i].XRel;
			MeasPositifIntensite[i]= result->PicsPlusVoie1[i].Intensite;
			result->PkOrigPlus[i] = 1;	// Voie 1
		}
		if(Lise.NombredeVoie == 2)
		{ // copie des pics de la voie 2 dans le buffer
			for(i=0;i<result->NbPicsPlusVoie2;i++)
			{
				MeasPositif[i+result->NbPicsPlusVoie1].Quality = result->PicsPlusVoie2[i].Qualite;
				MeasPositif[i+result->NbPicsPlusVoie1].Position = result->PicsPlusVoie2[i].XRel;
				MeasPositifIntensite[i+result->NbPicsPlusVoie1]= result->PicsPlusVoie2[i].Intensite;
				result->PkOrigPlus[i+result->NbPicsPlusVoie1] = 2;	// Voie 2
			}
		}

		if(Lise.NombredeVoie == 2)
		{ // cas deux voies de mesure
			// rangements des pics par Xrel croissant
			for(i = 0 ;i<result->NbPicsPlusVoie1+result->NbPicsPlusVoie2; i++)
			{
				for(int j = 1 ; j < result->NbPicsPlusVoie1+result->NbPicsPlusVoie2 - i ; j++)
				{
					if(MeasPositif[j].Position < MeasPositif[j-1].Position)
					{
						MEASPEAK MeasPeakEchange;
						MeasPeakEchange = MeasPositif[j];
						MeasPositif[j] = MeasPositif[j-1];
						MeasPositif[j-1] = MeasPeakEchange;

						int OrigEchange = result->PkOrigPlus[j];
						result->PkOrigPlus[j] = result->PkOrigPlus[j-1];
						result->PkOrigPlus[j-1] = OrigEchange;
					}
				}
			}
		}

		// on définit un mode de matching par défaut
		result->MatchModePlus = NoMatching;
		result->bMatchSuccessPlus = false;

		// ############  refine Auto airgap du 1er matching reussi #################

		// init des params pour effectuer un refine, uniquement sur la voie 1
		double RefineValues[2];memset(RefineValues,0,2*sizeof(double)); 
		int iIndexRefine =0;double LastPos = 0;bool bFound = false;
		for(int i =0;i<MAX_THICKNESS;i++)
		{
			// Ici la condition est le quality Threshold
			double _dThresholdValue = Lise.fAutoAirgapDetectionfactor * Lise.UnusedPeaksIntensity;
			if(!Lise.bUseAutoAirgapDetectionFactor)
			{
				if(Lise.fThresholAmplitudeAirgap >= Lise.fThresholdSwithAGAutoAgTh)
				{
					_dThresholdValue = Lise.fThresholAmplitudeAirgap;
				}
				else
				{
					_dThresholdValue = Lise.fThresholdSwithAGAutoAgTh;
				}
			}

			// On va la remplacer par le premier pic sortant du bruit
			//if(MeasPositifIntensite[i] >= 3 * Lise.UnusedPeaksIntensity)
			if(MeasPositifIntensite[i] >= _dThresholdValue )
			{
				//RefineValues[iIndexRefine] = MeasPositif[i].Position - LastPos;
				RefineValues[iIndexRefine] = result->PicsPlusVoie1[i].XRel - LastPos;
				LastPos = RefineValues[iIndexRefine];
				iIndexRefine++; 

				// refine values ont étés trouvées
				if(iIndexRefine>=2)
				{
					bFound = true;
					break;
				}
			}
		}

		if(Lise.bUseAirGapAuto)
		{
			// Seul cas ou on ne fait pas l'air gap auto : si on est en single shot ET on fait le refine ET ce n'est pas le premier de la moyenne
			if (!(Lise.AcqMode == SingleShot && Lise.bUseUpdateFirstAirGapDef && !Lise.iFirstMatchingSucces))
			{
				// cas dans lequel on tombe pour le mode continu
				if(bFound)
				{
					// Si le premier pic n'est pas dans la tolérance de recherche du pic de référence, on ne fait pas d'air gap auto.
					if (abs((double)fPositionRefOpt - RefineValues[0] * (Lise.dRefWaveLengthNm / 4.0 / 1000.0)) < fToleranceRefOpt)
					{
						Lise.sample.PkDef[0].ExpectedPosition = RefineValues[0];
						Lise.sample.PkDef[0].Tolerance = Lise.ComparisonTolerance / Lise.dStepMicron;
						
						Lise.sample.PkDef[1].ExpectedPosition = RefineValues[1];
						Lise.sample.PkDef[1].Tolerance = Lise.ComparisonTolerance / Lise.dStepMicron;

						Lise.bSampleDefUpdated = true;

						// si la sortie peaks est demandee
						if(Lise.bAllowSavePeaks)
						{
							LogPeakRefine(Lise,result,RefineValues,false);
							LogPeakRefine(Lise,result,RefineValues,true);
						}

						// log sur l'air gap auto
                        if(Lise.bDebugProcess == true)
                        {
                            if(Lise.bDebugProcess == true)
                            {
						        LogfileF(*Lise.Log,"[Auto AirGap]\t[AGAUTO]\tPos 1 : %f\tPos 2 : %f",Lise.sample.PkDef[0].ExpectedPosition,Lise.sample.PkDef[1].ExpectedPosition);
                            }
                        }
					}
				}
			}
		}

		// ############  refine Auto airgap du 1er matching reussi #################

		// on effectue le matching
		if(Lise.NombredeVoie == 2){
			DefMatch(MeasPositif,result->NbPicsPlusVoie1+result->NbPicsPlusVoie2,Lise.sample,result->fQualityPlus,result->PkIndexPlus, NB_PEAK_MAX);
		}
		else{		
			DefMatch(MeasPositif,result->NbPicsPlusVoie1,Lise.sample,result->fQualityPlus,result->PkIndexPlus, NB_PEAK_MAX);
		}

		// on prend comme qualité la qualité du plus mauvais pic
		if (result->fQualityPlus>0.0)
		{
			// ############  refine la definition avec l'airgap du 1er matching reussi #################
			result->bMatchSuccessPlus = true;

			// on a détecté le first airgap
			if(Lise.iFirstMatchingSucces && Lise.bUseUpdateFirstAirGapDef)
			{
				Lise.iFirstMatchingSucces=false;					
				Lise.sample.PkDef[1].ExpectedPosition = MeasPositif[result->PkIndexPlus[1]].Position - MeasPositif[result->PkIndexPlus[0]].Position;
				Lise.sample.PkDef[1].Tolerance = Lise.ComparisonTolerance / Lise.dStepMicron;
				Lise.bSampleDefUpdated = true;
			}

			// ############  refine la definition avec l'airgap du 1er matching reussi #################

			// Initialisation du minimum : qualité du premier pic choisi qui n'est pas le pic de référence
			//double fMinQuality = result->PicsPlusVoie1[result->PkIndexPlus[1]].Qualite;
			double fMinQuality = MeasPositif[result->PkIndexPlus[1]].Quality;

			// on recherche la plus basse qualité parmis les n+1 pics qui ont servi à trouver les n épaisseurs (sans le pic de référence)
			for(int k = 2;k < sample.NbThickness + 1 ;k++)
			{
				//if( result->PicsPlusVoie1[result->PkIndexPlus[k]].Qualite < fMinQuality )
				if( MeasPositif[result->PkIndexPlus[k]].Quality < fMinQuality )
				{
					//fMinQuality = result->PicsPlusVoie1[result->PkIndexPlus[k]].Qualite;
					fMinQuality = MeasPositif[result->PkIndexPlus[k]].Quality;
				}
			}

			// on associe la qualité du plus mauvais pic à la mesure
			result->fQualityPlus = fMinQuality;

			// on définit le mode de matching
			result->MatchModePlus = MatchingSucess;

			// on logue le mode
			if(Lise.bDebugProcess == true)
			{		
				LogfileF(*Lise.Log,"[LISEED]\t[MATCHING MODE]\tDefmatch Sucess Mode, PLUS WAY");

				// log de la valeur de qualité
				LogfileF(*Lise.Log,"[LISEED]\t[MATCHING MODE INFO]\tQuality Value = %f",fMinQuality);

				// log des deux premiers index définis pour la mesure
				LogfileF(*Lise.Log,"[LISEED]\t[MATCHING MODE INFO]\tFirst Index = %i, Second index equal = %i",result->PkIndexPlus[0],result->PkIndexPlus[1]);
			}
		}
		else
		{
			// le matching n'a pas marché, puisque qualité de zéro et il renvoie les index 0,1,2
			// on définit le nombre de thickness et on va rechercher parmis les pics les n meilleurs
			result->iNbThickness = Lise.sample.NbThickness;
			INDEXPEAK matchingPeak[NB_PEAK_MAX];
			for(int i = 0; i<NB_PEAK_MAX;i++)
			{
				 matchingPeak[i].index = i;
				 //matchingPeak[i].Quality = result->PicsPlusVoie1[i].Qualite;
				 matchingPeak[i].Quality = MeasPositif[i].Quality;
			}

			int NumPeaksAboveThreshold = CalculateIndexBestQualityPeak(matchingPeak,result->PkIndexPlus,result->iNbThickness+1,&result->fQualityPlus, Lise.sample.QualityThreshold);
			
			// ici le matching n'a pas marché, mais on retourne une valeur sur les meilleurs pics
			result->fQualityPlus = 0.0;
			
			// on définit le mode de matching comme celui des n meilleurs pics
			result->MatchModePlus = BestPeak;

			// on logue le mode
			if(Lise.bDebugProcess == true)
			{		
				LogfileF(*Lise.Log,"[LISEED]\t[MATCHING MODE]\tBest Peak Mode, PLUS WAY");
			}
		}

		// Calcul des épaisseurs pour le sens positif
		// Sens négatif
		MEASPEAK MeasNegatif[2*NB_PEAK_MAX];
		for (k=0; k<NB_PEAK_MAX*2; k++)
		{
			MeasNegatif[k].Position = 20000.0;
			MeasNegatif[k].Quality = 0.0;
		}

		for(i=0;i<result->NbPicsMoinsVoie1;i++)
		{
			MeasNegatif[i].Quality = result->PicsMoinsVoie1[i].Qualite;
			MeasNegatif[i].Position = result->PicsMoinsVoie1[i].XRel;
			result->PkOrigMoins[i] = 1;
		}

		// cas deux voies de mesure
		if(Lise.NombredeVoie == 2)
		{ 
			// copie des pics de la voie 2 dans le buffer
			for(i=0;i<result->NbPicsMoinsVoie2;i++)
			{
				MeasNegatif[i+result->NbPicsMoinsVoie1].Quality = result->PicsMoinsVoie2[i].Qualite;
				MeasNegatif[i+result->NbPicsMoinsVoie1].Position = result->PicsMoinsVoie2[i].XRel;
				result->PkOrigMoins[i+result->NbPicsMoinsVoie1] = 2;
			}

			// rangements des pics par Xrel croissant
			for(i = 0 ;i<result->NbPicsMoinsVoie1+result->NbPicsMoinsVoie1; i++)
			{
				for(int j = 1 ; j < result->NbPicsMoinsVoie1+result->NbPicsMoinsVoie2 - i ; j++)
				{
					if(MeasNegatif[j].Position < MeasNegatif[j-1].Position)
					{
						MEASPEAK MeasPeakEchange;
						MeasPeakEchange = MeasNegatif[j];
						MeasNegatif[j] = MeasNegatif[j-1];
						MeasNegatif[j-1] = MeasPeakEchange;

						int OrigEchange = result->PkOrigMoins[j];
						result->PkOrigMoins[j] = result->PkOrigMoins[j-1];
						result->PkOrigMoins[j-1] = OrigEchange;
					}
				}
			}
		}

		result->MatchModeMoins = NoMatching;
		result->bMatchSuccessMoins = false;

		if(Lise.NombredeVoie == 2){
			DefMatch(MeasNegatif,result->NbPicsMoinsVoie1+result->NbPicsMoinsVoie2,Lise.sample,result->fQualityMoins,result->PkIndexMoins, NB_PEAK_MAX);
		}
		else{	
			DefMatch(MeasNegatif,result->NbPicsMoinsVoie1,Lise.sample,result->fQualityMoins,result->PkIndexMoins, NB_PEAK_MAX);
		}
		// cas ou l'on a eu auparavant un matching des best peak, alors on force le matching best peak sur le sens moins
		if(result->MatchModePlus == BestPeak)
		{
			result->fQualityMoins = 0.0;
		}

		// on prend comme qualité la qualité du plus mauvais pic
		if (result->fQualityMoins>0.0)
		{
			result->bMatchSuccessMoins = true;

			// Initialisation du minimum : qualité du premier pic choisi qui n'est pas le pic de référence
			//double fMinQuality = result->PicsMoinsVoie1[result->PkIndexMoins[1]].Qualite;
			double fMinQuality = MeasNegatif[result->PkIndexMoins[1]].Quality;
			for(int k = 2;k < sample.NbThickness + 1 ;k++)
			{
				//if( result->PicsMoinsVoie1[result->PkIndexMoins[k]].Qualite < fMinQuality )
				if( MeasNegatif[result->PkIndexMoins[k]].Quality < fMinQuality )
				{
					//fMinQuality = result->PicsMoinsVoie1[result->PkIndexMoins[k]].Qualite;
					fMinQuality = MeasNegatif[result->PkIndexMoins[k]].Quality;
				}
			}
			result->fQualityMoins = fMinQuality;
			// mode pour le matching
			result->MatchModeMoins = MatchingSucess;

			// on logue le mode
			if(Lise.bDebugProcess == true)
			{		
				LogfileF(*Lise.Log,"[LISEED]\t[MATCHING MODE]\tDefMatch Sucess Mode, MINUS WAY");
			}
		}
		else
		{
			// le matching n'a pas marché, puisque qualité de zéro et il renvoie les index 0,1,2
			// on définit le nombre de thickness et on va rechercher parmis les pics les n meilleurs
			result->iNbThickness = Lise.sample.NbThickness;
			INDEXPEAK matchingPeak[NB_PEAK_MAX];
			for(int i = 0; i<NB_PEAK_MAX;i++)
			{
				matchingPeak[i].index = i;
				//matchingPeak[i].Quality = result->PicsMoinsVoie1[i].Qualite;
				matchingPeak[i].Quality = MeasNegatif[i].Quality;
			}
			int NumPeaksAboveThreshold = CalculateIndexBestQualityPeak(matchingPeak,result->PkIndexMoins,result->iNbThickness+1,&result->fQualityMoins, Lise.sample.QualityThreshold);
			
			// ici le matching n'a pas marché mais on retourne une valeur avec quality = 0
			result->fQualityMoins = 0.0;
			
			// mode du matching
			result->MatchModeMoins = BestPeak;
			
			// on logue le mode utilisé
			if(Lise.bDebugProcess == true)
			{		
				LogfileF(*Lise.Log,"[LISEED]\t[MATCHING MODE]\tBest Peak Mode, MINUS WAY");
			}
		}

		// on définit le mode pour la mesure
		if(result->MatchModePlus == MatchingSucess && result->MatchModeMoins == MatchingSucess)	
		{ 
			// cas ou les deux matchings sont en MatchingSucess
			result->MatchMode = MatchingSucess;
		}
		else if(result->MatchModePlus == MatchingSucess && result->MatchModeMoins == BestPeak)
		{ 
			// cas ou l'un des deux matching est BestPeak (ce ne peut être que le deuxième car le cas BestPeak (plus) et MatchingSuccess (mois) est écarté)
			result->MatchMode = BestPeak;
			
			// on doit faire le cas BestPeak au sens moins
			result->iNbThickness = Lise.sample.NbThickness;
			INDEXPEAK matchingPeak[NB_PEAK_MAX];
			for(int i = 0; i<NB_PEAK_MAX;i++)
			{
				 matchingPeak[i].index = i;
				 //matchingPeak[i].Quality = result->PicsPlusVoie1[i].Qualite;
				matchingPeak[i].Quality = MeasNegatif[i].Quality;
			}

			int NumPeaksAboveThreshold = CalculateIndexBestQualityPeak(matchingPeak,result->PkIndexPlus,result->iNbThickness+1,&result->fQualityPlus, Lise.sample.QualityThreshold );
			
			// ici le matching n'a pas marché, mais on retourne une valeur sur les meilleurs pics
			result->fQualityPlus = 0.0;
			
			// on définit le mode de matching comme celui des n meilleurs pics
			result->MatchModePlus = BestPeak;
			
			// on logue le mode
			if(Lise.bDebugProcess == true)
			{		
				LogfileF(*Lise.Log,"[LISEED]\t[MATCHING MODE]\tBest Peak Mode, PLUS WAY");
			}
		}
		else	
		{
			result->MatchMode = BestPeak;
		}

		// code pour faire le test de nombre best peak ou matching success
		if (result->fQualityPlus >= 0 && result->fQualityMoins >= 0)
		{
			result->iNbThickness = Lise.sample.NumPks - 1;
		}
		// fin de code

		// booléen pour le mode ou le matching success devient un bestpeak à cause d'une différence entre sens plus et moins trop élevé
		bool bIsDifferencePlusMinusWay = false;

		// Remplissage du tableau des épaisseurs
		for (i=0; i<result->iNbThickness; i++)
		{
			if(Lise.bDebugProcess == true)
			{
				// on loggue la couche que l'on calcule
				LogfileF(*Lise.Log,"[LISEED]\t[THREAD]\tCalculate thickness %i",i);
			}

			// cas ou l'on traite la voie numéro 2
			/*if(Lise.NombredeVoie == 2)
			{	
				// Avec séparation des sens + et -
				result->fThicknessPlus[i] = MeasPositif[result->PkIndexPlus[i+1]].Position - MeasPositif[result->PkIndexPlus[i]].Position;
				result->fThicknessPlus[i] *= Lise.dStepMicron / sample.fIndex[i];
				result->fThicknessMoins[i] = MeasNegatif[result->PkIndexMoins[i+1]].Position - MeasNegatif[result->PkIndexMoins[i]].Position;
				result->fThicknessMoins[i] *= Lise.dStepMicron / sample.fIndex[i];
				
				// on calcule la thickness sur l'aller retour
				result->fThickness[i] = (result->fThicknessPlus[i] + result->fThicknessMoins[i]) / 2.0;

				if(Lise.bDebugProcess == true)
				{	
					// on a trouvé les thickness plus et moins
					LogfileF(*Lise.Log,"[LISEED]\t[THREAD]\tThickness +/- found");
				
					// on a trouvé les thickness plus et moins
					LogfileF(*Lise.Log,"[LISEED]\t[THREAD]\tThickness measured = %f",result->fThickness[i]);
				}
			}
			else*/
			{
				// cas de la configuration en double sens
				if( Lise.iConfigSensUnique == 0)
				{
					//if(Lise.NombredeVoie == 2)
					{
						result->fThicknessPlus[i] = MeasPositif[result->PkIndexPlus[i+1]].Position - MeasPositif[result->PkIndexPlus[i]].Position;
						result->fThicknessPlus[i] *= Lise.dStepMicron / sample.fIndex[i];
						result->fThicknessMoins[i] = MeasNegatif[result->PkIndexMoins[i+1]].Position - MeasNegatif[result->PkIndexMoins[i]].Position;
						result->fThicknessMoins[i] *= Lise.dStepMicron / sample.fIndex[i];	
					}
					/*else{
						// Avec séparation des sens + et -
						result->fThicknessPlus[i] = result->PicsPlusVoie1[result->PkIndexPlus[i+1]].XRel - result->PicsPlusVoie1[result->PkIndexPlus[i]].XRel;
						result->fThicknessPlus[i] *= Lise.dStepMicron / sample.fIndex[i];
						result->fThicknessMoins[i] = result->PicsMoinsVoie1[result->PkIndexMoins[i+1]].XRel - result->PicsMoinsVoie1[result->PkIndexMoins[i]].XRel;
						result->fThicknessMoins[i] *= Lise.dStepMicron / sample.fIndex[i];
					}*/
					// MA 27/01/2009 : tests de comparaison sur les mesures d'épaisseurs dans les sens + et -, si l'écart entre les deux mesure est plus petit que la tolérance, alors la mesure est valide
					if( fabs(result->fThicknessPlus[i] - result->fThicknessMoins[i]) >= Lise.ComparisonTolerance)
					{
						//LogPeakComment(Lise,"PeakUsedForSampleDef.txt","First test, difference between plus and minus");
						if(Lise.bDebug == true)
						{		
							// Log dans FogaleProbeLog
							LogfileF(*Lise.Log,"[LISEED]\t[WARNING QUALITY]\tDifference between plus and minus ways is superior to tolerance value => Quality value equal to ZERO");
							LogfileF(*Lise.Log,"[LISEED]\t[WARNING QUALITY]\tLayer : %i\tPlus : %f\tMoins : %f\t",i,result->fThicknessPlus[i],result->fThicknessMoins[i]);
						}

						// ATTENTION seulement dans le cas du matching success
						if(result->MatchMode == MatchingSucess)
						{
							// Quality à Zero
							result->fQuality = 0.0;
							result->bMatchSuccessGoAndBack = false;

							// ATTENTION, il faut repasser en mode BestPeaks, on retraite
							bIsDifferencePlusMinusWay = true;
							
							// on arrête la boucle
							break;				
						}
						else
						{
							result->fThickness[i] = 0.0;
							result->fQuality = 0.0;
						}
					}
					else
					{
						// fin de modif
						result->fThickness[i] = (result->fThicknessPlus[i] + result->fThicknessMoins[i]) / 2.0;
						result->bMatchSuccessGoAndBack = true;
					}

					if(Lise.bDebugProcess == true)
					{
						// on a trouvé les thickness plus et moins
						LogfileF(*Lise.Log,"[LISEED]\t[THREAD]\tThickness +/- found");

						// on a trouvé les thickness plus et moins
						LogfileF(*Lise.Log,"[LISEED]\t[THREAD]\tThickness measured = %f",result->fThickness[i]);
					}
				}
				else
				{
					result->fThicknessPlus[i] = MeasPositif[result->PkIndexPlus[i+1]].Position - MeasPositif[result->PkIndexPlus[i]].Position;
					result->fThicknessPlus[i] *= Lise.dStepMicron / sample.fIndex[i];
					result->fThicknessMoins[i] = MeasNegatif[result->PkIndexMoins[i+1]].Position - MeasNegatif[result->PkIndexMoins[i]].Position;
					result->fThicknessMoins[i] *= Lise.dStepMicron / sample.fIndex[i];	
					// Avec séparation des sens + et -
					/*result->fThicknessPlus[i] = result->PicsPlusVoie1[result->PkIndexPlus[i+1]].XRel - result->PicsPlusVoie1[result->PkIndexPlus[i]].XRel;
					result->fThicknessPlus[i] *= Lise.dStepMicron / sample.fIndex[i];
					result->fThicknessMoins[i] = result->PicsMoinsVoie1[result->PkIndexMoins[i+1]].XRel - result->PicsMoinsVoie1[result->PkIndexMoins[i]].XRel;
					result->fThicknessMoins[i] *= Lise.dStepMicron / sample.fIndex[i];*/

					if(Lise.bDebugProcess == true)
					{
						// on a trouvé les thickness plus et moins
						LogfileF(*Lise.Log,"[LISEED]\t[THREAD]\tThickness +/- found");

						// on a trouvé les thickness plus et moins
						LogfileF(*Lise.Log,"[LISEED]\t[THREAD]\tThickness measured = %f",result->fThickness[i]);
					}

					// on définit le mode pour la mesure
					result->MatchMode = result->MatchModePlus;
				}
			}
		}

		// différence entre sens + et -
		if(bIsDifferencePlusMinusWay)
		{
			result->bMatchSuccessGoAndBack = false;
			for(int i=0;i<result->iNbThickness;i++)
			{
				// on recalcule en forçant la mesure en BestPeaks
				ForceResultToBestPeak(Lise,result);
				//LogPeakComment(Lise,"PeakUsedForSampleDef.txt","Force to best Peak");

				// on recalcule avec les nouveaux résultats taggués en BestPeaks
				//if(Lise.NombredeVoie == 2)
				{
					result->fThicknessPlus[i] = MeasPositif[result->PkIndexPlus[i+1]].Position - MeasPositif[result->PkIndexPlus[i]].Position;
					result->fThicknessPlus[i] *= Lise.dStepMicron / sample.fIndex[i];
					result->fThicknessMoins[i] = MeasNegatif[result->PkIndexMoins[i+1]].Position - MeasNegatif[result->PkIndexMoins[i]].Position;
					result->fThicknessMoins[i] *= Lise.dStepMicron / sample.fIndex[i];	
				}
				/*else{
					result->fThicknessPlus[i] = result->PicsPlusVoie1[result->PkIndexPlus[i+1]].XRel - result->PicsPlusVoie1[result->PkIndexPlus[i]].XRel;
					result->fThicknessPlus[i] *= Lise.dStepMicron / sample.fIndex[i];
					result->fThicknessMoins[i] = result->PicsMoinsVoie1[result->PkIndexMoins[i+1]].XRel - result->PicsMoinsVoie1[result->PkIndexMoins[i]].XRel;
					result->fThicknessMoins[i] *= Lise.dStepMicron / sample.fIndex[i];
				}*/
				// comparaison entre sens + et - pas bonne
				if( fabs(result->fThicknessPlus[i] - result->fThicknessMoins[i]) >= Lise.ComparisonTolerance)
				{
					//LogPeakComment(Lise,"PeakUsedForSampleDef.txt","Second test, difference between plus and minus");
					result->fThickness[i] = 0.0;
					result->fQuality = 0.0;
				}
				else
				{
					// calcul de l'épaisseurs
					result->fThickness[i] = (result->fThicknessPlus[i] + result->fThicknessMoins[i]) / 2.0;
				}
			}
		}

		// code pour faire l'ajout du nbre de matching success/best peaks et dire qu'on a toutes les épaisseurs
		if( Lise.iConfigSensUnique == 0)
		{
			if (result->fQualityPlus >= 0 && result->fQualityMoins >= 0)
			{
				// on force le resultLen
				Lise.WaitNThicknessStop.Len = Lise.ResultLen;
				
				// on incrémente le ring buffer si on est en l'attente de resultats
				if(Lise.bNThicknessNotReady) RBP_Inc(Lise.WaitNThicknessStop);

				//result->iNbThickness = Lise.sample.NumPks - 1;

				// Modif MA, on change la variable à incrémenter
				if(result->MatchMode == MatchingSucess)
				{
					result->iCounterMatchingS=Lise.iThicknessMatchingPeakMode;
					Lise.iThicknessMatchingPeakMode++;
				}
				else 
				{
					result->iCounterBestPeaks=Lise.iThicknessBestPeakMode;
					Lise.iThicknessBestPeakMode++;
				}
				// Lise.iThicknessAboveQThreshold++;
				result->fQuality = (result->fQualityPlus + result->fQualityMoins) / 2.0;

				// cas ou l'on a trouvé les n épaisseurs intéressantes pour le single shot acq, ou dans le cas d'une mesure LimitedTime
				if((Lise.iThicknessMatchingPeakMode >= Lise.Moyenne || Lise.iThicknessBestPeakMode >= Lise.Moyenne) || (Lise.bLimitedTime && ((Lise.iThicknessMatchingPeakMode + Lise.iThicknessBestPeakMode) >= Lise.Moyenne)) )
				{
					// les n valeurs sont prêtes pour calculer la valeur à retourner pour Getthickness
					Lise.bNThicknessNotReady = false;
					
					// log de l'instruction
					if(Lise.bDebug == true)
					{		
						LogfileF(*Lise.Log,"[LISEED]\t[THREAD]\tAll Thickness found");
					}
				}
			}
			else
			{
				if(Lise.bDebug == true)
				{		
					LogfileF(*Lise.Log,"[LISEED]\t[WARNING QUALITY]\tOne of both quality value is equal to ZERO");
				}

				// pas de qualité positive dans les deux sens
				result->fQuality = 0.0;
			}
		}
		else
		{
			if (result->fQualityPlus > 0)
			{
				// on force le resultLen
				Lise.WaitNThicknessStop.Len = Lise.ResultLen;

				// on incrémente le ring buffer
				RBP_Inc(Lise.WaitNThicknessStop);

				// on récupère le nombre de thickness
				result->iNbThickness = Lise.sample.NumPks - 1;

				// test si la valeur de la qualité est suppérieure au quality threshold
				if(result->fQualityPlus > Lise.sample.QualityThreshold )
				{ 
					// Parametre permettant de synchroniser avec la thread
					Lise.iThicknessAboveQThreshold++;
					result->fQuality = result->fQualityPlus;
					
					// si le nombre de points au dessus du seuil de qualité est supérieur au nombre de points pour faire une moyenne
					if(Lise.iThicknessAboveQThreshold >= Lise.Moyenne)
					{
						Lise.bNThicknessNotReady = false;
						
						// on loggue l'info des n thickness trouvée
						if(Lise.bDebug == true)
						{		
							LogfileF(*Lise.Log,"[LISEED]\t[THREAD]\tAll Thickness found");
						}
					}
				}
			}
			else
			{
				if(Lise.bDebug == true)
				{		
					LogfileF(*Lise.Log,"[LISEED]\t[WARNING QUALITY]\tQuality plus value is equal to ZERO because value is negative or null");
				}
				result->fQuality = 0.0;
			}
		}
		// du code déplacé pour le compte des best peaks, matching success // fin
	}

	// on affiche le result courant dans le fichier
	// MatMatDisplayResult(result,Lise,true);
	
	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tFunction Find Thickness Success");
	}

	return STATUS_OK;
}

int MemorisationResultatsPeriode(LISE& Lise)
{
// Memorisation dans la structure de resultat de la derniere periode

		if(Lise.bDebug == true)
		{		
			if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tEntering Function Memorisation Period Result");
		}

		if(Lise.NombredeVoie == 2)
		{
			Lise.Resultats[Lise.IndicePeriod.N].NbSamplesPeriod = Lise.NbSamplesLastPeriod / 2;
		}
		else
		{
			Lise.Resultats[Lise.IndicePeriod.N].NbSamplesPeriod = Lise.NbSamplesLastPeriod;
		}
		//Lise.Resultats[Lise.IndicePeriod.N].NbPicsPlusVoie2 = Lise.iNbrPicsVoie2;
		//Lise.Resultats[Lise.IndicePeriod.N].NbPicsMoinsVoie2 = Lise.iIndiceVoie2 - Lise.iNbrPicsVoie2;

		Lise.Resultats[Lise.IndicePeriod.N].iNbThickness = 0;

		if(Lise.bDebug == true)
		{		
			if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tFunction Memorisation Period Result Success");
		}

	return STATUS_OK;
}

int FindNBestPeak(LISE& Lise,RING_BUFFER_POS& WriteResult, bool bRefOptTheorique,float fPositionRefOpt,float fToleranceRefOpt)
{
	int i   = 0;
    int j   = 0;
    double tmp = 0;

	int NombreDePicsATraiter = Lise.NbPicPlusProcessVoie1 + Lise.NbPicMoinsProcessVoie1;
	/* Ajout voie 2*/
	int NombreDepicsATraiterVoie2 = Lise.NbPicPlusProcessVoie2 + Lise.NbPicMoinsProcessVoie2;

	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tEntering Function Find N Best Peaks");
	}

	RING_BUFFER_POS IndiceTraitement;

	// Cas où on utilise un pic de référence théorique :
	//		- Mise à 0 de la qualité de tous les pics dans la tolérance du pic de référence.
	//		- Ajout du pic théorique, de qualité 1 et d'intensité 1 à la place du pic conservé de plus petite qualité.
	if (bRefOptTheorique)
	{
		// Mise à 0 de la qualité de tous les pics dans la tolérance du pic de référence.
		IndiceTraitement = WriteResult;
		RBP_Sub(IndiceTraitement,NombreDePicsATraiter);
		double fTolPlus = (fPositionRefOpt + fToleranceRefOpt) / (Lise.dRefWaveLengthNm / 4.0 / 1000.0);
		double fTolMoins = (fPositionRefOpt - fToleranceRefOpt) / (Lise.dRefWaveLengthNm / 4.0 / 1000.0);
		for(i = 0; i<Lise.NbPicPlusProcessVoie1; i++)
		{
			PICRESULT* PkRes = &Lise.BufferResultat[0][IndiceTraitement.N];
			if (PkRes->XRel > fTolMoins && PkRes->XRel < fTolPlus)
			{
				PkRes->Qualite = 0.0;
			}
			RBP_Inc(IndiceTraitement);
		}
		if(Lise.NombredeVoie == 2)
		{
			IndiceTraitement = Lise.WriteResultSecondChannel;
			RBP_Sub(IndiceTraitement,NombreDepicsATraiterVoie2);
			for(i = 0; i<Lise.NbPicPlusProcessVoie2;  i++)
			{
				PICRESULT* PkRes = &Lise.BufferResultat[1][IndiceTraitement.N];
				if (PkRes->XRel > fTolMoins && PkRes->XRel < fTolPlus)
				{
					PkRes->Qualite = 0.0;
				}
				RBP_Inc(IndiceTraitement);
			}
		}
		IndiceTraitement = WriteResult;
		RBP_Sub(IndiceTraitement,Lise.NbPicMoinsProcessVoie1);
		for(i = 0; i<Lise.NbPicMoinsProcessVoie1; i++)
		{
			PICRESULT* PkRes = &Lise.BufferResultat[0][IndiceTraitement.N];
			if (PkRes->XRel > fTolMoins && PkRes->XRel < fTolPlus)
			{
				PkRes->Qualite = 0.0;
			}
			RBP_Inc(IndiceTraitement);
		}
		if(Lise.NombredeVoie == 2)
		{
			IndiceTraitement = Lise.WriteResultSecondChannel;
			RBP_Sub(IndiceTraitement,Lise.NbPicMoinsProcessVoie2);
			for(i = 0; i<Lise.NbPicMoinsProcessVoie2; i++)
			{
				PICRESULT* PkRes = &Lise.BufferResultat[1][IndiceTraitement.N];
				if (PkRes->XRel > fTolMoins && PkRes->XRel < fTolPlus)
				{
					PkRes->Qualite = 0.0;
				}
				RBP_Inc(IndiceTraitement);
			}
		}
	}

	for( i = 0; i<Lise.NbPicPlusProcessVoie1;i++)
	{ // Traitement pour la voie 1 pics plus, classement par ordre de qualité croissante
		IndiceTraitement = WriteResult;
		RBP_Sub(IndiceTraitement,NombreDePicsATraiter);
		RBP_Inc(IndiceTraitement);
		for(j = 1 ; j < Lise.NbPicPlusProcessVoie1 - i ; j++)
        {
			if(Lise.BufferResultat[0][IndiceTraitement.N].Qualite > Lise.BufferResultat[0][RBP_GetNMinusOne(IndiceTraitement)].Qualite)
			{
					PICRESULT PicEchange;
					
					PicEchange = Lise.BufferResultat[0][RBP_GetNMinusOne(IndiceTraitement)];
					Lise.BufferResultat[0][RBP_GetNMinusOne(IndiceTraitement)]= Lise.BufferResultat[0][IndiceTraitement.N];
					Lise.BufferResultat[0][IndiceTraitement.N]= PicEchange ;
			}
			RBP_Inc(IndiceTraitement);
		}
	}
	// modif MA 09/09/08
	RING_BUFFER_POS FirstQValueNotGood = WriteResult;
	RBP_Sub(FirstQValueNotGood,NombreDePicsATraiter);
	RBP_Add(FirstQValueNotGood,8);
	int index = FirstQValueNotGood.N;
	Lise.fQThresPlusCh1 = Lise.BufferResultat[0][FirstQValueNotGood.N].Qualite;
	// fin de modif MA
	if(Lise.NombredeVoie == 2)
	{ // Traitement des pics moins pour la voie 2, classement par ordre de qualité croissante
		for( i = 0; i<Lise.NbPicPlusProcessVoie2;i++)
		{ // Traitement pour la voie 1 pics plus
			IndiceTraitement = Lise.WriteResultSecondChannel;
			RBP_Sub(IndiceTraitement,NombreDepicsATraiterVoie2);
			RBP_Inc(IndiceTraitement);
			for(j = 1 ; j < Lise.NbPicPlusProcessVoie2 - i ; j++)
			{
				if(Lise.BufferResultat[1][IndiceTraitement.N].Qualite > Lise.BufferResultat[1][RBP_GetNMinusOne(IndiceTraitement)].Qualite)
				{
						PICRESULT PicEchange;
						
						PicEchange = Lise.BufferResultat[1][RBP_GetNMinusOne(IndiceTraitement)];
						Lise.BufferResultat[1][RBP_GetNMinusOne(IndiceTraitement)]= Lise.BufferResultat[1][IndiceTraitement.N];
						Lise.BufferResultat[1][IndiceTraitement.N]= PicEchange ;
				}
				RBP_Inc(IndiceTraitement);
			}
		}
	}

	RING_BUFFER_POS IndiceCopyTemp = Lise.WriteResult;
	RBP_Sub(IndiceCopyTemp,NombreDePicsATraiter);

	RING_BUFFER_POS IndiceCopyTempChannel2 = Lise.WriteResultSecondChannel;
	RBP_Sub(IndiceCopyTempChannel2,NombreDepicsATraiterVoie2);
	
	int CompteurPicsPlus = 0;
	int CompteurPicsPlusVoie2 = 0;

	for(i = 0; i<NB_PEAK_MAX; i++)
	{ // Copie des pics de la voie 1 pour le sens Positif
		if(Lise.BufferResultat[0][IndiceCopyTemp.N].Sens == true)
		{
			Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[i] = Lise.BufferResultat[0][IndiceCopyTemp.N];
			RBP_Inc(IndiceCopyTemp);
			CompteurPicsPlus++;
		}
		if(Lise.NombredeVoie == 2)
		{
			if(Lise.BufferResultat[1][IndiceCopyTempChannel2.N].Sens == true)
			{ // copie des pics de la voie 2 pour le sens positif
				Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie2[i] = Lise.BufferResultat[1][IndiceCopyTempChannel2.N];
				RBP_Inc(IndiceCopyTempChannel2);
				CompteurPicsPlusVoie2++;
			}
		}
	}

	// Pic dans le bruit
	Lise.UnusedPeaksIntensity = Lise.BufferResultat[0][IndiceCopyTemp.N].Intensite;
	/*if(Lise.NombredeVoie == 2)
		Lise.UnusedPeaksIntensity = Lise.BufferResultat[0][IndiceCopyTempChannel2.N].Intensite;*/
	

	for( i = 0; i<Lise.NbPicMoinsProcessVoie1;i++)
	{ // Traitement pour la voie 1 pics moins
		IndiceTraitement = WriteResult;
		RBP_Sub(IndiceTraitement,Lise.NbPicMoinsProcessVoie1);
		RBP_Inc(IndiceTraitement);
		for(j = 1 ; j < Lise.NbPicMoinsProcessVoie1 - i ; j++)
        {
			if(Lise.BufferResultat[0][IndiceTraitement.N].Qualite > Lise.BufferResultat[0][RBP_GetNMinusOne(IndiceTraitement)].Qualite)
			{
					PICRESULT PicEchange;
					
					PicEchange = Lise.BufferResultat[0][RBP_GetNMinusOne(IndiceTraitement)];
					Lise.BufferResultat[0][RBP_GetNMinusOne(IndiceTraitement)]= Lise.BufferResultat[0][IndiceTraitement.N];
					Lise.BufferResultat[0][IndiceTraitement.N]= PicEchange ;
			}
			RBP_Inc(IndiceTraitement);
		}
	}
	// modif MA 09/09/08
	FirstQValueNotGood = WriteResult;
	RBP_Sub(FirstQValueNotGood,Lise.NbPicMoinsProcessVoie1);
	RBP_Add(FirstQValueNotGood,8);
	index = FirstQValueNotGood.N;
	Lise.fQThresMoinsCh1 = Lise.BufferResultat[0][FirstQValueNotGood.N].Qualite;
	// fin de modif MA
	if(Lise.NombredeVoie == 2)
	{
		for( i = 0; i<Lise.NbPicMoinsProcessVoie2;i++)
		{ // Traitement pour la voie 2 pics moins
			IndiceTraitement = Lise.WriteResultSecondChannel;
			RBP_Sub(IndiceTraitement,Lise.NbPicMoinsProcessVoie2);
			RBP_Inc(IndiceTraitement);
			for(j = 1 ; j < Lise.NbPicMoinsProcessVoie2 - i ; j++)
			{
				if(Lise.BufferResultat[1][IndiceTraitement.N].Qualite > Lise.BufferResultat[1][RBP_GetNMinusOne(IndiceTraitement)].Qualite)
				{
						PICRESULT PicEchange;
						
						PicEchange = Lise.BufferResultat[1][RBP_GetNMinusOne(IndiceTraitement)];
						Lise.BufferResultat[1][RBP_GetNMinusOne(IndiceTraitement)] = Lise.BufferResultat[1][IndiceTraitement.N];
						Lise.BufferResultat[1][IndiceTraitement.N] = PicEchange ;
				}
				RBP_Inc(IndiceTraitement);
			}
		}
	}
	int CompteurPicsMoins = 0;
	int CompteurPicsMoinsVoie2 = 0;

	IndiceCopyTemp = Lise.WriteResult;
	RBP_Sub(IndiceCopyTemp,Lise.NbPicMoinsProcessVoie1);

	IndiceCopyTempChannel2 = Lise.WriteResultSecondChannel;
	RBP_Sub(IndiceCopyTempChannel2,Lise.NbPicMoinsProcessVoie2);

	for(i = 0; i<NB_PEAK_MAX; i++)
	{ // copie des pics de la voie 1 pour le sens négatif
		if(Lise.BufferResultat[0][IndiceCopyTemp.N].Sens == false)
		{
			Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[i] = Lise.BufferResultat[0][IndiceCopyTemp.N];
			RBP_Inc(IndiceCopyTemp);
			CompteurPicsMoins++;
		}
	}
	if(Lise.NombredeVoie == 2)
	{
		IndiceCopyTemp = Lise.WriteResultSecondChannel;
		RBP_Sub(IndiceCopyTemp,Lise.NbPicMoinsProcessVoie2);
		for(i = 0; i<NB_PEAK_MAX; i++)
		{ // copie des pics de la voie 2 pour le sens négatif
			if(Lise.BufferResultat[1][IndiceCopyTemp.N].Sens == false)
			{
				Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie2[i] = Lise.BufferResultat[1][IndiceCopyTempChannel2.N];
				RBP_Inc(IndiceCopyTempChannel2);
				CompteurPicsMoinsVoie2++;
			}
		}
	}

	if (bRefOptTheorique)
	{
		// Ajout du pic théorique, de qualité 1 et d'intensité 1 à la place du pic conservé de plus petite qualité.
		// Sens positif.
		Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[CompteurPicsPlus - 1].Position = 0.0;
		Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[CompteurPicsPlus - 1].XAbsN = 0;
		Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[CompteurPicsPlus - 1].Intensite = 1.0;
		Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[CompteurPicsPlus - 1].Sens = true;
		Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[CompteurPicsPlus - 1].Qualite = 1.0;
		Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[CompteurPicsPlus - 1].bSaturation = false;
		Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[CompteurPicsPlus - 1].XRel = fPositionRefOpt / (Lise.dRefWaveLengthNm / 4.0 / 1000.0);
		// Sens négatif.
		Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[CompteurPicsMoins - 1].Position = 0.0;
		Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[CompteurPicsMoins - 1].XAbsN = 0;
		Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[CompteurPicsMoins - 1].Intensite = 1.0;
		Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[CompteurPicsMoins - 1].Sens = true;
		Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[CompteurPicsMoins - 1].Qualite = 1.0;
		Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[CompteurPicsMoins - 1].bSaturation = false;
		Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[CompteurPicsMoins - 1].XRel = fPositionRefOpt / (Lise.dRefWaveLengthNm / 4.0 / 1000.0);
	}

	// Tri par XRel croissants, sens positif, voie 1
    for(i = 0 ;i<CompteurPicsPlus; i++)
    {
		for(j = 1 ; j < CompteurPicsPlus - i ; j++)
        {
			if(Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[j].XRel < Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[j-1].XRel/* && Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[j].XAbsN*/)
            {
				PICRESULT PicEchange;
				
				PicEchange = Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[j-1];
                Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[j-1]= Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[j];
                Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[j]= PicEchange ;
			}
        }
    }
	// Tri par XRel croissants, sens positif, voie 2
	if(Lise.NombredeVoie == 2)
	{
		for(i = 0 ;i<CompteurPicsPlusVoie2; i++)
		{
			for(j = 1 ; j < CompteurPicsPlusVoie2 - i ; j++)
			{
				if(Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie2[j].XRel < Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie2[j-1].XRel/* && Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie1[j].XAbsN*/)
				{
					PICRESULT PicEchange;
					
					PicEchange = Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie2[j-1];
					Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie2[j-1]= Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie2[j];
					Lise.Resultats[Lise.IndicePeriod.N].PicsPlusVoie2[j]= PicEchange ;
				}
			}
		}	
	}
	// Tri par XRel croissants, sens négatif, voie 1
    for(i = 0 ;i<CompteurPicsMoins; i++)
    {
		for(j = 1 ; j < CompteurPicsMoins - i ; j++)
        {
			if(Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[j].XRel < Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[j-1].XRel/* && Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[j].XAbsN*/)
            {
				PICRESULT PicEchange;
				
				PicEchange = Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[j-1];
                Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[j-1]= Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[j];
                Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[j]= PicEchange ;
			}
        }
    }
	// Tri par XRel croissants, sens négatif, voie 2
	if(Lise.NombredeVoie == 2)
	{
		for(i = 0 ;i<CompteurPicsMoinsVoie2; i++)
		{
			for(j = 1 ; j < CompteurPicsMoinsVoie2 - i ; j++)
			{
				if(Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie2[j].XRel < Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie2[j-1].XRel/* && Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie1[j].XAbsN*/)
				{
					PICRESULT PicEchange;
					
					PicEchange = Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie2[j-1];
					Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie2[j-1]= Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie2[j];
					Lise.Resultats[Lise.IndicePeriod.N].PicsMoinsVoie2[j]= PicEchange ;
				}
			}
		}
	}

	Lise.Resultats[Lise.IndicePeriod.N].NbPicsPlusVoie1= CompteurPicsPlus;
	Lise.Resultats[Lise.IndicePeriod.N].NbPicsMoinsVoie1 = CompteurPicsMoins;
	Lise.Resultats[Lise.IndicePeriod.N].NbPicsPlusVoie2 = CompteurPicsPlusVoie2;
	Lise.Resultats[Lise.IndicePeriod.N].NbPicsMoinsVoie2 = CompteurPicsMoinsVoie2;
	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tFunction Find N Best Peaks Success");
	}
	return STATUS_OK;
}

int BufferMoyenne(LISE& Lise,double* BufferTemp,RING_BUFFER_POS& WriteResultChannelProcess)
{
	if(Lise.iLissage <= 0)
	{
		if(Lise.bDebug == true)
		{
			if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tParam iLissage must be a positive and non-nul value");
		}
		return STATUS_FAIL;
	}

	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tEntering Function Buffer Average");
	}

	// Sauvegarde de la waveform avant moyenne
	RING_BUFFER_POS IndiceTraitement = Lise.Read;
	RING_BUFFER_POS IndiceMoyenne = Lise.Read;

	while(IndiceTraitement.AbsN < Lise.Write.AbsN - Lise.fitLen - (Lise.iLissage - 1))
	{
		int ComptePointPourMoyenne = 0;
		double Moyenne = 0.0;
		while(ComptePointPourMoyenne < Lise.iLissage)
		{
			Moyenne += BufferTemp[IndiceMoyenne.N];
			ComptePointPourMoyenne++;
			RBP_Inc(IndiceMoyenne);
			if(Lise.NombredeVoie == 2)
			{
				RBP_Inc(IndiceMoyenne);
			}
		}
		
		
		BufferTemp[IndiceTraitement.N] = Moyenne / (double)ComptePointPourMoyenne;
		RBP_Inc(IndiceTraitement);
		IndiceMoyenne = IndiceTraitement;
	}

	if(Lise.bDebug == true)
	{		
		if(Lise.bDebugProcess == true)	LogfileF(*Lise.Log,"[LISEED]\tBuffer Average Success");
	}

	return STATUS_OK;
}
