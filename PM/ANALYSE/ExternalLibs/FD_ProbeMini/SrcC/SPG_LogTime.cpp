
#include "SPG_General.h"
#ifdef SPG_General_USELOGTIME

#error "SPG_LogTime obsolete"

#include "SPG_Includes.h"
//fdef SPG_General_USELOGTIMETHREADID
#include "SPG_SysInc.h"
//ndif

#include "BreakHook.h"

#ifdef SPG_General_USELOGTIME_OSABNORMALTERMINATIONHANDLER

LONG WINAPI LogTime_UnhandledExceptionFilter(struct _EXCEPTION_POINTERS *ExceptionInfo);

//http://blog.kalmbachnet.de/?postid=75 "SetUnhandledExceptionFilter" and VC8
LPTOP_LEVEL_EXCEPTION_FILTER WINAPI MyDummySetUnhandledExceptionFilter(LPTOP_LEVEL_EXCEPTION_FILTER lpTopLevelExceptionFilter);

BOOL PreventSetUnhandledExceptionFilter();

#endif

#include <string.h>
#include <stdio.h>

/*
void SPG_CONV SPG_LogTime(SPG_LOGTIME& LT, int UID, int Flag, int Code)
{
	if(LT.Etat==0) return;
	int n=LT.NumEvents++;//pas thread safe; un evenement peut en ecraser un autre; en toute rigueur il faut un InterlockIncrement
	SPG_LOGEVENT& E=LT.E[n&LT.MskEvents];
	{S_TimerCountType T; S_Timer_RDTSC(T); E.Tick=T;}
	E.UID=UID; E.Flag=Flag; E.Code=Code;
	return;
}
*/

int SPG_CONV SPG_LogTimeCreateUID(SPG_LOGTIME& LT, int Flag, DWORD Instance, char* File, char* Name, char* Descr)
{
	if(LT.Etat==0) return 0;
	CHECK(LT.NumEventTypes==LT.MaxEventTypes,"SPG_LogTimeAddType",return 0);

	for(int i=1;i<LT.NumEventTypes;i++)
	{
		if(
			(strcmp(LT.T[i].File,File)==0)&&
			((Name==0)||(strcmp(LT.T[i].Name,Name)==0))&&
			((Descr==0)||(strcmp(LT.T[i].Descr,Descr)==0))&&
			(LT.T[i].Instance==Instance)
			)
		{
#ifdef SPG_General_USELOGTIMETHREADID
			SPG_LogTimeThreadId(LT.T[i]);//			LT.T[i].TID=GetCurrentThreadId();//met a jour, les threads changent d'ID quand elles sont recréées
#endif
			return i;
		}
	}
	int n=LT.NumEventTypes++;
	SPG_EVENTTYPE& T=LT.T[n];
	T.Flag=Flag;
	T.Instance=Instance;
#ifdef SPG_General_USELOGTIMETHREADID
	T.TID=GetCurrentThreadId();
#endif
	strncpy(T.File,File,LT_STR);
	if(Name) strncpy(T.Name,Name,LT_STR);
	if(Descr) strncpy(T.Descr,Descr,LT_STR);

	return n;
}

int SPG_CONV SPG_LogTimeInit(SPG_LOGTIME& LT, int MaxEventTypes, int MaxEvents, int Flag)
{//penser à maintenir SPG_wLogTimeInit synchronisé (duplicata de code)
	SPG_ZeroStruct(LT);
	//{for(int i=1;i<31;i++) { LT.MaxEvents=(1<<i); if(LT.MaxEvents>=MaxEvents) break; }}
	LT.Flag=Flag;
	LT.MskEvents=MaxEvents-1; for(int i=0;i<5;i++) {LT.MskEvents|=(LT.MskEvents>>(1<<i));} 
	LT.MaxEvents=LT.MskEvents+1;
	LT.E=SPG_TypeAlloc(LT.MaxEvents,SPG_LOGEVENT,"SPG_LogTimeInit");
	LT.MaxEventTypes=MaxEventTypes;
	LT.T=SPG_TypeAlloc(LT.MaxEventTypes,SPG_EVENTTYPE,"SPG_LogTimeInit");
	LT.TimeScale=1.0f/(float)V_Max(Global.CPUClock,1);
	LT.NumEventTypes=1;//le type zero est 'unknown' et sert a reperer les erreur

#ifdef SPG_General_USELOGTIME_OSABNORMALTERMINATIONHANDLER
	SPG_LogTimeSetFilter(LT);
#endif
#if (defined(SPG_General_USELOGTIMETHREADID)||defined(SPG_DEBUGCONFIG))
	SPL_Init(LT.L,20,"LogTime");
	SPL_Exit(LT.L);
#endif


	{S_TimerCountType T; S_Timer_RDTSC(T); LT.AbsoluteStartTick=T;}
	return LT.Etat=-1;
}

