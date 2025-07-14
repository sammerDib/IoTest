
#include "SPG_General.h"

#ifdef SPG_General_USEGLOBALTHREADNAME

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

void SPG_CONV SPG_RegisterGlobalThreadName(char* threadName, DWORD ThreadId)
{
	THREAD_DBG_INFOS& T=Global.ThreadDbgInfos;
	if(ThreadId==0) ThreadId=GetCurrentThreadId();
	int i;
	for(i=0;i<T.NumItems;i++)
	{
		if(T.tdn[i].ThreadId==ThreadId) break;
	}
	T.tdn[i].ThreadId=ThreadId;
	strncpy(T.tdn[i].Name,threadName,THREAD_DBG_NAME-1);
	T.NumItems=i+1;
	return;
}

void SPG_CONV SPG_UnRegisterGlobalThreadName(char* threadName, DWORD ThreadId )
{
	THREAD_DBG_INFOS& T=Global.ThreadDbgInfos;
	if(ThreadId==0) ThreadId=GetCurrentThreadId();
	int i;
	for(i=0;i<T.NumItems;i++)
	{
		if(T.tdn[i].ThreadId==ThreadId) break;
	}
	for(i++;i<T.NumItems;i++)
	{
		T.tdn[i-1]=T.tdn[i];
	}
	T.NumItems=i-1;
	return;
}

char* SPG_CONV SPG_GetGlobalThreadName(DWORD ThreadId )
{
	THREAD_DBG_INFOS& T=Global.ThreadDbgInfos;
	if(ThreadId==0) ThreadId=GetCurrentThreadId();
	int i;
	for(i=0;i<T.NumItems;i++)
	{
		if(T.tdn[i].ThreadId==ThreadId) return T.tdn[i].Name;
	}
	SPG_RegisterGlobalThreadName("Unknown");
	return T.tdn[T.NumItems-1].Name;
}

#endif

