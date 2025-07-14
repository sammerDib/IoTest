
#include "SPG_General.h"

#ifdef SPG_General_USEMECA

#include "SPG_Includes.h"


#ifdef DebugFloat
#include <float.h>
#endif
#include <string.h>
//pas de rotation posible
//#define LockRotate
//pas de rotation solide par deplacement,
//seulement par les forces
//#define LockSolidRotate
//pas de vitesse calculees par rotation
//#define LockOutRotate

int MECA_CONV MECA_Create(MECA_Bloc& MB, int NombreP, V_REPERE& Rep, V_VECT& Translation, V_VECT& Rotation, float Masse, float Moment, float Gravite)
{
	memset(&MB,0,sizeof(MECA_Bloc));
	CHECK((MB.NombreP=NombreP)<=0,"MECA_Create: MECA_Bloc nul",return 0);
	CHECK((MB.inP=SPG_TypeAlloc(MB.NombreP,MECA_Point,"MECA_Create"))==0,"MECA_Create: allocation echouee",MECA_Close(MB);return 0);
	CHECK((MB.outP=SPG_TypeAlloc(MB.NombreP,MECA_Point,"MECA_Create"))==0,"MECA_Create: allocation echouee",MECA_Close(MB);return 0);
	CHECK((MB.Point=SPG_TypeAlloc(MB.NombreP,V_VECT,"MECA_Create"))==0,"MECA_Create: allocation echouee",MECA_Close(MB);return 0);
	MB.Masse=Masse;
	MB.Moment=Moment;
	MB.Gravite=Gravite;
	MB.Rep=Rep;//repere
	MB.Translation=Translation;
	MB.Rotation=Rotation;
	return -1;
}

int MECA_CONV MECA_CreateFrom3DBloc(MECA_Bloc& MB, SG_FullBloc& B, V_REPERE& Rep, V_VECT& Translation, V_VECT& Rotation, float Masse, float Moment, float Gravite)
{
	CHECK((MB.NombreP=B.DB.NombreP)==0,"MECA_CreateFrom3DBloc: Bloc vide",return 0);
	CHECK(MECA_Create(MB,B.DB.NombreP,Rep,Translation,Rotation,Masse,Moment,Gravite)==0,"MECA_CreateFrom3DBloc: MECA_Create echoue",return 0);

	int i;
	for(i=0;i<MB.NombreP;i++)
	{
		MB.Point[i]=B.DB.MemPoints[i].P;//coordonnees dans le repere du CG
		
		MB.outP[i].pos=B.DB.MemPoints[i].P;//coordonnees dans le repere du CG
		MB.outP[i].Vitesse=Translation;//coordonnees dans le repere du CG
		V_VECT Zero={0,0,0};
		MB.outP[i].Force=Zero;//coordonnees dans le repere du CG
		
	}
	MECA_CalcGlobalOut(MB,0);

	MB.BRef=B.BRef;
	MB.Rayon=B.Rayon;

	//MECA_CalcGlobalRep(MB.outP,MB.NombreP,MB.Rep);

	return -1;
}

void MECA_CONV MECA_Close(MECA_Bloc& MB)
{
	if (MB.inP) SPG_MemFree(MB.inP);
	if (MB.outP) SPG_MemFree(MB.outP);
	if (MB.Point) SPG_MemFree(MB.Point);
	memset(&MB,0,sizeof(MECA_Bloc));
	return;
}

void MECA_CONV MECA_Generate(MECA_Bloc& MB, SG_FullBloc& B)
{

	CHECK(MB.NombreP!=B.DB.NombreP,"MECA_Generate: MECA ne peut generer dans un SG_FullBloc non deja genere par V_GenereBloc",return);

	int i;//deplace les points de l'objet
	for(i=0;i<MB.NombreP;i++)
	{
		B.DB.MemPoints[i].P=MB.outP[i].pos;
		/*
		V_Operate4(B.DB.MemPoints[i].P,
			//=MB.Rep.pos,//outP est deja en coordonnees absolues
			=MB.outP[i].pos.x*MB.Rep.axex,
			+MB.outP[i].pos.y*MB.Rep.axey,
			+MB.outP[i].pos.z*MB.Rep.axez);
			*/
	}

	V_Operate3(B.BRef,=MB.BRef,+MB.Rep.pos);
	B.Rayon=MB.Rayon;

	return;
}

