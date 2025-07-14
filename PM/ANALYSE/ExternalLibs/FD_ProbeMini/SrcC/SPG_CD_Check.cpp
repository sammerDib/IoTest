
#include "SPG_General.h"

#ifdef SPG_General_USECDCHECK

#include "SPG_Includes.h"

#include <stdio.h>
#include <stdlib.h>

#define FOR_ALL_0_3(Expr1,Expr2) Expr1##0##Expr2;Expr1##1##Expr2;Expr1##2##Expr2;Expr1##3##Expr2
#define FOR_ALL_4_7(Expr1,Expr2) Expr1##4##Expr2;Expr1##5##Expr2;Expr1##6##Expr2;Expr1##7##Expr2
#define FOR_ALL_8_11(Expr1,Expr2) Expr1##8##Expr2;Expr1##9##Expr2;Expr1##10##Expr2;Expr1##11##Expr2
#define FOR_ALL_12_15(Expr1,Expr2) Expr1##12##Expr2;Expr1##13##Expr2;Expr1##14##Expr2;Expr1##15##Expr2
#define FOR_ALL_16_19(Expr1,Expr2) Expr1##16##Expr2;Expr1##17##Expr2;Expr1##18##Expr2;Expr1##19##Expr2
#define FOR_ALL_20_23(Expr1,Expr2) Expr1##20##Expr2;Expr1##21##Expr2;Expr1##22##Expr2;Expr1##23##Expr2
#define FOR_ALL_24_27(Expr1,Expr2) Expr1##24##Expr2;Expr1##25##Expr2;Expr1##26##Expr2;Expr1##27##Expr2
#define FOR_ALL_28_31(Expr1,Expr2) Expr1##28##Expr2;Expr1##29##Expr2;Expr1##30##Expr2;Expr1##31##Expr2
#define FOR_ALL_0_31(Expr1,Expr2) FOR_ALL_0_3(Expr1,Expr2);FOR_ALL_4_7(Expr1,Expr2);FOR_ALL_8_11(Expr1,Expr2);FOR_ALL_12_15(Expr1,Expr2);FOR_ALL_16_19(Expr1,Expr2);FOR_ALL_20_23(Expr1,Expr2);FOR_ALL_24_27(Expr1,Expr2);FOR_ALL_28_31(Expr1,Expr2)

#define CRYPTE_Internal_4(Dest,Src) {BYTE B=0;for(int CRYPTE_MACRO_INDEX=0;CRYPTE_MACRO_INDEX<256;CRYPTE_MACRO_INDEX++) {B=Dest[(CRYPTE_MACRO_INDEX+B+Src[(CRYPTE_MACRO_INDEX-B)&3])&31]-=(1^Src[(CRYPTE_MACRO_INDEX+B+Dest[(CRYPTE_MACRO_INDEX-B)&31])&3]);}}
#define CRYPTE_Internal_8(Dest,Src) {BYTE B=0;for(int CRYPTE_MACRO_INDEX=0;CRYPTE_MACRO_INDEX<256;CRYPTE_MACRO_INDEX++) {B=Dest[(CRYPTE_MACRO_INDEX+B+Src[(CRYPTE_MACRO_INDEX-B)&7])&31]-=(1^Src[(CRYPTE_MACRO_INDEX+B+Dest[(CRYPTE_MACRO_INDEX-B)&31])&7]);}}
#define CRYPTE_Internal_16(Dest,Src) {BYTE B=0;for(int CRYPTE_MACRO_INDEX=0;CRYPTE_MACRO_INDEX<256;CRYPTE_MACRO_INDEX++) {B=Dest[(CRYPTE_MACRO_INDEX+B+Src[(CRYPTE_MACRO_INDEX-B)&15])&31]-=(1^Src[(CRYPTE_MACRO_INDEX+B+Dest[(CRYPTE_MACRO_INDEX-B)&31])&15]);}}

#define CRYPTE_DW(CA32,DW) CRYPTE_Internal_4(CA32.B,((BYTE*)&DW))
#define CRYPTE_DD(CA32,DD) CRYPTE_Internal_8(CA32.B,((BYTE*)&DD))
#define CRYPTE_D16P(CA32,D16P) CRYPTE_Internal_16(CA32.B,((BYTE*)D16P))

void SPG_CONV CD_CheckFile(CRYPTED_ARRAY_32& CD_UID, char* FullName)
{
	memset(&CD_UID,0,sizeof(CRYPTED_ARRAY_32));
	//char SN[1024];
	//strcpy(SN,FullName);
	char S[5];
	S[1]=':';
	S[2]='\\';
	S[3]='.';
	S[4]=0;
	BY_HANDLE_FILE_INFORMATION FI;
	HANDLE hE=CreateFile(FullName,0,FILE_SHARE_READ|FILE_SHARE_WRITE,0,OPEN_EXISTING,0,0);
	if(hE==INVALID_HANDLE_VALUE) return;
	GetFileInformationByHandle(hE,&FI);
	CloseHandle(hE);
	if(FullName[1]!=':') return;
	if(FullName[2]!='\\') return;
	S[0]=FullName[0];
	DWORD Type=GetDriveType(S);
	char VolumeName[17];
	memset(VolumeName,0,17);
	{
		DWORD Serial,MaxFilenameLen,Flags;
		UINT LastMode=SetErrorMode(SEM_FAILCRITICALERRORS);
		if(GetVolumeInformation(S,VolumeName,17,&Serial,&MaxFilenameLen,&Flags,0,0)==0) return;
		SetErrorMode(LastMode);
	}
	ULARGE_INTEGER AvailableSpace;//peut varier selon les droits utilisateur
	ULARGE_INTEGER TotalSpace;
	ULARGE_INTEGER FreeSpace;
	if(GetDiskFreeSpaceEx(S,&AvailableSpace,&TotalSpace,&FreeSpace)==0) return;
/*
	char S1[32];
	sprintf(S1,"\nType %.08X\n",Type);
	char S2[32];
	sprintf(S2,"FI.Serial %.08X\n",FI.dwVolumeSerialNumber);
	char S3[32];
	sprintf(S3,"FI.FileSize %.08X\n",FI.nFileSizeLow);
	char S4[32];
	sprintf(S4,"Serial %.08X\n",Serial);
	char S5[32];
	sprintf(S5,"Total space %.08X%.08X\n",TotalSpace.HighPart,TotalSpace.LowPart);
	char S6[32];
	sprintf(S6,"FreeSpace %.08X%.08X\n",FreeSpace.HighPart,FreeSpace.LowPart);
	strcat(SN,S1);
	strcat(SN,S2);
	strcat(SN,S3);
	strcat(SN,S4);
	strcat(SN,S5);
	strcat(SN,S6);
	MessageBox(0,SN,"CHECK",0);
*/
	CRYPTE_DW(CD_UID,Type);
	//CRYPTE_DW(CD_UID,FI.dwVolumeSerialNumber);
	CRYPTE_DW(CD_UID,FI.nFileSizeLow);
	//CRYPTE_DW(CD_UID,Serial);
	CRYPTE_D16P(CD_UID,VolumeName);
	CRYPTE_DD(CD_UID,TotalSpace);
	CRYPTE_DD(CD_UID,FreeSpace);
	return;
}


FOR_ALL_0_31(void __stdcall CD_Exit,(void) {exit(0);})

#endif
