
#include "SPG_General.h"

#ifdef SPG_General_USECut

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#define Cut_DefaultIndex 0

#define LoadFilterTXT "Texte\0*.txt\0"
#define SaveFilterTXT LoadFilterTXT
#define Cut_TXT 1
#define Cut_TXTlfIndex 1
#define Cut_TXTsfIndex 1

//definition du type PGC
//(type non supporte en lecture)
#define LoadFilterPGC "PG Cut\0*.pgc\0"
#define SaveFilterPGC LoadFilterPGC
//formats etendus
#define Cut_PGC 2
#define Cut_PGClfIndex 2
#define Cut_PGCsfIndex 2

//definition du type raw float
#define LoadFilterRF "Raw float\0*.*\0"
#define SaveFilterRF "Raw float\0*.rf\0"
//formats etendus
#define Cut_RAWFLOAT 3
#define Cut_RAWFLOATlfIndex 3
#define Cut_RAWFLOATsfIndex 3

//definition du type raw short
//(type non supporte en lecture)
#define LoadFilterRS "Raw short\0*.*\0"
#define SaveFilterRS "Raw short\0*.rs\0"
//formats etendus
#define Cut_RAWSHORT 4
#define Cut_RAWSHORTlfIndex 4
#define Cut_RAWSHORTsfIndex 4

//definition du type raw short unsigned
//(type non supporte en lecture)
#define LoadFilterRUS "Raw unsigned short\0*.*\0"
#define SaveFilterRUS "Raw unsigned short\0*.rus\0"
//formats etendus
#define Cut_RAWUSHORT 5
#define Cut_RAWUSHORTlfIndex 5
#define Cut_RAWUSHORTsfIndex 5

//definition du type raw short unsigned
//(type non supporte en lecture)
#define LoadFilterRB "Raw byte\0*.*\0"
#define SaveFilterRB "Raw byte\0*.rb\0"
//formats etendus
#define Cut_RAWBYTE 6
#define Cut_RAWBYTElfIndex 6
#define Cut_RAWBYTEsfIndex 6

#define LoadFilterWAV "Wave PCM 16 bits\0*.wav\0"
#define SaveFilterWAV "Wave PCM 16 bits\0*.wav\0"
//formats etendus
#define Cut_WAV 7
#define Cut_WAVlfIndex 7
#define Cut_WAVsfIndex 7

#define LoadFilterCUT "Coupe CUT M3D\0*.cut\0"
#define SaveFilterCUT "Coupe CUT M3D\0*.cut\0"
//formats etendus
#define Cut_CUT 8
#define Cut_CUTlfIndex 8
#define Cut_CUTsfIndex 8

#ifdef SPG_General_USEPRO
#include "PRO_Loader.h"
#define LoadFilterPRO "Digital Surf\0*.pro\0"
#define SaveFilterPRO LoadFilterPRO
#define Cut_PRO 9
#define Cut_PROlfIndex 9
#define Cut_PROsfIndex 9
#else
#define LoadFilterPRO
#define SaveFilterPRO
#endif

#include <stdio.h>
#include <string.h>

#define Cut_RET "\r\n"

int SPG_CONV Cut_GetExtens(const char *FullName)
{
	int l;
	l=strlen(FullName);
	if (l==0) return 0;

	int i;
	for(i=l;i>0;i--)
	{
		if(FullName[i]=='.') break;
	}
	if ((i==0)||(i==l)||(i==(l-1)))
	{
		return 0;
	}
	if (_stricmp(FullName+i,".txt")==0) return Cut_TXT;
	if (_stricmp(FullName+i,".pgc")==0) return Cut_PGC;
	if (_stricmp(FullName+i,".rf")==0) return Cut_RAWFLOAT;
	if (_stricmp(FullName+i,".rs")==0) return Cut_RAWSHORT;
	if (_stricmp(FullName+i,".rus")==0) return Cut_RAWUSHORT;
	if (_stricmp(FullName+i,".rb")==0) return Cut_RAWBYTE;
	if (_stricmp(FullName+i,".wav")==0) return Cut_WAV;
	if (_stricmp(FullName+i,".cut")==0) return Cut_CUT;
#ifdef SPG_General_USEHDF
	if (_stricmp(FullName+i,".pro")==0) return Cut_PRO;
#endif
	return 0;
}

