
#include "SPG_General.h"

#ifdef SPG_General_USECONFIGFILE

#include "SPG_Includes.h"

#include <string.h>
#include <stdio.h>

#define MAXFLOATSTR 10

typedef enum {RS_Ready,RS_ReadLVal,RS_ReadEqual,RS_ReadRVal,RS_ReadLPar,RS_ReadMin,RS_ReadV,RS_ReadMax,RS_ReadRPar,RS_ReadComment,RS_User} ReaderState;

#define CFG_CHECK(CP,Msg,Ret) CHECKTWO(CP.Etat==0,Msg,CP.Name,Ret;);CHECKTWO(CP.Type==0,Msg,CP.Name,Ret;)

SPG_CFG_TYPE SPG_CONV CFG_ReadBloc(char* D, int& j, int Len, int& BlocStart, int& BlocStop)
{//détoure un bloc
	SPG_CFG_TYPE Type;
	Type=SPG_CFG_TYPE::Null;
	CHECK(D==0,"CFG_ReadBloc",return Type);
	BlocStop=BlocStart=j;
	for(;j<Len;j++)
	{
		if(V_InclusiveBound(D[j],'a','z')||V_InclusiveBound(D[j],'A','Z')||(D[j]=='_')||(D[j]=='*')||(D[j]==':')||(D[j]=='\\')||(D[j]=='/')||(D[j]=='è')||(D[j]=='é')||(D[j]=='\"')||(D[j]=='<')||(D[j]=='>')||(D[j]=='[')||(D[j]==']')||(D[j]=='(')||(D[j]==')'))
		{//ceci est destiné à la lecture d'un nom de parametre ou d'une valeur chaine; la lecture des chaines commentaires se fait dans une autre fonction qui lit jusqu'à la fin de la ligne
			if(Type.e==SPG_CFG_TYPE::Null) {Type=CP_STRING;BlocStop=BlocStart=j;}//démarre une chaine de caracteres
			else if(enum_test(Type,String)) {BlocStop=j;}//continue une chaine
			else if(((enum_test(Type,Int))||(enum_test(Type,Float))||(enum_test(Type,Double)))&&((D[j]=='e')||(D[j]=='E')))//continue un nombre (qui devient flottant 'e')
			{
				BlocStop=j;
				Type=CP_DOUBLE;
			}
			else return Type;
		}
		else if(V_InclusiveBound(D[j],'0','9')||(D[j]=='.')||(D[j]=='-')||(D[j]=='+'))
		{
			if(Type==SPG_CFG_TYPE::Null) {Type=CP_INT;if(D[j]=='.') Type=CP_DOUBLE;BlocStop=BlocStart=j;}//debute un nombre (qui devient flottant '.')
			else if((enum_test(Type,Int))||(enum_test(Type,Float))||(enum_test(Type,Double))) {{if(D[j]=='.') Type=CP_DOUBLE;};BlocStop=j;}//continue un nombre (qui devient flottant '.')
			else if(enum_test(Type,String))  {BlocStop=j;}//continue une chaine
			else return Type;
		}
		else if((enum_test(Type,String)&&(D[j]==' ')))
		{
			BlocStop=j;//continue une chaine
		}
		else 
		{
			if((Type==CP_DOUBLE)&&(BlocStop-BlocStart)<MAXFLOATSTR) Type=CP_FLOAT;
			return Type;
		}

		/*
			
		if((Type&CP_INT)||(Type&CP_FLOAT)||(Type&CP_DOUBLE)) return Type;//stoppe un nombre
		else if(Type&CP_STRING) return Type;//stoppe une chaine
		else //if((Type==0)&&(D[j]=='\r')||(D[j]=='\n'))
		{
			return 0;
		}
		*/
		
		CHECK((enum_test(Type,String))&&((BlocStop-BlocStart+1)>(SPG_CONFIGCOMMENT-1)),"CFG_ReadBloc: Chaine trop longue",BlocStop=BlocStart+SPG_CONFIGCOMMENT-1;Type=CP_STRING; return Type);
			
	}
	return Type;
}

void SPG_CONV CFG_ReadBlocString(char* D, int BlocStart, int BlocStop, char* S, int MaxLen)
{//lit un nom de parametre
	CHECK(D==0,"CFG_ReadBlocString",return);
//	S=SPG_TypeAlloc((BlocStop-BlocStart+2),char,"CFG_ReadString");
	while((BlocStop>BlocStart)&&(D[BlocStop]==' '))
	{//enleve les caractère terminaux ' ' inutiles (ils sont acceptés dans le type string)
		BlocStop--;
	}
	strncpy(S,D+BlocStart,V_Min(BlocStop-BlocStart+1,MaxLen-1));
	S[MaxLen-1]=0;
	return;
}

void SPG_CONV CFG_ReadLineString(char* D, int& i, int Len, SPG_CONFIGUSER &u)
{//lit une valeur chaine d'un parametre de type chaine
	CHECK(D==0,"CFG_ReadLineString",return);
	int j;
	for(j=i;j<Len;j++) {if((D[j]=='\n')||(D[j]=='\r')) break;}
	int O=((D[i]==';')?0:1);//le point virgule sert à terminer un parametre chaine
	u.S=SPG_TypeAlloc((j-i+1+O),char,"CFG_ReadLineString");
	CHECK(u.S==0,"CFG_ReadLineString",return);
	u.S[0]=';';
	strncpy(u.S+O,D+i,j-i);
	u.S[(j-i+O)]=0;
	if(j==i) 
	{
		i++;
		return;
	}
	else
	{
		i=j;
		return;
	}
}

void SPG_CONV CFG_ReadCommentString(char* D, int& i, int Len, char* Comment, int MaxLen)
{//lit toute la ligne jusqu'à la fin (commentaire)
	CHECK(D==0,"CFG_ReadCommentString",return);
	int j;
	for(j=i;j<Len;j++) {if((D[j]=='\n')||(D[j]=='\r')) break;}
//	u.S=SPG_TypeAlloc((j-i+1),char,"CFG_ReadLineString");
	strncpy(Comment,D+i,V_Min(j-i,MaxLen-1));
	Comment[V_Min(j-i,MaxLen-1)]=0;
	i=j;
	return;
}

