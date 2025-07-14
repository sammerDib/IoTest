
#include "SPG_General.h"

#ifdef SPG_General_USEPARAFIT

#include "SPG_MinIncludes.h"
#include "SPG_ParaFit.h"

#define Da(i) D[(Pos i)*Pitch]
#define Dsa(i) D[(V_Sature((Pos i),PosMin,PosMax))*Pitch]

int SPG_CONV SPG_ParaFit3to13Safe(float* D, int Pitch, int PosMin, int PosMax, int Pos, float* a, float* b, float* c)
{
	SPG_ParaFit3( Dsa(-1),Dsa(+0),Dsa(+1),                                                                                a[0],b[0],c[0]);
	SPG_ParaFit5( Dsa(-2),Dsa(-1),Dsa(+0),Dsa(+1),Dsa(+2),                                                                a[1],b[1],c[1]);
	SPG_ParaFit7( Dsa(-3),Dsa(-2),Dsa(-1),Dsa(+0),Dsa(+1),Dsa(+2),Dsa(+3),                                                a[2],b[2],c[2]);
	SPG_ParaFit9( Dsa(-4),Dsa(-3),Dsa(-2),Dsa(-1),Dsa(+0),Dsa(+1),Dsa(+2),Dsa(+3),Dsa(+4),                                a[3],b[3],c[3]);
	SPG_ParaFit11(Dsa(-5),Dsa(-4),Dsa(-3),Dsa(-2),Dsa(-1),Dsa(+0),Dsa(+1),Dsa(+2),Dsa(+3),Dsa(+4),Dsa(+5),                a[4],b[4],c[4]);
	SPG_ParaFit13(Dsa(-6),Dsa(-5),Dsa(-4),Dsa(-3),Dsa(-2),Dsa(-1),Dsa(+0),Dsa(+1),Dsa(+2),Dsa(+3),Dsa(+4),Dsa(+5),Dsa(+6),a[5],b[5],c[5]);
	return (SPG_ParaFitMax-1)/2;
}

int SPG_CONV SPG_ParaFit3to13(float* D, int Pitch, int Pos, float* a, float* b, float* c)
{//a doit contenir (SPG_ParaFitMax-1)/2 elements
	SPG_ParaFit3( Da(-1),Da(+0),Da(+1),                                                                      a[0],b[0],c[0]);
	SPG_ParaFit5( Da(-2),Da(-1),Da(+0),Da(+1),Da(+2),                                                        a[1],b[1],c[1]);
	SPG_ParaFit7( Da(-3),Da(-2),Da(-1),Da(+0),Da(+1),Da(+2),Da(+3),                                          a[2],b[2],c[2]);
	SPG_ParaFit9( Da(-4),Da(-3),Da(-2),Da(-1),Da(+0),Da(+1),Da(+2),Da(+3),Da(+4),                            a[3],b[3],c[3]);
	SPG_ParaFit11(Da(-5),Da(-4),Da(-3),Da(-2),Da(-1),Da(+0),Da(+1),Da(+2),Da(+3),Da(+4),Da(+5),              a[4],b[4],c[4]);
	SPG_ParaFit13(Da(-6),Da(-5),Da(-4),Da(-3),Da(-2),Da(-1),Da(+0),Da(+1),Da(+2),Da(+3),Da(+4),Da(+5),Da(+6),a[5],b[5],c[5]);
	return (SPG_ParaFitMax-1)/2;
}

int SPG_CONV SPG_ParaFit3to13Scan(float* D, int Pitch, int PosMin, int PosMax, int Pos0, int ScanRange, int* x, float* a, float* b, float* c)
{
	for(int Pos=Pos0-ScanRange;Pos<=Pos0+ScanRange;Pos++)
	{
		x[0]=Pos; SPG_ParaFit3( Dsa(-1),Dsa(+0),Dsa(+1),                                                                                a[0],b[0],c[0]);
		x[1]=Pos; SPG_ParaFit5( Dsa(-2),Dsa(-1),Dsa(+0),Dsa(+1),Dsa(+2),                                                                a[1],b[1],c[1]);
		x[2]=Pos; SPG_ParaFit7( Dsa(-3),Dsa(-2),Dsa(-1),Dsa(+0),Dsa(+1),Dsa(+2),Dsa(+3),                                                a[2],b[2],c[2]);
		x[3]=Pos; SPG_ParaFit9( Dsa(-4),Dsa(-3),Dsa(-2),Dsa(-1),Dsa(+0),Dsa(+1),Dsa(+2),Dsa(+3),Dsa(+4),                                a[3],b[3],c[3]);
		x[4]=Pos; SPG_ParaFit11(Dsa(-5),Dsa(-4),Dsa(-3),Dsa(-2),Dsa(-1),Dsa(+0),Dsa(+1),Dsa(+2),Dsa(+3),Dsa(+4),Dsa(+5),                a[4],b[4],c[4]);
		x[5]=Pos; SPG_ParaFit13(Dsa(-6),Dsa(-5),Dsa(-4),Dsa(-3),Dsa(-2),Dsa(-1),Dsa(+0),Dsa(+1),Dsa(+2),Dsa(+3),Dsa(+4),Dsa(+5),Dsa(+6),a[5],b[5],c[5]);
		a+=(SPG_ParaFitMax-1)/2;
		b+=(SPG_ParaFitMax-1)/2;
		c+=(SPG_ParaFitMax-1)/2;
		x+=(SPG_ParaFitMax-1)/2;
	}
	return ((SPG_ParaFitMax-1)/2)*(2*ScanRange+1);
}

// UtU B = Ut Y
//Fit par une droite
FIT_COEFF lsCalculate(float* D,int len)
{
	int i;
	double XtX[2][2];
	double Y[2];

	double sum=0;
	double prod=0;
	for(i=0;i<len;i++)
	{
		sum+=i;
		prod+=i*i;
	}

	XtX[0][0]=len;
	XtX[0][1]=XtX[1][0]=sum;
	XtX[1][1]=prod;

	sum=0;
	prod=0;
	for(i=0;i<len;i++)
	{
		sum+=D[i];
		prod+=i*D[i];
	}
	Y[0]=sum;
	Y[1]=prod;

	double f=XtX[1][0]/XtX[0][0];
	XtX[1][0]-=f*XtX[0][0];
	XtX[1][1]-=f*XtX[0][1];
	Y[1]-=f*Y[0];

	XtX[1][0]=0;//

	FIT_COEFF r;
	r.cx=Y[1]/XtX[1][1];
	r.c0=(Y[0]-XtX[0][1]*r.cx)/XtX[0][0];
	r.cx2=0;
	return r;
}

