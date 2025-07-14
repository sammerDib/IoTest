
#include "SPG_General.h"

#ifdef SPG_General_USEFiles

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <stdio.h>
#include <string.h>
#include <ctype.h>

char* SPG_CONV SPG_StrFind(char* Complete, char* Item)
{//renvoie la position à l'origine de la chaine cherchée 
	if(Complete==0) return 0;
	if(Item==0) return 0;

	intptr_t L=strlen(Complete);
	if(L==0) return 0;
	
	intptr_t LI=strlen(Item);
	if(LI==0) return 0;
	
	intptr_t i;
	for(i=0;i<=L-LI;i++)
	{
		if(toupper(Complete[i])==toupper(*Item))
		{
			int j;
			for(j=1;j<LI;j++)
			{
				if(toupper(Complete[i+j])!=toupper(Item[j])) break;
			}
			if(j==LI) break;
		}
	}
	if(i<=L-LI) 
		return Complete+i;
	else
		return 0;
}

char* SPG_CONV SPG_StrFindAtEnd(char* Complete, char* Item)
{//renvoie la position à l'extrémité la chaine cherchée 
	if(Complete==0) return 0;
	if(Item==0) return 0;

	//int L=strlen(Complete);
	//if(L==0) return 0;
	
	intptr_t LI=strlen(Item);
	if(LI==0) return 0;
	
	bool Found=false;
	intptr_t i;
	for(i=0;;i++)
	{
		if(Complete[i]==0) break;
		if(toupper(Complete[i])==toupper(*Item))
		{
			int j;
			for(j=1;j<LI;j++)
			{
				if(Complete[i+j]==0) break;
				if(toupper(Complete[i+j])!=toupper(Item[j])) break;
			}
			if(j==LI) {Found=true;break;}
		}
	}
	if(Found) 
		return Complete+i+LI;
	else
		return 0;
}

char* SPG_CONV SPG_StrFindCharAtEnd_unsafe(char* Complete, char Item)
{//renvoie la position à l'extrémité la chaine cherchée 
	while(1)
	{
		if( *Complete == 0) return 0;
		if( *Complete++ == Item) return Complete;
	}
}

char* SPG_CONV SPG_StrLineHeadFind(char* Complete, char* Item)
{//on cherche une chaine sur un debut de ligne
	if(Complete==0) return 0;
	if(Item==0) return 0;

	intptr_t L=strlen(Complete);
	if(L==0) return 0;
	
	intptr_t LI=strlen(Item);
	if(LI==0) return 0;
	
	intptr_t i;
	for(i=0;i<=L-LI;i++)
	{
		if((toupper(Complete[i])==toupper(*Item))&&((i==0)||(Complete[i-1]=='\n')))
		{
			int j;
			for(j=1;j<LI;j++)
			{
				if(toupper(Complete[i+j])!=toupper(Item[j])) break;
			}
			if(j==LI) break;
		}
	}
	if(i<=L-LI) 
		return Complete+i;
	else
		return 0;
}

int SPG_CONV SPG_IsAbsolutePath(const char * Path)
{
	if(strlen(Path)<2) return 0;
	if( V_InclusiveBound(toupper(Path[0]),'A','Z') && (Path[1]==':') ) return -1; // e:
	if((Path[0]=='\\')||(Path[0]=='/')) return -1; // \root
	//if((Path[0]=='\\')&&(Path[1]=='\\')) return -1; // \\samba\xyz
	return 0;
}

int SPG_CONV SPG_IsRootPath(char* Path)
{
	//???? lettre:[\]
	if(Path==0) return 0;
	if(Path[0]==0) return 0;		if((!V_IsBound(Path[0],'a','z'))&&(!V_IsBound(Path[0],'A','Z'))) return 0;
	Path++;
	if(Path[0]==0) return 0;		if(Path[0]!=':') return 0;
	Path++;
	if(Path[0]==0) return -1;	if((Path[0]!='\\')&&(Path[0]!='/')) return 0;
	Path++;
	if(Path[0]==0) return -1;	return 0;
}

