
#include "SPG_General.h"

#ifdef SPG_General_USESGRAPH

#include "SPG_Includes.h"

#ifdef DebugFloat
#include <float.h>
#endif
#include <string.h>
//concat and dispatch realloue les blocs
#define CCAT_ReFitBlock

//concat and dispatch alloue un seul blocs
#define CCAT_SingleBlock

#ifdef DebugSGRAPH
void SPG_CONV SG_CheckBloc(SG_FullBloc &Bloc)
{
	CHECK((Bloc.Etat&SG_OK)==0,"Bloc avec flag OK nul",return);

#define Bloc Bloc.DB

	int NP=Bloc.NombreP;
	int NF=Bloc.NombreF;
	
	CHECK(Bloc.MemPoints==0,"Bloc vide",return);
	CHECK(Bloc.MemFaces==0,"Bloc vide",return);
	int n;
	for(n=0;n<NP;n++)
	{
		V_VECT V=Bloc.MemPoints[n].P; //'V' : unreferenced local variable: Volontaire
	}
	for(n=0;n<NF;n++)
	{
		SG_FACE F=Bloc.MemFaces[n]; //'F' : unreferenced local variable: Volontaire
		CHECK(
			!V_IsBound(
			Bloc.MemFaces[n].NumP1,
			Bloc.MemPoints,(Bloc.MemPoints+Bloc.NombreP)
			),"Pointeur de points incorrect",return);
		CHECK(
			!V_IsBound(
			Bloc.MemFaces[n].NumP2,
			Bloc.MemPoints,(Bloc.MemPoints+Bloc.NombreP)
			),"Pointeur de points incorrect",return);
		CHECK(
			!V_IsBound(
			Bloc.MemFaces[n].NumP3,
			Bloc.MemPoints,(Bloc.MemPoints+Bloc.NombreP)
			),"Pointeur de points incorrect",return);
		CHECK(
			!V_IsBound(
			Bloc.MemFaces[n].NumP4,
			Bloc.MemPoints,(Bloc.MemPoints+Bloc.NombreP)
			),"Pointeur de points incorrect",return);
	}
	return;

#undef Bloc

}

void SPG_CONV SG_CheckDispatchDescr(SG_DispatchDescr &DispDescr)
{
	for(int i=0;i<SG_DispatchNombreB(DispDescr);i++)
	{
		if(SG_DispatchIndex(DispDescr,i).Etat) SG_CheckBloc(SG_DispatchIndex(DispDescr,i));
	}
	return;
}

#endif


//ajuste BRef et rayon pour SGE_BlocVisible
void SPG_CONV SG_MakeBRef(SG_FullBloc &Bloc)
{
#ifdef DebugSGRAPH
	//SG_CheckBloc(Bloc);
#endif

	int i;

	CHECK(Bloc.DB.MemPoints==0,"SG_MakeBRef: Bloc vide",return);

	if (Bloc.DB.NombreP==0)
	{
		V_Operate1(Bloc.BRef,=0);
		Bloc.Rayon=0;
		return;
	}

	{
	V_VECT PMin;
	V_VECT PMax;
	PMax=PMin=(V_VECT)Bloc.DB.MemPoints[0].P;


	for(i=0; i<Bloc.DB.NombreP; i++)
	{
		PMin.x=V_Min(PMin.x,Bloc.DB.MemPoints[i].P.x);
		PMin.y=V_Min(PMin.y,Bloc.DB.MemPoints[i].P.y);
		PMin.z=V_Min(PMin.z,Bloc.DB.MemPoints[i].P.z);

		PMax.x=V_Max(PMax.x,Bloc.DB.MemPoints[i].P.x);
		PMax.y=V_Max(PMax.y,Bloc.DB.MemPoints[i].P.y);
		PMax.z=V_Max(PMax.z,Bloc.DB.MemPoints[i].P.z);
	}

	V_Operate3(Bloc.BRef,=PMin,+PMax);
	}

	V_Operate1(Bloc.BRef,*=0.5);

	float Rayon=0;
	for(i=0; i<Bloc.DB.NombreP; i++)
	{
		V_VECT VectDiff;
		V_Operate3(VectDiff,=Bloc.DB.MemPoints[i].P,-Bloc.BRef);
		float NvRayon;
		V_Mod2Vect(VectDiff,NvRayon);
		if(NvRayon>Rayon) Rayon=NvRayon;
	}

	Bloc.Rayon=sqrt(Rayon);

	return;
}