#ifdef SPG_General_USENetwork_Protocol
int SPG_CONV Cut_NetHook_Load(SPG_NET_PROTOCOL& SNP, SPG_NET_ADDR& SNA)
{
	SPG_StackAllocZ(Cut,C);
	if((Cut_InternalLoad(C,(char*)(SNP.Message.M))==0)||(Cut_Etat(C)==0)) 
	{
		SNP_Send_BYTE(SNP,SNA,SNP_BREAK,0);
		return 0;
	}
	CHECK(SNP_Send(SNP,SNA,SNP_C_STRUCT,&C,sizeof(Cut))==0,"Cut_NetHook_Load: Envoi echoue",Cut_Close(C);return 0);
	CHECK(SPG_Download_Send(SNP,SNA,SNP_C_DATA_SEND,SNP_C_DATA_RESPONSE,Cut_Data(C),Cut_NumS(C)*sizeof(float))==0,"Cut_NetHook_Load: Envoi echoue",Cut_Close(C);return 0);
	if(Cut_Msk(C)) CHECK(SPG_Download_Send(SNP,SNA,SNP_C_MSK_SEND,SNP_C_MSK_RESPONSE,Cut_Msk(C),Cut_NumS(C))==0,"Cut_NetHook_Load: Envoi echoue",Cut_Close(C);return 0);
	Cut_Close(C);
	return -1;
}

int SPG_CONV Cut_NetLoad(SPG_NET_PROTOCOL& SNP, SPG_NET_ADDR& SNA, Cut& C, char * SuggestedName)
{
	memset(&C,0,sizeof(Cut));
	CHECK(SNP_Send_char(SNP,SNA,SNP_C_STARTLOADCUT,SuggestedName)==0,"Cut_NetLoad: Envoi echoue",return 0);
	CHECK(SPG_NetworkDoEvents_LongWait(SNP,SNA,SNP_C_STRUCT)==0,"Cut_NetLoad: Pas de reponse",SNP_Send_BYTE(SNP,SNA,SNP_BREAK,1);return 0);
	CHECK(SNP_IsLen(SNP,sizeof(Cut))==0,"Cut_NetLoad: Taille recue imprevue",SNP_Send_BYTE(SNP,SNA,SNP_BREAK,1);return 0);
	memcpy(&C,SNP.Message.M,sizeof(Cut));
	if(Cut_Etat(C))
	{
		if(Cut_Create(C,Cut_NumS(C),Cut_XScale(C),Cut_UnitX(C),Cut_UnitY(C)))
		{
			CHECK(SPG_Download_Read(SNP,SNA,SNP_C_DATA_SEND,SNP_C_DATA_RESPONSE,Cut_Data(C),Cut_NumS(C)*sizeof(float))==0,"Cut_NetLoad: Envoi echoue",return 0);
			if(Cut_Msk(C)) CHECK(SPG_Download_Read(SNP,SNA,SNP_C_MSK_SEND,SNP_C_MSK_RESPONSE,Cut_Msk(C),Cut_NumS(C))==0,"Cut_NetLoad: Envoi echoue",return 0);
			return -1;
		}
		else
		{
			SNP_Send_BYTE(SNP,SNA,SNP_BREAK,1);
			return 0;
		}
	}
	else
		return 0;
}

int SPG_CONV Cut_NetHook_Save(SPG_NET_PROTOCOL& SNP, SPG_NET_ADDR& SNA)
{
	SPG_StackAllocZ(Cut,C);
	char SuggestedName[MaxProgDir];
	strcpy(SuggestedName,(char*)(SNP.Message.M));
	CHECK(SPG_Network_DoEvents(SNP,SNA,SNP_C_STRUCT)==0,"Cut_NetHook_Save: Pas de reponse",SNP_Send_BYTE(SNP,SNA,SNP_BREAK,1);return 0);
	CHECK(SNP_IsLen(SNP,sizeof(Cut))==0,"Cut_NetHook_Save: Taille recue imprevue",SNP_Send_BYTE(SNP,SNA,SNP_BREAK,1);return 0);
	memcpy(&C,SNP.Message.M,sizeof(Cut));
	if(Cut_Etat(C))
	{
		if(Cut_Create(C,Cut_NumS(C),Cut_XScale(C),Cut_UnitX(C),Cut_UnitY(C)))
		{
			CHECK(SPG_Download_Read(SNP,SNA,SNP_C_DATA_SEND,SNP_C_DATA_RESPONSE,Cut_Data(C),Cut_NumS(C)*sizeof(float))==0,"Cut_NetHook_Save: Envoi echoue",Cut_Close(C);return 0);
			if(Cut_Msk(C)) CHECK(SPG_Download_Read(SNP,SNA,SNP_C_MSK_SEND,SNP_C_MSK_RESPONSE,Cut_Msk(C),Cut_NumS(C))==0,"Cut_NetHook_Save: Envoi echoue",Cut_Close(C);return 0);
			CHECK(Cut_InternalSave(C,SuggestedName)==0,"Cut_NetHook_Save: Sauvegarde echouee",Cut_Close(C);return 0);
			Cut_Close(C);
			return -1;
		}
		else
		{
			SNP_Send_BYTE(SNP,SNA,SNP_BREAK,1);
			return 0;
		}
	}
	else
		return 0;
}

