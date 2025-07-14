
#include "SPG_General.h"

#ifdef SPG_General_USEAlign

#include "SPG_Includes.h"

#include <memory.h>

//transforme l'image en un estimateur de rugosité réduit
void SPG_CONV SPG_AlignTransform(Profil& Ref, Profil& T, int Downsampling, int AeraSize, float Threshold, float MaxIntensityLevel)
{
	CHECK(P_Etat(Ref)==0,"SPG_AlignTransform",return);
	//CHECK(AeraSize<2,"SPG_AlignTransform",return);
	AeraSize=V_Max(AeraSize,3);
	//cree le profil transformé
	CHECK(P_Create(T,(P_SizeX(Ref)-AeraSize+1)/Downsampling,(P_SizeY(Ref)-AeraSize+1)/Downsampling)==0,"SPG_AlignTransform",return);
	for(int y=0;y<P_SizeY(T);y++)
	{
		for(int x=0;x<P_SizeX(T);x++)
		{//parcoure le profil résultat
			float SumXY=0;
			float SumDE=0;
			float SumG=0;
			{for(int b=0;b<AeraSize-4;b++)
			{//parcoure une petite région d'intégration sur le profil source
				int iy=y*Downsampling+b;//calcule la position ix,iy dans le profil source 
				//sachant que x et y se réferent au profil résultat
				for(int a=0;a<AeraSize-4;a++)
				{
					int ix=x*Downsampling+a;
/*
					//filtre passe-haut (rugosité) appliqué sur le résultat
					float L=(
						P_Element(Ref,ix,iy)         - P_Element(Ref,(ix+1),iy)     + P_Element(Ref,(ix+2),iy)
					  - P_Element(Ref,ix,(iy+1))									- P_Element(Ref,(ix+2),(iy+1))
					  + P_Element(Ref,ix,(iy+2))     - P_Element(Ref,(ix+1),(iy+2)) + P_Element(Ref,(ix+2),(iy+2))
					  );
					Sum+=fabs(L);
*/
					//filtre passe-haut (rugosité) appliqué sur le résultat
					float LXY=(
						P_Element(Ref,ix,iy)		- P_Element(Ref,(ix+4),iy)
					  - P_Element(Ref,ix,(iy+4))	+ P_Element(Ref,(ix+4),(iy+4))
					  );
					float LDE=(
												P_Element(Ref,(ix+2),iy)
					  - P_Element(Ref,ix,(iy+2))					- P_Element(Ref,(ix+4),(iy+2))
												+ P_Element(Ref,(ix+2),(iy+4))
					  );
					SumXY+=fabs(LXY);
					SumDE+=fabs(LDE);
					//Offset+=(MaxIntensityLevel/8+P_Element(Ref,ix,iy));//calcule le niveau moyen pour que les régions sombres aient plus de poids
					//le maxintensitylevel/8 évite que les parties noires (tour du diaphragme) ne prennent un poids trop important
				}
			}}
			{for(int b=0;b<AeraSize-1;b++)
			{//parcoure une petite région d'intégration sur le profil source
				int iy=y*Downsampling+b;//calcule la position ix,iy dans le profil source 
				//sachant que x et y se réferent au profil résultat
				for(int a=0;a<AeraSize-1;a++)
				{
					int ix=x*Downsampling+a;
/*
					//filtre passe-haut (rugosité) appliqué sur le résultat
					float L=(
						P_Element(Ref,ix,iy)         - P_Element(Ref,(ix+1),iy)     + P_Element(Ref,(ix+2),iy)
					  - P_Element(Ref,ix,(iy+1))									- P_Element(Ref,(ix+2),(iy+1))
					  + P_Element(Ref,ix,(iy+2))     - P_Element(Ref,(ix+1),(iy+2)) + P_Element(Ref,(ix+2),(iy+2))
					  );
					Sum+=fabs(L);
*/
					//filtre passe-haut (rugosité) appliqué sur le résultat
					float LG=(
						fabs(P_Element(Ref,ix,iy)- P_Element(Ref,(ix+1),iy))+
						fabs(P_Element(Ref,ix,iy)- P_Element(Ref,ix,(iy+1)))
					  );
					SumG+=LG;
					//Offset+=(MaxIntensityLevel/8+P_Element(Ref,ix,iy));//calcule le niveau moyen pour que les régions sombres aient plus de poids
					//le maxintensitylevel/8 évite que les parties noires (tour du diaphragme) ne prennent un poids trop important
				}
			}}
			P_Element(T,x,y)=(2*SumXY+SumDE)/SumG;///Offset-Threshold;//ecrit le résultat
		}
	}
	//P_FastConvLowPass(T,1);//filtre le résultat une fois par une matrice 3x3
	//P_FastConvLowPass(T,1);
	return;
}

