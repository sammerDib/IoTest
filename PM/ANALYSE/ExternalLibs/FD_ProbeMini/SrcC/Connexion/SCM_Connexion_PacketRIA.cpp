

#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#include "..\SPG_Includes.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#define sci_UID 16
#define sci_NAME "PACKETRIA"   //NE PAS UTILISER - POUR DEVELOPPEMENT SPECIFIQUE FOGALE ULTERIEUR

#include "SPG_PacketBuffer.h"

// ###################################################

typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface

// ###################################################

typedef struct
{
	SCX_ADDRESS* Address;
	SCI_CONNEXIONINTERFACE* CI;
	int BufferSize;
	int EnablePacketSize;
	int EnableChecksum;
	//int EnableRPacketSeqID;
	int EnableSTXETX;
} PACKET_ADDRESS;

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire

	PACKET_ADDRESS R;
	PACKET_ADDRESS W;

	int EnableMultipleRead;
} SCX_ADDRESS;

#define CHAR_STX 0x02
#define CHAR_ETX 0x03

#define STR_STX "\x02"
#define STR_ETX "\x03"

// ###################################################

typedef struct
{
	SCX_CONNEXION* C;
	PACKET_BUFFER PB;
} PACKET_STATE;

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;
	PACKET_STATE R;
	PACKET_STATE W;
	SCX_CONNEXION* CTrash;//packet invalides
} SCX_STATE; //parametres d'une connexion en particulier

// ###################################################

#include "SCM_ExtensProtocol.h"

// ###################################################

static SCX_CONNEXION* SPG_CONV scxPACKETRIAOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	CHECK(pkBufferCreate(State.R.PB,State.Address.R.BufferSize)==0,"scxPACKETRIAOpen",scxFree(C->State);scxFree(C);return 0);
	CHECK(pkBufferCreate(State.W.PB,State.Address.W.BufferSize)==0,"scxPACKETRIAOpen",scxFree(C->State);scxFree(C);return 0);

	scxOpenProtocol(State, scxOpenFlag);

	C->Etat=scxOK;
	return C;
}

// ###################################################

static int SPG_CONV scxPACKETRIAClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;

	scxCloseProtocol(State);

	pkBufferDestroy(State.R.PB);
	pkBufferDestroy(State.W.PB);

	scxFree(C->State);scxFree(C);
	return scxOK;
}

// ###################################################

static int SPG_CONV scxPACKETRIAWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;
	PACKET_ADDRESS& Sx=State.Address.W;
	PACKET_BUFFER& PB=State.W.PB;


	pkBufferFillLastPacket(PB,0,0,PACKET_CMD_CREATE);

	if(Sx.EnableSTXETX) //Offset 0, BYTE STX
	{
		pkBufferFillLastPacket(PB,STR_STX,1,0); 
	}

	if(Sx.EnablePacketSize) //Offset 1, WORD PacketSize
	{
		WORD PacketSize=DataLen;//taille des données hors STX/Size/SeqID/.../Checksum/ETX
		pkBufferFillLastPacket(PB,&PacketSize,sizeof(PacketSize),0);
	}
/*
	if(State.Address.EnableWPacketSeqID) //Offset 3, BYTE SeqID
	{
		pkBufferFillLastPacket(State.PBW,&State.PBW.SeqID,1,0);
	}
*/
	pkBufferFillLastPacket(PB, Data, DataLen, PACKET_CMD_DATA);//Offset 4 Data

	if(Sx.EnableChecksum) //Offset 4 + DataLen, BYTE Checksum
	{
		BYTE Checksum=0x80;
		for(int i=0;i<DataLen;i++)
		{
			Checksum+=((BYTE*)Data)[i];
		}
		pkBufferFillLastPacket(PB,&Checksum,sizeof(Checksum),0);
	}

	if(Sx.EnableSTXETX) //Offset 5 + DataLen, BYTE ETX
	{
		pkBufferFillLastPacket(PB,STR_ETX,1,0);
	}

	pkBufferFillLastPacket(PB,0,0,PACKET_CMD_CLOSE);

	void* PacketAndHeader;
	int PacketAndHeaderLen;

	while(pkBufferHasFirstPacket(PB))
	{//si un message attend pour être envoyé
		if(pkBufferGetFirstPacketWData(PB,PacketAndHeader,PacketAndHeaderLen)==(PACKET_FULL|PACKET_HASDATA))
		{//si le message est valide
			if(scxWrite(PacketAndHeader,PacketAndHeaderLen,State.W.C)==0) break;//si l'envoie échoue le message restera dans le buffer d'envoi
		}
		else
		{
			scxWrite(PacketAndHeader,PacketAndHeaderLen,State.CTrash);
		}
		pkBufferRemoveFirstPacket(PB);
	}
	return DataLen;
}

