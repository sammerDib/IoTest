
#include "SPG_General.h"

#ifdef SPG_General_USECONNEXION
#ifdef SPG_General_USEBackupMe

#include "SPG_Includes.h"

#include <stdio.h>
#include <string.h>


typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire
	BYTE Private[];
} SCX_ADDRESS;

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;
} SCX_STATE; //parametres d'une connexion en particulier

int SPG_CONV BackupMe(char* decl, SPG_CONFIGFILE& CFG, SCX_CONNEXION* C)
{
	CHECK(CFG.Etat==0,"CFG_FlushToFile",return 0);

	CFG_ParamsFromPtr(CFG);

#define CurPar CFG.CP[i]
	char OutputString[2*MaxProgDir];
	for(int i=1;i<CFG.NumParams;i++)
	{
		SPG_MemCheckStruct(CurPar,"CFG_FlushToFile",CFG.FileName);
		if(CFG_ParamVarFToString(CFG,CurPar,SPG_CFG_FORMAT_TAB|SPG_CFG_FORMAT_COMMENT|SPG_CFG_FORMAT_MINMAX,OutputString,sizeof(OutputString)))
		{
			strcat(OutputString,";\r\n");
			scxWriteStrZ(OutputString,C);
		}

		if((strcmp(CurPar.Name,"W.Address")==0)&&(CurPar.i.Var)) {	BackupMe(CurPar.Name,(SCX_ADDRESS**)(CurPar.i.Var),C);	}
		if((strcmp(CurPar.Name,"W.CI")==0)&&(CurPar.i.Var)) {	BackupMe(CurPar.Name,(SCI_CONNEXIONINTERFACE**)(CurPar.i.Var),C);	}
		if((strcmp(CurPar.Name,"R.Address")==0)&&(CurPar.i.Var)) {	BackupMe(CurPar.Name,(SCX_ADDRESS**)(CurPar.i.Var),C);	}
		if((strcmp(CurPar.Name,"R.CI")==0)&&(CurPar.i.Var)) {	BackupMe(CurPar.Name,(SCI_CONNEXIONINTERFACE**)(CurPar.i.Var),C);	}
	}
	return -1;
#undef CurPar
}

#pragma warning(disable:4130)

int SPG_CONV BackupMe(char* decl, SCX_CONNEXION* p, SCX_CONNEXION* C)
{
	if(p==0) {BMW(sprintf(S,"%s = 0;",decl)); return -1;}

	BMW(sprintf(S,"%s = {",decl));

	SBM((*p),SCX_ADDRESS*,Address,		C);

	scxWriteStrZ("}\r\n",C);

	return -1;
}

int SPG_CONV BackupMe(char* decl, SCX_ADDRESS** p, SCX_CONNEXION* C)
{
	if(p==0) {BMW(sprintf(S,"%s = 0;",decl)); return -1;}
	if(*p==0) {BMW(sprintf(S,"(*%s) = 0;",decl)); return -1;}
#ifdef DebugMem
	CHECK( !SPG_MemIsValid((*p),0), "BackupMe", BMW(sprintf(S,"(*%s) = (invalid ptr);",decl));return -1);
#endif
	return BackupMe(decl,*p,C);
}

int SPG_CONV BackupMe(char* decl, SCX_ADDRESS* p, SCX_CONNEXION* C)
{
	if(p==0) {BMW(sprintf(S,"%s = 0",decl)); return -1;}
	//CHECK( !SPG_MemIsValid(p,0), "BackupMe", BMW(sprintf(S,"%s = (invalid ptr)",decl));return -1);

	BMW(sprintf(S,"%s = {",decl));

	SPG_CONFIGFILE CFG; SPG_ZeroStruct(CFG);
	SCI_CONNEXIONINTERFACE* pCI=sciInterfaceFromUID(Global.SCI,p->H.TypeUID);
	scxCfgAddress(CFG,p,pCI,scxCFGREADONLY);
	VBM(SCX_ADDRESS, CFG, C);
	CFG_Close(CFG,0);

	scxWriteStrZ("}\r\n",C);

	return -1;
}

int SPG_CONV BackupMe(char* decl, SCX_ETAT &p, SCX_CONNEXION* C)
{
	BMW(sprintf(S,"%s\t=\t%i",decl,(int)p));
	return -1;
}

int SPG_CONV BackupMe(char* decl, SCI_TYPE &p, SCX_CONNEXION* C)
{
	BMW(sprintf(S,"%s\t=\t%i",decl,(int)p));
	return -1;
}

int SPG_CONV BackupMe(char* decl, SCI_CONNEXIONINTERFACE** p, SCX_CONNEXION* C)
{
	if(p==0) {BMW(sprintf(S,"%s = 0;",decl)); return -1;}
	if(*p==0) {BMW(sprintf(S,"(*%s) = 0;",decl)); return -1;}
#ifdef DebugMem
	CHECK( !SPG_MemIsValid((*p),0), "BackupMe", BMW(sprintf(S,"(*%s) = (invalid ptr);",decl));return -1);
#endif
	return BackupMe(decl,*p,C);
}

int SPG_CONV BackupMe(char* decl, SCI_CONNEXIONINTERFACE* p, SCX_CONNEXION* C)
{
	if(p==0) {BMW(sprintf(S,"%s = 0",decl)); return -1;}

	BMW(sprintf(S,"%s = {",decl));

	SBM((*p),	SCX_ETAT, Etat,		C);
	SBM((*p),	SCI_TYPE, Type,		C);
	SBM((*p),	int, sizeofAddress,		C);
	SBM((*p),	int, TypeUID,		C);
	ABM((*p),	char, Name,SCI_CONNEXION_NAME,		C);
	SBM((*p),	char*, Description,		C);
	SBM((*p),	int, maxPacketSize,		C);
	PTRBM((*p),	SCI_PARAMETERS*, sciParameters,		C);//internal state

	PTRBM((*p),	SCX_OPEN,       scxOpen,		C);
	PTRBM((*p),	SCX_CLOSE,     scxClose,		C);
	PTRBM((*p),	SCX_WRITE,     scxWrite,		C);
	PTRBM((*p),	SCX_READ,       scxRead,		C);
	PTRBM((*p),	SCX_CFGADDRESS,   scxCfgAddress,		C);
	PTRBM((*p),	SCX_SETPARAMETER, scxSetParameter,		C);
	PTRBM((*p),	SCX_GETPARAMETER, scxGetParameter,		C);
	PTRBM((*p),	SCI_DESTROY, sciDestroyConnexionInterface,		C);

	scxWriteStrZ("}\r\n",C);

	return -1;
}


#endif
#endif
