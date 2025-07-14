 
#include "SPG_General.h"

#ifdef SPG_General_USEGlobal
#ifdef SPG_General_USENetwork_Protocol

#include "SPG_Includes.h"

#include <string.h>

//procedures de tache de fond de toutes les applications

#define NETBLIT_AllowedCPU 1.0/32.0

int SPG_CONV SPG_OpenNetworkInput(SPG_NET_PROTOCOL& SNP)
{
	CHECK(SNP.SN.Etat==0,"SPG_NET_PROTOCOL nul",return 0);
	S_InitTimer(Global.NetControlTimer,"NetworkMouseLocking");
	Global.SNP=&SNP;
#ifdef SPG_General_USEULIPS
	ULIPS_StartNetUpdate(Global.ULIST,SNP.SN.LocalNetAddr.Port);
	char IPFile[MaxProgDir];
	SPG_ConcatPath(IPFile,Global.LogDir,"ULIPS_USR.txt");
	ULIPS_Load(Global.ULIST,IPFile);
	SPG_ConcatPath(IPFile,Global.LogDir,"ULIPS_SRV.txt");
	ULIPS_Load(Global.ULIST,IPFile);
	SPG_AddUpdateOnDoEvents((SPG_CALLBACK)SPG_UlipsUpdate,&Global.ULIST,0);
#endif
	return -1;
}

void SPG_CONV SPG_CloseNetworkInput()
{
#ifdef SPG_General_USEULIPS
	SPG_KillUpdateOnDoEventsByParam(&Global.ULIST);
	char IPFile[MaxProgDir];
	SPG_ConcatPath(IPFile,Global.ProgDir,"ULIPS_USR.txt");
	ULIPS_Save(Global.ULIST,IPFile);
	ULIPS_StopNetUpdate(Global.ULIST);
#endif
	SPG_CloseNetworkView();
	SPG_CloseNetworkControl();
	SPG_CloseNetworkErrorReport();
	Global.SNP=0;
	return;
}

int SPG_CONV SPG_OpenNetworkErrorReport(SPG_NET_ADDR& SNA)
{
	CHECK(Global.SNP==0,"SPG_OpenNetworkErrorReport: Pas de controle reseau autorise",return 0);
	Global.ErrorReport=SNA;
	return -1;
}

int SPG_CONV SPG_CloseNetworkErrorReport()
{
	memset(&Global.ErrorReport,0,sizeof(SPG_NET_ADDR));
	CHECK(Global.SNP==0,"SPG_CloseNetworkErrorReport: Pas de controle reseau autorise",return 0);
	return -1;
}

int SPG_CONV SPG_OpenNetworkControl(SPG_NET_ADDR& SNA)
{
	CHECK(Global.SNP==0,"SPG_OpenNetworkControl: Pas de controle reseau autorise",return 0);
	SPG_CloseNetworkControl();
	Global.ControlSource=SNA;
	if(SPG_IsValidNetAddr(Global.ControlSource))
	{
		SNP_Send_BYTE(*(Global.SNP),Global.ControlSource,SNP_CONTROL_RESPONSE,1);
		ULIPS_ChangeState(Global.ULIST,ULIPS_STATE_BUSY);
	}
	else
		SPG_CloseNetworkControl();
	return -1;
}

int SPG_CONV SPG_CloseNetworkControl()
{
	if((Global.SNP)&&(SPG_IsValidNetAddr(Global.ControlSource)))
		SNP_Send_BYTE(*(Global.SNP),Global.ControlSource,SNP_CONTROL_RESPONSE,0);

	memset(&Global.ControlSource,0,sizeof(SPG_NET_ADDR));
	CHECK(Global.SNP==0,"SPG_CloseNetworkControl: Pas de controle reseau autorise",return 0);
	return -1;
}

