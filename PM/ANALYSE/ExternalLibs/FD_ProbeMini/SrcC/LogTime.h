
#define LT_STR 32

#ifdef SPG_General_USEWLTG

typedef struct
{
	BYTE Tick[6];
	WORD UID;
//utilisable seulement à l'intérieur du même process:
//	char* Comment;
//	char* Fct;
//	char* Src;
	DWORD TID; //reduire à word
	union
	{
		DWORD UserData;
		DWORD LogStrIndex;
		DWORD MemDelta;
		DWORD StackDelta;
	};
} LT_EVENT;

typedef struct
{
	char Name[LT_STR];
	char Descr[LT_STR];
	char FctName[LT_STR];
	char SourceFile[LT_STR];
	int Line;
	int Flag;
	void* ObjectRef;
} LT_EVENTTYPE;

#define LTG_THREAD_NAME 32
typedef struct
{
	int Status;//1 running 0 terminated (reuse)
	DWORD ThreadId;
	char Name[LTG_THREAD_NAME];
} LT_THREAD_INFOS;

#define LTG_LOGSTR_LEN 512
typedef struct
{
	char LogStr[LTG_LOGSTR_LEN];
} LT_LOG_INFOS;

typedef struct
{
	int szm;
	volatile int NumEvents;
	int MskEvents;//les numeros sont affectés continuement mais seuls les Msk derniers sont valides
	int MaxEvents;
	int szEvents;
#define LT_Events(t)	((LT_EVENT*)(t->Storage))
#define LT_Event(t,i)	((LT_EVENT*)(t->Storage))[i&(t->MskEvents)]
	volatile int NumEventTypes;
	int MaxEventTypes;
	int ofsEventTypes;
	int szEventTypes;
#define LT_EventTypes(t)	((LT_EVENTTYPE*)(t->Storage+t->ofsEventTypes))
#define LT_EventType(t,i)	((LT_EVENTTYPE*)(t->Storage+t->ofsEventTypes))[i]
	volatile int NumThInfos;
	int MaxThInfos;
	int ofsThInfos;
	int szThInfos;
#define LT_ThInfos(t)	((LT_THREAD_INFOS*)(t->Storage+t->ofsThInfos))
#define LT_ThInfo(t,i)	((LT_THREAD_INFOS*)(t->Storage+t->ofsThInfos))[i]
	volatile int NumLogInfos;
	int MskLogInfos;
	int MaxLogInfos;
	int ofsLogInfos;
	int szLogInfos;
#define LT_LogInfos(t)	((LT_LOG_INFOS*)(t->Storage+t->ofsLogInfos))
#define LT_LogInfo(t,i)	((LT_LOG_INFOS*)(t->Storage+t->ofsLogInfos))[i&(t->MskLogInfos)]

	char ExeName[512];
	__int64 TickPerSec;
	__int64 TickOrigin;

	BYTE Storage[];
	//suivi de LTG_EVENT
	//suivi de LTG_EVENTTYPE
	//suivi de LTG_THREAD_INFOS
	//suivi de LTG_LOGSTR_INFOS
} LOGTIME;

extern LOGTIME* global_t; //la reference globale (WLT_SetGlobalRef)
//ne jamais utiliser directement, toujours creer une copie locale du pointeur le temps de le manipuler

typedef struct //utilisé seulement  pour l'import d'un log existant
{
	//char* Name;
	void* hMapFile;
	SPG_CHECKCALLBACK CHECK_Callback;
	void* CHECK_User;
	LOGTIME* t;
} WLOGTIME;

#define LT_FLAG_MSK		0xF000
#define LT_FLAG_SHR		12

#define LT_UID_MSK		0x0FFF
#define LT_UID_NULL 0
#define LT_UID_HIDDEN 1

#define LT_ENTERIDLE_FLAG		0x0000
#define LT_EXITIDLE_FLAG			0x1000
#define LT_ENTER_FLAG			0x2000
#define LT_EXIT_FLAG				0x3000
#define LT_REENT_FLAG			0x4000
#define LT_LOOP_FLAG				0x5000
#define LT_ENTERI_FLAG			0x6000
#define LT_EXITI_FLAG				0x7000
#define LT_ENTERC_FLAG			0x8000
#define LT_EXITC_FLAG				0x9000
#define LT_ALLOC_FLAG			0xA000
#define LT_FREE_FLAG				0xB000
#define LT_STRLOG_FLAG			0xF000

