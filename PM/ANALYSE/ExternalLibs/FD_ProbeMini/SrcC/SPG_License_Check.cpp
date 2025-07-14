
#ifdef NOHARDWARE
#define SAVE_ON_DESKTOP
#endif

#include <windows.h>

#ifdef SAVE_ON_DESKTOP
#include <shlobj.h>
#include <shlwapi.h>
#pragma comment(lib,"shlwapi.lib")
#endif

#include "SPG_General.h"

#ifdef SPG_General_USELICENSECHECK

#include "SPG_Includes.h"
//nclude "SPG_SysInc.h"

#include <stdio.h>
#include <stdlib.h>
#include <memory.h>

typedef struct
{
	DWORD A;
	DWORD B;
} BLOCK;

//http://en.wikipedia.org/wiki/Feistel_cipher

DWORD SPG_CONV FSTL_RoundFunction(DWORD B, int nRound)	{	return ((B*B)^B^(B<<(13+nRound))^(B>>(15+nRound)));	}

void SPG_CONV FSTL_Intermix(BLOCK* K, int nBlocks)	{	for(int i=0;i<nBlocks-1;i++)	{	V_SWAP(DWORD,K[i].B,K[i+1].A);	}	}

void SPG_CONV FSTL_InPlaceLoop(BLOCK* K, int nBlocks, int nRounds, bool Direction)
{
	CHECK((K==0)||(nBlocks<=0),"FSTL_InPlaceLoop",return);
	int n;
	for(n=0;n<nRounds;n++)
	{
		int N = Direction?n:(nRounds-1-n);

		if((!Direction)&&(N&1)) {FSTL_Intermix(K,nBlocks);}
		for(int i=0;i<nBlocks;i++)	{	K[i].A^=FSTL_RoundFunction(K[i].B,N);	}
		if((Direction)&&(N&1)) {FSTL_Intermix(K,nBlocks);}
		if(n!=(nRounds-1)) { for(int i=0;i<nBlocks;i++)	{	V_SWAP(DWORD,K[i].A,K[i].B);	} }
	}
	return;
}

BYTE* SPG_CONV FSTL_Proceed(int& n_out, BYTE* B, int n_in, bool Direction, int nRounds)
{
	CHECK((B==0)||(n_in<=0),"FSTL_Proceed",return 0);
	int nBlocks=(n_in+sizeof(BLOCK)-1)/sizeof(BLOCK); n_out=nBlocks*sizeof(BLOCK);
	BLOCK* K = SPG_TypeAlloc( nBlocks, BLOCK, "FSTL_AllocateLoop" );
	memcpy(K,B,n_in);
	FSTL_InPlaceLoop(K,nBlocks,nRounds,Direction);
	return (BYTE*)K;
}

BYTE* SPG_CONV FSTL_Mix(int& n, BYTE* B1, int n1, BYTE* B2, int n2)
{
	CHECK((B1==0)||(n1<=0)||(B2==0)||(n2<=0),"FSTL_Mix",return 0);
	n=sizeof(BLOCK)*((V_Min(n1,n2)+sizeof(BLOCK)-1)/sizeof(BLOCK));
	BYTE* B = SPG_MemAlloc( n, "FSTL_AllocateLoop" );
	for(int i=0;i<n;i++)	{	B[i] = ( (i<n1) ? B1[i]:0 ) ^ ( (i<n2) ? B2[i]:0 );	}
	return B;
}

#define ToASCII(b) ((b)<10?((b)+'0'):((b)+'A'-10))
#define ToBYTE(c) ((c)<'A'?((c)-'0'):((c&0x2F)-'A'+10))

char* SPG_CONV FSTL_ToStrZ(BYTE* B, int n, char* EOL, int LineLen)
{
	CHECK((B==0)||(n<=0),"FSTL_ToStrZ",return 0);
	int n_out=2*n;
	n_out+=strlen(EOL)*(V_Max(n-1,0)/LineLen);
	n_out++;//zero terminal
	char* B_out = (char*)SPG_MemAlloc( n_out, "FSTL_ToStrZ" );
	int j=0;
	for(int i=0;i<n;i++)	
	{	
		if(i&&((i%LineLen)==0))		{	for(int s=0;s<strlen(EOL);s++)	{	B_out[j++]=EOL[s];	}	}
		B_out[j++]=ToASCII(B[i]>>4);	B_out[j++]=ToASCII(B[i]&15);
	}
	B_out[j++]=0;
	DbgCHECK(j!=n_out,"FSTL_ToStrZ");
	return B_out;
}

