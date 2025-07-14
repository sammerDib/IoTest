

#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#include "..\SPG_Includes.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#include "SPG_PacketBuffer.h"

#ifdef DebugNetwork
void SPG_CONV scxSendDebugData(char* FctName, char* Message, void* Data, int DataLen, SCX_CONNEXION* C);
#else
#define scxSendDebugData(FctName,Message,Data,DataLen,C)
#endif

// ###################################################

#include "SCM_Connexion_Packet_Internal.h"

#include "SCM_ExtensWriteThrough.h"

#include "SCM_ExtensProtocol.h"

// ###################################################

#define MAXPACKETSIZE 8192

// ###################################################

//efine NoChecksumTest //utilisé pour le laas - obsolete - utiliser EnableChecksum=1

static SCX_EXTWRITETHROUGH(scxPACKETWriteThrough);
static SCX_EXTREADTHROUGH(scxPACKETReadThrough);

static SCX_CONNEXION* SPG_CONV scxPACKETOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	CHECK(scxOpenProtocol(State,scxOpenFlag)==0,"scxPACKETOpen",scxCloseProtocol(State);scxFree(C->State);scxFree(C);return 0);

	CHECK(pkBufferCreate(State.R.PB,State.Address.R.BufferSize)==0,"scxPACKETOpen",scxFree(C->State);scxFree(C);return 0);
	CHECK(pkBufferCreate(State.W.PB,State.Address.W.BufferSize)==0,"scxPACKETOpen",scxFree(C->State);scxFree(C);return 0);
	if(State.Address.R.Transcode) 
	{
		CHECK(State.Address.R.Transcode!=2,"scxPACKETOpen: Configuration invalide",State.Address.R.Transcode=2);
		State.R.TranscodeBuffer=SPG_TypeAlloc(State.Address.R.BufferSize*2,BYTE,"scxPACKETOpen:State.R.TranscodeBuffer");
	}
	if(State.Address.W.Transcode) 
	{
		CHECK(State.Address.W.Transcode!=1,"scxPACKETOpen: Configuration invalide",State.Address.R.Transcode=1);
		State.W.TranscodeBuffer=SPG_TypeAlloc(State.Address.W.BufferSize*2,BYTE,"scxPACKETOpen:SP.TranscodeBuffer");
	}


	C->UserFctPtr[sci_EXT_WRITETHROUGH]=(SCX_USEREXTENSION)scxPACKETWriteThrough;
	C->UserFctPtr[sci_EXT_READTHROUGH]=(SCX_USEREXTENSION)scxPACKETReadThrough;
	scxProtocolInheritUserExtension(C,State.W.C,State.R.C);
	C->Etat=scxOK;
	return C;
}

// ###################################################

static int SPG_CONV scxPACKETClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;

	scxCloseProtocol(State);

	//desallocation STATE
	pkBufferDestroy(State.R.PB);
	pkBufferDestroy(State.W.PB);
	if(State.Address.R.Transcode) SPG_MemFree(State.R.TranscodeBuffer);
	if(State.Address.W.Transcode) SPG_MemFree(State.W.TranscodeBuffer);

	scxFree(C->State);scxFree(C);
	return scxOK;
}

// ###################################################

#define scxIsValidRIA(c) (V_InclusiveBound(c,'0','9')||V_InclusiveBound(c,'A','F'))

static void scxPACKETTranscode(int TranscodeMode, PACKET_CONNEXION_STATE& State, void* Data, int DataLen)
{
	if(TranscodeMode==0)//0:None 1:BYTEtoRIA 2:RIAtoBYTE
	{
		State.TranscodeBuffer=(BYTE*)Data;
		State.TranscodeLen=DataLen;
	}
	else if(TranscodeMode==1)//0:None 1:BYTEtoRIA 2:RIAtoBYTE
	{
		State.TranscodeLen=2*DataLen;
		for(int i=0;i<DataLen;i++)
		{
			BYTE bLOW=((BYTE*)Data)[i];
			BYTE bHI=bLOW>>4;
			State.TranscodeBuffer[2*i]=sci16ToHEX(bHI);//'a'+(((BYTE*)Data)[i]&16);
			State.TranscodeBuffer[2*i+1]=sci16ToHEX(bLOW);
		}
	}
	else if(TranscodeMode==2)//0:None 1:BYTEtoRIA 2:RIAtoBYTE
	{
		State.TranscodeLen=0;
		for(int i=0;i<DataLen;i++)
		{
			BYTE B0,B1;
			B0=((BYTE*)Data)[i];
			if(scxIsValidRIA(B0) &&(i<DataLen-1))
			{//transcode deux octets
				B1=((BYTE*)Data)[i+1];
				if(scxIsValidRIA(B1))
				{
					State.TranscodeBuffer[State.TranscodeLen]=(sciHEXTo16(B0)<<4)|(sciHEXTo16(B1));
					i++;//on a consommé un second octet dans ce tour de boucle
					State.TranscodeLen++;//on a ecrit 1 octet
				}
			}
			else
			{//pas de transcodage
				DbgCHECKV(1,"Caractere non transcodé BYTE=",B0);
				State.TranscodeBuffer[State.TranscodeLen]=B0;
				State.TranscodeLen++;//on a ecrit 1 octet
			}
		}
	}
	return;
}


