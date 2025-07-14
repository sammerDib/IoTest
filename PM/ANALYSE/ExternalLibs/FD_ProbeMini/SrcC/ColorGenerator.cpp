
#include "SPG_General.h"

#ifdef SPG_General_USEColorGenerator

#include "SPG_Includes.h"

#include <stdlib.h>

//#define TestMaxAlias

/*
//genere une couleur aleatoire
DWORD SPG_CONV CG_RandColor(ColorG& C)
{
	PixCoul Couleur;

	float A1=2.0*(float)rand()/RAND_MAX-1;

	{
	float A2=2.0*(float)rand()/RAND_MAX-1;
	int Rouge=C.Moyenne.R+
			C.Correl.R*A1+
			C.Aleat.R*A2;

	Couleur.R=V_Sature(Rouge,0,255);
	}

	{
	float A2=2.0*(float)rand()/RAND_MAX-1;
	int Vert=C.Moyenne.V+
			C.Correl.V*A1+
			C.Aleat.V*A2;

	Couleur.V=V_Sature(Vert,0,255);
	}

	{
	float A2=2.0*(float)rand()/RAND_MAX-1;
	int Bleu=C.Moyenne.B+
			C.Correl.B*A1+
			C.Aleat.B*A2;

	Couleur.B=V_Sature(Bleu,0,255);
	}

	Couleur.A=G_NORMAL_ALPHA;

	return Couleur.Coul;
}
*/

/*
PixCoul SPG_CONV CG_RandColor(ColorG& C)
{
	PixCoul Couleur;

	SHORT A1=(rand()&511)-256;

	{
	SHORT A2=(rand()&511)-256;
	int Rouge=C.Moyenne.R+
			((C.Correl.R*A1)>>8)+
			((C.Aleat.R*A2)>>8);

	Couleur.R=V_Sature(Rouge,0,255);
	}

	{
	SHORT A2=(rand()&511)-256;
	int Vert=C.Moyenne.V+
			((C.Correl.V*A1)>>8)+
			((C.Aleat.V*A2)>>8);

	Couleur.V=V_Sature(Vert,0,255);
	}

	{
	SHORT A2=(rand()&511)-256;
	int Bleu=C.Moyenne.B+
			((C.Correl.B*A1)>>8)+
			((C.Aleat.B*A2)>>8);

	Couleur.B=V_Sature(Bleu,0,255);
	}

	Couleur.A=G_NORMAL_ALPHA;

	return Couleur;
}
*/
#ifdef SPG_General_USESGRAPH

//remplit une texture avec un motif aleatoire
void SPG_CONV CG_FillRandTex(SG_FullTexture& T, ColorG& C, int SizeX, int SizeY)
{
	G_Ecran ETex;
#ifdef SPG_General_PGLib
	SG_SetToNullTex(T);
	PGLSurface*surface=pglCreateSurface(Global.display,SizeX,SizeY,32,0);
	CHECK(surface==0,"CG_FillRandTex: pglCreateSurface echoue",return);
	G_InitEcranFromPGLSurface(ETex,surface);
	G_LockEcran(ETex);
#else
	if (SG_CreateTexture(T,SizeX,SizeY)==0) return;
	SG_InitEcranFromTexture(ETex,T);
#endif

/*
X 0 Y 0
0 0 0 0
Y 0 Y 0
0 0 0 0

*/



	CHECK(ETex.Etat==0,"SG_FillRandTex: SG_InitEcranFromTexture echoue",return);
#ifdef TestMaxAlias
	{
	for(int y=0;y<ETex.SizeY;y++)
	{
	for(int x=0;x<ETex.SizeX;x++)
	{
		DWORD H=0;
		//if((x%2)==0) H|=0x80;
		if((x%2)==0) H|=0x80;
		//if((x%3)==0) H|=0x8000;
		//if((x%4)<2)  H|=0x800000;
		//if((x%5)<3) H|=0x404040;
		if((y%2)==0) H|=0x8000;
		//if((y%3)==0) H|=0x8000;
		//if((y%4)<2)  H|=0x800000;
		//if((y%5)<3) H|=0x404040;
		//G_DrawPixel(ETex,x,y,V_BitReverse(x,3)*31+((V_BitReverse(y,3)*31)<<8)+((V_BitReverse(x+y,3)*31)<<16));
		G_DrawPixel(ETex,x,y,H);
	}
	}
	G_SaveAs_ToBMP(ETex);
	}
#else

#define E ETex

if(E.POCT==4)
	for(int y=0;y<ETex.SizeY;y++)
	{
		DWORD* MECR=(DWORD*)(E.MECR+y*E.Pitch);
		for(int x=0;x<ETex.SizeX;x++)
		{
			// *MECR=CG_RandColor(C).Coul;
			CG_RandColor((*(PixCoul*)MECR),C);
			MECR++;
		}

	}
else if (E.POCT==3)
	for(int y=0;y<ETex.SizeY;y++)
	{
		PixCoul24* MECR=(PixCoul24*)(E.MECR+y*E.Pitch);
		for(int x=0;x<ETex.SizeX;x++)
		{
			// *MECR=CG_RandColor(C).P24;
			CG_RandColor24((*MECR),C);
			MECR++;
		}

	}
#undef E
else
{
	for(int y=0;y<ETex.SizeY;y++)
	{
	for(int x=0;x<ETex.SizeX;x++)
	{
		PixCoul PXC;
		CG_RandColor(PXC,C);
		G_DrawPixel(ETex,x,y,PXC.Coul);
	}
	}
}
#endif

#ifdef SPG_General_PGLib
	G_UnlockEcran(ETex);
	T=pglCreateTexture(PGL_RGB8,surface,PGL_LINEAR);
#endif

	G_CloseEcran(ETex);//==pglDestroySurface(surface)
	return;
}

