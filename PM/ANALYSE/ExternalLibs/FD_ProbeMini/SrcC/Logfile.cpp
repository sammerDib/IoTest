
#include "SPG_General.h"

#ifdef SPG_General_USEFiles

#include <stdarg.h>
#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <string.h>
#include <iostream>
#include <stdio.h>
#include <memory.h>

static void SPG_CONV SPG_GetDate(char* s, int Flag)
{
	SYSTEMTIME SystemTime;
	if(Flag&LOGWITHTHREADID)
	{
		GetLocalTime(&SystemTime);
		sprintf(s,"%0.2i/%0.2i/%0.2i %0.2i:%0.2i:%0.2i.%0.3i TID%i",
			(int)SystemTime.wDay,(int)SystemTime.wMonth,(int)SystemTime.wYear,(int)SystemTime.wHour,(int)SystemTime.wMinute,(int)SystemTime.wSecond,(int)SystemTime.wMilliseconds,(int)GetCurrentThreadId());
	}
	else if(Flag&LOGWITHDATE)
	{
		GetLocalTime(&SystemTime);
		sprintf(s,"%0.2i/%0.2i/%0.2i %0.2i:%0.2i:%0.2i.%0.3i",
			(int)SystemTime.wDay,(int)SystemTime.wMonth,(int)SystemTime.wYear,(int)SystemTime.wHour,(int)SystemTime.wMinute,(int)SystemTime.wSecond,(int)SystemTime.wMilliseconds);
	}
	else
	{
		s[0]=0;
	}
	return;
}

#ifdef SPG_General_USETimer
static void SPG_CONV SPG_GetTimer(char* s, int Flag, S_TIMER& T)
{
	double dT;
	if(Flag&LOGWITHTIMER)
	{
		S_GetTimerRunningTimeAndRestart(T,dT);
		sprintf(s,"%0.3f",1000*dT);
	}
	else
	{
		s[0]=0;
	}
	return;
}
#endif

#define OPENWA {DbgCHECK(logfile.Count!=0,"LogFile Open"); logfile.hFile = CreateFile(fdwstring(logfile.sLogName), FILE_WRITE_DATA, FILE_SHARE_READ, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL); DbgCHECKTWO(logfile.hFile==INVALID_HANDLE_VALUE,"Logfile Open",logfile.sLogName);  if(logfile.hFile!=INVALID_HANDLE_VALUE) {logfile.Count++; SetFilePointer((HANDLE)logfile.hFile,0,0,FILE_END);} }
#define CLOSE {if(logfile.hFile!=INVALID_HANDLE_VALUE) {logfile.Count--; CloseHandle((HANDLE)logfile.hFile);};logfile.hFile=INVALID_HANDLE_VALUE; DbgCHECK(logfile.Count!=0,"Logfile Close");}

static void SPG_CONV LogfileUpdate(LOGFILE& logfile)
{
	if((logfile.MaxSize>0)&&(logfile.MaxCount>0))
	{
		if(logfile.CurSize+logfile.MsgLen>=logfile.MaxSize)
		{
			if((logfile.Flag&LOGALWAYSCLOSE)==0) CLOSE;

			SPG_ArrayStackAlloc(char,bkSrcLogName,MAXLOGNAME);
			SPG_ArrayStackAlloc(char,bkDestLogName,MAXLOGNAME);
			//DeleteFile(logfile.bkName);
			for(int i=logfile.MaxCount-1;i>=0;i--)
			{
				char Xsrc[32];sprintf(Xsrc,"_log%.2i.txt",i);
				char Xdst[32];sprintf(Xdst,"_log%.2i.txt",i+1);
				strcpy(bkSrcLogName,logfile.sModuleName); SPG_SetExtens(bkSrcLogName,Xsrc);
				strcpy(bkDestLogName,logfile.sModuleName); SPG_SetExtens(bkDestLogName,Xdst);
				CopyFile(fdwstring(bkSrcLogName),fdwstring(bkDestLogName),0);//8 sur 9, 7 sur 8, ..., 0 sur 1
				SPG_ArrayStackCheck(bkSrcLogName);
				SPG_ArrayStackCheck(bkDestLogName);
			}
			DeleteFile(fdwstring(bkSrcLogName));
			logfile.CurSize=0;

			if((logfile.Flag&LOGALWAYSCLOSE)==0) OPENWA;
		}
	}
	return;
}

