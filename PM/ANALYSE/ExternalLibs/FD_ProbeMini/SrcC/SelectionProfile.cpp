
#include "SPG_General.h"

#ifdef SPG_General_USECut
#ifdef SPG_General_USEProfil
#ifdef SPG_General_USEGraphics

#include "SPG_Includes.h"

#include <string.h>
#include <stdio.h>
//abs
#include <stdlib.h>

//creation (initialisation du pointeur vars le profil)
int SPG_CONV SP_Create(SelectionProfile& SP,Profil& P)
{
	//CHECK(P_Etat(P)==0,"SP_Create: profil invalide",return 0);
	SP.Etat=1;
	SP.P=&P;
	SP.NSel=0;
	return -1;
}

//close
void SPG_CONV SP_Close(SelectionProfile& SP)
{
	SP.Etat=0;
	SP.NSel=0;
}

void SPG_CONV SP_SelectProfil(SelectionProfile& SP, Profil& P)
{
	CHECK(P_Etat(P)==0,"SP_SelectProfil: profil invalide",SP.P=0;return);
	SP.P=&P;
	return;
}

//dessine sur l'ecran en tenant compte des positions des scrollbars
void SPG_CONV SP_Draw(SelectionProfile& SP,G_Ecran& E, int xpos, int ypos)
{
	if (SP.NSel==0) return;
	if (SP.P==0) return;
	if (SP.P->H.Etat==0) return;
	int i;
	DWORD LineCoul=0xff00ff;
	DWORD CPixCoul=0x00ff00;
	//parcours des points de selection
	for(i=0;i<SP.NSel-1;i++)
	{
		//ligne
		G_DrawLine(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos,SP.PosX[i+1]+xpos,SP.PosY[i+1]+ypos,LineCoul);
		//petites croix
		G_DrawPixel(E,SP.PosX[i]+xpos-1,SP.PosY[i]+ypos,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos+1,SP.PosY[i]+ypos,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos-1,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos+1,CPixCoul);
	}
		//petites croix
		G_DrawPixel(E,SP.PosX[i]+xpos-1,SP.PosY[i]+ypos-1,0xff00);//vert
		G_DrawPixel(E,SP.PosX[i]+xpos+1,SP.PosY[i]+ypos-1,0xff00);
		G_DrawPixel(E,SP.PosX[i]+xpos-1,SP.PosY[i]+ypos+1,0xff00);
		G_DrawPixel(E,SP.PosX[i]+xpos+1,SP.PosY[i]+ypos+1,0xff00);

		i=0;
		//petites croix
		G_DrawPixel(E,SP.PosX[i]+xpos-1,SP.PosY[i]+ypos,0xff0000);//rouge
		G_DrawPixel(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos,0xff0000);
		G_DrawPixel(E,SP.PosX[i]+xpos+1,SP.PosY[i]+ypos,0xff0000);
		G_DrawPixel(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos-1,0xff0000);
		G_DrawPixel(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos+1,0xff0000);
	return;
}

