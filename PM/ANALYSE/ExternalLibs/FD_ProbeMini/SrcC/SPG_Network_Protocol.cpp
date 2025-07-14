

#include "SPG_General.h"

#ifdef SPG_General_USENetwork_Protocol

#include "SPG_Includes.h"

#include <string.h>

int SPG_CONV SNP_Init(SPG_NET_PROTOCOL& SNP, int Port)
{
	memset(&SNP,0,sizeof(SPG_NET_PROTOCOL));
	CHECK(SPG_InitUDP(SNP.SN,Port)==0,"SNP_Init: SPG_InitUDP echoue",return 0);

	SNP_AjustMaxPacketSize(SNP,SNP_MIN_MSG);

	while(SPG_ReadUDP(SNP.SN,SNP.SNA,&SNP.Message,SNP_HEADER_SIZE+SNP_MAX_MSG));
	SNP.Message.Type=0;
	return -1;
}

void SPG_CONV SNP_AjustMaxPacketSize(SPG_NET_PROTOCOL& SNP, int NewSize)
{

	CHECK(!V_InclusiveBound(NewSize,SNP_MIN_MSG,SNP_MAX_MSG),"SNP_AjustMaxPacketSize: Taille de packet invalide",return);

	SNP.MaxSize=NewSize;
	return;
}

void SPG_CONV SNP_Close(SPG_NET_PROTOCOL& SNP)
{
	SPG_CloseUDP(SNP.SN);
	memset(&SNP,0,sizeof(SPG_NET_PROTOCOL));
	return;
}

int SPG_CONV SNP_Send(SPG_NET_PROTOCOL& SNP,SPG_NET_ADDR& SNA,int MessageType)
{
	CHECK(MessageType==0,"SNP_Send: Mauvais type de message demande en ecriture",return 0);
	CHECK(!V_IsBound(MessageType,1,SNP_MAX_MSG_TYPE),"SNP_Send: Mauvais type de message demande en ecriture",return 0);
	return SPG_SendUDP(SNP.SN,SNA,&MessageType,SNP_HEADER_SIZE);
}

int SPG_CONV SNP_Send(SPG_NET_PROTOCOL& SNP,SPG_NET_ADDR& SNA,int MessageType,void* Data,int Len)
{
	CHECK(MessageType==0,"SNP_Send: Mauvais type de message demande en ecriture",return 0);
	CHECK(!V_IsBound(MessageType,1,SNP_MAX_MSG_TYPE),"SNP_Send: Mauvais type de message demande en ecriture",return 0);
	SNP_MESSAGE Message;
	Message.Type=MessageType;
	if(Data) memcpy(Message.M,Data,Len);
	return SPG_SendUDP(SNP.SN,SNA,&Message,SNP_HEADER_SIZE+Len);
}

int SPG_CONV SNP_Send_BYTE(SPG_NET_PROTOCOL& SNP,SPG_NET_ADDR& SNA,int MessageType, BYTE Data)
{
	CHECK(MessageType==0,"SNP_Send_BYTE: Mauvais type de message demande en ecriture",return 0);
	CHECK(!V_IsBound(MessageType,1,SNP_MAX_MSG_TYPE),"SNP_Send_BYTE: Mauvais type de message demande en ecriture",return 0);
	SNP_MESSAGE Message;
	Message.Type=MessageType;
	Message.M[0]=Data;
	return SPG_SendUDP(SNP.SN,SNA,&Message,SNP_HEADER_SIZE+1);
}

int SPG_CONV SNP_Send_TWO_BYTE(SPG_NET_PROTOCOL& SNP,SPG_NET_ADDR& SNA,int MessageType, BYTE Data0, BYTE Data1)
{
	CHECK(MessageType==0,"SNP_Send_BYTE: Mauvais type de message demande en ecriture",return 0);
	CHECK(!V_IsBound(MessageType,1,SNP_MAX_MSG_TYPE),"SNP_Send_BYTE: Mauvais type de message demande en ecriture",return 0);
	SNP_MESSAGE Message;
	Message.Type=MessageType;
	Message.M[0]=Data0;
	Message.M[1]=Data1;
	return SPG_SendUDP(SNP.SN,SNA,&Message,SNP_HEADER_SIZE+2);
}

int SPG_CONV SNP_Send_WORD(SPG_NET_PROTOCOL& SNP,SPG_NET_ADDR& SNA,int MessageType, WORD Data)
{
	CHECK(MessageType==0,"SNP_Send_WORD: Mauvais type de message demande en ecriture",return 0);
	CHECK(!V_IsBound(MessageType,1,SNP_MAX_MSG_TYPE),"SNP_Send_WORD: Mauvais type de message demande en ecriture",return 0);
	SNP_MESSAGE Message;
	Message.Type=MessageType;
	*((WORD*)Message.M)=Data;
	return SPG_SendUDP(SNP.SN,SNA,&Message,SNP_HEADER_SIZE+2);
}

int SPG_CONV SNP_Send_DWORD(SPG_NET_PROTOCOL& SNP,SPG_NET_ADDR& SNA,int MessageType, DWORD Data)
{
	CHECK(MessageType==0,"SNP_Send_DWORD: Mauvais type de message demande en ecriture",return 0);
	CHECK(!V_IsBound(MessageType,1,SNP_MAX_MSG_TYPE),"SNP_Send_WORD: Mauvais type de message demande en ecriture",return 0);
	SNP_MESSAGE Message;
	Message.Type=MessageType;
	*((DWORD*)Message.M)=Data;
	return SPG_SendUDP(SNP.SN,SNA,&Message,SNP_HEADER_SIZE+4);
}