// ###################################################

static int SPG_CONV scxPACKETWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;
	CHECK(State.W.C==0,"scxPACKETWrite",return 0);
	PACKETW_CONNEXION_ADDRESS& AP=State.Address.W;
	PACKET_CONNEXION_STATE& SP=State.W;
	PACKET_BUFFER& PB=SP.PB;


	DbgCHECK(PB.NumPacket!=0,"scxPACKETWrite: Erreur interne à l'entrée de scxPACKETWrite");

	pkBufferFillLastPacket(PB,0,0,PACKET_CMD_CREATE);

	if(AP.EnableSTXETX) //Offset 0, BYTE STX
	{
		pkBufferFillLastPacket(PB,STR_STX,1,0); 
	}

	if(AP.EnablePacketSize) //Offset 1, WORD PacketSize
	{
		WORD PacketSize=DataLen;//taille des données hors STX/Size/SeqID/.../Checksum/ETX
		pkBufferFillLastPacket(PB,&PacketSize,sizeof(PacketSize),0);
	}
/*
	if(State.Address.EnableWPacketSeqID) //Offset 3, BYTE SeqID
	{
		pkBufferFillLastPacket(SP.PB,&SP.PB.SeqID,1,0);
	}
*/
	scxPACKETTranscode(AP.Transcode,SP,Data,DataLen);//0:None 1:BYTEtoRIA 2:RIAtoBYTE
	pkBufferFillLastPacket(PB, SP.TranscodeBuffer, SP.TranscodeLen, PACKET_CMD_DATA);//Offset 4 Data

	if(AP.EnableChecksum)
	{
		BYTE Checksum=0x00;
		if(AP.EnableChecksum==1)//0:No checksum 1:Zero 2:SumBYTE+0x80 3:RIA
		{
			//pkBufferFillLastPacket(PB,&Checksum,sizeof(Checksum),0);
		}
		else if(AP.EnableChecksum==2)//0:No checksum 1:Zero 2:SumBYTE+0x80 3:RIA
		{ //Offset 4 + DataLen, BYTE Checksum
			Checksum=0x80;
			for(int i=0;i<DataLen;i++)
			{
				Checksum+=((BYTE*)Data)[i];
			}
			//pkBufferFillLastPacket(PB,&Checksum,sizeof(Checksum),0);
		}
		else if(AP.EnableChecksum==3) //0:No checksum 1:Zero 2:SumBYTE+0x80 3:RIA
		{ //Offset 4 + DataLen, WORD Checksum
			for(int i=0;i<DataLen;i++)
			{
				Checksum+=((BYTE*)Data)[i];
			}
			Checksum=(BYTE)(-Checksum);
		}
		scxPACKETTranscode(AP.Transcode, SP, &Checksum,1);
		pkBufferFillLastPacket(PB, SP.TranscodeBuffer, SP.TranscodeLen, 0);
	}

	if(AP.EnableSTXETX) //Offset 4 + DataLen + Checksum 0,1 ou 2, BYTE ETX
	{
		pkBufferFillLastPacket(PB,STR_ETX,1,0);
	}

	pkBufferFillLastPacket(PB,0,0,PACKET_CMD_CLOSE);

	DbgCHECK(PB.NumPacket!=1,"scxPACKETWrite: Erreur interne");

	void* PacketAndHeader;
	int PacketAndHeaderLen;

	while(pkBufferHasFirstPacket(PB))
	{//si un message attend pour être envoyé
		if(pkBufferGetFirstPacketWData(PB,PacketAndHeader,PacketAndHeaderLen)==(PACKET_FULL|PACKET_HASDATA))
		{//si le message est valide
			int s=0;
			for(;s<AP.MaxRetry;s++)
			{
				if(scxWrite(PacketAndHeader,PacketAndHeaderLen,SP.C)==PacketAndHeaderLen) break;
				DbgCHECK(1,"scxPACKETWrite: SEND FAILED");
			}
			CHECK(s==AP.MaxRetry,"scxWrite(PacketAndHeader,PacketAndHeaderLen,SP.C) failed MaxRetry times",DataLen=0);
		}
		else if(State.CTrash)
		{
			scxWrite(PacketAndHeader,PacketAndHeaderLen,State.CTrash);
		}
		else
		{
			scxSendDebugData("scxPACKETWrite","Failed to send",PacketAndHeader,PacketAndHeaderLen,C);
		}
		pkBufferRemoveFirstPacket(PB);
	}

	DbgCHECK(PB.NumPacket!=0,"scxPACKETWrite: Erreur interne à la sortie de scxPACKETWrite");
	return DataLen;
}