int SPG_CONV Cut_NetSave(SPG_NET_PROTOCOL& SNP, SPG_NET_ADDR& SNA, Cut& C, char * SuggestedName)
{
	CHECK(SNP_Send_char(SNP,SNA,SNP_C_STARTSAVECUT,SuggestedName)==0,"Cut_NetSave: Envoi echoue",return 0);
	CHECK(SNP_Send(SNP,SNA,SNP_C_STRUCT,&C,sizeof(Cut))==0,"Cut_NetSave: Envoi echoue",return 0);
	if(Cut_Etat(C))
	{
		CHECK(SPG_Download_Send(SNP,SNA,SNP_C_DATA_SEND,SNP_C_DATA_RESPONSE,Cut_Data(C),Cut_NumS(C)*sizeof(float))==0,"Cut_NetSave: Envoi echoue",return 0);
		if(Cut_Msk(C)) CHECK(SPG_Download_Send(SNP,SNA,SNP_C_MSK_SEND,SNP_C_MSK_RESPONSE,Cut_Msk(C),Cut_NumS(C))==0,"Cut_NetSave: Envoi echoue",return 0);
	}
	return -1;
}
#endif

#ifdef SPG_General_USEFilesWindows
int SPG_CONV Cut_Load(Cut& C, const char * SuggestedName)
{
#ifdef SPG_General_USEGlobal
#ifdef SPG_General_USENetwork_Protocol
	if(SPG_UnderNetControl) 
		return Cut_NetLoad(*(Global.SNP),Global.ControlSource,C,SuggestedName);
	else
#endif
#endif
		return Cut_InternalLoad(C,SuggestedName);
}

UINT CALLBACK Cut_InternalGenericHookProc(
  HWND hdlg,      // handle to child dialog window
  UINT uiMsg,     // message identifier
  WPARAM wParam,  // message parameter
  LPARAM lParam   // message parameter
)
{
	return 0;
}

int SPG_CONV Cut_InternalLoad(Cut& C, const char * SuggestedName)
{
	memset(&C,0,sizeof(Cut));

	SPG_StackAllocZ(OPENFILENAME,OFN);

	char ResultFile[MaxProgDir];
	strcpy(ResultFile,SuggestedName);

	OFN.lStructSize=sizeof(OPENFILENAME);
#ifdef SPG_General_USEGlobal
	OFN.hwndOwner=(HWND)Global.hWndWin;
	OFN.hInstance=0;
#endif
	OFN.lpstrFilter=LoadFilterTXT LoadFilterPGC LoadFilterRF LoadFilterRS LoadFilterRUS LoadFilterRB LoadFilterWAV LoadFilterCUT LoadFilterPRO"\0";
	int i=Cut_GetExtens(ResultFile); if(i==0) i=Cut_DefaultIndex;
	OFN.nFilterIndex=i;
	OFN.lpstrFile=ResultFile;
	OFN.nMaxFile=MaxProgDir;
	OFN.lpstrFileTitle=0;
	OFN.nMaxFileTitle=0;
#ifdef SPG_General_USEGlobal
	OFN.lpstrInitialDir=Global.CurDir;
#endif
	OFN.lpstrTitle= SPG_COMPANYNAME" - Charger un profil 1D";
	OFN.Flags=OFN_FILEMUSTEXIST|OFN_HIDEREADONLY|OFN_PATHMUSTEXIST|OFN_EXPLORER|OFN_ENABLEHOOK;
	OFN.nFileOffset=0;//retourne la position du nom hors chemin
	OFN.nFileExtension=0;//idem pour l'extension
	OFN.lpstrDefExt=0;
	OFN.lCustData=0;
	OFN.lpfnHook=Cut_InternalGenericHookProc;
	OFN.lpTemplateName=0;

	DoEvents(SPG_DOEV_READ_WIN_EVENTS);
	if (GetOpenFileName(&OFN))
	{
		SPG_SetCurDirFrom(ResultFile);
		SPG_WaitMouseRelease();

		i=Cut_GetExtens(ResultFile); if(i==0) i=OFN.nFilterIndex;

		int RetVal=Cut_CreateFromIndexFileType(C,ResultFile,i);
		SPG_StackCheck(OFN);
		return RetVal;
	}
	else
	{
		SPG_StackCheck(OFN);
		SPG_WaitMouseRelease();
		return 0;
	}
}

int SPG_CONV Cut_Save(Cut& C, const char* SuggestedName)
{
#ifdef SPG_General_USEGlobal
#ifdef SPG_General_USENetwork_Protocol
	if(SPG_UnderNetControl) 
		return Cut_NetSave(*(Global.SNP),Global.ControlSource,C,SuggestedName);
	else
#endif
#endif
		return Cut_InternalSave(C,SuggestedName);
}

