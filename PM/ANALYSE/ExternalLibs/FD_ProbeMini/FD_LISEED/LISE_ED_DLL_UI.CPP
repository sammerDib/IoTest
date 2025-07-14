/*
 * $Id: LISE_ED_DLL_UI.CPP 8257 2009-02-16 17:52:50Z S-PETITGRAND $
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

#include "../FD_LISELS/LISE_LSLI_DLL_Internal.h"
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

//#include <stdio.h>
//#include <stdlib.h>
#include <time.h>

// "LISE_ED_DLL_UI_Struct.h" (faire clic droit - open document)

//#include "LISE_ED_DLL_UI_Struct.h"

//Fonctions appalées de l'exterieur

#define UIENTER(fct) CHECK(s==0,#fct,return STATUS_FAIL);LISE_ED &LiseEdSet=*(LISE_ED*)s; LISE_ED_SETTINGS& LSt=LiseEdSet.LSettings;


int LEDIOpenSettingsWindow(void* s,int ShowGainSetting)
{
	UIENTER(LEDIOpenSettingsWindow);
	LISE_ED* LiseEd= (LISE_ED*)s;
	CHECK(LSt.Status!=eduiUninitialized,"LEDIOpenSettingsWindow",return STATUS_FAIL);

	SPG_ZeroStruct(LSt);

	if(ShowGainSetting == 1) //&&(GetAsyncKeyState(VK_SHIFT)==0))
	{
//		SPG_CreateWindow(LSt.SW,SPGWT_UserFriendly,640,400,0,"Press Escape to close","FPED",0,0);
		// 20080227 Doubler la taille de la fenêtre pour sigmatech
		SPG_CreateWindow(LSt.SW,SPGWT_UserFriendly,G_STANDARDW,1,-1,LiseEd->Lise.SettingWindowWidth,LiseEd->Lise.SettingWindowHeight,1,"Press Escape to close","FPED",0,0);
	}
	else
	{
//		SPG_CreateWindow(LSt.SW,SPGWT_UserFriendly,640,400,0,"","FPED",0,0);
		// 20080227 Doubler la taille de la fenêtre pour sigmatech
		SPG_CreateWindow(LSt.SW,SPGWT_UserFriendly,G_STANDARDW,1,-1,LiseEd->Lise.SettingWindowWidth,LiseEd->Lise.SettingWindowHeight,1,"","FPED",0,0);
	}
	SetWindowPos((HWND)LSt.SW.hWndWin,HWND_TOPMOST,0,0,0,0,SWP_NOACTIVATE|SWP_NOMOVE|SWP_NOREDRAW|SWP_NOREPOSITION|SWP_NOSIZE);

	G_DrawRect(LSt.SW.Ecran,0,0,G_SizeX(LSt.SW.Ecran),G_SizeY(LSt.SW.Ecran),0xffffff);

	G_BlitEcran(LSt.SW.Ecran);

#define CHEIGHT 2
#define BWIDTH 80
#define CHECK_BUTTON_SIZE 10
	G_InitSousEcran(LSt.EButtons,LSt.SW.Ecran,	0,0,	BWIDTH,G_SizeY(LSt.SW.Ecran) - 50);
	G_InitSousEcran(LSt.ECheckButton,LSt.SW.Ecran,	0,G_SizeY(LSt.SW.Ecran) - 50, BWIDTH,50);
	// Initialisation dans le sous écran pour les écrans
	if(LiseEd->Lise.NombredeVoie == 2)
	{
		G_InitSousEcran(LSt.EGraph,LSt.SW.Ecran,	BWIDTH,0,	G_SizeX(LSt.SW.Ecran)-BWIDTH,(G_SizeY(LSt.SW.Ecran)-CHEIGHT)/2 - 1);
		G_InitSousEcran(LSt.EGraphChannel2,LSt.SW.Ecran,	BWIDTH,(G_SizeY(LSt.SW.Ecran)-CHEIGHT)/2,	G_SizeX(LSt.SW.Ecran)-BWIDTH,(G_SizeY(LSt.SW.Ecran)-CHEIGHT)/2);
	}
	else
	{
		G_InitSousEcran(LSt.EGraph,LSt.SW.Ecran,	BWIDTH,0,	G_SizeX(LSt.SW.Ecran)-BWIDTH,G_SizeY(LSt.SW.Ecran)-CHEIGHT);
	}
	// Fin d'init
	//G_InitSousEcran(LSt.EGraph,LSt.SW.Ecran,	BWIDTH,0,	G_SizeX(LSt.SW.Ecran)-BWIDTH,G_SizeY(LSt.SW.Ecran)-CHEIGHT);
	G_InitSousEcran(LSt.EConsole,LSt.SW.Ecran,	BWIDTH,G_SizeY(LSt.SW.Ecran)-CHEIGHT,	G_SizeX(LSt.SW.Ecran)-BWIDTH,CHEIGHT);

	// Bouttons, on charge les bouttons
	B_LoadButtonsLib(LSt.BL,LSt.EButtons,0,0,0,"Buttons.bmp");
//	B_LoadButtonsLib(LSt.BLCheckButton,LSt.EButtons,0,0,0,"Buttons.bmp");

	C_LoadCaracLib(LSt.CL,LSt.SW.Ecran,0,"CNew8.bmp");
	if(ShowGainSetting == 1) //&&(GetAsyncKeyState(VK_SHIFT)==0))
	{
//		LISE_ED &Lise=*(LISE_ED*)&s.Probe[ProbeID].FProbeState;
//		LISE_ED_SETTINGS& LSt=Lise.LSettings;
		B_CreateVReglageNumeric(LSt.BL,LSt.EButtons,LSt.CL,30,20,G_SizeY(LSt.EButtons)-40,4,&LSt.fVolts,0.05,0,10);
		B_RedrawButtonsLib(LSt.BL,0);
	}
	// Creation d'un CheckButton pour choisir la voie en X = 1 et Y = 1
	B_CreateCheckButton(LSt.BLCheckButton,LSt.ECheckButton,1,1);
	// Fin de creation du bouton
	FileConsole_Create(LSt.Console,&LSt.EConsole,&LSt.CL,0,Global.ProgDir,"FogaleProbe_LiseED.txt");
	Console_Add(LSt.Console,"LEDIOpenSettingsWindow");
	Console_Update(LSt.Console);

	S_InitTimer(LSt.D,"DelayU");

	LSt.WavLen=65536;
	LSt.Wav=SPG_TypeAlloc(LSt.WavLen,double,"LSt.Wav");

	Cut_Create(LSt.CWav,LSt.WavLen);
	Cut_Create(LSt.CSaturationThreshold,LSt.WavLen);

	if(LiseEd->Lise.NombredeVoie == 2)
	{
		LSt.WavChannel2=SPG_TypeAlloc(LSt.WavLen,double,"LSt.WavChannel2");
		Cut_Create(LSt.CWavChannel2,LSt.WavLen);
	}
	LSt.pfVolts=LSt.fVolts=LiseEd->Lise.fSourceValue;

#ifdef SETTING_WINDOWS_SCROLLBAR
	B_CreateVReglageNumeric(LSt.BL,LSt.EButtons,LSt.CL,30,20,G_SizeY(LSt.EButtons)-40,4,&LSt.fVolts,0.05,0,10);
#endif

	B_RedrawButtonsLib(LSt.BL,0);
//	B_RedrawButtonsLib(LSt.BLCheckButton,0);

	G_BlitEcran(LSt.SW.Ecran);

	LEDIAcqStart(s);

	//while(GetAsyncKeyState(VK_CONTROL));

	S_StartTimer(LSt.D);
	LSt.Status=eduiOK;
	return STATUS_OK;
}

int LEDIUpdateSettingsWindow(void* s)
{
//	UIENTER(LEDIUpdateSettingsWindow);
	LISE_ED* LiseEd = (LISE_ED*)s;
	LISE_ED_SETTINGS& LSt = LiseEd->LSettings;
	
	if(LSt.Status)
	{
		
		CHECK(LSt.Status==eduiRunning,"LEDIUpdateSettingsWindow - Reentrance",LSt.FrameNr++;return STATUS_FAIL);
		CHECK(LSt.Status!=eduiOK,"LEDIUpdateSettingsWindow",LSt.FrameNr++;return STATUS_FAIL);
		LSt.Status=eduiRunning;
		double T;
		S_GetTimerRunningTime(LSt.D,T);
		if(T>0.05) 
		{
			S_StopTimer(LSt.D);
			S_ResetTimer(LSt.D);
//fdef SETTING_WINDOWS_SCROLLBAR
			// BLCheckButton
			int EtatButton = 0;

			B_UpdateButtonsLib(LSt.BL,LSt.SW.MouseX,LSt.SW.MouseY,LSt.SW.MouseLeft);
//ndif
			{
				int N=LSt.WavLen;
				int N2 = LSt.WavLen;
				float StepX=0;
				int returnValue = -10;
				/* Affichage double si on a deux voies de mesure */
				returnValue = LEDIGetWaveform(s,LSt.Wav,&N,&StepX,1,NULL,NULL,NULL,NULL);
				if(LiseEd->Lise.NombredeVoie == 2)
				{
					returnValue = LEDIGetWaveform(s,LSt.WavChannel2,&N2,&StepX,2,NULL,NULL,NULL,NULL);
				}
				//int Navant = N;
				N*=0.48;//n'affiche que le scan aller
				if(N>0)
				{
					for(int i=0;i<N;i++)
					{
						LSt.CWav.D[i] = LSt.Wav[i];
						LSt.CSaturationThreshold.D[i] = LiseEd->Lise.fThresholdSaturation;
						if(LiseEd->Lise.NombredeVoie == 2)
						{
							LSt.CWavChannel2.D[i] = LSt.WavChannel2[i];
						}
					}
					LSt.CWav.NumS=N;
					LSt.CWavChannel2.NumS = N;
					LSt.CSaturationThreshold.NumS = N;
					if(LiseEd->Lise.NombredeVoie == 2)
					{
						Cut_Draw2MultiScale(LSt.CWav,LSt.CSaturationThreshold,LSt.EGraph,0,1000,LSt.CL,-10.0f,9.0f);
						//Cut_DrawScaled(LSt.CWav,LSt.EGraph,0,LSt.CL,-5,9);
						Cut_DrawScaled(LSt.CWavChannel2,LSt.EGraphChannel2,100,LSt.CL,-10.0f,9.0f);
					}
					else
					{
						//Cut_DrawScaled(LSt.CWav,LSt.EGraph,0,LSt.CL,-5,9);
						// 20080225 Affichage de la waveform et du seuil de saturation
						Cut_Draw2MultiScale(LSt.CWav,LSt.CSaturationThreshold,LSt.EGraph,0,1000,LSt.CL,-10.0f,9.0f);
					}
					
					LSt.CWav.NumS=LSt.WavLen;
				}
			}


			if(LSt.fVolts!=LSt.pfVolts)
			{
				//LSt.fVolts = LISE->fSourceValue; 
				LEDISetSourcePower(s,LSt.fVolts);
				LiseEd->Lise.fSourceValue = LSt.fVolts;
				LSt.pfVolts=LSt.fVolts;
			}
		/*
			char Msg[512];
			sprintf(Msg,"LEDIUpdateSettingsWindow - %i",LSt.FrameNr);
			Console_Add(LSt.Console,Msg);
			Console_Update(LSt.Console);
		*/
			G_BlitEcran(LSt.SW.Ecran);
			S_StartTimer(LSt.D);
		}
		LSt.FrameNr++;
		LSt.Status=eduiOK;
	}
	return STATUS_OK;
}

