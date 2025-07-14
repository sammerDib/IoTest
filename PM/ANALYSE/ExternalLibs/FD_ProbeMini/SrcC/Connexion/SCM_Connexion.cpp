
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#include "..\SPG_Includes.h"

#ifdef SCX_ADDRESS_HASTIMER
typedef struct SCX_TIMER
{
	S_TIMER tWrite;
	S_TIMER tRead;
} SCX_TIMER;
#endif

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire
	BYTE Private[];
} SCX_ADDRESS;

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;
} SCX_STATE; //parametres d'une connexion en particulier

#include <string.h>
#include <stdio.h>

#include "SCM_ConnexionDbg.h"


// Interface vers les connexions

// implémentés: 
// Fichier, address = nom du fichier, read XOR write
// RS232, address = port, speed
// Buffer, address = taille mémoire, buffer circulaire en lecture, pas de bufferisation en écriture
// Chaine, address = connexion* C1, connexion* C2 (ecrit dans C2 les données de C1 et réciproquement)
// Double, address = connexion* C1, connexion* C2 (duplique les données vers deux connexions en écriture, concatène les données de deux connexions en lecture)
// UDP, address = LocalPort, destination (IP et port)

// implémentables sans développement
// GPIB
// sound capture
// avi
// modem (SrcB)

// non implémentés:
// zip/rle
// sorties fichiers à format spécifique
// TCP
// générateur de packets
// Buffer à validation de lecture, buffer circulaire en lecture, pas de bufferisation en écriture
// protocole RIA/Packet/Checksum Address=connexion*C, stx(O/N) + size(O/N) + Address(0-255) + checksum(O/N), lit et ecrit [STX][Address][Size][data][Checksum][ETX], utilise SetParameter pour lock/unlock en lecture permettant plusieurs ecoutes sur la même connexion
// packet buffer Address=size, packetmode lit, bufferise et rend des paquets soit en respectant la taille de données soit en faisant un merge (même différence que entre UDP et RS232)

// LES FONCTIONS DE L'INTERFACE

//----------------------------------------

SCX_CONNEXION* SPG_CONV scxOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* &Address, int scxOpenFlag)
{
	sciCHECKG(CI, "scxOpen");

	SPG_MemFastCheck();
	CHECKTWO(CI->scxOpen==0,"scxOpen non implémenté",CI->Name,return 0);

	scxSendDebugConnexionAddress("scxOpen","Trying to open",CI,Address);
#ifdef SCX_ADDRESS_HASsizeofAddress
	CHECKTWO(CI->sizeofAddress!=Address->H.sizeofAddress,"scxOpen: Incompatible sizeofAddress",CI->Name,return 0);
#endif
#ifdef SCX_ADDRESS_HASUID
	CHECKTWO(CI->TypeUID!=Address->H.TypeUID,"scxOpen: Incompatible TypeUID",CI->Name,return 0);
#endif
#ifdef SCX_ADDRESS_HASNAME
	CHECKTWO(strncmp(CI->Name,Address->H.Name,SCI_CONNEXION_NAME),"scxOpen: Incompatible address name",CI->Name,return 0);
#endif
#ifdef SPG_DEBUGCONFIG
#ifdef DebugMem
	CHECKTWO(SPG_MemIsExactBlock(Address,Address->H.sizeofAddress)==-1,"scxOpen: Invalid size or memory block",CI->Name,return 0);
#endif
#endif

#ifdef SCX_ADDRESS_HASGLOGTIME
	int LT_SCX=0;
	{
		struct
		{
			int LT_SCX;
		} C;
		C.LT_SCX=0;
		char Descr[LT_STR];
		sprintf(Descr,"%s:%i",CI->Name,CI->TypeUID);
		LTG_CreateDescr(C,LT_FLAG_NOCHECK  ,LT_SCX,0,Descr);
		LTG_EnterI(C,LT_SCX,CI->TypeUID);
		LT_SCX=C.LT_SCX;
	}
#endif

#ifdef DebugMem
	int SnapAllocated=Global.MS.NumAllocated-Global.MS.NumFreed; //verifie l'abscence de leak quand la creation echoue
	//CHECK(Global.MS.NumAllocated=0x6A,"Test",;);
#endif

	SCX_CONNEXION* C=CI->scxOpen(CI,Address,scxOpenFlag);

#ifdef DebugMem
	int ResAllocated=Global.MS.NumAllocated-Global.MS.NumFreed; //verifie l'abscence de leak quand la creation echoue
	DbgCHECKTWO( (C==0) &&  (CI->Type&sciPROTOCOL)         && (ResAllocated>=SnapAllocated), "scxOpen", Address->H.Name); //doit desallouer les sous adresses R et W ou la sous adresse R=W
	DbgCHECKTWO( (C==0) && ((CI->Type&sciPROTOCOL)==0) && (ResAllocated!=SnapAllocated), "scxOpen", Address->H.Name);
#endif

	if(C)
	{
#ifdef SCX_ADDRESS_HASGLOGTIME
		C->Address->H.LT_SCX=LT_SCX;
#endif
		scxSendDebugConnexionAddress("scxOpen","Open successful",CI,Address);
		C->CI=CI;
		CHECKTWO(C->Address==0,"scxOpen implementation error",CI->Name,return 0);
		//DbgCHECKTWO(scxIsNotTypeUID(C,17)&&scxIsNotTypeUID(C,22)&&memcmp(C->Address,Address,CI->sizeofAddress),"scxOpen please check implementation",CI->Name);
#ifdef SCX_ADDRESS_HASTIMER
		C->Address->H.Timer=SPG_TypeAlloc(1,SCX_TIMER,"scxOpen:Timer");
		S_InitTimer(C->Address->H.Timer->tWrite,"tWrite");
		S_InitTimer(C->Address->H.Timer->tRead,"tRead");
#endif
#ifdef SCX_ADDRESS_HASGLOGTIME
		LTG_ExitI(C->Address->H,LT_SCX,0);
#endif
	}
	else
	{
		scxSendDebugConnexionAddress("scxOpen","Open failed",CI,Address);
	}

	if((scxOpenFlag&SCXOPENPRESERVEADDRESS)==0) scxDestroyAddress(Address);

	SPG_MemFastCheck();
	return C;
}

