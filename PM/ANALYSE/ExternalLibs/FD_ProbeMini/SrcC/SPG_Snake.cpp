
#include "..\SrcC\SPG_General.h"

#ifdef SPG_General_USESnake

#include "..\SrcC\SPG_Includes.h"

#include <memory.h>
#include <string.h>
#include <stdio.h>

int SPG_CONV SnakeInit(SNAKE& S, int sz, int Flag, char* Name)
{
	SPG_ZeroStruct(S);
	CHECK(sz==0,"SNAKE ring buffer: sizeof(Element) cannot be null",return 0);
	S.sz=sz;
	S.Flag=Flag;
	strcpy(S.Name,Name);
	return -1;
}

void SPG_CONV SnakeClose(SNAKE& S)
{
	SPG_ZeroStruct(S);
	return;
}

void SPG_CONV SnakeGetStatus(char* Msg, SNAKE& S)
{
	Msg+=sprintf(Msg,"\r\n%s\tTail=%i\tHead=%i",S.Name,S.iTail,S.iHead);
	for(int i=0;i<MAXSNAKESEG;i++)
	{
		Msg+=sprintf(Msg,"\r\n%X\t%i\t%X",(int)S.G[i].iD,S.G[i].N,(int)(S.G[i].iD+S.G[i].N));
	}
	return;
}


SNAKEPTRPOS SPG_CONV SnakeGetPos(SNAKE S, SNAKESEG G, DWORD iD)//retourne -1 si S est nul
{//position du pointeur passé à Unlock
	//DbgCHECK(G.D==0,"SnakeGetPos");
	//DbgCHECK(G.N==0,"SnakeGetPos");
	if((G.D==0)||(G.N==0)) return POSEMPTY;//-2;
	if((G.iD>iD)||(G.iD+G.N*S.sz<iD)) return POSDISJOINT;//lu dans un segment autre, (anterieur ou suivant car les adresses ne sont pas ordonnees)
	else if(G.iD==iD) return POSORIGIN;//au debut pas entame
	else if(G.iD+G.N*S.sz==iD) return POSAPPEND;//lu jusqu'au dernier
	else return POSINSIDE;//en cours
}

SNAKESEGPOS SPG_CONV SnakeGetPos(SNAKE S, SNAKESEG G0, SNAKESEG G1)
{
	int p0=SnakeGetPos(S,G0,G1.iD);
	int p1=SnakeGetPos(S,G0,G1.iD+G1.N*S.sz);
	CHECK(p0==POSEMPTY,"SnakeGetPos",return SEGEMPTY);
	CHECK((p0==POSINSIDE)||(p1==POSINSIDE),"SnakeGetPos: Overlapping segments",return SEGOVERLAP);//ne détecte pas tous les cas d'overlap
	if(p0==POSAPPEND) return SEGAPPENDAFTER;
	else if (p1==POSORIGIN) return SEGAPPENDBEFORE;
	else return SEGDISJOINT;//DISJOINT = DISJOINT OU OVERLAP
}

int SPG_CONV SnakeGetTotalLength(SNAKE& S)
{
	int L=0;
	int i=S.iTail;
	for(;i!=S.iHead;i=SnakeNext(i))
	{
		DbgCHECK(S.G[i].N==0,"SnakeGetTotalLength");
		L+=S.G[i].N;
	}
	L+=S.G[i].N;//i==iHead

	return L;
}

BYTE* SPG_CONV SnakeGetReadPtr(SNAKE& S, int N)
{
	int L=0;
	int i=S.iTail;
	for(;i!=S.iHead;i=SnakeNext(i))
	{
		DbgCHECK(S.G[i].N==0,"SnakeGetTotalLength");
		if(S.G[i].N<N) {N-=S.G[i].N;} else {return S.G[i].bD+N*S.sz;}
	}
	if(S.G[i].N<=N) {return 0;} else {return S.G[i].bD+N*S.sz;} //i==iHead
}

