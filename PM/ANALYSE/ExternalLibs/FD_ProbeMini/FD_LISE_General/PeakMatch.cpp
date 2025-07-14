/*
 * $Id: PeakMatch.cpp 6919 2008-02-28 16:44:44Z y-randle $
 */


#include "..\SrcC\SPG.h"
#include "..\SrcC\SPG_SysInc.h"

#include "PeakMatch.h"

#include <memory.h>


#define DefEnterNoLock() __try { 
#define DefLock() WaitForSingleObject( SD.lock, 10000 );
#define DefEnter() DefEnterNoLock() DefLock()

#define DefExitUnlock() SetEvent( SD.lock );
#define DefExitNoLock() } __except(EXCEPTION_EXECUTE_HANDLER) { };
#define DefExit() DefExitNoLock() DefExitUnlock()

int DefInit(SAMPLEDEFINITION& SD, int MaxPks)
{
	SPG_ZeroStruct(SD);
    DefEnterNoLock();
    SD.lock = CreateEvent( 0, false, false, 0 );
	SD.MaxPks=MaxPks;
    DefMatchAlloc( SD.Best, MaxPks );
    SD.PkDef = SPG_TypeAlloc( MaxPks, PEAKDEFINITION, "DefInit");
    SD.PkDefBkp = SPG_TypeAlloc( MaxPks, PEAKDEFINITION, "DefInit");
    SD.fIndex = SPG_TypeAlloc( MaxPks, double, "DefInit");
    SD.fIntensity = SPG_TypeAlloc( MaxPks, double, "DefInit");
    DefExit();
	return SD.state = -1;
}

void DefClose(SAMPLEDEFINITION& SD)
{
    DefEnter();
    DefMatchFree( SD.Best );
    SPG_MemFree(SD.PkDef);
    SPG_MemFree(SD.PkDefBkp);
    SPG_MemFree(SD.fIndex);
    SPG_MemFree(SD.fIntensity);
    CloseHandle( SD.lock );
	SPG_ZeroStruct(SD);
    DefExitNoLock();
	return;
}

int DefAdd(SAMPLEDEFINITION& SD, PEAKTYPE PkType, PEAKREFERENCETYPE PkRefType, double ExpectedPosition, double Tolerance, PEAKWEIGHT W)
{
    DefEnter();
    CHECK(SD.state != -1,"DefAdd", DefExitUnlock(); return 0);
    CHECK(SD.NumPks >= SD.MaxPks,"DefAdd", DefExitUnlock(); return 0);

	SD.PkDef[SD.NumPks].PkType=PkType;

	SD.PkDef[SD.NumPks].ExpectedPosition=ExpectedPosition;
	SD.PkDef[SD.NumPks].Tolerance=Tolerance;

	SD.PkDef[SD.NumPks].PkRefType=PkRefType;
	if(PkRefType==PkRefOpticalRef)
	{
		int i;
		for(i=0;i<SD.NumPks;i++)
		{
			if(SD.PkDef[i].PkType==PkOptRef) break;
		}
		CHECK(i==SD.NumPks,"DefAdd: PkRefType==PkRefOpticalRef, No Optical Reference defined",DefExitUnlock(); return SD.NumPks);
		SD.PkDef[SD.NumPks].PkRef=i;
	}
	else if(PkRefType==PkRefPrevious)
	{
		CHECK(SD.NumPks<=0,"DefAdd: PkRefType==PkRefPrevious, No Previous Peak defined",DefExitUnlock(); return SD.NumPks);
		SD.PkDef[SD.NumPks].PkRef=SD.NumPks-1;
	}
	SD.PkDef[SD.NumPks].W=W;

	SD.NumPks++;
    DefExit();
	return SD.NumPks;
}

int DefClear(SAMPLEDEFINITION& SD)
{
    DefEnter();
	SD.NumPks = 0;
    DefExit();
	return SD.NumPks;
}

void DefBackup(SAMPLEDEFINITION& SD)
{
	//SD.PkDefBkp = SD.PkDef;
	// copie des donnée
	for(int i=0;i<SD.MaxPks;i++)
	{
		SD.PkDefBkp[i] = SD.PkDef[i];
	}
	return; 
}

