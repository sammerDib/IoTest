
#ifndef SPG_CONV
#error Convention d'appel non définie
#endif

typedef enum
{
	scxINVALID=0,
	scxOK=1
} SCX_ETAT;

typedef enum
{
	sciNONE=0,
	sciREAD=1,
	sciWRITE=2,
	sciBOTH=3,
	sciNODLG=4,
	sciPACKETMERGE=8,
	sciPACKETRESPECT=16,
	sciPROTOCOL=32,

	/*
	sciOVERWRITE=64,
	sciWRITETHROUGH=128,
	sciCONTINUOUS=256,
	sciCAMERA=512,
	sciBLADE=1024
	*/

} SCI_TYPE;

#define SCI_CONNEXION_NAME 16

#define SCX_ADDRESS_HASsizeofAddress
#define SCX_ADDRESS_HASUID
#define SCX_ADDRESS_HASNAME
//efine SCX_ADDRESS_HASTIMER
//efine SCX_ADDRESS_HASGLOGTIME

#ifdef SCX_ADDRESS_HASTIMER
struct SCX_TIMER;//Private dans SCM_Connexion.h
#endif

//--------------------------------------------------------------

typedef struct
{
#ifdef SCX_ADDRESS_HASsizeofAddress
	WORD sizeofAddress;
#endif
#ifdef SCX_ADDRESS_HASUID
	WORD TypeUID;
#endif
#ifdef SCX_ADDRESS_HASNAME
	char Name[SCI_CONNEXION_NAME];
#endif
#ifdef SCX_ADDRESS_HASGLOGTIME
	int LT_SCX;
#endif
#ifdef SCX_ADDRESS_HASTIMER
	SCX_TIMER* Timer;
#endif
} SCX_ADDRESSHEADER;

//--------------------------------------------------------------

struct SCX_ADDRESS;//Private, commencant par SCX_ADDRESSHEADER, déclaré dans chaque CPP d'implémentation
struct SCX_STATE;//Private, commencant par SCX_ADDRESS, déclaré dans chaque CPP d'implémentation
struct SCI_CONNEXIONINTERFACE;//Public, déclaré ci dessous
struct SCI_PARAMETERS;//Private, déclaré dans chaque CPP d'implémentation

//--------------------------------------------------------------

#define SCX_MAXUSEREXTENSION 16

struct SCX_CONNEXION;

//--------------------------------------------------------------

typedef SCI_CONNEXIONINTERFACE*(SPG_CONV  *SCI_CREATE)();
typedef SCX_CONNEXION*(SPG_CONV  *SCX_OPEN        )(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag);
typedef int           (SPG_CONV  *SCX_CLOSE       )(SCX_CONNEXION* C);
typedef int           (SPG_CONV  *SCX_WRITE       )(void* Data, int DataLen, SCX_CONNEXION* C);
typedef int           (SPG_CONV  *SCX_READ        )(void* Data, int& DataLen, SCX_CONNEXION* C);
typedef void          (SPG_CONV  *SCX_CFGADDRESS  )(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag);
typedef int           (SPG_CONV  *SCX_SETPARAMETER)(char* ParamName, void* Value, SCX_CONNEXION* C);
typedef int           (SPG_CONV  *SCX_GETPARAMETER)(char* ParamName, void* Value, SCX_CONNEXION* C);
typedef int           (SPG_CONV  *SCI_DESTROY)(SCI_CONNEXIONINTERFACE* CI);

typedef int           (SPG_CONV  *SCX_USEREXTENSION)(SCX_CONNEXION* C);


//--------------------------------------------------------------

typedef struct SCX_CONNEXION
{
	SCX_ETAT Etat;
	union
	{
		SCX_ADDRESS* Address;//Address pour référence ultérieure (flags)
		struct SCX_STATE* State;//Internal state, dont le premier champ est une Addresse
	};
	struct SCI_CONNEXIONINTERFACE* CI;//SCI_CONNEXIONINTERFACE

	SCX_USEREXTENSION UserFctPtr[SCX_MAXUSEREXTENSION];
	SCX_CONNEXION* UserFctData[SCX_MAXUSEREXTENSION];

} SCX_CONNEXION;

typedef struct SCI_CONNEXIONINTERFACE
{
	SCX_ETAT Etat;
	SCI_TYPE Type;
	int sizeofAddress;
	int TypeUID;
	char Name[SCI_CONNEXION_NAME];
	char* Description;
	int maxPacketSize;
	SCI_PARAMETERS* sciParameters;//internal state

	SCX_OPEN         scxOpen;
	SCX_CLOSE        scxClose;
	SCX_WRITE        scxWrite;
	SCX_READ         scxRead;
	SCX_CFGADDRESS   scxCfgAddress;
	SCX_SETPARAMETER scxSetParameter;
	SCX_GETPARAMETER scxGetParameter;
	SCI_DESTROY sciDestroyConnexionInterface;

} SCI_CONNEXIONINTERFACE;

//--------------------------------------------------------------

//Flag
#define scxCFGADDRESSSETDEFAULT 1
#define scxCFGREADONLY 2
#define scxCFGTERMINALREAD 4
#define scxCFGTERMINALWRITE 8

#define SCXOPENPRESERVEADDRESS 1

#define scxTypeAlloc(TYPE,Name) SPG_TypeAlloc(1,TYPE,Name)
#define scxMemAlloc(Size,Name) SPG_MemAlloc(Size,Name)
#define scxFree(PTR) SPG_MemFree(PTR)

