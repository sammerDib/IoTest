

#include "SPG_General.h"

#ifdef SPG_General_USEProfil

#include "SPG_Includes.h"

#include <memory.h>

void SPG_CONV P_CreateIList(Profil* Pdst, Profil* Psrc, P_ILISTSTRUCT &LS)
{
	SPG_ZeroStruct(LS);
	LS.Pdst=Pdst;
	LS.Psrc=Psrc;
	LS.L=SPG_TypeAlloc(P_SizeX((*Pdst))*P_SizeY((*Pdst)),P_ILIST,"P_CreateIList");
}

void SPG_CONV P_DestroyIList(P_ILISTSTRUCT &LS)
{
	SPG_MemFree(LS.L);
	SPG_ZeroStruct(LS);
}

void SPG_CONV P_SetMIList(P_ILISTSTRUCT& LS)
{
	int dstx=P_SizeX((*LS.Pdst));
	int dsty=P_SizeY((*LS.Pdst));
	int srcx=P_SizeX((*LS.Psrc));
	int srcy=P_SizeY((*LS.Psrc));
	int MX=(dstx-srcx);
	CHECK(MX&1,"P_SetMIList",return);
	MX/=2;
	int MY=(dsty-srcy);
	CHECK(MY&1,"P_SetMIList",return);
	MY/=2;

	for(int y=0;y<dsty;y++)
	{
		for(int x=0;x<dstx;x++)
		{
			P_ILIST& LP=LS.L[x+y*dstx];
			LP.NumPos=0;
			float TW=0;

			int dx=V_Sature(x,MX,(dstx-MX-1));
			int dy=V_Sature(y,MY,(dsty-MY-1));
			int R2=(x-dx)*(x-dx)+(y-dy)*(y-dy);
			if(R2==0) 
			{
				int sx=dx-MX;
				int sy=dy-MY;
				CHECK(!V_IsBound(sx,0,srcx),"P_SetMIList",return);
				CHECK(!V_IsBound(sy,0,srcx),"P_SetMIList",return);
				LP.WP[LP.NumPos].d=LS.Psrc->D+sx+sy*srcx;
				LP.WP[LP.NumPos].w=1;
				LP.NumPos++;
				continue;
			}
			int K=2;//le rayon du cercle d'integration est deux fois le rayon du pixel le plus proche
			int iR=V_Round(sqrtf(R2));
			for(int idy=y-K*iR-1;idy<=y+K*iR+1;idy++)
			{
				if(!V_IsBound(idy,MY,dsty-MY)) continue;
				for(int idx=x-K*iR-1;idx<=x+K*iR+1;idx++)
				{
					if(!V_IsBound(idx,MX,dstx-MX)) continue;
					float wR=K*sqrtf(R2)-sqrtf((x-idx)*(x-idx)+(y-idy)*(y-idy));
					if(wR<=0) continue;
					TW+=wR;

					int sx=idx-MX;
					int sy=idy-MY;
					CHECK(!V_IsBound(sx,0,srcx),"P_SetMIList",return);
					CHECK(!V_IsBound(sy,0,srcx),"P_SetMIList",return);
					LP.WP[LP.NumPos].d=LS.Psrc->D+sx+sy*srcx;
					LP.WP[LP.NumPos].w=wR;
					LP.NumPos++;

					CHECK(LP.NumPos>P_ILEN,"P_SetMIList",return);
				}
			}
			CHECK(LP.NumPos==0,"P_SetMIList",return);
			for(int i=0;i<LP.NumPos;i++)
			{
				//LP.WP[i].w=LP.WP[i].w/((1+(0.5f*iR)/(MX+MY))*TW);
				LP.WP[i].w=LP.WP[i].w/TW;
			}
		}
	}
}

void SPG_CONV P_IListFlip(P_ILISTSTRUCT& LS, int Orientation)
{
	int P=P_SizeX((*LS.Pdst))*P_SizeY((*LS.Pdst));
	{for(int p=0;p<P;p++) { LS.L[p].Flag=0; }}

	CHECK( (Orientation&PIList_OrientT) && ( P_SizeX((*LS.Pdst)) != P_SizeY((*LS.Pdst)) ) ,"P_IListFlip : Transpose",return);

	for(int x=0;x<P_SizeX((*LS.Pdst));x++)
	for(int y=0;y<P_SizeY((*LS.Pdst));y++)
	{
		P_ILIST flying=LS.L[x+y*P_SizeX((*LS.Pdst))];
		int dx=x; int dy=y; int Cycle=0;

		while(flying.Flag==0) //until(flying.Flag)
		{	CHECK(Cycle++>P,"P_IListFlip",break); //Flip X Y et T se redusent à un swap mais leur composition donne un cycle de longueur 4
			if(Orientation&PIList_OrientX) dx=P_SizeX((*LS.Pdst))-1-dx;
			if(Orientation&PIList_OrientY) dy=P_SizeY((*LS.Pdst))-1-dy;
			if(Orientation&PIList_OrientT) V_SWAP(int,dx,dy);
			flying.Flag=1; V_SWAP(P_ILIST,flying,LS.L[dx+dy*P_SizeX((*LS.Pdst))]);
		}
	}

	{for(int p=0;p<P;p++) { DbgCHECK(LS.L[p].Flag==0,"P_IListFlip");LS.L[p].Flag=0; }}
	return;
}

void SPG_CONV P_IListNormalize(P_ILISTSTRUCT& LS)
{
	for(int i=0;i<P_SizeX((*LS.Pdst))*P_SizeY((*LS.Pdst));i++)
	{
		P_ILIST& L=LS.L[i];
		float W=0;
		{for(int d=0;d<L.NumPos;d++)
		{
			W+=L.WP[d].w;
		}}
		if(W>0)
		{
			W=1/W;
			for(int d=0;d<L.NumPos;d++)
			{
				L.WP[d].w*=W;
			}
		}
	}
	return;
}

void SPG_CONV P_ComputeIList(P_ILISTSTRUCT& LS)
{
	for(int p=0;p<P_SizeX((*LS.Pdst))*P_SizeY((*LS.Pdst));p++)
	{
		P_ILIST& LP=LS.L[p];
		float& D=P_Data((*LS.Pdst))[p];
		D=0;
		{for(int i=0;i<LP.NumPos;i++)
		{
			D += *LP.WP[i].d * LP.WP[i].w;
		}}
	}
	return;
}


#endif