void MECA_CONV MECA_CalcLocalRep(MECA_Point* P, int NombreP, V_REPERE& Rep)
{
	for(int i=0;i<NombreP;i++)
	{
	V_VECT Absolu=V_Operate3V(Absolu,=P[i].pos,-Rep.pos);
	V_ScalVect(Absolu,Rep.axex,P[i].pos.x);
	V_ScalVect(Absolu,Rep.axey,P[i].pos.y);
	V_ScalVect(Absolu,Rep.axez,P[i].pos.z);
	Absolu=P[i].Vitesse;
	V_ScalVect(Absolu,Rep.axex,P[i].Vitesse.x);
	V_ScalVect(Absolu,Rep.axey,P[i].Vitesse.y);
	V_ScalVect(Absolu,Rep.axez,P[i].Vitesse.z);
	Absolu=P[i].Force;
	V_ScalVect(Absolu,Rep.axex,P[i].Force.x);
	V_ScalVect(Absolu,Rep.axey,P[i].Force.y);
	V_ScalVect(Absolu,Rep.axez,P[i].Force.z);
	}
	return;
}

void MECA_CONV MECA_CalcGlobalRep(MECA_Point* P, int NombreP, V_REPERE& Rep)
{
	for(int i=0;i<NombreP;i++)
	{
	V_VECT Relatif=P[i].pos;
	V_Operate5(P[i].pos,=Rep.pos,+Relatif.x*Rep.axex,+Relatif.y*Rep.axey,+Relatif.z*Rep.axez);
	Relatif=P[i].Vitesse;
	V_Operate4(P[i].Vitesse,=Relatif.x*Rep.axex,+Relatif.y*Rep.axey,+Relatif.z*Rep.axez);
	Relatif=P[i].Force;
	V_Operate4(P[i].Force,=Relatif.x*Rep.axex,+Relatif.y*Rep.axey,+Relatif.z*Rep.axez);
	}
	return;
}

void MECA_CONV MECA_Update(MECA_Bloc& MB,float DT)
{
	//calcule les variables internes avec le bilan des forces
	MECA_CalcLocalRep(MB.inP,MB.NombreP,MB.Rep);
	MECA_CalcLocalRep(MB.outP,MB.NombreP,MB.Rep);

	V_VECT Bilan_pos={0,0,0};
#ifndef LockRotate
	V_VECT Bilan_Orientation={0,0,0};
#endif
	V_VECT Bilan_Translation={0,0,0};
#ifndef LockRotate
#ifndef LockSolidRotate
	V_VECT Bilan_Rotation={0,0,0};
#endif
#endif
	V_VECT Bilan_Force={0,0,0};
#ifndef LockRotate
	V_VECT Bilan_Moment={0,0,0};
#endif

if(MB.inDiscreteInteraction)
{	

	for(int i=0;i<MB.NombreP;i++)
	{

		{//pos, Orientation
		V_VECT Deplacement=V_Operate2V(MB.inP[i].pos,-MB.outP[i].pos);

		V_Operate2(Bilan_pos,+=Deplacement);

#ifndef LockRotate
#ifndef LockSolidRotate
		V_VECT LocalRotation;
		V_VectVect(MB.inP[i].pos,Deplacement,LocalRotation);
		float RotNorm;
		V_InvModVect(MB.inP[i].pos,RotNorm);
		V_Operate2(Bilan_Orientation,
			+=RotNorm*LocalRotation);
		//verifier 1/r²
#endif
#endif
		}

		{//Translation, Rotation
		V_VECT RelativeVitesse=V_Operate2V(MB.inP[i].Vitesse,-MB.outP[i].Vitesse);

		V_Operate2(Bilan_Translation,+=RelativeVitesse);

#ifndef LockRotate
#ifndef LockSolidRotate
		V_VECT LocalRotation;
		V_VectVect(MB.inP[i].pos,RelativeVitesse,LocalRotation);
		float RotNorm;
		V_InvModVect(MB.inP[i].pos,RotNorm);
		V_Operate2(Bilan_Rotation,
			+=RotNorm*LocalRotation);
		//verifier 1/r²
#endif
#endif
		}

		{//Force,Moment
		V_Operate2(Bilan_Force,+=MB.inP[i].Force);

#ifndef LockRotate
		V_VECT LocalMoment;
		V_VectVect(MB.inP[i].pos,MB.inP[i].Force,LocalMoment);
		V_Operate2(Bilan_Moment,+=LocalMoment);
#endif
		}
	}

//	CHECK(V_Mod2Vect(
	/*
	DbgCHECK(V_Mod2Vect(Bilan_pos)>1.0e-7,"MECA_Update: Mouvement interdit");
	//DbgCHECK(V_Mod2Vect(Bilan_Orientation)>1.0e-7,"MECA_Update: Mouvement interdit");
	DbgCHECK(V_Mod2Vect(Bilan_Translation)>1.0e-7,"MECA_Update: Mouvement interdit");
	//DbgCHECK(V_Mod2Vect(Bilan_Rotation)>1.0e-7,"MECA_Update: Mouvement interdit");
	DbgCHECK(V_Mod2Vect(Bilan_Force)>1.0e-7,"MECA_Update: Mouvement interdit");
	//DbgCHECK(V_Mod2Vect(Bilan_Moment)>1.0e-7,"MECA_Update: Mouvement interdit");
	*/

	float InvinDiscrete=1.0f/MB.inDiscreteInteraction;

	V_Operate1(Bilan_pos,*=InvinDiscrete);
#ifndef LockRotate
	V_Operate1(Bilan_Orientation,*=InvinDiscrete);
#endif
	V_Operate1(Bilan_Translation,*=InvinDiscrete);
#ifndef LockRotate
#ifndef LockSolidRotate
	V_Operate1(Bilan_Rotation,*=InvinDiscrete);
#endif
#endif
	V_Operate1(Bilan_Force,*=InvinDiscrete);
#ifndef LockRotate
	V_Operate1(Bilan_Moment,*=InvinDiscrete);
#endif
	
}
	//Bilan_Force,Bilan_Moment

	//ici on est en local, un chment de repere s'imposerait
	//A FINIR
	//Bilan_Force.z-=MB.Masse*MB.Gravite;
	Bilan_Force.x-=MB.Masse*MB.Gravite*MB.Rep.axex.z;
	Bilan_Force.y-=MB.Masse*MB.Gravite*MB.Rep.axey.z;
	Bilan_Force.z-=MB.Masse*MB.Gravite*MB.Rep.axez.z;

	//puis on remonte

	V_Operate3(MB.Translation,+=Bilan_Translation,+DT/MB.Masse*Bilan_Force);

#ifndef LockRotate
#ifndef LockSolidRotate
	V_Operate3(MB.Rotation,+=Bilan_Rotation,+DT/MB.Moment*Bilan_Moment);
#else
	V_Operate2(MB.Rotation,+=DT/MB.Moment*Bilan_Moment);
#endif
#endif

	V_Operate2(Bilan_pos,+=DT*MB.Translation);

#ifndef LockRotate
	V_Operate2(Bilan_Orientation,+=DT*MB.Rotation);
#endif

	V_TranslateRepS(MB.Rep,Bilan_pos);
//attention les vecteurs translation 
//ne doivent pas tourner avec le repere local!!!
	//A FINIR: Deverouiller la rotation
#ifndef LockRotate

	{
	V_VECT VGlob;
	V_ProjAbs(MB.Rep,MB.Translation,VGlob);
	V_RotateRepS(MB.Rep,Bilan_Orientation);
	V_ProjRep(MB.Rep,VGlob,MB.Translation);
	}
	//V_RotateRepS(MB.Rep,Bilan_Orientation);
#endif

	V_Orthonorme(MB.Rep);
//A FINIR
	MECA_CalcGlobalOut(MB,DT);

	return;
}

