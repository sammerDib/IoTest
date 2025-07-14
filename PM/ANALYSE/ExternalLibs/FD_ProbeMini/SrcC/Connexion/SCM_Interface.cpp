
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#include "..\SPG_Includes.h"

#include "..\SPG_SysInc.h"

#include <memory.h>
#include <string.h>

#include "SCM_Connexion_Void.h"
#include "SCM_Connexion_Buffer.h"
#include "SCM_Connexion_Merge.h"
#include "SCM_Connexion_AsciiHEX.h"
#include "SCM_Connexion_BandPassRS.h"
//#include "SCM_Connexion_Transcode.h"
#include "SCM_Connexion_SCHK.h"
#include "SCM_Connexion_Packet.h"
//#include "SCM_Connexion_PacketRIA.h"
//#include "SCM_Connexion_Double.h"
//#include "SCM_Connexion_Chain.h"
#include "SCM_Connexion_File.h"
#include "SCM_Connexion_SplitFilename.h"
#include "SCM_Connexion_RS232.h"
#include "SCM_Connexion_FTD2XX.h"
#include "SCM_Connexion_UDP.h"
#include "SCM_Connexion_NIDAQmx.h"
#include "SCM_Connexion_CyUSB.h"
#include "SCM_Connexion_EmuleLISEED.h"
#include "SCM_Connexion_EmuleLISEEDI.h"
#include "SCM_Connexion_CameraEmulee.h"
#include "SCM_Connexion_CameraPad.h"
#include "SCM_Connexion_CamMulti.h"
//manque bufferized I/O Chain

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;
	BYTE Private[];
} SCX_ADDRESS;

//rechercher alloc, free, et typecast vers char*, char *, void et les éliminer

int SPG_CONV sciCreateInterfaces(SCM_INTERFACES& SCI)
{
	SPG_ZeroStruct(SCI);
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciVOIDCreateConnexionInterface();		if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#ifdef SPG_General_USERingBuffer
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciBUFFCreateConnexionInterface();		if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#endif
#ifdef SPG_General_USERingBuffer
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciMERGECreateConnexionInterface();	if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#endif
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciAHEXCreateConnexionInterface();		if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
//fdef SPG_General_USEProfil
//	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciBPRSCreateConnexionInterface();		if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
//ndif

//	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciTRANSCODECreateConnexionInterface();if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#ifdef SPG_General_USERingBuffer
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciSCHKCreateConnexionInterface();		if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#endif
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciPACKETCreateConnexionInterface();	if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
//	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciPACKETRIACreateConnexionInterface();if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciFILECreateConnexionInterface();		if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciSPLITFCreateConnexionInterface();		if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciRS232CreateConnexionInterface();	if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#ifdef SPG_General_USEFTD2XX
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciFTD2XXCreateConnexionInterface();		if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#endif
#ifdef SPG_General_USESCXUDP
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciUDPCreateConnexionInterface();		if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#endif
#ifdef SPG_General_USENIDAQmx
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciNIDAQCreateConnexionInterface();	if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#endif
#ifdef SPG_General_USECyUSB
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciCyUSBCreateConnexionInterface();	if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#endif
#ifdef SPG_General_USEELISED
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciELISEEDCreateConnexionInterface();	if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#endif
#ifdef SPG_General_USEELISEDI
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciELISEEDICreateConnexionInterface();	if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#endif
#ifdef SPG_General_USECAMERAEMULEE
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciCAMEMULEECreateConnexionInterface();	if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#endif
#ifdef SPG_General_USECAMERAPAD
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciCAMPADCreateConnexionInterface();	if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#endif
#ifdef SPG_General_USECAMMULTI
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciCAMMULTICreateConnexionInterface();	if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
#endif
/*
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciBUFFCreateConnexionInterface();		if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciDBLCreateConnexionInterface();	 	if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
	SCI.Interfaces[SCI.Count].Type=SCM_CPP; SCI.Interfaces[SCI.Count].CI=sciCHAINCreateConnexionInterface(); 	if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
*/
	return SCI.Count;
}

int SPG_CONV sciDestroyInterfaces(SCM_INTERFACES& SCI)
{
	int i;
	for(i=0;i<SCI.Count;i++)
	{
		CHECK(SCI.Interfaces[i].CI==0,"sciDestroyInterfaces",continue);
		for(int j=0;j<i;j++)
		{
			DbgCHECKV(SCI.Interfaces[i].CI->TypeUID==SCI.Interfaces[j].CI->TypeUID,"sciDestroyInterfaces: Found duplicate sci_UID",SCI.Interfaces[i].CI->TypeUID);
			DbgCHECKTWO(strcmp(SCI.Interfaces[i].CI->Name,SCI.Interfaces[j].CI->Name)==0,"sciDestroyInterfaces: Found duplicate name",SCI.Interfaces[i].CI->Name);
		}
	}
	for(i=0;i<SCI.Count;i++)
	{
		sciDestroyConnexionInterface(SCI.Interfaces[i].CI);
#ifdef SPG_General_USEWindows
		if(SCI.Interfaces[i].Type==SCM_DLL)
		{
			FreeLibrary((HINSTANCE)SCI.Interfaces[i].DLLInterface.hModule);
		}
		SPG_ZeroStruct(SCI.Interfaces[i]);
#endif
	}
	return 0;
}