int SPG_CONV SPG_OpenNetworkView(SPG_NET_ADDR& SNA, DWORD AllowedRate, float AllowedCPU)
{
	CHECK(Global.SNP==0,"SPG_OpenNetworkControl: Pas de controle reseau autorise",return 0);
	SPG_CloseNetworkView();
	Global.ScreenSendState.Destination=SNA;
	Global.ScreenSendState.AllowedRate=AllowedRate;
	Global.ScreenSendState.AllowedCPU=AllowedCPU;
	Global.ScreenSendState.LenPacketSent=AllowedRate;//pour calmer les demarrages, sinon 0

	S_InitTimer(Global.ScreenSendState.ScreenSend,"Debit screen view");

	S_StartTimer(Global.ScreenSendState.ScreenSend);
	if(SPG_IsValidNetAddr(SNA)) 
	{
		SNP_Send(*(Global.SNP),SNA,SNP_SCREEN_RESPONSE,&Global.Ecran,sizeof(G_Ecran));
		MELINK_InitSend(Global.ScreenSendState.MSND,MELINK_TYPE_RGB,G_GetPix(Global.Ecran),G_Pitch(Global.Ecran),G_POCT(Global.Ecran),G_SizeX(Global.Ecran),G_SizeY(Global.Ecran));
		ULIPS_ChangeState(Global.ULIST,ULIPS_STATE_BUSY);
	}
	else
		SPG_CloseNetworkView();
	return -1;
}

int SPG_CONV SPG_CloseNetworkView()
{
	if(S_IsOK(Global.ScreenSendState.ScreenSend))
	{
		if(S_IsStarted(Global.ScreenSendState.ScreenSend)) S_StopTimer(Global.ScreenSendState.ScreenSend);
		S_CloseTimer(Global.ScreenSendState.ScreenSend);
	}
	MELINK_CloseSend(Global.ScreenSendState.MSND);
	memset(&Global.ScreenSendState,0,sizeof(MELINK_SCREEN_SEND_STATE));
	return -1;
}

void SPG_CONV SPG_SendMouseState(SPG_NET_PROTOCOL& SNP, SPG_NET_ADDR& SNA, int MouseX,int MouseY,int MouseLeft,int MouseRight)
{
	SPG_NetMouseState NetMouse;
	NetMouse.MouseX=MouseX;
	NetMouse.MouseY=MouseY;
	NetMouse.MouseFlags=
		(MouseLeft?SPG_NETMOUSE_LEFT:0)|
		(MouseRight?SPG_NETMOUSE_RIGHT:0)|
		SPG_NETMOUSE_INWINDOW;
	SNP_Send(SNP,SNA,SNP_MOUSE,&NetMouse,sizeof(SPG_NetMouseState));
	return;
}

//retourne 0 si le message est processe, -1 sinon
bool SPG_CONV SPG_ProcessSpgMsg(SPG_NET_PROTOCOL& SNP, SPG_NET_ADDR& SNA, bool MaskSpgMsg)
{
	//SPG_NET_PROTOCOL& SNP
	if(SNP.Message.Type>=SNP_SPGMSG) return false;

	if(SPG_UnderNetControl)
	{
		if(S_IsStarted(Global.NetControlTimer)) S_StopTimer(Global.NetControlTimer);
		S_ResetTimer(Global.NetControlTimer);
		S_StartTimer(Global.NetControlTimer);
	}

	if(SNP.Message.Type==SNP_ERROR_REPORT)
	{
		if(SNP.Message.M[0])
			SPG_OpenNetworkErrorReport(SNP.SNA);
		else
			SPG_CloseNetworkErrorReport();
		return MaskSpgMsg;
	}

	if(SNP.Message.Type==SNP_CONTROL_REQUEST)
	{
		if(SNP.Message.M[0])
			SPG_OpenNetworkControl(SNP.SNA);
		else
			SPG_CloseNetworkControl();
		return MaskSpgMsg;
	}
	if(SNP.Message.Type==SNP_SCREEN_REQUEST)
	{
		if((*(DWORD*)SNP.Message.M)!=0)
			SPG_OpenNetworkView(SNP.SNA,*(DWORD*)SNP.Message.M,NETBLIT_AllowedCPU);
		else
			SPG_CloseNetworkView();
		return MaskSpgMsg;
	}

	if(SNP.Message.Type==SNP_P_STARTLOADPROFILE)
	{
		P_NetHook_Load(SNP,SNP.SNA);
	}
	if(SNP.Message.Type==SNP_P_STARTSAVEPROFILE)
	{
		P_NetHook_Save(SNP,SNP.SNA);
	}
	if(SNP.Message.Type==SNP_C_STARTLOADCUT)
	{
		Cut_NetHook_Load(SNP,SNP.SNA);
	}
	if(SNP.Message.Type==SNP_C_STARTSAVECUT)
	{
		Cut_NetHook_Save(SNP,SNP.SNA);
	}

/*
le premier if est facultatif
*/
	if(SPG_UnderNetControl)
	{
		if(SPG_IsValidNetAddr(SNA))
		{
			if(SNP_IsPresent(SNP,SNA,SNP_MOUSE,sizeof(SPG_NetMouseState)))
			{
#define NetMouse (*(SPG_NetMouseState*)SNP.Message.M)
				SPG_Global_MouseX=NetMouse.MouseX;
				SPG_Global_MouseY=NetMouse.MouseY;
				SPG_Global_MouseLeft=((NetMouse.MouseFlags&SPG_NETMOUSE_LEFT)!=0);
				SPG_Global_MouseRight=((NetMouse.MouseFlags&SPG_NETMOUSE_RIGHT)!=0);
				SPG_Global_MouseInWindow=((NetMouse.MouseFlags&SPG_NETMOUSE_INWINDOW)!=0);
#undef NetMouse
				//SetCursorPos(SPG_Global_MouseX,SPG_Global_MouseY);
				return MaskSpgMsg;
			}
		}
	}

	return MaskSpgMsg;//message systeme
#undef SNP
}

