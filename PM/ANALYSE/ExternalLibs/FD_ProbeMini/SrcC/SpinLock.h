
#ifdef SPG_General_USESpinLock
#include <windows.h>

typedef struct
{
	long State;
	int msTimeout;
#ifdef DebugMem
	char Owner[64];
	DWORD OwnerThreadID;
	S_TIMER STT;
	S_TIMER SLT;
#endif
	CRITICAL_SECTION critsec;
} SPINLOCK;

#define SPL_OK -1
#define SPL_TIMEOUT 0

#ifdef DebugMem

void SPG_CONV SPL_Init(SPINLOCK& L, int msTimeout, const char* Owner);
int SPG_CONV SPL_Enter(SPINLOCK& L, const char* Owner);

#else

#define SPL_Init(L,msTimeout,Owner) SPL_Init_r(L,msTimeout)
#define SPL_Enter(L,Owner) SPL_Enter_r(L)
void SPG_CONV SPL_Init_r(SPINLOCK& L, int msTimeout);
int SPG_CONV SPL_Enter_r(SPINLOCK& L);

#endif

void SPG_CONV SPL_Close(SPINLOCK& L);
void SPG_CONV SPL_Exit(SPINLOCK& L);

#else

#define SPL_OK -1
#define SPL_TIMEOUT 0

#define SPL_Init(L,msTimeout,Owner) 
#define SPL_Enter(L,Owner) SPL_OK
#define SPL_Exit(L)
#define SPL_Close(L)

#endif
