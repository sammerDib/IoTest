
#include "..\SrcC\SPG.h"
#include "LagrangePolynome.h"
#include <math.h>

typedef __int64 TI;

//le plus petit premier apres 2^32 : 4294967311
//le plus petit grand avant 2^31 : 2147483647
const TI P = 2147483647; //4294967311; //5+2^32
//4294967291

// 6  5  4  3  2  1  0  -1  -2  -3  -4  -5  -6
//             0   2  1 0  -1   -2   0  -1
//            0   2   1 0  2    1    0   2   1  

__int64 abs64(__int64 x) { return (x>=0)?x:-x; }

__int64 cDiv(__int64 n, __int64 d) 
{ 
	__int64 Q;
	if( ( (n>=0)&&(d>0) ) || ( (n<=0)&&(d<0) ) )
	{
		Q=abs64(n)/abs64(d); 
	}
	else
	{
		Q=-(abs64(n)+abs64(d)-1)/abs64(d);
	}
	DbgCHECK(!V_IsBound(n-Q*d,0,abs64(d)),"cDiv");
	return Q;
}
__int64 cMod(__int64 n, __int64 d) 
{
	return n-d*cDiv(n,d); 
}
__int64 cSub(__int64 n, __int64 d) { return n-d; }
__int64 cMul(__int64 n, __int64 d) { return n*d; }


TI Mod(TI x) { return cMod(x,P);} //{TI m; if(x>=0) m=x%P; else m=P-(x-1)%P; return m;}
TI Mod(TI n, TI d) { return cMod(n,d);} //{TI m; if(x>=0) m=x%P; else m=P-(x-1)%P; return m;}
TI Div(TI n, TI d) { return cDiv(n,d);} //{TI m; if(x>=0) m=x%P; else m=P-(x-1)%P; return m;}
TI Mul(TI a, TI b)	{	TI m=a*b; return Mod(m);	}
TI Add(TI a, TI b)	{	TI m=a+b; return Mod(m);	}
TI Sub(TI a, TI b)	{	TI m=a-b; return Mod(m);	}


//http://www.algorithmist.com/index.php/Modular_inverse
TI Inv(TI a)	
{
	TI abkp=a;

	__int64 b = P;
    __int64 x=0; __int64 y=1; __int64 u=1; __int64 v=0;
    while( a != 0 )
	{
        __int64 q = cDiv(b,a); __int64 r=cMod(b,a); __int64 m = cSub(x,cMul(u,q)); __int64 n = cSub(y,cMul(v,q));
        b=a;
		a=r;
		x=u;
		y=v;
		u=m;
		v=n;
	}

	//x=Mod(x);
	TI One=Mul(x,abkp);
	DbgCHECK(One!=1,"Inv");

    return x;//b, x, y;
}

void PolyN_Create(PolyN& p, int n)	{	p.n=n;	p.a=SPG_TypeAlloc(p.n,PTI,"PolyN");	}

void PolyN_Close(PolyN& p)	{	p.n=0;	SPG_MemFree(p.a);	}

void Mul(PolyN& T, PolyN& a, PolyN& b)
{
	for(int t=0;t<T.n;t++)
	{
		T.a[t]=0;
		for(int i=0;i<=t;i++)
		{
			int j=t-i;
			T.a[t]=Add(T.a[t],Mul(a.a[i],b.a[j]));
		}
	}
}

void Add(PolyN& a, PolyN& b)
{
	for(int i=0;i<V_Min(a.n,b.n);i++)
	{
		a.a[i]=Add(a.a[i],b.a[i]);
	}
	return;
}

void Sub(PolyN& a, PolyN& b)
{
	for(int i=0;i<V_Min(a.n,b.n);i++)
	{
		a.a[i]=Sub(a.a[i],b.a[i]);
	}
	return;
}

void Cpy(PolyN& a, PolyN& b)
{
	for(int i=0;i<V_Min(a.n,b.n);i++)
	{
		a.a[i]=b.a[i];
	}
	return;
}

typedef PolyN LControlNodes;

#define LControlNodes_Create PolyN_Create
#define LControlNodes_Close PolyN_Close


void PolyN_CreateLagrange(PolyN& M, LControlNodes& C, int Item)
{
	PolyN T;
	PolyN_Create(T,C.n);
	PolyN_Create(M,C.n);
	M.a[0]=1;
	PolyN d;
	PolyN_Create(d,C.n);
	for(int i=0;i<C.n;i++)
	{
		if(i==Item) continue;
		d.a[0]=Sub(0,C.a[i]);//-C.a[i];//
		d.a[1]=1;
		Mul(T,M,d);
		Cpy(M,T);
	}
	PolyN_Close(T);
	PolyN_Close(d);
	return;
}

