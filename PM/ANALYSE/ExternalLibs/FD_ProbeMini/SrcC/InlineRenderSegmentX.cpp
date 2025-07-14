
//DO NOT COMPILE: INCLUDE FILE ONLY
#ifdef SPG_General_USEGraphicsRenderPoly


#pragma warning( disable : 4701 )//used without init
#pragma warning( disable : 4189 )//used without init

#ifdef DebugGraphicsTimer
	S_StartTimer(Global.T_InlineRenderSegment);
#endif

{
//position du bord gauche
	BRES_Y BXG;
//position du bord droit
	BRES_Y BXD;
#ifdef DEF_G_TEXTURE
//position dans la texture du bord gauche
	BRES_XY TG;
//position dans la texture du bord droit
	BRES_XY TD;
#endif
	
//point courant du polygone
	int CurGPoint=0;
	int CurDPoint=0;
//depart et arrivee (verticalement) des segments des bords droit et gauche
	int ygstop=G_TruePixel(P[PntG[CurGPoint]].y);
	int ydstop=G_TruePixel(P[PntD[CurDPoint]].y);
	
	{
#ifndef G_FASTCOPY
#if (DEF_G_POCT==4)
		DWORD* restrict MECR=(DWORD*)(E.MECR+V_Max(G_TruePixel(P[NYMin].y),0)*E.Pitch);
#else
		BYTE* restrict MECR=E.MECR+V_Max(G_TruePixel(P[NYMin].y),0)*E.Pitch;
#endif
#else
#if (DEF_G_POCT==4)
		DWORD* restrict MECR=(DWORD*)(E.MECR+G_TruePixel(P[NYMin].y)*E.Pitch);
#else
		BYTE* restrict MECR=E.MECR+G_TruePixel(P[NYMin].y)*E.Pitch;
#endif
#endif

#pragma ivdep
#ifndef G_FASTCOPY
		for(int y=V_Max(G_TruePixel(P[NYMin].y),0);y<V_Min(G_TruePixel(P[NYMax].y),E.SizeY);y++)
#else
//parcours toute la face selon y
		for(int y=G_TruePixel(P[NYMin].y);y<G_TruePixel(P[NYMax].y);y++)
#endif
		{
			//si on arrive sur un nouveau segment
			while (ygstop<=y)
			{
				BRES_SET(BXG,P[PntG[CurGPoint]].x);
				BRES_NEWADDTO(BXG,P[PntG[CurGPoint+1]].y-P[PntG[CurGPoint]].y,P[PntG[CurGPoint+1]].x-P[PntG[CurGPoint]].x);
#ifndef G_FASTCOPY
				BRES_CLIP(BXG,P[PntG[CurGPoint]].y);
#endif
#ifdef DEF_G_TEXTURE
				BRESXY_SET(TG,T[PntG[CurGPoint]]);
				BRESXY_NEWADDTO(TG,
					T[PntG[CurGPoint+1]].x-T[PntG[CurGPoint]].x,
					T[PntG[CurGPoint+1]].y-T[PntG[CurGPoint]].y,
					P[PntG[CurGPoint+1]].y-P[PntG[CurGPoint]].y);
#ifndef G_FASTCOPY
				BRESXY_CLIP(TG,P[PntG[CurGPoint]].y);
#endif
#endif
				//G_DrawLine(E,P[PntG[CurGPoint]].x+1,P[PntG[CurGPoint]].y,P[PntG[CurGPoint+1]].x+1,P[PntG[CurGPoint+1]].y-1,0x00ff0000);
				CurGPoint++;
				ygstop=G_TruePixel(P[PntG[CurGPoint]].y);
				//DbgCHECK(CurGPoint>CurWPointG,"G");
			}
			while (ydstop<=y)
			{
				BRES_SET(BXD,P[PntD[CurDPoint]].x);
				BRES_NEWADDTO(BXD,P[PntD[CurDPoint+1]].y-P[PntD[CurDPoint]].y,P[PntD[CurDPoint+1]].x-P[PntD[CurDPoint]].x);
#ifndef G_FASTCOPY
				BRES_CLIP(BXD,P[PntD[CurDPoint]].y);
#endif
#ifdef DEF_G_TEXTURE
				BRESXY_SET(TD,T[PntD[CurDPoint]]);
				BRESXY_NEWADDTO(TD,
					T[PntD[CurDPoint+1]].x-T[PntD[CurDPoint]].x,
					T[PntD[CurDPoint+1]].y-T[PntD[CurDPoint]].y,
					P[PntD[CurDPoint+1]].y-P[PntD[CurDPoint]].y);
#ifndef G_FASTCOPY
				BRESXY_CLIP(TD,P[PntD[CurDPoint]].y);
#endif
#endif
				//G_DrawLine(E,P[PntD[CurDPoint]].x-1,P[PntD[CurDPoint]].y,P[PntD[CurDPoint+1]].x-1,P[PntD[CurDPoint+1]].y-1,0x00ff00);
				CurDPoint++;
				ydstop=G_TruePixel(P[PntD[CurDPoint]].y);
				//DbgCHECK(CurGPoint>CurWPointD,"D");
			}
/*
#ifndef G_FASTCOPY
			if (((unsigned)y)<((unsigned)E.SizeY))
			{
#endif
*/
#ifdef DEF_G_TEXTURE
			
			if (V_Min(BRES_GET(BXD),E.SizeX)-V_Max(BRES_GET(BXG),0)>0)
			{
					BRESXY_START(
					TM,
					TG,
					TD,
					BRES_GET(BXD)-BRES_GET(BXG));
#ifndef G_FASTCOPY
				BRESXY_CLIP(TM,BRES_GET(BXG));
#endif
#endif
				

#ifndef G_FASTCOPY
				const int xStart=V_Max(G_TruePixel(BRES_GET(BXG)),0);
#ifdef DrawInclusive
				const int xStop=V_Min(G_TruePixel(BRES_GET(BXD)),E.SizeX-1);
#else
				const int xStop=V_Min(G_TruePixel(BRES_GET(BXD)),E.SizeX);
#endif
#pragma ivdep
#ifdef DrawInclusive
				for(int x=xStart;x<=xStop;x++)
#else
				for(int x=xStart;x<xStop;x++)
#endif
#else
				const int xStart=G_TruePixel(BRES_GET(BXG));
				const int xStop=G_TruePixel(BRES_GET(BXD));
#pragma ivdep
#ifdef DrawInclusive
				for(int x=xStart;x<=xStop;x++)
#else
				for(int x=xStart;x<xStop;x++)
#endif
#endif
				{
#ifdef DEF_G_TEXTURE
#ifdef DebugRenderSegment
SHORT DebugXTEX=BRESXY_GETX(TM);
SHORT DebugYTEX=BRESXY_GETY(TM);
SHORT DebugXECR=x;
SHORT DebugYECR=y;
/*
DebugXECR=V_Sature(DebugXECR,0,E.SizeX);
DebugYECR=V_Sature(DebugYECR,0,E.SizeY);
*/
SHORT DebugXTEXS=V_Sature(DebugXTEX,0,((1<<DescrTex.LarT)-1));
SHORT DebugYTEXS=V_Sature(DebugYTEX,0,((1<<DescrTex.LarT)-1));
#define DEF_G_RENDERCOUL (*(PixCoul*)(DescrTex.MemTex+3*((DebugYTEXS<<DescrTex.LarT)+DebugXTEXS)))
#else
//DWORD CoulTexture=*(DWORD*)&(DescrTex.MemTex[3*((BRESXY_GETY(TM)<<DescrTex.LarT)+BRESXY_GETX(TM))]);
#define DEF_G_RENDERCOUL (*(PixCoul*)(DescrTex.MemTex+3*((BRESXY_GETY(TM)<<DescrTex.LarT)+BRESXY_GETX(TM))))
#endif

#define DEF_G_FACELIGHT CouleurFaceLight
//CoulTexture
#else
#define DEF_G_RENDERCOUL Coul
#endif


///////////////////////////////////////////

//#include "InlineCombineColor.cpp"


////////////////////////////////////

//DO NOT COMPILE: INCLUDE FILE ONLY

/*
#if (BRES_POCT==2)
WORD ICC_Coul=G_Make16From24(DEF_G_RENDERCOUL);
#else
#define ICC_Coul DEF_G_RENDERCOUL
#endif
*/

/////////////////////////////////////


//#include "InlineBuildCoulC.cpp"



#ifndef DEF_G_POCT
#error "DEF_G_POCT non defini"
#endif
#ifndef DEF_G_RENDERMODE
#error "DEF_G_RENDERMODE non defini"
#endif
#ifdef ICC_Coul
#error "ICC_Coul defini"
#endif

#if (DEF_G_POCT==4)
#define ICC_Coul DEF_G_RENDERCOUL.Coul
#elif (DEF_G_POCT==3)
#define ICC_Coul DEF_G_RENDERCOUL.P24
#elif (DEF_G_POCT==2)
#if (DEF_G_RENDERMODE==3)
#define ICC_Coul DEF_G_RENDERCOUL
#else
WORD ICC_Coul=G_Make16From24(DEF_G_RENDERCOUL);
#endif
#elif (DEF_G_POCT==1)
#if (DEF_G_RENDERMODE==3)
#define ICC_Coul DEF_G_RENDERCOUL
#else
BYTE ICC_Coul=G_Make8From24(DEF_G_RENDERCOUL);
#endif
#else
#error "DEF_G_POCT errone"
#endif



/////////////////////////////////



#if (DEF_G_RENDERMODE==0)

#elif (DEF_G_RENDERMODE==1)

#if (DEF_G_POCT==4)
		MECR[x]=ICC_Coul;
#elif (DEF_G_POCT==3)
		*(PixCoul24* restrict )(MECR+DEF_G_POCT*x)=ICC_Coul;
#elif (DEF_G_POCT==2)
		((WORD* restrict )MECR)[x]=ICC_Coul;
#elif (DEF_G_POCT==1)
		MECR[x]=ICC_Coul;
#endif

//rendu avec masque
#elif (DEF_G_RENDERMODE==5)

#if (DEF_G_POCT==4)
		if((ICC_Coul&0xFFFFFF)!=(Mask.Coul&0xFFFFFF)) MECR[x]=ICC_Coul;
#elif (DEF_G_POCT==3)
		if((DEF_G_RENDERCOUL.Coul&0xFFFFFF)!=(Mask.Coul&0xFFFFFF)) *(PixCoul24* restrict )(MECR+DEF_G_POCT*x)=ICC_Coul;
#elif (DEF_G_POCT==2)
		if(ICC_Coul!=G_Make16From24(Mask)) ((WORD* restrict )MECR)[x]=ICC_Coul;
#elif (DEF_G_POCT==1)
		if(ICC_Coul!=G_Make8From24(Mask)) MECR[x]=ICC_Coul;
#endif

//rendu translucide
#elif  (DEF_G_RENDERMODE==2) 

#if (DEF_G_POCT==4)
		MECR[x]=(((ICC_Coul&0xfefefe)+(MECR[x]&0xfefefe))>>1);
#elif (DEF_G_POCT==3)
		{
		PixCoul ICC_V_R2;
		ICC_V_R2.Coul=(((DEF_G_RENDERCOUL.Coul&0xfefefe)+(((DWORD*)MECR)[x]&0xfefefe))>>1);
		*(PixCoul24*)(MECR+DEF_G_POCT*x)=ICC_V_R2.P24;
		}
#elif (DEF_G_POCT==2)
		//0b111101111011110=0b 0111 1011 1101 1110=7 b d e
		((WORD*)MECR)[x]=
			((
			(ICC_Coul&0x7bde)
			+(((WORD*)MECR)[x]&0x7bde)
			)>>1);
//ragma message("Message de debogage")
#elif (DEF_G_POCT==1)
		MECR[x]=(ICC_Coul+MECR[x])>>1;
#endif

//rendu avec lumiere
#elif  (DEF_G_RENDERMODE==3) 
//DEF_G_RENDERCOUL est la texture, comme ICC_Coul
//DEF_G_FACELIGHT est la lumiere
#if (DEF_G_POCT==4)
#ifdef UseAlphaPixCoul
		G_CombineFaceAndAlphaPixCoulLight((*(PixCoul*)(MECR+x)),DEF_G_RENDERCOUL,DEF_G_FACELIGHT);
#else
		G_CombineFaceAndLight((*(PixCoul*)(MECR+x)),DEF_G_RENDERCOUL,DEF_G_FACELIGHT);
#endif
		/*
		MECR[4*x]=(DEF_G_RENDERCOUL.B*DEF_G_FACELIGHT.A+DEF_G_FACELIGHT.B*(256-DEF_G_FACELIGHT.A))>>8;
		MECR[4*x+1]=(DEF_G_RENDERCOUL.V*DEF_G_FACELIGHT.A+DEF_G_FACELIGHT.V*(256-DEF_G_FACELIGHT.A))>>8;
		MECR[4*x+2]=(DEF_G_RENDERCOUL.R*DEF_G_FACELIGHT.A+DEF_G_FACELIGHT.R*(256-DEF_G_FACELIGHT.A))>>8;
		*/
#elif (DEF_G_POCT==3)
#ifdef UseAlphaPixCoul
		G_CombineFaceAndAlphaPixCoulLight((*(PixCoul24*)(MECR+3*x)),DEF_G_RENDERCOUL,DEF_G_FACELIGHT);
#else
		G_CombineFaceAndLight((*(PixCoul24*)(MECR+3*x)),DEF_G_RENDERCOUL,DEF_G_FACELIGHT);
#endif
		/*
		MECR[3*x]=(DEF_G_RENDERCOUL.B*DEF_G_FACELIGHT.A+DEF_G_FACELIGHT.B*(256-DEF_G_FACELIGHT.A))>>8;
		MECR[3*x+1]=(DEF_G_RENDERCOUL.V*DEF_G_FACELIGHT.A+DEF_G_FACELIGHT.V*(256-DEF_G_FACELIGHT.A))>>8;
		MECR[3*x+2]=(DEF_G_RENDERCOUL.R*DEF_G_FACELIGHT.A+DEF_G_FACELIGHT.R*(256-DEF_G_FACELIGHT.A))>>8;
		*/
#elif (DEF_G_POCT==2)
		PixCoul ICC_Temp;
		G_CombineFaceAndLight(ICC_Temp,DEF_G_RENDERCOUL,DEF_G_FACELIGHT);
		((WORD*)MECR)[x]=G_Make16From24(ICC_Temp);
#elif (DEF_G_POCT==1)
		PixCoul ICC_Temp;
		G_CombineFaceAndLight(ICC_Temp,DEF_G_RENDERCOUL,DEF_G_FACELIGHT);
		MECR[x]=G_Make8From24(ICC_Temp);
#endif

#endif

#undef CoulC
#undef ICC_Coul
#undef DEF_G_RENDERCOUL
#undef DEF_G_FACELIGHT
#undef DEF_G_RENDERMODE

//////////////////////////////////////////


///////////////////////////////////////////

#ifdef DEF_G_TEXTURE
			BRESXY_NEXT(TM);
#endif
				}

#ifdef DEF_G_TEXTURE
			}
#endif
/*
#ifndef G_FASTCOPY
			}
#endif
*/
#undef G_FASTCOPY
			BRES_NEXT(BXG);
			BRES_NEXT(BXD);
#ifdef DEF_G_TEXTURE
			BRESXY_NEXT(TG);
			BRESXY_NEXT(TD);
#endif
#if (DEF_G_POCT==4) 
			(*(int*)&MECR)+=E.Pitch;
#else
			MECR+=E.Pitch;
#endif
		}
#ifdef DrawInclusive
		/*
		derniere ligne
		*/
#endif
#undef DrawInclusive
	}
}

#ifdef DebugGraphicsTimer
	S_StopTimer(Global.T_InlineRenderSegment);
#endif

#undef DEF_G_POCT

#pragma warning( default : 4701 )

#else
#error Configuration error
#endif

