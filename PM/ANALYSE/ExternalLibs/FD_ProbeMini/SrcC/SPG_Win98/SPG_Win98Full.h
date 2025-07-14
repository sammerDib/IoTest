//#pragma message("SPG_Win98Full_Start")
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 28 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 43 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
#pragma once
	// 50 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 100 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 121 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 125 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 129 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 133 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 137 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 141 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 145 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 150 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 151 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
#pragma warning(disable:4001)
#pragma warning(disable:4201)
#pragma warning(disable:4214)
#pragma warning(disable:4514)
	// 159 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\excpt.h"
#pragma once
	// 18 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\excpt.h"
	// 25 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\excpt.h"
#pragma pack(push,8)
	// 34 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\excpt.h"
extern "C" {
	// 38 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\excpt.h"
	// 57 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\excpt.h"
typedef enum _EXCEPTION_DISPOSITION {
ExceptionContinueExecution,ExceptionContinueSearch,ExceptionNestedException,ExceptionCollidedUnwind
} EXCEPTION_DISPOSITION;
struct _EXCEPTION_RECORD;
struct _CONTEXT;
EXCEPTION_DISPOSITION __cdecl _except_handler (
struct _EXCEPTION_RECORD *ExceptionRecord,void* EstablisherFrame,struct _CONTEXT *ContextRecord,void* DispatcherContext);
	// 118 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\excpt.h"
unsigned long __cdecl _exception_code(void);
void* __cdecl _exception_info(void);
int __cdecl _abnormal_termination(void);
	// 138 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\excpt.h"
}
	// 153 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\excpt.h"
#pragma pack(pop)
	// 157 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\excpt.h"
	// 159 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\excpt.h"
	// 160 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\stdarg.h"
#pragma once
	// 18 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\stdarg.h"
	// 25 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\stdarg.h"
#pragma pack(push,8)
	// 34 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\stdarg.h"
extern "C" {
	// 38 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\stdarg.h"
	// 153 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\stdarg.h"
}
	// 158 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\stdarg.h"
#pragma pack(pop)
	// 162 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\stdarg.h"
	// 164 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\stdarg.h"
	// 161 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 162 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 17 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 18 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
extern "C" {
	// 32 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
typedef unsigned long ULONG;
typedef ULONG ULONG_PTR;
typedef ULONG *PULONG;
typedef unsigned short USHORT;
typedef USHORT *PUSHORT;
typedef unsigned char UCHAR;
typedef UCHAR *PUCHAR;
typedef char *PSZ;
	// 51 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 65 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 69 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 73 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 77 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 81 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 93 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 100 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 104 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 105 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 132 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 140 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
typedef unsigned long DWORD;
typedef int BOOL;
typedef unsigned char BYTE;
typedef unsigned short WORD;
typedef float FLOAT;
typedef FLOAT *PFLOAT;
typedef BOOL *PBOOL;
typedef BOOL *LPBOOL;
typedef BYTE *PBYTE;
typedef BYTE *LPBYTE;
typedef int *PINT;
typedef int *LPINT;
typedef WORD *PWORD;
typedef WORD *LPWORD;
typedef long *LPLONG;
typedef DWORD *PDWORD;
typedef DWORD *LPDWORD;
typedef void *LPVOID;
typedef const void *LPCVOID;
typedef int INT;
typedef unsigned int UINT;
typedef unsigned int *PUINT;
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
extern "C" {
	// 24 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
#pragma once
	// 17 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
	// 24 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
extern "C" {
	// 29 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
	// 48 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
	// 75 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
extern unsigned short _ctype[];
extern unsigned short *_pctype;
extern wctype_t *_pwctype;
	// 83 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
	// 84 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
int __cdecl _isctype(int, int);
int __cdecl isalpha(int);
int __cdecl isupper(int);
int __cdecl islower(int);
int __cdecl isdigit(int);
int __cdecl isxdigit(int);
int __cdecl isspace(int);
int __cdecl ispunct(int);
int __cdecl isalnum(int);
int __cdecl isprint(int);
int __cdecl isgraph(int);
int __cdecl iscntrl(int);
//int __cdecl toupper(int);
//int __cdecl tolower(int);
int __cdecl _tolower(int);
int __cdecl _toupper(int);
int __cdecl __isascii(int);
int __cdecl __toascii(int);
int __cdecl __iscsymf(int);
int __cdecl __iscsym(int);
	// 128 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
int __cdecl iswalpha(wint_t);
int __cdecl iswupper(wint_t);
int __cdecl iswlower(wint_t);
int __cdecl iswdigit(wint_t);
int __cdecl iswxdigit(wint_t);
int __cdecl iswspace(wint_t);
int __cdecl iswpunct(wint_t);
int __cdecl iswalnum(wint_t);
int __cdecl iswprint(wint_t);
int __cdecl iswgraph(wint_t);
int __cdecl iswcntrl(wint_t);
int __cdecl iswascii(wint_t);
int __cdecl isleadbyte(int);
wchar_t __cdecl towupper(wchar_t);
wchar_t __cdecl towlower(wchar_t);
int __cdecl iswctype(wint_t, wctype_t);
int __cdecl is_wctype(wint_t, wctype_t);
	// 162 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
	// 163 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
	// 252 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
	// 254 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
	// 295 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
	// 297 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
	// 304 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
	// 321 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
	// 323 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
}
	// 327 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
	// 330 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\ctype.h"
	// 26 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 31 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 37 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 39 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 43 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 47 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 49 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 81 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 85 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef unsigned long POINTER_64_INT;
	// 88 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 90 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\basetsd.h"
extern "C" {
	// 27 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\basetsd.h"
typedef int LONG32, *PLONG32;
typedef int INT32, *PINT32;
typedef unsigned int ULONG32, *PULONG32;
typedef unsigned int DWORD32, *PDWORD32;
typedef unsigned int UINT32, *PUINT32;
typedef long INT_PTR, *PINT_PTR;
typedef unsigned long UINT_PTR, *PUINT_PTR;
typedef unsigned short UHALF_PTR, *PUHALF_PTR;
typedef short HALF_PTR, *PHALF_PTR;
	// 144 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\basetsd.h"
typedef UINT_PTR SIZE_T, *PSIZE_T;
typedef INT_PTR SSIZE_T, *PSSIZE_T;
typedef __int64 LONG64, *PLONG64;
typedef __int64 INT64, *PINT64;
typedef unsigned __int64 ULONG64, *PULONG64;
typedef unsigned __int64 DWORD64, *PDWORD64;
typedef unsigned __int64 UINT64, *PUINT64;
}
	// 172 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\basetsd.h"
	// 174 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\basetsd.h"
	// 92 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 99 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 105 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef void *PVOID;
typedef void* PVOID64;
	// 116 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 126 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef char CHAR;
typedef short SHORT;
typedef long LONG;
	// 138 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef wchar_t WCHAR;
	// 149 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef WCHAR *PWCHAR;
typedef WCHAR *LPWCH, *PWCH;
typedef const WCHAR *LPCWCH, *PCWCH;
typedef WCHAR *NWPSTR;
typedef WCHAR *LPWSTR, *PWSTR;
typedef const WCHAR *LPCWSTR, *PCWSTR;
typedef CHAR *PCHAR;
typedef CHAR *LPCH, *PCH;
typedef const CHAR *LPCCH, *PCCH;
typedef CHAR *NPSTR;
typedef CHAR *LPSTR, *PSTR;
typedef const CHAR *LPCSTR, *PCSTR;
typedef char TCHAR, *PTCHAR;
typedef unsigned char TBYTE , *PTBYTE ;
	// 193 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef LPSTR LPTCH, PTCH;
typedef LPSTR PTSTR, LPTSTR;
typedef LPCSTR LPCTSTR;
	// 200 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef SHORT *PSHORT;
typedef LONG *PLONG;
typedef void *HANDLE;
	// 213 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef HANDLE *PHANDLE;
typedef BYTE FCHAR;
typedef WORD FSHORT;
typedef DWORD FLONG;
typedef LONG HRESULT;
	// 230 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 236 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 246 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 260 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef char CCHAR;
typedef DWORD LCID;
typedef PDWORD PLCID;
typedef WORD LANGID;
typedef struct _FLOAT128 {
__int64 LowPart;
__int64 HighPart;
} FLOAT128;
typedef FLOAT128 *PFLOAT128;
	// 316 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef __int64 LONGLONG;
typedef unsigned __int64 ULONGLONG;
	// 342 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef LONGLONG *PLONGLONG;
typedef ULONGLONG *PULONGLONG;
typedef LONGLONG USN;
	// 353 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef union _LARGE_INTEGER {
struct {
DWORD LowPart;
LONG HighPart;
};
struct {
DWORD LowPart;
LONG HighPart;
} u;
	// 363 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
LONGLONG QuadPart;
} LARGE_INTEGER;
typedef LARGE_INTEGER *PLARGE_INTEGER;
	// 372 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef union _ULARGE_INTEGER {
struct {
DWORD LowPart;
DWORD HighPart;
};
struct {
DWORD LowPart;
DWORD HighPart;
} u;
	// 382 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
ULONGLONG QuadPart;
} ULARGE_INTEGER;
typedef ULARGE_INTEGER *PULARGE_INTEGER;
typedef struct _LUID {
DWORD LowPart;
LONG HighPart;
} LUID, *PLUID;
typedef ULONGLONG DWORDLONG;
typedef DWORDLONG *PDWORDLONG;
	// 425 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 489 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
ULONGLONG __stdcall Int64ShllMod32 (
ULONGLONG Value,DWORD ShiftCount);
LONGLONG __stdcall Int64ShraMod32 (
LONGLONG Value,DWORD ShiftCount);
ULONGLONG __stdcall Int64ShrlMod32 (
ULONGLONG Value,DWORD ShiftCount);
#pragma warning(disable:4035)
__inline ULONGLONG __stdcall Int64ShllMod32 (
ULONGLONG Value,DWORD ShiftCount
)
{
__asm {
mov ecx, ShiftCount
mov eax, dword ptr [Value]
mov edx, dword ptr [Value+4]
shld edx, eax, cl
shl eax, cl
}
}
__inline LONGLONG __stdcall Int64ShraMod32 (
LONGLONG Value,DWORD ShiftCount
)
{
__asm {
mov ecx, ShiftCount
mov eax, dword ptr [Value]
mov edx, dword ptr [Value+4]
shrd eax, edx, cl
sar edx, cl
}
}
__inline ULONGLONG __stdcall Int64ShrlMod32 (
ULONGLONG Value,DWORD ShiftCount
)
{
__asm {
mov ecx, ShiftCount
mov eax, dword ptr [Value]
mov edx, dword ptr [Value+4]
shrd eax, edx, cl
shr edx, cl
}
}
#pragma warning(default:4035)
	// 633 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef BYTE BOOLEAN;
typedef BOOLEAN *PBOOLEAN;
typedef struct _LIST_ENTRY {
struct _LIST_ENTRY *Flink;
struct _LIST_ENTRY *Blink;
} LIST_ENTRY, *PLIST_ENTRY,* PRLIST_ENTRY;
typedef struct _SINGLE_LIST_ENTRY {
struct _SINGLE_LIST_ENTRY *Next;
} SINGLE_LIST_ENTRY, *PSINGLE_LIST_ENTRY;
typedef struct _GUID {
DWORD Data1;
WORD Data2;
WORD Data3;
BYTE Data4[8];
} GUID;
	// 672 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _OBJECTID {
GUID Lineage;
DWORD Uniquifier;
} OBJECTID;
	// 681 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 1049 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 1088 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef UINT_PTR KSPIN_LOCK;
typedef KSPIN_LOCK *PKSPIN_LOCK;
	// 1109 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
struct _TEB *
NtCurrentTeb(void);
	// 1112 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
#pragma warning(disable:4164)
#pragma function(_enable)
#pragma function(_disable)
	// 1510 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
#pragma warning(default:4164)
	// 1514 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 1515 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
#pragma warning (disable:4035)
_inline PVOID GetFiberData( void ) { __asm {
mov eax, fs:[0x10]
mov eax,[eax]
}
}
_inline PVOID GetCurrentFiber( void ) { __asm mov eax, fs:[0x10] }
#pragma warning (default:4035)
	// 1528 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 1561 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _FLOATING_SAVE_AREA {
DWORD ControlWord;
DWORD StatusWord;
DWORD TagWord;
DWORD ErrorOffset;
DWORD ErrorSelector;
DWORD DataOffset;
DWORD DataSelector;
BYTE RegisterArea[80];
DWORD Cr0NpxState;
} FLOATING_SAVE_AREA;
typedef FLOATING_SAVE_AREA *PFLOATING_SAVE_AREA;
typedef struct _CONTEXT {
DWORD ContextFlags;
DWORD Dr0;
DWORD Dr1;
DWORD Dr2;
DWORD Dr3;
DWORD Dr6;
DWORD Dr7;
FLOATING_SAVE_AREA FloatSave;
DWORD SegGs;
DWORD SegFs;
DWORD SegEs;
DWORD SegDs;
DWORD Edi;
DWORD Esi;
DWORD Ebx;
DWORD Edx;
DWORD Ecx;
DWORD Eax;
DWORD Ebp;
DWORD Eip;
DWORD SegCs;
DWORD EFlags;
DWORD Esp;
DWORD SegSs;
BYTE ExtendedRegisters[512];
} CONTEXT;
typedef CONTEXT *PCONTEXT;
	// 1681 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _LDT_ENTRY {
WORD LimitLow;
WORD BaseLow;
union {
struct {
BYTE BaseMid;
BYTE Flags1;
BYTE Flags2;
BYTE BaseHi;
} Bytes;
struct {
DWORD BaseMid : 8;
DWORD Type : 5;
DWORD Dpl : 2;
DWORD Pres : 1;
DWORD LimitHi : 4;
DWORD Sys : 1;
DWORD Reserved_0 : 1;
DWORD Default_Big : 1;
DWORD Granularity : 1;
DWORD BaseHi : 8;
} Bits;
} HighWord;
} LDT_ENTRY, *PLDT_ENTRY;
	// 1712 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 2058 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 2069 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 2299 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 2528 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 2544 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _EXCEPTION_RECORD {
DWORD ExceptionCode;
DWORD ExceptionFlags;
struct _EXCEPTION_RECORD *ExceptionRecord;
PVOID ExceptionAddress;
DWORD NumberParameters;
UINT_PTR ExceptionInformation[15];
} EXCEPTION_RECORD;
typedef EXCEPTION_RECORD *PEXCEPTION_RECORD;
typedef struct _EXCEPTION_POINTERS {
PEXCEPTION_RECORD ExceptionRecord;
PCONTEXT ContextRecord;
} EXCEPTION_POINTERS, *PEXCEPTION_POINTERS;
typedef PVOID PACCESS_TOKEN;
typedef PVOID PSECURITY_DESCRIPTOR;
typedef PVOID PSID;
typedef DWORD ACCESS_MASK;
typedef ACCESS_MASK *PACCESS_MASK;
typedef struct _GENERIC_MAPPING {
ACCESS_MASK GenericRead;
ACCESS_MASK GenericWrite;
ACCESS_MASK GenericExecute;
ACCESS_MASK GenericAll;
} GENERIC_MAPPING;
typedef GENERIC_MAPPING *PGENERIC_MAPPING;
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack4.h"
#pragma warning(disable:4103)
#pragma pack(push)
	// 28 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack4.h"
#pragma pack(4)
	// 32 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack4.h"
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack4.h"
	// 3265 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _LUID_AND_ATTRIBUTES {
LUID Luid;
DWORD Attributes;
} LUID_AND_ATTRIBUTES,* PLUID_AND_ATTRIBUTES;
typedef LUID_AND_ATTRIBUTES LUID_AND_ATTRIBUTES_ARRAY[1];
typedef LUID_AND_ATTRIBUTES_ARRAY *PLUID_AND_ATTRIBUTES_ARRAY;
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
#pragma warning(disable:4103)
#pragma pack(pop)
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 36 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 37 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 3274 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _SID_IDENTIFIER_AUTHORITY {
BYTE Value[6];
} SID_IDENTIFIER_AUTHORITY, *PSID_IDENTIFIER_AUTHORITY;
	// 3312 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _SID {
BYTE Revision;
BYTE SubAuthorityCount;
SID_IDENTIFIER_AUTHORITY IdentifierAuthority;
DWORD SubAuthority[1];
	// 3325 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
} SID, *PISID;
	// 3327 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef enum _SID_NAME_USE {
SidTypeUser = 1,SidTypeGroup,SidTypeDomain,SidTypeAlias,SidTypeWellKnownGroup,SidTypeDeletedAccount,SidTypeInvalid,SidTypeUnknown,SidTypeComputer
} SID_NAME_USE, *PSID_NAME_USE;
typedef struct _SID_AND_ATTRIBUTES {
PSID Sid;
DWORD Attributes;
} SID_AND_ATTRIBUTES,* PSID_AND_ATTRIBUTES;
typedef SID_AND_ATTRIBUTES SID_AND_ATTRIBUTES_ARRAY[1];
typedef SID_AND_ATTRIBUTES_ARRAY *PSID_AND_ATTRIBUTES_ARRAY;






typedef struct _ACL {
BYTE AclRevision;
BYTE Sbz1;
WORD AclSize;
WORD AceCount;
WORD Sbz2;
} ACL;
typedef ACL *PACL;
typedef struct _ACE_HEADER {
BYTE AceType;
BYTE AceFlags;
WORD AceSize;
} ACE_HEADER;
typedef ACE_HEADER *PACE_HEADER;
typedef struct _ACCESS_ALLOWED_ACE {
ACE_HEADER Header;
ACCESS_MASK Mask;
DWORD SidStart;
} ACCESS_ALLOWED_ACE;
typedef ACCESS_ALLOWED_ACE *PACCESS_ALLOWED_ACE;
typedef struct _ACCESS_DENIED_ACE {
ACE_HEADER Header;
ACCESS_MASK Mask;
DWORD SidStart;
} ACCESS_DENIED_ACE;
typedef ACCESS_DENIED_ACE *PACCESS_DENIED_ACE;
typedef struct _SYSTEM_AUDIT_ACE {
ACE_HEADER Header;
ACCESS_MASK Mask;
DWORD SidStart;
} SYSTEM_AUDIT_ACE;
typedef SYSTEM_AUDIT_ACE *PSYSTEM_AUDIT_ACE;
typedef struct _SYSTEM_ALARM_ACE {
ACE_HEADER Header;
ACCESS_MASK Mask;
DWORD SidStart;
} SYSTEM_ALARM_ACE;
typedef SYSTEM_ALARM_ACE *PSYSTEM_ALARM_ACE;
typedef struct _ACCESS_ALLOWED_OBJECT_ACE {
ACE_HEADER Header;
ACCESS_MASK Mask;
DWORD Flags;
GUID ObjectType;
GUID InheritedObjectType;
DWORD SidStart;
} ACCESS_ALLOWED_OBJECT_ACE, *PACCESS_ALLOWED_OBJECT_ACE;
typedef struct _ACCESS_DENIED_OBJECT_ACE {
ACE_HEADER Header;
ACCESS_MASK Mask;
DWORD Flags;
GUID ObjectType;
GUID InheritedObjectType;
DWORD SidStart;
} ACCESS_DENIED_OBJECT_ACE, *PACCESS_DENIED_OBJECT_ACE;
typedef struct _SYSTEM_AUDIT_OBJECT_ACE {
ACE_HEADER Header;
ACCESS_MASK Mask;
DWORD Flags;
GUID ObjectType;
GUID InheritedObjectType;
DWORD SidStart;
} SYSTEM_AUDIT_OBJECT_ACE, *PSYSTEM_AUDIT_OBJECT_ACE;
typedef struct _SYSTEM_ALARM_OBJECT_ACE {
ACE_HEADER Header;
ACCESS_MASK Mask;
DWORD Flags;
GUID ObjectType;
GUID InheritedObjectType;
DWORD SidStart;
} SYSTEM_ALARM_OBJECT_ACE, *PSYSTEM_ALARM_OBJECT_ACE;


typedef enum _ACL_INFORMATION_CLASS {
AclRevisionInformation = 1,AclSizeInformation
} ACL_INFORMATION_CLASS;
typedef struct _ACL_REVISION_INFORMATION {
DWORD AclRevision;
} ACL_REVISION_INFORMATION;
typedef ACL_REVISION_INFORMATION *PACL_REVISION_INFORMATION;
typedef struct _ACL_SIZE_INFORMATION {
DWORD AceCount;
DWORD AclBytesInUse;
DWORD AclBytesFree;
} ACL_SIZE_INFORMATION;
typedef ACL_SIZE_INFORMATION *PACL_SIZE_INFORMATION;
typedef WORD SECURITY_DESCRIPTOR_CONTROL, *PSECURITY_DESCRIPTOR_CONTROL;
typedef struct _SECURITY_DESCRIPTOR_RELATIVE {
BYTE Revision;
BYTE Sbz1;
SECURITY_DESCRIPTOR_CONTROL Control;
DWORD Owner;
DWORD Group;
DWORD Sacl;
DWORD Dacl;
} SECURITY_DESCRIPTOR_RELATIVE, *PISECURITY_DESCRIPTOR_RELATIVE;
typedef struct _SECURITY_DESCRIPTOR {
BYTE Revision;
BYTE Sbz1;
SECURITY_DESCRIPTOR_CONTROL Control;
PSID Owner;
PSID Group;
PACL Sacl;
PACL Dacl;
} SECURITY_DESCRIPTOR, *PISECURITY_DESCRIPTOR;
typedef struct _OBJECT_TYPE_LIST {
WORD Level;
WORD Sbz;
GUID *ObjectType;
} OBJECT_TYPE_LIST, *POBJECT_TYPE_LIST;
typedef enum _AUDIT_EVENT_TYPE {
AuditEventObjectAccess,AuditEventDirectoryServiceAccess
} AUDIT_EVENT_TYPE, *PAUDIT_EVENT_TYPE;
typedef struct _PRIVILEGE_SET {
DWORD PrivilegeCount;
DWORD Control;
LUID_AND_ATTRIBUTES Privilege[1];
} PRIVILEGE_SET,* PPRIVILEGE_SET;
typedef enum _SECURITY_IMPERSONATION_LEVEL {
SecurityAnonymous,SecurityIdentification,SecurityImpersonation,SecurityDelegation
} SECURITY_IMPERSONATION_LEVEL,* PSECURITY_IMPERSONATION_LEVEL;
typedef enum _TOKEN_TYPE {
TokenPrimary = 1,TokenImpersonation
} TOKEN_TYPE;
typedef TOKEN_TYPE *PTOKEN_TYPE;
typedef enum _TOKEN_INFORMATION_CLASS {
TokenUser = 1,TokenGroups,TokenPrivileges,TokenOwner,TokenPrimaryGroup,TokenDefaultDacl,TokenSource,TokenType,TokenImpersonationLevel,TokenStatistics,TokenRestrictedSids,TokenSessionId
} TOKEN_INFORMATION_CLASS, *PTOKEN_INFORMATION_CLASS;
typedef struct _TOKEN_USER {
SID_AND_ATTRIBUTES User;
} TOKEN_USER, *PTOKEN_USER;
typedef struct _TOKEN_GROUPS {
DWORD GroupCount;
SID_AND_ATTRIBUTES Groups[1];
} TOKEN_GROUPS, *PTOKEN_GROUPS;
typedef struct _TOKEN_PRIVILEGES {
DWORD PrivilegeCount;
LUID_AND_ATTRIBUTES Privileges[1];
} TOKEN_PRIVILEGES, *PTOKEN_PRIVILEGES;
typedef struct _TOKEN_OWNER {
PSID Owner;
} TOKEN_OWNER, *PTOKEN_OWNER;
typedef struct _TOKEN_PRIMARY_GROUP {
PSID PrimaryGroup;
} TOKEN_PRIMARY_GROUP, *PTOKEN_PRIMARY_GROUP;
typedef struct _TOKEN_DEFAULT_DACL {
PACL DefaultDacl;
} TOKEN_DEFAULT_DACL, *PTOKEN_DEFAULT_DACL;
typedef struct _TOKEN_SOURCE {
CHAR SourceName[8];
LUID SourceIdentifier;
} TOKEN_SOURCE, *PTOKEN_SOURCE;
typedef struct _TOKEN_STATISTICS {
LUID TokenId;
LUID AuthenticationId;
LARGE_INTEGER ExpirationTime;
TOKEN_TYPE TokenType;
SECURITY_IMPERSONATION_LEVEL ImpersonationLevel;
DWORD DynamicCharged;
DWORD DynamicAvailable;
DWORD GroupCount;
DWORD PrivilegeCount;
LUID ModifiedId;
} TOKEN_STATISTICS, *PTOKEN_STATISTICS;
typedef struct _TOKEN_CONTROL {
LUID TokenId;
LUID AuthenticationId;
LUID ModifiedId;
TOKEN_SOURCE TokenSource;
} TOKEN_CONTROL, *PTOKEN_CONTROL;
typedef BOOLEAN SECURITY_CONTEXT_TRACKING_MODE,* PSECURITY_CONTEXT_TRACKING_MODE;
typedef struct _SECURITY_QUALITY_OF_SERVICE {
DWORD Length;
SECURITY_IMPERSONATION_LEVEL ImpersonationLevel;
SECURITY_CONTEXT_TRACKING_MODE ContextTrackingMode;
BOOLEAN EffectiveOnly;
} SECURITY_QUALITY_OF_SERVICE,* PSECURITY_QUALITY_OF_SERVICE;
typedef struct _SE_IMPERSONATION_STATE {
PACCESS_TOKEN Token;
BOOLEAN CopyOnOpen;
BOOLEAN EffectiveOnly;
SECURITY_IMPERSONATION_LEVEL Level;
} SE_IMPERSONATION_STATE, *PSE_IMPERSONATION_STATE;
typedef DWORD SECURITY_INFORMATION, *PSECURITY_INFORMATION;
typedef struct _NT_TIB {
struct _EXCEPTION_REGISTRATION_RECORD *ExceptionList;
PVOID StackBase;
PVOID StackLimit;
PVOID SubSystemTib;
union {
PVOID FiberData;
DWORD Version;
};
PVOID ArbitraryUserPointer;
struct _NT_TIB *Self;
} NT_TIB;
typedef NT_TIB *PNT_TIB;
	// 4366 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _QUOTA_LIMITS {
SIZE_T PagedPoolLimit;
SIZE_T NonPagedPoolLimit;
DWORD MinimumWorkingSetSize;
DWORD MaximumWorkingSetSize;
SIZE_T PagefileLimit;
LARGE_INTEGER TimeLimit;
} QUOTA_LIMITS;
typedef QUOTA_LIMITS *PQUOTA_LIMITS;
typedef struct _JOBOBJECT_BASIC_ACCOUNTING_INFORMATION {
LARGE_INTEGER TotalUserTime;
LARGE_INTEGER TotalKernelTime;
LARGE_INTEGER ThisPeriodTotalUserTime;
LARGE_INTEGER ThisPeriodTotalKernelTime;
DWORD TotalPageFaultCount;
DWORD TotalProcesses;
DWORD ActiveProcesses;
DWORD TotalTerminatedProcesses;
} JOBOBJECT_BASIC_ACCOUNTING_INFORMATION, *PJOBOBJECT_BASIC_ACCOUNTING_INFORMATION;
typedef struct _JOBOBJECT_BASIC_LIMIT_INFORMATION {
LARGE_INTEGER PerProcessUserTimeLimit;
LARGE_INTEGER PerJobUserTimeLimit;
DWORD LimitFlags;
DWORD MinimumWorkingSetSize;
DWORD MaximumWorkingSetSize;
DWORD ActiveProcessLimit;
DWORD Affinity;
DWORD PriorityClass;
} JOBOBJECT_BASIC_LIMIT_INFORMATION, *PJOBOBJECT_BASIC_LIMIT_INFORMATION;
typedef struct _JOBOBJECT_BASIC_PROCESS_ID_LIST {
DWORD NumberOfAssignedProcesses;
DWORD NumberOfProcessIdsInList;
UINT_PTR ProcessIdList[1];
} JOBOBJECT_BASIC_PROCESS_ID_LIST, *PJOBOBJECT_BASIC_PROCESS_ID_LIST;
typedef struct _JOBOBJECT_BASIC_UI_RESTRICTIONS {
DWORD UIRestrictionsClass;
} JOBOBJECT_BASIC_UI_RESTRICTIONS, *PJOBOBJECT_BASIC_UI_RESTRICTIONS;
typedef struct _JOBOBJECT_SECURITY_LIMIT_INFORMATION {
DWORD SecurityLimitFlags ;
HANDLE JobToken ;
PTOKEN_GROUPS SidsToDisable ;
PTOKEN_PRIVILEGES PrivilegesToDelete ;
PTOKEN_GROUPS RestrictedSids ;
} JOBOBJECT_SECURITY_LIMIT_INFORMATION, *PJOBOBJECT_SECURITY_LIMIT_INFORMATION ;
typedef struct _JOBOBJECT_END_OF_JOB_TIME_INFORMATION {
DWORD EndOfJobTimeAction;
} JOBOBJECT_END_OF_JOB_TIME_INFORMATION, *PJOBOBJECT_END_OF_JOB_TIME_INFORMATION;
typedef struct _JOBOBJECT_ASSOCIATE_COMPLETION_PORT {
PVOID CompletionKey;
HANDLE CompletionPort;
} JOBOBJECT_ASSOCIATE_COMPLETION_PORT, *PJOBOBJECT_ASSOCIATE_COMPLETION_PORT;
typedef enum _JOBOBJECTINFOCLASS {
JobObjectBasicAccountingInformation = 1,JobObjectBasicLimitInformation,JobObjectBasicProcessIdList,JobObjectBasicUIRestrictions,JobObjectSecurityLimitInformation,JobObjectEndOfJobTimeInformation,JobObjectAssociateCompletionPortInformation,MaxJobObjectInfoClass
} JOBOBJECTINFOCLASS;
typedef struct _MEMORY_BASIC_INFORMATION {
PVOID BaseAddress;
PVOID AllocationBase;
DWORD AllocationProtect;
SIZE_T RegionSize;
DWORD State;
DWORD Protect;
DWORD Type;
} MEMORY_BASIC_INFORMATION, *PMEMORY_BASIC_INFORMATION;
typedef struct _MEMORY_BASIC_INFORMATION_VLM {
union {
PVOID64 BaseAddress;
ULONGLONG BaseAddressAsUlongLong;
};
union {
PVOID64 AllocationBase;
ULONGLONG AllocationBaseAsUlongLong;
};
ULONGLONG RegionSize;
DWORD AllocationProtect;
DWORD State;
DWORD Protect;
DWORD Type;
} MEMORY_BASIC_INFORMATION_VLM, *PMEMORY_BASIC_INFORMATION_VLM;
typedef struct _FILE_NOTIFY_INFORMATION {
DWORD NextEntryOffset;
DWORD Action;
DWORD FileNameLength;
WCHAR FileName[1];
} FILE_NOTIFY_INFORMATION, *PFILE_NOTIFY_INFORMATION;
typedef union _FILE_SEGMENT_ELEMENT {
PVOID64 Buffer;
ULONGLONG Alignment;
}FILE_SEGMENT_ELEMENT, *PFILE_SEGMENT_ELEMENT;
typedef struct _REPARSE_DATA_BUFFER {
DWORD ReparseTag;
WORD ReparseDataLength;
WORD Reserved;
union {
struct {
WORD SubstituteNameOffset;
WORD SubstituteNameLength;
WORD PrintNameOffset;
WORD PrintNameLength;
WCHAR PathBuffer[1];
} SymbolicLinkReparseBuffer;
struct {
WORD SubstituteNameOffset;
WORD SubstituteNameLength;
WORD PrintNameOffset;
WORD PrintNameLength;
WCHAR PathBuffer[1];
} MountPointReparseBuffer;
struct {
BYTE DataBuffer[1];
} GenericReparseBuffer;
};
} REPARSE_DATA_BUFFER, *PREPARSE_DATA_BUFFER;
typedef struct _REPARSE_GUID_DATA_BUFFER {
DWORD ReparseTag;
WORD ReparseDataLength;
WORD Reserved;
GUID ReparseGuid;
struct {
BYTE DataBuffer[1];
} GenericReparseBuffer;
} REPARSE_GUID_DATA_BUFFER, *PREPARSE_GUID_DATA_BUFFER;
typedef struct _REPARSE_POINT_INFORMATION {
WORD ReparseDataLength;
WORD UnparsedNameLength;
} REPARSE_POINT_INFORMATION, *PREPARSE_POINT_INFORMATION;
typedef DWORD EXECUTION_STATE;
typedef enum {
LT_DONT_CARE,LT_LOWEST_LATENCY
} LATENCY_TIME;
typedef struct _POWER_DEVICE_TIMEOUTS {
DWORD ConservationIdleTime;
DWORD PerformanceIdleTime;
} POWER_DEVICE_TIMEOUTS, *PPOWER_DEVICE_TIMEOUTS;
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack4.h"
#pragma warning(disable:4103)
#pragma pack(push)
	// 28 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack4.h"
#pragma pack(4)
	// 32 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack4.h"
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack4.h"
	// 4950 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
#pragma warning(disable:4103)
#pragma pack(push)
	// 28 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
#pragma pack(2)
	// 32 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
	// 4958 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 4968 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _IMAGE_DOS_HEADER {
WORD e_magic;
WORD e_cblp;
WORD e_cp;
WORD e_crlc;
WORD e_cparhdr;
WORD e_minalloc;
WORD e_maxalloc;
WORD e_ss;
WORD e_sp;
WORD e_csum;
WORD e_ip;
WORD e_cs;
WORD e_lfarlc;
WORD e_ovno;
WORD e_res[4];
WORD e_oemid;
WORD e_oeminfo;
WORD e_res2[10];
LONG e_lfanew;
} IMAGE_DOS_HEADER, *PIMAGE_DOS_HEADER;
typedef struct _IMAGE_OS2_HEADER {
WORD ne_magic;
CHAR ne_ver;
CHAR ne_rev;
WORD ne_enttab;
WORD ne_cbenttab;
LONG ne_crc;
WORD ne_flags;
WORD ne_autodata;
WORD ne_heap;
WORD ne_stack;
LONG ne_csip;
LONG ne_sssp;
WORD ne_cseg;
WORD ne_cmod;
WORD ne_cbnrestab;
WORD ne_segtab;
WORD ne_rsrctab;
WORD ne_restab;
WORD ne_modtab;
WORD ne_imptab;
LONG ne_nrestab;
WORD ne_cmovent;
WORD ne_align;
WORD ne_cres;
BYTE ne_exetyp;
BYTE ne_flagsothers;
WORD ne_pretthunks;
WORD ne_psegrefbytes;
WORD ne_swaparea;
WORD ne_expver;
} IMAGE_OS2_HEADER, *PIMAGE_OS2_HEADER;
typedef struct _IMAGE_VXD_HEADER {
WORD e32_magic;
BYTE e32_border;
BYTE e32_worder;
DWORD e32_level;
WORD e32_cpu;
WORD e32_os;
DWORD e32_ver;
DWORD e32_mflags;
DWORD e32_mpages;
DWORD e32_startobj;
DWORD e32_eip;
DWORD e32_stackobj;
DWORD e32_esp;
DWORD e32_pagesize;
DWORD e32_lastpagesize;
DWORD e32_fixupsize;
DWORD e32_fixupsum;
DWORD e32_ldrsize;
DWORD e32_ldrsum;
DWORD e32_objtab;
DWORD e32_objcnt;
DWORD e32_objmap;
DWORD e32_itermap;
DWORD e32_rsrctab;
DWORD e32_rsrccnt;
DWORD e32_restab;
DWORD e32_enttab;
DWORD e32_dirtab;
DWORD e32_dircnt;
DWORD e32_fpagetab;
DWORD e32_frectab;
DWORD e32_impmod;
DWORD e32_impmodcnt;
DWORD e32_impproc;
DWORD e32_pagesum;
DWORD e32_datapage;
DWORD e32_preload;
DWORD e32_nrestab;
DWORD e32_cbnrestab;
DWORD e32_nressum;
DWORD e32_autodata;
DWORD e32_debuginfo;
DWORD e32_debuglen;
DWORD e32_instpreload;
DWORD e32_instdemand;
DWORD e32_heapsize;
BYTE e32_res3[12];
DWORD e32_winresoff;
DWORD e32_winreslen;
WORD e32_devid;
WORD e32_ddkver;
} IMAGE_VXD_HEADER, *PIMAGE_VXD_HEADER;
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
#pragma warning(disable:4103)
#pragma pack(pop)
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 36 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 37 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 5080 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 5081 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _IMAGE_FILE_HEADER {
WORD Machine;
WORD NumberOfSections;
DWORD TimeDateStamp;
DWORD PointerToSymbolTable;
DWORD NumberOfSymbols;
WORD SizeOfOptionalHeader;
WORD Characteristics;
} IMAGE_FILE_HEADER, *PIMAGE_FILE_HEADER;
typedef struct _IMAGE_DATA_DIRECTORY {
DWORD VirtualAddress;
DWORD Size;
} IMAGE_DATA_DIRECTORY, *PIMAGE_DATA_DIRECTORY;
typedef struct _IMAGE_OPTIONAL_HEADER {
WORD Magic;
BYTE MajorLinkerVersion;
BYTE MinorLinkerVersion;
DWORD SizeOfCode;
DWORD SizeOfInitializedData;
DWORD SizeOfUninitializedData;
DWORD AddressOfEntryPoint;
DWORD BaseOfCode;
DWORD BaseOfData;
DWORD ImageBase;
DWORD SectionAlignment;
DWORD FileAlignment;
WORD MajorOperatingSystemVersion;
WORD MinorOperatingSystemVersion;
WORD MajorImageVersion;
WORD MinorImageVersion;
WORD MajorSubsystemVersion;
WORD MinorSubsystemVersion;
DWORD Win32VersionValue;
DWORD SizeOfImage;
DWORD SizeOfHeaders;
DWORD CheckSum;
WORD Subsystem;
WORD DllCharacteristics;
DWORD SizeOfStackReserve;
DWORD SizeOfStackCommit;
DWORD SizeOfHeapReserve;
DWORD SizeOfHeapCommit;
DWORD LoaderFlags;
DWORD NumberOfRvaAndSizes;
IMAGE_DATA_DIRECTORY DataDirectory[16];
} IMAGE_OPTIONAL_HEADER32, *PIMAGE_OPTIONAL_HEADER32;
typedef struct _IMAGE_ROM_OPTIONAL_HEADER {
WORD Magic;
BYTE MajorLinkerVersion;
BYTE MinorLinkerVersion;
DWORD SizeOfCode;
DWORD SizeOfInitializedData;
DWORD SizeOfUninitializedData;
DWORD AddressOfEntryPoint;
DWORD BaseOfCode;
DWORD BaseOfData;
DWORD BaseOfBss;
DWORD GprMask;
DWORD CprMask[4];
DWORD GpValue;
} IMAGE_ROM_OPTIONAL_HEADER, *PIMAGE_ROM_OPTIONAL_HEADER;
typedef struct _IMAGE_OPTIONAL_HEADER64 {
WORD Magic;
BYTE MajorLinkerVersion;
BYTE MinorLinkerVersion;
DWORD SizeOfCode;
DWORD SizeOfInitializedData;
DWORD SizeOfUninitializedData;
DWORD AddressOfEntryPoint;
DWORD BaseOfCode;
ULONGLONG ImageBase;
DWORD SectionAlignment;
DWORD FileAlignment;
WORD MajorOperatingSystemVersion;
WORD MinorOperatingSystemVersion;
WORD MajorImageVersion;
WORD MinorImageVersion;
WORD MajorSubsystemVersion;
WORD MinorSubsystemVersion;
DWORD Win32VersionValue;
DWORD SizeOfImage;
DWORD SizeOfHeaders;
DWORD CheckSum;
WORD Subsystem;
WORD DllCharacteristics;
ULONGLONG SizeOfStackReserve;
ULONGLONG SizeOfStackCommit;
ULONGLONG SizeOfHeapReserve;
ULONGLONG SizeOfHeapCommit;
DWORD LoaderFlags;
DWORD NumberOfRvaAndSizes;
IMAGE_DATA_DIRECTORY DataDirectory[16];
} IMAGE_OPTIONAL_HEADER64, *PIMAGE_OPTIONAL_HEADER64;
typedef IMAGE_OPTIONAL_HEADER32 IMAGE_OPTIONAL_HEADER;
typedef PIMAGE_OPTIONAL_HEADER32 PIMAGE_OPTIONAL_HEADER;
	// 5262 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _IMAGE_NT_HEADERS64 {
DWORD Signature;
IMAGE_FILE_HEADER FileHeader;
IMAGE_OPTIONAL_HEADER64 OptionalHeader;
} IMAGE_NT_HEADERS64, *PIMAGE_NT_HEADERS64;
typedef struct _IMAGE_NT_HEADERS {
DWORD Signature;
IMAGE_FILE_HEADER FileHeader;
IMAGE_OPTIONAL_HEADER32 OptionalHeader;
} IMAGE_NT_HEADERS32, *PIMAGE_NT_HEADERS32;
typedef struct _IMAGE_ROM_HEADERS {
IMAGE_FILE_HEADER FileHeader;
IMAGE_ROM_OPTIONAL_HEADER OptionalHeader;
} IMAGE_ROM_HEADERS, *PIMAGE_ROM_HEADERS;
typedef IMAGE_NT_HEADERS32 IMAGE_NT_HEADERS;
typedef PIMAGE_NT_HEADERS32 PIMAGE_NT_HEADERS;
	// 5301 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _IMAGE_SECTION_HEADER {
BYTE Name[8];
union {
DWORD PhysicalAddress;
DWORD VirtualSize;
} Misc;
DWORD VirtualAddress;
DWORD SizeOfRawData;
DWORD PointerToRawData;
DWORD PointerToRelocations;
DWORD PointerToLinenumbers;
WORD NumberOfRelocations;
WORD NumberOfLinenumbers;
DWORD Characteristics;
} IMAGE_SECTION_HEADER, *PIMAGE_SECTION_HEADER;
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
#pragma warning(disable:4103)
#pragma pack(push)
	// 28 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
#pragma pack(2)
	// 32 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
	// 5427 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 5428 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _IMAGE_SYMBOL {
union {
BYTE ShortName[8];
struct {
DWORD Short;
DWORD Long;
} Name;
PBYTE LongName[2];
} N;
DWORD Value;
SHORT SectionNumber;
WORD Type;
BYTE StorageClass;
BYTE NumberOfAuxSymbols;
} IMAGE_SYMBOL;
typedef IMAGE_SYMBOL *PIMAGE_SYMBOL;
	// 5545 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 5550 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 5556 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 5561 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 5565 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 5568 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef union _IMAGE_AUX_SYMBOL {
struct {
DWORD TagIndex;
union {
struct {
WORD Linenumber;
WORD Size;
} LnSz;
DWORD TotalSize;
} Misc;
union {
struct {
DWORD PointerToLinenumber;
DWORD PointerToNextFunction;
} Function;
struct {
WORD Dimension[4];
} Array;
} FcnAry;
WORD TvIndex;
} Sym;
struct {
BYTE Name[18];
} File;
struct {
DWORD Length;
WORD NumberOfRelocations;
WORD NumberOfLinenumbers;
DWORD CheckSum;
SHORT Number;
BYTE Selection;
} Section;
} IMAGE_AUX_SYMBOL;
typedef IMAGE_AUX_SYMBOL *PIMAGE_AUX_SYMBOL;
typedef struct _IMAGE_RELOCATION {
union {
DWORD VirtualAddress;
DWORD RelocCount;
};
DWORD SymbolTableIndex;
WORD Type;
} IMAGE_RELOCATION;
typedef IMAGE_RELOCATION *PIMAGE_RELOCATION;
typedef struct _IMAGE_LINENUMBER {
union {
DWORD SymbolTableIndex;
DWORD VirtualAddress;
} Type;
WORD Linenumber;
} IMAGE_LINENUMBER;
typedef IMAGE_LINENUMBER *PIMAGE_LINENUMBER;
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
#pragma warning(disable:4103)
#pragma pack(pop)
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 36 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 37 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 5864 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 5865 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _IMAGE_BASE_RELOCATION {
DWORD VirtualAddress;
DWORD SizeOfBlock;
} IMAGE_BASE_RELOCATION;
typedef IMAGE_BASE_RELOCATION* PIMAGE_BASE_RELOCATION;
typedef struct _IMAGE_ARCHIVE_MEMBER_HEADER {
BYTE Name[16];
BYTE Date[12];
BYTE UserID[6];
BYTE GroupID[6];
BYTE Mode[8];
BYTE Size[10];
BYTE EndHeader[2];
} IMAGE_ARCHIVE_MEMBER_HEADER, *PIMAGE_ARCHIVE_MEMBER_HEADER;
typedef struct _IMAGE_EXPORT_DIRECTORY {
DWORD Characteristics;
DWORD TimeDateStamp;
WORD MajorVersion;
WORD MinorVersion;
DWORD Name;
DWORD Base;
DWORD NumberOfFunctions;
DWORD NumberOfNames;
DWORD AddressOfFunctions;
DWORD AddressOfNames;
DWORD AddressOfNameOrdinals;
} IMAGE_EXPORT_DIRECTORY, *PIMAGE_EXPORT_DIRECTORY;
typedef struct _IMAGE_IMPORT_BY_NAME {
WORD Hint;
BYTE Name[1];
} IMAGE_IMPORT_BY_NAME, *PIMAGE_IMPORT_BY_NAME;
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack8.h"
#pragma warning(disable:4103)
#pragma pack(push)
	// 28 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack8.h"
#pragma pack(8)
	// 32 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack8.h"
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack8.h"
	// 5953 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _IMAGE_THUNK_DATA64 {
union {
PBYTE ForwarderString;
PDWORD Function;
ULONGLONG Ordinal;
PIMAGE_IMPORT_BY_NAME AddressOfData;
} u1;
} IMAGE_THUNK_DATA64;
typedef IMAGE_THUNK_DATA64* PIMAGE_THUNK_DATA64;
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
#pragma warning(disable:4103)
#pragma pack(pop)
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 36 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 37 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 5965 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _IMAGE_THUNK_DATA32 {
union {
PBYTE ForwarderString;
PDWORD Function;
DWORD Ordinal;
PIMAGE_IMPORT_BY_NAME AddressOfData;
} u1;
} IMAGE_THUNK_DATA32;
typedef IMAGE_THUNK_DATA32* PIMAGE_THUNK_DATA32;
typedef void
(__stdcall *PIMAGE_TLS_CALLBACK) (
PVOID DllHandle,DWORD Reason,PVOID Reserved);
typedef struct _IMAGE_TLS_DIRECTORY64 {
ULONGLONG StartAddressOfRawData;
ULONGLONG EndAddressOfRawData;
PDWORD AddressOfIndex;
PIMAGE_TLS_CALLBACK *AddressOfCallBacks;
DWORD SizeOfZeroFill;
DWORD Characteristics;
} IMAGE_TLS_DIRECTORY64;
typedef IMAGE_TLS_DIRECTORY64* PIMAGE_TLS_DIRECTORY64;
typedef struct _IMAGE_TLS_DIRECTORY32 {
DWORD StartAddressOfRawData;
DWORD EndAddressOfRawData;
PDWORD AddressOfIndex;
PIMAGE_TLS_CALLBACK *AddressOfCallBacks;
DWORD SizeOfZeroFill;
DWORD Characteristics;
} IMAGE_TLS_DIRECTORY32;
typedef IMAGE_TLS_DIRECTORY32* PIMAGE_TLS_DIRECTORY32;
typedef IMAGE_THUNK_DATA32 IMAGE_THUNK_DATA;
typedef PIMAGE_THUNK_DATA32 PIMAGE_THUNK_DATA;
typedef IMAGE_TLS_DIRECTORY32 IMAGE_TLS_DIRECTORY;
typedef PIMAGE_TLS_DIRECTORY32 PIMAGE_TLS_DIRECTORY;
	// 6031 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _IMAGE_IMPORT_DESCRIPTOR {
union {
DWORD Characteristics;
DWORD OriginalFirstThunk;
};
DWORD TimeDateStamp;
DWORD ForwarderChain;
DWORD Name;
DWORD FirstThunk;
} IMAGE_IMPORT_DESCRIPTOR;
typedef IMAGE_IMPORT_DESCRIPTOR *PIMAGE_IMPORT_DESCRIPTOR;
typedef struct _IMAGE_BOUND_IMPORT_DESCRIPTOR {
DWORD TimeDateStamp;
WORD OffsetModuleName;
WORD NumberOfModuleForwarderRefs;
} IMAGE_BOUND_IMPORT_DESCRIPTOR, *PIMAGE_BOUND_IMPORT_DESCRIPTOR;
typedef struct _IMAGE_BOUND_FORWARDER_REF {
DWORD TimeDateStamp;
WORD OffsetModuleName;
WORD Reserved;
} IMAGE_BOUND_FORWARDER_REF, *PIMAGE_BOUND_FORWARDER_REF;
typedef struct _IMAGE_STUB_DIRECTORY {
DWORD SecondaryImportAddressTable;
WORD ExpectedISA[2];
DWORD StubAddressTable[2];
} IMAGE_STUB_DIRECTORY, *PIMAGE_STUB_DIRECTORY;
typedef struct _IMAGE_RESOURCE_DIRECTORY {
DWORD Characteristics;
DWORD TimeDateStamp;
WORD MajorVersion;
WORD MinorVersion;
WORD NumberOfNamedEntries;
WORD NumberOfIdEntries;
} IMAGE_RESOURCE_DIRECTORY, *PIMAGE_RESOURCE_DIRECTORY;
typedef struct _IMAGE_RESOURCE_DIRECTORY_ENTRY {
union {
struct {
DWORD NameOffset:31;
DWORD NameIsString:1;
};
DWORD Name;
WORD Id;
};
union {
DWORD OffsetToData;
struct {
DWORD OffsetToDirectory:31;
DWORD DataIsDirectory:1;
};
};
} IMAGE_RESOURCE_DIRECTORY_ENTRY, *PIMAGE_RESOURCE_DIRECTORY_ENTRY;
typedef struct _IMAGE_RESOURCE_DIRECTORY_STRING {
WORD Length;
CHAR NameString[ 1 ];
} IMAGE_RESOURCE_DIRECTORY_STRING, *PIMAGE_RESOURCE_DIRECTORY_STRING;
typedef struct _IMAGE_RESOURCE_DIR_STRING_U {
WORD Length;
WCHAR NameString[ 1 ];
} IMAGE_RESOURCE_DIR_STRING_U, *PIMAGE_RESOURCE_DIR_STRING_U;
typedef struct _IMAGE_RESOURCE_DATA_ENTRY {
DWORD OffsetToData;
DWORD Size;
DWORD CodePage;
DWORD Reserved;
} IMAGE_RESOURCE_DATA_ENTRY, *PIMAGE_RESOURCE_DATA_ENTRY;
typedef struct _IMAGE_LOAD_CONFIG_DIRECTORY {
DWORD Characteristics;
DWORD TimeDateStamp;
WORD MajorVersion;
WORD MinorVersion;
DWORD GlobalFlagsClear;
DWORD GlobalFlagsSet;
DWORD CriticalSectionDefaultTimeout;
DWORD DeCommitFreeBlockThreshold;
DWORD DeCommitTotalFreeThreshold;
PVOID LockPrefixTable;
DWORD MaximumAllocationSize;
DWORD VirtualMemoryThreshold;
DWORD ProcessHeapFlags;
DWORD ProcessAffinityMask;
WORD CSDVersion;
WORD Reserved1;
PVOID EditList;
DWORD Reserved[ 1 ];
} IMAGE_LOAD_CONFIG_DIRECTORY, *PIMAGE_LOAD_CONFIG_DIRECTORY;
typedef struct _IMAGE_IA64_RUNTIME_FUNCTION_ENTRY {
DWORD BeginAddress;
DWORD EndAddress;
DWORD UnwindInfoAddress;
} IMAGE_IA64_RUNTIME_FUNCTION_ENTRY, *PIMAGE_IA64_RUNTIME_FUNCTION_ENTRY;
typedef struct _IMAGE_ALPHA_RUNTIME_FUNCTION_ENTRY {
DWORD BeginAddress;
DWORD EndAddress;
DWORD ExceptionHandler;
DWORD HandlerData;
DWORD PrologEndAddress;
} IMAGE_ALPHA_RUNTIME_FUNCTION_ENTRY, *PIMAGE_ALPHA_RUNTIME_FUNCTION_ENTRY;
typedef struct _IMAGE_ALPHA64_RUNTIME_FUNCTION_ENTRY {
ULONGLONG BeginAddress;
ULONGLONG EndAddress;
ULONGLONG ExceptionHandler;
ULONGLONG HandlerData;
ULONGLONG PrologEndAddress;
} IMAGE_ALPHA64_RUNTIME_FUNCTION_ENTRY, *PIMAGE_ALPHA64_RUNTIME_FUNCTION_ENTRY;
typedef IMAGE_ALPHA64_RUNTIME_FUNCTION_ENTRY IMAGE_AXP64_RUNTIME_FUNCTION_ENTRY;
typedef PIMAGE_ALPHA64_RUNTIME_FUNCTION_ENTRY PIMAGE_AXP64_RUNTIME_FUNCTION_ENTRY;
typedef struct _IMAGE_CE_RUNTIME_FUNCTION_ENTRY {
DWORD FuncStart;
DWORD PrologLen : 8;
DWORD FuncLen : 22;
DWORD ThirtyTwoBit : 1;
DWORD ExceptionFlag : 1;
} IMAGE_CE_RUNTIME_FUNCTION_ENTRY,* PIMAGE_CE_RUNTIME_FUNCTION_ENTRY;
	// 6263 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef IMAGE_ALPHA_RUNTIME_FUNCTION_ENTRY IMAGE_RUNTIME_FUNCTION_ENTRY;
typedef PIMAGE_ALPHA_RUNTIME_FUNCTION_ENTRY PIMAGE_RUNTIME_FUNCTION_ENTRY;
	// 6268 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _IMAGE_DEBUG_DIRECTORY {
DWORD Characteristics;
DWORD TimeDateStamp;
WORD MajorVersion;
WORD MinorVersion;
DWORD Type;
DWORD SizeOfData;
DWORD AddressOfRawData;
DWORD PointerToRawData;
} IMAGE_DEBUG_DIRECTORY, *PIMAGE_DEBUG_DIRECTORY;
typedef struct _IMAGE_COFF_SYMBOLS_HEADER {
DWORD NumberOfSymbols;
DWORD LvaToFirstSymbol;
DWORD NumberOfLinenumbers;
DWORD LvaToFirstLinenumber;
DWORD RvaToFirstByteOfCode;
DWORD RvaToLastByteOfCode;
DWORD RvaToFirstByteOfData;
DWORD RvaToLastByteOfData;
} IMAGE_COFF_SYMBOLS_HEADER, *PIMAGE_COFF_SYMBOLS_HEADER;
typedef struct _FPO_DATA {
DWORD ulOffStart;
DWORD cbProcSize;
DWORD cdwLocals;
WORD cdwParams;
WORD cbProlog : 8;
WORD cbRegs : 3;
WORD fHasSEH : 1;
WORD fUseBP : 1;
WORD reserved : 1;
WORD cbFrame : 2;
} FPO_DATA, *PFPO_DATA;
typedef struct _IMAGE_DEBUG_MISC {
DWORD DataType;
DWORD Length;
BOOLEAN Unicode;
BYTE Reserved[ 3 ];
BYTE Data[ 1 ];
} IMAGE_DEBUG_MISC, *PIMAGE_DEBUG_MISC;
	// 6355 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _IMAGE_FUNCTION_ENTRY {
DWORD StartingAddress;
DWORD EndingAddress;
DWORD EndOfPrologue;
} IMAGE_FUNCTION_ENTRY, *PIMAGE_FUNCTION_ENTRY;
	// 6363 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _IMAGE_FUNCTION_ENTRY64 {
ULONGLONG StartingAddress;
ULONGLONG EndingAddress;
ULONGLONG EndOfPrologue;
} IMAGE_FUNCTION_ENTRY64, *PIMAGE_FUNCTION_ENTRY64;
typedef struct _IMAGE_SEPARATE_DEBUG_HEADER {
WORD Signature;
WORD Flags;
WORD Machine;
WORD Characteristics;
DWORD TimeDateStamp;
DWORD CheckSum;
DWORD ImageBase;
DWORD SizeOfImage;
DWORD NumberOfSections;
DWORD ExportedNamesSize;
DWORD DebugDirectorySize;
DWORD SectionAlignment;
DWORD Reserved[2];
} IMAGE_SEPARATE_DEBUG_HEADER, *PIMAGE_SEPARATE_DEBUG_HEADER;
	// 6410 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _ImageArchitectureHeader {
unsigned int AmaskValue: 1;
int :7;
unsigned int AmaskShift: 8;
int :16;
DWORD FirstEntryRVA;
} IMAGE_ARCHITECTURE_HEADER, *PIMAGE_ARCHITECTURE_HEADER;
typedef struct _ImageArchitectureEntry {
DWORD FixupInstRVA;
DWORD NewInst;
} IMAGE_ARCHITECTURE_ENTRY, *PIMAGE_ARCHITECTURE_ENTRY;
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
#pragma warning(disable:4103)
#pragma pack(pop)
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 36 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 37 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 6438 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct IMPORT_OBJECT_HEADER {
WORD Sig1;
WORD Sig2;
WORD Version;
WORD Machine;
DWORD TimeDateStamp;
DWORD SizeOfData;
union {
WORD Ordinal;
WORD Hint;
};
WORD Type : 2;
WORD NameType : 3;
WORD Reserved : 11;
} IMPORT_OBJECT_HEADER;
typedef enum IMPORT_OBJECT_TYPE
{
IMPORT_OBJECT_CODE = 0,IMPORT_OBJECT_DATA = 1,IMPORT_OBJECT_CONST = 2} IMPORT_OBJECT_TYPE;
typedef enum IMPORT_OBJECT_NAME_TYPE
{
IMPORT_OBJECT_ORDINAL = 0,IMPORT_OBJECT_NAME = 1,IMPORT_OBJECT_NAME_NO_PREFIX = 2,IMPORT_OBJECT_NAME_UNDECORATE = 3} IMPORT_OBJECT_NAME_TYPE;
	// 6494 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
__declspec(dllimport) SIZE_T __stdcall RtlCompareMemory (
const void *Source1,const void *Source2,SIZE_T Length);
	// 6597 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 6605 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 6636 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
typedef struct _MESSAGE_RESOURCE_ENTRY {
WORD Length;
WORD Flags;
BYTE Text[ 1 ];
} MESSAGE_RESOURCE_ENTRY, *PMESSAGE_RESOURCE_ENTRY;
typedef struct _MESSAGE_RESOURCE_BLOCK {
DWORD LowId;
DWORD HighId;
DWORD OffsetToEntries;
} MESSAGE_RESOURCE_BLOCK, *PMESSAGE_RESOURCE_BLOCK;
typedef struct _MESSAGE_RESOURCE_DATA {
DWORD NumberOfBlocks;
MESSAGE_RESOURCE_BLOCK Blocks[ 1 ];
} MESSAGE_RESOURCE_DATA, *PMESSAGE_RESOURCE_DATA;
typedef struct _RTL_CRITICAL_SECTION_DEBUG {
WORD Type;
WORD CreatorBackTraceIndex;
struct _RTL_CRITICAL_SECTION *CriticalSection;
LIST_ENTRY ProcessLocksList;
DWORD EntryCount;
DWORD ContentionCount;
DWORD Spare[ 2 ];
} RTL_CRITICAL_SECTION_DEBUG, *PRTL_CRITICAL_SECTION_DEBUG, RTL_RESOURCE_DEBUG, *PRTL_RESOURCE_DEBUG;
typedef struct _RTL_CRITICAL_SECTION {
PRTL_CRITICAL_SECTION_DEBUG DebugInfo;
LONG LockCount;
LONG RecursionCount;
HANDLE OwningThread;
HANDLE LockSemaphore;
DWORD SpinCount;
} RTL_CRITICAL_SECTION, *PRTL_CRITICAL_SECTION;
typedef void (__stdcall* WAITORTIMERCALLBACKFUNC) (PVOID, BOOLEAN );
typedef void (__stdcall* WORKERCALLBACKFUNC) (PVOID );
typedef struct _EVENTLOGRECORD {
DWORD Length;
DWORD Reserved; 
DWORD RecordNumber;
DWORD TimeGenerated;
DWORD TimeWritten;
DWORD EventID;
WORD EventType;
WORD NumStrings;
WORD EventCategory;
WORD ReservedFlags;
DWORD ClosingRecordNumber;
DWORD StringOffset;
DWORD UserSidLength;
DWORD UserSidOffset;
DWORD DataLength;
DWORD DataOffset;
} EVENTLOGRECORD, *PEVENTLOGRECORD;
#pragma warning(disable : 4200)
typedef struct _EVENTSFORLOGFILE{
DWORD ulSize;
WCHAR szLogicalLogFile[256];
DWORD ulNumRecords;
EVENTLOGRECORD pEventLogRecords[];
}EVENTSFORLOGFILE, *PEVENTSFORLOGFILE;
typedef struct _PACKEDEVENTINFO{
DWORD ulSize;
DWORD ulNumEventsForLogFile;
DWORD ulOffsets[];
}PACKEDEVENTINFO, *PPACKEDEVENTINFO;
//#pragma warning(default : 4200)


typedef enum _CM_SERVICE_NODE_TYPE {
DriverType = 0x00000001,FileSystemType = 0x00000002,Win32ServiceOwnProcess = 0x00000010,Win32ServiceShareProcess = 0x00000020,AdapterType = 0x00000004,RecognizerType = 0x00000008
} SERVICE_NODE_TYPE;
typedef enum _CM_SERVICE_LOAD_TYPE {
BootLoad = 0x00000000,SystemLoad = 0x00000001,AutoLoad = 0x00000002,DemandLoad = 0x00000003,DisableLoad = 0x00000004
} SERVICE_LOAD_TYPE;
typedef enum _CM_ERROR_CONTROL_TYPE {
IgnoreError = 0x00000000,NormalError = 0x00000001,SevereError = 0x00000002,CriticalError = 0x00000003
} SERVICE_ERROR_TYPE;
typedef struct _TAPE_ERASE {
DWORD Type;
BOOLEAN Immediate;
} TAPE_ERASE, *PTAPE_ERASE;
typedef struct _TAPE_PREPARE {
DWORD Operation;
BOOLEAN Immediate;
} TAPE_PREPARE, *PTAPE_PREPARE;
typedef struct _TAPE_WRITE_MARKS {
DWORD Type;
DWORD Count;
BOOLEAN Immediate;
} TAPE_WRITE_MARKS, *PTAPE_WRITE_MARKS;
typedef struct _TAPE_GET_POSITION {
DWORD Type;
DWORD Partition;
LARGE_INTEGER Offset;
} TAPE_GET_POSITION, *PTAPE_GET_POSITION;
typedef struct _TAPE_SET_POSITION {
DWORD Method;
DWORD Partition;
LARGE_INTEGER Offset;
BOOLEAN Immediate;
} TAPE_SET_POSITION, *PTAPE_SET_POSITION;
typedef struct _TAPE_GET_DRIVE_PARAMETERS {
BOOLEAN ECC;
BOOLEAN Compression;
BOOLEAN DataPadding;
BOOLEAN ReportSetmarks;
DWORD DefaultBlockSize;
DWORD MaximumBlockSize;
DWORD MinimumBlockSize;
DWORD MaximumPartitionCount;
DWORD FeaturesLow;
DWORD FeaturesHigh;
DWORD EOTWarningZoneSize;
} TAPE_GET_DRIVE_PARAMETERS, *PTAPE_GET_DRIVE_PARAMETERS;
typedef struct _TAPE_SET_DRIVE_PARAMETERS {
BOOLEAN ECC;
BOOLEAN Compression;
BOOLEAN DataPadding;
BOOLEAN ReportSetmarks;
DWORD EOTWarningZoneSize;
} TAPE_SET_DRIVE_PARAMETERS, *PTAPE_SET_DRIVE_PARAMETERS;
typedef struct _TAPE_GET_MEDIA_PARAMETERS {
LARGE_INTEGER Capacity;
LARGE_INTEGER Remaining;
DWORD BlockSize;
DWORD PartitionCount;
BOOLEAN WriteProtected;
} TAPE_GET_MEDIA_PARAMETERS, *PTAPE_GET_MEDIA_PARAMETERS;
typedef struct _TAPE_SET_MEDIA_PARAMETERS {
DWORD BlockSize;
} TAPE_SET_MEDIA_PARAMETERS, *PTAPE_SET_MEDIA_PARAMETERS;
typedef struct _TAPE_CREATE_PARTITION {
DWORD Method;
DWORD Count;
DWORD Size;
} TAPE_CREATE_PARTITION, *PTAPE_CREATE_PARTITION;
}
	// 7221 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 7223 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnt.h"
	// 167 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 168 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
typedef UINT WPARAM;
typedef LONG LPARAM;
typedef LONG LRESULT;
struct HWND__ { int unused; }; typedef struct HWND__ *HWND;
struct HHOOK__ { int unused; }; typedef struct HHOOK__ *HHOOK;
	// 201 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
typedef WORD ATOM;
typedef HANDLE *SPHANDLE;
typedef HANDLE *LPHANDLE;
typedef HANDLE HGLOBAL;
typedef HANDLE HLOCAL;
typedef HANDLE GLOBALHANDLE;
typedef HANDLE LOCALHANDLE;
typedef int ( __stdcall *FARPROC)();
typedef int ( __stdcall *NEARPROC)();
typedef int (__stdcall *PROC)();
	// 219 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
typedef void* HGDIOBJ;
	// 226 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 227 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
struct HACCEL__ { int unused; }; typedef struct HACCEL__ *HACCEL;
	// 231 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
struct HBITMAP__ { int unused; }; typedef struct HBITMAP__ *HBITMAP;
struct HBRUSH__ { int unused; }; typedef struct HBRUSH__ *HBRUSH;
	// 235 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
struct HCOLORSPACE__ { int unused; }; typedef struct HCOLORSPACE__ *HCOLORSPACE;
	// 238 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
struct HDC__ { int unused; }; typedef struct HDC__ *HDC;
	// 241 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
struct HGLRC__ { int unused; }; typedef struct HGLRC__ *HGLRC;
struct HDESK__ { int unused; }; typedef struct HDESK__ *HDESK;
struct HENHMETAFILE__ { int unused; }; typedef struct HENHMETAFILE__ *HENHMETAFILE;
struct HFONT__ { int unused; }; typedef struct HFONT__ *HFONT;
	// 247 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
struct HICON__ { int unused; }; typedef struct HICON__ *HICON;
struct HMENU__ { int unused; }; typedef struct HMENU__ *HMENU;
	// 251 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
struct HMETAFILE__ { int unused; }; typedef struct HMETAFILE__ *HMETAFILE;
struct HINSTANCE__ { int unused; }; typedef struct HINSTANCE__ *HINSTANCE;
typedef HINSTANCE HMODULE;
struct HPALETTE__ { int unused; }; typedef struct HPALETTE__ *HPALETTE;
struct HPEN__ { int unused; }; typedef struct HPEN__ *HPEN;
	// 258 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
struct HRGN__ { int unused; }; typedef struct HRGN__ *HRGN;
struct HRSRC__ { int unused; }; typedef struct HRSRC__ *HRSRC;
struct HSTR__ { int unused; }; typedef struct HSTR__ *HSTR;
struct HTASK__ { int unused; }; typedef struct HTASK__ *HTASK;
struct HWINSTA__ { int unused; }; typedef struct HWINSTA__ *HWINSTA;
struct HKL__ { int unused; }; typedef struct HKL__ *HKL;
	// 272 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
typedef int HFILE;
typedef HICON HCURSOR;
	// 280 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
typedef DWORD COLORREF;
typedef DWORD *LPCOLORREF;
typedef struct tagRECT
{
LONG left;
LONG top;
LONG right;
LONG bottom;
} RECT, *PRECT, *NPRECT, *LPRECT;
typedef const RECT* LPCRECT;
typedef struct _RECTL
{
LONG left;
LONG top;
LONG right;
LONG bottom;
} RECTL, *PRECTL, *LPRECTL;
typedef const RECTL* LPCRECTL;
typedef struct tagPOINT
{
LONG x;
LONG y;
} POINT, *PPOINT, *NPPOINT, *LPPOINT;
typedef struct _POINTL
{
LONG x;
LONG y;
} POINTL, *PPOINTL;
typedef struct tagSIZE
{
LONG cx;
LONG cy;
} SIZE, *PSIZE, *LPSIZE;
typedef SIZE SIZEL;
typedef SIZE *PSIZEL, *LPSIZEL;
typedef struct tagPOINTS
{
SHORT x;
SHORT y;
	// 336 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
} POINTS, *PPOINTS, *LPPOINTS;
}
	// 372 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 374 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windef.h"
	// 164 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 25 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 31 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 37 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
extern "C" {
	// 41 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 109 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 157 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
typedef struct _OVERLAPPED {
DWORD Internal;
DWORD InternalHigh;
DWORD Offset;
DWORD OffsetHigh;
HANDLE hEvent;
} OVERLAPPED, *LPOVERLAPPED;
typedef struct _SECURITY_ATTRIBUTES {
DWORD nLength;
LPVOID lpSecurityDescriptor;
BOOL bInheritHandle;
} SECURITY_ATTRIBUTES, *PSECURITY_ATTRIBUTES, *LPSECURITY_ATTRIBUTES;
typedef struct _PROCESS_INFORMATION {
HANDLE hProcess;
HANDLE hThread;
DWORD dwProcessId;
DWORD dwThreadId;
} PROCESS_INFORMATION, *PPROCESS_INFORMATION, *LPPROCESS_INFORMATION;
typedef struct _FILETIME {
DWORD dwLowDateTime;
DWORD dwHighDateTime;
} FILETIME, *PFILETIME, *LPFILETIME;
typedef struct _SYSTEMTIME {
WORD wYear;
WORD wMonth;
WORD wDayOfWeek;
WORD wDay;
WORD wHour;
WORD wMinute;
WORD wSecond;
WORD wMilliseconds;
} SYSTEMTIME, *PSYSTEMTIME, *LPSYSTEMTIME;
typedef DWORD (__stdcall *PTHREAD_START_ROUTINE)(
LPVOID lpThreadParameter);
typedef PTHREAD_START_ROUTINE LPTHREAD_START_ROUTINE;
	// 271 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
typedef RTL_CRITICAL_SECTION CRITICAL_SECTION;
typedef PRTL_CRITICAL_SECTION PCRITICAL_SECTION;
typedef PRTL_CRITICAL_SECTION LPCRITICAL_SECTION;
typedef RTL_CRITICAL_SECTION_DEBUG CRITICAL_SECTION_DEBUG;
typedef PRTL_CRITICAL_SECTION_DEBUG PCRITICAL_SECTION_DEBUG;
typedef PRTL_CRITICAL_SECTION_DEBUG LPCRITICAL_SECTION_DEBUG;
typedef PLDT_ENTRY LPLDT_ENTRY;
	// 285 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
typedef struct _COMMPROP {
WORD wPacketLength;
WORD wPacketVersion;
DWORD dwServiceMask;
DWORD dwReserved1;
DWORD dwMaxTxQueue;
DWORD dwMaxRxQueue;
DWORD dwMaxBaud;
DWORD dwProvSubType;
DWORD dwProvCapabilities;
DWORD dwSettableParams;
DWORD dwSettableBaud;
WORD wSettableData;
WORD wSettableStopParity;
DWORD dwCurrentTxQueue;
DWORD dwCurrentRxQueue;
DWORD dwProvSpec1;
DWORD dwProvSpec2;
WCHAR wcProvChar[1];
} COMMPROP,*LPCOMMPROP;
typedef struct _COMSTAT {
DWORD fCtsHold : 1;
DWORD fDsrHold : 1;
DWORD fRlsdHold : 1;
DWORD fXoffHold : 1;
DWORD fXoffSent : 1;
DWORD fEof : 1;
DWORD fTxim : 1;
DWORD fReserved : 25;
DWORD cbInQue;
DWORD cbOutQue;
} COMSTAT, *LPCOMSTAT;
typedef struct _DCB {
DWORD DCBlength;
DWORD BaudRate;
DWORD fBinary: 1;
DWORD fParity: 1;
DWORD fOutxCtsFlow:1;
DWORD fOutxDsrFlow:1;
DWORD fDtrControl:2;
DWORD fDsrSensitivity:1;
DWORD fTXContinueOnXoff: 1;
DWORD fOutX: 1;
DWORD fInX: 1;
DWORD fErrorChar: 1;
DWORD fNull: 1;
DWORD fRtsControl:2;
DWORD fAbortOnError:1;
DWORD fDummy2:17;
WORD wReserved;
WORD XonLim;
WORD XoffLim;
BYTE ByteSize;
BYTE Parity;
BYTE StopBits;
char XonChar;
char XoffChar;
char ErrorChar;
char EofChar;
char EvtChar;
WORD wReserved1;
} DCB, *LPDCB;
typedef struct _COMMTIMEOUTS {
DWORD ReadIntervalTimeout;
DWORD ReadTotalTimeoutMultiplier;
DWORD ReadTotalTimeoutConstant;
DWORD WriteTotalTimeoutMultiplier;
DWORD WriteTotalTimeoutConstant;
} COMMTIMEOUTS,*LPCOMMTIMEOUTS;
typedef struct _COMMCONFIG {
DWORD dwSize;
WORD wVersion;
WORD wReserved;
DCB dcb;
DWORD dwProviderSubType;
DWORD dwProviderOffset;
DWORD dwProviderSize;
WCHAR wcProviderData[1];
} COMMCONFIG,*LPCOMMCONFIG;
typedef struct _SYSTEM_INFO {
union {
DWORD dwOemId;
struct {
WORD wProcessorArchitecture;
WORD wReserved;
};
};
DWORD dwPageSize;
LPVOID lpMinimumApplicationAddress;
LPVOID lpMaximumApplicationAddress;
DWORD dwActiveProcessorMask;
DWORD dwNumberOfProcessors;
DWORD dwProcessorType;
DWORD dwAllocationGranularity;
WORD wProcessorLevel;
WORD wProcessorRevision;
} SYSTEM_INFO, *LPSYSTEM_INFO;
typedef struct _MEMORYSTATUS {
DWORD dwLength;
DWORD dwMemoryLoad;
DWORD dwTotalPhys;
DWORD dwAvailPhys;
DWORD dwTotalPageFile;
DWORD dwAvailPageFile;
DWORD dwTotalVirtual;
DWORD dwAvailVirtual;
} MEMORYSTATUS, *LPMEMORYSTATUS;
typedef struct _EXCEPTION_DEBUG_INFO {
EXCEPTION_RECORD ExceptionRecord;
DWORD dwFirstChance;
} EXCEPTION_DEBUG_INFO, *LPEXCEPTION_DEBUG_INFO;
typedef struct _CREATE_THREAD_DEBUG_INFO {
HANDLE hThread;
LPVOID lpThreadLocalBase;
LPTHREAD_START_ROUTINE lpStartAddress;
} CREATE_THREAD_DEBUG_INFO, *LPCREATE_THREAD_DEBUG_INFO;
typedef struct _CREATE_PROCESS_DEBUG_INFO {
HANDLE hFile;
HANDLE hProcess;
HANDLE hThread;
LPVOID lpBaseOfImage;
DWORD dwDebugInfoFileOffset;
DWORD nDebugInfoSize;
LPVOID lpThreadLocalBase;
LPTHREAD_START_ROUTINE lpStartAddress;
LPVOID lpImageName;
WORD fUnicode;
} CREATE_PROCESS_DEBUG_INFO, *LPCREATE_PROCESS_DEBUG_INFO;
typedef struct _EXIT_THREAD_DEBUG_INFO {
DWORD dwExitCode;
} EXIT_THREAD_DEBUG_INFO, *LPEXIT_THREAD_DEBUG_INFO;
typedef struct _EXIT_PROCESS_DEBUG_INFO {
DWORD dwExitCode;
} EXIT_PROCESS_DEBUG_INFO, *LPEXIT_PROCESS_DEBUG_INFO;
typedef struct _LOAD_DLL_DEBUG_INFO {
HANDLE hFile;
LPVOID lpBaseOfDll;
DWORD dwDebugInfoFileOffset;
DWORD nDebugInfoSize;
LPVOID lpImageName;
WORD fUnicode;
} LOAD_DLL_DEBUG_INFO, *LPLOAD_DLL_DEBUG_INFO;
typedef struct _UNLOAD_DLL_DEBUG_INFO {
LPVOID lpBaseOfDll;
} UNLOAD_DLL_DEBUG_INFO, *LPUNLOAD_DLL_DEBUG_INFO;
typedef struct _OUTPUT_DEBUG_STRING_INFO {
LPSTR lpDebugStringData;
WORD fUnicode;
WORD nDebugStringLength;
} OUTPUT_DEBUG_STRING_INFO, *LPOUTPUT_DEBUG_STRING_INFO;
typedef struct _RIP_INFO {
DWORD dwError;
DWORD dwType;
} RIP_INFO, *LPRIP_INFO;
typedef struct _DEBUG_EVENT {
DWORD dwDebugEventCode;
DWORD dwProcessId;
DWORD dwThreadId;
union {
EXCEPTION_DEBUG_INFO Exception;
CREATE_THREAD_DEBUG_INFO CreateThread;
CREATE_PROCESS_DEBUG_INFO CreateProcessInfo;
EXIT_THREAD_DEBUG_INFO ExitThread;
EXIT_PROCESS_DEBUG_INFO ExitProcess;
LOAD_DLL_DEBUG_INFO LoadDll;
UNLOAD_DLL_DEBUG_INFO UnloadDll;
OUTPUT_DEBUG_STRING_INFO DebugString;
RIP_INFO RipInfo;
} u;
} DEBUG_EVENT, *LPDEBUG_EVENT;
typedef PCONTEXT LPCONTEXT;
typedef PEXCEPTION_RECORD LPEXCEPTION_RECORD;
typedef PEXCEPTION_POINTERS LPEXCEPTION_POINTERS;
	// 719 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 734 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
typedef struct _OFSTRUCT {
BYTE cBytes;
BYTE fFixedDisk;
WORD nErrCode;
WORD Reserved1;
WORD Reserved2;
CHAR szPathName[128];
} OFSTRUCT, *LPOFSTRUCT, *POFSTRUCT;
	// 1003 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) LONG __stdcall InterlockedIncrement(
LPLONG lpAddend);
__declspec(dllimport) LONG __stdcall InterlockedDecrement(
LPLONG lpAddend);
__declspec(dllimport) LONG __stdcall InterlockedExchange(
LPLONG Target,LONG Value);
__declspec(dllimport) LONG __stdcall InterlockedExchangeAdd(
LPLONG Addend,LONG Value);
__declspec(dllimport) PVOID __stdcall InterlockedCompareExchange (
PVOID *Destination,PVOID Exchange,PVOID Comperand);
	// 1046 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 1048 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall FreeResource(
HGLOBAL hResData);
__declspec(dllimport) LPVOID __stdcall LockResource(
HGLOBAL hResData);
int __stdcall 	// 1075 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
WinMain(
HINSTANCE hInstance,HINSTANCE hPrevInstance,LPSTR lpCmdLine,int nShowCmd);
__declspec(dllimport) BOOL __stdcall FreeLibrary(
HMODULE hLibModule);
__declspec(dllimport) void __stdcall FreeLibraryAndExitThread(
HMODULE hLibModule,DWORD dwExitCode);
__declspec(dllimport) BOOL __stdcall DisableThreadLibraryCalls(
HMODULE hLibModule);
__declspec(dllimport) FARPROC __stdcall GetProcAddress(
HMODULE hModule,LPCSTR lpProcName);
__declspec(dllimport) DWORD __stdcall GetVersion( void );
__declspec(dllimport) HGLOBAL __stdcall GlobalAlloc(
UINT uFlags,DWORD dwBytes);
__declspec(dllimport) HGLOBAL __stdcall GlobalReAlloc(
HGLOBAL hMem,DWORD dwBytes,UINT uFlags);
__declspec(dllimport) DWORD __stdcall GlobalSize(
HGLOBAL hMem);
__declspec(dllimport) UINT __stdcall GlobalFlags(
HGLOBAL hMem);
__declspec(dllimport) LPVOID __stdcall GlobalLock(
HGLOBAL hMem);
__declspec(dllimport) HGLOBAL __stdcall GlobalHandle(
LPCVOID pMem);
__declspec(dllimport) BOOL __stdcall GlobalUnlock(
HGLOBAL hMem);
__declspec(dllimport) HGLOBAL __stdcall GlobalFree(
HGLOBAL hMem);
__declspec(dllimport) UINT __stdcall GlobalCompact(
DWORD dwMinFree);
__declspec(dllimport) void __stdcall GlobalFix(
HGLOBAL hMem);
__declspec(dllimport) void __stdcall GlobalUnfix(
HGLOBAL hMem);
__declspec(dllimport) LPVOID __stdcall GlobalWire(
HGLOBAL hMem);
__declspec(dllimport) BOOL __stdcall GlobalUnWire(
HGLOBAL hMem);
__declspec(dllimport) void __stdcall GlobalMemoryStatus(
LPMEMORYSTATUS lpBuffer);
__declspec(dllimport) HLOCAL __stdcall LocalAlloc(
UINT uFlags,UINT uBytes);
__declspec(dllimport) HLOCAL __stdcall LocalReAlloc(
HLOCAL hMem,UINT uBytes,UINT uFlags);
__declspec(dllimport) LPVOID __stdcall LocalLock(
HLOCAL hMem);
__declspec(dllimport) HLOCAL __stdcall LocalHandle(
LPCVOID pMem);
__declspec(dllimport) BOOL __stdcall LocalUnlock(
HLOCAL hMem);
__declspec(dllimport) UINT __stdcall LocalSize(
HLOCAL hMem);
__declspec(dllimport) UINT __stdcall LocalFlags(
HLOCAL hMem);
__declspec(dllimport) HLOCAL __stdcall LocalFree(
HLOCAL hMem);
__declspec(dllimport) UINT __stdcall LocalShrink(
HLOCAL hMem,UINT cbNewSize);
__declspec(dllimport) UINT __stdcall LocalCompact(
UINT uMinFree);
__declspec(dllimport) BOOL __stdcall FlushInstructionCache(
HANDLE hProcess,LPCVOID lpBaseAddress,DWORD dwSize);
__declspec(dllimport) LPVOID __stdcall VirtualAlloc(
LPVOID lpAddress,DWORD dwSize,DWORD flAllocationType,DWORD flProtect);
__declspec(dllimport) BOOL __stdcall VirtualFree(
LPVOID lpAddress,DWORD dwSize,DWORD dwFreeType);
__declspec(dllimport) BOOL __stdcall VirtualProtect(
LPVOID lpAddress,DWORD dwSize,DWORD flNewProtect,PDWORD lpflOldProtect);
__declspec(dllimport) DWORD __stdcall VirtualQuery(
LPCVOID lpAddress,PMEMORY_BASIC_INFORMATION lpBuffer,DWORD dwLength);
__declspec(dllimport) LPVOID __stdcall VirtualAllocEx(
HANDLE hProcess,LPVOID lpAddress,DWORD dwSize,DWORD flAllocationType,DWORD flProtect);
__declspec(dllimport) BOOL __stdcall VirtualFreeEx(
HANDLE hProcess,LPVOID lpAddress,DWORD dwSize,DWORD dwFreeType);
__declspec(dllimport) BOOL __stdcall VirtualProtectEx(
HANDLE hProcess,LPVOID lpAddress,DWORD dwSize,DWORD flNewProtect,PDWORD lpflOldProtect);
__declspec(dllimport) DWORD __stdcall VirtualQueryEx(
HANDLE hProcess,LPCVOID lpAddress,PMEMORY_BASIC_INFORMATION lpBuffer,DWORD dwLength);
__declspec(dllimport) HANDLE __stdcall HeapCreate(
DWORD flOptions,DWORD dwInitialSize,DWORD dwMaximumSize);
__declspec(dllimport) BOOL __stdcall HeapDestroy(
HANDLE hHeap);
__declspec(dllimport) LPVOID __stdcall HeapAlloc(
HANDLE hHeap,DWORD dwFlags,DWORD dwBytes);
__declspec(dllimport) LPVOID __stdcall HeapReAlloc(
HANDLE hHeap,DWORD dwFlags,LPVOID lpMem,DWORD dwBytes);
__declspec(dllimport) BOOL __stdcall HeapFree(
HANDLE hHeap,DWORD dwFlags,LPVOID lpMem);
__declspec(dllimport) DWORD __stdcall HeapSize(
HANDLE hHeap,DWORD dwFlags,LPCVOID lpMem);
__declspec(dllimport) BOOL __stdcall HeapValidate(
HANDLE hHeap,DWORD dwFlags,LPCVOID lpMem);
__declspec(dllimport) UINT __stdcall HeapCompact(
HANDLE hHeap,DWORD dwFlags);
__declspec(dllimport) HANDLE __stdcall GetProcessHeap( void );
__declspec(dllimport) DWORD __stdcall GetProcessHeaps(
DWORD NumberOfHeaps,PHANDLE ProcessHeaps);
typedef struct _PROCESS_HEAP_ENTRY {
PVOID lpData;
DWORD cbData;
BYTE cbOverhead;
BYTE iRegionIndex;
WORD wFlags;
union {
struct {
HANDLE hMem;
DWORD dwReserved[ 3 ];
} Block;
struct {
DWORD dwCommittedSize;
DWORD dwUnCommittedSize;
LPVOID lpFirstBlock;
LPVOID lpLastBlock;
} Region;
};
} PROCESS_HEAP_ENTRY, *LPPROCESS_HEAP_ENTRY, *PPROCESS_HEAP_ENTRY;
__declspec(dllimport) BOOL __stdcall HeapLock(
HANDLE hHeap);
__declspec(dllimport) BOOL __stdcall HeapUnlock(
HANDLE hHeap);
__declspec(dllimport) BOOL __stdcall HeapWalk(
HANDLE hHeap,LPPROCESS_HEAP_ENTRY lpEntry);
__declspec(dllimport) BOOL __stdcall GetBinaryTypeA(
LPCSTR lpApplicationName,LPDWORD lpBinaryType);
__declspec(dllimport) BOOL __stdcall GetBinaryTypeW(
LPCWSTR lpApplicationName,LPDWORD lpBinaryType);
	// 1547 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetShortPathNameA(
LPCSTR lpszLongPath,LPSTR lpszShortPath,DWORD cchBuffer);
__declspec(dllimport) DWORD __stdcall GetShortPathNameW(
LPCWSTR lpszLongPath,LPWSTR lpszShortPath,DWORD cchBuffer);
	// 1569 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetLongPathNameA(
LPCSTR lpszShortPath,LPSTR lpszLongPath,DWORD cchBuffer);
__declspec(dllimport) DWORD __stdcall GetLongPathNameW(
LPCWSTR lpszShortPath,LPWSTR lpszLongPath,DWORD cchBuffer);
	// 1591 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall GetProcessAffinityMask(
HANDLE hProcess,LPDWORD lpProcessAffinityMask,LPDWORD lpSystemAffinityMask);
__declspec(dllimport) BOOL __stdcall SetProcessAffinityMask(
HANDLE hProcess,DWORD dwProcessAffinityMask);
__declspec(dllimport) BOOL __stdcall GetProcessTimes(
HANDLE hProcess,LPFILETIME lpCreationTime,LPFILETIME lpExitTime,LPFILETIME lpKernelTime,LPFILETIME lpUserTime);
__declspec(dllimport) BOOL __stdcall GetProcessWorkingSetSize(
HANDLE hProcess,LPDWORD lpMinimumWorkingSetSize,LPDWORD lpMaximumWorkingSetSize
);
__declspec(dllimport) BOOL __stdcall SetProcessWorkingSetSize(
HANDLE hProcess,DWORD dwMinimumWorkingSetSize,DWORD dwMaximumWorkingSetSize);
__declspec(dllimport) HANDLE __stdcall OpenProcess(
DWORD dwDesiredAccess,BOOL bInheritHandle,DWORD dwProcessId);
__declspec(dllimport) HANDLE __stdcall GetCurrentProcess(
void);
__declspec(dllimport) DWORD __stdcall GetCurrentProcessId(
void);
__declspec(dllimport) void __stdcall ExitProcess(
UINT uExitCode);
__declspec(dllimport) BOOL __stdcall TerminateProcess(
HANDLE hProcess,UINT uExitCode);
__declspec(dllimport) BOOL __stdcall GetExitCodeProcess(
HANDLE hProcess,LPDWORD lpExitCode);
__declspec(dllimport) void __stdcall FatalExit(
int ExitCode);
__declspec(dllimport) LPSTR __stdcall GetEnvironmentStrings(
void);
__declspec(dllimport) LPWSTR __stdcall GetEnvironmentStringsW(
void);
	// 1712 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall FreeEnvironmentStringsA(
LPSTR);
__declspec(dllimport) BOOL __stdcall FreeEnvironmentStringsW(
LPWSTR);
	// 1730 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) void __stdcall RaiseException(
DWORD dwExceptionCode,DWORD dwExceptionFlags,DWORD nNumberOfArguments,const DWORD *lpArguments);
__declspec(dllimport) LONG __stdcall UnhandledExceptionFilter(
struct _EXCEPTION_POINTERS *ExceptionInfo);
typedef LONG (__stdcall *PTOP_LEVEL_EXCEPTION_FILTER)(
struct _EXCEPTION_POINTERS *ExceptionInfo);
typedef PTOP_LEVEL_EXCEPTION_FILTER LPTOP_LEVEL_EXCEPTION_FILTER;
__declspec(dllimport) LPTOP_LEVEL_EXCEPTION_FILTER __stdcall SetUnhandledExceptionFilter(
LPTOP_LEVEL_EXCEPTION_FILTER lpTopLevelExceptionFilter);
	// 1798 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HANDLE __stdcall CreateThread(
LPSECURITY_ATTRIBUTES lpThreadAttributes,DWORD dwStackSize,LPTHREAD_START_ROUTINE lpStartAddress,LPVOID lpParameter,DWORD dwCreationFlags,LPDWORD lpThreadId);
__declspec(dllimport) HANDLE __stdcall CreateRemoteThread(
HANDLE hProcess,LPSECURITY_ATTRIBUTES lpThreadAttributes,DWORD dwStackSize,LPTHREAD_START_ROUTINE lpStartAddress,LPVOID lpParameter,DWORD dwCreationFlags,LPDWORD lpThreadId);
__declspec(dllimport) HANDLE __stdcall GetCurrentThread(
void);
__declspec(dllimport) DWORD __stdcall GetCurrentThreadId(
void);
__declspec(dllimport) DWORD __stdcall SetThreadAffinityMask(
HANDLE hThread,DWORD dwThreadAffinityMask);
	// 1855 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall SetProcessPriorityBoost(
HANDLE hProcess,BOOL bDisablePriorityBoost);
__declspec(dllimport) BOOL __stdcall GetProcessPriorityBoost(
HANDLE hProcess,PBOOL pDisablePriorityBoost);
__declspec(dllimport) BOOL __stdcall RequestWakeupLatency(
LATENCY_TIME latency);
__declspec(dllimport) BOOL __stdcall SetThreadPriority(
HANDLE hThread,int nPriority);
__declspec(dllimport) BOOL __stdcall SetThreadPriorityBoost(
HANDLE hThread,BOOL bDisablePriorityBoost);
__declspec(dllimport) BOOL __stdcall GetThreadPriorityBoost(
HANDLE hThread,PBOOL pDisablePriorityBoost);
__declspec(dllimport) int __stdcall GetThreadPriority(
HANDLE hThread);
__declspec(dllimport) BOOL __stdcall GetThreadTimes(
HANDLE hThread,LPFILETIME lpCreationTime,LPFILETIME lpExitTime,LPFILETIME lpKernelTime,LPFILETIME lpUserTime);
__declspec(dllimport) void __stdcall ExitThread(
DWORD dwExitCode);
__declspec(dllimport) BOOL __stdcall TerminateThread(
HANDLE hThread,DWORD dwExitCode);
__declspec(dllimport) BOOL __stdcall GetExitCodeThread(
HANDLE hThread,LPDWORD lpExitCode);
__declspec(dllimport) BOOL __stdcall GetThreadSelectorEntry(
HANDLE hThread,DWORD dwSelector,LPLDT_ENTRY lpSelectorEntry);
__declspec(dllimport) EXECUTION_STATE __stdcall SetThreadExecutionState(
EXECUTION_STATE esFlags);
__declspec(dllimport) DWORD __stdcall GetLastError(
void);
__declspec(dllimport) void __stdcall SetLastError(
DWORD dwErrCode);
__declspec(dllimport) BOOL __stdcall GetOverlappedResult(
HANDLE hFile,LPOVERLAPPED lpOverlapped,LPDWORD lpNumberOfBytesTransferred,BOOL bWait);
__declspec(dllimport) HANDLE __stdcall CreateIoCompletionPort(
HANDLE FileHandle,HANDLE ExistingCompletionPort,DWORD CompletionKey,DWORD NumberOfConcurrentThreads);
__declspec(dllimport) BOOL __stdcall GetQueuedCompletionStatus(
HANDLE CompletionPort,LPDWORD lpNumberOfBytesTransferred,LPDWORD lpCompletionKey,LPOVERLAPPED *lpOverlapped,DWORD dwMilliseconds);
__declspec(dllimport) BOOL __stdcall PostQueuedCompletionStatus(
HANDLE CompletionPort,DWORD dwNumberOfBytesTransferred,DWORD dwCompletionKey,LPOVERLAPPED lpOverlapped);
__declspec(dllimport) UINT __stdcall SetErrorMode(
UINT uMode);
__declspec(dllimport) BOOL __stdcall ReadProcessMemory(
HANDLE hProcess,LPCVOID lpBaseAddress,LPVOID lpBuffer,DWORD nSize,LPDWORD lpNumberOfBytesRead);
__declspec(dllimport) BOOL __stdcall WriteProcessMemory(
HANDLE hProcess,LPVOID lpBaseAddress,LPVOID lpBuffer,DWORD nSize,LPDWORD lpNumberOfBytesWritten);
__declspec(dllimport) BOOL __stdcall GetThreadContext(
HANDLE hThread,LPCONTEXT lpContext);
__declspec(dllimport) BOOL __stdcall SetThreadContext(
HANDLE hThread,const CONTEXT *lpContext);
	// 2068 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall SuspendThread(
HANDLE hThread);
__declspec(dllimport) DWORD __stdcall ResumeThread(
HANDLE hThread);
	// 2101 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 2110 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) void __stdcall DebugBreak(
void);
__declspec(dllimport) BOOL __stdcall WaitForDebugEvent(
LPDEBUG_EVENT lpDebugEvent,DWORD dwMilliseconds);
__declspec(dllimport) BOOL __stdcall ContinueDebugEvent(
DWORD dwProcessId,DWORD dwThreadId,DWORD dwContinueStatus);
__declspec(dllimport) BOOL __stdcall DebugActiveProcess(
DWORD dwProcessId);
__declspec(dllimport) void __stdcall InitializeCriticalSection(
LPCRITICAL_SECTION lpCriticalSection);
__declspec(dllimport) void __stdcall EnterCriticalSection(
LPCRITICAL_SECTION lpCriticalSection);
__declspec(dllimport) void __stdcall LeaveCriticalSection(
LPCRITICAL_SECTION lpCriticalSection);
	// 2180 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 2189 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) void __stdcall DeleteCriticalSection(
LPCRITICAL_SECTION lpCriticalSection);
__declspec(dllimport) BOOL __stdcall SetEvent(
HANDLE hEvent);
__declspec(dllimport) BOOL __stdcall ResetEvent(
HANDLE hEvent);
__declspec(dllimport) BOOL __stdcall PulseEvent(
HANDLE hEvent);
__declspec(dllimport) BOOL __stdcall ReleaseSemaphore(
HANDLE hSemaphore,LONG lReleaseCount,LPLONG lpPreviousCount);
__declspec(dllimport) BOOL __stdcall ReleaseMutex(
HANDLE hMutex);
__declspec(dllimport) DWORD __stdcall WaitForSingleObject(
HANDLE hHandle,DWORD dwMilliseconds);
__declspec(dllimport) DWORD __stdcall WaitForMultipleObjects(
DWORD nCount,const HANDLE *lpHandles,BOOL bWaitAll,DWORD dwMilliseconds);
__declspec(dllimport) void __stdcall Sleep(
DWORD dwMilliseconds);
__declspec(dllimport) HGLOBAL __stdcall LoadResource(
HMODULE hModule,HRSRC hResInfo);
__declspec(dllimport) DWORD __stdcall SizeofResource(
HMODULE hModule,HRSRC hResInfo);
__declspec(dllimport) ATOM __stdcall GlobalDeleteAtom(
ATOM nAtom);
__declspec(dllimport) BOOL __stdcall InitAtomTable(
DWORD nSize);
__declspec(dllimport) ATOM __stdcall DeleteAtom(
ATOM nAtom);
__declspec(dllimport) UINT __stdcall SetHandleCount(
UINT uNumber);
__declspec(dllimport) DWORD __stdcall GetLogicalDrives(
void);
__declspec(dllimport) BOOL __stdcall LockFile(
HANDLE hFile,DWORD dwFileOffsetLow,DWORD dwFileOffsetHigh,DWORD nNumberOfBytesToLockLow,DWORD nNumberOfBytesToLockHigh);
__declspec(dllimport) BOOL __stdcall UnlockFile(
HANDLE hFile,DWORD dwFileOffsetLow,DWORD dwFileOffsetHigh,DWORD nNumberOfBytesToUnlockLow,DWORD nNumberOfBytesToUnlockHigh);
__declspec(dllimport) BOOL __stdcall LockFileEx(
HANDLE hFile,DWORD dwFlags,DWORD dwReserved,DWORD nNumberOfBytesToLockLow,DWORD nNumberOfBytesToLockHigh,LPOVERLAPPED lpOverlapped);
__declspec(dllimport) BOOL __stdcall UnlockFileEx(
HANDLE hFile,DWORD dwReserved,DWORD nNumberOfBytesToUnlockLow,DWORD nNumberOfBytesToUnlockHigh,LPOVERLAPPED lpOverlapped);
typedef struct _BY_HANDLE_FILE_INFORMATION {
DWORD dwFileAttributes;
FILETIME ftCreationTime;
FILETIME ftLastAccessTime;
FILETIME ftLastWriteTime;
DWORD dwVolumeSerialNumber;
DWORD nFileSizeHigh;
DWORD nFileSizeLow;
DWORD nNumberOfLinks;
DWORD nFileIndexHigh;
DWORD nFileIndexLow;
} BY_HANDLE_FILE_INFORMATION, *PBY_HANDLE_FILE_INFORMATION, *LPBY_HANDLE_FILE_INFORMATION;
__declspec(dllimport) BOOL __stdcall GetFileInformationByHandle(
HANDLE hFile,LPBY_HANDLE_FILE_INFORMATION lpFileInformation);
__declspec(dllimport) DWORD __stdcall GetFileType(
HANDLE hFile);
__declspec(dllimport) DWORD __stdcall GetFileSize(
HANDLE hFile,LPDWORD lpFileSizeHigh);
__declspec(dllimport) HANDLE __stdcall GetStdHandle(
DWORD nStdHandle);
__declspec(dllimport) BOOL __stdcall SetStdHandle(
DWORD nStdHandle,HANDLE hHandle);
__declspec(dllimport) BOOL __stdcall WriteFile(
HANDLE hFile,LPCVOID lpBuffer,DWORD nNumberOfBytesToWrite,LPDWORD lpNumberOfBytesWritten,LPOVERLAPPED lpOverlapped);
__declspec(dllimport) BOOL __stdcall ReadFile(
HANDLE hFile,LPVOID lpBuffer,DWORD nNumberOfBytesToRead,LPDWORD lpNumberOfBytesRead,LPOVERLAPPED lpOverlapped);
__declspec(dllimport) BOOL __stdcall FlushFileBuffers(
HANDLE hFile);
__declspec(dllimport) BOOL __stdcall DeviceIoControl(
HANDLE hDevice,DWORD dwIoControlCode,LPVOID lpInBuffer,DWORD nInBufferSize,LPVOID lpOutBuffer,DWORD nOutBufferSize,LPDWORD lpBytesReturned,LPOVERLAPPED lpOverlapped);
__declspec(dllimport) BOOL __stdcall GetDevicePowerState(
HANDLE hFile);
__declspec(dllimport) BOOL __stdcall SetEndOfFile(
HANDLE hFile);
__declspec(dllimport) DWORD __stdcall SetFilePointer(
HANDLE hFile,LONG lDistanceToMove,PLONG lpDistanceToMoveHigh,DWORD dwMoveMethod);
__declspec(dllimport) BOOL __stdcall FindClose(
HANDLE hFindFile);
__declspec(dllimport) BOOL __stdcall GetFileTime(
HANDLE hFile,LPFILETIME lpCreationTime,LPFILETIME lpLastAccessTime,LPFILETIME lpLastWriteTime);
__declspec(dllimport) BOOL __stdcall SetFileTime(
HANDLE hFile,const FILETIME *lpCreationTime,const FILETIME *lpLastAccessTime,const FILETIME *lpLastWriteTime);
__declspec(dllimport) BOOL __stdcall CloseHandle(
HANDLE hObject);
__declspec(dllimport) BOOL __stdcall DuplicateHandle(
HANDLE hSourceProcessHandle,HANDLE hSourceHandle,HANDLE hTargetProcessHandle,LPHANDLE lpTargetHandle,DWORD dwDesiredAccess,BOOL bInheritHandle,DWORD dwOptions);
__declspec(dllimport) BOOL __stdcall GetHandleInformation(
HANDLE hObject,LPDWORD lpdwFlags);
__declspec(dllimport) BOOL __stdcall SetHandleInformation(
HANDLE hObject,DWORD dwMask,DWORD dwFlags);
__declspec(dllimport) DWORD __stdcall LoadModule(
LPCSTR lpModuleName,LPVOID lpParameterBlock);
__declspec(dllimport) UINT __stdcall WinExec(
LPCSTR lpCmdLine,UINT uCmdShow);
__declspec(dllimport) BOOL __stdcall ClearCommBreak(
HANDLE hFile);
__declspec(dllimport) BOOL __stdcall ClearCommError(
HANDLE hFile,LPDWORD lpErrors,LPCOMSTAT lpStat);
__declspec(dllimport) BOOL __stdcall SetupComm(
HANDLE hFile,DWORD dwInQueue,DWORD dwOutQueue);
__declspec(dllimport) BOOL __stdcall EscapeCommFunction(
HANDLE hFile,DWORD dwFunc);
__declspec(dllimport) BOOL __stdcall GetCommConfig(
HANDLE hCommDev,LPCOMMCONFIG lpCC,LPDWORD lpdwSize);
__declspec(dllimport) BOOL __stdcall GetCommMask(
HANDLE hFile,LPDWORD lpEvtMask);
__declspec(dllimport) BOOL __stdcall GetCommProperties(
HANDLE hFile,LPCOMMPROP lpCommProp);
__declspec(dllimport) BOOL __stdcall GetCommModemStatus(
HANDLE hFile,LPDWORD lpModemStat);
__declspec(dllimport) BOOL __stdcall GetCommState(
HANDLE hFile,LPDCB lpDCB);
__declspec(dllimport) BOOL __stdcall GetCommTimeouts(
HANDLE hFile,LPCOMMTIMEOUTS lpCommTimeouts);
__declspec(dllimport) BOOL __stdcall PurgeComm(
HANDLE hFile,DWORD dwFlags);
__declspec(dllimport) BOOL __stdcall SetCommBreak(
HANDLE hFile);
__declspec(dllimport) BOOL __stdcall SetCommConfig(
HANDLE hCommDev,LPCOMMCONFIG lpCC,DWORD dwSize);
__declspec(dllimport) BOOL __stdcall SetCommMask(
HANDLE hFile,DWORD dwEvtMask);
__declspec(dllimport) BOOL __stdcall SetCommState(
HANDLE hFile,LPDCB lpDCB);
__declspec(dllimport) BOOL __stdcall SetCommTimeouts(
HANDLE hFile,LPCOMMTIMEOUTS lpCommTimeouts);
__declspec(dllimport) BOOL __stdcall TransmitCommChar(
HANDLE hFile,char cChar);
__declspec(dllimport) BOOL __stdcall WaitCommEvent(
HANDLE hFile,LPDWORD lpEvtMask,LPOVERLAPPED lpOverlapped);
__declspec(dllimport) DWORD __stdcall SetTapePosition(
HANDLE hDevice,DWORD dwPositionMethod,DWORD dwPartition,DWORD dwOffsetLow,DWORD dwOffsetHigh,BOOL bImmediate);
__declspec(dllimport) DWORD __stdcall GetTapePosition(
HANDLE hDevice,DWORD dwPositionType,LPDWORD lpdwPartition,LPDWORD lpdwOffsetLow,LPDWORD lpdwOffsetHigh);
__declspec(dllimport) DWORD __stdcall PrepareTape(
HANDLE hDevice,DWORD dwOperation,BOOL bImmediate);
__declspec(dllimport) DWORD __stdcall EraseTape(
HANDLE hDevice,DWORD dwEraseType,BOOL bImmediate);
__declspec(dllimport) DWORD __stdcall CreateTapePartition(
HANDLE hDevice,DWORD dwPartitionMethod,DWORD dwCount,DWORD dwSize);
__declspec(dllimport) DWORD __stdcall WriteTapemark(
HANDLE hDevice,DWORD dwTapemarkType,DWORD dwTapemarkCount,BOOL bImmediate);
__declspec(dllimport) DWORD __stdcall GetTapeStatus(
HANDLE hDevice);
__declspec(dllimport) DWORD __stdcall GetTapeParameters(
HANDLE hDevice,DWORD dwOperation,LPDWORD lpdwSize,LPVOID lpTapeInformation);
__declspec(dllimport) DWORD __stdcall SetTapeParameters(
HANDLE hDevice,DWORD dwOperation,LPVOID lpTapeInformation);
__declspec(dllimport) BOOL __stdcall Beep(
DWORD dwFreq,DWORD dwDuration);
__declspec(dllimport) int __stdcall MulDiv(
int nNumber,int nNumerator,int nDenominator);
__declspec(dllimport) void __stdcall GetSystemTime(
LPSYSTEMTIME lpSystemTime);
__declspec(dllimport) void __stdcall GetSystemTimeAsFileTime(
LPFILETIME lpSystemTimeAsFileTime);
__declspec(dllimport) BOOL __stdcall SetSystemTime(
const SYSTEMTIME *lpSystemTime);
__declspec(dllimport) void __stdcall GetLocalTime(
LPSYSTEMTIME lpSystemTime);
__declspec(dllimport) BOOL __stdcall SetLocalTime(
const SYSTEMTIME *lpSystemTime);
__declspec(dllimport) void __stdcall GetSystemInfo(
LPSYSTEM_INFO lpSystemInfo);
__declspec(dllimport) BOOL __stdcall IsProcessorFeaturePresent(
DWORD ProcessorFeature);
typedef struct _TIME_ZONE_INFORMATION {
LONG Bias;
WCHAR StandardName[ 32 ];
SYSTEMTIME StandardDate;
LONG StandardBias;
WCHAR DaylightName[ 32 ];
SYSTEMTIME DaylightDate;
LONG DaylightBias;
} TIME_ZONE_INFORMATION, *PTIME_ZONE_INFORMATION, *LPTIME_ZONE_INFORMATION;
__declspec(dllimport) BOOL __stdcall SystemTimeToTzSpecificLocalTime(
LPTIME_ZONE_INFORMATION lpTimeZoneInformation,LPSYSTEMTIME lpUniversalTime,LPSYSTEMTIME lpLocalTime);
__declspec(dllimport) DWORD __stdcall GetTimeZoneInformation(
LPTIME_ZONE_INFORMATION lpTimeZoneInformation);
__declspec(dllimport) BOOL __stdcall SetTimeZoneInformation(
const TIME_ZONE_INFORMATION *lpTimeZoneInformation);
__declspec(dllimport) BOOL __stdcall SystemTimeToFileTime(
const SYSTEMTIME *lpSystemTime,LPFILETIME lpFileTime);
__declspec(dllimport) BOOL __stdcall FileTimeToLocalFileTime(
const FILETIME *lpFileTime,LPFILETIME lpLocalFileTime);
__declspec(dllimport) BOOL __stdcall LocalFileTimeToFileTime(
const FILETIME *lpLocalFileTime,LPFILETIME lpFileTime);
__declspec(dllimport) BOOL __stdcall FileTimeToSystemTime(
const FILETIME *lpFileTime,LPSYSTEMTIME lpSystemTime);
__declspec(dllimport) LONG __stdcall CompareFileTime(
const FILETIME *lpFileTime1,const FILETIME *lpFileTime2);
__declspec(dllimport) BOOL __stdcall FileTimeToDosDateTime(
const FILETIME *lpFileTime,LPWORD lpFatDate,LPWORD lpFatTime);
__declspec(dllimport) BOOL __stdcall DosDateTimeToFileTime(
WORD wFatDate,WORD wFatTime,LPFILETIME lpFileTime);
__declspec(dllimport) DWORD __stdcall GetTickCount(
void);
__declspec(dllimport) BOOL __stdcall SetSystemTimeAdjustment(
DWORD dwTimeAdjustment,BOOL bTimeAdjustmentDisabled);
__declspec(dllimport) BOOL __stdcall GetSystemTimeAdjustment(
PDWORD lpTimeAdjustment,PDWORD lpTimeIncrement,PBOOL lpTimeAdjustmentDisabled);
__declspec(dllimport) DWORD __stdcall FormatMessageA(
DWORD dwFlags,LPCVOID lpSource,DWORD dwMessageId,DWORD dwLanguageId,LPSTR lpBuffer,DWORD nSize,va_list *Arguments);
__declspec(dllimport) DWORD __stdcall FormatMessageW(
DWORD dwFlags,LPCVOID lpSource,DWORD dwMessageId,DWORD dwLanguageId,LPWSTR lpBuffer,DWORD nSize,va_list *Arguments);
	// 3019 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 3020 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall CreatePipe(
PHANDLE hReadPipe,PHANDLE hWritePipe,LPSECURITY_ATTRIBUTES lpPipeAttributes,DWORD nSize);
__declspec(dllimport) BOOL __stdcall ConnectNamedPipe(
HANDLE hNamedPipe,LPOVERLAPPED lpOverlapped);
__declspec(dllimport) BOOL __stdcall DisconnectNamedPipe(
HANDLE hNamedPipe);
__declspec(dllimport) BOOL __stdcall SetNamedPipeHandleState(
HANDLE hNamedPipe,LPDWORD lpMode,LPDWORD lpMaxCollectionCount,LPDWORD lpCollectDataTimeout);
__declspec(dllimport) BOOL __stdcall GetNamedPipeInfo(
HANDLE hNamedPipe,LPDWORD lpFlags,LPDWORD lpOutBufferSize,LPDWORD lpInBufferSize,LPDWORD lpMaxInstances);
__declspec(dllimport) BOOL __stdcall PeekNamedPipe(
HANDLE hNamedPipe,LPVOID lpBuffer,DWORD nBufferSize,LPDWORD lpBytesRead,LPDWORD lpTotalBytesAvail,LPDWORD lpBytesLeftThisMessage);
__declspec(dllimport) BOOL __stdcall TransactNamedPipe(
HANDLE hNamedPipe,LPVOID lpInBuffer,DWORD nInBufferSize,LPVOID lpOutBuffer,DWORD nOutBufferSize,LPDWORD lpBytesRead,LPOVERLAPPED lpOverlapped);
__declspec(dllimport) HANDLE __stdcall CreateMailslotA(
LPCSTR lpName,DWORD nMaxMessageSize,DWORD lReadTimeout,LPSECURITY_ATTRIBUTES lpSecurityAttributes);
__declspec(dllimport) HANDLE __stdcall CreateMailslotW(
LPCWSTR lpName,DWORD nMaxMessageSize,DWORD lReadTimeout,LPSECURITY_ATTRIBUTES lpSecurityAttributes);
	// 3124 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall GetMailslotInfo(
HANDLE hMailslot,LPDWORD lpMaxMessageSize,LPDWORD lpNextSize,LPDWORD lpMessageCount,LPDWORD lpReadTimeout);
__declspec(dllimport) BOOL __stdcall SetMailslotInfo(
HANDLE hMailslot,DWORD lReadTimeout);
__declspec(dllimport) LPVOID __stdcall MapViewOfFile(
HANDLE hFileMappingObject,DWORD dwDesiredAccess,DWORD dwFileOffsetHigh,DWORD dwFileOffsetLow,DWORD dwNumberOfBytesToMap);
__declspec(dllimport) PVOID64 __stdcall MapViewOfFileVlm(
HANDLE hFileMappingObject,DWORD dwDesiredAccess,DWORDLONG ulOffset,DWORDLONG ulNumberOfBytesToMap,PVOID64 lpBaseAddress);
__declspec(dllimport) BOOL __stdcall FlushViewOfFile(
LPCVOID lpBaseAddress,DWORD dwNumberOfBytesToFlush);
__declspec(dllimport) BOOL __stdcall UnmapViewOfFile(
LPCVOID lpBaseAddress
);
__declspec(dllimport) BOOL __stdcall UnmapViewOfFileVlm(
PVOID64 lpBaseAddress);
__declspec(dllimport) BOOL __stdcall EncryptFileA(
LPCSTR lpFileName);
__declspec(dllimport) BOOL __stdcall EncryptFileW(
LPCWSTR lpFileName);
	// 3210 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall DecryptFileA(
LPCSTR lpFileName,DWORD dwReserved);
__declspec(dllimport) BOOL __stdcall DecryptFileW(
LPCWSTR lpFileName,DWORD dwReserved);
	// 3230 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
typedef
DWORD
(*PFE_EXPORT_FUNC)(
PBYTE pbData,PVOID pvCallbackContext,ULONG ulLength);
typedef
DWORD
(*PFE_IMPORT_FUNC)(
PBYTE pbData,PVOID pvCallbackContext,PULONG ulLength);
__declspec(dllimport) DWORD __stdcall OpenRawA(
LPCSTR lpFileName,ULONG ulFlags,PVOID* pvContext);
__declspec(dllimport) DWORD __stdcall OpenRawW(
LPCWSTR lpFileName,ULONG ulFlags,PVOID* pvContext);
	// 3283 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall ReadRaw(
PFE_EXPORT_FUNC pfExportCallback,PVOID pvCallbackContext,PVOID pvContext);
__declspec(dllimport) DWORD __stdcall WriteRaw(
PFE_IMPORT_FUNC pfImportCallback,PVOID pvCallbackContext,PVOID pvContext);
__declspec(dllimport) void __stdcall CloseRaw(
PVOID pvContext);
typedef struct _RECOVERY_AGENT_INFORMATIONA {
DWORD NextEntryOffset;
DWORD AgentNameLength;
CHAR AgentInformation[1];
} RECOVERY_AGENT_INFORMATIONA, *LPRECOVERY_AGENT_INFORMATIONA;
typedef struct _RECOVERY_AGENT_INFORMATIONW {
DWORD NextEntryOffset;
DWORD AgentNameLength;
WCHAR AgentInformation[1];
} RECOVERY_AGENT_INFORMATIONW, *LPRECOVERY_AGENT_INFORMATIONW;
typedef RECOVERY_AGENT_INFORMATIONA RECOVERY_AGENT_INFORMATION;
typedef LPRECOVERY_AGENT_INFORMATIONA LPRECOVERY_AGENT_INFORMATION;
	// 3326 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall QueryRecoveryAgentsA(
LPCSTR lpFileName,PDWORD AgentCount,LPRECOVERY_AGENT_INFORMATIONA* RecoveryAgentInformation);
__declspec(dllimport) DWORD __stdcall QueryRecoveryAgentsW(
LPCWSTR lpFileName,PDWORD AgentCount,LPRECOVERY_AGENT_INFORMATIONW* RecoveryAgentInformation);
	// 3348 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) int __stdcall lstrcmpA(
LPCSTR lpString1,LPCSTR lpString2);
__declspec(dllimport) int __stdcall lstrcmpW(
LPCWSTR lpString1,LPCWSTR lpString2);
	// 3372 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) int __stdcall lstrcmpiA(
LPCSTR lpString1,LPCSTR lpString2);
__declspec(dllimport) int __stdcall lstrcmpiW(
LPCWSTR lpString1,LPCWSTR lpString2);
	// 3392 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) LPSTR __stdcall lstrcpynA(
LPSTR lpString1,LPCSTR lpString2,int iMaxLength);
__declspec(dllimport) LPWSTR __stdcall lstrcpynW(
LPWSTR lpString1,LPCWSTR lpString2,int iMaxLength);
	// 3414 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) LPSTR __stdcall lstrcpyA(
LPSTR lpString1,LPCSTR lpString2);
__declspec(dllimport) LPWSTR __stdcall lstrcpyW(
LPWSTR lpString1,LPCWSTR lpString2);
	// 3434 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) LPSTR __stdcall lstrcatA(
LPSTR lpString1,LPCSTR lpString2);
__declspec(dllimport) LPWSTR __stdcall lstrcatW(
LPWSTR lpString1,LPCWSTR lpString2);
	// 3454 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) int __stdcall lstrlenA(
LPCSTR lpString);
__declspec(dllimport) int __stdcall lstrlenW(
LPCWSTR lpString);
	// 3472 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HFILE __stdcall OpenFile(
LPCSTR lpFileName,LPOFSTRUCT lpReOpenBuff,UINT uStyle);
__declspec(dllimport) HFILE __stdcall _lopen(
LPCSTR lpPathName,int iReadWrite);
__declspec(dllimport) HFILE __stdcall _lcreat(
LPCSTR lpPathName,int iAttribute);
__declspec(dllimport) UINT __stdcall _lread(
HFILE hFile,LPVOID lpBuffer,UINT uBytes);
__declspec(dllimport) UINT __stdcall _lwrite(
HFILE hFile,LPCSTR lpBuffer,UINT uBytes);
__declspec(dllimport) long __stdcall _hread(
HFILE hFile,LPVOID lpBuffer,long lBytes);
__declspec(dllimport) long __stdcall _hwrite(
HFILE hFile,LPCSTR lpBuffer,long lBytes);
__declspec(dllimport) HFILE __stdcall _lclose(
HFILE hFile);
__declspec(dllimport) LONG __stdcall _llseek(
HFILE hFile,LONG lOffset,int iOrigin);
__declspec(dllimport) BOOL __stdcall IsTextUnicode(
const LPVOID lpBuffer,int cb,LPINT lpi);
__declspec(dllimport) DWORD __stdcall TlsAlloc(
void);
__declspec(dllimport) LPVOID __stdcall TlsGetValue(
DWORD dwTlsIndex);
__declspec(dllimport) BOOL __stdcall TlsSetValue(
DWORD dwTlsIndex,LPVOID lpTlsValue);
__declspec(dllimport) BOOL __stdcall TlsFree(
DWORD dwTlsIndex);
typedef
void
(__stdcall *LPOVERLAPPED_COMPLETION_ROUTINE)(
DWORD dwErrorCode,DWORD dwNumberOfBytesTransfered,LPOVERLAPPED lpOverlapped);
__declspec(dllimport) DWORD __stdcall SleepEx(
DWORD dwMilliseconds,BOOL bAlertable);
__declspec(dllimport) DWORD __stdcall WaitForSingleObjectEx(
HANDLE hHandle,DWORD dwMilliseconds,BOOL bAlertable);
__declspec(dllimport) DWORD __stdcall WaitForMultipleObjectsEx(
DWORD nCount,const HANDLE *lpHandles,BOOL bWaitAll,DWORD dwMilliseconds,BOOL bAlertable);
	// 3637 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall ReadFileEx(
HANDLE hFile,LPVOID lpBuffer,DWORD nNumberOfBytesToRead,LPOVERLAPPED lpOverlapped,LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine);
__declspec(dllimport) BOOL __stdcall WriteFileEx(
HANDLE hFile,LPCVOID lpBuffer,DWORD nNumberOfBytesToWrite,LPOVERLAPPED lpOverlapped,LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine);
__declspec(dllimport) BOOL __stdcall BackupRead(
HANDLE hFile,LPBYTE lpBuffer,DWORD nNumberOfBytesToRead,LPDWORD lpNumberOfBytesRead,BOOL bAbort,BOOL bProcessSecurity,LPVOID *lpContext);
__declspec(dllimport) BOOL __stdcall BackupSeek(
HANDLE hFile,DWORD dwLowBytesToSeek,DWORD dwHighBytesToSeek,LPDWORD lpdwLowByteSeeked,LPDWORD lpdwHighByteSeeked,LPVOID *lpContext);
__declspec(dllimport) BOOL __stdcall BackupWrite(
HANDLE hFile,LPBYTE lpBuffer,DWORD nNumberOfBytesToWrite,LPDWORD lpNumberOfBytesWritten,BOOL bAbort,BOOL bProcessSecurity,LPVOID *lpContext);
typedef struct _WIN32_STREAM_ID {
DWORD dwStreamId ;
DWORD dwStreamAttributes ;
LARGE_INTEGER Size ;
DWORD dwStreamNameSize ;
WCHAR cStreamName[ 1 ] ;
} WIN32_STREAM_ID, *LPWIN32_STREAM_ID ;
__declspec(dllimport) BOOL __stdcall ReadFileScatter(
HANDLE hFile,FILE_SEGMENT_ELEMENT aSegmentArray[],DWORD nNumberOfBytesToRead,LPDWORD lpReserved,LPOVERLAPPED lpOverlapped);
__declspec(dllimport) BOOL __stdcall WriteFileGather(
HANDLE hFile,FILE_SEGMENT_ELEMENT aSegmentArray[],DWORD nNumberOfBytesToWrite,LPDWORD lpReserved,LPOVERLAPPED lpOverlapped);
	// 3773 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
typedef struct _STARTUPINFOA {
DWORD cb;
LPSTR lpReserved;
LPSTR lpDesktop;
LPSTR lpTitle;
DWORD dwX;
DWORD dwY;
DWORD dwXSize;
DWORD dwYSize;
DWORD dwXCountChars;
DWORD dwYCountChars;
DWORD dwFillAttribute;
DWORD dwFlags;
WORD wShowWindow;
WORD cbReserved2;
LPBYTE lpReserved2;
HANDLE hStdInput;
HANDLE hStdOutput;
HANDLE hStdError;
} STARTUPINFOA, *LPSTARTUPINFOA;
typedef struct _STARTUPINFOW {
DWORD cb;
LPWSTR lpReserved;
LPWSTR lpDesktop;
LPWSTR lpTitle;
DWORD dwX;
DWORD dwY;
DWORD dwXSize;
DWORD dwYSize;
DWORD dwXCountChars;
DWORD dwYCountChars;
DWORD dwFillAttribute;
DWORD dwFlags;
WORD wShowWindow;
WORD cbReserved2;
LPBYTE lpReserved2;
HANDLE hStdInput;
HANDLE hStdOutput;
HANDLE hStdError;
} STARTUPINFOW, *LPSTARTUPINFOW;
typedef STARTUPINFOA STARTUPINFO;
typedef LPSTARTUPINFOA LPSTARTUPINFO;
	// 3821 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
typedef struct _WIN32_FIND_DATAA {
DWORD dwFileAttributes;
FILETIME ftCreationTime;
FILETIME ftLastAccessTime;
FILETIME ftLastWriteTime;
DWORD nFileSizeHigh;
DWORD nFileSizeLow;
DWORD dwReserved0;
DWORD dwReserved1;
CHAR cFileName[ 260 ];
CHAR cAlternateFileName[ 14 ];
} WIN32_FIND_DATAA, *PWIN32_FIND_DATAA, *LPWIN32_FIND_DATAA;
typedef struct _WIN32_FIND_DATAW {
DWORD dwFileAttributes;
FILETIME ftCreationTime;
FILETIME ftLastAccessTime;
FILETIME ftLastWriteTime;
DWORD nFileSizeHigh;
DWORD nFileSizeLow;
DWORD dwReserved0;
DWORD dwReserved1;
WCHAR cFileName[ 260 ];
WCHAR cAlternateFileName[ 14 ];
} WIN32_FIND_DATAW, *PWIN32_FIND_DATAW, *LPWIN32_FIND_DATAW;
typedef WIN32_FIND_DATAA WIN32_FIND_DATA;
typedef PWIN32_FIND_DATAA PWIN32_FIND_DATA;
typedef LPWIN32_FIND_DATAA LPWIN32_FIND_DATA;
	// 3867 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
typedef struct _WIN32_FILE_ATTRIBUTE_DATA {
DWORD dwFileAttributes;
FILETIME ftCreationTime;
FILETIME ftLastAccessTime;
FILETIME ftLastWriteTime;
DWORD nFileSizeHigh;
DWORD nFileSizeLow;
} WIN32_FILE_ATTRIBUTE_DATA, *LPWIN32_FILE_ATTRIBUTE_DATA;
__declspec(dllimport) HANDLE __stdcall CreateMutexA(
LPSECURITY_ATTRIBUTES lpMutexAttributes,BOOL bInitialOwner,LPCSTR lpName);
__declspec(dllimport) HANDLE __stdcall CreateMutexW(
LPSECURITY_ATTRIBUTES lpMutexAttributes,BOOL bInitialOwner,LPCWSTR lpName);
	// 3898 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HANDLE __stdcall OpenMutexA(
DWORD dwDesiredAccess,BOOL bInheritHandle,LPCSTR lpName);
__declspec(dllimport) HANDLE __stdcall OpenMutexW(
DWORD dwDesiredAccess,BOOL bInheritHandle,LPCWSTR lpName);
	// 3920 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HANDLE __stdcall CreateEventA(
LPSECURITY_ATTRIBUTES lpEventAttributes,BOOL bManualReset,BOOL bInitialState,LPCSTR lpName);
__declspec(dllimport) HANDLE __stdcall CreateEventW(
LPSECURITY_ATTRIBUTES lpEventAttributes,BOOL bManualReset,BOOL bInitialState,LPCWSTR lpName);
	// 3944 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HANDLE __stdcall OpenEventA(
DWORD dwDesiredAccess,BOOL bInheritHandle,LPCSTR lpName);
__declspec(dllimport) HANDLE __stdcall OpenEventW(
DWORD dwDesiredAccess,BOOL bInheritHandle,LPCWSTR lpName);
	// 3966 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HANDLE __stdcall CreateSemaphoreA(
LPSECURITY_ATTRIBUTES lpSemaphoreAttributes,LONG lInitialCount,LONG lMaximumCount,LPCSTR lpName);
__declspec(dllimport) HANDLE __stdcall CreateSemaphoreW(
LPSECURITY_ATTRIBUTES lpSemaphoreAttributes,LONG lInitialCount,LONG lMaximumCount,LPCWSTR lpName);
	// 3990 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HANDLE __stdcall OpenSemaphoreA(
DWORD dwDesiredAccess,BOOL bInheritHandle,LPCSTR lpName);
__declspec(dllimport) HANDLE __stdcall OpenSemaphoreW(
DWORD dwDesiredAccess,BOOL bInheritHandle,LPCWSTR lpName);
	// 4012 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 4085 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HANDLE __stdcall CreateFileMappingA(
HANDLE hFile,LPSECURITY_ATTRIBUTES lpFileMappingAttributes,DWORD flProtect,DWORD dwMaximumSizeHigh,DWORD dwMaximumSizeLow,LPCSTR lpName);
__declspec(dllimport) HANDLE __stdcall CreateFileMappingW(
HANDLE hFile,LPSECURITY_ATTRIBUTES lpFileMappingAttributes,DWORD flProtect,DWORD dwMaximumSizeHigh,DWORD dwMaximumSizeLow,LPCWSTR lpName);
	// 4113 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HANDLE __stdcall OpenFileMappingA(
DWORD dwDesiredAccess,BOOL bInheritHandle,LPCSTR lpName);
__declspec(dllimport) HANDLE __stdcall OpenFileMappingW(
DWORD dwDesiredAccess,BOOL bInheritHandle,LPCWSTR lpName);
	// 4135 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetLogicalDriveStringsA(
DWORD nBufferLength,LPSTR lpBuffer);
__declspec(dllimport) DWORD __stdcall GetLogicalDriveStringsW(
DWORD nBufferLength,LPWSTR lpBuffer);
	// 4155 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HMODULE __stdcall LoadLibraryA(
LPCSTR lpLibFileName);
__declspec(dllimport) HMODULE __stdcall LoadLibraryW(
LPCWSTR lpLibFileName);
	// 4173 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HMODULE __stdcall LoadLibraryExA(
LPCSTR lpLibFileName,HANDLE hFile,DWORD dwFlags);
__declspec(dllimport) HMODULE __stdcall LoadLibraryExW(
LPCWSTR lpLibFileName,HANDLE hFile,DWORD dwFlags);
	// 4195 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetModuleFileNameA(
HMODULE hModule,LPSTR lpFilename,DWORD nSize);
__declspec(dllimport) DWORD __stdcall GetModuleFileNameW(
HMODULE hModule,LPWSTR lpFilename,DWORD nSize);
	// 4223 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HMODULE __stdcall GetModuleHandleA(
LPCSTR lpModuleName);
__declspec(dllimport) HMODULE __stdcall GetModuleHandleW(
LPCWSTR lpModuleName);
	// 4241 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall CreateProcessA(
LPCSTR lpApplicationName,LPSTR lpCommandLine,LPSECURITY_ATTRIBUTES lpProcessAttributes,LPSECURITY_ATTRIBUTES lpThreadAttributes,BOOL bInheritHandles,DWORD dwCreationFlags,LPVOID lpEnvironment,LPCSTR lpCurrentDirectory,LPSTARTUPINFOA lpStartupInfo,LPPROCESS_INFORMATION lpProcessInformation);
__declspec(dllimport) BOOL __stdcall CreateProcessW(
LPCWSTR lpApplicationName,LPWSTR lpCommandLine,LPSECURITY_ATTRIBUTES lpProcessAttributes,LPSECURITY_ATTRIBUTES lpThreadAttributes,BOOL bInheritHandles,DWORD dwCreationFlags,LPVOID lpEnvironment,LPCWSTR lpCurrentDirectory,LPSTARTUPINFOW lpStartupInfo,LPPROCESS_INFORMATION lpProcessInformation);
	// 4277 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall SetProcessShutdownParameters(
DWORD dwLevel,DWORD dwFlags);
__declspec(dllimport) BOOL __stdcall GetProcessShutdownParameters(
LPDWORD lpdwLevel,LPDWORD lpdwFlags);
__declspec(dllimport) DWORD __stdcall GetProcessVersion(
DWORD ProcessId);
__declspec(dllimport) void __stdcall FatalAppExitA(
UINT uAction,LPCSTR lpMessageText);
__declspec(dllimport) void __stdcall FatalAppExitW(
UINT uAction,LPCWSTR lpMessageText);
	// 4320 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) void __stdcall GetStartupInfoA(
LPSTARTUPINFOA lpStartupInfo);
__declspec(dllimport) void __stdcall GetStartupInfoW(
LPSTARTUPINFOW lpStartupInfo);
	// 4338 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) LPSTR __stdcall GetCommandLineA(
void);
__declspec(dllimport) LPWSTR __stdcall GetCommandLineW(
void);
	// 4356 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetEnvironmentVariableA(
LPCSTR lpName,LPSTR lpBuffer,DWORD nSize);
__declspec(dllimport) DWORD __stdcall GetEnvironmentVariableW(
LPCWSTR lpName,LPWSTR lpBuffer,DWORD nSize);
	// 4378 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall SetEnvironmentVariableA(
LPCSTR lpName,LPCSTR lpValue);
__declspec(dllimport) BOOL __stdcall SetEnvironmentVariableW(
LPCWSTR lpName,LPCWSTR lpValue);
	// 4398 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport)
DWORD __stdcall ExpandEnvironmentStringsA(
LPCSTR lpSrc,LPSTR lpDst,DWORD nSize);
__declspec(dllimport) DWORD __stdcall ExpandEnvironmentStringsW(
LPCWSTR lpSrc,LPWSTR lpDst,DWORD nSize);
	// 4420 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) void __stdcall OutputDebugStringA(
LPCSTR lpOutputString);
__declspec(dllimport) void __stdcall OutputDebugStringW(
LPCWSTR lpOutputString);
	// 4438 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HRSRC __stdcall FindResourceA(
HMODULE hModule,LPCSTR lpName,LPCSTR lpType);
__declspec(dllimport) HRSRC __stdcall FindResourceW(
HMODULE hModule,LPCWSTR lpName,LPCWSTR lpType);
	// 4460 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HRSRC __stdcall FindResourceExA(
HMODULE hModule,LPCSTR lpType,LPCSTR lpName,WORD wLanguage);
__declspec(dllimport) HRSRC __stdcall FindResourceExW(
HMODULE hModule,LPCWSTR lpType,LPCWSTR lpName,WORD wLanguage);
	// 4484 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
typedef BOOL (__stdcall* ENUMRESTYPEPROC)(HMODULE hModule, LPTSTR lpType,LONG lParam);
typedef BOOL (__stdcall* ENUMRESNAMEPROC)(HMODULE hModule, LPCTSTR lpType,LPTSTR lpName, LONG lParam);
typedef BOOL (__stdcall* ENUMRESLANGPROC)(HMODULE hModule, LPCTSTR lpType,LPCTSTR lpName, WORD wLanguage, LONG lParam);
	// 4497 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall EnumResourceTypesA(
HMODULE hModule,ENUMRESTYPEPROC lpEnumFunc,LONG lParam);
__declspec(dllimport) BOOL __stdcall EnumResourceTypesW(
HMODULE hModule,ENUMRESTYPEPROC lpEnumFunc,LONG lParam);
	// 4519 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall EnumResourceNamesA(
HMODULE hModule,LPCSTR lpType,ENUMRESNAMEPROC lpEnumFunc,LONG lParam);
__declspec(dllimport) BOOL __stdcall EnumResourceNamesW(
HMODULE hModule,LPCWSTR lpType,ENUMRESNAMEPROC lpEnumFunc,LONG lParam);
	// 4544 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall EnumResourceLanguagesA(
HMODULE hModule,LPCSTR lpType,LPCSTR lpName,ENUMRESLANGPROC lpEnumFunc,LONG lParam);
__declspec(dllimport) BOOL __stdcall EnumResourceLanguagesW(
HMODULE hModule,LPCWSTR lpType,LPCWSTR lpName,ENUMRESLANGPROC lpEnumFunc,LONG lParam);
	// 4570 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HANDLE __stdcall BeginUpdateResourceA(
LPCSTR pFileName,BOOL bDeleteExistingResources);
__declspec(dllimport) HANDLE __stdcall BeginUpdateResourceW(
LPCWSTR pFileName,BOOL bDeleteExistingResources);
	// 4590 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall UpdateResourceA(
HANDLE hUpdate,LPCSTR lpType,LPCSTR lpName,WORD wLanguage,LPVOID lpData,DWORD cbData);
__declspec(dllimport) BOOL __stdcall UpdateResourceW(
HANDLE hUpdate,LPCWSTR lpType,LPCWSTR lpName,WORD wLanguage,LPVOID lpData,DWORD cbData);
	// 4618 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall EndUpdateResourceA(
HANDLE hUpdate,BOOL fDiscard);
__declspec(dllimport) BOOL __stdcall EndUpdateResourceW(
HANDLE hUpdate,BOOL fDiscard);
	// 4638 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) ATOM __stdcall GlobalAddAtomA(
LPCSTR lpString);
__declspec(dllimport) ATOM __stdcall GlobalAddAtomW(
LPCWSTR lpString);
	// 4656 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) ATOM __stdcall GlobalFindAtomA(
LPCSTR lpString);
__declspec(dllimport) ATOM __stdcall GlobalFindAtomW(
LPCWSTR lpString);
	// 4674 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) UINT __stdcall GlobalGetAtomNameA(
ATOM nAtom,LPSTR lpBuffer,int nSize);
__declspec(dllimport) UINT __stdcall GlobalGetAtomNameW(
ATOM nAtom,LPWSTR lpBuffer,int nSize);
	// 4696 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) ATOM __stdcall AddAtomA(
LPCSTR lpString);
__declspec(dllimport) ATOM __stdcall AddAtomW(
LPCWSTR lpString);
	// 4714 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) ATOM __stdcall FindAtomA(
LPCSTR lpString);
__declspec(dllimport) ATOM __stdcall FindAtomW(
LPCWSTR lpString);
	// 4732 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) UINT __stdcall GetAtomNameA(
ATOM nAtom,LPSTR lpBuffer,int nSize);
__declspec(dllimport) UINT __stdcall GetAtomNameW(
ATOM nAtom,LPWSTR lpBuffer,int nSize);
	// 4754 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) UINT
__stdcall
GetProfileIntA(
LPCSTR lpAppName,LPCSTR lpKeyName,INT nDefault);
__declspec(dllimport) UINT __stdcall GetProfileIntW(
LPCWSTR lpAppName,LPCWSTR lpKeyName,INT nDefault);
	// 4776 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetProfileStringA(
LPCSTR lpAppName,LPCSTR lpKeyName,LPCSTR lpDefault,LPSTR lpReturnedString,DWORD nSize);
__declspec(dllimport) DWORD __stdcall GetProfileStringW(
LPCWSTR lpAppName,LPCWSTR lpKeyName,LPCWSTR lpDefault,LPWSTR lpReturnedString,DWORD nSize);
	// 4802 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall WriteProfileStringA(
LPCSTR lpAppName,LPCSTR lpKeyName,LPCSTR lpString);
__declspec(dllimport) BOOL __stdcall WriteProfileStringW(
LPCWSTR lpAppName,LPCWSTR lpKeyName,LPCWSTR lpString);
	// 4824 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetProfileSectionA(
LPCSTR lpAppName,LPSTR lpReturnedString,DWORD nSize);
__declspec(dllimport) DWORD __stdcall GetProfileSectionW(
LPCWSTR lpAppName,LPWSTR lpReturnedString,DWORD nSize);
	// 4846 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall WriteProfileSectionA(
LPCSTR lpAppName,LPCSTR lpString);
__declspec(dllimport) BOOL __stdcall WriteProfileSectionW(
LPCWSTR lpAppName,LPCWSTR lpString);
	// 4866 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) UINT __stdcall GetPrivateProfileIntA(
LPCSTR lpAppName,LPCSTR lpKeyName,INT nDefault,LPCSTR lpFileName);
__declspec(dllimport) UINT __stdcall GetPrivateProfileIntW(
LPCWSTR lpAppName,LPCWSTR lpKeyName,INT nDefault,LPCWSTR lpFileName);
	// 4890 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetPrivateProfileStringA(
LPCSTR lpAppName,LPCSTR lpKeyName,LPCSTR lpDefault,LPSTR lpReturnedString,DWORD nSize,LPCSTR lpFileName);
__declspec(dllimport) DWORD __stdcall GetPrivateProfileStringW(
LPCWSTR lpAppName,LPCWSTR lpKeyName,LPCWSTR lpDefault,LPWSTR lpReturnedString,DWORD nSize,LPCWSTR lpFileName);
	// 4918 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall WritePrivateProfileStringA(
LPCSTR lpAppName,LPCSTR lpKeyName,LPCSTR lpString,LPCSTR lpFileName);
__declspec(dllimport) BOOL __stdcall WritePrivateProfileStringW(
LPCWSTR lpAppName,LPCWSTR lpKeyName,LPCWSTR lpString,LPCWSTR lpFileName);
	// 4942 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetPrivateProfileSectionA(
LPCSTR lpAppName,LPSTR lpReturnedString,DWORD nSize,LPCSTR lpFileName);
__declspec(dllimport) DWORD __stdcall GetPrivateProfileSectionW(
LPCWSTR lpAppName,LPWSTR lpReturnedString,DWORD nSize,LPCWSTR lpFileName);
	// 4966 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall WritePrivateProfileSectionA(
LPCSTR lpAppName,LPCSTR lpString,LPCSTR lpFileName);
__declspec(dllimport) BOOL __stdcall WritePrivateProfileSectionW(
LPCWSTR lpAppName,LPCWSTR lpString,LPCWSTR lpFileName);
	// 4988 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetPrivateProfileSectionNamesA(
LPSTR lpszReturnBuffer,DWORD nSize,LPCSTR lpFileName);
__declspec(dllimport) DWORD __stdcall GetPrivateProfileSectionNamesW(
LPWSTR lpszReturnBuffer,DWORD nSize,LPCWSTR lpFileName);
	// 5011 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall GetPrivateProfileStructA(
LPCSTR lpszSection,LPCSTR lpszKey,LPVOID lpStruct,UINT uSizeStruct,LPCSTR szFile);
__declspec(dllimport) BOOL __stdcall GetPrivateProfileStructW(
LPCWSTR lpszSection,LPCWSTR lpszKey,LPVOID lpStruct,UINT uSizeStruct,LPCWSTR szFile);
	// 5037 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall WritePrivateProfileStructA(
LPCSTR lpszSection,LPCSTR lpszKey,LPVOID lpStruct,UINT uSizeStruct,LPCSTR szFile);
__declspec(dllimport) BOOL __stdcall WritePrivateProfileStructW(
LPCWSTR lpszSection,LPCWSTR lpszKey,LPVOID lpStruct,UINT uSizeStruct,LPCWSTR szFile);
	// 5063 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) UINT __stdcall GetDriveTypeA(
LPCSTR lpRootPathName);
__declspec(dllimport) UINT __stdcall GetDriveTypeW(
LPCWSTR lpRootPathName);
	// 5082 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) UINT __stdcall GetSystemDirectoryA(
LPSTR lpBuffer,UINT uSize);
__declspec(dllimport) UINT __stdcall GetSystemDirectoryW(
LPWSTR lpBuffer,UINT uSize);
	// 5102 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetTempPathA(
DWORD nBufferLength,LPSTR lpBuffer);
__declspec(dllimport) DWORD __stdcall GetTempPathW(
DWORD nBufferLength,LPWSTR lpBuffer);
	// 5122 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) UINT __stdcall GetTempFileNameA(
LPCSTR lpPathName,LPCSTR lpPrefixString,UINT uUnique,LPSTR lpTempFileName);
__declspec(dllimport) UINT __stdcall GetTempFileNameW(
LPCWSTR lpPathName,LPCWSTR lpPrefixString,UINT uUnique,LPWSTR lpTempFileName);
	// 5146 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) UINT __stdcall GetWindowsDirectoryA(
LPSTR lpBuffer,UINT uSize);
__declspec(dllimport) UINT __stdcall GetWindowsDirectoryW(
LPWSTR lpBuffer,UINT uSize);
	// 5166 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall SetCurrentDirectoryA(
LPCSTR lpPathName);
__declspec(dllimport) BOOL __stdcall SetCurrentDirectoryW(
LPCWSTR lpPathName);
	// 5184 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetCurrentDirectoryA(
DWORD nBufferLength,LPSTR lpBuffer);
__declspec(dllimport) DWORD __stdcall GetCurrentDirectoryW(
DWORD nBufferLength,LPWSTR lpBuffer);
	// 5204 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall GetDiskFreeSpaceA(
LPCSTR lpRootPathName,LPDWORD lpSectorsPerCluster,LPDWORD lpBytesPerSector,LPDWORD lpNumberOfFreeClusters,LPDWORD lpTotalNumberOfClusters);
__declspec(dllimport) BOOL __stdcall GetDiskFreeSpaceW(
LPCWSTR lpRootPathName,LPDWORD lpSectorsPerCluster,LPDWORD lpBytesPerSector,LPDWORD lpNumberOfFreeClusters,LPDWORD lpTotalNumberOfClusters);
	// 5230 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall GetDiskFreeSpaceExA(
LPCSTR lpDirectoryName,PULARGE_INTEGER lpFreeBytesAvailableToCaller,PULARGE_INTEGER lpTotalNumberOfBytes,PULARGE_INTEGER lpTotalNumberOfFreeBytes);
__declspec(dllimport) BOOL __stdcall GetDiskFreeSpaceExW(
LPCWSTR lpDirectoryName,PULARGE_INTEGER lpFreeBytesAvailableToCaller,PULARGE_INTEGER lpTotalNumberOfBytes,PULARGE_INTEGER lpTotalNumberOfFreeBytes);
	// 5254 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall CreateDirectoryA(
LPCSTR lpPathName,LPSECURITY_ATTRIBUTES lpSecurityAttributes);
__declspec(dllimport) BOOL __stdcall CreateDirectoryW(
LPCWSTR lpPathName,LPSECURITY_ATTRIBUTES lpSecurityAttributes);
	// 5274 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall CreateDirectoryExA(
LPCSTR lpTemplateDirectory,LPCSTR lpNewDirectory,LPSECURITY_ATTRIBUTES lpSecurityAttributes);
__declspec(dllimport) BOOL __stdcall CreateDirectoryExW(
LPCWSTR lpTemplateDirectory,LPCWSTR lpNewDirectory,LPSECURITY_ATTRIBUTES lpSecurityAttributes);
	// 5296 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall RemoveDirectoryA(
LPCSTR lpPathName);
__declspec(dllimport) BOOL __stdcall RemoveDirectoryW(
LPCWSTR lpPathName);
	// 5314 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetFullPathNameA(
LPCSTR lpFileName,DWORD nBufferLength,LPSTR lpBuffer,LPSTR *lpFilePart);
__declspec(dllimport) DWORD __stdcall GetFullPathNameW(
LPCWSTR lpFileName,DWORD nBufferLength,LPWSTR lpBuffer,LPWSTR *lpFilePart);
	// 5338 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall DefineDosDeviceA(
DWORD dwFlags,LPCSTR lpDeviceName,LPCSTR lpTargetPath);
__declspec(dllimport) BOOL __stdcall DefineDosDeviceW(
DWORD dwFlags,LPCWSTR lpDeviceName,LPCWSTR lpTargetPath);
	// 5366 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall QueryDosDeviceA(
LPCSTR lpDeviceName,LPSTR lpTargetPath,DWORD ucchMax);
__declspec(dllimport) DWORD __stdcall QueryDosDeviceW(
LPCWSTR lpDeviceName,LPWSTR lpTargetPath,DWORD ucchMax);
	// 5388 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HANDLE __stdcall CreateFileA(
LPCSTR lpFileName,DWORD dwDesiredAccess,DWORD dwShareMode,LPSECURITY_ATTRIBUTES lpSecurityAttributes,DWORD dwCreationDisposition,DWORD dwFlagsAndAttributes,HANDLE hTemplateFile);
__declspec(dllimport) HANDLE __stdcall CreateFileW(
LPCWSTR lpFileName,DWORD dwDesiredAccess,DWORD dwShareMode,LPSECURITY_ATTRIBUTES lpSecurityAttributes,DWORD dwCreationDisposition,DWORD dwFlagsAndAttributes,HANDLE hTemplateFile);
	// 5420 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall SetFileAttributesA(
LPCSTR lpFileName,DWORD dwFileAttributes);
__declspec(dllimport) BOOL __stdcall SetFileAttributesW(
LPCWSTR lpFileName,DWORD dwFileAttributes);
	// 5440 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetFileAttributesA(
LPCSTR lpFileName);
__declspec(dllimport) DWORD __stdcall GetFileAttributesW(
LPCWSTR lpFileName);
	// 5458 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
typedef enum _GET_FILEEX_INFO_LEVELS {
GetFileExInfoStandard,GetFileExMaxInfoLevel
} GET_FILEEX_INFO_LEVELS;
__declspec(dllimport) BOOL __stdcall GetFileAttributesExA(
LPCSTR lpFileName,GET_FILEEX_INFO_LEVELS fInfoLevelId,LPVOID lpFileInformation);
__declspec(dllimport) BOOL __stdcall GetFileAttributesExW(
LPCWSTR lpFileName,GET_FILEEX_INFO_LEVELS fInfoLevelId,LPVOID lpFileInformation);
	// 5485 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall GetCompressedFileSizeA(
LPCSTR lpFileName,LPDWORD lpFileSizeHigh);
__declspec(dllimport) DWORD __stdcall GetCompressedFileSizeW(
LPCWSTR lpFileName,LPDWORD lpFileSizeHigh);
	// 5505 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall DeleteFileA(
LPCSTR lpFileName);
__declspec(dllimport) BOOL
__stdcall
DeleteFileW(
LPCWSTR lpFileName);
	// 5523 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 5567 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HANDLE __stdcall FindFirstFileA(
LPCSTR lpFileName,LPWIN32_FIND_DATAA lpFindFileData);
__declspec(dllimport) HANDLE __stdcall FindFirstFileW(
LPCWSTR lpFileName,LPWIN32_FIND_DATAW lpFindFileData);
	// 5587 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall FindNextFileA(
HANDLE hFindFile,LPWIN32_FIND_DATAA lpFindFileData);
__declspec(dllimport) BOOL __stdcall FindNextFileW(
HANDLE hFindFile,LPWIN32_FIND_DATAW lpFindFileData);
	// 5607 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) DWORD __stdcall SearchPathA(
LPCSTR lpPath,LPCSTR lpFileName,LPCSTR lpExtension,DWORD nBufferLength,LPSTR lpBuffer,LPSTR *lpFilePart);
__declspec(dllimport) DWORD
__stdcall
SearchPathW(
LPCWSTR lpPath,LPCWSTR lpFileName,LPCWSTR lpExtension,DWORD nBufferLength,LPWSTR lpBuffer,LPWSTR *lpFilePart);
	// 5635 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall CopyFileA(
LPCSTR lpExistingFileName,LPCSTR lpNewFileName,BOOL bFailIfExists);
__declspec(dllimport) BOOL __stdcall CopyFileW(
LPCWSTR lpExistingFileName,LPCWSTR lpNewFileName,BOOL bFailIfExists);
	// 5657 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 5701 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall MoveFileA(
LPCSTR lpExistingFileName,LPCSTR lpNewFileName);
__declspec(dllimport) BOOL __stdcall MoveFileW(
LPCWSTR lpExistingFileName,LPCWSTR lpNewFileName);
	// 5721 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall MoveFileExA(
LPCSTR lpExistingFileName,LPCSTR lpNewFileName,DWORD dwFlags);
__declspec(dllimport) BOOL __stdcall MoveFileExW(
LPCWSTR lpExistingFileName,LPCWSTR lpNewFileName,DWORD dwFlags);
	// 5743 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 5771 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 5780 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 5810 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HANDLE __stdcall CreateNamedPipeA(
LPCSTR lpName,DWORD dwOpenMode,DWORD dwPipeMode,DWORD nMaxInstances,DWORD nOutBufferSize,DWORD nInBufferSize,DWORD nDefaultTimeOut,LPSECURITY_ATTRIBUTES lpSecurityAttributes);
__declspec(dllimport) HANDLE __stdcall CreateNamedPipeW(
LPCWSTR lpName,DWORD dwOpenMode,DWORD dwPipeMode,DWORD nMaxInstances,DWORD nOutBufferSize,DWORD nInBufferSize,DWORD nDefaultTimeOut,LPSECURITY_ATTRIBUTES lpSecurityAttributes);
	// 5843 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall GetNamedPipeHandleStateA(
HANDLE hNamedPipe,LPDWORD lpState,LPDWORD lpCurInstances,LPDWORD lpMaxCollectionCount,LPDWORD lpCollectDataTimeout,LPSTR lpUserName,DWORD nMaxUserNameSize);
__declspec(dllimport) BOOL __stdcall GetNamedPipeHandleStateW(
HANDLE hNamedPipe,LPDWORD lpState,LPDWORD lpCurInstances,LPDWORD lpMaxCollectionCount,LPDWORD lpCollectDataTimeout,LPWSTR lpUserName,DWORD nMaxUserNameSize);
	// 5873 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall CallNamedPipeA(
LPCSTR lpNamedPipeName,LPVOID lpInBuffer,DWORD nInBufferSize,LPVOID lpOutBuffer,DWORD nOutBufferSize,LPDWORD lpBytesRead,DWORD nTimeOut);
__declspec(dllimport) BOOL __stdcall CallNamedPipeW(
LPCWSTR lpNamedPipeName,LPVOID lpInBuffer,DWORD nInBufferSize,LPVOID lpOutBuffer,DWORD nOutBufferSize,LPDWORD lpBytesRead,DWORD nTimeOut);
	// 5903 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall WaitNamedPipeA(
LPCSTR lpNamedPipeName,DWORD nTimeOut);
__declspec(dllimport) BOOL __stdcall WaitNamedPipeW(
LPCWSTR lpNamedPipeName,DWORD nTimeOut);
	// 5923 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall SetVolumeLabelA(
LPCSTR lpRootPathName,LPCSTR lpVolumeName);
__declspec(dllimport) BOOL __stdcall SetVolumeLabelW(
LPCWSTR lpRootPathName,LPCWSTR lpVolumeName);
	// 5943 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) void __stdcall SetFileApisToOEM( void );
__declspec(dllimport) void __stdcall SetFileApisToANSI( void );
__declspec(dllimport) BOOL __stdcall AreFileApisANSI( void );
__declspec(dllimport) BOOL __stdcall GetVolumeInformationA(
LPCSTR lpRootPathName,LPSTR lpVolumeNameBuffer,DWORD nVolumeNameSize,LPDWORD lpVolumeSerialNumber,LPDWORD lpMaximumComponentLength,LPDWORD lpFileSystemFlags,LPSTR lpFileSystemNameBuffer,DWORD nFileSystemNameSize);
__declspec(dllimport) BOOL __stdcall GetVolumeInformationW(
LPCWSTR lpRootPathName,LPWSTR lpVolumeNameBuffer,DWORD nVolumeNameSize,LPDWORD lpVolumeSerialNumber,LPDWORD lpMaximumComponentLength,LPDWORD lpFileSystemFlags,LPWSTR lpFileSystemNameBuffer,DWORD nFileSystemNameSize);
	// 5990 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall CancelIo(
HANDLE hFile);
__declspec(dllimport) BOOL __stdcall ClearEventLogA (
HANDLE hEventLog,LPCSTR lpBackupFileName);
__declspec(dllimport) BOOL __stdcall ClearEventLogW (
HANDLE hEventLog,LPCWSTR lpBackupFileName);
	// 6021 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall BackupEventLogA (
HANDLE hEventLog,LPCSTR lpBackupFileName);
__declspec(dllimport) BOOL __stdcall BackupEventLogW (
HANDLE hEventLog,LPCWSTR lpBackupFileName);
	// 6041 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall CloseEventLog (
HANDLE hEventLog);
__declspec(dllimport) BOOL __stdcall DeregisterEventSource (
HANDLE hEventLog);
__declspec(dllimport) BOOL __stdcall NotifyChangeEventLog(
HANDLE hEventLog,HANDLE hEvent);
__declspec(dllimport) BOOL __stdcall GetNumberOfEventLogRecords (
HANDLE hEventLog,PDWORD NumberOfRecords);
__declspec(dllimport) BOOL __stdcall GetOldestEventLogRecord (
HANDLE hEventLog,PDWORD OldestRecord);
__declspec(dllimport) HANDLE __stdcall OpenEventLogA (
LPCSTR lpUNCServerName,LPCSTR lpSourceName);
__declspec(dllimport) HANDLE __stdcall OpenEventLogW (
LPCWSTR lpUNCServerName,LPCWSTR lpSourceName);
	// 6099 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HANDLE __stdcall RegisterEventSourceA (
LPCSTR lpUNCServerName,LPCSTR lpSourceName);
__declspec(dllimport) HANDLE __stdcall RegisterEventSourceW (
LPCWSTR lpUNCServerName,LPCWSTR lpSourceName);
	// 6119 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) HANDLE __stdcall OpenBackupEventLogA (
LPCSTR lpUNCServerName,LPCSTR lpFileName);
__declspec(dllimport) HANDLE __stdcall OpenBackupEventLogW (
LPCWSTR lpUNCServerName,LPCWSTR lpFileName);
	// 6139 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall ReadEventLogA (
HANDLE hEventLog,DWORD dwReadFlags,DWORD dwRecordOffset,LPVOID lpBuffer,DWORD nNumberOfBytesToRead,DWORD *pnBytesRead,DWORD *pnMinNumberOfBytesNeeded);
__declspec(dllimport) BOOL __stdcall ReadEventLogW (
HANDLE hEventLog,DWORD dwReadFlags,DWORD dwRecordOffset,LPVOID lpBuffer,DWORD nNumberOfBytesToRead,DWORD *pnBytesRead,DWORD *pnMinNumberOfBytesNeeded);
	// 6169 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall ReportEventA (
HANDLE hEventLog,WORD wType,WORD wCategory,DWORD dwEventID,PSID lpUserSid,WORD wNumStrings,DWORD dwDataSize,LPCSTR *lpStrings,LPVOID lpRawData);
__declspec(dllimport) BOOL __stdcall ReportEventW (
HANDLE hEventLog,WORD wType,WORD wCategory,DWORD dwEventID,PSID lpUserSid,WORD wNumStrings,DWORD dwDataSize,LPCWSTR *lpStrings,LPVOID lpRawData);
	// 6203 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall DuplicateToken(
HANDLE ExistingTokenHandle,SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,PHANDLE DuplicateTokenHandle);
__declspec(dllimport) BOOL __stdcall GetKernelObjectSecurity (
HANDLE Handle,SECURITY_INFORMATION RequestedInformation,PSECURITY_DESCRIPTOR pSecurityDescriptor,DWORD nLength,LPDWORD lpnLengthNeeded);
__declspec(dllimport) BOOL __stdcall ImpersonateNamedPipeClient(
HANDLE hNamedPipe);
__declspec(dllimport) BOOL __stdcall ImpersonateSelf(
SECURITY_IMPERSONATION_LEVEL ImpersonationLevel);
__declspec(dllimport) BOOL __stdcall RevertToSelf (
void);
__declspec(dllimport) BOOL __stdcall SetThreadToken (
PHANDLE Thread,HANDLE Token);
__declspec(dllimport) BOOL __stdcall AccessCheck (
PSECURITY_DESCRIPTOR pSecurityDescriptor,HANDLE ClientToken,DWORD DesiredAccess,PGENERIC_MAPPING GenericMapping,PPRIVILEGE_SET PrivilegeSet,LPDWORD PrivilegeSetLength,LPDWORD GrantedAccess,LPBOOL AccessStatus);
	// 6309 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall OpenProcessToken (
HANDLE ProcessHandle,DWORD DesiredAccess,PHANDLE TokenHandle);
__declspec(dllimport) BOOL __stdcall OpenThreadToken (
HANDLE ThreadHandle,DWORD DesiredAccess,BOOL OpenAsSelf,PHANDLE TokenHandle);
__declspec(dllimport) BOOL __stdcall GetTokenInformation (
HANDLE TokenHandle,TOKEN_INFORMATION_CLASS TokenInformationClass,LPVOID TokenInformation,DWORD TokenInformationLength,PDWORD ReturnLength);
__declspec(dllimport) BOOL __stdcall SetTokenInformation (
HANDLE TokenHandle,TOKEN_INFORMATION_CLASS TokenInformationClass,LPVOID TokenInformation,DWORD TokenInformationLength);
__declspec(dllimport) BOOL __stdcall AdjustTokenPrivileges (
HANDLE TokenHandle,BOOL DisableAllPrivileges,PTOKEN_PRIVILEGES NewState,DWORD BufferLength,PTOKEN_PRIVILEGES PreviousState,PDWORD ReturnLength);
__declspec(dllimport) BOOL __stdcall AdjustTokenGroups (
HANDLE TokenHandle,BOOL ResetToDefault,PTOKEN_GROUPS NewState,DWORD BufferLength,PTOKEN_GROUPS PreviousState,PDWORD ReturnLength);
__declspec(dllimport) BOOL __stdcall PrivilegeCheck (
HANDLE ClientToken,PPRIVILEGE_SET RequiredPrivileges,LPBOOL pfResult);
__declspec(dllimport) BOOL __stdcall AccessCheckAndAuditAlarmA (
LPCSTR SubsystemName,LPVOID HandleId,LPSTR ObjectTypeName,LPSTR ObjectName,PSECURITY_DESCRIPTOR SecurityDescriptor,DWORD DesiredAccess,PGENERIC_MAPPING GenericMapping,BOOL ObjectCreation,LPDWORD GrantedAccess,LPBOOL AccessStatus,LPBOOL pfGenerateOnClose);
__declspec(dllimport) BOOL __stdcall AccessCheckAndAuditAlarmW (
LPCWSTR SubsystemName,LPVOID HandleId,LPWSTR ObjectTypeName,LPWSTR ObjectName,PSECURITY_DESCRIPTOR SecurityDescriptor,DWORD DesiredAccess,PGENERIC_MAPPING GenericMapping,BOOL ObjectCreation,LPDWORD GrantedAccess,LPBOOL AccessStatus,LPBOOL pfGenerateOnClose);
	// 6428 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 6528 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall ObjectOpenAuditAlarmA (
LPCSTR SubsystemName,LPVOID HandleId,LPSTR ObjectTypeName,LPSTR ObjectName,PSECURITY_DESCRIPTOR pSecurityDescriptor,HANDLE ClientToken,DWORD DesiredAccess,DWORD GrantedAccess,PPRIVILEGE_SET Privileges,BOOL ObjectCreation,BOOL AccessGranted,LPBOOL GenerateOnClose);
__declspec(dllimport) BOOL __stdcall ObjectOpenAuditAlarmW (
LPCWSTR SubsystemName,LPVOID HandleId,LPWSTR ObjectTypeName,LPWSTR ObjectName,PSECURITY_DESCRIPTOR pSecurityDescriptor,HANDLE ClientToken,DWORD DesiredAccess,DWORD GrantedAccess,PPRIVILEGE_SET Privileges,BOOL ObjectCreation,BOOL AccessGranted,LPBOOL GenerateOnClose);
	// 6569 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall ObjectPrivilegeAuditAlarmA (
LPCSTR SubsystemName,LPVOID HandleId,HANDLE ClientToken,DWORD DesiredAccess,PPRIVILEGE_SET Privileges,BOOL AccessGranted);
__declspec(dllimport) BOOL __stdcall ObjectPrivilegeAuditAlarmW (
LPCWSTR SubsystemName,LPVOID HandleId,HANDLE ClientToken,DWORD DesiredAccess,PPRIVILEGE_SET Privileges,BOOL AccessGranted);
	// 6598 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall ObjectCloseAuditAlarmA (
LPCSTR SubsystemName,LPVOID HandleId,BOOL GenerateOnClose);
__declspec(dllimport) BOOL __stdcall ObjectCloseAuditAlarmW (
LPCWSTR SubsystemName,LPVOID HandleId,BOOL GenerateOnClose);
	// 6621 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall ObjectDeleteAuditAlarmA (
LPCSTR SubsystemName,LPVOID HandleId,BOOL GenerateOnClose);
__declspec(dllimport) BOOL __stdcall ObjectDeleteAuditAlarmW (
LPCWSTR SubsystemName,LPVOID HandleId,BOOL GenerateOnClose);
	// 6644 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall PrivilegedServiceAuditAlarmA (
LPCSTR SubsystemName,LPCSTR ServiceName,HANDLE ClientToken,PPRIVILEGE_SET Privileges,BOOL AccessGranted);
__declspec(dllimport) BOOL __stdcall PrivilegedServiceAuditAlarmW (
LPCWSTR SubsystemName,LPCWSTR ServiceName,HANDLE ClientToken,PPRIVILEGE_SET Privileges,BOOL AccessGranted);
	// 6671 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall IsValidSid (
PSID pSid);
__declspec(dllimport) BOOL __stdcall EqualSid (
PSID pSid1,PSID pSid2);
__declspec(dllimport) BOOL __stdcall EqualPrefixSid (
PSID pSid1,PSID pSid2);
__declspec(dllimport) DWORD __stdcall GetSidLengthRequired (
UCHAR nSubAuthorityCount);
__declspec(dllimport) BOOL __stdcall AllocateAndInitializeSid (
PSID_IDENTIFIER_AUTHORITY pIdentifierAuthority,BYTE nSubAuthorityCount,DWORD nSubAuthority0,DWORD nSubAuthority1,DWORD nSubAuthority2,DWORD nSubAuthority3,DWORD nSubAuthority4,DWORD nSubAuthority5,DWORD nSubAuthority6,DWORD nSubAuthority7,PSID *pSid);
__declspec(dllimport) PVOID __stdcall FreeSid(
PSID pSid);
__declspec(dllimport) BOOL __stdcall InitializeSid (
PSID Sid,PSID_IDENTIFIER_AUTHORITY pIdentifierAuthority,BYTE nSubAuthorityCount);
__declspec(dllimport) PSID_IDENTIFIER_AUTHORITY __stdcall GetSidIdentifierAuthority (
PSID pSid);
__declspec(dllimport) PDWORD __stdcall GetSidSubAuthority (
PSID pSid,DWORD nSubAuthority);
__declspec(dllimport) PUCHAR
__stdcall
GetSidSubAuthorityCount (
PSID pSid);
__declspec(dllimport) DWORD __stdcall GetLengthSid (
PSID pSid);
__declspec(dllimport) BOOL __stdcall CopySid (
DWORD nDestinationSidLength,PSID pDestinationSid,PSID pSourceSid);
__declspec(dllimport) BOOL __stdcall AreAllAccessesGranted (
DWORD GrantedAccess,DWORD DesiredAccess);
__declspec(dllimport) BOOL __stdcall AreAnyAccessesGranted (
DWORD GrantedAccess,DWORD DesiredAccess);
__declspec(dllimport) void __stdcall MapGenericMask (
PDWORD AccessMask,PGENERIC_MAPPING GenericMapping);
__declspec(dllimport) BOOL __stdcall IsValidAcl (
PACL pAcl);
__declspec(dllimport) BOOL __stdcall InitializeAcl (
PACL pAcl,DWORD nAclLength,DWORD dwAclRevision);
__declspec(dllimport) BOOL __stdcall GetAclInformation (
PACL pAcl,LPVOID pAclInformation,DWORD nAclInformationLength,ACL_INFORMATION_CLASS dwAclInformationClass);
__declspec(dllimport) BOOL __stdcall SetAclInformation (
PACL pAcl,LPVOID pAclInformation,DWORD nAclInformationLength,ACL_INFORMATION_CLASS dwAclInformationClass);
__declspec(dllimport) BOOL __stdcall AddAce (
PACL pAcl,DWORD dwAceRevision,DWORD dwStartingAceIndex,LPVOID pAceList,DWORD nAceListLength);
__declspec(dllimport) BOOL __stdcall DeleteAce (
PACL pAcl,DWORD dwAceIndex);
__declspec(dllimport) BOOL __stdcall GetAce (
PACL pAcl,DWORD dwAceIndex,LPVOID *pAce);
__declspec(dllimport) BOOL __stdcall AddAccessAllowedAce (
PACL pAcl,DWORD dwAceRevision,DWORD AccessMask,PSID pSid);
	// 6904 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall AddAccessDeniedAce (
PACL pAcl,DWORD dwAceRevision,DWORD AccessMask,PSID pSid);
	// 6928 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall AddAuditAccessAce(
PACL pAcl,DWORD dwAceRevision,DWORD dwAccessMask,PSID pSid,BOOL bAuditSuccess,BOOL bAuditFailure);
	// 6996 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall FindFirstFreeAce (
PACL pAcl,LPVOID *pAce);
__declspec(dllimport) BOOL __stdcall InitializeSecurityDescriptor (
PSECURITY_DESCRIPTOR pSecurityDescriptor,DWORD dwRevision);
__declspec(dllimport) BOOL __stdcall IsValidSecurityDescriptor (
PSECURITY_DESCRIPTOR pSecurityDescriptor);
__declspec(dllimport) DWORD __stdcall GetSecurityDescriptorLength (
PSECURITY_DESCRIPTOR pSecurityDescriptor);
__declspec(dllimport) BOOL __stdcall GetSecurityDescriptorControl (
PSECURITY_DESCRIPTOR pSecurityDescriptor,PSECURITY_DESCRIPTOR_CONTROL pControl,LPDWORD lpdwRevision);
	// 7050 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall SetSecurityDescriptorDacl (
PSECURITY_DESCRIPTOR pSecurityDescriptor,BOOL bDaclPresent,PACL pDacl,BOOL bDaclDefaulted);
__declspec(dllimport) BOOL __stdcall GetSecurityDescriptorDacl (
PSECURITY_DESCRIPTOR pSecurityDescriptor,LPBOOL lpbDaclPresent,PACL *pDacl,LPBOOL lpbDaclDefaulted);
__declspec(dllimport) BOOL __stdcall SetSecurityDescriptorSacl (
PSECURITY_DESCRIPTOR pSecurityDescriptor,BOOL bSaclPresent,PACL pSacl,BOOL bSaclDefaulted);
__declspec(dllimport) BOOL __stdcall GetSecurityDescriptorSacl (
PSECURITY_DESCRIPTOR pSecurityDescriptor,LPBOOL lpbSaclPresent,PACL *pSacl,LPBOOL lpbSaclDefaulted);
__declspec(dllimport) BOOL __stdcall SetSecurityDescriptorOwner (
PSECURITY_DESCRIPTOR pSecurityDescriptor,PSID pOwner,BOOL bOwnerDefaulted);
__declspec(dllimport) BOOL __stdcall GetSecurityDescriptorOwner (
PSECURITY_DESCRIPTOR pSecurityDescriptor,PSID *pOwner,LPBOOL lpbOwnerDefaulted);
__declspec(dllimport) BOOL __stdcall SetSecurityDescriptorGroup (
PSECURITY_DESCRIPTOR pSecurityDescriptor,PSID pGroup,BOOL bGroupDefaulted);
__declspec(dllimport) BOOL __stdcall GetSecurityDescriptorGroup (
PSECURITY_DESCRIPTOR pSecurityDescriptor,PSID *pGroup,LPBOOL lpbGroupDefaulted);
__declspec(dllimport) BOOL __stdcall CreatePrivateObjectSecurity (
PSECURITY_DESCRIPTOR ParentDescriptor,PSECURITY_DESCRIPTOR CreatorDescriptor,PSECURITY_DESCRIPTOR* NewDescriptor,BOOL IsDirectoryObject,HANDLE Token,PGENERIC_MAPPING GenericMapping);
	// 7175 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall SetPrivateObjectSecurity (
SECURITY_INFORMATION SecurityInformation,PSECURITY_DESCRIPTOR ModificationDescriptor,PSECURITY_DESCRIPTOR *ObjectsSecurityDescriptor,PGENERIC_MAPPING GenericMapping,HANDLE Token);
	// 7200 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall GetPrivateObjectSecurity (
PSECURITY_DESCRIPTOR ObjectDescriptor,SECURITY_INFORMATION SecurityInformation,PSECURITY_DESCRIPTOR ResultantDescriptor,DWORD DescriptorLength,PDWORD ReturnLength);
__declspec(dllimport) BOOL __stdcall DestroyPrivateObjectSecurity (
PSECURITY_DESCRIPTOR* ObjectDescriptor);
__declspec(dllimport) BOOL __stdcall MakeSelfRelativeSD (
PSECURITY_DESCRIPTOR pAbsoluteSecurityDescriptor,PSECURITY_DESCRIPTOR pSelfRelativeSecurityDescriptor,LPDWORD lpdwBufferLength);
__declspec(dllimport) BOOL __stdcall MakeAbsoluteSD (
PSECURITY_DESCRIPTOR pSelfRelativeSecurityDescriptor,PSECURITY_DESCRIPTOR pAbsoluteSecurityDescriptor,LPDWORD lpdwAbsoluteSecurityDescriptorSize,PACL pDacl,LPDWORD lpdwDaclSize,PACL pSacl,LPDWORD lpdwSaclSize,PSID pOwner,LPDWORD lpdwOwnerSize,PSID pPrimaryGroup,LPDWORD lpdwPrimaryGroupSize);
__declspec(dllimport) BOOL __stdcall SetFileSecurityA (
LPCSTR lpFileName,SECURITY_INFORMATION SecurityInformation,PSECURITY_DESCRIPTOR pSecurityDescriptor);
__declspec(dllimport) BOOL __stdcall SetFileSecurityW (
LPCWSTR lpFileName,SECURITY_INFORMATION SecurityInformation,PSECURITY_DESCRIPTOR pSecurityDescriptor);
	// 7270 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall GetFileSecurityA (
LPCSTR lpFileName,SECURITY_INFORMATION RequestedInformation,PSECURITY_DESCRIPTOR pSecurityDescriptor,DWORD nLength,LPDWORD lpnLengthNeeded);
__declspec(dllimport) BOOL __stdcall GetFileSecurityW (
LPCWSTR lpFileName,SECURITY_INFORMATION RequestedInformation,PSECURITY_DESCRIPTOR pSecurityDescriptor,DWORD nLength,LPDWORD lpnLengthNeeded);
	// 7297 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall SetKernelObjectSecurity (
HANDLE Handle,SECURITY_INFORMATION SecurityInformation,PSECURITY_DESCRIPTOR SecurityDescriptor);
__declspec(dllimport) HANDLE __stdcall FindFirstChangeNotificationA(
LPCSTR lpPathName,BOOL bWatchSubtree,DWORD dwNotifyFilter);
__declspec(dllimport) HANDLE __stdcall FindFirstChangeNotificationW(
LPCWSTR lpPathName,BOOL bWatchSubtree,DWORD dwNotifyFilter);
	// 7331 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall FindNextChangeNotification(
HANDLE hChangeHandle);
__declspec(dllimport) BOOL __stdcall FindCloseChangeNotification(
HANDLE hChangeHandle);
	// 7361 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall VirtualLock(
LPVOID lpAddress,DWORD dwSize);
__declspec(dllimport) BOOL __stdcall VirtualUnlock(
LPVOID lpAddress,DWORD dwSize);
__declspec(dllimport) LPVOID __stdcall MapViewOfFileEx(
HANDLE hFileMappingObject,DWORD dwDesiredAccess,DWORD dwFileOffsetHigh,DWORD dwFileOffsetLow,DWORD dwNumberOfBytesToMap,LPVOID lpBaseAddress);
__declspec(dllimport) BOOL __stdcall SetPriorityClass(
HANDLE hProcess,DWORD dwPriorityClass);
__declspec(dllimport) DWORD __stdcall GetPriorityClass(
HANDLE hProcess);
__declspec(dllimport) BOOL __stdcall IsBadReadPtr(
const void *lp,UINT ucb);
__declspec(dllimport) BOOL __stdcall IsBadWritePtr(
LPVOID lp,UINT ucb);
__declspec(dllimport) BOOL __stdcall IsBadHugeReadPtr(
const void *lp,UINT ucb);
__declspec(dllimport) BOOL __stdcall IsBadHugeWritePtr(
LPVOID lp,UINT ucb);
__declspec(dllimport) BOOL __stdcall IsBadCodePtr(
FARPROC lpfn);
__declspec(dllimport) BOOL __stdcall IsBadStringPtrA(
LPCSTR lpsz,UINT ucchMax);
__declspec(dllimport) BOOL __stdcall IsBadStringPtrW(
LPCWSTR lpsz,UINT ucchMax);
	// 7463 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall LookupAccountSidA(
LPCSTR lpSystemName,PSID Sid,LPSTR Name,LPDWORD cbName,LPSTR ReferencedDomainName,LPDWORD cbReferencedDomainName,PSID_NAME_USE peUse);
__declspec(dllimport) BOOL __stdcall LookupAccountSidW(
LPCWSTR lpSystemName,PSID Sid,LPWSTR Name,LPDWORD cbName,LPWSTR ReferencedDomainName,LPDWORD cbReferencedDomainName,PSID_NAME_USE peUse);
	// 7493 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall LookupAccountNameA(
LPCSTR lpSystemName,LPCSTR lpAccountName,PSID Sid,LPDWORD cbSid,LPSTR ReferencedDomainName,LPDWORD cbReferencedDomainName,PSID_NAME_USE peUse);
__declspec(dllimport) BOOL __stdcall LookupAccountNameW(
LPCWSTR lpSystemName,LPCWSTR lpAccountName,PSID Sid,LPDWORD cbSid,LPWSTR ReferencedDomainName,LPDWORD cbReferencedDomainName,PSID_NAME_USE peUse);
	// 7523 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall LookupPrivilegeValueA(
LPCSTR lpSystemName,LPCSTR lpName,PLUID lpLuid);
__declspec(dllimport) BOOL __stdcall LookupPrivilegeValueW(
LPCWSTR lpSystemName,LPCWSTR lpName,PLUID lpLuid);
	// 7545 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall LookupPrivilegeNameA(
LPCSTR lpSystemName,PLUID lpLuid,LPSTR lpName,LPDWORD cbName);
__declspec(dllimport) BOOL __stdcall LookupPrivilegeNameW(
LPCWSTR lpSystemName,PLUID lpLuid,LPWSTR lpName,LPDWORD cbName);
	// 7569 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL
__stdcall
LookupPrivilegeDisplayNameA(
LPCSTR lpSystemName,LPCSTR lpName,LPSTR lpDisplayName,LPDWORD cbDisplayName,LPDWORD lpLanguageId);
__declspec(dllimport) BOOL __stdcall LookupPrivilegeDisplayNameW(
LPCWSTR lpSystemName,LPCWSTR lpName,LPWSTR lpDisplayName,LPDWORD cbDisplayName,LPDWORD lpLanguageId);
	// 7595 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall AllocateLocallyUniqueId(
PLUID Luid);
__declspec(dllimport) BOOL __stdcall BuildCommDCBA(
LPCSTR lpDef,LPDCB lpDCB);
__declspec(dllimport) BOOL __stdcall BuildCommDCBW(
LPCWSTR lpDef,LPDCB lpDCB);
	// 7622 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall BuildCommDCBAndTimeoutsA(
LPCSTR lpDef,LPDCB lpDCB,LPCOMMTIMEOUTS lpCommTimeouts);
__declspec(dllimport) BOOL __stdcall BuildCommDCBAndTimeoutsW(
LPCWSTR lpDef,LPDCB lpDCB,LPCOMMTIMEOUTS lpCommTimeouts);
	// 7644 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall CommConfigDialogA(
LPCSTR lpszName,HWND hWnd,LPCOMMCONFIG lpCC);
__declspec(dllimport) BOOL __stdcall CommConfigDialogW(
LPCWSTR lpszName,HWND hWnd,LPCOMMCONFIG lpCC);
	// 7666 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall GetDefaultCommConfigA(
LPCSTR lpszName,LPCOMMCONFIG lpCC,LPDWORD lpdwSize);
__declspec(dllimport) BOOL __stdcall GetDefaultCommConfigW(
LPCWSTR lpszName,LPCOMMCONFIG lpCC,LPDWORD lpdwSize);
	// 7688 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall SetDefaultCommConfigA(
LPCSTR lpszName,LPCOMMCONFIG lpCC,DWORD dwSize);
__declspec(dllimport) BOOL __stdcall SetDefaultCommConfigW(
LPCWSTR lpszName,LPCOMMCONFIG lpCC,DWORD dwSize);
	// 7710 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 7716 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall GetComputerNameA (
LPSTR lpBuffer,LPDWORD nSize);
__declspec(dllimport) BOOL __stdcall GetComputerNameW (
LPWSTR lpBuffer,LPDWORD nSize);
	// 7736 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall SetComputerNameA (
LPCSTR lpComputerName);
__declspec(dllimport) BOOL __stdcall SetComputerNameW (
LPCWSTR lpComputerName);
	// 7754 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall GetUserNameA (
LPSTR lpBuffer,LPDWORD nSize);
__declspec(dllimport) BOOL __stdcall GetUserNameW (
LPWSTR lpBuffer,LPDWORD nSize);
	// 7774 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 7789 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 7792 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall LogonUserA (
LPSTR lpszUsername,LPSTR lpszDomain,LPSTR lpszPassword,DWORD dwLogonType,DWORD dwLogonProvider,PHANDLE phToken);
__declspec(dllimport) BOOL __stdcall LogonUserW (
LPWSTR lpszUsername,LPWSTR lpszDomain,LPWSTR lpszPassword,DWORD dwLogonType,DWORD dwLogonProvider,PHANDLE phToken);
	// 7822 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall ImpersonateLoggedOnUser(
HANDLE hToken);
__declspec(dllimport) BOOL __stdcall CreateProcessAsUserA (
HANDLE hToken,LPCSTR lpApplicationName,LPSTR lpCommandLine,LPSECURITY_ATTRIBUTES lpProcessAttributes,LPSECURITY_ATTRIBUTES lpThreadAttributes,BOOL bInheritHandles,DWORD dwCreationFlags,LPVOID lpEnvironment,LPCSTR lpCurrentDirectory,LPSTARTUPINFOA lpStartupInfo,LPPROCESS_INFORMATION lpProcessInformation);
__declspec(dllimport) BOOL __stdcall CreateProcessAsUserW (
HANDLE hToken,LPCWSTR lpApplicationName,LPWSTR lpCommandLine,LPSECURITY_ATTRIBUTES lpProcessAttributes,LPSECURITY_ATTRIBUTES lpThreadAttributes,BOOL bInheritHandles,DWORD dwCreationFlags,LPVOID lpEnvironment,LPCWSTR lpCurrentDirectory,LPSTARTUPINFOW lpStartupInfo,LPPROCESS_INFORMATION lpProcessInformation);
	// 7867 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall DuplicateTokenEx(
HANDLE hExistingToken,DWORD dwDesiredAccess,LPSECURITY_ATTRIBUTES lpTokenAttributes,SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,TOKEN_TYPE TokenType,PHANDLE phNewToken);
__declspec(dllimport) BOOL __stdcall CreateRestrictedToken(
HANDLE ExistingTokenHandle,DWORD Flags,DWORD DisableSidCount,PSID_AND_ATTRIBUTES SidsToDisable ,DWORD DeletePrivilegeCount,PLUID_AND_ATTRIBUTES PrivilegesToDelete ,DWORD RestrictedSidCount,PSID_AND_ATTRIBUTES SidsToRestrict ,PHANDLE NewTokenHandle);
__declspec(dllimport) BOOL __stdcall IsTokenRestricted(
HANDLE TokenHandle);
	// 7953 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall QueryPerformanceCounter(
LARGE_INTEGER *lpPerformanceCount);
__declspec(dllimport) BOOL __stdcall QueryPerformanceFrequency(
LARGE_INTEGER *lpFrequency);
typedef struct _OSVERSIONINFOA {
DWORD dwOSVersionInfoSize;
DWORD dwMajorVersion;
DWORD dwMinorVersion;
DWORD dwBuildNumber;
DWORD dwPlatformId;
CHAR szCSDVersion[ 128 ];
} OSVERSIONINFOA, *POSVERSIONINFOA, *LPOSVERSIONINFOA;
typedef struct _OSVERSIONINFOW {
DWORD dwOSVersionInfoSize;
DWORD dwMajorVersion;
DWORD dwMinorVersion;
DWORD dwBuildNumber;
DWORD dwPlatformId;
WCHAR szCSDVersion[ 128 ];
} OSVERSIONINFOW, *POSVERSIONINFOW, *LPOSVERSIONINFOW;
typedef OSVERSIONINFOA OSVERSIONINFO;
typedef POSVERSIONINFOA POSVERSIONINFO;
typedef LPOSVERSIONINFOA LPOSVERSIONINFO;
	// 7997 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
typedef struct _OSVERSIONINFOEXA {
DWORD dwOSVersionInfoSize;
DWORD dwMajorVersion;
DWORD dwMinorVersion;
DWORD dwBuildNumber;
DWORD dwPlatformId;
CHAR szCSDVersion[ 128 ];
WORD wServicePackMajor;
WORD wServicePackMinor;
WORD wReserved[2];
} OSVERSIONINFOEXA, *POSVERSIONINFOEXA, *LPOSVERSIONINFOEXA;
typedef struct _OSVERSIONINFOEXW {
DWORD dwOSVersionInfoSize;
DWORD dwMajorVersion;
DWORD dwMinorVersion;
DWORD dwBuildNumber;
DWORD dwPlatformId;
WCHAR szCSDVersion[ 128 ];
WORD wServicePackMajor;
WORD wServicePackMinor;
WORD wReserved[2];
} OSVERSIONINFOEXW, *POSVERSIONINFOEXW, *LPOSVERSIONINFOEXW;
typedef OSVERSIONINFOEXA OSVERSIONINFOEX;
typedef POSVERSIONINFOEXA POSVERSIONINFOEX;
typedef LPOSVERSIONINFOEXA LPOSVERSIONINFOEX;
	// 8029 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
__declspec(dllimport) BOOL __stdcall GetVersionExA(
LPOSVERSIONINFOA lpVersionInformation);
__declspec(dllimport) BOOL __stdcall GetVersionExW(
LPOSVERSIONINFOW lpVersionInformation);
	// 8056 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winerror.h"
	// 8021 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winerror.h"
	// 8208 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winerror.h"
	// 12937 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winerror.h"
	// 8063 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
typedef struct _SYSTEM_POWER_STATUS {
BYTE ACLineStatus;
BYTE BatteryFlag;
BYTE BatteryLifePercent;
BYTE Reserved1;
DWORD BatteryLifeTime;
DWORD BatteryFullLifeTime;
} SYSTEM_POWER_STATUS, *LPSYSTEM_POWER_STATUS;
BOOL __stdcall GetSystemPowerStatus(
LPSYSTEM_POWER_STATUS lpSystemPowerStatus);
BOOL __stdcall SetSystemPowerState(
BOOL fSuspend,BOOL fForce);
	// 8115 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
typedef struct _WIN_CERTIFICATE {
DWORD dwLength;
WORD wRevision;
WORD wCertificateType;
BYTE bCertificate[1];
} WIN_CERTIFICATE, *LPWIN_CERTIFICATE;
BOOL __stdcall WinSubmitCertificate(
LPWIN_CERTIFICATE lpCertificate);
LONG __stdcall WinVerifyTrust(
HWND hwnd,GUID* ActionID,LPVOID ActionData);
BOOL __stdcall WinLoadTrustProvider(
GUID* ActionID);
typedef LPVOID WIN_TRUST_SUBJECT;
typedef struct _WIN_TRUST_ACTDATA_CONTEXT_WITH_SUBJECT {
HANDLE hClientToken;
GUID* SubjectType;
WIN_TRUST_SUBJECT Subject;
} WIN_TRUST_ACTDATA_CONTEXT_WITH_SUBJECT, *LPWIN_TRUST_ACTDATA_CONTEXT_WITH_SUBJECT ;
typedef struct _WIN_TRUST_ACTDATA_SUBJECT_ONLY {
GUID* SubjectType;
WIN_TRUST_SUBJECT Subject;
} WIN_TRUST_ACTDATA_SUBJECT_ONLY, *LPWIN_TRUST_ACTDATA_SUBJECT_ONLY;
typedef struct _WIN_TRUST_SUBJECT_FILE {
HANDLE hFile;
LPCWSTR lpPath;
} WIN_TRUST_SUBJECT_FILE, *LPWIN_TRUST_SUBJECT_FILE;
typedef struct _WIN_TRUST_SUBJECT_FILE_AND_DISPLAY {
HANDLE hFile;
LPCWSTR lpPath;
LPCWSTR lpDisplayName;
} WIN_TRUST_SUBJECT_FILE_AND_DISPLAY, *LPWIN_TRUST_SUBJECT_FILE_AND_DISPLAY;
typedef struct _WIN_SPUB_TRUSTED_PUBLISHER_DATA {
HANDLE hClientToken;
LPWIN_CERTIFICATE lpCertificate;
} WIN_SPUB_TRUSTED_PUBLISHER_DATA, *LPWIN_SPUB_TRUSTED_PUBLISHER_DATA;
	// 8611 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
}
	// 8616 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 8619 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winbase.h"
	// 165 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 25 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 35 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
extern "C" {
	// 39 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 88 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 122 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 145 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 162 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 165 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct _PSINJECTDATA {
DWORD DataBytes;
DWORD InjectionPoint;
DWORD Flags;
DWORD Reserved;
} PSINJECTDATA, *PPSINJECTDATA;
typedef struct tagXFORM
{
FLOAT eM11;
FLOAT eM12;
FLOAT eM21;
FLOAT eM22;
FLOAT eDx;
FLOAT eDy;
} XFORM, *PXFORM, *LPXFORM;
typedef struct tagBITMAP
{
LONG bmType;
LONG bmWidth;
LONG bmHeight;
LONG bmWidthBytes;
WORD bmPlanes;
WORD bmBitsPixel;
LPVOID bmBits;
} BITMAP, *PBITMAP, *NPBITMAP, *LPBITMAP;
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack1.h"
#pragma warning(disable:4103)
#pragma pack(push)
	// 28 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack1.h"
#pragma pack(1)
	// 32 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack1.h"
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack1.h"
	// 489 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct tagRGBTRIPLE {
BYTE rgbtBlue;
BYTE rgbtGreen;
BYTE rgbtRed;
} RGBTRIPLE;
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
#pragma warning(disable:4103)
#pragma pack(pop)
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 36 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 37 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 495 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct tagRGBQUAD {
BYTE rgbBlue;
BYTE rgbGreen;
BYTE rgbRed;
BYTE rgbReserved;
} RGBQUAD;
typedef RGBQUAD* LPRGBQUAD;
typedef LONG LCSCSTYPE;
typedef LONG LCSGAMUTMATCH;
typedef long FXPT16DOT16, *LPFXPT16DOT16;
typedef long FXPT2DOT30, *LPFXPT2DOT30;
typedef struct tagCIEXYZ
{
FXPT2DOT30 ciexyzX;
FXPT2DOT30 ciexyzY;
FXPT2DOT30 ciexyzZ;
} CIEXYZ;
typedef CIEXYZ *LPCIEXYZ;
typedef struct tagICEXYZTRIPLE
{
CIEXYZ ciexyzRed;
CIEXYZ ciexyzGreen;
CIEXYZ ciexyzBlue;
} CIEXYZTRIPLE;
typedef CIEXYZTRIPLE *LPCIEXYZTRIPLE;
typedef struct tagLOGCOLORSPACEA {
DWORD lcsSignature;
DWORD lcsVersion;
DWORD lcsSize;
LCSCSTYPE lcsCSType;
LCSGAMUTMATCH lcsIntent;
CIEXYZTRIPLE lcsEndpoints;
DWORD lcsGammaRed;
DWORD lcsGammaGreen;
DWORD lcsGammaBlue;
CHAR lcsFilename[260];
} LOGCOLORSPACEA, *LPLOGCOLORSPACEA;
typedef struct tagLOGCOLORSPACEW {
DWORD lcsSignature;
DWORD lcsVersion;
DWORD lcsSize;
LCSCSTYPE lcsCSType;
LCSGAMUTMATCH lcsIntent;
CIEXYZTRIPLE lcsEndpoints;
DWORD lcsGammaRed;
DWORD lcsGammaGreen;
DWORD lcsGammaBlue;
WCHAR lcsFilename[260];
} LOGCOLORSPACEW, *LPLOGCOLORSPACEW;
typedef LOGCOLORSPACEA LOGCOLORSPACE;
typedef LPLOGCOLORSPACEA LPLOGCOLORSPACE;
	// 611 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 613 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct tagBITMAPCOREHEADER {
DWORD bcSize;
WORD bcWidth;
WORD bcHeight;
WORD bcPlanes;
WORD bcBitCount;
} BITMAPCOREHEADER, *LPBITMAPCOREHEADER, *PBITMAPCOREHEADER;
typedef struct tagBITMAPINFOHEADER{
DWORD biSize;
LONG biWidth;
LONG biHeight;
WORD biPlanes;
WORD biBitCount;
DWORD biCompression;
DWORD biSizeImage;
LONG biXPelsPerMeter;
LONG biYPelsPerMeter;
DWORD biClrUsed;
DWORD biClrImportant;
} BITMAPINFOHEADER, *LPBITMAPINFOHEADER, *PBITMAPINFOHEADER;
typedef struct {
DWORD bV4Size;
LONG bV4Width;
LONG bV4Height;
WORD bV4Planes;
WORD bV4BitCount;
DWORD bV4V4Compression;
DWORD bV4SizeImage;
LONG bV4XPelsPerMeter;
LONG bV4YPelsPerMeter;
DWORD bV4ClrUsed;
DWORD bV4ClrImportant;
DWORD bV4RedMask;
DWORD bV4GreenMask;
DWORD bV4BlueMask;
DWORD bV4AlphaMask;
DWORD bV4CSType;
CIEXYZTRIPLE bV4Endpoints;
DWORD bV4GammaRed;
DWORD bV4GammaGreen;
DWORD bV4GammaBlue;
} BITMAPV4HEADER, *LPBITMAPV4HEADER, *PBITMAPV4HEADER;
	// 661 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 694 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 702 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct tagBITMAPINFO {
BITMAPINFOHEADER bmiHeader;
RGBQUAD bmiColors[1];
} BITMAPINFO, *LPBITMAPINFO, *PBITMAPINFO;
typedef struct tagBITMAPCOREINFO {
BITMAPCOREHEADER bmciHeader;
RGBTRIPLE bmciColors[1];
} BITMAPCOREINFO, *LPBITMAPCOREINFO, *PBITMAPCOREINFO;
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
#pragma warning(disable:4103)
#pragma pack(push)
	// 28 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
#pragma pack(2)
	// 32 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
	// 714 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct tagBITMAPFILEHEADER {
WORD bfType;
DWORD bfSize;
WORD bfReserved1;
WORD bfReserved2;
DWORD bfOffBits;
} BITMAPFILEHEADER, *LPBITMAPFILEHEADER, *PBITMAPFILEHEADER;
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
#pragma warning(disable:4103)
#pragma pack(pop)
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 36 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 37 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 722 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct tagFONTSIGNATURE
{
DWORD fsUsb[4];
DWORD fsCsb[2];
} FONTSIGNATURE, *PFONTSIGNATURE, *LPFONTSIGNATURE;
typedef struct tagCHARSETINFO
{
UINT ciCharset;
UINT ciACP;
FONTSIGNATURE fs;
} CHARSETINFO, *PCHARSETINFO, *NPCHARSETINFO, *LPCHARSETINFO;
typedef struct tagLOCALESIGNATURE
{
DWORD lsUsb[4];
DWORD lsCsbDefault[2];
DWORD lsCsbSupported[2];
} LOCALESIGNATURE, *PLOCALESIGNATURE, *LPLOCALESIGNATURE;
	// 753 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 754 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct tagPELARRAY
{
LONG paXCount;
LONG paYCount;
LONG paXExt;
LONG paYExt;
BYTE paRGBs;
} PELARRAY, *PPELARRAY, *NPPELARRAY, *LPPELARRAY;
typedef struct tagLOGBRUSH
{
UINT lbStyle;
COLORREF lbColor;
LONG lbHatch;
} LOGBRUSH, *PLOGBRUSH, *NPLOGBRUSH, *LPLOGBRUSH;
typedef LOGBRUSH PATTERN;
typedef PATTERN *PPATTERN;
typedef PATTERN *NPPATTERN;
typedef PATTERN *LPPATTERN;
typedef struct tagLOGPEN
{
UINT lopnStyle;
POINT lopnWidth;
COLORREF lopnColor;
} LOGPEN, *PLOGPEN, *NPLOGPEN, *LPLOGPEN;
typedef struct tagEXTLOGPEN {
DWORD elpPenStyle;
DWORD elpWidth;
UINT elpBrushStyle;
COLORREF elpColor;
LONG elpHatch;
DWORD elpNumEntries;
DWORD elpStyleEntry[1];
} EXTLOGPEN, *PEXTLOGPEN, *NPEXTLOGPEN, *LPEXTLOGPEN;
typedef struct tagPALETTEENTRY {
BYTE peRed;
BYTE peGreen;
BYTE peBlue;
BYTE peFlags;
} PALETTEENTRY, *PPALETTEENTRY, *LPPALETTEENTRY;
typedef struct tagLOGPALETTE {
WORD palVersion;
WORD palNumEntries;
PALETTEENTRY palPalEntry[1];
} LOGPALETTE, *PLOGPALETTE, *NPLOGPALETTE, *LPLOGPALETTE;
typedef struct tagLOGFONTA
{
LONG lfHeight;
LONG lfWidth;
LONG lfEscapement;
LONG lfOrientation;
LONG lfWeight;
BYTE lfItalic;
BYTE lfUnderline;
BYTE lfStrikeOut;
BYTE lfCharSet;
BYTE lfOutPrecision;
BYTE lfClipPrecision;
BYTE lfQuality;
BYTE lfPitchAndFamily;
CHAR lfFaceName[32];
} LOGFONTA, *PLOGFONTA, *NPLOGFONTA, *LPLOGFONTA;
typedef struct tagLOGFONTW
{
LONG lfHeight;
LONG lfWidth;
LONG lfEscapement;
LONG lfOrientation;
LONG lfWeight;
BYTE lfItalic;
BYTE lfUnderline;
BYTE lfStrikeOut;
BYTE lfCharSet;
BYTE lfOutPrecision;
BYTE lfClipPrecision;
BYTE lfQuality;
BYTE lfPitchAndFamily;
WCHAR lfFaceName[32];
} LOGFONTW, *PLOGFONTW, *NPLOGFONTW, *LPLOGFONTW;
typedef LOGFONTA LOGFONT;
typedef PLOGFONTA PLOGFONT;
typedef NPLOGFONTA NPLOGFONT;
typedef LPLOGFONTA LPLOGFONT;
	// 1117 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct tagENUMLOGFONTA
{
LOGFONTA elfLogFont;
BYTE elfFullName[64];
BYTE elfStyle[32];
} ENUMLOGFONTA,* LPENUMLOGFONTA;
typedef struct tagENUMLOGFONTW
{
LOGFONTW elfLogFont;
WCHAR elfFullName[64];
WCHAR elfStyle[32];
} ENUMLOGFONTW,* LPENUMLOGFONTW;
typedef ENUMLOGFONTA ENUMLOGFONT;
typedef LPENUMLOGFONTA LPENUMLOGFONT;
	// 1141 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct tagENUMLOGFONTEXA
{
LOGFONTA elfLogFont;
BYTE elfFullName[64];
BYTE elfStyle[32];
BYTE elfScript[32];
} ENUMLOGFONTEXA, *LPENUMLOGFONTEXA;
typedef struct tagENUMLOGFONTEXW
{
LOGFONTW elfLogFont;
WCHAR elfFullName[64];
WCHAR elfStyle[32];
WCHAR elfScript[32];
} ENUMLOGFONTEXW, *LPENUMLOGFONTEXW;
typedef ENUMLOGFONTEXA ENUMLOGFONTEX;
typedef LPENUMLOGFONTEXA LPENUMLOGFONTEX;
	// 1164 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1165 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1192 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1199 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1240 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct tagPANOSE
{
BYTE bFamilyType;
BYTE bSerifStyle;
BYTE bWeight;
BYTE bProportion;
BYTE bContrast;
BYTE bStrokeVariation;
BYTE bArmStyle;
BYTE bLetterform;
BYTE bMidline;
BYTE bXHeight;
} PANOSE,* LPPANOSE;
typedef struct tagEXTLOGFONTA {
LOGFONTA elfLogFont;
BYTE elfFullName[64];
BYTE elfStyle[32];
DWORD elfVersion;
DWORD elfStyleSize;
DWORD elfMatch;
DWORD elfReserved;
BYTE elfVendorId[4];
DWORD elfCulture;
PANOSE elfPanose;
} EXTLOGFONTA, *PEXTLOGFONTA, *NPEXTLOGFONTA, *LPEXTLOGFONTA;
typedef struct tagEXTLOGFONTW {
LOGFONTW elfLogFont;
WCHAR elfFullName[64];
WCHAR elfStyle[32];
DWORD elfVersion;
DWORD elfStyleSize;
DWORD elfMatch;
DWORD elfReserved;
BYTE elfVendorId[4];
DWORD elfCulture;
PANOSE elfPanose;
} EXTLOGFONTW, *PEXTLOGFONTW, *NPEXTLOGFONTW, *LPEXTLOGFONTW;
typedef EXTLOGFONTA EXTLOGFONT;
typedef PEXTLOGFONTA PEXTLOGFONT;
typedef NPEXTLOGFONTA NPEXTLOGFONT;
typedef LPEXTLOGFONTA LPEXTLOGFONT;
	// 1445 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1526 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1531 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1535 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1539 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1620 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1754 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct _devicemodeA {
BYTE dmDeviceName[32];
WORD dmSpecVersion;
WORD dmDriverVersion;
WORD dmSize;
WORD dmDriverExtra;
DWORD dmFields;
union {
struct {
short dmOrientation;
short dmPaperSize;
short dmPaperLength;
short dmPaperWidth;
};
POINTL dmPosition;
};
short dmScale;
short dmCopies;
short dmDefaultSource;
short dmPrintQuality;
short dmColor;
short dmDuplex;
short dmYResolution;
short dmTTOption;
short dmCollate;
BYTE dmFormName[32];
WORD dmLogPixels;
DWORD dmBitsPerPel;
DWORD dmPelsWidth;
DWORD dmPelsHeight;
DWORD dmDisplayFlags;
DWORD dmDisplayFrequency;
DWORD dmICMMethod;
DWORD dmICMIntent;
DWORD dmMediaType;
DWORD dmDitherType;
DWORD dmReserved1;
DWORD dmReserved2;
	// 1824 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1825 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
} DEVMODEA, *PDEVMODEA, *NPDEVMODEA, *LPDEVMODEA;
typedef struct _devicemodeW {
WCHAR dmDeviceName[32];
WORD dmSpecVersion;
WORD dmDriverVersion;
WORD dmSize;
WORD dmDriverExtra;
DWORD dmFields;
union {
struct {
short dmOrientation;
short dmPaperSize;
short dmPaperLength;
short dmPaperWidth;
};
POINTL dmPosition;
};
short dmScale;
short dmCopies;
short dmDefaultSource;
short dmPrintQuality;
short dmColor;
short dmDuplex;
short dmYResolution;
short dmTTOption;
short dmCollate;
WCHAR dmFormName[32];
WORD dmLogPixels;
DWORD dmBitsPerPel;
DWORD dmPelsWidth;
DWORD dmPelsHeight;
DWORD dmDisplayFlags;
DWORD dmDisplayFrequency;
DWORD dmICMMethod;
DWORD dmICMIntent;
DWORD dmMediaType;
DWORD dmDitherType;
DWORD dmReserved1;
DWORD dmReserved2;
	// 1869 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1870 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
} DEVMODEW, *PDEVMODEW, *NPDEVMODEW, *LPDEVMODEW;
typedef DEVMODEA DEVMODE;
typedef PDEVMODEA PDEVMODE;
typedef NPDEVMODEA NPDEVMODE;
typedef LPDEVMODEA LPDEVMODE;
	// 1882 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1887 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1891 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1901 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 1924 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 2001 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 2054 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 2058 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 2062 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 2107 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 2157 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct _DISPLAY_DEVICEA {
DWORD cb;
BYTE DeviceName[32];
BYTE DeviceString[128];
DWORD StateFlags;
} DISPLAY_DEVICEA, *PDISPLAY_DEVICEA, *LPDISPLAY_DEVICEA;
typedef struct _DISPLAY_DEVICEW {
DWORD cb;
WCHAR DeviceName[32];
WCHAR DeviceString[128];
DWORD StateFlags;
} DISPLAY_DEVICEW, *PDISPLAY_DEVICEW, *LPDISPLAY_DEVICEW;
typedef DISPLAY_DEVICEA DISPLAY_DEVICE;
typedef PDISPLAY_DEVICEA PDISPLAY_DEVICE;
typedef LPDISPLAY_DEVICEA LPDISPLAY_DEVICE;
	// 2179 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct _RGNDATAHEADER {
DWORD dwSize;
DWORD iType;
DWORD nCount;
DWORD nRgnSize;
RECT rcBound;
} RGNDATAHEADER, *PRGNDATAHEADER;
typedef struct _RGNDATA {
RGNDATAHEADER rdh;
char Buffer[1];
} RGNDATA, *PRGNDATA, *NPRGNDATA, *LPRGNDATA;
typedef struct _ABC {
int abcA;
UINT abcB;
int abcC;
} ABC, *PABC, *NPABC, *LPABC;
typedef struct _ABCFLOAT {
FLOAT abcfA;
FLOAT abcfB;
FLOAT abcfC;
} ABCFLOAT, *PABCFLOAT, *NPABCFLOAT, *LPABCFLOAT;
typedef struct tagPOLYTEXTA
{
int x;
int y;
UINT n;
LPCSTR lpstr;
UINT uiFlags;
RECT rcl;
int *pdx;
} POLYTEXTA, *PPOLYTEXTA, *NPPOLYTEXTA, *LPPOLYTEXTA;
typedef struct tagPOLYTEXTW
{
int x;
int y;
UINT n;
LPCWSTR lpstr;
UINT uiFlags;
RECT rcl;
int *pdx;
} POLYTEXTW, *PPOLYTEXTW, *NPPOLYTEXTW, *LPPOLYTEXTW;
typedef POLYTEXTA POLYTEXT;
typedef PPOLYTEXTA PPOLYTEXT;
typedef NPPOLYTEXTA NPPOLYTEXT;
typedef LPPOLYTEXTA LPPOLYTEXT;
	// 2339 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct _FIXED {
WORD fract;
short value;
	// 2348 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
} FIXED;
typedef struct _MAT2 {
FIXED eM11;
FIXED eM12;
FIXED eM21;
FIXED eM22;
} MAT2, *LPMAT2;
typedef struct _GLYPHMETRICS {
UINT gmBlackBoxX;
UINT gmBlackBoxY;
POINT gmptGlyphOrigin;
short gmCellIncX;
short gmCellIncY;
} GLYPHMETRICS, *LPGLYPHMETRICS;
	// 2381 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct tagPOINTFX
{
FIXED x;
FIXED y;
} POINTFX,* LPPOINTFX;
typedef struct tagTTPOLYCURVE
{
WORD wType;
WORD cpfx;
POINTFX apfx[1];
} TTPOLYCURVE,* LPTTPOLYCURVE;
typedef struct tagTTPOLYGONHEADER
{
DWORD cb;
DWORD dwType;
POINTFX pfxStart;
} TTPOLYGONHEADER,* LPTTPOLYGONHEADER;
typedef struct tagGCP_RESULTSA
{
DWORD lStructSize;
LPSTR lpOutString;
UINT *lpOrder;
int *lpDx;
int *lpCaretPos;
LPSTR lpClass;
LPWSTR lpGlyphs;
UINT nGlyphs;
int nMaxFit;
} GCP_RESULTSA,* LPGCP_RESULTSA;
typedef struct tagGCP_RESULTSW
{
DWORD lStructSize;
LPWSTR lpOutString;
UINT *lpOrder;
int *lpDx;
int *lpCaretPos;
LPSTR lpClass;
LPWSTR lpGlyphs;
UINT nGlyphs;
int nMaxFit;
} GCP_RESULTSW,* LPGCP_RESULTSW;
typedef GCP_RESULTSA GCP_RESULTS;
typedef LPGCP_RESULTSA LPGCP_RESULTS;
	// 2483 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 2484 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct _RASTERIZER_STATUS {
short nSize;
short wFlags;
short nLanguageID;
} RASTERIZER_STATUS, *LPRASTERIZER_STATUS;
typedef struct tagPIXELFORMATDESCRIPTOR
{
WORD nSize;
WORD nVersion;
DWORD dwFlags;
BYTE iPixelType;
BYTE cColorBits;
BYTE cRedBits;
BYTE cRedShift;
BYTE cGreenBits;
BYTE cGreenShift;
BYTE cBlueBits;
BYTE cBlueShift;
BYTE cAlphaBits;
BYTE cAlphaShift;
BYTE cAccumBits;
BYTE cAccumRedBits;
BYTE cAccumGreenBits;
BYTE cAccumBlueBits;
BYTE cAccumAlphaBits;
BYTE cDepthBits;
BYTE cStencilBits;
BYTE cAuxBuffers;
BYTE iLayerType;
BYTE bReserved;
DWORD dwLayerMask;
DWORD dwVisibleMask;
DWORD dwDamageMask;
} PIXELFORMATDESCRIPTOR, *PPIXELFORMATDESCRIPTOR, *LPPIXELFORMATDESCRIPTOR;
	// 2566 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef int (__stdcall* OLDFONTENUMPROCA)(const LOGFONTA* ,const void *, DWORD, LPARAM);
typedef int (__stdcall* OLDFONTENUMPROCW)(const LOGFONTW* ,const void *, DWORD, LPARAM);
	// 2573 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 2574 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef OLDFONTENUMPROCA FONTENUMPROCA;
typedef OLDFONTENUMPROCW FONTENUMPROCW;
typedef FONTENUMPROCA FONTENUMPROC;
	// 2582 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef int (__stdcall* GOBJENUMPROC)(LPVOID, LPARAM);
typedef void (__stdcall* LINEDDAPROC)(int, int, LPARAM);
	// 2597 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) int __stdcall AddFontResourceA(LPCSTR);
__declspec(dllimport) int __stdcall AddFontResourceW(LPCWSTR);
	// 2605 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall AnimatePalette(HPALETTE, UINT, UINT, const PALETTEENTRY *);
__declspec(dllimport) BOOL __stdcall Arc(HDC, int, int, int, int, int, int, int, int);
__declspec(dllimport) BOOL __stdcall BitBlt(HDC, int, int, int, int, HDC, int, int, DWORD);
__declspec(dllimport) BOOL __stdcall CancelDC(HDC);
__declspec(dllimport) BOOL __stdcall Chord(HDC, int, int, int, int, int, int, int, int);
__declspec(dllimport) int __stdcall ChoosePixelFormat(HDC, const PIXELFORMATDESCRIPTOR *);
__declspec(dllimport) HMETAFILE __stdcall CloseMetaFile(HDC);
__declspec(dllimport) int __stdcall CombineRgn(HRGN, HRGN, HRGN, int);
__declspec(dllimport) HMETAFILE __stdcall CopyMetaFileA(HMETAFILE, LPCSTR);
__declspec(dllimport) HMETAFILE __stdcall CopyMetaFileW(HMETAFILE, LPCWSTR);
	// 2622 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) HBITMAP __stdcall CreateBitmap(int, int, UINT, UINT, const void *);
__declspec(dllimport) HBITMAP __stdcall CreateBitmapIndirect(const BITMAP *);
__declspec(dllimport) HBRUSH __stdcall CreateBrushIndirect(const LOGBRUSH *);
__declspec(dllimport) HBITMAP __stdcall CreateCompatibleBitmap(HDC, int, int);
__declspec(dllimport) HBITMAP __stdcall CreateDiscardableBitmap(HDC, int, int);
__declspec(dllimport) HDC __stdcall CreateCompatibleDC(HDC);
__declspec(dllimport) HDC __stdcall CreateDCA(LPCSTR, LPCSTR , LPCSTR , const DEVMODEA *);
__declspec(dllimport) HDC __stdcall CreateDCW(LPCWSTR, LPCWSTR , LPCWSTR , const DEVMODEW *);
	// 2635 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) HBITMAP __stdcall CreateDIBitmap(HDC, const BITMAPINFOHEADER *, DWORD, const void *, const BITMAPINFO *, UINT);
__declspec(dllimport) HBRUSH __stdcall CreateDIBPatternBrush(HGLOBAL, UINT);
__declspec(dllimport) HBRUSH __stdcall CreateDIBPatternBrushPt(const void *, UINT);
__declspec(dllimport) HRGN __stdcall CreateEllipticRgn(int, int, int, int);
__declspec(dllimport) HRGN __stdcall CreateEllipticRgnIndirect(const RECT *);
__declspec(dllimport) HFONT __stdcall CreateFontIndirectA(const LOGFONTA *);
__declspec(dllimport) HFONT __stdcall CreateFontIndirectW(const LOGFONTW *);
	// 2647 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) HFONT __stdcall CreateFontA(int, int, int, int, int, DWORD,DWORD, DWORD, DWORD, DWORD, DWORD,DWORD, DWORD, LPCSTR);
__declspec(dllimport) HFONT __stdcall CreateFontW(int, int, int, int, int, DWORD,DWORD, DWORD, DWORD, DWORD, DWORD,DWORD, DWORD, LPCWSTR);
	// 2658 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) HBRUSH __stdcall CreateHatchBrush(int, COLORREF);
__declspec(dllimport) HDC __stdcall CreateICA(LPCSTR, LPCSTR , LPCSTR , const DEVMODEA *);
__declspec(dllimport) HDC __stdcall CreateICW(LPCWSTR, LPCWSTR , LPCWSTR , const DEVMODEW *);
	// 2667 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) HDC __stdcall CreateMetaFileA(LPCSTR);
__declspec(dllimport) HDC __stdcall CreateMetaFileW(LPCWSTR);
	// 2674 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) HPALETTE __stdcall CreatePalette(const LOGPALETTE *);
__declspec(dllimport) HPEN __stdcall CreatePen(int, int, COLORREF);
__declspec(dllimport) HPEN __stdcall CreatePenIndirect(const LOGPEN *);
__declspec(dllimport) HRGN __stdcall CreatePolyPolygonRgn(const POINT *, const INT *, int, int);
__declspec(dllimport) HBRUSH __stdcall CreatePatternBrush(HBITMAP);
__declspec(dllimport) HRGN __stdcall CreateRectRgn(int, int, int, int);
__declspec(dllimport) HRGN __stdcall CreateRectRgnIndirect(const RECT *);
__declspec(dllimport) HRGN __stdcall CreateRoundRectRgn(int, int, int, int, int, int);
__declspec(dllimport) BOOL __stdcall CreateScalableFontResourceA(DWORD, LPCSTR, LPCSTR, LPCSTR);
__declspec(dllimport) BOOL __stdcall CreateScalableFontResourceW(DWORD, LPCWSTR, LPCWSTR, LPCWSTR);
	// 2689 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) HBRUSH __stdcall CreateSolidBrush(COLORREF);
__declspec(dllimport) BOOL __stdcall DeleteDC(HDC);
__declspec(dllimport) BOOL __stdcall DeleteMetaFile(HMETAFILE);
__declspec(dllimport) BOOL __stdcall DeleteObject(HGDIOBJ);
__declspec(dllimport) int __stdcall DescribePixelFormat(HDC, int, UINT, LPPIXELFORMATDESCRIPTOR);
typedef UINT (__stdcall* LPFNDEVMODE)(HWND, HMODULE, LPDEVMODE, LPSTR, LPSTR, LPDEVMODE, LPSTR, UINT);
typedef DWORD (__stdcall* LPFNDEVCAPS)(LPSTR, LPSTR, UINT, LPSTR, LPDEVMODE);
	// 2742 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 2757 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 2775 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) int __stdcall DeviceCapabilitiesA(LPCSTR, LPCSTR, WORD,LPSTR, const DEVMODEA *);
__declspec(dllimport) int __stdcall DeviceCapabilitiesW(LPCWSTR, LPCWSTR, WORD,LPWSTR, const DEVMODEW *);
	// 2785 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) int __stdcall DrawEscape(HDC, int, int, LPCSTR);
__declspec(dllimport) BOOL __stdcall Ellipse(HDC, int, int, int, int);
__declspec(dllimport) int __stdcall EnumFontFamiliesExA(HDC, LPLOGFONTA,FONTENUMPROCA, LPARAM,DWORD);
__declspec(dllimport) int __stdcall EnumFontFamiliesExW(HDC, LPLOGFONTW,FONTENUMPROCW, LPARAM,DWORD);
	// 2797 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 2798 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) int __stdcall EnumFontFamiliesA(HDC, LPCSTR, FONTENUMPROCA, LPARAM);
__declspec(dllimport) int __stdcall EnumFontFamiliesW(HDC, LPCWSTR, FONTENUMPROCW, LPARAM);
	// 2806 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) int __stdcall EnumFontsA(HDC, LPCSTR, FONTENUMPROCA, LPARAM);
__declspec(dllimport) int __stdcall EnumFontsW(HDC, LPCWSTR, FONTENUMPROCW, LPARAM);
	// 2813 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) int __stdcall EnumObjects(HDC, int, GOBJENUMPROC, LPARAM);
	// 2819 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall EqualRgn(HRGN, HRGN);
__declspec(dllimport) int __stdcall Escape(HDC, int, int, LPCSTR, LPVOID);
__declspec(dllimport) int __stdcall ExtEscape(HDC, int, int, LPCSTR, int, LPSTR);
__declspec(dllimport) int __stdcall ExcludeClipRect(HDC, int, int, int, int);
__declspec(dllimport) HRGN __stdcall ExtCreateRegion(const XFORM *, DWORD, const RGNDATA *);
__declspec(dllimport) BOOL __stdcall ExtFloodFill(HDC, int, int, COLORREF, UINT);
__declspec(dllimport) BOOL __stdcall FillRgn(HDC, HRGN, HBRUSH);
__declspec(dllimport) BOOL __stdcall FloodFill(HDC, int, int, COLORREF);
__declspec(dllimport) BOOL __stdcall FrameRgn(HDC, HRGN, HBRUSH, int, int);
__declspec(dllimport) int __stdcall GetROP2(HDC);
__declspec(dllimport) BOOL __stdcall GetAspectRatioFilterEx(HDC, LPSIZE);
__declspec(dllimport) COLORREF __stdcall GetBkColor(HDC);
	// 2838 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) int __stdcall GetBkMode(HDC);
__declspec(dllimport) LONG __stdcall GetBitmapBits(HBITMAP, LONG, LPVOID);
__declspec(dllimport) BOOL __stdcall GetBitmapDimensionEx(HBITMAP, LPSIZE);
__declspec(dllimport) UINT __stdcall GetBoundsRect(HDC, LPRECT, UINT);
__declspec(dllimport) BOOL __stdcall GetBrushOrgEx(HDC, LPPOINT);
__declspec(dllimport) BOOL __stdcall GetCharWidthA(HDC, UINT, UINT, LPINT);
__declspec(dllimport) BOOL __stdcall GetCharWidthW(HDC, UINT, UINT, LPINT);
	// 2853 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall GetCharWidth32A(HDC, UINT, UINT, LPINT);
__declspec(dllimport) BOOL __stdcall GetCharWidth32W(HDC, UINT, UINT, LPINT);
	// 2860 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall GetCharWidthFloatA(HDC, UINT, UINT, PFLOAT);
__declspec(dllimport) BOOL __stdcall GetCharWidthFloatW(HDC, UINT, UINT, PFLOAT);
	// 2867 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall GetCharABCWidthsA(HDC, UINT, UINT, LPABC);
__declspec(dllimport) BOOL __stdcall GetCharABCWidthsW(HDC, UINT, UINT, LPABC);
	// 2874 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall GetCharABCWidthsFloatA(HDC, UINT, UINT, LPABCFLOAT);
__declspec(dllimport) BOOL __stdcall GetCharABCWidthsFloatW(HDC, UINT, UINT, LPABCFLOAT);
	// 2881 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) int __stdcall GetClipBox(HDC, LPRECT);
__declspec(dllimport) int __stdcall GetClipRgn(HDC, HRGN);
__declspec(dllimport) int __stdcall GetMetaRgn(HDC, HRGN);
__declspec(dllimport) HGDIOBJ __stdcall GetCurrentObject(HDC, UINT);
__declspec(dllimport) BOOL __stdcall GetCurrentPositionEx(HDC, LPPOINT);
__declspec(dllimport) int __stdcall GetDeviceCaps(HDC, int);
__declspec(dllimport) int __stdcall GetDIBits(HDC, HBITMAP, UINT, UINT, LPVOID, LPBITMAPINFO, UINT);
__declspec(dllimport) DWORD __stdcall GetFontData(HDC, DWORD, DWORD, LPVOID, DWORD);
__declspec(dllimport) DWORD __stdcall GetGlyphOutlineA(HDC, UINT, UINT, LPGLYPHMETRICS, DWORD, LPVOID, const MAT2 *);
__declspec(dllimport) DWORD __stdcall GetGlyphOutlineW(HDC, UINT, UINT, LPGLYPHMETRICS, DWORD, LPVOID, const MAT2 *);
	// 2896 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) int __stdcall GetGraphicsMode(HDC);
__declspec(dllimport) int __stdcall GetMapMode(HDC);
__declspec(dllimport) UINT __stdcall GetMetaFileBitsEx(HMETAFILE, UINT, LPVOID);
__declspec(dllimport) HMETAFILE __stdcall GetMetaFileA(LPCSTR);
__declspec(dllimport) HMETAFILE __stdcall GetMetaFileW(LPCWSTR);
	// 2906 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) COLORREF __stdcall GetNearestColor(HDC, COLORREF);
__declspec(dllimport) UINT __stdcall GetNearestPaletteIndex(HPALETTE, COLORREF);
__declspec(dllimport) DWORD __stdcall GetObjectType(HGDIOBJ h);
__declspec(dllimport) UINT __stdcall GetPaletteEntries(HPALETTE, UINT, UINT, LPPALETTEENTRY);
__declspec(dllimport) COLORREF __stdcall GetPixel(HDC, int, int);
__declspec(dllimport) int __stdcall GetPixelFormat(HDC);
__declspec(dllimport) int __stdcall GetPolyFillMode(HDC);
__declspec(dllimport) BOOL __stdcall GetRasterizerCaps(LPRASTERIZER_STATUS, UINT);
__declspec(dllimport) DWORD __stdcall GetRegionData(HRGN, DWORD, LPRGNDATA);
__declspec(dllimport) int __stdcall GetRgnBox(HRGN, LPRECT);
__declspec(dllimport) HGDIOBJ __stdcall GetStockObject(int);
__declspec(dllimport) int __stdcall GetStretchBltMode(HDC);
__declspec(dllimport) UINT __stdcall GetSystemPaletteEntries(HDC, UINT, UINT, LPPALETTEENTRY);
__declspec(dllimport) UINT __stdcall GetSystemPaletteUse(HDC);
__declspec(dllimport) int __stdcall GetTextCharacterExtra(HDC);
__declspec(dllimport) UINT __stdcall GetTextAlign(HDC);
__declspec(dllimport) COLORREF __stdcall GetTextColor(HDC);
__declspec(dllimport) BOOL __stdcall GetTextExtentPointA(
HDC,LPCSTR,int,LPSIZE);
__declspec(dllimport) BOOL __stdcall GetTextExtentPointW(
HDC,LPCWSTR,int,LPSIZE);
	// 2954 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall GetTextExtentPoint32A(
HDC,LPCSTR,int,LPSIZE);
__declspec(dllimport) BOOL __stdcall GetTextExtentPoint32W(
HDC,LPCWSTR,int,LPSIZE);
	// 2972 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall GetTextExtentExPointA(
HDC,LPCSTR,int,int,LPINT,LPINT,LPSIZE);
__declspec(dllimport) BOOL __stdcall GetTextExtentExPointW(
HDC,LPCWSTR,int,int,LPINT,LPINT,LPSIZE);
	// 2996 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) int __stdcall GetTextCharset(HDC hdc);
__declspec(dllimport) int __stdcall GetTextCharsetInfo(HDC hdc, LPFONTSIGNATURE lpSig, DWORD dwFlags);
__declspec(dllimport) BOOL __stdcall TranslateCharsetInfo( DWORD *lpSrc, LPCHARSETINFO lpCs, DWORD dwFlags);
__declspec(dllimport) DWORD __stdcall GetFontLanguageInfo( HDC );
__declspec(dllimport) DWORD __stdcall GetCharacterPlacementA(HDC, LPCSTR, int, int, LPGCP_RESULTSA, DWORD);
__declspec(dllimport) DWORD __stdcall GetCharacterPlacementW(HDC, LPCWSTR, int, int, LPGCP_RESULTSW, DWORD);
	// 3008 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 3009 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 3188 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall GetViewportExtEx(HDC, LPSIZE);
__declspec(dllimport) BOOL __stdcall GetViewportOrgEx(HDC, LPPOINT);
__declspec(dllimport) BOOL __stdcall GetWindowExtEx(HDC, LPSIZE);
__declspec(dllimport) BOOL __stdcall GetWindowOrgEx(HDC, LPPOINT);
__declspec(dllimport) int __stdcall IntersectClipRect(HDC, int, int, int, int);
__declspec(dllimport) BOOL __stdcall InvertRgn(HDC, HRGN);
__declspec(dllimport) BOOL __stdcall LineDDA(int, int, int, int, LINEDDAPROC, LPARAM);
__declspec(dllimport) BOOL __stdcall LineTo(HDC, int, int);
__declspec(dllimport) BOOL __stdcall MaskBlt(HDC, int, int, int, int,HDC, int, int, HBITMAP, int, int, DWORD);
__declspec(dllimport) BOOL __stdcall PlgBlt(HDC, const POINT *, HDC, int, int, int,int, HBITMAP, int, int);
__declspec(dllimport) int __stdcall OffsetClipRgn(HDC, int, int);
__declspec(dllimport) int __stdcall OffsetRgn(HRGN, int, int);
__declspec(dllimport) BOOL __stdcall PatBlt(HDC, int, int, int, int, DWORD);
__declspec(dllimport) BOOL __stdcall Pie(HDC, int, int, int, int, int, int, int, int);
__declspec(dllimport) BOOL __stdcall PlayMetaFile(HDC, HMETAFILE);
__declspec(dllimport) BOOL __stdcall PaintRgn(HDC, HRGN);
__declspec(dllimport) BOOL __stdcall PolyPolygon(HDC, const POINT *, const INT *, int);
__declspec(dllimport) BOOL __stdcall PtInRegion(HRGN, int, int);
__declspec(dllimport) BOOL __stdcall PtVisible(HDC, int, int);
__declspec(dllimport) BOOL __stdcall RectInRegion(HRGN, const RECT *);
__declspec(dllimport) BOOL __stdcall RectVisible(HDC, const RECT *);
__declspec(dllimport) BOOL __stdcall Rectangle(HDC, int, int, int, int);
__declspec(dllimport) BOOL __stdcall RestoreDC(HDC, int);
__declspec(dllimport) HDC __stdcall ResetDCA(HDC, const DEVMODEA *);
__declspec(dllimport) HDC __stdcall ResetDCW(HDC, const DEVMODEW *);
	// 3224 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) UINT __stdcall RealizePalette(HDC);
__declspec(dllimport) BOOL __stdcall RemoveFontResourceA(LPCSTR);
__declspec(dllimport) BOOL __stdcall RemoveFontResourceW(LPCWSTR);
	// 3232 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall RoundRect(HDC, int, int, int, int, int, int);
__declspec(dllimport) BOOL __stdcall ResizePalette(HPALETTE, UINT);
__declspec(dllimport) int __stdcall SaveDC(HDC);
__declspec(dllimport) int __stdcall SelectClipRgn(HDC, HRGN);
__declspec(dllimport) int __stdcall ExtSelectClipRgn(HDC, HRGN, int);
__declspec(dllimport) int __stdcall SetMetaRgn(HDC);
__declspec(dllimport) HGDIOBJ __stdcall SelectObject(HDC, HGDIOBJ);
__declspec(dllimport) HPALETTE __stdcall SelectPalette(HDC, HPALETTE, BOOL);
__declspec(dllimport) COLORREF __stdcall SetBkColor(HDC, COLORREF);
	// 3247 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) int __stdcall SetBkMode(HDC, int);
__declspec(dllimport) LONG __stdcall SetBitmapBits(HBITMAP, DWORD, const void *);
__declspec(dllimport) UINT __stdcall SetBoundsRect(HDC, const RECT *, UINT);
__declspec(dllimport) int __stdcall SetDIBits(HDC, HBITMAP, UINT, UINT, const void *, const BITMAPINFO *, UINT);
__declspec(dllimport) int __stdcall SetDIBitsToDevice(HDC, int, int, DWORD, DWORD, int,int, UINT, UINT, const void *, const BITMAPINFO *, UINT);
__declspec(dllimport) DWORD __stdcall SetMapperFlags(HDC, DWORD);
__declspec(dllimport) int __stdcall SetGraphicsMode(HDC hdc, int iMode);
__declspec(dllimport) int __stdcall SetMapMode(HDC, int);
__declspec(dllimport) HMETAFILE __stdcall SetMetaFileBitsEx(UINT, const BYTE *);
__declspec(dllimport) UINT __stdcall SetPaletteEntries(HPALETTE, UINT, UINT, const PALETTEENTRY *);
__declspec(dllimport) COLORREF __stdcall SetPixel(HDC, int, int, COLORREF);
__declspec(dllimport) BOOL __stdcall SetPixelV(HDC, int, int, COLORREF);
__declspec(dllimport) BOOL __stdcall SetPixelFormat(HDC, int, const PIXELFORMATDESCRIPTOR *);
__declspec(dllimport) int __stdcall SetPolyFillMode(HDC, int);
__declspec(dllimport) BOOL __stdcall StretchBlt(HDC, int, int, int, int, HDC, int, int, int, int, DWORD);
__declspec(dllimport) BOOL __stdcall SetRectRgn(HRGN, int, int, int, int);
__declspec(dllimport) int __stdcall StretchDIBits(HDC, int, int, int, int, int, int, int, int, const
void *, const BITMAPINFO *, UINT, DWORD);
__declspec(dllimport) int __stdcall SetROP2(HDC, int);
__declspec(dllimport) int __stdcall SetStretchBltMode(HDC, int);
__declspec(dllimport) UINT __stdcall SetSystemPaletteUse(HDC, UINT);
__declspec(dllimport) int __stdcall SetTextCharacterExtra(HDC, int);
__declspec(dllimport) COLORREF __stdcall SetTextColor(HDC, COLORREF);
__declspec(dllimport) UINT __stdcall SetTextAlign(HDC, UINT);
__declspec(dllimport) BOOL __stdcall SetTextJustification(HDC, int, int);
__declspec(dllimport) BOOL __stdcall UpdateColors(HDC);
typedef USHORT COLOR16;
typedef struct _TRIVERTEX
{
LONG x;
LONG y;
COLOR16 Red;
COLOR16 Green;
COLOR16 Blue;
COLOR16 Alpha;
}TRIVERTEX,*PTRIVERTEX,*LPTRIVERTEX;
typedef struct _GRADIENT_TRIANGLE
{
ULONG Vertex1;
ULONG Vertex2;
ULONG Vertex3;
} GRADIENT_TRIANGLE,*PGRADIENT_TRIANGLE,*LPGRADIENT_TRIANGLE;
typedef struct _GRADIENT_RECT
{
ULONG UpperLeft;
ULONG LowerRight;
}GRADIENT_RECT,*PGRADIENT_RECT,*LPGRADIENT_RECT;
typedef struct _BLENDFUNCTION
{
BYTE BlendOp;
BYTE BlendFlags;
BYTE SourceConstantAlpha;
BYTE AlphaFormat;
}BLENDFUNCTION,*PBLENDFUNCTION;
__declspec(dllimport) BOOL __stdcall AlphaBlend(HDC,int,int,int,int,HDC,int,int,int,int,BLENDFUNCTION);
__declspec(dllimport) BOOL __stdcall AlphaDIBBlend(HDC,int,int,int,int,const void *,const BITMAPINFO *,UINT,int,int,int,int,BLENDFUNCTION);
__declspec(dllimport) BOOL __stdcall TransparentBlt(HDC,int,int,int,int,HDC,int,int,int,int,UINT);
__declspec(dllimport) BOOL __stdcall TransparentDIBits(HDC,int,int,int,int,const void *,const BITMAPINFO *,UINT,int,int,int,int,UINT);
__declspec(dllimport) BOOL __stdcall GradientFill(HDC,PTRIVERTEX,ULONG,PVOID,ULONG,ULONG);
	// 3355 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct tagDIBSECTION {
BITMAP dsBm;
BITMAPINFOHEADER dsBmih;
DWORD dsBitfields[3];
HANDLE dshSection;
DWORD dsOffset;
} DIBSECTION, *LPDIBSECTION, *PDIBSECTION;
__declspec(dllimport) BOOL __stdcall AngleArc(HDC, int, int, DWORD, FLOAT, FLOAT);
__declspec(dllimport) BOOL __stdcall PolyPolyline(HDC, const POINT *, const DWORD *, DWORD);
__declspec(dllimport) BOOL __stdcall GetWorldTransform(HDC, LPXFORM);
__declspec(dllimport) BOOL __stdcall SetWorldTransform(HDC, const XFORM *);
__declspec(dllimport) BOOL __stdcall ModifyWorldTransform(HDC, const XFORM *, DWORD);
__declspec(dllimport) BOOL __stdcall CombineTransform(LPXFORM, const XFORM *, const XFORM *);
__declspec(dllimport) HBITMAP __stdcall CreateDIBSection(HDC, const BITMAPINFO *, UINT, void **, HANDLE, DWORD);
__declspec(dllimport) UINT __stdcall GetDIBColorTable(HDC, UINT, UINT, RGBQUAD *);
__declspec(dllimport) UINT __stdcall SetDIBColorTable(HDC, UINT, UINT, const RGBQUAD *);
typedef struct tagCOLORADJUSTMENT {
WORD caSize;
WORD caFlags;
WORD caIlluminantIndex;
WORD caRedGamma;
WORD caGreenGamma;
WORD caBlueGamma;
WORD caReferenceBlack;
WORD caReferenceWhite;
SHORT caContrast;
SHORT caBrightness;
SHORT caColorfulness;
SHORT caRedGreenTint;
} COLORADJUSTMENT, *PCOLORADJUSTMENT, *LPCOLORADJUSTMENT;
__declspec(dllimport) BOOL __stdcall SetColorAdjustment(HDC, const COLORADJUSTMENT *);
__declspec(dllimport) BOOL __stdcall GetColorAdjustment(HDC, LPCOLORADJUSTMENT);
__declspec(dllimport) HPALETTE __stdcall CreateHalftonePalette(HDC);
typedef BOOL (__stdcall* ABORTPROC)(HDC, int);
	// 3504 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct _DOCINFOA {
int cbSize;
LPCSTR lpszDocName;
LPCSTR lpszOutput;
LPCSTR lpszDatatype;
DWORD fwType;
	// 3513 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
} DOCINFOA, *LPDOCINFOA;
typedef struct _DOCINFOW {
int cbSize;
LPCWSTR lpszDocName;
LPCWSTR lpszOutput;
LPCWSTR lpszDatatype;
DWORD fwType;
	// 3522 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
} DOCINFOW, *LPDOCINFOW;
typedef DOCINFOA DOCINFO;
typedef LPDOCINFOA LPDOCINFO;
	// 3530 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 3535 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) int __stdcall StartDocA(HDC, const DOCINFOA *);
__declspec(dllimport) int __stdcall StartDocW(HDC, const DOCINFOW *);
	// 3543 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) int __stdcall EndDoc(HDC);
__declspec(dllimport) int __stdcall StartPage(HDC);
__declspec(dllimport) int __stdcall EndPage(HDC);
__declspec(dllimport) int __stdcall AbortDoc(HDC);
__declspec(dllimport) int __stdcall SetAbortProc(HDC, ABORTPROC);
__declspec(dllimport) BOOL __stdcall AbortPath(HDC);
__declspec(dllimport) BOOL __stdcall ArcTo(HDC, int, int, int, int, int, int,int, int);
__declspec(dllimport) BOOL __stdcall BeginPath(HDC);
__declspec(dllimport) BOOL __stdcall CloseFigure(HDC);
__declspec(dllimport) BOOL __stdcall EndPath(HDC);
__declspec(dllimport) BOOL __stdcall FillPath(HDC);
__declspec(dllimport) BOOL __stdcall FlattenPath(HDC);
__declspec(dllimport) int __stdcall GetPath(HDC, LPPOINT, LPBYTE, int);
__declspec(dllimport) HRGN __stdcall PathToRegion(HDC);
__declspec(dllimport) BOOL __stdcall PolyDraw(HDC, const POINT *, const BYTE *, int);
__declspec(dllimport) BOOL __stdcall SelectClipPath(HDC, int);
__declspec(dllimport) int __stdcall SetArcDirection(HDC, int);
__declspec(dllimport) BOOL __stdcall SetMiterLimit(HDC, FLOAT, PFLOAT);
__declspec(dllimport) BOOL __stdcall StrokeAndFillPath(HDC);
__declspec(dllimport) BOOL __stdcall StrokePath(HDC);
__declspec(dllimport) BOOL __stdcall WidenPath(HDC);
__declspec(dllimport) HPEN __stdcall ExtCreatePen(DWORD, DWORD, const LOGBRUSH *, DWORD, const DWORD *);
__declspec(dllimport) BOOL __stdcall GetMiterLimit(HDC, PFLOAT);
__declspec(dllimport) int __stdcall GetArcDirection(HDC);
__declspec(dllimport) int __stdcall GetObjectA(HGDIOBJ, int, LPVOID);
__declspec(dllimport) int __stdcall GetObjectW(HGDIOBJ, int, LPVOID);
	// 3576 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall MoveToEx(HDC, int, int, LPPOINT);
__declspec(dllimport) BOOL __stdcall TextOutA(HDC, int, int, LPCSTR, int);
__declspec(dllimport) BOOL __stdcall TextOutW(HDC, int, int, LPCWSTR, int);
	// 3584 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall ExtTextOutA(HDC, int, int, UINT, const RECT *,LPCSTR, UINT, const INT *);
__declspec(dllimport) BOOL __stdcall ExtTextOutW(HDC, int, int, UINT, const RECT *,LPCWSTR, UINT, const INT *);
	// 3591 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall PolyTextOutA(HDC, const POLYTEXTA *, int);
__declspec(dllimport) BOOL __stdcall PolyTextOutW(HDC, const POLYTEXTW *, int);
	// 3598 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) HRGN __stdcall CreatePolygonRgn(const POINT *, int, int);
__declspec(dllimport) BOOL __stdcall DPtoLP(HDC, LPPOINT, int);
__declspec(dllimport) BOOL __stdcall LPtoDP(HDC, LPPOINT, int);
__declspec(dllimport) BOOL __stdcall Polygon(HDC, const POINT *, int);
__declspec(dllimport) BOOL __stdcall Polyline(HDC, const POINT *, int);
__declspec(dllimport) BOOL __stdcall PolyBezier(HDC, const POINT *, DWORD);
__declspec(dllimport) BOOL __stdcall PolyBezierTo(HDC, const POINT *, DWORD);
__declspec(dllimport) BOOL __stdcall PolylineTo(HDC, const POINT *, DWORD);
__declspec(dllimport) BOOL __stdcall SetViewportExtEx(HDC, int, int, LPSIZE);
__declspec(dllimport) BOOL __stdcall SetViewportOrgEx(HDC, int, int, LPPOINT);
__declspec(dllimport) BOOL __stdcall SetWindowExtEx(HDC, int, int, LPSIZE);
__declspec(dllimport) BOOL __stdcall SetWindowOrgEx(HDC, int, int, LPPOINT);
__declspec(dllimport) BOOL __stdcall OffsetViewportOrgEx(HDC, int, int, LPPOINT);
__declspec(dllimport) BOOL __stdcall OffsetWindowOrgEx(HDC, int, int, LPPOINT);
__declspec(dllimport) BOOL __stdcall ScaleViewportExtEx(HDC, int, int, int, int, LPSIZE);
__declspec(dllimport) BOOL __stdcall ScaleWindowExtEx(HDC, int, int, int, int, LPSIZE);
__declspec(dllimport) BOOL __stdcall SetBitmapDimensionEx(HBITMAP, int, int, LPSIZE);
__declspec(dllimport) BOOL __stdcall SetBrushOrgEx(HDC, int, int, LPPOINT);
__declspec(dllimport) int __stdcall GetTextFaceA(HDC, int, LPSTR);
__declspec(dllimport) int __stdcall GetTextFaceW(HDC, int, LPWSTR);
	// 3628 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct tagKERNINGPAIR {
WORD wFirst;
WORD wSecond;
int iKernAmount;
} KERNINGPAIR, *LPKERNINGPAIR;
__declspec(dllimport) DWORD __stdcall GetKerningPairsA(HDC, DWORD, LPKERNINGPAIR);
__declspec(dllimport) DWORD __stdcall GetKerningPairsW(HDC, DWORD, LPKERNINGPAIR);
	// 3644 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall GetDCOrgEx(HDC,LPPOINT);
__declspec(dllimport) BOOL __stdcall FixBrushOrgEx(HDC,int,int,LPPOINT);
__declspec(dllimport) BOOL __stdcall UnrealizeObject(HGDIOBJ);
__declspec(dllimport) BOOL __stdcall GdiFlush();
__declspec(dllimport) DWORD __stdcall GdiSetBatchLimit(DWORD);
__declspec(dllimport) DWORD __stdcall GdiGetBatchLimit();
typedef int (__stdcall* ICMENUMPROCA)(LPSTR, LPARAM);
typedef int (__stdcall* ICMENUMPROCW)(LPWSTR, LPARAM);
	// 3668 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) int __stdcall SetICMMode(HDC, int);
__declspec(dllimport) BOOL __stdcall CheckColorsInGamut(HDC,LPVOID,LPVOID,DWORD);
__declspec(dllimport) HCOLORSPACE __stdcall GetColorSpace(HDC);
__declspec(dllimport) BOOL __stdcall GetLogColorSpaceA(HCOLORSPACE,LPLOGCOLORSPACEA,DWORD);
__declspec(dllimport) BOOL __stdcall GetLogColorSpaceW(HCOLORSPACE,LPLOGCOLORSPACEW,DWORD);
	// 3679 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) HCOLORSPACE __stdcall CreateColorSpaceA(LPLOGCOLORSPACEA);
__declspec(dllimport) HCOLORSPACE __stdcall CreateColorSpaceW(LPLOGCOLORSPACEW);
	// 3686 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) HCOLORSPACE __stdcall SetColorSpace(HDC,HCOLORSPACE);
__declspec(dllimport) BOOL __stdcall DeleteColorSpace(HCOLORSPACE);
__declspec(dllimport) BOOL __stdcall GetICMProfileA(HDC,LPDWORD,LPSTR);
__declspec(dllimport) BOOL __stdcall GetICMProfileW(HDC,LPDWORD,LPWSTR);
	// 3695 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall SetICMProfileA(HDC,LPSTR);
__declspec(dllimport) BOOL __stdcall SetICMProfileW(HDC,LPWSTR);
	// 3702 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall GetDeviceGammaRamp(HDC,LPVOID);
__declspec(dllimport) BOOL __stdcall SetDeviceGammaRamp(HDC,LPVOID);
__declspec(dllimport) BOOL __stdcall ColorMatchToTarget(HDC,HDC,DWORD);
__declspec(dllimport) int __stdcall EnumICMProfilesA(HDC,ICMENUMPROCA,LPARAM);
__declspec(dllimport) int __stdcall EnumICMProfilesW(HDC,ICMENUMPROCW,LPARAM);
	// 3712 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall UpdateICMRegKeyA(DWORD,LPSTR,LPSTR,UINT);
__declspec(dllimport) BOOL __stdcall UpdateICMRegKeyW(DWORD,LPWSTR,LPWSTR,UINT);
	// 3719 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 3720 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 3724 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall wglCopyContext(HGLRC, HGLRC, UINT);
__declspec(dllimport) HGLRC __stdcall wglCreateContext(HDC);
__declspec(dllimport) HGLRC __stdcall wglCreateLayerContext(HDC, int);
__declspec(dllimport) BOOL __stdcall wglDeleteContext(HGLRC);
__declspec(dllimport) HGLRC __stdcall wglGetCurrentContext(void);
__declspec(dllimport) HDC __stdcall wglGetCurrentDC(void);
__declspec(dllimport) PROC __stdcall wglGetProcAddress(LPCSTR);
__declspec(dllimport) BOOL __stdcall wglMakeCurrent(HDC, HGLRC);
__declspec(dllimport) BOOL __stdcall wglShareLists(HGLRC, HGLRC);
__declspec(dllimport) BOOL __stdcall wglUseFontBitmapsA(HDC, DWORD, DWORD, DWORD);
__declspec(dllimport) BOOL __stdcall wglUseFontBitmapsW(HDC, DWORD, DWORD, DWORD);
	// 4613 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
__declspec(dllimport) BOOL __stdcall SwapBuffers(HDC);
typedef struct _POINTFLOAT {
FLOAT x;
FLOAT y;
} POINTFLOAT, *PPOINTFLOAT;
typedef struct _GLYPHMETRICSFLOAT {
FLOAT gmfBlackBoxX;
FLOAT gmfBlackBoxY;
POINTFLOAT gmfptGlyphOrigin;
FLOAT gmfCellIncX;
FLOAT gmfCellIncY;
} GLYPHMETRICSFLOAT, *PGLYPHMETRICSFLOAT, *LPGLYPHMETRICSFLOAT;
__declspec(dllimport) BOOL __stdcall wglUseFontOutlinesA(HDC, DWORD, DWORD, DWORD, FLOAT,FLOAT, int, LPGLYPHMETRICSFLOAT);
__declspec(dllimport) BOOL __stdcall wglUseFontOutlinesW(HDC, DWORD, DWORD, DWORD, FLOAT,FLOAT, int, LPGLYPHMETRICSFLOAT);
	// 4639 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
typedef struct tagLAYERPLANEDESCRIPTOR {
WORD nSize;
WORD nVersion;
DWORD dwFlags;
BYTE iPixelType;
BYTE cColorBits;
BYTE cRedBits;
BYTE cRedShift;
BYTE cGreenBits;
BYTE cGreenShift;
BYTE cBlueBits;
BYTE cBlueShift;
BYTE cAlphaBits;
BYTE cAlphaShift;
BYTE cAccumBits;
BYTE cAccumRedBits;
BYTE cAccumGreenBits;
BYTE cAccumBlueBits;
BYTE cAccumAlphaBits;
BYTE cDepthBits;
BYTE cStencilBits;
BYTE cAuxBuffers;
BYTE iLayerPlane;
BYTE bReserved;
COLORREF crTransparent;
} LAYERPLANEDESCRIPTOR, *PLAYERPLANEDESCRIPTOR, *LPLAYERPLANEDESCRIPTOR;
__declspec(dllimport) BOOL __stdcall wglDescribeLayerPlane(HDC, int, int, UINT,LPLAYERPLANEDESCRIPTOR);
__declspec(dllimport) int __stdcall wglSetLayerPaletteEntries(HDC, int, int, int,const COLORREF *);
__declspec(dllimport) int __stdcall wglGetLayerPaletteEntries(HDC, int, int, int,COLORREF *);
__declspec(dllimport) BOOL __stdcall wglRealizeLayerPalette(HDC, int, BOOL);
__declspec(dllimport) BOOL __stdcall wglSwapLayerBuffers(HDC, UINT);
	// 4738 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 4740 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
}
	// 4744 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 4747 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wingdi.h"
	// 166 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 22 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
extern "C" {
	// 30 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef HANDLE HDWP;
typedef void MENUTEMPLATEA;
typedef void MENUTEMPLATEW;
typedef MENUTEMPLATEA MENUTEMPLATE;
	// 47 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef PVOID LPMENUTEMPLATEA;
typedef PVOID LPMENUTEMPLATEW;
typedef LPMENUTEMPLATEA LPMENUTEMPLATE;
	// 54 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef LRESULT (__stdcall* WNDPROC)(HWND, UINT, WPARAM, LPARAM);
typedef BOOL (__stdcall* DLGPROC)(HWND, UINT, WPARAM, LPARAM);
typedef void (__stdcall* TIMERPROC)(HWND, UINT, UINT, DWORD);
typedef BOOL (__stdcall* GRAYSTRINGPROC)(HDC, LPARAM, int);
typedef BOOL (__stdcall* WNDENUMPROC)(HWND, LPARAM);
typedef LRESULT (__stdcall* HOOKPROC)(int code, WPARAM wParam, LPARAM lParam);
typedef void (__stdcall* SENDASYNCPROC)(HWND, UINT, DWORD, LRESULT);
typedef BOOL (__stdcall* PROPENUMPROCA)(HWND, LPCSTR, HANDLE);
typedef BOOL (__stdcall* PROPENUMPROCW)(HWND, LPCWSTR, HANDLE);
typedef BOOL (__stdcall* PROPENUMPROCEXA)(HWND, LPSTR, HANDLE, DWORD);
typedef BOOL (__stdcall* PROPENUMPROCEXW)(HWND, LPWSTR, HANDLE, DWORD);
typedef int (__stdcall* EDITWORDBREAKPROCA)(LPSTR lpch, int ichCurrent, int cch, int code);
typedef int (__stdcall* EDITWORDBREAKPROCW)(LPWSTR lpch, int ichCurrent, int cch, int code);
typedef BOOL (__stdcall* DRAWSTATEPROC)(HDC hdc, LPARAM lData, WPARAM wData, int cx, int cy);
	// 78 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 100 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef PROPENUMPROCA PROPENUMPROC;
typedef PROPENUMPROCEXA PROPENUMPROCEX;
typedef EDITWORDBREAKPROCA EDITWORDBREAKPROC;
	// 110 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef BOOL (__stdcall* NAMEENUMPROCA)(LPSTR, LPARAM);
typedef BOOL (__stdcall* NAMEENUMPROCW)(LPWSTR, LPARAM);
typedef NAMEENUMPROCA WINSTAENUMPROCA;
typedef NAMEENUMPROCA DESKTOPENUMPROCA;
typedef NAMEENUMPROCW WINSTAENUMPROCW;
typedef NAMEENUMPROCW DESKTOPENUMPROCW;
	// 133 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef WINSTAENUMPROCA WINSTAENUMPROC;
typedef DESKTOPENUMPROCA DESKTOPENUMPROC;
	// 144 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 152 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall wvsprintfA(
LPSTR,LPCSTR,va_list arglist);
__declspec(dllimport) int __stdcall wvsprintfW(
LPWSTR,LPCWSTR,va_list arglist);
	// 204 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __cdecl wsprintfA(LPSTR, LPCSTR, ...);
__declspec(dllimport) int __cdecl wsprintfW(LPWSTR, LPCWSTR, ...);
	// 212 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 287 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 303 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 436 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 450 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 704 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HKL __stdcall LoadKeyboardLayoutA(
LPCSTR pwszKLID,UINT Flags);
__declspec(dllimport) HKL __stdcall LoadKeyboardLayoutW(
LPCWSTR pwszKLID,UINT Flags);
	// 729 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HKL __stdcall ActivateKeyboardLayout(
HKL hkl,UINT Flags);
	// 746 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall ToUnicodeEx(
UINT wVirtKey,UINT wScanCode,PBYTE lpKeyState,LPWSTR pwszBuff,int cchBuff,UINT wFlags,HKL dwhkl);
	// 760 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall UnloadKeyboardLayout(
HKL hkl);
__declspec(dllimport) BOOL __stdcall GetKeyboardLayoutNameA(
LPSTR pwszKLID);
__declspec(dllimport) BOOL __stdcall GetKeyboardLayoutNameW(
LPWSTR pwszKLID);
	// 782 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall GetKeyboardLayoutList(
int nBuff,HKL *lpList);
__declspec(dllimport) HKL __stdcall GetKeyboardLayout(
DWORD dwLayout);
	// 798 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 824 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagWNDCLASSEXA {
UINT cbSize;
UINT style;
WNDPROC lpfnWndProc;
int cbClsExtra;
int cbWndExtra;
HINSTANCE hInstance;
HICON hIcon;
HCURSOR hCursor;
HBRUSH hbrBackground;
LPCSTR lpszMenuName;
LPCSTR lpszClassName;
HICON hIconSm;
} WNDCLASSEXA, *PWNDCLASSEXA, *NPWNDCLASSEXA, *LPWNDCLASSEXA;
typedef struct tagWNDCLASSEXW {
UINT cbSize;
UINT style;
WNDPROC lpfnWndProc;
int cbClsExtra;
int cbWndExtra;
HINSTANCE hInstance;
HICON hIcon;
HCURSOR hCursor;
HBRUSH hbrBackground;
LPCWSTR lpszMenuName;
LPCWSTR lpszClassName;
HICON hIconSm;
} WNDCLASSEXW, *PWNDCLASSEXW, *NPWNDCLASSEXW, *LPWNDCLASSEXW;
typedef WNDCLASSEXA WNDCLASSEX;
typedef PWNDCLASSEXA PWNDCLASSEX;
typedef NPWNDCLASSEXA NPWNDCLASSEX;
typedef LPWNDCLASSEXA LPWNDCLASSEX;
	// 1182 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1183 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagWNDCLASSA {
UINT style;
WNDPROC lpfnWndProc;
int cbClsExtra;
int cbWndExtra;
HINSTANCE hInstance;
HICON hIcon;
HCURSOR hCursor;
HBRUSH hbrBackground;
LPCSTR lpszMenuName;
LPCSTR lpszClassName;
} WNDCLASSA, *PWNDCLASSA, *NPWNDCLASSA, *LPWNDCLASSA;
typedef struct tagWNDCLASSW {
UINT style;
WNDPROC lpfnWndProc;
int cbClsExtra;
int cbWndExtra;
HINSTANCE hInstance;
HICON hIcon;
HCURSOR hCursor;
HBRUSH hbrBackground;
LPCWSTR lpszMenuName;
LPCWSTR lpszClassName;
} WNDCLASSW, *PWNDCLASSW, *NPWNDCLASSW, *LPWNDCLASSW;
typedef WNDCLASSA WNDCLASS;
typedef PWNDCLASSA PWNDCLASS;
typedef NPWNDCLASSA NPWNDCLASS;
typedef LPWNDCLASSA LPWNDCLASS;
	// 1219 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagMSG {
HWND hwnd;
UINT message;
WPARAM wParam;
LPARAM lParam;
DWORD time;
POINT pt;
} MSG, *PMSG, *NPMSG, *LPMSG;
	// 1249 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1325 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagMINMAXINFO {
POINT ptReserved;
POINT ptMaxSize;
POINT ptMaxPosition;
POINT ptMinTrackSize;
POINT ptMaxTrackSize;
} MINMAXINFO, *PMINMAXINFO, *LPMINMAXINFO;
	// 1369 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagCOPYDATASTRUCT {
DWORD dwData;
DWORD cbData;
PVOID lpData;
} COPYDATASTRUCT, *PCOPYDATASTRUCT;
	// 1421 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1458 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1477 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1504 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1506 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1510 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1513 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagMDINEXTMENU
{
HMENU hmenuIn;
HMENU hmenuNext;
HWND hwndNext;
} MDINEXTMENU,* PMDINEXTMENU,* LPMDINEXTMENU;
	// 1563 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1591 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1594 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1598 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1604 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1635 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1643 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1664 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) UINT __stdcall RegisterWindowMessageA(
LPCSTR lpString);
__declspec(dllimport) UINT __stdcall RegisterWindowMessageW(
LPCWSTR lpString);
	// 1745 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagWINDOWPOS {
HWND hwnd;
HWND hwndInsertAfter;
int x;
int y;
int cx;
int cy;
UINT flags;
} WINDOWPOS, *LPWINDOWPOS, *PWINDOWPOS;
typedef struct tagNCCALCSIZE_PARAMS {
RECT rgrc[3];
PWINDOWPOS lppos;
} NCCALCSIZE_PARAMS, *LPNCCALCSIZE_PARAMS;
	// 1815 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1845 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1849 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1932 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1955 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall DrawEdge(HDC hdc, LPRECT qrc, UINT edge, UINT grfFlags);
	// 2021 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2055 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall DrawFrameControl(HDC, LPRECT, UINT, UINT);
	// 2075 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall DrawCaption(HWND, HDC, const RECT *, UINT);
__declspec(dllimport) BOOL __stdcall DrawAnimatedRects(HWND hwnd, int idAni, const RECT* lprcFrom, const RECT* lprcTo);
	// 2092 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2119 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2141 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagACCEL {
BYTE fVirt;
WORD key;
WORD cmd;
	// 2161 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
} ACCEL, *LPACCEL;
typedef struct tagPAINTSTRUCT {
HDC hdc;
BOOL fErase;
RECT rcPaint;
BOOL fRestore;
BOOL fIncUpdate;
BYTE rgbReserved[32];
} PAINTSTRUCT, *PPAINTSTRUCT, *NPPAINTSTRUCT, *LPPAINTSTRUCT;
typedef struct tagCREATESTRUCTA {
LPVOID lpCreateParams;
HINSTANCE hInstance;
HMENU hMenu;
HWND hwndParent;
int cy;
int cx;
int y;
int x;
LONG style;
LPCSTR lpszName;
LPCSTR lpszClass;
DWORD dwExStyle;
} CREATESTRUCTA, *LPCREATESTRUCTA;
typedef struct tagCREATESTRUCTW {
LPVOID lpCreateParams;
HINSTANCE hInstance;
HMENU hMenu;
HWND hwndParent;
int cy;
int cx;
int y;
int x;
LONG style;
LPCWSTR lpszName;
LPCWSTR lpszClass;
DWORD dwExStyle;
} CREATESTRUCTW, *LPCREATESTRUCTW;
typedef CREATESTRUCTA CREATESTRUCT;
typedef LPCREATESTRUCTA LPCREATESTRUCT;
	// 2207 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagWINDOWPLACEMENT {
UINT length;
UINT flags;
UINT showCmd;
POINT ptMinPosition;
POINT ptMaxPosition;
RECT rcNormalPosition;
} WINDOWPLACEMENT;
typedef WINDOWPLACEMENT *PWINDOWPLACEMENT, *LPWINDOWPLACEMENT;
typedef struct tagNMHDR
{
HWND hwndFrom;
UINT idFrom;
UINT code;
} NMHDR;
typedef NMHDR* LPNMHDR;
typedef struct tagSTYLESTRUCT
{
DWORD styleOld;
DWORD styleNew;
} STYLESTRUCT,* LPSTYLESTRUCT;
	// 2238 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2250 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2270 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2274 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagMEASUREITEMSTRUCT {
UINT CtlType;
UINT CtlID;
UINT itemID;
UINT itemWidth;
UINT itemHeight;
DWORD itemData;
} MEASUREITEMSTRUCT, *PMEASUREITEMSTRUCT, *LPMEASUREITEMSTRUCT;
typedef struct tagDRAWITEMSTRUCT {
UINT CtlType;
UINT CtlID;
UINT itemID;
UINT itemAction;
UINT itemState;
HWND hwndItem;
HDC hDC;
RECT rcItem;
DWORD itemData;
} DRAWITEMSTRUCT, *PDRAWITEMSTRUCT, *LPDRAWITEMSTRUCT;
typedef struct tagDELETEITEMSTRUCT {
UINT CtlType;
UINT CtlID;
UINT itemID;
HWND hwndItem;
UINT itemData;
} DELETEITEMSTRUCT, *PDELETEITEMSTRUCT, *LPDELETEITEMSTRUCT;
typedef struct tagCOMPAREITEMSTRUCT {
UINT CtlType;
UINT CtlID;
HWND hwndItem;
UINT itemID1;
DWORD itemData1;
UINT itemID2;
DWORD itemData2;
DWORD dwLocaleId;
} COMPAREITEMSTRUCT, *PCOMPAREITEMSTRUCT, *LPCOMPAREITEMSTRUCT;
__declspec(dllimport) BOOL __stdcall GetMessageA(
LPMSG lpMsg,HWND hWnd ,UINT wMsgFilterMin,UINT wMsgFilterMax);
__declspec(dllimport) BOOL __stdcall GetMessageW(
LPMSG lpMsg,HWND hWnd ,UINT wMsgFilterMin,UINT wMsgFilterMax);
	// 2355 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall TranslateMessage(
const MSG *lpMsg);
__declspec(dllimport) LONG __stdcall DispatchMessageA(
const MSG *lpMsg);
__declspec(dllimport) LONG __stdcall DispatchMessageW(
const MSG *lpMsg);
	// 2377 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall SetMessageQueue(
int cMessagesMax);
__declspec(dllimport) BOOL __stdcall PeekMessageA(
LPMSG lpMsg,HWND hWnd ,UINT wMsgFilterMin,UINT wMsgFilterMax,UINT wRemoveMsg);
__declspec(dllimport) BOOL __stdcall PeekMessageW(
LPMSG lpMsg,HWND hWnd ,UINT wMsgFilterMin,UINT wMsgFilterMax,UINT wRemoveMsg);
	// 2408 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2417 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall RegisterHotKey(
HWND hWnd ,int id,UINT fsModifiers,UINT vk);
__declspec(dllimport) BOOL __stdcall UnregisterHotKey(
HWND hWnd,int id);
	// 2456 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2465 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall ExitWindowsEx(
UINT uFlags,DWORD dwReserved);
__declspec(dllimport) BOOL __stdcall SwapMouseButton(
BOOL fSwap);
__declspec(dllimport) DWORD __stdcall GetMessagePos(
void);
__declspec(dllimport) LONG __stdcall GetMessageTime(
void);
__declspec(dllimport) LONG __stdcall GetMessageExtraInfo(
void);
__declspec(dllimport) LPARAM __stdcall SetMessageExtraInfo(
LPARAM lParam);
	// 2507 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) LRESULT __stdcall SendMessageA(
HWND hWnd,UINT Msg,WPARAM wParam,LPARAM lParam);
__declspec(dllimport) LRESULT __stdcall SendMessageW(
HWND hWnd,UINT Msg,WPARAM wParam,LPARAM lParam);
	// 2529 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) LRESULT __stdcall SendMessageTimeoutA(
HWND hWnd,UINT Msg,WPARAM wParam,LPARAM lParam,UINT fuFlags,UINT uTimeout,LPDWORD lpdwResult);
__declspec(dllimport) LRESULT __stdcall SendMessageTimeoutW(
HWND hWnd,UINT Msg,WPARAM wParam,LPARAM lParam,UINT fuFlags,UINT uTimeout,LPDWORD lpdwResult);
	// 2557 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall SendNotifyMessageA(
HWND hWnd,UINT Msg,WPARAM wParam,LPARAM lParam);
__declspec(dllimport) BOOL __stdcall SendNotifyMessageW(
HWND hWnd,UINT Msg,WPARAM wParam,LPARAM lParam);
	// 2579 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall SendMessageCallbackA(
HWND hWnd,UINT Msg,WPARAM wParam,LPARAM lParam,SENDASYNCPROC lpResultCallBack,DWORD dwData);
__declspec(dllimport) BOOL __stdcall SendMessageCallbackW(
HWND hWnd,UINT Msg,WPARAM wParam,LPARAM lParam,SENDASYNCPROC lpResultCallBack,DWORD dwData);
	// 2605 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2617 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2620 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2640 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2678 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall PostMessageA(
HWND hWnd,UINT Msg,WPARAM wParam,LPARAM lParam);
__declspec(dllimport) BOOL __stdcall PostMessageW(
HWND hWnd,UINT Msg,WPARAM wParam,LPARAM lParam);
	// 2701 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall PostThreadMessageA(
DWORD idThread,UINT Msg,WPARAM wParam,LPARAM lParam);
__declspec(dllimport) BOOL __stdcall PostThreadMessageW(
DWORD idThread,UINT Msg,WPARAM wParam,LPARAM lParam);
	// 2723 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2733 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2742 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall AttachThreadInput(
DWORD idAttach,DWORD idAttachTo,BOOL fAttach);
__declspec(dllimport) BOOL __stdcall ReplyMessage(
LRESULT lResult);
__declspec(dllimport) BOOL __stdcall WaitMessage(
void);
__declspec(dllimport) DWORD __stdcall WaitForInputIdle(
HANDLE hProcess,DWORD dwMilliseconds);
__declspec(dllimport) LRESULT __stdcall 	// 2779 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
DefWindowProcA(
HWND hWnd,UINT Msg,WPARAM wParam,LPARAM lParam);
__declspec(dllimport) LRESULT __stdcall 	// 2792 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
DefWindowProcW(
HWND hWnd,UINT Msg,WPARAM wParam,LPARAM lParam);
	// 2802 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) void __stdcall PostQuitMessage(
int nExitCode);
__declspec(dllimport) LRESULT __stdcall CallWindowProcA(
WNDPROC lpPrevWndFunc,HWND hWnd,UINT Msg,WPARAM wParam,LPARAM lParam);
__declspec(dllimport) LRESULT __stdcall CallWindowProcW(
WNDPROC lpPrevWndFunc,HWND hWnd,UINT Msg,WPARAM wParam,LPARAM lParam);
	// 2834 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2862 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall InSendMessage(
void);
	// 2885 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) UINT __stdcall GetDoubleClickTime(
void);
__declspec(dllimport) BOOL __stdcall SetDoubleClickTime(
UINT);
__declspec(dllimport) ATOM __stdcall RegisterClassA(
const WNDCLASSA *lpWndClass);
__declspec(dllimport) ATOM __stdcall RegisterClassW(
const WNDCLASSW *lpWndClass);
	// 2913 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall UnregisterClassA(
LPCSTR lpClassName,HINSTANCE hInstance);
__declspec(dllimport) BOOL __stdcall UnregisterClassW(
LPCWSTR lpClassName,HINSTANCE hInstance);
	// 2931 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall GetClassInfoA(
HINSTANCE hInstance ,LPCSTR lpClassName,LPWNDCLASSA lpWndClass);
__declspec(dllimport) BOOL __stdcall GetClassInfoW(
HINSTANCE hInstance ,LPCWSTR lpClassName,LPWNDCLASSW lpWndClass);
	// 2951 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) ATOM __stdcall RegisterClassExA(const WNDCLASSEXA *);
__declspec(dllimport) ATOM __stdcall RegisterClassExW(const WNDCLASSEXW *);
	// 2966 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall GetClassInfoExA(HINSTANCE, LPCSTR, LPWNDCLASSEXA);
__declspec(dllimport) BOOL __stdcall GetClassInfoExW(HINSTANCE, LPCWSTR, LPWNDCLASSEXW);
	// 2980 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 2982 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HWND __stdcall CreateWindowExA(
DWORD dwExStyle,LPCSTR lpClassName,LPCSTR lpWindowName,DWORD dwStyle,int X,int Y,int nWidth,int nHeight,HWND hWndParent ,HMENU hMenu,HINSTANCE hInstance,LPVOID lpParam);
__declspec(dllimport) HWND __stdcall CreateWindowExW(
DWORD dwExStyle,LPCWSTR lpClassName,LPCWSTR lpWindowName,DWORD dwStyle,int X,int Y,int nWidth,int nHeight,HWND hWndParent ,HMENU hMenu,HINSTANCE hInstance,LPVOID lpParam);
	// 3027 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 3041 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall IsWindow(
HWND hWnd);
__declspec(dllimport) BOOL __stdcall IsMenu(
HMENU hMenu);
__declspec(dllimport) BOOL __stdcall IsChild(
HWND hWndParent,HWND hWnd);
__declspec(dllimport) BOOL __stdcall DestroyWindow(
HWND hWnd);
__declspec(dllimport) BOOL __stdcall ShowWindow(
HWND hWnd,int nCmdShow);
	// 3083 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall ShowWindowAsync(
HWND hWnd,int nCmdShow);
	// 3092 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall FlashWindow(
HWND hWnd,	// 3101 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
BOOL bInvert);
	// 3103 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 3112 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall ShowOwnedPopups(
HWND hWnd,BOOL fShow);
__declspec(dllimport) BOOL __stdcall OpenIcon(
HWND hWnd);
__declspec(dllimport) BOOL __stdcall CloseWindow(
HWND hWnd);
__declspec(dllimport) BOOL __stdcall MoveWindow(
HWND hWnd,int X,int Y,int nWidth,int nHeight,BOOL bRepaint);
__declspec(dllimport) BOOL __stdcall SetWindowPos(
HWND hWnd,HWND hWndInsertAfter ,int X,int Y,int cx,int cy,UINT uFlags);
__declspec(dllimport) BOOL __stdcall GetWindowPlacement(
HWND hWnd,WINDOWPLACEMENT *lpwndpl);
__declspec(dllimport) BOOL __stdcall SetWindowPlacement(
HWND hWnd,const WINDOWPLACEMENT *lpwndpl);
__declspec(dllimport) BOOL __stdcall IsWindowVisible(
HWND hWnd);
__declspec(dllimport) BOOL __stdcall IsIconic(
HWND hWnd);
__declspec(dllimport) BOOL __stdcall AnyPopup(
void);
__declspec(dllimport) BOOL __stdcall BringWindowToTop(
HWND hWnd);
__declspec(dllimport) BOOL __stdcall IsZoomed(
HWND hWnd);
	// 3251 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
#pragma warning(disable:4103)
#pragma pack(push)
	// 28 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
#pragma pack(2)
	// 32 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack2.h"
	// 3266 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct {
DWORD style;
DWORD dwExtendedStyle;
WORD cdit;
short x;
short y;
short cx;
short cy;
} DLGTEMPLATE;
typedef DLGTEMPLATE *LPDLGTEMPLATEA;
typedef DLGTEMPLATE *LPDLGTEMPLATEW;
typedef LPDLGTEMPLATEA LPDLGTEMPLATE;
	// 3286 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef const DLGTEMPLATE *LPCDLGTEMPLATEA;
typedef const DLGTEMPLATE *LPCDLGTEMPLATEW;
typedef LPCDLGTEMPLATEA LPCDLGTEMPLATE;
	// 3293 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct {
DWORD style;
DWORD dwExtendedStyle;
short x;
short y;
short cx;
short cy;
WORD id;
} DLGITEMTEMPLATE;
typedef DLGITEMTEMPLATE *PDLGITEMTEMPLATEA;
typedef DLGITEMTEMPLATE *PDLGITEMTEMPLATEW;
typedef PDLGITEMTEMPLATEA PDLGITEMTEMPLATE;
	// 3313 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef DLGITEMTEMPLATE *LPDLGITEMTEMPLATEA;
typedef DLGITEMTEMPLATE *LPDLGITEMTEMPLATEW;
typedef LPDLGITEMTEMPLATEA LPDLGITEMTEMPLATE;
	// 3320 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
#pragma warning(disable:4103)
#pragma pack(pop)
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 36 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 37 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 3323 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HWND __stdcall CreateDialogParamA(
HINSTANCE hInstance,LPCSTR lpTemplateName,HWND hWndParent ,DLGPROC lpDialogFunc,LPARAM dwInitParam);
__declspec(dllimport) HWND __stdcall CreateDialogParamW(
HINSTANCE hInstance,LPCWSTR lpTemplateName,HWND hWndParent ,DLGPROC lpDialogFunc,LPARAM dwInitParam);
	// 3347 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HWND __stdcall CreateDialogIndirectParamA(
HINSTANCE hInstance,LPCDLGTEMPLATEA lpTemplate,HWND hWndParent,DLGPROC lpDialogFunc,LPARAM dwInitParam);
__declspec(dllimport) HWND __stdcall CreateDialogIndirectParamW(
HINSTANCE hInstance,LPCDLGTEMPLATEW lpTemplate,HWND hWndParent,DLGPROC lpDialogFunc,LPARAM dwInitParam);
	// 3371 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 3381 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 3391 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall DialogBoxParamA(
HINSTANCE hInstance,LPCSTR lpTemplateName,HWND hWndParent ,DLGPROC lpDialogFunc,LPARAM dwInitParam);
__declspec(dllimport) int __stdcall DialogBoxParamW(
HINSTANCE hInstance,LPCWSTR lpTemplateName,HWND hWndParent ,DLGPROC lpDialogFunc,LPARAM dwInitParam);
	// 3415 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall DialogBoxIndirectParamA(
HINSTANCE hInstance,LPCDLGTEMPLATEA hDialogTemplate,HWND hWndParent ,DLGPROC lpDialogFunc,LPARAM dwInitParam);
__declspec(dllimport) int __stdcall DialogBoxIndirectParamW(
HINSTANCE hInstance,LPCDLGTEMPLATEW hDialogTemplate,HWND hWndParent ,DLGPROC lpDialogFunc,LPARAM dwInitParam);
	// 3439 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 3449 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 3459 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall EndDialog(
HWND hDlg,int nResult);
__declspec(dllimport) HWND
__stdcall
GetDlgItem(
HWND hDlg,int nIDDlgItem);
__declspec(dllimport) BOOL __stdcall SetDlgItemInt(
HWND hDlg,int nIDDlgItem,UINT uValue,BOOL bSigned);
__declspec(dllimport) UINT __stdcall GetDlgItemInt(
HWND hDlg,int nIDDlgItem,BOOL *lpTranslated,BOOL bSigned);
__declspec(dllimport) BOOL __stdcall SetDlgItemTextA(
HWND hDlg,int nIDDlgItem,LPCSTR lpString);
__declspec(dllimport) BOOL __stdcall SetDlgItemTextW(
HWND hDlg,int nIDDlgItem,LPCWSTR lpString);
	// 3511 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) UINT __stdcall GetDlgItemTextA(
HWND hDlg,int nIDDlgItem,LPSTR lpString,int nMaxCount);
__declspec(dllimport) UINT __stdcall GetDlgItemTextW(
HWND hDlg,int nIDDlgItem,LPWSTR lpString,int nMaxCount);
	// 3533 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall CheckDlgButton(
HWND hDlg,int nIDButton,UINT uCheck);
__declspec(dllimport) BOOL __stdcall CheckRadioButton(
HWND hDlg,int nIDFirstButton,int nIDLastButton,int nIDCheckButton);
__declspec(dllimport) UINT __stdcall IsDlgButtonChecked(
HWND hDlg,int nIDButton);
__declspec(dllimport) LONG __stdcall SendDlgItemMessageA(
HWND hDlg,int nIDDlgItem,UINT Msg,WPARAM wParam,LPARAM lParam);
__declspec(dllimport) LONG __stdcall SendDlgItemMessageW(
HWND hDlg,int nIDDlgItem,UINT Msg,WPARAM wParam,LPARAM lParam);
	// 3581 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HWND __stdcall GetNextDlgGroupItem(
HWND hDlg,HWND hCtl,BOOL bPrevious);
__declspec(dllimport) HWND __stdcall GetNextDlgTabItem(
HWND hDlg,HWND hCtl,BOOL bPrevious);
__declspec(dllimport) int __stdcall GetDlgCtrlID(
HWND hWnd);
__declspec(dllimport) long __stdcall GetDialogBaseUnits(void);
__declspec(dllimport) LRESULT __stdcall 	// 3617 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
DefDlgProcA(
HWND hDlg,UINT Msg,WPARAM wParam,LPARAM lParam);
__declspec(dllimport) LRESULT __stdcall 	// 3630 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
DefDlgProcW(
HWND hDlg,UINT Msg,WPARAM wParam,LPARAM lParam);
	// 3640 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 3649 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 3651 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall CallMsgFilterA(
LPMSG lpMsg,int nCode);
__declspec(dllimport) BOOL __stdcall CallMsgFilterW(
LPMSG lpMsg,int nCode);
	// 3671 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 3673 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall OpenClipboard(
HWND hWndNewOwner);
__declspec(dllimport) BOOL __stdcall CloseClipboard(
void);
	// 3700 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HWND __stdcall GetClipboardOwner(
void);
__declspec(dllimport) HWND __stdcall SetClipboardViewer(
HWND hWndNewViewer);
__declspec(dllimport) HWND __stdcall GetClipboardViewer(
void);
__declspec(dllimport) BOOL __stdcall ChangeClipboardChain(
HWND hWndRemove,HWND hWndNewNext);

__declspec(dllimport) HANDLE __stdcall SetClipboardData(
UINT uFormat,HANDLE hMem);
__declspec(dllimport) HANDLE __stdcall GetClipboardData(
UINT uFormat);
__declspec(dllimport) UINT __stdcall RegisterClipboardFormatA(
LPCSTR lpszFormat);
__declspec(dllimport) UINT __stdcall RegisterClipboardFormatW(
LPCWSTR lpszFormat);
	// 3754 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall CountClipboardFormats(
void);
__declspec(dllimport) UINT __stdcall EnumClipboardFormats(
UINT format);
__declspec(dllimport) int __stdcall GetClipboardFormatNameA(
UINT format,LPSTR lpszFormatName,int cchMaxCount);
__declspec(dllimport) int __stdcall GetClipboardFormatNameW(
UINT format,LPWSTR lpszFormatName,int cchMaxCount);
	// 3786 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall EmptyClipboard(
void);
__declspec(dllimport) BOOL __stdcall IsClipboardFormatAvailable(
UINT format);
__declspec(dllimport) int __stdcall GetPriorityClipboardFormat(
UINT *paFormatPriorityList,int cFormats);
__declspec(dllimport) HWND __stdcall GetOpenClipboardWindow(
void);
	// 3813 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall CharToOemA(
LPCSTR lpszSrc,LPSTR lpszDst);
__declspec(dllimport) BOOL __stdcall CharToOemW(
LPCWSTR lpszSrc,LPSTR lpszDst);
	// 3835 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall OemToCharA(
LPCSTR lpszSrc,LPSTR lpszDst);
__declspec(dllimport) BOOL __stdcall OemToCharW(
LPCSTR lpszSrc,LPWSTR lpszDst);
	// 3853 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall CharToOemBuffA(
LPCSTR lpszSrc,LPSTR lpszDst,DWORD cchDstLength);
__declspec(dllimport) BOOL __stdcall CharToOemBuffW(
LPCWSTR lpszSrc,LPSTR lpszDst,DWORD cchDstLength);
	// 3873 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall OemToCharBuffA(
LPCSTR lpszSrc,LPSTR lpszDst,DWORD cchDstLength);
__declspec(dllimport) BOOL __stdcall OemToCharBuffW(
LPCSTR lpszSrc,LPWSTR lpszDst,DWORD cchDstLength);
	// 3893 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) LPSTR __stdcall CharUpperA(
LPSTR lpsz);
__declspec(dllimport) LPWSTR __stdcall CharUpperW(
LPWSTR lpsz);
	// 3909 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) DWORD __stdcall CharUpperBuffA(
LPSTR lpsz,DWORD cchLength);
__declspec(dllimport) DWORD __stdcall CharUpperBuffW(
LPWSTR lpsz,DWORD cchLength);
	// 3927 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) LPSTR __stdcall CharLowerA(
LPSTR lpsz);
__declspec(dllimport) LPWSTR __stdcall CharLowerW(
LPWSTR lpsz);
	// 3943 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) DWORD __stdcall CharLowerBuffA(
LPSTR lpsz,DWORD cchLength);
__declspec(dllimport) DWORD __stdcall CharLowerBuffW(
LPWSTR lpsz,DWORD cchLength);
	// 3961 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) LPSTR __stdcall CharNextA(
LPCSTR lpsz);
__declspec(dllimport) LPWSTR __stdcall CharNextW(
LPCWSTR lpsz);
	// 3977 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) LPSTR __stdcall CharPrevA(
LPCSTR lpszStart,LPCSTR lpszCurrent);
__declspec(dllimport) LPWSTR __stdcall CharPrevW(
LPCWSTR lpszStart,LPCWSTR lpszCurrent);
	// 3995 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) LPSTR __stdcall CharNextExA(
WORD CodePage,LPCSTR lpCurrentChar,DWORD dwFlags);
__declspec(dllimport) LPSTR __stdcall CharPrevExA(
WORD CodePage,LPCSTR lpStart,LPCSTR lpCurrentChar,DWORD dwFlags);
	// 4014 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall IsCharAlphaA(
CHAR ch);
__declspec(dllimport) BOOL __stdcall IsCharAlphaW(
WCHAR ch);
	// 4049 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall IsCharAlphaNumericA(
CHAR ch);
__declspec(dllimport) BOOL __stdcall IsCharAlphaNumericW(
WCHAR ch);
	// 4065 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall IsCharUpperA(
CHAR ch);
__declspec(dllimport) BOOL __stdcall IsCharUpperW(
WCHAR ch);
	// 4081 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall IsCharLowerA(
CHAR ch);
__declspec(dllimport) BOOL __stdcall IsCharLowerW(
WCHAR ch);
	// 4097 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 4099 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HWND __stdcall SetFocus(
HWND hWnd);
__declspec(dllimport) HWND __stdcall GetActiveWindow(
void);
__declspec(dllimport) HWND __stdcall GetFocus(
void);
__declspec(dllimport) UINT __stdcall GetKBCodePage(
void);
__declspec(dllimport) SHORT __stdcall GetKeyState(
int nVirtKey);
__declspec(dllimport) SHORT __stdcall GetAsyncKeyState(
int vKey);
__declspec(dllimport) BOOL __stdcall GetKeyboardState(
PBYTE lpKeyState);
__declspec(dllimport) BOOL __stdcall SetKeyboardState(
LPBYTE lpKeyState);
__declspec(dllimport) int __stdcall GetKeyNameTextA(
LONG lParam,LPSTR lpString,int nSize);
__declspec(dllimport) int __stdcall GetKeyNameTextW(
LONG lParam,LPWSTR lpString,int nSize);
	// 4169 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall GetKeyboardType(
int nTypeFlag);
__declspec(dllimport) int __stdcall ToAscii(
UINT uVirtKey,UINT uScanCode,PBYTE lpKeyState,LPWORD lpChar,UINT uFlags);
__declspec(dllimport) int __stdcall ToAsciiEx(
UINT uVirtKey,UINT uScanCode,PBYTE lpKeyState,LPWORD lpChar,UINT uFlags,HKL dwhkl);
	// 4198 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall ToUnicode(
UINT wVirtKey,UINT wScanCode,PBYTE lpKeyState,LPWSTR pwszBuff,int cchBuff,UINT wFlags);
__declspec(dllimport) DWORD __stdcall OemKeyScan(
WORD wOemChar);
__declspec(dllimport) SHORT __stdcall VkKeyScanA(
CHAR ch);
__declspec(dllimport) SHORT __stdcall VkKeyScanW(
WCHAR ch);
	// 4231 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) SHORT
__stdcall VkKeyScanExA(
CHAR ch,HKL dwhkl);
__declspec(dllimport) SHORT
__stdcall VkKeyScanExW(
WCHAR ch,HKL dwhkl);
	// 4248 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 4249 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) void __stdcall keybd_event(
BYTE bVk,BYTE bScan,DWORD dwFlags,DWORD dwExtraInfo);
__declspec(dllimport) void __stdcall mouse_event(
DWORD dwFlags,DWORD dx,DWORD dy,DWORD dwData,DWORD dwExtraInfo);
	// 4330 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 4343 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) UINT __stdcall MapVirtualKeyA(
UINT uCode,UINT uMapType);
__declspec(dllimport) UINT __stdcall MapVirtualKeyW(
UINT uCode,UINT uMapType);
	// 4361 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) UINT __stdcall MapVirtualKeyExA(
UINT uCode,UINT uMapType,HKL dwhkl);
__declspec(dllimport) UINT __stdcall MapVirtualKeyExW(
UINT uCode,UINT uMapType,HKL dwhkl);
	// 4382 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 4383 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall GetInputState(
void);
__declspec(dllimport) DWORD __stdcall GetQueueStatus(
UINT flags);
__declspec(dllimport) HWND __stdcall GetCapture(
void);
__declspec(dllimport) HWND __stdcall SetCapture(
HWND hWnd);
__declspec(dllimport) BOOL __stdcall ReleaseCapture(
void);
__declspec(dllimport) DWORD __stdcall MsgWaitForMultipleObjects(
DWORD nCount,LPHANDLE pHandles,BOOL fWaitAll,DWORD dwMilliseconds,DWORD dwWakeMask);
__declspec(dllimport) DWORD __stdcall MsgWaitForMultipleObjectsEx(
DWORD nCount,LPHANDLE pHandles,DWORD dwMilliseconds,DWORD dwWakeMask,DWORD dwFlags);
__declspec(dllimport) UINT __stdcall SetTimer(
HWND hWnd ,UINT nIDEvent,UINT uElapse,TIMERPROC lpTimerFunc);
__declspec(dllimport) BOOL __stdcall KillTimer(
HWND hWnd,UINT uIDEvent);
__declspec(dllimport) BOOL __stdcall IsWindowUnicode(
HWND hWnd);
__declspec(dllimport) BOOL __stdcall EnableWindow(
HWND hWnd,BOOL bEnable);
__declspec(dllimport) BOOL __stdcall IsWindowEnabled(
HWND hWnd);
__declspec(dllimport) HACCEL __stdcall LoadAcceleratorsA(
HINSTANCE hInstance,LPCSTR lpTableName);
__declspec(dllimport) HACCEL __stdcall LoadAcceleratorsW(
HINSTANCE hInstance,LPCWSTR lpTableName);
	// 4527 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HACCEL __stdcall CreateAcceleratorTableA(
LPACCEL, int);
__declspec(dllimport) HACCEL __stdcall CreateAcceleratorTableW(
LPACCEL, int);
	// 4543 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall DestroyAcceleratorTable(
HACCEL hAccel);
__declspec(dllimport) int __stdcall CopyAcceleratorTableA(
HACCEL hAccelSrc,LPACCEL lpAccelDst,int cAccelEntries);
__declspec(dllimport) int __stdcall CopyAcceleratorTableW(
HACCEL hAccelSrc,LPACCEL lpAccelDst,int cAccelEntries);
	// 4569 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall TranslateAcceleratorA(
HWND hWnd,HACCEL hAccTable,LPMSG lpMsg);
__declspec(dllimport) int __stdcall TranslateAcceleratorW(
HWND hWnd,HACCEL hAccTable,LPMSG lpMsg);
	// 4591 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 4593 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 4675 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 4682 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 4685 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 4693 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 4699 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall GetSystemMetrics(
int nIndex);
	// 4707 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagDROPSTRUCT
{
HWND hwndSource;
HWND hwndSink;
DWORD wFmt;
DWORD dwData;
POINT ptDrop;
DWORD dwControlData;
} DROPSTRUCT, *PDROPSTRUCT, *LPDROPSTRUCT;
__declspec(dllimport) DWORD __stdcall DragObject(HWND, HWND, UINT, DWORD, HCURSOR);
__declspec(dllimport) BOOL __stdcall DragDetect(HWND, POINT);
	// 5322 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall DrawIcon(
HDC hDC,int X,int Y,HICON hIcon);
__declspec(dllimport) BOOL __stdcall GrayStringA(
HDC hDC,HBRUSH hBrush,GRAYSTRINGPROC lpOutputFunc,LPARAM lpData,int nCount,int X,int Y,int nWidth,int nHeight);
__declspec(dllimport) BOOL __stdcall GrayStringW(
HDC hDC,HBRUSH hBrush,GRAYSTRINGPROC lpOutputFunc,LPARAM lpData,int nCount,int X,int Y,int nWidth,int nHeight);
	// 5447 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall DrawStateA(HDC, HBRUSH, DRAWSTATEPROC, LPARAM, WPARAM, int, int, int, int, UINT);
__declspec(dllimport) BOOL __stdcall DrawStateW(HDC, HBRUSH, DRAWSTATEPROC, LPARAM, WPARAM, int, int, int, int, UINT);
	// 5471 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 5472 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) LONG __stdcall TabbedTextOutA(
HDC hDC,int X,int Y,LPCSTR lpString,int nCount,int nTabPositions,LPINT lpnTabStopPositions,int nTabOrigin);
__declspec(dllimport) LONG
__stdcall
TabbedTextOutW(
HDC hDC,int X,int Y,LPCWSTR lpString,int nCount,int nTabPositions,LPINT lpnTabStopPositions,int nTabOrigin);
	// 5502 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) DWORD __stdcall GetTabbedTextExtentA(
HDC hDC,LPCSTR lpString,int nCount,int nTabPositions,LPINT lpnTabStopPositions);
__declspec(dllimport) DWORD __stdcall GetTabbedTextExtentW(
HDC hDC,LPCWSTR lpString,int nCount,int nTabPositions,LPINT lpnTabStopPositions);
	// 5526 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall UpdateWindow(
HWND hWnd);
__declspec(dllimport) HWND __stdcall SetActiveWindow(
HWND hWnd);
__declspec(dllimport) HWND __stdcall GetForegroundWindow(
void);
__declspec(dllimport) BOOL __stdcall PaintDesktop(HDC hdc);
	// 5549 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall SetForegroundWindow(
HWND hWnd);
__declspec(dllimport) HWND __stdcall WindowFromDC(
HDC hDC);
__declspec(dllimport) HDC __stdcall GetDC(
HWND hWnd);
__declspec(dllimport) HDC __stdcall GetDCEx(
HWND hWnd ,HRGN hrgnClip,DWORD flags);
__declspec(dllimport) BOOL __stdcall AlignRects(LPRECT arc, DWORD cCount, DWORD iPrimary, DWORD dwFlags);
__declspec(dllimport) HDC __stdcall GetWindowDC(
HWND hWnd);
__declspec(dllimport) int __stdcall ReleaseDC(
HWND hWnd,HDC hDC);
__declspec(dllimport) HDC __stdcall BeginPaint(
HWND hWnd,LPPAINTSTRUCT lpPaint);
__declspec(dllimport) BOOL __stdcall EndPaint(
HWND hWnd,const PAINTSTRUCT *lpPaint);
__declspec(dllimport) BOOL __stdcall GetUpdateRect(
HWND hWnd,LPRECT lpRect,BOOL bErase);
__declspec(dllimport) int __stdcall GetUpdateRgn(
HWND hWnd,HRGN hRgn,BOOL bErase);
__declspec(dllimport) int __stdcall SetWindowRgn(
HWND hWnd,HRGN hRgn,BOOL bRedraw);
__declspec(dllimport) int __stdcall GetWindowRgn(
HWND hWnd,HRGN hRgn);
__declspec(dllimport) int __stdcall ExcludeUpdateRgn(
HDC hDC,HWND hWnd);
__declspec(dllimport) BOOL __stdcall InvalidateRect(
HWND hWnd ,const RECT *lpRect,BOOL bErase);
__declspec(dllimport) BOOL __stdcall ValidateRect(
HWND hWnd ,const RECT *lpRect);
__declspec(dllimport) BOOL __stdcall InvalidateRgn(
HWND hWnd,HRGN hRgn,BOOL bErase);
__declspec(dllimport) BOOL __stdcall ValidateRgn(
HWND hWnd,HRGN hRgn);
__declspec(dllimport) BOOL __stdcall RedrawWindow(
HWND hWnd,const RECT *lprcUpdate,HRGN hrgnUpdate,UINT flags);
__declspec(dllimport) BOOL __stdcall LockWindowUpdate(
HWND hWndLock);
__declspec(dllimport) BOOL __stdcall ScrollWindow(
HWND hWnd,int XAmount,int YAmount,const RECT *lpRect,const RECT *lpClipRect);
__declspec(dllimport) BOOL __stdcall ScrollDC(
HDC hDC,int dx,int dy,const RECT *lprcScroll,const RECT *lprcClip ,HRGN hrgnUpdate,LPRECT lprcUpdate);
__declspec(dllimport) int __stdcall ScrollWindowEx(
HWND hWnd,int dx,int dy,const RECT *prcScroll,const RECT *prcClip ,HRGN hrgnUpdate,LPRECT prcUpdate,UINT flags);
	// 5792 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall SetPropA(
HWND hWnd,LPCSTR lpString,HANDLE hData);
__declspec(dllimport) BOOL __stdcall SetPropW(
HWND hWnd,LPCWSTR lpString,HANDLE hData);
	// 5884 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HANDLE __stdcall GetPropA(
HWND hWnd,LPCSTR lpString);
__declspec(dllimport) HANDLE __stdcall GetPropW(
HWND hWnd,LPCWSTR lpString);
	// 5902 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HANDLE __stdcall RemovePropA(
HWND hWnd,LPCSTR lpString);
__declspec(dllimport) HANDLE __stdcall RemovePropW(
HWND hWnd,LPCWSTR lpString);
	// 5920 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall EnumPropsExA(
HWND hWnd,PROPENUMPROCEXA lpEnumFunc,LPARAM lParam);
__declspec(dllimport) int __stdcall EnumPropsExW(
HWND hWnd,PROPENUMPROCEXW lpEnumFunc,LPARAM lParam);
	// 5940 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall EnumPropsA(
HWND hWnd,PROPENUMPROCA lpEnumFunc);
__declspec(dllimport) int __stdcall EnumPropsW(
HWND hWnd,PROPENUMPROCW lpEnumFunc);
	// 5958 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall SetWindowTextA(
HWND hWnd,LPCSTR lpString);
__declspec(dllimport) BOOL __stdcall SetWindowTextW(
HWND hWnd,LPCWSTR lpString);
	// 5976 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall GetWindowTextA(
HWND hWnd,LPSTR lpString,int nMaxCount);
__declspec(dllimport) int __stdcall GetWindowTextW(
HWND hWnd,LPWSTR lpString,int nMaxCount);
	// 5996 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall GetWindowTextLengthA(
HWND hWnd);
__declspec(dllimport) int __stdcall GetWindowTextLengthW(
HWND hWnd);
	// 6012 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall GetClientRect(
HWND hWnd,LPRECT lpRect);
__declspec(dllimport) BOOL __stdcall GetWindowRect(
HWND hWnd,LPRECT lpRect);
__declspec(dllimport) BOOL __stdcall AdjustWindowRect(
LPRECT lpRect,DWORD dwStyle,BOOL bMenu);
__declspec(dllimport) BOOL __stdcall AdjustWindowRectEx(
LPRECT lpRect,DWORD dwStyle,BOOL bMenu,DWORD dwExStyle);
typedef struct tagHELPINFO
{
UINT cbSize;
int iContextType;
int iCtrlId;
HANDLE hItemHandle;
DWORD dwContextId;
POINT MousePos;
} HELPINFO, *LPHELPINFO;
__declspec(dllimport) BOOL __stdcall SetWindowContextHelpId(HWND, DWORD);
__declspec(dllimport) DWORD __stdcall GetWindowContextHelpId(HWND);
__declspec(dllimport) BOOL __stdcall SetMenuContextHelpId(HMENU, DWORD);
__declspec(dllimport) DWORD __stdcall GetMenuContextHelpId(HMENU);
	// 6063 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 6088 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 6098 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 6105 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 6117 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall MessageBoxA(
HWND hWnd ,LPCSTR lpText,LPCSTR lpCaption,UINT uType);
__declspec(dllimport) int __stdcall MessageBoxW(
HWND hWnd ,LPCWSTR lpText,LPCWSTR lpCaption,UINT uType);
	// 6154 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall MessageBoxExA(
HWND hWnd ,LPCSTR lpText,LPCSTR lpCaption,UINT uType,WORD wLanguageId);
__declspec(dllimport) int __stdcall MessageBoxExW(
HWND hWnd ,LPCWSTR lpText,LPCWSTR lpCaption,UINT uType,WORD wLanguageId);
	// 6178 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef void (__stdcall *MSGBOXCALLBACK)(LPHELPINFO lpHelpInfo);
typedef struct tagMSGBOXPARAMSA
{
UINT cbSize;
HWND hwndOwner;
HINSTANCE hInstance;
LPCSTR lpszText;
LPCSTR lpszCaption;
DWORD dwStyle;
LPCSTR lpszIcon;
DWORD dwContextHelpId;
MSGBOXCALLBACK lpfnMsgBoxCallback;
DWORD dwLanguageId;
} MSGBOXPARAMSA, *PMSGBOXPARAMSA, *LPMSGBOXPARAMSA;
typedef struct tagMSGBOXPARAMSW
{
UINT cbSize;
HWND hwndOwner;
HINSTANCE hInstance;
LPCWSTR lpszText;
LPCWSTR lpszCaption;
DWORD dwStyle;
LPCWSTR lpszIcon;
DWORD dwContextHelpId;
MSGBOXCALLBACK lpfnMsgBoxCallback;
DWORD dwLanguageId;
} MSGBOXPARAMSW, *PMSGBOXPARAMSW, *LPMSGBOXPARAMSW;
typedef MSGBOXPARAMSA MSGBOXPARAMS;
typedef PMSGBOXPARAMSA PMSGBOXPARAMS;
typedef LPMSGBOXPARAMSA LPMSGBOXPARAMS;
	// 6218 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall MessageBoxIndirectA(LPMSGBOXPARAMSA);
__declspec(dllimport) int __stdcall MessageBoxIndirectW(LPMSGBOXPARAMSW);
	// 6227 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 6228 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall MessageBeep(
UINT uType);
	// 6238 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall ShowCursor(
BOOL bShow);
__declspec(dllimport) BOOL __stdcall SetCursorPos(
int X,int Y);
__declspec(dllimport) HCURSOR __stdcall SetCursor(
HCURSOR hCursor);
__declspec(dllimport) BOOL __stdcall GetCursorPos(
LPPOINT lpPoint);
__declspec(dllimport) BOOL __stdcall ClipCursor(
const RECT *lpRect);
__declspec(dllimport) BOOL __stdcall GetClipCursor(
LPRECT lpRect);
__declspec(dllimport) HCURSOR __stdcall GetCursor(
void);
__declspec(dllimport) BOOL __stdcall CreateCaret(
HWND hWnd,HBITMAP hBitmap ,int nWidth,int nHeight);
__declspec(dllimport) UINT __stdcall GetCaretBlinkTime(
void);
__declspec(dllimport) BOOL __stdcall SetCaretBlinkTime(
UINT uMSeconds);
__declspec(dllimport) BOOL __stdcall DestroyCaret(
void);
__declspec(dllimport) BOOL __stdcall HideCaret(
HWND hWnd);
__declspec(dllimport) BOOL __stdcall ShowCaret(
HWND hWnd);
__declspec(dllimport) BOOL __stdcall SetCaretPos(
int X,int Y);
__declspec(dllimport) BOOL __stdcall GetCaretPos(
LPPOINT lpPoint);
__declspec(dllimport) BOOL __stdcall ClientToScreen(
HWND hWnd,LPPOINT lpPoint);
__declspec(dllimport) BOOL __stdcall ScreenToClient(
HWND hWnd,LPPOINT lpPoint);
__declspec(dllimport) int __stdcall MapWindowPoints(
HWND hWndFrom,HWND hWndTo,LPPOINT lpPoints,UINT cPoints);
__declspec(dllimport) HWND __stdcall WindowFromPoint(
POINT Point);
__declspec(dllimport) HWND __stdcall ChildWindowFromPoint(
HWND hWndParent,POINT Point);
__declspec(dllimport) HWND __stdcall ChildWindowFromPointEx(HWND, POINT, UINT);
	// 6378 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall DrawFocusRect(
HDC hDC,const RECT* lprc);
__declspec(dllimport) int __stdcall FillRect(
HDC hDC,const RECT *lprc,HBRUSH hbr);
__declspec(dllimport) int __stdcall FrameRect(
HDC hDC,const RECT *lprc,HBRUSH hbr);
__declspec(dllimport) BOOL __stdcall InvertRect(
HDC hDC,const RECT *lprc);
__declspec(dllimport) BOOL __stdcall SetRect(
LPRECT lprc,int xLeft,int yTop,int xRight,int yBottom);
__declspec(dllimport) BOOL __stdcall SetRectEmpty(
LPRECT lprc);
__declspec(dllimport) BOOL __stdcall CopyRect(
LPRECT lprcDst,const RECT *lprcSrc);
__declspec(dllimport) BOOL __stdcall InflateRect(
LPRECT lprc,int dx,int dy);
__declspec(dllimport) BOOL __stdcall IntersectRect(
LPRECT lprcDst,const RECT *lprcSrc1,const RECT *lprcSrc2);
__declspec(dllimport) BOOL __stdcall UnionRect(
LPRECT lprcDst,const RECT *lprcSrc1,const RECT *lprcSrc2);
__declspec(dllimport) BOOL __stdcall SubtractRect(
LPRECT lprcDst,const RECT *lprcSrc1,const RECT *lprcSrc2);
__declspec(dllimport) BOOL __stdcall OffsetRect(
LPRECT lprc,int dx,int dy);
__declspec(dllimport) BOOL __stdcall IsRectEmpty(
const RECT *lprc);
__declspec(dllimport) BOOL __stdcall EqualRect(
const RECT *lprc1,const RECT *lprc2);
__declspec(dllimport) BOOL __stdcall PtInRect(
const RECT *lprc,POINT pt);
__declspec(dllimport) LONG __stdcall GetWindowLongA(
HWND hWnd, int nIndex);
__declspec(dllimport) LONG __stdcall GetWindowLongW(
HWND hWnd, int nIndex);
__declspec(dllimport) LONG __stdcall SetWindowLongA(
HWND hWnd,int nIndex,LONG dwNewLong);
__declspec(dllimport) LONG __stdcall SetWindowLongW(
HWND hWnd, int nIndex, LONG dwNewLong);
__declspec(dllimport) HWND __stdcall GetDesktopWindow(
void);
__declspec(dllimport) HWND __stdcall GetParent(
HWND hWnd);
__declspec(dllimport) HWND __stdcall SetParent(
HWND hWndChild,HWND hWndNewParent);
__declspec(dllimport) BOOL __stdcall EnumChildWindows(
HWND hWndParent,WNDENUMPROC lpEnumFunc,LPARAM lParam);
__declspec(dllimport) HWND __stdcall FindWindowA(
LPCSTR lpClassName ,LPCSTR lpWindowName);
__declspec(dllimport) HWND __stdcall FindWindowW(
LPCWSTR lpClassName ,LPCWSTR lpWindowName);
	// 6732 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HWND __stdcall FindWindowExA(HWND, HWND, LPCSTR, LPCSTR);
__declspec(dllimport) HWND __stdcall FindWindowExW(HWND, HWND, LPCWSTR, LPCWSTR);
	// 6741 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 6743 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall EnumWindows(
WNDENUMPROC lpEnumFunc,LPARAM lParam);

__declspec(dllimport) BOOL __stdcall EnumThreadWindows(
DWORD dwThreadId,WNDENUMPROC lpfn,LPARAM lParam);
__declspec(dllimport) int __stdcall GetClassNameA(
HWND hWnd,LPSTR lpClassName,int nMaxCount);
__declspec(dllimport) int __stdcall GetClassNameW(
HWND hWnd,LPWSTR lpClassName,int nMaxCount);
	// 6781 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HWND __stdcall GetTopWindow(
HWND hWnd);
__declspec(dllimport) DWORD __stdcall GetWindowThreadProcessId(
HWND hWnd,LPDWORD lpdwProcessId);
__declspec(dllimport) HWND __stdcall GetLastActivePopup(
HWND hWnd);
	// 6823 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HWND __stdcall GetWindow(
HWND hWnd,UINT uCmd);
	// 7073 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7081 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HBITMAP __stdcall LoadBitmapA(
HINSTANCE hInstance,LPCSTR lpBitmapName);
__declspec(dllimport) HBITMAP __stdcall LoadBitmapW(
HINSTANCE hInstance,LPCWSTR lpBitmapName);
	// 7103 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HCURSOR __stdcall LoadCursorA(
HINSTANCE hInstance,LPCSTR lpCursorName);
__declspec(dllimport) HCURSOR __stdcall LoadCursorW(
HINSTANCE hInstance,LPCWSTR lpCursorName);
	// 7121 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HCURSOR __stdcall LoadCursorFromFileA(
LPCSTR lpFileName);
__declspec(dllimport) HCURSOR __stdcall LoadCursorFromFileW(
LPCWSTR lpFileName);
	// 7137 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HCURSOR __stdcall CreateCursor(
HINSTANCE hInst,int xHotSpot,int yHotSpot,int nWidth,int nHeight,const void *pvANDPlane,const void *pvXORPlane);
__declspec(dllimport) BOOL __stdcall DestroyCursor(
HCURSOR hCursor);
	// 7165 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7185 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7189 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall SetSystemCursor(
HCURSOR hcur,DWORD id);
typedef struct _ICONINFO {
BOOL fIcon;
DWORD xHotspot;
DWORD yHotspot;
HBITMAP hbmMask;
HBITMAP hbmColor;
} ICONINFO;
typedef ICONINFO *PICONINFO;
__declspec(dllimport) HICON __stdcall LoadIconA(
HINSTANCE hInstance,LPCSTR lpIconName);
__declspec(dllimport) HICON __stdcall LoadIconW(
HINSTANCE hInstance,LPCWSTR lpIconName);
	// 7223 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HICON __stdcall CreateIcon(
HINSTANCE hInstance,int nWidth,int nHeight,BYTE cPlanes,BYTE cBitsPixel,const BYTE *lpbANDbits,const BYTE *lpbXORbits);
__declspec(dllimport) BOOL __stdcall DestroyIcon(
HICON hIcon);
__declspec(dllimport) int __stdcall LookupIconIdFromDirectory(
PBYTE presbits,BOOL fIcon);
__declspec(dllimport) int __stdcall LookupIconIdFromDirectoryEx(
PBYTE presbits,BOOL fIcon,int cxDesired,int cyDesired,UINT Flags);
	// 7261 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HICON __stdcall CreateIconFromResource(
PBYTE presbits,DWORD dwResSize,BOOL fIcon,DWORD dwVer);
__declspec(dllimport) HICON __stdcall CreateIconFromResourceEx(
PBYTE presbits,DWORD dwResSize,BOOL fIcon,DWORD dwVer,int cxDesired,int cyDesired,UINT Flags);
typedef struct tagCURSORSHAPE
{
int xHotSpot;
int yHotSpot;
int cx;
int cy;
int cbWidth;
BYTE Planes;
BYTE BitsPixel;
} CURSORSHAPE, *LPCURSORSHAPE;
	// 7296 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HANDLE __stdcall LoadImageA(
HINSTANCE,LPCSTR,UINT,int,int,UINT);
__declspec(dllimport) HANDLE __stdcall LoadImageW(
HINSTANCE,LPCWSTR,UINT,int,int,UINT);
	// 7342 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HANDLE __stdcall CopyImage(
HANDLE,UINT,int,int,UINT);
__declspec(dllimport) BOOL __stdcall DrawIconEx(HDC hdc, int xLeft, int yTop,HICON hIcon, int cxWidth, int cyWidth,UINT istepIfAniCur, HBRUSH hbrFlickerFreeDraw, UINT diFlags);
	// 7363 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) HICON __stdcall CreateIconIndirect(
PICONINFO piconinfo);
__declspec(dllimport) HICON __stdcall CopyIcon(
HICON hIcon);
__declspec(dllimport) BOOL __stdcall GetIconInfo(
HICON hIcon,PICONINFO piconinfo);
	// 7387 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall LoadStringA(
HINSTANCE hInstance,UINT uID,LPSTR lpBuffer,int nBufferMax);
__declspec(dllimport) int __stdcall LoadStringW(
HINSTANCE hInstance,UINT uID,LPWSTR lpBuffer,int nBufferMax);
	// 7531 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7548 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7580 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7584 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7603 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7613 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7660 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7665 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7669 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7710 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7727 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7747 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7773 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7785 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7802 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7804 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall IsDialogMessageA(
HWND hDlg,LPMSG lpMsg);
__declspec(dllimport) BOOL __stdcall IsDialogMessageW(
HWND hDlg,LPMSG lpMsg);
	// 7840 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7842 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall MapDialogRect(
HWND hDlg,LPRECT lpRect);
__declspec(dllimport) int __stdcall DlgDirListA(
HWND hDlg,LPSTR lpPathSpec,int nIDListBox,int nIDStaticPath,UINT uFileType);
__declspec(dllimport) int __stdcall DlgDirListW(
HWND hDlg,LPWSTR lpPathSpec,int nIDListBox,int nIDStaticPath,UINT uFileType);
	// 7873 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall DlgDirSelectExA(
HWND hDlg,LPSTR lpString,int nCount,int nIDListBox);
__declspec(dllimport) BOOL __stdcall DlgDirSelectExW(
HWND hDlg,LPWSTR lpString,int nCount,int nIDListBox);
	// 7909 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) int __stdcall DlgDirListComboBoxA(
HWND hDlg,LPSTR lpPathSpec,int nIDComboBox,int nIDStaticPath,UINT uFiletype);
__declspec(dllimport) int __stdcall DlgDirListComboBoxW(
HWND hDlg,LPWSTR lpPathSpec,int nIDComboBox,int nIDStaticPath,UINT uFiletype);
	// 7933 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall DlgDirSelectComboBoxExA(
HWND hDlg,LPSTR lpString,int nCount,int nIDComboBox);
__declspec(dllimport) BOOL __stdcall DlgDirSelectComboBoxExW(
HWND hDlg,LPWSTR lpString,int nCount,int nIDComboBox);
	// 7955 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7981 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 7990 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8086 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8091 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8093 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8118 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8123 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8169 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8172 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8214 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8219 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8220 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8242 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8246 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagSCROLLINFO
{
UINT cbSize;
UINT fMask;
int nMin;
int nMax;
UINT nPage;
int nPos;
int nTrackPos;
} SCROLLINFO, *LPSCROLLINFO;
typedef SCROLLINFO const *LPCSCROLLINFO;
__declspec(dllimport) int __stdcall SetScrollInfo(HWND, int, LPCSCROLLINFO, BOOL);
__declspec(dllimport) BOOL __stdcall GetScrollInfo(HWND, int, LPSCROLLINFO);
	// 8283 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8284 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8285 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8451 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"

	// 8587 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8672 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8688 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8704 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8711 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8736 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagNONCLIENTMETRICSA
{
UINT cbSize;
int iBorderWidth;
int iScrollWidth;
int iScrollHeight;
int iCaptionWidth;
int iCaptionHeight;
LOGFONTA lfCaptionFont;
int iSmCaptionWidth;
int iSmCaptionHeight;
LOGFONTA lfSmCaptionFont;
int iMenuWidth;
int iMenuHeight;
LOGFONTA lfMenuFont;
LOGFONTA lfStatusFont;
LOGFONTA lfMessageFont;
} NONCLIENTMETRICSA, *PNONCLIENTMETRICSA,* LPNONCLIENTMETRICSA;
typedef struct tagNONCLIENTMETRICSW
{
UINT cbSize;
int iBorderWidth;
int iScrollWidth;
int iScrollHeight;
int iCaptionWidth;
int iCaptionHeight;
LOGFONTW lfCaptionFont;
int iSmCaptionWidth;
int iSmCaptionHeight;
LOGFONTW lfSmCaptionFont;
int iMenuWidth;
int iMenuHeight;
LOGFONTW lfMenuFont;
LOGFONTW lfStatusFont;
LOGFONTW lfMessageFont;
} NONCLIENTMETRICSW, *PNONCLIENTMETRICSW,* LPNONCLIENTMETRICSW;
typedef NONCLIENTMETRICSA NONCLIENTMETRICS;
typedef PNONCLIENTMETRICSA PNONCLIENTMETRICS;
typedef LPNONCLIENTMETRICSA LPNONCLIENTMETRICS;
	// 8793 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8794 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8795 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagMINIMIZEDMETRICS
{
UINT cbSize;
int iWidth;
int iHorzGap;
int iVertGap;
int iArrange;
} MINIMIZEDMETRICS, *PMINIMIZEDMETRICS, *LPMINIMIZEDMETRICS;
typedef struct tagICONMETRICSA
{
UINT cbSize;
int iHorzSpacing;
int iVertSpacing;
int iTitleWrap;
LOGFONTA lfFont;
} ICONMETRICSA, *PICONMETRICSA, *LPICONMETRICSA;
typedef struct tagICONMETRICSW
{
UINT cbSize;
int iHorzSpacing;
int iVertSpacing;
int iTitleWrap;
LOGFONTW lfFont;
} ICONMETRICSW, *PICONMETRICSW, *LPICONMETRICSW;
typedef ICONMETRICSA ICONMETRICS;
typedef PICONMETRICSA PICONMETRICS;
typedef LPICONMETRICSA LPICONMETRICS;
	// 8846 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8847 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 8848 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagANIMATIONINFO
{
UINT cbSize;
int iMinAnimate;
} ANIMATIONINFO, *LPANIMATIONINFO;
typedef struct tagSERIALKEYSA
{
UINT cbSize;
DWORD dwFlags;
LPSTR lpszActivePort;
LPSTR lpszPort;
UINT iBaudRate;
UINT iPortState;
UINT iActive;
} SERIALKEYSA, *LPSERIALKEYSA;
typedef struct tagSERIALKEYSW
{
UINT cbSize;
DWORD dwFlags;
LPWSTR lpszActivePort;
LPWSTR lpszPort;
UINT iBaudRate;
UINT iPortState;
UINT iActive;
} SERIALKEYSW, *LPSERIALKEYSW;
typedef SERIALKEYSA SERIALKEYS;
typedef LPSERIALKEYSA LPSERIALKEYS;
	// 8882 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagHIGHCONTRASTA
{
UINT cbSize;
DWORD dwFlags;
LPSTR lpszDefaultScheme;
} HIGHCONTRASTA, *LPHIGHCONTRASTA;
typedef struct tagHIGHCONTRASTW
{
UINT cbSize;
DWORD dwFlags;
LPWSTR lpszDefaultScheme;
} HIGHCONTRASTW, *LPHIGHCONTRASTW;
typedef HIGHCONTRASTA HIGHCONTRAST;
typedef LPHIGHCONTRASTA LPHIGHCONTRAST;
	// 8908 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) LONG __stdcall ChangeDisplaySettingsA(
LPDEVMODEA lpDevMode,DWORD dwFlags);
__declspec(dllimport) LONG __stdcall ChangeDisplaySettingsW(
LPDEVMODEW lpDevMode,DWORD dwFlags);
	// 8957 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) LONG __stdcall ChangeDisplaySettingsExA(
LPCSTR lpszDeviceName,LPDEVMODEA lpDevMode,HWND hwnd,DWORD dwflags,LPVOID lParam);
__declspec(dllimport) LONG __stdcall ChangeDisplaySettingsExW(
LPCWSTR lpszDeviceName,LPDEVMODEW lpDevMode,HWND hwnd,DWORD dwflags,LPVOID lParam);
	// 8981 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall EnumDisplaySettingsA(
LPCSTR lpszDeviceName,DWORD iModeNum,LPDEVMODEA lpDevMode);
__declspec(dllimport) BOOL __stdcall EnumDisplaySettingsW(
LPCWSTR lpszDeviceName,DWORD iModeNum,LPDEVMODEW lpDevMode);
	// 9004 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 9029 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 9031 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 9032 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
__declspec(dllimport) BOOL __stdcall SystemParametersInfoA(
UINT uiAction,UINT uiParam,PVOID pvParam,UINT fWinIni);
__declspec(dllimport) BOOL __stdcall SystemParametersInfoW(
UINT uiAction,UINT uiParam,PVOID pvParam,UINT fWinIni);
	// 9055 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 9057 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagFILTERKEYS
{
UINT cbSize;
DWORD dwFlags;
DWORD iWaitMSec;
DWORD iDelayMSec;
DWORD iRepeatMSec;
DWORD iBounceMSec;
} FILTERKEYS, *LPFILTERKEYS;
typedef struct tagSTICKYKEYS
{
UINT cbSize;
DWORD dwFlags;
} STICKYKEYS, *LPSTICKYKEYS;
	// 9118 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagMOUSEKEYS
{
UINT cbSize;
DWORD dwFlags;
DWORD iMaxSpeed;
DWORD iTimeToMaxSpeed;
DWORD iCtrlSpeed;
DWORD dwReserved1;
DWORD dwReserved2;
} MOUSEKEYS, *LPMOUSEKEYS;
	// 9148 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagACCESSTIMEOUT
{
UINT cbSize;
DWORD dwFlags;
DWORD iTimeOutMSec;
} ACCESSTIMEOUT, *LPACCESSTIMEOUT;
typedef struct tagSOUNDSENTRYA
{
UINT cbSize;
DWORD dwFlags;
DWORD iFSTextEffect;
DWORD iFSTextEffectMSec;
DWORD iFSTextEffectColorBits;
DWORD iFSGrafEffect;
DWORD iFSGrafEffectMSec;
DWORD iFSGrafEffectColor;
DWORD iWindowsEffect;
DWORD iWindowsEffectMSec;
LPSTR lpszWindowsEffectDLL;
DWORD iWindowsEffectOrdinal;
} SOUNDSENTRYA, *LPSOUNDSENTRYA;
typedef struct tagSOUNDSENTRYW
{
UINT cbSize;
DWORD dwFlags;
DWORD iFSTextEffect;
DWORD iFSTextEffectMSec;
DWORD iFSTextEffectColorBits;
DWORD iFSGrafEffect;
DWORD iFSGrafEffectMSec;
DWORD iFSGrafEffectColor;
DWORD iWindowsEffect;
DWORD iWindowsEffectMSec;
LPWSTR lpszWindowsEffectDLL;
DWORD iWindowsEffectOrdinal;
} SOUNDSENTRYW, *LPSOUNDSENTRYW;
typedef SOUNDSENTRYA SOUNDSENTRY;
typedef LPSOUNDSENTRYA LPSOUNDSENTRY;
	// 9216 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
typedef struct tagTOGGLEKEYS
{
UINT cbSize;
DWORD dwFlags;
} TOGGLEKEYS, *LPTOGGLEKEYS;
__declspec(dllimport) void __stdcall SetDebugErrorLevel(
DWORD dwLevel);
__declspec(dllimport) void __stdcall SetLastErrorEx(
DWORD dwErrCode,DWORD dwType);
	// 10129 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 10138 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 10146 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
}
	// 10150 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 10152 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winuser.h"
	// 167 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnls.h"
extern "C" {
	// 24 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnls.h"
}
	// 1381 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnls.h"
	// 1383 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnls.h"
	// 173 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 174 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
extern "C" {
	// 28 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
typedef struct _COORD {
SHORT X;
SHORT Y;
} COORD, *PCOORD;
typedef struct _SMALL_RECT {
SHORT Left;
SHORT Top;
SHORT Right;
SHORT Bottom;
} SMALL_RECT, *PSMALL_RECT;
typedef struct _KEY_EVENT_RECORD {
BOOL bKeyDown;
WORD wRepeatCount;
WORD wVirtualKeyCode;
WORD wVirtualScanCode;
union {
WCHAR UnicodeChar;
CHAR AsciiChar;
} uChar;
DWORD dwControlKeyState;
} KEY_EVENT_RECORD, *PKEY_EVENT_RECORD;
typedef struct _MOUSE_EVENT_RECORD {
COORD dwMousePosition;
DWORD dwButtonState;
DWORD dwControlKeyState;
DWORD dwEventFlags;
} MOUSE_EVENT_RECORD, *PMOUSE_EVENT_RECORD;
typedef struct _WINDOW_BUFFER_SIZE_RECORD {
COORD dwSize;
} WINDOW_BUFFER_SIZE_RECORD, *PWINDOW_BUFFER_SIZE_RECORD;
typedef struct _MENU_EVENT_RECORD {
UINT dwCommandId;
} MENU_EVENT_RECORD, *PMENU_EVENT_RECORD;
typedef struct _FOCUS_EVENT_RECORD {
BOOL bSetFocus;
} FOCUS_EVENT_RECORD, *PFOCUS_EVENT_RECORD;
typedef struct _INPUT_RECORD {
WORD EventType;
union {
KEY_EVENT_RECORD KeyEvent;
MOUSE_EVENT_RECORD MouseEvent;
WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
MENU_EVENT_RECORD MenuEvent;
FOCUS_EVENT_RECORD FocusEvent;
} Event;
} INPUT_RECORD, *PINPUT_RECORD;
typedef struct _CHAR_INFO {
union {
WCHAR UnicodeChar;
CHAR AsciiChar;
} Char;
WORD Attributes;
} CHAR_INFO, *PCHAR_INFO;
typedef struct _CONSOLE_SCREEN_BUFFER_INFO {
COORD dwSize;
COORD dwCursorPosition;
WORD wAttributes;
SMALL_RECT srWindow;
COORD dwMaximumWindowSize;
} CONSOLE_SCREEN_BUFFER_INFO, *PCONSOLE_SCREEN_BUFFER_INFO;
typedef struct _CONSOLE_CURSOR_INFO {
DWORD dwSize;
BOOL bVisible;
} CONSOLE_CURSOR_INFO, *PCONSOLE_CURSOR_INFO;
typedef
BOOL
(__stdcall *PHANDLER_ROUTINE)(
DWORD CtrlType);
__declspec(dllimport) BOOL __stdcall PeekConsoleInputA(
HANDLE hConsoleInput,PINPUT_RECORD lpBuffer,DWORD nLength,LPDWORD lpNumberOfEventsRead);
__declspec(dllimport) BOOL __stdcall PeekConsoleInputW(
HANDLE hConsoleInput,PINPUT_RECORD lpBuffer,DWORD nLength,LPDWORD lpNumberOfEventsRead);
	// 238 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
__declspec(dllimport) BOOL __stdcall ReadConsoleInputA(
HANDLE hConsoleInput,PINPUT_RECORD lpBuffer,DWORD nLength,LPDWORD lpNumberOfEventsRead);
__declspec(dllimport) BOOL __stdcall ReadConsoleInputW(
HANDLE hConsoleInput,PINPUT_RECORD lpBuffer,DWORD nLength,LPDWORD lpNumberOfEventsRead);
	// 262 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
__declspec(dllimport) BOOL __stdcall WriteConsoleInputA(
HANDLE hConsoleInput,const INPUT_RECORD *lpBuffer,DWORD nLength,LPDWORD lpNumberOfEventsWritten);
__declspec(dllimport) BOOL __stdcall WriteConsoleInputW(
HANDLE hConsoleInput,const INPUT_RECORD *lpBuffer,DWORD nLength,LPDWORD lpNumberOfEventsWritten);
	// 286 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
__declspec(dllimport) BOOL __stdcall ReadConsoleOutputA(
HANDLE hConsoleOutput,PCHAR_INFO lpBuffer,COORD dwBufferSize,COORD dwBufferCoord,PSMALL_RECT lpReadRegion);
__declspec(dllimport) BOOL __stdcall ReadConsoleOutputW(
HANDLE hConsoleOutput,PCHAR_INFO lpBuffer,COORD dwBufferSize,COORD dwBufferCoord,PSMALL_RECT lpReadRegion);
	// 312 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
__declspec(dllimport) BOOL __stdcall WriteConsoleOutputA(
HANDLE hConsoleOutput,const CHAR_INFO *lpBuffer,COORD dwBufferSize,COORD dwBufferCoord,PSMALL_RECT lpWriteRegion);
__declspec(dllimport) BOOL __stdcall WriteConsoleOutputW(
HANDLE hConsoleOutput,const CHAR_INFO *lpBuffer,COORD dwBufferSize,COORD dwBufferCoord,PSMALL_RECT lpWriteRegion);
	// 338 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
__declspec(dllimport) BOOL __stdcall ReadConsoleOutputCharacterA(
HANDLE hConsoleOutput,LPSTR lpCharacter,DWORD nLength,COORD dwReadCoord,LPDWORD lpNumberOfCharsRead);
__declspec(dllimport) BOOL __stdcall ReadConsoleOutputCharacterW(
HANDLE hConsoleOutput,LPWSTR lpCharacter,DWORD nLength,COORD dwReadCoord,LPDWORD lpNumberOfCharsRead);
	// 364 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
__declspec(dllimport) BOOL __stdcall ReadConsoleOutputAttribute(
HANDLE hConsoleOutput,LPWORD lpAttribute,DWORD nLength,COORD dwReadCoord,LPDWORD lpNumberOfAttrsRead);
__declspec(dllimport) BOOL __stdcall WriteConsoleOutputCharacterA(
HANDLE hConsoleOutput,LPCSTR lpCharacter,DWORD nLength,COORD dwWriteCoord,LPDWORD lpNumberOfCharsWritten);
__declspec(dllimport) BOOL __stdcall WriteConsoleOutputCharacterW(
HANDLE hConsoleOutput,LPCWSTR lpCharacter,DWORD nLength,COORD dwWriteCoord,LPDWORD lpNumberOfCharsWritten);
	// 401 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
__declspec(dllimport) BOOL __stdcall WriteConsoleOutputAttribute(
HANDLE hConsoleOutput,const WORD *lpAttribute,DWORD nLength,COORD dwWriteCoord,LPDWORD lpNumberOfAttrsWritten);
__declspec(dllimport) BOOL __stdcall FillConsoleOutputCharacterA(
HANDLE hConsoleOutput,CHAR cCharacter,DWORD nLength,COORD dwWriteCoord,LPDWORD lpNumberOfCharsWritten);
__declspec(dllimport) BOOL __stdcall FillConsoleOutputCharacterW(
HANDLE hConsoleOutput,WCHAR cCharacter,DWORD nLength,COORD dwWriteCoord,LPDWORD lpNumberOfCharsWritten);
	// 438 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
__declspec(dllimport) BOOL __stdcall FillConsoleOutputAttribute(
HANDLE hConsoleOutput,WORD wAttribute,DWORD nLength,COORD dwWriteCoord,LPDWORD lpNumberOfAttrsWritten);
__declspec(dllimport) BOOL __stdcall GetConsoleMode(
HANDLE hConsoleHandle,LPDWORD lpMode);
__declspec(dllimport) BOOL __stdcall GetNumberOfConsoleInputEvents(
HANDLE hConsoleInput,LPDWORD lpNumberOfEvents);
__declspec(dllimport) BOOL __stdcall GetConsoleScreenBufferInfo(
HANDLE hConsoleOutput,PCONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);
__declspec(dllimport) COORD __stdcall GetLargestConsoleWindowSize(
HANDLE hConsoleOutput);
__declspec(dllimport) BOOL __stdcall GetConsoleCursorInfo(
HANDLE hConsoleOutput,PCONSOLE_CURSOR_INFO lpConsoleCursorInfo);
__declspec(dllimport) BOOL __stdcall GetNumberOfConsoleMouseButtons(
LPDWORD lpNumberOfMouseButtons);
__declspec(dllimport) BOOL __stdcall SetConsoleMode(
HANDLE hConsoleHandle,DWORD dwMode);
__declspec(dllimport) BOOL __stdcall SetConsoleActiveScreenBuffer(
HANDLE hConsoleOutput);
__declspec(dllimport) BOOL __stdcall FlushConsoleInputBuffer(
HANDLE hConsoleInput);
__declspec(dllimport) BOOL __stdcall SetConsoleScreenBufferSize(
HANDLE hConsoleOutput,COORD dwSize);
__declspec(dllimport) BOOL __stdcall SetConsoleCursorPosition(
HANDLE hConsoleOutput,COORD dwCursorPosition);
__declspec(dllimport) BOOL __stdcall SetConsoleCursorInfo(
HANDLE hConsoleOutput,const CONSOLE_CURSOR_INFO *lpConsoleCursorInfo);
__declspec(dllimport) BOOL __stdcall ScrollConsoleScreenBufferA(
HANDLE hConsoleOutput,const SMALL_RECT *lpScrollRectangle,const SMALL_RECT *lpClipRectangle,COORD dwDestinationOrigin,const CHAR_INFO *lpFill);
__declspec(dllimport) BOOL __stdcall ScrollConsoleScreenBufferW(
HANDLE hConsoleOutput,const SMALL_RECT *lpScrollRectangle,const SMALL_RECT *lpClipRectangle,COORD dwDestinationOrigin,const CHAR_INFO *lpFill);
	// 567 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
__declspec(dllimport) BOOL __stdcall SetConsoleWindowInfo(
HANDLE hConsoleOutput,BOOL bAbsolute,const SMALL_RECT *lpConsoleWindow);
__declspec(dllimport) BOOL __stdcall SetConsoleTextAttribute(
HANDLE hConsoleOutput,WORD wAttributes);
__declspec(dllimport) BOOL __stdcall SetConsoleCtrlHandler(
PHANDLER_ROUTINE HandlerRoutine,BOOL Add);
__declspec(dllimport) BOOL __stdcall GenerateConsoleCtrlEvent(
DWORD dwCtrlEvent,DWORD dwProcessGroupId);
__declspec(dllimport) BOOL __stdcall AllocConsole( void );
__declspec(dllimport) BOOL __stdcall FreeConsole( void );
__declspec(dllimport) DWORD __stdcall GetConsoleTitleA(
LPSTR lpConsoleTitle,DWORD nSize);
__declspec(dllimport) DWORD __stdcall GetConsoleTitleW(
LPWSTR lpConsoleTitle,DWORD nSize);
	// 631 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
__declspec(dllimport) BOOL __stdcall SetConsoleTitleA(
LPCSTR lpConsoleTitle);
__declspec(dllimport) BOOL __stdcall SetConsoleTitleW(
LPCWSTR lpConsoleTitle);
	// 649 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
__declspec(dllimport) BOOL __stdcall ReadConsoleA(
HANDLE hConsoleInput,LPVOID lpBuffer,DWORD nNumberOfCharsToRead,LPDWORD lpNumberOfCharsRead,LPVOID lpReserved);
__declspec(dllimport) BOOL __stdcall ReadConsoleW(
HANDLE hConsoleInput,LPVOID lpBuffer,DWORD nNumberOfCharsToRead,LPDWORD lpNumberOfCharsRead,LPVOID lpReserved);
	// 675 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
__declspec(dllimport) BOOL __stdcall WriteConsoleA(
HANDLE hConsoleOutput,const void *lpBuffer,DWORD nNumberOfCharsToWrite,LPDWORD lpNumberOfCharsWritten,LPVOID lpReserved);
__declspec(dllimport) BOOL __stdcall WriteConsoleW(
HANDLE hConsoleOutput,const void *lpBuffer,DWORD nNumberOfCharsToWrite,LPDWORD lpNumberOfCharsWritten,LPVOID lpReserved);
	// 701 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
__declspec(dllimport) HANDLE __stdcall CreateConsoleScreenBuffer(
DWORD dwDesiredAccess,DWORD dwShareMode,const SECURITY_ATTRIBUTES *lpSecurityAttributes,DWORD dwFlags,LPVOID lpScreenBufferData);
__declspec(dllimport) UINT __stdcall GetConsoleCP( void );
__declspec(dllimport) BOOL __stdcall SetConsoleCP(
UINT wCodePageID);
__declspec(dllimport) UINT __stdcall GetConsoleOutputCP( void );
__declspec(dllimport) BOOL __stdcall SetConsoleOutputCP(
UINT wCodePageID);
}
	// 742 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
	// 744 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\wincon.h"
	// 176 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winver.h"
extern "C" {
	// 19 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winver.h"

	// 31 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winver.h"
typedef struct tagVS_FIXEDFILEINFO
{
DWORD dwSignature;
DWORD dwStrucVersion;
DWORD dwFileVersionMS;
DWORD dwFileVersionLS;
DWORD dwProductVersionMS;
DWORD dwProductVersionLS;
DWORD dwFileFlagsMask;
DWORD dwFileFlags;
DWORD dwFileOS;
DWORD dwFileType;
DWORD dwFileSubtype;
DWORD dwFileDateMS;
DWORD dwFileDateLS;
} VS_FIXEDFILEINFO;
DWORD __stdcall VerFindFileA(
DWORD uFlags,LPSTR szFileName,LPSTR szWinDir,LPSTR szAppDir,LPSTR szCurDir,PUINT lpuCurDirLen,LPSTR szDestDir,PUINT lpuDestDirLen);
DWORD __stdcall VerFindFileW(
DWORD uFlags,LPWSTR szFileName,LPWSTR szWinDir,LPWSTR szAppDir,LPWSTR szCurDir,PUINT lpuCurDirLen,LPWSTR szDestDir,PUINT lpuDestDirLen);
	// 176 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winver.h"
DWORD __stdcall VerInstallFileA(
DWORD uFlags,LPSTR szSrcFileName,LPSTR szDestFileName,LPSTR szSrcDir,LPSTR szDestDir,LPSTR szCurDir,LPSTR szTmpFile,PUINT lpuTmpFileLen);
DWORD __stdcall VerInstallFileW(
DWORD uFlags,LPWSTR szSrcFileName,LPWSTR szDestFileName,LPWSTR szSrcDir,LPWSTR szDestDir,LPWSTR szCurDir,LPWSTR szTmpFile,PUINT lpuTmpFileLen);
	// 206 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winver.h"
DWORD __stdcall GetFileVersionInfoSizeA(
LPSTR lptstrFilename,LPDWORD lpdwHandle);
DWORD __stdcall GetFileVersionInfoSizeW(
LPWSTR lptstrFilename,LPDWORD lpdwHandle);
	// 226 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winver.h"
BOOL __stdcall GetFileVersionInfoA(
LPSTR lptstrFilename,DWORD dwHandle,DWORD dwLen,LPVOID lpData);
BOOL __stdcall GetFileVersionInfoW(
LPWSTR lptstrFilename,DWORD dwHandle,DWORD dwLen,LPVOID lpData);
	// 250 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winver.h"
DWORD __stdcall VerLanguageNameA(
DWORD wLang,LPSTR szLang,DWORD nSize);
DWORD __stdcall VerLanguageNameW(
DWORD wLang,LPWSTR szLang,DWORD nSize);
	// 270 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winver.h"
BOOL __stdcall VerQueryValueA(
const LPVOID pBlock,LPSTR lpSubBlock,LPVOID* lplpBuffer,PUINT puLen);
BOOL __stdcall VerQueryValueW(
const LPVOID pBlock,LPWSTR lpSubBlock,LPVOID* lplpBuffer,PUINT puLen);
	// 292 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winver.h"
	// 294 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winver.h"
}
	// 298 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winver.h"
	// 300 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winver.h"
	// 177 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 178 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
extern "C" {
	// 27 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
typedef ACCESS_MASK REGSAM;
struct HKEY__ { int unused; }; typedef struct HKEY__ *HKEY;
typedef HKEY *PHKEY;
	// 45 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
struct val_context {
int valuelen;
LPVOID value_context;
LPVOID val_buff_ptr;
};
typedef struct val_context *PVALCONTEXT;
typedef struct pvalueA {
LPSTR pv_valuename;
int pv_valuelen;
LPVOID pv_value_context;
DWORD pv_type;
}PVALUEA, *PPVALUEA;
typedef struct pvalueW {
LPWSTR pv_valuename;
int pv_valuelen;
LPVOID pv_value_context;
DWORD pv_type;
}PVALUEW, *PPVALUEW;
typedef PVALUEA PVALUE;
typedef PPVALUEA PPVALUE;
	// 91 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
typedef
DWORD _cdecl
QUERYHANDLER (LPVOID keycontext, PVALCONTEXT val_list, DWORD num_vals,LPVOID outputbuffer, DWORD *total_outlen, DWORD input_blen);
typedef QUERYHANDLER *PQUERYHANDLER;
typedef struct provider_info {
PQUERYHANDLER pi_R0_1val;
PQUERYHANDLER pi_R0_allvals;
PQUERYHANDLER pi_R3_1val;
PQUERYHANDLER pi_R3_allvals;
DWORD pi_flags;
LPVOID pi_key_context;
}REG_PROVIDER;
typedef struct provider_info *PPROVIDER;
typedef struct value_entA {
LPSTR ve_valuename;
DWORD ve_valuelen;
DWORD ve_valueptr;
DWORD ve_type;
}VALENTA, *PVALENTA;
typedef struct value_entW {
LPWSTR ve_valuename;
DWORD ve_valuelen;
DWORD ve_valueptr;
DWORD ve_type;
}VALENTW, *PVALENTW;
typedef VALENTA VALENT;
typedef PVALENTA PVALENT;
	// 129 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
	// 131 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
	// 134 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegCloseKey (
HKEY hKey);
__declspec(dllimport) LONG __stdcall RegOverridePredefKey (
HKEY hKey,HKEY hNewHKey);
__declspec(dllimport) LONG __stdcall RegConnectRegistryA (
LPCSTR lpMachineName,HKEY hKey,PHKEY phkResult);
__declspec(dllimport) LONG __stdcall RegConnectRegistryW (
LPCWSTR lpMachineName,HKEY hKey,PHKEY phkResult);
	// 183 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegCreateKeyA (
HKEY hKey,LPCSTR lpSubKey,PHKEY phkResult);
__declspec(dllimport) LONG __stdcall RegCreateKeyW (
HKEY hKey,LPCWSTR lpSubKey,PHKEY phkResult);
	// 205 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegCreateKeyExA (
HKEY hKey,LPCSTR lpSubKey,DWORD Reserved,LPSTR lpClass,DWORD dwOptions,REGSAM samDesired,LPSECURITY_ATTRIBUTES lpSecurityAttributes,PHKEY phkResult,LPDWORD lpdwDisposition);
__declspec(dllimport) LONG __stdcall RegCreateKeyExW (
HKEY hKey,LPCWSTR lpSubKey,DWORD Reserved,LPWSTR lpClass,DWORD dwOptions,REGSAM samDesired,LPSECURITY_ATTRIBUTES lpSecurityAttributes,PHKEY phkResult,LPDWORD lpdwDisposition);
	// 239 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegDeleteKeyA (
HKEY hKey,LPCSTR lpSubKey);
__declspec(dllimport) LONG __stdcall RegDeleteKeyW (
HKEY hKey,LPCWSTR lpSubKey);
	// 259 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegDeleteValueA (
HKEY hKey,LPCSTR lpValueName);
__declspec(dllimport) LONG __stdcall RegDeleteValueW (
HKEY hKey,LPCWSTR lpValueName);
	// 279 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegEnumKeyA (
HKEY hKey,DWORD dwIndex,LPSTR lpName,DWORD cbName);
__declspec(dllimport) LONG __stdcall RegEnumKeyW (
HKEY hKey,DWORD dwIndex,LPWSTR lpName,DWORD cbName);
	// 303 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegEnumKeyExA (
HKEY hKey,DWORD dwIndex,LPSTR lpName,LPDWORD lpcbName,LPDWORD lpReserved,LPSTR lpClass,LPDWORD lpcbClass,PFILETIME lpftLastWriteTime);
__declspec(dllimport) LONG __stdcall RegEnumKeyExW (
HKEY hKey,DWORD dwIndex,LPWSTR lpName,LPDWORD lpcbName,LPDWORD lpReserved,LPWSTR lpClass,LPDWORD lpcbClass,PFILETIME lpftLastWriteTime);
	// 335 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegEnumValueA (
HKEY hKey,DWORD dwIndex,LPSTR lpValueName,LPDWORD lpcbValueName,LPDWORD lpReserved,LPDWORD lpType,LPBYTE lpData,LPDWORD lpcbData);
__declspec(dllimport) LONG __stdcall RegEnumValueW (
HKEY hKey,DWORD dwIndex,LPWSTR lpValueName,LPDWORD lpcbValueName,LPDWORD lpReserved,LPDWORD lpType,LPBYTE lpData,LPDWORD lpcbData);
	// 367 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegFlushKey (
HKEY hKey);
__declspec(dllimport) LONG __stdcall RegGetKeySecurity (
HKEY hKey,SECURITY_INFORMATION SecurityInformation,PSECURITY_DESCRIPTOR pSecurityDescriptor,LPDWORD lpcbSecurityDescriptor);
__declspec(dllimport) LONG __stdcall RegLoadKeyA (
HKEY hKey,LPCSTR lpSubKey,LPCSTR lpFile);
__declspec(dllimport) LONG __stdcall RegLoadKeyW (
HKEY hKey,LPCWSTR lpSubKey,LPCWSTR lpFile);
	// 406 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegNotifyChangeKeyValue (
HKEY hKey,BOOL bWatchSubtree,DWORD dwNotifyFilter,HANDLE hEvent,BOOL fAsynchronus);
__declspec(dllimport) LONG __stdcall RegOpenKeyA (
HKEY hKey,LPCSTR lpSubKey,PHKEY phkResult);
__declspec(dllimport) LONG __stdcall RegOpenKeyW (
HKEY hKey,LPCWSTR lpSubKey,PHKEY phkResult);
	// 439 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegOpenKeyExA (
HKEY hKey,LPCSTR lpSubKey,DWORD ulOptions,REGSAM samDesired,PHKEY phkResult);
__declspec(dllimport) LONG __stdcall RegOpenKeyExW (
HKEY hKey,LPCWSTR lpSubKey,DWORD ulOptions,REGSAM samDesired,PHKEY phkResult);
	// 465 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegQueryInfoKeyA (
HKEY hKey,LPSTR lpClass,LPDWORD lpcbClass,LPDWORD lpReserved,LPDWORD lpcSubKeys,LPDWORD lpcbMaxSubKeyLen,LPDWORD lpcbMaxClassLen,LPDWORD lpcValues,LPDWORD lpcbMaxValueNameLen,LPDWORD lpcbMaxValueLen,LPDWORD lpcbSecurityDescriptor,PFILETIME lpftLastWriteTime);
__declspec(dllimport) LONG __stdcall RegQueryInfoKeyW (
HKEY hKey,LPWSTR lpClass,LPDWORD lpcbClass,LPDWORD lpReserved,LPDWORD lpcSubKeys,LPDWORD lpcbMaxSubKeyLen,LPDWORD lpcbMaxClassLen,LPDWORD lpcValues,LPDWORD lpcbMaxValueNameLen,LPDWORD lpcbMaxValueLen,LPDWORD lpcbSecurityDescriptor,PFILETIME lpftLastWriteTime);
	// 505 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegQueryValueA (
HKEY hKey,LPCSTR lpSubKey,LPSTR lpValue,PLONG lpcbValue);
__declspec(dllimport) LONG __stdcall RegQueryValueW (
HKEY hKey,LPCWSTR lpSubKey,LPWSTR lpValue,PLONG lpcbValue);
	// 529 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegQueryMultipleValuesA (
HKEY hKey,PVALENTA val_list,DWORD num_vals,LPSTR lpValueBuf,LPDWORD ldwTotsize);
__declspec(dllimport) LONG __stdcall RegQueryMultipleValuesW (
HKEY hKey,PVALENTW val_list,DWORD num_vals,LPWSTR lpValueBuf,LPDWORD ldwTotsize);
	// 556 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
	// 557 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegQueryValueExA (
HKEY hKey,LPCSTR lpValueName,LPDWORD lpReserved,LPDWORD lpType,LPBYTE lpData,LPDWORD lpcbData);
__declspec(dllimport) LONG __stdcall RegQueryValueExW (
HKEY hKey,LPCWSTR lpValueName,LPDWORD lpReserved,LPDWORD lpType,LPBYTE lpData,LPDWORD lpcbData);
	// 585 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegReplaceKeyA (
HKEY hKey,LPCSTR lpSubKey,LPCSTR lpNewFile,LPCSTR lpOldFile);
__declspec(dllimport) LONG __stdcall RegReplaceKeyW (
HKEY hKey,LPCWSTR lpSubKey,LPCWSTR lpNewFile,LPCWSTR lpOldFile);
	// 609 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegRestoreKeyA (
HKEY hKey,LPCSTR lpFile,DWORD dwFlags);
__declspec(dllimport) LONG __stdcall RegRestoreKeyW (
HKEY hKey,LPCWSTR lpFile,DWORD dwFlags);
	// 631 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegSaveKeyA (
HKEY hKey,LPCSTR lpFile,LPSECURITY_ATTRIBUTES lpSecurityAttributes);
__declspec(dllimport) LONG __stdcall RegSaveKeyW (
HKEY hKey,LPCWSTR lpFile,LPSECURITY_ATTRIBUTES lpSecurityAttributes);
	// 653 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegSetKeySecurity (
HKEY hKey,SECURITY_INFORMATION SecurityInformation,PSECURITY_DESCRIPTOR pSecurityDescriptor);
__declspec(dllimport) LONG __stdcall RegSetValueA (
HKEY hKey,LPCSTR lpSubKey,DWORD dwType,LPCSTR lpData,DWORD cbData);
__declspec(dllimport) LONG __stdcall RegSetValueW (
HKEY hKey,LPCWSTR lpSubKey,DWORD dwType,LPCWSTR lpData,DWORD cbData);
	// 688 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegSetValueExA (
HKEY hKey,LPCSTR lpValueName,DWORD Reserved,DWORD dwType,const BYTE* lpData,DWORD cbData);
__declspec(dllimport) LONG __stdcall RegSetValueExW (
HKEY hKey,LPCWSTR lpValueName,DWORD Reserved,DWORD dwType,const BYTE* lpData,DWORD cbData);
	// 717 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) LONG __stdcall RegUnLoadKeyA (
HKEY hKey,LPCSTR lpSubKey);
__declspec(dllimport) LONG __stdcall RegUnLoadKeyW (
HKEY hKey,LPCWSTR lpSubKey);
	// 737 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) BOOL __stdcall InitiateSystemShutdownA(
LPSTR lpMachineName,LPSTR lpMessage,DWORD dwTimeout,BOOL bForceAppsClosed,BOOL bRebootAfterShutdown);
__declspec(dllimport) BOOL __stdcall InitiateSystemShutdownW(
LPWSTR lpMachineName,LPWSTR lpMessage,DWORD dwTimeout,BOOL bForceAppsClosed,BOOL bRebootAfterShutdown);
	// 767 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
__declspec(dllimport) BOOL __stdcall AbortSystemShutdownA(
LPSTR lpMachineName);
__declspec(dllimport) BOOL __stdcall AbortSystemShutdownW(
LPWSTR lpMachineName);
	// 786 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
}
	// 790 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
	// 793 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winreg.h"
	// 180 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 181 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
extern "C" {
	// 30 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 79 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 86 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 96 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 110 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 114 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
typedef struct _NETRESOURCEA {
DWORD dwScope;
DWORD dwType;
DWORD dwDisplayType;
DWORD dwUsage;
LPSTR lpLocalName;
LPSTR lpRemoteName;
LPSTR lpComment ;
LPSTR lpProvider;
}NETRESOURCEA, *LPNETRESOURCEA;
typedef struct _NETRESOURCEW {
DWORD dwScope;
DWORD dwType;
DWORD dwDisplayType;
DWORD dwUsage;
LPWSTR lpLocalName;
LPWSTR lpRemoteName;
LPWSTR lpComment ;
LPWSTR lpProvider;
}NETRESOURCEW, *LPNETRESOURCEW;
typedef NETRESOURCEA NETRESOURCE;
typedef LPNETRESOURCEA LPNETRESOURCE;
	// 142 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 164 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetAddConnectionA(
LPCSTR lpRemoteName,LPCSTR lpPassword,LPCSTR lpLocalName);
DWORD __stdcall
WNetAddConnectionW(
LPCWSTR lpRemoteName,LPCWSTR lpPassword,LPCWSTR lpLocalName);
	// 182 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetAddConnection2A(
LPNETRESOURCEA lpNetResource,LPCSTR lpPassword,LPCSTR lpUserName,DWORD dwFlags);
DWORD __stdcall
WNetAddConnection2W(
LPNETRESOURCEW lpNetResource,LPCWSTR lpPassword,LPCWSTR lpUserName,DWORD dwFlags);
	// 202 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetAddConnection3A(
HWND hwndOwner,LPNETRESOURCEA lpNetResource,LPCSTR lpPassword,LPCSTR lpUserName,DWORD dwFlags);
DWORD __stdcall
WNetAddConnection3W(
HWND hwndOwner,LPNETRESOURCEW lpNetResource,LPCWSTR lpPassword,LPCWSTR lpUserName,DWORD dwFlags);
	// 224 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetCancelConnectionA(
LPCSTR lpName,BOOL fForce);
DWORD __stdcall
WNetCancelConnectionW(
LPCWSTR lpName,BOOL fForce);
	// 240 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetCancelConnection2A(
LPCSTR lpName,DWORD dwFlags,BOOL fForce);
DWORD __stdcall
WNetCancelConnection2W(
LPCWSTR lpName,DWORD dwFlags,BOOL fForce);
	// 258 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetGetConnectionA(
LPCSTR lpLocalName,LPSTR lpRemoteName,LPDWORD lpnLength);
DWORD __stdcall
WNetGetConnectionW(
LPCWSTR lpLocalName,LPWSTR lpRemoteName,LPDWORD lpnLength);
	// 276 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetUseConnectionA(
HWND hwndOwner,LPNETRESOURCEA lpNetResource,LPCSTR lpUserID,LPCSTR lpPassword,DWORD dwFlags,LPSTR lpAccessName,LPDWORD lpBufferSize,LPDWORD lpResult);
DWORD __stdcall
WNetUseConnectionW(
HWND hwndOwner,LPNETRESOURCEW lpNetResource,LPCWSTR lpUserID,LPCWSTR lpPassword,DWORD dwFlags,LPWSTR lpAccessName,LPDWORD lpBufferSize,LPDWORD lpResult);
	// 306 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 307 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetConnectionDialog(
HWND hwnd,DWORD dwType);
DWORD __stdcall
WNetDisconnectDialog(
HWND hwnd,DWORD dwType);
typedef struct _CONNECTDLGSTRUCTA{
DWORD cbStructure;
HWND hwndOwner;
LPNETRESOURCEA lpConnRes;
DWORD dwFlags;
DWORD dwDevNum;
} CONNECTDLGSTRUCTA, *LPCONNECTDLGSTRUCTA;
typedef struct _CONNECTDLGSTRUCTW{
DWORD cbStructure;
HWND hwndOwner;
LPNETRESOURCEW lpConnRes;
DWORD dwFlags;
DWORD dwDevNum;
} CONNECTDLGSTRUCTW, *LPCONNECTDLGSTRUCTW;
typedef CONNECTDLGSTRUCTA CONNECTDLGSTRUCT;
typedef LPCONNECTDLGSTRUCTA LPCONNECTDLGSTRUCT;
	// 347 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetConnectionDialog1A(
LPCONNECTDLGSTRUCTA lpConnDlgStruct);
DWORD __stdcall
WNetConnectionDialog1W(
LPCONNECTDLGSTRUCTW lpConnDlgStruct);
	// 374 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
typedef struct _DISCDLGSTRUCTA{
DWORD cbStructure;
HWND hwndOwner;
LPSTR lpLocalName;
LPSTR lpRemoteName;
DWORD dwFlags;
} DISCDLGSTRUCTA, *LPDISCDLGSTRUCTA;
typedef struct _DISCDLGSTRUCTW{
DWORD cbStructure;
HWND hwndOwner;
LPWSTR lpLocalName;
LPWSTR lpRemoteName;
DWORD dwFlags;
} DISCDLGSTRUCTW, *LPDISCDLGSTRUCTW;
typedef DISCDLGSTRUCTA DISCDLGSTRUCT;
typedef LPDISCDLGSTRUCTA LPDISCDLGSTRUCT;
	// 396 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetDisconnectDialog1A(
LPDISCDLGSTRUCTA lpConnDlgStruct);
DWORD __stdcall
WNetDisconnectDialog1W(
LPDISCDLGSTRUCTW lpConnDlgStruct);
	// 413 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 414 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetOpenEnumA(
DWORD dwScope,DWORD dwType,DWORD dwUsage,LPNETRESOURCEA lpNetResource,LPHANDLE lphEnum);
DWORD __stdcall
WNetOpenEnumW(
DWORD dwScope,DWORD dwType,DWORD dwUsage,LPNETRESOURCEW lpNetResource,LPHANDLE lphEnum);
	// 440 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetEnumResourceA(
HANDLE hEnum,LPDWORD lpcCount,LPVOID lpBuffer,LPDWORD lpBufferSize);
DWORD __stdcall
WNetEnumResourceW(
HANDLE hEnum,LPDWORD lpcCount,LPVOID lpBuffer,LPDWORD lpBufferSize);
	// 460 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetCloseEnum(
HANDLE hEnum);
typedef struct _UNIVERSAL_NAME_INFOA {
LPSTR lpUniversalName;
}UNIVERSAL_NAME_INFOA, *LPUNIVERSAL_NAME_INFOA;
typedef struct _UNIVERSAL_NAME_INFOW {
LPWSTR lpUniversalName;
}UNIVERSAL_NAME_INFOW, *LPUNIVERSAL_NAME_INFOW;
typedef UNIVERSAL_NAME_INFOA UNIVERSAL_NAME_INFO;
typedef LPUNIVERSAL_NAME_INFOA LPUNIVERSAL_NAME_INFO;
	// 487 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
typedef struct _REMOTE_NAME_INFOA {
LPSTR lpUniversalName;
LPSTR lpConnectionName;
LPSTR lpRemainingPath;
}REMOTE_NAME_INFOA, *LPREMOTE_NAME_INFOA;
typedef struct _REMOTE_NAME_INFOW {
LPWSTR lpUniversalName;
LPWSTR lpConnectionName;
LPWSTR lpRemainingPath;
}REMOTE_NAME_INFOW, *LPREMOTE_NAME_INFOW;
typedef REMOTE_NAME_INFOA REMOTE_NAME_INFO;
typedef LPREMOTE_NAME_INFOA LPREMOTE_NAME_INFO;
	// 505 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetGetUniversalNameA(
LPCSTR lpLocalPath,DWORD dwInfoLevel,LPVOID lpBuffer,LPDWORD lpBufferSize);
DWORD __stdcall
WNetGetUniversalNameW(
LPCWSTR lpLocalPath,DWORD dwInfoLevel,LPVOID lpBuffer,LPDWORD lpBufferSize);
	// 525 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetGetUserA(
LPCSTR lpName,LPSTR lpUserName,LPDWORD lpnLength);
DWORD __stdcall
WNetGetUserW(
LPCWSTR lpName,LPWSTR lpUserName,LPDWORD lpnLength);
	// 547 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 559 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetGetProviderNameA(
DWORD dwNetType,LPSTR lpProviderName,LPDWORD lpBufferSize);
DWORD __stdcall
WNetGetProviderNameW(
DWORD dwNetType,LPWSTR lpProviderName,LPDWORD lpBufferSize);
	// 579 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
typedef struct _NETINFOSTRUCT{
DWORD cbStructure;
DWORD dwProviderVersion;
DWORD dwStatus;
DWORD dwCharacteristics;
DWORD dwHandle;
WORD wNetType;
DWORD dwPrinters;
DWORD dwDrives;
} NETINFOSTRUCT, *LPNETINFOSTRUCT;
DWORD __stdcall
WNetGetNetworkInformationA(
LPCSTR lpProvider,LPNETINFOSTRUCT lpNetInfoStruct);
DWORD __stdcall
WNetGetNetworkInformationW(
LPCWSTR lpProvider,LPNETINFOSTRUCT lpNetInfoStruct);
	// 610 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
typedef UINT ( __stdcall *PFNGETPROFILEPATHA) (
LPCSTR pszUsername,LPSTR pszBuffer,UINT cbBuffer);
typedef UINT ( __stdcall *PFNGETPROFILEPATHW) (
LPCWSTR pszUsername,LPWSTR pszBuffer,UINT cbBuffer);
	// 630 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
typedef UINT ( __stdcall *PFNRECONCILEPROFILEA) (
LPCSTR pszCentralFile,LPCSTR pszLocalFile,DWORD dwFlags);
typedef UINT ( __stdcall *PFNRECONCILEPROFILEW) (
LPCWSTR pszCentralFile,LPCWSTR pszLocalFile,DWORD dwFlags);
	// 646 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
typedef BOOL ( __stdcall *PFNPROCESSPOLICIESA) (
HWND hwnd,LPCSTR pszPath,LPCSTR pszUsername,LPCSTR pszComputerName,DWORD dwFlags);
typedef BOOL ( __stdcall *PFNPROCESSPOLICIESW) (
HWND hwnd,LPCWSTR pszPath,LPCWSTR pszUsername,LPCWSTR pszComputerName,DWORD dwFlags);
	// 674 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 677 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
DWORD __stdcall
WNetGetLastErrorA(
LPDWORD lpError,LPSTR lpErrorBuf,DWORD nErrorBufSize,LPSTR lpNameBuf,DWORD nNameBufSize);
DWORD __stdcall
WNetGetLastErrorW(
LPDWORD lpError,LPWSTR lpErrorBuf,DWORD nErrorBufSize,LPWSTR lpNameBuf,DWORD nNameBufSize);
	// 703 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 733 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 764 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
typedef struct _NETCONNECTINFOSTRUCT{
DWORD cbStructure;
DWORD dwFlags;
DWORD dwSpeed;
DWORD dwDelay;
DWORD dwOptDataSize;
} NETCONNECTINFOSTRUCT, *LPNETCONNECTINFOSTRUCT;
DWORD __stdcall
MultinetGetConnectionPerformanceA(
LPNETRESOURCEA lpNetResource,LPNETCONNECTINFOSTRUCT lpNetConnectInfoStruct);
DWORD __stdcall
MultinetGetConnectionPerformanceW(
LPNETRESOURCEW lpNetResource,LPNETCONNECTINFOSTRUCT lpNetConnectInfoStruct);
	// 798 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 799 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
}
	// 803 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 805 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\winnetwk.h"
	// 183 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 184 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 248 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 249 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
#pragma warning(default:4001)
#pragma warning(default:4201)
#pragma warning(default:4214)
	// 257 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 258 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 260 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 262 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 263 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\windows.h"
	// 13 "C:\\Sylvain\\Developpement C\\SrcC\\SPG_Win98\\SPG_Win98Full.cpp"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack1.h"
#pragma warning(disable:4103)
#pragma pack(push)
	// 28 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack1.h"
#pragma pack(1)
	// 32 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack1.h"
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\pshpack1.h"
	// 13 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
extern "C" {
	// 17 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 26 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 34 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 35 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
typedef UINT (__stdcall *LPOFNHOOKPROC) (HWND, UINT, WPARAM, LPARAM);
	// 55 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
typedef struct tagOFNA {
DWORD lStructSize;
HWND hwndOwner;
HINSTANCE hInstance;
LPCSTR lpstrFilter;
LPSTR lpstrCustomFilter;
DWORD nMaxCustFilter;
DWORD nFilterIndex;
LPSTR lpstrFile;
DWORD nMaxFile;
LPSTR lpstrFileTitle;
DWORD nMaxFileTitle;
LPCSTR lpstrInitialDir;
LPCSTR lpstrTitle;
DWORD Flags;
WORD nFileOffset;
WORD nFileExtension;
LPCSTR lpstrDefExt;
LPARAM lCustData;
LPOFNHOOKPROC lpfnHook;
LPCSTR lpTemplateName;
} OPENFILENAMEA, *LPOPENFILENAMEA;
typedef struct tagOFNW {
DWORD lStructSize;
HWND hwndOwner;
HINSTANCE hInstance;
LPCWSTR lpstrFilter;
LPWSTR lpstrCustomFilter;
DWORD nMaxCustFilter;
DWORD nFilterIndex;
LPWSTR lpstrFile;
DWORD nMaxFile;
LPWSTR lpstrFileTitle;
DWORD nMaxFileTitle;
LPCWSTR lpstrInitialDir;
LPCWSTR lpstrTitle;
DWORD Flags;
WORD nFileOffset;
WORD nFileExtension;
LPCWSTR lpstrDefExt;
LPARAM lCustData;
LPOFNHOOKPROC lpfnHook;
LPCWSTR lpTemplateName;
} OPENFILENAMEW, *LPOPENFILENAMEW;
typedef OPENFILENAMEA OPENFILENAME;
typedef LPOPENFILENAMEA LPOPENFILENAME;
	// 115 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
BOOL __stdcall GetOpenFileNameA(LPOPENFILENAMEA);
BOOL __stdcall GetOpenFileNameW(LPOPENFILENAMEW);
	// 123 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
BOOL __stdcall GetSaveFileNameA(LPOPENFILENAMEA);
BOOL __stdcall GetSaveFileNameW(LPOPENFILENAMEW);
	// 130 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
short __stdcall GetFileTitleA(LPCSTR, LPSTR, WORD);
short __stdcall GetFileTitleW(LPCWSTR, LPWSTR, WORD);
	// 137 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 164 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
typedef UINT (__stdcall *LPCCHOOKPROC) (HWND, UINT, WPARAM, LPARAM);
	// 184 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
typedef struct _OFNOTIFYA
{
NMHDR hdr;
LPOPENFILENAMEA lpOFN;
LPSTR pszFile;
} OFNOTIFYA, *LPOFNOTIFYA;
typedef struct _OFNOTIFYW
{
NMHDR hdr;
LPOPENFILENAMEW lpOFN;
LPWSTR pszFile;
} OFNOTIFYW, *LPOFNOTIFYW;
typedef OFNOTIFYA OFNOTIFY;
typedef LPOFNOTIFYA LPOFNOTIFY;
	// 207 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
typedef struct _OFNOTIFYEXA
{
NMHDR hdr;
LPOPENFILENAMEA lpOFN;
LPVOID psf;
LPVOID pidl;
} OFNOTIFYEXA, *LPOFNOTIFYEXA;
typedef struct _OFNOTIFYEXW
{
NMHDR hdr;
LPOPENFILENAMEW lpOFN;
LPVOID psf;
LPVOID pidl;
} OFNOTIFYEXW, *LPOFNOTIFYEXW;
typedef OFNOTIFYEXA OFNOTIFYEX;
typedef LPOFNOTIFYEXA LPOFNOTIFYEX;
	// 232 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 265 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 279 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 293 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 322 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
typedef struct tagCHOOSECOLORA {
DWORD lStructSize;
HWND hwndOwner;
HWND hInstance;
COLORREF rgbResult;
COLORREF* lpCustColors;
DWORD Flags;
LPARAM lCustData;
LPCCHOOKPROC lpfnHook;
LPCSTR lpTemplateName;
} CHOOSECOLORA, *LPCHOOSECOLORA;
typedef struct tagCHOOSECOLORW {
DWORD lStructSize;
HWND hwndOwner;
HWND hInstance;
COLORREF rgbResult;
COLORREF* lpCustColors;
DWORD Flags;
LPARAM lCustData;
LPCCHOOKPROC lpfnHook;
LPCWSTR lpTemplateName;
} CHOOSECOLORW, *LPCHOOSECOLORW;
typedef CHOOSECOLORA CHOOSECOLOR;
typedef LPCHOOSECOLORA LPCHOOSECOLOR;
	// 353 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 386 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
BOOL __stdcall ChooseColorA(LPCHOOSECOLORA);
BOOL __stdcall ChooseColorW(LPCHOOSECOLORW);
	// 394 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 406 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
typedef UINT (__stdcall *LPFRHOOKPROC) (HWND, UINT, WPARAM, LPARAM);
	// 412 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
typedef struct tagFINDREPLACEA {
DWORD lStructSize;
HWND hwndOwner;
HINSTANCE hInstance;
DWORD Flags;
LPSTR lpstrFindWhat;
LPSTR lpstrReplaceWith;
WORD wFindWhatLen;
WORD wReplaceWithLen;
LPARAM lCustData;
LPFRHOOKPROC lpfnHook;
LPCSTR lpTemplateName;
} FINDREPLACEA, *LPFINDREPLACEA;
typedef struct tagFINDREPLACEW {
DWORD lStructSize;
HWND hwndOwner;
HINSTANCE hInstance;
DWORD Flags;
LPWSTR lpstrFindWhat;
LPWSTR lpstrReplaceWith;
WORD wFindWhatLen;
WORD wReplaceWithLen;
LPARAM lCustData;
LPFRHOOKPROC lpfnHook;
LPCWSTR lpTemplateName;
} FINDREPLACEW, *LPFINDREPLACEW;
typedef FINDREPLACEA FINDREPLACE;
typedef LPFINDREPLACEA LPFINDREPLACE;
	// 448 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
HWND __stdcall FindTextA(LPFINDREPLACEA);
HWND __stdcall FindTextW(LPFINDREPLACEW);
	// 474 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
HWND __stdcall ReplaceTextA(LPFINDREPLACEA);
HWND __stdcall ReplaceTextW(LPFINDREPLACEW);
	// 482 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 491 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
typedef UINT (__stdcall *LPCFHOOKPROC) (HWND, UINT, WPARAM, LPARAM);
	// 497 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
typedef struct tagCHOOSEFONTA {
DWORD lStructSize;
HWND hwndOwner;
HDC hDC;
LPLOGFONTA lpLogFont;
INT iPointSize;
DWORD Flags;
COLORREF rgbColors;
LPARAM lCustData;
LPCFHOOKPROC lpfnHook;
LPCSTR lpTemplateName; 
HINSTANCE hInstance;
LPSTR lpszStyle;
WORD nFontType;
WORD ___MISSING_ALIGNMENT__;
INT nSizeMin;
INT nSizeMax;
} CHOOSEFONTA, *LPCHOOSEFONTA;
typedef struct tagCHOOSEFONTW {
DWORD lStructSize;
HWND hwndOwner;
HDC hDC;
LPLOGFONTW lpLogFont;
INT iPointSize;
DWORD Flags;
COLORREF rgbColors;
LPARAM lCustData;
LPCFHOOKPROC lpfnHook;
LPCWSTR lpTemplateName;
HINSTANCE hInstance;
LPWSTR lpszStyle;
WORD nFontType;
WORD ___MISSING_ALIGNMENT__;
INT nSizeMin;
INT nSizeMax;
} CHOOSEFONTW, *LPCHOOSEFONTW;
typedef CHOOSEFONTA CHOOSEFONT;
typedef LPCHOOSEFONTA LPCHOOSEFONT;
	// 551 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
BOOL __stdcall ChooseFontA(LPCHOOSEFONTA);
BOOL __stdcall ChooseFontW(LPCHOOSEFONTW);
	// 559 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 575 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 592 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 655 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
typedef UINT (__stdcall *LPPRINTHOOKPROC) (HWND, UINT, WPARAM, LPARAM);
typedef UINT (__stdcall *LPSETUPHOOKPROC) (HWND, UINT, WPARAM, LPARAM);
	// 669 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
typedef struct tagPDA {
DWORD lStructSize;
HWND hwndOwner;
HGLOBAL hDevMode;
HGLOBAL hDevNames;
HDC hDC;
DWORD Flags;
WORD nFromPage;
WORD nToPage;
WORD nMinPage;
WORD nMaxPage;
WORD nCopies;
HINSTANCE hInstance;
LPARAM lCustData;
LPPRINTHOOKPROC lpfnPrintHook;
LPSETUPHOOKPROC lpfnSetupHook;
LPCSTR lpPrintTemplateName;
LPCSTR lpSetupTemplateName;
HGLOBAL hPrintTemplate;
HGLOBAL hSetupTemplate;
} PRINTDLGA, *LPPRINTDLGA;
typedef struct tagPDW {
DWORD lStructSize;
HWND hwndOwner;
HGLOBAL hDevMode;
HGLOBAL hDevNames;
HDC hDC;
DWORD Flags;
WORD nFromPage;
WORD nToPage;
WORD nMinPage;
WORD nMaxPage;
WORD nCopies;
HINSTANCE hInstance;
LPARAM lCustData;
LPPRINTHOOKPROC lpfnPrintHook;
LPSETUPHOOKPROC lpfnSetupHook;
LPCWSTR lpPrintTemplateName;
LPCWSTR lpSetupTemplateName;
HGLOBAL hPrintTemplate;
HGLOBAL hSetupTemplate;
} PRINTDLGW, *LPPRINTDLGW;
typedef PRINTDLGA PRINTDLG;
typedef LPPRINTDLGA LPPRINTDLG;
	// 719 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
BOOL __stdcall PrintDlgA(LPPRINTDLGA);
BOOL __stdcall PrintDlgW(LPPRINTDLGW);
	// 727 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
typedef struct tagDEVNAMES {
WORD wDriverOffset;
WORD wDeviceOffset;
WORD wOutputOffset;
WORD wDefault;
} DEVNAMES;
typedef DEVNAMES* LPDEVNAMES;
DWORD __stdcall CommDlgExtendedError(void);
typedef UINT (__stdcall* LPPAGEPAINTHOOK)( HWND, UINT, WPARAM, LPARAM );
typedef UINT (__stdcall* LPPAGESETUPHOOK)( HWND, UINT, WPARAM, LPARAM );
typedef struct tagPSDA
{
DWORD lStructSize;
HWND hwndOwner;
HGLOBAL hDevMode;
HGLOBAL hDevNames;
DWORD Flags;
POINT ptPaperSize;
RECT rtMinMargin;
RECT rtMargin;
HINSTANCE hInstance;
LPARAM lCustData;
LPPAGESETUPHOOK lpfnPageSetupHook;
LPPAGEPAINTHOOK lpfnPagePaintHook;
LPCSTR lpPageSetupTemplateName;
HGLOBAL hPageSetupTemplate;
} PAGESETUPDLGA,* LPPAGESETUPDLGA;
typedef struct tagPSDW
{
DWORD lStructSize;
HWND hwndOwner;
HGLOBAL hDevMode;
HGLOBAL hDevNames;
DWORD Flags;
POINT ptPaperSize;
RECT rtMinMargin;
RECT rtMargin;
HINSTANCE hInstance;
LPARAM lCustData;
LPPAGESETUPHOOK lpfnPageSetupHook;
LPPAGEPAINTHOOK lpfnPagePaintHook;
LPCWSTR lpPageSetupTemplateName;
HGLOBAL hPageSetupTemplate;
} PAGESETUPDLGW,* LPPAGESETUPDLGW;
typedef PAGESETUPDLGA PAGESETUPDLG;
typedef LPPAGESETUPDLGA LPPAGESETUPDLG;
	// 820 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
BOOL __stdcall PageSetupDlgA( LPPAGESETUPDLGA );
BOOL __stdcall PageSetupDlgW( LPPAGESETUPDLGW );
	// 828 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 850 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
}
	// 854 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 1 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
#pragma warning(disable:4103)
#pragma pack(pop)
	// 33 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 36 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 37 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\poppack.h"
	// 856 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 857 "C:\\Program Files\\Microsoft Visual Studio\\VC98\\INCLUDE\\commdlg.h"
	// 14 "C:\\Sylvain\\Developpement C\\SrcC\\SPG_Win98\\SPG_Win98Full.cpp"
//#pragma message("SPG_Win98Full_Stop")