// ###################################################

//#define PELT (*pkBufferGetLastPacket(PB)) //can not be null
//PELT = PB.P[P.NumPacket-1]

static int SPG_CONV scxPACKETRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;
	CHECK(State.R.C==0,"scxPACKETRead",return DataLen=0);
	PACKETR_CONNEXION_ADDRESS& AP=State.Address.R;
	PACKET_CONNEXION_STATE& SP=State.R;
	PACKET_BUFFER& PB=SP.PB;

	int PacketRespect=SP.C->CI->Type&sciPACKETRESPECT;//si le protocole respecte le concept de packet

	int HeaderSize=(AP.EnableSTXETX?1:0)+ //STX
				   (AP.EnablePacketSize?2:0); //WORD //(AP.EnablePacketSeqID?1:0); //BYTE

	int PacketSizeOffset=(AP.EnableSTXETX?1:0);

	int ChecksumLen=((AP.EnableChecksum==1)?1:0)+ //BYTE
					((AP.EnableChecksum==2)?1:0)+ //BYTE
					((AP.EnableChecksum==3)?2:0); //WORD

	int TrailerSize=ChecksumLen+(AP.EnableSTXETX?1:0); //ETX

	int HeaderTrailerSize=HeaderSize+TrailerSize;//longueur totale ajoutée aux données


	//-----------CREE UN NOUVEAU PACKET SI NECESSAIRE----------

	if(//PacketRespect||  // (ajouter un test de time out entre les anciennes données et les nouvelles, basé par exemple sur 16 octets de transmission RS232 ou sur 250ms (un ping Ethernet))
		pkBufferGetLastPacketState(PB)&PACKET_FULL)//il n'y a pas d'ancien message ou celui qu'il y a est complet
	{//démarre un nouveau message
		pkBufferFillLastPacket(PB,0,0,PACKET_CMD_CREATE);
	}//else concatène au message précédent


	//-----------LIT LES DONNEES----------

