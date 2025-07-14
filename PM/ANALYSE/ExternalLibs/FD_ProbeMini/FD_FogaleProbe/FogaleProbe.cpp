/*
 * $Id: FogaleProbe.cpp 9539 2009-07-03 07:10:24Z m-abet $
 */
#include <windows.h>
#include <stdio.h>
#include <string.h>
#include "crtdbg.h"

#include <shlobj.h> //SHGetKnownFolder...

// ## public headers ##
#include "FogaleProbeReturnValues.h"
#include "FogaleProbeParamID.h"

#define FOGALEPROBE_EXPORTS
#include "../LiseHardwareConfiguration.h"
#include "FogaleProbe.h"
// ## public headers ##

// ## probe-common headers ##
#include "..\SrcC\SPG.h"
#include "..\SrcC\BreakHook.h"
#include "NIDAQmxConfig.h"
// ## probe-common headers ##

// ## probe-specific headers ##
#ifdef FDE
#include "../FP_CHROM/Comm.h"
#include "../FP_CHROM/Chrom.h"
#include "../FP_CHROM/CHR_Module.h"
#include "../FP_CHROM/CHR_Double_Module.h"
#endif
#ifdef SPG_General_USESTIL
// STIL
#include "../FP_STIL/STIL_Module.h"
#endif
// SPIRO
#ifdef FDE
#include "../FP_SPIRO/SPIRO_Module.h"
#endif

#include "../FD_LISE_General/LISE_Consts.h"
#include "../FD_LISE_General/PeakMatch.h"
#include "../FD_LISE_General/LISE_Struct_Process.h"
#include "../FD_LISE_General/LISE_Struct.h"

// #include "../FD_LISELS/LISE_LSLI_DLL_Internal.h"

#include "../FD_LISEED/LISE_ED_DLL_UI_Struct.h"
#include "../FD_LISEED/LISE_ED_DLL_Internal.h"

#include "../FD_DBLLISEED/DBL_LISE_ED_DLL_Internal.h"

#ifdef FDE
#include "../FP_LISE_ED_EXTENDED/LISE_ED_EXT_Internal.h"
#endif

// #### LENSCAN ####
// // #include "..\FD_LenScan\LenScanUI.h"
#ifdef FDE
#include "..\FD_LenScanMotorAxis\MotorAxis.h"
#include "..\FD_LenScanMotorAxis\MotorAxisWindow.h"
#endif
// #include "..\FD_LenScan\LenScanHardware.h"
// #include "..\FD_LenScan\LenScanDemodulation.h"
// #include "..\FD_LenScan\LenScanSignal.h"
// #include "..\FD_LenScan\LMatch.h"
//#include "..\FD_LenScanMotorAxis\MotorAxis.h"
// #include "..\FD_LenScan\GBPMConstants.h" 
// #include "..\FD_LenScan\MaterialDatabase.h"
// #include "..\FD_LenScan\LenScanMeas.h"
// #include "..\FD_LenScan\LenScanProbeStruct.h"
// #include "..\FD_LenScan\LenScanProbe.h"
// #### LENSCAN ####



// ## probe-specific headers ##

// ## internal headers ##
#include "FogaleProbeCommonInterface.h"
#include "FogaleProbeInternal.h"
// ## internal headers ##


//efine HASP
#define FOGALEPROBE_ASMODULE

#ifndef HASP
#pragma SPGMSG(__FILE__,__LINE__,"###########          HASP DISABLED          ###########")
#else
// include des drivers pour clé HASP
#include "..\HASP_SDK\HASP.h"
#pragma comment(lib,"..\\HASP_SDK\\libhasp_windows.lib")
#endif


FPDLL_STATE s;

#define VERSION_OEM	"OEM"
#define VERSION_STAND_ALONE	"STAND_ALONE"


BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved
					 )
{
    switch (ul_reason_for_call)
	{
		case DLL_PROCESS_ATTACH:
			//FPDLLInit();
			break;
		case DLL_PROCESS_DETACH:
			//FPDLLClose();
			break;
			/*
		case DLL_THREAD_ATTACH:
		case DLL_THREAD_DETACH:
			*/
    }
    return TRUE;
}


// Returns FOGALEPROBE_VERSION (to check correspondance between header and DLL)
int FP_API FPGetVersion()
{
	return FOGALEPROBE_VERSION;
}

// function to set software in stand alone versio
FPDLLEXP int FP_API FPSetDllVersion(bool bStandAlone)
{
	if(bStandAlone)
	{	// définition de la dll pour le mode stand alone avec l'IHM Fogale
		strcpy(s.VersionDll,VERSION_STAND_ALONE);
		// log de l'information stand alone ou dll
		LogfileF(s.Log,"Fogale Probe version of dll is Stand Alone version");
	}
	else
	{	// mode dll
		strcpy(s.VersionDll,VERSION_OEM);
		// log de l'information stand alone ou dll
		LogfileF(s.Log,"Fogale Probe version of dll is OEM");
	}

	// retour de variable OK
	return FP_OK;
}

/*
int FP_API FPDLLAbout()
{
	char AboutTemporary[FPMAXSTR];
	sprintf(AboutTemporary,"Fogale Nanotech\r\nVersion of Fogale Probe DLL %i\r\nCopyright (C) 2007",FPGetVersion());
	MessageBox(0,AboutTemporary,"About Fogale Probe DLL",0);
	return FP_OK;
}
*/

#define Flag_Log 0
#define Flag_CheckID 2
#define Flag_CheckState 4

int FP_Enter(int ProbeID, char* fct, char* crequieredState, FPSTATE requieredState, int Flag)
{
	if(s.DLL_State==0) return FP_FAIL;
	if(Flag&Flag_CheckID)
	{
		if((ProbeID<=0)||(ProbeID>=s.NumProbe))	{	LogfileF(s.Log,"%s\tProbeID %i: Invalid probe ID (return FP_FAIL)",fct,ProbeID);	return FP_FAIL;	}
		if(Flag&Flag_CheckState)
		{
			FPROBE_STATE* curProbe = &s.Probe[ProbeID];
			if(curProbe->state == fpClosed)	
			{	
				LogfileF(s.Log,"%s\tProbeID %i: Closed probe (return FP_FAIL)",fct,ProbeID);
				return FP_FAIL;	
			}
			//if(curProbe==0)	{	LogfileF(s.Log,"%s\tProbeID %i: Probe not initialized (return FP_FAIL)",fct,ProbeID,curProbe->state,crequieredState,requieredState);	return FP_FAIL;	}//pointeur sur un tableau statique
			if(curProbe->bReentranceFogaleProbe)	
			{	
				LogfileF(s.Log,"%s\tProbeID %i: Attempted to reenter (return FP_BUSY)",fct,ProbeID);	
				return FP_BUSY;	
			} // mutex indisponible alors on retourne une erreur
			if((requieredState!=fpAnyState)&&(curProbe->state != requieredState))
			{	
				LogfileF(s.Log,"%s\tProbeID %i: Incorrect probe sequence state %i (expected %s(%i)) (return FP_FAIL)",fct,ProbeID,curProbe->state,crequieredState,requieredState);	
				return FP_FAIL;	
			}
			curProbe->bReentranceFogaleProbe = true;// on n'autorise plus l'entrée
		}
	}
	if(Flag&Flag_Log) LogfileF(s.Log,"%s\tEnter",fct);
	return FP_OK;
}

