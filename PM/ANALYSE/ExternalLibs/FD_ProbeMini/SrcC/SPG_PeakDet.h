#ifdef INC_SPG_PeakDet_INC
#error SPG_PeakDet Included twice
#endif
#define	INC_SPG_PeakDet_INC

typedef struct
{
	int Etat;
	float LowTimeCste;
	float PeakTimeCste;

	float NoiseLow;
	float NoiseHigh;

	float SignalLow;//=PEAKDET_SIGNAL(PD)
	float SignalHigh;

	float Weight;
} SPG_PEAKDET;		 

#define PEAKDET_OK 1
#define PEAKDET_ON 2

#define PEAKDET_SNR 3

//#define PEAKDET_SIGNAL(PD) PD.NoiseHigh+(PD.NoiseHigh-PD.NoiseLow)/PD.PeakTimeCste
#define PEAKDET_SIGNALLOW(PD) ((1+PEAKDET_SNR)*PD.NoiseHigh-PEAKDET_SNR*PD.NoiseLow)
#define PEAKDET_SIGNALHIGH(PD) ((1+2*PEAKDET_SNR)*PD.NoiseHigh-2*PEAKDET_SNR*PD.NoiseLow)
#define PEAKDET_ISSIGNAL(PD,V) (V>PD.SignalLow)
#define PEAKDET_WEIGHT(PD,V) (V-PD.SignalLow)/(PD.SignalHigh-PD.SignalLow)

int SPG_CONV PeakDet_Init(SPG_PEAKDET& PD, float LowTimeCste=0.002f, float PeakTimeCste=0.5f);
void SPG_CONV PeakDet_Close(SPG_PEAKDET& PD);
int SPG_CONV PeakDet_Update(SPG_PEAKDET& PD, float Signal);