void SPG_CONV SPG_UlipsUpdate(ULIPS_List& L)
{
	if(L.Etat) 
	{
		ULIPS_UpdateNetUpdate(L);

	if(!(
		SPG_IsValidNetAddr(Global.ErrorReport)||
		SPG_IsValidNetAddr(Global.ControlSource)||
		SPG_IsValidNetAddr(Global.ScreenSendState.Destination)))
	{
		if(ULocal(L).State&ULIPS_STATE_BUSY) ULIPS_ChangeState(L,ULIPS_STATE_NORMAL);
	}

	}
	return;
}

int SPG_CONV SPG_NetworkUpdate(SPG_NET_PROTOCOL& SNP, SPG_NET_ADDR& SNA, bool MaskSysMsg, bool MaskSpgMsg)
{
	//SPG_NET_PROTOCOL& SNP
	CHECK(SNP.SN.Etat==0,"SPG_NetworkUpdate: Pas de protocole",return 0);
	do
	{
		if(SNP_Update(SNP,MaskSysMsg)==0)
		{
			return 0;//0=pas de message
		}
	} while(SPG_ProcessSpgMsg(SNP,SNA,MaskSpgMsg));//message systeme=masqué

	return -1;//-1=a trouve un message non masqué
#undef SNP
}

#ifdef SPG_General_USEMELINK
int SPG_CONV SPG_Network_SendScreen(SPG_NET_PROTOCOL& SNP, MELINK_SCREEN_SEND_STATE& ScreenSendState)
{
	CHECK(ScreenSendState.MSND.Etat==0,"SPG_Network_SendScreen: MELINK_SEND non initialise",return 0);
	CHECK(SPG_IsValidNetAddr(ScreenSendState.Destination)==0,"SPG_Network_SendScreen: Adresse destination incorrecte",return 0);
	float ScreenSent;
	S_GetTimerRunningTime(Global.ScreenSendState.ScreenSend,ScreenSent);
	if ((ScreenSent*Global.ScreenSendState.AllowedRate)<(Global.ScreenSendState.LenPacketSent)) return 0;
	Global.ScreenSendState.LenPacketSent=SNP_Send_MELINK(SNP,ScreenSendState.Destination,SNP_SCREEN_CONTENT_SEND,Global.ScreenSendState.MSND);
	S_StopTimer(Global.ScreenSendState.ScreenSend);
	S_ResetTimer(Global.ScreenSendState.ScreenSend);
	S_StartTimer(Global.ScreenSendState.ScreenSend);
	return Global.ScreenSendState.LenPacketSent;
}
#endif

int SPG_CONV SPG_Network_DoEvents(SPG_NET_PROTOCOL& SNP, SPG_NET_ADDR& SNA, int MessageType)
{
	S_CreateTimer(TimeOut,"SPG_NetworkDoEvents");
	S_StartTimer(TimeOut);
	int RetVal=-1;
	SPG_NetworkUpdate(SNP,SNA,true,false);//needs to receive break message
	while((SNP_IsFrom(SNP,SNA)==0)||(SNP_IsType(SNP,MessageType)==0))
	{
		CHECK(SNP_IsFrom(SNP,SNA)&&SNP_IsType(SNP,SNP_BREAK),"SPG_Network_DoEvents: Break",RetVal=0;break);
		float Delai;
		S_GetTimerRunningTime(TimeOut,Delai);
		CHECK(Delai>SNP_MaxPingTime,"SPG_Network_DoEvents: Time out",RetVal=0;break);
	SPG_NetworkUpdate(SNP,SNA,true,false);//needs to receive break message
	DoEvents(SPG_DOEV_UPDATE);
		/*
		if(SPG_UnderNetControl)
			DoEvents(SPG_DOEV_ALL_NOBLIT);
		else
			SPG_NetworkUpdate(SNP,SNA,true,true);
		*/
	}
	S_CloseTimer(TimeOut);
	return RetVal;
}