void SPG_CONV SP_DrawLarge(SelectionProfile& SP,G_Ecran& E, int xpos, int ypos)
{
	if (SP.NSel==0) return;
	if (SP.P==0) return;
	if (SP.P->H.Etat==0) return;
	int i;
	DWORD LineExt=0xff80ff;
	DWORD LineInt=0x000000;
	DWORD CPixCoul=0x80ff80;
	//parcours des points de selection
	for(i=0;i<SP.NSel-1;i++)
	{
		//ligne
		G_DrawLine(E,SP.PosX[i]+xpos+1,SP.PosY[i]+ypos,SP.PosX[i+1]+xpos+1,SP.PosY[i+1]+ypos,LineExt);
		G_DrawLine(E,SP.PosX[i]+xpos-1,SP.PosY[i]+ypos,SP.PosX[i+1]+xpos-1,SP.PosY[i+1]+ypos,LineExt);
		G_DrawLine(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos+1,SP.PosX[i+1]+xpos,SP.PosY[i+1]+ypos+1,LineExt);
		G_DrawLine(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos-1,SP.PosX[i+1]+xpos,SP.PosY[i+1]+ypos-1,LineExt);
		G_DrawLine(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos,SP.PosX[i+1]+xpos,SP.PosY[i+1]+ypos,LineInt);
		//petites croix
		G_DrawPixel(E,SP.PosX[i]+xpos-1,SP.PosY[i]+ypos,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos+1,SP.PosY[i]+ypos,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos-1,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos+1,CPixCoul);
	}
		//grosse croix
		G_DrawPixel(E,SP.PosX[i]+xpos-1,SP.PosY[i]+ypos-1,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos+1,SP.PosY[i]+ypos-1,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos-1,SP.PosY[i]+ypos+1,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos+1,SP.PosY[i]+ypos+1,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos-2,SP.PosY[i]+ypos,LineInt);
		G_DrawPixel(E,SP.PosX[i]+xpos+2,SP.PosY[i]+ypos,LineInt);
		G_DrawPixel(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos-2,LineInt);
		G_DrawPixel(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos+2,LineInt);

		i=0;
		//grosse croix
		G_DrawPixel(E,SP.PosX[i]+xpos-1,SP.PosY[i]+ypos-1,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos+1,SP.PosY[i]+ypos-1,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos-1,SP.PosY[i]+ypos+1,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos+1,SP.PosY[i]+ypos+1,CPixCoul);
		G_DrawPixel(E,SP.PosX[i]+xpos-2,SP.PosY[i]+ypos,LineInt);
		G_DrawPixel(E,SP.PosX[i]+xpos+2,SP.PosY[i]+ypos,LineInt);
		G_DrawPixel(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos-2,LineInt);
		G_DrawPixel(E,SP.PosX[i]+xpos,SP.PosY[i]+ypos+2,LineInt);
	return;
}

//ajouter un point de selection
void SPG_CONV SP_Add(SelectionProfile& SP, int x, int y)//ajoute un point de selection
{
	if (SP.P==0) return;
	if (SP.P->H.Etat==0) return;

//force la selection a etre a au moins un pixel du bord a cause du resampling
	x=V_Sature(x,1,(SP.P->H.SizeX-2));
	y=V_Sature(y,1,(SP.P->H.SizeY-2));
	if (SP.NSel)//ne pas ajouter deux fois le meme point
	{
		if ((SP.PosX[SP.NSel-1]==x)&&(SP.PosY[SP.NSel-1]==y)) return;
	}
	SP.PosX[SP.NSel]=x;
	SP.PosY[SP.NSel]=y;
	SP.NSel++;
//protection contre un depassement du nombre de points (SelectionProfile.h\define MaxSel)
	SP.NSel=V_Min(SP.NSel,MaxSel-1);
}

//supprime le dernier point
void SPG_CONV SP_Del(SelectionProfile& SP)
{
	SP.NSel--;
	SP.NSel=V_Max(SP.NSel,0);
}

