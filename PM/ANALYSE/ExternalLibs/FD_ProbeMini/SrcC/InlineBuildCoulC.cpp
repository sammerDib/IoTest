

//DO NOT COMPILE: INCLUDE FILE ONLY
//#ifdef SPG_General_USEGraphicsRenderPoly


/*
#if (DEF_G_POCT==2)
WORD ICC_Coul=G_Make16From24(DEF_G_RENDERCOUL);
#else
#define ICC_Coul DEF_G_RENDERCOUL
#endif
*/

//#pragma message("InlineBuildCoulC include")

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

//#endif