#ifdef SPG_General_USEWindows
int SPG_CONV sciInterfacesAddDll(SCM_INTERFACES& SCI, char* DllName)
{
	CHECK(DllName==0,"sciInterfacesAddDll",return 0);
	CHECK(DllName[0]==0,"sciInterfacesAddDll",return 0);
	SCI.Interfaces[SCI.Count].Type=SCM_DLL; 

	SCI.Interfaces[SCI.Count].DLLInterface.hModule=(int)LoadLibrary(DllName);
	CHECKTWO(SCI.Interfaces[SCI.Count].DLLInterface.hModule==0,"LoadLibrary failed",DllName,return 0);

	SCI_CREATE sciCreateConnexionInterface=(SCI_CREATE)GetProcAddress((HMODULE)SCI.Interfaces[SCI.Count].DLLInterface.hModule,"sciCreateConnexionInterface");
	CHECKTWO(sciCreateConnexionInterface==0,"GetProcAddress failed",DllName,FreeLibrary((HMODULE)SCI.Interfaces[SCI.Count].DLLInterface.hModule);return 0);

	SCI.Interfaces[SCI.Count].CI=sciCreateConnexionInterface();

	if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
	return SCI.Count;
}
#endif

int SPG_CONV sciInterfacesAddPtr(SCM_INTERFACES& SCI, SCI_CREATE sciCreateConnexionInterface)
{
	CHECK(sciCreateConnexionInterface==0,"sciInterfacesAddPtr",return 0);
	SCI.Interfaces[SCI.Count].Type=SCM_PTR; 
	SCI.Interfaces[SCI.Count].CI=sciCreateConnexionInterface();
	if(SCI.Interfaces[SCI.Count].CI) SCI.Count++;
	return SCI.Count;
}

SCI_CONNEXIONINTERFACE* SPG_CONV sciInterfaceFromName(SCM_INTERFACES& SCI, char Name[SCI_CONNEXION_NAME])
{
	CHECK(Name==0,"sciInterfaceFromName",return 0);
	CHECK(Name[0]==0,"sciInterfaceFromName",return 0);
	for(int i=0;i<SCI.Count;i++)
	{
		if(strncmp(SCI.Interfaces[i].CI->Name,Name,SCI_CONNEXION_NAME)==0)
		{
			return SCI.Interfaces[i].CI;
		}
	}
	DbgCHECKTWO(1,"sciInterfaceFromName: no connexion interface for:",Name);
	return 0;
}

SCI_CONNEXIONINTERFACE* SPG_CONV sciInterfaceFromUID(SCM_INTERFACES& SCI, int TypeUID)
{
	for(int i=0;i<SCI.Count;i++)
	{
		if(SCI.Interfaces[i].CI->TypeUID==TypeUID)
		{
			return SCI.Interfaces[i].CI;
		}
	}
	DbgCHECKV(1,"sciInterfaceFromName: no connexion interface for:",TypeUID);
	return 0;
}

SCX_ADDRESS* SPG_CONV sciAddressFromName(SCM_INTERFACES& SCI, SPG_CONFIGFILE& CFG, char Name[SCI_CONNEXION_NAME], SCI_CONNEXIONINTERFACE** pCI)
{//CFG peut préexister s'il contient des parametres d'un fichier à copier dans l'addresse
	SCI_CONNEXIONINTERFACE* CI=sciInterfaceFromName(SCI,Name);
	if(pCI) *pCI=CI;
	CHECKTWO(CI==0,"sciAddressFromName: Interface not found",Name,return 0);
	return scxCreateCfgAddress(CI,CFG);
}