//Fit par une droite (masque)
FIT_COEFF lsCalculateM(float* D, BYTE* M, int len)
{
	int i, N;
	double XtX[2][2];
	double Y[2];

	double sum=0;
	double prod=0;
	N = 0;
	for(i=0;i<len;i++)
	{
		if(M[i])
		{
			sum+=i;
			prod+=i*i;
			N++;
		}
	}

	if(N==0)
	{
		FIT_COEFF r;
		r.cx=0;
		r.c0=0;
		r.cx2=0;
		return r;
	}

	XtX[0][0]=N;
	XtX[0][1]=XtX[1][0]=sum;
	XtX[1][1]=prod;

	sum=0;
	prod=0;
	for(i=0;i<len;i++)
	{
		if(M[i])
		{
			sum+=D[i];
			prod+=i*D[i];
		}
	}
	Y[0]=sum;
	Y[1]=prod;

	double f=XtX[1][0]/XtX[0][0];
	XtX[1][0]-=f*XtX[0][0];
	XtX[1][1]-=f*XtX[0][1];
	Y[1]-=f*Y[0];

	XtX[1][0]=0;//

	FIT_COEFF r;
	r.cx=Y[1]/XtX[1][1];
	r.c0=(Y[0]-XtX[0][1]*r.cx)/XtX[0][0];
	r.cx2=0;
	return r;
}

// UtU B = Ut Z
//Fit par un plan
FIT_COEFF_2D lsCalculate2d(float* D,int sx,int sy)
{
	CHECK((sx<=0)||(sy<=0),"lsCalculate2d",	{FIT_COEFF_2D r;r.c0=0;r.cx=0;r.cy=0;r.cx2=0;r.cy2=0;return r;};);
	double UtU[3][3];
	double Z[3];

	DWORD N=0;
	double sumX=0;
	double sumX2=0;
	double sumY=0;
	double sumY2=0;
	double sumXY=0;
	DWORD x,y;

	for(y=0;y<sy;y++)
		for(x=0;x<sx;x++)
		{
			N++;
			sumX+=x;
			sumY+=y;
			sumX2+=x*x;
			sumY2+=y*y;
			sumXY+=x*y;
		}

	UtU[0][0]=N;
	UtU[0][1]=UtU[1][0]=sumX;
	UtU[0][2]=UtU[2][0]=sumY;
	UtU[1][2]=UtU[2][1]=sumXY;
	UtU[1][1]=sumX2;
	UtU[2][2]=sumY2;

	double sumZ=0;
	double sumXZ=0;
	double sumYZ=0;
	for(y=0;y<sy;y++)
		for(x=0;x<sx;x++)
		{
			double z=D[y*sx+x];
			sumZ+=z;
			sumXZ+=x*z;
			sumYZ+=y*z;
		}

	Z[0]=sumZ;
	Z[1]=sumXZ;
	Z[2]=sumYZ;

	double f;

	//Gauss
	//en descendant ...
	f=UtU[1][0]/UtU[0][0];
	UtU[1][0]-=f*UtU[0][0];
	UtU[1][1]-=f*UtU[0][1];
	UtU[1][2]-=f*UtU[0][2];
	Z[1]-=f*Z[0];
	UtU[1][0]=0;

	f=UtU[2][0]/UtU[0][0];
	UtU[2][0]-=f*UtU[0][0];
	UtU[2][1]-=f*UtU[0][1];
	UtU[2][2]-=f*UtU[0][2];
	Z[2]-=f*Z[0];
	UtU[2][0]=0;

	f=UtU[2][1]/UtU[1][1];
	UtU[2][0]-=f*UtU[1][0]; //superflu
	UtU[2][1]-=f*UtU[1][1];
	UtU[2][2]-=f*UtU[1][2];
	Z[2]-=f*Z[1];
	UtU[2][1]=0;

	//en remontant ...
	f=UtU[1][2]/UtU[2][2];
	UtU[1][0]-=f*UtU[2][0]; //superflu
	UtU[1][1]-=f*UtU[2][1];
	UtU[1][2]-=f*UtU[2][2];
	Z[1]-=f*Z[2];
	UtU[1][2]=0;

	f=UtU[0][2]/UtU[2][2];
	UtU[0][0]-=f*UtU[2][0]; //superflu
	UtU[0][1]-=f*UtU[2][1];
	UtU[0][2]-=f*UtU[2][2];
	Z[0]-=f*Z[2];
	UtU[0][2]=0;

	f=UtU[0][1]/UtU[1][1];
	UtU[0][0]-=f*UtU[1][0]; //superflu
	UtU[0][1]-=f*UtU[1][1];
	UtU[0][2]-=f*UtU[1][2]; //superflu
	Z[0]-=f*Z[1];
	UtU[0][1]=0;
	//=> matrice diagonale

	FIT_COEFF_2D r;
	r.c0=Z[0]/UtU[0][0];
	r.cx=Z[1]/UtU[1][1];
	r.cy=Z[2]/UtU[2][2];
	r.cx2=0;
	r.cy2=0;
	return r;
}

