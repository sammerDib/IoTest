
#ifdef SPG_General_USERINGREC

int SPG_CONV RGR_Load(RING_REC& RGR, char* FileName);
int SPG_CONV RGR_M3DLoad(RING_REC& RGR, char* M3DFileName, WORD* *StartIndex=0);
int SPG_CONV RGR_M3DLoadAndConvert(RING_REC& RGR, char* M3DFileName);
int SPG_CONV RGR_Save(RING_REC& RGR, char* FileName);
int SPG_CONV RGR_M3DSave(RING_REC& RGR, char* M3DFileName);
int SPG_CONV M3D_RGR_Load(M3D_RGR& RGR, char* FileName);
void SPG_CONV M3D_RGR_Close(M3D_RGR& RGR);

#endif

