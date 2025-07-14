
#include "SPG_General.h"

#ifdef SPG_General_USEConsole

#include "SPG_Includes.h"

#define Console_HasDate

#ifdef Console_HasDate
#include "SPG_SysInc.h"
#endif

#include <string.h>
#include <stdio.h>

SPG_Console* SPG_CONV Console_Create(G_Ecran* E, C_Lib* CL, int AutoUpdate)
{
#ifdef SPG_General_USECarac
	CHECK((E==0)||(CL==0),"Console_Create: Parametres invalides",return 0);
#endif
	SPG_Console*C=SPG_TypeAlloc(1,SPG_Console,"Console_Create");
	if(C) Console_Create(*C,E,CL,AutoUpdate);
	return C;
}

SPG_Console* SPG_CONV FileConsole_Create(G_Ecran* E, C_Lib* CL, int AutoUpdate, char* Path, char* CslName, int FlushFile, int Flag)
{
#ifdef SPG_General_USECarac
	CHECK((E==0)||(CL==0),"FileConsole_Create: Parametres invalides",return 0);
#endif
	SPG_Console*C=SPG_TypeAlloc(1,SPG_Console,"FileConsole_Create");
	if(C) FileConsole_Create(*C,E,CL,AutoUpdate,Path,CslName,FlushFile,Flag);
	return C;
}

void SPG_CONV Console_AddDate(SPG_Console& C)
{
	SYSTEMTIME SystemTime;
	//GetSystemTime(&SystemTime);
	GetLocalTime(&SystemTime);
	char Date[128];
	sprintf(Date,"%0.2i/%0.2i/%0.2i %0.2i:%0.2i:%0.2i",
		(int)SystemTime.wDay,(int)SystemTime.wMonth,(int)SystemTime.wYear,(int)SystemTime.wHour,(int)SystemTime.wMinute,(int)SystemTime.wSecond);
	Console_Add(C,Date);
	return;
}

int SPG_CONV FileConsole_Create(SPG_Console& C, G_Ecran* E, C_Lib* CL, int AutoUpdate, char* Path, char* CslName, int FlushFile, int Flag)
{
	DbgCHECK(!SPG_GLOBAL_ETAT(OK),"FileConsole_Create: Call SPG_Initialise first");
	int i=Console_Create(C,E,CL,AutoUpdate);
	if(i) 
	{
		char PathAndFilename[MaxProgDir];
		SPG_ConcatPath(PathAndFilename,Path?Path:Global.LogDir,CslName);
		if(FlushFile)
			C.F=fopen(PathAndFilename,"wb");
		else
			C.F=fopen(PathAndFilename,"ab");
		if(C.F) C.Etat|=CONSOLE_HOOKFILE;
#ifdef Console_HasDate
		if(Flag&CONSOLE_WITHDATE)
		{
			C.Etat|=CONSOLE_WITHDATE;
			Console_AddDate(C);
		}
#endif
#ifdef DebugList
		if(Flag&CONSOLE_HOOKCHECK)
		{
			SPG_AddCallbackOnCheck((SPG_CHECKCALLBACK)((void(SPG_CONV*)(SPG_Console*,char*))Console_Add),&C);
			C.Etat|=CONSOLE_HOOKCHECK;
		}
#endif
	}
	return i;
}

int SPG_CONV Console_Create(SPG_Console& C, G_Ecran* E, C_Lib* CL, int AutoUpdate)
{
	memset(&C,0,sizeof(SPG_Console));
//ifdef SPG_General_USECarac
//	CHECK((E==0)||(CL==0),"Console_Create: Parametres invalides",return 0);
//endif
	C.CarLib=CL;
	C.Ecran=E;
#ifdef SPG_General_USETimer
	S_InitTimer(C.UpdateTimer,"Console");
	S_StartTimer(C.UpdateTimer);
#endif
	C.Etat=CONSOLE_OK;

	if (AutoUpdate)
	{
		SPG_AddUpdateOnDoEvents((SPG_CALLBACK)(void(SPG_CONV*)(SPG_Console&))Console_Update,&C,2);
		C.Etat|=CONSOLE_AUTOUPDATE;
	}
#ifdef DebugMem
	C.SPG_STACK_SPY_BeforeCTest=0xAA55AA55;
	C.SPG_STACK_SPY_AfterCTest=0xAA55AA55;
#endif

	return -1;
}

//FDE
//void SPG_CONV Console_Add(SPG_Console* C, const char*T)
//{
//	if(C) Console_Add(*C,T);
//	return;
//}