BYTE* SPG_CONV FSTL_FromStrZ(int& n, char* c)
{
	CHECK(c==0,"FSTL_ToStrZ",return 0);
	BYTE* B=(BYTE*)SPG_MemAlloc((strlen(c)+1)/2,"FSTL_FromStrZ");
	n=0;
	for(;*c;c++)
	{
		int q=4*(1-(n&1));
		if(V_InclusiveBound(*c,'0','9')) {B[n/2]|=(*c-'0')<<q;n++;}
		if(V_InclusiveBound(*c,'a','f')) {B[n/2]|=(*c-'a'+10)<<q;n++;}
		if(V_InclusiveBound(*c,'A','F')) {B[n/2]|=(*c-'A'+10)<<q;n++;}
	}
	DbgCHECK(n&1,"FSTL_FromStrZ");
	n>>=1;
	return B;
}

void SPG_CONV FSTL_CopyToClipboard(void* hWnd, char* StrZ)
{
	CHECK(StrZ==0,"FSTL_CopyToClipboard",return);
	if(OpenClipboard((HWND)hWnd))
	{
	  if(EmptyClipboard())
	  {
		  BYTE* hData=(BYTE*)GlobalAlloc(GMEM_ZEROINIT|GMEM_MOVEABLE|GMEM_DDESHARE,1+strlen(StrZ));
		  if(hData)
		  {
				char* lpData=(char*)GlobalLock(hData);
				strcpy(lpData,StrZ);
				GlobalUnlock(hData);
				SetClipboardData( CF_TEXT, hData );
		  }
	  }
	  CloseClipboard();
	}
	return;
}

/*
// --------------------- HARDWARE --------------
HDD Serial, modele, etc ...
// --------------------- HARDWARE --------------

// --------------------- APPLICATION --------------
typedef struct { Hardware, CustomerName, software ID and version, ... } Sg_Decode
cKey (ascii-hex params.ini, parametres utilisateurs encodés)

Proceed, si -1 continue si 0 quitte
Decode typedef struct {Vendor ID, Options, } LICENCE_DATA

// --------------------- APPLICATION --------------

// --------------------- SrcC --------------
Proceed:
Sg_Encode=Encode(Sg_Decode)  //encode signature hardware
Key_Encode=ToBYTE(cKey) //parameters utilisateur encodés et mixés avec la clefs hardware
Key_Decode=Decode(Key_Encode)
Key_Data=Mix(KeyDecode,Sg_Encode) //recupere les parametres utilisateurs decodés; si la clef hardware correspond. On utilise un champ constant pour s'assurer que le décodage est correct, la technique produit un nombre de faux positifs qui depend du nombre de champs que l'on teste 

Key_Data valide -> retourne Data

Key_Data invalide -> retourne 0

// --------------------- SrcC --------------
*/




