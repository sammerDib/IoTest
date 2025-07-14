
#ifdef SPG_General_USEWLTG

typedef struct
{
	SPG_Window SW;
	C_Lib CL;
	WLOGTIME wt;

	//entrees
	char Name[256];
	int StopFlag;

	//sorties
	DWORD hThread;
	unsigned int ThreadID;
	unsigned int TimerID;
	int Stopped;

} WLTW;

int SPG_CONV WLTW_InitImport(WLTW& w);
int SPG_CONV WLTW_InitLocal(WLTW& w);
int SPG_CONV WLTW_Save(WLTW& w);
int SPG_CONV WLTW_Update(WLTW& w);
void SPG_CONV WLTW_DoMain();


void SPG_CONV WLTW_StartSatLocal(WLTW* &w);
void SPG_CONV WLTW_StopSatLocal(WLTW* &w);

#else

#define WLTW_StartSatLocal(w)
#define WLTW_StopSatLocal(w)

#endif