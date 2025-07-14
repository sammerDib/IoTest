
#include "SPG_General.h"

#ifdef SPG_General_USESpinLock

#include "SPG_Includes.h"

#include "BreakHook.h"

#ifdef SPG_General_USEWindows
#include <windows.h>
#endif
#include <string.h>
#include <stdio.h>

#define SPINLOCK_READY 0x99
#define SPINLOCK_OWNED 0xBB
#define SPINLOCK_UNINIT 0xCC
//efine SPINLOCK_DEBUG 0xDD

#ifdef DebugMem

void SPG_CONV SPL_Init(SPINLOCK& L, int msTimeout, const char* Owner)
{
	L.State = SPINLOCK_OWNED;
	L.msTimeout = msTimeout;
	strcpy(L.Owner, Owner);
#ifdef FDE
#ifdef DebugMem
	L.OwnerThreadID = GetCurrentThreadId();
	S_InitTimer(L.STT, Owner);
	S_InitTimer(L.SLT, Owner);
	S_StartTimer(L.STT);
	S_StartTimer(L.SLT);
#endif
#else
	L.OwnerThreadID = GetCurrentThreadId();
	InitializeCriticalSection(&L.critsec);
	EnterCriticalSection(&L.critsec);
#endif	//FDE
	return;
}

#else

void SPG_CONV SPL_Init_r(SPINLOCK& L, int msTimeout)
{
	L.State=SPINLOCK_OWNED;
	L.msTimeout=msTimeout;
	return;
}

#endif

void SPG_CONV SPL_Close(SPINLOCK& L)
{
	DbgCHECK(L.State!=SPINLOCK_OWNED,"SPL_Close");
#ifdef FDE
#ifdef DebugMem
	S_StopTimer(L.SLT);
	S_StopTimer(L.STT);
#endif
	L.State=SPINLOCK_UNINIT;
#ifdef DebugMem
	S_CloseTimer(L.STT);
	S_CloseTimer(L.SLT);
#endif
	return;
#else
	DeleteCriticalSection(&L.critsec);
	L.State = SPINLOCK_UNINIT;
#ifdef DebugMem
	strcpy_s(L.Owner, "");
	L.OwnerThreadID = -1;
#endif
#endif
}

#ifdef DebugMem

/*
int SPG_CONV SPL_TryEnter(SPINLOCK& L, char* Owner)
{
	long S=SPINLOCK_OWNED;
	S=InterlockedExchange(&L.State,S);
	if(S!=SPINLOCK_READY) { L.State=SPINLOCK_DEBUG;BreakHook(); }
	return SPL_OK;
}
*/

int SPG_CONV SPL_Enter(SPINLOCK& L, const char* Owner)
{
#ifdef FDE
	long S=SPINLOCK_OWNED;
	S=InterlockedExchange(&L.State,S);
	CHECKTWO((S!=SPINLOCK_OWNED)&&(S!=SPINLOCK_READY),"SPL_Enter",Owner,InterlockedExchange(&L.State,S));
	int i; int t=0;
	for(i=-16;(t<L.msTimeout)&&(S==SPINLOCK_OWNED);i++)
	{
		SPG_Sleep(V_Max(0,i)); t+=V_Max(0,i);
		S=InterlockedExchange(&L.State,S);
	}
	if(S!=SPINLOCK_READY) 
	{
		SPINLOCK H=L;
#ifdef SPINLOCK_DEBUG
		L.State=SPINLOCK_DEBUG;
		BreakHook();
#endif
#ifdef DebugList
		DWORD CID=GetCurrentThreadId();
		char M[512]; sprintf(M,"SPL_Enter: return SPL_TIMEOUT\nRequested by [%i]%s:%s\nOwned by [%i]%s:%s",CID,SPG_GetGlobalThreadName(CID),Owner,L.OwnerThreadID,SPG_GetGlobalThreadName(L.OwnerThreadID),L.Owner); 
		BreakHook();
		SPG_List(M);
#endif

		return SPL_TIMEOUT;
	}
	S_StartTimer(L.SLT);
#else
	DbgCHECK(L.State == SPINLOCK_UNINIT, "SPL_Enter");
	EnterCriticalSection(&L.critsec);
	strcpy_s(L.Owner,Owner);
	L.State = SPINLOCK_OWNED;
#endif
	L.OwnerThreadID=GetCurrentThreadId();
	return SPL_OK;
}

#else

int SPG_CONV SPL_Enter_r(SPINLOCK& L)
{
	long S=SPINLOCK_OWNED;
	S=InterlockedExchange(&L.State,S);
	CHECK((S!=SPINLOCK_OWNED)&&(S!=SPINLOCK_READY),"SPL_Enter",InterlockedExchange(&L.State,S));
	int i; int t=0;
	for(i=0;(t<=L.msTimeout)&&(S==SPINLOCK_OWNED);i++)
	{
		SPG_Sleep(i); t+=i;
		S=InterlockedExchange(&L.State,S);
	}
	if(S!=SPINLOCK_READY) return SPL_TIMEOUT;
	return SPL_OK;
}

#endif

void SPG_CONV SPL_Exit(SPINLOCK& L)
{
#ifdef DebugMem
	DbgCHECK(L.OwnerThreadID!=GetCurrentThreadId(),"SPL_Exit");
#endif

#ifdef FDE
#ifdef DebugMem
	S_StopTimer(L.SLT);
#endif

	long S=SPINLOCK_READY;
	S=InterlockedExchange(&L.State,S);
	DbgCHECK((S!=SPINLOCK_OWNED),"SPL_Exit");

#ifdef DebugMem
	if(Global.CPUClock&&(L.msTimeout>0))
	{
		double T;
		S_GetTimerRunningTime(L.SLT,T);
		T*=1000;
		char Msg[256]; sprintf(Msg,"%s : spent:%.3fms timeout:%.3fms",L.Owner,(float)T,(float)L.msTimeout);
		DbgCHECKTWO(T > L.msTimeout,"SPL_Exit : Warning lock time exceed time out",Msg);
	}
#endif
	return;
#else
	DbgCHECK((L.State != SPINLOCK_OWNED), "SPL_Exit");
#ifdef DebugMem
	strcpy_s(L.Owner, "");
	L.OwnerThreadID = 0;
#endif
	L.State = SPINLOCK_READY;
	LeaveCriticalSection(&L.critsec);
#endif
}

#else

#pragma SPGMSG(__FILE__,__LINE__,"SPINLOCK INHIBITED - WARNING FOR MULTITHREAD COMPILATION - SPINLOCK INHIBITED")


#endif