void DefRestore(SAMPLEDEFINITION& SD)
{
	//SD.PkDef = SD.PkDefBkp;
	// copies des données pour le restore
	for(int i=0;i<SD.MaxPks;i++)
	{
		SD.PkDef[i] = SD.PkDefBkp[i];
	}
	return;
}

double DefComputeWeight(double Distance, double Tolerance, PEAKWEIGHT W)
{
	if(!V_InclusiveBound(Distance,-Tolerance,Tolerance)) return 0;

// Début modif YR du 28 Février 2008 : Modification du calcul de la qualité des pics et des épaisseurs.
	/*
	if(W==WeightUniform)
	{
		 return 0.5/Tolerance;
	}
	else if(W==WeightCos)
	{
		return (0.5+0.5*cos(V_PI*Distance/Tolerance)) /Tolerance;
	}
	else if(W==WeightPeak)
	{
		return 2.0/(log(5.0)*(Tolerance+4.0*fabs(Distance)));
	}
	*/
	if(W==WeightUniform)
	{
		 return 1.0;
	}
	else if(W==WeightCos)
	{
		return (1.0+cos(V_PI*Distance/Tolerance));
	}
	else if(W==WeightPeak)
	{
		return 3.0/(1.0+4.0*fabs(Distance)/Tolerance);
	}
// Fin modif YR.

	return 0.0;
}

double DefComputeQuality(const SAMPLEDEFINITION& SD, const MEASPEAK* PkMeas, const DEFMATCHING& DM)
{
    CHECK(SD.state != -1,"DefAdd", return 0);
	CHECK(SD.NumPks!=DM.NumPks,"DefComputeQuality: Mismatch between Sample definition and Matching array",return 0);

	double QualitySum=0;

	for(int i=0;i<DM.NumPks;i++)
	{//pour tous les pics du modele
		int PkMesIndex=DM.PkIndex[i];//i = index dans l'échantillon - MesIndex = Index dans la mesure - PkIndex matchning courant entre echantillon et mesure

		double MeasuredPosition=PkMeas[PkMesIndex].Position;

		if(SD.PkDef[i].PkRefType!=PkRefAbsolute)
		{//la position du pic était définie dans le modele par rapport à un autre pic (reference optique ou un pic de sample)
			int PkOriginMesIndex=DM.PkIndex[SD.PkDef[i].PkRef];
			MeasuredPosition-=PkMeas[PkOriginMesIndex].Position;
		}

#ifdef DebugPeaks
		//pour debug
		DM.MeasuredPosition[i]=MeasuredPosition;
		DM.ExpectedPosition[i]=SD.PkDef[i].ExpectedPosition;
		//pour debug
#endif
		double Quality1Pic=DefComputeWeight(MeasuredPosition-SD.PkDef[i].ExpectedPosition,SD.PkDef[i].Tolerance,SD.PkDef[i].W)*PkMeas[PkMesIndex].Quality;
		if(Quality1Pic==0) return 0;
		// Modif YR 9 Sept 2008
		QualitySum+=Quality1Pic;
		//if (SD.PkDef[i].PkType!=PkOptRef)
		//{
		//	QualitySum+=Quality1Pic;
		//}
		//QualitySum+=DefComputeWeight(MeasuredPosition-SD.PkDef[i].ExpectedPosition,SD.PkDef[i].Tolerance,SD.PkDef[i].W)*PkMeas[i].Quality;
	}

	return QualitySum;
}

void DefMatchAlloc(DEFMATCHING& d, int MaxPks)
{
	SPG_ZeroStruct(d);
	d.MaxPks=MaxPks;
    d.PkIndex = SPG_TypeAlloc( MaxPks, int, "DefMatchingInit");
	return;
}

void DefMatchFree(DEFMATCHING& d)
{
    SPG_MemFree(d.PkIndex);
	SPG_ZeroStruct(d);
	return;
}

int DefMatchStart(DEFMATCHING& D, const SAMPLEDEFINITION& SD, int NumMesPks)
{
	// CHECK(NumMesPks<SD.NumPks,"DefMatchStart : cannot match with less peaks in the signal than in sample definition",return 0);
	if(NumMesPks<SD.NumPks) return 0;
	if(NumMesPks>D.MaxPks) NumMesPks = D.MaxPks;
	CHECK(D.MaxPks!=SD.MaxPks,"DefMatchStart",return 0);
	//D.MaxPks=SD.MaxPks;
	D.NumPks=SD.NumPks;
	D.NumMesPk=NumMesPks;
	for(int i=0;i<D.NumPks;i++)
	{//les index pointent sur les pics [0,..,D.NumPk-1] et la mesure contient les pics [0,...,D.NumMesPk-1]
		D.PkIndex[i]=i;
	}
	return D.NumPks;
}