void SPG_CONV SPG_OptimizePath(char * AnyPath, int bWarnAbs)
{
	if(AnyPath==0) return;
	intptr_t ls=strlen(AnyPath);
	char* d=AnyPath;

	for(char * i=AnyPath;i<AnyPath+ls;i++)
	{
		if (i<(AnyPath+ls-2))
		{// recherche de \DriveLetter:
			if((//Si un chemin absolu (ex:sur un autre volume) est introduit il commence par '\' ou '/'
				(*i=='\\')&& V_InclusiveBound( toupper(*(i+1)),'A','Z' ) && (*(i+2)==':')
				)||(
				(*i=='/')&& V_InclusiveBound( toupper(*(i+1)),'A','Z' ) && (*(i+2)==':')
				))
			{
#ifdef DebugList	
				SPG_List2S("SPG_OptimizePath - please use SPG_ConcatPath to concat strings",AnyPath);
#endif
				d=AnyPath;//recule à la racine du chemin
				continue;
			}
		}
		if (i<(AnyPath+ls-1))
		{// recherche de path\\nameorpath ou path\\\samba
			if((
				(*i=='\\')&&(*(i+1)=='\\')
				)||(
				(*i=='/')&&(*(i+1)=='/')
				))
			{
			if(i==AnyPath)
			{
				//pas de pb, chemin absolu type \\samba\xyz
			}
			else
			{
#ifdef DebugList
				SPG_List2S("SPG_OptimizePath - please use SPG_ConcatPath to concat strings",AnyPath);
#endif
				d=AnyPath+1;//recule à la racine du chemin sans le premier backslash qui venait supposément de la terminaison du chemin de gauche
			continue;
			}
			}
		}
		if (i<(AnyPath+ls-2))
		{// recherche de \.\ et de /./
			if((
				(*i=='\\')&&(*(i+1)=='.')&&(*(i+2)=='\\')
				)||(
				(*i=='/')&&(*(i+1)=='.')&&(*(i+2)=='/')
				))
			{
				i++;
				continue;
			}
		}
		if (i<(AnyPath+ls-3))
		{// recherche de \..\ et de /../
			if((
				(*i=='\\')&&(*(i+1)=='.')&&(*(i+2)=='.')&&(*(i+3)=='\\')
				)||(
				(*i=='/')&&(*(i+1)=='.')&&(*(i+2)=='.')&&(*(i+3)=='/')
				))
			{
				while(d>AnyPath)
				{//recule jusqu'au backslask parent
					d--;
					if(*d=='\\') break; //sera ecrase a la prochaine ecriture
					if(*d=='/') break; //sera ecrase a la prochaine ecriture
				}
				i+=2;
				continue;
			}
		}
		if(d!=i) *d=*i;
		d++;
	}
	*d=0;
	return;
}

void SPG_CONV SPG_MakeCleanFileName(char *DstName, const char* Name)
{
	CHECKTWO(DstName==0,"SPG_MakeCleanFileName: Destination nulle",DstName,return);
	CHECKTWO(Name==0,"SPG_MakeCleanFileName: Source nulle",Name,return);

	char* d=DstName-1;
	const char* s=Name;

	while(*++d=*s++) // assignement volontaire
    { 
        CHECK(s-Name>255,"SPG_MakeCleanFileName",*d=0;break;); 
        if((!V_IsBound(*d,'a','z'))&&(!V_IsBound(*d,'A','Z'))&&(!V_IsBound(*d,'0','9'))) *d='_'; 
    }

	return;
}


#ifdef SPG_DEBUGCONFIG

void SPG_CONV SPG_ConcatPath_dbg(char *FullName,const char *WorkDir,const char *Name, const char* Msg)
{
	CHECKTWO(FullName==0,"SPG_ConcatPath: Destination nulle",Msg,return);
	CHECKTWO(Name==0,"SPG_ConcatPath: Source nulle",Msg,return);

	intptr_t l=0;
	if(WorkDir) l=strlen(WorkDir);
	if (l!=0)
	{
		if(FullName!=WorkDir) strcpy(FullName,WorkDir);
		if ((FullName[l-1]!='\\')&&(FullName[l-1]!='/'))
		{//ajout du backslash terminal
			DbgCHECKTHREE(1,"SPG_ConcatPath: Path missing the final backslash",WorkDir,Name);
			FullName[l]='\\';
			FullName[l+1]=0;
		}
		if(SPG_IsAbsolutePath(Name))
		{
			strcpy(FullName,Name);
		}
		else
		{
			strcat(FullName,Name);
		}
	}
	else
	{
		strcpy(FullName,Name);
	}

	SPG_OptimizePath(FullName);

	return;
}

