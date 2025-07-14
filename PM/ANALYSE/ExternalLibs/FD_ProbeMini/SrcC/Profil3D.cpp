
#include "SPG_General.h"

#ifdef SPG_General_USEProfil3D

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#ifdef DebugFloat
#include <float.h>
#endif

#include "SPG_Includes.h"

//vue avec les coupes XY ou vue 3d seulement
#define Pure3D
//#undef Pure3D

#ifdef SPG_General_USESTEREOVIS
#define AllowStereo
#endif

#ifdef DebugRender
#define LOWRESSIZE 64
#else
#define LOWRESSIZE 128
#endif

//ajuste les hauteurs des points 3D par rapport au profil
//on passe soi meme l'echelle, ajuste les normales
void SPG_CONV Profil3D_SetHeightFromFloat(SG_FullBloc &B,float TMin,float TDiff,Profil& P)
{
	CHECK(P_Etat(P)==0,"Profil3D_SetHeightFromFloat: Profil nul",return);
	CHECK(P.D==0,"Profil3D_SetHeightFromFloat: Profil vide",return);
	if (TDiff!=0) TDiff=1.0f/TDiff;
	for(int y=0;y<P_SizeY(P);y++)
	{
		IF_CD_G_CHECK(9,return);
		SG_PNT3D* SGPoint=B.DB.MemPoints+P_SizeX(P)*y;
		float *D=P.D+P_SizeX(P)*y;
		for(int x=0;x<P_SizeX(P);x++)
		{
			SGPoint[x].P.z=(D[x]-TMin)*TDiff;
		}
	}
	V_VECT Vvertical={0,0,1};
	SG_CalcNormalesToDir(B,Vvertical);
	return;
}

//ajuste les coordonnes de texture en fonction de la valeur de z
//on passe soi meme l'echelle
void SPG_CONV Profil3D_SetTexFromFloat(SG_FullBloc &B,float TMin,float TDiff, SG_FullTexture& T, Profil& P)
{
	CHECK(P_Etat(P)==0,"Profil3D_SetHeightFromFloat: Profil nul",return);
	CHECK(P.D==0,"Profil3D_SetHeightFromFloat: Profil vide",return);
	if (TDiff!=0) TDiff=SG_TexSizeX(T)/TDiff;
	for(int y=0;y<(P_SizeY(P)-1);y++)
	{
		SG_FACE* CurFace=B.DB.MemFaces+(P_SizeX(P)-1)*y;
		float *D=P.D+P_SizeX(P)*y;
		for(int x=0;x<(P_SizeX(P)-1);x++)
		{
			int XT=V_FloatToInt((D[x]-TMin)*TDiff);
			CurFace[x].XT1=(SHORT)V_Sature(XT,0,(SG_TexSizeX(T)-1));
			XT=V_FloatToInt((D[x+1]-TMin)*TDiff);
			CurFace[x].XT2=(SHORT)V_Sature(XT,0,(SG_TexSizeX(T)-1));
			XT=V_FloatToInt((D[x+1+P_SizeX(P)]-TMin)*TDiff);
			CurFace[x].XT3=(SHORT)V_Sature(XT,0,(SG_TexSizeX(T)-1));
			XT=V_FloatToInt((D[x+P_SizeX(P)]-TMin)*TDiff);
			CurFace[x].XT4=(SHORT)V_Sature(XT,0,(SG_TexSizeX(T)-1));
			//B.DB.MemFaces[x+(SizeX-1)*y].YT4=STY/2;
//#ifdef SGE_EMC
			if(SG_IsValidTex(T))
			{
			CurFace[x].Couleur.P24=Texel(T.T,
				((CurFace[x].XT1+CurFace[x].XT2+CurFace[x].XT3+CurFace[x].XT4)>>2),
				(SG_TexSizeY(T)>>1));
			}
			//CurFace[x].Couleur.Coul=0x00ff00;
//#endif
		}
	}
	return;
}

//masque les faces masquees, met les autres a style
void SPG_CONV Profil3D_SetMsk(SG_FullBloc& FB,Profil& P, int Style)
{
	if(P.Msk)
	{
		for(int y=0;y<P_SizeY(P)-1;y++)
		{
			for(int x=0;x<P_SizeX(P)-1;x++)
			{
				if(P_ElementMsk(P,x,y)|P_ElementMsk(P,x+1,y)|P_ElementMsk(P,x,y+1)|P_ElementMsk(P,x+1,y+1))
				{
					SG_GetFce(FB,(x+(P_SizeX(P)-1)*y)).Style=0;
				}
				else
				{
					SG_GetFce(FB,(x+(P_SizeX(P)-1)*y)).Style=(BYTE)Style;
				}
			}
		}
	}
	return;
}

//mappe une texture tel quel (necessite de desactiver les effets de lumiere)
void SPG_CONV Profil3D_TexMap(SG_FullBloc &B, SG_FullTexture& T, int SizeX, int SizeY)
{
	for(int y=0;y<(SizeY-1);y++)
	{
		SG_FACE* CurFace=B.DB.MemFaces+(SizeX-1)*y;
		for(int x=0;x<(SizeX-1);x++)
		{
			int yr=SizeY-1-y;//on mappe a l'envers pour des histoires de sens de texture
			CurFace[x].XT1=(SHORT)(((SG_TexSizeX(T)-1)*x)/SizeX);
			CurFace[x].YT1=(SHORT)(((SG_TexSizeY(T)-1)*(yr+1))/SizeY);
			CurFace[x].XT2=(SHORT)(((SG_TexSizeX(T)-1)*(x+1))/SizeX);
			CurFace[x].YT2=(SHORT)(((SG_TexSizeY(T)-1)*(yr+1))/SizeY);
			CurFace[x].XT3=(SHORT)(((SG_TexSizeX(T)-1)*(x+1))/SizeX);
			CurFace[x].YT3=(SHORT)(((SG_TexSizeY(T)-1)*(yr))/SizeY);
			CurFace[x].XT4=(SHORT)(((SG_TexSizeX(T)-1)*(x))/SizeX);
			CurFace[x].YT4=(SHORT)(((SG_TexSizeY(T)-1)*(yr))/SizeY);

			CurFace[x].Couleur.P24=Texel(T.T,
				((CurFace[x].XT1+CurFace[x].XT2+CurFace[x].XT3+CurFace[x].XT4)>>2),
				((CurFace[x].YT1+CurFace[x].YT2+CurFace[x].YT3+CurFace[x].YT4)>>2));
		}
	}
	return;
}

/*
void Profil3D_SetGreyscaleTexture(PROFIL3D_STATE& PS,BYTE* GreyPicture, int SizeX, int SizeY, int Pitch)
{
	CHECK(P_Etat(PS.HiP)==0,"Profil3D_SetGreyscaleTexture: Profil nul",return);
	CHECK(SizeX!=P_SizeX(PS.HiP),"Profil3D_SetGreyscaleTexture: Taille de l'image discordante",return);
	CHECK(SizeY!=P_SizeY(PS.HiP),"Profil3D_SetGreyscaleTexture: Taille de l'image discordante",return);
	for(int y=0;y<(P_SizeY(PS.HiP)-1);y++)
	{
		SG_FACE* CurFace=PS.HiB.DB.MemFaces+(P_SizeX(PS.HiP)-1)*y;
		BYTE *D=GreyPicture+Pitch*y;
		for(int x=0;x<(P_SizeX(PS.HiP)-1);x++)
		{
#define T PS.T
			CurFace[x].XT1=(SHORT)V_Sature(SG_TexSizeX(T)*D[x]/256,0,(SG_TexSizeX(T)-1));
			CurFace[x].XT2=(SHORT)V_Sature(SG_TexSizeX(T)*D[x+1]/256,0,(SG_TexSizeX(T)-1));
			CurFace[x].XT3=(SHORT)V_Sature(SG_TexSizeX(T)*D[x+1+Pitch]/256,0,(SG_TexSizeX(T)-1));
			CurFace[x].XT4=(SHORT)V_Sature(SG_TexSizeX(T)*D[x+Pitch]/256,0,(SG_TexSizeX(T)-1));
#undef T

			CurFace[x].Couleur.B=D[x];
			CurFace[x].Couleur.V=D[x];
			CurFace[x].Couleur.R=D[x];
			CurFace[x].Couleur.A=SG_NORMAL_ALPHA;
		}
	}
	return;
}
*/

/*
void SPG_CONV Profil3D_SetAuxiliaryColor(PROFIL3D_STATE& PS,Profil& P)
{
	CHECK(P_Etat(P)==0,"Profil3D_SetAuxiliaryColor: Profil nul",return);
	CHECK(P_SizeX(P)!=P_SizeX(PS.HiP),"Profil3D_SetAuxiliaryColor: Taille de l'image discordante",return);
	CHECK(P_SizeY(P)!=P_SizeY(PS.HiP),"Profil3D_SetAuxiliaryColor: Taille de l'image discordante",return);
	float fMin,fMax;
	P_FindMinMax(P,fMin,fMax);
	for(int y=0;y<(P_SizeY(PS.HiP)-1);y++)
	{
		SG_FACE* CurFace=PS.HiB.DB.MemFaces+(P_SizeX(PS.HiP)-1)*y;
		float *D=P.D+P_SizeX(P)*y;
		for(int x=0;x<(P_SizeX(PS.HiP)-1);x++)
		{
#define T PS.T
			CurFace[x].XT1=(SHORT)V_Sature((int)(SG_TexSizeX(T)*(D[x]-fMin)/(fMax-fMin)),0,(SG_TexSizeX(T)-1));
			CurFace[x].XT2=(SHORT)V_Sature((int)(SG_TexSizeX(T)*(D[x+1]-fMin)/(fMax-fMin)),0,(SG_TexSizeX(T)-1));
			CurFace[x].XT3=(SHORT)V_Sature((int)(SG_TexSizeX(T)*(D[x+1+P_SizeX(P)]-fMin)/(fMax-fMin)),0,(SG_TexSizeX(T)-1));
			CurFace[x].XT4=(SHORT)V_Sature((int)(SG_TexSizeX(T)*(D[x+P_SizeX(P)]-fMin)/(fMax-fMin)),0,(SG_TexSizeX(T)-1));
#undef T

			BYTE C=V_Round(255.0f*(D[x]-fMin)/(fMax-fMin));
			CurFace[x].Couleur.B=C;
			CurFace[x].Couleur.V=C;
			CurFace[x].Couleur.R=C;
			CurFace[x].Couleur.A=SG_NORMAL_ALPHA;
		}
	}
	return;
}
*/