// ###################################################

static int SPG_CONV scxPACKETRIARead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;
	PACKET_ADDRESS& Sx=State.Address.R;
	PACKET_BUFFER& PB=State.R.PB;

	int PacketRespect=State.R.C->CI->Type&sciPACKETRESPECT;//si le protocole respecte le concept de packet

	int HeaderSize=(Sx.EnableSTXETX?1:0)+ //STX
				   (Sx.EnablePacketSize?2:0); //WORD
				   //(Sx.EnablePacketSeqID?1:0); //BYTE

	int PacketSizeOffset=(Sx.EnableSTXETX?1:0);

	int TrailerSize=(Sx.EnableChecksum?1:0)+ //BYTE
					(Sx.EnableSTXETX?1:0); //ETX

	int HeaderTrailerSize=HeaderSize+TrailerSize;//longueur totale ajoutée aux données


	//-----------CREE UN NOUVEAU PACKET SI NECESSAIRE----------

	if(PacketRespect||  
		pkBufferGetLastPacketState(PB)&PACKET_FULL)//ou il n'y a pas d'ancien message ou celui qu'il y a est complet
	{//démarre un nouveau message
		pkBufferFillLastPacket(PB,0,0,PACKET_CMD_CREATE);
	}//else concatène au message précédent

#define PELT (*pkBufferGetLastPacket(PB)) //can not be null


	//-----------LIT LES DONNEES----------

	//concatène les données recues (ajouter un test de time out entre les anciennes données et les nouvelles basé par exemple sur 16 octets de transmission RS232 ou sur 250ms (un ping Ethernet))
	int ReadLen=PB.Size-(PELT.Start+PELT.Len);
	PELT.Len+=scxRead((BYTE*)PB.B+PELT.Start+PELT.Len,ReadLen,State.R.C);

	//if(ReadLen==0) rien de nouveau


	//-----------DECODE----------

