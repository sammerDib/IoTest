/*
 * $Id: LISE_ED_DLL_Log.cpp 8257 2009-02-16 17:52:50Z S-PETITGRAND $
 */

#include <windows.h>
#include <stdio.h>
#include <string.h>

// ## probe-common headers ##
#include "..\SrcC\SPG.h"
#include "..\SrcC\BreakHook.h"
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
#include "LISE_ED_DLL_Internal.h"
#include "LISE_ED_DLL_Acquisition.h"
#include "LISE_ED_DLL_Config.h"
#include "LISE_ED_DLL_Create.h"
#include "LISE_ED_DLL_General.h"
#include "LISE_ED_DLL_Process.h"
#include "LISE_ED_DLL_Log.h"
#include "LISE_ED_DLL_Reglages.h"


// fonction pour faire un log per channel
int LogPeriodSignal(LISE_ED& LiseEd)
{
	//LiseEd.Lise.bSaveSignalPerChan = true;
	if(LiseEd.Lise.bSaveSignalPerChan)
	{
		// création du répertoire et du filename
		char DirectoryName[1024];strcpy(DirectoryName,LiseEd.Lise.FileNameStartupPath);strcat(DirectoryName,"PeriodSignal\\");

		// on crée le répertoire
		CreateDirectory(fdwstring(DirectoryName),0); 

		for(int j=0;j<LiseEd.Lise.NombredeVoie;j++)
		{
			// name du fichier
			char FileName[128]; sprintf(FileName,"Channel%i_Period%i.txt",j,LiseEd.Lise.Periode);
			char FileInDirectory[2056];strcpy(FileInDirectory,DirectoryName);
			strcat(FileInDirectory,FileName);

			FILE* fichier = fopen(FileInDirectory,"wb");

			if(fichier == NULL){
#ifdef _DEBUG
				BreakHook();
#endif
				return 0;
			}
			// on logggue l'info de période
			fprintf(fichier,"Num Periode : %i\r\n",LiseEd.Lise.Periode);
			fprintf(fichier,"Num Sample: %i\r\n",LiseEd.Lise.NbSamplesLastPeriod);

			// on bloucle sur l'ensemble des échantillons
			for(int i=0;i<LiseEd.Lise.NbSamplesLastPeriod;i++)
			{
				fprintf(fichier,"%f\r\n",LiseEd.Lise.BufferPeriodChannel[j][i]);
			}

			// fermeture du fichier
			fclose(fichier);
		}
	}

	return 1;
}

// fonction pour logguer dans un fichier les signaux brut comprenant n colonnes
void LogFilePeriodNSignal(LISE_ED& LiseEd)
{
	// si permis de ssauvegarder le fichier signal brut
	if(LiseEd.Lise.bAllowSaveRawSignal)
	{
		// création du répertoire et du filename
		char DirectoryName[1024];strcpy(DirectoryName,LiseEd.Lise.FileNameStartupPath);strcat(DirectoryName,"\\RawSignal\\");
		char FileName[128]; sprintf(FileName,"%i_%s",LiseEd.Lise.Periode,LiseEd.Lise.FileNameRawSignal);

		// on crée le répertoire
		CreateDirectory(fdwstring(DirectoryName),0);

		// name du fichier
		char FileInDirectory[2056];strcpy(FileInDirectory,DirectoryName);
		strcat(FileInDirectory,FileName);

		FILE* fichier = fopen(FileInDirectory,"wb");
		if(fichier == NULL){
#ifdef _DEBUG
			BreakHook();
#endif
			return;
		}
		// on logggue l'info de période
		fprintf(fichier,"Num Periode : %i\r\n",LiseEd.Lise.Periode);
		fprintf(fichier,"Num Sample: %i\r\n",LiseEd.Lise.NbSamplesLastPeriod);

		// on bloucle sur l'ensemble des échantillons
		for(int i=0;i<LiseEd.Lise.NbSamplesLastPeriod;i++)
		{
			// on boucle sur les 
			for(int j=0;j<LiseEd.Lise.NombredeVoie;j++)
			{
				fprintf(fichier,"%f\t",LiseEd.Lise.BufferPeriodChannel[j][i]);
			}
			fprintf(fichier,"\r\n");
		}

		// fermeture du fichier
		fclose(fichier);
	}
}