static int SPG_CONV strinsert(char* c, const char* i)
{
	if((c==0)||(i==0)) return 0;
	intptr_t cln=strlen(c);
	intptr_t iln=strlen(i);
	memmove(c+iln,c,cln);//comme potentiellement len(c)>len(i) c'est un memmove et pas un strcpy
	memcpy(c,i,iln);
	return iln;
}

#ifdef FDE
void SPG_CONV LogfileUpdateWindow(LOGFILE& logfile)
{
#ifdef SPG_General_USEConsole
	Console_Update(logfile.C);
#endif
	SPG_BlitWindow(*(SPG_Window**)&logfile.SW);
	return;
}
#endif

void SPG_CONV LogfileInit(LOGFILE& logfile, const char* sModuleName, int Flag, int MaxFileSize, int MaxFileCount, char* Path)
{
#ifdef FDE
	SPG_ZeroStruct(logfile);

	SPL_Init(logfile.L,200,"LogfileInit");

	if(Path) SPG_ConcatPath(logfile.sModuleName,Global.ProgDir,Path); else strcpy(logfile.sModuleName,Global.LogDir);
	if(sModuleName) SPG_ConcatPath(logfile.sModuleName,logfile.sModuleName,sModuleName); else SPG_ConcatPath(logfile.sModuleName,logfile.sModuleName,Global.ProgName);

	//a ce stade logfile.sModuleName contient le nom et le chemin complet. L'extension (exe ou dll) sera remplacée par "_00.txt" par SPG_SetExtens
	
	//logfile.sLogName contient le nom du fichier courant (avec "_00.txt")
	strcpy(logfile.sLogName,logfile.sModuleName); SPG_SetExtens(logfile.sLogName,"_log00.txt"); //sprintf(Xsrc,"_log%.2i.txt",0);
	
	logfile.Flag=Flag;
	logfile.MaxSize=MaxFileSize;				//memorise la taille max
	logfile.MaxCount=MaxFileCount;
	
	SPG_GetFileSize(logfile.sLogName,logfile.CurSize);	//recupere la taille courante

	if((logfile.Flag&LOGALWAYSCLOSE)==0)	OPENWA;

#ifdef FDE
#ifdef SPG_General_USEWindow
#ifdef SPG_General_USEConsole
	if(logfile.Flag&LOGWINDOW)
	{
		if(SPG_CreateWindow(*(SPG_Window**)&logfile.SW,SPGWT_UserFriendly|SPGWT_Moveable|SPGWT_NoClose,G_STANDARDW,1,-1,256,128,0,logfile.sModuleName,"LOGF",0,&logfile,Global.hInstance))
		{
			C_LoadCaracLib(logfile.CL,((SPG_Window*)logfile.SW)->Ecran,0,"Carac\\C5.bmp");
			Console_Create(logfile.C,&((SPG_Window*)logfile.SW)->Ecran,&logfile.CL,0);
			SPG_AddUpdateOnDoEvents((SPG_CALLBACK)(LogfileUpdateWindow),&logfile,2);
		}
		else
		{
			logfile.Flag&=~LOGWINDOW;
		}
	}
#endif
#endif
#endif //FDE
	if(logfile.Flag&LOGCHECKCLBK)	SPG_AddCallbackOnCheck((SPG_CHECKCALLBACK)Logfile,&logfile);

	S_InitTimer(logfile.T,"Logfile");
	S_StartTimer(logfile.T);
	logfile.Etat=-1;

	SPL_Exit(logfile.L);

	return;
#endif	//FDE
}

