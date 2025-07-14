
#include "..\SrcC\SPG.h"
#include "..\SrcC\SPG_SysInc.h"

#ifdef SPG_General_USEWindows
int SPG_CONV StrMsg(SPGSTR& S, int hWnd, char* Title, int Buttons)
{
	return MessageBox((HWND)hWnd,StrZGet(S),Title,Buttons);
}
#endif

#ifdef SPG_General_USECONNEXION
int SPG_CONV StrCat(SPGSTR& S, SCX_CONNEXION* C)
{
	int RLen=S.MaxLen-S.Len;
	S.Len+=scxRead(S.D+S.Len,RLen,C);
	return RLen;
}
#endif