#else

void SPG_CONV SPG_ConcatPath(char *FullName, const char *WorkDir, const char *Name)
{
	CHECK(FullName==0,"SPG_ConcatPath: Destination nulle",return);
	CHECK(Name==0,"SPG_ConcatPath: Source nulle",return);

	intptr_t l=0;
	if(WorkDir) l=strlen(WorkDir);
	if (l!=0)
	{
		if(FullName!=WorkDir) strcpy(FullName,WorkDir);
		if ((FullName[l-1]!='\\')&&(FullName[l-1]!='/'))
		{//ajout du backslash terminal
			DbgCHECKTHREE(1,"SPG_ConcatPath: Path missing the final backslash",WorkDir,Name);
			FullName[l]='\\';
			FullName[l+1]=0;
		}
		if(SPG_IsAbsolutePath(Name))
		{
			strcpy(FullName,Name);
		}
		else
		{
			strcat(FullName,Name);
		}
	}
	else
	{
		strcpy(FullName,Name);
	}

	SPG_OptimizePath(FullName);

	return;
}

#endif

void SPG_CONV SPG_PathOnly(char * PathAndFilename)
{
	if(PathAndFilename==0) return;
	intptr_t l=strlen(PathAndFilename);
	intptr_t i;
	for(i=l-1;i>=0;i--)
	{
		if (PathAndFilename[i]=='\\') break;
		if (PathAndFilename[i]=='/') break;
	}
	if (i<0) 
		strcat(PathAndFilename,"\\");
	else
		PathAndFilename[i+1]=0;
	return;
}

char* SPG_CONV SPG_NameOnly(char * PathAndFilename)
{
	if(PathAndFilename==0) return 0;
	intptr_t l=strlen(PathAndFilename);
	intptr_t i;
	for(i=l-1;i>=0;i--)
	{
		if (PathAndFilename[i]=='\\') break;
		if (PathAndFilename[i]=='/') break;
	}
	if (i<0) 
		return PathAndFilename;
	else
		return PathAndFilename+i+1;
}

char* SPG_CONV SPG_ExtensOnly(char *FullName)
{
	if(FullName==0) return 0;
	intptr_t l=strlen(FullName);
	intptr_t i;
	for(i=l-1;i>=0;i--)
	{
		if(FullName[i]=='.') break;
	}
	if (i<0)
		return FullName+l;
	else
		return FullName+i+1;
}

int SPG_CONV SPG_GetExtens(char *FullName)
{
	if(FullName==0) return 0;
	char* Extens=SPG_ExtensOnly(FullName);
	if (_stricmp(Extens,"txt")==0) return SPG_TXT;
	if (_stricmp(Extens,"bmp")==0) return SPG_BMP;
	if (_stricmp(Extens,"avi")==0) return SPG_AVI;
	if (_stricmp(Extens,"wav")==0) return SPG_WAV;
	if (_stricmp(Extens,"rgr")==0) return SPG_RGR;
	if (_stricmp(Extens,"tpo")==0) return SPG_TPO;
	if (_stricmp(Extens,"rs" )==0) return SPG_RS;
	if (_stricmp(Extens,"raw")==0) return SPG_RAW;
	if (_stricmp(Extens,"ai")==0) return SPG_AI;
	if (_stricmp(Extens,"di")==0) return SPG_DI;
	CD_G_CHECK_EXIT(25,17);

	return 0;
}

int SPG_CONV SPG_SetExtens(char *FullName,const char *Extens)//Extens doit contenir le point
{
	if((FullName==0)||(Extens==0)) return 0;
	intptr_t l;
	l=strlen(FullName);
	if (l==0) return 0;

	intptr_t i;
	for(i=l-1;i>=0;i--)
	{
		if(FullName[i]=='.') break;
		if(FullName[i]=='\\') {i=-1;break;}
		if(FullName[i]=='/') {i=-1;break;}
	}
	
	if (i<0)
	{
		i=l;
	}
	/*
	if (_stricmp(FullName+i,".txt")==0) return SPG_TXT;
	if (_stricmp(FullName+i,".bmp")==0) return SPG_BMP;
	*/
	if(Extens) strcpy(FullName+i,Extens); else FullName[i]=0;

	return -1;
}