/*
//masque les bords pour les problemes
void SPG_CONV Profil3D_EraseBorders(Profil& P)
{
	for(int y=0;y<P_SizeY(P);y++)
	{
		P.D[P_SizeX(P)*y]=P.D[P_SizeX(P)*y+1]=P.D[P_SizeX(P)*y+2];
		P.D[P_SizeX(P)-1+P_SizeX(P)*y]=P.D[P_SizeX(P)-2+P_SizeX(P)*y]=P.D[P_SizeX(P)-3+P_SizeX(P)*y]=P.D[P_SizeX(P)-4+P_SizeX(P)*y];
	}
	for(int x=0;x<P_SizeX(P);x++)
	{
		P.D[x]=P.D[x+P_SizeX(P)]=P.D[x+2*P_SizeX(P)];
		P.D[x+(P_SizeY(P)-1)*P_SizeX(P)]=P.D[x+(P_SizeY(P)-2)*P_SizeX(P)]=P.D[x+(P_SizeY(P)-3)*P_SizeX(P)];
	}
	return;
}
*/

//effet d'aspiration
void SPG_CONV Profil3D_BlackHole(SG_FullBloc BRef)
{
	for(int i=0;i<BRef.DB.NombreP;i++)
	{
		float CDiff=
			BRef.DB.MemPoints[i].P.x*
			BRef.DB.MemPoints[i].P.x+
			BRef.DB.MemPoints[i].P.y*
			BRef.DB.MemPoints[i].P.y;
		float VROTATE=0.03f*(.2f+BRef.DB.MemPoints[i].P.z)/(0.2f+CDiff);
		float UMEpsilon=1-VROTATE;
		float XYUMEpsilon=1-VROTATE;

		BRef.DB.MemPoints[i].P.x*=XYUMEpsilon;
		BRef.DB.MemPoints[i].P.x+=VROTATE*BRef.DB.MemPoints[i].P.y;
		BRef.DB.MemPoints[i].P.y*=XYUMEpsilon;
		BRef.DB.MemPoints[i].P.y-=VROTATE*BRef.DB.MemPoints[i].P.x;
		BRef.DB.MemPoints[i].P.z=
			1.2f-(1.2f-BRef.DB.MemPoints[i].P.z)*UMEpsilon;
	}
}

/*
//genere un profil 2D
void SPG_CONV Profil3D_Generate(Profil& P)
{
//  P_Create(Profil& P, int SizeX, int SizeY, float XScale,float YScale, char*UnitX,char*UnitY,char*UnitZ, int WithMask)
	P_Create(P,256,256,1,1,0,0,0,0);
	for(int y=0;y<P_SizeY(P);y++)
	{
		float fy=y/(float)(P_SizeY(P)-1);
		for(int x=0;x<P_SizeX(P);x++)
		{
			
			float fx=x/(float)(SizeX-1);
			Df[x+SizeX*y]=
				fy*(1-fy)*
				fy*(1-fy)*
				fx*(1-fx)*
				fx*(1-fx)*
				(0.005+(fx-fy)*(fx-fy)*(1-(fx+fy))*(1-(fx+fy)))
				+0.0000000000005*rand();//-0.1*(fx-0.4)*(0.6-fx);
			//Df[x+SizeX*y]*=Df[x+SizeX*y];
			
			P.D[x+P_SizeX(P)*y]=0;
			if(x<P_SizeX(P)/2)
				P.D[x+P_SizeX(P)*y]=y;
		}
	}
	return;
}
*/

//cree le repere 3D (fleches colorees)
void SPG_CONV T_CreateRep(SG_FullBloc& Rep, int FaceType, DWORD CoulX, DWORD CoulY, DWORD CoulZ)
{

	SG_FullBloc AXEXBase,AXEXSommet;
	
	{
	V_VECT vO={-1.1f,-1.1f,0};//origine
	V_VECT vH={0.6f,0,0};//direction
	V_VECT vB1={0,0.05f,0};//axex principaux de la base du cylindre
	V_VECT vB2={0,0,0.05f};
	
	SG_FullTexture NullTex;
	SG_SetToNullTex(NullTex);
//cree un cylindre non texture
	SG_CreateCylinder(AXEXBase,7,12,
		vO,vB1,vB2,vH,
		CoulX,FaceType,NullTex,
		0,0, 0,0, 0,0, 0,0);
	
	V_Operate2(vO,+=0.9f*vH);
	V_Operate1(vB1,*=1.6f);
	V_Operate1(vB2,*=1.6f);
	V_Operate1(vH,*=0.5f);
	SG_CreateCone(AXEXSommet,7,12,
		vO,vB1,vB2,vH,
		CoulX,FaceType,NullTex,
		0,0, 0,0, 0,0, 0,0);
	}
	
	
	SG_FullBloc AXEYBase,AXEYSommet;
	
	{
	V_VECT vO={-1.1f,-1.1f,0};
	V_VECT vH={0,0.6f,0};
	V_VECT vB1={0,0,0.05f};
	V_VECT vB2={-0.05f,0,0};
	
	SG_FullTexture NullTex;
	SG_SetToNullTex(NullTex);
	SG_CreateCylinder(AXEYBase,7,12,
		vO,vB1,vB2,vH,
		CoulY,FaceType,NullTex,
		0,0, 0,0, 0,0, 0,0);
	
	V_Operate2(vO,+=0.9f*vH);
	V_Operate1(vB1,*=1.6f);
	V_Operate1(vB2,*=1.6f);
	V_Operate1(vH,*=0.5f);
	SG_CreateCone(AXEYSommet,7,12,
		vO,vB1,vB2,vH,
		CoulY,FaceType,NullTex,
		0,0, 0,0, 0,0, 0,0);
	}
	
	SG_FullBloc AXEZBase,AXEZSommet;
	
	{
	V_VECT vO={-1.1f,-1.1f,0};
	V_VECT vH={0,0,0.6f};
	V_VECT vB1={0.05f,0,0};
	V_VECT vB2={0,0.05f,0};
	
	SG_FullTexture NullTex;
	SG_SetToNullTex(NullTex);
	SG_CreateCylinder(AXEZBase,7,12,
		vO,vB1,vB2,vH,
		CoulZ,FaceType,NullTex,
		0,0, 0,0, 0,0, 0,0);
	
	V_Operate2(vO,+=0.9f*vH);
	V_Operate1(vB1,*=1.6f);
	V_Operate1(vB2,*=1.6f);
	V_Operate1(vH,*=0.5f);
	SG_CreateCone(AXEZSommet,7,12,
		vO,vB1,vB2,vH,
		CoulZ,FaceType,NullTex,
		0,0, 0,0, 0,0, 0,0);
	}

	SG_ConcatBloc6(Rep,
		AXEXBase,AXEXSommet,
		AXEYBase,AXEYSommet,
		AXEZBase,AXEZSommet);
	
	SG_CloseBloc(AXEXBase);
	SG_CloseBloc(AXEXSommet);
	SG_CloseBloc(AXEYBase);
	SG_CloseBloc(AXEYSommet);
	SG_CloseBloc(AXEZBase);
	SG_CloseBloc(AXEZSommet);
	return;
}

void SPG_CONV T_SaveXCut(float * Tableau, int SizeX, int SizeY,int ysel)
{
	//DoEvents(SPG_DOEV_MIN);
		Cut C;
		Cut_Create(C,SizeX,1,"","");
		int x;
		for(x=0;x<SizeX;x++)
		{
			C.D[x]=Tableau[x+SizeX*ysel];
		}
		Cut_Save(C,"CoupeX");
		Cut_Close(C);
	return;
}

void SPG_CONV T_SaveYCut(float * Tableau, int SizeX, int SizeY,int xsel)
{
	//DoEvents(SPG_DOEV_MIN);
		Cut C;
		Cut_Create(C,SizeY,1,"","");
		int y;
		for(y=0;y<SizeY;y++)
		{
			C.D[y]=Tableau[xsel+SizeX*y];
		}
		Cut_Save(C,"CoupeY");
		Cut_Close(C);
	return;
}

//dessine dans les deux ecrans du bas
void SPG_CONV T_Draw2DXYCut(G_Ecran& SECoupeX,G_Ecran& SECoupeY,float * Tableau, int SizeX, int SizeY,int xsel,int ysel, C_Lib& CL)
{
	{
		Cut C;
		
		Cut_Create(C,SizeX,1,"","");
		int x;
		for(x=0;x<SizeX;x++)
		{
			C.D[x]=Tableau[x+SizeX*ysel];
		}
		Cut_Draw(C,SECoupeX,0x00ff0000,CL);
		Cut_Close(C);
		
		Cut_Create(C,SizeY,1,"","");
		int y;
		for(y=0;y<SizeY;y++)
		{
			C.D[y]=Tableau[xsel+SizeX*y];
		}
		Cut_Draw(C,SECoupeY,0x00ff00,CL);
		Cut_Close(C);
	}
	return;
}