#ifdef SPG_General_USECONFIGFILEDLG
SCX_ADDRESS* SPG_CONV sciAddressFromDlg(SCM_INTERFACES& SCI, SCI_CONNEXIONINTERFACE** pCI)
{
	char* Name[SCI_CONNEXION_NAME];
	char* Description[SCI_CONNEXION_NAME];

	if(pCI) *pCI=0;

	for(int i=0;i<SCI.Count;i++)
	{
		Name[i]=0;
		Description[i]=0;
		if((SCI.Interfaces[i].CI->Type&sciNODLG)==0)
		{
			Name[i]=SCI.Interfaces[i].CI->Name;
			Description[i]=SCI.Interfaces[i].CI->Description;
		}
	}

	int Choix;
	while(Choix=CFG_CreateChooseDialog("Connexion interface",Name,Description,SCI.Count),V_IsBound(Choix,0,SCI.Count))
	{
		SPG_CONFIGFILE CFG;
		SPG_ZeroStruct(CFG);
		SCX_ADDRESS* Address=scxCreateCfgAddress(SCI.Interfaces[Choix].CI,CFG);
		if(Address)
		{
			int DlgID=CFG_CreateDialog(CFG,CFG_scxDlgFlag);
			CFG_Close(CFG);
			if(DlgID==IDOK) 
			{
				if(pCI) *pCI=SCI.Interfaces[Choix].CI;
				return Address;
			}
			scxDestroyAddress(Address);
		}
	}
	return 0;
}
#endif

SCX_ADDRESS* SPG_CONV sciAddressFromFile(SCM_INTERFACES& SCI, char* Path, char* FileName, SCI_CONNEXIONINTERFACE** pCI)
{
	SPG_CONFIGFILE CFG;
	SPG_ZeroStruct(CFG);
	CHECK(FileName==0,"sciAddressFromFile",return 0);
	CHECK(FileName[0]==0,"sciAddressFromFile",return 0);
	CHECKTWO(CFG_Init(CFG,Path,FileName,128,1)==0,"sciAddressFromFile: can not open file:",FileName,return 0);

	SCX_ADDRESS* Address=sciAddressFromCFG(SCI,Path,CFG,pCI);
	SPG_MemCatName(Address,CFG.FileName);
	//sciAddressFromCFG fait que CFG contient des liens vers Address
	//il faut faire close de CFG avant de faire close de Address ou appeler sciDestroyAddressFromCFG
	CFG_Close(CFG);
	return Address;
}

SCX_ADDRESS* SPG_CONV sciAddressDuplicate(SCM_INTERFACES& SCI, SCX_ADDRESS* AddressTemplate, SCI_CONNEXIONINTERFACE** pCI)
{
	CHECK(AddressTemplate==0,"sciAddressDuplicate",return 0)
	SCI_CONNEXIONINTERFACE* localCI; if(pCI==0) pCI=&localCI;
	*pCI=sciInterfaceFromName(SCI,AddressTemplate->H.Name);
	SCX_ADDRESS* D=scxCreateAddress(*pCI);
	scxAddressCopy(D,AddressTemplate);
	if((*pCI)->Type&sciPROTOCOL)
	{
		SCI_CONNEXIONINTERFACE *pRCI=0, *pWCI=0;
		SCX_ADDRESS *TemplateRA=0, *TemplateWA=0;
		sciGetProtocolAddress(AddressTemplate,*pCI,&TemplateRA,&pRCI,&TemplateWA,&pWCI);
		//DbgCHECKTWO(TemplateRA==0,"sciAddressDuplicate",AddressTemplate->H.Name);
		//DbgCHECKTWO(TemplateWA==0,"sciAddressDuplicate",AddressTemplate->H.Name);
		SCX_ADDRESS* RA=TemplateRA?sciAddressDuplicate(SCI,TemplateRA,0):0;
		SCX_ADDRESS* WA=RA;
		if(TemplateRA!=TemplateWA)
		{
			WA=TemplateWA?sciAddressDuplicate(SCI,TemplateWA,0):0;
		}
		sciSetProtocolAddress(D,*pCI,RA,pRCI,WA,pWCI);
	}
	return D;
}

SCX_ADDRESS* SPG_CONV sciAddressDuplicate(SCI_CONNEXIONINTERFACE* CITemplate, SCX_ADDRESS* AddressTemplate, SCI_CONNEXIONINTERFACE** pCI)
{
	CHECK(AddressTemplate==0,"sciAddressDuplicate",return 0)
	SCI_CONNEXIONINTERFACE* localCI; if(pCI==0) pCI=&localCI;
	*pCI=CITemplate;//sciInterfaceFromName(SCI,AddressTemplate->H.Name);
	SCX_ADDRESS* D=scxCreateAddress(*pCI);
	scxAddressCopy(D,AddressTemplate);
	if((*pCI)->Type&sciPROTOCOL)
	{
		SCI_CONNEXIONINTERFACE *pRCI=0, *pWCI=0;
		SCX_ADDRESS *TemplateRA=0, *TemplateWA=0;
		sciGetProtocolAddress(AddressTemplate,*pCI,&TemplateRA,&pRCI,&TemplateWA,&pWCI);
		//DbgCHECKTWO(TemplateRA==0,"sciAddressDuplicate",AddressTemplate->H.Name);
		//DbgCHECKTWO(TemplateWA==0,"sciAddressDuplicate",AddressTemplate->H.Name);
		SCX_ADDRESS* RA=TemplateRA?sciAddressDuplicate(pRCI,TemplateRA,0):0;
		SCX_ADDRESS* WA=RA;
		if(TemplateRA!=TemplateWA)
		{
			WA=TemplateWA?sciAddressDuplicate(pWCI,TemplateWA,0):0;
		}
		sciSetProtocolAddress(D,*pCI,RA,pRCI,WA,pWCI);
	}
	return D;
}

