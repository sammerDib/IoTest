
#include "SPG_General.h"
#ifdef SPG_General_USELOOPTHREAD

#include "SPG_Includes.h"

#include "BreakHook.h"

//nclude "SPG_LoopThread.h"
#include "SPG_SysInc.h"

#include <memory.h>
#include <string.h>
#include <process.h>    /* _beginthread, _endthread */
#include <stdio.h>    /* _beginthread, _endthread */

#include "BreakHook.h"

#define tSYNC 0x80
#define tINIT 0x200
#define tCLOS 0x300
#define tDOEV 0x1000

typedef enum
{
	LT_Invalid,
	LT_Waiting,
	LT_RunningInit,
	LT_RunningLoop,
	LT_RunningClose
} LOOPTHREADSTATUS;

typedef enum
{
	LM_Invalid,
	LM_Ready,		//wait was successfull
	LM_LoopRunning //init,loop ou close
} LOOPMAINSTATUS;

//verifier reentrance et tableaux statiques

typedef struct SPG_LOOPTHREADSTATE
{
//##### THREAD #####

	volatile LOOPTHREADSTATUS thStatus;
	HANDLE hThread;
	unsigned int thThreadID;
	volatile int thLoopCount;
	
	int InitReturnCode;
	int LoopReturnCode;
	int CloseReturnCode;

	//thread event
	HANDLE evWaiting;

#ifdef SPG_General_USEWLTG_GLOBAL
	int thLTG;//LogTimeUID
	int thLTG_Wait;//LogTimeUID
#endif

//##### MAIN #####

	volatile LOOPMAINSTATUS mStatus;
#ifdef DebugList
	unsigned int mThreadID;
#endif

	//main application event
	HANDLE evExitThread;// WAIT_OBJECT_0 + 0
	HANDLE evLoopThread;// WAIT_OBJECT_0 + 1

	volatile int mLoopCount;
	volatile int mReenterCount;

	SPG_LOOPTHREADFCT Init;
	SPG_LOOPTHREADFCT Loop;
	SPG_LOOPTHREADFCT Close;

	void* User;
	int ThreadOrder;

	int mWaitms;
	int Threaded;

#ifdef SPG_General_USEWLTG_GLOBAL
	int mLTG;//LogTimeUID
	int mLTG_Wait;//LogTimeUID
#endif

#ifdef DebugList

//"It works in VC6, but I can only get it to display the first 9 characters."
#define LTNAME 8

	char Name[LTNAME];

	char mString[256];

	char ThString[256];

#endif

} SPG_LOOPTHREADSTATE;

//efine THWAIT 10000 //INFINITE

#ifdef DebugList

//############################### DEBUG ###################################

void SPG_CONV LoopThreadStatus(SPG_LOOPTHREADSTATE& LT, char* Msg)
{
	SPG_ArrayStackAllocZ(char,MS,32); SPG_ArrayStackAllocZ(char,TS,32);
	switch(LT.mStatus)
	{
	case LM_Invalid:
		strcpy(MS,"LM_Invalid");
		break;
	case LM_Ready:
		strcpy(MS,"LM_Ready");
		break;
	case LM_LoopRunning:
		strcpy(MS,"LM_LoopRunning");
		break;
	}
	switch(LT.thStatus)
	{
	case LT_Invalid:
		strcpy(TS,"LT_Invalid");
		break;
	case LT_Waiting:
		strcpy(TS,"LT_Waiting");
		break;
	case LT_RunningInit:
		strcpy(TS,"RunningInit");
		break;
	case LT_RunningLoop:
		strcpy(TS,"RunningLoop");
		break;
	case LT_RunningClose:
		strcpy(TS,"RunningClose");
		break;
	}
	sprintf(Msg,"%s\nMainStatus:%s(%i) - ThreadStatus:%s(%i)\nInit:%i Loop:%i Close:%i\nMainLoopCount:%i - ThreadLoopCount:%i",LT.Name,MS,LT.mThreadID,TS,LT.thThreadID,LT.InitReturnCode,LT.LoopReturnCode,LT.CloseReturnCode,LT.mLoopCount,LT.thLoopCount);
	SPG_ArrayStackCheck(MS);SPG_ArrayStackCheck(TS);
}