int SPG_CONV WLT_Create(WLOGTIME& wt, int MaxEvents, int MaxThreads, int MaxLog, char* Name, __int64 TickPerSec);
LOGTIME* SPG_CONV LT_CreateWorkingCopy(LOGTIME* t);
int SPG_CONV LT_CreateUID(LOGTIME* pWTLG, const char* Name, const char* Descr, const char* FctName, const char* SourceFile, int Line, int Flag, void* ObjectRef);
int SPG_CONV LT_CreateUID(char* Name, const char* Descr, const char* FctName, const char* SourceFile, int Line, int Flag, void* ObjectRef);

//### LT_Log ###

//en toute rigueur il faudrait un interlockincrement(NumEvents) mais l'attribut volatile force un inc [ecx] qui suffira
//le __asm ecrit les 6 octets de poids faible de RDTSC et le ThreadId

//utilisable seulement à l'intérieur du même process:
//	E->Comment=strComment;																																							\
//	E->Fct=strFct;																																												\
//	E->Src=strSrc;																																												\

#ifdef FDE
#ifdef SPG_DEBUGCONFIG
#define LT_Log(uid,userdata)		{	if(global_t) {	DbgCHECK((uid&LT_FLAG_MSK)==LT_UID_NULL,"LT_Log");				\
	if((uid&LT_UID_MSK)>LT_UID_HIDDEN)	{																															\
	int local_n=global_t->NumEvents++; LT_EVENT* E = LT_Events(global_t) + ( local_n & global_t->MskEvents );		\
	E->UID=uid;																																											\
	E->UserData=userdata;																																						\
	{	__asm push eax __asm push edx __asm push ebx __asm mov ebx,E  __asm _emit 0x0F __asm _emit 0x31					\
		__asm mov  dword ptr [ebx]LT_EVENT.Tick,eax __asm mov  word ptr [ebx]LT_EVENT.Tick+4,dx									\
		__asm mov eax,dword ptr fs:[0x18] __asm mov eax,dword ptr [eax+0x24] __asm mov dword ptr [ebx]LT_EVENT.TID,eax		\
		__asm pop ebx __asm pop edx __asm pop eax	}		}																									\
													}					}
#else
#define LT_Log(uid,userdata)		{	if(global_t) {																									\
	int local_n=global_t->NumEvents++; LT_EVENT* E = LT_Events(global_t) + ( local_n & global_t->MskEvents );		\
	E->UID=uid;																																							\
	E->UserData=userdata;																																		\
	{	__asm push eax __asm push edx __asm push ebx __asm mov ebx,E  __asm _emit 0x0F __asm _emit 0x31	\
		__asm mov  dword ptr [ebx]LT_EVENT.Tick,eax __asm mov  word ptr [ebx]LT_EVENT.Tick+4,dx		\
		__asm mov eax,dword ptr fs:[0x18] __asm mov eax,dword ptr [eax+0x24] __asm mov dword ptr [ebx]LT_EVENT.TID,eax		\
		__asm pop ebx __asm pop edx __asm pop eax	}																							\
													}					}
#endif
#else
#define LT_Log(uid,userdata) printf("%X", userdata)
#endif	//FDE
//### LT_Log ###

void SPG_CONV LT_LogStr(WORD uid, char* LogStr);
void SPG_CONV LT_LogStrClbk(void* User, char* Str);

