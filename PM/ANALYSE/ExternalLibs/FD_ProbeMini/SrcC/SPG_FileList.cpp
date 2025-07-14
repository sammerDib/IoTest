
#include "SPG_General.h"

#ifdef SPG_General_USEFileList

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

void SPG_CONV FileListRecurseDir(SPG_FL& FL, char* WorkDir, int Flag)
{
	WIN32_FIND_DATA FD;
	char DName[FileListMaxFileName];

	//printf(WorkDir);
	//printf("\n");

	SPG_ConcatPath(DName,WorkDir,"*");

	int OldCount=FL.Count;

	SPG_StackAllocZ(SPG_FL,PrivateDIR);//liste des repertoires
	//SPG_SpecName(SpecListCreate,SPG_FL_FILE) est un nom de fonction implémenté dans SPG_FileListSpec
	SPG_SpecName(SpecListCreate,SPG_FL_FILE)(PrivateDIR);

	HANDLE HD=FindFirstFile(DName,&FD);
	if(HD==INVALID_HANDLE_VALUE) return;
	do
	{
		if(strcmp(FD.cFileName,".")==0) 
			continue;
		else if(strcmp(FD.cFileName,"..")==0) 
			continue;
		else if(FD.dwFileAttributes&FILE_ATTRIBUTE_DIRECTORY)
		{
			SPG_ConcatPath(DName,WorkDir,FD.cFileName);
			if(strcmp(DName,WorkDir)!=0)//protection contre le sur-place
			{
			//FileListRecurseDir(FL,DName,Flag);
			SPG_FL_FILE* PrivateFN=FileListAdd(PrivateDIR);//retourne un pointeur sur le nouvel element
			strcpy(PrivateFN->Name,DName);
			PrivateFN->PathLen=strlen(DName)+1;
			//PrivateDIR->NameLen=0;
			//PrivateDIR->ExtLen=0;
			}
		}
		else
		{
			SPG_ConcatPath(DName,WorkDir,FD.cFileName);
			SPG_FL_FILE* FN=FileListAdd(FL);//retourne un pointeur sur le nouvel element
			if(FN==0) break;
			strcpy(FN->Name,DName);

			int N=(int)SPG_NameOnly(DName);
			FN->PathLen=N-(int)DName;//le chemin contient le dernier antislash
			int M=(int)SPG_ExtensOnly(DName);
			FN->NameLen=M-N;//le nom contient le point
			FN->ExtLen=strlen(DName)-FN->PathLen-FN->NameLen;//l'extension ne contient pas le point
			FN->Size=FD.nFileSizeLow;
			FN->AccessDate=*(__int64*)&(FD.ftLastAccessTime);
			FN->WriteDate=*(__int64*)&(FD.ftLastWriteTime);
			//ordre des colonnes: Size AccessDate WriteDate
		}
	} while(FindNextFile(HD,&FD));

	FindClose(HD);

	if((Flag&FILELIST_NOSORTING)==0) FileListSort(FL,OldCount,FL.Count);

	FileListSort(PrivateDIR,0,PrivateDIR.Count);
	for(int i=0;i<PrivateDIR.Count;i++)
	{
		strcat(FileListGet(PrivateDIR,i)->Name,"\\");//les noms de dossiers doivent se terminer par \ dans la convention de srcc
		FileListRecurseDir(FL,FileListGet(PrivateDIR,i)->Name,Flag);
	}
	FileListClose(PrivateDIR);
}

int SPG_CONV FileListCreate(SPG_FL& FL, char* WorkDir, int Flag)
{
	if(SPG_SpecName(SpecListCreate,SPG_FL_FILE)(FL)) FileListRecurseDir(FL,WorkDir,Flag);

	return FL.Etat;
}