int SPG_CONV SNP_Send_char(SPG_NET_PROTOCOL& SNP,SPG_NET_ADDR& SNA,int MessageType, char* Data)
{
	CHECK(MessageType==0,"SNP_Send_WORD: Mauvais type de message demande en ecriture",return 0);
	CHECK(!V_IsBound(MessageType,1,SNP_MAX_MSG_TYPE),"SNP_Send_char: Mauvais type de message demande en ecriture",return 0);
	SNP_MESSAGE Message;
	Message.Type=MessageType;
	int Len=strlen(Data);
	strcpy((char*)Message.M,Data);
	return SPG_SendUDP(SNP.SN,SNA,&Message,SNP_HEADER_SIZE+Len+1);
}

#ifdef SPG_General_USEMELINK
int SPG_CONV SNP_Send_MELINK(SPG_NET_PROTOCOL& SNP,SPG_NET_ADDR& SNA,int MessageType,MELINK_SEND& MSND)
{
	CHECK(MessageType==0,"SNP_Send_MELINK: Mauvais type de message demande en ecriture",return 0);
	CHECK(!V_IsBound(MessageType,1,SNP_MAX_MSG_TYPE),"SNP_Send_MELINK: Mauvais type de message demande en ecriture",return 0);
	SNP_MESSAGE Message;
	Message.Type=MessageType;
	int Len;
	//MELINK_SaveSendState(MSND);
	if(Len=MELINK_FullFillSendMsg(MSND,Message.M,SNP_DYN_MSG(SNP)))
	{
		if(Len=SPG_SendUDP(SNP.SN,SNA,&Message,SNP_HEADER_SIZE+Len))
		{
			return Len;
		}
		else
		{
			//MELINK_RevertToSaved(MSND);
			return 0;
		}
	}
	else
	{
		return 0;
	}
}
#endif

//retourne 0 si le message est processe, -1 sinon
bool SPG_CONV SNP_ProcessSysMsg(SPG_NET_PROTOCOL& SNP, bool MaskSysMsg)
{
	CHECK(SNP.Message.Type==0,"SNP_ProcessMsg: Message nul",return true);
	if(SNP.Message.Type>=SNP_SYSMSG) return false;
	
	if(SNP.Message.Type==SNP_PING_REQUEST) 
	{
		SNP_Send(SNP,SNP.SNA,SNP_PING_RESPONSE,SNP.Message.M,SNP.Len);
	}
	
	if(SNP_IsType(SNP,SNP_PACKETSIZE)&&(SNP.Len>=2)) SNP_AjustMaxPacketSize(SNP,*(WORD*)SNP.Message.M);

	return MaskSysMsg;//message systeme
}

int SPG_CONV SNP_Update(SPG_NET_PROTOCOL& SNP, bool MaskSysMsg)
{
	do
	{
		if((SNP.Len=(SPG_ReadUDP(SNP.SN,SNP.SNA,&SNP.Message,SNP_HEADER_SIZE+SNP_MAX_MSG)-SNP_HEADER_SIZE))<0)
		{
			//sortie s'il n'y a pas de message
			SNP.Message.Type=0;
			return 0;
		}
	} while(SNP_ProcessSysMsg(SNP, MaskSysMsg));//message systeme

	return -1;//message non masque
}

void SPG_CONV SNP_SetOptimalPacketSize(SPG_NET_PROTOCOL& SNP,SPG_NET_ADDR& SNA)
{
	S_CreateTimer(TimeOut,"SNP_SetOptimalPacketSize");
	int PacketSize=SNP_MIN_MSG;
	int PacketGranularite=64;

	while(SNP_Update(SNP,true));

	while(1)
	{
		SPG_StackAllocZ(SNP_MESSAGE,PM);
		for(int i=0;i<SNP_MAX_MSG;i++)
		{
			PM.M[i]=(i^0xA5)+TimeOut.StartTime;
		}
		S_ResetTimer(TimeOut);
		SNP_Send(SNP,SNA,SNP_PING_REQUEST,&PM.M,PacketSize);
		S_StartTimer(TimeOut);

		while(1)
		{
			SNP_Update(SNP,false);
			DoEvents(SPG_DOEV_UPDATE);
			if(SNP_IsPresent(SNP,SNA,SNP_PING_RESPONSE,PacketSize)) break;

			float Delai;
			S_GetTimerRunningTime(TimeOut,Delai);
			if(Delai>SNP_MaxPacketSizeTime) break;
		}
		SPG_StackCheck(PM);
		S_StopTimer(TimeOut);
		if(SNP.Message.Type!=SNP_PING_RESPONSE) break;
		PacketSize+=PacketGranularite;
		if(PacketGranularite*8<=PacketSize) PacketGranularite<<=1;
		if(PacketSize>SNP_MAX_MSG) break;
	}

	PacketSize-=PacketGranularite;
	if(PacketSize<SNP_MIN_MSG) PacketSize=SNP_MIN_MSG;

	while(SNP_Update(SNP,true));

	SNP_Send_WORD(SNP,SNA,SNP_PACKETSIZE,PacketSize);
	SNP_AjustMaxPacketSize(SNP,PacketSize);

	S_CloseTimer(TimeOut);

	return;
}

#endif


