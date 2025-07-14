

#include "SPG_General.h"

#ifdef SPG_General_USEInterpolateur

//#ifdef DebugFloat
#include <float.h>
//#endif

#include "SPG_Includes.h"

#define AngleMin 0.0001

//rajouter le cas ou on cherche un point tel que la suite
//soit alignee
#ifdef DebugInterpole
#define CHECKLONGUEUR(I) CHECK(I.Longueur==0,"I_CalcParams: Points confondus",I.Etat=0;return);CHECK(_finite(I.Longueur)==0,"I_CalcParams: Longueur invalide",I.Etat=0;return);CHECK(_finite(I.Longueur)==0,"I_CalcParams: Longueur invalide",I.Etat=0;return);CHECK(I.Longueur<0,"Longueur invalide",I.Etat=0;return);CHECK(I.Longueur>1.0e9,"Longueur invalide",I.Etat=0;return);
#else
#define CHECKLONGUEUR(I)
#endif

//Course est le vecteur joignant VPos au plan dans la direction vdir
int SPG_FASTCONV I_Intercept(V_VECT& Course, V_VECT& PlanPos, V_VECT& PlanNorm, V_VECT& VPos, V_VECT& Vdir)
{
	V_Normalise(PlanNorm);
	V_Normalise(Vdir);
	float f;
	V_ScalVect(PlanNorm,Vdir,f);
	//la direction est parallele au plan
	if (fabs(f)<0.0001)
		return 0;
	//on calcule la distance normale au plan (on prend un vecteur quelconque) projete sur la direction normale
	V_VECT VDiff;
	V_Operate3(VDiff,=PlanPos,-VPos);
	float t;
	V_ScalVect(VDiff,PlanNorm,t);
	//la vraie distance est (f=1/cos(angle))*(t=distance normale)
	f=t/f;
	V_Operate2(Course,=f*Vdir);
	V_CheckSup(Course);
	return -1;
}

//calcule la droite joignant deux points
void SPG_CONV I_CalcParamsLineaire(InterpolationParameters& I, V_VECT& PStart, V_VECT& PStop)
{
	I.Etat=0;
	I.PStart=PStart;
	I.PStop=PStop;

	V_CheckSup(I.PStart);
	V_CheckSup(I.PStop);

	V_VECT DiffDir;
	V_Operate3(DiffDir,=I.PStop,-I.PStart);
	V_ModVect(DiffDir,I.Longueur);
	CHECKLONGUEUR(I);
	V_Normalise(DiffDir);
	V_CheckSup(DiffDir);
	I.DirStop=I.DirStart=DiffDir;
	I.Etat=IP_LINEAIRE;
	return;
}

