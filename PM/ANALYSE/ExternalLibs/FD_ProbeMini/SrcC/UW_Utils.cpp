
#include "SPG_General.h"

#ifdef SPG_General_USEUnwrap

#include "SPG_Includes.h"

//#include "UW_Utils.h"
#include "Algo\GoldsteinUnwrap.h"
#include "Algo\QualGuidedUnwrap.h"
#include <math.h>
#include <stdlib.h>
#include <memory.h>

/*
point de coupure pour l'exportation du code
*/


//params normaux: densite 1 bruit 0.6
//genere un profil de hauteur artificiel
void SPG_CONV UW_Generate(Profil& P,float Densite,float Bruit)
{
	for(int y=0;y<P_SizeY(P);y++)
	{
		float yf=((float)(y-(P_SizeY(P)>>1)))/(float)P_SizeY(P);
		for(int x=0;x<P_SizeX(P);x++)
		{
			float xf=((float)(x-(P_SizeX(P)>>1)))/(float)P_SizeX(P);

			//cas d'ecole deux residus de meme signe (pour affichage seulement)
			//float Re=cos(Densite*16*xf*xf);// *cos(Densite*16*yf*yf)*cos(Densite*64*(xf-yf)*(xf-yf))+Bruit*(float)rand()/RAND_MAX;
			//float Im=sin(Densite*16*xf*yf);// *sin(Densite*3*yf*yf)*sin(Densite*8*(xf+yf))+Bruit*(float)rand()/RAND_MAX;
			

			
			//cas d'ecole deux residus de signe oppose (pour affichage seulement)
			//float Re=cos(Densite*16*xf*xf);
			//float Im=sin(Densite*16*xf*xf*yf);
			

			
			//cas d'ecole qui marche: on retrouve la phase en selle de cheval xf*xf-yf*yf
			//float Re=cos(Densite*100*(xf*xf-yf*yf));
			//float Im=sin(Densite*100*(xf*xf-yf*yf));
			

			
			//cas mathematiquement indepliable
			float Re=cos(Densite*100*(xf*xf-yf*yf))*cos(Densite*10*(xf*xf+yf*yf))+0.01*sin(20*(xf+yf));
			float Im=sin(Densite*100*(xf*xf-yf*yf))*cos(Densite*10*(xf*xf+yf*yf))+0.01*cos(20*(xf+yf));
			

			
			//cas encore pire
			//float Re=cos(Densite*100*(xf*xf-yf*yf))*cos(Densite*20*sqrt(xf*xf+yf*yf))+0.05*sin(20*(xf*yf+yf));// *cos(Densite*16*yf*yf)*cos(Densite*64*(xf-yf)*(xf-yf))+Bruit*(float)rand()/RAND_MAX;
			//float Im=sin(Densite*100*(xf*xf-yf*yf))*cos(Densite*20*sqrt(xf*xf+yf*yf))+0.05*cos(20*(xf*yf+yf));// *sin(Densite*3*yf*yf)*sin(Densite*8*(xf+yf))+Bruit*(float)rand()/RAND_MAX;
			
			//calcul d'un argument entre 0 et 2pi
			float phase;

			if(Re>0) 
			{
				phase=V_PI/2-atan(Im/Re);
			}
			else if(Re<0)
			{
				phase=3*V_PI/2-atan(Im/Re);
			}
			else
			{
				if(Im<0) 
					phase=V_PI;
				else
					phase=0;

			}

				
			P_Element(P,x,y)=phase/(V_2PI)+Bruit*(float)rand()/RAND_MAX;
			V_Wrap(P_Element(P,x,y),1);
		}
	}
	return;
}

//simule le wrapping
void SPG_CONV UW_Wrap(Profil& P)
{
	for(int i=0;i<P_SizeX(P)*P_SizeY(P);i++)
	{
		P.D[i]=P.D[i]-floor(P.D[i]);
	}
	return;
}

