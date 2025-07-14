

#include "SPG_General.h"

#ifdef SPG_General_USEINTERACT3D

#include "SPG_Includes.h"

#include <float.h>

int SPG_CONV Interact3D_Init(Interact3D& it3d, Interact3D_Element* Elements, int NumElements, int XOffset, int YOffset, int ZOffset, int ElementSize, float XMin, float YMin, float ZMin, float XSize, float YSize, float ZSize, int SizeX, int SizeY, int SizeZ, int MaxDispatchElements)
{
	it3d.Elements=Elements;
	it3d.NumElements=NumElements;
	it3d.XOffset=XOffset;//position de float X dans la structure
	it3d.YOffset=YOffset;
	it3d.ZOffset=ZOffset;
	it3d.ElementSize=ElementSize;//taille de la structure
	it3d.iXMin=it3d.XMin=XMin;
	it3d.iYMin=it3d.YMin=YMin;
	it3d.iZMin=it3d.ZMin=ZMin;
	it3d.XSize=XSize;//taille d'une division
	it3d.YSize=YSize;
	it3d.ZSize=ZSize;
	it3d.SizeX=SizeX;//nombre de divisions de l'espace
	it3d.SizeY=SizeY;
	it3d.SizeZ=SizeZ;
	it3d.MaxDispatchElements=MaxDispatchElements;
	it3d.Dispatch=SPG_TypeAlloc(it3d.SizeX*it3d.SizeY*it3d.SizeZ,Interact3D_Dispatch,"it3d Dispatch");
	it3d.Dispatch[0].Elements=SPG_TypeAlloc(it3d.SizeX*it3d.SizeY*it3d.SizeZ*it3d.MaxDispatchElements,Interact3D_ElementRef,"XYZDispatch");
	it3d.Color=C256_Init(0,0,0,0);
	//Dispatch est un tableau de sizex*sizey*sizez elements du maillage de l'espace
	//chaque element contient la liste des <MaxDispatchElements objets dans ce volume de l'espace, ici tout est alloue d'un bloc, les pointeurs sont initialises chacun sur sa partie
	for(int i=0;i<it3d.SizeX*it3d.SizeY*it3d.SizeZ;i++)
	{
		it3d.Dispatch[i].NumElements=0;
		it3d.Dispatch[i].Elements=it3d.Dispatch[0].Elements+i*it3d.MaxDispatchElements;
	}
	return -1;
}

void SPG_CONV Interact3D_Close(Interact3D& it3d)
{
	SPG_MemFree(it3d.Dispatch[0].Elements);
	for(int i=0;i<it3d.SizeX*it3d.SizeY*it3d.SizeZ;i++)
	{
		it3d.Dispatch[i].NumElements=0;
		it3d.Dispatch[i].Elements=0;
	}
	SPG_MemFree(it3d.Dispatch);
	SPG_MemFree(it3d.Color);
	return;
}

int SPG_CONV Interact3D_InitAuto(Interact3D& it3d, Interact3D_Element* Elements, int NumElements, int XOffset, int YOffset, int ZOffset, int ElementSize, float InteractionRadius)
{
	it3d.Elements=Elements;
	it3d.NumElements=NumElements;
	it3d.XOffset=XOffset;
	it3d.YOffset=YOffset;
	it3d.ZOffset=ZOffset;
	it3d.ElementSize=ElementSize;
	float XMin=FLT_MAX;
	float YMin=FLT_MAX;
	float ZMin=FLT_MAX;
	float XMax=-FLT_MAX;
	float YMax=-FLT_MAX;
	float ZMax=-FLT_MAX;
	for(int i=0;i<it3d.NumElements;i++)
	{
		float Xc=IT3DXfromI(it3d,i);
		float Yc=IT3DYfromI(it3d,i);
		float Zc=IT3DZfromI(it3d,i);
		XMin=V_Min(XMin,Xc);
		YMin=V_Min(YMin,Yc);
		ZMin=V_Min(ZMin,Zc);
		XMin=V_Max(XMin,Xc);
		YMax=V_Max(YMin,Yc);
		ZMax=V_Max(ZMin,Zc);
	}
	it3d.SizeX=1+V_Floor((XMax-XMin)/InteractionRadius);
	it3d.SizeY=1+V_Floor((YMax-YMin)/InteractionRadius);
	it3d.SizeZ=1+V_Floor((ZMax-ZMin)/InteractionRadius);

	Interact3D_Init(it3d,it3d.Elements,it3d.NumElements,it3d.XOffset,it3d.YOffset,it3d.ZOffset,it3d.ElementSize,
		XMin,YMin,ZMin,
		(XMax-XMin)/it3d.SizeX,(YMax-YMin)/it3d.SizeY,(YMax-YMin)/it3d.SizeY,//dimensions des volumes elementaires
		it3d.SizeX,it3d.SizeY,it3d.SizeZ,//nombre de cases
		16*NumElements/(it3d.SizeX*it3d.SizeY*it3d.SizeZ));//variation max de densité

	return -1;
}

int SPG_CONV Interact3D_InitList(Interact3D& it3d, Interact3D_List& List)
{
	List.MaxElements=27*it3d.MaxDispatchElements;//les 3x3x3 cases avoisinnantes sont concernees
	List.NumElements=0;
	List.ElementList=SPG_TypeAlloc(List.MaxElements,Interact3D_ElementList,"List");
	return -1;
}

void SPG_CONV Interact3D_CloseList(Interact3D_List& List)
{
	List.MaxElements=0;
	List.NumElements=0;
	SPG_MemFree(List.ElementList);
	return;
}