int SPG_CONV Cut_InternalSave(Cut& C, const char* SuggestedName)
{
	SPG_StackAllocZ(OPENFILENAME,OFN);

	char ResultFile[MaxProgDir];
	strcpy(ResultFile,SuggestedName);

	OFN.lStructSize=sizeof(OFN);
#ifdef SPG_General_USEGlobal
	OFN.hwndOwner=(HWND)Global.hWndWin;
	OFN.hInstance=0;
#endif
	OFN.lpstrFilter=SaveFilterTXT SaveFilterPGC SaveFilterRF SaveFilterRS SaveFilterRUS SaveFilterRB SaveFilterWAV SaveFilterCUT SaveFilterPRO"\0";
	int i=Cut_GetExtens(ResultFile); if(i==0) i=Cut_DefaultIndex;
	OFN.nFilterIndex=i;
	OFN.lpstrFile=ResultFile;
	OFN.nMaxFile=MaxProgDir;
	OFN.lpstrFileTitle=0;
	OFN.nMaxFileTitle=0;
#ifdef SPG_General_USEGlobal
	OFN.lpstrInitialDir=Global.CurDir;
#endif
	OFN.lpstrTitle= SPG_COMPANYNAME" - Enregistrer un profil 1D";
	OFN.Flags=OFN_OVERWRITEPROMPT|OFN_FILEMUSTEXIST|OFN_HIDEREADONLY|OFN_PATHMUSTEXIST|OFN_EXPLORER|OFN_ENABLEHOOK;
	OFN.nFileOffset=0;//retourne la position du nom hors chemin
	OFN.nFileExtension=0;//idem pour l'extension
	OFN.lpstrDefExt=0;
	OFN.lCustData=0;
	OFN.lpfnHook=Cut_InternalGenericHookProc;
	OFN.lpTemplateName=0;

	DoEvents(SPG_DOEV_READ_WIN_EVENTS);
	if (GetSaveFileName(&OFN))
	{
		SPG_SetCurDirFrom(ResultFile);
		SPG_WaitMouseRelease();
		int E=Cut_GetExtens(ResultFile);
		if ((E!=Cut_TXT)
			&&(E!=Cut_PGC)
			&&(E!=Cut_RAWFLOAT)
			&&(E!=Cut_RAWSHORT)
			&&(E!=Cut_RAWUSHORT)
			&&(E!=Cut_RAWBYTE)
			&&(E!=Cut_WAV)
			&&(E!=Cut_CUT)
#ifdef SPG_General_USEPRO
			&&(E!=Cut_PRO)
#endif
			)
		{
			//attention ce code doit etre en toute rigeur modifie
			//lorsqu'on change les combinaisons de types de fichiers
			if (OFN.nFilterIndex==Cut_TXTsfIndex) SPG_SetExtens(ResultFile,".txt");
			if (OFN.nFilterIndex==Cut_PGCsfIndex) SPG_SetExtens(ResultFile,".pgc");
			if (OFN.nFilterIndex==Cut_RAWFLOATsfIndex) SPG_SetExtens(ResultFile,".rf");
			if (OFN.nFilterIndex==Cut_RAWSHORTsfIndex) SPG_SetExtens(ResultFile,".rs");
			if (OFN.nFilterIndex==Cut_RAWUSHORTsfIndex) SPG_SetExtens(ResultFile,".rus");
			if (OFN.nFilterIndex==Cut_RAWBYTEsfIndex) SPG_SetExtens(ResultFile,".rb");
			if (OFN.nFilterIndex==Cut_WAVsfIndex) SPG_SetExtens(ResultFile,".wav");
			if (OFN.nFilterIndex==Cut_CUTsfIndex) SPG_SetExtens(ResultFile,".cut");
#ifdef SPG_General_USEPRO
			if (OFN.nFilterIndex==Cut_PROsfIndex) SPG_SetExtens(ResultFile,".pro");
#endif
		}
		int RetVal=Cut_SaveToFile(C,ResultFile);
		SPG_StackCheck(OFN);
		return RetVal;
	}
	else
	{
		SPG_StackCheck(OFN);
		SPG_WaitMouseRelease();
		return 0;
	}
}
#endif

int SPG_CONV Cut_ReadTXT(Cut& C, const char* FName)
{
	memset(&C,0,sizeof(Cut));
	//manquent les memset(0), idem dans profil
		int SizeX,SizeY;
/*
		Cut_Data(C)=Text_Read(FName,SizeX,SizeY,1);

			char Msg[512];
			sprintf(Msg,"%s\n%d elements",FName,SizeX*SizeY);
			
#ifdef SPG_General_USEGlobal
			if (MessageBox((HWND)Global.hWndWin,Msg,"Cut_CreateFromFile",MB_TOPMOST|MB_YESNO)!=IDYES) return 0;
#else
			if (MessageBox(0,Msg,"Cut_CreateFromFile",MB_TOPMOST|MB_YESNO)!=IDYES) return 0;
#endif
*/
		Cut_Data(C)=Text_Read(FName,SizeX,SizeY,0);
		CHECK(Cut_Data(C)==0,"Cut_CreateFromTXTFile: Chargement echoue",return 0);

		Cut_Etat(C)=Cut_WithMEM;
		Cut_NumS(C)=SizeX*SizeY;
		Cut_XScale(C)=1;
		return -1;
}

