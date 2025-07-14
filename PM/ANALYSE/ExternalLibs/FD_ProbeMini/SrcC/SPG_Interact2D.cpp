

#include "SPG_General.h"

#ifdef SPG_General_USEINTERACT2D

#include "SPG_Includes.h"

#include <float.h>

int SPG_CONV Interact2D_Init(Interact2D& it2d, Interact2D_Element* Elements, int NumElements, int XOffset, int YOffset, int ElementSize, float XMin, float YMin, float XSize, float YSize, int SizeX, int SizeY, int MaxDispatchElements)
{
	it2d.Elements=Elements;
	it2d.NumElements=NumElements;
	it2d.XOffset=XOffset;
	it2d.YOffset=YOffset;
	it2d.ElementSize=ElementSize;
	it2d.iXMin=it2d.XMin=XMin;
	it2d.iYMin=it2d.YMin=YMin;
	it2d.XSize=XSize;
	it2d.YSize=YSize;
	it2d.SizeX=SizeX;
	it2d.SizeY=SizeY;
	it2d.MaxDispatchElements=MaxDispatchElements;
	it2d.Dispatch=SPG_TypeAlloc(it2d.SizeX*it2d.SizeY,Interact2D_Dispatch,"it2d Dispatch");
	it2d.Dispatch[0].Elements=SPG_TypeAlloc(it2d.SizeX*it2d.SizeY*it2d.MaxDispatchElements,Interact2D_ElementRef,"XY Dispatch");
	for(int i=0;i<it2d.SizeX*it2d.SizeY;i++)
	{
		it2d.Dispatch[i].NumElements=0;
		it2d.Dispatch[i].Elements=it2d.Dispatch[0].Elements+i*it2d.MaxDispatchElements;
	}
	return -1;
}

void SPG_CONV Interact2D_Close(Interact2D& it2d)
{
	SPG_MemFree(it2d.Dispatch[0].Elements);
	for(int i=0;i<it2d.SizeX*it2d.SizeY;i++)
	{
		it2d.Dispatch[i].NumElements=0;
		it2d.Dispatch[i].Elements=0;
	}
	SPG_MemFree(it2d.Dispatch);
	return;
}

int SPG_CONV Interact2D_InitAuto(Interact2D& it2d, Interact2D_Element* Elements, int NumElements, int XOffset, int YOffset, int ElementSize, float InteractionRadius)
{
	it2d.Elements=Elements;
	it2d.NumElements=NumElements;
	it2d.XOffset=XOffset;
	it2d.YOffset=YOffset;
	it2d.ElementSize=ElementSize;
	float XMin=FLT_MAX;
	float YMin=FLT_MAX;
	float XMax=-FLT_MAX;
	float YMax=-FLT_MAX;
	for(int i=0;i<it2d.NumElements;i++)
	{
		float Xc=IT2DXfromI(it2d,i);
		float Yc=IT2DYfromI(it2d,i);
		XMin=V_Min(XMin,Xc);
		YMin=V_Min(YMin,Yc);
		XMin=V_Max(XMin,Xc);
		YMax=V_Max(YMin,Yc);
	}
	it2d.SizeX=1+V_Floor((XMax-XMin)/InteractionRadius);
	it2d.SizeY=1+V_Floor((YMax-YMin)/InteractionRadius);

	Interact2D_Init(it2d,it2d.Elements,it2d.NumElements,it2d.XOffset,it2d.YOffset,it2d.ElementSize,
		XMin,YMin,
		(XMax-XMin)/it2d.SizeX,(YMax-YMin)/it2d.SizeY,
		it2d.SizeX,it2d.SizeY,9*NumElements/(it2d.SizeX*it2d.SizeY));

	return -1;
}

int SPG_CONV Interact2D_InitList(Interact2D& it2d, Interact2D_List& List)
{
	List.MaxElements=9*it2d.MaxDispatchElements;
	List.NumElements=0;
	List.ElementList=SPG_TypeAlloc(List.MaxElements,Interact2D_ElementList,"List");
	return -1;
}

void SPG_CONV Interact2D_CloseList(Interact2D_List& List)
{
	List.MaxElements=0;
	List.NumElements=0;
	SPG_MemFree(List.ElementList);
	return;
}