int FP_Enternolock(int ProbeID, char* fct, char* crequieredState, FPSTATE requieredState, int Flag)
{
	if(s.DLL_State==0) return FP_FAIL;
	if(Flag&Flag_CheckID)
	{
		if((ProbeID<=0)||(ProbeID>=s.NumProbe))	{	LogfileF(s.Log,"%s\tProbeID %i: Invalid probe ID (return FP_FAIL)",fct,ProbeID);	return FP_FAIL;	}
		if(Flag&Flag_CheckState)
		{
			FPROBE_STATE* curProbe = &s.Probe[ProbeID];
			if(curProbe->state == fpClosed)	
			{	
				LogfileF(s.Log,"%s\tProbeID %i: Closed probe (return FP_FAIL)",fct,ProbeID);
				return FP_FAIL;	
			}
			//if(curProbe==0)	{	LogfileF(s.Log,"%s\tProbeID %i: Probe not initialized (return FP_FAIL)",fct,ProbeID,curProbe->state,crequieredState,requieredState);	return FP_FAIL;	}//pointeur sur un tableau statique
			//if(curProbe->bReentranceFogaleProbe)	{	LogfileF(s.Log,"%s\tProbeID %i: Attempted to reenter (return FP_BUSY)",fct,ProbeID);	return FP_BUSY;	} // mutex indisponible alors on retourne une erreur
			if((requieredState!=fpAnyState)&&(curProbe->state != requieredState))	
			{	
				LogfileF(s.Log,"%s\tProbeID %i: Incorrect probe sequence state %i (expected %s(%i)) (return FP_FAIL)",fct,ProbeID,curProbe->state,crequieredState,requieredState);	
				return FP_FAIL;	
			}
			//curProbe->bReentranceFogaleProbe = true;// on n'autorise plus l'entrée
		}
	}
	if(Flag&Flag_Log) LogfileF(s.Log,"%s\tEnter",fct);
	return FP_OK;
}

int FP_Return(int ProbeID, char* cReturnValue, int iReturnValue, char* fct, int Line, int Flag)
{
	if(Flag&Flag_CheckID)
	{
		if((ProbeID>0)&&(ProbeID<s.NumProbe))
		{
			if(Flag&Flag_CheckState)	
			{
				FPROBE_STATE* curProbe = &s.Probe[ProbeID];
				curProbe->bReentranceFogaleProbe = false;// debloque l'entrée
				if(Flag&Flag_Log) LogfileF(s.Log,"%s\tProbe %i State %i\tReturn code %s(%i) (Line:%i)",fct,ProbeID,curProbe->state,cReturnValue,iReturnValue,Line);
			}
			else	{	if(Flag&Flag_Log) LogfileF(s.Log,"%s\tProbe %i  \tReturn code %s(%i) (Line:%i)",fct,ProbeID,cReturnValue,iReturnValue,Line);	}
		}
		else	{	if(Flag&Flag_Log) LogfileF(s.Log,"%s\tIncorrect Probe %i  \tReturn code %s(%i) (Line:%i)",fct,ProbeID,cReturnValue,iReturnValue,Line);	}
	}
	else	{	if(Flag&Flag_Log) LogfileF(s.Log,"%s\t  \tReturn code %s(%i) (Line:%i)",fct,cReturnValue,iReturnValue,Line);	}
	//CHECK(s.Probe[ProbeID].bReentranceFogaleProbe == true,"Previously missing FP_RETURN",s.Probe[ProbeID].bReentranceFogaleProbe = false)
	return iReturnValue;
}

int FP_Returnnolock(int ProbeID, char* cReturnValue, int iReturnValue, char* fct, int Line, int Flag)
{
	if(Flag&Flag_CheckID)
	{
		if((ProbeID>0)&&(ProbeID<s.NumProbe))
		{
			if(Flag&Flag_CheckState)	
			{
				FPROBE_STATE* curProbe = &s.Probe[ProbeID];
				//curProbe->bReentranceFogaleProbe = false;// debloque l'entrée
				if(Flag&Flag_Log) LogfileF(s.Log,"%s\tProbe %i State %i\tReturn code %s(%i) (Line:%i)",fct,ProbeID,curProbe->state,cReturnValue,iReturnValue,Line);
			}
			else	{	if(Flag&Flag_Log) LogfileF(s.Log,"%s\tProbe %i  \tReturn code %s(%i) (Line:%i)",fct,ProbeID,cReturnValue,iReturnValue,Line);	}
		}
		else	{	if(Flag&Flag_Log) LogfileF(s.Log,"%s\tIncorrect Probe %i  \tReturn code %s(%i) (Line:%i)",fct,ProbeID,cReturnValue,iReturnValue,Line);	}
	}
	else	{	if(Flag&Flag_Log) LogfileF(s.Log,"%s\t  \tReturn code %s(%i) (Line:%i)",fct,cReturnValue,iReturnValue,Line);	}
	//CHECK(s.Probe[ProbeID].bReentranceFogaleProbe == true,"Previously missing FP_RETURN",s.Probe[ProbeID].bReentranceFogaleProbe = false)
	return iReturnValue;
}

/*
void SPG_CONV FPDLLCheckClbk(void* User, char* str)
{
	if(User==0) return;
	FPDLL_STATE& s = *(FPDLL_STATE*)User;
	if(s.Log.Etat) LogfileF(s.Log,"FPDLLCheckClbk : %s",str);

//fdef _DEBUG
	if((s.EnableMsgBox)&&(s.MsgCount<8)) 
	{
		if(MessageBox(0,str,"FogaleProbe : FPDLLCheckClbk",MB_OKCANCEL)!=IDOK) 
		{
#ifdef _DEBUG
			BreakHook();
#endif
		}
		s.MsgCount++;
	}
	return;
}
*/
int FP_API FPDLLInit()
{	
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
	_CrtSetReportMode(_CRT_ERROR, _CRTDBG_MODE_DEBUG);
#ifndef FOGALEPROBE_ASMODULE
	OutputDebugString(fdwstring("DEBUG CHECK START AT FPDLLInit\n"));
	_CrtDumpMemoryLeaks();
	OutputDebugString(fdwstring("DEBUG CHECK STOP AT FPDLLInit\n"));
#endif

	// on définit le fichier
	//char LogPathName[1024];
	//strcpy(LogPathName,"\\Log\\FogaleProbeLog\\");
	//strcat(LogPathName,FP_DLL_NAME);

#ifndef FOGALEPROBE_ASMODULE // lorsque les cpp sont utilisés directement dans un projet (FP_Calibrate) FOGALEPROBE_ASMODULE doit etre défini pour ne pas initialiser deux fois la lib
	SPG_WinMainStart(0,0,0,0,SPG_SM_NoDisplay,0,0,0,0,0);
#endif
	//FDE
	//SPG_WinMainStart(0, 0, 0, 0, SPG_SM_NoDisplay, 0, 0, 0, 0, 0);
	FD_MainStart();

	// création du fichier de log dans le répertoire log
	//LogfileInit(s.Log,LogPathName);

#if 0
	PWSTR wAppPath; 
	SHGetKnownFolderPath(FOLDERID_ProgramData,0,0,&wAppPath); 
	char ansiAppPath[MaxProgDir]; 
	BOOL U;
	WideCharToMultiByte(CP_ACP,0,wAppPath,-1,ansiAppPath,MaxProgDir,"_",&U);
	char ProgName[MaxProgDir]; GetModuleFileName(0,ProgName,MaxProgDir);
	 char* LogPath="\\Log";

	 SPG_SetExtens(ProgName,"");

	 char FinalPath[MaxProgDir];
	 sprintf(FinalPath,"%s\\%s",ansiAppPath,SPG_NameOnly(ProgName));
	 CreateDirectory(FinalPath,0);
	 sprintf(FinalPath,"%s\\%s%s",ansiAppPath,SPG_NameOnly(ProgName),LogPath);
	 CreateDirectory(FinalPath,0);

	 CoTaskMemFree(wAppPath);

	MessageBox(0,FinalPath,"Test of log path",MB_OK);
#else
	char* FinalPath="\\Log\\FogaleProbe";
#endif

	LogfileInit(s.Log,FP_DLL_NAME, LOGWITHDATE|LOGWITHTHREADID|LOGCHECKCLBK, 1024*1024*8, 3,"c:\\temp");
	//SPG_ZeroStruct(s.Log);
	s.DLL_State=FP_OK;
	FP_ENTERnoID();
	char Temp[FPMAXSTR];
	sprintf(Temp,"FogaleProbe Version %i",FOGALEPROBE_VERSION);
	Logfile(s.Log,Temp);
	//memset()
	
	s.NumProbe=1;//la probe 0 existe toujours et reste vide, elle sert à repérer les erreurs de parametres

	//MessageBox(NULL, "DLL Initialization", "FogaleProbe", MB_OK);

	// Initialisation librairie SPG. Le dernier paramètre à 0 permet d'utiliser les double.
	s.iIndexOnSNTable = 0;
	s.iSNDispo = 0;
	FP_RETURNnoID(FP_OK);
}