void SPG_CONV sciDestroyAddress(SCX_ADDRESS* &Address, SCI_CONNEXIONINTERFACE* CI)
{//pour les address PROTOCOL n'ayant jamais servi à faire scxOpen
	CHECK(Address==0,"sciDestroyAddress",return);
	CHECK(CI==0,"sciDestroyAddress",return);
#ifdef DebugMem
	DbgCHECK(SPG_MemIsExactBlock(Address,Address->H.sizeofAddress)==-1,"sciDestroyAddress: Invalid size or memory block");
#endif
	if(CI->Type&sciPROTOCOL)
	{
		SCI_CONNEXIONINTERFACE *pRCI=0, *pWCI=0;
		SCX_ADDRESS *ReadAddress=0, *WriteAddress=0;
		sciGetProtocolAddress(Address,CI,&ReadAddress,&pRCI,&WriteAddress,&pWCI);
		//DbgCHECKTWO(ReadAddress==0,"sciDestroyAddress",Address->H.Name);
		//DbgCHECKTWO(WriteAddress==0,"sciDestroyAddress",Address->H.Name);
		if(pRCI!=pWCI)
		{
			if(ReadAddress) sciDestroyAddress(ReadAddress,pRCI);//la fonction gerant les branches du protocole
			if(WriteAddress) sciDestroyAddress(WriteAddress,pWCI);
		}
		else
		{
			if(ReadAddress) sciDestroyAddress(ReadAddress,pRCI);//la fonction gerant les branches du protocole
			//pWCI=0;
		}
	}
	//scxDestroyAddress(Address); //la fonction private
#ifdef DebugMem
	DbgCHECKTWO(SPG_MemIsExactBlock(Address,Address->H.sizeofAddress)==0,"sciDestroyAddress: Invalid size or memory block",CI->Name);
	memset(Address,0,Address->H.sizeofAddress);
#endif
	scxFree(Address);
	return;
}

void SPG_CONV sciDestroyAddress(SCM_INTERFACES& SCI, SCX_ADDRESS* &Address)
{//pour les address PROTOCOL n'ayant jamais servi à faire scxOpen
	CHECK(Address==0,"sciDestroyAddress",return)
#ifdef DebugMem
	DbgCHECK(SPG_MemIsExactBlock(Address,Address->H.sizeofAddress)==0,"sciDestroyAddress: Invalid size or memory block");
#endif
	SCI_CONNEXIONINTERFACE* CI=sciInterfaceFromName(SCI,Address->H.Name);
	sciDestroyAddress(Address,CI);
	return;
}

void SPG_CONV sciSetProtocolAddress(SCX_ADDRESS* Address, SCI_CONNEXIONINTERFACE* pCI, SCX_ADDRESS* ReadAddress, SCI_CONNEXIONINTERFACE* pRCI, SCX_ADDRESS* WriteAddress, SCI_CONNEXIONINTERFACE* pWCI)
{
	SPG_CONFIGFILE ProtocolCFG;//remplis les champs de la connexion Protocol avec les pointeurs vers les sous connexions ReadConnexion et WriteConnexion
	SPG_ZeroStruct(ProtocolCFG);
	scxCfgAddress(ProtocolCFG,Address,pCI,scxCFGREADONLY);//cette opération n'écrit rien en fichier
	CHECK(CFG_SetIntParam(ProtocolCFG,"R.Address",(int)ReadAddress)==0,"sciSetProtocolAddress",;);
	CHECK(CFG_SetIntParam(ProtocolCFG,"R.CI",(int)pRCI)==0,"sciSetProtocolAddress",;);
	CHECK(CFG_SetIntParam(ProtocolCFG,"W.Address",(int)WriteAddress)==0,"sciSetProtocolAddress",;);
	CHECK(CFG_SetIntParam(ProtocolCFG,"W.CI",(int)pWCI)==0,"",);
	CFG_Close(ProtocolCFG,0);//cette opération n'écrit rien en fichier
	return;
}

