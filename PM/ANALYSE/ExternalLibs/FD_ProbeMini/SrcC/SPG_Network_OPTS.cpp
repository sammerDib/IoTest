
#include "SPG_General.h"

#ifdef SPG_General_USENetwork_OPTS

#include "SPG_Includes.h"

#include <string.h>
#include <stdio.h>

//procedures de controle des autres applications

int SPG_CONV SPG_CreateNetOpts(SPG_NET_OPTS& SNO, SPG_NET_PROTOCOL& SNP, G_Ecran& E, int AutoUpdate)
{
	memset(&SNO,0,sizeof(SPG_NET_OPTS));
	CHECK(SNP.SN.Etat==0,"SPG_CreateNetOpts: SPG_NET_PROTOCOL nul",return 0);
	B_LoadButtonsLib(SNO.BL,E,1,0,"..\\SrcC\\Interface","Buttons.bmp");
	C_LoadCaracLib(SNO.CL,E,"..\\SrcC\\Carac","CaracNoir.bmp");
	SNO.Ecran=E;
	SNO.SNP=&SNP;
	SNO.IP0=SNO.SNP->SN.LocalNetAddr.IP0;
	SNO.IP1=SNO.SNP->SN.LocalNetAddr.IP1;
	SNO.IP2=SNO.SNP->SN.LocalNetAddr.IP2;
	SNO.IP3=SNO.SNP->SN.LocalNetAddr.IP3;
	SNO.Port=SNO.SNP->SN.LocalNetAddr.Port;
	SNO.ReqScreenRate=65536*2;

	int BSpaceX=SNO.BL.SizeX[ClickSprite]+SNO.CL.SizeX;
	int BSpaceY=SNO.BL.SizeY[ClickSprite]+SNO.CL.SpaceY+2;
	int CurrentX=2;
	int CurrentY=2*SNO.CL.SpaceY;
	SNO.BIP0=B_CreateCliquableNumericIntButton(SNO.BL,E,SNO.CL,CurrentX,CurrentY,3,(int*)&SNO.IP0);
	CurrentX+=5*SNO.CL.SizeX;
	SNO.BIP1=B_CreateCliquableNumericIntButton(SNO.BL,E,SNO.CL,CurrentX,CurrentY,3,(int*)&SNO.IP1);
	CurrentX+=5*SNO.CL.SizeX;
	SNO.BIP2=B_CreateCliquableNumericIntButton(SNO.BL,E,SNO.CL,CurrentX,CurrentY,3,(int*)&SNO.IP2);
	CurrentX+=5*SNO.CL.SizeX;
	SNO.BIP3=B_CreateCliquableNumericIntButton(SNO.BL,E,SNO.CL,CurrentX,CurrentY,3,(int*)&SNO.IP3);
	CurrentX+=5*SNO.CL.SizeX;
	SNO.BPort=B_CreateCliquableNumericIntButton(SNO.BL,E,SNO.CL,CurrentX,CurrentY,5,(int*)&SNO.Port);
	
	CurrentX+=10*SNO.CL.SizeX;
	B_CreateCliquableNumericIntButton(SNO.BL,E,SNO.CL,CurrentX,CurrentY,10,(int*)&SNO.ReqScreenRate);

	CurrentY+=BSpaceY;
	CurrentX=BSpaceX;
	BSpaceX=SNO.BL.SizeX[ClickSprite]+3*SNO.CL.SizeX;
	SNO.BScreenView=B_CreateCheckButton(SNO.BL,E,CurrentX,CurrentY);
	B_PrintLabel(SNO.BL,SNO.BScreenView,SNO.CL,"Screen\nView");
	SNO.BRequestControl=B_CreateCheckButton(SNO.BL,E,CurrentX+BSpaceX,CurrentY);
	B_PrintLabel(SNO.BL,SNO.BRequestControl,SNO.CL,"Request\nControl");
	SNO.BErrorReport=B_CreateCheckButton(SNO.BL,E,CurrentX+2*BSpaceX,CurrentY);
	B_PrintLabel(SNO.BL,SNO.BErrorReport,SNO.CL,"Error\nReport");
	SNO.BBreak=B_CreateClickButton(SNO.BL,E,CurrentX+3*BSpaceX,CurrentY);
	B_PrintLabel(SNO.BL,SNO.BBreak,SNO.CL,"Break");
	SNO.BPing=B_CreateClickButton(SNO.BL,E,CurrentX+4*BSpaceX,CurrentY);
	B_PrintLabel(SNO.BL,SNO.BPing,SNO.CL,"Ping");
	SNO.BPacket=B_CreateClickButton(SNO.BL,E,CurrentX+5*BSpaceX,CurrentY);
	B_PrintLabel(SNO.BL,SNO.BPacket,SNO.CL,"Packet\nSize");

	G_InitSousEcran(SNO.CslEcran,SNO.Ecran,CurrentX+6*BSpaceX,0,SNO.Ecran.SizeX-(CurrentX+6*BSpaceX),SNO.Ecran.SizeY);

//SPG_Console* SPG_CONV Console_Create(G_Ecran* E, C_Lib* CL, int AutoUpdate)
	Console_Create(SNO.Console,&SNO.CslEcran,&SNO.CL,0);

	B_RedrawButtonsLib(SNO.BL,0);

	if (AutoUpdate) 
	{
		SNO.Etat|=SNO_AUTOUPDATE;
		SPG_AddUpdateOnDoEvents((SPG_CALLBACK)SPG_UpdateNetOpts,&SNO,0);
	}

	S_InitTimer(SNO.SPing,"Timer Ping NetOpts");
	S_InitTimer(SNO.DebitTimer,"DebitTimer NetOpts");

	SNO.DebitCumule=0;
	S_StartTimer(SNO.SPing);
	S_StartTimer(SNO.DebitTimer);
//	S_StartTimer(SNO.SRefreshScreen);

	Console_Add(SNO.Console,"\n\n");

	Console_Add(SNO.Console,SNO.SNP->SN.ServerName);

	char Msg[128];
	sprintf(Msg,"%d.%d.%d.%d:%d\n",SNO.SNP->SN.LocalNetAddr.IP0,SNO.SNP->SN.LocalNetAddr.IP1,SNO.SNP->SN.LocalNetAddr.IP2,SNO.SNP->SN.LocalNetAddr.IP3,SNO.SNP->SN.LocalNetAddr.Port);
	Console_Add(SNO.Console,Msg);

	return SNO.Etat|=SNO_OK;
}

