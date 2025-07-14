
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#include "..\SPG_Includes.h"
#include "..\SPG_SysInc.h"

#include "SCM_ConnexionDbg.h"

#include <string.h>
#include <stdio.h>

#define ExpectedTypeUID 15 //"PACKET", SCM_Connexion_Packet

#if(ExpectedTypeUID==15)
#include "SPG_PacketBuffer.h"
#include "SCM_Connexion_Packet_Internal.h"
#endif

#include "SCM_ExtensWriteThrough.h"

// ###########################################################################

//Etat de la communication (typedef enum SCX_PACKET_ETAT)
void scxChangeState(SCX_PACKET_CONNEXION* CP, SCX_PACKET_ETAT Etat, int Address, int msTimeOut)
{
	CHECK(CP==0,"SCM_Interface_Packet:scxChangeState",return);
	CP->ppHist=CP->pHist;
	CP->pHist=CP->Hist;
	CP->Hist.Etat=Etat;
	CP->Hist.Address=Address;
	CP->Hist.msTimeOut=msTimeOut;
	S_GetTimerRunningTime(CP->Timer,CP->Hist.secTimeOrigin);

	char Msg[256];
	strcpy(Msg,"SCM_Interface_Packet:scxChangeState:");
	scxDbgAppendState(Msg,CP->Hist);
	scxSendDebugConnexion("scxChangeState",Msg,CP->Protocol);
	return;
}

void scxGetState(SCX_PACKET_CONNEXION* CP, SCX_PACKET_HIST& Hist,SCX_PACKET_HIST& pHist,SCX_PACKET_HIST& ppHist)
{
	CHECK(CP==0,"SCM_Interface_Packet:scxGetState",SPG_ZeroStruct(ppHist);SPG_ZeroStruct(pHist);SPG_ZeroStruct(Hist);return);
	Hist=CP->Hist;
	pHist=CP->pHist;
	ppHist=CP->ppHist;
	return;
}

void scxDbgAppendState(char* Msg, SCX_PACKET_HIST Hist)
{
	switch(Hist.Etat)
	{
	case scxpkINVALID:			strcat(Msg,"scxpkINVALID"); break;
	case scxpkOK:				strcat(Msg,"scxpkOK");		break;

	case scxpkWaitingRcvACK:	strcat(Msg,"scxpkWaitingRcvACK"); break;
	case scxpkRcvTimeout:		strcat(Msg,"scxpkRcvTimeout");	break;
	case scxpkRcvACK:			strcat(Msg,"scxpkRcvACK");	break;
	case scxpkRcvACKR:			strcat(Msg,"scxpkRcvACKR"); break;
	case scxpkRcvNACK:			strcat(Msg,"scxpkRcvNACK"); break;
	case scxpkRcvNACKR:			strcat(Msg,"scxpkRcvNACK"); break;
	case scxpkRcvSTX:			strcat(Msg,"scxpkRcvSTX");	break;

	case scxpkWaitingSndACK:	strcat(Msg,"scxpkWaitingSndACK"); break;
	case scxpkSndACK:			strcat(Msg,"scxpkSndACK");	break;
	case scxpkSndNACK:			strcat(Msg,"scxpkSndNACK"); break;
	case scxpkSndTimeout:		strcat(Msg,"scxpkSndTimeout"); break;

	case scxpkAbort:			strcat(Msg,"scxpkAbort"); break;
	default:					strcat(Msg,"unknown");	break;
	}
	if(Hist.Address) sprintf(Msg+strlen(Msg),"(%i)",Hist.Address);
	return;
}

void scxDbgGetState(SCX_PACKET_CONNEXION* CP, char* Msg, int MsgLen)
{
	strcat(Msg,"..->");
	scxDbgAppendState(Msg,CP->ppHist);
	strcat(Msg,"->");
	scxDbgAppendState(Msg,CP->pHist);
	strcat(Msg,"->");
	scxDbgAppendState(Msg,CP->Hist);
	DbgCHECK(strlen(Msg)>MsgLen,"scxDbgGetState");
	return;
}

// ###########################################################################

