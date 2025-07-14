
#ifdef SPG_General_USEGraphics


#ifndef SPG_General_USEWindows
typedef struct tagRGBQUAD {
 BYTE rgbBlue;
 BYTE rgbGreen;
 BYTE rgbRed;
 BYTE rgbReserved;
} RGBQUAD;
typedef struct tagBITMAPCOREHEADER {
 DWORD bcSize;
 WORD bcWidth;
 WORD bcHeight;
 WORD bcPlanes;
 WORD bcBitCount;
} BITMAPCOREHEADER, *LPBITMAPCOREHEADER, *PBITMAPCOREHEADER;
typedef struct tagBITMAPINFOHEADER{
 DWORD biSize;
 LONG biWidth;
 LONG biHeight;
 WORD biPlanes;
 WORD biBitCount;
 DWORD biCompression;
 DWORD biSizeImage;
 LONG biXPelsPerMeter;
 LONG biYPelsPerMeter;
 DWORD biClrUsed;
 DWORD biClrImportant;
} BITMAPINFOHEADER, *LPBITMAPINFOHEADER, *PBITMAPINFOHEADER;
typedef struct tagBITMAPINFO {
 BITMAPINFOHEADER bmiHeader;
 RGBQUAD bmiColors[1];
} BITMAPINFO, *LPBITMAPINFO, *PBITMAPINFO;
#endif

#define UseAlphaPixCoulLight

//#define G_PITCH_EQU_WIDTH 1

#define G_MEMORYAVAILABLE 1
//G_MEMORYAVAILABLE est appele a devenir E->Lock()
//commandes à fixer soi meme pour G_InitEcran
#define G_ALLOC_MEMOIRE 2
#define G_ALIAS_MEMOIRE 4
#ifdef SPG_General_USEWindows
#define G_ECRAN_SETDIB 8
#define G_ECRAN_SETDIBBITSTODEVICE G_ECRAN_SETDIB
#define G_ECRAN_DIB G_ECRAN_SETDIB
#ifndef G_NODRAWDIB
#define G_ECRAN_DRAWDIB 16
#endif
#define G_ECRAN_DIBSECT 32 
#define G_ECRAN_DIBSECTION G_ECRAN_DIBSECT
#define G_ECRAN_CBMP 64
#endif

#define G_ECRAN_PGL 128
#ifdef SPG_General_USEWindows
#define G_ECRAN_USEGDI	256
#endif

//autres

#define G_STANDARDW G_ECRAN_SETDIB|G_ALLOC_MEMOIRE
//#define ECRAN_DDRAW 3

typedef struct
{
	int Etat;
	BYTE* restrict MECR;//pointeur de memoire ecran
	int Pitch;
	int POCT;//octets/pixel 1 2 3(24bits) ou 4(32bits)
	int SizeX;
	int SizeY;
	int PosX;
	int PosY;

	//divers pratique
	int Size;//=POCT*SizeX*SizeY

	//etat|=G_ECRAN_WINDOWS ecran type window avec bitmapinfo, hdc et hwnd
	void* HECR;
	void* bmpinf;

	union
	{
		void* hCompatDC;//DibSection,Compatible bitmap
		void* hDrawDib;//DrawDib
#ifdef SPG_General_PGLib
		PGLSurface* surface;
#endif
	};
	union
	{
		void* DibSect;//DibSection
		void* CompatBMP;//Compatible bitmap
	};
	void* OldSelect;//DibSection,Compatible bitmap
} G_Ecran;

typedef struct
{
	BYTE B;
	union
	{
		BYTE V;
		BYTE G;
	};
	BYTE R;
} PixCoul24;

typedef struct
{
union
{
DWORD Coul;
	struct
	{
		union
		{
			struct
			{
			BYTE B;
			union
			{
				BYTE V;
				BYTE G;
			};
			BYTE R;
			};
			PixCoul24 P24;
		};
		BYTE A;
	};
};
} PixCoul;

typedef WORD PixCoul16;

typedef BYTE PixCoul8;

#define G_NORMAL_ALPHA 0
#define G_MAX_ALPHA 255

#ifdef UseAlphaPixCoulLight

typedef struct
{
	WORD AB;
	WORD AV;
	WORD AR;
	WORD InvA;
} AlphaPixCoul;

#endif