//cree un bloc
int SPG_CONV SG_CreateBloc(SG_FullBloc &Bloc,int NumP,int NumF,int MODE)
{

	memset(&Bloc,0,LenFullBloc);
#ifdef SPG_General_PGLib
	//Bloc.Bloc=pglCreateLinkedBloc(PGL_QUADS);
#endif

	CHECK((NumP==0)&&(NumF==0),"SG_CreateBloc: Bloc vide",return 0);

	Bloc.Etat|=SG_OK;

	if(NumP)
	{
	Bloc.DB.NombreP = NumP;
	if (MODE&SG_WithPOINTS)
	{
	//zeroinit a cause de l'effet memoire de XECR=-32766 a -32768
	CHECK((Bloc.DB.MemPoints = SPG_TypeAlloc(NumP,SG_PNT3D,"Points"))==0,"SG_CreateBloc: Erreur allocation points",SG_CloseBloc(Bloc);return 0);
	Bloc.Etat|=SG_WithPOINTS;
	}
	}

	if(NumF)
	{
	Bloc.DB.NombreF=NumF;
	if (MODE&SG_WithFACES)
	{
	//zeroinit a cause de l'effet memoire de style je sais pu pourquoi (tri)
	CHECK((Bloc.DB.MemFaces = SPG_TypeAlloc(NumF,SG_FACE,"Faces"))==0,"SG_CreateBloc: Erreur allocation faces",SG_CloseBloc(Bloc);return 0);
	Bloc.Etat|=SG_WithFACES;
	}
	}
	/*
#ifdef SGE_EMC
	Bloc.LightInvertTransform.axex.x=1;
	Bloc.LightInvertTransform.axey.y=1;
	Bloc.LightInvertTransform.axez.z=1;
#endif
	*/
	return -1;
}

int SPG_CONV SG_InitBloc(SG_FullBloc &Bloc,SG_PNT3D *Pnt,int NumP,
				SG_FACE *Fce,int NumF,int MODE)
{
	//si on passe le flag SG_OK on considere que le bloc est deja initialise
	//pour faire une duplication sur un alias
	if ((MODE&SG_OK)==0) CHECK(SG_CreateBloc(Bloc,NumP,NumF,MODE)==0,"SG_InitBloc: SG_CreateBloc echoue",return 0);

	if ((MODE&SG_WithPOINTS)||(MODE&SG_OK))
	{
		if (Pnt) memcpy(Bloc.DB.MemPoints,Pnt,sizeof(SG_PNT3D)*NumP);
	}
	else
		Bloc.DB.MemPoints = Pnt;
	
	if ((MODE&SG_WithFACES)||(MODE&SG_OK))
	{
		if (Fce) memcpy(Bloc.DB.MemFaces,Fce,sizeof(SG_FACE)*NumF);
	}
	else
		Bloc.DB.MemFaces = Fce;
	
	if (((MODE&SG_WithFACES)&&(MODE&SG_WithPOINTS))||(MODE&SG_OK))
	{
		int i;
		for(i=0; i<Bloc.DB.NombreF; i++)
		{
			*(int*)(&Bloc.DB.MemFaces[i].NumP1) += (int)Bloc.DB.MemPoints-(int)Pnt;
			*(int*)(&Bloc.DB.MemFaces[i].NumP2) += (int)Bloc.DB.MemPoints-(int)Pnt;
			*(int*)(&Bloc.DB.MemFaces[i].NumP3) += (int)Bloc.DB.MemPoints-(int)Pnt;
			*(int*)(&Bloc.DB.MemFaces[i].NumP4) += (int)Bloc.DB.MemPoints-(int)Pnt;
		}
	}

	SG_MakeBRef(Bloc);

	return -1;
}

void SPG_CONV SG_CloseBloc(SG_FullBloc &Bloc)
{
#ifdef DebugSGRAPH
	CHECK((Bloc.Etat&SG_OK)==0,"SG_CloseBloc: Close de bloc vide",return);
#endif
	CHECK(Bloc.Etat&~(SG_WithPOINTS|SG_WithFACES|SG_WithTEXTURE|SG_OK),"SG_CloseBloc: Close de n'importe quoi",return);
	//CHECK(Bloc.Etat==0,"SG_CloseBloc: Close de bloc invalide",return);

	SG_PGLCloseDispatch(Bloc);

	Bloc.DB.NombreP=0;
	if (Bloc.Etat&SG_WithPOINTS)
	{
#ifndef DebugSGRAPH
		if(Bloc.DB.MemPoints)
#endif
			SPG_MemFree(Bloc.DB.MemPoints);
	}
	Bloc.DB.MemPoints=0;
	Bloc.DB.NombreF=0;
	
	if (Bloc.Etat&SG_WithFACES)
	{
#ifndef DebugSGRAPH
		if(Bloc.DB.MemFaces)
#endif
			SPG_MemFree(Bloc.DB.MemFaces);
	}
	
	Bloc.DB.MemFaces=0;
	Bloc.Etat=0;
	return;
}

