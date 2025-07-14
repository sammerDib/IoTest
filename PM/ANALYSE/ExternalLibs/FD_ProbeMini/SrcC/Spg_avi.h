
#ifdef SPG_General_USEAVI

#define SPG_PAVIFILE void*
#define SPG_PAVISTREAM void*

typedef struct
{
	int Etat;
	int AviSizeX;
	int AviSizeY;
	int AviPitch;
	int AviPOCT;
	int FPS;
	float DT;
	int YReverse;
	int POCT;
	int FrameSize;
	int FrameNumber;
	SPG_PAVIFILE pfile;
	SPG_PAVISTREAM pavi;
	SPG_PAVISTREAM psCompressed;
	void* binfo;
	BYTE* OneFrame;
	void* GFrame;
} SPG_AVI_STREAM;

int SPG_CONV SPG_AVI_StreamSave_Start(SPG_AVI_STREAM& SAS, char* FileName, int SizeX, int SizeY, int POCT, int FPS, int YReverse=1, int hWnd=0);
void SPG_CONV SPG_AVI_StreamSave_Stop(SPG_AVI_STREAM& SAS);
int SPG_CONV SPG_AVI_StreamSave_AddFrame(SPG_AVI_STREAM& SAS, BYTE* E, int Pitch);
int SPG_CONV SPG_AVI_Save(char* FileName, BYTE* E, int SizeX, int SizeY, int Pitch, int SizeP, int POCT, int NumS, int FPS, int YReverse=1, int hWnd=0);
int SPG_CONV SPG_AVI_StreamLoad_Start(SPG_AVI_STREAM& SAS, char* FileName, int& SizeX, int& SizeY, int& POCT, int& FPS, int& NumS, int YReverse=1);
void SPG_CONV SPG_AVI_StreamLoad_Stop(SPG_AVI_STREAM& SAS);
int SPG_CONV SPG_AVI_StreamLoad_AddFrame(SPG_AVI_STREAM& SAS, BYTE* E, int Pitch);
int SPG_CONV SPG_AVI_Load(char* FileName, BYTE*& E, int& SizeX, int& SizeY, int& POCT, int& NumS, int& FPS, int YReverse=1);

#endif