void SPG_CONV SPG_UpdateNetOpts(SPG_NET_OPTS& SNO)
{
	if (SNO.Etat) 
	{
		B_UpdateButtonsLib(SNO.BL,SPG_Global_MouseX,SPG_Global_MouseY,SPG_Global_MouseLeft);
		SNO.IP0=V_Sature(SNO.IP0,0,255);
		SNO.IP1=V_Sature(SNO.IP1,0,255);
		SNO.IP2=V_Sature(SNO.IP2,0,255);
		SNO.IP3=V_Sature(SNO.IP3,0,255);
		SNO.Port=V_Sature(SNO.Port,0,65535);

		SNO.IPDest.IP0=SNO.IP0;
		SNO.IPDest.IP1=SNO.IP1;
		SNO.IPDest.IP2=SNO.IP2;
		SNO.IPDest.IP3=SNO.IP3;
		SNO.IPDest.Port=SNO.Port;

		S_TimerCountType T;
		S_GetTimerRunningCount(SNO.SPing,T);


		if(B_IsChangedToClick(SNO.BL,SNO.BScreenView))
		{
			SPG_DownloadScreenNetOptsStop(SNO);
			if(SNP_Send_DWORD(*(SNO.SNP),SNO.IPDest,SNP_SCREEN_REQUEST,SNO.ReqScreenRate))
			{
			char Msg[128];
			sprintf(Msg,"ScreenView request to %d.%d.%d.%d:%d",SNO.IP0,SNO.IP1,SNO.IP2,SNO.IP3,SNO.Port);
			Console_Add(SNO.Console,Msg);
			}
		}
		if(B_IsChangedToNotClick(SNO.BL,SNO.BScreenView))
		{
			SPG_DownloadScreenNetOptsStop(SNO);
			if(SNP_Send_DWORD(*(SNO.SNP),SNO.IPDest,SNP_SCREEN_REQUEST,0))
			{
			char Msg[128];
			sprintf(Msg,"ScreenView stop to %d.%d.%d.%d:%d",SNO.IP0,SNO.IP1,SNO.IP2,SNO.IP3,SNO.Port);
			Console_Add(SNO.Console,Msg);
			}
		}

		if(B_IsChangedToClick(SNO.BL,SNO.BRequestControl))
		{
			if(SNP_Send_BYTE(*(SNO.SNP),SNO.IPDest,SNP_CONTROL_REQUEST,1))
			{
			char Msg[128];
			sprintf(Msg,"Control request to %d.%d.%d.%d:%d",SNO.IP0,SNO.IP1,SNO.IP2,SNO.IP3,SNO.Port);
			Console_Add(SNO.Console,Msg);
			}
		}
		if(B_IsChangedToNotClick(SNO.BL,SNO.BRequestControl))
		{
			if(SNP_Send_BYTE(*(SNO.SNP),SNO.IPDest,SNP_CONTROL_REQUEST,0))
			{
			char Msg[128];
			sprintf(Msg,"Control stop sent %d.%d.%d.%d:%d",SNO.IP0,SNO.IP1,SNO.IP2,SNO.IP3,SNO.Port);
			Console_Add(SNO.Console,Msg);
			}
		}

		if(B_IsChangedToClick(SNO.BL,SNO.BPing))
		{
			if(SNP_Send(*(SNO.SNP),SNO.IPDest,SNP_PING_REQUEST,&T,sizeof(S_TimerCountType)))
			{
			char Msg[128];
			sprintf(Msg,"Ping request sent %d.%d.%d.%d:%d",SNO.IP0,SNO.IP1,SNO.IP2,SNO.IP3,SNO.Port);
			Console_Add(SNO.Console,Msg);
			}
		}

		if(B_IsChangedToClick(SNO.BL,SNO.BErrorReport))
		{
			if(SNP_Send_BYTE(*(SNO.SNP),SNO.IPDest,SNP_ERROR_REPORT, 1))
			{
			char Msg[128];
			sprintf(Msg,"Error report request to %d.%d.%d.%d:%d",SNO.IP0,SNO.IP1,SNO.IP2,SNO.IP3,SNO.Port);
			Console_Add(SNO.Console,Msg);
			}
		}
		if(B_IsChangedToNotClick(SNO.BL,SNO.BErrorReport))
		{
			if(SNP_Send_BYTE(*(SNO.SNP),SNO.IPDest,SNP_ERROR_REPORT, 0))
			{
			char Msg[128];
			sprintf(Msg,"Error report stop to %d.%d.%d.%d:%d",SNO.IP0,SNO.IP1,SNO.IP2,SNO.IP3,SNO.Port);
			Console_Add(SNO.Console,Msg);
			}
		}
		if(B_IsChangedToClick(SNO.BL,SNO.BBreak))
		{
			if(SNP_Send_BYTE(*(SNO.SNP),SNO.IPDest,SNP_BREAK, 1))
			{
			char Msg[128];
			sprintf(Msg,"Break to %d.%d.%d.%d:%d",SNO.IP0,SNO.IP1,SNO.IP2,SNO.IP3,SNO.Port);
			Console_Add(SNO.Console,Msg);
			}
		}

		if(B_IsChangedToClick(SNO.BL,SNO.BPacket))
		{
			/*
			if(B_IsClick(SNO.BL,SNO.BRequestControl))
			{
			*/
				SNP_SetOptimalPacketSize(*(SNO.SNP),SNO.IPDest);
				char Msg[128];
				sprintf(Msg,"New packet size %d",(*(SNO.SNP)).MaxSize);
				Console_Add(SNO.Console,Msg);
			/*
			}
			else
			{
				char Msg[128];
				sprintf(Msg,"Not controlled. Packet size %d",(*(SNO.SNP)).MaxSize);
				Console_Add(SNO.Console,Msg);
			}
			*/
		}

		//SPG_NET_ADDR SNA;
		if((*(SNO.SNP)).Message.Type) SNO.DebitCumule+=(*(SNO.SNP)).Len;

		float DebitTime;
		S_GetTimerRunningTime(SNO.DebitTimer,DebitTime);
		if(DebitTime>5.0)
		{
			char Msg[512];
			sprintf(Msg,"Current incoming rate: %d bytes/sec",(int)(SNO.DebitCumule/DebitTime));
			Console_Add(SNO.Console,Msg);
			S_StopTimer(SNO.DebitTimer);
			S_ResetTimer(SNO.DebitTimer);
			S_StartTimer(SNO.DebitTimer);
			SNO.DebitCumule=0;
		}

		if(SNP_IsType((*(SNO.SNP)),SNP_SCREEN_RESPONSE)&&SNP_IsLen((*(SNO.SNP)),sizeof(G_Ecran)))
		{
			SPG_DownloadScreenNetOptsStop(SNO);
			CHECK(SPG_DownloadScreenNetOptsStart(SNO,(G_Ecran*)SNO.SNP->Message.M)==0,"SPG_UpdateNetOpts: ScreenView error",;);
		}
		if(SNP_IsType((*(SNO.SNP)),SNP_SCREEN_CONTENT_SEND))
		{
			SPG_DownloadScreenNetOptsSetMsg(SNO,SNO.SNP->Message.M,SNO.SNP->Len);
		}

		//S_TimerCountType R;
		//if(SNP_ReadAny(*(SNO.SNP),SNA,SNP_PING_RESPONSE,&R))
		SNP_IF_READ_IsTypeIsLen((*(SNO.SNP)),SNP_PING_RESPONSE,S_TimerCountType,R)
			char Msg[128];
			sprintf(Msg,"%d.%d.%d.%d:%d Ping ",(int)SNO.SNP->SNA.IP0,(int)SNO.SNP->SNA.IP1,(int)SNO.SNP->SNA.IP2,(int)SNO.SNP->SNA.IP3,(int)SNO.SNP->SNA.Port);
			CF_GetString(Msg,1000.0*((double)(T-*R)/Global.CPUClock),5);
			strcat(Msg,"ms");
			Console_Add(SNO.Console,Msg);
		SNP_END_IF_READ(SNP)
		if(SNP_IsType((*(SNO.SNP)),SNP_LIST))
		{
			char Msg[128];
			sprintf(Msg,"%d.%d.%d.%d:%d ",(int)SNO.SNP->SNA.IP0,(int)SNO.SNP->SNA.IP1,(int)SNO.SNP->SNA.IP2,(int)SNO.SNP->SNA.IP3,(int)SNO.SNP->SNA.Port);
			Console_Add(SNO.Console,Msg);
			Console_Add(SNO.Console,(char*)(SNO.SNP->Message.M));
			if (!(Global.SNP&&SPG_IsValidNetAddr(Global.ErrorReport)))
			{
				SPG_List2S(Msg,(char*)(SNO.SNP->Message.M));
			}
		}
		if(SNP_IsType((*(SNO.SNP)),SNP_CONTROL_RESPONSE))
		{
			char Msg[128];
			if(SNO.SNP->Message.M[0])
			{
			sprintf(Msg,"You are controlling %d.%d.%d.%d:%d ",(int)SNO.SNP->SNA.IP0,(int)SNO.SNP->SNA.IP1,(int)SNO.SNP->SNA.IP2,(int)SNO.SNP->SNA.IP3,(int)SNO.SNP->SNA.Port);
			if(SPG_IsEqualNetAddr(SNO.SNP->SNA,SNO.IPDest))
			{
				B_SetAndRedraw(SNO.BL,SNO.BRequestControl,B_Click|B_Waiting);
			}
			}
			else
			{
			sprintf(Msg,"You have no control on %d.%d.%d.%d:%d ",(int)SNO.SNP->SNA.IP0,(int)SNO.SNP->SNA.IP1,(int)SNO.SNP->SNA.IP2,(int)SNO.SNP->SNA.IP3,(int)SNO.SNP->SNA.Port);
			if(SPG_IsEqualNetAddr(SNO.SNP->SNA,SNO.IPDest))
			{
				B_SetAndRedraw(SNO.BL,SNO.BRequestControl,B_Normal|B_Waiting);
			}
			}
			Console_Add(SNO.Console,Msg);
		}
		Console_Update(SNO.Console);
	}
	return;
}