void SPG_CONV SnakeAppend(SNAKE& S, void* D, int N)
{
	if(D==0) return;
	if(N==0) return;
	//CHECK(D==0,"SnakeAppend",return);
	//CHECK(N==0,"SnakeAppend",return);
	switch(SnakeGetPos(S,S.G[S.iHead],*(DWORD*)&D))
	{
	case POSDISJOINT://cree un nouveau segment
		S.iHead=SnakeNext(S.iHead); DbgCHECK(S.iHead==S.iTail,"SnakeAppend:Too many segments");
		// NO BREAK
	case POSEMPTY://fall through - initialise le segment en cours
		S.G[S.iHead].D=D;
		S.G[S.iHead].N=N;
		break;
	case POSAPPEND://append directement au segment head
		S.G[S.iHead].N+=N;
		break;
	case POSORIGIN:
	case POSINSIDE:
		DbgCHECK(1,"SnakeAppend:SnakeGetPos:Segment overlap");
		break;
	default:
		DbgCHECK(1,"SnakeAppend");
		break;
	}
	return;
}

int SPG_CONV SnakeIsEmpty(SNAKE& S)//void* &D0,int N0,...
{
	
	DbgCHECK((S.iTail!=S.iHead)&&(S.G[S.iTail].N==0),"SnakeIsEmpty");
	return S.G[S.iTail].N;
}

int SPG_CONV SnakeLock(SNAKE& mS, SNAKEPTR& mP)
{
	DbgCHECK((mS.iTail!=mS.iHead)&&(mS.G[mS.iTail].N==0),"SnakeLock");
	mP.index=mS.iTail; 
	mP.g=0;
	return mS.G[mP.index].N;
}

int SPG_CONV SnakeLock1(SNAKE& S, SNAKESEG& G0)//void* &D0,int N0,...
{
	
	G0=S.G[S.iTail];
	while((G0.N==0)&&(S.iTail!=S.iHead))
	{
		S.iTail=SnakeNext(S.iTail);
		G0=S.G[S.iTail];
	}
	return G0.N;
}

int SPG_CONV SnakeLock2(SNAKE& S, SNAKESEG& G0, SNAKESEG& G1)
{
	SnakeLock1(S,G0);

	if(S.iTail!=S.iHead) 
		G1=S.G[SnakeNext(S.iTail)];
	else
		SnakeClearSeg(G1);
	return G0.N+G1.N;
}

int SPG_CONV SnakeLockLinear(SNAKE& C, SNAKESEG& G0)//void* &D0,int N0,...
{
	CHECK((C.Flag&SNAKE_UNORDERED)==0,"SnakeLockLin : incorrect on sequential data",return 0);
	SNAKESEG G1;
	SnakeLock2(C,G0,G1);
	if(G1.N>G0.N)
	{
		SnakeCommit(C,G0.D,&C);
		G0=G1;
	}
	return G0.N;
}

int SPG_CONV SnakeIncSeg(SNAKE& mS, SNAKEPTR& mP) 
{
	if(mP.index!=mS.iHead) 
	{ 
		mP.index=SnakeNext(mP.index);
		mP.g=0; 
		return mS.G[mP.index].N;
	} 
	else 
	{
		mP.g=mS.G[mP.index].N;
		return 0;
	}
}

int SPG_CONV SnakeAddPtr(SNAKE& mS, SNAKEPTR& mP, int& mN) 
{
	for(int local_i=0;local_i<MAXSNAKESEG;local_i++) 
	{ 
		int d=(mS.G[mP.index].N-mP.g); 
		DbgCHECK(!V_InclusiveBound(d,0,mS.G[mP.index].N),"SnakeAddPtr"); 
		if(d>mN) 
		{ 
			mP.g+=mN;
			d-=mN;
			mN=0; 
			return d; 
		} 
		else 
		{ 
			mN-=d; 
			if(SnakeIncSeg(mS,mP)==0) return 0; 
		}
	}
	return 0;
}