BYTE* SPG_CONV SPG_LoadFileAlloc(const char* FileName, int& FileLen, int WarningLevel, int numTermZ)
{
	if(FileName==0) return 0;
	if(FileName[0]==0) return 0;
	FILE* F=fopen(FileName,"rb");
	CHECKWTWO(WarningLevel,F==0,"SPG_LoadFile failed",FileName,FileLen=0;return 0);
	
	fseek(F,0,SEEK_END);
	FileLen=ftell(F);
	fseek(F,0,SEEK_SET);
	
	CHECKWTWO(WarningLevel,FileLen<=0,"SPG_LoadFile",FileName,fclose(F);FileLen=0;return 0);

	BYTE* D=SPG_TypeAlloc(FileLen+numTermZ,BYTE,"SPG_LoadFile:");//assure la présence d'un zero terminal
	if(D==0) return 0;
	SPG_CatMemName(D,SPG_NameOnly((char*)FileName));
	CHECKWTWO(WarningLevel,fread(D,FileLen,1,F)==0,"SPG_LoadFile",FileName,;);
	fclose(F);
	return D;
}

int SPG_CONV SPG_LoadFileMatchSize(char* FileName, BYTE* Data, int DataLen, int WarningLevel)
{
	if(FileName==0) return 0;
	if(FileName[0]==0) return 0;
	if(Data==0) return 0;
	if(DataLen==0) return 0;
	FILE* F=fopen(FileName,"rb");
	CHECKWTWO(WarningLevel,F==0,"SPG_LoadFile failed",FileName,return 0);
	
	fseek(F,0,SEEK_END);
	CHECKWTWO(WarningLevel,DataLen!=ftell(F),"SPG_LoadFile: Size mismatch",FileName,fclose(F);return 0);
	fseek(F,0,SEEK_SET);

	CHECKWTWO(WarningLevel,fread(Data,DataLen,1,F)==0,"SPG_LoadFile",FileName,;);
	fclose(F);
	return -1;
}

int SPG_CONV SPG_LoadFileUpTo(char* FileName, BYTE* Data, int& DataLen, int WarningLevel)
{
	if(FileName==0) return 0;
	if(FileName[0]==0) return 0;
	if(Data==0) return 0;
	if(DataLen==0) return 0;
	FILE* F=fopen(FileName,"rb");
	CHECKWTWO(WarningLevel,F==0,"SPG_LoadFile failed",FileName,return 0);
	/*
	fseek(F,0,SEEK_END);
	CHECKWTWO(WarningLevel,DataLen!=ftell(F),"SPG_LoadFile: Size mismatch",FileName,fclose(F);return 0);
	fseek(F,0,SEEK_SET);
	*/

	DataLen=(int)fread(Data,1,DataLen,F);

	fclose(F);
	return -1;
}

int SPG_CONV SPG_SaveFile(char* FileName, BYTE* Data, int DataLen, int WarningLevel)
{
	if(FileName==0) return 0;
	if(FileName[0]==0) return 0;
	if(Data==0) return 0;
	if(DataLen==0) return 0;
	FILE* F=fopen(FileName,"wb+");
	CHECKWTWO(WarningLevel,F==0,"SPG_SaveFile failed",FileName,return 0);
	DataLen=(int)fwrite(Data,DataLen,1,F);
	fclose(F);
	return DataLen;
}

int SPG_CONV SPG_CopyFile(char* DstFileName, char* SrcFileName, int WarningLevel)
{
	int DataLen=0;
	BYTE* Data=SPG_LoadFileAlloc(SrcFileName,DataLen,WarningLevel);
	if(Data)
	{
		int S=SPG_SaveFile(DstFileName,Data,DataLen,WarningLevel);
		SPG_MemFree(Data);
		return S;
	}
	else
	{
		return 0;
	}
}