void SPG_CONV sciSetProtocolAddress(SCX_ADDRESS* Address, SCI_CONNEXIONINTERFACE* pCI, SCX_ADDRESS* RWAddress, SCI_CONNEXIONINTERFACE* pRWCI)
{
	sciSetProtocolAddress(Address,pCI,RWAddress,pRWCI,RWAddress,pRWCI);
	return;
}

void SPG_CONV sciGetProtocolAddress(SCX_ADDRESS* Address, SCI_CONNEXIONINTERFACE* pCI, SCX_ADDRESS** pReadAddress, SCI_CONNEXIONINTERFACE** pRCI, SCX_ADDRESS** pWriteAddress, SCI_CONNEXIONINTERFACE** pWCI)
{
	SPG_CONFIGFILE ProtocolCFG;//remplis les champs de la connexion Protocol avec les pointeurs vers les sous connexions ReadConnexion et WriteConnexion
	SPG_ZeroStruct(ProtocolCFG);
	scxCfgAddress(ProtocolCFG,Address,pCI,scxCFGREADONLY);//cette opération n'écrit rien en fichier
	if(pReadAddress) *(int*)pReadAddress=CFG_GetIntParam(ProtocolCFG,"R.Address",0);
	if(pRCI) *(int*)pRCI=CFG_GetIntParam(ProtocolCFG,"R.CI",0);
	if(pWriteAddress) *(int*)pWriteAddress=CFG_GetIntParam(ProtocolCFG,"W.Address",0);
	if(pWCI) *(int*)pWCI=CFG_GetIntParam(ProtocolCFG,"W.CI",0);
	CFG_Close(ProtocolCFG,0);//cette opération n'écrit rien en fichier
	return;
}

SCX_ADDRESS* SPG_CONV sciAddressFromCFG(SCM_INTERFACES& SCI, char* Path, SPG_CONFIGFILE& CFG, SCI_CONNEXIONINTERFACE** pCI)
{
	int iName=0;
	CHECKTWO((iName=CFG_ParamFromName(CFG,"Name"))==0,"sciAddressFromCFG can not find Name parameter",CFG.FileName,return 0); //CFG_SetStringParam(CFG,"Name","VOID");

	if(   (strncmp(CFG.CP[iName].s.S_F,"REDIR",V_Min(SPG_CONFIGCOMMENT,SCI_CONNEXION_NAME))==0)
		||(strncmp(CFG.CP[iName].s.S_F,"REDIRECT",V_Min(SPG_CONFIGCOMMENT,SCI_CONNEXION_NAME))==0) 
		||(strncmp(CFG.CP[iName].s.S_F,"REDIRECTION",V_Min(SPG_CONFIGCOMMENT,SCI_CONNEXION_NAME))==0) )
	{//redirection
		int iFile=0;
		CHECKTWO((iFile=CFG_ParamFromName(CFG,"File"))==0,"sciAddressFromCFG can not find File parameter",CFG.FileName,return 0);
		SCX_ADDRESS* Address=sciAddressFromFile(SCI,Path,CFG.CP[iFile].s.S_F,pCI);
		DbgCHECKTWO(Address==0,"sciAddressFromCFG:REDIR:sciAddressFromFile failed",CFG.FileName);
		return Address;
	}//FIN CONNEXION REDIR

	if(   (strncmp(CFG.CP[iName].s.S_F,"DLG",V_Min(SPG_CONFIGCOMMENT,SCI_CONNEXION_NAME))==0)
		||(strncmp(CFG.CP[iName].s.S_F,"DLGBOX",V_Min(SPG_CONFIGCOMMENT,SCI_CONNEXION_NAME))==0) 
		||(strncmp(CFG.CP[iName].s.S_F,"DIALOG",V_Min(SPG_CONFIGCOMMENT,SCI_CONNEXION_NAME))==0) 
		||(strncmp(CFG.CP[iName].s.S_F,"DIALOGBOX",V_Min(SPG_CONFIGCOMMENT,SCI_CONNEXION_NAME))==0) )
	{//redirection
#ifndef SPG_General_USECONFIGFILEDLG
		SPG_List2S("sciAddressFromCFG:DLG not enabled",CFG.FileName); return 0;
#else
		SCX_ADDRESS* Address=sciAddressFromDlg(SCI,pCI);
		DbgCHECKTWO(Address==0,"sciAddressFromCFG:DLG:sciAddressFromFile failed",CFG.FileName);
		return Address;
#endif
	}//FIN CONNEXION DLGBOX

	if(   (strncmp(CFG.CP[iName].s.S_F,"PROTOCOL",V_Min(SPG_CONFIGCOMMENT,SCI_CONNEXION_NAME))==0))
	{//packet avec deux sous-connexions
		int iProtocol=0;
		int iReadConnexion=0;
		int iWriteConnexion=0;
		CHECKTWO((iProtocol=CFG_ParamFromName(CFG,"Protocol"))==0,"sciAddressFromCFG can not find Protocol parameter",CFG.FileName,return 0);
		CHECKTWO((iReadConnexion=CFG_ParamFromName(CFG,"ReadConnexion"))==0,"sciAddressFromCFG can not find ReadConnexion parameter",CFG.FileName,return 0);
		CHECKTWO((iWriteConnexion=CFG_ParamFromName(CFG,"WriteConnexion"))==0,"sciAddressFromCFG can not find WriteConnexion parameter",CFG.FileName,return 0);
		
		SCI_CONNEXIONINTERFACE* pRCI;					SCI_CONNEXIONINTERFACE* pWCI;
		SCX_ADDRESS* ReadAddress=sciAddressFromFile(SCI,Path,CFG.CP[iReadConnexion].s.S_F,&pRCI);		
														SCX_ADDRESS* WriteAddress=sciAddressFromFile(SCI,Path,CFG.CP[iWriteConnexion].s.S_F,&pWCI);
		SPG_MemCatName(ReadAddress,"(sciAddCFG:R)");	SPG_MemCatName(WriteAddress,"(sciAddrCFG:W)");
		CHECKTWO(ReadAddress==0,"sciAddressFromCFG can not open ReadAddress",CFG.FileName,if(WriteAddress) scxDestroyAddress(WriteAddress);return 0);		
														CHECKTWO(WriteAddress==0,"sciAddressFromCFG can not open WriteAddress",CFG.FileName,if(ReadAddress) scxDestroyAddress(ReadAddress);return 0);

		SCI_CONNEXIONINTERFACE* localCI; if(pCI==0) pCI=&localCI;
		SCX_ADDRESS* Address=sciAddressFromFile(SCI,Path,CFG.CP[iProtocol].s.S_F,pCI);
		CHECKTWO(Address==0,"sciAddressFromCFG:PROTOCOL:sciAddressFromFile failed",CFG.FileName,scxDestroyAddress(ReadAddress);scxDestroyAddress(WriteAddress););
		sciSetProtocolAddress(Address,*pCI,ReadAddress,pRCI,WriteAddress,pWCI);
		return Address;
	}//FIN CONNEXION PACKET AVEC DEUX SOUS-CONNEXIONS

	//Initialisation d'une connexion simple

	SCX_ADDRESS* Address=sciAddressFromName(SCI,CFG,CFG.CP[iName].s.S_F,pCI);
	return Address;//Attention ne pas faire de destroy de Address avant le destroy de CFG; ou utiliser sciDestroyAddressFromCFG dans ce cas
}

