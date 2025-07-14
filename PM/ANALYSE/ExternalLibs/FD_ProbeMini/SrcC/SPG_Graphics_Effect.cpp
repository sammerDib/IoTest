
#include "SPG_General.h"

#ifdef SPG_General_USEGraphics
#ifdef SPG_General_USEGEFFECT

#define FastBlur
//#define LongBlur

#include "SPG_Includes.h"

#include <string.h>

int SPG_CONV G_MB_Init(G_MotionBlur& MB, G_Ecran& EDest, int NumBlur)
{
	memset(&MB,0,sizeof(MB));
	CHECK(EDest.Etat==0,"G_MB_Init: Ecran nul",return 0);
	MB.NumBlur=V_Max(NumBlur,1);
	G_InitMemoryEcran(MB.EMegaPitch,EDest.POCT,EDest.SizeX*MB.NumBlur,EDest.SizeY);
	SPG_SetMemName(MB.EMegaPitch.MECR,"G_MotionBlur");
	MB.EDraw=MB.EMegaPitch;
	MB.EDraw.SizeX=EDest.SizeX;
	MB.EDest=&EDest;
	int Pan=MB.EDraw.POCT*MB.EDraw.SizeX;
	for(int n=0;n<MB.NumBlur;n++)
	{
		MB.EDraw.MECR=MB.EMegaPitch.MECR+n*Pan;
		G_Copy(MB.EDraw,EDest);
	}
	MB.EDraw.MECR=MB.EMegaPitch.MECR;
	MB.CurrentDraw=0;
	return MB.Etat=-1;
}

void SPG_CONV G_MB_Close(G_MotionBlur& MB)
{
	G_CloseEcran(MB.EMegaPitch);
	memset(&MB,0,sizeof(MB));
	return;
}

#ifdef FastBlur

void SPG_CONV G_MB_Render(G_MotionBlur& MB)
{
	int Pan=MB.EDraw.POCT*MB.EDraw.SizeX;
	int Div=65536/MB.NumBlur;
	BYTE* MECRL=MB.EMegaPitch.MECR;
	BYTE* MECRDL=MB.EDest->MECR;

	switch(MB.NumBlur)
	{
	case 4:
	{
		for(int y=0;y<MB.EMegaPitch.SizeY;y++)
		{
			BYTE*MECRP=MECRL;
			BYTE*MECRDP=MECRDL;
			for(int x=0;x<(Pan&0xFFFFFC);x+=4)
			{
				DWORD Sum0=0;
				DWORD Sum2=0;
				for(int n=0;n<4;n++)
				{
					DWORD P=*(DWORD*)(MECRP+n*Pan);
					Sum0+=(P&0x00FF00FF);
					Sum2+=((P>>8)&0x00FF00FF);
				}
				*(DWORD*)MECRDP=((Sum0&0x03FC03FC)>>2)+((Sum2&0x03FC03FC)<<6);
				MECRP+=4;
				MECRDP+=4;
			}
			MECRL+=MB.EMegaPitch.Pitch;
			MECRDL+=MB.EDest->Pitch;
		}
		break;
	}
	case 8:
	{
		for(int y=0;y<MB.EMegaPitch.SizeY;y++)
		{
			BYTE*MECRP=MECRL;
			BYTE*MECRDP=MECRDL;
			for(int x=0;x<(Pan&0xFFFFFC);x+=4)
			{
				DWORD Sum0=0;
				DWORD Sum2=0;
				for(int n=0;n<8;n++)
				{
					DWORD P0=*(DWORD*)(MECRP+n*Pan);
					Sum0+=(P0&0x00FF00FF);
					Sum2+=((P0>>8)&0x00FF00FF);
				}
				*(DWORD*)MECRDP=((Sum0&0x07F807F8)>>3)+((Sum2&0x07F807F8)<<5);
				MECRP+=4;
				MECRDP+=4;
			}
			MECRL+=MB.EMegaPitch.Pitch;
			MECRDL+=MB.EDest->Pitch;
		}
		break;
	}
	default:
	{
		for(int y=0;y<MB.EMegaPitch.SizeY;y++)
		{
			BYTE*MECRP=MECRL;
			BYTE*MECRDP=MECRDL;
			for(int x=0;x<(Pan&0xFFFFFC);x+=4)
			{
				WORD Sum0=0;
				WORD Sum1=0;
				WORD Sum2=0;
				WORD Sum3=0;
				for(int n=0;n<MB.NumBlur;n++)
				{
					Sum0+=MECRP[n*Pan];
					Sum1+=MECRP[n*Pan+1];
					Sum2+=MECRP[n*Pan+2];
					Sum3+=MECRP[n*Pan+3];
					//Sum=V_Max(Sum,MECRP[n*Pan]);
				}
				MECRDP[0]=(Sum0*Div)>>16;
				MECRDP[1]=(Sum1*Div)>>16;
				MECRDP[2]=(Sum2*Div)>>16;
				MECRDP[3]=(Sum3*Div)>>16;
				MECRP+=4;
				MECRDP+=4;
			}
			MECRL+=MB.EMegaPitch.Pitch;
			MECRDL+=MB.EDest->Pitch;
		}
		break;
	}
	}
	MB.CurrentDraw++;
	if(MB.CurrentDraw>=MB.NumBlur) MB.CurrentDraw=0;
	MB.EDraw.MECR=MB.EMegaPitch.MECR+Pan*MB.CurrentDraw;
	return;
}