void SPG_CONV SPG_LoopThreadTimeFrozen(SPG_LOOPTHREADSTATE& LT, int r, char* Msg)
{
#ifdef DebugList
				SPG_ArrayStackAllocZ(char,Msg0,512); LoopThreadStatus(LT,Msg0); SPG_ArrayStackAllocZ(char,Msg1,768); 
				sprintf(Msg1,"SPG_LoopThreadClose:%s\nMsgWaitForMultipleObjectsEx aborded code %i",Msg0,r);
				//BreakHook();
				SPG_List(Msg1);
				SPG_ArrayStackCheck(Msg0);SPG_ArrayStackCheck(Msg1);

#endif
#ifdef SPG_General_USEWLTG_GLOBAL
	char F[512];
	sprintf(F,"%sLogTimeFrozen_%s_%s.txt",Global.LogDir,LT.Name,Msg);

	SPG_LogTimeSaveToFile_OneRow(Global.wt,F);
#endif
	return;
}

//############################### DEBUG ###################################

#endif


//############################### SATELLITE THREAD ###################################


static unsigned int __stdcall LoopThread(void* pLT)
{	//la 'loopthread'
	CHECK(pLT==0,"LoopThread",_endthreadex( 0 )); // verifier qu'on compile avec un runtime multithreaded sinon 'endthreadex' : undeclared identifier
	SPG_LOOPTHREADSTATE& LT=*(SPG_LOOPTHREADSTATE*)pLT;

	//thread INIT
	LT.thStatus=LT_RunningInit;
	//Set ThreadID thLTG, thLTG_Wait
	LTG_EnterI(LT,thLTG,0);
	if(LT.Init)
	{
		CHECK((LT.InitReturnCode=LT.Init(LT.User,LT.ThreadOrder))==0,"LoopThread", LT.thStatus=LT_Invalid;SetEvent(LT.evWaiting);_endthreadex( 0 ));
	}
	LTG_ExitI(LT,thLTG,0);

	typedef enum { LT_cmdWait, LT_cmdLoop, LT_cmdExit, LT_cmdDoEvents } SPG_LOOPTHREADCOMMAND;

	SPG_StackAlloc(SPG_LOOPTHREADCOMMAND,c);c=LT_cmdWait;

	do//boucle externe sur le SetEvent/Wait/LoopCallback
	{//thread LOOP
		LT.thStatus=LT_Waiting;
		SetEvent(LT.evWaiting); 
		LTG_EnterEH(LT,thLTG_Wait,0)
		do//gestion 'interne' du MsgWaitForMultipleObjectsEx
		{//thread LOOP
			int r = MsgWaitForMultipleObjectsEx(2,&LT.evExitThread,INFINITE,0,MWMO_ALERTABLE);//QS_SENDMESSAGE//QS_ALLINPUT
			//if( r == (WAIT_FAILED )			{ c=LT_cmdExit; }
			if(      r == (WAIT_OBJECT_0 + 0) )	{ c=LT_cmdExit; }
			else if( r == (WAIT_OBJECT_0 + 1) )	{ c=LT_cmdLoop; }
			else if( r == (WAIT_OBJECT_0 + 2) )	{ c=LT_cmdDoEvents;	}
			//else if( r == (WAIT_TIMEOUT) )	{ c=LT_cmdExit; }
			else if( r == (WAIT_IO_COMPLETION) ){ c=LT_cmdWait; }
			else
			{//erreur (-1) ou timeout (258)
#ifdef DebugList
				SPG_LoopThreadTimeFrozen(LT,r,"LoopThread");
#endif
#ifdef SPG_DEBUGCONFIG
				c=LT_cmdWait;//reboucle sur l'erreur pour permettre le debogage
#else
				c=LT_cmdExit;
#endif
			}

			if(c==LT_cmdDoEvents) { LTG_Reent(LT,thLTG_Wait,tDOEV); /*BreakHook();*/ DoEvents(SPG_DOEV_READ_WIN_EVENTS); LTG_Reent(LT,thLTG_Wait,tDOEV); }
		} while( (c!=LT_cmdExit) && (c!=LT_cmdLoop) );
		LTG_ExitEH(LT,thLTG_Wait,0)

		if(c == LT_cmdLoop)
		{
			LTG_EnterEH(LT,thLTG,LT.thLoopCount);
			LT.thStatus=LT_RunningLoop;
			DbgCHECK(LT.Loop==0,"LoopThread");
			if(LT.Loop) LT.LoopReturnCode=LT.Loop(LT.User,LT.ThreadOrder);  //appelle la fonction Loop une fois
			LT.thLoopCount++;
			DbgCHECK(LT.thLoopCount!=LT.mLoopCount,"LoopThread");
			LTG_ExitEH(LT,thLTG,LT.thLoopCount);
		}
	} while( c != LT_cmdExit );

	LT.thStatus=LT_RunningClose;
	LTG_EnterC(LT,thLTG,0);
	if(LT.Close) 
	{
		LT.CloseReturnCode=LT.Close(LT.User,LT.ThreadOrder);// thread CLOSE
	}
	LTG_ExitC(LT,thLTG,0);
	LT.thStatus=LT_Invalid;
	_endthreadex( 0 );//le return n'est pas vraiment exécutée ...
    return 0;
}

