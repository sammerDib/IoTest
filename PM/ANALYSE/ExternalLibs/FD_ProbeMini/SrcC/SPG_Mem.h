
#ifdef DebugMem

#ifdef SPG_General_USEGlobal
#define SPG_MEMORY_MANAGER SPG_MEM_STATE& MS=Global.MS
#else
#define SPG_MEMORY_MANAGER SPG_MEM_STATE& MS=(*(SPG_MEM_STATE*)(0))
#endif

void SPG_CONV SPG_MemSetPadding(void* M, int Len);
bool SPG_CONV SPG_MemCheckPadding(void* M, int Len);

void SPG_CONV SPG_MemStateInit(int MaxBlock, SPG_MEMORY_MANAGER);
void SPG_CONV SPG_MemStateClose(SPG_MEMORY_MANAGER);
void SPG_CONV SPG_MemStateDump(char* File, SPG_MEMORY_MANAGER);
int SPG_CONV SPG_MemStateCheck(SPG_MEMORY_MANAGER);
#define SPG_MemFastCheck() SPG_MemStateCheck(Global.MS)
BYTE* SPG_CONV SPG_MemAlloc_nz(int Size, char* Name, SPG_MEMORY_MANAGER);
void SPG_CONV SPG_MemFree_nz(void* Block, SPG_MEMORY_MANAGER);
BYTE* SPG_CONV SPG_MemAlloc(int Size, const char* Name, SPG_MEMORY_MANAGER);
void SPG_CONV SPG_MemFreePtr(void* &Block, SPG_MEMORY_MANAGER);
#define SPG_MemFree(Block) SPG_MemFreePtr(*(void**)&Block, Global.MS);
int SPG_CONV SPG_MemIsBlock(void* Block, SPG_MEMORY_MANAGER);
int SPG_CONV SPG_MemIsExactBlock(void* Block, int Len, SPG_MEMORY_MANAGER);
int SPG_CONV SPG_MemIsValid(void* Block, int Len, SPG_MEMORY_MANAGER);
void SPG_CONV SPG_SetMemBreakOnFree(void* Block, const char* Name, SPG_MEMORY_MANAGER);
void SPG_CONV SPG_SetMemName(void* Block, const char* Name, SPG_MEMORY_MANAGER);
void SPG_CONV SPG_CatMemName(void* Block, const char* Name, SPG_MEMORY_MANAGER);

#define SPG_SET_STACK_SPY(LABEL) __int64 SPG_STACK_SPY_##LABEL; SPG_MEMSETPAD(SPG_STACK_SPY_##LABEL);
#define SPG_SET_ARRAY_SPY(VARIABLE) SPG_MemSetPadding(VARIABLE##_AndBounds,STKELTPAD*STACK_TYPE_SIZE_##VARIABLE);SPG_MemSetPadding(VARIABLE##_AndBounds+STACK_ELEMENTS_##VARIABLE+STKELTPAD,STKELTPAD*STACK_TYPE_SIZE_##VARIABLE);
#define SPG_CHECK_STACK_SPY(LABEL) {DbgCHECK(SPG_MEMCHKPAD(SPG_STACK_SPY_##LABEL)==0,"Stack error "#LABEL);}
#define SPG_CHECK_ARRAY_SPY(VARIABLE) {DbgCHECK(SPG_MemCheckPadding(VARIABLE##_AndBounds,STKELTPAD*STACK_TYPE_SIZE_##VARIABLE)==0,"Stack error "#VARIABLE);DbgCHECK(SPG_MemCheckPadding(VARIABLE##_AndBounds+STACK_ELEMENTS_##VARIABLE+STKELTPAD,STKELTPAD*STACK_TYPE_SIZE_##VARIABLE)==0,"Stack error "#VARIABLE);}

#ifdef SPG_General_USEGlobal
#define SPG_GET_STACK_INITIAL_POS() {int SPG_SK_TST;Global.StackInitPos=(intptr_t)&SPG_SK_TST;}
#define SPG_GET_STACK(N) {int SPG_SK_TST;N=Global.StackInitPos-((intptr_t)&SPG_SK_TST);}
#else //!SPG_General_USEGlobal
#define SPG_GET_STACK_INITIAL_POS()
#define SPG_GET_STACK()

#endif //SPG_General_USEGlobal //!DebugMem

#define STKELTPAD 4
#define SPG_MEMSETPAD(x) SPG_MemSetPadding(&x,sizeof(x));
#define SPG_MEMCHKPAD(x) SPG_MemCheckPadding(&x,sizeof(x))