#ifdef SPG_General_USEFLOATIO
int SPG_CONV Cut_ReadRF(Cut& C, const char* FName)
{
	memset(&C,0,sizeof(Cut));
	int NumS;
	Cut_Data(C)=FIO_ReadRF1D(FName,NumS);
	CHECK(Cut_Data(C)==0,"Cut_CreateFromFloatFile: Chargement echoue",return 0);
	Cut_Etat(C)=Cut_WithMEM;
	Cut_NumS(C)=NumS;
	Cut_XScale(C)=1;
	return -1;
}

int SPG_CONV Cut_ReadRS(Cut& C, const char* FName)
{
	memset(&C,0,sizeof(Cut));
	int NumS;
	Cut_Data(C)=FIO_ReadRS1D(FName,NumS);
	CHECK(Cut_Data(C)==0,"Cut_CreateFromFloatFile: Chargement echoue",return 0);
	Cut_Etat(C)=Cut_WithMEM;
	Cut_NumS(C)=NumS;
	Cut_XScale(C)=1;
	return -1;
}

int SPG_CONV Cut_ReadRUS(Cut& C, const char* FName)
{
	memset(&C,0,sizeof(Cut));
	int NumS;
	Cut_Data(C)=FIO_ReadRUS1D(FName,NumS);
	CHECK(Cut_Data(C)==0,"Cut_CreateFromFloatFile: Chargement echoue",return 0);
	Cut_Etat(C)=Cut_WithMEM;
	Cut_NumS(C)=NumS;
	Cut_XScale(C)=1;
	return -1;
}

int SPG_CONV Cut_ReadRB(Cut& C, const char* FName)
{
	memset(&C,0,sizeof(Cut));
	int NumS;
	Cut_Data(C)=FIO_ReadRB1D(FName,NumS);
	CHECK(Cut_Data(C)==0,"Cut_CreateFromFloatFile: Chargement echoue",return 0);
	Cut_Etat(C)=Cut_WithMEM;
	Cut_NumS(C)=NumS;
	Cut_XScale(C)=1;
	return -1;
}
#endif

#ifdef SPG_General_USEWAVIO
int SPG_CONV Cut_ReadWAV(Cut& C, const char* FName)
{
	memset(&C,0,sizeof(Cut));
	int NumS;
	int Frequence;
	Cut_Data(C)=WAV_ReadFloatMono(FName,NumS,Frequence);
	CHECK(Cut_Data(C)==0,"Cut_CreateFromWAVFile: Chargement echoue",return 0);
	Cut_Etat(C)=Cut_WithMEM;
	Cut_NumS(C)=NumS;
	Cut_XScale(C)=1;
	return -1;
}
#endif

int SPG_CONV Cut_ReadCUT(Cut& C, const char* FName)
{
	memset(&C,0,sizeof(Cut));
	FILE *F = fopen(FName,"rb");
	CHECKTWO(F==0,"Cut_ReadCUT: Impossible d'ouvrir le fichier",FName,return 0);
	SPG_StackAllocZ(Cut,Ctmp);
	fread(&Ctmp.NumS,4,1,F);
	CHECK(Cut_Create(C,Cut_NumS(Ctmp),Cut_XScale(Ctmp),Cut_UnitX(Ctmp),Cut_UnitY(Ctmp))==0,"Cut_ReadCUT: Cut_Create echoue",fclose(F);return 0);
	SPG_StackCheck(Ctmp);
	fread(Cut_Data(C),Cut_NumS(C)*sizeof(float),1,F);

	int Type;
	fread(&Type,sizeof(int),1,F);

	fread(&C.XScale,sizeof(float),1,F);
	fread(C.UnitX,16,1,F);
	float YScale;
	fread(&YScale,sizeof(float),1,F);
	fread(C.UnitY,16,1,F);

	fclose(F);	
	return -1;
}


int SPG_CONV Cut_ReadPGC(Cut& C, const char * FName)
{
	memset(&C,0,sizeof(Cut));
	FILE *F = fopen(FName,"rb");
	CHECKTWO(F==0,"Cut_ReadPGC: Impossible d'ouvrir le fichier",FName,return 0);
	char Signature[]=FileC_Sign;
	fread(Signature,4,1,F);
	CHECK(strcmp(Signature,FileC_Sign),"Cut_ReadPGC: Ce fichier n'est pas du type " FileC_Sign,fclose(F);return 0);

	{
		SPG_StackAllocZ(Cut,Ctmp);
		fread(&Ctmp,sizeof(Cut),1,F);
		CHECK(Cut_Create(C,Cut_NumS(Ctmp),Cut_XScale(Ctmp),Cut_UnitX(Ctmp),Cut_UnitY(Ctmp))==0,"Cut_ReadPGP: Cut_Create echoue",fclose(F);return 0);
		SPG_StackCheck(Ctmp);
	}

	fread(Cut_Data(C),Cut_NumS(C)*sizeof(float),1,F);
	if (Cut_Msk(C)) fread(Cut_Msk(C),Cut_NumS(C),1,F);
	if (Cut_Decor(C)) fread(Cut_Decor(C),Cut_NumS(C),1,F);
	fclose(F);	
	return -1;
}