//remplit une texture avec un motif de bandes de 4 couleurs
void SPG_CONV CG_FillRandModuleXTex(SG_FullTexture& T, ColorG& C1,ColorG& C2,ColorG& C3,ColorG& C4, int SizeX, int SizeY)
{
	G_Ecran ETex;
#ifdef SPG_General_PGLib
	SG_SetToNullTex(T);
	PGLSurface*surface=pglCreateSurface(Global.display,SizeX,SizeY,32,0);
	CHECK(surface==0,"CG_FillRandTex: pglCreateSurface echoue",return);
	G_InitEcranFromPGLSurface(ETex,surface);
	G_LockEcran(ETex);
#else
	if (SG_CreateTexture(T,SizeX,SizeY)==0) return;
	SG_InitEcranFromTexture(ETex,T);
#endif

	CHECK(ETex.Etat==0,"SG_FillRandTex: SG_InitEcranFromTexture echoue",return);



	/*
	for(int y=0;y<ETex.SizeY;y++)
	{
	for(int x=0;x<ETex.SizeX;x+=4)
	{
		PixCoul PXC1;
		PixCoul PXC2;
		PixCoul PXC3;
		PixCoul PXC4;
		CG_RandColor(PXC1,C1);
		CG_RandColor(PXC2,C2);
		CG_RandColor(PXC3,C3);
		CG_RandColor(PXC4,C4);
		G_DrawPixel(ETex,x,y,PXC1.Coul);
		G_DrawPixel(ETex,x+1,y,PXC2.Coul);
		G_DrawPixel(ETex,x+2,y,PXC3.Coul);
		G_DrawPixel(ETex,x+3,y,PXC4.Coul);
	}
	}
	*/
#define E ETex

if(E.POCT==4)
	for(int y=0;y<ETex.SizeY;y++)
	{
		DWORD* MECR=(DWORD*)(E.MECR+y*E.Pitch);
		for(int x=0;x<ETex.SizeX;x+=4)
		{
			// *MECR=CG_RandColor(C).Coul;
			CG_RandColor((*(PixCoul*)MECR),C1);
			MECR++;
			CG_RandColor((*(PixCoul*)MECR),C2);
			MECR++;
			CG_RandColor((*(PixCoul*)MECR),C3);
			MECR++;
			CG_RandColor((*(PixCoul*)MECR),C4);
			MECR++;
		}

	}
else if (E.POCT==3)
	for(int y=0;y<ETex.SizeY;y++)
	{
		PixCoul24* MECR=(PixCoul24*)(E.MECR+y*E.Pitch);
		for(int x=0;x<ETex.SizeX;x+=4)
		{
			// *MECR=CG_RandColor(C).P24;
			CG_RandColor24((*MECR),C1);
			MECR++;
			CG_RandColor24((*MECR),C2);
			MECR++;
			CG_RandColor24((*MECR),C3);
			MECR++;
			CG_RandColor24((*MECR),C4);
			MECR++;
		}

	}
#undef E
else
{
	for(int y=0;y<ETex.SizeY;y++)
	{
	for(int x=0;x<ETex.SizeX;x+=4)
	{
		PixCoul PXC1;
		PixCoul PXC2;
		PixCoul PXC3;
		PixCoul PXC4;
		CG_RandColor(PXC1,C1);
		CG_RandColor(PXC2,C2);
		CG_RandColor(PXC3,C3);
		CG_RandColor(PXC4,C4);
		G_DrawPixel(ETex,x,y,PXC1.Coul);
		G_DrawPixel(ETex,x+1,y,PXC2.Coul);
		G_DrawPixel(ETex,x+2,y,PXC3.Coul);
		G_DrawPixel(ETex,x+3,y,PXC4.Coul);
	}
	}
}






#ifdef SPG_General_PGLib
	G_UnlockEcran(ETex);
	T=pglCreateTexture(PGL_RGB8,surface,PGL_LINEAR);//PGL_NEAREST);
#endif

	G_CloseEcran(ETex);//==pglDestroySurface(surface)
	return;
}

