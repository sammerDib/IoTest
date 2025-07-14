
//OpenMode 1 Read 2 Write
#define sciOpenFile(C, FileName, OpenMode)	{ SPG_CONFIGFILE CFG; SPG_ZeroStruct(CFG); SCI_CONNEXIONINTERFACE* pCI; SCX_ADDRESS* Address=sciAddressFromName(Global.SCI,CFG,sci_NAME_FILE,&pCI); CFG_SetIntParam(CFG,"OpenMode",OpenMode); CFG_SetStringParam(CFG,"FileName",FileName); CFG_Close(CFG);	C=scxOpen(pCI,Address);}
#define sciOpenReadFile(C, FileName)	sciOpenFile(C, FileName, 1)
#define sciOpenWriteFile(C, FileName)	sciOpenFile(C, FileName, 2)

int SPG_CONV BackupMe(char* decl, SPG_CONFIGFILE& CFG, SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, SCX_CONNEXION* p, SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, SCX_ADDRESS** p, SCX_CONNEXION* C);//R.Address W.Address PROTOCOL
int SPG_CONV BackupMe(char* decl, SCX_ADDRESS* p, SCX_CONNEXION* C);
int SPG_CONV BackupMe(char* decl, SCI_CONNEXIONINTERFACE** p, SCX_CONNEXION* C);//R.CI W.CI PROTOCOL
int SPG_CONV BackupMe(char* decl, SCI_CONNEXIONINTERFACE* p, SCX_CONNEXION* C);