SCX_PACKET_CONNEXION* SPG_CONV scxPacketOpen(SCX_CONNEXION* C)
{
	CHECK(C==0,"SCM_Interface_Packet:scxOpen",return 0);
	DbgCHECK(scxIsNotTypeUID(C,ExpectedTypeUID),"SCM_Interface_Packet:scxOpen");//,return 0);
	SCX_PACKET_CONNEXION* CP=scxTypeAlloc(SCX_PACKET_CONNEXION,"SCM_Interface_Packet:scxOpen");

	CP->Protocol=C;
	/*
	if(scxIsTypeUID(C,ExpectedTypeUID))
	{
		CP->PhysicalR=C->State->R.C;
		CP->PhysicalW=C->State->W.C;
	}
	*/

	S_InitTimer(CP->Timer,"SCX_PACKET_CONNEXION:scxOpen");
	S_StartTimer(CP->Timer);

	CP->Hist.Etat=scxpkOK;
	return CP;
}

int SPG_CONV scxClose(SCX_PACKET_CONNEXION* &CP)
{
	CHECK(CP==0,"SCM_Interface_Packet:scxClose",return scxINVALID);
	CHECK(scxpkEtat(CP)==scxpkWaitingSndACK,"SCM_Interface_Packet:scxClose:Auto send NACK",scxWriteNACK(CP));
	if(scxpkEtat(CP)==scxpkWaitingRcvACK) {scxReadACK(CP);}
	CHECK((scxpkEtat(CP)==scxpkWaitingRcvACK),"SCM_Interface_Packet:scxClose:ACK/NACK not received",return 0);
	DbgCHECK(scxpkEtat(CP)!=scxpkOK,"SCM_Interface_Packet:scxClose");

	scxClose(CP->Protocol);

	S_StopTimer(CP->Timer);
	S_CloseTimer(CP->Timer);

	SPG_MemFree(CP);
	return scxOK;
}


int SPG_CONV scxWrite(void* Data, int DataLen, SCX_PACKET_CONNEXION* CP, int msACKTimeOut)
{
	CHECK(CP==0,"SCM_Interface_Packet:scxWrite",return 0);
	CHECK((scxpkEtat(CP)==scxpkWaitingSndACK),"SCM_Interface_Packet:scxWrite:Auto send NACK",scxWriteNACK(CP));
	if(scxpkEtat(CP)==scxpkWaitingRcvACK) {scxReadACK(CP);}
	CHECK((scxpkEtat(CP)==scxpkWaitingRcvACK),"SCM_Interface_Packet:scxWrite:ACK/NACK not received",return 0);
	CHECK(scxpkEtat(CP)!=scxpkOK,"SCM_Interface_Packet:scxWrite",return 0);
	if(DataLen)
	{
		CHECK(scxWrite(Data, DataLen, CP->Protocol)!=DataLen,"SCM_Interface_Packet:scxWrite:scxWrite failed",scxChangeState(CP,scxpkINVALID,scxPacketAddress(Data),0);scxChangeState(CP,scxpkOK);return 0); //si msTimeOut n'est pas à zero (fonctionnement 'standard') on s'attend à recevoir ACK ou NACK en réponse à ce message
		scxChangeState(CP,msACKTimeOut?scxpkWaitingRcvACK:scxpkOK,scxPacketAddress(Data),msACKTimeOut);
	}

	return DataLen;
}


int SPG_CONV scxRead(void* Data, int& DataLen, SCX_PACKET_CONNEXION* CP, int msACKTimeOut)
{
	CHECK(CP==0,"SCM_Interface_Packet:scxRead",return DataLen=0);
	CHECK(scxpkEtat(CP)==scxpkWaitingSndACK,"SCM_Interface_Packet:scxRead:Auto send NACK",scxWriteNACK(CP));
	if(scxpkEtat(CP)==scxpkWaitingRcvACK) {scxReadACK(CP);}
	CHECK((scxpkEtat(CP)==scxpkWaitingRcvACK),"SCM_Interface_Packet:scxRead:ACK/NACK not received",return 0);
	CHECK(scxpkEtat(CP)!=scxpkOK,"SCM_Interface_Packet:scxRead",return DataLen=0);
	CHECK((CP->Protocol->Address->R.ChecksumOkAutoReply!=0)&&(msACKTimeOut!=0),"scxRead: msTimeOut!=0 but ACK already sent",msACKTimeOut=0);//si on utilise une connexion PACKET avec CheckSumOkReply!=0 une réponse est déjà parvenue

	if(scxRead(Data, DataLen, CP->Protocol)) 
	{
		scxChangeState(CP,msACKTimeOut?scxpkWaitingSndACK:scxpkOK,CP->Hist.Address,msACKTimeOut); //si msTimeOut n'est pas à zero (fonctionnement 'standard') on devra répondre ACK ou NACK à ce message dans le délai imparti
		return DataLen;
	}
	return DataLen=0;
}

