
#include "SPG_General.h"

#ifdef SPG_General_USEWLTG

#include "SPG_Includes.h"
#include "SPG_SysInc.h"
//nclude <stdio.h>
//nclude <stdlib.h>
//nclude <memory.h>

LOGTIME* global_t=0;

#define MSK6BYTES 0xFFFFFFFFFFFF

//####   WLOGTIMEPAIR   ####

typedef struct
{
	int i;
	int i_prev;
	int N;
	__int64 FirstOccurenceE;
	__int64 FirstOccurenceX;
	__int64 LastOccurenceE;
	__int64 LastOccurenceX;
	__int64 MinTime;
	__int64 SumTime;
	__int64 MaxTime;
} WPAIR;// 9 x 8 = 56 bytes

typedef struct
{
	int MaxPair;
	int NumPair;
	FILE* F;
	WPAIR* P;
	LOGTIME* t;
} WLOGTIMEPAIR;

int SPG_CONV WLT_InitPairs(WLOGTIMEPAIR& WP, LOGTIME* t, char* FileName);
int SPG_CONV WLT_ClosePairs(WLOGTIMEPAIR& WP);
int SPG_CONV WLT_SavePairs(WLOGTIMEPAIR& WP);
int SPG_CONV WLT_Equal(WLOGTIMEPAIR& WP, LT_EVENT& E1, LT_EVENT& E2);
int SPG_CONV WLT_EqualUID(WLOGTIMEPAIR& WP, LT_EVENT& E1, LT_EVENT& E2);
int SPG_CONV WLT_FindOrCreate(WLOGTIMEPAIR& WP, int i, int i_prev);
void SPG_CONV WLT_AddPair(WLOGTIMEPAIR& WP, int i, int i_prev);
void SPG_CONV WLT_ScanLog(WLOGTIMEPAIR& WP);
void SPG_CONV WLT_ScanSuccessivePairs(WLOGTIMEPAIR& WP);
void SPG_CONV WLT_ScanEnterExitReentLoopPairs(WLOGTIMEPAIR& WP);

//####   WLOGTIMEPAIR   ####

#ifdef FDE
//code pour creer un logtime qui soit visible de l'exterieur
int SPG_CONV WLT_Create(WLOGTIME& wt, int MaxEvents, int MaxThreads, int MaxLog, char* Name, __int64 TickPerSec)
{
	SPG_ZeroStruct(wt);

	MaxEvents--;	for(int i=0;i<4;i++) MaxEvents|=(MaxEvents>>(1<<i));	int MskEvents=MaxEvents;	MaxEvents++;

	int MaxEventTypes=LT_UID_MSK;

	MaxLog--;	for(int i=0;i<4;i++) MaxLog|=(MaxLog>>(1<<i));	int MskLog=MaxLog;	MaxLog++;

	int szm=sizeof(LOGTIME);	int szEvents=sizeof(LT_EVENT)*MaxEvents;	int szEventTypes=sizeof(LT_EVENTTYPE)*MaxEventTypes;	int szThInfos=sizeof(LT_THREAD_INFOS)*MaxThreads;	int szLogInfos=sizeof(LT_LOG_INFOS)*MaxLog;

	int AllocSize=szm+szEvents+szEventTypes+szThInfos+szLogInfos;

	char iName[64];	int i;
	for(i=0;i<8;i++)//plusieurs programmes peuvent tourner simultanément avec le même nom et on numerote avec une limite à 8
	{
		sprintf(iName,"%s_%i",Name,i);
		wt.hMapFile = CreateFileMapping(INVALID_HANDLE_VALUE,0,PAGE_READWRITE , 0, AllocSize, fdwstring(iName)); CHECK(wt.hMapFile == NULL,"LT_Create",return 0); 
		if(wt.hMapFile) break;
		if(GetLastError() != ERROR_ALREADY_EXISTS) return 0;
	}
	if(i==8) return 0;

	wt.t=(LOGTIME*)MapViewOfFile(wt.hMapFile,FILE_MAP_ALL_ACCESS,0,0,0); CHECK(wt.t == NULL,"LT_Create",CloseHandle(wt.hMapFile);return 0); 
	CHECK(wt.t==0,"LT_Create",return 0);

	memset(wt.t,0,AllocSize);

	wt.t->szm=szm;																																																int ofs=0;
	wt.t->NumEvents=0;		wt.t->MskEvents=MskEvents;		wt.t->MaxEvents=MaxEvents;																		ofs+=wt.t->szEvents=szEvents;
	wt.t->NumEventTypes=0;														wt.t->MaxEventTypes=MaxEventTypes;		wt.t->ofsEventTypes=ofs	;	ofs+=wt.t->szEventTypes=szEventTypes;		
	wt.t->NumThInfos=0;															wt.t->MaxThInfos=MaxThreads;					wt.t->ofsThInfos=ofs;			ofs+=wt.t->szThInfos=szThInfos;					
	wt.t->NumLogInfos=0;	wt.t->MskLogInfos=MskLog;		wt.t->MaxLogInfos=MaxLog;						wt.t->ofsLogInfos=ofs;		ofs+=wt.t->szLogInfos=szLogInfos;				

	wt.t->TickPerSec=TickPerSec;

#ifdef FDE
	GetModuleFileName(GetModuleHandle(0), wt.t->ExeName, sizeof(wt.t->ExeName));
#else
	wchar_t* wExename = new wchar_t[sizeof(wt.t->ExeName)];
	GetModuleFileName(GetModuleHandle(0),wExename,sizeof(wt.t->ExeName));
	strcpy_s(wt.t->ExeName, sizeof(wt.t->ExeName), fdstring(wExename));
	delete wExename;
#endif

//efine LT_UID_NULL 0
	LT_CreateUID(wt.t,"(null)","(null)",__FUNCTION__,__FILE__,__LINE__,0,&wt);//l'UID zero est marqué différemment, on le trouve en cas d'erreur (init de l'UID qui manque)
//efine LT_UID_HIDDEN 1
	LT_CreateUID(wt.t,"(hidden)","(hidden)",__FUNCTION__,__FILE__,__LINE__,0,&wt);//l'UID 1 est marqué différemment, on le trouve pour les msg de log dont on ne peut tracer l'origine, ou pour les modules qui ne souhaitent pas apparaitre dans le log

	SPG_AddCallbackOnCheck((SPG_CHECKCALLBACK)LT_LogStrClbk,&wt);

	__int64* pTick=&wt.t->TickOrigin;
	//ecrit les 6 octets de poids faible de RDTSC
	{	__asm push eax __asm push edx __asm push ebx __asm mov ebx,pTick __asm _emit 0x0F __asm _emit 0x31 __asm mov  DWORD PTR [ebx],eax __asm mov  WORD PTR [ebx]+4,dx __asm pop ebx __asm pop edx __asm pop eax	}
	return -1;	
}
#endif //FDE

