
#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION
#ifdef SPG_General_USECyUSB

#include "..\SPG_Includes.h"

#include <string.h>
#include <memory.h>
#include <stdlib.h>
#include <stdio.h>

#include "..\SPG_SysInc.h"
#include "SCM_Connexion_CyUSB_Internal.h"

/*
#define USBEndPtR State.CyUSB->BulkInEndPt
#define USBEndPtW State.CyUSB->ControlEndPt
*/

#define CyUSBLog(Msg, Var) CyUSBLogInt(Msg,#Var,Var)

void CyUSBLogInt(char* Msg, char* VarName, int Value)
{
	FILE* F=fopen("CyUSBLog.txt","ab+");
	if(F)
	{
		fprintf(F,"%s: %s=%i\r\n",Msg,VarName,Value);
		fclose(F);
	}
	return;
}

void CyUSBLogMsg(char* Msg)
{
	FILE* F=fopen("CyUSBLog.txt","ab+");
	if(F)
	{
		fprintf(F,Msg);
		fprintf(F,"\r\n");
		fclose(F);
	}
	return;
}

void CyUSBLogNewLine()
{
	FILE* F=fopen("CyUSBLog.txt","ab+");
	if(F)
	{
		fprintf(F,"\r\n---------------\r\n");
		fclose(F);
	}
	return;
}

static SCX_CONNEXION* SPG_CONV scxCyUSBOpen(SCI_CONNEXIONINTERFACE* CI, SCX_ADDRESS* Address, int scxOpenFlag)
{
	sciCHECK(CI,"scxOpen");
	SCX_CONNEXION* C=scxTypeAlloc(SCX_CONNEXION,sci_NAME ":scxOpen");
	C->State=scxTypeAlloc(SCX_STATE,sci_NAME ":scxOpen");
	C->State->Address=*Address;
	SCX_STATE& State=*C->State;

	DbgCHECK(State.CyUSB!=0,"scxCyUSBOpen");

			CyUSBLogNewLine();CyUSBLogMsg("scxCyUSBOpen");

	State.CyUSB=new CCyUSBDevice();//0,StringToGuid(Address->DriverGUID));
	CHECK(State.CyUSB==0,"scxCyUSBOpen:new CCyUSBDevice failed",scxFree(C->State);scxFree(C);return 0);
	State.Address.infoDeviceCount=State.CyUSB->DeviceCount();
			CyUSBLog("scxCyUSBOpen,State.Address.infoDeviceCount=State.CyUSB->DeviceCount()",State.Address.infoDeviceCount);
	CHECK(State.Address.infoDeviceCount==0,"scxCyUSBOpen: no device found",scxFree(C->State);scxFree(C);return 0);

	int DevNth=0;
	int CorrespondingDevice=-1;
	for(int i=0;i<State.Address.infoDeviceCount;i++)
	{
		bool b=State.CyUSB->Open(i);//initialise CyUSB en tant que device nr i
		if(b==0)
		{
			State.CyUSB->Reset();//voir CyAPI.pdf p34
			b=State.CyUSB->Open(i);
		}
		if(b==0) {State.CyUSB->Close(); CyUSBLog("scxCyUSBOpen,Open(i) failed",i); continue;}//n'a pas pu ouvrir le device nr i
			CyUSBLog("scxCyUSBOpen,Open(i) succeded",i);

		int VendorID=State.CyUSB->VendorID;		CyUSBLog("scxCyUSBOpen,Open(i)",VendorID);
		int ProductID=State.CyUSB->ProductID;	CyUSBLog("scxCyUSBOpen,Open(i)",ProductID);
		
		if(i<SCX_USB_MAXDEVICE) 
		{
			State.Address.infoProductID[i]=ProductID;
			State.Address.infoVendorID[i]=VendorID;
		}
		if((ProductID==State.Address.ProductID)&&(VendorID==State.Address.VendorID)) 
		{
			if(DevNth==State.Address.DevNth) CorrespondingDevice=i;
			DevNth++;
		}
		State.CyUSB->Close();
		State.CyUSB->Reset();
	}

	if(CorrespondingDevice!=-1)
	{
		CyUSBLog("scxCyUSBOpen,Open",CorrespondingDevice);
		bool b=State.CyUSB->Open(CorrespondingDevice);
		if(b==0)
		{
			State.CyUSB->Reset();//voir CyAPI.pdf p34
			b=State.CyUSB->Open(CorrespondingDevice);
		}
	}
	else
	{//pas de périphérique présent, ou Open a échoué, ou pas de PID/VID
		CyUSBLogMsg("scxCyUSBOpen: no matching Nth PID/VID");
		DbgCHECK(1,"scxCyUSBOpen: no matching Nth PID/VID");
		if(State.CyUSB) delete State.CyUSB;
		State.CyUSB=0;
		scxFree(C->State);scxFree(C);return 0;
	}

	DbgCHECK(State.CyUSB->IsOpen()==0,"scxCyUSBOpen: Open failed (information)");

	//State.CyUSB->SetConfig(State.Address.cfg);

	//if(State.Address.XferSize>0) USBEndPtR->SetXferSize(State.Address.XferSize);

	USBEndPtR(C)->TimeOut=State.Address.msTimeOutRead;
	USBEndPtW(C)->TimeOut=State.Address.msTimeOutWrite;

	C->Etat=scxOK;
	return C;
}






