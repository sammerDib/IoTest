
#define MAX_SCI 32
//#define MAX_SCX 256

typedef enum
{
	SCM_CPP,
#ifdef SPG_General_USEWindows
	SCM_DLL,
#endif
	SCM_PTR
} SCMINTERFACE_TYPE;

typedef struct
{
} SCM_CPP_Interface;

#ifdef SPG_General_USEWindows
typedef struct
{
	int hModule;
} SCM_DLL_Interface;
#endif

typedef struct
{
} SCM_PTR_Interface;

typedef struct
{
	SCMINTERFACE_TYPE Type;
	SCI_CONNEXIONINTERFACE* CI;
	union
	{
		SCM_CPP_Interface CPPInterface;
#ifdef SPG_General_USEWindows
		SCM_DLL_Interface DLLInterface;
#endif
		SCM_PTR_Interface PTRInterface;
	};
} SCMODULEINTERFACE;

typedef struct
{
	int Count;
	SCMODULEINTERFACE Interfaces[MAX_SCI];
} SCM_INTERFACES;

int SPG_CONV sciCreateInterfaces(SCM_INTERFACES& SCI);
int SPG_CONV sciDestroyInterfaces(SCM_INTERFACES& SCI);
int SPG_CONV sciInterfacesAddDll(SCM_INTERFACES& SCI, char* DllName);
int SPG_CONV sciInterfacesAddPtr(SCM_INTERFACES& SCI, SCI_CREATE sciCreateConnexionInterface);
SCI_CONNEXIONINTERFACE* SPG_CONV sciInterfaceFromName(SCM_INTERFACES& SCI, char Name[SCI_CONNEXION_NAME]);
SCI_CONNEXIONINTERFACE* SPG_CONV sciInterfaceFromUID(SCM_INTERFACES& SCI, int TypeUID);
//SPG_CONV SCX_ADDRESS* sciAddressByTypeID(SCM_INTERFACES& SCI, SPG_CONFIGFILE& CFG, int TypeID);
SCX_ADDRESS* SPG_CONV sciAddressFromName(SCM_INTERFACES& SCI, SPG_CONFIGFILE& CFG, char Name[SCI_CONNEXION_NAME], SCI_CONNEXIONINTERFACE** pCI=0);
SCX_ADDRESS* SPG_CONV sciAddressFromDlg(SCM_INTERFACES& SCI, SCI_CONNEXIONINTERFACE** pCI=0);
SCX_ADDRESS* SPG_CONV sciAddressFromFile(SCM_INTERFACES& SCI, char* Path, char* FileName, SCI_CONNEXIONINTERFACE** pCI=0);
SCX_ADDRESS* SPG_CONV sciAddressDuplicate(SCM_INTERFACES& SCI, SCX_ADDRESS* AddressTemplate, SCI_CONNEXIONINTERFACE** pCI=0);
SCX_ADDRESS* SPG_CONV sciAddressDuplicate(SCI_CONNEXIONINTERFACE* CITemplate, SCX_ADDRESS* AddressTemplate, SCI_CONNEXIONINTERFACE** pCI=0);
#define sciAddressDupliquate sciAddressDuplicate
void SPG_CONV sciDestroyAddress(SCX_ADDRESS* &Address, SCI_CONNEXIONINTERFACE* CI);
void SPG_CONV sciDestroyAddress(SCM_INTERFACES& SCI, SCX_ADDRESS* &Address);

void SPG_CONV sciSetProtocolAddress(SCX_ADDRESS* Address, SCI_CONNEXIONINTERFACE* pCI, SCX_ADDRESS* ReadAddress, SCI_CONNEXIONINTERFACE* pRCI, SCX_ADDRESS* WriteAddress, SCI_CONNEXIONINTERFACE* pWCI);
void SPG_CONV sciSetProtocolAddress(SCX_ADDRESS* Address, SCI_CONNEXIONINTERFACE* pCI, SCX_ADDRESS* RWAddress, SCI_CONNEXIONINTERFACE* pRWCI);
void SPG_CONV sciGetProtocolAddress(SCX_ADDRESS* Address, SCI_CONNEXIONINTERFACE* pCI, SCX_ADDRESS** pReadAddress, SCI_CONNEXIONINTERFACE** pRCI, SCX_ADDRESS** pWriteAddress, SCI_CONNEXIONINTERFACE** pWCI);

//fonction apres laquelle il faut destroy CFG avant destroy Address car CFG point e sur Address - ou utiliser sciDestroyAddressFromCFG
SCX_ADDRESS* SPG_CONV sciAddressFromCFG(SCM_INTERFACES& SCI, char* Path, SPG_CONFIGFILE& CFG, SCI_CONNEXIONINTERFACE** pCI=0);
//void SPG_CONV sciDestroyAddressFromCFG(SCX_ADDRESS* &Address, SPG_CONFIGFILE& CFG);

#define sci16ToHEX(bQuartet) ((((bQuartet)&15)<10)?('0'+(bQuartet&15)):((BYTE)'A'-10+(bQuartet&15)))
#define sciHEXTo16(cQuartet) ((cQuartet<(BYTE)'A')?(cQuartet-(BYTE)'0'):(cQuartet+10-(BYTE)'A'))
void SPG_CONV sciDataToHex(void* Data, int DataLen, char* Msg, int MsgLen);
void SPG_CONV sciDataToString(void* Data, int DataLen, char* Msg, int MsgLen);
void SPG_CONV sciDataPtrToHEX(void* Data, char* Msg);

SCX_CONNEXION* SPG_CONV sciOpen(SCM_INTERFACES& SCI, SCX_ADDRESS* Address, int scxOpenFlag=0);
SCX_CONNEXION* SPG_CONV sciOpenFromDlg(SCM_INTERFACES& SCI);
SCX_CONNEXION* SPG_CONV sciOpenFromFile(SCM_INTERFACES& SCI, char* Path, char* FileName);
SCX_CONNEXION* SPG_CONV sciOpenFromCFG(SCM_INTERFACES& SCI, char* Path, SPG_CONFIGFILE& CFG);

#define sciClose(C) scxClose(C)
