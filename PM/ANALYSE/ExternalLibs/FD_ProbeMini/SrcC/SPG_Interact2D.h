
#ifdef SPG_General_USEINTERACT2D

typedef void Interact2D_Element;
typedef Interact2D_Element* Interact2D_ElementRef;

typedef struct
{
	int NumElements;
	Interact2D_ElementRef* Elements;
} Interact2D_Dispatch;

typedef struct
{
	float DistanceCarre;
	float DeltaX;
	float DeltaY;
	Interact2D_ElementRef Element;
} Interact2D_ElementList;

typedef struct
{
	int MaxElements;
	int NumElements;
	Interact2D_ElementList* ElementList;
} Interact2D_List;

typedef struct
{
	int NumElements;
	Interact2D_Element* Elements;
	int ElementSize;
	int XOffset;
	int YOffset;
	float XMin;
	float YMin;
	float iXMin;
	float iYMin;
	float XSize;
	float YSize;
	int SizeX;
	int SizeY;
	int Active;
	int MaxDispatchElements;
	Interact2D_Dispatch* Dispatch;
} Interact2D;

#define GET_LIST_MODE_ALL 0
#define GET_LIST_MODE_LOWER 1
#define GET_LIST_MODE_UPPER 2


#define IT2DEfromI(it2d,I) ((BYTE*)it2d.Elements)+it2d.ElementSize*I

#define IT2DXfromI(it2d,I) (*(float*)(IT2DEfromI(it2d,I)+it2d.XOffset))
#define IT2DYfromI(it2d,I) (*(float*)(IT2DEfromI(it2d,I)+it2d.YOffset))

#define IT2DX(it2d,E) (*(float*)(((BYTE*)(E))+it2d.XOffset))
#define IT2DY(it2d,E) (*(float*)(((BYTE*)(E))+it2d.YOffset))

#define IT2DN_X(it2d,x) V_Floor((x-it2d.iXMin)/it2d.XSize)
#define IT2DN_Y(it2d,y) V_Floor((y-it2d.iXMin)/it2d.XSize)

#define IT2DNXfromI(it2d,I) IT2DN_X(it2d,IT2DXfromI(it2d,I))
#define IT2DNYfromI(it2d,I) IT2DN_Y(it2d,IT2DYfromI(it2d,I))

#define IT2DNX(it2d,E) IT2DN_X(it2d,IT2DX(it2d,E))
#define IT2DNY(it2d,E) IT2DN_Y(it2d,IT2DY(it2d,E))

int SPG_CONV Interact2D_Init(Interact2D& it2d, Interact2D_Element* Elements, int NumElements, int XOffset, int YOffset, int ElementSize, float XMin, float YMin, float XSize, float YSize, int SizeX, int SizeY, int MaxDispatchElements);
int SPG_CONV Interact2D_InitAuto(Interact2D& it2d, Interact2D_Element* Elements, int NumElements, int XOffset, int YOffset, int ElementSize, float InteractionRadius);
int SPG_CONV Interact2D_InitList(Interact2D& it2d, Interact2D_List& List);
void SPG_CONV Interact2D_CloseList(Interact2D_List& List);
void SPG_CONV Interact2D_Fill(Interact2D& it2d);
void SPG_CONV Interact2D_Draw(Interact2D& it2d, G_Ecran& E);
void SPG_CONV Interact2D_Close(Interact2D& it2d);
void SPG_CONV Interact2D_GetInteractionList(Interact2D& it2d, Interact2D_List& List, int ElementNumber, float InteractionRadius);

#endif
