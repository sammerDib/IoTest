
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION
#ifdef SPG_General_USESCXDISPATCH

#include "..\SPG_Includes.h"
#include "..\SPG_SysInc.h"

#include "SCM_ConnexionDbg.h"

#include <string.h>
#include <stdio.h>




// ###########################################################################

SCX_DISPATCH_CONNEXION* SPG_CONV scxDispatchOpen()
{
	SCX_DISPATCH_CONNEXION* CD=SPG_TypeAlloc(1,SCX_DISPATCH_CONNEXION,"scxDispatchOpen");
	SPL_Init(CD->L,100,"DispatchConnexion");
	CD->NumC=0;
	CD->MaxDispatchFct = MAXDISPATCHFCT;
	CD->NumDispatchFct = 0;
	SPL_Exit(CD->L);
	return CD;
}

void SPG_CONV scxAttach(SCX_DISPATCH_CONNEXION* CD, SCX_CONNEXION* C)
{
	CHECK(C==0,"SCM_Interface_Dispatch",return);
	CHECK(CD->NumC>=MAXDISPATCHSCX,"scxAttach",return);
	SPL_Enter(CD->L,"scxDetach");
	CD->C[CD->NumC++]=C;
	SPL_Exit(CD->L);
	return;
}

void SPG_CONV scxDetach(SCX_DISPATCH_CONNEXION* CD, SCX_CONNEXION* C)
{
	CHECK(C==0,"SCM_Interface_Dispatch",return);
	SPL_Enter(CD->L,"scxDetach");
	int n;
	for(n=0;n<CD->NumC;n++)
	{
	 if(CD->C[n]==C) break;
	}
	CHECK(n==CD->NumC,"scxDetach not found",return);
	for(;n<CD->NumC-1;n++)
	{
		CD->C[n]=CD->C[n+1];
	}
	CD->NumC=n;
	SPL_Exit(CD->L);
	return;
}

int SPG_CONV scxClose(SCX_DISPATCH_CONNEXION* &CD)
{
	CHECK(CD==0,"SCM_Interface_Packet:scxClose",return scxINVALID);
	SPL_Enter(CD->L,"scxClose");
	SPL_Close(CD->L);
	SPG_MemFree(CD);
	return scxOK;
}

/*
int SPG_CONV scxWrite(void* Data, int DataLen, SCX_DISPATCH_CONNEXION* CD)
{
	CHECK(CD==0,"Null DispatchConnexion",return 0);
	return scxWrite(Data,DataLen,CD->C);
}
*/

int SPG_CONV scxReadLock(SNAKESEG G[MAXSNAKESEG], int& N, SCX_DISPATCH_CONNEXION* CD, int n, char* Owner)
{
	CHECK(CD==0,"Null DispatchConnexion",return N=0);//N=0; int L=0; return L;
	CHECK(SPL_Enter(CD->L,Owner)!=SPL_OK,"Deadlocked",return N=0);
	{//ajoute de nouvelles données si il y en a
		SNAKESEG readG;
		SnakeLockLinear(CD->Free[n],readG);
		int rLen=scxRead(readG.D,readG.N,CD->C[n]);
		if(rLen>0) SnakeCommit(CD->Msg[n],readG.bD+rLen,&CD->Free[n]);
	}

	int L=0;
	int u=0;
	for(int i=CD->Msg[n].iTail;i!=CD->Msg[n].iHead;i=SnakeNext(i))
	{
		DbgCHECK((CD->Msg[n].G[i].N==0)&&(i!=CD->Msg[n].iTail),"Null segment in list");
		if(CD->Msg[n].G[i].N)	{	L+=CD->Msg[n].G[i].N; G[u++]=CD->Msg[n].G[i];	}
		if(u==N) break;
	}
	N=u;
	//if(n==0) SPL_Exit(CD->L);
	return L;
}

void SPG_CONV scxReadUnock(SNAKESEG G[MAXSNAKESEG], void* D, SCX_DISPATCH_CONNEXION* CD, int n)
{
	SnakeCommit(CD->Free[n],D,&CD->Msg[n]);
	SPL_Exit(CD->L);
	return;
}

void SPG_CONV scxReadUnock(SNAKESEG G[MAXSNAKESEG], SNAKEPTR& P, SCX_DISPATCH_CONNEXION* CD, int n)
{
	SnakeCommit(CD->Free[n],P,&CD->Msg[n]);
	SPL_Exit(CD->L);
	return;
}

int SPG_CONV scxSetDispatchFct(SCX_DISPATCH_CONNEXION* CD, SCX_DISPATCH_CLBK scxDispatchFct, void* scxDispatchUser)
{
	CHECK(CD->NumDispatchFct>=CD->MaxDispatchFct,"scxSetDispatchFct",return -1);
	CD->scxDispatchFct[CD->NumDispatchFct]=scxDispatchFct;
	CD->scxDispatchUser[CD->NumDispatchFct]=scxDispatchUser;
	return CD->NumDispatchFct++;
}

void SPG_CONV scxRemoveDispatchFct(SCX_DISPATCH_CONNEXION* CD, SCX_DISPATCH_CLBK scxDispatchFct, void* scxDispatchUser)
{
	int i;
	for(i=0;i<CD->NumDispatchFct;i++)
	{
		if( (CD->scxDispatchFct[i]==scxDispatchFct)&&(CD->scxDispatchUser[i]==scxDispatchUser) ) break;
	}
	for(;i<CD->NumDispatchFct-1;i++)
	{
		CD->scxDispatchFct[i]=CD->scxDispatchFct[i+1];
		CD->scxDispatchUser[i]=CD->scxDispatchUser[i+1];
	}
	CD->NumDispatchFct=i;
	return;
}

void SPG_CONV scxDispatch(SCX_DISPATCH_CONNEXION* CD)
{
	for(int n=0;n<CD->NumC;n++)
	{
		int ReadLen;
		do
		{
			SNAKESEG G[MAXSNAKESEG]; int N=MAXSNAKESEG;	ReadLen=0;
			SNAKEPTR P; P.g=0; P.index=CD->Msg[n].iTail;
			int L=scxReadLock(G,N,CD,n,"scxDispatch");
			if(L)
			{
				for(int i=0;i<CD->NumDispatchFct;i++)
				{
					ReadLen=CD->scxDispatchFct[i](G,N,CD->scxDispatchUser[i],CD->C[n]->CI->scxWrite);
					if(ReadLen>0) break;
				}
			}
			SnakeAddPtr(CD->Msg[n], P, ReadLen);
			scxReadUnock(G,P,CD,n);
		} while (ReadLen);
	}
	return;
}

#endif
#endif


