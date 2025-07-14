
#ifdef SPG_General_USETimer

#define MaxName 64
#define S_TimerCountType __int64

typedef struct 
{
	int Etat;
#ifdef DebugTimer
	char TimerName[MaxName];
#endif
	int StopsCount;
	S_TimerCountType TotalCount; 
	S_TimerCountType StartTime; 
} S_TIMER;

#include "SPG_Timer.agh"

#define S_USED 1
#define S_RUNNING 2

//FDE #ifdef TimerPentium
#ifndef _WIN64
#define S_Timer_RDTSC(T) {__asm push eax __asm push edx __asm _emit 0x0F __asm _emit 0x31 __asm mov  DWORD PTR T,eax __asm mov  DWORD PTR [T+4],edx __asm pop edx __asm pop eax}
#else
//FDE #define S_Timer_RDTSC(T) QueryPerformanceCounter((LARGE_INTEGER*)&T)
#define S_Timer_RDTSC(T) QueryPerformanceCounter((LARGE_INTEGER*)&T)
#endif 

#define S_CheckTimer(STimer) DbgCHECK((STimer.Etat&S_USED)==0,"Timer "#STimer" non initialise")

//syntax error : '.' = utiliser init au lieu de create
#define S_CreateTimer(Variable,Name) S_TIMER Variable;S_InitTimer(Variable,Name)

#ifdef DebugTimer
#define S_InitTimer(Variable,Name) memset(&Variable,0,sizeof(S_TIMER));Variable.Etat=S_USED;strcpy(Variable.TimerName,Name)
#else
#define S_InitTimer(Variable,Name) memset(&Variable,0,sizeof(S_TIMER));Variable.Etat=S_USED
#endif

#define S_CloseTimer(Variable) memset(&Variable,0,sizeof(S_TIMER))

#define S_StartTimer(STimer) {S_CheckTimer(STimer);DbgCHECK(STimer.Etat&S_RUNNING,"Timer "#STimer" deja enclenche");STimer.Etat|=S_RUNNING;S_TimerCountType MACRO_T;S_Timer_RDTSC(MACRO_T);STimer.StartTime=MACRO_T;}
#define S_StopTimer(STimer) {S_CheckTimer(STimer);DbgCHECK((STimer.Etat&S_RUNNING)==0,"Timer "#STimer" deja arrete");STimer.Etat&=~S_RUNNING;S_TimerCountType MACRO_T;S_Timer_RDTSC(MACRO_T);STimer.TotalCount+=MACRO_T-STimer.StartTime;STimer.StopsCount++;}
#define S_RestartTimer(STimer) {S_CheckTimer(STimer);S_TimerCountType MACRO_T;S_Timer_RDTSC(MACRO_T);STimer.StartTime=MACRO_T;}
//count est en tops d'horloge (int)
#define S_GetTimerRunningCount(STimer,Resultat) {S_CheckTimer(STimer);S_TimerCountType MACRO_T;S_Timer_RDTSC(MACRO_T);Resultat=MACRO_T-STimer.StartTime;}
//time est en secondes (double)
#define S_PRIVATE_GetTimerRunningTime(STimer,Resultat) S_CheckTimer(STimer);DbgCHECK(Global.CPUClock==0,"Timer CPU RDTSC non calibre");S_TimerCountType MACRO_T;S_Timer_RDTSC(MACRO_T);Resultat=(double)(MACRO_T-STimer.StartTime)/Global.CPUClock;
#define S_GetTimerRunningTime(STimer,Resultat) {S_PRIVATE_GetTimerRunningTime(STimer,Resultat);}
#define S_GetTimerRunningTimeAndRestart(STimer,Resultat) {DbgCHECK((STimer.Etat&S_RUNNING)==0,"Timer "#STimer" non demarre");S_PRIVATE_GetTimerRunningTime(STimer,Resultat);STimer.StartTime=MACRO_T;}

#define S_GetTimerTotalCount(STimer,Resultat) {S_CheckTimer(STimer);DbgCHECK(STimer.Etat&S_RUNNING,"Timer "#STimer" non arrete");DbgCHECK(STimer.StopsCount==0,"Timer "#STimer" Zero intervalles");Resultat=STimer.TotalCount;}
#define S_GetTimerTotalTime(STimer,Resultat) {S_CheckTimer(STimer);DbgCHECK(STimer.Etat&S_RUNNING,"Timer "#STimer" non arrete");DbgCHECK(STimer.StopsCount==0,"Timer "#STimer" Zero intervalles");Resultat=(double)STimer.TotalCount/Global.CPUClock;}
#define S_GetTimerCount(STimer,Resultat) {S_CheckTimer(STimer);DbgCHECK(STimer.Etat&S_RUNNING,"Timer "#STimer" non arrete");DbgCHECK(STimer.StopsCount==0,"Timer "#STimer" Zero intervalles");Resultat=STimer.TotalCount/STimer.StopsCount;}
#define S_GetTimerTime(STimer,Resultat) {S_CheckTimer(STimer);DbgCHECK(STimer.Etat&S_RUNNING,"Timer "#STimer" non arrete");DbgCHECK(STimer.StopsCount==0,"Timer "#STimer" Zero intervalles");Resultat=(double)STimer.TotalCount/Global.CPUClock/STimer.StopsCount;}
#define S_ResetTimer(STimer) {S_CheckTimer(STimer);STimer.TotalCount=0;STimer.StopsCount=0;}

#define S_IsOK(STimer) (STimer.Etat&S_USED)
#define S_IsStarted(STimer) (STimer.Etat&S_RUNNING)

#ifdef DebugBugSearch
#define SPG_ChronoCall(FCT,PARAMS,Console) SPG_VerboseCall(FCT##PARAMS)
#else
#ifdef DebugTimer
#define SPG_ChronoCall(FCT,PARAMS,Console) {Console_Add(Console,#FCT);S_CreateTimer(ChronoCall,"ChronoCall");S_StartTimer(ChronoCall);FCT##PARAMS;S_StopTimer(ChronoCall);char TimeResult[64];strcpy(TimeResult," ");float TimeVal=0;S_GetTimerTime(ChronoCall,TimeVal);CF_GetString(TimeResult,1000.0*TimeVal,4);strcat(TimeResult,"ms");Console_AddOnSameLine(Console,TimeResult);}
//#define SPG_ChronoCall(FCT,Console) {Console_Add(Console,#FCT);DoEvents(SPG_DOEV_ALL);FCT;}
#else
#define SPG_ChronoCall(FCT,PARAMS,Console) FCT##PARAMS
#endif
#endif

#else

#define S_InitTimer(Variable,Name)
#define S_CloseTimer(Variable)
#define S_StartTimer(STimer)
#define S_StopTimer(STimer)
#define S_RestartTimer(STimer)
#define S_ResetTimer(STimer)

#endif