#define pkDataB(Offset) (((BYTE*)PB.B)[PELT.Start+Offset])
#define pkDataW(Offset) (*(WORD*)&(pkDataB(Offset)))

	while((PELT.Len>0)&&((PELT.State&PACKET_FULL)==0)) // s'il est trop court le packet restera partiel jusqu'à réception de la suite des données
	{//debut du décodage d'un packet
		if(Sx.EnableSTXETX)
		{
			if(pkDataB(0)!=CHAR_STX) //Offset 0, BYTE STX
			{//le premier octet du packet est un octet d'erreur, on met le premier octet dans un packet d'erreur spécifique inséré
				if(PacketRespect)
				{
					pkBufferFillLastPacket(PB,0,0,PACKET_INVALIDSTX|PACKET_CMD_CLOSE);
				}
				else
				{//n'efface que le premier octet et retente un décodage
					pkBufferCutLastPacket(PB,PACKET_INVALIDSTX|PACKET_FULL,1,PACKET_PARTIAL);
				}
				continue; //puis on recommence le décodage du packet au début avec une trame plus courte (pas de boucle infinie)
			}
		}

		if(Sx.EnablePacketSize)//Offset 1, WORD PacketSize
		{//si le protocole gere intrinsèquement le concept de paquet (TCP ou UDP) le champ size est inutile et ne sert que de verification
			if(PELT.Len<(PacketSizeOffset+2)) break;// s'il est trop court le packet restera partiel jusqu'à réception de la suite des données

			if(
			   PacketRespect&&
			   (pkDataW(PacketSizeOffset)!=(PELT.Len-HeaderTrailerSize)) //ce test n'est pas réalisable en RS232
			  )
			{//le champ PacketSize n'est pas cohérent avec la taille lue par scxRead, on met le premier octet dans un packet d'erreur spécifique inséré
				//inserterrorpacket; ou trash all si le protocole respecte les paquets
				//pkCutLastPacket(PACKETINVALIDCHECKSUM|PACKET_FULL,PacketSizeOffset+2,PACKET_PARTIAL);//inserterrorpacket; ou trash all si le protocole respecte les paquets
				if(PacketRespect)
				{
					pkBufferFillLastPacket(PB,0,0,PACKET_INVALIDSIZE|PACKET_CMD_CLOSE);
				}
				else
				{//n'efface que le premier octet et retente un décodage
					pkBufferCutLastPacket(PB,PACKET_INVALIDSIZE|PACKET_FULL,1,PACKET_PARTIAL);
				}
				continue; //puis on recommence le décodage du packet au début avec une trame plus courte de un octet (STX est peut être dans ce qu'on prend pour le champ PacketSize)
			}

			PELT.DataStart=PELT.Start+HeaderSize;
			PELT.DataLen=pkDataW(PacketSizeOffset);//si le champ ne gere pas le concept de packet on fait confiance au champ PacketSize
			PELT.State|=PACKET_HASDATA;
			if(PELT.Len<PELT.DataLen+HeaderTrailerSize) 
			{//on a bien pu lire l'entete mais la suite des données n'est pas arrivée
				break;//le packet restera partiel jusqu'à réception de la suite des données
			} 
		}
		else if(PacketRespect)
		{//il n'y a pas de champ PacketSize mais le protocole respecte la taille des données sans troncature/concaténation
			PELT.DataStart=PELT.Start+HeaderSize;
			PELT.DataLen=PELT.Len-HeaderTrailerSize;
			PELT.State|=PACKET_HASDATA;
		}
		else
		{
			CHECKTWO(1,"scxPACKETRIARead: Missing EnableRPacketSize",State.R.C->CI->Name,pkBufferFillLastPacket(PB,0,0,PACKET_INVALIDSIZE|PACKET_CMD_CLOSE);break);
			//le protocole sousjacent ne garantit pas le respect des packets de données (troncature ou merge implicite)
			//et il n'y a pas de champ taille dans le packet
		}
		//à partir de là on a les données du message sur une longueur suffisante pour contenir data+checksum+ETX
		if(Sx.EnableSTXETX)
		{
			if(pkDataB(HeaderTrailerSize+PELT.DataLen-1)!=CHAR_ETX)//Dernier octet, ETX
			{//il manque ETX à la fin du message
				//inserterrorpacket; ou trash all si le protocole respecte les paquets
				if(PacketRespect)
				{
					pkBufferFillLastPacket(PB,0,0,PACKET_INVALIDETX|PACKET_CMD_CLOSE);
				}
				else
				{//n'efface que le premier octet et retente un décodage
					pkBufferCutLastPacket(PB,PACKET_INVALIDETX|PACKET_FULL,1,PACKET_PARTIAL);
				}
				continue; //puis on recommence le décodage du packet au début avec une trame plus courte (pas de boucle infinie)
			}
		}
/*
		if(State.Address.EnableRPacketSeqID)
		{
			if(PacketData[0]!=*(BYTE*)&State.PBR.SeqID)
			{
				pkBufferFillLastPacket(State.PBR,RawData,RawLen,PACKET_ELT_CREATE|PACKET_ELT_CLOSE|PACKET_INVALIDSeqID);
				goto scxPACKETRead_ReadPacket;
			}
			RawData++;RawLen--;
		}
*/

		if(Sx.EnableChecksum)
		{
			BYTE Checksum=0x80;
			for(int i=0;i<PELT.DataLen;i++)
			{
				Checksum+=pkDataB(HeaderSize+i);
			}
			if(pkDataB(HeaderSize+PELT.DataLen)!=Checksum)
			{//erreur de checksum
				//inserterrorpacket; ou trash all si le protocole respecte les paquets
				if(PacketRespect)
				{
					pkBufferFillLastPacket(PB,0,0,PACKET_INVALIDCHECKSUM|PACKET_CMD_CLOSE);
				}
				else
				{//n'efface que le premier octet et retente un décodage
					pkBufferCutLastPacket(PB,PACKET_INVALIDCHECKSUM|PACKET_FULL,1,PACKET_PARTIAL);
				}
				continue;
			}
		}

		//tout est OK, on coupe le packet apres ETX
		pkBufferCutLastPacket(PB,PACKET_FULL,HeaderTrailerSize+PELT.DataLen,PACKET_PARTIAL);//n'efface que le STX
	}//loop pour continuer le decodage du dernier packet du buffer

	int DataMaxLen=DataLen;//sauve pour le cas ou on lit plusieurs packets
	//lit les packet décodés
	while(pkBufferHasFirstPacket(PB))
	{//si un message attend pour être envoyé
		void* PBData=0;
		if((pkBufferGetFirstPacketRData(PB,PBData,DataLen)&(~PACKET_PARTIAL))==(PACKET_FULL|PACKET_HASDATA)) 
		{//Data et DataLen contiennent les données recues
			CHECK(DataLen>DataMaxLen,"scxPACKETRIARead:pkBufferGetFirstPacketRData",DataLen=V_Min(DataLen,DataMaxLen));
			memcpy(Data,PBData,DataLen);
			if(State.Address.EnableMultipleRead==0) pkBufferRemoveFirstPacket(PB); //attention si la connexion est lue à plusiers occasions (demultiplexage d'addresse) le remove ne sera effectué qu'après acquitement d'un des recepteurs
			//CHECK(PBData!=Data,"scxPACKETRIARead:pkBufferGetFirstPacketRData",return DataLen=0);
			return DataLen;//retourne le premier message valide
		}
		else if(State.CTrash)
		{
			scxWrite(PBData,DataLen,State.CTrash);
		}
		pkBufferRemoveFirstPacket(PB);
	}
	return DataLen=0;
}

