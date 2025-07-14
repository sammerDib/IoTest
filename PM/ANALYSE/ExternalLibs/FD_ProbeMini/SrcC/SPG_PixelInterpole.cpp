
#include "SPG_General.h"

#ifdef SPG_General_USEPIXINT

#include "SPG_Includes.h"

#include <memory.h>

#define PX_AXIS_N -1.0f
#define PX_DIAG_N -0.707f

#define PX_AXIS (-1.0f*Lambda)
#define PX_DIAG (-0.707f*Lambda)

int SPG_CONV PIXINT_RELAX_Init(SPG_PIXINT_RELAX& PX, int SizeX, int SizeY, float* InterpoleTemp)
{
	memset(&PX,0,sizeof(SPG_PIXINT_RELAX));
	PX.SizeX=SizeX;
	PX.SizeY=SizeY;
	//PX.Downsampling=Downsampling;
	//PX.PixInterpole=SPG_TypeAlloc(SizeX*Downsampling*SizeY*Downsampling,float,"PIXINT_Init");
	PX.PixInterpole=InterpoleTemp;
	PX.PixInterpoleDelta=InterpoleTemp+SizeX*SizeY*PIXINT_RELAX_OVERSAMPLING*PIXINT_RELAX_OVERSAMPLING;
	CHECKPOINTER_L(PX.PixInterpole,PX.SizeX*PIXINT_RELAX_OVERSAMPLING*PX.SizeY*PIXINT_RELAX_OVERSAMPLING*sizeof(float),"PIXINT_ComputeDelta",return 0);
	CHECKPOINTER_L(PX.PixInterpoleDelta,PX.SizeX*PIXINT_RELAX_OVERSAMPLING*PX.SizeY*PIXINT_RELAX_OVERSAMPLING*sizeof(float),"PIXINT_ComputeDelta",return 0);

	PX.Sonde=SPG_TypeAlloc(2*PIXINT_RELAX_OVERSAMPLING*PIXINT_RELAX_OVERSAMPLING,PIXINT_RELAX_Kernel,"PIXINT_Init");
	PX.Matrix=PX.Sonde+PIXINT_RELAX_OVERSAMPLING*PIXINT_RELAX_OVERSAMPLING;

	for(int x=0;x<PIXINT_RELAX_OVERSAMPLING;x++)
	{
		for(int y=0;y<PIXINT_RELAX_OVERSAMPLING;y++)
		{
			float Lambda=1.0f;
			bool xBorder=((x==0)||(x==(PIXINT_RELAX_OVERSAMPLING-1)));
			bool yBorder=((y==0)||(y==(PIXINT_RELAX_OVERSAMPLING-1)));
			
			if(xBorder&&yBorder)//si trop fort -> effet de croix 
			{
				Lambda*=0.035f;
			}
			else if(xBorder||yBorder)//si trop fort -> effet de scintillement ou de point 
			{
				Lambda*=0.03f;
			}
			else
			{
				Lambda*=0.03f;
			}
			

			PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[0][0]=PX_DIAG_N;
			PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[1][0]=PX_AXIS_N;
			PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[2][0]=PX_DIAG_N;

			PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[0][1]=PX_AXIS_N;
			PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[2][1]=PX_AXIS_N;

			PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[0][2]=PX_DIAG_N;
			PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[1][2]=PX_AXIS_N;
			PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[2][2]=PX_DIAG_N;

			PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[1][1]=-(
				PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[0][0]+
				PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[1][0]+
				PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[2][0]+

				PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[0][1]+
				PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[2][1]+

				PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[0][2]+
				PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[1][2]+
				PX.Sonde[x+y*PIXINT_RELAX_OVERSAMPLING].v[2][2]);

			PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[0][0]=((x>0)&&(y>0))?PX_DIAG:0;
			PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[1][0]=(x>0)?PX_AXIS:0;
			PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[2][0]=((x>0)&&(y<PIXINT_RELAX_OVERSAMPLING-1))?PX_DIAG:0;

			PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[0][1]=(y>0)?PX_AXIS:0;
			PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[2][1]=(y<PIXINT_RELAX_OVERSAMPLING-1)?PX_AXIS:0;

			PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[0][2]=((x<PIXINT_RELAX_OVERSAMPLING-1)&&(y>0))?PX_DIAG:0;
			PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[1][2]=(x<PIXINT_RELAX_OVERSAMPLING-1)?PX_AXIS:0;
			PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[2][2]=((x<PIXINT_RELAX_OVERSAMPLING-1)&&(y<PIXINT_RELAX_OVERSAMPLING-1))?PX_DIAG:0;

			PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[1][1]=-(
				PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[0][0]+
				PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[1][0]+
				PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[2][0]+

				PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[0][1]+
				PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[2][1]+

				PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[0][2]+
				PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[1][2]+
				PX.Matrix[x+y*PIXINT_RELAX_OVERSAMPLING].v[2][2]);
		}
	}
	return PX.Etat=PX_OK;
}