//deplie le profil inplace
//la phase doit etre codee entre 0 et 1
void SPG_CONV UW_InPlaceUnwrap(Profil& P, UNWRAPPING_PARAMS& UWP)
{
	CHECK(P_Etat(P)==0,"UW_InPlaceUnwrap: Profil nul",return);
	CHECK(P_Data(P)==0,"UW_InPlaceUnwrap: Profil vide",return);
	CHECK(!V_IsBound(UWP.Algo,0,UW_NumAlgo),"UW_InPlaceUnwrap: Choix d'algorithme invalide\nL'algo zero sera utilisé",UWP.Algo=0);

	switch(UWP.Algo)
	{
	case 0:
		//l'algo simple est intrinsequement in place
		P_Unwrap_0_1(P);
		break;
	case 1:
		{
		Profil Pdest;
		//cree un profil destination temporaire

		P_DupliquateWithMsk(Pdest,P);

		GoldsteinUnwrap(P_Data(P),P_Data(Pdest),P_Msk(Pdest),P_SizeX(P),P_SizeY(P));

		//recopie le resultat a sa place
		P_Copy(P,Pdest);
		//desalloue la destination temporaire
		P_Close(Pdest);
		}
		break;
	case 2:
		{
		Profil Pdest;

		//cree un profil destination temporaire
		P_Dupliquate(Pdest,P);

		//cree un profil temporaire pour stocker la carte de qualite
		Profil Pqual;
		P_DupliquateWithMsk(Pqual,P);

		//deplie de P vers Pdest en utilisant Pqual comme tableau temporaire
		GradientQualGuidedUnwrap(P_Data(P),P_Data(Pdest),P_Data(Pqual),P_Msk(Pqual),P_SizeX(P),P_SizeY(P));

		P_Close(Pqual);

		//remet le resultat dans le tableau source
		P_Copy(P,Pdest);
		//desalloue les tableaux temporaires
		P_Close(Pdest);
		}
		break;
	case 3:
		{
		Profil Pdest;
		P_Dupliquate(Pdest,P);
		Profil Pqual;
		P_DupliquateWithMsk(Pqual,P);

		VarianceQualGuidedUnwrap(P_Data(P),P_Data(Pdest),P_Data(Pqual),P_Msk(Pqual),P_SizeX(P),P_SizeY(P));

		P_Close(Pqual);

		P_Copy(P,Pdest);
		P_Close(Pdest);
		}
		break;
	}

	return;
}

//le profil destination est créé par la procedure
//la phase doit etre codee entre 0 et 1
void SPG_CONV UW_Unwrap(Profil& PsrcWrap, Profil& PdstUnwrap, UNWRAPPING_PARAMS& UWP)
{
	CHECK(P_Etat(PsrcWrap)==0,"UW_Unwrap: Profil nul",return);
	CHECK(P_Data(PsrcWrap)==0,"UW_Unwrap: Profil vide",return);
	CHECK(!V_IsBound(UWP.Algo,0,UW_NumAlgo),"UW_Unwrap: Choix d'algorithme invalide\nL'algo zero sera utilisé",UWP.Algo=0);

	memset(&PdstUnwrap,0,sizeof(Profil));

	switch(UWP.Algo)
	{
	case 0:
		P_Dupliquate(PdstUnwrap,PsrcWrap);
		//l'algo simple est intrinsequement in place donc on copie tout
		//dans la destination et on unwrap
		P_Copy(PdstUnwrap,PsrcWrap);
		P_Unwrap_0_1(PdstUnwrap);
		break;
	case 1:
		{
		P_Dupliquate(PdstUnwrap,PsrcWrap);
	{
	float fMin,fMax;
	P_FindMinMax(PdstUnwrap,fMin,fMax);
	}
		//cree un tableau d'octets pour stocker le masque
		Profil8 Ptmp8;
		P8_Create(Ptmp8,
			P_SizeX(PsrcWrap),P_SizeY(PsrcWrap),
			P_XScale(PsrcWrap),P_YScale(PsrcWrap),
			P_UnitX(PsrcWrap),P_UnitY(PsrcWrap),P_UnitZ(PsrcWrap),0);
	{
	float fMin,fMax;
	P_FindMinMax(PdstUnwrap,fMin,fMax);
	}
		GoldsteinUnwrap(P_Data(PsrcWrap),P_Data(PdstUnwrap),P8_Data(Ptmp8),P_SizeX(PsrcWrap),P_SizeY(PsrcWrap));
	{
	float fMin,fMax;
	P_FindMinMax(PdstUnwrap,fMin,fMax);
	}
		P8_Close(Ptmp8);
		}
		break;
	case 2:
		{
		P_Dupliquate(PdstUnwrap,PsrcWrap);
		//on cree un tableau temporaire pour la carte de qualite
		Profil Pqual;
		P_DupliquateWithMsk(Pqual,PsrcWrap);
		GradientQualGuidedUnwrap(P_Data(PsrcWrap),P_Data(PdstUnwrap),P_Data(Pqual),P_Msk(Pqual),P_SizeX(PsrcWrap),P_SizeY(PsrcWrap));
		P_Close(Pqual);
		}
		break;
	case 3:
		{
		P_Dupliquate(PdstUnwrap,PsrcWrap);
		//on cree un tableau temporaire pour la carte de qualite
		Profil Pqual;
		P_DupliquateWithMsk(Pqual,PsrcWrap);
		VarianceQualGuidedUnwrap(P_Data(PsrcWrap),P_Data(PdstUnwrap),P_Data(Pqual),P_Msk(Pqual),P_SizeX(PsrcWrap),P_SizeY(PsrcWrap));
		P_Close(Pqual);
		}
		break;
	}
	return;
}