//----------------------------------------

static int SPG_CONV scxCyUSBClose(SCX_CONNEXION* C)
{
	scxCHECK(C, "scxClose");
	SCX_STATE& State=*C->State;
	C->Etat=scxINVALID;

	DbgCHECK(State.CyUSB==0,"scxCyUSBClose");
	if(State.CyUSB) 
	{
		ASYNC_USB_TRANSFERT& AR=State.AR;
		if(AR.Status==ASYNC_STATUS_RUNNING)
		{
			USBEndPtR(C)->Abort();
			long lDataLen=AR.DataLen;
			USBEndPtR(C)->FinishDataXfer((BYTE*)AR.Data,lDataLen,&AR.ov,AR.Context);
			CloseHandle(State.AR.ov.hEvent);
			State.AR.Data=0;
			State.AR.DataLen=0;
			State.AR.Context=0;
			State.AR.Status=ASYNC_STATUS_OFF;
		}
		State.CyUSB->Close(); //ajout
		delete State.CyUSB;
	}
	State.CyUSB=0;

	scxFree(C->State);scxFree(C);
	return scxOK;
}

//----------------------------------------

static int SPG_CONV scxCyUSBWrite(void* Data, int DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxWrite");
	SCX_STATE& State=*C->State;

	//DbgCHECK(State.Address.WriteEndPoint!=0,"scxCyUSBWrite");
	//if(State.Address.WriteEndPoint==0)
	//{

	CHECK(DataLen<5,"scxCyUSBWrite: Invalid write data: ReqCode 1 byte - Value 2 bytes - Index 2 bytes - Data",return 0);

	State.CyUSB->ControlEndPt->ReqCode = ((BYTE*)Data)[0];	
	State.CyUSB->ControlEndPt->Value = *(WORD*)((char*)Data+1);
	State.CyUSB->ControlEndPt->Index = *(WORD*)((char*)Data+3);
	long lDataLen = DataLen-5;
	//CHECK(State.CyUSB->ControlEndPt->Write(lDataLen?(BYTE*)Data+5:0,lDataLen)==0,"scxCyUSBWrite",return 0);
	if(State.CyUSB->ControlEndPt->Write(lDataLen?(BYTE*)Data+5:0,lDataLen)==0) return 0;
	return lDataLen+5;

	//}
	//else
	//{
	//	long lDataLen=DataLen;
	//	USBEndPtW(C)->XferData((BYTE*)Data,lDataLen);//==0,"scxCyUSBRead",return DataLen=0);
	//	return (int)lDataLen;//Read();
	//}
}

