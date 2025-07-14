
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#include "..\SPG_Includes.h"

#include "SPG_PacketBuffer.h"

#include <memory.h>
#include <string.h>

int SPG_CONV pkBufferCreate(PACKET_BUFFER& P, int Size)
{
	SPG_ZeroStruct(P);
	P.Size=Size;
	P.MaxPacket=(Size+15)/16;
	P.B=SPG_MemAlloc(P.Size,"pkBufferCreate");
	CHECK(P.B==0,"pkBufferCreate",return 0);
	P.P=SPG_TypeAlloc(P.MaxPacket,PACKET_BUFFER_ELT,"pkBufferCreate");
	CHECK(P.P==0,"pkBufferCreate",return 0);
	S_InitTimer(P.T,"pkBufferCreate");
	S_StartTimer(P.T);
	//S_GetTimerRunninCount(P.StartTime);
	return P.MaxPacket;
}

void SPG_CONV pkBufferDestroy(PACKET_BUFFER& P)
{
	S_StopTimer(P.T);
	S_CloseTimer(P.T);
	SPG_MemFree(P.B);
	SPG_MemFree(P.P);
	SPG_ZeroStruct(P);
	return;
}

int SPG_CONV pkBufferFillLastPacket(PACKET_BUFFER& P, void* Data, int Size, int Flag)
{
	if(Flag&PACKET_CMD_CREATE)
	{
		CHECK(P.NumPacket==P.MaxPacket,"pkBufferFillLastPacket: Buffer full",pkBufferRemoveFirstPacket(P));
		if(P.NumPacket)
		{
			DbgCHECK((P.P[P.NumPacket-1].State==PACKET_PARTIAL)&&(P.P[P.NumPacket-1].Len>0),"pkBufferFillLastPacket:PACKET_CMD_CREATE")
			P.P[P.NumPacket].Start=P.P[P.NumPacket-1].Start+P.P[P.NumPacket-1].Len;
		}
		else
		{
			P.P[P.NumPacket].Start=0;
		}
		P.P[P.NumPacket].Len=0;
		S_GetTimerRunningCount(P.T,P.P[P.NumPacket].StartTransmit);
		P.P[P.NumPacket].State=PACKET_PARTIAL;
		P.NumPacket++;
	}

	if(Data&&Size)
	{
		CHECK(P.NumPacket==0,"pkBufferFillElt: Invalid flag",return 0);
		DbgCHECK((P.P[P.NumPacket-1].State&PACKET_FILLSTATE_MASK)!=PACKET_PARTIAL,"pkBufferFillElt: Invalid flag");
		CHECK(P.P[P.NumPacket-1].Start+P.P[P.NumPacket-1].Len+Size>P.Size,"pkBufferFillElt: Buffer plein",return P.NumPacket-1);
		if(Flag&PACKET_CMD_DATA)
		{
			if(P.P[P.NumPacket-1].State&PACKET_HASDATA)
			{
				DbgCHECK(
					(P.P[P.NumPacket-1].DataStart+P.P[P.NumPacket-1].DataLen)!=
					(P.P[P.NumPacket-1].Start+P.P[P.NumPacket-1].Len),
					"pkBufferFillLastPacket: Use of PACKET_ELT_DATA must be consecutive");
				P.P[P.NumPacket-1].DataLen+=Size;
			}
			else
			{
				P.P[P.NumPacket-1].DataStart=P.P[P.NumPacket-1].Start;
				P.P[P.NumPacket-1].DataLen=Size;
			}
			P.P[P.NumPacket-1].State|=PACKET_HASDATA;
		}
		memmove((BYTE*)P.B+P.P[P.NumPacket-1].Start+P.P[P.NumPacket-1].Len,Data,Size);//car le message recu (Data) est parfois directement stocké dans l'espace libre du packet buffer (pkBufferAppendPtr)
		P.P[P.NumPacket-1].Len+=Size;
	}

	if(Flag&PACKET_CMD_CLOSE)
	{
		DbgCHECK((P.P[P.NumPacket-1].State&PACKET_FILLSTATE_MASK)!=PACKET_PARTIAL,"pkBufferFillElt: Invalid flag");
		P.P[P.NumPacket-1].State&=~PACKET_PARTIAL;
		P.P[P.NumPacket-1].State|=PACKET_FULL;
	}

	S_GetTimerRunningCount(P.T,P.P[P.NumPacket-1].EndTransmit);

//	DbgCHECK(P.P[P.NumPacket-1].Len==0,"pkBufferFillLastPacket");
	return P.P[P.NumPacket-1].State|=(Flag&PACKET_ERRORSTATE_MASK);
}