//----------------------------------------

int SPG_CONV scxClose(SCX_CONNEXION* &C)
{
	scxCHECKG(C, "scxClose");
	CHECKTWO(C->CI->scxClose==0,"scxClose non implémenté",C->CI->Name,return 0);

#ifdef SCX_ADDRESS_HASLOGTIME
	LTG_EnterC(C->Address->H,LT_SCX,0);
	int LT_SCX=C->Address->H.LT_SCX;
#endif

#ifdef SCX_ADDRESS_HASTIMER
	if(C->Address->H.Timer) 
	{
		S_CloseTimer(C->Address->H.Timer->tWrite);
		S_CloseTimer(C->Address->H.Timer->tRead);
		SPG_MemFree(C->Address->H.Timer);
	}
#endif

	scxSendDebugConnexion("scxClose","Closing",C);

	int r=C->CI->scxClose(C);

	C=0;//pour eviter d'appeler la callback sur une connexion invalide

	scxSendDebugRetval("scxClose","Closing returned",r);//attention la connexion est nulle si on fait scxClose(scxLog)
	//SPG_MemFastCheck();
#ifdef SCX_ADDRESS_HASTIMER
	{
		struct
		{
			int LT_SCX;
		} C;
		C.LT_SCX=LT_SCX;
		LTG_ExitC(C,LT_SCX,0);
	}
#endif
	return r;
}

//----------------------------------------

int SPG_CONV scxWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	TRY_BEGIN
	//SPG_MemFastCheck();
	scxCHECKG(C, "scxWrite");
	CHECKTWO(Data==0,"scxWrite",C->CI->Name,return 0);
	CHECKTWO(DataLen<=0,"scxWrite",C->CI->Name,return 0);
	if(C->CI->maxPacketSize) CHECK(DataLen>C->CI->maxPacketSize,"scxWrite",return 0);
	CHECKTWO(C->CI->scxWrite==0,"scxWrite non implémenté",C->CI->Name,return 0);
	scxSendDebugData("scxWrite","Writing",Data,DataLen,C);

#ifdef SCX_ADDRESS_HASGLOGTIME
	if((C->CI->Type&sciPROTOCOL)==0) { LTG_Enter(C->Address->H,LT_SCX,DataLen); }
#endif

#ifdef SCX_ADDRESS_HASTIMER
	if(C->Address->H.Timer) { S_StartTimer(C->Address->H.Timer->tWrite); }
