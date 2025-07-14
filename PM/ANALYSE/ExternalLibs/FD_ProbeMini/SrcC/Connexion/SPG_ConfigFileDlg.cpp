

#include "..\SPG_General.h"

#ifdef SPG_General_USECONFIGFILEDLG

#include "..\SPG_Includes.h"

#include "..\SPG_SysInc.h"

#include <tchar.h>
#include "flipDialogBox.h"

typedef struct
{
	int x;
	int y;
	int dx;
	int dy;
} CTRLRECT;

void SPG_CONV CtrlRectInit(CTRLRECT& R, int x, int y, int dx, int dy)
{
	R.x=x;
	R.y=y;
	R.dx=dx;
	R.dy=dy;
}

void SPG_CONV CtrlRectXCenter(CTRLRECT& R, CTRLRECT& Ref)
{
	R.x=(Ref.dx-R.dx)/2;
}

void SPG_CONV CtrlRectYCenter(CTRLRECT& R, CTRLRECT& Ref)
{
	R.y=(Ref.dy-R.dy)/2;
}

typedef struct
{
	HWND hDlg;
	CTRLRECT MainDlgRect;
	int CurrentYPos;
	int TopMargin;
	int LeftMargin;
	int LineHeight;
	int SpaceX;
	int SpaceY;
	int ClickID;
} DIALOG_STATE;

typedef struct
{
	DIALOG_STATE DS;
	SPG_CONFIGFILE* CFG;
} CFG_DIALOG_STATE;

#define PARAMtoID(PARAM) (((PARAM)<<2)+32)
#define IDtoPARAM(ID) (((ID)-32)>>2)
#define IDisVALID(ID) (((ID)>=32)&&(((ID)&3)==0))

int SPG_CONV CFG_DlgCFGInit(CFG_DIALOG_STATE& CDS)
{
	CHECK(CDS.CFG==0,"CFG_DlgInit",return 0);
	CHECK(CDS.CFG->Etat!=-1,"CFG_DlgInit",return 0);
	return -1;
}

int SPG_CONV CFG_DlgCFGOK(CFG_DIALOG_STATE& CDS)
{
	SPG_ArrayStackAllocZ(char,Msg,SPG_CONFIGCOMMENT);
	for(int i=1;i<CDS.CFG->NumParams;i++)
	{
		if(GetDlgItemText(CDS.DS.hDlg,PARAMtoID(i),Msg,SPG_CONFIGCOMMENT))
		{
			Msg[SPG_CONFIGCOMMENT-1]=0;
			char* r=Msg;
			if(enum_test(CDS.CFG->CP[i].Type,Int))
			{
				CDS.CFG->CP[i].i.Var_F=SPG_ReadInt(r);
			} 
			else if(enum_test(CDS.CFG->CP[i].Type,Float))
			{
				CDS.CFG->CP[i].f.Var_F=SPG_ReadFloat(r); //DoubleCF_ReadFloat(Msg,0,strlen(Msg));
			} 
			else if(enum_test(CDS.CFG->CP[i].Type,Double))
			{
				CDS.CFG->CP[i].d.Var_F=SPG_ReadDouble(r); //DoubleCF_ReadFloat(Msg,0,strlen(Msg));
			} 
			else if(enum_test(CDS.CFG->CP[i].Type,String))
			{
				strcpy(CDS.CFG->CP[i].s.S_F,r);
			}
		}
	}
	SPG_ArrayStackCheck(Msg);
	return -1;
}

int SPG_CONV CFG_DlgCFGCANCEL(CFG_DIALOG_STATE& CDS)
{
	return 0;
}

int SPG_CONV CFG_DlgCFGCommand(CFG_DIALOG_STATE& CDS, WPARAM wParam, LPARAM lParam, int Index)
{
	return FALSE;
}