void SPG_CONV Console_Add(SPG_Console& C, const char*T)
{
	if(C.Etat==0) return;
#ifdef DebugMem
	CHECK((C.SPG_STACK_SPY_BeforeCTest!=0xAA55AA55)||(C.SPG_STACK_SPY_AfterCTest!=0xAA55AA55),"ConsoleAdd: Buffer overwrite",;);
#endif
	int Ltoadd=strlen(T);
	if(C.Etat&CONSOLE_HOOKFILE) 
	{
		fwrite("\n",1,1,(FILE*)C.F);
		fwrite(T,Ltoadd,1,(FILE*)C.F);
		fflush((FILE*)C.F);
	}
	CHECK(Ltoadd>=MaxConsole-16,"Console_Add: Texte trop long",return);
	int Lavant=strlen(C.CText);
	while((Lavant)&&(Lavant+Ltoadd>=MaxConsole-16))
	{
		Console_DeleteALine(C);
		Lavant=strlen(C.CText);
	}
	strcat(C.CText,"\n");
	strcat(C.CText,T);
	if(C.CarLib)
	{
		while((Lavant)
#ifdef SPG_General_USECarac
			&&(CF_LineCount(C.CText)*(C.CarLib->SpaceY)>=(C.Ecran->SizeY))
#endif
			)
		{
			Console_DeleteALine(C);
			Lavant=strlen(C.CText);
		}
	}
#ifdef SPG_General_USETimer
	S_StopTimer(C.UpdateTimer);
	S_ResetTimer(C.UpdateTimer);
	S_StartTimer(C.UpdateTimer);
#endif
#ifdef DebugMem
	CHECK((C.SPG_STACK_SPY_BeforeCTest!=0xAA55AA55)||(C.SPG_STACK_SPY_AfterCTest!=0xAA55AA55),"ConsoleAdd: Buffer overwrite",;);
#endif
	return;
}

void SPG_CONV Console_AddOnSameLine(SPG_Console* C, char*T)
{
	if(C) Console_AddOnSameLine(*C, T);
	return;
}

void SPG_CONV Console_AddOnSameLine(SPG_Console& C, char*T)
{
	if(C.Etat==0) return;
#ifdef DebugMem
	CHECK((C.SPG_STACK_SPY_BeforeCTest!=0xAA55AA55)||(C.SPG_STACK_SPY_AfterCTest!=0xAA55AA55),"Console_AddOnSameLine: Buffer overwrite",;);
#endif
	int Ltoadd=strlen(T);
	if(C.Etat&CONSOLE_HOOKFILE) 
	{
		fwrite(T,Ltoadd,1,(FILE*)C.F);
	}
	CHECK(Ltoadd>=MaxConsole,"Console_Add: Texte trop long",return);
	int Lavant=strlen(C.CText);
	while((Lavant)&&(Lavant+Ltoadd>=MaxConsole))
	{
		Console_DeleteALine(C);
		Lavant=strlen(C.CText);
	}
	//strcat(C->CText,"\n");
	strcat(C.CText,T);
	while((Lavant)
#ifdef SPG_General_USECarac
		&&(CF_LineCount(C.CText)*(C.CarLib->SpaceY)>=(C.Ecran->SizeY))
#endif
		)
	{
		Console_DeleteALine(C);
		Lavant=strlen(C.CText);
	}
#ifdef SPG_General_USETimer
	S_StopTimer(C.UpdateTimer);
	S_ResetTimer(C.UpdateTimer);
	S_StartTimer(C.UpdateTimer);
#endif
#ifdef DebugMem
	CHECK((C.SPG_STACK_SPY_BeforeCTest!=0xAA55AA55)||(C.SPG_STACK_SPY_AfterCTest!=0xAA55AA55),"Console_AddOnSameLine: Buffer overwrite",;);
#endif
	return;
}

void SPG_CONV Console_DeleteALine(SPG_Console* C)
{
	if(C)  Console_DeleteALine(*C);
	return;
}

void SPG_CONV Console_DeleteALine(SPG_Console& C)
{
	if(C.Etat==0) return;
#ifdef DebugMem
	CHECK((C.SPG_STACK_SPY_BeforeCTest!=0xAA55AA55)||(C.SPG_STACK_SPY_AfterCTest!=0xAA55AA55),"Console_DeleteALine: Buffer overwrite",;);
#endif
	int Lavant=strlen(C.CText);
	int i;
	for(i=0;i<Lavant;i++)
	{
		if (C.CText[i]=='\n') break;
	}
	if (i==Lavant) 
	{
		C.CText[0]=0;
	}
	else
		memmove(C.CText,C.CText+i+1,Lavant-i);
#ifdef DebugMem
	CHECK((C.SPG_STACK_SPY_BeforeCTest!=0xAA55AA55)||(C.SPG_STACK_SPY_AfterCTest!=0xAA55AA55),"Console_DeleteALine: Buffer overwrite",;);
#endif
	return;
}

