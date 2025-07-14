#ifndef _SRCC_SPG_CONFIGFILE_H_
#define _SRCC_SPG_CONFIGFILE_H_

//#include "SPG_GlobalConst.h"

#ifdef SPG_General_USECONFIGFILE

#define SPG_CONFIGNAME 128
#define SPG_CONFIGSTRINGLEN MaxProgDir
#define SPG_CONFIGCOMMENT SPG_CONFIGSTRINGLEN

typedef struct
{//valeur parametre entier
	void* Var;//pointeur sur la valeur
} SPG_CONFIGVOIDPARAM;

typedef struct
{//valeur parametre entier
	int* Var;//pointeur sur la valeur
	int Var_F;//valeur mémorisée lue/ecrite dans le fichier
	int Min;
	int Max;
} SPG_CONFIGINTPARAM;

typedef struct
{//valeur parametre float
	float* Var;//pointeur sur la valeur
	float Var_F;//valeur mémorisée lue/ecrite dans le fichier
	float Min;
	float Max;
} SPG_CONFIGFLOATPARAM;

typedef struct
{//valeur parametre float
	double* Var;//pointeur sur la valeur
	double Var_F;//valeur mémorisée lue/ecrite dans le fichier
	double Min;
	double Max;
} SPG_CONFIGDOUBLEPARAM;

typedef struct
{//valeur parametre chaine
	const char* S;//pointeur sur la valeur
	char S_F[SPG_CONFIGSTRINGLEN];//valeur lue/ecrite dans le fichier
} SPG_CONFIGSTRINGPARAM;

typedef struct
{//ligne de commentaire
	char* S;
} SPG_CONFIGUSER;

//SPG_CONFIGPARAM.Type
typedef struct SPG_CFG_TYPE
{
	typedef enum ENUM:int
	{
		Null = 0,
		Int = 1,
		Float = 2,
		Double = 4,
		String = 8,
		User = 16,//comment field member only
		TypeMask = 31,
		HasVal = 32,
		HasPtr = 64,
		HasMin = 128,
		HasMax = 256,
		HasMinMax = (HasMin|HasMax)
	} ENUM;
	ENUM e;
	operator ENUM() { return this->e; }
	OP_ENUM_DECL(SPG_CFG_TYPE);
} SPG_CFG_TYPE;
OP_ENUM_STATICDECL(SPG_CFG_TYPE);

#define CP_INT                   SPG_CFG_TYPE::Int
#define CP_FLOAT				 SPG_CFG_TYPE::Float
#define CP_DOUBLE			 SPG_CFG_TYPE::Double
#define CP_STRING			 SPG_CFG_TYPE::String
#define CP_USER				 SPG_CFG_TYPE::User
#define CP_TYPEMASK		 SPG_CFG_TYPE::TypeMask
#define CP_HASVAL			 SPG_CFG_TYPE::HasVal
#define CP_HASPTR			 SPG_CFG_TYPE::HasPtr
#define CP_HASMIN			 SPG_CFG_TYPE::HasMin
#define CP_HASMAX			 SPG_CFG_TYPE::HasMax
#define CP_HASMINMAX	 SPG_CFG_TYPE::HasMinMax

typedef struct SPG_CFG_OTHER
{
	typedef enum ENUM:int
	{
		Null = 1,
		test = 2
	} ENUM;
	ENUM e;
	operator ENUM() { return this->e; }
	OP_ENUM_DECL(SPG_CFG_OTHER);
} SPG_CFG_OTHER;
OP_ENUM_STATICDECL(SPG_CFG_OTHER);

typedef struct
{//parametre
#ifdef DebugMem
	int MemStructCheckLower;
#endif
	//int Etat;
	SPG_CFG_TYPE Type;
	char Name[SPG_CONFIGNAME];
	char Comment[SPG_CONFIGCOMMENT];
	union
	{
		SPG_CONFIGVOIDPARAM v;
		SPG_CONFIGINTPARAM i;
		SPG_CONFIGFLOATPARAM f;
		SPG_CONFIGDOUBLEPARAM d;
		SPG_CONFIGSTRINGPARAM s;
		SPG_CONFIGUSER u;
	};
#ifdef DebugMem
	int MemStructCheckHigher;
#endif
} SPG_CONFIGPARAM;

//SPG_CONFIGPARAM.Etat
#define CP_EMPTY 0
#define CP_LOADED 1
#define CP_CREATED 2

#define SPG_CFG_FORMAT_TAB 1
#define SPG_CFG_FORMAT_COMMENT 2
#define SPG_CFG_FORMAT_MINMAX 4