int DefMatchNext(DEFMATCHING& D)
{
	if(D.PkIndex[D.NumPks-1]<D.NumMesPk-1)
	{//cas le plus fréquent: on incrémente le dernier index sauf s'il pointe sur le dernier pic de la mesure
		D.PkIndex[D.NumPks-1]++;
		return -1;
	}
	else
	{//cas ou on doit ramener le(s) derniers index accolés aux premiers apres avoir incrémenté l'un des index antérieurs
		for(int Rang=D.NumPks-2;Rang>=0;Rang--)
		{//cherche quel index on peut incrémenter
			if(D.PkIndex[Rang]<D.PkIndex[Rang+1]-1) 
			{
				D.PkIndex[Rang]++;
				for(int RzRang=Rang+1;RzRang<D.NumPks;RzRang++)
				{//les index suivant celui qu'on a incrémenté sont consécutifs
					D.PkIndex[RzRang]=1+D.PkIndex[RzRang-1];
				}
				return -1;
			}//incrementation réalisée
		}
		//arrivé là c'est que les index pointent sur les derniers pics, [D.NumMesPk-D.NumPks,...,D.NumMesPk-1], on a terminé
	}
	return 0;
}

#ifdef DebugPeaks
int DefMatchCheck(DEFMATCHING& D)
{
	for(int i=0;i<D.NumPks;i++)
	{
		CHECK(!V_IsBound(D.PkIndex[i],0,D.NumMesPk),"DefMatchCheck",return 0);
		if(i>0)
		{
			CHECK(D.PkIndex[i]<=D.PkIndex[i-1],"DefMatchCheck",return 0)
		}
	}
	return -1;
}
#endif

void DefCopy(DEFMATCHING& dest, const DEFMATCHING& src)
{
	dest.MaxPks = src.MaxPks;
	CHECK(src.NumPks>dest.MaxPks,"DefCopy", return);
	dest.NumPks = src.NumPks;
	dest.NumMesPk = src.NumMesPk;
	for(int i = 0; i<src.NumMesPk; i++) { dest.PkIndex[i] = src.PkIndex[i]; }
	dest.MeasuredPosition = src.MeasuredPosition;
	dest.ExpectedPosition = src.ExpectedPosition;
}

void DefMatch(const MEASPEAK* PkMeas, int NumMeasPks, const SAMPLEDEFINITION& SD, double& outQuality, int* outPkIndex, int MaxPks)
{
    CHECK(SD.state != -1,"DefMatch", return);

    DefEnter();

	outQuality=0;

	DEFMATCHING Current;	DEFMATCHING Best;
    DefMatchAlloc( Current, SD.MaxPks ); DefMatchAlloc( Best, SD.MaxPks ); 
	double CurQuality=0;	double BestQuality=0;

	int Count=0;

	DefMatchStart(Best,SD,NumMeasPks);
	if(DefMatchStart(Current,SD,NumMeasPks))
	{
#ifdef DebugPeaks
		SD.Count=0;
#endif
		do
		{
#ifdef DebugPeaks
			//pour debug
			DefMatchCheck(Current);
			//pour debug
#endif

			CurQuality=DefComputeQuality(SD,PkMeas,Current);
			if(CurQuality>BestQuality) 
			{
				DefCopy(Best,Current);
				BestQuality=CurQuality;
			}
#ifdef DebugPeaks
			//pour debug
			SD.Count++;
#endif
		} while(DefMatchNext(Current));
	}

#ifdef DebugPeaks
	//pour debug
	SD.Best=Best;
	SD.BestQuality=BestQuality;
	//pour debug
#endif

	int i;
	for(i=0;i<SD.NumPks;i++)
	{
		outPkIndex[i]=Best.PkIndex[i];
	}
	for(;i<MaxPks;i++)
	{
		outPkIndex[i]=0;
	}
	outQuality=BestQuality;

    DefMatchFree( Current ); DefMatchFree( Best ); 
    DefExit();
	return;
}

