
#ifdef SPG_General_USEINTERACT3D

typedef void Interact3D_Element;
typedef Interact3D_Element* Interact3D_ElementRef;

typedef struct
{
	int NumElements;
	Interact3D_ElementRef* Elements;
} Interact3D_Dispatch;

typedef struct
{
	float DistanceCarre;
	float DeltaX;
	float DeltaY;
	float DeltaZ;
	Interact3D_ElementRef Element;
} Interact3D_ElementList;

typedef struct
{
	int MaxElements;
	int NumElements;
	Interact3D_ElementList* ElementList;
} Interact3D_List;

typedef struct
{
	int NumElements;
	Interact3D_Element* Elements;
	int ElementSize;
	int XOffset;
	int YOffset;
	int ZOffset;
	float XMin;
	float YMin;
	float ZMin;
	float iXMin;
	float iYMin;
	float iZMin;
	float XSize;
	float YSize;
	float ZSize;
	int SizeX;
	int SizeY;
	int SizeZ;
	int Active;
	int MaxDispatchElements;
	Interact3D_Dispatch* Dispatch;
	PixCoul* Color;
} Interact3D;

#define GET_LIST_MODE_ALL 0
#define GET_LIST_MODE_LOWER 1
#define GET_LIST_MODE_UPPER 2


#define IT3DEfromI(it3d,I) ((BYTE*)it3d.Elements)+it3d.ElementSize*I

#define IT3DXfromI(it3d,I) (*(float*)(IT2DEfromI(it3d,I)+it3d.XOffset))
#define IT3DYfromI(it3d,I) (*(float*)(IT2DEfromI(it3d,I)+it3d.YOffset))
#define IT3DZfromI(it3d,I) (*(float*)(IT2DEfromI(it3d,I)+it3d.ZOffset))

#define IT3DX(it3d,E) (*(float*)(((BYTE*)(E))+it3d.XOffset))
#define IT3DY(it3d,E) (*(float*)(((BYTE*)(E))+it3d.YOffset))
#define IT3DZ(it3d,E) (*(float*)(((BYTE*)(E))+it3d.ZOffset))

#define IT3DN_X(it3d,x) V_Floor((x-it3d.iXMin)/it3d.XSize)
#define IT3DN_Y(it3d,y) V_Floor((y-it3d.iYMin)/it3d.YSize)
#define IT3DN_Z(it3d,z) V_Floor((z-it3d.iZMin)/it3d.ZSize)

#define IT3DNXfromI(it3d,I) IT3DN_X(it3d,IT3DXfromI(it3d,I))
#define IT3DNYfromI(it3d,I) IT3DN_Y(it3d,IT3DYfromI(it3d,I))
#define IT3DNZfromI(it3d,I) IT3DN_Y(it3d,IT3DZfromI(it3d,I))

#define IT3DNX(it3d,E) IT3DN_X(it3d,IT3DX(it3d,E))
#define IT3DNY(it3d,E) IT3DN_Y(it3d,IT3DY(it3d,E))
#define IT3DNZ(it3d,E) IT3DN_Z(it3d,IT3DZ(it3d,E))

int SPG_CONV Interact3D_Init(Interact3D& it3d, Interact3D_Element* Elements, int NumElements, int XOffset, int YOffset, int ZOffset, int ElementSize, float XMin, float YMin, float ZMin, float XSize, float YSize, float ZSize, int SizeX, int SizeY, int SizeZ, int MaxDispatchElements);
int SPG_CONV Interact3D_InitAuto(Interact3D& it3d, Interact3D_Element* Elements, int NumElements, int XOffset, int YOffset, int ElementSize, float InteractionRadius);
int SPG_CONV Interact3D_InitList(Interact3D& it3d, Interact3D_List& List);
void SPG_CONV Interact3D_CloseList(Interact3D_List& List);
void SPG_CONV Interact3D_Fill(Interact3D& it3d);
void SPG_CONV Interact3D_Draw(Interact3D& it3d, G_Ecran& E, int ProjPlane=0);
void SPG_CONV Interact3D_Close(Interact3D& it3d);
void SPG_CONV Interact3D_GetInteractionList(Interact3D& it3d, Interact3D_List& List, int ElementNumber, float InteractionRadius);

#endif