/*
void SPG_CONV sciDestroyAddressFromCFG(SCX_ADDRESS* &Address, SPG_CONFIGFILE& CFG)
{//permet de detruire une adresse en s'assurant que les références enregistrées dans CFG sont aussi éliminées
	CFG_ParamsFromPtr(CFG);
	CFG_RemoveRef(CFG,Address,Address->H.sizeofAddress);
	sciDestroyAddress(Address);
	return;
}
*/

/*
//printable 33 to 126 inclus
//chaine 48-57, 65-90, 97-122
char SPG_BINtoASCII(int V)
{
	CHECK(!V_IsBound(V,0,XXX),"SPG_BINtoASCII",return 0);

	return;
}

typedef struct
{
	char IndexStart;
	char IndexStop;
	char Min;
	char Max;
} ASCII_Interval;

typedef struct
{
	ASCII_Interval I[16];
	int TotalCharCount;
	int NumInterval;
} ASCII_Intervals;

SPG_CONV int SPG_ASCII_Intervals(ASCII_Intervals& AIS, int Flag)
{
	SGP_ZeroStruct(AIS);
	if(Flag&ASCII_Printable)
	{
		AIS.I[AI.NumInterval].IndexStart=AIS.TotalCharCount;
		AIS.I[AI.NumInterval].Min=33;
		AIS.I[AI.NumInterval].Max=126;
		AIS.TotalCharCount=1+AIS.I[AIS.NumInterval].Max-AIS.I[AIS.NumInterval].Min;
		AIS.I[AI.NumInterval].IndexStop=AIS.TotalCharCount-1;
		AIS.NumInterval++;
	}
	else if(Flag&ASCII_String)
	{
		AIS.I[AI.NumInterval].Index=AIS.TotalCharCount;
		AIS.I[AIS.NumInterval].Min=48;
		AIS.I[AIS.NumInterval].Max=57;
		AIS.TotalCharCount=1+AIS.I[AIS.NumInterval].Max-AIS.I[AIS.NumInterval].Min;
		AIS.I[AI.NumInterval].IndexStop=AIS.TotalCharCount-1;
		AIS.NumInterval++;
		AIS.I[AI.NumInterval].Index=AIS.TotalCharCount;
		AIS.I[AIS.NumInterval].Min=65;
		AIS.I[AIS.NumInterval].Max=90;
		AIS.TotalCharCount=1+AIS.I[AIS.NumInterval].Max-AIS.I[AIS.NumInterval].Min;
		AIS.I[AI.NumInterval].IndexStop=AIS.TotalCharCount-1;
		AIS.NumInterval++;
		AIS.I[AI.NumInterval].Index=AIS.TotalCharCount;
		AIS.I[AIS.NumInterval].Min=97;
		AIS.I[AIS.NumInterval].Max=122;
		AIS.TotalCharCount=1+AIS.I[AIS.NumInterval].Max-AIS.I[AIS.NumInterval].Min;
		AIS.I[AI.NumInterval].IndexStop=AIS.TotalCharCount-1;
		AIS.NumInterval++;
	}
	return;
}

SPG_CONV char SPG_BINtoASCII(ASCII_Intervals& AIS, BYTE B)
{
	for(int i=0;i<AIS.NumInterval;i++)
	{
		if(V_InclusiveBound(B,AIS.I[i].IndexStart,AIS.I[i].IndexStop))
		{
			return  B-AIS.I[i].IndexStart+AIS.I[AIS.NumInterval].Min;
		}
	}
	DbgCHECK(true,SPG_BINtoASCII);
	return 0;
}

SPG_CONV BYTE SPG_ASCIItoBIN(ASCII_Intervals& AIS, char C)
{
	for(int i=0;i<AIS.NumInterval;i++)
	{
		if(V_InclusiveBound(C,AIS.I[i].Min,AIS.I[i].Max))
		{
			return  C-AIS.I[i].Min+AIS.I[AIS.NumInterval].IndexStart;
		}
	}
	DbgCHECK(true,SPG_BINtoASCII);
	return 0;
}

SPG_CONV int SPG_BINtoASCIIarray(char* String, const void* Data, int sizeofData, int Flag)//
{
	ASCII_Intervals AIS;
	SPG_ASCII_Intervals(AIS,Flag);
	CHECK(AIS.TotalCharCount==0,"SPG_BINtoASCII",return 0);
	for(int i=0;i<sizeofData;i++)
	{
	}
	return -1;
}

SPG_CONV int SPG_ASCIItoBINarray(void* Data, int& sizeofData, const char* String)
{
	return -1;
}
*/