//dessine dans la fenetre 3D
void SPG_CONV T_Draw3DXYCut(G_Ecran& SEGraph,SG_FullBloc& B, float * Tableau, int SizeX, int SizeY,int xsel,int ysel, C_Lib& CL)
{
	int x,y;

	for(x=0;x<SizeX-1;x++)
	{//recupere les coordonnees ecran des points B.DB.MemPoints
		G_DrawLine(SEGraph,B.DB.MemPoints[x+SizeX*ysel].XECR-1,
			B.DB.MemPoints[x+SizeX*ysel].YECR,
			B.DB.MemPoints[x+1+SizeX*ysel].XECR-1,
			B.DB.MemPoints[x+1+SizeX*ysel].YECR,0xff8080);
		G_DrawLine(SEGraph,B.DB.MemPoints[x+SizeX*ysel].XECR+1,
			B.DB.MemPoints[x+SizeX*ysel].YECR,
			B.DB.MemPoints[x+1+SizeX*ysel].XECR+1,
			B.DB.MemPoints[x+1+SizeX*ysel].YECR,0xff8080);
		G_DrawLine(SEGraph,B.DB.MemPoints[x+SizeX*ysel].XECR,
			B.DB.MemPoints[x+SizeX*ysel].YECR-1,
			B.DB.MemPoints[x+1+SizeX*ysel].XECR,
			B.DB.MemPoints[x+1+SizeX*ysel].YECR-1,0xff8080);
		G_DrawLine(SEGraph,B.DB.MemPoints[x+SizeX*ysel].XECR,
			B.DB.MemPoints[x+SizeX*ysel].YECR+1,
			B.DB.MemPoints[x+1+SizeX*ysel].XECR,
			B.DB.MemPoints[x+1+SizeX*ysel].YECR+1,0xff8080);
		G_DrawLine(SEGraph,B.DB.MemPoints[x+SizeX*ysel].XECR,
			B.DB.MemPoints[x+SizeX*ysel].YECR,
			B.DB.MemPoints[x+1+SizeX*ysel].XECR,
			B.DB.MemPoints[x+1+SizeX*ysel].YECR,0xff0000);
	}
	for(y=0;y<SizeY-1;y++)
	{
		G_DrawLine(SEGraph,B.DB.MemPoints[xsel+SizeX*y].XECR-1,
			B.DB.MemPoints[xsel+SizeX*y].YECR,
			B.DB.MemPoints[xsel+SizeX*(y+1)].XECR-1,
			B.DB.MemPoints[xsel+SizeX*(y+1)].YECR,0x0080ff80);
		G_DrawLine(SEGraph,B.DB.MemPoints[xsel+SizeX*y].XECR+1,
			B.DB.MemPoints[xsel+SizeX*y].YECR,
			B.DB.MemPoints[xsel+SizeX*(y+1)].XECR+1,
			B.DB.MemPoints[xsel+SizeX*(y+1)].YECR,0x0080ff80);
		G_DrawLine(SEGraph,B.DB.MemPoints[xsel+SizeX*y].XECR,
			B.DB.MemPoints[xsel+SizeX*y].YECR-1,
			B.DB.MemPoints[xsel+SizeX*(y+1)].XECR,
			B.DB.MemPoints[xsel+SizeX*(y+1)].YECR-1,0x0080ff80);
		G_DrawLine(SEGraph,B.DB.MemPoints[xsel+SizeX*y].XECR,
			B.DB.MemPoints[xsel+SizeX*y].YECR+1,
			B.DB.MemPoints[xsel+SizeX*(y+1)].XECR,
			B.DB.MemPoints[xsel+SizeX*(y+1)].YECR+1,0x0080ff80);
		G_DrawLine(SEGraph,B.DB.MemPoints[xsel+SizeX*y].XECR,
			B.DB.MemPoints[xsel+SizeX*y].YECR,
			B.DB.MemPoints[xsel+SizeX*(y+1)].XECR,
			B.DB.MemPoints[xsel+SizeX*(y+1)].YECR,0x0000ff00);
	}

	char Msg[256];
//ecrit les coordonnees a cote du curseur
	sprintf(Msg,"x=%d\ny=%d\nz=%.3g",xsel,ysel,Tableau[xsel+SizeX*ysel]);
	/*
	int i;
	for(i=0;i<(int)strlen(Msg);i++)
		Msg[i]=toupper(Msg[i]);
		*/
	C_PrintWithBorder(SEGraph,
		B.DB.MemPoints[xsel+SizeX*ysel].XECR-4,
		B.DB.MemPoints[xsel+SizeX*ysel].YECR-4,
		Msg,CL,XRIGHT|YDN,0xffffffff);

	return;
}

//cherche les coordonnees xy dans le profil 2D du curseur de la souris au dessus de la vue 3D
int SPG_CONV T_Find3DXYCut(G_Ecran& SEGraph,int GMouseX,int GMouseY,SG_FullBloc& B, int SizeX, int SizeY, int SelSize, int& xsel, int& ysel)
{
	int LMX=GMouseX-SEGraph.PosX;
	int LMY=GMouseY-SEGraph.PosY;
	int NumP;
	if((NumP=S_GetPoint(B.DB,LMX,LMY,SelSize))!=-1)
	{
		xsel=NumP%SizeX;
		ysel=NumP/SizeX;
		return -1;
	}
	else
	return 0;
}

int SPG_CONV Profil3D_DrawScale(PROFIL3D_STATE& PS)
{
	{for(int y=4*PS.CL.SpaceY;y<G_SizeY(PS.SEGraph)-4*PS.CL.SpaceY;y++)
	{
		G_DrawLine(PS.SEGraph,
			0,
			G_SizeY(PS.SEGraph)-1-y,
			16,
			G_SizeY(PS.SEGraph)-1-y,
			PS.ColorTable[
			(256*(y-4*PS.CL.SpaceY))
			/(G_SizeY(PS.SEGraph)-8*PS.CL.SpaceY)
			].Coul);
	}}
	{for(int y=4*PS.CL.SpaceY;y<G_SizeY(PS.SEGraph)-4*PS.CL.SpaceY;y+=3*PS.CL.SpaceY)
	{
		char Msg[32];
		Msg[0]=0;
		CF_GetString(Msg,
			PS.ZScale*(G_SizeY(PS.SEGraph)-4*PS.CL.SpaceY-1-y)
			/(G_SizeY(PS.SEGraph)-4*PS.CL.SpaceY-1),
			6);
		strcat(Msg,P_UnitZ(PS.HiP));
		C_PrintWithBorder(PS.SEGraph,8,y,Msg,PS.CL,XLEFT,0xFFFFFF);
	}}
	{
		char Msg[128];
		strcpy(Msg,"SizeX=");
		CF_GetString(Msg,P_SizeX(PS.HiP)*P_XScale(PS.HiP),6);
		strcat(Msg,P_UnitX(PS.HiP));
		strcat(Msg,"\nSizeY=");
		CF_GetString(Msg,P_SizeY(PS.HiP)*P_YScale(PS.HiP),6);
		strcat(Msg,P_UnitY(PS.HiP));
		C_PrintWithBorder(PS.SEGraph,4,G_SizeY(PS.SEGraph)-3*PS.CL.SpaceY,Msg,PS.CL,XLEFT,0xFFFFFF);
	}
	return -1;
}

