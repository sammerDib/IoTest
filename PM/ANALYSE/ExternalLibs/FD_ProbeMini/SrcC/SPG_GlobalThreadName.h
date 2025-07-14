
#ifdef SPG_General_USEGLOBALTHREADNAME

#define THREAD_DBG_NAME 32

typedef struct
{
	DWORD ThreadId;
	char Name[THREAD_DBG_NAME];
} THREAD_DBG_INFO_ITEM;

#define THREAD_DBG_NUMITEMS 64

typedef struct
{
	int MaxItems; //=128;
	int NumItems; //
	THREAD_DBG_INFO_ITEM tdn[THREAD_DBG_NUMITEMS];
} THREAD_DBG_INFOS;

void SPG_CONV SPG_RegisterGlobalThreadName(char* threadName, DWORD ThreadId = 0 ); //associe un nom au thread ID dans une structure en mémoire (voir aussi SPG_SetThreadName)
void SPG_CONV SPG_UnRegisterGlobalThreadName(char* threadName, DWORD ThreadId = 0 );
char* SPG_CONV SPG_GetGlobalThreadName(DWORD ThreadId = 0 );

#endif