//code pour creer un logtime qui soit visible de l'exterieur
LOGTIME* SPG_CONV LT_CreateWorkingCopy(LOGTIME* t)
{
	if(t==0) return 0;
	int AllocSize=t->szm+t->szEvents+t->szEventTypes+t->szThInfos+t->szLogInfos;
	LOGTIME* W=(LOGTIME*)SPG_MemAlloc(AllocSize,"WKCY");
	memcpy(W,t,AllocSize);
	return W;
}

int SPG_CONV LT_CreateUID(LOGTIME* t, const char* Name, const char* Descr, const char* FctName, const char* SourceFile, int Line, int Flag, void* ObjectRef)
{
	if(t==0) return LT_UID_MSK;
	LT_EVENTTYPE* EventType=LT_EventTypes(t);
	int n;
	for(n=0;n<t->NumEventTypes;n++)
	{
		if(
		( strcmp(EventType[n].Name,Name) == 0 ) && //la comparaison n'inclue pas le champ description qui est parfois renseigné en cours de route
		( strcmp(EventType[n].FctName,FctName) == 0 ) &&
		( strcmp(EventType[n].SourceFile,SourceFile) == 0 ) &&
		( EventType[n].Line==Line ) &&
		( EventType[n].ObjectRef==ObjectRef ) ) return n;
	}
	n=t->NumEventTypes;
	t->NumEventTypes++;
	t->NumEventTypes&=LT_UID_MSK;
	if(Name) strncpy(EventType[n].Name,Name,sizeof(EventType[n].Name)-1);
	if(Descr) strncpy(EventType[n].Descr,Descr,sizeof(EventType[n].Descr)-1);
	if(FctName) strncpy(EventType[n].FctName,FctName,sizeof(EventType[n].FctName)-1);
	if(SourceFile) strncpy(EventType[n].SourceFile,SourceFile,sizeof(EventType[n].SourceFile)-1);
	EventType[n].Line=Line;
	EventType[n].ObjectRef=ObjectRef;
	return n;
}

int SPG_CONV LT_CreateUID(const char* Name, const char* Descr, const char* FctName, const char* SourceFile, int Line, int Flag, void* ObjectRef)
{
	return LT_CreateUID(global_t,Name,Descr,FctName,SourceFile,Line,Flag,ObjectRef);
}

void SPG_CONV WLT_SetGlobalRef(WLOGTIME& wt)
{
	global_t=wt.t;//il est prêt, le pointer global pointe dessus
}
/*
#ifdef DebugList
void SPG_CONV WLT_SetGlobalCheckClbkHook(WLOGTIME& wt)
{
	wt.CHECK_Callback=Global.CHECK_Callback;
	wt.CHECK_User=Global.CHECK_User;
	Global.CHECK_Callback=(SPG_CHECKCALLBACK)LT_LogStrClbk;
	Global.CHECK_User=&wt;
}

void SPG_CONV WLT_ClearGlobalCheckClbkHook(WLOGTIME& wt)
{
	CHECK_ELSE(Global.CHECK_Callback!=(SPG_CHECKCALLBACK)LT_LogStrClbk,"LT_ClearGlobalCheckClbkHook",return)
	{
		Global.CHECK_Callback=wt.CHECK_Callback;//restore la callback originale
		Global.CHECK_User=wt.CHECK_User;//restore la callback originale
	}
}
#endif
*/
void SPG_CONV WLT_ClearGlobalRef(WLOGTIME& wt)
{
	CHECK(global_t!=wt.t,"LT_ClearGlobalRef",return);
	global_t=0;
}