int SPG_CONV Profil3D_Init(G_Ecran &E,PROFIL3D_STATE& PS,Profil& P,int IsStereo)
{
	memset(&PS,0,sizeof(PROFIL3D_STATE));
	
	CHECK(P_Etat(P)==0,"Profil nul",return 0);
	CHECK(P.D==0,"Donnees vides",return 0);
	CHECK(P.Msk==0,"Masque vide",return 0);
	CHECK(P_SizeX(P)<1,"Taille X invalide",return 0);
	CHECK(P_SizeY(P)<1,"Taille Y invalide",return 0);
	//SPG_VerboseCall(P_Load(PS.HiP,"Profil2D"));
	P_Init(PS.HiP,P.D,P.Msk,P_SizeX(P),P_SizeY(P),P_Alias);
	P_SetScale(PS.HiP,P_XScale(P),P_YScale(P),P_UnitX(P),P_UnitY(P),P_UnitZ(P));
	
	IF_CD_G_CHECK(26,return 0);
	//ote les bords, pb avec les txt fogale
	//SPG_VerboseCall(T_EraseBorders(P));
	CHECK(P_Etat(PS.HiP)==0,"Profil nul",return 0);
	CHECK(PS.HiP.D==0,"Donnees vides",return 0);
	CHECK(P_SizeX(PS.HiP)<1,"Taille X invalide",return 0);
	CHECK(P_SizeY(PS.HiP)<1,"Taille Y invalide",return 0);

#ifdef AllowStereo	
	PS.IsStereo=IsStereo;
#endif
	
	
	G_InitSousEcran(PS.SEBoutons,E,
		E.SizeX-128,0,
		128,384);
	if(E.SizeY-PS.SEBoutons.SizeY>=128)
		{
	G_InitSousEcran(PS.SEopts,E,
		E.SizeX-128,E.SizeY-128,
		128,128);
		}
	
#ifdef Pure3D
	G_InitSousEcran(PS.SEGraph,E,0,0,PS.SEBoutons.PosX-E.PosX,E.SizeY);
#else
	G_InitSousEcran(PS.SEGraph,E,0,0,PS.SEBoutons.PosX-E.PosX,E.SizeY/2);
#endif
	
#ifdef Pure3D
#else
	
	G_InitSousEcran(PS.SECoupeX,E,
		0,PS.SEGraph.PosY+PS.SEGraph.SizeY,
		PS.SEBoutons.PosX,(E.SizeY-(PS.SEGraph.PosY+PS.SEGraph.SizeY))/2);
	G_InitSousEcran(PS.SECoupeY,E,
		0,PS.SECoupeX.PosY+PS.SECoupeX.SizeY,
		PS.SEBoutons.PosX,E.SizeY-(PS.SECoupeX.PosY+PS.SECoupeX.SizeY));
#endif
	
	B_LoadButtonsLib(PS.BL,PS.SEBoutons,1,0,"..\\SrcC\\Interface","Buttons.bmp");
	
	C_LoadCaracLib(PS.CL,E,"..\\SrcC\\Carac","CaracNoir.bmp");
	
	if((P_SizeX(PS.HiP)>LOWRESSIZE)||(P_SizeY(PS.HiP)>LOWRESSIZE))
	{	//downsample le profil pour faire une vue basse resolution
		//on utilise le fait que P=HiResP avant
		P_GetUndersW( PS.LowP, PS.HiP, 0, P_SizeX(PS.HiP)-1, 0, P_SizeY(PS.HiP)-1, V_Ceil(P_SizeX(PS.HiP)/(float)LOWRESSIZE) );
	}

	SG_InitVue(PS.Vue,PS.SEGraph,
		5,//distance de vue
		5,//distance de texturage
#ifdef AllowStereo	
		PS.IsStereo?0.16f:0.2f,//distance de l'ecran
#else
		0.2f,//distance de l'ecran
#endif
#ifdef AllowStereo	
		PS.IsStereo?1.3:2,//distance a plat de l'origine du monde (definit la position initiale)
#else
		2,//distance a plat de l'origine du monde (definit la position initiale)
#endif
		0,//distance verticale de l'origine du monde (definit la position initiale)
#ifdef AllowStereo	
		PS.IsStereo?0:0x7070d0,
#else
		0x6060e0,
#endif
		0x7070d0,
		1024*1024);//max tri
#ifdef AllowStereo	
	if(PS.IsStereo)
	{
	SPG_SV_Init(PS.SV,PS.Vue,
		0.06,
		0,
		0.04,
		SPG_SV_REDBLUE);
	}
#endif
#ifdef SPG_General_USESGRAPH_OPTS
	if(G_Etat(PS.SEopts)) PS.SGopts=SG_CreateOpts(PS.Vue,PS.SEopts,0);//boutons de reglage des parametres
#endif
	
	PS.ColorTable=C256_Init(0,0,0,0);
#ifdef AllowStereo	
	if(PS.IsStereo)
	{
	C256_InterpoleT(PS.ColorTable,0,255,0,255,0,255,0,255);
	}
#endif
	PS.STY=PS.STX=256;
	SG_SetToNullTex(PS.T);
	SG_CreateTexture(PS.T,PS.STX,PS.STY);
	SG_AttachTexture(PS.Vue,PS.T,SG_FirstFreeTex(PS.Vue));//attache la texture a la vue 3D

	//a l'affichage on veut x sur -Horizontal
	//y sur Vertical
	//z en -Prof
	{
		V_VECT v0;//les 4 coins du profil
		V_VECT v1;
		V_VECT v2;
		V_VECT v3;
		if (P_SizeX(PS.HiP)>P_SizeY(PS.HiP))
		{
			V_SetXYZ(v0,-1,(float)P_SizeY(PS.HiP)/P_SizeX(PS.HiP),0);
			V_SetXYZ(v1,1,(float)P_SizeY(PS.HiP)/P_SizeX(PS.HiP),0);
			V_SetXYZ(v2,1,-(float)P_SizeY(PS.HiP)/P_SizeX(PS.HiP),0);
			V_SetXYZ(v3,-1,-(float)P_SizeY(PS.HiP)/P_SizeX(PS.HiP),0);
		}
		else
		{
			V_SetXYZ(v0,-(float)P_SizeX(PS.HiP)/P_SizeY(PS.HiP),1,0);
			V_SetXYZ(v1,(float)P_SizeX(PS.HiP)/P_SizeY(PS.HiP),1,0);
			V_SetXYZ(v2,(float)P_SizeX(PS.HiP)/P_SizeY(PS.HiP),-1,0);
			V_SetXYZ(v3,-(float)P_SizeX(PS.HiP)/P_SizeY(PS.HiP),-1,0);
		}
		//cree un plan
		SG_CreatePlan(PS.HiB,P_SizeX(PS.HiP)-1,P_SizeY(PS.HiP)-1,v0,v1,v2,v3,
			0x00ff00,SG_TEX,PS.T,
			0,0, PS.STX-1,0, PS.STX-1,PS.STY-1, 0,PS.STY-1);

		if(P_Etat(PS.LowP))
		{
			SG_CreatePlan(PS.LowB,P_SizeX(PS.LowP)-1,P_SizeY(PS.LowP)-1,v0,v1,v2,v3,
				0x00ff00,SG_TEX,PS.T,
				0,0, PS.STX-1,0, PS.STX-1,PS.STY-1, 0,PS.STY-1);
		}
	}
	
	
	T_CreateRep(PS.AXES,SG_UNI,0xff0000,0x00ff00,0x0000ff);
	
	//SPG_VerboseCall();
	P_FindMinMax(PS.HiP,PS.TMin,PS.TMax);
	PS.ZScale=(PS.TMax-PS.TMin);
#ifdef AllowStereo
	if(PS.IsStereo)
	{
		PS.TDiff=3*PS.ZScale;
	}
	else
#endif
	{
		PS.TDiff=10*PS.ZScale;
	}
	//if (PS.TDiff!=0) PS.TDiff=0.2f/PS.TDiff;
	
	//duplique les points des objets 3D pour avoir un fixe qui sert a generer un mobile
#ifdef SGE_EMC
	//duplique les normales ausssi
	SPG_VerboseCall(SG_DupliqueBloc_NewMacroVersion(PS.HiBRef,PS.HiB,SG_WithPOINTS|SG_WithFACES));
	if (P_Etat(PS.LowP)) SPG_VerboseCall(SG_DupliqueBloc_NewMacroVersion(PS.LowBRef,PS.LowB,SG_WithPOINTS|SG_WithFACES));
	SPG_VerboseCall(SG_DupliqueBloc_NewMacroVersion(PS.AXESRef,PS.AXES,SG_WithPOINTS|SG_WithFACES));
#else
	SPG_VerboseCall(SG_DupliqueBloc_NewMacroVersion(PS.HiBRef,PS.HiB,SG_WithPOINTS));
	if (P_Etat(PS.LowP)) SPG_VerboseCall(SG_DupliqueBloc_NewMacroVersion(PS.LowBRef,PS.LowB,SG_WithPOINTS));
	SPG_VerboseCall(SG_DupliqueBloc_NewMacroVersion(PS.AXESRef,PS.AXES,SG_WithPOINTS));
#endif


	Profil3D_UpdateColorMaps(PS,1,1,1,1);
	
	//repere depuis lequel on regarde
	//pos=position, axex=axe de vue, axey=x de l'ecran, axez=y de l'ecran
#ifdef AllowStereo
	if(PS.IsStereo)
	{
	V_SetXYZ(PS.V_Rep.pos,0,0,0);
	V_SetXYZ(PS.V_Rep.axex,1,0,0);
	V_SetXYZ(PS.V_Rep.axey,0,0.1f,0.9f);
	V_SetXYZ(PS.V_Rep.axez,0,-0.9f,0.1f);
	//attention au(x) bug du compilateur
	//V_Orthonorme(PS.V_Rep);
	}
	else
#endif
	{
	V_SetXYZ(PS.V_Rep.pos,0,0,0);
	V_SetXYZ(PS.V_Rep.axex,1,0,0);
	V_SetXYZ(PS.V_Rep.axey,0,0.707f,0.707f);
	V_SetXYZ(PS.V_Rep.axez,0,-0.707f,0.707f);
	//attention au(x) bug du compilateur
	//V_Orthonorme(PS.V_Rep);
	}

	V_SetXYZ(PS.Rotation,0,0,0);
	V_SetXYZ(PS.Light,-0.3f,-0.8f,0.3f);
	PS.LightIntensity=1;
#ifdef AllowStereo
	if(PS.IsStereo)
	{
		PS.Diffusivity=1;
	}
	else
#endif
	{
		PS.Diffusivity=4;
	}
	
	
	//bug du compilateur
	//SG_CheckRepere(PS.V_Rep);
	{
	V_Orthonorme(PS.V_Rep);
	}
	
	/*
#define Rep PS.V_Rep
	{V_Normalise(Rep.axex);{float s1=V_ScalVect(Rep.axex,Rep.axey);V_Operate2(Rep.axey,-=s1*Rep.axex);}V_Normalise(Rep.axey);{float s1=V_ScalVect(Rep.axex,Rep.axez);V_Operate2(Rep.axez,-=s1*Rep.axex);}{float s1=V_ScalVect(Rep.axey,Rep.axez);V_Operate2(Rep.axez,-=s1*Rep.axey);}V_Normalise(Rep.axez);}
#undef Rep
	*/
	//SG_CheckRepere(PS.V_Rep);

	//cree les boutons de l'interface
	int SpaceX=PS.BL.SizeX[ClickSprite]+2+PS.CL.SizeX;
	int SpaceY=PS.BL.SizeY[ClickSprite]+PS.CL.SpaceY+2;
	int CurX=2;
	int CurY=PS.CL.SpaceY;
	PS.BQuit=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX+SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.BQuit,PS.CL,"Quit");

	CurY+=SpaceY;
	B_PrintLabel(PS.BL,B_CreateVReglage(PS.BL,PS.SEBoutons,
		CurX,CurY,64,&PS.LightIntensity,0.025f,0,2)-2,PS.CL,
		"Intens");
	B_PrintLabel(PS.BL,B_CreateVReglage(PS.BL,PS.SEBoutons,
		CurX+SpaceX,CurY,64,&PS.Diffusivity,0.1f,1,8)-2,PS.CL,
		"Diff");
	PS.LightAjust=B_CreateCheckButton(PS.BL,PS.SEBoutons,
		CurX+2*SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.LightAjust,PS.CL,"Light\nPos");
	PS.HeightAjust=B_CreateVReglage(PS.BL,PS.SEBoutons,
		CurX+3*SpaceX,CurY,64,&PS.TDiff,PS.TDiff/40,PS.TDiff/10,PS.TDiff*2);
	B_PrintLabel(PS.BL,PS.HeightAjust-2,PS.CL,"Z");
	PS.ZoomAjust=B_CreateVReglage(PS.BL,PS.SEBoutons,
		CurX+4*SpaceX,CurY,64,&PS.Vue.DEcr,PS.Vue.DEcr/8,PS.Vue.DEcr/4,PS.Vue.DEcr*4);
	B_PrintLabel(PS.BL,PS.ZoomAjust-2,PS.CL,"Zoom");
	PS.FocaleAjust=B_CreateVReglage(PS.BL,PS.SEBoutons,
		CurX+5*SpaceX,CurY,64,&PS.Vue.Rep.pos.y,0.01f,-4.0f,-0.2f);
	B_PrintLabel(PS.BL,PS.FocaleAjust-2,PS.CL,"Focale");
	
	CurY+=PS.CL.SpaceY+64;
	B_PrintLabel(PS.BL,B_CreateHReglage(PS.BL,PS.SEBoutons,
		CurX,CurY,PS.SEBoutons.SizeX-CurX,&PS.Rotation.z,0.025f,-1,1),PS.CL,
		"Rotation");
	
	CurY+=SpaceY;
	PS.LockPos=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX,CurY);
	B_PrintLabel(PS.BL,PS.LockPos,PS.CL,"Lock");
	PS.FrontView=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX+SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.FrontView,PS.CL,"Front");
	PS.ShowAxes=B_CreateCheckButton(PS.BL,PS.SEBoutons,CurX+2*SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.ShowAxes,PS.CL,"Axes");
	PS.BShowScale=B_CreateCheckButton(PS.BL,PS.SEBoutons,CurX+3*SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.BShowScale,PS.CL,"Scale");
	B_Set(PS.BL,PS.BShowScale,B_Click);
	
	CurY+=SpaceY+PS.CL.SpaceY;
	PS.NewColor=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX,CurY);
	B_PrintLabel(PS.BL,PS.NewColor,PS.CL,"Color");
	PS.DefaultColor=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX+SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.DefaultColor,PS.CL,"DefCol");
	PS.LoadColors=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX+2*SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.LoadColors,PS.CL,"Load\nColors");
	/*
	PS.LoadAux=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX+3*SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.LoadAux,PS.CL,"Load\nTexture");
	*/

	CurY+=SpaceY+PS.CL.SpaceY;