#else

void SPG_CONV G_MB_Render(G_MotionBlur& MB)
{
	int Pan=MB.EDraw.POCT*MB.EDraw.SizeX;
	int Div=65536/MB.NumBlur;
	BYTE* MECRL=MB.EMegaPitch.MECR;
	BYTE* MECRDL=MB.EDest->MECR;
	if(MB.NumBlur==2)
	{
	for(int y=0;y<MB.EMegaPitch.SizeY;y++)
	{
		BYTE*MECRP=MECRL;
		BYTE*MECRDP=MECRDL;
		for(int x=0;x<Pan;x++)
		{
			WORD Sum=(WORD)MECRP[x]+(WORD)MECRP[x+Pan];
			MECRP[x]=MECRDP[x]=Sum>>1;
		}
		MECRL+=MB.EMegaPitch.Pitch;
		MECRDL+=MB.EDest->Pitch;
	}
	}
	else if(MB.NumBlur==6)
	{
	for(int y=0;y<MB.EMegaPitch.SizeY;y++)
	{
		BYTE*MECRP=MECRL;
		BYTE*MECRDP=MECRDL;
		for(int x=0;x<Pan;x++)
		{
			int Sum=0;
			for(int n=0;n<6;n++)
			{
				Sum+=MECRP[n*Pan];
			}
			MECRP[MB.CurrentDraw*Pan]=*MECRDP=(Sum*Div)>>16;
			MECRP++;
			MECRDP++;
		}
		MECRL+=MB.EMegaPitch.Pitch;
		MECRDL+=MB.EDest->Pitch;
	}
	}
	else
	{
	for(int y=0;y<MB.EMegaPitch.SizeY;y++)
	{
		BYTE*MECRP=MECRL;
		BYTE*MECRDP=MECRDL;
		for(int x=0;x<Pan;x++)
		{
			int Sum=0;
			for(int n=0;n<MB.NumBlur;n++)
			{
				Sum+=MECRP[n*Pan];
				//Sum=V_Max(Sum,MECRP[n*Pan]);
			}
			MECRP[MB.CurrentDraw*Pan]=*MECRDP=(Sum*Div)>>16;
			MECRP++;
			MECRDP++;
		}
		MECRL+=MB.EMegaPitch.Pitch;
		MECRDL+=MB.EDest->Pitch;
	}
	MB.CurrentDraw++;
	if(MB.CurrentDraw>=MB.NumBlur) MB.CurrentDraw=0;
	MB.EDraw.MECR=MB.EMegaPitch.MECR+Pan*MB.CurrentDraw;
	}
	return;
}

#endif

void SPG_CONV G_MB_RenderFrom(G_MotionBlur& MB, G_Ecran& E)
{
#ifdef FastBlur
	//MB.EDraw.MECR=MB.EMegaPitch.MECR+MB.CurrentDraw*MB.EDraw.POCT*MB.EDraw.SizeX;
#ifdef LongBlur
	G_BlurCopy(MB.EDraw,E);
#else
	G_Copy(MB.EDraw,E);
#endif
#else
	G_Copy(G_MB_Ecran(MB),E);
#endif
	G_MB_Render(MB);
	return;
}

#endif
#endif