void SPG_CONV FileListSave(SPG_FL& FL, char* Filename, int Flag)
{
	CHECK(FL.Etat!=-1,"FileListPrint: Liste invalide",return);
	FILE* F=fopen(Filename,"wb+");
	CHECKTWO(F==0,"FileListSave: can not create file",Filename,return);

	char Line[1024];
	strcpy(Line,"Path\tPath\tPath\tPath\tPath\tPath\tPath\tPath\tSize\tAccess\tWrite\tAccess\tWrite\r\n");
	fwrite(Line,strlen(Line),1,F);

	if(F)
	{
		for(int i=0;i<FL.Count;i++)
		{
			if(Flag&FILELIST_SAVE_EXEL)
			{
				int TabLevel=8;
				char* L=Line;
				for(int m=0;m<strlen(FileListGet(FL,i)->Name);m++)
				{
					char c=FileListGet(FL,i)->Name[m];
					if(c=='\\')
					{
						*L++='\t';
						TabLevel--;
					}
					else
						*L++=c;
				}
				if(Flag&(FILELIST_SAVE_SIZE|FILELIST_SAVE_DATE))
				{
					while(TabLevel-->0) 
					{
						*L++='\t';
					}
				}
				*L=0;
			}
			else
			{
				strcpy(Line,FileListGet(FL,i)->Name);
			}
			if(Flag&FILELIST_SAVE_SIZE)
			{
				char Size[32];
				itoa(FileListGet(FL,i)->Size,Size,10);
				strcat(Line,"\t");
				strcat(Line,Size);
			}
			if(Flag&FILELIST_SAVE_DATE)
			{
				SYSTEMTIME SystemAccessTime;
				SYSTEMTIME SystemWriteTime;
				FileTimeToSystemTime((FILETIME*)&(FileListGet(FL,i)->AccessDate),&SystemAccessTime);
				FileTimeToSystemTime((FILETIME*)&(FileListGet(FL,i)->WriteDate),&SystemWriteTime);
				char Date[128];
				sprintf(Date,"\t%I64i\t%I64i\t%0.2i/%0.2i/%0.2i %0.2i:%0.2i\t%0.2i/%0.2i/%0.2i %0.2i:%0.2i",
					FileListGet(FL,i)->AccessDate,FileListGet(FL,i)->WriteDate,
					(int)SystemAccessTime.wDay,(int)SystemAccessTime.wMonth,(int)SystemAccessTime.wYear,(int)SystemAccessTime.wHour,(int)SystemAccessTime.wMinute,
					(int)SystemWriteTime.wDay,(int)SystemWriteTime.wMonth,(int)SystemWriteTime.wYear,(int)SystemWriteTime.wHour,(int)SystemWriteTime.wMinute);
				strcat(Line,Date);
			}
			//ordre des colonnes: Size AccessDate WriteDate
			strcat(Line,"\r\n");
			fwrite(Line,strlen(Line),1,F);
		}
		fclose(F);
	}
	return;
}

void SPG_CONV DirListSave(SPG_FL& FL, char* Filename)
{
	CHECK(FL.Etat!=-1,"FileListPrint: Liste invalide",return);
	FILE* F=fopen(Filename,"wb+");
	if(F)
	{
		char Path[1024];
		Path[0]=0;
		int Size=0;
		int FileCount=0;
		for(int i=0;i<FL.Count;i++)
		{

			char CurrentPath[1024];
			strcpy(CurrentPath,FileListGet(FL,i)->Name);
			SPG_PathOnly(CurrentPath);
			if(strcmp(Path,CurrentPath)||(i==(FL.Count-1)))
			{
				if(i==(FL.Count-1))
				{
			Size+=FileListGet(FL,i)->Size;
			FileCount++;
				}

				if(Path[0])
				{
					char Line[1024];
					strcpy(Line,Path);
					strcat(Line,"\t");
					char SSize[16];
					itoa(Size,SSize,10);
					strcat(Line,SSize);
					strcat(Line,"\t");
					itoa(FileCount,SSize,10);
					strcat(Line,SSize);
					strcat(Line,"\r\n");
					fwrite(Line,strlen(Line),1,F);
				}
				strcpy(Path,CurrentPath);
				Size=0;
				FileCount=0;
			}

			Size+=FileListGet(FL,i)->Size;
			FileCount++;

		}
		fclose(F);
	}
	return;
}