void SPG_CONV PIXINT_RELAX_Close(SPG_PIXINT_RELAX& PX)
{
	if(PX.Etat)
	{
		//SPG_MemFreeZ(PX.PixBrut);
		//SPG_MemFreeZ(PX.PixInterpole);
		SPG_MemFree(PX.Sonde);
		//SPG_MemFreeZ(PX.Matrix);
	}
	memset(&PX,0,sizeof(SPG_PIXINT_RELAX));
	return;
}

/*
float SPG_FASTCONV PIXINT_Convolve(SPG_PIXINT& PX, int x, int y, int xd, int yd)
{
	int IntSizeX=PX.Downsampling*PX.SizeX;
	PIXINT_Kernel& K=PX.Sonde[xd+yd*PX.Downsampling];
	float Sonde=0;
	for(int yi=-1;yi<=1;yi++)
	{
		int IDOffset=(x*PX.Downsampling+xd)+(y*PX.Downsampling+yd+yi)*IntSizeX;
		for(int xi=-1;xi<=1;xi++)
		{
			RetVal+=K.v[1+yi][1+xi]*PX.PixInterpole[IDOffset+xi];
		}
	}
	return Sonde;
}

void SPG_FASTCONV PIXINT_AddMatrix(SPG_PIXINT& PX, int x, int y, int xd, int yd, float Sonde)
{
	int IntSizeX=PX.Downsampling*PX.SizeX;
	PIXINT_Kernel& K=PX.Matrix[xd+yd*PX.Downsampling];
	for(int yi=-1;yi<=1;yi++)
	{
		int IDOffset=(x*PX.Downsampling+xd)+(y*PX.Downsampling+yd+yi)*IntSizeX;
		for(int xi=-1;xi<=1;xi++)
		{
			PX.PixInterpoleDelta[IDOffset+xi]+=Sonde*K.v[1+yi][1+xi];
		}
	}
	return;
}
*/

#define MACRO_FOR_3(Instructions,Index) {const int Index=-1;Instructions;}{const int Index=0;Instructions;}{const int Index=1;Instructions;}
//#define MACRO_FOR_3(Instructions,Index) {for(int Index=-1;Index<=1;Index++){Instructions;}}
#define MACRO_FOR_S(Instructions,Index) {const int Index=0;Instructions;}{const int Index=1;Instructions;}{const int Index=2;Instructions;}
//#define MACRO_FOR_S(Instructions,Index) {for(int Index=0;Index<PIXINT_OVERSAMPLING;Index++){Instructions;}}

void SPG_CONV PIXINT_RELAX_ComputeDelta(SPG_PIXINT_RELAX& PX)
{
	CHECKPOINTER_L(PX.PixInterpoleDelta,PX.SizeX*PIXINT_RELAX_OVERSAMPLING*PX.SizeY*PIXINT_RELAX_OVERSAMPLING*sizeof(float),"PIXINT_ComputeDelta",return);
	memset(PX.PixInterpoleDelta,0,PX.SizeX*PIXINT_RELAX_OVERSAMPLING*PX.SizeY*PIXINT_RELAX_OVERSAMPLING*sizeof(float));
	int IntSizeX=PIXINT_RELAX_OVERSAMPLING*PX.SizeX;
	for(int y=1;y<PX.SizeY-1;y++)
	{
		for(int x=1;x<PX.SizeX-1;x++)
		{
			MACRO_FOR_S(
			int IDyxydOffset=(x*PIXINT_RELAX_OVERSAMPLING)+(y*PIXINT_RELAX_OVERSAMPLING+yd)*IntSizeX;
			MACRO_FOR_S(
			PIXINT_RELAX_Kernel& KS=PX.Sonde[xd+yd*PIXINT_RELAX_OVERSAMPLING];
			float Sonde=0;
			int IDyxydxdOffset=IDyxydOffset+xd;
			MACRO_FOR_3(int IDOffset=IDyxydxdOffset+yi*IntSizeX;MACRO_FOR_3(Sonde+=KS.v[1+yi][1+xi]*PX.PixInterpole[IDOffset+xi],xi),yi);
			PIXINT_RELAX_Kernel& KM=PX.Matrix[xd+yd*PIXINT_RELAX_OVERSAMPLING];
			MACRO_FOR_3(int IDOffset=IDyxydxdOffset+yi*IntSizeX;MACRO_FOR_3(PX.PixInterpoleDelta[IDOffset+xi]+=Sonde*KM.v[1+yi][1+xi],xi),yi)
			,xd)
			,yd);
		}
	}
	return;
}

void SPG_CONV PIXINT_RELAX_UpdatePixInterpole(SPG_PIXINT_RELAX& PX)
{
	CHECKPOINTER_L(PX.PixInterpole,PX.SizeX*PIXINT_RELAX_OVERSAMPLING*PX.SizeY*PIXINT_RELAX_OVERSAMPLING*sizeof(float),"PIXINT_ComputeDelta",return);
	CHECKPOINTER_L(PX.PixInterpoleDelta,PX.SizeX*PIXINT_RELAX_OVERSAMPLING*PX.SizeY*PIXINT_RELAX_OVERSAMPLING*sizeof(float),"PIXINT_ComputeDelta",return);
	const int Maxi=PX.SizeX*PIXINT_RELAX_OVERSAMPLING*PX.SizeY*PIXINT_RELAX_OVERSAMPLING;
	for(int i=0;i<Maxi;i++)
	{
		PX.PixInterpole[i]-=PX.PixInterpoleDelta[i];
	}
	return;
}