//Reference=image de reference
//BumpSize=taille caractéristique en pixels des motifs cherchés (ordre de grandeur +-50%)
//MaxIntensityLevel: intensité max des images (256 pour une image 8bits, 1024 pour une image 10bits, etc)
//Threshold Seuil de rugosité, Typiquement 0.05
int SPG_CONV SPG_AlignInit(SPG_Align& SA, Profil& Reference, int BumpSize, float MaxIntensityLevel, float Threshold)
{
	memset(&SA,0,sizeof(SPG_Align));
	P_Duplicate(SA.Reference,Reference);
	SA.BumpSize=BumpSize;
	int D=(4*P_SizeY(Reference))/SA.BumpSize;

	SA.Threshold=Threshold;
	SA.MaxIntensityLevel=MaxIntensityLevel;

	//SA.Downsampling=V_Max(D,1);

	SA.Downsampling=1;//V_Max(SA.BumpSize/4,1);
	SA.AeraSize=SA.Downsampling+3;//+(SA.Downsampling>>1);//V_Max(SA.BumpSize/4,3);

	SA.AeraSize=SA.Downsampling+8;//+(SA.Downsampling>>1);//V_Max(SA.BumpSize/4,3);
	//transforme la référence
	SPG_AlignTransform(SA.Reference,SA.RTransform,SA.Downsampling,SA.AeraSize,SA.Threshold,SA.MaxIntensityLevel);

	return SA.Etat=-1;
}

void SPG_CONV SPG_AlignClose(SPG_Align& SA)
{
	P_Close(SA.Reference);
	P_Close(SA.RTransform);
	memset(&SA,0,sizeof(SPG_Align));
	return;
}

void SPG_CONV SPG_AlignEcart(SPG_Align& SA, Profil& PTransform, float& Diff, float& Weight, int XTest, int YTest)
{

	//le profil de test PTransform est placé aux coordonnées XTest,Ytest dans le profil de référence SA.RTRansform
	//on doit déterminer les coordonnées du rectangle d'intersection dans le profil de référence
	//pour calculer la différence des valeurs sur les parties chevauchantes de SA.RTransform et PTransform

	//positions dans le profil reference
	int XStart=V_Max(XTest,0);
	int YStart=V_Max(YTest,0);
	int XStop=V_Min((P_SizeX(PTransform)+XTest),P_SizeX(SA.RTransform));
	int YStop=V_Min((P_SizeY(PTransform)+YTest),P_SizeY(SA.RTransform));
	int XLen=XStop-XStart;
	int YLen=YStop-YStart;

	//positions dans le profil testé
	int XT=V_Max((-XTest),0);
	int YT=V_Max((-YTest),0);

	//vérifie que les boucles ne vont pas sortir des surfaces
	CHECK((XLen<=0)||(YLen<=0),"SPG_AlignEcart",return);
	CHECK(XStart+XLen>P_SizeX(SA.RTransform),"SPG_AlignEcart",return);
	CHECK(YStart+YLen>P_SizeY(SA.RTransform),"SPG_AlignEcart",return);
	CHECK(XT+XLen>P_SizeX(PTransform),"SPG_AlignEcart",return);
	CHECK(YT+YLen>P_SizeY(PTransform),"SPG_AlignEcart",return);

	Diff=0;
	Weight=0;

	int Count=XLen*YLen;

	for(int yr=YStart;yr<YStop;yr++)
	{//boucle dans la référence
		int yp=yr-YStart+YT;//position dans le profil de test

		float* RefLine=P_Data(SA.RTransform)+yr*P_SizeX(SA.RTransform);
		float* TstLine=P_Data(PTransform)+yp*P_SizeX(PTransform)-XStart+XT;

		for(int xr=XStart;xr<XStop;xr++)
		{
			//CHECKPOINTER(RefLine+xr,"SPG_AlignEcart",;);
			//CHECKPOINTER(TstLine+xr,"SPG_AlignEcart",;);

			float AlphaR=RefLine[xr];
			float AlphaP=TstLine[xr];
			float Delta=AlphaR-AlphaP;
			Diff+=Delta*Delta;
			Weight+=AlphaR*AlphaR+AlphaP*AlphaP;
		}
	}
	Diff/=Weight;
	return;
}