int FP_API FPDLLClose()
{
	FP_ENTERnoID();

	int RetValue = FP_OK;

	//fermer toutes les probes
	for (int i=0; i<MAX_NB_PROBES; i++)
	{
		if (s.Probe[i].state==fpStarted)
		{
			FPStopSingleShotAcq(i);
		}
		if (s.Probe[i].state!=fpClosed)
		{
			RetValue = FPClose(i);
		}
	}
	Logfile(s.Log, "--1--");
	DbgCHECK(_CrtCheckMemory()==0,"");
	Logfile(s.Log, "--2--");
	LogfileClose(s.Log);  //ne pas faire de log apres la fermeture de la lib
#ifndef FOGALEPROBE_ASMODULE
	SPG_WinMainClose();
#endif
	s.DLL_State=0;
	memset(&s,0,sizeof(s));
#ifndef FOGALEPROBE_ASMODULE
	OutputDebugString(fdwstring("DEBUG CHECK START AT FPDLLClose\n"));
	_CrtDumpMemoryLeaks();
	OutputDebugString(fdwstring("DEBUG CHECK STOP AT FPDLLClose\n"));
#endif
	return RetValue;//FP_RETURNnoID(RetValue);  //ne pas faire de log apres la fermeture de la lib
}

FPDLLEXP int FP_API FPInitialize(DBL_LISE_HCONFIG* HardwareConfigDual, LISE_HCONFIG* HardwareConfigTop, LISE_HCONFIG* HardwareConfigBottom, char* Name, int Param1, int Param2, int Param3)
{
	FP_ENTERnoID();
	// avant position de la mise à jour de Probe ID, à faire que lorsqu'on a passé tous les tests
	SPG_MemFastCheck();
	int ProbeID;
	for(ProbeID=1;ProbeID<s.NumProbe;ProbeID++) //ProbeID zero est reserve pour la probe nulle, de même que s.Probe[0] est la structure probe vide
	{
		if(s.Probe[ProbeID].state==fpClosed) break;
	}
	if(ProbeID==s.NumProbe)
	{
		CHECK(s.NumProbe>=MAX_NB_PROBES,"FPInitialize : Too many open probes\nUse FPClose to close unused probes",FP_RETURNnoID(FP_FAIL))
		s.NumProbe++;
	}

	CHECK(s.Probe[ProbeID].state!=fpClosed,"",FP_RETURN(FP_FAIL));

#ifdef FDE
	GetModuleFileName(GetModuleHandle(FP_DLL_NAME),s.AbsModulePath,FPMAXSTR);
#else
	wchar_t path[FPMAXSTR];
	GetModuleFileName(GetModuleHandle(fdwstring(FP_DLL_NAME)), path, FPMAXSTR);
	strcpy_s(s.AbsModulePath, fdstring(&path[0]));
#endif
	LogfileF(s.Log, "DLL path: %s", s.AbsModulePath);
	SPG_PathOnly(s.AbsModulePath);//chemin de la DLL  ...  qu'on soit en labview compilé ou non  ... 

	// on va mettre les fichiers de log dans un dossier log
	//FDE SPG_ConcatPath(s.AbsLogPath, s.AbsModulePath, "Log\\FogaleProbe\\");
	SPG_ConcatPath(s.AbsLogPath, s.AbsModulePath, "Log\\FogaleProbe\\");
	CreateDirectory(fdwstring(s.AbsLogPath),0);

	if(Param2 == 9972)
	{ // code permettant d'activer le soft en version StandAlone
		FPSetDllVersion(true);
	}
	else
	{ // version OEM par défaut
		FPSetDllVersion(false);
	}

	strcpy(Global.ProgDir,s.AbsModulePath);//stocke le chemin absolu pour la librairie SrcC (B_LoadButtonsLib, C_LoadCaracLib)

	char ParamsPath[FPMAXSTR];
	SPG_ConcatPath(ParamsPath,s.AbsModulePath,ParamsFolder);//chemin du dossier DLL\Params

	if(Name!=FP_USERSELECTEDPROBE)
	{
		SPG_ConcatPath(s.Probe[ProbeID].AbsParamsFile,ParamsPath,Name);//fichier DLL\Params\Name
	}
	/*
	else if(Name==NULL)
	{
		SPG_ConcatPath(s.Probe[ProbeID].AbsParamsFile,ParamsPath,"Probe_Configuration_File.txt");//fichier DLL\Params\Name
	}
	*/
	else
	{
		SPG_ConcatPath(s.Probe[ProbeID].AbsParamsFile,ParamsPath,"Config*.txt");
#ifdef FDE
		CHECK(SPG_GetLoadName(SPG_TXT,s.Probe[ProbeID].AbsParamsFile,FPMAXSTR)==0,"No file selected",FP_RETURN(FP_FAIL));
#else
		FP_RETURN(FP_FAIL);
#endif
	}
	//SPG_SetExtens(s.Probe[ProbeID].AbsParamsFile,".txt");

	char mess[FPMAXSTR];
	sprintf(mess, "Build date:  " __DATE__ ", " __TIME__);
	Logfile(s.Log, mess);
	sprintf(mess, "Params file path: %s", s.Probe[ProbeID].AbsParamsFile);
	Logfile(s.Log, mess);
	sprintf(mess, "Global.ProgDir: %s", Global.ProgDir);
	Logfile(s.Log, mess);

	FPROBE_STATE* curProbe = &s.Probe[ProbeID];

// Ajout pour aller lire le nom du device dans le fichier de config
	SPG_CONFIGFILE CFG; SPG_ZeroStruct(CFG);
	CFG_Init(CFG,0,s.Probe[ProbeID].AbsParamsFile,FPMAXSTR,0);
	char TypeDevice[FPMAXSTR]; memset(TypeDevice,0,FPMAXSTR);

// Definition de l'appareil
	char ParamName[FPMAXSTR]; strcpy(ParamName,"LENSCAN");
	sprintf(ParamName,"DeviceType");
	CFG_StringParam(CFG,ParamName,TypeDevice,"Type of device: LISE_ED, CHROM, CHROM_DOUBLE, LISE_LS, LENSCAN, SPIRO",1);

#ifdef FDE
	// Modif MP22/05/2012: Lise_SD n'existe plus, on l'appelle SPIRO, ici on se protège des fichiers de parametres qui pourraient contenir
	if(strcmp(TypeDevice,"LISE_SD") == 0)
		strcpy(TypeDevice,SPIRO_TYPE);
#endif

	// On copie le nom de produit
	strcpy(s.ProductName,TypeDevice);

	CFG_IntParam(CFG,"EnableMsgBox",&Global.EnableList,"1:always 0:only if no callback defined -1:never",1,CP_INT,-1,1);

	// on ferme le fichier de config
	CFG_Close(CFG);

	// test de clé HASP pour fogale Probe en fonction de la probe

#ifdef	HASP
	bool bHASPSuccess = TestHaspVersion(s.ProductName,s.VersionDll);
#endif

	SPG_MemFastCheck();

	// Lire le type de sonde.
    if (strcmp(Name, "") == 0)
    {
        strcpy(TypeDevice, "LISE_ED_DOUBLE"); //ANALYSE: Force value to LISE_ED_DOUBLE when no config txt given
    }

	if(strcmp(TypeDevice,"LISE_ED") == 0)
	{
		curProbe->Type = fpLiseED;
		LogfileF(s.Log, "Configuration file for Fogale LISE ED.");
	}
	else if(strcmp(TypeDevice,"LISE_ED_DOUBLE") == 0)
	{
		curProbe->Type = fpLiseEDDouble;
		LogfileF(s.Log, "Configuration file for Fogale Double Lise Ed.");
	}
	else if(strcmp(TypeDevice,"LISE_ED_EXT") == 0)
	{
		curProbe->Type = fpLiseEDExtended;
		LogfileF(s.Log, "Configuration file for Fogale Lise Ed Extended.");
	}
	else if(strcmp(TypeDevice,"LISE_LS")==0)
	{
		curProbe->Type = fpLiseLS;
		LogfileF(s.Log,"Configuration file for Fogale LISE LS.");
	}
	else if(strcmp(TypeDevice,"CHROM") == 0)
	{
		curProbe->Type = fpChrom;
		LogfileF(s.Log, "Configuration file for Fogale Chromatic.");
	}
	else if(strcmp(TypeDevice,"CHROM_DOUBLE") == 0)
	{
		curProbe->Type = fpChromDouble;
		LogfileF(s.Log, "Configuration file for Fogale Double Chromatic.");
	}
#ifdef SPG_General_USESTIL
	else if(strcmp(TypeDevice,CCS_PRIMA_TYPE) == 0)
	{
		curProbe->Type = fpCCS_PRIMA;
		LogfileF(s.Log, "Configuration file for STIL CCS PRIMA.");
	}
	else if(strcmp(TypeDevice,STIL_DUO_TYPE) == 0)
	{
		curProbe->Type = fpSTIL_DUO;
		LogfileF(s.Log, "Configuration file for STIL DUO.");
	}
#endif
#ifdef FDE
	else if(strcmp(TypeDevice,SPIRO_TYPE) == 0)
	{
		curProbe->Type = fpSPIRO;
		LogfileF(s.Log, "Configuration file for SPIRO probe.");
	}
#endif
	else if(strcmp(TypeDevice,"LENSCAN") == 0 || strcmp(TypeDevice,"DEEPROBE") == 0)
	{
		curProbe->Type = fpLenScan;
		if(strcmp(TypeDevice,"LENSCAN") == 0) LogfileF(s.Log, "Configuration file for Fogale LENSCAN.");
		else LogfileF(s.Log, "Configuration file for Fogale DEEPROBE.");
	}
	else
	{
		// sortir de l'init avec une erreur...
		LogfileF(s.Log, "Error in initialisation, can't find parameter file / device type field");
		FP_RETURNnoID(FP_FAIL);
	}

	//curProbe->Type = fpLiseED;
	int NumberSN = 0;

#ifdef FDE 
	if (curProbe->Type == fpLiseED)
	{
		curProbe->FProbeInit = LEDIInit;
		curProbe->FPProbeLampState = NULL;
		curProbe->FProbeClose = LEDIClose;
		curProbe->FProbeStart = LEDIAcqStart;
		curProbe->FProbeStop = LEDIAcqStop;
		curProbe->FProbeDefineSample = LEDIDefineSample;
		curProbe->FProbeGetThickness = LEDIGetThickness;
		curProbe->FProbeGetThicknesses = LEDIGetThicknesses;
		curProbe->FProbeSetStagePositionInfo = LEDISetStagePositionInfo;

		curProbe->FPOpenSettingsWindow = LEDIOpenSettingsWindow;
		curProbe->FPUpdateSettingsWindow = LEDIUpdateSettingsWindow;
		curProbe->FPCloseSettingsWindow = LEDICloseSettingsWindow;
		curProbe->FPGetSystemCaps = LEDIGetSystemCaps;
		curProbe->FPGetRawSignal = LEDIGetRawSignal;
		curProbe->FPCalibrateDark = LEDICalibrateDark;
		curProbe->FPCalibrateThickness = LEDICalibrateThickness;

		// Modification des modes d'acsuisition
		curProbe->FProbeStartSingleShot = LEDIStartSingleShotAcq;
		curProbe->FProbeStartContinuous = LEDIStartContinuousAcq;
		// Ma 23/01/2009 : fin de modif

		curProbe->FProbeGetParam = LEDIGetParam;
		curProbe->FProbeSetParam = LEDISetParam;

		NumberSN = 1;
	}
	else
#endif //FDE 
		if(curProbe->Type == fpLiseEDDouble)
	{
		curProbe->FProbeInit = DBL_LEDIInit;
		curProbe->FPProbeLampState = NULL;
		curProbe->FProbeClose = DBL_LEDIClose;
		curProbe->FProbeStart = DBL_LEDIAcqStart;
		curProbe->FProbeStop = DBL_LEDIAcqStop;
		curProbe->FProbeDefineSample = DBL_LEDIDefineSample;
		curProbe->FProbeGetThickness = DBL_LEDIGetThickness;
		curProbe->FProbeGetThicknesses = DBL_LEDIGetThicknesses;
		curProbe->FProbeSetStagePositionInfo = DBL_LEDISetStagePositionInfo;

#ifdef FDE
		curProbe->FPOpenSettingsWindow = DBL_LEDIOpenSettingsWindow;
		curProbe->FPUpdateSettingsWindow = DBL_LEDIUpdateSettingsWindow;
		curProbe->FPCloseSettingsWindow = DBL_LEDICloseSettingsWindow;
#endif
		curProbe->FPGetSystemCaps = DBL_LEDIGetSystemCaps;
		curProbe->FPGetRawSignal = DBL_LEDIGetRawSignal;
		curProbe->FPCalibrateDark = DBL_LEDICalibrateDark;
		curProbe->FPCalibrateThickness = DBL_LEDICalibrateThickness;

		curProbe->FProbeStartSingleShot = DBL_LEDIStartSingleShotAcq;
		curProbe->FProbeStartContinuous = DBL_LEDIStartContinuousAcq;

		curProbe->FProbeGetParam = DBL_LEDIGetParam;
		curProbe->FProbeSetParam = DBL_LEDISetParam;

		curProbe->FPDefineSampleDouble = DBL_LEDIDefineSampleDouble;
		curProbe->FPGetSystemCapsDouble = DBL_LEDIGetSystemCapsDouble;

		NumberSN = 2;
	}
#ifdef FDE
	else if(curProbe->Type == fpLiseEDExtended)
	{
		curProbe->FProbeInit = EXT_LEDIInit;
		curProbe->FPProbeLampState = NULL;
		curProbe->FProbeClose = EXT_LEDIClose;
		curProbe->FProbeStart = EXT_LEDIAcqStart;
		curProbe->FProbeStop = EXT_LEDIAcqStop;
		curProbe->FProbeDefineSample = EXT_LEDIDefineSample;
		curProbe->FProbeGetThickness = EXT_LEDIGetThickness;
		curProbe->FProbeGetThicknesses = EXT_LEDIGetThicknesses;
		curProbe->FProbeSetStagePositionInfo = EXT_LEDISetStagePositionInfo;

		curProbe->FPOpenSettingsWindow = EXT_LEDIOpenSettingsWindow;
		curProbe->FPUpdateSettingsWindow = EXT_LEDIUpdateSettingsWindow;
		curProbe->FPCloseSettingsWindow = EXT_LEDICloseSettingsWindow;
		curProbe->FPGetSystemCaps = EXT_LEDIGetSystemCaps;
		curProbe->FPGetRawSignal = EXT_LEDIGetRawSignal;
		curProbe->FPCalibrateDark = EXT_LEDICalibrateDark;
		curProbe->FPCalibrateThickness = EXT_LEDICalibrateThickness;

		curProbe->FProbeStartSingleShot = EXT_LEDIStartSingleShotAcq;
		curProbe->FProbeStartContinuous = EXT_LEDIStartContinuousAcq;

		curProbe->FProbeGetParam = EXT_LEDIGetParam;
		curProbe->FProbeSetParam = EXT_LEDISetParam;

		NumberSN = 1;
	}
#endif
#ifdef FDE
	else if(curProbe->Type == fpLiseLS)
	{
		curProbe->FProbeInit = LSLIInit;
		curProbe->FPProbeLampState = NULL;
		curProbe->FProbeClose = LSLIClose;
		curProbe->FProbeStart = LSLIAcqStart;
		curProbe->FProbeStop = LSLIAcqStop;
		curProbe->FProbeDefineSample = LSLIDefineSample;
		curProbe->FProbeGetThickness = LSLIGetThickness;
		curProbe->FProbeGetThicknesses = LSLIGetThicknesses;
		curProbe->FProbeSetStagePositionInfo = LSLISetStagePositionInfo;

		curProbe->FPOpenSettingsWindow = LSLIOpenSettingsWindow;
		curProbe->FPUpdateSettingsWindow = LSLIUpdateSettingsWindow;
		curProbe->FPCloseSettingsWindow = LSLICloseSettingsWindow;
		curProbe->FPGetSystemCaps = LSLIGetSystemCaps;
		curProbe->FPGetRawSignal = LSLIGetRawSignal;
		curProbe->FPCalibrateDark = LSLICalibrateDark;
		curProbe->FPCalibrateThickness = LSLICalibrateThickness;

		curProbe->FProbeStartSingleShot = curProbe->FProbeStart;
		curProbe->FProbeStartContinuous = curProbe->FProbeStart;
		NumberSN = 1;

	}
	else if (curProbe->Type == fpChrom)
	{
		curProbe->FProbeInit = CHRIInit;
		curProbe->FPProbeLampState = NULL;
		curProbe->FProbeClose = CHRIClose;
		curProbe->FProbeStart = CHRIAcqStart;
		curProbe->FProbeStop = CHRIAcqStop;
		curProbe->FProbeDefineSample = CHRIDefineSample;
		curProbe->FProbeGetThickness = CHRIGetThickness;
		curProbe->FProbeGetThicknesses = CHRIGetThicknesses;
		curProbe->FProbeSetStagePositionInfo = CHRISetStagePositionInfo;

		curProbe->FPOpenSettingsWindow = CHRIOpenSettingsWindow;
		curProbe->FPUpdateSettingsWindow = CHRIUpdateSettingsWindow;
		curProbe->FPCloseSettingsWindow = CHRICloseSettingsWindow;
		curProbe->FPGetSystemCaps = CHRIGetSystemCaps;
		curProbe->FPGetRawSignal = CHRIGetRawSignal;
		curProbe->FPCalibrateDark = CHRICalibrateDark;
		curProbe->FPCalibrateThickness = CHRICalibrateThickness;

		curProbe->FProbeGetParam = CHRIGetParam;
		curProbe->FProbeSetParam = CHRISetParam;

		curProbe->FProbeStartSingleShot = curProbe->FProbeStart;
		curProbe->FProbeStartContinuous = curProbe->FProbeStart;
		NumberSN = 1;
	}
	else if (curProbe->Type == fpChromDouble)
	{
		curProbe->FProbeInit = CHRDIInit;
		curProbe->FPProbeLampState = NULL;
		curProbe->FProbeClose = CHRDIClose;
		curProbe->FProbeStart = CHRDIAcqStart;
		curProbe->FProbeStop = CHRDIAcqStop;
		curProbe->FProbeDefineSample = CHRDIDefineSample;
		curProbe->FProbeGetThickness = CHRDIGetThickness;
		curProbe->FProbeGetThicknesses = CHRDIGetThicknesses;
		curProbe->FProbeSetStagePositionInfo = CHRDISetStagePositionInfo;

		curProbe->FPOpenSettingsWindow = CHRDIOpenSettingsWindow;
		curProbe->FPUpdateSettingsWindow = CHRDIUpdateSettingsWindow;
		curProbe->FPCloseSettingsWindow = CHRDICloseSettingsWindow;
		curProbe->FPGetSystemCaps = CHRDIGetSystemCaps;
		curProbe->FPGetRawSignal = CHRDIGetRawSignal;
		curProbe->FPCalibrateDark = CHRDICalibrateDark;
		curProbe->FPCalibrateThickness = CHRDICalibrateThickness;

		curProbe->FProbeStartSingleShot = curProbe->FProbeStart;
		curProbe->FProbeStartContinuous = curProbe->FProbeStart;
		NumberSN = 2;
	}
	else if (curProbe->Type == fpSPIRO)
	{
		curProbe->FProbeInit = SPIRO_Init;
		curProbe->FPProbeLampState = SPIRO_LampState;
		curProbe->FProbeClose = SPIRO_Close;
		curProbe->FProbeStart = SPIRO_AcqStart;
		curProbe->FProbeStop = SPIRO_AcqStop;
		curProbe->FProbeDefineSample = SPIRO_DefineSample;
		curProbe->FProbeGetThickness = SPIRO_GetThickness;
		curProbe->FProbeGetThicknesses = SPIRO_GetMultipleThicknesses;

		curProbe->FProbeSetStagePositionInfo = SPIRO_SetStagePositionInfo;

		curProbe->FPGetSystemCaps = SPIRO_GetSystemCaps;
		curProbe->FPGetRawSignal = SPIRO_GetRawSignal;
		curProbe->FPCalibrateDark = SPIRO_CalibrateDark;
		curProbe->FPCalibrateThickness = SPIRO_CalibrateThickness;

		curProbe->FProbeGetParam = SPIRO_GetParam;
		curProbe->FProbeSetParam = SPIRO_SetParam;

		curProbe->FProbeStartSingleShot = SPIRO_StartSingleShotAcq;
		curProbe->FProbeStartContinuous = SPIRO_StartContinuousAcq;
		NumberSN = 1;
	}
#endif
#ifdef SPG_General_USESTIL
	else if (curProbe->Type == fpCCS_PRIMA)
	{
		curProbe->FProbeInit = STIL_Init;
		curProbe->FPProbeLampState = NULL;
		curProbe->FProbeClose = STIL_Close;
		curProbe->FProbeStart = STIL_AcqStart;
		curProbe->FProbeStop = STIL_AcqStop;
		curProbe->FProbeDefineSample = STIL_DefineSample;
		curProbe->FProbeGetThickness = STIL_GetThickness;
		curProbe->FProbeGetThicknesses = STIL_GetMultipleDistances;

		curProbe->FProbeSetStagePositionInfo = STIL_SetStagePositionInfo;

		curProbe->FPGetSystemCaps = STIL_GetSystemCaps;
		curProbe->FPGetRawSignal = STIL_GetRawSignal;
		curProbe->FPCalibrateDark = STIL_CalibrateDark;
		curProbe->FPCalibrateThickness = STIL_CalibrateThickness;

		curProbe->FProbeGetParam = STIL_GetParam;
		curProbe->FProbeSetParam = STIL_SetParam;

		curProbe->FProbeStartSingleShot = STIL_StartSingleShotAcq;
		curProbe->FProbeStartContinuous = STIL_StartContinuousAcq;
		NumberSN = 1;
	}
	else if (curProbe->Type == fpSTIL_DUO)
	{
		curProbe->FProbeInit = STIL_Init;
		curProbe->FPProbeLampState = NULL;
		curProbe->FProbeClose = STIL_Close;
		curProbe->FProbeStart = STIL_AcqStart;
		curProbe->FProbeStop = STIL_AcqStop;
		curProbe->FProbeDefineSample = STIL_DefineSample;
		curProbe->FProbeGetThickness = STIL_GetThickness;
		curProbe->FProbeGetThicknesses = STIL_GetMultipleDistances;

		curProbe->FProbeSetStagePositionInfo = STIL_SetStagePositionInfo;

		curProbe->FPGetSystemCaps = STIL_GetSystemCaps;
		curProbe->FPGetRawSignal = STIL_GetRawSignal;
		curProbe->FPCalibrateDark = STIL_CalibrateDark;
		curProbe->FPCalibrateThickness = STIL_CalibrateThickness;

		curProbe->FProbeGetParam = STIL_GetParam;
		curProbe->FProbeSetParam = STIL_SetParam;

		curProbe->FProbeStartSingleShot = STIL_StartSingleShotAcq;
		curProbe->FProbeStartContinuous = STIL_StartContinuousAcq;
		NumberSN = 1;
	}
#endif
#ifdef FDE
	else if (curProbe->Type == fpLenScan)
	{
		curProbe->FProbeInit = LenScanInit;
		curProbe->FPProbeLampState = NULL;
		curProbe->FProbeClose = LenScanClose;
		curProbe->FProbeStart = LenScanStart;
		curProbe->FProbeStop = LenScanStop;
		curProbe->FProbeDefineSample = LenScanDefineSample;
		curProbe->FProbeGetThickness = LenScanGetThickness;
		curProbe->FProbeGetThicknesses = LenScanGetThicknesses;

		curProbe->FProbeGetParam = LenScanGetParam;
		curProbe->FProbeSetParam = LenScanSetParam;

		curProbe->FProbeSetStagePositionInfo = LenScanSetPosInfo;

		curProbe->FPOpenSettingsWindow = LenScanOpenSettingsWindow;
		curProbe->FPUpdateSettingsWindow = LenScanUpdateSettingsWindow;
		curProbe->FPCloseSettingsWindow = LenScanCloseSettingsWindow;
		curProbe->FPGetSystemCaps = LenScanGetCaps;
		curProbe->FPGetRawSignal = LenScanGetRaw;
		//curProbe->FPCalibrateDark = LenScanCalibrateDark;
		//curProbe->FPCalibrateThickness = LenScanCalibrateThickness;

		curProbe->FProbeStartSingleShot = curProbe->FProbeStart;
		curProbe->FProbeStartContinuous = curProbe->FProbeStart;
		NumberSN = 1;
	}
#endif
	SPG_MemFastCheck();

	if(s.iSNDispo + NumberSN < MAX_NB_PROBES)
	{
		// Init OK
	}
	else
	{
		Logfile(s.Log, "Maximum probe autorized reached, please close probe to init another probe");
		FP_RETURNnoID(FP_FAIL);
	}

	Logfile(s.Log, "curProbe->FProbeInit");
	int r = curProbe->FProbeInit(&curProbe->FProbeState, HardwareConfigDual, HardwareConfigTop, HardwareConfigBottom, curProbe->AbsParamsFile, &s.Log, Param1, Param2, Param3);
	CHECK(r != FP_OK,"FProbeInit",FP_RETURN(r));

	Logfile(s.Log, "curProbe->FProbeInit OK");

	// Ajout des numéros de série dans la SN table que si on a passé l'initialisation
	// On ne peut le faire avant car on n'a pas les numéros de série disponible dans la structure. Sinon on aurait pu le regourper avec les tests précédents
	bool bSNAlreadyExist = false;
	char sSerialNumber[2][1024];	// Tableau de numéro de série pouvant contenir deux SN pour le double Chrom
	memset(sSerialNumber,0,2048);//plantage assuré dans probeclose à cause des strcpy

	SPG_MemFastCheck();

	if(curProbe->Type == fpLiseED)
	{// si LISE ED
		strcpy(sSerialNumber[0],curProbe->LiseEd.Lise.SerialNumber);
		NumberSN = 1;
	}
	else if(curProbe->Type == fpLiseEDDouble)
	{// si LISE ED Double
		strcpy(sSerialNumber[0],curProbe->DblLiseEd.LiseEd[0].Lise.SerialNumber);
		strcpy(sSerialNumber[1],curProbe->DblLiseEd.LiseEd[1].Lise.SerialNumber);
		NumberSN = 2;
	}
#ifdef FDE
	else if(curProbe->Type == fpLiseEDExtended)
	{// si LISE ED Extended
		strcpy(sSerialNumber[0],curProbe->LiseEd.Lise.SerialNumber);
		EXT_LEDISetParam(&curProbe->FProbeState,s.AbsModulePath,FPID_C_SETABSPATH);
		NumberSN = 1;
	}
#endif
	//else if(curProbe->Type == fpLiseLS)
	//{// si LISE LS
	//	strcpy(sSerialNumber[0],curProbe->LiseLsLi.Lise.SerialNumber);
	//	NumberSN = 1;
	//}
#ifdef FDE
	else if(curProbe->Type == fpChrom)
	{// Si Chromatic
		strcpy(sSerialNumber[0],curProbe->Chrom.SerialNumber);
		NumberSN = 1;
	}
	else if(curProbe->Type == fpChromDouble)
	{// Si Double CHROM
		strcpy(sSerialNumber[0],curProbe->ChromDouble.SerialNumber[0]);
		strcpy(sSerialNumber[1],curProbe->ChromDouble.SerialNumber[1]);
		NumberSN = 2;
	}
	else if(curProbe->Type == fpSPIRO)
	{
		strcpy(sSerialNumber[0],curProbe->SpiroProbe.SerialNumber);
		NumberSN = 1;
	}
#endif
#ifdef SPG_General_USESTIL
	else if(curProbe->Type == fpCCS_PRIMA)
	{// Si CHROM STIL
		strcpy(sSerialNumber[0],curProbe->StilProbe.SerialNumber);
		NumberSN = 1;
	}
	else if(curProbe->Type == fpSTIL_DUO)
	{
		strcpy(sSerialNumber[0],curProbe->StilProbe.SerialNumber);
		NumberSN = 1;
	}
#endif
	//else if(curProbe->Type == fpLenScan)
	//{// Si Lenscan
	//	strcpy(sSerialNumber[0],curProbe->LS.SerialNumber);
	//	NumberSN = 1;
	//}
	SPG_MemFastCheck();

	for(int i = 0;i<MAX_NB_PROBES;i++)
	{
		for(int j = 0;j<NumberSN;j++)
		{
			if(strcmp(sSerialNumber[j],s.sSerialNumberTable[i]) == 0)
			{
				bSNAlreadyExist = true;
				break;
			}
		}
		if(bSNAlreadyExist == true)
		{
			break;
		}
	}

	if(bSNAlreadyExist == true)
	{
		Logfile(s.Log, "A device already exist with this Serial Number");
		FP_RETURN(FP_FAIL);
	}
	else
	{
		int j = 0;
		for(int i = 0;i<MAX_NB_PROBES;i++)
		{
			if(strcmp(s.sSerialNumberTable[i],"") == 0)
			{
				strcpy(s.sSerialNumberTable[s.iIndexOnSNTable],sSerialNumber[j]);
				strcpy(s.Probe[ProbeID].sSerialNumber[j],sSerialNumber[j]);
				if(j>NumberSN)
				{
					break;
				}
				else
				{
					j++;
				}
			}
		}
	}
	
	// Machine d'état de la sonde
	curProbe->state = fpStopped;

	// Mode de paramétrage
	curProbe->setting = false;

	// création d'un Mutex à associer à la sonde
	//curProbe->Mutex = CreateMutex(NULL,FALSE,NULL);
	curProbe->bReentranceFogaleProbe = false;

	SPG_MemFastCheck();

#ifdef HASP
	// le test de clé HASP n'a pass marché, il faut fermer le soft
	if(!bHASPSuccess)
	{
		// Message box pour expliquer que le dongle n'a pas été détecté
		MessageBox(0,"HASP dongle not present or expired","HASP error",0);
		// close de la probe courante
		FPClose(ProbeID);
		// on ferme la dll
		FPDLLClose();
		// on sort du programme
		exit(0);
	}
#endif

	FP_RETURN(ProbeID);
}