#endif
	int w=C->CI->scxWrite(Data,DataLen,C);
#ifdef SCX_ADDRESS_HASTIMER
	if(C->Address->H.Timer) { S_StopTimer(C->Address->H.Timer->tWrite); }
#endif
	DbgCHECKTWO(w>DataLen,"scxWrite: Implémentation incorrecte",C->CI->Name);
	scxSendDebugConnexionRetval("scxWrite","return",w,C);
	//SPG_MemFastCheck();
#ifdef SCX_ADDRESS_HASGLOGTIME
	if((C->CI->Type&sciPROTOCOL)==0) { LTG_Exit(C->Address->H,LT_SCX,w); }
#endif
	return w;
	TRY_ENDGRZ("scxWrite")
}

//----------------------------------------

int SPG_CONV scxRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	TRY_BEGIN
	//SPG_MemFastCheck();
	scxCHECKR(C, "scxRead");
	CHECKTWO(Data==0,"scxRead",C->CI->Name,return DataLen=0);
	CHECKTWO(DataLen==0,"scxRead",C->CI->Name,return DataLen=0);
	CHECKTWO(C->CI->scxRead==0,"scxRead non implémenté",C->CI->Name,return DataLen=0);
#ifdef SCX_ADDRESS_HASGLOGTIME
	if((C->CI->Type&sciPROTOCOL)==0) { LTG_Enter(C->Address->H,LT_SCX,DataLen); }
#endif
	scxSendDebugConnexionLen("scxRead","Reading",Data,DataLen,C);
	int InitialDataLen=DataLen;
#ifdef SCX_ADDRESS_HASTIMER
	if(C->Address->H.Timer) { S_StartTimer(C->Address->H.Timer->tRead); }
#endif
	int r=C->CI->scxRead(Data,DataLen,C);//renvoie -1 si une opération asynchrone est en cours
#ifdef SCX_ADDRESS_HASTIMER
	if(C->Address->H.Timer) { S_StopTimer(C->Address->H.Timer->tRead); }
#endif
	DbgCHECK(r!=DataLen,"scxRead: Implémentation incorrecte");
	DbgCHECK(r>InitialDataLen,"scxRead: Implémentation incorrecte");
	//DbgCHECK((C->CI->Type&sciPACKETRESPECT)&&(InitialDataLen==DataLen)&&(InitialDataLen>1),"scxRead: Message potentially larger than buffer");
	scxSendDebugDataRetval("scxRead","return",Data,DataLen,r,C);
	//SPG_MemFastCheck();
#ifdef SCX_ADDRESS_HASGLOGTIME
	if((C->CI->Type&sciPROTOCOL)==0) { LTG_Exit(C->Address->H,LT_SCX,r); }
#endif
	return r;
	TRY_ENDGRZ("scxRead");
}

//----------------------------------------

SCX_ADDRESS* SPG_CONV scxCreateAddress(SCI_CONNEXIONINTERFACE* CI)
{
	//SPG_MemFastCheck();
	sciCHECKG(CI, "scxCreateAddress");

	SCX_ADDRESS* Address=(SCX_ADDRESS*)scxMemAlloc(CI->sizeofAddress,CI->Name);
	SPG_CatMemName(Address,"(scxCreateAddress)");

	CHECK(Address==0,"scxCreateAddress",return 0);
#ifdef SCX_ADDRESS_HASsizeofAddress
	Address->H.sizeofAddress=CI->sizeofAddress;
#endif
#ifdef SCX_ADDRESS_HASUID
	Address->H.TypeUID=CI->TypeUID;
#endif
#ifdef SCX_ADDRESS_HASNAME
	strcpy(Address->H.Name,CI->Name);
#endif
	//SPG_MemFastCheck();
	return Address;
}

int SPG_CONV scxAddressCompare(SCX_ADDRESS* A, SCX_ADDRESS* B)
{
	if(A==B) return 0;
	if(A==0) return -1;
	if(B==0) return -1;
#ifdef SCX_ADDRESS_HASsizeofAddress
	if(A->H.sizeofAddress!=B->H.sizeofAddress) return -1;
#endif
#ifdef DebugMem
	CHECK(!SPG_MemIsBlock(A,Global.MS),"scxAddressCompare",return -1);
	CHECK(!SPG_MemIsBlock(B,Global.MS),"scxAddressCompare",return -1);
#endif
	//SPG_MemFastCheck();
	return memcmp(A,B,A->H.sizeofAddress);
}