//recupere des coordonnes de texture aleatoires a l'interieur d'une texture pour texturer une face
void SPG_CONV CG_RandTexCoord(int TextureSizeX,int TextureSizeY,
				  SHORT&XT1,SHORT&YT1,
				  SHORT&XT2,SHORT&YT2,
				  SHORT&XT3,SHORT&YT3,
				  SHORT&XT4,SHORT&YT4)
{
	float AX=(float)((float)rand()/RAND_MAX-0.5);
	float AY=(float)((float)rand()/RAND_MAX-0.5);
	float Teta=(float)(V_2PI*(float)rand()/RAND_MAX);
	
	float B1=(float)((float)rand()/RAND_MAX-0.5);
	float B2=(float)((float)rand()/RAND_MAX-0.5);
	float B3=(float)((float)rand()/RAND_MAX-0.5);
	float B4=(float)((float)rand()/RAND_MAX-0.5);
	int XCenter=V_FloatToInt(TextureSizeX*(1.0+AX)/2);
	int YCenter=V_FloatToInt(TextureSizeY*(1.0+AY)/2);
	
	float CT=cos(Teta);
	float ST=sin(Teta);
/*
	if (TextureSizeX!=TextureSizeY)
	{
	ST=CT=0;
	}
*/
	float R1=(float)(TextureSizeX*0.25*(1+0.2*B1));
	float R2=(float)(TextureSizeX*0.25*(1+0.2*B2));
	float R3=(float)(TextureSizeX*0.25*(1+0.2*B3));
	float R4=(float)(TextureSizeX*0.25*(1+0.2*B4));
	
	XT1=V_FloatToShort(XCenter+R1*CT);
	YT1=V_FloatToShort(YCenter+R1*ST);
	
	XT2=V_FloatToShort(XCenter+R2*ST);
	YT2=V_FloatToShort(YCenter-R2*CT);
	
	XT3=V_FloatToShort(XCenter-R3*CT);
	YT3=V_FloatToShort(YCenter-R3*ST);
	
	XT4=V_FloatToShort(XCenter-R4*ST);
	YT4=V_FloatToShort(YCenter+R4*CT);
	
	XT1=(SHORT)V_Sature(XT1,0,TextureSizeX-1);
	YT1=(SHORT)V_Sature(YT1,0,TextureSizeY-1);
	XT2=(SHORT)V_Sature(XT2,0,TextureSizeX-1);
	YT2=(SHORT)V_Sature(YT2,0,TextureSizeY-1);
	XT3=(SHORT)V_Sature(XT3,0,TextureSizeX-1);
	YT3=(SHORT)V_Sature(YT3,0,TextureSizeY-1);
	XT4=(SHORT)V_Sature(XT4,0,TextureSizeX-1);
	YT4=(SHORT)V_Sature(YT4,0,TextureSizeY-1);
	return;
}