// UtU B = Ut Z
//Fit par un plan
FIT_COEFF_2D lsCalculate2dM(float* D,BYTE* M, int sx,int sy)
{
	CHECK((sx<=0)||(sy<=0),"lsCalculate2d",	{FIT_COEFF_2D r;r.c0=0;r.cx=0;r.cy=0;r.cx2=0;r.cy2=0;return r;};);
	double UtU[3][3];
	double Z[3];

	DWORD N=0;
	double sumX=0;
	double sumX2=0;
	double sumY=0;
	double sumY2=0;
	double sumXY=0;
	DWORD x,y;

	for(y=0;y<sy;y++)
		for(x=0;x<sx;x++)
		{
			if(M[x+sx*y])
			{
				N++;
				sumX+=x;
				sumY+=y;
				sumX2+=x*x;
				sumY2+=y*y;
				sumXY+=x*y;
			}
		}
	if(N==0)
	{
		FIT_COEFF_2D r;
		r.c0=0;
		r.cx=0;
		r.cy=0;
		r.cx2=0;
		r.cy2=0;
		return r;
	}

	UtU[0][0]=N;
	UtU[0][1]=UtU[1][0]=sumX;
	UtU[0][2]=UtU[2][0]=sumY;
	UtU[1][2]=UtU[2][1]=sumXY;
	UtU[1][1]=sumX2;
	UtU[2][2]=sumY2;

	double sumZ=0;
	double sumXZ=0;
	double sumYZ=0;
	for(y=0;y<sy;y++)
		for(x=0;x<sx;x++)
		{
			if(M[x+sx*y])
			{
				double z=D[y*sx+x];
				sumZ+=z;
				sumXZ+=x*z;
				sumYZ+=y*z;
			}
		}

	Z[0]=sumZ;
	Z[1]=sumXZ;
	Z[2]=sumYZ;

	double f;

	//Gauss
	//en descendant ...
	f=UtU[1][0]/UtU[0][0];
	UtU[1][0]-=f*UtU[0][0];
	UtU[1][1]-=f*UtU[0][1];
	UtU[1][2]-=f*UtU[0][2];
	Z[1]-=f*Z[0];
	UtU[1][0]=0;

	f=UtU[2][0]/UtU[0][0];
	UtU[2][0]-=f*UtU[0][0];
	UtU[2][1]-=f*UtU[0][1];
	UtU[2][2]-=f*UtU[0][2];
	Z[2]-=f*Z[0];
	UtU[2][0]=0;

	f=UtU[2][1]/UtU[1][1];
	UtU[2][0]-=f*UtU[1][0]; //superflu
	UtU[2][1]-=f*UtU[1][1];
	UtU[2][2]-=f*UtU[1][2];
	Z[2]-=f*Z[1];
	UtU[2][1]=0;

	//en remontant ...
	f=UtU[1][2]/UtU[2][2];
	UtU[1][0]-=f*UtU[2][0]; //superflu
	UtU[1][1]-=f*UtU[2][1];
	UtU[1][2]-=f*UtU[2][2];
	Z[1]-=f*Z[2];
	UtU[1][2]=0;

	f=UtU[0][2]/UtU[2][2];
	UtU[0][0]-=f*UtU[2][0]; //superflu
	UtU[0][1]-=f*UtU[2][1];
	UtU[0][2]-=f*UtU[2][2];
	Z[0]-=f*Z[2];
	UtU[0][2]=0;

	f=UtU[0][1]/UtU[1][1];
	UtU[0][0]-=f*UtU[1][0]; //superflu
	UtU[0][1]-=f*UtU[1][1];
	UtU[0][2]-=f*UtU[1][2]; //superflu
	Z[0]-=f*Z[1];
	UtU[0][1]=0;
	//=> matrice diagonale

	FIT_COEFF_2D r;
	r.c0=Z[0]/UtU[0][0];
	r.cx=Z[1]/UtU[1][1];
	r.cy=Z[2]/UtU[2][2];
	r.cx2=0;
	r.cy2=0;
	return r;
}

// UtU B = Ut Y
//fit par une parabole (1d)
FIT_COEFF lsCalculate2float(float* D,int sx)
{
	double UtU[3][3];
	double Z[3];

	DWORD N=0;
	double sumX=0;
	double sumX2=0;
	double sumY=0;
	double sumY2=0;
	double sumXY=0;
	DWORD xc;
	double x,y;
//	double fFact=1/double(sx*sx);

	for(xc=0;xc<sx;xc++)
	{
		x=xc/double(sx);
		y=x*x;
		N++;
		sumX+=x;
		sumY+=y;
		sumX2+=x*x;
		sumY2+=y*y;
		sumXY+=x*y;
	}

	UtU[0][0]=N;
	UtU[0][1]=UtU[1][0]=sumX;
	UtU[0][2]=UtU[2][0]=sumY;
	UtU[1][2]=UtU[2][1]=sumXY;
	UtU[1][1]=sumX2;
	UtU[2][2]=sumY2;

	double sumZ=0;
	double sumXZ=0;
	double sumYZ=0;
	for(xc=0;xc<sx;xc++)
	{
		x=xc/double(sx);
		y=x*x;
		double z=D[xc];
		sumZ+=z;
		sumXZ+=x*z;
		sumYZ+=y*z;
	}

	Z[0]=sumZ;
	Z[1]=sumXZ;
	Z[2]=sumYZ;

	double f;

	//Gauss
	//en descendant ...
	f=UtU[1][0]/UtU[0][0];
	UtU[1][0]-=f*UtU[0][0];
	UtU[1][1]-=f*UtU[0][1];
	UtU[1][2]-=f*UtU[0][2];
	Z[1]-=f*Z[0];
	UtU[1][0]=0;

	f=UtU[2][0]/UtU[0][0];
	UtU[2][0]-=f*UtU[0][0];
	UtU[2][1]-=f*UtU[0][1];
	UtU[2][2]-=f*UtU[0][2];
	Z[2]-=f*Z[0];
	UtU[2][0]=0;

	f=UtU[2][1]/UtU[1][1];
	UtU[2][0]-=f*UtU[1][0]; //superflu
	UtU[2][1]-=f*UtU[1][1];
	UtU[2][2]-=f*UtU[1][2];
	Z[2]-=f*Z[1];
	UtU[2][1]=0;

	//en remontant ...
	f=UtU[1][2]/UtU[2][2];
	UtU[1][0]-=f*UtU[2][0]; //superflu
	UtU[1][1]-=f*UtU[2][1];
	UtU[1][2]-=f*UtU[2][2];
	Z[1]-=f*Z[2];
	UtU[1][2]=0;

	f=UtU[0][2]/UtU[2][2];
	UtU[0][0]-=f*UtU[2][0]; //superflu
	UtU[0][1]-=f*UtU[2][1];
	UtU[0][2]-=f*UtU[2][2];
	Z[0]-=f*Z[2];
	UtU[0][2]=0;

	f=UtU[0][1]/UtU[1][1];
	UtU[0][0]-=f*UtU[1][0]; //superflu
	UtU[0][1]-=f*UtU[1][1];
	UtU[0][2]-=f*UtU[1][2]; //superflu
	Z[0]-=f*Z[1];
	UtU[0][1]=0;
	//=> matrice diagonale

	FIT_COEFF r;
	r.c0=Z[0]/UtU[0][0];
	r.cx=Z[1]/UtU[1][1];
	r.cx/=sx;
	r.cx2=Z[2]/UtU[2][2];
	r.cx2/=sx*sx;
	return r;
}