//cree une coupe a partir de la selection
int SPG_CONV SP_CreateCut(SelectionProfile& SP,Cut& C)
{
	memset(&C,0,sizeof(Cut));

	if (SP.NSel<=1) return 0;//il faut au moins deux points
	if (SP.P==0) return 0;
	if (SP.P->H.Etat==0) return 0;

//	CHECK(SP.P->Etat==0,"SP_CreateCut: Profil nul",return 0);

	int NpTotal=0;
	int i;
//calcul de la longueur a un echantillon par pixel ex
//000000
//00x000
//00x000
//000x00
//000x00
//0000x0
//0000x0
//000000
//donne L=6
	for(i=0;i<(SP.NSel-1);i++)
	{
		NpTotal+=(V_Max(abs(SP.PosX[i+1]-SP.PosX[i]),abs(SP.PosY[i+1]-SP.PosY[i])));
	}

//cree la coupe de la longueur calculee
	Cut_Create(C,NpTotal,SP.P->H.SizeX,SP.P->H.UnitX,SP.P->H.UnitZ);//reprendre de SP.P....

	CHECK(C.Etat==0,"SP_CreateCut: Cut_Create echoue",return 0);

	int n=0;

//parcours des lignes
	for(i=0;i<(SP.NSel-1);i++)
	{
		if (abs(SP.PosX[i+1]-SP.PosX[i])>=abs(SP.PosY[i+1]-SP.PosY[i]))
		{//parcours selon x si la ligne est plutot horizontale
			int x;
			for(x=SP.PosX[i];x!=SP.PosX[i+1];x+=(V_Signe(SP.PosX[i+1]-SP.PosX[i])))
			{//
				CHECK(n==NpTotal,"Extraction de coupe buggée",break);
				int y=(((x-SP.PosX[i])*SP.PosY[i+1]+(SP.PosX[i+1]-x)*SP.PosY[i]))/(SP.PosX[i+1]-SP.PosX[i]);
				/*
				if (V_IsBound(x,0,SP.P->SizeX)&&V_IsBound(y,0,SP.P->SizeY))
				//test inutile si dans SP_Add on enleve les bords
					C.D[n]=SP.P->D[x+SP.P->SizeX*y];
				else
					C.D[n]=1;
					*/
				C.D[n]=SP.P->D[x+SP.P->H.SizeX*y];
				n++;
			}
		}
		else
		{//parcours selon y si la coupe est verticale
			int y;
			for(y=SP.PosY[i];y!=SP.PosY[i+1];y+=(V_Signe(SP.PosY[i+1]-SP.PosY[i])))
			{//
				CHECK(n==NpTotal,"Extraction de coupe buggée",break);
				int x=(((y-SP.PosY[i])*SP.PosX[i+1]+(SP.PosY[i+1]-y)*SP.PosX[i]))/(SP.PosY[i+1]-SP.PosY[i]);
				/*
				if (V_IsBound(x,0,SP.P->SizeX)&&V_IsBound(y,0,SP.P->SizeY))
					C.D[n]=SP.P->D[x+SP.P->SizeX*y];
				else
					C.D[n]=1;
					*/
				C.D[n]=SP.P->D[x+SP.P->H.SizeX*y];
				n++;
			}
		}
	}
	//DbgCHECK(n<NpTotal,"Extraction de coupe legerement buggée\n(Les derniers points sont faux)");
	DbgCHECK(n<NpTotal,"Message de debogage\nExtraction de coupe:\nCondition de test réalisee");

	return -1;
}

//idem avec un facteur de resampling
int SPG_CONV SP_ResampleCut(SelectionProfile& SP,Cut& C,float RsStep)//Step=nb de points de coupe par pixel
{
	memset(&C,0,sizeof(Cut));

	if (SP.NSel<=1) return 0;
	if (SP.P==0) return 0;
	if (SP.P->H.Etat==0) return 0;
	if (RsStep<=0) return 0;

	float LgTotal=0;
	int i;
	for(i=0;i<(SP.NSel-1);i++)
	{
		LgTotal+=sqrt(powfInt((SP.PosX[i+1]-SP.PosX[i]),2)+powfInt((SP.PosY[i+1]-SP.PosY[i]),2));
	}
	int NpTotal=V_FloatToInt(LgTotal/RsStep);

	Cut_Create(C,NpTotal+1,SP.P->H.XScale*RsStep,SP.P->H.UnitX,SP.P->H.UnitZ);

	CHECK(C.Etat==0,"SP_ResampleCut: Cut_Create echoue",return 0);

	int n=0;

//	float CurrAbs=0;
	float CurrSegmentAbs=0;
	float CurrSegmentLongueur=sqrt(powfInt((SP.PosX[1]-SP.PosX[0]),2)+powfInt((SP.PosY[1]-SP.PosY[0]),2));
	int CurrSegment=0;
		C.Decor[0]=1;
	for(i=0;i<NpTotal;i++)
	{
		float xvrai=SP.PosX[CurrSegment]+(SP.PosX[CurrSegment+1]-SP.PosX[CurrSegment])*CurrSegmentAbs/CurrSegmentLongueur;
		float yvrai=SP.PosY[CurrSegment]+(SP.PosY[CurrSegment+1]-SP.PosY[CurrSegment])*CurrSegmentAbs/CurrSegmentLongueur;
		int xmin=V_Floor(xvrai);
		int ymin=V_Floor(yvrai);
		float xdir=xvrai-xmin;
		float ydir=yvrai-ymin;
		float* Source=SP.P->D+xmin+SP.P->H.SizeX*ymin;
		C.D[n]=
			(
			(*Source)*(1-xdir)+
			(*(Source+1))*xdir
			)*(1-ydir)+
			(
			(*(Source+SP.P->H.SizeX))*(1-xdir)+
			(*(Source+SP.P->H.SizeX+1))*xdir
			)*(ydir);
		/*
		C.D[n]=
			(
			SP.P->D[xmin+SP.P->SizeX*ymin]*(1-xdir)+
			SP.P->D[xmin+1+SP.P->SizeX*ymin]*xdir
			)*(1-ydir)+
			(
			SP.P->D[xmin+SP.P->SizeX*(ymin+1)]*(1-xdir)+
			SP.P->D[xmin+1+SP.P->SizeX*(ymin+1)]*xdir
			)*(ydir);
			*/
		n++;
		CurrSegmentAbs+=RsStep;
		while (CurrSegmentAbs>CurrSegmentLongueur) 
		{
			C.Decor[n]=1;
			CurrSegmentAbs-=CurrSegmentLongueur;
			CurrSegment++;
			CurrSegmentLongueur=sqrt(powfInt((SP.PosX[CurrSegment+1]-SP.PosX[CurrSegment]),2)+powfInt((SP.PosY[CurrSegment+1]-SP.PosY[CurrSegment]),2));
			if (CurrSegment>=SP.NSel) 
			{
				C.D[n]=SP.P->D[SP.PosX[CurrSegment]+SP.P->H.SizeX*SP.PosY[CurrSegment]];
				n++;
				goto FinRemplissage;
			}
		}
	}
FinRemplissage:
	//test pour le debogage
	DbgCHECK(n<NpTotal,"Message de debogage\nExtraction de coupe:\nCondition de test réalisee");
	C.NumS=V_Min(n,NpTotal+1);
	C.Decor[C.NumS-1]=1;

	return -1;
}