/*
int SPG_CONV SG_DupliqueBloc(SG_FullBloc &BlocRef,SG_FullBloc &Bloc,int MODE)
{
	CHECK(SG_InitBloc(Bloc,BlocRef.DB.MemPoints,BlocRef.DB.NombreP,BlocRef.DB.MemFaces,BlocRef.DB.NombreF,MODE)==0,"SG_DupliqueBoc: SG_InitBloc echoue",return 0);
	return -1;
}
*/
//fait un seul bloc avec plusieurs
int SPG_CONV SG_ConcatBlocList(SG_FullBloc &BlocResult,SG_FullBloc **BlocToAdd,int NombreB)
{
	memset(&BlocResult,0,sizeof(SG_FullBloc));
	
	int TotalP = 0;
	int TotalF = 0;
	int i;
	for(i=0; i<NombreB; i++)
	{
		DbgCHECK((BlocToAdd[i]->DB.MemPoints==0)||(BlocToAdd[i]->DB.MemFaces==0),"SG_ConcatBlocList: Bloc invalide");
		if ((BlocToAdd[i]->DB.MemPoints)&&(BlocToAdd[i]->DB.MemFaces))
		{
			TotalP += BlocToAdd[i]->DB.NombreP;
			TotalF += BlocToAdd[i]->DB.NombreF;
		}
	}
	CHECK(SG_CreateBloc(BlocResult,TotalP,TotalF,SG_WithPOINTS|SG_WithFACES)==0,"SG_ConcatBlocList: SG_CreateBloc echoue",return 0);
	int PartP = 0;
	int PartF = 0;
	for(i=0; i<NombreB; i++)
	{
		if ((BlocToAdd[i]->DB.MemPoints)&&(BlocToAdd[i]->DB.MemFaces))
		{
			memcpy(BlocResult.DB.MemPoints+PartP,BlocToAdd[i]->DB.MemPoints,sizeof(SG_PNT3D)*BlocToAdd[i]->DB.NombreP);
			memcpy(BlocResult.DB.MemFaces+PartF,BlocToAdd[i]->DB.MemFaces,sizeof(SG_FACE)*BlocToAdd[i]->DB.NombreF);
			
			int j;
			for(j=0; j<BlocToAdd[i]->DB.NombreF; j++)
			{
				*(int*)(&BlocResult.DB.MemFaces[j+PartF].NumP1)+=(int)(BlocResult.DB.MemPoints+PartP)-(int)BlocToAdd[i]->DB.MemPoints;
				*(int*)(&BlocResult.DB.MemFaces[j+PartF].NumP2)+=(int)(BlocResult.DB.MemPoints+PartP)-(int)BlocToAdd[i]->DB.MemPoints;
				*(int*)(&BlocResult.DB.MemFaces[j+PartF].NumP3)+=(int)(BlocResult.DB.MemPoints+PartP)-(int)BlocToAdd[i]->DB.MemPoints;
				*(int*)(&BlocResult.DB.MemFaces[j+PartF].NumP4)+=(int)(BlocResult.DB.MemPoints+PartP)-(int)BlocToAdd[i]->DB.MemPoints;
			}
			PartP+=BlocToAdd[i]->DB.NombreP;
			PartF+=BlocToAdd[i]->DB.NombreF;
		}
	}
	SG_MakeBRef(BlocResult);
	return -1;
}

int SPG_CONV SG_ConcatBloc2(SG_FullBloc &BlocResult,
				   SG_FullBloc &BlocToAdd1,
				   SG_FullBloc &BlocToAdd2)
{
	SG_FullBloc *BlocList[2];
	BlocList[0] = &BlocToAdd1;
	BlocList[1] = &BlocToAdd2;
	return SG_ConcatBlocList(BlocResult,BlocList,2);
}

int SPG_CONV SG_ConcatBloc3(SG_FullBloc &BlocResult,
				   SG_FullBloc &BlocToAdd1,
				   SG_FullBloc &BlocToAdd2,
				   SG_FullBloc &BlocToAdd3)
{
	SG_FullBloc* BlocList[3];
	BlocList[0] = &BlocToAdd1;
	BlocList[1] = &BlocToAdd2;
	BlocList[2] = &BlocToAdd3;
	return SG_ConcatBlocList(BlocResult,BlocList,3);
}