#define SPG_PRIVATE_StackAlloc(TYPE,VARIABLE) SPG_SET_STACK_SPY(Before##VARIABLE);TYPE VARIABLE;SPG_SET_STACK_SPY(After##VARIABLE);
#define SPG_PRIVATE_ArrayStackAlloc(TYPE,VARIABLE,ELEMENTS) int STACK_TYPE_SIZE_##VARIABLE=sizeof(TYPE);int STACK_ELEMENTS_##VARIABLE=ELEMENTS;TYPE VARIABLE##_AndBounds[ELEMENTS+2*STKELTPAD];TYPE* VARIABLE=VARIABLE##_AndBounds+STKELTPAD;SPG_SET_ARRAY_SPY(VARIABLE);
#define SPG_StackAlloc(TYPE,VARIABLE) SPG_PRIVATE_StackAlloc(TYPE,VARIABLE); memset(&VARIABLE,SPGMEMCODE,sizeof(TYPE))
#define SPG_ArrayStackAlloc(TYPE,VARIABLE,ELEMENTS) SPG_PRIVATE_ArrayStackAlloc(TYPE,VARIABLE,ELEMENTS); memset(VARIABLE,0x55,ELEMENTS*sizeof(TYPE))
#define SPG_StackAllocZ(TYPE,VARIABLE) SPG_PRIVATE_StackAlloc(TYPE,VARIABLE); memset(&VARIABLE,0,sizeof(TYPE))
#define SPG_ArrayStackAllocZ(TYPE,VARIABLE,ELEMENTS) SPG_PRIVATE_ArrayStackAlloc(TYPE,VARIABLE,ELEMENTS); memset(VARIABLE,0,ELEMENTS*sizeof(TYPE))
#define SPG_CHECK_StackAlloc(VARIABLE) SPG_CHECK_STACK_SPY(Before##VARIABLE);SPG_CHECK_STACK_SPY(After##VARIABLE)
#define SPG_StackCheck(VARIABLE) SPG_CHECK_StackAlloc(VARIABLE)
#define SPG_ArrayStackCheck(VARIABLE) SPG_CHECK_ARRAY_SPY(VARIABLE);

#define CHECKPOINTER(Mem,Msg,Ret) CHECKV(SPG_MemIsValid(Mem,0)==-1,Msg,(void*)(Mem),Ret)
#define CHECKPOINTER_L(Mem,Len,Msg,Ret) CHECKV(SPG_MemIsValid(Mem,Len)==-1,Msg,(void*)(Mem),Ret)
#define CHECKPOINTER_L_ELSE(Mem,Len,Msg,Ret) CHECKV_ELSE(SPG_MemIsValid(Mem,Len)==-1,Msg,(void*)(Mem),Ret)
#define SPG_Memcpy(MemDst, MemSrc, Len) { CHECKPOINTER_L_ELSE(MemDst,Len,"SPG_Memcpy",;) else {CHECKPOINTER_L_ELSE(MemSrc,Len,"SPG_Memcpy",;) else {DbgCHECK(MemDst==MemSrc,"SPG_Memcpy");memcpy(MemDst,MemSrc,Len);} } }
#define SPG_Memmove(MemDst, MemSrc, Len) { CHECKPOINTER_L_ELSE(MemDst,Len,"SPG_Memmove",;) else {CHECKPOINTER_L_ELSE(MemSrc,Len,"SPG_Memcpy",;) else {memmove(MemDst,MemSrc,Len);} } }
#define SPG_Memset(MemDst, c, Len) { CHECKPOINTER_L_ELSE(MemDst,Len,"SPG_Memset",;) else {memset(MemDst,c,Len);} }
#define SPG_MemSetStruct(S) { S.MemStructCheckLower=SPGCHECKCODE; S.MemStructCheckHigher=SPGCHECKCODE;}
#define SPG_MemCheckStruct(S, Msg, Str) { DbgCHECKTWO(S.MemStructCheckLower!=SPGCHECKCODE,Msg,Str); DbgCHECKTWO(S.MemStructCheckHigher!=SPGCHECKCODE,Msg,Str);}

#else

#define SPG_MemStateInit(MaxBlock, MS)
#define SPG_MemStateClose(MS)
#define SPG_MemStateCheck(MS)

#ifdef SPG_UseNew

#include <memory.h>

__inline BYTE* SPG_CONV newZ(int Size) { BYTE* M=new BYTE[Size];memset(M,0,Size);return M; }
template<typename T> __inline T* newTZ(int Size) { T* M=new T[Size];memset(M,0,Size*sizeof(T));return M; }
#define SPG_MemAlloc_nz(Size, Name) new BYTE[Size]
#define SPG_MemFree_nz(Block)       delete Block[]
#define SPG_MemAlloc(Size, Name)    newZ(Size)
#define SPG_MemFree(Block)          {delete[] Block;Block=0;}
#endif

#ifdef SPG_UseMalloc

#include <memory.h>