//idem avec un facteur de resampling et une moyenne sur 3 pixels Y
/*
doit reprendre modele sur SP_ResampleCut: plus a jour
*/
int SPG_CONV SP_ResampleCut3Y(SelectionProfile& SP,Cut& C,float RsStep)//Step=nb de points de coupe par pixel
{
	memset(&C,0,sizeof(Cut));

	if (SP.NSel<=1) return 0;
	if (SP.P==0) return 0;
	if (SP.P->H.Etat==0) return 0;
	if (RsStep<=0) return 0;

	float LgTotal=0;
	int i;
	for(i=0;i<(SP.NSel-1);i++)
	{
		LgTotal+=sqrt(powfInt((SP.PosX[i+1]-SP.PosX[i]),2)+powfInt((SP.PosY[i+1]-SP.PosY[i]),2));
	}
	int NpTotal=LgTotal/RsStep;

	Cut_Create(C,NpTotal+1,SP.P->H.XScale*RsStep,SP.P->H.UnitX,SP.P->H.UnitZ);

	CHECK(C.Etat==0,"SP_ResampleCut: Cut_Create echoue",return 0);

	int n=0;

//	float CurrAbs=0;
	float CurrSegmentAbs=0;
	float CurrSegmentLongueur=sqrt(powfInt((SP.PosX[1]-SP.PosX[0]),2)+powfInt((SP.PosY[1]-SP.PosY[0]),2));
	int CurrSegment=0;
		C.Decor[0]=1;
	for(i=0;i<NpTotal;i++)
	{
		float xvrai=SP.PosX[CurrSegment]+(SP.PosX[CurrSegment+1]-SP.PosX[CurrSegment])*CurrSegmentAbs/CurrSegmentLongueur;
		float yvrai=SP.PosY[CurrSegment]+(SP.PosY[CurrSegment+1]-SP.PosY[CurrSegment])*CurrSegmentAbs/CurrSegmentLongueur;
		int xmin=V_Floor(xvrai);
		int ymin=V_Floor(yvrai);
		float xdir=xvrai-xmin;
		float ydir=yvrai-ymin;
		float* Source=SP.P->D+xmin+SP.P->H.SizeX*ymin;
#define Avg3Y(S) (*(S-SP.P->H.SizeX))+(*(S))+(*(S+SP.P->H.SizeX))
		C.D[n]=
			(
			Avg3Y(Source)*(1-xdir)+
			Avg3Y(Source+1)*xdir
			)*(1-ydir)+
			(
			Avg3Y(Source+SP.P->H.SizeX)*(1-xdir)+
			Avg3Y(Source+SP.P->H.SizeX+1)*xdir
			)*(ydir);
#undef Avg3Y
		n++;
		CurrSegmentAbs+=RsStep;
		while (CurrSegmentAbs>CurrSegmentLongueur) 
		{
			C.Decor[n]=1;
			CurrSegmentAbs-=CurrSegmentLongueur;
			CurrSegment++;
			CurrSegmentLongueur=sqrt(powfInt((SP.PosX[CurrSegment+1]-SP.PosX[CurrSegment]),2)+powfInt((SP.PosY[CurrSegment+1]-SP.PosY[CurrSegment]),2));
			if (CurrSegment>=SP.NSel) 
			{
				C.D[n]=SP.P->D[SP.PosX[CurrSegment]+SP.P->H.SizeX*SP.PosY[CurrSegment]];
				n++;
				goto FinRemplissage;
			}
		}
	}
FinRemplissage:
	//test pour le debogage
	DbgCHECK(n<NpTotal,"Message de debogage\nExtraction de coupe:\nCondition de test réalisee");
	C.NumS=V_Min(n,NpTotal+1);
	C.Decor[C.NumS-1]=1;

	return -1;
}