BOOL CALLBACK DlgCFGCallBack(HWND hDlg, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	static LPARAM INITDIALOG_lParam;
	switch(uMsg)
	{
		case WM_INITDIALOG:
		{
			CHECK(lParam==0,"DlgCallBack:WM_INITDIALOG",return EndDialog(hDlg,0));
			INITDIALOG_lParam=lParam;
			CFG_DIALOG_STATE& CDS=*(CFG_DIALOG_STATE*)INITDIALOG_lParam;
			CDS.DS.hDlg=hDlg;
			if(CFG_DlgCFGInit(CDS)) 
			{
				return TRUE;
			}
			else
			{
				INITDIALOG_lParam=0;
				return EndDialog(hDlg,0);
			}
		}
		case WM_COMMAND:
		{
			//CHECK(INITDIALOG_lParam==0,"DlgCallBack:WM_COMMAND",return FALSE);
			if(INITDIALOG_lParam==0) return FALSE;
			CFG_DIALOG_STATE& CDS=*(CFG_DIALOG_STATE*)INITDIALOG_lParam;
			CDS.DS.ClickID=LOWORD(wParam);
            switch (LOWORD(wParam)) 
            { 
                case IDOK: 
				{
					INITDIALOG_lParam=0;
					return EndDialog(hDlg,CFG_DlgCFGOK(CDS));
				}
                case IDCANCEL: 
				{
					INITDIALOG_lParam=0;
					return EndDialog(hDlg,CFG_DlgCFGCANCEL(CDS));
				}
			}
			if(IDisVALID(LOWORD(wParam)))
			{
				return CFG_DlgCFGCommand(CDS,wParam,lParam,IDtoPARAM(LOWORD(wParam)));
			}
			return FALSE;// DefWindowProc(hDlg,uMsg,wParam,lParam);
		}
	}
	return FALSE;//DefWindowProc(hDlg,uMsg,wParam,lParam);
}

/*
Value Meaning 
0x0080 Button 
0x0081 Edit 
0x0082 Static 
0x0083 List box 
0x0084 Scroll bar 
0x0085 Combo box 
*/

void SPG_CONV CFG_CutString(char* FullStr, int StrLen, char Cut, char** Name, char** Value)
{
	*Value=*Name=FullStr;
	for(int i=0;i<StrLen;i++)
	{//coupe la chaine en deux à l'emplacement du signe '='
		if(FullStr[i]==0)
		{//fin de chaine atteinte
			break;
		}
		if(FullStr[i]=='=')
		{//cut position atteinte
			FullStr[i]=0;
			*Value=FullStr+i+1;
		}
	}
	return;
}

int SPG_CONV CFG_DialogCreateParamDisplay(CFG_DIALOG_STATE& CDS, flipDialogTemplate& Dlg, SPG_CONFIGPARAM& CP,int ID)
{
	DIALOG_STATE& DS=CDS.DS;
	CTRLRECT R;
	CtrlRectInit(R,0,DS.CurrentYPos+DS.TopMargin,DS.MainDlgRect.dx-2*DS.LeftMargin,DS.LineHeight);
	DS.CurrentYPos+=DS.LineHeight+DS.SpaceY;

	char Msg[SPG_CONFIGCOMMENT]; 
	CFG_ParamVarFToString(*CDS.CFG,CP,SPG_CFG_FORMAT_MINMAX|SPG_CFG_FORMAT_COMMENT,Msg,SPG_CONFIGCOMMENT);

	CtrlRectXCenter(R,DS.MainDlgRect);
	Dlg.AddEditBox(_T(Msg), WS_VISIBLE|WS_CHILD|ES_READONLY|ES_CENTER|ES_AUTOHSCROLL , WS_EX_STATICEDGE, R.x, R.y, R.dx, R.dy, (WORD)-1);
	return -1;
}