char* SPG_CONV QueryLicenseDataFromSgEncode(BYTE* Sg_Encode, int nSg_Encode, void* hWnd)
{

	//cKey
	char* cSignature = FSTL_ToStrZ(Sg_Encode,nSg_Encode,"\n",32);

	//DialogBox
	SPG_CONFIGFILE CFG; SPG_ZeroStruct(CFG); CFG_Init(CFG);

	CFG_StringParam(CFG,"Signature",cSignature);
	char* cKey_Encode=SPG_TypeAlloc(SPG_CONFIGSTRINGLEN,char,"QueryLicenseDataFromSgEncode");
	strcpy(cKey_Encode,"<paste key here - digits 0-9, A-F, a-f>");
	CFG_StringParam(CFG,"Key",cKey_Encode);

	CFG_ParamsFromPtr(CFG);
	if(CFG_CreateDialog(CFG)==IDOK) 
	{
		CFG_Close(CFG);
		SPG_MemFree(cSignature);
		return cKey_Encode;
	}
	SPG_MemFree(cKey_Encode);
	CFG_Close(CFG);

	//char* Clipboard=SPG_TypeAlloc(128+strlen(cSg), char, "LicenseQueryKey"); sprintf(Clipboard,"\r\n%s Software Key Code Request\r\nSoftware signature:\r\n%s\r\n",SPG_COMPANYNAME, cSg);
	FSTL_CopyToClipboard(hWnd, cSignature);
	//SPG_MemFree(cSg); SPG_MemFree(Clipboard);

	//MailTo
	char* cSg = FSTL_ToStrZ(Sg_Encode,nSg_Encode,"%0D%0A",32);
	char* MailTo=SPG_TypeAlloc(128+strlen(cSg), char, "LicenseQueryKey"); sprintf(MailTo,"<A HREF=\"mailto:s.petitgrand@fogale.fr?subject=%s Software Key Code Request&body=%%0D%%0ASoftware signature:%%0D%%0A%s%%0D%%0A%%0D%%0ASincerely%%0D%%0A\">%s Software Key Code Request</A>",SPG_COMPANYNAME,cSg,SPG_COMPANYNAME);
	SPG_MemFree(cSg);

	//MessageBox
	char* cMsg=SPG_TypeAlloc(128+strlen(MailTo), char, "LicenseQueryKey"); sprintf(cMsg,"%s Software Key Code Request\n\nSoftware signature:\n%s\n\nUse Ctrl+V to paste signature into email to s.petitgrand@fogale.fr\nOr refer to htm file on Desktop",SPG_COMPANYNAME,cSignature);
	//SPG_List(cMsg);
	MessageBox((HWND)hWnd,cMsg,"Software Key Code Request",MB_OK);
	SPG_MemFree(cMsg);
	
	//HTML
	char* RequestHTML=SPG_TypeAlloc(128+strlen(MailTo), char, "LicenseQueryKey"); sprintf(RequestHTML,"<HTML><TITLE>%s Software Key Code Request</TITLE><BODY><CENTER><BR>%s<BR><BR>%s<BR><BR>Please contact Fogale and transmit the hexadecimal signature above to obtain licence key by return<BR></CENTER></BODY></HTML>",SPG_COMPANYNAME,MailTo,cSignature);
	SPG_MemFree(MailTo);
	char Path[MaxProgDir]; strcpy(Path,Global.ProgDir);
#ifdef SAVE_ON_DESKTOP
#pragma SPGMSG(__FILE__,__LINE__,"SPG_License_Check uses shlwapi")

	IShellFolder* S;
	if(S_OK==SHGetDesktopFolder(&S))
	{
		STRRET strDispName;
		if(S_OK == (S->GetDisplayNameOf(0, SHGDN_NORMAL | SHGDN_FORPARSING, &strDispName)))
		{
			LPTSTR Name;
			if(S_OK == StrRetToStr(&strDispName,0,&Name))
			{
				strcpy(Path,Name);
			}
			CoTaskMemFree(Name);
		}
		S->Release();
	}
#endif
	SPG_ArrayStackAllocZ(char,KeyRequestFile,MaxProgDir); sprintf(KeyRequestFile,"%s\\%s_key_request.htm",Path,SPG_COMPANYNAME); //strncpy(KeyRequestFile,KeyFile,MaxProgDir-32); SPG_SetExtens(KeyRequestFile,"_request.htm");
	if(SPG_SaveFile(KeyRequestFile,(BYTE*)RequestHTML,strlen(RequestHTML)))
	{
		ShellExecute((HWND)hWnd,"open",KeyRequestFile,0,0,SW_SHOWNORMAL);
	}

	SPG_MemFree(RequestHTML);
	SPG_MemFree(cSignature);
	return 0;
}

char* SPG_CONV QueryLicenseDataFromSgDecode(BYTE* Sg_Decode, int nSg_Decode, void* hWnd)
{
	int nSg_Encode; BYTE* Sg_Encode=FSTL_Proceed(nSg_Encode, Sg_Decode,nSg_Decode,true,FSTL_ITER);

	char* cKey=QueryLicenseDataFromSgEncode(Sg_Encode,nSg_Encode,hWnd);

	SPG_MemFree(Sg_Encode);
	return cKey;
}