#define pkStart			(LastPacket->Start)
#define pkLen			(LastPacket->Len)
#define pkState			(LastPacket->State)
#define pkDataStart		(LastPacket->DataStart)
#define pkDataLen		(LastPacket->DataLen)
#define pkData(Offset)  (((BYTE*)PB.B)+pkStart+Offset)
#define pkDataB(Offset) (((BYTE*)PB.B)[pkStart+Offset])
#define pkDataW(Offset) (*(WORD*)&(pkDataB(Offset)))

	PACKET_BUFFER_ELT* LastPacket=pkBufferGetLastPacket(PB);//can not be null
	//concatène les données recues
	int ReadLen=PB.Size-(pkStart+pkLen);
	pkLen+=scxRead((BYTE*)PB.B+pkStart+pkLen,ReadLen,SP.C);

	//-----------DECODE----------
	while(
		LastPacket=pkBufferGetLastPacket(PB),//can not be null
		(pkLen>0)&&((pkState&PACKET_FULL)==0)) // s'il est trop court le packet restera partiel jusqu'à réception de la suite des données
	{//debut du décodage d'un packet
		scxSendDebugData("scxPACKETRead","Decodage",pkData(0),pkLen,C);
		if(AP.EnableSTXETX)
		{
			if(pkDataB(0)!=CHAR_STX) //Offset 0, BYTE STX
			{//le premier octet du packet est un octet d'erreur, on met le premier octet dans un packet d'erreur spécifique inséré
				if(PacketRespect)
				{
					scxSendDebugData("scxPACKETRead","STX expected - Discarded (PACKET_INVALIDSTX|PACKET_CMD_CLOSE)",pkData(0),pkLen,C);
					pkBufferFillLastPacket(PB,0,0,PACKET_INVALIDSTX|PACKET_CMD_CLOSE);
				}
				else
				{
					int i;
					for(i=0;i<pkLen;i++)
					{
						if(pkDataB(i)==CHAR_STX) break;
					}
					if(i<pkLen) 
					{
						scxSendDebugData("scxPACKETRead","STX expected - Restart from STX (PACKET_PARTIAL)",pkData(0),pkLen,C);
						pkBufferCutLastPacket(PB,PACKET_INVALIDSTX|PACKET_FULL,i,PACKET_PARTIAL);
					}
					else
					{
						scxSendDebugData("scxPACKETRead","STX expected - Discarded (PACKET_INVALIDSTX|PACKET_CMD_CLOSE)",pkData(0),pkLen,C);
						pkBufferFillLastPacket(PB,0,0,PACKET_INVALIDSTX|PACKET_CMD_CLOSE);
					}
				}
				continue; //puis on recommence le décodage du packet au début avec une trame plus courte (pas de boucle infinie)
			}
		}

		if(AP.EnablePacketSize)//Offset 1, WORD PacketSize
		{//si le protocole gere intrinsèquement le concept de paquet (TCP ou UDP) le champ size est inutile et ne sert que de verification
			if(pkLen<(PacketSizeOffset+2)) break;// s'il est trop court le packet restera partiel jusqu'à réception de la suite des données

			if(
			   PacketRespect&&
			   (pkDataW(PacketSizeOffset)!=(pkLen-HeaderTrailerSize)) //ce test n'est pas réalisable en RS232
			  )
			{//le champ PacketSize n'est pas cohérent avec la taille lue par scxRead, on met le premier octet dans un packet d'erreur spécifique inséré
				//inserterrorpacket; ou trash all si le protocole respecte les paquets
				//pkCutLastPacket(PACKETINVALIDCHECKSUM|PACKET_FULL,PacketSizeOffset+2,PACKET_PARTIAL);//inserterrorpacket; ou trash all si le protocole respecte les paquets
				if(PacketRespect)
				{
					scxSendDebugData("scxPACKETRead","PacketSize Error - Discarded (PACKET_INVALIDSTX|PACKET_CMD_CLOSE)",pkData(0),pkLen,C);
					pkBufferFillLastPacket(PB,0,0,PACKET_INVALIDSIZE|PACKET_CMD_CLOSE);
				}
				else
				{//n'efface que le premier octet et retente un décodage
					scxSendDebugData("scxPACKETRead","PacketSize Error - Restart after removing one byte (PACKET_PARTIAL)",pkData(0),pkLen,C);
					pkBufferCutLastPacket(PB,PACKET_INVALIDSIZE|PACKET_FULL,1,PACKET_PARTIAL);
				}
				continue; //puis on recommence le décodage du packet au début avec une trame plus courte de un octet (STX est peut être dans ce qu'on prend pour le champ PacketSize)
			}

			pkDataStart=pkStart+HeaderSize;
			pkDataLen=pkDataW(PacketSizeOffset);//si le champ ne gere pas le concept de packet on fait confiance au champ PacketSize
			pkState|=PACKET_HASDATA;
			if(pkLen<pkDataLen+HeaderTrailerSize) 
			{//on a bien pu lire l'entete mais la suite des données n'est pas arrivée
				scxSendDebugData("scxPACKETRead","Packet partial - pkLen<pkDataLen+HeaderTrailerSize",pkData(0),pkLen,C);
				break;//le packet restera partiel jusqu'à réception de la suite des données
			} 
		}
		else if(AP.EnableSTXETX)
		{	//CHECKTWO(1,"scxPACKETRead: Missing EnableRPacketSize",SP.C->CI->Name,pkBufferFillLastPacket(PB,0,0,PACKET_INVALIDSIZE|PACKET_CMD_CLOSE);break);
			//le protocole sousjacent ne garantit pas le respect des packets de données (troncature ou merge implicite)
			//et il n'y a pas de champ taille dans le packet
			int PacketLen=pkLen;
			int i=1;
			for(;i<PacketLen;i++)
			{
				if(pkDataB(i)==CHAR_ETX) 
				{
					pkDataStart=HeaderSize;
					pkDataLen=i+1-HeaderTrailerSize;
					pkState|=PACKET_HASDATA;
					break;
				}
				if(AP.Transcode==2)
				{//les caracteres non ASCII ne sont pas valides dans un message à convertir ultérieurement en ASCIItoBYTE
					if(!scxIsValidRIA(pkDataB(i)))
					{
						if(PacketRespect)
						{
							scxSendDebugData("scxPACKETRead","Invalid RIA character after STX - Discard (PACKET_INVALIDETX|PACKET_CMD_CLOSE)",pkData(0),pkLen,C);
							pkBufferFillLastPacket(PB,0,0,PACKET_INVALIDETX|PACKET_CMD_CLOSE);
						}
						else if(pkDataB(i)==CHAR_STX)
						{
							scxSendDebugData("scxPACKETRead","STX character after STX - Restart at STX (PACKET_PARTIAL)",pkData(0),pkLen,C);
							pkBufferCutLastPacket(PB,PACKET_INVALIDETX|PACKET_FULL,i,PACKET_PARTIAL);
						}
						else
						{
							scxSendDebugData("scxPACKETRead","Invalid RIA character after STX - Restart after invalid character (PACKET_PARTIAL)",pkData(0),pkLen,C);
							pkBufferCutLastPacket(PB,PACKET_INVALIDETX|PACKET_FULL,i+1,PACKET_PARTIAL);
						}
						break; //puis on recommence le décodage du packet au début avec une trame plus courte (pas de boucle infinie)
					}
				}
			}
			if((i==PacketLen)&&(pkState&PACKET_HASDATA)==0)
			{
				scxSendDebugData("scxPACKETRead","Packet partial - Waiting for ETX",pkData(0),pkLen,C);
				break;//le packet n'est pas complètement arrivé
			}
			else if((i!=PacketLen)&&(pkState&PACKET_HASDATA)==0)
			{
				continue;
			}
		}
		else
		{//il n'y a pas de champ PacketSize mais le protocole respecte la taille des données sans troncature/concaténation
			DbgCHECK(PacketRespect==0,"scxPACKETRead: Configuration invalide: PacketRespect=0 EnableSTXETX=0 EnablePacketSize=0");
			pkDataStart=pkStart+HeaderSize;
			pkDataLen=pkLen-HeaderTrailerSize;
			pkState|=PACKET_HASDATA;
		}

		//à partir de là on a les données du message sur une longueur suffisante pour contenir data+checksum+ETX
		if(AP.EnableSTXETX)
		{
			if(pkDataB(HeaderTrailerSize+pkDataLen-1)!=CHAR_ETX)//Dernier octet, ETX
			{//il manque ETX à la fin du message
				//inserterrorpacket; ou trash all si le protocole respecte les paquets
				if(PacketRespect)
				{
					scxSendDebugData("scxPACKETRead","ETX not found - Ignore packet (PACKET_INVALIDETX|PACKET_CMD_CLOSE)",pkData(0),pkLen,C);
					pkBufferFillLastPacket(PB,0,0,PACKET_INVALIDETX|PACKET_CMD_CLOSE);
				}
				else
				{//n'efface que le premier octet et retente un décodage
					scxSendDebugData("scxPACKETRead","ETX not found - Restart after removing one byte (PACKET_PARTIAL)",pkData(0),pkLen,C);
					pkBufferCutLastPacket(PB,PACKET_INVALIDETX|PACKET_FULL,1,PACKET_PARTIAL);
				}
				continue; //puis on recommence le décodage du packet au début avec une trame plus courte (pas de boucle infinie)
			}
		}
/*
		if(AP.EnableRPacketSeqID)
		{
			if(PacketData[0]!=*(BYTE*)&PB.SeqID)
			{
				pkBufferFillLastPacket(PB,RawData,RawLen,PACKET_ELT_CREATE|PACKET_ELT_CLOSE|PACKET_INVALIDSeqID);
				goto scxPACKETRead_ReadPacket;
			}
			RawData++;RawLen--;
		}
*/
		scxPACKETTranscode(AP.Transcode,SP,pkData(HeaderSize),pkDataLen+ChecksumLen);//0:None 1:BYTEtoRIA 2:RIAtoBYTE
//*(BYTE*)PB.B *(BYTE*)SP.TranscodeBuffer
		if((AP.ChecksumErrorAutoReply!=1)&&(AP.EnableChecksum!=0))//ChecksumErrorReply: 1=No Check EnableChecksum 0:No checksum 1:Zero 2:SumBYTE+0x80 3:RIA
		{
			BYTE Checksum=0x00;
			if(AP.EnableChecksum==1)//0:No checksum 1:Zero 2:SumBYTE+0x80 3:RIA
			{
			}
			else if(AP.EnableChecksum==2)//0:No checksum 1:Zero 2:SumBYTE+0x80 3:RIA
			{
				Checksum=0x80;
				for(int i=0;i<SP.TranscodeLen-1;i++)
				{
					Checksum+=SP.TranscodeBuffer[i];
				}
			}
			else if(AP.EnableChecksum==3)//0:No checksum 1:Zero 2:SumBYTE+0x80 3:RIA
			{
				for(int i=0;i<SP.TranscodeLen-1;i++)
				{
					Checksum+=SP.TranscodeBuffer[i];
				}
				Checksum=(BYTE)(-Checksum);
			}
			if(SP.TranscodeBuffer[SP.TranscodeLen-1]!=Checksum)
			{//erreur de checksum
				//inserterrorpacket; ou trash all si le protocole respecte les paquets
				scxSendDebugData("scxPACKETRead","Checksum Error",SP.TranscodeBuffer,SP.TranscodeLen,C);
				if(AP.ChecksumErrorAutoReply==2) // 2 =renvoi NACK
				{
					scxSendDebugData("scxPACKETRead","Checksum Error - Auto send NACK",SP.TranscodeBuffer,SP.TranscodeLen,C);
					scxWrite(STR_NACK,1,State.W.C);
				}

				if(PacketRespect)
				{
					scxSendDebugData("scxPACKETRead","Checksum Error - Ignore packet (PACKET_INVALIDCHECKSUM|PACKET_CMD_CLOSE)",SP.TranscodeBuffer,SP.TranscodeLen,C);
					pkBufferFillLastPacket(PB,0,0,PACKET_INVALIDCHECKSUM|PACKET_CMD_CLOSE);
				}
				else
				{//n'efface que le premier octet et retente un décodage
					scxSendDebugData("scxPACKETRead","Checksum Error - Restart after removing one byte (PACKET_PARTIAL)",SP.TranscodeBuffer,SP.TranscodeLen,C);
					pkBufferCutLastPacket(PB,PACKET_INVALIDCHECKSUM|PACKET_FULL,1,PACKET_PARTIAL);
				}
				continue;
			}
		}

		//tout est OK, on coupe le packet apres ETX
		pkBufferCutLastPacket(PB,PACKET_FULL,HeaderTrailerSize+pkDataLen,PACKET_PARTIAL);
		if(AP.ChecksumOkAutoReply==1) 
		{
			scxWrite(STR_ACK,1,State.W.C);
		}
	}//loop pour continuer le decodage du dernier packet du buffer

	int DataMaxLen=DataLen;//sauve pour le cas ou on lit plusieurs packets

	while(pkBufferHasFirstPacket(PB))	//lit les packet décodés
	{//si un message attend pour être envoyé
		void* PBData=0; int PreTranscodeLen=0;
		int r=pkBufferGetFirstPacketRData(PB,PBData,PreTranscodeLen);
		if((r&(~PACKET_PARTIAL))==(PACKET_FULL|PACKET_HASDATA)) 
		{//Data et DataLen contiennent les données recues
			scxPACKETTranscode(AP.Transcode,SP,PBData,PreTranscodeLen);//0:None 1:BYTEtoRIA 2:RIAtoBYTE
			DataLen=SP.TranscodeLen;
			CHECK(DataLen>DataMaxLen,"scxPACKETRead:pkBufferGetFirstPacketRData",DataLen=V_Min(DataLen,DataMaxLen));
			memcpy(Data,SP.TranscodeBuffer,DataLen);
			if(AP.EnableMultipleRead==0) pkBufferRemoveFirstPacket(PB); //attention si la connexion est lue à plusiers occasions (demultiplexage d'addresse) le remove ne sera effectué qu'après acquitement d'un des recepteurs
			//CHECK(PBData!=Data,"scxPACKETRead:pkBufferGetFirstPacketRData",return DataLen=0);
			return DataLen;//retourne le premier message valide
		}
		else 
		{//packet erroné
			pkBufferGetFirstPacketRRaw(PB,PBData,PreTranscodeLen);

			if(State.CTrash) { scxWrite(PBData,1,State.CTrash); }
			else { scxSendDebugData("scxPACKETRead","The following bytes where discarded during abovementioned errors",PBData,PreTranscodeLen,C); }

			pkBufferRemoveFirstPacket(PB);
		}
	}
	return DataLen=0;
}