int SPG_CONV CFG_DialogCreateParamControl(CFG_DIALOG_STATE& CDS, flipDialogTemplate& Dlg, SPG_CONFIGPARAM& CP,int ID)
{
	DIALOG_STATE& DS=CDS.DS;
	CTRLRECT R;
	CtrlRectInit(R,0,DS.CurrentYPos+DS.TopMargin,DS.MainDlgRect.dx-2*DS.LeftMargin,DS.LineHeight);
	DS.CurrentYPos+=DS.LineHeight+DS.SpaceY;

	char Msg[SPG_CONFIGCOMMENT]; 
	if(CFG_ParamVarFToString(*CDS.CFG,CP,0,Msg,SPG_CONFIGCOMMENT))
	{
		char* Name; char* Value;
		CFG_CutString(Msg,SPG_CONFIGCOMMENT,'=',&Name, &Value);

		if(enum_test(CP.Type,Int))
		{
			R.dx/=2;
			CtrlRectXCenter(R,DS.MainDlgRect);
			R.dx-=DS.SpaceX;
			Dlg.AddEditBox(_T(Name), WS_VISIBLE|WS_CHILD|ES_READONLY|ES_AUTOHSCROLL , WS_EX_STATICEDGE, R.x, R.y, R.dx/2, R.dy, (WORD)-1);
			Dlg.AddEditBox(_T(Value), WS_VISIBLE|WS_CHILD|WS_TABSTOP|ES_AUTOHSCROLL|ES_NUMBER , WS_EX_STATICEDGE, R.x+DS.SpaceX+R.dx/2, R.y, R.dx-R.dx/2, R.dy, ID);
		}
		else if(enum_test(CP.Type,Float))
		{
			R.dx/=2;
			CtrlRectXCenter(R,DS.MainDlgRect);
			R.dx-=DS.SpaceX;
			Dlg.AddEditBox(_T(Name), WS_VISIBLE|WS_CHILD|ES_READONLY|ES_AUTOHSCROLL , WS_EX_STATICEDGE, R.x, R.y, R.dx/2, R.dy, (WORD)-1);
			Dlg.AddEditBox(_T(Value), WS_VISIBLE|WS_CHILD|WS_TABSTOP|ES_AUTOHSCROLL , WS_EX_STATICEDGE, R.x+DS.SpaceX+R.dx/2, R.y, R.dx-R.dx/2, R.dy, ID);
		}
		else if(enum_test(CP.Type,Double))
		{
			R.dx/=2;
			CtrlRectXCenter(R,DS.MainDlgRect);
			R.dx-=DS.SpaceX;
			Dlg.AddEditBox(_T(Name), WS_VISIBLE|WS_CHILD|ES_READONLY|ES_AUTOHSCROLL , WS_EX_STATICEDGE, R.x, R.y, R.dx/2, R.dy, (WORD)-1);
			Dlg.AddEditBox(_T(Value), WS_VISIBLE|WS_CHILD|WS_TABSTOP|ES_AUTOHSCROLL , WS_EX_STATICEDGE, R.x+DS.SpaceX+R.dx/2, R.y, R.dx-R.dx/2, R.dy, ID);
		}
		else if(enum_test(CP.Type,String))
		{
			CtrlRectXCenter(R,DS.MainDlgRect);
			R.dx-=DS.SpaceX;
			Dlg.AddEditBox(_T(Name), WS_VISIBLE|WS_CHILD|ES_READONLY|ES_AUTOHSCROLL , WS_EX_STATICEDGE, R.x, R.y, R.dx/3, R.dy, (WORD)-1);
			Dlg.AddEditBox(_T(Value), WS_VISIBLE|WS_CHILD|WS_TABSTOP|ES_AUTOHSCROLL , WS_EX_STATICEDGE, R.x+DS.SpaceX+R.dx/3, R.y, R.dx-R.dx/3, R.dy, ID);
		}
		return -1;
	}
	else
		return 0;
}