//-1:un packet valide est en attente, 0:pas de packet valide, mais un packet en cours de décodage peut exister
int SPG_CONV scxTestRead(SCX_PACKET_CONNEXION* CP)
{
	CHECK(CP==0,"SCM_Interface_Packet:scxTestRead",return 0);
	return scxTestPACKETRead(CP->Protocol);
}

//vide tous les packets en attente y compris ceux en cours de décodage
void SPG_CONV scxFlushRead(SCX_PACKET_CONNEXION* CP)
{
	CHECK(CP==0,"SCM_Interface_Packet:scxFlushRead",return);
	scxFlushPACKETRead(CP->Protocol);
	return;
}


int SPG_CONV scxWriteACK(SCX_PACKET_CONNEXION* CP)
{
	CHECK(CP==0,"SCM_Interface_Packet:scxWriteACK",return scxINVALID);
	CHECK(scxpkEtat(CP)!=scxpkWaitingSndACK,"SCM_Interface_Packet:scxWriteACK:Internal error - No pending ACK",return scxINVALID);
	if(CP->Protocol->Address->R.ChecksumOkAutoReply!=0) return scxINVALID;//erreur silencieuse, ACK est déjà renvoyé dans la couche inférieure à réception d'un packet valide
	//if(CP->Protocol->Address->R.ChecksumErrorReply!=0) {};//problème d'interprétation si on utilise ChecksumErrorReply=2:NACK (NACK signifie alors erreur de checksum)

	//if(CP->PhysicalW==0) {scxChangeState(CP,scxpkOK,CP->Hist.Address);return scxINVALID;}//CHECK(scxIsTypeUID(C->Protocol,15)==0,"SCM_Interface_Packet:scxWriteACK: La connexion spécifiée n'est pas le protocole PACKET attendu",return);

	double secT;S_GetTimerRunningTime(CP->Timer,secT);
	if((secT-CP->Hist.secTimeOrigin)*1000>CP->Hist.msTimeOut)
	{
		DbgCHECK(1,"SCM_Interface_Packet:scxWriteACK:hors delai");
		scxChangeState(CP,scxpkSndTimeout,CP->Hist.Address);
		scxChangeState(CP,scxpkOK,CP->Hist.Address);
		return scxINVALID;
	}

	scxChangeState(CP,scxpkSndACK,CP->Hist.Address);
	CHECK(scxWriteThrough(STR_ACK,1,CP->Protocol)!=1,"SCM_Interface_Packet:scxWriteACK:scxWrite failed",return scxINVALID);
	scxChangeState(CP,scxpkOK);
	return scxOK;
}

int SPG_CONV scxWriteNACK(SCX_PACKET_CONNEXION* CP)
{
	CHECK(CP==0,"SCM_Interface_Packet:scxWriteNACK",return scxINVALID);
	CHECK(scxpkEtat(CP)!=scxpkWaitingSndACK,"SCM_Interface_Packet:scxWriteNACK:No pending ACK",return scxINVALID);
	if(CP->Protocol->Address->R.ChecksumOkAutoReply!=0) return scxINVALID;//erreur silencieuse, ACK est déjà renvoyé dans la couche inférieure à réception d'un packet valide
	//if(CP->Protocol->Address->R.ChecksumErrorReply!=0) {};//problème d'interprétation si on utilise ChecksumErrorReply=2:NACK (NACK signifie alors erreur de checksum)

	//if(CP->PhysicalW==0) {scxChangeState(CP,scxpkOK);return scxINVALID;}//CHECK(scxIsTypeUID(C->Protocol,15)==0,"SCM_Interface_Packet:scxWriteNACK: La connexion spécifiée n'est pas le protocole PACKET attendu",return);
	
	double secT;S_GetTimerRunningTime(CP->Timer,secT);
	if((secT-CP->Hist.secTimeOrigin)*1000>CP->Hist.msTimeOut)
	{
		DbgCHECK(1,"SCM_Interface_Packet:scxWriteNACK:hors delai");
		scxChangeState(CP,scxpkSndTimeout,CP->Hist.Address);
		scxChangeState(CP,scxpkOK,CP->Hist.Address);
		return scxINVALID;
	}
	scxChangeState(CP,scxpkSndNACK,CP->Hist.Address);
	CHECK(scxWriteThrough(STR_NAK,1,CP->Protocol)!=1,"SCM_Interface_Packet:scxWriteNACK:scxWrite failed",return scxINVALID);
	scxChangeState(CP,scxpkOK);
	return scxOK;
}