// fonction pour logguer le signal brut à l'acquisition
void LogSignalBrut(LISE_ED& LiseEd,double* BufferTemp,int NumSample)
{
	//LiseEd.Lise.bSaveSignalBrut = true;
	// log si l'instruction est définie
	if(LiseEd.Lise.bSaveSignalBrut)
	{
		// création du répertoire et du filename
		char DirectoryName[1024];strcpy(DirectoryName,LiseEd.Lise.FileNameStartupPath);strcat(DirectoryName,"\\RawSignal\\");
		char FileName[128]; sprintf(FileName,"SignalBrut_%i.txt",LiseEd.Lise.iCounterSignalBrut);

		// on crée le répertoire
		CreateDirectory(fdwstring(DirectoryName),0); strcat(DirectoryName,FileName);

		int iCountSample = 0;
		FILE* fichier = fopen(DirectoryName,"wb"); 
		if(fichier == NULL){
#ifdef _DEBUG
			BreakHook();
#endif
			return;
		}
		fprintf(fichier,"Num Sample: %i\r\n",NumSample);
		// on loggue tous les échantillons
		while(iCountSample<NumSample)
		{
			for(int j=0;j<LiseEd.Lise.NombredeVoie;j++)
			{
				fprintf(fichier,"%f\t",BufferTemp[iCountSample]);
				iCountSample++;
			}
			fprintf(fichier,"\r\n");
		}
		fclose(fichier);
	}
	LiseEd.Lise.iCounterSignalBrut++;
}


// ################################### Log dand pic moyenne ###########################################

// log dans le pic moyenne des refine values
void LogPeakRefine(LISE& Lise,PERIOD_RESULT* LastNMeasure,double* refineValues,bool bMoins)
{
	//seulement si le mode de sauvegarde est enclenché
	if(Lise.bAllowSavePeakMoyenne)
	{
		ManagePicMoyenne(Lise);

		// on écrit dans un fichier les best peaks
		FILE* fichier = fopen(Lise.FileNameSavePeakMoyenne,"ab");

		if(fichier == NULL){
#ifdef _DEBUG
			BreakHook();
#endif
			return;
		}
		//fprintf(fichier,"\r\n%s: open\r\n",__FUNCTION__); fflush(fichier);

		// log des refine values
		fprintf(fichier,"%i\t%f\t%f\tQT = %f\t\r\n",bMoins,refineValues[0],refineValues[1],Lise.sample.QualityThreshold);
		
		if(!bMoins)
		{
			for(int i = 0;i<8;i++) fprintf(fichier,"%i\t%f\t%f\t",bMoins,LastNMeasure->PicsPlusVoie1[i].XRel,LastNMeasure->PicsPlusVoie1[i].Qualite);
		}
		else
		{
			for(int i = 0;i<8;i++) fprintf(fichier,"%i\t%f\t%f\t",bMoins,LastNMeasure->PicsMoinsVoie1[i].XRel,LastNMeasure->PicsMoinsVoie1[i].Qualite);
		}
		fprintf(fichier,"\r\n");

		// on ferme le fichier
		//fprintf(fichier,"\r\n%s: close\r\n",__FUNCTION__); fflush(fichier);
		fclose(fichier);
	}
}