bool SPG_CONV FileListCmpXchg(SPG_FL_FILE& F1, SPG_FL_FILE& F2)
{
	int c,d;
	if(
		((c=_strnicmp(F1.Name,F2.Name,V_Max(0,(V_Min(F1.PathLen,F2.PathLen)-1))))>0)||
		(
			(c==0)&&
			(
				(F1.PathLen>F2.PathLen)||
				(
					(F1.PathLen==F2.PathLen)&&
					(
						((d=stricmp(F1.Name+F1.PathLen+F1.NameLen,F2.Name+F2.PathLen+F2.NameLen))>0)||
						(
							(d==0)&&
							(stricmp(F1.Name+F1.PathLen,F2.Name+F1.PathLen)>0)
						)
					)
				)
			)
		)
	)
	{
		V_Swap(SPG_FL_FILE,F1,F2);
		return true;
	}
	return false;
}

bool SPG_CONV FileListCmpXchgTst(SPG_FL_FILE& F1, SPG_FL_FILE& F2)
{
	int c,d;
	if(
		((c=_strnicmp(F1.Name,F2.Name,V_Max(0,(V_Min(F1.PathLen,F2.PathLen)-1))))>0)||
		(
			(c==0)&&
			(
				(F1.PathLen>F2.PathLen)||
				(
					(F1.PathLen==F2.PathLen)&&
					(
						((d=stricmp(F1.Name+F1.PathLen+F1.NameLen,F2.Name+F2.PathLen+F2.NameLen))>0)||
						(
							(d==0)&&
							(stricmp(F1.Name+F1.PathLen,F2.Name+F1.PathLen)>0)
						)
					)
				)
			)
		)
	)
	{
		//V_Swap(SPG_FL_FILE,F1,F2);
		return true;
	}
	return false;
}

void SPG_CONV FileListSort(SPG_FL& FL,int StartBound, int StopBound)
{
	CHECK(FL.Etat!=-1,"FileListShort: Liste invalide",return);

	int FirstXchg;
	int LastXchg = 0;
	int nStartBound=StartBound;
	int nStopBound=StopBound;
	do
	{
		FirstXchg=-1;
		for(int i=nStartBound;i<(nStopBound-1);i++)
		{
			if(FileListCmpXchg(*FileListGet(FL,i),*FileListGet(FL,i+1)))
			{//echange
				if(FirstXchg==-1) FirstXchg=i;
				LastXchg=i;//initialise LastXchg si un echange a eu lieu
			}
		}
		nStartBound=V_Max(StartBound,(FirstXchg-1));
		nStopBound=V_Min(StopBound,(LastXchg+2)); //utilisé seulement si le while reboucle ce qui est le cas seuleemnt si un echange a eu lieu
	} while(FirstXchg!=-1);
/*
	int newStart=StartBound=V_Max(StartBound,0);
	int newStop=StopBound=V_Min(StopBound-1,FL.Count-1);
	while(1)
	{
		//printf("Tri %i à %i\n",newStart,newStop);
		int Start,Stop;
		for(int i=newStart;i<newStop;i++)
		{
			if(FileListCmpXchg(*FileListGet(FL,i),*FileListGet(FL,i+1)))
			{
				Start=i;
				Stop=i;
				break;
			}
		}
		if(i>=newStop) break;//si start=stop ou stop=start+1 dans les arguments ca ne trie pas
		for(;i<newStop;i++)
		{
			if(FileListCmpXchg(*FileListGet(FL,i),*FileListGet(FL,i+1)))
			{
				Stop=i;
			}
		}
		if(Stop<StopBound) Stop++;
		if(Start>StartBound) Start--;
		newStart=Start;
		newStop=Stop;
	}
*/
#ifdef DebugList
	{for(int i=StartBound;i<StopBound-1;i++)
	{
		CHECK(FileListCmpXchgTst(*FileListGet(FL,i),*FileListGet(FL,i+1)),"FileListSort: Tri intermediaire incomplet",break);
	}}
	{for(int i=0;i<FL.Count-1;i++)
	{
		CHECK(FileListCmpXchgTst(*FileListGet(FL,i),*FileListGet(FL,i+1)),"FileListSort: Tri incomplet",break);
	}}
#endif
	return;
}

#endif
