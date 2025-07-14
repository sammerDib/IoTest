
#include "SPG_General.h"

#ifdef SPG_General_USESON
#ifdef SPG_General_USEWindows

#include "V_General.h"
#include "SPG_List.h"

#include <stdio.h>
#include <windows.h>
#include "Config\SPG_Warning.h"
#include <mmsystem.h>
#define DIRECTSOUND_VERSION 0x0500
#include <dsound.h>

#define MaxSndBuffer 16
#define IsValid(Num) ((Num>=0)&&(Num<MaxSndBuffer))
#define IsStop 0
#define IsPlaying 1

#include "SPG_Son.h"

typedef struct
{
	SHORT wFormatTag;
	SHORT nChannels;
	LONG SamplesPerSec;
	LONG nAvgBytesPerSec;
	SHORT nBlockAlign;
	SHORT wBitsPerSample;
} xWAVEFORMAT;

typedef struct
{
	LONG SRIFF;
	LONG TM8;
	LONG SWAVE;
	LONG SFMT;
	LONG TEnt;
	xWAVEFORMAT WVF;
	LONG SDATA;
	LONG LGData;
} ENTETEWAV;



LPDIRECTSOUND	lpDS;
LPDIRECTSOUNDBUFFER		lpDSPrimary;
LPDIRECTSOUNDBUFFER		lpDSBuffer[MaxSndBuffer];
int EtatSon[MaxSndBuffer];
WAVEFORMATEX	wfxFormatPrimary;
/*
DSBUFFERDESC	dsdesc;
WAVEFORMATEX	wfxFormat;
*/
/*
void *			lpBuff;
DWORD			ROct;
void *			lpBuff2;
DWORD			ROct2;
*/

int SPG_CONV InitSon(int FWnd,int Frequency,int BitsPerSample,int Channels)//char* FileName)
{
	lpDS=0;
	lpDSPrimary=0;
	CoCreateInstance(CLSID_DirectSound,
	NULL, CLSCTX_INPROC_SERVER, IID_IDirectSound, (void**)&lpDS);
	if (lpDS==0) return 0;

	if (lpDS->Initialize(NULL)!=DS_OK) return 0;

	lpDS->SetCooperativeLevel((HWND)FWnd,DSSCL_PRIORITY);//DSSCL_PRIORITY);

	DSBUFFERDESC	dsdesc;
	memset(&dsdesc,0,sizeof(DSBUFFERDESC));
	dsdesc.dwSize=sizeof(DSBUFFERDESC);
	dsdesc.dwFlags=DSBCAPS_PRIMARYBUFFER;
	dsdesc.lpwfxFormat=0;//&wfxFormatPrimary;

	wfxFormatPrimary.wFormatTag=WAVE_FORMAT_PCM;
	wfxFormatPrimary.nChannels=Channels;
	wfxFormatPrimary.nSamplesPerSec=Frequency;
	wfxFormatPrimary.nAvgBytesPerSec=Frequency*(BitsPerSample/8)*Channels;
	wfxFormatPrimary.nBlockAlign=(BitsPerSample/8)*Channels;
	wfxFormatPrimary.wBitsPerSample=BitsPerSample;
	wfxFormatPrimary.cbSize=0;

	
	lpDS->CreateSoundBuffer(&dsdesc,&lpDSPrimary,0);
	if (lpDSPrimary==0) return 0;


	lpDSPrimary->SetFormat(&wfxFormatPrimary);
	
/*
// The function completed successfully
int v1=DS_OK;
// The function completed successfully, but we had to substitute the 3D algorithm
int v2=DS_NO_VIRTUALIZATION;
// The call failed because resources (such as a priority level)
// were already being used by another caller
int v3=DSERR_ALLOCATED;
// The control (vol,pan,etc.) requested by the caller is not available
int v4=DSERR_CONTROLUNAVAIL;
// An invalid parameter was passed to the returning function
int v5=DSERR_INVALIDPARAM;
// This call is not valid for the current state of this object
int v6=DSERR_INVALIDCALL;
// An undetermined error occured inside the DirectSound subsystem
int v7=DSERR_GENERIC;
// The caller does not have the priority level required for the function to
// succeed
int v8=DSERR_PRIOLEVELNEEDED;
// Not enough free memory is available to complete the operation
int v9=DSERR_OUTOFMEMORY;
// The specified WAVE format is not supported
int v10=DSERR_BADFORMAT;
// The function called is not supported at this time
int v11=DSERR_UNSUPPORTED;
// No sound driver is available for use
int v12=DSERR_NODRIVER;
// This object is already initialized
int v13=DSERR_ALREADYINITIALIZED;
// This object does not support aggregation
int v14=DSERR_NOAGGREGATION;
// The buffer memory has been lost, and must be restored
int v15=DSERR_BUFFERLOST;
// Another app has a higher priority level, preventing this call from
// succeeding
int v16=DSERR_OTHERAPPHASPRIO;
// This object has not been initialized
int v17=DSERR_UNINITIALIZED;
// The requested COM interface is not available
int v18=DSERR_NOINTERFACE;
// Access is denied
int v19=DSERR_ACCESSDENIED;
*/
	/*
	wfxFormat.wBitsPerSample=0;
	wfxFormat.nChannels=0;
	wfxFormat.nBlockAlign=0;
	wfxFormat.nSamplesPerSec=0;
	char Infos[256];
	lpDSPrimary->GetFormat(&wfxFormat,sizeof(WAVEFORMATEX),0);
	wsprintf(Infos,"Bits:%d   Blocs:%d   Channels:%d   Frequency:%d\0",wfxFormat.wBitsPerSample,wfxFormat.nBlockAlign,wfxFormat.nChannels,wfxFormat.nSamplesPerSec);
	MessageBox(0,Infos,Infos,MB_OK);
	*/
	return -1;
}