// fonction pour le log des pics
void LogPeakUsed(LISE& Lise,PERIOD_RESULT* LastNMeasure,int Index,int Mode)
{
	////seulement si le mode de sauvegarde est enclanché
	//if(Lise.bAllowSavePeakMoyenne)
	//{
	//	// on écrit dans un fichier les best peaks
	//	FILE* fichier = fopen(Lise.FileNameSavePeakMoyenne,"ab");
	//	// Mode
	//	if(LastNMeasure->MatchMode==BestPeak) fprintf(fichier,"BestPeaks\t");
	//	else fprintf(fichier,"MatchingS\t");

	//	// on printe le mode Single Shot ou Continue
	//	if(Mode == 0)	fprintf(fichier,"SingleS\t");
	//	else	fprintf(fichier,"Continu\t");

	//	// NumMoy|th1|th2|quality|PK+Voie1.1|PK+Voie1.2|PK+Voie1.3|PK-Voie1.1|PK-Voie1.2|PK-Voie1.3|th0+|th1+|th0-|th1-
	//	fprintf(fichier,"%i\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t\r\n",Index,LastNMeasure->fThickness[0],LastNMeasure->fThickness[1],LastNMeasure->fQuality,LastNMeasure->PicsPlusVoie1[LastNMeasure->PkIndexPlus[0]].XRel,LastNMeasure->PicsPlusVoie1[LastNMeasure->PkIndexPlus[0]].Qualite,LastNMeasure->PicsPlusVoie1[LastNMeasure->PkIndexPlus[1]].XRel,LastNMeasure->PicsPlusVoie1[LastNMeasure->PkIndexPlus[1]].Qualite,LastNMeasure->PicsPlusVoie1[LastNMeasure->PkIndexPlus[2]].XRel,LastNMeasure->PicsPlusVoie1[LastNMeasure->PkIndexPlus[2]].Qualite,LastNMeasure->PicsMoinsVoie1[LastNMeasure->PkIndexMoins[0]].XRel,LastNMeasure->PicsMoinsVoie1[LastNMeasure->PkIndexMoins[0]].Qualite,LastNMeasure->PicsMoinsVoie1[LastNMeasure->PkIndexMoins[1]].XRel,LastNMeasure->PicsMoinsVoie1[LastNMeasure->PkIndexMoins[1]].Qualite,LastNMeasure->PicsMoinsVoie1[LastNMeasure->PkIndexMoins[2]].XRel,LastNMeasure->PicsMoinsVoie1[LastNMeasure->PkIndexMoins[2]].Qualite,LastNMeasure->fThicknessPlus[0],LastNMeasure->fThicknessPlus[1],LastNMeasure->fThicknessMoins[0],LastNMeasure->fThicknessMoins[1]);
	//	// on ferme le fichier
	//	fclose(fichier);
	//}

	//seulement si le mode de sauvegarde est enclanché
	if(Lise.bAllowSavePeakMoyenne)
	{
		ManagePicMoyenne(Lise);

		// on écrit dans un fichier les best peaks
		FILE* fichier = fopen(Lise.FileNameSavePeakMoyenne,"ab");
		if(fichier == NULL){
#ifdef _DEBUG
			BreakHook();
#endif
			return;
		}
		//fprintf(fichier,"\r\n%s: open\r\n",__FUNCTION__); fflush(fichier);

		// Mode
		//PERIOD_RESULT LastNMeasure = LiseEd.Lise.Resultats[RBP_GetNMinusOne(IndiceCalculMoyenne)];
		int iCounterMode = 9999;
		if(LastNMeasure->MatchMode==BestPeak)
		{
			fprintf(fichier,"BestPeaks\t");
			iCounterMode = LastNMeasure->iCounterBestPeaks;
		}
		else
		{
			fprintf(fichier,"MatchingS\t");
			iCounterMode = LastNMeasure->iCounterMatchingS;
		}

		//	// on printe le mode Single Shot ou Continue
		if(Mode == 0)	fprintf(fichier,"SingleS\t");
		else	fprintf(fichier,"Continu\t");

		// NumMoy|th1|th2|quality|PK+Voie1.1|QK+Voie1.1|PK+Voie1.2|QK+Voie1.2|PK+Voie1.3|QK+Voie1.3|PK-Voie1.1|QK-Voie1.1|PK-Voie1.2|QK-Voie1.2|PK-Voie1.3|QK-Voie1.3|th0+|th1+|th0-|th1-
		fprintf(fichier,"%i\t%i\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t%f\t",Index,iCounterMode,LastNMeasure->fThickness[0],LastNMeasure->fThickness[1],LastNMeasure->fQuality,LastNMeasure->PicsPlusVoie1[LastNMeasure->PkIndexPlus[0]].XRel,LastNMeasure->PicsPlusVoie1[LastNMeasure->PkIndexPlus[0]].Qualite,LastNMeasure->PicsPlusVoie1[LastNMeasure->PkIndexPlus[1]].XRel,LastNMeasure->PicsPlusVoie1[LastNMeasure->PkIndexPlus[1]].Qualite,LastNMeasure->PicsPlusVoie1[LastNMeasure->PkIndexPlus[2]].XRel,LastNMeasure->PicsPlusVoie1[LastNMeasure->PkIndexPlus[2]].Qualite,LastNMeasure->PicsMoinsVoie1[LastNMeasure->PkIndexMoins[0]].XRel,LastNMeasure->PicsMoinsVoie1[LastNMeasure->PkIndexMoins[0]].Qualite,LastNMeasure->PicsMoinsVoie1[LastNMeasure->PkIndexMoins[1]].XRel,LastNMeasure->PicsMoinsVoie1[LastNMeasure->PkIndexMoins[1]].Qualite,LastNMeasure->PicsMoinsVoie1[LastNMeasure->PkIndexMoins[2]].XRel,LastNMeasure->PicsMoinsVoie1[LastNMeasure->PkIndexMoins[2]].Qualite,LastNMeasure->fThicknessPlus[0],LastNMeasure->fThicknessPlus[1],LastNMeasure->fThicknessMoins[0],LastNMeasure->fThicknessMoins[1]);
		fprintf(fichier,"%i\t%i\t%i\t\r\n",LastNMeasure->bMatchSuccessPlus,LastNMeasure->bMatchSuccessMoins,LastNMeasure->bMatchSuccessGoAndBack);
		
		// on ferme le fichier
		//fprintf(fichier,"\r\n%s: close\r\n",__FUNCTION__); fflush(fichier);
		fclose(fichier);
	}
}