void SPG_CONV LogfileClose(LOGFILE& logfile)
{
#ifdef FDE
	if(logfile.Etat!=-1) return;

	if(logfile.Flag&LOGCHECKCLBK)	SPG_RemoveCallbackOnCheck((SPG_CHECKCALLBACK)Logfile,&logfile);

	CHECK(SPL_Enter(logfile.L,"LogfileClose")==SPL_TIMEOUT,"LogfileClose",return);

	S_StopTimer(logfile.T);
	S_CloseTimer(logfile.T);

	if((logfile.Flag&LOGALWAYSCLOSE)==0) CLOSE;

#ifdef FDE
#ifdef SPG_General_USEConsole
	if(logfile.Flag&LOGWINDOW)
	{
		SPG_KillUpdateOnDoEvents((SPG_CALLBACK)(LogfileUpdateWindow),&logfile);
		Console_Close(logfile.C);
		C_CloseCaracLib(logfile.CL);
		SPG_CloseWindow(*(SPG_Window**)&logfile.SW);
	}
#endif
#endif

	SPL_Close(logfile.L);

	SPG_ZeroStruct(logfile);
	return;
#endif	//FDE
}

void SPG_CONV Logfile(LOGFILE& logfile, const char* Msg)
{
#ifdef FDE
	if(logfile.Etat!=-1) return;

	if(SPL_Enter(logfile.L,"Logfile")==SPL_TIMEOUT) return;

	SPG_GetDate(logfile.sDate,logfile.Flag);
#ifdef SPG_General_USETimer
	SPG_GetTimer(logfile.sDate+strlen(logfile.sDate),logfile.Flag,logfile.T);
#endif
	logfile.MsgLen=_snprintf(logfile.sMsg,MAXLOGMSG-MAXLOGDATE,"%s\t%s",logfile.sDate,Msg);

	for(int i=0;i<logfile.MsgLen;i++)		{	if(logfile.sMsg[i]==' ') { logfile.sMsg[i]=' '; }	else if(logfile.sMsg[i]=='\r') { logfile.sMsg[i]='\t'; }	else if(logfile.sMsg[i]=='\n') { logfile.sMsg[i]='\t'; }	else if(logfile.sMsg[i]=='\t') { logfile.sMsg[i]='\t'; }	}  //remplacer ici les carateres à ne pas imprimer sur la ligne de log

#ifdef FDE
#ifdef SPG_General_USEConsole
	if(logfile.Flag&LOGWINDOW) Console_Add(logfile.C,logfile.sMsg);
#endif
#endif

	strcpy(logfile.sMsg+logfile.MsgLen,"\r\n"); logfile.MsgLen+=2;

	LogfileUpdate(logfile);
	
	if(logfile.Flag&LOGALWAYSCLOSE) OPENWA;

	if(logfile.hFile!=INVALID_HANDLE_VALUE)
	{
		DWORD Written;
		WriteFile((HANDLE)logfile.hFile,logfile.sMsg,logfile.MsgLen,&Written,0);
		logfile.CurSize+=logfile.MsgLen;//Written=MsgLen ?
	}

	if(logfile.Flag&LOGALWAYSCLOSE) CLOSE;

	SPL_Exit(logfile.L);

	return;
#else
	std::cout << Msg << std::endl;
#endif
}

void __cdecl LogfileF(LOGFILE &log, const char *format, ...)
{
	SPG_ArrayStackAlloc(char,message,MAXLOGMSG);
	va_list args;
	va_start(args, format);
	_vsnprintf(message, MAXLOGMSG, format, args);
	SPG_ArrayStackCheck(message);
	va_end(args);
	message[MAXLOGMSG - 1] = '\0';
	Logfile(log, message);
}

void SPG_CONV LogfileEmptyLine(LOGFILE& logfile)
{
#ifdef FDE
	if(SPL_Enter(logfile.L,"LogfileEmptyLine")==SPL_TIMEOUT) return;

	strcpy(logfile.sMsg,"\r\n");
	logfile.MsgLen=2;
	LogfileUpdate(logfile);

	if(logfile.Flag&LOGALWAYSCLOSE) OPENWA;

	if(logfile.hFile!=INVALID_HANDLE_VALUE)
	{
		DWORD Written;
		WriteFile((HANDLE)logfile.hFile,logfile.sMsg,logfile.MsgLen,&Written,0);
		logfile.CurSize+=logfile.MsgLen;//Written=MsgLen ?
	}

	if(logfile.Flag&LOGALWAYSCLOSE) CLOSE;

	SPL_Exit(logfile.L);
	return;
#else
	std::cout << std::endl;
#endif//FDE
}

#endif