//recupere des coordonnes de texture aleatoires avec un rapport d'aspect specifie
void SPG_CONV CG_RandFlatTexCoord(int TextureSizeX,int TextureSizeY,
				  SHORT&XT1,SHORT&YT1,
				  SHORT&XT2,SHORT&YT2,
				  SHORT&XT3,SHORT&YT3,
				  SHORT&XT4,SHORT&YT4,
				  float FlatFact)
{
	float AX=(float)((float)rand()/RAND_MAX-0.5);
	float AY=(float)((float)rand()/RAND_MAX-0.5);
	float Teta=(float)(V_2PI*(float)rand()/RAND_MAX);
	
	float B1=(float)((float)rand()/RAND_MAX-0.5);
//	float B2=(float)rand()/RAND_MAX-0.5;
//	float B3=(float)rand()/RAND_MAX-0.5;
//	float B4=(float)rand()/RAND_MAX-0.5;
	int XCenter=V_FloatToInt(TextureSizeX*(1.0+AX)/2.0);
	int YCenter=V_FloatToInt(TextureSizeY*(1.0+AY)/2.0);
	
	float CT=cos(Teta);
	float ST=sin(Teta);
	float CTf=cos(Teta+0.5f*(float)V_PI*FlatFact);
	float STf=sin(Teta+0.5f*(float)V_PI*FlatFact);
/*
	if (TextureSizeX!=TextureSizeY)
	{
	ST=CT=0;
	STf=CTf=0;
	}
*/
	float R1=(float)(TextureSizeX*0.25*(1+0.2*B1*FlatFact));
	float R2=R1;//TextureSizeX*0.25*(1+0.2*B2*FlatFact);
	float R3=R2;//TextureSizeX*0.25*(1+0.2*B3*FlatFact);
	float R4=R3;//TextureSizeX*0.25*(1+0.2*B4*FlatFact);

	XT1=V_FloatToShort(XCenter+R1*CT);
	YT1=V_FloatToShort(YCenter+R1*ST);
	
	XT1=V_FloatToShort(XCenter+R2*CTf);
	YT1=V_FloatToShort(YCenter+R2*STf);
	
	XT3=V_FloatToShort(XCenter-R3*CT);
	YT3=V_FloatToShort(YCenter-R3*ST);
	
	XT4=V_FloatToShort(XCenter-R4*CTf);
	YT4=V_FloatToShort(YCenter-R4*STf);
	
	XT1=(SHORT)V_Sature(XT1,0,TextureSizeX-1);
	YT1=(SHORT)V_Sature(YT1,0,TextureSizeY-1);
	XT2=(SHORT)V_Sature(XT2,0,TextureSizeX-1);
	YT2=(SHORT)V_Sature(YT2,0,TextureSizeY-1);
	XT3=(SHORT)V_Sature(XT3,0,TextureSizeX-1);
	YT3=(SHORT)V_Sature(YT3,0,TextureSizeY-1);
	XT4=(SHORT)V_Sature(XT4,0,TextureSizeX-1);
	YT4=(SHORT)V_Sature(YT4,0,TextureSizeY-1);
	return;
}

//aussi mais tient compte du raboutement du motif
void SPG_CONV CG_RandModuleXTexCoord(int TextureSizeX,int TextureSizeY,
				  SHORT&XT1,SHORT&YT1,
				  SHORT&XT2,SHORT&YT2,
				  SHORT&XT3,SHORT&YT3,
				  SHORT&XT4,SHORT&YT4)
{
	float B1=(float)rand()/RAND_MAX;
	float B2=(float)rand()/RAND_MAX;
	float B3=(float)rand()/RAND_MAX;
//	float B4=(float)rand()/RAND_MAX;
	
	float T=(float)rand()/RAND_MAX;
	
	XT1=(SHORT)(4*(V_Floor((B1*TextureSizeX)/8)));
	YT1=(SHORT)((.8f*T+0.1f*B2)*TextureSizeY);
	
	XT2=XT1;
	YT2=(SHORT)((.8f*T+0.1+0.1f*B3)*TextureSizeY);
	
	XT3=(SHORT)(XT1+TextureSizeX/2);
	YT3=YT2;
	
	XT4=XT3;
	YT4=YT1;
	
	XT1=(SHORT)V_Sature(XT1,0,TextureSizeX-1);
	YT1=(SHORT)V_Sature(YT1,0,TextureSizeY-1);
	XT2=(SHORT)V_Sature(XT2,0,TextureSizeX-1);
	YT2=(SHORT)V_Sature(YT2,0,TextureSizeY-1);
	XT3=(SHORT)V_Sature(XT3,0,TextureSizeX-1);
	YT3=(SHORT)V_Sature(YT3,0,TextureSizeY-1);
	XT4=(SHORT)V_Sature(XT4,0,TextureSizeX-1);
	YT4=(SHORT)V_Sature(YT4,0,TextureSizeY-1);
	return;
}