// fonction pour logguer les éléments de position dans le buffer et valeur du peak
void LogPeakRBIInfo(LISE& Lise,int Index,double ThicknessValue,int ModeMatching)
{
	//seulement si le mode de sauvegarde est enclenché
	if(Lise.bAllowSavePeakMoyenne)
	{
		ManagePicMoyenne(Lise);

		// on écrit dans un fichier les best peaks
		FILE* fichier = fopen(Lise.FileNameSavePeakMoyenne,"ab");
		if(fichier == NULL){
#ifdef _DEBUG
			BreakHook();
#endif
			return;
		}
		//fprintf(fichier,"\r\n%s: open\r\n",__FUNCTION__); fflush(fichier);

		if(ModeMatching == 0)
		{
			fprintf(fichier,"BestPeak\t");
		}
		else if(ModeMatching == 1)
		{
			fprintf(fichier,"MatchingS\t"); 
		}
		else
		{
			fprintf(fichier,"NotTagged\t"); 
		}

		fprintf(fichier,"RBI : %i\t%f\t\r\n",Index,ThicknessValue);
		//fprintf(fichier,"\r\n%s: close\r\n",__FUNCTION__); fflush(fichier);

		fclose(fichier);
	}
}
// log du resultat sur les infos
void LogPeakRBIInfoResult(LISE& Lise,int Index,int NumBestPeak, int NumMatchingSuccess, int IndexStart, int IndexStop)
{
	if(Lise.bAllowSavePeakMoyenne)
	{
		FILE* fichier = fopen(Lise.FileNameSavePeakMoyenne,"ab");
		if(fichier == NULL){
#ifdef _DEBUG
			BreakHook();
#endif
			return;
		}
		//fprintf(fichier,"\r\n%s: open\r\n",__FUNCTION__); fflush(fichier);

		fprintf(fichier,"BestPeak : %i\t MatchingS : %i\r\n",NumBestPeak,NumMatchingSuccess);
		fprintf(fichier,"Start : %i\t Stop: %i\r\n", IndexStart,IndexStop);
		fprintf(fichier,"--------------------------------\r\n");
		//fprintf(fichier,"\r\n%s: close\r\n",__FUNCTION__); fflush(fichier);

		fclose(fichier);
	}
}
// pour ajouter un commentaire au fichier de log
void LogPeakComment(LISE& Lise,char* Comment)
{
	//seulement si le mode de sauvegarde est enclenché
	if(Lise.bAllowSavePeakMoyenne)
	{
		// on écrit dans un fichier les best peaks
		FILE* fichier = fopen(Lise.FileNameSavePeakMoyenne,"ab");
		if(fichier == NULL){
#ifdef _DEBUG
			BreakHook();
#endif
			return;
		}
		//fprintf(fichier,"\r\n%s: open\r\n",__FUNCTION__); fflush(fichier);

		fprintf(fichier,"%s",Comment);
		fprintf(fichier,"\r\n");
		//fprintf(fichier,"\r\n%s: close\r\n",__FUNCTION__); fflush(fichier);

		fclose(fichier);
	}
}
// pour ajouter un commentaire au fichier de log
void LogPeakCommentResult(LISE& Lise,char* Comment)
{
	//seulement si le mode de sauvegarde est enclanché
	if(Lise.bAllowSavePeakMoyenne)
	{
		// on écrit dans un fichier les best peaks
		FILE* fichier = fopen(Lise.FileNameSavePeakMoyenne,"ab");
		if(fichier == NULL){
#ifdef _DEBUG
			BreakHook();
#endif
			return;
		}
		//fprintf(fichier,"\r\n%s: open\r\n",__FUNCTION__); fflush(fichier);

		fprintf(fichier,"-------------------------------RESULT----------------------------\r\n");
		fprintf(fichier,"%s",Comment);
		fprintf(fichier,"\r\n");
		fprintf(fichier,"-------------------------------BREAK----------------------------\r\n");
		//fprintf(fichier,"\r\n%s: close\r\n",__FUNCTION__); fflush(fichier);

		fclose(fichier);
	}
}
// fonction pour le resultat
void LogPeakResult(LISE& Lise,double* Thickness, double* Quality)
{
	if(Lise.bAllowSavePeakMoyenne)
	{
		// on écrit dans un fichier les best peaks
		FILE* fichier = fopen(Lise.FileNameSavePeakMoyenne,"ab");
		if(fichier == NULL){
#ifdef _DEBUG
			BreakHook();
#endif
			return;
		}
		//fprintf(fichier,"\r\n%s: open\r\n",__FUNCTION__); fflush(fichier);

		fprintf(fichier,"-------------------------------RESULT----------------------------\r\n");
		fprintf(fichier,"%f\t%f\t%f\t\r\n",Thickness[0],Thickness[1],Quality[0]);
		fprintf(fichier,"-------------------------------BREAK----------------------------\r\n");
		//fprintf(fichier,"\r\n%s: close\r\n",__FUNCTION__); fflush(fichier);
		fclose(fichier);
	}
}