//----------------------------------------

static int SPG_CONV scxCyUSBRead(void* Data, int& DataLen, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;

	if(State.Address.AsynchronousRead)
	{
		ASYNC_USB_TRANSFERT& AR=State.AR;
		if(AR.Status==ASYNC_STATUS_OFF)
		{//démarre une async read - il faut appeler read en boucle avec les mêmes parametres pour suivre l'état d'avancement de la copie
			AR.ov.hEvent=CreateEvent(0,0,0,0);//"scxCyUSBRead");
			AR.Data=Data;
			AR.DataLen=DataLen;
			AR.Status=ASYNC_STATUS_RUNNING;
			AR.Context=USBEndPtR(C)->BeginDataXfer((BYTE*)Data,DataLen,&AR.ov);
			return DataLen=-1;//0 = erreur, -1 = copie en cours
		}
		else if(AR.Status==ASYNC_STATUS_RUNNING)
		{//il faut appeler read en boucle avec les mêmes parametres pour suivre l'état d'avancement de la copie
			CHECK((Data!=AR.Data)||(DataLen!=AR.DataLen),"scxCyUSBRead: Asynchronous Read Pending",return 0);

			if(WaitForSingleObject(AR.ov.hEvent,0)==WAIT_TIMEOUT)	
			{//la copie est toujours en cours
				return DataLen=-1;//-1 = copie en cours
			}
			else
			{//la copie est terminée
				long lDataLen=DataLen;
				USBEndPtR(C)->FinishDataXfer((BYTE*)Data,lDataLen,&AR.ov,AR.Context);
				CloseHandle(State.AR.ov.hEvent);
				State.AR.Data=0;
				State.AR.DataLen=0;
				State.AR.Context=0;
				State.AR.Status=ASYNC_STATUS_OFF;
				return DataLen=lDataLen;//renvoie le nombre total d'octets copiés
			}
		}
		CHECK(1,"scxCyUSBRead:AsynchronousRead:AR.Status unexpected value",return 0);
	}
	else
	{
		long lDataLen=DataLen;
		USBEndPtR(C)->XferData((BYTE*)Data,lDataLen);//==0,"scxCyUSBRead",return DataLen=0);
		return DataLen=lDataLen;//Read();
	}
}

//----------------------------------------

//FONCTION SPECIFIQUE

int SPG_CONV scxCyUSBRead(void* Data, int& DataLen, SCX_CONNEXION* C, int EndPointNr) //non static car utilisé dans FogaleProbe\FP_Chrom\Comm.cpp
{
	scxCHECK(C, "scxRead");
	SCX_STATE& State=*C->State;

	CHECK(EndPointNr>=C->State->CyUSB->EndPointCount(),"scxCyUSBRead: EndPointNr does not exist on the device",return DataLen=0);

	long lDataLen=DataLen;
	//USBEndPtR(C)->XferData((BYTE*)Data,lDataLen);
	(C->State->CyUSB->EndPoints[EndPointNr])->XferData((BYTE*)Data,lDataLen);
	return DataLen=lDataLen;
}


//----------------------------------------