void SPG_CONV SPG_LogTimeClear(SPG_LOGTIME& LT)
{
	if(LT.Etat==0) return;
	LT.NumEvents=0;
	{S_TimerCountType T; S_Timer_RDTSC(T); LT.AbsoluteStartTick=T;}
	return;
}

void SPG_CONV SPG_LogTimeClose(SPG_LOGTIME& LT)
{
#ifdef SPG_General_USELOGTIME_OSABNORMALTERMINATIONHANDLER
	if(LT.Flag&LOGTIMEFLAG_CATCHOSABNORMALTERMINATION)
	{
		CHECK( LogTime_UnhandledExceptionFilter!=SetUnhandledExceptionFilter((LPTOP_LEVEL_EXCEPTION_FILTER)Global.LT_DefaultOSAbnormalTerminationHandler),"SPG_LogTimeClose",;);
	}
#endif
#if (defined(SPG_General_USELOGTIMETHREADID)||defined(SPG_DEBUGCONFIG))
	SPL_Enter(LT.L,"SPG_LogTimeClose");
	SPL_Close(LT.L);
#endif
	LT.Etat=0; SPG_MemFree(LT.E); SPG_MemFree(LT.T); SPG_ZeroStruct(LT);
	return;
}

void SPG_CONV SPG_LogTime(SPG_LOGTIME& LT, int UID, int Flag, int Code)
{
	if(LT.Etat==0) return;
	CHECK(UID==0,"SPG_LogTime",return);
	int n=InterlockedIncrement((LONG*)&LT.NumEvents)-1; // n=(vLT).NumEvents++
	SPG_LogTimeCreateE(LT,E); 
	SPG_LogTimeGetTime(E); 
	E.UID=UID; 
	E.Flag=Flag; 
	E.Code=Code;
#ifdef SPG_General_USELOGTIMETHREADID
	SPG_LogTimeThreadId(E); 
#endif
	SPG_EVENTTYPE& T=LT.T[E.UID];
#ifdef SPG_General_USELOGTIMETHREADID
	if(T.Flag&LT_FLAG_IDCHECK)	{	DbgCHECK(E.TID!=T.TID,"SPG_LogTime");	}
	if(T.Flag&LT_FLAG_ENTRYCOUNTCHECK)	
	{
		if(E.Flag&LT_ENTER)	CHECK(InterlockedIncrement((LONG*)&T.EntryCount)!=1,"SPG_LogTime",;);	
		if(E.Flag&LT_EXIT)	CHECK(InterlockedDecrement((LONG*)&T.EntryCount)!=0,"SPG_LogTime",;);	
	}
#endif
	return;
}

void SPG_CONV SPG_LogTimeDraw(SPG_LOGTIME& LT, G_Ecran& E, C_Lib* CL)
{
//	LT.TimeScale=1.0f/(float)V_Max(Global.CPUClock,1);
	if(LT.Etat==0) return;
	return;
}

#define K (LT_ENTER|LT_REENT|LT_EXIT|LT_INIT|LT_CLOSE)

#ifdef SPG_General_USELOGTIMETHREADID

