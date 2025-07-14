
#ifdef SPG_General_USENetwork_Protocol

//define statique a n'utiliser que dans les dimensionnements
//de buffers statiques
#define SNP_MAX_MSG 16384
#define SNP_MIN_MSG 448
//define specifique pour les messages exploitant la taille maximum
#define SNP_DYN_MSG(SNP) SNP.MaxSize

#define SNP_MAX_MSG_TYPE 128
#define SNP_HEADER_SIZE 2

typedef struct
{
	WORD Type;
	BYTE M[SNP_MAX_MSG];
} SNP_MESSAGE;

typedef struct
{
	int MaxSize;//MTU
	int Len;//taille du dernier message lu header non inclus (taille de uniquement Message.M)
	SNP_MESSAGE Message;//dernier message lu
	SPG_NET_ADDR SNA;//son adresse source
	SPG_NETWORK SN;
} SPG_NET_PROTOCOL;

typedef struct
{
	SPG_NET_ADDR Destination;
	MELINK_SEND MSND;
	S_TIMER ScreenSend;
	int AllowedRate;//bytes/sec
	float AllowedCPU;
	int LenPacketSent;
} MELINK_SCREEN_SEND_STATE;

typedef struct
{
	SPG_NET_ADDR Remote;
	MELINK_RCV MRCV;
} MELINK_SCREEN_RCV_STATE;

#define SNP_MaxPacketSizeTime 2.0f
#ifdef DebugNetwork
//#define SNP_MaxPingTime 10.0
//#define SNP_MaxWaitTime 30.0
#define SNP_MaxPingTime 10.0f
#define SNP_MaxWaitTime 30.0f
#else
#define SNP_MaxPingTime 1.0f
#define SNP_MaxWaitTime 30.0f
#endif

#define SNP_IsType(SNP,CheckType) (SNP.Message.Type==CheckType)
#define SNP_IsLen(SNP,CheckLen) (SNP.Len==CheckLen)
#define SNP_IsFrom(SNP,CheckSNA) SPG_IsEqualNetAddr(SNP.SNA,CheckSNA)
#define SNP_IsPresent(SNP,CheckSNA,CheckType,CheckLen) (SNP_IsType(SNP,CheckType)&&SNP_IsLen(SNP,CheckLen)&&SNP_IsFrom(SNP,CheckSNA))
#define SNP_IF_READ_IsTypeIsLen(SNP,CheckType,TypeName,VarName) if(SNP_IsType(SNP,CheckType)&&SNP_IsLen(SNP,sizeof(TypeName))){TypeName* VarName=(TypeName*)SNP.Message.M;
#define SNP_END_IF_READ(SNP) }

/*
messages systeme
*/
#define SNP_PING_REQUEST 1
#define SNP_PING_RESPONSE 2
#define SNP_PACKETSIZE 3
/*
messages systeme <8
*/
#define SNP_SYSMSG 8
/*
messages lib
*/

//demande la taille de l'ecran
#define SNP_SCREEN_REQUEST SNP_SYSMSG
//messages spg_list
#define SNP_LIST SNP_SYSMSG+1
//demander a etre destination des messages d'erreur
#define SNP_ERROR_REPORT SNP_SYSMSG+2

#define SNP_MOUSE SNP_SYSMSG+3
#define SNP_CONTROL_REQUEST SNP_SYSMSG+4
//#define SNP_VIEW_REQUEST SNP_SYSMSG+5
#define SNP_P_STARTLOADPROFILE SNP_SYSMSG+6
#define SNP_P_STARTSAVEPROFILE SNP_SYSMSG+7
#define SNP_C_STARTLOADCUT SNP_SYSMSG+8
#define SNP_C_STARTSAVECUT SNP_SYSMSG+9

#define SNP_SCREEN_RESPONSE SNP_SYSMSG+10
#define SNP_CONTROL_RESPONSE SNP_SYSMSG+11
//#define SNP_CONTROL_RESPONSE_SIZE SNP_SYSMSG+12
//#define SNP_VIEW_RESPONSE SNP_SYSMSG+13
//#define SNP_VIEW_RESPONSE_SIZE SNP_SYSMSG+14
//#define SNP_VIEW_SCREENUPDATE SNP_SYSMSG+15
#define SNP_BREAK SNP_SYSMSG+16

#define PROFIL_DWNLOAD 32
#define SNP_P_STRUCT PROFIL_DWNLOAD
#define SNP_P_DATA_SEND PROFIL_DWNLOAD+1
#define SNP_P_DATA_RESPONSE PROFIL_DWNLOAD+2
#define SNP_P_MSK_SEND PROFIL_DWNLOAD+3
#define SNP_P_MSK_RESPONSE PROFIL_DWNLOAD+4

#define CUT_DWNLOAD 48
#define SNP_C_STRUCT CUT_DWNLOAD
#define SNP_C_DATA_SEND CUT_DWNLOAD+1
#define SNP_C_DATA_RESPONSE CUT_DWNLOAD+2
#define SNP_C_MSK_SEND CUT_DWNLOAD+3
#define SNP_C_MSK_RESPONSE CUT_DWNLOAD+4

#define SCREEN_DWNLOAD 64
#define SNP_SCREEN_CONTENT_SEND SCREEN_DWNLOAD



/*
messages lib <16
*/
#define SNP_SPGMSG 96
/*
messages user
*/

#include "SPG_Network_Protocol.agh"

#endif

