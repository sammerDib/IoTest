
#include "SPG_General.h"

#ifdef SPG_General_USEGENERICEXCEPTIONHANDLER

#include "SPG_Includes.h"

#include "SPG_SysInc.h"

#include <stdio.h>

#pragma SPGMSG(__FILE__,__LINE__,"Using Generic Exception Handler")

#define DefineToString(m,x) case x: strcpy(m,#x); break
#define DefineXToString(m,x) case EXCEPTION_##x: strcpy(m,#x); break

void SPG_CONV ExceptionString(DWORD e, char* ME)
{
	switch(e)
	{
		DefineXToString(	ME,	ACCESS_VIOLATION					);
		DefineXToString(	ME,	DATATYPE_MISALIGNMENT		);
		DefineXToString(	ME,	BREAKPOINT								);
		DefineXToString(	ME,	SINGLE_STEP								);
		DefineXToString(	ME,	ARRAY_BOUNDS_EXCEEDED		);
		DefineXToString(	ME,	FLT_DENORMAL_OPERAND		);
		DefineXToString(	ME,	FLT_DIVIDE_BY_ZERO				    );
		DefineXToString(	ME,	FLT_INEXACT_RESULT					);
		DefineXToString(	ME,	FLT_INVALID_OPERATION			);
		DefineXToString(	ME,	FLT_OVERFLOW							);
		DefineXToString(	ME,	FLT_STACK_CHECK						);
		DefineXToString(	ME,	FLT_UNDERFLOW						);
		DefineXToString(	ME,	INT_DIVIDE_BY_ZERO					);
		DefineXToString(	ME,	INT_OVERFLOW							);
		DefineXToString(	ME,	PRIV_INSTRUCTION					);
		DefineXToString(	ME,	IN_PAGE_ERROR						);
		DefineXToString(	ME,	ILLEGAL_INSTRUCTION				);
		DefineXToString(	ME,	NONCONTINUABLE_EXCEPTION	);
		DefineXToString(	ME,	STACK_OVERFLOW					);
		DefineXToString(	ME,	INVALID_DISPOSITION				);
		DefineXToString(	ME,	GUARD_PAGE								);
		DefineXToString(	ME,	INVALID_HANDLE						);
//		DefineXToString(	ME,	POSSIBLE_DEADLOCK				);
		DefineToString(	ME,	CONTROL_C_EXIT							);
	default:
		strcpy(ME, "Unknown");
	}
	return;
}

void SPG_CONV GenericSnapShotFct(LPEXCEPTION_POINTERS e, const char* fctname)
{
	return;
}

int SPG_CONV GenericExceptionFilter(LPEXCEPTION_POINTERS e, void SPG_CONV snapshotfct(LPEXCEPTION_POINTERS,const char*) , const char* fctname)
{
	LTG_Enter(Global,LT_OSAbnormalTermination,e->ExceptionRecord->ExceptionCode);

	SPG_List_ResetMsg();
#ifdef SPG_DEBUGCONFIG
	if(IsDebuggerPresent())
	{
		if(MessageBox(0,L"GenericExceptionFilter: Break into debugger ?",fdwstring(SPG_COMPANYNAME),MB_YESNO)==IDYES) 
		{
			DebugBreak();
		}
	}
#endif

	char ME[32]; ExceptionString(e->ExceptionRecord->ExceptionCode,ME);

	char Msg[256];
#ifdef SPG_General_USEWLTG
	char CleanName[256];
	SPG_MakeCleanFileName(CleanName,fctname);
	sprintf(Msg,"%sLogTimeExcpt_%s_%0.4X_%s.txt",Global.LogDir,ME,e->ExceptionRecord->ExceptionCode,CleanName);

	LT_Save_TXT(Global.wt.t,Msg);

	if(snapshotfct!=0) snapshotfct(e,(char*)fctname); else GenericSnapShotFct(e,fctname);
#endif
#ifdef DebugList	
	sprintf(Msg,"Exception %s(%.04X) in\n%s",ME,e->ExceptionRecord->ExceptionCode,fctname);
	SPG_List(Msg);
#endif

	LTG_Exit(Global,LT_OSAbnormalTermination,e->ExceptionRecord->ExceptionCode);

	if(e->ExceptionRecord->ExceptionFlags==EXCEPTION_NONCONTINUABLE_EXCEPTION)
	{
		return EXCEPTION_CONTINUE_SEARCH;
	}
	else if(e->ExceptionRecord->ExceptionCode==EXCEPTION_INT_DIVIDE_BY_ZERO)
	{
		return EXCEPTION_EXECUTE_HANDLER;
		//return EXCEPTION_CONTINUE_EXECUTION;
	}
	else if(e->ExceptionRecord->ExceptionCode==EXCEPTION_ACCESS_VIOLATION)
	{
		return EXCEPTION_EXECUTE_HANDLER;
		//return EXCEPTION_CONTINUE_EXECUTION;
	}
	else
	{
		return EXCEPTION_CONTINUE_SEARCH;
		//return EXCEPTION_CONTINUE_EXECUTION;
	}
	//continuable : EXCEPTION_CONTINUE_EXECUTION
	//EXCEPTION_CONTINUE_EXECUTION
}

int SPG_CONV GenericExceptionHandlerRZ()
{
	return 0;
}

#endif