FPDLLEXP int FP_API FPLampState(int ProbeID, int *itSate)
{
	FP_ENTERnolock(fpAnyState);

	if	(s.Probe[ProbeID].FPProbeLampState == 0) 
		FP_RETURN(FP_UNAVAILABLE);

	*itSate = s.Probe[ProbeID].FPProbeLampState(&s.Probe[ProbeID].FProbeState);

	FP_RETURNnolock(*itSate);
}

// Close probe
FPDLLEXP int FP_API FPClose(int ProbeID)
{
	FP_ENTER(fpStopped);

	// opération pour permettre la réinitialisation d'un boitier fermé, on efface le numéro de série dans la 
	char sTemp[1024];
	int j = 0;
	for(j = 0; j<2;j++)
	{
		strcpy(sTemp,s.Probe[ProbeID].sSerialNumber[j]);
	}

	for(int i = 0;i<MAX_NB_PROBES;i++)
	{
		for(j = 0;j<2;j++)
		{
			if(strcmp(s.Probe[ProbeID].sSerialNumber[j],s.sSerialNumberTable[i]) == 0)
			{
				strcpy(s.sSerialNumberTable[i],"");
				s.iSNDispo--;
			}
		}
	}

	int ProbeCloseReturnValue;
	PROBE_CALL(ProbeCloseReturnValue,FProbeClose);
	// Machine d'état de la sonde
	s.Probe[ProbeID].state = fpClosed;
	memset(s.Probe+ProbeID,0,sizeof(*s.Probe));

	FP_RETURNnoID(ProbeCloseReturnValue);
}