typedef struct
{
union
	{
	DWORD xy;
	struct
		{
	SHORT x;
	SHORT y;
		};
	};
} G_PixCoord;

typedef G_PixCoord G_TexCoord;

typedef struct
{
BYTE * MemTex;
DWORD LarT;
} G_Texture;

typedef struct
{
	int X0;
	int Y0;
	int X1;
	int Y1;
	LONG Coul1;
	LONG Coul2;
	float *ValeurAMesurer;
	float VMin;
	float VMax;
} G_Jauge;

#define DRAWVECTCOLOR 1024
#define DRAWVECTRAMP 5
//#define DRAWVECTRAMP 6
#define DRAWVECTLEN	((DRAWVECTCOLOR+DRAWVECTRAMP-1)/DRAWVECTRAMP)

typedef struct
{
	float DrawScale;
	float ColorScale;
	PixCoul Color[DRAWVECTCOLOR];
} DRAWVECT;

/*
#define G_SubPixelLevel 0
#define G_SubPixelValue 1
#define G_TruePixel(x) (x>>G_SubPixelLevel)
#define G_SubPixel(x) (x<<G_SubPixelLevel)
#define G_TrueTexel(x) (x)
#define G_SubTexel(x) (x)
*/
#define G_TruePixel(x) (x)
#define G_SubPixel(x) (x)
#define G_TrueTexel(x) (x)
#define G_SubTexel(x) (x)

#define Pixel(M,P,poct,x,y) (*(PixCoul*)(M+poct*x+y*P))
#define PixelPTR(M,P,poct,x,y) (M+poct*x+y*P)
#define PixEcr(E,x,y) (*(PixCoul*)(E.MECR+E.POCT*x+E.Pitch*y))
#define PixEcrSafe(E,x,y) PixEcr(E,(V_Sature(x,0,(G_SizeX(E)-1))),(V_Sature(y,0,(G_SizeY(E)-1))))
#define PixEcrPTR(E,x,y) (E.MECR+E.POCT*x+E.Pitch*y)
#define G_Etat(E) E.Etat
#define G_GetPix(E) E.MECR
#define G_MECR(E) E.MECR
#define G_SizeX(E) E.SizeX
#define G_SizeY(E) E.SizeY
#define G_PosX(E) E.PosX
#define G_PosY(E) E.PosY
#define G_Pitch(E) E.Pitch
#define G_POCT(E) E.POCT
#define G_IsInEcran(E,AbsoluteX,AbsoluteY) (V_IsBound(AbsoluteX,E.PosX,E.PosX+E.SizeX)&&V_IsBound(AbsoluteY,E.PosY,E.PosY+E.SizeY))
//#define G_InEcr(P,E) (V_IsBound(P.x,0,G_SubPixel(E.SizeX))&&V_IsBound(P.y,0,G_SubPixel(E.SizeY)))
#define G_InEcr(P,E) (((unsigned)P.x)<((unsigned)G_SubPixel(E.SizeX)))&&(((unsigned)P.y)<((unsigned)G_SubPixel(E.SizeY)))
#define G_Make16From24(Coul) (WORD)((((*(PixCoul*)(&Coul)).R>>3)<<10)|(((*(PixCoul*)(&Coul)).V>>3)<<5)|((*(PixCoul*)(&Coul)).B>>3))
#define G_Make24From16(Coul) ( ( ((DWORD)(*(WORD*)&Coul)&0x7C00)<<9 ) | ( ((DWORD)(*(WORD*)&Coul)&0x03E0)<<6 ) | ( ((DWORD)(*(WORD*)&Coul)&0x001F)<<3 ) )
#define G_Make8From24(Coul) (BYTE)((((BYTE*)&(Coul))[0]+((BYTE*)&(Coul))[1]+((BYTE*)&(Coul))[1]+((BYTE*)&(Coul))[2])>>2)
#define G_Make24From8(Coul) ((DWORD)(*(BYTE*)&Coul)|(DWORD)(*(BYTE*)&Coul)<<8|(DWORD)(*(BYTE*)&Coul)<<16)
#define G_MakeCompatibleFrom24(E, Coul, Dest) {if (E.POCT==4) {*((PixCoul24*)&Dest)=(*(PixCoul24*)&Coul);(*(PixCoul*)&Dest).A=0;} else if (E.POCT==3) *((PixCoul24*)&Dest)=*((PixCoul24*)&Coul);else if (E.POCT==2) Dest=G_Make16From24(Coul);else /*if (E.POCT==1)*/ {DWORD G_MACRO_C=Coul;Dest=G_Make8From24(G_MACRO_C);}}
#define G_Make24(E, Coul, Dest) {if (E.POCT==4) {*((PixCoul24*)&Dest)=(*(PixCoul24*)&Coul);(*(PixCoul*)&Dest).A=0;} else if (E.POCT==3) *((PixCoul24*)&Dest)=*((PixCoul24*)&Coul);else if (E.POCT==2) Dest=G_Make24From16(Coul);else /*if (E.POCT==1)*/ {Dest=G_Make24From8(Coul);}}
#define G_MakeCompatibleFromP24(E, PCoul, Dest) {if (E.POCT==4) {*((PixCoul24*)&Dest)=(*(PixCoul24*)PCoul);(*(PixCoul*)&Dest).A=0;} else if (E.POCT==3) *((PixCoul24*)&Dest)=*((PixCoul24*)PCoul);else if (E.POCT==2) Dest=G_Make16From24(*(PixCoul24*)PCoul);else /*if (E.POCT==1)*/ {PixCoul24 G_MACRO_C=*(PixCoul24*)PCoul;Dest=G_Make8From24(G_MACRO_C);}}
#define G_MakePixCoulFromFloat(Dest,fR,fG,fB) Dest.R=V_Sature(256.0*(fR),0,255.0);Dest.V=V_Sature(256.0*(fG),0,255.0);Dest.B=V_Sature(256.0*(fB),0,255.0)
#define G_MakeRGBFromFloat(Dest,fR,fG,fB) G_MakePixCoulFromFloat((*(PixCoul*)(&Dest)),fR,fG,fB)