int SPG_CONV CFG_DialogCreateChoixBtn(DIALOG_STATE& DS, flipDialogTemplate& Dlg, int ID, char* BtnText, char* Description)
{
	CHECK(BtnText==0,"CFG_DialogCreateChoixBtn",return 0);
	CTRLRECT R;
	CtrlRectInit(R,0,DS.CurrentYPos+DS.TopMargin,DS.MainDlgRect.dx-2*DS.LeftMargin,DS.LineHeight);
	DS.CurrentYPos+=DS.LineHeight+DS.SpaceY;
	CtrlRectXCenter(R,DS.MainDlgRect);
	R.dx-=DS.SpaceX;
	Dlg.AddButton(_T(BtnText), WS_VISIBLE|WS_CHILD|WS_TABSTOP, 0, R.x, R.y-1, R.dx/3, R.dy+2, ID);
	if(Description) Dlg.AddEditBox(_T(Description), WS_VISIBLE|WS_CHILD|ES_READONLY|ES_AUTOHSCROLL , WS_EX_STATICEDGE, R.x+DS.SpaceX+R.dx/3, R.y, R.dx-R.dx/3, R.dy, (WORD)-1);
	return -1;
}

int SPG_CONV CFG_DialogCreateOK(DIALOG_STATE& DS, flipDialogTemplate& Dlg, char* Text)
{
	CTRLRECT R;
	CtrlRectInit(R,0,DS.CurrentYPos+DS.TopMargin,DS.MainDlgRect.dx-2*DS.LeftMargin,DS.LineHeight);
	R.dx/=2;
	R.dx-=DS.SpaceX;
	R.dx/=2;
	Dlg.AddButton(_T(Text), WS_VISIBLE|WS_CHILD|WS_TABSTOP, 0, R.x, R.y, R.dx, R.dy+DS.SpaceY, IDOK);
	return -1;
}

int SPG_CONV CFG_DialogCreateCancel(DIALOG_STATE& DS, flipDialogTemplate& Dlg, char* Text)
{
	CTRLRECT R;
	CtrlRectInit(R,0,DS.CurrentYPos+DS.TopMargin,DS.MainDlgRect.dx-2*DS.LeftMargin,DS.LineHeight);
	R.dx/=2;
	R.dx-=DS.SpaceX;
	R.dx/=2;
	CtrlRectXCenter(R,DS.MainDlgRect);
	Dlg.AddButton(_T(Text), WS_VISIBLE|WS_CHILD|WS_TABSTOP, 0, R.x, R.y, R.dx, R.dy+DS.SpaceY, IDCANCEL);
	return -1;
}

int SPG_CONV CFG_DialogCreateOKCancel(DIALOG_STATE& DS, flipDialogTemplate& Dlg)
{
	CTRLRECT R;
	CtrlRectInit(R,0,DS.CurrentYPos+DS.TopMargin,DS.MainDlgRect.dx-2*DS.LeftMargin,DS.LineHeight);
	R.dx/=2;
	CtrlRectXCenter(R,DS.MainDlgRect);
	R.dx-=DS.SpaceX;
	Dlg.AddButton(_T("OK"), WS_VISIBLE|WS_CHILD|WS_TABSTOP, 0, R.x, R.y, R.dx/2, R.dy+DS.SpaceY, IDOK);
	Dlg.AddButton(_T("Cancel"), WS_VISIBLE|WS_CHILD|WS_TABSTOP, 0, R.x+DS.SpaceX+R.dx/2, R.y, R.dx-R.dx/2, R.dy+DS.SpaceY, IDCANCEL);
	return -1;
}

int SPG_CONV CFG_DialogCreateOKCancelFirstLast(DIALOG_STATE& DS, flipDialogTemplate& Dlg, int bFirst, int bLast)
{
	CTRLRECT R;
	CtrlRectInit(R,0,DS.CurrentYPos+DS.TopMargin,DS.MainDlgRect.dx-2*DS.LeftMargin,DS.LineHeight);
	R.dx/=2;
	CtrlRectXCenter(R,DS.MainDlgRect);
	R.dx-=DS.SpaceX;
	if(bLast)
	{
		Dlg.AddButton(_T("OK"), WS_VISIBLE|WS_CHILD|WS_TABSTOP, 0, R.x+DS.SpaceX+R.dx/2, R.y, R.dx/2, R.dy+DS.SpaceY, IDOK);
	}
	else
	{
		Dlg.AddButton(_T(">>"), WS_VISIBLE|WS_CHILD|WS_TABSTOP, 0, R.x+DS.SpaceX+R.dx/2, R.y, R.dx/2, R.dy+DS.SpaceY, IDOK);
	}
	if(bFirst)
	{
		Dlg.AddButton(_T("Cancel"), WS_VISIBLE|WS_CHILD|WS_TABSTOP, 0, R.x, R.y, R.dx-R.dx/2, R.dy+DS.SpaceY, IDCANCEL);
	}
	else
	{
		Dlg.AddButton(_T("<<"), WS_VISIBLE|WS_CHILD|WS_TABSTOP, 0, R.x, R.y, R.dx-R.dx/2, R.dy+DS.SpaceY, IDCANCEL);
	}
	return -1;
}

