
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#ifdef SPG_General_USEELISEDI

#include "..\SPG_Includes.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#define sci_UID sci_UID_EmuleLISEEDI
#define sci_NAME sci_NAME_EmuleLISEEDI


// ###################################################

typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	//int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface



// ###################################################

typedef struct
{
	S_TimerCountType Time;
	__int64 Sample;//point courant
	__int64 PeriodPos;//point de départ de la période courante
	int PeriodLength;//en nombre d'échantillons
	float NormalizedPos;
} SIGNAL_POSITION;

static void SPG_CONV SPOS_Create(SIGNAL_POSITION& SPos, int PeriodLength)
{
	SPos.Time=0;
	SPos.Sample=0;
	SPos.PeriodPos=0;
	SPos.PeriodLength=PeriodLength;
	SPos.NormalizedPos=0;
}

static void SPG_FASTCONV SPOS_Increment(SIGNAL_POSITION& SPos)
{
	SPos.Sample++;
	if(SPos.Sample>=SPos.PeriodPos+SPos.PeriodLength) SPos.PeriodPos+=SPos.PeriodLength;
	SPos.NormalizedPos=((float)(SPos.Sample-SPos.PeriodPos))/SPos.PeriodLength;
	return;
}

static void SPG_CONV SPOS_Increment(SIGNAL_POSITION& SPos, int Frequency, S_TimerCountType Time)
{
	S_TimerCountType Elapsed=Time-SPos.Time;
	SPos.Sample+=(Elapsed*Frequency)/Global.CPUClock;
	SPos.Time=Time;
	while(SPos.PeriodPos+SPos.PeriodLength<=SPos.Sample)
	{
		SPos.PeriodPos+=SPos.PeriodLength;
	}
	SPos.NormalizedPos=((double)(SPos.Sample-SPos.PeriodPos))/SPos.PeriodLength;
	return;
}

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire
	float FrequencyHz;
	int BufferSamples;

	float SignalFrequencyHz;
	float PeakWidth;
	int Wavemode;
} SCX_ADDRESS;







// ###################################################

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;

	S_TIMER T;

	S_TimerCountType TRead;
	SIGNAL_POSITION Written;
	SIGNAL_POSITION Sampled;

} SCX_STATE; //parametres d'une connexion en particulier

//efine NIDAQmx_USECallback

// ###################################################



static SCX_CONNEXION* SPG_CONV scxELISEEDIOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAllocZ(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAllocZ(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	S_InitTimer(State.T,"scxELISEEDIOpen");

	SPOS_Create(State.Written,(int)(State.Address.FrequencyHz/State.Address.SignalFrequencyHz));
	SPOS_Create(State.Sampled,(int)(State.Address.FrequencyHz/State.Address.SignalFrequencyHz));

	S_StartTimer(State.T);

	C->Etat=scxOK;
	return C;
}






// ###################################################

static int SPG_CONV scxELISEEDIClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;

	scxFree(C->State);scxFree(C);
	return scxOK;
}

// ###################################################

static int SPG_CONV scxELISEEDIWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;

	return DataLen;
}

// ###################################################

static int SPG_FASTCONV scxLISEEDIGenerateSampleX(SCX_STATE& State, SIGNAL_POSITION& SPos)
{
	double Rampe=2*SPos.NormalizedPos;
	if(Rampe>1) Rampe=2-Rampe;
	int X=(2*Rampe-1)*1023;
	return X;
}

static int SPG_FASTCONV scxLISEEDIGenerateSampleI(SCX_STATE& State, SIGNAL_POSITION& SPos)
{
	double Rampe=2*SPos.NormalizedPos;
	if(Rampe>1) Rampe=2-Rampe;
	float Pos=0.3+0.1*cos(0.1*(SPos.Sample/SPos.PeriodLength));
	float D=fabs(Rampe-Pos);
	int I=-1000;
	float w=State.Address.PeakWidth;
	if(D<w)
	{
		I+=1000*(1+cos(V_PI*D/w));
	}
	return I;
}