int SPG_CONV SG_ConcatBloc6(SG_FullBloc &BlocResult,
				   SG_FullBloc &BlocToAdd1,
				   SG_FullBloc &BlocToAdd2,
				   SG_FullBloc &BlocToAdd3,
				   SG_FullBloc &BlocToAdd4,
				   SG_FullBloc &BlocToAdd5,
				   SG_FullBloc &BlocToAdd6)
{
	SG_FullBloc* BlocList[6];
	BlocList[0] = &BlocToAdd1;
	BlocList[1] = &BlocToAdd2;
	BlocList[2] = &BlocToAdd3;
	BlocList[3] = &BlocToAdd4;
	BlocList[4] = &BlocToAdd5;
	BlocList[5] = &BlocToAdd6;
	return SG_ConcatBlocList(BlocResult,BlocList,6);
}

//repartit geographiquement dans NBlockX*NBlockY blocs les points et faces d'une liste de blocs
//enleve les points non references, duplique les points qui doivent aller dans plusieurs blocs
//a la fois
int SPG_CONV SG_ConcatAndDispatchBlocList(SG_DispatchDescr& BlocDescr,int NBlocX, int NBlocY, float PntIdTol, SG_FullBloc** BlocToAdd,int NombreB)
{
	memset(&BlocDescr,0,sizeof(SG_DispatchDescr));
	
	CHECK((BlocDescr.Bloc=SPG_TypeAlloc(NBlocX*NBlocY,SG_FullBloc,"Liste de DispatchBlocs"))==0,"SG_ConcatAndDispatchBlocList: Allocation echouee",return 0);
	BlocDescr.BlocX=NBlocX;
	BlocDescr.BlocY=NBlocY;

	int* TotalP=SPG_TypeAlloc(NBlocX*NBlocY,int,"Distribution des points");
	int* PartP=SPG_TypeAlloc(NBlocX*NBlocY,int,"Distribution des points");
	int* TotalF=SPG_TypeAlloc(NBlocX*NBlocY,int,"Distribution des faces");
	int* PartF=SPG_TypeAlloc(NBlocX*NBlocY,int,"Distribution des faces");
	
	int i;
	for(i=0; i<NombreB; i++)
	{
		DbgCHECK(((BlocToAdd[i])&&(BlocToAdd[i]->DB.MemPoints)&&(BlocToAdd[i]->DB.MemFaces)&&(BlocToAdd[i]->Etat>0))==0,"SG_ConcatAndDispatchBlocList: Bloc invalide");//,return 0);
		
		if ((BlocToAdd[i])&&(BlocToAdd[i]->DB.MemPoints)&&(BlocToAdd[i]->DB.MemFaces)&&(BlocToAdd[i]->Etat>0))
		{
			int f;
			for (f=0;f<BlocToAdd[i]->DB.NombreF;f++)
			{
				SG_PNT3D* j=BlocToAdd[i]->DB.MemFaces[f].NumP1;
				{
					BlocDescr.XMin=V_Min(BlocDescr.XMin,j->P.x);
					BlocDescr.XMax=V_Max(BlocDescr.XMax,j->P.x);
					BlocDescr.YMin=V_Min(BlocDescr.YMin,j->P.y);
					BlocDescr.YMax=V_Max(BlocDescr.YMax,j->P.y);
				}
			}
		}
	}
	
	{
		int posby;
		for(posby=0;posby<NBlocY;posby++)
		{
			int posbx;
			for(posbx=0;posbx<NBlocX;posbx++)
			{
				TotalP[posbx+NBlocX*posby]=PartP[posbx+NBlocX*posby]=TotalF[posbx+NBlocX*posby]=PartF[posbx+NBlocX*posby]=0;
			}
		}
	}
	
	BlocDescr.InvMaxMinX=BlocDescr.BlocX/(BlocDescr.XMax-BlocDescr.XMin);
	BlocDescr.InvMaxMinY=BlocDescr.BlocY/(BlocDescr.YMax-BlocDescr.YMin);
	

	int TotalPointsBloc=0;
	int TotalFacesBloc=0;

	for(i=0; i<NombreB; i++)
	{
		if ((BlocToAdd[i])&&(BlocToAdd[i]->DB.MemPoints)&&(BlocToAdd[i]->DB.MemFaces)&&(BlocToAdd[i]->Etat>0))
		{
			int f;
			for (f=0;f<BlocToAdd[i]->DB.NombreF;f++)
			{
				SG_PNT3D* j=BlocToAdd[i]->DB.MemFaces[f].NumP1;
				{
					int n=SG_DispatchXYS(BlocDescr,j->P.x,j->P.y);
					TotalP[n]+=4;
					TotalPointsBloc+=4;
					TotalF[n]++;
					TotalFacesBloc++;
				}
			}
		}
	}
	

#ifdef CCAT_SingleBlock

	SG_CreateBloc(BlocDescr.Bloc[0],TotalPointsBloc,TotalFacesBloc,SG_WithPOINTS|SG_WithFACES);

	SG_PNT3D* BDescrPoints=BlocDescr.Bloc[0].DB.MemPoints;
	SG_FACE* BDescrFaces=BlocDescr.Bloc[0].DB.MemFaces;

	for(i=1; i<NBlocX*NBlocY; i++)
	{
		BDescrPoints+=TotalP[i-1];
		BDescrFaces+=TotalF[i-1];
		//if (TotalF[i]) SG_InitBloc(BlocDescr.Bloc[i],BDescrPoints,TotalP[i],BDescrFaces,TotalF[i],SG_OK);
		//ne copie rien, n'alloue pas le bloc, met juste les pointeurs
		/*
		BlocDescr.Bloc[i].DB.NombreP = 0;//BDescrPoints;
		BlocDescr.Bloc[i].DB.NombreF = 0;//BDescrFaces;
		*/
		BlocDescr.Bloc[i].DB.MemPoints = BDescrPoints;
		BlocDescr.Bloc[i].DB.MemFaces = BDescrFaces;
		BlocDescr.Bloc[i].Etat=SG_OK;
	}

#else
	
	for(i=0; i<NBlocX*NBlocY; i++)
	{
		if (TotalF[i]) SG_CreateBloc(BlocDescr.Bloc[i],TotalP[i],TotalF[i],SG_WithPOINTS|SG_WithFACES);
		//alloue le bloc
	}
	
#endif

	float EqDist=PntIdTol*(fabs(BlocDescr.XMax)+fabs(BlocDescr.XMin)+fabs(BlocDescr.YMax)+fabs(BlocDescr.YMin));
	EqDist*=EqDist;
	
#define BlocResult BlocDescr.Bloc
	for(i=0; i<NombreB; i++)
	{
		if ((BlocToAdd[i])&&(BlocToAdd[i]->DB.MemPoints)&&(BlocToAdd[i]->DB.MemFaces)&&(BlocToAdd[i]->Etat>0))
		{
			int f;
			for (f=0;f<BlocToAdd[i]->DB.NombreF;f++)
			{
				SG_PNT3D* j=BlocToAdd[i]->DB.MemFaces[f].NumP1;
				{//le bloc de destination est defini par le premier point
					int BDest=SG_DispatchXYS(BlocDescr,j->P.x,j->P.y);
					
					SG_PNT3D* P[4];
					
					P[0]=BlocToAdd[i]->DB.MemFaces[f].NumP1;
					P[1]=BlocToAdd[i]->DB.MemFaces[f].NumP2;
					P[2]=BlocToAdd[i]->DB.MemFaces[f].NumP3;
					P[3]=BlocToAdd[i]->DB.MemFaces[f].NumP4;
					
					int p;
					for(p=0;p<4;p++)
					{
						int pr;//recherche des points egaux
						for(pr=PartP[BDest]-1;pr>=0;pr--)
						{
							V_VECT D;
							V_Operate3(D,=P[p]->P,-BlocResult[BDest].DB.MemPoints[pr].P);
							float DistEq;
							V_Mod2Vect(D,DistEq);
							if (DistEq<=EqDist)
							{
								P[p]=BlocResult[BDest].DB.MemPoints+pr;
								break;
							}
						}
						
						if (pr==-1)//PartP[BDest])
						{//un nouveau point doit s'ajouter
							BlocResult[BDest].DB.MemPoints[PartP[BDest]]=*P[p];
							P[p]=BlocResult[BDest].DB.MemPoints+PartP[BDest];
							PartP[BDest]++;
						}
					}
					
					DbgCHECK(PartF[BDest]>=TotalF[BDest],"SG_ConcatAndDispatchBlocList: Erreur grave");
					
					//si 3 des 4 points sont egaux ne pas ajouter la face
					BlocResult[BDest].DB.MemFaces[PartF[BDest]]=BlocToAdd[i]->DB.MemFaces[f];
					BlocResult[BDest].DB.MemFaces[PartF[BDest]].NumP1=P[0];
					BlocResult[BDest].DB.MemFaces[PartF[BDest]].NumP2=P[1];
					BlocResult[BDest].DB.MemFaces[PartF[BDest]].NumP3=P[2];
					BlocResult[BDest].DB.MemFaces[PartF[BDest]].NumP4=P[3];
					PartF[BDest]++;
					//DbgCHECK(PartF[BDest]>TotalF[BDest],"SG_ConcatAndDispatchBlocList: Depassement");
/*					
					if (PartF[BDest]==TotalF[BDest])
					{
						BlocResult[BDest].DB.NombreP=PartP[BDest];
						BlocResult[BDest].DB.NombreF=PartF[BDest];
						SG_MakeBRef(BlocResult[BDest]);
#ifdef ReFitBlock
						if(PartP[BDest]<TotalP[BDest])
						{
						SG_FullBloc BTmp;//=BlocResult[i];
						SG_DupliqueBloc(BlocResult[BDest],BTmp,SG_WithPOINTS);
						SPG_MemFree(BlocResult[BDest].DB.MemPoints);
						
						int j;
						for(j=0; j<BlocResult[BDest].DB.NombreF; j++)
						{
							*(int*)(&BlocResult[BDest].DB.MemFaces[j].NumP1)+=(int)BTmp.DB.MemPoints-(int)BlocResult[BDest].DB.MemPoints;
							*(int*)(&BlocResult[BDest].DB.MemFaces[j].NumP2)+=(int)BTmp.DB.MemPoints-(int)BlocResult[BDest].DB.MemPoints;
							*(int*)(&BlocResult[BDest].DB.MemFaces[j].NumP3)+=(int)BTmp.DB.MemPoints-(int)BlocResult[BDest].DB.MemPoints;
							*(int*)(&BlocResult[BDest].DB.MemFaces[j].NumP4)+=(int)BTmp.DB.MemPoints-(int)BlocResult[BDest].DB.MemPoints;
						}
						BlocResult[BDest].DB.MemPoints=BTmp.DB.MemPoints;
						}
#endif
					}
*/
				}
			}
		}
	}
	


#ifdef CCAT_SingleBlock
#define TotalP _TotalP_NE_DOIT_PLUS_ETRE_UTILISE
#define TotalF _TotalF_NE_DOIT_PLUS_ETRE_UTILISE

	int TotalPointsBlocFinal=0;
	int TotalFacesBlocFinal=0;
	for(i=0; i<NBlocX*NBlocY; i++)
	{
		BlocResult[i].DB.NombreP=PartP[i];
		BlocResult[i].DB.NombreF=PartF[i];
		SG_MakeBRef(BlocResult[i]);
		TotalPointsBlocFinal+=PartP[i];
		TotalFacesBlocFinal+=PartF[i];
		//SG_CheckBloc(BlocResult[i]);
	}
#ifdef CCAT_ReFitBlock
	SG_FullBloc BTmp;
	SG_FullBloc BlocToClose=BlocDescr.Bloc[0];

	if(TotalPointsBlocFinal<TotalPointsBloc)
	{
	SG_CreateBloc(BTmp,TotalPointsBlocFinal,0,SG_WithPOINTS);
	//alloue un tableau de points, les faces on s'en fout

	SG_PNT3D* tBDescrPoints=BTmp.DB.MemPoints;

	for(i=0; i<NBlocX*NBlocY; i++)
	{
		//le pointeur de face est facultatif car on copie le pointeur de point
		//et on actualise les reference de faces
		//SG_InitBloc(BlocDescr.Bloc[i],tBDescrPoints,TotalP[i],0,TotalF[i],SG_OK|SG_WithPOINTS);
		SG_PNT3D* PointsSource=BlocDescr.Bloc[i].DB.MemPoints;
		BlocDescr.Bloc[i].DB.MemPoints=tBDescrPoints;
		SG_InitBloc(BlocDescr.Bloc[i],PointsSource,PartP[i],0,0,SG_OK|SG_WithPOINTS);
		tBDescrPoints+=PartP[i];
	}
	BlocToClose.Etat&=~SG_WithPOINTS;
	SPG_MemFree(BlocToClose.DB.MemPoints);
	}


	if(TotalFacesBlocFinal<TotalFacesBloc)
	{
	SG_CreateBloc(BTmp,0,TotalFacesBlocFinal,SG_WithFACES);
	//alloue un tableau de faces, les points on s'en fout

	SG_FACE* tBDescrFaces=BTmp.DB.MemFaces;

	for(i=0; i<NBlocX*NBlocY; i++)
	{
		//le pointeur de points est facultatif car on copie le pointeur de faces
		//SG_FACE* FacesSource=BlocDescr.Bloc[i].DB.MemFaces;
		BlocDescr.Bloc[i].DB.MemFaces=tBDescrFaces;
		SG_InitBloc(BlocDescr.Bloc[i],0,0,tBDescrFaces,PartF[i],SG_OK|SG_WithFACES);
		tBDescrFaces+=PartF[i];
	}
	BlocToClose.Etat&=~SG_WithPOINTS;
	SPG_MemFree(BlocToClose.DB.MemPoints);
	}
	//verifier que
	//BlocDescr.Bloc[0]=SG_OK|SG_WithPOINTS|SG_WithFACES,
	//BlocDescr.Bloc[i>0]=SG_OK
	//SG_CheckDispatchDescr(BlocDescr);
#endif
	//si nombref=0 on peut faire etat=0 puisque la memoire n'est pas allouee,
	//sauf pour i=0 car il contient toute la memoire, auquel cas il faut reporter
	//cette responsabilite sur le suivant
	bool FirstToHaveMem=true;
	for(i=0; i<NBlocX*NBlocY; i++)
	{
		if(BlocDescr.Bloc[i].DB.NombreF==0) 
		{
			BlocDescr.Bloc[i].Etat=0;
		}
		else if (FirstToHaveMem)
		{
			BlocDescr.Bloc[i].Etat|=SG_WithFACES|SG_WithPOINTS|SG_OK;
			FirstToHaveMem=false;
		}
	}

#else

	for(i=0; i<NBlocX*NBlocY; i++)
	{
		DbgCHECK(PartF[i]>TotalF[i],"SG_ConcatAndDispatchBlocList: Erreur grave de nombre de faces");
		DbgCHECK(PartP[i]>TotalP[i],"SG_ConcatAndDispatchBlocList: Erreur grave de nombre de points");
		//DbgCHECK(PartF[i]<TotalF[i],"SG_ConcatAndDispatchBlocList: Moins de faces que prevu");
		//DbgCHECK(PartP[i]<TotalP[i],"SG_ConcatAndDispatchBlocList: Moins de points que prevu");
		if (PartF[i]&&(PartF[i]<=TotalF[i]))
		{//si PartF=0 on a quand meme alloue la memoire et on ne doit
		 //pas faire bloc.etat=0
			BlocResult[i].DB.NombreP=PartP[i];
			BlocResult[i].DB.NombreF=PartF[i];
			SG_MakeBRef(BlocResult[i]);
#ifdef CCAT_ReFitBlock
						if((PartP[i]<TotalP[i])||(PartF[i]<TotalF[i]))
						{
			SG_FullBloc BTmp=BlocResult[i];
			SG_DupliqueBloc(BTmp,BlocResult[i],SG_WithPOINTS|SG_WithFACES);
			SG_CloseBloc(BTmp);
			//alloue un nouveau bloc=SG_InitBloc des bons params

			/*
			SPG_MemFree(BlocResult[i].DB.MemFaces);
			SPG_MemFree(BlocResult[i].DB.MemPoints);
			BlocResult[i].DB.MemFaces=BTmp.DB.MemFaces;
			int j;
			for(j=0; j<BlocResult[i].DB.NombreF; j++)
			{
				*(int*)(&BlocResult[i].DB.MemFaces[j].NumP1)+=(int)BTmp.DB.MemPoints-(int)BlocResult[i].DB.MemPoints;
				*(int*)(&BlocResult[i].DB.MemFaces[j].NumP2)+=(int)BTmp.DB.MemPoints-(int)BlocResult[i].DB.MemPoints;
				*(int*)(&BlocResult[i].DB.MemFaces[j].NumP3)+=(int)BTmp.DB.MemPoints-(int)BlocResult[i].DB.MemPoints;
				*(int*)(&BlocResult[i].DB.MemFaces[j].NumP4)+=(int)BTmp.DB.MemPoints-(int)BlocResult[i].DB.MemPoints;
			}
			BlocResult[i].DB.MemPoints=BTmp.DB.MemPoints;
			*/
						}
#endif
		}
		else
		{//ici on desalloue et etat=0
			SG_CloseBloc(BlocResult[i]);
		}
	}
#endif

#undef BlocResult
	
#undef TotalP
#undef TotalF

	SPG_MemFree(TotalP);
	SPG_MemFree(PartP);
	SPG_MemFree(TotalF);
	SPG_MemFree(PartF);
	return -1;
	/*
FreeAndFail:
	SPG_MemFree(TotalP);
	SPG_MemFree(PartP);
	SPG_MemFree(TotalF);
	SPG_MemFree(PartF);
	return 0;
	*/
}