//fonction non blocante indiquant si le ACK attendu est arrivé ou si le timeout a expiré
SCX_ACK_ETAT SPG_CONV scxTestACK(SCX_PACKET_CONNEXION* CP)
{
	CHECK(CP==0,"SCM_Interface_Packet:scxTestACK",return scx_ACK);
	CHECK(scxpkEtat(CP)!=scxpkWaitingRcvACK,"SCM_Interface_Packet:scxTestACK:Internal error - Not expecting ACK",return scx_ACK);

	//if(CP->PhysicalR==0) {scxChangeState(CP,scxpkOK,CP->Hist.Address);return scx_ACK;}

	char Buffer;
	int Len;
	while(Len=1,scxReadThrough(&Buffer,Len,CP->Protocol))
	{
		if(Buffer==CHAR_ACK)
		{
			//DbgCHECK(1,"SCM_Interface_Packet:scxTestACK:Recu ACK");
			//scxSendDebugConnexion("scxTestACK","SCM_Interface_Packet:scxTestACK:Recu ACK",CP->Protocol);
			scxChangeState(CP,scxpkRcvACK,CP->Hist.Address);
			scxChangeState(CP,scxpkOK);
			return scx_ACK;
		}
		else if(Buffer==CHAR_NAK) 
		{
			DbgCHECK(1,"SCM_Interface_Packet:scxTestACK:Recu NACK");
			scxChangeState(CP,scxpkRcvNACK,CP->Hist.Address);
			scxChangeState(CP,scxpkOK);
			return scx_NACK;
		}
		else if(Buffer==CHAR_ACKR)
		{
			//DbgCHECK(1,"SCM_Interface_Packet:scxTestACK:Recu ACKR");
			scxChangeState(CP,scxpkRcvACKR,CP->Hist.Address);
			scxChangeState(CP,scxpkOK);
			return scx_ACKR;
		}
		else if(Buffer==CHAR_NACKR)
		{
			DbgCHECK(1,"SCM_Interface_Packet:scxTestACK:Recu NACKR");
			scxChangeState(CP,scxpkRcvNACKR,CP->Hist.Address);
			scxChangeState(CP,scxpkOK);
			return scx_NACKR;
		}
		else if(Buffer==CHAR_STX)//on attendait ACK et on recoit le debut d'un message
		{
			DbgCHECK(1,"SCM_Interface_Packet:scxTestACK:Waiting ACK/NAK:Discarded STX");
			if(CP->Protocol->State->CTrash) scxWrite(&Buffer,Len,CP->Protocol->State->CTrash);

			/*
			int DataLen=1;//remet STX 'dans le paquet'
			scxInput(STR_STX,DataLen,CP->Protocol);

			scxChangeState(CP,scxpkRcvSTX,CP->Hist.Address);
			scxChangeState(CP,scxpkOK);
			return scx_STX;
			*/
		}
		else if(Buffer==CHAR_ETX)//on attendait ACK et on recoit le debut d'un message
		{
			DbgCHECK(1,"SCM_Interface_Packet:scxTestACK:Waiting ACK/NAK:Discarded ETX");
			if(CP->Protocol->State->CTrash) scxWrite(&Buffer,Len,CP->Protocol->State->CTrash);

			/*
			int DataLen=1;//remet ETX 'dans le paquet'
			scxInput(STR_ETX,DataLen,CP->Protocol);

			scxChangeState(CP,scxpkRcvSTX,CP->Hist.Address);
			scxChangeState(CP,scxpkOK);
			return scx_STX;
			*/
		}
		else
		{
#ifdef DebugList
			char c=Buffer;
			if(!(
				((Buffer>='a')&&(Buffer<='z'))||((Buffer>='A')&&(Buffer<='Z'))||((Buffer>='0')&&(Buffer<='9'))
				))
			{
				c='.';
			}
			char Msg[128];
			sprintf(Msg,"SCM_Interface_Packet:scxTestACK:Waiting ACK/NAK:Discarded 0x%X %c",(int)(BYTE)Buffer,c);
			SPG_List(Msg);
#endif
			if(CP->Protocol->State->CTrash) scxWrite(&Buffer,Len,CP->Protocol->State->CTrash);
		}
	}
	
	double secT;S_GetTimerRunningTime(CP->Timer,secT);
	if((secT-CP->Hist.secTimeOrigin)*1000>CP->Hist.msTimeOut)
	{
		DbgCHECKV(1,"SCM_Interface_Packet:scxTestACK:Timeout (ms)",CP->Hist.msTimeOut);
		scxChangeState(CP,scxpkRcvTimeout,CP->Hist.Address);
		scxChangeState(CP,scxpkOK);
		return scx_Timeout;
	}

	return scx_Waiting;
}