// UtU B = Ut Y
//fit par une parabole (1d)
FIT_COEFF lsCalculate2float(float* D1, int N1, float* D2, int N2, int Step)
{
	double UtU[3][3];
	double Z[3];

	DWORD N=0;
	double sumX=0;
	double sumX2=0;
	double sumY=0;
	double sumY2=0;
	double sumXY=0;
	DWORD xc;
	double x,y;
//	double fFact=1/double(sx*sx);

	int sx=(N1+N2)/Step;

	for(xc=0;xc<sx;xc++)
	{
		x=xc/double(sx);
		y=x*x;
		N++;
		sumX+=x;
		sumY+=y;
		sumX2+=x*x;
		sumY2+=y*y;
		sumXY+=x*y;
	}

	UtU[0][0]=N;
	UtU[0][1]=UtU[1][0]=sumX;
	UtU[0][2]=UtU[2][0]=sumY;
	UtU[1][2]=UtU[2][1]=sumXY;
	UtU[1][1]=sumX2;
	UtU[2][2]=sumY2;

	double sumZ=0;
	double sumXZ=0;
	double sumYZ=0;

	int Pos=0;

	for(xc=0;xc<sx;xc++)
	{
		x=xc/double(sx);
		y=x*x;
		double z=D1[Pos];
		sumZ+=z;
		sumXZ+=x*z;
		sumYZ+=y*z;
		Pos+=Step; if(Pos>=N1) {Pos-=N1;xc++;break;}
	}
	for(;xc<sx;xc++)
	{
		x=xc/double(sx);
		y=x*x;
		double z=D2[Pos];
		Pos+=Step;
		sumZ+=z;
		sumXZ+=x*z;
		sumYZ+=y*z;
	}

	Z[0]=sumZ;
	Z[1]=sumXZ;
	Z[2]=sumYZ;

	double f;

	//Gauss
	//en descendant ...
	f=UtU[1][0]/UtU[0][0];
	UtU[1][0]-=f*UtU[0][0];
	UtU[1][1]-=f*UtU[0][1];
	UtU[1][2]-=f*UtU[0][2];
	Z[1]-=f*Z[0];
	UtU[1][0]=0;

	f=UtU[2][0]/UtU[0][0];
	UtU[2][0]-=f*UtU[0][0];
	UtU[2][1]-=f*UtU[0][1];
	UtU[2][2]-=f*UtU[0][2];
	Z[2]-=f*Z[0];
	UtU[2][0]=0;

	f=UtU[2][1]/UtU[1][1];
	UtU[2][0]-=f*UtU[1][0]; //superflu
	UtU[2][1]-=f*UtU[1][1];
	UtU[2][2]-=f*UtU[1][2];
	Z[2]-=f*Z[1];
	UtU[2][1]=0;

	//en remontant ...
	f=UtU[1][2]/UtU[2][2];
	UtU[1][0]-=f*UtU[2][0]; //superflu
	UtU[1][1]-=f*UtU[2][1];
	UtU[1][2]-=f*UtU[2][2];
	Z[1]-=f*Z[2];
	UtU[1][2]=0;

	f=UtU[0][2]/UtU[2][2];
	UtU[0][0]-=f*UtU[2][0]; //superflu
	UtU[0][1]-=f*UtU[2][1];
	UtU[0][2]-=f*UtU[2][2];
	Z[0]-=f*Z[2];
	UtU[0][2]=0;

	f=UtU[0][1]/UtU[1][1];
	UtU[0][0]-=f*UtU[1][0]; //superflu
	UtU[0][1]-=f*UtU[1][1];
	UtU[0][2]-=f*UtU[1][2]; //superflu
	Z[0]-=f*Z[1];
	UtU[0][1]=0;
	//=> matrice diagonale

	FIT_COEFF r;
	r.c0=Z[0]/UtU[0][0];
	r.cx=Z[1]/UtU[1][1];
	r.cx/=sx;
	r.cx2=Z[2]/UtU[2][2];
	r.cx2/=sx*sx;
	return r;
}

// UtU B = Ut Y
//fit par une parabole (1d)
FIT_COEFF lsCalculate2double(double* D,int sx)
{
	double UtU[3][3];
	double Z[3];

	DWORD N=0;
	double sumX=0;
	double sumX2=0;
	double sumY=0;
	double sumY2=0;
	double sumXY=0;
	DWORD xc;
	double x,y;
//	double fFact=1/double(sx*sx);

	for(xc=0;xc<sx;xc++)
	{
		x=xc/double(sx);
		y=x*x;
		N++;
		sumX+=x;
		sumY+=y;
		sumX2+=x*x;
		sumY2+=y*y;
		sumXY+=x*y;
	}

	UtU[0][0]=N;
	UtU[0][1]=UtU[1][0]=sumX;
	UtU[0][2]=UtU[2][0]=sumY;
	UtU[1][2]=UtU[2][1]=sumXY;
	UtU[1][1]=sumX2;
	UtU[2][2]=sumY2;

	double sumZ=0;
	double sumXZ=0;
	double sumYZ=0;
	for(xc=0;xc<sx;xc++)
	{
		x=xc/double(sx);
		y=x*x;
		double z=D[xc];
		sumZ+=z;
		sumXZ+=x*z;
		sumYZ+=y*z;
	}

	Z[0]=sumZ;
	Z[1]=sumXZ;
	Z[2]=sumYZ;

	double f;

	//Gauss
	//en descendant ...
	f=UtU[1][0]/UtU[0][0];
	UtU[1][0]-=f*UtU[0][0];
	UtU[1][1]-=f*UtU[0][1];
	UtU[1][2]-=f*UtU[0][2];
	Z[1]-=f*Z[0];
	UtU[1][0]=0;

	f=UtU[2][0]/UtU[0][0];
	UtU[2][0]-=f*UtU[0][0];
	UtU[2][1]-=f*UtU[0][1];
	UtU[2][2]-=f*UtU[0][2];
	Z[2]-=f*Z[0];
	UtU[2][0]=0;

	f=UtU[2][1]/UtU[1][1];
	UtU[2][0]-=f*UtU[1][0]; //superflu
	UtU[2][1]-=f*UtU[1][1];
	UtU[2][2]-=f*UtU[1][2];
	Z[2]-=f*Z[1];
	UtU[2][1]=0;

	//en remontant ...
	f=UtU[1][2]/UtU[2][2];
	UtU[1][0]-=f*UtU[2][0]; //superflu
	UtU[1][1]-=f*UtU[2][1];
	UtU[1][2]-=f*UtU[2][2];
	Z[1]-=f*Z[2];
	UtU[1][2]=0;

	f=UtU[0][2]/UtU[2][2];
	UtU[0][0]-=f*UtU[2][0]; //superflu
	UtU[0][1]-=f*UtU[2][1];
	UtU[0][2]-=f*UtU[2][2];
	Z[0]-=f*Z[2];
	UtU[0][2]=0;

	f=UtU[0][1]/UtU[1][1];
	UtU[0][0]-=f*UtU[1][0]; //superflu
	UtU[0][1]-=f*UtU[1][1];
	UtU[0][2]-=f*UtU[1][2]; //superflu
	Z[0]-=f*Z[1];
	UtU[0][1]=0;
	//=> matrice diagonale

	FIT_COEFF r;
	r.c0=Z[0]/UtU[0][0];
	r.cx=Z[1]/UtU[1][1];
	r.cx/=sx;
	r.cx2=Z[2]/UtU[2][2];
	r.cx2/=sx*sx;
	return r;
}