int SPG_CONV CFG_CreateDialog(SPG_CONFIGFILE& CFG, int Flag, int iMin, int iMax, int bFirst, int bLast)
{
	SPG_StackAllocZ(CFG_DIALOG_STATE,CDS);
	DIALOG_STATE& DS=CDS.DS;
	CDS.CFG=&CFG;

	int DlgWidth=384;
	DS.LineHeight=11;
	DS.SpaceX=6;
	DS.SpaceY=6;
	DS.LeftMargin=2*DS.SpaceX;
	DS.TopMargin=2*DS.SpaceY;
	DS.ClickID=-1;

	CtrlRectInit(DS.MainDlgRect,0,0,DlgWidth,(DS.LineHeight+DS.SpaceX)*(iMax-iMin+1)+2*DS.TopMargin);

	flipDialogTemplate Dlg(_T(CFG.FileName[0]?CFG.FileName:"Parameters"), WS_VISIBLE|WS_SYSMENU|WS_CAPTION|DS_CENTER,
		DS.MainDlgRect.x, DS.MainDlgRect.y, DS.MainDlgRect.dx, DS.MainDlgRect.dy, _T("Tahoma"));

	if(Flag&CFG_DIALOG_HASNAMEPARAM) 
	{
		CFG_DialogCreateParamDisplay(CDS,Dlg,CFG.CP[1],PARAMtoID(1));
	}
	for(int i=iMin;i<iMax;i++)
	{
		CFG_DialogCreateParamControl(CDS,Dlg,CFG.CP[i],PARAMtoID(i));
		SPG_StackCheck(CDS);
	}

	CFG_DialogCreateOKCancelFirstLast(DS,Dlg,bFirst,bLast);

	int res = DialogBoxIndirectParam((HINSTANCE)Global.hInstance, Dlg, (HWND)Global.hWndWin, DlgCFGCallBack,(int)&CDS);//(DLGPROC)AssertDlgProc);
	SPG_StackCheck(CDS);
	return DS.ClickID;
}

