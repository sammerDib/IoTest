
#define MAXSNAKESEG 4

typedef struct
{
	union
	{
		void* D;
		BYTE* bD;
		DWORD iD;
	};
	int N;
} SNAKESEG;

typedef struct
{
	char Name[32];
	SNAKESEG SnakeNullSeg;
	SNAKESEG G[MAXSNAKESEG];
	int iTail;
	int iHead;
	int sz;
	int Flag;
} SNAKE;

#define SNAKE_UNORDERED 1  //le merge peut se faire en desordre (buffer free ne contenant pas de données), et le lock linear est disponible

typedef enum
{
	POSEMPTY=-2,
	POSDISJOINT=-1,
	POSORIGIN=0,
	POSAPPEND=1,
	POSINSIDE=2
} SNAKEPTRPOS;

typedef enum
{
	SEGEMPTY=-2,
	SEGDISJOINT=-1,//DISJOINT = DISJOINT OU OVERLAP
	SEGAPPENDBEFORE=0,
	SEGAPPENDAFTER=1,
	SEGOVERLAP=2
} SNAKESEGPOS;

typedef struct
{
	int g; //numero de l'element
	int index; //numero du buffer
} SNAKEPTR;


//efine Snake_CommitPtr(mD,mS,mP) SnakeCommit(mS,mG.D,&mD)

#define SnakeNext(i) ((i+1)%MAXSNAKESEG)
#define SnakePrev(i) ((i+MAXSNAKESEG-1)%MAXSNAKESEG)
#define SnakeClearSeg(G) {G.D=0;G.N=0;}

#define SnakeSegIsNull(G) (G.N==0)
#define SnakeIsNull(S) SnakeIsEmpty(S)

int SPG_CONV SnakeInit(SNAKE& S, int sz, int Flag, char* Name);
void SPG_CONV SnakeClose(SNAKE& S);

void SPG_CONV SnakeGetStatus(char* Msg, SNAKE& S);
SNAKEPTRPOS SPG_CONV SnakeGetPos(SNAKE S, SNAKESEG G, DWORD iD); //private
SNAKESEGPOS SPG_CONV SnakeGetPos(SNAKE S, SNAKESEG G0, SNAKESEG G1); //private
int SPG_CONV SnakeGetTotalLength(SNAKE& S); //slow
BYTE* SPG_CONV SnakeGetReadPtr(SNAKE& S, int N); //slow

#define SnakeGetElt(mS,mP,mT) (*(mT*)((BYTE*)mS.G[mP.index].D+mP.g*mS.sz)) //mS:SNAKE mP:SNAKEPTR, mT:Type pour le cast
#define SnakeGetPtr(mS,mP) (mS.G[mP.index].bD+mP.g*mS.sz)

void SPG_CONV SnakeAppend(SNAKE& S, void* D, int N);

int SPG_CONV SnakeIsEmpty(SNAKE& S);
int SPG_CONV SnakeLock(SNAKE& S, SNAKEPTR& P);

int SPG_CONV SnakeLock1(SNAKE& S, SNAKESEG& G0);//void* &D0,int N0,...
int SPG_CONV SnakeLock2(SNAKE& S, SNAKESEG& G0, SNAKESEG& G1);
int SPG_CONV SnakeLockLinear(SNAKE& C, SNAKESEG& G0);//void* &D0,int N0,...

int SPG_CONV SnakeIncSeg(SNAKE& mS, SNAKEPTR& mP);
int SPG_CONV SnakeAddPtr(SNAKE& mS, SNAKEPTR& mP, int& mN);

int SPG_CONV SnakeCommit(SNAKE& S, SNAKEPTR& P, SNAKE* C);
int SPG_CONV SnakeCommit(SNAKE& S, void* D, SNAKE* C);

#ifdef SPG_General_USESpinLock
int SPG_CONV SnakeRead1(SNAKE& S, SPINLOCK& LS, SNAKESEG& G0, int DataLen);
int SPG_CONV SnakeWrite1(SNAKE& C, SPINLOCK& LC, SNAKESEG& G0);
#endif

void SPG_CONV SnakeFlush(SNAKE& S, SNAKE& C);
int SPG_CONV SnakeDeleteSegAndGetNext(SNAKE& S, int i); //private
int SPG_CONV SnakeMergeSegAndGetNext(SNAKE& S, int i, int j); //private
void SPG_CONV SnakeReorder(SNAKE& S); //private
