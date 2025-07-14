
#include "SPG_General.h"

#ifdef SPG_General_USEPaint

#include "SPG_Includes.h"

#include <memory.h>
//#include <stdio.h>

#ifdef SPG_PAINT_DEBUG
#define C_Add(Pnt,S) {Console_Add(Pnt.C,S);Console_Update(Pnt.C);DoEvents(SPG_DOEV_ALL);}
#define C_AddF(Pnt,S) {Console_Add(Pnt.C,S);}
#endif

int SPG_CONV SPG_PaintInit(SPG_PAINT& P)
{
	memset(&P,0,sizeof(SPG_PAINT));
#ifdef SPG_PAINT_DEBUG
	C_LoadCaracLib(P.CL,Global.Ecran);
	Console_Create(P.C,&Global.Ecran,&P.CL,0);
#endif
	P.CurrentIndice=1;
	return -1;
}

void SPG_CONV SPG_PaintClose(SPG_PAINT& P)
{
#ifdef SPG_PAINT_DEBUG
	C_CloseCaracLib(P.CL);
	Console_Close(P.C);
#endif
	memset(&P,0,sizeof(SPG_PAINT));
}

void SPG_CONV SPG_PaintFlushByte(SPG_PAINT& P, BYTE* Regions, int Imax)
{
	//return;
#ifdef SPG_PAINT_DEBUG
	C_Add(P,"SPG_PaintFlushByte Enter");
#endif
	P.CurrentIndice=SPG_PaintFlushEqvTable(P);//établit la table de correspondances
	for(int i=0;i<Imax;i++)
	{//réaffecte les numeros
		Regions[i]=P.EqvAeras[Regions[i]];//tous les indices de régions sont ramenés à leur équivalent
	}
	SPG_PaintRestartEqvTable(P);//repart
#ifdef SPG_PAINT_DEBUG
	C_Add(P,"SPG_PaintFlushByte Leave");
#endif
	return;
}

int SPG_FASTCONV SPG_PaintGetNextIndice(SPG_PAINT& P, BYTE* Regions, int Imax)
{//donne un indice de région libre et dont la valeur n'a pas encore été mise dans un point de masque
	//CHECK(P.CurrentIndice>=SPG_PAINT_AERAS,"SPG_PaintGetNextIndice",return 0);
	if(P.CurrentIndice==SPG_PAINT_AERAS-1)
	{//tous les indices sont utilisés: réaffecte les numeros de régions
		SPG_PaintFlushByte(P,Regions,Imax);
		CHECK(P.CurrentIndice==(SPG_PAINT_AERAS-1),"SPG_PaintGetNextIndice: Trop de régions",return P.CurrentIndice);
	}
	return P.CurrentIndice++;
}

int SPG_FASTCONV SPG_PaintSetEqv(SPG_PAINT& P, BYTE CP, BYTE OP)
{//relie un indice de région avec un indice déja existant
	if(P.EqvAeras[OP]) OP=P.EqvAeras[OP];//trouve les indices de base pour établir la correspondance
	if(P.EqvAeras[CP]) CP=P.EqvAeras[CP];
	if(CP==OP) return CP;//peint en couleur uniforme: pas de problème la correspondance a déjà été faite avant
	if(CP==0) return OP;//si on ne peignait pas au pixel précédent il n'y a pas de lien à faire, on prend le meme indice que le pixel d'au dessus

	if(OP>CP)
	{
		V_SWAP(BYTE,CP,OP);
	}

	//CHECK(CP<=OP,"SPG_PaintSetEqv",return 0);
	for(int i=OP+1;i<P.CurrentIndice;i++)
	{
		if((i==CP)||(P.EqvAeras[i]==CP))
		{
			P.EqvAeras[i]=OP;
		}
		CHECK(P.EqvAeras[i]>=i,"SPG_PaintSetEqv",return 0);
	}
	return OP;//s'il y a déjà une équivalence
}

int SPG_CONV SPG_PaintGetFirstEqv(SPG_PAINT& P)
{//retourne le premier indice de région
	return P.Eqv=1;
}

int SPG_CONV SPG_PaintGetNextEqv(SPG_PAINT& P)
{//retourne le prochain indice de région non connexe avec le précédent, on le reconnais car il n'a pas d'équivalence définie vers un indice inférieur
	for(;P.Eqv<P.CurrentIndice;P.Eqv++)
	{
		if(P.EqvAeras[P.Eqv]==0) return P.Eqv;
	}
	return 0;
}