void SPG_CONV PIXINT_RELAX_Interpole(SPG_PIXINT_RELAX& PX, float* restrict PixBrut, int Iterations)
{
	CHECKPOINTER_L(PX.PixInterpole,PX.SizeX*PIXINT_RELAX_OVERSAMPLING*PX.SizeY*PIXINT_RELAX_OVERSAMPLING*sizeof(float),"PIXINT_ComputeDelta",return);
	CHECKPOINTER_L(PX.PixInterpoleDelta,PX.SizeX*PIXINT_RELAX_OVERSAMPLING*PX.SizeY*PIXINT_RELAX_OVERSAMPLING*sizeof(float),"PIXINT_ComputeDelta",return);
	int IntSizeX=PIXINT_RELAX_OVERSAMPLING*PX.SizeX;
	for(int y=0;y<PX.SizeY;y++)
	{
		for(int x=0;x<PX.SizeX;x++)
		{
			MACRO_FOR_S(MACRO_FOR_S(int xi=x*PIXINT_RELAX_OVERSAMPLING+xd;int yi=y*PIXINT_RELAX_OVERSAMPLING+yd;PX.PixInterpole[xi+yi*IntSizeX]=PixBrut[x+y*PX.SizeX],xd),yd);
		}
	}
	for(int i=0;i<Iterations;i++)
	{
		PIXINT_RELAX_ComputeDelta(PX);
		PIXINT_RELAX_UpdatePixInterpole(PX);
	}
	return;
}

/*
int SPG_FASTCONV PIXINT_Element(SPG_PIXINT& PX, int x, int y, int ix, int iy)
{
	CHECK(!V_InclusiveBound(x,-PX.PixSize,PX.PixSize),"PIXINT_Element",return PX.CenterIndex);
	CHECK(!V_InclusiveBound(y,-PX.PixSize,PX.PixSize),"PIXINT_Element",return PX.CenterIndex);
	CHECK(!V_InclusiveBound(ix,-PX.Oversampling,PX.Oversampling),"PIXINT_Element",return PX.CenterIndex);
	CHECK(!V_InclusiveBound(iy,-PX.Oversampling,PX.Oversampling),"PIXINT_Element",return PX.CenterIndex);
	return PX.CenterIndex+
		(x*(2*PX.Oversampling+1)+ix)+
		(y*(2*PX.Oversampling+1)+iy)*PX.Size;
}
*/

#define PIXINT_Element(PX,x,y,ix,iy) PX.CenterIndex+(x*(2*PX.Oversampling+1)+ix)+(y*(2*PX.Oversampling+1)+iy)*PX.Size

#define PIXINT_EDGE 2
/*
void SPG_CONV PIXINT_ComputeFilter(SPG_PIXINT& PX, int dks)
{
	int ks=2*dks+1;
	P_Create(PX.PSmoothKernel,ks,ks);
	PX.Kernel=P_Data(PX.P[PX.CurrentProfil]);
	float Sum=0;
	for(int y=0;y<ks;y++)
	{
		for(int x=0;x<ks;x++)
		{
			float C;
			if((x==dks)&&(y==dks)) 
			{
				C=1;
			}
			else
			{
				float R=sqrt( ((x-dks)*(x-dks)+(y-dks)*(y-dks)) )/ (float)(1+dks);
				//float R=(abs(x-dks)+abs(y-dks))/(float)(1+dks);
				float E=0.5f+0.5f*cos(V_PI*V_Sature(R,0,1));
				C=E*sin(V_DPI*R)/(V_DPI*R);
			}
			P_Element(PX.PSmoothKernel,x,y)=C;
			Sum+=C;
		}
	}
	for(int i=0;i<P_SizeX(PX.PSmoothKernel)*P_SizeY(PX.PSmoothKernel);i++)
	{
		P_Data(PX.PSmoothKernel)[i]/=Sum;
	}
	return;
}
*/
void SPG_CONV PIXINT_ComputeFilter(SPG_PIXINT& PX, int dks, float SincBandPass)
{
	CHECK((PX.Etat&PX_MEMALLOC)==0,"PIXINT_ComputeFilter",PX.Etat&=~PX_GEOMETRY;return);
	int ks=2*dks+1;
	if((PX.Etat&PX_FILTER)==0) P_Create(PX.PSmoothKernel,ks,ks);
	float Sum=0;
	for(int y=0;y<ks;y++)
	{
		for(int x=0;x<ks;x++)
		{
			float C;
			if((x==dks)&&(y==dks)) 
			{
				C=1;
			}
			else
			{
				float R=sqrt( ((x-dks)*(x-dks)+(y-dks)*(y-dks)) )/ (float)(1+dks);
				//float R=(abs(x-dks)+abs(y-dks))/(float)(1+dks);
				float E=0.5f+0.5f*cos(V_PI*V_Sature(R,0,1));
				C=E*sin(SincBandPass*R)/(SincBandPass*R);
			}
			P_Element(PX.PSmoothKernel,x,y)=C;
			Sum+=C;
		}
	}
	for(int i=0;i<P_SizeX(PX.PSmoothKernel)*P_SizeY(PX.PSmoothKernel);i++)
	{
		P_Data(PX.PSmoothKernel)[i]/=Sum;
	}
	PX.Etat|=PX_FILTER;
	return;
}

