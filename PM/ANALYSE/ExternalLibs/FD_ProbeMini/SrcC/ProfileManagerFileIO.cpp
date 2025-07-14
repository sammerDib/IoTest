
#include "SPG_General.h"

#ifdef SPG_General_USEProfil

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#define P_DefaultIndex 3

#define LoadFilterTXT "Texte\0*.txt\0"
#define SaveFilterTXT LoadFilterTXT
#define P_TXT 1
#define P_TXTlfIndex 1
#define P_TXTsfIndex 1

#define LoadFilterBMP "Bitmap\0*.bmp\0"
#define SaveFilterBMP LoadFilterBMP
#define P_BMP 2
#define P_BMPlfIndex 2
#define P_BMPsfIndex 2

//definition du type PGP
//(type non supporte en lecture)
#define LoadFilterPGP "PG Profil\0*.pgp\0"
#define SaveFilterPGP LoadFilterPGP
//formats etendus
#define P_PGP 3
#define P_PGPlfIndex 3
#define P_PGPsfIndex 3

//definition du type raw float
#define LoadFilterRF "Raw float\0*.*\0"
#define SaveFilterRF "Raw float\0*.rf\0"
//formats etendus
#define P_RAWFLOAT 4
#define P_RAWFLOATlfIndex 4
#define P_RAWFLOATsfIndex 4

//definition du type raw short
//(type non supporte en lecture)
#define LoadFilterRS "Raw short\0*.*\0"
#define SaveFilterRS "Raw short\0*.rs\0"
//formats etendus
#define P_RAWSHORT 5
#define P_RAWSHORTlfIndex 5
#define P_RAWSHORTsfIndex 5

//definition du type raw short unsigned
//(type non supporte en lecture)
#define LoadFilterRUS "Raw unsigned short\0*.*\0"
#define SaveFilterRUS "Raw unsigned short\0*.rus\0"
//formats etendus
#define P_RAWUSHORT 6
#define P_RAWUSHORTlfIndex 6
#define P_RAWUSHORTsfIndex 6

//definition du type TPO (Fabien)
#define LoadFilterTPO "Topographie complète M3D\0*.tpo\0"
#define SaveFilterTPO LoadFilterTPO
//formats etendus
#define P_TPO 7
#define P_TPOlfIndex 7
#define P_TPOsfIndex 7

#ifdef SPG_General_USEHDF
#include "HDF_Loader.h"
#define P_HDF 8
#define P_HDFlfIndex 8
#define LoadFilterHDF "HDF File Format\0*.hdf\0"
//type non supporte en enregistrement
//SaveFilterHDF
#else
#define LoadFilterHDF
//SaveFilterHDF
#define P_HDFlfIndex 7
#endif

#ifdef SPG_General_USESUR
#include "SUR_Loader.h"
#define LoadFilterSUR "Digital Surf\0*.sur\0"
#define SaveFilterSUR LoadFilterSUR
#define P_SUR 9
#define P_SURlfIndex P_HDFlfIndex+1
#define P_SURsfIndex 8
#else
#define LoadFilterSUR
#define SaveFilterSUR
#endif

#include <stdio.h>
#include <string.h>

#define P_RET "\r\n"

int SPG_CONV P_GetExtens(char *FullName)
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
	if (_stricmp(FullName+i,".bmp")==0) return P_BMP;
	if (_stricmp(FullName+i,".txt")==0) return P_TXT;
	if (_stricmp(FullName+i,".pgp")==0) return P_PGP;
	if (_stricmp(FullName+i,".rf")==0) return P_RAWFLOAT;
	if (_stricmp(FullName+i,".rs")==0) return P_RAWSHORT;
	if (_stricmp(FullName+i,".rus")==0) return P_RAWUSHORT;
	if (_stricmp(FullName+i,".tpo")==0) return P_TPO;
#ifdef SPG_General_USEHDF
	if (_stricmp(FullName+i,".hdf")==0) return P_HDF;
#endif
#ifdef SPG_General_USESUR
	if (_stricmp(FullName+i,".sur")==0) return P_SUR;
#endif
	return 0;
}

#ifdef SPG_General_USENetwork_Protocol
int SPG_CONV P_NetHook_Load(SPG_NET_PROTOCOL& SNP, SPG_NET_ADDR& SNA)
{
	SPG_StackAllocZ(Profil,P);
	if((P_InternalLoad(P,(char*)(SNP.Message.M))==0)||(P_Etat(P)==0))
	{
		SNP_Send_BYTE(SNP,SNA,SNP_BREAK,0);
		return 0;
	}
	CHECK(SNP_Send(SNP,SNA,SNP_P_STRUCT,&P,sizeof(Profil))==0,"P_NetHook_Load: Envoi echoue",P_Close(P);return 0);
	CHECK(SPG_Download_Send(SNP,SNA,SNP_P_DATA_SEND,SNP_P_DATA_RESPONSE,P_Data(P),P_SizeX(P)*P_SizeY(P)*sizeof(float))==0,"P_NetHook_Load: Envoi echoue",P_Close(P);return 0);
	if(P_Msk(P)) CHECK(SPG_Download_Send(SNP,SNA,SNP_P_MSK_SEND,SNP_P_MSK_RESPONSE,P_Msk(P),P_SizeX(P)*P_SizeY(P))==0,"P_NetHook_Load: Envoi echoue",P_Close(P);return 0);
	P_Close(P);
	return -1;
}