//										#### FP MEASUREMENT FUNCTIONS ####

// Begin single shot acquisition and process
FPDLLEXP int FP_API FPStartSingleShotAcq(int ProbeID)
{
	FP_ENTER(fpStopped);

#ifdef HASP
	// test de clé HASP pour fogale Probe en fonction de la probe
	bool bHASPSuccess = TestHaspVersion(s.ProductName,s.VersionDll);

	// le test de clé HASP n'a pass marché, il faut fermer le soft
	if(!bHASPSuccess)
	{
		FPClose(ProbeID);
		// on ferme la dll
		FPDLLClose();
		// on sort du programme
		exit(0);
	}
#endif

	// 20080227 Modification pour permettre la sauvegarde des fichiers à chaque Start
	FPROBE_STATE* curProbe = &s.Probe[ProbeID];

	int r;

#ifdef FDE
	if (curProbe->Type == fpLiseED)
	{
		LEDISavePeaks(&curProbe->LiseEd,"Peaks.txt");
		LEDIAcqSaveThickness(&curProbe->LiseEd, "Thickness.txt");
		PROBE_CALL(r,FProbeStartSingleShot);
	}
	else
#endif
		if (curProbe->Type == fpLiseEDDouble)
	{
		PROBE_CALL(r,FProbeStartSingleShot);
	}
#ifdef FDE
	else if(curProbe->Type == fpLiseLS)
	{
		LSLISavePeaks(&curProbe->LiseLsLi,"Peaks.txt");
		LSLIAcqSaveThickness(&curProbe->LiseLsLi, "Thickness.txt");
		PROBE_CALL(r,FProbeStart);
	}
	else if(curProbe->Type == fpChromDouble || curProbe->Type == fpChrom)
	{
		PROBE_CALL(r,FProbeStart);
	}
	else if(curProbe->Type == fpCCS_PRIMA)
	{// Si CHROM STIL
		PROBE_CALL(r,FProbeStart);
	}
	else if(curProbe->Type == fpSTIL_DUO)
	{// Si CHROM STIL
		PROBE_CALL(r,FProbeStart);
	}
	else if(curProbe->Type == fpSPIRO)
	{
		PROBE_CALL(r,FProbeStartSingleShot);
	}
#endif //FDE
	else
	{
		PROBE_CALL(r,FProbeStart);
	}
	// Machine d'état de la sonde
	s.Probe[ProbeID].state = fpStarted;
	FP_RETURN(r);
}