//fonction blocante jusqu'à réception de ACK ou Timeout
SCX_ACK_ETAT SPG_CONV scxReadACK(SCX_PACKET_CONNEXION* CP)
{
	CHECK(CP==0,"SCM_Interface_Packet:scxReadACK",return scx_ACK);
	CHECK(scxpkEtat(CP)!=scxpkWaitingRcvACK,"SCM_Interface_Packet:scxReadACK:Internal error - Not expecting ACK",return scx_ACK);

	//if(CP->PhysicalR==0) {scxChangeState(CP,scxpkOK);return scx_ACK;}

	SCX_ACK_ETAT ACK_Etat = scx_Waiting; //initialisation inutile
	while(
		( (ACK_Etat=scxTestACK(CP)) == scx_Waiting)  //affectation volontaire
		&& (SPG_GLOBAL_ETAT(MUSTEXIT)==0)
		)
	{
		DoEvents(SPG_DOEV_ALL);
		SPG_Sleep(10);
	}
	return ACK_Etat;
}

// ###########################################################################

//ecrit des données directement sur la RS sans passer par le protocole packet
//utilisé par exemple pour envoyer ACK qui n'est pas une trame STX/ETX/Checksum valide
int SPG_CONV scxWriteThrough(void* Data, int DataLen, SCX_PACKET_CONNEXION* CP)
{
	CHECK(CP==0,"SCM_Interface_Packet:scxWritePhysical",return 0);
	CHECK(scxpkEtat(CP)!=scxpkOK,"SCM_Interface_Packet:scxWriteThrough allowed only during idle communication",return 0);
	return scxWriteThrough(Data, DataLen, CP->Protocol);
}

//lit des données directement depuis la RS sans passer par le protocole packet
//utilisé par exemple pour lire les codes ACK NACK qui ne sont pas des trames STX/ETX/Checksum valides
int SPG_CONV scxReadThrough(void* Data, int& DataLen, SCX_PACKET_CONNEXION* CP)
{
	CHECK(CP==0,"SCM_Interface_Packet:scxReadPhysical",return DataLen=0);
	CHECK(scxpkEtat(CP)==scxpkOK,"SCM_Interface_Packet:scxReadThrough allowed only during idle communication",return DataLen=0);
	return scxReadThrough(Data, DataLen, CP->Protocol);
}

//rentre des données dans le protocole packet comme si elles venaient de la connexion physique, utile pour le débogage
int SPG_CONV scxInput(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxInput");
	SCX_STATE& State=*C->State;
	CHECK(State.R.C==0,"scxInput",return DataLen=0);
	//PACKET_CONNEXION_ADDRESS& AP=State.Address.R;
	PACKET_CONNEXION_STATE& SP=State.R;
	PACKET_BUFFER& PB=SP.PB;

	//-----------CREE UN NOUVEAU PACKET SI NECESSAIRE----------

	if(//PacketRespect||  // (ajouter un test de time out entre les anciennes données et les nouvelles, basé par exemple sur 16 octets de transmission RS232 ou sur 250ms (un ping Ethernet))
		pkBufferGetLastPacketState(PB)&PACKET_FULL)//il n'y a pas d'ancien message ou celui qu'il y a est complet
	{//démarre un nouveau message
		pkBufferFillLastPacket(PB,0,0,PACKET_CMD_CREATE);
	}//else concatène au message précédent

	//-----------LIT LES DONNEES----------

#define pkStart			(LastPacket->Start)
#define pkLen			(LastPacket->Len)

	PACKET_BUFFER_ELT* LastPacket=pkBufferGetLastPacket(PB);//can not be null
	//concatène les données recues
	int ReadLen=V_Min(PB.Size-(pkStart+pkLen),DataLen);
	memcpy((BYTE*)PB.B+pkStart+pkLen,Data,ReadLen);
	return ReadLen;
}

// ###########################################################################

#endif