int SPG_CONV P_NetLoad(SPG_NET_PROTOCOL& SNP, SPG_NET_ADDR& SNA, Profil& P, char * SuggestedName)
{
	memset(&P,0,sizeof(Profil));
	CHECK(SNP_Send_char(SNP,SNA,SNP_P_STARTLOADPROFILE,SuggestedName)==0,"P_NetLoad: Envoi echoue",return 0);
	CHECK(SPG_NetworkDoEvents_LongWait(SNP,SNA,SNP_P_STRUCT)==0,"P_NetLoad: Pas de reponse",SNP_Send_BYTE(SNP,SNA,SNP_BREAK,1);return 0);
	CHECK(SNP_IsLen(SNP,sizeof(Profil))==0,"P_NetLoad: Taille recue imprevue",SNP_Send_BYTE(SNP,SNA,SNP_BREAK,1);return 0);
	memcpy(&P,SNP.Message.M,sizeof(Profil));
	if(P_Etat(P))
	{
		if(P_Create(P,P_SizeX(P),P_SizeY(P),P_XScale(P),P_YScale(P),P_UnitX(P),P_UnitY(P),P_UnitZ(P), (int)P_Msk(P)))
		{
			CHECK(SPG_Download_Read(SNP,SNA,SNP_P_DATA_SEND,SNP_P_DATA_RESPONSE,P_Data(P),P_SizeX(P)*P_SizeY(P)*sizeof(float))==0,"P_NetLoad: Envoi echoue",return 0);
			if(P_Msk(P)) CHECK(SPG_Download_Read(SNP,SNA,SNP_P_MSK_SEND,SNP_P_MSK_RESPONSE,P_Msk(P),P_SizeX(P)*P_SizeY(P)),"P_NetLoad: Envoi echoue",return 0);
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

int SPG_CONV P_NetHook_Save(SPG_NET_PROTOCOL& SNP, SPG_NET_ADDR& SNA)
{
	SPG_StackAllocZ(Profil,P);
	char SuggestedName[MaxProgDir];
	strcpy(SuggestedName,(char*)(SNP.Message.M));
	CHECK(SPG_Network_DoEvents(SNP,SNA,SNP_P_STRUCT)==0,"P_NetHook_Save: Pas de reponse",SNP_Send_BYTE(SNP,SNA,SNP_BREAK,1);return 0);
	CHECK(SNP_IsLen(SNP,sizeof(Profil))==0,"P_NetHook_Save: Taille recue imprevue",SNP_Send_BYTE(SNP,SNA,SNP_BREAK,1);return 0);
	memcpy(&P,SNP.Message.M,sizeof(Profil));
	if(P_Etat(P))
	{
		if(P_Create(P,P_SizeX(P),P_SizeY(P),P_XScale(P),P_YScale(P),P_UnitX(P),P_UnitY(P),P_UnitZ(P), (int)P_Msk(P)))
		{
			CHECK(SPG_Download_Read(SNP,SNA,SNP_P_DATA_SEND,SNP_P_DATA_RESPONSE,P_Data(P),P_SizeX(P)*P_SizeY(P)*sizeof(float)),"P_NetHook_Save: Envoi echoue",P_Close(P);return 0);
			if(P_Msk(P)) CHECK(SPG_Download_Read(SNP,SNA,SNP_P_MSK_SEND,SNP_P_MSK_RESPONSE,P_Msk(P),P_SizeX(P)*P_SizeY(P))==0,"P_NetHook_Save: Envoi echoue",P_Close(P);return 0);
			CHECK(P_InternalSave(P,SuggestedName)==0,"P_NetHook_Save: Sauvegarde echouee",P_Close(P);return 0);
			P_Close(P);
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

int SPG_CONV P_NetSave(SPG_NET_PROTOCOL& SNP, SPG_NET_ADDR& SNA, Profil& P, char * SuggestedName)
{
	CHECK(SNP_Send_char(SNP,SNA,SNP_P_STARTSAVEPROFILE,SuggestedName)==0,"P_NetSave: Envoi echoue",return 0);
	CHECK(SNP_Send(SNP,SNA,SNP_P_STRUCT,&P,sizeof(Profil))==0,"P_NetSave: Envoi echoue",return 0);
	if(P_Etat(P))
	{
		CHECK(SPG_Download_Send(SNP,SNA,SNP_P_DATA_SEND,SNP_P_DATA_RESPONSE,P_Data(P),P_SizeX(P)*P_SizeY(P)*sizeof(float)),"P_NetSave: Envoi echoue",return 0);
		if(P_Msk(P)) CHECK(SPG_Download_Send(SNP,SNA,SNP_P_MSK_SEND,SNP_P_MSK_RESPONSE,P_Msk(P),P_SizeX(P)*P_SizeY(P)),"P_NetSave: Envoi echoue",return 0);
	}
	return -1;
}
#endif

#ifdef SPG_General_USEFilesWindows
int SPG_CONV P_Load(Profil& P, char * SuggestedName)
{
#ifdef SPG_General_USEGlobal
#ifdef SPG_General_USENetwork_Protocol
	if(SPG_UnderNetControl) 
		return P_NetLoad(*(Global.SNP),Global.ControlSource,P,SuggestedName);
	else
#endif
#endif
		return P_InternalLoad(P,SuggestedName);
}


UINT CALLBACK P_InternalGenericHookProc(
  HWND hdlg,      // handle to child dialog window
  UINT uiMsg,     // message identifier
  WPARAM wParam,  // message parameter
  LPARAM lParam   // message parameter
)
{
	return 0;
}

int SPG_CONV P_InternalLoad(Profil& P, char * SuggestedName)
{
	memset(&P,0,sizeof(Profil));

	SPG_StackAllocZ(OPENFILENAME,OFN);

	char ResultFile[MaxProgDir];
	strcpy(ResultFile,SuggestedName);

	OFN.lStructSize=sizeof(OPENFILENAME);
#ifdef SPG_General_USEGlobal
	OFN.hwndOwner=(HWND)Global.hWndWin;
#endif
	OFN.hInstance=0;
	OFN.lpstrFilter=LoadFilterTXT LoadFilterBMP LoadFilterPGP LoadFilterRF LoadFilterRS LoadFilterRUS LoadFilterTPO LoadFilterHDF LoadFilterSUR"\0";
	OFN.nFilterIndex=P_DefaultIndex;
	OFN.lpstrFile=ResultFile;
	OFN.nMaxFile=MaxProgDir;
	OFN.lpstrFileTitle=0;
	OFN.nMaxFileTitle=0;
#ifdef SPG_General_USEGlobal
	OFN.lpstrInitialDir=Global.CurDir;
#endif
	OFN.lpstrTitle= SPG_COMPANYNAME" - Charger un profil 2D";
	OFN.Flags=OFN_FILEMUSTEXIST|OFN_HIDEREADONLY|OFN_PATHMUSTEXIST|OFN_EXPLORER|OFN_ENABLEHOOK;
	OFN.nFileOffset=0;//retourne la position du nom hors chemin
	OFN.nFileExtension=0;//idem pour l'extension
	OFN.lpstrDefExt=0;
	OFN.lCustData=0;
	OFN.lpfnHook=P_InternalGenericHookProc;
	OFN.lpTemplateName=0;

	DoEvents(SPG_DOEV_READ_WIN_EVENTS);
	CD_G_CHECK_EXIT(6,30);
	if (GetOpenFileName(&OFN))
	{
		SPG_SetCurDirFrom(ResultFile);
		SPG_WaitMouseRelease();
		//int RetVal=P_CreateFromFile(P,ResultFile);
		IF_CD_G_CHECK(30,return 0);
		int RetVal=P_CreateFromIndexFileType(P,ResultFile,OFN.nFilterIndex);
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

int SPG_CONV P_Save(Profil& P, char* SuggestedName)
{
#ifdef SPG_General_USEGlobal
#ifdef SPG_General_USENetwork_Protocol
	if(SPG_UnderNetControl) 
		return P_NetSave(*(Global.SNP),Global.ControlSource,P,SuggestedName);
	else
#endif
#endif
		return P_InternalSave(P,SuggestedName);
}

int SPG_CONV P_InternalSave(Profil& P, char* SuggestedName)
{
	SPG_StackAllocZ(OPENFILENAME,OFN);

	char ResultFile[MaxProgDir];
	strcpy(ResultFile,SuggestedName);

	OFN.lStructSize=sizeof(OFN);
#ifdef SPG_General_USEGlobal
	OFN.hwndOwner=(HWND)Global.hWndWin;
	OFN.hInstance=0;
#endif
	OFN.lpstrFilter=SaveFilterTXT SaveFilterBMP SaveFilterPGP SaveFilterRF SaveFilterRS SaveFilterRUS SaveFilterTPO SaveFilterSUR"\0";
	OFN.nFilterIndex=P_DefaultIndex;
	OFN.lpstrFile=ResultFile;
	OFN.nMaxFile=MaxProgDir;
	OFN.lpstrFileTitle=0;
	OFN.nMaxFileTitle=0;
#ifdef SPG_General_USEGlobal
	OFN.lpstrInitialDir=Global.CurDir;
#endif
	OFN.lpstrTitle= SPG_COMPANYNAME" - Enregistrer un profil 2D";
	OFN.Flags=OFN_OVERWRITEPROMPT|OFN_FILEMUSTEXIST|OFN_HIDEREADONLY|OFN_PATHMUSTEXIST|OFN_EXPLORER|OFN_ENABLEHOOK;
	OFN.nFileOffset=0;//retourne la position du nom hors chemin
	OFN.nFileExtension=0;//idem pour l'extension
	OFN.lpstrDefExt=0;
	OFN.lCustData=0;
	OFN.lpfnHook=P_InternalGenericHookProc;
	OFN.lpTemplateName=0;

	DoEvents(SPG_DOEV_READ_WIN_EVENTS);
	CD_G_CHECK_EXIT(20,13);
	if (GetSaveFileName(&OFN))
	{
		SPG_SetCurDirFrom(ResultFile);
		SPG_WaitMouseRelease();
		int E=P_GetExtens(ResultFile);
		if ((E!=P_BMP)
			&&(E!=P_TXT)
			&&(E!=P_PGP)
			&&(E!=P_RAWFLOAT)
			&&(E!=P_RAWSHORT)
			&&(E!=P_RAWUSHORT)
			/*
#ifdef SPG_General_USEHDF
			&&(E!=P_HDF)
#endif
			*/
#ifdef SPG_General_USESUR
			&&(E!=P_SUR)
#endif
			)
		{
			//attention ce code doit etre en toute rigeur modifie
			//lorsqu'on change les combinaisons de types de fichiers
			if (OFN.nFilterIndex==P_TXTsfIndex) SPG_SetExtens(ResultFile,".txt");
			if (OFN.nFilterIndex==P_BMPsfIndex) SPG_SetExtens(ResultFile,".bmp");
			if (OFN.nFilterIndex==P_PGPsfIndex) SPG_SetExtens(ResultFile,".pgp");
			if (OFN.nFilterIndex==P_RAWFLOATsfIndex) SPG_SetExtens(ResultFile,".rf");
			if (OFN.nFilterIndex==P_RAWSHORTsfIndex) SPG_SetExtens(ResultFile,".rs");
			if (OFN.nFilterIndex==P_RAWUSHORTsfIndex) SPG_SetExtens(ResultFile,".rus");
			if (OFN.nFilterIndex==P_TPO) SPG_SetExtens(ResultFile,".tpo");
//			if (OFN.nFilterIndex==P_HDFfIndex) SPG_SetExtens(ResultFile,".hdf");
#ifdef SPG_General_USESUR
			if (OFN.nFilterIndex==P_SURsfIndex) SPG_SetExtens(ResultFile,".sur");
#endif
		}
		IF_CD_G_CHECK(3,return 0);
		int RetVal=P_SaveToFile(P,ResultFile);
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

int SPG_CONV P_ReadTXT(Profil& P, char* FName)
{
	memset(&P,0,sizeof(Profil));
	int SizeX,SizeY;
	
	P_Data(P)=Text_Read(FName,SizeX,SizeY,1);

	char Msg[512];
	sprintf(Msg,"%s\n%d Colonnes, %d Lignes",FName,SizeX,SizeY);

#ifdef SPG_General_USEWindows
#ifdef SPG_General_USEGlobal
	if (MessageBox((HWND)Global.hWndWin,Msg,"P_CreateFromFile",MB_TOPMOST|MB_YESNO)!=IDYES) return 0;
#else
	if (MessageBox(0,Msg,"P_CreateFromFile",MB_TOPMOST|MB_YESNO)!=IDYES) return 0;
#endif
#endif

	P_Data(P)=Text_Read(FName,SizeX,SizeY,0);
	CHECK(P_Data(P)==0,"P_ReadTXT: Chargement echoue",return 0);

	P_Etat(P)=P_WithMEM;
	P_SizeX(P)=SizeX;
	P_SizeY(P)=SizeY;
	P_XScale(P)=1;
	P_YScale(P)=1;
	return -1;
}

int SPG_CONV P_ReadPGP(Profil& P, char * FName)
{
	memset(&P,0,sizeof(Profil));
	FILE *F = fopen(FName,"rb");
	CHECKTWO(F==0,"P_ReadPGP: Impossible d'ouvrir le fichier",FName,return 0);
	char Signature[]=FileP_Sign;
	fread(Signature,4,1,F);
	CHECK(strcmp(Signature,FileP_Sign),"P_ReadPGP: Ce fichier n'est pas du type "FileP_Sign,fclose(F);return 0);
	{
		SPG_StackAllocZ(Profil,Ptmp);
		fread(&Ptmp,sizeof(Profil),1,F);
		CHECK(P_Create(P,P_SizeX(Ptmp),P_SizeY(Ptmp),P_XScale(Ptmp),P_YScale(Ptmp),P_UnitX(Ptmp),P_UnitY(Ptmp),P_UnitZ(Ptmp),P_Msk(Ptmp)!=0)==0,"P_ReadPGP: P_Create echoue",fclose(F);return 0);
		SPG_StackCheck(Ptmp);
	}
	fread(P_Data(P),P_SizeX(P)*P_SizeY(P)*sizeof(float),1,F);
	if (P_Msk(P)) fread(P_Msk(P),P_SizeX(P)*P_SizeY(P)*sizeof(BYTE),1,F);
	fclose(F);	
	return -1;
}

int SPG_CONV P_ReadBMP(Profil& P, char* FName)
{
	memset(&P,0,sizeof(Profil));
	int SizeX,SizeY;
	P_Data(P)=BMP_ReadFloat(FName,SizeX,SizeY);
	CHECK(P_Data(P)==0,"P_ReadBMP: Chargement echoue",return 0);
	P_Etat(P)=P_WithMEM;
	P_SizeX(P)=SizeX;
	P_SizeY(P)=SizeY;
	P_XScale(P)=1;
	P_YScale(P)=1;
	return -1;
}

int SPG_CONV P_ReadRF(Profil& P, char* FName)
{
	memset(&P,0,sizeof(Profil));
	int SizeX,SizeY;
	P_Data(P)=FIO_ReadRF2D(FName,SizeX,SizeY);
	CHECK(P_Data(P)==0,"P_ReadRF: Chargement echoue",return 0);
	P_Etat(P)=P_WithMEM;
	P_SizeX(P)=SizeX;
	P_SizeY(P)=SizeY;
	P_XScale(P)=1;
	P_YScale(P)=1;
	return -1;
}


int SPG_CONV P_ReadRS(Profil& P, char* FName)
{
	memset(&P,0,sizeof(Profil));
	int SizeX,SizeY;
	P_Data(P)=FIO_ReadRS2D(FName,SizeX,SizeY);
	CHECK(P_Data(P)==0,"P_ReadRS: Chargement echoue",return 0);
	P_Etat(P)=P_WithMEM;
	P_SizeX(P)=SizeX;
	P_SizeY(P)=SizeY;
	P_XScale(P)=1;
	P_YScale(P)=1;
	return -1;
}

int SPG_CONV P_ReadRUS(Profil& P, char* FName)
{
	memset(&P,0,sizeof(Profil));
	int SizeX,SizeY;
	P_Data(P)=FIO_ReadRUS2D(FName,SizeX,SizeY);
	CHECK(P_Data(P)==0,"P_ReadRUS: Chargement echoue",return 0);
	P_Etat(P)=P_WithMEM;
	P_SizeX(P)=SizeX;
	P_SizeY(P)=SizeY;
	P_XScale(P)=1;
	P_YScale(P)=1;
	return -1;
}

#ifdef SPG_General_USEHDF
int SPG_CONV P_ReadHDF(Profil& P, char* FName)
{
	memset(&P,0,sizeof(Profil));
	int SizeX,SizeY;
	P_Data(P)=HDF_Read(FName,SizeX,SizeY);
	CHECK(P_Data(P)==0,"P_ReadHDF: Chargement echoue",return 0);
	P_Etat(P)=P_WithMEM;
	P_SizeX(P)=SizeX;
	P_SizeY(P)=SizeY;
	P_XScale(P)=1;
	P_YScale(P)=1;
	return -1;
}
#endif

int SPG_CONV P_ReadTPO(Profil& P, char* FName)
{
	memset(&P,0,sizeof(Profil));
	FILE *F = fopen(FName,"rb");
	CHECKTWO(F==0,"P_ReadTPO: Impossible d'ouvrir le fichier",FName,return 0);
	{
		SPG_StackAllocZ(Profil,Ptmp);
		fread(&Ptmp.H.SizeX,2*sizeof(int),1,F);
		CHECK(P_Create(P,P_SizeX(Ptmp),P_SizeY(Ptmp),1,1,P_UnitX(Ptmp),P_UnitY(Ptmp),P_UnitZ(Ptmp),1)==0,"P_ReadTPO: P_Create echoue",fclose(F);return 0);
		SPG_StackCheck(Ptmp);
	}
	fread(P_Data(P),P_SizeX(P)*P_SizeY(P)*sizeof(float),1,F);
	int nVersion;
	fread(&nVersion,sizeof(int),1,F);//non géré
	int Type;
	fread(&Type,sizeof(int),1,F);
	fread(&P.H.XScale,sizeof(float),1,F);
	fread(&P.H.YScale,sizeof(float),1,F);
	float ZScale=1;
	fread(&ZScale,sizeof(float),1,F);

	strcpy(P.H.UnitX,"µm");
	strcpy(P.H.UnitY,"µm");
	strcpy(P.H.UnitZ,"µm");
	int i;
	for(i=0;i<P_SizeX(P)*P_SizeY(P);i++)
	{
		P_Data(P)[i]*=ZScale;
	}

	int cut_mode;
	fread(&cut_mode,sizeof(int),1,F);
	int nPoints;
	fread(&nPoints,sizeof(int),1,F);
	fseek(F,nPoints*8,SEEK_CUR);//sizeof(TypPoint)

	fread(P_Msk(P),P_SizeX(P)*P_SizeY(P),1,F);

	for(i=0;i<P.H.SizeX*P.H.SizeY;i++)
	{
		if(P_Msk(P)[i]==0xFF)
			P_Msk(P)[i]=0;
		else
			P_Msk(P)[i]=1;
	}

	fclose(F);

	return -1;
}

int SPG_CONV P_CreateFromFile(Profil& P,char * FName)
{
	CD_G_CHECK_EXIT(30,19);
	if(P_GetExtens(FName)==P_TXT)
	{
		return P_ReadTXT(P,FName);
	}
	else if (P_GetExtens(FName)==P_BMP)
	{
		return P_ReadBMP(P,FName);
	}
	else if(P_GetExtens(FName)==P_PGP)
	{
		return P_ReadPGP(P,FName);
	}
	else if(P_GetExtens(FName)==P_RAWFLOAT)
	{
		return P_ReadRF(P,FName);
	}
	else if(P_GetExtens(FName)==P_RAWSHORT)
	{
		return P_ReadRS(P,FName);
	}
	else if(P_GetExtens(FName)==P_RAWUSHORT)
	{
		return P_ReadRUS(P,FName);
	}
	else if(P_GetExtens(FName)==P_TPO)
	{
		return P_ReadTPO(P,FName);
	}
#ifdef SPG_General_USEHDF
	else if(P_GetExtens(FName)==P_HDF)
	{
		return P_ReadHDF(P,FName);
	}
#endif
#ifdef SPG_General_USESUR
	else if(P_GetExtens(FName)==P_SUR)
	{
		return P_ReadSUR(P,FName);
	}
#endif
#ifdef SPG_General_USEWindows
		char Msg[1024];
		sprintf(Msg,"Le fichier %s\nn'a pas une extension connue.",FName);
#ifdef SPG_General_USEGlobal
		MessageBox((HWND)Global.hWndWin,Msg,"Chargement d'un profil",0);
#else
		MessageBox(0,Msg,"Chargement d'un profil",0);
#endif
#endif
		return 0;
}

int SPG_CONV P_CreateFromIndexFileType(Profil& P,char * FName, int Index)
{
	CD_G_CHECK_EXIT(10,17);
	if(Index==P_TXTlfIndex)
	{
		return P_ReadTXT(P,FName);
	}
	else if (Index==P_BMPlfIndex)
	{
		return P_ReadBMP(P,FName);
	}
	else if(Index==P_PGPlfIndex)
	{
		return P_ReadPGP(P,FName);
	}
	else if(Index==P_RAWFLOATlfIndex)
	{
		return P_ReadRF(P,FName);
	}
	else if(Index==P_RAWSHORTlfIndex)
	{
		return P_ReadRS(P,FName);
	}
	else if(Index==P_RAWUSHORTlfIndex)
	{
		return P_ReadRUS(P,FName);
	}
	else if(Index==P_TPOlfIndex)
	{
		return P_ReadTPO(P,FName);
	}
#ifdef SPG_General_USEHDF
	else if(Index==P_HDFlfIndex)
	{
		return P_ReadHDF(P,FName);
	}
#endif
#ifdef SPG_General_USESUR
	else if(Index==P_SURlfIndex)
	{
		return P_ReadSUR(P,FName);
	}
#endif
#ifdef SPG_General_USEWindows
		char Msg[1024];
		sprintf(Msg,"Le fichier %s\nn'a pas une extension connue.",FName);
#ifdef SPG_General_USEGlobal
		MessageBox((HWND)Global.hWndWin,Msg,"Chargement d'un profil",0);
#else
		MessageBox(0,Msg,"Chargement d'un profil",0);
#endif
#endif
		return 0;
}

int SPG_CONV P_SaveToFile(Profil& P,char * FName)
{
	CHECK(FName==0,"P_SaveToFile",return 0);
	CHECKTWO(P_Etat(P)==0,"P_SaveToFile: Profil nul",FName,return 0);
	CHECKTWO(P_Data(P)==0,"P_SaveToFile: Donnees nulles",FName,return 0);
	CD_G_CHECK_EXIT(19,14);
	if(P_GetExtens(FName)==P_TXT)
	{
		return P_WriteTXT(P,FName,6,S_Tabulation,1);
	}
	else if (P_GetExtens(FName)==P_BMP)
	{
		return P_WriteBMP(P,FName);
	}
	else if (P_GetExtens(FName)==P_PGP)
	{
		return P_WritePGP(P,FName);
	}
	else if (P_GetExtens(FName)==P_RAWFLOAT)
	{
		return P_WriteRF(P,FName,1);
	}
	else if (P_GetExtens(FName)==P_RAWSHORT)
	{
		return P_WriteRS(P,FName,1);
	}
	else if (P_GetExtens(FName)==P_RAWUSHORT)
	{
		return P_WriteRUS(P,FName,1);
	}
	else if (P_GetExtens(FName)==P_TPO)
	{
		return P_WriteTPO(P,FName);
	}
#ifdef SPG_General_USESUR
	else if (P_GetExtens(FName)==P_SUR)
	{
		return P_WriteSUR(P,FName);
	}
#endif
#ifdef SPG_General_USEWindows
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
#endif
	return 0;
}

int SPG_CONV P_WriteBMP(Profil& P, char* FName)
{
	CHECK(P_Etat(P)==0,"P_WriteBMP: Profil nul",return 0);
	CHECK(P_Data(P)==0,"P_WriteBMP: Donnees nulles",return 0);
	float VMin,VMax;
	P_FindMinMax(P,VMin,VMax);
	return BMP_WriteFloat(P_Data(P),P_SizeX(P),P_SizeY(P),VMin,VMax,FName);
}

int SPG_CONV P_WritePGP(Profil& P, char* FName)
{
	CHECK(P_Etat(P)==0,"P_SavePGP: Profil nul",return 0);
	CHECK(P_Data(P)==0,"P_SavePGP: Donnees nulles",return 0);

	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"P_SavePGP: Impossible d'ouvrir le fichier",FName,return 0);

	char* Signature=FileP_Sign;
	fwrite(Signature,4,1,F);
	fwrite(&P,sizeof(Profil),1,F);
	fwrite(P_Data(P),P_SizeX(P)*P_SizeY(P)*sizeof(float),1,F);
	if (P_Msk(P)) fwrite(P_Msk(P),P_SizeX(P)*P_SizeY(P)*sizeof(BYTE),1,F);
	fclose(F);
	return -1;
}

int SPG_CONV P_WriteRF(Profil& P,char * FName,int WithTxtInfo)
{
	CHECK(P_Etat(P)==0,"P_WriteAsRawFloat: Profil nul",return 0);
	CHECK(P_Data(P)==0,"P_WriteAsRawFloat: Donnees nulles",return 0);

	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"P_WriteAsRawFloat: Impossible d'ouvrir le fichier",FName,return 0);

	fwrite(P_Data(P),P_SizeX(P)*P_SizeY(P)*sizeof(float),1,F);

	fclose(F);

	if(WithTxtInfo)
	{
		SPG_SetExtens(FName,".txt");

		F=fopen(FName,"wb");
		CHECKTWO(F==0,"P_WriteAsRawFloat: Impossible d'ouvrir le fichier texte associe",FName,return 0);

		P_WriteTxtHeader(P, F, 6);

		fclose(F);
	}
	return -1;
}

int SPG_CONV P_WriteRS(Profil& P,char * FName,int WithTxtInfo)
{
	CHECK(P_Etat(P)==0,"P_WriteAsRawFloat: Profil nul",return 0);
	CHECK(P_Data(P)==0,"P_WriteAsRawFloat: Donnees nulles",return 0);

	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"P_WriteAsRawShort: Impossible d'ouvrir le fichier",FName,return 0);

	for(int i=0;i<P_SizeX(P)*P_SizeY(P);i++)
	{
		SHORT Z=(SHORT)P_Data(P)[i];
		fwrite(&Z,sizeof(SHORT),1,F);
	}

	fclose(F);

	if(WithTxtInfo)
	{
		SPG_SetExtens(FName,".txt");

		F=fopen(FName,"wb");
		CHECKTWO(F==0,"P_WriteAsRawShort: Impossible d'ouvrir le fichier texte associe",FName,return 0);

		P_WriteTxtHeader(P, F, 6);

		fclose(F);
	}
	return -1;
}

int SPG_CONV P_WriteRUS(Profil& P,char * FName,int WithTxtInfo)
{
	CHECK(P_Etat(P)==0,"P_WriteAsRawUFloat: Profil nul",return 0);
	CHECK(P_Data(P)==0,"P_WriteAsRawUFloat: Donnees nulles",return 0);

	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"P_WriteAsRawUShort: Impossible d'ouvrir le fichier",FName,return 0);

	for(int i=0;i<P_SizeX(P)*P_SizeY(P);i++)
	{
		WORD Z=(WORD)P_Data(P)[i];
		fwrite(&Z,sizeof(WORD),1,F);
	}

	fclose(F);

	if(WithTxtInfo)
	{
		SPG_SetExtens(FName,".txt");

		F=fopen(FName,"wb");
		CHECKTWO(F==0,"P_WriteAsRawUShort: Impossible d'ouvrir le fichier texte associe",FName,return 0);

		P_WriteTxtHeader(P, F, 6);

		fclose(F);
	}
	return -1;
}

int SPG_CONV P_WriteTPO(Profil& P,char * FName)
{
	CHECK(P_Etat(P)==0,"P_WriteAsRawFloat: Profil nul",return 0);
	CHECK(P_Data(P)==0,"P_WriteAsRawFloat: Donnees nulles",return 0);

	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"P_WriteTPO: Impossible d'ouvrir le fichier",FName,return 0);

	fwrite(&P.H.SizeX,2*sizeof(int),1,F);
	fwrite(P_Data(P),P_SizeX(P)*P_SizeY(P)*sizeof(float),1,F);

	int nVersion=MAKELONG(MAKEWORD(0,0),MAKEWORD(0,1));
	fwrite(&nVersion,sizeof(int),1,F);
	int Type=1;//TOPO_TYPE_TOPO,TOPO_TYPE_CONTRASTE,TOPO_TYPE_PHASE
	fwrite(&Type,sizeof(int),1,F);
	float factorX=1;
	if(strcmp(P.H.UnitX,"nm")==0) 
		factorX=0.001f;
	else if(strcmp(P.H.UnitX,"µm")==0)
		factorX=1;
#ifdef DebugList
	else
		SPG_List2S("P_WriteTPO: Conversion X en nanometres non effectuée:",P.H.UnitX);
#endif

	float factorY=1;
	if(strcmp(P.H.UnitY,"nm")==0) 
		factorY=0.001f;
	else if(strcmp(P.H.UnitY,"µm")==0)
		factorY=1;
#ifdef DebugList
	else
		SPG_List2S("P_WriteTPO: Conversion Y en nanometres non effectuée:",P.H.UnitY);
#endif

	float factorZ=1;
	if(strcmp(P.H.UnitZ,"nm")==0) 
		factorZ=0.001f;
	else if(strcmp(P.H.UnitZ,"µm")==0)
		factorZ=1;
#ifdef DebugList
	else
		SPG_List2S("P_WriteTPO: Conversion Z en nanometres non effectuée:",P.H.UnitZ);
#endif

	float XScale_nm=P.H.XScale*factorX;
	float YScale_nm=P.H.YScale*factorY;
	fwrite(&XScale_nm,sizeof(float),1,F);
	fwrite(&YScale_nm,sizeof(float),1,F);
	fwrite(&factorZ,sizeof(float),1,F);
	int cut_mode=2;//TOPO_CUT_ANY
	fwrite(&cut_mode,sizeof(int),1,F);
	int nPoints=0;
	fwrite(&nPoints,sizeof(int),1,F);

	SPG_PtrAlloc(Masque,P.H.SizeX*P.H.SizeY,BYTE,"TPOMsk");

	for(int i=0;i<P.H.SizeX*P.H.SizeY;i++)
	{
		if((P_Msk(P)==0)||(P_Msk(P)[i]==0))
			Masque[i]=0xFF;
		else
			Masque[i]=0;
	}

	fwrite(Masque,P_SizeX(P)*P_SizeY(P),1,F);

	SPG_PtrFree(Masque);

	fclose(F);
	return -1;
}

int SPG_CONV P_WriteTxtHeader(Profil& P, void* F, int ChiffSignif)
{
	CHECK(F==0,"P_WriteTxtHeader: Mauvais handle de fichier",return 0);
	CHECK(P_Etat(P)==0,"P_WriteTxtHeader: Profil nul",return 0);
   	char Msg[256];
	Msg[0]=0;
	strcat(Msg,"SizeX:");
	CF_GetString(Msg,(float)P_SizeX(P),ChiffSignif);
	strcat(Msg,P_RET"SizeY:");
	CF_GetString(Msg,(float)P_SizeY(P),ChiffSignif);

	strcat(Msg,P_RET"UniteX:");
	CF_GetString(Msg,P_XScale(P),ChiffSignif);
	strcat(Msg,P_UnitX(P));
	strcat(Msg,P_RET"UniteY:");
	CF_GetString(Msg,P_YScale(P),ChiffSignif);
	strcat(Msg,P_UnitY(P));
	strcat(Msg,P_RET"AxeZ:");
	strcat(Msg,P_UnitZ(P));
	strcat(Msg,P_RET);
	fwrite(Msg,strlen(Msg),1,(FILE*)F);
	return -1;
}


int SPG_CONV P_WriteTXT(Profil& P, char*FName, int ChiffSignif, int Separateur, int WithHeader)
{
	//if(V_GetExtens(FName)==V_TXT)

	CHECK(P_Etat(P)==0,"P_Write: Profil nul",return 0);
	CHECK(P_Data(P)==0,"P_Write: Donnees nulles",return 0);

	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"Impossible d'ouvrir le fichier",FName,return 0);

	if(WithHeader) P_WriteTxtHeader(P,F,ChiffSignif);

	char FSep[2];
	FSep[1]=0;

	if  (Separateur==S_Virgule)
	FSep[0]=',';
	else if  (Separateur==S_PointVirgule)
	FSep[0]=';';
	else if  (Separateur==S_Espace)
	FSep[0]=' ';
	else //if (Separateur==S_Tabulation)
	FSep[0]='\t';

	float fMin,fMax;
	P_FindMinMax(P,fMin,fMax);

	int x,y;
	for(y=0;y<P_SizeY(P);y++)
	{
		for(x=0;x<P_SizeX(P)-1;x++)
		{
			char Val[64];
			Val[0]=0;
			float D=P_Data(P)[x+P_SizeX(P)*y];
			if((P_Msk(P)&&((P_Msk(P)[x+P_SizeX(P)*y])!=0))) D=fMin;
			CF_GetString(Val,D,ChiffSignif);
			strcat(Val,FSep);
			fwrite(Val,strlen(Val),1,F);
		}
			char Val[64];
			Val[0]=0;
			float D=P_Data(P)[x+P_SizeX(P)*y];
			if((P_Msk(P)&&((P_Msk(P)[x+P_SizeX(P)*y])!=0))) D=fMin;
			CF_GetString(Val,D,ChiffSignif);
			strcat(Val,P_RET);
			fwrite(Val,strlen(Val),1,F);
	}

	fclose(F);
	return -1;
}


#endif