int SPG_CONV SPG_DuplicateFile(char* DstPath, char* SrcFileName, int WarningLevel)
{
	char DstFileName[MaxProgDir];
	SPG_ConcatPath(DstFileName,DstPath,SPG_NameOnly(SrcFileName));
	return SPG_CopyFile(DstFileName,SrcFileName,WarningLevel);
}

#ifdef SPG_General_USEFilesWindows

int SPG_CONV SPG_GetFileSize(char* f, int& Size)
{
	Size=0; if(f==0) return 0; if(f[0]==0) return 0;

	HANDLE hFile = CreateFile(fdwstring(f), GENERIC_READ, FILE_SHARE_READ, 
					 NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);//READ

	//CHECKTWO(hFile==INVALID_HANDLE_VALUE,"SPG_GetFileSize : Open failed",f,return);

	if(hFile==INVALID_HANDLE_VALUE) {Size=0;return 0;}
	
	Size=GetFileSize(hFile,0);
	if(Size==INVALID_FILE_SIZE) Size=0;
	CloseHandle(hFile);
	return -1;
}


UINT CALLBACK F_InternalGenericHookProc(
  HWND hdlg,      // handle to child dialog window
  UINT uiMsg,     // message identifier
  WPARAM wParam,  // message parameter
  LPARAM lParam   // message parameter
)
{
	return 0;
}

#ifdef FDE
int SPG_CONV SPG_GetLoadName(int Type, char* ResultFile, int LenResultFile)
{
	SPG_StackAllocZ(OPENFILENAME,OFN);

	OFN.lStructSize=sizeof(OFN);
#ifdef SPG_General_USEGlobal
	OFN.hwndOwner=(HWND)Global.hWndWin;
	OFN.hInstance=(HINSTANCE)Global.hInstance;
#endif
	switch (Type)
	{
	case SPG_TXT:
	OFN.lpstrFilter="Texte\0*.txt\0\0";
		break;
	case SPG_BMP:
	OFN.lpstrFilter="Bitmap\0*.bmp\0\0";
		break;
	case SPG_AVI:
	OFN.lpstrFilter="Video\0*.avi\0\0";
		break;
	case SPG_WAV:
	OFN.lpstrFilter="Son\0*.wav\0\0";
		break;
	case SPG_RGR:
	OFN.lpstrFilter="Sequence\0*.rgr\0\0";
		break;
	case SPG_TPO:
	OFN.lpstrFilter="Topographie\0*.tpo\0\0";
		break;
	case SPG_RS:
	OFN.lpstrFilter="Raw short\0*.rs\0\0";
		break;
	case SPG_RAW:
	OFN.lpstrFilter="Raw binary\0*.raw\0\0";
		break;
	case SPG_AI:
	OFN.lpstrFilter="Raw ai\0*.ai\0\0";
		break;
	case SPG_DI:
	OFN.lpstrFilter="Raw di\0*.di\0\0";
		break;
	default:
	OFN.lpstrFilter="*.*\0\0";
//		return 0;
	}
	OFN.lpstrFile=ResultFile;
	OFN.nMaxFile=LenResultFile;
	OFN.lpstrFileTitle=0;
	OFN.nMaxFileTitle=0;
#ifdef SPG_General_USEGlobal
	OFN.lpstrInitialDir=Global.CurDir;
#endif
	OFN.lpstrTitle="Charger";
	OFN.Flags=OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST|OFN_EXPLORER|OFN_ENABLEHOOK;
	OFN.nFileOffset=0;//retourne la position du nom hors chemin
	OFN.nFileExtension=0;//idem pour l'extension
	OFN.lpstrDefExt=0;
	OFN.lCustData=0;
	OFN.lpfnHook=F_InternalGenericHookProc;
	OFN.lpTemplateName=0;

	//SPG_WaitMouseRelease();
	DoEvents(SPG_DOEV_MIN);

	if(GetOpenFileName(&OFN))//if fails to open, check the ResultFile name is already initialized before the call otherwise GetLastError returns 87 invalid parameter
	{
		SPG_StackCheck(OFN);
		SPG_SetCurDirFrom(ResultFile);
		SPG_WaitMouseRelease();
		int E=SPG_GetExtens(ResultFile);
		if (E!=Type) return 0;
		return -1;
	}
	else
	{
		SPG_WaitMouseRelease();
		return 0;
	}
}