#define sciCHECK(CI, fctName) \
	CHECK((CI)==0,sci_NAME "\r\n" fctName,return 0);\
	CHECK((CI)->Etat!=scxOK,sci_NAME "\r\n" fctName,return 0);\
	CHECK((CI)->TypeUID!=sci_UID,sci_NAME "\r\n" fctName,return 0);\
	CHECK((CI)->sciParameters==0,sci_NAME "\r\n" fctName,return 0);

#define scxCHECK(C, fctName) \
	CHECK(C==0,sci_NAME "\r\n" fctName,return 0);\
	CHECK(C->Etat!=scxOK,sci_NAME "\r\n" fctName,return 0);\
	CHECK(C->State==0,sci_NAME "\r\n" fctName,return 0);\
	sciCHECK((C->CI), fctName);

#define sciCHECKGNR(CI, fctName) \
	CHECK((CI)==0,fctName,return);\
	CHECK((CI)->Etat!=scxOK,fctName,return);\
	CHECK((CI)->sciParameters==0,fctName,return);
#define sciCHECKG(CI, fctName) \
	CHECK((CI)==0,fctName,return 0);\
	CHECK((CI)->Etat!=scxOK,fctName,return 0);\
	CHECK((CI)->sciParameters==0,fctName,return 0);
#define sciCHECKR(CI, fctName) \
	CHECK((CI)==0,fctName,return DataLen=0);\
	CHECK((CI)->Etat!=scxOK,fctName,return DataLen=0);\
	CHECK((CI)->sciParameters==0,fctName,return DataLen=0);

#define scxCHECKG(C, fctName) \
	CHECK(C==0,fctName,return 0);\
	CHECK(C->Etat!=scxOK,fctName,return 0);\
	CHECK(C->State==0,fctName,return 0);\
	sciCHECKG(C->CI, fctName);
#define scxCHECKR(C, fctName) \
	CHECK(C==0,fctName,return DataLen=0);\
	CHECK(C->Etat!=scxOK,fctName,return DataLen=0);\
	CHECK(C->State==0,fctName,return DataLen=0);\
	sciCHECKR(C->CI, fctName);

//--------------------------------------------------------------

SCX_CONNEXION* SPG_CONV scxOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* &Address, int scxOpenFlag=0);
int SPG_CONV scxClose(SCX_CONNEXION* &C);
int SPG_CONV scxWrite(void* Data, int DataLen, SCX_CONNEXION* C);
#define scxWriteStrZ(StrZstring,C) {CHECK_ELSE((char*)StrZstring==0,"scxWriteStrZ",;) else scxWrite(StrZstring,strlen(StrZstring),C);}
int SPG_CONV scxRead(void* Data, int& DataLen, SCX_CONNEXION* C);
SCX_ADDRESS* SPG_CONV scxCreateAddress(SCI_CONNEXIONINTERFACE* CI);
int SPG_CONV scxAddressCompare(SCX_ADDRESS* A, SCX_ADDRESS* B);
int SPG_CONV scxAddressCopy(SCX_ADDRESS* D, SCX_ADDRESS* S);
int SPG_CONV scxAddressSave(SCX_ADDRESS* S, char* Filename);
void SPG_CONV scxDestroyAddress(SCX_ADDRESS* &Address);
SCX_ADDRESS* SPG_CONV scxCreateCfgAddress(SCI_CONNEXIONINTERFACE* CI,SPG_CONFIGFILE& CFG,int scxCfgFlag=scxCFGADDRESSSETDEFAULT);
int SPG_CONV scxCfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, SCI_CONNEXIONINTERFACE* CI, int scxCfgFlag);
int SPG_CONV scxSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C);
int SPG_CONV scxGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C);
void SPG_CONV sciDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* &CI);

//--------------------------------------------------------------

//efine scxDestroyAddressZ(Address) {scxDestroyAddress(Address);Address=0;}
#define sciIsTypeUID(C,t) (  (C) && (( (SCX_ADDRESSHEADER*)(C))->TypeUID==t)  )
#define sciIsNotTypeUID(C,t) (  (C) && (( (SCX_ADDRESSHEADER*)(C))->TypeUID!=t)  )
#define sciIsNullOrVoid(C,t) (  (C==0) || (( (SCX_ADDRESSHEADER*)(C))->TypeUID==sci_UID_VOID)  )

#define scxIsTypeUID(C,t) (  (C) && (( (SCX_ADDRESSHEADER*)((C)->Address) )->TypeUID==t)  )
#define scxIsNotTypeUID(C,t) (  (C) && (( (SCX_ADDRESSHEADER*)((C)->Address) )->TypeUID!=t)  )
#define scxIsNullOrVoid(C,t) (  (C==0) || (( (SCX_ADDRESSHEADER*)((C)->Address) )->TypeUID==sci_UID_VOID)  )

//--------------------------------------------------------------

#ifdef DebugNetwork
typedef void(SPG_CONV *MSGCALLBACK)(char* Msg, void* UserData);
void SPG_CONV scxStartDebug(MSGCALLBACK MsgCallback, void* UserData);
void SPG_CONV scxStopDebug();
//void SPG_CONV scxDataToHEX(void* Data, int DataLen, char* Msg, int MsgLen);
#else
#define scxStartDebug(MsgCallback, UserData)
#define scxStopDebug()
#endif

//--------------------------------------------------------------


