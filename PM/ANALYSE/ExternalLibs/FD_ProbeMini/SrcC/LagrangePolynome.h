
typedef DWORD PTI; //anciennement __int64, à tester
//le plus petit premier apres 2^32 : 4294967311
//le plus petit grand avant 2^31 : 2147483647

typedef struct
{
	int n;
	PTI* a;
} PolyN;

void PolyN_Create(PolyN& p, int n);
void PolyN_Close(PolyN& p);

typedef PolyN LControlNodes;
#define LControlNodes_Create PolyN_Create
#define LControlNodes_Close PolyN_Close

void PolyN_CreateLagrange(PolyN& M, LControlNodes& C, int Item);
PTI Eval(PolyN& p, PTI x);