void SPG_CONV WLT_Close(WLOGTIME& wt)
{
	CHECK(global_t==wt.t,"WLT_Close",WLT_ClearGlobalRef(wt));
	SPG_RemoveCallbackOnCheck((SPG_CHECKCALLBACK)LT_LogStrClbk,&wt);
	//CHECK(Global.CHECK_Callback==(SPG_CHECKCALLBACK)LT_LogStrClbk,"WLT_Close",WLT_ClearGlobalCheckClbkHook(wt))
	//WLT_ClearGlobalCheckClbkHook(wt);
	//WLT_ClearGlobalRef(wt);
	if(wt.t!=0) UnmapViewOfFile(wt.t);
	if(wt.hMapFile!=0) CloseHandle(wt.hMapFile);
	SPG_ZeroStruct(wt);
	return; 
}

#ifdef FDE
void SPG_CONV LT_LogStr(WORD uid, char* LogStr)
{	
	LOGTIME* t=global_t;
	if(t) 
	{
		//global_t=0;

		LT_EVENT* E = LT_Events(t) + ( (t->NumEvents++) & t->MskEvents );
		E->UID=uid|LT_STRLOG_FLAG;
		DbgCHECK((E->UID&LT_FLAG_MSK)!=LT_STRLOG_FLAG,"LT_LogStr");
		E->LogStrIndex=LT_StoreStr(t,LogStr);
		{	
			__asm push eax __asm push edx __asm push ebx __asm mov ebx,E  __asm _emit 0x0F __asm _emit 0x31
			__asm mov  dword ptr [ebx]LT_EVENT.Tick,eax __asm mov  word ptr [ebx]LT_EVENT.Tick+4,dx
			__asm mov eax,dword ptr fs:[0x18] __asm mov eax,dword ptr [eax+0x24] __asm mov dword ptr [ebx]LT_EVENT.TID,eax
			__asm pop ebx __asm pop edx __asm pop eax
		}

		//global_t=t;
	}
	return;
}
#endif	//FDE

void SPG_CONV LT_LogStrClbk(void* User, char* Str)
{
	if(User==0) return;
	WLOGTIME& wt=*(WLOGTIME*)User;
//FDE 	LT_LogStr(LT_STRLOG_FLAG,Str);
	if(wt.CHECK_Callback!=0)
	{
		wt.CHECK_Callback(wt.CHECK_User,Str);
	}
}

void SPG_CONV LT_RegisterThreadName(char* threadName, DWORD ThreadId)
{
	if(global_t==0) return;
	LT_THREAD_INFOS* thn=LT_ThInfos(global_t);
	if(ThreadId==0) ThreadId=GetCurrentThreadId();
	int i;	for(i=0;i<global_t->NumThInfos;i++)	{	if(thn[i].ThreadId==ThreadId) break;	}
	int w=i++;
	thn[w].ThreadId=ThreadId;
	thn[w].Status=1;
	strncpy(thn[w].Name,threadName,LTG_THREAD_NAME-1);
	global_t->NumThInfos=V_Min(w+1,global_t->MaxThInfos);
	return;
}

void SPG_CONV LT_UnRegisterThreadName(char* threadName, DWORD ThreadId)
{
	if(global_t==0) return;
	LT_THREAD_INFOS* thn=LT_ThInfos(global_t);
	if(ThreadId==0) ThreadId=GetCurrentThreadId();
	int i;	for(i=0;i<global_t->NumThInfos;i++)	{	if(thn[i].ThreadId==ThreadId) thn[i].Status=0;	}
	return;
}

const char* SPG_CONV LT_GetThreadName(LOGTIME* t, DWORD ThreadId)
{
	LT_THREAD_INFOS* thn=LT_ThInfos(t);
	if(ThreadId==0) ThreadId=GetCurrentThreadId();
	int i;	for(i=0;i<t->NumThInfos;i++)	{	if(thn[i].ThreadId==ThreadId) return thn[i].Name;	}
	//LT_RegisterThreadName("Unknown@GetThreadName");
	return "unknown";//thi.thn[thi.NumItems-1].Name;
}

const char* SPG_CONV LT_GetThreadName(DWORD ThreadId)
{
	if(global_t==0) return "t=0";
	return LT_GetThreadName(global_t,ThreadId);
}

int SPG_CONV LT_StoreStr(LOGTIME* t, char* LogStr)
{
	CHECK(LogStr==0,"LT_StoreStr",return 0);
	int n=t->NumLogInfos++;
	LT_LOG_INFOS& lgn=LT_LogInfo(t,n);
	//strncpy(lsi.LogStrItem[n&LTG_LOGSTR_MSKITEMS].LogStr,LogStr,LTG_LOGSTR_LEN-1);
	int c;
	for(c=0;c<LTG_LOGSTR_LEN-1;c++)
	{
		char k=LogStr[c];
		if(k==0) break;
		if(k=='\r') k=' ';
		if(k=='\n') k=' ';
		if(k=='\t') k=' ';
		lgn.LogStr[c]=k;
	}
	lgn.LogStr[c]=0;
	for(int m=V_Max(n-32,0);m<n;m++) //boucle pourrie lente et qui ne respecte pas l'incrementation atomique mais qui eveite que les repetitions de messages ne poussent les anciens messages hors du buffer
	{
		if(strcmp(lgn.LogStr,LT_LogInfo(t,m).LogStr)==0) {n=m;t->NumLogInfos--;}
	}
	return n;
}

const char* SPG_CONV LT_GetStr(LOGTIME* t, int StrIndex)
{
		const char* LogStr="Outdated msg";
		if(V_IsBound(StrIndex,t->NumLogInfos-t->MskLogInfos,t->NumLogInfos)) LogStr=LT_LogInfo(t,StrIndex).LogStr;
		return LogStr;
}