int SPG_CONV SPG_LogTimeCheckThreadIdConsistency(SPG_LOGTIME& LT)
{
	/*
#ifdef DebugList
	{for(int i=V_Max(LT.NumEvents-LT.MaxEvents,0);i<LT.NumEvents;i++)
	{
		SPG_LOGEVENT& E=LT.E[i&LT.MskEvents];
		SPG_EVENTTYPE& T=LT.T[E.UID];
		if(T.TID!=E.TID)
		{
			char F[256];
			if((E.Flag&K)==LT_ENTER)						{ sprintf(F,"ENTER \t%s\t%i",T.Name,E.Code); }
			else if((E.Flag&K)==   LT_REENT )			{ sprintf(F,"REENT \t%s\t%i",T.Name,E.Code); }
			else if((E.Flag&K)==   LT_EXIT  )				{ sprintf(F,"EXIT  \t%s\t%i",T.Name,E.Code); }
			else if((E.Flag&K)== ( LT_ENTER|LT_INIT  ))	{ sprintf(F,"ENTERI\t%s\t%i",T.Name,E.Code); }
			else if((E.Flag&K)== ( LT_REENT|LT_INIT  ))	{ sprintf(F,"REENTI\t%s\t%i",T.Name,E.Code); }
			else if((E.Flag&K)== ( LT_EXIT |LT_INIT  ))		{ sprintf(F,"EXITI \t%s\t%i",T.Name,E.Code); }
			else if((E.Flag&K)== ( LT_ENTER|LT_CLOSE ))	{ sprintf(F,"ENTERC\t%s\t%i",T.Name,E.Code); }
			else if((E.Flag&K)== ( LT_REENT|LT_CLOSE ))	{ sprintf(F,"REENTC\t%s\t%i",T.Name,E.Code); }
			else if((E.Flag&K)== ( LT_EXIT |LT_CLOSE ))		{ sprintf(F,"EXITC \t%s\t%i",T.Name,E.Code); }
			else										{ sprintf(F,"XXXXX \t%s\t%i",T.Name,E.Code); }

			SPG_List2S("SPG_LogTimeCheckThreadIdConsistency",F);
		}
	}}
#endif
	*/
	return -1;
}

#endif

/*
int SPG_CONV SPG_LogTimeSaveToFileL(SPG_LOGTIME& LT, char* FileName)
{
	if(LT.Etat==0) return 0;
	FILE* F=fopen(FileName,"wb+");
//	LT.TimeScale=1.0f/(float)V_Max(Global.CPUClock,1);
	CHECKTWO(F==0,"SPG_LogTimeSaveToFile",FileName,return 0);
	fprintf(F,"Tick per second\t%I64i\r\n",Global.CPUClock);
	fprintf(F,"Tick      \tDescr    \tType    \tName        \tCode\r\n");

	for(int i=V_Max(LT.NumEvents-LT.MaxEvents,0);i<LT.NumEvents;i++)
	{
		SPG_LOGEVENT& E=LT.E[i%LT.MaxEvents]; if(E.UID>LT.NumEventTypes) E.UID=0; SPG_EVENTTYPE& T=LT.T[E.UID];
		fprintf(F,"%0.10I64i\t%s\t",E.Tick-LT.AbsoluteStartTick,T.Descr);
		{
			if((E.Flag&K)==LT_ENTER)						{ fprintf(F,"ENTER \t%s\t",T.Name); }
			else if((E.Flag&K)==   LT_REENT )			{ fprintf(F,"REENT \t%s\t",T.Name); }
			else if((E.Flag&K)==   LT_EXIT  )				{ fprintf(F,"EXIT  \t%s\t",T.Name); }
			else if((E.Flag&K)== ( LT_ENTER|LT_INIT  ))	{ fprintf(F,"ENTERI\t%s\t",T.Name); }
			else if((E.Flag&K)== ( LT_REENT|LT_INIT  ))	{ fprintf(F,"REENTI\t%s\t",T.Name); }
			else if((E.Flag&K)== ( LT_EXIT |LT_INIT  ))		{ fprintf(F,"EXITI \t%s\t",T.Name); }
			else if((E.Flag&K)== ( LT_ENTER|LT_CLOSE ))	{ fprintf(F,"ENTERC\t%s\t",T.Name); }
			else if((E.Flag&K)== ( LT_REENT|LT_CLOSE ))	{ fprintf(F,"REENTC\t%s\t",T.Name); }
			else if((E.Flag&K)== ( LT_EXIT |LT_CLOSE ))		{ fprintf(F,"EXITC \t%s\t",T.Name); }
			else										{ fprintf(F,"XXXXX \t%s\t",T.Name); }
		}
		fprintf(F,"%X\r\n",E.Code);
	}
	fclose(F);
	return -1;
}
*/