int SPG_CONV CFG_ParamFromName(SPG_CONFIGFILE& CFG, const char* ParamName)
{
	CHECK(CFG.Etat==0,"CFG_ParamFromName",return 0);
	CHECK((ParamName==0)||(ParamName[0]==0),"CFG_ParamFromName",return 0);

	for(int i=0;i<CFG.NumParams;i++)
	{
		if(enum_test(CFG.CP[i].Type,User)) continue;
		if(strncmp(CFG.CP[i].Name,ParamName,SPG_CONFIGNAME-1)==0)
		{
			return i;
		}
	}
	return 0;
}

//(*(void**)(&D.a.Var))=(void*)(localTMP.b.Var)
#define CFG_TypeConvert(TypeDest,m,D,a,S,b) {SPG_CONFIGPARAM localTMP=S; (*(void**)(&D.a.Var))=(void*)(localTMP.b.Var); D.a.Var_F=m(localTMP.b.Var_F); D.a.Min=m(localTMP.b.Min); D.a.Max=m(localTMP.b.Max); D.Type&=~CP_TYPEMASK; SPG_CFG_TYPE T=TypeDest; D.Type|=T&CP_TYPEMASK;}

int SPG_CONV CFG_Cast(SPG_CONFIGPARAM& CP, SPG_CFG_TYPE Type)
{
	if((enum_test(CP.Type,Int))&&(enum_test(Type,Int))) return -1;
	if((enum_test(CP.Type,Float))&&(enum_test(Type,Float))) return -1;
	if((enum_test(CP.Type,Double))&&(enum_test(Type,Double))) return -1;
	if((enum_test(CP.Type,String))&&(enum_test(Type,String))) return -1;
	if((enum_test(CP.Type,User))&&(enum_test(Type,User))) return -1;

	if((enum_test(CP.Type,Int))&&	(enum_test(Type,Float)))	{CFG_TypeConvert(CP_FLOAT,V_IntToFloat,CP,f,CP,i);return -1;}
	if((enum_test(CP.Type,Int))&&	(enum_test(Type,Double)))	{CFG_TypeConvert(CP_DOUBLE,V_IntToDouble,CP,d,CP,i);return -1;}

	if((enum_test(CP.Type,Float))&&	(enum_test(Type,Int)))		{CFG_TypeConvert(CP_INT,V_FloatToInt,CP,i,CP,f);return -1;}
	if((enum_test(CP.Type,Float))&&	(enum_test(Type,Double)))	{DbgCHECKTWO(1,"CFG_Cast: (CP.Type&CP_FLOAT)&&(Type&CP_DOUBLE) : using V_FloatToDouble",CP.Name);CFG_TypeConvert(CP_DOUBLE,V_FloatToDouble,CP,d,CP,f);return -1;}

	if((enum_test(CP.Type,Double))&&(enum_test(Type,Int)))		{CFG_TypeConvert(CP_INT,V_DoubleToInt,CP,i,CP,d);return -1;}
	if((enum_test(CP.Type,Double))&&(enum_test(Type,Float)))	{DbgCHECKTWO(1,"CFG_Cast: (CP.Type&CP_FLOAT)&&(Type&CP_DOUBLE) : using V_DoubleToFloat",CP.Name);CFG_TypeConvert(CP_FLOAT,V_DoubleToFloat,CP,f,CP,d);return -1;}

	return 0;
}

int SPG_CONV CFG_CreateName(SPG_CONFIGFILE& CFG, const char* ParamName, SPG_CFG_TYPE Type, const char* Comment, int Clear)
{
	CHECK(CFG.Etat==0,"CFG_CreateName",return 0);
	CHECK((ParamName==0)||(ParamName[0]==0),"CFG_CreateName",return 0);

	int i=CFG_ParamFromName(CFG,ParamName);
	if(i)
	{//le nom existe déjà, vérification et adaptation du type 
#define CurPar CFG.CP[i]
//les parametres float peuvent etre lus comme int dans le fichier s'ils n'ont pas de partie décimale mais accédés comme double par le programme
		SPG_MemSetStruct(CurPar);
		if(CFG_Cast(CurPar,Type))
		{//retourne le parametre deja existant casté dans le type demandé
		}
		else
		{
			CHECKTWO(
				((CurPar.Type&CP_TYPEMASK).e)!=((Type&CP_TYPEMASK).e),
				"CFG_CreateName: Types discordants",ParamName,{if(enum_test(CurPar.Type,User)) SPG_MemFree(CurPar.u.S);} CurPar.Type=Type;);
			//int SCur=((CurPar.Comment[0]==';')?1:0);
			//int SPar=((Comment[0]==';')?1:0);
			//deja existant: copie la valeur depuis le fichier
		}
		return i;
	}
	else
	{//création du nom
		i=CFG.NumParams;
		//sinon copie a l'envers (flush...)
		CHECKTWO(CFG.NumParams==CFG.MaxParams,"CFG_CreateName: trop de parametres",ParamName,return 0;);
		//SPG_List2S("CFG_CreateName: Creation de ",ParamName);
		if(Clear) SPG_ZeroStruct(CurPar);SPG_MemSetStruct(CurPar);
		CurPar.Type=Type;
		if(ParamName&&(ParamName!=CurPar.Name))//protege contre le passage d'arguments qui soient déjà pris dans  la structure (je ne sais plus quel était le problème que cela posait)
		{
			strncpy(CurPar.Name,ParamName,SPG_CONFIGNAME-1);
			CurPar.Name[SPG_CONFIGNAME-1]=0;
		}
		if(Comment&&(Comment!=CurPar.Comment)) 
		{
			strncpy(CurPar.Comment,Comment,SPG_CONFIGCOMMENT-1);
			CurPar.Comment[SPG_CONFIGCOMMENT-1]=0;
		}
		CFG.NumParams++;
		SPG_MemCheckStruct(CurPar,"CFG_CreateName",CFG.FileName);
		return i;
	}
#undef CurPar
}