// log de la probe courrante dans le fichier peak moyenne 
void LogCurrentProbe(LISE& Lise, int CurrentProbe)
{
	if(Lise.bAllowSavePeakMoyenne)
	{
		// on écrit dans un fichier les best peaks
		FILE* fichier = fopen(Lise.FileNameSavePeakMoyenne,"ab");
		if(fichier == NULL){
#ifdef _DEBUG
			BreakHook();
#endif
			return;
		}
		//fprintf(fichier,"\r\n%s: open\r\n",__FUNCTION__); fflush(fichier);

		fprintf(fichier,"-------------------------------CUR PROBE----------------------------\r\n");
		fprintf(fichier,"%i\t\r\n",CurrentProbe);
		fprintf(fichier,"-------------------------------CUR PROBE----------------------------\r\n");
		//fprintf(fichier,"\r\n%s: close\r\n",__FUNCTION__); fflush(fichier);
		fclose(fichier);
	}
}

// fonction restart du fichier pic moyenne associe a la probe
void RestartPicMoyenne(LISE& Lise)
{
	// On ouvre le fichier
	if(Lise.bAllowSavePeakMoyenne)
	{
		FILE* fichier = fopen(Lise.FileNameSavePeakMoyenne,"r");
		if(fichier == NULL){
#ifdef _DEBUG
			BreakHook();
#endif
			return;
		}
		// si le fichier existe
		if(fichier != NULL)
		{
			// fermeture puis suppression
			fclose(fichier);
			DeleteFileA((LPCSTR)Lise.FileNameSavePeakMoyenne);

			// restart de l'indice de mesure
			Lise.iCounterMeasure = 0;
		}
	}
}
void ManagePicMoyenne(LISE& Lise){

	//LogfileUpdate(Lise.FileNameSavePeakMoyenne);

	int lSize = 0;
	SPG_GetFileSize(Lise.FileNameSavePeakMoyenne,lSize);

	// Rotation du fichier de log si supérieur à 4Mo.
	if (lSize > 4*1024*1024)
	{
		RestartPicMoyenne(Lise);
	}
}
/*int fsize(const char * fname, long * ptr)
{
    
    FILE * f;
    int ret = 0;
    
    f = fopen(fname, "rb");   
    if (f != NULL)
    {
        fseek(f, 0, SEEK_END); 
        *ptr = ftell(f); 
        fclose(f);
    }
    else
        ret = 1;
    
    return ret;
}*/
// ################################# Log dans pic moyenne #############################################

//int LogOutput;//0 = message box, 1 = fichier de log, 2 = les deux, 3 = ...
void LogLEDInfo(LOGFILE& log,char* Message,bool bDisplayInLogFile)
{
	if(bDisplayInLogFile) LogfileF(log,Message);
}

void __cdecl logF(LISE_ED* LiseEd, char* format, ...)
{
	if(LiseEd->Lise.bDebug)
	{
		char message[MAXLOGMSG];
		va_list args;
		va_start(args, format);
		_vsnprintf(message, MAXLOGMSG, format, args);
		va_end(args);
		message[MAXLOGMSG - 1] = '\0';
		Logfile(*LiseEd->Lise.Log, message);
	}
	return;
}