int SPG_CONV SPG_LogTimeSaveToFile_MultiRow(SPG_LOGTIME& LT, char* FileName)
{
	if(LT.Etat==0) return 0;
	int* C0=SPG_TypeAlloc(LT.NumEventTypes,int,"LTSave"); int* T1=SPG_TypeAlloc(LT.NumEventTypes,int,"LTSave"); int* T2=SPG_TypeAlloc(LT.NumEventTypes,int,"LTSave"); __int64* T3=SPG_TypeAlloc(LT.NumEventTypes,__int64,"LTSave");

	int* sor=C0; {for(int n=0;n<LT.NumEventTypes;n++) {sor[n]=n;}}
	/*
	{
		int* Count=T1; memset(Count,0,LT.NumEventTypes*sizeof(int));
		int* Active=T2; memset(Active,0,LT.NumEventTypes*sizeof(int));
		{for(int i=V_Max(LT.NumEvents-LT.MaxEvents,0);i<LT.NumEvents;i++)
		{
			SPG_LOGEVENT& E=LT.E[i&LT.MskEvents]; if(E.UID>LT.NumEventTypes) E.UID=0; SPG_EVENTTYPE& T=LT.T[E.UID];
			if(E.Flag&LT_ENTER)	Active[E.UID]=1;
			if(E.Flag&LT_REENT)	Active[E.UID]=1;
			if((E.Flag&K)==LT_ENTER)	Active[E.UID]=4;
			if((E.Flag&K)==LT_REENT)	Active[E.UID]=4;
			{for(int n=0;n<LT.NumEventTypes;n++) {Count[n]+=Active[n];}}
			if(E.Flag&LT_EXIT)		Active[E.UID]=0;
		}}
		{for(int n0=0;n0<LT.NumEventTypes-2;n0++)
		{
			{for(int n1=n0+1;n1<LT.NumEventTypes;n1++) { if(Count[n0]<Count[n1]) { V_Swap(int,Count[n0],Count[n1]); V_Swap(int,sor[n0],sor[n1]); } }}
		}}
	}
	*/

	int* Origin=T1; memset(Origin,0,LT.NumEventTypes*sizeof(int));
	int* Active=T2; memset(Active,0,LT.NumEventTypes*sizeof(int));
	__int64* Total=T3; memset(Total,0,LT.NumEventTypes*sizeof(__int64));

	FILE* F=fopen(FileName,"wb+");
	CHECKTWO(F==0,"SPG_LogTimeSaveToFile",FileName,return 0);
	fprintf(F,"Tick per second\t%I64i\r\n",Global.CPUClock);
	fprintf(F,    "FILE  \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%s\t",nT.File); }}
	fprintf(F,"\r\nNAME  \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%s\t",nT.Name); }}
	fprintf(F,"\r\nDESCR \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%s\t",nT.Descr);}}
#ifdef SPG_General_USELOGTIMETHREADID
	fprintf(F,"\r\nTIME       \tTID   \tFLAG  \tNAME  \tDESCR \t CODE  \r\n");
#else
	fprintf(F,"\r\nTIME       \tFLAG  \tNAME  \tDESCR \t CODE  \r\n");