int SPG_CONV CFG_LoadFile(SPG_CONFIGFILE& CFG, const char* Directory, const char* FileName, int WarningLevel)
{
//Parametre en cours de remplissage = le dernier de la liste
	CHECK(CFG.Etat==0,"CFG_LoadFile",return 0);
	CHECK(FileName==0,"CFG_LoadFile",return 0);
	
	SPG_ConcatPath(CFG.FileName,Directory,FileName);

#define CurPar CFG.CP[CFG.NumParams]
	int Len=0;
	char* D=(char*)SPG_LoadFileAlloc(CFG.FileName,Len,WarningLevel);
	if(D)
	{
		ReaderState RS=RS_Ready;
		int i=0;
		SPG_CFG_TYPE Type;
		int BlocStart,BlocStop;
		int istart = 0;
		while((i<Len)||((i==Len)&&(RS!=RS_Ready)))
		{
			CHECKTWO(CFG.NumParams>=CFG.MaxParams,"CFG_LoadFile: trop de parametres",CFG.FileName,break);
			switch(RS)
			{
			case(RS_Ready):
				memset(&(CurPar),0,sizeof(SPG_CONFIGPARAM));
				SPG_MemSetStruct(CurPar);
				istart=i;
				if(i==Len) 
				{
					break;
				}
				else if(((D[i]==' ')||(D[i]=='\t')||(D[i]=='\r')||(D[i]=='\n'))) {i++;}
				else if(CFG_ReadBloc(D,i,Len,BlocStart,BlocStop)==CP_STRING)
				{//nom du paramètre
					CFG_ReadBlocString(D,BlocStart,BlocStop,CurPar.Name,SPG_CONFIGNAME);
					RS=RS_ReadRVal;
				}
				else
				{
					CurPar.Type=CP_USER;
					i=istart;
					CFG_ReadLineString(D,i,Len,CurPar.u);
					CFG.NumParams++;
					RS=RS_Ready;
				}
				break;
			case(RS_ReadRVal):
				if((i<Len) && ((D[i]==' ')||(D[i]=='\t')||(D[i]=='='))) {i++;}
				else
				{
					Type=CFG_ReadBloc(D,i,Len,BlocStart,BlocStop);
					if(Type==SPG_CFG_TYPE::Null)
					{
						CurPar.Type=CP_USER; //note: warning C4701: variable locale 'istart' potentiellement non initialisée utilisée -> istartest est initialisé par case(RS_Ready) etat initial de RenderStateRS
						i=istart; //reprend la ligne du debut en tant que ligne de commentaire (prevoir plutot variable nulle/chaine nulle si on trouve au moins le signe =)
						CFG_ReadLineString(D,i,Len,CurPar.u);
						CFG.NumParams++;
						RS=RS_Ready;
						break;
					}
					else
					{
						CurPar.Type=Type|CP_HASVAL;
						if(enum_test(Type,String)) {CFG_ReadBlocString(D,BlocStart,BlocStop,CurPar.s.S_F,SPG_CONFIGCOMMENT);RS=RS_ReadComment;istart=i;}
						else if(enum_test(Type,Int)) {CurPar.i.Var_F=CF_ReadInt(D,BlocStart,BlocStop);RS=RS_ReadLPar;istart=i;}
						else if(enum_test(Type,Float)) {CurPar.f.Var_F=CF_ReadFloat(D,BlocStart,BlocStop);RS=RS_ReadLPar;istart=i;}
						else if(enum_test(Type,Double)) 
						{
							CurPar.d.Var_F=CF_ReadDouble(D,BlocStart,BlocStop);
							SPG_CFG_TYPE Dest; Dest=CP_FLOAT;
							if(BlocStop-BlocStart<=MAXFLOATSTR) CFG_Cast(CurPar,Dest);
							RS=RS_ReadLPar;istart=i;
						}
					}
				}
				break;
			case(RS_ReadMin):
				if((i<Len) && ((D[i]==' ')||(D[i]=='\t'))) {i++;}
				else
				{
					Type=CFG_ReadBloc(D,i,Len,BlocStart,BlocStop);
					if((enum_test(Type,Int))||(enum_test(Type,Float))||(enum_test(Type,Double)))
					{
						CurPar.Type|=CP_HASMIN;
						if(enum_test(CurPar.Type,Int)) {CurPar.i.Min=CF_ReadInt(D,BlocStart,BlocStop);RS=RS_ReadV;istart=i;}
						else if(enum_test(CurPar.Type,Float)) {CurPar.f.Min=CF_ReadFloat(D,BlocStart,BlocStop);RS=RS_ReadV;istart=i;}
						else if(enum_test(CurPar.Type,Double)) {CurPar.d.Min=CF_ReadDouble(D,BlocStart,BlocStop);RS=RS_ReadV;istart=i;}
					}
					else {i=istart;RS=RS_ReadComment;}
				}
				break;
			case(RS_ReadMax):
				if((i<Len) && ((D[i]==' ')||(D[i]=='\t'))) {i++;}
				else
				{
					Type=CFG_ReadBloc(D,i,Len,BlocStart,BlocStop);
					if((enum_test(Type,Int))||(enum_test(Type,Float))||(enum_test(Type,Double)))
					{
						CurPar.Type|=CP_HASMAX;
						if(enum_test(CurPar.Type,Int)) {CurPar.i.Max=CF_ReadInt(D,BlocStart,BlocStop);RS=RS_ReadRPar;istart=i;}
						else if(enum_test(CurPar.Type,Float)) {CurPar.f.Max=CF_ReadFloat(D,BlocStart,BlocStop);RS=RS_ReadRPar;istart=i;}
						else if(enum_test(CurPar.Type,Double)) {CurPar.d.Max=CF_ReadDouble(D,BlocStart,BlocStop);RS=RS_ReadRPar;istart=i;}
					}
					else {RS=RS_ReadRPar;}
				}
				break;
			case(RS_ReadLPar):
				if((i<Len) && ((D[i]==' ')||(D[i]=='\t'))) {i++;}
				else if(D[i]=='(') {i++;RS=RS_ReadMin;}
				else {istart=i;RS=RS_ReadComment;}
				break;
			case(RS_ReadRPar):
				if((i<Len) && ((D[i]==' ')||(D[i]=='\t'))) {i++;}
				else if(D[i]==')') {i++;RS=RS_ReadComment;}
				else {i=istart;RS=RS_ReadComment;}
				break;
			case(RS_ReadV):
				if((i<Len) && ((D[i]==' ')||(D[i]=='\t'))) {i++;}
				else if(D[i]==',') {i++;RS=RS_ReadMax;}
				else {i=istart;RS=RS_ReadRPar;}
				break;
			case(RS_ReadComment):
				if((i<Len) && ((D[i]==' ')||(D[i]=='\t')||(D[i]==';'))) {i++;}
				else
				{//ajoute le paramètre avec createname
					CFG_ReadCommentString(D,i,Len,CurPar.Comment,SPG_CONFIGCOMMENT);
					int CurrentIndex=CFG_CreateName(CFG,CurPar.Name,CurPar.Type,CurPar.Comment,0);
					if (CurrentIndex != CFG.NumParams - 1)
					{//copie de parametre courant dans son emplacement
						SPG_Memcpy(CFG.CP + CurrentIndex, CFG.CP + CFG.NumParams, sizeof(SPG_CONFIGPARAM));
					}
					RS=RS_Ready;
				}
				break;
			}
		}
		SPG_MemFree(D);
		return -1;
	}
	return 0;
#undef CurPar
}