int SPG_CONV scxAddressCopy(SCX_ADDRESS* D, SCX_ADDRESS* S)
{
	if(S==D) return 0;
	if(S==0) return 0;
	if(D==0) return 0;
#ifdef SCX_ADDRESS_HASsizeofAddress
	if(D->H.sizeofAddress!=S->H.sizeofAddress) return 0;
#endif
	//SPG_MemFastCheck();
	memcpy(D,S,S->H.sizeofAddress);
	return -1;
}

int SPG_CONV scxAddressSave(SCX_ADDRESS* S, char* Filename)
{
	return SPG_SaveFile(Filename,(BYTE*)S,S->H.sizeofAddress);
}

//----------------------------------------

void SPG_CONV scxDestroyAddress(SCX_ADDRESS* &Address)
{//voir sciDestroyAddress pour desallouer aussi les descendants (PROTOCOL) et sciDestroyAddressFromCFG pour supprimer les references à un CFG reste ouvert
	//SPG_MemFastCheck();
	CHECK(Address==0,"scxDestroyAddress",return);
#ifdef DebugMem
	DbgCHECKTWO(SPG_MemIsExactBlock(Address,Address->H.sizeofAddress)==0,"scxOpen: Invalid size or memory block",Address->H.Name);
	memset(Address,0,Address->H.sizeofAddress);
#endif
	scxFree(Address);
	//SPG_MemFastCheck();
	return;
}

//----------------------------------------

SCX_ADDRESS* SPG_CONV scxCreateCfgAddress(SCI_CONNEXIONINTERFACE* CI,SPG_CONFIGFILE& CFG,int scxCfgFlag)
{
	//SPG_MemFastCheck();
	sciCHECKG(CI, "scxCreateCfgAddress");
	SCX_ADDRESS* Address=scxCreateAddress(CI);
	CHECKTWO(Address==0,"scxCreateCfgAddress",CI->Name,return 0);
	scxCfgAddress(CFG,Address,CI,scxCfgFlag);
	//SPG_MemFastCheck();
	return Address;
}

//----------------------------------------

int SPG_CONV scxCfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, SCI_CONNEXIONINTERFACE* CI, int scxCfgFlag)
{
	//SPG_MemFastCheck();
	CHECK(Address==0,"scxCfgAddress",return scxINVALID);
	sciCHECKG(CI, "scxCfgAddress");//variables crées CI,sciParameters,scxParameters
	CHECKTWO(CI->scxCfgAddress==0,"scxCfgAddress non implémenté",CI->Name,return 0);
	//CHECKTWO(Address==0,"scxCfgAddress",CI->Name,return 0);
	CHECKTWO(CFG.Etat&&(CFG.Etat!=-1),"scxCfgAddress: Missing SPG_ZeroStruct on SPG_CONFIGFILE CFG",CI->Name,return 0);
	CI->scxCfgAddress(CFG,Address,scxCfgFlag);
	CFG_ParamsFromPtr(CFG);
	//SPG_MemFastCheck();
	return scxOK;
}

//----------------------------------------

int SPG_CONV scxSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECKG(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	CHECK(ParamName==0,"scxSetParameter", return 0);
	CHECK(Value==0,"scxSetParameter", return 0);
	CHECKTWO(C->CI->scxSetParameter==0,"scxSetParameter non implémenté",C->CI->Name,return 0);
	return C->CI->scxSetParameter(ParamName,Value,C);
}

//----------------------------------------

int SPG_CONV scxGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECKG(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	CHECK(ParamName==0,"scxGetParameter", return 0);
	CHECK(Value==0,"scxGetParameter", return 0);
	CHECKTWO(C->CI->scxGetParameter==0,"scxGetParameter non implémenté",C->CI->Name,return 0);
	return C->CI->scxGetParameter(ParamName,Value,C);
}

//----------------------------------------

void SPG_CONV sciDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* &CI)
{
	//SPG_MemFastCheck();
	sciCHECKGNR(CI, "sciDestroyConnexionInterface");
	CHECKTWO(CI->sciDestroyConnexionInterface==0,"sciDestroyConnexionInterface non implémenté",CI->Name,return);
	CI->sciDestroyConnexionInterface(CI);
	CI=0;
	return;
}

#endif
