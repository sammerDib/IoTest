
#include "SPG_General.h"
#ifdef SPG_General_USEGENERICEXCEPTIONHANDLER
#include "SPG_Includes.h"

#include "SPG_SysInc.h"

#include <tchar.h>
#include <stdio.h>

LONG WINAPI SPG_UnhandledExceptionFilter(struct _EXCEPTION_POINTERS *ExceptionInfo)
{
	LTG_Enter(Global,LT_OSAbnormalTermination,ExceptionInfo->ExceptionRecord->ExceptionCode);
	GenericExceptionFilter(ExceptionInfo,0,"AbnormalTermination");
	LTG_Exit(Global,LT_OSAbnormalTermination,0);
	return ((LPTOP_LEVEL_EXCEPTION_FILTER)Global.LT_DefaultOSAbnormalTerminationHandler ) (ExceptionInfo);//return EXCEPTION_CONTINUE_SEARCH;
}

//http://blog.kalmbachnet.de/?postid=75 "SetUnhandledExceptionFilter" and VC8

LPTOP_LEVEL_EXCEPTION_FILTER WINAPI MyDummySetUnhandledExceptionFilter(
	LPTOP_LEVEL_EXCEPTION_FILTER lpTopLevelExceptionFilter)
{
	LTG_Enter(Global,LT_AttemptToChangeOSAbnormalTermination,(DWORD)lpTopLevelExceptionFilter);
	DbgCHECK(1,"MyDummySetUnhandledExceptionFilter:AttemptToChangeOSAbnormalTermination");
	LTG_Exit(Global,LT_AttemptToChangeOSAbnormalTermination,0);
return NULL;
}

BOOL PreventSetUnhandledExceptionFilter()
{
    HMODULE hKernel32 = LoadLibrary(_T("kernel32.dll"));
    if (hKernel32==NULL) return FALSE;
    void *pOrgEntry = GetProcAddress(hKernel32, "SetUnhandledExceptionFilter");
    if(pOrgEntry==NULL) return FALSE;
    unsigned char newJump[ sizeof(void*) + 1 ];
    DWORD dwOrgEntryAddr = (DWORD) pOrgEntry;
    dwOrgEntryAddr += 5; // add 5 for 5 op-codes for jmp far
    void *pNewFunc = &MyDummySetUnhandledExceptionFilter;
    DWORD dwNewEntryAddr = (DWORD) pNewFunc;
    DWORD dwRelativeAddr = dwNewEntryAddr - dwOrgEntryAddr;

    newJump[ 0 ] = 0xE9;  // JMP absolute
    memcpy(&newJump[ 1 ], &dwRelativeAddr, sizeof(pNewFunc));
    SIZE_T bytesWritten;
    BOOL bRet = WriteProcessMemory(GetCurrentProcess(), pOrgEntry, newJump, sizeof(pNewFunc) + 1, &bytesWritten);
    return bRet;
}


int SPG_CONV SPG_AbnormalTerminationSetFilter()
{
	Global.LT_DefaultOSAbnormalTerminationHandler=(DWORD)SetUnhandledExceptionFilter(SPG_UnhandledExceptionFilter);
	PreventSetUnhandledExceptionFilter();
	return -1;
}

#endif
