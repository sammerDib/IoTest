
#include "SPG_General.h"

#ifdef SPG_General_USENIDAQmxEnum

#include "SPG_Includes.h"
#include "Connexion\SCM_Connexion_NIDAQmx_Internal.h"
#include "SPG_SysInc.h"

#define NIDAQmxRELPATH "NIDAQ_SDK\\"
#ifdef SPG_General_CapabladeV3Config
#include "..\..\NIDAQ_SDK\SPG_NIDAQmxConfig.h"
#else
#include "..\NIDAQ_SDK\SPG_NIDAQmxConfig.h"
#endif

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

int SPG_CONV SCM_NIDAQmxList(SCM_DEVICELIST& L)
{
	SPG_ZeroStruct(L);

#ifdef SPG_General_USENIDAQmx

	char DevNames[1024]; DAQmxGetSysDevNames(DevNames,1024); //SPG_List(DevNames);
	char* StartOfName=DevNames; char* EndOfName=DevNames;
	int match=0; int DeviceFound=0;

	while(*EndOfName!='\0')
	{
		StartOfName=EndOfName;
		while((*EndOfName!=';')&&(*EndOfName!=',')&&(*EndOfName!='\0')&&(*EndOfName!=' ')) {EndOfName++;}//passe sur le nom
		while((*EndOfName==';')||(*EndOfName==',')||(*EndOfName==' ')) {*EndOfName=0; EndOfName++;}//passe sur les séparateurs en les écrasant

		//StartOfName est le nom courant, vérifie d'apres l'existence du serial nr que le device est réellement présent
		L.D[L.n].Serial=0;
		DAQmxGetDevSerialNum(StartOfName,&L.D[L.n].Serial);
		if(L.D[L.n].Serial==0) continue;//le device n'est pas réellement pluggé dans la machine

		strncpy(L.D[L.n].Name,StartOfName,63);
		DAQmxGetDevProductType(L.D[L.n].Name,L.D[L.n].Type,64);
		L.n++;
	}

#endif

	return L.n;
}

int SPG_CONV SCM_NIDAQmxGetNthOfType(SCM_DEVICELIST& L, int Nth, char* DeviceType)//Nth à base zero
{
	int match=0;
	for(int n=0;n<L.n;n++)
	{
		if(SPG_StrFind(DeviceType,L.D[n].Type)) {if(match==Nth) {return n;} match++;}
	}
	return -1;
}

int SPG_CONV SCM_NIDAQmxIsInList(SCM_DEVICELIST& L, char* DeviceName)
{
	for(int n=0;n<L.n;n++)
	{
		if(stricmp(DeviceName,L.D[n].Name)==0) return L.n;
	}
	return -1;
}

int SPG_CONV SCM_NIDAQmxListSave(char* FileName, SCM_DEVICELIST& L)
{
	FILE* F=fopen(FileName,"wb+");
	fprintf(F,"Name\tType\tSerial\r\n");
	for(int n=0;n<L.n;n++)
	{
		fprintf(F,"%s\t%s\t%i\r\n",L.D[n].Name,L.D[n].Type,L.D[n].Serial);
	}
	fclose(F);
	return -1;
}

int SPG_CONV SCM_NIDAQmxListDisplay(SCM_DEVICELIST& L)
{
	char Msg[1024];
	char* cM=Msg;
	if(L.n)
	{
		cM+=sprintf(cM,"Name - Type - Serial\r\n");
		for(int n=0;n<L.n;n++)
		{
			cM+=sprintf(cM,"%s - %s - %i\n",L.D[n].Name,L.D[n].Type,L.D[n].Serial);
		}
	}
	else
	{
#ifdef SPG_General_USENIDAQmx
		cM=strcpy(cM,"No NIDAQmx device found");
#else
		cM=strcpy(cM,"NIDAQmx not enabled in this exe file (dev build)");
#endif
	}
#ifndef SPG_General_USEWindows
	SPG_List(Msg);
#else
	MessageBox((HWND)Global.hWndWin,Msg,"SCM_NIDAQmxListDisplay",MB_OK);
#endif
	return -1;
}

int SPG_CONV SCM_NIDAQmxGetFirstLineNr(char* DigLine)
{
	char localDigLineName[NISTRLEN];//ce n'est pas indispensable de faire une copie locale si on accepte que le nom de device recu en argument passe en minuscule
	strncpy(localDigLineName,DigLine,NISTRLEN);
	_strlwr(localDigLineName);
	char* L=strstr(localDigLineName,"line");
	if(L==0) return 0;
	if(L[4]==0) return 0;
	return L[4]-'0';
}