void SPG_CONV Console_Update(SPG_Console* C)
{
	if(C) Console_Update(*C);
	return;
}

void SPG_CONV Console_Update(SPG_Console& C)
{
#ifdef SPG_General_USECarac
	if(C.Etat==0) return;
	if (C.CText[0])
	{
	//S_StopTimer(C->UpdateTimer);
		/*
	char S[1024];
	sprintf(S,"Temps depuis refresh: %f",S_GetTimerTime(C->UpdateTimer));
	Console_Add(C,S);
	*/
		int LineCount=CF_LineCount(C.CText);
	G_DrawRect(*(C.Ecran),0,0,C.Ecran->SizeX,LineCount*(C.CarLib->SpaceY),C.CarLib->BackGroundColor.Coul);

#ifdef SPG_General_USETimer
#ifdef SPG_General_USEGlobal
	float T;
	S_GetTimerRunningTime(C.UpdateTimer,T);
	float ConsoleFilling=((float)CF_LineCount(C.CText)*(C.CarLib->SpaceY)/((float)C.Ecran->SizeY));
	if (T>(1.0f*(1-(ConsoleFilling*ConsoleFilling))))
#endif
#endif
	{
		Console_DeleteALine(C);
#ifdef SPG_General_USETimer
		S_StopTimer(C.UpdateTimer);
		S_ResetTimer(C.UpdateTimer);
		S_StartTimer(C.UpdateTimer);
#endif
	}
	//S_StartTimer(C->UpdateTimer);
	//C_PrintWithBorder(*(C->Ecran),0,0,C->CText,*(C->CarLib),0,0);
	//G_DrawRect(*(C->Ecran),0,0,C->Ecran->SizeX,C->Ecran->SizeY,C->CarLib->BackGroundColor.Coul);
	C_Print(*(C.Ecran),0,0,C.CText,*(C.CarLib),0);
	}
#endif
	return;
}

void SPG_CONV Console_Clear(SPG_Console* C)
{
	if(C) Console_Clear(*C);
	return;
}

void SPG_CONV Console_Clear(SPG_Console& C)
{
	if(C.Etat==0) return;
	//G_DrawRect(*(C.Ecran),0,0,C.Ecran->SizeX,CF_LineCount(C.CText)*(C.CarLib->SpaceY),C.CarLib->BackGroundColor.Coul);
	C.CText[0]=0;
	return;
}
/*
void SPG_CONV Console_ForceUpdate(SPG_Console*C)
{
	Console_Update(C);
#ifndef SPG_General_PGLib
	G_BlitEcran(Global.Ecran);
#endif
}
*/

void SPG_CONV Console_Close(SPG_Console* C)
{
	if(C)
	{
		Console_Close(*C);
		SPG_MemFree(C);
	}
	return;
}

void SPG_CONV Console_Close(SPG_Console& C)
{
#ifdef DebugMem
	CHECK((C.SPG_STACK_SPY_BeforeCTest!=0xAA55AA55)||(C.SPG_STACK_SPY_AfterCTest!=0xAA55AA55),"Console_Close: Possibly buffer overwrite",;);
#endif
	if(C.Etat&CONSOLE_AUTOUPDATE) SPG_KillUpdateOnDoEvents((SPG_CALLBACK)(void(SPG_CONV*)(SPG_Console&))Console_Update,&C);
	if(C.Etat&CONSOLE_WITHDATE)
	{
		Console_AddDate(C);
	}
	if(C.Etat&CONSOLE_HOOKFILE)
	{
		fclose((FILE*)C.F);
	}
#ifdef DebugList
	if(C.Etat&CONSOLE_HOOKCHECK)
	{
		SPG_RemoveCallbackOnCheck((SPG_CHECKCALLBACK)((void(SPG_CONV*)(SPG_Console*,char*))Console_Add),&C);
	}
#endif

#ifdef SPG_General_USETimer
	S_StopTimer(C.UpdateTimer);
	S_CloseTimer(C.UpdateTimer);
#endif
#ifdef DebugMem
	CHECK((C.SPG_STACK_SPY_BeforeCTest!=0xAA55AA55)||(C.SPG_STACK_SPY_AfterCTest!=0xAA55AA55),"Console_Close: Possibly buffer overwrite",;);
#endif
	memset(&C,0,sizeof(SPG_Console));
	return;
}

#endif

