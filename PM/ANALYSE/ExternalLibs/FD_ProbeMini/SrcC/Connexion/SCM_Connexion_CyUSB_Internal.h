
//----------------------------------------

#define sci_UID sci_UID_CyUSB
#define sci_NAME sci_NAME_CyUSB

//----------------------------------------

#include "..\..\CypressUSB_SDK\CyAPI.h"
#pragma comment(lib,"..\\CypressUSB_SDK\\CyAPI.lib")
//attention CyAPI.lib contient la commande -defaultlib:LIBCMT; ce qui provoque le message LIBCD.lib(crt0init.obj) : warning LNK4098: defaultlib "libcmt.lib" conflicts with use of other libs; use /NODEFAULTLIB:library

//----------------------------------------

typedef struct SCI_PARAMETERS
{
	//parametres de l'interface
	//int DummyInterfaceParam;
} SCI_PARAMETERS; //parametres de l'interface


//----------------------------------------

#define SCX_USB_MAXDEVICE 8

typedef struct SCX_ADDRESS
{
	SCX_ADDRESSHEADER H;//obligatoire

	int DevNth;//Ouvre le nième device (compté de zero) de ceux dont VendorID et ProductID correspondent
	int VendorID;
	int ProductID;
	//int ReadEndPoint;
	//int WriteEndPoint;
	//int XferSize;//transfert size
	int AsynchronousRead;
	int msTimeOutRead;
	int msTimeOutWrite;

	int infoDeviceCount;//champs write only, actualisés par scxOpen
	WORD infoVendorID[SCX_USB_MAXDEVICE];
	WORD infoProductID[SCX_USB_MAXDEVICE];

	//...
} SCX_ADDRESS;



//----------------------------------------

#define ASYNC_STATUS_OFF 0
#define ASYNC_STATUS_RUNNING 1
#define ASYNC_STATUS_TERMINATED 2

typedef struct
{
	DWORD Status;
	OVERLAPPED ov;
	void* Data;
	int DataLen;
	BYTE* Context;
} ASYNC_USB_TRANSFERT;

typedef struct SCX_STATE
{
	SCX_ADDRESS Address;
	CCyUSBDevice *CyUSB;

	ASYNC_USB_TRANSFERT AR;

	//...
} SCX_STATE; //parametres d'une connexion en particulier


//----------------------------------------

//#define USBEndPtR(C) C->State->CyUSB->EndPoints[C->State->Address.ReadEndPoint]
//#define USBEndPtW(C) C->State->CyUSB->EndPoints[C->State->Address.WriteEndPoint]

#define USBEndPtR(C) C->State->CyUSB->BulkInEndPt
#define USBEndPtW(C) C->State->CyUSB->ControlEndPt

//----------------------------------------

