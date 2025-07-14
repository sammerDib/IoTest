
#ifdef DebugMem

#define SPGMEMNAME 128

#define MS_FLAG_FREE 0
#define MS_FLAG_ALLOC 1
#define MS_FLAG_FROZEN 2
#define MS_FLAG_MSKNOTFREE 3
#define MS_FLAG_DEBUGBREAKONFREE 4

#define SPGMEMCODE 0x55
#define SPGDWMEMCODE 0x55555555
#define SPGCHECKCODE 0xAA55AA55

typedef struct
{
	int MemStructCheckLower;
	int Flag;
	int SkipCheck;//pour éviter les répétitions de messages
	int AllocSequence;
	int FreeSequence;
	char Name[SPGMEMNAME];
	int Padding;
	BYTE* FullBlock;
	int FullLen;
	BYTE* Block;
	int Len;
	int MemStructCheckHigher;
} SPG_MEM_RECORD;

typedef struct
{
	int MemStructCheckLower;
	int Freeze;
	int MaxBlock;
	int NumBlock;

#ifdef SPG_General_USESpinLock
	SPINLOCK L;
#else
#endif

	SPG_MEM_RECORD* M;
	int Sequence;

	int BreakOnAllocSequence;

	int TotalAllocated;
	int TotalFreed;
	int NumAllocated;
	int NumFreed;
	int MemStructCheckHigher;
} SPG_MEM_STATE;

//extern SPG_MEM_STATE MODULE_MS;


#endif