//############################### SATELLITE THREAD ###################################

//############################### MAIN THREAD ###################################
#ifdef DebugList
#define LoopThreadCheck(LT,f) CHECK(LT.thStatus==LT_Invalid,f,return 0);CHECK(LT.mStatus==LM_Invalid,f,return 0);CHECK(GetCurrentThreadId()!=LT.mThreadID,f,return 0);
#else
#define LoopThreadCheck(LT,f) CHECK(LT.thStatus==LT_Invalid,f,return 0);CHECK(LT.mStatus==LM_Invalid,f,return 0);
#endif

int SPG_CONV SPG_WaitLoop(SPG_LOOPTHREADSTATE* pLT)
{//programme principal
	SPG_LOOPTHREADSTATE& LT=*(SPG_LOOPTHREADSTATE*)pLT;

//fdef DebugList
//	DbgCHECK(LT.mThreadID!=GetCurrentThreadId(),"SPG_LoopThread");
//ndif
	DbgCHECK(++LT.mReenterCount != 1,"SPG_WaitLoop");

	CHECK(LT.thStatus==LT_Invalid,"SPG_LoopThread",return 0);
	CHECK(LT.mStatus!=LM_LoopRunning,"SPG_WaitLoop",return 0;)
	if(LT.Threaded)
	{
		typedef enum { LW_cmdWait, LW_cmdExit, LW_cmdDoEvents } SPG_LOOPWAITCOMMAND;

		SPG_StackAlloc(SPG_LOOPWAITCOMMAND,c); c=LW_cmdWait;
		LTG_EnterEH(LT,mLTG_Wait,0);
		do//gestion 'interne' du MsgWaitForMultipleObjectsEx
		{
			int r = MsgWaitForMultipleObjectsEx(1,&LT.evWaiting,LT.mWaitms,0,MWMO_ALERTABLE);//QS_SENDMESSAGE //QS_ALLINPUT
			//if(	 r == (WAIT_FAILED) )		{ c=LW_cmdExit; }
			if(      r == (WAIT_OBJECT_0 + 0) )	{ c=LW_cmdExit; DbgCHECK(LT.mLoopCount!=LT.thLoopCount,"SPG_WaitLoop"); DbgCHECK(LT.thStatus!=LT_Waiting,"SPG_WaitLoop"); }
			else if( r == (WAIT_OBJECT_0 + 1) )	{ c=LW_cmdDoEvents;	}
			//se if( r == (WAIT_TIMEOUT) )		{ c=LW_cmdExit; }
			else if( r == (WAIT_IO_COMPLETION) ){ c=LW_cmdWait; }
			else
			{//erreur (-1) ou timeout (258)
#ifdef DebugList
				SPG_LoopThreadTimeFrozen(LT,r,"SPG_WaitLoop");
#endif
#ifdef SPG_DEBUGCONFIG
				c=LW_cmdWait;//reboucle sur l'erreur pour permettre le debogage
#else
				c=LW_cmdExit;
#endif
			}
			//si le traitement du doevents est bloquant (menu) et que la thread satellite se termine le cpu tombe a zero
			//si on ne traite pas le doevents et que la thread satellite sollicite un sendmessage le cpu tombe a zero et le soft reste bloque (deadlock)
			if(c == LW_cmdDoEvents) { LTG_Reent(LT,mLTG_Wait,tDOEV); /*BreakHook();*/ DoEvents(SPG_DOEV_READ_WIN_EVENTS); LTG_Reent(LT,mLTG_Wait,tDOEV); }

		} while( c != LW_cmdExit );
		//DbgCHECK(c!=LW_cmdExit,"SPG_WaitLoop");
		LTG_ExitEH(LT,mLTG_Wait,0);
	}
	else
	{
		LTG_EnterEH(LT,thLTG,tSYNC);
		DbgCHECK(LT.Loop==0,"LoopThread");
		if(LT.Loop) LT.LoopReturnCode=LT.Loop(LT.User,LT.ThreadOrder);
		LT.thLoopCount++;
		DbgCHECK(LT.thLoopCount!=LT.mLoopCount,"LoopThread");
		LTG_ExitEH(LT,thLTG,tSYNC);
	}
	LT.mStatus=LM_Ready;

	DbgCHECK(--LT.mReenterCount != 0,"SPG_WaitLoop");
	return -1;
}