void SPG_CONV WLT_SetGlobalRef(WLOGTIME& wt);
void SPG_CONV WLT_ClearGlobalRef(WLOGTIME& wt);
/*
#ifdef DebugList
void SPG_CONV WLT_SetGlobalCheckClbkHook(WLOGTIME& wt);
void SPG_CONV WLT_ClearGlobalCheckClbkHook(WLOGTIME& wt);
#endif
*/
void SPG_CONV WLT_Close(WLOGTIME& wt);
void SPG_CONV LT_RegisterThreadName(char* threadName, DWORD ThreadId =0); //associe un nom au thread ID dans une structure en mémoire (voir aussi SPG_SetThreadName)
void SPG_CONV LT_UnRegisterThreadName(char* threadName, DWORD ThreadId = 0 );
const char* SPG_CONV LT_GetThreadName(LOGTIME* t, DWORD ThreadId);
const char* SPG_CONV LT_GetThreadName(DWORD ThreadId = 0 );
int SPG_CONV LT_StoreStr(LOGTIME* t, char* LogStr);
int SPG_CONV WLT_Import(WLOGTIME& wt, char* Name);
void SPG_CONV WLT_Discard(WLOGTIME& wt);
void SPG_CONV LT_Clear(LOGTIME* t);
void SPG_CONV LT_Clear();
int SPG_CONV LT_Save_Binary(LOGTIME* t, char* File);
int SPG_CONV LT_Save_TXT(LOGTIME* t, char* File);
int SPG_CONV LT_Save_TXT(LOGTIME* t, char* Path, char* File);
void SPG_CONV LT_Display(LOGTIME* t, G_Ecran& Ecran, C_Lib& CL);

#else //SPG_General_USEWLTG non defini les macros s annulent

#define WLT_Create(wt, MaxEvents, MaxThreads, MaxLog,  Name, TickPerSec)
#define LT_CreateWorkingCopy(t)
//efine LT_CreateUID(pWTLG, Name, Descr, FctName, SourceFile, Line, Flag, ObjectRef)
#define LT_CreateUID(Name, Descr, FctName, SourceFile, Line, Flag, ObjectRef)

#define LT_Log(uid,UserData)
#define LT_LogStr(uid,LogStr) SPG_List(LogStr)

#define WLT_SetGlobalRef(wt)
#define WLT_ClearGlobalRef(wt)
#define WLT_SetGlobalCheckClbkHook(wt)
#define WLT_ClearGlobalCheckClbkHook(wt)
#define WLT_Close(wt)
#define LT_RegisterThreadName(threadName, ThreadId) //associe un nom au thread ID dans une structure en mémoire (voir aussi SPG_SetThreadName)
//efine LT_GetThreadName(t, ThreadId)
#define LT_GetThreadName(ThreadId) "unknown"
#define LT_StoreStr(t, LogStr)
#define WLT_Import(wt, Name)
#define WLT_Discard(wt)
#define LT_Clear(t)
//efine LT_Clear()
#define LT_Save_Binary(t, File)
//efine LT_Save_TXT(t, File)
#define LT_Save_TXT(t, Path, File)
#define LT_Display(t, Ecran, CL)

#endif


//les fonctions de log par uid telles qu'utilisées dans le code

#define LT_Enter(uid,UserData)		LT_Log(uid|LT_ENTER_FLAG,UserData)
#define LT_Reent(uid,UserData)		LT_Log(uid|LT_REENT_FLAG,UserData)
#define LT_Loop(uid,UserData)		LT_Log(uid|LT_LOOP_FLAG,UserData)
#define LT_Exit(uid,UserData)			LT_Log(uid|LT_EXIT_FLAG,UserData)

#define LT_EnterI(uid,UserData)		LT_Log(uid|LT_ENTERI_FLAG,UserData)
#define LT_ReentI(uid,UserData)		LT_Log(uid|LT_REENTI_FLAG,UserData)
#define LT_LoopI(uid,UserData)		LT_Log(uid|LT_LOOPI_FLAG,UserData)
#define LT_ExitI(uid,UserData)			LT_Log(uid|LT_EXITI_FLAG,UserData)

#define LT_EnterC(uid,UserData)		LT_Log(uid|LT_ENTERC_FLAG,UserData)
#define LT_ReentC(uid,UserData)	LT_Log(uid|LT_REENTC_FLAG,UserData)
#define LT_LoopC(uid,UserData)		LT_Log(uid|LT_LOOPC_FLAG,UserData)
#define LT_ExitC(uid,UserData)		LT_Log(uid|LT_EXITC_FLAG,UserData)