#endif
	int imin=V_Max(LT.NumEvents-LT.MaxEvents,0);
	int imax=LT.NumEvents;
	{for(int i=imin;i<imax;i++)
	{
		SPG_LOGEVENT& E=LT.E[i&LT.MskEvents]; if(E.UID>LT.NumEventTypes) E.UID=0; SPG_EVENTTYPE& T=LT.T[E.UID];
		{
#ifdef SPG_General_USELOGTIMETHREADID
			if((E.Flag&K)==LT_ENTER)					{ fprintf(F,"%0.11I64i\t%0.8X\tENTER \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)==   LT_REENT )			{ fprintf(F,"%0.11I64i\t%0.8X\tREENT \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)==   LT_EXIT  )			{ fprintf(F,"%0.11I64i\t%0.8X\tEXIT  \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_ENTER|LT_INIT  ))	{ fprintf(F,"%0.11I64i\t%0.8X\tENTERI\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_REENT|LT_INIT  ))	{ fprintf(F,"%0.11I64i\t%0.8X\tREENTI\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_EXIT |LT_INIT  ))	{ fprintf(F,"%0.11I64i\t%0.8X\tEXITI \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_ENTER|LT_CLOSE ))	{ fprintf(F,"%0.11I64i\t%0.8X\tENTERC\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_REENT|LT_CLOSE ))	{ fprintf(F,"%0.11I64i\t%0.8X\tREENTC\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_EXIT |LT_CLOSE ))	{ fprintf(F,"%0.11I64i\t%0.8X\tEXITC \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else										{ fprintf(F,"%0.11I64i\tXXXXX \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
#else
			if((E.Flag&K)==LT_ENTER)					{ fprintf(F,"%0.11I64i\tENTER \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)==   LT_REENT )			{ fprintf(F,"%0.11I64i\tREENT \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)==   LT_EXIT  )			{ fprintf(F,"%0.11I64i\tEXIT  \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_ENTER|LT_INIT  ))	{ fprintf(F,"%0.11I64i\tENTERI\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_REENT|LT_INIT  ))	{ fprintf(F,"%0.11I64i\tREENTI\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_EXIT |LT_INIT  ))	{ fprintf(F,"%0.11I64i\tEXITI \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_ENTER|LT_CLOSE ))	{ fprintf(F,"%0.11I64i\tENTERC\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_REENT|LT_CLOSE ))	{ fprintf(F,"%0.11I64i\tREENTC\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_EXIT |LT_CLOSE ))	{ fprintf(F,"%0.11I64i\tEXITC \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else										{ fprintf(F,"%0.11I64i\tXXXXX \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
#endif
		}
		{for(int n=0;n<LT.NumEventTypes;n++)
		{
			SPG_EVENTTYPE& nT=LT.T[sor[n]];

			if(E.UID==sor[n])
			{
				if(E.Flag&LT_ENTER) { Active[n]=1; Origin[n]=i%LT.MaxEvents; }
			}

			if(Active[n]) 
			{ 
					fprintf(F,"%0.10I64i\t",E.Tick-LT.E[Origin[n]].Tick); 
			}
			else 	{ fprintf(F,"\t"); }

			if(E.UID==sor[n]) 
			{
				if(E.Flag&LT_EXIT) { Total[n]+=E.Tick-LT.E[Origin[n]].Tick; Active[n]=0; }
			}
		}}
		fprintf(F,"\r\n");
	}}

	__int64 AT=LT.E[V_Max(imax-1,0)&LT.MskEvents].Tick-LT.E[imin&LT.MskEvents].Tick;
	fprintf(F,"Tick per second\t%I64i\r\n",Global.CPUClock);
	fprintf(F,    "FILE  \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%s\t",nT.File); }}
	fprintf(F,"\r\nNAME  \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%s\t",nT.Name); }}
	fprintf(F,"\r\nDESCR \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%s\t",nT.Descr);}}
	fprintf(F,"\r\nTHREAD \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%s\t",SPG_GetGlobalThreadName(nT.TID));}}
	fprintf(F,"\r\nTOTAL \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%0.11I64i\t",Total[n]);}}
	fprintf(F,"\r\nRATIO(%%) \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%0.2f\t",(100*(double)(Total[n]))/((double)(AT)));}}

	fclose(F);
	SPG_MemFree(C0); SPG_MemFree(T1); SPG_MemFree(T2); SPG_MemFree(T3);

