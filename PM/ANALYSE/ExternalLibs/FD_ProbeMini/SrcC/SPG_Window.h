
#define WMaxName 32

#define SPGWT_NoDisplay 0
#define SPGWT_Fixed 1
#define SPGWT_UserFriendly 2
#define SPGWT_Moveable 4
#define SPGWT_Sizeable 8
#define SPGWT_Topmost 16
#define SPGWT_AAGraphics 32
#define SPGWT_NoClose 64
//efine SPGWT_FullScreen 64
//efine SPGWT_ForceThisMode 128

struct SPG_Window;

#ifdef SPG_General_USEWindow

typedef int(SPG_CONV * SPG_USERWINDOWCALLBACK)(SPG_Window& SW, void* User, DWORD hwnd, DWORD uMsg, DWORD wParam, DWORD lParam);

typedef struct SPG_Window
{
	int Etat;

	int LT_WProc;
	int LT_Callback;
	int LT_Blit;

	DWORD ThreadID;

#ifdef SPG_General_USESpinLock
	SPINLOCK L;
#endif
	int Interlocked;
	//int AllocatedSize;
	G_Ecran Ecran;

#ifdef SPG_General_USEGEFFECT
	ANTIALIASECRAN AG;
#else
	struct
	{
		int K;
	} AG;
#endif
	G_Ecran EBlit;

	int SPGWindowType;
	int EType;
	int SizeX;
	int SizeY;
	int Depth;

	int CreateWindowFlag;

	int ScreenX;
	int ScreenY;
	int ExternX;//dimensions exterieures
	int ExternY;//dimensions exterieures
	int ExternPosX;//position de la fenetre dans l'ecran
	int ExternPosY;

	int hInstance;

	int hWndWin;
	int HDCWND;
	void* FullScreenPalette;
	void* OldPalette;
	void* Palette;
	char WName[WMaxName];
	char WClassName[WMaxName];

	bool Active;
	bool Visible;
	bool SizeChanged;
	bool UserInput;
	bool MouseLeft;
	bool MouseRight;
	int MouseX;
	int MouseY;

	SPG_USERWINDOWCALLBACK WCB;//si on spécifie une callback celle ci doit préférablement se terminer par return DefWindowProc(hwnd,uMsg,wParam,lParam)
	void* User;

} SPG_Window;

#define KEYPRESSED(vk) (HasFocus && ((GetAsyncKeyState(vk)&0x8001)!=0)) 
#define KEYSTROKE(vk)  (HasFocus && ((GetAsyncKeyState(vk)&0x0001)==0x0001)) 

//#define SPG_WindowHasFocus(SW) (GetFocus()==(HWND)SW.hWndWin)
#define SPG_WindowHasFocus(SW) (SW.Active) //alternative
#define SPG_WindowVisible(SW) (SW.Visible) //alternative

#define SPG_ReadWindowParams(S,defXAlign,defYAlign,defSizeX,defSizeY,defType) { CFG_GetFloatDMM(S.CFG,S.XAlign,defXAlign,-1,1); CFG_GetFloatDMM(S.CFG,S.YAlign,defYAlign,-1,1); CFG_GetIntD(S.CFG,S.SizeX,defSizeX); CFG_GetIntD(S.CFG,S.SizeY,defSizeY); CFG_GetIntDC(S.CFG,S.SPGWindowType,defType,"1:Fixed 2:UserFriendly 4:Moveable 8:Sizeable 16:Topmost 32:AAGraphics 64:NoClose"); }

int SPG_CONV SPG_CreateWindow(SPG_Window& SW, int SPGWindowType, int EType, float XAlign, float YAlign, int SizeX, int SizeY, int Depth, const char* WName, const char* WClassName, SPG_USERWINDOWCALLBACK WCB=0, void* User=0, int hInstance=0);
int SPG_CONV SPG_CloseWindow(SPG_Window& SW);
void SPG_CONV SPG_BlitWindow(SPG_Window& SW);

int SPG_CONV SPG_CreateWindow(SPG_Window* &SW, int SPGWindowType, int EType, int XAlign, int YAlign, int SizeX, int SizeY, int Depth, const char* WName, const char* WClassName, SPG_USERWINDOWCALLBACK WCB=0, void* User=0, int hInstance=0);
int SPG_CONV SPG_CloseWindow(SPG_Window* &SW);
void SPG_CONV SPG_BlitWindow(SPG_Window* &SW);

#else

#define KEYPRESSED(vk) 0
#define KEYSTROKE(vk)  0

//#define SPG_WindowHasFocus(SW) (GetFocus()==(HWND)SW.hWndWin)
#define SPG_WindowHasFocus(SW) (SW.Active) 0
#define SPG_WindowVisible(SW) (SW.Visible) 0

#define SPG_ReadWindowParams(S,defXAlign,defYAlign,defSizeX,defSizeY,defType)

#define SPG_CreateWindow(SW, SPGWindowType, EType, XAlign, YAlign, SizeX, SizeY, Depth, WName, WClassName, WCB, User, hInstance)
#define SPG_CloseWindow(SW)
#define SPG_BlitWindow(SW)

#endif