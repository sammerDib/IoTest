
#include "SPG_General.h"

#ifdef SPG_General_USESTEREOVIS

#include "..\SrcC\SPG_MinIncludes.h"
#include "SPG_StereoVision.h"

#include <string.h>
#include <float.h>

int SPG_CONV SPG_SV_Init(SPG_STEREOVIS& SV, SG_PDV& Vue, float EyeDist, int EyeDistPix, float EyeConvergence, int Mode)
{
	memset(&SV,0,sizeof(SPG_STEREOVIS));
	SV.Vue=&Vue;
	G_DrawRect(Vue.Ecran,0,0,G_SizeX(Vue.Ecran),G_SizeY(Vue.Ecran),0);
	SV.EyeDist=EyeDist;
	SV.EyeDistPix=EyeDistPix;
	SV.EyeConvergence=EyeConvergence;
	SV.Mode=Mode;
	switch(Mode)
	{
	case SPG_SV_HALFSCREEN:
		G_InitSousEcran(SV.ELeft,Vue.Ecran,0,0,G_SizeX(Vue.Ecran)>>1,G_SizeY(Vue.Ecran));
		G_InitSousEcran(SV.ERight,Vue.Ecran,G_SizeX(Vue.Ecran)>>1,0,G_SizeX(Vue.Ecran)>>1,G_SizeY(Vue.Ecran));
		break;
	case SPG_SV_REDBLUE:
		G_InitMemoryEcran(SV.ELeft,G_POCT(Vue.Ecran),G_SizeX(Vue.Ecran),G_SizeY(Vue.Ecran));
		G_InitMemoryEcran(SV.ERight,G_POCT(Vue.Ecran),G_SizeX(Vue.Ecran),G_SizeY(Vue.Ecran));
		break;
	}
	return -1;
}

void SPG_CONV SPG_SV_Close(SPG_STEREOVIS& SV)
{
	G_CloseEcran(SV.ELeft);
	G_CloseEcran(SV.ERight);
	memset(&SV,0,sizeof(SPG_STEREOVIS));
	return;
}

void SPG_CONV SPG_SV_FinishRenderREDBLUE(SPG_STEREOVIS& SV)
{
	if(SV.Vue==0) return;
	CHECK(SV.Mode!=SPG_SV_REDBLUE,"SPG_SV_FinishRenderREDBLUE",return);
	SG_PDV& VueRef=*(SV.Vue);
	if(
		(G_POCT(SV.ELeft)==4)&&
		(G_POCT(SV.ERight)==4)&&
		(G_POCT(VueRef.Ecran)==4))
	{
		
		for(int y=0;y<G_SizeY(VueRef.Ecran);y++)
		{
			DWORD* DestLine=(DWORD*)(G_MECR(VueRef.Ecran)+y*G_Pitch(VueRef.Ecran));
			DWORD* SrcLeft=(DWORD*)(G_MECR(SV.ELeft)+y*G_Pitch(SV.ELeft));
			DWORD* SrcRight=(DWORD*)(G_MECR(SV.ERight)+y*G_Pitch(SV.ERight));
			for(int x=0;x<G_SizeX(VueRef.Ecran);x+=4)
			{
				DestLine[x]=
					(SrcLeft[x]&0x00FFFF)|
					(SrcRight[x]&0xFF0000);
				DestLine[x+1]=
					(SrcLeft[x+1]&0x00FFFF)|
					(SrcRight[x+1]&0xFF0000);
				DestLine[x+2]=
					(SrcLeft[x+2]&0x00FFFF)|
					(SrcRight[x+2]&0xFF0000);
				DestLine[x+3]=
					(SrcLeft[x+3]&0x00FFFF)|
					(SrcRight[x+3]&0xFF0000);
			}
		}
		/*
		for(int y=0;y<G_SizeY(VueRef.Ecran);y++)
		{
			BYTE* restrict DestLine=(BYTE*)(G_MECR(VueRef.Ecran)+y*G_Pitch(VueRef.Ecran));
			BYTE* restrict SrcLeft=(BYTE*)(G_MECR(SV.ELeft)+y*G_Pitch(SV.ELeft));
			BYTE* restrict SrcRight=(BYTE*)(G_MECR(SV.ERight)+y*G_Pitch(SV.ERight));
			for(int x=0;x<G_SizeX(VueRef.Ecran);x+=4)
			{
				DestLine[4*x]=SrcLeft[4*x];
				DestLine[4*x+2]=SrcRight[4*x];
				DestLine[4*x+4]=SrcLeft[4*x+4];
				DestLine[4*x+6]=SrcRight[4*x+4];
				DestLine[4*x+8]=SrcLeft[4*x+8];
				DestLine[4*x+10]=SrcRight[4*x+8];
				DestLine[4*x+12]=SrcLeft[4*x+12];
				DestLine[4*x+14]=SrcRight[4*x+12];
			}
		}
		*/
	}
	else
	{
		for(int y=0;y<G_SizeY(VueRef.Ecran);y++)
		{
			BYTE* DestLine=G_MECR(VueRef.Ecran)+y*G_Pitch(VueRef.Ecran);
			BYTE* SrcLeft=G_MECR(SV.ELeft)+y*G_Pitch(SV.ELeft);
			BYTE* SrcRight=G_MECR(SV.ERight)+y*G_Pitch(SV.ERight);
			for(int x=0;x<G_SizeX(VueRef.Ecran);x++)
			{
				DestLine[x*G_POCT(VueRef.Ecran)]=
					SrcLeft[x*G_POCT(SV.ELeft)];
				//DestLine[x*G_POCT(VueRef.Ecran)+1]=
				//	SrcLeft[x*G_POCT(SV.ELeft)+1];
				DestLine[x*G_POCT(VueRef.Ecran)+2]=
					SrcRight[x*G_POCT(SV.ERight)+2];
			}
		}
	}
	return;
}