int SPG_CONV CFG_Init(SPG_CONFIGFILE& CFG, const char* Directory, const char* FileName, int MaxParams, int WarningLevel)
{
	SPG_ZeroStruct(CFG); SPG_MemSetStruct(CFG);
	CHECK(MaxParams<=1,"CFG_Init",MaxParams=128);

	CFG.MaxParams=MaxParams;

	CFG.CP=SPG_TypeAlloc(CFG.MaxParams,SPG_CONFIGPARAM,"CFG_Init");

//	CFG.LoadTime=GetSystemTime(&CFG.LoadTime);

	CFG.WarningLevel=WarningLevel;

	CFG.NumParams=1;//laisse un parametre vide en position zero pour la detection d'erreur d'index

	CFG.Etat=-1;

	if(FileName)
	{//un fichier a été spécifié
		int CFG_LoadFileRetval=CFG_LoadFile(CFG,Directory,FileName,CFG.WarningLevel);
		if(CFG.WarningLevel==1) {DbgCHECKTWO(CFG_LoadFileRetval==0,"CFG_Init: Params file not found, using default values",CFG.FileName);}
	}

	SPG_MemFastCheck();
	return CFG.Etat;
}

void SPG_CONV CFG_Close(SPG_CONFIGFILE& CFG, int FlushToFile)
{
	CHECK(CFG.Etat==0,"CFG_Close",return);
#define CurPar CFG.CP[i]
	if(FlushToFile)
	{
		//CFG_ParamsFromPtr(CFG);
		CFG_FlushToFile(CFG);
	}
	for(int i=0;i<CFG.NumParams;i++)
	{
//		if(CurPar.Name) SPG_MemFree(CurPar.Name);
//		if(CurPar.Comment) SPG_MemFree(CurPar.Comment);
		if(enum_test(CurPar.Type,CP_USER))
		{
			SPG_MemFree(CurPar.u.S);
		}
	}
	SPG_MemFree(CFG.CP);
	SPG_MemCheckStruct(CFG,"CFG_Close",CFG.FileName); SPG_ZeroStruct(CFG);
	SPG_MemFastCheck();
	return;
#undef CurPar
}

int SPG_CONV CFG_FindPtr(SPG_CONFIGFILE& CFG, void* Var)
{
	CHECK(CFG.Etat==0,"CFG_FindPtr",return 0);
	for(int i=0;i<CFG.NumParams;i++)
	{//il n'y a pas de risque de dupliquer le pointer de i==0 car 
		//la premiere variable est la variable nulle 
		if(CFG.CP[i].v.Var==Var) return i;
	}
	return 0;
}

int SPG_CONV CFG_IntParam(SPG_CONFIGFILE& CFG, const char* ParamName, int* Variable, const char* Comment, int LoadImmediate, SPG_CFG_TYPE Type, int Min, int Max)
{
	CHECK(CFG.Etat==0,"CFG_IntParam",return 0);
#define CurPar CFG.CP[i]
	CHECK(Variable==0,"CFG_IntParam",return 0);
	CHECKTWO(CFG_FindPtr(CFG,Variable),"CFG_IntParam: pointeur dupliqué",ParamName,return 0);
	SPG_CFG_TYPE T; T=Type;
	int i=CFG_CreateName(CFG,ParamName,T,Comment);
	if(i)
	{
		CurPar.Type|=Type|CP_HASPTR;
		CurPar.i.Var=Variable;
		CurPar.i.Min=Min;
		CurPar.i.Max=Max;
		if(LoadImmediate)
		{
			CFG_ParamFromF(CFG,i);
		}
	}
	return i;
#undef CurPar
}

int SPG_CONV CFG_FloatParam(SPG_CONFIGFILE& CFG, const char* ParamName, float* Variable, const char* Comment, int LoadImmediate, SPG_CFG_TYPE Type, float Min, float Max)
{
	CHECK(CFG.Etat==0,"CFG_FloatParam",return 0);
#define CurPar CFG.CP[i]
	CHECK(Variable==0,"CFG_FloatParam",return 0);
	CHECKTWO(CFG_FindPtr(CFG,Variable),"CFG_FloatParam: pointeur dupliqué",ParamName,return 0);
	SPG_CFG_TYPE T; T=Type;
	int i=CFG_CreateName(CFG,ParamName,T,Comment);
	if(i)
	{
		CurPar.Type|=Type|CP_HASPTR;
		CurPar.f.Var=Variable;
		CurPar.f.Min=Min;
		CurPar.f.Max=Max;
		if(LoadImmediate)
		{
			CFG_ParamFromF(CFG,i);
		}
	}
	return i;
#undef CurPar
}

int SPG_CONV CFG_DoubleParam(SPG_CONFIGFILE& CFG, const char* ParamName, double* Variable, const char* Comment, int LoadImmediate, SPG_CFG_TYPE Type, double Min, double Max)
{
	CHECK(CFG.Etat==0,"CFG_DoubleParam",return 0);
#define CurPar CFG.CP[i]
	CHECK(Variable==0,"CFG_DoubleParam",return 0);
	CHECKTWO(CFG_FindPtr(CFG,Variable),"CFG_DoubleParam: pointeur dupliqué",ParamName,return 0);
	SPG_CFG_TYPE T; T=Type;
	int i=CFG_CreateName(CFG,ParamName,T,Comment);
	if(i)
	{
		CurPar.Type|=Type|CP_HASPTR;
		CurPar.d.Var=Variable;
		CurPar.d.Min=Min;
		CurPar.d.Max=Max;
		if(LoadImmediate)
		{
			CFG_ParamFromF(CFG,i);
		}
	}
	return i;
#undef CurPar
}