int SPG_CONV SPG_NetworkDoEvents_LongWait(SPG_NET_PROTOCOL& SNP, SPG_NET_ADDR& SNA, int MessageType)
{
	S_CreateTimer(TimeOut,"SPG_NetworkDoEvents_LongWait");
	S_StartTimer(TimeOut);
	int RetVal=-1;
	SPG_NetworkUpdate(SNP,SNA,true,false);//needs to receive break message
	while((SNP_IsFrom(SNP,SNA)==0)||(SNP_IsType(SNP,MessageType)==0))
	{
		CHECK(SNP_IsFrom(SNP,SNA)&&SNP_IsType(SNP,SNP_BREAK),"SPG_Network_DoEvents_LongWait: Break",RetVal=0;break);
		float Delai;
		S_GetTimerRunningTime(TimeOut,Delai);
		CHECK(Delai>SNP_MaxWaitTime,"SPG_NetworkDoEvents_LongWait: Time out",RetVal=0;break);
	SPG_NetworkUpdate(SNP,SNA,true,false);//needs to receive break message
	DoEvents(SPG_DOEV_UPDATE);
		/*
		if(SPG_UnderNetControl)
			DoEvents(SPG_DOEV_ALL_NOBLIT);
		else
			SPG_NetworkUpdate(SNP,SNA,true,true);
		*/
	}
	S_CloseTimer(TimeOut);
	return RetVal;
}


int SPG_CONV SPG_Download_Send(SPG_NET_PROTOCOL& SNP,SPG_NET_ADDR& SNA, int SendMsg, int ReadMsg, void* Data, int Len)
{
	BYTE NrBloc=0;
	BYTE* ReadingPos=(BYTE*)Data;
	int LenRemaining=Len;
	while(SPG_Network_DoEvents(SNP,SNA,ReadMsg))
	{
		CHECK(!SNP_IsPresent(SNP,SNA,ReadMsg,1),"SPG_Download_Send: BREAK",break);
		CHECK(NrBloc!=SNP.Message.M[0],"SPG_Download_Send: desynchronisation",break);
		int LenSent=V_Min(LenRemaining,SNP_DYN_MSG(SNP));
		CHECK(SNP_Send(SNP,SNA,SendMsg,ReadingPos,LenSent)==0,"SPG_Download_Send: Envoi echoue",break);
		ReadingPos+=LenSent;
		LenRemaining-=LenSent;
		NrBloc++;
		if(LenRemaining<=0) return -1;
	}
	SNP_Send_BYTE(SNP,SNA,SNP_BREAK,1);
	return 0;
}

int SPG_Download_Read(SPG_NET_PROTOCOL& SNP,SPG_NET_ADDR& SNA, int SendMsg, int ReadMsg, void* Data, int Len)
{
	BYTE NrBloc=0;
	BYTE* WritingPos=(BYTE*)Data;
	SNP_Send_BYTE(SNP,SNA,ReadMsg,NrBloc);
	while(SPG_Network_DoEvents(SNP,SNA,SendMsg))
	{
		CHECK((SNP_IsFrom(SNP,SNA)==0)||(SNP_IsType(SNP,SendMsg)==0),"SPG_Download_Read: BREAK",break);
		if(WritingPos-((BYTE*)Data)+SNP.Len>Len) break;
		memcpy(WritingPos,SNP.Message.M,SNP.Len);
		WritingPos+=SNP.Len;
		if(WritingPos-((BYTE*)Data)==Len) return -1;
		NrBloc++;
		CHECK(SNP_Send_BYTE(SNP,SNA,ReadMsg,NrBloc)==0,"SPG_Download_Read: Envoi echoue",break);
	}
	SNP_Send_BYTE(SNP,SNA,SNP_BREAK,1);
	return 0;
}

#endif
#endif


