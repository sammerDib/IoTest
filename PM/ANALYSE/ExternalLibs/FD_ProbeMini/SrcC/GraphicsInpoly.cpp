
{
#ifndef DEF_G_POCT
#error GraphicsInpoly: DEF_G_POCT non defini
#endif

#include "InlineBuildCoulC.cpp"

int y=V_Max(Y[NYMin],0);

BYTE*MECR=E.MECR+y*E.Pitch;
BRES_Y BXG;
BRES_Y BXD;
for(int nseg=0;nseg<3;nseg++)
{
	if (CPG[nseg])
	{
		BRES_SET(BXG,XRefG[nseg]);
		BRES_SAFENEWADDTO(BXG,DYG[nseg],DXG[nseg]);
	}
	if (CPD[nseg]) 
	{
		BRES_SET(BXD,XRefD[nseg]);
		BRES_SAFENEWADDTO(BXD,DYD[nseg],DXD[nseg]);
	}
	
	for (;y<YPos[nseg];y++)
	{
		int X1=BRES_GET(BXG);
		int X2=BRES_GET(BXD);
		
		for(int x=V_Max(X1,0);x<V_Min(X2,E.SizeX);x++)
#if (DEF_G_POCT==4)
			((DWORD*)MECR)[x]=ICC_Coul;
#elif (DEF_G_POCT==3)
			*(PixCoul24*)(MECR+3*x)=ICC_Coul;
#elif (DEF_G_POCT==2)
			((WORD*)MECR)[x]=ICC_Coul;
#elif (DEF_G_POCT==1)
			MECR[x]=ICC_Coul;
#else
#error GraphicsInpoly: DEF_G_POCT invalide
#endif
		
		MECR+=E.Pitch;
		BRES_NEXT(BXG);
		BRES_NEXT(BXD);
	}
}

#undef DEF_G_POCT
#undef ICC_Coul
}

