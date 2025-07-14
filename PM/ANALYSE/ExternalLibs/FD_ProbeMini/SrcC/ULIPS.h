
#ifdef SPG_General_USEULIPS

#define ULIPS_NAME_SIZE 8
#define ULIPS_DEFAULT_PORT 1978
#define ULIPS_DEFAULT_ELEMENTS 128

typedef BYTE LOGBYTE;

typedef struct
{
	LOGBYTE DataRateIn;
	LOGBYTE BlockSizeIn;
	LOGBYTE DataRateOut;
	LOGBYTE BlockSizeOut;
} ULIPS_CONNEXION_INFO;

typedef struct
{
	char Name[ULIPS_NAME_SIZE];
} ULIPS_Identifier;

typedef struct
{
	ULIPS_CONNEXION_INFO CxInfo;
	WORD ApplicationPort;
	char AppName[ULIPS_NAME_SIZE];
} ULIPS_Application;

#define ULIPS_STATE_NORMAL 1
#define ULIPS_STATE_BUSY 2

typedef struct
{
	SPG_NET_ADDR NetAddr;
	BYTE State;//Etat
	BYTE LifeTime;//0=local 1=copie recue en main propre >1 copie par intermédiaire
	//BYTE UserByte0;//KnownMask;
	//BYTE UserByte1;
	ULIPS_Identifier ID;//Nom
	ULIPS_Application APP;//Nom de l'application qui tourne
	BYTE UserData[ULIPS_NAME_SIZE];
} ULIPS_Element;

typedef enum
{
	ULIPS_ETAT_Normal,
	ULIPS_ETAT_StartQueryList,
	ULIPS_ETAT_ContinueQueryList,
	ULIPS_ETAT_QueryOne
} ULIPS_ETAT; 

typedef struct
{
	ULIPS_ETAT Etat;
	int PosInList;
	S_TimerCountType Ping;//Ping courant
	S_TimerCountType LastSend;//Dernier ping emis
	S_TimerCountType LastSendQueryList;//Dernier ping emis
	S_TimerCountType LastReceive;//Dernier Ping recu
	ULIPS_Element E;
} ULIPS_ListElement;

typedef struct
{
	//SPG_Console C;
	int Etat;
	SPG_NETWORK NET;
	S_TIMER T;
 	S_TimerCountType StartTime;//temps courant
 	S_TimerCountType CurTime;//temps courant
	S_TimerCountType QueryOldestPeriod;
	S_TimerCountType QueryListOldestPeriod;
	S_TimerCountType DeleteOldestPeriod;
	S_TimerCountType SendListElementPeriod;
	int MaxElement;
	int NumElement;
	ULIPS_ListElement* List;
} ULIPS_List;

typedef enum
{
	ULIPS_COMMANDE_ADD,
	ULIPS_COMMANDE_DEL,
	ULIPS_COMMANDE_QUERYLIST,
	ULIPS_COMMANDE_QUERYONE
} ULIPS_COMMANDE;

typedef struct
{
	ULIPS_COMMANDE Commande;
	ULIPS_Element E;
} ULIPS_Request;//Requete de liste, reponse

void SPG_CONV ULIPS_Combine(ULIPS_ListElement& L, ULIPS_ETAT Etat);
int SPG_CONV ULIPS_AddOrUpdate(ULIPS_List& L, ULIPS_Element& E, SPG_NET_ADDR& NetAddr, ULIPS_ETAT Etat);
int SPG_CONV ULIPS_Del(ULIPS_List& L, SPG_NET_ADDR& NetAddr);
int SPG_CONV ULIPS_Del(ULIPS_List& L, ULIPS_Element& E);
int SPG_CONV ULIPS_Del(ULIPS_List& L, ULIPS_Element& E, SPG_NET_ADDR& NetAddr);
void SPG_CONV ULIPS_Send(ULIPS_List& L, int ListEntry, ULIPS_COMMANDE Commande, SPG_NET_ADDR& IP);
int SPG_CONV ULIPS_Read(ULIPS_List& L, int& Commande, ULIPS_Element& E, SPG_NET_ADDR& IP);
void SPG_CONV ULIPS_Save(ULIPS_List& L, char* FileName);
void SPG_CONV ULIPS_Load(ULIPS_List& L, char* FileName);
int SPG_CONV ULIPS_StartNetUpdate(ULIPS_List& L, int ApplicationPort, int ULIPS_Port=ULIPS_DEFAULT_PORT, int MaxElement=ULIPS_DEFAULT_ELEMENTS);
int SPG_CONV ULIPS_UpdateQueryOldest(ULIPS_List& L);
int SPG_CONV ULIPS_UpdateSendListElement(ULIPS_List& L);
int SPG_CONV ULIPS_UpdateReadMsg(ULIPS_List& L);
int SPG_CONV ULIPS_UpdateNetUpdate(ULIPS_List& L);
void SPG_CONV ULIPS_StopNetUpdate(ULIPS_List& L);
void SPG_CONV ULIPS_ChangeState(ULIPS_List& L, int State);

#define ULocal(L) (L.List[0].E)
#define ULocalData(L) (L.List[0].E.UserData)
#define ULocalAppPort(L) (L.List[0].E.ID.ApplicationPort)
#define URemoteMax(L) (L.NumElements-1)
#define URemote(L,i) (L.List[((i)+1)].E)
#define URemoteData(L,i) (L.List[((i)+1)].E.UserData)

#define UIntToLogByte(v) V_Sature(V_Round(4.0f*logf((float)V_Max(v,1))/logf(2)),0,255);
#define ULogByteToInt(b) V_Round(powf(2,b/4.0f))

#endif