int SPG_CONV CFG_CreateDialog(SPG_CONFIGFILE& CFG, int Flag)
{
	int iMin=1;
	if(Flag&CFG_DIALOG_HASNAMEPARAM) iMin=2;

	int BestLook=20;
	int Quality=0;
	for(int NumPanelsLayoutAttempt=18;NumPanelsLayoutAttempt<24;NumPanelsLayoutAttempt++)
	{
		int NumPanelsNum = CFG.NumParams - ((Flag&CFG_DIALOG_HASNAMEPARAM)?1:0);
		int NumPanelsDiv = NumPanelsLayoutAttempt - ((Flag&CFG_DIALOG_HASNAMEPARAM)?1:0);
		int NumPanels=(NumPanelsNum+NumPanelsDiv-1)/NumPanelsDiv; //arrondi par exces
		if((NumPanelsNum%NumPanels)>Quality)
		{
			Quality=NumPanelsNum%NumPanels;
			BestLook=NumPanelsLayoutAttempt;
		}
	}
	int NumPanelsLayoutAttempt = BestLook;
	int NumPanelsNum = CFG.NumParams - ((Flag&CFG_DIALOG_HASNAMEPARAM)?1:0);
	int NumPanelsDiv = NumPanelsLayoutAttempt - ((Flag&CFG_DIALOG_HASNAMEPARAM)?1:0);
	int NumPanels=(NumPanelsNum+NumPanelsDiv-1)/NumPanelsDiv; //arrondi par exces

	int CurrentPanel=0;
	int ClickID;
	do
	{
		int iMinPartial=iMin+CurrentPanel*NumPanelsDiv;
		int iMaxPartial=V_Min(iMin+CurrentPanel*NumPanelsDiv+NumPanelsDiv,CFG.NumParams);
		ClickID = CFG_CreateDialog(CFG,Flag,iMinPartial,iMaxPartial,CurrentPanel==0,CurrentPanel==(NumPanels-1));
	} while(((ClickID==IDOK)&&((++CurrentPanel)<NumPanels))||((ClickID==IDCANCEL)&&((--CurrentPanel)>=0)));

	if(ClickID!=IDOK) return IDCANCEL;

	if(Flag&CFG_DIALOG_CHECKMINMAX) 
	{
		for(int i=1;i<CFG.NumParams;i++)
		{
			SPG_CONFIGPARAM& CP=CFG.CP[i];
			if(enum_test(CP.Type,Int))
			{
				//char* staticMsg="Parameters";
				//char* Msg="Parameters";//(CFG.FileName[0])?(CFG.FileName):(staticMsg);
				if(enum_test(CP.Type,HasMin)) CHECKTWO(CP.i.Var_F<CP.i.Min,"Parameters",CP.Name,CP.i.Var_F=CP.i.Min);
				if(enum_test(CP.Type,HasMax)) CHECKTWO(CP.i.Var_F>CP.i.Max,"Parameters",CP.Name,CP.i.Var_F=CP.i.Max);
			}
			else if(enum_test(CP.Type,Float))
			{
				if(enum_test(CP.Type,HasMin)) CHECKTWO(CP.f.Var_F<CP.f.Min,"Parameters",CP.Name,CP.f.Var_F=CP.f.Min);
				if(enum_test(CP.Type,HasMax)) CHECKTWO(CP.f.Var_F>CP.f.Max,"Parameters",CP.Name,CP.f.Var_F=CP.f.Max);
			}
			else if(enum_test(CP.Type,Double))
			{
				if(enum_test(CP.Type,HasMin)) CHECKTWO(CP.d.Var_F<CP.d.Min,"Parameters",CP.Name,CP.d.Var_F=CP.d.Min);
				if(enum_test(CP.Type,HasMax)) CHECKTWO(CP.d.Var_F>CP.d.Max,"Parameters",CP.Name,CP.d.Var_F=CP.d.Max);
			}
		}
	}
	CFG_FlushToFile(CFG,SPG_CFG_FORMAT_TAB|SPG_CFG_FORMAT_COMMENT|SPG_CFG_FORMAT_MINMAX,false); //ecrit le fichier - attention à ne pas reécraser lors du CFG_Close (mettre flush=0)
	if(Flag&CFG_DIALOG_APPLY) CFG_ParamsFromF(CFG); //ne pas utiliser si l'appli ne supporte pas les changement en live (bornes)
	return ClickID;
}

typedef struct
{
	DIALOG_STATE DS;
} CFG_CHOIX_DIALOG_STATE;

int SPG_CONV CFG_CX_DlgInit(CFG_CHOIX_DIALOG_STATE& CXDS)
{
	return -1;
}

int SPG_CONV CFG_CX_DlgOK(CFG_CHOIX_DIALOG_STATE& CXDS)
{
	return -1;
}

int SPG_CONV CFG_CX_DlgCANCEL(CFG_CHOIX_DIALOG_STATE& CXDS)
{
	return 0;
}

int SPG_CONV CFG_CX_DlgCommand(CFG_CHOIX_DIALOG_STATE& CXDS, WPARAM wParam, LPARAM lParam, int Index)
{
	return EndDialog(CXDS.DS.hDlg,CFG_CX_DlgOK(CXDS));
}