void SPG_CONV SPG_CloseNetOpts(SPG_NET_OPTS& SNO)
{
	if (SNO.Etat) 
	{
		if (SNO.Etat&SNO_AUTOUPDATE) SPG_KillUpdateOnDoEventsByParam(&SNO);
		if(B_IsClick(SNO.BL,SNO.BScreenView))
		{
			SNP_Send_DWORD(*(SNO.SNP),SNO.IPDest,SNP_SCREEN_REQUEST,0);
		}
		if(B_IsClick(SNO.BL,SNO.BRequestControl))
		{
			SNP_Send_BYTE(*(SNO.SNP),SNO.IPDest,SNP_CONTROL_REQUEST,0);
		}
		if(B_IsClick(SNO.BL,SNO.BErrorReport))
		{
			SNP_Send_BYTE(*(SNO.SNP),SNO.IPDest,SNP_ERROR_REPORT, 0);
		}
		SPG_DownloadScreenNetOptsStop(SNO);
		Console_Close(SNO.Console);
		G_CloseEcran(SNO.CslEcran);
		B_CloseButtonsLib(SNO.BL);
		C_CloseCaracLib(SNO.CL);
		S_StopTimer(SNO.SPing);
		S_CloseTimer(SNO.SPing);
		S_StopTimer(SNO.DebitTimer);
		S_CloseTimer(SNO.DebitTimer);
		//S_StopTimer(SNO.SRefreshScreen);
	}
	memset(&SNO,0,sizeof(SPG_NET_OPTS));
	return;
}

