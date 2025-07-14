
#include "SPG_General.h"

#ifdef SPG_General_USEWEIGHTEDPARAFIT

#include "SPG_Includes.h"
#include <memory.h>

template <typename K_FIT_ELT> void SPG_CONV K_FITW_InitElt(K_FIT_ELT* e, int FITORDER, int N, SPG_FITWEIGHT w, void* User)
{
	CHECK(e==0,"K_FITW_InitElt",return);
	CHECK(w==0,"K_FITW_InitElt",return);

	//const int FITORDER = 2;

	Matrix M;
	DiagonalMatrix W;

	int P=V_Min(N,(1+FITORDER));

	M.ReSize(N,P);
	W.ReSize(N);

	{for(int n=0;n<N;n++)
	{
		kfloat x=(2*n+1-N)/(kfloat)N;
		W(1+n,1+n)=w(User,x);
	}}

	{for(int n=0;n<N;n++)
	{
		M(1+n,1+0)=1;
	}}
	{for(int p=1;p<P;p++)
	{
		for(int n=0;n<N;n++)
		{
			kfloat x=0.5*(2*n+1-N);
			M(1+n,1+p)=(x)*M(1+n,1+p-1);
		}
	}}

	//Mt.W.M.x = Mt.W.y
	//x = (Mt.W.M)inv . Mt . W . y
	//char strM[256]; PrintMatrix(strM,M,"M"); SPG_List(strM);

	Matrix Q;
	Q=M.t() * W * M;	//char strQ[256]; PrintMatrix(strQ,Q,"Q"); SPG_List(strQ);
	Matrix iQ=Q.i();	//char striQ[256]; PrintMatrix(striQ,iQ,"iQ"); SPG_List(striQ);
	Matrix iQMW = iQ * M.t() * W; 	//char striQMW[256]; PrintMatrix(striQMW,iQMW,"iQMW"); SPG_List(striQMW);

	{
		int p;
		for(p=0;p<P;p++)
		{
			for(int n=0;n<N;n++)
			{
				e[n].a[p]=iQMW(p+1,n+1);
				//fprintf(F,"%g\t",iQMW(p+1,n+1));
				if(2*n>=N)
				{
					if((p&1)==0)
					{//les coefficients d'ordre pair sont symétriques
						kfloat S	=	e[n].a[p]	+	e[N-1-n].a[p];
						e[N-1-n].a[p]	=	0.5 * S;
						e[n].a[p]	=	0.5 * S;
					}
					else
					{//les coefficients d'ordre impair sont asymétriques
						kfloat D	=	e[n].a[p]	-	e[N-1-n].a[p];
						e[N-1-n].a[p]	=	-0.5 * D;
						e[n].a[p]	=	0.5 * D;
					}

				}
			}
		}
		for(;p<(1+FITORDER);p++)
		{
			for(int n=0;n<N;n++)
			{
				e[n].a[p]=0;
				//fprintf(F,"%g\t",iQMW(p+1,n+1));
			}
		}
	}
	return;
}

void SPG_CONV K_FIT2W_Init(K_FIT2& KF2, int Len, SPG_FITWEIGHT w, void* User)
{
	int S=0;//nombre total d'element
	KF2.Len=Len;
	KF2.e=SPG_TypeAlloc( (1+KF2.Len) , K_FIT2_ELT* ,"K_FIT2W_Init");
	int L;
	for(L=0;L<=Len;L++) { S+=L; }//nombre total d'element
	KF2.e[0]=SPG_TypeAlloc(S,K_FIT2_ELT,"K_FIT2W_Init");
	S=0;
	for(L=1;L<=Len;L++)//pour chaque longueur de fit possible
	{
		KF2.e[L]=KF2.e[0]+S;//ajoute l'adresse de base du tableau à l'offset
		S+=L;
		//K_FIT2W_InitElt(KF2.e[L],L,w,User);
		K_FITW_InitElt(KF2.e[L],2,L,w,User);
	}
	return;
}

void SPG_CONV K_FIT2W_Close(K_FIT2& KF)
{
	CHECK(KF.e==0,"K_FIT2W_Close",return);
	SPG_MemFree(KF.e[0]);
	SPG_MemFree(KF.e);
	SPG_ZeroStruct(KF);
	return;
}

void SPG_CONV K_FIT3W_Init(K_FIT3& KF3, int Len, SPG_FITWEIGHT w, void* User)
{
	int S=0;//nombre total d'element
	KF3.Len=Len;
	KF3.e=SPG_TypeAlloc( (1+KF3.Len) , K_FIT3_ELT* ,"K_FIT3W_Init");
	int L;
	for(L=0;L<=Len;L++) { S+=L; }//nombre total d'element
	KF3.e[0]=SPG_TypeAlloc(S,K_FIT3_ELT,"K_FIT3W_Init");
	S=0;
	for(L=1;L<=Len;L++)//pour chaque longueur de fit possible
	{
		KF3.e[L]=KF3.e[0]+S;//ajoute l'adresse de base du tableau à l'offset
		S+=L;
		//K_FIT2W_InitElt(KF2.e[L],L,w,User);
		K_FITW_InitElt(KF3.e[L],3,L,w,User);
	}
	return;
}

void SPG_CONV K_FIT3W_Close(K_FIT3& KF)
{
	CHECK(KF.e==0,"K_FIT3W_Close",return);
	SPG_MemFree(KF.e[0]);
	SPG_MemFree(KF.e);
	SPG_ZeroStruct(KF);
	return;
}

void SPG_CONV K_FIT4W_Init(K_FIT4& KF4, int Len, SPG_FITWEIGHT w, void* User)
{
	int S=0;//nombre total d'element
	KF4.Len=Len;
	KF4.e=SPG_TypeAlloc( (1+KF4.Len) , K_FIT4_ELT* ,"K_FIT4W_Init");
	int L;
	for(L=0;L<=Len;L++) { S+=L; }//nombre total d'element
	KF4.e[0]=SPG_TypeAlloc(S,K_FIT4_ELT,"K_FIT4W_Init");
	S=0;
	for(L=1;L<=Len;L++)//pour chaque longueur de fit possible
	{
		KF4.e[L]=KF4.e[0]+S;//ajoute l'adresse de base du tableau à l'offset
		S+=L;
		//K_FIT2W_InitElt(KF2.e[L],L,w,User);
		K_FITW_InitElt(KF4.e[L],4,L,w,User);
	}
	return;
}

void SPG_CONV K_FIT4W_Close(K_FIT4& KF)
{
	CHECK(KF.e==0,"K_FIT4W_Close",return);
	SPG_MemFree(KF.e[0]);
	SPG_MemFree(KF.e);
	SPG_ZeroStruct(KF);
	return;
}

#endif