BOOL CALLBACK DlgCXCallBack(HWND hDlg, UINT uMsg, WPARAM wParam, LPARAM lParam)
{
	static LPARAM INITDIALOG_lParam;
	switch(uMsg)
	{
		case WM_INITDIALOG:
		{
			CHECK(lParam==0,"DlgCallBack:WM_INITDIALOG",return EndDialog(hDlg,0));
			INITDIALOG_lParam=lParam;
			CFG_CHOIX_DIALOG_STATE& CXDS=*(CFG_CHOIX_DIALOG_STATE*)INITDIALOG_lParam;
			CXDS.DS.hDlg=hDlg;
			if(CFG_CX_DlgInit(CXDS)) 
			{
				return TRUE;
			}
			else
			{
				return EndDialog(hDlg,0);
			}
		}
		case WM_COMMAND:
		{
			if(INITDIALOG_lParam==0) return FALSE;
			CFG_CHOIX_DIALOG_STATE& CXDS=*(CFG_CHOIX_DIALOG_STATE*)INITDIALOG_lParam;
			CXDS.DS.ClickID=LOWORD(wParam);
			switch (LOWORD(wParam)) 
            { 
                case IDOK: 
				{
					return EndDialog(hDlg,CFG_CX_DlgOK(CXDS));
				}
                case IDCANCEL: 
				{
					return EndDialog(hDlg,CFG_CX_DlgCANCEL(CXDS));
				}
			}
			if(IDisVALID(LOWORD(wParam)))
			{
				CFG_CX_DlgCommand(CXDS,wParam,lParam,IDtoPARAM(LOWORD(wParam)));
			}
			return FALSE;// DefWindowProc(hDlg,uMsg,wParam,lParam);
		}
	}
	return FALSE;//DefWindowProc(hDlg,uMsg,wParam,lParam);
}

int SPG_CONV CFG_CreateChooseDialog(char* Text, char** ChoixBtn, char** ChoixDescr, int MaxChoix)
{
	CHECK(ChoixBtn==0,"CFG_CreateChooseDialog",return -1);
	CHECK(ChoixDescr==0,"CFG_CreateChooseDialog",return -1);
	CHECK(MaxChoix==0,"CFG_CreateChooseDialog",return -1);
	CFG_CHOIX_DIALOG_STATE CXDS;
	SPG_ZeroStruct(CXDS);

	DIALOG_STATE& DS=CXDS.DS;

	int DlgWidth=384;
	DS.LineHeight=11;
	DS.SpaceX=6;
	DS.SpaceY=6;
	DS.LeftMargin=2*DS.SpaceX;
	DS.TopMargin=2*DS.SpaceY;
	DS.ClickID=-1;

	int yCount=0;
	int i;
	for(i=0;i<MaxChoix;i++)
	{
		if(ChoixBtn[i]) yCount++;
	}

	CtrlRectInit(DS.MainDlgRect,0,0,DlgWidth,(DS.LineHeight+DS.SpaceX)*(yCount+1)+2*DS.TopMargin);

	flipDialogTemplate Dlg(_T(Text), WS_VISIBLE|WS_SYSMENU|WS_CAPTION|DS_CENTER,
		DS.MainDlgRect.x, DS.MainDlgRect.y, DS.MainDlgRect.dx, DS.MainDlgRect.dy, _T("Tahoma"));

	for(i=0;i<MaxChoix;i++)
	{
		if(ChoixBtn[i]) CFG_DialogCreateChoixBtn(DS,Dlg,PARAMtoID(i),ChoixBtn[i],ChoixDescr[i]);
	}
	CFG_DialogCreateCancel(DS,Dlg,"Cancel");

	//(Global.hWndWin?(HWND)Global.hWndWin:GetDesktopWindow())
	int res = DialogBoxIndirectParam((HINSTANCE)Global.hInstance, Dlg, (HWND)Global.hWndWin, DlgCXCallBack,(int)&CXDS);//(DLGPROC)AssertDlgProc);

	if(!IDisVALID(CXDS.DS.ClickID)) return -1;

	return IDtoPARAM(CXDS.DS.ClickID);
}

#endif
