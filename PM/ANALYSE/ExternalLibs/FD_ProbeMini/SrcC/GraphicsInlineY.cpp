

//DO NOT COMPILE: INCLUDE FILE ONLY
//#ifdef SPG_General_USEGraphicsRenderPoly



#ifndef DEF_G_POCT
#error GraphicsInlineY: DEF_G_POCT non defini
#endif
#ifdef CoulC
#error GraphicsInlineY: CoulC defini
#endif

#include "InlineBuildCoulC.cpp"

{

BYTE*M=E.MECR+E.Pitch*V_Max(Y0,0);
BRES_START_CLIP(X,Y0,X0,Y1-Y0,X1-X0);
if (
	((unsigned)BRES_GET(X)<(unsigned)E.SizeX)
	&&
	((unsigned)X1<(unsigned)E.SizeX)
	)
{
#pragma ivdep
	for(int Y=V_Max(Y0,0);Y<=V_Min(Y1,E.SizeY-1);Y++)
	{
#if (DEF_G_POCT==4)
		((DWORD*)M)[BRES_GET(X)]=ICC_Coul;
#elif (DEF_G_POCT==3)
		*(PixCoul24*)(M+DEF_G_POCT*BRES_GET(X))=ICC_Coul;
#elif (DEF_G_POCT==2)
		((WORD*)M)[BRES_GET(X)]=ICC_Coul;
#elif (DEF_G_POCT==1)
		M[BRES_GET(X)]=ICC_Coul;
#else
#error GraphicsInlineX: DEF_G_POCT invalide
#endif
		M+=E.Pitch;
		BRES_NEXT(X);
	}
}
else
{
#pragma ivdep
	for(int Y=V_Max(Y0,0);Y<=V_Min(Y1,E.SizeY-1);Y++)
	{
		if (V_IsBound(BRES_GET(X),0,E.SizeX))
		{
#if (DEF_G_POCT==4)
		((DWORD*)M)[BRES_GET(X)]=ICC_Coul;
#elif (DEF_G_POCT==3)
		*(PixCoul24*)(M+DEF_G_POCT*BRES_GET(X))=ICC_Coul;
#elif (DEF_G_POCT==2)
		((WORD*)M)[BRES_GET(X)]=ICC_Coul;
#elif (DEF_G_POCT==1)
		M[BRES_GET(X)]=ICC_Coul;
#else
#error GraphicsInlineY: DEF_G_POCT invalide
#endif
		}
		M+=E.Pitch;
		BRES_NEXT(X);
		
	}
}

}

#undef DEF_G_POCT
#undef ICC_Coul
#undef DEF_G_RENDERCOUL


//#endif

