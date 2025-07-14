
#include "SPG_General.h"

#ifdef DebugMem

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include "BreakHook.h"

#ifdef SPG_UseMalloc
#include <memory.h>
#define MA(n) (BYTE*)malloc(n)
#define MF(x) free(x)
#endif

#ifdef SPG_UseGlobalAlloc
//nclude "SPG_SysInc.h"
#define MA(Size) (BYTE*)GlobalAlloc(0,Size)
#define MF(Block) GlobalFree(Block)
#endif

#ifdef SPG_UseNew
#define MA(Size) new BYTE[Size]
#define MF(Block) delete[] Block
#endif

#include <stdio.h>
#include <string.h>
#include <crtdbg.h>

//efine SPGMEMFREEZE

void SPG_CONV SPG_MemSetPadding(void* M, int Len) { CHECK(Len==0,"SPG_MemSetPadding",return); memset(M,SPGMEMCODE,Len); return; }

bool SPG_CONV SPG_MemCheckPaddingByte(BYTE* M, int Len) { int i; for(i=0;i<Len;i++) { if(M[i]!=SPGMEMCODE) break; } return i==Len; }

//parcoure 4 par 4
bool SPG_CONV SPG_MemCheckPadding16(DWORD* M, int Len16) { int i; const int LenDW=4*Len16; for(i=0;i<LenDW;i+=4) { if( (M[i]!=SPGDWMEMCODE) || (M[i+1]!=SPGDWMEMCODE) || (M[i+2]!=SPGDWMEMCODE) || (M[i+3]!=SPGDWMEMCODE) ) break; } return i==LenDW; }

bool SPG_CONV SPG_MemCheckPadding(void* M, int Len)
{
	if(Len<32) 
		return SPG_MemCheckPaddingByte((BYTE*)M,Len);
	else
	{
		//aligne le point de depart sur un multiple de 16
		intptr_t &IM=*(intptr_t*)&M;
		intptr_t L1=IM&15;
		bool a=SPG_MemCheckPaddingByte((BYTE*)IM,L1);
		Len-=L1;
		IM+=L1;

		//parcourt 16 par 16
		L1=Len/16;
		bool b=SPG_MemCheckPadding16((DWORD*)IM,L1);
		Len-=16*L1;
		IM+=16*L1;

		//termine la partie fractionnaire
		bool c=SPG_MemCheckPaddingByte((BYTE*)IM,Len);
		return (a&b&c);
	}
}

bool SPG_CONV SPG_MemBlockCheck(int i, SPG_MEM_STATE& MS)
{
	if( MS.M[i].Flag&MS_FLAG_ALLOC )
	{
		int a=SPG_MemCheckPadding(MS.M[i].FullBlock,MS.M[i].Padding);
		int b=SPG_MemCheckPadding(MS.M[i].Block+MS.M[i].Len,MS.M[i].Padding);
		return a&b;
	}
	else if(MS.M[i].Flag&MS_FLAG_FROZEN)
	{
		int a=SPG_MemCheckPadding(MS.M[i].FullBlock,MS.M[i].FullLen);
		return a;
	}
	else if((MS.M[i].Flag&MS_FLAG_MSKNOTFREE)==0)
	{
		return true;
	}
	else
	{
		return false;
	}
}

bool SPG_CONV SPG_MemBlockCheckToString(char* Str, int i, SPG_MEM_STATE& MS)
{
	if( MS.M[i].Flag&MS_FLAG_ALLOC )
	{
		int a=SPG_MemCheckPadding(MS.M[i].FullBlock,MS.M[i].Padding);
		int b=SPG_MemCheckPadding(MS.M[i].Block+MS.M[i].Len,MS.M[i].Padding);
		const char* M="OK";
		if(a==0) M="LOWER GUARD CORRUPTED"; if(b==0) M="UPPER GUARD CORRUPTED"; if((a|b)==0) M="BOTH GUARD CORRUPTED";
		sprintf(Str,"\r\n%s\t%i\t%i\t%s\t%s",MS.M[i].Name,MS.M[i].AllocSequence,MS.M[i].Len,"ALLOC",M);
		return a&b;
	}
	else if(MS.M[i].Flag&MS_FLAG_FROZEN)
	{
		int a=SPG_MemCheckPadding(MS.M[i].FullBlock,MS.M[i].FullLen);
		sprintf(Str,"\r\n%s\t%i:%i\t%i\t%s\t%s",MS.M[i].Name,MS.M[i].AllocSequence,MS.M[i].FreeSequence,MS.M[i].Len,"FROZEN",a?"OK":"CORRUPTED AFTER FREE");
		return a;
	}
	else if((MS.M[i].Flag&MS_FLAG_MSKNOTFREE)==0)
	{
		sprintf(Str,"\r\n%s\t%i:%i\t%i\t%s\t%s",MS.M[i].Name,MS.M[i].AllocSequence,MS.M[i].FreeSequence,MS.M[i].Len,"FREE","NOT HANDLED");
		return true;
	}
	else
	{
		strcpy(Str,"Unknown block flag");
		return false;
	}
}