int SPG_CONV SnakeCommit(SNAKE& S, SNAKEPTR& P, SNAKE* C)
{
	DbgCHECK(C&&(S.sz!=C->sz),"SnakeCommit");

	for(;S.iTail!=P.index;S.iTail=SnakeNext(S.iTail))
	{
		if(C) SnakeAppend(*C,S.G[S.iTail].D,S.G[S.iTail].N);
	}
	if(C) SnakeAppend(*C,S.G[S.iTail].D,P.g);
	S.G[S.iTail].N-=P.g;
	S.G[S.iTail].bD+=P.g*S.sz;
	while((S.G[S.iTail].N==0)&&(S.iTail!=S.iHead))
	{
		S.G[S.iTail].D=0;
		S.iTail=SnakeNext(S.iTail);
	}
	return 0;
}

int SPG_CONV SnakeCommit(SNAKE& S, void* D, SNAKE* C)
{
	CHECK(D==0,"SnakeCommit",return 0);
	DbgCHECK(C&&(S.sz!=C->sz),"SnakeUnlock");
	int allseg;
	for(allseg=0;allseg<MAXSNAKESEG;allseg++)//si le lock prend deux buffers max on peut se limiter a deux iterations (on itere que quand SnakeGetPos=-1)
	{
		switch(SnakeGetPos(S,S.G[S.iTail],*(DWORD*)&D))
		{
			case POSEMPTY:
				{DbgCHECK(1,"SnakeCommit : S tail segment is null");	return 0;}
				break;

			case POSDISJOINT:
				{//le segment est complètement dépassé, il va entièrement dans C, et il faut tester le prochain
					void* tailD=S.G[S.iTail].D;
					int tailN=S.G[S.iTail].N;

					DbgCHECK(S.iTail==S.iHead,"SnakeCommit:POSDISJOINT: no next segment");//impossible dans le cas POSDISJOINT sauf pointeur D invalide

					SnakeClearSeg(S.G[S.iTail]);

					if(S.iTail!=S.iHead) S.iTail=SnakeNext(S.iTail);//segment totalement vide mais il en existe un suivant
					//else //dernier segment totalement vide - SNAKE totalement vide - impossible dans le cas POSDISJOINT sauf pointeur D invalide

					if(C) SnakeAppend(*C,tailD,tailN);
				}
				break;

			case POSORIGIN:
				{//le segment est inchangé donc on return directement
					return -1;
				}
				break;

			case POSAPPEND:
				{//le segment est complètement dépassé, il va entièrement dans C, le prochain est inchangé (donc on return directement)
					void* cD=S.G[S.iTail].D;
					int cN=S.G[S.iTail].N;

					SnakeClearSeg(S.G[S.iTail]);//dernier segment totalement vide - SNAKE totalement vide
					if(S.iTail!=S.iHead) S.iTail=SnakeNext(S.iTail);//segment totalement vide mais il en existe un suivant
					//else

					if(C) SnakeAppend(*C,cD,cN);

					return -1;
				}
				break;

			case POSINSIDE://le segment est partiellement commité, le debut est mis dans C et le reste dans S
				{	
					int L=(BYTE*)D-S.G[S.iTail].bD;
					DbgCHECK(L%S.sz,"SnakeUnlock : element size mismatch");
					int cN=L/S.sz;
					void* cD=S.G[S.iTail].D;
					S.G[S.iTail].D=D;
					S.G[S.iTail].N-=cN;
					DbgCHECK(S.G[S.iTail].N==0,"SnakeCommit:POSINSIDE was actually POSAPPEND");
					if(C) SnakeAppend(*C,cD,cN);
					return -1;//trouve le segment en cours
				}
		}
	}
	DbgCHECK(true,"SnakeCommit:allseg=MAXSNAKESEG");
	return 0;
}

