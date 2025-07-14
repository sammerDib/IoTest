

typedef struct
{
	float* d;
	float w;
} P_WEIGHTEDPOS;

#define P_ILEN 64

typedef struct
{
	//int MaxPos; //=P_ILEN
	int NumPos;
	int Flag;
	P_WEIGHTEDPOS WP[P_ILEN];
} P_ILIST;

typedef struct
{
	Profil* Pdst;
	Profil* Psrc;
	P_ILIST* L;
	//P_ILIST flyingList
} P_ILISTSTRUCT;

#define PIList_OrientX 1
#define PIList_OrientY 2
#define PIList_OrientT 4

void SPG_CONV P_CreateIList(Profil* Pdst, Profil* Psrc, P_ILISTSTRUCT &LS);
void SPG_CONV P_DestroyIList(P_ILISTSTRUCT &LS);
void SPG_CONV P_SetMIList(P_ILISTSTRUCT& LS);
void SPG_CONV P_IListFlip(P_ILISTSTRUCT& LS, int Orientation);
void SPG_CONV P_IListNormalize(P_ILISTSTRUCT& LS);
void SPG_CONV P_ComputeIList(P_ILISTSTRUCT& LS);
