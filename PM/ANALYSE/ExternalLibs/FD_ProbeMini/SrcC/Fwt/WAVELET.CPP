
#include "..\SPG_General.h"

#ifdef SPG_General_USEFWT

#include "wavelet.h"

#pragma SPGMSG(__FILE__,__LINE__,"Using FWT")

#pragma warning(disable:4244)
#pragma warning(disable:4305)

void SPG_CONV wtransform(float* a,float* wksp,unsigned long n,int isign,
		 void (*wtstep)(float*,float*,unsigned long,int));
//void pwt(float* a,float* wksp,unsigned long n,int isign);

#define C0 0.4829629131445341
#define C1 0.8365163037378079
#define C2 0.2241438680420134
#define C3 -0.1294095225512604

void SPG_CONV daub4(float* a,float* wksp,unsigned long n,int isign)
{
	unsigned long nh,nh1,i,j;

	if(n<4)
		return;
	nh1=(nh=n >> 1)+1;
	if(isign>=0)
	{
		for(i=1,j=1;j<=n-3;j+=2,i++)
		{
			wksp[i]=C0*a[j]+C1*a[j+1]+C2*a[j+2]+C3*a[j+3];
			wksp[i+nh]=C3*a[j]-C2*a[j+1]+C1*a[j+2]-C0*a[j+3];
		}
		wksp[i]=C0*a[n-1]+C1*a[n]+C2*a[1]+C3*a[2];
		wksp[i+nh]=C3*a[n-1]-C2*a[n]+C1*a[1]-C0*a[2];
	}
	else
	{
		wksp[1]=C2*a[nh]+C1*a[n]+C0*a[1]+C3*a[nh1];
		wksp[2]=C3*a[nh]-C0*a[n]+C1*a[1]-C2*a[nh1];
		for(i=1,j=3;i<nh;i++)
		{
			wksp[j++]=C2*a[i]+C1*a[i+nh]+C0*a[i+1]+C3*a[i+nh1];
			wksp[j++]=C3*a[i]-C0*a[i+nh]+C1*a[i+1]-C2*a[i+nh1];
		}
	}
	for(i=1;i<=n;i++)
		a[i]=wksp[i];
	return;
}

typedef struct
{
	int ncof,ioff,joff;
	float *cc,*cr;
} wavefilt;

wavefilt wfilt;

void SPG_CONV pwtset(int n)
{
	wavefilt* wf=&wfilt;
	int k;
	float sig= -1.0;
	static float c4[5]={0.0,0.4829629131445341,0.8365163037378079,
		0.2241438680420134,-0.1294095225512604};
	static float c12[13]={0.0,0.111540743350,0.494623890398,0.751133908021,
		0.315250351709,-0.226264693965,-0.129766867567,
		0.098501605587,0.027522865530,-0.031582039318,
		0.000553842201,0.004777257511,-0.001077301085};
	static float c20[21]={0.0,0.026670057901,0.188176800078,0.527201188932,
		0.688459039454,0.281172343661,-0.249846424327,
		-0.195946274377,0.127369340336,0.093057364604,
		-0.071394147166,-0.029457536822,0.033212674059,
		0.003606553567,-0.010733175483,0.001395351747,
		0.001992405295,-0.000685856695,-0.000116466855,
		0.000093588670,-0.000013264203};
	static float c4r[5],c12r[13],c20r[21];

	wf->ncof=n;
	if(n==4)
	{
		wf->cc=c4;
		wf->cr=c4r;
	}
	else
	if(n==12)
	{
		wf->cc=c12;
		wf->cr=c12r;
	}
	else
	if(n==20)
	{
		wf->cc=c20;
		wf->cr=c20r;
	}

	for(k=1;k<=n;k++)
	{
		wf->cr[wf->ncof+1-k]=sig*wf->cc[k];
		sig=-sig;
	}
	wf->ioff=wf->joff=-(n>>1);
//	wf->ioff=-2;
//	wf->joff=-n+2;
	return;
}

void SPG_CONV pwt(float* a,float* wksp,unsigned long n,int isign)
{
	float ai,ai1;
	unsigned long i,ii,j,jf,jr,k,n1,ni,nj,nh,nmod;

	if(n<4)
		return;
	nmod=wfilt.ncof*n;
	n1=n-1;
	nh=n>>1;
	for(j=1;j<=n;j++)
		wksp[j]=0.0;

	if(isign>=0)
	{
		for(ii=1,i=1;i<=n;i+=2,ii++)
		{
			ni=i+nmod+wfilt.ioff;
			nj=i+nmod+wfilt.joff;
			for(k=1;(signed)k<=wfilt.ncof;k++)
			{
				jf=n1&(ni+k);
				jr=n1&(nj+k);
				wksp[ii]+=wfilt.cc[k]*a[jf+1];
				wksp[ii+nh]+=wfilt.cr[k]*a[jr+1];
			}
		}
	}
	else
	{
		for(ii=1,i=1;i<=n;i+=2,ii++)
		{
			ai=a[ii];
			ai1=a[ii+nh];
			ni=i+nmod+wfilt.ioff;
			nj=i+nmod+wfilt.joff;
			for(k=1;(signed)k<=wfilt.ncof;k++)
			{
				jf=(n1&(ni+k))+1;
				jr=(n1&(nj+k))+1;
				wksp[jf]+=wfilt.cc[k]*ai;
				wksp[jr]+=wfilt.cr[k]*ai1;
			}
		}
	}
	for(j=1;j<=n;j++)
		a[j]=wksp[j];
	return;
}

void SPG_CONV wtransform(float* a,float* wksp,unsigned long n,int isign,
		 void (*wtstep)(float*,float*,unsigned long,int))
{
	unsigned long nn;

	if(n<4)
		return;
	if(isign>=0)
	{
		for(nn=n;nn>=4;nn>>=1)
			(*wtstep)(a,wksp,nn,isign);
	}
	else
	{
		for(nn=4;nn<=n;nn<<=1)
			(*wtstep)(a,wksp,nn,isign);
	}
	return;
}

#endif