//le profil destination est créé par la procedure
//la phase doit etre codee entre 0 et 1
void SPG_CONV UW_UnwrapWithQualMap(Profil& PsrcWrap, Profil& PdstUnwrap, Profil& Pqual, UNWRAPPING_PARAMS& UWP)
{
	CHECK(P_Etat(PsrcWrap)==0,"UW_Unwrap: Profil nul",return);
	CHECK(P_Data(PsrcWrap)==0,"UW_Unwrap: Profil vide",return);
	CHECK(!V_IsBound(UWP.Algo,0,UW_NumAlgo),"UW_Unwrap: Choix d'algorithme invalide\nL'algo zero sera utilisé",UWP.Algo=0);

	memset(&PdstUnwrap,0,sizeof(Profil));
	memset(&Pqual,0,sizeof(Profil));

	switch(UWP.Algo)
	{
	case 0:
		P_Dupliquate(PdstUnwrap,PsrcWrap);
		//l'algo simple est intrinsequement in place donc on copie tout
		//dans la destination et on unwrap
		P_Copy(PdstUnwrap,PsrcWrap);
		P_Unwrap_0_1(PdstUnwrap);
		break;
	case 1:
		{
		P_Dupliquate(PdstUnwrap,PsrcWrap);
		//cree un tableau d'octets pour stocker le masque
		Profil8 Ptmp8;
		P8_Create(Ptmp8,
			P_SizeX(PsrcWrap),P_SizeY(PsrcWrap),
			P_XScale(PsrcWrap),P_YScale(PsrcWrap),
			P_UnitX(PsrcWrap),P_UnitY(PsrcWrap),P_UnitZ(PsrcWrap),0);
		GoldsteinUnwrap(P_Data(PsrcWrap),P_Data(PdstUnwrap),P_Data(Ptmp8),P_SizeX(PsrcWrap),P_SizeY(PsrcWrap));
		
		//transforme Ptmp8 en float en gardant les bons bits
		//car on voulait que goldstein genere ausi un masque de qualite
		//Les masques de qualite seront seuilles plus tard
		P_Dupliquate(Pqual,PsrcWrap);

		for(int i=0;i<P_SizeX(Ptmp8)*P_SizeY(Ptmp8);i++)
		{//0=barriere ou non unwrappe 1=residu 2=ok
//l'ordre des instructions est important car 
//il peut coexister plusiers bits simultanement
			Pqual.D[i]=(Ptmp8.D[i]&0x10)?0:2;//barriere donne 0 sinon 2=ok
			if((Ptmp8.D[i]&0x40)==0) Pqual.D[i]=0;//non unwrappe donne 0
			if(Ptmp8.D[i]&3) Pqual.D[i]=1;//residu donne 0
		}


		P8_Close(Ptmp8);
		}
		break;
	case 2:
		{
		P_Dupliquate(PdstUnwrap,PsrcWrap);
		//on cree le tableau de la carte de qualite
		P_DupliquateWithMsk(Pqual,PsrcWrap);
		GradientQualGuidedUnwrap(P_Data(PsrcWrap),P_Data(PdstUnwrap),P_Data(Pqual),P_Msk(Pqual),P_SizeX(PsrcWrap),P_SizeY(PsrcWrap));
		}
		break;
	case 3:
		{
		P_Dupliquate(PdstUnwrap,PsrcWrap);
		//on cree le tableau de la carte de qualite
		P_DupliquateWithMsk(Pqual,PsrcWrap);
		VarianceQualGuidedUnwrap(P_Data(PsrcWrap),P_Data(PdstUnwrap),P_Data(Pqual),P_Msk(Pqual),P_SizeX(PsrcWrap),P_SizeY(PsrcWrap));
		}
		break;
	}
	//les donnes de qualite sont dans float*Pqual.D
	//on enleve eventuellement le masque Pqual.Msk du profil Pqual
	//car il peut gener un affichage correct de la qualite
	//(Le masque est utilise par le fonctions P_xxx)
	if(Pqual.Msk) 
	{
		SPG_MemFree(Pqual.Msk);
		Pqual.Msk=0;
	}
	return;
}

#endif