#ifdef SPG_General_USESpinLock
int SPG_CONV SnakeRead1(SNAKE& S, SPINLOCK& LS, SNAKESEG& G0, int DataLen)
{
	CHECK(DataLen%S.sz,"SnakeReadLinear1",return 0);
	CHECKTWO(SPL_Enter(LS,"SnakeReadLinear1")==SPL_TIMEOUT,"SnakeRead1",S.Name,return 0);
	if(SnakeLock1(S,G0)<DataLen) { SPL_Exit(LS); return 0; }
	G0.N=DataLen/S.sz;
	SnakeCommit(S,G0.bD+DataLen,0);
	SPL_Exit(LS);
	return G0.N;
}

int SPG_CONV SnakeWrite1(SNAKE& C, SPINLOCK& LC, SNAKESEG& G0)
{
	CHECKTWO(SPL_Enter(LC,"SnakeWrite1")==SPL_TIMEOUT,"SnakeWrite1",C.Name,return 0);
	SnakeAppend(C,G0.bD,G0.N*C.sz);
	SPL_Exit(LC);
	return -1;
}
#endif



void SPG_CONV SnakeFlush(SNAKE& S, SNAKE& C) //vide S dans C, a la sortie S est vide
{
	int allseg;
	for(allseg=0;allseg<MAXSNAKESEG;allseg++)//itere potentiellement plusieurs fois, (compteur pour eviter les boucles infinies en cas de bug), sort par return quand SnakeClearSeg
	{
		SnakeAppend(C,S.G[S.iTail].D,S.G[S.iTail].N); //vide la queue de S dans C
		if(S.iTail!=S.iHead) 
		{
			S.iTail=SnakeNext(S.iTail);//segment totalement vide mais il en existe un suivant
		}
		else
		{
			SnakeClearSeg(S.G[S.iTail]);//dernier segment totalement vide - SNAKE totalement vide
			return;//trouve le segment totalement lu
		}
	}
	DbgCHECK(true,"SnakeFlush:allseg=MAXSNAKESEG");//bug
	return;
}

int SPG_CONV SnakeDeleteSegAndGetNext(SNAKE& S, int i)
{
	int j=-1;
	int allseg;
	for(allseg=0;allseg<MAXSNAKESEG;allseg++)
	{//la boucle 'for' borne le nbre d'itérations
		if(i==S.iHead)
		{
			if(i==S.iTail)
			{
				SnakeClearSeg(S.G[i]);
				return j;
			}
			else
			{
				S.iHead=SnakePrev(i);
				return j;
			}
		}
		else
		{//ecrase le segment i
			S.G[i]=S.G[SnakeNext(i)];//propage le décalage induit par la suppression d'un segment
			i=SnakeNext(i);//successeur de i
			if(j==-1) j=i;//affecte le nr du segment suivant
		}
	}
	DbgCHECK(true,"SnakeDeleteSegAndGetNext:allseg=MAXSNAKESEG");
	return j;
}

int SPG_CONV SnakeMergeSegAndGetNext(SNAKE& S, int i, int j)
{
	int p=SnakeGetPos(S,S.G[i],S.G[j]);
	if(p==0) return (j==S.iHead)?-1:(SnakeNext(j));

	if(p==1)
	{
		S.G[i].N+=S.G[j].N;
	}
	else if(p==-1)
	{
		S.G[i].D=S.G[j].D;
		S.G[i].N+=S.G[j].N;
	}
	return SnakeDeleteSegAndGetNext(S,j);
}

void SPG_CONV SnakeReorder(SNAKE& S)
{
	if(S.Flag&SNAKE_UNORDERED)//&&!Locked
	{
		int i=S.iTail;
		int allseg;
		for(allseg=0;allseg<MAXSNAKESEG;allseg++)
		{
			int j=i;
			if(j==S.iHead) return;
			j=SnakeNext(j);
			do
			{
				j=SnakeMergeSegAndGetNext(S,i,j);
			} while(j!=-1);
			if(i==S.iHead) break;
			i=SnakeNext(i);
		}
		DbgCHECK(true,"SnakeReorder:allseg=MAXSNAKESEG");
	}
	return;	
}




#endif