static int SPG_CONV scxELISEEDIRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;

	S_GetTimerRunningCount(State.T,State.TRead);
	SPOS_Increment(State.Sampled,State.Address.FrequencyHz,State.TRead);

	
	int SamplesToWrite=State.Sampled.Sample-State.Written.Sample;
	
	CHECK(SamplesToWrite>State.Address.BufferSamples,
				"scxELISEEDIRead: Emulating internal overwrite",
					S_GetTimerRunningCount(State.T,State.TRead);
					SPOS_Increment(State.Sampled,State.Address.FrequencyHz,State.TRead);
					State.Written=State.Sampled;
			return DataLen=0);
	
	DbgCHECK(DataLen&1,"scxELISEEDIRead: Incorrect buffer size");
	SamplesToWrite=V_Min(SamplesToWrite,DataLen/2);//2 = I:2

	short int* const WData=(short int*) Data;
	const float wPos=0.2/State.Written.PeriodLength;
	const float wD=V_PI/State.Address.PeakWidth;
	const float nD=1/State.Address.PeakWidth;
	for(int i=0;i<SamplesToWrite;i++)
	{
		/*
		WData[2*i]=0;
		WData[2*i+1]=-1000;
		*/
		
		float Rampe=2*State.Written.NormalizedPos;
		if(Rampe>1) Rampe=2-Rampe;
		//WData[2*i+1]=(2*Rampe-1)*1023;
		WData[i]=-1000;
		float Pos=0.5+0.0001*wPos*State.Written.Sample*wPos*State.Written.Sample*cos(wPos*State.Written.Sample);
		float D=fabs(Rampe-Pos);
		if(D<State.Address.PeakWidth)
		{
			WData[i]+=1000*(1+cos(wD*D));
			//WData[2*i]+=2000*(1-nD*D);
		}
		
		/*
		int X=scxLISEEDGenerateSampleX(State,State.Written);
		int I=scxLISEEDGenerateSampleI(State,State.Written);
		*WData++=(short int)X;
		*WData++=(short int)I;
		*/
		SPOS_Increment(State.Written);
	}
	return DataLen=SamplesToWrite*2;//short int
}

// ###################################################

static void SPG_CONV scxELISEEDICfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values
		Address->FrequencyHz=10000;
		Address->BufferSamples=128;
		Address->SignalFrequencyHz=100;
		Address->PeakWidth=0.005;
		Address->Wavemode=1;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);
	CFG_StringParam(CFG,"Name",Address->H.Name,0,CFG.FileName[0]?1:0);

	CFG_FloatParam(CFG,"FrequencyHz",&Address->FrequencyHz,0,1);
	CFG_IntParam(CFG,"BufferSamples",&Address->BufferSamples,0,1);
	CFG_FloatParam(CFG,"SignalFrequencyHz",&Address->SignalFrequencyHz,0,1);
	CFG_FloatParam(CFG,"PeakWidth",&Address->PeakWidth,0,1);
	CFG_IntParam(CFG,"Wavemode",&Address->Wavemode,0,1);
	return;
}

// ###################################################

static int SPG_CONV scxELISEEDISetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");
	return 0;
}

// ###################################################

static int SPG_CONV scxELISEEDIGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");
	return 0;
}

// ###################################################

static int SPG_CONV sciELISEEDIDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciELISEEDICreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAllocZ(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETMERGE);
	strcpy(CI->Name,sci_NAME);

	CI->Description="Emulateur";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=0;//spécifique

	CI->scxOpen=scxELISEEDIOpen;
	CI->scxClose=scxELISEEDIClose;
	CI->scxWrite=scxELISEEDIWrite;
	CI->scxRead=scxELISEEDIRead;
	CI->scxCfgAddress=scxELISEEDICfgAddress;
	CI->scxSetParameter=scxELISEEDISetParameter;
	CI->scxGetParameter=scxELISEEDIGetParameter;
	CI->sciDestroyConnexionInterface=sciELISEEDIDestroyConnexionInterface;

	CI->sciParameters=scxTypeAllocZ(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	//sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#else
#pragma SPGMSG(__FILE__,__LINE__,"NoELISEDI : Emule LISE EDI disabled")
#endif
#endif