void MECA_CONV MECA_CalcGlobalOut(MECA_Bloc& MB, float DT)
{
	for(int i=0;i<MB.NombreP;i++)
	{
		MB.outP[i].pos=MB.Point[i];//regenere les points d'apres le modele: ici solide indeformable
//A FINIR
#ifndef LockRotate
#ifndef LockSolidRotate
#ifndef LockOutRotate
		V_VectVect(MB.Rotation,MB.outP[i].pos,MB.outP[i].Vitesse);//vitesse en rotation
		//V_VectVect(MB.outP[i].pos,MB.Rotation,MB.outP[i].Vitesse);//vitesse en rotation
#endif
#endif
#endif
		
		if(DT>0)
		{//force qu'on recevrait si on bloquait ce point
		float PosDist2;
		V_Mod2Vect(MB.outP[i].pos,PosDist2);
		float ForceNorm=MB.Moment/DT/PosDist2;
		V_Operate2(MB.outP[i].Force,=ForceNorm*MB.outP[i].Vitesse);
		}
		else
			V_SetXYZ(MB.outP[i].Force,0,0,0)
		
#ifndef LockRotate
#ifndef LockSolidRotate
#ifndef LockOutRotate
		V_Operate2(MB.outP[i].Vitesse,+=MB.Translation);
#else
		MB.outP[i].Vitesse=MB.Translation;
#endif
#else
		MB.outP[i].Vitesse=MB.Translation;
#endif
#else
		MB.outP[i].Vitesse=MB.Translation;
#endif
		
		if(DT>0)
		{
		float ForceNorm=MB.Masse/DT;
		V_Operate2(MB.outP[i].Force,+=ForceNorm*MB.Translation);
		}
		else
			V_SetXYZ(MB.outP[i].Force,0,0,0)
		

	}


	MECA_CalcGlobalRep(MB.outP,MB.NombreP,MB.Rep);
	if(DT==0) 
	{
		for(int i=0; i<MB.NombreP; i++)
		{
		MB.inP[i]=MB.outP[i];
		}
	}

	MB.inDiscreteInteraction=0;

	return;
}

/*

  a^b=ay.bz-az.by 
      az.bx-ax.bz
	  ax.by-ay.bx

*/

#endif