int SPG_CONV CreateSBuffer(int Size,int Frequency,int BitsPerSample,int Channels)
{
	int Num;
	for (Num=0;Num<MaxSndBuffer;Num++)
	{
		if (lpDSBuffer[Num]==0) break;
	}
	if (Num==MaxSndBuffer) return -1;

	DSBUFFERDESC	dsdesc;
	WAVEFORMATEX	wfxFormat;
	memset(&dsdesc,0,sizeof(DSBUFFERDESC));
	dsdesc.dwSize=sizeof(DSBUFFERDESC);
	dsdesc.dwFlags=DSBCAPS_GLOBALFOCUS|DSBCAPS_GETCURRENTPOSITION2|DSBCAPS_CTRLFREQUENCY|DSBCAPS_CTRLVOLUME;
	dsdesc.dwBufferBytes=Size;
	dsdesc.lpwfxFormat=&wfxFormat;
	dsdesc.lpwfxFormat->wFormatTag=WAVE_FORMAT_PCM;
	dsdesc.lpwfxFormat->wBitsPerSample=(WORD)BitsPerSample;
	dsdesc.lpwfxFormat->nChannels=(WORD)Channels;
	dsdesc.lpwfxFormat->nSamplesPerSec=Frequency;
	dsdesc.lpwfxFormat->nBlockAlign=(WORD)(Channels*BitsPerSample/8);
	dsdesc.lpwfxFormat->nAvgBytesPerSec=Frequency*dsdesc.lpwfxFormat->nBlockAlign;
	dsdesc.lpwfxFormat->cbSize=0;
	lpDS->CreateSoundBuffer(&dsdesc,&lpDSBuffer[Num],0);
	if (lpDSBuffer[Num]==0) return -1;
	EtatSon[Num]=0;
	return Num;
}

int SPG_CONV CreateWaveBuffer(char* FileName)
{
	int Num;
	FILE *wfich;
	ENTETEWAV EntWav;
	wfich=fopen(FileName,"rb");
	CHECKTWO(wfich==0,"CreateWaveBuffer: Impossible d'ouvrir le fichier",FileName,return -1;)

	if(fread(&EntWav,sizeof(ENTETEWAV),1,wfich)==0)
	{
		return -1;
	}
	
	Num=CreateSBuffer(EntWav.LGData,EntWav.WVF.SamplesPerSec,EntWav.WVF.wBitsPerSample,EntWav.WVF.nChannels);//EntWav.WVF.Groups*8/EntWav.WVF.Tech);
	if (Num==-1) return -1;

	void *lpBuff;
	DWORD ROct;

	lpDSBuffer[Num]->Lock(0,EntWav.LGData,&lpBuff,&ROct,0,0,0);
	fread(lpBuff,EntWav.LGData,1,wfich);
	lpDSBuffer[Num]->Unlock(lpBuff,ROct,0,0);
	fclose(wfich);
	return Num;
}