BYTE* SPG_CONV SetLicenseData(int& nKey_Encode, BYTE* Data, int& nData, BYTE* Sg, int SgLen)
{
	//sciOpenWriteFile(C, "SetLicenseData.txt");														ArrayBackupMe("BYTE* Data",Data,nData,C);								ArrayBackupMe("BYTE* Sg",Sg,SgLen,C);

	//Encode la signature du hardware
	int nSg_Encode; BYTE* Sg_Encode=FSTL_Proceed(nSg_Encode, Sg,SgLen,true,FSTL_ITER);																					//ArrayBackupMe("BYTE* Sg_Encode",Sg_Encode,nSg_Encode,C);
	DbgCHECK(nData>nSg_Encode,"SetLicenceData");

	//Cree la clef de licence (Data) par combinaison des données en clair avec la signature codée
	int nKey_Decode=0; BYTE* KeyB_Decode=FSTL_Mix(nKey_Decode, Data,nData, Sg_Encode,nSg_Encode);	SPG_MemFree(Sg_Encode);				//ArrayBackupMe("BYTE* KeyB_Decode",KeyB_Decode,nKey_Decode,C);

	//Encode la clef de license
	nKey_Encode=0; BYTE* KeyB_Encode=FSTL_Proceed(nKey_Encode, KeyB_Decode,nKey_Decode,true,FSTL_ITER);	SPG_MemFree(KeyB_Decode);		//ArrayBackupMe("BYTE* KeyB_Encode",KeyB_Encode,nKey_Encode,C);
	//scxClose(C);
	return KeyB_Encode;
}

int SPG_CONV SaveLicenseData(char* KeyFile, BYTE* Data, int nData, BYTE* Sg, int SgLen)
{
	int nKey_Encode=0; BYTE* KeyB_Encode=SetLicenseData(nKey_Encode, Data,nData, Sg,SgLen);
	SPG_SaveFile(KeyFile, KeyB_Encode,nKey_Encode);
	SPG_MemFree(KeyB_Encode);
	return -1;
}

BYTE* SPG_CONV GetLicenseData(int& nData, BYTE* KeyB_Encode,int nKey_Encode, BYTE* Sg, int SgLen)
{
	if(nKey_Encode==0) return 0;
	if(SgLen==0) return 0;
	//sciOpenWriteFile(C, "GetLicenseData.txt");

	//Encode la signature du hardware
	int nSg_Encode; BYTE* Sg_Encode=FSTL_Proceed(nSg_Encode, Sg,SgLen,true,FSTL_ITER);																					//ArrayBackupMe("BYTE* Sg_Encode",Sg_Encode,nSg_Encode,C);

	//Decode la clef de license
	int nKey_Decode=0; BYTE* KeyB_Decode=FSTL_Proceed(nKey_Decode, KeyB_Encode,nKey_Encode, false,FSTL_ITER);											//ArrayBackupMe("BYTE* KeyB_Decode",KeyB_Decode,nKey_Decode,C);
	if(KeyB_Decode==0) {nData=0; return 0; }
	DbgCHECK(nKey_Decode>nSg_Encode,"GetLicenceData");

	//Isole les donnéees de licence (Data) par combinaison avec la signature codée
	BYTE* Data=FSTL_Mix(nData, KeyB_Decode,nKey_Decode, Sg_Encode,nSg_Encode); SPG_MemFree(KeyB_Decode);	SPG_MemFree(Sg_Encode);		//ArrayBackupMe("BYTE* Data",Data,nData,C);

	//scxClose(C);
	return Data;
}

BYTE* SPG_CONV LoadLicenseData(int& nData, char* KeyFile, BYTE* Sg, int SgLen)
{
	int nKey_Encode=0; BYTE* KeyB_Encode=SPG_LoadFileAlloc(KeyFile, nKey_Encode);
	nData=0; BYTE* Data=GetLicenseData(nData, KeyB_Encode,nKey_Encode, Sg,SgLen);
	SPG_MemFree(KeyB_Encode);
	return Data;
}

