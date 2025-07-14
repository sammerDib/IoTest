

//DO NOT COMPILE: INCLUDE FILE ONLY
//#ifdef SPG_General_USEGraphicsRenderPoly



#ifndef DEF_G_POCT
#error GraphicsInlineX: DEF_G_POCT non defini
#endif
#include "InlineBuildCoulC.cpp"
{

BYTE*M=E.MECR+DEF_G_POCT*V_Max(X0,0);
BRES_START_CLIP(Y,X0,Y0,X1-X0,Y1-Y0);
if (
	((unsigned)BRES_GET(Y)<(unsigned)E.SizeY)
	&&
	((unsigned)Y1<(unsigned)E.SizeY)
	)
{
#pragma ivdep
	for(int X=V_Max(X0,0);X<=V_Min(X1,E.SizeX-1);X++)
	{
#if (DEF_G_POCT==4)
		*(DWORD*)(M+E.Pitch*BRES_GET(Y))=ICC_Coul;
#elif (DEF_G_POCT==3)
		*(PixCoul24*)(M+E.Pitch*BRES_GET(Y))=ICC_Coul;
#elif (DEF_G_POCT==2)
		*(WORD*)(M+E.Pitch*BRES_GET(Y))=ICC_Coul;
#elif (DEF_G_POCT==1)
		M[E.Pitch*BRES_GET(Y)]=ICC_Coul;
#else
#error GraphicsInlineX: DEF_G_POCT invalide
#endif
		M+=DEF_G_POCT;
		BRES_NEXT(Y);
	}
}
else
{
#pragma ivdep
	for(int X=V_Max(X0,0);X<=V_Min(X1,E.SizeX-1);X++)
	{
		if (V_IsBound(BRES_GET(Y),0,E.SizeY))
		{
#if (DEF_G_POCT==4)
		*(DWORD*)(M+E.Pitch*BRES_GET(Y))=ICC_Coul;
#elif (DEF_G_POCT==3)
		*(PixCoul24*)(M+E.Pitch*BRES_GET(Y))=ICC_Coul;
#elif (DEF_G_POCT==2)
		*(WORD*)(M+E.Pitch*BRES_GET(Y))=ICC_Coul;
#elif (DEF_G_POCT==1)
		M[E.Pitch*BRES_GET(Y)]=ICC_Coul;
#else
#error GraphicsInlineX: DEF_G_POCT invalide
#endif
		}
		M+=DEF_G_POCT;
		BRES_NEXT(Y);
		
	}
}

}

#undef DEF_G_POCT
#undef ICC_Coul
#undef DEF_G_RENDERCOUL


//#endif