__inline BYTE* SPG_CONV mallocZ(int Size) { BYTE* M=(BYTE*)malloc(Size);memset(M,0,Size);return M; }
#define SPG_MemAlloc_nz(Size, Name) (BYTE*)malloc(Size)
#define SPG_MemFree_nz(Block)       free(Block)
#define SPG_MemAlloc(Size, Name)    mallocZ(Size)
#define SPG_MemFree(Block)          {free(Block);Block=0;}
#endif

#ifdef SPG_UseGlobalAlloc

#define SPG_MemAlloc_nz(Size, Name) (BYTE*)GlobalAlloc(0,Size)
#define SPG_MemFree_nz(Block)		GlobalFree(Block)
#define SPG_MemAlloc(Size, Name)    (BYTE*)GlobalAlloc(0x40,Size)
#define SPG_MemFree(Block)          {GlobalFree(Block);Block=0;}
#endif

#define SPG_SET_STACK_SPY(LABEL)
#define SPG_SET_ARRAY_SPY(VARIABLE)
#define SPG_CHECK_STACK_SPY(LABEL)
#define SPG_GET_STACK_INITIAL_POS()
#define SPG_GET_STACK(N)
#define SPG_DISPLAY_STACK(CONSOLE)
#define SPG_DISPLAY_TYPEINFO(CONSOLE,TYPE)
#define SPG_StackAlloc(TYPE,VARIABLE) TYPE VARIABLE
#define SPG_ArrayStackAlloc(TYPE,VARIABLE,ELEMENTS) TYPE VARIABLE[ELEMENTS]
#define SPG_StackAllocZ(TYPE,VARIABLE) SPG_StackAlloc(TYPE,VARIABLE); memset(&VARIABLE,0,sizeof(TYPE));
#define SPG_ArrayStackAllocZ(TYPE,VARIABLE,ELEMENTS) SPG_ArrayStackAlloc(TYPE,VARIABLE,ELEMENTS); memset(&VARIABLE,0,ELEMENTS*sizeof(TYPE));
#define SPG_CHECK_StackAlloc(VARIABLE)
#define SPG_StackCheck(VARIABLE)
#define SPG_ArrayStackCheck(VARIABLE)
#define SPG_MemFastCheck()
#define SPG_SetMemName(Mem,Name)
#define SPG_GetMemName(Mem,Name)
#define SPG_CatMemName(Mem,Name)
#define SPG_Memcpy(MemDst, MemSrc, Len) memcpy(MemDst,MemSrc,Len)
#define SPG_Memmove(MemDst, MemSrc, Len) memmove(MemDst,MemSrc,Len)
#define SPG_Memset(MemDst, c, Len) memset(MemDst,c,Len)
int SPG_CONV SPG_Memcmp(void* Mem, BYTE C, int Len);
#define SPG_MemcmpM(Mem,C,Len) SPG_Memcmp(Mem,C,Len)
#define CHECKPOINTER(Mem,Msg,Ret) CHECK(Mem==0,Msg,Ret)
#define CHECKPOINTER_L(Mem,Len,Msg,Ret) CHECK(Mem==0,Msg,Ret)
#define CHECKPOINTER_L_ELSE(Mem,Len,Msg,Ret) CHECK_ELSE(Mem==0,Msg,Ret)
#define SPG_MemSetStruct(S)
#define SPG_MemCheckStruct(S, Msg, Str)

#endif

#define SPG_ZeroStruct(S) memset(&S,0,sizeof(S));
#define SPG_MemSetName(Mem,Name) SPG_SetMemName(Mem,Name)
#define SPG_MemCatName(Mem,Name) SPG_CatMemName(Mem,Name)
#if defined(SPG_UseNew)&&(!defined(DebugMem))
#define SPG_TypeAlloc_nz(Size,Type,Nom) new Type[Size]
#define SPG_TypeAlloc(Size,Type,Nom) newTZ<Type>(Size)
#else
#define SPG_TypeAlloc_nz(Size,Type,Nom) (Type*)SPG_MemAlloc_nz((Size)*sizeof(Type),#Type":"Nom)
#define SPG_TypeAlloc(Size,Type,Nom) (Type*)SPG_MemAlloc((Size)*sizeof(Type),#Type ## ":" ## Nom)
#endif
#define SPG_PtrAlloc_nz(Ptr,Size,Type,Nom) SPG_SET_STACK_SPY(Before##Ptr);Type* Ptr=(Type*)SPG_MemAlloc_nz((Size)*sizeof(Type),#Type":"Nom);SPG_SET_STACK_SPY(After##Ptr)
#define SPG_PtrAlloc(Ptr,Size,Type,Nom) SPG_SET_STACK_SPY(Before##Ptr);Type* Ptr=(Type*)SPG_MemAlloc((Size)*sizeof(Type),#Type":"Nom);SPG_SET_STACK_SPY(After##Ptr)
#define SPG_PtrFree(Ptr) SPG_CHECK_StackAlloc(Ptr);SPG_MemFree(Ptr);