//calcule l'arc de cercle partant d'un point selon une direction et passant par un point
void SPG_CONV I_CalcParamsCirculaire(InterpolationParameters& I, V_VECT& PStart, V_VECT& DirStart, V_VECT& PStop)
{
	I.Etat=0;
	I.PStart=PStart;
	I.PStop=PStop;

	I.DirStart=DirStart;

	V_CheckSup(I.PStart);
	V_CheckSup(I.PStop);
	V_CheckSup(I.DirStart);

		V_VECT DiffDir;
		V_Operate3(DiffDir,=I.PStop,-I.PStart);
		V_ModVect(DiffDir,I.Longueur);
		CHECKLONGUEUR(I);
		V_Normalise(DiffDir);

	float NrmDirStart;
	V_ModVect(I.DirStart,NrmDirStart);
	if (NrmDirStart==0)
	{
		I.DirStop=I.DirStart=DiffDir;
#ifdef DebugList
		SPG_List("I_CalcParamsCirculaire: Troncon lineaire");
#endif
		I.Etat=IP_LINEAIRE;
		return;
	}

	V_Normalise(I.DirStart);
	I.DirStop=DirStart;

	V_VECT Middle;
	V_Operate3(Middle,=0.5f*I.PStart,+0.5f*I.PStop);

	V_VECT P_Normal;
	V_VectVect(DiffDir,I.DirStart,P_Normal);

	float NrmNormal;
	V_ModVect(P_Normal,NrmNormal);
	if(NrmNormal<AngleMin)
	{
		//normal, c'est colineaire
#ifdef DebugList
		SPG_List("I_CalcParamsCirculaire: Erreur dans l'interpolation\nCentre de courbure non trouvé\nPoints colineaires");
#endif
		I.DirStop=I.DirStart=DiffDir;
		I.Etat=IP_LINEAIRE;
		return;
	}
	V_Normalise(P_Normal);

	V_VECT DirFromMiddle;
	V_VectVect(DiffDir,P_Normal,DirFromMiddle);
	float NrmDirFromMiddle;
	V_ModVect(DirFromMiddle,NrmDirFromMiddle)
	if(NrmDirFromMiddle<AngleMin)
	{
		//mathematiquement c'est impossible d'en arriver la
#ifdef DebugList
		SPG_List("I_CalcParamsCirculaire: Erreur dans l'interpolation\nCentre de courbure non trouvé");
#endif
		I.Etat=IP_LINEAIRE;
		return;
	}
	V_Normalise(DirFromMiddle);

	V_VECT CenterFromMiddle;
	if (I_Intercept(CenterFromMiddle,I.PStart,I.DirStart,Middle,DirFromMiddle)==0)
	{	//la parcontre peut etre
#ifdef DebugList
		SPG_List("I_CalcParamsCirculaire: Erreur dans l'interpolation\nDirFromMiddle colineaire au plan I.DirStart");
#endif
		I.Etat=IP_LINEAIRE;
		return;
	}
	V_Operate3(I.Center,=Middle,+CenterFromMiddle);

	float Deport;
	V_ScalVect(I.DirStart,DiffDir,Deport);
	I.TetaMax=2*acos(V_Sature(Deport,-1,1));
	CHECK(!V_InclusiveBound(I.TetaMax,0,V_2PI),"I_CalcParamsCirculaire: TetaMax faux",I.TetaMax=0);

	V_Operate3(I.RayonStart,=I.PStart,-I.Center);
	I.DirStartNorm=I.RayonStart;
	V_Normalise(I.DirStartNorm);

	V_VECT RayonStop;
	V_Operate3(RayonStop,=I.PStop,-I.Center);
	V_VectVect(RayonStop,P_Normal,I.DirStop);

	V_Normalise(I.DirStop);
	V_VectVect(I.RayonStart,P_Normal,I.RayonNorm);
	float NrmRayonStart;
	V_ModVect(I.RayonStart,NrmRayonStart);
	I.Longueur=NrmRayonStart*I.TetaMax;
	CHECKLONGUEUR(I);
	I.Etat=IP_CIRCULAIRE;
	return;
}

//calcule l'arc polynomial parametrique partant d'un point selon une direction et passant par un point selon une direction
void SPG_CONV I_CalcParamsSPline(InterpolationParameters& I, V_VECT& PStart, V_VECT& DirStart, V_VECT& PStop, V_VECT& DirStop)
{
	I.Etat=0;
	I.PStart=PStart;
	I.PStop=PStop;

	I.DirStart=DirStart;
	I.DirStop=DirStop;

	V_CheckSup(I.PStart);
	V_CheckSup(I.PStop);
	V_CheckSup(I.DirStart);
	V_CheckSup(I.DirStop);

		V_VECT DiffDir;
		V_Operate3(DiffDir,=I.PStop,-I.PStart);
		V_ModVect(DiffDir,I.Longueur);
		CHECKLONGUEUR(I);
		V_Normalise(DiffDir);

		float NrmStart;
		V_ModVect(I.DirStart,NrmStart);
		float NrmStop;
		V_ModVect(I.DirStart,NrmStop);

	if ((NrmStart==0)||(NrmStop==0))
	{
#ifdef DebugList
		SPG_List("I_CalcParamsSPline: Troncon lineaire");
#endif
		I.DirStop=I.DirStart=DiffDir;
		I.Etat=IP_LINEAIRE;
		return;
	}
	
	I.SPLongueur=I.Longueur;

	V_Normalise(I.DirStart);
	V_Normalise(I.DirStop);
//	I.PolyFact=PolyFact;
	I.Etat=IP_SPLINE;

	V_VECT P1=I.PStart;
	V_VECT P2,D2;
	I_Interpole(I,0.125,P2,D2);
	V_VECT P3,D3;
	I_Interpole(I,0.25,P3,D3);
	V_VECT P4,D4;
	I_Interpole(I,0.375,P4,D4);
	V_VECT P5,D5;
	I_Interpole(I,0.5,P5,D5);
	V_VECT P6,D6;
	I_Interpole(I,0.625,P6,D6);
	V_VECT P7,D7;
	I_Interpole(I,0.75,P7,D7);
	V_VECT P8,D8;
	I_Interpole(I,0.875,P8,D8);
	V_VECT P9=I.PStop;
	//I.Longueur=0;
	V_ModDiffVect(P1,P2,I.Longueur);
	float Tmp;
	V_ModDiffVect(P2,P3,Tmp);
	I.Longueur+=Tmp;
	V_ModDiffVect(P3,P4,Tmp);
	I.Longueur+=Tmp;
	V_ModDiffVect(P4,P5,Tmp);
	I.Longueur+=Tmp;
	V_ModDiffVect(P5,P6,Tmp);
	I.Longueur+=Tmp;
	V_ModDiffVect(P6,P7,Tmp);
	I.Longueur+=Tmp;
	V_ModDiffVect(P7,P8,Tmp);
	I.Longueur+=Tmp;
	V_ModDiffVect(P8,P9,Tmp);
	I.Longueur+=Tmp;
	CHECKLONGUEUR(I);
	return;
}