// UtU B = Ut Y
//fit par une parabole (1d)
FIT_COEFF lsCalculate2short(short* D, int InterleaveStep, int sx)
{
	double UtU[3][3];
	double Z[3];

	DWORD N=0;
	double sumX=0;
	double sumX2=0;
	double sumY=0;
	double sumY2=0;
	double sumXY=0;
	DWORD xc;
	double x,y;
//	double fFact=1/double(sx*sx);

	for(xc=0;xc<sx;xc++)
	{
		x=xc/double(sx);
		y=x*x;
		N++;
		sumX+=x;
		sumY+=y;
		sumX2+=x*x;
		sumY2+=y*y;
		sumXY+=x*y;
	}

	UtU[0][0]=N;
	UtU[0][1]=UtU[1][0]=sumX;
	UtU[0][2]=UtU[2][0]=sumY;
	UtU[1][2]=UtU[2][1]=sumXY;
	UtU[1][1]=sumX2;
	UtU[2][2]=sumY2;

	double sumZ=0;
	double sumXZ=0;
	double sumYZ=0;
	for(xc=0;xc<sx;xc++)
	{
		x=xc/double(sx);
		y=x*x;
		double z=D[xc*InterleaveStep];
		sumZ+=z;
		sumXZ+=x*z;
		sumYZ+=y*z;
	}

	Z[0]=sumZ;
	Z[1]=sumXZ;
	Z[2]=sumYZ;

	double f;

	//Gauss
	//en descendant ...
	f=UtU[1][0]/UtU[0][0];
	UtU[1][0]-=f*UtU[0][0];
	UtU[1][1]-=f*UtU[0][1];
	UtU[1][2]-=f*UtU[0][2];
	Z[1]-=f*Z[0];
	UtU[1][0]=0;

	f=UtU[2][0]/UtU[0][0];
	UtU[2][0]-=f*UtU[0][0];
	UtU[2][1]-=f*UtU[0][1];
	UtU[2][2]-=f*UtU[0][2];
	Z[2]-=f*Z[0];
	UtU[2][0]=0;

	f=UtU[2][1]/UtU[1][1];
	UtU[2][0]-=f*UtU[1][0]; //superflu
	UtU[2][1]-=f*UtU[1][1];
	UtU[2][2]-=f*UtU[1][2];
	Z[2]-=f*Z[1];
	UtU[2][1]=0;

	//en remontant ...
	f=UtU[1][2]/UtU[2][2];
	UtU[1][0]-=f*UtU[2][0]; //superflu
	UtU[1][1]-=f*UtU[2][1];
	UtU[1][2]-=f*UtU[2][2];
	Z[1]-=f*Z[2];
	UtU[1][2]=0;

	f=UtU[0][2]/UtU[2][2];
	UtU[0][0]-=f*UtU[2][0]; //superflu
	UtU[0][1]-=f*UtU[2][1];
	UtU[0][2]-=f*UtU[2][2];
	Z[0]-=f*Z[2];
	UtU[0][2]=0;

	f=UtU[0][1]/UtU[1][1];
	UtU[0][0]-=f*UtU[1][0]; //superflu
	UtU[0][1]-=f*UtU[1][1];
	UtU[0][2]-=f*UtU[1][2]; //superflu
	Z[0]-=f*Z[1];
	UtU[0][1]=0;
	//=> matrice diagonale

	FIT_COEFF r;
	r.c0=Z[0]/UtU[0][0];
	r.cx=Z[1]/UtU[1][1];
	r.cx/=sx;
	r.cx2=Z[2]/UtU[2][2];
	r.cx2/=sx*sx;
	return r;
}

// UtU B = Ut Y
//fit par une parabole (1d)
FIT_COEFF lsCalculate2short(short* D1, int N1, short* D2, int N2, int Step)
{
	double UtU[3][3];
	double Z[3];

	DWORD N=0;
	double sumX=0;
	double sumX2=0;
	double sumY=0;
	double sumY2=0;
	double sumXY=0;
	DWORD xc;
	double x,y;
//	double fFact=1/double(sx*sx);

	int sx=(N1+N2)/Step;

	for(xc=0;xc<sx;xc++)
	{
		x=xc/double(sx);
		y=x*x;
		N++;
		sumX+=x;
		sumY+=y;
		sumX2+=x*x;
		sumY2+=y*y;
		sumXY+=x*y;
	}

	UtU[0][0]=N;
	UtU[0][1]=UtU[1][0]=sumX;
	UtU[0][2]=UtU[2][0]=sumY;
	UtU[1][2]=UtU[2][1]=sumXY;
	UtU[1][1]=sumX2;
	UtU[2][2]=sumY2;

	double sumZ=0;
	double sumXZ=0;
	double sumYZ=0;

	int Pos=0;

	for(xc=0;xc<sx;xc++)
	{
		x=xc/double(sx);
		y=x*x;
		double z=D1[Pos];
		sumZ+=z;
		sumXZ+=x*z;
		sumYZ+=y*z;
		Pos+=Step; if(Pos>=N1) {Pos-=N1;xc++;break;}
	}
	for(;xc<sx;xc++)
	{
		x=xc/double(sx);
		y=x*x;
		double z=D2[Pos];
		Pos+=Step;
		// *D2/=2;
		sumZ+=z;
		sumXZ+=x*z;
		sumYZ+=y*z;
	}

	Z[0]=sumZ;
	Z[1]=sumXZ;
	Z[2]=sumYZ;

	double f;

	//Gauss
	//en descendant ...
	f=UtU[1][0]/UtU[0][0];
	UtU[1][0]-=f*UtU[0][0];
	UtU[1][1]-=f*UtU[0][1];
	UtU[1][2]-=f*UtU[0][2];
	Z[1]-=f*Z[0];
	UtU[1][0]=0;

	f=UtU[2][0]/UtU[0][0];
	UtU[2][0]-=f*UtU[0][0];
	UtU[2][1]-=f*UtU[0][1];
	UtU[2][2]-=f*UtU[0][2];
	Z[2]-=f*Z[0];
	UtU[2][0]=0;

	f=UtU[2][1]/UtU[1][1];
	UtU[2][0]-=f*UtU[1][0]; //superflu
	UtU[2][1]-=f*UtU[1][1];
	UtU[2][2]-=f*UtU[1][2];
	Z[2]-=f*Z[1];
	UtU[2][1]=0;

	//en remontant ...
	f=UtU[1][2]/UtU[2][2];
	UtU[1][0]-=f*UtU[2][0]; //superflu
	UtU[1][1]-=f*UtU[2][1];
	UtU[1][2]-=f*UtU[2][2];
	Z[1]-=f*Z[2];
	UtU[1][2]=0;

	f=UtU[0][2]/UtU[2][2];
	UtU[0][0]-=f*UtU[2][0]; //superflu
	UtU[0][1]-=f*UtU[2][1];
	UtU[0][2]-=f*UtU[2][2];
	Z[0]-=f*Z[2];
	UtU[0][2]=0;

	f=UtU[0][1]/UtU[1][1];
	UtU[0][0]-=f*UtU[1][0]; //superflu
	UtU[0][1]-=f*UtU[1][1];
	UtU[0][2]-=f*UtU[1][2]; //superflu
	Z[0]-=f*Z[1];
	UtU[0][1]=0;
	//=> matrice diagonale

	FIT_COEFF r;
	r.c0=Z[0]/UtU[0][0];
	r.cx=Z[1]/UtU[1][1];
	r.cx/=sx;
	r.cx2=Z[2]/UtU[2][2];
	r.cx2/=sx*sx;
	return r;
}

