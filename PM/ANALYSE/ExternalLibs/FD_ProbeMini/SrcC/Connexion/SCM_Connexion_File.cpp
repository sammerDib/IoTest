
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION
#ifdef SPG_General_USEWindows

#include "..\SPG_Includes.h"
#include "..\SPG_SysInc.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>


// ###################################################

#include "SCM_Connexion_File_Internal.h"

#include "SCM_ExtensOverwrite.h"

#define sci_UID sci_UID_FILE
#define sci_NAME sci_NAME_FILE

// ###################################################

#define DEFAULTFILENAME "scxFILE.bin"

typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	//int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire
	int OpenMode;
	DWORD dwDesiredAccess;
	DWORD dwShareMode;
	DWORD dwCreationDisposition;
	DWORD dwFlagsAndAttributes;
    SECURITY_ATTRIBUTES SecurityAttributes;

	char FileName[SCX_LENFILENAME];
	int Loop;
} SCX_ADDRESS;


// ###################################################

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;
	HANDLE hFile;
	//...
} SCX_STATE; //parametres d'une connexion en particulier


// ###################################################

static SCX_EXTOVERWRITE(scxFILEOverwrite);
static SCX_EXTGETTOTALSIZE(scxFILEGetTotalSize);


static SCX_CONNEXION* SPG_CONV scxFILEOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	if(State.Address.OpenMode==1)//read
	{
		State.Address.dwDesiredAccess=FILE_READ_DATA;
		State.Address.dwShareMode=FILE_SHARE_READ;
		State.Address.dwCreationDisposition=OPEN_EXISTING;
		State.Address.dwFlagsAndAttributes=FILE_FLAG_SEQUENTIAL_SCAN|FILE_ATTRIBUTE_READONLY;
	}
	else if(Address->OpenMode==2)//write
	{
		State.Address.dwDesiredAccess=FILE_WRITE_DATA;
		State.Address.dwShareMode=FILE_SHARE_READ;
		State.Address.dwCreationDisposition=CREATE_ALWAYS;
		State.Address.dwFlagsAndAttributes=FILE_FLAG_SEQUENTIAL_SCAN;
	}

	State.Address.SecurityAttributes.nLength = sizeof( State.Address.SecurityAttributes );
	State.Address.SecurityAttributes.lpSecurityDescriptor = NULL;
	State.Address.SecurityAttributes.bInheritHandle = TRUE;

	DbgCHECK(stricmp(State.Address.FileName,DEFAULTFILENAME)==0,"scxFILEOpen");
	DbgCHECK(State.Address.FileName[0]==0,"scxFILEOpen");

	SPG_ArrayStackAllocZ(char,F,MaxProgDir);
	SPG_ConcatPath(F,Global.ProgDir,State.Address.FileName);
	SPG_ArrayStackCheck(F);

	State.hFile=CreateFile(
		F,
		State.Address.dwDesiredAccess,
		State.Address.dwShareMode,
		&State.Address.SecurityAttributes,
		State.Address.dwCreationDisposition,
		State.Address.dwFlagsAndAttributes,
		0);

	if((State.hFile==0)||(State.hFile==INVALID_HANDLE_VALUE)) 
	{
#ifdef DebugList
		char Msg[512];//Si le chemin est absolu (ex:sur un autre volume) il doit commencer par '\' ou '/'
		int L=sprintf(Msg,"State.Address.FileName=%s\nAddress->OpenMode=%i\nGetLastWinError=",F,State.Address.OpenMode);
		SPG_GetLastWinError(Msg+L,512-L); 
		DbgCHECKTWO(1,"scxFILEOpen:CreateFile failed",Msg);
#endif
		scxFree(C->State); scxFree(C); 
		return 0;
	}

	C->UserFctPtr[sci_EXT_OVERWRITE]=(SCX_USEREXTENSION)scxFILEOverwrite;
	C->UserFctPtr[sci_EXT_GETTOTALSIZE]=(SCX_USEREXTENSION)scxFILEGetTotalSize;

	C->Etat=scxOK;
	return C;
}