// Stop single shot acquisition and process
FPDLLEXP int FP_API FPStopSingleShotAcq(int ProbeID)
{
	FP_ENTER(fpStarted);

	CHECK(s.Probe[ProbeID].setting == true,"Close setting window before changing acquisition state",FP_RETURN(FP_FAIL));

	int r;
	PROBE_CALL(r,FProbeStop);
	// Machine d'état de la sonde
	s.Probe[ProbeID].state = fpStopped;
	FP_RETURN(r);
}

// Begin continuous acquisition and process
FPDLLEXP int FP_API FPStartContinuousAcq(int ProbeID)
{
	FP_ENTER(fpStopped);

	int r=FP_FAIL;

#ifdef HASP
	// test de clé HASP pour fogale Probe en fonction de la probe
	bool bHASPSuccess = TestHaspVersion(s.ProductName,s.VersionDll);

	// le test de clé HASP n'a pass marché, il faut fermer le soft
	if(!bHASPSuccess)
	{
		FPClose(ProbeID);
		// on ferme la dll
		FPDLLClose();
		// on sort du programme
		exit(0);
	}
#endif

	//PROBE_CALL(r,FProbeStart);
	// 20080227 Modification pour permettre la sauvegarde des fichiers à chaque Start
	FPROBE_STATE* curProbe = &s.Probe[ProbeID];

#ifdef FDE
	if (curProbe->Type == fpLiseED)
	{
		LEDISavePeaks(&curProbe->LiseEd,"Peaks.txt");
		LEDIAcqSaveThickness(&curProbe->LiseEd, "Thickness.txt");
		PROBE_CALL(r,FProbeStartContinuous);
	}
	else 
#endif //FDE
		if (curProbe->Type == fpLiseEDDouble)
	{
		PROBE_CALL(r,FProbeStartContinuous);
	}
#ifdef FDE
	else if(curProbe->Type == fpLiseLS)
	{
		LSLISavePeaks(&curProbe->LiseLsLi,"Peaks.txt");
		LSLIAcqSaveThickness(&curProbe->LiseLsLi, "Thickness.txt");
	}
	else if(curProbe->Type == fpChromDouble || curProbe->Type == fpChrom)
	{
		PROBE_CALL(r,FProbeStart);
	}
	else if(curProbe->Type == fpCCS_PRIMA)
	{
		PROBE_CALL(r,FProbeStart);
	}
	else if(curProbe->Type == fpSTIL_DUO)
	{
		PROBE_CALL(r,FProbeStart);
	}
	else if(curProbe->Type == fpSPIRO)
	{
		PROBE_CALL(r,FProbeStartContinuous);
	}
#endif //FDE
	else
	{
		PROBE_CALL(r,FProbeStart);
	}
	// Machine d'état de la sonde
	s.Probe[ProbeID].state = fpStarted;
	
	FP_RETURN(r);
}