//fit par une parabole (1d) avec masque
FIT_COEFF lsCalculate2M(float* D, BYTE* M, int sx)
{
	double UtU[3][3];
	double Z[3];

	DWORD N=0;
	double sumX=0;
	double sumX2=0;
	double sumY=0;
	double sumY2=0;
	double sumXY=0;
	DWORD xc;
	double x,y;
//	double fFact=1/double(sx*sx);

	for(xc=0;xc<sx;xc++)
	{
		if(M[xc])
		{
			x=xc/double(sx);
			y=x*x;
			N++;
			sumX+=x;
			sumY+=y;
			sumX2+=x*x;
			sumY2+=y*y;
			sumXY+=x*y;
		}
	}

	if(N==0)
	{
		FIT_COEFF r;
		r.c0=0;
		r.cx=0;
		r.cx2=0;
		return r;
	}

	UtU[0][0]=N;
	UtU[0][1]=UtU[1][0]=sumX;
	UtU[0][2]=UtU[2][0]=sumY;
	UtU[1][2]=UtU[2][1]=sumXY;
	UtU[1][1]=sumX2;
	UtU[2][2]=sumY2;

	double sumZ=0;
	double sumXZ=0;
	double sumYZ=0;
	for(xc=0;xc<sx;xc++)
	{
		if(M[xc])
		{
			x=xc/double(sx);
			y=x*x;
			double z=D[xc];
			sumZ+=z;
			sumXZ+=x*z;
			sumYZ+=y*z;
		}
	}

	Z[0]=sumZ;
	Z[1]=sumXZ;
	Z[2]=sumYZ;

	double f;

	//Gauss
	//en descendant ...
	f=UtU[1][0]/UtU[0][0];
	UtU[1][0]-=f*UtU[0][0];
	UtU[1][1]-=f*UtU[0][1];
	UtU[1][2]-=f*UtU[0][2];
	Z[1]-=f*Z[0];
	UtU[1][0]=0;

	f=UtU[2][0]/UtU[0][0];
	UtU[2][0]-=f*UtU[0][0];
	UtU[2][1]-=f*UtU[0][1];
	UtU[2][2]-=f*UtU[0][2];
	Z[2]-=f*Z[0];
	UtU[2][0]=0;

	f=UtU[2][1]/UtU[1][1];
	UtU[2][0]-=f*UtU[1][0]; //superflu
	UtU[2][1]-=f*UtU[1][1];
	UtU[2][2]-=f*UtU[1][2];
	Z[2]-=f*Z[1];
	UtU[2][1]=0;

	//en remontant ...
	f=UtU[1][2]/UtU[2][2];
	UtU[1][0]-=f*UtU[2][0]; //superflu
	UtU[1][1]-=f*UtU[2][1];
	UtU[1][2]-=f*UtU[2][2];
	Z[1]-=f*Z[2];
	UtU[1][2]=0;

	f=UtU[0][2]/UtU[2][2];
	UtU[0][0]-=f*UtU[2][0]; //superflu
	UtU[0][1]-=f*UtU[2][1];
	UtU[0][2]-=f*UtU[2][2];
	Z[0]-=f*Z[2];
	UtU[0][2]=0;

	f=UtU[0][1]/UtU[1][1];
	UtU[0][0]-=f*UtU[1][0]; //superflu
	UtU[0][1]-=f*UtU[1][1];
	UtU[0][2]-=f*UtU[1][2]; //superflu
	Z[0]-=f*Z[1];
	UtU[0][1]=0;
	//=> matrice diagonale

	FIT_COEFF r;
	r.c0=Z[0]/UtU[0][0];
	r.cx=Z[1]/UtU[1][1];
	r.cx/=sx;
	r.cx2=Z[2]/UtU[2][2];
	r.cx2/=sx*sx;
	return r;
}

// UtU B = Ut Z
//Fit par une parabole de rivolution
FIT_COEFF_2D lsCalculate2d2(float* D,int sx,int sy)
{
	CHECK((sx<=0)||(sy<=0),"lsCalculate2d",	{FIT_COEFF_2D r;r.c0=0;r.cx=0;r.cy=0;r.cx2=0;r.cy2=0;return r;};);
	double UtU[5][5];
	double Z[5];

	DWORD N=0;
	double sumX=0;
	double sumX2=0;
	double sumY=0;
	double sumY2=0;
	double sumXY=0;
	double sumX2Y=0;
	double sumXY2=0;
	double sumX2Y2=0;
	double sumX3=0;
	double sumY3=0;
	double sumX4=0;
	double sumY4=0;

	DWORD xc,yc;
	double x,y;
	double s=1.0f;

	for(yc=0;yc<sy;yc++)
	{
		y=yc/double(sy);
		for(xc=0;xc<sx;xc++)
		{
			x=xc/double(sx);
			double x2=x*x;
			double y2=y*y;

			N++;
			sumX+=s*x;
			sumY+=s*y;
			sumX2+=s*x2;
			sumY2+=s*y2;
			sumXY+=s*x*y;

			sumX2Y+=s*x2*y;
			sumXY2+=s*x*y2;

			sumX3+=s*x2*x;
			sumY3+=s*y2*y;

			sumX4+=s*x2*x2;
			sumY4+=s*y2*y2;

			sumX2Y2+=s*x2*y2;
		}
	}

	UtU[0][0]=N;
	UtU[1][1]=sumX2;
	UtU[2][2]=sumY2;
	UtU[3][3]=sumX4;
	UtU[4][4]=sumY4;

	UtU[0][1]=UtU[1][0]=sumX;
	UtU[0][2]=UtU[2][0]=sumY;
	UtU[0][3]=UtU[3][0]=sumX2;
	UtU[0][4]=UtU[4][0]=sumY2;

	UtU[1][2]=UtU[2][1]=sumXY;

	UtU[1][3]=UtU[3][1]=sumX3;
	UtU[1][4]=UtU[4][1]=sumXY2;

	UtU[2][3]=UtU[3][2]=sumX2Y;
	UtU[2][4]=UtU[4][2]=sumY3;

	UtU[3][4]=UtU[4][3]=sumX2Y2;

	double sumZ=0;
	double sumXZ=0;
	double sumYZ=0;
	double sumX2Z=0;
	double sumY2Z=0;
	for(yc=0;yc<sy;yc++)
	{
		y=yc/double(sy);
		for(xc=0;xc<sx;xc++)
		{
			x=xc/double(sx);
			double z=D[yc*sx+xc];
			sumZ+=s*z;
			sumXZ+=s*x*z;
			sumYZ+=s*y*z;
			sumX2Z+=s*x*x*z;
			sumY2Z+=s*y*y*z;
		}
	}

	Z[0]=sumZ;
	Z[1]=sumXZ;
	Z[2]=sumYZ;
	Z[3]=sumX2Z;
	Z[4]=sumY2Z;

	double f;

	int p,l,c;
	for(p=0;p<4;p++) //numiro du pivot
		for(l=p+1;l<5;l++) //ligne ` simplifier
		{
			f=UtU[l][p]/UtU[p][p]; //UtU[p][p] est le pivot
			for(c=p;c<5;c++)
				UtU[l][c]-=f*UtU[p][c];
			Z[l]-=f*Z[p];
			UtU[l][p]=0;
		}

	for(p=4;p>0;p--)
		for(l=p-1;l>=0;l--)
		{
			f=UtU[l][p]/UtU[p][p];
			for(c=4;c>=p;c--)
				UtU[l][c]-=f*UtU[p][c];
			Z[l]-=f*Z[p];
			UtU[l][p]=0;
		}

	for(l=0;l<5;l++)
	{
		Z[l]/=UtU[l][l];
		UtU[l][l]=1;
	}

	FIT_COEFF_2D r;
	r.c0=Z[0];
	r.cx=Z[1]/sx;
	r.cy=Z[2]/sy;
	r.cx2=Z[3]/(sx*sx);
	r.cy2=Z[4]/(sy*sy);
	return r;
}