int SPG_CONV SP_Flatten(SelectionProfile& SP)
{
	CHECK(SP_NumSelPoints(SP)!=2,"SP_Flatten: Il faut selectionner deux points",return 0);

	SPG_StackAlloc(Cut,C);
	SP_ResampleCut(SP,C,0.25);
/*
Cut_Create(C,4,1,"","");
for(int i=0;i<C.NumS;i++)
{
C.D[i]=i;
}
*/
/*
S1(N)=sum(n,n=0,N-1)=N*(N-1)/2
S2(N)=sum(n²,n=0,N-1)=sum(sum(n,i,N-1),i=1,N-1)
sum(n,i,N-1)=S1(N)-S1(i)=S1(N)-i*(i-1)/2=S1-i²/2+i/2
S2=(N-1)*S1(N)-S2(N)/2+S1(N)/2
S2=(2*N*(N-1)²+N*(N-1))/6
S2=(N-1)*(2N²-2N+N)/6
S2=N(N-1)(2N-1)/6
D(n)=an+b+f(n)
sum(f(n),n=0,N-1)=0
sum(nf(n),n=0,N-1)=0
M0=sum(D(n),n=0,N-1)=a*N*(N-1)/2+b*N
M1=sum(nD(n),n=0,N-1)=a*N(N-1)(2N-1)/6+b*N*(N-1)/2
T0=M0/N=a*(N-1)/2+b
T1=2*M1/(N*(N-1))=a*(2N-1)/3+b
D0=T1-T0=a*((2N-1)/3-(N-1)/2)=a*(4N-2-3N+3)/6=a*(N+1)/6
a=6*D0/(N+1)
b=T0-(N-1)/2a
*/
	float M0=0;
	float M1=0;
	for(int n=0;n<C.NumS;n++)
	{
		M0+=C.D[n];
		M1+=n*C.D[n];
	}
	float T0=M0/C.NumS;
	float T1=2.0f*M1/(C.NumS*(C.NumS-1));
	float D0=T1-T0;
	float a=6*D0*(C.NumS-1)/(C.NumS+1);
	float b=T0-0.5f*a;

	Cut_Close(C);

	int DeltaX=SP.PosX[1]-SP.PosX[0];
	int DeltaY=SP.PosY[1]-SP.PosY[0];
	a/=(DeltaX*DeltaX+DeltaY*DeltaY);
	for(int y=0;y<P_SizeY((*(SP.P)));y++)
	{
		for(int x=0;x<P_SizeX((*(SP.P)));x++)
		{
			P_Element((*(SP.P)),x,y)-=(b+
				a*
				((x-SP.PosX[0])*DeltaX+(y-SP.PosY[0])*DeltaY));
		}
	}

	SPG_StackCheck(C);
	return -1;
}