void SPG_CONV SPG_MemStateInit(int MaxBlock, SPG_MEM_STATE& MS)
{
#ifdef DebugMem
	_CrtSetDbgFlag( _CRTDBG_LEAK_CHECK_DF |
		            _CRTDBG_CHECK_ALWAYS_DF |
#ifdef SPGMEMFREEZE
					_CRTDBG_DELAY_FREE_MEM_DF |
#endif
		            _CRTDBG_CHECK_CRT_DF);
	_CrtSetReportMode(_CRT_WARN,_CRTDBG_MODE_WNDW|_CRTDBG_MODE_DEBUG);
	_CrtSetReportMode(_CRT_ERROR,_CRTDBG_MODE_WNDW|_CRTDBG_MODE_DEBUG);
	_CrtSetReportMode(_CRT_ASSERT,_CRTDBG_MODE_WNDW|_CRTDBG_MODE_DEBUG);
#endif
	SPG_ZeroStruct(MS);
	SPG_MemSetStruct(MS);
	SPL_Init(MS.L,200,"SPG_MemStateInit");
	MS.M=(SPG_MEM_RECORD*)MA(MaxBlock*sizeof(SPG_MEM_RECORD));
	MS.MaxBlock=MaxBlock;
	for(int i=0;i<MaxBlock;i++)
	{
		SPG_MemSetStruct(MS.M[i]);
	}
#ifdef SPGMEMFREEZE
	MS.Freeze=1;
#endif
	SPL_Exit(MS.L);
	return;
}

void SPG_CONV SPG_MemStateClose(SPG_MEM_STATE& MS)
{
	CHECK(SPL_Enter(MS.L,"SPG_MemStateClose")==SPL_TIMEOUT,"SPG_MemStateClose",return);
	{for(int i=0;i<MS.NumBlock;i++) { if((MS.M[i].Flag&MS_FLAG_ALLOC)==0) continue;
#ifdef DebugList
	char M[256]; sprintf(M,"SPG_MemStateClose\nMemory bloc\nName=%s\nSequence=%i\nLen=%i",MS.M[i].Name,MS.M[i].AllocSequence,MS.M[i].Len); SPG_List(M);
#endif
		SPL_Exit(MS.L);  SPG_MemFreePtr(*(void**)&MS.M[i].Block,MS);  SPL_Enter(MS.L,"SPG_MemStateClose - Reenter after free"); 
	}}
	{for(int i=0;i<MS.NumBlock;i++) { if( MS.M[i].Flag&MS_FLAG_FROZEN) {MF(MS.M[i].FullBlock); MS.M[i].Flag=MS_FLAG_FREE;} }}
	MF(MS.M);
	SPL_Close(MS.L);
	SPG_ZeroStruct(MS);
	_CrtSetReportMode(_CRT_WARN,_CRTDBG_MODE_DEBUG);
	//_CrtDumpMemoryLeaks();
	return;
}

void SPG_CONV SPG_MemStateDump(char* File, SPG_MEM_STATE& MS)
{
	FILE* F=fopen(File,"wb+");
	if(F)
	{
		fprintf(F,"Name                \tSeq\tSize\tStatus\tCheck");
		{for(int i=0;i<MS.NumBlock;i++) 
		{ 
			SPG_ArrayStackAlloc(char,Str,512); SPG_MemBlockCheckToString(Str,i,MS); SPG_ArrayStackCheck(Str);
			fprintf(F,Str);
		}}
		fclose(F);
	}
}