int SPG_CONV SPG_LoopThread(SPG_LOOPTHREADSTATE* pLT)
{//programme principal
	SPG_LOOPTHREADSTATE& LT=*(SPG_LOOPTHREADSTATE*)pLT;

#ifdef DebugList
	DbgCHECK((LT.mThreadID!=GetCurrentThreadId())&&((LT.thStatus!=LT_Waiting)||(LT.mStatus!=LM_Ready)),"SPG_LoopThread");
#endif

	CHECK(LT.thStatus==LT_Invalid,"SPG_LoopThread",return 0);


	if(LT.mStatus==LM_LoopRunning) SPG_WaitLoop(pLT); 

	LTG_EnterEH(LT,mLTG,LT.mLoopCount);
	DbgCHECK(++LT.mReenterCount != 1,"SPG_LoopThread");


	CHECK(LT.mStatus!=LM_Ready,"SPG_LoopThread",return 0);
	CHECK(LT.thStatus!=LT_Waiting,"SPG_LoopThread",return 0);
	CHECK(LT.mLoopCount!=LT.thLoopCount,"SPG_LoopThread",return 0);

	LT.mStatus=LM_LoopRunning; LT.mLoopCount++;
	if(LT.Threaded) SetEvent(LT.evLoopThread); 

	DbgCHECK(--LT.mReenterCount != 0,"SPG_LoopThread");
	LTG_ExitEH(LT,mLTG,LT.mLoopCount);
	return -1;
}

SPG_LOOPTHREADSTATE* SPG_CONV SPG_LoopThreadInit(SPG_LOOPTHREADFCT Init, SPG_LOOPTHREADFCT Loop, SPG_LOOPTHREADFCT Close, void* User, int ThreadOrder, int Threaded, int mWaitms, char* Name)
{//programme principal
	SPG_LOOPTHREADSTATE* pLT=SPG_TypeAlloc(1,SPG_LOOPTHREADSTATE,"SPG_LoopThreadInit");
	SPG_LOOPTHREADSTATE& LT=*(SPG_LOOPTHREADSTATE*)pLT;
	LT.Init=Init; LT.Loop=Loop; LT.Close=Close; LT.User=User; LT.ThreadOrder=ThreadOrder; LT.Threaded=Threaded; LT.mWaitms=mWaitms;
#ifdef DebugList
	_snprintf(LT.Name,LTNAME,"%s:%i",Name,ThreadOrder);
#endif

#ifdef DebugList
		LT.mThreadID=GetCurrentThreadId();
#endif

#ifdef SPG_General_USEWLTG_GLOBAL
	char Descr[LT_STR];
	sprintf(Descr,"mL:%s_%i",Name,ThreadOrder); LTG_CreateDescr(LT,LT_FLAG_NOCHECK  ,mLTG,0,Descr); //appelé alternativement depuis Main et Satellite, ne peut pas être threadsensitive
	sprintf(Descr,"mW:%s_%i",Name,ThreadOrder); LTG_CreateDescr(LT,LT_FLAG_NOCHECK  ,mLTG_Wait,0,Descr); 

	sprintf(Descr,"thL:%s_%i",Name,ThreadOrder); LTG_CreateDescr(LT,LT_FLAG_NOCHECK  ,thLTG,0,Descr); //LT_FLAG_NOCHECK car le ThreadId n'est pas connu à ce stade, mais pas critique la thread n'ayant qu'une seule fonction
	sprintf(Descr,"thW:%s_%i",Name,ThreadOrder); LTG_CreateDescr(LT,LT_FLAG_NOCHECK  ,thLTG_Wait,0,Descr); 
#endif

	LTG_EnterI(LT,mLTG,0);

	if(Threaded)
	{
		LT.evWaiting=CreateEvent(0,0,0,0); LT.evExitThread=CreateEvent(0,0,0,0); LT.evLoopThread=CreateEvent(0,0,0,0);
		LT.mStatus=LM_LoopRunning; LT.thStatus=LT_RunningInit;
		LT.hThread = (HANDLE)_beginthreadex(0,0,LoopThread,pLT,0,&LT.thThreadID);
#ifdef DebugList
		SPG_SetThreadName(LT.thThreadID,LT.Name);
		SPG_RegisterGlobalThreadName(LT.Name,LT.thThreadID);
#endif
		SPG_WaitLoop(pLT);
	}
	else
	{
		LTG_EnterI(LT,thLTG,tSYNC);
		LT.thStatus=LT_Waiting;
		if(LT.Init) { CHECK(LT.Init(LT.User,LT.ThreadOrder)==0,"LoopThread",LT.thStatus=LT_Invalid); }
		LT.mStatus=LM_Ready;
		LTG_ExitI(LT,thLTG,tSYNC);
	}

	DbgCHECK(LT.thStatus==LT_Invalid,"SPG_LoopThreadInit failed");

	LTG_ExitI(LT,mLTG,0);

	return pLT;
}

