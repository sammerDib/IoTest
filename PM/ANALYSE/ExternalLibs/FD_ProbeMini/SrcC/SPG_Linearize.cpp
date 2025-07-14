
#include "SPG_General.h"
#include "SPG_Includes.h"
#include <memory.h>
#include <float.h>

int SPG_CONV SPG_LinInit(SPG_LINTABLE& P, double* D, int NX, int NY)
{
	SPG_ZeroStruct(P);

	CHECK((P.LX=D)==0,"SPG_LinInit: not loaded",return 0);
	CHECK((NX<1)||(NY!=2),"SPG_LinInit\nFirst line Position (Tabulation separated)\nSecond line Correction (Tabulation separated)",SPG_MemFree(P.LX);SPG_ZeroStruct(P);return 0);
	P.LY=P.LX+NX;
	P.N=NX;
	return -1;
}

int SPG_CONV SPG_LinInit(SPG_LINTABLE& P, char* Workdir, char* F)
{
	SPG_ZeroStruct(P);
	CHECK((F==0)||(F[0]==0),"SPG_LinInit: null file",return 0)
	char Filename[MaxProgDir];SPG_ConcatPath(Filename,Workdir,F);

	int NX=0; int NY=0;
	double* D=Text_ReadDouble(Filename,NX,NY);//la première ligne du fichier est la position en pas de règle
	CHECKTWO(D==0,"SPG_LinInit",F,SPG_ZeroStruct(P);return 0);

	//s'il n'y a que deux colonnes, et deux lignes où davantage, on transpose (texte en colonne est plus conventionnel)
	if((NX==2)&&(NY>=2))
	{
		double* TD=SPG_TypeAlloc(NX*NY,double,"SPG_LinInitTransp");
		for(int iy=0;iy<NY;iy++)
		{
			for(int ix=0;ix<NX;ix++)
			{
				TD[iy+ix*NY]=D[ix+iy*NX];
			}
		}
		V_SWAP(double*,D,TD);
		V_SWAP(int,NX,NY);
		SPG_MemFree(TD);
	}
	return SPG_LinInit(P,D,NX,NY);
}

void SPG_CONV SPG_LinClose(SPG_LINTABLE& P)
{
	CHECK(P.LX==0,"SPG_LinClose",return);
	P.LY=0;
	SPG_MemFree(P.LX);
	SPG_ZeroStruct(P);
	return;
}

double SPG_CONV SPG_Lin(double x, SPG_LINTABLE& P)
{
	CHECK(P.N<2,"SPG_Lin",return 0);
	int n=1;
	for(;n<P.N-1;n++) { if(x<P.LX[n]) break; }
	double Dx=P.LX[n]-P.LX[n-1]; double Dy=P.LY[n]-P.LY[n-1]; double dx=x-P.LX[n-1];
	return P.LY[n-1] + dx * Dy/Dx;
}

double SPG_CONV SPG_LinGetSlope(SPG_LINTABLE& P)
{
	CHECK(P.N<2,"SPG_LinGetSlope",return 0);
	//P.LY = a * P.LX + b
	double x=0;		double xy=0;		double x2=0;		double y=0;
	for(int i=0;i<P.N;i++)
	{
		x+=P.LX[i];
		xy+=P.LX[i]*P.LY[i];
		x2+=P.LX[i]*P.LX[i];
		y+=P.LY[i];
	}
	x/=P.N;				xy/=P.N;				x2/=P.N;				y/=P.N;
	CHECK(x2-x*x<FLT_EPSILON*(x2+x*x),"SPG_LinGetSlope : null x span",return 0);
	double a = (xy - x*y) / (x2 - x*x);
	return a;
	// b = (y - a * x)
}

void SPG_CONV SPG_LinAddSlope(double S, SPG_LINTABLE& P)
{
	CHECK(P.N==0,"SPG_LinAddSlope",return)
	for(int i=0;i<P.N;i++)
	{
		P.LY[i]+=S*P.LX[i];
	}
	return;
}

int SPG_CONV SPG_PolyInit(SPG_LINPOLY& P, double* D, int NX, int NY)
{
	CHECK((P.A=D)==0,"SPG_PolyInit: not loaded",return 0);
	CHECK((NX!=1)&&(NY!=1),"SPG_PolyInit\nList of coefficients (1D line or column), zero order (offset) coeff is first, zero order coeff is last",SPG_MemFree(P.A);SPG_ZeroStruct(P);return 0);
	P.N=V_Max(NX,NY);
	return -1;
}

int SPG_CONV SPG_PolyInit(SPG_LINPOLY& P, char* Workdir, char* F)
{
	SPG_ZeroStruct(P);
	char Filename[MaxProgDir];SPG_ConcatPath(Filename,Workdir,F);

	int NX=0; int NY=0;
	double* D=Text_ReadDouble(Filename,NX,NY);
	CHECKTWO(D==0,"SPG_PolyInit",F,SPG_ZeroStruct(P);return 0);
	return SPG_PolyInit(P,D,NX,NY);
}

void SPG_CONV SPG_PolyClose(SPG_LINPOLY& P)
{
	CHECK(P.A==0,"SPG_PolyClose",return);
	SPG_MemFree(P.A);
	SPG_ZeroStruct(P);
	return;
}

