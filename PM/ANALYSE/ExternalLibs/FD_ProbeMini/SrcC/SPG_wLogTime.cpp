
#include "SPG_General.h"
#ifdef SPG_General_USELOGTIME

#error "SPG_LogTime obsolete"

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <string.h>
#include <stdio.h>

//fdef DebugWatchLogTime

//######################## PROCESS INTERNE ###########################
/*
void SPG_CONV SPG_LogTime(SPG_WLOGTIME& wLT, int UID, int Flag, int Code)
{
	if(wLT.pLT) SPG_LogTime(*wLT.pLT,UID,Flag,Code);
}
*/

int SPG_CONV SPG_LogTimeCreateUID(SPG_WLOGTIME& wLT, int Flag, DWORD Color, char* File, char* Name, char* Descr)
{
	if(wLT.pLT) return SPG_LogTimeCreateUID(*wLT.pLT,Flag,Color,File,Name,Descr); else return 0;
}

#define ERROR_ALREADY_EXISTS             183L

int SPG_CONV SPG_LogTimeInit(SPG_WLOGTIME& wLT, int MaxEventTypes, int MaxEvents, int Flag)
{
	SPG_ZeroStruct(wLT);
	int ltMaxEvents=1;
	{for(int i=1;i<31;i++) { ltMaxEvents=(1<<i); if(ltMaxEvents>=MaxEvents) break; }}
	int FileSize=sizeof(SPG_LOGTIME)+MaxEventTypes*sizeof(SPG_EVENTTYPE)+ltMaxEvents*sizeof(SPG_LOGEVENT);
	wLT.hMapFile = CreateFileMapping(INVALID_HANDLE_VALUE,0,PAGE_READWRITE,0,FileSize,wLogTimeName); CHECK(wLT.hMapFile == NULL,"SPG_LogTimeInit",return 0); 
	DbgCHECK(GetLastError() == ERROR_ALREADY_EXISTS,"SPG_LogTimeInit")
	wLT.lpMapAddress = MapViewOfFile(wLT.hMapFile,FILE_MAP_ALL_ACCESS,0,0,0); CHECK(wLT.lpMapAddress == NULL,"SPG_LogTimeInit",CloseHandle(wLT.hMapFile);return 0); 
	wLT.pLT=(SPG_LOGTIME*)wLT.lpMapAddress;

	//SPG_LogTimeInit(LT,MaxEventTypes,MaxEvents,Flag);
	SPG_LOGTIME& LT=*wLT.pLT;
	LT.Flag=Flag;
	LT.MaxEvents=ltMaxEvents;
	LT.MskEvents=ltMaxEvents-1;
	LT.MaxEventTypes=MaxEventTypes;
	LT.T=(SPG_EVENTTYPE*)((BYTE*)wLT.pLT+sizeof(SPG_LOGTIME)); //initialise dans le contexte du process interne
	LT.E=(SPG_LOGEVENT*)((BYTE*)LT.T+LT.MaxEventTypes*sizeof(SPG_EVENTTYPE));
	LT.TimeScale=1.0f/(float)V_Max(Global.CPUClock,1);
	LT.NumEventTypes=1;//le type zero est 'unknown' et sert a reperer les erreur
	LT.NumEvents=0;
#ifdef SPG_General_USELOGTIME_OSABNORMALTERMINATIONHANDLER
	SPG_LogTimeSetFilter(LT);
#endif
#ifdef SPG_General_USELOGTIMETHREADID
	SPL_Init(LT.L,5,"LogTime");
	SPL_Exit(LT.L);
#endif

	/*
	*/
	//int FileSize=sizeof(SPG_LOGTIME)+MaxEventTypes*sizeof(SPG_EVENTTYPE)+MaxEvents*sizeof(SPG_LOGEVENT);
	{S_TimerCountType T; S_Timer_RDTSC(T); LT.AbsoluteStartTick=T;}
	return LT.Etat=-1;
}

int SPG_CONV SPG_LogTimeClose(SPG_WLOGTIME& wLT)
{
	if (wLT.lpMapAddress!=0) UnmapViewOfFile(wLT.lpMapAddress);
	if (wLT.hMapFile!=0) CloseHandle(wLT.hMapFile); 
	SPG_ZeroStruct(wLT);
	return -1;
}

void SPG_CONV SPG_LogTimeClear(SPG_WLOGTIME& wLT)
{
	if(wLT.pLT) SPG_LogTimeClear(*wLT.pLT); return;
}

int SPG_CONV SPG_LogTimeSaveToFile_OneRow(SPG_WLOGTIME& wLT, char* FileName)
{
	if(wLT.pLT) return SPG_LogTimeSaveToFile_OneRow(*wLT.pLT,FileName); else return 0;
}

int SPG_CONV SPG_LogTimeSaveToFile_MultiRow(SPG_WLOGTIME& wLT, char* FileName)
{
	if(wLT.pLT) return SPG_LogTimeSaveToFile_MultiRow(*wLT.pLT,FileName); else return 0;
}

//######################## PROCESS INTERNE ###########################

//lse

//######################## PROCESS EXTERNE ###########################

//le watchdog doit etre démarré en premier

int SPG_CONV SPG_LogTimeWatchdogInit(SPG_WATCHTIME& WT)
{
	SPG_ZeroStruct(WT);
	WT.hMapFile = OpenFileMapping(FILE_MAP_ALL_ACCESS,0,wLogTimeName); CHECK(WT.hMapFile == NULL,"SPG_LogTimeWatchdogInit",return 0); 
	WT.lpMapAddress = MapViewOfFile(WT.hMapFile,FILE_MAP_ALL_ACCESS,0,0,0); CHECK(WT.lpMapAddress == NULL,"SPG_LogTimeWatchdogInit",CloseHandle(WT.hMapFile);return 0); 
	WT.pLT=(SPG_LOGTIME*)WT.lpMapAddress;
	SPG_LOGTIME& LT=*WT.pLT;
	//CHECK(LT.MaxEventTypes==0,"SPG_wLogTimeInit",return 0);
	//CHECK(LT.MaxEvents==0,"SPG_wLogTimeInit",return 0);
	return -1;
}

int SPG_CONV SPG_LogTimeWatchdogClose(SPG_WATCHTIME& WT)
{
	if (WT.lpMapAddress!=0) UnmapViewOfFile(WT.lpMapAddress);
	if (WT.hMapFile!=0) CloseHandle(WT.hMapFile); 
	SPG_ZeroStruct(WT);
	return -1;
}

int SPG_CONV SPG_LogTimeWatchdogDump(SPG_WATCHTIME& WT, char* FileName)
{
	SPG_LOGTIME LT=*WT.pLT;//copie la structure cas ses pointeurs ne sont pas valides dans le contexte du process externe
	LT.T=(SPG_EVENTTYPE*)((BYTE*)WT.pLT+sizeof(SPG_LOGTIME)); //initialise dans le contexte du process interne
	LT.E=(SPG_LOGEVENT*)((BYTE*)LT.T+LT.MaxEventTypes*sizeof(SPG_EVENTTYPE));
	return SPG_LogTimeSaveToFile_MultiRow(LT,FileName);
}

//######################## PROCESS EXTERNE ###########################


//ndif
#endif