#ifdef SPG_General_USELOGTIMETHREADID
	SPG_LogTimeCheckThreadIdConsistency(LT);
#endif
	return -1;
}

int SPG_CONV SPG_LogTimeSaveToFile_OneRow(SPG_LOGTIME& LT, char* FileName)
{
	if(LT.Etat==0) return 0;
	int* C0=SPG_TypeAlloc(LT.NumEventTypes,int,"LTSave"); int* T1=SPG_TypeAlloc(LT.NumEventTypes,int,"LTSave"); int* T2=SPG_TypeAlloc(LT.NumEventTypes,int,"LTSave"); __int64* T3=SPG_TypeAlloc(LT.NumEventTypes,__int64,"LTSave");

	int* sor=C0; {for(int n=0;n<LT.NumEventTypes;n++) {sor[n]=n;}}
	/*
	{
		int* Count=T1; memset(Count,0,LT.NumEventTypes*sizeof(int));
		int* Active=T2; memset(Active,0,LT.NumEventTypes*sizeof(int));
		{for(int i=V_Max(LT.NumEvents-LT.MaxEvents,0);i<LT.NumEvents;i++)
		{
			SPG_LOGEVENT& E=LT.E[i&LT.MskEvents]; if(E.UID>LT.NumEventTypes) E.UID=0; SPG_EVENTTYPE& T=LT.T[E.UID];
			if(E.Flag&LT_ENTER)	Active[E.UID]=1;
			if(E.Flag&LT_REENT)	Active[E.UID]=1;
			if((E.Flag&K)==LT_ENTER)	Active[E.UID]=4;
			if((E.Flag&K)==LT_REENT)	Active[E.UID]=4;
			{for(int n=0;n<LT.NumEventTypes;n++) {Count[n]+=Active[n];}}
			if(E.Flag&LT_EXIT)		Active[E.UID]=0;
		}}
		{for(int n0=0;n0<LT.NumEventTypes-2;n0++)
		{
			{for(int n1=n0+1;n1<LT.NumEventTypes;n1++) { if(Count[n0]<Count[n1]) { V_Swap(int,Count[n0],Count[n1]); V_Swap(int,sor[n0],sor[n1]); } }}
		}}
	}
	*/

	int* Origin=T1; memset(Origin,0,LT.NumEventTypes*sizeof(int));
	int* Active=T2; memset(Active,0,LT.NumEventTypes*sizeof(int));
	__int64* Total=T3; memset(Total,0,LT.NumEventTypes*sizeof(__int64));

	FILE* F=fopen(FileName,"wb+");
	CHECKTWO(F==0,"SPG_LogTimeSaveToFile",FileName,return 0);
	fprintf(F,"Tick per second\t%I64i\r\n",Global.CPUClock);
	fprintf(F,    "FILE  \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%s\t",nT.File); }}
	fprintf(F,"\r\nNAME  \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%s\t",nT.Name); }}
	fprintf(F,"\r\nDESCR \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%s\t",nT.Descr);}}
#ifdef SPG_General_USELOGTIMETHREADID
	fprintf(F,"\r\nTIME       \tTID   \tFLAG  \tNAME  \tDESCR \t CODE  \r\n");
#else
	fprintf(F,"\r\nTIME       \tFLAG  \tNAME  \tDESCR \t CODE  \r\n");