//code pour afficher un logtime qui vient d'un programme externe
int SPG_CONV WLT_Import(WLOGTIME& wt, char* Name)
{
	SPG_ZeroStruct(wt);
	char iName[64];
	int i;
	for(i=0;i<8;i++)
	{
		sprintf(iName,"%s_%i",Name,i);
		if(wt.hMapFile) break;
		wt.hMapFile = OpenFileMapping(FILE_MAP_ALL_ACCESS,0,fdwstring(iName)); CHECK(wt.hMapFile == NULL,"LTG_Import",return 0); 
	}
	if(i==8) return 0;
	wt.t = (LOGTIME*)MapViewOfFile(wt.hMapFile,FILE_MAP_ALL_ACCESS,0,0,0); CHECK(wt.t == NULL,"LTG_Import",CloseHandle(wt.t);return 0); 
	CHECK(wt.t==0,"LTG_Import",return 0);
	return -1;
}

void SPG_CONV WLT_Discard(WLOGTIME& wt)
{
	if(wt.t!=0) UnmapViewOfFile(wt.t);
	if(wt.hMapFile!=0) CloseHandle(wt.hMapFile);
	SPG_ZeroStruct(wt);
	return;
}

int sprintEventHeader(LOGTIME* t, char* S)
{
	return sprintf(S,"Time(ms)\tTID\tTName\tFlag\tUserData\t");
}

//le champ flag est paddé à 5 caractères d'ou le %.5s #WF "  " et les espaces apres FREE et LOG