int SPG_CONV CFG_StringParam(SPG_CONFIGFILE& CFG, const char* ParamName, const char* Variable, const char* Comment, int LoadImmediate, SPG_CFG_TYPE Type)
{
	CHECK(CFG.Etat==0,"CFG_StringParam",return 0);
#define CurPar CFG.CP[i]
	CHECK(Variable==0,"CFG_StringParam",return 0);
	CHECKTWO(CFG_FindPtr(CFG,(void*)Variable),"CFG_StringParam: pointeur dupliqué",ParamName,return 0);
	SPG_CFG_TYPE T; T=Type;
	int i=CFG_CreateName(CFG,ParamName,T,Comment);
	if(i)
	{
		CurPar.Type|=Type|CP_HASPTR;
		CurPar.s.S=Variable;
		if(LoadImmediate)
		{
			CFG_ParamFromF(CFG,i);
		}
	}
	return i;
#undef CurPar
}

void SPG_CONV CFG_ParamFromF(SPG_CONFIGFILE& CFG, int i)
{
	CHECK(CFG.Etat==0,"CFG_ParamFromF",return);
	//if(CFG.FileName[0]==0) return; //il faut bien que CFG_ParamsFromF copie les valeurs lues dans la boite de dialogue (CFG_DlgCFGOK)
#define CurPar CFG.CP[i]
	if(enum_test(CurPar.Type,User)) return;
	CHECKWTHREE((CFG.WarningLevel>1),(enum_test(CurPar.Type,HasPtr))==0,"CFG_ParamFromF: Exist in file but not used",CurPar.Name,CFG.FileName,return);
	CHECKWTHREE(((CFG.WarningLevel>1)&&(CFG.FileName[0]!=0)),(enum_test(CurPar.Type,HasVal))==0,"CFG_ParamFromF: no value in CFG file",CurPar.Name,CFG.FileName,return);
	if(enum_test(CurPar.Type,Int))
	{
		CHECKTWO_ELSE(CurPar.i.Var==0,"CFG_ParamFromF",CurPar.Name,return)
		else
		{
			if(enum_test(CurPar.Type,HasMin))
			{
				CHECKTHREE(CurPar.i.Var_F<CurPar.i.Min,"CFG_ParamFromF: Parametre incorrect",CFG.FileName,CurPar.Name,CurPar.i.Var_F=CurPar.i.Min);
			}
			if(enum_test(CurPar.Type,HasMax))
			{
				CHECKTHREE(CurPar.i.Var_F>CurPar.i.Max,"CFG_ParamFromF: Parametre incorrect",CFG.FileName,CurPar.Name,CurPar.i.Var_F=CurPar.i.Max);
			}
			*CurPar.i.Var=CurPar.i.Var_F;
		}
	}
	else if(enum_test(CurPar.Type,Float))
	{
		CHECKTWO_ELSE(CurPar.f.Var==0,"CFG_ParamFromF",CurPar.Name,return)
		else
		{
			if(enum_test(CurPar.Type,HasMin))
			{
				CHECKTHREE(CurPar.f.Var_F<CurPar.f.Min,"CFG_ParamFromF: Parametre incorrect",CFG.FileName,CurPar.Name,CurPar.f.Var_F=CurPar.f.Min);
			}
			if(enum_test(CurPar.Type,HasMax))
			{
				CHECKTHREE(CurPar.f.Var_F>CurPar.f.Max,"CFG_ParamFromF: Parametre incorrect",CFG.FileName,CurPar.Name,CurPar.f.Var_F=CurPar.f.Max);
			}
			*CurPar.f.Var=CurPar.f.Var_F;
		}
	}
	else if(enum_test(CurPar.Type,Double))
	{
		CHECKTWO_ELSE(CurPar.d.Var==0,"CFG_ParamFromF",CurPar.Name,return)
		else
		{
			if(enum_test(CurPar.Type,HasMin))
			{
				CHECKTHREE(CurPar.d.Var_F<CurPar.d.Min,"CFG_ParamFromF: Parametre incorrect",CFG.FileName,CurPar.Name,CurPar.d.Var_F=CurPar.d.Min);
			}
			if(enum_test(CurPar.Type,HasMax))
			{
				CHECKTHREE(CurPar.d.Var_F>CurPar.d.Max,"CFG_ParamFromF: Parametre incorrect",CFG.FileName,CurPar.Name,CurPar.d.Var_F=CurPar.d.Max);
			}
			*CurPar.d.Var=CurPar.d.Var_F;
		}
	}
	else if(enum_test(CurPar.Type,String))
	{
		CHECKTHREE_ELSE(CurPar.s.S==0,"CFG_ParamFromF",CFG.FileName,CurPar.Name,return)
		else
		{
			CurPar.s.S_F[SPG_CONFIGCOMMENT-1]=0;
			strcpy((char* /*FDE*/)CurPar.s.S, CurPar.s.S_F);
	}
	}
	SPG_MemFastCheck();
	return;
#undef CurPar
}

//lit les valeurs du fichier et les copie sur les addresse pointées
void SPG_CONV CFG_ParamsFromF(SPG_CONFIGFILE& CFG)
{
	CHECK(CFG.Etat==0,"CFG_ParamFromF",return);
	for(int i=1;i<CFG.NumParams;i++)
	{
		CFG_ParamFromF(CFG,i);
	}
	return;
}

//lit les valeurs pointées et les mémorise
void SPG_CONV CFG_ParamsFromPtr(SPG_CONFIGFILE& CFG)
{
	CHECK(CFG.Etat==0,"CFG_ParamsFromPtr",return);
#define CurPar CFG.CP[i]
	for(int i=1;i<CFG.NumParams;i++)
	{
		if(enum_test(CurPar.Type,User)) continue;
		CHECKWTHREE((CFG.WarningLevel>1),(enum_test(CurPar.Type,HasPtr))==0,"CFG_ParamsFromPtr: Exist in file but not used",CurPar.Name,CFG.FileName,continue);
		if(enum_test(CurPar.Type,Int))
		{
			CHECKTHREE_ELSE(CurPar.i.Var==0,"CFG_ParamsFromPtr",CurPar.Name,CFG.FileName,continue)
			else
			{
				CurPar.i.Var_F=*CurPar.i.Var;
				CurPar.Type|=CP_HASVAL;
			}
		}
		else if(enum_test(CurPar.Type,Float))
		{
			CHECKTHREE_ELSE(CurPar.f.Var==0,"CFG_ParamsFromPtr",CurPar.Name,CFG.FileName,continue)
			else
			{
				CurPar.f.Var_F=*CurPar.f.Var;
				CurPar.Type|=CP_HASVAL;
			}
		}
		else if(enum_test(CurPar.Type,Double))
		{
			CHECKTHREE_ELSE(CurPar.d.Var==0,"CFG_ParamsFromPtr",CurPar.Name,CFG.FileName,continue)
			else
			{
				CurPar.d.Var_F=*CurPar.d.Var;
				CurPar.Type|=CP_HASVAL;
			}
		}
		else if(enum_test(CurPar.Type,String))
		{
			CHECKTHREE_ELSE(CurPar.s.S==0,"CFG_ParamsFromPtr",CurPar.Name,CFG.FileName,continue)
			else
			{
				strncpy(CurPar.s.S_F,CurPar.s.S,SPG_CONFIGCOMMENT-1);
				CurPar.s.S_F[SPG_CONFIGCOMMENT-1]=0;
				CurPar.Type|=CP_HASVAL;
			}
		}
	}
	return;
#undef CurPar
}