void SPG_CONV ReleaseSBuffer(int Num)
{
	if (IsValid(Num))
	{
	if (lpDSBuffer[Num]!=0)
	{
		lpDSBuffer[Num]->Stop();
		lpDSBuffer[Num]->Release();
		lpDSBuffer[Num]=0;
	}
	}
	return;
}

int SPG_CONV CopyToSBuffer(int Num,BYTE * Data,int PosB,int L)
{
	if (IsValid(Num))
	{
	void *lpBuff;
	DWORD ROct;

	lpBuff=0;
	lpDSBuffer[Num]->Lock(PosB,L,&lpBuff,&ROct,0,0,0);
	if (lpBuff!=0)
	{
		memcpy(lpBuff,Data,L);
		lpDSBuffer[Num]->Unlock(lpBuff,L,0,0);
		return -1;
	}
	}
	return 0;
}


int SPG_CONV GetPlayPos(int Num)
{
	DWORD PPos,WPos;
	if (IsValid(Num))
	{
	lpDSBuffer[Num]->GetCurrentPosition(&PPos,&WPos);
	return (int)PPos;
	}
	else
	{
	return 0;
	}
}


void SPG_CONV FinSon(void)
{
	for (int Num=0;Num<MaxSndBuffer;Num++)
	{
		ReleaseSBuffer(Num);
	}
	if (lpDSPrimary!=0)
		lpDSPrimary->Release();
	lpDSPrimary=0;
	if (lpDS!=0)
		lpDS->Release();
	lpDS=0;
	return;
}


void SPG_CONV PlaySon(int Num,int Looping)
{
	if (IsValid(Num))
	{
	if (lpDSBuffer[Num]!=0)
	{
		if (!EtatSon[Num]&IsPlaying)
		{
		if (Looping!=0)
		{
			EtatSon[Num]=IsPlaying;
			lpDSBuffer[Num]->Play(0,0,DSBPLAY_LOOPING);
		}
		else
			lpDSBuffer[Num]->Play(0,0,0);
		}
	}
	}
	return;
}

void SPG_CONV StopSon(int Num)
{
	if (IsValid(Num))
	{
		if (EtatSon[Num]&IsPlaying)
		{
			EtatSon[Num]=0;
	if (lpDSBuffer[Num]!=0)
		lpDSBuffer[Num]->Stop();
		}
	}
	return;
}

void SPG_CONV RestoreSon()
{
	if (lpDSPrimary!=0)
		lpDSPrimary->Restore();
for(int Num=0;Num<MaxSndBuffer;Num++)
{
	if (lpDSBuffer[Num]!=0)
		lpDSBuffer[Num]->Restore();
}
	if (lpDSPrimary!=0)
		lpDSPrimary->SetFormat(&wfxFormatPrimary);
	return;
}

void SPG_CONV ResetSon(int Num)
{
	if (IsValid(Num))
	{
	if (lpDSBuffer[Num]!=0)
		lpDSBuffer[Num]->SetCurrentPosition(0);
	}
	return;
}

void SPG_CONV FreqSon(int Num,float Freq)
{
	if (IsValid(Num))
	{
	if (lpDSBuffer[Num]!=0)
		if(Freq*wfxFormatPrimary.nSamplesPerSec>100)
		{
		lpDSBuffer[Num]->SetFrequency(Freq*wfxFormatPrimary.nSamplesPerSec);
		}
		else
		{
		lpDSBuffer[Num]->SetFrequency(100);
		}
	}
	return;
}

void SPG_CONV VolumeSon(int Num,float Volume)
{
	if (IsValid(Num))
	{
	if (lpDSBuffer[Num]!=0)
		if (Volume>0)
		{
		lpDSBuffer[Num]->SetVolume(1000*log10(Volume));
		}
		else
		{
		lpDSBuffer[Num]->SetVolume(-10000);
		}
	}
	return;
}

#endif
#endif