#ifndef Pure3D
	PS.XYCut=B_CreateCheckButton(PS.BL,PS.SEBoutons,CurX,CurY);
	B_PrintLabel(PS.BL,PS.XYCut,PS.CL,"XYCut");
	PS.PntSel=B_CreateCheckButton(PS.BL,PS.SEBoutons,CurX+SpaceX,CurY);
#endif
	PS.BSaveColors=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX+2*SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.BSaveColors,PS.CL,"Save\nColors");
	PS.BTexMap=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX+3*SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.BTexMap,PS.CL,"Map\nTexture");
	
	
	CurY+=SpaceY;
	PS.MecaMove=B_CreateCheckButton(PS.BL,PS.SEBoutons,CurX,CurY);
	B_PrintLabel(PS.BL,PS.MecaMove,PS.CL,"Move");
	B_Set(PS.BL,PS.MecaMove,B_Click|B_Change);
	
	if (P_Etat(PS.LowP))
	{
		PS.HiResDraw=B_CreateCheckButton(PS.BL,PS.SEBoutons,CurX+2*SpaceX,CurY);
		B_PrintLabel(PS.BL,PS.HiResDraw,PS.CL,"FULL RES");
	}
	
	
	CurY+=SpaceY+PS.CL.SpaceY;
	PS.SaveProfil=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX,CurY);
	B_PrintLabel(PS.BL,PS.SaveProfil,PS.CL,"Save\nProfil");
	PS.SavePicture=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX+SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.SavePicture,PS.CL,"Save\nPicture");
	
#ifndef Pure3D
	CurY+=SpaceY+PS.CL.SpaceY;
	PS.BSaveCutX=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX,CurY);
	B_PrintLabel(PS.BL,PS.BSaveCutX,PS.CL,"Save\nRED");
	PS.BSaveCutY=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX+SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.BSaveCutY,PS.CL,"Save\nGREEN");
#endif
	
	CurY+=SpaceY+PS.CL.SpaceY;
	if(CurY<=PS.SEBoutons.SizeY-PS.BL.SizeY[ClickSprite])
	{
	PS.BMaskMin=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX,CurY);
	B_PrintLabel(PS.BL,PS.BMaskMin,PS.CL,"MASK");
	/*
	PS.BMedianFilter=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX+SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.BMedianFilter,PS.CL,"MEDIAN");
	*/
	PS.BConvFilter=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX+2*SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.BConvFilter,PS.CL,"CONV\n3x3");
	/*
	PS.BSpace=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX+3*SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.BSpace,PS.CL,"BLACK\nHOLE");
	*/
	}
	
	CurY+=SpaceY+PS.CL.SpaceY;
	if(CurY<=PS.SEBoutons.SizeY-PS.BL.SizeY[ClickSprite])
	{
	PS.BRevY=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX,CurY);
	B_PrintLabel(PS.BL,PS.BRevY,PS.CL,"Reverse\nY");
	/*
	int BMedianFilter=B_CreateClickButton(BL,SEBoutons,8+24,CurY);
	B_PrintLabel(BL,BMedianFilter,CL,"MEDIAN");
	int BConvFilter=B_CreateClickButton(BL,SEBoutons,8+24+24,CurY);
	B_PrintLabel(BL,BConvFilter,CL,"CONV\n3x3");
	*/
	/*
	PS.BLoad=B_CreateClickButton(PS.BL,PS.SEBoutons,CurX+2*SpaceX,CurY);
	B_PrintLabel(PS.BL,PS.BLoad,PS.CL,"Load");
	*/
	PS.BManualScale=B_CreateCliquableNumericButton(PS.BL,PS.SEBoutons,PS.CL,CurX+SpaceX,CurY,6,&PS.ZScale);
	}
	
	SPG_VerboseCall(B_RedrawButtonsLib(PS.BL,0));
	
	PS.DT=0;
	PS.XSel=P_SizeX(PS.HiP)/2;
	PS.YSel=P_SizeY(PS.HiP)/2;//selection 3D
	//int XYSelOk=1;//selection valide (en l'occurence oui)
	PS.XYSelOk=0;//selection valide (en l'occurence non)
	PS.MSSS=0;//mouse ref pos pour mecamove valide

	S_InitTimer(PS.ImageTimer,"Image step");
#ifdef DebugProfil3D
	S_InitTimer(PS.Timer[TmGeneral],"Temps total");
	S_InitTimer(PS.Timer[TmGenerate3D],"Gestion 3D");
	S_InitTimer(PS.Timer[TmLight],"Lumiere 3D");
	S_InitTimer(PS.Timer[TmInitRender],"Init 3D");
	S_InitTimer(PS.Timer[TmRender],"Rendu 3D");
	S_InitTimer(PS.Timer[TmButtons],"Gestion interface");
	S_InitTimer(PS.Timer[TmOutside],"Exterieur");
	
	S_StartTimer(PS.Timer[TmGeneral]);
	S_StartTimer(PS.Timer[TmOutside]);
#endif

	S_StartTimer(PS.ImageTimer);
	return PS.Etat|=PROFIL3D_OK;
}
	/*
	{//ici le bloc
	//n'est pas encore correctement oriente
	V_VECT TrueLight;
	float NLightIntensity=LightIntensity/V_ModVect(Light);
	V_Operate2(TrueLight,=NLightIntensity*Light);
	C256_MakeFaceColor(B.DB, SizeX-1,SizeY-1,STX,STY,TrueLight,Diffusivity);
	C256_MakeFaceColor(HiResB.DB, HiResSizeX-1,HiResSizeY-1,STX,STY,TrueLight,Diffusivity);
	}
	*/

#define IF_HIRES if (B_IsClick(PS.BL,PS.HiResDraw)||(PS.HiResDraw==0))


