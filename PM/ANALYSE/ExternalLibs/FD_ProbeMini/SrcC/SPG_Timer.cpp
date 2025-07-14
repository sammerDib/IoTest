
#include "SPG_General.h"

#ifdef SPG_General_USETimer

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <stdio.h>

#include <string.h>

#ifndef SPG_General_USEGlobal
typedef struct
{
	S_TimerCountType CPUClock;
} TIMER_Struct;

TIMER_Struct Global;

#endif

__int64 InitTimerStartClock;
LARGE_INTEGER InitTimerStartTime;

void SPG_CONV S_StartInitTimer()
{
	__int64 T;
	S_Timer_RDTSC(T);

	LARGE_INTEGER TDN;
	LARGE_INTEGER TDU;
	QueryPerformanceCounter(&TDN);
	QueryPerformanceCounter(&TDN);
	do
	{
		QueryPerformanceCounter(&TDU);
	} while (TDU.LowPart==TDN.LowPart);
	S_Timer_RDTSC(T);
	InitTimerStartClock=T;
	InitTimerStartTime=TDU;
	return;
}

void SPG_CONV S_StopInitTimer()
{
	CHECK(Global.CPUClock!=0,"Timer deja etalonne",return);

	__int64 SavedInitTimerStartClock=InitTimerStartClock;
	LARGE_INTEGER SavedInitTimerStartTime=InitTimerStartTime;

	S_StartInitTimer();

	LARGE_INTEGER Freq;
	QueryPerformanceFrequency(&Freq);

	Global.CPUClock=
		(
		(SavedInitTimerStartClock-InitTimerStartClock)*
		(*(__int64*)&Freq)
		)
		/
		(
		(*(__int64*)&SavedInitTimerStartTime)-
		(*(__int64*)&InitTimerStartTime)
		);
	return;
}

#ifdef SPG_General_USECaracF
void SPG_CONV S_PrintRatio(S_TIMER* T, int n)
{
	SPG_ArrayStackAlloc(char,Msg,1024);
	Msg[0]=0;
	S_GetRatio(T,n,Msg);
	SPG_ArrayStackCheck(Msg);
#ifdef SPG_General_USEGlobal
	MessageBox((HWND)Global.hWndWin,Msg,SPG_COMPANYNAME,0);
#else
	MessageBox(0,Msg,SPG_COMPANYNAME,0);
#endif
	return;
}

void SPG_CONV S_GetRatio(S_TIMER* T, int n, char* Msg)
{
#ifdef DebugTimer
	strcat(Msg,T[0].TimerName);
	strcat(Msg," ");
#else
	strcat(Msg,"Total: ");
#endif
	float RefTime;
	S_GetTimerTotalTime(T[0],RefTime);
	CF_GetString(Msg,RefTime,4);
	strcat(Msg,"s");
	for(int i=0;i<n;i++)
	{
	strcat (Msg,"\n");
#ifdef DebugTimer
	strcat(Msg,T[i].TimerName);
	strcat(Msg," ");
#endif
	if(T[i].StopsCount)
	{
		float CurTime;
		S_GetTimerTotalTime(T[i],CurTime);
		CF_GetString(Msg,100.0f*CurTime/RefTime,4);
		strcat(Msg,"%, ");
		CF_GetString(Msg,1000*CurTime/T[i].StopsCount,5);
		strcat(Msg,"ms/call");
	}
	SPG_ArrayStackAlloc(char,nCall,16);
	sprintf(nCall,"(%i)",T[i].StopsCount);
	SPG_ArrayStackCheck(nCall);
	strcat(Msg,nCall);
#ifdef DebugMem
	CHECK(strlen(Msg)>1023,"S_GetRatio",return);
#endif
	}
	return;
}
#else
#pragma SPGMSG(__FILE__,__LINE__,"S_PrintRatio needs CaracF")
#endif