int SPG_CONV SPG_LoopThreadClose(SPG_LOOPTHREADSTATE* pLT)
{//programme principal
	SPG_LOOPTHREADSTATE& LT=*(SPG_LOOPTHREADSTATE*)pLT;

	LTG_EnterC(LT,mLTG,0);

	if(LT.Threaded)
	{
		if(LT.mStatus==LM_LoopRunning) SPG_WaitLoop(pLT);
		LTG_EnterEH(LT,mLTG_Wait,0);
		SetEvent(LT.evExitThread); //WaitForSingleObject(LT.hThread,LT.ThWaitms);

		typedef enum { LW_cmdWait, LW_cmdExit, LW_cmdDoEvents } SPG_LOOPWAITCOMMAND;
		SPG_StackAlloc(SPG_LOOPWAITCOMMAND,c); c=LW_cmdWait;
		do//gestion'interne' du MsgWaitForMultipleObjectsEx
		{
			int r = MsgWaitForMultipleObjectsEx(1,&LT.hThread,LT.mWaitms,0,MWMO_ALERTABLE);//QS_ALLINPUT
			//if(	 r == (WAIT_FAILED) )		{ c=LW_cmdExit; }
			if(      r == (WAIT_OBJECT_0 + 0) )	{ c=LW_cmdExit; }
			else if( r == (WAIT_OBJECT_0 + 1) )	{ c=LW_cmdDoEvents;	}
			//se if( r == (WAIT_TIMEOUT) )		{ c=LW_cmdExit; }
			else if( r == (WAIT_IO_COMPLETION) ){ c=LW_cmdWait; }
			else
			{
#ifdef DebugList
				SPG_LoopThreadTimeFrozen(LT,r,"LoopThreadClose");
#endif
#ifdef SPG_DEBUGCONFIG
				c=LW_cmdWait;//reboucle sur l'erreur pour permettre le debogage
#else
				c=LW_cmdExit;
#endif
			}

			if(c == LW_cmdDoEvents) { LTG_Reent(LT,mLTG_Wait,tDOEV); DoEvents(SPG_DOEV_READ_WIN_EVENTS); LTG_Reent(LT,mLTG_Wait,tDOEV); }

		} while( c != LW_cmdExit );
		LTG_ExitEH(LT,mLTG_Wait,0);

		CloseHandle(LT.hThread); CloseHandle(LT.evWaiting); CloseHandle(LT.evExitThread); CloseHandle(LT.evLoopThread);
//fdef DebugList
//		SPG_UnRegisterGlobalThreadName(LT.Name,LT.thThreadID);
//ndif
	}
	else
	{
		LTG_EnterC(LT,thLTG,tSYNC);
		if(LT.Close) LT.Close(LT.User,LT.ThreadOrder);
		LTG_ExitC(LT,thLTG,tSYNC);
	}
	LTG_ExitC(LT,mLTG,0);
	SPG_ZeroStruct(LT);
	SPG_MemFree(pLT);
	return -1;
}