int SPG_CONV PIXINT_Init(SPG_PIXINT& PX, int Size, int Oversampling)
{
	memset(&PX,0,sizeof(SPG_PIXINT));
	PX.KernelSize=Size;
	PX.PixSize=Size+PIXINT_EDGE;
	PX.Oversampling=Oversampling;
	PX.Size=(2*PX.PixSize+1)*(2*PX.Oversampling+1);
	PX.CenterIndex=(PX.PixSize*(2*PX.Oversampling+1)+PX.Oversampling)*(1+PX.Size);//Centre=IndexX+IndexY*Pitch=Index*(1+Pitch)
	PX.CCDMask=SPG_TypeAlloc(PX.Size*PX.Size,BYTE,"PIXINT_Init");
	//PX.Kernel=SPG_TypeAlloc(PX.Size*PX.Size,float,"PIXINT_Init");
	PX.CurrentProfil=0;
	P_Create(PX.P[0],PX.Size,PX.Size);
	P_Create(PX.P[1],PX.Size,PX.Size);
	PX.Kernel=P_Data(PX.P[PX.CurrentProfil]);
	return PX.Etat=PX_MEMALLOC;
}

void SPG_CONV PIXINT_Close(SPG_PIXINT& PX)
{
	//SPG_MemFree(PX.Kernel);
	//PX.Kernel=0 dans le memset
	P_Close(PX.P[0]);
	P_Close(PX.P[1]);
	if(PX.Etat&PX_FILTER) P_Close(PX.PSmoothKernel);
	SPG_MemFree(PX.CCDMask);
	memset(&PX,0,sizeof(SPG_PIXINT));
}

#define CCDMASK_CENTERPIXEL 1
#define CCDMASK_EDGEPIXEL 2

#define PIXINT_FOR(PX,x,y) {for(int y=-PX.PixSize;y<=PX.PixSize;y++){for(int x=-PX.PixSize;x<=PX.PixSize;x++){
#define PIXINT_FORK(PX,x,y) {for(int y=-PX.KernelSize;y<=PX.KernelSize;y++){for(int x=-PX.KernelSize;x<=PX.KernelSize;x++){
#define PIXINT_FOROVER(PX,ix,iy) {for(int iy=-PX.Oversampling;iy<=PX.Oversampling;iy++){for(int ix=-PX.Oversampling;ix<=PX.Oversampling;ix++){
#define PIXINT_FORSUB(PX,x,y,ix,iy,I) PIXINT_FOROVER(PX,ix,iy) int I=PIXINT_Element(PX,x,y,ix,iy);
#define PIXINT_NEXT }}}

void SPG_CONV PIXINT_Compute(SPG_PIXINT& PX, int DeadZoneX, int DeadZoneY, int NIter, float Lambda)
{
	PIXINT_SetCCDGeometry(PX,DeadZoneX,DeadZoneY);
//NIter=6 Lambda=1.40 Filter=4*(PX.Oversampling-1) BandPass=0.97f
	PIXINT_ComputeFilter(PX,4*(PX.Oversampling-1),0.97f*V_DPI);
	for(int i=0;i<NIter;i++)
	{
		PIXINT_IntegreKernel(PX,Lambda);
		PIXINT_Smooth(PX);
		PIXINT_Normalize(PX);
	}
	PIXINT_IntegreKernel(PX,1);
	//PIXINT_SelectiveSmooth(PX);
	PIXINT_Smooth(PX);
	PIXINT_Normalize(PX);
	//PIXINT_SelectiveSmooth(PX);
	return;
}

void SPG_CONV PIXINT_ComputeBilinear(SPG_PIXINT& PX)
{
	PIXINT_SetCCDGeometry(PX,0,0);
	PIXINT_FOR(PX,x,y)
			PIXINT_FORSUB(PX,x,y,ix,iy,I)
			float X=x+(float)ix/(2*PX.Oversampling+1);
			float Y=y+(float)iy/(2*PX.Oversampling+1);
			float DX=(1.0f-fabs(X));
			float DY=(1.0f-fabs(Y));
			PX.Kernel[I]=V_Max(DX,0)*V_Max(DY,0);
			PIXINT_NEXT
	PIXINT_NEXT
	//PIXINT_Normalize(PX);
	return;
}

void SPG_CONV PIXINT_ComputeCosinus(SPG_PIXINT& PX)
{
	PIXINT_SetCCDGeometry(PX,0,0);
	PIXINT_FOR(PX,x,y)
			PIXINT_FORSUB(PX,x,y,ix,iy,I)
			float X=x+(float)ix/(2*PX.Oversampling+1);
			float Y=y+(float)iy/(2*PX.Oversampling+1);
			X=V_Sature(X,-1,1);
			Y=V_Sature(Y,-1,1);
			float DX=0.5f+0.5f*cos(V_PI*X);
			float DY=0.5f+0.5f*cos(V_PI*Y);
			PX.Kernel[I]=DX*DY;
			PIXINT_NEXT
	PIXINT_NEXT
	//PIXINT_Normalize(PX);
	return;
}

