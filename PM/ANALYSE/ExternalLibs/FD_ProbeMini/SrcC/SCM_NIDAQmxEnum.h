

#define NISTRLEN 32
#define NIMAXDEV 16

typedef struct
{
	char Name[NISTRLEN];
	char Type[NISTRLEN];
	DWORD Serial;
} SCM_DEVICENAME;

typedef struct
{
	int n;
	SCM_DEVICENAME D[NIMAXDEV];
} SCM_DEVICELIST;

int SPG_CONV SCM_NIDAQmxList(SCM_DEVICELIST& L);
int SPG_CONV SCM_NIDAQmxGetNthOfType(SCM_DEVICELIST& L, int Nth, char* DeviceType);//Nth à base zero
int SPG_CONV SCM_NIDAQmxIsInList(SCM_DEVICELIST& L, char* DeviceName);
int SPG_CONV SCM_NIDAQmxListSave(char* FileName, SCM_DEVICELIST& L);
int SPG_CONV SCM_NIDAQmxListDisplay(SCM_DEVICELIST& L);

int SPG_CONV SCM_NIDAQmxGetNumChan(char* chan);
int SPG_CONV SCM_NIDAQmxGetFirstLineNr(char* DigLine);
int SPG_CONV SCM_NIDAQmxGetModeFlag(char* chan, char* clockName, double FrequencyHz, DWORD& dwMode);