//enleve les pointeurs sur un zone mémoire donnée
void SPG_CONV CFG_RemoveRef(SPG_CONFIGFILE& CFG, void* M, int Len)
{
	CHECK(CFG.Etat==0,"CFG_ParamsFromPtr",return);
#define CurPar CFG.CP[i]
	for(int i=1;i<CFG.NumParams;i++)
	{
		if(V_IsBound(
			((intptr_t)CurPar.v.Var) , ((intptr_t)M) , ((intptr_t)M+Len)
		  ))
		{
			CurPar.v.Var=0;
			CurPar.Type&=~CP_HASPTR;
		}
	}
	return;
#undef CurPar
}

int SPG_CONV CFG_ParamVarFToString(SPG_CONFIGFILE& CFG, SPG_CONFIGPARAM& CurPar, int Format, char* OutputString, int OutputLen)
{
	CHECK(CFG.Etat==0,"CFG_ParamVarFToString",return 0);
	CHECK(OutputString==0,"CFG_ParamVarFToString",return 0);
	CHECK(OutputLen<=0,"CFG_ParamVarFToString",return 0);
	OutputString[0]=0;
	CHECK(OutputLen==1,"CFG_ParamVarFToString",return 0);

#define EndOfString(S,L) {S[L-1]=0;intptr_t count=strlen(S);S+=count;L-=count;}

	if(enum_test(CurPar.Type,Int))
	{
		CHECKTWO((enum_test(CurPar.Type,HasVal))==0,"CFG_ParamVarFToString",CurPar.Name,return 0);
		if(Format&SPG_CFG_FORMAT_TAB)
		{
			_snprintf(OutputString,OutputLen,"%s\t=\t%i",CurPar.Name,CurPar.i.Var_F);
		}
		else
		{
			_snprintf(OutputString,OutputLen,"%s=%i",CurPar.Name,CurPar.i.Var_F);
		}
		
		EndOfString(OutputString,OutputLen);if(OutputLen<=0) return 0;

		if(Format&SPG_CFG_FORMAT_MINMAX)
		{
			if((enum_test(CurPar.Type,HasMin))&&(enum_test(CurPar.Type,HasMax)))
			{
				_snprintf(OutputString,OutputLen,"\t(%i,%i)",CurPar.i.Min,CurPar.i.Max);
			}
			else if(enum_test(CurPar.Type,HasMin))
			{
				_snprintf(OutputString,OutputLen,"\t(%i)",CurPar.i.Min);
			}
		}

		EndOfString(OutputString,OutputLen);if(OutputLen<=0) return 0;

		if(Format&SPG_CFG_FORMAT_COMMENT)
		{
			int Start=CurPar.Comment[0]==';'?1:0;
			if(CurPar.Comment[Start])
			{
				_snprintf(OutputString,OutputLen,"\t;%s",CurPar.Comment+Start);
			}
		}
		//else
		//{
		//	_snprintf(OutputString,OutputLen,"\r\n");
		//}
	}
	else if(enum_test(CurPar.Type,Float))
	{
		CHECKTWO((enum_test(CurPar.Type,HasVal))==0,"CFG_ParamVarFToString",CurPar.Name,return 0);
		char CVar[32],CMin[32],CMax[32];
		CF_GetStringZ(CVar,CurPar.f.Var_F,CF_DIGITFLOAT);
		CF_GetStringZ(CMin,CurPar.f.Min,CF_DIGITFLOAT);
		CF_GetStringZ(CMax,CurPar.f.Max,CF_DIGITFLOAT);
		if(Format&SPG_CFG_FORMAT_TAB)
		{
			_snprintf(OutputString,OutputLen,"%s\t=\t%s",CurPar.Name,CVar);
		}
		else
		{
			_snprintf(OutputString,OutputLen,"%s=%s",CurPar.Name,CVar);
		}

		EndOfString(OutputString,OutputLen);if(OutputLen<=0) return 0;

		if(Format&SPG_CFG_FORMAT_MINMAX)
		{
			if((enum_test(CurPar.Type,HasMin))&&(enum_test(CurPar.Type,HasMax)))
			{
				_snprintf(OutputString,OutputLen,"\t(%s,%s)",CMin,CMax);
			}
			else if(enum_test(CurPar.Type,HasMin))
			{
				_snprintf(OutputString,OutputLen,"\t(%s)",CMin);
			}
		}

		EndOfString(OutputString,OutputLen);if(OutputLen<=0) return 0;

		if(Format&SPG_CFG_FORMAT_COMMENT)
		{
			int Start=CurPar.Comment[0]==';'?1:0;
			if(CurPar.Comment[Start])
			{
				_snprintf(OutputString,OutputLen,"\t;%s",CurPar.Comment+Start);
			}
		}
	}
	else if(enum_test(CurPar.Type,Double))
	{
		CHECKTWO((enum_test(CurPar.Type,HasVal))==0,"CFG_ParamVarFToString",CurPar.Name,return 0);
		char CVar[32],CMin[32],CMax[32];
		CF_GetStringZ(CVar,CurPar.d.Var_F,CF_DIGITDOUBLE);
		CF_GetStringZ(CMin,CurPar.d.Min,CF_DIGITDOUBLE);
		CF_GetStringZ(CMax,CurPar.d.Max,CF_DIGITDOUBLE);
		if(Format&SPG_CFG_FORMAT_TAB)
		{
			_snprintf(OutputString,OutputLen,"%s\t=\t%s",CurPar.Name,CVar);
		}
		else
		{
			_snprintf(OutputString,OutputLen,"%s=%s",CurPar.Name,CVar);
		}

		EndOfString(OutputString,OutputLen);if(OutputLen<=0) return 0;

		if(Format&SPG_CFG_FORMAT_MINMAX)
		{
			if((enum_test(CurPar.Type,HasMin))&&(enum_test(CurPar.Type,HasMax)))
			{
				_snprintf(OutputString,OutputLen,"\t(%s,%s)",CMin,CMax);
			}
			else if(enum_test(CurPar.Type,HasMin))
			{
				_snprintf(OutputString,OutputLen,"\t(%s)",CMin);
			}
		}

		EndOfString(OutputString,OutputLen);if(OutputLen<=0) return 0;

		if(Format&SPG_CFG_FORMAT_COMMENT)
		{
			int Start=CurPar.Comment[0]==';'?1:0;
			if(CurPar.Comment[Start])
			{
				_snprintf(OutputString,OutputLen,"\t;%s",CurPar.Comment+Start);
			}
		}
	}
	else if(enum_test(CurPar.Type,String))
	{
		CHECKTWO((enum_test(CurPar.Type,HasVal))==0,"CFG_ParamVarFToString",CurPar.Name,return 0);
		if(Format&SPG_CFG_FORMAT_TAB)
		{
			int Start=CurPar.Comment[0]==';'?1:0;
			if(CurPar.Comment[Start])
			{
				_snprintf(OutputString,OutputLen,"%s\t=\t%s\t;%s",CurPar.Name,CurPar.s.S_F,CurPar.Comment+Start);
			}
			else
			{
				_snprintf(OutputString,OutputLen,"%s\t=\t%s",CurPar.Name,CurPar.s.S_F);
			}
		}
		else
		{
			_snprintf(OutputString,OutputLen,"%s=%s",CurPar.Name,CurPar.s.S_F);
		}
	}
	else
	{
		if(Format&SPG_CFG_FORMAT_COMMENT)
		{
			_snprintf(OutputString,OutputLen,"%s",CurPar.u.S);
		}
	}
	OutputString[OutputLen-1]=0;
	return -1;
#undef EndOfString
}

