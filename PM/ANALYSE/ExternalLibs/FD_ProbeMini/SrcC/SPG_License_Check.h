

#define FSTL_ITER 7

//void SPG_CONV FSTL_InPlaceLoop(BLOCK* K, int nBlocks, int nRounds, bool Direction);
BYTE* SPG_CONV FSTL_Proceed(int& n_out, BYTE* B, int n_in, bool Direction, int nRounds);

BYTE* SPG_CONV FSTL_Mix(int& n, BYTE* B1, int n1, BYTE* B2, int n2);
char* SPG_CONV FSTL_ToStrZ(BYTE* B, int n, char* EOL, int LineLen);
BYTE* SPG_CONV FSTL_FromStrZ(int& n, char* c);
void SPG_CONV FSTL_CopyToClipboard(void* hWnd, char* StrZ);

char* SPG_CONV QueryLicenseDataFromSgEncode(BYTE* Sg_Encode, int nSg_Encode, void* hWnd);
char* SPG_CONV QueryLicenseDataFromSgDecode(BYTE* Sg_Decode, int nSg_Decode, void* hWnd);
BYTE* SPG_CONV SetLicenseData(int& nKey_Encode, BYTE* Data, int& nData, BYTE* Sg, int SgLen);
int SPG_CONV SaveLicenseData(char* KeyFile, BYTE* Data, int nData, BYTE* Sg, int SgLen);
BYTE* SPG_CONV GetLicenseData(int& nData, BYTE* KeyB_Encode,int nKey_Encode, BYTE* Sg, int SgLen);
BYTE* SPG_CONV LoadLicenseData(int& nData, char* KeyFile, BYTE* Sg, int SgLen);
int SPG_CONV LicenseTestValidate(char* KeyFile, BYTE* Data, int nData, BYTE* Sg, int SgLen);
//int SPG_CONV LicenseHardwareSignature(LICENSE_HWSIG& H, int hInstance);//signature complète
int SPG_CONV LicenseHardwaredwSignature(DWORD& dw_h, int hInstance);//signature raccourcie
int SPG_CONV LicenseHardwarewSignature(WORD& w_h, int hInstance);//signature raccourcie