void SPG_CONV SPG_MemClean(SPG_MEM_STATE& MS)
{
	for(int i=0;i<MS.NumBlock;i++) 
	{ 
		if( MS.M[i].Flag&MS_FLAG_FROZEN) {MF(MS.M[i].FullBlock); MS.M[i].Flag=MS_FLAG_FREE;}
		if((MS.M[i].Flag&MS_FLAG_MSKNOTFREE)==0)
		{//remplis la première case vide avec le premier bloc alloué à suivre
			int j;
			for(j=i+1;j<MS.NumBlock;j++) { if(MS.M[j].Flag&MS_FLAG_ALLOC) {MS.M[i]=MS.M[j]; MS.M[j].Flag=MS_FLAG_FREE; break;} }
			if(j==MS.NumBlock) { MS.NumBlock=i; break; }
		}
	}
	return;
}

BYTE* SPG_CONV SPG_MemAlloc_nz(int Size, const char* Name, SPG_MEM_STATE& MS)
{
	CHECKTWO(Size==0,"SPG_MemAlloc_nz",Name,return 0);
	CHECK(SPL_Enter(MS.L,"SPG_MemAlloc_nz")==SPL_TIMEOUT,"SPG_MemAlloc_nz",return 0);
	int i=MS.NumBlock;
	if(MS.NumBlock<MS.MaxBlock)
	{
		MS.NumBlock++;
	}
	else
	{
		for(i=0;i<MS.NumBlock;i++) { if((MS.M[i].Flag&MS_FLAG_MSKNOTFREE)==0) break; }
		if(i==MS.NumBlock)
		{
			CHECKTWO(MS.NumBlock==MS.MaxBlock,"SPG_MemAlloc_nz : Too many blocks or MemState not initialized",Name,SPL_Exit(MS.L);return 0) 
			MS.NumBlock++;
		}
	}
	SPG_MEM_RECORD& R=MS.M[i];
	R.Flag=MS_FLAG_ALLOC;
	MS.M[i].SkipCheck=0;
	if(MS.BreakOnAllocSequence&&(MS.Sequence==MS.BreakOnAllocSequence)) 
	{
		BreakHook();
	}
	R.AllocSequence=MS.Sequence++;
	strncpy(R.Name,Name,SPGMEMNAME-1); R.Name[SPGMEMNAME-1]=0;
	R.Padding = V_Max(4,V_Round(sqrtf(Size)));
	R.FullLen = R.Padding+Size+R.Padding;
	R.FullBlock = MA(R.FullLen);
	CHECKTWO(R.FullBlock==0,"SPG_MemAlloc_nz : Failed",Name,SPL_Exit(MS.L);return 0);
	R.Block = R.FullBlock + R.Padding;
	R.Len = Size;

	SPG_MemSetPadding(R.FullBlock,R.FullLen);

	MS.NumAllocated++; MS.TotalAllocated+=Size;
	SPL_Exit(MS.L);
	return R.Block;
}

void SPG_CONV SPG_MemFree_nz(void* Block, SPG_MEM_STATE& MS)
{
	CHECK(SPL_Enter(MS.L,"SPG_MemFree_nz")==SPL_TIMEOUT,"SPG_MemFree_nz",return);
	int i=0;
	for(i=0;i<MS.NumBlock;i++) { if( (MS.M[i].Flag&MS_FLAG_ALLOC) && (MS.M[i].Block==Block) ) break; }
	CHECK(i==MS.NumBlock,"SPG_MemFree_nz : Invalid free ptr",SPL_Exit(MS.L);return);
	SPG_MEM_RECORD& R=MS.M[i];
	DbgCHECK(R.Flag&MS_FLAG_DEBUGBREAKONFREE,"SPG_MemFree_nz : Found debug flag MS_FLAG_DEBUGBREAKONFREE");
	R.FreeSequence=MS.Sequence++;
	SPG_MemSetPadding(R.Block,R.Len);
	if(MS.Freeze)
	{
		R.Flag=MS_FLAG_FROZEN;
	}
	else
	{
		MF(R.FullBlock);
		R.Flag=MS_FLAG_FREE;
	}
	MS.NumFreed++; MS.TotalFreed+=R.Len;
	SPL_Exit(MS.L);
	return;
}

BYTE* SPG_CONV SPG_MemAlloc(int Size, const char* Name, SPG_MEM_STATE& MS) { BYTE* M=SPG_MemAlloc_nz(Size,Name,MS); if(M) memset(M,0,Size); return M; }

void SPG_CONV SPG_MemFreePtr(void* &Block, SPG_MEM_STATE& MS) { SPG_MemFree_nz(Block,MS); Block=0; return; }