/*
void I_CalcParamsPolynomial(InterpolationParameters& I, V_VECT& PStart, V_VECT& DirStart, V_VECT& PStop, V_VECT& DirStop, float PolyFact)
{
	I.Etat=0;
	I.PStart=PStart;
	I.PStop=PStop;

	I.DirStart=DirStart;
	I.DirStop=DirStop;

		V_VECT DiffDir;
		V_Operate3(DiffDir,=I.PStop,-I.PStart);
		I.Longueur=V_ModVect(DiffDir);
		CHECK(I.Longueur==0,"I_CalcParams: Points confondus",I.Etat=0;return)
		V_Normalise(DiffDir);

	if ((V_ModVect(I.DirStart)==0)||(V_ModVect(I.DirStop)==0))
	{
		I.DirStop=I.DirStart=DiffDir;
		I.Etat=IP_LINEAIRE;
		return;
	}

	V_Normalise(I.DirStart);
	V_Normalise(I.DirStop);
	I.PolyFact=PolyFact;
	I.Etat=IP_POLYNOMIAL;
}
*/
/*
void I_Check(InterpolationParameters& I)
{
	V_VECT Dest;
	if (I.Etat==IP_ARC)
	{
	V_Operate4(
		Dest,
		=I.Center,
		+cos(I.TetaMax)*I.RayonStart,
		+sin(I.TetaMax)*I.RayonNorm);
	V_VECT Tol;
	V_Operate3(Tol,=I.PStop,-Dest);
	CHECK(V_ModVect(Tol)>0.0001,"I_Check: Interpolation circulaire hors tolerance",return);
	return;
	}
	else if (I.Etat==IP_DROITE)
	{
	V_Operate3(
		Dest,
		=I.PStart,
		+I.Longueur*I.DirStart);
	V_VECT Tol;
	V_Operate3(Tol,=I.PStop,-Dest);
	CHECK(V_ModVect(Tol)>0.0001,"I_Check: Interpolation lineaire hors tolerance",return);
	return;
	}
	else
		DbgCHECK(1,"I_Check: Interpolation non initialisee");
		return;
}
*/

