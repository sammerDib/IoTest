
#ifdef SPG_General_USEFileList

#define FileListFilePerBlock 256
#define FileListMaxBlock 256
#define FileListMaxFileName 512

typedef struct
{
	char Name[FileListMaxFileName];
	int PathLen;
	int NameLen;
	int ExtLen;
	int Size;
	__int64 AccessDate;
	__int64 WriteDate;
} SPG_FL_FILE;

/////////////////////////////////////////////////

//declaration de la liste de type SPG_FL_FILE

#define SPG_SpecType SPG_FL_FILE
#define SPG_SpecBlockSize FileListFilePerBlock
#define SPG_SpecMaxBlock FileListMaxBlock

#include "SpecList.h"

//////////////////////////////////////////////////

#define SPG_FL SPG_SpecName(SPECLIST,SPG_FL_FILE) 
#define SPG_FL_FILEBLOCK SPG_SpecName(SPECLIST,SPG_FL_FILE) 

#define FileListAdd(FL) SPG_SpecName(SpecListAdd,SPG_FL_FILE)(FL) 
#define FileListGet(FL,Nr) SPG_SpecName(SpecListGet,SPG_FL_FILE)(FL,Nr) 
#define FileListClose(FL) SPG_SpecName(SpecListClose,SPG_FL_FILE)(FL) 

#define FILELIST_SAVE_SIZE 1
#define FILELIST_SAVE_EXEL 2
#define FILELIST_SAVE_DATE 4
#define FILELIST_NOSORTING 8

#include "SPG_FileList.agh"

#endif

