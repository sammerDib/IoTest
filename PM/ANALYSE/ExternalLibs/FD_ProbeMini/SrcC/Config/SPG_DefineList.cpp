
#include "..\SPG_General.h"

#include "..\SPG_MinIncludes.h"

#include <string.h>
#include <stdio.h>

typedef struct
{
	int Etat;
	FILE* F;
} SPG_CS;

typedef struct
{
	int Etat;
	int StrLen;
	char* C;
} SPG_CC;


void SPG_CONV SPG_Config_Callback(SPG_CS& C, char* Msg, int Defined, char* Value)
{
	if(C.Etat)
	{
		/*
		fwrite(Msg,strlen(Msg),1,C.F);
		if(Etat)
		{
			fwrite(" defined\n",strlen(" defined\n"),1,C.F);
		}
		else
		{
			fwrite(" not defined\n",strlen(" not defined\n"),1,C.F);
		}
		*/
		fwrite(Msg,strlen(Msg),1,C.F);
		fwrite("\t",1,1,C.F);
		if(Defined)
		{
			fwrite("1",1,1,C.F);
		}
		else
		{
			fwrite("0",1,1,C.F);
		}
		fwrite("\t",1,1,C.F);
		fwrite(Value,strlen(Value),1,C.F);
		fwrite("\r\n",2,1,C.F);
	}
	return;
}

void SPG_CONV SPG_Config_Save(char* Filename)
{
	SPG_CS C;
	memset(&C,0,sizeof(SPG_CS));
	C.F=fopen(Filename,"wb");
	if(C.F)
	{
		C.Etat=1;
		SPG_Config_EnumDefines((SPG_DEFINELIST_CALLBACK)SPG_Config_Callback,&C);
		fclose(C.F);
	}
	C.Etat=0;
	memset(&C,0,sizeof(SPG_CS));
	return;
}

void SPG_CONV SPG_Config_StrCallback(SPG_CC& C, char* Msg, int Defined, char* Value)
{
	if(C.Etat)
	{
		if(C.StrLen>strlen(Msg)+strlen(Value)+5)
		{
			sprintf(C.C,"%s\t%i\t%s\r\n",Msg,Defined,Value);
			int l=strlen(C.C);
			C.StrLen-=l;
			C.C+=l;
		}
	}
	return;
}

void SPG_CONV SPG_Config_WriteToString(char* Msg, int MaxLen)
{
	CHECK(Msg==0,"SPG_Config_WriteToString",return);
	SPG_CC C;
	memset(&C,0,sizeof(SPG_CC));
	C.StrLen=MaxLen;
	C.C=Msg;
	C.Etat=1;
	SPG_Config_EnumDefines((SPG_DEFINELIST_CALLBACK)SPG_Config_StrCallback,&C);
	C.Etat=0;
	memset(&C,0,sizeof(SPG_CC));
	return;
}