int SPG_CONV Cut_CreateFromFile(Cut& C, const char * FName)
{
	if(Cut_GetExtens(FName)==Cut_TXT)
	{
		return Cut_ReadTXT(C,FName);
	}
	else if(Cut_GetExtens(FName)==Cut_PGC)
	{
		return Cut_ReadPGC(C,FName);
	}
	else if(Cut_GetExtens(FName)==Cut_RAWFLOAT)
	{
		return Cut_ReadRF(C,FName);
	}
	else if(Cut_GetExtens(FName)==Cut_RAWSHORT)
	{
		return Cut_ReadRS(C,FName);
	}
	else if(Cut_GetExtens(FName)==Cut_RAWUSHORT)
	{
		return Cut_ReadRUS(C,FName);
	}
	else if(Cut_GetExtens(FName)==Cut_RAWBYTE)
	{
		return Cut_ReadRB(C,FName);
	}
#ifdef SPG_General_USEWAVIO
	else if(Cut_GetExtens(FName)==Cut_WAV)
	{
		return Cut_ReadWAV(C,FName);
	}
#endif
	else if(Cut_GetExtens(FName)==Cut_CUT)
	{
		return Cut_ReadCUT(C,FName);
	}
#ifdef SPG_General_USEPRO
	else if(Cut_GetExtens(FName)==Cut_PRO)
	{
		return Cut_ReadPRO(C,FName);
	}
#endif
		char Msg[1024];
		sprintf(Msg,"Le fichier %s\nn'a pas une extension connue.",FName);
#ifdef SPG_General_USEGlobal
		MessageBox((HWND)Global.hWndWin,Msg,"Chargement d'un profil",0);
#else
		MessageBox(0,Msg,"Chargement d'un profil",0);
#endif
		return 0;
}

int SPG_CONV Cut_CreateFromIndexFileType(Cut& C, const char * FName, int Index)
{
	if(Index==Cut_TXTlfIndex)
	{
		return Cut_ReadTXT(C,FName);
	}
	else if(Index==Cut_PGClfIndex)
	{
		return Cut_ReadPGC(C,FName);
	}
	else if(Index==Cut_RAWFLOATlfIndex)
	{
		return Cut_ReadRF(C,FName);
	}
	else if(Index==Cut_RAWSHORTlfIndex)
	{
		return Cut_ReadRS(C,FName);
	}
	else if(Index==Cut_RAWUSHORTlfIndex)
	{
		return Cut_ReadRUS(C,FName);
	}
	else if(Index==Cut_RAWBYTElfIndex)
	{
		return Cut_ReadRB(C,FName);
	}
#ifdef SPG_General_USEWAVIO
	else if(Index==Cut_WAVlfIndex)
	{
		return Cut_ReadWAV(C,FName);
	}
#endif
	else if(Index==Cut_CUTlfIndex)
	{
		return Cut_ReadCUT(C,FName);
	}
#ifdef SPG_General_USEPRO
	else if(Index==Cut_PROlfIndex)
	{
		return Cut_ReadPRO(C,FName);
	}
#endif
		char Msg[1024];
		sprintf(Msg,"Le fichier %s\nn'a pas une extension connue.",FName);
#ifdef SPG_General_USEGlobal
		MessageBox((HWND)Global.hWndWin,Msg,"Chargement d'un profil",0);
#else
		MessageBox(0,Msg,"Chargement d'un profil",0);
#endif
		return 0;
}

int SPG_CONV Cut_SaveToFile(Cut& C,const char * FName)
{
	if(Cut_GetExtens(FName)==Cut_TXT)
	{
		//Text_Write(P_Data(P),P_SizeX(P),P_SizeY(P),FName,6,S_Tabulation);
		return Cut_WriteTXT(C,FName,6,S_RetourChariot,1);
	}
	else if (Cut_GetExtens(FName)==Cut_PGC)
	{
		return Cut_WritePGC(C,FName);
	}
	else if (Cut_GetExtens(FName)==Cut_RAWFLOAT)
	{
		return Cut_WriteRF(C,FName,1);
	}
	else if (Cut_GetExtens(FName)==Cut_RAWSHORT)
	{
		return Cut_WriteRS(C,FName,1);
	}
	else if (Cut_GetExtens(FName)==Cut_RAWUSHORT)
	{
		return Cut_WriteRUS(C,FName,1);
	}
	else if (Cut_GetExtens(FName)==Cut_RAWBYTE)
	{
		return Cut_WriteRB(C,FName,1);
	}
#ifdef SPG_General_USEWAVIO
	else if (Cut_GetExtens(FName)==Cut_WAV)
	{
		return Cut_WriteWAV(C,FName,44100);
	}
#endif
	else if (Cut_GetExtens(FName)==Cut_CUT)
	{
		return Cut_WriteCUT(C,FName);
	}
#ifdef SPG_General_USEPRO
	else if (Cut_GetExtens(FName)==Cut_PRO)
	{
		return Cut_WritePRO(C,FName);
	}
#endif
	else
	{
		char Msg[1024];
		sprintf(Msg,"Le fichier %s\nn'a pas une extension connue.\nSauvegardez a nouveau.",FName);
#ifdef SPG_General_USEGlobal
		MessageBox((HWND)Global.hWndWin,Msg,"Enregistrement d'un profil",0);
#else
		MessageBox(0,Msg,"Enregistrement d'un profil",0);
#endif
	}
	return 0;
}


