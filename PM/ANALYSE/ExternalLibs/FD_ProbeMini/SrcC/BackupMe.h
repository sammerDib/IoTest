
#ifdef SPG_General_USEBackupMe

//template <typename T>int SPG_CONV ArrayBackupMe(char* decl, T* p, int n, SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, BYTE& p, SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, char& p, SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, char* &p, SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, DWORD& p,   SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, short& p,   SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, int& p,   SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, float& p, SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, double& p, SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, __int64& p, SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, void* &p, SCX_CONNEXION* C);

int SPG_CONV ArrayBackupMe(char* decl, char* p, int n, SCX_CONNEXION* C);

template <typename T>int SPG_CONV ArrayBackupMe(char* decl, T* p, int n, SCX_CONNEXION* C) 
{ 
	char S[128]; 
	if(p==0)
	{
		sprintf(S,"%s = 0;\t//null ptr\r\n",decl); scxWrite(S,strlen(S),C);
	}
	else
	{
		/*
		int nmax;
		for(nmax=n-1;nmax>=1;nmax--) 
		{ 
			if(memcmp(p[nmax-1],0,sizeof(T))) break;
		}
		*/
		int nmax=n-1;
		for(int i=0;i<=nmax;i++) 
		{ 
			sprintf(S,"%s[%i]",decl,i); BackupMe(S,p[i],C);
		}
	}
	return -1; 
}

#define SBM(B,T,t,C) BackupMe(#T " " #t,B.t,C);	//Membre d'une structure
#define VBM(T,t,C) BackupMe(#T " " #t,t,C);	//Variable locale
#define EBM(B,T,t,C) BackupMe(#T " " #t,*(int*)&B.t,C);	//Enum
#define PTRBM(B,T,t,C) BackupMe(#T " " #t,*(void**)&(B.t),C);
//efine STRZBM(B,T,t,n,C) StrzBackupMe(#T " " #t,B.t,n,C);
#define ABM(B,T,t,n,C) ArrayBackupMe(#T " " #t,B.t,n,C);
#define ANBM(B,T,t,C) ArrayBackupMe(#T " " #t,B.t,B.Num##t,C);

#define BMW(f) char S[128]; f;  strcat(S,"\r\n"); scxWriteStrZ(S,C);
#define BMWL(f) char S[2048]; f;  strcat(S,"\r\n"); scxWriteStrZ(S,C);

#define SXF(xf,B,T,t,C) xf(#T " " #t,B.t,C);
#define VXF(xf,T,t,C) xf(#T " " #t,t,C);
#define PTRXF(xf,B,T,t,C) xf(#T " " #t,*(void**)&(B.t),C);
//efine STRZBM(B,T,t,n,C) StrzBackupMe(#T " " #t,B.t,n,C);
#define AXF(xf,B,T,t,n,C) Array##xf(#T " " #t,B.t,n,C);
#define ANXF(xf,B,T,t,C) Array##xf(#T " " #t,B.t,B.Num##t,C);

#define XFW(f) char S[128]; f;  strcat(S,"\r\n"); scxWriteStrZ(S,C);
#define XFWL(f) char S[2048]; f;  strcat(S,"\r\n"); scxWriteStrZ(S,C);

#endif