double SPG_CONV SPG_Poly(double x, SPG_LINPOLY& P)
{
	CHECK(P.A==0,"SPG_Poly",return 0)
	double y=0;
	
	//for(int n=0;n<P.N;n++)	{	y=x*y+P.A[n];	} //highest order coeff is first, zero order (offset) coeff is last
	for(int n=P.N-1;n>=0;n--)	{	y=x*y+P.A[n];	} //zero order (offset) coeff is first, zero order coeff is last 

	return y;
}

double SPG_CONV SPG_PolyGetSlope(SPG_LINPOLY& P)
{
	return 0;
}

void SPG_CONV SPG_PolyAddSlope(double S, SPG_LINPOLY& P)
{
	CHECK(P.N<1,"SPG_PolyAddSlope",return);
	P.A[1]+=S;
	return;
}

//##############  INTERPOLATION OU POLYNOME  ###############

int SPG_CONV SPG_LinAutoInit(SPG_LINAUTO& P, char* Workdir, char* F)
{
	SPG_ZeroStruct(P);
	char Filename[MaxProgDir];SPG_ConcatPath(Filename,Workdir,F);

	int NX=0; int NY=0;
	double* D=Text_ReadDouble(Filename,NX,NY);
	CHECKTWO(D==0,"SPG_LinAutoInit",F,SPG_ZeroStruct(P);return 0);

	if((NX==1)||(NY==1))
	{
		int r = SPG_PolyInit(P.Lin.Poly,D,NX,NY);
		if(r) 
		{
			P.POLYF=SPG_Poly;
			P.POLYGS=SPG_PolyGetSlope;
			P.POLYAS=SPG_PolyAddSlope;
			P.POLYClose=SPG_PolyClose;
		}
		return r;
	}
	else
	{
		int r = SPG_LinInit(P.Lin.Table,D,NX,NY);
		if(r) 
		{
			P.TABLEF=SPG_Lin;
			P.TABLEGS=SPG_LinGetSlope;
			P.TABLEAS=SPG_LinAddSlope;
			P.TABLEClose=SPG_LinClose;
		}
		return r;
	}
}

void SPG_CONV SPG_LinAutoClose(SPG_LINAUTO& P)
{
	if(P.Close) P.Close(P.Lin);
	SPG_ZeroStruct(P);
	return;
}

double SPG_CONV SPG_LinAuto(double x, SPG_LINAUTO& P)
{
	if(P.F) return P.F(x,P.Lin);
	return 0;
}

double SPG_CONV SPG_LinAutoGetSlope(SPG_LINAUTO& P)
{
	if(P.GS) return P.GS(P.Lin);
	return 0;
}

void SPG_CONV SPG_LinAutoAddSlope(double x, SPG_LINAUTO& P)
{
	if(P.AS) return P.AS(x,P.Lin);
	return;
}

//##############  INTERPOLATION OU POLYNOME  ###############

//##############  SPLINES (non testé)  ###############

int SPG_CONV SPG_PolyTableInit(SPG_LINPOLYTABLE& P, char* Workdir, char* F)
{
	SPG_ZeroStruct(P);
	char Filename[MaxProgDir];SPG_ConcatPath(Filename,Workdir,F);

	P.SLNX=0; P.SLNY=0;
	P.A=Text_ReadDouble(Filename,P.SLNX,P.SLNY);
	CHECKTWO(P.A==0,"SPG_PolyTableInit: not loaded",F,return 0);
	CHECKTWO((P.SLNX<2)||(P.SLNY<1),"SPG_PolyTableInit\nxOrigin followed by list of coefficients, highest order coeff is first, zero order (offset) coeff is last, Tabulation separated",F,SPG_MemFree(P.A);SPG_ZeroStruct(P);return 0);

	P.Order=P.SLNX-1;
	P.N=P.SLNY;

	P.xmin=SPG_POLYLINE(P,0).x;
	P.xmax=SPG_POLYLINE(P,P.N-1).x;
	P.xcoeff=P.N/(P.xmax-P.xmin);

	return -1;
}

int SPG_CONV SPG_PolyTableClose(SPG_LINPOLYTABLE& P)
{
	CHECK(P.A==0,"SPG_PolyClose",return 0);
	SPG_MemFree(P.A);
	SPG_ZeroStruct(P);
	return -1;
}

double SPG_CONV SPG_PolyTable(double x, SPG_LINPOLYTABLE& P)
{
	CHECK(P.A==0,"SPG_PolyTable",return 0)

	int nguess=V_Floor((x-P.xmin)*P.xcoeff);
	nguess=V_Sature(nguess,0,P.N-1);
	int Step=V_Max(P.N/8,1);

	//autre solution partir d'un pas de 1 et le doubler à chaque fois qu'on se déplace dans la même direction qu'au coup d'avant

	//for(int i=0;i<P.N;i++) //si on n'aime pas le risque, borner les iterations
	while(1)
	{
		if( (nguess>Step) && (x<SPG_POLYLINE(P,nguess).x) )
		{
			nguess-=Step;
		}
		else if( (nguess<P.N-Step) && (x>=SPG_POLYLINE(P,nguess+Step).x) )
		{
			nguess+=Step;
		}
		else if(Step>1)
		{
			Step>>=1;
		}
		else break;
	}

	SPG_LINPOLYLINE& L=SPG_POLYLINE(P,nguess);
	x-=L.x;

	double y=0;
	for(int n=0;n<P.Order;n++)
	{
		y=x*y+L.a[n];
	}
	return y;
}

//##############  SPLINES (non testé)  ###############