// ###################################################

static void SPG_CONV scxPACKETRIACfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values
		Address->EnableMultipleRead=0;

		Address->R.Address=0;
		Address->R.CI=0;
		Address->R.BufferSize=8192;
		Address->R.EnablePacketSize=1;
		Address->R.EnableChecksum=1;
		//Address->R.EnablePacketSeqID=1;
		Address->R.EnableSTXETX=1;

		Address->W.Address=0;
		Address->W.CI=0;
		Address->W.BufferSize=1024;
		Address->W.EnablePacketSize=1;
		Address->W.EnableChecksum=1;
		//Address->W.EnablePacketSeqID=1;
		Address->W.EnableSTXETX=1;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);

	CFG_StringParam(CFG,"Name",Address->H.Name,0,1);

	CFG_IntParam(CFG,"R.Address",	(int*)&Address->R.Address,0,1);
	CFG_IntParam(CFG,"R.CI",	(int*)&Address->R.CI,0,1);
	CFG_IntParam(CFG,"R.BufferSize",	&Address->R.BufferSize,0,1,			CP_INT|CP_HASMIN|CP_HASMAX,0,65535);
	CFG_IntParam(CFG,"R.EnablePacketSize",&Address->R.EnablePacketSize,0,1,	CP_INT|CP_HASMIN|CP_HASMAX,0,1);
	CFG_IntParam(CFG,"R.EnableChecksum",&Address->R.EnableChecksum,0,1,		CP_INT|CP_HASMIN|CP_HASMAX,0,1);
	//CFG_IntParam(CFG,"R.EnablePacketSeqID",&Address->EnablePacketSeqID,0,CFG.FileName[0]?1:0,CP_INT|CP_HASMIN|CP_HASMAX,0,1);
	CFG_IntParam(CFG,"R.EnableSTXETX",	&Address->R.EnableSTXETX,0,	1,		CP_INT|CP_HASMIN|CP_HASMAX,0,1);

	CFG_IntParam(CFG,"W.Address",	(int*)&Address->W.Address,0,1);
	CFG_IntParam(CFG,"W.CI",	(int*)&Address->W.CI,0,1);
	CFG_IntParam(CFG,"W.BufferSize",	&Address->W.BufferSize,0,1,		CP_INT|CP_HASMIN|CP_HASMAX,0,65535);
	CFG_IntParam(CFG,"W.EnablePacketSize",&Address->W.EnablePacketSize,0,1,	CP_INT|CP_HASMIN|CP_HASMAX,0,1);
	CFG_IntParam(CFG,"W.EnableChecksum",&Address->W.EnableChecksum,0,1,		CP_INT|CP_HASMIN|CP_HASMAX,0,1);
	//CFG_IntParam(CFG,"W.EnablePacketSeqID",&Address->W.EnablePacketSeqID,0,CFG.FileName[0]?1:0,CP_INT|CP_HASMIN|CP_HASMAX,0,1);
	CFG_IntParam(CFG,"W.EnableSTXETX",	&Address->W.EnableSTXETX,0,1,		CP_INT|CP_HASMIN|CP_HASMAX,0,1);
/*
	scxCfgAddress(CFG, Address->RAddress, Address->RCI, Flag);
	scxCfgAddress(CFG, Address->WAddress, Address->WCI, Flag);
*/
	return;
}

// ###################################################

static int SPG_CONV scxPACKETRIASetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV scxPACKETRIAGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return scxINVALID;
}

// ###################################################

static int SPG_CONV sciPACKETRIADestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciPACKETRIACreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETRESPECT);
	strcpy(CI->Name,sci_NAME);

	CI->Description="PACKETRIA";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=0;//spécifique

	CI->scxOpen=scxPACKETRIAOpen;
	CI->scxClose=scxPACKETRIAClose;
	CI->scxWrite=scxPACKETRIAWrite;
	CI->scxRead=scxPACKETRIARead;
	CI->scxCfgAddress=scxPACKETRIACfgAddress;
	CI->scxSetParameter=scxPACKETRIASetParameter;
	CI->scxGetParameter=scxPACKETRIAGetParameter;
	CI->sciDestroyConnexionInterface=sciPACKETRIADestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#endif
