
struct SPG_LOOPTHREADSTATE;

#define SPG_LOOPTHREADCLBKDEF(f) int(__cdecl f)(void* User, int ThreadOrder)

typedef SPG_LOOPTHREADCLBKDEF(*SPG_LOOPTHREADFCT);

#ifdef SPG_DEBUGCONFIG
void SPG_CONV SPG_SetThreadName(DWORD dwThreadID, char* threadName); //nomme la thread dans l'environnement de developpement
#else
#define SPG_SetThreadName(dwThreadID,threadName)
#endif

int SPG_CONV SPG_LoopThreadGetLogicalProcCount();

#define LOOPTHREADTIMEOUT 10000

SPG_LOOPTHREADSTATE* SPG_CONV SPG_LoopThreadInit(SPG_LOOPTHREADFCT Init, SPG_LOOPTHREADFCT Loop, SPG_LOOPTHREADFCT Close, void* User, int ThreadOrder, int Threaded, int ThWaitms, char* Name);
int SPG_CONV SPG_LoopThreadClose(SPG_LOOPTHREADSTATE* LT);
int SPG_CONV SPG_WaitLoop(SPG_LOOPTHREADSTATE* pLT);
int SPG_CONV SPG_LoopThread(SPG_LOOPTHREADSTATE* LT);