void SPG_CONV PIXINT_Save(SPG_PIXINT& PX)
{
	CHECK((PX.Etat&PX_MEMALLOC)==0,"PIXINT_SetCCDGeometry",return);
	BMP_WriteFloat(PX.Kernel,PX.Size,PX.Size,-1,2,"PIXINT_Kernel.bmp");
	BMP_WriteByte(PX.CCDMask,PX.Size,PX.Size,"PIXINT_CCDMask.bmp");
	if(PX.Etat&PX_FILTER) P_SaveToFile(PX.PSmoothKernel,"PIXINT_Smooth.bmp");
	P_SaveToFile(PX.P[PX.CurrentProfil],"PIXINT_P.bmp");
	return;
}

void SPG_CONV PIXINT_SetCCDGeometry(SPG_PIXINT& PX, int DeadZoneX, int DeadZoneY)
{
	CHECK((PX.Etat&PX_MEMALLOC)==0,"PIXINT_SetCCDGeometry",PX.Etat&=~PX_GEOMETRY;return);
	//CHECK((PX.Etat&PX_FILTER)==0,"PIXINT_SetCCDGeometry",PX.Etat&=~PX_GEOMETRY;return);
	memset(PX.Kernel,0,PX.Size*PX.Size*sizeof(float));
	memset(PX.CCDMask,0,PX.Size*PX.Size*sizeof(BYTE));
	PX.PixelCount=0;//surface active d'un pixel
	PIXINT_FOR(PX,x,y)
			int S=2;//pixels peripheriques
			if((x==0)&&(y==0)) S=1;//pixel central
			//parcours de la zone active
			int LY=PX.Oversampling-DeadZoneY;
			int LX=PX.Oversampling-DeadZoneX;
			for(int iy=-LY;iy<=LY;iy++)
			{
				for(int ix=-LX;ix<=LX;ix++)
				{
					int I=PIXINT_Element(PX,x,y,ix,iy);
					if(S==1)
					{
						PX.Kernel[I]=1.0f;
						PX.CCDMask[I]=CCDMASK_CENTERPIXEL;
						PX.PixelCount++;
					}
					else
					{
						PX.CCDMask[I]=CCDMASK_EDGEPIXEL;
					}
				}
			}
	PIXINT_NEXT
	PX.Etat|=PX_GEOMETRY;
	return;
}

void SPG_CONV PIXINT_UseCCDGeometry(SPG_PIXINT& PX, G_Ecran& Edest, G_Ecran& Esrc)
{
	CHECK((PX.Etat&(PX_MEMALLOC|PX_GEOMETRY))!=(PX_MEMALLOC|PX_GEOMETRY),"PIXINT_UseCCDGeometry",return);
	CHECK(Edest.Etat==0,"PIXINT_UseCCDGeometry",return);
	CHECK(Esrc.Etat==0,"PIXINT_UseCCDGeometry",return);
	//CHECK(G_SizeX(Edest)*(2*PX.Oversampling+1)>G_SizeX(Esrc),"PIXINT_UseCCDGeometry",return);
	//CHECK(G_SizeY(Edest)*(2*PX.Oversampling+1)>G_SizeY(Esrc),"PIXINT_UseCCDGeometry",return);

	int Scale=(2*PX.Oversampling+1);
	for(int y=0;y<G_SizeY(Edest);y++)
	{
		for(int x=0;x<G_SizeX(Edest);x++)
		{
			for(int p=0;p<Edest.POCT;p++)
			{
				float Sum=0;
				int Count=0;
				for(int iy=-PX.Oversampling;iy<=PX.Oversampling;iy++)
				{
					int yr=V_Sature((y*Scale+iy),0,(G_SizeY(Esrc)-1))*G_Pitch(Esrc);
					for(int ix=-PX.Oversampling;ix<=PX.Oversampling;ix++)
					{
						int I=PIXINT_Element(PX,0,0,ix,iy);
						if(PX.CCDMask[I]==CCDMASK_CENTERPIXEL)
						{
							int xr=V_Sature((x*Scale+ix),0,(G_SizeX(Esrc)-1))*G_POCT(Esrc);
							BYTE P=Esrc.MECR[yr+xr+p];
							Sum+=P;
							Count++;
						}
					}
				}
				Edest.MECR[p+x*Edest.POCT+y*Edest.Pitch]=V_Sature(Sum/Count,0,255);
			}
		}
	}
	return;
}