void SPG_CONV sciDataToHex(void* Data, int DataLen, char* Msg, int MsgLen)
{
	//nombre max d'items affichable dans la chaine moins l'insersion 6 et le zero terminal 1
	int NMax=(MsgLen-6-1)/3;//3 caractere par octet de data

	int SkipStart=DataLen;
	int SkipStop=DataLen;
	if(NMax<DataLen)
	{//il faut skipper une partie
		int SkipLength=DataLen-NMax+1;
		SkipStart=(DataLen-SkipLength)/2;//SkipStart est pas dessiné
		SkipStop=SkipStart+SkipLength;//SkipStop ne l'est pas
	}

	int c=0;
	for(int i=0;i<DataLen;i++)
	{

		BYTE b=((BYTE*)Data)[i];

		//3 caractere par octet de data
		Msg[c++]=sci16ToHEX(b>>4);
		Msg[c++]=sci16ToHEX(b);
		Msg[c++]=' ';

		if(i==SkipStart) 
		{//insert 6 octets
			Msg[c++]='<';
			Msg[c++]='.';
			Msg[c++]='.';
			Msg[c++]='.';
			Msg[c++]='>';
			Msg[c++]=' ';
			i=SkipStop;
		}

		CHECK(c>MsgLen-1,"sciDataToHex",Msg[MsgLen-1]=0;return);
	}
	Msg[c]=0;//insert le zero terminal
	return;
}

void SPG_CONV sciDataToString(void* Data, int DataLen, char* Msg, int MsgLen)
{
	//nombre max d'items affichable dans la chaine moins l'insersion 6 et le zero terminal 1
	int NMax=(MsgLen-6-1);//1 caractere par octet de data

	int SkipStart=DataLen;
	int SkipStop=DataLen;
	if(NMax<DataLen)
	{//il faut skipper une partie
		int SkipLength=DataLen-NMax+1;
		SkipStart=(DataLen-SkipLength)/2;//SkipStart est pas dessiné
		SkipStop=SkipStart+SkipLength;//SkipStop ne l'est pas
	}

	int c=0;
	for(int i=0;i<DataLen;i++)
	{

		BYTE b=((BYTE*)Data)[i];
		if(((b>='a')&&(b<='z'))||((b>='A')&&(b<='Z'))||((b>='0')&&(b<='9')))
		{
			Msg[c++]=b;
		}
		else
		{
			Msg[c++]='.';
		}
		if(i==SkipStart) 
		{
			Msg[c++]='<';
			Msg[c++]='.';
			Msg[c++]='.';
			Msg[c++]='.';
			Msg[c++]='>';
			Msg[c++]=' ';
			i=SkipStop;
		}
		CHECK(c>MsgLen-1,"sciDataToString",Msg[c-1]=0;return);
	}
	Msg[c]=0;
	return;
}
void SPG_CONV sciDataPtrToHEX(void* Data, char* Msg)
{
	int c=0;
	Msg[c++]='[';
	//int Ptr=(int)Data;
	for(int i=0;i<4;i++)
	{
		Msg[c++]=sci16ToHEX(((int)Data)>>(4*i));
	}
	Msg[c++]=']';
	return;
}