int SPG_CONV SPG_PaintFlushEqvTable(SPG_PAINT& P)
{
	int EqvAeras=SPG_PaintGetFirstEqv(P);//trouve le premier indice de région
	int RegionNr=1;

	do
	{
#ifdef SPG_PAINT_DEBUG
		char Msg[128];
		sprintf(Msg,"Region %i",RegionNr);
		C_Add(P,Msg);
#endif
		for(int i=RegionNr;i<P.CurrentIndice;i++)
		{
			if((i==EqvAeras)||(P.EqvAeras[i]==EqvAeras))//si cet indice est connexe
			{
				P.EqvAeras[i]=RegionNr;//il devient ce numero de region
#ifdef SPG_PAINT_DEBUG
				sprintf(Msg,"Msk %i",i);
				C_Add(P,Msg);
#endif
			}
		}
		RegionNr++;
	} while(EqvAeras=SPG_PaintGetNextEqv(P));//affectation volontaire
#ifdef SPG_PAINT_DEBUG
	SPG_BlitAndWaitForClick();
#endif
	return P.CurrentIndice=RegionNr;//nombre de régions disjointes trouvées
}

void SPG_CONV SPG_PaintRestartEqvTable(SPG_PAINT& P)
{
	memset(P.EqvAeras,0,SPG_PAINT_AERAS);
}

#ifdef SPG_PAINT_DEBUG
void SPG_PaintDrawState(SPG_PAINT& Pnt, Profil& P)
{
	int XPos=G_SizeX(Global.Ecran)-P_SizeX(P);
	for(int y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
		}
	}
	//DoEvents(SPG_DOEV_ALL);
}
#endif

int SPG_CONV SPG_PaintProfil(Profil& P, float Threshold)
{
	CHECK(P_Msk(P)==0,"P_Paint",return 0)

	SPG_PAINT Pnt;
	SPG_PaintInit(Pnt);
#ifdef SPG_PAINT_DEBUG
	char Msg[128];
#endif

	for(int y=0;y<P_SizeY(P);y++)
	{
		int yPitch=y*P_SizeX(P);
		BYTE* MskLine=P_Msk(P)+yPitch;
		float* DataLine=P_Data(P)+yPitch;
		int Paint=0;
		//if((P_ElementMsk(P,0,y))||(P_Element(P,0,y)<Threshold))
		if(MskLine[0]||(DataLine[0]<Threshold))
		{
			if(y)
			{
				if(MskLine[-P_SizeX(P)])
				{
					Paint=SPG_PaintSetEqv(Pnt,Paint,MskLine[-P_SizeX(P)]);
				}
			}
			if(Paint==0)
			{
				Paint=SPG_PaintGetNextIndice(Pnt,P_Msk(P),yPitch);
			}
			MskLine[0]=Paint;
#ifdef SPG_PAINT_DEBUG
			sprintf(Msg,"\r\nWriting %i",Paint);
			C_Add(Pnt,Msg);
#endif
		}
		else if(MskLine[0]==0)
		{
			Paint=0;
#ifdef SPG_PAINT_DEBUG
			sprintf(Msg,"\r\nWriting %i",Paint);
			C_Add(Pnt,Msg);
#endif
		}

		for(int x=1;x<P_SizeX(P);x++)
		{
#ifdef SPG_PAINT_DEBUG
			SPG_PaintDrawState(Pnt,P);
#endif
			if(MskLine[x]||(DataLine[x]<Threshold))
			{
					if(y)
					{
						if(MskLine[x-P_SizeX(P)])
						{//propage la couleur vers le bas, etablit implicitement les correspondances
							Paint=SPG_PaintSetEqv(Pnt,Paint,MskLine[x-P_SizeX(P)]);
						}
					}
					if(Paint==0)
					{//cherche un nouvel indice
						Paint=SPG_PaintGetNextIndice(Pnt,P_Msk(P),yPitch+x);
					}
					MskLine[x]=Paint;
#ifdef SPG_PAINT_DEBUG
					sprintf(Msg,"Writing %i",Paint);
					C_AddF(Pnt,Msg);
#endif
			}
			else if(MskLine[x]==0)
			{
				Paint=0;
#ifdef SPG_PAINT_DEBUG
				sprintf(Msg,"Writing %i",Paint);
				C_AddF(Pnt,Msg);
#endif
			}
		}
	}
	SPG_PaintFlushByte(Pnt,P_Msk(P),P_Size(P));
	int T=Pnt.CurrentIndice;
	SPG_PaintClose(Pnt);
	return T;
}

#endif

