#ifndef INC_SPG_SysInc_INC
#define	INC_SPG_SysInc_INC

#ifndef _INC_WINDOWS
#ifdef SPG_General_USEWindows

/*
#define VERTOSTRING(x) TOSTRING(x)
#define TOSTRING(x) #x
#pragma message("_MSC_VER=" VERTOSTRING(_MSC_VER))
*/


#if(_MSC_VER>=1500)
#include <memory.h>
#include <string.h>
#include <math.h>
#include <stdlib.h>
#include <stdio.h>
#include "SPG_WinVS2008\SPG_WinVS2008Hack.h"
#include "SPG_WinVS2008\SPG_WinVS2008.h"
#include "SPG_WinVS2008\SPG_WinVS2008Define.h"
//nclude <windows.h>
//nclude <winsock2.h>
//nclude <commdlg.h>
//nclude <vfw.h>
#elif(_MSC_VER<1400)
#include "SPG_Win98\SPG_Win98Hack.h"
#include "SPG_Win98\SPG_Win98Full.h"
#include "SPG_Win98\SPG_Win98Define.h"
#else
#define _WIN32_WINNT 0x0400 
#include <memory.h>
#include <string.h>
#include <math.h>
#include <stdlib.h>
#include <stdio.h>
#include <windows.h>
#include <winsock2.h>
#include <commdlg.h>
#include <vfw.h>
#endif



#pragma comment(lib,"kernel32.lib")
#pragma comment(lib,"user32.lib")
#pragma comment(lib,"gdi32.lib")
//ragma comment(lib,"winspool.lib")
#pragma comment(lib,"comdlg32.lib")
//ragma comment(lib,"advapi32.lib")
//ragma comment(lib,"shell32.lib")
//ragma comment(lib,"ole32.lib")
//ragma comment(lib,"oleaut32.lib")
//ragma comment(lib,"uuid.lib")
//ragma comment(lib,"odbc32.lib")
//ragma comment(lib,"odbccp32.lib")
//#else
//#include <commdlg.h>
//#include "Config\SPG_Warning.h"
#endif
#else
//#error "Inclusion de windows.h en mode systeme-independant, definir USEWindows dans SPG_General.h"
#endif

#else
//error "SPG_SysInc Included twice"
//ragma SPGMSG(__FILE__,__LINE__,"SPG_SysInc Included twice")
#endif
