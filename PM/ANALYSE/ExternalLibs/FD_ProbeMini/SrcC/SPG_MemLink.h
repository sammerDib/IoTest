
#ifdef SPG_General_USEMELINK

typedef struct
{
	int Type;
	int SizeE;//taille d'un element
	int SizeX;
	int SizeY;
//	int BlockSize;
} MELINK_FORMAT;

typedef struct
{
	BYTE* M;
	int Pitch;
	int PitchE;
} MELINK_DATA;

typedef struct
{
	int Etat;
//	int MaxMsgSize;
	int FlagSize;//taille des blocs a compacter

	int Downsampling;//pas du calcul d'erreur
	int XNumber;
	int YNumber;

	MELINK_FORMAT MF;
	MELINK_FORMAT DWNS_MF;
	MELINK_DATA Local;
	MELINK_DATA Remote;
	MELINK_DATA RemoteBackup;
#ifdef DebugMelinkTimer
	S_TIMER Total;
	S_TIMER FindMaxDiffSquare;
	S_TIMER ChooseEncodeMode;
	S_TIMER EncodeMsg;
	S_TIMER SetRcvMsgInternal;
	S_TIMER StartTime;
	float DebitMoyen;
	int TotalSent;
#endif
} MELINK_SEND;

typedef struct
{
	int Etat;
	MELINK_FORMAT MF;
	MELINK_DATA Local;
} MELINK_RCV;

typedef struct
{
	BYTE Mode;//MELINK_MODE+MELINK_PACKING+MELINK_SQUARE
	BYTE PosX;
	BYTE PosY;
	BYTE M[];
} MELINK_MSG;

#define MELINK_HEADER_SIZE 3

typedef struct
{
	int Max;
} MELINK_MINMAX_RGB;

typedef struct
{
	//MELINK_MINMAX_RGB Absolute;
	//MELINK_MINMAX_RGB Differential;
	MELINK_MINMAX_RGB DifferentialError;
} MELINK_STATS_RGB;

#include "SPG_MemLink.agh"

#define MELINK_LocalMem(MSND) MSND.Local.M
#define MELINK_RemoteMem(MSND) MSND.Remote.M
#define MELINK_ElementPtr(MD,x,y) MD.M+x*MD.PitchE+y*MD.Pitch
#define MELINK_LocalElementPtr(MSND,x,y) MELINK_ElementPtr(,MSND.Local,x,y)
#define MELINK_RemoteElementPtr(MSND,x,y) MELINK_ElementPtr(MSND.Remote,x,y)

#define MELINK_TYPE_RGB 1
#define MELINK_TYPE_BYTE 2
#define MELINK_TYPE_FLOAT 4

#define MELINK_DIST_BYTE(ME1,ME2) abs(*ME1-*ME2)
#define MELINK_DIST_RGB(ME1,ME2) (abs(ME1[0]-ME2[0])+abs(ME1[1]-ME2[1])+abs(ME1[2]-ME2[2]))
#define MELINK_DIST_FLOAT(ME1,ME2) (fabs(ME1[0]-ME2[0])+fabs(ME1[1]-ME2[1])+fabs(ME1[2]-ME2[2]))

#define MELINK_OK 1

#define MELINK_SaveSendState(MSND) CHECK_ELSE(MSND.Etat==0,"MELINK_SaveSendState: Non initialise",;) else memcpy(MSND.RemoteBackup.M,MSND.Remote.M,MSND.Remote.Pitch*MSND.MF.SizeY);
#define MELINK_RevertToSaved(MSND) CHECK_ELSE(MSND.Etat==0,"MELINK_SaveSendState: Non initialise",;) else memcpy(MSND.Remote.M,MSND.RemoteBackup.M,MSND.Remote.Pitch*MSND.MF.SizeY);

#endif