void SPG_CONV SPG_Config_EnumDefines(SPG_DEFINELIST_CALLBACK SPG_DisplayMacroValue, void* DMV)
{

#define MakeString(U) KMakeString((U))
#define KMakeString(P) #P

//respecter l'ordre alphabetique pour eviter les doublons

#ifdef _WINDOWS_
SPG_DisplayMacroValue(DMV,"_WINDOWS_",1,MakeString(_WINDOWS_));
#else
SPG_DisplayMacroValue(DMV,"_WINDOWS_",0,"");
#endif
#ifdef B_Lib
SPG_DisplayMacroValue(DMV,"B_Lib",1,MakeString(B_Lib));
#else
SPG_DisplayMacroValue(DMV,"B_Lib",0,"");
#endif
#ifdef CHECK
SPG_DisplayMacroValue(DMV,"CHECK(Cond,Msg,Ret)",1,MakeString(CHECK(Cond,Msg,Ret)));
#else
SPG_DisplayMacroValue(DMV,"CHECK(Cond,Msg,Ret)",0,"");
#endif
#ifdef CHECKFLOAT
SPG_DisplayMacroValue(DMV,"CHECKFLOAT(FloatRes,Msg)",1,MakeString(CHECKFLOAT(FloatRes,Msg)));
#else
SPG_DisplayMacroValue(DMV,"CHECKFLOAT(FloatRes,Msg)",0,"");
#endif
#ifdef DbgCHECK
SPG_DisplayMacroValue(DMV,"DbgCHECK(Cond,Msg)",1,MakeString(DbgCHECK(Cond,Msg)));
#else
SPG_DisplayMacroValue(DMV,"DbgCHECK(Cond,Msg)",0,"");
#endif
#ifdef SPG_COMPANYNAME
SPG_DisplayMacroValue(DMV,"SPG_COMPANYNAME",1,MakeString(SPG_COMPANYNAME));
#else
SPG_DisplayMacroValue(DMV,"SPG_COMPANYNAME",0,"");
#endif
#ifdef SPG_DEBUGCONFIG
SPG_DisplayMacroValue(DMV,"SPG_DEBUGCONFIG",1,MakeString(SPG_DEBUGCONFIG));
#else
SPG_DisplayMacroValue(DMV,"SPG_DEBUGCONFIG",0,"");
#endif
#ifdef SPG_RELEASECONFIG
SPG_DisplayMacroValue(DMV,"SPG_RELEASECONFIG",1,MakeString(SPG_RELEASECONFIG));
#else
SPG_DisplayMacroValue(DMV,"SPG_RELEASECONFIG",0,"");
#endif
#ifdef DebugFloat
SPG_DisplayMacroValue(DMV,"DebugFloat",1,MakeString(DebugFloat));
#else
SPG_DisplayMacroValue(DMV,"DebugFloat",0,"");
#endif
#ifdef DebugFloatHard
SPG_DisplayMacroValue(DMV,"DebugFloatHard",1,MakeString(DebugFloatHard));
#else
SPG_DisplayMacroValue(DMV,"DebugFloatHard",0,"");
#endif
#ifdef DebugGraphics
SPG_DisplayMacroValue(DMV,"DebugGraphics",1,MakeString(DebugGraphics));
#else
SPG_DisplayMacroValue(DMV,"DebugGraphics",0,"");
#endif
#ifdef DebugGraphicsTimer
SPG_DisplayMacroValue(DMV,"DebugGraphicsTimer",1,MakeString(DebugGraphicsTimer));
#else
SPG_DisplayMacroValue(DMV,"DebugGraphicsTimer",0,"");
#endif
#ifdef DebugList
SPG_DisplayMacroValue(DMV,"DebugList",1,MakeString(DebugList));
#else
SPG_DisplayMacroValue(DMV,"DebugList",0,"");
#endif
#ifdef DebugLogTime
SPG_DisplayMacroValue(DMV,"DebugLogTime",1,MakeString(DebugLogTime));
#else
SPG_DisplayMacroValue(DMV,"DebugLogTime",0,"");
#endif
#ifdef DebugMeca
SPG_DisplayMacroValue(DMV,"DebugMeca",1,MakeString(DebugMeca));
#else
SPG_DisplayMacroValue(DMV,"DebugMeca",0,"");
#endif
#ifdef DebugMelinkTimer
SPG_DisplayMacroValue(DMV,"DebugMelinkTimer",1,MakeString(DebugMelinkTimer));
#else
SPG_DisplayMacroValue(DMV,"DebugMelinkTimer",0,"");
#endif
#ifdef DebugMem
SPG_DisplayMacroValue(DMV,"DebugMem",1,MakeString(DebugMem));
#else
SPG_DisplayMacroValue(DMV,"DebugMem",0,"");
#endif
#ifdef DebugNetwork
SPG_DisplayMacroValue(DMV,"DebugNetwork",1,MakeString(DebugNetwork));
#else
SPG_DisplayMacroValue(DMV,"DebugNetwork",0,"");
#endif
#ifdef DebugNetworkTimer
SPG_DisplayMacroValue(DMV,"DebugNetworkTimer",1,MakeString(DebugNetworkTimer));
#else
SPG_DisplayMacroValue(DMV,"DebugNetworkTimer",0,"");
#endif
#ifdef DebugPeakDet2D
SPG_DisplayMacroValue(DMV,"DebugPeakDet2D",1,MakeString(DebugPeakDet2D));
#else
SPG_DisplayMacroValue(DMV,"DebugPeakDet2D",0,"");
#endif
#ifdef DebugProfil3DTimer
SPG_DisplayMacroValue(DMV,"DebugProfil3DTimer",1,MakeString(DebugProfil3DTimer));
#else
SPG_DisplayMacroValue(DMV,"DebugProfil3DTimer",0,"");
#endif
#ifdef DebugProfilManagerTimer
SPG_DisplayMacroValue(DMV,"DebugProfilManagerTimer",1,MakeString(DebugProfilManagerTimer));
#else
SPG_DisplayMacroValue(DMV,"DebugProfilManagerTimer",0,"");
#endif
#ifdef SPG_General_USEWLTG_GLOBAL
SPG_DisplayMacroValue(DMV,"SPG_General_USEWLTG_GLOBAL",1,MakeString(SPG_General_USEWLTG_GLOBAL));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEWLTG_GLOBAL",0,"");
#endif
#ifdef DebugProgPrincipalTimer
SPG_DisplayMacroValue(DMV,"DebugProgPrincipalTimer",1,MakeString(DebugProgPrincipalTimer));
#else
SPG_DisplayMacroValue(DMV,"DebugProgPrincipalTimer",0,"");
#endif
#ifdef DebugRender
SPG_DisplayMacroValue(DMV,"DebugRender",1,MakeString(DebugRender));
#else
SPG_DisplayMacroValue(DMV,"DebugRender",0,"");
#endif
#ifdef DebugRenderTimer
SPG_DisplayMacroValue(DMV,"DebugRenderTimer",1,MakeString(DebugRenderTimer));
#else
SPG_DisplayMacroValue(DMV,"DebugRenderTimer",0,"");
#endif
#ifdef DebugSGRAPH
SPG_DisplayMacroValue(DMV,"DebugSGRAPH",1,MakeString(DebugSGRAPH));
#else
SPG_DisplayMacroValue(DMV,"DebugSGRAPH",0,"");
#endif
#ifdef DebugTimer
SPG_DisplayMacroValue(DMV,"DebugTimer",1,MakeString(DebugTimer));
#else
SPG_DisplayMacroValue(DMV,"DebugTimer",0,"");
#endif
#ifdef DF_CALIBRATIONSCALE
SPG_DisplayMacroValue(DMV,"DF_CALIBRATIONSCALE",1,MakeString(DF_CALIBRATIONSCALE));
#else
SPG_DisplayMacroValue(DMV,"DF_CALIBRATIONSCALE",0,"");
#endif
#ifdef DF_FASTEST
SPG_DisplayMacroValue(DMV,"DF_FASTEST",1,MakeString(DF_FASTEST));
#else
SPG_DisplayMacroValue(DMV,"DF_FASTEST",0,"");
#endif
#ifdef DF_NOCALIBR
SPG_DisplayMacroValue(DMV,"DF_NOCALIBR",1,MakeString(DF_NOCALIBR));
#else
SPG_DisplayMacroValue(DMV,"DF_NOCALIBR",0,"");
#endif
#ifdef DF_PREFILTER
SPG_DisplayMacroValue(DMV,"DF_PREFILTER",1,MakeString(DF_PREFILTER));
#else
SPG_DisplayMacroValue(DMV,"DF_PREFILTER",0,"");
#endif
#ifdef DF_SAVECALIBR
SPG_DisplayMacroValue(DMV,"DF_SAVECALIBR",1,MakeString(DF_SAVECALIBR));
#else
SPG_DisplayMacroValue(DMV,"DF_SAVECALIBR",0,"");
#endif
#ifdef DF_SAVEGRADINFO
SPG_DisplayMacroValue(DMV,"DF_SAVEGRADINFO",1,MakeString(DF_SAVEGRADINFO));
#else
SPG_DisplayMacroValue(DMV,"DF_SAVEGRADINFO",0,"");
#endif
#ifdef DF_SAVEKERNEL
SPG_DisplayMacroValue(DMV,"DF_SAVEKERNEL",1,MakeString(DF_SAVEKERNEL));
#else
SPG_DisplayMacroValue(DMV,"DF_SAVEKERNEL",0,"");
#endif
#ifdef GUID_DEFINED
SPG_DisplayMacroValue(DMV,"GUID_DEFINED",1,MakeString(GUID_DEFINED));
#else
SPG_DisplayMacroValue(DMV,"GUID_DEFINED",0,"");
#endif
#ifdef HookMem
SPG_DisplayMacroValue(DMV,"HookMem",1,MakeString(HookMem));
#else
SPG_DisplayMacroValue(DMV,"HookMem",0,"");
#endif
#ifdef IntelCompilerFPU
SPG_DisplayMacroValue(DMV,"IntelCompilerFPU",1,MakeString(IntelCompilerFPU));
#else
SPG_DisplayMacroValue(DMV,"IntelCompilerFPU",0,"");
#endif
#ifdef IntelSpeedStepFix
SPG_DisplayMacroValue(DMV,"IntelSpeedStepFix",1,MakeString(IntelSpeedStepFix));
#else
SPG_DisplayMacroValue(DMV,"IntelSpeedStepFix",0,"");
#endif
/*
#ifdef NO_BASEINTERFACE_FUNCS
SPG_DisplayMacroValue(DMV,"NO_BASEINTERFACE_FUNCS",1,MakeString(NO_BASEINTERFACE_FUNCS));
#else
SPG_DisplayMacroValue(DMV,"NO_BASEINTERFACE_FUNCS",0,"");
#endif
#ifdef NO_INTSHCUT_GUIDS
SPG_DisplayMacroValue(DMV,"NO_INTSHCUT_GUIDS",1,MakeString(NO_INTSHCUT_GUIDS));
#else
SPG_DisplayMacroValue(DMV,"NO_INTSHCUT_GUIDS",0,"");
#endif
#ifdef NO_SHDOCVW_GUIDS
SPG_DisplayMacroValue(DMV,"NO_SHDOCVW_GUIDS",1,MakeString(NO_SHDOCVW_GUIDS));
#else
SPG_DisplayMacroValue(DMV,"NO_SHDOCVW_GUIDS",0,"");
#endif
#ifdef NO_SHLWAPI_GDI
SPG_DisplayMacroValue(DMV,"NO_SHLWAPI_GDI",1,MakeString(NO_SHLWAPI_GDI));
#else
SPG_DisplayMacroValue(DMV,"NO_SHLWAPI_GDI",0,"");
#endif
#ifdef NO_SHLWAPI_PATH
SPG_DisplayMacroValue(DMV,"NO_SHLWAPI_PATH",1,MakeString(NO_SHLWAPI_PATH));
#else
SPG_DisplayMacroValue(DMV,"NO_SHLWAPI_PATH",0,"");
#endif
#ifdef NO_SHLWAPI_REG
SPG_DisplayMacroValue(DMV,"NO_SHLWAPI_REG",1,MakeString(NO_SHLWAPI_REG));
#else
SPG_DisplayMacroValue(DMV,"NO_SHLWAPI_REG",0,"");
#endif
#ifdef NO_SHLWAPI_STREAM
SPG_DisplayMacroValue(DMV,"NO_SHLWAPI_STREAM",1,MakeString(NO_SHLWAPI_STREAM));
#else
SPG_DisplayMacroValue(DMV,"NO_SHLWAPI_STREAM",0,"");
#endif
#ifdef NO_SHLWAPI_STRFCNS
SPG_DisplayMacroValue(DMV,"NO_SHLWAPI_STRFCNS",1,MakeString(NO_SHLWAPI_STRFCNS));
#else
SPG_DisplayMacroValue(DMV,"NO_SHLWAPI_STRFCNS",0,"");
#endif
#ifdef NOANIMATE
SPG_DisplayMacroValue(DMV,"NOANIMATE",1,MakeString(NOANIMATE));
#else
SPG_DisplayMacroValue(DMV,"NOANIMATE",0,"");
#endif
#ifdef NOAVICAP
SPG_DisplayMacroValue(DMV,"NOAVICAP",1,MakeString(NOAVICAP));
#else
SPG_DisplayMacroValue(DMV,"NOAVICAP",0,"");
#endif
#ifdef NOAVIFILE
SPG_DisplayMacroValue(DMV,"NOAVIFILE",1,MakeString(NOAVIFILE));
#else
SPG_DisplayMacroValue(DMV,"NOAVIFILE",0,"");
#endif
#ifdef NOAVIFMT
SPG_DisplayMacroValue(DMV,"NOAVIFMT",1,MakeString(NOAVIFMT));
#else
SPG_DisplayMacroValue(DMV,"NOAVIFMT",0,"");
#endif
#ifdef NOBITMAP
SPG_DisplayMacroValue(DMV,"NOBITMAP",1,MakeString(NOBITMAP));
#else
SPG_DisplayMacroValue(DMV,"NOBITMAP",0,"");
#endif
#ifdef NOCLIPBOARD
SPG_DisplayMacroValue(DMV,"NOCLIPBOARD",1,MakeString(NOCLIPBOARD));
#else
SPG_DisplayMacroValue(DMV,"NOCLIPBOARD",0,"");
#endif
#ifdef NOCOLOR
SPG_DisplayMacroValue(DMV,"NOCOLOR",1,MakeString(NOCOLOR));
#else
SPG_DisplayMacroValue(DMV,"NOCOLOR",0,"");
#endif
#ifdef NOCOMPMAN
SPG_DisplayMacroValue(DMV,"NOCOMPMAN",1,MakeString(NOCOMPMAN));
#else
SPG_DisplayMacroValue(DMV,"NOCOMPMAN",0,"");
#endif
#ifdef NOCRYPT
SPG_DisplayMacroValue(DMV,"NOCRYPT",1,MakeString(NOCRYPT));
#else
SPG_DisplayMacroValue(DMV,"NOCRYPT",0,"");
#endif
#ifdef NOCTLMGR
SPG_DisplayMacroValue(DMV,"NOCTLMGR",1,MakeString(NOCTLMGR));
#else
SPG_DisplayMacroValue(DMV,"NOCTLMGR",0,"");
#endif
#ifdef NODATETIMEPICK
SPG_DisplayMacroValue(DMV,"NODATETIMEPICK",1,MakeString(NODATETIMEPICK));
#else
SPG_DisplayMacroValue(DMV,"NODATETIMEPICK",0,"");
#endif
#ifdef NODDEMLSPY
SPG_DisplayMacroValue(DMV,"NODDEMLSPY",1,MakeString(NODDEMLSPY));
#else
SPG_DisplayMacroValue(DMV,"NODDEMLSPY",0,"");
#endif
#ifdef NODEFERWINDOWPOS
SPG_DisplayMacroValue(DMV,"NODEFERWINDOWPOS",1,MakeString(NODEFERWINDOWPOS));
#else
SPG_DisplayMacroValue(DMV,"NODEFERWINDOWPOS",0,"");
#endif
#ifdef NODESKTOP
SPG_DisplayMacroValue(DMV,"NODESKTOP",1,MakeString(NODESKTOP));
#else
SPG_DisplayMacroValue(DMV,"NODESKTOP",0,"");
#endif
#ifdef NODRAGLIST
SPG_DisplayMacroValue(DMV,"NODRAGLIST",1,MakeString(NODRAGLIST));
#else
SPG_DisplayMacroValue(DMV,"NODRAGLIST",0,"");
#endif
#ifdef NODRAWDIB
SPG_DisplayMacroValue(DMV,"NODRAWDIB",1,MakeString(NODRAWDIB));
#else
SPG_DisplayMacroValue(DMV,"NODRAWDIB",0,"");
#endif
#ifdef NODRAWTEXT
SPG_DisplayMacroValue(DMV,"NODRAWTEXT",1,MakeString(NODRAWTEXT));
#else
SPG_DisplayMacroValue(DMV,"NODRAWTEXT",0,"");
#endif
#ifdef NOEXCHEXTGUIDS
SPG_DisplayMacroValue(DMV,"NOEXCHEXTGUIDS",1,MakeString(NOEXCHEXTGUIDS));
#else
SPG_DisplayMacroValue(DMV,"NOEXCHEXTGUIDS",0,"");
#endif
#ifdef NOEXCHFORMGUIDS
SPG_DisplayMacroValue(DMV,"NOEXCHFORMGUIDS",1,MakeString(NOEXCHFORMGUIDS));
#else
SPG_DisplayMacroValue(DMV,"NOEXCHFORMGUIDS",0,"");
#endif
#ifdef NOEXTAPI
SPG_DisplayMacroValue(DMV,"NOEXTAPI",1,MakeString(NOEXTAPI));
#else
SPG_DisplayMacroValue(DMV,"NOEXTAPI",0,"");
#endif
#ifdef NOFLATSBAPIS
SPG_DisplayMacroValue(DMV,"NOFLATSBAPIS",1,MakeString(NOFLATSBAPIS));
#else
SPG_DisplayMacroValue(DMV,"NOFLATSBAPIS",0,"");
#endif
#ifdef NOFONTSIG
SPG_DisplayMacroValue(DMV,"NOFONTSIG",1,MakeString(NOFONTSIG));
#else
SPG_DisplayMacroValue(DMV,"NOFONTSIG",0,"");
#endif
#ifdef NOGDI
SPG_DisplayMacroValue(DMV,"NOGDI",1,MakeString(NOGDI));
#else
SPG_DisplayMacroValue(DMV,"NOGDI",0,"");
#endif
#ifdef NOGDICAPMASKS
SPG_DisplayMacroValue(DMV,"NOGDICAPMASKS",1,MakeString(NOGDICAPMASKS));
#else
SPG_DisplayMacroValue(DMV,"NOGDICAPMASKS",0,"");
#endif
#ifdef NOHEADER
SPG_DisplayMacroValue(DMV,"NOHEADER",1,MakeString(NOHEADER));
#else
SPG_DisplayMacroValue(DMV,"NOHEADER",0,"");
#endif
#ifdef NOHELP
SPG_DisplayMacroValue(DMV,"NOHELP",1,MakeString(NOHELP));
#else
SPG_DisplayMacroValue(DMV,"NOHELP",0,"");
#endif
#ifdef NOHOTKEY
SPG_DisplayMacroValue(DMV,"NOHOTKEY",1,MakeString(NOHOTKEY));
#else
SPG_DisplayMacroValue(DMV,"NOHOTKEY",0,"");
#endif
#ifdef NOICONS
SPG_DisplayMacroValue(DMV,"NOICONS",1,MakeString(NOICONS));
#else
SPG_DisplayMacroValue(DMV,"NOICONS",0,"");
#endif
#ifdef NOIDLEENGINE
SPG_DisplayMacroValue(DMV,"NOIDLEENGINE",1,MakeString(NOIDLEENGINE));
#else
SPG_DisplayMacroValue(DMV,"NOIDLEENGINE",0,"");
#endif
#ifdef NOIMAGEAPIS
SPG_DisplayMacroValue(DMV,"NOIMAGEAPIS",1,MakeString(NOIMAGEAPIS));
#else
SPG_DisplayMacroValue(DMV,"NOIMAGEAPIS",0,"");
#endif
#ifdef NOIME
SPG_DisplayMacroValue(DMV,"NOIME",1,MakeString(NOIME));
#else
SPG_DisplayMacroValue(DMV,"NOIME",0,"");
#endif
#ifdef NOIPADDRESS
SPG_DisplayMacroValue(DMV,"NOIPADDRESS",1,MakeString(NOIPADDRESS));
#else
SPG_DisplayMacroValue(DMV,"NOIPADDRESS",0,"");
#endif
#ifdef NOJPEGDIB
SPG_DisplayMacroValue(DMV,"NOJPEGDIB",1,MakeString(NOJPEGDIB));
#else
SPG_DisplayMacroValue(DMV,"NOJPEGDIB",0,"");
#endif
#ifdef NOKEYSTATES
SPG_DisplayMacroValue(DMV,"NOKEYSTATES",1,MakeString(NOKEYSTATES));
#else
SPG_DisplayMacroValue(DMV,"NOKEYSTATES",0,"");
#endif
#ifdef NOLISTVIEW
SPG_DisplayMacroValue(DMV,"NOLISTVIEW",1,MakeString(NOLISTVIEW));
#else
SPG_DisplayMacroValue(DMV,"NOLISTVIEW",0,"");
#endif
#ifdef NOMB
SPG_DisplayMacroValue(DMV,"NOMB",1,MakeString(NOMB));
#else
SPG_DisplayMacroValue(DMV,"NOMB",0,"");
#endif
#ifdef NOMCIWND
SPG_DisplayMacroValue(DMV,"NOMCIWND",1,MakeString(NOMCIWND));
#else
SPG_DisplayMacroValue(DMV,"NOMCIWND",0,"");
#endif
#ifdef NOMCX
SPG_DisplayMacroValue(DMV,"NOMCX",1,MakeString(NOMCX));
#else
SPG_DisplayMacroValue(DMV,"NOMCX",0,"");
#endif
#ifdef NOMDI
SPG_DisplayMacroValue(DMV,"NOMDI",1,MakeString(NOMDI));
#else
SPG_DisplayMacroValue(DMV,"NOMDI",0,"");
#endif
#ifdef NOMENUHELP
SPG_DisplayMacroValue(DMV,"NOMENUHELP",1,MakeString(NOMENUHELP));
#else
SPG_DisplayMacroValue(DMV,"NOMENUHELP",0,"");
#endif
#ifdef NOMENUS
SPG_DisplayMacroValue(DMV,"NOMENUS",1,MakeString(NOMENUS));
#else
SPG_DisplayMacroValue(DMV,"NOMENUS",0,"");
#endif
#ifdef NOMETAFILE
SPG_DisplayMacroValue(DMV,"NOMETAFILE",1,MakeString(NOMETAFILE));
#else
SPG_DisplayMacroValue(DMV,"NOMETAFILE",0,"");
#endif
#ifdef NOMINMAX
SPG_DisplayMacroValue(DMV,"NOMINMAX",1,MakeString(NOMINMAX));
#else
SPG_DisplayMacroValue(DMV,"NOMINMAX",0,"");
#endif
#ifdef NOMMIDS
SPG_DisplayMacroValue(DMV,"NOMMIDS",1,MakeString(NOMMIDS));
#else
SPG_DisplayMacroValue(DMV,"NOMMIDS",0,"");
#endif
#ifdef NOMMREG
SPG_DisplayMacroValue(DMV,"NOMMREG",1,MakeString(NOMMREG));
#else
SPG_DisplayMacroValue(DMV,"NOMMREG",0,"");
#endif
#ifdef NOMONTHCAL
SPG_DisplayMacroValue(DMV,"NOMONTHCAL",1,MakeString(NOMONTHCAL));
#else
SPG_DisplayMacroValue(DMV,"NOMONTHCAL",0,"");
#endif
#ifdef NOMSACM
SPG_DisplayMacroValue(DMV,"NOMSACM",1,MakeString(NOMSACM));
#else
SPG_DisplayMacroValue(DMV,"NOMSACM",0,"");
#endif
#ifdef NOMSG
SPG_DisplayMacroValue(DMV,"NOMSG",1,MakeString(NOMSG));
#else
SPG_DisplayMacroValue(DMV,"NOMSG",0,"");
#endif
#ifdef NONATIVEFONTCTL
SPG_DisplayMacroValue(DMV,"NONATIVEFONTCTL",1,MakeString(NONATIVEFONTCTL));
#else
SPG_DisplayMacroValue(DMV,"NONATIVEFONTCTL",0,"");
#endif
#ifdef NONCMESSAGES
SPG_DisplayMacroValue(DMV,"NONCMESSAGES",1,MakeString(NONCMESSAGES));
#else
SPG_DisplayMacroValue(DMV,"NONCMESSAGES",0,"");
#endif
#ifdef NONEWIC
SPG_DisplayMacroValue(DMV,"NONEWIC",1,MakeString(NONEWIC));
#else
SPG_DisplayMacroValue(DMV,"NONEWIC",0,"");
#endif
#ifdef NONEWWAVE
SPG_DisplayMacroValue(DMV,"NONEWWAVE",1,MakeString(NONEWWAVE));
#else
SPG_DisplayMacroValue(DMV,"NONEWWAVE",0,"");
#endif
#ifdef NONLS
SPG_DisplayMacroValue(DMV,"NONLS",1,MakeString(NONLS));
#else
SPG_DisplayMacroValue(DMV,"NONLS",0,"");
#endif
#ifdef NOPAGESCROLLER
SPG_DisplayMacroValue(DMV,"NOPAGESCROLLER",1,MakeString(NOPAGESCROLLER));
#else
SPG_DisplayMacroValue(DMV,"NOPAGESCROLLER",0,"");
#endif
#ifdef NOPENALC
SPG_DisplayMacroValue(DMV,"NOPENALC",1,MakeString(NOPENALC));
#else
SPG_DisplayMacroValue(DMV,"NOPENALC",0,"");
#endif
#ifdef NOPENAPIFUN
SPG_DisplayMacroValue(DMV,"NOPENAPIFUN",1,MakeString(NOPENAPIFUN));
#else
SPG_DisplayMacroValue(DMV,"NOPENAPIFUN",0,"");
#endif
#ifdef NOPENAPPS
SPG_DisplayMacroValue(DMV,"NOPENAPPS",1,MakeString(NOPENAPPS));
#else
SPG_DisplayMacroValue(DMV,"NOPENAPPS",0,"");
#endif
#ifdef NOPENBEDIT
SPG_DisplayMacroValue(DMV,"NOPENBEDIT",1,MakeString(NOPENBEDIT));
#else
SPG_DisplayMacroValue(DMV,"NOPENBEDIT",0,"");
#endif
#ifdef NOPENBMP
SPG_DisplayMacroValue(DMV,"NOPENBMP",1,MakeString(NOPENBMP));
#else
SPG_DisplayMacroValue(DMV,"NOPENBMP",0,"");
#endif
#ifdef NOPENCTL
SPG_DisplayMacroValue(DMV,"NOPENCTL",1,MakeString(NOPENCTL));
#else
SPG_DisplayMacroValue(DMV,"NOPENCTL",0,"");
#endif
#ifdef NOPENCURS
SPG_DisplayMacroValue(DMV,"NOPENCURS",1,MakeString(NOPENCURS));
#else
SPG_DisplayMacroValue(DMV,"NOPENCURS",0,"");
#endif
#ifdef NOPENDATA
SPG_DisplayMacroValue(DMV,"NOPENDATA",1,MakeString(NOPENDATA));
#else
SPG_DisplayMacroValue(DMV,"NOPENDATA",0,"");
#endif
#ifdef NOPENDICT
SPG_DisplayMacroValue(DMV,"NOPENDICT",1,MakeString(NOPENDICT));
#else
SPG_DisplayMacroValue(DMV,"NOPENDICT",0,"");
#endif
#ifdef NOPENDRIVER
SPG_DisplayMacroValue(DMV,"NOPENDRIVER",1,MakeString(NOPENDRIVER));
#else
SPG_DisplayMacroValue(DMV,"NOPENDRIVER",0,"");
#endif
#ifdef NOPENHEDIT
SPG_DisplayMacroValue(DMV,"NOPENHEDIT",1,MakeString(NOPENHEDIT));
#else
SPG_DisplayMacroValue(DMV,"NOPENHEDIT",0,"");
#endif
#ifdef NOPENHRC
SPG_DisplayMacroValue(DMV,"NOPENHRC",1,MakeString(NOPENHRC));
#else
SPG_DisplayMacroValue(DMV,"NOPENHRC",0,"");
#endif
#ifdef NOPENIEDIT
SPG_DisplayMacroValue(DMV,"NOPENIEDIT",1,MakeString(NOPENIEDIT));
#else
SPG_DisplayMacroValue(DMV,"NOPENIEDIT",0,"");
#endif
#ifdef NOPENINKPUT
SPG_DisplayMacroValue(DMV,"NOPENINKPUT",1,MakeString(NOPENINKPUT));
#else
SPG_DisplayMacroValue(DMV,"NOPENINKPUT",0,"");
#endif
#ifdef NOPENMISC
SPG_DisplayMacroValue(DMV,"NOPENMISC",1,MakeString(NOPENMISC));
#else
SPG_DisplayMacroValue(DMV,"NOPENMISC",0,"");
#endif
#ifdef NOPENMSGS
SPG_DisplayMacroValue(DMV,"NOPENMSGS",1,MakeString(NOPENMSGS));
#else
SPG_DisplayMacroValue(DMV,"NOPENMSGS",0,"");
#endif
#ifdef NOPENRC1
SPG_DisplayMacroValue(DMV,"NOPENRC1",1,MakeString(NOPENRC1));
#else
SPG_DisplayMacroValue(DMV,"NOPENRC1",0,"");
#endif
#ifdef NOPENTARGET
SPG_DisplayMacroValue(DMV,"NOPENTARGET",1,MakeString(NOPENTARGET));
#else
SPG_DisplayMacroValue(DMV,"NOPENTARGET",0,"");
#endif
#ifdef NOPENVIRTEVENT
SPG_DisplayMacroValue(DMV,"NOPENVIRTEVENT",1,MakeString(NOPENVIRTEVENT));
#else
SPG_DisplayMacroValue(DMV,"NOPENVIRTEVENT",0,"");
#endif
#ifdef NOPROGRESS
SPG_DisplayMacroValue(DMV,"NOPROGRESS",1,MakeString(NOPROGRESS));
#else
SPG_DisplayMacroValue(DMV,"NOPROGRESS",0,"");
#endif
#ifdef NORASTEROPS
SPG_DisplayMacroValue(DMV,"NORASTEROPS",1,MakeString(NORASTEROPS));
#else
SPG_DisplayMacroValue(DMV,"NORASTEROPS",0,"");
#endif
#ifdef NOREBAR
SPG_DisplayMacroValue(DMV,"NOREBAR",1,MakeString(NOREBAR));
#else
SPG_DisplayMacroValue(DMV,"NOREBAR",0,"");
#endif
#ifdef NORESOURCE
SPG_DisplayMacroValue(DMV,"NORESOURCE",1,MakeString(NORESOURCE));
#else
SPG_DisplayMacroValue(DMV,"NORESOURCE",0,"");
#endif
#ifdef NOSCROLL
SPG_DisplayMacroValue(DMV,"NOSCROLL",1,MakeString(NOSCROLL));
#else
SPG_DisplayMacroValue(DMV,"NOSCROLL",0,"");
#endif
#ifdef NOSECURITY
SPG_DisplayMacroValue(DMV,"NOSECURITY",1,MakeString(NOSECURITY));
#else
SPG_DisplayMacroValue(DMV,"NOSECURITY",0,"");
#endif
#ifdef NOSERVICE
SPG_DisplayMacroValue(DMV,"NOSERVICE",1,MakeString(NOSERVICE));
#else
SPG_DisplayMacroValue(DMV,"NOSERVICE",0,"");
#endif
#ifdef NOSHLWAPI
SPG_DisplayMacroValue(DMV,"NOSHLWAPI",1,MakeString(NOSHLWAPI));
#else
SPG_DisplayMacroValue(DMV,"NOSHLWAPI",0,"");
#endif
#ifdef NOSHOWWINDOW
SPG_DisplayMacroValue(DMV,"NOSHOWWINDOW",1,MakeString(NOSHOWWINDOW));
#else
SPG_DisplayMacroValue(DMV,"NOSHOWWINDOW",0,"");
#endif
#ifdef NOSTATUSBAR
SPG_DisplayMacroValue(DMV,"NOSTATUSBAR",1,MakeString(NOSTATUSBAR));
#else
SPG_DisplayMacroValue(DMV,"NOSTATUSBAR",0,"");
#endif
#ifdef NOSYSCOMMANDS
SPG_DisplayMacroValue(DMV,"NOSYSCOMMANDS",1,MakeString(NOSYSCOMMANDS));
#else
SPG_DisplayMacroValue(DMV,"NOSYSCOMMANDS",0,"");
#endif
#ifdef NOSYSMETRICS
SPG_DisplayMacroValue(DMV,"NOSYSMETRICS",1,MakeString(NOSYSMETRICS));
#else
SPG_DisplayMacroValue(DMV,"NOSYSMETRICS",0,"");
#endif
#ifdef NOSYSPARAMSINFO
SPG_DisplayMacroValue(DMV,"NOSYSPARAMSINFO",1,MakeString(NOSYSPARAMSINFO));
#else
SPG_DisplayMacroValue(DMV,"NOSYSPARAMSINFO",0,"");
#endif
#ifdef NOTABCONTROL
SPG_DisplayMacroValue(DMV,"NOTABCONTROL",1,MakeString(NOTABCONTROL));
#else
SPG_DisplayMacroValue(DMV,"NOTABCONTROL",0,"");
#endif
#ifdef NOTEXTMETRIC
SPG_DisplayMacroValue(DMV,"NOTEXTMETRIC",1,MakeString(NOTEXTMETRIC));
#else
SPG_DisplayMacroValue(DMV,"NOTEXTMETRIC",0,"");
#endif
#ifdef NOTOOLBAR
SPG_DisplayMacroValue(DMV,"NOTOOLBAR",1,MakeString(NOTOOLBAR));
#else
SPG_DisplayMacroValue(DMV,"NOTOOLBAR",0,"");
#endif
#ifdef NOTOOLTIPS
SPG_DisplayMacroValue(DMV,"NOTOOLTIPS",1,MakeString(NOTOOLTIPS));
#else
SPG_DisplayMacroValue(DMV,"NOTOOLTIPS",0,"");
#endif
#ifdef NOTRACKBAR
SPG_DisplayMacroValue(DMV,"NOTRACKBAR",1,MakeString(NOTRACKBAR));
#else
SPG_DisplayMacroValue(DMV,"NOTRACKBAR",0,"");
#endif
#ifdef NOTRACKMOUSEEVENT
SPG_DisplayMacroValue(DMV,"NOTRACKMOUSEEVENT",1,MakeString(NOTRACKMOUSEEVENT));
#else
SPG_DisplayMacroValue(DMV,"NOTRACKMOUSEEVENT",0,"");
#endif
#ifdef NOTREEVIEW
SPG_DisplayMacroValue(DMV,"NOTREEVIEW",1,MakeString(NOTREEVIEW));
#else
SPG_DisplayMacroValue(DMV,"NOTREEVIEW",0,"");
#endif
#ifdef NOUPDOWN
SPG_DisplayMacroValue(DMV,"NOUPDOWN",1,MakeString(NOUPDOWN));
#else
SPG_DisplayMacroValue(DMV,"NOUPDOWN",0,"");
#endif
#ifdef NOUSER
SPG_DisplayMacroValue(DMV,"NOUSER",1,MakeString(NOUSER));
#else
SPG_DisplayMacroValue(DMV,"NOUSER",0,"");
#endif
#ifdef NOUSEREXCONTROLS
SPG_DisplayMacroValue(DMV,"NOUSEREXCONTROLS",1,MakeString(NOUSEREXCONTROLS));
#else
SPG_DisplayMacroValue(DMV,"NOUSEREXCONTROLS",0,"");
#endif
#ifdef NOVIRTUALKEYCODES
SPG_DisplayMacroValue(DMV,"NOVIRTUALKEYCODES",1,MakeString(NOVIRTUALKEYCODES));
#else
SPG_DisplayMacroValue(DMV,"NOVIRTUALKEYCODES",0,"");
#endif
#ifdef NOWH
SPG_DisplayMacroValue(DMV,"NOWH",1,MakeString(NOWH));
#else
SPG_DisplayMacroValue(DMV,"NOWH",0,"");
#endif
#ifdef NOWINABLE
SPG_DisplayMacroValue(DMV,"NOWINABLE",1,MakeString(NOWINABLE));
#else
SPG_DisplayMacroValue(DMV,"NOWINABLE",0,"");
#endif
#ifdef NOWINDOWSTATION
SPG_DisplayMacroValue(DMV,"NOWINDOWSTATION",1,MakeString(NOWINDOWSTATION));
#else
SPG_DisplayMacroValue(DMV,"NOWINDOWSTATION",0,"");
#endif
#ifdef NOWINMESSAGES
SPG_DisplayMacroValue(DMV,"NOWINMESSAGES",1,MakeString(NOWINMESSAGES));
#else
SPG_DisplayMacroValue(DMV,"NOWINMESSAGES",0,"");
#endif
#ifdef NOWINOFFSETS
SPG_DisplayMacroValue(DMV,"NOWINOFFSETS",1,MakeString(NOWINOFFSETS));
#else
SPG_DisplayMacroValue(DMV,"NOWINOFFSETS",0,"");
#endif
#ifdef NOWINSTYLES
SPG_DisplayMacroValue(DMV,"NOWINSTYLES",1,MakeString(NOWINSTYLES));
#else
SPG_DisplayMacroValue(DMV,"NOWINSTYLES",0,"");
#endif
*/
#ifdef PGLDisplay
SPG_DisplayMacroValue(DMV,"PGLDisplay",1,MakeString(PGLDisplay));
#else
SPG_DisplayMacroValue(DMV,"PGLDisplay",0,"");
#endif
#ifdef PGLPrimitive
SPG_DisplayMacroValue(DMV,"PGLPrimitive",1,MakeString(PGLPrimitive));
#else
SPG_DisplayMacroValue(DMV,"PGLPrimitive",0,"");
#endif
#ifdef PGLSurface
SPG_DisplayMacroValue(DMV,"PGLSurface",1,MakeString(PGLSurface));
#else
SPG_DisplayMacroValue(DMV,"PGLSurface",0,"");
#endif
#ifdef ResetCoproState
SPG_DisplayMacroValue(DMV,"ResetCoproState",1,MakeString(ResetCoproState));
#else
SPG_DisplayMacroValue(DMV,"ResetCoproState",0,"");
#endif
#ifdef restrict
SPG_DisplayMacroValue(DMV,"restrict",1,MakeString(restrict));
#else
SPG_DisplayMacroValue(DMV,"restrict",0,"");
#endif
#ifdef SetCoproState
SPG_DisplayMacroValue(DMV,"SetCoproState",1,MakeString(SetCoproState));
#else
SPG_DisplayMacroValue(DMV,"SetCoproState",0,"");
#endif
#ifdef SetCoproStateFirst
SPG_DisplayMacroValue(DMV,"SetCoproStateFirst",1,MakeString(SetCoproStateFirst));
#else
SPG_DisplayMacroValue(DMV,"SetCoproStateFirst",0,"");
#endif
#ifdef SGE_DiscardFace
SPG_DisplayMacroValue(DMV,"SGE_DiscardFace",1,MakeString(SGE_DiscardFace));
#else
SPG_DisplayMacroValue(DMV,"SGE_DiscardFace",0,"");
#endif
#ifdef SGE_DrawNormales
SPG_DisplayMacroValue(DMV,"SGE_DrawNormales",1,MakeString(SGE_DrawNormales));
#else
SPG_DisplayMacroValue(DMV,"SGE_DrawNormales",0,"");
#endif
#ifdef SGE_EMC
SPG_DisplayMacroValue(DMV,"SGE_EMC",1,MakeString(SGE_EMC));
#else
SPG_DisplayMacroValue(DMV,"SGE_EMC",0,"");
#endif
#ifdef SPG_COMPLEX_DEFINED
SPG_DisplayMacroValue(DMV,"SPG_COMPLEX_DEFINED",1,MakeString(SPG_COMPLEX_DEFINED));
#else
SPG_DisplayMacroValue(DMV,"SPG_COMPLEX_DEFINED",0,"");
#endif
#ifdef SPG_CONV
SPG_DisplayMacroValue(DMV,"SPG_CONV",1,MakeString(SPG_CONV));
#else
SPG_DisplayMacroValue(DMV,"SPG_CONV",0,"");
#endif
#ifdef SPG_FASTCONV
SPG_DisplayMacroValue(DMV,"SPG_FASTCONV",1,MakeString(SPG_FASTCONV));
#else
SPG_DisplayMacroValue(DMV,"SPG_FASTCONV",0,"");
#endif
#ifdef SPG_General_FastMath
SPG_DisplayMacroValue(DMV,"SPG_General_FastMath",1,MakeString(SPG_General_FastMath));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_FastMath",0,"");
#endif
#ifdef SPG_General_HIDEButtons
SPG_DisplayMacroValue(DMV,"SPG_General_HIDEButtons",1,MakeString(SPG_General_HIDEButtons));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_HIDEButtons",0,"");
#endif
#ifdef SPG_General_HIDECarac
SPG_DisplayMacroValue(DMV,"SPG_General_HIDECarac",1,MakeString(SPG_General_HIDECarac));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_HIDECarac",0,"");
#endif
#ifdef SPG_General_PGLib
SPG_DisplayMacroValue(DMV,"SPG_General_PGLib",1,MakeString(SPG_General_PGLib));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_PGLib",0,"");
#endif
#ifdef SPG_General_USE3DSelect
SPG_DisplayMacroValue(DMV,"SPG_General_USE3DSelect",1,MakeString(SPG_General_USE3DSelect));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USE3DSelect",0,"");
#endif
#ifdef SPG_General_USEACC
SPG_DisplayMacroValue(DMV,"SPG_General_USEACC",1,MakeString(SPG_General_USEACC));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEACC",0,"");
#endif
#ifdef SPG_General_USEAVI
SPG_DisplayMacroValue(DMV,"SPG_General_USEAVI",1,MakeString(SPG_General_USEAVI));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEAVI",0,"");
#endif
#ifdef SPG_General_USEAlign
SPG_DisplayMacroValue(DMV,"SPG_General_USEAlign",1,MakeString(SPG_General_USEAlign));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEAlign",0,"");
#endif
#ifdef SPG_General_USEBINCORR
SPG_DisplayMacroValue(DMV,"SPG_General_USEBINCORR",1,MakeString(SPG_General_USEBINCORR));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEBINCORR",0,"");
#endif
#ifdef SPG_General_USEBMPIO
SPG_DisplayMacroValue(DMV,"SPG_General_USEBMPIO",1,MakeString(SPG_General_USEBMPIO));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEBMPIO",0,"");
#endif
#ifdef SPG_General_USEBRES
SPG_DisplayMacroValue(DMV,"SPG_General_USEBRES",1,MakeString(SPG_General_USEBRES));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEBRES",0,"");
#endif
#ifdef SPG_General_USEButtons
SPG_DisplayMacroValue(DMV,"SPG_General_USEButtons",1,MakeString(SPG_General_USEButtons));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEButtons",0,"");
#endif
#ifdef SPG_General_USECarac
SPG_DisplayMacroValue(DMV,"SPG_General_USECarac",1,MakeString(SPG_General_USECarac));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USECarac",0,"");
#endif
#ifdef SPG_General_USECaracF
SPG_DisplayMacroValue(DMV,"SPG_General_USECaracF",1,MakeString(SPG_General_USECaracF));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USECaracF",0,"");
#endif
#ifdef SPG_General_USECDCHECK
SPG_DisplayMacroValue(DMV,"SPG_General_USECDCHECK",1,MakeString(SPG_General_USECDCHECK));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USECDCHECK",0,"");
#endif
#ifdef SPG_General_USEColorGenerator
SPG_DisplayMacroValue(DMV,"SPG_General_USEColorGenerator",1,MakeString(SPG_General_USEColorGenerator));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEColorGenerator",0,"");
#endif
#ifdef SPG_General_USEColors256
SPG_DisplayMacroValue(DMV,"SPG_General_USEColors256",1,MakeString(SPG_General_USEColors256));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEColors256",0,"");
#endif
#ifdef SPG_General_USECONFIGFILE
SPG_DisplayMacroValue(DMV,"SPG_General_USECONFIGFILE",1,MakeString(SPG_General_USECONFIGFILE));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USECONFIGFILE",0,"");
#endif
#ifdef SPG_General_USEConsole
SPG_DisplayMacroValue(DMV,"SPG_General_USEConsole",1,MakeString(SPG_General_USEConsole));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEConsole",0,"");
#endif
#ifdef SPG_General_USECut
SPG_DisplayMacroValue(DMV,"SPG_General_USECut",1,MakeString(SPG_General_USECut));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USECut",0,"");
#endif
#ifdef SPG_General_USEDCRZ
SPG_DisplayMacroValue(DMV,"SPG_General_USEDCRZ",1,MakeString(SPG_General_USEDCRZ));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEDCRZ",0,"");
#endif
#ifdef SPG_General_USEDECPHASE
SPG_DisplayMacroValue(DMV,"SPG_General_USEDECPHASE",1,MakeString(SPG_General_USEDECPHASE));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEDECPHASE",0,"");
#endif
#ifdef SPG_General_USEDiskBuffer
SPG_DisplayMacroValue(DMV,"SPG_General_USEDiskBuffer",1,MakeString(SPG_General_USEDiskBuffer));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEDiskBuffer",0,"");
#endif
#ifdef SPG_General_USEDISPFIELD
SPG_DisplayMacroValue(DMV,"SPG_General_USEDISPFIELD",1,MakeString(SPG_General_USEDISPFIELD));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEDISPFIELD",0,"");
#endif
#ifdef SPG_General_USEFFT
SPG_DisplayMacroValue(DMV,"SPG_General_USEFFT",1,MakeString(SPG_General_USEFFT));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEFFT",0,"");
#endif
#ifdef SPG_General_USEFileList
SPG_DisplayMacroValue(DMV,"SPG_General_USEFileList",1,MakeString(SPG_General_USEFileList));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEFileList",0,"");
#endif
#ifdef SPG_General_USEFiles
SPG_DisplayMacroValue(DMV,"SPG_General_USEFiles",1,MakeString(SPG_General_USEFiles));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEFiles",0,"");
#endif
#ifdef SPG_General_USEFilesWindows
SPG_DisplayMacroValue(DMV,"SPG_General_USEFilesWindows",1,MakeString(SPG_General_USEFilesWindows));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEFilesWindows",0,"");
#endif
#ifdef SPG_General_USEFirDesign
SPG_DisplayMacroValue(DMV,"SPG_General_USEFirDesign",1,MakeString(SPG_General_USEFirDesign));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEFirDesign",0,"");
#endif
#ifdef SPG_General_USEFirInvert
SPG_DisplayMacroValue(DMV,"SPG_General_USEFirInvert",1,MakeString(SPG_General_USEFirInvert));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEFirInvert",0,"");
#endif
#ifdef SPG_General_USEFLOATIO
SPG_DisplayMacroValue(DMV,"SPG_General_USEFLOATIO",1,MakeString(SPG_General_USEFLOATIO));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEFLOATIO",0,"");
#endif
#ifdef SPG_General_USEFTP
SPG_DisplayMacroValue(DMV,"SPG_General_USEFTP",1,MakeString(SPG_General_USEFTP));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEFTP",0,"");
#endif
#ifdef SPG_General_USEGEFFECT
SPG_DisplayMacroValue(DMV,"SPG_General_USEGEFFECT",1,MakeString(SPG_General_USEGEFFECT));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEGEFFECT",0,"");
#endif
#ifdef SPG_General_USEGlobal
SPG_DisplayMacroValue(DMV,"SPG_General_USEGlobal",1,MakeString(SPG_General_USEGlobal));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEGlobal",0,"");
#endif
#ifdef SPG_General_USEGraphics
SPG_DisplayMacroValue(DMV,"SPG_General_USEGraphics",1,MakeString(SPG_General_USEGraphics));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEGraphics",0,"");
#endif
#ifdef SPG_General_USEGraphicsRenderPoly
SPG_DisplayMacroValue(DMV,"SPG_General_USEGraphicsRenderPoly",1,MakeString(SPG_General_USEGraphicsRenderPoly));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEGraphicsRenderPoly",0,"");
#endif
#ifdef SPG_General_USEHDF
SPG_DisplayMacroValue(DMV,"SPG_General_USEHDF",1,MakeString(SPG_General_USEHDF));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEHDF",0,"");
#endif
#ifdef SPG_General_USEHIST
SPG_DisplayMacroValue(DMV,"SPG_General_USEHIST",1,MakeString(SPG_General_USEHIST));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEHIST",0,"");
#endif
#ifdef SPG_General_USEHYP
SPG_DisplayMacroValue(DMV,"SPG_General_USEHYP",1,MakeString(SPG_General_USEHYP));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEHYP",0,"");
#endif
#ifdef SPG_General_USEINTERACT2D
SPG_DisplayMacroValue(DMV,"SPG_General_USEINTERACT2D",1,MakeString(SPG_General_USEINTERACT2D));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEINTERACT2D",0,"");
#endif
#ifdef SPG_General_USEINTERACT3D
SPG_DisplayMacroValue(DMV,"SPG_General_USEINTERACT3D",1,MakeString(SPG_General_USEINTERACT3D));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEINTERACT3D",0,"");
#endif
#ifdef SPG_General_USEInterpolateur
SPG_DisplayMacroValue(DMV,"SPG_General_USEInterpolateur",1,MakeString(SPG_General_USEInterpolateur));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEInterpolateur",0,"");
#endif
#ifdef SPG_General_USEINVJ0
SPG_DisplayMacroValue(DMV,"SPG_General_USEINVJ0",1,MakeString(SPG_General_USEINVJ));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEINVJ0",0,"");
#endif
#ifdef SPG_General_USEMECA
SPG_DisplayMacroValue(DMV,"SPG_General_USEMECA",1,MakeString(SPG_General_USEMECA));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEMECA",0,"");
#endif
#ifdef SPG_General_USEMELINK
SPG_DisplayMacroValue(DMV,"SPG_General_USEMELINK",1,MakeString(SPG_General_USEMELINK));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEMELINK",0,"");
#endif
#ifdef SPG_General_USENetwork
SPG_DisplayMacroValue(DMV,"SPG_General_USENetwork",1,MakeString(SPG_General_USENetwork));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USENetwork",0,"");
#endif
#ifdef SPG_General_USENetwork_OPTS
SPG_DisplayMacroValue(DMV,"SPG_General_USENetwork_OPTS",1,MakeString(SPG_General_USENetwork_OPTS));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USENetwork_OPTS",0,"");
#endif
#ifdef SPG_General_USENetwork_Protocol
SPG_DisplayMacroValue(DMV,"SPG_General_USENetwork_Protocol",1,MakeString(SPG_General_USENetwork_Protocol));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USENetwork_Protocol",0,"");
#endif
#ifdef SPG_General_USENetworkEmule
SPG_DisplayMacroValue(DMV,"SPG_General_USENetworkEmule",1,MakeString(SPG_General_USENetworkEmule));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USENetworkEmule",0,"");
#endif
#ifdef SPG_General_USEPaint
SPG_DisplayMacroValue(DMV,"SPG_General_USEPaint",1,MakeString(SPG_General_USEPaint));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEPaint",0,"");
#endif
#ifdef SPG_General_USEPARAFIT
SPG_DisplayMacroValue(DMV,"SPG_General_USEPARAFIT",1,MakeString(SPG_General_USEPARAFIT));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEPARAFIT",0,"");
#endif
#ifdef SPG_General_USEPEAKDET
SPG_DisplayMacroValue(DMV,"SPG_General_USEPEAKDET",1,MakeString(SPG_General_USEPEAKDET));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEPEAKDET",0,"");
#endif
#ifdef SPG_General_USEPEAKDET2D
SPG_DisplayMacroValue(DMV,"SPG_General_USEPEAKDET2D",1,MakeString(SPG_General_USEPEAKDET2D));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEPEAKDET2D",0,"");
#endif
#ifdef SPG_General_USEPIXINT
SPG_DisplayMacroValue(DMV,"SPG_General_USEPIXINT",1,MakeString(SPG_General_USEPIXINT));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEPIXINT",0,"");
#endif
#ifdef SPG_General_USEPrCV
SPG_DisplayMacroValue(DMV,"SPG_General_USEPrCV",1,MakeString(SPG_General_USEPrCV));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEPrCV",0,"");
#endif
#ifdef SPG_General_USEPRO
SPG_DisplayMacroValue(DMV,"SPG_General_USEPRO",1,MakeString(SPG_General_USEPRO));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEPRO",0,"");
#endif
#ifdef SPG_General_USEProfil
SPG_DisplayMacroValue(DMV,"SPG_General_USEProfil",1,MakeString(SPG_General_USEProfil));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEProfil",0,"");
#endif
#ifdef SPG_General_USEProfil3D
SPG_DisplayMacroValue(DMV,"SPG_General_USEProfil3D",1,MakeString(SPG_General_USEProfil3D));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEProfil3D",0,"");
#endif
#ifdef SPG_General_USEProgPrincipal
SPG_DisplayMacroValue(DMV,"SPG_General_USEProgPrincipal",1,MakeString(SPG_General_USEProgPrincipal));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEProgPrincipal",0,"");
#endif
#ifdef SPG_General_USEPrXt
SPG_DisplayMacroValue(DMV,"SPG_General_USEPrXt",1,MakeString(SPG_General_USEPrXt));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEPrXt",0,"");
#endif
#ifdef SPG_General_USERINGREC
SPG_DisplayMacroValue(DMV,"SPG_General_USERINGREC",1,MakeString(SPG_General_USERINGREC));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USERINGREC",0,"");
#endif
#ifdef SPG_General_USEScale
SPG_DisplayMacroValue(DMV,"SPG_General_USEScale",1,MakeString(SPG_General_USEScale));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEScale",0,"");
#endif
#ifdef SPG_General_USESerialComm
SPG_DisplayMacroValue(DMV,"SPG_General_USESerialComm",1,MakeString(SPG_General_USESerialComm));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USESerialComm",0,"");
#endif
#ifdef SPG_General_USESGRAPH
SPG_DisplayMacroValue(DMV,"SPG_General_USESGRAPH",1,MakeString(SPG_General_USESGRAPH));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USESGRAPH",0,"");
#endif
#ifdef SPG_General_USESGRAPH_OPTS
SPG_DisplayMacroValue(DMV,"SPG_General_USESGRAPH_OPTS",1,MakeString(SPG_General_USESGRAPH_OPTS));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USESGRAPH_OPTS",0,"");
#endif
#ifdef SPG_General_USESMTPPOP3
SPG_DisplayMacroValue(DMV,"SPG_General_USESMTPPOP3",1,MakeString(SPG_General_USESMTPPOP3));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USESMTPPOP3",0,"");
#endif
#ifdef SPG_General_USESON
SPG_DisplayMacroValue(DMV,"SPG_General_USESON",1,MakeString(SPG_General_USESON));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USESON",0,"");
#endif
#ifdef SPG_General_USESTEREOVIS
SPG_DisplayMacroValue(DMV,"SPG_General_USESTEREOVIS",1,MakeString(SPG_General_USESTEREOVIS));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USESTEREOVIS",0,"");
#endif
#ifdef SPG_General_USESUR
SPG_DisplayMacroValue(DMV,"SPG_General_USESUR",1,MakeString(SPG_General_USESUR));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USESUR",0,"");
#endif
#ifdef SPG_General_USETimer
SPG_DisplayMacroValue(DMV,"SPG_General_USETimer",1,MakeString(SPG_General_USETimer));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USETimer",0,"");
#endif
#ifdef SPG_General_USETREEVIEW
SPG_DisplayMacroValue(DMV,"SPG_General_USETREEVIEW",1,MakeString(SPG_General_USETREEVIEW));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USETREEVIEW",0,"");
#endif
#ifdef SPG_General_USETXT
SPG_DisplayMacroValue(DMV,"SPG_General_USETXT",1,MakeString(SPG_General_USETXT));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USETXT",0,"");
#endif
#ifdef SPG_General_USEULIPS
SPG_DisplayMacroValue(DMV,"SPG_General_USEULIPS",1,MakeString(SPG_General_USEULIPS));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEULIPS",0,"");
#endif
#ifdef SPG_General_USEULIPSINTERFACE
SPG_DisplayMacroValue(DMV,"SPG_General_USEULIPSINTERFACE",1,MakeString(SPG_General_USEULIPSINTERFACE));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEULIPSINTERFACE",0,"");
#endif
#ifdef SPG_General_USEUnwrap
SPG_DisplayMacroValue(DMV,"SPG_General_USEUnwrap",1,MakeString(SPG_General_USEUnwrap));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEUnwrap",0,"");
#endif
#ifdef SPG_General_USEVidCap
SPG_DisplayMacroValue(DMV,"SPG_General_USEVidCap",1,MakeString(SPG_General_USEVidCap));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEVidCap",0,"");
#endif
#ifdef SPG_General_USEWAVIO
SPG_DisplayMacroValue(DMV,"SPG_General_USEWAVIO",1,MakeString(SPG_General_USEWAVIO));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEWAVIO",0,"");
#endif
#ifdef SPG_General_USEWEIGHTBUFFER
SPG_DisplayMacroValue(DMV,"SPG_General_USEWEIGHTBUFFER",1,MakeString(SPG_General_USEWEIGHTBUFFER));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEWEIGHTBUFFER",0,"");
#endif
#ifdef SPG_General_USEWindows
SPG_DisplayMacroValue(DMV,"SPG_General_USEWindows",1,MakeString(SPG_General_USEWindows));
#else
SPG_DisplayMacroValue(DMV,"SPG_General_USEWindows",0,"");
#endif
#ifdef SPG_GlobalAllocDefined
SPG_DisplayMacroValue(DMV,"SPG_GlobalAllocDefined",1,MakeString(SPG_GlobalAllocDefined));
#else
SPG_DisplayMacroValue(DMV,"SPG_GlobalAllocDefined",0,"");
#endif
#ifdef SPG_SAFECONV
SPG_DisplayMacroValue(DMV,"SPG_SAFECONV",1,MakeString(SPG_SAFECONV));
#else
SPG_DisplayMacroValue(DMV,"SPG_SAFECONV",0,"");
#endif
#ifdef SPG_UseGlobalAlloc
SPG_DisplayMacroValue(DMV,"SPG_UseGlobalAlloc",1,MakeString(SPG_UseGlobalAlloc));
#else
SPG_DisplayMacroValue(DMV,"SPG_UseGlobalAlloc",0,"");
#endif
#ifdef SPG_UseMalloc
SPG_DisplayMacroValue(DMV,"SPG_UseMalloc",1,MakeString(SPG_UseMalloc));
#else
SPG_DisplayMacroValue(DMV,"SPG_UseMalloc",0,"");
#endif
#ifdef SPG_VIDEOCHANGE
SPG_DisplayMacroValue(DMV,"SPG_VIDEOCHANGE",1,MakeString(SPG_VIDEOCHANGE));
#else
SPG_DisplayMacroValue(DMV,"SPG_VIDEOCHANGE",0,"");
#endif
#ifdef TimerPentium
SPG_DisplayMacroValue(DMV,"TimerPentium",1,MakeString(TimerPentium));
#else
SPG_DisplayMacroValue(DMV,"TimerPentium",0,"");
#endif
#ifdef UseFFT4ASM
SPG_DisplayMacroValue(DMV,"UseFFT4ASM",1,MakeString(UseFFT4ASM));
#else
SPG_DisplayMacroValue(DMV,"UseFFT4ASM",0,"");
#endif
#ifdef UseFFT4C
SPG_DisplayMacroValue(DMV,"UseFFT4C",1,MakeString(UseFFT4C));
#else
SPG_DisplayMacroValue(DMV,"UseFFT4C",0,"");
#endif
#ifdef UseFFTW
SPG_DisplayMacroValue(DMV,"UseFFTW",1,MakeString(UseFFTW));
#else
SPG_DisplayMacroValue(DMV,"UseFFTW",0,"");
#endif
#ifdef WIN32_LEAN_AND_MEAN
SPG_DisplayMacroValue(DMV,"WIN32_LEAN_AND_MEAN",1,MakeString(WIN32_LEAN_AND_MEAN));
#else
SPG_DisplayMacroValue(DMV,"WIN32_LEAN_AND_MEAN",0,"");
#endif
#ifdef WINVER
SPG_DisplayMacroValue(DMV,"WINVER",1,MakeString(WINVER));
#else
SPG_DisplayMacroValue(DMV,"WINVER",0,"");
#endif


return;
}