void SPG_CONV SG_CloseDispatch(SG_DispatchDescr& BlocDescr)
{
	if (BlocDescr.Bloc) 
	{
//#ifdef CCAT_SingleBlock//ce n'est pas forcement le premier bloc qui porte la memoire
//		if (BlocDescr.Bloc[0].Etat) SG_CloseBloc(BlocDescr.Bloc[0]);
//#else
		for(int n=0;n<SG_DispatchNombreB(BlocDescr);n++)
			if (SG_DispatchIndex(BlocDescr,n).Etat) SG_CloseBloc(SG_DispatchIndex(BlocDescr,n));
//#endif
		SPG_MemFree(BlocDescr.Bloc);
	}
	memset(&BlocDescr,0,sizeof(SG_DispatchDescr));
	return;
}

/*
int SG_ConcatBloc2(SG_FullBloc &BlocResult,
				   SG_FullBloc &BlocToAdd1,
				   SG_FullBloc &BlocToAdd2)
{
	SG_FullBloc *BlocList[2];
	BlocList[0] = &BlocToAdd1;
	BlocList[1] = &BlocToAdd2;
	return SG_ConcatBlocList(BlocResult,BlocList,2);
}

int SG_ConcatBloc3(SG_FullBloc &BlocResult,
				   SG_FullBloc &BlocToAdd1,
				   SG_FullBloc &BlocToAdd2,
				   SG_FullBloc &BlocToAdd3)
{
	SG_FullBloc* BlocList[3];
	BlocList[0] = &BlocToAdd1;
	BlocList[1] = &BlocToAdd2;
	BlocList[2] = &BlocToAdd3;
	return SG_ConcatBlocList(BlocResult,BlocList,3);
}
*/
int SPG_CONV SG_ConcatAndDispatchBloc5(SG_DispatchDescr& BlocResult,
									  int NBlockX,int NBlockY,float PntIdTol,
				   SG_FullBloc &BlocToAdd1,
				   SG_FullBloc &BlocToAdd2,
				   SG_FullBloc &BlocToAdd3,
				   SG_FullBloc &BlocToAdd4,
				   SG_FullBloc &BlocToAdd5)
{
	SG_FullBloc* BlocList[5];
	BlocList[0] = &BlocToAdd1;
	BlocList[1] = &BlocToAdd2;
	BlocList[2] = &BlocToAdd3;
	BlocList[3] = &BlocToAdd4;
	BlocList[4] = &BlocToAdd5;
	return SG_ConcatAndDispatchBlocList(BlocResult,NBlockX,NBlockY,PntIdTol,BlocList,5);
}