void SPG_CONV Profil3D_UpdateColorMaps(PROFIL3D_STATE& PS, int ChangeMsk, int ChangeZ, int ChangeTexZ, int SetTexLightMode)
{
/*
#ifdef SGE_DrawNormales
char Msg[1024];
sprintf(Msg,"void SPG_CONV Profil3D_UpdateColorMaps(\nPROFIL3D_STATE& PS,\nint ChangeMsk=%i,\nint ChangeZ=%i,\nint ChangeTexZ=%i,\nint SetTexLightMode=%i)",ChangeMsk,ChangeZ,ChangeTexZ,SetTexLightMode);
SPG_List(Msg);
#endif
*/
if(ChangeMsk)
{
	if (PS.LowB.Etat)
	{
		Profil3D_SetMsk(PS.LowB,PS.LowP,SG_TEX);
	}
	if (PS.HiB.Etat)
	{
		Profil3D_SetMsk(PS.HiB,PS.HiP,SG_TEX);
	}
}
if(ChangeZ)
{
	if (PS.HiBRef.Etat) Profil3D_SetHeightFromFloat(PS.HiBRef,PS.TMin,PS.TDiff,PS.HiP);
	if (PS.LowBRef.Etat) Profil3D_SetHeightFromFloat(PS.LowBRef,PS.TMin,PS.TDiff,PS.LowP);
}
if(SetTexLightMode)
{
	PS.Etat|=PROFIL3D_TEXLIGHT;
	PS.Etat&=~PROFIL3D_TEXMAP;
	if(
		(!SG_IsValidTex(PS.T))||(PS.T.SizeX!=PS.STX)||(PS.T.SizeY!=PS.STY)
	  )
	{
		if(SG_IsValidTex(PS.T))
		{
			SG_DetachTexture(PS.Vue,PS.T);
			SG_CloseTexture(PS.T);
		}
		SG_CreateTexture(PS.T,PS.STX,PS.STY);
		SG_AttachTexture(PS.Vue,PS.T,SG_FirstFreeTex(PS.Vue));//attache la texture a la vue 3D
	}
	C256_FillTexture(PS.T,PS.ColorTable);
	DbgCHECK(ChangeTexZ==0,"Profil3D_UpdateColorMaps: Operation ambigue");
}
if(ChangeTexZ)
{
	DbgCHECK((PS.Etat&PROFIL3D_TEXLIGHT)==0,"Profil3D_UpdateColorMaps: Passer en TEXLIGHT"); 

	float TMin,TMax;
	if(P_Etat(PS.HiP))
	{
		P_FindMinMax(PS.HiP,TMin,TMax);
	}
	else if(P_Etat(PS.LowP))
	{
		P_FindMinMax(PS.LowP,TMin,TMax);
	}
	else 
		return;
	float TDiff=TMax-TMin;
	if (PS.HiBRef.Etat) Profil3D_SetTexFromFloat(PS.HiBRef,TMin,TDiff,PS.T,PS.HiP);
	if (PS.LowBRef.Etat) Profil3D_SetTexFromFloat(PS.LowBRef,TMin,TDiff,PS.T,PS.LowP);
	if (PS.HiB.Etat) Profil3D_SetTexFromFloat(PS.HiB,TMin,TDiff,PS.T,PS.HiP);
	if (PS.LowB.Etat) Profil3D_SetTexFromFloat(PS.LowB,TMin,TDiff,PS.T,PS.LowP);
}
	return;
}

void SPG_CONV Profil3D_UpdateButtons(PROFIL3D_STATE& PS)
{
	B_UpdateButtonsLib(PS.BL, SPG_Global_MouseX, SPG_Global_MouseY, SPG_Global_MouseLeft);
	
	if(B_IsChangedToClick(PS.BL,PS.LockPos))
	{
		memset(&PS.Rotation,0,sizeof(V_VECT));
	}
	if(B_IsChangedToClick(PS.BL,PS.FrontView))
	{
		V_Operate2(PS.V_Rep.axex,=PS.Vue.Rep.axey);
		V_Operate2(PS.V_Rep.axey,=PS.Vue.Rep.axez);
		V_Operate2(PS.V_Rep.axez,=-PS.Vue.Rep.axex);
	}
	if(B_IsChangedToClick(PS.BL,PS.NewColor))
	{
		C256_Init(PS.ColorTable,(int)(1+(PS.ImageTimer.StartTime&3)),(int)(PS.ImageTimer.StartTime),(int)(PS.ImageTimer.StartTime^(PS.ImageTimer.StartTime<<1)));
		Profil3D_UpdateColorMaps(PS,0,0,1,1);
	}
	if(B_IsChangedToClick(PS.BL,PS.DefaultColor))
	{
		C256_Init(PS.ColorTable,0,0,0);
		Profil3D_UpdateColorMaps(PS,0,0,1,1);
	}
	if(B_IsChangedToClick(PS.BL,PS.LoadColors))
	{
		C256_Load(PS.ColorTable,"Colors");
		Profil3D_UpdateColorMaps(PS,0,0,1,1);
	}
	if(B_IsChangedToClick(PS.BL,PS.BSaveColors))
	{
		C256_Save(PS.ColorTable,"Colors");
	}
	if(B_IsChangedToClick(PS.BL,PS.BTexMap))
	{
		char TName[MaxProgDir];
		strcpy(TName,"Texture.bmp");
		if(SPG_GetLoadName(SPG_BMP,TName,MaxProgDir))
		{
			SG_DetachTexture(PS.Vue,PS.T);
			SG_CloseTexture(PS.T);
			SG_LoadTexture(PS.T,TName);
			SG_AttachTexture(PS.Vue,PS.T,SG_FirstFreeTex(PS.Vue));
			Profil3D_TexMap(PS.LowBRef,PS.T,P_SizeX(PS.LowP),P_SizeY(PS.LowP));
			Profil3D_TexMap(PS.HiBRef,PS.T,P_SizeX(PS.HiP),P_SizeY(PS.HiP));
			Profil3D_TexMap(PS.LowB,PS.T,P_SizeX(PS.LowP),P_SizeY(PS.LowP));
			Profil3D_TexMap(PS.HiB,PS.T,P_SizeX(PS.HiP),P_SizeY(PS.HiP));
			PS.Etat|=PROFIL3D_TEXMAP;
			PS.Etat&=~PROFIL3D_TEXLIGHT;
		}
		else
		{
			Profil3D_UpdateColorMaps(PS,0,0,1,1);
		}
	}
	if(B_IsChangedToClick(PS.BL,PS.BMaskMin))
	{
		int y;
		for(y=0;y<(P_SizeY(PS.HiP)-1);y++)
		{
			int x;
			for(x=0;x<(P_SizeX(PS.HiP)-1);x++)
			{
				if (
					(PS.HiP.D[x+P_SizeX(PS.HiP)*y]<=PS.TMin)||
					(PS.HiP.D[x+1+P_SizeX(PS.HiP)*y]<=PS.TMin)||
					(PS.HiP.D[x+P_SizeX(PS.HiP)*(y+1)]<=PS.TMin)||
					(PS.HiP.D[x+1+P_SizeX(PS.HiP)*(y+1)]<=PS.TMin)
					)
					PS.HiP.Msk[x+P_SizeX(PS.HiP)*y]=1;
			}
		}
		if(P_Etat(PS.LowP))
		{
		for(y=0;y<(P_SizeY(PS.LowP)-1);y++)
		{
			int x;
			for(x=0;x<(P_SizeX(PS.LowP)-1);x++)
			{
				if (
					(PS.LowP.D[x+P_SizeX(PS.LowP)*y]<=PS.TMin)||
					(PS.LowP.D[x+1+P_SizeX(PS.LowP)*y]<=PS.TMin)||
					(PS.LowP.D[x+P_SizeX(PS.LowP)*(y+1)]<=PS.TMin)||
					(PS.LowP.D[x+1+P_SizeX(PS.LowP)*(y+1)]<=PS.TMin)
					)
					PS.LowP.Msk[x+P_SizeX(PS.LowP)*y]=1;
			}
		}
		}
		Profil3D_UpdateColorMaps(PS,1,0,0,0);
	}
#ifndef Pure3D
	if(B_IsChangedToClick(PS.BL,PS.XYCut))
	{
		PS.XYSelOk=0;
		PS.MSSS=0;
		PS.BL.B[PS.LightAjust].Etat=0;
		PS.BL.B[PS.MecaMove].Etat=0;
		B_RedrawButtonsLib(PS.BL,0);
	}
#endif
	if(B_IsChangedToClick(PS.BL,PS.LightAjust))
	{
		//il faut utiliser B_Set par ecrire comme ca (a finir)
#ifndef Pure3D
		PS.BL.B[PS.XYCut].Etat=0;
#endif
		PS.BL.B[PS.MecaMove].Etat=0;
		PS.MSSS=0;
		B_RedrawButtonsLib(PS.BL,0);
	}
	if(B_IsChangedToClick(PS.BL,PS.MecaMove))
	{
#ifndef Pure3D
		PS.BL.B[PS.XYCut].Etat=0;
#endif
		PS.BL.B[PS.LightAjust].Etat=0;
		PS.MSSS=0;
		B_RedrawButtonsLib(PS.BL,0);
	}
	if(B_IsChanged(PS.BL,PS.HeightAjust))
	{
			Profil3D_UpdateColorMaps(PS,0,1,0,0);
	}
	if (B_IsChangedToClick(PS.BL,PS.BMedianFilter))
	{
		//attention lorsque HiResDraw=0
		IF_HIRES
		{
			Profil Ptmp;
			P_Create(Ptmp,P_SizeX(PS.HiP),P_SizeY(PS.HiP),P_XScale(PS.HiP),P_YScale(PS.HiP),P_UnitX(PS.HiP),P_UnitY(PS.HiP),P_UnitZ(PS.HiP),0);
			P_MedianFilter(PS.HiP,Ptmp,3);
			P_Close(Ptmp);
			Profil3D_UpdateColorMaps(PS,0,1,PS.Etat&PROFIL3D_TEXLIGHT,PS.Etat&PROFIL3D_TEXLIGHT);
		}
		else
		{
			Profil Ptmp;
			P_Create(Ptmp,P_SizeX(PS.LowP),P_SizeY(PS.LowP),P_XScale(PS.LowP),P_YScale(PS.LowP),P_UnitX(PS.LowP),P_UnitY(PS.LowP),P_UnitZ(PS.LowP),0);
			P_MedianFilter(PS.LowP,Ptmp,3);
			P_Close(Ptmp);
			Profil3D_UpdateColorMaps(PS,0,1,PS.Etat&PROFIL3D_TEXLIGHT,PS.Etat&PROFIL3D_TEXLIGHT);
		}
	}
	if (B_IsClick(PS.BL,PS.BConvFilter))
	{
		IF_HIRES
		{
			P_MaskConv3x3(PS.HiP);
			Profil3D_UpdateColorMaps(PS,0,1,PS.Etat&PROFIL3D_TEXLIGHT,PS.Etat&PROFIL3D_TEXLIGHT);
		}
		else
		{
			P_MaskConv3x3(PS.LowP);
			Profil3D_UpdateColorMaps(PS,0,1,PS.Etat&PROFIL3D_TEXLIGHT,PS.Etat&PROFIL3D_TEXLIGHT);
		}
	}
	if (B_IsClick(PS.BL,PS.BSpace))
	{
		IF_HIRES
		{
			Profil3D_BlackHole(PS.HiBRef);
		}
		else
		{
			Profil3D_BlackHole(PS.LowBRef);
		}
	}
	if (B_IsChangedToClick(PS.BL,PS.BRevY))
	{
		if (P_Etat(PS.HiP)) P_YReverse(PS.HiP);
		if (P_Etat(PS.LowP)) P_YReverse(PS.LowP);
		Profil3D_UpdateColorMaps(PS,1,1,PS.Etat&PROFIL3D_TEXLIGHT,PS.Etat&PROFIL3D_TEXLIGHT);
	}
	if(B_IsChanged(PS.BL,PS.BManualScale))
	{
		if (PS.HiBRef.Etat) Profil3D_SetTexFromFloat(PS.HiBRef,PS.TMin,PS.ZScale,PS.T,PS.HiP);
		if (PS.LowBRef.Etat) Profil3D_SetTexFromFloat(PS.LowBRef,PS.TMin,PS.ZScale,PS.T,PS.LowP);
		if (PS.HiB.Etat) Profil3D_SetTexFromFloat(PS.HiB,PS.TMin,PS.ZScale,PS.T,PS.HiP);
		if (PS.LowB.Etat) Profil3D_SetTexFromFloat(PS.LowB,PS.TMin,PS.ZScale,PS.T,PS.LowP);
	}
	CD_G_CHECK_EXIT(16,2);

	return;
}