int SPG_CONV SPG_GetSaveName(int Type, char* ResultFile, int LenResultFile)
{
	SPG_StackAllocZ(OPENFILENAME,OFN);

	OFN.lStructSize=sizeof(OFN);
#ifdef SPG_General_USEGlobal
	OFN.hwndOwner=(HWND)Global.hWndWin;
	OFN.hInstance=(HINSTANCE)Global.hInstance;
#endif
	switch (Type)
	{
	case SPG_TXT:
	OFN.lpstrFilter="Texte\0*.txt\0\0";
		break;
	case SPG_BMP:
	OFN.lpstrFilter="Bitmap\0*.bmp\0\0";
		break;
	case SPG_AVI:
	OFN.lpstrFilter="Video\0*.avi\0\0";
		break;
	case SPG_WAV:
	OFN.lpstrFilter="Son\0*.wav\0\0";
		break;
	case SPG_RGR:
	OFN.lpstrFilter="Sequence\0*.rgr\0\0";
		break;
	case SPG_TPO:
	OFN.lpstrFilter="Topographie\0*.tpo\0\0";
		break;
	case SPG_RS:
	OFN.lpstrFilter="Raw short\0*.rs\0\0";
		break;
	default:
		return 0;
	}
	OFN.lpstrFile=ResultFile;
	OFN.nMaxFile=LenResultFile;
	OFN.lpstrFileTitle=0;
	OFN.nMaxFileTitle=0;
#ifdef SPG_General_USEGlobal
	OFN.lpstrInitialDir=Global.CurDir;
#endif
	OFN.lpstrTitle="Enregistrer";
	OFN.Flags=OFN_OVERWRITEPROMPT|OFN_PATHMUSTEXIST|OFN_EXPLORER|OFN_ENABLEHOOK;
	OFN.nFileOffset=0;//retourne la position du nom hors chemin
	OFN.nFileExtension=0;//idem pour l'extension
	OFN.lpstrDefExt=0;
	OFN.lCustData=0;
	OFN.lpfnHook=F_InternalGenericHookProc;
	OFN.lpTemplateName=0;

//SPG_GetSaveName_AskAgain

	DoEvents(SPG_DOEV_MIN);

	if (GetSaveFileName(&OFN))
	{
		SPG_StackCheck(OFN);
		SPG_SetCurDirFrom(ResultFile);
		SPG_WaitMouseRelease();
		int E=SPG_GetExtens(ResultFile);
		if (E!=Type)
		{
			if (OFN.nFilterIndex==1) 
			{
			if (Type==SPG_TXT)
				SPG_SetExtens(ResultFile,".txt");
			else if (Type==SPG_BMP)
				SPG_SetExtens(ResultFile,".bmp");
			else if (Type==SPG_AVI)
				SPG_SetExtens(ResultFile,".avi");
			else if (Type==SPG_WAV)
				SPG_SetExtens(ResultFile,".wav");
			else if (Type==SPG_RGR)
				SPG_SetExtens(ResultFile,".rgr");
			else if (Type==SPG_TPO)
				SPG_SetExtens(ResultFile,".tpo");
			else return 0;
			}
			else return 0;
		}
		/*
		HANDLE File=0;
		if((File=CreateFile(ResultFile,GENERIC_READ|GENERIC_WRITE,0,0,OPEN_EXISTING,0,0))!=INVALID_HANDLE_VALUE)
		{
			CloseHandle(File);
			char Msg[1024];
			strcpy(Msg,ResultFile);
			strcat(Msg,"\nExite déjà\nOverwrite ?");
			if(MessageBox((HWND)Global.hWndWin,Msg,"Enregistrement de fichier",MB_OKCANCEL)==IDOK)
			{
				DeleteFile(ResultFile);
			}
			else 
			{
				goto SPG_GetSaveName_AskAgain;
				//return 0;
			}
		}
		*/
		return -1;
	}
	else
	{
		SPG_WaitMouseRelease();
		return 0;
	}
}
#endif //FDE
#endif
#endif