#endif
	int imin=V_Max(LT.NumEvents-LT.MaxEvents,0);
	int imax=LT.NumEvents;
	{for(int i=imin;i<imax;i++)
	{
		SPG_LOGEVENT& E=LT.E[i&LT.MskEvents]; if(E.UID>LT.NumEventTypes) E.UID=0; SPG_EVENTTYPE& T=LT.T[E.UID];
		{
#ifdef SPG_General_USELOGTIMETHREADID
			if((E.Flag&K)==LT_ENTER)					{ fprintf(F,"%0.11I64i\t%0.8X\tENTER \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)==   LT_REENT )			{ fprintf(F,"%0.11I64i\t%0.8X\tREENT \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)==   LT_EXIT  )			{ fprintf(F,"%0.11I64i\t%0.8X\tEXIT  \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_ENTER|LT_INIT  ))	{ fprintf(F,"%0.11I64i\t%0.8X\tENTERI\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_REENT|LT_INIT  ))	{ fprintf(F,"%0.11I64i\t%0.8X\tREENTI\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_EXIT |LT_INIT  ))	{ fprintf(F,"%0.11I64i\t%0.8X\tEXITI \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_ENTER|LT_CLOSE ))	{ fprintf(F,"%0.11I64i\t%0.8X\tENTERC\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_REENT|LT_CLOSE ))	{ fprintf(F,"%0.11I64i\t%0.8X\tREENTC\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_EXIT |LT_CLOSE ))	{ fprintf(F,"%0.11I64i\t%0.8X\tEXITC \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
			else										{ fprintf(F,"%0.11I64i\tXXXXX \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,E.TID,T.Name,T.Descr,E.Code); }
#else
			if((E.Flag&K)==LT_ENTER)					{ fprintf(F,"%0.11I64i\tENTER \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)==   LT_REENT )			{ fprintf(F,"%0.11I64i\tREENT \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)==   LT_EXIT  )			{ fprintf(F,"%0.11I64i\tEXIT  \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_ENTER|LT_INIT  ))	{ fprintf(F,"%0.11I64i\tENTERI\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_REENT|LT_INIT  ))	{ fprintf(F,"%0.11I64i\tREENTI\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_EXIT |LT_INIT  ))	{ fprintf(F,"%0.11I64i\tEXITI \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_ENTER|LT_CLOSE ))	{ fprintf(F,"%0.11I64i\tENTERC\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_REENT|LT_CLOSE ))	{ fprintf(F,"%0.11I64i\tREENTC\t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else if((E.Flag&K)== ( LT_EXIT |LT_CLOSE ))	{ fprintf(F,"%0.11I64i\tEXITC \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
			else										{ fprintf(F,"%0.11I64i\tXXXXX \t%s\t%s\t%X\t",E.Tick-LT.AbsoluteStartTick,T.Name,T.Descr,E.Code); }
#endif
		}
		{for(int n=0;n<LT.NumEventTypes;n++)
		{
			SPG_EVENTTYPE& nT=LT.T[sor[n]];

			if(E.UID==sor[n])
			{
				if(E.Flag&LT_ENTER) { Active[n]=1; Origin[n]=i%LT.MaxEvents; }
				if(E.Flag&LT_EXIT) { if(Active[n]) {Total[n]+=E.Tick-LT.E[Origin[n]].Tick; fprintf(F,"%0.10I64i",E.Tick-LT.E[Origin[n]].Tick);Active[n]=0; } else {}}
			}
		}}
		fprintf(F,"\r\n");
	}}

	__int64 AT=LT.E[V_Max(imax-1,0)&LT.MskEvents].Tick-LT.E[imin&LT.MskEvents].Tick;
	fprintf(F,"Tick per second\t%I64i\r\n",Global.CPUClock);
	fprintf(F,    "FILE  \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%s\t",nT.File); }}
	fprintf(F,"\r\nNAME  \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%s\t",nT.Name); }}
	fprintf(F,"\r\nDESCR \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%s\t",nT.Descr);}}
	fprintf(F,"\r\nTHREAD \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%s\t",SPG_GetGlobalThreadName(nT.TID));}}
	fprintf(F,"\r\nTOTAL \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%0.11I64i\t",Total[n]);}}
	fprintf(F,"\r\nRATIO(%%) \t\t\t\t\t"); {for(int n=1;n<LT.NumEventTypes;n++) { SPG_EVENTTYPE& nT=LT.T[sor[n]]; fprintf(F,"%0.2f\t",(100*(double)(Total[n]))/((double)(AT)));}}

	fclose(F);
	SPG_MemFree(C0); SPG_MemFree(T1); SPG_MemFree(T2); SPG_MemFree(T3);

#ifdef SPG_General_USELOGTIMETHREADID
	SPG_LogTimeCheckThreadIdConsistency(LT);
#endif
	return -1;
}








#endif



