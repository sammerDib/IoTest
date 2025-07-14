
#ifdef SPG_General_USELOGTIME
//fdef DebugWatchLogTime

#error "SPG_LogTime obsolete"

#define wLogTimeName "SPG_LogTime"

typedef struct
{
	void* hMapFile;
	void* lpMapAddress;
	SPG_LOGTIME* pLT;
} SPG_WLOGTIME;

//void SPG_CONV SPG_LogTime(SPG_WLOGTIME& wLT, int UID, int Flag, int Code);
#define SPG_wLogTime(wLT,UID,Flag,Code) { if(wLT.pLT) SPG_LogTime(*(wLT.pLT),UID,Flag,Code); }

//efine LT_FLAG_NOCHECK 1
//efine LT_FLAG_NOCHECK 0
int SPG_CONV SPG_LogTimeCreateUID(SPG_WLOGTIME& wLT, int Flag, DWORD Color, char* File, char* Name, char* Descr);
int SPG_CONV SPG_LogTimeInit(SPG_WLOGTIME& wLT, int MaxEventTypes=128, int MaxEvents=(1<<17), int Flag=0);
int SPG_CONV SPG_LogTimeClose(SPG_WLOGTIME& wLT);
void SPG_CONV SPG_LogTimeClear(SPG_WLOGTIME& wLT);
int SPG_CONV SPG_LogTimeSaveToFile_MultiRow(SPG_WLOGTIME& wLT, char* FileName);
int SPG_CONV SPG_LogTimeSaveToFile_OneRow(SPG_WLOGTIME& wLT, char* FileName);

//lse

typedef struct
{
	void* hMapFile;
	void* lpMapAddress;
	SPG_LOGTIME* pLT;//attention les pointeurs internes sont invalides vus du process externe

	DWORD EventTimeout;
	int Etat;
	DWORD CheckedTime;
	DWORD EventTime;
	int EventCount;
	int Trigger;
	int Count;
} SPG_WATCHTIME;

int SPG_CONV SPG_LogTimeWatchdogInit(SPG_WATCHTIME& WT);
int SPG_CONV SPG_LogTimeWatchdogClose(SPG_WATCHTIME& WT);
int SPG_CONV SPG_LogTimeWatchdogDump(SPG_WATCHTIME& WT, char* FileName);

//ndif
#endif