#define EXCASES SICCASES(ENTER) SICCASES(EXIT) GCASE(REENT) GCASE(LOOP)  GCASE(ENTERIDLE) GCASE(EXITIDLE)
#define SICCASES(WF) GCASE(WF) GCASE(WF##I) GCASE(WF##C)
#define GCASE(WF)	case	LT_##WF##_FLAG:																\
										{																															\
											return sprintf(S,"%0.6f\t"	"%04X\t%s\t"										"%.5s\t"	"%X\t"	,	\
																	1000*T,		E.TID,	LT_GetThreadName(t, E.TID),	#WF "  ", E.UserData);	\
										}


int sprintEvent(LOGTIME* t, LT_EVENT& E, char* S)
{
	__int64 Tick=*(__int64*)E.Tick; Tick=(Tick-t->TickOrigin)&MSK6BYTES; double T = ((double)Tick)/((double)t->TickPerSec);
	//char* Msg=CHK_ExpandAndConvertToString(GCASE(ENTER));
	switch(E.UID&LT_FLAG_MSK)
	{
	case	LT_ALLOC_FLAG	:
		{
			return sprintf(S,"%0.6f\t"					"%04X\t%s\t"													"ALLOC\t"	"%X\t"	,
										1000*T,				E.TID,	LT_GetThreadName(t, E.TID),						E.MemDelta	);
		}
	case	LT_FREE_FLAG		:
		{
			return sprintf(S,"%0.6f\t"					"%04X\t%s\t"													"FREE \t"	"%X\t"	,
										1000*T,				E.TID,	LT_GetThreadName(t, E.TID),					E.MemDelta	);
		}
	case	LT_STRLOG_FLAG	:
		{
			return sprintf(S,"%0.6f\t"					"%04X\t%s\t"													"LOG  \t"	"%s\t"	,
										1000*T,				E.TID,	LT_GetThreadName(t, E.TID),					LT_GetStr(t,E.LogStrIndex));
		}
	EXCASES
	default	:
		{
			return sprintf(S,"%0.6f\t"					"%04X\t%s\t"												"%04X \t"	"%X\t"	,
									1000*T,				E.TID,	LT_GetThreadName(t, E.TID),	E.UID,		E.UserData	);
		}
	}
}

int sprintTypeHeader(LOGTIME* t, char* S)
{
	return sprintf(S,"Name\tDescr\tFct\tSource\tLine\tFlag\tObjectRef\t");
}
int sprintType(LOGTIME* t, LT_EVENTTYPE& E, char* S)
{
	return sprintf(S,"%s\t"     "%s\t"     "%s\t"         "%s\t"        "%i\t"  "%08X\t"  "%08X\t",
							  E.Name,E.Descr,E.FctName,E.SourceFile,E.Line,E.Flag,*(DWORD*)&E.ObjectRef);
}

#ifdef FDE
void SPG_CONV LT_Clear(LOGTIME* t)
{
	if(t==0) return;
	t->NumEvents=0;
	//t->NumLogInfos=0;
	__int64* pTick=&t->TickOrigin;
	//ecrit les 6 octets de poids faible de RDTSC
	{	__asm push eax __asm push edx __asm push ebx __asm mov ebx,pTick __asm _emit 0x0F __asm _emit 0x31 __asm mov  DWORD PTR [ebx],eax __asm mov  WORD PTR [ebx]+4,dx __asm pop ebx __asm pop edx __asm pop eax	}
	return;
}

void SPG_CONV LT_Clear()
{
	if(global_t==0) return;
	LT_Clear(global_t);
	return;
}
#endif

int SPG_CONV LT_Save_Binary(LOGTIME* t, char* File)
{
	if(t==0) return 0;
	int AllocSize=t->szm+t->szEvents+t->szEventTypes;
	FILE* F=fopen(File,"wb+");
	if(F==0) return 0;
	fwrite(F,AllocSize,1,F);
	fclose(F);
	return -1;
}

int SPG_CONV LT_Save_TXT(LOGTIME* t, char* File)
{
	if(t==0) return 0;
	LT_EVENT* Events=LT_Events(t);
	LT_EVENTTYPE* EventType=LT_EventTypes(t);
	FILE* F=fopen(File,"wb+");
	if(F==0) return 0;
	char* SLine=SPG_TypeAlloc(16384,char,"LT_Save");
	char* S=SLine;
	S+=sprintEventHeader(t,S);					S+=sprintTypeHeader(t,S);					S+=sprintf(S,"\r\n");					fprintf(F,SLine);
	for(int i=t->NumEvents-1;i>=t->NumEvents-V_Min(t->NumEvents,t->MskEvents);i--)
	{//par ordre chronologique decroissant

		char* S=SLine;

		LT_EVENT& E=Events[i&t->MskEvents];
		int uid=E.UID&LT_UID_MSK;

		if(uid==0) continue;

		if(V_IsBound(uid,0,t->NumEventTypes)) 	{	S+=sprintEvent(t,E,S);			S+=sprintType(t,EventType[uid],S);		}
		else																{	S+=sprintEvent(t,E,S);			S+=sprintf(S,"Error\tError\t");				}
		S+=sprintf(S,"\r\n");			fprintf(F,SLine);
	}
	SPG_MemFree(SLine);
	fclose(F);

	/*
	WLOGTIMEPAIR WP;

	{	char File_L[512]; strcpy(File_L,File); SPG_SetExtens(File_L,"_Log.txt");
	if(WLT_InitPairs(WP,t,File_L))		{	WLT_ScanLog(WP);																				WLT_ClosePairs(WP);	}	}

	{	char File_SC[512]; strcpy(File_SC,File); SPG_SetExtens(File_SC,"_Sequential.txt");
	if(WLT_InitPairs(WP,t,File_SC))	{	WLT_ScanSuccessivePairs(WP);						WLT_SavePairs(WP);	WLT_ClosePairs(WP);	}	}

	{	char File_N[512]; strcpy(File_N,File); SPG_SetExtens(File_N,"_Stacked.txt");
	if(WLT_InitPairs(WP,t,File_N))	{	WLT_ScanEnterExitReentLoopPairs(WP);		WLT_SavePairs(WP);	WLT_ClosePairs(WP);	}	}
	*/

	return -1;
}

int SPG_CONV LT_Save_TXT(LOGTIME* t, char* Path, char* File)
{
	char F[MaxProgDir];
	SPG_ConcatPath(F,Path,File);
	return LT_Save_TXT(t,F);
}


//SPG_TREE* SPG_CONV LT_Tree(LOGTIME* t)
//{
//	if(t==0) return;
//	LT_EVENTTYPE* EventType=(LT_EVENTTYPE*)((char*)t+t->szm+t->szEvents);
//
//	STREE_Init(t->Msk*sizeof(LT_EVENT));
//	SPG_TNODE* n=STREE_Root(t);
//	for(int i=t->NumEvents-1;i>=t->NumEvents-V_Min(t->NumEvents,t->Msk);i--)
//	{//par ordre chronologique decroissant
//		LT_EVENT& E=t->Event[i&t->Msk];
//		int uid=E.UID&LT_UID_MSK;
//		switch(uid)
//		{
//		case LT_ENTER_FLAG:
//		case LT_ENTERI_FLAG:
//		case LT_ENTERC_FLAG:
//			n=STREE_CreateUpRight(t,n,E,sizeof(E));
//			break;
//		case LT_EXIT_FLAG:
//		case LT_EXITI_FLAG:
//		case LT_EXITC_FLAG:
//			n=STREE_CreateDnLeft(t,n,E,sizeof(E));
//			break;
//		default:
//			n=STREE_CreateUpLeft(t,n,E,sizeof(E));
//			break;
//		}
//	}
//}


DWORD SPG_CONV LT_Color(int uid, DWORD UserData)
{
	int n=uid+(uid<<5)+UserData+(UserData<<3)+(UserData<<5)+(UserData>>4)+(UserData>>6)+(UserData>>17);
	DWORD Coul=0;
	int nchannels=3;//B V R 
	int channelwidth=8;
	for(int depth=1;depth<channelwidth;depth++)//ne remplis pas le bit de poids fort pour rester dans des couleurs sombres
	{
		for(int nchannel=0;nchannel<3;nchannel++)
		{
			//src
			if(n&(1<<(nchannel+depth*nchannels))) Coul|=1<<(channelwidth-1-depth+channelwidth*nchannel);
		}
	}
	return Coul;
}

#ifdef FDE
void SPG_CONV LT_Display(LOGTIME* t, G_Ecran& Ecran, C_Lib& CL)
{
	DWORD Color=0xD0D0D0;
	LT_EVENTTYPE* EventType=(LT_EVENTTYPE*)((char*)t+t->szm+t->szEvents);
	G_DrawRect(Ecran,0,0,G_SizeX(Ecran),G_SizeY(Ecran),Color);
	int AbsShift=G_SizeX(Ecran)/16;
	int i; int y;
	//char* SLine=SPG_TypeAlloc(16384,char,"LT_Display");
	SPG_ArrayStackAlloc(char,SLine,4096);
	for(	y=0, i=t->NumEvents-1;		(y<G_SizeY(Ecran)/CL.SpaceY/2)&&(i>=t->NumEvents-V_Min(t->NumEvents,t->MskEvents));	y++,i-- 	)
	{
		char* S=SLine;
		LT_EVENT& E=LT_Event(t,i);
		int uid=E.UID&LT_UID_MSK;
		int flag=E.UID&LT_FLAG_MSK;

		if(( flag == LT_EXIT_FLAG)||
		   ( flag == LT_EXITI_FLAG)||
		   ( flag == LT_EXITC_FLAG)) AbsShift+=3;

		if(V_IsBound(uid,0,t->NumEventTypes))	{	S+=sprintEvent(t,E,S);	S+=sprintType(t,EventType[uid],S);	}
		else																{	S+=sprintEvent(t,E,S);	S+=sprintf(S,"Error\tError\t");			}

		SPG_ArrayStackCheck(SLine);
		DWORD C=LT_Color(uid, E.UserData);
		C_PrintUni(Ecran,AbsShift,y*CL.SpaceY,SLine,CL,0,C);

		if(( flag ==LT_ENTER_FLAG )||
		   ( flag == LT_ENTERI_FLAG)||
		   ( flag == LT_ENTERC_FLAG)) AbsShift-=3;
	}

	for(	y=0, i=t->NumLogInfos-1;		(y<G_SizeY(Ecran)/CL.SpaceY/2)&&(i>=t->NumLogInfos-V_Min(t->NumLogInfos,t->MskLogInfos));	y++,i-- 	)
	{
		C_PrintUni(Ecran,0,G_SizeY(Ecran)/2+y*CL.SpaceY,LT_LogInfo(t,i).LogStr,CL,0);
	}
//	SPG_MemFree(SLine);
}
#endif //FDE


int SPG_CONV WLT_InitPairs(WLOGTIMEPAIR& WP, LOGTIME* t, char* FileName)
{
	SPG_ZeroStruct(WP);
	if(t==0) return 0;
	WP.F=fopen(FileName,"wb+");
	if(WP.F==0) return 0;
	WP.t=LT_CreateWorkingCopy(t);
	WP.MaxPair=V_Min(WP.t->NumEvents,V_Min(1024*1024 , WP.t->NumEventTypes*WP.t->NumEventTypes*8 )); //64Mo max -> 1024*1024
	WP.P=SPG_TypeAlloc(WP.MaxPair,WPAIR,"LT_InitPairs");
	return -1;
}

int SPG_CONV WLT_ClosePairs(WLOGTIMEPAIR& WP)
{
	fclose(WP.F);
	SPG_MemFree(WP.t);
	SPG_MemFree(WP.P);
	SPG_ZeroStruct(WP);
	return -1;
}

#define CASEFLAG(v) case LT_##v##_FLAG: return #v

const char* WLT_FlagAsString(int Flag)
{
	switch(Flag&=LT_FLAG_MSK)
	{
	CASEFLAG(ENTERIDLE   );
	CASEFLAG(EXITIDLE       );
	CASEFLAG(ENTER   );
	CASEFLAG(EXIT       );
	CASEFLAG(REENT   );
	CASEFLAG(LOOP    );
	CASEFLAG(ENTERI  );
	CASEFLAG(EXITI      );
	CASEFLAG(ENTERC );
	CASEFLAG(EXITC     );
	CASEFLAG(ALLOC   );
	CASEFLAG(FREE      );
	CASEFLAG(STRLOG );
	default: return "Unknown";
	}
}

int SPG_CONV WLT_SavePairs(WLOGTIMEPAIR& WP)
{
	FILE* F=WP.F;
	if(F==0) return 0;
	char* SLine=SPG_TypeAlloc(16384,char,"WLT_SavePairs");

	{
		char* S=SLine;
		S+=sprintf(S,"Thread:NameDescr:Data:UserData\t");
		S+=sprintf(S,"Thread:NameDescr:Data:UserData\t");

		S+=sprintf(S,"N\tSum\tMin\tMean\tMax\t");
		S+=sprintf(S,"FE\tFX\tLE\tLX\t");

		S+=sprintTypeHeader(WP.t,S);
		S+=sprintTypeHeader(WP.t,S);
		S+=sprintf(S,"\r\n");
		fprintf(F,SLine);
	}

	LT_EVENT* Events=LT_Events(WP.t);
	LT_EVENTTYPE* EventType=LT_EventTypes(WP.t);
	for(int p=0;p<WP.NumPair;p++)
	{
		char* S=SLine;
		LT_EVENT& VE=Events[WP.P[p].i&WP.t->MskEvents];
		LT_EVENT& VF=Events[WP.P[p].i_prev&WP.t->MskEvents];
		int uid=VE.UID;
		int uid_prev=VF.UID;
		LT_EVENTTYPE& TE=EventType[VE.UID&LT_UID_MSK];
		LT_EVENTTYPE& TF=EventType[VF.UID&LT_UID_MSK];

		S+=sprintf(S,"[%s]%s(%s):%s\t",	LT_GetThreadName(WP.t,VE.TID),	TE.Name,	TE.Descr,	WLT_FlagAsString(VE.UID));
		S+=sprintf(S,"[%s]%s(%s):%s\t",	LT_GetThreadName(WP.t,VF.TID),	TF.Name,	TF.Descr,	WLT_FlagAsString(VF.UID));

		S+=sprintf(S,"%i\t%0.6f\t%0.6f\t%0.6f\t%0.6f\t",	WP.P[p].N,
																							1000*(double)WP.P[p].SumTime/WP.t->TickPerSec,
																							1000*(double)WP.P[p].MinTime/WP.t->TickPerSec,
																							1000*(double)WP.P[p].SumTime/WP.P[p].N/WP.t->TickPerSec,
																							1000*(double)WP.P[p].MaxTime/WP.t->TickPerSec); //manque % of total
		S+=sprintf(S,"%0.6f\t%0.6f\t%0.6f\t%0.6f\t",			1000*(double)WP.P[p].FirstOccurenceE/WP.t->TickPerSec,
																							1000*(double)WP.P[p].FirstOccurenceX/WP.t->TickPerSec,
																							1000*(double)WP.P[p].LastOccurenceE/WP.t->TickPerSec,
																							1000*(double)WP.P[p].LastOccurenceX/WP.t->TickPerSec);

		S+=sprintType(WP.t,TE,S);
		S+=sprintType(WP.t,TF,S);
		S+=sprintf(S,"\r\n");
		fprintf(F,SLine);
	}
	SPG_MemFree(SLine);
	return -1;
}

int SPG_CONV WLT_Equal(WLOGTIMEPAIR& WP, LT_EVENT& E1, LT_EVENT& E2)
{
	return (E1.UID==E2.UID) && (E1.TID==E2.TID) && (E1.UserData==E2.UserData);
}

int SPG_CONV WLT_EqualNoUserData(WLOGTIMEPAIR& WP, LT_EVENT& E1, LT_EVENT& E2)
{
	return (E1.UID==E2.UID) && (E1.TID==E2.TID);
}

int SPG_CONV WLT_EqualUID(WLOGTIMEPAIR& WP, LT_EVENT& E1, LT_EVENT& E2)
{
	return ( (E1.UID&LT_UID_MSK) == (E2.UID&LT_UID_MSK) ) && ( E1.TID == E2.TID ) && ( E1.UserData == E2.UserData );
}

int SPG_CONV WLT_EqualUIDNoUserData(WLOGTIMEPAIR& WP, LT_EVENT& E1, LT_EVENT& E2)
{
	return ( (E1.UID&LT_UID_MSK) == (E2.UID&LT_UID_MSK) ) && ( E1.TID == E2.TID);
}

int SPG_CONV WLT_IsN(int u)		{	return ( u == LT_ENTER_FLAG ) || ( u == LT_EXIT_FLAG ) || ( u == LT_REENT_FLAG ) || ( u == LT_LOOP_FLAG );	}

int SPG_CONV WLT_IsI(int u)			{	return ( u == LT_ENTERI_FLAG ) || ( u == LT_EXITI_FLAG );	}

int SPG_CONV WLT_IsC(int u)		{	return ( u == LT_ENTERC_FLAG ) || ( u == LT_EXITC_FLAG );	}

int SPG_CONV WLT_EqualFlagCategory(WLOGTIMEPAIR& WP, LT_EVENT& E1, LT_EVENT& E2)
{
	int E=E1.UID&LT_FLAG_MSK; int F=E2.UID&LT_FLAG_MSK;
	if(WLT_IsN(E)&&WLT_IsN(F)) return -1;		if(WLT_IsI(E)&&WLT_IsI(F)) return -1;		if(WLT_IsC(E)&&WLT_IsC(F)) return -1;
	return 0;
}

int SPG_CONV WLT_FindOrCreate(WLOGTIMEPAIR& WP, int i, int i_prev)
{
	int p;
	for(p=0;p<WP.NumPair;p++)
	{
		if( WLT_EqualNoUserData(WP,LT_Event(WP.t,WP.P[p].i),LT_Event(WP.t,i) ) &&
			WLT_EqualNoUserData(WP,LT_Event(WP.t,WP.P[p].i_prev),LT_Event(WP.t,i_prev) ) ) return p;
	}
	if(WP.NumPair==WP.MaxPair) return -1;//full
	return WP.NumPair++;
}

void SPG_CONV WLT_AddPair(WLOGTIMEPAIR& WP, int i, int i_prev)
{
	int p=WLT_FindOrCreate(WP,i,i_prev);
	CHECK(p==-1,"LT_AddPair",return);

	__int64 Tick_next = MSK6BYTES & *(__int64*)LT_Event(WP.t,i).Tick;
	__int64 Tick_prev = MSK6BYTES & *(__int64*)LT_Event(WP.t,i_prev).Tick;
	__int64 Tick=Tick_next-Tick_prev;
	//Tick=(Tick-t.TickOrigin)&MSK6BYTES; double T = ((double)Tick)/((double)t.TickPerSec);
	if(WP.P[p].N==0)
	{
		WP.P[p].i=i;
		WP.P[p].i_prev=i_prev;
		WP.P[p].LastOccurenceE=WP.P[p].FirstOccurenceE=Tick_prev-WP.t->TickOrigin;
		WP.P[p].LastOccurenceX=WP.P[p].FirstOccurenceX=Tick_next-WP.t->TickOrigin;
		WP.P[p].MinTime=Tick;
		WP.P[p].SumTime=Tick;
		WP.P[p].MaxTime=Tick;
	}
	else
	{
		WP.P[p].LastOccurenceE=Tick_prev-WP.t->TickOrigin;
		WP.P[p].LastOccurenceX=Tick_next-WP.t->TickOrigin;
		WP.P[p].MinTime=V_Min(WP.P[p].MinTime,Tick);
		WP.P[p].SumTime+=Tick;
		WP.P[p].MaxTime=V_Max(WP.P[p].MaxTime,Tick);
	}
	WP.P[p].N++;
	return;
}

void SPG_CONV WLT_ScanLog(WLOGTIMEPAIR& WP)
{
	LOGTIME* t=WP.t;
	LT_EVENT* Events=LT_Events(t);
	LT_EVENTTYPE* EventTypes=LT_EventTypes(t);
	char SLine[1024];
	for(int i=t->NumEvents-1;i>=t->NumEvents-V_Min(t->NumEvents,t->MskEvents);i--)
	{//par ordre chronologique decroissant
		LT_EVENT& E=Events[i&t->MskEvents];
		if(	((E.UID&LT_FLAG_MSK)==LT_STRLOG_FLAG) )
		{

			char* S=SLine;
			S+=sprintEvent(t,E,S);			S+=sprintType(t,EventTypes[E.UID&LT_UID_MSK],S);			S+=sprintf(S,"\r\n");
			fprintf(WP.F,SLine);
		}
	}
}

void SPG_CONV WLT_ScanSuccessivePairs(WLOGTIMEPAIR& WP)
{
	LOGTIME* t=WP.t;
	LT_EVENT* Events=LT_Events(t);
	LT_EVENTTYPE* EventTypes=LT_EventTypes(t);
	int i=t->NumEvents-1;
	for(int i_prev=i-1;i_prev>=t->NumEvents-V_Min(t->NumEvents,t->MskEvents);i_prev--)
	{//par ordre chronologique decroissant
		LT_EVENT& E=Events[i_prev&t->MskEvents];
		if(	((E.UID&LT_FLAG_MSK)!=LT_STRLOG_FLAG)&&
			((E.UID&LT_FLAG_MSK)!=LT_ALLOC_FLAG)&&
			((E.UID&LT_FLAG_MSK)!=LT_FREE_FLAG))
		{
			WLT_AddPair(WP,i,i_prev);
			i=i_prev;
		}
	}
}

void SPG_CONV WLT_ScanEnterExitReentLoopPairs(WLOGTIMEPAIR& WP)
{
	LOGTIME* t=WP.t;
	LT_EVENT* Events=LT_Events(t);
	LT_EVENTTYPE* EventTypes=LT_EventTypes(t);
	//int i=t->NumEvents-1;
	for(int i_prev=t->NumEvents-2;i_prev>=t->NumEvents-V_Min(t->NumEvents,t->MskEvents);i_prev--)
	{//par ordre chronologique decroissant
		LT_EVENT& E=Events[i_prev&t->MskEvents];
		if(	((E.UID&LT_FLAG_MSK)!=LT_STRLOG_FLAG)&&
			((E.UID&LT_FLAG_MSK)!=LT_ALLOC_FLAG)&&
			((E.UID&LT_FLAG_MSK)!=LT_FREE_FLAG)&&
			((E.UID&LT_FLAG_MSK)!=LT_EXIT_FLAG)&&
			((E.UID&LT_FLAG_MSK)!=LT_EXITI_FLAG)&&
			((E.UID&LT_FLAG_MSK)!=LT_EXITC_FLAG) )
		{//pour tous les EXIT/REENT/LOOP
			for(int j=i_prev+1;j<t->NumEvents-1;j++)
			{
				LT_EVENT& F=Events[j&t->MskEvents];
				if(WLT_EqualUIDNoUserData(WP,E,F))
				{//former les paires
					if(WLT_EqualFlagCategory(WP,E,F)) WLT_AddPair(WP,j,i_prev);
				}
				if(WLT_EqualUIDNoUserData(WP,E,F))
				{//stopper sur un EXIT, ENTER (signe d'une recursion ou d'un manque de EXIT)
					if(	((F.UID&LT_FLAG_MSK)==LT_EXIT_FLAG)||
						((F.UID&LT_FLAG_MSK)==LT_EXITI_FLAG)||
						((F.UID&LT_FLAG_MSK)==LT_EXITC_FLAG) )
					{
							break;
					}

					if(	((F.UID&LT_FLAG_MSK)==LT_ENTER_FLAG)||
						((F.UID&LT_FLAG_MSK)==LT_ENTERI_FLAG)||
						((F.UID&LT_FLAG_MSK)==LT_ENTERC_FLAG) )
					{
							//DbgCHECK(1,"WLT_ScanEnterExitReentLoopPairs");
							break; //condition de reentrance, possible manque de LTG_Exit
					}
				}
			}
		}
	}
	return;
}

/*

int SPG_CONV LT_ScanLoopPairs(WLOGTIMEPAIR& WP)
{
	LOGTIME* t=WP.t;
	LT_EVENTTYPE* EventType=(LT_EVENTTYPE*)((char*)t+t->szm+t->szEvents);
}

int SPG_CONV LT_ScanReentPairs(WLOGTIMEPAIR& WP)
{
	LOGTIME* t=WP.t;
	LT_EVENTTYPE* EventType=(LT_EVENTTYPE*)((char*)t+t->szm+t->szEvents);
}

*/

#endif