//ecrit les valeurs mémorisées en fichier
void SPG_CONV CFG_FlushToFile(SPG_CONFIGFILE& CFG, int Format, bool UpdateFromPtr)
{
	CHECK(CFG.Etat==0,"CFG_FlushToFile",return);
	if(CFG.FileName[0]==0) return;
	
	if(UpdateFromPtr) CFG_ParamsFromPtr(CFG);

#define CurPar CFG.CP[i]
	FILE*F=fopen(CFG.FileName,"wb+");
	CHECKWTWO(CFG.WarningLevel>0,F==0,"CFG_FlushToFile: Couldn't write file ",CFG.FileName,return);
	char OutputString[2*MaxProgDir];
	for(int i=1;i<CFG.NumParams;i++)
	{
		SPG_MemCheckStruct(CurPar,"CFG_FlushToFile",CFG.FileName);
		if(CFG_ParamVarFToString(CFG,CurPar,Format,OutputString,sizeof(OutputString)))
		{
			fprintf(F,OutputString);
			fprintf(F,"\r\n");
		}
	}
	fclose(F);
	return;
#undef CurPar
}


int SPG_CONV CFG_SetIntParam(SPG_CONFIGFILE& CFG, const char* ParamName, int ParamValue, int Silent)
{
	CHECKTWO(CFG.Etat==0,"CFG_SetIntParam",ParamName,return 0);
	int i=CFG_ParamFromName(CFG,ParamName); if((i==0)&&Silent) return 0;
	CHECKTWO(i==0,"CFG_SetIntParam: Parameter not found",ParamName,return 0);
	SPG_CFG_TYPE T; T=CP_INT;
	CHECKTWO(CFG_Cast(CFG.CP[i],T)==0,"CFG_SetIntParam: Wrong type",ParamName,return 0);
	//CHECKTWO((CFG.CP[i].Type&CP_HASPTR)==0,"CFG_SetIntParam: Parameter not initialized",ParamName,return 0);
	//CHECK(CFG.CP[i].i.Var==0,"CFG_SetIntParam: Parameter error",return 0);
	if(enum_test(CFG.CP[i].Type,HasPtr)) 
	{
		*CFG.CP[i].i.Var=ParamValue;
	}
	else
	{
		CFG.CP[i].i.Var_F=ParamValue;
	}
	return i;
}

int SPG_CONV CFG_GetIntParam(SPG_CONFIGFILE& CFG, const char* ParamName, int Default, const char* Comment)
{
	CHECKTWO(CFG.Etat==0,"CFG_GetIntParam",ParamName,return 0);
	SPG_CFG_TYPE T; T=CP_INT;
	int i=CFG_ParamFromName(CFG,ParamName);
	if(i==0)
	{
		DbgCHECKTHREE((CFG.WarningLevel>1),"CFG_GetIntParam: Using default value",ParamName,CFG.FileName);
		i=CFG_CreateName(CFG,ParamName,T,Comment,1); CFG.CP[i].i.Var_F=Default; CFG.CP[i].Type|=CP_HASVAL;
	}
	else
	{
		CHECKTWO(CFG_Cast(CFG.CP[i],T)==0,"CFG_GetIntParam: Wrong type",ParamName,return Default);
	}
	if(enum_test(CFG.CP[i].Type,HasPtr)) 
	{
		return *CFG.CP[i].i.Var;
	}
	else
	{
		return CFG.CP[i].i.Var_F;
	}
}

int SPG_CONV CFG_SetFloatParam(SPG_CONFIGFILE& CFG, const char* ParamName, float ParamValue, int Silent)
{
	CHECKTWO(CFG.Etat==0,"CFG_SetFloatParam",ParamName,return 0);
	int i=CFG_ParamFromName(CFG,ParamName); if((i==0)&&Silent) return 0;
	CHECKTWO(i==0,"CFG_SetFloatParam: Parameter not found",ParamName,return 0);
	CHECKTWO(CFG_Cast(CFG.CP[i],CP_FLOAT)==0,"CFG_SetFloatParam: Wrong type",ParamName,return 0);
	//CHECKTWO((CFG.CP[i].Type&CP_HASPTR)==0,"CFG_SetFloatParam: Parameter not initialized",ParamName,return 0);
	//CHECK(CFG.CP[i].f.Var==0,"CFG_SetFloatParam: Parameter error",return 0);
	if(enum_test(CFG.CP[i].Type,HasPtr)) 
	{
		*CFG.CP[i].f.Var=ParamValue;
	}
	else
	{
		CFG.CP[i].f.Var_F=ParamValue;
	}
	return i;
}

