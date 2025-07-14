
#ifdef SPG_General_USELOGTIME

#error "SPG_LogTime obsolete"

#define SPG_LOG_MAXPARAM 8
#define LT_STR 128

typedef struct
{
	S_TimerCountType Tick;//8 octets
	int Code;
	WORD UID;
	WORD Flag;
#ifdef SPG_General_USELOGTIMETHREADID
	DWORD TID;
#endif
} SPG_LOGEVENT;//16 octets

#define LT_ENTER	0x8000
#define LT_REENT	0x4000
#define LT_EXIT		0x2000
#define LT_INIT		0x1000
#define LT_CLOSE	0x0800

#define SPG_LOGEVENT_REENTER SPG_LOGEVENT_REENT

typedef struct
{
	DWORD Instance;//ou pour différencier plusieurs instances d'un même objet
	DWORD TID;
	int Flag;//LT_FLAG_NOCHECK
#ifdef SPG_General_USELOGTIMETHREADID
	int EntryCount;
#endif
	char File[LT_STR];//source file
	char Name[LT_STR];
	char Descr[LT_STR];
} SPG_EVENTTYPE;

typedef struct
{
	int Etat;

	int Flag; //LOGTIMEFLAG_CATCHOSABNORMALTERMINATION

#ifdef SPG_General_USESpinLock
#if (defined(SPG_General_USELOGTIMETHREADID)||defined(SPG_DEBUGCONFIG))
	SPINLOCK L;
#endif
#endif

	int MaxEvents;
	int MskEvents;
	int NumEvents;
	SPG_LOGEVENT* E;

	int MaxEventTypes;
	int NumEventTypes;
	SPG_EVENTTYPE* T;

	float TimeScale;//time per tick 1/Global.CPUClock
	S_TimerCountType AbsoluteStartTick;

} SPG_LOGTIME;

#define LOGTIMEFLAG_CATCHOSABNORMALTERMINATION 1

#define LT_FLAG_NOCHECK 0
#define LT_FLAG_IDCHECK 1
#define LT_FLAG_ENTRYCOUNTCHECK 2
#define LT_FLAG_FULLCHECK 3

#define LT_Create(S,Flag,uid,Color) S.uid = SPG_LogTimeCreateUID(S.LT,Flag,Color,__FILE__, #S "." #uid ,0)
int SPG_CONV SPG_LogTimeCreateUID(SPG_LOGTIME& LT, int Flag, DWORD Color, char* File, char* Name, char* Descr);

int SPG_CONV SPG_LogTimeInit(SPG_LOGTIME& LT, int MaxEventTypes=128, int MaxEvents=(1<<17), int Flag=0);
void SPG_CONV SPG_LogTimeClear(SPG_LOGTIME& LT);
void SPG_CONV SPG_LogTimeClose(SPG_LOGTIME& LT);

void SPG_CONV SPG_LogTimeThreadIdCheck(SPG_LOGTIME& LT, SPG_LOGEVENT& E, char* S);
void SPG_CONV SPG_LogTimeCountPre(SPG_LOGTIME& LT, int UID, int Flag, char*S);
void SPG_CONV SPG_LogTimeCountPost(SPG_LOGTIME& LT, int UID, int Flag);
#define SPG_LogTimeCreateE(vLT,E) SPG_LOGEVENT& E=(vLT).E[n&(vLT).MskEvents];
#define SPG_LogTimeGetTime(E) {S_TimerCountType MACRO_T; S_Timer_RDTSC(MACRO_T); E.Tick=MACRO_T;}
//#define SPG_LogTimeThreadId(E) { __asm push eax __asm mov eax,fs:[0x00000018] __asm mov eax,DWORD PTR [eax+0x24] __asm mov ebx,E __asm mov [ebx]E.TID,eax __asm pop eax } //MACRO_E est une variable locale créée par SPG_LogTime
#define SPG_LogTimeThreadId(E) E.TID=GetCurrentThreadId()

#if (defined(SPG_General_USELOGTIMETHREADID)||defined(SPG_DEBUGCONFIG))
void SPG_CONV SPG_LogTime(SPG_LOGTIME& LT, int UID, int Flag, int Code);
#else
#define SPG_LogTime(vLT,vUID,vFlag,vCode) { int n=(vLT).NumEvents++; SPG_LogTimeCreateE(vLT,E); SPG_LogTimeGetTime(E); E.UID=vUID; E.Flag=vFlag; E.Code=vCode; }
#endif

void SPG_CONV SPG_LogTimeDraw(SPG_LOGTIME& LT, G_Ecran& E, C_Lib* CL=0);
int SPG_CONV SPG_LogTimeSaveToFile_MultiRow(SPG_LOGTIME& LT, char* FileName);
int SPG_CONV SPG_LogTimeSaveToFile_OneRow(SPG_LOGTIME& LT, char* FileName);

#ifdef SPG_General_USELOGTIME_OSABNORMALTERMINATIONHANDLER
int SPG_CONV SPG_LogTimeSetFilter(SPG_LOGTIME& LT);
#endif

#ifdef DebugWatchLogTime

#define LT_Enter(S,uid,code) SPG_wLogTime(S.LT,S.uid,LT_ENTER,(int)code)
#define LT_Reent(S,uid,code) SPG_wLogTime(S.LT,S.uid,LT_REENT,(int)code)
#define LT_Exit(S,uid,code) SPG_wLogTime(S.LT,S.uid,LT_EXIT,(int)code)

#define LT_EnterI(S,uid,code) SPG_wLogTime(S.LT,S.uid,LT_ENTER|LT_INIT,(int)code)
#define LT_ReentI(S,uid,code) SPG_wLogTime(S.LT,S.uid,LT_REENT|LT_INIT,(int)code)
#define LT_ExitI(S,uid,code) SPG_wLogTime(S.LT,S.uid,LT_EXIT|LT_INIT,(int)code)

#define LT_EnterC(S,uid,code) SPG_wLogTime(S.LT,S.uid,LT_ENTER|LT_CLOSE,(int)code)
#define LT_ReentC(S,uid,code) SPG_wLogTime(S.LT,S.uid,LT_REENT|LT_CLOSE,(int)code)
#define LT_ExitC(S,uid,code) SPG_wLogTime(S.LT,S.uid,LT_EXIT|LT_CLOSE,(int)code)

#else

#define LT_Enter(S,uid,code) SPG_LogTime(S.LT,S.uid,LT_ENTER,(int)code)
#define LT_Reent(S,uid,code) SPG_LogTime(S.LT,S.uid,LT_REENT,(int)code)
#define LT_Exit(S,uid,code) SPG_LogTime(S.LT,S.uid,LT_EXIT,(int)code)

#define LT_EnterI(S,uid,code) SPG_LogTime(S.LT,S.uid,LT_ENTER|LT_INIT,(int)code)
#define LT_ReentI(S,uid,code) SPG_LogTime(S.LT,S.uid,LT_REENT|LT_INIT,(int)code)
#define LT_ExitI(S,uid,code) SPG_LogTime(S.LT,S.uid,LT_EXIT|LT_INIT,(int)code)

#define LT_EnterC(S,uid,code) SPG_LogTime(S.LT,S.uid,LT_ENTER|LT_CLOSE,(int)code)
#define LT_ReentC(S,uid,code) SPG_LogTime(S.LT,S.uid,LT_REENT|LT_CLOSE,(int)code)
#define LT_ExitC(S,uid,code) SPG_LogTime(S.LT,S.uid,LT_EXIT|LT_CLOSE,(int)code)

#endif

#endif