void SPG_CONV Profil3D_Update(PROFIL3D_STATE& PS)
{
	CHECK(PS.Etat==0,"Profil3D_Update: PROFIL3D_STATE non initialise",return);
#ifdef DebugSGRAPH			
//	if (SG_VueCheck(PS.Vue)==0) return;
#endif
#ifdef DebugProfil3D
	S_StopTimer(PS.Timer[TmOutside]);
	S_StartTimer(PS.Timer[TmButtons]);
#endif
	Profil3D_UpdateButtons(PS);
		
#ifdef DebugProfil3D
	S_StopTimer(PS.Timer[TmButtons]);

	S_StartTimer(PS.Timer[TmGenerate3D]);
#endif

	S_GetTimerRunningTimeAndRestart(PS.ImageTimer,PS.DT);
#ifdef SPG_General_USEAVI
	if(Global.AVISG.Etat) PS.DT=Global.AVISG.DT;
#endif
			
	//V_Orthonorme(PS.V_Rep);

	V_RotateRep(PS.V_Rep,PS.DT*,PS.Rotation);

	{
		V_VECT TrueLight;
		float NrmLight;
		V_ModVect(PS.Light,NrmLight)
		float NLightIntensity=PS.LightIntensity/NrmLight;
		V_Operate2(TrueLight,=NLightIntensity*PS.Light);
		
	IF_HIRES
	{
		V_GenereBloc(PS.HiBRef,PS.HiB,PS.V_Rep);
	}
	else
	{
		if(P_Etat(PS.LowP)) V_GenereBloc(PS.LowBRef,PS.LowB,PS.V_Rep);
	}

#ifdef DebugProfil3D
	S_StopTimer(PS.Timer[TmGenerate3D]);

	S_StartTimer(PS.Timer[TmLight]);
#endif

	IF_SG_RenderWithoutTexNorLight(PS.Vue)
	{
		if(PS.Etat&PROFIL3D_TEXLIGHT)
		{
			IF_HIRES
			{
				C256_MakeFaceColor(PS.HiB.DB, P_SizeX(PS.HiP)-1,P_SizeY(PS.HiP)-1,PS.T,TrueLight,PS.Diffusivity);
			}
			else
			{
				if(P_Etat(PS.LowP)) C256_MakeFaceColor(PS.LowB.DB, P_SizeX(PS.LowP)-1,P_SizeY(PS.LowP)-1,PS.T,TrueLight,PS.Diffusivity);
			}
		}
	}
	else
	{
		IF_HIRES
		{
			C256_MakeFaceBlend(PS.HiB,PS.HiBRef,TrueLight,PS.Diffusivity);
		}
		else
		{
			if(P_Etat(PS.LowP)) C256_MakeFaceBlend(PS.LowB,PS.LowBRef,TrueLight,PS.Diffusivity);
		}
	}

#ifdef DebugProfil3D
	S_StopTimer(PS.Timer[TmLight]);
#endif
		

		if(B_IsClick(PS.BL,PS.ShowAxes))
		{
			V_GenereBloc(PS.AXESRef,PS.AXES,PS.V_Rep);
			C256_MakeFaceBlend(PS.AXES,PS.AXESRef,TrueLight,PS.Diffusivity);
		}
	}
	
	//S_StopTimer(PS.Timer[TmGenerate3D]);

#ifdef DebugProfil3D
	S_StartTimer(PS.Timer[TmInitRender]);
#endif

	SG_ClearVue(PS.Vue);

#ifndef SGE_EMC
	if (PS.Vue.ModeV == 32)
		CLBD32(PS.Vue.MemEcr, PS.Vue.PixTX, PS.Vue.PixTY, 0x0000FF, 0x00C0C0C0, PS.Vue.Pitch);
	else
		CLBD(PS.Vue.MemEcr, PS.Vue.PixTX, PS.Vue.PixTY, 0x0000FF, 0x00C0C0C0, PS.Vue.Pitch);
#endif
	IF_HIRES
	{
#ifndef SGE_EMC
		PS.Vue.Etapes=2;
#endif
		SG_AddToVue(PS.Vue,PS.HiB);
	}
	else
	{
#ifndef SGE_EMC
		PS.Vue.Etapes=2;
#endif
		SG_AddToVue(PS.Vue,PS.LowB);
	}

#ifndef SGE_EMC
	if(B_IsChanged(PS.BL,PS.HiResDraw)||(PS.HiResDraw==0)) PS.Vue.Etapes=0;
	if(B_IsChangedToClick(PS.BL,PS.LockPos)) PS.Vue.Etapes=0;
#endif
			
	if(B_IsClick(PS.BL,PS.ShowAxes)) SG_AddToVue(PS.Vue,PS.AXES);
			

#ifdef DebugProfil3D
	S_StopTimer(PS.Timer[TmInitRender]);

	S_StartTimer(PS.Timer[TmRender]);
#endif
#ifdef AllowStereo	
	if(PS.IsStereo)
	{
		SPG_SV_Render(PS.SV);
	}
	else
#endif
	{
		SG_TFACES(PS.Vue);
	}
#ifdef DebugProfil3D
	S_StopTimer(PS.Timer[TmRender]);
#endif

	SG_DrawFPS(PS.SEGraph,PS.CL,PS.DT);

	if(B_IsClick(PS.BL,PS.BShowScale)) Profil3D_DrawScale(PS);

#ifndef Pure3D
	//a finir il reste un pb quand le profil n'est pas hires justement le bt n'est pas clique
	if(PS.XYSelOk)
	{
		IF_HIRES
		{
			if (B_IsChangedToClick(PS.BL,PS.BSaveCutX)) T_SaveXCut(PS.HiP.D,P_SizeX(PS.HiP),P_SizeY(PS.HiP),PS.YSel);
			if (B_IsChangedToClick(PS.BL,PS.BSaveCutY)) T_SaveYCut(PS.HiP.D,P_SizeX(PS.HiP),P_SizeY(PS.HiP),PS.XSel);
		}
		else
		{
			if (B_IsChangedToClick(PS.BL,PS.BSaveCutX)) T_SaveXCut(PS.HiP.D,P_SizeX(PS.HiP),P_SizeY(PS.HiP),PS.YSel*P_SizeY(PS.HiP)/P_SizeY(PS.LowP));
			if (B_IsChangedToClick(PS.BL,PS.BSaveCutY)) T_SaveYCut(PS.HiP.D,P_SizeX(PS.HiP),P_SizeY(PS.HiP),PS.XSel*P_SizeX(PS.HiP)/P_SizeX(PS.LowP));
		}
	}
#endif
			
	if (G_IsInEcran(PS.SEGraph,SPG_Global_MouseX,SPG_Global_MouseY)&&(PS.BL.FocusButton==0))
	{
#ifndef Pure3D
		if(B_IsClick(PS.BL,PS.XYCut))
		{
			IF_HIRES
				PS.XYSelOk=T_Find3DXYCut(PS.SEGraph,SPG_Global_MouseX,SPG_Global_MouseY,PS.HiB,P_SizeX(PS.HiP),P_SizeY(PS.HiP),1+(2*PS.SEGraph.SizeY)/P_SizeY(PS.HiP),PS.XSel,PS.YSel);
			else
				PS.XYSelOk=T_Find3DXYCut(PS.SEGraph,SPG_Global_MouseX,SPG_Global_MouseY,PS.LowB,P_SizeX(PS.LowP),P_SizeY(PS.LowP),1+(2*PS.SEGraph.SizeY)/P_SizeY(PS.LowP),PS.XSel,PS.YSel);
		}		
#endif
		if (SPG_Global_MouseLeft)
		{
#ifndef Pure3D
			B_Set(PS.BL,PS.XYCut,B_Normal);
#endif
			B_Set(PS.BL,PS.LightAjust,B_Normal);
			B_RedrawButtonsLib(PS.BL,0);
		}
#ifndef Pure3D							
		if(PS.XYSelOk)
		{
			IF_HIRES
				T_Draw3DXYCut(PS.SEGraph,PS.HiB,PS.HiP.D,P_SizeX(PS.HiP),P_SizeY(PS.HiP),PS.XSel,PS.YSel, PS.CL);
			else
				T_Draw3DXYCut(PS.SEGraph,PS.LowB,PS.LowP.D,P_SizeX(PS.LowP),P_SizeY(PS.LowP),PS.XSel,PS.YSel, PS.CL);
			IF_HIRES
			{
				T_Draw2DXYCut(PS.SECoupeX,PS.SECoupeY,PS.HiP.D,P_SizeX(PS.HiP),P_SizeY(PS.HiP),PS.XSel,PS.YSel, PS.CL);
			}
			else
			{
				T_Draw2DXYCut(PS.SECoupeX,PS.SECoupeY,PS.LowP.D,P_SizeX(PS.LowP),P_SizeY(PS.LowP),PS.XSel,PS.YSel, PS.CL);
			}
			
		}
#endif
		if (B_IsClick(PS.BL,PS.LightAjust))
		{
			PS.Light.x=(-1+2*((float)(SPG_Global_MouseX-PS.SEGraph.PosX))/PS.SEGraph.SizeX);
			PS.Light.z=(1-2*((float)(SPG_Global_MouseY-PS.SEGraph.PosY))/PS.SEGraph.SizeY);
		}
		
		if (B_IsClick(PS.BL,PS.MecaMove))
		{
			if(SPG_Global_MouseRight&&SPG_Global_MouseLeft)
			{
				if (PS.MSSS!=1)
				{
					PS.MSTX=SPG_Global_MouseX;
					PS.MSTY=SPG_Global_MouseY;
					V_SetXYZ(PS.TMSTR,0,0,0);
					PS.MSSS=1;
				}
				else if (PS.MSSS)
				{
					PS.TMSTR.z=0.8f*PS.TMSTR.z-(SPG_Global_MouseY-PS.MSTY);
					PS.TMSTR.x=0.8f*PS.TMSTR.x+SPG_Global_MouseX-PS.MSTX;
					//V_VECT TheRotationGlob={Global.MouseY-MSSY,0,Global.MouseX-MSSX};
					V_VECT TheTranslLoc;
					V_ProjRep(PS.V_Rep,PS.TMSTR,TheTranslLoc);
					PS.MSTX=SPG_Global_MouseX;
					PS.MSTY=SPG_Global_MouseY;
					V_TranslateRep(PS.V_Rep,0.001f*,TheTranslLoc);
				}
			}
			else if(SPG_Global_MouseLeft)
			{
				if (PS.MSSS!=2)
				{
					PS.MSSX=SPG_Global_MouseX;
					PS.MSSY=SPG_Global_MouseY;
					V_SetXYZ(PS.TMSSR,0,0,0);
					PS.MSSS=2;
				}
				else if (PS.MSSS==2)
				{
					PS.TMSSR.x=0.8f*PS.TMSSR.x+SPG_Global_MouseY-PS.MSSY;
					PS.TMSSR.z=0.8f*PS.TMSSR.z+SPG_Global_MouseX-PS.MSSX;
					//V_VECT TheRotationGlob={Global.MouseY-MSSY,0,Global.MouseX-MSSX};
					V_VECT TheRotationLoc;
					V_ProjRep(PS.V_Rep,PS.TMSSR,TheRotationLoc);
					V_RotateRep(PS.V_Rep,0.005f*,TheRotationLoc);
					PS.MSSX=SPG_Global_MouseX;
					PS.MSSY=SPG_Global_MouseY;
				}
			}
			else if(SPG_Global_MouseRight)
			{
				if (PS.MSSS!=3)
				{
					PS.MSSX=SPG_Global_MouseX;
					PS.MSSY=SPG_Global_MouseY;
					V_SetXYZ(PS.TMSSR,0,0,0);
					PS.MSSS=3;
				}
				else if (PS.MSSS==3)
				{
					PS.TMSSR.y=0.8f*PS.TMSSR.y+(SPG_Global_MouseY-PS.MSSY)*((float)(SPG_Global_MouseX-PS.SEGraph.PosX)/(PS.SEGraph.SizeX)-0.5f);
					//PS.TMSSR.z=0.8*PS.TMSSR.z+Global.MouseX-PS.MSSX;
					//V_VECT TheRotationGlob={Global.MouseY-MSSY,0,Global.MouseX-MSSX};
					V_VECT TheRotationLoc;
					V_ProjRep(PS.V_Rep,PS.TMSSR,TheRotationLoc);
					V_RotateRep(PS.V_Rep,0.005f*,TheRotationLoc);
					V_SetXYZ(TheRotationLoc,0,0,V_IntToFloat(SPG_Global_MouseX-PS.MSSX));
					V_RotateRep(PS.V_Rep,0.005f*,TheRotationLoc);
					PS.MSSX=SPG_Global_MouseX;
					PS.MSSY=SPG_Global_MouseY;
				}
			}
			else
			{
				PS.MSSS=0;
			}
		}
		else
		{
			PS.MSSS=0;
		}
	}
	
	/*
	G_DrawRect(E,Global.MouseX-2,Global.MouseY-2,Global.MouseX+1,Global.MouseY+1,0);
	G_DrawPixel(E,Global.MouseX-1,Global.MouseY-1,0xffffffff);
	*/
#ifdef SPG_General_USESGRAPH_OPTS
	SG_UpdateOpts(PS.SGopts);
#endif
	
	if (B_IsChangedToClick(PS.BL,PS.SaveProfil))
	{
		P_Save(PS.HiP,"ProfilZ");
	}
	if (B_IsChangedToClick(PS.BL,PS.SavePicture))
	{
		G_SaveAs_ToBMP(PS.SEGraph,"Vue3D.bmp");
	}

#ifdef DebugProfil3D
	S_StartTimer(PS.Timer[TmOutside]);
#endif
	return;
}

