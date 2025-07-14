

#include "SPG_General.h"

#ifdef SPG_General_USEPEAKDET

#include "SPG_Includes.h"

#include <string.h>

//#define PEAKDET_SIGNAL(PD) PD.NoiseHigh+(PD.NoiseHigh-PD.NoiseLow)/PD.PeakTimeCste
//#define PEAKDET_ISSIGNAL(PD,V) (V>PD.SignalLow)
//#define PEAKDET_WEIGHT(PD,V) (V-PD.SignalLow)/(PD.SignalHigh-PD.SignalLow)

int SPG_CONV PeakDet_Init(SPG_PEAKDET& PD, float LowTimeCste, float PeakTimeCste)
{
	memset(&PD,0,sizeof(SPG_PEAKDET));
	PD.LowTimeCste=LowTimeCste;
	PD.PeakTimeCste=PeakTimeCste;
	return PD.Etat=PEAKDET_OK;
}

void SPG_CONV PeakDet_Close(SPG_PEAKDET& PD)
{
	memset(&PD,0,sizeof(SPG_PEAKDET));
	return;
}

int SPG_CONV PeakDet_Update(SPG_PEAKDET& PD, float Signal)
{
	if(Signal>PD.SignalLow)
	{
		if(Signal>PD.SignalHigh)
		{
			PD.SignalHigh=PD.SignalHigh*(1-PD.PeakTimeCste)+Signal*PD.PeakTimeCste;
		}
		else
		{
			PD.SignalHigh=PD.SignalHigh*(1-PD.LowTimeCste)+Signal*PD.LowTimeCste;
		}
		PD.Weight=(Signal-PD.SignalLow)/(PD.SignalHigh-PD.SignalLow);
		PD.NoiseHigh=PD.NoiseHigh*(1-PD.LowTimeCste)+Signal*PD.LowTimeCste;
		PD.NoiseLow=PD.NoiseLow*(1-0.5f*PD.LowTimeCste)+Signal*0.5f*PD.LowTimeCste;
		PD.Etat|=PEAKDET_ON;
	}
	else
	{
		if(Signal>PD.NoiseHigh)
		{
			PD.SignalHigh=PD.SignalHigh*(1-PD.LowTimeCste)+Signal*PD.LowTimeCste;
		}
		if(Signal<PD.NoiseLow)
		{
			PD.NoiseLow=PD.NoiseLow*(1-PD.PeakTimeCste)+Signal*PD.PeakTimeCste;
			PD.NoiseHigh=PD.NoiseHigh*(1-0.5f*PD.LowTimeCste)+Signal*0.5f*PD.LowTimeCste;
		}
		else
		{
			PD.NoiseLow=PD.NoiseLow*(1-0.25f*PD.LowTimeCste)+Signal*0.25f*PD.LowTimeCste;
			PD.NoiseHigh=PD.NoiseHigh*(1-2.0f*PD.LowTimeCste)+Signal*2.0f*PD.LowTimeCste;
		}
		PD.Weight=(Signal-PD.SignalLow)/(PD.NoiseHigh-PD.NoiseLow);
		PD.Etat&=~PEAKDET_ON;
	}
	PD.SignalLow=PEAKDET_SIGNALLOW(PD);
	PD.SignalHigh=V_Max(PD.SignalHigh,PEAKDET_SIGNALHIGH(PD));
	return PD.Etat&PEAKDET_ON;
}

#endif
