
//#include "..\SrcC\SPG.h"

typedef enum
{
	eduiUninitialized,
	eduiOK,
	eduiRunning
} EDUIStat;

#define UIMaxThick 16

typedef struct
{
	EDUIStat Status;
#ifdef FDE
	SPG_Window SW;
	B_Lib BL;
	B_Lib BLCheckButton;
	C_Lib CL;

	G_Ecran EGraph;
	G_Ecran EGraphChannel2;
	G_Ecran EButtons;
	G_Ecran ECheckButton;
	G_Ecran EConsole;

	SPG_Console Console;
#endif
	int FrameNr;

	S_TIMER D;//Delay between updates

	int WavLen;
	double* Wav;
	double* WavChannel2;
	double* SaturationThreshold;

	Cut CWav;
	Cut CWavChannel2;
	Cut CSaturationThreshold;

	int BVolts;
	float fVolts;
	float pfVolts;

	double Thickness[UIMaxThick];
	double Quality;

} LISE_ED_SETTINGS;