#define LT_Save(Path,txtFile)			LT_Save_TXT(global_t,Path,txtFile)
#define LT_SaveAndClear(Path,txtFile)		{ LT_Save_TXT(global_t,Path,txtFile); LT_Clear(global_t); }



//wrapping des fonctions ancienne forme

#ifdef SPG_General_USEWLTG
#define LTG_Create(S,Flag,uid,Color) S.uid = LT_CreateUID(#S "." #uid,0,__FUNCTION__,__FILE__,__LINE__,0,(void*)&S)
#define LTG_CreateDescr(S,Flag,uid,Color,descr) S.uid = LT_CreateUID(#S "." #uid,descr,__FUNCTION__,__FILE__,__LINE__,0,(void*)&S)
#else
#define LTG_Create(S,Flag,uid,Color)
#define LTG_CreateDescr(S,Flag,uid,Color,descr)
#endif

#define LTG_Enter(S,uid,UserData) LT_Enter(S.uid,UserData)
#define LTG_Reent(S,uid,UserData) LT_Reent(S.uid,UserData)
#define LTG_Exit(S,uid,UserData) LT_Exit(S.uid,UserData)

#define LTG_vEnter(uid,UserData) LT_Enter(uid,UserData)
#define LTG_vReent(uid,UserData) LT_Reent(uid,UserData)
#define LTG_vExit(uid,UserData) LT_Exit(uid,UserData)
#define LTG_vExit_nowarning(uid,UserData) LT_Exit(uid,UserData)

#define LTG_EnterEH(S,uid,UserData) TRY_BEGIN LT_Enter(S.uid,UserData)
#define LTG_ExitEH(S,uid,UserData) LT_Exit(S.uid,UserData); TRY_ENDG(#S "." #uid)
#define LTG_ExitEHNZ(S,uid,UserData) LT_Exit(S.uid,UserData); return -1; TRY_ENDGRZ(#S "." #uid)
#define LTG_ExitEHV(S,uid,UserData,v) LT_Exit(S.uid,UserData); return v; TRY_ENDGRZ(#S "." #uid)

#define LTG_EnterI(S,uid,UserData) LT_EnterI(S.uid,UserData)
#define LTG_ReentI(S,uid,UserData) LT_ReentI(S.uid,UserData)
#define LTG_ExitI(S,uid,UserData) LT_ExitI(S.uid,UserData)

#define LTG_EnterC(S,uid,UserData) LT_EnterC(S.uid,UserData)
#define LTG_ReentC(S,uid,UserData) LT_ReentC(S.uid,UserData)
#define LTG_ExitC(S,uid,UserData) LT_ExitC(S.uid,UserData)

#define G_LogTimeEV(Type,Valeur) LT_Enter(Global.Type,__LINE__)
#define G_LogTimeEN(Type) LT_Enter(Global.Type,__LINE__)
#define G_LogTimeREN(Type) LT_Reent(Global.Type,__LINE__)
#define G_LogTimeRV(Type,Valeur) LT_Exit(Global.Type,__LINE__)
#define G_LogTimeRN(Type) LT_Exit(Global.Type,__LINE__)

#define G_LogTimeENI(Type) LT_EnterI(Global.Type,__LINE__)
#define G_LogTimeEVI(Type,Valeur) LT_EnterI(Global.Type,Valeur)
#define G_LogTimeRNI(Type) LT_ExitI(Global.Type,__LINE__)
#define G_LogTimeRVI(Type,Valeur) LT_ExitI(Global.Type,Valeur)
#define G_LogTimeENC(Type) LT_EnterC(Global.Type,__LINE__)
#define G_LogTimeEVC(Type,Valeur) LT_EnterC(Global.Type,__LINE__)
#define G_LogTimeRNC(Type) LT_ExitC(Global.Type,__LINE__)
#define G_LogTimeRVC(Type,Valeur) LT_ExitC(Global.Type,Valeur)

#define SPG_LogTimeSaveToFile_OneRow(wt,FileName) {LT_Save_TXT(wt.t,FileName); }

#define SPG_RegisterGlobalThreadName LT_RegisterThreadName //associe un nom au thread ID dans une structure en mémoire (voir aussi SPG_SetThreadName)
#define SPG_GetGlobalThreadName LT_GetThreadName