int SPG_CONV SP_Level(SelectionProfile& SP)
{
	CHECK(SP_NumSelPoints(SP)!=2,"SP_Level: Il faut selectionner deux points",return 0);
	float Z1=0;
	int N1=0;
	{
		for(int y=SP.PosY[0]-1;y<SP.PosY[0]+1;y++)
		{
		for(int x=SP.PosX[0]-1;x<SP.PosX[0]+1;x++)
		{
			if(P_IsInProfil( (*(SP.P)),x,y ) )
			{
				if(
					(P_Msk((*(SP.P)))==0) || 
					(P_ElementMsk( (*(SP.P)),x,y ) ==0 )
					)
				{
					Z1+=P_Element( (*(SP.P)),x,y );
					N1++;
				}
			}
		}
		}
	}
	float Z2=0;
	int N2=0;
	{
		for(int y=SP.PosY[1]-1;y<SP.PosY[1]+1;y++)
		{
		for(int x=SP.PosX[1]-1;x<SP.PosX[1]+1;x++)
		{
			if(P_IsInProfil( (*(SP.P)),x,y ) )
			{
				if(
					(P_Msk((*(SP.P)))==0) || 
					(P_ElementMsk( (*(SP.P)),x,y ) ==0 )
					)
				{
					Z2+=P_Element( (*(SP.P)),x,y );
					N2++;
				}
			}
		}
		}
	}
	if(N1&&N2)
	{
	int DeltaX=SP.PosX[1]-SP.PosX[0];
	int DeltaY=SP.PosY[1]-SP.PosY[0];
	float b=Z1/N1;
	float a=(Z2/N2-Z1/N1)/(DeltaX*DeltaX+DeltaY*DeltaY);
	for(int y=0;y<P_SizeY((*(SP.P)));y++)
	{
		for(int x=0;x<P_SizeX((*(SP.P)));x++)
		{
			P_Element((*(SP.P)),x,y)-=(b+
				a*
				((x-SP.PosX[0])*DeltaX+(y-SP.PosY[0])*DeltaY));
		}
	}
	}
	return -1;
}

//creation (initialisation du pointeur vars le profil)
int SPG_CONV ZP_Create(ZoneProfile& ZP,ProfilHeader& PH)
{
	//memset(ZP,0,sizeof(ZoneProfile));
	CHECK(PH.Etat==0,"ZP_Create: profil header invalide",return 0);
	ZP.Etat=1;
	ZP.PH=&PH;
	ZP.PosX=0;
	ZP.PosY=0;
	ZP_SizeX(ZP)=PH.SizeX;
	ZP_SizeY(ZP)=PH.SizeY;
	return -1;
}

int SPG_CONV ZP_Init(ZoneProfile& ZP,int PosX, int PosY, int SizeX, int SizeY)
{
	//memset(ZP,0,sizeof(ZoneProfile));
	ZP.Etat=1;
	ZP.PH=0;
	ZP.PosX=PosX;
	ZP.PosY=PosY;
	ZP_SizeX(ZP)=SizeX;
	ZP_SizeY(ZP)=SizeY;
	return -1;
}

//close
void SPG_CONV ZP_Close(ZoneProfile& ZP)
{
	ZP.Etat=0;
	return;
}

void SPG_CONV ZP_AjustBounds(ZoneProfile& ZP)
{
	if (ZP.PH==0) return;
	if (ZP.PH->Etat==0) return;
	ZP.PosX=V_Sature(ZP.PosX,0,ZP.PH->SizeX-2);
	ZP.PosY=V_Sature(ZP.PosY,0,ZP.PH->SizeY-2);
	ZP_SizeX(ZP)=V_Sature(ZP_SizeX(ZP),2,(ZP.PH->SizeX-ZP.PosX));
	ZP_SizeY(ZP)=V_Sature(ZP_SizeY(ZP),2,(ZP.PH->SizeY-ZP.PosY));
	return;
}

void SPG_CONV ZP_SelectProfil(ZoneProfile& ZP, ProfilHeader& PH)
{
	CHECK(PH.Etat==0,"SP_SelectProfil: profil invalide",ZP.PH=0;return);
	ZP.PH=&PH;
	ZP_AjustBounds(ZP);
	return;
}