// Stop continuous acquisition and process
FPDLLEXP int FP_API FPStopContinuousAcq(int ProbeID)
{
	FP_ENTER(fpStarted);

	CHECK(s.Probe[ProbeID].setting == true,"Close setting window before changing acquisition state",FP_RETURN(FP_FAIL));

	int r;

	PROBE_CALL(r,FProbeStop);
	// Machine d'état de la sonde
	s.Probe[ProbeID].state = fpStopped;

	FP_RETURN(r);
}
FPDLLEXP int FP_API FPDefineSample(int ProbeID, char* Name, char* SampleNumber, double* Thickness, double* Tolerance, double* Index, double* Type, int NbThickness, double Gain, double QualityThreshold)
{
	FP_ENTERnolock(fpAnyState);

    if (Flag_Log) {
	    // Log de la définition de l'échantillon
	    LogfileF(s.Log, "Sample name: %s", Name);
	    LogfileF(s.Log, "Sample number: %s", SampleNumber);
	    int i;
	    for (i=0; i<NbThickness; i++)
	    {
		    LogfileF(s.Log, "Layer %i: thickness=%fµm; tolerance=%fµm; index=%f; type=%f", i, Thickness[i], Tolerance[i], Index[i], Type[i]);
	    }
    }

	int r = s.Probe[ProbeID].FProbeDefineSample(&s.Probe[ProbeID].FProbeState, Name, SampleNumber, Thickness, Tolerance, Index, Type, NbThickness, Gain, QualityThreshold);

	FP_RETURNnolock(r);
}