// ###################################################

static int SPG_CONV scxFILEClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;

	CloseHandle(State.hFile);

	scxFree(C->State); scxFree(C); 
	return scxOK;
}

// ###################################################

static int SPG_CONV scxFILEWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;

	DWORD WrittenBytes=0;
	WriteFile(State.hFile,Data,DataLen,&WrittenBytes,0);

	return WrittenBytes;
}


static SCX_EXTOVERWRITE(scxFILEOverwrite) //scxFILEOverwrite(void* Data, int DataLen, int Offset, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxOverwrite");
	SCX_STATE& State=*C->State;

	SetFilePointer(State.hFile,Offset,0,FILE_BEGIN);

	DWORD WrittenBytes=0;
	WriteFile(State.hFile,Data,DataLen,&WrittenBytes,0);

	SetFilePointer(State.hFile,0,0,FILE_END);

	return WrittenBytes;
}

// ###################################################

static int SPG_CONV scxFILERead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;

	DWORD ReadBytes=0;
	ReadFile(State.hFile,Data,DataLen,&ReadBytes,0);

	if((ReadBytes==0)&&(State.Address.Loop)) {SetFilePointer(State.hFile,0,0,FILE_BEGIN);}

	return DataLen=ReadBytes;
}

static SCX_EXTGETTOTALSIZE(scxFILEGetTotalSize) //scxFILEOverwrite(void* Data, int DataLen, int Offset, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetSize");
	SCX_STATE& State=*C->State;

	GetFileSize(State.hFile,(unsigned long*)&Size);//même taille en win32 ...

	return 0;
}

// ###################################################

static void SPG_CONV scxFILECfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{//set default values
		Address->OpenMode=1;
		Address->dwDesiredAccess=FILE_READ_DATA;
		Address->dwShareMode=0;
		Address->dwCreationDisposition=OPEN_EXISTING;
		Address->dwFlagsAndAttributes=FILE_FLAG_SEQUENTIAL_SCAN;
		strcpy(Address->FileName,DEFAULTFILENAME);
		Address->Loop=0;
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);
	CFG_StringParam(CFG,"Name",Address->H.Name,0,1);

	CFG_IntParam(CFG,"OpenMode",(int*)&Address->OpenMode,"0:use flags 1:read 2:write",1);
	CFG_IntParam(CFG,"dwDesiredAccess",(int*)&Address->dwDesiredAccess,0,1);
	CFG_IntParam(CFG,"dwShareMode",(int*)&Address->dwShareMode,0,1);
	CFG_IntParam(CFG,"dwCreationDisposition",(int*)&Address->dwCreationDisposition,0,1);
	CFG_IntParam(CFG,"dwFlagsAndAttributes",(int*)&Address->dwFlagsAndAttributes,0,1);
	CFG_StringParam(CFG,"FileName",Address->FileName,0,1);//Si le chemin est absolu (ex:sur un autre volume) il doit commencer par '\' ou '/'
	if(Address->OpenMode!=2) CFG_IntParam(CFG,"LoopRead",(int*)&Address->Loop,0,1);
	return;
}

// ###################################################

static int SPG_CONV scxFILESetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");//variables crées CI,sciParameters,scxParameters
	return 0;
}

// ###################################################

static int SPG_CONV scxFILEGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");//variables crées CI,sciParameters,scxParameters
	return 0;
}

// ###################################################

static int SPG_CONV sciFILEDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}

// ###################################################

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciFILECreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETMERGE);
	strcpy(CI->Name,sci_NAME);

	CI->Description="Read or write binary file";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=0;

	CI->scxOpen=scxFILEOpen;
	CI->scxClose=scxFILEClose;
	CI->scxWrite=scxFILEWrite;
	CI->scxRead=scxFILERead;
	CI->scxCfgAddress=scxFILECfgAddress;
	CI->scxSetParameter=scxFILESetParameter;
	CI->scxGetParameter=scxFILEGetParameter;
	CI->sciDestroyConnexionInterface=sciFILEDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	//sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

// ###################################################

#endif
#endif