void SPG_CONV SPG_SV_Render(SPG_STEREOVIS& SV)
{
	CHECK(SV.Vue==0,"SPG_SV_Render",return;);

	SG_PDV& VueRef=*(SV.Vue);

	SG_PDV VuePart=VueRef;
	{
		VuePart.Ecran=SV.ELeft;
		{
			V_StackAllocXYZ(T,0,-SV.EyeDist,0);
			V_TranslateRepS(VuePart.Rep,T);
		}
		{
			V_StackAllocXYZ(R,0,0,SV.EyeConvergence);
			V_RotateRepS(VuePart.Rep,R);
		}
		SGE_TransformAndRender(VuePart);
	}
	VuePart=VueRef;
	{
		VuePart.Ecran=SV.ERight;
		{
			V_StackAllocXYZ(T,0,SV.EyeDist,0);
			V_TranslateRepS(VuePart.Rep,T);
		}
		{
			V_StackAllocXYZ(R,0,0,-SV.EyeConvergence);
			V_RotateRepS(VuePart.Rep,R);
		}
		SGE_TransformAndRender(VuePart);
	}
	//VuePart=VueRef;
	switch(SV.Mode)
	{
	case SPG_SV_HALFSCREEN:
		break;
	case SPG_SV_REDBLUE:
		SPG_SV_FinishRenderREDBLUE(SV);
		break;
	}
	return;
}

void SPG_CONV SPG_SV_AdaptTextureColor(BYTE* M, int POCT, int Pitch, int SizeX, int SizeY)
{
	CHECK((M==0)||(POCT==0)||(Pitch==0),"SPG_SV_AdaptTextureColor",return);
	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
			int B=M[x*POCT+y*Pitch];
			int V=M[x*POCT+y*Pitch+1];
			int R=M[x*POCT+y*Pitch+2];
			B=V_Max(B,V);
			V>>=1;
			B=V_Max(B,(R+B)>>1);
			R=V_Max(R,(R+B)>>1);
			M[x*POCT+y*Pitch]=B;
			M[x*POCT+y*Pitch+1]=V;
			M[x*POCT+y*Pitch+2]=R;
		}
	}
	return;
}


#endif