static SCX_EXTWRITETHROUGH(scxPACKETWriteThrough)
{
	scxCHECK(C, "scxPACKETReadThrough");
	SCX_STATE& State=*C->State;
	CHECK(State.W.C==0,"scxPACKETWriteThrough",return 0);
	PACKET_CONNEXION_STATE& SP=State.W;
	return scxWrite(Data,DataLen,SP.C);
}

static SCX_EXTREADTHROUGH(scxPACKETReadThrough)
{
	scxCHECK(C, "scxPACKETReadThrough");
	SCX_STATE& State=*C->State;
	CHECK(State.R.C==0,"scxPACKETReadThrough",return DataLen=0);
	PACKET_CONNEXION_STATE& SP=State.R;

	int& NumPacket=SP.PB.NumPacket;
	if(NumPacket>0)
	{
		int PrevBytesIn=SP.PB.P[NumPacket-1].Start+SP.PB.P[NumPacket].Len;
		DbgCHECK(PrevBytesIn>0,"scxPACKETReadThrough: Trashing input bytes (collision)");
		NumPacket=0;//efface le contenu du buffer d'entrée
	}
	return scxRead(Data,DataLen,SP.C);
}


int SPG_CONV scxTestPACKETRead(SCX_CONNEXION* C)
{//elimine les paquets invalides,  renvoie -1 si un message est complet prêt à être lu, 0 si tous les buffers sont vide (apres elimination des packets 
	scxCHECK(C, "scxTestRead");
	SCX_STATE& State=*C->State;
	CHECK(State.R.C==0,"scxTestPACKETRead",return 0);
	//PACKETR_CONNEXION_ADDRESS& AP=State.Address.R;
	PACKET_CONNEXION_STATE& SP=State.R;
	PACKET_BUFFER& PB=SP.PB;
	while(pkBufferHasFirstPacket(PB))	//lit les packet décodés
	{//si un message attend pour être envoyé
		void* PBData=0; int PreTranscodeLen=0;
		int r=pkBufferGetFirstPacketRData(PB,PBData,PreTranscodeLen);
		if((r&(~PACKET_PARTIAL))==(PACKET_FULL|PACKET_HASDATA)) 
		{
			return -1;
		}
		else 
		{//packet erroné
			pkBufferGetFirstPacketRRaw(PB,PBData,PreTranscodeLen);

			if(State.CTrash) { scxWrite(PBData,1,State.CTrash); }
			else { scxSendDebugData("scxTestPACKETRead","The following bytes where discarded during abovementioned errors",PBData,PreTranscodeLen,C); }

			pkBufferRemoveFirstPacket(PB);
		}
		//DbgCHECK(PB.P[0].Len==0,"scxPACKETRead"); -> redondant avec CTrash et scxSendDebugData
		pkBufferRemoveFirstPacket(PB);
	}
	return 0;
}