void SPG_CONV Cut_SaveAsNr(Cut& C, const char * FName_NoExtens, int StrCatNumberToFileName)
{
	char ResultFile[MaxProgDir];
	sprintf(ResultFile,"%s%d.txt",FName_NoExtens,StrCatNumberToFileName);
	if ((C.Etat!=0)&&(C.NumS>2))
		Cut_WriteTXT(C,ResultFile,6,S_RetourChariot,1);
	return;
}

int SPG_CONV Cut_WriteTXT(Cut& C, const char*FName, int ChiffSignif, int Separateur,int WithHeader)
{
	CHECK(Cut_Etat(C)==0,"WriteTXT: Profil nul",return 0);
	CHECK(Cut_Data(C)==0,"WriteTXT: Donnees nulles",return 0);
	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"Cut_Write: Ne peut ouvrir le fichier",FName,return 0);
	char FSep[3];
	FSep[1]=0;
	FSep[2]=0;
	if (Separateur==S_Tabulation)
	FSep[0]='\t';
	else if (Separateur==S_Virgule)
	FSep[0]=',';
	else// (Separateur==S_RetourChariot)
	{
	FSep[0]='\r';
	FSep[1]='\n';
	}

	if(WithHeader) Cut_WriteTxtHeader(C,F,6);

	int x;
	for(x=0;x<C.NumS;x++)
	{
		char Val[256];
		Val[0]=0;
		CF_GetString(Val,C.D[x],ChiffSignif);
		strcat(Val,FSep);
		//sprintf(Val,FSep,C.D[x]);
		fwrite(Val,strlen(Val),1,F);
	}
	fclose(F);
	/*
		char Val[255];
		sprintf(Val,"%f\n",Df[x+SizeX*y]);
		fwrite(Val,strlen(Val),1,F);
		*/
	return -1;
}

int SPG_CONV Cut_WritePGC(Cut& C, const char * FName)
{
	CHECK(Cut_Etat(C)==0,"Cut_WritePGP: Profil nul",return 0);
	CHECK(Cut_Data(C)==0,"Cut_WritePGP: Donnees nulles",return 0);

	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"Cut_WritePGP: Impossible d'ouvrir le fichier",FName,return 0);

	const char* Signature=FileC_Sign;
	fwrite(Signature,4,1,F);
	fwrite(&C,sizeof(Cut),1,F);
	fwrite(Cut_Data(C),Cut_NumS(C)*sizeof(float),1,F);
	if (Cut_Msk(C)) fwrite(Cut_Msk(C),Cut_NumS(C),1,F);
	if (Cut_Decor(C)) fwrite(Cut_Decor(C),Cut_NumS(C),1,F);
	fclose(F);
	return -1;
}

int SPG_CONV Cut_WriteRF(Cut& C, const char * FName,int WithTxtInfo)
{
	CHECK(Cut_Etat(C)==0,"Cut_WriteAsRawFloat: Cut nul",return 0);
	CHECK(Cut_Data(C)==0,"Cut_WriteAsRawFloat: Donnees nulles",return 0);

	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"Cut_WriteAsRawFloat: Impossible d'ouvrir le fichier",FName,return 0);

	fwrite(Cut_Data(C),Cut_NumS(C)*sizeof(float),1,F);

	fclose(F);

	if(WithTxtInfo)
	{
		SPG_SetExtens(FName,".txt");

		F=fopen(FName,"wb");
		CHECKTWO(F==0,"Cut_WriteAsRawFloat: Impossible d'ouvrir le fichier texte associe",FName,return 0);

		Cut_WriteTxtHeader(C, F, 6);

		fclose(F);
	}
	return -1;
}

int SPG_CONV Cut_WriteRS(Cut& C, const char * FName,int WithTxtInfo)
{
	CHECK(Cut_Etat(C)==0,"Cut_WriteAsRawShort: Cut nul",return 0);
	CHECK(Cut_Data(C)==0,"Cut_WriteAsRawShort: Donnees nulles",return 0);

	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"Cut_WriteAsRawFloat: Impossible d'ouvrir le fichier",FName,return 0);

	
	for(int i=0;i<Cut_NumS(C);i++)
	{
		SHORT Z=(SHORT)Cut_Data(C)[i];
		fwrite(&Z,sizeof(SHORT),1,F);
	}

	fclose(F);

	if(WithTxtInfo)
	{
		SPG_SetExtens(FName,".txt");

		F=fopen(FName,"wb");
		CHECKTWO(F==0,"Cut_WriteAsRawFloat: Impossible d'ouvrir le fichier texte associe",FName,return 0);

		Cut_WriteTxtHeader(C, F, 6);

		fclose(F);
	}
	return -1;
}