void SPG_CONV PIXINT_UseCCDGeometry(SPG_PIXINT& PX, float* Dest, int DestSizeX, int DestSizeY, float* Src, int SrcSizeX, int SrcSizeY)
{
	CHECK((PX.Etat&(PX_MEMALLOC|PX_GEOMETRY))!=(PX_MEMALLOC|PX_GEOMETRY),"PIXINT_UseCCDGeometry",return);
	CHECK(Dest==0,"PIXINT_UseCCDGeometry",return);
	CHECK(Src==0,"PIXINT_UseCCDGeometry",return);
	//CHECK(G_SizeX(Edest)*(2*PX.Oversampling+1)>G_SizeX(Esrc),"PIXINT_UseCCDGeometry",return);
	//CHECK(G_SizeY(Edest)*(2*PX.Oversampling+1)>G_SizeY(Esrc),"PIXINT_UseCCDGeometry",return);

	int Scale=(2*PX.Oversampling+1);
	for(int y=0;y<DestSizeY;y++)
	{
		for(int x=0;x<DestSizeX;x++)
		{
			float Sum=0;
			int Count=0;
			for(int iy=-PX.Oversampling;iy<=PX.Oversampling;iy++)
			{
				int yr=V_Sature((y*Scale+iy),0,(SrcSizeY-1))*SrcSizeX;
				for(int ix=-PX.Oversampling;ix<=PX.Oversampling;ix++)
				{
					int I=PIXINT_Element(PX,0,0,ix,iy);
					if(PX.CCDMask[I]==CCDMASK_CENTERPIXEL)
					{
						int xr=V_Sature((x*Scale+ix),0,(SrcSizeX-1));
						Sum+=Src[yr+xr];
						Count++;
					}
				}
			}
			Dest[x+y*DestSizeX]=Sum/Count;
		}
	}
	return;
}

void SPG_CONV PIXINT_IntegreKernel(SPG_PIXINT& PX, const float Lambda)
{
	CHECK((PX.Etat&(PX_MEMALLOC|PX_GEOMETRY))!=(PX_MEMALLOC|PX_GEOMETRY),"PIXINT_IntegreKernel",return);
	PIXINT_FOR(PX,x,y)
		if((x==0)&&(y==0))
		{
			float Sum=0;
			PIXINT_FORSUB(PX,x,y,ix,iy,I)
				if(PX.CCDMask[I]==CCDMASK_CENTERPIXEL)
				{
					Sum+=PX.Kernel[I];
				}
			PIXINT_NEXT
			Sum=-Lambda*(Sum/PX.PixelCount-1);
			PIXINT_FORSUB(PX,x,y,ix,iy,I)
				if(PX.CCDMask[I]==CCDMASK_CENTERPIXEL)
				{
					PX.Kernel[I]+=Sum;
				}
			PIXINT_NEXT
		}
		else if((x>=-PX.KernelSize)&&(x<=PX.KernelSize)&&(y>=-PX.KernelSize)&&(y<=PX.KernelSize))
		{
			float Sum=0;
			PIXINT_FORSUB(PX,x,y,ix,iy,I)
				if(PX.CCDMask[I]==CCDMASK_EDGEPIXEL)
				{
					Sum+=PX.Kernel[I];
				}
			PIXINT_NEXT
			Sum=-Lambda*Sum/PX.PixelCount;
			PIXINT_FORSUB(PX,x,y,ix,iy,I)
				if(PX.CCDMask[I]==CCDMASK_EDGEPIXEL)
				{
					PX.Kernel[I]+=Sum;
				}
			PIXINT_NEXT
		}
		else
		{
			PIXINT_FORSUB(PX,x,y,ix,iy,I)
				//if(PX.CCDMask[I]==CCDMASK_EDGEPIXEL)
				{
					PX.Kernel[I]=0;
				}
			PIXINT_NEXT
		}
	PIXINT_NEXT
	return;
}

void SPG_CONV PIXINT_Smooth(SPG_PIXINT& PX)
{
	CHECK((PX.Etat&(PX_MEMALLOC|PX_GEOMETRY))!=(PX_MEMALLOC|PX_GEOMETRY),"PIXINT_Smooth",return);
	//P_Convolve(PX.P[1-PX.CurrentProfil],PX.P[PX.CurrentProfil],PX.PSmoothKernel);
	P_ConvolveFast(PX.P[1-PX.CurrentProfil],PX.P[PX.CurrentProfil],PX.PSmoothKernel);
	PX.CurrentProfil=1-PX.CurrentProfil;
	PX.Kernel=P_Data(PX.P[PX.CurrentProfil]);
	/*
	PIXINT_FOR(PX,x,y)
		PIXINT_FORSUB(PX,x,y,ix,iy,I)
		if((PX.CCDMask[I]==CCDMASK_CENTERPIXEL)||(PX.CCDMask[I]==CCDMASK_EDGEPIXEL))
		{
			PX.Kernel[I]=0.125f*PX.Kernel[I]+0.875f*P_Data(PX.P[1-PX.CurrentProfil])[I];
		}
		PIXINT_NEXT
	PIXINT_NEXT
	*/
	return;
}