int SaveLastWaveformError(LISE_ED& LiseEd,bool bFunctionInThread)
{ // sous - programme permettant de sauvegarder la dernière 
	// Synchronisation avec la thread d'acquisition. Mieux vaut-il une synchro avec la threa ou arrêter l'acquisition?
	LiseEd.Lise.bNeedRead = true;	// parametre pour la thread
	if(bFunctionInThread == false)
	{
		if(LiseEd.Lise.bThreadActive == true)
		{
			while(!LiseEd.Lise.bReadAllowed)	// parametre pour la thread
			{
				Sleep(1);
			}
		}
	}
	int NumberPointToPrint = 100000;
	// Ou On arrête la thread d'acquisition ...  a débattre
	// CloseAnalogChannel(LISE);
	FILE* fichier = fopen("ErrorWaveform.txt","wb");
	if(LiseEd.Lise.Read.AbsN >= NumberPointToPrint)
	{ // Attention si la thread est toujours active, problème de copie. Gérer le cas
		RING_BUFFER_POS IndiceCopyBuffer = LiseEd.Lise.Read;
		RBP_Sub(IndiceCopyBuffer,NumberPointToPrint);
		while(IndiceCopyBuffer.AbsN < LiseEd.Lise.Read.AbsN)
		{
			fprintf(fichier,"%f\r\n",LiseEd.Lise.BufferIntensity[IndiceCopyBuffer.AbsN]);
			RBP_Inc(IndiceCopyBuffer);
		}
		LogfileF(*LiseEd.Lise.Log,"[LISEED`]\tSave Last waveform period error");
	}
	else if(LiseEd.Lise.Read.AbsN < NumberPointToPrint)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED`]\tWarning, number of point < BufferLen. Saving available data.");
		RING_BUFFER_POS IndiceCopyBuffer = LiseEd.Lise.Read;
		// On ne récupère que les points disponible
		RBP_Sub(IndiceCopyBuffer,LiseEd.Lise.Read.AbsN);
		while(IndiceCopyBuffer.AbsN < LiseEd.Lise.Read.AbsN)
		{
			fprintf(fichier,"%f\r\n",LiseEd.Lise.BufferIntensity[IndiceCopyBuffer.AbsN]);
			RBP_Inc(IndiceCopyBuffer);
		}
		LogfileF(*LiseEd.Lise.Log,"[LISEED`]\tSave Last waveform period error");
	}
	else if(LiseEd.Lise.Read.AbsN == 0)
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED`]\tNo points available in SaveLastWaveformError. Impossible to save Last Period");
		fclose(fichier);
		if(bFunctionInThread == false)
		{
			LiseEd.Lise.bNeedRead = false;
		}
		// StartAnalogChannel(LISE);
		return STATUS_FAIL;
	}
	else
	{
		LogfileF(*LiseEd.Lise.Log,"[LISEED`]\tNo points available in SaveLastWaveformError. Impossible to save Last Period");
		fclose(fichier);
		if(bFunctionInThread == false)
		{
			LiseEd.Lise.bNeedRead = false;
		}
		// StartAnalogChannel(LISE);
		return STATUS_FAIL;
	}
	fclose(fichier);
	if(bFunctionInThread == false)
	{
		LiseEd.Lise.bNeedRead = false;
	}
	// StartAnalogChannel(LISE);
	return STATUS_OK;
}

#ifndef NOHARDWARE
void DisplayDAQmxError(LISE_ED& LiseEd,int32 error,char* FileError)
{
#ifdef DEVICECONNECTED
	if(LiseEd.bLiseEDConnected == true)
	{
		if( DAQmxFailed(error) )
		{
			char errBuff[2048];
			DAQmxGetExtendedErrorInfo(errBuff,2048);
			if(LiseEd.Lise.bDebug == true)
			{
				LogfileF(*LiseEd.Lise.Log,errBuff);
			}
			if(LiseEd.Lise.DisplayNIError == 0)
			{
				if(Global.EnableList>=1) MessageBox(0,fdwstring(errBuff),fdwstring(FileError),0);
			}
		}
	}
#endif //DEVICECONNECTED
}
#endif