int SPG_CONV SPG_LoopThreadGetLogicalProcCount()
{
	int LogicalCount=0;
	DWORD ProcessAffinityMask;
	DWORD SystemAffinityMask;
	GetProcessAffinityMask(GetCurrentProcess(),&ProcessAffinityMask,&SystemAffinityMask);
	for(int n=0;n<32;n++)
	{
		if(ProcessAffinityMask&(1<<n)) LogicalCount++;
	}
	return LogicalCount;
}

#ifdef SPG_DEBUGCONFIG
//
// Usage: SetThreadName (-1, "MainThread");
//

//It works in VC6, but can only display the first 9 characters.

//nclude <windows.h>
#define MS_VC_EXCEPTION 0x406D1388
#define EXCEPTION_EXECUTE_HANDLER       1
#define EXCEPTION_CONTINUE_SEARCH       0
#define EXCEPTION_CONTINUE_EXECUTION    -1

#pragma pack(push,8)
typedef struct tagTHREADNAME_INFO
{
   DWORD dwType; // Must be 0x1000.
   LPCSTR szName; // Pointer to name (in user addr space).
   DWORD dwThreadID; // Thread ID (-1=caller thread).
   DWORD dwFlags; // Reserved for future use, must be zero.
} THREADNAME_INFO;
#pragma pack(pop)

void SPG_CONV SPG_SetThreadName(DWORD dwThreadID, char* threadName) //enregistre le nom de thread dans le debogueur
{
   Sleep(10);
   THREADNAME_INFO info;
   info.dwType = 0x1000;
   info.szName = threadName;
   info.dwThreadID = dwThreadID;
   info.dwFlags = 0;

   __try
   {
      RaiseException( MS_VC_EXCEPTION, 0, sizeof(info)/sizeof(ULONG_PTR), (ULONG_PTR*)&info );
   }
   __except(EXCEPTION_EXECUTE_HANDLER)
   {
   }
}
#endif

/*

//
// Usage: SetThreadName (-1, "MainThread");
//
typedef struct tagTHREADNAME_INFO
{
  DWORD dwType; // must be 0x1000
  LPCSTR szName; // pointer to name (in user addr space)
  DWORD dwThreadID; // thread ID (-1=caller thread)
  DWORD dwFlags; // reserved for future use, must be zero
} THREADNAME_INFO;

void SetThreadName( DWORD dwThreadID, LPCSTR szThreadName)
{
  THREADNAME_INFO info;
  {
    info.dwType = 0x1000;
    info.szName = szThreadName;
    info.dwThreadID = dwThreadID;
    info.dwFlags = 0;
  }
  __try
  {
    RaiseException( 0x406D1388, 0, sizeof(info)/sizeof(DWORD), (DWORD*)&info );
  }
  __except (EXCEPTION_CONTINUE_EXECUTION)
  {
  }
}

//
// Usage: SetThreadName (-1, "MainThread");
//
#include <windows.h>
#define MS_VC_EXCEPTION 0x406D1388

#pragma pack(push,8)
typedef struct tagTHREADNAME_INFO
{
   DWORD dwType; // Must be 0x1000.
   LPCSTR szName; // Pointer to name (in user addr space).
   DWORD dwThreadID; // Thread ID (-1=caller thread).
   DWORD dwFlags; // Reserved for future use, must be zero.
} THREADNAME_INFO;
#pragma pack(pop)

void SetThreadName( DWORD dwThreadID, char* threadName)
{
   Sleep(10);
   THREADNAME_INFO info;
   info.dwType = 0x1000;
   info.szName = threadName;
   info.dwThreadID = dwThreadID;
   info.dwFlags = 0;

   __try
   {
      RaiseException( MS_VC_EXCEPTION, 0, sizeof(info)/sizeof(ULONG_PTR), (ULONG_PTR*)&info );
   }
   __except(EXCEPTION_EXECUTE_HANDLER)
   {
   }
}

*/

#endif