int SPG_CONV scxFlushPACKETRead(SCX_CONNEXION* C)
{//elimine tous les octets en attente, y compris les emssages incomplets
	scxCHECK(C, "scxFlushPACKETRead");
	SCX_STATE& State=*C->State;
	CHECK(State.R.C==0,"scxFlushPACKETRead",return 0);
	//PACKETR_CONNEXION_ADDRESS& AP=State.Address.R;
	PACKET_CONNEXION_STATE& SP=State.R;
	PACKET_BUFFER& PB=SP.PB;
	void* PBData=0; int PreTranscodeLen=0;
	while(pkBufferGetFirstPacketRRaw(PB,PBData,PreTranscodeLen)!=PACKET_EMPTY)	//lit les packet décodés ou partiels
	{
		if(PreTranscodeLen)
		{//car l'état le plus courant du buffer est PACKET_PARTIAL avec longueur nulle
			if(State.CTrash) { scxWrite(PBData,PreTranscodeLen,State.CTrash); }
			else { scxSendDebugData("scxFlushPACKETRead","The following bytes where discarded",PBData,PreTranscodeLen,C); }
		}
		pkBufferRemoveFirstPacket(PB);
	}
	CHECK(PB.NumPacket!=0,"scxFlushPACKETRead",PB.NumPacket=0);
	return 0;
}

