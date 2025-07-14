
#include "..\SrcC\SPG.h"
#include "..\SrcC\SPG_SysInc.h"

#include <string.h>
#include <stdio.h>

#ifdef SPG_General_USEBackupMe

int SPG_CONV BackupMe(char* decl, BYTE &p, SCX_CONNEXION* C) { BMW(sprintf(S,"%s\t=\t%i;\t//0x%X '%c'",decl,p,(int)p,(int)p)); return -1;}

int SPG_CONV BackupMe(char* decl, char &p, SCX_CONNEXION* C) { BMW(sprintf(S,"%s\t=\t%i;\t//0x%X '%c'",decl,p,(int)p,(int)p)); return -1;}

int SPG_CONV BackupMe(char* decl, char* &p, SCX_CONNEXION* C) { BMW(if(p==0) sprintf(S,"%s\t=\t(null ptr);",decl,p); else if(p[0]==0) sprintf(S,"%s\t=\t(empty string);",decl); else sprintf(S,"%s\t=\t\"%s\";",decl,p)); return -1;}

int SPG_CONV BackupMe(char* decl, DWORD &p,   SCX_CONNEXION* C) { BMW(sprintf(S,"%s\t=\t%i;\t//0x%X",decl,p,p)); return -1;}

int SPG_CONV BackupMe(char* decl, int &p,   SCX_CONNEXION* C) { BMW(sprintf(S,"%s\t=\t%i;\t//0x%X",decl,p,p)); return -1;}

int SPG_CONV BackupMe(char* decl, short &p,   SCX_CONNEXION* C) { BMW(sprintf(S,"%s\t=\t%i;\t//0x%X",decl,p,p)); return -1;}

int SPG_CONV BackupMe(char* decl, float &p, SCX_CONNEXION* C) { BMW(sprintf(S,"%s\t=\t",decl); CF_GetString(S,p,CF_DIGITFLOAT);strcat(S,";")); return -1;}

int SPG_CONV BackupMe(char* decl, double &p, SCX_CONNEXION* C) { BMW(sprintf(S,"%s\t=\t",decl); CF_GetString(S,p,CF_DIGITDOUBLE);strcat(S,";")); return -1;}

int SPG_CONV BackupMe(char* decl, __int64 &p,   SCX_CONNEXION* C) { BMW(sprintf(S,"%s\t=\t%I64i;\t//0x%I64X",decl,p,p)); return -1;}

int SPG_CONV BackupMe(char* decl, void* &p, SCX_CONNEXION* C) { BMW(if(p==0) sprintf(S,"%s\t=\t(null ptr);",decl); else sprintf(S,"%s\t=\t0x%0.8X;",decl,*(int*)&p);); return -1;}

int SPG_CONV ArrayBackupMe(char* decl, char* p, int n, SCX_CONNEXION* C) { CHECKTWO(strlen(p)>n,"ArrayBackupMe",decl,return 0);BMWL(if(p==0) sprintf(S,"%s\t=\t(null ptr);",decl,p); else if(p[0]==0) sprintf(S,"%s\t=\t(empty string);",decl); else sprintf(S,"%s\t=\t\"%s\";",decl,p)); return -1;}

#endif