int SPG_CONV LicenseTestValidate(char* KeyFile, BYTE* Data, int nData, BYTE* Sg, int SgLen)
{
	//int nKey_Encode=0; BYTE* KeyB_Encode = SetLicenseData(nKey_Encode, Data,nData, Sg,SgLen);
	SaveLicenseData(KeyFile,Data,nData,Sg,SgLen);

	//int nKey_Decode=0; BYTE* KeyB_Decode = GetLicenseData(nKey_Decode, KeyB_Encode,nKey_Encode, Sg,SgLen);
	int r_nData=0; BYTE* r_Data=LoadLicenseData(r_nData, KeyFile, Sg,SgLen);
	SPG_MemFree(r_Data);

	return -1;
}

typedef struct
{
	//DWORD dwVolumeSerialNumber;
	ULARGE_INTEGER TotalSpace;
	DWORD dwVolumeSerialNumber2;
	UINT Type;
	char VolumeName[20];
} LICENSE_HWSIG;

int SPG_CONV LicenseHardwareSignature(LICENSE_HWSIG& H, int hInstance)
{//manque la MAC-Address à ajouter ultérieurement
	char ModuleName[MaxProgDir];
	GetModuleFileName((HINSTANCE)hInstance,ModuleName,MaxProgDir-1);
/*
	HANDLE hE=CreateFile(ModuleName,0,FILE_SHARE_READ|FILE_SHARE_WRITE,0,OPEN_EXISTING,0,0);
	CHECK(hE==INVALID_HANDLE_VALUE,"LicenseHardwareSignature",return 0);

	BY_HANDLE_FILE_INFORMATION FI; SPG_ZeroStruct(FI);
	GetFileInformationByHandle(hE,&FI);
	CloseHandle(hE);

	H.dwVolumeSerialNumber=FI.dwVolumeSerialNumber;
*/
	CHECK(ModuleName[1]!=':',"LicenseHardwareSignature",return 0);	CHECK(ModuleName[2]!='\\',"LicenseHardwareSignature",return 0)
	char S[5];	S[0]=ModuleName[0];	S[1]=':';	S[2]='\\';	S[3]='.';	S[4]=0;

	H.Type=GetDriveType(S);
	memset(H.VolumeName,0,20);
	{
		DWORD MaxFilenameLen,Flags;
		UINT LastMode=SetErrorMode(SEM_FAILCRITICALERRORS);
		CHECK(GetVolumeInformation(S,H.VolumeName,17,&H.dwVolumeSerialNumber2,&MaxFilenameLen,&Flags,0,0)==0,"LicenseHardwareSignature",return 0);
		SetErrorMode(LastMode);
	}
	ULARGE_INTEGER AvailableSpace;//peut varier selon les droits utilisateur - à n'utiliser que sur support CD gravé pas sur HDD
	ULARGE_INTEGER FreeSpace;
	CHECK(GetDiskFreeSpaceEx(S,&AvailableSpace,&H.TotalSpace,&FreeSpace)==0,"LicenseHardwareSignature",return 0);
	return -1;
}

int SPG_CONV LicenseHardwaredwSignature(DWORD& dw_h, int hInstance)
{
	LICENSE_HWSIG H;
	SPG_ZeroStruct(H);
	CHECK(LicenseHardwareSignature(H,hInstance)==0,"LicenseHardwaredwSignature",return 0);
//BYTE* SPG_CONV FSTL_Proceed(int& n_out, BYTE* B, int n_in, bool Direction, int nRounds)
	int nh_out=0; BYTE* h_out = FSTL_Proceed(nh_out, (BYTE*)&H, sizeof(H),true,FSTL_ITER);

	dw_h=*(DWORD*)h_out;

	SPG_MemFree(h_out);
	return -1;
}

int SPG_CONV LicenseHardwarewSignature(WORD& w_h, int hInstance)
{
	LICENSE_HWSIG H;
	SPG_ZeroStruct(H);
	CHECK(LicenseHardwareSignature(H,hInstance)==0,"LicenseHardwaredwSignature",return 0);
//BYTE* SPG_CONV FSTL_Proceed(int& n_out, BYTE* B, int n_in, bool Direction, int nRounds)
	int nh_out=0; BYTE* h_out = FSTL_Proceed(nh_out, (BYTE*)&H, sizeof(H),true,FSTL_ITER);

	w_h=*(WORD*)h_out;

	SPG_MemFree(h_out);
	return -1;
}

#endif

