
#ifdef SPG_General_USEConsole

#define MaxConsole 4096

typedef struct
{
	int Etat;
	G_Ecran* Ecran;
	C_Lib* CarLib;
	void* F;
#ifdef SPG_General_USETimer
	S_TIMER UpdateTimer;
#endif
	/*
	int MaxTextLen;
	int MaxTextLines;
	*/
	/*
	int LineOutDelay;
	int LineAddTime;
	*/
#ifdef DebugMem
	int SPG_STACK_SPY_BeforeCTest; //0xAA55AA55
#endif
	char CText[MaxConsole];
#ifdef DebugMem
	int SPG_STACK_SPY_AfterCTest; //0xAA55AA55
#endif
} SPG_Console;

#define CONSOLE_OK 1
#define CONSOLE_HOOKFILE 2
#define CONSOLE_HOOKCHECK 4
#define CONSOLE_AUTOUPDATE 8
#define CONSOLE_WITHDATE 16

SPG_Console* SPG_CONV Console_Create(G_Ecran* E, C_Lib* CL, int AutoUpdate=0);
SPG_Console* SPG_CONV FileConsole_Create(G_Ecran* E, C_Lib* CL, int AutoUpdate=0, char* Path=0, char* CslName="Console.txt", int FlushFile=0, int Flag=CONSOLE_HOOKCHECK|CONSOLE_WITHDATE);
int SPG_CONV Console_Create(SPG_Console& C, G_Ecran* E, C_Lib* CL, int AutoUpdate=0);
int SPG_CONV FileConsole_Create(SPG_Console& C, G_Ecran* E, C_Lib* CL, int AutoUpdate=0, char* Path=0, char* CslName="Console.txt", int FlushFile=0, int Flag=CONSOLE_HOOKCHECK|CONSOLE_WITHDATE);
//FDE void SPG_CONV Console_Add(SPG_Console* C, const char*T);
void SPG_CONV Console_Add(SPG_Console& C, const char*T);
void SPG_CONV Console_AddOnSameLine(SPG_Console* C, char*T);
void SPG_CONV Console_AddOnSameLine(SPG_Console& C, char*T);
void SPG_CONV Console_DeleteALine(SPG_Console* C);
void SPG_CONV Console_DeleteALine(SPG_Console& C);
void SPG_CONV Console_Update(SPG_Console* C);
void SPG_CONV Console_Update(SPG_Console& C);
void SPG_CONV Console_Clear(SPG_Console* C);
void SPG_CONV Console_Clear(SPG_Console& C);
void SPG_CONV Console_Close(SPG_Console* C);
void SPG_CONV Console_Close(SPG_Console& C);

#define Console_Init Console_Create
#define FileConsole_Init FileConsole_Create
#endif