SCX_CONNEXION* SPG_CONV sciOpen(SCM_INTERFACES& SCI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	CHECK(Address==0,"sciOpen",return 0);
	SCX_ADDRESSHEADER& H=*(SCX_ADDRESSHEADER*)Address;
#ifdef SCX_ADDRESS_HASNAME
	CHECK(H.Name[0]==0,"sciOpen: Address not initialized",return 0);
#endif
#ifdef SCX_ADDRESS_HASsizeofAddress
	CHECKTWO(H.sizeofAddress==0,"sciOpen: Address not initialized",H.Name,return 0);
#endif
#ifdef SCX_ADDRESS_HASUID
	CHECKTWO(H.TypeUID==0,"sciOpen: Address not initialized",H.Name,return 0);
#endif
	for(int i=0;i<SCI.Count;i++)
	{
		if(strncmp(SCI.Interfaces[i].CI->Name,H.Name,SCI_CONNEXION_NAME)==0)
		{
			return scxOpen(SCI.Interfaces[i].CI, Address, scxOpenFlag);
		}
	}
	DbgCHECK(1,"sciOpen: Invalid Address");
	//SPG_MemFastCheck();
	return 0;
}

#pragma warning( disable : 4706)//assignment within conditional expression

#ifdef SPG_General_USECONFIGFILEDLG

SCX_CONNEXION* SPG_CONV sciOpenFromDlg(SCM_INTERFACES& SCI)
{
	SCX_ADDRESS* Address;
	if(Address=sciAddressFromDlg(SCI))//affectation volontaire
	{
		SCX_CONNEXION* C=sciOpen(SCI,Address);
#ifdef SCX_ADDRESS_HASNAME
		DbgCHECKTWO(C==0,"sciOpenFromDlg:sciOpen failed",Address->H.Name);
#endif
		return C;
	}
	else
	{
		return 0;
	}
}

#endif

SCX_CONNEXION* SPG_CONV sciOpenFromFile(SCM_INTERFACES& SCI, char* Path, char* FileName)
{
	CHECK(FileName==0,"sciOpenFromFile",return 0);
	CHECK(FileName[0]==0,"sciOpenFromFile",return 0);
	SCX_ADDRESS* Address=0;
	SCX_CONNEXION* C=0;
	if(Address=sciAddressFromFile(SCI,Path,FileName))//affectation volontaire
	{
		C=sciOpen(SCI,Address);
#ifdef DebugMem
		if(C) 
		{
			SPG_MemCatName(C,FileName);
			SPG_MemCatName(C->State,FileName);
		}
#endif
#ifdef SCX_ADDRESS_HASNAME
		DbgCHECKTWO(C==0,"sciOpenFromFile:sciOpen failed",FileName);
#endif
	}
	//si C est valide C a pris la référence sur l'adresse et la destruction se fera dans scxClose(C), si C n'est pas valide l'adresse a été désalouée lors de la tentative de création (scxOpen)
	//SPG_MemFastCheck();
	return C;
}


SCX_CONNEXION* SPG_CONV sciOpenFromCFG(SCM_INTERFACES& SCI, char* Path, SPG_CONFIGFILE& CFG)
{
	SCX_ADDRESS* Address=0;
	SCX_CONNEXION* C=0;
	if(Address=sciAddressFromCFG(SCI,Path,CFG))//affectation volontaire
	{
		C=sciOpen(SCI,Address,SCXOPENPRESERVEADDRESS);
#ifdef SCX_ADDRESS_HASNAME
		DbgCHECK(C==0,"sciOpenFromCFG:sciOpen failed");
#endif
		//cas particulier dangereux, 
		//sciAddressFromCFG fait que CFG contient des liens vers Address,
		//qu'il faut éliminer avant de faire le destroy sur Address
		CFG_RemoveRef(CFG,Address,Address->H.sizeofAddress);
		sciDestroyAddress(SCI,Address);
	}
	//SPG_MemFastCheck();
	return C;
}


#endif