int LEDICloseSettingsWindow(void* s)
{
	UIENTER(LEDICloseSettingsWindow);
	LISE_ED* LiseEd = (LISE_ED*)s;

	LEDIAcqStop(s);

	if(LSt.Status)
	{
		CHECK(LSt.Status==eduiRunning,"LEDICloseSettingsWindow - Reentrance",return STATUS_FAIL);
		CHECK(LSt.Status!=eduiOK,"LEDICloseSettingsWindow",return STATUS_FAIL);
		LSt.Status=eduiRunning;

//fdef SETTING_WINDOWS_SCROLLBAR
//		Global.Ecran=LSt.SW.Ecran;
//ndif
		Console_Add(LSt.Console,"LEDICloseSettingsWindow");
		Console_Update(LSt.Console);
		G_BlitEcran(LSt.SW.Ecran);

		S_CloseTimer(LSt.D);

		SPG_MemFree(LSt.Wav);
		Cut_Close(LSt.CWav);
		Cut_Close(LSt.CSaturationThreshold);
		if(LiseEd->Lise.NombredeVoie == 2)
		{
			SPG_MemFree(LSt.WavChannel2);
			Cut_Close(LSt.CWavChannel2);
			G_CloseEcran(LSt.EGraphChannel2);
		}
		//SPG_ZeroStruct(Global.Ecran);

		G_CloseEcran(LSt.EButtons);
		G_CloseEcran(LSt.EGraph);
		G_CloseEcran(LSt.EConsole);

		C_CloseCaracLib(LSt.CL);
		B_CloseButtonsLib(LSt.BL);
	//	B_CloseButtonsLib(LSt.BLCheckButton);
		//BLCheckButton

		SPG_CloseWindow(LSt.SW);
		LSt.Status=eduiUninitialized;
	}
	return STATUS_OK;
}


//Fonctions appelées par le système lui même pour émuler les appels par le programme principal s'il ne le fait pas
/*
void LEDUIUpdateOnInit(LISE_ED& LISE)
{
	//LEDIOpenSettingsWindow(&LISE);
	return;
}

void LEDUIUpdateOnClose(LISE_ED& LISE)
{
	//LEDICloseSettingsWindow(&LISE);
	return;
}

void LEDUIUpdateOnStart(LISE_ED& LISE)
{
	//LEDIOpenSettingsWindow(&LISE);
	return;
}

void LEDUIUpdateOnStop(LISE_ED& LISE)
{
	//LEDICloseSettingsWindow(&LISE);
	return;
}

void LEDUIUpdateOnAcqAndProcess(LISE_ED& LISE)
{
	//LEDIUpdateSettingsWindow(&LISE);
	return;
}
*/