typedef struct
{
#ifdef DebugMem
	int MemStructCheckLower;
#endif
	int Etat;
	char FileName[MaxProgDir];
//	SYSTEMTIME LoadTime;
//	SYSTEMTIME SaveTime;

	int WarningLevel;

	int MaxParams;
	int NumParams;
	SPG_CONFIGPARAM* CP;
#ifdef DebugMem
	int MemStructCheckHigher;
#endif
} SPG_CONFIGFILE;

//sauve toutes les valeurs dans le fichier
#define CFG_Save(CFG) {CFG_ParamsFromPtr(CFG);CFG_FlushToFile(CFG);}
//charge toutes les valeurs depuis le fichier
#define CFG_Load(CFG) {CFG_ParamsFromF(CFG);CFG_Save(CFG);}

#define CFG_DisabledString(s) V_DisabledString(s)

int SPG_CONV CFG_LoadFile(SPG_CONFIGFILE& CFG, const char* Directory, const char* FileName, int WarningLevel=1);
int SPG_CONV CFG_Init(SPG_CONFIGFILE& CFG, const char* Directory=0, const char* FileName=0, int MaxParams=128, int WarningLevel=2);
void SPG_CONV CFG_Close(SPG_CONFIGFILE& CFG, int FlushToFile=1);

int SPG_CONV CFG_ParamFromName(SPG_CONFIGFILE& CFG, const char* ParamName);
int SPG_CONV CFG_CreateName(SPG_CONFIGFILE& CFG, const char* ParamName, SPG_CFG_TYPE Type, const char* Comment, int Clear=1);

int SPG_CONV CFG_FindPtr(SPG_CONFIGFILE& CFG, void* Var);

//etablit la correspondance entre un parametre défini par son pointeur et son nom
int SPG_CONV CFG_IntParam(SPG_CONFIGFILE& CFG, const char* ParamName, int* Variable, const char* Comment=0, int LoadImmediate=1, SPG_CFG_TYPE Type=CP_INT, int Min=0, int Max=0);
int SPG_CONV CFG_FloatParam(SPG_CONFIGFILE& CFG, const char* ParamName, float* Variable, const char* Comment=0, int LoadImmediate=1, SPG_CFG_TYPE Type=CP_FLOAT, float Min=0.0f, float Max=0.0f);
int SPG_CONV CFG_DoubleParam(SPG_CONFIGFILE& CFG, const char* ParamName, double* Variable, const char* Comment=0, int LoadImmediate=1, SPG_CFG_TYPE Type=CP_DOUBLE, double Min=0.0, double Max=0.0);
int SPG_CONV CFG_StringParam(SPG_CONFIGFILE& CFG, const char* ParamName, const char* Variable, const char* Comment=0, int LoadImmediate=1, SPG_CFG_TYPE Type=CP_STRING);


#define CFG_PathParam(CFG,Name,S,Comment) {CFG_StringParam(CFG,Name,S,Comment);SPG_PathOnly(S);}

//copie une valeur du fichier sur une valeur pointée
void SPG_CONV CFG_ParamFromF(SPG_CONFIGFILE& CFG, int i);
//copie les valeurs du fichier sur les valeurs pointées
void SPG_CONV CFG_ParamsFromF(SPG_CONFIGFILE& CFG);
//lit les valeurs des variables pointées pour les mémoriser
void SPG_CONV CFG_ParamsFromPtr(SPG_CONFIGFILE& CFG);
//enleve les pointeurs sur un zone mémoire donnée
void SPG_CONV CFG_RemoveRef(SPG_CONFIGFILE& CFG, void* M, int Len=1);//len=sizeof(struct) est utilisé pour enlever les reference aveuglement sur une structure de données
//ecrit un parametre en chaine de caractere
int SPG_CONV CFG_ParamVarFToString(SPG_CONFIGFILE& CFG, SPG_CONFIGPARAM& CurPar, int Format, char* OutputString, int OutputLen);
//ecrit les valeurs mémorisées en fichier
void SPG_CONV CFG_FlushToFile(SPG_CONFIGFILE& CFG, int Format=SPG_CFG_FORMAT_TAB|SPG_CFG_FORMAT_COMMENT|SPG_CFG_FORMAT_MINMAX, bool UpdateFromPtr=true);

