
////////////////////////////////////


//DO NOT COMPILE: INCLUDE FILE ONLY
#ifdef SPG_General_USEGraphicsRenderPoly


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

#else
#error Configuration error
#endif

