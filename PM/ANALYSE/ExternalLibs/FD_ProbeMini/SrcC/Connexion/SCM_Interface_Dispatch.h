
#ifndef SPG_General_USESnake
#error Define SPG_General_USESNAKE to use SCM_Interface_Dispatch
#endif

#define MAXDISPATCHFCT 32
#define MAXDISPATCHSCX 32

typedef int (__cdecl *SCX_DISPATCH_CLBK)(SNAKESEG G[MAXSNAKESEG], int N, void* User, SCX_WRITE w);

typedef struct
{

	SPINLOCK L;

	int Size;
	void* Buffer[MAXDISPATCHSCX];
	SNAKE Msg[MAXDISPATCHSCX];
	SNAKE Free[MAXDISPATCHSCX];
	int NumC;
	SCX_CONNEXION* C[MAXDISPATCHSCX];

	int MaxDispatchFct;
	int NumDispatchFct;
	SCX_DISPATCH_CLBK scxDispatchFct[MAXDISPATCHFCT];
	void* scxDispatchUser[MAXDISPATCHFCT];

} SCX_DISPATCH_CONNEXION;

SCX_DISPATCH_CONNEXION* SPG_CONV scxDispatchOpen();
int SPG_CONV scxClose(SCX_DISPATCH_CONNEXION* &CP);

int SPG_CONV scxWrite(void* Data, int DataLen, SCX_DISPATCH_CONNEXION* CP);
int SPG_CONV scxReadLock(SNAKESEG G[MAXSNAKESEG], int& N, SCX_DISPATCH_CONNEXION* CP, char* Owner);
void SPG_CONV scxReadUnock(SNAKESEG G[MAXSNAKESEG], void* D, SCX_DISPATCH_CONNEXION* CP);
void SPG_CONV scxReadUnock(SNAKESEG G[MAXSNAKESEG], SNAKEPTR& P, SCX_DISPATCH_CONNEXION* CP);
int SPG_CONV scxSetDispatchFct(SCX_DISPATCH_CONNEXION* CP, SCX_DISPATCH_CLBK scxDispatchFct, void* scxDispatchUser);
void SPG_CONV scxRemoveDispatchFct(SCX_DISPATCH_CONNEXION* CP, SCX_DISPATCH_CLBK scxDispatchFct, void* scxDispatchUser);
void SPG_CONV scxDispatch(SCX_DISPATCH_CONNEXION* CP);