/*
int StartInitTimer()
{

	Global.CPUClock=0;
}

int StopInitTimer()
{

}
*/
/*
void SPG_CONV S_CloseTimer(S_Timer& ST)
{
	ST.Etat=0;
	return;
}
*/
/*
void SPG_CONV S_StartTimer(int i)
{
	CHECK(V_IsBound(i,0,MaxTimer)==0,"Ce timer n'existe pas",return);
	CHECK((GT[i].Etat&S_USED)==0,"Timer non initialise",return);
	CHECK(GT[i].Etat&S_RUNNING,"Timer deja enclenche",return);
	GT[i].Etat|=S_RUNNING;
	__int64 T;
	__asm
	{
	push eax
	push edx
	_emit 0x0F
	_emit 0x31
	mov  DWORD PTR T,eax
	mov  DWORD PTR [T+4],edx
	pop edx
	pop eax
	}
	GT[i].StartTime=T;
	return;
}

void SPG_CONV S_StopTimer(int i)
{
	CHECK(V_IsBound(i,0,MaxTimer)==0,"Ce timer n'existe pas",return);
	CHECK((GT[i].Etat&S_USED)==0,"Timer non initialise",return);
	CHECK((GT[i].Etat&S_RUNNING)==0,"Timer deja arrete",return);
	GT[i].Etat&=~S_RUNNING;
	__int64 T;
	__asm
	{
	push eax
	push edx
	_emit 0x0F
	_emit 0x31
	mov  DWORD PTR T,eax
	mov  DWORD PTR [T+4],edx
	pop edx
	pop eax
	}
	GT[i].TotalCount+=T-GT[i].StartTime;
	GT[i].StopsCount++;
	return;
}

__int64 SPG_FASTCONV S_GetTimerRunningCount(int i)
{
	CHECK(V_IsBound(i,0,MaxTimer)==0,"Ce timer n'existe pas",return 0);
	__int64 T;
	__asm
	{
	push eax
	push edx
	_emit 0x0F
	_emit 0x31
	mov  DWORD PTR T,eax
	mov  DWORD PTR [T+4],edx
	pop edx
	pop eax
	}
	return T-GT[i].StartTime;
}

double SPG_CONV S_GetTimerRunningTime(int i)
{
	CHECK(V_IsBound(i,0,MaxTimer)==0,"Ce timer n'existe pas",return 0.0);
	CHECK(Global.CPUClock==0,"Timer non calibre",return 0.0);
	//CHECK(GT[i].StopsCount==0,"Timer non arrete",return 0.0);
	__int64 T;
	__asm
	{
	push eax
	push edx
	_emit 0x0F
	_emit 0x31
	mov  DWORD PTR T,eax
	mov  DWORD PTR [T+4],edx
	pop edx
	pop eax
	}
	return (double)(T-GT[i].StartTime)/Global.CPUClock;
}

__int64 SPG_FASTCONV S_GetTimerCount(int i)
{
	CHECK(V_IsBound(i,0,MaxTimer)==0,"Ce timer n'existe pas",return 0);
	return GT[i].TotalCount/GT[i].StopsCount;
}

double SPG_CONV S_GetTimerTime(int i)
{
	CHECK(V_IsBound(i,0,MaxTimer)==0,"Ce timer n'existe pas",return 0.0);
	CHECK(Global.CPUClock==0,"Timer non calibre",return 0.0);
	CHECK(GT[i].StopsCount==0,"Timer non arrete",return 0.0);
	return (double)GT[i].TotalCount/Global.CPUClock/GT[i].StopsCount;
}

void SPG_CONV S_ResetTimer(int i)
{
	CHECK(V_IsBound(i,0,MaxTimer)==0,"Ce timer n'existe pas",return);
	GT[i].TotalCount=0;
	GT[i].StopsCount=0;
	return;
}
*/

/*
#ifdef DebugTimer
void SPG_CONV S_PrintRatio(int RefTimer)
{
	char Msg[4*MaxName];
	//int total=0;

	sprintf(Msg,"Processeur %dMHz\n",Global.CPUClock/1000000);
	for(int i=0;i<MaxTimer;i++)
	{
		if (GT[i].Etat&S_USED)
		{
			char LMsg[2*MaxName];
			sprintf(LMsg,"%s\t%.3f%\t",TimerName+MaxName*i,100*(float)GT[i].TotalCount/GT[RefTimer].TotalCount);
			if (Global.CPUClock) 
			{
				CF_GetString(LMsg,(float)(1000*S_GetTimerTime(i)),4);
				strcat(LMsg,"ms");
			}
			strcat(Msg,LMsg);
			strcat(Msg,"\n");
			//total += MemSize[i];
		}
	}
	{
			//char AMsg[128];
			//sprintf(AMsg,"TOTAL: %d octets",total);
			//strcat(Msg,AMsg);
	}

	SPG_List(Msg);
}

void SPG_CONV S_PrintAbsoluteTime()
{
	CHECK(Global.CPUClock==0,"Timer non calibre",return);

	char Msg[4*MaxName];
	//int total=0;

	//Msg[0]=0;
	sprintf(Msg,"Processeur %dMHz\n",Global.CPUClock/1000000);
	for(int i=0;i<MaxTimer;i++)
	{
		if (GT[i].Etat&S_USED)
		{
			char LMsg[2*MaxName];
			sprintf(LMsg,"%s ",TimerName+MaxName*i);
			CF_GetString(LMsg,(float)(1000*S_GetTimerTime(i)),4);
			strcat(Msg,LMsg);
			strcat(Msg,"ms");
			strcat(Msg,"\n");
			//total += MemSize[i];
		}
	}

	SPG_List(Msg);
}
#endif
*/


#endif