// UtU B = Ut Z
//Fit par une parabole de rivolution
FIT_COEFF_2D lsCalculate2d2M(float* D,BYTE* M,int sx,int sy)
{
	CHECK((sx<=0)||(sy<=0),"lsCalculate2d",	{FIT_COEFF_2D r;r.c0=0;r.cx=0;r.cy=0;r.cx2=0;r.cy2=0;return r;};);
	double UtU[5][5];
	double Z[5];

	DWORD N=0;
	double sumX=0;
	double sumX2=0;
	double sumY=0;
	double sumY2=0;
	double sumXY=0;
	double sumX2Y=0;
	double sumXY2=0;
	double sumX2Y2=0;
	double sumX3=0;
	double sumY3=0;
	double sumX4=0;
	double sumY4=0;

	DWORD xc,yc;
	double x,y;
	double s=1.0f;

	for(yc=0;yc<sy;yc++)
	{
		y=yc/double(sy);
		for(xc=0;xc<sx;xc++)
		{
			if(M[xc+sx*yc])
			{
			x=xc/double(sx);
			double x2=x*x;
			double y2=y*y;

			N++;
			sumX+=s*x;
			sumY+=s*y;
			sumX2+=s*x2;
			sumY2+=s*y2;
			sumXY+=s*x*y;

			sumX2Y+=s*x2*y;
			sumXY2+=s*x*y2;

			sumX3+=s*x2*x;
			sumY3+=s*y2*y;

			sumX4+=s*x2*x2;
			sumY4+=s*y2*y2;

			sumX2Y2+=s*x2*y2;
			}
		}
	}
	if(N==0)
	{
		FIT_COEFF_2D r;
		r.c0=0;
		r.cx=0;
		r.cy=0;
		r.cx2=0;
		r.cy2=0;
		return r;
	}

	UtU[0][0]=N;
	UtU[1][1]=sumX2;
	UtU[2][2]=sumY2;
	UtU[3][3]=sumX4;
	UtU[4][4]=sumY4;

	UtU[0][1]=UtU[1][0]=sumX;
	UtU[0][2]=UtU[2][0]=sumY;
	UtU[0][3]=UtU[3][0]=sumX2;
	UtU[0][4]=UtU[4][0]=sumY2;

	UtU[1][2]=UtU[2][1]=sumXY;

	UtU[1][3]=UtU[3][1]=sumX3;
	UtU[1][4]=UtU[4][1]=sumXY2;

	UtU[2][3]=UtU[3][2]=sumX2Y;
	UtU[2][4]=UtU[4][2]=sumY3;

	UtU[3][4]=UtU[4][3]=sumX2Y2;

	double sumZ=0;
	double sumXZ=0;
	double sumYZ=0;
	double sumX2Z=0;
	double sumY2Z=0;
	for(yc=0;yc<sy;yc++)
	{
		y=yc/double(sy);
		for(xc=0;xc<sx;xc++)
		{
			if(M[xc+sx*yc])
			{
			x=xc/double(sx);
			double z=D[yc*sx+xc];
			sumZ+=s*z;
			sumXZ+=s*x*z;
			sumYZ+=s*y*z;
			sumX2Z+=s*x*x*z;
			sumY2Z+=s*y*y*z;
			}
		}
	}

	Z[0]=sumZ;
	Z[1]=sumXZ;
	Z[2]=sumYZ;
	Z[3]=sumX2Z;
	Z[4]=sumY2Z;

	double f;

	int p,l,c;
	for(p=0;p<4;p++) //numiro du pivot
		for(l=p+1;l<5;l++) //ligne ` simplifier
		{
			f=UtU[l][p]/UtU[p][p]; //UtU[p][p] est le pivot
			for(c=p;c<5;c++)
				UtU[l][c]-=f*UtU[p][c];
			Z[l]-=f*Z[p];
			UtU[l][p]=0;
		}

	for(p=4;p>0;p--)
		for(l=p-1;l>=0;l--)
		{
			f=UtU[l][p]/UtU[p][p];
			for(c=4;c>=p;c--)
				UtU[l][c]-=f*UtU[p][c];
			Z[l]-=f*Z[p];
			UtU[l][p]=0;
		}

	for(l=0;l<5;l++)
	{
		Z[l]/=UtU[l][l];
		UtU[l][l]=1;
	}

	FIT_COEFF_2D r;
	r.c0=Z[0];
	r.cx=Z[1]/sx;
	r.cy=Z[2]/sy;
	r.cx2=Z[3]/(sx*sx);
	r.cy2=Z[4]/(sy*sy);
	return r;
}


/*
void SPG_CONV SPG_ParaFit3to13(float* D, int Pos, int Step, float* a, float* b, float* c)
{
}
*/

/*
void SPG_CONV SPG_ParaFitL(float* D, float& a, float& b, float& c, int FitLen)
{
	switch(FitLen)
	{
	case 3:
		SPG_ParaFit3(D,a,b,c);
		break;
	case 5:
		SPG_ParaFit5(D,a,b,c);
		break;
	case 7:
		SPG_ParaFit7(D,a,b,c);
		break;
	case 9:
		SPG_ParaFit9(D,a,b,c);
		break;
	case 11:
		SPG_ParaFit11(D,a,b,c);
		break;
	case 13:
		SPG_ParaFit13(D,a,b,c);
		break;
#ifdef DebugList
	default:
		SPG_List("SPG_ParaFitL: Longueurs valides: 3 5 7 9 11 13");
#endif
	}
	return;
}
*/