void SPG_CONV PIXINT_SelectiveSmooth(SPG_PIXINT& PX)
{
	CHECK((PX.Etat&(PX_MEMALLOC|PX_GEOMETRY))!=(PX_MEMALLOC|PX_GEOMETRY),"PIXINT_SelectiveSmooth",return);
	P_Convolve(PX.P[1-PX.CurrentProfil],PX.P[PX.CurrentProfil],PX.PSmoothKernel);
	PX.CurrentProfil=1-PX.CurrentProfil;
	PX.Kernel=P_Data(PX.P[PX.CurrentProfil]);
	
	PIXINT_FOR(PX,x,y)
		PIXINT_FORSUB(PX,x,y,ix,iy,I)
		if((PX.CCDMask[I]==CCDMASK_CENTERPIXEL)||(PX.CCDMask[I]==CCDMASK_EDGEPIXEL))
		{
			PX.Kernel[I]=P_Data(PX.P[1-PX.CurrentProfil])[I];
		}
		PIXINT_NEXT
	PIXINT_NEXT
	
	return;
}

void SPG_CONV PIXINT_Normalize(SPG_PIXINT& PX)
{
	CHECK((PX.Etat&(PX_MEMALLOC|PX_GEOMETRY))!=(PX_MEMALLOC|PX_GEOMETRY),"PIXINT_Normalize",return);
	PIXINT_FOROVER(PX,ix,iy)
		float Sum=0;
		int Count=0;
		PIXINT_FORK(PX,x,y)
			int I=PIXINT_Element(PX,x,y,ix,iy);
			Sum+=PX.Kernel[I];
			Count++;
		PIXINT_NEXT
		PIXINT_FORK(PX,x,y)
			int I=PIXINT_Element(PX,x,y,ix,iy);
			PX.Kernel[I]-=(Sum-1.0f)/Count;
			PIXINT_Round(PX.Kernel[I]);
		PIXINT_NEXT
	PIXINT_NEXT
	return;
}

void SPG_CONV PIXINT_GetInterpolationKernel(SPG_PIXINT& PX, int PosX, int PosY, float* Kernel)
{
	CHECKPOINTER_L(Kernel,(2*PX.KernelSize+1)*(2*PX.KernelSize+1)*sizeof(float),"PIXINT_GetInterpolationKernel",return);
	CHECK(!V_InclusiveBound(PosX,-PX.Oversampling,PX.Oversampling),"PIXINT_GetInterpolationKernel",return);
	CHECK(!V_InclusiveBound(PosY,-PX.Oversampling,PX.Oversampling),"PIXINT_GetInterpolationKernel",return);
	CHECK((PX.Etat&(PX_MEMALLOC|PX_GEOMETRY))!=(PX_MEMALLOC|PX_GEOMETRY),"PIXINT_GetInterpolationKernel",return);

	int KernelPitch=2*PX.KernelSize+1;
	float* KernelCenter=Kernel+PX.KernelSize*(1+KernelPitch);
	int Scale=2*PX.Oversampling+1;
	float* KernelSubCenter=PX.Kernel+PX.CenterIndex+PosX+PosY*PX.Size;
//PX.CenterIndex+(x*(2*PX.Oversampling+1)+ix)+(y*(2*PX.Oversampling+1)+iy)*PX.Size
	PIXINT_FORK(PX,x,y)
		KernelCenter[x+y*KernelPitch]=KernelSubCenter[x*Scale+y*Scale*PX.Size];
		//PX.Kernel[PIXINT_Element(PX,x,y,PosX,PosY)];
	PIXINT_NEXT
	return;
}


void SPG_CONV PIXINT_AddInterpolationKernel(SPG_PIXINT& PX, int PosX, int PosY, float* Kernel, float Coeff)
{
	CHECKPOINTER_L(Kernel,(2*PX.KernelSize+1)*(2*PX.KernelSize+1)*sizeof(float),"PIXINT_AddInterpolationKernel",return);
	CHECK(!V_InclusiveBound(PosX,-PX.Oversampling,PX.Oversampling),"PIXINT_AddInterpolationKernel",return);
	CHECK(!V_InclusiveBound(PosY,-PX.Oversampling,PX.Oversampling),"PIXINT_AddInterpolationKernel",return);
	CHECK((PX.Etat&(PX_MEMALLOC|PX_GEOMETRY))!=(PX_MEMALLOC|PX_GEOMETRY),"PIXINT_AddInterpolationKernel",return);

	int KernelPitch=2*PX.KernelSize+1;
	float* KernelCenter=Kernel+PX.KernelSize*(1+KernelPitch);
	int Scale=2*PX.Oversampling+1;
	float* KernelSubCenter=PX.Kernel+PX.CenterIndex+PosX+PosY*PX.Size;
	PIXINT_FORK(PX,x,y)
		KernelCenter[x+y*KernelPitch]+=Coeff*KernelSubCenter[x*Scale+y*Scale*PX.Size];
		//PX.Kernel[PIXINT_Element(PX,x,y,PosX,PosY)];
	PIXINT_NEXT
	return;
}