// Get thickness
FPDLLEXP int FP_API FPGetThickness(int ProbeID, double* Thickness, double* Quality , int _iNumThickness)
{
	FP_ENTERnolock(fpAnyState);//AnyState autorise quand même le recompute pour lenscan

	CHECK(
		(s.Probe[ProbeID].Type != fpLenScan) &&
		(s.Probe[ProbeID].state!=fpStarted),
		"Probe Acquisition must be started before calling GetThickness",
		FP_RETURN(FP_FAIL));

	CHECK(Thickness==0,"FPGetThickness",FP_RETURN(FP_INVALIDPARAM)); 

	// appel de la fonction GetThickness correspondant de la dll
	TRY_BEGIN
	int r = s.Probe[ProbeID].FProbeGetThickness(&s.Probe[ProbeID].FProbeState, Thickness, Quality, _iNumThickness);

	FP_RETURNnolock(r);

	TRY_ENDGRZ("s.Probe[ProbeID].FProbeGetThickness internal error");
}
// Get thicknesses
FPDLLEXP int FP_API FPGetThicknesses(int ProbeID, double* Dates, double* Thicknesses, double* Quality,int NumValues)
{
	FP_ENTERnolock(fpAnyState);//AnyState autorise quand même le recompute pour lenscan

	CHECK(
		(s.Probe[ProbeID].Type != fpLenScan) &&
		(s.Probe[ProbeID].state!=fpStarted),
		"Probe Acquisition must be started before calling GetThicknesses",
		FP_RETURN(FP_FAIL));

	CHECK(Thicknesses==0,"FPGetThicknesses",FP_RETURN(FP_INVALIDPARAM)); 

	// appel de la fonction GetThicknesses correspondant de la dll
	TRY_BEGIN

	// appel de la fonction Getthicknesses correspondant de la dll
	int r = s.Probe[ProbeID].FProbeGetThicknesses(&s.Probe[ProbeID].FProbeState,Dates, Thicknesses, Quality,NumValues);
	FP_RETURNnolock(r);

	TRY_ENDGRZ("s.Probe[ProbeID].FProbeGetThicknesses internal error");
}
//									#### FP MEASUREMENT FUNCTIONS ####







//									#### FP PARAMETERS FUNCTIONS ####


FPDLLEXP int FP_API FPGetParam(int ProbeID, void* Param, int ParamID)
{
	FP_ENTERnolock(fpAnyState);

	if(s.Probe[ProbeID].FProbeGetParam==0) FP_RETURN(FP_UNAVAILABLE);
	int r = s.Probe[ProbeID].FProbeGetParam(&s.Probe[ProbeID].FProbeState, Param, ParamID);

	FP_RETURNnolock(r);
}

FPDLLEXP int FP_API FPSetParam(int ProbeID, void* Param, int ParamID)
{
	FP_ENTERnolock(fpAnyState);

	if(s.Probe[ProbeID].FProbeSetParam==0) FP_RETURN(FP_UNAVAILABLE);
	int r = s.Probe[ProbeID].FProbeSetParam(&s.Probe[ProbeID].FProbeState, Param, ParamID);

	FP_RETURNnolock(r);
}

// Open setting window - modal - and Close window when finished
FPDLLEXP int FP_API FPDoSettings(int ProbeID)
{
	FP_ENTER(fpStopped);

	int r;
	Logfile(s.Log, "Calling FPOpenSettingWindow");
	r = s.Probe[ProbeID].FPOpenSettingsWindow(&s.Probe[ProbeID].FProbeState, 1);
	if(r == FP_OK)
	{
		Logfile(s.Log, "FPOpenSettingWindow Success");
	}
	else
	{
		Logfile(s.Log, "FPOpenSettingWindow Fail");
	}
/*	
	if(s.Probe[ProbeID].Type == fpLiseED) //&&(GetAsyncKeyState(VK_SHIFT)==0))
	{
		LISE_ED &Lise=*(LISE_ED*)&s.Probe[ProbeID].FProbeState;
		LISE_ED_SETTINGS& LSt=Lise.LSettings;
		B_CreateVReglageNumeric(LSt.BL,LSt.EButtons,LSt.CL,30,20,G_SizeY(LSt.EButtons)-40,4,&LSt.fVolts,0.05,0,10);
		B_RedrawButtonsLib(LSt.BL,0);
	}
*/	
	// 20080227 Modification pour permettre la sauvegarde des fichiers à chaque Start
	FPROBE_STATE* curProbe = &s.Probe[ProbeID];
	if (curProbe->Type == fpLiseED)
	{
		LEDISavePeaks(&curProbe->LiseEd,"Peaks.txt");
		LEDIAcqSaveThickness(&curProbe->LiseEd, "Thickness.txt");
	}

	while((r==FP_OK)&&(GetAsyncKeyState(VK_ESCAPE)==0))
	{
		PROBE_CALL(r,FPUpdateSettingsWindow);
		CHECK(r == FP_FAIL,"FPUpdateSettingsWindow", FPCloseSettingsWindow(ProbeID); FP_RETURN(FP_FAIL));// Si jamais l'update ne se fait pas convenablement, retour de DLL_FAIL, on sort de la fonction avec une erreur.
		//FDE DoEvents(SPG_DOEV_READ_WIN_EVENTS);
	}
	PROBE_CALL(r,FPCloseSettingsWindow);

	FP_RETURN(FP_OK);
}

FPDLLEXP int FP_API FPOpenSettingsWindow(int ProbeID)
{
	FP_ENTER(fpStopped);

	FPROBE_STATE* curProbe = &s.Probe[ProbeID];
	if (curProbe->Type == fpLiseED)
	{
		LEDISavePeaks(&curProbe->LiseEd,"Peaks.txt");
		LEDIAcqSaveThickness(&curProbe->LiseEd, "Thickness.txt");
	}
	
	int r = s.Probe[ProbeID].FPOpenSettingsWindow(&s.Probe[ProbeID].FProbeState, 0);
	//s.Probe[ProbeID].state = fpSettings;
	// OpenSettingWindow démarre l'acquisition
	s.Probe[ProbeID].state = fpStarted;
	// On autorise le setting, on met l'etat a true
	s.Probe[ProbeID].setting = true;

	FP_RETURN(r);
}

FPDLLEXP int FP_API FPUpdateSettingsWindow(int ProbeID)
{
	FP_ENTER(fpStarted);

	CHECK(s.Probe[ProbeID].setting == true,"Open setting window first",FP_RETURN(FP_FAIL));

	int r = s.Probe[ProbeID].FPUpdateSettingsWindow(&s.Probe[ProbeID].FProbeState);
	FP_RETURN(r);
}

FPDLLEXP int FP_API FPCloseSettingsWindow(int ProbeID)
{
	FP_ENTER(fpStarted);
	CHECK(s.Probe[ProbeID].setting == true,"Open setting window first",FP_RETURN(FP_FAIL));

	int r = s.Probe[ProbeID].FPCloseSettingsWindow(&s.Probe[ProbeID].FProbeState);
	// CloseSettingsWindow arrête l'acquisition.
	s.Probe[ProbeID].state = fpStopped;
	s.Probe[ProbeID].setting = false;
	FP_RETURN(r);
}

//										#### FP PARAMETERS FUNCTIONS ####
