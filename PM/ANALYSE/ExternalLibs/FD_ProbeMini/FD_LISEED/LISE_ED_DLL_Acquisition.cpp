/*
 * $Id: LISE_ED_DLL_Acquisition.cpp 8372 2009-02-26 10:18:16Z y-randle $
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


double* SignalEmule;

void CreateSignalEmule(LISE& Lise)
{
	if(Lise.ReadFileForEmulation == 1)
	{
		// On definit une taille maximum pour le buffer 
		Lise.MaxSignalEmule = 100000;
	}
	else
	{
		if(Lise.NombredeVoie == 2)
		{
			Lise.MaxSignalEmule = 72000;
		}
		else
		{
			Lise.MaxSignalEmule = 36000;
		}
	}
	SignalEmule = (double*) malloc(Lise.MaxSignalEmule * sizeof(double));
}

int FillSignalEmule(LISE_ED& LiseEd)
{
	CreateSignalEmule(LiseEd.Lise);
	int NoiseMsk = 0;

	if(LiseEd.Lise.ReadFileForEmulation == 1)
	{
		ReadSignalEmule(LiseEd,LiseEd.Lise.FichierReadSignal);
	}
	else
	{
#ifndef REFOPTIQUE
// Definition d'une période de signal non calée sur un Pulse
    for(int i =0;i<6000;i++)        SignalEmule[i] = -4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
// Pic Numero 1 
    for(i = 6000;i<6100;i++)        SignalEmule[i] = 0 + (double) ( 3.00 * (0.5-0.5*cos(6.28 * ((double)i-6000)/(6100-6000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
    for(i = 6100;i<8975;i++)        SignalEmule[i] = - 4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
// Pulse Plus    
    for(i = 8975;i<9025 ;i++)        SignalEmule[i] = 9.54 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
    for(i = 9025;i<12000 ;i++)        SignalEmule[i] = - 4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
// Pic Numero Deux
    for(i = 12000;i<12100;i++)        SignalEmule[i] = 0 + (double) ( 3.00 * (0.5-0.5*cos(6.28 * ((double)i-12000)/(12100-12000)))) + (rand()&&NoiseMsk)-NoiseMsk/2; 
    for(i = 12100;i<20000;i++)        SignalEmule[i] = -4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
// Pic Numero trois
    for(i = 20000;i<20100;i++)        SignalEmule[i] = 0 + (double) ( 3.00 * (0.5-0.5*cos(6.28 * ((double)i-20000)/(20100-20000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
    for(i = 20100;i<26975;i++)        SignalEmule[i] = -4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
// Pulse Moins 
    for(i = 26975;i<27025;i++)        SignalEmule[i] = -9.58 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
    for(i =27025;i<34000;i++)        SignalEmule[i] = -4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10; 
// Pic Numero quatre
    for(i = 34000;i<34100;i++)        SignalEmule[i] = 0 + (double) ( 3.00 * (0.5-0.5*cos(6.28 * ((double)i-34000)/(34100-34000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
    for(i = 34100;i<36000;i++)        SignalEmule[i] = - 4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
#else
	if(LiseEd.Lise.NombredeVoie == 2)
	{ // Voie entrelacée
		int test;
	// Definition d'une période de signal non calée sur un Pulse
		int i =0;
		for(i =0;i<12000;i=i+2)
		{
			SignalEmule[i] = -4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
			SignalEmule[i+1] = -4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
		}
	// Pic Numero 1
		test = LiseEd.iPicSatureDansSimulation & 0x02;
		if(test == 2)
		{
			for(i = 12000;i<12200;i=i+2)
			{
				SignalEmule[i] = -4.94 + (double) ( 120 * 3.00 * (0.5-0.5*cos(6.28 * ((double)i-12000)/(12200-12000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
				if(SignalEmule[i] > 8) SignalEmule[i] = 8.0;
				SignalEmule[i+1] = -4.94 + (double) (1.2 * 3.00 * (0.5-0.5*cos(6.28 * ((double)i-12000)/(12200-12000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
			}
		}
		else
		{
			for(i = 12000;i<12200;i=i+2)
			{
				SignalEmule[i] = -4.94 + (double) ( 3.00 * (0.5-0.5*cos(6.28 * ((double)i-12000)/(12200-12000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
				SignalEmule[i+1] = -4.94 + (double) (3.00/100 * (0.5-0.5*cos(6.28 * ((double)i-12000)/(12200-12000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
			}
		}
		for(i = 12200;i<17000;i=i+2)
		{
			SignalEmule[i] = - 4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
			SignalEmule[i+1] = - 4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
		}
	// Reference optique sens negatif
		test = LiseEd.iPicSatureDansSimulation & 0x01;
		if(test == 1)
		{
			for(i = 17000;i<17200;i=i+2)
			{
				SignalEmule[i] = -4.94 + (double) ( 120 * 3.00 * (0.5-0.5*cos(6.28 * ((double)i-17000)/(17200-17000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
				if(SignalEmule[i] > 8) SignalEmule[i] = 8.0;
				SignalEmule[i+1] = -4.94 + (double) (1.2 * 3.00 * (0.5-0.5*cos(6.28 * ((double)i-17000)/(17200-17000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
			}
		}
		else
		{
			for(i = 17000;i<17200;i=i+2)
			{
				SignalEmule[i] = -4.94 + (double) ( 3.00 * (0.5-0.5*cos(6.28 * ((double)i-17000)/(17200-17000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
				SignalEmule[i+1] = -4.94 + (double) ( 3.00 / 100 * (0.5-0.5*cos(6.28 * ((double)i-17000)/(17200-17000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
			}
		}
		for(i = 17200;i<17950;i=i+2)
		{
			SignalEmule[i] = - 4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
			SignalEmule[i+1] = - 4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
		}
	// Pulse Plus    
		for(i = 17950;i<18050 ;i=i+2)
		{ // Les pulses ne sont pas présents sur la voie 2
			SignalEmule[i] = 9.54 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
			SignalEmule[i+1] = -4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
		}
		for(i = 18050;i<18800 ;i=i+2)
		{
			SignalEmule[i] = - 4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
			SignalEmule[i+1] = - 4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
		}
	// Reference optique sens positif
		test = LiseEd.iPicSatureDansSimulation & 0x01;
		if(test == 1)
		{
			for(i = 18800;i<19000;i=i+2)
			{
				SignalEmule[i] = -4.94 + (double) ( 120 * 3.00 * (0.5-0.5*cos(6.28 * ((double)i-18800)/(19000-18800)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
				if(SignalEmule[i] > 8) SignalEmule[i] = 8.0;
				SignalEmule[i+1] = -4.94 + (double) (1.2 * 3.00 * (0.5-0.5*cos(6.28 * ((double)i-18800)/(19000-18800)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
			}
		}
		else
		{
			for(i = 18800;i<19000;i=i+2)
			{
				SignalEmule[i] = -4.94 + (double) ( 3.00 * (0.5-0.5*cos(6.28 * ((double)i-18800)/(19000-18800)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
				SignalEmule[i+1] = -4.94 + (double) ( 3.00 / 100 * (0.5-0.5*cos(6.28 * ((double)i-18800)/(19000-18800)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
			}
		}
		for(i = 19000;i<24000;i=i+2)
		{
			SignalEmule[i] = - 4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
			SignalEmule[i+1] = - 4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
		}
	// Pic Numero Deux
		test = LiseEd.iPicSatureDansSimulation & 0x02;
		if(test == 2)
		{
			for(i = 24000;i<24200;i=i+2)
			{
				SignalEmule[i] = -4.94 + (double) ( 120 * 3.00 * (0.5-0.5*cos(6.28 * ((double)i-24000)/(24200-24000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
				if(SignalEmule[i] > 8) SignalEmule[i] = 8;
				SignalEmule[i+1] = -4.94 + (double) ( 1.2 * 3.00 * (0.5-0.5*cos(6.28 * ((double)i-24000)/(24200-24000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
			}
		}
		else
		{
			for(i = 24000;i<24200;i=i+2)
			{
				SignalEmule[i] = -4.94 + (double) ( 3.00 * (0.5-0.5*cos(6.28 * ((double)i-24000)/(24200-24000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
				SignalEmule[i+1] = -4.94 + (double) ( 3.00/100 * (0.5-0.5*cos(6.28 * ((double)i-24000)/(24200-24000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
			}
		}
		for(i = 24200;i<40000;i=i+2)
		{
			SignalEmule[i] = -4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
			SignalEmule[i+1] = -4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
		}
	// Pic Numero trois
		test = LiseEd.iPicSatureDansSimulation & 0x04;
		if(test == 4)
		{
			for(i = 40000;i<40200;i=i+2)
			{
				SignalEmule[i] = -4.94 + (double) ( 120 * 3.00 * (0.5-0.5*cos(6.28 * ((double)i-40000)/(40200-40000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
				if(SignalEmule[i] > 8) SignalEmule[i] = 8;
				SignalEmule[i+1] = -4.94 + (double) ( 1.2 * 3.00 * (0.5-0.5*cos(6.28 * ((double)i-40000)/(40200-40000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
			}
		}
		else
		{
			for(i = 40000;i<40200;i=i+2)
			{
				SignalEmule[i] = -4.94 + (double) ( 3.00 * (0.5-0.5*cos(6.28 * ((double)i-40000)/(40200-40000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
				SignalEmule[i+1] = -4.94 + (double) ( 3.00 / 100 * (0.5-0.5*cos(6.28 * ((double)i-40000)/(40200-40000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
			}
		}
		for(i = 40200;i<53950;i=i+2)
		{
			SignalEmule[i] = -4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
			SignalEmule[i+1] = -4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
		}
	// Pulse Moins 
		for(i = 53950;i<54050;i=i+2)
		{ // Les pulses ne sont pas présents sur la voie 2
			SignalEmule[i] = -9.58 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
			SignalEmule[i+1] = -4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
		}
		for(i =54050;i<68000;i=i+2)
		{
			SignalEmule[i] = -4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
			SignalEmule[i+1] = -4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
		}
	// Pic Numero quatre
		test = LiseEd.iPicSatureDansSimulation & 0x04;
		if(test == 4)
		{
			for(i = 68000;i<68200;i=i+2)
			{
				SignalEmule[i] = -4.94 + (double) ( 120 * 3.00 * (0.5-0.5*cos(6.28 * ((double)i-68000)/(68200-68000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
				if(SignalEmule[i] > 8) SignalEmule[i] = 8;
				SignalEmule[i+1] = -4.94 + (double) ( 1.2 * 3.00 * (0.5-0.5*cos(6.28 * ((double)i-68000)/(68200-68000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
			}
		}
		else
		{
			for(i = 68000;i<68200;i=i+2)
			{
				SignalEmule[i] = -4.94 + (double) ( 3.00 * (0.5-0.5*cos(6.28 * ((double)i-68000)/(68200-68000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
				SignalEmule[i+1] = -4.94 + (double) ( 3.00 / 100 * (0.5-0.5*cos(6.28 * ((double)i-68000)/(68200-68000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
			}
		}
		for(i = 68200;i<72000;i=i+2)
		{
			SignalEmule[i] = - 4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
			SignalEmule[i+1] = - 4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
		}
	}
	else
	{ // Cas ou l'on a qu'une voie et tous les autres cas
	// Definition d'une période de signal non calée sur un Pulse
		int i = 0;
		for(i =0;i<6000;i++)        SignalEmule[i] = -4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
	// Pic Numero 1 
		for(i = 6000;i<6100;i++)        SignalEmule[i] = -4.94 + (double) ( 6.00 * (0.5-0.5*cos(6.28 * ((double)i-6000)/(6100-6000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
		for(i = 6100;i<8500;i++)        SignalEmule[i] = - 4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
	// Reference optique sens negatif
		for(i = 8500;i<8600;i++)		SignalEmule[i] = -4.94 + (double) ( 6.00 * (0.5-0.5*cos(6.28 * ((double)i-6000)/(6100-6000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
		for(i = 8600;i<8975;i++)		SignalEmule[i] = - 4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
	// Pulse Plus    
		for(i = 8975;i<9025 ;i++)        SignalEmule[i] = 9.04 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
		for(i = 9025;i<9400 ;i++)        SignalEmule[i] = - 4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
	// Reference optique sens positif
		for(i = 9400;i<9500;i++)		SignalEmule[i] = -4.94 + (double) ( 6.00 * (0.5-0.5*cos(6.28 * ((double)i-6000)/(6100-6000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
		for(i = 9500;i<12000;i++)		SignalEmule[i] = - 4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
	// Pic Numero Deux
		for(i = 12000;i<12100;i++)        SignalEmule[i] = -4.94 + (double) ( 6.00 * (0.5-0.5*cos(6.28 * ((double)i-12000)/(12100-12000)))) + (rand()&&NoiseMsk)-NoiseMsk/2; 
		for(i = 12100;i<20000;i++)        SignalEmule[i] = -4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
	// Pic Numero trois
		for(i = 20000;i<20100;i++)        SignalEmule[i] = -4.94 + (double) ( 6.00 * (0.5-0.5*cos(6.28 * ((double)i-20000)/(20100-20000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
		for(i = 20100;i<26975;i++)        SignalEmule[i] = -4.94 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
	// Pulse Moins 
		for(i = 26975;i<27025;i++)        SignalEmule[i] = -9.08 + (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
		for(i =27025;i<34000;i++)        SignalEmule[i] = -4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10; 
	// Pic Numero quatre
		for(i = 34000;i<34100;i++)        SignalEmule[i] = -4.94 + (double) ( 6.00 * (0.5-0.5*cos(6.28 * ((double)i-34000)/(34100-34000)))) + (rand()&&NoiseMsk)-NoiseMsk/2;
		for(i = 34100;i<36000;i++)        SignalEmule[i] = - 4.94+ (double)((rand()&&NoiseMsk)-NoiseMsk/2)/10;
	} // Cas de la deuxième voie

#endif
	}
	return STATUS_OK;
}

int ReadSignalEmule(LISE_ED& LiseEd,char* FileName)
{
	float x = 0.0;
	char ma_chaine[128];
	int i = 0;
	bool bFichierPresent = true;
	FILE* fichier = fopen(FileName,"rb");
	int CompteurFinFichier = 0;
	// cas de la lecture d'un fichier avec des nombres avec .
	if (fichier != (FILE*) NULL)
	{
		while(!feof(fichier))
		{
			fscanf(fichier,"%s",&ma_chaine);
			if(ma_chaine[0] == '0' || ma_chaine[0] == '1' || ma_chaine[0] == '2' || ma_chaine[0] == '3' || ma_chaine[0] == '4' || ma_chaine[0] == '5' || ma_chaine[0] == '6' || ma_chaine[0] == '7' || ma_chaine[0] == '8' || ma_chaine[0] == '9' || '-')
			{
				
				for(int k = 0;k<sizeof(ma_chaine);k++)
				{
					if(ma_chaine[k] == ',')	
					{
						ma_chaine[k] = '.';
					}
				}
				x = atof(ma_chaine);
				if(i>1)
				{
					if(SignalEmule[i-1] == x)
					{
						CompteurFinFichier = 1;
					}
					else
					{
						CompteurFinFichier = 0;
					}
				}
				SignalEmule[i] = (double)x;
				i++;
			}
			int a = 0;
		}
	}
	else
	{ // pas de fichier
		bFichierPresent = false;
		LiseEd.Lise.ReadFileForEmulation = 0;
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\tFile For Emulation Is Not Present In The Directory - Please Check Your File For Emulation\r\n");
		}
	}
	
	if(bFichierPresent == true)
	{
		LiseEd.Lise.MaxSignalEmule = i - CompteurFinFichier;
		fclose(fichier);
	}
	else
		FillSignalEmule(LiseEd);

	
	return STATUS_OK;
}

int StartAcquisition(LISE_ED& LiseEd)
{
#ifdef DEVICECONNECTED
	if(LiseEd.bLiseEDConnected == true)
	{
		if(LiseEd.Lise.T_VoieAnalogIn!=0)
		{
			int32 error;
			error = DAQmxStartTask(LiseEd.Lise.T_VoieAnalogIn); DisplayDAQmxError(LiseEd,error,"StartAcquisition Error");
			return STATUS_OK;
		}
		else return STATUS_FAIL;
	}
	else
	{
		return STATUS_OK;
	}
#else //DEVICECONNECTED
	return STATUS_OK;
#endif //DEVICECONNECTED
}

#define VoltToW  -1.17e-6

int ReadPuissTemperature(LISE_ED& LiseEd,double* Valeur)
{
#ifdef DEVICECONNECTED
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		error = DAQmxReadAnalogScalarF64 (LiseEd.Lise.T_PuissanceREcouplee,LiseEd.Lise.Timeout,Valeur,NULL);
		DisplayDAQmxError(LiseEd,error,"Read Puissance Temperature Error");
		*Valeur *= VoltToW;

		if(error == 0) return STATUS_OK;
		else return STATUS_FAIL;

		return STATUS_OK;
	}
	else
	{
		*Valeur = 0.0;
		return STATUS_OK;
	}
#else //DEVICECONNECTED
	*Valeur = 0.0;
	return STATUS_OK;
#endif //DEVICECONNECTED
}

//puissance recouplee
int ReadPuissAveraged(LISE_ED& LiseEd,double* Valeur)
{
	*Valeur=0;

#ifdef DEVICECONNECTED
	if(LiseEd.bLiseEDConnected == true)
	{
		int32 error;
		int N=0;

		DWORD T=GetTickCount();

		while((GetTickCount()-T<75)&&(N<1))
		{
			double Vimmed=0;
			error = DAQmxReadAnalogScalarF64 (LiseEd.Lise.T_PuissanceREcouplee,LiseEd.Lise.Timeout,&Vimmed,NULL);
			if(DAQmxFailed(error))
			{
				DisplayDAQmxError(LiseEd,error,"Read Puissance Error");
				return STATUS_FAIL;
			}
			*Valeur+=pow(V_Max(Vimmed*VoltToW,0),3);
			N++;
		}
		*Valeur=pow(V_Max(*Valeur,0)/V_Max(N,1),0.3333);
		return STATUS_OK;
	}
	else
#endif

	return STATUS_OK;
}

int AcqReadEmule(LISE_ED& LiseEd,int32& NumSample,double* BufferTemp,int BufferSize)
{
	int SizeBuffer = LiseEd.Lise.MaxSignalEmule - 1;
	int Flag = 0;
	int i = 0;
	int j = LiseEd.IndiceCopieSignalEmule;
	double Noise = 1;
//	int coef;

	// Si l'appareil n'est pas connecté, alors on va aller lire dans le Tableau du Signal Emulé
	for(i=0;i<NumSample;i++)
	{
		BufferTemp[i] = SignalEmule[j];
		BufferTemp[i] = BufferTemp[i] + LiseEd.Lise.fSourceValue * LiseEd.Lise.fNoiseEmulateSignal / (rand()%9 + 1);
        if( ( ((LiseEd.fNiveauHaut + LiseEd.fTolerance) <= BufferTemp[i]) || ((LiseEd.fNiveauHaut - LiseEd.fTolerance) >= BufferTemp[i])) && ( ((LiseEd.fNiveauBas - LiseEd.fTolerance) >= BufferTemp[i]) || ((LiseEd.fNiveauBas + LiseEd.fTolerance) <= BufferTemp[i])))
		{
			if(LiseEd.Lise.fSourceValue >= 0)
			{
				BufferTemp[i] = (( BufferTemp[i] - LiseEd.Lise.LigneDeBase ) * LiseEd.Lise.fSourceValue ) + LiseEd.Lise.LigneDeBase;
				if(BufferTemp[i] >= LiseEd.Lise.fThresholdSaturation)
				{
					BufferTemp[i] = LiseEd.Lise.fThresholdSaturation;
				}
			}
		}
		if(LiseEd.Lise.FlagSaveSignal == true)
		{
			fprintf(LiseEd.Lise.FichierSaveSignal,"%f\r\n",BufferTemp[i]);
		}
		j++;
		if(j>=LiseEd.Lise.MaxSignalEmule)
		{
			j = 0;
		}
	}
	LiseEd.IndiceCopieSignalEmule = j;
	return STATUS_OK;
}

int AcqReaddeviceConnected(LISE_ED& LiseEd,int32& NumSample,double* BufferTemp,int BufferSize)
{
#ifdef DEVICECONNECTED
	if(LiseEd.Lise.T_VoieAnalogIn!=0)
	{
		int32 error = 0;
		int32 read = 0;
		int32 NumAvantAcquisition = NumSample;
		error = DAQmxReadAnalogF64(LiseEd.Lise.T_VoieAnalogIn,DAQmx_Val_Auto,LiseEd.Lise.Timeout,DAQmx_Val_GroupByScanNumber,BufferTemp,NumSample,&read,NULL);
		NumSample = read;
		if(NumSample == 0)
		{ // Cas ou on ne reçoit aucun échantillon
			Sleep(1);	// Sleep de 1 ms
			LiseEd.Lise.iHardwareTimeout++; // On incrémente le compteur Harware Timeout
			if(LiseEd.Lise.iHardwareTimeout >= 1000) // Si on va au dela d'un millier d'itération (plus qu'une seconde)
			{
				LogfileF(*LiseEd.Lise.Log,"No samples in signal found. Check if the device (LISE ED or Ni Card) is plugged or turned on. If it's the case there is probably a problem with the hardware, refers to the instruction manual");
				if(Global.EnableList>=1) MessageBox(0,L"No points in signal found. Check if the device (LISE ED or Ni Card) is plugged or turned on. If it's the case there is probably a problem with the hardware, refers to the instruction manual.",L"Acquisition Error",0);
				LiseEd.Lise.iHardwareTimeout = 0;
				return STATUS_FAIL;
			}
			if(LiseEd.Lise.NbSamplesLastPeriod > LiseEd.Lise.iNbSampleMaxBuffer)
			{
				LogfileF(*LiseEd.Lise.Log,"Problem with electronic signal. Pulse not found or too many points in waveform");
				if(Global.EnableList>=1) MessageBox(0,L"Problem with electronic signal. Pulse not found or too many points in waveform",L"Acquisition Error",0);
				SaveLastWaveformError(LiseEd,1);
				return STATUS_FAIL;
			}
		}
		else
		{
			LiseEd.Lise.iHardwareTimeout = 0;
		}

		if(LiseEd.Lise.NombredeVoie > 1)
		{
			NumSample = LiseEd.Lise.NombredeVoie * NumSample;
		}

        if(error == -200279 || error == -200019)
        {
            // Cas de l'erreur sur l'horloge ou des échantillons que l'on laisse partir
			return error;
        }
        else if (DAQmxFailed(error))
		{
            LogfileF(*LiseEd.Lise.Log,"DAQmx error : %d", error);
			DisplayDAQmxError(LiseEd,error,"AcqRead Error Dans AcqRead");
		}

		// fonction pour le log du signal brut
		LogSignalBrut(LiseEd,BufferTemp,NumSample);

		if(error == 0) return STATUS_OK;
		else return STATUS_FAIL;
	}
	else return STATUS_FAIL;
#endif //DEVICECONNECTED

	return STATUS_OK;
}

int AcqRead(LISE_ED& LiseEd,int32& NumSample,double* BufferTemp,int BufferSize)
{
#ifndef DEVICECONNECTED
	AcqReadEmule(LiseEd,NumSample,BufferTemp,BufferSize);
#else
	if(LiseEd.bLiseEDConnected == true)
	{
		return AcqReaddeviceConnected(LiseEd,NumSample,BufferTemp,BufferSize);
	}
	else
	{
		return AcqReadEmule(LiseEd,NumSample,BufferTemp,BufferSize);
		
	}
	return STATUS_OK;
#endif
	return STATUS_OK;
}

//#define _DEBUG_RING_BUFFER_

int CalculeLongueurBuffer(LISE& Lise,int32& DataLen0, int32& DataLen1)
{

#ifdef _DEBUG_RING_BUFFER_
	FILE* fichier = fopen("Debug_Ring_Buffer.txt","ab");
#endif

	//3 configurations valables TRW(1) RWT(2) WTR(3)
	//3 configurations non valables TWR(4) RTW(5) WRT(6)
	if(Lise.Read.N<=Lise.Write.N)//R<=W
	{
		if(Lise.Write.N<Lise.Trailer.N)//W<T
		{//R<=W et W<T = RWT(2)
			DataLen0=Lise.Trailer.N-Lise.Write.N;//on peut remplir le buffer jusqu'à la fin
			DataLen1=0;
			return STATUS_OK;
		}
		else//T<=W
		{//R<=W et T<=W //T<->R inconnu
			//R<T<=W serait une erreur RTW(5)
			if(Lise.Read.N<Lise.Trailer.N)
			{
				return STATUS_FAIL;//R<T
			}
			//R<=W et T<=W et T<=R = TRW(1)
			DataLen0=Lise.BufferLen-Lise.Write.N;//on peut remplir le buffer jusqu'à la fin
			DataLen1=Lise.Trailer.N;
			return STATUS_OK;
		}
	}
	else if(Lise.Write.N<Lise.Trailer.N)
	{//W<=R et W<T
		//W<=R<T serait une erreur WRT(6)
		if(Lise.Read.N<Lise.Trailer.N)
		{
			return STATUS_FAIL;//R<T
		}
		//W<R et W<T et T<=R = WTR(3)
		DataLen0=Lise.Trailer.N-Lise.Write.N;//on peut remplir le buffer jusqu'au début de la partie non traitée
		DataLen1=0;
		return STATUS_OK;
	}
	else
	{// T<=W<R est un cas d'erreur TWR(4)
		LogfileF(*Lise.Log,"[LISEED]\t[Ring Buffer Error]\tRead=%i\tWrite=%i\tTrailer=%i",Lise.Read.N,Lise.Write.N,Lise.Trailer.N);
		return STATUS_FAIL;
	}
	//return STATUS_OK;
}

int AcqAndProcess(LISE_ED& LiseEd)
{
#ifdef _WATCHTIME_
	LogfileF(*LiseEd.Lise.Log,"[LISEED]\tWatchtime activated.");
	WatchTimeWatch(LiseEd.WatchThreadLoop,"AcqAndProcessEnter");
#endif

	int BufferSize = LiseEd.Lise.BufferLen; 

	int32 a = 0;
	int32 DataLen0 = 0;	// Données écrites dans le Buffer Circulaire
	int32 DataLen1 = 0;	// les données au delà de l'adresse de la fin du Buffer repartent à l'adress 0
	
	int ReturnValue = 0;
	ReturnValue = CalculeLongueurBuffer(LiseEd.Lise,DataLen0,DataLen1);
	if(ReturnValue == STATUS_FAIL)
	{
#ifdef _DEBUG
		if(LiseEd.bLiseEDConnected)
		MessageBox(0,L"Ring buffer index error. Possible problem with synchronisation pulse",L"Acquisition Error",0);
#else
		// on fait un sleep pour ne pas relancer directement l'acauisition
		// temps arbitraire
		Sleep(2000);
#endif
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tRing Buffer Index Error. Possible problem with synchronisation pulses.");
		LogfileF(*LiseEd.Lise.Log,"[LISEED]\tWrite.N=%i\tRead.N=%i\tTrailer.N=%i\tWrite.AbsN=%I64d\tRead.AbsN=%I64d\tTrailer.AbsN=%I64d\t",LiseEd.Lise.Write.N,LiseEd.Lise.Read.N,LiseEd.Lise.Trailer.N,LiseEd.Lise.Write.AbsN,LiseEd.Lise.Read.AbsN,LiseEd.Lise.Trailer.AbsN);
		SaveLastWaveformError(LiseEd,1);
		return STATUS_FAIL;
	}

#ifdef _WATCHTIME_
		WatchTimeWatch(LiseEd.WatchThreadLoop,"CalculBuffLen");
#endif

	int r;
	r = AcqRead(LiseEd,DataLen0,LiseEd.Lise.BufferIntensity + LiseEd.Lise.Write.N,LiseEd.Lise.BufferLen);
	if(r==-200279 || r == -200019)
	{	
		return r;
	}
	RBP_Add(LiseEd.Lise.Write,DataLen0);

	if(DataLen0 && DataLen1 && LiseEd.Lise.Write.N == 0)
	{
		AcqRead(LiseEd,DataLen1,LiseEd.Lise.BufferIntensity + LiseEd.Lise.Write.N,LiseEd.Lise.BufferLen);
		RBP_Add(LiseEd.Lise.Write,DataLen1);
	}
	else
	{
		DataLen1=0;
	}

	int retFindPic = FindPicInBuffer(LiseEd,0);

#ifdef _WATCHTIME_
		WatchTimeWatch(LiseEd.WatchThreadLoop,"FindPic");
#endif

	return retFindPic;

//	return STATUS_OK;
}

// thread de traitement et acquisition
DWORD WINAPI AcquisitionEtProcess( LPVOID lpParam ) 
{ 
	LISE_ED* LiseEd = (LISE_ED*) lpParam;
	if(LiseEd->Lise.bDebug)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[THREAD]\tEntering in thread");
	}
	
	// booléen pour témoigner de l'activation de la thread dans la thread principale
	LiseEd->Lise.bThreadActive = true;

#ifdef _WATCHTIME_
	WatchTimeWatch(LiseEd->WatchThreadUse,"ThreadCreated");
	WatchTimeInit(LiseEd->WatchThreadLoop,"AcquisitionThread");
#endif

	// boucle de thread
	//int _iCountLoop = 0;
	while (!LiseEd->Lise.bStopDemande)
	{
#ifdef _WATCHTIME_
		WatchTimeRestart(LiseEd->WatchThreadLoop);
#endif

		int r = 0;

		r = AcqAndProcess(*LiseEd);

//#ifdef _DEBUG
//		LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[THREAD]\tAcqAndProcess Done: %i",_iCountLoop);
//#endif

		if(r == -200279 || r == -200019 || r == STATUS_FAIL)
		{
			CloseAnalogChannel(*LiseEd);
			StartAnalogChannel(*LiseEd);
			if(LiseEd->Lise.bDebug == true)
			{
				LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[THREAD]\tStopping and Restarting the Acquisition");
			}
			LiseEd->Lise.iThicknessAboveQThreshold = 0;
			// rajout MA pour le traitement sur les la qualité
			LiseEd->Lise.iThicknessMatchingPeakMode = 0;
			LiseEd->Lise.iThicknessBestPeakMode = 0;
			// fin de modif MA
			LiseEd->Lise.bThreadActive = true;
			continue;
		}


#ifdef _WATCHTIME_
		WatchTimeWatch(LiseEd->WatchThreadLoop,"Restart");
#endif
		LiseEd->Lise.bReadAllowed = true;

		int c=0;
		while(LiseEd->Lise.bNeedRead)
		{ // Lorsqu'on lit les données pour afficher, on suspend l'acquisition et process
			Sleep(10);
			if(c++>10)
			{
				if(LiseEd->Lise.bDebug == true)
				{
					LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[THREAD]\tTimeout in thread exceeded - break loop bNeedRead");
				}
				LiseEd->Lise.bNeedRead = false;
				break;
			}
		}

		Sleep(10);
		LiseEd->Lise.bReadAllowed = false;
		//_iCountLoop++;

#ifdef _WATCHTIME_
		WatchTimeWatch(LiseEd->WatchThreadLoop,"SynchroWithThread");
		WatchTimeStop(LiseEd->WatchThreadLoop);
#endif
	}

	if(LiseEd->Lise.bDebug)
	{
		LogfileF(*LiseEd->Lise.Log,"[LISEED]\t[THREAD]\tExit thread");
	}	

	LiseEd->Lise.bThreadActive = false;
	LiseEd->Lise.bStopDemande = false;

#ifdef _WATCHTIME_
	WatchTimeWatch(LiseEd->WatchThreadUse,"ThreadLooFinished");
	WatchTimeClose(LiseEd->WatchThreadLoop);
#endif

	return 0; 
} 