static void SPG_CONV scxCyUSBCfgAddress(SPG_CONFIGFILE& CFG, SCX_ADDRESS* Address, int Flag)
{
	CHECK(Address==0,"scxCfgAddress",return);
	if(Flag&scxCFGADDRESSSETDEFAULT)
	{
		Address->DevNth=0; //0: Ouvre le nième device (compté de zero) de ceux dont VendorID et ProductID correspondent
		Address->VendorID=0;
		Address->ProductID=0;
		//Address->ReadEndPoint=2;
		//Address->WriteEndPoint=0;
		//Address->XferSize=0; //4194304 = 2^22 ~ 480000000 / 8 / 200ms;
		Address->AsynchronousRead=0;
		Address->msTimeOutRead=10;//le minimum possible d'apres CyAPI.pdf est 1000
		Address->msTimeOutWrite=100;//le minimum possible d'apres CyAPI.pdf est 1000
		//strcpy(Address->DriverGUID,@CYUSBDRV_GUID);
	}

	if(CFG.Etat==0) CFG_Init(CFG,0,0);
	CFG_StringParam(CFG,"Name",Address->H.Name,0,1);
	CFG_IntParam(CFG,"DevNth",&Address->DevNth,"Ouvre le nième device (compté de zero) de ceux dont VendorID et ProductID correspondent",1);
	CFG_IntParam(CFG,"VendorID",&Address->VendorID,0,1);
	CFG_IntParam(CFG,"ProductID",&Address->ProductID,0,1);
	//CFG_IntParam(CFG,"ReadEndPoint",&Address->ReadEndPoint,0,7);
	//CFG_IntParam(CFG,"WriteEndPoint",&Address->WriteEndPoint,0,7);
	//CFG_IntParam(CFG,"XferSize",&Address->XferSize,0,1);
	CFG_IntParam(CFG,"AsynchronousRead",&Address->AsynchronousRead,0,1);
	CFG_IntParam(CFG,"msTimeOutRead",&Address->msTimeOutRead,0,1);
	CFG_IntParam(CFG,"msTimeOutWrite",&Address->msTimeOutWrite,0,1);
	//CFG_StringParam(CFG,"DriverGUID",Address->DriverGUID,0,1);

//	CFG_IntParam(CFG,"",&Address->,0,1,CP_INT);
	return;
}









//----------------------------------------

static int SPG_CONV scxCyUSBSetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxSetParameter");
	return 0;
}

//----------------------------------------

static int SPG_CONV scxCyUSBGetParameter(char* ParamName, void* Value, SCX_CONNEXION* C)
{
	scxCHECK(C, "scxGetParameter");
	return 0;
}

//----------------------------------------

static int SPG_CONV sciCyUSBDestroyConnexionInterface(SCI_CONNEXIONINTERFACE* CI)
{
	sciCHECK(CI,"scxDeleteConnexionInterface");
	scxFree(CI->sciParameters);	scxFree(CI);
	return scxOK;
}


//----------------------------------------

//fonctions exportées

SCI_CONNEXIONINTERFACE* SPG_CONV sciCyUSBCreateConnexionInterface()
{
	SCI_CONNEXIONINTERFACE* CI=scxTypeAlloc(SCI_CONNEXIONINTERFACE,sci_NAME ":sciCreateConnexionInterface()");

	CI->TypeUID=sci_UID;
	CI->Type=(SCI_TYPE)(sciBOTH|sciPACKETMERGE);
	strcpy(CI->Name,sci_NAME);

	CI->Description="USB port (Cypress API)";

	CI->sizeofAddress=sizeof(SCX_ADDRESS);
	CI->maxPacketSize=1024;//spécifique

	CI->scxOpen=scxCyUSBOpen;
	CI->scxClose=scxCyUSBClose;
	CI->scxWrite=scxCyUSBWrite;
	CI->scxRead=scxCyUSBRead;
	CI->scxCfgAddress=scxCyUSBCfgAddress;
	CI->scxSetParameter=scxCyUSBSetParameter;
	CI->scxGetParameter=scxCyUSBGetParameter;
	CI->sciDestroyConnexionInterface=sciCyUSBDestroyConnexionInterface;

	CI->sciParameters=scxTypeAlloc(SCI_PARAMETERS,sci_NAME ":sciCreateConnexionInterface()");
	SCI_PARAMETERS& sciParameters=*CI->sciParameters;

	//sciParameters.DummyInterfaceParam=0;//spécifique

	CI->Etat=scxOK;
	return CI;
}

//----------------------------------------

#else
#pragma SPGMSG(__FILE__,__LINE__,"Connexion : Cypress USB disabled")
#endif
#endif