int SPG_CONV SG_ConcatAndDispatchBloc6(SG_DispatchDescr& BlocResult,
									  int NBlockX,int NBlockY,float PntIdTol,
				   SG_FullBloc &BlocToAdd1,
				   SG_FullBloc &BlocToAdd2,
				   SG_FullBloc &BlocToAdd3,
				   SG_FullBloc &BlocToAdd4,
				   SG_FullBloc &BlocToAdd5,
				   SG_FullBloc &BlocToAdd6)
{
	SG_FullBloc* BlocList[6];
	BlocList[0] = &BlocToAdd1;
	BlocList[1] = &BlocToAdd2;
	BlocList[2] = &BlocToAdd3;
	BlocList[3] = &BlocToAdd4;
	BlocList[4] = &BlocToAdd5;
	BlocList[5] = &BlocToAdd6;
	return SG_ConcatAndDispatchBlocList(BlocResult,NBlockX,NBlockY,PntIdTol,BlocList,6);
}

/*
void SPG_CONV SG_AddDispatchToVue(SG_PDV& Vue, SG_DispatchDescr& SDD)
{
	int NBlocs=SDD.BlockX*SDD.BlockY;
	for(int i=0;i<NBlocs;i++)
	{
		if (SDD.Bloc[i].Etat) SG_AddToVue(Vue,Terrain.Bloc[i]);
	}
	return;
}
*/

#endif