void SPG_CONV Interact2D_Fill(Interact2D& it2d)
{
	{for(int i=0;i<it2d.SizeX*it2d.SizeY;i++)
	{
		it2d.Dispatch[i].NumElements=0;
	}}
	{for(int i=0;i<it2d.NumElements;i++)
	{
		Interact2D_Element* E=IT2DEfromI(it2d,i);
		int X=IT2DNX(it2d,E);
		int Y=IT2DNY(it2d,E);
		int Indice=V_Sature(Y,0,it2d.SizeY-1)*it2d.SizeX+V_Sature(X,0,it2d.SizeX-1);
		CHECK_ELSE(it2d.Dispatch[Indice].NumElements>=it2d.MaxDispatchElements,"Interact2D_Fill: Dispatch overload",continue;)
		else
		{
			it2d.Dispatch[Indice].Elements[it2d.Dispatch[Indice].NumElements++]=E;
			//CHECK(((int)(it2d.Dispatch[Indice].Elements[it2d.Dispatch[Indice].NumElements-1]))>=(int)it2d.Elements+it2d.NumElements*it2d.ElementSize,"Interact2D_Fill",continue;);
		}
	}}
	return;
}

void SPG_CONV Interact2D_Draw(Interact2D& it2d, G_Ecran& E)
{
	for(int i=0;i<it2d.NumElements;i++)
	{
		float X=(IT2DXfromI(it2d,i)-it2d.XMin)/(it2d.SizeX*it2d.XSize);
		float Y=(IT2DYfromI(it2d,i)-it2d.YMin)/(it2d.SizeY*it2d.YSize);
		if(X<0) X=0;
		if(X>1) X=1;
		if(Y<0) Y=0;
		if(Y>1) Y=1;
		int PixX=V_FloatToInt(X*(G_SizeX(E)-1));
		int PixY=V_FloatToInt(Y*(G_SizeY(E)-1));
		//G_DrawRect(E,PixX-1,PixY-1,PixX+1,PixY+1,0x00FF00);
		*(DWORD*)(E.MECR+E.POCT*PixX+E.Pitch*PixY)=0x00FF00;
	}
	return;
}

void SPG_CONV Interact2D_GetInteractionList(Interact2D& it2d, Interact2D_List& List, int ElementNumber, float InteractionRadius)
{
	List.NumElements=0;
	Interact2D_Element* E=IT2DEfromI(it2d,ElementNumber);
	float X=IT2DX(it2d,E);	
	float Y=IT2DY(it2d,E);	
	int NX=IT2DNX(it2d,E);
	int NY=IT2DNY(it2d,E);
	//int InterTri=(int)E;
	float RayonCarre=InteractionRadius*InteractionRadius;
	int ystart=V_Max(NY-1,0);
	int ystop=V_Min(NY+1,it2d.SizeY-1);
	int xstart=V_Max(NX-1,0);
	int xstop=V_Min(NX+1,it2d.SizeX-1);
	for(int ny=ystart;ny<=ystop;ny++)
	{
	for(int nx=xstart;nx<=xstop;nx++)
	{
		Interact2D_Dispatch& Dispatch=it2d.Dispatch[ny*it2d.SizeX+nx];
		for(int i=0;i<Dispatch.NumElements;i++)
		{
			//if(((int)(it2d.Dispatch[Indice].Elements+i))<=InterTri) continue;
			//CHECK(((int)(it2d.Dispatch[Indice].Elements[i]))>=(int)it2d.Elements+it2d.NumElements*it2d.ElementSize,"Interact2D_GetInteractionList",continue;);
			float DeltaX=IT2DX(it2d,Dispatch.Elements[i])-X;
			float DeltaY=IT2DY(it2d,Dispatch.Elements[i])-Y;
			float DistanceCarre=DeltaX*DeltaX+DeltaY*DeltaY;
			if((DistanceCarre<=RayonCarre)&&(DistanceCarre>0))
			{
				CHECK_ELSE(List.NumElements>=List.MaxElements,"Interact2D_GetInteractionList: Overload",return;)
				else
				{
					List.ElementList[List.NumElements].DeltaX=DeltaX;
					List.ElementList[List.NumElements].DeltaY=DeltaY;
					List.ElementList[List.NumElements].DistanceCarre=DistanceCarre;
					List.ElementList[List.NumElements].Element=Dispatch.Elements[i];
					List.NumElements++;
				}
			}
		}

	}
	}
	return;
}

#endif
