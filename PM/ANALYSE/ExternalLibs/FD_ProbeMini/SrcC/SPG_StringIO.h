

//StringIO.h est inclus dans SPG_Includes.h apres connexion

int SPG_CONV StrMsg(SPGSTR& S, int hWnd, char* Title=SPG_COMPANYNAME, int Buttons=0);
#ifdef SPG_General_USECONNEXION
int SPG_CONV StrCat(SPGSTR& S, SCX_CONNEXION* C);
#endif