#define G_CombineFaceAndLightChannel(CDest,CFace,CLight,ALPHA) CDest=(BYTE)(((CFace<<8)+ALPHA*(CLight-CFace))>>8);
#define G_CombineFaceAndLight(CDest,CFace,CLight) G_CombineFaceAndLightChannel(CDest.B,CFace.B,CLight.B,CLight.A);G_CombineFaceAndLightChannel(CDest.V,CFace.V,CLight.V,CLight.A);G_CombineFaceAndLightChannel(CDest.R,CFace.R,CLight.R,CLight.A)
//CDest.B=(BYTE)(((CFace.B<<8)+CLight.A*(CLight.B-CFace.B))>>8);CDest.V=(BYTE)(((CFace.V<<8)+CLight.A*(CLight.V-CFace.V))>>8);CDest.R=(BYTE)(((CFace.R<<8)+CLight.A*(CLight.R-CFace.R))>>8)

#define G_BlendColorChannel(CDest,CFace,ALPHA) CDest=((CFace<<8)+ALPHA*(255-CFace))>>8;
#define G_BlendColor(CDest,CFace,ALPHA) G_BlendColorChannel(CDest.B,CFace.B,ALPHA);G_BlendColorChannel(CDest.V,CFace.V,ALPHA);G_BlendColorChannel(CDest.R,CFace.R,ALPHA)
#define G_FadeColorChannel(CDest,CFace,ALPHA) CDest=(ALPHA*CFace)>>8;
#define G_FadeColor(CDest,CFace,ALPHA) G_FadeColorChannel(CDest.B,CFace.B,ALPHA);G_FadeColorChannel(CDest.V,CFace.V,ALPHA);G_FadeColorChannel(CDest.R,CFace.R,ALPHA)

#define G_FullLightColor(CDest,CFace,ALPHA) {if(ALPHA>0) {G_BlendColor(CDest,CFace,ALPHA);} else if (ALPHA<0) {int UnMALPHA=256+ALPHA; G_FadeColor(CDest,CFace,UnMALPHA);} else CDest=CFace;}

#ifdef UseAlphaPixCoulLight
#define G_ComputeAlphaPixCoulLight(APCL,CLight) APCL.AB=CLight.A*CLight.B;APCL.AV=CLight.A*CLight.V;APCL.AR=CLight.A*CLight.R;APCL.InvA=255-CLight.A
#define G_CombineFaceAndAlphaPixCoulLight(CDest,CFace,CLight) CDest.B=(BYTE)((CFace.B*CLight.InvA+CLight.AB)>>8);CDest.V=(BYTE)((CFace.V*CLight.InvA+CLight.AV)>>8);CDest.R=(BYTE)((CFace.R*CLight.InvA+CLight.AR)>>8)
#endif