// ###################################################

static void SPG_CONV scxPACKETCfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values

		Address->W.Address=0;
		Address->W.CI=0;
		Address->W.BufferSize=2*MAXPACKETSIZE;
		Address->W.EnablePacketSize=1;
		Address->W.EnableChecksum=2;
		//Address->W.EnablePacketSeqID=1;
		Address->W.EnableSTXETX=0;
		Address->W.Transcode=0;
		Address->W.MaxRetry=3;

		Address->R.Address=0;
		Address->R.CI=0;
		Address->R.BufferSize=2*MAXPACKETSIZE;
		Address->R.EnablePacketSize=1;
		Address->R.EnableChecksum=2;
		//Address->R.EnablePacketSeqID=1;
		Address->R.EnableSTXETX=0;
		Address->R.Transcode=0;
		Address->R.ChecksumOkAutoReply=0;
		Address->R.ChecksumErrorAutoReply=0;
		Address->R.EnableMultipleRead=0;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);

	CFG_StringParam(CFG,"Name",Address->H.Name,0,1);

	//Les noms W.Address et W.CI sont imposés par SCM_Interafce.cpp:sciAddressFromCFG
	CFG_IntParam(CFG,"W.Address",	(int*)&Address->W.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"W.CI",	(int*)&Address->W.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"W.BufferSize",	&Address->W.BufferSize,0,1,			CP_INT|CP_HASMIN,1);
	CFG_IntParam(CFG,"W.EnablePacketSize",&Address->W.EnablePacketSize,0,1,	CP_INT|CP_HASMIN|CP_HASMAX,0,1);
	CFG_IntParam(CFG,"W.EnableChecksum",&Address->W.EnableChecksum,"0:No checksum 1:Zero 2:SumBYTE+0x80 3:RIA",1,CP_INT|CP_HASMIN|CP_HASMAX,0,3);
	//CFG_IntParam(CFG,"W.EnablePacketSeqID",&Address->W.EnablePacketSeqID,0,CFG.FileName[0]?1:0,CP_INT|CP_HASMIN|CP_HASMAX,0,1);
	CFG_IntParam(CFG,"W.EnableSTXETX",	&Address->W.EnableSTXETX,0,1,		CP_INT|CP_HASMIN|CP_HASMAX,0,1);
	CFG_IntParam(CFG,"W.Transcode",	&Address->W.Transcode,"0:None 1:BYTEtoRIA 2:RIAtoBYTE",	1,CP_INT|CP_HASMIN|CP_HASMAX,0,2);
	CFG_IntParam(CFG,"W.MaxRetry",	&Address->W.MaxRetry,0,	1,		CP_INT|CP_HASMIN,1);

	//Les noms R.Address et R.CI sont imposés par SCM_Interafce.cpp:sciAddressFromCFG
	CFG_IntParam(CFG,"R.Address",	(int*)&Address->R.Address,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"R.CI",	(int*)&Address->R.CI,0,1,					CP_INT|CP_HASMIN|CP_HASMAX,0,0);
	CFG_IntParam(CFG,"R.BufferSize",	&Address->R.BufferSize,0,1,			CP_INT|CP_HASMIN,1);
	CFG_IntParam(CFG,"R.EnablePacketSize",&Address->R.EnablePacketSize,0,1,	CP_INT|CP_HASMIN|CP_HASMAX,0,1);
	CFG_IntParam(CFG,"R.EnableChecksum",&Address->R.EnableChecksum,"0:No checksum 1:Zero 2:SumBYTE+0x80 3:RIA",1,CP_INT|CP_HASMIN|CP_HASMAX,0,3);
	//CFG_IntParam(CFG,"R.EnablePacketSeqID",&Address->EnablePacketSeqID,0,CFG.FileName[0]?1:0,CP_INT|CP_HASMIN|CP_HASMAX,0,1);
	CFG_IntParam(CFG,"R.EnableSTXETX",	&Address->R.EnableSTXETX,0,	1,		CP_INT|CP_HASMIN|CP_HASMAX,0,1);
	CFG_IntParam(CFG,"R.Transcode",	&Address->R.Transcode,"0:None 1:BYTEtoRIA 2:RIAtoBYTE",	1,CP_INT|CP_HASMIN|CP_HASMAX,0,2);
	CFG_IntParam(CFG,"R.ChecksumOkAutoReply",	&Address->R.ChecksumOkAutoReply,"0:None 1:ACK",	1,CP_INT|CP_HASMIN|CP_HASMAX,0,1);
	CFG_IntParam(CFG,"R.ChecksumErrorAutoReply",	&Address->R.ChecksumErrorAutoReply,"0:DiscardMsg 1:NoCheck 2:NACK",	1,CP_INT|CP_HASMIN|CP_HASMAX,0,2);

	return;
}

// ###################################################

static int SPG_CONV scxPACKETSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV scxPACKETGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV sciPACKETDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciPACKETCreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETRESPECT|sciPROTOCOL);
	strcpy(CI->Name,sci_NAME);

	CI->Description="PACKET RIA - Z3D - MMF";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=MAXPACKETSIZE;//spécifique

	CI->scxOpen=scxPACKETOpen;
	CI->scxClose=scxPACKETClose;
	CI->scxWrite=scxPACKETWrite;
	CI->scxRead=scxPACKETRead;
	CI->scxCfgAddress=scxPACKETCfgAddress;
	CI->scxSetParameter=scxPACKETSetParameter;
	CI->scxGetParameter=scxPACKETGetParameter;
	CI->sciDestroyConnexionInterface=sciPACKETDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#endif
