
#include <winsock2.h>
#include <windows.h>

#include "LISE_ED_DLL_External.h"

#include "..\SrcC\SPG.h"

#include <stdio.h>
#include <stdlib.h>
#include <windows.h>
#include <math.h>
#include <conio.h>

//FDE #include "NIDAQmx.h"

#define AFFICHAGE

// Point d'entré du programme principal
int WINAPI WinMain( HINSTANCE hInstance, 
				   // handle to current instance 
				   
				   HINSTANCE hPrevInstance, 
				   // handle to previous instance 
				   
				   LPSTR lpCmdLine, 
				   // pointer to command line 
				   
				   int nCmdShow 
				   // show state of window 
				   
				   )
{
	SPG_WinMainStart((int)hInstance,0,0,0,SPG_SM_UserFriendly,G_ECRAN_DIBSECT,"LISEAPP","LISEAPP",0);

	char strV[128];
// on recupere le numero de version du programme et on l'affiche dans une fenetre
	int V = LiseEDGetVersion();
	sprintf(strV,"DLL LISE_LS_DLL chargée, version %i",V);
	if(Global.EnableList>=1) MessageBox(0,fdwstring(strV),L"LISE APP",MB_OK);

	
	LiseEDInit("Configuration_Systeme.txt");
	LiseEDSavePeaks("SauvegardePic.txt");

	int CompteurPanne = 0;
	int NbIterationAvantPanne = 100;
	int Panne = 0;

	int NbThickness = 12;
	double* Thickness = (double*) malloc(NbThickness*sizeof(double));
	double* Quality = (double*) malloc (NbThickness*sizeof(double));

	int NbSamples = 100000;
	double* I = (double*) malloc (NbSamples * sizeof(double));	
	double* IVoie2 = (double*) malloc (NbSamples * sizeof(double));

	int NbPeaks = 12;
	__int64* XPosAbs = (__int64*) malloc (NbPeaks*sizeof(__int64));
	double* XPosRel = (double*) malloc (NbPeaks*sizeof(double));
	double* Intensite= (double*) malloc (NbPeaks*sizeof(double));//	double ValeurSource = 1.0;
	double* Qualite= (double*) malloc (NbPeaks*sizeof(double));//	double ValeurSource = 1.0;
	int* Sens = (int*) malloc (NbPeaks*sizeof(int));

	float StepX;

	char* Name = "Sample";
	char* SampleNumber = "SampleNumber";

	// Définition de l'échantillon
	int NbEpaisseurs = 2;
	double* Thicknesss = (double*) malloc (NbEpaisseurs * sizeof(double));
	double* Tolerance = (double*) malloc (NbEpaisseurs * sizeof(double));
	double* Index = (double*) malloc (NbEpaisseurs * sizeof(double));
	double* Intensity = (double*) malloc (NbEpaisseurs * sizeof(double));
	Index[0] = 1.0;
	Index[1] = 3.68;
	Thicknesss[0] = 3000 * Index[0];	// Epaisseur en µm
	Thicknesss[1] = 656.0;
	Tolerance[0] = 3000.0 * Index[0];	// Tolérance en µm
	Tolerance[1] = 10.0;
	Intensity[0] = 0.0;
	Intensity[1] = 0.0;
	double Gain = 1.2;
	double QualityThreshold = 1;

	LiseEDDefineSample(Name,SampleNumber,Thicknesss,Tolerance,Index,Intensity,&NbEpaisseurs/*,&Gain,&QualityThreshold*/);
	// Fin définition de l'échantillon

	LiseEDAcqSaveThickness("Mes_epaisseurs.txt");
	
	char cType[128];
	char cSerialNumber[128];
	double dRange;
	int iFrequency;
	double dGainMin;
	double dGainMax;
	double dGainStep;

	LiseEDGetSystemCaps(cType,cSerialNumber,&dRange,&iFrequency,&dGainMin,&dGainMax,&dGainStep);
//	LiseEDSetStagePositionInfo();
	int Voie = 0;
	bool escape = false;
	double ValeurSource = 1.2; // 1.15 dernières valeurs 1.25 mesure ou 1.9 saturation
	LiseEDSetSourcePower(&ValeurSource,&Voie);
	

	double ThicknessResult[8];
	double dQualityValue;

	LiseEDAcqStart();
	LiseEDAcqStop();
	double PuissanceRecouplee;
	//LiseEDDisplaySettingWindow();

	while (!escape)
	{	//VK_ESCAPE
		escape = GetAsyncKeyState(VK_ESCAPE) ? true : false;	

		Sleep(100);

		LiseEDGetNbSamplesWaveform(&NbSamples);
		LiseEDGetNbPeaksPeriod(&NbPeaks,&Voie);
		
 		if (NbSamples > 0)
		{
   			int RetValue = LiseEDGetWaveform(I,&NbSamples,&StepX,1);
   		//	LiseEDGetWaveform(IVoie2,&NbSamples,&StepX,2);
			LiseEDGetNbPeaksPeriod(&NbPeaks,&Voie);
			

		//  Fonctions d'affichage
			if(RetValue == 1)
			{
				C_Lib CL;
				C_LoadCaracLib(CL,Global.Ecran,"..\\..\\SrcC\\carac","Cnew8.bmp");
				
				Cut C;
				Cut C2;
				Cut_Create(C,NbSamples);
			//	Cut_Create(C2,NbSamples);
				for(int i=0;i<NbSamples;i++)
				{
					C.D[i] = I[i];
	//				C2.D[i] = IVoie2[i];
				}	

				// Affichage d'une seule voie
				Cut_DrawScaled(C,Global.Ecran,0,CL, -8.0f, 8.0f);
				// Affichage des deux voies sur le meme écran
			//	Cut_Draw2MultiScale(C,C2,Global.Ecran,0,100,CL,-8.0f,8.0f);
			//	SPG_BlitAndWaitForClick();
				DoEvents(SPG_DOEV_ALL);
				Cut_Close(C);
				C_CloseCaracLib(CL);
			}
		}
		//LiseEDReadPower(&PuissanceRecouplee);
		if (GetAsyncKeyState(VK_UP))
		{
			ValeurSource += 0.01;
		}
		else if (GetAsyncKeyState(VK_DOWN))
		{
			ValeurSource -= 0.01;
		}
		else if (GetAsyncKeyState(VK_PRIOR))
		{
			ValeurSource += 0.1;
		}
		else if (GetAsyncKeyState(VK_NEXT))
		{
			ValeurSource -= 0.1;
		}
		else if(GetAsyncKeyState('S'))
		{
			LiseEDSaveWaveForm("Mon_Signal_Last_Periode.txt",&StepX);
		}
		else if(GetAsyncKeyState('G'))
		{
			LiseEDGetPeaksPeriod(XPosRel,Intensite,Qualite,Sens,NULL,&NbPeaks,&Voie);
			LiseEDGetThickness(Thickness,&NbThickness,Quality, NbThickness);
		}
	/*	
		CompteurPanne++;
		if(CompteurPanne == NbIterationAvantPanne)
		{
			if
		}*/

		LiseEDSetSourcePower(&ValeurSource,&Voie);
/*		Gain = 0.0;
		ValeurSource = 0.0;
		LiseEDDefineSample(Name,SampleNumber,Thicknesss,Tolerance,Index,Intensity,&NbEpaisseurs,&Gain,&QualityThreshold);
*/
	}
	LiseEDAcqStop();
//	LiseEDDestroySettingWindow();
//	double Power;
//	LiseEDReadPower(&Power);

	free(I);
	free(XPosAbs);
	free(XPosRel);
	free(Intensite);
	free(Sens);
	free(Thickness);
	free(Quality);

	LiseEDClose();
 	return 0;
}