#define G_Interpole(C_I_Dest,C_I_1,C_I_2,coeff) C_I_Dest.B=C_I_1.B*coeff+C_I_2.B*(1-coeff);C_I_Dest.V=C_I_1.V*coeff+C_I_2.V*(1-coeff);C_I_Dest.R=C_I_1.R*coeff+C_I_2.R*(1-coeff)

#define G_Combine(CDest,C1,C2) {CDest.A=V_Max(C1.A,C2.A); WORD ForDiv=(WORD)65535/CDest.A; {WORD G_C_B=(WORD)(((C1.B*C1.A+C2.B*C2.A)*ForDiv)>>16); CDest.B=V_SatureUP(G_C_B,255);} {WORD G_C_V=(WORD)(((C1.V*C1.A+C2.V*C2.A)*ForDiv)>>16); CDest.V=V_SatureUP(G_C_V,255);} {WORD G_C_R=(WORD)(((C1.R*C1.A+C2.R*C2.A)*ForDiv)>>16); CDest.R=V_SatureUP(G_C_R,255);}}

#define G_DrawPixelMacro(E,x,y,Coul) {int X=x;int Y=y;if (V_IsBound(X,0,E.SizeX)&&V_IsBound(Y,0,E.SizeY)) memcpy(E.MECR+E.POCT*X+E.Pitch*Y,&Coul,E.POCT);}

#ifdef SPG_General_PGLib
#define G_LockEcran(E) {CHECK(E.surface==0,"surface inexistante",return);CHECK(E.Etat&G_MEMORYAVAILABLE,"Surface deja lockee",return);E.MECR=pglLockSurface(E.surface);E.Pitch=E.surface->pitch;E.Etat|=G_MEMORYAVAILABLE;}
#define G_UnlockEcran(E) {CHECK(E.surface==0,"surface inexistante",return);CHECK((E.Etat&G_MEMORYAVAILABLE)==0,"Surface deja unlockee",return);pglUnlockSurface(E.surface);E.Etat&=~G_MEMORYAVAILABLE;}
#else
#define G_LockEcran(E)
#define G_UnlockEcran(E)
#endif

#define G_DrawPixelInt(E,X,Y,Coul) G_DrawPixel(E,V_FloatToInt(X),V_FloatToInt(Y),Coul)
#define G_DrawLineInt(E,X0,Y0,X1,Y1,Coul) G_DrawLine(E,V_FloatToInt(X0),V_FloatToInt(Y0),V_FloatToInt(X1),V_FloatToInt(Y1),Coul);
#define G_DrawRectInt(E,X0,Y0,X1,Y1,Coul) G_DrawRect(E,V_FloatToInt(X0),V_FloatToInt(Y0),V_FloatToInt(X1),V_FloatToInt(Y1),Coul);

#include "SPG_Graphics.agh"
#include "SPG_Graphics_Render.agh"
#ifdef SPG_General_USEGraphicsRenderPoly
#include "SPG_Graphics_RenderPoly.agh"
#endif

/*
int G_InitEcran(Ecran& E, int EType, BYTE* MECR, int Pitch, int POCT, int SizeX, int SizeY, int PosX, int PosY, HDC HDCECR);

int G_InitEcranWindows(Ecran& E, int POCT, int SizeX, int SizeY, HDC HDCECR);
int G_InitSousEcran(Ecran& E, Ecran& ERef,int PosX,int PosY, int SizeX,int SizeY);

void G_BlitEcran(Ecran& E);
void G_SaveToBMP(char * Name,BYTE* M, int POCT, int Pitch, int SizeX, int SizeY);
void G_DelEcran(Ecran& E);
void G_DrawLine(Ecran& E,int X0,int Y0,int X1,int Y1,DWORD Coul);
void G_DrawRect(Ecran& E,int X0, int Y0, int X1, int Y1, DWORD Coul);
void G_DrawOutRect(Ecran& E,int X0, int Y0, int X1, int Y1, DWORD Coul);
void G_InitJauge(Ecran& E,G_Jauge& J, float*ValeurAMesurer, float VMax, int Y0, DWORD Coul1, DWORD Coul2);
void G_DrawJauge(Ecran& E,G_Jauge& J);
*/


#endif

