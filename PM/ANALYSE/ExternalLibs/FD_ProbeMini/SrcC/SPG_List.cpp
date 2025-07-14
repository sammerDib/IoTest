
#include "SPG_General.h"

#ifdef DebugList

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include "BreakHook.h"

#include <memory.h>
#include <stdio.h>
#include <string.h>

void SPG_CONV SPG_List_CountMsg()
{
#ifdef SPG_General_USEGlobal
	if (Global.TotalMsg==8) 
#endif
	{
		while(GetAsyncKeyState(VK_ESCAPE)||GetAsyncKeyState(VK_SPACE)||GetAsyncKeyState(VK_RETURN));
#ifdef FDE
		DoEvents(SPG_DOEV_READ_WIN_EVENTS|SPG_DOEV_FLUSH_WIN_EVENTS);
#endif
#ifdef SPG_General_USEWindows
		if (MessageBox(
			(HWND)Global.hWndWin,
			L"Disable messagebox error report for next messages ?",
			fdwstring(SPG_COMPANYNAME),MB_YESNO|MB_APPLMODAL|MB_SETFOREGROUND)!=IDYES)
			Global.TotalMsg=0;
#endif
	}
	return;
}

int SPG_CONV SPG_GetLastWinError(char* Msg, int N)
{
#ifdef SPG_General_USEWindows
	Msg[0]=0;
    DWORD dw = GetLastError(); 
	if(dw==0) return 0;

	wchar_t* wmsg = new wchar_t[N];
    FormatMessage( FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS, 0,
        dw, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), wmsg, N, NULL );
	strcpy_s(Msg, N, fdstring(wmsg));
	delete wmsg;
	return dw;
#else
	return 0;
#endif
}

// ################# SPG_List affiche les messages correspondant aux test de préconditions (macros check(c,m,r)) ####################

#ifdef SPG_General_USEGlobal

void SPG_CONV SPG_List(const char* Txt)
{
	int ShowMsgBox=Global.EnableList>-1;
	int DoBreak=0;

	if(SPG_RunAllCheckCallbacks(Txt)&&(Global.EnableList<1)) ShowMsgBox=0;//boucle qui appelle toutes les callback recevant les messages des macros CHECK (une précondition est non valide)

	if(Global.SPG_List_Recurse) ShowMsgBox=0;

	Global.SPG_List_Recurse++;

	if(ShowMsgBox)
	{
		SPG_List_CountMsg();

#ifdef SPG_General_USEWindows
		if (Global.TotalMsg<8) 
		{
			while(GetAsyncKeyState(VK_ESCAPE)||GetAsyncKeyState(VK_SPACE)||GetAsyncKeyState(VK_RETURN));

			int r=MessageBox( (HWND)Global.hWndWin,
				fdwstring(Txt),	//Txt contient le message d'erreur de la message box
				fdwstring(SPG_COMPANYNAME),
#ifdef SPG_DEBUGCONFIG
				MB_OKCANCEL|
#endif
				MB_APPLMODAL|MB_SETFOREGROUND);
#ifdef FDE
			DoEvents(SPG_DOEV_MIN);//sinon ca clique sur l'image de fond
#endif
			if(r==IDCANCEL) DoBreak=1;
		}
		else
		{
			DoBreak=0;
		}
#endif
	}

#ifdef SPG_DEBUGCONFIG
	if(DoBreak) BreakHook(); //note: le parametre Txt contient le message d'erreur de la message box (selectionner Txt , SHIFT+F9
#endif

	Global.TotalMsg++; //note: le parametre Txt contient le message d'erreur de la message box (selectionner Txt , SHIFT+F9
	Global.SPG_List_Recurse--;
	return;
}

#else