/*
//ax2+bx+c
//x=-1 0 1
void SPG_FASTCONV SPG_ParaFit3(float D[3], float& a, float& b, float& c)
{
#define am1 D[0]
#define a0 D[1]
#define ap1 D[2]
	float Sum1=am1+ap1;
	float Diff1=ap1-am1;
	a=0.5*Sum1-a0;
	b=0.5*Diff1;
	c=a0;
	return;
#undef am1
#undef a0
#undef ap1
}
*/

/*
//ax2+bx+c
//x=-2 -1 0 1 2
void SPG_FASTCONV SPG_ParaFit5(float D[5], float& a, float& b, float& c)
{
#define am2 D[0]
#define am1 D[1]
#define a0 D[2]
#define ap1 D[3]
#define ap2 D[4]
	float Sum1=am1+ap1;
	float Diff1=ap1-am1;
	float Sum2=am2+ap2;
	float Diff2=ap2-am2;
	a=(2*Sum2-Sum1-2*a0)/14.0;
	b=(2*Diff2+Diff1)/10.0;
	c=(-3*Sum2+12*Sum1+17*a0)/35.0;
	return;
#undef am2
#undef am1
#undef a0
#undef ap1
#undef ap2
}
*/

/*
//ax2+bx+c
//x=-3 -2 -1 0 1 2 3
void SPG_FASTCONV SPG_ParaFit7(float D[7], float& a, float& b, float& c)
{
#define am3 D[0]
#define am2 D[1]
#define am1 D[2]
#define a0 D[3]
#define ap1 D[4]
#define ap2 D[5]
#define ap3 D[6]
	float Sum1=am1+ap1;
	float Diff1=ap1-am1;
	float Sum2=am2+ap2;
	float Diff2=ap2-am2;
	float Sum3=am3+ap3;
	float Diff3=ap3-am3;
	a=(5*Sum3-3*Sum1-4*a0)/84.0;
	b=(3*Diff3+2*Diff2+Diff1)/28.0;
	c=(-2*Sum3+3*Sum2+6*Sum1+7*a0)/21.0;
	return;
#undef am3
#undef am2
#undef am1
#undef a0
#undef ap1
#undef ap2
#undef ap3
}
*/

/*
//ax2+bx+c
//x=-4 -3 -2 -1 0 1 2 3 4
void SPG_FASTCONV SPG_ParaFit9(float D[9], float& a, float& b, float& c)
{
#define am4 D[0]
#define am3 D[1]
#define am2 D[2]
#define am1 D[3]
#define a0 D[4]
#define ap1 D[5]
#define ap2 D[6]
#define ap3 D[7]
#define ap4 D[8]
	float Sum1=am1+ap1;
	float Diff1=ap1-am1;
	float Sum2=am2+ap2;
	float Diff2=ap2-am2;
	float Sum3=am3+ap3;
	float Diff3=ap3-am3;
	float Sum4=am4+ap4;
	float Diff4=ap4-am4;
	a=(28*Sum4+7*Sum3-8*Sum2-17*Sum1-20*a0)/924.0;
	b=(4*Diff4+3*Diff3+2*Diff2+Diff1)/60.0;
	c=(-21*Sum4+14*Sum3+39*Sum2+54*Sum1+59*a0)/231.0;
	return;
#undef am4
#undef am3
#undef am2
#undef am1
#undef a0
#undef ap1
#undef ap2
#undef ap3
#undef ap4
}
*/

/*
//ax2+bx+c
//x=-5 -4 -3 -2 -1 0 1 2 3 4 5
void SPG_FASTCONV SPG_ParaFit11(float D[11], float& a, float& b, float& c)
{
#define am5 D[0]
#define am4 D[1]
#define am3 D[2]
#define am2 D[3]
#define am1 D[4]
#define a0 D[5]
#define ap1 D[6]
#define ap2 D[7]
#define ap3 D[8]
#define ap4 D[9]
#define ap5 D[10]
	float Sum1=am1+ap1;
	float Diff1=ap1-am1;
	float Sum2=am2+ap2;
	float Diff2=ap2-am2;
	float Sum3=am3+ap3;
	float Diff3=ap3-am3;
	float Sum4=am4+ap4;
	float Diff4=ap4-am4;
	float Sum5=am5+ap5;
	float Diff5=ap5-am5;
	a=(15*Sum5+6*Sum4-Sum3-6*Sum2-9*Sum1-10*a0)/858.0;
	b=(5*Diff5+4*Diff4+3*Diff3+2*Diff2+Diff1)/110.0;
	c=(-36*Sum5+9*Sum4+44*Sum3+69*Sum2+84*Sum1+89*a0)/429.0;
	return;
#undef am5
#undef am4
#undef am3
#undef am2
#undef am1
#undef a0
#undef ap1
#undef ap2
#undef ap3
#undef ap4
#undef ap5
}
*/

/*
//ax2+bx+c
//x=-6 -5 -4 -3 -2 -1 0 1 2 3 4 5 6
void SPG_FASTCONV SPG_ParaFit13(float D[13], float& a, float& b, float& c)
{
#define am6 D[0]
#define am5 D[1]
#define am4 D[2]
#define am3 D[3]
#define am2 D[4]
#define am1 D[5]
#define a0 D[6]
#define ap1 D[7]
#define ap2 D[8]
#define ap3 D[9]
#define ap4 D[10]
#define ap5 D[11]
#define ap6 D[12]
	float Sum1=am1+ap1;
	float Diff1=ap1-am1;
	float Sum2=am2+ap2;
	float Diff2=ap2-am2;
	float Sum3=am3+ap3;
	float Diff3=ap3-am3;
	float Sum4=am4+ap4;
	float Diff4=ap4-am4;
	float Sum5=am5+ap5;
	float Diff5=ap5-am5;
	float Sum6=am6+ap6;
	float Diff6=ap6-am6;
	a=(22*Sum6+11*Sum5+2*Sum4-5*Sum3-10*Sum2-13*Sum1-14*a0)/2002.0;
	b=(6*Diff6+5*Diff5+4*Diff4+3*Diff3+2*Diff2+Diff1)/182.0;
	c=(-11*Sum6+9*Sum4+16*Sum3+21*Sum2+24*Sum1+25*a0)/143.0;
	return;
#undef am6
#undef am5
#undef am4
#undef am3
#undef am2
#undef am1
#undef a0
#undef ap1
#undef ap2
#undef ap3
#undef ap4
#undef ap5
#undef ap6
}
*/

#endif