/*
#ifdef _WATCHTIME_
// fonction watchtime pour aller surveiller les boucles de mesures

void WatchTimeInit(LISE_ED& LiseEd,char* Name)
{
	// on configure le file path
	char CurrentDir[LONG_STR];
	GetCurrentDirectoryA(LONG_STR,(LPSTR)CurrentDir);
	strcpy(LiseEd.WatchThreadLoop.FileName,CurrentDir);
	strcat(LiseEd.WatchThreadLoop.FileName,"\\WatchtimeDirectory\\");
	CreateDirectoryA(LiseEd.WatchThreadLoop.FileName,NULL);
	strcat(LiseEd.WatchThreadLoop.FileName,Name);
	strcat(LiseEd.WatchThreadLoop.FileName,".txt");
	
	// on copie le nom
	strcpy(LiseEd.WatchThreadLoop.Name,Name);

	// initialisation du fichier
	LiseEd.WatchThreadLoop.FileWatch = fopen(LiseEd.WatchThreadLoop.FileName,"wb");

	// le fichier est ouvert, on autorise le reste des operations
	LiseEd.WatchThreadLoop.bInitialized = true;
	LiseEd.WatchThreadLoop.bWatchPointHeaderPrint = false;
	LiseEd.WatchThreadLoop.iNumWatchPoint = 0;
}
void WatchTimeRestart(LISE_ED& LiseEd)
{
	if(LiseEd.WatchThreadLoop.bInitialized)
	{
		// on recupere l'heure
		SYSTEMTIME Time;
		GetLocalTime(&Time);
		fprintf(LiseEd.WatchThreadLoop.FileWatch,"%d:%d:%d\t",Time.wHour,Time.wMinute,Time.wSecond);

		LiseEd.WatchThreadLoop.LastTimeWatch = clock();
	}
}
void WatchTimeWatch(LISE_ED& LiseEd, char* Name)
{
	if(LiseEd.WatchThreadLoop.bInitialized)
	{
		// on loggue dans le fichier le temps et on le met a jour
		fprintf(LiseEd.WatchThreadLoop.FileWatch,"%d\t",(clock() - LiseEd.WatchThreadLoop.LastTimeWatch));
		LiseEd.WatchThreadLoop.LastTimeWatch = clock();

		if(!LiseEd.WatchThreadLoop.bWatchPointHeaderPrint)
		{
			strcpy(LiseEd.WatchThreadLoop.WatchPointName[LiseEd.WatchThreadLoop.iNumWatchPoint],Name);
			LiseEd.WatchThreadLoop.iNumWatchPoint++;
		}
	}
}
void WatchTimeStop(LISE_ED& LiseEd)
{
	if(LiseEd.WatchThreadLoop.bInitialized)
	{
		// on loggue dans le fichier le temps et on le met a jour
		fprintf(LiseEd.WatchThreadLoop.FileWatch,"\r\n");

		if(!LiseEd.WatchThreadLoop.bWatchPointHeaderPrint) 
		{
			LiseEd.WatchThreadLoop.bWatchPointHeaderPrint = true; 
			for(int i =0;i<LiseEd.WatchThreadLoop.iNumWatchPoint;i++)
			{
				fprintf(LiseEd.WatchThreadLoop.FileWatch,"%s\t",LiseEd.WatchThreadLoop.WatchPointName[i]);
			}
		}
	}
}
void WatchTimeClose(LISE_ED& LiseEd)
{
	if(LiseEd.WatchThreadLoop.bInitialized)
	{
		// on ferme le fichier
		fclose(LiseEd.WatchThreadLoop.FileWatch);
		LiseEd.WatchThreadLoop.bInitialized = false;
		LiseEd.WatchThreadLoop.bWatchPointHeaderPrint = false;
		LiseEd.WatchThreadLoop.iNumWatchPoint = 0;
	}
}
#endif
*/

/*
-------------------------------------------------------------------------------

//exemple d'utilisation :

    __int64 start;
    double ellapsedTime;

    start = timGetPerfCount();

    // traitement...

    ellapsedTime = timGetElapsedTimeMs(start);

*/

#ifdef _WATCHTIME_
// fonction watchtime pour aller surveiller les boucles de mesures

__int64 timGetPerfCount()
{
	__int64 counter;

	QueryPerformanceCounter((LARGE_INTEGER*)&counter);
	return counter;
}

void timInitPerfCountfreq(WATCH_TIME_STRUCT& Watcher)
{
	QueryPerformanceFrequency((LARGE_INTEGER*)&Watcher.freq);
}