int SPG_CONV CFG_SetIntParam(SPG_CONFIGFILE& CFG, const char* ParamName, int ParamValue, int Silent=0);
int SPG_CONV CFG_GetIntParam(SPG_CONFIGFILE& CFG, const char* ParamName, int Default, const char* Comment=0);
int SPG_CONV CFG_SetFloatParam(SPG_CONFIGFILE& CFG, const char* ParamName, float ParamValue, int Silent=0);
float SPG_CONV CFG_GetFloatParam(SPG_CONFIGFILE& CFG, const char* ParamName, float Default, const char* Comment=0);
int SPG_CONV CFG_SetDoubleParam(SPG_CONFIGFILE& CFG, const char* ParamName, double ParamValue, int Silent=0);
double SPG_CONV CFG_GetDoubleParam(SPG_CONFIGFILE& CFG, const char* ParamName, double Default, const char* Comment=0);
int SPG_CONV CFG_SetStringParam(SPG_CONFIGFILE& CFG, const char* ParamName, const char* ParamValue, int Silent=0);
const char* SPG_CONV CFG_GetStringParam(SPG_CONFIGFILE& CFG, const char* ParamName, const char* Default, const char* Comment=0);

//int SPG_CONV CFG_GetIntParam(SPG_CONFIGFILE& CFG, char* ParamName, int Default, int Min, int Max, char* Comment=0);
//float SPG_CONV CFG_GetFloatParam(SPG_CONFIGFILE& CFG, char* ParamName, float Default, float Min, float Max, char* Comment=0);
//char* SPG_CONV CFG_GetStringParam(SPG_CONFIGFILE& CFG, char* ParamName, char* Default, char* Comment=0);
//SPG_CONFIGPARAM* SPG_CONV CFG_GetParam(SPG_CONFIGFILE& CFG, char* ParamName, int DefaultType);


#define CFG_GetInt(CFG,Variable) CFG_IntParam(CFG,#Variable,&Variable,0,1)
#define CFG_GetIntD(CFG,Variable,Vdefault) {Variable=Vdefault;CFG_IntParam(CFG,#Variable,&Variable,"default:" #Vdefault,1);}
#define CFG_GetIntDMM(CFG,Variable,Vdefault,Vmin,Vmax) {Variable=Vdefault;CFG_IntParam(CFG,#Variable,&Variable,"default:" #Vdefault,1,CP_INT|CP_HASMINMAX,Vmin,Vmax);}
#define CFG_GetIntC(CFG,Variable,Comment) CFG_IntParam(CFG,#Variable,&Variable,Comment,1)
#define CFG_GetIntDC(CFG,Variable,Vdefault,Comment) {Variable=Vdefault;CFG_IntParam(CFG,#Variable,&Variable,Comment " default:" #Vdefault,1);}

#define CFG_GetFloat(CFG,Variable) CFG_FloatParam(CFG,#Variable,&Variable,0,1)
#define CFG_GetFloatD(CFG,Variable,Vdefault) {Variable=Vdefault;CFG_FloatParam(CFG,#Variable,&Variable,"default:" #Vdefault,1);}
#define CFG_GetFloatDMM(CFG,Variable,Vdefault,Vmin,Vmax) {Variable=Vdefault;CFG_FloatParam(CFG,#Variable,&Variable,"default:" #Vdefault,1,CP_FLOAT|CP_HASMINMAX,Vmin,Vmax);}
#define CFG_GetFloatC(CFG,Variable,Comment) CFG_FloatParam(CFG,#Variable,&Variable,Comment,1)
#define CFG_GetFloatDC(CFG,Variable,Vdefault,Comment) {Variable=Vdefault;CFG_FloatParam(CFG,#Variable,&Variable,Comment " default:" #Vdefault,1);}

#define CFG_GetDouble(CFG,Variable) CFG_DoubleParam(CFG,#Variable,&Variable,0,1)
#define CFG_GetDoubleD(CFG,Variable,Vdefault) {Variable=Vdefault;CFG_DoubleParam(CFG,#Variable,&Variable,"default:" #Vdefault,1);}
#define CFG_GetDoubleC(CFG,Variable,Comment) CFG_DoubleParam(CFG,#Variable,&Variable,Comment,1)
#define CFG_GetDoubleDC(CFG,Variable,Vdefault,Comment) {Variable=Vdefault;CFG_DoubleParam(CFG,#Variable,&Variable,Comment " default:" #Vdefault,1);}

#define CFG_GetString(CFG,Variable) CFG_StringParam(CFG,#Variable,(char*)Variable,0,1)
#define CFG_GetStringC(CFG,Variable,Comment) CFG_StringParam(CFG,#Variable,(char*)Variable,Comment,1)

#define CFG_GetIntA(CFG, S, index, Variable) {char Name[64]; sprintf(Name,#S "[%i]." #Variable,index); CFG_IntParam(CFG,Name,&S[index].Variable,0,1); }

#define CFG_GetFloatA(CFG, S, index, Variable) {char Name[64]; sprintf(Name,#S "[%i]." #Variable,index); CFG_FloatParam(CFG,Name,&S[index].Variable,0,1); }

#endif


#endif //_SRCC_SPG_CONFIGFILE_H_