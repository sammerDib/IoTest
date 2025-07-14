
#include "SPG_General.h"

#ifdef SPG_General_USEGlobal

#include "SPG_Includes.h"

#include "BreakHook.h"

#include <string.h>

GlobalType Global;



void SPG_CONV SPG_AddCallbackOnCheck(SPG_CHECKCALLBACK AddrCallBack, void * ParamCallBack)
{
#ifdef DebugList
	for(int i=0;i<MAXCHECKCALLBACK;i++)
	{
		if((Global.CHECK_Callback[i].Addr==0)&&(Global.CHECK_Callback[i].Param==0))
		{
			Global.CHECK_Callback[i].Addr=AddrCallBack;
			Global.CHECK_Callback[i].Param=ParamCallBack;
			return;
		}
	}
	SPG_List("SPG_AddCallbackOnCheck: No emty slot");
#endif
	return;
}

void SPG_CONV SPG_RemoveCallbackOnCheck(SPG_CHECKCALLBACK AddrCallBack, void * ParamCallBack)
{
#ifdef DebugList
	for(int i=0;i<MAXCHECKCALLBACK;i++)
	{
		if((Global.CHECK_Callback[i].Addr==AddrCallBack)&&(Global.CHECK_Callback[i].Param==ParamCallBack))
		{
			Global.CHECK_Callback[i].Addr=0;
			Global.CHECK_Callback[i].Param=0;
			return;
		}
	}
	SPG_List("SPG_RemoveCallbackOnCheck: Not found");
#endif
	return;
}

bool SPG_CONV SPG_RunAllCheckCallbacks(const char* Txt)
{
	bool processed=false;
	for(int i=0;i<MAXCHECKCALLBACK;i++)
	{
		if((Global.CHECK_Callback[i].Addr!=0)&&(Global.CHECK_Callback[i].Param!=0)&&(Global.CHECK_Callback[i].Recurse==0))
		{
			Global.CHECK_Callback[i].Recurse++;
			Global.CHECK_Callback[i].Addr(Global.CHECK_Callback[i].Param,Txt);
			processed=true;
			Global.CHECK_Callback[i].Recurse--;
		}
		else if(Global.CHECK_Callback[i].Recurse)
		{
#ifdef SPG_DEBUGCONFIG
			//Attempted to reenter
			BreakHook();
#endif
		}
	} //boucle qui appelle toutes les callback recevant les messages des macros CHECK (une précondition est non valide)
	return processed;
}

#ifdef MaxCallBack

void SPG_CONV SPG_AddUpdateOnDoEvents(SPG_CALLBACK AddrCallBack, void * ParamCallBack, int NrOrdre)
{
	CHECK(Global.NumCallBack>=MaxCallBack,"SPG_AddUpdateOnDoEvents: Trop de callbacks",return);
	int i;
	for(i=0;i<Global.NumCallBack;i++)
	{
		if (Global.CallBack[i].NrOrdre>=NrOrdre) break;
	}
	if(i!=Global.NumCallBack) memmove(Global.CallBack+i+1,Global.CallBack+i,(Global.NumCallBack-i)*sizeof(Global_CALLBACK));
	Global.NumCallBack++;
	Global.CallBack[i].Addr=AddrCallBack;
	Global.CallBack[i].Param=ParamCallBack;
	Global.CallBack[i].NrOrdre=NrOrdre;
	Global.CallBack[i].Recurse=false;
	CD_G_CHECK_EXIT(16,13);
	return;
}

void SPG_CONV SPG_KillUpdateOnDoEvents(SPG_CALLBACK AddrCallBack, void * ParamCallBack)
{
#ifdef DebugList
	int Found=0;
#endif
	for(int i=0;i<Global.NumCallBack;i++)
	{
		if((Global.CallBack[i].Addr==AddrCallBack)&&(Global.CallBack[i].Param==ParamCallBack))
		{
			memmove(Global.CallBack+i,Global.CallBack+i+1,(Global.NumCallBack-i)*sizeof(Global_CALLBACK));
			Global.NumCallBack--;
#ifdef DebugList
			Found=1;
#endif
		}
	}
	DbgCHECK(Found==0,"SPG_KillUpdateOnDoEvents: Non trouve");
	return;
}

/*
void SPG_CONV SPG_KillUpdateOnDoEventsByAddr(SPG_CALLBACK AddrCallBack)//, void * ParamCallBack)
{
#ifdef DebugList
	int Found=0;
#endif
	for(int i=0;i<Global.NumCallBack;i++)
	{
		if(Global.CallBack[i].Addr==AddrCallBack)
		{
			memmove(Global.CallBack+i,Global.CallBack+i+1,(Global.NumCallBack-i)*sizeof(Global_CALLBACK));
			Global.NumCallBack--;
#ifdef DebugList
			Found=1;
#endif
		}
	}
	DbgCHECK(Found==0,"SPG_KillUpdateOnDoEventsByAddr: Non trouve");
	return;
}

void SPG_CONV SPG_KillUpdateOnDoEventsByParam(void* ParamCallBack)//, void * ParamCallBack)
{
#ifdef DebugList
	int Found=0;
#endif
	for(int i=0;i<Global.NumCallBack;i++)
	{
		if(Global.CallBack[i].Param==ParamCallBack)
		{
			memmove(Global.CallBack+i,Global.CallBack+i+1,(Global.NumCallBack-i)*sizeof(Global_CALLBACK));
			Global.NumCallBack--;
#ifdef DebugList
			Found=1;
#endif
		}
	}
	DbgCHECK(Found==0,"SPG_KillUpdateOnDoEventsByParam: Non trouve");
	return;
}
*/

#endif

#else
#pragma SPGMSG(__FILE__,__LINE__,"NoGlobal")
#endif