double timGetElapsedTimeMs_Diff(WATCH_TIME_STRUCT& Watcher, __int64 end)
{
    if(!Watcher.freq)
	{
		timInitPerfCountfreq(Watcher);
	}

	return ((double)end - Watcher.i64startTimeWatch) / Watcher.freq*1000;
}

double timGetElapsedTimeMs_Simple(WATCH_TIME_STRUCT& Watcher)
{
    return timGetElapsedTimeMs_Diff(Watcher , timGetPerfCount());
}

void WatchTimeInit(WATCH_TIME_STRUCT& Watcher,char* Name)
{
	// on configure le file path
	char CurrentDir[LONG_STR];
	GetCurrentDirectoryA(LONG_STR,(LPSTR)CurrentDir);
	strcpy(Watcher.FileName,CurrentDir);
	strcat(Watcher.FileName,"\\WatchtimeDirectory\\");
	CreateDirectoryA(Watcher.FileName,NULL);
	//LogfileF(*LiseEd.Lise.Log,"[LISEED]\t[THREAD]\t[WATCHTIME]\tWatchTime directory: %s",LiseEd.WatchThreadLoop.FileName);

	// concatenation pour le nom de fichier
	strcat(Watcher.FileName,Name);
	strcat(Watcher.FileName,".txt");
	
	// on copie le nom
	strcpy(Watcher.Name,Name);

	// initialisation du fichier
	Watcher.FileWatch = fopen(Watcher.FileName,"wb");

	// le fichier est ouvert, on autorise le reste des operations
	Watcher.bInitialized = true;
	Watcher.bWatchPointHeaderPrint = false;
	Watcher.iNumWatchPoint = 0;
	Watcher.dTotalTimeLoop = 0.0;

	// initialisation du compteur de fréquence
	timInitPerfCountfreq(Watcher);
	//LogfileF(*LiseEd.Lise.Log,"[LISEED]\t[THREAD]\t[WATCHTIME]\tWatchTime Activated");
}
void WatchTimeRestart(WATCH_TIME_STRUCT& Watcher)
{
	if(Watcher.bInitialized)
	{
		// on recupere l'heure
		SYSTEMTIME Time;
		GetLocalTime(&Time);
		fprintf(Watcher.FileWatch,"%d:%d:%d\t",Time.wHour,Time.wMinute,Time.wSecond);

		Watcher.i64startTimeWatch = timGetPerfCount();
		Watcher.dTotalTimeLoop = 0.0;
	}
}
void WatchTimeWatch(WATCH_TIME_STRUCT& Watcher, char* Name)
{
	if(Watcher.bInitialized)
	{
		// on récupère le temps de précision
		double ellapsedTime = timGetElapsedTimeMs_Diff(Watcher,timGetPerfCount());

		// on loggue dans le fichier le temps et on le met a jour
		fprintf(Watcher.FileWatch,"%f\t",ellapsedTime);
		Watcher.dTotalTimeLoop += ellapsedTime;

		Watcher.i64startTimeWatch = timGetPerfCount();

		if(!Watcher.bWatchPointHeaderPrint)
		{
			strcpy(Watcher.WatchPointName[Watcher.iNumWatchPoint],Name);
			Watcher.iNumWatchPoint++;
		}
	}
}
void WatchTimeStop(WATCH_TIME_STRUCT& Watcher)
{
	if(Watcher.bInitialized)
	{
		if(!Watcher.bWatchPointHeaderPrint) 
		{
			Watcher.bWatchPointHeaderPrint = true; 
			for(int i =0;i<Watcher.iNumWatchPoint;i++)
			{
				fprintf(Watcher.FileWatch,"%s\t",Watcher.WatchPointName[i]);
			}

			// on ajoute le message total
			fprintf(Watcher.FileWatch,"Total\t");
			fprintf(Watcher.FileWatch,"\r\n");
		}
		else
		{
			// on loggue dans le fichier le temps et on le met a jour
			fprintf(Watcher.FileWatch,"%f\t",Watcher.dTotalTimeLoop);
			fprintf(Watcher.FileWatch,"\r\n");
		}
	}
}
void WatchTimeClose(WATCH_TIME_STRUCT& Watcher)
{
	if(Watcher.bInitialized)
	{
		// on ferme le fichier
		fclose(Watcher.FileWatch);
		Watcher.bInitialized = false;
		Watcher.bWatchPointHeaderPrint = false;
		Watcher.iNumWatchPoint = 0;
	}
}
#endif