int SPG_CONV SPG_MemIsBlock(void* Block, SPG_MEM_STATE& MS)
{
	int i=0;
	for(i=0;i<MS.NumBlock;i++) { if((MS.M[i].Flag&MS_FLAG_ALLOC)&&(MS.M[i].Block==Block)) break; }
	return (i!=MS.NumBlock);
}

int SPG_CONV SPG_MemIsExactBlock(void* Block, int Len, SPG_MEM_STATE& MS)
{
	int i=0;
	for(i=0;i<MS.NumBlock;i++) { if( (MS.M[i].Flag&MS_FLAG_ALLOC) && (MS.M[i].Block==Block) ) break; }
	return ( (i!=MS.NumBlock) && (MS.M[i].Len==Len) );
}

int SPG_CONV SPG_MemIsValid(void* Block, int Len, SPG_MEM_STATE& MS)
{
	int i=0;
	for(i=0;i<MS.NumBlock;i++)
	{
		if( (MS.M[i].Flag&MS_FLAG_ALLOC) &&
			(MS.M[i].Block<=Block) && 
			( (MS.M[i].Block+MS.M[i].Len) >= ((BYTE*)Block+Len) ) 
		  ) break;
	}
	return (i!=MS.NumBlock);
}

int SPG_CONV SPG_MemStateCheck(SPG_MEM_STATE& MS)
{
	if(GetAsyncKeyState(VK_ESCAPE)) return 0;
	CHECK(Global.Etat==0,"SrcC not initialized - call first",return 0);
	CHECK(SPL_Enter(MS.L,"SPG_MemStateCheck")==SPL_TIMEOUT,"SPG_MemStateCheck",return 0);
	SPG_MemCheckStruct(MS,"SPG_MemStateCheck","MS");
	int T=0;
	SPG_ArrayStackAlloc(char,Str,512);
	for(int i=0;i<MS.NumBlock;i++)
	{
		if(MS.M[i].SkipCheck>3) continue;
		//SPG_MemCheckStruct(MS,"SPG_MemStateCheck : corrupted state structure",MS.M[i].Name);
		if(!SPG_MemBlockCheck(i,MS))
		{
			SPG_MemBlockCheckToString(Str,i,MS);
			SPG_List(Str);
			SPG_ArrayStackCheck(Str);
			MS.M[i].SkipCheck++;
		}
		if(MS.M[i].Flag&MS_FLAG_ALLOC) T+=MS.M[i].Len;
	}
	DbgCHECK(_CrtCheckMemory()==0,"");
	SPL_Exit(MS.L);
	return T;
}

void SPG_CONV SPG_SetMemBreakOnFree(void* Block, const char* Name, SPG_MEM_STATE& MS)
{
	int i=0;
	for(i=0;i<MS.NumBlock;i++) { if( (MS.M[i].Flag&MS_FLAG_ALLOC) && (MS.M[i].Block==Block) ) break; }
	CHECK(i==MS.NumBlock,"SPG_SetMemName : Invalid ptr",return);
	SPG_MEM_RECORD& R=MS.M[i];
	R.Flag|=MS_FLAG_DEBUGBREAKONFREE;
	return;
}

void SPG_CONV SPG_SetMemName(void* Block, const char* Name, SPG_MEM_STATE& MS)
{
	if(Block==0) return; //erreur silencieuse
	int i=0;
	for(i=0;i<MS.NumBlock;i++) { if( (MS.M[i].Flag&MS_FLAG_ALLOC) && (MS.M[i].Block==Block) ) break; }
	CHECK(i==MS.NumBlock,"SPG_SetMemName : Invalid ptr",return);
	SPG_MEM_RECORD& R=MS.M[i];
	strncpy(R.Name,Name,SPGMEMNAME-1); R.Name[SPGMEMNAME-1]=0;
	return;
}

void SPG_CONV SPG_CatMemName(void* Block, const char* Name, SPG_MEM_STATE& MS)
{
	if(Block==0) return; //erreur silencieuse
	int i=0;
	for(i=0;i<MS.NumBlock;i++) { if( (MS.M[i].Flag&MS_FLAG_ALLOC) && (MS.M[i].Block==Block) ) break; }
	CHECK(i==MS.NumBlock,"SPG_SetMemName : Invalid ptr",return);
	SPG_MEM_RECORD& R=MS.M[i];
	int n=SPGMEMNAME-(int)strlen(R.Name);
	strncat(R.Name,Name,n-1); R.Name[SPGMEMNAME-1]=0;
	return;
}

#endif