void SPG_CONV Interact3D_Fill(Interact3D& it3d)
{
	{for(int i=0;i<it3d.SizeX*it3d.SizeY*it3d.SizeZ;i++)
	{
		it3d.Dispatch[i].NumElements=0;
	}}
	{for(int i=0;i<it3d.NumElements;i++)
	{
		Interact3D_Element* E=IT3DEfromI(it3d,i);
		int X=IT3DNX(it3d,E);
		int Y=IT3DNY(it3d,E);
		int Z=IT3DNZ(it3d,E);
		int Indice=
			(
			V_Sature(Z,0,it3d.SizeZ-1)
				*it3d.SizeY+
			V_Sature(Y,0,it3d.SizeY-1)
				)*it3d.SizeX+
			V_Sature(X,0,it3d.SizeX-1);

		CHECK_ELSE(it3d.Dispatch[Indice].NumElements>=it3d.MaxDispatchElements,"Interact3D_Fill: Dispatch overload",continue;)
		else
		{
			it3d.Dispatch[Indice].Elements[it3d.Dispatch[Indice].NumElements++]=E;
			//CHECK(((int)(it3d.Dispatch[Indice].Elements[it3d.Dispatch[Indice].NumElements-1]))>=(int)it3d.Elements+it3d.NumElements*it3d.ElementSize,"Interact3D_Fill",continue;);
		}
	}}
	return;
}

void SPG_CONV Interact3D_Draw(Interact3D& it3d, G_Ecran& E, int ProjPlane)
{
	for(int i=0;i<it3d.NumElements;i++)
	{
		float X=(IT3DXfromI(it3d,i)-it3d.XMin)/(it3d.SizeX*it3d.XSize);
		float Y=(IT3DYfromI(it3d,i)-it3d.YMin)/(it3d.SizeY*it3d.YSize);
		float Z=(IT3DZfromI(it3d,i)-it3d.ZMin)/(it3d.SizeZ*it3d.ZSize);
		if(X<0) X=0;
		if(X>1) X=1;
		if(Y<0) Y=0;
		if(Y>1) Y=1;
		if(Z<0) Z=0;
		if(Z>1) Z=1;
		if(ProjPlane>0) 
		{
			V_SWAP(float,X,Z);
		}
		else if(ProjPlane<0) 
		{
			V_SWAP(float,Y,Z);
		}
		int PixX=V_FloatToInt(X*(G_SizeX(E)-1));
		int PixY=V_FloatToInt(Y*(G_SizeY(E)-1));
		//int PixZ=V_FloatToInt(Y*G_SizeY(E));
		//G_DrawRect(E,PixX-1,PixY-1,PixX+1,PixY+1,0x00FF00);
		//
		*(PixCoul*)(E.MECR+E.POCT*PixX+E.Pitch*PixY)=it3d.Color[(int)(Z*255)];
	}
	return;
}

void SPG_CONV Interact3D_GetInteractionList(Interact3D& it3d, Interact3D_List& List, int ElementNumber, float InteractionRadius)
{
	List.NumElements=0;
	Interact3D_Element* E=IT2DEfromI(it3d,ElementNumber);
	float X=IT3DX(it3d,E);	
	float Y=IT3DY(it3d,E);	
	float Z=IT3DZ(it3d,E);	
	int NX=IT3DNX(it3d,E);
	int NY=IT3DNY(it3d,E);
	int NZ=IT3DNZ(it3d,E);
	//int InterTri=(int)E;
	float RayonCarre=InteractionRadius*InteractionRadius;
	int zstart=V_Max(NZ-1,0);
	int zstop=V_Min(NZ+1,it3d.SizeZ-1);
	int ystart=V_Max(NY-1,0);
	int ystop=V_Min(NY+1,it3d.SizeY-1);
	int xstart=V_Max(NX-1,0);
	int xstop=V_Min(NX+1,it3d.SizeX-1);
	for(int nz=zstart;nz<=zstop;nz++)
	{
	for(int ny=ystart;ny<=ystop;ny++)
	{
	for(int nx=xstart;nx<=xstop;nx++)
	{
		Interact3D_Dispatch& Dispatch=
			it3d.Dispatch[(nz*it3d.SizeY+ny)*it3d.SizeX+nx];
		for(int i=0;i<Dispatch.NumElements;i++)
		{
			//if(((int)(it3d.Dispatch[Indice].Elements+i))<=InterTri) continue;
			//CHECK(((int)(it3d.Dispatch[Indice].Elements[i]))>=(int)it3d.Elements+it3d.NumElements*it3d.ElementSize,"Interact3D_GetInteractionList",continue;);
			float DeltaX=IT3DX(it3d,Dispatch.Elements[i])-X;
			float DeltaY=IT3DY(it3d,Dispatch.Elements[i])-Y;
			float DeltaZ=IT3DZ(it3d,Dispatch.Elements[i])-Z;
			float DistanceCarre=DeltaX*DeltaX+DeltaY*DeltaY+DeltaZ*DeltaZ;
			if((DistanceCarre<=RayonCarre)&&(DistanceCarre>0))
			{
				CHECK_ELSE(List.NumElements>=List.MaxElements,"Interact3D_GetInteractionList: Overload",return;)
				else
				{
					List.ElementList[List.NumElements].DeltaX=DeltaX;
					List.ElementList[List.NumElements].DeltaY=DeltaY;
					List.ElementList[List.NumElements].DeltaZ=DeltaZ;
					List.ElementList[List.NumElements].DistanceCarre=DistanceCarre;
					List.ElementList[List.NumElements].Element=Dispatch.Elements[i];
					List.NumElements++;
				}
			}
		}

	}
	}
	}
	return;
}

#endif