void SPG_CONV SPG_AlignGetPos(SPG_Align& SA, Profil& P, int& PosX, int& PosY, int SearchAera)
{
	CHECK(P_Etat(P)==0,"SPG_AlignGetPos",return);

	Profil PTransform;
	SPG_AlignTransform(P,PTransform,SA.Downsampling,SA.AeraSize,SA.Threshold,SA.MaxIntensityLevel);

/*
	int ExclusionBorderX=V_Min(P_SizeX(PTransform),P_SizeX(SA.RTransform))/4;
	int ExclusionBorderY=V_Min(P_SizeY(PTransform),P_SizeY(SA.RTransform))/4;
	
	int XMin=-P_SizeX(PTransform)+ExclusionBorderX;
	int XMax=P_SizeX(SA.RTransform)-ExclusionBorderX;
	int YMin=-P_SizeY(PTransform)+ExclusionBorderY;
	int YMax=P_SizeY(SA.RTransform)-ExclusionBorderY;

	XMin=V_Max(XMin,(PosX-SearchAera)/SA.Downsampling);
	XMax=V_Min(XMax,(PosX+SearchAera)/SA.Downsampling);
	YMin=V_Max(YMin,(PosY-SearchAera)/SA.Downsampling);
	YMax=V_Min(YMax,(PosY+SearchAera)/SA.Downsampling);

	Profil PDiff;
	Profil PWeight;

	P_Create(PDiff,XMax-XMin,YMax-YMin);
	P_Create(PWeight,XMax-XMin,YMax-YMin);

	for(int YTest=YMin;YTest<YMax;YTest++)
	{
		for(int XTest=XMin;XTest<XMax;XTest++)
		{
			float Diff=0;
			float Weight=0;
			SPG_AlignEcart(SA,PTransform,Diff,Weight,XTest,YTest);
			P_Element(PDiff,(XTest-XMin),(YTest-YMin))=Diff;
			P_Element(PWeight,(XTest-XMin),(YTest-YMin))=Weight;
		}
	}

	int DownSPosX,DownSPosY;
	if(P_FindMin(PDiff,DownSPosX,DownSPosY))
	{
		SA.Weight=P_Element(PWeight,DownSPosX,DownSPosY);
		PosX=SA.PosX=(DownSPosX+XMin)*SA.Downsampling;
		PosY=SA.PosY=(DownSPosY+YMin)*SA.Downsampling;
	}

	P_SaveToFile(PDiff,"PDiff.pgp");
	P_SaveToFile(PWeight,"PWeight.pgp");
	P_Close(PDiff);
	P_Close(PWeight);
*/
	P_SaveToFile(SA.RTransform,"Ref.pgp");
	P_SaveToFile(PTransform,"P.pgp");


	P_Close(PTransform);
	return;
}

#endif