//interpole avec les parametre calcules, donne la position et la direction sur un intervalle 0<AbsCurv<1
void SPG_CONV I_Interpole(InterpolationParameters& I, float AbscCurv, V_VECT& Vpos, V_VECT& Vdir)
{
	DbgCHECK(V_InclusiveBound(AbscCurv,0,1)==0,"I_Interpole: interpolation hors limites");
	DbgCHECK(I.Etat==0,"I_Interpole: interpolation non initialisee");
	CHECKLONGUEUR(I);
	/*
	if (I.Etat==IP_POLYNOMIAL)
	{
		//SPG_List("I_Interpole: IP_POLYNOMIAL");
	V_VECT Vsrc,Vdst;
	V_Operate3(Vsrc,=I.PStart,+AbscCurv*I.PolyFact*I.Longueur*I.DirStart);
	V_Operate3(Vdst,=I.PStop,+(AbscCurv-1)*I.PolyFact*I.Longueur*I.DirStop);
	float Ksrc=(1-AbscCurv)*(1-AbscCurv);
	float Kdst=AbscCurv*AbscCurv;
	float K2src=Ksrc/(Ksrc+Kdst);
	float K2dst=Kdst/(Ksrc+Kdst);
	V_Operate3(Vpos,=K2src*Vsrc,+K2dst*Vdst);
	//V_Operate3(Vdir,=K2src*I.DirStart,+K2dst*I.DirStop);
	V_VECT Vdiff;
	V_Operate3(Vdiff,=Vdst,-Vsrc);
	V_Normalise(Vdiff);
	V_Operate4(Vdir,
		=(1-AbscCurv)*I.DirStart,
		+AbscCurv*I.DirStop,
		+AbscCurv*(1-AbscCurv)*Vdiff);
	V_Normalise(Vdir);
	return;
	}
	*/
	if (I.Etat==IP_SPLINE)
	{
		//attention bug du compilateur
		float UnA=(1-AbscCurv);
		float UnA2=UnA*UnA;
		float UnA3=UnA2*UnA;
		float a=(float)(3.0*UnA2-2.0*UnA3);
		//V_CheckFloat(a,"I_Interpole IP_SPLINE");
			//integrale de 6*(1-AbscCurv)*AbscCurv;

		float b=(float)(1.5*AbscCurv*UnA2*I.SPLongueur);//3*AbscCurv*(1-AbscCurv)*(1-AbscCurv);
		//V_CheckFloat(b,"I_Interpole IP_SPLINE");
		// *I.PolyFact

		float c=(float)1.0-a;
		//V_CheckFloat(c,"I_Interpole IP_SPLINE");

		float d=(float)(1.5*AbscCurv*AbscCurv*UnA*I.SPLongueur);
		//V_CheckFloat(d,"I_Interpole IP_SPLINE");

		V_Operate5(
			Vpos,=a*I.PStart,
			+b*I.DirStart,
			+c*I.PStop,
			-d*I.DirStop);

		float ap=6*(1-AbscCurv)*AbscCurv;// *Vdiff
		float bp=(float)(1.5*I.SPLongueur*((1-AbscCurv)*(1-AbscCurv)-2*AbscCurv*(1-AbscCurv)));
		float dp=(float)(1.5*I.SPLongueur*(AbscCurv*AbscCurv-2*AbscCurv*(1-AbscCurv)));

		V_VECT VDiff;
		V_Operate3(VDiff,=I.PStop,-I.PStart);
		//V_Normalise(VDiff);
		V_Operate4(
			Vdir,=ap*VDiff,
			+bp*I.DirStart,
			+dp*I.DirStop);
		//Vdir=I.DirStart;
		V_Normalise(Vdir);
	V_CheckSup(Vpos);
	V_CheckSup(Vdir);

/*
	V_VECT Vsrc,Vdst;
	V_Operate3(Vsrc,=I.PStart,+AbscCurv*I.PolyFact*I.Longueur*I.DirStart);
	V_Operate3(Vdst,=I.PStop,+(AbscCurv-1)*I.PolyFact*I.Longueur*I.DirStop);
	float Ksrc=(1-AbscCurv)*(1-AbscCurv);
	float Kdst=AbscCurv*AbscCurv;
	float K2src=Ksrc/(Ksrc+Kdst);
	float K2dst=Kdst/(Ksrc+Kdst);
	V_Operate3(Vpos,=K2src*Vsrc,+K2dst*Vdst);
	//V_Operate3(Vdir,=K2src*I.DirStart,+K2dst*I.DirStop);
	V_VECT Vdiff;
	V_Operate3(Vdiff,=Vdst,-Vsrc);
	V_Normalise(Vdiff);
	V_Operate4(Vdir,
		=(1-AbscCurv)*I.DirStart,
		+AbscCurv*I.DirStop,
		+AbscCurv*(1-AbscCurv)*Vdiff);
	V_Normalise(Vdir);
*/
	return;
	}
	else if (I.Etat==IP_CIRCULAIRE)
	{
		DbgCHECK(I.TetaMax<0.001,"I_Interpole: IP_CIRCULAIRE: Longueur trop faible");
		float fc=cos(I.TetaMax*AbscCurv);
		float fs=sin(I.TetaMax*AbscCurv);
	V_Operate4(
		Vpos,
		=I.Center,
		+fc*I.RayonStart,
		+fs*I.RayonNorm);
	V_Operate3(
		Vdir,
		=fc*I.DirStart,
		-fs*I.DirStartNorm);
	/*
	V_VECT Tol;
	V_Operate3(Tol,=I.PStop,-Dest);
	CHECK(V_ModVect(Tol)>0.0001,"I_Check: Interpolation circulaire hors tolerance",return);
	*/
	V_CheckSup(Vpos);
	V_CheckSup(Vdir);
	return;
	}
	else if (I.Etat==IP_LINEAIRE)
	{
		DbgCHECK(I.Longueur<0.001,"I_Interpole: IP_LINEAIRE: Longueur trop faible");
	V_Operate3(
		Vpos,
		=I.PStart,
		+I.Longueur*AbscCurv*I.DirStart);
	Vdir=I.DirStart;
	/*
	V_VECT Tol;
	V_Operate3(Tol,=I.PStop,-Dest);
	CHECK(V_ModVect(Tol)>0.0001,"I_Check: Interpolation lineaire hors tolerance",return);
	*/
	V_CheckSup(Vpos);
	V_CheckSup(Vdir);
	return;
	}
#ifdef DebugList
	else
		SPG_List("I_Interpole: Interpolation non initialisee");
#endif
		return;
}

#endif