int SPG_CONV SPG_DownloadScreenNetOptsStart(SPG_NET_OPTS& SNO,G_Ecran* E)
{
	memcpy(&SNO.ERemote,E,sizeof(G_Ecran));
	char Msg[128];
	sprintf(Msg,"%d.%d.%d.%d:%d Screen SizeX=%d SizeY=%d Depth=%d",(int)SNO.SNP->SNA.IP0,(int)SNO.SNP->SNA.IP1,(int)SNO.SNP->SNA.IP2,(int)SNO.SNP->SNA.IP3,(int)SNO.SNP->SNA.Port,SNO.ERemote.SizeX,SNO.ERemote.SizeY,8*SNO.ERemote.POCT);
	Console_Add(SNO.Console,Msg);
	if(V_InclusiveBound(SNO.ERemote.POCT,3,4)&&V_InclusiveBound(G_POCT(Global.Ecran),3,4))
	{
		CHECK(
		G_InitMemoryEcran(
			SNO.ERemote,
			G_POCT(Global.Ecran),
			SNO.ERemote.SizeX,
			SNO.ERemote.SizeY)==0,
			"SPG_DownloadScreenNetOpts: Creation de l'ecran echouee",return 0);
	}
	else
	{
		CHECK(
		G_InitMemoryEcran(
			SNO.ERemote,
			SNO.ERemote.POCT,
			SNO.ERemote.SizeX,
			SNO.ERemote.SizeY)==0,
			"SPG_DownloadScreenNetOpts: Creation de l'ecran echouee",return 0);
	}
	SPG_SetMemName(G_GetPix(SNO.ERemote),"Remote screen");
	if(MELINK_InitRcv(SNO.ScreenRcvState.MRCV,MELINK_TYPE_RGB,G_GetPix(SNO.ERemote),G_Pitch(SNO.ERemote),G_POCT(SNO.ERemote),G_SizeX(SNO.ERemote),G_SizeY(SNO.ERemote)))
	{
		SNO.ScreenRcvState.Remote=SNO.SNP->SNA;
	}
	return -1;
}

void SPG_CONV SPG_DownloadScreenNetOptsSetMsg(SPG_NET_OPTS& SNO, BYTE* Msg, int Len)
{
	if(SNO.ScreenRcvState.MRCV.Etat)
	{
		if(SPG_IsEqualNetAddr(SNO.ScreenRcvState.Remote,SNO.SNP->SNA))
		{
			MELINK_FullSetRcvMsg(SNO.ScreenRcvState.MRCV,Msg,Len);
		}
	}
	return;
}

void SPG_CONV SPG_DownloadScreenNetOptsStop(SPG_NET_OPTS& SNO)
{
	if(SNO.ScreenRcvState.MRCV.Etat) MELINK_CloseRcv(SNO.ScreenRcvState.MRCV);
	if(SNO.ERemote.Etat) G_CloseEcran(SNO.ERemote);
	memset(&SNO.ScreenRcvState,0,sizeof(MELINK_SCREEN_RCV_STATE));
	return;
}

#endif