void SPG_CONV SPG_List(const char* Txt)
{
#ifdef SPG_General_USEWindows
	while(GetAsyncKeyState(VK_ESCAPE)||GetAsyncKeyState(VK_SPACE)||GetAsyncKeyState(VK_RETURN));

	if(MessageBox( (HWND)Global.hWndWin,
		Txt,	//Txt contient le message d'erreur de la message box
		SPG_COMPANYNAME,
#ifdef BreakHookDebugBreak
		MB_OKCANCEL|
#endif
		MB_APPLMODAL|MB_SETFOREGROUND)!=IDOK)
	{
#ifdef BreakHookDebugBreak
		BreakHook();		//point d'arret sur message d'erreur si on clique annuler sur une messagebox d'erreur
#endif
	}
	DoEvents(SPG_DOEV_MIN);//sinon ca clique sur l'image de fond
#endif
}

#endif

#ifdef BreakHookDebugBreak
#define scrop(t) {if(strlen(t)>512) {BreakHook();t[512]=0;}}
#else
// FDE #define scrop(t) {if(strlen(t)>512) {t[512]=0;}}
#define scrop(t) {}
#endif

void SPG_CONV SPG_List1(const char* Txt,void* Val)
{
	SPG_ArrayStackAlloc(char,Msg,1024);
	scrop(Txt);
	sprintf(Msg,"%s %p",Txt,Val);
	SPG_ArrayStackCheck(Msg);
	SPG_List(Msg);
	return;
}

void SPG_CONV SPG_List2S(const char* Txt,const char* Txt1)
{
	SPG_ArrayStackAlloc(char,Msg,1024);
	scrop(Txt);scrop(Txt1);
	sprintf(Msg,"%s\n%s",Txt,Txt1);
	SPG_ArrayStackCheck(Msg);
	SPG_List(Msg);
	return;
}

void SPG_CONV SPG_List3S(const char* Txt,const char* Txt1,const char* Txt2)
{
	SPG_ArrayStackAlloc(char,Msg,1024);
	scrop(Txt);scrop(Txt1);scrop(Txt2);
	sprintf(Msg,"%s\n%s\n%s",Txt,Txt1,Txt2);
	SPG_ArrayStackCheck(Msg);
	SPG_List(Msg);
	return;
}

void SPG_CONV SPG_ListSSN(const char* Txt,const char* Txt1, int Val)
{
	SPG_ArrayStackAlloc(char,Msg,1024);
	scrop(Txt);scrop(Txt1);
	sprintf(Msg,"%s\n%s\n%d",Txt,Txt1,Val);
	SPG_ArrayStackCheck(Msg);
	SPG_List(Msg);
	return;
}

void SPG_CONV SPG_List2(const char* Txt,int Val,const char* Txt1,int Val1)
{
	SPG_ArrayStackAlloc(char,Msg,1024);
	scrop(Txt);scrop(Txt1);
	sprintf(Msg,"%s%d%s%d",Txt,Val,Txt1,Val1);
	SPG_List(Msg);
	return;
}

void SPG_CONV SPG_ListSNSS(const char* Txt,int Val,const char* Txt1,const char* Txt2)
{
	SPG_ArrayStackAlloc(char,Msg,1024);
	scrop(Txt);scrop(Txt1);
	sprintf(Msg,"%s%d%s%s",Txt,Val,Txt1,Txt2);
	SPG_ArrayStackCheck(Msg);
	SPG_List(Msg);
	return;
}

void SPG_CONV SPG_ListSNSSS(const char* Txt,int Val,const char* Txt1,const char* Txt2,const char* Txt3)
{
	SPG_ArrayStackAlloc(char,Msg,1024);
	scrop(Txt);scrop(Txt1);scrop(Txt2);scrop(Txt3);
	sprintf(Msg,"%s%d%s%s\r\n%s",Txt,Val,Txt1,Txt2,Txt3);
	SPG_ArrayStackCheck(Msg);
	SPG_List(Msg);
	return;
}
void SPG_CONV SPG_List3(const char* Txt,int Val,const char* Txt1,int Val1,const char* Txt2,int Val2)
{
	SPG_ArrayStackAlloc(char,Msg,1024);
	scrop(Txt);scrop(Txt1);scrop(Txt2);
	sprintf(Msg,"%s%d%s%d%s%d",Txt,Val,Txt1,Val1,Txt2,Val2);
	SPG_ArrayStackCheck(Msg);
	SPG_List(Msg);
	return;
}

#endif