float SPG_CONV CFG_GetFloatParam(SPG_CONFIGFILE& CFG, const char* ParamName, float Default, const char* Comment)
{
	CHECKTWO(CFG.Etat==0,"CFG_GetFloatParam",ParamName,return 0);
	SPG_CFG_TYPE T; T=CP_FLOAT;
	int i=CFG_ParamFromName(CFG,ParamName);
	if(i==0)
	{
		DbgCHECKTHREE((CFG.WarningLevel>1),"CFG_GetFloatParam: Using default value",ParamName,CFG.FileName);
		i=CFG_CreateName(CFG,ParamName,T,Comment,1); CFG.CP[i].f.Var_F=Default; CFG.CP[i].Type|=CP_HASVAL;

	}
	else
	{
		CHECKTWO(CFG_Cast(CFG.CP[i],T)==0,"CFG_GetFloatParam: Wrong type",ParamName,return Default);
	}
	if(enum_test(CFG.CP[i].Type,HasPtr)) 
	{
		return *CFG.CP[i].f.Var;
	}
	else
	{
		return CFG.CP[i].f.Var_F;
	}
}

int SPG_CONV CFG_SetDoubleParam(SPG_CONFIGFILE& CFG, const char* ParamName, double ParamValue, int Silent)
{
	CHECKTWO(CFG.Etat==0,"CFG_SetDoubleParam",ParamName,return 0);
	SPG_CFG_TYPE T; T=CP_DOUBLE;
	int i=CFG_ParamFromName(CFG,ParamName); if((i==0)&&Silent) return 0;
	CHECKTWO(i==0,"CFG_SetDoubleParam: Parameter not found",ParamName,return 0);
	CHECKTWO(CFG_Cast(CFG.CP[i],T)==0,"CFG_SetDoubleParam: Wrong type",ParamName,return 0);
	//CHECKTWO((CFG.CP[i].Type&CP_HASPTR)==0,"CFG_SetDoubleParam: Parameter not initialized",ParamName,return 0);
	//CHECK(CFG.CP[i].d.Var==0,"CFG_SetDoubleParam: Parameter error",return 0);
	if(enum_test(CFG.CP[i].Type,HasPtr)) 
	{
		*CFG.CP[i].d.Var=ParamValue;
	}
	else
	{
		CFG.CP[i].d.Var_F=ParamValue;
	}
	return i;
}

double SPG_CONV CFG_GetDoubleParam(SPG_CONFIGFILE& CFG, const char* ParamName, double Default, const char* Comment)
{
	CHECKTWO(CFG.Etat==0,"CFG_GetDoubleParam",ParamName,return 0);
	SPG_CFG_TYPE T; T=CP_DOUBLE;
	int i=CFG_ParamFromName(CFG,ParamName);
	if(i==0)
	{
		DbgCHECKTHREE((CFG.WarningLevel>1),"CFG_GetDoubleParam: Using default value",ParamName,CFG.FileName);
		i=CFG_CreateName(CFG,ParamName,T,Comment,1); CFG.CP[i].d.Var_F=Default; CFG.CP[i].Type|=CP_HASVAL;

	}
	else
	{
		CHECKTWO(CFG_Cast(CFG.CP[i],T)==0,"CFG_GetDoubleParam: Wrong type",ParamName,return Default);
	}
	if(enum_test(CFG.CP[i].Type,HasPtr)) 
	{
		return *CFG.CP[i].d.Var;
	}
	else
	{
		return CFG.CP[i].d.Var_F;
	}
}

int SPG_CONV CFG_SetStringParam(SPG_CONFIGFILE& CFG, const char* ParamName, const char* ParamValue, int Silent)
{
	CHECKTWO(CFG.Etat==0,"CFG_SetStringParam",ParamName,return 0);
	int i=CFG_ParamFromName(CFG,ParamName); if((i==0)&&Silent) return 0;
	CHECKTWO(i==0,"CFG_SetStringParam: Parameter not found",ParamName,return 0);
	CHECKTWO(CFG_Cast(CFG.CP[i],CP_STRING)==0,"CFG_SetStringParam: Wrong type",ParamName,return 0);
	//CHECKTWO((CFG.CP[i].Type&CP_HASPTR)==0,"CFG_SetStringParam: Parameter not initialized",ParamName,return 0);
	//CHECK(CFG.CP[i].s.S==0,"CFG_SetStringParam: Parameter error",return 0);
	if(enum_test(CFG.CP[i].Type,HasPtr)) 
	{
//FDE		strcpy(CFG.CP[i].s.S, ParamValue);
		CFG.CP[i].s.S = ParamValue;
	}
	else
	{
		strcpy(CFG.CP[i].s.S_F,ParamValue);
	}
	return i;
}

const char* SPG_CONV CFG_GetStringParam(SPG_CONFIGFILE& CFG, const char* ParamName, const char* Default, const char* Comment)
{
	CHECKTWO(CFG.Etat==0,"CFG_GetStringParam",ParamName,return 0);
	int i=CFG_ParamFromName(CFG,ParamName);
	if(i==0)
	{
		DbgCHECKTHREE((CFG.WarningLevel>1),"CFG_GetStringParam: Using default value",ParamName,CFG.FileName);
		i=CFG_CreateName(CFG,ParamName,CP_STRING,Comment,1); strcpy(CFG.CP[i].s.S_F,Default); CFG.CP[i].Type|=CP_HASVAL;
	}
	else
	{
		CHECKTWO(CFG_Cast(CFG.CP[i],CP_STRING)==0,"CFG_GetStringParam: Wrong type",ParamName,return Default);
	}
	if(enum_test(CFG.CP[i].Type,HasPtr)) 
	{
		return CFG.CP[i].s.S;
	}
	else
	{
		return CFG.CP[i].s.S_F;
	}
}

#endif