PTI Eval(PolyN& p, PTI x)
{
	TI Y=0;
	int i;
	for(i=p.n-1;i>0;i--)
	{
		Y=Add(Y,p.a[i]);
		Y=Mul(Y,x);
	}
	return Y=Add(Y,p.a[i]);
}

#if 0

void Print(PolyN& p)
{
	char S[256]; char*s=S;
	for(int i=0;i<p.n;i++)
	{
		s+=sprintf(s,"%I64i x%i\n",p.a[i],i);
	}
	SPG_List(S);
}


void LagrangePolynomeTest()
{
	LControlNodes C;
	LControlNodes_Create(C,8);
	for(int i=0;i<C.n;i++)
	{
		C.a[i]=2*i;
	}
	PolyN* M=SPG_TypeAlloc(C.n,PolyN,"LagrangePolynomeTest");
	for(int i=0;i<C.n;i++)
	{
		PolyN_CreateLagrange(M[i],C,i);
		TI Y=Eval(M[i],C.a[i]);//evalue à l'endroit du control point
		TI N=Inv(Y);
		//N=Mul(N,i);//on veut M tq Eval(M[i],C.a[i]) == i
		
		for(int h=0;h<M[i].n;h++)
		{
			M[i].a[h]=Mul(M[i].a[h],N);
		}
		for(int h=0;h<M[i].n;h++)
		{
			M[i].a[h]=Mul(M[i].a[h],i);
		}
		TI tst=Eval(M[i],C.a[i]);//evalue à l'endroit du control point
		DbgCHECK(tst!=i,"LagrangePolynomeTest");

		//Print(M[i]);
		
		for(int x=0;x<10;x++)
		{
			char S[512];
			sprintf(S,"P%i x=%i y=%I64i\r\n",i,x,Eval(M[i],x));
			OutputDebugString(S);
		}
	}

	PolyN H; PolyN_Create(H,C.n);

	for(int i=0;i<C.n;i++) Add(H,M[i]);


	for(int x=0;x<10;x++)
	{
		char S[512];
		sprintf(S,"x=%i y=%I64i\r\n",x,Eval(H,x));
		OutputDebugString(S);
	}
	for(int i=0;i<C.n;i++)
	{
		char S[512];
		sprintf(S,"x=%I64i y=%I64i\r\n",C.a[i],Eval(H,C.a[i]));
		OutputDebugString(S);
	}

typedef struct LISTPTR
{
	int Padding0;
	int L;
//	int Padding1;
} LISTPTR;

	int Max=64*1024*1024;
	int Msk=Max-1;
	LISTPTR* L=SPG_TypeAlloc(128*1024*1024,LISTPTR,"ListPtr");
	for(int n=0;n<Max;n++)
	{
		L[n].L=(rand()+(rand()<<8)+(rand()<<16)+(rand()<<24))&Msk;
		//L[n].L=12345;//(n+1)&Msk;
	}

	S_CreateTimer(Trnd,"Trnd");
	S_CreateTimer(TP,"TP");

	int HS=0;
	int HN0=0;
	int HN1=0;
	int HN2=0;


	for(int i=0;i<5;i++)
	{

	S_StartTimer(Trnd);
	for(int i=0;i<100;i++)
	{
		int L0=(rand()+(rand()<<12))&Msk;
		for(int c=0;c<C.n;c++)
		{
			L0=L[L0].L;
		}
		HS+=L0;
		HN0++;
	}
	S_StopTimer(Trnd);

	S_StartTimer(TP);
	for(int i=0;i<100;i++)
	{
		int L0=(rand()+(rand()<<12))&15;
		L0=Eval(H,L0);
		HS+=L0;
		HN1++;
	}
	S_StopTimer(TP);

	}

	char Msg[512];
	sprintf(Msg,"Random LUT read : %I64i\nPolynom : %I64i\n%.2f\n%i",Trnd.TotalCount/HN0/C.n,TP.TotalCount/HN1/C.n,(double)Trnd.TotalCount/(double)TP.TotalCount,HS);
	MessageBox(0,Msg,0,0);

	S_CloseTimer(Trnd);
	S_CloseTimer(TP);

	SPG_MemFree(L);

	for(int i=0;i<C.n;i++)
	{
		PolyN_Close(M[i]);
	}
	SPG_MemFree(M);
	PolyN_Close(H);
	LControlNodes_Close(C);
}

#endif