void SPG_CONV PIXINT_GetDXKernel(SPG_PIXINT& PX, float* Kernel)
{
	CHECKPOINTER_L(Kernel,(2*PX.KernelSize+1)*(2*PX.KernelSize+1)*sizeof(float),"PIXINT_GetInterpolationKernel",return);
	CHECK((PX.Etat&(PX_MEMALLOC|PX_GEOMETRY))!=(PX_MEMALLOC|PX_GEOMETRY),"PIXINT_GetDXKernel",return);

	memset(Kernel,0,(2*PX.KernelSize+1)*(2*PX.KernelSize+1)*sizeof(float));
	for(int iy=-PX.Oversampling;iy<=PX.Oversampling;iy++)
	{
		int Msk=0;
		int ix;
		for(ix=-PX.Oversampling;ix<=PX.Oversampling;ix++)
		{
			int I=PIXINT_Element(PX,0,0,ix,iy);
			if(Msk==(PX.CCDMask[I]&CCDMASK_CENTERPIXEL)) continue;
			if((Msk=(PX.CCDMask[I]&CCDMASK_CENTERPIXEL))==CCDMASK_CENTERPIXEL)
			{
				PIXINT_AddInterpolationKernel(PX,-ix,-iy,Kernel,-1);
			}
			else
			{//impossible au premier pixel
				PIXINT_AddInterpolationKernel(PX,-(ix-1),-iy,Kernel,1);
			}
		}
		if(Msk==CCDMASK_CENTERPIXEL)
		{//possible au dernier pixel
			PIXINT_AddInterpolationKernel(PX,-(ix-1),-iy,Kernel,1);
		}
	}
	for(int i=0;i<(2*PX.KernelSize+1)*(2*PX.KernelSize+1);i++)
	{
		PIXINT_Round(Kernel[i]);
	}
	return;
}

void SPG_CONV PIXINT_GetDYKernel(SPG_PIXINT& PX, float* Kernel)
{
	CHECKPOINTER_L(Kernel,(2*PX.KernelSize+1)*(2*PX.KernelSize+1)*sizeof(float),"PIXINT_GetInterpolationKernel",return);
	CHECK((PX.Etat&(PX_MEMALLOC|PX_GEOMETRY))!=(PX_MEMALLOC|PX_GEOMETRY),"PIXINT_GetDYKernel",return);

	memset(Kernel,0,PX.KernelSize*PX.KernelSize*sizeof(float));
	for(int ix=-PX.Oversampling;ix<=PX.Oversampling;ix++)
	{
		int Msk=0;
		int iy;
		for(iy=-PX.Oversampling;iy<=PX.Oversampling;iy++)
		{
			int I=PIXINT_Element(PX,0,0,ix,iy);
			if(Msk==(PX.CCDMask[I]&CCDMASK_CENTERPIXEL)) continue;
			if((Msk=(PX.CCDMask[I]&CCDMASK_CENTERPIXEL))==CCDMASK_CENTERPIXEL)
			{
				PIXINT_AddInterpolationKernel(PX,-ix,-iy,Kernel,-1);
			}
			else
			{//impossible au premier pixel
				PIXINT_AddInterpolationKernel(PX,-ix,-(iy-1),Kernel,1);
			}
		}
		if(Msk==CCDMASK_CENTERPIXEL)
		{//possible au dernier pixel
			PIXINT_AddInterpolationKernel(PX,-ix,-(iy-1),Kernel,1);
		}
	}
	for(int i=0;i<(2*PX.KernelSize+1)*(2*PX.KernelSize+1);i++)
	{
		PIXINT_Round(Kernel[i]);
	}
	return;
}

void SPG_CONV PIXINT_Interpole(SPG_PIXINT& PX, float* Dest, int DestSizeX, int DestSizeY, float* Src, int SrcSizeX, int SrcSizeY)
{
	CHECK((PX.Etat&(PX_MEMALLOC|PX_GEOMETRY))!=(PX_MEMALLOC|PX_GEOMETRY),"PIXINT_Interpole",return);
	int KernelPitch=2*PX.KernelSize+1;
	SPG_PtrAlloc(Kernel,KernelPitch*KernelPitch,float,"PIXINT_Interpole");
	float* KernelCenter=Kernel+PX.KernelSize*(1+KernelPitch);

	int Scale=(2*PX.Oversampling+1);
	for(int iy=-PX.Oversampling;iy<=PX.Oversampling;iy++)
	{
		for(int ix=-PX.Oversampling;ix<=PX.Oversampling;ix++)
		{
			PIXINT_GetInterpolationKernel(PX,-ix,-iy,Kernel);
			for(int y=PX.KernelSize;y<SrcSizeY-PX.KernelSize;y++)
			{
				int yd=V_Sature((y*Scale+iy),0,(DestSizeY-1))*DestSizeX;
				for(int x=PX.KernelSize;x<SrcSizeX-PX.KernelSize;x++)
				{
					int xd=V_Sature((x*Scale+ix),0,(DestSizeX-1));
					float Sum=0;
					for(int ky=-PX.KernelSize;ky<=PX.KernelSize;ky++)
					{
						//int yr=V_Sature((ky+y),0,(SrcSizeY-1))*SrcSizeX;
						int yr=(ky+y)*SrcSizeX;
						for(int kx=-PX.KernelSize;kx<=PX.KernelSize;kx++)
						{
							//int xr=V_Sature((kx+x),0,(SrcSizeX-1));
							int xr=(kx+x);
							Sum+=Src[yr+xr]*KernelCenter[kx+ky*KernelPitch];
						}
					}
					Dest[yd+xd]=Sum;
				}
			}
		}
	}
	SPG_PtrFree(Kernel);
	return;
}

#endif