void SPG_CONV ZP_Update(ZoneProfile& ZP, int MouseX, int MouseY, int MouseButton)
{
	if(MouseButton==0) 
	{
		ZP.Etat&=~ZP_SEL_CORNER_MSK;
		return;
	}
	else 
	{
		if (ZP_HasFocus(ZP)==0)
		{
			int NumCorner=0;
			int Dist=abs(ZP_CornerCoordX0(ZP)-MouseX)+abs(ZP_CornerCoordY0(ZP)-MouseY);
			int DistTmp=abs(ZP_CornerCoordX1(ZP)-MouseX)+abs(ZP_CornerCoordY1(ZP)-MouseY);
			if (DistTmp<Dist) 
			{
				Dist=DistTmp;
				NumCorner=1;
			}
			DistTmp=abs(ZP_CornerCoordX2(ZP)-MouseX)+abs(ZP_CornerCoordY2(ZP)-MouseY);
			if (DistTmp<Dist) 
			{
				Dist=DistTmp;
				NumCorner=2;
			}
			DistTmp=abs(ZP_CornerCoordX3(ZP)-MouseX)+abs(ZP_CornerCoordY3(ZP)-MouseY);
			if (DistTmp<Dist) 
			{
				Dist=DistTmp;
				NumCorner=3;
			}
			//active la selection (focus)
			if(Dist<8) ZP.Etat|=(ZP_SEL_CORNER_0<<NumCorner);
		}
		if (ZP_HasFocus(ZP))
		{
			switch (ZP.Etat&ZP_SEL_CORNER_MSK)
			{
				case ZP_SEL_CORNER_0:
					ZP.PosX=MouseX;
					ZP.PosY=MouseY;
					break;
				case ZP_SEL_CORNER_1:
					ZP.SizeX=1+MouseX-ZP.PosX;
					ZP.PosY=MouseY;
					break;
				case ZP_SEL_CORNER_2:
					ZP.SizeX=1+MouseX-ZP.PosX;
					ZP.SizeY=1+MouseY-ZP.PosY;
					break;
				case ZP_SEL_CORNER_3:
					ZP.PosX=MouseX;
					ZP.SizeY=1+MouseY-ZP.PosY;
					break;
			}
			ZP_AjustBounds(ZP);
		}
	}
	return;
}

//dessine sur l'ecran en tenant compte des positions des scrollbars
void SPG_CONV ZP_Draw(ZoneProfile& ZP,G_Ecran& E, C_Lib& CL, int xpos, int ypos)
{
	if (ZP.PH==0) return;
	if (ZP.PH->Etat==0) return;
	DWORD LineCoul=0x00ff00;
	G_DrawOutRect(E,ZP_Xmin(ZP)+xpos,ZP_Ymin(ZP)+ypos,ZP_Xmax(ZP)+xpos,ZP_Ymax(ZP)+ypos,LineCoul);
	int Xmid=ZP_Xmin(ZP)+(ZP_SizeX(ZP)>>1)+xpos;
	int Ymid=ZP_Ymin(ZP)+(ZP_SizeY(ZP)>>1)+ypos;
	//petite croix
	G_DrawPixel(E,Xmid-1,Ymid-1,LineCoul);
	G_DrawPixel(E,Xmid+1,Ymid-1,LineCoul);
	G_DrawPixel(E,Xmid-1,Ymid+1,LineCoul);
	G_DrawPixel(E,Xmid+1,Ymid+1,LineCoul);

	if(ZP_HasFocus(ZP))
	{
		char Msg[64];
		sprintf(Msg," X=%d       Y=%d\nDx=%d     Dy=%d\n",ZP.PosX,ZP.PosY,ZP.SizeX,ZP.SizeY);
		CF_GetString(Msg,ZP.SizeX*ZP.PH->XScale,5);
		strcat(Msg,ZP.PH->UnitX);
		strcat(Msg," ");
		CF_GetString(Msg,ZP.SizeY*ZP.PH->YScale,5);
		strcat(Msg,ZP.PH->UnitY);
		C_PrintUni(E,ZP.PosX+xpos+4,ZP.PosY+ypos+4,Msg,CL,0,LineCoul);
	}
	return;
}


#endif
#endif
#endif