//definit entierement une face aleatoire
void SPG_CONV CG_DefRandFce(SG_FACE &F,
			   SG_PNT3D *p1,SG_PNT3D *p2,SG_PNT3D *p3,SG_PNT3D *p4,
			   ColorG& coul,int style,SG_FullTexture& T)
{

	if (!SG_IsValidTex(T))
	{
		PixCoul CGRC;
		CG_RandColor(CGRC,coul);
		SG_DefUniSFce(F,p1,p2,p3,p4,CGRC.Coul,(style&SG_MASKUNI));
	}
		else
	{
	F.NumP1 = p1;
	F.NumP2 = p2;
	F.NumP3 = p3;
	F.NumP4 = p4;
	//F.Couleur.Coul = CG_RandColor(coul).Coul;
	CG_RandColor(F.Couleur,coul);
	F.Style = (BYTE)style;
	SG_SetFceTex(F,T);
	//F.NumTex = T.NumAttach;

CG_RandTexCoord(SG_TexSizeX(T),SG_TexSizeY(T),
				  F.XT1,F.YT1,
				  F.XT2,F.YT2,
				  F.XT3,F.YT3,
				  F.XT4,F.YT4);
	SG_CheckFce(F,"CG_DefRandFce:",;);
	}
	return;
}

//definit entierement une face aleatoire
void SPG_CONV CG_DefRandFlatFce(SG_FACE &F,
			   SG_PNT3D *p1,SG_PNT3D *p2,SG_PNT3D *p3,SG_PNT3D *p4,
			   ColorG& coul,int style,SG_FullTexture& T, float FlatFact)
{

	if (!SG_IsValidTex(T))
	{
		PixCoul CGRC;
		CG_RandColor(CGRC,coul);
		SG_DefUniSFce(F,p1,p2,p3,p4,CGRC.Coul,(style&SG_MASKUNI));
	}
		else
	{
	F.NumP1 = p1;
	F.NumP2 = p2;
	F.NumP3 = p3;
	F.NumP4 = p4;
	CG_RandColor(F.Couleur,coul);
	F.Style = (BYTE)style;
	SG_SetFceTex(F,T);
	//F.NumTex = T.NumAttach;

CG_RandFlatTexCoord(SG_TexSizeX(T),SG_TexSizeY(T),
				  F.XT1,F.YT1,
				  F.XT2,F.YT2,
				  F.XT3,F.YT3,
				  F.XT4,F.YT4,
				  FlatFact);
	SG_CheckFce(F,"CG_DefRandFlatFce:",;);
	}
	return;
}

//idem avec une modulation X
void SPG_CONV CG_DefRandModuleXFce(SG_FACE &F,
			   SG_PNT3D *p1,SG_PNT3D *p2,SG_PNT3D *p3,SG_PNT3D *p4,
			   ColorG& coul,int style,SG_FullTexture& T)
{
	if (!SG_IsValidTex(T))
	{
		PixCoul CGRC;
		CG_RandColor(CGRC,coul);
		SG_DefUniSFce(F,p1,p2,p3,p4,CGRC.Coul,(style&SG_MASKUNI));
	}
		else
	{
	F.NumP1 = p1;
	F.NumP2 = p2;
	F.NumP3 = p3;
	F.NumP4 = p4;
	CG_RandColor(F.Couleur,coul);
	F.Style = (BYTE)style;
	SG_SetFceTex(F,T);

CG_RandModuleXTexCoord(SG_TexSizeX(T),SG_TexSizeY(T),
				  F.XT1,F.YT1,
				  F.XT2,F.YT2,
				  F.XT3,F.YT3,
				  F.XT4,F.YT4);
	SG_CheckFce(F,"CG_DefRandModuleXFce:",;);
	}
	return;
}

#endif


#endif