#pragma warning(disable:4706)

int SPG_CONV SCM_NIDAQmxGetNumChan(char* chan)
{	
	int L;
	CHECK( (chan==0)||(L=strlen(chan)<3), "SCM_NIDAQmxGetNumChan",return 0); //assignement volontaire
	//aix ou aix:y
	if(((chan[0]=='a')||(chan[0]=='d'))&&((chan[1]=='i')||(chan[1]=='o')))
	{
		chan+=2;
		int i0 = SPG_ReadInt(chan);
		if(*chan==0) return 1;
		CHECKTWO(*chan++!=':',"SCM_NIDAQmxGetNumChan",chan-3,return 0);
		int i1 = SPG_ReadInt(chan);
		return 1+i1-i0;
	}
	else
	{
		DbgCHECKTWO(1,"SCM_NIDAQmxGetNumChan",chan);
	}
	return 0;
}

int SPG_CONV SCM_NIDAQmxGetModeFlag(char* chan, char* clockName, double FrequencyHz, DWORD& dwMode)
{
	char localChanName[NISTRLEN];//ce n'est pas indispensable de faire une copie locale si on accepte que le nom de device recu en argument passe en minuscule
	strncpy(localChanName,chan,NISTRLEN);
	_strlwr(localChanName);

	if(dwMode==0) dwMode|=M_RAW;

	if(strstr(localChanName,"ai")) dwMode|=M_Read|M_CreateAIVoltageChan;
	if(strstr(localChanName,"ao")) dwMode|=M_Write|M_CreateAOVoltageChan;

	char* L;
	if(L=strstr(localChanName,"di")) //assignement volontaire
	{//attention ai part de zero di/line part de 1
		strcpy(chan,"Port0/Line");
		strcat(chan,L+2);
		dwMode|=M_Read|M_CreateDIChan;
	}
	if(L=strstr(localChanName,"do")) //assignement volontaire
	{//attention ai part de zero di/line part de 1
		strcpy(chan,"Port0/Line");
		strcat(chan,L+2);
		dwMode|=M_Write|M_CreateDOChan;
	}

	if(FrequencyHz>0) dwMode|=M_CfgSampClkTiming;

	if(stricmp(clockName,COUNTER0CLOCK)==0) dwMode|=M_CounterClkTiming;
	
	return -1;
}


/*
int SPG_CONV SCM_NIDAQmxValidate(char* DeviceName, char* DeviceType, int DeviceNth)
{
	CHECK(DeviceName[0]==0,"scxNIDAQOpen: No device specified",return 0);

	if(DeviceName[0]=='*')
	{
		char DevNames[1024]; DAQmxGetSysDevNames(DevNames,1024); //SPG_List(DevNames);
		char* StartOfName=DevNames; char* EndOfName=DevNames;
		int match=0; int DeviceFound=0;

		while(*EndOfName!='\0')
		{
			while((*EndOfName!=';')&&(*EndOfName!=',')&&(*EndOfName!='\0')) {EndOfName++;}//passe sur le nom
			if((*EndOfName==';')||(*EndOfName==',')) {*EndOfName=0; EndOfName++;}//passe sur les séparateurs en les écrasant

			//StartOfName est le nom courant, à zero terminal - récupère le type

			if(stricmp(DevName,StartOfName)

			char DevType[256]; SPG_ZeroStruct(DevType); 
			DAQmxGetDevProductType(StartOfName,DevType,256); //SPG_List2S(StartOfName,DevType);

			if(stricmp(DevType,DeviceType)==0)//le type correspond
			{
				if(match==DeviceNth) { DeviceFound=1; break; }
				match++;
			}
		}
		CHECK(DeviceFound==0,"scxNIDAQOpen: No device matching DevType and DevNth",return 0);
		strcpy(DeviceName,StartOfName);
	}
}

*/

/*

int SPG_CONV SCM_NIDAQmxList(SCM_DEVICELIST& L)
{
	SPG_ZeroStruct(L);
	return 0;
}

int SPG_CONV SCM_NIDAQmxIsInList(SCM_DEVICELIST& L, char* DeviceName)
{
	return 0;
}

int SPG_CONV SCM_NIDAQmxListSave(char* FileName, SCM_DEVICELIST& L)
{
	return -1;
}

int SPG_CONV SCM_NIDAQmxListDisplay(SCM_DEVICELIST& L)
{
	SPG_List("SrcC Configuration : NIDAQ interface not enabled");
	return -1;
}

*/

#endif