int SPG_CONV pkBufferCutLastPacket(PACKET_BUFFER& P, int FlagLeft, int CutOffset, int FlagRight)//inserterrorpacket; ou trash all si le protocole respecte les paquets
{
	CHECK((FlagLeft&PACKET_FULL)==0,"pkBufferCutLastPacket",FlagLeft|=PACKET_FULL);
	CHECK(P.NumPacket==0,"pkBufferCutLastPacket: Invalid flag",return 0);
	CHECK(P.P[P.NumPacket-1].Len==0,"pkBufferCutLastPacket: Invalid flag",return 0);
	CHECK(CutOffset==0,"pkBufferCutLastPacket: Invalid flag",return 0);
	CHECK(P.P[P.NumPacket-1].Len<CutOffset,"pkBufferCutLastPacket: Invalid flag",return P.P[P.NumPacket-1].State|=FlagLeft);
	CHECK(P.NumPacket==P.MaxPacket,"pkBufferCutLastPacket: Buffer full",pkBufferRemoveFirstPacket(P));
	P.P[P.NumPacket-1].State|=FlagLeft;
	S_GetTimerRunningCount(P.T,P.P[P.NumPacket-1].EndTransmit);
	if(CutOffset==P.P[P.NumPacket-1].Len) return P.P[P.NumPacket-1].State;
	P.P[P.NumPacket].State=FlagRight;
	P.P[P.NumPacket].Start=P.P[P.NumPacket-1].Start+CutOffset;
	P.P[P.NumPacket].Len=P.P[P.NumPacket-1].Len-CutOffset;
	P.P[P.NumPacket-1].Len=CutOffset;
	DbgCHECK(P.P[P.NumPacket].Len==0,"pkBufferCutLastPacket");
	P.P[P.NumPacket].DataStart=0;
	P.P[P.NumPacket].DataLen=0;
	S_GetTimerRunningCount(P.T,P.P[P.NumPacket].StartTransmit);
	P.NumPacket++;
	return P.P[P.NumPacket-1].State;
}

int SPG_CONV pkBufferGetLastPacketState(PACKET_BUFFER& P)
{
	if(P.NumPacket)
	{
		return P.P[P.NumPacket-1].State;
	}
	else
		return PACKET_FULL;//s'il n'y a pas de message en attente, on indique que le précédent message est déjà complet, qu'il faut en créer un nouveau
}

PACKET_BUFFER_ELT* SPG_CONV pkBufferGetLastPacket(PACKET_BUFFER& P) //can not be null
{
	CHECK(P.NumPacket==0,"pkBufferGetLastPacket",pkBufferFillLastPacket(P,0,0,PACKET_CMD_CREATE));
	return P.P+P.NumPacket-1;//can not be null
}

int SPG_CONV pkBufferGetFirstPacketState(PACKET_BUFFER& P)
{
	if(P.NumPacket)
	{
		return P.P[0].State;
	}
	else
		return PACKET_EMPTY;
}

int SPG_CONV pkBufferHasFirstPacket(PACKET_BUFFER& P)
{
	DbgCHECK((P.P[0].State&PACKET_FULL)&&(P.P[0].Len==0),"pkBufferHasFirstPacket");
	return (
		    (P.NumPacket>1)||
		    ((P.NumPacket==1)&&(P.P[0].State&PACKET_FULL))
		   );
}


int SPG_CONV pkBufferGetFirstPacketRRaw(PACKET_BUFFER& P, void* &PacketData, int &PacketDataSize)
{
	int State=PACKET_EMPTY;
	PacketData=0;
	PacketDataSize=0;
	if(P.NumPacket)
	{
		State=P.P[0].State;
		PacketData=(BYTE*)P.B+P.P[0].Start;
		PacketDataSize=P.P[0].Len;
	}
	return State;
}

int SPG_CONV pkBufferGetFirstPacketRData(PACKET_BUFFER& P, void* &PacketData, int &PacketDataSize)
{
	int State=PACKET_EMPTY;
	PacketData=0;
	PacketDataSize=0;
	if(P.NumPacket)
	{
		State=P.P[0].State;
		PacketData=(BYTE*)P.B+P.P[0].DataStart;
		PacketDataSize=P.P[0].DataLen;
	}
	return State;
}

int SPG_CONV pkBufferGetFirstPacketWData(PACKET_BUFFER& P, void* &PacketHeaderAndData, int &PacketHeaderAndDataSize)
{
	int State=PACKET_EMPTY;
	PacketHeaderAndData=0;
	PacketHeaderAndDataSize=0;
	if(P.NumPacket)
	{
		State=P.P[0].State;
		PacketHeaderAndData=(BYTE*)P.B+P.P[0].Start;
		PacketHeaderAndDataSize=P.P[0].Len;
	}
	return State;
}

void SPG_CONV pkBufferGetFirstPacket(PACKET_BUFFER& P, PACKET_BUFFER_ELT& PELT)
{
	SPG_ZeroStruct(PELT);
	CHECK(P.NumPacket==0,"pkBufferGetFirstPacket",return);
	PELT=P.P[0];
	return;
}

int SPG_CONV pkBufferRemoveFirstPacket(PACKET_BUFFER& P)
{
	if(P.NumPacket==0)
	{
	}
	else if(P.NumPacket==1)
	{
		P.NumPacket=0;
	}
	else// if(P.NumPacket>1)
	{
		int OrgMove=P.P[1].Start;
		int LenMove=P.P[P.NumPacket-1].Start+P.P[P.NumPacket-1].Len-OrgMove;
		//DbgCHECK(LenMove<=0,"pkBufferRemovePacket");//normal si le packet 0 est discard et le 1 ne contient que STX ou une suite non interpretable
		if(LenMove>0) memmove(P.B,(BYTE*)P.B+OrgMove,LenMove);
		for(int i=1;i<P.NumPacket;i++)
		{
			P.P[i-1]=P.P[i];
			P.P[i-1].Start-=OrgMove;
		}
		P.NumPacket--;
	}
	return P.NumPacket;
}

#endif