void SPG_CONV Profil3D_Close(PROFIL3D_STATE& PS)
{
	CHECK(PS.Etat==0,"Profil3D_Close: PROFIL3D_STATE non initialise",return);

#ifdef DebugProfil3D
	S_StopTimer(PS.Timer[TmOutside]);
	S_StopTimer(PS.Timer[TmGeneral]);
#endif

	SG_CloseBloc(PS.HiB);
	SG_CloseBloc(PS.HiBRef);
	P_Close(PS.HiP);

	if(PS.LowB.Etat) SG_CloseBloc(PS.LowB);
	if(PS.LowBRef.Etat) SG_CloseBloc(PS.LowBRef);
	if(P_Etat(PS.LowP)) P_Close(PS.LowP);

	SG_CloseBloc(PS.AXESRef);
	SG_CloseBloc(PS.AXES);

#ifdef SPG_General_USESGRAPH_OPTS
	SG_CloseOpts(PS.SGopts);
#endif
#ifdef AllowStereo	
	if(PS.IsStereo)
	{
		SPG_SV_Close(PS.SV);
	}
#endif
	SG_DetachTexture(PS.Vue,PS.T);
	SG_CloseTexture(PS.T);
	SG_CloseVue(PS.Vue);

	C256_Close(PS.ColorTable);
	
	B_CloseButtonsLib(PS.BL);
	
	C_CloseCaracLib(PS.CL);
	
	G_CloseEcran(PS.SEGraph);
#ifndef Pure3D							
	G_CloseEcran(PS.SECoupeX);
	G_CloseEcran(PS.SECoupeY);
#endif
	G_CloseEcran(PS.SEBoutons);
	if(G_Etat(PS.SEopts)) G_CloseEcran(PS.SEopts);



	S_StopTimer(PS.ImageTimer);
	S_CloseTimer(PS.ImageTimer);
#ifdef DebugProfil3D
	S_PrintRatio(PS.Timer,7);
	S_CloseTimer(PS.Timer[TmGeneral]);
	S_CloseTimer(PS.Timer[TmGenerate3D]);
	S_CloseTimer(PS.Timer[TmLight]);
	S_CloseTimer(PS.Timer[TmInitRender]);
	S_CloseTimer(PS.Timer[TmRender]);
	S_CloseTimer(PS.Timer[TmButtons]);
	S_CloseTimer(PS.Timer[TmOutside]);
#endif

	memset(&PS,0,sizeof(PROFIL3D_STATE));
	return;
}



#endif