int SPG_CONV Cut_WriteRUS(Cut& C, const char * FName,int WithTxtInfo)
{
	CHECK(Cut_Etat(C)==0,"Cut_WriteAsRawUnsignedShort: Cut nul",return 0);
	CHECK(Cut_Data(C)==0,"Cut_WriteAsRawUnsignedShort: Donnees nulles",return 0);

	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"Cut_WriteAsRawFloat: Impossible d'ouvrir le fichier",FName,return 0);

	
	for(int i=0;i<Cut_NumS(C);i++)
	{
		WORD Z=(WORD)Cut_Data(C)[i];
		fwrite(&Z,sizeof(WORD),1,F);
	}

	fclose(F);

	if(WithTxtInfo)
	{
		SPG_SetExtens(FName,".txt");

		F=fopen(FName,"wb");
		CHECKTWO(F==0,"Cut_WriteAsRawFloat: Impossible d'ouvrir le fichier texte associe",FName,return 0);

		Cut_WriteTxtHeader(C, F, 6);

		fclose(F);
	}
	return -1;
}

int SPG_CONV Cut_WriteRB(Cut& C,const char * FName,int WithTxtInfo)
{
	CHECK(Cut_Etat(C)==0,"Cut_WriteAsRawByte: Cut nul",return 0);
	CHECK(Cut_Data(C)==0,"Cut_WriteAsRawByte: Donnees nulles",return 0);

	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"Cut_WriteAsRawFloat: Impossible d'ouvrir le fichier",FName,return 0);

	
	for(int i=0;i<Cut_NumS(C);i++)
	{
		BYTE Z=(BYTE)Cut_Data(C)[i];
		fwrite(&Z,sizeof(BYTE),1,F);
	}

	fclose(F);

	if(WithTxtInfo)
	{
		SPG_SetExtens(FName,".txt");

		F=fopen(FName,"wb");
		CHECKTWO(F==0,"Cut_WriteAsRawFloat: Impossible d'ouvrir le fichier texte associe",FName,return 0);

		Cut_WriteTxtHeader(C, F, 6);

		fclose(F);
	}
	return -1;
}

#ifdef SPG_General_USEWAVIO
int SPG_CONV Cut_WriteWAV(Cut& C,char * FName, int Frequency)
{
	CHECK(Cut_Etat(C)==0,"Cut_WriteWAV: Cut nul",return 0);
	CHECK(Cut_Data(C)==0,"Cut_WriteWAV: Donnees nulles",return 0);
	float fMin,fMax;
	Cut_FindMinMax(C,fMin,fMax);
	return WAV_WriteFloat(Cut_Data(C),Cut_NumS(C),Frequency,fMin,fMax,FName);
}
#endif

int SPG_CONV Cut_WriteCUT(Cut& C, const char * FName)
{
	CHECK(Cut_Etat(C)==0,"Cut_WritePGP: Profil nul",return 0);
	CHECK(Cut_Data(C)==0,"Cut_WritePGP: Donnees nulles",return 0);

	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"Cut_WritePGP: Impossible d'ouvrir le fichier",FName,return 0);

	fwrite(&C.NumS,sizeof(int),1,F);
	fwrite(Cut_Data(C),Cut_NumS(C)*sizeof(float),1,F);

	int Type=1;
	fwrite(&Type,sizeof(int),1,F);

	fwrite(&C.XScale,sizeof(float),1,F);
	fwrite(C.UnitX,16,1,F);
	float YScale;
	fwrite(&YScale,sizeof(float),1,F);
	fwrite(C.UnitY,16,1,F);

	int Bound=0;
	fwrite(&Bound,sizeof(int),0,F);
	fwrite(&Bound,sizeof(int),0,F);//curs0
	fwrite(&Bound,sizeof(int),0,F);//curs1

	fclose(F);
	return -1;
}

int SPG_CONV Cut_WriteTxtHeader(Cut& C, void* F, int ChiffSignif)
{
	CHECK(F==0,"Cut_WriteTxtHeader: Mauvais handle de fichier",return 0);
	CHECK(Cut_Etat(C)==0,"Cut_WriteTxtHeader: Cut nul",return 0);
   	char Msg[256];
	Msg[0]=0;
	strcat(Msg,"NumS:");
	CF_GetString(Msg,Cut_NumS(C),ChiffSignif);

	strcat(Msg,Cut_RET"UniteX:");
	CF_GetString(Msg,Cut_XScale(C),ChiffSignif);
	strcat(Msg,Cut_UnitX(C));
	strcat(Msg,Cut_RET"AxeY:");
	strcat(Msg,Cut_UnitY(C));
	strcat(Msg,Cut_RET);
	fwrite(Msg,strlen(Msg),1,(FILE*)F);
	return -1;
}


#endif

