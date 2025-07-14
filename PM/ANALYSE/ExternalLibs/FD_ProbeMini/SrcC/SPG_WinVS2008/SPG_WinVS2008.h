
#pragma once
#pragma once
#pragma warning(disable:4514)
#pragma warning(disable:4103)
#pragma warning(push)
#pragma warning(disable:4001)
#pragma warning(disable:4201)
#pragma warning(disable:4214)
#pragma once
#pragma pack(push,8)
extern "C" {
typedef enum _EXCEPTION_DISPOSITION {
 ExceptionContinueExecution,
 ExceptionContinueSearch,
 ExceptionNestedException,
 ExceptionCollidedUnwind
} EXCEPTION_DISPOSITION;
struct _EXCEPTION_RECORD;
struct _CONTEXT;
EXCEPTION_DISPOSITION __cdecl _except_handler (
 [SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] struct _EXCEPTION_RECORD *_ExceptionRecord,
 [SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] void * _EstablisherFrame,
 [SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Post(Deref=1,Valid=SA_Yes)] struct _CONTEXT *_ContextRecord,
 [SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Post(Deref=1,Valid=SA_Yes)] void * _DispatcherContext
 );
unsigned long __cdecl _exception_code(void);
void * __cdecl _exception_info(void);
int __cdecl _abnormal_termination(void);
}
#pragma pack(pop)
#pragma once
#pragma once
extern "C" {
typedef unsigned long ULONG;
typedef ULONG *PULONG;
typedef unsigned short USHORT;
typedef USHORT *PUSHORT;
typedef unsigned char UCHAR;
typedef UCHAR *PUCHAR;
typedef char *PSZ;
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
extern "C" {
#pragma once
extern "C" {
 const unsigned short * __cdecl __pctype_func(void);
 extern const unsigned short *_pctype;
 extern const unsigned short _wctype[];
 const wctype_t * __cdecl __pwctype_func(void);
 extern const wctype_t *_pwctype;
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _isctype([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _Type);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _isctype_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _Type, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl isalpha([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _isalpha_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl isupper([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _isupper_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl islower([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _islower_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl isdigit([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _isdigit_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl isxdigit([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _isxdigit_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl isspace([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _isspace_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl ispunct([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _ispunct_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl isalnum([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _isalnum_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl isprint([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _isprint_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl isgraph([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _isgraph_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl iscntrl([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iscntrl_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl toupper([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl tolower([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _tolower([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _tolower_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _toupper([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _toupper_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl __isascii([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl __toascii([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl __iscsymf([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl __iscsym([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl iswalpha([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iswalpha_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl iswupper([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iswupper_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl iswlower([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iswlower_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl iswdigit([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iswdigit_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl iswxdigit([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iswxdigit_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl iswspace([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iswspace_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl iswpunct([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iswpunct_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl iswalnum([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iswalnum_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl iswprint([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iswprint_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl iswgraph([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iswgraph_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl iswcntrl([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iswcntrl_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl iswascii([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl isleadbyte([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _isleadbyte_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] int _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] wint_t __cdecl towupper([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] wint_t __cdecl _towupper_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] wint_t __cdecl towlower([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] wint_t __cdecl _towlower_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl iswctype([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wctype_t _Type);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iswctype_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wctype_t _Type, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl __iswcsymf([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iswcsymf_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl __iswcsym([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C);
[returnvalue:SA_Post(MustCheck=SA_Yes)] int __cdecl _iswcsym_l([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_Maybe)] [SA_Pre(Deref=1,Valid=SA_Yes)] [SA_Pre(Deref=1,Access=SA_Read)] _locale_t _Locale);
__declspec(deprecated("This function or variable has been superceded by newer library or operating system functionality. Consider using " "iswctype" " instead. See online help for details.")) int __cdecl is_wctype([SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wint_t _C, [SA_Pre(Null=SA_No)] [SA_Pre(Deref=1,Valid=SA_Yes,Access=SA_Read)] wctype_t _Type);
}
#pragma once
extern "C" {
}
#pragma once
typedef char* ValidCompNameA;
typedef unsigned short* ValidCompNameW;
typedef const unsigned short* ConstValidCompNameW;
typedef char* SAL_ValidCompNameT;
typedef const char* SAL_ConstValidCompNameT;
#pragma once
 typedef unsigned long POINTER_64_INT;
#pragma once
extern "C" {
typedef signed char INT8, *PINT8;
typedef signed short INT16, *PINT16;
typedef signed int INT32, *PINT32;
typedef signed __int64 INT64, *PINT64;
typedef unsigned char UINT8, *PUINT8;
typedef unsigned short UINT16, *PUINT16;
typedef unsigned int UINT32, *PUINT32;
typedef unsigned __int64 UINT64, *PUINT64;
typedef signed int LONG32, *PLONG32;
typedef unsigned int ULONG32, *PULONG32;
typedef unsigned int DWORD32, *PDWORD32;
 typedef __w64 int INT_PTR, *PINT_PTR;
 typedef __w64 unsigned int UINT_PTR, *PUINT_PTR;
 typedef __w64 long LONG_PTR, *PLONG_PTR;
 typedef __w64 unsigned long ULONG_PTR, *PULONG_PTR;
typedef unsigned short UHALF_PTR, *PUHALF_PTR;
typedef short HALF_PTR, *PHALF_PTR;
typedef __w64 long SHANDLE_PTR;
typedef __w64 unsigned long HANDLE_PTR;
__inline
void * __ptr64
PtrToPtr64(
 const void *p
 )
{
 return((void * __ptr64) (unsigned __int64) (ULONG_PTR)p );
}
__inline
void *
Ptr64ToPtr(
 const void * __ptr64 p
 )
{
 return((void *) (ULONG_PTR) (unsigned __int64) p);
}
__inline
void * __ptr64
HandleToHandle64(
 const void *h
 )
{
 return((void * __ptr64)(__int64)(LONG_PTR)h );
}
__inline
void *
Handle64ToHandle(
 const void * __ptr64 h
 )
{
 return((void *) (ULONG_PTR) (unsigned __int64) h );
}
typedef ULONG_PTR SIZE_T, *PSIZE_T;
typedef LONG_PTR SSIZE_T, *PSSIZE_T;
typedef ULONG_PTR DWORD_PTR, *PDWORD_PTR;
typedef __int64 LONG64, *PLONG64;
typedef unsigned __int64 ULONG64, *PULONG64;
typedef unsigned __int64 DWORD64, *PDWORD64;
typedef ULONG_PTR KAFFINITY;
typedef KAFFINITY *PKAFFINITY;
}
typedef void *PVOID;
typedef void * __ptr64 PVOID64;
typedef char CHAR;
typedef short SHORT;
typedef long LONG;
typedef int INT;
typedef wchar_t WCHAR;
typedef WCHAR *PWCHAR, *LPWCH, *PWCH;
typedef const WCHAR *LPCWCH, *PCWCH;
typedef WCHAR *NWPSTR, *LPWSTR, *PWSTR;
typedef PWSTR *PZPWSTR;
typedef const PWSTR *PCZPWSTR;
typedef WCHAR *LPUWSTR, *PUWSTR;
typedef const WCHAR *LPCWSTR, *PCWSTR;
typedef PCWSTR *PZPCWSTR;
typedef const WCHAR *LPCUWSTR, *PCUWSTR;
typedef const WCHAR *LPCWCHAR, *PCWCHAR;
typedef const WCHAR *LPCUWCHAR, *PCUWCHAR;
typedef unsigned long UCSCHAR;
typedef UCSCHAR *PUCSCHAR;
typedef const UCSCHAR *PCUCSCHAR;
typedef UCSCHAR *PUCSSTR;
typedef UCSCHAR *PUUCSSTR;
typedef const UCSCHAR *PCUCSSTR;
typedef const UCSCHAR *PCUUCSSTR;
typedef UCSCHAR *PUUCSCHAR;
typedef const UCSCHAR *PCUUCSCHAR;
typedef CHAR *PCHAR, *LPCH, *PCH;
typedef const CHAR *LPCCH, *PCCH;
typedef CHAR *NPSTR, *LPSTR, *PSTR;
typedef PSTR *PZPSTR;
typedef const PSTR *PCZPSTR;
typedef const CHAR *LPCSTR, *PCSTR;
typedef PCSTR *PZPCSTR;
typedef char TCHAR, *PTCHAR;
typedef unsigned char TBYTE , *PTBYTE ;
typedef LPCH LPTCH, PTCH;
typedef LPSTR PTSTR, LPTSTR, PUTSTR, LPUTSTR;
typedef LPCSTR PCTSTR, LPCTSTR, PCUTSTR, LPCUTSTR;
typedef SHORT *PSHORT;
typedef LONG *PLONG;
typedef void *HANDLE;
typedef HANDLE *PHANDLE;
typedef BYTE FCHAR;
typedef WORD FSHORT;
typedef DWORD FLONG;
typedef long HRESULT;
typedef char CCHAR;
typedef DWORD LCID;
typedef PDWORD PLCID;
typedef WORD LANGID;
typedef struct _FLOAT128 {
 __int64 LowPart;
 __int64 HighPart;
} FLOAT128;
typedef FLOAT128 *PFLOAT128;
typedef __int64 LONGLONG;
typedef unsigned __int64 ULONGLONG;
typedef LONGLONG *PLONGLONG;
typedef ULONGLONG *PULONGLONG;
typedef LONGLONG USN;
typedef union _LARGE_INTEGER {
 struct {
 DWORD LowPart;
 LONG HighPart;
 };
 struct {
 DWORD LowPart;
 LONG HighPart;
 } u;
 LONGLONG QuadPart;
} LARGE_INTEGER;
typedef LARGE_INTEGER *PLARGE_INTEGER;
typedef union _ULARGE_INTEGER {
 struct {
 DWORD LowPart;
 DWORD HighPart;
 };
 struct {
 DWORD LowPart;
 DWORD HighPart;
 } u;
 ULONGLONG QuadPart;
} ULARGE_INTEGER;
typedef ULARGE_INTEGER *PULARGE_INTEGER;
typedef struct _LUID {
 DWORD LowPart;
 LONG HighPart;
} LUID, *PLUID;
typedef ULONGLONG DWORDLONG;
typedef DWORDLONG *PDWORDLONG;
ULONGLONG
__stdcall
Int64ShllMod32 (
 ULONGLONG Value,
 DWORD ShiftCount
 );
LONGLONG
__stdcall
Int64ShraMod32 (
 LONGLONG Value,
 DWORD ShiftCount
 );
ULONGLONG
__stdcall
Int64ShrlMod32 (
 ULONGLONG Value,
 DWORD ShiftCount
 );
#pragma warning(push)
#pragma warning(disable:4035 4793)
__inline ULONGLONG
__stdcall
Int64ShllMod32 (
 ULONGLONG Value,
 DWORD ShiftCount
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
__inline LONGLONG
__stdcall
Int64ShraMod32 (
 LONGLONG Value,
 DWORD ShiftCount
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
__inline ULONGLONG
__stdcall
Int64ShrlMod32 (
 ULONGLONG Value,
 DWORD ShiftCount
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
#pragma warning(pop)
extern "C" {
unsigned int
__cdecl
_rotl (
 unsigned int Value,
 int Shift
 );
unsigned __int64
__cdecl
_rotl64 (
 unsigned __int64 Value,
 int Shift
 );
unsigned int
__cdecl
_rotr (
 unsigned int Value,
 int Shift
 );
unsigned __int64
__cdecl
_rotr64 (
 unsigned __int64 Value,
 int Shift
 );
#pragma intrinsic(_rotl)
#pragma intrinsic(_rotl64)
#pragma intrinsic(_rotr)
#pragma intrinsic(_rotr64)
}
typedef BYTE BOOLEAN;
typedef BOOLEAN *PBOOLEAN;
typedef struct _LIST_ENTRY {
 struct _LIST_ENTRY *Flink;
 struct _LIST_ENTRY *Blink;
} LIST_ENTRY, *PLIST_ENTRY, * PRLIST_ENTRY;
typedef struct _SINGLE_LIST_ENTRY {
 struct _SINGLE_LIST_ENTRY *Next;
} SINGLE_LIST_ENTRY, *PSINGLE_LIST_ENTRY;
typedef struct LIST_ENTRY32 {
 DWORD Flink;
 DWORD Blink;
} LIST_ENTRY32;
typedef LIST_ENTRY32 *PLIST_ENTRY32;
typedef struct LIST_ENTRY64 {
 ULONGLONG Flink;
 ULONGLONG Blink;
} LIST_ENTRY64;
typedef LIST_ENTRY64 *PLIST_ENTRY64;
typedef struct _GUID {
 unsigned long Data1;
 unsigned short Data2;
 unsigned short Data3;
 unsigned char Data4[ 8 ];
} GUID;
typedef GUID *LPGUID;
typedef const GUID *LPCGUID;
typedef GUID IID;
typedef IID *LPIID;
typedef GUID CLSID;
typedef CLSID *LPCLSID;
typedef GUID FMTID;
typedef FMTID *LPFMTID;
__inline int InlineIsEqualGUID(const GUID & rguid1, const GUID & rguid2)
{
 return (
 ((unsigned long *) &rguid1)[0] == ((unsigned long *) &rguid2)[0] &&
 ((unsigned long *) &rguid1)[1] == ((unsigned long *) &rguid2)[1] &&
 ((unsigned long *) &rguid1)[2] == ((unsigned long *) &rguid2)[2] &&
 ((unsigned long *) &rguid1)[3] == ((unsigned long *) &rguid2)[3]);
}
__inline int IsEqualGUID(const GUID & rguid1, const GUID & rguid2)
{
 return !memcmp(&rguid1, &rguid2, sizeof(GUID));
}
__inline int operator==(const GUID & guidOne, const GUID & guidOther)
{
 return IsEqualGUID(guidOne,guidOther);
}
__inline int operator!=(const GUID & guidOne, const GUID & guidOther)
{
 return !(guidOne == guidOther);
}
typedef struct _OBJECTID {
 GUID Lineage;
 DWORD Uniquifier;
} OBJECTID;
extern "C++"
template <typename T, size_t N>
char (*RtlpNumberOf( T (&)[N] ))[N];
typedef ULONG_PTR KSPIN_LOCK;
typedef KSPIN_LOCK *PKSPIN_LOCK;
#pragma warning(push)
#pragma warning(disable:4164)
#pragma function(_enable)
#pragma function(_disable)
#pragma warning(pop)
extern "C" {
BOOLEAN
_bittest (
 LONG const *Base,
 LONG Offset
 );
BOOLEAN
_bittestandcomplement (
 LONG *Base,
 LONG Offset
 );
BOOLEAN
_bittestandset (
 LONG *Base,
 LONG Offset
 );
BOOLEAN
_bittestandreset (
 LONG *Base,
 LONG Offset
 );
BOOLEAN
_interlockedbittestandset (
 LONG volatile *Base,
 LONG Offset
 );
BOOLEAN
_interlockedbittestandreset (
 LONG volatile *Base,
 LONG Offset
 );
#pragma intrinsic(_bittest)
#pragma intrinsic(_bittestandcomplement)
#pragma intrinsic(_bittestandset)
#pragma intrinsic(_bittestandreset)
#pragma intrinsic(_interlockedbittestandset)
#pragma intrinsic(_interlockedbittestandreset)
BOOLEAN
_BitScanForward (
 DWORD *Index,
 DWORD Mask
 );
BOOLEAN
_BitScanReverse (
 DWORD *Index,
 DWORD Mask
 );
#pragma intrinsic(_BitScanForward)
#pragma intrinsic(_BitScanReverse)
SHORT
_InterlockedCompareExchange16 (
 SHORT volatile *Destination,
 SHORT ExChange,
 SHORT Comperand
 );
#pragma intrinsic(_InterlockedCompareExchange16)
#pragma warning(push)
#pragma warning(disable:4035 4793)
__forceinline
BOOLEAN
InterlockedBitTestAndComplement (
 LONG volatile *Base,
 LONG Bit
 )
{
 __asm {
 mov eax, Bit
 mov ecx, Base
 lock btc [ecx], eax
 setc al
 };
}
#pragma warning(pop)
BYTE
__readfsbyte (
 DWORD Offset
 );
WORD
__readfsword (
 DWORD Offset
 );
DWORD
__readfsdword (
 DWORD Offset
 );
void
__writefsbyte (
 DWORD Offset,
 BYTE Data
 );
void
__writefsword (
 DWORD Offset,
 WORD Data
 );
void
__writefsdword (
 DWORD Offset,
 DWORD Data
 );
#pragma intrinsic(__readfsbyte)
#pragma intrinsic(__readfsword)
#pragma intrinsic(__readfsdword)
#pragma intrinsic(__writefsbyte)
#pragma intrinsic(__writefsword)
#pragma intrinsic(__writefsdword)
void
__incfsbyte (
 DWORD Offset
 );
void
__addfsbyte (
 DWORD Offset,
 BYTE Value
 );
void
__incfsword (
 DWORD Offset
 );
void
__addfsword (
 DWORD Offset,
 WORD Value
 );
void
__incfsdword (
 DWORD Offset
 );
void
__addfsdword (
 DWORD Offset,
 DWORD Value
 );
void
_mm_pause (
 void
 );
#pragma intrinsic(_mm_pause)
}
#pragma warning( push )
#pragma warning( disable : 4793 )
__forceinline
void
MemoryBarrier (
 void
 )
{
 LONG Barrier;
 __asm {
 xchg Barrier, eax
 }
}
#pragma warning( pop )
DWORD64
__readpmc (
 DWORD Counter
 );
#pragma intrinsic(__readpmc)
DWORD64
__rdtsc (
 void
 );
#pragma intrinsic(__rdtsc)
void
__int2c (
 void
 );
#pragma intrinsic(__int2c)
__inline PVOID GetFiberData( void ) { return *(PVOID *) (ULONG_PTR) __readfsdword (0x10);}
__inline PVOID GetCurrentFiber( void ) { return (PVOID) (ULONG_PTR) __readfsdword (0x10);}
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
typedef struct _WOW64_FLOATING_SAVE_AREA {
 DWORD ControlWord;
 DWORD StatusWord;
 DWORD TagWord;
 DWORD ErrorOffset;
 DWORD ErrorSelector;
 DWORD DataOffset;
 DWORD DataSelector;
 BYTE RegisterArea[80];
 DWORD Cr0NpxState;
} WOW64_FLOATING_SAVE_AREA;
typedef WOW64_FLOATING_SAVE_AREA *PWOW64_FLOATING_SAVE_AREA;
typedef struct _WOW64_CONTEXT {
 DWORD ContextFlags;
 DWORD Dr0;
 DWORD Dr1;
 DWORD Dr2;
 DWORD Dr3;
 DWORD Dr6;
 DWORD Dr7;
 WOW64_FLOATING_SAVE_AREA FloatSave;
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
} WOW64_CONTEXT;
typedef WOW64_CONTEXT *PWOW64_CONTEXT;
typedef struct _EXCEPTION_RECORD {
 DWORD ExceptionCode;
 DWORD ExceptionFlags;
 struct _EXCEPTION_RECORD *ExceptionRecord;
 PVOID ExceptionAddress;
 DWORD NumberParameters;
 ULONG_PTR ExceptionInformation[15];
 } EXCEPTION_RECORD;
typedef EXCEPTION_RECORD *PEXCEPTION_RECORD;
typedef struct _EXCEPTION_RECORD32 {
 DWORD ExceptionCode;
 DWORD ExceptionFlags;
 DWORD ExceptionRecord;
 DWORD ExceptionAddress;
 DWORD NumberParameters;
 DWORD ExceptionInformation[15];
} EXCEPTION_RECORD32, *PEXCEPTION_RECORD32;
typedef struct _EXCEPTION_RECORD64 {
 DWORD ExceptionCode;
 DWORD ExceptionFlags;
 DWORD64 ExceptionRecord;
 DWORD64 ExceptionAddress;
 DWORD NumberParameters;
 DWORD __unusedAlignment;
 DWORD64 ExceptionInformation[15];
} EXCEPTION_RECORD64, *PEXCEPTION_RECORD64;
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
#pragma warning(disable:4103)
#pragma pack(push,4)
typedef struct _LUID_AND_ATTRIBUTES {
 LUID Luid;
 DWORD Attributes;
 } LUID_AND_ATTRIBUTES, * PLUID_AND_ATTRIBUTES;
typedef LUID_AND_ATTRIBUTES LUID_AND_ATTRIBUTES_ARRAY[1];
typedef LUID_AND_ATTRIBUTES_ARRAY *PLUID_AND_ATTRIBUTES_ARRAY;
#pragma warning(disable:4103)
#pragma pack(pop)
typedef struct _SID_IDENTIFIER_AUTHORITY {
 BYTE Value[6];
} SID_IDENTIFIER_AUTHORITY, *PSID_IDENTIFIER_AUTHORITY;
typedef struct _SID {
 BYTE Revision;
 BYTE SubAuthorityCount;
 SID_IDENTIFIER_AUTHORITY IdentifierAuthority;
 DWORD SubAuthority[1];
} SID, *PISID;
typedef enum _SID_NAME_USE {
 SidTypeUser = 1,
 SidTypeGroup,
 SidTypeDomain,
 SidTypeAlias,
 SidTypeWellKnownGroup,
 SidTypeDeletedAccount,
 SidTypeInvalid,
 SidTypeUnknown,
 SidTypeComputer,
 SidTypeLabel
} SID_NAME_USE, *PSID_NAME_USE;
typedef struct _SID_AND_ATTRIBUTES {
 PSID Sid;
 DWORD Attributes;
 } SID_AND_ATTRIBUTES, * PSID_AND_ATTRIBUTES;
typedef SID_AND_ATTRIBUTES SID_AND_ATTRIBUTES_ARRAY[1];
typedef SID_AND_ATTRIBUTES_ARRAY *PSID_AND_ATTRIBUTES_ARRAY;
typedef ULONG_PTR SID_HASH_ENTRY, *PSID_HASH_ENTRY;
typedef struct _SID_AND_ATTRIBUTES_HASH {
 DWORD SidCount;
 PSID_AND_ATTRIBUTES SidAttr;
 SID_HASH_ENTRY Hash[32];
} SID_AND_ATTRIBUTES_HASH, *PSID_AND_ATTRIBUTES_HASH;
typedef enum {
 WinNullSid = 0,
 WinWorldSid = 1,
 WinLocalSid = 2,
 WinCreatorOwnerSid = 3,
 WinCreatorGroupSid = 4,
 WinCreatorOwnerServerSid = 5,
 WinCreatorGroupServerSid = 6,
 WinNtAuthoritySid = 7,
 WinDialupSid = 8,
 WinNetworkSid = 9,
 WinBatchSid = 10,
 WinInteractiveSid = 11,
 WinServiceSid = 12,
 WinAnonymousSid = 13,
 WinProxySid = 14,
 WinEnterpriseControllersSid = 15,
 WinSelfSid = 16,
 WinAuthenticatedUserSid = 17,
 WinRestrictedCodeSid = 18,
 WinTerminalServerSid = 19,
 WinRemoteLogonIdSid = 20,
 WinLogonIdsSid = 21,
 WinLocalSystemSid = 22,
 WinLocalServiceSid = 23,
 WinNetworkServiceSid = 24,
 WinBuiltinDomainSid = 25,
 WinBuiltinAdministratorsSid = 26,
 WinBuiltinUsersSid = 27,
 WinBuiltinGuestsSid = 28,
 WinBuiltinPowerUsersSid = 29,
 WinBuiltinAccountOperatorsSid = 30,
 WinBuiltinSystemOperatorsSid = 31,
 WinBuiltinPrintOperatorsSid = 32,
 WinBuiltinBackupOperatorsSid = 33,
 WinBuiltinReplicatorSid = 34,
 WinBuiltinPreWindows2000CompatibleAccessSid = 35,
 WinBuiltinRemoteDesktopUsersSid = 36,
 WinBuiltinNetworkConfigurationOperatorsSid = 37,
 WinAccountAdministratorSid = 38,
 WinAccountGuestSid = 39,
 WinAccountKrbtgtSid = 40,
 WinAccountDomainAdminsSid = 41,
 WinAccountDomainUsersSid = 42,
 WinAccountDomainGuestsSid = 43,
 WinAccountComputersSid = 44,
 WinAccountControllersSid = 45,
 WinAccountCertAdminsSid = 46,
 WinAccountSchemaAdminsSid = 47,
 WinAccountEnterpriseAdminsSid = 48,
 WinAccountPolicyAdminsSid = 49,
 WinAccountRasAndIasServersSid = 50,
 WinNTLMAuthenticationSid = 51,
 WinDigestAuthenticationSid = 52,
 WinSChannelAuthenticationSid = 53,
 WinThisOrganizationSid = 54,
 WinOtherOrganizationSid = 55,
 WinBuiltinIncomingForestTrustBuildersSid = 56,
 WinBuiltinPerfMonitoringUsersSid = 57,
 WinBuiltinPerfLoggingUsersSid = 58,
 WinBuiltinAuthorizationAccessSid = 59,
 WinBuiltinTerminalServerLicenseServersSid = 60,
 WinBuiltinDCOMUsersSid = 61,
 WinBuiltinIUsersSid = 62,
 WinIUserSid = 63,
 WinBuiltinCryptoOperatorsSid = 64,
 WinUntrustedLabelSid = 65,
 WinLowLabelSid = 66,
 WinMediumLabelSid = 67,
 WinHighLabelSid = 68,
 WinSystemLabelSid = 69,
 WinWriteRestrictedCodeSid = 70,
 WinCreatorOwnerRightsSid = 71,
 WinCacheablePrincipalsGroupSid = 72,
 WinNonCacheablePrincipalsGroupSid = 73,
 WinEnterpriseReadonlyControllersSid = 74,
 WinAccountReadonlyControllersSid = 75,
 WinBuiltinEventLogReadersGroup = 76,
} WELL_KNOWN_SID_TYPE;
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
typedef struct _SYSTEM_MANDATORY_LABEL_ACE {
 ACE_HEADER Header;
 ACCESS_MASK Mask;
 DWORD SidStart;
} SYSTEM_MANDATORY_LABEL_ACE, *PSYSTEM_MANDATORY_LABEL_ACE;
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
typedef struct _ACCESS_ALLOWED_CALLBACK_ACE {
 ACE_HEADER Header;
 ACCESS_MASK Mask;
 DWORD SidStart;
} ACCESS_ALLOWED_CALLBACK_ACE, *PACCESS_ALLOWED_CALLBACK_ACE;
typedef struct _ACCESS_DENIED_CALLBACK_ACE {
 ACE_HEADER Header;
 ACCESS_MASK Mask;
 DWORD SidStart;
} ACCESS_DENIED_CALLBACK_ACE, *PACCESS_DENIED_CALLBACK_ACE;
typedef struct _SYSTEM_AUDIT_CALLBACK_ACE {
 ACE_HEADER Header;
 ACCESS_MASK Mask;
 DWORD SidStart;
} SYSTEM_AUDIT_CALLBACK_ACE, *PSYSTEM_AUDIT_CALLBACK_ACE;
typedef struct _SYSTEM_ALARM_CALLBACK_ACE {
 ACE_HEADER Header;
 ACCESS_MASK Mask;
 DWORD SidStart;
} SYSTEM_ALARM_CALLBACK_ACE, *PSYSTEM_ALARM_CALLBACK_ACE;
typedef struct _ACCESS_ALLOWED_CALLBACK_OBJECT_ACE {
 ACE_HEADER Header;
 ACCESS_MASK Mask;
 DWORD Flags;
 GUID ObjectType;
 GUID InheritedObjectType;
 DWORD SidStart;
} ACCESS_ALLOWED_CALLBACK_OBJECT_ACE, *PACCESS_ALLOWED_CALLBACK_OBJECT_ACE;
typedef struct _ACCESS_DENIED_CALLBACK_OBJECT_ACE {
 ACE_HEADER Header;
 ACCESS_MASK Mask;
 DWORD Flags;
 GUID ObjectType;
 GUID InheritedObjectType;
 DWORD SidStart;
} ACCESS_DENIED_CALLBACK_OBJECT_ACE, *PACCESS_DENIED_CALLBACK_OBJECT_ACE;
typedef struct _SYSTEM_AUDIT_CALLBACK_OBJECT_ACE {
 ACE_HEADER Header;
 ACCESS_MASK Mask;
 DWORD Flags;
 GUID ObjectType;
 GUID InheritedObjectType;
 DWORD SidStart;
} SYSTEM_AUDIT_CALLBACK_OBJECT_ACE, *PSYSTEM_AUDIT_CALLBACK_OBJECT_ACE;
typedef struct _SYSTEM_ALARM_CALLBACK_OBJECT_ACE {
 ACE_HEADER Header;
 ACCESS_MASK Mask;
 DWORD Flags;
 GUID ObjectType;
 GUID InheritedObjectType;
 DWORD SidStart;
} SYSTEM_ALARM_CALLBACK_OBJECT_ACE, *PSYSTEM_ALARM_CALLBACK_OBJECT_ACE;
typedef enum _ACL_INFORMATION_CLASS {
 AclRevisionInformation = 1,
 AclSizeInformation
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
 AuditEventObjectAccess,
 AuditEventDirectoryServiceAccess
} AUDIT_EVENT_TYPE, *PAUDIT_EVENT_TYPE;
typedef struct _PRIVILEGE_SET {
 DWORD PrivilegeCount;
 DWORD Control;
 LUID_AND_ATTRIBUTES Privilege[1];
 } PRIVILEGE_SET, * PPRIVILEGE_SET;
typedef enum _SECURITY_IMPERSONATION_LEVEL {
 SecurityAnonymous,
 SecurityIdentification,
 SecurityImpersonation,
 SecurityDelegation
 } SECURITY_IMPERSONATION_LEVEL, * PSECURITY_IMPERSONATION_LEVEL;
typedef enum _TOKEN_TYPE {
 TokenPrimary = 1,
 TokenImpersonation
 } TOKEN_TYPE;
typedef TOKEN_TYPE *PTOKEN_TYPE;
typedef enum _TOKEN_ELEVATION_TYPE {
 TokenElevationTypeDefault = 1,
 TokenElevationTypeFull,
 TokenElevationTypeLimited,
} TOKEN_ELEVATION_TYPE, *PTOKEN_ELEVATION_TYPE;
typedef enum _TOKEN_INFORMATION_CLASS {
 TokenUser = 1,
 TokenGroups,
 TokenPrivileges,
 TokenOwner,
 TokenPrimaryGroup,
 TokenDefaultDacl,
 TokenSource,
 TokenType,
 TokenImpersonationLevel,
 TokenStatistics,
 TokenRestrictedSids,
 TokenSessionId,
 TokenGroupsAndPrivileges,
 TokenSessionReference,
 TokenSandBoxInert,
 TokenAuditPolicy,
 TokenOrigin,
 TokenElevationType,
 TokenLinkedToken,
 TokenElevation,
 TokenHasRestrictions,
 TokenAccessInformation,
 TokenVirtualizationAllowed,
 TokenVirtualizationEnabled,
 TokenIntegrityLevel,
 TokenUIAccess,
 TokenMandatoryPolicy,
 TokenLogonSid,
 MaxTokenInfoClass
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
typedef struct _TOKEN_GROUPS_AND_PRIVILEGES {
 DWORD SidCount;
 DWORD SidLength;
 PSID_AND_ATTRIBUTES Sids;
 DWORD RestrictedSidCount;
 DWORD RestrictedSidLength;
 PSID_AND_ATTRIBUTES RestrictedSids;
 DWORD PrivilegeCount;
 DWORD PrivilegeLength;
 PLUID_AND_ATTRIBUTES Privileges;
 LUID AuthenticationId;
} TOKEN_GROUPS_AND_PRIVILEGES, *PTOKEN_GROUPS_AND_PRIVILEGES;
typedef struct _TOKEN_LINKED_TOKEN {
 HANDLE LinkedToken;
} TOKEN_LINKED_TOKEN, *PTOKEN_LINKED_TOKEN;
typedef struct _TOKEN_ELEVATION {
 DWORD TokenIsElevated;
} TOKEN_ELEVATION, *PTOKEN_ELEVATION;
typedef struct _TOKEN_MANDATORY_LABEL {
 SID_AND_ATTRIBUTES Label;
} TOKEN_MANDATORY_LABEL, *PTOKEN_MANDATORY_LABEL;
typedef struct _TOKEN_MANDATORY_POLICY {
 DWORD Policy;
} TOKEN_MANDATORY_POLICY, *PTOKEN_MANDATORY_POLICY;
typedef struct _TOKEN_ACCESS_INFORMATION {
 PSID_AND_ATTRIBUTES_HASH SidHash;
 PSID_AND_ATTRIBUTES_HASH RestrictedSidHash;
 PTOKEN_PRIVILEGES Privileges;
 LUID AuthenticationId;
 TOKEN_TYPE TokenType;
 SECURITY_IMPERSONATION_LEVEL ImpersonationLevel;
 TOKEN_MANDATORY_POLICY MandatoryPolicy;
 DWORD Flags;
} TOKEN_ACCESS_INFORMATION, *PTOKEN_ACCESS_INFORMATION;
typedef struct _TOKEN_AUDIT_POLICY {
 BYTE PerUserPolicy[(((50)) >> 1) + 1];
} TOKEN_AUDIT_POLICY, *PTOKEN_AUDIT_POLICY;
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
typedef struct _TOKEN_ORIGIN {
 LUID OriginatingLogonSession ;
} TOKEN_ORIGIN, * PTOKEN_ORIGIN ;
typedef enum _MANDATORY_LEVEL {
 MandatoryLevelUntrusted = 0,
 MandatoryLevelLow,
 MandatoryLevelMedium,
 MandatoryLevelHigh,
 MandatoryLevelSystem,
 MandatoryLevelSecureProcess,
 MandatoryLevelCount
} MANDATORY_LEVEL, *PMANDATORY_LEVEL;
typedef BOOLEAN SECURITY_CONTEXT_TRACKING_MODE,
 * PSECURITY_CONTEXT_TRACKING_MODE;
typedef struct _SECURITY_QUALITY_OF_SERVICE {
 DWORD Length;
 SECURITY_IMPERSONATION_LEVEL ImpersonationLevel;
 SECURITY_CONTEXT_TRACKING_MODE ContextTrackingMode;
 BOOLEAN EffectiveOnly;
 } SECURITY_QUALITY_OF_SERVICE, * PSECURITY_QUALITY_OF_SERVICE;
typedef struct _SE_IMPERSONATION_STATE {
 PACCESS_TOKEN Token;
 BOOLEAN CopyOnOpen;
 BOOLEAN EffectiveOnly;
 SECURITY_IMPERSONATION_LEVEL Level;
} SE_IMPERSONATION_STATE, *PSE_IMPERSONATION_STATE;
typedef DWORD SECURITY_INFORMATION, *PSECURITY_INFORMATION;
typedef struct _JOB_SET_ARRAY {
 HANDLE JobHandle;
 DWORD MemberLevel;
 DWORD Flags;
} JOB_SET_ARRAY, *PJOB_SET_ARRAY;
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
typedef struct _NT_TIB32 {
 DWORD ExceptionList;
 DWORD StackBase;
 DWORD StackLimit;
 DWORD SubSystemTib;
 union {
 DWORD FiberData;
 DWORD Version;
 };
 DWORD ArbitraryUserPointer;
 DWORD Self;
} NT_TIB32, *PNT_TIB32;
typedef struct _NT_TIB64 {
 DWORD64 ExceptionList;
 DWORD64 StackBase;
 DWORD64 StackLimit;
 DWORD64 SubSystemTib;
 union {
 DWORD64 FiberData;
 DWORD Version;
 };
 DWORD64 ArbitraryUserPointer;
 DWORD64 Self;
} NT_TIB64, *PNT_TIB64;
typedef struct _QUOTA_LIMITS {
 SIZE_T PagedPoolLimit;
 SIZE_T NonPagedPoolLimit;
 SIZE_T MinimumWorkingSetSize;
 SIZE_T MaximumWorkingSetSize;
 SIZE_T PagefileLimit;
 LARGE_INTEGER TimeLimit;
} QUOTA_LIMITS, *PQUOTA_LIMITS;
typedef enum _PS_RATE_PHASE {
 PsRateOneSecond = 0,
 PsRateTwoSecond,
 PsRateThreeSecond,
 PsRateMaxPhase
} PS_RATE_PHASE;
typedef union _RATE_QUOTA_LIMIT {
 DWORD RateData;
 struct {
 DWORD RatePhase : 4;
 DWORD RatePercent : 28;
 };
} RATE_QUOTA_LIMIT, *PRATE_QUOTA_LIMIT;
typedef struct _QUOTA_LIMITS_EX {
 SIZE_T PagedPoolLimit;
 SIZE_T NonPagedPoolLimit;
 SIZE_T MinimumWorkingSetSize;
 SIZE_T MaximumWorkingSetSize;
 SIZE_T PagefileLimit;
 LARGE_INTEGER TimeLimit;
 SIZE_T WorkingSetLimit;
 SIZE_T Reserved2;
 SIZE_T Reserved3;
 SIZE_T Reserved4;
 DWORD Flags;
 RATE_QUOTA_LIMIT CpuRateLimit;
} QUOTA_LIMITS_EX, *PQUOTA_LIMITS_EX;
typedef struct _IO_COUNTERS {
 ULONGLONG ReadOperationCount;
 ULONGLONG WriteOperationCount;
 ULONGLONG OtherOperationCount;
 ULONGLONG ReadTransferCount;
 ULONGLONG WriteTransferCount;
 ULONGLONG OtherTransferCount;
} IO_COUNTERS;
typedef IO_COUNTERS *PIO_COUNTERS;
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
 SIZE_T MinimumWorkingSetSize;
 SIZE_T MaximumWorkingSetSize;
 DWORD ActiveProcessLimit;
 ULONG_PTR Affinity;
 DWORD PriorityClass;
 DWORD SchedulingClass;
} JOBOBJECT_BASIC_LIMIT_INFORMATION, *PJOBOBJECT_BASIC_LIMIT_INFORMATION;
typedef struct _JOBOBJECT_EXTENDED_LIMIT_INFORMATION {
 JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
 IO_COUNTERS IoInfo;
 SIZE_T ProcessMemoryLimit;
 SIZE_T JobMemoryLimit;
 SIZE_T PeakProcessMemoryUsed;
 SIZE_T PeakJobMemoryUsed;
} JOBOBJECT_EXTENDED_LIMIT_INFORMATION, *PJOBOBJECT_EXTENDED_LIMIT_INFORMATION;
typedef struct _JOBOBJECT_BASIC_PROCESS_ID_LIST {
 DWORD NumberOfAssignedProcesses;
 DWORD NumberOfProcessIdsInList;
 ULONG_PTR ProcessIdList[1];
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
typedef struct _JOBOBJECT_BASIC_AND_IO_ACCOUNTING_INFORMATION {
 JOBOBJECT_BASIC_ACCOUNTING_INFORMATION BasicInfo;
 IO_COUNTERS IoInfo;
} JOBOBJECT_BASIC_AND_IO_ACCOUNTING_INFORMATION, *PJOBOBJECT_BASIC_AND_IO_ACCOUNTING_INFORMATION;
typedef struct _JOBOBJECT_JOBSET_INFORMATION {
 DWORD MemberLevel;
} JOBOBJECT_JOBSET_INFORMATION, *PJOBOBJECT_JOBSET_INFORMATION;
typedef enum _JOBOBJECTINFOCLASS {
 JobObjectBasicAccountingInformation = 1,
 JobObjectBasicLimitInformation,
 JobObjectBasicProcessIdList,
 JobObjectBasicUIRestrictions,
 JobObjectSecurityLimitInformation,
 JobObjectEndOfJobTimeInformation,
 JobObjectAssociateCompletionPortInformation,
 JobObjectBasicAndIoAccountingInformation,
 JobObjectExtendedLimitInformation,
 JobObjectJobSetInformation,
 MaxJobObjectInfoClass
 } JOBOBJECTINFOCLASS;
typedef enum _LOGICAL_PROCESSOR_RELATIONSHIP {
 RelationProcessorCore,
 RelationNumaNode,
 RelationCache,
 RelationProcessorPackage
} LOGICAL_PROCESSOR_RELATIONSHIP;
typedef enum _PROCESSOR_CACHE_TYPE {
 CacheUnified,
 CacheInstruction,
 CacheData,
 CacheTrace
} PROCESSOR_CACHE_TYPE;
typedef struct _CACHE_DESCRIPTOR {
 BYTE Level;
 BYTE Associativity;
 WORD LineSize;
 DWORD Size;
 PROCESSOR_CACHE_TYPE Type;
} CACHE_DESCRIPTOR, *PCACHE_DESCRIPTOR;
typedef struct _SYSTEM_LOGICAL_PROCESSOR_INFORMATION {
 ULONG_PTR ProcessorMask;
 LOGICAL_PROCESSOR_RELATIONSHIP Relationship;
 union {
 struct {
 BYTE Flags;
 } ProcessorCore;
 struct {
 DWORD NodeNumber;
 } NumaNode;
 CACHE_DESCRIPTOR Cache;
 ULONGLONG Reserved[2];
 };
} SYSTEM_LOGICAL_PROCESSOR_INFORMATION, *PSYSTEM_LOGICAL_PROCESSOR_INFORMATION;
typedef struct _MEMORY_BASIC_INFORMATION {
 PVOID BaseAddress;
 PVOID AllocationBase;
 DWORD AllocationProtect;
 SIZE_T RegionSize;
 DWORD State;
 DWORD Protect;
 DWORD Type;
} MEMORY_BASIC_INFORMATION, *PMEMORY_BASIC_INFORMATION;
typedef struct _MEMORY_BASIC_INFORMATION32 {
 DWORD BaseAddress;
 DWORD AllocationBase;
 DWORD AllocationProtect;
 DWORD RegionSize;
 DWORD State;
 DWORD Protect;
 DWORD Type;
} MEMORY_BASIC_INFORMATION32, *PMEMORY_BASIC_INFORMATION32;
typedef struct __declspec(align(16)) _MEMORY_BASIC_INFORMATION64 {
 ULONGLONG BaseAddress;
 ULONGLONG AllocationBase;
 DWORD AllocationProtect;
 DWORD __alignment1;
 ULONGLONG RegionSize;
 DWORD State;
 DWORD Protect;
 DWORD Type;
 DWORD __alignment2;
} MEMORY_BASIC_INFORMATION64, *PMEMORY_BASIC_INFORMATION64;
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
typedef struct _REPARSE_GUID_DATA_BUFFER {
 DWORD ReparseTag;
 WORD ReparseDataLength;
 WORD Reserved;
 GUID ReparseGuid;
 struct {
 BYTE DataBuffer[1];
 } GenericReparseBuffer;
} REPARSE_GUID_DATA_BUFFER, *PREPARSE_GUID_DATA_BUFFER;
extern "C" const GUID GUID_MAX_POWER_SAVINGS;
extern "C" const GUID GUID_MIN_POWER_SAVINGS;
extern "C" const GUID GUID_TYPICAL_POWER_SAVINGS;
extern "C" const GUID NO_SUBGROUP_GUID;
extern "C" const GUID ALL_POWERSCHEMES_GUID;
extern "C" const GUID GUID_POWERSCHEME_PERSONALITY;
extern "C" const GUID GUID_ACTIVE_POWERSCHEME;
extern "C" const GUID GUID_VIDEO_SUBGROUP;
extern "C" const GUID GUID_VIDEO_POWERDOWN_TIMEOUT;
extern "C" const GUID GUID_VIDEO_ADAPTIVE_POWERDOWN;
extern "C" const GUID GUID_MONITOR_POWER_ON;
extern "C" const GUID GUID_DISK_SUBGROUP;
extern "C" const GUID GUID_DISK_POWERDOWN_TIMEOUT;
extern "C" const GUID GUID_DISK_ADAPTIVE_POWERDOWN;
extern "C" const GUID GUID_SLEEP_SUBGROUP;
extern "C" const GUID GUID_SLEEP_IDLE_THRESHOLD;
extern "C" const GUID GUID_STANDBY_TIMEOUT;
extern "C" const GUID GUID_HIBERNATE_TIMEOUT;
extern "C" const GUID GUID_HIBERNATE_FASTS4_POLICY;
extern "C" const GUID GUID_CRITICAL_POWER_TRANSITION;
extern "C" const GUID GUID_SYSTEM_AWAYMODE;
extern "C" const GUID GUID_ALLOW_AWAYMODE;
extern "C" const GUID GUID_ALLOW_STANDBY_STATES;
extern "C" const GUID GUID_ALLOW_RTC_WAKE;
extern "C" const GUID GUID_SYSTEM_BUTTON_SUBGROUP;
extern "C" const GUID GUID_POWERBUTTON_ACTION;
extern "C" const GUID GUID_POWERBUTTON_ACTION_FLAGS;
extern "C" const GUID GUID_SLEEPBUTTON_ACTION;
extern "C" const GUID GUID_SLEEPBUTTON_ACTION_FLAGS;
extern "C" const GUID GUID_USERINTERFACEBUTTON_ACTION;
extern "C" const GUID GUID_LIDCLOSE_ACTION;
extern "C" const GUID GUID_LIDCLOSE_ACTION_FLAGS;
extern "C" const GUID GUID_LIDOPEN_POWERSTATE;
extern "C" const GUID GUID_BATTERY_SUBGROUP;
extern "C" const GUID GUID_BATTERY_DISCHARGE_ACTION_0;
extern "C" const GUID GUID_BATTERY_DISCHARGE_LEVEL_0;
extern "C" const GUID GUID_BATTERY_DISCHARGE_FLAGS_0;
extern "C" const GUID GUID_BATTERY_DISCHARGE_ACTION_1;
extern "C" const GUID GUID_BATTERY_DISCHARGE_LEVEL_1;
extern "C" const GUID GUID_BATTERY_DISCHARGE_FLAGS_1;
extern "C" const GUID GUID_BATTERY_DISCHARGE_ACTION_2;
extern "C" const GUID GUID_BATTERY_DISCHARGE_LEVEL_2;
extern "C" const GUID GUID_BATTERY_DISCHARGE_FLAGS_2;
extern "C" const GUID GUID_BATTERY_DISCHARGE_ACTION_3;
extern "C" const GUID GUID_BATTERY_DISCHARGE_LEVEL_3;
extern "C" const GUID GUID_BATTERY_DISCHARGE_FLAGS_3;
extern "C" const GUID GUID_PROCESSOR_SETTINGS_SUBGROUP;
extern "C" const GUID GUID_PROCESSOR_THROTTLE_POLICY;
extern "C" const GUID GUID_PROCESSOR_THROTTLE_MAXIMUM;
extern "C" const GUID GUID_PROCESSOR_THROTTLE_MINIMUM;
extern "C" const GUID GUID_PROCESSOR_IDLESTATE_POLICY;
extern "C" const GUID GUID_PROCESSOR_PERFSTATE_POLICY;
extern "C" const GUID GUID_SYSTEM_COOLING_POLICY;
extern "C" const GUID GUID_LOCK_CONSOLE_ON_WAKE;
extern "C" const GUID GUID_ACDC_POWER_SOURCE;
extern "C" const GUID GUID_LIDSWITCH_STATE_CHANGE;
extern "C" const GUID GUID_BATTERY_PERCENTAGE_REMAINING;
extern "C" const GUID GUID_IDLE_BACKGROUND_TASK;
extern "C" const GUID GUID_BACKGROUND_TASK_NOTIFICATION;
extern "C" const GUID GUID_APPLAUNCH_BUTTON;
extern "C" const GUID GUID_PCIEXPRESS_SETTINGS_SUBGROUP;
extern "C" const GUID GUID_PCIEXPRESS_ASPM_POLICY;
typedef enum _SYSTEM_POWER_STATE {
 PowerSystemUnspecified = 0,
 PowerSystemWorking = 1,
 PowerSystemSleeping1 = 2,
 PowerSystemSleeping2 = 3,
 PowerSystemSleeping3 = 4,
 PowerSystemHibernate = 5,
 PowerSystemShutdown = 6,
 PowerSystemMaximum = 7
} SYSTEM_POWER_STATE, *PSYSTEM_POWER_STATE;
typedef enum {
 PowerActionNone = 0,
 PowerActionReserved,
 PowerActionSleep,
 PowerActionHibernate,
 PowerActionShutdown,
 PowerActionShutdownReset,
 PowerActionShutdownOff,
 PowerActionWarmEject
} POWER_ACTION, *PPOWER_ACTION;
typedef enum _DEVICE_POWER_STATE {
 PowerDeviceUnspecified = 0,
 PowerDeviceD0,
 PowerDeviceD1,
 PowerDeviceD2,
 PowerDeviceD3,
 PowerDeviceMaximum
} DEVICE_POWER_STATE, *PDEVICE_POWER_STATE;
typedef DWORD EXECUTION_STATE;
typedef enum {
 LT_DONT_CARE,
 LT_LOWEST_LATENCY
} LATENCY_TIME;
typedef struct CM_Power_Data_s {
 DWORD PD_Size;
 DEVICE_POWER_STATE PD_MostRecentPowerState;
 DWORD PD_Capabilities;
 DWORD PD_D1Latency;
 DWORD PD_D2Latency;
 DWORD PD_D3Latency;
 DEVICE_POWER_STATE PD_PowerStateMapping[7];
 SYSTEM_POWER_STATE PD_DeepestSystemWake;
} CM_POWER_DATA, *PCM_POWER_DATA;
typedef enum {
 SystemPowerPolicyAc,
 SystemPowerPolicyDc,
 VerifySystemPolicyAc,
 VerifySystemPolicyDc,
 SystemPowerCapabilities,
 SystemBatteryState,
 SystemPowerStateHandler,
 ProcessorStateHandler,
 SystemPowerPolicyCurrent,
 AdministratorPowerPolicy,
 SystemReserveHiberFile,
 ProcessorInformation,
 SystemPowerInformation,
 ProcessorStateHandler2,
 LastWakeTime,
 LastSleepTime,
 SystemExecutionState,
 SystemPowerStateNotifyHandler,
 ProcessorPowerPolicyAc,
 ProcessorPowerPolicyDc,
 VerifyProcessorPowerPolicyAc,
 VerifyProcessorPowerPolicyDc,
 ProcessorPowerPolicyCurrent,
 SystemPowerStateLogging,
 SystemPowerLoggingEntry,
 SetPowerSettingValue,
 NotifyUserPowerSetting,
 GetPowerTransitionVetoes,
 SetPowerTransitionVeto,
 SystemVideoState,
 TraceApplicationPowerMessage,
 TraceApplicationPowerMessageEnd,
 ProcessorPerfStates,
 ProcessorIdleStates,
 ProcessorThrottleStates,
 SystemWakeSource,
 SystemHiberFileInformation,
 TraceServicePowerMessage,
 ProcessorLoad,
 PowerShutdownNotification
} POWER_INFORMATION_LEVEL;
typedef struct _PO_TRANSITION_VETO_REASON {
 DWORD ResourceId;
 DWORD ModuleNameOffset;
} PO_TRANSITION_VETO_REASON, *PPO_TRANSITION_VETO_REASON;
typedef struct _PO_TRANSITION_VETO_WINDOW {
 HANDLE Handle;
} PO_TRANSITION_VETO_WINDOW, *PPO_TRANSITION_VETO_WINDOW;
typedef struct _PO_TRANSITION_VETO_SERVICE {
 DWORD ServiceNameOffset;
} PO_TRANSITION_VETO_SERVICE, *PPO_TRANSITION_VETO_SERVICE;
typedef struct _PO_TRANSITION_VETO {
 DWORD Type;
 PO_TRANSITION_VETO_REASON Reason;
 DWORD ProcessId;
 union {
 PO_TRANSITION_VETO_WINDOW Window;
 PO_TRANSITION_VETO_SERVICE Service;
 };
} PO_TRANSITION_VETO, *PPO_TRANSITION_VETO;
typedef struct _PO_TRANSITION_VETOES {
 DWORD Count;
 PO_TRANSITION_VETO Vetoes[1];
} PO_TRANSITION_VETOES, *PPO_TRANSITION_VETOES;
typedef enum {
 PoAc,
 PoDc,
 PoHot,
 PoConditionMaximum
} SYSTEM_POWER_CONDITION;
typedef struct {
 DWORD Version;
 GUID Guid;
 SYSTEM_POWER_CONDITION PowerCondition;
 DWORD DataLength;
 BYTE Data[1];
} SET_POWER_SETTING_VALUE, *PSET_POWER_SETTING_VALUE;
typedef struct {
 GUID Guid;
} NOTIFY_USER_POWER_SETTING, *PNOTIFY_USER_POWER_SETTING;
typedef struct _APPLICATIONLAUNCH_SETTING_VALUE {
 LARGE_INTEGER ActivationTime;
 DWORD Flags;
 DWORD ButtonInstanceID;
} APPLICATIONLAUNCH_SETTING_VALUE, *PAPPLICATIONLAUNCH_SETTING_VALUE;
typedef enum {
 PlatformRoleUnspecified = 0,
 PlatformRoleDesktop,
 PlatformRoleMobile,
 PlatformRoleWorkstation,
 PlatformRoleEnterpriseServer,
 PlatformRoleSOHOServer,
 PlatformRoleAppliancePC,
 PlatformRolePerformanceServer,
 PlatformRoleMaximum
} POWER_PLATFORM_ROLE;
typedef enum {
 DeviceWakeSourceType,
 FixedWakeSourceType
} PO_WAKE_SOURCE_TYPE, *PPO_WAKE_SOURCE_TYPE;
typedef enum {
 FixedWakeSourcePowerButton,
 FixedWakeSourceSleepButton,
 FixedWakeSourceRtc
} PO_FIXED_WAKE_SOURCE_TYPE, *PPO_FIXED_WAKE_SOURCE_TYPE;
typedef struct _PO_WAKE_SOURCE_HEADER {
 PO_WAKE_SOURCE_TYPE Type;
 DWORD Size;
} PO_WAKE_SOURCE_HEADER, *PPO_WAKE_SOURCE_HEADER;
typedef struct _PO_WAKE_SOURCE_DEVICE {
 PO_WAKE_SOURCE_HEADER Header;
 WCHAR InstancePath[1];
} PO_WAKE_SOURCE_DEVICE, *PPO_WAKE_SOURCE_DEVICE;
typedef struct _PO_WAKE_SOURCE_FIXED {
 PO_WAKE_SOURCE_HEADER Header;
 PO_FIXED_WAKE_SOURCE_TYPE FixedWakeSourceType;
} PO_WAKE_SOURCE_FIXED, *PPO_WAKE_SOURCE_FIXED;
typedef struct _PO_WAKE_SOURCE_INFO {
 DWORD Count;
 DWORD Offsets[1];
} PO_WAKE_SOURCE_INFO, *PPO_WAKE_SOURCE_INFO;
typedef struct _PO_WAKE_SOURCE_HISTORY {
 DWORD Count;
 DWORD Offsets[1];
} PO_WAKE_SOURCE_HISTORY, *PPO_WAKE_SOURCE_HISTORY;
typedef struct {
 DWORD Granularity;
 DWORD Capacity;
} BATTERY_REPORTING_SCALE, *PBATTERY_REPORTING_SCALE;
typedef struct {
 BOOLEAN Enabled;
 BYTE PercentBusy[32];
} PPM_SIMULATED_PROCESSOR_LOAD, *PPPM_SIMULATED_PROCESSOR_LOAD;
typedef struct {
 DWORD Frequency;
 DWORD Flags;
 DWORD PercentFrequency;
} PPM_WMI_LEGACY_PERFSTATE, *PPPM_WMI_LEGACY_PERFSTATE;
typedef struct {
 DWORD Latency;
 DWORD Power;
 DWORD TimeCheck;
 BYTE PromotePercent;
 BYTE DemotePercent;
 BYTE StateType;
 BYTE Reserved;
 DWORD StateFlags;
 DWORD Context;
 DWORD IdleHandler;
 DWORD Reserved1;
} PPM_WMI_IDLE_STATE, *PPPM_WMI_IDLE_STATE;
typedef struct {
 DWORD Type;
 DWORD Count;
 DWORD TargetState;
 DWORD OldState;
 DWORD64 TargetProcessors;
 PPM_WMI_IDLE_STATE State[1];
} PPM_WMI_IDLE_STATES, *PPPM_WMI_IDLE_STATES;
typedef struct {
 DWORD Frequency;
 DWORD Power;
 BYTE PercentFrequency;
 BYTE IncreaseLevel;
 BYTE DecreaseLevel;
 BYTE Type;
 DWORD IncreaseTime;
 DWORD DecreaseTime;
 DWORD64 Control;
 DWORD64 Status;
 DWORD HitCount;
 DWORD Reserved1;
 DWORD64 Reserved2;
 DWORD64 Reserved3;
} PPM_WMI_PERF_STATE, *PPPM_WMI_PERF_STATE;
typedef struct {
 DWORD Count;
 DWORD MaxFrequency;
 DWORD CurrentState;
 DWORD MaxPerfState;
 DWORD MinPerfState;
 DWORD LowestPerfState;
 DWORD ThermalConstraint;
 BYTE BusyAdjThreshold;
 BYTE PolicyType;
 BYTE Type;
 BYTE Reserved;
 DWORD TimerInterval;
 DWORD64 TargetProcessors;
 DWORD PStateHandler;
 DWORD PStateContext;
 DWORD TStateHandler;
 DWORD TStateContext;
 DWORD FeedbackHandler;
 DWORD Reserved1;
 DWORD64 Reserved2;
 PPM_WMI_PERF_STATE State[1];
} PPM_WMI_PERF_STATES, *PPPM_WMI_PERF_STATES;
typedef struct {
 DWORD IdleTransitions;
 DWORD FailedTransitions;
 DWORD InvalidBucketIndex;
 DWORD64 TotalTime;
 DWORD IdleTimeBuckets[6];
} PPM_IDLE_STATE_ACCOUNTING, *PPPM_IDLE_STATE_ACCOUNTING;
typedef struct {
 DWORD StateCount;
 DWORD TotalTransitions;
 DWORD ResetCount;
 DWORD64 StartTime;
 PPM_IDLE_STATE_ACCOUNTING State[1];
} PPM_IDLE_ACCOUNTING, *PPPM_IDLE_ACCOUNTING;
extern "C" const GUID PPM_PERFSTATE_CHANGE_GUID;
extern "C" const GUID PPM_PERFSTATE_DOMAIN_CHANGE_GUID;
extern "C" const GUID PPM_IDLESTATE_CHANGE_GUID;
extern "C" const GUID PPM_PERFSTATES_DATA_GUID;
extern "C" const GUID PPM_IDLESTATES_DATA_GUID;
extern "C" const GUID PPM_IDLE_ACCOUNTING_GUID;
extern "C" const GUID PPM_THERMALCONSTRAINT_GUID;
extern "C" const GUID PPM_PERFMON_PERFSTATE_GUID;
extern "C" const GUID PPM_THERMAL_POLICY_CHANGE_GUID;
typedef struct {
 DWORD State;
 DWORD Status;
 DWORD Latency;
 DWORD Speed;
 DWORD Processor;
} PPM_PERFSTATE_EVENT, *PPPM_PERFSTATE_EVENT;
typedef struct {
 DWORD State;
 DWORD Latency;
 DWORD Speed;
 DWORD64 Processors;
} PPM_PERFSTATE_DOMAIN_EVENT, *PPPM_PERFSTATE_DOMAIN_EVENT;
typedef struct {
 DWORD NewState;
 DWORD OldState;
 DWORD64 Processors;
} PPM_IDLESTATE_EVENT, *PPPM_IDLESTATE_EVENT;
typedef struct {
 DWORD ThermalConstraint;
 DWORD64 Processors;
} PPM_THERMALCHANGE_EVENT, *PPPM_THERMALCHANGE_EVENT;
#pragma warning(push)
#pragma warning(disable:4121)
typedef struct {
 BYTE Mode;
 DWORD64 Processors;
} PPM_THERMAL_POLICY_EVENT, *PPPM_THERMAL_POLICY_EVENT;
#pragma warning(pop)
typedef struct {
 POWER_ACTION Action;
 DWORD Flags;
 DWORD EventCode;
} POWER_ACTION_POLICY, *PPOWER_ACTION_POLICY;
typedef struct {
 BOOLEAN Enable;
 BYTE Spare[3];
 DWORD BatteryLevel;
 POWER_ACTION_POLICY PowerPolicy;
 SYSTEM_POWER_STATE MinSystemState;
} SYSTEM_POWER_LEVEL, *PSYSTEM_POWER_LEVEL;
typedef struct _SYSTEM_POWER_POLICY {
 DWORD Revision;
 POWER_ACTION_POLICY PowerButton;
 POWER_ACTION_POLICY SleepButton;
 POWER_ACTION_POLICY LidClose;
 SYSTEM_POWER_STATE LidOpenWake;
 DWORD Reserved;
 POWER_ACTION_POLICY Idle;
 DWORD IdleTimeout;
 BYTE IdleSensitivity;
 BYTE DynamicThrottle;
 BYTE Spare2[2];
 SYSTEM_POWER_STATE MinSleep;
 SYSTEM_POWER_STATE MaxSleep;
 SYSTEM_POWER_STATE ReducedLatencySleep;
 DWORD WinLogonFlags;
 DWORD Spare3;
 DWORD DozeS4Timeout;
 DWORD BroadcastCapacityResolution;
 SYSTEM_POWER_LEVEL DischargePolicy[4];
 DWORD VideoTimeout;
 BOOLEAN VideoDimDisplay;
 DWORD VideoReserved[3];
 DWORD SpindownTimeout;
 BOOLEAN OptimizeForPower;
 BYTE FanThrottleTolerance;
 BYTE ForcedThrottle;
 BYTE MinThrottle;
 POWER_ACTION_POLICY OverThrottled;
} SYSTEM_POWER_POLICY, *PSYSTEM_POWER_POLICY;
typedef struct {
 DWORD TimeCheck;
 BYTE DemotePercent;
 BYTE PromotePercent;
 BYTE Spare[2];
} PROCESSOR_IDLESTATE_INFO, *PPROCESSOR_IDLESTATE_INFO;
typedef struct {
 WORD Revision;
 union {
 WORD AsWORD ;
 struct {
 WORD AllowScaling : 1;
 WORD Disabled : 1;
 WORD Reserved : 14;
 };
 } Flags;
 DWORD PolicyCount;
 PROCESSOR_IDLESTATE_INFO Policy[0x3];
} PROCESSOR_IDLESTATE_POLICY, *PPROCESSOR_IDLESTATE_POLICY;
typedef struct _PROCESSOR_POWER_POLICY_INFO {
 DWORD TimeCheck;
 DWORD DemoteLimit;
 DWORD PromoteLimit;
 BYTE DemotePercent;
 BYTE PromotePercent;
 BYTE Spare[2];
 DWORD AllowDemotion:1;
 DWORD AllowPromotion:1;
 DWORD Reserved:30;
} PROCESSOR_POWER_POLICY_INFO, *PPROCESSOR_POWER_POLICY_INFO;
typedef struct _PROCESSOR_POWER_POLICY {
 DWORD Revision;
 BYTE DynamicThrottle;
 BYTE Spare[3];
 DWORD DisableCStates:1;
 DWORD Reserved:31;
 DWORD PolicyCount;
 PROCESSOR_POWER_POLICY_INFO Policy[3];
} PROCESSOR_POWER_POLICY, *PPROCESSOR_POWER_POLICY;
typedef struct {
 DWORD Revision;
 BYTE MaxThrottle;
 BYTE MinThrottle;
 BYTE BusyAdjThreshold;
 union {
 BYTE Spare;
 union {
 BYTE AsBYTE ;
 struct {
 BYTE NoDomainAccounting : 1;
 BYTE IncreasePolicy: 2;
 BYTE DecreasePolicy: 2;
 BYTE Reserved : 3;
 };
 } Flags;
 };
 DWORD TimeCheck;
 DWORD IncreaseTime;
 DWORD DecreaseTime;
 DWORD IncreasePercent;
 DWORD DecreasePercent;
} PROCESSOR_PERFSTATE_POLICY, *PPROCESSOR_PERFSTATE_POLICY;
typedef struct _ADMINISTRATOR_POWER_POLICY {
 SYSTEM_POWER_STATE MinSleep;
 SYSTEM_POWER_STATE MaxSleep;
 DWORD MinVideoTimeout;
 DWORD MaxVideoTimeout;
 DWORD MinSpindownTimeout;
 DWORD MaxSpindownTimeout;
} ADMINISTRATOR_POWER_POLICY, *PADMINISTRATOR_POWER_POLICY;
typedef struct {
 BOOLEAN PowerButtonPresent;
 BOOLEAN SleepButtonPresent;
 BOOLEAN LidPresent;
 BOOLEAN SystemS1;
 BOOLEAN SystemS2;
 BOOLEAN SystemS3;
 BOOLEAN SystemS4;
 BOOLEAN SystemS5;
 BOOLEAN HiberFilePresent;
 BOOLEAN FullWake;
 BOOLEAN VideoDimPresent;
 BOOLEAN ApmPresent;
 BOOLEAN UpsPresent;
 BOOLEAN ThermalControl;
 BOOLEAN ProcessorThrottle;
 BYTE ProcessorMinThrottle;
 BYTE ProcessorMaxThrottle;
 BOOLEAN FastSystemS4;
 BYTE spare2[3];
 BOOLEAN DiskSpinDown;
 BYTE spare3[8];
 BOOLEAN SystemBatteriesPresent;
 BOOLEAN BatteriesAreShortTerm;
 BATTERY_REPORTING_SCALE BatteryScale[3];
 SYSTEM_POWER_STATE AcOnLineWake;
 SYSTEM_POWER_STATE SoftLidWake;
 SYSTEM_POWER_STATE RtcWake;
 SYSTEM_POWER_STATE MinDeviceWakeState;
 SYSTEM_POWER_STATE DefaultLowLatencyWake;
} SYSTEM_POWER_CAPABILITIES, *PSYSTEM_POWER_CAPABILITIES;
typedef struct {
 BOOLEAN AcOnLine;
 BOOLEAN BatteryPresent;
 BOOLEAN Charging;
 BOOLEAN Discharging;
 BOOLEAN Spare1[4];
 DWORD MaxCapacity;
 DWORD RemainingCapacity;
 DWORD Rate;
 DWORD EstimatedTime;
 DWORD DefaultAlert1;
 DWORD DefaultAlert2;
} SYSTEM_BATTERY_STATE, *PSYSTEM_BATTERY_STATE;
#pragma warning(disable:4103)
#pragma pack(push,4)
#pragma warning(disable:4103)
#pragma pack(push,2)
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
#pragma warning(disable:4103)
#pragma pack(pop)
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
typedef struct ANON_OBJECT_HEADER {
 WORD Sig1;
 WORD Sig2;
 WORD Version;
 WORD Machine;
 DWORD TimeDateStamp;
 CLSID ClassID;
 DWORD SizeOfData;
} ANON_OBJECT_HEADER;
typedef struct ANON_OBJECT_HEADER_V2 {
 WORD Sig1;
 WORD Sig2;
 WORD Version;
 WORD Machine;
 DWORD TimeDateStamp;
 CLSID ClassID;
 DWORD SizeOfData;
 DWORD Flags;
 DWORD MetaDataSize;
 DWORD MetaDataOffset;
} ANON_OBJECT_HEADER_V2;
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
#pragma warning(disable:4103)
#pragma pack(push,2)
typedef struct _IMAGE_SYMBOL {
 union {
 BYTE ShortName[8];
 struct {
 DWORD Short;
 DWORD Long;
 } Name;
 DWORD LongName[2];
 } N;
 DWORD Value;
 SHORT SectionNumber;
 WORD Type;
 BYTE StorageClass;
 BYTE NumberOfAuxSymbols;
} IMAGE_SYMBOL;
typedef IMAGE_SYMBOL *PIMAGE_SYMBOL;
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
typedef enum IMAGE_AUX_SYMBOL_TYPE {
 IMAGE_AUX_SYMBOL_TYPE_TOKEN_DEF = 1,
} IMAGE_AUX_SYMBOL_TYPE;
#pragma warning(disable:4103)
#pragma pack(push,2)
typedef struct IMAGE_AUX_SYMBOL_TOKEN_DEF {
 BYTE bAuxType;
 BYTE bReserved;
 DWORD SymbolTableIndex;
 BYTE rgbReserved[12];
} IMAGE_AUX_SYMBOL_TOKEN_DEF;
typedef IMAGE_AUX_SYMBOL_TOKEN_DEF *PIMAGE_AUX_SYMBOL_TOKEN_DEF;
#pragma warning(disable:4103)
#pragma pack(pop)
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
#pragma warning(disable:4103)
#pragma pack(pop)
typedef struct _IMAGE_BASE_RELOCATION {
 DWORD VirtualAddress;
 DWORD SizeOfBlock;
} IMAGE_BASE_RELOCATION;
typedef IMAGE_BASE_RELOCATION * PIMAGE_BASE_RELOCATION;
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
#pragma warning(disable:4103)
#pragma pack(push,8)
typedef struct _IMAGE_THUNK_DATA64 {
 union {
 ULONGLONG ForwarderString;
 ULONGLONG Function;
 ULONGLONG Ordinal;
 ULONGLONG AddressOfData;
 } u1;
} IMAGE_THUNK_DATA64;
typedef IMAGE_THUNK_DATA64 * PIMAGE_THUNK_DATA64;
#pragma warning(disable:4103)
#pragma pack(pop)
typedef struct _IMAGE_THUNK_DATA32 {
 union {
 DWORD ForwarderString;
 DWORD Function;
 DWORD Ordinal;
 DWORD AddressOfData;
 } u1;
} IMAGE_THUNK_DATA32;
typedef IMAGE_THUNK_DATA32 * PIMAGE_THUNK_DATA32;
typedef void
(__stdcall *PIMAGE_TLS_CALLBACK) (
 PVOID DllHandle,
 DWORD Reason,
 PVOID Reserved
 );
typedef struct _IMAGE_TLS_DIRECTORY64 {
 ULONGLONG StartAddressOfRawData;
 ULONGLONG EndAddressOfRawData;
 ULONGLONG AddressOfIndex;
 ULONGLONG AddressOfCallBacks;
 DWORD SizeOfZeroFill;
 DWORD Characteristics;
} IMAGE_TLS_DIRECTORY64;
typedef IMAGE_TLS_DIRECTORY64 * PIMAGE_TLS_DIRECTORY64;
typedef struct _IMAGE_TLS_DIRECTORY32 {
 DWORD StartAddressOfRawData;
 DWORD EndAddressOfRawData;
 DWORD AddressOfIndex;
 DWORD AddressOfCallBacks;
 DWORD SizeOfZeroFill;
 DWORD Characteristics;
} IMAGE_TLS_DIRECTORY32;
typedef IMAGE_TLS_DIRECTORY32 * PIMAGE_TLS_DIRECTORY32;
typedef IMAGE_THUNK_DATA32 IMAGE_THUNK_DATA;
typedef PIMAGE_THUNK_DATA32 PIMAGE_THUNK_DATA;
typedef IMAGE_TLS_DIRECTORY32 IMAGE_TLS_DIRECTORY;
typedef PIMAGE_TLS_DIRECTORY32 PIMAGE_TLS_DIRECTORY;
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
typedef struct {
 DWORD Size;
 DWORD TimeDateStamp;
 WORD MajorVersion;
 WORD MinorVersion;
 DWORD GlobalFlagsClear;
 DWORD GlobalFlagsSet;
 DWORD CriticalSectionDefaultTimeout;
 DWORD DeCommitFreeBlockThreshold;
 DWORD DeCommitTotalFreeThreshold;
 DWORD LockPrefixTable;
 DWORD MaximumAllocationSize;
 DWORD VirtualMemoryThreshold;
 DWORD ProcessHeapFlags;
 DWORD ProcessAffinityMask;
 WORD CSDVersion;
 WORD Reserved1;
 DWORD EditList;
 DWORD SecurityCookie;
 DWORD SEHandlerTable;
 DWORD SEHandlerCount;
} IMAGE_LOAD_CONFIG_DIRECTORY32, *PIMAGE_LOAD_CONFIG_DIRECTORY32;
typedef struct {
 DWORD Size;
 DWORD TimeDateStamp;
 WORD MajorVersion;
 WORD MinorVersion;
 DWORD GlobalFlagsClear;
 DWORD GlobalFlagsSet;
 DWORD CriticalSectionDefaultTimeout;
 ULONGLONG DeCommitFreeBlockThreshold;
 ULONGLONG DeCommitTotalFreeThreshold;
 ULONGLONG LockPrefixTable;
 ULONGLONG MaximumAllocationSize;
 ULONGLONG VirtualMemoryThreshold;
 ULONGLONG ProcessAffinityMask;
 DWORD ProcessHeapFlags;
 WORD CSDVersion;
 WORD Reserved1;
 ULONGLONG EditList;
 ULONGLONG SecurityCookie;
 ULONGLONG SEHandlerTable;
 ULONGLONG SEHandlerCount;
} IMAGE_LOAD_CONFIG_DIRECTORY64, *PIMAGE_LOAD_CONFIG_DIRECTORY64;
typedef IMAGE_LOAD_CONFIG_DIRECTORY32 IMAGE_LOAD_CONFIG_DIRECTORY;
typedef PIMAGE_LOAD_CONFIG_DIRECTORY32 PIMAGE_LOAD_CONFIG_DIRECTORY;
typedef struct _IMAGE_CE_RUNTIME_FUNCTION_ENTRY {
 DWORD FuncStart;
 DWORD PrologLen : 8;
 DWORD FuncLen : 22;
 DWORD ThirtyTwoBit : 1;
 DWORD ExceptionFlag : 1;
} IMAGE_CE_RUNTIME_FUNCTION_ENTRY, * PIMAGE_CE_RUNTIME_FUNCTION_ENTRY;
typedef struct _IMAGE_ALPHA64_RUNTIME_FUNCTION_ENTRY {
 ULONGLONG BeginAddress;
 ULONGLONG EndAddress;
 ULONGLONG ExceptionHandler;
 ULONGLONG HandlerData;
 ULONGLONG PrologEndAddress;
} IMAGE_ALPHA64_RUNTIME_FUNCTION_ENTRY, *PIMAGE_ALPHA64_RUNTIME_FUNCTION_ENTRY;
typedef struct _IMAGE_ALPHA_RUNTIME_FUNCTION_ENTRY {
 DWORD BeginAddress;
 DWORD EndAddress;
 DWORD ExceptionHandler;
 DWORD HandlerData;
 DWORD PrologEndAddress;
} IMAGE_ALPHA_RUNTIME_FUNCTION_ENTRY, *PIMAGE_ALPHA_RUNTIME_FUNCTION_ENTRY;
typedef struct _IMAGE_RUNTIME_FUNCTION_ENTRY {
 DWORD BeginAddress;
 DWORD EndAddress;
 DWORD UnwindInfoAddress;
} _IMAGE_RUNTIME_FUNCTION_ENTRY, *_PIMAGE_RUNTIME_FUNCTION_ENTRY;
typedef _IMAGE_RUNTIME_FUNCTION_ENTRY IMAGE_IA64_RUNTIME_FUNCTION_ENTRY;
typedef _PIMAGE_RUNTIME_FUNCTION_ENTRY PIMAGE_IA64_RUNTIME_FUNCTION_ENTRY;
typedef _IMAGE_RUNTIME_FUNCTION_ENTRY IMAGE_RUNTIME_FUNCTION_ENTRY;
typedef _PIMAGE_RUNTIME_FUNCTION_ENTRY PIMAGE_RUNTIME_FUNCTION_ENTRY;
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
typedef struct _IMAGE_FUNCTION_ENTRY {
 DWORD StartingAddress;
 DWORD EndingAddress;
 DWORD EndOfPrologue;
} IMAGE_FUNCTION_ENTRY, *PIMAGE_FUNCTION_ENTRY;
typedef struct _IMAGE_FUNCTION_ENTRY64 {
 ULONGLONG StartingAddress;
 ULONGLONG EndingAddress;
 union {
 ULONGLONG EndOfPrologue;
 ULONGLONG UnwindInfoAddress;
 };
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
typedef struct _NON_PAGED_DEBUG_INFO {
 WORD Signature;
 WORD Flags;
 DWORD Size;
 WORD Machine;
 WORD Characteristics;
 DWORD TimeDateStamp;
 DWORD CheckSum;
 DWORD SizeOfImage;
 ULONGLONG ImageBase;
} NON_PAGED_DEBUG_INFO, *PNON_PAGED_DEBUG_INFO;
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
#pragma warning(disable:4103)
#pragma pack(pop)
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
 IMPORT_OBJECT_CODE = 0,
 IMPORT_OBJECT_DATA = 1,
 IMPORT_OBJECT_CONST = 2,
} IMPORT_OBJECT_TYPE;
typedef enum IMPORT_OBJECT_NAME_TYPE
{
 IMPORT_OBJECT_ORDINAL = 0,
 IMPORT_OBJECT_NAME = 1,
 IMPORT_OBJECT_NAME_NO_PREFIX = 2,
 IMPORT_OBJECT_NAME_UNDECORATE = 3,
} IMPORT_OBJECT_NAME_TYPE;
typedef enum ReplacesCorHdrNumericDefines
{
 COMIMAGE_FLAGS_ILONLY =0x00000001,
 COMIMAGE_FLAGS_32BITREQUIRED =0x00000002,
 COMIMAGE_FLAGS_IL_LIBRARY =0x00000004,
 COMIMAGE_FLAGS_STRONGNAMESIGNED =0x00000008,
 COMIMAGE_FLAGS_TRACKDEBUGDATA =0x00010000,
 COR_VERSION_MAJOR_V2 =2,
 COR_VERSION_MAJOR =COR_VERSION_MAJOR_V2,
 COR_VERSION_MINOR =0,
 COR_DELETED_NAME_LENGTH =8,
 COR_VTABLEGAP_NAME_LENGTH =8,
 NATIVE_TYPE_MAX_CB =1,
 COR_ILMETHOD_SECT_SMALL_MAX_DATASIZE=0xFF,
 IMAGE_COR_MIH_METHODRVA =0x01,
 IMAGE_COR_MIH_EHRVA =0x02,
 IMAGE_COR_MIH_BASICBLOCK =0x08,
 COR_VTABLE_32BIT =0x01,
 COR_VTABLE_64BIT =0x02,
 COR_VTABLE_FROM_UNMANAGED =0x04,
 COR_VTABLE_FROM_UNMANAGED_RETAIN_APPDOMAIN =0x08,
 COR_VTABLE_CALL_MOST_DERIVED =0x10,
 IMAGE_COR_EATJ_THUNK_SIZE =32,
 MAX_CLASS_NAME =1024,
 MAX_PACKAGE_NAME =1024,
} ReplacesCorHdrNumericDefines;
typedef struct IMAGE_COR20_HEADER
{
 DWORD cb;
 WORD MajorRuntimeVersion;
 WORD MinorRuntimeVersion;
 IMAGE_DATA_DIRECTORY MetaData;
 DWORD Flags;
 DWORD EntryPointToken;
 IMAGE_DATA_DIRECTORY Resources;
 IMAGE_DATA_DIRECTORY StrongNameSignature;
 IMAGE_DATA_DIRECTORY CodeManagerTable;
 IMAGE_DATA_DIRECTORY VTableFixups;
 IMAGE_DATA_DIRECTORY ExportAddressTableJumps;
 IMAGE_DATA_DIRECTORY ManagedNativeHeader;
} IMAGE_COR20_HEADER, *PIMAGE_COR20_HEADER;
typedef union _SLIST_HEADER {
 ULONGLONG Alignment;
 struct {
 SINGLE_LIST_ENTRY Next;
 WORD Depth;
 WORD Sequence;
 };
} SLIST_HEADER, *PSLIST_HEADER;
__declspec(dllimport)
void
__stdcall
RtlInitializeSListHead (
 PSLIST_HEADER ListHead
 );
__declspec(dllimport)
PSINGLE_LIST_ENTRY
__stdcall
RtlFirstEntrySList (
 const SLIST_HEADER *ListHead
 );
__declspec(dllimport)
PSINGLE_LIST_ENTRY
__stdcall
RtlInterlockedPopEntrySList (
 PSLIST_HEADER ListHead
 );
__declspec(dllimport)
PSINGLE_LIST_ENTRY
__stdcall
RtlInterlockedPushEntrySList (
 PSLIST_HEADER ListHead,
 PSINGLE_LIST_ENTRY ListEntry
 );
__declspec(dllimport)
PSINGLE_LIST_ENTRY
__stdcall
RtlInterlockedFlushSList (
 PSLIST_HEADER ListHead
 );
__declspec(dllimport)
WORD
__stdcall
RtlQueryDepthSList (
 PSLIST_HEADER ListHead
 );
typedef union _RTL_RUN_ONCE {
 PVOID Ptr;
} RTL_RUN_ONCE, *PRTL_RUN_ONCE;
typedef
DWORD
(__stdcall *PRTL_RUN_ONCE_INIT_FN) (
 PRTL_RUN_ONCE RunOnce,
 PVOID Parameter,
 PVOID *Context
 );
void
RtlRunOnceInitialize (
 PRTL_RUN_ONCE RunOnce
 );
DWORD
RtlRunOnceExecuteOnce (
 PRTL_RUN_ONCE RunOnce,
 PRTL_RUN_ONCE_INIT_FN InitFn,
 PVOID Parameter,
 PVOID *Context
 );
DWORD
RtlRunOnceBeginInitialize (
 PRTL_RUN_ONCE RunOnce,
 DWORD Flags,
 PVOID *Context
 );
DWORD
RtlRunOnceComplete (
 PRTL_RUN_ONCE RunOnce,
 DWORD Flags,
 PVOID Context
 );
__forceinline
DWORD
HEAP_MAKE_TAG_FLAGS (
 DWORD TagBase,
 DWORD Tag
 )
{
 __pragma(warning(push)) __pragma(warning(disable : 4548)) do {__noop(TagBase);} while((0,0) __pragma(warning(pop)) );
 return ((DWORD)((TagBase) + ((Tag) << 18)));
}
__declspec(dllimport)
WORD
__stdcall
RtlCaptureStackBackTrace(
 DWORD FramesToSkip,
 DWORD FramesToCapture,
 PVOID *BackTrace,
 PDWORD BackTraceHash
 );
__declspec(dllimport)
void
__stdcall
RtlCaptureContext (
 PCONTEXT ContextRecord
 );
__declspec(dllimport)
SIZE_T
__stdcall
RtlCompareMemory (
 const void *Source1,
 const void *Source2,
 SIZE_T Length
 );
__forceinline
PVOID
RtlSecureZeroMemory(
 PVOID ptr,
 SIZE_T cnt
 )
{
 volatile char *vptr = (volatile char *)ptr;
 while (cnt) {
 *vptr = 0;
 vptr++;
 cnt--;
 }
 return ptr;
}
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
} OSVERSIONINFOW, *POSVERSIONINFOW, *LPOSVERSIONINFOW, RTL_OSVERSIONINFOW, *PRTL_OSVERSIONINFOW;
typedef OSVERSIONINFOA OSVERSIONINFO;
typedef POSVERSIONINFOA POSVERSIONINFO;
typedef LPOSVERSIONINFOA LPOSVERSIONINFO;
typedef struct _OSVERSIONINFOEXA {
 DWORD dwOSVersionInfoSize;
 DWORD dwMajorVersion;
 DWORD dwMinorVersion;
 DWORD dwBuildNumber;
 DWORD dwPlatformId;
 CHAR szCSDVersion[ 128 ];
 WORD wServicePackMajor;
 WORD wServicePackMinor;
 WORD wSuiteMask;
 BYTE wProductType;
 BYTE wReserved;
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
 WORD wSuiteMask;
 BYTE wProductType;
 BYTE wReserved;
} OSVERSIONINFOEXW, *POSVERSIONINFOEXW, *LPOSVERSIONINFOEXW, RTL_OSVERSIONINFOEXW, *PRTL_OSVERSIONINFOEXW;
typedef OSVERSIONINFOEXA OSVERSIONINFOEX;
typedef POSVERSIONINFOEXA POSVERSIONINFOEX;
typedef LPOSVERSIONINFOEXA LPOSVERSIONINFOEX;
__declspec(dllimport)
ULONGLONG
__stdcall
VerSetConditionMask(
 ULONGLONG ConditionMask,
 DWORD TypeMask,
 BYTE Condition
 );
__declspec(dllimport)
BOOLEAN
__stdcall
RtlGetProductInfo(
 DWORD OSMajorVersion,
 DWORD OSMinorVersion,
 DWORD SpMajorVersion,
 DWORD SpMinorVersion,
 PDWORD ReturnedProductType
 );
typedef struct _RTL_CRITICAL_SECTION_DEBUG {
 WORD Type;
 WORD CreatorBackTraceIndex;
 struct _RTL_CRITICAL_SECTION *CriticalSection;
 LIST_ENTRY ProcessLocksList;
 DWORD EntryCount;
 DWORD ContentionCount;
 DWORD Flags;
 WORD CreatorBackTraceIndexHigh;
 WORD SpareWORD ;
} RTL_CRITICAL_SECTION_DEBUG, *PRTL_CRITICAL_SECTION_DEBUG, RTL_RESOURCE_DEBUG, *PRTL_RESOURCE_DEBUG;
#pragma pack(push, 8)
typedef struct _RTL_CRITICAL_SECTION {
 PRTL_CRITICAL_SECTION_DEBUG DebugInfo;
 LONG LockCount;
 LONG RecursionCount;
 HANDLE OwningThread;
 HANDLE LockSemaphore;
 ULONG_PTR SpinCount;
} RTL_CRITICAL_SECTION, *PRTL_CRITICAL_SECTION;
#pragma pack(pop)
typedef struct _RTL_SRWLOCK {
 PVOID Ptr;
} RTL_SRWLOCK, *PRTL_SRWLOCK;
typedef struct _RTL_CONDITION_VARIABLE {
 PVOID Ptr;
} RTL_CONDITION_VARIABLE, *PRTL_CONDITION_VARIABLE;
typedef LONG (__stdcall *PVECTORED_EXCEPTION_HANDLER)(
 struct _EXCEPTION_POINTERS *ExceptionInfo
 );
typedef enum _HEAP_INFORMATION_CLASS {
 HeapCompatibilityInformation,
 HeapEnableTerminationOnCorruption
} HEAP_INFORMATION_CLASS;
typedef void (__stdcall * WAITORTIMERCALLBACKFUNC) (PVOID, BOOLEAN );
typedef void (__stdcall * WORKERCALLBACKFUNC) (PVOID );
typedef void (__stdcall * APC_CALLBACK_FUNCTION) (DWORD , PVOID, PVOID);
typedef
void
(__stdcall *PFLS_CALLBACK_FUNCTION) (
 PVOID lpFlsData
 );
typedef enum _ACTIVATION_CONTEXT_INFO_CLASS {
 ActivationContextBasicInformation = 1,
 ActivationContextDetailedInformation = 2,
 AssemblyDetailedInformationInActivationContext = 3,
 FileInformationInAssemblyOfAssemblyInActivationContext = 4,
 RunlevelInformationInActivationContext = 5,
 MaxActivationContextInfoClass,
 AssemblyDetailedInformationInActivationContxt = 3,
 FileInformationInAssemblyOfAssemblyInActivationContxt = 4
} ACTIVATION_CONTEXT_INFO_CLASS;
typedef struct _ACTIVATION_CONTEXT_QUERY_INDEX {
 DWORD ulAssemblyIndex;
 DWORD ulFileIndexInAssembly;
} ACTIVATION_CONTEXT_QUERY_INDEX, * PACTIVATION_CONTEXT_QUERY_INDEX;
typedef const struct _ACTIVATION_CONTEXT_QUERY_INDEX * PCACTIVATION_CONTEXT_QUERY_INDEX;
typedef struct _ASSEMBLY_FILE_DETAILED_INFORMATION {
 DWORD ulFlags;
 DWORD ulFilenameLength;
 DWORD ulPathLength;
 PCWSTR lpFileName;
 PCWSTR lpFilePath;
} ASSEMBLY_FILE_DETAILED_INFORMATION, *PASSEMBLY_FILE_DETAILED_INFORMATION;
typedef const ASSEMBLY_FILE_DETAILED_INFORMATION *PCASSEMBLY_FILE_DETAILED_INFORMATION;
typedef struct _ACTIVATION_CONTEXT_ASSEMBLY_DETAILED_INFORMATION {
 DWORD ulFlags;
 DWORD ulEncodedAssemblyIdentityLength;
 DWORD ulManifestPathType;
 DWORD ulManifestPathLength;
 LARGE_INTEGER liManifestLastWriteTime;
 DWORD ulPolicyPathType;
 DWORD ulPolicyPathLength;
 LARGE_INTEGER liPolicyLastWriteTime;
 DWORD ulMetadataSatelliteRosterIndex;
 DWORD ulManifestVersionMajor;
 DWORD ulManifestVersionMinor;
 DWORD ulPolicyVersionMajor;
 DWORD ulPolicyVersionMinor;
 DWORD ulAssemblyDirectoryNameLength;
 PCWSTR lpAssemblyEncodedAssemblyIdentity;
 PCWSTR lpAssemblyManifestPath;
 PCWSTR lpAssemblyPolicyPath;
 PCWSTR lpAssemblyDirectoryName;
 DWORD ulFileCount;
} ACTIVATION_CONTEXT_ASSEMBLY_DETAILED_INFORMATION, * PACTIVATION_CONTEXT_ASSEMBLY_DETAILED_INFORMATION;
typedef const struct _ACTIVATION_CONTEXT_ASSEMBLY_DETAILED_INFORMATION * PCACTIVATION_CONTEXT_ASSEMBLY_DETAILED_INFORMATION ;
typedef enum
{
 ACTCTX_RUN_LEVEL_UNSPECIFIED = 0,
 ACTCTX_RUN_LEVEL_AS_INVOKER,
 ACTCTX_RUN_LEVEL_HIGHEST_AVAILABLE,
 ACTCTX_RUN_LEVEL_REQUIRE_ADMIN,
 ACTCTX_RUN_LEVEL_NUMBERS
} ACTCTX_REQUESTED_RUN_LEVEL;
typedef struct _ACTIVATION_CONTEXT_RUN_LEVEL_INFORMATION {
 DWORD ulFlags;
 ACTCTX_REQUESTED_RUN_LEVEL RunLevel;
 DWORD UiAccess;
} ACTIVATION_CONTEXT_RUN_LEVEL_INFORMATION, * PACTIVATION_CONTEXT_RUN_LEVEL_INFORMATION;
typedef const struct _ACTIVATION_CONTEXT_RUN_LEVEL_INFORMATION * PCACTIVATION_CONTEXT_RUN_LEVEL_INFORMATION ;
typedef struct _ACTIVATION_CONTEXT_DETAILED_INFORMATION {
 DWORD dwFlags;
 DWORD ulFormatVersion;
 DWORD ulAssemblyCount;
 DWORD ulRootManifestPathType;
 DWORD ulRootManifestPathChars;
 DWORD ulRootConfigurationPathType;
 DWORD ulRootConfigurationPathChars;
 DWORD ulAppDirPathType;
 DWORD ulAppDirPathChars;
 PCWSTR lpRootManifestPath;
 PCWSTR lpRootConfigurationPath;
 PCWSTR lpAppDirPath;
} ACTIVATION_CONTEXT_DETAILED_INFORMATION, *PACTIVATION_CONTEXT_DETAILED_INFORMATION;
typedef const struct _ACTIVATION_CONTEXT_DETAILED_INFORMATION *PCACTIVATION_CONTEXT_DETAILED_INFORMATION;
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
#pragma warning(push)
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
#pragma warning(pop)
typedef enum _CM_SERVICE_NODE_TYPE {
 DriverType = 0x00000001,
 FileSystemType = 0x00000002,
 Win32ServiceOwnProcess = 0x00000010,
 Win32ServiceShareProcess = 0x00000020,
 AdapterType = 0x00000004,
 RecognizerType = 0x00000008
} SERVICE_NODE_TYPE;
typedef enum _CM_SERVICE_LOAD_TYPE {
 BootLoad = 0x00000000,
 SystemLoad = 0x00000001,
 AutoLoad = 0x00000002,
 DemandLoad = 0x00000003,
 DisableLoad = 0x00000004
} SERVICE_LOAD_TYPE;
typedef enum _CM_ERROR_CONTROL_TYPE {
 IgnoreError = 0x00000000,
 NormalError = 0x00000001,
 SevereError = 0x00000002,
 CriticalError = 0x00000003
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
typedef struct _TAPE_WMI_OPERATIONS {
 DWORD Method;
 DWORD DataBufferSize;
 PVOID DataBuffer;
} TAPE_WMI_OPERATIONS, *PTAPE_WMI_OPERATIONS;
typedef enum _TAPE_DRIVE_PROBLEM_TYPE {
 TapeDriveProblemNone, TapeDriveReadWriteWarning,
 TapeDriveReadWriteError, TapeDriveReadWarning,
 TapeDriveWriteWarning, TapeDriveReadError,
 TapeDriveWriteError, TapeDriveHardwareError,
 TapeDriveUnsupportedMedia, TapeDriveScsiConnectionError,
 TapeDriveTimetoClean, TapeDriveCleanDriveNow,
 TapeDriveMediaLifeExpired, TapeDriveSnappedTape
} TAPE_DRIVE_PROBLEM_TYPE;
extern "C" {
extern "C" {
typedef GUID UOW, *PUOW;
typedef GUID CRM_PROTOCOL_ID, *PCRM_PROTOCOL_ID;
typedef ULONG NOTIFICATION_MASK;
typedef struct _TRANSACTION_NOTIFICATION {
 PVOID TransactionKey;
 ULONG TransactionNotification;
 LARGE_INTEGER TmVirtualClock;
 ULONG ArgumentLength;
} TRANSACTION_NOTIFICATION, *PTRANSACTION_NOTIFICATION;
typedef struct _TRANSACTION_NOTIFICATION_RECOVERY_ARGUMENT {
 GUID EnlistmentId;
 UOW UOW;
} TRANSACTION_NOTIFICATION_RECOVERY_ARGUMENT, *PTRANSACTION_NOTIFICATION_RECOVERY_ARGUMENT;
typedef ULONG SAVEPOINT_ID, *PSAVEPOINT_ID;
typedef struct _TRANSACTION_NOTIFICATION_SAVEPOINT_ARGUMENT {
 SAVEPOINT_ID SavepointId;
} TRANSACTION_NOTIFICATION_SAVEPOINT_ARGUMENT, *PTRANSACTION_NOTIFICATION_SAVEPOINT_ARGUMENT;
typedef struct _TRANSACTION_NOTIFICATION_PROPAGATE_ARGUMENT {
 ULONG PropagationCookie;
 GUID UOW;
 GUID TmIdentity;
 ULONG BufferLength;
} TRANSACTION_NOTIFICATION_PROPAGATE_ARGUMENT, *PTRANSACTION_NOTIFICATION_PROPAGATE_ARGUMENT;
typedef struct _TRANSACTION_NOTIFICATION_MARSHAL_ARGUMENT {
 ULONG MarshalCookie;
 GUID UOW;
} TRANSACTION_NOTIFICATION_MARSHAL_ARGUMENT, *PTRANSACTION_NOTIFICATION_MARSHAL_ARGUMENT;
typedef TRANSACTION_NOTIFICATION_PROPAGATE_ARGUMENT TRANSACTION_NOTIFICATION_PROMOTE_ARGUMENT, *PTRANSACTION_NOTIFICATION_PROMOTE_ARGUMENT;
typedef struct _KCRM_MARSHAL_HEADER {
 ULONG VersionMajor;
 ULONG VersionMinor;
 ULONG NumProtocols;
 ULONG Unused;
} KCRM_MARSHAL_HEADER, *PKCRM_MARSHAL_HEADER, * PRKCRM_MARSHAL_HEADER;
typedef struct _KCRM_TRANSACTION_BLOB {
 UOW UOW;
 GUID TmIdentity;
 ULONG IsolationLevel;
 ULONG IsolationFlags;
 ULONG Timeout;
 WCHAR Description[64];
} KCRM_TRANSACTION_BLOB, *PKCRM_TRANSACTION_BLOB, * PRKCRM_TRANSACTION_BLOB;
typedef struct _KCRM_PROTOCOL_BLOB {
 CRM_PROTOCOL_ID ProtocolId;
 ULONG StaticInfoLength;
 ULONG TransactionIdInfoLength;
 ULONG Unused1;
 ULONG Unused2;
} KCRM_PROTOCOL_BLOB, *PKCRM_PROTOCOL_BLOB, * PRKCRM_PROTOCOL_BLOB;
}
typedef enum _TRANSACTION_OUTCOME {
 TransactionOutcomeUndetermined = 1,
 TransactionOutcomeCommitted,
 TransactionOutcomeAborted,
} TRANSACTION_OUTCOME;
typedef enum _TRANSACTION_STATE {
 TransactionStateNormal = 1,
 TransactionStateIndoubt,
 TransactionStateCommittedNotify,
} TRANSACTION_STATE;
typedef struct _TRANSACTION_BASIC_INFORMATION {
 GUID TransactionId;
 DWORD State;
 DWORD Outcome;
} TRANSACTION_BASIC_INFORMATION, *PTRANSACTION_BASIC_INFORMATION;
typedef struct _TRANSACTIONMANAGER_BASIC_INFORMATION {
 GUID TmIdentity;
 LARGE_INTEGER VirtualClock;
} TRANSACTIONMANAGER_BASIC_INFORMATION, *PTRANSACTIONMANAGER_BASIC_INFORMATION;
typedef struct _TRANSACTIONMANAGER_LOG_INFORMATION {
 GUID LogIdentity;
} TRANSACTIONMANAGER_LOG_INFORMATION, *PTRANSACTIONMANAGER_LOG_INFORMATION;
typedef struct _TRANSACTIONMANAGER_LOGPATH_INFORMATION {
 DWORD LogPathLength;
 WCHAR LogPath[1];
} TRANSACTIONMANAGER_LOGPATH_INFORMATION, *PTRANSACTIONMANAGER_LOGPATH_INFORMATION;
typedef struct _TRANSACTION_PROPERTIES_INFORMATION {
 DWORD IsolationLevel;
 DWORD IsolationFlags;
 LARGE_INTEGER Timeout;
 DWORD Outcome;
 DWORD DescriptionLength;
 WCHAR Description[1];
} TRANSACTION_PROPERTIES_INFORMATION, *PTRANSACTION_PROPERTIES_INFORMATION;
typedef struct _TRANSACTION_BIND_INFORMATION {
 HANDLE TmHandle;
} TRANSACTION_BIND_INFORMATION, *PTRANSACTION_BIND_INFORMATION;
typedef struct _TRANSACTION_ENLISTMENT_PAIR {
 GUID EnlistmentId;
 GUID ResourceManagerId;
} TRANSACTION_ENLISTMENT_PAIR, *PTRANSACTION_ENLISTMENT_PAIR;
typedef struct _TRANSACTION_ENLISTMENTS_INFORMATION {
 DWORD NumberOfEnlistments;
 TRANSACTION_ENLISTMENT_PAIR EnlistmentPair[1];
} TRANSACTION_ENLISTMENTS_INFORMATION, *PTRANSACTION_ENLISTMENTS_INFORMATION;
typedef struct _TRANSACTION_FULL_INFORMATION {
 DWORD NameLength;
} TRANSACTION_FULL_INFORMATION, *PTRANSACTION_FULL_INFORMATION;
typedef struct _RESOURCEMANAGER_BASIC_INFORMATION {
 GUID ResourceManagerId;
 DWORD DescriptionLength;
 WCHAR Description[1];
} RESOURCEMANAGER_BASIC_INFORMATION, *PRESOURCEMANAGER_BASIC_INFORMATION;
typedef struct _RESOURCEMANAGER_COMPLETION_INFORMATION {
 HANDLE IoCompletionPortHandle;
 ULONG_PTR CompletionKey;
} RESOURCEMANAGER_COMPLETION_INFORMATION, *PRESOURCEMANAGER_COMPLETION_INFORMATION;
typedef struct _TRANSACTION_NAME_INFORMATION {
 DWORD NameLength;
 WCHAR Name[1];
} TRANSACTION_NAME_INFORMATION, *PTRANSACTION_NAME_INFORMATION;
typedef enum _TRANSACTION_INFORMATION_CLASS {
 TransactionBasicInformation,
 TransactionPropertiesInformation,
 TransactionEnlistmentInformation,
 TransactionFullInformation
 ,
 TransactionBindInformation
 ,
} TRANSACTION_INFORMATION_CLASS;
typedef enum _TRANSACTIONMANAGER_INFORMATION_CLASS {
 TransactionManagerBasicInformation,
 TransactionManagerLogInformation,
 TransactionManagerLogPathInformation,
 TransactionManagerOnlineProbeInformation
} TRANSACTIONMANAGER_INFORMATION_CLASS;
typedef enum _RESOURCEMANAGER_INFORMATION_CLASS {
 ResourceManagerBasicInformation,
 ResourceManagerCompletionInformation,
 ResourceManagerFullInformation
 ,
 ResourceManagerNameInformation
} RESOURCEMANAGER_INFORMATION_CLASS;
typedef struct _ENLISTMENT_BASIC_INFORMATION {
 GUID EnlistmentId;
 GUID TransactionId;
 GUID ResourceManagerId;
} ENLISTMENT_BASIC_INFORMATION, *PENLISTMENT_BASIC_INFORMATION;
typedef enum _ENLISTMENT_INFORMATION_CLASS {
 EnlistmentBasicInformation,
 EnlistmentRecoveryInformation,
 EnlistmentFullInformation
 ,
 EnlistmentNameInformation
} ENLISTMENT_INFORMATION_CLASS;
typedef struct _TRANSACTION_LIST_ENTRY {
 UOW UOW;
} TRANSACTION_LIST_ENTRY, *PTRANSACTION_LIST_ENTRY;
typedef struct _TRANSACTION_LIST_INFORMATION {
 DWORD NumberOfTransactions;
 TRANSACTION_LIST_ENTRY TransactionInformation[1];
} TRANSACTION_LIST_INFORMATION, *PTRANSACTION_LIST_INFORMATION;
typedef enum _KTMOBJECT_TYPE {
 KTMOBJECT_TRANSACTION,
 KTMOBJECT_TRANSACTION_MANAGER,
 KTMOBJECT_RESOURCE_MANAGER,
 KTMOBJECT_ENLISTMENT,
 KTMOBJECT_INVALID
} KTMOBJECT_TYPE, *PKTMOBJECT_TYPE;
typedef struct _KTMOBJECT_CURSOR {
 GUID LastQuery;
 DWORD ObjectIdCount;
 GUID ObjectIds[1];
} KTMOBJECT_CURSOR, *PKTMOBJECT_CURSOR;
}
typedef DWORD TP_VERSION, *PTP_VERSION;
typedef struct _TP_CALLBACK_INSTANCE TP_CALLBACK_INSTANCE, *PTP_CALLBACK_INSTANCE;
typedef void (__stdcall *PTP_SIMPLE_CALLBACK)(
 PTP_CALLBACK_INSTANCE Instance,
 PVOID Context
 );
typedef struct _TP_POOL TP_POOL, *PTP_POOL;
typedef struct _TP_CLEANUP_GROUP TP_CLEANUP_GROUP, *PTP_CLEANUP_GROUP;
typedef void (__stdcall *PTP_CLEANUP_GROUP_CANCEL_CALLBACK)(
 PVOID ObjectContext,
 PVOID CleanupContext
 );
typedef struct _TP_CALLBACK_ENVIRON {
 TP_VERSION Version;
 PTP_POOL Pool;
 PTP_CLEANUP_GROUP CleanupGroup;
 PTP_CLEANUP_GROUP_CANCEL_CALLBACK CleanupGroupCancelCallback;
 PVOID RaceDll;
 struct _ACTIVATION_CONTEXT *ActivationContext;
 PTP_SIMPLE_CALLBACK FinalizationCallback;
 union {
 DWORD Flags;
 struct {
 DWORD LongFunction : 1;
 DWORD Private : 31;
 } s;
 } u;
} TP_CALLBACK_ENVIRON, *PTP_CALLBACK_ENVIRON;
__forceinline
void
TpInitializeCallbackEnviron(
 PTP_CALLBACK_ENVIRON CallbackEnviron
 )
{
 CallbackEnviron->Version = 1;
 CallbackEnviron->Pool = 0;
 CallbackEnviron->CleanupGroup = 0;
 CallbackEnviron->CleanupGroupCancelCallback = 0;
 CallbackEnviron->RaceDll = 0;
 CallbackEnviron->ActivationContext = 0;
 CallbackEnviron->FinalizationCallback = 0;
 CallbackEnviron->u.Flags = 0;
}
__forceinline
void
TpSetCallbackThreadpool(
 PTP_CALLBACK_ENVIRON CallbackEnviron,
 PTP_POOL Pool
 )
{
 CallbackEnviron->Pool = Pool;
}
__forceinline
void
TpSetCallbackCleanupGroup(
 PTP_CALLBACK_ENVIRON CallbackEnviron,
 PTP_CLEANUP_GROUP CleanupGroup,
 PTP_CLEANUP_GROUP_CANCEL_CALLBACK CleanupGroupCancelCallback
 )
{
 CallbackEnviron->CleanupGroup = CleanupGroup;
 CallbackEnviron->CleanupGroupCancelCallback = CleanupGroupCancelCallback;
}
__forceinline
void
TpSetCallbackActivationContext(
 PTP_CALLBACK_ENVIRON CallbackEnviron,
 struct _ACTIVATION_CONTEXT *ActivationContext
 )
{
 CallbackEnviron->ActivationContext = ActivationContext;
}
__forceinline
void
TpSetCallbackNoActivationContext(
 PTP_CALLBACK_ENVIRON CallbackEnviron
 )
{
 CallbackEnviron->ActivationContext = (struct _ACTIVATION_CONTEXT *)(LONG_PTR) -1;
}
__forceinline
void
TpSetCallbackLongFunction(
 PTP_CALLBACK_ENVIRON CallbackEnviron
 )
{
 CallbackEnviron->u.s.LongFunction = 1;
}
__forceinline
void
TpSetCallbackRaceWithDll(
 PTP_CALLBACK_ENVIRON CallbackEnviron,
 PVOID DllHandle
 )
{
 CallbackEnviron->RaceDll = DllHandle;
}
__forceinline
void
TpSetCallbackFinalizationCallback(
 PTP_CALLBACK_ENVIRON CallbackEnviron,
 PTP_SIMPLE_CALLBACK FinalizationCallback
 )
{
 CallbackEnviron->FinalizationCallback = FinalizationCallback;
}
__forceinline
void
TpDestroyCallbackEnviron(
 PTP_CALLBACK_ENVIRON CallbackEnviron
 )
{
 (CallbackEnviron);
}
typedef struct _TP_WORK TP_WORK, *PTP_WORK;
typedef void (__stdcall *PTP_WORK_CALLBACK)(
 PTP_CALLBACK_INSTANCE Instance,
 PVOID Context,
 PTP_WORK Work
 );
typedef struct _TP_TIMER TP_TIMER, *PTP_TIMER;
typedef void (__stdcall *PTP_TIMER_CALLBACK)(
 PTP_CALLBACK_INSTANCE Instance,
 PVOID Context,
 PTP_TIMER Timer
 );
typedef DWORD TP_WAIT_RESULT;
typedef struct _TP_WAIT TP_WAIT, *PTP_WAIT;
typedef void (__stdcall *PTP_WAIT_CALLBACK)(
 PTP_CALLBACK_INSTANCE Instance,
 PVOID Context,
 PTP_WAIT Wait,
 TP_WAIT_RESULT WaitResult
 );
typedef struct _TP_IO TP_IO, *PTP_IO;
__inline struct _TEB * NtCurrentTeb( void ) { return (struct _TEB *) (ULONG_PTR) __readfsdword (0x18); }
}
typedef UINT_PTR WPARAM;
typedef LONG_PTR LPARAM;
typedef LONG_PTR LRESULT;
struct HWND__ { int unused; }; typedef struct HWND__ *HWND;
struct HHOOK__ { int unused; }; typedef struct HHOOK__ *HHOOK;
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
typedef void * HGDIOBJ;
struct HKEY__ { int unused; }; typedef struct HKEY__ *HKEY;
typedef HKEY *PHKEY;
struct HACCEL__ { int unused; }; typedef struct HACCEL__ *HACCEL;
struct HBITMAP__ { int unused; }; typedef struct HBITMAP__ *HBITMAP;
struct HBRUSH__ { int unused; }; typedef struct HBRUSH__ *HBRUSH;
struct HCOLORSPACE__ { int unused; }; typedef struct HCOLORSPACE__ *HCOLORSPACE;
struct HDC__ { int unused; }; typedef struct HDC__ *HDC;
struct HGLRC__ { int unused; }; typedef struct HGLRC__ *HGLRC;
struct HDESK__ { int unused; }; typedef struct HDESK__ *HDESK;
struct HENHMETAFILE__ { int unused; }; typedef struct HENHMETAFILE__ *HENHMETAFILE;
struct HFONT__ { int unused; }; typedef struct HFONT__ *HFONT;
struct HICON__ { int unused; }; typedef struct HICON__ *HICON;
struct HMENU__ { int unused; }; typedef struct HMENU__ *HMENU;
struct HMETAFILE__ { int unused; }; typedef struct HMETAFILE__ *HMETAFILE;
struct HINSTANCE__ { int unused; }; typedef struct HINSTANCE__ *HINSTANCE;
typedef HINSTANCE HMODULE;
struct HPALETTE__ { int unused; }; typedef struct HPALETTE__ *HPALETTE;
struct HPEN__ { int unused; }; typedef struct HPEN__ *HPEN;
struct HRGN__ { int unused; }; typedef struct HRGN__ *HRGN;
struct HRSRC__ { int unused; }; typedef struct HRSRC__ *HRSRC;
struct HSPRITE__ { int unused; }; typedef struct HSPRITE__ *HSPRITE;
struct HSTR__ { int unused; }; typedef struct HSTR__ *HSTR;
struct HTASK__ { int unused; }; typedef struct HTASK__ *HTASK;
struct HWINSTA__ { int unused; }; typedef struct HWINSTA__ *HWINSTA;
struct HKL__ { int unused; }; typedef struct HKL__ *HKL;
struct HWINEVENTHOOK__ { int unused; }; typedef struct HWINEVENTHOOK__ *HWINEVENTHOOK;
struct HMONITOR__ { int unused; }; typedef struct HMONITOR__ *HMONITOR;
struct HUMPD__ { int unused; }; typedef struct HUMPD__ *HUMPD;
typedef int HFILE;
typedef HICON HCURSOR;
typedef DWORD COLORREF;
typedef DWORD *LPCOLORREF;
typedef struct tagRECT
{
 LONG left;
 LONG top;
 LONG right;
 LONG bottom;
} RECT, *PRECT, *NPRECT, *LPRECT;
typedef const RECT * LPCRECT;
typedef struct _RECTL
{
 LONG left;
 LONG top;
 LONG right;
 LONG bottom;
} RECTL, *PRECTL, *LPRECTL;
typedef const RECTL * LPCRECTL;
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
} POINTS, *PPOINTS, *LPPOINTS;
typedef struct _FILETIME {
 DWORD dwLowDateTime;
 DWORD dwHighDateTime;
} FILETIME, *PFILETIME, *LPFILETIME;
}
#pragma once
extern "C" {
typedef struct _OVERLAPPED {
 ULONG_PTR Internal;
 ULONG_PTR InternalHigh;
 union {
 struct {
 DWORD Offset;
 DWORD OffsetHigh;
 };
 PVOID Pointer;
 };
 HANDLE hEvent;
} OVERLAPPED, *LPOVERLAPPED;
typedef struct _OVERLAPPED_ENTRY {
 ULONG_PTR lpCompletionKey;
 LPOVERLAPPED lpOverlapped;
 ULONG_PTR Internal;
 DWORD dwNumberOfBytesTransferred;
} OVERLAPPED_ENTRY, *LPOVERLAPPED_ENTRY;
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
 LPVOID lpThreadParameter
 );
typedef PTHREAD_START_ROUTINE LPTHREAD_START_ROUTINE;
typedef void (__stdcall *PFIBER_START_ROUTINE)(
 LPVOID lpFiberParameter
 );
typedef PFIBER_START_ROUTINE LPFIBER_START_ROUTINE;
typedef RTL_CRITICAL_SECTION CRITICAL_SECTION;
typedef PRTL_CRITICAL_SECTION PCRITICAL_SECTION;
typedef PRTL_CRITICAL_SECTION LPCRITICAL_SECTION;
typedef RTL_CRITICAL_SECTION_DEBUG CRITICAL_SECTION_DEBUG;
typedef PRTL_CRITICAL_SECTION_DEBUG PCRITICAL_SECTION_DEBUG;
typedef PRTL_CRITICAL_SECTION_DEBUG LPCRITICAL_SECTION_DEBUG;
typedef RTL_RUN_ONCE INIT_ONCE;
typedef PRTL_RUN_ONCE PINIT_ONCE;
typedef PRTL_RUN_ONCE LPINIT_ONCE;
typedef
BOOL
(__stdcall *PINIT_ONCE_FN) (
 PINIT_ONCE InitOnce,
 PVOID Parameter,
 PVOID *Context
 );
__declspec(dllimport)
void
__stdcall
InitOnceInitialize (
 PINIT_ONCE InitOnce
 );
__declspec(dllimport)
BOOL
__stdcall
InitOnceExecuteOnce (
 PINIT_ONCE InitOnce,
 PINIT_ONCE_FN InitFn,
 PVOID Parameter,
 LPVOID *Context
 );
__declspec(dllimport)
BOOL
__stdcall
InitOnceBeginInitialize (
 LPINIT_ONCE lpInitOnce,
 DWORD dwFlags,
 PBOOL fPending,
 LPVOID *lpContext
 );
__declspec(dllimport)
BOOL
__stdcall
InitOnceComplete (
 LPINIT_ONCE lpInitOnce,
 DWORD dwFlags,
 LPVOID lpContext
 );
typedef RTL_SRWLOCK SRWLOCK, *PSRWLOCK;
__declspec(dllimport)
void
__stdcall
InitializeSRWLock (
 PSRWLOCK SRWLock
 );
__declspec(dllimport)
void
__stdcall
ReleaseSRWLockExclusive (
 PSRWLOCK SRWLock
 );
__declspec(dllimport)
void
__stdcall
ReleaseSRWLockShared (
 PSRWLOCK SRWLock
 );
__declspec(dllimport)
void
__stdcall
AcquireSRWLockExclusive (
 PSRWLOCK SRWLock
 );
__declspec(dllimport)
void
__stdcall
AcquireSRWLockShared (
 PSRWLOCK SRWLock
 );
typedef RTL_CONDITION_VARIABLE CONDITION_VARIABLE, *PCONDITION_VARIABLE;
__declspec(dllimport)
void
__stdcall
InitializeConditionVariable (
 PCONDITION_VARIABLE ConditionVariable
 );
__declspec(dllimport)
void
__stdcall
WakeConditionVariable (
 PCONDITION_VARIABLE ConditionVariable
 );
__declspec(dllimport)
void
__stdcall
WakeAllConditionVariable (
 PCONDITION_VARIABLE ConditionVariable
 );
__declspec(dllimport)
BOOL
__stdcall
SleepConditionVariableCS (
 PCONDITION_VARIABLE ConditionVariable,
 PCRITICAL_SECTION CriticalSection,
 DWORD dwMilliseconds
 );
__declspec(dllimport)
BOOL
__stdcall
SleepConditionVariableSRW (
 PCONDITION_VARIABLE ConditionVariable,
 PSRWLOCK SRWLock,
 DWORD dwMilliseconds,
 ULONG Flags
 );
__declspec(dllimport)
PVOID
__stdcall
EncodePointer (
 PVOID Ptr
 );
__declspec(dllimport)
PVOID
__stdcall
DecodePointer (
 PVOID Ptr
 );
__declspec(dllimport)
PVOID
__stdcall
EncodeSystemPointer (
 PVOID Ptr
 );
__declspec(dllimport)
PVOID
__stdcall
DecodeSystemPointer (
 PVOID Ptr
 );
typedef PLDT_ENTRY LPLDT_ENTRY;
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
 DWORD_PTR dwActiveProcessorMask;
 DWORD dwNumberOfProcessors;
 DWORD dwProcessorType;
 DWORD dwAllocationGranularity;
 WORD wProcessorLevel;
 WORD wProcessorRevision;
} SYSTEM_INFO, *LPSYSTEM_INFO;
typedef struct _MEMORYSTATUS {
 DWORD dwLength;
 DWORD dwMemoryLoad;
 SIZE_T dwTotalPhys;
 SIZE_T dwAvailPhys;
 SIZE_T dwTotalPageFile;
 SIZE_T dwAvailPageFile;
 SIZE_T dwTotalVirtual;
 SIZE_T dwAvailVirtual;
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
typedef struct _JIT_DEBUG_INFO {
 DWORD dwSize;
 DWORD dwProcessorArchitecture;
 DWORD dwThreadID;
 DWORD dwReserved0;
 ULONG64 lpExceptionAddress;
 ULONG64 lpExceptionRecord;
 ULONG64 lpContextRecord;
} JIT_DEBUG_INFO, *LPJIT_DEBUG_INFO;
typedef JIT_DEBUG_INFO JIT_DEBUG_INFO32, *LPJIT_DEBUG_INFO32;
typedef JIT_DEBUG_INFO JIT_DEBUG_INFO64, *LPJIT_DEBUG_INFO64;
typedef PCONTEXT LPCONTEXT;
typedef PEXCEPTION_RECORD LPEXCEPTION_RECORD;
typedef PEXCEPTION_POINTERS LPEXCEPTION_POINTERS;
typedef struct _OFSTRUCT {
 BYTE cBytes;
 BYTE fFixedDisk;
 WORD nErrCode;
 WORD Reserved1;
 WORD Reserved2;
 CHAR szPathName[128];
} OFSTRUCT, *LPOFSTRUCT, *POFSTRUCT;
__declspec(dllimport)
LONG
__stdcall
InterlockedIncrement (
 LONG volatile *lpAddend
 );
__declspec(dllimport)
LONG
__stdcall
InterlockedDecrement (
 LONG volatile *lpAddend
 );
__declspec(dllimport)
LONG
__stdcall
InterlockedExchange (
 LONG volatile *Target,
 LONG Value
 );
__declspec(dllimport)
LONG
__stdcall
InterlockedExchangeAdd (
 LONG volatile *Addend,
 LONG Value
 );
__declspec(dllimport)
LONG
__stdcall
InterlockedCompareExchange (
 LONG volatile *Destination,
 LONG Exchange,
 LONG Comperand
 );
__declspec(dllimport)
LONGLONG
__stdcall
InterlockedCompareExchange64 (
 LONGLONG volatile *Destination,
 LONGLONG Exchange,
 LONGLONG Comperand
 );
__forceinline
LONGLONG
InterlockedAnd64 (
 LONGLONG volatile *Destination,
 LONGLONG Value
 )
{
 LONGLONG Old;
 do {
 Old = *Destination;
 } while (InterlockedCompareExchange64(Destination,
 Old & Value,
 Old) != Old);
 return Old;
}
__forceinline
LONGLONG
InterlockedOr64 (
 LONGLONG volatile *Destination,
 LONGLONG Value
 )
{
 LONGLONG Old;
 do {
 Old = *Destination;
 } while (InterlockedCompareExchange64(Destination,
 Old | Value,
 Old) != Old);
 return Old;
}
__forceinline
LONGLONG
InterlockedXor64 (
 LONGLONG volatile *Destination,
 LONGLONG Value
 )
{
 LONGLONG Old;
 do {
 Old = *Destination;
 } while (InterlockedCompareExchange64(Destination,
 Old ^ Value,
 Old) != Old);
 return Old;
}
__forceinline
LONGLONG
InterlockedIncrement64 (
 LONGLONG volatile *Addend
 )
{
 LONGLONG Old;
 do {
 Old = *Addend;
 } while (InterlockedCompareExchange64(Addend,
 Old + 1,
 Old) != Old);
 return Old + 1;
}
__forceinline
LONGLONG
InterlockedDecrement64 (
 LONGLONG volatile *Addend
 )
{
 LONGLONG Old;
 do {
 Old = *Addend;
 } while (InterlockedCompareExchange64(Addend,
 Old - 1,
 Old) != Old);
 return Old - 1;
}
__forceinline
LONGLONG
InterlockedExchange64 (
 LONGLONG volatile *Target,
 LONGLONG Value
 )
{
 LONGLONG Old;
 do {
 Old = *Target;
 } while (InterlockedCompareExchange64(Target,
 Value,
 Old) != Old);
 return Old;
}
__forceinline
LONGLONG
InterlockedExchangeAdd64(
 LONGLONG volatile *Addend,
 LONGLONG Value
 )
{
 LONGLONG Old;
 do {
 Old = *Addend;
 } while (InterlockedCompareExchange64(Addend,
 Old + Value,
 Old) != Old);
 return Old;
}
__forceinline
PVOID
__cdecl
__InlineInterlockedCompareExchangePointer (
 PVOID volatile *Destination,
 PVOID ExChange,
 PVOID Comperand
 )
{
 return((PVOID)(LONG_PTR)InterlockedCompareExchange((LONG volatile *)Destination, (LONG)(LONG_PTR)ExChange, (LONG)(LONG_PTR)Comperand));
}
__declspec(dllimport)
void
__stdcall
InitializeSListHead (
 PSLIST_HEADER ListHead
 );
__declspec(dllimport)
PSINGLE_LIST_ENTRY
__stdcall
InterlockedPopEntrySList (
 PSLIST_HEADER ListHead
 );
__declspec(dllimport)
PSINGLE_LIST_ENTRY
__stdcall
InterlockedPushEntrySList (
 PSLIST_HEADER ListHead,
 PSINGLE_LIST_ENTRY ListEntry
 );
__declspec(dllimport)
PSINGLE_LIST_ENTRY
__stdcall
InterlockedFlushSList (
 PSLIST_HEADER ListHead
 );
__declspec(dllimport)
USHORT
__stdcall
QueryDepthSList (
 PSLIST_HEADER ListHead
 );
__declspec(dllimport)
BOOL
__stdcall
FreeResource(
 HGLOBAL hResData
 );
__declspec(dllimport)
LPVOID
__stdcall
LockResource(
 HGLOBAL hResData
 );
int
__stdcall
WinMain (
 HINSTANCE hInstance,
 HINSTANCE hPrevInstance,
 LPSTR lpCmdLine,
 int nShowCmd
 );
int
__stdcall
wWinMain(
 HINSTANCE hInstance,
 HINSTANCE hPrevInstance,
 LPWSTR lpCmdLine,
 int nShowCmd
 );
__declspec(dllimport)
BOOL
__stdcall
FreeLibrary (
 HMODULE hLibModule
 );
__declspec(dllimport)
__declspec(noreturn)
void
__stdcall
FreeLibraryAndExitThread (
 HMODULE hLibModule,
 DWORD dwExitCode
 );
__declspec(dllimport)
BOOL
__stdcall
DisableThreadLibraryCalls (
 HMODULE hLibModule
 );
__declspec(dllimport)
FARPROC
__stdcall
GetProcAddress (
 HMODULE hModule,
 LPCSTR lpProcName
 );
__declspec(dllimport)
DWORD
__stdcall
GetVersion (
 void
 );
__declspec(dllimport)
HGLOBAL
__stdcall
GlobalAlloc (
 UINT uFlags,
 SIZE_T dwBytes
 );
__declspec(dllimport)
HGLOBAL
__stdcall
GlobalReAlloc (
 HGLOBAL hMem,
 SIZE_T dwBytes,
 UINT uFlags
 );
__declspec(dllimport)
SIZE_T
__stdcall
GlobalSize (
 HGLOBAL hMem
 );
__declspec(dllimport)
UINT
__stdcall
GlobalFlags (
 HGLOBAL hMem
 );
__declspec(dllimport)
LPVOID
__stdcall
GlobalLock (
 HGLOBAL hMem
 );
__declspec(dllimport)
HGLOBAL
__stdcall
GlobalHandle (
 LPCVOID pMem
 );
__declspec(dllimport)
BOOL
__stdcall
GlobalUnlock(
 HGLOBAL hMem
 );
__declspec(dllimport)
HGLOBAL
__stdcall
GlobalFree(
 HGLOBAL hMem
 );
__declspec(dllimport)
SIZE_T
__stdcall
GlobalCompact(
 DWORD dwMinFree
 );
__declspec(dllimport)
void
__stdcall
GlobalFix(
 HGLOBAL hMem
 );
__declspec(dllimport)
void
__stdcall
GlobalUnfix(
 HGLOBAL hMem
 );
__declspec(dllimport)
LPVOID
__stdcall
GlobalWire(
 HGLOBAL hMem
 );
__declspec(dllimport)
BOOL
__stdcall
GlobalUnWire(
 HGLOBAL hMem
 );
__declspec(dllimport)
void
__stdcall
GlobalMemoryStatus(
 LPMEMORYSTATUS lpBuffer
 );
typedef struct _MEMORYSTATUSEX {
 DWORD dwLength;
 DWORD dwMemoryLoad;
 DWORDLONG ullTotalPhys;
 DWORDLONG ullAvailPhys;
 DWORDLONG ullTotalPageFile;
 DWORDLONG ullAvailPageFile;
 DWORDLONG ullTotalVirtual;
 DWORDLONG ullAvailVirtual;
 DWORDLONG ullAvailExtendedVirtual;
} MEMORYSTATUSEX, *LPMEMORYSTATUSEX;
__declspec(dllimport)
BOOL
__stdcall
GlobalMemoryStatusEx(
 LPMEMORYSTATUSEX lpBuffer
 );
__declspec(dllimport)
HLOCAL
__stdcall
LocalAlloc(
 UINT uFlags,
 SIZE_T uBytes
 );
__declspec(dllimport)
HLOCAL
__stdcall
LocalReAlloc(
 HLOCAL hMem,
 SIZE_T uBytes,
 UINT uFlags
 );
__declspec(dllimport)
LPVOID
__stdcall
LocalLock(
 HLOCAL hMem
 );
__declspec(dllimport)
HLOCAL
__stdcall
LocalHandle(
 LPCVOID pMem
 );
__declspec(dllimport)
BOOL
__stdcall
LocalUnlock(
 HLOCAL hMem
 );
__declspec(dllimport)
SIZE_T
__stdcall
LocalSize(
 HLOCAL hMem
 );
__declspec(dllimport)
UINT
__stdcall
LocalFlags(
 HLOCAL hMem
 );
__declspec(dllimport)
HLOCAL
__stdcall
LocalFree(
 HLOCAL hMem
 );
__declspec(dllimport)
SIZE_T
__stdcall
LocalShrink(
 HLOCAL hMem,
 UINT cbNewSize
 );
__declspec(dllimport)
SIZE_T
__stdcall
LocalCompact(
 UINT uMinFree
 );
__declspec(dllimport)
BOOL
__stdcall
FlushInstructionCache(
 HANDLE hProcess,
 LPCVOID lpBaseAddress,
 SIZE_T dwSize
 );
__declspec(dllimport)
void
__stdcall
FlushProcessWriteBuffers(
 void
 );
__declspec(dllimport)
BOOL
__stdcall
QueryThreadCycleTime (
 HANDLE ThreadHandle,
 PULONG64 CycleTime
 );
__declspec(dllimport)
BOOL
__stdcall
QueryProcessCycleTime (
 HANDLE ProcessHandle,
 PULONG64 CycleTime
 );
__declspec(dllimport)
BOOL
__stdcall
QueryIdleProcessorCycleTime (
 PULONG BufferLength,
 PULONG64 ProcessorIdleCycleTime
 );
__declspec(dllimport)
LPVOID
__stdcall
VirtualAlloc(
 LPVOID lpAddress,
 SIZE_T dwSize,
 DWORD flAllocationType,
 DWORD flProtect
 );
__declspec(dllimport)
BOOL
__stdcall
VirtualFree(
 LPVOID lpAddress,
 SIZE_T dwSize,
 DWORD dwFreeType
 );
__declspec(dllimport)
BOOL
__stdcall
VirtualProtect(
 LPVOID lpAddress,
 SIZE_T dwSize,
 DWORD flNewProtect,
 PDWORD lpflOldProtect
 );
__declspec(dllimport)
SIZE_T
__stdcall
VirtualQuery(
 LPCVOID lpAddress,
 PMEMORY_BASIC_INFORMATION lpBuffer,
 SIZE_T dwLength
 );
__declspec(dllimport)
LPVOID
__stdcall
VirtualAllocEx(
 HANDLE hProcess,
 LPVOID lpAddress,
 SIZE_T dwSize,
 DWORD flAllocationType,
 DWORD flProtect
 );
__declspec(dllimport)
LPVOID
__stdcall
VirtualAllocExNuma(
 HANDLE hProcess,
 LPVOID lpAddress,
 SIZE_T dwSize,
 DWORD flAllocationType,
 DWORD flProtect,
 DWORD nndPreferred
 );
__declspec(dllimport)
UINT
__stdcall
GetWriteWatch(
 DWORD dwFlags,
 PVOID lpBaseAddress,
 SIZE_T dwRegionSize,
 PVOID *lpAddresses,
 ULONG_PTR *lpdwCount,
 PULONG lpdwGranularity
 );
__declspec(dllimport)
UINT
__stdcall
ResetWriteWatch(
 LPVOID lpBaseAddress,
 SIZE_T dwRegionSize
 );
__declspec(dllimport)
SIZE_T
__stdcall
GetLargePageMinimum(
 void
 );
__declspec(dllimport)
UINT
__stdcall
EnumSystemFirmwareTables(
 DWORD FirmwareTableProviderSignature,
 PVOID pFirmwareTableEnumBuffer,
 DWORD BufferSize
 );
__declspec(dllimport)
UINT
__stdcall
GetSystemFirmwareTable(
 DWORD FirmwareTableProviderSignature,
 DWORD FirmwareTableID,
 PVOID pFirmwareTableBuffer,
 DWORD BufferSize
 );
__declspec(dllimport)
BOOL
__stdcall
VirtualFreeEx(
 HANDLE hProcess,
 LPVOID lpAddress,
 SIZE_T dwSize,
 DWORD dwFreeType
 );
__declspec(dllimport)
BOOL
__stdcall
VirtualProtectEx(
 HANDLE hProcess,
 LPVOID lpAddress,
 SIZE_T dwSize,
 DWORD flNewProtect,
 PDWORD lpflOldProtect
 );
__declspec(dllimport)
SIZE_T
__stdcall
VirtualQueryEx(
 HANDLE hProcess,
 LPCVOID lpAddress,
 PMEMORY_BASIC_INFORMATION lpBuffer,
 SIZE_T dwLength
 );
__declspec(dllimport)
HANDLE
__stdcall
HeapCreate(
 DWORD flOptions,
 SIZE_T dwInitialSize,
 SIZE_T dwMaximumSize
 );
__declspec(dllimport)
BOOL
__stdcall
HeapDestroy(
 HANDLE hHeap
 );
__declspec(dllimport)
LPVOID
__stdcall
HeapAlloc(
 HANDLE hHeap,
 DWORD dwFlags,
 SIZE_T dwBytes
 );
__declspec(dllimport)
LPVOID
__stdcall
HeapReAlloc(
 HANDLE hHeap,
 DWORD dwFlags,
 LPVOID lpMem,
 SIZE_T dwBytes
 );
__declspec(dllimport)
BOOL
__stdcall
HeapFree(
 HANDLE hHeap,
 DWORD dwFlags,
 LPVOID lpMem
 );
__declspec(dllimport)
SIZE_T
__stdcall
HeapSize(
 HANDLE hHeap,
 DWORD dwFlags,
 LPCVOID lpMem
 );
__declspec(dllimport)
BOOL
__stdcall
HeapValidate(
 HANDLE hHeap,
 DWORD dwFlags,
 LPCVOID lpMem
 );
__declspec(dllimport)
SIZE_T
__stdcall
HeapCompact(
 HANDLE hHeap,
 DWORD dwFlags
 );
__declspec(dllimport)
HANDLE
__stdcall
GetProcessHeap( void );
__declspec(dllimport)
DWORD
__stdcall
GetProcessHeaps(
 DWORD NumberOfHeaps,
 PHANDLE ProcessHeaps
 );
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
__declspec(dllimport)
BOOL
__stdcall
HeapLock(
 HANDLE hHeap
 );
__declspec(dllimport)
BOOL
__stdcall
HeapUnlock(
 HANDLE hHeap
 );
__declspec(dllimport)
BOOL
__stdcall
HeapWalk(
 HANDLE hHeap,
 LPPROCESS_HEAP_ENTRY lpEntry
 );
__declspec(dllimport)
BOOL
__stdcall
HeapSetInformation (
 HANDLE HeapHandle,
 HEAP_INFORMATION_CLASS HeapInformationClass,
 PVOID HeapInformation,
 SIZE_T HeapInformationLength
 );
__declspec(dllimport)
BOOL
__stdcall
HeapQueryInformation (
 HANDLE HeapHandle,
 HEAP_INFORMATION_CLASS HeapInformationClass,
 PVOID HeapInformation,
 SIZE_T HeapInformationLength,
 PSIZE_T ReturnLength
 );
__declspec(dllimport)
BOOL
__stdcall
GetBinaryTypeA(
 LPCSTR lpApplicationName,
 LPDWORD lpBinaryType
 );
__declspec(dllimport)
BOOL
__stdcall
GetBinaryTypeW(
 LPCWSTR lpApplicationName,
 LPDWORD lpBinaryType
 );
__declspec(dllimport)
DWORD
__stdcall
GetShortPathNameA(
 LPCSTR lpszLongPath,
 LPSTR lpszShortPath,
 DWORD cchBuffer
 );
__declspec(dllimport)
DWORD
__stdcall
GetShortPathNameW(
 LPCWSTR lpszLongPath,
 LPWSTR lpszShortPath,
 DWORD cchBuffer
 );
__declspec(dllimport)
DWORD
__stdcall
GetLongPathNameA(
 LPCSTR lpszShortPath,
 LPSTR lpszLongPath,
 DWORD cchBuffer
 );
__declspec(dllimport)
DWORD
__stdcall
GetLongPathNameW(
 LPCWSTR lpszShortPath,
 LPWSTR lpszLongPath,
 DWORD cchBuffer
 );
__declspec(dllimport)
DWORD
__stdcall
GetLongPathNameTransactedA(
 LPCSTR lpszShortPath,
 LPSTR lpszLongPath,
 DWORD cchBuffer,
 HANDLE hTransaction
 );
__declspec(dllimport)
DWORD
__stdcall
GetLongPathNameTransactedW(
 LPCWSTR lpszShortPath,
 LPWSTR lpszLongPath,
 DWORD cchBuffer,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
GetProcessAffinityMask(
 HANDLE hProcess,
 PDWORD_PTR lpProcessAffinityMask,
 PDWORD_PTR lpSystemAffinityMask
 );
__declspec(dllimport)
BOOL
__stdcall
SetProcessAffinityMask(
 HANDLE hProcess,
 DWORD_PTR dwProcessAffinityMask
 );
__declspec(dllimport)
BOOL
__stdcall
GetProcessHandleCount(
 HANDLE hProcess,
 PDWORD pdwHandleCount
 );
__declspec(dllimport)
BOOL
__stdcall
GetProcessTimes(
 HANDLE hProcess,
 LPFILETIME lpCreationTime,
 LPFILETIME lpExitTime,
 LPFILETIME lpKernelTime,
 LPFILETIME lpUserTime
 );
__declspec(dllimport)
BOOL
__stdcall
GetProcessIoCounters(
 HANDLE hProcess,
 PIO_COUNTERS lpIoCounters
 );
__declspec(dllimport)
BOOL
__stdcall
GetProcessWorkingSetSize(
 HANDLE hProcess,
 PSIZE_T lpMinimumWorkingSetSize,
 PSIZE_T lpMaximumWorkingSetSize
 );
__declspec(dllimport)
BOOL
__stdcall
GetProcessWorkingSetSizeEx(
 HANDLE hProcess,
 PSIZE_T lpMinimumWorkingSetSize,
 PSIZE_T lpMaximumWorkingSetSize,
 PDWORD Flags
 );
__declspec(dllimport)
BOOL
__stdcall
SetProcessWorkingSetSize(
 HANDLE hProcess,
 SIZE_T dwMinimumWorkingSetSize,
 SIZE_T dwMaximumWorkingSetSize
 );
__declspec(dllimport)
BOOL
__stdcall
SetProcessWorkingSetSizeEx(
 HANDLE hProcess,
 SIZE_T dwMinimumWorkingSetSize,
 SIZE_T dwMaximumWorkingSetSize,
 DWORD Flags
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenProcess(
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 DWORD dwProcessId
 );
__declspec(dllimport)
HANDLE
__stdcall
GetCurrentProcess(
 void
 );
__declspec(dllimport)
DWORD
__stdcall
GetCurrentProcessId(
 void
 );
__declspec(dllimport)
__declspec(noreturn)
void
__stdcall
ExitProcess(
 UINT uExitCode
 );
__declspec(dllimport)
BOOL
__stdcall
TerminateProcess(
 HANDLE hProcess,
 UINT uExitCode
 );
__declspec(dllimport)
BOOL
__stdcall
GetExitCodeProcess(
 HANDLE hProcess,
 LPDWORD lpExitCode
 );
__declspec(dllimport)
void
__stdcall
FatalExit(
 int ExitCode
 );
__declspec(dllimport)
LPCH
__stdcall
GetEnvironmentStrings(
 void
 );
__declspec(dllimport)
LPWCH
__stdcall
GetEnvironmentStringsW(
 void
 );
__declspec(dllimport)
BOOL
__stdcall
SetEnvironmentStringsA(
 LPCH NewEnvironment
 );
__declspec(dllimport)
BOOL
__stdcall
SetEnvironmentStringsW(
 LPWCH NewEnvironment
 );
__declspec(dllimport)
BOOL
__stdcall
FreeEnvironmentStringsA(
 LPCH
 );
__declspec(dllimport)
BOOL
__stdcall
FreeEnvironmentStringsW(
 LPWCH
 );
__declspec(dllimport)
void
__stdcall
RaiseException(
 DWORD dwExceptionCode,
 DWORD dwExceptionFlags,
 DWORD nNumberOfArguments,
 const ULONG_PTR *lpArguments
 );
__declspec(dllimport)
LONG
__stdcall
UnhandledExceptionFilter(
 struct _EXCEPTION_POINTERS *ExceptionInfo
 );
typedef LONG (__stdcall *PTOP_LEVEL_EXCEPTION_FILTER)(
 struct _EXCEPTION_POINTERS *ExceptionInfo
 );
typedef PTOP_LEVEL_EXCEPTION_FILTER LPTOP_LEVEL_EXCEPTION_FILTER;
__declspec(dllimport)
LPTOP_LEVEL_EXCEPTION_FILTER
__stdcall
SetUnhandledExceptionFilter(
 LPTOP_LEVEL_EXCEPTION_FILTER lpTopLevelExceptionFilter
 );
__declspec(dllimport)
LPVOID
__stdcall
CreateFiber(
 SIZE_T dwStackSize,
 LPFIBER_START_ROUTINE lpStartAddress,
 LPVOID lpParameter
 );
__declspec(dllimport)
LPVOID
__stdcall
CreateFiberEx(
 SIZE_T dwStackCommitSize,
 SIZE_T dwStackReserveSize,
 DWORD dwFlags,
 LPFIBER_START_ROUTINE lpStartAddress,
 LPVOID lpParameter
 );
__declspec(dllimport)
void
__stdcall
DeleteFiber(
 LPVOID lpFiber
 );
__declspec(dllimport)
LPVOID
__stdcall
ConvertThreadToFiber(
 LPVOID lpParameter
 );
__declspec(dllimport)
LPVOID
__stdcall
ConvertThreadToFiberEx(
 LPVOID lpParameter,
 DWORD dwFlags
 );
__declspec(dllimport)
BOOL
__stdcall
ConvertFiberToThread(
 void
 );
__declspec(dllimport)
BOOL
__stdcall
IsThreadAFiber(
 void
 );
__declspec(dllimport)
void
__stdcall
SwitchToFiber(
 LPVOID lpFiber
 );
__declspec(dllimport)
BOOL
__stdcall
SwitchToThread(
 void
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateThread(
 LPSECURITY_ATTRIBUTES lpThreadAttributes,
 SIZE_T dwStackSize,
 LPTHREAD_START_ROUTINE lpStartAddress,
 LPVOID lpParameter,
 DWORD dwCreationFlags,
 LPDWORD lpThreadId
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateRemoteThread(
 HANDLE hProcess,
 LPSECURITY_ATTRIBUTES lpThreadAttributes,
 SIZE_T dwStackSize,
 LPTHREAD_START_ROUTINE lpStartAddress,
 LPVOID lpParameter,
 DWORD dwCreationFlags,
 LPDWORD lpThreadId
 );
__declspec(dllimport)
HANDLE
__stdcall
GetCurrentThread(
 void
 );
__declspec(dllimport)
DWORD
__stdcall
GetCurrentThreadId(
 void
 );
__declspec(dllimport)
BOOL
__stdcall
SetThreadStackGuarantee (
 PULONG StackSizeInBytes
 );
__declspec(dllimport)
DWORD
__stdcall
GetProcessIdOfThread(
 HANDLE Thread
 );
__declspec(dllimport)
DWORD
__stdcall
GetThreadId(
 HANDLE Thread
 );
__declspec(dllimport)
DWORD
__stdcall
GetProcessId(
 HANDLE Process
 );
__declspec(dllimport)
DWORD
__stdcall
GetCurrentProcessorNumber(
 void
 );
__declspec(dllimport)
DWORD_PTR
__stdcall
SetThreadAffinityMask(
 HANDLE hThread,
 DWORD_PTR dwThreadAffinityMask
 );
__declspec(dllimport)
DWORD
__stdcall
SetThreadIdealProcessor(
 HANDLE hThread,
 DWORD dwIdealProcessor
 );
__declspec(dllimport)
BOOL
__stdcall
SetProcessPriorityBoost(
 HANDLE hProcess,
 BOOL bDisablePriorityBoost
 );
__declspec(dllimport)
BOOL
__stdcall
GetProcessPriorityBoost(
 HANDLE hProcess,
 PBOOL pDisablePriorityBoost
 );
__declspec(dllimport)
BOOL
__stdcall
RequestWakeupLatency(
 LATENCY_TIME latency
 );
__declspec(dllimport)
BOOL
__stdcall
IsSystemResumeAutomatic(
 void
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenThread(
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 DWORD dwThreadId
 );
__declspec(dllimport)
BOOL
__stdcall
SetThreadPriority(
 HANDLE hThread,
 int nPriority
 );
__declspec(dllimport)
BOOL
__stdcall
SetThreadPriorityBoost(
 HANDLE hThread,
 BOOL bDisablePriorityBoost
 );
__declspec(dllimport)
BOOL
__stdcall
GetThreadPriorityBoost(
 HANDLE hThread,
 PBOOL pDisablePriorityBoost
 );
__declspec(dllimport)
int
__stdcall
GetThreadPriority(
 HANDLE hThread
 );
__declspec(dllimport)
BOOL
__stdcall
GetThreadTimes(
 HANDLE hThread,
 LPFILETIME lpCreationTime,
 LPFILETIME lpExitTime,
 LPFILETIME lpKernelTime,
 LPFILETIME lpUserTime
 );
__declspec(dllimport)
BOOL
__stdcall
GetThreadIOPendingFlag(
 HANDLE hThread,
 PBOOL lpIOIsPending
 );
__declspec(dllimport)
__declspec(noreturn)
void
__stdcall
ExitThread(
 DWORD dwExitCode
 );
__declspec(dllimport)
BOOL
__stdcall
TerminateThread(
 HANDLE hThread,
 DWORD dwExitCode
 );
__declspec(dllimport)
BOOL
__stdcall
GetExitCodeThread(
 HANDLE hThread,
 LPDWORD lpExitCode
 );
__declspec(dllimport)
BOOL
__stdcall
GetThreadSelectorEntry(
 HANDLE hThread,
 DWORD dwSelector,
 LPLDT_ENTRY lpSelectorEntry
 );
__declspec(dllimport)
EXECUTION_STATE
__stdcall
SetThreadExecutionState(
 EXECUTION_STATE esFlags
 );
__declspec(dllimport)
DWORD
__stdcall
GetLastError(
 void
 );
__declspec(dllimport)
void
__stdcall
SetLastError(
 DWORD dwErrCode
 );
__declspec(dllimport)
BOOL
__stdcall
GetOverlappedResult(
 HANDLE hFile,
 LPOVERLAPPED lpOverlapped,
 LPDWORD lpNumberOfBytesTransferred,
 BOOL bWait
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateIoCompletionPort(
 HANDLE FileHandle,
 HANDLE ExistingCompletionPort,
 ULONG_PTR CompletionKey,
 DWORD NumberOfConcurrentThreads
 );
__declspec(dllimport)
BOOL
__stdcall
GetQueuedCompletionStatus(
 HANDLE CompletionPort,
 LPDWORD lpNumberOfBytesTransferred,
 PULONG_PTR lpCompletionKey,
 LPOVERLAPPED *lpOverlapped,
 DWORD dwMilliseconds
 );
__declspec(dllimport)
BOOL
__stdcall
GetQueuedCompletionStatusEx(
 HANDLE CompletionPort,
 LPOVERLAPPED_ENTRY lpCompletionPortEntries,
 ULONG ulCount,
 PULONG ulNumEntriesRemoved,
 DWORD dwMilliseconds,
 BOOL fAlertable
 );
__declspec(dllimport)
BOOL
__stdcall
PostQueuedCompletionStatus(
 HANDLE CompletionPort,
 DWORD dwNumberOfBytesTransferred,
 ULONG_PTR dwCompletionKey,
 LPOVERLAPPED lpOverlapped
 );
__declspec(dllimport)
BOOL
__stdcall
SetFileCompletionNotificationModes(
 HANDLE FileHandle,
 UCHAR Flags
 );
__declspec(dllimport)
BOOL
__stdcall
SetFileIoOverlappedRange(
 HANDLE FileHandle,
 PUCHAR OverlappedRangeStart,
 ULONG Length
 );
__declspec(dllimport)
UINT
__stdcall
GetErrorMode(
 void
 );
__declspec(dllimport)
UINT
__stdcall
SetErrorMode(
 UINT uMode
 );
__declspec(dllimport)
BOOL
__stdcall
ReadProcessMemory(
 HANDLE hProcess,
 LPCVOID lpBaseAddress,
 LPVOID lpBuffer,
 SIZE_T nSize,
 SIZE_T * lpNumberOfBytesRead
 );
__declspec(dllimport)
BOOL
__stdcall
WriteProcessMemory(
 HANDLE hProcess,
 LPVOID lpBaseAddress,
 LPCVOID lpBuffer,
 SIZE_T nSize,
 SIZE_T * lpNumberOfBytesWritten
 );
__declspec(dllimport)
BOOL
__stdcall
GetThreadContext(
 HANDLE hThread,
 LPCONTEXT lpContext
 );
__declspec(dllimport)
BOOL
__stdcall
SetThreadContext(
 HANDLE hThread,
 const CONTEXT *lpContext
 );
__declspec(dllimport)
BOOL
__stdcall
Wow64GetThreadContext(
 HANDLE hThread,
 PWOW64_CONTEXT lpContext
 );
__declspec(dllimport)
BOOL
__stdcall
Wow64SetThreadContext(
 HANDLE hThread,
 const WOW64_CONTEXT *lpContext
 );
__declspec(dllimport)
DWORD
__stdcall
SuspendThread(
 HANDLE hThread
 );
__declspec(dllimport)
DWORD
__stdcall
Wow64SuspendThread(
 HANDLE hThread
 );
__declspec(dllimport)
DWORD
__stdcall
ResumeThread(
 HANDLE hThread
 );
typedef
void
(__stdcall *PAPCFUNC)(
 ULONG_PTR dwParam
 );
__declspec(dllimport)
DWORD
__stdcall
QueueUserAPC(
 PAPCFUNC pfnAPC,
 HANDLE hThread,
 ULONG_PTR dwData
 );
__declspec(dllimport)
BOOL
__stdcall
IsDebuggerPresent(
 void
 );
__declspec(dllimport)
BOOL
__stdcall
CheckRemoteDebuggerPresent(
 HANDLE hProcess,
 PBOOL pbDebuggerPresent
 );
__declspec(dllimport)
void
__stdcall
DebugBreak(
 void
 );
__declspec(dllimport)
BOOL
__stdcall
WaitForDebugEvent(
 LPDEBUG_EVENT lpDebugEvent,
 DWORD dwMilliseconds
 );
__declspec(dllimport)
BOOL
__stdcall
ContinueDebugEvent(
 DWORD dwProcessId,
 DWORD dwThreadId,
 DWORD dwContinueStatus
 );
__declspec(dllimport)
BOOL
__stdcall
DebugActiveProcess(
 DWORD dwProcessId
 );
__declspec(dllimport)
BOOL
__stdcall
DebugActiveProcessStop(
 DWORD dwProcessId
 );
__declspec(dllimport)
BOOL
__stdcall
DebugSetProcessKillOnExit(
 BOOL KillOnExit
 );
__declspec(dllimport)
BOOL
__stdcall
DebugBreakProcess (
 HANDLE Process
 );
__declspec(dllimport)
void
__stdcall
InitializeCriticalSection(
 LPCRITICAL_SECTION lpCriticalSection
 );
__declspec(dllimport)
void
__stdcall
EnterCriticalSection(
 LPCRITICAL_SECTION lpCriticalSection
 );
__declspec(dllimport)
void
__stdcall
LeaveCriticalSection(
 LPCRITICAL_SECTION lpCriticalSection
 );
__declspec(dllimport)
BOOL
__stdcall
InitializeCriticalSectionAndSpinCount(
 LPCRITICAL_SECTION lpCriticalSection,
 DWORD dwSpinCount
 );
__declspec(dllimport)
BOOL
__stdcall
InitializeCriticalSectionEx(
 LPCRITICAL_SECTION lpCriticalSection,
 DWORD dwSpinCount,
 DWORD Flags
 );
__declspec(dllimport)
DWORD
__stdcall
SetCriticalSectionSpinCount(
 LPCRITICAL_SECTION lpCriticalSection,
 DWORD dwSpinCount
 );
__declspec(dllimport)
BOOL
__stdcall
TryEnterCriticalSection(
 LPCRITICAL_SECTION lpCriticalSection
 );
__declspec(dllimport)
void
__stdcall
DeleteCriticalSection(
 LPCRITICAL_SECTION lpCriticalSection
 );
__declspec(dllimport)
BOOL
__stdcall
SetEvent(
 HANDLE hEvent
 );
__declspec(dllimport)
BOOL
__stdcall
ResetEvent(
 HANDLE hEvent
 );
__declspec(dllimport)
BOOL
__stdcall
PulseEvent(
 HANDLE hEvent
 );
__declspec(dllimport)
BOOL
__stdcall
ReleaseSemaphore(
 HANDLE hSemaphore,
 LONG lReleaseCount,
 LPLONG lpPreviousCount
 );
__declspec(dllimport)
BOOL
__stdcall
ReleaseMutex(
 HANDLE hMutex
 );
__declspec(dllimport)
DWORD
__stdcall
WaitForSingleObject(
 HANDLE hHandle,
 DWORD dwMilliseconds
 );
__declspec(dllimport)
DWORD
__stdcall
WaitForMultipleObjects(
 DWORD nCount,
 const HANDLE *lpHandles,
 BOOL bWaitAll,
 DWORD dwMilliseconds
 );
__declspec(dllimport)
void
__stdcall
Sleep(
 DWORD dwMilliseconds
 );
__declspec(dllimport)
HGLOBAL
__stdcall
LoadResource(
 HMODULE hModule,
 HRSRC hResInfo
 );
__declspec(dllimport)
DWORD
__stdcall
SizeofResource(
 HMODULE hModule,
 HRSRC hResInfo
 );
__declspec(dllimport)
ATOM
__stdcall
GlobalDeleteAtom(
 ATOM nAtom
 );
__declspec(dllimport)
BOOL
__stdcall
InitAtomTable(
 DWORD nSize
 );
__declspec(dllimport)
ATOM
__stdcall
DeleteAtom(
 ATOM nAtom
 );
__declspec(dllimport)
UINT
__stdcall
SetHandleCount(
 UINT uNumber
 );
__declspec(dllimport)
DWORD
__stdcall
GetLogicalDrives(
 void
 );
__declspec(dllimport)
BOOL
__stdcall
LockFile(
 HANDLE hFile,
 DWORD dwFileOffsetLow,
 DWORD dwFileOffsetHigh,
 DWORD nNumberOfBytesToLockLow,
 DWORD nNumberOfBytesToLockHigh
 );
__declspec(dllimport)
BOOL
__stdcall
UnlockFile(
 HANDLE hFile,
 DWORD dwFileOffsetLow,
 DWORD dwFileOffsetHigh,
 DWORD nNumberOfBytesToUnlockLow,
 DWORD nNumberOfBytesToUnlockHigh
 );
__declspec(dllimport)
BOOL
__stdcall
LockFileEx(
 HANDLE hFile,
 DWORD dwFlags,
 DWORD dwReserved,
 DWORD nNumberOfBytesToLockLow,
 DWORD nNumberOfBytesToLockHigh,
 LPOVERLAPPED lpOverlapped
 );
__declspec(dllimport)
BOOL
__stdcall
UnlockFileEx(
 HANDLE hFile,
 DWORD dwReserved,
 DWORD nNumberOfBytesToUnlockLow,
 DWORD nNumberOfBytesToUnlockHigh,
 LPOVERLAPPED lpOverlapped
 );
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
__declspec(dllimport)
BOOL
__stdcall
GetFileInformationByHandle(
 HANDLE hFile,
 LPBY_HANDLE_FILE_INFORMATION lpFileInformation
 );
__declspec(dllimport)
DWORD
__stdcall
GetFileType(
 HANDLE hFile
 );
__declspec(dllimport)
DWORD
__stdcall
GetFileSize(
 HANDLE hFile,
 LPDWORD lpFileSizeHigh
 );
__declspec(dllimport)
BOOL
__stdcall
GetFileSizeEx(
 HANDLE hFile,
 PLARGE_INTEGER lpFileSize
 );
__declspec(dllimport)
HANDLE
__stdcall
GetStdHandle(
 DWORD nStdHandle
 );
__declspec(dllimport)
BOOL
__stdcall
SetStdHandle(
 DWORD nStdHandle,
 HANDLE hHandle
 );
__declspec(dllimport)
BOOL
__stdcall
SetStdHandleEx(
 DWORD nStdHandle,
 HANDLE hHandle,
 PHANDLE phPrevValue
 );
__declspec(dllimport)
BOOL
__stdcall
WriteFile(
 HANDLE hFile,
 LPCVOID lpBuffer,
 DWORD nNumberOfBytesToWrite,
 LPDWORD lpNumberOfBytesWritten,
 LPOVERLAPPED lpOverlapped
 );
__declspec(dllimport)
BOOL
__stdcall
ReadFile(
 HANDLE hFile,
 LPVOID lpBuffer,
 DWORD nNumberOfBytesToRead,
 LPDWORD lpNumberOfBytesRead,
 LPOVERLAPPED lpOverlapped
 );
__declspec(dllimport)
BOOL
__stdcall
FlushFileBuffers(
 HANDLE hFile
 );
__declspec(dllimport)
BOOL
__stdcall
DeviceIoControl(
 HANDLE hDevice,
 DWORD dwIoControlCode,
 LPVOID lpInBuffer,
 DWORD nInBufferSize,
 LPVOID lpOutBuffer,
 DWORD nOutBufferSize,
 LPDWORD lpBytesReturned,
 LPOVERLAPPED lpOverlapped
 );
__declspec(dllimport)
BOOL
__stdcall
RequestDeviceWakeup(
 HANDLE hDevice
 );
__declspec(dllimport)
BOOL
__stdcall
CancelDeviceWakeupRequest(
 HANDLE hDevice
 );
__declspec(dllimport)
BOOL
__stdcall
GetDevicePowerState(
 HANDLE hDevice,
 BOOL *pfOn
 );
__declspec(dllimport)
BOOL
__stdcall
SetMessageWaitingIndicator(
 HANDLE hMsgIndicator,
 ULONG ulMsgCount
 );
__declspec(dllimport)
BOOL
__stdcall
SetEndOfFile(
 HANDLE hFile
 );
__declspec(dllimport)
DWORD
__stdcall
SetFilePointer(
 HANDLE hFile,
 LONG lDistanceToMove,
 PLONG lpDistanceToMoveHigh,
 DWORD dwMoveMethod
 );
__declspec(dllimport)
BOOL
__stdcall
SetFilePointerEx(
 HANDLE hFile,
 LARGE_INTEGER liDistanceToMove,
 PLARGE_INTEGER lpNewFilePointer,
 DWORD dwMoveMethod
 );
__declspec(dllimport)
BOOL
__stdcall
FindClose(
 HANDLE hFindFile
 );
__declspec(dllimport)
BOOL
__stdcall
GetFileTime(
 HANDLE hFile,
 LPFILETIME lpCreationTime,
 LPFILETIME lpLastAccessTime,
 LPFILETIME lpLastWriteTime
 );
__declspec(dllimport)
BOOL
__stdcall
SetFileTime(
 HANDLE hFile,
 const FILETIME *lpCreationTime,
 const FILETIME *lpLastAccessTime,
 const FILETIME *lpLastWriteTime
 );
__declspec(dllimport)
BOOL
__stdcall
SetFileValidData(
 HANDLE hFile,
 LONGLONG ValidDataLength
 );
__declspec(dllimport)
BOOL
__stdcall
SetFileShortNameA(
 HANDLE hFile,
 LPCSTR lpShortName
 );
__declspec(dllimport)
BOOL
__stdcall
SetFileShortNameW(
 HANDLE hFile,
 LPCWSTR lpShortName
 );
__declspec(dllimport)
BOOL
__stdcall
CloseHandle(
 HANDLE hObject
 );
__declspec(dllimport)
BOOL
__stdcall
DuplicateHandle(
 HANDLE hSourceProcessHandle,
 HANDLE hSourceHandle,
 HANDLE hTargetProcessHandle,
 LPHANDLE lpTargetHandle,
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 DWORD dwOptions
 );
__declspec(dllimport)
BOOL
__stdcall
GetHandleInformation(
 HANDLE hObject,
 LPDWORD lpdwFlags
 );
__declspec(dllimport)
BOOL
__stdcall
SetHandleInformation(
 HANDLE hObject,
 DWORD dwMask,
 DWORD dwFlags
 );
__declspec(dllimport)
DWORD
__stdcall
LoadModule(
 LPCSTR lpModuleName,
 LPVOID lpParameterBlock
 );
__declspec(dllimport)
UINT
__stdcall
WinExec(
 LPCSTR lpCmdLine,
 UINT uCmdShow
 );
__declspec(dllimport)
BOOL
__stdcall
ClearCommBreak(
 HANDLE hFile
 );
__declspec(dllimport)
BOOL
__stdcall
ClearCommError(
 HANDLE hFile,
 LPDWORD lpErrors,
 LPCOMSTAT lpStat
 );
__declspec(dllimport)
BOOL
__stdcall
SetupComm(
 HANDLE hFile,
 DWORD dwInQueue,
 DWORD dwOutQueue
 );
__declspec(dllimport)
BOOL
__stdcall
EscapeCommFunction(
 HANDLE hFile,
 DWORD dwFunc
 );
__declspec(dllimport)
BOOL
__stdcall
GetCommConfig(
 HANDLE hCommDev,
 LPCOMMCONFIG lpCC,
 LPDWORD lpdwSize
 );
__declspec(dllimport)
BOOL
__stdcall
GetCommMask(
 HANDLE hFile,
 LPDWORD lpEvtMask
 );
__declspec(dllimport)
BOOL
__stdcall
GetCommProperties(
 HANDLE hFile,
 LPCOMMPROP lpCommProp
 );
__declspec(dllimport)
BOOL
__stdcall
GetCommModemStatus(
 HANDLE hFile,
 LPDWORD lpModemStat
 );
__declspec(dllimport)
BOOL
__stdcall
GetCommState(
 HANDLE hFile,
 LPDCB lpDCB
 );
__declspec(dllimport)
BOOL
__stdcall
GetCommTimeouts(
 HANDLE hFile,
 LPCOMMTIMEOUTS lpCommTimeouts
 );
__declspec(dllimport)
BOOL
__stdcall
PurgeComm(
 HANDLE hFile,
 DWORD dwFlags
 );
__declspec(dllimport)
BOOL
__stdcall
SetCommBreak(
 HANDLE hFile
 );
__declspec(dllimport)
BOOL
__stdcall
SetCommConfig(
 HANDLE hCommDev,
 LPCOMMCONFIG lpCC,
 DWORD dwSize
 );
__declspec(dllimport)
BOOL
__stdcall
SetCommMask(
 HANDLE hFile,
 DWORD dwEvtMask
 );
__declspec(dllimport)
BOOL
__stdcall
SetCommState(
 HANDLE hFile,
 LPDCB lpDCB
 );
__declspec(dllimport)
BOOL
__stdcall
SetCommTimeouts(
 HANDLE hFile,
 LPCOMMTIMEOUTS lpCommTimeouts
 );
__declspec(dllimport)
BOOL
__stdcall
TransmitCommChar(
 HANDLE hFile,
 char cChar
 );
__declspec(dllimport)
BOOL
__stdcall
WaitCommEvent(
 HANDLE hFile,
 LPDWORD lpEvtMask,
 LPOVERLAPPED lpOverlapped
 );
__declspec(dllimport)
DWORD
__stdcall
SetTapePosition(
 HANDLE hDevice,
 DWORD dwPositionMethod,
 DWORD dwPartition,
 DWORD dwOffsetLow,
 DWORD dwOffsetHigh,
 BOOL bImmediate
 );
__declspec(dllimport)
DWORD
__stdcall
GetTapePosition(
 HANDLE hDevice,
 DWORD dwPositionType,
 LPDWORD lpdwPartition,
 LPDWORD lpdwOffsetLow,
 LPDWORD lpdwOffsetHigh
 );
__declspec(dllimport)
DWORD
__stdcall
PrepareTape(
 HANDLE hDevice,
 DWORD dwOperation,
 BOOL bImmediate
 );
__declspec(dllimport)
DWORD
__stdcall
EraseTape(
 HANDLE hDevice,
 DWORD dwEraseType,
 BOOL bImmediate
 );
__declspec(dllimport)
DWORD
__stdcall
CreateTapePartition(
 HANDLE hDevice,
 DWORD dwPartitionMethod,
 DWORD dwCount,
 DWORD dwSize
 );
__declspec(dllimport)
DWORD
__stdcall
WriteTapemark(
 HANDLE hDevice,
 DWORD dwTapemarkType,
 DWORD dwTapemarkCount,
 BOOL bImmediate
 );
__declspec(dllimport)
DWORD
__stdcall
GetTapeStatus(
 HANDLE hDevice
 );
__declspec(dllimport)
DWORD
__stdcall
GetTapeParameters(
 HANDLE hDevice,
 DWORD dwOperation,
 LPDWORD lpdwSize,
 LPVOID lpTapeInformation
 );
__declspec(dllimport)
DWORD
__stdcall
SetTapeParameters(
 HANDLE hDevice,
 DWORD dwOperation,
 LPVOID lpTapeInformation
 );
__declspec(dllimport)
BOOL
__stdcall
Beep(
 DWORD dwFreq,
 DWORD dwDuration
 );
__declspec(dllimport)
int
__stdcall
MulDiv(
 int nNumber,
 int nNumerator,
 int nDenominator
 );
__declspec(dllimport)
void
__stdcall
GetSystemTime(
 LPSYSTEMTIME lpSystemTime
 );
__declspec(dllimport)
void
__stdcall
GetSystemTimeAsFileTime(
 LPFILETIME lpSystemTimeAsFileTime
 );
__declspec(dllimport)
BOOL
__stdcall
SetSystemTime(
 const SYSTEMTIME *lpSystemTime
 );
__declspec(dllimport)
void
__stdcall
GetLocalTime(
 LPSYSTEMTIME lpSystemTime
 );
__declspec(dllimport)
BOOL
__stdcall
SetLocalTime(
 const SYSTEMTIME *lpSystemTime
 );
__declspec(dllimport)
void
__stdcall
GetSystemInfo(
 LPSYSTEM_INFO lpSystemInfo
 );
__declspec(dllimport)
BOOL
__stdcall
SetSystemFileCacheSize (
 SIZE_T MinimumFileCacheSize,
 SIZE_T MaximumFileCacheSize,
 DWORD Flags
 );
__declspec(dllimport)
BOOL
__stdcall
GetSystemFileCacheSize (
 PSIZE_T lpMinimumFileCacheSize,
 PSIZE_T lpMaximumFileCacheSize,
 PDWORD lpFlags
 );
__declspec(dllimport)
BOOL
__stdcall
GetSystemRegistryQuota(
 PDWORD pdwQuotaAllowed,
 PDWORD pdwQuotaUsed
 );
BOOL
__stdcall
GetSystemTimes(
 LPFILETIME lpIdleTime,
 LPFILETIME lpKernelTime,
 LPFILETIME lpUserTime
 );
__declspec(dllimport)
void
__stdcall
GetNativeSystemInfo(
 LPSYSTEM_INFO lpSystemInfo
 );
__declspec(dllimport)
BOOL
__stdcall
IsProcessorFeaturePresent(
 DWORD ProcessorFeature
 );
typedef struct _TIME_ZONE_INFORMATION {
 LONG Bias;
 WCHAR StandardName[ 32 ];
 SYSTEMTIME StandardDate;
 LONG StandardBias;
 WCHAR DaylightName[ 32 ];
 SYSTEMTIME DaylightDate;
 LONG DaylightBias;
} TIME_ZONE_INFORMATION, *PTIME_ZONE_INFORMATION, *LPTIME_ZONE_INFORMATION;
typedef struct _TIME_DYNAMIC_ZONE_INFORMATION {
 LONG Bias;
 WCHAR StandardName[ 32 ];
 SYSTEMTIME StandardDate;
 LONG StandardBias;
 WCHAR DaylightName[ 32 ];
 SYSTEMTIME DaylightDate;
 LONG DaylightBias;
 WCHAR TimeZoneKeyName[ 128 ];
 BOOLEAN DynamicDaylightTimeDisabled;
} DYNAMIC_TIME_ZONE_INFORMATION, *PDYNAMIC_TIME_ZONE_INFORMATION;
__declspec(dllimport)
BOOL
__stdcall
SystemTimeToTzSpecificLocalTime(
 const TIME_ZONE_INFORMATION *lpTimeZoneInformation,
 const SYSTEMTIME *lpUniversalTime,
 LPSYSTEMTIME lpLocalTime
 );
__declspec(dllimport)
BOOL
__stdcall
TzSpecificLocalTimeToSystemTime(
 const TIME_ZONE_INFORMATION *lpTimeZoneInformation,
 const SYSTEMTIME *lpLocalTime,
 LPSYSTEMTIME lpUniversalTime
 );
__declspec(dllimport)
DWORD
__stdcall
GetTimeZoneInformation(
 LPTIME_ZONE_INFORMATION lpTimeZoneInformation
 );
__declspec(dllimport)
BOOL
__stdcall
SetTimeZoneInformation(
 const TIME_ZONE_INFORMATION *lpTimeZoneInformation
 );
__declspec(dllimport)
DWORD
__stdcall
GetDynamicTimeZoneInformation(
 PDYNAMIC_TIME_ZONE_INFORMATION pTimeZoneInformation
 );
__declspec(dllimport)
BOOL
__stdcall
SetDynamicTimeZoneInformation(
 const DYNAMIC_TIME_ZONE_INFORMATION *lpTimeZoneInformation
 );
__declspec(dllimport)
BOOL
__stdcall
SystemTimeToFileTime(
 const SYSTEMTIME *lpSystemTime,
 LPFILETIME lpFileTime
 );
__declspec(dllimport)
BOOL
__stdcall
FileTimeToLocalFileTime(
 const FILETIME *lpFileTime,
 LPFILETIME lpLocalFileTime
 );
__declspec(dllimport)
BOOL
__stdcall
LocalFileTimeToFileTime(
 const FILETIME *lpLocalFileTime,
 LPFILETIME lpFileTime
 );
__declspec(dllimport)
BOOL
__stdcall
FileTimeToSystemTime(
 const FILETIME *lpFileTime,
 LPSYSTEMTIME lpSystemTime
 );
__declspec(dllimport)
LONG
__stdcall
CompareFileTime(
 const FILETIME *lpFileTime1,
 const FILETIME *lpFileTime2
 );
__declspec(dllimport)
BOOL
__stdcall
FileTimeToDosDateTime(
 const FILETIME *lpFileTime,
 LPWORD lpFatDate,
 LPWORD lpFatTime
 );
__declspec(dllimport)
BOOL
__stdcall
DosDateTimeToFileTime(
 WORD wFatDate,
 WORD wFatTime,
 LPFILETIME lpFileTime
 );
__declspec(dllimport)
DWORD
__stdcall
GetTickCount(
 void
 );
__declspec(dllimport)
ULONGLONG
__stdcall
GetTickCount64(
 void
 );
__declspec(dllimport)
BOOL
__stdcall
SetSystemTimeAdjustment(
 DWORD dwTimeAdjustment,
 BOOL bTimeAdjustmentDisabled
 );
__declspec(dllimport)
BOOL
__stdcall
GetSystemTimeAdjustment(
 PDWORD lpTimeAdjustment,
 PDWORD lpTimeIncrement,
 PBOOL lpTimeAdjustmentDisabled
 );
__declspec(dllimport)
DWORD
__stdcall
FormatMessageA(
 DWORD dwFlags,
 LPCVOID lpSource,
 DWORD dwMessageId,
 DWORD dwLanguageId,
 LPSTR lpBuffer,
 DWORD nSize,
 va_list *Arguments
 );
__declspec(dllimport)
DWORD
__stdcall
FormatMessageW(
 DWORD dwFlags,
 LPCVOID lpSource,
 DWORD dwMessageId,
 DWORD dwLanguageId,
 LPWSTR lpBuffer,
 DWORD nSize,
 va_list *Arguments
 );
__declspec(dllimport)
BOOL
__stdcall
CreatePipe(
 PHANDLE hReadPipe,
 PHANDLE hWritePipe,
 LPSECURITY_ATTRIBUTES lpPipeAttributes,
 DWORD nSize
 );
__declspec(dllimport)
BOOL
__stdcall
ConnectNamedPipe(
 HANDLE hNamedPipe,
 LPOVERLAPPED lpOverlapped
 );
__declspec(dllimport)
BOOL
__stdcall
DisconnectNamedPipe(
 HANDLE hNamedPipe
 );
__declspec(dllimport)
BOOL
__stdcall
SetNamedPipeHandleState(
 HANDLE hNamedPipe,
 LPDWORD lpMode,
 LPDWORD lpMaxCollectionCount,
 LPDWORD lpCollectDataTimeout
 );
__declspec(dllimport)
BOOL
__stdcall
GetNamedPipeInfo(
 HANDLE hNamedPipe,
 LPDWORD lpFlags,
 LPDWORD lpOutBufferSize,
 LPDWORD lpInBufferSize,
 LPDWORD lpMaxInstances
 );
__declspec(dllimport)
BOOL
__stdcall
PeekNamedPipe(
 HANDLE hNamedPipe,
 LPVOID lpBuffer,
 DWORD nBufferSize,
 LPDWORD lpBytesRead,
 LPDWORD lpTotalBytesAvail,
 LPDWORD lpBytesLeftThisMessage
 );
__declspec(dllimport)
BOOL
__stdcall
TransactNamedPipe(
 HANDLE hNamedPipe,
 LPVOID lpInBuffer,
 DWORD nInBufferSize,
 LPVOID lpOutBuffer,
 DWORD nOutBufferSize,
 LPDWORD lpBytesRead,
 LPOVERLAPPED lpOverlapped
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateMailslotA(
 LPCSTR lpName,
 DWORD nMaxMessageSize,
 DWORD lReadTimeout,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateMailslotW(
 LPCWSTR lpName,
 DWORD nMaxMessageSize,
 DWORD lReadTimeout,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes
 );
__declspec(dllimport)
BOOL
__stdcall
GetMailslotInfo(
 HANDLE hMailslot,
 LPDWORD lpMaxMessageSize,
 LPDWORD lpNextSize,
 LPDWORD lpMessageCount,
 LPDWORD lpReadTimeout
 );
__declspec(dllimport)
BOOL
__stdcall
SetMailslotInfo(
 HANDLE hMailslot,
 DWORD lReadTimeout
 );
__declspec(dllimport)
LPVOID
__stdcall
MapViewOfFile(
 HANDLE hFileMappingObject,
 DWORD dwDesiredAccess,
 DWORD dwFileOffsetHigh,
 DWORD dwFileOffsetLow,
 SIZE_T dwNumberOfBytesToMap
 );
__declspec(dllimport)
BOOL
__stdcall
FlushViewOfFile(
 LPCVOID lpBaseAddress,
 SIZE_T dwNumberOfBytesToFlush
 );
__declspec(dllimport)
BOOL
__stdcall
UnmapViewOfFile(
 LPCVOID lpBaseAddress
 );
__declspec(dllimport)
BOOL
__stdcall
EncryptFileA(
 LPCSTR lpFileName
 );
__declspec(dllimport)
BOOL
__stdcall
EncryptFileW(
 LPCWSTR lpFileName
 );
__declspec(dllimport)
BOOL
__stdcall
DecryptFileA(
 LPCSTR lpFileName,
 DWORD dwReserved
 );
__declspec(dllimport)
BOOL
__stdcall
DecryptFileW(
 LPCWSTR lpFileName,
 DWORD dwReserved
 );
__declspec(dllimport)
BOOL
__stdcall
FileEncryptionStatusA(
 LPCSTR lpFileName,
 LPDWORD lpStatus
 );
__declspec(dllimport)
BOOL
__stdcall
FileEncryptionStatusW(
 LPCWSTR lpFileName,
 LPDWORD lpStatus
 );
typedef
DWORD
(__stdcall *PFE_EXPORT_FUNC)(
 PBYTE pbData,
 PVOID pvCallbackContext,
 ULONG ulLength
 );
typedef
DWORD
(__stdcall *PFE_IMPORT_FUNC)(
 PBYTE pbData,
 PVOID pvCallbackContext,
 PULONG ulLength
 );
__declspec(dllimport)
DWORD
__stdcall
OpenEncryptedFileRawA(
 LPCSTR lpFileName,
 ULONG ulFlags,
 PVOID *pvContext
 );
__declspec(dllimport)
DWORD
__stdcall
OpenEncryptedFileRawW(
 LPCWSTR lpFileName,
 ULONG ulFlags,
 PVOID *pvContext
 );
__declspec(dllimport)
DWORD
__stdcall
ReadEncryptedFileRaw(
 PFE_EXPORT_FUNC pfExportCallback,
 PVOID pvCallbackContext,
 PVOID pvContext
 );
__declspec(dllimport)
DWORD
__stdcall
WriteEncryptedFileRaw(
 PFE_IMPORT_FUNC pfImportCallback,
 PVOID pvCallbackContext,
 PVOID pvContext
 );
__declspec(dllimport)
void
__stdcall
CloseEncryptedFileRaw(
 PVOID pvContext
 );
__declspec(dllimport)
int
__stdcall
lstrcmpA(
 LPCSTR lpString1,
 LPCSTR lpString2
 );
__declspec(dllimport)
int
__stdcall
lstrcmpW(
 LPCWSTR lpString1,
 LPCWSTR lpString2
 );
__declspec(dllimport)
int
__stdcall
lstrcmpiA(
 LPCSTR lpString1,
 LPCSTR lpString2
 );
__declspec(dllimport)
int
__stdcall
lstrcmpiW(
 LPCWSTR lpString1,
 LPCWSTR lpString2
 );
__declspec(dllimport)
LPSTR
__stdcall
lstrcpynA(
 LPSTR lpString1,
 LPCSTR lpString2,
 int iMaxLength
 );
__declspec(dllimport)
LPWSTR
__stdcall
lstrcpynW(
 LPWSTR lpString1,
 LPCWSTR lpString2,
 int iMaxLength
 );
#pragma warning(push)
#pragma warning(disable:4995)
__declspec(dllimport)
LPSTR
__stdcall
lstrcpyA(
 LPSTR lpString1,
 LPCSTR lpString2
 );
__declspec(dllimport)
LPWSTR
__stdcall
lstrcpyW(
 LPWSTR lpString1,
 LPCWSTR lpString2
 );
__declspec(dllimport)
LPSTR
__stdcall
lstrcatA(
 LPSTR lpString1,
 LPCSTR lpString2
 );
__declspec(dllimport)
LPWSTR
__stdcall
lstrcatW(
 LPWSTR lpString1,
 LPCWSTR lpString2
 );
#pragma warning(pop)
__declspec(dllimport)
int
__stdcall
lstrlenA(
 LPCSTR lpString
 );
__declspec(dllimport)
int
__stdcall
lstrlenW(
 LPCWSTR lpString
 );
__declspec(dllimport)
HFILE
__stdcall
OpenFile(
 LPCSTR lpFileName,
 LPOFSTRUCT lpReOpenBuff,
 UINT uStyle
 );
__declspec(dllimport)
HFILE
__stdcall
_lopen(
 LPCSTR lpPathName,
 int iReadWrite
 );
__declspec(dllimport)
HFILE
__stdcall
_lcreat(
 LPCSTR lpPathName,
 int iAttribute
 );
__declspec(dllimport)
UINT
__stdcall
_lread(
 HFILE hFile,
 LPVOID lpBuffer,
 UINT uBytes
 );
__declspec(dllimport)
UINT
__stdcall
_lwrite(
 HFILE hFile,
 LPCCH lpBuffer,
 UINT uBytes
 );
__declspec(dllimport)
long
__stdcall
_hread(
 HFILE hFile,
 LPVOID lpBuffer,
 long lBytes
 );
__declspec(dllimport)
long
__stdcall
_hwrite(
 HFILE hFile,
 LPCCH lpBuffer,
 long lBytes
 );
__declspec(dllimport)
HFILE
__stdcall
_lclose(
 HFILE hFile
 );
__declspec(dllimport)
LONG
__stdcall
_llseek(
 HFILE hFile,
 LONG lOffset,
 int iOrigin
 );
__declspec(dllimport)
BOOL
__stdcall
IsTextUnicode(
 const void* lpv,
 int iSize,
 LPINT lpiResult
 );
__declspec(dllimport)
DWORD
__stdcall
FlsAlloc(
 PFLS_CALLBACK_FUNCTION lpCallback
 );
__declspec(dllimport)
PVOID
__stdcall
FlsGetValue(
 DWORD dwFlsIndex
 );
__declspec(dllimport)
BOOL
__stdcall
FlsSetValue(
 DWORD dwFlsIndex,
 PVOID lpFlsData
 );
__declspec(dllimport)
BOOL
__stdcall
FlsFree(
 DWORD dwFlsIndex
 );
__declspec(dllimport)
DWORD
__stdcall
TlsAlloc(
 void
 );
__declspec(dllimport)
LPVOID
__stdcall
TlsGetValue(
 DWORD dwTlsIndex
 );
__declspec(dllimport)
BOOL
__stdcall
TlsSetValue(
 DWORD dwTlsIndex,
 LPVOID lpTlsValue
 );
__declspec(dllimport)
BOOL
__stdcall
TlsFree(
 DWORD dwTlsIndex
 );
typedef
void
(__stdcall *LPOVERLAPPED_COMPLETION_ROUTINE)(
 DWORD dwErrorCode,
 DWORD dwNumberOfBytesTransfered,
 LPOVERLAPPED lpOverlapped
 );
__declspec(dllimport)
DWORD
__stdcall
SleepEx(
 DWORD dwMilliseconds,
 BOOL bAlertable
 );
__declspec(dllimport)
DWORD
__stdcall
WaitForSingleObjectEx(
 HANDLE hHandle,
 DWORD dwMilliseconds,
 BOOL bAlertable
 );
__declspec(dllimport)
DWORD
__stdcall
WaitForMultipleObjectsEx(
 DWORD nCount,
 const HANDLE *lpHandles,
 BOOL bWaitAll,
 DWORD dwMilliseconds,
 BOOL bAlertable
 );
__declspec(dllimport)
DWORD
__stdcall
SignalObjectAndWait(
 HANDLE hObjectToSignal,
 HANDLE hObjectToWaitOn,
 DWORD dwMilliseconds,
 BOOL bAlertable
 );
__declspec(dllimport)
BOOL
__stdcall
ReadFileEx(
 HANDLE hFile,
 LPVOID lpBuffer,
 DWORD nNumberOfBytesToRead,
 LPOVERLAPPED lpOverlapped,
 LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine
 );
__declspec(dllimport)
BOOL
__stdcall
WriteFileEx(
 HANDLE hFile,
 LPCVOID lpBuffer,
 DWORD nNumberOfBytesToWrite,
 LPOVERLAPPED lpOverlapped,
 LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine
 );
__declspec(dllimport)
BOOL
__stdcall
BackupRead(
 HANDLE hFile,
 LPBYTE lpBuffer,
 DWORD nNumberOfBytesToRead,
 LPDWORD lpNumberOfBytesRead,
 BOOL bAbort,
 BOOL bProcessSecurity,
 LPVOID *lpContext
 );
__declspec(dllimport)
BOOL
__stdcall
BackupSeek(
 HANDLE hFile,
 DWORD dwLowBytesToSeek,
 DWORD dwHighBytesToSeek,
 LPDWORD lpdwLowByteSeeked,
 LPDWORD lpdwHighByteSeeked,
 LPVOID *lpContext
 );
__declspec(dllimport)
BOOL
__stdcall
BackupWrite(
 HANDLE hFile,
 LPBYTE lpBuffer,
 DWORD nNumberOfBytesToWrite,
 LPDWORD lpNumberOfBytesWritten,
 BOOL bAbort,
 BOOL bProcessSecurity,
 LPVOID *lpContext
 );
typedef struct _WIN32_STREAM_ID {
 DWORD dwStreamId ;
 DWORD dwStreamAttributes ;
 LARGE_INTEGER Size ;
 DWORD dwStreamNameSize ;
 WCHAR cStreamName[ 1 ] ;
} WIN32_STREAM_ID, *LPWIN32_STREAM_ID ;
__declspec(dllimport)
BOOL
__stdcall
ReadFileScatter(
 HANDLE hFile,
 FILE_SEGMENT_ELEMENT aSegmentArray[],
 DWORD nNumberOfBytesToRead,
 LPDWORD lpReserved,
 LPOVERLAPPED lpOverlapped
 );
__declspec(dllimport)
BOOL
__stdcall
WriteFileGather(
 HANDLE hFile,
 FILE_SEGMENT_ELEMENT aSegmentArray[],
 DWORD nNumberOfBytesToWrite,
 LPDWORD lpReserved,
 LPOVERLAPPED lpOverlapped
 );
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
typedef struct _STARTUPINFOEXA {
 STARTUPINFOA StartupInfo;
 struct _PROC_THREAD_ATTRIBUTE_LIST *lpAttributeList;
} STARTUPINFOEXA, *LPSTARTUPINFOEXA;
typedef struct _STARTUPINFOEXW {
 STARTUPINFOW StartupInfo;
 struct _PROC_THREAD_ATTRIBUTE_LIST *lpAttributeList;
} STARTUPINFOEXW, *LPSTARTUPINFOEXW;
typedef STARTUPINFOEXA STARTUPINFOEX;
typedef LPSTARTUPINFOEXA LPSTARTUPINFOEX;
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
typedef struct _WIN32_FILE_ATTRIBUTE_DATA {
 DWORD dwFileAttributes;
 FILETIME ftCreationTime;
 FILETIME ftLastAccessTime;
 FILETIME ftLastWriteTime;
 DWORD nFileSizeHigh;
 DWORD nFileSizeLow;
} WIN32_FILE_ATTRIBUTE_DATA, *LPWIN32_FILE_ATTRIBUTE_DATA;
__declspec(dllimport)
HANDLE
__stdcall
CreateMutexA(
 LPSECURITY_ATTRIBUTES lpMutexAttributes,
 BOOL bInitialOwner,
 LPCSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateMutexW(
 LPSECURITY_ATTRIBUTES lpMutexAttributes,
 BOOL bInitialOwner,
 LPCWSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenMutexA(
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 LPCSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenMutexW(
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 LPCWSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateEventA(
 LPSECURITY_ATTRIBUTES lpEventAttributes,
 BOOL bManualReset,
 BOOL bInitialState,
 LPCSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateEventW(
 LPSECURITY_ATTRIBUTES lpEventAttributes,
 BOOL bManualReset,
 BOOL bInitialState,
 LPCWSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenEventA(
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 LPCSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenEventW(
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 LPCWSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateSemaphoreA(
 LPSECURITY_ATTRIBUTES lpSemaphoreAttributes,
 LONG lInitialCount,
 LONG lMaximumCount,
 LPCSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateSemaphoreW(
 LPSECURITY_ATTRIBUTES lpSemaphoreAttributes,
 LONG lInitialCount,
 LONG lMaximumCount,
 LPCWSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenSemaphoreA(
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 LPCSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenSemaphoreW(
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 LPCWSTR lpName
 );
typedef
void
(__stdcall *PTIMERAPCROUTINE)(
 LPVOID lpArgToCompletionRoutine,
 DWORD dwTimerLowValue,
 DWORD dwTimerHighValue
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateWaitableTimerA(
 LPSECURITY_ATTRIBUTES lpTimerAttributes,
 BOOL bManualReset,
 LPCSTR lpTimerName
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateWaitableTimerW(
 LPSECURITY_ATTRIBUTES lpTimerAttributes,
 BOOL bManualReset,
 LPCWSTR lpTimerName
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenWaitableTimerA(
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 LPCSTR lpTimerName
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenWaitableTimerW(
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 LPCWSTR lpTimerName
 );
__declspec(dllimport)
BOOL
__stdcall
SetWaitableTimer(
 HANDLE hTimer,
 const LARGE_INTEGER *lpDueTime,
 LONG lPeriod,
 PTIMERAPCROUTINE pfnCompletionRoutine,
 LPVOID lpArgToCompletionRoutine,
 BOOL fResume
 );
__declspec(dllimport)
BOOL
__stdcall
CancelWaitableTimer(
 HANDLE hTimer
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateMutexExA(
 LPSECURITY_ATTRIBUTES lpMutexAttributes,
 LPCSTR lpName,
 DWORD dwFlags,
 DWORD dwDesiredAccess
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateMutexExW(
 LPSECURITY_ATTRIBUTES lpMutexAttributes,
 LPCWSTR lpName,
 DWORD dwFlags,
 DWORD dwDesiredAccess
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateEventExA(
 LPSECURITY_ATTRIBUTES lpEventAttributes,
 LPCSTR lpName,
 DWORD dwFlags,
 DWORD dwDesiredAccess
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateEventExW(
 LPSECURITY_ATTRIBUTES lpEventAttributes,
 LPCWSTR lpName,
 DWORD dwFlags,
 DWORD dwDesiredAccess
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateSemaphoreExA(
 LPSECURITY_ATTRIBUTES lpSemaphoreAttributes,
 LONG lInitialCount,
 LONG lMaximumCount,
 LPCSTR lpName,
 DWORD dwFlags,
 DWORD dwDesiredAccess
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateSemaphoreExW(
 LPSECURITY_ATTRIBUTES lpSemaphoreAttributes,
 LONG lInitialCount,
 LONG lMaximumCount,
 LPCWSTR lpName,
 DWORD dwFlags,
 DWORD dwDesiredAccess
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateWaitableTimerExA(
 LPSECURITY_ATTRIBUTES lpTimerAttributes,
 LPCSTR lpTimerName,
 DWORD dwFlags,
 DWORD dwDesiredAccess
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateWaitableTimerExW(
 LPSECURITY_ATTRIBUTES lpTimerAttributes,
 LPCWSTR lpTimerName,
 DWORD dwFlags,
 DWORD dwDesiredAccess
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateFileMappingA(
 HANDLE hFile,
 LPSECURITY_ATTRIBUTES lpFileMappingAttributes,
 DWORD flProtect,
 DWORD dwMaximumSizeHigh,
 DWORD dwMaximumSizeLow,
 LPCSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateFileMappingW(
 HANDLE hFile,
 LPSECURITY_ATTRIBUTES lpFileMappingAttributes,
 DWORD flProtect,
 DWORD dwMaximumSizeHigh,
 DWORD dwMaximumSizeLow,
 LPCWSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateFileMappingNumaA(
 HANDLE hFile,
 LPSECURITY_ATTRIBUTES lpFileMappingAttributes,
 DWORD flProtect,
 DWORD dwMaximumSizeHigh,
 DWORD dwMaximumSizeLow,
 LPCSTR lpName,
 DWORD nndPreferred
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateFileMappingNumaW(
 HANDLE hFile,
 LPSECURITY_ATTRIBUTES lpFileMappingAttributes,
 DWORD flProtect,
 DWORD dwMaximumSizeHigh,
 DWORD dwMaximumSizeLow,
 LPCWSTR lpName,
 DWORD nndPreferred
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenFileMappingA(
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 LPCSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenFileMappingW(
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 LPCWSTR lpName
 );
__declspec(dllimport)
DWORD
__stdcall
GetLogicalDriveStringsA(
 DWORD nBufferLength,
 LPSTR lpBuffer
 );
__declspec(dllimport)
DWORD
__stdcall
GetLogicalDriveStringsW(
 DWORD nBufferLength,
 LPWSTR lpBuffer
 );
typedef enum _MEMORY_RESOURCE_NOTIFICATION_TYPE {
 LowMemoryResourceNotification,
 HighMemoryResourceNotification
} MEMORY_RESOURCE_NOTIFICATION_TYPE;
__declspec(dllimport)
HANDLE
__stdcall
CreateMemoryResourceNotification(
 MEMORY_RESOURCE_NOTIFICATION_TYPE NotificationType
 );
__declspec(dllimport)
BOOL
__stdcall
QueryMemoryResourceNotification(
 HANDLE ResourceNotificationHandle,
 PBOOL ResourceState
 );
__declspec(dllimport)
HMODULE
__stdcall
LoadLibraryA(
 LPCSTR lpLibFileName
 );
__declspec(dllimport)
HMODULE
__stdcall
LoadLibraryW(
 LPCWSTR lpLibFileName
 );
__declspec(dllimport)
HMODULE
__stdcall
LoadLibraryExA(
 LPCSTR lpLibFileName,
 HANDLE hFile,
 DWORD dwFlags
 );
__declspec(dllimport)
HMODULE
__stdcall
LoadLibraryExW(
 LPCWSTR lpLibFileName,
 HANDLE hFile,
 DWORD dwFlags
 );
__declspec(dllimport)
DWORD
__stdcall
GetModuleFileNameA(
 HMODULE hModule,
 LPCH lpFilename,
 DWORD nSize
 );
__declspec(dllimport)
DWORD
__stdcall
GetModuleFileNameW(
 HMODULE hModule,
 LPWCH lpFilename,
 DWORD nSize
 );
__declspec(dllimport)
HMODULE
__stdcall
GetModuleHandleA(
 LPCSTR lpModuleName
 );
__declspec(dllimport)
HMODULE
__stdcall
GetModuleHandleW(
 LPCWSTR lpModuleName
 );
typedef
BOOL
(__stdcall*
PGET_MODULE_HANDLE_EXA)(
 DWORD dwFlags,
 LPCSTR lpModuleName,
 HMODULE* phModule
 );
typedef
BOOL
(__stdcall*
PGET_MODULE_HANDLE_EXW)(
 DWORD dwFlags,
 LPCWSTR lpModuleName,
 HMODULE* phModule
 );
__declspec(dllimport)
BOOL
__stdcall
GetModuleHandleExA(
 DWORD dwFlags,
 LPCSTR lpModuleName,
 HMODULE* phModule
 );
__declspec(dllimport)
BOOL
__stdcall
GetModuleHandleExW(
 DWORD dwFlags,
 LPCWSTR lpModuleName,
 HMODULE* phModule
 );
__declspec(dllimport)
BOOL
__stdcall
NeedCurrentDirectoryForExePathA(
 LPCSTR ExeName
 );
__declspec(dllimport)
BOOL
__stdcall
NeedCurrentDirectoryForExePathW(
 LPCWSTR ExeName
 );
__declspec(dllimport)
BOOL
__stdcall
QueryFullProcessImageNameA(
 HANDLE hProcess,
 DWORD dwFlags,
 LPSTR lpExeName,
 PDWORD lpdwSize
 );
__declspec(dllimport)
BOOL
__stdcall
QueryFullProcessImageNameW(
 HANDLE hProcess,
 DWORD dwFlags,
 LPWSTR lpExeName,
 PDWORD lpdwSize
 );
typedef enum _PROC_THREAD_ATTRIBUTE_NUM {
 ProcThreadAttributeParentProcess = 0,
 ProcThreadAttributeExtendedFlags,
 ProcThreadAttributeHandleList,
 ProcThreadAttributeMax
} PROC_THREAD_ATTRIBUTE_NUM;
typedef struct _PROC_THREAD_ATTRIBUTE_LIST *PPROC_THREAD_ATTRIBUTE_LIST, *LPPROC_THREAD_ATTRIBUTE_LIST;
__declspec(dllimport)
BOOL
__stdcall
InitializeProcThreadAttributeList(
 LPPROC_THREAD_ATTRIBUTE_LIST lpAttributeList,
 DWORD dwAttributeCount,
 DWORD dwFlags,
 PSIZE_T lpSize
 );
__declspec(dllimport)
void
__stdcall
DeleteProcThreadAttributeList(
 LPPROC_THREAD_ATTRIBUTE_LIST lpAttributeList
 );
__declspec(dllimport)
BOOL
__stdcall
UpdateProcThreadAttribute(
 LPPROC_THREAD_ATTRIBUTE_LIST lpAttributeList,
 DWORD dwFlags,
 DWORD_PTR Attribute,
 PVOID lpValue,
 SIZE_T cbSize,
 PVOID lpPreviousValue,
 PSIZE_T lpReturnSize
 );
__declspec(dllimport)
BOOL
__stdcall
CreateProcessA(
 LPCSTR lpApplicationName,
 LPSTR lpCommandLine,
 LPSECURITY_ATTRIBUTES lpProcessAttributes,
 LPSECURITY_ATTRIBUTES lpThreadAttributes,
 BOOL bInheritHandles,
 DWORD dwCreationFlags,
 LPVOID lpEnvironment,
 LPCSTR lpCurrentDirectory,
 LPSTARTUPINFOA lpStartupInfo,
 LPPROCESS_INFORMATION lpProcessInformation
 );
__declspec(dllimport)
BOOL
__stdcall
CreateProcessW(
 LPCWSTR lpApplicationName,
 LPWSTR lpCommandLine,
 LPSECURITY_ATTRIBUTES lpProcessAttributes,
 LPSECURITY_ATTRIBUTES lpThreadAttributes,
 BOOL bInheritHandles,
 DWORD dwCreationFlags,
 LPVOID lpEnvironment,
 LPCWSTR lpCurrentDirectory,
 LPSTARTUPINFOW lpStartupInfo,
 LPPROCESS_INFORMATION lpProcessInformation
 );
__declspec(dllimport)
BOOL
__stdcall
SetProcessShutdownParameters(
 DWORD dwLevel,
 DWORD dwFlags
 );
__declspec(dllimport)
BOOL
__stdcall
GetProcessShutdownParameters(
 LPDWORD lpdwLevel,
 LPDWORD lpdwFlags
 );
__declspec(dllimport)
DWORD
__stdcall
GetProcessVersion(
 DWORD ProcessId
 );
__declspec(dllimport)
void
__stdcall
FatalAppExitA(
 UINT uAction,
 LPCSTR lpMessageText
 );
__declspec(dllimport)
void
__stdcall
FatalAppExitW(
 UINT uAction,
 LPCWSTR lpMessageText
 );
__declspec(dllimport)
void
__stdcall
GetStartupInfoA(
 LPSTARTUPINFOA lpStartupInfo
 );
__declspec(dllimport)
void
__stdcall
GetStartupInfoW(
 LPSTARTUPINFOW lpStartupInfo
 );
__declspec(dllimport)
LPSTR
__stdcall
GetCommandLineA(
 void
 );
__declspec(dllimport)
LPWSTR
__stdcall
GetCommandLineW(
 void
 );
__declspec(dllimport)
DWORD
__stdcall
GetEnvironmentVariableA(
 LPCSTR lpName,
 LPSTR lpBuffer,
 DWORD nSize
 );
__declspec(dllimport)
DWORD
__stdcall
GetEnvironmentVariableW(
 LPCWSTR lpName,
 LPWSTR lpBuffer,
 DWORD nSize
 );
__declspec(dllimport)
BOOL
__stdcall
SetEnvironmentVariableA(
 LPCSTR lpName,
 LPCSTR lpValue
 );
__declspec(dllimport)
BOOL
__stdcall
SetEnvironmentVariableW(
 LPCWSTR lpName,
 LPCWSTR lpValue
 );
__declspec(dllimport)
DWORD
__stdcall
ExpandEnvironmentStringsA(
 LPCSTR lpSrc,
 LPSTR lpDst,
 DWORD nSize
 );
__declspec(dllimport)
DWORD
__stdcall
ExpandEnvironmentStringsW(
 LPCWSTR lpSrc,
 LPWSTR lpDst,
 DWORD nSize
 );
__declspec(dllimport)
DWORD
__stdcall
GetFirmwareEnvironmentVariableA(
 LPCSTR lpName,
 LPCSTR lpGuid,
 PVOID pBuffer,
 DWORD nSize
 );
__declspec(dllimport)
DWORD
__stdcall
GetFirmwareEnvironmentVariableW(
 LPCWSTR lpName,
 LPCWSTR lpGuid,
 PVOID pBuffer,
 DWORD nSize
 );
__declspec(dllimport)
BOOL
__stdcall
SetFirmwareEnvironmentVariableA(
 LPCSTR lpName,
 LPCSTR lpGuid,
 PVOID pValue,
 DWORD nSize
 );
__declspec(dllimport)
BOOL
__stdcall
SetFirmwareEnvironmentVariableW(
 LPCWSTR lpName,
 LPCWSTR lpGuid,
 PVOID pValue,
 DWORD nSize
 );
__declspec(dllimport)
void
__stdcall
OutputDebugStringA(
 LPCSTR lpOutputString
 );
__declspec(dllimport)
void
__stdcall
OutputDebugStringW(
 LPCWSTR lpOutputString
 );
__declspec(dllimport)
HRSRC
__stdcall
FindResourceA(
 HMODULE hModule,
 LPCSTR lpName,
 LPCSTR lpType
 );
__declspec(dllimport)
HRSRC
__stdcall
FindResourceW(
 HMODULE hModule,
 LPCWSTR lpName,
 LPCWSTR lpType
 );
__declspec(dllimport)
HRSRC
__stdcall
FindResourceExA(
 HMODULE hModule,
 LPCSTR lpType,
 LPCSTR lpName,
 WORD wLanguage
 );
__declspec(dllimport)
HRSRC
__stdcall
FindResourceExW(
 HMODULE hModule,
 LPCWSTR lpType,
 LPCWSTR lpName,
 WORD wLanguage
 );
typedef BOOL (__stdcall* ENUMRESTYPEPROCA)( HMODULE hModule, LPSTR lpType,
 LONG_PTR lParam);
typedef BOOL (__stdcall* ENUMRESTYPEPROCW)( HMODULE hModule, LPWSTR lpType,
 LONG_PTR lParam);
typedef BOOL (__stdcall* ENUMRESNAMEPROCA)( HMODULE hModule, LPCSTR lpType,
 LPSTR lpName, LONG_PTR lParam);
typedef BOOL (__stdcall* ENUMRESNAMEPROCW)( HMODULE hModule, LPCWSTR lpType,
 LPWSTR lpName, LONG_PTR lParam);
typedef BOOL (__stdcall* ENUMRESLANGPROCA)( HMODULE hModule, LPCSTR lpType,
 LPCSTR lpName, WORD wLanguage, LONG_PTR lParam);
typedef BOOL (__stdcall* ENUMRESLANGPROCW)( HMODULE hModule, LPCWSTR lpType,
 LPCWSTR lpName, WORD wLanguage, LONG_PTR lParam);
__declspec(dllimport)
BOOL
__stdcall
EnumResourceTypesA(
 HMODULE hModule,
 ENUMRESTYPEPROCA lpEnumFunc,
 LONG_PTR lParam
 );
__declspec(dllimport)
BOOL
__stdcall
EnumResourceTypesW(
 HMODULE hModule,
 ENUMRESTYPEPROCW lpEnumFunc,
 LONG_PTR lParam
 );
__declspec(dllimport)
BOOL
__stdcall
EnumResourceNamesA(
 HMODULE hModule,
 LPCSTR lpType,
 ENUMRESNAMEPROCA lpEnumFunc,
 LONG_PTR lParam
 );
__declspec(dllimport)
BOOL
__stdcall
EnumResourceNamesW(
 HMODULE hModule,
 LPCWSTR lpType,
 ENUMRESNAMEPROCW lpEnumFunc,
 LONG_PTR lParam
 );
__declspec(dllimport)
BOOL
__stdcall
EnumResourceLanguagesA(
 HMODULE hModule,
 LPCSTR lpType,
 LPCSTR lpName,
 ENUMRESLANGPROCA lpEnumFunc,
 LONG_PTR lParam
 );
__declspec(dllimport)
BOOL
__stdcall
EnumResourceLanguagesW(
 HMODULE hModule,
 LPCWSTR lpType,
 LPCWSTR lpName,
 ENUMRESLANGPROCW lpEnumFunc,
 LONG_PTR lParam
 );
__declspec(dllimport)
BOOL
__stdcall
EnumResourceTypesExA(
 HMODULE hModule,
 ENUMRESTYPEPROCA lpEnumFunc,
 LONG_PTR lParam,
 DWORD dwFlags,
 LANGID LangId
 );
__declspec(dllimport)
BOOL
__stdcall
EnumResourceTypesExW(
 HMODULE hModule,
 ENUMRESTYPEPROCW lpEnumFunc,
 LONG_PTR lParam,
 DWORD dwFlags,
 LANGID LangId
 );
__declspec(dllimport)
BOOL
__stdcall
EnumResourceNamesExA(
 HMODULE hModule,
 LPCSTR lpType,
 ENUMRESNAMEPROCA lpEnumFunc,
 LONG_PTR lParam,
 DWORD dwFlags,
 LANGID LangId
 );
__declspec(dllimport)
BOOL
__stdcall
EnumResourceNamesExW(
 HMODULE hModule,
 LPCWSTR lpType,
 ENUMRESNAMEPROCW lpEnumFunc,
 LONG_PTR lParam,
 DWORD dwFlags,
 LANGID LangId
 );
__declspec(dllimport)
BOOL
__stdcall
EnumResourceLanguagesExA(
 HMODULE hModule,
 LPCSTR lpType,
 LPCSTR lpName,
 ENUMRESLANGPROCA lpEnumFunc,
 LONG_PTR lParam,
 DWORD dwFlags,
 LANGID LangId
 );
__declspec(dllimport)
BOOL
__stdcall
EnumResourceLanguagesExW(
 HMODULE hModule,
 LPCWSTR lpType,
 LPCWSTR lpName,
 ENUMRESLANGPROCW lpEnumFunc,
 LONG_PTR lParam,
 DWORD dwFlags,
 LANGID LangId
 );
__declspec(dllimport)
HANDLE
__stdcall
BeginUpdateResourceA(
 LPCSTR pFileName,
 BOOL bDeleteExistingResources
 );
__declspec(dllimport)
HANDLE
__stdcall
BeginUpdateResourceW(
 LPCWSTR pFileName,
 BOOL bDeleteExistingResources
 );
__declspec(dllimport)
BOOL
__stdcall
UpdateResourceA(
 HANDLE hUpdate,
 LPCSTR lpType,
 LPCSTR lpName,
 WORD wLanguage,
 LPVOID lpData,
 DWORD cb
 );
__declspec(dllimport)
BOOL
__stdcall
UpdateResourceW(
 HANDLE hUpdate,
 LPCWSTR lpType,
 LPCWSTR lpName,
 WORD wLanguage,
 LPVOID lpData,
 DWORD cb
 );
__declspec(dllimport)
BOOL
__stdcall
EndUpdateResourceA(
 HANDLE hUpdate,
 BOOL fDiscard
 );
__declspec(dllimport)
BOOL
__stdcall
EndUpdateResourceW(
 HANDLE hUpdate,
 BOOL fDiscard
 );
__declspec(dllimport)
ATOM
__stdcall
GlobalAddAtomA(
 LPCSTR lpString
 );
__declspec(dllimport)
ATOM
__stdcall
GlobalAddAtomW(
 LPCWSTR lpString
 );
__declspec(dllimport)
ATOM
__stdcall
GlobalFindAtomA(
 LPCSTR lpString
 );
__declspec(dllimport)
ATOM
__stdcall
GlobalFindAtomW(
 LPCWSTR lpString
 );
__declspec(dllimport)
UINT
__stdcall
GlobalGetAtomNameA(
 ATOM nAtom,
 LPSTR lpBuffer,
 int nSize
 );
__declspec(dllimport)
UINT
__stdcall
GlobalGetAtomNameW(
 ATOM nAtom,
 LPWSTR lpBuffer,
 int nSize
 );
__declspec(dllimport)
ATOM
__stdcall
AddAtomA(
 LPCSTR lpString
 );
__declspec(dllimport)
ATOM
__stdcall
AddAtomW(
 LPCWSTR lpString
 );
__declspec(dllimport)
ATOM
__stdcall
FindAtomA(
 LPCSTR lpString
 );
__declspec(dllimport)
ATOM
__stdcall
FindAtomW(
 LPCWSTR lpString
 );
__declspec(dllimport)
UINT
__stdcall
GetAtomNameA(
 ATOM nAtom,
 LPSTR lpBuffer,
 int nSize
 );
__declspec(dllimport)
UINT
__stdcall
GetAtomNameW(
 ATOM nAtom,
 LPWSTR lpBuffer,
 int nSize
 );
__declspec(dllimport)
UINT
__stdcall
GetProfileIntA(
 LPCSTR lpAppName,
 LPCSTR lpKeyName,
 INT nDefault
 );
__declspec(dllimport)
UINT
__stdcall
GetProfileIntW(
 LPCWSTR lpAppName,
 LPCWSTR lpKeyName,
 INT nDefault
 );
__declspec(dllimport)
DWORD
__stdcall
GetProfileStringA(
 LPCSTR lpAppName,
 LPCSTR lpKeyName,
 LPCSTR lpDefault,
 LPSTR lpReturnedString,
 DWORD nSize
 );
__declspec(dllimport)
DWORD
__stdcall
GetProfileStringW(
 LPCWSTR lpAppName,
 LPCWSTR lpKeyName,
 LPCWSTR lpDefault,
 LPWSTR lpReturnedString,
 DWORD nSize
 );
__declspec(dllimport)
BOOL
__stdcall
WriteProfileStringA(
 LPCSTR lpAppName,
 LPCSTR lpKeyName,
 LPCSTR lpString
 );
__declspec(dllimport)
BOOL
__stdcall
WriteProfileStringW(
 LPCWSTR lpAppName,
 LPCWSTR lpKeyName,
 LPCWSTR lpString
 );
__declspec(dllimport)
DWORD
__stdcall
GetProfileSectionA(
 LPCSTR lpAppName,
 LPSTR lpReturnedString,
 DWORD nSize
 );
__declspec(dllimport)
DWORD
__stdcall
GetProfileSectionW(
 LPCWSTR lpAppName,
 LPWSTR lpReturnedString,
 DWORD nSize
 );
__declspec(dllimport)
BOOL
__stdcall
WriteProfileSectionA(
 LPCSTR lpAppName,
 LPCSTR lpString
 );
__declspec(dllimport)
BOOL
__stdcall
WriteProfileSectionW(
 LPCWSTR lpAppName,
 LPCWSTR lpString
 );
__declspec(dllimport)
UINT
__stdcall
GetPrivateProfileIntA(
 LPCSTR lpAppName,
 LPCSTR lpKeyName,
 INT nDefault,
 LPCSTR lpFileName
 );
__declspec(dllimport)
UINT
__stdcall
GetPrivateProfileIntW(
 LPCWSTR lpAppName,
 LPCWSTR lpKeyName,
 INT nDefault,
 LPCWSTR lpFileName
 );
__declspec(dllimport)
DWORD
__stdcall
GetPrivateProfileStringA(
 LPCSTR lpAppName,
 LPCSTR lpKeyName,
 LPCSTR lpDefault,
 LPSTR lpReturnedString,
 DWORD nSize,
 LPCSTR lpFileName
 );
__declspec(dllimport)
DWORD
__stdcall
GetPrivateProfileStringW(
 LPCWSTR lpAppName,
 LPCWSTR lpKeyName,
 LPCWSTR lpDefault,
 LPWSTR lpReturnedString,
 DWORD nSize,
 LPCWSTR lpFileName
 );
__declspec(dllimport)
BOOL
__stdcall
WritePrivateProfileStringA(
 LPCSTR lpAppName,
 LPCSTR lpKeyName,
 LPCSTR lpString,
 LPCSTR lpFileName
 );
__declspec(dllimport)
BOOL
__stdcall
WritePrivateProfileStringW(
 LPCWSTR lpAppName,
 LPCWSTR lpKeyName,
 LPCWSTR lpString,
 LPCWSTR lpFileName
 );
__declspec(dllimport)
DWORD
__stdcall
GetPrivateProfileSectionA(
 LPCSTR lpAppName,
 LPSTR lpReturnedString,
 DWORD nSize,
 LPCSTR lpFileName
 );
__declspec(dllimport)
DWORD
__stdcall
GetPrivateProfileSectionW(
 LPCWSTR lpAppName,
 LPWSTR lpReturnedString,
 DWORD nSize,
 LPCWSTR lpFileName
 );
__declspec(dllimport)
BOOL
__stdcall
WritePrivateProfileSectionA(
 LPCSTR lpAppName,
 LPCSTR lpString,
 LPCSTR lpFileName
 );
__declspec(dllimport)
BOOL
__stdcall
WritePrivateProfileSectionW(
 LPCWSTR lpAppName,
 LPCWSTR lpString,
 LPCWSTR lpFileName
 );
__declspec(dllimport)
DWORD
__stdcall
GetPrivateProfileSectionNamesA(
 LPSTR lpszReturnBuffer,
 DWORD nSize,
 LPCSTR lpFileName
 );
__declspec(dllimport)
DWORD
__stdcall
GetPrivateProfileSectionNamesW(
 LPWSTR lpszReturnBuffer,
 DWORD nSize,
 LPCWSTR lpFileName
 );
__declspec(dllimport)
BOOL
__stdcall
GetPrivateProfileStructA(
 LPCSTR lpszSection,
 LPCSTR lpszKey,
 LPVOID lpStruct,
 UINT uSizeStruct,
 LPCSTR szFile
 );
__declspec(dllimport)
BOOL
__stdcall
GetPrivateProfileStructW(
 LPCWSTR lpszSection,
 LPCWSTR lpszKey,
 LPVOID lpStruct,
 UINT uSizeStruct,
 LPCWSTR szFile
 );
__declspec(dllimport)
BOOL
__stdcall
WritePrivateProfileStructA(
 LPCSTR lpszSection,
 LPCSTR lpszKey,
 LPVOID lpStruct,
 UINT uSizeStruct,
 LPCSTR szFile
 );
__declspec(dllimport)
BOOL
__stdcall
WritePrivateProfileStructW(
 LPCWSTR lpszSection,
 LPCWSTR lpszKey,
 LPVOID lpStruct,
 UINT uSizeStruct,
 LPCWSTR szFile
 );
__declspec(dllimport)
UINT
__stdcall
GetDriveTypeA(
 LPCSTR lpRootPathName
 );
__declspec(dllimport)
UINT
__stdcall
GetDriveTypeW(
 LPCWSTR lpRootPathName
 );
__declspec(dllimport)
UINT
__stdcall
GetSystemDirectoryA(
 LPSTR lpBuffer,
 UINT uSize
 );
__declspec(dllimport)
UINT
__stdcall
GetSystemDirectoryW(
 LPWSTR lpBuffer,
 UINT uSize
 );
__declspec(dllimport)
DWORD
__stdcall
GetTempPathA(
 DWORD nBufferLength,
 LPSTR lpBuffer
 );
__declspec(dllimport)
DWORD
__stdcall
GetTempPathW(
 DWORD nBufferLength,
 LPWSTR lpBuffer
 );
__declspec(dllimport)
UINT
__stdcall
GetTempFileNameA(
 LPCSTR lpPathName,
 LPCSTR lpPrefixString,
 UINT uUnique,
 LPSTR lpTempFileName
 );
__declspec(dllimport)
UINT
__stdcall
GetTempFileNameW(
 LPCWSTR lpPathName,
 LPCWSTR lpPrefixString,
 UINT uUnique,
 LPWSTR lpTempFileName
 );
__declspec(dllimport)
UINT
__stdcall
GetWindowsDirectoryA(
 LPSTR lpBuffer,
 UINT uSize
 );
__declspec(dllimport)
UINT
__stdcall
GetWindowsDirectoryW(
 LPWSTR lpBuffer,
 UINT uSize
 );
__declspec(dllimport)
UINT
__stdcall
GetSystemWindowsDirectoryA(
 LPSTR lpBuffer,
 UINT uSize
 );
__declspec(dllimport)
UINT
__stdcall
GetSystemWindowsDirectoryW(
 LPWSTR lpBuffer,
 UINT uSize
 );
__declspec(dllimport)
UINT
__stdcall
GetSystemWow64DirectoryA(
 LPSTR lpBuffer,
 UINT uSize
 );
__declspec(dllimport)
UINT
__stdcall
GetSystemWow64DirectoryW(
 LPWSTR lpBuffer,
 UINT uSize
 );
__declspec(dllimport)
BOOLEAN
__stdcall
Wow64EnableWow64FsRedirection (
 BOOLEAN Wow64FsEnableRedirection
 );
__declspec(dllimport)
BOOL
__stdcall
Wow64DisableWow64FsRedirection (
 PVOID *OldValue
 );
__declspec(dllimport)
BOOL
__stdcall
Wow64RevertWow64FsRedirection (
 PVOID OlValue
 );
typedef UINT (__stdcall* PGET_SYSTEM_WOW64_DIRECTORY_A)( LPSTR lpBuffer, UINT uSize);
typedef UINT (__stdcall* PGET_SYSTEM_WOW64_DIRECTORY_W)( LPWSTR lpBuffer, UINT uSize);
__declspec(dllimport)
BOOL
__stdcall
SetCurrentDirectoryA(
 LPCSTR lpPathName
 );
__declspec(dllimport)
BOOL
__stdcall
SetCurrentDirectoryW(
 LPCWSTR lpPathName
 );
__declspec(dllimport)
DWORD
__stdcall
GetCurrentDirectoryA(
 DWORD nBufferLength,
 LPSTR lpBuffer
 );
__declspec(dllimport)
DWORD
__stdcall
GetCurrentDirectoryW(
 DWORD nBufferLength,
 LPWSTR lpBuffer
 );
__declspec(dllimport)
BOOL
__stdcall
SetDllDirectoryA(
 LPCSTR lpPathName
 );
__declspec(dllimport)
BOOL
__stdcall
SetDllDirectoryW(
 LPCWSTR lpPathName
 );
__declspec(dllimport)
DWORD
__stdcall
GetDllDirectoryA(
 DWORD nBufferLength,
 LPSTR lpBuffer
 );
__declspec(dllimport)
DWORD
__stdcall
GetDllDirectoryW(
 DWORD nBufferLength,
 LPWSTR lpBuffer
 );
__declspec(dllimport)
BOOL
__stdcall
GetDiskFreeSpaceA(
 LPCSTR lpRootPathName,
 LPDWORD lpSectorsPerCluster,
 LPDWORD lpBytesPerSector,
 LPDWORD lpNumberOfFreeClusters,
 LPDWORD lpTotalNumberOfClusters
 );
__declspec(dllimport)
BOOL
__stdcall
GetDiskFreeSpaceW(
 LPCWSTR lpRootPathName,
 LPDWORD lpSectorsPerCluster,
 LPDWORD lpBytesPerSector,
 LPDWORD lpNumberOfFreeClusters,
 LPDWORD lpTotalNumberOfClusters
 );
__declspec(dllimport)
BOOL
__stdcall
GetDiskFreeSpaceExA(
 LPCSTR lpDirectoryName,
 PULARGE_INTEGER lpFreeBytesAvailableToCaller,
 PULARGE_INTEGER lpTotalNumberOfBytes,
 PULARGE_INTEGER lpTotalNumberOfFreeBytes
 );
__declspec(dllimport)
BOOL
__stdcall
GetDiskFreeSpaceExW(
 LPCWSTR lpDirectoryName,
 PULARGE_INTEGER lpFreeBytesAvailableToCaller,
 PULARGE_INTEGER lpTotalNumberOfBytes,
 PULARGE_INTEGER lpTotalNumberOfFreeBytes
 );
__declspec(dllimport)
BOOL
__stdcall
CreateDirectoryA(
 LPCSTR lpPathName,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes
 );
__declspec(dllimport)
BOOL
__stdcall
CreateDirectoryW(
 LPCWSTR lpPathName,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes
 );
__declspec(dllimport)
BOOL
__stdcall
CreateDirectoryExA(
 LPCSTR lpTemplateDirectory,
 LPCSTR lpNewDirectory,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes
 );
__declspec(dllimport)
BOOL
__stdcall
CreateDirectoryExW(
 LPCWSTR lpTemplateDirectory,
 LPCWSTR lpNewDirectory,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes
 );
__declspec(dllimport)
BOOL
__stdcall
CreateDirectoryTransactedA(
 LPCSTR lpTemplateDirectory,
 LPCSTR lpNewDirectory,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
CreateDirectoryTransactedW(
 LPCWSTR lpTemplateDirectory,
 LPCWSTR lpNewDirectory,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
RemoveDirectoryA(
 LPCSTR lpPathName
 );
__declspec(dllimport)
BOOL
__stdcall
RemoveDirectoryW(
 LPCWSTR lpPathName
 );
__declspec(dllimport)
BOOL
__stdcall
RemoveDirectoryTransactedA(
 LPCSTR lpPathName,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
RemoveDirectoryTransactedW(
 LPCWSTR lpPathName,
 HANDLE hTransaction
 );
__declspec(dllimport)
DWORD
__stdcall
GetFullPathNameA(
 LPCSTR lpFileName,
 DWORD nBufferLength,
 LPSTR lpBuffer,
 LPSTR *lpFilePart
 );
__declspec(dllimport)
DWORD
__stdcall
GetFullPathNameW(
 LPCWSTR lpFileName,
 DWORD nBufferLength,
 LPWSTR lpBuffer,
 LPWSTR *lpFilePart
 );
__declspec(dllimport)
DWORD
__stdcall
GetFullPathNameTransactedA(
 LPCSTR lpFileName,
 DWORD nBufferLength,
 LPSTR lpBuffer,
 LPSTR *lpFilePart,
 HANDLE hTransaction
 );
__declspec(dllimport)
DWORD
__stdcall
GetFullPathNameTransactedW(
 LPCWSTR lpFileName,
 DWORD nBufferLength,
 LPWSTR lpBuffer,
 LPWSTR *lpFilePart,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
DefineDosDeviceA(
 DWORD dwFlags,
 LPCSTR lpDeviceName,
 LPCSTR lpTargetPath
 );
__declspec(dllimport)
BOOL
__stdcall
DefineDosDeviceW(
 DWORD dwFlags,
 LPCWSTR lpDeviceName,
 LPCWSTR lpTargetPath
 );
__declspec(dllimport)
DWORD
__stdcall
QueryDosDeviceA(
 LPCSTR lpDeviceName,
 LPSTR lpTargetPath,
 DWORD ucchMax
 );
__declspec(dllimport)
DWORD
__stdcall
QueryDosDeviceW(
 LPCWSTR lpDeviceName,
 LPWSTR lpTargetPath,
 DWORD ucchMax
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateFileA(
 LPCSTR lpFileName,
 DWORD dwDesiredAccess,
 DWORD dwShareMode,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 DWORD dwCreationDisposition,
 DWORD dwFlagsAndAttributes,
 HANDLE hTemplateFile
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateFileW(
 LPCWSTR lpFileName,
 DWORD dwDesiredAccess,
 DWORD dwShareMode,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 DWORD dwCreationDisposition,
 DWORD dwFlagsAndAttributes,
 HANDLE hTemplateFile
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateFileTransactedA(
 LPCSTR lpFileName,
 DWORD dwDesiredAccess,
 DWORD dwShareMode,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 DWORD dwCreationDisposition,
 DWORD dwFlagsAndAttributes,
 HANDLE hTemplateFile,
 HANDLE hTransaction,
 PUSHORT pusMiniVersion,
 PVOID lpExtendedParameter
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateFileTransactedW(
 LPCWSTR lpFileName,
 DWORD dwDesiredAccess,
 DWORD dwShareMode,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 DWORD dwCreationDisposition,
 DWORD dwFlagsAndAttributes,
 HANDLE hTemplateFile,
 HANDLE hTransaction,
 PUSHORT pusMiniVersion,
 PVOID lpExtendedParameter
 );
__declspec(dllimport)
HANDLE
__stdcall
ReOpenFile(
 HANDLE hOriginalFile,
 DWORD dwDesiredAccess,
 DWORD dwShareMode,
 DWORD dwFlagsAndAttributes
 );
__declspec(dllimport)
BOOL
__stdcall
SetFileAttributesA(
 LPCSTR lpFileName,
 DWORD dwFileAttributes
 );
__declspec(dllimport)
BOOL
__stdcall
SetFileAttributesW(
 LPCWSTR lpFileName,
 DWORD dwFileAttributes
 );
__declspec(dllimport)
DWORD
__stdcall
GetFileAttributesA(
 LPCSTR lpFileName
 );
__declspec(dllimport)
DWORD
__stdcall
GetFileAttributesW(
 LPCWSTR lpFileName
 );
__declspec(dllimport)
BOOL
__stdcall
SetFileAttributesTransactedA(
 LPCSTR lpFileName,
 DWORD dwFileAttributes,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
SetFileAttributesTransactedW(
 LPCWSTR lpFileName,
 DWORD dwFileAttributes,
 HANDLE hTransaction
 );
typedef enum _GET_FILEEX_INFO_LEVELS {
 GetFileExInfoStandard,
 GetFileExMaxInfoLevel
} GET_FILEEX_INFO_LEVELS;
__declspec(dllimport)
BOOL
__stdcall
GetFileAttributesTransactedA(
 LPCSTR lpFileName,
 GET_FILEEX_INFO_LEVELS fInfoLevelId,
 LPVOID lpFileInformation,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
GetFileAttributesTransactedW(
 LPCWSTR lpFileName,
 GET_FILEEX_INFO_LEVELS fInfoLevelId,
 LPVOID lpFileInformation,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
GetFileAttributesExA(
 LPCSTR lpFileName,
 GET_FILEEX_INFO_LEVELS fInfoLevelId,
 LPVOID lpFileInformation
 );
__declspec(dllimport)
BOOL
__stdcall
GetFileAttributesExW(
 LPCWSTR lpFileName,
 GET_FILEEX_INFO_LEVELS fInfoLevelId,
 LPVOID lpFileInformation
 );
__declspec(dllimport)
DWORD
__stdcall
GetCompressedFileSizeA(
 LPCSTR lpFileName,
 LPDWORD lpFileSizeHigh
 );
__declspec(dllimport)
DWORD
__stdcall
GetCompressedFileSizeW(
 LPCWSTR lpFileName,
 LPDWORD lpFileSizeHigh
 );
__declspec(dllimport)
DWORD
__stdcall
GetCompressedFileSizeTransactedA(
 LPCSTR lpFileName,
 LPDWORD lpFileSizeHigh,
 HANDLE hTransaction
 );
__declspec(dllimport)
DWORD
__stdcall
GetCompressedFileSizeTransactedW(
 LPCWSTR lpFileName,
 LPDWORD lpFileSizeHigh,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
DeleteFileA(
 LPCSTR lpFileName
 );
__declspec(dllimport)
BOOL
__stdcall
DeleteFileW(
 LPCWSTR lpFileName
 );
__declspec(dllimport)
BOOL
__stdcall
DeleteFileTransactedA(
 LPCSTR lpFileName,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
DeleteFileTransactedW(
 LPCWSTR lpFileName,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
CheckNameLegalDOS8Dot3A(
 LPCSTR lpName,
 LPSTR lpOemName,
 DWORD OemNameSize,
 PBOOL pbNameContainsSpaces ,
 PBOOL pbNameLegal
 );
__declspec(dllimport)
BOOL
__stdcall
CheckNameLegalDOS8Dot3W(
 LPCWSTR lpName,
 LPSTR lpOemName,
 DWORD OemNameSize,
 PBOOL pbNameContainsSpaces ,
 PBOOL pbNameLegal
 );
typedef enum _FINDEX_INFO_LEVELS {
 FindExInfoStandard,
 FindExInfoMaxInfoLevel
} FINDEX_INFO_LEVELS;
typedef enum _FINDEX_SEARCH_OPS {
 FindExSearchNameMatch,
 FindExSearchLimitToDirectories,
 FindExSearchLimitToDevices,
 FindExSearchMaxSearchOp
} FINDEX_SEARCH_OPS;
__declspec(dllimport)
HANDLE
__stdcall
FindFirstFileExA(
 LPCSTR lpFileName,
 FINDEX_INFO_LEVELS fInfoLevelId,
 LPVOID lpFindFileData,
 FINDEX_SEARCH_OPS fSearchOp,
 LPVOID lpSearchFilter,
 DWORD dwAdditionalFlags
 );
__declspec(dllimport)
HANDLE
__stdcall
FindFirstFileExW(
 LPCWSTR lpFileName,
 FINDEX_INFO_LEVELS fInfoLevelId,
 LPVOID lpFindFileData,
 FINDEX_SEARCH_OPS fSearchOp,
 LPVOID lpSearchFilter,
 DWORD dwAdditionalFlags
 );
__declspec(dllimport)
HANDLE
__stdcall
FindFirstFileTransactedA(
 LPCSTR lpFileName,
 FINDEX_INFO_LEVELS fInfoLevelId,
 LPVOID lpFindFileData,
 FINDEX_SEARCH_OPS fSearchOp,
 LPVOID lpSearchFilter,
 DWORD dwAdditionalFlags,
 HANDLE hTransaction
 );
__declspec(dllimport)
HANDLE
__stdcall
FindFirstFileTransactedW(
 LPCWSTR lpFileName,
 FINDEX_INFO_LEVELS fInfoLevelId,
 LPVOID lpFindFileData,
 FINDEX_SEARCH_OPS fSearchOp,
 LPVOID lpSearchFilter,
 DWORD dwAdditionalFlags,
 HANDLE hTransaction
 );
__declspec(dllimport)
HANDLE
__stdcall
FindFirstFileA(
 LPCSTR lpFileName,
 LPWIN32_FIND_DATAA lpFindFileData
 );
__declspec(dllimport)
HANDLE
__stdcall
FindFirstFileW(
 LPCWSTR lpFileName,
 LPWIN32_FIND_DATAW lpFindFileData
 );
__declspec(dllimport)
BOOL
__stdcall
FindNextFileA(
 HANDLE hFindFile,
 LPWIN32_FIND_DATAA lpFindFileData
 );
__declspec(dllimport)
BOOL
__stdcall
FindNextFileW(
 HANDLE hFindFile,
 LPWIN32_FIND_DATAW lpFindFileData
 );
__declspec(dllimport)
DWORD
__stdcall
SearchPathA(
 LPCSTR lpPath,
 LPCSTR lpFileName,
 LPCSTR lpExtension,
 DWORD nBufferLength,
 LPSTR lpBuffer,
 LPSTR *lpFilePart
 );
__declspec(dllimport)
DWORD
__stdcall
SearchPathW(
 LPCWSTR lpPath,
 LPCWSTR lpFileName,
 LPCWSTR lpExtension,
 DWORD nBufferLength,
 LPWSTR lpBuffer,
 LPWSTR *lpFilePart
 );
__declspec(dllimport)
BOOL
__stdcall
CopyFileA(
 LPCSTR lpExistingFileName,
 LPCSTR lpNewFileName,
 BOOL bFailIfExists
 );
__declspec(dllimport)
BOOL
__stdcall
CopyFileW(
 LPCWSTR lpExistingFileName,
 LPCWSTR lpNewFileName,
 BOOL bFailIfExists
 );
typedef
DWORD
(__stdcall *LPPROGRESS_ROUTINE)(
 LARGE_INTEGER TotalFileSize,
 LARGE_INTEGER TotalBytesTransferred,
 LARGE_INTEGER StreamSize,
 LARGE_INTEGER StreamBytesTransferred,
 DWORD dwStreamNumber,
 DWORD dwCallbackReason,
 HANDLE hSourceFile,
 HANDLE hDestinationFile,
 LPVOID lpData
 );
__declspec(dllimport)
BOOL
__stdcall
CopyFileExA(
 LPCSTR lpExistingFileName,
 LPCSTR lpNewFileName,
 LPPROGRESS_ROUTINE lpProgressRoutine,
 LPVOID lpData,
 LPBOOL pbCancel,
 DWORD dwCopyFlags
 );
__declspec(dllimport)
BOOL
__stdcall
CopyFileExW(
 LPCWSTR lpExistingFileName,
 LPCWSTR lpNewFileName,
 LPPROGRESS_ROUTINE lpProgressRoutine,
 LPVOID lpData,
 LPBOOL pbCancel,
 DWORD dwCopyFlags
 );
__declspec(dllimport)
BOOL
__stdcall
CopyFileTransactedA(
 LPCSTR lpExistingFileName,
 LPCSTR lpNewFileName,
 LPPROGRESS_ROUTINE lpProgressRoutine,
 LPVOID lpData,
 LPBOOL pbCancel,
 DWORD dwCopyFlags,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
CopyFileTransactedW(
 LPCWSTR lpExistingFileName,
 LPCWSTR lpNewFileName,
 LPPROGRESS_ROUTINE lpProgressRoutine,
 LPVOID lpData,
 LPBOOL pbCancel,
 DWORD dwCopyFlags,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
MoveFileA(
 LPCSTR lpExistingFileName,
 LPCSTR lpNewFileName
 );
__declspec(dllimport)
BOOL
__stdcall
MoveFileW(
 LPCWSTR lpExistingFileName,
 LPCWSTR lpNewFileName
 );
__declspec(dllimport)
BOOL
__stdcall
MoveFileExA(
 LPCSTR lpExistingFileName,
 LPCSTR lpNewFileName,
 DWORD dwFlags
 );
__declspec(dllimport)
BOOL
__stdcall
MoveFileExW(
 LPCWSTR lpExistingFileName,
 LPCWSTR lpNewFileName,
 DWORD dwFlags
 );
__declspec(dllimport)
BOOL
__stdcall
MoveFileWithProgressA(
 LPCSTR lpExistingFileName,
 LPCSTR lpNewFileName,
 LPPROGRESS_ROUTINE lpProgressRoutine,
 LPVOID lpData,
 DWORD dwFlags
 );
__declspec(dllimport)
BOOL
__stdcall
MoveFileWithProgressW(
 LPCWSTR lpExistingFileName,
 LPCWSTR lpNewFileName,
 LPPROGRESS_ROUTINE lpProgressRoutine,
 LPVOID lpData,
 DWORD dwFlags
 );
__declspec(dllimport)
BOOL
__stdcall
MoveFileTransactedA(
 LPCSTR lpExistingFileName,
 LPCSTR lpNewFileName,
 LPPROGRESS_ROUTINE lpProgressRoutine,
 LPVOID lpData,
 DWORD dwFlags,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
MoveFileTransactedW(
 LPCWSTR lpExistingFileName,
 LPCWSTR lpNewFileName,
 LPPROGRESS_ROUTINE lpProgressRoutine,
 LPVOID lpData,
 DWORD dwFlags,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
ReplaceFileA(
 LPCSTR lpReplacedFileName,
 LPCSTR lpReplacementFileName,
 LPCSTR lpBackupFileName,
 DWORD dwReplaceFlags,
 LPVOID lpExclude,
 LPVOID lpReserved
 );
__declspec(dllimport)
BOOL
__stdcall
ReplaceFileW(
 LPCWSTR lpReplacedFileName,
 LPCWSTR lpReplacementFileName,
 LPCWSTR lpBackupFileName,
 DWORD dwReplaceFlags,
 LPVOID lpExclude,
 LPVOID lpReserved
 );
__declspec(dllimport)
BOOL
__stdcall
CreateHardLinkA(
 LPCSTR lpFileName,
 LPCSTR lpExistingFileName,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes
 );
__declspec(dllimport)
BOOL
__stdcall
CreateHardLinkW(
 LPCWSTR lpFileName,
 LPCWSTR lpExistingFileName,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes
 );
__declspec(dllimport)
BOOL
__stdcall
CreateHardLinkTransactedA(
 LPCSTR lpFileName,
 LPCSTR lpExistingFileName,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 HANDLE hTransaction
 );
__declspec(dllimport)
BOOL
__stdcall
CreateHardLinkTransactedW(
 LPCWSTR lpFileName,
 LPCWSTR lpExistingFileName,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 HANDLE hTransaction
 );
typedef enum _STREAM_INFO_LEVELS {
 FindStreamInfoStandard,
 FindStreamInfoMaxInfoLevel
} STREAM_INFO_LEVELS;
typedef struct _WIN32_FIND_STREAM_DATA {
 LARGE_INTEGER StreamSize;
 WCHAR cStreamName[ 260 + 36 ];
} WIN32_FIND_STREAM_DATA, *PWIN32_FIND_STREAM_DATA;
HANDLE
__stdcall
FindFirstStreamW(
 LPCWSTR lpFileName,
 STREAM_INFO_LEVELS InfoLevel,
 LPVOID lpFindStreamData,
 DWORD dwFlags
 );
BOOL
__stdcall
FindNextStreamW(
 HANDLE hFindStream,
 LPVOID lpFindStreamData
 );
HANDLE
__stdcall
FindFirstFileNameW (
 LPCWSTR lpFileName,
 DWORD dwFlags,
 LPDWORD StringLength,
 PWCHAR LinkName
 );
BOOL
__stdcall
FindNextFileNameW (
 HANDLE hFindStream,
 LPDWORD StringLength,
 PWCHAR LinkName
 );
HANDLE
__stdcall
FindFirstFileNameTransactedW (
 LPCWSTR lpFileName,
 DWORD dwFlags,
 LPDWORD StringLength,
 PWCHAR LinkName,
 HANDLE hTransaction
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateNamedPipeA(
 LPCSTR lpName,
 DWORD dwOpenMode,
 DWORD dwPipeMode,
 DWORD nMaxInstances,
 DWORD nOutBufferSize,
 DWORD nInBufferSize,
 DWORD nDefaultTimeOut,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateNamedPipeW(
 LPCWSTR lpName,
 DWORD dwOpenMode,
 DWORD dwPipeMode,
 DWORD nMaxInstances,
 DWORD nOutBufferSize,
 DWORD nInBufferSize,
 DWORD nDefaultTimeOut,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes
 );
__declspec(dllimport)
BOOL
__stdcall
GetNamedPipeHandleStateA(
 HANDLE hNamedPipe,
 LPDWORD lpState,
 LPDWORD lpCurInstances,
 LPDWORD lpMaxCollectionCount,
 LPDWORD lpCollectDataTimeout,
 LPSTR lpUserName,
 DWORD nMaxUserNameSize
 );
__declspec(dllimport)
BOOL
__stdcall
GetNamedPipeHandleStateW(
 HANDLE hNamedPipe,
 LPDWORD lpState,
 LPDWORD lpCurInstances,
 LPDWORD lpMaxCollectionCount,
 LPDWORD lpCollectDataTimeout,
 LPWSTR lpUserName,
 DWORD nMaxUserNameSize
 );
__declspec(dllimport)
BOOL
__stdcall
CallNamedPipeA(
 LPCSTR lpNamedPipeName,
 LPVOID lpInBuffer,
 DWORD nInBufferSize,
 LPVOID lpOutBuffer,
 DWORD nOutBufferSize,
 LPDWORD lpBytesRead,
 DWORD nTimeOut
 );
__declspec(dllimport)
BOOL
__stdcall
CallNamedPipeW(
 LPCWSTR lpNamedPipeName,
 LPVOID lpInBuffer,
 DWORD nInBufferSize,
 LPVOID lpOutBuffer,
 DWORD nOutBufferSize,
 LPDWORD lpBytesRead,
 DWORD nTimeOut
 );
__declspec(dllimport)
BOOL
__stdcall
WaitNamedPipeA(
 LPCSTR lpNamedPipeName,
 DWORD nTimeOut
 );
__declspec(dllimport)
BOOL
__stdcall
WaitNamedPipeW(
 LPCWSTR lpNamedPipeName,
 DWORD nTimeOut
 );
typedef enum {
 PipeAttribute,
 PipeConnectionAttribute,
 PipeHandleAttribute
} PIPE_ATTRIBUTE_TYPE;
__declspec(dllimport)
BOOL
__stdcall
GetNamedPipeAttribute(
 HANDLE Pipe,
 PIPE_ATTRIBUTE_TYPE AttributeType,
 PSTR AttributeName,
 PVOID AttributeValue,
 PSIZE_T AttributeValueLength
 );
__declspec(dllimport)
BOOL
__stdcall
SetNamedPipeAttribute(
 HANDLE Pipe,
 PIPE_ATTRIBUTE_TYPE AttributeType,
 PSTR AttributeName,
 PVOID AttributeValue,
 SIZE_T AttributeValueLength
 );
__declspec(dllimport)
BOOL
__stdcall
GetNamedPipeClientComputerNameA(
 HANDLE Pipe,
 LPSTR ClientComputerName,
 ULONG ClientComputerNameLength
 );
__declspec(dllimport)
BOOL
__stdcall
GetNamedPipeClientComputerNameW(
 HANDLE Pipe,
 LPWSTR ClientComputerName,
 ULONG ClientComputerNameLength
 );
__declspec(dllimport)
BOOL
__stdcall
GetNamedPipeClientProcessId(
 HANDLE Pipe,
 PULONG ClientProcessId
 );
__declspec(dllimport)
BOOL
__stdcall
GetNamedPipeClientSessionId(
 HANDLE Pipe,
 PULONG ClientSessionId
 );
__declspec(dllimport)
BOOL
__stdcall
GetNamedPipeServerProcessId(
 HANDLE Pipe,
 PULONG ServerProcessId
 );
__declspec(dllimport)
BOOL
__stdcall
GetNamedPipeServerSessionId(
 HANDLE Pipe,
 PULONG ServerSessionId
 );
__declspec(dllimport)
BOOL
__stdcall
SetVolumeLabelA(
 LPCSTR lpRootPathName,
 LPCSTR lpVolumeName
 );
__declspec(dllimport)
BOOL
__stdcall
SetVolumeLabelW(
 LPCWSTR lpRootPathName,
 LPCWSTR lpVolumeName
 );
__declspec(dllimport)
void
__stdcall
SetFileApisToOEM( void );
__declspec(dllimport)
void
__stdcall
SetFileApisToANSI( void );
__declspec(dllimport)
BOOL
__stdcall
AreFileApisANSI( void );
__declspec(dllimport)
BOOL
__stdcall
GetVolumeInformationA(
 LPCSTR lpRootPathName,
 LPSTR lpVolumeNameBuffer,
 DWORD nVolumeNameSize,
 LPDWORD lpVolumeSerialNumber,
 LPDWORD lpMaximumComponentLength,
 LPDWORD lpFileSystemFlags,
 LPSTR lpFileSystemNameBuffer,
 DWORD nFileSystemNameSize
 );
__declspec(dllimport)
BOOL
__stdcall
GetVolumeInformationW(
 LPCWSTR lpRootPathName,
 LPWSTR lpVolumeNameBuffer,
 DWORD nVolumeNameSize,
 LPDWORD lpVolumeSerialNumber,
 LPDWORD lpMaximumComponentLength,
 LPDWORD lpFileSystemFlags,
 LPWSTR lpFileSystemNameBuffer,
 DWORD nFileSystemNameSize
 );
__declspec(dllimport)
BOOL
__stdcall
GetVolumeInformationByHandleW(
 HANDLE hFile,
 LPWSTR lpVolumeNameBuffer,
 DWORD nVolumeNameSize,
 LPDWORD lpVolumeSerialNumber,
 LPDWORD lpMaximumComponentLength,
 LPDWORD lpFileSystemFlags,
 LPWSTR lpFileSystemNameBuffer,
 DWORD nFileSystemNameSize
 );
__declspec(dllimport)
BOOL
__stdcall
CancelSynchronousIo(
 HANDLE hThread
 );
__declspec(dllimport)
BOOL
__stdcall
CancelIoEx(
 HANDLE hFile,
 LPOVERLAPPED lpOverlapped
 );
__declspec(dllimport)
BOOL
__stdcall
CancelIo(
 HANDLE hFile
 );
__declspec(dllimport)
BOOL
__stdcall
SetFileBandwidthReservation(
 HANDLE hFile,
 DWORD nPeriodMilliseconds,
 DWORD nBytesPerPeriod,
 BOOL bDiscardable,
 LPDWORD lpTransferSize,
 LPDWORD lpNumOutstandingRequests
 );
__declspec(dllimport)
BOOL
__stdcall
GetFileBandwidthReservation(
 HANDLE hFile,
 LPDWORD lpPeriodMilliseconds,
 LPDWORD lpBytesPerPeriod,
 LPBOOL pDiscardable,
 LPDWORD lpTransferSize,
 LPDWORD lpNumOutstandingRequests
 );
__declspec(dllimport)
BOOL
__stdcall
ClearEventLogA (
 HANDLE hEventLog,
 LPCSTR lpBackupFileName
 );
__declspec(dllimport)
BOOL
__stdcall
ClearEventLogW (
 HANDLE hEventLog,
 LPCWSTR lpBackupFileName
 );
__declspec(dllimport)
BOOL
__stdcall
BackupEventLogA (
 HANDLE hEventLog,
 LPCSTR lpBackupFileName
 );
__declspec(dllimport)
BOOL
__stdcall
BackupEventLogW (
 HANDLE hEventLog,
 LPCWSTR lpBackupFileName
 );
__declspec(dllimport)
BOOL
__stdcall
CloseEventLog (
 HANDLE hEventLog
 );
__declspec(dllimport)
BOOL
__stdcall
DeregisterEventSource (
 HANDLE hEventLog
 );
__declspec(dllimport)
BOOL
__stdcall
NotifyChangeEventLog(
 HANDLE hEventLog,
 HANDLE hEvent
 );
__declspec(dllimport)
BOOL
__stdcall
GetNumberOfEventLogRecords (
 HANDLE hEventLog,
 PDWORD NumberOfRecords
 );
__declspec(dllimport)
BOOL
__stdcall
GetOldestEventLogRecord (
 HANDLE hEventLog,
 PDWORD OldestRecord
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenEventLogA (
 LPCSTR lpUNCServerName,
 LPCSTR lpSourceName
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenEventLogW (
 LPCWSTR lpUNCServerName,
 LPCWSTR lpSourceName
 );
__declspec(dllimport)
HANDLE
__stdcall
RegisterEventSourceA (
 LPCSTR lpUNCServerName,
 LPCSTR lpSourceName
 );
__declspec(dllimport)
HANDLE
__stdcall
RegisterEventSourceW (
 LPCWSTR lpUNCServerName,
 LPCWSTR lpSourceName
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenBackupEventLogA (
 LPCSTR lpUNCServerName,
 LPCSTR lpFileName
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenBackupEventLogW (
 LPCWSTR lpUNCServerName,
 LPCWSTR lpFileName
 );
__declspec(dllimport)
BOOL
__stdcall
ReadEventLogA (
 HANDLE hEventLog,
 DWORD dwReadFlags,
 DWORD dwRecordOffset,
 LPVOID lpBuffer,
 DWORD nNumberOfBytesToRead,
 DWORD *pnBytesRead,
 DWORD *pnMinNumberOfBytesNeeded
 );
__declspec(dllimport)
BOOL
__stdcall
ReadEventLogW (
 HANDLE hEventLog,
 DWORD dwReadFlags,
 DWORD dwRecordOffset,
 LPVOID lpBuffer,
 DWORD nNumberOfBytesToRead,
 DWORD *pnBytesRead,
 DWORD *pnMinNumberOfBytesNeeded
 );
__declspec(dllimport)
BOOL
__stdcall
ReportEventA (
 HANDLE hEventLog,
 WORD wType,
 WORD wCategory,
 DWORD dwEventID,
 PSID lpUserSid,
 WORD wNumStrings,
 DWORD dwDataSize,
 LPCSTR *lpStrings,
 LPVOID lpRawData
 );
__declspec(dllimport)
BOOL
__stdcall
ReportEventW (
 HANDLE hEventLog,
 WORD wType,
 WORD wCategory,
 DWORD dwEventID,
 PSID lpUserSid,
 WORD wNumStrings,
 DWORD dwDataSize,
 LPCWSTR *lpStrings,
 LPVOID lpRawData
 );
typedef struct _EVENTLOG_FULL_INFORMATION
{
 DWORD dwFull;
}
EVENTLOG_FULL_INFORMATION, *LPEVENTLOG_FULL_INFORMATION;
__declspec(dllimport)
BOOL
__stdcall
GetEventLogInformation (
 HANDLE hEventLog,
 DWORD dwInfoLevel,
 LPVOID lpBuffer,
 DWORD cbBufSize,
 LPDWORD pcbBytesNeeded
 );
__declspec(dllimport)
BOOL
__stdcall
DuplicateToken(
 HANDLE ExistingTokenHandle,
 SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,
 PHANDLE DuplicateTokenHandle
 );
__declspec(dllimport)
BOOL
__stdcall
GetKernelObjectSecurity (
 HANDLE Handle,
 SECURITY_INFORMATION RequestedInformation,
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 DWORD nLength,
 LPDWORD lpnLengthNeeded
 );
__declspec(dllimport)
BOOL
__stdcall
ImpersonateNamedPipeClient(
 HANDLE hNamedPipe
 );
__declspec(dllimport)
BOOL
__stdcall
ImpersonateSelf(
 SECURITY_IMPERSONATION_LEVEL ImpersonationLevel
 );
__declspec(dllimport)
BOOL
__stdcall
RevertToSelf (
 void
 );
__declspec(dllimport)
BOOL
__stdcall
SetThreadToken (
 PHANDLE Thread,
 HANDLE Token
 );
__declspec(dllimport)
BOOL
__stdcall
AccessCheck (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 HANDLE ClientToken,
 DWORD DesiredAccess,
 PGENERIC_MAPPING GenericMapping,
 PPRIVILEGE_SET PrivilegeSet,
 LPDWORD PrivilegeSetLength,
 LPDWORD GrantedAccess,
 LPBOOL AccessStatus
 );
__declspec(dllimport)
BOOL
__stdcall
AccessCheckByType (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 PSID PrincipalSelfSid,
 HANDLE ClientToken,
 DWORD DesiredAccess,
 POBJECT_TYPE_LIST ObjectTypeList,
 DWORD ObjectTypeListLength,
 PGENERIC_MAPPING GenericMapping,
 PPRIVILEGE_SET PrivilegeSet,
 LPDWORD PrivilegeSetLength,
 LPDWORD GrantedAccess,
 LPBOOL AccessStatus
 );
__declspec(dllimport)
BOOL
__stdcall
AccessCheckByTypeResultList (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 PSID PrincipalSelfSid,
 HANDLE ClientToken,
 DWORD DesiredAccess,
 POBJECT_TYPE_LIST ObjectTypeList,
 DWORD ObjectTypeListLength,
 PGENERIC_MAPPING GenericMapping,
 PPRIVILEGE_SET PrivilegeSet,
 LPDWORD PrivilegeSetLength,
 LPDWORD GrantedAccessList,
 LPDWORD AccessStatusList
 );
__declspec(dllimport)
BOOL
__stdcall
OpenProcessToken (
 HANDLE ProcessHandle,
 DWORD DesiredAccess,
 PHANDLE TokenHandle
 );
__declspec(dllimport)
BOOL
__stdcall
OpenThreadToken (
 HANDLE ThreadHandle,
 DWORD DesiredAccess,
 BOOL OpenAsSelf,
 PHANDLE TokenHandle
 );
__declspec(dllimport)
BOOL
__stdcall
GetTokenInformation (
 HANDLE TokenHandle,
 TOKEN_INFORMATION_CLASS TokenInformationClass,
 LPVOID TokenInformation,
 DWORD TokenInformationLength,
 PDWORD ReturnLength
 );
__declspec(dllimport)
BOOL
__stdcall
SetTokenInformation (
 HANDLE TokenHandle,
 TOKEN_INFORMATION_CLASS TokenInformationClass,
 LPVOID TokenInformation,
 DWORD TokenInformationLength
 );
__declspec(dllimport)
BOOL
__stdcall
AdjustTokenPrivileges (
 HANDLE TokenHandle,
 BOOL DisableAllPrivileges,
 PTOKEN_PRIVILEGES NewState,
 DWORD BufferLength,
 PTOKEN_PRIVILEGES PreviousState,
 PDWORD ReturnLength
 );
__declspec(dllimport)
BOOL
__stdcall
AdjustTokenGroups (
 HANDLE TokenHandle,
 BOOL ResetToDefault,
 PTOKEN_GROUPS NewState,
 DWORD BufferLength,
 PTOKEN_GROUPS PreviousState,
 PDWORD ReturnLength
 );
__declspec(dllimport)
BOOL
__stdcall
PrivilegeCheck (
 HANDLE ClientToken,
 PPRIVILEGE_SET RequiredPrivileges,
 LPBOOL pfResult
 );
__declspec(dllimport)
BOOL
__stdcall
AccessCheckAndAuditAlarmA (
 LPCSTR SubsystemName,
 LPVOID HandleId,
 LPSTR ObjectTypeName,
 LPSTR ObjectName,
 PSECURITY_DESCRIPTOR SecurityDescriptor,
 DWORD DesiredAccess,
 PGENERIC_MAPPING GenericMapping,
 BOOL ObjectCreation,
 LPDWORD GrantedAccess,
 LPBOOL AccessStatus,
 LPBOOL pfGenerateOnClose
 );
__declspec(dllimport)
BOOL
__stdcall
AccessCheckAndAuditAlarmW (
 LPCWSTR SubsystemName,
 LPVOID HandleId,
 LPWSTR ObjectTypeName,
 LPWSTR ObjectName,
 PSECURITY_DESCRIPTOR SecurityDescriptor,
 DWORD DesiredAccess,
 PGENERIC_MAPPING GenericMapping,
 BOOL ObjectCreation,
 LPDWORD GrantedAccess,
 LPBOOL AccessStatus,
 LPBOOL pfGenerateOnClose
 );
__declspec(dllimport)
BOOL
__stdcall
AccessCheckByTypeAndAuditAlarmA (
 LPCSTR SubsystemName,
 LPVOID HandleId,
 LPCSTR ObjectTypeName,
 LPCSTR ObjectName,
 PSECURITY_DESCRIPTOR SecurityDescriptor,
 PSID PrincipalSelfSid,
 DWORD DesiredAccess,
 AUDIT_EVENT_TYPE AuditType,
 DWORD Flags,
 POBJECT_TYPE_LIST ObjectTypeList,
 DWORD ObjectTypeListLength,
 PGENERIC_MAPPING GenericMapping,
 BOOL ObjectCreation,
 LPDWORD GrantedAccess,
 LPBOOL AccessStatus,
 LPBOOL pfGenerateOnClose
 );
__declspec(dllimport)
BOOL
__stdcall
AccessCheckByTypeAndAuditAlarmW (
 LPCWSTR SubsystemName,
 LPVOID HandleId,
 LPCWSTR ObjectTypeName,
 LPCWSTR ObjectName,
 PSECURITY_DESCRIPTOR SecurityDescriptor,
 PSID PrincipalSelfSid,
 DWORD DesiredAccess,
 AUDIT_EVENT_TYPE AuditType,
 DWORD Flags,
 POBJECT_TYPE_LIST ObjectTypeList,
 DWORD ObjectTypeListLength,
 PGENERIC_MAPPING GenericMapping,
 BOOL ObjectCreation,
 LPDWORD GrantedAccess,
 LPBOOL AccessStatus,
 LPBOOL pfGenerateOnClose
 );
__declspec(dllimport)
BOOL
__stdcall
AccessCheckByTypeResultListAndAuditAlarmA (
 LPCSTR SubsystemName,
 LPVOID HandleId,
 LPCSTR ObjectTypeName,
 LPCSTR ObjectName,
 PSECURITY_DESCRIPTOR SecurityDescriptor,
 PSID PrincipalSelfSid,
 DWORD DesiredAccess,
 AUDIT_EVENT_TYPE AuditType,
 DWORD Flags,
 POBJECT_TYPE_LIST ObjectTypeList,
 DWORD ObjectTypeListLength,
 PGENERIC_MAPPING GenericMapping,
 BOOL ObjectCreation,
 LPDWORD GrantedAccess,
 LPDWORD AccessStatusList,
 LPBOOL pfGenerateOnClose
 );
__declspec(dllimport)
BOOL
__stdcall
AccessCheckByTypeResultListAndAuditAlarmW (
 LPCWSTR SubsystemName,
 LPVOID HandleId,
 LPCWSTR ObjectTypeName,
 LPCWSTR ObjectName,
 PSECURITY_DESCRIPTOR SecurityDescriptor,
 PSID PrincipalSelfSid,
 DWORD DesiredAccess,
 AUDIT_EVENT_TYPE AuditType,
 DWORD Flags,
 POBJECT_TYPE_LIST ObjectTypeList,
 DWORD ObjectTypeListLength,
 PGENERIC_MAPPING GenericMapping,
 BOOL ObjectCreation,
 LPDWORD GrantedAccess,
 LPDWORD AccessStatusList,
 LPBOOL pfGenerateOnClose
 );
__declspec(dllimport)
BOOL
__stdcall
AccessCheckByTypeResultListAndAuditAlarmByHandleA (
 LPCSTR SubsystemName,
 LPVOID HandleId,
 HANDLE ClientToken,
 LPCSTR ObjectTypeName,
 LPCSTR ObjectName,
 PSECURITY_DESCRIPTOR SecurityDescriptor,
 PSID PrincipalSelfSid,
 DWORD DesiredAccess,
 AUDIT_EVENT_TYPE AuditType,
 DWORD Flags,
 POBJECT_TYPE_LIST ObjectTypeList,
 DWORD ObjectTypeListLength,
 PGENERIC_MAPPING GenericMapping,
 BOOL ObjectCreation,
 LPDWORD GrantedAccess,
 LPDWORD AccessStatusList,
 LPBOOL pfGenerateOnClose
 );
__declspec(dllimport)
BOOL
__stdcall
AccessCheckByTypeResultListAndAuditAlarmByHandleW (
 LPCWSTR SubsystemName,
 LPVOID HandleId,
 HANDLE ClientToken,
 LPCWSTR ObjectTypeName,
 LPCWSTR ObjectName,
 PSECURITY_DESCRIPTOR SecurityDescriptor,
 PSID PrincipalSelfSid,
 DWORD DesiredAccess,
 AUDIT_EVENT_TYPE AuditType,
 DWORD Flags,
 POBJECT_TYPE_LIST ObjectTypeList,
 DWORD ObjectTypeListLength,
 PGENERIC_MAPPING GenericMapping,
 BOOL ObjectCreation,
 LPDWORD GrantedAccess,
 LPDWORD AccessStatusList,
 LPBOOL pfGenerateOnClose
 );
__declspec(dllimport)
BOOL
__stdcall
ObjectOpenAuditAlarmA (
 LPCSTR SubsystemName,
 LPVOID HandleId,
 LPSTR ObjectTypeName,
 LPSTR ObjectName,
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 HANDLE ClientToken,
 DWORD DesiredAccess,
 DWORD GrantedAccess,
 PPRIVILEGE_SET Privileges,
 BOOL ObjectCreation,
 BOOL AccessGranted,
 LPBOOL GenerateOnClose
 );
__declspec(dllimport)
BOOL
__stdcall
ObjectOpenAuditAlarmW (
 LPCWSTR SubsystemName,
 LPVOID HandleId,
 LPWSTR ObjectTypeName,
 LPWSTR ObjectName,
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 HANDLE ClientToken,
 DWORD DesiredAccess,
 DWORD GrantedAccess,
 PPRIVILEGE_SET Privileges,
 BOOL ObjectCreation,
 BOOL AccessGranted,
 LPBOOL GenerateOnClose
 );
__declspec(dllimport)
BOOL
__stdcall
ObjectPrivilegeAuditAlarmA (
 LPCSTR SubsystemName,
 LPVOID HandleId,
 HANDLE ClientToken,
 DWORD DesiredAccess,
 PPRIVILEGE_SET Privileges,
 BOOL AccessGranted
 );
__declspec(dllimport)
BOOL
__stdcall
ObjectPrivilegeAuditAlarmW (
 LPCWSTR SubsystemName,
 LPVOID HandleId,
 HANDLE ClientToken,
 DWORD DesiredAccess,
 PPRIVILEGE_SET Privileges,
 BOOL AccessGranted
 );
__declspec(dllimport)
BOOL
__stdcall
ObjectCloseAuditAlarmA (
 LPCSTR SubsystemName,
 LPVOID HandleId,
 BOOL GenerateOnClose
 );
__declspec(dllimport)
BOOL
__stdcall
ObjectCloseAuditAlarmW (
 LPCWSTR SubsystemName,
 LPVOID HandleId,
 BOOL GenerateOnClose
 );
__declspec(dllimport)
BOOL
__stdcall
ObjectDeleteAuditAlarmA (
 LPCSTR SubsystemName,
 LPVOID HandleId,
 BOOL GenerateOnClose
 );
__declspec(dllimport)
BOOL
__stdcall
ObjectDeleteAuditAlarmW (
 LPCWSTR SubsystemName,
 LPVOID HandleId,
 BOOL GenerateOnClose
 );
__declspec(dllimport)
BOOL
__stdcall
PrivilegedServiceAuditAlarmA (
 LPCSTR SubsystemName,
 LPCSTR ServiceName,
 HANDLE ClientToken,
 PPRIVILEGE_SET Privileges,
 BOOL AccessGranted
 );
__declspec(dllimport)
BOOL
__stdcall
PrivilegedServiceAuditAlarmW (
 LPCWSTR SubsystemName,
 LPCWSTR ServiceName,
 HANDLE ClientToken,
 PPRIVILEGE_SET Privileges,
 BOOL AccessGranted
 );
__declspec(dllimport)
BOOL
__stdcall
IsWellKnownSid (
 PSID pSid,
 WELL_KNOWN_SID_TYPE WellKnownSidType
 );
__declspec(dllimport)
BOOL
__stdcall
CreateWellKnownSid(
 WELL_KNOWN_SID_TYPE WellKnownSidType,
 PSID DomainSid,
 PSID pSid,
 DWORD *cbSid
 );
__declspec(dllimport)
BOOL
__stdcall
EqualDomainSid(
 PSID pSid1,
 PSID pSid2,
 BOOL *pfEqual
 );
__declspec(dllimport)
BOOL
__stdcall
GetWindowsAccountDomainSid(
 PSID pSid,
 PSID pDomainSid,
 DWORD* cbDomainSid
 );
__declspec(dllimport)
BOOL
__stdcall
IsValidSid (
 PSID pSid
 );
__declspec(dllimport)
BOOL
__stdcall
EqualSid (
 PSID pSid1,
 PSID pSid2
 );
__declspec(dllimport)
BOOL
__stdcall
EqualPrefixSid (
 PSID pSid1,
 PSID pSid2
 );
__declspec(dllimport)
DWORD
__stdcall
GetSidLengthRequired (
 UCHAR nSubAuthorityCount
 );
__declspec(dllimport)
BOOL
__stdcall
AllocateAndInitializeSid (
 PSID_IDENTIFIER_AUTHORITY pIdentifierAuthority,
 BYTE nSubAuthorityCount,
 DWORD nSubAuthority0,
 DWORD nSubAuthority1,
 DWORD nSubAuthority2,
 DWORD nSubAuthority3,
 DWORD nSubAuthority4,
 DWORD nSubAuthority5,
 DWORD nSubAuthority6,
 DWORD nSubAuthority7,
 PSID *pSid
 );
__declspec(dllimport)
PVOID
__stdcall
FreeSid(
 PSID pSid
 );
__declspec(dllimport)
BOOL
__stdcall
InitializeSid (
 PSID Sid,
 PSID_IDENTIFIER_AUTHORITY pIdentifierAuthority,
 BYTE nSubAuthorityCount
 );
__declspec(dllimport)
PSID_IDENTIFIER_AUTHORITY
__stdcall
GetSidIdentifierAuthority (
 PSID pSid
 );
__declspec(dllimport)
PDWORD
__stdcall
GetSidSubAuthority (
 PSID pSid,
 DWORD nSubAuthority
 );
__declspec(dllimport)
PUCHAR
__stdcall
GetSidSubAuthorityCount (
 PSID pSid
 );
__declspec(dllimport)
DWORD
__stdcall
GetLengthSid (
 PSID pSid
 );
__declspec(dllimport)
BOOL
__stdcall
CopySid (
 DWORD nDestinationSidLength,
 PSID pDestinationSid,
 PSID pSourceSid
 );
__declspec(dllimport)
BOOL
__stdcall
AreAllAccessesGranted (
 DWORD GrantedAccess,
 DWORD DesiredAccess
 );
__declspec(dllimport)
BOOL
__stdcall
AreAnyAccessesGranted (
 DWORD GrantedAccess,
 DWORD DesiredAccess
 );
__declspec(dllimport)
void
__stdcall
MapGenericMask (
 PDWORD AccessMask,
 PGENERIC_MAPPING GenericMapping
 );
__declspec(dllimport)
BOOL
__stdcall
IsValidAcl (
 PACL pAcl
 );
__declspec(dllimport)
BOOL
__stdcall
InitializeAcl (
 PACL pAcl,
 DWORD nAclLength,
 DWORD dwAclRevision
 );
__declspec(dllimport)
BOOL
__stdcall
GetAclInformation (
 PACL pAcl,
 LPVOID pAclInformation,
 DWORD nAclInformationLength,
 ACL_INFORMATION_CLASS dwAclInformationClass
 );
__declspec(dllimport)
BOOL
__stdcall
SetAclInformation (
 PACL pAcl,
 LPVOID pAclInformation,
 DWORD nAclInformationLength,
 ACL_INFORMATION_CLASS dwAclInformationClass
 );
__declspec(dllimport)
BOOL
__stdcall
AddAce (
 PACL pAcl,
 DWORD dwAceRevision,
 DWORD dwStartingAceIndex,
 LPVOID pAceList,
 DWORD nAceListLength
 );
__declspec(dllimport)
BOOL
__stdcall
DeleteAce (
 PACL pAcl,
 DWORD dwAceIndex
 );
__declspec(dllimport)
BOOL
__stdcall
GetAce (
 PACL pAcl,
 DWORD dwAceIndex,
 LPVOID *pAce
 );
__declspec(dllimport)
BOOL
__stdcall
AddAccessAllowedAce (
 PACL pAcl,
 DWORD dwAceRevision,
 DWORD AccessMask,
 PSID pSid
 );
__declspec(dllimport)
BOOL
__stdcall
AddAccessAllowedAceEx (
 PACL pAcl,
 DWORD dwAceRevision,
 DWORD AceFlags,
 DWORD AccessMask,
 PSID pSid
 );
__declspec(dllimport)
BOOL
__stdcall
AddMandatoryAce (
 PACL pAcl,
 DWORD dwAceRevision,
 DWORD AceFlags,
 DWORD MandatoryPolicy,
 PSID pLabelSid
 );
__declspec(dllimport)
BOOL
__stdcall
AddAccessDeniedAce (
 PACL pAcl,
 DWORD dwAceRevision,
 DWORD AccessMask,
 PSID pSid
 );
__declspec(dllimport)
BOOL
__stdcall
AddAccessDeniedAceEx (
 PACL pAcl,
 DWORD dwAceRevision,
 DWORD AceFlags,
 DWORD AccessMask,
 PSID pSid
 );
__declspec(dllimport)
BOOL
__stdcall
AddAuditAccessAce(
 PACL pAcl,
 DWORD dwAceRevision,
 DWORD dwAccessMask,
 PSID pSid,
 BOOL bAuditSuccess,
 BOOL bAuditFailure
 );
__declspec(dllimport)
BOOL
__stdcall
AddAuditAccessAceEx(
 PACL pAcl,
 DWORD dwAceRevision,
 DWORD AceFlags,
 DWORD dwAccessMask,
 PSID pSid,
 BOOL bAuditSuccess,
 BOOL bAuditFailure
 );
__declspec(dllimport)
BOOL
__stdcall
AddAccessAllowedObjectAce (
 PACL pAcl,
 DWORD dwAceRevision,
 DWORD AceFlags,
 DWORD AccessMask,
 GUID *ObjectTypeGuid,
 GUID *InheritedObjectTypeGuid,
 PSID pSid
 );
__declspec(dllimport)
BOOL
__stdcall
AddAccessDeniedObjectAce (
 PACL pAcl,
 DWORD dwAceRevision,
 DWORD AceFlags,
 DWORD AccessMask,
 GUID *ObjectTypeGuid,
 GUID *InheritedObjectTypeGuid,
 PSID pSid
 );
__declspec(dllimport)
BOOL
__stdcall
AddAuditAccessObjectAce (
 PACL pAcl,
 DWORD dwAceRevision,
 DWORD AceFlags,
 DWORD AccessMask,
 GUID *ObjectTypeGuid,
 GUID *InheritedObjectTypeGuid,
 PSID pSid,
 BOOL bAuditSuccess,
 BOOL bAuditFailure
 );
__declspec(dllimport)
BOOL
__stdcall
FindFirstFreeAce (
 PACL pAcl,
 LPVOID *pAce
 );
__declspec(dllimport)
BOOL
__stdcall
InitializeSecurityDescriptor (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 DWORD dwRevision
 );
__declspec(dllimport)
BOOL
__stdcall
IsValidSecurityDescriptor (
 PSECURITY_DESCRIPTOR pSecurityDescriptor
 );
__declspec(dllimport)
BOOL
__stdcall
IsValidRelativeSecurityDescriptor (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 ULONG SecurityDescriptorLength,
 SECURITY_INFORMATION RequiredInformation
 );
__declspec(dllimport)
DWORD
__stdcall
GetSecurityDescriptorLength (
 PSECURITY_DESCRIPTOR pSecurityDescriptor
 );
__declspec(dllimport)
BOOL
__stdcall
GetSecurityDescriptorControl (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 PSECURITY_DESCRIPTOR_CONTROL pControl,
 LPDWORD lpdwRevision
 );
__declspec(dllimport)
BOOL
__stdcall
SetSecurityDescriptorControl (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 SECURITY_DESCRIPTOR_CONTROL ControlBitsOfInterest,
 SECURITY_DESCRIPTOR_CONTROL ControlBitsToSet
 );
__declspec(dllimport)
BOOL
__stdcall
SetSecurityDescriptorDacl (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 BOOL bDaclPresent,
 PACL pDacl,
 BOOL bDaclDefaulted
 );
__declspec(dllimport)
BOOL
__stdcall
GetSecurityDescriptorDacl (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 LPBOOL lpbDaclPresent,
 PACL *pDacl,
 LPBOOL lpbDaclDefaulted
 );
__declspec(dllimport)
BOOL
__stdcall
SetSecurityDescriptorSacl (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 BOOL bSaclPresent,
 PACL pSacl,
 BOOL bSaclDefaulted
 );
__declspec(dllimport)
BOOL
__stdcall
GetSecurityDescriptorSacl (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 LPBOOL lpbSaclPresent,
 PACL *pSacl,
 LPBOOL lpbSaclDefaulted
 );
__declspec(dllimport)
BOOL
__stdcall
SetSecurityDescriptorOwner (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 PSID pOwner,
 BOOL bOwnerDefaulted
 );
__declspec(dllimport)
BOOL
__stdcall
GetSecurityDescriptorOwner (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 PSID *pOwner,
 LPBOOL lpbOwnerDefaulted
 );
__declspec(dllimport)
BOOL
__stdcall
SetSecurityDescriptorGroup (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 PSID pGroup,
 BOOL bGroupDefaulted
 );
__declspec(dllimport)
BOOL
__stdcall
GetSecurityDescriptorGroup (
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 PSID *pGroup,
 LPBOOL lpbGroupDefaulted
 );
__declspec(dllimport)
DWORD
__stdcall
SetSecurityDescriptorRMControl(
 PSECURITY_DESCRIPTOR SecurityDescriptor,
 PUCHAR RMControl
 );
__declspec(dllimport)
DWORD
__stdcall
GetSecurityDescriptorRMControl(
 PSECURITY_DESCRIPTOR SecurityDescriptor,
 PUCHAR RMControl
 );
__declspec(dllimport)
BOOL
__stdcall
CreatePrivateObjectSecurity (
 PSECURITY_DESCRIPTOR ParentDescriptor,
 PSECURITY_DESCRIPTOR CreatorDescriptor,
 PSECURITY_DESCRIPTOR * NewDescriptor,
 BOOL IsDirectoryObject,
 HANDLE Token,
 PGENERIC_MAPPING GenericMapping
 );
__declspec(dllimport)
BOOL
__stdcall
ConvertToAutoInheritPrivateObjectSecurity(
 PSECURITY_DESCRIPTOR ParentDescriptor,
 PSECURITY_DESCRIPTOR CurrentSecurityDescriptor,
 PSECURITY_DESCRIPTOR *NewSecurityDescriptor,
 GUID *ObjectType,
 BOOLEAN IsDirectoryObject,
 PGENERIC_MAPPING GenericMapping
 );
__declspec(dllimport)
BOOL
__stdcall
CreatePrivateObjectSecurityEx (
 PSECURITY_DESCRIPTOR ParentDescriptor,
 PSECURITY_DESCRIPTOR CreatorDescriptor,
 PSECURITY_DESCRIPTOR * NewDescriptor,
 GUID *ObjectType,
 BOOL IsContainerObject,
 ULONG AutoInheritFlags,
 HANDLE Token,
 PGENERIC_MAPPING GenericMapping
 );
__declspec(dllimport)
BOOL
__stdcall
CreatePrivateObjectSecurityWithMultipleInheritance (
 PSECURITY_DESCRIPTOR ParentDescriptor,
 PSECURITY_DESCRIPTOR CreatorDescriptor,
 PSECURITY_DESCRIPTOR * NewDescriptor,
 GUID **ObjectTypes,
 ULONG GuidCount,
 BOOL IsContainerObject,
 ULONG AutoInheritFlags,
 HANDLE Token,
 PGENERIC_MAPPING GenericMapping
 );
__declspec(dllimport)
BOOL
__stdcall
SetPrivateObjectSecurity (
 SECURITY_INFORMATION SecurityInformation,
 PSECURITY_DESCRIPTOR ModificationDescriptor,
 PSECURITY_DESCRIPTOR *ObjectsSecurityDescriptor,
 PGENERIC_MAPPING GenericMapping,
 HANDLE Token
 );
__declspec(dllimport)
BOOL
__stdcall
SetPrivateObjectSecurityEx (
 SECURITY_INFORMATION SecurityInformation,
 PSECURITY_DESCRIPTOR ModificationDescriptor,
 PSECURITY_DESCRIPTOR *ObjectsSecurityDescriptor,
 ULONG AutoInheritFlags,
 PGENERIC_MAPPING GenericMapping,
 HANDLE Token
 );
__declspec(dllimport)
BOOL
__stdcall
GetPrivateObjectSecurity (
 PSECURITY_DESCRIPTOR ObjectDescriptor,
 SECURITY_INFORMATION SecurityInformation,
 PSECURITY_DESCRIPTOR ResultantDescriptor,
 DWORD DescriptorLength,
 PDWORD ReturnLength
 );
__declspec(dllimport)
BOOL
__stdcall
DestroyPrivateObjectSecurity (
 PSECURITY_DESCRIPTOR * ObjectDescriptor
 );
__declspec(dllimport)
BOOL
__stdcall
MakeSelfRelativeSD (
 PSECURITY_DESCRIPTOR pAbsoluteSecurityDescriptor,
 PSECURITY_DESCRIPTOR pSelfRelativeSecurityDescriptor,
 LPDWORD lpdwBufferLength
 );
__declspec(dllimport)
BOOL
__stdcall
MakeAbsoluteSD (
 PSECURITY_DESCRIPTOR pSelfRelativeSecurityDescriptor,
 PSECURITY_DESCRIPTOR pAbsoluteSecurityDescriptor,
 LPDWORD lpdwAbsoluteSecurityDescriptorSize,
 PACL pDacl,
 LPDWORD lpdwDaclSize,
 PACL pSacl,
 LPDWORD lpdwSaclSize,
 PSID pOwner,
 LPDWORD lpdwOwnerSize,
 PSID pPrimaryGroup,
 LPDWORD lpdwPrimaryGroupSize
 );
__declspec(dllimport)
BOOL
__stdcall
MakeAbsoluteSD2 (
 PSECURITY_DESCRIPTOR pSelfRelativeSecurityDescriptor,
 LPDWORD lpdwBufferSize
 );
__declspec(dllimport)
void
__stdcall
QuerySecurityAccessMask(
 SECURITY_INFORMATION SecurityInformation,
 LPDWORD DesiredAccess
 );
__declspec(dllimport)
void
__stdcall
SetSecurityAccessMask(
 SECURITY_INFORMATION SecurityInformation,
 LPDWORD DesiredAccess
 );
__declspec(dllimport)
BOOL
__stdcall
SetFileSecurityA (
 LPCSTR lpFileName,
 SECURITY_INFORMATION SecurityInformation,
 PSECURITY_DESCRIPTOR pSecurityDescriptor
 );
__declspec(dllimport)
BOOL
__stdcall
SetFileSecurityW (
 LPCWSTR lpFileName,
 SECURITY_INFORMATION SecurityInformation,
 PSECURITY_DESCRIPTOR pSecurityDescriptor
 );
__declspec(dllimport)
BOOL
__stdcall
GetFileSecurityA (
 LPCSTR lpFileName,
 SECURITY_INFORMATION RequestedInformation,
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 DWORD nLength,
 LPDWORD lpnLengthNeeded
 );
__declspec(dllimport)
BOOL
__stdcall
GetFileSecurityW (
 LPCWSTR lpFileName,
 SECURITY_INFORMATION RequestedInformation,
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 DWORD nLength,
 LPDWORD lpnLengthNeeded
 );
__declspec(dllimport)
BOOL
__stdcall
SetKernelObjectSecurity (
 HANDLE Handle,
 SECURITY_INFORMATION SecurityInformation,
 PSECURITY_DESCRIPTOR SecurityDescriptor
 );
__declspec(dllimport)
HANDLE
__stdcall
FindFirstChangeNotificationA(
 LPCSTR lpPathName,
 BOOL bWatchSubtree,
 DWORD dwNotifyFilter
 );
__declspec(dllimport)
HANDLE
__stdcall
FindFirstChangeNotificationW(
 LPCWSTR lpPathName,
 BOOL bWatchSubtree,
 DWORD dwNotifyFilter
 );
__declspec(dllimport)
BOOL
__stdcall
FindNextChangeNotification(
 HANDLE hChangeHandle
 );
__declspec(dllimport)
BOOL
__stdcall
FindCloseChangeNotification(
 HANDLE hChangeHandle
 );
__declspec(dllimport)
BOOL
__stdcall
ReadDirectoryChangesW(
 HANDLE hDirectory,
 LPVOID lpBuffer,
 DWORD nBufferLength,
 BOOL bWatchSubtree,
 DWORD dwNotifyFilter,
 LPDWORD lpBytesReturned,
 LPOVERLAPPED lpOverlapped,
 LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine
 );
__declspec(dllimport)
BOOL
__stdcall
VirtualLock(
 LPVOID lpAddress,
 SIZE_T dwSize
 );
__declspec(dllimport)
BOOL
__stdcall
VirtualUnlock(
 LPVOID lpAddress,
 SIZE_T dwSize
 );
__declspec(dllimport)
LPVOID
__stdcall
MapViewOfFileEx(
 HANDLE hFileMappingObject,
 DWORD dwDesiredAccess,
 DWORD dwFileOffsetHigh,
 DWORD dwFileOffsetLow,
 SIZE_T dwNumberOfBytesToMap,
 LPVOID lpBaseAddress
 );
__declspec(dllimport)
LPVOID
__stdcall
MapViewOfFileExNuma(
 HANDLE hFileMappingObject,
 DWORD dwDesiredAccess,
 DWORD dwFileOffsetHigh,
 DWORD dwFileOffsetLow,
 SIZE_T dwNumberOfBytesToMap,
 LPVOID lpBaseAddress,
 DWORD nndPreferred
 );
__declspec(dllimport)
BOOL
__stdcall
SetPriorityClass(
 HANDLE hProcess,
 DWORD dwPriorityClass
 );
__declspec(dllimport)
DWORD
__stdcall
GetPriorityClass(
 HANDLE hProcess
 );
__declspec(dllimport)
BOOL
__stdcall
IsBadReadPtr(
 const void *lp,
 UINT_PTR ucb
 );
__declspec(dllimport)
BOOL
__stdcall
IsBadWritePtr(
 LPVOID lp,
 UINT_PTR ucb
 );
__declspec(dllimport)
BOOL
__stdcall
IsBadHugeReadPtr(
 const void *lp,
 UINT_PTR ucb
 );
__declspec(dllimport)
BOOL
__stdcall
IsBadHugeWritePtr(
 LPVOID lp,
 UINT_PTR ucb
 );
__declspec(dllimport)
BOOL
__stdcall
IsBadCodePtr(
 FARPROC lpfn
 );
__declspec(dllimport)
BOOL
__stdcall
IsBadStringPtrA(
 LPCSTR lpsz,
 UINT_PTR ucchMax
 );
__declspec(dllimport)
BOOL
__stdcall
IsBadStringPtrW(
 LPCWSTR lpsz,
 UINT_PTR ucchMax
 );
__declspec(dllimport)
BOOL
__stdcall
LookupAccountSidA(
 LPCSTR lpSystemName,
 PSID Sid,
 LPSTR Name,
 LPDWORD cchName,
 LPSTR ReferencedDomainName,
 LPDWORD cchReferencedDomainName,
 PSID_NAME_USE peUse
 );
__declspec(dllimport)
BOOL
__stdcall
LookupAccountSidW(
 LPCWSTR lpSystemName,
 PSID Sid,
 LPWSTR Name,
 LPDWORD cchName,
 LPWSTR ReferencedDomainName,
 LPDWORD cchReferencedDomainName,
 PSID_NAME_USE peUse
 );
__declspec(dllimport)
BOOL
__stdcall
LookupAccountNameA(
 LPCSTR lpSystemName,
 LPCSTR lpAccountName,
 PSID Sid,
 LPDWORD cbSid,
 LPSTR ReferencedDomainName,
 LPDWORD cchReferencedDomainName,
 PSID_NAME_USE peUse
 );
__declspec(dllimport)
BOOL
__stdcall
LookupAccountNameW(
 LPCWSTR lpSystemName,
 LPCWSTR lpAccountName,
 PSID Sid,
 LPDWORD cbSid,
 LPWSTR ReferencedDomainName,
 LPDWORD cchReferencedDomainName,
 PSID_NAME_USE peUse
 );
__declspec(dllimport)
BOOL
__stdcall
LookupPrivilegeValueA(
 LPCSTR lpSystemName,
 LPCSTR lpName,
 PLUID lpLuid
 );
__declspec(dllimport)
BOOL
__stdcall
LookupPrivilegeValueW(
 LPCWSTR lpSystemName,
 LPCWSTR lpName,
 PLUID lpLuid
 );
__declspec(dllimport)
BOOL
__stdcall
LookupPrivilegeNameA(
 LPCSTR lpSystemName,
 PLUID lpLuid,
 LPSTR lpName,
 LPDWORD cchName
 );
__declspec(dllimport)
BOOL
__stdcall
LookupPrivilegeNameW(
 LPCWSTR lpSystemName,
 PLUID lpLuid,
 LPWSTR lpName,
 LPDWORD cchName
 );
__declspec(dllimport)
BOOL
__stdcall
LookupPrivilegeDisplayNameA(
 LPCSTR lpSystemName,
 LPCSTR lpName,
 LPSTR lpDisplayName,
 LPDWORD cchDisplayName,
 LPDWORD lpLanguageId
 );
__declspec(dllimport)
BOOL
__stdcall
LookupPrivilegeDisplayNameW(
 LPCWSTR lpSystemName,
 LPCWSTR lpName,
 LPWSTR lpDisplayName,
 LPDWORD cchDisplayName,
 LPDWORD lpLanguageId
 );
__declspec(dllimport)
BOOL
__stdcall
AllocateLocallyUniqueId(
 PLUID Luid
 );
__declspec(dllimport)
BOOL
__stdcall
BuildCommDCBA(
 LPCSTR lpDef,
 LPDCB lpDCB
 );
__declspec(dllimport)
BOOL
__stdcall
BuildCommDCBW(
 LPCWSTR lpDef,
 LPDCB lpDCB
 );
__declspec(dllimport)
BOOL
__stdcall
BuildCommDCBAndTimeoutsA(
 LPCSTR lpDef,
 LPDCB lpDCB,
 LPCOMMTIMEOUTS lpCommTimeouts
 );
__declspec(dllimport)
BOOL
__stdcall
BuildCommDCBAndTimeoutsW(
 LPCWSTR lpDef,
 LPDCB lpDCB,
 LPCOMMTIMEOUTS lpCommTimeouts
 );
__declspec(dllimport)
BOOL
__stdcall
CommConfigDialogA(
 LPCSTR lpszName,
 HWND hWnd,
 LPCOMMCONFIG lpCC
 );
__declspec(dllimport)
BOOL
__stdcall
CommConfigDialogW(
 LPCWSTR lpszName,
 HWND hWnd,
 LPCOMMCONFIG lpCC
 );
__declspec(dllimport)
BOOL
__stdcall
GetDefaultCommConfigA(
 LPCSTR lpszName,
 LPCOMMCONFIG lpCC,
 LPDWORD lpdwSize
 );
__declspec(dllimport)
BOOL
__stdcall
GetDefaultCommConfigW(
 LPCWSTR lpszName,
 LPCOMMCONFIG lpCC,
 LPDWORD lpdwSize
 );
__declspec(dllimport)
BOOL
__stdcall
SetDefaultCommConfigA(
 LPCSTR lpszName,
 LPCOMMCONFIG lpCC,
 DWORD dwSize
 );
__declspec(dllimport)
BOOL
__stdcall
SetDefaultCommConfigW(
 LPCWSTR lpszName,
 LPCOMMCONFIG lpCC,
 DWORD dwSize
 );
__declspec(dllimport)
BOOL
__stdcall
GetComputerNameA (
 LPSTR lpBuffer,
 LPDWORD nSize
 );
__declspec(dllimport)
BOOL
__stdcall
GetComputerNameW (
 LPWSTR lpBuffer,
 LPDWORD nSize
 );
__declspec(dllimport)
BOOL
__stdcall
SetComputerNameA (
 LPCSTR lpComputerName
 );
__declspec(dllimport)
BOOL
__stdcall
SetComputerNameW (
 LPCWSTR lpComputerName
 );
typedef enum _COMPUTER_NAME_FORMAT {
 ComputerNameNetBIOS,
 ComputerNameDnsHostname,
 ComputerNameDnsDomain,
 ComputerNameDnsFullyQualified,
 ComputerNamePhysicalNetBIOS,
 ComputerNamePhysicalDnsHostname,
 ComputerNamePhysicalDnsDomain,
 ComputerNamePhysicalDnsFullyQualified,
 ComputerNameMax
} COMPUTER_NAME_FORMAT ;
__declspec(dllimport)
BOOL
__stdcall
GetComputerNameExA (
 COMPUTER_NAME_FORMAT NameType,
 LPSTR lpBuffer,
 LPDWORD nSize
 );
__declspec(dllimport)
BOOL
__stdcall
GetComputerNameExW (
 COMPUTER_NAME_FORMAT NameType,
 LPWSTR lpBuffer,
 LPDWORD nSize
 );
__declspec(dllimport)
BOOL
__stdcall
SetComputerNameExA (
 COMPUTER_NAME_FORMAT NameType,
 LPCSTR lpBuffer
 );
__declspec(dllimport)
BOOL
__stdcall
SetComputerNameExW (
 COMPUTER_NAME_FORMAT NameType,
 LPCWSTR lpBuffer
 );
__declspec(dllimport)
BOOL
__stdcall
DnsHostnameToComputerNameA (
 LPCSTR Hostname,
 LPSTR ComputerName,
 LPDWORD nSize
 );
__declspec(dllimport)
BOOL
__stdcall
DnsHostnameToComputerNameW (
 LPCWSTR Hostname,
 LPWSTR ComputerName,
 LPDWORD nSize
 );
__declspec(dllimport)
BOOL
__stdcall
GetUserNameA (
 LPSTR lpBuffer,
 LPDWORD pcbBuffer
 );
__declspec(dllimport)
BOOL
__stdcall
GetUserNameW (
 LPWSTR lpBuffer,
 LPDWORD pcbBuffer
 );
__declspec(dllimport)
BOOL
__stdcall
LogonUserA (
 LPCSTR lpszUsername,
 LPCSTR lpszDomain,
 LPCSTR lpszPassword,
 DWORD dwLogonType,
 DWORD dwLogonProvider,
 PHANDLE phToken
 );
__declspec(dllimport)
BOOL
__stdcall
LogonUserW (
 LPCWSTR lpszUsername,
 LPCWSTR lpszDomain,
 LPCWSTR lpszPassword,
 DWORD dwLogonType,
 DWORD dwLogonProvider,
 PHANDLE phToken
 );
__declspec(dllimport)
BOOL
__stdcall
LogonUserExA (
 LPCSTR lpszUsername,
 LPCSTR lpszDomain,
 LPCSTR lpszPassword,
 DWORD dwLogonType,
 DWORD dwLogonProvider,
 PHANDLE phToken,
 PSID *ppLogonSid,
 PVOID *ppProfileBuffer,
 LPDWORD pdwProfileLength,
 PQUOTA_LIMITS pQuotaLimits
 );
__declspec(dllimport)
BOOL
__stdcall
LogonUserExW (
 LPCWSTR lpszUsername,
 LPCWSTR lpszDomain,
 LPCWSTR lpszPassword,
 DWORD dwLogonType,
 DWORD dwLogonProvider,
 PHANDLE phToken,
 PSID *ppLogonSid,
 PVOID *ppProfileBuffer,
 LPDWORD pdwProfileLength,
 PQUOTA_LIMITS pQuotaLimits
 );
__declspec(dllimport)
BOOL
__stdcall
ImpersonateLoggedOnUser(
 HANDLE hToken
 );
__declspec(dllimport)
BOOL
__stdcall
CreateProcessAsUserA (
 HANDLE hToken,
 LPCSTR lpApplicationName,
 LPSTR lpCommandLine,
 LPSECURITY_ATTRIBUTES lpProcessAttributes,
 LPSECURITY_ATTRIBUTES lpThreadAttributes,
 BOOL bInheritHandles,
 DWORD dwCreationFlags,
 LPVOID lpEnvironment,
 LPCSTR lpCurrentDirectory,
 LPSTARTUPINFOA lpStartupInfo,
 LPPROCESS_INFORMATION lpProcessInformation
 );
__declspec(dllimport)
BOOL
__stdcall
CreateProcessAsUserW (
 HANDLE hToken,
 LPCWSTR lpApplicationName,
 LPWSTR lpCommandLine,
 LPSECURITY_ATTRIBUTES lpProcessAttributes,
 LPSECURITY_ATTRIBUTES lpThreadAttributes,
 BOOL bInheritHandles,
 DWORD dwCreationFlags,
 LPVOID lpEnvironment,
 LPCWSTR lpCurrentDirectory,
 LPSTARTUPINFOW lpStartupInfo,
 LPPROCESS_INFORMATION lpProcessInformation
 );
__declspec(dllimport)
BOOL
__stdcall
CreateProcessWithLogonW(
 LPCWSTR lpUsername,
 LPCWSTR lpDomain,
 LPCWSTR lpPassword,
 DWORD dwLogonFlags,
 LPCWSTR lpApplicationName,
 LPWSTR lpCommandLine,
 DWORD dwCreationFlags,
 LPVOID lpEnvironment,
 LPCWSTR lpCurrentDirectory,
 LPSTARTUPINFOW lpStartupInfo,
 LPPROCESS_INFORMATION lpProcessInformation
 );
__declspec(dllimport)
BOOL
__stdcall
CreateProcessWithTokenW(
 HANDLE hToken,
 DWORD dwLogonFlags,
 LPCWSTR lpApplicationName,
 LPWSTR lpCommandLine,
 DWORD dwCreationFlags,
 LPVOID lpEnvironment,
 LPCWSTR lpCurrentDirectory,
 LPSTARTUPINFOW lpStartupInfo,
 LPPROCESS_INFORMATION lpProcessInformation
 );
__declspec(dllimport)
BOOL
__stdcall
ImpersonateAnonymousToken(
 HANDLE ThreadHandle
 );
__declspec(dllimport)
BOOL
__stdcall
DuplicateTokenEx(
 HANDLE hExistingToken,
 DWORD dwDesiredAccess,
 LPSECURITY_ATTRIBUTES lpTokenAttributes,
 SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,
 TOKEN_TYPE TokenType,
 PHANDLE phNewToken);
__declspec(dllimport)
BOOL
__stdcall
CreateRestrictedToken(
 HANDLE ExistingTokenHandle,
 DWORD Flags,
 DWORD DisableSidCount,
 PSID_AND_ATTRIBUTES SidsToDisable,
 DWORD DeletePrivilegeCount,
 PLUID_AND_ATTRIBUTES PrivilegesToDelete,
 DWORD RestrictedSidCount,
 PSID_AND_ATTRIBUTES SidsToRestrict,
 PHANDLE NewTokenHandle
 );
__declspec(dllimport)
BOOL
__stdcall
IsTokenRestricted(
 HANDLE TokenHandle
 );
__declspec(dllimport)
BOOL
__stdcall
IsTokenUntrusted(
 HANDLE TokenHandle
 );
__declspec(dllimport)
BOOL
__stdcall
CheckTokenMembership(
 HANDLE TokenHandle,
 PSID SidToCheck,
 PBOOL IsMember
 );
typedef WAITORTIMERCALLBACKFUNC WAITORTIMERCALLBACK ;
__declspec(dllimport)
BOOL
__stdcall
RegisterWaitForSingleObject(
 PHANDLE phNewWaitObject,
 HANDLE hObject,
 WAITORTIMERCALLBACK Callback,
 PVOID Context,
 ULONG dwMilliseconds,
 ULONG dwFlags
 );
__declspec(dllimport)
HANDLE
__stdcall
RegisterWaitForSingleObjectEx(
 HANDLE hObject,
 WAITORTIMERCALLBACK Callback,
 PVOID Context,
 ULONG dwMilliseconds,
 ULONG dwFlags
 );
__declspec(dllimport)
BOOL
__stdcall
UnregisterWait(
 HANDLE WaitHandle
 );
__declspec(dllimport)
BOOL
__stdcall
UnregisterWaitEx(
 HANDLE WaitHandle,
 HANDLE CompletionEvent
 );
__declspec(dllimport)
BOOL
__stdcall
QueueUserWorkItem(
 LPTHREAD_START_ROUTINE Function,
 PVOID Context,
 ULONG Flags
 );
__declspec(dllimport)
BOOL
__stdcall
BindIoCompletionCallback (
 HANDLE FileHandle,
 LPOVERLAPPED_COMPLETION_ROUTINE Function,
 ULONG Flags
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateTimerQueue(
 void
 );
__declspec(dllimport)
BOOL
__stdcall
CreateTimerQueueTimer(
 PHANDLE phNewTimer,
 HANDLE TimerQueue,
 WAITORTIMERCALLBACK Callback,
 PVOID Parameter,
 DWORD DueTime,
 DWORD Period,
 ULONG Flags
 ) ;
__declspec(dllimport)
BOOL
__stdcall
ChangeTimerQueueTimer(
 HANDLE TimerQueue,
 HANDLE Timer,
 ULONG DueTime,
 ULONG Period
 );
__declspec(dllimport)
BOOL
__stdcall
DeleteTimerQueueTimer(
 HANDLE TimerQueue,
 HANDLE Timer,
 HANDLE CompletionEvent
 );
__declspec(dllimport)
BOOL
__stdcall
DeleteTimerQueueEx(
 HANDLE TimerQueue,
 HANDLE CompletionEvent
 );
__declspec(dllimport)
HANDLE
__stdcall
SetTimerQueueTimer(
 HANDLE TimerQueue,
 WAITORTIMERCALLBACK Callback,
 PVOID Parameter,
 DWORD DueTime,
 DWORD Period,
 BOOL PreferIo
 );
__declspec(dllimport)
BOOL
__stdcall
CancelTimerQueueTimer(
 HANDLE TimerQueue,
 HANDLE Timer
 );
__declspec(dllimport)
BOOL
__stdcall
DeleteTimerQueue(
 HANDLE TimerQueue
 );
typedef void (__stdcall *PTP_WIN32_IO_CALLBACK)(
 PTP_CALLBACK_INSTANCE Instance,
 PVOID Context,
 PVOID Overlapped,
 ULONG IoResult,
 ULONG_PTR NumberOfBytesTransferred,
 PTP_IO Io
 );
__declspec(dllimport)
PTP_POOL
__stdcall
CreateThreadpool(
 PVOID reserved
 );
__declspec(dllimport)
void
__stdcall
SetThreadpoolThreadMaximum(
 PTP_POOL ptpp,
 DWORD cthrdMost
 );
__declspec(dllimport)
BOOL
__stdcall
SetThreadpoolThreadMinimum(
 PTP_POOL ptpp,
 DWORD cthrdMic
 );
__declspec(dllimport)
void
__stdcall
CloseThreadpool(
 PTP_POOL ptpp
 );
__declspec(dllimport)
PTP_CLEANUP_GROUP
__stdcall
CreateThreadpoolCleanupGroup(
 void
 );
__declspec(dllimport)
void
__stdcall
CloseThreadpoolCleanupGroupMembers(
 PTP_CLEANUP_GROUP ptpcg,
 BOOL fCancelPendingCallbacks,
 PVOID pvCleanupContext
 );
__declspec(dllimport)
void
__stdcall
CloseThreadpoolCleanupGroup(
 PTP_CLEANUP_GROUP ptpcg
 );
__forceinline
void
InitializeThreadpoolEnvironment(
 PTP_CALLBACK_ENVIRON pcbe
 )
{
 TpInitializeCallbackEnviron(pcbe);
}
__forceinline
void
SetThreadpoolCallbackPool(
 PTP_CALLBACK_ENVIRON pcbe,
 PTP_POOL ptpp
 )
{
 TpSetCallbackThreadpool(pcbe, ptpp);
}
__forceinline
void
SetThreadpoolCallbackCleanupGroup(
 PTP_CALLBACK_ENVIRON pcbe,
 PTP_CLEANUP_GROUP ptpcg,
 PTP_CLEANUP_GROUP_CANCEL_CALLBACK pfng
 )
{
 TpSetCallbackCleanupGroup(pcbe, ptpcg, pfng);
}
__forceinline
void
SetThreadpoolCallbackRunsLong(
 PTP_CALLBACK_ENVIRON pcbe
 )
{
 TpSetCallbackLongFunction(pcbe);
}
__forceinline
void
SetThreadpoolCallbackLibrary(
 PTP_CALLBACK_ENVIRON pcbe,
 PVOID mod
 )
{
 TpSetCallbackRaceWithDll(pcbe, mod);
}
__forceinline
void
DestroyThreadpoolEnvironment(
 PTP_CALLBACK_ENVIRON pcbe
 )
{
 TpDestroyCallbackEnviron(pcbe);
}
__declspec(dllimport)
void
__stdcall
SetEventWhenCallbackReturns(
 PTP_CALLBACK_INSTANCE pci,
 HANDLE evt
 );
__declspec(dllimport)
void
__stdcall
ReleaseSemaphoreWhenCallbackReturns(
 PTP_CALLBACK_INSTANCE pci,
 HANDLE sem,
 DWORD crel
 );
__declspec(dllimport)
void
__stdcall
ReleaseMutexWhenCallbackReturns(
 PTP_CALLBACK_INSTANCE pci,
 HANDLE mut
 );
__declspec(dllimport)
void
__stdcall
LeaveCriticalSectionWhenCallbackReturns(
 PTP_CALLBACK_INSTANCE pci,
 PCRITICAL_SECTION pcs
 );
__declspec(dllimport)
void
__stdcall
FreeLibraryWhenCallbackReturns(
 PTP_CALLBACK_INSTANCE pci,
 HMODULE mod
 );
__declspec(dllimport)
BOOL
__stdcall
CallbackMayRunLong(
 PTP_CALLBACK_INSTANCE pci
 );
__declspec(dllimport)
void
__stdcall
DisassociateCurrentThreadFromCallback(
 PTP_CALLBACK_INSTANCE pci
 );
__declspec(dllimport)
BOOL
__stdcall
TrySubmitThreadpoolCallback(
 PTP_SIMPLE_CALLBACK pfns,
 PVOID pv,
 PTP_CALLBACK_ENVIRON pcbe
 );
__declspec(dllimport)
PTP_WORK
__stdcall
CreateThreadpoolWork(
 PTP_WORK_CALLBACK pfnwk,
 PVOID pv,
 PTP_CALLBACK_ENVIRON pcbe
 );
__declspec(dllimport)
void
__stdcall
SubmitThreadpoolWork(
 PTP_WORK pwk
 );
__declspec(dllimport)
void
__stdcall
WaitForThreadpoolWorkCallbacks(
 PTP_WORK pwk,
 BOOL fCancelPendingCallbacks
 );
__declspec(dllimport)
void
__stdcall
CloseThreadpoolWork(
 PTP_WORK pwk
 );
__declspec(dllimport)
PTP_TIMER
__stdcall
CreateThreadpoolTimer(
 PTP_TIMER_CALLBACK pfnti,
 PVOID pv,
 PTP_CALLBACK_ENVIRON pcbe
 );
__declspec(dllimport)
void
__stdcall
SetThreadpoolTimer(
 PTP_TIMER pti,
 PFILETIME pftDueTime,
 DWORD msPeriod,
 DWORD msWindowLength
 );
__declspec(dllimport)
BOOL
__stdcall
IsThreadpoolTimerSet(
 PTP_TIMER pti
 );
__declspec(dllimport)
void
__stdcall
WaitForThreadpoolTimerCallbacks(
 PTP_TIMER pti,
 BOOL fCancelPendingCallbacks
 );
__declspec(dllimport)
void
__stdcall
CloseThreadpoolTimer(
 PTP_TIMER pti
 );
__declspec(dllimport)
PTP_WAIT
__stdcall
CreateThreadpoolWait(
 PTP_WAIT_CALLBACK pfnwa,
 PVOID pv,
 PTP_CALLBACK_ENVIRON pcbe
 );
__declspec(dllimport)
void
__stdcall
SetThreadpoolWait(
 PTP_WAIT pwa,
 HANDLE h,
 PFILETIME pftTimeout
 );
__declspec(dllimport)
void
__stdcall
WaitForThreadpoolWaitCallbacks(
 PTP_WAIT pwa,
 BOOL fCancelPendingCallbacks
 );
__declspec(dllimport)
void
__stdcall
CloseThreadpoolWait(
 PTP_WAIT pwa
 );
__declspec(dllimport)
PTP_IO
__stdcall
CreateThreadpoolIo(
 HANDLE fl,
 PTP_WIN32_IO_CALLBACK pfnio,
 PVOID pv,
 PTP_CALLBACK_ENVIRON pcbe
 );
__declspec(dllimport)
void
__stdcall
StartThreadpoolIo(
 PTP_IO pio
 );
__declspec(dllimport)
void
__stdcall
CancelThreadpoolIo(
 PTP_IO pio
 );
__declspec(dllimport)
void
__stdcall
WaitForThreadpoolIoCallbacks(
 PTP_IO pio,
 BOOL fCancelPendingCallbacks
 );
__declspec(dllimport)
void
__stdcall
CloseThreadpoolIo(
 PTP_IO pio
 );
__declspec(dllimport)
HANDLE
__stdcall
CreatePrivateNamespaceA(
 LPSECURITY_ATTRIBUTES lpPrivateNamespaceAttributes,
 LPVOID lpBoundaryDescriptor,
 LPCSTR lpAliasPrefix
 );
__declspec(dllimport)
HANDLE
__stdcall
CreatePrivateNamespaceW(
 LPSECURITY_ATTRIBUTES lpPrivateNamespaceAttributes,
 LPVOID lpBoundaryDescriptor,
 LPCWSTR lpAliasPrefix
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenPrivateNamespaceA(
 LPVOID lpBoundaryDescriptor,
 LPCSTR lpAliasPrefix
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenPrivateNamespaceW(
 LPVOID lpBoundaryDescriptor,
 LPCWSTR lpAliasPrefix
 );
__declspec(dllimport)
BOOLEAN
__stdcall
ClosePrivateNamespace(
 HANDLE Handle,
 ULONG Flags
 );
HANDLE
__stdcall
CreateBoundaryDescriptorA(
 LPCSTR Name,
 ULONG Flags
 );
HANDLE
__stdcall
CreateBoundaryDescriptorW(
 LPCWSTR Name,
 ULONG Flags
 );
__declspec(dllimport)
BOOL
__stdcall
AddSIDToBoundaryDescriptor(
 HANDLE * BoundaryDescriptor,
 PSID RequiredSid
 );
__declspec(dllimport)
void
__stdcall
DeleteBoundaryDescriptor(
 HANDLE BoundaryDescriptor
 );
typedef struct tagHW_PROFILE_INFOA {
 DWORD dwDockInfo;
 CHAR szHwProfileGuid[39];
 CHAR szHwProfileName[80];
} HW_PROFILE_INFOA, *LPHW_PROFILE_INFOA;
typedef struct tagHW_PROFILE_INFOW {
 DWORD dwDockInfo;
 WCHAR szHwProfileGuid[39];
 WCHAR szHwProfileName[80];
} HW_PROFILE_INFOW, *LPHW_PROFILE_INFOW;
typedef HW_PROFILE_INFOA HW_PROFILE_INFO;
typedef LPHW_PROFILE_INFOA LPHW_PROFILE_INFO;
__declspec(dllimport)
BOOL
__stdcall
GetCurrentHwProfileA (
 LPHW_PROFILE_INFOA lpHwProfileInfo
 );
__declspec(dllimport)
BOOL
__stdcall
GetCurrentHwProfileW (
 LPHW_PROFILE_INFOW lpHwProfileInfo
 );
__declspec(dllimport)
BOOL
__stdcall
QueryPerformanceCounter(
 LARGE_INTEGER *lpPerformanceCount
 );
__declspec(dllimport)
BOOL
__stdcall
QueryPerformanceFrequency(
 LARGE_INTEGER *lpFrequency
 );
__declspec(dllimport)
BOOL
__stdcall
GetVersionExA(
 LPOSVERSIONINFOA lpVersionInformation
 );
__declspec(dllimport)
BOOL
__stdcall
GetVersionExW(
 LPOSVERSIONINFOW lpVersionInformation
 );
__declspec(dllimport)
BOOL
__stdcall
VerifyVersionInfoA(
 LPOSVERSIONINFOEXA lpVersionInformation,
 DWORD dwTypeMask,
 DWORDLONG dwlConditionMask
 );
__declspec(dllimport)
BOOL
__stdcall
VerifyVersionInfoW(
 LPOSVERSIONINFOEXW lpVersionInformation,
 DWORD dwTypeMask,
 DWORDLONG dwlConditionMask
 );
__declspec(dllimport)
BOOL
__stdcall
GetProductInfo(
 DWORD dwOSMajorVersion,
 DWORD dwOSMinorVersion,
 DWORD dwSpMajorVersion,
 DWORD dwSpMinorVersion,
 PDWORD pdwReturnedProductType
 );
#pragma once
__forceinline HRESULT HRESULT_FROM_WIN32(unsigned long x) { return (HRESULT)(x) <= 0 ? (HRESULT)(x) : (HRESULT) (((x) & 0x0000FFFF) | (7 << 16) | 0x80000000);}
typedef struct _SYSTEM_POWER_STATUS {
 BYTE ACLineStatus;
 BYTE BatteryFlag;
 BYTE BatteryLifePercent;
 BYTE Reserved1;
 DWORD BatteryLifeTime;
 DWORD BatteryFullLifeTime;
} SYSTEM_POWER_STATUS, *LPSYSTEM_POWER_STATUS;
BOOL
__stdcall
GetSystemPowerStatus(
 LPSYSTEM_POWER_STATUS lpSystemPowerStatus
 );
BOOL
__stdcall
SetSystemPowerState(
 BOOL fSuspend,
 BOOL fForce
 );
__declspec(dllimport)
BOOL
__stdcall
AllocateUserPhysicalPages(
 HANDLE hProcess,
 PULONG_PTR NumberOfPages,
 PULONG_PTR PageArray
 );
__declspec(dllimport)
BOOL
__stdcall
AllocateUserPhysicalPagesNuma(
 HANDLE hProcess,
 PULONG_PTR NumberOfPages,
 PULONG_PTR PageArray,
 DWORD nndPreferred
 );
__declspec(dllimport)
BOOL
__stdcall
FreeUserPhysicalPages(
 HANDLE hProcess,
 PULONG_PTR NumberOfPages,
 PULONG_PTR PageArray
 );
__declspec(dllimport)
BOOL
__stdcall
MapUserPhysicalPages(
 PVOID VirtualAddress,
 ULONG_PTR NumberOfPages,
 PULONG_PTR PageArray
 );
__declspec(dllimport)
BOOL
__stdcall
MapUserPhysicalPagesScatter(
 PVOID *VirtualAddresses,
 ULONG_PTR NumberOfPages,
 PULONG_PTR PageArray
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateJobObjectA(
 LPSECURITY_ATTRIBUTES lpJobAttributes,
 LPCSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateJobObjectW(
 LPSECURITY_ATTRIBUTES lpJobAttributes,
 LPCWSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenJobObjectA(
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 LPCSTR lpName
 );
__declspec(dllimport)
HANDLE
__stdcall
OpenJobObjectW(
 DWORD dwDesiredAccess,
 BOOL bInheritHandle,
 LPCWSTR lpName
 );
__declspec(dllimport)
BOOL
__stdcall
AssignProcessToJobObject(
 HANDLE hJob,
 HANDLE hProcess
 );
__declspec(dllimport)
BOOL
__stdcall
TerminateJobObject(
 HANDLE hJob,
 UINT uExitCode
 );
__declspec(dllimport)
BOOL
__stdcall
QueryInformationJobObject(
 HANDLE hJob,
 JOBOBJECTINFOCLASS JobObjectInformationClass,
 LPVOID lpJobObjectInformation,
 DWORD cbJobObjectInformationLength,
 LPDWORD lpReturnLength
 );
__declspec(dllimport)
BOOL
__stdcall
SetInformationJobObject(
 HANDLE hJob,
 JOBOBJECTINFOCLASS JobObjectInformationClass,
 LPVOID lpJobObjectInformation,
 DWORD cbJobObjectInformationLength
 );
__declspec(dllimport)
BOOL
__stdcall
IsProcessInJob (
 HANDLE ProcessHandle,
 HANDLE JobHandle,
 PBOOL Result
 );
__declspec(dllimport)
BOOL
__stdcall
CreateJobSet (
 ULONG NumJob,
 PJOB_SET_ARRAY UserJobSet,
 ULONG Flags);
__declspec(dllimport)
PVOID
__stdcall
AddVectoredExceptionHandler (
 ULONG First,
 PVECTORED_EXCEPTION_HANDLER Handler
 );
__declspec(dllimport)
ULONG
__stdcall
RemoveVectoredExceptionHandler (
 PVOID Handle
 );
__declspec(dllimport)
PVOID
__stdcall
AddVectoredContinueHandler (
 ULONG First,
 PVECTORED_EXCEPTION_HANDLER Handler
 );
__declspec(dllimport)
ULONG
__stdcall
RemoveVectoredContinueHandler (
 PVOID Handle
 );
__declspec(dllimport)
HANDLE
__stdcall
FindFirstVolumeA(
 LPSTR lpszVolumeName,
 DWORD cchBufferLength
 );
__declspec(dllimport)
HANDLE
__stdcall
FindFirstVolumeW(
 LPWSTR lpszVolumeName,
 DWORD cchBufferLength
 );
__declspec(dllimport)
BOOL
__stdcall
FindNextVolumeA(
 HANDLE hFindVolume,
 LPSTR lpszVolumeName,
 DWORD cchBufferLength
 );
__declspec(dllimport)
BOOL
__stdcall
FindNextVolumeW(
 HANDLE hFindVolume,
 LPWSTR lpszVolumeName,
 DWORD cchBufferLength
 );
__declspec(dllimport)
BOOL
__stdcall
FindVolumeClose(
 HANDLE hFindVolume
 );
__declspec(dllimport)
HANDLE
__stdcall
FindFirstVolumeMountPointA(
 LPCSTR lpszRootPathName,
 LPSTR lpszVolumeMountPoint,
 DWORD cchBufferLength
 );
__declspec(dllimport)
HANDLE
__stdcall
FindFirstVolumeMountPointW(
 LPCWSTR lpszRootPathName,
 LPWSTR lpszVolumeMountPoint,
 DWORD cchBufferLength
 );
__declspec(dllimport)
BOOL
__stdcall
FindNextVolumeMountPointA(
 HANDLE hFindVolumeMountPoint,
 LPSTR lpszVolumeMountPoint,
 DWORD cchBufferLength
 );
__declspec(dllimport)
BOOL
__stdcall
FindNextVolumeMountPointW(
 HANDLE hFindVolumeMountPoint,
 LPWSTR lpszVolumeMountPoint,
 DWORD cchBufferLength
 );
__declspec(dllimport)
BOOL
__stdcall
FindVolumeMountPointClose(
 HANDLE hFindVolumeMountPoint
 );
__declspec(dllimport)
BOOL
__stdcall
SetVolumeMountPointA(
 LPCSTR lpszVolumeMountPoint,
 LPCSTR lpszVolumeName
 );
__declspec(dllimport)
BOOL
__stdcall
SetVolumeMountPointW(
 LPCWSTR lpszVolumeMountPoint,
 LPCWSTR lpszVolumeName
 );
__declspec(dllimport)
BOOL
__stdcall
DeleteVolumeMountPointA(
 LPCSTR lpszVolumeMountPoint
 );
__declspec(dllimport)
BOOL
__stdcall
DeleteVolumeMountPointW(
 LPCWSTR lpszVolumeMountPoint
 );
__declspec(dllimport)
BOOL
__stdcall
GetVolumeNameForVolumeMountPointA(
 LPCSTR lpszVolumeMountPoint,
 LPSTR lpszVolumeName,
 DWORD cchBufferLength
 );
__declspec(dllimport)
BOOL
__stdcall
GetVolumeNameForVolumeMountPointW(
 LPCWSTR lpszVolumeMountPoint,
 LPWSTR lpszVolumeName,
 DWORD cchBufferLength
 );
__declspec(dllimport)
BOOL
__stdcall
GetVolumePathNameA(
 LPCSTR lpszFileName,
 LPSTR lpszVolumePathName,
 DWORD cchBufferLength
 );
__declspec(dllimport)
BOOL
__stdcall
GetVolumePathNameW(
 LPCWSTR lpszFileName,
 LPWSTR lpszVolumePathName,
 DWORD cchBufferLength
 );
__declspec(dllimport)
BOOL
__stdcall
GetVolumePathNamesForVolumeNameA(
 LPCSTR lpszVolumeName,
 LPCH lpszVolumePathNames,
 DWORD cchBufferLength,
 PDWORD lpcchReturnLength
 );
__declspec(dllimport)
BOOL
__stdcall
GetVolumePathNamesForVolumeNameW(
 LPCWSTR lpszVolumeName,
 LPWCH lpszVolumePathNames,
 DWORD cchBufferLength,
 PDWORD lpcchReturnLength
 );
typedef struct tagACTCTXA {
 ULONG cbSize;
 DWORD dwFlags;
 LPCSTR lpSource;
 USHORT wProcessorArchitecture;
 LANGID wLangId;
 LPCSTR lpAssemblyDirectory;
 LPCSTR lpResourceName;
 LPCSTR lpApplicationName;
 HMODULE hModule;
} ACTCTXA, *PACTCTXA;
typedef struct tagACTCTXW {
 ULONG cbSize;
 DWORD dwFlags;
 LPCWSTR lpSource;
 USHORT wProcessorArchitecture;
 LANGID wLangId;
 LPCWSTR lpAssemblyDirectory;
 LPCWSTR lpResourceName;
 LPCWSTR lpApplicationName;
 HMODULE hModule;
} ACTCTXW, *PACTCTXW;
typedef ACTCTXA ACTCTX;
typedef PACTCTXA PACTCTX;
typedef const ACTCTXA *PCACTCTXA;
typedef const ACTCTXW *PCACTCTXW;
typedef PCACTCTXA PCACTCTX;
__declspec(dllimport)
HANDLE
__stdcall
CreateActCtxA(
 PCACTCTXA pActCtx
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateActCtxW(
 PCACTCTXW pActCtx
 );
__declspec(dllimport)
void
__stdcall
AddRefActCtx(
 HANDLE hActCtx
 );
__declspec(dllimport)
void
__stdcall
ReleaseActCtx(
 HANDLE hActCtx
 );
__declspec(dllimport)
BOOL
__stdcall
ZombifyActCtx(
 HANDLE hActCtx
 );
__declspec(dllimport)
BOOL
__stdcall
ActivateActCtx(
 HANDLE hActCtx,
 ULONG_PTR *lpCookie
 );
__declspec(dllimport)
BOOL
__stdcall
DeactivateActCtx(
 DWORD dwFlags,
 ULONG_PTR ulCookie
 );
__declspec(dllimport)
BOOL
__stdcall
GetCurrentActCtx(
 HANDLE *lphActCtx);
typedef struct tagACTCTX_SECTION_KEYED_DATA_2600 {
 ULONG cbSize;
 ULONG ulDataFormatVersion;
 PVOID lpData;
 ULONG ulLength;
 PVOID lpSectionGlobalData;
 ULONG ulSectionGlobalDataLength;
 PVOID lpSectionBase;
 ULONG ulSectionTotalLength;
 HANDLE hActCtx;
 ULONG ulAssemblyRosterIndex;
} ACTCTX_SECTION_KEYED_DATA_2600, *PACTCTX_SECTION_KEYED_DATA_2600;
typedef const ACTCTX_SECTION_KEYED_DATA_2600 * PCACTCTX_SECTION_KEYED_DATA_2600;
typedef struct tagACTCTX_SECTION_KEYED_DATA_ASSEMBLY_METADATA {
 PVOID lpInformation;
 PVOID lpSectionBase;
 ULONG ulSectionLength;
 PVOID lpSectionGlobalDataBase;
 ULONG ulSectionGlobalDataLength;
} ACTCTX_SECTION_KEYED_DATA_ASSEMBLY_METADATA, *PACTCTX_SECTION_KEYED_DATA_ASSEMBLY_METADATA;
typedef const ACTCTX_SECTION_KEYED_DATA_ASSEMBLY_METADATA *PCACTCTX_SECTION_KEYED_DATA_ASSEMBLY_METADATA;
typedef struct tagACTCTX_SECTION_KEYED_DATA {
 ULONG cbSize;
 ULONG ulDataFormatVersion;
 PVOID lpData;
 ULONG ulLength;
 PVOID lpSectionGlobalData;
 ULONG ulSectionGlobalDataLength;
 PVOID lpSectionBase;
 ULONG ulSectionTotalLength;
 HANDLE hActCtx;
 ULONG ulAssemblyRosterIndex;
 ULONG ulFlags;
 ACTCTX_SECTION_KEYED_DATA_ASSEMBLY_METADATA AssemblyMetadata;
} ACTCTX_SECTION_KEYED_DATA, *PACTCTX_SECTION_KEYED_DATA;
typedef const ACTCTX_SECTION_KEYED_DATA * PCACTCTX_SECTION_KEYED_DATA;
__declspec(dllimport)
BOOL
__stdcall
FindActCtxSectionStringA(
 DWORD dwFlags,
 const GUID *lpExtensionGuid,
 ULONG ulSectionId,
 LPCSTR lpStringToFind,
 PACTCTX_SECTION_KEYED_DATA ReturnedData
 );
__declspec(dllimport)
BOOL
__stdcall
FindActCtxSectionStringW(
 DWORD dwFlags,
 const GUID *lpExtensionGuid,
 ULONG ulSectionId,
 LPCWSTR lpStringToFind,
 PACTCTX_SECTION_KEYED_DATA ReturnedData
 );
__declspec(dllimport)
BOOL
__stdcall
FindActCtxSectionGuid(
 DWORD dwFlags,
 const GUID *lpExtensionGuid,
 ULONG ulSectionId,
 const GUID *lpGuidToFind,
 PACTCTX_SECTION_KEYED_DATA ReturnedData
 );
typedef struct _ACTIVATION_CONTEXT_BASIC_INFORMATION {
 HANDLE hActCtx;
 DWORD dwFlags;
} ACTIVATION_CONTEXT_BASIC_INFORMATION, *PACTIVATION_CONTEXT_BASIC_INFORMATION;
typedef const struct _ACTIVATION_CONTEXT_BASIC_INFORMATION *PCACTIVATION_CONTEXT_BASIC_INFORMATION;
__declspec(dllimport)
BOOL
__stdcall
QueryActCtxW(
 DWORD dwFlags,
 HANDLE hActCtx,
 PVOID pvSubInstance,
 ULONG ulInfoClass,
 PVOID pvBuffer,
 SIZE_T cbBuffer,
 SIZE_T *pcbWrittenOrRequired
 );
typedef BOOL (__stdcall * PQUERYACTCTXW_FUNC)(
 DWORD dwFlags,
 HANDLE hActCtx,
 PVOID pvSubInstance,
 ULONG ulInfoClass,
 PVOID pvBuffer,
 SIZE_T cbBuffer,
 SIZE_T *pcbWrittenOrRequired
 );
__declspec(dllimport)
BOOL
__stdcall
ProcessIdToSessionId(
 DWORD dwProcessId,
 DWORD *pSessionId
 );
__declspec(dllimport)
DWORD
__stdcall
WTSGetActiveConsoleSessionId(
 void
 );
__declspec(dllimport)
BOOL
__stdcall
IsWow64Process(
 HANDLE hProcess,
 PBOOL Wow64Process
 );
__declspec(dllimport)
BOOL
__stdcall
GetLogicalProcessorInformation(
 PSYSTEM_LOGICAL_PROCESSOR_INFORMATION Buffer,
 PDWORD ReturnedLength
 );
__declspec(dllimport)
BOOL
__stdcall
GetNumaHighestNodeNumber(
 PULONG HighestNodeNumber
 );
__declspec(dllimport)
BOOL
__stdcall
GetNumaProcessorNode(
 UCHAR Processor,
 PUCHAR NodeNumber
 );
__declspec(dllimport)
BOOL
__stdcall
GetNumaNodeProcessorMask(
 UCHAR Node,
 PULONGLONG ProcessorMask
 );
__declspec(dllimport)
BOOL
__stdcall
GetNumaAvailableMemoryNode(
 UCHAR Node,
 PULONGLONG AvailableBytes
 );
__declspec(dllimport)
BOOL
__stdcall
GetNumaProximityNode(
 ULONG ProximityId,
 PUCHAR NodeNumber
 );
typedef DWORD (__stdcall *APPLICATION_RECOVERY_CALLBACK)(PVOID pvParameter);
__declspec(dllimport)
HRESULT
__stdcall
RegisterApplicationRecoveryCallback(
 APPLICATION_RECOVERY_CALLBACK pRecoveyCallback,
 PVOID pvParameter,
 DWORD dwPingInterval,
 DWORD dwFlags
 );
__declspec(dllimport)
HRESULT
__stdcall
UnregisterApplicationRecoveryCallback();
__declspec(dllimport)
HRESULT
__stdcall
RegisterApplicationRestart(
 PCWSTR pwzCommandline,
 DWORD dwFlags
 );
__declspec(dllimport)
HRESULT
__stdcall
UnregisterApplicationRestart();
__declspec(dllimport)
HRESULT
__stdcall
GetApplicationRecoveryCallback(
 HANDLE hProcess,
 APPLICATION_RECOVERY_CALLBACK* pRecoveryCallback,
 PVOID* ppvParameter,
 PDWORD pdwPingInterval,
 PDWORD pdwFlags
 );
__declspec(dllimport)
HRESULT
__stdcall
GetApplicationRestartSettings(
 HANDLE hProcess,
 PWSTR pwzCommandline,
 PDWORD pcchSize,
 PDWORD pdwFlags
 );
__declspec(dllimport)
HRESULT
__stdcall
ApplicationRecoveryInProgress(
 PBOOL pbCancelled
 );
__declspec(dllimport)
void
__stdcall
ApplicationRecoveryFinished(
 BOOL bSuccess
 );
typedef enum _FILE_INFO_BY_HANDLE_CLASS {
 FileBasicInfo,
 FileStandardInfo,
 FileNameInfo,
 FileRenameInfo,
 FileDispositionInfo,
 FileAllocationInfo,
 FileEndOfFileInfo,
 FileStreamInfo,
 FileCompressionInfo,
 FileAttributeTagInfo,
 FileIdBothDirectoryInfo,
 FileIdBothDirectoryRestartInfo,
 FileIoPriorityHintInfo,
 MaximumFileInfoByHandleClass
} FILE_INFO_BY_HANDLE_CLASS, *PFILE_INFO_BY_HANDLE_CLASS;
typedef struct _FILE_BASIC_INFO {
 LARGE_INTEGER CreationTime;
 LARGE_INTEGER LastAccessTime;
 LARGE_INTEGER LastWriteTime;
 LARGE_INTEGER ChangeTime;
 DWORD FileAttributes;
} FILE_BASIC_INFO, *PFILE_BASIC_INFO;
typedef struct _FILE_STANDARD_INFO {
 LARGE_INTEGER AllocationSize;
 LARGE_INTEGER EndOfFile;
 DWORD NumberOfLinks;
 BOOLEAN DeletePending;
 BOOLEAN Directory;
} FILE_STANDARD_INFO, *PFILE_STANDARD_INFO;
typedef struct _FILE_NAME_INFO {
 DWORD FileNameLength;
 WCHAR FileName[1];
} FILE_NAME_INFO, *PFILE_NAME_INFO;
typedef struct _FILE_RENAME_INFO {
 BOOLEAN ReplaceIfExists;
 HANDLE RootDirectory;
 DWORD FileNameLength;
 WCHAR FileName[1];
} FILE_RENAME_INFO, *PFILE_RENAME_INFO;
typedef struct _FILE_ALLOCATION_INFO {
 LARGE_INTEGER AllocationSize;
} FILE_ALLOCATION_INFO, *PFILE_ALLOCATION_INFO;
typedef struct _FILE_END_OF_FILE_INFO {
 LARGE_INTEGER EndOfFile;
} FILE_END_OF_FILE_INFO, *PFILE_END_OF_FILE_INFO;
typedef struct _FILE_STREAM_INFO {
 DWORD NextEntryOffset;
 DWORD StreamNameLength;
 LARGE_INTEGER StreamSize;
 LARGE_INTEGER StreamAllocationSize;
 WCHAR StreamName[1];
} FILE_STREAM_INFO, *PFILE_STREAM_INFO;
typedef struct _FILE_COMPRESSION_INFO {
 LARGE_INTEGER CompressedFileSize;
 WORD CompressionFormat;
 UCHAR CompressionUnitShift;
 UCHAR ChunkShift;
 UCHAR ClusterShift;
 UCHAR Reserved[3];
} FILE_COMPRESSION_INFO, *PFILE_COMPRESSION_INFO;
typedef struct _FILE_ATTRIBUTE_TAG_INFO {
 DWORD FileAttributes;
 DWORD ReparseTag;
} FILE_ATTRIBUTE_TAG_INFO, *PFILE_ATTRIBUTE_TAG_INFO;
typedef struct _FILE_DISPOSITION_INFO {
 BOOLEAN DeleteFileA;
} FILE_DISPOSITION_INFO, *PFILE_DISPOSITION_INFO;
typedef struct _FILE_ID_BOTH_DIR_INFO {
 DWORD NextEntryOffset;
 DWORD FileIndex;
 LARGE_INTEGER CreationTime;
 LARGE_INTEGER LastAccessTime;
 LARGE_INTEGER LastWriteTime;
 LARGE_INTEGER ChangeTime;
 LARGE_INTEGER EndOfFile;
 LARGE_INTEGER AllocationSize;
 DWORD FileAttributes;
 DWORD FileNameLength;
 DWORD EaSize;
 CCHAR ShortNameLength;
 WCHAR ShortName[12];
 LARGE_INTEGER FileId;
 WCHAR FileName[1];
} FILE_ID_BOTH_DIR_INFO, *PFILE_ID_BOTH_DIR_INFO;
typedef enum _PRIORITY_HINT {
 IoPriorityHintVeryLow = 0,
 IoPriorityHintLow,
 IoPriorityHintNormal,
 MaximumIoPriorityHintType
} PRIORITY_HINT;
typedef struct _FILE_IO_PRIORITY_HINT_INFO {
 PRIORITY_HINT PriorityHint;
} FILE_IO_PRIORITY_HINT_INFO, *PFILE_IO_PRIORITY_HINT_INFO;
BOOL
__stdcall
SetFileInformationByHandle(
 HANDLE hFile,
 FILE_INFO_BY_HANDLE_CLASS FileInformationClass,
 LPVOID lpFileInformation,
 DWORD dwBufferSize
);
BOOL
__stdcall
GetFileInformationByHandleEx(
 HANDLE hFile,
 FILE_INFO_BY_HANDLE_CLASS FileInformationClass,
 LPVOID lpFileInformation,
 DWORD dwBufferSize
);
typedef enum _FILE_ID_TYPE {
 FileIdType,
 ObjectIdType,
 MaximumFileIdType
} FILE_ID_TYPE, *PFILE_ID_TYPE;
typedef struct FILE_ID_DESCRIPTOR {
 DWORD dwSize;
 FILE_ID_TYPE Type;
 union {
 LARGE_INTEGER FileId;
 GUID ObjectId;
 };
} FILE_ID_DESCRIPTOR, *LPFILE_ID_DESCRIPTOR;
HANDLE
__stdcall
OpenFileById (
 HANDLE hVolumeHint,
 LPFILE_ID_DESCRIPTOR lpFileId,
 DWORD dwDesiredAccess,
 DWORD dwShareMode,
 LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 DWORD dwFlagsAndAttributes
 );
BOOLEAN
__stdcall
CreateSymbolicLinkA (
 LPCSTR lpSymlinkFileName,
 LPCSTR lpTargetFileName,
 DWORD dwFlags
 );
BOOLEAN
__stdcall
CreateSymbolicLinkW (
 LPCWSTR lpSymlinkFileName,
 LPCWSTR lpTargetFileName,
 DWORD dwFlags
 );
BOOLEAN
__stdcall
CreateSymbolicLinkTransactedA (
 LPCSTR lpSymlinkFileName,
 LPCSTR lpTargetFileName,
 DWORD dwFlags,
 HANDLE hTransaction
 );
BOOLEAN
__stdcall
CreateSymbolicLinkTransactedW (
 LPCWSTR lpSymlinkFileName,
 LPCWSTR lpTargetFileName,
 DWORD dwFlags,
 HANDLE hTransaction
 );
DWORD
__stdcall
GetFinalPathNameByHandleA (
 HANDLE hFile,
 LPSTR lpszFilePath,
 DWORD cchFilePath,
 DWORD dwFlags
);
DWORD
__stdcall
GetFinalPathNameByHandleW (
 HANDLE hFile,
 LPWSTR lpszFilePath,
 DWORD cchFilePath,
 DWORD dwFlags
);
__declspec(dllimport)
BOOL
__stdcall
QueryActCtxSettingsW(
 DWORD dwFlags,
 HANDLE hActCtx,
 PCWSTR settingsNameSpace,
 PCWSTR settingName,
 PWSTR pvBuffer,
 SIZE_T dwBuffer,
 SIZE_T *pdwWrittenOrRequired
 );
}
#pragma once
extern "C" {
typedef struct _PSINJECTDATA {
 DWORD DataBytes;
 WORD InjectionPoint;
 WORD PageNumber;
} PSINJECTDATA, *PPSINJECTDATA;
typedef struct _PSFEATURE_OUTPUT {
 BOOL bPageIndependent;
 BOOL bSetPageDevice;
} PSFEATURE_OUTPUT, *PPSFEATURE_OUTPUT;
typedef struct _PSFEATURE_CUSTPAPER {
 LONG lOrientation;
 LONG lWidth;
 LONG lHeight;
 LONG lWidthOffset;
 LONG lHeightOffset;
} PSFEATURE_CUSTPAPER, *PPSFEATURE_CUSTPAPER;
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
#pragma warning(disable:4103)
#pragma pack(push,1)
typedef struct tagRGBTRIPLE {
 BYTE rgbtBlue;
 BYTE rgbtGreen;
 BYTE rgbtRed;
} RGBTRIPLE, *PRGBTRIPLE, *NPRGBTRIPLE, *LPRGBTRIPLE;
#pragma warning(disable:4103)
#pragma pack(pop)
typedef struct tagRGBQUAD {
 BYTE rgbBlue;
 BYTE rgbGreen;
 BYTE rgbRed;
 BYTE rgbReserved;
} RGBQUAD;
typedef RGBQUAD * LPRGBQUAD;
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
typedef struct {
 DWORD bV5Size;
 LONG bV5Width;
 LONG bV5Height;
 WORD bV5Planes;
 WORD bV5BitCount;
 DWORD bV5Compression;
 DWORD bV5SizeImage;
 LONG bV5XPelsPerMeter;
 LONG bV5YPelsPerMeter;
 DWORD bV5ClrUsed;
 DWORD bV5ClrImportant;
 DWORD bV5RedMask;
 DWORD bV5GreenMask;
 DWORD bV5BlueMask;
 DWORD bV5AlphaMask;
 DWORD bV5CSType;
 CIEXYZTRIPLE bV5Endpoints;
 DWORD bV5GammaRed;
 DWORD bV5GammaGreen;
 DWORD bV5GammaBlue;
 DWORD bV5Intent;
 DWORD bV5ProfileData;
 DWORD bV5ProfileSize;
 DWORD bV5Reserved;
} BITMAPV5HEADER, *LPBITMAPV5HEADER, *PBITMAPV5HEADER;
typedef struct tagBITMAPINFO {
 BITMAPINFOHEADER bmiHeader;
 RGBQUAD bmiColors[1];
} BITMAPINFO, *LPBITMAPINFO, *PBITMAPINFO;
typedef struct tagBITMAPCOREINFO {
 BITMAPCOREHEADER bmciHeader;
 RGBTRIPLE bmciColors[1];
} BITMAPCOREINFO, *LPBITMAPCOREINFO, *PBITMAPCOREINFO;
#pragma warning(disable:4103)
#pragma pack(push,2)
typedef struct tagBITMAPFILEHEADER {
 WORD bfType;
 DWORD bfSize;
 WORD bfReserved1;
 WORD bfReserved2;
 DWORD bfOffBits;
} BITMAPFILEHEADER, *LPBITMAPFILEHEADER, *PBITMAPFILEHEADER;
#pragma warning(disable:4103)
#pragma pack(pop)
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
 ULONG_PTR lbHatch;
 } LOGBRUSH, *PLOGBRUSH, *NPLOGBRUSH, *LPLOGBRUSH;
typedef struct tagLOGBRUSH32
 {
 UINT lbStyle;
 COLORREF lbColor;
 ULONG lbHatch;
 } LOGBRUSH32, *PLOGBRUSH32, *NPLOGBRUSH32, *LPLOGBRUSH32;
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
 ULONG_PTR elpHatch;
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
typedef struct tagENUMLOGFONTA
{
 LOGFONTA elfLogFont;
 BYTE elfFullName[64];
 BYTE elfStyle[32];
} ENUMLOGFONTA, * LPENUMLOGFONTA;
typedef struct tagENUMLOGFONTW
{
 LOGFONTW elfLogFont;
 WCHAR elfFullName[64];
 WCHAR elfStyle[32];
} ENUMLOGFONTW, * LPENUMLOGFONTW;
typedef ENUMLOGFONTA ENUMLOGFONT;
typedef LPENUMLOGFONTA LPENUMLOGFONT;
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
} PANOSE, * LPPANOSE;
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
 short dmScale;
 short dmCopies;
 short dmDefaultSource;
 short dmPrintQuality;
 };
 struct {
 POINTL dmPosition;
 DWORD dmDisplayOrientation;
 DWORD dmDisplayFixedOutput;
 };
 };
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
 union {
 DWORD dmDisplayFlags;
 DWORD dmNup;
 };
 DWORD dmDisplayFrequency;
 DWORD dmICMMethod;
 DWORD dmICMIntent;
 DWORD dmMediaType;
 DWORD dmDitherType;
 DWORD dmReserved1;
 DWORD dmReserved2;
 DWORD dmPanningWidth;
 DWORD dmPanningHeight;
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
 short dmScale;
 short dmCopies;
 short dmDefaultSource;
 short dmPrintQuality;
 };
 struct {
 POINTL dmPosition;
 DWORD dmDisplayOrientation;
 DWORD dmDisplayFixedOutput;
 };
 };
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
 union {
 DWORD dmDisplayFlags;
 DWORD dmNup;
 };
 DWORD dmDisplayFrequency;
 DWORD dmICMMethod;
 DWORD dmICMIntent;
 DWORD dmMediaType;
 DWORD dmDitherType;
 DWORD dmReserved1;
 DWORD dmReserved2;
 DWORD dmPanningWidth;
 DWORD dmPanningHeight;
} DEVMODEW, *PDEVMODEW, *NPDEVMODEW, *LPDEVMODEW;
typedef DEVMODEA DEVMODE;
typedef PDEVMODEA PDEVMODE;
typedef NPDEVMODEA NPDEVMODE;
typedef LPDEVMODEA LPDEVMODE;
typedef struct _DISPLAY_DEVICEA {
 DWORD cb;
 CHAR DeviceName[32];
 CHAR DeviceString[128];
 DWORD StateFlags;
 CHAR DeviceID[128];
 CHAR DeviceKey[128];
} DISPLAY_DEVICEA, *PDISPLAY_DEVICEA, *LPDISPLAY_DEVICEA;
typedef struct _DISPLAY_DEVICEW {
 DWORD cb;
 WCHAR DeviceName[32];
 WCHAR DeviceString[128];
 DWORD StateFlags;
 WCHAR DeviceID[128];
 WCHAR DeviceKey[128];
} DISPLAY_DEVICEW, *PDISPLAY_DEVICEW, *LPDISPLAY_DEVICEW;
typedef DISPLAY_DEVICEA DISPLAY_DEVICE;
typedef PDISPLAY_DEVICEA PDISPLAY_DEVICE;
typedef LPDISPLAY_DEVICEA LPDISPLAY_DEVICE;
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
typedef struct _FIXED {
 WORD fract;
 short value;
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
typedef struct tagPOINTFX
{
 FIXED x;
 FIXED y;
} POINTFX, * LPPOINTFX;
typedef struct tagTTPOLYCURVE
{
 WORD wType;
 WORD cpfx;
 POINTFX apfx[1];
} TTPOLYCURVE, * LPTTPOLYCURVE;
typedef struct tagTTPOLYGONHEADER
{
 DWORD cb;
 DWORD dwType;
 POINTFX pfxStart;
} TTPOLYGONHEADER, * LPTTPOLYGONHEADER;
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
 } GCP_RESULTSA, * LPGCP_RESULTSA;
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
 } GCP_RESULTSW, * LPGCP_RESULTSW;
typedef GCP_RESULTSA GCP_RESULTS;
typedef LPGCP_RESULTSA LPGCP_RESULTS;
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
typedef int (__stdcall* OLDFONTENUMPROCA)(const LOGFONTA *, const void *, DWORD, LPARAM);
typedef int (__stdcall* OLDFONTENUMPROCW)(const LOGFONTW *, const void *, DWORD, LPARAM);
typedef OLDFONTENUMPROCA FONTENUMPROCA;
typedef OLDFONTENUMPROCW FONTENUMPROCW;
typedef FONTENUMPROCA FONTENUMPROC;
typedef int (__stdcall* GOBJENUMPROC)(LPVOID, LPARAM);
typedef void (__stdcall* LINEDDAPROC)(int, int, LPARAM);
__declspec(dllimport) int __stdcall AddFontResourceA( LPCSTR);
__declspec(dllimport) int __stdcall AddFontResourceW( LPCWSTR);
 __declspec(dllimport) BOOL __stdcall AnimatePalette( HPALETTE hPal, UINT iStartIndex, UINT cEntries, const PALETTEENTRY * ppe);
 __declspec(dllimport) BOOL __stdcall Arc( HDC hdc, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4);
 __declspec(dllimport) BOOL __stdcall BitBlt( HDC hdc, int x, int y, int cx, int cy, HDC hdcSrc, int x1, int y1, DWORD rop);
__declspec(dllimport) BOOL __stdcall CancelDC( HDC hdc);
 __declspec(dllimport) BOOL __stdcall Chord( HDC hdc, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4);
__declspec(dllimport) int __stdcall ChoosePixelFormat( HDC hdc, const PIXELFORMATDESCRIPTOR *ppfd);
__declspec(dllimport) HMETAFILE __stdcall CloseMetaFile( HDC hdc);
__declspec(dllimport) int __stdcall CombineRgn( HRGN hrgnDst, HRGN hrgnSrc1, HRGN hrgnSrc2, int iMode);
__declspec(dllimport) HMETAFILE __stdcall CopyMetaFileA( HMETAFILE, LPCSTR);
__declspec(dllimport) HMETAFILE __stdcall CopyMetaFileW( HMETAFILE, LPCWSTR);
 __declspec(dllimport) HBITMAP __stdcall CreateBitmap( int nWidth, int nHeight, UINT nPlanes, UINT nBitCount, const void *lpBits);
 __declspec(dllimport) HBITMAP __stdcall CreateBitmapIndirect( const BITMAP *pbm);
 __declspec(dllimport) HBRUSH __stdcall CreateBrushIndirect( const LOGBRUSH *plbrush);
__declspec(dllimport) HBITMAP __stdcall CreateCompatibleBitmap( HDC hdc, int cx, int cy);
__declspec(dllimport) HBITMAP __stdcall CreateDiscardableBitmap( HDC hdc, int cx, int cy);
__declspec(dllimport) HDC __stdcall CreateCompatibleDC( HDC hdc);
__declspec(dllimport) HDC __stdcall CreateDCA( LPCSTR pwszDriver, LPCSTR pwszDevice, LPCSTR pszPort, const DEVMODEA * pdm);
__declspec(dllimport) HDC __stdcall CreateDCW( LPCWSTR pwszDriver, LPCWSTR pwszDevice, LPCWSTR pszPort, const DEVMODEW * pdm);
__declspec(dllimport) HBITMAP __stdcall CreateDIBitmap( HDC hdc, const BITMAPINFOHEADER *pbmih, DWORD flInit, const void *pjBits, const BITMAPINFO *pbmi, UINT iUsage);
__declspec(dllimport) HBRUSH __stdcall CreateDIBPatternBrush( HGLOBAL h, UINT iUsage);
 __declspec(dllimport) HBRUSH __stdcall CreateDIBPatternBrushPt( const void *lpPackedDIB, UINT iUsage);
__declspec(dllimport) HRGN __stdcall CreateEllipticRgn( int x1, int y1, int x2, int y2);
__declspec(dllimport) HRGN __stdcall CreateEllipticRgnIndirect( const RECT *lprect);
 __declspec(dllimport) HFONT __stdcall CreateFontIndirectA( const LOGFONTA *lplf);
 __declspec(dllimport) HFONT __stdcall CreateFontIndirectW( const LOGFONTW *lplf);
__declspec(dllimport) HFONT __stdcall CreateFontA( int cHeight, int cWidth, int cEscapement, int cOrientation, int cWeight, DWORD bItalic,
 DWORD bUnderline, DWORD bStrikeOut, DWORD iCharSet, DWORD iOutPrecision, DWORD iClipPrecision,
 DWORD iQuality, DWORD iPitchAndFamily, LPCSTR pszFaceName);
__declspec(dllimport) HFONT __stdcall CreateFontW( int cHeight, int cWidth, int cEscapement, int cOrientation, int cWeight, DWORD bItalic,
 DWORD bUnderline, DWORD bStrikeOut, DWORD iCharSet, DWORD iOutPrecision, DWORD iClipPrecision,
 DWORD iQuality, DWORD iPitchAndFamily, LPCWSTR pszFaceName);
__declspec(dllimport) HBRUSH __stdcall CreateHatchBrush( int iHatch, COLORREF color);
__declspec(dllimport) HDC __stdcall CreateICA( LPCSTR pszDriver, LPCSTR pszDevice, LPCSTR pszPort, const DEVMODEA * pdm);
__declspec(dllimport) HDC __stdcall CreateICW( LPCWSTR pszDriver, LPCWSTR pszDevice, LPCWSTR pszPort, const DEVMODEW * pdm);
__declspec(dllimport) HDC __stdcall CreateMetaFileA( LPCSTR pszFile);
__declspec(dllimport) HDC __stdcall CreateMetaFileW( LPCWSTR pszFile);
 __declspec(dllimport) HPALETTE __stdcall CreatePalette( const LOGPALETTE * plpal);
__declspec(dllimport) HPEN __stdcall CreatePen( int iStyle, int cWidth, COLORREF color);
 __declspec(dllimport) HPEN __stdcall CreatePenIndirect( const LOGPEN *plpen);
__declspec(dllimport) HRGN __stdcall CreatePolyPolygonRgn( const POINT *pptl,
 const INT *pc,
 int cPoly,
 int iMode);
 __declspec(dllimport) HBRUSH __stdcall CreatePatternBrush( HBITMAP hbm);
__declspec(dllimport) HRGN __stdcall CreateRectRgn( int x1, int y1, int x2, int y2);
__declspec(dllimport) HRGN __stdcall CreateRectRgnIndirect( const RECT *lprect);
__declspec(dllimport) HRGN __stdcall CreateRoundRectRgn( int x1, int y1, int x2, int y2, int w, int h);
__declspec(dllimport) BOOL __stdcall CreateScalableFontResourceA( DWORD fdwHidden, LPCSTR lpszFont, LPCSTR lpszFile, LPCSTR lpszPath);
__declspec(dllimport) BOOL __stdcall CreateScalableFontResourceW( DWORD fdwHidden, LPCWSTR lpszFont, LPCWSTR lpszFile, LPCWSTR lpszPath);
__declspec(dllimport) HBRUSH __stdcall CreateSolidBrush( COLORREF color);
__declspec(dllimport) BOOL __stdcall DeleteDC( HDC hdc);
__declspec(dllimport) BOOL __stdcall DeleteMetaFile( HMETAFILE hmf);
 __declspec(dllimport) BOOL __stdcall DeleteObject( HGDIOBJ ho);
__declspec(dllimport) int __stdcall DescribePixelFormat( HDC hdc,
 int iPixelFormat,
 UINT nBytes,
 LPPIXELFORMATDESCRIPTOR ppfd);
typedef UINT (__stdcall* LPFNDEVMODE)(HWND, HMODULE, LPDEVMODE, LPSTR, LPSTR, LPDEVMODE, LPSTR, UINT);
typedef DWORD (__stdcall* LPFNDEVCAPS)(LPSTR, LPSTR, UINT, LPSTR, LPDEVMODE);
__declspec(dllimport)
int
__stdcall
DeviceCapabilitiesA(
 LPCSTR pDevice,
 LPCSTR pPort,
 WORD fwCapability,
 LPSTR pOutput,
 const DEVMODEA *pDevMode
 );
__declspec(dllimport)
int
__stdcall
DeviceCapabilitiesW(
 LPCWSTR pDevice,
 LPCWSTR pPort,
 WORD fwCapability,
 LPWSTR pOutput,
 const DEVMODEW *pDevMode
 );
__declspec(dllimport) int __stdcall DrawEscape( HDC hdc,
 int iEscape,
 int cjIn,
 LPCSTR lpIn);
 __declspec(dllimport) BOOL __stdcall Ellipse( HDC hdc, int left, int top, int right, int bottom);
__declspec(dllimport) int __stdcall EnumFontFamiliesExA( HDC hdc, LPLOGFONTA lpLogfont, FONTENUMPROCA lpProc, LPARAM lParam, DWORD dwFlags);
__declspec(dllimport) int __stdcall EnumFontFamiliesExW( HDC hdc, LPLOGFONTW lpLogfont, FONTENUMPROCW lpProc, LPARAM lParam, DWORD dwFlags);
__declspec(dllimport) int __stdcall EnumFontFamiliesA( HDC hdc, LPCSTR lpLogfont, FONTENUMPROCA lpProc, LPARAM lParam);
__declspec(dllimport) int __stdcall EnumFontFamiliesW( HDC hdc, LPCWSTR lpLogfont, FONTENUMPROCW lpProc, LPARAM lParam);
__declspec(dllimport) int __stdcall EnumFontsA( HDC hdc, LPCSTR lpLogfont, FONTENUMPROCA lpProc, LPARAM lParam);
__declspec(dllimport) int __stdcall EnumFontsW( HDC hdc, LPCWSTR lpLogfont, FONTENUMPROCW lpProc, LPARAM lParam);
__declspec(dllimport) int __stdcall EnumObjects( HDC hdc, int nType, GOBJENUMPROC lpFunc, LPARAM lParam);
__declspec(dllimport) BOOL __stdcall EqualRgn( HRGN hrgn1, HRGN hrgn2);
 __declspec(dllimport) int __stdcall Escape( HDC hdc,
 int iEscape,
 int cjIn,
 LPCSTR pvIn,
 LPVOID pvOut);
__declspec(dllimport) int __stdcall ExtEscape( HDC hdc,
 int iEscape,
 int cjInput,
 LPCSTR lpInData,
 int cjOutput,
 LPSTR lpOutData);
 __declspec(dllimport) int __stdcall ExcludeClipRect( HDC hdc, int left, int top, int right, int bottom);
 __declspec(dllimport) HRGN __stdcall ExtCreateRegion( const XFORM * lpx, DWORD nCount, const RGNDATA * lpData);
 __declspec(dllimport) BOOL __stdcall ExtFloodFill( HDC hdc, int x, int y, COLORREF color, UINT type);
 __declspec(dllimport) BOOL __stdcall FillRgn( HDC hdc, HRGN hrgn, HBRUSH hbr);
 __declspec(dllimport) BOOL __stdcall FloodFill( HDC hdc, int x, int y, COLORREF color);
 __declspec(dllimport) BOOL __stdcall FrameRgn( HDC hdc, HRGN hrgn, HBRUSH hbr, int w, int h);
__declspec(dllimport) int __stdcall GetROP2( HDC hdc);
__declspec(dllimport) BOOL __stdcall GetAspectRatioFilterEx( HDC hdc, LPSIZE lpsize);
__declspec(dllimport) COLORREF __stdcall GetBkColor( HDC hdc);
__declspec(dllimport) COLORREF __stdcall GetDCBrushColor( HDC hdc);
__declspec(dllimport) COLORREF __stdcall GetDCPenColor( HDC hdc);
__declspec(dllimport)
int
__stdcall
GetBkMode(
 HDC hdc
 );
__declspec(dllimport)
LONG
__stdcall
GetBitmapBits(
 HBITMAP hbit,
 LONG cb,
 LPVOID lpvBits
 );
__declspec(dllimport) BOOL __stdcall GetBitmapDimensionEx( HBITMAP hbit, LPSIZE lpsize);
__declspec(dllimport) UINT __stdcall GetBoundsRect( HDC hdc, LPRECT lprect, UINT flags);
__declspec(dllimport) BOOL __stdcall GetBrushOrgEx( HDC hdc, LPPOINT lppt);
__declspec(dllimport) BOOL __stdcall GetCharWidthA( HDC hdc, UINT iFirst, UINT iLast, LPINT lpBuffer);
__declspec(dllimport) BOOL __stdcall GetCharWidthW( HDC hdc, UINT iFirst, UINT iLast, LPINT lpBuffer);
__declspec(dllimport) BOOL __stdcall GetCharWidth32A( HDC hdc, UINT iFirst, UINT iLast, LPINT lpBuffer);
__declspec(dllimport) BOOL __stdcall GetCharWidth32W( HDC hdc, UINT iFirst, UINT iLast, LPINT lpBuffer);
__declspec(dllimport) BOOL __stdcall GetCharWidthFloatA( HDC hdc, UINT iFirst, UINT iLast, PFLOAT lpBuffer);
__declspec(dllimport) BOOL __stdcall GetCharWidthFloatW( HDC hdc, UINT iFirst, UINT iLast, PFLOAT lpBuffer);
__declspec(dllimport) BOOL __stdcall GetCharABCWidthsA( HDC hdc,
 UINT wFirst,
 UINT wLast,
 LPABC lpABC);
__declspec(dllimport) BOOL __stdcall GetCharABCWidthsW( HDC hdc,
 UINT wFirst,
 UINT wLast,
 LPABC lpABC);
__declspec(dllimport) BOOL __stdcall GetCharABCWidthsFloatA( HDC hdc, UINT iFirst, UINT iLast, LPABCFLOAT lpABC);
__declspec(dllimport) BOOL __stdcall GetCharABCWidthsFloatW( HDC hdc, UINT iFirst, UINT iLast, LPABCFLOAT lpABC);
__declspec(dllimport) int __stdcall GetClipBox( HDC hdc, LPRECT lprect);
__declspec(dllimport) int __stdcall GetClipRgn( HDC hdc, HRGN hrgn);
__declspec(dllimport) int __stdcall GetMetaRgn( HDC hdc, HRGN hrgn);
__declspec(dllimport) HGDIOBJ __stdcall GetCurrentObject( HDC hdc, UINT type);
__declspec(dllimport) BOOL __stdcall GetCurrentPositionEx( HDC hdc, LPPOINT lppt);
__declspec(dllimport) int __stdcall GetDeviceCaps( HDC hdc, int index);
__declspec(dllimport) int __stdcall GetDIBits( HDC hdc, HBITMAP hbm, UINT start, UINT cLines, LPVOID lpvBits, LPBITMAPINFO lpbmi, UINT usage);
__declspec(dllimport) DWORD __stdcall GetFontData ( HDC hdc,
 DWORD dwTable,
 DWORD dwOffset,
 PVOID pvBuffer,
 DWORD cjBuffer
 );
__declspec(dllimport) DWORD __stdcall GetGlyphOutlineA( HDC hdc,
 UINT uChar,
 UINT fuFormat,
 LPGLYPHMETRICS lpgm,
 DWORD cjBuffer,
 LPVOID pvBuffer,
 const MAT2 *lpmat2
 );
__declspec(dllimport) DWORD __stdcall GetGlyphOutlineW( HDC hdc,
 UINT uChar,
 UINT fuFormat,
 LPGLYPHMETRICS lpgm,
 DWORD cjBuffer,
 LPVOID pvBuffer,
 const MAT2 *lpmat2
 );
__declspec(dllimport) int __stdcall GetGraphicsMode( HDC hdc);
__declspec(dllimport) int __stdcall GetMapMode( HDC hdc);
__declspec(dllimport) UINT __stdcall GetMetaFileBitsEx( HMETAFILE hMF, UINT cbBuffer, LPVOID lpData);
__declspec(dllimport) HMETAFILE __stdcall GetMetaFileA( LPCSTR lpName);
__declspec(dllimport) HMETAFILE __stdcall GetMetaFileW( LPCWSTR lpName);
__declspec(dllimport) COLORREF __stdcall GetNearestColor( HDC hdc, COLORREF color);
__declspec(dllimport) UINT __stdcall GetNearestPaletteIndex( HPALETTE h, COLORREF color);
__declspec(dllimport) DWORD __stdcall GetObjectType( HGDIOBJ h);
__declspec(dllimport) UINT __stdcall GetPaletteEntries( HPALETTE hpal,
 UINT iStart,
 UINT cEntries,
 LPPALETTEENTRY pPalEntries);
__declspec(dllimport) COLORREF __stdcall GetPixel( HDC hdc, int x, int y);
__declspec(dllimport) int __stdcall GetPixelFormat( HDC hdc);
__declspec(dllimport) int __stdcall GetPolyFillMode( HDC hdc);
__declspec(dllimport) BOOL __stdcall GetRasterizerCaps( LPRASTERIZER_STATUS lpraststat,
 UINT cjBytes);
__declspec(dllimport) int __stdcall GetRandomRgn ( HDC hdc, HRGN hrgn, INT i);
__declspec(dllimport) DWORD __stdcall GetRegionData( HRGN hrgn,
 DWORD nCount,
 LPRGNDATA lpRgnData);
__declspec(dllimport) int __stdcall GetRgnBox( HRGN hrgn, LPRECT lprc);
__declspec(dllimport) HGDIOBJ __stdcall GetStockObject( int i);
__declspec(dllimport) int __stdcall GetStretchBltMode( HDC hdc);
__declspec(dllimport)
UINT
__stdcall
GetSystemPaletteEntries(
 HDC hdc,
 UINT iStart,
 UINT cEntries,
 LPPALETTEENTRY pPalEntries
 );
__declspec(dllimport) UINT __stdcall GetSystemPaletteUse( HDC hdc);
__declspec(dllimport) int __stdcall GetTextCharacterExtra( HDC hdc);
__declspec(dllimport) UINT __stdcall GetTextAlign( HDC hdc);
__declspec(dllimport) COLORREF __stdcall GetTextColor( HDC hdc);
__declspec(dllimport)
BOOL
__stdcall
GetTextExtentPointA(
 HDC hdc,
 LPCSTR lpString,
 int c,
 LPSIZE lpsz
 );
__declspec(dllimport)
BOOL
__stdcall
GetTextExtentPointW(
 HDC hdc,
 LPCWSTR lpString,
 int c,
 LPSIZE lpsz
 );
__declspec(dllimport)
BOOL
__stdcall
GetTextExtentPoint32A(
 HDC hdc,
 LPCSTR lpString,
 int c,
 LPSIZE psizl
 );
__declspec(dllimport)
BOOL
__stdcall
GetTextExtentPoint32W(
 HDC hdc,
 LPCWSTR lpString,
 int c,
 LPSIZE psizl
 );
__declspec(dllimport)
BOOL
__stdcall
GetTextExtentExPointA(
 HDC hdc,
 LPCSTR lpszString,
 int cchString,
 int nMaxExtent,
 LPINT lpnFit,
 LPINT lpnDx,
 LPSIZE lpSize
 );
__declspec(dllimport)
BOOL
__stdcall
GetTextExtentExPointW(
 HDC hdc,
 LPCWSTR lpszString,
 int cchString,
 int nMaxExtent,
 LPINT lpnFit,
 LPINT lpnDx,
 LPSIZE lpSize
 );
__declspec(dllimport) int __stdcall GetTextCharset( HDC hdc);
__declspec(dllimport) int __stdcall GetTextCharsetInfo( HDC hdc, LPFONTSIGNATURE lpSig, DWORD dwFlags);
__declspec(dllimport) BOOL __stdcall TranslateCharsetInfo( DWORD *lpSrc, LPCHARSETINFO lpCs, DWORD dwFlags);
__declspec(dllimport) DWORD __stdcall GetFontLanguageInfo( HDC hdc);
__declspec(dllimport) DWORD __stdcall GetCharacterPlacementA( HDC hdc, LPCSTR lpString, int nCount, int nMexExtent, LPGCP_RESULTSA lpResults, DWORD dwFlags);
__declspec(dllimport) DWORD __stdcall GetCharacterPlacementW( HDC hdc, LPCWSTR lpString, int nCount, int nMexExtent, LPGCP_RESULTSW lpResults, DWORD dwFlags);
typedef struct tagWCRANGE
{
 WCHAR wcLow;
 USHORT cGlyphs;
} WCRANGE, *PWCRANGE, *LPWCRANGE;
typedef struct tagGLYPHSET
{
 DWORD cbThis;
 DWORD flAccel;
 DWORD cGlyphsSupported;
 DWORD cRanges;
 WCRANGE ranges[1];
} GLYPHSET, *PGLYPHSET, *LPGLYPHSET;
__declspec(dllimport) DWORD __stdcall GetFontUnicodeRanges( HDC hdc, LPGLYPHSET lpgs);
__declspec(dllimport) DWORD __stdcall GetGlyphIndicesA( HDC hdc, LPCSTR lpstr, int c, LPWORD pgi, DWORD fl);
__declspec(dllimport) DWORD __stdcall GetGlyphIndicesW( HDC hdc, LPCWSTR lpstr, int c, LPWORD pgi, DWORD fl);
__declspec(dllimport) BOOL __stdcall GetTextExtentPointI( HDC hdc, LPWORD pgiIn, int cgi, LPSIZE psize);
__declspec(dllimport) BOOL __stdcall GetTextExtentExPointI ( HDC hdc,
 LPWORD lpwszString,
 int cwchString,
 int nMaxExtent,
 LPINT lpnFit,
 LPINT lpnDx,
 LPSIZE lpSize
 );
__declspec(dllimport) BOOL __stdcall GetCharWidthI( HDC hdc,
 UINT giFirst,
 UINT cgi,
 LPWORD pgi,
 LPINT piWidths
 );
__declspec(dllimport) BOOL __stdcall GetCharABCWidthsI( HDC hdc,
 UINT giFirst,
 UINT cgi,
 LPWORD pgi,
 LPABC pabc
 );
typedef struct tagDESIGNVECTOR
{
 DWORD dvReserved;
 DWORD dvNumAxes;
 LONG dvValues[16];
} DESIGNVECTOR, *PDESIGNVECTOR, *LPDESIGNVECTOR;
__declspec(dllimport) int __stdcall AddFontResourceExA( LPCSTR name, DWORD fl, PVOID res);
__declspec(dllimport) int __stdcall AddFontResourceExW( LPCWSTR name, DWORD fl, PVOID res);
__declspec(dllimport) BOOL __stdcall RemoveFontResourceExA( LPCSTR name, DWORD fl, PVOID pdv);
__declspec(dllimport) BOOL __stdcall RemoveFontResourceExW( LPCWSTR name, DWORD fl, PVOID pdv);
__declspec(dllimport) HANDLE __stdcall AddFontMemResourceEx( PVOID pFileView,
 DWORD cjSize,
 PVOID pvResrved,
 DWORD* pNumFonts);
__declspec(dllimport) BOOL __stdcall RemoveFontMemResourceEx( HANDLE h);
typedef struct tagAXISINFOA
{
 LONG axMinValue;
 LONG axMaxValue;
 BYTE axAxisName[16];
} AXISINFOA, *PAXISINFOA, *LPAXISINFOA;
typedef struct tagAXISINFOW
{
 LONG axMinValue;
 LONG axMaxValue;
 WCHAR axAxisName[16];
} AXISINFOW, *PAXISINFOW, *LPAXISINFOW;
typedef AXISINFOA AXISINFO;
typedef PAXISINFOA PAXISINFO;
typedef LPAXISINFOA LPAXISINFO;
typedef struct tagAXESLISTA
{
 DWORD axlReserved;
 DWORD axlNumAxes;
 AXISINFOA axlAxisInfo[16];
} AXESLISTA, *PAXESLISTA, *LPAXESLISTA;
typedef struct tagAXESLISTW
{
 DWORD axlReserved;
 DWORD axlNumAxes;
 AXISINFOW axlAxisInfo[16];
} AXESLISTW, *PAXESLISTW, *LPAXESLISTW;
typedef AXESLISTA AXESLIST;
typedef PAXESLISTA PAXESLIST;
typedef LPAXESLISTA LPAXESLIST;
typedef struct tagENUMLOGFONTEXDVA
{
 ENUMLOGFONTEXA elfEnumLogfontEx;
 DESIGNVECTOR elfDesignVector;
} ENUMLOGFONTEXDVA, *PENUMLOGFONTEXDVA, *LPENUMLOGFONTEXDVA;
typedef struct tagENUMLOGFONTEXDVW
{
 ENUMLOGFONTEXW elfEnumLogfontEx;
 DESIGNVECTOR elfDesignVector;
} ENUMLOGFONTEXDVW, *PENUMLOGFONTEXDVW, *LPENUMLOGFONTEXDVW;
typedef ENUMLOGFONTEXDVA ENUMLOGFONTEXDV;
typedef PENUMLOGFONTEXDVA PENUMLOGFONTEXDV;
typedef LPENUMLOGFONTEXDVA LPENUMLOGFONTEXDV;
__declspec(dllimport) HFONT __stdcall CreateFontIndirectExA( const ENUMLOGFONTEXDVA *);
__declspec(dllimport) HFONT __stdcall CreateFontIndirectExW( const ENUMLOGFONTEXDVW *);
__declspec(dllimport) BOOL __stdcall GetViewportExtEx( HDC hdc, LPSIZE lpsize);
__declspec(dllimport) BOOL __stdcall GetViewportOrgEx( HDC hdc, LPPOINT lppoint);
__declspec(dllimport) BOOL __stdcall GetWindowExtEx( HDC hdc, LPSIZE lpsize);
__declspec(dllimport) BOOL __stdcall GetWindowOrgEx( HDC hdc, LPPOINT lppoint);
 __declspec(dllimport) int __stdcall IntersectClipRect( HDC hdc, int left, int top, int right, int bottom);
 __declspec(dllimport) BOOL __stdcall InvertRgn( HDC hdc, HRGN hrgn);
__declspec(dllimport) BOOL __stdcall LineDDA( int xStart, int yStart, int xEnd, int yEnd, LINEDDAPROC lpProc, LPARAM data);
 __declspec(dllimport) BOOL __stdcall LineTo( HDC hdc, int x, int y);
__declspec(dllimport) BOOL __stdcall MaskBlt( HDC hdcDest, int xDest, int yDest, int width, int height,
 HDC hdcSrc, int xSrc, int ySrc, HBITMAP hbmMask, int xMask, int yMask, DWORD rop);
__declspec(dllimport) BOOL __stdcall PlgBlt( HDC hdcDest, const POINT * lpPoint, HDC hdcSrc, int xSrc, int ySrc, int width,
 int height, HBITMAP hbmMask, int xMask, int yMask);
 __declspec(dllimport) int __stdcall OffsetClipRgn( HDC hdc, int x, int y);
__declspec(dllimport) int __stdcall OffsetRgn( HRGN hrgn, int x, int y);
 __declspec(dllimport) BOOL __stdcall PatBlt( HDC hdc, int x, int y, int w, int h, DWORD rop);
 __declspec(dllimport) BOOL __stdcall Pie( HDC hdc, int left, int top, int right, int bottom, int xr1, int yr1, int xr2, int yr2);
__declspec(dllimport) BOOL __stdcall PlayMetaFile( HDC hdc, HMETAFILE hmf);
 __declspec(dllimport) BOOL __stdcall PaintRgn( HDC hdc, HRGN hrgn);
 __declspec(dllimport) BOOL __stdcall PolyPolygon( HDC hdc, const POINT *apt, const INT *asz, int csz);
__declspec(dllimport) BOOL __stdcall PtInRegion( HRGN hrgn, int x, int y);
__declspec(dllimport) BOOL __stdcall PtVisible( HDC hdc, int x, int y);
__declspec(dllimport) BOOL __stdcall RectInRegion( HRGN hrgn, const RECT * lprect);
__declspec(dllimport) BOOL __stdcall RectVisible( HDC hdc, const RECT * lprect);
 __declspec(dllimport) BOOL __stdcall Rectangle( HDC hdc, int left, int top, int right, int bottom);
 __declspec(dllimport) BOOL __stdcall RestoreDC( HDC hdc, int nSavedDC);
 __declspec(dllimport) HDC __stdcall ResetDCA( HDC hdc, const DEVMODEA * lpdm);
 __declspec(dllimport) HDC __stdcall ResetDCW( HDC hdc, const DEVMODEW * lpdm);
 __declspec(dllimport) UINT __stdcall RealizePalette( HDC hdc);
__declspec(dllimport) BOOL __stdcall RemoveFontResourceA( LPCSTR lpFileName);
__declspec(dllimport) BOOL __stdcall RemoveFontResourceW( LPCWSTR lpFileName);
 __declspec(dllimport) BOOL __stdcall RoundRect( HDC hdc, int left, int top, int right, int bottom, int width, int height);
 __declspec(dllimport) BOOL __stdcall ResizePalette( HPALETTE hpal, UINT n);
 __declspec(dllimport) int __stdcall SaveDC( HDC hdc);
 __declspec(dllimport) int __stdcall SelectClipRgn( HDC hdc, HRGN hrgn);
__declspec(dllimport) int __stdcall ExtSelectClipRgn( HDC hdc, HRGN hrgn, int mode);
__declspec(dllimport) int __stdcall SetMetaRgn( HDC hdc);
 __declspec(dllimport) HGDIOBJ __stdcall SelectObject( HDC hdc, HGDIOBJ h);
 __declspec(dllimport) HPALETTE __stdcall SelectPalette( HDC hdc, HPALETTE hPal, BOOL bForceBkgd);
 __declspec(dllimport) COLORREF __stdcall SetBkColor( HDC hdc, COLORREF color);
__declspec(dllimport) COLORREF __stdcall SetDCBrushColor( HDC hdc, COLORREF color);
__declspec(dllimport) COLORREF __stdcall SetDCPenColor( HDC hdc, COLORREF color);
 __declspec(dllimport) int __stdcall SetBkMode( HDC hdc, int mode);
__declspec(dllimport)
LONG __stdcall
SetBitmapBits(
 HBITMAP hbm,
 DWORD cb,
 const void *pvBits);
__declspec(dllimport) UINT __stdcall SetBoundsRect( HDC hdc, const RECT * lprect, UINT flags);
__declspec(dllimport) int __stdcall SetDIBits( HDC hdc, HBITMAP hbm, UINT start, UINT cLines, const void *lpBits, const BITMAPINFO * lpbmi, UINT ColorUse);
 __declspec(dllimport) int __stdcall SetDIBitsToDevice( HDC hdc, int xDest, int yDest, DWORD w, DWORD h, int xSrc,
 int ySrc, UINT StartScan, UINT cLines, const void * lpvBits, const BITMAPINFO * lpbmi, UINT ColorUse);
 __declspec(dllimport) DWORD __stdcall SetMapperFlags( HDC hdc, DWORD flags);
__declspec(dllimport) int __stdcall SetGraphicsMode( HDC hdc, int iMode);
 __declspec(dllimport) int __stdcall SetMapMode( HDC hdc, int iMode);
 __declspec(dllimport) DWORD __stdcall SetLayout( HDC hdc, DWORD l);
__declspec(dllimport) DWORD __stdcall GetLayout( HDC hdc);
__declspec(dllimport) HMETAFILE __stdcall SetMetaFileBitsEx( UINT cbBuffer, const BYTE *lpData);
 __declspec(dllimport) UINT __stdcall SetPaletteEntries( HPALETTE hpal,
 UINT iStart,
 UINT cEntries,
 const PALETTEENTRY *pPalEntries);
 __declspec(dllimport) COLORREF __stdcall SetPixel( HDC hdc, int x, int y, COLORREF color);
__declspec(dllimport) BOOL __stdcall SetPixelV( HDC hdc, int x, int y, COLORREF color);
__declspec(dllimport) BOOL __stdcall SetPixelFormat( HDC hdc, int format, const PIXELFORMATDESCRIPTOR * ppfd);
 __declspec(dllimport) int __stdcall SetPolyFillMode( HDC hdc, int mode);
 __declspec(dllimport) BOOL __stdcall StretchBlt( HDC hdcDest, int xDest, int yDest, int wDest, int hDest, HDC hdcSrc, int xSrc, int ySrc, int wSrc, int hSrc, DWORD rop);
__declspec(dllimport) BOOL __stdcall SetRectRgn( HRGN hrgn, int left, int top, int right, int bottom);
 __declspec(dllimport) int __stdcall StretchDIBits( HDC hdc, int xDest, int yDest, int DestWidth, int DestHeight, int xSrc, int ySrc, int SrcWidth, int SrcHeight,
 const void * lpBits, const BITMAPINFO * lpbmi, UINT iUsage, DWORD rop);
 __declspec(dllimport) int __stdcall SetROP2( HDC hdc, int rop2);
 __declspec(dllimport) int __stdcall SetStretchBltMode( HDC hdc, int mode);
__declspec(dllimport) UINT __stdcall SetSystemPaletteUse( HDC hdc, UINT use);
 __declspec(dllimport) int __stdcall SetTextCharacterExtra( HDC hdc, int extra);
 __declspec(dllimport) COLORREF __stdcall SetTextColor( HDC hdc, COLORREF color);
 __declspec(dllimport) UINT __stdcall SetTextAlign( HDC hdc, UINT align);
 __declspec(dllimport) BOOL __stdcall SetTextJustification( HDC hdc, int extra, int count);
__declspec(dllimport) BOOL __stdcall UpdateColors( HDC hdc);
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
__declspec(dllimport) BOOL __stdcall AlphaBlend( HDC hdcDest, int xoriginDest, int yoriginDest, int wDest, int hDest, HDC hdcSrc, int xoriginSrc, int yoriginSrc, int wSrc, int hSrc, BLENDFUNCTION ftn);
__declspec(dllimport) BOOL __stdcall TransparentBlt( HDC hdcDest, int xoriginDest, int yoriginDest, int wDest, int hDest, HDC hdcSrc,
 int xoriginSrc, int yoriginSrc, int wSrc, int hSrc, UINT crTransparent);
__declspec(dllimport)
BOOL
__stdcall
GradientFill(
 HDC hdc,
 PTRIVERTEX pVertex,
 ULONG nVertex,
 PVOID pMesh,
 ULONG nMesh,
 ULONG ulMode
 );
__declspec(dllimport) BOOL __stdcall GdiAlphaBlend( HDC hdcDest, int xoriginDest, int yoriginDest, int wDest, int hDest, HDC hdcSrc, int xoriginSrc, int yoriginSrc, int wSrc, int hSrc, BLENDFUNCTION ftn);
__declspec(dllimport) BOOL __stdcall GdiTransparentBlt( HDC hdcDest, int xoriginDest, int yoriginDest, int wDest, int hDest, HDC hdcSrc,
 int xoriginSrc, int yoriginSrc, int wSrc, int hSrc, UINT crTransparent);
__declspec(dllimport) BOOL __stdcall GdiGradientFill( HDC hdc,
 PTRIVERTEX pVertex,
 ULONG nVertex,
 PVOID pMesh,
 ULONG nCount,
 ULONG ulMode);
typedef struct tagDIBSECTION {
 BITMAP dsBm;
 BITMAPINFOHEADER dsBmih;
 DWORD dsBitfields[3];
 HANDLE dshSection;
 DWORD dsOffset;
} DIBSECTION, *LPDIBSECTION, *PDIBSECTION;
__declspec(dllimport) BOOL __stdcall AngleArc( HDC hdc, int x, int y, DWORD r, FLOAT StartAngle, FLOAT SweepAngle);
__declspec(dllimport) BOOL __stdcall PolyPolyline( HDC hdc, const POINT *apt, const DWORD *asz, DWORD csz);
__declspec(dllimport) BOOL __stdcall GetWorldTransform( HDC hdc, LPXFORM lpxf);
__declspec(dllimport) BOOL __stdcall SetWorldTransform( HDC hdc, const XFORM * lpxf);
__declspec(dllimport) BOOL __stdcall ModifyWorldTransform( HDC hdc, const XFORM * lpxf, DWORD mode);
__declspec(dllimport) BOOL __stdcall CombineTransform( LPXFORM lpxfOut, const XFORM *lpxf1, const XFORM *lpxf2);
__declspec(dllimport) HBITMAP __stdcall CreateDIBSection( HDC hdc, const BITMAPINFO *lpbmi, UINT usage, void **ppvBits, HANDLE hSection, DWORD offset);
__declspec(dllimport) UINT __stdcall GetDIBColorTable( HDC hdc,
 UINT iStart,
 UINT cEntries,
 RGBQUAD *prgbq);
__declspec(dllimport) UINT __stdcall SetDIBColorTable( HDC hdc,
 UINT iStart,
 UINT cEntries,
 const RGBQUAD *prgbq);
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
__declspec(dllimport) BOOL __stdcall SetColorAdjustment( HDC hdc, const COLORADJUSTMENT *lpca);
__declspec(dllimport) BOOL __stdcall GetColorAdjustment( HDC hdc, LPCOLORADJUSTMENT lpca);
__declspec(dllimport) HPALETTE __stdcall CreateHalftonePalette( HDC hdc);
typedef BOOL (__stdcall* ABORTPROC)( HDC, int);
typedef struct _DOCINFOA {
 int cbSize;
 LPCSTR lpszDocName;
 LPCSTR lpszOutput;
 LPCSTR lpszDatatype;
 DWORD fwType;
} DOCINFOA, *LPDOCINFOA;
typedef struct _DOCINFOW {
 int cbSize;
 LPCWSTR lpszDocName;
 LPCWSTR lpszOutput;
 LPCWSTR lpszDatatype;
 DWORD fwType;
} DOCINFOW, *LPDOCINFOW;
typedef DOCINFOA DOCINFO;
typedef LPDOCINFOA LPDOCINFO;
 __declspec(dllimport) int __stdcall StartDocA( HDC hdc, const DOCINFOA *lpdi);
 __declspec(dllimport) int __stdcall StartDocW( HDC hdc, const DOCINFOW *lpdi);
 __declspec(dllimport) int __stdcall EndDoc( HDC hdc);
 __declspec(dllimport) int __stdcall StartPage( HDC hdc);
 __declspec(dllimport) int __stdcall EndPage( HDC hdc);
 __declspec(dllimport) int __stdcall AbortDoc( HDC hdc);
__declspec(dllimport) int __stdcall SetAbortProc( HDC hdc, ABORTPROC proc);
__declspec(dllimport) BOOL __stdcall AbortPath( HDC hdc);
__declspec(dllimport) BOOL __stdcall ArcTo( HDC hdc, int left, int top, int right, int bottom, int xr1, int yr1, int xr2, int yr2);
__declspec(dllimport) BOOL __stdcall BeginPath( HDC hdc);
__declspec(dllimport) BOOL __stdcall CloseFigure( HDC hdc);
__declspec(dllimport) BOOL __stdcall EndPath( HDC hdc);
__declspec(dllimport) BOOL __stdcall FillPath( HDC hdc);
__declspec(dllimport) BOOL __stdcall FlattenPath( HDC hdc);
__declspec(dllimport) int __stdcall GetPath( HDC hdc, LPPOINT apt, LPBYTE aj, int cpt);
__declspec(dllimport) HRGN __stdcall PathToRegion( HDC hdc);
__declspec(dllimport) BOOL __stdcall PolyDraw( HDC hdc, const POINT * apt, const BYTE * aj, int cpt);
__declspec(dllimport) BOOL __stdcall SelectClipPath( HDC hdc, int mode);
__declspec(dllimport) int __stdcall SetArcDirection( HDC hdc, int dir);
__declspec(dllimport) BOOL __stdcall SetMiterLimit( HDC hdc, FLOAT limit, PFLOAT old);
__declspec(dllimport) BOOL __stdcall StrokeAndFillPath( HDC hdc);
__declspec(dllimport) BOOL __stdcall StrokePath( HDC hdc);
__declspec(dllimport) BOOL __stdcall WidenPath( HDC hdc);
__declspec(dllimport) HPEN __stdcall ExtCreatePen( DWORD iPenStyle,
 DWORD cWidth,
 const LOGBRUSH *plbrush,
 DWORD cStyle,
 const DWORD *pstyle);
__declspec(dllimport) BOOL __stdcall GetMiterLimit( HDC hdc, PFLOAT plimit);
__declspec(dllimport) int __stdcall GetArcDirection( HDC hdc);
__declspec(dllimport) int __stdcall GetObjectA( HANDLE h, int c, LPVOID pv);
__declspec(dllimport) int __stdcall GetObjectW( HANDLE h, int c, LPVOID pv);
 __declspec(dllimport) BOOL __stdcall MoveToEx( HDC hdc, int x, int y, LPPOINT lppt);
 __declspec(dllimport) BOOL __stdcall TextOutA( HDC hdc, int x, int y, LPCSTR lpString, int c);
 __declspec(dllimport) BOOL __stdcall TextOutW( HDC hdc, int x, int y, LPCWSTR lpString, int c);
 __declspec(dllimport) BOOL __stdcall ExtTextOutA( HDC hdc, int x, int y, UINT options, const RECT * lprect, LPCSTR lpString, UINT c, const INT * lpDx);
 __declspec(dllimport) BOOL __stdcall ExtTextOutW( HDC hdc, int x, int y, UINT options, const RECT * lprect, LPCWSTR lpString, UINT c, const INT * lpDx);
__declspec(dllimport) BOOL __stdcall PolyTextOutA( HDC hdc, const POLYTEXTA * ppt, int nstrings);
__declspec(dllimport) BOOL __stdcall PolyTextOutW( HDC hdc, const POLYTEXTW * ppt, int nstrings);
__declspec(dllimport) HRGN __stdcall CreatePolygonRgn( const POINT *pptl,
 int cPoint,
 int iMode);
__declspec(dllimport) BOOL __stdcall DPtoLP( HDC hdc, LPPOINT lppt, int c);
__declspec(dllimport) BOOL __stdcall LPtoDP( HDC hdc, LPPOINT lppt, int c);
 __declspec(dllimport) BOOL __stdcall Polygon( HDC hdc, const POINT *apt, int cpt);
 __declspec(dllimport) BOOL __stdcall Polyline( HDC hdc, const POINT *apt, int cpt);
__declspec(dllimport) BOOL __stdcall PolyBezier( HDC hdc, const POINT * apt, DWORD cpt);
__declspec(dllimport) BOOL __stdcall PolyBezierTo( HDC hdc, const POINT * apt, DWORD cpt);
__declspec(dllimport) BOOL __stdcall PolylineTo( HDC hdc, const POINT * apt, DWORD cpt);
 __declspec(dllimport) BOOL __stdcall SetViewportExtEx( HDC hdc, int x, int y, LPSIZE lpsz);
 __declspec(dllimport) BOOL __stdcall SetViewportOrgEx( HDC hdc, int x, int y, LPPOINT lppt);
 __declspec(dllimport) BOOL __stdcall SetWindowExtEx( HDC hdc, int x, int y, LPSIZE lpsz);
 __declspec(dllimport) BOOL __stdcall SetWindowOrgEx( HDC hdc, int x, int y, LPPOINT lppt);
 __declspec(dllimport) BOOL __stdcall OffsetViewportOrgEx( HDC hdc, int x, int y, LPPOINT lppt);
 __declspec(dllimport) BOOL __stdcall OffsetWindowOrgEx( HDC hdc, int x, int y, LPPOINT lppt);
 __declspec(dllimport) BOOL __stdcall ScaleViewportExtEx( HDC hdc, int xn, int dx, int yn, int yd, LPSIZE lpsz);
 __declspec(dllimport) BOOL __stdcall ScaleWindowExtEx( HDC hdc, int xn, int xd, int yn, int yd, LPSIZE lpsz);
__declspec(dllimport) BOOL __stdcall SetBitmapDimensionEx( HBITMAP hbm, int w, int h, LPSIZE lpsz);
__declspec(dllimport) BOOL __stdcall SetBrushOrgEx( HDC hdc, int x, int y, LPPOINT lppt);
__declspec(dllimport) int __stdcall GetTextFaceA( HDC hdc, int c, LPSTR lpName);
__declspec(dllimport) int __stdcall GetTextFaceW( HDC hdc, int c, LPWSTR lpName);
typedef struct tagKERNINGPAIR {
 WORD wFirst;
 WORD wSecond;
 int iKernAmount;
} KERNINGPAIR, *LPKERNINGPAIR;
__declspec(dllimport) DWORD __stdcall GetKerningPairsA( HDC hdc,
 DWORD nPairs,
 LPKERNINGPAIR lpKernPair);
__declspec(dllimport) DWORD __stdcall GetKerningPairsW( HDC hdc,
 DWORD nPairs,
 LPKERNINGPAIR lpKernPair);
__declspec(dllimport) BOOL __stdcall GetDCOrgEx( HDC hdc, LPPOINT lppt);
__declspec(dllimport) BOOL __stdcall FixBrushOrgEx( HDC hdc, int x, int y, LPPOINT ptl);
__declspec(dllimport) BOOL __stdcall UnrealizeObject( HGDIOBJ h);
__declspec(dllimport) BOOL __stdcall GdiFlush(void);
__declspec(dllimport) DWORD __stdcall GdiSetBatchLimit( DWORD dw);
__declspec(dllimport) DWORD __stdcall GdiGetBatchLimit(void);
typedef int (__stdcall* ICMENUMPROCA)(LPSTR, LPARAM);
typedef int (__stdcall* ICMENUMPROCW)(LPWSTR, LPARAM);
__declspec(dllimport) int __stdcall SetICMMode( HDC hdc, int mode);
__declspec(dllimport) BOOL __stdcall CheckColorsInGamut( HDC hdc,
 LPRGBTRIPLE lpRGBTriple,
 LPVOID dlpBuffer,
 DWORD nCount);
__declspec(dllimport) HCOLORSPACE __stdcall GetColorSpace( HDC hdc);
__declspec(dllimport) BOOL __stdcall GetLogColorSpaceA( HCOLORSPACE hColorSpace,
 LPLOGCOLORSPACEA lpBuffer,
 DWORD nSize);
__declspec(dllimport) BOOL __stdcall GetLogColorSpaceW( HCOLORSPACE hColorSpace,
 LPLOGCOLORSPACEW lpBuffer,
 DWORD nSize);
__declspec(dllimport) HCOLORSPACE __stdcall CreateColorSpaceA( LPLOGCOLORSPACEA lplcs);
__declspec(dllimport) HCOLORSPACE __stdcall CreateColorSpaceW( LPLOGCOLORSPACEW lplcs);
__declspec(dllimport) HCOLORSPACE __stdcall SetColorSpace( HDC hdc, HCOLORSPACE hcs);
__declspec(dllimport) BOOL __stdcall DeleteColorSpace( HCOLORSPACE hcs);
__declspec(dllimport) BOOL __stdcall GetICMProfileA( HDC hdc,
 LPDWORD pBufSize,
 LPSTR pszFilename);
__declspec(dllimport) BOOL __stdcall GetICMProfileW( HDC hdc,
 LPDWORD pBufSize,
 LPWSTR pszFilename);
__declspec(dllimport) BOOL __stdcall SetICMProfileA( HDC hdc, LPSTR lpFileName);
__declspec(dllimport) BOOL __stdcall SetICMProfileW( HDC hdc, LPWSTR lpFileName);
__declspec(dllimport) BOOL __stdcall GetDeviceGammaRamp( HDC hdc, LPVOID lpRamp);
__declspec(dllimport) BOOL __stdcall SetDeviceGammaRamp( HDC hdc, LPVOID lpRamp);
__declspec(dllimport) BOOL __stdcall ColorMatchToTarget( HDC hdc, HDC hdcTarget, DWORD action);
__declspec(dllimport) int __stdcall EnumICMProfilesA( HDC hdc, ICMENUMPROCA proc, LPARAM param);
__declspec(dllimport) int __stdcall EnumICMProfilesW( HDC hdc, ICMENUMPROCW proc, LPARAM param);
__declspec(dllimport) BOOL __stdcall UpdateICMRegKeyA( DWORD reserved, LPSTR lpszCMID, LPSTR lpszFileName, UINT command);
__declspec(dllimport) BOOL __stdcall UpdateICMRegKeyW( DWORD reserved, LPWSTR lpszCMID, LPWSTR lpszFileName, UINT command);
#pragma deprecated (UpdateICMRegKeyW)
#pragma deprecated (UpdateICMRegKeyA)
__declspec(dllimport) BOOL __stdcall ColorCorrectPalette( HDC hdc, HPALETTE hPal, DWORD deFirst, DWORD num);
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
__declspec(dllimport) BOOL __stdcall wglUseFontOutlinesA(HDC, DWORD, DWORD, DWORD, FLOAT,
 FLOAT, int, LPGLYPHMETRICSFLOAT);
__declspec(dllimport) BOOL __stdcall wglUseFontOutlinesW(HDC, DWORD, DWORD, DWORD, FLOAT,
 FLOAT, int, LPGLYPHMETRICSFLOAT);
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
__declspec(dllimport) BOOL __stdcall wglDescribeLayerPlane(HDC, int, int, UINT,
 LPLAYERPLANEDESCRIPTOR);
__declspec(dllimport) int __stdcall wglSetLayerPaletteEntries(HDC, int, int, int,
 const COLORREF *);
__declspec(dllimport) int __stdcall wglGetLayerPaletteEntries(HDC, int, int, int,
 COLORREF *);
__declspec(dllimport) BOOL __stdcall wglRealizeLayerPalette(HDC, int, BOOL);
__declspec(dllimport) BOOL __stdcall wglSwapLayerBuffers(HDC, UINT);
typedef struct _WGLSWAP
{
 HDC hdc;
 UINT uiFlags;
} WGLSWAP, *PWGLSWAP, *LPWGLSWAP;
__declspec(dllimport) DWORD __stdcall wglSwapMultipleBuffers(UINT, const WGLSWAP *);
}
#pragma once
extern "C" {
typedef HANDLE HDWP;
typedef void MENUTEMPLATEA;
typedef void MENUTEMPLATEW;
typedef MENUTEMPLATEA MENUTEMPLATE;
typedef PVOID LPMENUTEMPLATEA;
typedef PVOID LPMENUTEMPLATEW;
typedef LPMENUTEMPLATEA LPMENUTEMPLATE;
typedef LRESULT (__stdcall* WNDPROC)(HWND, UINT, WPARAM, LPARAM);
typedef INT_PTR (__stdcall* DLGPROC)(HWND, UINT, WPARAM, LPARAM);
typedef void (__stdcall* TIMERPROC)(HWND, UINT, UINT_PTR, DWORD);
typedef BOOL (__stdcall* GRAYSTRINGPROC)(HDC, LPARAM, int);
typedef BOOL (__stdcall* WNDENUMPROC)(HWND, LPARAM);
typedef LRESULT (__stdcall* HOOKPROC)(int code, WPARAM wParam, LPARAM lParam);
typedef void (__stdcall* SENDASYNCPROC)(HWND, UINT, ULONG_PTR, LRESULT);
typedef BOOL (__stdcall* PROPENUMPROCA)(HWND, LPCSTR, HANDLE);
typedef BOOL (__stdcall* PROPENUMPROCW)(HWND, LPCWSTR, HANDLE);
typedef BOOL (__stdcall* PROPENUMPROCEXA)(HWND, LPSTR, HANDLE, ULONG_PTR);
typedef BOOL (__stdcall* PROPENUMPROCEXW)(HWND, LPWSTR, HANDLE, ULONG_PTR);
typedef int (__stdcall* EDITWORDBREAKPROCA)(LPSTR lpch, int ichCurrent, int cch, int code);
typedef int (__stdcall* EDITWORDBREAKPROCW)(LPWSTR lpch, int ichCurrent, int cch, int code);
typedef BOOL (__stdcall* DRAWSTATEPROC)(HDC hdc, LPARAM lData, WPARAM wData, int cx, int cy);
typedef PROPENUMPROCA PROPENUMPROC;
typedef PROPENUMPROCEXA PROPENUMPROCEX;
typedef EDITWORDBREAKPROCA EDITWORDBREAKPROC;
typedef BOOL (__stdcall* NAMEENUMPROCA)(LPSTR, LPARAM);
typedef BOOL (__stdcall* NAMEENUMPROCW)(LPWSTR, LPARAM);
typedef NAMEENUMPROCA WINSTAENUMPROCA;
typedef NAMEENUMPROCA DESKTOPENUMPROCA;
typedef NAMEENUMPROCW WINSTAENUMPROCW;
typedef NAMEENUMPROCW DESKTOPENUMPROCW;
typedef WINSTAENUMPROCA WINSTAENUMPROC;
typedef DESKTOPENUMPROCA DESKTOPENUMPROC;
#pragma warning(push)
#pragma warning(disable:4995)
__declspec(dllimport)
int
__stdcall
wvsprintfA(
 LPSTR,
 LPCSTR,
 va_list arglist);
__declspec(dllimport)
int
__stdcall
wvsprintfW(
 LPWSTR,
 LPCWSTR,
 va_list arglist);
__declspec(dllimport)
int
__cdecl
wsprintfA(
 LPSTR,
 LPCSTR,
 ...);
__declspec(dllimport)
int
__cdecl
wsprintfW(
 LPWSTR,
 LPCWSTR,
 ...);
#pragma warning(pop)
__declspec(dllimport)
HKL
__stdcall
LoadKeyboardLayoutA(
 LPCSTR pwszKLID,
 UINT Flags);
__declspec(dllimport)
HKL
__stdcall
LoadKeyboardLayoutW(
 LPCWSTR pwszKLID,
 UINT Flags);
__declspec(dllimport)
HKL
__stdcall
ActivateKeyboardLayout(
 HKL hkl,
 UINT Flags);
__declspec(dllimport)
int
__stdcall
ToUnicodeEx(
 UINT wVirtKey,
 UINT wScanCode,
 const BYTE *lpKeyState,
 LPWSTR pwszBuff,
 int cchBuff,
 UINT wFlags,
 HKL dwhkl);
__declspec(dllimport)
BOOL
__stdcall
UnloadKeyboardLayout(
 HKL hkl);
__declspec(dllimport)
BOOL
__stdcall
GetKeyboardLayoutNameA(
 LPSTR pwszKLID);
__declspec(dllimport)
BOOL
__stdcall
GetKeyboardLayoutNameW(
 LPWSTR pwszKLID);
__declspec(dllimport)
int
__stdcall
GetKeyboardLayoutList(
 int nBuff,
 HKL *lpList);
__declspec(dllimport)
HKL
__stdcall
GetKeyboardLayout(
 DWORD idThread);
typedef struct tagMOUSEMOVEPOINT {
 int x;
 int y;
 DWORD time;
 ULONG_PTR dwExtraInfo;
} MOUSEMOVEPOINT, *PMOUSEMOVEPOINT, * LPMOUSEMOVEPOINT;
__declspec(dllimport)
int
__stdcall
GetMouseMovePointsEx(
 UINT cbSize,
 LPMOUSEMOVEPOINT lppt,
 LPMOUSEMOVEPOINT lpptBuf,
 int nBufPoints,
 DWORD resolution);
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
__declspec(dllimport)
BOOL
__stdcall
IsHungAppWindow(
 HWND hwnd);
__declspec(dllimport)
void
__stdcall
DisableProcessWindowsGhosting(
 void);
typedef struct tagMSG {
 HWND hwnd;
 UINT message;
 WPARAM wParam;
 LPARAM lParam;
 DWORD time;
 POINT pt;
} MSG, *PMSG, *NPMSG, *LPMSG;
typedef struct tagMINMAXINFO {
 POINT ptReserved;
 POINT ptMaxSize;
 POINT ptMaxPosition;
 POINT ptMinTrackSize;
 POINT ptMaxTrackSize;
} MINMAXINFO, *PMINMAXINFO, *LPMINMAXINFO;
typedef struct tagCOPYDATASTRUCT {
 ULONG_PTR dwData;
 DWORD cbData;
 PVOID lpData;
} COPYDATASTRUCT, *PCOPYDATASTRUCT;
typedef struct tagMDINEXTMENU
{
 HMENU hmenuIn;
 HMENU hmenuNext;
 HWND hwndNext;
} MDINEXTMENU, * PMDINEXTMENU, * LPMDINEXTMENU;
typedef struct {
 GUID PowerSetting;
 DWORD DataLength;
 UCHAR Data[1];
} POWERBROADCAST_SETTING, *PPOWERBROADCAST_SETTING;
__declspec(dllimport)
UINT
__stdcall
RegisterWindowMessageA(
 LPCSTR lpString);
__declspec(dllimport)
UINT
__stdcall
RegisterWindowMessageW(
 LPCWSTR lpString);
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
typedef struct tagTRACKMOUSEEVENT {
 DWORD cbSize;
 DWORD dwFlags;
 HWND hwndTrack;
 DWORD dwHoverTime;
} TRACKMOUSEEVENT, *LPTRACKMOUSEEVENT;
__declspec(dllimport)
BOOL
__stdcall
TrackMouseEvent(
 LPTRACKMOUSEEVENT lpEventTrack);
__declspec(dllimport)
BOOL
__stdcall
DrawEdge(
 HDC hdc,
 LPRECT qrc,
 UINT edge,
 UINT grfFlags);
__declspec(dllimport)
BOOL
__stdcall
DrawFrameControl(
 HDC,
 LPRECT,
 UINT,
 UINT);
__declspec(dllimport)
BOOL
__stdcall
DrawCaption(
 HWND hwnd,
 HDC hdc,
 const RECT * lprect,
 UINT flags);
__declspec(dllimport)
BOOL
__stdcall
DrawAnimatedRects(
 HWND hwnd,
 int idAni,
 const RECT *lprcFrom,
 const RECT *lprcTo);
typedef struct tagACCEL {
 BYTE fVirt;
 WORD key;
 WORD cmd;
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
 UINT_PTR idFrom;
 UINT code;
} NMHDR;
typedef NMHDR * LPNMHDR;
typedef struct tagSTYLESTRUCT
{
 DWORD styleOld;
 DWORD styleNew;
} STYLESTRUCT, * LPSTYLESTRUCT;
typedef struct tagMEASUREITEMSTRUCT {
 UINT CtlType;
 UINT CtlID;
 UINT itemID;
 UINT itemWidth;
 UINT itemHeight;
 ULONG_PTR itemData;
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
 ULONG_PTR itemData;
} DRAWITEMSTRUCT, *PDRAWITEMSTRUCT, *LPDRAWITEMSTRUCT;
typedef struct tagDELETEITEMSTRUCT {
 UINT CtlType;
 UINT CtlID;
 UINT itemID;
 HWND hwndItem;
 ULONG_PTR itemData;
} DELETEITEMSTRUCT, *PDELETEITEMSTRUCT, *LPDELETEITEMSTRUCT;
typedef struct tagCOMPAREITEMSTRUCT {
 UINT CtlType;
 UINT CtlID;
 HWND hwndItem;
 UINT itemID1;
 ULONG_PTR itemData1;
 UINT itemID2;
 ULONG_PTR itemData2;
 DWORD dwLocaleId;
} COMPAREITEMSTRUCT, *PCOMPAREITEMSTRUCT, *LPCOMPAREITEMSTRUCT;
__declspec(dllimport)
BOOL
__stdcall
GetMessageA(
 LPMSG lpMsg,
 HWND hWnd,
 UINT wMsgFilterMin,
 UINT wMsgFilterMax);
__declspec(dllimport)
BOOL
__stdcall
GetMessageW(
 LPMSG lpMsg,
 HWND hWnd,
 UINT wMsgFilterMin,
 UINT wMsgFilterMax);
__declspec(dllimport)
BOOL
__stdcall
TranslateMessage(
 const MSG *lpMsg);
__declspec(dllimport)
LRESULT
__stdcall
DispatchMessageA(
 const MSG *lpMsg);
__declspec(dllimport)
LRESULT
__stdcall
DispatchMessageW(
 const MSG *lpMsg);
__declspec(dllimport)
BOOL
__stdcall
SetMessageQueue(
 int cMessagesMax);
__declspec(dllimport)
BOOL
__stdcall
PeekMessageA(
 LPMSG lpMsg,
 HWND hWnd,
 UINT wMsgFilterMin,
 UINT wMsgFilterMax,
 UINT wRemoveMsg);
__declspec(dllimport)
BOOL
__stdcall
PeekMessageW(
 LPMSG lpMsg,
 HWND hWnd,
 UINT wMsgFilterMin,
 UINT wMsgFilterMax,
 UINT wRemoveMsg);
__declspec(dllimport)
BOOL
__stdcall
RegisterHotKey(
 HWND hWnd,
 int id,
 UINT fsModifiers,
 UINT vk);
__declspec(dllimport)
BOOL
__stdcall
UnregisterHotKey(
 HWND hWnd,
 int id);
__declspec(dllimport)
BOOL
__stdcall
ExitWindowsEx(
 UINT uFlags,
 DWORD dwReason);
__declspec(dllimport)
BOOL
__stdcall
SwapMouseButton(
 BOOL fSwap);
__declspec(dllimport)
DWORD
__stdcall
GetMessagePos(
 void);
__declspec(dllimport)
LONG
__stdcall
GetMessageTime(
 void);
__declspec(dllimport)
LPARAM
__stdcall
GetMessageExtraInfo(
 void);
__declspec(dllimport)
BOOL
__stdcall
IsWow64Message(
 void);
__declspec(dllimport)
LPARAM
__stdcall
SetMessageExtraInfo(
 LPARAM lParam);
__declspec(dllimport)
LRESULT
__stdcall
SendMessageA(
 HWND hWnd,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
LRESULT
__stdcall
SendMessageW(
 HWND hWnd,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
LRESULT
__stdcall
SendMessageTimeoutA(
 HWND hWnd,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam,
 UINT fuFlags,
 UINT uTimeout,
 PDWORD_PTR lpdwResult);
__declspec(dllimport)
LRESULT
__stdcall
SendMessageTimeoutW(
 HWND hWnd,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam,
 UINT fuFlags,
 UINT uTimeout,
 PDWORD_PTR lpdwResult);
__declspec(dllimport)
BOOL
__stdcall
SendNotifyMessageA(
 HWND hWnd,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
BOOL
__stdcall
SendNotifyMessageW(
 HWND hWnd,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
BOOL
__stdcall
SendMessageCallbackA(
 HWND hWnd,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam,
 SENDASYNCPROC lpResultCallBack,
 ULONG_PTR dwData);
__declspec(dllimport)
BOOL
__stdcall
SendMessageCallbackW(
 HWND hWnd,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam,
 SENDASYNCPROC lpResultCallBack,
 ULONG_PTR dwData);
typedef struct {
 UINT cbSize;
 HDESK hdesk;
 HWND hwnd;
 LUID luid;
} BSMINFO, *PBSMINFO;
__declspec(dllimport)
long
__stdcall
BroadcastSystemMessageExA(
 DWORD flags,
 LPDWORD lpInfo,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam,
 PBSMINFO pbsmInfo);
__declspec(dllimport)
long
__stdcall
BroadcastSystemMessageExW(
 DWORD flags,
 LPDWORD lpInfo,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam,
 PBSMINFO pbsmInfo);
__declspec(dllimport)
long
__stdcall
BroadcastSystemMessageA(
 DWORD flags,
 LPDWORD lpInfo,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
long
__stdcall
BroadcastSystemMessageW(
 DWORD flags,
 LPDWORD lpInfo,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
typedef PVOID HDEVNOTIFY;
typedef HDEVNOTIFY *PHDEVNOTIFY;
__declspec(dllimport)
HDEVNOTIFY
__stdcall
RegisterDeviceNotificationA(
 HANDLE hRecipient,
 LPVOID NotificationFilter,
 DWORD Flags);
__declspec(dllimport)
HDEVNOTIFY
__stdcall
RegisterDeviceNotificationW(
 HANDLE hRecipient,
 LPVOID NotificationFilter,
 DWORD Flags);
__declspec(dllimport)
BOOL
__stdcall
UnregisterDeviceNotification(
 HDEVNOTIFY Handle
 );
typedef PVOID HPOWERNOTIFY;
typedef HPOWERNOTIFY *PHPOWERNOTIFY;
__declspec(dllimport)
HPOWERNOTIFY
__stdcall
RegisterPowerSettingNotification(
 HANDLE hRecipient,
 LPCGUID PowerSettingGuid,
 DWORD Flags
 );
__declspec(dllimport)
BOOL
__stdcall
UnregisterPowerSettingNotification(
 HPOWERNOTIFY Handle
 );
__declspec(dllimport)
BOOL
__stdcall
PostMessageA(
 HWND hWnd,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
BOOL
__stdcall
PostMessageW(
 HWND hWnd,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
BOOL
__stdcall
PostThreadMessageA(
 DWORD idThread,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
BOOL
__stdcall
PostThreadMessageW(
 DWORD idThread,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
BOOL
__stdcall
AttachThreadInput(
 DWORD idAttach,
 DWORD idAttachTo,
 BOOL fAttach);
__declspec(dllimport)
BOOL
__stdcall
ReplyMessage(
 LRESULT lResult);
__declspec(dllimport)
BOOL
__stdcall
WaitMessage(
 void);
__declspec(dllimport)
DWORD
__stdcall
WaitForInputIdle(
 HANDLE hProcess,
 DWORD dwMilliseconds);
__declspec(dllimport)
LRESULT
__stdcall
DefWindowProcA(
 HWND hWnd,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
LRESULT
__stdcall
DefWindowProcW(
 HWND hWnd,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
void
__stdcall
PostQuitMessage(
 int nExitCode);
__declspec(dllimport)
LRESULT
__stdcall
CallWindowProcA(
 WNDPROC lpPrevWndFunc,
 HWND hWnd,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
LRESULT
__stdcall
CallWindowProcW(
 WNDPROC lpPrevWndFunc,
 HWND hWnd,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
BOOL
__stdcall
InSendMessage(
 void);
__declspec(dllimport)
DWORD
__stdcall
InSendMessageEx(
 LPVOID lpReserved);
__declspec(dllimport)
UINT
__stdcall
GetDoubleClickTime(
 void);
__declspec(dllimport)
BOOL
__stdcall
SetDoubleClickTime(
 UINT);
__declspec(dllimport)
ATOM
__stdcall
RegisterClassA(
 const WNDCLASSA *lpWndClass);
__declspec(dllimport)
ATOM
__stdcall
RegisterClassW(
 const WNDCLASSW *lpWndClass);
__declspec(dllimport)
BOOL
__stdcall
UnregisterClassA(
 LPCSTR lpClassName,
 HINSTANCE hInstance);
__declspec(dllimport)
BOOL
__stdcall
UnregisterClassW(
 LPCWSTR lpClassName,
 HINSTANCE hInstance);
__declspec(dllimport)
BOOL
__stdcall
GetClassInfoA(
 HINSTANCE hInstance,
 LPCSTR lpClassName,
 LPWNDCLASSA lpWndClass);
__declspec(dllimport)
BOOL
__stdcall
GetClassInfoW(
 HINSTANCE hInstance,
 LPCWSTR lpClassName,
 LPWNDCLASSW lpWndClass);
__declspec(dllimport)
ATOM
__stdcall
RegisterClassExA(
 const WNDCLASSEXA *);
__declspec(dllimport)
ATOM
__stdcall
RegisterClassExW(
 const WNDCLASSEXW *);
__declspec(dllimport)
BOOL
__stdcall
GetClassInfoExA(
 HINSTANCE hInstance,
 LPCSTR lpszClass,
 LPWNDCLASSEXA lpwcx);
__declspec(dllimport)
BOOL
__stdcall
GetClassInfoExW(
 HINSTANCE hInstance,
 LPCWSTR lpszClass,
 LPWNDCLASSEXW lpwcx);
typedef BOOLEAN (__stdcall * PREGISTERCLASSNAMEW)(LPCWSTR);
__declspec(dllimport)
HWND
__stdcall
CreateWindowExA(
 DWORD dwExStyle,
 LPCSTR lpClassName,
 LPCSTR lpWindowName,
 DWORD dwStyle,
 int X,
 int Y,
 int nWidth,
 int nHeight,
 HWND hWndParent,
 HMENU hMenu,
 HINSTANCE hInstance,
 LPVOID lpParam);
__declspec(dllimport)
HWND
__stdcall
CreateWindowExW(
 DWORD dwExStyle,
 LPCWSTR lpClassName,
 LPCWSTR lpWindowName,
 DWORD dwStyle,
 int X,
 int Y,
 int nWidth,
 int nHeight,
 HWND hWndParent,
 HMENU hMenu,
 HINSTANCE hInstance,
 LPVOID lpParam);
__declspec(dllimport)
BOOL
__stdcall
IsWindow(
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
IsMenu(
 HMENU hMenu);
__declspec(dllimport)
BOOL
__stdcall
IsChild(
 HWND hWndParent,
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
DestroyWindow(
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
ShowWindow(
 HWND hWnd,
 int nCmdShow);
__declspec(dllimport)
BOOL
__stdcall
AnimateWindow(
 HWND hWnd,
 DWORD dwTime,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
UpdateLayeredWindow(
 HWND hWnd,
 HDC hdcDst,
 POINT *pptDst,
 SIZE *psize,
 HDC hdcSrc,
 POINT *pptSrc,
 COLORREF crKey,
 BLENDFUNCTION *pblend,
 DWORD dwFlags);
typedef struct tagUPDATELAYEREDWINDOWINFO
{
 DWORD cbSize;
 HDC hdcDst;
 POINT const *pptDst;
 SIZE const *psize;
 HDC hdcSrc;
 POINT const *pptSrc;
 COLORREF crKey;
 BLENDFUNCTION const *pblend;
 DWORD dwFlags;
 RECT const *prcDirty;
} UPDATELAYEREDWINDOWINFO, *PUPDATELAYEREDWINDOWINFO;
__declspec(dllimport)
BOOL
__stdcall
UpdateLayeredWindowIndirect(
 HWND hWnd,
 UPDATELAYEREDWINDOWINFO const *pULWInfo);
__declspec(dllimport)
BOOL
__stdcall
GetLayeredWindowAttributes(
 HWND hwnd,
 COLORREF *pcrKey,
 BYTE *pbAlpha,
 DWORD *pdwFlags);
__declspec(dllimport)
BOOL
__stdcall
PrintWindow(
 HWND hwnd,
 HDC hdcBlt,
 UINT nFlags);
__declspec(dllimport)
BOOL
__stdcall
SetLayeredWindowAttributes(
 HWND hwnd,
 COLORREF crKey,
 BYTE bAlpha,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
ShowWindowAsync(
 HWND hWnd,
 int nCmdShow);
__declspec(dllimport)
BOOL
__stdcall
FlashWindow(
 HWND hWnd,
 BOOL bInvert);
typedef struct {
 UINT cbSize;
 HWND hwnd;
 DWORD dwFlags;
 UINT uCount;
 DWORD dwTimeout;
} FLASHWINFO, *PFLASHWINFO;
__declspec(dllimport)
BOOL
__stdcall
FlashWindowEx(
 PFLASHWINFO pfwi);
__declspec(dllimport)
BOOL
__stdcall
ShowOwnedPopups(
 HWND hWnd,
 BOOL fShow);
__declspec(dllimport)
BOOL
__stdcall
OpenIcon(
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
CloseWindow(
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
MoveWindow(
 HWND hWnd,
 int X,
 int Y,
 int nWidth,
 int nHeight,
 BOOL bRepaint);
__declspec(dllimport)
BOOL
__stdcall
SetWindowPos(
 HWND hWnd,
 HWND hWndInsertAfter,
 int X,
 int Y,
 int cx,
 int cy,
 UINT uFlags);
__declspec(dllimport)
BOOL
__stdcall
GetWindowPlacement(
 HWND hWnd,
 WINDOWPLACEMENT *lpwndpl);
__declspec(dllimport)
BOOL
__stdcall
SetWindowPlacement(
 HWND hWnd,
 const WINDOWPLACEMENT *lpwndpl);
__declspec(dllimport)
BOOL
__stdcall
IsWindowVisible(
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
IsIconic(
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
AnyPopup(
 void);
__declspec(dllimport)
BOOL
__stdcall
BringWindowToTop(
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
IsZoomed(
 HWND hWnd);
#pragma warning(disable:4103)
#pragma pack(push,2)
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
typedef const DLGTEMPLATE *LPCDLGTEMPLATEA;
typedef const DLGTEMPLATE *LPCDLGTEMPLATEW;
typedef LPCDLGTEMPLATEA LPCDLGTEMPLATE;
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
typedef DLGITEMTEMPLATE *LPDLGITEMTEMPLATEA;
typedef DLGITEMTEMPLATE *LPDLGITEMTEMPLATEW;
typedef LPDLGITEMTEMPLATEA LPDLGITEMTEMPLATE;
#pragma warning(disable:4103)
#pragma pack(pop)
__declspec(dllimport)
HWND
__stdcall
CreateDialogParamA(
 HINSTANCE hInstance,
 LPCSTR lpTemplateName,
 HWND hWndParent,
 DLGPROC lpDialogFunc,
 LPARAM dwInitParam);
__declspec(dllimport)
HWND
__stdcall
CreateDialogParamW(
 HINSTANCE hInstance,
 LPCWSTR lpTemplateName,
 HWND hWndParent,
 DLGPROC lpDialogFunc,
 LPARAM dwInitParam);
__declspec(dllimport)
HWND
__stdcall
CreateDialogIndirectParamA(
 HINSTANCE hInstance,
 LPCDLGTEMPLATEA lpTemplate,
 HWND hWndParent,
 DLGPROC lpDialogFunc,
 LPARAM dwInitParam);
__declspec(dllimport)
HWND
__stdcall
CreateDialogIndirectParamW(
 HINSTANCE hInstance,
 LPCDLGTEMPLATEW lpTemplate,
 HWND hWndParent,
 DLGPROC lpDialogFunc,
 LPARAM dwInitParam);
__declspec(dllimport)
INT_PTR
__stdcall
DialogBoxParamA(
 HINSTANCE hInstance,
 LPCSTR lpTemplateName,
 HWND hWndParent,
 DLGPROC lpDialogFunc,
 LPARAM dwInitParam);
__declspec(dllimport)
INT_PTR
__stdcall
DialogBoxParamW(
 HINSTANCE hInstance,
 LPCWSTR lpTemplateName,
 HWND hWndParent,
 DLGPROC lpDialogFunc,
 LPARAM dwInitParam);
__declspec(dllimport)
INT_PTR
__stdcall
DialogBoxIndirectParamA(
 HINSTANCE hInstance,
 LPCDLGTEMPLATEA hDialogTemplate,
 HWND hWndParent,
 DLGPROC lpDialogFunc,
 LPARAM dwInitParam);
__declspec(dllimport)
INT_PTR
__stdcall
DialogBoxIndirectParamW(
 HINSTANCE hInstance,
 LPCDLGTEMPLATEW hDialogTemplate,
 HWND hWndParent,
 DLGPROC lpDialogFunc,
 LPARAM dwInitParam);
__declspec(dllimport)
BOOL
__stdcall
EndDialog(
 HWND hDlg,
 INT_PTR nResult);
__declspec(dllimport)
HWND
__stdcall
GetDlgItem(
 HWND hDlg,
 int nIDDlgItem);
__declspec(dllimport)
BOOL
__stdcall
SetDlgItemInt(
 HWND hDlg,
 int nIDDlgItem,
 UINT uValue,
 BOOL bSigned);
__declspec(dllimport)
UINT
__stdcall
GetDlgItemInt(
 HWND hDlg,
 int nIDDlgItem,
 BOOL *lpTranslated,
 BOOL bSigned);
__declspec(dllimport)
BOOL
__stdcall
SetDlgItemTextA(
 HWND hDlg,
 int nIDDlgItem,
 LPCSTR lpString);
__declspec(dllimport)
BOOL
__stdcall
SetDlgItemTextW(
 HWND hDlg,
 int nIDDlgItem,
 LPCWSTR lpString);
__declspec(dllimport)
UINT
__stdcall
GetDlgItemTextA(
 HWND hDlg,
 int nIDDlgItem,
 LPSTR lpString,
 int cchMax);
__declspec(dllimport)
UINT
__stdcall
GetDlgItemTextW(
 HWND hDlg,
 int nIDDlgItem,
 LPWSTR lpString,
 int cchMax);
__declspec(dllimport)
BOOL
__stdcall
CheckDlgButton(
 HWND hDlg,
 int nIDButton,
 UINT uCheck);
__declspec(dllimport)
BOOL
__stdcall
CheckRadioButton(
 HWND hDlg,
 int nIDFirstButton,
 int nIDLastButton,
 int nIDCheckButton);
__declspec(dllimport)
UINT
__stdcall
IsDlgButtonChecked(
 HWND hDlg,
 int nIDButton);
__declspec(dllimport)
LRESULT
__stdcall
SendDlgItemMessageA(
 HWND hDlg,
 int nIDDlgItem,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
LRESULT
__stdcall
SendDlgItemMessageW(
 HWND hDlg,
 int nIDDlgItem,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
HWND
__stdcall
GetNextDlgGroupItem(
 HWND hDlg,
 HWND hCtl,
 BOOL bPrevious);
__declspec(dllimport)
HWND
__stdcall
GetNextDlgTabItem(
 HWND hDlg,
 HWND hCtl,
 BOOL bPrevious);
__declspec(dllimport)
int
__stdcall
GetDlgCtrlID(
 HWND hWnd);
__declspec(dllimport)
long
__stdcall
GetDialogBaseUnits(void);
__declspec(dllimport)
LRESULT
__stdcall
DefDlgProcA(
 HWND hDlg,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
LRESULT
__stdcall
DefDlgProcW(
 HWND hDlg,
 UINT Msg,
 WPARAM wParam,
 LPARAM lParam);
__declspec(dllimport)
BOOL
__stdcall
CallMsgFilterA(
 LPMSG lpMsg,
 int nCode);
__declspec(dllimport)
BOOL
__stdcall
CallMsgFilterW(
 LPMSG lpMsg,
 int nCode);
__declspec(dllimport)
BOOL
__stdcall
OpenClipboard(
 HWND hWndNewOwner);
__declspec(dllimport)
BOOL
__stdcall
CloseClipboard(
 void);
__declspec(dllimport)
DWORD
__stdcall
GetClipboardSequenceNumber(
 void);
__declspec(dllimport)
HWND
__stdcall
GetClipboardOwner(
 void);
__declspec(dllimport)
HWND
__stdcall
SetClipboardViewer(
 HWND hWndNewViewer);
__declspec(dllimport)
HWND
__stdcall
GetClipboardViewer(
 void);
__declspec(dllimport)
BOOL
__stdcall
ChangeClipboardChain(
 HWND hWndRemove,
 HWND hWndNewNext);
__declspec(dllimport)
HANDLE
__stdcall
SetClipboardData(
 UINT uFormat,
 HANDLE hMem);
__declspec(dllimport)
HANDLE
__stdcall
GetClipboardData(
 UINT uFormat);
__declspec(dllimport)
UINT
__stdcall
RegisterClipboardFormatA(
 LPCSTR lpszFormat);
__declspec(dllimport)
UINT
__stdcall
RegisterClipboardFormatW(
 LPCWSTR lpszFormat);
__declspec(dllimport)
int
__stdcall
CountClipboardFormats(
 void);
__declspec(dllimport)
UINT
__stdcall
EnumClipboardFormats(
 UINT format);
__declspec(dllimport)
int
__stdcall
GetClipboardFormatNameA(
 UINT format,
 LPSTR lpszFormatName,
 int cchMaxCount);
__declspec(dllimport)
int
__stdcall
GetClipboardFormatNameW(
 UINT format,
 LPWSTR lpszFormatName,
 int cchMaxCount);
__declspec(dllimport)
BOOL
__stdcall
EmptyClipboard(
 void);
__declspec(dllimport)
BOOL
__stdcall
IsClipboardFormatAvailable(
 UINT format);
__declspec(dllimport)
int
__stdcall
GetPriorityClipboardFormat(
 UINT *paFormatPriorityList,
 int cFormats);
__declspec(dllimport)
HWND
__stdcall
GetOpenClipboardWindow(
 void);
__declspec(dllimport)
BOOL
__stdcall
AddClipboardFormatListener(
 HWND hwnd);
__declspec(dllimport)
BOOL
__stdcall
RemoveClipboardFormatListener(
 HWND hwnd);
__declspec(dllimport)
BOOL
__stdcall
GetUpdatedClipboardFormats(
 PUINT lpuiFormats,
 UINT cFormats,
 PUINT pcFormatsOut);
__declspec(dllimport)
BOOL
__stdcall
CharToOemA(
 LPCSTR pSrc,
 LPSTR pDst);
__declspec(dllimport)
BOOL
__stdcall
CharToOemW(
 LPCWSTR pSrc,
 LPSTR pDst);
__declspec(dllimport)
BOOL
__stdcall
OemToCharA(
 LPCSTR pSrc,
 LPSTR pDst);
__declspec(dllimport)
BOOL
__stdcall
OemToCharW(
 LPCSTR pSrc,
 LPWSTR pDst);
__declspec(dllimport)
BOOL
__stdcall
CharToOemBuffA(
 LPCSTR lpszSrc,
 LPSTR lpszDst,
 DWORD cchDstLength);
__declspec(dllimport)
BOOL
__stdcall
CharToOemBuffW(
 LPCWSTR lpszSrc,
 LPSTR lpszDst,
 DWORD cchDstLength);
__declspec(dllimport)
BOOL
__stdcall
OemToCharBuffA(
 LPCSTR lpszSrc,
 LPSTR lpszDst,
 DWORD cchDstLength);
__declspec(dllimport)
BOOL
__stdcall
OemToCharBuffW(
 LPCSTR lpszSrc,
 LPWSTR lpszDst,
 DWORD cchDstLength);
__declspec(dllimport)
LPSTR
__stdcall
CharUpperA(
 LPSTR lpsz);
__declspec(dllimport)
LPWSTR
__stdcall
CharUpperW(
 LPWSTR lpsz);
__declspec(dllimport)
DWORD
__stdcall
CharUpperBuffA(
 LPSTR lpsz,
 DWORD cchLength);
__declspec(dllimport)
DWORD
__stdcall
CharUpperBuffW(
 LPWSTR lpsz,
 DWORD cchLength);
__declspec(dllimport)
LPSTR
__stdcall
CharLowerA(
 LPSTR lpsz);
__declspec(dllimport)
LPWSTR
__stdcall
CharLowerW(
 LPWSTR lpsz);
__declspec(dllimport)
DWORD
__stdcall
CharLowerBuffA(
 LPSTR lpsz,
 DWORD cchLength);
__declspec(dllimport)
DWORD
__stdcall
CharLowerBuffW(
 LPWSTR lpsz,
 DWORD cchLength);
__declspec(dllimport)
LPSTR
__stdcall
CharNextA(
 LPCSTR lpsz);
__declspec(dllimport)
LPWSTR
__stdcall
CharNextW(
 LPCWSTR lpsz);
__declspec(dllimport)
LPSTR
__stdcall
CharPrevA(
 LPCSTR lpszStart,
 LPCSTR lpszCurrent);
__declspec(dllimport)
LPWSTR
__stdcall
CharPrevW(
 LPCWSTR lpszStart,
 LPCWSTR lpszCurrent);
__declspec(dllimport)
LPSTR
__stdcall
CharNextExA(
 WORD CodePage,
 LPCSTR lpCurrentChar,
 DWORD dwFlags);
__declspec(dllimport)
LPSTR
__stdcall
CharPrevExA(
 WORD CodePage,
 LPCSTR lpStart,
 LPCSTR lpCurrentChar,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
IsCharAlphaA(
 CHAR ch);
__declspec(dllimport)
BOOL
__stdcall
IsCharAlphaW(
 WCHAR ch);
__declspec(dllimport)
BOOL
__stdcall
IsCharAlphaNumericA(
 CHAR ch);
__declspec(dllimport)
BOOL
__stdcall
IsCharAlphaNumericW(
 WCHAR ch);
__declspec(dllimport)
BOOL
__stdcall
IsCharUpperA(
 CHAR ch);
__declspec(dllimport)
BOOL
__stdcall
IsCharUpperW(
 WCHAR ch);
__declspec(dllimport)
BOOL
__stdcall
IsCharLowerA(
 CHAR ch);
__declspec(dllimport)
BOOL
__stdcall
IsCharLowerW(
 WCHAR ch);
__declspec(dllimport)
HWND
__stdcall
SetFocus(
 HWND hWnd);
__declspec(dllimport)
HWND
__stdcall
GetActiveWindow(
 void);
__declspec(dllimport)
HWND
__stdcall
GetFocus(
 void);
__declspec(dllimport)
UINT
__stdcall
GetKBCodePage(
 void);
__declspec(dllimport)
SHORT
__stdcall
GetKeyState(
 int nVirtKey);
__declspec(dllimport)
SHORT
__stdcall
GetAsyncKeyState(
 int vKey);
__declspec(dllimport)
BOOL
__stdcall
GetKeyboardState(
 PBYTE lpKeyState);
__declspec(dllimport)
BOOL
__stdcall
SetKeyboardState(
 LPBYTE lpKeyState);
__declspec(dllimport)
int
__stdcall
GetKeyNameTextA(
 LONG lParam,
 LPSTR lpString,
 int cchSize);
__declspec(dllimport)
int
__stdcall
GetKeyNameTextW(
 LONG lParam,
 LPWSTR lpString,
 int cchSize);
__declspec(dllimport)
int
__stdcall
GetKeyboardType(
 int nTypeFlag);
__declspec(dllimport)
int
__stdcall
ToAscii(
 UINT uVirtKey,
 UINT uScanCode,
 const BYTE *lpKeyState,
 LPWORD lpChar,
 UINT uFlags);
__declspec(dllimport)
int
__stdcall
ToAsciiEx(
 UINT uVirtKey,
 UINT uScanCode,
 const BYTE *lpKeyState,
 LPWORD lpChar,
 UINT uFlags,
 HKL dwhkl);
__declspec(dllimport)
int
__stdcall
ToUnicode(
 UINT wVirtKey,
 UINT wScanCode,
 const BYTE *lpKeyState,
 LPWSTR pwszBuff,
 int cchBuff,
 UINT wFlags);
__declspec(dllimport)
DWORD
__stdcall
OemKeyScan(
 WORD wOemChar);
__declspec(dllimport)
SHORT
__stdcall
VkKeyScanA(
 CHAR ch);
__declspec(dllimport)
SHORT
__stdcall
VkKeyScanW(
 WCHAR ch);
__declspec(dllimport)
SHORT
__stdcall
VkKeyScanExA(
 CHAR ch,
 HKL dwhkl);
__declspec(dllimport)
SHORT
__stdcall
VkKeyScanExW(
 WCHAR ch,
 HKL dwhkl);
__declspec(dllimport)
void
__stdcall
keybd_event(
 BYTE bVk,
 BYTE bScan,
 DWORD dwFlags,
 ULONG_PTR dwExtraInfo);
__declspec(dllimport)
void
__stdcall
mouse_event(
 DWORD dwFlags,
 DWORD dx,
 DWORD dy,
 DWORD dwData,
 ULONG_PTR dwExtraInfo);
typedef struct tagMOUSEINPUT {
 LONG dx;
 LONG dy;
 DWORD mouseData;
 DWORD dwFlags;
 DWORD time;
 ULONG_PTR dwExtraInfo;
} MOUSEINPUT, *PMOUSEINPUT, * LPMOUSEINPUT;
typedef struct tagKEYBDINPUT {
 WORD wVk;
 WORD wScan;
 DWORD dwFlags;
 DWORD time;
 ULONG_PTR dwExtraInfo;
} KEYBDINPUT, *PKEYBDINPUT, * LPKEYBDINPUT;
typedef struct tagHARDWAREINPUT {
 DWORD uMsg;
 WORD wParamL;
 WORD wParamH;
} HARDWAREINPUT, *PHARDWAREINPUT, * LPHARDWAREINPUT;
typedef struct tagINPUT {
 DWORD type;
 union
 {
 MOUSEINPUT mi;
 KEYBDINPUT ki;
 HARDWAREINPUT hi;
 };
} INPUT, *PINPUT, * LPINPUT;
__declspec(dllimport)
UINT
__stdcall
SendInput(
 UINT cInputs,
 LPINPUT pInputs,
 int cbSize);
typedef struct tagLASTINPUTINFO {
 UINT cbSize;
 DWORD dwTime;
} LASTINPUTINFO, * PLASTINPUTINFO;
__declspec(dllimport)
BOOL
__stdcall
GetLastInputInfo(
 PLASTINPUTINFO plii);
__declspec(dllimport)
UINT
__stdcall
MapVirtualKeyA(
 UINT uCode,
 UINT uMapType);
__declspec(dllimport)
UINT
__stdcall
MapVirtualKeyW(
 UINT uCode,
 UINT uMapType);
__declspec(dllimport)
UINT
__stdcall
MapVirtualKeyExA(
 UINT uCode,
 UINT uMapType,
 HKL dwhkl);
__declspec(dllimport)
UINT
__stdcall
MapVirtualKeyExW(
 UINT uCode,
 UINT uMapType,
 HKL dwhkl);
__declspec(dllimport)
BOOL
__stdcall
GetInputState(
 void);
__declspec(dllimport)
DWORD
__stdcall
GetQueueStatus(
 UINT flags);
__declspec(dllimport)
HWND
__stdcall
GetCapture(
 void);
__declspec(dllimport)
HWND
__stdcall
SetCapture(
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
ReleaseCapture(
 void);
__declspec(dllimport)
DWORD
__stdcall
MsgWaitForMultipleObjects(
 DWORD nCount,
 const HANDLE *pHandles,
 BOOL fWaitAll,
 DWORD dwMilliseconds,
 DWORD dwWakeMask);
__declspec(dllimport)
DWORD
__stdcall
MsgWaitForMultipleObjectsEx(
 DWORD nCount,
 const HANDLE *pHandles,
 DWORD dwMilliseconds,
 DWORD dwWakeMask,
 DWORD dwFlags);
__declspec(dllimport)
UINT_PTR
__stdcall
SetTimer(
 HWND hWnd,
 UINT_PTR nIDEvent,
 UINT uElapse,
 TIMERPROC lpTimerFunc);
__declspec(dllimport)
BOOL
__stdcall
KillTimer(
 HWND hWnd,
 UINT_PTR uIDEvent);
__declspec(dllimport)
BOOL
__stdcall
IsWindowUnicode(
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
EnableWindow(
 HWND hWnd,
 BOOL bEnable);
__declspec(dllimport)
BOOL
__stdcall
IsWindowEnabled(
 HWND hWnd);
__declspec(dllimport)
HACCEL
__stdcall
LoadAcceleratorsA(
 HINSTANCE hInstance,
 LPCSTR lpTableName);
__declspec(dllimport)
HACCEL
__stdcall
LoadAcceleratorsW(
 HINSTANCE hInstance,
 LPCWSTR lpTableName);
__declspec(dllimport)
HACCEL
__stdcall
CreateAcceleratorTableA(
 LPACCEL paccel,
 int cAccel);
__declspec(dllimport)
HACCEL
__stdcall
CreateAcceleratorTableW(
 LPACCEL paccel,
 int cAccel);
__declspec(dllimport)
BOOL
__stdcall
DestroyAcceleratorTable(
 HACCEL hAccel);
__declspec(dllimport)
int
__stdcall
CopyAcceleratorTableA(
 HACCEL hAccelSrc,
 LPACCEL lpAccelDst,
 int cAccelEntries);
__declspec(dllimport)
int
__stdcall
CopyAcceleratorTableW(
 HACCEL hAccelSrc,
 LPACCEL lpAccelDst,
 int cAccelEntries);
__declspec(dllimport)
int
__stdcall
TranslateAcceleratorA(
 HWND hWnd,
 HACCEL hAccTable,
 LPMSG lpMsg);
__declspec(dllimport)
int
__stdcall
TranslateAcceleratorW(
 HWND hWnd,
 HACCEL hAccTable,
 LPMSG lpMsg);
__declspec(dllimport)
int
__stdcall
GetSystemMetrics(
 int nIndex);

typedef struct tagDROPSTRUCT
{
 HWND hwndSource;
 HWND hwndSink;
 DWORD wFmt;
 ULONG_PTR dwData;
 POINT ptDrop;
 DWORD dwControlData;
} DROPSTRUCT, *PDROPSTRUCT, *LPDROPSTRUCT;
__declspec(dllimport)
DWORD
__stdcall
DragObject(
 HWND hwndParent,
 HWND hwndFrom,
 UINT fmt,
 ULONG_PTR data,
 HCURSOR hcur);
__declspec(dllimport)
BOOL
__stdcall
DragDetect(
 HWND hwnd,
 POINT pt);
__declspec(dllimport)
BOOL
__stdcall
DrawIcon(
 HDC hDC,
 int X,
 int Y,
 HICON hIcon);
__declspec(dllimport)
BOOL
__stdcall
GrayStringA(
 HDC hDC,
 HBRUSH hBrush,
 GRAYSTRINGPROC lpOutputFunc,
 LPARAM lpData,
 int nCount,
 int X,
 int Y,
 int nWidth,
 int nHeight);
__declspec(dllimport)
BOOL
__stdcall
GrayStringW(
 HDC hDC,
 HBRUSH hBrush,
 GRAYSTRINGPROC lpOutputFunc,
 LPARAM lpData,
 int nCount,
 int X,
 int Y,
 int nWidth,
 int nHeight);
__declspec(dllimport)
BOOL
__stdcall
DrawStateA(
 HDC hdc,
 HBRUSH hbrFore,
 DRAWSTATEPROC qfnCallBack,
 LPARAM lData,
 WPARAM wData,
 int x,
 int y,
 int cx,
 int cy,
 UINT uFlags);
__declspec(dllimport)
BOOL
__stdcall
DrawStateW(
 HDC hdc,
 HBRUSH hbrFore,
 DRAWSTATEPROC qfnCallBack,
 LPARAM lData,
 WPARAM wData,
 int x,
 int y,
 int cx,
 int cy,
 UINT uFlags);
__declspec(dllimport)
LONG
__stdcall
TabbedTextOutA(
 HDC hdc,
 int x,
 int y,
 LPCSTR lpString,
 int chCount,
 int nTabPositions,
 const INT *lpnTabStopPositions,
 int nTabOrigin);
__declspec(dllimport)
LONG
__stdcall
TabbedTextOutW(
 HDC hdc,
 int x,
 int y,
 LPCWSTR lpString,
 int chCount,
 int nTabPositions,
 const INT *lpnTabStopPositions,
 int nTabOrigin);
__declspec(dllimport)
DWORD
__stdcall
GetTabbedTextExtentA(
 HDC hdc,
 LPCSTR lpString,
 int chCount,
 int nTabPositions,
 const INT *lpnTabStopPositions);
__declspec(dllimport)
DWORD
__stdcall
GetTabbedTextExtentW(
 HDC hdc,
 LPCWSTR lpString,
 int chCount,
 int nTabPositions,
 const INT *lpnTabStopPositions);
__declspec(dllimport)
BOOL
__stdcall
UpdateWindow(
 HWND hWnd);
__declspec(dllimport)
HWND
__stdcall
SetActiveWindow(
 HWND hWnd);
__declspec(dllimport)
HWND
__stdcall
GetForegroundWindow(
 void);
__declspec(dllimport)
BOOL
__stdcall
PaintDesktop(
 HDC hdc);
__declspec(dllimport)
void
__stdcall
SwitchToThisWindow(
 HWND hwnd,
 BOOL fUnknown);
__declspec(dllimport)
BOOL
__stdcall
SetForegroundWindow(
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
AllowSetForegroundWindow(
 DWORD dwProcessId);
__declspec(dllimport)
BOOL
__stdcall
LockSetForegroundWindow(
 UINT uLockCode);
__declspec(dllimport)
HWND
__stdcall
WindowFromDC(
 HDC hDC);
__declspec(dllimport)
HDC
__stdcall
GetDC(
 HWND hWnd);
__declspec(dllimport)
HDC
__stdcall
GetDCEx(
 HWND hWnd,
 HRGN hrgnClip,
 DWORD flags);
__declspec(dllimport)
HDC
__stdcall
GetWindowDC(
 HWND hWnd);
__declspec(dllimport)
int
__stdcall
ReleaseDC(
 HWND hWnd,
 HDC hDC);
__declspec(dllimport)
HDC
__stdcall
BeginPaint(
 HWND hWnd,
 LPPAINTSTRUCT lpPaint);
__declspec(dllimport)
BOOL
__stdcall
EndPaint(
 HWND hWnd,
 const PAINTSTRUCT *lpPaint);
__declspec(dllimport)
BOOL
__stdcall
GetUpdateRect(
 HWND hWnd,
 LPRECT lpRect,
 BOOL bErase);
__declspec(dllimport)
int
__stdcall
GetUpdateRgn(
 HWND hWnd,
 HRGN hRgn,
 BOOL bErase);
__declspec(dllimport)
int
__stdcall
SetWindowRgn(
 HWND hWnd,
 HRGN hRgn,
 BOOL bRedraw);
__declspec(dllimport)
int
__stdcall
GetWindowRgn(
 HWND hWnd,
 HRGN hRgn);
__declspec(dllimport)
int
__stdcall
GetWindowRgnBox(
 HWND hWnd,
 LPRECT lprc);
__declspec(dllimport)
int
__stdcall
ExcludeUpdateRgn(
 HDC hDC,
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
InvalidateRect(
 HWND hWnd,
 const RECT *lpRect,
 BOOL bErase);
__declspec(dllimport)
BOOL
__stdcall
ValidateRect(
 HWND hWnd,
 const RECT *lpRect);
__declspec(dllimport)
BOOL
__stdcall
InvalidateRgn(
 HWND hWnd,
 HRGN hRgn,
 BOOL bErase);
__declspec(dllimport)
BOOL
__stdcall
ValidateRgn(
 HWND hWnd,
 HRGN hRgn);
__declspec(dllimport)
BOOL
__stdcall
RedrawWindow(
 HWND hWnd,
 const RECT *lprcUpdate,
 HRGN hrgnUpdate,
 UINT flags);
__declspec(dllimport)
BOOL
__stdcall
LockWindowUpdate(
 HWND hWndLock);
__declspec(dllimport)
BOOL
__stdcall
ScrollWindow(
 HWND hWnd,
 int XAmount,
 int YAmount,
 const RECT *lpRect,
 const RECT *lpClipRect);
__declspec(dllimport)
BOOL
__stdcall
ScrollDC(
 HDC hDC,
 int dx,
 int dy,
 const RECT *lprcScroll,
 const RECT *lprcClip,
 HRGN hrgnUpdate,
 LPRECT lprcUpdate);
__declspec(dllimport)
int
__stdcall
ScrollWindowEx(
 HWND hWnd,
 int dx,
 int dy,
 const RECT *prcScroll,
 const RECT *prcClip,
 HRGN hrgnUpdate,
 LPRECT prcUpdate,
 UINT flags);
__declspec(dllimport)
BOOL
__stdcall
SetPropA(
 HWND hWnd,
 LPCSTR lpString,
 HANDLE hData);
__declspec(dllimport)
BOOL
__stdcall
SetPropW(
 HWND hWnd,
 LPCWSTR lpString,
 HANDLE hData);
__declspec(dllimport)
HANDLE
__stdcall
GetPropA(
 HWND hWnd,
 LPCSTR lpString);
__declspec(dllimport)
HANDLE
__stdcall
GetPropW(
 HWND hWnd,
 LPCWSTR lpString);
__declspec(dllimport)
HANDLE
__stdcall
RemovePropA(
 HWND hWnd,
 LPCSTR lpString);
__declspec(dllimport)
HANDLE
__stdcall
RemovePropW(
 HWND hWnd,
 LPCWSTR lpString);
__declspec(dllimport)
int
__stdcall
EnumPropsExA(
 HWND hWnd,
 PROPENUMPROCEXA lpEnumFunc,
 LPARAM lParam);
__declspec(dllimport)
int
__stdcall
EnumPropsExW(
 HWND hWnd,
 PROPENUMPROCEXW lpEnumFunc,
 LPARAM lParam);
__declspec(dllimport)
int
__stdcall
EnumPropsA(
 HWND hWnd,
 PROPENUMPROCA lpEnumFunc);
__declspec(dllimport)
int
__stdcall
EnumPropsW(
 HWND hWnd,
 PROPENUMPROCW lpEnumFunc);
__declspec(dllimport)
BOOL
__stdcall
SetWindowTextA(
 HWND hWnd,
 LPCSTR lpString);
__declspec(dllimport)
BOOL
__stdcall
SetWindowTextW(
 HWND hWnd,
 LPCWSTR lpString);
__declspec(dllimport)
int
__stdcall
GetWindowTextA(
 HWND hWnd,
 LPSTR lpString,
 int nMaxCount);
__declspec(dllimport)
int
__stdcall
GetWindowTextW(
 HWND hWnd,
 LPWSTR lpString,
 int nMaxCount);
__declspec(dllimport)
int
__stdcall
GetWindowTextLengthA(
 HWND hWnd);
__declspec(dllimport)
int
__stdcall
GetWindowTextLengthW(
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
GetClientRect(
 HWND hWnd,
 LPRECT lpRect);
__declspec(dllimport)
BOOL
__stdcall
GetWindowRect(
 HWND hWnd,
 LPRECT lpRect);
__declspec(dllimport)
BOOL
__stdcall
AdjustWindowRect(
 LPRECT lpRect,
 DWORD dwStyle,
 BOOL bMenu);
__declspec(dllimport)
BOOL
__stdcall
AdjustWindowRectEx(
 LPRECT lpRect,
 DWORD dwStyle,
 BOOL bMenu,
 DWORD dwExStyle);
typedef struct tagHELPINFO
{
 UINT cbSize;
 int iContextType;
 int iCtrlId;
 HANDLE hItemHandle;
 DWORD_PTR dwContextId;
 POINT MousePos;
} HELPINFO, *LPHELPINFO;
__declspec(dllimport)
BOOL
__stdcall
SetWindowContextHelpId(
 HWND,
 DWORD);
__declspec(dllimport)
DWORD
__stdcall
GetWindowContextHelpId(
 HWND);
__declspec(dllimport)
BOOL
__stdcall
SetMenuContextHelpId(
 HMENU,
 DWORD);
__declspec(dllimport)
DWORD
__stdcall
GetMenuContextHelpId(
 HMENU);
__declspec(dllimport)
int
__stdcall
MessageBoxA(
 HWND hWnd,
 LPCSTR lpText,
 LPCSTR lpCaption,
 UINT uType);
__declspec(dllimport)
int
__stdcall
MessageBoxW(
 HWND hWnd,
 LPCWSTR lpText,
 LPCWSTR lpCaption,
 UINT uType);
__declspec(dllimport)
int
__stdcall
MessageBoxExA(
 HWND hWnd,
 LPCSTR lpText,
 LPCSTR lpCaption,
 UINT uType,
 WORD wLanguageId);
__declspec(dllimport)
int
__stdcall
MessageBoxExW(
 HWND hWnd,
 LPCWSTR lpText,
 LPCWSTR lpCaption,
 UINT uType,
 WORD wLanguageId);
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
 DWORD_PTR dwContextHelpId;
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
 DWORD_PTR dwContextHelpId;
 MSGBOXCALLBACK lpfnMsgBoxCallback;
 DWORD dwLanguageId;
} MSGBOXPARAMSW, *PMSGBOXPARAMSW, *LPMSGBOXPARAMSW;
typedef MSGBOXPARAMSA MSGBOXPARAMS;
typedef PMSGBOXPARAMSA PMSGBOXPARAMS;
typedef LPMSGBOXPARAMSA LPMSGBOXPARAMS;
__declspec(dllimport)
int
__stdcall
MessageBoxIndirectA(
 const MSGBOXPARAMSA * lpmbp);
__declspec(dllimport)
int
__stdcall
MessageBoxIndirectW(
 const MSGBOXPARAMSW * lpmbp);
__declspec(dllimport)
BOOL
__stdcall
MessageBeep(
 UINT uType);
__declspec(dllimport)
int
__stdcall
ShowCursor(
 BOOL bShow);
__declspec(dllimport)
BOOL
__stdcall
SetCursorPos(
 int X,
 int Y);
__declspec(dllimport)
BOOL
__stdcall
SetPhysicalCursorPos(
 int X,
 int Y);
__declspec(dllimport)
HCURSOR
__stdcall
SetCursor(
 HCURSOR hCursor);
__declspec(dllimport)
BOOL
__stdcall
GetCursorPos(
 LPPOINT lpPoint);
__declspec(dllimport)
BOOL
__stdcall
GetPhysicalCursorPos(
 LPPOINT lpPoint);
__declspec(dllimport)
BOOL
__stdcall
ClipCursor(
 const RECT *lpRect);
__declspec(dllimport)
BOOL
__stdcall
GetClipCursor(
 LPRECT lpRect);
__declspec(dllimport)
HCURSOR
__stdcall
GetCursor(
 void);
__declspec(dllimport)
BOOL
__stdcall
CreateCaret(
 HWND hWnd,
 HBITMAP hBitmap,
 int nWidth,
 int nHeight);
__declspec(dllimport)
UINT
__stdcall
GetCaretBlinkTime(
 void);
__declspec(dllimport)
BOOL
__stdcall
SetCaretBlinkTime(
 UINT uMSeconds);
__declspec(dllimport)
BOOL
__stdcall
DestroyCaret(
 void);
__declspec(dllimport)
BOOL
__stdcall
HideCaret(
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
ShowCaret(
 HWND hWnd);
__declspec(dllimport)
BOOL
__stdcall
SetCaretPos(
 int X,
 int Y);
__declspec(dllimport)
BOOL
__stdcall
GetCaretPos(
 LPPOINT lpPoint);
__declspec(dllimport)
BOOL
__stdcall
ClientToScreen(
 HWND hWnd,
 LPPOINT lpPoint);
__declspec(dllimport)
BOOL
__stdcall
ScreenToClient(
 HWND hWnd,
 LPPOINT lpPoint);
__declspec(dllimport)
BOOL
__stdcall
LogicalToPhysicalPoint(
 HWND hWnd,
 LPPOINT lpPoint);
__declspec(dllimport)
BOOL
__stdcall
PhysicalToLogicalPoint(
 HWND hWnd,
 LPPOINT lpPoint);
__declspec(dllimport)
int
__stdcall
MapWindowPoints(
 HWND hWndFrom,
 HWND hWndTo,
 LPPOINT lpPoints,
 UINT cPoints);
__declspec(dllimport)
HWND
__stdcall
WindowFromPoint(
 POINT Point);
__declspec(dllimport)
HWND
__stdcall
WindowFromPhysicalPoint(
 POINT Point);
__declspec(dllimport)
HWND
__stdcall
ChildWindowFromPoint(
 HWND hWndParent,
 POINT Point);
__declspec(dllimport)
HWND
__stdcall
ChildWindowFromPointEx(
 HWND hwnd,
 POINT pt,
 UINT flags);
__declspec(dllimport)
BOOL
__stdcall
DrawFocusRect(
 HDC hDC,
 const RECT * lprc);
__declspec(dllimport)
int
__stdcall
FillRect(
 HDC hDC,
 const RECT *lprc,
 HBRUSH hbr);
__declspec(dllimport)
int
__stdcall
FrameRect(
 HDC hDC,
 const RECT *lprc,
 HBRUSH hbr);
__declspec(dllimport)
BOOL
__stdcall
InvertRect(
 HDC hDC,
 const RECT *lprc);
__declspec(dllimport)
BOOL
__stdcall
SetRect(
 LPRECT lprc,
 int xLeft,
 int yTop,
 int xRight,
 int yBottom);
__declspec(dllimport)
BOOL
__stdcall
SetRectEmpty(
 LPRECT lprc);
__declspec(dllimport)
BOOL
__stdcall
CopyRect(
 LPRECT lprcDst,
 const RECT *lprcSrc);
__declspec(dllimport)
BOOL
__stdcall
InflateRect(
 LPRECT lprc,
 int dx,
 int dy);
__declspec(dllimport)
BOOL
__stdcall
IntersectRect(
 LPRECT lprcDst,
 const RECT *lprcSrc1,
 const RECT *lprcSrc2);
__declspec(dllimport)
BOOL
__stdcall
UnionRect(
 LPRECT lprcDst,
 const RECT *lprcSrc1,
 const RECT *lprcSrc2);
__declspec(dllimport)
BOOL
__stdcall
SubtractRect(
 LPRECT lprcDst,
 const RECT *lprcSrc1,
 const RECT *lprcSrc2);
__declspec(dllimport)
BOOL
__stdcall
OffsetRect(
 LPRECT lprc,
 int dx,
 int dy);
__declspec(dllimport)
BOOL
__stdcall
IsRectEmpty(
 const RECT *lprc);
__declspec(dllimport)
BOOL
__stdcall
EqualRect(
 const RECT *lprc1,
 const RECT *lprc2);
__declspec(dllimport)
BOOL
__stdcall
PtInRect(
 const RECT *lprc,
 POINT pt);
__declspec(dllimport)
WORD
__stdcall
GetWindowWord(
 HWND hWnd,
 int nIndex);
__declspec(dllimport)
WORD
__stdcall
SetWindowWord(
 HWND hWnd,
 int nIndex,
 WORD wNewWord);
__declspec(dllimport)
LONG
__stdcall
GetWindowLongA(
 HWND hWnd,
 int nIndex);
__declspec(dllimport)
LONG
__stdcall
GetWindowLongW(
 HWND hWnd,
 int nIndex);
__declspec(dllimport)
LONG
__stdcall
SetWindowLongA(
 HWND hWnd,
 int nIndex,
 LONG dwNewLong);
__declspec(dllimport)
LONG
__stdcall
SetWindowLongW(
 HWND hWnd,
 int nIndex,
 LONG dwNewLong);
__declspec(dllimport)
WORD
__stdcall
GetClassWord(
 HWND hWnd,
 int nIndex);
__declspec(dllimport)
WORD
__stdcall
SetClassWord(
 HWND hWnd,
 int nIndex,
 WORD wNewWord);
__declspec(dllimport)
DWORD
__stdcall
GetClassLongA(
 HWND hWnd,
 int nIndex);
__declspec(dllimport)
DWORD
__stdcall
GetClassLongW(
 HWND hWnd,
 int nIndex);
__declspec(dllimport)
DWORD
__stdcall
SetClassLongA(
 HWND hWnd,
 int nIndex,
 LONG dwNewLong);
__declspec(dllimport)
DWORD
__stdcall
SetClassLongW(
 HWND hWnd,
 int nIndex,
 LONG dwNewLong);
__declspec(dllimport)
BOOL
__stdcall
GetProcessDefaultLayout(
 DWORD *pdwDefaultLayout);
__declspec(dllimport)
BOOL
__stdcall
SetProcessDefaultLayout(
 DWORD dwDefaultLayout);
__declspec(dllimport)
HWND
__stdcall
GetDesktopWindow(
 void);
__declspec(dllimport)
HWND
__stdcall
GetParent(
 HWND hWnd);
__declspec(dllimport)
HWND
__stdcall
SetParent(
 HWND hWndChild,
 HWND hWndNewParent);
__declspec(dllimport)
BOOL
__stdcall
EnumChildWindows(
 HWND hWndParent,
 WNDENUMPROC lpEnumFunc,
 LPARAM lParam);
__declspec(dllimport)
HWND
__stdcall
FindWindowA(
 LPCSTR lpClassName,
 LPCSTR lpWindowName);
__declspec(dllimport)
HWND
__stdcall
FindWindowW(
 LPCWSTR lpClassName,
 LPCWSTR lpWindowName);
__declspec(dllimport)
HWND
__stdcall
FindWindowExA(
 HWND hWndParent,
 HWND hWndChildAfter,
 LPCSTR lpszClass,
 LPCSTR lpszWindow);
__declspec(dllimport)
HWND
__stdcall
FindWindowExW(
 HWND hWndParent,
 HWND hWndChildAfter,
 LPCWSTR lpszClass,
 LPCWSTR lpszWindow);
__declspec(dllimport)
HWND
__stdcall
GetShellWindow(
 void);
__declspec(dllimport)
BOOL
__stdcall
RegisterShellHookWindow(
 HWND hwnd);
__declspec(dllimport)
BOOL
__stdcall
DeregisterShellHookWindow(
 HWND hwnd);
__declspec(dllimport)
BOOL
__stdcall
EnumWindows(
 WNDENUMPROC lpEnumFunc,
 LPARAM lParam);
__declspec(dllimport)
BOOL
__stdcall
EnumThreadWindows(
 DWORD dwThreadId,
 WNDENUMPROC lpfn,
 LPARAM lParam);
__declspec(dllimport)
int
__stdcall
GetClassNameA(
 HWND hWnd,
 LPSTR lpClassName,
 int nMaxCount
 );
__declspec(dllimport)
int
__stdcall
GetClassNameW(
 HWND hWnd,
 LPWSTR lpClassName,
 int nMaxCount
 );
__declspec(dllimport)
HWND
__stdcall
GetTopWindow(
 HWND hWnd);
__declspec(dllimport)
DWORD
__stdcall
GetWindowThreadProcessId(
 HWND hWnd,
 LPDWORD lpdwProcessId);
__declspec(dllimport)
BOOL
__stdcall
IsGUIThread(
 BOOL bConvert);
__declspec(dllimport)
HWND
__stdcall
GetLastActivePopup(
 HWND hWnd);
__declspec(dllimport)
HWND
__stdcall
GetWindow(
 HWND hWnd,
 UINT uCmd);
__declspec(dllimport)
HBITMAP
__stdcall
LoadBitmapA(
 HINSTANCE hInstance,
 LPCSTR lpBitmapName);
__declspec(dllimport)
HBITMAP
__stdcall
LoadBitmapW(
 HINSTANCE hInstance,
 LPCWSTR lpBitmapName);
__declspec(dllimport)
HCURSOR
__stdcall
LoadCursorA(
 HINSTANCE hInstance,
 LPCSTR lpCursorName);
__declspec(dllimport)
HCURSOR
__stdcall
LoadCursorW(
 HINSTANCE hInstance,
 LPCWSTR lpCursorName);
__declspec(dllimport)
HCURSOR
__stdcall
LoadCursorFromFileA(
 LPCSTR lpFileName);
__declspec(dllimport)
HCURSOR
__stdcall
LoadCursorFromFileW(
 LPCWSTR lpFileName);
__declspec(dllimport)
HCURSOR
__stdcall
CreateCursor(
 HINSTANCE hInst,
 int xHotSpot,
 int yHotSpot,
 int nWidth,
 int nHeight,
 const void *pvANDPlane,
 const void *pvXORPlane);
__declspec(dllimport)
BOOL
__stdcall
DestroyCursor(
 HCURSOR hCursor);
__declspec(dllimport)
BOOL
__stdcall
SetSystemCursor(
 HCURSOR hcur,
 DWORD id);
typedef struct _ICONINFO {
 BOOL fIcon;
 DWORD xHotspot;
 DWORD yHotspot;
 HBITMAP hbmMask;
 HBITMAP hbmColor;
} ICONINFO;
typedef ICONINFO *PICONINFO;
__declspec(dllimport)
HICON
__stdcall
LoadIconA(
 HINSTANCE hInstance,
 LPCSTR lpIconName);
__declspec(dllimport)
HICON
__stdcall
LoadIconW(
 HINSTANCE hInstance,
 LPCWSTR lpIconName);
__declspec(dllimport)
UINT
__stdcall
PrivateExtractIconsA(
 LPCSTR szFileName,
 int nIconIndex,
 int cxIcon,
 int cyIcon,
 HICON *phicon,
 UINT *piconid,
 UINT nIcons,
 UINT flags);
__declspec(dllimport)
UINT
__stdcall
PrivateExtractIconsW(
 LPCWSTR szFileName,
 int nIconIndex,
 int cxIcon,
 int cyIcon,
 HICON *phicon,
 UINT *piconid,
 UINT nIcons,
 UINT flags);
__declspec(dllimport)
HICON
__stdcall
CreateIcon(
 HINSTANCE hInstance,
 int nWidth,
 int nHeight,
 BYTE cPlanes,
 BYTE cBitsPixel,
 const BYTE *lpbANDbits,
 const BYTE *lpbXORbits);
__declspec(dllimport)
BOOL
__stdcall
DestroyIcon(
 HICON hIcon);
__declspec(dllimport)
int
__stdcall
LookupIconIdFromDirectory(
 PBYTE presbits,
 BOOL fIcon);
__declspec(dllimport)
int
__stdcall
LookupIconIdFromDirectoryEx(
 PBYTE presbits,
 BOOL fIcon,
 int cxDesired,
 int cyDesired,
 UINT Flags);
__declspec(dllimport)
HICON
__stdcall
CreateIconFromResource(
 PBYTE presbits,
 DWORD dwResSize,
 BOOL fIcon,
 DWORD dwVer);
__declspec(dllimport)
HICON
__stdcall
CreateIconFromResourceEx(
 PBYTE presbits,
 DWORD dwResSize,
 BOOL fIcon,
 DWORD dwVer,
 int cxDesired,
 int cyDesired,
 UINT Flags);
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
__declspec(dllimport)
HANDLE
__stdcall
LoadImageA(
 HINSTANCE hInst,
 LPCSTR name,
 UINT type,
 int cx,
 int cy,
 UINT fuLoad);
__declspec(dllimport)
HANDLE
__stdcall
LoadImageW(
 HINSTANCE hInst,
 LPCWSTR name,
 UINT type,
 int cx,
 int cy,
 UINT fuLoad);

__declspec(dllimport)
HANDLE
__stdcall
CopyImage(
 HANDLE h,
 UINT type,
 int cx,
 int cy,
 UINT flags);
__declspec(dllimport) BOOL __stdcall DrawIconEx(
 HDC hdc,
 int xLeft,
 int yTop,
 HICON hIcon,
 int cxWidth,
 int cyWidth,
 UINT istepIfAniCur,
 HBRUSH hbrFlickerFreeDraw,
 UINT diFlags);
__declspec(dllimport)
HICON
__stdcall
CreateIconIndirect(
 PICONINFO piconinfo);
__declspec(dllimport)
HICON
__stdcall
CopyIcon(
 HICON hIcon);
__declspec(dllimport)
BOOL
__stdcall
GetIconInfo(
 HICON hIcon,
 PICONINFO piconinfo);
typedef struct _ICONINFOEXA {
 DWORD cbSize;
 BOOL fIcon;
 DWORD xHotspot;
 DWORD yHotspot;
 HBITMAP hbmMask;
 HBITMAP hbmColor;
 WORD wResID;
 CHAR szModName[260];
 CHAR szResName[260];
} ICONINFOEXA, *PICONINFOEXA;
typedef struct _ICONINFOEXW {
 DWORD cbSize;
 BOOL fIcon;
 DWORD xHotspot;
 DWORD yHotspot;
 HBITMAP hbmMask;
 HBITMAP hbmColor;
 WORD wResID;
 WCHAR szModName[260];
 WCHAR szResName[260];
} ICONINFOEXW, *PICONINFOEXW;
typedef ICONINFOEXA ICONINFOEX;
typedef PICONINFOEXA PICONINFOEX;
__declspec(dllimport)
BOOL
__stdcall
GetIconInfoExA(
 HICON hicon,
 PICONINFOEXA piconinfo);
__declspec(dllimport)
BOOL
__stdcall
GetIconInfoExW(
 HICON hicon,
 PICONINFOEXW piconinfo);
__declspec(dllimport)
int
__stdcall
LoadStringA(
 HINSTANCE hInstance,
 UINT uID,
 LPSTR lpBuffer,
 int cchBufferMax);
__declspec(dllimport)
int
__stdcall
LoadStringW(
 HINSTANCE hInstance,
 UINT uID,
 LPWSTR lpBuffer,
 int cchBufferMax);
__declspec(dllimport)
BOOL
__stdcall
IsDialogMessageA(
 HWND hDlg,
 LPMSG lpMsg);
__declspec(dllimport)
BOOL
__stdcall
IsDialogMessageW(
 HWND hDlg,
 LPMSG lpMsg);
__declspec(dllimport)
BOOL
__stdcall
MapDialogRect(
 HWND hDlg,
 LPRECT lpRect);
__declspec(dllimport)
int
__stdcall
DlgDirListA(
 HWND hDlg,
 LPSTR lpPathSpec,
 int nIDListBox,
 int nIDStaticPath,
 UINT uFileType);
__declspec(dllimport)
int
__stdcall
DlgDirListW(
 HWND hDlg,
 LPWSTR lpPathSpec,
 int nIDListBox,
 int nIDStaticPath,
 UINT uFileType);
__declspec(dllimport)
BOOL
__stdcall
DlgDirSelectExA(
 HWND hwndDlg,
 LPSTR lpString,
 int chCount,
 int idListBox);
__declspec(dllimport)
BOOL
__stdcall
DlgDirSelectExW(
 HWND hwndDlg,
 LPWSTR lpString,
 int chCount,
 int idListBox);
__declspec(dllimport)
int
__stdcall
DlgDirListComboBoxA(
 HWND hDlg,
 LPSTR lpPathSpec,
 int nIDComboBox,
 int nIDStaticPath,
 UINT uFiletype);
__declspec(dllimport)
int
__stdcall
DlgDirListComboBoxW(
 HWND hDlg,
 LPWSTR lpPathSpec,
 int nIDComboBox,
 int nIDStaticPath,
 UINT uFiletype);
__declspec(dllimport)
BOOL
__stdcall
DlgDirSelectComboBoxExA(
 HWND hwndDlg,
 LPSTR lpString,
 int cchOut,
 int idComboBox);
__declspec(dllimport)
BOOL
__stdcall
DlgDirSelectComboBoxExW(
 HWND hwndDlg,
 LPWSTR lpString,
 int cchOut,
 int idComboBox);
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
__declspec(dllimport)
int
__stdcall
SetScrollInfo(
 HWND hwnd,
 int nBar,
 LPCSCROLLINFO lpsi,
 BOOL redraw);
__declspec(dllimport)
BOOL
__stdcall
GetScrollInfo(
 HWND hwnd,
 int nBar,
 LPSCROLLINFO lpsi);
__declspec(dllimport)
DWORD
__stdcall
GetGuiResources(
 HANDLE hProcess,
 DWORD uiFlags);
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
 int iPaddedBorderWidth;
} NONCLIENTMETRICSA, *PNONCLIENTMETRICSA, * LPNONCLIENTMETRICSA;
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
 int iPaddedBorderWidth;
} NONCLIENTMETRICSW, *PNONCLIENTMETRICSW, * LPNONCLIENTMETRICSW;
typedef NONCLIENTMETRICSA NONCLIENTMETRICS;
typedef PNONCLIENTMETRICSA PNONCLIENTMETRICS;
typedef LPNONCLIENTMETRICSA LPNONCLIENTMETRICS;
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
#pragma once
typedef struct _VIDEOPARAMETERS {
 GUID Guid;
 ULONG dwOffset;
 ULONG dwCommand;
 ULONG dwFlags;
 ULONG dwMode;
 ULONG dwTVStandard;
 ULONG dwAvailableModes;
 ULONG dwAvailableTVStandard;
 ULONG dwFlickerFilter;
 ULONG dwOverScanX;
 ULONG dwOverScanY;
 ULONG dwMaxUnscaledX;
 ULONG dwMaxUnscaledY;
 ULONG dwPositionX;
 ULONG dwPositionY;
 ULONG dwBrightness;
 ULONG dwContrast;
 ULONG dwCPType;
 ULONG dwCPCommand;
 ULONG dwCPStandard;
 ULONG dwCPKey;
 ULONG bCP_APSTriggerBits;
 UCHAR bOEMCopyProtection[256];
} VIDEOPARAMETERS, *PVIDEOPARAMETERS, *LPVIDEOPARAMETERS;
__declspec(dllimport)
LONG
__stdcall
ChangeDisplaySettingsA(
 LPDEVMODEA lpDevMode,
 DWORD dwFlags);
__declspec(dllimport)
LONG
__stdcall
ChangeDisplaySettingsW(
 LPDEVMODEW lpDevMode,
 DWORD dwFlags);
__declspec(dllimport)
LONG
__stdcall
ChangeDisplaySettingsExA(
 LPCSTR lpszDeviceName,
 LPDEVMODEA lpDevMode,
 HWND hwnd,
 DWORD dwflags,
 LPVOID lParam);
__declspec(dllimport)
LONG
__stdcall
ChangeDisplaySettingsExW(
 LPCWSTR lpszDeviceName,
 LPDEVMODEW lpDevMode,
 HWND hwnd,
 DWORD dwflags,
 LPVOID lParam);
__declspec(dllimport)
BOOL
__stdcall
EnumDisplaySettingsA(
 LPCSTR lpszDeviceName,
 DWORD iModeNum,
 LPDEVMODEA lpDevMode);
__declspec(dllimport)
BOOL
__stdcall
EnumDisplaySettingsW(
 LPCWSTR lpszDeviceName,
 DWORD iModeNum,
 LPDEVMODEW lpDevMode);
__declspec(dllimport)
BOOL
__stdcall
EnumDisplaySettingsExA(
 LPCSTR lpszDeviceName,
 DWORD iModeNum,
 LPDEVMODEA lpDevMode,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
EnumDisplaySettingsExW(
 LPCWSTR lpszDeviceName,
 DWORD iModeNum,
 LPDEVMODEW lpDevMode,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
EnumDisplayDevicesA(
 LPCSTR lpDevice,
 DWORD iDevNum,
 PDISPLAY_DEVICEA lpDisplayDevice,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
EnumDisplayDevicesW(
 LPCWSTR lpDevice,
 DWORD iDevNum,
 PDISPLAY_DEVICEW lpDisplayDevice,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
SystemParametersInfoA(
 UINT uiAction,
 UINT uiParam,
 PVOID pvParam,
 UINT fWinIni);
__declspec(dllimport)
BOOL
__stdcall
SystemParametersInfoW(
 UINT uiAction,
 UINT uiParam,
 PVOID pvParam,
 UINT fWinIni);
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
__declspec(dllimport)
BOOL
__stdcall
SoundSentry(void);
typedef struct tagTOGGLEKEYS
{
 UINT cbSize;
 DWORD dwFlags;
} TOGGLEKEYS, *LPTOGGLEKEYS;
typedef struct tagAUDIODESCRIPTION {
 UINT cbSize;
 BOOL Enabled;
 LCID Locale;
} AUDIODESCRIPTION, *LPAUDIODESCRIPTION;
__declspec(dllimport)
void
__stdcall
SetDebugErrorLevel(
 DWORD dwLevel);
__declspec(dllimport)
void
__stdcall
SetLastErrorEx(
 DWORD dwErrCode,
 DWORD dwType);
__declspec(dllimport)
int
__stdcall
InternalGetWindowText(
 HWND hWnd,
 LPWSTR pString,
 int cchMaxCount);
__declspec(dllimport)
BOOL
__stdcall
CancelShutdown(
 void);
__declspec(dllimport)
HMONITOR
__stdcall
MonitorFromPoint(
 POINT pt,
 DWORD dwFlags);
__declspec(dllimport)
HMONITOR
__stdcall
MonitorFromRect(
 LPCRECT lprc,
 DWORD dwFlags);
__declspec(dllimport)
HMONITOR
__stdcall
MonitorFromWindow(
 HWND hwnd,
 DWORD dwFlags);
typedef struct tagMONITORINFO
{
 DWORD cbSize;
 RECT rcMonitor;
 RECT rcWork;
 DWORD dwFlags;
} MONITORINFO, *LPMONITORINFO;
typedef struct tagMONITORINFOEXA : public tagMONITORINFO
{
 CHAR szDevice[32];
} MONITORINFOEXA, *LPMONITORINFOEXA;
typedef struct tagMONITORINFOEXW : public tagMONITORINFO
{
 WCHAR szDevice[32];
} MONITORINFOEXW, *LPMONITORINFOEXW;
typedef MONITORINFOEXA MONITORINFOEX;
typedef LPMONITORINFOEXA LPMONITORINFOEX;
__declspec(dllimport)
BOOL
__stdcall
GetMonitorInfoA(
 HMONITOR hMonitor,
 LPMONITORINFO lpmi);
__declspec(dllimport)
BOOL
__stdcall
GetMonitorInfoW(
 HMONITOR hMonitor,
 LPMONITORINFO lpmi);
typedef BOOL (__stdcall* MONITORENUMPROC)(HMONITOR, HDC, LPRECT, LPARAM);
__declspec(dllimport)
BOOL
__stdcall
EnumDisplayMonitors(
 HDC hdc,
 LPCRECT lprcClip,
 MONITORENUMPROC lpfnEnum,
 LPARAM dwData);
__declspec(dllimport)
void
__stdcall
NotifyWinEvent(
 DWORD event,
 HWND hwnd,
 LONG idObject,
 LONG idChild);
typedef void (__stdcall* WINEVENTPROC)(
 HWINEVENTHOOK hWinEventHook,
 DWORD event,
 HWND hwnd,
 LONG idObject,
 LONG idChild,
 DWORD idEventThread,
 DWORD dwmsEventTime);
__declspec(dllimport)
HWINEVENTHOOK
__stdcall
SetWinEventHook(
 DWORD eventMin,
 DWORD eventMax,
 HMODULE hmodWinEventProc,
 WINEVENTPROC pfnWinEventProc,
 DWORD idProcess,
 DWORD idThread,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
IsWinEventHookInstalled(
 DWORD event);
__declspec(dllimport)
BOOL
__stdcall
UnhookWinEvent(
 HWINEVENTHOOK hWinEventHook);
typedef struct tagGUITHREADINFO
{
 DWORD cbSize;
 DWORD flags;
 HWND hwndActive;
 HWND hwndFocus;
 HWND hwndCapture;
 HWND hwndMenuOwner;
 HWND hwndMoveSize;
 HWND hwndCaret;
 RECT rcCaret;
} GUITHREADINFO, *PGUITHREADINFO, * LPGUITHREADINFO;
__declspec(dllimport)
BOOL
__stdcall
GetGUIThreadInfo(
 DWORD idThread,
 PGUITHREADINFO pgui);
__declspec(dllimport)
BOOL
__stdcall
BlockInput(
 BOOL fBlockIt);
__declspec(dllimport)
BOOL
__stdcall
SetProcessDPIAware(
 void);
__declspec(dllimport)
BOOL
__stdcall
IsProcessDPIAware(
 void);
__declspec(dllimport)
UINT
__stdcall
GetWindowModuleFileNameA(
 HWND hwnd,
 LPSTR pszFileName,
 UINT cchFileNameMax);
__declspec(dllimport)
UINT
__stdcall
GetWindowModuleFileNameW(
 HWND hwnd,
 LPWSTR pszFileName,
 UINT cchFileNameMax);
typedef struct tagCURSORINFO
{
 DWORD cbSize;
 DWORD flags;
 HCURSOR hCursor;
 POINT ptScreenPos;
} CURSORINFO, *PCURSORINFO, *LPCURSORINFO;
__declspec(dllimport)
BOOL
__stdcall
GetCursorInfo(
 PCURSORINFO pci);
typedef struct tagWINDOWINFO
{
 DWORD cbSize;
 RECT rcWindow;
 RECT rcClient;
 DWORD dwStyle;
 DWORD dwExStyle;
 DWORD dwWindowStatus;
 UINT cxWindowBorders;
 UINT cyWindowBorders;
 ATOM atomWindowType;
 WORD wCreatorVersion;
} WINDOWINFO, *PWINDOWINFO, *LPWINDOWINFO;
__declspec(dllimport)
BOOL
__stdcall
GetWindowInfo(
 HWND hwnd,
 PWINDOWINFO pwi);
typedef struct tagTITLEBARINFO
{
 DWORD cbSize;
 RECT rcTitleBar;
 DWORD rgstate[5 + 1];
} TITLEBARINFO, *PTITLEBARINFO, *LPTITLEBARINFO;
__declspec(dllimport)
BOOL
__stdcall
GetTitleBarInfo(
 HWND hwnd,
 PTITLEBARINFO pti);
typedef struct tagTITLEBARINFOEX
{
 DWORD cbSize;
 RECT rcTitleBar;
 DWORD rgstate[5 + 1];
 RECT rgrect[5 + 1];
} TITLEBARINFOEX, *PTITLEBARINFOEX, *LPTITLEBARINFOEX;
typedef struct tagMENUBARINFO
{
 DWORD cbSize;
 RECT rcBar;
 HMENU hMenu;
 HWND hwndMenu;
 BOOL fBarFocused:1;
 BOOL fFocused:1;
} MENUBARINFO, *PMENUBARINFO, *LPMENUBARINFO;
__declspec(dllimport)
BOOL
__stdcall
GetMenuBarInfo(
 HWND hwnd,
 LONG idObject,
 LONG idItem,
 PMENUBARINFO pmbi);
typedef struct tagSCROLLBARINFO
{
 DWORD cbSize;
 RECT rcScrollBar;
 int dxyLineButton;
 int xyThumbTop;
 int xyThumbBottom;
 int reserved;
 DWORD rgstate[5 + 1];
} SCROLLBARINFO, *PSCROLLBARINFO, *LPSCROLLBARINFO;
__declspec(dllimport)
BOOL
__stdcall
GetScrollBarInfo(
 HWND hwnd,
 LONG idObject,
 PSCROLLBARINFO psbi);
typedef struct tagCOMBOBOXINFO
{
 DWORD cbSize;
 RECT rcItem;
 RECT rcButton;
 DWORD stateButton;
 HWND hwndCombo;
 HWND hwndItem;
 HWND hwndList;
} COMBOBOXINFO, *PCOMBOBOXINFO, *LPCOMBOBOXINFO;
__declspec(dllimport)
BOOL
__stdcall
GetComboBoxInfo(
 HWND hwndCombo,
 PCOMBOBOXINFO pcbi);
__declspec(dllimport)
HWND
__stdcall
GetAncestor(
 HWND hwnd,
 UINT gaFlags);
__declspec(dllimport)
HWND
__stdcall
RealChildWindowFromPoint(
 HWND hwndParent,
 POINT ptParentClientCoords);
__declspec(dllimport)
UINT
__stdcall
RealGetWindowClassA(
 HWND hwnd,
 LPSTR ptszClassName,
 UINT cchClassNameMax);
__declspec(dllimport)
UINT
__stdcall
RealGetWindowClassW(
 HWND hwnd,
 LPWSTR ptszClassName,
 UINT cchClassNameMax);
typedef struct tagALTTABINFO
{
 DWORD cbSize;
 int cItems;
 int cColumns;
 int cRows;
 int iColFocus;
 int iRowFocus;
 int cxItem;
 int cyItem;
 POINT ptStart;
} ALTTABINFO, *PALTTABINFO, *LPALTTABINFO;
__declspec(dllimport)
BOOL
__stdcall
GetAltTabInfoA(
 HWND hwnd,
 int iItem,
 PALTTABINFO pati,
 LPSTR pszItemText,
 UINT cchItemText);
__declspec(dllimport)
BOOL
__stdcall
GetAltTabInfoW(
 HWND hwnd,
 int iItem,
 PALTTABINFO pati,
 LPWSTR pszItemText,
 UINT cchItemText);
__declspec(dllimport)
DWORD
__stdcall
GetListBoxInfo(
 HWND hwnd);
__declspec(dllimport)
BOOL
__stdcall
LockWorkStation(
 void);
__declspec(dllimport)
BOOL
__stdcall
UserHandleGrantAccess(
 HANDLE hUserHandle,
 HANDLE hJob,
 BOOL bGrant);
struct HRAWINPUT__ { int unused; }; typedef struct HRAWINPUT__ *HRAWINPUT;
typedef struct tagRAWINPUTHEADER {
 DWORD dwType;
 DWORD dwSize;
 HANDLE hDevice;
 WPARAM wParam;
} RAWINPUTHEADER, *PRAWINPUTHEADER, *LPRAWINPUTHEADER;
typedef struct tagRAWMOUSE {
 USHORT usFlags;
 union {
 ULONG ulButtons;
 struct {
 USHORT usButtonFlags;
 USHORT usButtonData;
 };
 };
 ULONG ulRawButtons;
 LONG lLastX;
 LONG lLastY;
 ULONG ulExtraInformation;
} RAWMOUSE, *PRAWMOUSE, *LPRAWMOUSE;
typedef struct tagRAWKEYBOARD {
 USHORT MakeCode;
 USHORT Flags;
 USHORT Reserved;
 USHORT VKey;
 UINT Message;
 ULONG ExtraInformation;
} RAWKEYBOARD, *PRAWKEYBOARD, *LPRAWKEYBOARD;
typedef struct tagRAWHID {
 DWORD dwSizeHid;
 DWORD dwCount;
 BYTE bRawData[1];
} RAWHID, *PRAWHID, *LPRAWHID;
typedef struct tagRAWINPUT {
 RAWINPUTHEADER header;
 union {
 RAWMOUSE mouse;
 RAWKEYBOARD keyboard;
 RAWHID hid;
 } data;
} RAWINPUT, *PRAWINPUT, *LPRAWINPUT;
__declspec(dllimport)
UINT
__stdcall
GetRawInputData(
 HRAWINPUT hRawInput,
 UINT uiCommand,
 LPVOID pData,
 PUINT pcbSize,
 UINT cbSizeHeader);
typedef struct tagRID_DEVICE_INFO_MOUSE {
 DWORD dwId;
 DWORD dwNumberOfButtons;
 DWORD dwSampleRate;
 BOOL fHasHorizontalWheel;
} RID_DEVICE_INFO_MOUSE, *PRID_DEVICE_INFO_MOUSE;
typedef struct tagRID_DEVICE_INFO_KEYBOARD {
 DWORD dwType;
 DWORD dwSubType;
 DWORD dwKeyboardMode;
 DWORD dwNumberOfFunctionKeys;
 DWORD dwNumberOfIndicators;
 DWORD dwNumberOfKeysTotal;
} RID_DEVICE_INFO_KEYBOARD, *PRID_DEVICE_INFO_KEYBOARD;
typedef struct tagRID_DEVICE_INFO_HID {
 DWORD dwVendorId;
 DWORD dwProductId;
 DWORD dwVersionNumber;
 USHORT usUsagePage;
 USHORT usUsage;
} RID_DEVICE_INFO_HID, *PRID_DEVICE_INFO_HID;
typedef struct tagRID_DEVICE_INFO {
 DWORD cbSize;
 DWORD dwType;
 union {
 RID_DEVICE_INFO_MOUSE mouse;
 RID_DEVICE_INFO_KEYBOARD keyboard;
 RID_DEVICE_INFO_HID hid;
 };
} RID_DEVICE_INFO, *PRID_DEVICE_INFO, *LPRID_DEVICE_INFO;
__declspec(dllimport)
UINT
__stdcall
GetRawInputDeviceInfoA(
 HANDLE hDevice,
 UINT uiCommand,
 LPVOID pData,
 PUINT pcbSize);
__declspec(dllimport)
UINT
__stdcall
GetRawInputDeviceInfoW(
 HANDLE hDevice,
 UINT uiCommand,
 LPVOID pData,
 PUINT pcbSize);
__declspec(dllimport)
UINT
__stdcall
GetRawInputBuffer(
 PRAWINPUT pData,
 PUINT pcbSize,
 UINT cbSizeHeader);
typedef struct tagRAWINPUTDEVICE {
 USHORT usUsagePage;
 USHORT usUsage;
 DWORD dwFlags;
 HWND hwndTarget;
} RAWINPUTDEVICE, *PRAWINPUTDEVICE, *LPRAWINPUTDEVICE;
typedef const RAWINPUTDEVICE* PCRAWINPUTDEVICE;
__declspec(dllimport)
BOOL
__stdcall
RegisterRawInputDevices(
 PCRAWINPUTDEVICE pRawInputDevices,
 UINT uiNumDevices,
 UINT cbSize);
__declspec(dllimport)
UINT
__stdcall
GetRegisteredRawInputDevices(
 PRAWINPUTDEVICE pRawInputDevices,
 PUINT puiNumDevices,
 UINT cbSize);
typedef struct tagRAWINPUTDEVICELIST {
 HANDLE hDevice;
 DWORD dwType;
} RAWINPUTDEVICELIST, *PRAWINPUTDEVICELIST;
__declspec(dllimport)
UINT
__stdcall
GetRawInputDeviceList(
 PRAWINPUTDEVICELIST pRawInputDeviceList,
 PUINT puiNumDevices,
 UINT cbSize);
__declspec(dllimport)
LRESULT
__stdcall
DefRawInputProc(
 PRAWINPUT* paRawInput,
 INT nInput,
 UINT cbSizeHeader);
__declspec(dllimport)
BOOL
__stdcall
ChangeWindowMessageFilter(
 UINT message,
 DWORD dwFlag);
__declspec(dllimport)
BOOL
__stdcall
ShutdownBlockReasonCreate(
 HWND hWnd,
 LPCWSTR pwszReason);
__declspec(dllimport)
BOOL
__stdcall
ShutdownBlockReasonQuery(
 HWND hWnd,
 LPWSTR pwszBuff,
 DWORD *pcchBuff);
__declspec(dllimport)
BOOL
__stdcall
ShutdownBlockReasonDestroy(
 HWND hWnd);
}
extern "C" {
typedef DWORD LGRPID;
typedef DWORD LCTYPE;
typedef DWORD CALTYPE;
typedef DWORD CALID;
typedef struct _cpinfo {
 UINT MaxCharSize;
 BYTE DefaultChar[2];
 BYTE LeadByte[12];
} CPINFO, *LPCPINFO;
typedef struct _cpinfoexA {
 UINT MaxCharSize;
 BYTE DefaultChar[2];
 BYTE LeadByte[12];
 WCHAR UnicodeDefaultChar;
 UINT CodePage;
 CHAR CodePageName[260];
} CPINFOEXA, *LPCPINFOEXA;
typedef struct _cpinfoexW {
 UINT MaxCharSize;
 BYTE DefaultChar[2];
 BYTE LeadByte[12];
 WCHAR UnicodeDefaultChar;
 UINT CodePage;
 WCHAR CodePageName[260];
} CPINFOEXW, *LPCPINFOEXW;
typedef CPINFOEXA CPINFOEX;
typedef LPCPINFOEXA LPCPINFOEX;
typedef struct _numberfmtA {
 UINT NumDigits;
 UINT LeadingZero;
 UINT Grouping;
 LPSTR lpDecimalSep;
 LPSTR lpThousandSep;
 UINT NegativeOrder;
} NUMBERFMTA, *LPNUMBERFMTA;
typedef struct _numberfmtW {
 UINT NumDigits;
 UINT LeadingZero;
 UINT Grouping;
 LPWSTR lpDecimalSep;
 LPWSTR lpThousandSep;
 UINT NegativeOrder;
} NUMBERFMTW, *LPNUMBERFMTW;
typedef NUMBERFMTA NUMBERFMT;
typedef LPNUMBERFMTA LPNUMBERFMT;
typedef struct _currencyfmtA {
 UINT NumDigits;
 UINT LeadingZero;
 UINT Grouping;
 LPSTR lpDecimalSep;
 LPSTR lpThousandSep;
 UINT NegativeOrder;
 UINT PositiveOrder;
 LPSTR lpCurrencySymbol;
} CURRENCYFMTA, *LPCURRENCYFMTA;
typedef struct _currencyfmtW {
 UINT NumDigits;
 UINT LeadingZero;
 UINT Grouping;
 LPWSTR lpDecimalSep;
 LPWSTR lpThousandSep;
 UINT NegativeOrder;
 UINT PositiveOrder;
 LPWSTR lpCurrencySymbol;
} CURRENCYFMTW, *LPCURRENCYFMTW;
typedef CURRENCYFMTA CURRENCYFMT;
typedef LPCURRENCYFMTA LPCURRENCYFMT;
enum SYSNLS_FUNCTION{
 COMPARE_STRING = 0x0001,
};
typedef DWORD NLS_FUNCTION;
typedef struct _nlsversioninfo{
 DWORD dwNLSVersionInfoSize;
 DWORD dwNLSVersion;
 DWORD dwDefinedVersion;
} NLSVERSIONINFO, *LPNLSVERSIONINFO;
typedef struct _nlsversioninfoex{
 DWORD dwNLSVersionInfoSize;
 DWORD dwNLSVersion;
 DWORD dwDefinedVersion;
 DWORD dwEffectiveId;
 GUID guidCustomVersion;
} NLSVERSIONINFOEX, *LPNLSVERSIONINFOEX;
typedef LONG GEOID;
typedef DWORD GEOTYPE;
typedef DWORD GEOCLASS;
enum SYSGEOTYPE {
 GEO_NATION = 0x0001,
 GEO_LATITUDE = 0x0002,
 GEO_LONGITUDE = 0x0003,
 GEO_ISO2 = 0x0004,
 GEO_ISO3 = 0x0005,
 GEO_RFC1766 = 0x0006,
 GEO_LCID = 0x0007,
 GEO_FRIENDLYNAME= 0x0008,
 GEO_OFFICIALNAME= 0x0009,
 GEO_TIMEZONES = 0x000A,
 GEO_OFFICIALLANGUAGES = 0x000B,
};
enum SYSGEOCLASS {
 GEOCLASS_NATION = 16,
 GEOCLASS_REGION = 14,
};
typedef enum _NORM_FORM {
 NormalizationOther = 0,
 NormalizationC = 0x1,
 NormalizationD = 0x2,
 NormalizationKC = 0x5,
 NormalizationKD = 0x6
} NORM_FORM;
typedef BOOL (__stdcall* LANGUAGEGROUP_ENUMPROCA)(LGRPID, LPSTR, LPSTR, DWORD, LONG_PTR);
typedef BOOL (__stdcall* LANGGROUPLOCALE_ENUMPROCA)(LGRPID, LCID, LPSTR, LONG_PTR);
typedef BOOL (__stdcall* UILANGUAGE_ENUMPROCA)(LPSTR, LONG_PTR);
typedef BOOL (__stdcall* LOCALE_ENUMPROCA)(LPSTR);
typedef BOOL (__stdcall* CODEPAGE_ENUMPROCA)(LPSTR);
typedef BOOL (__stdcall* DATEFMT_ENUMPROCA)(LPSTR);
typedef BOOL (__stdcall* DATEFMT_ENUMPROCEXA)(LPSTR, CALID);
typedef BOOL (__stdcall* TIMEFMT_ENUMPROCA)(LPSTR);
typedef BOOL (__stdcall* CALINFO_ENUMPROCA)(LPSTR);
typedef BOOL (__stdcall* CALINFO_ENUMPROCEXA)(LPSTR, CALID);
typedef BOOL (__stdcall* LANGUAGEGROUP_ENUMPROCW)(LGRPID, LPWSTR, LPWSTR, DWORD, LONG_PTR);
typedef BOOL (__stdcall* LANGGROUPLOCALE_ENUMPROCW)(LGRPID, LCID, LPWSTR, LONG_PTR);
typedef BOOL (__stdcall* UILANGUAGE_ENUMPROCW)(LPWSTR, LONG_PTR);
typedef BOOL (__stdcall* LOCALE_ENUMPROCW)(LPWSTR);
typedef BOOL (__stdcall* CODEPAGE_ENUMPROCW)(LPWSTR);
typedef BOOL (__stdcall* DATEFMT_ENUMPROCW)(LPWSTR);
typedef BOOL (__stdcall* DATEFMT_ENUMPROCEXW)(LPWSTR, CALID);
typedef BOOL (__stdcall* TIMEFMT_ENUMPROCW)(LPWSTR);
typedef BOOL (__stdcall* CALINFO_ENUMPROCW)(LPWSTR);
typedef BOOL (__stdcall* CALINFO_ENUMPROCEXW)(LPWSTR, CALID);
typedef BOOL (__stdcall* GEO_ENUMPROC)(GEOID);
typedef struct _FILEMUIINFO {
 DWORD dwSize;
 DWORD dwVersion;
 DWORD dwFileType;
 BYTE pChecksum[16];
 BYTE pServiceChecksum[16];
 DWORD dwLanguageNameOffset;
 DWORD dwTypeIDMainSize;
 DWORD dwTypeIDMainOffset;
 DWORD dwTypeNameMainOffset;
 DWORD dwTypeIDMUISize;
 DWORD dwTypeIDMUIOffset;
 DWORD dwTypeNameMUIOffset;
 BYTE abBuffer[8];
} FILEMUIINFO, *PFILEMUIINFO;
__declspec(dllimport)
BOOL
__stdcall
IsValidCodePage(
 UINT CodePage);
__declspec(dllimport)
UINT
__stdcall
GetACP(void);
__declspec(dllimport)
UINT
__stdcall
GetOEMCP(void);
__declspec(dllimport)
BOOL
__stdcall
GetCPInfo(
 UINT CodePage,
 LPCPINFO lpCPInfo);
__declspec(dllimport)
BOOL
__stdcall
GetCPInfoExA(
 UINT CodePage,
 DWORD dwFlags,
 LPCPINFOEXA lpCPInfoEx);
__declspec(dllimport)
BOOL
__stdcall
GetCPInfoExW(
 UINT CodePage,
 DWORD dwFlags,
 LPCPINFOEXW lpCPInfoEx);
__declspec(dllimport)
BOOL
__stdcall
IsDBCSLeadByte(
 BYTE TestChar);
__declspec(dllimport)
BOOL
__stdcall
IsDBCSLeadByteEx(
 UINT CodePage,
 BYTE TestChar);
__declspec(dllimport)
int
__stdcall
MultiByteToWideChar(
 UINT CodePage,
 DWORD dwFlags,
 LPCSTR lpMultiByteStr,
 int cbMultiByte,
 LPWSTR lpWideCharStr,
 int cchWideChar);
__declspec(dllimport)
int
__stdcall
WideCharToMultiByte(
 UINT CodePage,
 DWORD dwFlags,
 LPCWSTR lpWideCharStr,
 int cchWideChar,
 LPSTR lpMultiByteStr,
 int cbMultiByte,
 LPCSTR lpDefaultChar,
 LPBOOL lpUsedDefaultChar);
__declspec(dllimport)
int
__stdcall
CompareStringA(
 LCID Locale,
 DWORD dwCmpFlags,
 LPCSTR lpString1,
 int cchCount1,
 LPCSTR lpString2,
 int cchCount2);
__declspec(dllimport)
int
__stdcall
CompareStringW(
 LCID Locale,
 DWORD dwCmpFlags,
 LPCWSTR lpString1,
 int cchCount1,
 LPCWSTR lpString2,
 int cchCount2);
__declspec(dllimport)
int
__stdcall
FindNLSString(
 LCID Locale,
 DWORD dwFindNLSStringFlags,
 LPCWSTR lpStringSource,
 int cchSource,
 LPCWSTR lpStringValue,
 int cchValue,
 LPINT pcchFound);
__declspec(dllimport)
int
__stdcall
LCMapStringA(
 LCID Locale,
 DWORD dwMapFlags,
 LPCSTR lpSrcStr,
 int cchSrc,
 LPSTR lpDestStr,
 int cchDest);
__declspec(dllimport)
int
__stdcall
LCMapStringW(
 LCID Locale,
 DWORD dwMapFlags,
 LPCWSTR lpSrcStr,
 int cchSrc,
 LPWSTR lpDestStr,
 int cchDest);
__declspec(dllimport)
int
__stdcall
GetLocaleInfoA(
 LCID Locale,
 LCTYPE LCType,
 LPSTR lpLCData,
 int cchData);
__declspec(dllimport)
int
__stdcall
GetLocaleInfoW(
 LCID Locale,
 LCTYPE LCType,
 LPWSTR lpLCData,
 int cchData);
__declspec(dllimport)
BOOL
__stdcall
SetLocaleInfoA(
 LCID Locale,
 LCTYPE LCType,
 LPCSTR lpLCData);
__declspec(dllimport)
BOOL
__stdcall
SetLocaleInfoW(
 LCID Locale,
 LCTYPE LCType,
 LPCWSTR lpLCData);
__declspec(dllimport)
int
__stdcall
GetCalendarInfoA(
 LCID Locale,
 CALID Calendar,
 CALTYPE CalType,
 LPSTR lpCalData,
 int cchData,
 LPDWORD lpValue);
__declspec(dllimport)
int
__stdcall
GetCalendarInfoW(
 LCID Locale,
 CALID Calendar,
 CALTYPE CalType,
 LPWSTR lpCalData,
 int cchData,
 LPDWORD lpValue);
__declspec(dllimport)
BOOL
__stdcall
SetCalendarInfoA(
 LCID Locale,
 CALID Calendar,
 CALTYPE CalType,
 LPCSTR lpCalData);
__declspec(dllimport)
BOOL
__stdcall
SetCalendarInfoW(
 LCID Locale,
 CALID Calendar,
 CALTYPE CalType,
 LPCWSTR lpCalData);
__declspec(dllimport)
int
__stdcall
LCIDToLocaleName(
 LCID Locale,
 LPWSTR lpName,
 int cchName,
 DWORD dwFlags);
__declspec(dllimport)
LCID
__stdcall
LocaleNameToLCID(
 LPCWSTR lpName,
 DWORD dwFlags);
__declspec(dllimport)
int
__stdcall
GetTimeFormatA(
 LCID Locale,
 DWORD dwFlags,
 const SYSTEMTIME *lpTime,
 LPCSTR lpFormat,
 LPSTR lpTimeStr,
 int cchTime);
__declspec(dllimport)
int
__stdcall
GetTimeFormatW(
 LCID Locale,
 DWORD dwFlags,
 const SYSTEMTIME *lpTime,
 LPCWSTR lpFormat,
 LPWSTR lpTimeStr,
 int cchTime);
__declspec(dllimport)
int
__stdcall
GetDurationFormat(
 LCID Locale,
 DWORD dwFlags,
 const SYSTEMTIME *lpDuration,
 ULONGLONG ullDuration,
 LPCWSTR lpFormat,
 LPWSTR lpDurationStr,
 int cchDuration);
__declspec(dllimport)
int
__stdcall
GetDateFormatA(
 LCID Locale,
 DWORD dwFlags,
 const SYSTEMTIME *lpDate,
 LPCSTR lpFormat,
 LPSTR lpDateStr,
 int cchDate);
__declspec(dllimport)
int
__stdcall
GetDateFormatW(
 LCID Locale,
 DWORD dwFlags,
 const SYSTEMTIME *lpDate,
 LPCWSTR lpFormat,
 LPWSTR lpDateStr,
 int cchDate);
__declspec(dllimport)
int
__stdcall
GetNumberFormatA(
 LCID Locale,
 DWORD dwFlags,
 LPCSTR lpValue,
 const NUMBERFMTA *lpFormat,
 LPSTR lpNumberStr,
 int cchNumber);
__declspec(dllimport)
int
__stdcall
GetNumberFormatW(
 LCID Locale,
 DWORD dwFlags,
 LPCWSTR lpValue,
 const NUMBERFMTW *lpFormat,
 LPWSTR lpNumberStr,
 int cchNumber);
__declspec(dllimport)
int
__stdcall
GetCurrencyFormatA(
 LCID Locale,
 DWORD dwFlags,
 LPCSTR lpValue,
 const CURRENCYFMTA *lpFormat,
 LPSTR lpCurrencyStr,
 int cchCurrency);
__declspec(dllimport)
int
__stdcall
GetCurrencyFormatW(
 LCID Locale,
 DWORD dwFlags,
 LPCWSTR lpValue,
 const CURRENCYFMTW *lpFormat,
 LPWSTR lpCurrencyStr,
 int cchCurrency);
__declspec(dllimport)
BOOL
__stdcall
EnumCalendarInfoA(
 CALINFO_ENUMPROCA lpCalInfoEnumProc,
 LCID Locale,
 CALID Calendar,
 CALTYPE CalType);
__declspec(dllimport)
BOOL
__stdcall
EnumCalendarInfoW(
 CALINFO_ENUMPROCW lpCalInfoEnumProc,
 LCID Locale,
 CALID Calendar,
 CALTYPE CalType);
__declspec(dllimport)
BOOL
__stdcall
EnumCalendarInfoExA(
 CALINFO_ENUMPROCEXA lpCalInfoEnumProcEx,
 LCID Locale,
 CALID Calendar,
 CALTYPE CalType);
__declspec(dllimport)
BOOL
__stdcall
EnumCalendarInfoExW(
 CALINFO_ENUMPROCEXW lpCalInfoEnumProcEx,
 LCID Locale,
 CALID Calendar,
 CALTYPE CalType);
__declspec(dllimport)
BOOL
__stdcall
EnumTimeFormatsA(
 TIMEFMT_ENUMPROCA lpTimeFmtEnumProc,
 LCID Locale,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
EnumTimeFormatsW(
 TIMEFMT_ENUMPROCW lpTimeFmtEnumProc,
 LCID Locale,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
EnumDateFormatsA(
 DATEFMT_ENUMPROCA lpDateFmtEnumProc,
 LCID Locale,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
EnumDateFormatsW(
 DATEFMT_ENUMPROCW lpDateFmtEnumProc,
 LCID Locale,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
EnumDateFormatsExA(
 DATEFMT_ENUMPROCEXA lpDateFmtEnumProcEx,
 LCID Locale,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
EnumDateFormatsExW(
 DATEFMT_ENUMPROCEXW lpDateFmtEnumProcEx,
 LCID Locale,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
IsValidLanguageGroup(
 LGRPID LanguageGroup,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
GetNLSVersion(
 NLS_FUNCTION Function,
 LCID Locale,
 LPNLSVERSIONINFO lpVersionInformation);
__declspec(dllimport)
BOOL
__stdcall
IsNLSDefinedString(
 NLS_FUNCTION Function,
 DWORD dwFlags,
 LPNLSVERSIONINFO lpVersionInformation,
 LPCWSTR lpString,
 INT cchStr);
__declspec(dllimport)
BOOL
__stdcall
IsValidLocale(
 LCID Locale,
 DWORD dwFlags);
__declspec(dllimport)
int
__stdcall
GetGeoInfoA(
 GEOID Location,
 GEOTYPE GeoType,
 LPSTR lpGeoData,
 int cchData,
 LANGID LangId);
__declspec(dllimport)
int
__stdcall
GetGeoInfoW(
 GEOID Location,
 GEOTYPE GeoType,
 LPWSTR lpGeoData,
 int cchData,
 LANGID LangId);
__declspec(dllimport)
BOOL
__stdcall
EnumSystemGeoID(
 GEOCLASS GeoClass,
 GEOID ParentGeoId,
 GEO_ENUMPROC lpGeoEnumProc);
__declspec(dllimport)
GEOID
__stdcall
GetUserGeoID(
 GEOCLASS GeoClass);
__declspec(dllimport)
BOOL
__stdcall
SetUserGeoID(
 GEOID GeoId);
__declspec(dllimport)
LCID
__stdcall
ConvertDefaultLocale(
 LCID Locale);
__declspec(dllimport)
LCID
__stdcall
GetThreadLocale(void);
__declspec(dllimport)
BOOL
__stdcall
SetThreadLocale(
 LCID Locale
 );
__declspec(dllimport)
LANGID
__stdcall
GetSystemDefaultUILanguage(void);
__declspec(dllimport)
LANGID
__stdcall
GetUserDefaultUILanguage(void);
__declspec(dllimport)
LANGID
__stdcall
GetSystemDefaultLangID(void);
__declspec(dllimport)
LANGID
__stdcall
GetUserDefaultLangID(void);
__declspec(dllimport)
LCID
__stdcall
GetSystemDefaultLCID(void);
__declspec(dllimport)
LCID
__stdcall
GetUserDefaultLCID(void);
__declspec(dllimport)
LANGID
__stdcall
SetThreadUILanguage( LANGID LangId);
__declspec(dllimport)
LANGID
__stdcall
GetThreadUILanguage(void);
__declspec(dllimport)
BOOL
__stdcall
GetUserPreferredUILanguages (
 DWORD dwFlags,
 PULONG pulNumLanguages,
 PWSTR pwszLanguagesBuffer,
 PULONG pcchLanguagesBuffer
);
__declspec(dllimport)
BOOL
__stdcall
GetSystemPreferredUILanguages (
 DWORD dwFlags,
 PULONG pulNumLanguages,
 PWSTR pwszLanguagesBuffer,
 PULONG pcchLanguagesBuffer
);
__declspec(dllimport)
BOOL
__stdcall
GetThreadPreferredUILanguages(
 DWORD dwFlags,
 PULONG pulNumLanguages,
 PWSTR pwszLanguagesBuffer,
 PULONG pcchLanguagesBuffer
);
__declspec(dllimport)
BOOL
__stdcall
SetThreadPreferredUILanguages(
 DWORD dwFlags,
 PCWSTR pwszLanguagesBuffer,
 PULONG pulNumLanguages
);
__declspec(dllimport)
BOOL
__stdcall
GetFileMUIInfo(
 DWORD dwFlags,
 PCWSTR pcwszFilePath,
 PFILEMUIINFO pFileMUIInfo,
 DWORD* pcbFileMUIInfo);
__declspec(dllimport)
BOOL
__stdcall
GetFileMUIPath(
 DWORD dwFlags,
 PCWSTR pcwszFilePath ,
 PWSTR pwszLanguage,
 PULONG pcchLanguage,
 PWSTR pwszFileMUIPath,
 PULONG pcchFileMUIPath,
 PULONGLONG pululEnumerator
);
__declspec(dllimport)
BOOL
__stdcall
GetUILanguageInfo(
 DWORD dwFlags,
 PCWSTR pwmszLanguage,
 PWSTR pwszFallbackLanguages,
 PDWORD pcchFallbackLanguages,
 PDWORD pAttributes
);
__declspec(dllimport)
BOOL
__stdcall
NotifyUILanguageChange(
 DWORD dwFlags,
 PCWSTR pcwstrNewLanguage,
 PCWSTR pcwstrPreviousLanguage,
 DWORD dwReserved,
 PDWORD pdwStatusRtrn
);
__declspec(dllimport)
BOOL
__stdcall
GetStringTypeExA(
 LCID Locale,
 DWORD dwInfoType,
 LPCSTR lpSrcStr,
 int cchSrc,
 LPWORD lpCharType);
__declspec(dllimport)
BOOL
__stdcall
GetStringTypeExW(
 LCID Locale,
 DWORD dwInfoType,
 LPCWSTR lpSrcStr,
 int cchSrc,
 LPWORD lpCharType);
__declspec(dllimport)
BOOL
__stdcall
GetStringTypeA(
 LCID Locale,
 DWORD dwInfoType,
 LPCSTR lpSrcStr,
 int cchSrc,
 LPWORD lpCharType);
__declspec(dllimport)
BOOL
__stdcall
GetStringTypeW(
 DWORD dwInfoType,
 LPCWSTR lpSrcStr,
 int cchSrc,
 LPWORD lpCharType);
__declspec(dllimport)
int
__stdcall
FoldStringA(
 DWORD dwMapFlags,
 LPCSTR lpSrcStr,
 int cchSrc,
 LPSTR lpDestStr,
 int cchDest);
__declspec(dllimport)
int
__stdcall
FoldStringW(
 DWORD dwMapFlags,
 LPCWSTR lpSrcStr,
 int cchSrc,
 LPWSTR lpDestStr,
 int cchDest);
__declspec(dllimport)
BOOL
__stdcall
EnumSystemLanguageGroupsA(
 LANGUAGEGROUP_ENUMPROCA lpLanguageGroupEnumProc,
 DWORD dwFlags,
 LONG_PTR lParam);
__declspec(dllimport)
BOOL
__stdcall
EnumSystemLanguageGroupsW(
 LANGUAGEGROUP_ENUMPROCW lpLanguageGroupEnumProc,
 DWORD dwFlags,
 LONG_PTR lParam);
__declspec(dllimport)
BOOL
__stdcall
EnumLanguageGroupLocalesA(
 LANGGROUPLOCALE_ENUMPROCA lpLangGroupLocaleEnumProc,
 LGRPID LanguageGroup,
 DWORD dwFlags,
 LONG_PTR lParam);
__declspec(dllimport)
BOOL
__stdcall
EnumLanguageGroupLocalesW(
 LANGGROUPLOCALE_ENUMPROCW lpLangGroupLocaleEnumProc,
 LGRPID LanguageGroup,
 DWORD dwFlags,
 LONG_PTR lParam);
__declspec(dllimport)
BOOL
__stdcall
EnumUILanguagesA(
 UILANGUAGE_ENUMPROCA lpUILanguageEnumProc,
 DWORD dwFlags,
 LONG_PTR lParam);
__declspec(dllimport)
BOOL
__stdcall
EnumUILanguagesW(
 UILANGUAGE_ENUMPROCW lpUILanguageEnumProc,
 DWORD dwFlags,
 LONG_PTR lParam);
__declspec(dllimport)
BOOL
__stdcall
EnumSystemLocalesA(
 LOCALE_ENUMPROCA lpLocaleEnumProc,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
EnumSystemLocalesW(
 LOCALE_ENUMPROCW lpLocaleEnumProc,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
EnumSystemCodePagesA(
 CODEPAGE_ENUMPROCA lpCodePageEnumProc,
 DWORD dwFlags);
__declspec(dllimport)
BOOL
__stdcall
EnumSystemCodePagesW(
 CODEPAGE_ENUMPROCW lpCodePageEnumProc,
 DWORD dwFlags);
__declspec(dllimport)
int
__stdcall NormalizeString( NORM_FORM NormForm,
 LPCWSTR lpSrcString,
 int cwSrcLength,
 LPWSTR lpDstString,
 int cwDstLength );
__declspec(dllimport)
BOOL
__stdcall IsNormalizedString( NORM_FORM NormForm,
 LPCWSTR lpString,
 int cwLength );
__declspec(dllimport)
int
__stdcall IdnToAscii( DWORD dwFlags,
 LPCWSTR lpUnicodeCharStr,
 int cchUnicodeChar,
 LPWSTR lpASCIICharStr,
 int cchASCIIChar);
__declspec(dllimport)
int
__stdcall IdnToNameprepUnicode( DWORD dwFlags,
 LPCWSTR lpUnicodeCharStr,
 int cchUnicodeChar,
 LPWSTR lpNameprepCharStr,
 int cchNameprepChar);
__declspec(dllimport)
int
__stdcall IdnToUnicode( DWORD dwFlags,
 LPCWSTR lpASCIICharStr,
 int cchASCIIChar,
 LPWSTR lpUnicodeCharStr,
 int cchUnicodeChar);
__declspec(dllimport)
BOOL
__stdcall VerifyScripts(
 DWORD dwFlags,
 LPCWSTR lpLocaleScripts,
 int cchLocaleScripts,
 LPCWSTR lpTestScripts,
 int cchTestScripts);
__declspec(dllimport)
int
__stdcall GetStringScripts(
 DWORD dwFlags,
 LPCWSTR lpString,
 int cchString,
 LPWSTR lpScripts,
 int cchScripts);
__declspec(dllimport)
int
__stdcall
GetLocaleInfoEx(
 LPCWSTR lpLocaleName,
 LCTYPE LCType,
 LPWSTR lpLCData,
 int cchData
);
__declspec(dllimport)
int
__stdcall
GetCalendarInfoEx(
 LPCWSTR lpLocaleName,
 CALID Calendar,
 LPCWSTR lpReserved,
 CALTYPE CalType,
 LPWSTR lpCalData,
 int cchData,
 LPDWORD lpValue
);
__declspec(dllimport)
int
__stdcall
GetTimeFormatEx(
 LPCWSTR lpLocaleName,
 DWORD dwFlags,
 const SYSTEMTIME *lpTime,
 LPCWSTR lpFormat,
 LPWSTR lpTimeStr,
 int cchTime
);
__declspec(dllimport)
int
__stdcall
GetDateFormatEx(
 LPCWSTR lpLocaleName,
 DWORD dwFlags,
 const SYSTEMTIME *lpDate,
 LPCWSTR lpFormat,
 LPWSTR lpDateStr,
 int cchDate,
 LPCWSTR lpCalendar
);
__declspec(dllimport)
int
__stdcall
GetDurationFormatEx(
 LPCWSTR lpLocaleName,
 DWORD dwFlags,
 const SYSTEMTIME *lpDuration,
 ULONGLONG ullDuration,
 LPCWSTR lpFormat,
 LPWSTR lpDurationStr,
 int cchDuration
);
__declspec(dllimport)
int
__stdcall
GetNumberFormatEx(
 LPCWSTR lpLocaleName,
 DWORD dwFlags,
 LPCWSTR lpValue,
 const NUMBERFMTW *lpFormat,
 LPWSTR lpNumberStr,
 int cchNumber
);
__declspec(dllimport)
int
__stdcall
GetCurrencyFormatEx(
 LPCWSTR lpLocaleName,
 DWORD dwFlags,
 LPCWSTR lpValue,
 const CURRENCYFMTW *lpFormat,
 LPWSTR lpCurrencyStr,
 int cchCurrency
);
__declspec(dllimport)
int
__stdcall
GetUserDefaultLocaleName(
 LPWSTR lpLocaleName,
 int cchLocaleName
);
__declspec(dllimport)
int
__stdcall
GetSystemDefaultLocaleName(
 LPWSTR lpLocaleName,
 int cchLocaleName
);
__declspec(dllimport)
BOOL
__stdcall
GetNLSVersionEx(
 NLS_FUNCTION function,
 LPCWSTR lpLocaleName,
 LPNLSVERSIONINFOEX lpVersionInformation
);
__declspec(dllimport)
int
__stdcall
CompareStringEx(
 LPCWSTR lpLocaleName,
 DWORD dwCmpFlags,
 LPCWSTR lpString1,
 int cchCount1,
 LPCWSTR lpString2,
 int cchCount2,
 LPNLSVERSIONINFO lpVersionInformation,
 LPVOID lpReserved,
 LPARAM lParam
);
__declspec(dllimport)
int
__stdcall
FindNLSStringEx(
 LPCWSTR lpLocaleName,
 DWORD dwFindNLSStringFlags,
 LPCWSTR lpStringSource,
 int cchSource,
 LPCWSTR lpStringValue,
 int cchValue,
 LPINT pcchFound,
 LPNLSVERSIONINFO lpVersionInformation,
 LPVOID lpReserved,
 LPARAM lParam
);
__declspec(dllimport)
int
__stdcall
LCMapStringEx(
 LPCWSTR lpLocaleName,
 DWORD dwMapFlags,
 LPCWSTR lpSrcStr,
 int cchSrc,
 LPWSTR lpDestStr,
 int cchDest,
 LPNLSVERSIONINFO lpVersionInformation,
 LPVOID lpReserved,
 LPARAM lParam
);
__declspec(dllimport)
int
__stdcall
CompareStringOrdinal(
 LPCWSTR lpString1,
 int cchCount1,
 LPCWSTR lpString2,
 int cchCount2,
 BOOL bIgnoreCase
);
__declspec(dllimport)
BOOL
__stdcall
IsValidLocaleName(
 LPCWSTR lpLocaleName
);
typedef BOOL (__stdcall* CALINFO_ENUMPROCEXEX)(LPWSTR, CALID, LPWSTR, LPARAM);
__declspec(dllimport)
BOOL
__stdcall
EnumCalendarInfoExEx(
 CALINFO_ENUMPROCEXEX pCalInfoEnumProcExEx,
 LPCWSTR lpLocaleName,
 CALID Calendar,
 LPCWSTR lpReserved,
 CALTYPE CalType,
 LPARAM lParam
);
typedef BOOL (__stdcall* DATEFMT_ENUMPROCEXEX)(LPWSTR, CALID, LPARAM);
__declspec(dllimport)
BOOL
__stdcall
EnumDateFormatsExEx(
 DATEFMT_ENUMPROCEXEX lpDateFmtEnumProcExEx,
 LPCWSTR lpLocaleName,
 DWORD dwFlags,
 LPARAM lParam
);
typedef BOOL (__stdcall* TIMEFMT_ENUMPROCEX)(LPWSTR, LPARAM);
__declspec(dllimport)
BOOL
__stdcall
EnumTimeFormatsEx(
 TIMEFMT_ENUMPROCEX lpTimeFmtEnumProcEx,
 LPCWSTR lpLocaleName,
 DWORD dwFlags,
 LPARAM lParam
);
typedef BOOL (__stdcall* LOCALE_ENUMPROCEX)(LPWSTR, DWORD, LPARAM);
__declspec(dllimport)
BOOL
__stdcall
EnumSystemLocalesEx(
 LOCALE_ENUMPROCEX lpLocaleEnumProcEx,
 DWORD dwFlags,
 LPARAM lParam,
 LPVOID lpReserved
);
}
#pragma once
extern "C" {
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
typedef struct _CONSOLE_SCREEN_BUFFER_INFOEX {
 ULONG cbSize;
 COORD dwSize;
 COORD dwCursorPosition;
 WORD wAttributes;
 SMALL_RECT srWindow;
 COORD dwMaximumWindowSize;
 WORD wPopupAttributes;
 BOOL bFullscreenSupported;
 COLORREF ColorTable[16];
} CONSOLE_SCREEN_BUFFER_INFOEX, *PCONSOLE_SCREEN_BUFFER_INFOEX;
typedef struct _CONSOLE_CURSOR_INFO {
 DWORD dwSize;
 BOOL bVisible;
} CONSOLE_CURSOR_INFO, *PCONSOLE_CURSOR_INFO;
typedef struct _CONSOLE_FONT_INFO {
 DWORD nFont;
 COORD dwFontSize;
} CONSOLE_FONT_INFO, *PCONSOLE_FONT_INFO;
typedef struct _CONSOLE_FONT_INFOEX {
 ULONG cbSize;
 DWORD nFont;
 COORD dwFontSize;
 UINT FontFamily;
 UINT FontWeight;
 WCHAR FaceName[32];
} CONSOLE_FONT_INFOEX, *PCONSOLE_FONT_INFOEX;
typedef struct _CONSOLE_HISTORY_INFO {
 UINT cbSize;
 UINT HistoryBufferSize;
 UINT NumberOfHistoryBuffers;
 DWORD dwFlags;
} CONSOLE_HISTORY_INFO, *PCONSOLE_HISTORY_INFO;
typedef struct _CONSOLE_SELECTION_INFO {
 DWORD dwFlags;
 COORD dwSelectionAnchor;
 SMALL_RECT srSelection;
} CONSOLE_SELECTION_INFO, *PCONSOLE_SELECTION_INFO;
typedef
BOOL
(__stdcall *PHANDLER_ROUTINE)(
 DWORD CtrlType
 );
__declspec(dllimport)
BOOL
__stdcall
PeekConsoleInputA(
 HANDLE hConsoleInput,
 PINPUT_RECORD lpBuffer,
 DWORD nLength,
 LPDWORD lpNumberOfEventsRead
 );
__declspec(dllimport)
BOOL
__stdcall
PeekConsoleInputW(
 HANDLE hConsoleInput,
 PINPUT_RECORD lpBuffer,
 DWORD nLength,
 LPDWORD lpNumberOfEventsRead
 );
__declspec(dllimport)
BOOL
__stdcall
ReadConsoleInputA(
 HANDLE hConsoleInput,
 PINPUT_RECORD lpBuffer,
 DWORD nLength,
 LPDWORD lpNumberOfEventsRead
 );
__declspec(dllimport)
BOOL
__stdcall
ReadConsoleInputW(
 HANDLE hConsoleInput,
 PINPUT_RECORD lpBuffer,
 DWORD nLength,
 LPDWORD lpNumberOfEventsRead
 );
__declspec(dllimport)
BOOL
__stdcall
WriteConsoleInputA(
 HANDLE hConsoleInput,
 const INPUT_RECORD *lpBuffer,
 DWORD nLength,
 LPDWORD lpNumberOfEventsWritten
 );
__declspec(dllimport)
BOOL
__stdcall
WriteConsoleInputW(
 HANDLE hConsoleInput,
 const INPUT_RECORD *lpBuffer,
 DWORD nLength,
 LPDWORD lpNumberOfEventsWritten
 );
__declspec(dllimport)
BOOL
__stdcall
ReadConsoleOutputA(
 HANDLE hConsoleOutput,
 PCHAR_INFO lpBuffer,
 COORD dwBufferSize,
 COORD dwBufferCoord,
 PSMALL_RECT lpReadRegion
 );
__declspec(dllimport)
BOOL
__stdcall
ReadConsoleOutputW(
 HANDLE hConsoleOutput,
 PCHAR_INFO lpBuffer,
 COORD dwBufferSize,
 COORD dwBufferCoord,
 PSMALL_RECT lpReadRegion
 );
__declspec(dllimport)
BOOL
__stdcall
WriteConsoleOutputA(
 HANDLE hConsoleOutput,
 const CHAR_INFO *lpBuffer,
 COORD dwBufferSize,
 COORD dwBufferCoord,
 PSMALL_RECT lpWriteRegion
 );
__declspec(dllimport)
BOOL
__stdcall
WriteConsoleOutputW(
 HANDLE hConsoleOutput,
 const CHAR_INFO *lpBuffer,
 COORD dwBufferSize,
 COORD dwBufferCoord,
 PSMALL_RECT lpWriteRegion
 );
__declspec(dllimport)
BOOL
__stdcall
ReadConsoleOutputCharacterA(
 HANDLE hConsoleOutput,
 LPSTR lpCharacter,
 DWORD nLength,
 COORD dwReadCoord,
 LPDWORD lpNumberOfCharsRead
 );
__declspec(dllimport)
BOOL
__stdcall
ReadConsoleOutputCharacterW(
 HANDLE hConsoleOutput,
 LPWSTR lpCharacter,
 DWORD nLength,
 COORD dwReadCoord,
 LPDWORD lpNumberOfCharsRead
 );
__declspec(dllimport)
BOOL
__stdcall
ReadConsoleOutputAttribute(
 HANDLE hConsoleOutput,
 LPWORD lpAttribute,
 DWORD nLength,
 COORD dwReadCoord,
 LPDWORD lpNumberOfAttrsRead
 );
__declspec(dllimport)
BOOL
__stdcall
WriteConsoleOutputCharacterA(
 HANDLE hConsoleOutput,
 LPCSTR lpCharacter,
 DWORD nLength,
 COORD dwWriteCoord,
 LPDWORD lpNumberOfCharsWritten
 );
__declspec(dllimport)
BOOL
__stdcall
WriteConsoleOutputCharacterW(
 HANDLE hConsoleOutput,
 LPCWSTR lpCharacter,
 DWORD nLength,
 COORD dwWriteCoord,
 LPDWORD lpNumberOfCharsWritten
 );
__declspec(dllimport)
BOOL
__stdcall
WriteConsoleOutputAttribute(
 HANDLE hConsoleOutput,
 const WORD *lpAttribute,
 DWORD nLength,
 COORD dwWriteCoord,
 LPDWORD lpNumberOfAttrsWritten
 );
__declspec(dllimport)
BOOL
__stdcall
FillConsoleOutputCharacterA(
 HANDLE hConsoleOutput,
 CHAR cCharacter,
 DWORD nLength,
 COORD dwWriteCoord,
 LPDWORD lpNumberOfCharsWritten
 );
__declspec(dllimport)
BOOL
__stdcall
FillConsoleOutputCharacterW(
 HANDLE hConsoleOutput,
 WCHAR cCharacter,
 DWORD nLength,
 COORD dwWriteCoord,
 LPDWORD lpNumberOfCharsWritten
 );
__declspec(dllimport)
BOOL
__stdcall
FillConsoleOutputAttribute(
 HANDLE hConsoleOutput,
 WORD wAttribute,
 DWORD nLength,
 COORD dwWriteCoord,
 LPDWORD lpNumberOfAttrsWritten
 );
__declspec(dllimport)
BOOL
__stdcall
GetConsoleMode(
 HANDLE hConsoleHandle,
 LPDWORD lpMode
 );
__declspec(dllimport)
BOOL
__stdcall
GetNumberOfConsoleInputEvents(
 HANDLE hConsoleInput,
 LPDWORD lpNumberOfEvents
 );
__declspec(dllimport)
BOOL
__stdcall
GetConsoleScreenBufferInfo(
 HANDLE hConsoleOutput,
 PCONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo
 );
__declspec(dllimport)
BOOL
__stdcall
GetConsoleScreenBufferInfoEx(
 HANDLE hConsoleOutput,
 PCONSOLE_SCREEN_BUFFER_INFOEX lpConsoleScreenBufferInfoEx);
__declspec(dllimport)
BOOL
__stdcall
SetConsoleScreenBufferInfoEx(
 HANDLE hConsoleOutput,
 PCONSOLE_SCREEN_BUFFER_INFOEX lpConsoleScreenBufferInfoEx);
__declspec(dllimport)
COORD
__stdcall
GetLargestConsoleWindowSize(
 HANDLE hConsoleOutput
 );
__declspec(dllimport)
BOOL
__stdcall
GetConsoleCursorInfo(
 HANDLE hConsoleOutput,
 PCONSOLE_CURSOR_INFO lpConsoleCursorInfo
 );
__declspec(dllimport)
BOOL
__stdcall
GetCurrentConsoleFont(
 HANDLE hConsoleOutput,
 BOOL bMaximumWindow,
 PCONSOLE_FONT_INFO lpConsoleCurrentFont
 );
__declspec(dllimport)
BOOL
__stdcall
GetCurrentConsoleFontEx(
 HANDLE hConsoleOutput,
 BOOL bMaximumWindow,
 PCONSOLE_FONT_INFOEX lpConsoleCurrentFontEx);
__declspec(dllimport)
BOOL
__stdcall
SetCurrentConsoleFontEx(
 HANDLE hConsoleOutput,
 BOOL bMaximumWindow,
 PCONSOLE_FONT_INFOEX lpConsoleCurrentFontEx);
__declspec(dllimport)
BOOL
__stdcall
GetConsoleHistoryInfo(
 PCONSOLE_HISTORY_INFO lpConsoleHistoryInfo);
__declspec(dllimport)
BOOL
__stdcall
SetConsoleHistoryInfo(
 PCONSOLE_HISTORY_INFO lpConsoleHistoryInfo);
__declspec(dllimport)
COORD
__stdcall
GetConsoleFontSize(
 HANDLE hConsoleOutput,
 DWORD nFont
 );
__declspec(dllimport)
BOOL
__stdcall
GetConsoleSelectionInfo(
 PCONSOLE_SELECTION_INFO lpConsoleSelectionInfo
 );
__declspec(dllimport)
BOOL
__stdcall
GetNumberOfConsoleMouseButtons(
 LPDWORD lpNumberOfMouseButtons
 );
__declspec(dllimport)
BOOL
__stdcall
SetConsoleMode(
 HANDLE hConsoleHandle,
 DWORD dwMode
 );
__declspec(dllimport)
BOOL
__stdcall
SetConsoleActiveScreenBuffer(
 HANDLE hConsoleOutput
 );
__declspec(dllimport)
BOOL
__stdcall
FlushConsoleInputBuffer(
 HANDLE hConsoleInput
 );
__declspec(dllimport)
BOOL
__stdcall
SetConsoleScreenBufferSize(
 HANDLE hConsoleOutput,
 COORD dwSize
 );
__declspec(dllimport)
BOOL
__stdcall
SetConsoleCursorPosition(
 HANDLE hConsoleOutput,
 COORD dwCursorPosition
 );
__declspec(dllimport)
BOOL
__stdcall
SetConsoleCursorInfo(
 HANDLE hConsoleOutput,
 const CONSOLE_CURSOR_INFO *lpConsoleCursorInfo
 );
__declspec(dllimport)
BOOL
__stdcall
ScrollConsoleScreenBufferA(
 HANDLE hConsoleOutput,
 const SMALL_RECT *lpScrollRectangle,
 const SMALL_RECT *lpClipRectangle,
 COORD dwDestinationOrigin,
 const CHAR_INFO *lpFill
 );
__declspec(dllimport)
BOOL
__stdcall
ScrollConsoleScreenBufferW(
 HANDLE hConsoleOutput,
 const SMALL_RECT *lpScrollRectangle,
 const SMALL_RECT *lpClipRectangle,
 COORD dwDestinationOrigin,
 const CHAR_INFO *lpFill
 );
__declspec(dllimport)
BOOL
__stdcall
SetConsoleWindowInfo(
 HANDLE hConsoleOutput,
 BOOL bAbsolute,
 const SMALL_RECT *lpConsoleWindow
 );
__declspec(dllimport)
BOOL
__stdcall
SetConsoleTextAttribute(
 HANDLE hConsoleOutput,
 WORD wAttributes
 );
__declspec(dllimport)
BOOL
__stdcall
SetConsoleCtrlHandler(
 PHANDLER_ROUTINE HandlerRoutine,
 BOOL Add);
__declspec(dllimport)
BOOL
__stdcall
GenerateConsoleCtrlEvent(
 DWORD dwCtrlEvent,
 DWORD dwProcessGroupId
 );
__declspec(dllimport)
BOOL
__stdcall
AllocConsole( void );
__declspec(dllimport)
BOOL
__stdcall
FreeConsole( void );
__declspec(dllimport)
BOOL
__stdcall
AttachConsole(
 DWORD dwProcessId
 );
__declspec(dllimport)
DWORD
__stdcall
GetConsoleTitleA(
 LPSTR lpConsoleTitle,
 DWORD nSize
 );
__declspec(dllimport)
DWORD
__stdcall
GetConsoleTitleW(
 LPWSTR lpConsoleTitle,
 DWORD nSize
 );
__declspec(dllimport)
DWORD
__stdcall
GetConsoleOriginalTitleA(
 LPSTR lpConsoleTitle,
 DWORD nSize);
__declspec(dllimport)
DWORD
__stdcall
GetConsoleOriginalTitleW(
 LPWSTR lpConsoleTitle,
 DWORD nSize);
__declspec(dllimport)
BOOL
__stdcall
SetConsoleTitleA(
 LPCSTR lpConsoleTitle
 );
__declspec(dllimport)
BOOL
__stdcall
SetConsoleTitleW(
 LPCWSTR lpConsoleTitle
 );
typedef struct _CONSOLE_READCONSOLE_CONTROL {
 ULONG nLength;
 ULONG nInitialChars;
 ULONG dwCtrlWakeupMask;
 ULONG dwControlKeyState;
} CONSOLE_READCONSOLE_CONTROL, *PCONSOLE_READCONSOLE_CONTROL;
__declspec(dllimport)
BOOL
__stdcall
ReadConsoleA(
 HANDLE hConsoleInput,
 LPVOID lpBuffer,
 DWORD nNumberOfCharsToRead,
 LPDWORD lpNumberOfCharsRead,
 PCONSOLE_READCONSOLE_CONTROL pInputControl
 );
__declspec(dllimport)
BOOL
__stdcall
ReadConsoleW(
 HANDLE hConsoleInput,
 LPVOID lpBuffer,
 DWORD nNumberOfCharsToRead,
 LPDWORD lpNumberOfCharsRead,
 PCONSOLE_READCONSOLE_CONTROL pInputControl
 );
__declspec(dllimport)
BOOL
__stdcall
WriteConsoleA(
 HANDLE hConsoleOutput,
 const void *lpBuffer,
 DWORD nNumberOfCharsToWrite,
 LPDWORD lpNumberOfCharsWritten,
 LPVOID lpReserved
 );
__declspec(dllimport)
BOOL
__stdcall
WriteConsoleW(
 HANDLE hConsoleOutput,
 const void *lpBuffer,
 DWORD nNumberOfCharsToWrite,
 LPDWORD lpNumberOfCharsWritten,
 LPVOID lpReserved
 );
__declspec(dllimport)
HANDLE
__stdcall
CreateConsoleScreenBuffer(
 DWORD dwDesiredAccess,
 DWORD dwShareMode,
 const SECURITY_ATTRIBUTES *lpSecurityAttributes,
 DWORD dwFlags,
 LPVOID lpScreenBufferData
 );
__declspec(dllimport)
UINT
__stdcall
GetConsoleCP( void );
__declspec(dllimport)
BOOL
__stdcall
SetConsoleCP(
 UINT wCodePageID
 );
__declspec(dllimport)
UINT
__stdcall
GetConsoleOutputCP( void );
__declspec(dllimport)
BOOL
__stdcall
SetConsoleOutputCP(
 UINT wCodePageID
 );
__declspec(dllimport)
BOOL
__stdcall
GetConsoleDisplayMode(
 LPDWORD lpModeFlags);
BOOL
__stdcall
SetConsoleDisplayMode(
 HANDLE hConsoleOutput,
 DWORD dwFlags,
 PCOORD lpNewScreenBufferDimensions);
__declspec(dllimport)
HWND
__stdcall
GetConsoleWindow(
 void
 );
__declspec(dllimport)
DWORD
__stdcall
GetConsoleProcessList(
 LPDWORD lpdwProcessList,
 DWORD dwProcessCount);
__declspec(dllimport)
BOOL
__stdcall
AddConsoleAliasA(
 LPSTR Source,
 LPSTR Target,
 LPSTR ExeName);
__declspec(dllimport)
BOOL
__stdcall
AddConsoleAliasW(
 LPWSTR Source,
 LPWSTR Target,
 LPWSTR ExeName);
__declspec(dllimport)
DWORD
__stdcall
GetConsoleAliasA(
 LPSTR Source,
 LPSTR TargetBuffer,
 DWORD TargetBufferLength,
 LPSTR ExeName);
__declspec(dllimport)
DWORD
__stdcall
GetConsoleAliasW(
 LPWSTR Source,
 LPWSTR TargetBuffer,
 DWORD TargetBufferLength,
 LPWSTR ExeName);
__declspec(dllimport)
DWORD
__stdcall
GetConsoleAliasesLengthA(
 LPSTR ExeName);
__declspec(dllimport)
DWORD
__stdcall
GetConsoleAliasesLengthW(
 LPWSTR ExeName);
__declspec(dllimport)
DWORD
__stdcall
GetConsoleAliasExesLengthA(
 void);
__declspec(dllimport)
DWORD
__stdcall
GetConsoleAliasExesLengthW(
 void);
__declspec(dllimport)
DWORD
__stdcall
GetConsoleAliasesA(
 LPSTR AliasBuffer,
 DWORD AliasBufferLength,
 LPSTR ExeName);
__declspec(dllimport)
DWORD
__stdcall
GetConsoleAliasesW(
 LPWSTR AliasBuffer,
 DWORD AliasBufferLength,
 LPWSTR ExeName);
__declspec(dllimport)
DWORD
__stdcall
GetConsoleAliasExesA(
 LPSTR ExeNameBuffer,
 DWORD ExeNameBufferLength);
__declspec(dllimport)
DWORD
__stdcall
GetConsoleAliasExesW(
 LPWSTR ExeNameBuffer,
 DWORD ExeNameBufferLength);
}
extern "C" {
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
DWORD
__stdcall
VerFindFileA(
 DWORD uFlags,
 LPCSTR szFileName,
 LPCSTR szWinDir,
 LPCSTR szAppDir,
 LPSTR szCurDir,
 PUINT lpuCurDirLen,
 LPSTR szDestDir,
 PUINT lpuDestDirLen
 );
DWORD
__stdcall
VerFindFileW(
 DWORD uFlags,
 LPCWSTR szFileName,
 LPCWSTR szWinDir,
 LPCWSTR szAppDir,
 LPWSTR szCurDir,
 PUINT lpuCurDirLen,
 LPWSTR szDestDir,
 PUINT lpuDestDirLen
 );
DWORD
__stdcall
VerInstallFileA(
 DWORD uFlags,
 LPCSTR szSrcFileName,
 LPCSTR szDestFileName,
 LPCSTR szSrcDir,
 LPCSTR szDestDir,
 LPCSTR szCurDir,
 LPSTR szTmpFile,
 PUINT lpuTmpFileLen
 );
DWORD
__stdcall
VerInstallFileW(
 DWORD uFlags,
 LPCWSTR szSrcFileName,
 LPCWSTR szDestFileName,
 LPCWSTR szSrcDir,
 LPCWSTR szDestDir,
 LPCWSTR szCurDir,
 LPWSTR szTmpFile,
 PUINT lpuTmpFileLen
 );
DWORD
__stdcall
GetFileVersionInfoSizeA(
 LPCSTR lptstrFilename,
 LPDWORD lpdwHandle
 );
DWORD
__stdcall
GetFileVersionInfoSizeW(
 LPCWSTR lptstrFilename,
 LPDWORD lpdwHandle
 );
BOOL
__stdcall
GetFileVersionInfoA(
 LPCSTR lptstrFilename,
 DWORD dwHandle,
 DWORD dwLen,
 LPVOID lpData
 );
BOOL
__stdcall
GetFileVersionInfoW(
 LPCWSTR lptstrFilename,
 DWORD dwHandle,
 DWORD dwLen,
 LPVOID lpData
 );
DWORD __stdcall GetFileVersionInfoSizeExA( DWORD dwFlags, LPCSTR lpwstrFilename, LPDWORD lpdwHandle);
DWORD __stdcall GetFileVersionInfoSizeExW( DWORD dwFlags, LPCWSTR lpwstrFilename, LPDWORD lpdwHandle);
BOOL __stdcall GetFileVersionInfoExA( DWORD dwFlags,
 LPCSTR lpwstrFilename,
 DWORD dwHandle,
 DWORD dwLen,
 LPVOID lpData);
BOOL __stdcall GetFileVersionInfoExW( DWORD dwFlags,
 LPCWSTR lpwstrFilename,
 DWORD dwHandle,
 DWORD dwLen,
 LPVOID lpData);
DWORD
__stdcall
VerLanguageNameA(
 DWORD wLang,
 LPSTR szLang,
 DWORD cchLang
 );
DWORD
__stdcall
VerLanguageNameW(
 DWORD wLang,
 LPWSTR szLang,
 DWORD cchLang
 );
BOOL
__stdcall
VerQueryValueA(
 LPCVOID pBlock,
 LPCSTR lpSubBlock,
 LPVOID * lplpBuffer,
 PUINT puLen
 );
BOOL
__stdcall
VerQueryValueW(
 LPCVOID pBlock,
 LPCWSTR lpSubBlock,
 LPVOID * lplpBuffer,
 PUINT puLen
 );
}
extern "C" {
typedef ACCESS_MASK REGSAM;
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
typedef
DWORD _cdecl
QUERYHANDLER (LPVOID keycontext, PVALCONTEXT val_list, DWORD num_vals,
 LPVOID outputbuffer, DWORD *total_outlen, DWORD input_blen);
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
 DWORD_PTR ve_valueptr;
 DWORD ve_type;
}VALENTA, *PVALENTA;
typedef struct value_entW {
 LPWSTR ve_valuename;
 DWORD ve_valuelen;
 DWORD_PTR ve_valueptr;
 DWORD ve_type;
}VALENTW, *PVALENTW;
typedef VALENTA VALENT;
typedef PVALENTA PVALENT;
typedef LONG LSTATUS;
__declspec(dllimport)
LSTATUS
__stdcall
RegCloseKey (
 HKEY hKey
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegOverridePredefKey (
 HKEY hKey,
 HKEY hNewHKey
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegOpenUserClassesRoot(
 HANDLE hToken,
 DWORD dwOptions,
 REGSAM samDesired,
 PHKEY phkResult
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegOpenCurrentUser(
 REGSAM samDesired,
 PHKEY phkResult
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegDisablePredefinedCache(
 void
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegDisablePredefinedCacheEx(
 void
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegConnectRegistryA (
 LPCSTR lpMachineName,
 HKEY hKey,
 PHKEY phkResult
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegConnectRegistryW (
 LPCWSTR lpMachineName,
 HKEY hKey,
 PHKEY phkResult
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegConnectRegistryExA (
 LPCSTR lpMachineName,
 HKEY hKey,
 ULONG Flags,
 PHKEY phkResult
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegConnectRegistryExW (
 LPCWSTR lpMachineName,
 HKEY hKey,
 ULONG Flags,
 PHKEY phkResult
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegCreateKeyA (
 HKEY hKey,
 LPCSTR lpSubKey,
 PHKEY phkResult
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegCreateKeyW (
 HKEY hKey,
 LPCWSTR lpSubKey,
 PHKEY phkResult
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegCreateKeyExA (
 HKEY hKey,
 LPCSTR lpSubKey,
 DWORD Reserved,
 LPSTR lpClass,
 DWORD dwOptions,
 REGSAM samDesired,
 const LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 PHKEY phkResult,
 LPDWORD lpdwDisposition
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegCreateKeyExW (
 HKEY hKey,
 LPCWSTR lpSubKey,
 DWORD Reserved,
 LPWSTR lpClass,
 DWORD dwOptions,
 REGSAM samDesired,
 const LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 PHKEY phkResult,
 LPDWORD lpdwDisposition
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegCreateKeyTransactedA (
 HKEY hKey,
 LPCSTR lpSubKey,
 DWORD Reserved,
 LPSTR lpClass,
 DWORD dwOptions,
 REGSAM samDesired,
 const LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 PHKEY phkResult,
 LPDWORD lpdwDisposition,
 HANDLE hTransaction,
 PVOID pExtendedParemeter
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegCreateKeyTransactedW (
 HKEY hKey,
 LPCWSTR lpSubKey,
 DWORD Reserved,
 LPWSTR lpClass,
 DWORD dwOptions,
 REGSAM samDesired,
 const LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 PHKEY phkResult,
 LPDWORD lpdwDisposition,
 HANDLE hTransaction,
 PVOID pExtendedParemeter
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegDeleteKeyA (
 HKEY hKey,
 LPCSTR lpSubKey
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegDeleteKeyW (
 HKEY hKey,
 LPCWSTR lpSubKey
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegDeleteKeyExA (
 HKEY hKey,
 LPCSTR lpSubKey,
 REGSAM samDesired,
 DWORD Reserved
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegDeleteKeyExW (
 HKEY hKey,
 LPCWSTR lpSubKey,
 REGSAM samDesired,
 DWORD Reserved
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegDeleteKeyTransactedA (
 HKEY hKey,
 LPCSTR lpSubKey,
 REGSAM samDesired,
 DWORD Reserved,
 HANDLE hTransaction,
 PVOID pExtendedParameter
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegDeleteKeyTransactedW (
 HKEY hKey,
 LPCWSTR lpSubKey,
 REGSAM samDesired,
 DWORD Reserved,
 HANDLE hTransaction,
 PVOID pExtendedParameter
 );
__declspec(dllimport)
LONG
__stdcall
RegDisableReflectionKey (
 HKEY hBase
 );
__declspec(dllimport)
LONG
__stdcall
RegEnableReflectionKey (
 HKEY hBase
 );
__declspec(dllimport)
LONG
__stdcall
RegQueryReflectionKey (
 HKEY hBase,
 BOOL *bIsReflectionDisabled
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegDeleteValueA (
 HKEY hKey,
 LPCSTR lpValueName
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegDeleteValueW (
 HKEY hKey,
 LPCWSTR lpValueName
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegEnumKeyA (
 HKEY hKey,
 DWORD dwIndex,
 LPSTR lpName,
 DWORD cchName
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegEnumKeyW (
 HKEY hKey,
 DWORD dwIndex,
 LPWSTR lpName,
 DWORD cchName
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegEnumKeyExA (
 HKEY hKey,
 DWORD dwIndex,
 LPSTR lpName,
 LPDWORD lpcchName,
 LPDWORD lpReserved,
 LPSTR lpClass,
 LPDWORD lpcchClass,
 PFILETIME lpftLastWriteTime
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegEnumKeyExW (
 HKEY hKey,
 DWORD dwIndex,
 LPWSTR lpName,
 LPDWORD lpcchName,
 LPDWORD lpReserved,
 LPWSTR lpClass,
 LPDWORD lpcchClass,
 PFILETIME lpftLastWriteTime
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegEnumValueA (
 HKEY hKey,
 DWORD dwIndex,
 LPSTR lpValueName,
 LPDWORD lpcchValueName,
 LPDWORD lpReserved,
 LPDWORD lpType,
 LPBYTE lpData,
 LPDWORD lpcbData
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegEnumValueW (
 HKEY hKey,
 DWORD dwIndex,
 LPWSTR lpValueName,
 LPDWORD lpcchValueName,
 LPDWORD lpReserved,
 LPDWORD lpType,
 LPBYTE lpData,
 LPDWORD lpcbData
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegFlushKey (
 HKEY hKey
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegGetKeySecurity (
 HKEY hKey,
 SECURITY_INFORMATION SecurityInformation,
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 LPDWORD lpcbSecurityDescriptor
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegLoadKeyA (
 HKEY hKey,
 LPCSTR lpSubKey,
 LPCSTR lpFile
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegLoadKeyW (
 HKEY hKey,
 LPCWSTR lpSubKey,
 LPCWSTR lpFile
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegNotifyChangeKeyValue (
 HKEY hKey,
 BOOL bWatchSubtree,
 DWORD dwNotifyFilter,
 HANDLE hEvent,
 BOOL fAsynchronous
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegOpenKeyA (
 HKEY hKey,
 LPCSTR lpSubKey,
 PHKEY phkResult
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegOpenKeyW (
 HKEY hKey,
 LPCWSTR lpSubKey,
 PHKEY phkResult
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegOpenKeyExA (
 HKEY hKey,
 LPCSTR lpSubKey,
 DWORD ulOptions,
 REGSAM samDesired,
 PHKEY phkResult
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegOpenKeyExW (
 HKEY hKey,
 LPCWSTR lpSubKey,
 DWORD ulOptions,
 REGSAM samDesired,
 PHKEY phkResult
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegOpenKeyTransactedA (
 HKEY hKey,
 LPCSTR lpSubKey,
 DWORD ulOptions,
 REGSAM samDesired,
 PHKEY phkResult,
 HANDLE hTransaction,
 PVOID pExtendedParemeter
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegOpenKeyTransactedW (
 HKEY hKey,
 LPCWSTR lpSubKey,
 DWORD ulOptions,
 REGSAM samDesired,
 PHKEY phkResult,
 HANDLE hTransaction,
 PVOID pExtendedParemeter
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegQueryInfoKeyA (
 HKEY hKey,
 LPSTR lpClass,
 LPDWORD lpcchClass,
 LPDWORD lpReserved,
 LPDWORD lpcSubKeys,
 LPDWORD lpcbMaxSubKeyLen,
 LPDWORD lpcbMaxClassLen,
 LPDWORD lpcValues,
 LPDWORD lpcbMaxValueNameLen,
 LPDWORD lpcbMaxValueLen,
 LPDWORD lpcbSecurityDescriptor,
 PFILETIME lpftLastWriteTime
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegQueryInfoKeyW (
 HKEY hKey,
 LPWSTR lpClass,
 LPDWORD lpcchClass,
 LPDWORD lpReserved,
 LPDWORD lpcSubKeys,
 LPDWORD lpcbMaxSubKeyLen,
 LPDWORD lpcbMaxClassLen,
 LPDWORD lpcValues,
 LPDWORD lpcbMaxValueNameLen,
 LPDWORD lpcbMaxValueLen,
 LPDWORD lpcbSecurityDescriptor,
 PFILETIME lpftLastWriteTime
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegQueryValueA (
 HKEY hKey,
 LPCSTR lpSubKey,
 LPSTR lpData,
 PLONG lpcbData
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegQueryValueW (
 HKEY hKey,
 LPCWSTR lpSubKey,
 LPWSTR lpData,
 PLONG lpcbData
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegQueryMultipleValuesA (
 HKEY hKey,
 PVALENTA val_list,
 DWORD num_vals,
 LPSTR lpValueBuf,
 LPDWORD ldwTotsize
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegQueryMultipleValuesW (
 HKEY hKey,
 PVALENTW val_list,
 DWORD num_vals,
 LPWSTR lpValueBuf,
 LPDWORD ldwTotsize
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegQueryValueExA (
 HKEY hKey,
 LPCSTR lpValueName,
 LPDWORD lpReserved,
 LPDWORD lpType,
 LPBYTE lpData,
 LPDWORD lpcbData
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegQueryValueExW (
 HKEY hKey,
 LPCWSTR lpValueName,
 LPDWORD lpReserved,
 LPDWORD lpType,
 LPBYTE lpData,
 LPDWORD lpcbData
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegReplaceKeyA (
 HKEY hKey,
 LPCSTR lpSubKey,
 LPCSTR lpNewFile,
 LPCSTR lpOldFile
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegReplaceKeyW (
 HKEY hKey,
 LPCWSTR lpSubKey,
 LPCWSTR lpNewFile,
 LPCWSTR lpOldFile
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegRestoreKeyA (
 HKEY hKey,
 LPCSTR lpFile,
 DWORD dwFlags
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegRestoreKeyW (
 HKEY hKey,
 LPCWSTR lpFile,
 DWORD dwFlags
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegSaveKeyA (
 HKEY hKey,
 LPCSTR lpFile,
 const LPSECURITY_ATTRIBUTES lpSecurityAttributes
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegSaveKeyW (
 HKEY hKey,
 LPCWSTR lpFile,
 const LPSECURITY_ATTRIBUTES lpSecurityAttributes
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegSetKeySecurity (
 HKEY hKey,
 SECURITY_INFORMATION SecurityInformation,
 PSECURITY_DESCRIPTOR pSecurityDescriptor
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegSetValueA (
 HKEY hKey,
 LPCSTR lpSubKey,
 DWORD dwType,
 LPCSTR lpData,
 DWORD cbData
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegSetValueW (
 HKEY hKey,
 LPCWSTR lpSubKey,
 DWORD dwType,
 LPCWSTR lpData,
 DWORD cbData
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegSetValueExA (
 HKEY hKey,
 LPCSTR lpValueName,
 DWORD Reserved,
 DWORD dwType,
 const BYTE* lpData,
 DWORD cbData
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegSetValueExW (
 HKEY hKey,
 LPCWSTR lpValueName,
 DWORD Reserved,
 DWORD dwType,
 const BYTE* lpData,
 DWORD cbData
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegUnLoadKeyA (
 HKEY hKey,
 LPCSTR lpSubKey
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegUnLoadKeyW (
 HKEY hKey,
 LPCWSTR lpSubKey
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegDeleteKeyValueA (
 HKEY hKey,
 LPCSTR lpSubKey,
 LPCSTR lpValueName
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegDeleteKeyValueW (
 HKEY hKey,
 LPCWSTR lpSubKey,
 LPCWSTR lpValueName
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegSetKeyValueA (
 HKEY hKey,
 LPCSTR lpSubKey,
 LPCSTR lpValueName,
 DWORD dwType,
 LPCVOID lpData,
 DWORD cbData
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegSetKeyValueW (
 HKEY hKey,
 LPCWSTR lpSubKey,
 LPCWSTR lpValueName,
 DWORD dwType,
 LPCVOID lpData,
 DWORD cbData
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegDeleteTreeA (
 HKEY hKey,
 LPCSTR lpSubKey
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegDeleteTreeW (
 HKEY hKey,
 LPCWSTR lpSubKey
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegCopyTreeA (
 HKEY hKeySrc,
 LPCSTR lpSubKey,
 HKEY hKeyDest
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegCopyTreeW (
 HKEY hKeySrc,
 LPCWSTR lpSubKey,
 HKEY hKeyDest
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegGetValueA (
 HKEY hkey,
 LPCSTR lpSubKey,
 LPCSTR lpValue,
 DWORD dwFlags,
 LPDWORD pdwType,
 PVOID pvData,
 LPDWORD pcbData
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegGetValueW (
 HKEY hkey,
 LPCWSTR lpSubKey,
 LPCWSTR lpValue,
 DWORD dwFlags,
 LPDWORD pdwType,
 PVOID pvData,
 LPDWORD pcbData
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegLoadMUIStringA (
 HKEY hKey,
 LPCSTR pszValue,
 LPSTR pszOutBuf,
 DWORD cbOutBuf,
 LPDWORD pcbData,
 DWORD Flags,
 LPCSTR pszDirectory
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegLoadMUIStringW (
 HKEY hKey,
 LPCWSTR pszValue,
 LPWSTR pszOutBuf,
 DWORD cbOutBuf,
 LPDWORD pcbData,
 DWORD Flags,
 LPCWSTR pszDirectory
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegLoadAppKeyA (
 LPCSTR lpFile,
 PHKEY phkResult,
 REGSAM samDesired,
 DWORD dwOptions,
 DWORD Reserved
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegLoadAppKeyW (
 LPCWSTR lpFile,
 PHKEY phkResult,
 REGSAM samDesired,
 DWORD dwOptions,
 DWORD Reserved
 );
__declspec(dllimport)
BOOL
__stdcall
InitiateSystemShutdownA(
 LPSTR lpMachineName,
 LPSTR lpMessage,
 DWORD dwTimeout,
 BOOL bForceAppsClosed,
 BOOL bRebootAfterShutdown
 );
__declspec(dllimport)
BOOL
__stdcall
InitiateSystemShutdownW(
 LPWSTR lpMachineName,
 LPWSTR lpMessage,
 DWORD dwTimeout,
 BOOL bForceAppsClosed,
 BOOL bRebootAfterShutdown
 );
__declspec(dllimport)
BOOL
__stdcall
AbortSystemShutdownA(
 LPSTR lpMachineName
 );
__declspec(dllimport)
BOOL
__stdcall
AbortSystemShutdownW(
 LPWSTR lpMachineName
 );
#pragma once
__declspec(dllimport)
BOOL
__stdcall
InitiateSystemShutdownExA(
 LPSTR lpMachineName,
 LPSTR lpMessage,
 DWORD dwTimeout,
 BOOL bForceAppsClosed,
 BOOL bRebootAfterShutdown,
 DWORD dwReason
 );
__declspec(dllimport)
BOOL
__stdcall
InitiateSystemShutdownExW(
 LPWSTR lpMachineName,
 LPWSTR lpMessage,
 DWORD dwTimeout,
 BOOL bForceAppsClosed,
 BOOL bRebootAfterShutdown,
 DWORD dwReason
 );
__declspec(dllimport)
DWORD
__stdcall
InitiateShutdownA(
 LPSTR lpMachineName,
 LPSTR lpMessage,
 DWORD dwGracePeriod,
 DWORD dwShutdownFlags,
 DWORD dwReason
 );
__declspec(dllimport)
DWORD
__stdcall
InitiateShutdownW(
 LPWSTR lpMachineName,
 LPWSTR lpMessage,
 DWORD dwGracePeriod,
 DWORD dwShutdownFlags,
 DWORD dwReason
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegSaveKeyExA (
 HKEY hKey,
 LPCSTR lpFile,
 const LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 DWORD Flags
 );
__declspec(dllimport)
LSTATUS
__stdcall
RegSaveKeyExW (
 HKEY hKey,
 LPCWSTR lpFile,
 const LPSECURITY_ATTRIBUTES lpSecurityAttributes,
 DWORD Flags
 );
__declspec(dllimport)
LONG
__stdcall
Wow64Win32ApiEntry (
 DWORD dwFuncNumber,
 DWORD dwFlag,
 DWORD dwRes
 );
}
#pragma once
extern "C" {
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
DWORD __stdcall
WNetAddConnectionA(
 LPCSTR lpRemoteName,
 LPCSTR lpPassword,
 LPCSTR lpLocalName
 );
DWORD __stdcall
WNetAddConnectionW(
 LPCWSTR lpRemoteName,
 LPCWSTR lpPassword,
 LPCWSTR lpLocalName
 );
DWORD __stdcall
WNetAddConnection2A(
 LPNETRESOURCEA lpNetResource,
 LPCSTR lpPassword,
 LPCSTR lpUserName,
 DWORD dwFlags
 );
DWORD __stdcall
WNetAddConnection2W(
 LPNETRESOURCEW lpNetResource,
 LPCWSTR lpPassword,
 LPCWSTR lpUserName,
 DWORD dwFlags
 );
DWORD __stdcall
WNetAddConnection3A(
 HWND hwndOwner,
 LPNETRESOURCEA lpNetResource,
 LPCSTR lpPassword,
 LPCSTR lpUserName,
 DWORD dwFlags
 );
DWORD __stdcall
WNetAddConnection3W(
 HWND hwndOwner,
 LPNETRESOURCEW lpNetResource,
 LPCWSTR lpPassword,
 LPCWSTR lpUserName,
 DWORD dwFlags
 );
DWORD __stdcall
WNetCancelConnectionA(
 LPCSTR lpName,
 BOOL fForce
 );
DWORD __stdcall
WNetCancelConnectionW(
 LPCWSTR lpName,
 BOOL fForce
 );
DWORD __stdcall
WNetCancelConnection2A(
 LPCSTR lpName,
 DWORD dwFlags,
 BOOL fForce
 );
DWORD __stdcall
WNetCancelConnection2W(
 LPCWSTR lpName,
 DWORD dwFlags,
 BOOL fForce
 );
DWORD __stdcall
WNetGetConnectionA(
 LPCSTR lpLocalName,
 LPSTR lpRemoteName,
 LPDWORD lpnLength
 );
DWORD __stdcall
WNetGetConnectionW(
 LPCWSTR lpLocalName,
 LPWSTR lpRemoteName,
 LPDWORD lpnLength
 );
DWORD __stdcall
WNetRestoreSingleConnectionW(
 HWND hwndParent,
 LPCWSTR lpDevice,
 BOOL fUseUI
 );
DWORD __stdcall
WNetUseConnectionA(
 HWND hwndOwner,
 LPNETRESOURCEA lpNetResource,
 LPCSTR lpPassword,
 LPCSTR lpUserId,
 DWORD dwFlags,
 LPSTR lpAccessName,
 LPDWORD lpBufferSize,
 LPDWORD lpResult
 );
DWORD __stdcall
WNetUseConnectionW(
 HWND hwndOwner,
 LPNETRESOURCEW lpNetResource,
 LPCWSTR lpPassword,
 LPCWSTR lpUserId,
 DWORD dwFlags,
 LPWSTR lpAccessName,
 LPDWORD lpBufferSize,
 LPDWORD lpResult
 );
DWORD __stdcall
WNetConnectionDialog(
 HWND hwnd,
 DWORD dwType
 );
DWORD __stdcall
WNetDisconnectDialog(
 HWND hwnd,
 DWORD dwType
 );
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
DWORD __stdcall
WNetConnectionDialog1A(
 LPCONNECTDLGSTRUCTA lpConnDlgStruct
 );
DWORD __stdcall
WNetConnectionDialog1W(
 LPCONNECTDLGSTRUCTW lpConnDlgStruct
 );
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
DWORD __stdcall
WNetDisconnectDialog1A(
 LPDISCDLGSTRUCTA lpConnDlgStruct
 );
DWORD __stdcall
WNetDisconnectDialog1W(
 LPDISCDLGSTRUCTW lpConnDlgStruct
 );

DWORD __stdcall
WNetOpenEnumA(
 DWORD dwScope,
 DWORD dwType,
 DWORD dwUsage,
 LPNETRESOURCEA lpNetResource,
 LPHANDLE lphEnum
 );
DWORD __stdcall
WNetOpenEnumW(
 DWORD dwScope,
 DWORD dwType,
 DWORD dwUsage,
 LPNETRESOURCEW lpNetResource,
 LPHANDLE lphEnum
 );
DWORD __stdcall
WNetEnumResourceA(
 HANDLE hEnum,
 LPDWORD lpcCount,
 LPVOID lpBuffer,
 LPDWORD lpBufferSize
 );
DWORD __stdcall
WNetEnumResourceW(
 HANDLE hEnum,
 LPDWORD lpcCount,
 LPVOID lpBuffer,
 LPDWORD lpBufferSize
 );
DWORD __stdcall
WNetCloseEnum(
 HANDLE hEnum
 );
DWORD __stdcall
WNetGetResourceParentA(
 LPNETRESOURCEA lpNetResource,
 LPVOID lpBuffer,
 LPDWORD lpcbBuffer
 );
DWORD __stdcall
WNetGetResourceParentW(
 LPNETRESOURCEW lpNetResource,
 LPVOID lpBuffer,
 LPDWORD lpcbBuffer
 );
DWORD __stdcall
WNetGetResourceInformationA(
 LPNETRESOURCEA lpNetResource,
 LPVOID lpBuffer,
 LPDWORD lpcbBuffer,
 LPSTR *lplpSystem
 );
DWORD __stdcall
WNetGetResourceInformationW(
 LPNETRESOURCEW lpNetResource,
 LPVOID lpBuffer,
 LPDWORD lpcbBuffer,
 LPWSTR *lplpSystem
 );
typedef struct _UNIVERSAL_NAME_INFOA {
 LPSTR lpUniversalName;
}UNIVERSAL_NAME_INFOA, *LPUNIVERSAL_NAME_INFOA;
typedef struct _UNIVERSAL_NAME_INFOW {
 LPWSTR lpUniversalName;
}UNIVERSAL_NAME_INFOW, *LPUNIVERSAL_NAME_INFOW;
typedef UNIVERSAL_NAME_INFOA UNIVERSAL_NAME_INFO;
typedef LPUNIVERSAL_NAME_INFOA LPUNIVERSAL_NAME_INFO;
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
DWORD __stdcall
WNetGetUniversalNameA(
 LPCSTR lpLocalPath,
 DWORD dwInfoLevel,
 LPVOID lpBuffer,
 LPDWORD lpBufferSize
 );
DWORD __stdcall
WNetGetUniversalNameW(
 LPCWSTR lpLocalPath,
 DWORD dwInfoLevel,
 LPVOID lpBuffer,
 LPDWORD lpBufferSize
 );
DWORD __stdcall
WNetGetUserA(
 LPCSTR lpName,
 LPSTR lpUserName,
 LPDWORD lpnLength
 );
DWORD __stdcall
WNetGetUserW(
 LPCWSTR lpName,
 LPWSTR lpUserName,
 LPDWORD lpnLength
 );
DWORD __stdcall
WNetGetProviderNameA(
 DWORD dwNetType,
 LPSTR lpProviderName,
 LPDWORD lpBufferSize
 );
DWORD __stdcall
WNetGetProviderNameW(
 DWORD dwNetType,
 LPWSTR lpProviderName,
 LPDWORD lpBufferSize
 );
typedef struct _NETINFOSTRUCT{
 DWORD cbStructure;
 DWORD dwProviderVersion;
 DWORD dwStatus;
 DWORD dwCharacteristics;
 ULONG_PTR dwHandle;
 WORD wNetType;
 DWORD dwPrinters;
 DWORD dwDrives;
} NETINFOSTRUCT, *LPNETINFOSTRUCT;
DWORD __stdcall
WNetGetNetworkInformationA(
 LPCSTR lpProvider,
 LPNETINFOSTRUCT lpNetInfoStruct
 );
DWORD __stdcall
WNetGetNetworkInformationW(
 LPCWSTR lpProvider,
 LPNETINFOSTRUCT lpNetInfoStruct
 );
DWORD __stdcall
WNetGetLastErrorA(
 LPDWORD lpError,
 LPSTR lpErrorBuf,
 DWORD nErrorBufSize,
 LPSTR lpNameBuf,
 DWORD nNameBufSize
 );
DWORD __stdcall
WNetGetLastErrorW(
 LPDWORD lpError,
 LPWSTR lpErrorBuf,
 DWORD nErrorBufSize,
 LPWSTR lpNameBuf,
 DWORD nNameBufSize
 );
typedef struct _NETCONNECTINFOSTRUCT{
 DWORD cbStructure;
 DWORD dwFlags;
 DWORD dwSpeed;
 DWORD dwDelay;
 DWORD dwOptDataSize;
} NETCONNECTINFOSTRUCT, *LPNETCONNECTINFOSTRUCT;
DWORD __stdcall
MultinetGetConnectionPerformanceA(
 LPNETRESOURCEA lpNetResource,
 LPNETCONNECTINFOSTRUCT lpNetConnectInfoStruct
 );
DWORD __stdcall
MultinetGetConnectionPerformanceW(
 LPNETRESOURCEW lpNetResource,
 LPNETCONNECTINFOSTRUCT lpNetConnectInfoStruct
 );
}
extern "C" {
__inline
PUWSTR
static
__declspec(deprecated)
ua_wcscpy(
 PUWSTR Destination,
 PCUWSTR Source
 )
{
#pragma warning(push)
#pragma warning(disable:4995)
#pragma warning(disable:4996)
 return wcscpy(Destination, Source);
#pragma warning(pop)
}
}
#pragma warning(pop)
#pragma once
extern "C" {
typedef unsigned char u_char;
typedef unsigned short u_short;
typedef unsigned int u_int;
typedef unsigned long u_long;
typedef unsigned __int64 u_int64;
#pragma once
extern "C" {
#pragma warning(push)
#pragma warning(disable:4201)
#pragma warning(disable:4214)
#pragma once
typedef struct in_addr {
 union {
 struct { UCHAR s_b1,s_b2,s_b3,s_b4; } S_un_b;
 struct { USHORT s_w1,s_w2; } S_un_w;
 ULONG S_addr;
 } S_un;
} IN_ADDR, *PIN_ADDR, *LPIN_ADDR;
typedef USHORT ADDRESS_FAMILY;
typedef struct sockaddr {
 ADDRESS_FAMILY sa_family;
 CHAR sa_data[14];
} SOCKADDR, *PSOCKADDR, *LPSOCKADDR;
typedef struct _SOCKET_ADDRESS {
 LPSOCKADDR lpSockaddr;
 INT iSockaddrLength;
} SOCKET_ADDRESS, *PSOCKET_ADDRESS, *LPSOCKET_ADDRESS;
typedef struct _SOCKET_ADDRESS_LIST {
 INT iAddressCount;
 SOCKET_ADDRESS Address[1];
} SOCKET_ADDRESS_LIST, *PSOCKET_ADDRESS_LIST, *LPSOCKET_ADDRESS_LIST;
typedef struct _CSADDR_INFO {
 SOCKET_ADDRESS LocalAddr ;
 SOCKET_ADDRESS RemoteAddr ;
 INT iSocketType ;
 INT iProtocol ;
} CSADDR_INFO, *PCSADDR_INFO, * LPCSADDR_INFO ;
typedef struct sockaddr_storage {
 ADDRESS_FAMILY ss_family;
 CHAR __ss_pad1[((sizeof(__int64)) - sizeof(USHORT))];
 __int64 __ss_align;
 CHAR __ss_pad2[(128 - (sizeof(USHORT) + ((sizeof(__int64)) - sizeof(USHORT)) + (sizeof(__int64))))];
} SOCKADDR_STORAGE_LH, *PSOCKADDR_STORAGE_LH, *LPSOCKADDR_STORAGE_LH;
typedef struct sockaddr_storage_xp {
 short ss_family;
 CHAR __ss_pad1[((sizeof(__int64)) - sizeof(USHORT))];
 __int64 __ss_align;
 CHAR __ss_pad2[(128 - (sizeof(USHORT) + ((sizeof(__int64)) - sizeof(USHORT)) + (sizeof(__int64))))];
} SOCKADDR_STORAGE_XP, *PSOCKADDR_STORAGE_XP, *LPSOCKADDR_STORAGE_XP;
typedef SOCKADDR_STORAGE_LH SOCKADDR_STORAGE;
typedef SOCKADDR_STORAGE *PSOCKADDR_STORAGE, *LPSOCKADDR_STORAGE;
typedef enum {
 IPPROTO_HOPOPTS = 0,
 IPPROTO_ICMP = 1,
 IPPROTO_IGMP = 2,
 IPPROTO_GGP = 3,
 IPPROTO_IPV4 = 4,
 IPPROTO_ST = 5,
 IPPROTO_TCP = 6,
 IPPROTO_CBT = 7,
 IPPROTO_EGP = 8,
 IPPROTO_IGP = 9,
 IPPROTO_PUP = 12,
 IPPROTO_UDP = 17,
 IPPROTO_IDP = 22,
 IPPROTO_RDP = 27,
 IPPROTO_IPV6 = 41,
 IPPROTO_ROUTING = 43,
 IPPROTO_FRAGMENT = 44,
 IPPROTO_ESP = 50,
 IPPROTO_AH = 51,
 IPPROTO_ICMPV6 = 58,
 IPPROTO_NONE = 59,
 IPPROTO_DSTOPTS = 60,
 IPPROTO_ND = 77,
 IPPROTO_ICLFXBM = 78,
 IPPROTO_PIM = 103,
 IPPROTO_PGM = 113,
 IPPROTO_L2TP = 115,
 IPPROTO_SCTP = 132,
 IPPROTO_RAW = 255,
 IPPROTO_MAX = 256,
 IPPROTO_RESERVED_RAW = 257,
 IPPROTO_RESERVED_IPSEC = 258,
 IPPROTO_RESERVED_IPSECOFFLOAD = 259,
 IPPROTO_RESERVED_MAX = 260
} IPPROTO, *PIPROTO;
typedef enum {
 ScopeLevelInterface = 1,
 ScopeLevelLink = 2,
 ScopeLevelSubnet = 3,
 ScopeLevelAdmin = 4,
 ScopeLevelSite = 5,
 ScopeLevelOrganization = 8,
 ScopeLevelGlobal = 14,
 ScopeLevelCount = 16
} SCOPE_LEVEL;
typedef struct {
 union {
 struct {
 ULONG Zone : 28;
 ULONG Level : 4;
 };
 ULONG Value;
 };
} SCOPE_ID, *PSCOPE_ID;
typedef struct sockaddr_in {
 ADDRESS_FAMILY sin_family;
 USHORT sin_port;
 IN_ADDR sin_addr;
 CHAR sin_zero[8];
} SOCKADDR_IN, *PSOCKADDR_IN;
typedef struct _WSABUF {
 ULONG len;
 CHAR *buf;
} WSABUF, * LPWSABUF;
typedef struct _WSAMSG {
 LPSOCKADDR name;
 INT namelen;
 LPWSABUF lpBuffers;
 ULONG dwBufferCount;
 WSABUF Control;
 ULONG dwFlags;
} WSAMSG, *PWSAMSG, * LPWSAMSG;
typedef struct cmsghdr {
 SIZE_T cmsg_len;
 INT cmsg_level;
 INT cmsg_type;
} WSACMSGHDR, *PWSACMSGHDR, *LPWSACMSGHDR;
typedef WSACMSGHDR CMSGHDR, *PCMSGHDR;
#pragma warning(pop)
}
typedef UINT_PTR SOCKET;
typedef struct fd_set {
 u_int fd_count;
 SOCKET fd_array[64];
} fd_set;
 extern int __stdcall __WSAFDIsSet(SOCKET fd, fd_set *);
struct timeval {
 long tv_sec;
 long tv_usec;
};
struct hostent {
 char * h_name;
 char * * h_aliases;
 short h_addrtype;
 short h_length;
 char * * h_addr_list;
};
struct netent {
 char * n_name;
 char * * n_aliases;
 short n_addrtype;
 u_long n_net;
};
struct servent {
 char * s_name;
 char * * s_aliases;
 short s_port;
 char * s_proto;
};
struct protoent {
 char * p_name;
 char * * p_aliases;
 short p_proto;
};
typedef struct WSAData {
 WORD wVersion;
 WORD wHighVersion;
 char szDescription[256+1];
 char szSystemStatus[128+1];
 unsigned short iMaxSockets;
 unsigned short iMaxUdpDg;
 char * lpVendorInfo;
} WSADATA, * LPWSADATA;
struct sockproto {
 u_short sp_family;
 u_short sp_protocol;
};
struct linger {
 u_short l_onoff;
 u_short l_linger;
};
typedef struct _OVERLAPPED * LPWSAOVERLAPPED;
#pragma once
typedef ULONG SERVICETYPE;
typedef struct _flowspec
{
 ULONG TokenRate;
 ULONG TokenBucketSize;
 ULONG PeakBandwidth;
 ULONG Latency;
 ULONG DelayVariation;
 SERVICETYPE ServiceType;
 ULONG MaxSduSize;
 ULONG MinimumPolicedSize;
} FLOWSPEC, *PFLOWSPEC, * LPFLOWSPEC;
typedef struct {
 ULONG ObjectType;
 ULONG ObjectLength;
} QOS_OBJECT_HDR, *LPQOS_OBJECT_HDR;
typedef struct _QOS_SD_MODE {
 QOS_OBJECT_HDR ObjectHdr;
 ULONG ShapeDiscardMode;
} QOS_SD_MODE, *LPQOS_SD_MODE;
typedef struct _QOS_SHAPING_RATE {
 QOS_OBJECT_HDR ObjectHdr;
 ULONG ShapingRate;
} QOS_SHAPING_RATE, *LPQOS_SHAPING_RATE;
typedef struct _QualityOfService
{
 FLOWSPEC SendingFlowspec;
 FLOWSPEC ReceivingFlowspec;
 WSABUF ProviderSpecific;
} QOS, * LPQOS;
typedef unsigned int GROUP;
typedef struct _WSANETWORKEVENTS {
 long lNetworkEvents;
 int iErrorCode[10];
} WSANETWORKEVENTS, * LPWSANETWORKEVENTS;
typedef struct _WSAPROTOCOLCHAIN {
 int ChainLen;
 DWORD ChainEntries[7];
} WSAPROTOCOLCHAIN, * LPWSAPROTOCOLCHAIN;
typedef struct _WSAPROTOCOL_INFOA {
 DWORD dwServiceFlags1;
 DWORD dwServiceFlags2;
 DWORD dwServiceFlags3;
 DWORD dwServiceFlags4;
 DWORD dwProviderFlags;
 GUID ProviderId;
 DWORD dwCatalogEntryId;
 WSAPROTOCOLCHAIN ProtocolChain;
 int iVersion;
 int iAddressFamily;
 int iMaxSockAddr;
 int iMinSockAddr;
 int iSocketType;
 int iProtocol;
 int iProtocolMaxOffset;
 int iNetworkByteOrder;
 int iSecurityScheme;
 DWORD dwMessageSize;
 DWORD dwProviderReserved;
 CHAR szProtocol[255+1];
} WSAPROTOCOL_INFOA, * LPWSAPROTOCOL_INFOA;
typedef struct _WSAPROTOCOL_INFOW {
 DWORD dwServiceFlags1;
 DWORD dwServiceFlags2;
 DWORD dwServiceFlags3;
 DWORD dwServiceFlags4;
 DWORD dwProviderFlags;
 GUID ProviderId;
 DWORD dwCatalogEntryId;
 WSAPROTOCOLCHAIN ProtocolChain;
 int iVersion;
 int iAddressFamily;
 int iMaxSockAddr;
 int iMinSockAddr;
 int iSocketType;
 int iProtocol;
 int iProtocolMaxOffset;
 int iNetworkByteOrder;
 int iSecurityScheme;
 DWORD dwMessageSize;
 DWORD dwProviderReserved;
 WCHAR szProtocol[255+1];
} WSAPROTOCOL_INFOW, * LPWSAPROTOCOL_INFOW;
typedef WSAPROTOCOL_INFOA WSAPROTOCOL_INFO;
typedef LPWSAPROTOCOL_INFOA LPWSAPROTOCOL_INFO;
typedef
int
(__stdcall * LPCONDITIONPROC)(
 LPWSABUF lpCallerId,
 LPWSABUF lpCallerData,
 LPQOS lpSQOS,
 LPQOS lpGQOS,
 LPWSABUF lpCalleeId,
 LPWSABUF lpCalleeData,
 GROUP * g,
 DWORD_PTR dwCallbackData
 );
typedef
void
(__stdcall * LPWSAOVERLAPPED_COMPLETION_ROUTINE)(
 DWORD dwError,
 DWORD cbTransferred,
 LPWSAOVERLAPPED lpOverlapped,
 DWORD dwFlags
 );
typedef enum _WSACOMPLETIONTYPE {
 NSP_NOTIFY_IMMEDIATELY = 0,
 NSP_NOTIFY_HWND,
 NSP_NOTIFY_EVENT,
 NSP_NOTIFY_PORT,
 NSP_NOTIFY_APC,
} WSACOMPLETIONTYPE, *PWSACOMPLETIONTYPE, * LPWSACOMPLETIONTYPE;
typedef struct _WSACOMPLETION {
 WSACOMPLETIONTYPE Type;
 union {
 struct {
 HWND hWnd;
 UINT uMsg;
 WPARAM context;
 } WindowMessage;
 struct {
 LPWSAOVERLAPPED lpOverlapped;
 } Event;
 struct {
 LPWSAOVERLAPPED lpOverlapped;
 LPWSAOVERLAPPED_COMPLETION_ROUTINE lpfnCompletionProc;
 } Apc;
 struct {
 LPWSAOVERLAPPED lpOverlapped;
 HANDLE hPort;
 ULONG_PTR Key;
 } Port;
 } Parameters;
} WSACOMPLETION, *PWSACOMPLETION, *LPWSACOMPLETION;
typedef struct _BLOB {
 ULONG cbSize ;
 BYTE *pBlobData ;
} BLOB, *LPBLOB ;
typedef struct _AFPROTOCOLS {
 INT iAddressFamily;
 INT iProtocol;
} AFPROTOCOLS, *PAFPROTOCOLS, *LPAFPROTOCOLS;
typedef enum _WSAEcomparator
{
 COMP_EQUAL = 0,
 COMP_NOTLESS
} WSAECOMPARATOR, *PWSAECOMPARATOR, *LPWSAECOMPARATOR;
typedef struct _WSAVersion
{
 DWORD dwVersion;
 WSAECOMPARATOR ecHow;
}WSAVERSION, *PWSAVERSION, *LPWSAVERSION;
typedef struct _WSAQuerySetA
{
 DWORD dwSize;
 LPSTR lpszServiceInstanceName;
 LPGUID lpServiceClassId;
 LPWSAVERSION lpVersion;
 LPSTR lpszComment;
 DWORD dwNameSpace;
 LPGUID lpNSProviderId;
 LPSTR lpszContext;
 DWORD dwNumberOfProtocols;
 LPAFPROTOCOLS lpafpProtocols;
 LPSTR lpszQueryString;
 DWORD dwNumberOfCsAddrs;
 LPCSADDR_INFO lpcsaBuffer;
 DWORD dwOutputFlags;
 LPBLOB lpBlob;
} WSAQUERYSETA, *PWSAQUERYSETA, *LPWSAQUERYSETA;
typedef struct _WSAQuerySetW
{
 DWORD dwSize;
 LPWSTR lpszServiceInstanceName;
 LPGUID lpServiceClassId;
 LPWSAVERSION lpVersion;
 LPWSTR lpszComment;
 DWORD dwNameSpace;
 LPGUID lpNSProviderId;
 LPWSTR lpszContext;
 DWORD dwNumberOfProtocols;
 LPAFPROTOCOLS lpafpProtocols;
 LPWSTR lpszQueryString;
 DWORD dwNumberOfCsAddrs;
 LPCSADDR_INFO lpcsaBuffer;
 DWORD dwOutputFlags;
 LPBLOB lpBlob;
} WSAQUERYSETW, *PWSAQUERYSETW, *LPWSAQUERYSETW;
typedef struct _WSAQuerySet2A
{
 DWORD dwSize;
 LPSTR lpszServiceInstanceName;
 LPWSAVERSION lpVersion;
 LPSTR lpszComment;
 DWORD dwNameSpace;
 LPGUID lpNSProviderId;
 LPSTR lpszContext;
 DWORD dwNumberOfProtocols;
 LPAFPROTOCOLS lpafpProtocols;
 LPSTR lpszQueryString;
 DWORD dwNumberOfCsAddrs;
 LPCSADDR_INFO lpcsaBuffer;
 DWORD dwOutputFlags;
 LPBLOB lpBlob;
} WSAQUERYSET2A, *PWSAQUERYSET2A, *LPWSAQUERYSET2A;
typedef struct _WSAQuerySet2W
{
 DWORD dwSize;
 LPWSTR lpszServiceInstanceName;
 LPWSAVERSION lpVersion;
 LPWSTR lpszComment;
 DWORD dwNameSpace;
 LPGUID lpNSProviderId;
 LPWSTR lpszContext;
 DWORD dwNumberOfProtocols;
 LPAFPROTOCOLS lpafpProtocols;
 LPWSTR lpszQueryString;
 DWORD dwNumberOfCsAddrs;
 LPCSADDR_INFO lpcsaBuffer;
 DWORD dwOutputFlags;
 LPBLOB lpBlob;
} WSAQUERYSET2W, *PWSAQUERYSET2W, *LPWSAQUERYSET2W;
typedef WSAQUERYSETA WSAQUERYSET;
typedef PWSAQUERYSETA PWSAQUERYSET;
typedef LPWSAQUERYSETA LPWSAQUERYSET;
typedef WSAQUERYSET2A WSAQUERYSET2;
typedef PWSAQUERYSET2A PWSAQUERYSET2;
typedef LPWSAQUERYSET2A LPWSAQUERYSET2;
typedef enum _WSAESETSERVICEOP
{
 RNRSERVICE_REGISTER=0,
 RNRSERVICE_DEREGISTER,
 RNRSERVICE_DELETE
} WSAESETSERVICEOP, *PWSAESETSERVICEOP, *LPWSAESETSERVICEOP;
typedef struct _WSANSClassInfoA
{
 LPSTR lpszName;
 DWORD dwNameSpace;
 DWORD dwValueType;
 DWORD dwValueSize;
 LPVOID lpValue;
}WSANSCLASSINFOA, *PWSANSCLASSINFOA, *LPWSANSCLASSINFOA;
typedef struct _WSANSClassInfoW
{
 LPWSTR lpszName;
 DWORD dwNameSpace;
 DWORD dwValueType;
 DWORD dwValueSize;
 LPVOID lpValue;
}WSANSCLASSINFOW, *PWSANSCLASSINFOW, *LPWSANSCLASSINFOW;
typedef WSANSCLASSINFOA WSANSCLASSINFO;
typedef PWSANSCLASSINFOA PWSANSCLASSINFO;
typedef LPWSANSCLASSINFOA LPWSANSCLASSINFO;
typedef struct _WSAServiceClassInfoA
{
 LPGUID lpServiceClassId;
 LPSTR lpszServiceClassName;
 DWORD dwCount;
 LPWSANSCLASSINFOA lpClassInfos;
}WSASERVICECLASSINFOA, *PWSASERVICECLASSINFOA, *LPWSASERVICECLASSINFOA;
typedef struct _WSAServiceClassInfoW
{
 LPGUID lpServiceClassId;
 LPWSTR lpszServiceClassName;
 DWORD dwCount;
 LPWSANSCLASSINFOW lpClassInfos;
}WSASERVICECLASSINFOW, *PWSASERVICECLASSINFOW, *LPWSASERVICECLASSINFOW;
typedef WSASERVICECLASSINFOA WSASERVICECLASSINFO;
typedef PWSASERVICECLASSINFOA PWSASERVICECLASSINFO;
typedef LPWSASERVICECLASSINFOA LPWSASERVICECLASSINFO;
typedef struct _WSANAMESPACE_INFOA {
 GUID NSProviderId;
 DWORD dwNameSpace;
 BOOL fActive;
 DWORD dwVersion;
 LPSTR lpszIdentifier;
} WSANAMESPACE_INFOA, *PWSANAMESPACE_INFOA, *LPWSANAMESPACE_INFOA;
typedef struct _WSANAMESPACE_INFOW {
 GUID NSProviderId;
 DWORD dwNameSpace;
 BOOL fActive;
 DWORD dwVersion;
 LPWSTR lpszIdentifier;
} WSANAMESPACE_INFOW, *PWSANAMESPACE_INFOW, *LPWSANAMESPACE_INFOW;
typedef struct _WSANAMESPACE_INFOEXA {
 GUID NSProviderId;
 DWORD dwNameSpace;
 BOOL fActive;
 DWORD dwVersion;
 LPSTR lpszIdentifier;
 BLOB ProviderSpecific;
} WSANAMESPACE_INFOEXA, *PWSANAMESPACE_INFOEXA, *LPWSANAMESPACE_INFOEXA;
typedef struct _WSANAMESPACE_INFOEXW {
 GUID NSProviderId;
 DWORD dwNameSpace;
 BOOL fActive;
 DWORD dwVersion;
 LPWSTR lpszIdentifier;
 BLOB ProviderSpecific;
} WSANAMESPACE_INFOEXW, *PWSANAMESPACE_INFOEXW, *LPWSANAMESPACE_INFOEXW;
typedef WSANAMESPACE_INFOA WSANAMESPACE_INFO;
typedef PWSANAMESPACE_INFOA PWSANAMESPACE_INFO;
typedef LPWSANAMESPACE_INFOA LPWSANAMESPACE_INFO;
typedef WSANAMESPACE_INFOEXA WSANAMESPACE_INFOEX;
typedef PWSANAMESPACE_INFOEXA PWSANAMESPACE_INFOEX;
typedef LPWSANAMESPACE_INFOEXA LPWSANAMESPACE_INFOEX;
typedef struct pollfd {
 SOCKET fd;
 SHORT events;
 SHORT revents;
} WSAPOLLFD, *PWSAPOLLFD, *LPWSAPOLLFD;
__declspec(dllimport)
SOCKET
 __stdcall
accept(
 SOCKET s,
 struct sockaddr * addr,
 int * addrlen
 );
__declspec(dllimport)
int
 __stdcall
bind(
 SOCKET s,
 const struct sockaddr * name,
 int namelen
 );
__declspec(dllimport)
int
 __stdcall
closesocket(
 SOCKET s
 );
__declspec(dllimport)
int
 __stdcall
connect(
 SOCKET s,
 const struct sockaddr * name,
 int namelen
 );
__declspec(dllimport)
int
 __stdcall
ioctlsocket(
 SOCKET s,
 long cmd,
 u_long * argp
 );
__declspec(dllimport)
int
 __stdcall
getpeername(
 SOCKET s,
 struct sockaddr * name,
 int * namelen
 );
__declspec(dllimport)
int
 __stdcall
getsockname(
 SOCKET s,
 struct sockaddr * name,
 int * namelen
 );

__declspec(dllimport)
int
 __stdcall
getsockopt(
 SOCKET s,
 int level,
 int optname,
 char * optval,
 int * optlen
 );
__declspec(dllimport)
u_long
 __stdcall
htonl(
 u_long hostlong
 );
__declspec(dllimport)
u_short
 __stdcall
htons(
 u_short hostshort
 );
__declspec(dllimport)
unsigned long
 __stdcall
inet_addr(
 const char * cp
 );
__declspec(dllimport)
char *
 __stdcall
inet_ntoa(
 struct in_addr in
 );
__declspec(dllimport)
int
 __stdcall
listen(
 SOCKET s,
 int backlog
 );
__declspec(dllimport)
u_long
 __stdcall
ntohl(
 u_long netlong
 );
__declspec(dllimport)
u_short
 __stdcall
ntohs(
 u_short netshort
 );
__declspec(dllimport)
int
 __stdcall
recv(
 SOCKET s,
 char * buf,
 int len,
 int flags
 );
 __declspec(dllimport)
int
 __stdcall
recvfrom(
 SOCKET s,
 char * buf,
 int len,
 int flags,
 struct sockaddr * from,
 int * fromlen
 );
__declspec(dllimport)
int
 __stdcall
select(
 int nfds,
 fd_set * readfds,
 fd_set * writefds,
 fd_set * exceptfds,
 const struct timeval * timeout
 );
__declspec(dllimport)
int
 __stdcall
send(
 SOCKET s,
 const char * buf,
 int len,
 int flags
 );
__declspec(dllimport)
int
 __stdcall
sendto(
 SOCKET s,
 const char * buf,
 int len,
 int flags,
 const struct sockaddr * to,
 int tolen
 );
__declspec(dllimport)
int
 __stdcall
setsockopt(
 SOCKET s,
 int level,
 int optname,
 const char * optval,
 int optlen
 );
__declspec(dllimport)
int
 __stdcall
shutdown(
 SOCKET s,
 int how
 );
__declspec(dllimport)
SOCKET
 __stdcall
socket(
 int af,
 int type,
 int protocol
 );
__declspec(dllimport)
struct hostent *
 __stdcall
gethostbyaddr(
 const char * addr,
 int len,
 int type
 );
__declspec(dllimport)
struct hostent *
 __stdcall
gethostbyname(
 const char * name
 );
__declspec(dllimport)
int
 __stdcall
gethostname(
 char * name,
 int namelen
 );
__declspec(dllimport)
struct servent *
 __stdcall
getservbyport(
 int port,
 const char * proto
 );
__declspec(dllimport)
struct servent *
 __stdcall
getservbyname(
 const char * name,
 const char * proto
 );
__declspec(dllimport)
struct protoent *
 __stdcall
getprotobynumber(
 int number
 );
__declspec(dllimport)
struct protoent *
 __stdcall
getprotobyname(
 const char * name
 );
__declspec(dllimport)
int
 __stdcall
WSAStartup(
 WORD wVersionRequested,
 LPWSADATA lpWSAData
 );
__declspec(dllimport)
int
 __stdcall
WSACleanup(
 void
 );
__declspec(dllimport)
void
 __stdcall
WSASetLastError(
 int iError
 );
__declspec(dllimport)
int
 __stdcall
WSAGetLastError(
 void
 );
__declspec(dllimport)
BOOL
 __stdcall
WSAIsBlocking(
 void
 );
__declspec(dllimport)
int
 __stdcall
WSAUnhookBlockingHook(
 void
 );
__declspec(dllimport)
FARPROC
 __stdcall
WSASetBlockingHook(
 FARPROC lpBlockFunc
 );
__declspec(dllimport)
int
 __stdcall
WSACancelBlockingCall(
 void
 );
 __declspec(dllimport)
HANDLE
 __stdcall
WSAAsyncGetServByName(
 HWND hWnd,
 u_int wMsg,
 const char * name,
 const char * proto,
 char * buf,
 int buflen
 );
 __declspec(dllimport)
HANDLE
 __stdcall
WSAAsyncGetServByPort(
 HWND hWnd,
 u_int wMsg,
 int port,
 const char * proto,
 char * buf,
 int buflen
 );
 __declspec(dllimport)
HANDLE
 __stdcall
WSAAsyncGetProtoByName(
 HWND hWnd,
 u_int wMsg,
 const char * name,
 char * buf,
 int buflen
 );
 __declspec(dllimport)
HANDLE
 __stdcall
WSAAsyncGetProtoByNumber(
 HWND hWnd,
 u_int wMsg,
 int number,
 char * buf,
 int buflen
 );
 __declspec(dllimport)
HANDLE
 __stdcall
WSAAsyncGetHostByName(
 HWND hWnd,
 u_int wMsg,
 const char * name,
 char * buf,
 int buflen
 );
 __declspec(dllimport)
HANDLE
 __stdcall
WSAAsyncGetHostByAddr(
 HWND hWnd,
 u_int wMsg,
 const char * addr,
 int len,
 int type,
 char * buf,
 int buflen
 );
 __declspec(dllimport)
int
 __stdcall
WSACancelAsyncRequest(
 HANDLE hAsyncTaskHandle
 );
__declspec(dllimport)
int
 __stdcall
WSAAsyncSelect(
 SOCKET s,
 HWND hWnd,
 u_int wMsg,
 long lEvent
 );
__declspec(dllimport)
SOCKET
 __stdcall
WSAAccept(
 SOCKET s,
 struct sockaddr * addr,
 LPINT addrlen,
 LPCONDITIONPROC lpfnCondition,
 DWORD_PTR dwCallbackData
 );
__declspec(dllimport)
BOOL
 __stdcall
WSACloseEvent(
 HANDLE hEvent
 );
__declspec(dllimport)
int
 __stdcall
WSAConnect(
 SOCKET s,
 const struct sockaddr * name,
 int namelen,
 LPWSABUF lpCallerData,
 LPWSABUF lpCalleeData,
 LPQOS lpSQOS,
 LPQOS lpGQOS
 );
BOOL
__stdcall
WSAConnectByNameW(
 SOCKET s,
 LPWSTR nodename,
 LPWSTR servicename,
 LPDWORD LocalAddressLength,
 LPSOCKADDR LocalAddress,
 LPDWORD RemoteAddressLength,
 LPSOCKADDR RemoteAddress,
 const struct timeval * timeout,
 LPWSAOVERLAPPED Reserved);
BOOL
__stdcall
WSAConnectByNameA(
 SOCKET s,
 LPCSTR nodename,
 LPCSTR servicename,
 LPDWORD LocalAddressLength,
 LPSOCKADDR LocalAddress,
 LPDWORD RemoteAddressLength,
 LPSOCKADDR RemoteAddress,
 const struct timeval * timeout,
 LPWSAOVERLAPPED Reserved);
BOOL
__stdcall
WSAConnectByList(
 SOCKET s,
 PSOCKET_ADDRESS_LIST SocketAddress,
 LPDWORD LocalAddressLength,
 LPSOCKADDR LocalAddress,
 LPDWORD RemoteAddressLength,
 LPSOCKADDR RemoteAddress,
 const struct timeval * timeout,
 LPWSAOVERLAPPED Reserved);
__declspec(dllimport)
HANDLE
 __stdcall
WSACreateEvent(
 void
 );
__declspec(dllimport)
int
 __stdcall
WSADuplicateSocketA(
 SOCKET s,
 DWORD dwProcessId,
 LPWSAPROTOCOL_INFOA lpProtocolInfo
 );
__declspec(dllimport)
int
 __stdcall
WSADuplicateSocketW(
 SOCKET s,
 DWORD dwProcessId,
 LPWSAPROTOCOL_INFOW lpProtocolInfo
 );
__declspec(dllimport)
int
 __stdcall
WSAEnumNetworkEvents(
 SOCKET s,
 HANDLE hEventObject,
 LPWSANETWORKEVENTS lpNetworkEvents
 );
__declspec(dllimport)
int
 __stdcall
WSAEnumProtocolsA(
 LPINT lpiProtocols,
 LPWSAPROTOCOL_INFOA lpProtocolBuffer,
 LPDWORD lpdwBufferLength
 );
__declspec(dllimport)
int
 __stdcall
WSAEnumProtocolsW(
 LPINT lpiProtocols,
 LPWSAPROTOCOL_INFOW lpProtocolBuffer,
 LPDWORD lpdwBufferLength
 );
__declspec(dllimport)
int
 __stdcall
WSAEventSelect(
 SOCKET s,
 HANDLE hEventObject,
 long lNetworkEvents
 );
__declspec(dllimport)
BOOL
 __stdcall
WSAGetOverlappedResult(
 SOCKET s,
 LPWSAOVERLAPPED lpOverlapped,
 LPDWORD lpcbTransfer,
 BOOL fWait,
 LPDWORD lpdwFlags
 );
__declspec(dllimport)
BOOL
 __stdcall
WSAGetQOSByName(
 SOCKET s,
 LPWSABUF lpQOSName,
 LPQOS lpQOS
 );
__declspec(dllimport)
int
 __stdcall
WSAHtonl(
 SOCKET s,
 u_long hostlong,
 u_long * lpnetlong
 );
__declspec(dllimport)
int
 __stdcall
WSAHtons(
 SOCKET s,
 u_short hostshort,
 u_short * lpnetshort
 );
 __declspec(dllimport)
int
 __stdcall
WSAIoctl(
 SOCKET s,
 DWORD dwIoControlCode,
 LPVOID lpvInBuffer,
 DWORD cbInBuffer,
 LPVOID lpvOutBuffer,
 DWORD cbOutBuffer,
 LPDWORD lpcbBytesReturned,
 LPWSAOVERLAPPED lpOverlapped,
 LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine
 );
 __declspec(dllimport)
SOCKET
 __stdcall
WSAJoinLeaf(
 SOCKET s,
 const struct sockaddr * name,
 int namelen,
 LPWSABUF lpCallerData,
 LPWSABUF lpCalleeData,
 LPQOS lpSQOS,
 LPQOS lpGQOS,
 DWORD dwFlags
 );
__declspec(dllimport)
int
 __stdcall
WSANtohl(
 SOCKET s,
 u_long netlong,
 u_long * lphostlong
 );
__declspec(dllimport)
int
 __stdcall
WSANtohs(
 SOCKET s,
 u_short netshort,
 u_short * lphostshort
 );
__declspec(dllimport)
int
 __stdcall
WSARecv(
 SOCKET s,
 LPWSABUF lpBuffers,
 DWORD dwBufferCount,
 LPDWORD lpNumberOfBytesRecvd,
 LPDWORD lpFlags,
 LPWSAOVERLAPPED lpOverlapped,
 LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine
 );
__declspec(dllimport)
int
 __stdcall
WSARecvDisconnect(
 SOCKET s,
 LPWSABUF lpInboundDisconnectData
 );
__declspec(dllimport)
int
 __stdcall
WSARecvFrom(
 SOCKET s,
 LPWSABUF lpBuffers,
 DWORD dwBufferCount,
 LPDWORD lpNumberOfBytesRecvd,
 LPDWORD lpFlags,
 struct sockaddr * lpFrom,
 LPINT lpFromlen,
 LPWSAOVERLAPPED lpOverlapped,
 LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine
 );
__declspec(dllimport)
BOOL
 __stdcall
WSAResetEvent(
 HANDLE hEvent
 );
__declspec(dllimport)
int
 __stdcall
WSASend(
 SOCKET s,
 LPWSABUF lpBuffers,
 DWORD dwBufferCount,
 LPDWORD lpNumberOfBytesSent,
 DWORD dwFlags,
 LPWSAOVERLAPPED lpOverlapped,
 LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine
 );
__declspec(dllimport)
int
 __stdcall
WSASendMsg(
 SOCKET Handle,
 LPWSAMSG lpMsg,
 DWORD dwFlags,
 LPDWORD lpNumberOfBytesSent,
 LPWSAOVERLAPPED lpOverlapped,
 LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine
 );
__declspec(dllimport)
int
 __stdcall
WSASendDisconnect(
 SOCKET s,
 LPWSABUF lpOutboundDisconnectData
 );
__declspec(dllimport)
int
 __stdcall
WSASendTo(
 SOCKET s,
 LPWSABUF lpBuffers,
 DWORD dwBufferCount,
 LPDWORD lpNumberOfBytesSent,
 DWORD dwFlags,
 const struct sockaddr * lpTo,
 int iTolen,
 LPWSAOVERLAPPED lpOverlapped,
 LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine
 );
__declspec(dllimport)
BOOL
 __stdcall
WSASetEvent(
 HANDLE hEvent
 );
__declspec(dllimport)
SOCKET
 __stdcall
WSASocketA(
 int af,
 int type,
 int protocol,
 LPWSAPROTOCOL_INFOA lpProtocolInfo,
 GROUP g,
 DWORD dwFlags
 );
__declspec(dllimport)
SOCKET
 __stdcall
WSASocketW(
 int af,
 int type,
 int protocol,
 LPWSAPROTOCOL_INFOW lpProtocolInfo,
 GROUP g,
 DWORD dwFlags
 );
__declspec(dllimport)
DWORD
 __stdcall
WSAWaitForMultipleEvents(
 DWORD cEvents,
 const HANDLE * lphEvents,
 BOOL fWaitAll,
 DWORD dwTimeout,
 BOOL fAlertable
 );
__declspec(dllimport)
INT
 __stdcall
WSAAddressToStringA(
 LPSOCKADDR lpsaAddress,
 DWORD dwAddressLength,
 LPWSAPROTOCOL_INFOA lpProtocolInfo,
 LPSTR lpszAddressString,
 LPDWORD lpdwAddressStringLength
 );
__declspec(dllimport)
INT
 __stdcall
WSAAddressToStringW(
 LPSOCKADDR lpsaAddress,
 DWORD dwAddressLength,
 LPWSAPROTOCOL_INFOW lpProtocolInfo,
 LPWSTR lpszAddressString,
 LPDWORD lpdwAddressStringLength
 );
 __declspec(dllimport)
INT
 __stdcall
WSAStringToAddressA(
 LPSTR AddressString,
 INT AddressFamily,
 LPWSAPROTOCOL_INFOA lpProtocolInfo,
 LPSOCKADDR lpAddress,
 LPINT lpAddressLength
 );
 __declspec(dllimport)
INT
 __stdcall
WSAStringToAddressW(
 LPWSTR AddressString,
 INT AddressFamily,
 LPWSAPROTOCOL_INFOW lpProtocolInfo,
 LPSOCKADDR lpAddress,
 LPINT lpAddressLength
 );
__declspec(dllimport)
INT
 __stdcall
WSALookupServiceBeginA(
 LPWSAQUERYSETA lpqsRestrictions,
 DWORD dwControlFlags,
 LPHANDLE lphLookup
 );
__declspec(dllimport)
INT
 __stdcall
WSALookupServiceBeginW(
 LPWSAQUERYSETW lpqsRestrictions,
 DWORD dwControlFlags,
 LPHANDLE lphLookup
 );
 __declspec(dllimport)
INT
 __stdcall
WSALookupServiceNextA(
 HANDLE hLookup,
 DWORD dwControlFlags,
 LPDWORD lpdwBufferLength,
 LPWSAQUERYSETA lpqsResults
 );
 __declspec(dllimport)
INT
 __stdcall
WSALookupServiceNextW(
 HANDLE hLookup,
 DWORD dwControlFlags,
 LPDWORD lpdwBufferLength,
 LPWSAQUERYSETW lpqsResults
 );
__declspec(dllimport)
INT
 __stdcall
WSANSPIoctl(
 HANDLE hLookup,
 DWORD dwControlCode,
 LPVOID lpvInBuffer,
 DWORD cbInBuffer,
 LPVOID lpvOutBuffer,
 DWORD cbOutBuffer,
 LPDWORD lpcbBytesReturned,
 LPWSACOMPLETION lpCompletion
 );
 __declspec(dllimport)
INT
 __stdcall
WSALookupServiceEnd(
 HANDLE hLookup
 );
__declspec(dllimport)
INT
 __stdcall
WSAInstallServiceClassA(
 LPWSASERVICECLASSINFOA lpServiceClassInfo
 );
__declspec(dllimport)
INT
 __stdcall
WSAInstallServiceClassW(
 LPWSASERVICECLASSINFOW lpServiceClassInfo
 );
__declspec(dllimport)
INT
 __stdcall
WSARemoveServiceClass(
 LPGUID lpServiceClassId
 );
__declspec(dllimport)
INT
 __stdcall
WSAGetServiceClassInfoA(
 LPGUID lpProviderId,
 LPGUID lpServiceClassId,
 LPDWORD lpdwBufSize,
 LPWSASERVICECLASSINFOA lpServiceClassInfo
 );
__declspec(dllimport)
INT
 __stdcall
WSAGetServiceClassInfoW(
 LPGUID lpProviderId,
 LPGUID lpServiceClassId,
 LPDWORD lpdwBufSize,
 LPWSASERVICECLASSINFOW lpServiceClassInfo
 );
__declspec(dllimport)
INT
 __stdcall
WSAEnumNameSpaceProvidersA(
 LPDWORD lpdwBufferLength,
 LPWSANAMESPACE_INFOA lpnspBuffer
 );
__declspec(dllimport)
INT
 __stdcall
WSAEnumNameSpaceProvidersW(
 LPDWORD lpdwBufferLength,
 LPWSANAMESPACE_INFOW lpnspBuffer
 );
__declspec(dllimport)
INT
 __stdcall
WSAEnumNameSpaceProvidersExA(
 LPDWORD lpdwBufferLength,
 LPWSANAMESPACE_INFOEXA lpnspBuffer
 );
__declspec(dllimport)
INT
 __stdcall
WSAEnumNameSpaceProvidersExW(
 LPDWORD lpdwBufferLength,
 LPWSANAMESPACE_INFOEXW lpnspBuffer
 );

__declspec(dllimport)
INT
 __stdcall
WSAGetServiceClassNameByClassIdA(
 LPGUID lpServiceClassId,
 LPSTR lpszServiceClassName,
 LPDWORD lpdwBufferLength
 );
__declspec(dllimport)
INT
 __stdcall
WSAGetServiceClassNameByClassIdW(
 LPGUID lpServiceClassId,
 LPWSTR lpszServiceClassName,
 LPDWORD lpdwBufferLength
 );
__declspec(dllimport)
INT
 __stdcall
WSASetServiceA(
 LPWSAQUERYSETA lpqsRegInfo,
 WSAESETSERVICEOP essoperation,
 DWORD dwControlFlags
 );
__declspec(dllimport)
INT
 __stdcall
WSASetServiceW(
 LPWSAQUERYSETW lpqsRegInfo,
 WSAESETSERVICEOP essoperation,
 DWORD dwControlFlags
 );
__declspec(dllimport)
INT
 __stdcall
WSAProviderConfigChange(
 LPHANDLE lpNotificationHandle,
 LPWSAOVERLAPPED lpOverlapped,
 LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine
 );
__declspec(dllimport)
int
 __stdcall
WSAPoll(
 LPWSAPOLLFD fdArray,
 ULONG fds,
 INT timeout
 );
typedef struct sockaddr_in *LPSOCKADDR_IN;
typedef struct linger LINGER;
typedef struct linger *PLINGER;
typedef struct linger *LPLINGER;
typedef struct fd_set FD_SET;
typedef struct fd_set *PFD_SET;
typedef struct fd_set *LPFD_SET;
typedef struct hostent HOSTENT;
typedef struct hostent *PHOSTENT;
typedef struct hostent *LPHOSTENT;
typedef struct servent SERVENT;
typedef struct servent *PSERVENT;
typedef struct servent *LPSERVENT;
typedef struct protoent PROTOENT;
typedef struct protoent *PPROTOENT;
typedef struct protoent *LPPROTOENT;
typedef struct timeval TIMEVAL;
typedef struct timeval *PTIMEVAL;
typedef struct timeval *LPTIMEVAL;
}
extern "C" const GUID IID_IPrintDialogCallback;
extern "C" const GUID IID_IPrintDialogServices;
#pragma warning(push)
#pragma warning(disable:4201)
#pragma warning(disable:4103)
#pragma pack(push,4)
extern "C" {
struct _PSP;
typedef struct _PSP * HPROPSHEETPAGE;
struct _PROPSHEETPAGEA;
struct _PROPSHEETPAGEW;
typedef UINT (__stdcall *LPFNPSPCALLBACKA)(HWND hwnd, UINT uMsg, struct _PROPSHEETPAGEA *ppsp);
typedef UINT (__stdcall *LPFNPSPCALLBACKW)(HWND hwnd, UINT uMsg, struct _PROPSHEETPAGEW *ppsp);
typedef LPCDLGTEMPLATE PROPSHEETPAGE_RESOURCE;
typedef struct _PROPSHEETPAGEA_V1
{
 DWORD dwSize; DWORD dwFlags; HINSTANCE hInstance; union { LPCSTR pszTemplate; PROPSHEETPAGE_RESOURCE pResource; } ; union { HICON hIcon; LPCSTR pszIcon; } ; LPCSTR pszTitle; DLGPROC pfnDlgProc; LPARAM lParam; LPFNPSPCALLBACKA pfnCallback; UINT *pcRefParent;
} PROPSHEETPAGEA_V1, *LPPROPSHEETPAGEA_V1;
typedef const PROPSHEETPAGEA_V1 *LPCPROPSHEETPAGEA_V1;
typedef struct _PROPSHEETPAGEA_V2
{
 DWORD dwSize; DWORD dwFlags; HINSTANCE hInstance; union { LPCSTR pszTemplate; PROPSHEETPAGE_RESOURCE pResource; } ; union { HICON hIcon; LPCSTR pszIcon; } ; LPCSTR pszTitle; DLGPROC pfnDlgProc; LPARAM lParam; LPFNPSPCALLBACKA pfnCallback; UINT *pcRefParent;
 LPCSTR pszHeaderTitle;
 LPCSTR pszHeaderSubTitle;
} PROPSHEETPAGEA_V2, *LPPROPSHEETPAGEA_V2;
typedef const PROPSHEETPAGEA_V2 *LPCPROPSHEETPAGEA_V2;
typedef struct _PROPSHEETPAGEA_V3
{
 DWORD dwSize; DWORD dwFlags; HINSTANCE hInstance; union { LPCSTR pszTemplate; PROPSHEETPAGE_RESOURCE pResource; } ; union { HICON hIcon; LPCSTR pszIcon; } ; LPCSTR pszTitle; DLGPROC pfnDlgProc; LPARAM lParam; LPFNPSPCALLBACKA pfnCallback; UINT *pcRefParent;
 LPCSTR pszHeaderTitle;
 LPCSTR pszHeaderSubTitle;
 HANDLE hActCtx;
} PROPSHEETPAGEA_V3, *LPPROPSHEETPAGEA_V3;
typedef const PROPSHEETPAGEA_V3 *LPCPROPSHEETPAGEA_V3;
typedef struct _PROPSHEETPAGEA
{
 DWORD dwSize; DWORD dwFlags; HINSTANCE hInstance; union { LPCSTR pszTemplate; PROPSHEETPAGE_RESOURCE pResource; } ; union { HICON hIcon; LPCSTR pszIcon; } ; LPCSTR pszTitle; DLGPROC pfnDlgProc; LPARAM lParam; LPFNPSPCALLBACKA pfnCallback; UINT *pcRefParent;
 LPCSTR pszHeaderTitle;
 LPCSTR pszHeaderSubTitle;
 HANDLE hActCtx;
 union
 {
 HBITMAP hbmHeader;
 LPCSTR pszbmHeader;
 } ;
} PROPSHEETPAGEA_V4, *LPPROPSHEETPAGEA_V4;
typedef const PROPSHEETPAGEA_V4 *LPCPROPSHEETPAGEA_V4;
typedef struct _PROPSHEETPAGEW_V1
{
 DWORD dwSize; DWORD dwFlags; HINSTANCE hInstance; union { LPCWSTR pszTemplate; PROPSHEETPAGE_RESOURCE pResource; } ; union { HICON hIcon; LPCWSTR pszIcon; } ; LPCWSTR pszTitle; DLGPROC pfnDlgProc; LPARAM lParam; LPFNPSPCALLBACKW pfnCallback; UINT *pcRefParent;
} PROPSHEETPAGEW_V1, *LPPROPSHEETPAGEW_V1;
typedef const PROPSHEETPAGEW_V1 *LPCPROPSHEETPAGEW_V1;
typedef struct _PROPSHEETPAGEW_V2
{
 DWORD dwSize; DWORD dwFlags; HINSTANCE hInstance; union { LPCWSTR pszTemplate; PROPSHEETPAGE_RESOURCE pResource; } ; union { HICON hIcon; LPCWSTR pszIcon; } ; LPCWSTR pszTitle; DLGPROC pfnDlgProc; LPARAM lParam; LPFNPSPCALLBACKW pfnCallback; UINT *pcRefParent;
 LPCWSTR pszHeaderTitle;
 LPCWSTR pszHeaderSubTitle;
} PROPSHEETPAGEW_V2, *LPPROPSHEETPAGEW_V2;
typedef const PROPSHEETPAGEW_V2 *LPCPROPSHEETPAGEW_V2;
typedef struct _PROPSHEETPAGEW_V3
{
 DWORD dwSize; DWORD dwFlags; HINSTANCE hInstance; union { LPCWSTR pszTemplate; PROPSHEETPAGE_RESOURCE pResource; } ; union { HICON hIcon; LPCWSTR pszIcon; } ; LPCWSTR pszTitle; DLGPROC pfnDlgProc; LPARAM lParam; LPFNPSPCALLBACKW pfnCallback; UINT *pcRefParent;
 LPCWSTR pszHeaderTitle;
 LPCWSTR pszHeaderSubTitle;
 HANDLE hActCtx;
} PROPSHEETPAGEW_V3, *LPPROPSHEETPAGEW_V3;
typedef const PROPSHEETPAGEW_V3 *LPCPROPSHEETPAGEW_V3;
typedef struct _PROPSHEETPAGEW
{
 DWORD dwSize; DWORD dwFlags; HINSTANCE hInstance; union { LPCWSTR pszTemplate; PROPSHEETPAGE_RESOURCE pResource; } ; union { HICON hIcon; LPCWSTR pszIcon; } ; LPCWSTR pszTitle; DLGPROC pfnDlgProc; LPARAM lParam; LPFNPSPCALLBACKW pfnCallback; UINT *pcRefParent;
 LPCWSTR pszHeaderTitle;
 LPCWSTR pszHeaderSubTitle;
 HANDLE hActCtx;
 union
 {
 HBITMAP hbmHeader;
 LPCWSTR pszbmHeader;
 } ;
} PROPSHEETPAGEW_V4, *LPPROPSHEETPAGEW_V4;
typedef const PROPSHEETPAGEW_V4 *LPCPROPSHEETPAGEW_V4;
typedef PROPSHEETPAGEA_V4 PROPSHEETPAGEA_LATEST;
typedef PROPSHEETPAGEW_V4 PROPSHEETPAGEW_LATEST;
typedef LPPROPSHEETPAGEA_V4 LPPROPSHEETPAGEA_LATEST;
typedef LPPROPSHEETPAGEW_V4 LPPROPSHEETPAGEW_LATEST;
typedef LPCPROPSHEETPAGEA_V4 LPCPROPSHEETPAGEA_LATEST;
typedef LPCPROPSHEETPAGEW_V4 LPCPROPSHEETPAGEW_LATEST;
typedef PROPSHEETPAGEA_V4 PROPSHEETPAGEA;
typedef PROPSHEETPAGEW_V4 PROPSHEETPAGEW;
typedef LPPROPSHEETPAGEA_V4 LPPROPSHEETPAGEA;
typedef LPPROPSHEETPAGEW_V4 LPPROPSHEETPAGEW;
typedef LPCPROPSHEETPAGEA_V4 LPCPROPSHEETPAGEA;
typedef LPCPROPSHEETPAGEW_V4 LPCPROPSHEETPAGEW;
typedef int (__stdcall *PFNPROPSHEETCALLBACK)(HWND, UINT, LPARAM);
typedef struct _PROPSHEETHEADERA_V1
{
 DWORD dwSize; DWORD dwFlags; HWND hwndParent; HINSTANCE hInstance; union { HICON hIcon; LPCSTR pszIcon; } ; LPCSTR pszCaption; UINT nPages; union { UINT nStartPage; LPCSTR pStartPage; } ; union { LPCPROPSHEETPAGEA ppsp; HPROPSHEETPAGE *phpage; } ; PFNPROPSHEETCALLBACK pfnCallback;
} PROPSHEETHEADERA_V1, *LPPROPSHEETHEADERA_V1;
typedef const PROPSHEETHEADERA_V1 *LPCPROPSHEETHEADERA_V1;
typedef struct _PROPSHEETHEADERA_V2
{
 DWORD dwSize; DWORD dwFlags; HWND hwndParent; HINSTANCE hInstance; union { HICON hIcon; LPCSTR pszIcon; } ; LPCSTR pszCaption; UINT nPages; union { UINT nStartPage; LPCSTR pStartPage; } ; union { LPCPROPSHEETPAGEA ppsp; HPROPSHEETPAGE *phpage; } ; PFNPROPSHEETCALLBACK pfnCallback;
 union
 {
 HBITMAP hbmWatermark;
 LPCSTR pszbmWatermark;
 } ;
 HPALETTE hplWatermark;
 union
 {
 HBITMAP hbmHeader;
 LPCSTR pszbmHeader;
 } ;
} PROPSHEETHEADERA_V2, *LPPROPSHEETHEADERA_V2;
typedef const PROPSHEETHEADERA_V2 *LPCPROPSHEETHEADERA_V2;
typedef struct _PROPSHEETHEADERW_V1
{
 DWORD dwSize; DWORD dwFlags; HWND hwndParent; HINSTANCE hInstance; union { HICON hIcon; LPCWSTR pszIcon; } ; LPCWSTR pszCaption; UINT nPages; union { UINT nStartPage; LPCWSTR pStartPage; } ; union { LPCPROPSHEETPAGEW ppsp; HPROPSHEETPAGE *phpage; } ; PFNPROPSHEETCALLBACK pfnCallback;
} PROPSHEETHEADERW_V1, *LPPROPSHEETHEADERW_V1;
typedef const PROPSHEETHEADERW_V1 *LPCPROPSHEETHEADERW_V1;
typedef struct _PROPSHEETHEADERW_V2
{
 DWORD dwSize; DWORD dwFlags; HWND hwndParent; HINSTANCE hInstance; union { HICON hIcon; LPCWSTR pszIcon; } ; LPCWSTR pszCaption; UINT nPages; union { UINT nStartPage; LPCWSTR pStartPage; } ; union { LPCPROPSHEETPAGEW ppsp; HPROPSHEETPAGE *phpage; } ; PFNPROPSHEETCALLBACK pfnCallback;
 union
 {
 HBITMAP hbmWatermark;
 LPCWSTR pszbmWatermark;
 } ;
 HPALETTE hplWatermark;
 union
 {
 HBITMAP hbmHeader;
 LPCWSTR pszbmHeader;
 } ;
} PROPSHEETHEADERW_V2, *LPPROPSHEETHEADERW_V2;
typedef const PROPSHEETHEADERW_V2 *LPCPROPSHEETHEADERW_V2;
typedef PROPSHEETHEADERA_V2 PROPSHEETHEADERA;
typedef PROPSHEETHEADERW_V2 PROPSHEETHEADERW;
typedef LPPROPSHEETHEADERA_V2 LPPROPSHEETHEADERA;
typedef LPPROPSHEETHEADERW_V2 LPPROPSHEETHEADERW;
typedef LPCPROPSHEETHEADERA_V2 LPCPROPSHEETHEADERA;
typedef LPCPROPSHEETHEADERW_V2 LPCPROPSHEETHEADERW;
__declspec(dllimport) HPROPSHEETPAGE __stdcall CreatePropertySheetPageA(LPCPROPSHEETPAGEA constPropSheetPagePointer);
__declspec(dllimport) HPROPSHEETPAGE __stdcall CreatePropertySheetPageW(LPCPROPSHEETPAGEW constPropSheetPagePointer);
__declspec(dllimport) BOOL __stdcall DestroyPropertySheetPage(HPROPSHEETPAGE);
__declspec(dllimport) INT_PTR __stdcall PropertySheetA(LPCPROPSHEETHEADERA);
__declspec(dllimport) INT_PTR __stdcall PropertySheetW(LPCPROPSHEETHEADERW);
typedef BOOL (__stdcall *LPFNADDPROPSHEETPAGE)(HPROPSHEETPAGE, LPARAM);
typedef BOOL (__stdcall *LPFNADDPROPSHEETPAGES)(LPVOID, LPFNADDPROPSHEETPAGE, LPARAM);
typedef struct _PSHNOTIFY
{
 NMHDR hdr;
 LPARAM lParam;
} PSHNOTIFY, *LPPSHNOTIFY;
#pragma warning(pop)
}
#pragma warning(disable:4103)
#pragma pack(pop)
#pragma warning(disable:4103)
#pragma pack(push,1)
extern "C" {
typedef UINT_PTR (__stdcall *LPOFNHOOKPROC) (HWND, UINT, WPARAM, LPARAM);
typedef struct tagOFN_NT4A {
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
} OPENFILENAME_NT4A, *LPOPENFILENAME_NT4A;
typedef struct tagOFN_NT4W {
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
} OPENFILENAME_NT4W, *LPOPENFILENAME_NT4W;
typedef OPENFILENAME_NT4A OPENFILENAME_NT4;
typedef LPOPENFILENAME_NT4A LPOPENFILENAME_NT4;
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
 void * pvReserved;
 DWORD dwReserved;
 DWORD FlagsEx;
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
 void * pvReserved;
 DWORD dwReserved;
 DWORD FlagsEx;
} OPENFILENAMEW, *LPOPENFILENAMEW;
typedef OPENFILENAMEA OPENFILENAME;
typedef LPOPENFILENAMEA LPOPENFILENAME;
__declspec(dllimport) BOOL __stdcall GetOpenFileNameA(LPOPENFILENAMEA);
__declspec(dllimport) BOOL __stdcall GetOpenFileNameW(LPOPENFILENAMEW);
__declspec(dllimport) BOOL __stdcall GetSaveFileNameA(LPOPENFILENAMEA);
__declspec(dllimport) BOOL __stdcall GetSaveFileNameW(LPOPENFILENAMEW);
__declspec(dllimport) short __stdcall GetFileTitleA(LPCSTR, LPSTR Buf, WORD cchSize);
__declspec(dllimport) short __stdcall GetFileTitleW(LPCWSTR, LPWSTR Buf, WORD cchSize);
typedef UINT_PTR (__stdcall *LPCCHOOKPROC) (HWND, UINT, WPARAM, LPARAM);
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
__declspec(dllimport) BOOL __stdcall ChooseColorA(LPCHOOSECOLORA);
__declspec(dllimport) BOOL __stdcall ChooseColorW(LPCHOOSECOLORW);
typedef UINT_PTR (__stdcall *LPFRHOOKPROC) (HWND, UINT, WPARAM, LPARAM);
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
__declspec(dllimport) HWND __stdcall FindTextA(LPFINDREPLACEA);
__declspec(dllimport) HWND __stdcall FindTextW(LPFINDREPLACEW);
__declspec(dllimport) HWND __stdcall ReplaceTextA(LPFINDREPLACEA);
__declspec(dllimport) HWND __stdcall ReplaceTextW(LPFINDREPLACEW);
typedef UINT_PTR (__stdcall *LPCFHOOKPROC) (HWND, UINT, WPARAM, LPARAM);
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
} CHOOSEFONTA;
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
} CHOOSEFONTW;
typedef CHOOSEFONTA CHOOSEFONT;
typedef CHOOSEFONTA *LPCHOOSEFONTA;
typedef CHOOSEFONTW *LPCHOOSEFONTW;
typedef LPCHOOSEFONTA LPCHOOSEFONT;
typedef const CHOOSEFONTA *PCCHOOSEFONTA;
typedef const CHOOSEFONTW *PCCHOOSEFONTW;
typedef CHOOSEFONTA CHOOSEFONT;
typedef PCCHOOSEFONTA PCCHOOSEFONT;
__declspec(dllimport) BOOL __stdcall ChooseFontA(LPCHOOSEFONTA);
__declspec(dllimport) BOOL __stdcall ChooseFontW(LPCHOOSEFONTW);
typedef UINT_PTR (__stdcall *LPPRINTHOOKPROC) (HWND, UINT, WPARAM, LPARAM);
typedef UINT_PTR (__stdcall *LPSETUPHOOKPROC) (HWND, UINT, WPARAM, LPARAM);
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
__declspec(dllimport) BOOL __stdcall PrintDlgA(LPPRINTDLGA);
__declspec(dllimport) BOOL __stdcall PrintDlgW(LPPRINTDLGW);
typedef struct tagDEVNAMES {
 WORD wDriverOffset;
 WORD wDeviceOffset;
 WORD wOutputOffset;
 WORD wDefault;
} DEVNAMES;
typedef DEVNAMES *LPDEVNAMES;
typedef const DEVNAMES *PCDEVNAMES;
__declspec(dllimport) DWORD __stdcall CommDlgExtendedError(void);
typedef UINT_PTR (__stdcall* LPPAGEPAINTHOOK)( HWND, UINT, WPARAM, LPARAM );
typedef UINT_PTR (__stdcall* LPPAGESETUPHOOK)( HWND, UINT, WPARAM, LPARAM );
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
} PAGESETUPDLGA, * LPPAGESETUPDLGA;
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
} PAGESETUPDLGW, * LPPAGESETUPDLGW;
typedef PAGESETUPDLGA PAGESETUPDLG;
typedef LPPAGESETUPDLGA LPPAGESETUPDLG;
__declspec(dllimport) BOOL __stdcall PageSetupDlgA( LPPAGESETUPDLGA );
__declspec(dllimport) BOOL __stdcall PageSetupDlgW( LPPAGESETUPDLGW );
}
#pragma warning(disable:4103)
#pragma pack(pop)
extern "C" {
DWORD __stdcall VideoForWindowsVersion(void);
LONG __stdcall InitVFW(void);
LONG __stdcall TermVFW(void);
}
#pragma warning(disable:4103)
#pragma pack(push,1)
extern "C" {
typedef UINT MMVERSION;
typedef UINT MMRESULT;
typedef UINT *LPUINT;
typedef struct mmtime_tag
{
 UINT wType;
 union
 {
 DWORD ms;
 DWORD sample;
 DWORD cb;
 DWORD ticks;
 struct
 {
 BYTE hour;
 BYTE min;
 BYTE sec;
 BYTE frame;
 BYTE fps;
 BYTE dummy;
 BYTE pad[2];
 } smpte;
 struct
 {
 DWORD songptrpos;
 } midi;
 } u;
} MMTIME, *PMMTIME, *NPMMTIME, *LPMMTIME;
struct HDRVR__ { int unused; }; typedef struct HDRVR__ *HDRVR;

typedef struct DRVCONFIGINFOEX {
 DWORD dwDCISize;
 LPCWSTR lpszDCISectionName;
 LPCWSTR lpszDCIAliasName;
 DWORD dnDevNode;
} DRVCONFIGINFOEX, *PDRVCONFIGINFOEX, *NPDRVCONFIGINFOEX, *LPDRVCONFIGINFOEX;
typedef struct tagDRVCONFIGINFO {
 DWORD dwDCISize;
 LPCWSTR lpszDCISectionName;
 LPCWSTR lpszDCIAliasName;
} DRVCONFIGINFO, *PDRVCONFIGINFO, *NPDRVCONFIGINFO, *LPDRVCONFIGINFO;
typedef LRESULT (__stdcall* DRIVERPROC)(DWORD_PTR, HDRVR, UINT, LPARAM, LPARAM);
__declspec(dllimport) LRESULT __stdcall CloseDriver( HDRVR hDriver, LPARAM lParam1, LPARAM lParam2);
__declspec(dllimport) HDRVR __stdcall OpenDriver( LPCWSTR szDriverName, LPCWSTR szSectionName, LPARAM lParam2);
__declspec(dllimport) LRESULT __stdcall SendDriverMessage( HDRVR hDriver, UINT message, LPARAM lParam1, LPARAM lParam2);
__declspec(dllimport) HMODULE __stdcall DrvGetModuleHandle( HDRVR hDriver);
__declspec(dllimport) HMODULE __stdcall GetDriverModuleHandle( HDRVR hDriver);
__declspec(dllimport) LRESULT __stdcall DefDriverProc( DWORD_PTR dwDriverIdentifier, HDRVR hdrvr, UINT uMsg, LPARAM lParam1, LPARAM lParam2);
typedef void (__stdcall DRVCALLBACK)(HDRVR hdrvr, UINT uMsg, DWORD_PTR dwUser, DWORD_PTR dw1, DWORD_PTR dw2);
typedef DRVCALLBACK *LPDRVCALLBACK;
typedef DRVCALLBACK *PDRVCALLBACK;
__declspec(dllimport) BOOL __stdcall sndPlaySoundA( LPCSTR pszSound, UINT fuSound);
__declspec(dllimport) BOOL __stdcall sndPlaySoundW( LPCWSTR pszSound, UINT fuSound);
__declspec(dllimport) BOOL __stdcall PlaySoundA( LPCSTR pszSound, HMODULE hmod, DWORD fdwSound);
__declspec(dllimport) BOOL __stdcall PlaySoundW( LPCWSTR pszSound, HMODULE hmod, DWORD fdwSound);
struct HWAVE__ { int unused; }; typedef struct HWAVE__ *HWAVE;
struct HWAVEIN__ { int unused; }; typedef struct HWAVEIN__ *HWAVEIN;
struct HWAVEOUT__ { int unused; }; typedef struct HWAVEOUT__ *HWAVEOUT;
typedef HWAVEIN *LPHWAVEIN;
typedef HWAVEOUT *LPHWAVEOUT;
typedef DRVCALLBACK WAVECALLBACK;
typedef WAVECALLBACK *LPWAVECALLBACK;
typedef struct wavehdr_tag {
 LPSTR lpData;
 DWORD dwBufferLength;
 DWORD dwBytesRecorded;
 DWORD_PTR dwUser;
 DWORD dwFlags;
 DWORD dwLoops;
 struct wavehdr_tag *lpNext;
 DWORD_PTR reserved;
} WAVEHDR, *PWAVEHDR, *NPWAVEHDR, *LPWAVEHDR;
typedef struct tagWAVEOUTCAPSA {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 CHAR szPname[32];
 DWORD dwFormats;
 WORD wChannels;
 WORD wReserved1;
 DWORD dwSupport;
} WAVEOUTCAPSA, *PWAVEOUTCAPSA, *NPWAVEOUTCAPSA, *LPWAVEOUTCAPSA;
typedef struct tagWAVEOUTCAPSW {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 WCHAR szPname[32];
 DWORD dwFormats;
 WORD wChannels;
 WORD wReserved1;
 DWORD dwSupport;
} WAVEOUTCAPSW, *PWAVEOUTCAPSW, *NPWAVEOUTCAPSW, *LPWAVEOUTCAPSW;
typedef WAVEOUTCAPSA WAVEOUTCAPS;
typedef PWAVEOUTCAPSA PWAVEOUTCAPS;
typedef NPWAVEOUTCAPSA NPWAVEOUTCAPS;
typedef LPWAVEOUTCAPSA LPWAVEOUTCAPS;
typedef struct tagWAVEOUTCAPS2A {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 CHAR szPname[32];
 DWORD dwFormats;
 WORD wChannels;
 WORD wReserved1;
 DWORD dwSupport;
 GUID ManufacturerGuid;
 GUID ProductGuid;
 GUID NameGuid;
} WAVEOUTCAPS2A, *PWAVEOUTCAPS2A, *NPWAVEOUTCAPS2A, *LPWAVEOUTCAPS2A;
typedef struct tagWAVEOUTCAPS2W {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 WCHAR szPname[32];
 DWORD dwFormats;
 WORD wChannels;
 WORD wReserved1;
 DWORD dwSupport;
 GUID ManufacturerGuid;
 GUID ProductGuid;
 GUID NameGuid;
} WAVEOUTCAPS2W, *PWAVEOUTCAPS2W, *NPWAVEOUTCAPS2W, *LPWAVEOUTCAPS2W;
typedef WAVEOUTCAPS2A WAVEOUTCAPS2;
typedef PWAVEOUTCAPS2A PWAVEOUTCAPS2;
typedef NPWAVEOUTCAPS2A NPWAVEOUTCAPS2;
typedef LPWAVEOUTCAPS2A LPWAVEOUTCAPS2;
typedef struct tagWAVEINCAPSA {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 CHAR szPname[32];
 DWORD dwFormats;
 WORD wChannels;
 WORD wReserved1;
} WAVEINCAPSA, *PWAVEINCAPSA, *NPWAVEINCAPSA, *LPWAVEINCAPSA;
typedef struct tagWAVEINCAPSW {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 WCHAR szPname[32];
 DWORD dwFormats;
 WORD wChannels;
 WORD wReserved1;
} WAVEINCAPSW, *PWAVEINCAPSW, *NPWAVEINCAPSW, *LPWAVEINCAPSW;
typedef WAVEINCAPSA WAVEINCAPS;
typedef PWAVEINCAPSA PWAVEINCAPS;
typedef NPWAVEINCAPSA NPWAVEINCAPS;
typedef LPWAVEINCAPSA LPWAVEINCAPS;
typedef struct tagWAVEINCAPS2A {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 CHAR szPname[32];
 DWORD dwFormats;
 WORD wChannels;
 WORD wReserved1;
 GUID ManufacturerGuid;
 GUID ProductGuid;
 GUID NameGuid;
} WAVEINCAPS2A, *PWAVEINCAPS2A, *NPWAVEINCAPS2A, *LPWAVEINCAPS2A;
typedef struct tagWAVEINCAPS2W {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 WCHAR szPname[32];
 DWORD dwFormats;
 WORD wChannels;
 WORD wReserved1;
 GUID ManufacturerGuid;
 GUID ProductGuid;
 GUID NameGuid;
} WAVEINCAPS2W, *PWAVEINCAPS2W, *NPWAVEINCAPS2W, *LPWAVEINCAPS2W;
typedef WAVEINCAPS2A WAVEINCAPS2;
typedef PWAVEINCAPS2A PWAVEINCAPS2;
typedef NPWAVEINCAPS2A NPWAVEINCAPS2;
typedef LPWAVEINCAPS2A LPWAVEINCAPS2;
typedef struct waveformat_tag {
 WORD wFormatTag;
 WORD nChannels;
 DWORD nSamplesPerSec;
 DWORD nAvgBytesPerSec;
 WORD nBlockAlign;
} WAVEFORMAT, *PWAVEFORMAT, *NPWAVEFORMAT, *LPWAVEFORMAT;
typedef struct pcmwaveformat_tag {
 WAVEFORMAT wf;
 WORD wBitsPerSample;
} PCMWAVEFORMAT, *PPCMWAVEFORMAT, *NPPCMWAVEFORMAT, *LPPCMWAVEFORMAT;
typedef struct tWAVEFORMATEX
{
 WORD wFormatTag;
 WORD nChannels;
 DWORD nSamplesPerSec;
 DWORD nAvgBytesPerSec;
 WORD nBlockAlign;
 WORD wBitsPerSample;
 WORD cbSize;
} WAVEFORMATEX, *PWAVEFORMATEX, *NPWAVEFORMATEX, *LPWAVEFORMATEX;
typedef const WAVEFORMATEX *LPCWAVEFORMATEX;
__declspec(dllimport) UINT __stdcall waveOutGetNumDevs(void);
__declspec(dllimport) MMRESULT __stdcall waveOutGetDevCapsA( UINT_PTR uDeviceID, LPWAVEOUTCAPSA pwoc, UINT cbwoc);
__declspec(dllimport) MMRESULT __stdcall waveOutGetDevCapsW( UINT_PTR uDeviceID, LPWAVEOUTCAPSW pwoc, UINT cbwoc);
__declspec(dllimport) MMRESULT __stdcall waveOutGetVolume( HWAVEOUT hwo, LPDWORD pdwVolume);
__declspec(dllimport) MMRESULT __stdcall waveOutSetVolume( HWAVEOUT hwo, DWORD dwVolume);
__declspec(dllimport) MMRESULT __stdcall waveOutGetErrorTextA( MMRESULT mmrError, LPSTR pszText, UINT cchText);
__declspec(dllimport) MMRESULT __stdcall waveOutGetErrorTextW( MMRESULT mmrError, LPWSTR pszText, UINT cchText);
__declspec(dllimport) MMRESULT __stdcall waveOutOpen( LPHWAVEOUT phwo, UINT uDeviceID,
 LPCWAVEFORMATEX pwfx, DWORD_PTR dwCallback, DWORD_PTR dwInstance, DWORD fdwOpen);
__declspec(dllimport) MMRESULT __stdcall waveOutClose( HWAVEOUT hwo);
__declspec(dllimport) MMRESULT __stdcall waveOutPrepareHeader( HWAVEOUT hwo, LPWAVEHDR pwh, UINT cbwh);
__declspec(dllimport) MMRESULT __stdcall waveOutUnprepareHeader( HWAVEOUT hwo, LPWAVEHDR pwh, UINT cbwh);
__declspec(dllimport) MMRESULT __stdcall waveOutWrite( HWAVEOUT hwo, LPWAVEHDR pwh, UINT cbwh);
__declspec(dllimport) MMRESULT __stdcall waveOutPause( HWAVEOUT hwo);
__declspec(dllimport) MMRESULT __stdcall waveOutRestart( HWAVEOUT hwo);
__declspec(dllimport) MMRESULT __stdcall waveOutReset( HWAVEOUT hwo);
__declspec(dllimport) MMRESULT __stdcall waveOutBreakLoop( HWAVEOUT hwo);
__declspec(dllimport) MMRESULT __stdcall waveOutGetPosition( HWAVEOUT hwo, LPMMTIME pmmt, UINT cbmmt);
__declspec(dllimport) MMRESULT __stdcall waveOutGetPitch( HWAVEOUT hwo, LPDWORD pdwPitch);
__declspec(dllimport) MMRESULT __stdcall waveOutSetPitch( HWAVEOUT hwo, DWORD dwPitch);
__declspec(dllimport) MMRESULT __stdcall waveOutGetPlaybackRate( HWAVEOUT hwo, LPDWORD pdwRate);
__declspec(dllimport) MMRESULT __stdcall waveOutSetPlaybackRate( HWAVEOUT hwo, DWORD dwRate);
__declspec(dllimport) MMRESULT __stdcall waveOutGetID( HWAVEOUT hwo, LPUINT puDeviceID);
__declspec(dllimport) MMRESULT __stdcall waveOutMessage( HWAVEOUT hwo, UINT uMsg, DWORD_PTR dw1, DWORD_PTR dw2);
__declspec(dllimport) UINT __stdcall waveInGetNumDevs(void);
__declspec(dllimport) MMRESULT __stdcall waveInGetDevCapsA( UINT_PTR uDeviceID, LPWAVEINCAPSA pwic, UINT cbwic);
__declspec(dllimport) MMRESULT __stdcall waveInGetDevCapsW( UINT_PTR uDeviceID, LPWAVEINCAPSW pwic, UINT cbwic);
__declspec(dllimport) MMRESULT __stdcall waveInGetErrorTextA( MMRESULT mmrError, LPSTR pszText, UINT cchText);
__declspec(dllimport) MMRESULT __stdcall waveInGetErrorTextW( MMRESULT mmrError, LPWSTR pszText, UINT cchText);
__declspec(dllimport) MMRESULT __stdcall waveInOpen( LPHWAVEIN phwi, UINT uDeviceID,
 LPCWAVEFORMATEX pwfx, DWORD_PTR dwCallback, DWORD_PTR dwInstance, DWORD fdwOpen);
__declspec(dllimport) MMRESULT __stdcall waveInClose( HWAVEIN hwi);
__declspec(dllimport) MMRESULT __stdcall waveInPrepareHeader( HWAVEIN hwi, LPWAVEHDR pwh, UINT cbwh);
__declspec(dllimport) MMRESULT __stdcall waveInUnprepareHeader( HWAVEIN hwi, LPWAVEHDR pwh, UINT cbwh);
__declspec(dllimport) MMRESULT __stdcall waveInAddBuffer( HWAVEIN hwi, LPWAVEHDR pwh, UINT cbwh);
__declspec(dllimport) MMRESULT __stdcall waveInStart( HWAVEIN hwi);
__declspec(dllimport) MMRESULT __stdcall waveInStop( HWAVEIN hwi);
__declspec(dllimport) MMRESULT __stdcall waveInReset( HWAVEIN hwi);
__declspec(dllimport) MMRESULT __stdcall waveInGetPosition( HWAVEIN hwi, LPMMTIME pmmt, UINT cbmmt);
__declspec(dllimport) MMRESULT __stdcall waveInGetID( HWAVEIN hwi, LPUINT puDeviceID);
__declspec(dllimport) MMRESULT __stdcall waveInMessage( HWAVEIN hwi, UINT uMsg, DWORD_PTR dw1, DWORD_PTR dw2);
struct HMIDI__ { int unused; }; typedef struct HMIDI__ *HMIDI;
struct HMIDIIN__ { int unused; }; typedef struct HMIDIIN__ *HMIDIIN;
struct HMIDIOUT__ { int unused; }; typedef struct HMIDIOUT__ *HMIDIOUT;
struct HMIDISTRM__ { int unused; }; typedef struct HMIDISTRM__ *HMIDISTRM;
typedef HMIDI *LPHMIDI;
typedef HMIDIIN *LPHMIDIIN;
typedef HMIDIOUT *LPHMIDIOUT;
typedef HMIDISTRM *LPHMIDISTRM;
typedef DRVCALLBACK MIDICALLBACK;
typedef MIDICALLBACK *LPMIDICALLBACK;
typedef WORD PATCHARRAY[128];
typedef WORD *LPPATCHARRAY;
typedef WORD KEYARRAY[128];
typedef WORD *LPKEYARRAY;
typedef struct tagMIDIOUTCAPSA {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 CHAR szPname[32];
 WORD wTechnology;
 WORD wVoices;
 WORD wNotes;
 WORD wChannelMask;
 DWORD dwSupport;
} MIDIOUTCAPSA, *PMIDIOUTCAPSA, *NPMIDIOUTCAPSA, *LPMIDIOUTCAPSA;
typedef struct tagMIDIOUTCAPSW {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 WCHAR szPname[32];
 WORD wTechnology;
 WORD wVoices;
 WORD wNotes;
 WORD wChannelMask;
 DWORD dwSupport;
} MIDIOUTCAPSW, *PMIDIOUTCAPSW, *NPMIDIOUTCAPSW, *LPMIDIOUTCAPSW;
typedef MIDIOUTCAPSA MIDIOUTCAPS;
typedef PMIDIOUTCAPSA PMIDIOUTCAPS;
typedef NPMIDIOUTCAPSA NPMIDIOUTCAPS;
typedef LPMIDIOUTCAPSA LPMIDIOUTCAPS;
typedef struct tagMIDIOUTCAPS2A {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 CHAR szPname[32];
 WORD wTechnology;
 WORD wVoices;
 WORD wNotes;
 WORD wChannelMask;
 DWORD dwSupport;
 GUID ManufacturerGuid;
 GUID ProductGuid;
 GUID NameGuid; 
} MIDIOUTCAPS2A, *PMIDIOUTCAPS2A, *NPMIDIOUTCAPS2A, *LPMIDIOUTCAPS2A;
typedef struct tagMIDIOUTCAPS2W {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 WCHAR szPname[32];
 WORD wTechnology;
 WORD wVoices;
 WORD wNotes;
 WORD wChannelMask;
 DWORD dwSupport;
 GUID ManufacturerGuid;
 GUID ProductGuid;
 GUID NameGuid;
} MIDIOUTCAPS2W, *PMIDIOUTCAPS2W, *NPMIDIOUTCAPS2W, *LPMIDIOUTCAPS2W;
typedef MIDIOUTCAPS2A MIDIOUTCAPS2;
typedef PMIDIOUTCAPS2A PMIDIOUTCAPS2;
typedef NPMIDIOUTCAPS2A NPMIDIOUTCAPS2;
typedef LPMIDIOUTCAPS2A LPMIDIOUTCAPS2;
typedef struct tagMIDIINCAPSA {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 CHAR szPname[32];
 DWORD dwSupport;
} MIDIINCAPSA, *PMIDIINCAPSA, *NPMIDIINCAPSA, *LPMIDIINCAPSA;
typedef struct tagMIDIINCAPSW {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 WCHAR szPname[32];
 DWORD dwSupport;
} MIDIINCAPSW, *PMIDIINCAPSW, *NPMIDIINCAPSW, *LPMIDIINCAPSW;
typedef MIDIINCAPSA MIDIINCAPS;
typedef PMIDIINCAPSA PMIDIINCAPS;
typedef NPMIDIINCAPSA NPMIDIINCAPS;
typedef LPMIDIINCAPSA LPMIDIINCAPS;
typedef struct tagMIDIINCAPS2A {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 CHAR szPname[32];
 DWORD dwSupport;
 GUID ManufacturerGuid;
 GUID ProductGuid;
 GUID NameGuid;
} MIDIINCAPS2A, *PMIDIINCAPS2A, *NPMIDIINCAPS2A, *LPMIDIINCAPS2A;
typedef struct tagMIDIINCAPS2W {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 WCHAR szPname[32];
 DWORD dwSupport;
 GUID ManufacturerGuid;
 GUID ProductGuid;
 GUID NameGuid;
} MIDIINCAPS2W, *PMIDIINCAPS2W, *NPMIDIINCAPS2W, *LPMIDIINCAPS2W;
typedef MIDIINCAPS2A MIDIINCAPS2;
typedef PMIDIINCAPS2A PMIDIINCAPS2;
typedef NPMIDIINCAPS2A NPMIDIINCAPS2;
typedef LPMIDIINCAPS2A LPMIDIINCAPS2;
typedef struct midihdr_tag {
 LPSTR lpData;
 DWORD dwBufferLength;
 DWORD dwBytesRecorded;
 DWORD_PTR dwUser;
 DWORD dwFlags;
 struct midihdr_tag *lpNext;
 DWORD_PTR reserved;
 DWORD dwOffset;
 DWORD_PTR dwReserved[8];
} MIDIHDR, *PMIDIHDR, *NPMIDIHDR, *LPMIDIHDR;
typedef struct midievent_tag
{
 DWORD dwDeltaTime;
 DWORD dwStreamID;
 DWORD dwEvent;
 DWORD dwParms[1];
} MIDIEVENT;
typedef struct midistrmbuffver_tag
{
 DWORD dwVersion;
 DWORD dwMid;
 DWORD dwOEMVersion;
} MIDISTRMBUFFVER;
typedef struct midiproptimediv_tag
{
 DWORD cbStruct;
 DWORD dwTimeDiv;
} MIDIPROPTIMEDIV, *LPMIDIPROPTIMEDIV;
typedef struct midiproptempo_tag
{
 DWORD cbStruct;
 DWORD dwTempo;
} MIDIPROPTEMPO, *LPMIDIPROPTEMPO;
__declspec(dllimport) UINT __stdcall midiOutGetNumDevs(void);
__declspec(dllimport) MMRESULT __stdcall midiStreamOpen( LPHMIDISTRM phms, LPUINT puDeviceID, DWORD cMidi, DWORD_PTR dwCallback, DWORD_PTR dwInstance, DWORD fdwOpen);
__declspec(dllimport) MMRESULT __stdcall midiStreamClose( HMIDISTRM hms);
__declspec(dllimport) MMRESULT __stdcall midiStreamProperty( HMIDISTRM hms, LPBYTE lppropdata, DWORD dwProperty);
__declspec(dllimport) MMRESULT __stdcall midiStreamPosition( HMIDISTRM hms, LPMMTIME lpmmt, UINT cbmmt);
__declspec(dllimport) MMRESULT __stdcall midiStreamOut( HMIDISTRM hms, LPMIDIHDR pmh, UINT cbmh);
__declspec(dllimport) MMRESULT __stdcall midiStreamPause( HMIDISTRM hms);
__declspec(dllimport) MMRESULT __stdcall midiStreamRestart( HMIDISTRM hms);
__declspec(dllimport) MMRESULT __stdcall midiStreamStop( HMIDISTRM hms);
__declspec(dllimport) MMRESULT __stdcall midiConnect( HMIDI hmi, HMIDIOUT hmo, LPVOID pReserved);
__declspec(dllimport) MMRESULT __stdcall midiDisconnect( HMIDI hmi, HMIDIOUT hmo, LPVOID pReserved);
__declspec(dllimport) MMRESULT __stdcall midiOutGetDevCapsA( UINT_PTR uDeviceID, LPMIDIOUTCAPSA pmoc, UINT cbmoc);
__declspec(dllimport) MMRESULT __stdcall midiOutGetDevCapsW( UINT_PTR uDeviceID, LPMIDIOUTCAPSW pmoc, UINT cbmoc);
__declspec(dllimport) MMRESULT __stdcall midiOutGetVolume( HMIDIOUT hmo, LPDWORD pdwVolume);
__declspec(dllimport) MMRESULT __stdcall midiOutSetVolume( HMIDIOUT hmo, DWORD dwVolume);
__declspec(dllimport) MMRESULT __stdcall midiOutGetErrorTextA( MMRESULT mmrError, LPSTR pszText, UINT cchText);
__declspec(dllimport) MMRESULT __stdcall midiOutGetErrorTextW( MMRESULT mmrError, LPWSTR pszText, UINT cchText);
__declspec(dllimport) MMRESULT __stdcall midiOutOpen( LPHMIDIOUT phmo, UINT uDeviceID,
 DWORD_PTR dwCallback, DWORD_PTR dwInstance, DWORD fdwOpen);
__declspec(dllimport) MMRESULT __stdcall midiOutClose( HMIDIOUT hmo);
__declspec(dllimport) MMRESULT __stdcall midiOutPrepareHeader( HMIDIOUT hmo, LPMIDIHDR pmh, UINT cbmh);
__declspec(dllimport) MMRESULT __stdcall midiOutUnprepareHeader( HMIDIOUT hmo, LPMIDIHDR pmh, UINT cbmh);
__declspec(dllimport) MMRESULT __stdcall midiOutShortMsg( HMIDIOUT hmo, DWORD dwMsg);
__declspec(dllimport) MMRESULT __stdcall midiOutLongMsg( HMIDIOUT hmo, LPMIDIHDR pmh, UINT cbmh);
__declspec(dllimport) MMRESULT __stdcall midiOutReset( HMIDIOUT hmo);
__declspec(dllimport) MMRESULT __stdcall midiOutCachePatches( HMIDIOUT hmo, UINT uBank, LPWORD pwpa, UINT fuCache);
__declspec(dllimport) MMRESULT __stdcall midiOutCacheDrumPatches( HMIDIOUT hmo, UINT uPatch, LPWORD pwkya, UINT fuCache);
__declspec(dllimport) MMRESULT __stdcall midiOutGetID( HMIDIOUT hmo, LPUINT puDeviceID);
__declspec(dllimport) MMRESULT __stdcall midiOutMessage( HMIDIOUT hmo, UINT uMsg, DWORD_PTR dw1, DWORD_PTR dw2);
__declspec(dllimport) UINT __stdcall midiInGetNumDevs(void);
__declspec(dllimport) MMRESULT __stdcall midiInGetDevCapsA( UINT_PTR uDeviceID, LPMIDIINCAPSA pmic, UINT cbmic);
__declspec(dllimport) MMRESULT __stdcall midiInGetDevCapsW( UINT_PTR uDeviceID, LPMIDIINCAPSW pmic, UINT cbmic);
__declspec(dllimport) MMRESULT __stdcall midiInGetErrorTextA( MMRESULT mmrError, LPSTR pszText, UINT cchText);
__declspec(dllimport) MMRESULT __stdcall midiInGetErrorTextW( MMRESULT mmrError, LPWSTR pszText, UINT cchText);
__declspec(dllimport) MMRESULT __stdcall midiInOpen( LPHMIDIIN phmi, UINT uDeviceID,
 DWORD_PTR dwCallback, DWORD_PTR dwInstance, DWORD fdwOpen);
__declspec(dllimport) MMRESULT __stdcall midiInClose( HMIDIIN hmi);
__declspec(dllimport) MMRESULT __stdcall midiInPrepareHeader( HMIDIIN hmi, LPMIDIHDR pmh, UINT cbmh);
__declspec(dllimport) MMRESULT __stdcall midiInUnprepareHeader( HMIDIIN hmi, LPMIDIHDR pmh, UINT cbmh);
__declspec(dllimport) MMRESULT __stdcall midiInAddBuffer( HMIDIIN hmi, LPMIDIHDR pmh, UINT cbmh);
__declspec(dllimport) MMRESULT __stdcall midiInStart( HMIDIIN hmi);
__declspec(dllimport) MMRESULT __stdcall midiInStop( HMIDIIN hmi);
__declspec(dllimport) MMRESULT __stdcall midiInReset( HMIDIIN hmi);
__declspec(dllimport) MMRESULT __stdcall midiInGetID( HMIDIIN hmi, LPUINT puDeviceID);
__declspec(dllimport) MMRESULT __stdcall midiInMessage( HMIDIIN hmi, UINT uMsg, DWORD_PTR dw1, DWORD_PTR dw2);
typedef struct tagAUXCAPSA {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 CHAR szPname[32];
 WORD wTechnology;
 WORD wReserved1;
 DWORD dwSupport;
} AUXCAPSA, *PAUXCAPSA, *NPAUXCAPSA, *LPAUXCAPSA;
typedef struct tagAUXCAPSW {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 WCHAR szPname[32];
 WORD wTechnology;
 WORD wReserved1;
 DWORD dwSupport;
} AUXCAPSW, *PAUXCAPSW, *NPAUXCAPSW, *LPAUXCAPSW;
typedef AUXCAPSA AUXCAPS;
typedef PAUXCAPSA PAUXCAPS;
typedef NPAUXCAPSA NPAUXCAPS;
typedef LPAUXCAPSA LPAUXCAPS;
typedef struct tagAUXCAPS2A {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 CHAR szPname[32];
 WORD wTechnology;
 WORD wReserved1;
 DWORD dwSupport;
 GUID ManufacturerGuid;
 GUID ProductGuid;
 GUID NameGuid;
} AUXCAPS2A, *PAUXCAPS2A, *NPAUXCAPS2A, *LPAUXCAPS2A;
typedef struct tagAUXCAPS2W {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 WCHAR szPname[32];
 WORD wTechnology;
 WORD wReserved1;
 DWORD dwSupport;
 GUID ManufacturerGuid;
 GUID ProductGuid;
 GUID NameGuid;
} AUXCAPS2W, *PAUXCAPS2W, *NPAUXCAPS2W, *LPAUXCAPS2W;
typedef AUXCAPS2A AUXCAPS2;
typedef PAUXCAPS2A PAUXCAPS2;
typedef NPAUXCAPS2A NPAUXCAPS2;
typedef LPAUXCAPS2A LPAUXCAPS2;
__declspec(dllimport) UINT __stdcall auxGetNumDevs(void);
__declspec(dllimport) MMRESULT __stdcall auxGetDevCapsA( UINT_PTR uDeviceID, LPAUXCAPSA pac, UINT cbac);
__declspec(dllimport) MMRESULT __stdcall auxGetDevCapsW( UINT_PTR uDeviceID, LPAUXCAPSW pac, UINT cbac);
__declspec(dllimport) MMRESULT __stdcall auxSetVolume( UINT uDeviceID, DWORD dwVolume);
__declspec(dllimport) MMRESULT __stdcall auxGetVolume( UINT uDeviceID, LPDWORD pdwVolume);
__declspec(dllimport) MMRESULT __stdcall auxOutMessage( UINT uDeviceID, UINT uMsg, DWORD_PTR dw1, DWORD_PTR dw2);
struct HMIXEROBJ__ { int unused; }; typedef struct HMIXEROBJ__ *HMIXEROBJ;
typedef HMIXEROBJ *LPHMIXEROBJ;
struct HMIXER__ { int unused; }; typedef struct HMIXER__ *HMIXER;
typedef HMIXER *LPHMIXER;
__declspec(dllimport) UINT __stdcall mixerGetNumDevs(void);
typedef struct tagMIXERCAPSA {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 CHAR szPname[32];
 DWORD fdwSupport;
 DWORD cDestinations;
} MIXERCAPSA, *PMIXERCAPSA, *LPMIXERCAPSA;
typedef struct tagMIXERCAPSW {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 WCHAR szPname[32];
 DWORD fdwSupport;
 DWORD cDestinations;
} MIXERCAPSW, *PMIXERCAPSW, *LPMIXERCAPSW;
typedef MIXERCAPSA MIXERCAPS;
typedef PMIXERCAPSA PMIXERCAPS;
typedef LPMIXERCAPSA LPMIXERCAPS;
typedef struct tagMIXERCAPS2A {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 CHAR szPname[32];
 DWORD fdwSupport;
 DWORD cDestinations;
 GUID ManufacturerGuid;
 GUID ProductGuid;
 GUID NameGuid;
} MIXERCAPS2A, *PMIXERCAPS2A, *LPMIXERCAPS2A;
typedef struct tagMIXERCAPS2W {
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 WCHAR szPname[32];
 DWORD fdwSupport;
 DWORD cDestinations;
 GUID ManufacturerGuid;
 GUID ProductGuid;
 GUID NameGuid;
} MIXERCAPS2W, *PMIXERCAPS2W, *LPMIXERCAPS2W;
typedef MIXERCAPS2A MIXERCAPS2;
typedef PMIXERCAPS2A PMIXERCAPS2;
typedef LPMIXERCAPS2A LPMIXERCAPS2;
__declspec(dllimport) MMRESULT __stdcall mixerGetDevCapsA( UINT_PTR uMxId, LPMIXERCAPSA pmxcaps, UINT cbmxcaps);
__declspec(dllimport) MMRESULT __stdcall mixerGetDevCapsW( UINT_PTR uMxId, LPMIXERCAPSW pmxcaps, UINT cbmxcaps);
__declspec(dllimport) MMRESULT __stdcall mixerOpen( LPHMIXER phmx, UINT uMxId, DWORD_PTR dwCallback, DWORD_PTR dwInstance, DWORD fdwOpen);
__declspec(dllimport) MMRESULT __stdcall mixerClose( HMIXER hmx);
__declspec(dllimport) DWORD __stdcall mixerMessage( HMIXER hmx, UINT uMsg, DWORD_PTR dwParam1, DWORD_PTR dwParam2);
typedef struct tagMIXERLINEA {
 DWORD cbStruct;
 DWORD dwDestination;
 DWORD dwSource;
 DWORD dwLineID;
 DWORD fdwLine;
 DWORD_PTR dwUser;
 DWORD dwComponentType;
 DWORD cChannels;
 DWORD cConnections;
 DWORD cControls;
 CHAR szShortName[16];
 CHAR szName[64];
 struct {
 DWORD dwType;
 DWORD dwDeviceID;
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 CHAR szPname[32];
 } Target;
} MIXERLINEA, *PMIXERLINEA, *LPMIXERLINEA;
typedef struct tagMIXERLINEW {
 DWORD cbStruct;
 DWORD dwDestination;
 DWORD dwSource;
 DWORD dwLineID;
 DWORD fdwLine;
 DWORD_PTR dwUser;
 DWORD dwComponentType;
 DWORD cChannels;
 DWORD cConnections;
 DWORD cControls;
 WCHAR szShortName[16];
 WCHAR szName[64];
 struct {
 DWORD dwType;
 DWORD dwDeviceID;
 WORD wMid;
 WORD wPid;
 MMVERSION vDriverVersion;
 WCHAR szPname[32];
 } Target;
} MIXERLINEW, *PMIXERLINEW, *LPMIXERLINEW;
typedef MIXERLINEA MIXERLINE;
typedef PMIXERLINEA PMIXERLINE;
typedef LPMIXERLINEA LPMIXERLINE;
__declspec(dllimport) MMRESULT __stdcall mixerGetLineInfoA( HMIXEROBJ hmxobj, LPMIXERLINEA pmxl, DWORD fdwInfo);
__declspec(dllimport) MMRESULT __stdcall mixerGetLineInfoW( HMIXEROBJ hmxobj, LPMIXERLINEW pmxl, DWORD fdwInfo);
__declspec(dllimport) MMRESULT __stdcall mixerGetID( HMIXEROBJ hmxobj, UINT *puMxId, DWORD fdwId);
typedef struct tagMIXERCONTROLA {
 DWORD cbStruct;
 DWORD dwControlID;
 DWORD dwControlType;
 DWORD fdwControl;
 DWORD cMultipleItems;
 CHAR szShortName[16];
 CHAR szName[64];
 union {
 struct {
 LONG lMinimum;
 LONG lMaximum;
 };
 struct {
 DWORD dwMinimum;
 DWORD dwMaximum;
 };
 DWORD dwReserved[6];
 } Bounds;
 union {
 DWORD cSteps;
 DWORD cbCustomData;
 DWORD dwReserved[6];
 } Metrics;
} MIXERCONTROLA, *PMIXERCONTROLA, *LPMIXERCONTROLA;
typedef struct tagMIXERCONTROLW {
 DWORD cbStruct;
 DWORD dwControlID;
 DWORD dwControlType;
 DWORD fdwControl;
 DWORD cMultipleItems;
 WCHAR szShortName[16];
 WCHAR szName[64];
 union {
 struct {
 LONG lMinimum;
 LONG lMaximum;
 };
 struct {
 DWORD dwMinimum;
 DWORD dwMaximum;
 };
 DWORD dwReserved[6];
 } Bounds;
 union {
 DWORD cSteps;
 DWORD cbCustomData;
 DWORD dwReserved[6];
 } Metrics;
} MIXERCONTROLW, *PMIXERCONTROLW, *LPMIXERCONTROLW;
typedef MIXERCONTROLA MIXERCONTROL;
typedef PMIXERCONTROLA PMIXERCONTROL;
typedef LPMIXERCONTROLA LPMIXERCONTROL;
typedef struct tagMIXERLINECONTROLSA {
 DWORD cbStruct;
 DWORD dwLineID;
 union {
 DWORD dwControlID;
 DWORD dwControlType;
 };
 DWORD cControls;
 DWORD cbmxctrl;
 LPMIXERCONTROLA pamxctrl;
} MIXERLINECONTROLSA, *PMIXERLINECONTROLSA, *LPMIXERLINECONTROLSA;
typedef struct tagMIXERLINECONTROLSW {
 DWORD cbStruct;
 DWORD dwLineID;
 union {
 DWORD dwControlID;
 DWORD dwControlType;
 };
 DWORD cControls;
 DWORD cbmxctrl;
 LPMIXERCONTROLW pamxctrl;
} MIXERLINECONTROLSW, *PMIXERLINECONTROLSW, *LPMIXERLINECONTROLSW;
typedef MIXERLINECONTROLSA MIXERLINECONTROLS;
typedef PMIXERLINECONTROLSA PMIXERLINECONTROLS;
typedef LPMIXERLINECONTROLSA LPMIXERLINECONTROLS;
__declspec(dllimport) MMRESULT __stdcall mixerGetLineControlsA( HMIXEROBJ hmxobj, LPMIXERLINECONTROLSA pmxlc, DWORD fdwControls);
__declspec(dllimport) MMRESULT __stdcall mixerGetLineControlsW( HMIXEROBJ hmxobj, LPMIXERLINECONTROLSW pmxlc, DWORD fdwControls);
typedef struct tMIXERCONTROLDETAILS {
 DWORD cbStruct;
 DWORD dwControlID;
 DWORD cChannels;
 union {
 HWND hwndOwner;
 DWORD cMultipleItems;
 };
 DWORD cbDetails;
 LPVOID paDetails;
} MIXERCONTROLDETAILS, *PMIXERCONTROLDETAILS, *LPMIXERCONTROLDETAILS;
typedef struct tagMIXERCONTROLDETAILS_LISTTEXTA {
 DWORD dwParam1;
 DWORD dwParam2;
 CHAR szName[64];
} MIXERCONTROLDETAILS_LISTTEXTA, *PMIXERCONTROLDETAILS_LISTTEXTA, *LPMIXERCONTROLDETAILS_LISTTEXTA;
typedef struct tagMIXERCONTROLDETAILS_LISTTEXTW {
 DWORD dwParam1;
 DWORD dwParam2;
 WCHAR szName[64];
} MIXERCONTROLDETAILS_LISTTEXTW, *PMIXERCONTROLDETAILS_LISTTEXTW, *LPMIXERCONTROLDETAILS_LISTTEXTW;
typedef MIXERCONTROLDETAILS_LISTTEXTA MIXERCONTROLDETAILS_LISTTEXT;
typedef PMIXERCONTROLDETAILS_LISTTEXTA PMIXERCONTROLDETAILS_LISTTEXT;
typedef LPMIXERCONTROLDETAILS_LISTTEXTA LPMIXERCONTROLDETAILS_LISTTEXT;
typedef struct tMIXERCONTROLDETAILS_BOOLEAN {
 LONG fValue;
} MIXERCONTROLDETAILS_BOOLEAN,
 *PMIXERCONTROLDETAILS_BOOLEAN,
 *LPMIXERCONTROLDETAILS_BOOLEAN;
typedef struct tMIXERCONTROLDETAILS_SIGNED {
 LONG lValue;
} MIXERCONTROLDETAILS_SIGNED,
 *PMIXERCONTROLDETAILS_SIGNED,
 *LPMIXERCONTROLDETAILS_SIGNED;
typedef struct tMIXERCONTROLDETAILS_UNSIGNED {
 DWORD dwValue;
} MIXERCONTROLDETAILS_UNSIGNED,
 *PMIXERCONTROLDETAILS_UNSIGNED,
 *LPMIXERCONTROLDETAILS_UNSIGNED;
__declspec(dllimport) MMRESULT __stdcall mixerGetControlDetailsA( HMIXEROBJ hmxobj, LPMIXERCONTROLDETAILS pmxcd, DWORD fdwDetails);
__declspec(dllimport) MMRESULT __stdcall mixerGetControlDetailsW( HMIXEROBJ hmxobj, LPMIXERCONTROLDETAILS pmxcd, DWORD fdwDetails);
__declspec(dllimport) MMRESULT __stdcall mixerSetControlDetails( HMIXEROBJ hmxobj, LPMIXERCONTROLDETAILS pmxcd, DWORD fdwDetails);
typedef void (__stdcall TIMECALLBACK)(UINT uTimerID, UINT uMsg, DWORD_PTR dwUser, DWORD_PTR dw1, DWORD_PTR dw2);
typedef TIMECALLBACK *LPTIMECALLBACK;
typedef struct timecaps_tag {
 UINT wPeriodMin;
 UINT wPeriodMax;
} TIMECAPS, *PTIMECAPS, *NPTIMECAPS, *LPTIMECAPS;
__declspec(dllimport) MMRESULT __stdcall timeGetSystemTime( LPMMTIME pmmt, UINT cbmmt);
__declspec(dllimport) DWORD __stdcall timeGetTime(void);
__declspec(dllimport) MMRESULT __stdcall timeSetEvent( UINT uDelay, UINT uResolution,
 LPTIMECALLBACK fptc, DWORD_PTR dwUser, UINT fuEvent);
__declspec(dllimport) MMRESULT __stdcall timeKillEvent( UINT uTimerID);
__declspec(dllimport) MMRESULT __stdcall timeGetDevCaps( LPTIMECAPS ptc, UINT cbtc);
__declspec(dllimport) MMRESULT __stdcall timeBeginPeriod( UINT uPeriod);
__declspec(dllimport) MMRESULT __stdcall timeEndPeriod( UINT uPeriod);
typedef struct tagJOYCAPSA {
 WORD wMid;
 WORD wPid;
 CHAR szPname[32];
 UINT wXmin;
 UINT wXmax;
 UINT wYmin;
 UINT wYmax;
 UINT wZmin;
 UINT wZmax;
 UINT wNumButtons;
 UINT wPeriodMin;
 UINT wPeriodMax;
 UINT wRmin;
 UINT wRmax;
 UINT wUmin;
 UINT wUmax;
 UINT wVmin;
 UINT wVmax;
 UINT wCaps;
 UINT wMaxAxes;
 UINT wNumAxes;
 UINT wMaxButtons;
 CHAR szRegKey[32];
 CHAR szOEMVxD[260];
} JOYCAPSA, *PJOYCAPSA, *NPJOYCAPSA, *LPJOYCAPSA;
typedef struct tagJOYCAPSW {
 WORD wMid;
 WORD wPid;
 WCHAR szPname[32];
 UINT wXmin;
 UINT wXmax;
 UINT wYmin;
 UINT wYmax;
 UINT wZmin;
 UINT wZmax;
 UINT wNumButtons;
 UINT wPeriodMin;
 UINT wPeriodMax;
 UINT wRmin;
 UINT wRmax;
 UINT wUmin;
 UINT wUmax;
 UINT wVmin;
 UINT wVmax;
 UINT wCaps;
 UINT wMaxAxes;
 UINT wNumAxes;
 UINT wMaxButtons;
 WCHAR szRegKey[32];
 WCHAR szOEMVxD[260];
} JOYCAPSW, *PJOYCAPSW, *NPJOYCAPSW, *LPJOYCAPSW;
typedef JOYCAPSA JOYCAPS;
typedef PJOYCAPSA PJOYCAPS;
typedef NPJOYCAPSA NPJOYCAPS;
typedef LPJOYCAPSA LPJOYCAPS;
typedef struct tagJOYCAPS2A {
 WORD wMid;
 WORD wPid;
 CHAR szPname[32];
 UINT wXmin;
 UINT wXmax;
 UINT wYmin;
 UINT wYmax;
 UINT wZmin;
 UINT wZmax;
 UINT wNumButtons;
 UINT wPeriodMin;
 UINT wPeriodMax;
 UINT wRmin;
 UINT wRmax;
 UINT wUmin;
 UINT wUmax;
 UINT wVmin;
 UINT wVmax;
 UINT wCaps;
 UINT wMaxAxes;
 UINT wNumAxes;
 UINT wMaxButtons;
 CHAR szRegKey[32];
 CHAR szOEMVxD[260];
 GUID ManufacturerGuid;
 GUID ProductGuid;
 GUID NameGuid;
} JOYCAPS2A, *PJOYCAPS2A, *NPJOYCAPS2A, *LPJOYCAPS2A;
typedef struct tagJOYCAPS2W {
 WORD wMid;
 WORD wPid;
 WCHAR szPname[32];
 UINT wXmin;
 UINT wXmax;
 UINT wYmin;
 UINT wYmax;
 UINT wZmin;
 UINT wZmax;
 UINT wNumButtons;
 UINT wPeriodMin;
 UINT wPeriodMax;
 UINT wRmin;
 UINT wRmax;
 UINT wUmin;
 UINT wUmax;
 UINT wVmin;
 UINT wVmax; 
 UINT wCaps;
 UINT wMaxAxes;
 UINT wNumAxes;
 UINT wMaxButtons;
 WCHAR szRegKey[32];
 WCHAR szOEMVxD[260];
 GUID ManufacturerGuid;
 GUID ProductGuid;
 GUID NameGuid;
} JOYCAPS2W, *PJOYCAPS2W, *NPJOYCAPS2W, *LPJOYCAPS2W;
typedef JOYCAPS2A JOYCAPS2;
typedef PJOYCAPS2A PJOYCAPS2;
typedef NPJOYCAPS2A NPJOYCAPS2;
typedef LPJOYCAPS2A LPJOYCAPS2;
typedef struct joyinfo_tag {
 UINT wXpos;
 UINT wYpos;
 UINT wZpos;
 UINT wButtons;
} JOYINFO, *PJOYINFO, *NPJOYINFO, *LPJOYINFO;
typedef struct joyinfoex_tag {
 DWORD dwSize;
 DWORD dwFlags;
 DWORD dwXpos;
 DWORD dwYpos;
 DWORD dwZpos;
 DWORD dwRpos;
 DWORD dwUpos;
 DWORD dwVpos;
 DWORD dwButtons;
 DWORD dwButtonNumber;
 DWORD dwPOV;
 DWORD dwReserved1;
 DWORD dwReserved2;
} JOYINFOEX, *PJOYINFOEX, *NPJOYINFOEX, *LPJOYINFOEX;
__declspec(dllimport) UINT __stdcall joyGetNumDevs(void);
__declspec(dllimport) MMRESULT __stdcall joyGetDevCapsA( UINT_PTR uJoyID, LPJOYCAPSA pjc, UINT cbjc);
__declspec(dllimport) MMRESULT __stdcall joyGetDevCapsW( UINT_PTR uJoyID, LPJOYCAPSW pjc, UINT cbjc);
__declspec(dllimport) MMRESULT __stdcall joyGetPos( UINT uJoyID, LPJOYINFO pji);
__declspec(dllimport) MMRESULT __stdcall joyGetPosEx( UINT uJoyID, LPJOYINFOEX pji);
__declspec(dllimport) MMRESULT __stdcall joyGetThreshold( UINT uJoyID, LPUINT puThreshold);
__declspec(dllimport) MMRESULT __stdcall joyReleaseCapture( UINT uJoyID);
__declspec(dllimport) MMRESULT __stdcall joySetCapture( HWND hwnd, UINT uJoyID, UINT uPeriod,
 BOOL fChanged);
__declspec(dllimport) MMRESULT __stdcall joySetThreshold( UINT uJoyID, UINT uThreshold);
typedef DWORD FOURCC;
typedef char * HPSTR;
struct HMMIO__ { int unused; }; typedef struct HMMIO__ *HMMIO;
typedef LRESULT (__stdcall MMIOPROC)(LPSTR lpmmioinfo, UINT uMsg,
 LPARAM lParam1, LPARAM lParam2);
typedef MMIOPROC *LPMMIOPROC;
typedef struct _MMIOINFO
{
 DWORD dwFlags;
 FOURCC fccIOProc;
 LPMMIOPROC pIOProc;
 UINT wErrorRet;
 HTASK htask;
 LONG cchBuffer;
 HPSTR pchBuffer;
 HPSTR pchNext;
 HPSTR pchEndRead;
 HPSTR pchEndWrite;
 LONG lBufOffset;
 LONG lDiskOffset;
 DWORD adwInfo[3];
 DWORD dwReserved1;
 DWORD dwReserved2;
 HMMIO hmmio;
} MMIOINFO, *PMMIOINFO, *NPMMIOINFO, *LPMMIOINFO;
typedef const MMIOINFO *LPCMMIOINFO;
typedef struct _MMCKINFO
{
 FOURCC ckid;
 DWORD cksize;
 FOURCC fccType;
 DWORD dwDataOffset;
 DWORD dwFlags;
} MMCKINFO, *PMMCKINFO, *NPMMCKINFO, *LPMMCKINFO;
typedef const MMCKINFO *LPCMMCKINFO;
__declspec(dllimport) FOURCC __stdcall mmioStringToFOURCCA( LPCSTR sz, UINT uFlags);
__declspec(dllimport) FOURCC __stdcall mmioStringToFOURCCW( LPCWSTR sz, UINT uFlags);
__declspec(dllimport) LPMMIOPROC __stdcall mmioInstallIOProcA( FOURCC fccIOProc, LPMMIOPROC pIOProc, DWORD dwFlags);
__declspec(dllimport) LPMMIOPROC __stdcall mmioInstallIOProcW( FOURCC fccIOProc, LPMMIOPROC pIOProc, DWORD dwFlags);
 __declspec(dllimport) HMMIO __stdcall mmioOpenA( LPSTR pszFileName, LPMMIOINFO pmmioinfo, DWORD fdwOpen);
 __declspec(dllimport) HMMIO __stdcall mmioOpenW( LPWSTR pszFileName, LPMMIOINFO pmmioinfo, DWORD fdwOpen);
__declspec(dllimport) MMRESULT __stdcall mmioRenameA( LPCSTR pszFileName, LPCSTR pszNewFileName, LPCMMIOINFO pmmioinfo, DWORD fdwRename);
__declspec(dllimport) MMRESULT __stdcall mmioRenameW( LPCWSTR pszFileName, LPCWSTR pszNewFileName, LPCMMIOINFO pmmioinfo, DWORD fdwRename);
__declspec(dllimport) MMRESULT __stdcall mmioClose( HMMIO hmmio, UINT fuClose);
__declspec(dllimport) LONG __stdcall mmioRead( HMMIO hmmio, HPSTR pch, LONG cch);
__declspec(dllimport) LONG __stdcall mmioWrite( HMMIO hmmio, const char * pch, LONG cch);
__declspec(dllimport) LONG __stdcall mmioSeek( HMMIO hmmio, LONG lOffset, int iOrigin);
__declspec(dllimport) MMRESULT __stdcall mmioGetInfo( HMMIO hmmio, LPMMIOINFO pmmioinfo, UINT fuInfo);
__declspec(dllimport) MMRESULT __stdcall mmioSetInfo( HMMIO hmmio, LPCMMIOINFO pmmioinfo, UINT fuInfo);
__declspec(dllimport) MMRESULT __stdcall mmioSetBuffer( HMMIO hmmio, LPSTR pchBuffer, LONG cchBuffer,
 UINT fuBuffer);
__declspec(dllimport) MMRESULT __stdcall mmioFlush( HMMIO hmmio, UINT fuFlush);
__declspec(dllimport) MMRESULT __stdcall mmioAdvance( HMMIO hmmio, LPMMIOINFO pmmioinfo, UINT fuAdvance);
__declspec(dllimport) LRESULT __stdcall mmioSendMessage( HMMIO hmmio, UINT uMsg,
 LPARAM lParam1, LPARAM lParam2);
__declspec(dllimport) MMRESULT __stdcall mmioDescend( HMMIO hmmio, LPMMCKINFO pmmcki,
 const MMCKINFO * pmmckiParent, UINT fuDescend);
__declspec(dllimport) MMRESULT __stdcall mmioAscend( HMMIO hmmio, LPMMCKINFO pmmcki, UINT fuAscend);
__declspec(dllimport) MMRESULT __stdcall mmioCreateChunk( HMMIO hmmio, LPMMCKINFO pmmcki, UINT fuCreate);
typedef DWORD MCIERROR;
typedef UINT MCIDEVICEID;
typedef UINT (__stdcall *YIELDPROC)(MCIDEVICEID mciId, DWORD dwYieldData);
__declspec(dllimport) MCIERROR __stdcall mciSendCommandA( MCIDEVICEID mciId, UINT uMsg, DWORD_PTR dwParam1, DWORD_PTR dwParam2);
__declspec(dllimport) MCIERROR __stdcall mciSendCommandW( MCIDEVICEID mciId, UINT uMsg, DWORD_PTR dwParam1, DWORD_PTR dwParam2);
__declspec(dllimport) MCIERROR __stdcall mciSendStringA( LPCSTR lpstrCommand, LPSTR lpstrReturnString, UINT uReturnLength, HWND hwndCallback);
__declspec(dllimport) MCIERROR __stdcall mciSendStringW( LPCWSTR lpstrCommand, LPWSTR lpstrReturnString, UINT uReturnLength, HWND hwndCallback);
__declspec(dllimport) MCIDEVICEID __stdcall mciGetDeviceIDA( LPCSTR pszDevice);
__declspec(dllimport) MCIDEVICEID __stdcall mciGetDeviceIDW( LPCWSTR pszDevice);
__declspec(dllimport) MCIDEVICEID __stdcall mciGetDeviceIDFromElementIDA( DWORD dwElementID, LPCSTR lpstrType );
__declspec(dllimport) MCIDEVICEID __stdcall mciGetDeviceIDFromElementIDW( DWORD dwElementID, LPCWSTR lpstrType );
__declspec(dllimport) BOOL __stdcall mciGetErrorStringA( MCIERROR mcierr, LPSTR pszText, UINT cchText);
__declspec(dllimport) BOOL __stdcall mciGetErrorStringW( MCIERROR mcierr, LPWSTR pszText, UINT cchText);
__declspec(dllimport) BOOL __stdcall mciSetYieldProc( MCIDEVICEID mciId, YIELDPROC fpYieldProc,
 DWORD dwYieldData);
__declspec(dllimport) HTASK __stdcall mciGetCreatorTask( MCIDEVICEID mciId);
__declspec(dllimport) YIELDPROC __stdcall mciGetYieldProc( MCIDEVICEID mciId, LPDWORD pdwYieldData);
typedef struct tagMCI_GENERIC_PARMS {
 DWORD_PTR dwCallback;
} MCI_GENERIC_PARMS, *PMCI_GENERIC_PARMS, *LPMCI_GENERIC_PARMS;
typedef struct tagMCI_OPEN_PARMSA {
 DWORD_PTR dwCallback;
 MCIDEVICEID wDeviceID;
 LPCSTR lpstrDeviceType;
 LPCSTR lpstrElementName;
 LPCSTR lpstrAlias;
} MCI_OPEN_PARMSA, *PMCI_OPEN_PARMSA, *LPMCI_OPEN_PARMSA;
typedef struct tagMCI_OPEN_PARMSW {
 DWORD_PTR dwCallback;
 MCIDEVICEID wDeviceID;
 LPCWSTR lpstrDeviceType;
 LPCWSTR lpstrElementName;
 LPCWSTR lpstrAlias;
} MCI_OPEN_PARMSW, *PMCI_OPEN_PARMSW, *LPMCI_OPEN_PARMSW;
typedef MCI_OPEN_PARMSA MCI_OPEN_PARMS;
typedef PMCI_OPEN_PARMSA PMCI_OPEN_PARMS;
typedef LPMCI_OPEN_PARMSA LPMCI_OPEN_PARMS;
typedef struct tagMCI_PLAY_PARMS {
 DWORD_PTR dwCallback;
 DWORD dwFrom;
 DWORD dwTo;
} MCI_PLAY_PARMS, *PMCI_PLAY_PARMS, *LPMCI_PLAY_PARMS;
typedef struct tagMCI_SEEK_PARMS {
 DWORD_PTR dwCallback;
 DWORD dwTo;
} MCI_SEEK_PARMS, *PMCI_SEEK_PARMS, *LPMCI_SEEK_PARMS;
typedef struct tagMCI_STATUS_PARMS {
 DWORD_PTR dwCallback;
 DWORD_PTR dwReturn;
 DWORD dwItem;
 DWORD dwTrack;
} MCI_STATUS_PARMS, *PMCI_STATUS_PARMS, * LPMCI_STATUS_PARMS;
typedef struct tagMCI_INFO_PARMSA {
 DWORD_PTR dwCallback;
 LPSTR lpstrReturn;
 DWORD dwRetSize;
} MCI_INFO_PARMSA, * LPMCI_INFO_PARMSA;
typedef struct tagMCI_INFO_PARMSW {
 DWORD_PTR dwCallback;
 LPWSTR lpstrReturn;
 DWORD dwRetSize;
} MCI_INFO_PARMSW, * LPMCI_INFO_PARMSW;
typedef MCI_INFO_PARMSA MCI_INFO_PARMS;
typedef LPMCI_INFO_PARMSA LPMCI_INFO_PARMS;
typedef struct tagMCI_GETDEVCAPS_PARMS {
 DWORD_PTR dwCallback;
 DWORD dwReturn;
 DWORD dwItem;
} MCI_GETDEVCAPS_PARMS, *PMCI_GETDEVCAPS_PARMS, * LPMCI_GETDEVCAPS_PARMS;
typedef struct tagMCI_SYSINFO_PARMSA {
 DWORD_PTR dwCallback;
 LPSTR lpstrReturn;
 DWORD dwRetSize;
 DWORD dwNumber;
 UINT wDeviceType;
} MCI_SYSINFO_PARMSA, *PMCI_SYSINFO_PARMSA, * LPMCI_SYSINFO_PARMSA;
typedef struct tagMCI_SYSINFO_PARMSW {
 DWORD_PTR dwCallback;
 LPWSTR lpstrReturn;
 DWORD dwRetSize;
 DWORD dwNumber;
 UINT wDeviceType;
} MCI_SYSINFO_PARMSW, *PMCI_SYSINFO_PARMSW, * LPMCI_SYSINFO_PARMSW;
typedef MCI_SYSINFO_PARMSA MCI_SYSINFO_PARMS;
typedef PMCI_SYSINFO_PARMSA PMCI_SYSINFO_PARMS;
typedef LPMCI_SYSINFO_PARMSA LPMCI_SYSINFO_PARMS;
typedef struct tagMCI_SET_PARMS {
 DWORD_PTR dwCallback;
 DWORD dwTimeFormat;
 DWORD dwAudio;
} MCI_SET_PARMS, *PMCI_SET_PARMS, *LPMCI_SET_PARMS;
typedef struct tagMCI_BREAK_PARMS {
 DWORD_PTR dwCallback;
 int nVirtKey;
 HWND hwndBreak;
} MCI_BREAK_PARMS, *PMCI_BREAK_PARMS, * LPMCI_BREAK_PARMS;
typedef struct tagMCI_SAVE_PARMSA {
 DWORD_PTR dwCallback;
 LPCSTR lpfilename;
} MCI_SAVE_PARMSA, *PMCI_SAVE_PARMSA, * LPMCI_SAVE_PARMSA;
typedef struct tagMCI_SAVE_PARMSW {
 DWORD_PTR dwCallback;
 LPCWSTR lpfilename;
} MCI_SAVE_PARMSW, *PMCI_SAVE_PARMSW, * LPMCI_SAVE_PARMSW;
typedef MCI_SAVE_PARMSA MCI_SAVE_PARMS;
typedef PMCI_SAVE_PARMSA PMCI_SAVE_PARMS;
typedef LPMCI_SAVE_PARMSA LPMCI_SAVE_PARMS;
typedef struct tagMCI_LOAD_PARMSA {
 DWORD_PTR dwCallback;
 LPCSTR lpfilename;
} MCI_LOAD_PARMSA, *PMCI_LOAD_PARMSA, * LPMCI_LOAD_PARMSA;
typedef struct tagMCI_LOAD_PARMSW {
 DWORD_PTR dwCallback;
 LPCWSTR lpfilename;
} MCI_LOAD_PARMSW, *PMCI_LOAD_PARMSW, * LPMCI_LOAD_PARMSW;
typedef MCI_LOAD_PARMSA MCI_LOAD_PARMS;
typedef PMCI_LOAD_PARMSA PMCI_LOAD_PARMS;
typedef LPMCI_LOAD_PARMSA LPMCI_LOAD_PARMS;
typedef struct tagMCI_RECORD_PARMS {
 DWORD_PTR dwCallback;
 DWORD dwFrom;
 DWORD dwTo;
} MCI_RECORD_PARMS, *LPMCI_RECORD_PARMS;
typedef struct tagMCI_VD_PLAY_PARMS {
 DWORD_PTR dwCallback;
 DWORD dwFrom;
 DWORD dwTo;
 DWORD dwSpeed;
} MCI_VD_PLAY_PARMS, *PMCI_VD_PLAY_PARMS, *LPMCI_VD_PLAY_PARMS;
typedef struct tagMCI_VD_STEP_PARMS {
 DWORD_PTR dwCallback;
 DWORD dwFrames;
} MCI_VD_STEP_PARMS, *PMCI_VD_STEP_PARMS, *LPMCI_VD_STEP_PARMS;
typedef struct tagMCI_VD_ESCAPE_PARMSA {
 DWORD_PTR dwCallback;
 LPCSTR lpstrCommand;
} MCI_VD_ESCAPE_PARMSA, *PMCI_VD_ESCAPE_PARMSA, *LPMCI_VD_ESCAPE_PARMSA;
typedef struct tagMCI_VD_ESCAPE_PARMSW {
 DWORD_PTR dwCallback;
 LPCWSTR lpstrCommand;
} MCI_VD_ESCAPE_PARMSW, *PMCI_VD_ESCAPE_PARMSW, *LPMCI_VD_ESCAPE_PARMSW;
typedef MCI_VD_ESCAPE_PARMSA MCI_VD_ESCAPE_PARMS;
typedef PMCI_VD_ESCAPE_PARMSA PMCI_VD_ESCAPE_PARMS;
typedef LPMCI_VD_ESCAPE_PARMSA LPMCI_VD_ESCAPE_PARMS;
typedef struct tagMCI_WAVE_OPEN_PARMSA {
 DWORD_PTR dwCallback;
 MCIDEVICEID wDeviceID;
 LPCSTR lpstrDeviceType;
 LPCSTR lpstrElementName;
 LPCSTR lpstrAlias;
 DWORD dwBufferSeconds;
} MCI_WAVE_OPEN_PARMSA, *PMCI_WAVE_OPEN_PARMSA, *LPMCI_WAVE_OPEN_PARMSA;
typedef struct tagMCI_WAVE_OPEN_PARMSW {
 DWORD_PTR dwCallback;
 MCIDEVICEID wDeviceID;
 LPCWSTR lpstrDeviceType;
 LPCWSTR lpstrElementName;
 LPCWSTR lpstrAlias;
 DWORD dwBufferSeconds;
} MCI_WAVE_OPEN_PARMSW, *PMCI_WAVE_OPEN_PARMSW, *LPMCI_WAVE_OPEN_PARMSW;
typedef MCI_WAVE_OPEN_PARMSA MCI_WAVE_OPEN_PARMS;
typedef PMCI_WAVE_OPEN_PARMSA PMCI_WAVE_OPEN_PARMS;
typedef LPMCI_WAVE_OPEN_PARMSA LPMCI_WAVE_OPEN_PARMS;
typedef struct tagMCI_WAVE_DELETE_PARMS {
 DWORD_PTR dwCallback;
 DWORD dwFrom;
 DWORD dwTo;
} MCI_WAVE_DELETE_PARMS, *PMCI_WAVE_DELETE_PARMS, *LPMCI_WAVE_DELETE_PARMS;
typedef struct tagMCI_WAVE_SET_PARMS {
 DWORD_PTR dwCallback;
 DWORD dwTimeFormat;
 DWORD dwAudio;
 UINT wInput;
 UINT wOutput;
 WORD wFormatTag;
 WORD wReserved2;
 WORD nChannels;
 WORD wReserved3;
 DWORD nSamplesPerSec;
 DWORD nAvgBytesPerSec;
 WORD nBlockAlign;
 WORD wReserved4;
 WORD wBitsPerSample;
 WORD wReserved5;
} MCI_WAVE_SET_PARMS, *PMCI_WAVE_SET_PARMS, * LPMCI_WAVE_SET_PARMS;
typedef struct tagMCI_SEQ_SET_PARMS {
 DWORD_PTR dwCallback;
 DWORD dwTimeFormat;
 DWORD dwAudio;
 DWORD dwTempo;
 DWORD dwPort;
 DWORD dwSlave;
 DWORD dwMaster;
 DWORD dwOffset;
} MCI_SEQ_SET_PARMS, *PMCI_SEQ_SET_PARMS, * LPMCI_SEQ_SET_PARMS;
typedef struct tagMCI_ANIM_OPEN_PARMSA {
 DWORD_PTR dwCallback;
 MCIDEVICEID wDeviceID;
 LPCSTR lpstrDeviceType;
 LPCSTR lpstrElementName;
 LPCSTR lpstrAlias;
 DWORD dwStyle;
 HWND hWndParent;
} MCI_ANIM_OPEN_PARMSA, *PMCI_ANIM_OPEN_PARMSA, *LPMCI_ANIM_OPEN_PARMSA;
typedef struct tagMCI_ANIM_OPEN_PARMSW {
 DWORD_PTR dwCallback;
 MCIDEVICEID wDeviceID;
 LPCWSTR lpstrDeviceType;
 LPCWSTR lpstrElementName;
 LPCWSTR lpstrAlias;
 DWORD dwStyle;
 HWND hWndParent;
} MCI_ANIM_OPEN_PARMSW, *PMCI_ANIM_OPEN_PARMSW, *LPMCI_ANIM_OPEN_PARMSW;
typedef MCI_ANIM_OPEN_PARMSA MCI_ANIM_OPEN_PARMS;
typedef PMCI_ANIM_OPEN_PARMSA PMCI_ANIM_OPEN_PARMS;
typedef LPMCI_ANIM_OPEN_PARMSA LPMCI_ANIM_OPEN_PARMS;
typedef struct tagMCI_ANIM_PLAY_PARMS {
 DWORD_PTR dwCallback;
 DWORD dwFrom;
 DWORD dwTo;
 DWORD dwSpeed;
} MCI_ANIM_PLAY_PARMS, *PMCI_ANIM_PLAY_PARMS, *LPMCI_ANIM_PLAY_PARMS;
typedef struct tagMCI_ANIM_STEP_PARMS {
 DWORD_PTR dwCallback;
 DWORD dwFrames;
} MCI_ANIM_STEP_PARMS, *PMCI_ANIM_STEP_PARMS, *LPMCI_ANIM_STEP_PARMS;
typedef struct tagMCI_ANIM_WINDOW_PARMSA {
 DWORD_PTR dwCallback;
 HWND hWnd;
 UINT nCmdShow;
 LPCSTR lpstrText;
} MCI_ANIM_WINDOW_PARMSA, *PMCI_ANIM_WINDOW_PARMSA, * LPMCI_ANIM_WINDOW_PARMSA;
typedef struct tagMCI_ANIM_WINDOW_PARMSW {
 DWORD_PTR dwCallback;
 HWND hWnd;
 UINT nCmdShow;
 LPCWSTR lpstrText;
} MCI_ANIM_WINDOW_PARMSW, *PMCI_ANIM_WINDOW_PARMSW, * LPMCI_ANIM_WINDOW_PARMSW;
typedef MCI_ANIM_WINDOW_PARMSA MCI_ANIM_WINDOW_PARMS;
typedef PMCI_ANIM_WINDOW_PARMSA PMCI_ANIM_WINDOW_PARMS;
typedef LPMCI_ANIM_WINDOW_PARMSA LPMCI_ANIM_WINDOW_PARMS;
typedef struct tagMCI_ANIM_RECT_PARMS {
 DWORD_PTR dwCallback;
 RECT rc;
} MCI_ANIM_RECT_PARMS;
typedef MCI_ANIM_RECT_PARMS * PMCI_ANIM_RECT_PARMS;
typedef MCI_ANIM_RECT_PARMS * LPMCI_ANIM_RECT_PARMS;
typedef struct tagMCI_ANIM_UPDATE_PARMS {
 DWORD_PTR dwCallback;
 RECT rc;
 HDC hDC;
} MCI_ANIM_UPDATE_PARMS, *PMCI_ANIM_UPDATE_PARMS, * LPMCI_ANIM_UPDATE_PARMS;
typedef struct tagMCI_OVLY_OPEN_PARMSA {
 DWORD_PTR dwCallback;
 MCIDEVICEID wDeviceID;
 LPCSTR lpstrDeviceType;
 LPCSTR lpstrElementName;
 LPCSTR lpstrAlias;
 DWORD dwStyle;
 HWND hWndParent;
} MCI_OVLY_OPEN_PARMSA, *PMCI_OVLY_OPEN_PARMSA, *LPMCI_OVLY_OPEN_PARMSA;
typedef struct tagMCI_OVLY_OPEN_PARMSW {
 DWORD_PTR dwCallback;
 MCIDEVICEID wDeviceID;
 LPCWSTR lpstrDeviceType;
 LPCWSTR lpstrElementName;
 LPCWSTR lpstrAlias;
 DWORD dwStyle;
 HWND hWndParent;
} MCI_OVLY_OPEN_PARMSW, *PMCI_OVLY_OPEN_PARMSW, *LPMCI_OVLY_OPEN_PARMSW;
typedef MCI_OVLY_OPEN_PARMSA MCI_OVLY_OPEN_PARMS;
typedef PMCI_OVLY_OPEN_PARMSA PMCI_OVLY_OPEN_PARMS;
typedef LPMCI_OVLY_OPEN_PARMSA LPMCI_OVLY_OPEN_PARMS;
typedef struct tagMCI_OVLY_WINDOW_PARMSA {
 DWORD_PTR dwCallback;
 HWND hWnd;
 UINT nCmdShow;
 LPCSTR lpstrText;
} MCI_OVLY_WINDOW_PARMSA, *PMCI_OVLY_WINDOW_PARMSA, * LPMCI_OVLY_WINDOW_PARMSA;
typedef struct tagMCI_OVLY_WINDOW_PARMSW {
 DWORD_PTR dwCallback;
 HWND hWnd;
 UINT nCmdShow;
 LPCWSTR lpstrText;
} MCI_OVLY_WINDOW_PARMSW, *PMCI_OVLY_WINDOW_PARMSW, * LPMCI_OVLY_WINDOW_PARMSW;
typedef MCI_OVLY_WINDOW_PARMSA MCI_OVLY_WINDOW_PARMS;
typedef PMCI_OVLY_WINDOW_PARMSA PMCI_OVLY_WINDOW_PARMS;
typedef LPMCI_OVLY_WINDOW_PARMSA LPMCI_OVLY_WINDOW_PARMS;
typedef struct tagMCI_OVLY_RECT_PARMS {
 DWORD_PTR dwCallback;
 RECT rc;
} MCI_OVLY_RECT_PARMS, *PMCI_OVLY_RECT_PARMS, * LPMCI_OVLY_RECT_PARMS;
typedef struct tagMCI_OVLY_SAVE_PARMSA {
 DWORD_PTR dwCallback;
 LPCSTR lpfilename;
 RECT rc;
} MCI_OVLY_SAVE_PARMSA, *PMCI_OVLY_SAVE_PARMSA, * LPMCI_OVLY_SAVE_PARMSA;
typedef struct tagMCI_OVLY_SAVE_PARMSW {
 DWORD_PTR dwCallback;
 LPCWSTR lpfilename;
 RECT rc;
} MCI_OVLY_SAVE_PARMSW, *PMCI_OVLY_SAVE_PARMSW, * LPMCI_OVLY_SAVE_PARMSW;
typedef MCI_OVLY_SAVE_PARMSA MCI_OVLY_SAVE_PARMS;
typedef PMCI_OVLY_SAVE_PARMSA PMCI_OVLY_SAVE_PARMS;
typedef LPMCI_OVLY_SAVE_PARMSA LPMCI_OVLY_SAVE_PARMS;
typedef struct tagMCI_OVLY_LOAD_PARMSA {
 DWORD_PTR dwCallback;
 LPCSTR lpfilename;
 RECT rc;
} MCI_OVLY_LOAD_PARMSA, *PMCI_OVLY_LOAD_PARMSA, * LPMCI_OVLY_LOAD_PARMSA;
typedef struct tagMCI_OVLY_LOAD_PARMSW {
 DWORD_PTR dwCallback;
 LPCWSTR lpfilename;
 RECT rc;
} MCI_OVLY_LOAD_PARMSW, *PMCI_OVLY_LOAD_PARMSW, * LPMCI_OVLY_LOAD_PARMSW;
typedef MCI_OVLY_LOAD_PARMSA MCI_OVLY_LOAD_PARMS;
typedef PMCI_OVLY_LOAD_PARMSA PMCI_OVLY_LOAD_PARMS;
typedef LPMCI_OVLY_LOAD_PARMSA LPMCI_OVLY_LOAD_PARMS;
}
#pragma warning(disable:4103)
#pragma pack(pop)
extern "C" {
typedef HANDLE HDRAWDIB;
extern BOOL __stdcall DrawDibInit(void);
extern HDRAWDIB __stdcall DrawDibOpen(void);
extern
BOOL
__stdcall
DrawDibClose(
 HDRAWDIB hdd
 );
extern
LPVOID
__stdcall
DrawDibGetBuffer(
 HDRAWDIB hdd,
 LPBITMAPINFOHEADER lpbi,
 DWORD dwSize,
 DWORD dwFlags
 );
extern UINT __stdcall DrawDibError(HDRAWDIB hdd);
extern
HPALETTE
__stdcall
DrawDibGetPalette(
 HDRAWDIB hdd
 );
extern
BOOL
__stdcall
DrawDibSetPalette(
 HDRAWDIB hdd,
 HPALETTE hpal
 );
extern
BOOL
__stdcall
DrawDibChangePalette(
 HDRAWDIB hdd,
 int iStart,
 int iLen,
 LPPALETTEENTRY lppe
 );
extern
UINT
__stdcall
DrawDibRealize(
 HDRAWDIB hdd,
 HDC hdc,
 BOOL fBackground
 );
extern
BOOL
__stdcall
DrawDibStart(
 HDRAWDIB hdd,
 DWORD rate
 );
extern
BOOL
__stdcall
DrawDibStop(
 HDRAWDIB hdd
 );
extern
BOOL
__stdcall
DrawDibBegin(
 HDRAWDIB hdd,
 HDC hdc,
 int dxDst,
 int dyDst,
 LPBITMAPINFOHEADER lpbi,
 int dxSrc,
 int dySrc,
 UINT wFlags
 );
extern
BOOL
__stdcall
DrawDibDraw(
 HDRAWDIB hdd,
 HDC hdc,
 int xDst,
 int yDst,
 int dxDst,
 int dyDst,
 LPBITMAPINFOHEADER lpbi,
 LPVOID lpBits,
 int xSrc,
 int ySrc,
 int dxSrc,
 int dySrc,
 UINT wFlags
 );
extern
BOOL
__stdcall
DrawDibEnd(
 HDRAWDIB hdd
 );
typedef struct {
 LONG timeCount;
 LONG timeDraw;
 LONG timeDecompress;
 LONG timeDither;
 LONG timeStretch;
 LONG timeBlt;
 LONG timeSetDIBits;
} DRAWDIBTIME, *LPDRAWDIBTIME;
BOOL
__stdcall
DrawDibTime(
 HDRAWDIB hdd,
 LPDRAWDIBTIME lpddtime
 );
LRESULT
__stdcall
DrawDibProfileDisplay(
 LPBITMAPINFOHEADER lpbi
 );
#pragma warning(disable:4200)
typedef WORD TWOCC;
typedef struct
{
 DWORD dwMicroSecPerFrame;
 DWORD dwMaxBytesPerSec;
 DWORD dwPaddingGranularity;
 DWORD dwFlags;
 DWORD dwTotalFrames;
 DWORD dwInitialFrames;
 DWORD dwStreams;
 DWORD dwSuggestedBufferSize;
 DWORD dwWidth;
 DWORD dwHeight;
 DWORD dwReserved[4];
} MainAVIHeader;
typedef struct {
 FOURCC fccType;
 FOURCC fccHandler;
 DWORD dwFlags;
 WORD wPriority;
 WORD wLanguage;
 DWORD dwInitialFrames;
 DWORD dwScale;
 DWORD dwRate;
 DWORD dwStart;
 DWORD dwLength;
 DWORD dwSuggestedBufferSize;
 DWORD dwQuality;
 DWORD dwSampleSize;
 RECT rcFrame;
} AVIStreamHeader;
typedef struct
{
 DWORD ckid;
 DWORD dwFlags;
 DWORD dwChunkOffset;
 DWORD dwChunkLength;
} AVIINDEXENTRY;
typedef struct
{
 BYTE bFirstEntry;
 BYTE bNumEntries;
 WORD wFlags;
 PALETTEENTRY peNew[];
} AVIPALCHANGE;
}
#pragma warning(disable:4103)
#pragma pack(push,8)
extern "C" {
typedef struct _AVISTREAMINFOW {
 DWORD fccType;
 DWORD fccHandler;
 DWORD dwFlags;
 DWORD dwCaps;
 WORD wPriority;
 WORD wLanguage;
 DWORD dwScale;
 DWORD dwRate;
 DWORD dwStart;
 DWORD dwLength;
 DWORD dwInitialFrames;
 DWORD dwSuggestedBufferSize;
 DWORD dwQuality;
 DWORD dwSampleSize;
 RECT rcFrame;
 DWORD dwEditCount;
 DWORD dwFormatChangeCount;
 WCHAR szName[64];
} AVISTREAMINFOW, * LPAVISTREAMINFOW;
typedef struct _AVISTREAMINFOA {
 DWORD fccType;
 DWORD fccHandler;
 DWORD dwFlags;
 DWORD dwCaps;
 WORD wPriority;
 WORD wLanguage;
 DWORD dwScale;
 DWORD dwRate;
 DWORD dwStart;
 DWORD dwLength;
 DWORD dwInitialFrames;
 DWORD dwSuggestedBufferSize;
 DWORD dwQuality;
 DWORD dwSampleSize;
 RECT rcFrame;
 DWORD dwEditCount;
 DWORD dwFormatChangeCount;
 char szName[64];
} AVISTREAMINFOA, * LPAVISTREAMINFOA;
typedef struct _AVIFILEINFOW {
 DWORD dwMaxBytesPerSec;
 DWORD dwFlags;
 DWORD dwCaps;
 DWORD dwStreams;
 DWORD dwSuggestedBufferSize;
 DWORD dwWidth;
 DWORD dwHeight;
 DWORD dwScale;
 DWORD dwRate;
 DWORD dwLength;
 DWORD dwEditCount;
 WCHAR szFileType[64];
} AVIFILEINFOW, * LPAVIFILEINFOW;
typedef struct _AVIFILEINFOA {
 DWORD dwMaxBytesPerSec;
 DWORD dwFlags;
 DWORD dwCaps;
 DWORD dwStreams;
 DWORD dwSuggestedBufferSize;
 DWORD dwWidth;
 DWORD dwHeight;
 DWORD dwScale;
 DWORD dwRate;
 DWORD dwLength;
 DWORD dwEditCount;
 char szFileType[64];
} AVIFILEINFOA, * LPAVIFILEINFOA;
typedef BOOL ( __stdcall * AVISAVECALLBACK)(int);
typedef struct {
 DWORD fccType;
 DWORD fccHandler;
 DWORD dwKeyFrameEvery;
 DWORD dwQuality;
 DWORD dwBytesPerSecond;
 DWORD dwFlags;
 LPVOID lpFormat;
 DWORD cbFormat;
 LPVOID lpParms;
 DWORD cbParms;
 DWORD dwInterleaveEvery;
} AVICOMPRESSOPTIONS, *LPAVICOMPRESSOPTIONS;
}
#pragma once
#pragma warning(disable:4103)
#pragma pack(push,8)
#pragma once
extern "C" {
typedef void * I_RPC_HANDLE;
typedef long RPC_STATUS;
#pragma once
extern "C" {
typedef unsigned char * RPC_CSTR;
typedef unsigned short * RPC_WSTR;
typedef I_RPC_HANDLE RPC_BINDING_HANDLE;
typedef RPC_BINDING_HANDLE handle_t;
typedef GUID UUID;
typedef struct _RPC_BINDING_VECTOR
{
 unsigned long Count;
 RPC_BINDING_HANDLE BindingH[1];
} RPC_BINDING_VECTOR;
typedef struct _UUID_VECTOR
{
 unsigned long Count;
 UUID *Uuid[1];
} UUID_VECTOR;
typedef void * RPC_IF_HANDLE;
typedef struct _RPC_IF_ID
{
 UUID Uuid;
 unsigned short VersMajor;
 unsigned short VersMinor;
} RPC_IF_ID;
typedef struct _RPC_PROTSEQ_VECTORA
{
 unsigned int Count;
 unsigned char * Protseq[1];
} RPC_PROTSEQ_VECTORA;
typedef struct _RPC_PROTSEQ_VECTORW
{
 unsigned int Count;
 unsigned short * Protseq[1];
} RPC_PROTSEQ_VECTORW;
typedef struct _RPC_POLICY {
 unsigned int Length ;
 unsigned long EndpointFlags ;
 unsigned long NICFlags ;
 } RPC_POLICY, *PRPC_POLICY ;
typedef void __stdcall
RPC_OBJECT_INQ_FN (
 UUID * ObjectUuid,
 UUID * TypeUuid,
 RPC_STATUS * Status
 );
typedef RPC_STATUS __stdcall
RPC_IF_CALLBACK_FN (
 RPC_IF_HANDLE InterfaceUuid,
 void *Context
 ) ;
typedef void __stdcall
RPC_SECURITY_CALLBACK_FN (
 void *Context
 ) ;
typedef struct
{
 unsigned int Count;
 unsigned long Stats[1];
} RPC_STATS_VECTOR;
typedef struct
{
 unsigned long Count;
 RPC_IF_ID * IfId[1];
} RPC_IF_ID_VECTOR;
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingCopy (
 RPC_BINDING_HANDLE SourceBinding,
 RPC_BINDING_HANDLE * DestinationBinding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingFree (
 RPC_BINDING_HANDLE * Binding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingSetOption (
 RPC_BINDING_HANDLE hBinding,
 unsigned long option,
 ULONG_PTR optionValue
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingInqOption (
 RPC_BINDING_HANDLE hBinding,
 unsigned long option,
 ULONG_PTR *pOptionValue
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingFromStringBindingA (
 RPC_CSTR StringBinding,
 RPC_BINDING_HANDLE * Binding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingFromStringBindingW (
 RPC_WSTR StringBinding,
 RPC_BINDING_HANDLE * Binding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcSsGetContextBinding (
 void *ContextHandle,
 RPC_BINDING_HANDLE * Binding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingInqObject (
 RPC_BINDING_HANDLE Binding,
 UUID * ObjectUuid
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingReset (
 RPC_BINDING_HANDLE Binding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingSetObject (
 RPC_BINDING_HANDLE Binding,
 UUID * ObjectUuid
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtInqDefaultProtectLevel (
 unsigned long AuthnSvc,
 unsigned long *AuthnLevel
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingToStringBindingA (
 RPC_BINDING_HANDLE Binding,
 RPC_CSTR * StringBinding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingToStringBindingW (
 RPC_BINDING_HANDLE Binding,
 RPC_WSTR * StringBinding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingVectorFree (
 RPC_BINDING_VECTOR * * BindingVector
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcStringBindingComposeA (
 RPC_CSTR ObjUuid,
 RPC_CSTR ProtSeq,
 RPC_CSTR NetworkAddr,
 RPC_CSTR Endpoint,
 RPC_CSTR Options,
 RPC_CSTR * StringBinding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcStringBindingComposeW (
 RPC_WSTR ObjUuid,
 RPC_WSTR ProtSeq,
 RPC_WSTR NetworkAddr,
 RPC_WSTR Endpoint,
 RPC_WSTR Options,
 RPC_WSTR * StringBinding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcStringBindingParseA (
 RPC_CSTR StringBinding,
 RPC_CSTR * ObjUuid,
 RPC_CSTR * Protseq,
 RPC_CSTR * NetworkAddr,
 RPC_CSTR * Endpoint,
 RPC_CSTR * NetworkOptions
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcStringBindingParseW (
 RPC_WSTR StringBinding,
 RPC_WSTR * ObjUuid,
 RPC_WSTR * Protseq,
 RPC_WSTR * NetworkAddr,
 RPC_WSTR * Endpoint,
 RPC_WSTR * NetworkOptions
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcStringFreeA (
 RPC_CSTR * String
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcStringFreeW (
 RPC_WSTR * String
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcIfInqId (
 RPC_IF_HANDLE RpcIfHandle,
 RPC_IF_ID * RpcIfId
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcNetworkIsProtseqValidA (
 RPC_CSTR Protseq
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcNetworkIsProtseqValidW (
 RPC_WSTR Protseq
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtInqComTimeout (
 RPC_BINDING_HANDLE Binding,
 unsigned int * Timeout
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtSetComTimeout (
 RPC_BINDING_HANDLE Binding,
 unsigned int Timeout
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtSetCancelTimeout(
 long Timeout
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcNetworkInqProtseqsA (
 RPC_PROTSEQ_VECTORA * * ProtseqVector
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcNetworkInqProtseqsW (
 RPC_PROTSEQ_VECTORW * * ProtseqVector
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcObjectInqType (
 UUID * ObjUuid,
 UUID * TypeUuid
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcObjectSetInqFn (
 RPC_OBJECT_INQ_FN * InquiryFn
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcObjectSetType (
 UUID * ObjUuid,
 UUID * TypeUuid
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcProtseqVectorFreeA (
 RPC_PROTSEQ_VECTORA * * ProtseqVector
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcProtseqVectorFreeW (
 RPC_PROTSEQ_VECTORW * * ProtseqVector
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerInqBindings (
 RPC_BINDING_VECTOR * * BindingVector
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerInqIf (
 RPC_IF_HANDLE IfSpec,
 UUID * MgrTypeUuid,
 void * * MgrEpv
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerListen (
 unsigned int MinimumCallThreads,
 unsigned int MaxCalls,
 unsigned int DontWait
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerRegisterIf (
 RPC_IF_HANDLE IfSpec,
 UUID * MgrTypeUuid,
 void * MgrEpv
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerRegisterIfEx (
 RPC_IF_HANDLE IfSpec,
 UUID * MgrTypeUuid,
 void * MgrEpv,
 unsigned int Flags,
 unsigned int MaxCalls,
 RPC_IF_CALLBACK_FN *IfCallback
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerRegisterIf2 (
 RPC_IF_HANDLE IfSpec,
 UUID * MgrTypeUuid,
 void * MgrEpv,
 unsigned int Flags,
 unsigned int MaxCalls,
 unsigned int MaxRpcSize,
 RPC_IF_CALLBACK_FN *IfCallbackFn
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUnregisterIf (
 RPC_IF_HANDLE IfSpec,
 UUID * MgrTypeUuid,
 unsigned int WaitForCallsToComplete
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUnregisterIfEx (
 RPC_IF_HANDLE IfSpec,
 UUID * MgrTypeUuid,
 int RundownContextHandles
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseAllProtseqs (
 unsigned int MaxCalls,
 void * SecurityDescriptor
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseAllProtseqsEx (
 unsigned int MaxCalls,
 void * SecurityDescriptor,
 PRPC_POLICY Policy
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseAllProtseqsIf (
 unsigned int MaxCalls,
 RPC_IF_HANDLE IfSpec,
 void * SecurityDescriptor
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseAllProtseqsIfEx (
 unsigned int MaxCalls,
 RPC_IF_HANDLE IfSpec,
 void * SecurityDescriptor,
 PRPC_POLICY Policy
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseProtseqA (
 RPC_CSTR Protseq,
 unsigned int MaxCalls,
 void * SecurityDescriptor
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseProtseqExA (
 RPC_CSTR Protseq,
 unsigned int MaxCalls,
 void * SecurityDescriptor,
 PRPC_POLICY Policy
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseProtseqW (
 RPC_WSTR Protseq,
 unsigned int MaxCalls,
 void * SecurityDescriptor
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseProtseqExW (
 RPC_WSTR Protseq,
 unsigned int MaxCalls,
 void * SecurityDescriptor,
 PRPC_POLICY Policy
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseProtseqEpA (
 RPC_CSTR Protseq,
 unsigned int MaxCalls,
 RPC_CSTR Endpoint,
 void * SecurityDescriptor
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseProtseqEpExA (
 RPC_CSTR Protseq,
 unsigned int MaxCalls,
 RPC_CSTR Endpoint,
 void * SecurityDescriptor,
 PRPC_POLICY Policy
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseProtseqEpW (
 RPC_WSTR Protseq,
 unsigned int MaxCalls,
 RPC_WSTR Endpoint,
 void * SecurityDescriptor
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseProtseqEpExW (
 RPC_WSTR Protseq,
 unsigned int MaxCalls,
 RPC_WSTR Endpoint,
 void * SecurityDescriptor,
 PRPC_POLICY Policy
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseProtseqIfA (
 RPC_CSTR Protseq,
 unsigned int MaxCalls,
 RPC_IF_HANDLE IfSpec,
 void * SecurityDescriptor
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseProtseqIfExA (
 RPC_CSTR Protseq,
 unsigned int MaxCalls,
 RPC_IF_HANDLE IfSpec,
 void * SecurityDescriptor,
 PRPC_POLICY Policy
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseProtseqIfW (
 RPC_WSTR Protseq,
 unsigned int MaxCalls,
 RPC_IF_HANDLE IfSpec,
 void * SecurityDescriptor
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUseProtseqIfExW (
 RPC_WSTR Protseq,
 unsigned int MaxCalls,
 RPC_IF_HANDLE IfSpec,
 void * SecurityDescriptor,
 PRPC_POLICY Policy
 );
__declspec(dllimport)
void
__stdcall
RpcServerYield (
 void
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtStatsVectorFree (
 RPC_STATS_VECTOR ** StatsVector
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtInqStats (
 RPC_BINDING_HANDLE Binding,
 RPC_STATS_VECTOR ** Statistics
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtIsServerListening (
 RPC_BINDING_HANDLE Binding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtStopServerListening (
 RPC_BINDING_HANDLE Binding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtWaitServerListen (
 void
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtSetServerStackSize (
 unsigned long ThreadStackSize
 );
__declspec(dllimport)
void
__stdcall
RpcSsDontSerializeContext (
 void
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtEnableIdleCleanup (
 void
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtInqIfIds (
 RPC_BINDING_HANDLE Binding,
 RPC_IF_ID_VECTOR * * IfIdVector
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcIfIdVectorFree (
 RPC_IF_ID_VECTOR * * IfIdVector
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtInqServerPrincNameA (
 RPC_BINDING_HANDLE Binding,
 unsigned long AuthnSvc,
 RPC_CSTR * ServerPrincName
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtInqServerPrincNameW (
 RPC_BINDING_HANDLE Binding,
 unsigned long AuthnSvc,
 RPC_WSTR * ServerPrincName
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerInqDefaultPrincNameA (
 unsigned long AuthnSvc,
 RPC_CSTR * PrincName
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerInqDefaultPrincNameW (
 unsigned long AuthnSvc,
 RPC_WSTR * PrincName
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcEpResolveBinding (
 RPC_BINDING_HANDLE Binding,
 RPC_IF_HANDLE IfSpec
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcNsBindingInqEntryNameA (
 RPC_BINDING_HANDLE Binding,
 unsigned long EntryNameSyntax,
 RPC_CSTR * EntryName
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcNsBindingInqEntryNameW (
 RPC_BINDING_HANDLE Binding,
 unsigned long EntryNameSyntax,
 RPC_WSTR * EntryName
 );
typedef void * RPC_AUTH_IDENTITY_HANDLE;
typedef void * RPC_AUTHZ_HANDLE;
typedef struct _RPC_SECURITY_QOS {
 unsigned long Version;
 unsigned long Capabilities;
 unsigned long IdentityTracking;
 unsigned long ImpersonationType;
} RPC_SECURITY_QOS, *PRPC_SECURITY_QOS;
typedef struct _SEC_WINNT_AUTH_IDENTITY_W {
 unsigned short *User;
 unsigned long UserLength;
 unsigned short *Domain;
 unsigned long DomainLength;
 unsigned short *Password;
 unsigned long PasswordLength;
 unsigned long Flags;
} SEC_WINNT_AUTH_IDENTITY_W, *PSEC_WINNT_AUTH_IDENTITY_W;
typedef struct _SEC_WINNT_AUTH_IDENTITY_A {
 unsigned char *User;
 unsigned long UserLength;
 unsigned char *Domain;
 unsigned long DomainLength;
 unsigned char *Password;
 unsigned long PasswordLength;
 unsigned long Flags;
} SEC_WINNT_AUTH_IDENTITY_A, *PSEC_WINNT_AUTH_IDENTITY_A;
typedef struct _RPC_HTTP_TRANSPORT_CREDENTIALS_W
{
 SEC_WINNT_AUTH_IDENTITY_W *TransportCredentials;
 unsigned long Flags;
 unsigned long AuthenticationTarget;
 unsigned long NumberOfAuthnSchemes;
 unsigned long *AuthnSchemes;
 unsigned short *ServerCertificateSubject;
} RPC_HTTP_TRANSPORT_CREDENTIALS_W, *PRPC_HTTP_TRANSPORT_CREDENTIALS_W;
typedef struct _RPC_HTTP_TRANSPORT_CREDENTIALS_A
{
 SEC_WINNT_AUTH_IDENTITY_A *TransportCredentials;
 unsigned long Flags;
 unsigned long AuthenticationTarget;
 unsigned long NumberOfAuthnSchemes;
 unsigned long *AuthnSchemes;
 unsigned char *ServerCertificateSubject;
} RPC_HTTP_TRANSPORT_CREDENTIALS_A, *PRPC_HTTP_TRANSPORT_CREDENTIALS_A;
typedef struct _RPC_HTTP_TRANSPORT_CREDENTIALS_V2_W
{
 SEC_WINNT_AUTH_IDENTITY_W *TransportCredentials;
 unsigned long Flags;
 unsigned long AuthenticationTarget;
 unsigned long NumberOfAuthnSchemes;
 unsigned long *AuthnSchemes;
 unsigned short *ServerCertificateSubject;
 SEC_WINNT_AUTH_IDENTITY_W *ProxyCredentials;
 unsigned long NumberOfProxyAuthnSchemes;
 unsigned long *ProxyAuthnSchemes;
} RPC_HTTP_TRANSPORT_CREDENTIALS_V2_W, *PRPC_HTTP_TRANSPORT_CREDENTIALS_V2_W;
typedef struct _RPC_HTTP_TRANSPORT_CREDENTIALS_V2_A
{
 SEC_WINNT_AUTH_IDENTITY_A *TransportCredentials;
 unsigned long Flags;
 unsigned long AuthenticationTarget;
 unsigned long NumberOfAuthnSchemes;
 unsigned long *AuthnSchemes;
 unsigned char *ServerCertificateSubject;
 SEC_WINNT_AUTH_IDENTITY_A *ProxyCredentials;
 unsigned long NumberOfProxyAuthnSchemes;
 unsigned long *ProxyAuthnSchemes;
} RPC_HTTP_TRANSPORT_CREDENTIALS_V2_A, *PRPC_HTTP_TRANSPORT_CREDENTIALS_V2_A;
typedef struct _RPC_SECURITY_QOS_V2_W {
 unsigned long Version;
 unsigned long Capabilities;
 unsigned long IdentityTracking;
 unsigned long ImpersonationType;
 unsigned long AdditionalSecurityInfoType;
 union
 {
 RPC_HTTP_TRANSPORT_CREDENTIALS_W *HttpCredentials;
 } u;
} RPC_SECURITY_QOS_V2_W, *PRPC_SECURITY_QOS_V2_W;
typedef struct _RPC_SECURITY_QOS_V2_A {
 unsigned long Version;
 unsigned long Capabilities;
 unsigned long IdentityTracking;
 unsigned long ImpersonationType;
 unsigned long AdditionalSecurityInfoType;
 union
 {
 RPC_HTTP_TRANSPORT_CREDENTIALS_A *HttpCredentials;
 } u;
} RPC_SECURITY_QOS_V2_A, *PRPC_SECURITY_QOS_V2_A;
typedef struct _RPC_SECURITY_QOS_V3_W {
 unsigned long Version;
 unsigned long Capabilities;
 unsigned long IdentityTracking;
 unsigned long ImpersonationType;
 unsigned long AdditionalSecurityInfoType;
 union
 {
 RPC_HTTP_TRANSPORT_CREDENTIALS_W *HttpCredentials;
 } u;
 void *Sid;
} RPC_SECURITY_QOS_V3_W, *PRPC_SECURITY_QOS_V3_W;
typedef struct _RPC_SECURITY_QOS_V3_A {
 unsigned long Version;
 unsigned long Capabilities;
 unsigned long IdentityTracking;
 unsigned long ImpersonationType;
 unsigned long AdditionalSecurityInfoType;
 union
 {
 RPC_HTTP_TRANSPORT_CREDENTIALS_A *HttpCredentials;
 } u;
 void *Sid;
} RPC_SECURITY_QOS_V3_A, *PRPC_SECURITY_QOS_V3_A;
typedef struct _RPC_SECURITY_QOS_V4_W {
 unsigned long Version;
 unsigned long Capabilities;
 unsigned long IdentityTracking;
 unsigned long ImpersonationType;
 unsigned long AdditionalSecurityInfoType;
 union
 {
 RPC_HTTP_TRANSPORT_CREDENTIALS_W *HttpCredentials;
 } u;
 void *Sid;
 unsigned int EffectiveOnly;
} RPC_SECURITY_QOS_V4_W, *PRPC_SECURITY_QOS_V4_W;
typedef struct _RPC_SECURITY_QOS_V4_A {
 unsigned long Version;
 unsigned long Capabilities;
 unsigned long IdentityTracking;
 unsigned long ImpersonationType;
 unsigned long AdditionalSecurityInfoType;
 union
 {
 RPC_HTTP_TRANSPORT_CREDENTIALS_A *HttpCredentials;
 } u;
 void *Sid;
 unsigned int EffectiveOnly;
} RPC_SECURITY_QOS_V4_A, *PRPC_SECURITY_QOS_V4_A;
typedef struct _RPC_BINDING_HANDLE_TEMPLATE_V1_W {
 unsigned long Version;
 unsigned long Flags;
 unsigned long ProtocolSequence;
 unsigned short *NetworkAddress;
 unsigned short *StringEndpoint;
 union
 {
 unsigned short *Reserved;
 } u1;
 UUID ObjectUuid;
} RPC_BINDING_HANDLE_TEMPLATE_V1_W, *PRPC_BINDING_HANDLE_TEMPLATE_V1_W;
typedef struct _RPC_BINDING_HANDLE_TEMPLATE_V1_A {
 unsigned long Version;
 unsigned long Flags;
 unsigned long ProtocolSequence;
 unsigned char *NetworkAddress;
 unsigned char *StringEndpoint;
 union
 {
 unsigned char *Reserved;
 } u1;
 UUID ObjectUuid;
} RPC_BINDING_HANDLE_TEMPLATE_V1_A, *PRPC_BINDING_HANDLE_TEMPLATE_V1_A;
typedef struct _RPC_BINDING_HANDLE_SECURITY_V1_W {
 unsigned long Version;
 unsigned short *ServerPrincName;
 unsigned long AuthnLevel;
 unsigned long AuthnSvc;
 SEC_WINNT_AUTH_IDENTITY_W *AuthIdentity;
 RPC_SECURITY_QOS *SecurityQos;
} RPC_BINDING_HANDLE_SECURITY_V1_W, *PRPC_BINDING_HANDLE_SECURITY_V1_W;
typedef struct _RPC_BINDING_HANDLE_SECURITY_V1_A {
 unsigned long Version;
 unsigned char *ServerPrincName;
 unsigned long AuthnLevel;
 unsigned long AuthnSvc;
 SEC_WINNT_AUTH_IDENTITY_A *AuthIdentity;
 RPC_SECURITY_QOS *SecurityQos;
} RPC_BINDING_HANDLE_SECURITY_V1_A, *PRPC_BINDING_HANDLE_SECURITY_V1_A;
typedef struct _RPC_BINDING_HANDLE_OPTIONS_V1 {
 unsigned long Version;
 unsigned long Flags;
 unsigned long ComTimeout;
 unsigned long CallTimeout;
} RPC_BINDING_HANDLE_OPTIONS_V1, *PRPC_BINDING_HANDLE_OPTIONS_V1;
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingCreateA (
 RPC_BINDING_HANDLE_TEMPLATE_V1_A * Template,
 RPC_BINDING_HANDLE_SECURITY_V1_A * Security,
 RPC_BINDING_HANDLE_OPTIONS_V1 * Options,
 RPC_BINDING_HANDLE * Binding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingCreateW (
 RPC_BINDING_HANDLE_TEMPLATE_V1_W * Template,
 RPC_BINDING_HANDLE_SECURITY_V1_W * Security,
 RPC_BINDING_HANDLE_OPTIONS_V1 * Options,
 RPC_BINDING_HANDLE * Binding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingGetTrainingContextHandle (
 RPC_BINDING_HANDLE Binding,
 void ** ContextHandle
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerInqBindingHandle (
 RPC_BINDING_HANDLE * Binding
 );
typedef enum _RPC_HTTP_REDIRECTOR_STAGE
{
 RPCHTTP_RS_REDIRECT = 1,
 RPCHTTP_RS_ACCESS_1,
 RPCHTTP_RS_SESSION,
 RPCHTTP_RS_ACCESS_2,
 RPCHTTP_RS_INTERFACE
} RPC_HTTP_REDIRECTOR_STAGE;
typedef RPC_STATUS
(__stdcall * RPC_NEW_HTTP_PROXY_CHANNEL) (
 RPC_HTTP_REDIRECTOR_STAGE RedirectorStage,
 RPC_WSTR ServerName,
 RPC_WSTR ServerPort,
 RPC_WSTR RemoteUser,
 RPC_WSTR AuthType,
 void * ResourceUuid,
 void * Metadata,
 void * SessionId,
 void * Interface,
 void * Reserved,
 unsigned long Flags,
 RPC_WSTR * NewServerName,
 RPC_WSTR * NewServerPort
 );
typedef void
(__stdcall * RPC_HTTP_PROXY_FREE_STRING) (
 RPC_WSTR String
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcImpersonateClient (
 RPC_BINDING_HANDLE BindingHandle
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcRevertToSelfEx (
 RPC_BINDING_HANDLE BindingHandle
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcRevertToSelf (
 void
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingInqAuthClientA (
 RPC_BINDING_HANDLE ClientBinding,
 RPC_AUTHZ_HANDLE * Privs,
 RPC_CSTR * ServerPrincName,
 unsigned long * AuthnLevel,
 unsigned long * AuthnSvc,
 unsigned long * AuthzSvc
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingInqAuthClientW (
 RPC_BINDING_HANDLE ClientBinding,
 RPC_AUTHZ_HANDLE * Privs,
 RPC_WSTR * ServerPrincName,
 unsigned long * AuthnLevel,
 unsigned long * AuthnSvc,
 unsigned long * AuthzSvc
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingInqAuthClientExA (
 RPC_BINDING_HANDLE ClientBinding,
 RPC_AUTHZ_HANDLE * Privs,
 RPC_CSTR * ServerPrincName,
 unsigned long * AuthnLevel,
 unsigned long * AuthnSvc,
 unsigned long * AuthzSvc,
 unsigned long Flags
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingInqAuthClientExW (
 RPC_BINDING_HANDLE ClientBinding,
 RPC_AUTHZ_HANDLE * Privs,
 RPC_WSTR * ServerPrincName,
 unsigned long * AuthnLevel,
 unsigned long * AuthnSvc,
 unsigned long * AuthzSvc,
 unsigned long Flags
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingInqAuthInfoA (
 RPC_BINDING_HANDLE Binding,
 RPC_CSTR * ServerPrincName,
 unsigned long * AuthnLevel,
 unsigned long * AuthnSvc,
 RPC_AUTH_IDENTITY_HANDLE * AuthIdentity,
 unsigned long * AuthzSvc
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingInqAuthInfoW (
 RPC_BINDING_HANDLE Binding,
 RPC_WSTR * ServerPrincName,
 unsigned long * AuthnLevel,
 unsigned long * AuthnSvc,
 RPC_AUTH_IDENTITY_HANDLE * AuthIdentity,
 unsigned long * AuthzSvc
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingSetAuthInfoA (
 RPC_BINDING_HANDLE Binding,
 RPC_CSTR ServerPrincName,
 unsigned long AuthnLevel,
 unsigned long AuthnSvc,
 RPC_AUTH_IDENTITY_HANDLE AuthIdentity,
 unsigned long AuthzSvc
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingSetAuthInfoExA (
 RPC_BINDING_HANDLE Binding,
 RPC_CSTR ServerPrincName,
 unsigned long AuthnLevel,
 unsigned long AuthnSvc,
 RPC_AUTH_IDENTITY_HANDLE AuthIdentity,
 unsigned long AuthzSvc,
 RPC_SECURITY_QOS * SecurityQos
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingSetAuthInfoW (
 RPC_BINDING_HANDLE Binding,
 RPC_WSTR ServerPrincName,
 unsigned long AuthnLevel,
 unsigned long AuthnSvc,
 RPC_AUTH_IDENTITY_HANDLE AuthIdentity,
 unsigned long AuthzSvc
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingSetAuthInfoExW (
 RPC_BINDING_HANDLE Binding,
 RPC_WSTR ServerPrincName,
 unsigned long AuthnLevel,
 unsigned long AuthnSvc,
 RPC_AUTH_IDENTITY_HANDLE AuthIdentity,
 unsigned long AuthzSvc,
 RPC_SECURITY_QOS * SecurityQOS
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingInqAuthInfoExA (
 RPC_BINDING_HANDLE Binding,
 RPC_CSTR * ServerPrincName,
 unsigned long * AuthnLevel,
 unsigned long * AuthnSvc,
 RPC_AUTH_IDENTITY_HANDLE * AuthIdentity,
 unsigned long * AuthzSvc,
 unsigned long RpcQosVersion,
 RPC_SECURITY_QOS *SecurityQOS
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingInqAuthInfoExW (
 RPC_BINDING_HANDLE Binding,
 RPC_WSTR * ServerPrincName,
 unsigned long * AuthnLevel,
 unsigned long * AuthnSvc,
 RPC_AUTH_IDENTITY_HANDLE * AuthIdentity,
 unsigned long * AuthzSvc,
 unsigned long RpcQosVersion,
 RPC_SECURITY_QOS * SecurityQOS
 );
typedef void
(__stdcall * RPC_AUTH_KEY_RETRIEVAL_FN) (
 void * Arg,
 RPC_WSTR ServerPrincName,
 unsigned long KeyVer,
 void * * Key,
 RPC_STATUS * Status
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerCompleteSecurityCallback(
 RPC_BINDING_HANDLE BindingHandle,
 RPC_STATUS Status
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerRegisterAuthInfoA (
 RPC_CSTR ServerPrincName,
 unsigned long AuthnSvc,
 RPC_AUTH_KEY_RETRIEVAL_FN GetKeyFn,
 void * Arg
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerRegisterAuthInfoW (
 RPC_WSTR ServerPrincName,
 unsigned long AuthnSvc,
 RPC_AUTH_KEY_RETRIEVAL_FN GetKeyFn,
 void * Arg
 );
typedef struct {
 unsigned char * UserName;
 unsigned char * ComputerName;
 unsigned short Privilege;
 unsigned long AuthFlags;
} RPC_CLIENT_INFORMATION1, * PRPC_CLIENT_INFORMATION1;
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingServerFromClient (
 RPC_BINDING_HANDLE ClientBinding,
 RPC_BINDING_HANDLE * ServerBinding
 );
__declspec(dllimport)
__declspec(noreturn)
void
__stdcall
RpcRaiseException (
 RPC_STATUS exception
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcTestCancel(
 void
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerTestCancel (
 RPC_BINDING_HANDLE BindingHandle
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcCancelThread(
 void * Thread
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcCancelThreadEx(
 void * Thread,
 long Timeout
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
UuidCreate (
 UUID * Uuid
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
UuidCreateSequential (
 UUID * Uuid
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
UuidToStringA (
 const UUID * Uuid,
 RPC_CSTR * StringUuid
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
UuidFromStringA (
 RPC_CSTR StringUuid,
 UUID * Uuid
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
UuidToStringW (
 const UUID * Uuid,
 RPC_WSTR * StringUuid
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
UuidFromStringW (
 RPC_WSTR StringUuid,
 UUID * Uuid
 );
__declspec(dllimport)
signed int
__stdcall
UuidCompare (
 UUID * Uuid1,
 UUID * Uuid2,
 RPC_STATUS * Status
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
UuidCreateNil (
 UUID * NilUuid
 );
__declspec(dllimport)
int
__stdcall
UuidEqual (
 UUID * Uuid1,
 UUID * Uuid2,
 RPC_STATUS * Status
 );
__declspec(dllimport)
unsigned short
__stdcall
UuidHash (
 UUID * Uuid,
 RPC_STATUS * Status
 );
__declspec(dllimport)
int
__stdcall
UuidIsNil (
 UUID * Uuid,
 RPC_STATUS * Status
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcEpRegisterNoReplaceA (
 RPC_IF_HANDLE IfSpec,
 RPC_BINDING_VECTOR * BindingVector,
 UUID_VECTOR * UuidVector,
 RPC_CSTR Annotation
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcEpRegisterNoReplaceW (
 RPC_IF_HANDLE IfSpec,
 RPC_BINDING_VECTOR * BindingVector,
 UUID_VECTOR * UuidVector,
 RPC_WSTR Annotation
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcEpRegisterA (
 RPC_IF_HANDLE IfSpec,
 RPC_BINDING_VECTOR * BindingVector,
 UUID_VECTOR * UuidVector,
 RPC_CSTR Annotation
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcEpRegisterW (
 RPC_IF_HANDLE IfSpec,
 RPC_BINDING_VECTOR * BindingVector,
 UUID_VECTOR * UuidVector,
 RPC_WSTR Annotation
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcEpUnregister(
 RPC_IF_HANDLE IfSpec,
 RPC_BINDING_VECTOR * BindingVector,
 UUID_VECTOR * UuidVector
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
DceErrorInqTextA (
 RPC_STATUS RpcStatus,
 RPC_CSTR ErrorText
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
DceErrorInqTextW (
 RPC_STATUS RpcStatus,
 RPC_WSTR ErrorText
 );
typedef I_RPC_HANDLE * RPC_EP_INQ_HANDLE;
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtEpEltInqBegin (
 RPC_BINDING_HANDLE EpBinding,
 unsigned long InquiryType,
 RPC_IF_ID * IfId,
 unsigned long VersOption,
 UUID * ObjectUuid,
 RPC_EP_INQ_HANDLE * InquiryContext
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtEpEltInqDone (
 RPC_EP_INQ_HANDLE * InquiryContext
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtEpEltInqNextA (
 RPC_EP_INQ_HANDLE InquiryContext,
 RPC_IF_ID * IfId,
 RPC_BINDING_HANDLE * Binding,
 UUID * ObjectUuid,
 RPC_CSTR * Annotation
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtEpEltInqNextW (
 RPC_EP_INQ_HANDLE InquiryContext,
 RPC_IF_ID * IfId,
 RPC_BINDING_HANDLE * Binding,
 UUID * ObjectUuid,
 RPC_WSTR * Annotation
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtEpUnregister (
 RPC_BINDING_HANDLE EpBinding,
 RPC_IF_ID * IfId,
 RPC_BINDING_HANDLE Binding,
 UUID * ObjectUuid
 );
typedef int
(__stdcall * RPC_MGMT_AUTHORIZATION_FN) (
 RPC_BINDING_HANDLE ClientBinding,
 unsigned long RequestedMgmtOperation,
 RPC_STATUS * Status
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcMgmtSetAuthorizationFn (
 RPC_MGMT_AUTHORIZATION_FN AuthorizationFn
 );
__declspec(dllimport)
int
__stdcall
RpcExceptionFilter (
 unsigned long ExceptionCode
 );
#pragma once
extern "C" {
typedef struct _RPC_VERSION {
 unsigned short MajorVersion;
 unsigned short MinorVersion;
} RPC_VERSION;
typedef struct _RPC_SYNTAX_IDENTIFIER {
 GUID SyntaxGUID;
 RPC_VERSION SyntaxVersion;
} RPC_SYNTAX_IDENTIFIER, * PRPC_SYNTAX_IDENTIFIER;
typedef struct _RPC_MESSAGE
{
 RPC_BINDING_HANDLE Handle;
 unsigned long DataRepresentation;
 void * Buffer;
 unsigned int BufferLength;
 unsigned int ProcNum;
 PRPC_SYNTAX_IDENTIFIER TransferSyntax;
 void * RpcInterfaceInformation;
 void * ReservedForRuntime;
 void * ManagerEpv;
 void * ImportContext;
 unsigned long RpcFlags;
} RPC_MESSAGE, * PRPC_MESSAGE;
typedef RPC_STATUS
__stdcall RPC_FORWARD_FUNCTION(
 UUID * InterfaceId,
 RPC_VERSION * InterfaceVersion,
 UUID * ObjectId,
 unsigned char * Rpcpro,
 void * * ppDestEndpoint);
enum RPC_ADDRESS_CHANGE_TYPE
{
 PROTOCOL_NOT_LOADED = 1,
 PROTOCOL_LOADED,
 PROTOCOL_ADDRESS_CHANGE
};
typedef void
__stdcall RPC_ADDRESS_CHANGE_FN(
 void * arg
 );
typedef
void
(__stdcall * RPC_DISPATCH_FUNCTION) (
 PRPC_MESSAGE Message
 );
typedef struct {
 unsigned int DispatchTableCount;
 RPC_DISPATCH_FUNCTION * DispatchTable;
 LONG_PTR Reserved;
} RPC_DISPATCH_TABLE, * PRPC_DISPATCH_TABLE;
typedef struct _RPC_PROTSEQ_ENDPOINT
{
 unsigned char * RpcProtocolSequence;
 unsigned char * Endpoint;
} RPC_PROTSEQ_ENDPOINT, * PRPC_PROTSEQ_ENDPOINT;
typedef struct _RPC_SERVER_INTERFACE
{
 unsigned int Length;
 RPC_SYNTAX_IDENTIFIER InterfaceId;
 RPC_SYNTAX_IDENTIFIER TransferSyntax;
 PRPC_DISPATCH_TABLE DispatchTable;
 unsigned int RpcProtseqEndpointCount;
 PRPC_PROTSEQ_ENDPOINT RpcProtseqEndpoint;
 void *DefaultManagerEpv;
 void const *InterpreterInfo;
 unsigned int Flags ;
} RPC_SERVER_INTERFACE, * PRPC_SERVER_INTERFACE;
typedef struct _RPC_CLIENT_INTERFACE
{
 unsigned int Length;
 RPC_SYNTAX_IDENTIFIER InterfaceId;
 RPC_SYNTAX_IDENTIFIER TransferSyntax;
 PRPC_DISPATCH_TABLE DispatchTable;
 unsigned int RpcProtseqEndpointCount;
 PRPC_PROTSEQ_ENDPOINT RpcProtseqEndpoint;
 ULONG_PTR Reserved;
 void const * InterpreterInfo;
 unsigned int Flags ;
} RPC_CLIENT_INTERFACE, * PRPC_CLIENT_INTERFACE;
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcNegotiateTransferSyntax (
 RPC_MESSAGE * Message
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcGetBuffer (
 RPC_MESSAGE * Message
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcGetBufferWithObject (
 RPC_MESSAGE * Message,
 UUID * ObjectUuid
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcSendReceive (
 RPC_MESSAGE * Message
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcFreeBuffer (
 RPC_MESSAGE * Message
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcSend (
 PRPC_MESSAGE Message
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcReceive (
 PRPC_MESSAGE Message,
 unsigned int Size
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcFreePipeBuffer (
 RPC_MESSAGE * Message
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcReallocPipeBuffer (
 PRPC_MESSAGE Message,
 unsigned int NewSize
 );
typedef void * I_RPC_MUTEX;
__declspec(dllimport)
void
__stdcall
I_RpcRequestMutex (
 I_RPC_MUTEX * Mutex
 );
__declspec(dllimport)
void
__stdcall
I_RpcClearMutex (
 I_RPC_MUTEX Mutex
 );
__declspec(dllimport)
void
__stdcall
I_RpcDeleteMutex (
 I_RPC_MUTEX Mutex
 );
__declspec(dllimport)
void *
__stdcall
I_RpcAllocate (
 unsigned int Size
 );
__declspec(dllimport)
void
__stdcall
I_RpcFree (
 void * Object
 );
__declspec(dllimport)
void
__stdcall
I_RpcPauseExecution (
 unsigned long Milliseconds
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcGetExtendedError (
 void
 );
typedef
void
(__stdcall * PRPC_RUNDOWN) (
 void * AssociationContext
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcMonitorAssociation (
 RPC_BINDING_HANDLE Handle,
 PRPC_RUNDOWN RundownRoutine,
 void * Context
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcStopMonitorAssociation (
 RPC_BINDING_HANDLE Handle
 );
__declspec(dllimport)
RPC_BINDING_HANDLE
__stdcall
I_RpcGetCurrentCallHandle(
 void
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcGetAssociationContext (
 RPC_BINDING_HANDLE BindingHandle,
 void * * AssociationContext
 );
__declspec(dllimport)
void *
__stdcall
I_RpcGetServerContextList (
 RPC_BINDING_HANDLE BindingHandle
 );
__declspec(dllimport)
void
__stdcall
I_RpcSetServerContextList (
 RPC_BINDING_HANDLE BindingHandle,
 void * ServerContextList
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcNsInterfaceExported (
 unsigned long EntryNameSyntax,
 unsigned short *EntryName,
 RPC_SERVER_INTERFACE * RpcInterfaceInformation
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcNsInterfaceUnexported (
 unsigned long EntryNameSyntax,
 unsigned short *EntryName,
 RPC_SERVER_INTERFACE * RpcInterfaceInformation
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingToStaticStringBindingW (
 RPC_BINDING_HANDLE Binding,
 unsigned short **StringBinding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingInqSecurityContext (
 RPC_BINDING_HANDLE Binding,
 void **SecurityContextHandle
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingInqSecurityContextKeyInfo (
 RPC_BINDING_HANDLE Binding,
 void *KeyInfo
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingInqWireIdForSnego (
 RPC_BINDING_HANDLE Binding,
 RPC_CSTR WireId
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingInqMarshalledTargetInfo (
 RPC_BINDING_HANDLE Binding,
 unsigned long * MarshalledTargetInfoSize,
 RPC_CSTR * MarshalledTargetInfo
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingInqLocalClientPID (
 RPC_BINDING_HANDLE Binding,
 unsigned long *Pid
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingHandleToAsyncHandle (
 RPC_BINDING_HANDLE Binding,
 void **AsyncHandle
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcNsBindingSetEntryNameW (
 RPC_BINDING_HANDLE Binding,
 unsigned long EntryNameSyntax,
 RPC_WSTR EntryName
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcNsBindingSetEntryNameA (
 RPC_BINDING_HANDLE Binding,
 unsigned long EntryNameSyntax,
 RPC_CSTR EntryName
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcServerUseProtseqEp2A (
 RPC_CSTR NetworkAddress,
 RPC_CSTR Protseq,
 unsigned int MaxCalls,
 RPC_CSTR Endpoint,
 void * SecurityDescriptor,
 void * Policy
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcServerUseProtseqEp2W (
 RPC_WSTR NetworkAddress,
 RPC_WSTR Protseq,
 unsigned int MaxCalls,
 RPC_WSTR Endpoint,
 void * SecurityDescriptor,
 void * Policy
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcServerUseProtseq2W (
 RPC_WSTR NetworkAddress,
 RPC_WSTR Protseq,
 unsigned int MaxCalls,
 void * SecurityDescriptor,
 void * Policy
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcServerUseProtseq2A (
 RPC_CSTR NetworkAddress,
 RPC_CSTR Protseq,
 unsigned int MaxCalls,
 void * SecurityDescriptor,
 void * Policy
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcServerStartService (
 RPC_WSTR Protseq,
 RPC_WSTR Endpoint,
 RPC_IF_HANDLE IfSpec
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingInqDynamicEndpointW (
 RPC_BINDING_HANDLE Binding,
 RPC_WSTR *DynamicEndpoint
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingInqDynamicEndpointA (
 RPC_BINDING_HANDLE Binding,
 RPC_CSTR *DynamicEndpoint
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcServerCheckClientRestriction (
 RPC_BINDING_HANDLE Context
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingInqTransportType (
 RPC_BINDING_HANDLE Binding,
 unsigned int * Type
 );
typedef struct _RPC_TRANSFER_SYNTAX
{
 UUID Uuid;
 unsigned short VersMajor;
 unsigned short VersMinor;
} RPC_TRANSFER_SYNTAX;
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcIfInqTransferSyntaxes (
 RPC_IF_HANDLE RpcIfHandle,
 RPC_TRANSFER_SYNTAX * TransferSyntaxes,
 unsigned int TransferSyntaxSize,
 unsigned int * TransferSyntaxCount
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_UuidCreate (
 UUID * Uuid
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingCopy (
 RPC_BINDING_HANDLE SourceBinding,
 RPC_BINDING_HANDLE * DestinationBinding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingIsClientLocal (
 RPC_BINDING_HANDLE BindingHandle,
 unsigned int * ClientLocalFlag
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingInqConnId (
 RPC_BINDING_HANDLE Binding,
 void **ConnId,
 int *pfFirstCall
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingCreateNP (
 RPC_WSTR ServerName,
 RPC_WSTR ServiceName,
 RPC_WSTR NetworkOptions,
 RPC_BINDING_HANDLE *Binding
 );
__declspec(dllimport)
void
__stdcall
I_RpcSsDontSerializeContext (
 void
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcLaunchDatagramReceiveThread(
 void * pAddress
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcServerRegisterForwardFunction (
 RPC_FORWARD_FUNCTION * pForwardFunction
 );
RPC_ADDRESS_CHANGE_FN * __stdcall
I_RpcServerInqAddressChangeFn(
 void
 );
RPC_STATUS __stdcall
I_RpcServerSetAddressChangeFn(
 RPC_ADDRESS_CHANGE_FN * pAddressChangeFn
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcServerInqLocalConnAddress (
 RPC_BINDING_HANDLE Binding,
 void *Buffer,
 unsigned long *BufferSize,
 unsigned long *AddressFormat
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcServerInqRemoteConnAddress (
 RPC_BINDING_HANDLE Binding,
 void *Buffer,
 unsigned long *BufferSize,
 unsigned long *AddressFormat
 );
__declspec(dllimport)
void
__stdcall
I_RpcSessionStrictContextHandle (
 void
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcTurnOnEEInfoPropagation (
 void
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcConnectionInqSockBuffSize(
 unsigned long * RecvBuffSize,
 unsigned long * SendBuffSize
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcConnectionSetSockBuffSize(
 unsigned long RecvBuffSize,
 unsigned long SendBuffSize
 );
typedef
void
(*RPCLT_PDU_FILTER_FUNC) (
 void *Buffer,
 unsigned int BufferLength,
 int fDatagram
 );
typedef
void
(__cdecl *RPC_SETFILTER_FUNC) (
 RPCLT_PDU_FILTER_FUNC pfnFilter
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcServerStartListening(
 void * hWnd
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcServerStopListening(
 void
 );
typedef RPC_STATUS (*RPC_BLOCKING_FN) (
 void * hWnd,
 void * Context,
 void * hSyncEvent
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcBindingSetAsync(
 RPC_BINDING_HANDLE Binding,
 RPC_BLOCKING_FN BlockingFn,
 unsigned long ServerTid
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcSetThreadParams(
 int fClientFree,
 void *Context,
 void * hWndClient
 );
__declspec(dllimport)
unsigned int
__stdcall
I_RpcWindowProc(
 void * hWnd,
 unsigned int Message,
 unsigned int wParam,
 unsigned long lParam
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcServerUnregisterEndpointA (
 RPC_CSTR Protseq,
 RPC_CSTR Endpoint
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcServerUnregisterEndpointW (
 RPC_WSTR Protseq,
 RPC_WSTR Endpoint
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcServerInqTransportType(
 unsigned int * Type
 );
__declspec(dllimport)
long
__stdcall
I_RpcMapWin32Status (
 RPC_STATUS Status
 );
typedef struct _RPC_C_OPT_METADATA_DESCRIPTOR
{
 unsigned long BufferSize;
 char *Buffer;
} RPC_C_OPT_METADATA_DESCRIPTOR;
typedef struct _RDR_CALLOUT_STATE
{
 RPC_STATUS LastError;
 void *LastEEInfo;
 RPC_HTTP_REDIRECTOR_STAGE LastCalledStage;
 unsigned short *ServerName;
 unsigned short *ServerPort;
 unsigned short *RemoteUser;
 unsigned short *AuthType;
 unsigned char ResourceTypePresent;
 unsigned char MetadataPresent;
 unsigned char SessionIdPresent;
 unsigned char InterfacePresent;
 UUID ResourceType;
 RPC_C_OPT_METADATA_DESCRIPTOR Metadata;
 UUID SessionId;
 RPC_SYNTAX_IDENTIFIER Interface;
 void *CertContext;
} RDR_CALLOUT_STATE;
typedef RPC_STATUS
(__stdcall *I_RpcProxyIsValidMachineFn)
 (
 char *pszMachine,
 char *pszDotMachine,
 unsigned long dwPortNumber
 );
typedef RPC_STATUS
(__stdcall *I_RpcProxyGetClientAddressFn)
 (
 void *Context,
 char *Buffer,
 unsigned long *BufferLength
 );
typedef RPC_STATUS
(__stdcall *I_RpcProxyGetConnectionTimeoutFn)
 (
 unsigned long *ConnectionTimeout
 );
typedef RPC_STATUS
(__stdcall *I_RpcPerformCalloutFn)
 (
 void *Context,
 RDR_CALLOUT_STATE *CallOutState,
 RPC_HTTP_REDIRECTOR_STAGE Stage
 );
typedef void
(__stdcall *I_RpcFreeCalloutStateFn)
 (
 RDR_CALLOUT_STATE *CallOutState
 );
typedef RPC_STATUS
(__stdcall *I_RpcProxyGetClientSessionAndResourceUUID)
 (
 void *Context,
 int *SessionIdPresent,
 UUID *SessionId,
 int *ResourceIdPresent,
 UUID *ResourceId
 );
typedef RPC_STATUS
(__stdcall *I_RpcProxyFilterIfFn)
 (
 void *Context,
 UUID *IfUuid,
 unsigned short IfMajorVersion,
 int *fAllow
 );
typedef struct tagI_RpcProxyCallbackInterface
{
 I_RpcProxyIsValidMachineFn IsValidMachineFn;
 I_RpcProxyGetClientAddressFn GetClientAddressFn;
 I_RpcProxyGetConnectionTimeoutFn GetConnectionTimeoutFn;
 I_RpcPerformCalloutFn PerformCalloutFn;
 I_RpcFreeCalloutStateFn FreeCalloutStateFn;
 I_RpcProxyGetClientSessionAndResourceUUID GetClientSessionAndResourceUUIDFn;
 I_RpcProxyFilterIfFn ProxyFilterIfFn;
} I_RpcProxyCallbackInterface;
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcProxyNewConnection (
 unsigned long ConnectionType,
 unsigned short *ServerAddress,
 unsigned short *ServerPort,
 unsigned short *MinConnTimeout,
 void *ConnectionParameter,
 RDR_CALLOUT_STATE *CallOutState,
 I_RpcProxyCallbackInterface *ProxyCallbackInterface
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcReplyToClientWithStatus (
 void *ConnectionParameter,
 RPC_STATUS RpcStatus
 );
__declspec(dllimport)
void
__stdcall
I_RpcRecordCalloutFailure (
 RPC_STATUS RpcStatus,
 RDR_CALLOUT_STATE *CallOutState,
 unsigned short *DllName
 );
}
}
#pragma once
typedef void * RPC_NS_HANDLE;
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsBindingExportA(
 unsigned long EntryNameSyntax,
 RPC_CSTR EntryName,
 RPC_IF_HANDLE IfSpec,
 RPC_BINDING_VECTOR *BindingVec,
 UUID_VECTOR *ObjectUuidVec
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsBindingUnexportA(
 unsigned long EntryNameSyntax,
 RPC_CSTR EntryName,
 RPC_IF_HANDLE IfSpec,
 UUID_VECTOR *ObjectUuidVec
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsBindingExportW(
 unsigned long EntryNameSyntax,
 RPC_WSTR EntryName,
 RPC_IF_HANDLE IfSpec,
 RPC_BINDING_VECTOR *BindingVec,
 UUID_VECTOR *ObjectUuidVec
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsBindingUnexportW(
 unsigned long EntryNameSyntax,
 RPC_WSTR EntryName,
 RPC_IF_HANDLE IfSpec,
 UUID_VECTOR *ObjectUuidVec
 );
RPC_STATUS __stdcall
RpcNsBindingExportPnPA(
 unsigned long EntryNameSyntax,
 RPC_CSTR EntryName,
 RPC_IF_HANDLE IfSpec,
 UUID_VECTOR *ObjectVector
 );
RPC_STATUS __stdcall
RpcNsBindingUnexportPnPA(
 unsigned long EntryNameSyntax,
 RPC_CSTR EntryName,
 RPC_IF_HANDLE IfSpec,
 UUID_VECTOR *ObjectVector
 );
RPC_STATUS __stdcall
RpcNsBindingExportPnPW(
 unsigned long EntryNameSyntax,
 RPC_WSTR EntryName,
 RPC_IF_HANDLE IfSpec,
 UUID_VECTOR *ObjectVector
 );
RPC_STATUS __stdcall
RpcNsBindingUnexportPnPW(
 unsigned long EntryNameSyntax,
 RPC_WSTR EntryName,
 RPC_IF_HANDLE IfSpec,
 UUID_VECTOR *ObjectVector
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsBindingLookupBeginA(
 unsigned long EntryNameSyntax,
 RPC_CSTR EntryName,
 RPC_IF_HANDLE IfSpec,
 UUID *ObjUuid,
 unsigned long BindingMaxCount,
 RPC_NS_HANDLE *LookupContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsBindingLookupBeginW(
 unsigned long EntryNameSyntax,
 RPC_WSTR EntryName,
 RPC_IF_HANDLE IfSpec,
 UUID *ObjUuid,
 unsigned long BindingMaxCount,
 RPC_NS_HANDLE *LookupContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsBindingLookupNext(
 RPC_NS_HANDLE LookupContext,
 RPC_BINDING_VECTOR * * BindingVec
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsBindingLookupDone(
 RPC_NS_HANDLE * LookupContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsGroupDeleteA(
 unsigned long GroupNameSyntax,
 RPC_CSTR GroupName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsGroupMbrAddA(
 unsigned long GroupNameSyntax,
 RPC_CSTR GroupName,
 unsigned long MemberNameSyntax,
 RPC_CSTR MemberName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsGroupMbrRemoveA(
 unsigned long GroupNameSyntax,
 RPC_CSTR GroupName,
 unsigned long MemberNameSyntax,
 RPC_CSTR MemberName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsGroupMbrInqBeginA(
 unsigned long GroupNameSyntax,
 RPC_CSTR GroupName,
 unsigned long MemberNameSyntax,
 RPC_NS_HANDLE *InquiryContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsGroupMbrInqNextA(
 RPC_NS_HANDLE InquiryContext,
 RPC_CSTR *MemberName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsGroupDeleteW(
 unsigned long GroupNameSyntax,
 RPC_WSTR GroupName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsGroupMbrAddW(
 unsigned long GroupNameSyntax,
 RPC_WSTR GroupName,
 unsigned long MemberNameSyntax,
 RPC_WSTR MemberName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsGroupMbrRemoveW(
 unsigned long GroupNameSyntax,
 RPC_WSTR GroupName,
 unsigned long MemberNameSyntax,
 RPC_WSTR MemberName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsGroupMbrInqBeginW(
 unsigned long GroupNameSyntax,
 RPC_WSTR GroupName,
 unsigned long MemberNameSyntax,
 RPC_NS_HANDLE *InquiryContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsGroupMbrInqNextW(
 RPC_NS_HANDLE InquiryContext,
 RPC_WSTR *MemberName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsGroupMbrInqDone(
 RPC_NS_HANDLE * InquiryContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsProfileDeleteA(
 unsigned long ProfileNameSyntax,
 RPC_CSTR ProfileName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsProfileEltAddA(
 unsigned long ProfileNameSyntax,
 RPC_CSTR ProfileName,
 RPC_IF_ID *IfId,
 unsigned long MemberNameSyntax,
 RPC_CSTR MemberName,
 unsigned long Priority,
 RPC_CSTR Annotation
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsProfileEltRemoveA(
 unsigned long ProfileNameSyntax,
 RPC_CSTR ProfileName,
 RPC_IF_ID *IfId,
 unsigned long MemberNameSyntax,
 RPC_CSTR MemberName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsProfileEltInqBeginA(
 unsigned long ProfileNameSyntax,
 RPC_CSTR ProfileName,
 unsigned long InquiryType,
 RPC_IF_ID *IfId,
 unsigned long VersOption,
 unsigned long MemberNameSyntax,
 RPC_CSTR MemberName,
 RPC_NS_HANDLE *InquiryContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsProfileEltInqNextA(
 RPC_NS_HANDLE InquiryContext,
 RPC_IF_ID *IfId,
 RPC_CSTR *MemberName,
 unsigned long *Priority,
 RPC_CSTR *Annotation
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsProfileDeleteW(
 unsigned long ProfileNameSyntax,
 RPC_WSTR ProfileName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsProfileEltAddW(
 unsigned long ProfileNameSyntax,
 RPC_WSTR ProfileName,
 RPC_IF_ID *IfId,
 unsigned long MemberNameSyntax,
 RPC_WSTR MemberName,
 unsigned long Priority,
 RPC_WSTR Annotation
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsProfileEltRemoveW(
 unsigned long ProfileNameSyntax,
 RPC_WSTR ProfileName,
 RPC_IF_ID *IfId,
 unsigned long MemberNameSyntax,
 RPC_WSTR MemberName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsProfileEltInqBeginW(
 unsigned long ProfileNameSyntax,
 RPC_WSTR ProfileName,
 unsigned long InquiryType,
 RPC_IF_ID *IfId,
 unsigned long VersOption,
 unsigned long MemberNameSyntax,
 RPC_WSTR MemberName,
 RPC_NS_HANDLE *InquiryContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsProfileEltInqNextW(
 RPC_NS_HANDLE InquiryContext,
 RPC_IF_ID *IfId,
 RPC_WSTR *MemberName,
 unsigned long *Priority,
 RPC_WSTR *Annotation
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsProfileEltInqDone(
 RPC_NS_HANDLE * InquiryContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsEntryObjectInqBeginA(
 unsigned long EntryNameSyntax,
 RPC_CSTR EntryName,
 RPC_NS_HANDLE *InquiryContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsEntryObjectInqBeginW(
 unsigned long EntryNameSyntax,
 RPC_WSTR EntryName,
 RPC_NS_HANDLE *InquiryContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsEntryObjectInqNext(
 RPC_NS_HANDLE InquiryContext,
 UUID * ObjUuid
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsEntryObjectInqDone(
 RPC_NS_HANDLE * InquiryContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsEntryExpandNameA(
 unsigned long EntryNameSyntax,
 RPC_CSTR EntryName,
 RPC_CSTR *ExpandedName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsMgmtBindingUnexportA(
 unsigned long EntryNameSyntax,
 RPC_CSTR EntryName,
 RPC_IF_ID *IfId,
 unsigned long VersOption,
 UUID_VECTOR *ObjectUuidVec
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsMgmtEntryCreateA(
 unsigned long EntryNameSyntax,
 RPC_CSTR EntryName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsMgmtEntryDeleteA(
 unsigned long EntryNameSyntax,
 RPC_CSTR EntryName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsMgmtEntryInqIfIdsA(
 unsigned long EntryNameSyntax,
 RPC_CSTR EntryName,
 RPC_IF_ID_VECTOR * *IfIdVec
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsMgmtHandleSetExpAge(
 RPC_NS_HANDLE NsHandle,
 unsigned long ExpirationAge
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsMgmtInqExpAge(
 unsigned long * ExpirationAge
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsMgmtSetExpAge(
 unsigned long ExpirationAge
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsEntryExpandNameW(
 unsigned long EntryNameSyntax,
 RPC_WSTR EntryName,
 RPC_WSTR *ExpandedName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsMgmtBindingUnexportW(
 unsigned long EntryNameSyntax,
 RPC_WSTR EntryName,
 RPC_IF_ID *IfId,
 unsigned long VersOption,
 UUID_VECTOR *ObjectUuidVec
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsMgmtEntryCreateW(
 unsigned long EntryNameSyntax,
 RPC_WSTR EntryName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsMgmtEntryDeleteW(
 unsigned long EntryNameSyntax,
 RPC_WSTR EntryName
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsMgmtEntryInqIfIdsW(
 unsigned long EntryNameSyntax,
 RPC_WSTR EntryName,
 RPC_IF_ID_VECTOR * *IfIdVec
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsBindingImportBeginA(
 unsigned long EntryNameSyntax,
 RPC_CSTR EntryName,
 RPC_IF_HANDLE IfSpec,
 UUID *ObjUuid,
 RPC_NS_HANDLE *ImportContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsBindingImportBeginW(
 unsigned long EntryNameSyntax,
 RPC_WSTR EntryName,
 RPC_IF_HANDLE IfSpec,
 UUID *ObjUuid,
 RPC_NS_HANDLE *ImportContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsBindingImportNext(
 RPC_NS_HANDLE ImportContext,
 RPC_BINDING_HANDLE * Binding
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsBindingImportDone(
 RPC_NS_HANDLE * ImportContext
 );
__declspec(dllimport) RPC_STATUS __stdcall
RpcNsBindingSelect(
 RPC_BINDING_VECTOR * BindingVec,
 RPC_BINDING_HANDLE * Binding
 );
#pragma once
#pragma once
extern "C" {
typedef
enum _RPC_NOTIFICATION_TYPES
{
 RpcNotificationTypeNone,
 RpcNotificationTypeEvent,
 RpcNotificationTypeApc,
 RpcNotificationTypeIoc,
 RpcNotificationTypeHwnd,
 RpcNotificationTypeCallback
} RPC_NOTIFICATION_TYPES;
typedef
enum _RPC_ASYNC_EVENT {
 RpcCallComplete,
 RpcSendComplete,
 RpcReceiveComplete,
 RpcClientDisconnect,
 RpcClientCancel
 } RPC_ASYNC_EVENT;
struct _RPC_ASYNC_STATE;
typedef void __stdcall
RPCNOTIFICATION_ROUTINE (
 struct _RPC_ASYNC_STATE *pAsync,
 void *Context,
 RPC_ASYNC_EVENT Event);
typedef RPCNOTIFICATION_ROUTINE *PFN_RPCNOTIFICATION_ROUTINE;
typedef union _RPC_ASYNC_NOTIFICATION_INFO {
 struct {
 PFN_RPCNOTIFICATION_ROUTINE NotificationRoutine;
 HANDLE hThread;
 } APC;
 struct {
 HANDLE hIOPort;
 DWORD dwNumberOfBytesTransferred;
 DWORD_PTR dwCompletionKey;
 LPOVERLAPPED lpOverlapped;
 } IOC;
 struct {
 HWND hWnd;
 UINT Msg;
 } HWND;
 HANDLE hEvent;
 PFN_RPCNOTIFICATION_ROUTINE NotificationRoutine;
} RPC_ASYNC_NOTIFICATION_INFO, *PRPC_ASYNC_NOTIFICATION_INFO;
typedef struct _RPC_ASYNC_STATE {
 unsigned int Size;
 unsigned long Signature;
 long Lock;
 unsigned long Flags;
 void *StubInfo;
 void *UserInfo;
 void *RuntimeInfo;
 RPC_ASYNC_EVENT Event;
 RPC_NOTIFICATION_TYPES NotificationType;
 RPC_ASYNC_NOTIFICATION_INFO u;
 LONG_PTR Reserved[4];
 } RPC_ASYNC_STATE, *PRPC_ASYNC_STATE;
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcAsyncInitializeHandle (
 PRPC_ASYNC_STATE pAsync,
 unsigned int Size
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcAsyncRegisterInfo (
 PRPC_ASYNC_STATE pAsync
 ) ;
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcAsyncGetCallStatus (
 PRPC_ASYNC_STATE pAsync
 ) ;
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcAsyncCompleteCall (
 PRPC_ASYNC_STATE pAsync,
 void *Reply
 ) ;
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcAsyncAbortCall (
 PRPC_ASYNC_STATE pAsync,
 unsigned long ExceptionCode
 ) ;
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcAsyncCancelCall (
 PRPC_ASYNC_STATE pAsync,
 BOOL fAbort
 ) ;
typedef enum tagExtendedErrorParamTypes
{
 eeptAnsiString = 1,
 eeptUnicodeString,
 eeptLongVal,
 eeptShortVal,
 eeptPointerVal,
 eeptNone,
 eeptBinary
} ExtendedErrorParamTypes;
typedef struct tagBinaryParam
{
 void *Buffer;
 short Size;
} BinaryParam;
typedef struct tagRPC_EE_INFO_PARAM
{
 ExtendedErrorParamTypes ParameterType;
 union
 {
 LPSTR AnsiString;
 LPWSTR UnicodeString;
 long LVal;
 short SVal;
 ULONGLONG PVal;
 BinaryParam BVal;
 } u;
} RPC_EE_INFO_PARAM;
typedef struct tagRPC_EXTENDED_ERROR_INFO
{
 ULONG Version;
 LPWSTR ComputerName;
 ULONG ProcessID;
 union
 {
 SYSTEMTIME SystemTime;
 FILETIME FileTime;
 } u;
 ULONG GeneratingComponent;
 ULONG Status;
 USHORT DetectionLocation;
 USHORT Flags;
 int NumberOfParameters;
 RPC_EE_INFO_PARAM Parameters[4];
} RPC_EXTENDED_ERROR_INFO;
typedef struct tagRPC_ERROR_ENUM_HANDLE
{
 ULONG Signature;
 void *CurrentPos;
 void *Head;
} RPC_ERROR_ENUM_HANDLE;
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcErrorStartEnumeration (
 RPC_ERROR_ENUM_HANDLE *EnumHandle
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcErrorGetNextRecord (
 RPC_ERROR_ENUM_HANDLE *EnumHandle,
 BOOL CopyStrings,
 RPC_EXTENDED_ERROR_INFO *ErrorInfo
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcErrorEndEnumeration (
 RPC_ERROR_ENUM_HANDLE *EnumHandle
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcErrorResetEnumeration (
 RPC_ERROR_ENUM_HANDLE *EnumHandle
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcErrorGetNumberOfRecords (
 RPC_ERROR_ENUM_HANDLE *EnumHandle,
 int *Records
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcErrorSaveErrorInfo (
 RPC_ERROR_ENUM_HANDLE *EnumHandle,
 PVOID *ErrorBlob,
 size_t *BlobSize
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcErrorLoadErrorInfo (
 PVOID ErrorBlob,
 size_t BlobSize,
 RPC_ERROR_ENUM_HANDLE *EnumHandle
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcErrorAddRecord (
 RPC_EXTENDED_ERROR_INFO *ErrorInfo
 );
__declspec(dllimport)
void
__stdcall
RpcErrorClearInformation (
 void
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcAsyncCleanupThread (
 DWORD dwTimeout
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcGetAuthorizationContextForClient (
 RPC_BINDING_HANDLE ClientBinding,
 BOOL ImpersonateOnReturn,
 PVOID Reserved1,
 PLARGE_INTEGER pExpirationTime,
 LUID Reserved2,
 DWORD Reserved3,
 PVOID Reserved4,
 PVOID *pAuthzClientContext
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcFreeAuthorizationContext (
 PVOID *pAuthzClientContext
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcSsContextLockExclusive (
 RPC_BINDING_HANDLE ServerBindingHandle,
 PVOID UserContext
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcSsContextLockShared (
 RPC_BINDING_HANDLE ServerBindingHandle,
 PVOID UserContext
 );
typedef enum tagRpcLocalAddressFormat
{
 rlafInvalid = 0,
 rlafIPv4,
 rlafIPv6
} RpcLocalAddressFormat;
typedef struct _RPC_CALL_LOCAL_ADDRESS_V1
{
 unsigned int Version;
 void *Buffer;
 unsigned long BufferSize;
 RpcLocalAddressFormat AddressFormat;
} RPC_CALL_LOCAL_ADDRESS_V1, *PRPC_CALL_LOCAL_ADDRESS_V1;
typedef struct tagRPC_CALL_ATTRIBUTES_V1_W
{
 unsigned int Version;
 unsigned long Flags;
 unsigned long ServerPrincipalNameBufferLength;
 unsigned short *ServerPrincipalName;
 unsigned long ClientPrincipalNameBufferLength;
 unsigned short *ClientPrincipalName;
 unsigned long AuthenticationLevel;
 unsigned long AuthenticationService;
 BOOL NullSession;
} RPC_CALL_ATTRIBUTES_V1_W;
typedef struct tagRPC_CALL_ATTRIBUTES_V1_A
{
 unsigned int Version;
 unsigned long Flags;
 unsigned long ServerPrincipalNameBufferLength;
 unsigned char *ServerPrincipalName;
 unsigned long ClientPrincipalNameBufferLength;
 unsigned char *ClientPrincipalName;
 unsigned long AuthenticationLevel;
 unsigned long AuthenticationService;
 BOOL NullSession;
} RPC_CALL_ATTRIBUTES_V1_A;
typedef enum tagRpcCallType
{
 rctInvalid = 0,
 rctNormal,
 rctTraining,
 rctGuaranteed
} RpcCallType;
typedef enum tagRpcCallClientLocality
{
 rcclInvalid = 0,
 rcclLocal,
 rcclRemote,
 rcclClientUnknownLocality
} RpcCallClientLocality;
typedef struct tagRPC_CALL_ATTRIBUTES_V2_W
{
 unsigned int Version;
 unsigned long Flags;
 unsigned long ServerPrincipalNameBufferLength;
 unsigned short *ServerPrincipalName;
 unsigned long ClientPrincipalNameBufferLength;
 unsigned short *ClientPrincipalName;
 unsigned long AuthenticationLevel;
 unsigned long AuthenticationService;
 BOOL NullSession;
 BOOL KernelModeCaller;
 unsigned long ProtocolSequence;
 RpcCallClientLocality IsClientLocal;
 HANDLE ClientPID;
 unsigned long CallStatus;
 RpcCallType CallType;
 RPC_CALL_LOCAL_ADDRESS_V1 *CallLocalAddress;
 unsigned short OpNum;
 UUID InterfaceUuid;
} RPC_CALL_ATTRIBUTES_V2_W;
typedef struct tagRPC_CALL_ATTRIBUTES_V2_A
{
 unsigned int Version;
 unsigned long Flags;
 unsigned long ServerPrincipalNameBufferLength;
 unsigned char *ServerPrincipalName;
 unsigned long ClientPrincipalNameBufferLength;
 unsigned char *ClientPrincipalName;
 unsigned long AuthenticationLevel;
 unsigned long AuthenticationService;
 BOOL NullSession;
 BOOL KernelModeCaller;
 unsigned long ProtocolSequence;
 unsigned long IsClientLocal;
 HANDLE ClientPID;
 unsigned long CallStatus;
 RpcCallType CallType;
 RPC_CALL_LOCAL_ADDRESS_V1 *CallLocalAddress;
 unsigned short OpNum;
 UUID InterfaceUuid;
} RPC_CALL_ATTRIBUTES_V2_A;
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerInqCallAttributesW (
 RPC_BINDING_HANDLE ClientBinding,
 void *RpcCallAttributes
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerInqCallAttributesA (
 RPC_BINDING_HANDLE ClientBinding,
 void *RpcCallAttributes
 );
typedef RPC_CALL_ATTRIBUTES_V2_A RPC_CALL_ATTRIBUTES;
typedef enum _RPC_NOTIFICATIONS
{
 RpcNotificationCallNone = 0,
 RpcNotificationClientDisconnect = 1,
 RpcNotificationCallCancel = 2
} RPC_NOTIFICATIONS;
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerSubscribeForNotification (
 RPC_BINDING_HANDLE Binding,
 RPC_NOTIFICATIONS Notification,
 RPC_NOTIFICATION_TYPES NotificationType,
 RPC_ASYNC_NOTIFICATION_INFO *NotificationInfo
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcServerUnsubscribeForNotification (
 RPC_BINDING_HANDLE Binding,
 RPC_NOTIFICATIONS Notification,
 unsigned long *NotificationsQueued
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingBind (
 PRPC_ASYNC_STATE pAsync,
 RPC_BINDING_HANDLE Binding,
 RPC_IF_HANDLE IfSpec
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcBindingUnbind (
 RPC_BINDING_HANDLE Binding
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcDiagnoseError (
 RPC_BINDING_HANDLE BindingHandle,
 RPC_IF_HANDLE IfSpec,
 RPC_STATUS RpcStatus,
 RPC_ERROR_ENUM_HANDLE *EnumHandle,
 ULONG Options,
 HWND ParentWindow
 );
RPC_STATUS __stdcall
I_RpcAsyncSetHandle (
 PRPC_MESSAGE Message,
 PRPC_ASYNC_STATE pAsync
 );
RPC_STATUS __stdcall
I_RpcAsyncAbortCall (
 PRPC_ASYNC_STATE pAsync,
 unsigned long ExceptionCode
 ) ;
int
__stdcall
I_RpcExceptionFilter (
 unsigned long ExceptionCode
 );
}
}
#pragma once
#pragma warning(disable:4103)
#pragma pack(push,8)
#pragma once
extern "C" {
typedef struct
{
 RPC_NS_HANDLE LookupContext;
 RPC_BINDING_HANDLE ProposedHandle;
 RPC_BINDING_VECTOR * Bindings;
} RPC_IMPORT_CONTEXT_P, * PRPC_IMPORT_CONTEXT_P;
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcNsGetBuffer(
 PRPC_MESSAGE Message
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcNsSendReceive(
 PRPC_MESSAGE Message,
 RPC_BINDING_HANDLE * Handle
 );
__declspec(dllimport)
void
__stdcall
I_RpcNsRaiseException(
 PRPC_MESSAGE Message,
 RPC_STATUS Status
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_RpcReBindBuffer(
 PRPC_MESSAGE Message
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_NsServerBindSearch(
 void
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
I_NsClientBindSearch(
 void
 );
__declspec(dllimport)
void
__stdcall
I_NsClientBindDone(
 void
 );
}
#pragma once
extern "C" {
}
extern "C" {
typedef unsigned char byte;
typedef byte cs_byte;
typedef unsigned char boolean;
void * __stdcall MIDL_user_allocate(size_t size);
void __stdcall MIDL_user_free( void * );
void * __stdcall I_RpcDefaultAllocate(
 handle_t bh, size_t size, void * (* RealAlloc)(size_t) );
void __stdcall I_RpcDefaultFree(
 handle_t bh, void *, void (*RealFree)(void *) );
typedef void * NDR_CCONTEXT;
typedef struct
 {
 void * pad[2];
 void * userContext;
 } * NDR_SCONTEXT;
typedef void (__stdcall * NDR_RUNDOWN)(void * context);
typedef void (__stdcall * NDR_NOTIFY_ROUTINE)(void);
typedef void (__stdcall * NDR_NOTIFY2_ROUTINE)(boolean flag);
typedef struct _SCONTEXT_QUEUE {
 unsigned long NumberOfObjects;
 NDR_SCONTEXT * ArrayOfObjects;
 } SCONTEXT_QUEUE, * PSCONTEXT_QUEUE;
__declspec(dllimport)
RPC_BINDING_HANDLE
__stdcall
NDRCContextBinding (
 NDR_CCONTEXT CContext
 );
__declspec(dllimport)
void
__stdcall
NDRCContextMarshall (
 NDR_CCONTEXT CContext,
 void *pBuff
 );
__declspec(dllimport)
void
__stdcall
NDRCContextUnmarshall (
 NDR_CCONTEXT * pCContext,
 RPC_BINDING_HANDLE hBinding,
 void * pBuff,
 unsigned long DataRepresentation
 );
__declspec(dllimport)
void
__stdcall
NDRCContextUnmarshall2 (
 NDR_CCONTEXT * pCContext,
 RPC_BINDING_HANDLE hBinding,
 void * pBuff,
 unsigned long DataRepresentation
 );
__declspec(dllimport)
void
__stdcall
NDRSContextMarshall (
 NDR_SCONTEXT CContext,
 void * pBuff,
 NDR_RUNDOWN userRunDownIn
 );
__declspec(dllimport)
NDR_SCONTEXT
__stdcall
NDRSContextUnmarshall (
 void * pBuff,
 unsigned long DataRepresentation
 );
__declspec(dllimport)
void
__stdcall
NDRSContextMarshallEx (
 RPC_BINDING_HANDLE BindingHandle,
 NDR_SCONTEXT CContext,
 void * pBuff,
 NDR_RUNDOWN userRunDownIn
 );
__declspec(dllimport)
void
__stdcall
NDRSContextMarshall2 (
 RPC_BINDING_HANDLE BindingHandle,
 NDR_SCONTEXT CContext,
 void * pBuff,
 NDR_RUNDOWN userRunDownIn,
 void * CtxGuard,
 unsigned long Flags
 );
__declspec(dllimport)
NDR_SCONTEXT
__stdcall
NDRSContextUnmarshallEx (
 RPC_BINDING_HANDLE BindingHandle,
 void * pBuff,
 unsigned long DataRepresentation
 );
__declspec(dllimport)
NDR_SCONTEXT
__stdcall
NDRSContextUnmarshall2(
 RPC_BINDING_HANDLE BindingHandle,
 void * pBuff,
 unsigned long DataRepresentation,
 void * CtxGuard,
 unsigned long Flags
 );
__declspec(dllimport)
void
__stdcall
RpcSsDestroyClientContext (
 void * * ContextHandle
 );
typedef unsigned long error_status_t;
struct _MIDL_STUB_MESSAGE;
struct _MIDL_STUB_DESC;
struct _FULL_PTR_XLAT_TABLES;
typedef unsigned char * RPC_BUFPTR;
typedef unsigned long RPC_LENGTH;
typedef void (__stdcall * EXPR_EVAL)( struct _MIDL_STUB_MESSAGE * );
typedef const unsigned char * PFORMAT_STRING;
typedef struct
 {
 long Dimension;
 unsigned long * BufferConformanceMark;
 unsigned long * BufferVarianceMark;
 unsigned long * MaxCountArray;
 unsigned long * OffsetArray;
 unsigned long * ActualCountArray;
 } ARRAY_INFO, *PARRAY_INFO;
typedef struct _NDR_ASYNC_MESSAGE * PNDR_ASYNC_MESSAGE;
typedef struct _NDR_CORRELATION_INFO *PNDR_CORRELATION_INFO;
typedef const unsigned char * PFORMAT_STRING;
typedef struct _MIDL_SYNTAX_INFO MIDL_SYNTAX_INFO, *PMIDL_SYNTAX_INFO;
struct NDR_ALLOC_ALL_NODES_CONTEXT;
struct NDR_POINTER_QUEUE_STATE;
struct _NDR_PROC_CONTEXT;
typedef struct _MIDL_STUB_MESSAGE
 {
 PRPC_MESSAGE RpcMsg;
 unsigned char * Buffer;
 unsigned char * BufferStart;
 unsigned char * BufferEnd;
 unsigned char * BufferMark;
 unsigned long BufferLength;
 unsigned long MemorySize;
 unsigned char * Memory;
 unsigned char IsClient;
 unsigned char Pad;
 unsigned short uFlags2;
 int ReuseBuffer;
 struct NDR_ALLOC_ALL_NODES_CONTEXT *pAllocAllNodesContext;
 struct NDR_POINTER_QUEUE_STATE *pPointerQueueState;
 int IgnoreEmbeddedPointers;
 unsigned char * PointerBufferMark;
 unsigned char CorrDespIncrement;
 unsigned char uFlags;
 unsigned short UniquePtrCount;
 ULONG_PTR MaxCount;
 unsigned long Offset;
 unsigned long ActualCount;
 void * ( __stdcall * pfnAllocate)( size_t );
 void ( __stdcall * pfnFree)(void *);
 unsigned char * StackTop;
 unsigned char * pPresentedType;
 unsigned char * pTransmitType;
 handle_t SavedHandle;
 const struct _MIDL_STUB_DESC * StubDesc;
 struct _FULL_PTR_XLAT_TABLES * FullPtrXlatTables;
 unsigned long FullPtrRefId;
 unsigned long PointerLength;
 int fInDontFree :1;
 int fDontCallFreeInst :1;
 int fInOnlyParam :1;
 int fHasReturn :1;
 int fHasExtensions :1;
 int fHasNewCorrDesc :1;
 int fIsIn :1;
 int fIsOut :1;
 int fIsOicf :1;
 int fBufferValid :1;
 int fHasMemoryValidateCallback: 1;
 int fInFree :1;
 int fNeedMCCP :1;
 int fUnused :3;
 int fUnused2 :16;
 unsigned long dwDestContext;
 void * pvDestContext;
 NDR_SCONTEXT * SavedContextHandles;
 long ParamNumber;
 struct IRpcChannelBuffer * pRpcChannelBuffer;
 PARRAY_INFO pArrayInfo;
 unsigned long * SizePtrCountArray;
 unsigned long * SizePtrOffsetArray;
 unsigned long * SizePtrLengthArray;
 void * pArgQueue;
 unsigned long dwStubPhase;
 void * LowStackMark;
 PNDR_ASYNC_MESSAGE pAsyncMsg;
 PNDR_CORRELATION_INFO pCorrInfo;
 unsigned char * pCorrMemory;
 void * pMemoryList;
 INT_PTR pCSInfo;
 unsigned char * ConformanceMark;
 unsigned char * VarianceMark;
 INT_PTR Unused;
 struct _NDR_PROC_CONTEXT * pContext;
 void * ContextHandleHash;
 void * pUserMarshalList;
 INT_PTR Reserved51_3;
 INT_PTR Reserved51_4;
 INT_PTR Reserved51_5;
 } MIDL_STUB_MESSAGE, *PMIDL_STUB_MESSAGE;
typedef struct _MIDL_STUB_MESSAGE MIDL_STUB_MESSAGE, *PMIDL_STUB_MESSAGE;
typedef void *
 ( __stdcall * GENERIC_BINDING_ROUTINE)
 (void *);
typedef void
 ( __stdcall * GENERIC_UNBIND_ROUTINE)
 (void *, unsigned char *);
typedef struct _GENERIC_BINDING_ROUTINE_PAIR
 {
 GENERIC_BINDING_ROUTINE pfnBind;
 GENERIC_UNBIND_ROUTINE pfnUnbind;
 } GENERIC_BINDING_ROUTINE_PAIR, *PGENERIC_BINDING_ROUTINE_PAIR;
typedef struct __GENERIC_BINDING_INFO
 {
 void * pObj;
 unsigned int Size;
 GENERIC_BINDING_ROUTINE pfnBind;
 GENERIC_UNBIND_ROUTINE pfnUnbind;
 } GENERIC_BINDING_INFO, *PGENERIC_BINDING_INFO;
typedef void ( __stdcall * XMIT_HELPER_ROUTINE)
 ( PMIDL_STUB_MESSAGE );
typedef struct _XMIT_ROUTINE_QUINTUPLE
 {
 XMIT_HELPER_ROUTINE pfnTranslateToXmit;
 XMIT_HELPER_ROUTINE pfnTranslateFromXmit;
 XMIT_HELPER_ROUTINE pfnFreeXmit;
 XMIT_HELPER_ROUTINE pfnFreeInst;
 } XMIT_ROUTINE_QUINTUPLE, *PXMIT_ROUTINE_QUINTUPLE;
typedef unsigned long
( __stdcall * USER_MARSHAL_SIZING_ROUTINE)
 (unsigned long *,
 unsigned long,
 void * );
typedef unsigned char *
( __stdcall * USER_MARSHAL_MARSHALLING_ROUTINE)
 (unsigned long *,
 unsigned char * ,
 void * );
typedef unsigned char *
( __stdcall * USER_MARSHAL_UNMARSHALLING_ROUTINE)
 (unsigned long *,
 unsigned char *,
 void * );
typedef void ( __stdcall * USER_MARSHAL_FREEING_ROUTINE)
 (unsigned long *,
 void * );
typedef struct _USER_MARSHAL_ROUTINE_QUADRUPLE
 {
 USER_MARSHAL_SIZING_ROUTINE pfnBufferSize;
 USER_MARSHAL_MARSHALLING_ROUTINE pfnMarshall;
 USER_MARSHAL_UNMARSHALLING_ROUTINE pfnUnmarshall;
 USER_MARSHAL_FREEING_ROUTINE pfnFree;
 } USER_MARSHAL_ROUTINE_QUADRUPLE;
typedef enum _USER_MARSHAL_CB_TYPE
{
 USER_MARSHAL_CB_BUFFER_SIZE,
 USER_MARSHAL_CB_MARSHALL,
 USER_MARSHAL_CB_UNMARSHALL,
 USER_MARSHAL_CB_FREE
} USER_MARSHAL_CB_TYPE;
typedef struct _USER_MARSHAL_CB
{
 unsigned long Flags;
 PMIDL_STUB_MESSAGE pStubMsg;
 PFORMAT_STRING pReserve;
 unsigned long Signature;
 USER_MARSHAL_CB_TYPE CBType;
 PFORMAT_STRING pFormat;
 PFORMAT_STRING pTypeFormat;
} USER_MARSHAL_CB;
typedef struct _MALLOC_FREE_STRUCT
 {
 void * ( __stdcall * pfnAllocate)(size_t);
 void ( __stdcall * pfnFree)(void *);
 } MALLOC_FREE_STRUCT;
typedef struct _COMM_FAULT_OFFSETS
 {
 short CommOffset;
 short FaultOffset;
 } COMM_FAULT_OFFSETS;
typedef enum _IDL_CS_CONVERT
 {
 IDL_CS_NO_CONVERT,
 IDL_CS_IN_PLACE_CONVERT,
 IDL_CS_NEW_BUFFER_CONVERT
 } IDL_CS_CONVERT;
typedef void
( __stdcall * CS_TYPE_NET_SIZE_ROUTINE)
 (RPC_BINDING_HANDLE hBinding,
 unsigned long ulNetworkCodeSet,
 unsigned long ulLocalBufferSize,
 IDL_CS_CONVERT * conversionType,
 unsigned long * pulNetworkBufferSize,
 error_status_t * pStatus);
typedef void
( __stdcall * CS_TYPE_LOCAL_SIZE_ROUTINE)
 (RPC_BINDING_HANDLE hBinding,
 unsigned long ulNetworkCodeSet,
 unsigned long ulNetworkBufferSize,
 IDL_CS_CONVERT * conversionType,
 unsigned long * pulLocalBufferSize,
 error_status_t * pStatus);
typedef void
( __stdcall * CS_TYPE_TO_NETCS_ROUTINE)
 (RPC_BINDING_HANDLE hBinding,
 unsigned long ulNetworkCodeSet,
 void * pLocalData,
 unsigned long ulLocalDataLength,
 byte * pNetworkData,
 unsigned long * pulNetworkDataLength,
 error_status_t * pStatus);
typedef void
( __stdcall * CS_TYPE_FROM_NETCS_ROUTINE)
 (RPC_BINDING_HANDLE hBinding,
 unsigned long ulNetworkCodeSet,
 byte * pNetworkData,
 unsigned long ulNetworkDataLength,
 unsigned long ulLocalBufferSize,
 void * pLocalData,
 unsigned long * pulLocalDataLength,
 error_status_t * pStatus);
typedef void
( __stdcall * CS_TAG_GETTING_ROUTINE)
 (RPC_BINDING_HANDLE hBinding,
 int fServerSide,
 unsigned long * pulSendingTag,
 unsigned long * pulDesiredReceivingTag,
 unsigned long * pulReceivingTag,
 error_status_t * pStatus);
void __stdcall
RpcCsGetTags(
 RPC_BINDING_HANDLE hBinding,
 int fServerSide,
 unsigned long * pulSendingTag,
 unsigned long * pulDesiredReceivingTag,
 unsigned long * pulReceivingTag,
 error_status_t * pStatus);
typedef struct _NDR_CS_SIZE_CONVERT_ROUTINES
 {
 CS_TYPE_NET_SIZE_ROUTINE pfnNetSize;
 CS_TYPE_TO_NETCS_ROUTINE pfnToNetCs;
 CS_TYPE_LOCAL_SIZE_ROUTINE pfnLocalSize;
 CS_TYPE_FROM_NETCS_ROUTINE pfnFromNetCs;
 } NDR_CS_SIZE_CONVERT_ROUTINES;
typedef struct _NDR_CS_ROUTINES
 {
 NDR_CS_SIZE_CONVERT_ROUTINES *pSizeConvertRoutines;
 CS_TAG_GETTING_ROUTINE *pTagGettingRoutines;
 } NDR_CS_ROUTINES;
typedef struct _NDR_EXPR_DESC
{
 const unsigned short * pOffset;
 PFORMAT_STRING pFormatExpr;
} NDR_EXPR_DESC;
typedef struct _MIDL_STUB_DESC
 {
 void * RpcInterfaceInformation;
 void * ( __stdcall * pfnAllocate)(size_t);
 void ( __stdcall * pfnFree)(void *);
 union
 {
 handle_t * pAutoHandle;
 handle_t * pPrimitiveHandle;
 PGENERIC_BINDING_INFO pGenericBindingInfo;
 } IMPLICIT_HANDLE_INFO;
 const NDR_RUNDOWN * apfnNdrRundownRoutines;
 const GENERIC_BINDING_ROUTINE_PAIR * aGenericBindingRoutinePairs;
 const EXPR_EVAL * apfnExprEval;
 const XMIT_ROUTINE_QUINTUPLE * aXmitQuintuple;
 const unsigned char * pFormatTypes;
 int fCheckBounds;
 unsigned long Version;
 MALLOC_FREE_STRUCT * pMallocFreeStruct;
 long MIDLVersion;
 const COMM_FAULT_OFFSETS * CommFaultOffsets;
 const USER_MARSHAL_ROUTINE_QUADRUPLE * aUserMarshalQuadruple;
 const NDR_NOTIFY_ROUTINE * NotifyRoutineTable;
 ULONG_PTR mFlags;
 const NDR_CS_ROUTINES * CsRoutineTables;
 void * ProxyServerInfo;
 const NDR_EXPR_DESC * pExprInfo;
 } MIDL_STUB_DESC;
typedef const MIDL_STUB_DESC * PMIDL_STUB_DESC;
typedef void * PMIDL_XMIT_TYPE;
#pragma warning(push)
#pragma warning( disable:4200 )
typedef struct _MIDL_FORMAT_STRING
 {
 short Pad;
 unsigned char Format[];
 } MIDL_FORMAT_STRING;
#pragma warning(pop)
typedef void ( __stdcall * STUB_THUNK)( PMIDL_STUB_MESSAGE );
typedef long ( __stdcall * SERVER_ROUTINE)();
typedef struct _MIDL_SERVER_INFO_
 {
 PMIDL_STUB_DESC pStubDesc;
 const SERVER_ROUTINE * DispatchTable;
 PFORMAT_STRING ProcString;
 const unsigned short * FmtStringOffset;
 const STUB_THUNK * ThunkTable;
 PRPC_SYNTAX_IDENTIFIER pTransferSyntax;
 ULONG_PTR nCount;
 PMIDL_SYNTAX_INFO pSyntaxInfo;
 } MIDL_SERVER_INFO, *PMIDL_SERVER_INFO;
typedef struct _MIDL_STUBLESS_PROXY_INFO
 {
 PMIDL_STUB_DESC pStubDesc;
 PFORMAT_STRING ProcFormatString;
 const unsigned short * FormatStringOffset;
 PRPC_SYNTAX_IDENTIFIER pTransferSyntax;
 ULONG_PTR nCount;
 PMIDL_SYNTAX_INFO pSyntaxInfo;
 } MIDL_STUBLESS_PROXY_INFO;
typedef MIDL_STUBLESS_PROXY_INFO * PMIDL_STUBLESS_PROXY_INFO;
typedef struct _MIDL_SYNTAX_INFO
{
RPC_SYNTAX_IDENTIFIER TransferSyntax;
RPC_DISPATCH_TABLE * DispatchTable;
PFORMAT_STRING ProcString;
const unsigned short * FmtStringOffset;
PFORMAT_STRING TypeString;
const void * aUserMarshalQuadruple;
ULONG_PTR pReserved1;
ULONG_PTR pReserved2;
} MIDL_SYNTAX_INFO, *PMIDL_SYNTAX_INFO;
typedef unsigned short * PARAM_OFFSETTABLE, *PPARAM_OFFSETTABLE;
typedef union _CLIENT_CALL_RETURN
 {
 void * Pointer;
 LONG_PTR Simple;
 } CLIENT_CALL_RETURN;
typedef enum
 {
 XLAT_SERVER = 1,
 XLAT_CLIENT
 } XLAT_SIDE;
typedef struct _FULL_PTR_XLAT_TABLES
{
 void * RefIdToPointer;
 void * PointerToRefId;
 unsigned long NextRefId;
 XLAT_SIDE XlatSide;
} FULL_PTR_XLAT_TABLES, *PFULL_PTR_XLAT_TABLES;
RPC_STATUS __stdcall
NdrClientGetSupportedSyntaxes(
 RPC_CLIENT_INTERFACE * pInf,
 unsigned long * pCount,
 MIDL_SYNTAX_INFO ** pArr );
RPC_STATUS __stdcall
NdrServerGetSupportedSyntaxes(
 RPC_SERVER_INTERFACE * pInf,
 unsigned long * pCount,
 MIDL_SYNTAX_INFO ** pArr,
 unsigned long * pPreferSyntaxIndex);
__declspec(dllimport)
void
__stdcall
NdrSimpleTypeMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 unsigned char FormatChar
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrPointerMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrCsArrayMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrCsTagMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrSimpleStructMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrConformantStructMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrConformantVaryingStructMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrComplexStructMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrFixedArrayMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrConformantArrayMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrConformantVaryingArrayMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrVaryingArrayMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrComplexArrayMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrNonConformantStringMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrConformantStringMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrEncapsulatedUnionMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrNonEncapsulatedUnionMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrByteCountPointerMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrXmitOrRepAsMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrUserMarshalMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrInterfacePointerMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrClientContextMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 NDR_CCONTEXT ContextHandle,
 int fCheck
 );
__declspec(dllimport)
void
__stdcall
NdrServerContextMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 NDR_SCONTEXT ContextHandle,
 NDR_RUNDOWN RundownRoutine
 );
__declspec(dllimport)
void
__stdcall
NdrServerContextNewMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 NDR_SCONTEXT ContextHandle,
 NDR_RUNDOWN RundownRoutine,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrSimpleTypeUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 unsigned char FormatChar
 );
__declspec(dllimport)
unsigned char * __stdcall
__stdcall
NdrCsArrayUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char ** ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char * __stdcall
__stdcall
NdrCsTagUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char ** ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char * __stdcall
NdrRangeUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char ** ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
void
__stdcall
NdrCorrelationInitialize(
 PMIDL_STUB_MESSAGE pStubMsg,
 void * pMemory,
 unsigned long CacheSize,
 unsigned long flags
 );
__declspec(dllimport)
void
__stdcall
NdrCorrelationPass(
 PMIDL_STUB_MESSAGE pStubMsg
 );
__declspec(dllimport)
void
__stdcall
NdrCorrelationFree(
 PMIDL_STUB_MESSAGE pStubMsg
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrPointerUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrSimpleStructUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrConformantStructUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrConformantVaryingStructUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrComplexStructUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrFixedArrayUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrConformantArrayUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrConformantVaryingArrayUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrVaryingArrayUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrComplexArrayUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrNonConformantStringUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrConformantStringUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrEncapsulatedUnionUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrNonEncapsulatedUnionUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrByteCountPointerUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrXmitOrRepAsUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrUserMarshalUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrInterfacePointerUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * * ppMemory,
 PFORMAT_STRING pFormat,
 unsigned char fMustAlloc
 );
__declspec(dllimport)
void
__stdcall
NdrClientContextUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 NDR_CCONTEXT * pContextHandle,
 RPC_BINDING_HANDLE BindHandle
 );
__declspec(dllimport)
NDR_SCONTEXT
__stdcall
NdrServerContextUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg
 );
__declspec(dllimport)
NDR_SCONTEXT
__stdcall
NdrContextHandleInitialize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
NDR_SCONTEXT
__stdcall
NdrServerContextNewUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrPointerBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrCsArrayBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrCsTagBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrSimpleStructBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrConformantStructBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrConformantVaryingStructBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrComplexStructBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrFixedArrayBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrConformantArrayBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrConformantVaryingArrayBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrVaryingArrayBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrComplexArrayBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrConformantStringBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrNonConformantStringBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrEncapsulatedUnionBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrNonEncapsulatedUnionBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrByteCountPointerBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrXmitOrRepAsBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrUserMarshalBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrInterfacePointerBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrContextHandleSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrPointerMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrContextHandleMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrCsArrayMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrCsTagMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrSimpleStructMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrConformantStructMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrConformantVaryingStructMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrComplexStructMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrFixedArrayMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrConformantArrayMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrConformantVaryingArrayMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrVaryingArrayMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrComplexArrayMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrConformantStringMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrNonConformantStringMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrEncapsulatedUnionMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrNonEncapsulatedUnionMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrXmitOrRepAsMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrUserMarshalMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned long
__stdcall
NdrInterfacePointerMemorySize(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrPointerFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrCsArrayFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrSimpleStructFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrConformantStructFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrConformantVaryingStructFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrComplexStructFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrFixedArrayFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrConformantArrayFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrConformantVaryingArrayFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrVaryingArrayFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrComplexArrayFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrEncapsulatedUnionFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrNonEncapsulatedUnionFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrByteCountPointerFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrXmitOrRepAsFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrUserMarshalFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrInterfacePointerFree(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pMemory,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
void
__stdcall
NdrConvert2(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat,
 long NumberParams
 );
__declspec(dllimport)
void
__stdcall
NdrConvert(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrUserMarshalSimpleTypeConvert(
 unsigned long * pFlags,
 unsigned char * pBuffer,
 unsigned char FormatChar
 );
__declspec(dllimport)
void
__stdcall
NdrClientInitializeNew(
 PRPC_MESSAGE pRpcMsg,
 PMIDL_STUB_MESSAGE pStubMsg,
 PMIDL_STUB_DESC pStubDescriptor,
 unsigned int ProcNum
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrServerInitializeNew(
 PRPC_MESSAGE pRpcMsg,
 PMIDL_STUB_MESSAGE pStubMsg,
 PMIDL_STUB_DESC pStubDescriptor
 );
__declspec(dllimport)
void
__stdcall
NdrServerInitializePartial(
 PRPC_MESSAGE pRpcMsg,
 PMIDL_STUB_MESSAGE pStubMsg,
 PMIDL_STUB_DESC pStubDescriptor,
 unsigned long RequestedBufferSize
 );
__declspec(dllimport)
void
__stdcall
NdrClientInitialize(
 PRPC_MESSAGE pRpcMsg,
 PMIDL_STUB_MESSAGE pStubMsg,
 PMIDL_STUB_DESC pStubDescriptor,
 unsigned int ProcNum
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrServerInitialize(
 PRPC_MESSAGE pRpcMsg,
 PMIDL_STUB_MESSAGE pStubMsg,
 PMIDL_STUB_DESC pStubDescriptor
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrServerInitializeUnmarshall (
 PMIDL_STUB_MESSAGE pStubMsg,
 PMIDL_STUB_DESC pStubDescriptor,
 PRPC_MESSAGE pRpcMsg
 );
__declspec(dllimport)
void
__stdcall
NdrServerInitializeMarshall (
 PRPC_MESSAGE pRpcMsg,
 PMIDL_STUB_MESSAGE pStubMsg
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrGetBuffer(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned long BufferLength,
 RPC_BINDING_HANDLE Handle
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrNsGetBuffer(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned long BufferLength,
 RPC_BINDING_HANDLE Handle
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrSendReceive(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pBufferEnd
 );
__declspec(dllimport)
unsigned char *
__stdcall
NdrNsSendReceive(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned char * pBufferEnd,
 RPC_BINDING_HANDLE * pAutoHandle
 );
__declspec(dllimport)
void
__stdcall
NdrFreeBuffer(
 PMIDL_STUB_MESSAGE pStubMsg
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
NdrGetDcomProtocolVersion(
 PMIDL_STUB_MESSAGE pStubMsg,
 RPC_VERSION * pVersion );
CLIENT_CALL_RETURN __cdecl
NdrClientCall2(
 PMIDL_STUB_DESC pStubDescriptor,
 PFORMAT_STRING pFormat,
 ...
 );
CLIENT_CALL_RETURN __cdecl
NdrClientCall(
 PMIDL_STUB_DESC pStubDescriptor,
 PFORMAT_STRING pFormat,
 ...
 );
CLIENT_CALL_RETURN __cdecl
NdrAsyncClientCall(
 PMIDL_STUB_DESC pStubDescriptor,
 PFORMAT_STRING pFormat,
 ...
 );
CLIENT_CALL_RETURN __cdecl
NdrDcomAsyncClientCall(
 PMIDL_STUB_DESC pStubDescriptor,
 PFORMAT_STRING pFormat,
 ...
 );
typedef enum {
 STUB_UNMARSHAL,
 STUB_CALL_SERVER,
 STUB_MARSHAL,
 STUB_CALL_SERVER_NO_HRESULT
}STUB_PHASE;
typedef enum {
 PROXY_CALCSIZE,
 PROXY_GETBUFFER,
 PROXY_MARSHAL,
 PROXY_SENDRECEIVE,
 PROXY_UNMARSHAL
}PROXY_PHASE;
struct IRpcStubBuffer;
__declspec(dllimport)
void
__stdcall
NdrAsyncServerCall(
 PRPC_MESSAGE pRpcMsg
 );
__declspec(dllimport)
long
__stdcall
NdrAsyncStubCall(
 struct IRpcStubBuffer * pThis,
 struct IRpcChannelBuffer * pChannel,
 PRPC_MESSAGE pRpcMsg,
 unsigned long * pdwStubPhase
 );
__declspec(dllimport)
long
__stdcall
NdrDcomAsyncStubCall(
 struct IRpcStubBuffer * pThis,
 struct IRpcChannelBuffer * pChannel,
 PRPC_MESSAGE pRpcMsg,
 unsigned long * pdwStubPhase
 );
__declspec(dllimport)
long
__stdcall
NdrStubCall2(
 struct IRpcStubBuffer * pThis,
 struct IRpcChannelBuffer * pChannel,
 PRPC_MESSAGE pRpcMsg,
 unsigned long * pdwStubPhase
 );
__declspec(dllimport)
void
__stdcall
NdrServerCall2(
 PRPC_MESSAGE pRpcMsg
 );
__declspec(dllimport)
long
__stdcall
NdrStubCall (
 struct IRpcStubBuffer * pThis,
 struct IRpcChannelBuffer * pChannel,
 PRPC_MESSAGE pRpcMsg,
 unsigned long * pdwStubPhase
 );
__declspec(dllimport)
void
__stdcall
NdrServerCall(
 PRPC_MESSAGE pRpcMsg
 );
__declspec(dllimport)
int
__stdcall
NdrServerUnmarshall(
 struct IRpcChannelBuffer * pChannel,
 PRPC_MESSAGE pRpcMsg,
 PMIDL_STUB_MESSAGE pStubMsg,
 PMIDL_STUB_DESC pStubDescriptor,
 PFORMAT_STRING pFormat,
 void * pParamList
 );
__declspec(dllimport)
void
__stdcall
NdrServerMarshall(
 struct IRpcStubBuffer * pThis,
 struct IRpcChannelBuffer * pChannel,
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
NdrMapCommAndFaultStatus(
 PMIDL_STUB_MESSAGE pStubMsg,
 unsigned long * pCommStatus,
 unsigned long * pFaultStatus,
 RPC_STATUS Status
 );
typedef void * RPC_SS_THREAD_HANDLE;
typedef void * __stdcall
RPC_CLIENT_ALLOC (
 size_t Size
 );
typedef void __stdcall
RPC_CLIENT_FREE (
 void * Ptr
 );
__declspec(dllimport)
void *
__stdcall
RpcSsAllocate (
 size_t Size
 );
__declspec(dllimport)
void
__stdcall
RpcSsDisableAllocate (
 void
 );
__declspec(dllimport)
void
__stdcall
RpcSsEnableAllocate (
 void
 );
__declspec(dllimport)
void
__stdcall
RpcSsFree (
 void * NodeToFree
 );
__declspec(dllimport)
RPC_SS_THREAD_HANDLE
__stdcall
RpcSsGetThreadHandle (
 void
 );
__declspec(dllimport)
void
__stdcall
RpcSsSetClientAllocFree (
 RPC_CLIENT_ALLOC * ClientAlloc,
 RPC_CLIENT_FREE * ClientFree
 );
__declspec(dllimport)
void
__stdcall
RpcSsSetThreadHandle (
 RPC_SS_THREAD_HANDLE Id
 );
__declspec(dllimport)
void
__stdcall
RpcSsSwapClientAllocFree (
 RPC_CLIENT_ALLOC * ClientAlloc,
 RPC_CLIENT_FREE * ClientFree,
 RPC_CLIENT_ALLOC * * OldClientAlloc,
 RPC_CLIENT_FREE * * OldClientFree
 );
__declspec(dllimport)
void *
__stdcall
RpcSmAllocate (
 size_t Size,
 RPC_STATUS * pStatus
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcSmClientFree (
 void * pNodeToFree
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcSmDestroyClientContext (
 void * * ContextHandle
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcSmDisableAllocate (
 void
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcSmEnableAllocate (
 void
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcSmFree (
 void * NodeToFree
 );
__declspec(dllimport)
RPC_SS_THREAD_HANDLE
__stdcall
RpcSmGetThreadHandle (
 RPC_STATUS * pStatus
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcSmSetClientAllocFree (
 RPC_CLIENT_ALLOC * ClientAlloc,
 RPC_CLIENT_FREE * ClientFree
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcSmSetThreadHandle (
 RPC_SS_THREAD_HANDLE Id
 );
__declspec(dllimport)
RPC_STATUS
__stdcall
RpcSmSwapClientAllocFree (
 RPC_CLIENT_ALLOC * ClientAlloc,
 RPC_CLIENT_FREE * ClientFree,
 RPC_CLIENT_ALLOC * * OldClientAlloc,
 RPC_CLIENT_FREE * * OldClientFree
 );
__declspec(dllimport)
void
__stdcall
NdrRpcSsEnableAllocate(
 PMIDL_STUB_MESSAGE pMessage );
__declspec(dllimport)
void
__stdcall
NdrRpcSsDisableAllocate(
 PMIDL_STUB_MESSAGE pMessage );
__declspec(dllimport)
void
__stdcall
NdrRpcSmSetClientToOsf(
 PMIDL_STUB_MESSAGE pMessage );
__declspec(dllimport)
void *
__stdcall
NdrRpcSmClientAllocate (
 size_t Size
 );
__declspec(dllimport)
void
__stdcall
NdrRpcSmClientFree (
 void * NodeToFree
 );
__declspec(dllimport)
void *
__stdcall
NdrRpcSsDefaultAllocate (
 size_t Size
 );
__declspec(dllimport)
void
__stdcall
NdrRpcSsDefaultFree (
 void * NodeToFree
 );
__declspec(dllimport)
PFULL_PTR_XLAT_TABLES
__stdcall
NdrFullPointerXlatInit(
 unsigned long NumberOfPointers,
 XLAT_SIDE XlatSide
 );
__declspec(dllimport)
void
__stdcall
NdrFullPointerXlatFree(
 PFULL_PTR_XLAT_TABLES pXlatTables
 );
__declspec(dllimport)
void *
__stdcall
NdrAllocate(
 PMIDL_STUB_MESSAGE pStubMsg,
 size_t Len
 );
__declspec(dllimport)
void
__stdcall
NdrClearOutParameters(
 PMIDL_STUB_MESSAGE pStubMsg,
 PFORMAT_STRING pFormat,
 void * ArgAddr
 );
__declspec(dllimport)
void *
__stdcall
NdrOleAllocate (
 size_t Size
 );
__declspec(dllimport)
void
__stdcall
NdrOleFree (
 void * NodeToFree
 );
typedef struct _NDR_USER_MARSHAL_INFO_LEVEL1
{
 void * Buffer;
 unsigned long BufferSize;
 void *(__stdcall * pfnAllocate)(size_t);
 void (__stdcall * pfnFree)(void *);
 struct IRpcChannelBuffer * pRpcChannelBuffer;
 ULONG_PTR Reserved[5];
} NDR_USER_MARSHAL_INFO_LEVEL1;
#pragma warning(push)
#pragma warning(disable:4201)
typedef struct _NDR_USER_MARSHAL_INFO
{
 unsigned long InformationLevel;
 union {
 NDR_USER_MARSHAL_INFO_LEVEL1 Level1;
 };
} NDR_USER_MARSHAL_INFO;
#pragma warning(pop)
RPC_STATUS
__stdcall
NdrGetUserMarshalInfo (
 unsigned long * pFlags,
 unsigned long InformationLevel,
 NDR_USER_MARSHAL_INFO * pMarshalInfo
 );
RPC_STATUS __stdcall
NdrCreateServerInterfaceFromStub(
 struct IRpcStubBuffer* pStub,
 RPC_SERVER_INTERFACE *pServerIf );
CLIENT_CALL_RETURN __cdecl
NdrClientCall3(
 MIDL_STUBLESS_PROXY_INFO *pProxyInfo,
 unsigned long nProcNum,
 void * pReturnValue,
 ...
 );
CLIENT_CALL_RETURN __cdecl
Ndr64AsyncClientCall(
 MIDL_STUBLESS_PROXY_INFO *pProxyInfo,
 unsigned long nProcNum,
 void * pReturnValue,
 ...
 );
CLIENT_CALL_RETURN __cdecl
Ndr64DcomAsyncClientCall(
 MIDL_STUBLESS_PROXY_INFO *pProxyInfo,
 unsigned long nProcNum,
 void * pReturnValue,
 ...
 );
struct IRpcStubBuffer;
__declspec(dllimport)
void
__stdcall
Ndr64AsyncServerCall(
 PRPC_MESSAGE pRpcMsg
 );
__declspec(dllimport)
void
__stdcall
Ndr64AsyncServerCall64(
 PRPC_MESSAGE pRpcMsg
 );
__declspec(dllimport)
void
__stdcall
Ndr64AsyncServerCallAll(
 PRPC_MESSAGE pRpcMsg
 );
__declspec(dllimport)
long
__stdcall
Ndr64AsyncStubCall(
 struct IRpcStubBuffer * pThis,
 struct IRpcChannelBuffer * pChannel,
 PRPC_MESSAGE pRpcMsg,
 unsigned long * pdwStubPhase
 );
__declspec(dllimport)
long
__stdcall
Ndr64DcomAsyncStubCall(
 struct IRpcStubBuffer * pThis,
 struct IRpcChannelBuffer * pChannel,
 PRPC_MESSAGE pRpcMsg,
 unsigned long * pdwStubPhase
 );
__declspec(dllimport)
long
__stdcall
NdrStubCall3 (
 struct IRpcStubBuffer * pThis,
 struct IRpcChannelBuffer * pChannel,
 PRPC_MESSAGE pRpcMsg,
 unsigned long * pdwStubPhase
 );
__declspec(dllimport)
void
__stdcall
NdrServerCallAll(
 PRPC_MESSAGE pRpcMsg
 );
__declspec(dllimport)
void
__stdcall
NdrServerCallNdr64(
 PRPC_MESSAGE pRpcMsg
 );
__declspec(dllimport)
void
__stdcall
NdrServerCall3(
 PRPC_MESSAGE pRpcMsg
 );
__declspec(dllimport)
void
__stdcall
NdrPartialIgnoreClientMarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 void * pMemory
 );
__declspec(dllimport)
void
__stdcall
NdrPartialIgnoreServerUnmarshall(
 PMIDL_STUB_MESSAGE pStubMsg,
 void ** ppMemory
 );
__declspec(dllimport)
void
__stdcall
NdrPartialIgnoreClientBufferSize(
 PMIDL_STUB_MESSAGE pStubMsg,
 void * pMemory
 );
__declspec(dllimport)
void
__stdcall
NdrPartialIgnoreServerInitialize(
 PMIDL_STUB_MESSAGE pStubMsg,
 void ** ppMemory,
 PFORMAT_STRING pFormat
 );
void __stdcall
RpcUserFree( handle_t AsyncHandle, void * pBuffer );
}
#pragma warning(disable:4103)
#pragma pack(pop)
#pragma once
#pragma warning(disable:4103)
#pragma pack(push,8)
extern "C++"
{
 template<typename T> void** IID_PPV_ARGS_Helper(T** pp)
 {
 static_cast<IUnknown*>(*pp);
 return reinterpret_cast<void**>(pp);
 }
}
typedef enum tagREGCLS
{
 REGCLS_SINGLEUSE = 0,
 REGCLS_MULTIPLEUSE = 1,
 REGCLS_MULTI_SEPARATE = 2,
 REGCLS_SUSPENDED = 4,
 REGCLS_SURROGATE = 8
} REGCLS;
typedef DWORD STGFMT;
typedef struct IRpcStubBuffer IRpcStubBuffer;
typedef struct IRpcChannelBuffer IRpcChannelBuffer;
#pragma warning( disable: 4049 )
#pragma once
extern "C"{
#pragma once
extern RPC_IF_HANDLE __MIDL_itf_wtypes_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_wtypes_0000_0000_v0_0_s_ifspec;
typedef struct tagRemHGLOBAL
 {
 long fNullHGlobal;
 unsigned long cbData;
 byte data[ 1 ];
 } RemHGLOBAL;
typedef struct tagRemHMETAFILEPICT
 {
 long mm;
 long xExt;
 long yExt;
 unsigned long cbData;
 byte data[ 1 ];
 } RemHMETAFILEPICT;
typedef struct tagRemHENHMETAFILE
 {
 unsigned long cbData;
 byte data[ 1 ];
 } RemHENHMETAFILE;
typedef struct tagRemHBITMAP
 {
 unsigned long cbData;
 byte data[ 1 ];
 } RemHBITMAP;
typedef struct tagRemHPALETTE
 {
 unsigned long cbData;
 byte data[ 1 ];
 } RemHPALETTE;
typedef struct tagRemBRUSH
 {
 unsigned long cbData;
 byte data[ 1 ];
 } RemHBRUSH;
typedef WCHAR OLECHAR;
typedef OLECHAR *LPOLESTR;
typedef const OLECHAR *LPCOLESTR;
typedef unsigned char UCHAR;
typedef short SHORT;
typedef unsigned short USHORT;
typedef DWORD ULONG;
typedef double DOUBLE;
typedef struct _COAUTHIDENTITY
 {
 USHORT *User;
 ULONG UserLength;
 USHORT *Domain;
 ULONG DomainLength;
 USHORT *Password;
 ULONG PasswordLength;
 ULONG Flags;
 } COAUTHIDENTITY;
typedef struct _COAUTHINFO
 {
 DWORD dwAuthnSvc;
 DWORD dwAuthzSvc;
 LPWSTR pwszServerPrincName;
 DWORD dwAuthnLevel;
 DWORD dwImpersonationLevel;
 COAUTHIDENTITY *pAuthIdentityData;
 DWORD dwCapabilities;
 } COAUTHINFO;
typedef LONG SCODE;
typedef SCODE *PSCODE;
typedef
enum tagMEMCTX
 { MEMCTX_TASK = 1,
 MEMCTX_SHARED = 2,
 MEMCTX_MACSYSTEM = 3,
 MEMCTX_UNKNOWN = -1,
 MEMCTX_SAME = -2
 } MEMCTX;
typedef
enum tagCLSCTX
 { CLSCTX_INPROC_SERVER = 0x1,
 CLSCTX_INPROC_HANDLER = 0x2,
 CLSCTX_LOCAL_SERVER = 0x4,
 CLSCTX_INPROC_SERVER16 = 0x8,
 CLSCTX_REMOTE_SERVER = 0x10,
 CLSCTX_INPROC_HANDLER16 = 0x20,
 CLSCTX_RESERVED1 = 0x40,
 CLSCTX_RESERVED2 = 0x80,
 CLSCTX_RESERVED3 = 0x100,
 CLSCTX_RESERVED4 = 0x200,
 CLSCTX_NO_CODE_DOWNLOAD = 0x400,
 CLSCTX_RESERVED5 = 0x800,
 CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
 CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
 CLSCTX_NO_FAILURE_LOG = 0x4000,
 CLSCTX_DISABLE_AAA = 0x8000,
 CLSCTX_ENABLE_AAA = 0x10000,
 CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
 CLSCTX_ACTIVATE_32_BIT_SERVER = 0x40000,
 CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000,
 CLSCTX_ENABLE_CLOAKING = 0x100000,
 CLSCTX_PS_DLL = 0x80000000
 } CLSCTX;
typedef
enum tagMSHLFLAGS
 { MSHLFLAGS_NORMAL = 0,
 MSHLFLAGS_TABLESTRONG = 1,
 MSHLFLAGS_TABLEWEAK = 2,
 MSHLFLAGS_NOPING = 4,
 MSHLFLAGS_RESERVED1 = 8,
 MSHLFLAGS_RESERVED2 = 16,
 MSHLFLAGS_RESERVED3 = 32,
 MSHLFLAGS_RESERVED4 = 64
 } MSHLFLAGS;
typedef
enum tagMSHCTX
 { MSHCTX_LOCAL = 0,
 MSHCTX_NOSHAREDMEM = 1,
 MSHCTX_DIFFERENTMACHINE = 2,
 MSHCTX_INPROC = 3,
 MSHCTX_CROSSCTX = 4
 } MSHCTX;
typedef
enum tagDVASPECT
 { DVASPECT_CONTENT = 1,
 DVASPECT_THUMBNAIL = 2,
 DVASPECT_ICON = 4,
 DVASPECT_DOCPRINT = 8
 } DVASPECT;
typedef
enum tagSTGC
 { STGC_DEFAULT = 0,
 STGC_OVERWRITE = 1,
 STGC_ONLYIFCURRENT = 2,
 STGC_DANGEROUSLYCOMMITMERELYTODISKCACHE = 4,
 STGC_CONSOLIDATE = 8
 } STGC;
typedef
enum tagSTGMOVE
 { STGMOVE_MOVE = 0,
 STGMOVE_COPY = 1,
 STGMOVE_SHALLOWCOPY = 2
 } STGMOVE;
typedef
enum tagSTATFLAG
 { STATFLAG_DEFAULT = 0,
 STATFLAG_NONAME = 1,
 STATFLAG_NOOPEN = 2
 } STATFLAG;
typedef void *HCONTEXT;
typedef DWORD LCID;
typedef USHORT LANGID;
typedef struct _BYTE_BLOB
 {
 unsigned long clSize;
 byte abData[ 1 ];
 } BYTE_BLOB;
typedef BYTE_BLOB *UP_BYTE_BLOB;
typedef struct _WORD_BLOB
 {
 unsigned long clSize;
 unsigned short asData[ 1 ];
 } WORD_BLOB;
typedef WORD_BLOB *UP_WORD_BLOB;
typedef struct _DWORD_BLOB
 {
 unsigned long clSize;
 unsigned long alData[ 1 ];
 } DWORD_BLOB;
typedef DWORD_BLOB *UP_DWORD_BLOB;
typedef struct _FLAGGED_BYTE_BLOB
 {
 unsigned long fFlags;
 unsigned long clSize;
 byte abData[ 1 ];
 } FLAGGED_BYTE_BLOB;
typedef FLAGGED_BYTE_BLOB *UP_FLAGGED_BYTE_BLOB;
typedef struct _FLAGGED_WORD_BLOB
 {
 unsigned long fFlags;
 unsigned long clSize;
 unsigned short asData[ 1 ];
 } FLAGGED_WORD_BLOB;
typedef FLAGGED_WORD_BLOB *UP_FLAGGED_WORD_BLOB;
typedef struct _BYTE_SIZEDARR
 {
 unsigned long clSize;
 byte *pData;
 } BYTE_SIZEDARR;
typedef struct _SHORT_SIZEDARR
 {
 unsigned long clSize;
 unsigned short *pData;
 } WORD_SIZEDARR;
typedef struct _LONG_SIZEDARR
 {
 unsigned long clSize;
 unsigned long *pData;
 } DWORD_SIZEDARR;
typedef struct _HYPER_SIZEDARR
 {
 unsigned long clSize;
 __int64 *pData;
 } HYPER_SIZEDARR;
typedef struct _userCLIPFORMAT
 {
 long fContext;
 union __MIDL_IWinTypes_0001
 {
 DWORD dwValue;
 wchar_t *pwszName;
 } u;
 } userCLIPFORMAT;
typedef userCLIPFORMAT *wireCLIPFORMAT;
typedef WORD CLIPFORMAT;
typedef struct _GDI_NONREMOTE
 {
 long fContext;
 union __MIDL_IWinTypes_0002
 {
 long hInproc;
 DWORD_BLOB *hRemote;
 } u;
 } GDI_NONREMOTE;
typedef struct _userHGLOBAL
 {
 long fContext;
 union __MIDL_IWinTypes_0003
 {
 long hInproc;
 FLAGGED_BYTE_BLOB *hRemote;
 __int64 hInproc64;
 } u;
 } userHGLOBAL;
typedef userHGLOBAL *wireHGLOBAL;
typedef struct _userHMETAFILE
 {
 long fContext;
 union __MIDL_IWinTypes_0004
 {
 long hInproc;
 BYTE_BLOB *hRemote;
 __int64 hInproc64;
 } u;
 } userHMETAFILE;
typedef struct _remoteMETAFILEPICT
 {
 long mm;
 long xExt;
 long yExt;
 userHMETAFILE *hMF;
 } remoteMETAFILEPICT;
typedef struct _userHMETAFILEPICT
 {
 long fContext;
 union __MIDL_IWinTypes_0005
 {
 long hInproc;
 remoteMETAFILEPICT *hRemote;
 __int64 hInproc64;
 } u;
 } userHMETAFILEPICT;
typedef struct _userHENHMETAFILE
 {
 long fContext;
 union __MIDL_IWinTypes_0006
 {
 long hInproc;
 BYTE_BLOB *hRemote;
 __int64 hInproc64;
 } u;
 } userHENHMETAFILE;
typedef struct _userBITMAP
 {
 LONG bmType;
 LONG bmWidth;
 LONG bmHeight;
 LONG bmWidthBytes;
 WORD bmPlanes;
 WORD bmBitsPixel;
 ULONG cbSize;
 byte pBuffer[ 1 ];
 } userBITMAP;
typedef struct _userHBITMAP
 {
 long fContext;
 union __MIDL_IWinTypes_0007
 {
 long hInproc;
 userBITMAP *hRemote;
 __int64 hInproc64;
 } u;
 } userHBITMAP;
typedef struct _userHPALETTE
 {
 long fContext;
 union __MIDL_IWinTypes_0008
 {
 long hInproc;
 LOGPALETTE *hRemote;
 __int64 hInproc64;
 } u;
 } userHPALETTE;
typedef struct _RemotableHandle
 {
 long fContext;
 union __MIDL_IWinTypes_0009
 {
 long hInproc;
 long hRemote;
 } u;
 } RemotableHandle;
typedef RemotableHandle *wireHWND;
typedef RemotableHandle *wireHMENU;
typedef RemotableHandle *wireHACCEL;
typedef RemotableHandle *wireHBRUSH;
typedef RemotableHandle *wireHFONT;
typedef RemotableHandle *wireHDC;
typedef RemotableHandle *wireHICON;
typedef RemotableHandle *wireHRGN;
typedef struct tagTEXTMETRICW
 {
 LONG tmHeight;
 LONG tmAscent;
 LONG tmDescent;
 LONG tmInternalLeading;
 LONG tmExternalLeading;
 LONG tmAveCharWidth;
 LONG tmMaxCharWidth;
 LONG tmWeight;
 LONG tmOverhang;
 LONG tmDigitizedAspectX;
 LONG tmDigitizedAspectY;
 WCHAR tmFirstChar;
 WCHAR tmLastChar;
 WCHAR tmDefaultChar;
 WCHAR tmBreakChar;
 BYTE tmItalic;
 BYTE tmUnderlined;
 BYTE tmStruckOut;
 BYTE tmPitchAndFamily;
 BYTE tmCharSet;
 } TEXTMETRICW;
typedef struct tagTEXTMETRICW *PTEXTMETRICW;
typedef struct tagTEXTMETRICW *LPTEXTMETRICW;
typedef userHBITMAP *wireHBITMAP;
typedef userHPALETTE *wireHPALETTE;
typedef userHENHMETAFILE *wireHENHMETAFILE;
typedef userHMETAFILE *wireHMETAFILE;
typedef userHMETAFILEPICT *wireHMETAFILEPICT;
typedef void *HMETAFILEPICT;
extern RPC_IF_HANDLE IWinTypes_v0_1_c_ifspec;
extern RPC_IF_HANDLE IWinTypes_v0_1_s_ifspec;
#pragma warning(push)
#pragma warning(disable:4201)
typedef double DATE;
typedef union tagCY {
 struct {
 unsigned long Lo;
 long Hi;
 };
 LONGLONG int64;
} CY;
typedef CY *LPCY;
typedef struct tagDEC {
 USHORT wReserved;
 union {
 struct {
 BYTE scale;
 BYTE sign;
 };
 USHORT signscale;
 };
 ULONG Hi32;
 union {
 struct {
 ULONG Lo32;
 ULONG Mid32;
 };
 ULONGLONG Lo64;
 };
} DECIMAL;
typedef DECIMAL *LPDECIMAL;
#pragma warning(pop)
typedef FLAGGED_WORD_BLOB *wireBSTR;
typedef OLECHAR *BSTR;
typedef BSTR *LPBSTR;
typedef short VARIANT_BOOL;
typedef boolean BOOLEAN;
typedef struct tagBSTRBLOB
 {
 ULONG cbSize;
 BYTE *pData;
 } BSTRBLOB;
typedef struct tagBSTRBLOB *LPBSTRBLOB;
typedef struct tagCLIPDATA
 {
 ULONG cbSize;
 long ulClipFmt;
 BYTE *pClipData;
 } CLIPDATA;
typedef unsigned short VARTYPE;
enum VARENUM
 { VT_EMPTY = 0,
 VT_NULL = 1,
 VT_I2 = 2,
 VT_I4 = 3,
 VT_R4 = 4,
 VT_R8 = 5,
 VT_CY = 6,
 VT_DATE = 7,
 VT_BSTR = 8,
 VT_DISPATCH = 9,
 VT_ERROR = 10,
 VT_BOOL = 11,
 VT_VARIANT = 12,
 VT_UNKNOWN = 13,
 VT_DECIMAL = 14,
 VT_I1 = 16,
 VT_UI1 = 17,
 VT_UI2 = 18,
 VT_UI4 = 19,
 VT_I8 = 20,
 VT_UI8 = 21,
 VT_INT = 22,
 VT_UINT = 23,
 VT_VOID = 24,
 VT_HRESULT = 25,
 VT_PTR = 26,
 VT_SAFEARRAY = 27,
 VT_CARRAY = 28,
 VT_USERDEFINED = 29,
 VT_LPSTR = 30,
 VT_LPWSTR = 31,
 VT_RECORD = 36,
 VT_INT_PTR = 37,
 VT_UINT_PTR = 38,
 VT_FILETIME = 64,
 VT_BLOB = 65,
 VT_STREAM = 66,
 VT_STORAGE = 67,
 VT_STREAMED_OBJECT = 68,
 VT_STORED_OBJECT = 69,
 VT_BLOB_OBJECT = 70,
 VT_CF = 71,
 VT_CLSID = 72,
 VT_VERSIONED_STREAM = 73,
 VT_BSTR_BLOB = 0xfff,
 VT_VECTOR = 0x1000,
 VT_ARRAY = 0x2000,
 VT_BYREF = 0x4000,
 VT_RESERVED = 0x8000,
 VT_ILLEGAL = 0xffff,
 VT_ILLEGALMASKED = 0xfff,
 VT_TYPEMASK = 0xfff
 } ;
typedef ULONG PROPID;
typedef struct _tagpropertykey
 {
 GUID fmtid;
 DWORD pid;
 } PROPERTYKEY;
typedef struct tagCSPLATFORM
 {
 DWORD dwPlatformId;
 DWORD dwVersionHi;
 DWORD dwVersionLo;
 DWORD dwProcessorArch;
 } CSPLATFORM;
typedef struct tagQUERYCONTEXT
 {
 DWORD dwContext;
 CSPLATFORM Platform;
 LCID Locale;
 DWORD dwVersionHi;
 DWORD dwVersionLo;
 } QUERYCONTEXT;
typedef
enum tagTYSPEC
 { TYSPEC_CLSID = 0,
 TYSPEC_FILEEXT = ( TYSPEC_CLSID + 1 ) ,
 TYSPEC_MIMETYPE = ( TYSPEC_FILEEXT + 1 ) ,
 TYSPEC_FILENAME = ( TYSPEC_MIMETYPE + 1 ) ,
 TYSPEC_PROGID = ( TYSPEC_FILENAME + 1 ) ,
 TYSPEC_PACKAGENAME = ( TYSPEC_PROGID + 1 ) ,
 TYSPEC_OBJECTID = ( TYSPEC_PACKAGENAME + 1 )
 } TYSPEC;
typedef struct __MIDL___MIDL_itf_wtypes_0000_0001_0001
 {
 DWORD tyspec;
 union __MIDL___MIDL_itf_wtypes_0000_0001_0005
 {
 CLSID clsid;
 LPOLESTR pFileExt;
 LPOLESTR pMimeType;
 LPOLESTR pProgId;
 LPOLESTR pFileName;
 struct
 {
 LPOLESTR pPackageName;
 GUID PolicyId;
 } ByName;
 struct
 {
 GUID ObjectId;
 GUID PolicyId;
 } ByObjectId;
 } tagged_union;
 } uCLSSPEC;
extern RPC_IF_HANDLE __MIDL_itf_wtypes_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_wtypes_0000_0001_v0_0_s_ifspec;
}
#pragma warning( disable: 4049 )
#pragma once
typedef struct IUnknown IUnknown;
typedef struct AsyncIUnknown AsyncIUnknown;
typedef struct IClassFactory IClassFactory;
extern "C"{
#pragma once
extern RPC_IF_HANDLE __MIDL_itf_unknwn_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_unknwn_0000_0000_v0_0_s_ifspec;
typedef IUnknown *LPUNKNOWN;
 extern "C" const IID IID_IUnknown;
 extern "C++"
 {
 struct __declspec(uuid("00000000-0000-0000-C000-000000000046")) __declspec(novtable)
 IUnknown
 {
 public:
 virtual HRESULT __stdcall QueryInterface(
 const IID & riid,
 void * *ppvObject) = 0;
 virtual ULONG __stdcall AddRef( void) = 0;
 virtual ULONG __stdcall Release( void) = 0;
 template<class Q>
 HRESULT
 __stdcall
 QueryInterface(Q** pp)
 {
 return QueryInterface(__uuidof(Q), (void **)pp);
 }
 };
 }
 HRESULT __stdcall IUnknown_QueryInterface_Proxy(
 IUnknown * This,
 const IID & riid,
 void * *ppvObject);
 void __stdcall IUnknown_QueryInterface_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 ULONG __stdcall IUnknown_AddRef_Proxy(
 IUnknown * This);
 void __stdcall IUnknown_AddRef_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 ULONG __stdcall IUnknown_Release_Proxy(
 IUnknown * This);
 void __stdcall IUnknown_Release_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
extern RPC_IF_HANDLE __MIDL_itf_unknwn_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_unknwn_0000_0001_v0_0_s_ifspec;
extern "C" const IID IID_AsyncIUnknown;
 struct __declspec(uuid("000e0000-0000-0000-C000-000000000046")) __declspec(novtable)
 AsyncIUnknown : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Begin_QueryInterface(
 const IID & riid) = 0;
 virtual HRESULT __stdcall Finish_QueryInterface(
 void **ppvObject) = 0;
 virtual HRESULT __stdcall Begin_AddRef( void) = 0;
 virtual ULONG __stdcall Finish_AddRef( void) = 0;
 virtual HRESULT __stdcall Begin_Release( void) = 0;
 virtual ULONG __stdcall Finish_Release( void) = 0;
 };
typedef IClassFactory *LPCLASSFACTORY;
extern "C" const IID IID_IClassFactory;
 struct __declspec(uuid("00000001-0000-0000-C000-000000000046")) __declspec(novtable)
 IClassFactory : public IUnknown
 {
 public:
 virtual HRESULT __stdcall CreateInstance(
 IUnknown *pUnkOuter,
 const IID & riid,
 void **ppvObject) = 0;
 virtual HRESULT __stdcall LockServer(
 BOOL fLock) = 0;
 };
 HRESULT __stdcall IClassFactory_RemoteCreateInstance_Proxy(
 IClassFactory * This,
 const IID & riid,
 IUnknown **ppvObject);
void __stdcall IClassFactory_RemoteCreateInstance_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IClassFactory_RemoteLockServer_Proxy(
 IClassFactory * This,
 BOOL fLock);
void __stdcall IClassFactory_RemoteLockServer_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IClassFactory_CreateInstance_Proxy(
 IClassFactory * This,
 IUnknown *pUnkOuter,
 const IID & riid,
 void **ppvObject);
 HRESULT __stdcall IClassFactory_CreateInstance_Stub(
 IClassFactory * This,
 const IID & riid,
 IUnknown **ppvObject);
 HRESULT __stdcall IClassFactory_LockServer_Proxy(
 IClassFactory * This,
 BOOL fLock);
 HRESULT __stdcall IClassFactory_LockServer_Stub(
 IClassFactory * This,
 BOOL fLock);
}
#pragma warning( disable: 4049 )
#pragma once
typedef struct IMarshal IMarshal;
typedef struct IMarshal2 IMarshal2;
typedef struct IMalloc IMalloc;
typedef struct IMallocSpy IMallocSpy;
typedef struct IStdMarshalInfo IStdMarshalInfo;
typedef struct IExternalConnection IExternalConnection;
typedef struct IMultiQI IMultiQI;
typedef struct AsyncIMultiQI AsyncIMultiQI;
typedef struct IInternalUnknown IInternalUnknown;
typedef struct IEnumUnknown IEnumUnknown;
typedef struct IBindCtx IBindCtx;
typedef struct IEnumMoniker IEnumMoniker;
typedef struct IRunnableObject IRunnableObject;
typedef struct IRunningObjectTable IRunningObjectTable;
typedef struct IPersist IPersist;
typedef struct IPersistStream IPersistStream;
typedef struct IMoniker IMoniker;
typedef struct IROTData IROTData;
typedef struct IEnumString IEnumString;
typedef struct ISequentialStream ISequentialStream;
typedef struct IStream IStream;
typedef struct IEnumSTATSTG IEnumSTATSTG;
typedef struct IStorage IStorage;
typedef struct IPersistFile IPersistFile;
typedef struct IPersistStorage IPersistStorage;
typedef struct ILockBytes ILockBytes;
typedef struct IEnumFORMATETC IEnumFORMATETC;
typedef struct IEnumSTATDATA IEnumSTATDATA;
typedef struct IRootStorage IRootStorage;
typedef struct IAdviseSink IAdviseSink;
typedef struct AsyncIAdviseSink AsyncIAdviseSink;
typedef struct IAdviseSink2 IAdviseSink2;
typedef struct AsyncIAdviseSink2 AsyncIAdviseSink2;
typedef struct IDataObject IDataObject;
typedef struct IDataAdviseHolder IDataAdviseHolder;
typedef struct IMessageFilter IMessageFilter;
typedef struct IRpcChannelBuffer IRpcChannelBuffer;
typedef struct IRpcChannelBuffer2 IRpcChannelBuffer2;
typedef struct IAsyncRpcChannelBuffer IAsyncRpcChannelBuffer;
typedef struct IRpcChannelBuffer3 IRpcChannelBuffer3;
typedef struct IRpcSyntaxNegotiate IRpcSyntaxNegotiate;
typedef struct IRpcProxyBuffer IRpcProxyBuffer;
typedef struct IRpcStubBuffer IRpcStubBuffer;
typedef struct IPSFactoryBuffer IPSFactoryBuffer;
typedef struct IChannelHook IChannelHook;
typedef struct IClientSecurity IClientSecurity;
typedef struct IServerSecurity IServerSecurity;
typedef struct IClassActivator IClassActivator;
typedef struct IRpcOptions IRpcOptions;
typedef struct IFillLockBytes IFillLockBytes;
typedef struct IProgressNotify IProgressNotify;
typedef struct ILayoutStorage ILayoutStorage;
typedef struct IBlockingLock IBlockingLock;
typedef struct ITimeAndNoticeControl ITimeAndNoticeControl;
typedef struct IOplockStorage IOplockStorage;
typedef struct ISurrogate ISurrogate;
typedef struct IGlobalInterfaceTable IGlobalInterfaceTable;
typedef struct IDirectWriterLock IDirectWriterLock;
typedef struct ISynchronize ISynchronize;
typedef struct ISynchronizeHandle ISynchronizeHandle;
typedef struct ISynchronizeEvent ISynchronizeEvent;
typedef struct ISynchronizeContainer ISynchronizeContainer;
typedef struct ISynchronizeMutex ISynchronizeMutex;
typedef struct ICancelMethodCalls ICancelMethodCalls;
typedef struct IAsyncManager IAsyncManager;
typedef struct ICallFactory ICallFactory;
typedef struct IRpcHelper IRpcHelper;
typedef struct IReleaseMarshalBuffers IReleaseMarshalBuffers;
typedef struct IWaitMultiple IWaitMultiple;
typedef struct IUrlMon IUrlMon;
typedef struct IForegroundTransfer IForegroundTransfer;
typedef struct IAddrTrackingControl IAddrTrackingControl;
typedef struct IAddrExclusionControl IAddrExclusionControl;
typedef struct IPipeByte IPipeByte;
typedef struct AsyncIPipeByte AsyncIPipeByte;
typedef struct IPipeLong IPipeLong;
typedef struct AsyncIPipeLong AsyncIPipeLong;
typedef struct IPipeDouble IPipeDouble;
typedef struct AsyncIPipeDouble AsyncIPipeDouble;
typedef struct IThumbnailExtractor IThumbnailExtractor;
typedef struct IDummyHICONIncluder IDummyHICONIncluder;
typedef struct IEnumContextProps IEnumContextProps;
typedef struct IContext IContext;
typedef struct IObjContext IObjContext;
typedef struct IProcessLock IProcessLock;
typedef struct ISurrogateService ISurrogateService;
typedef struct IComThreadingInfo IComThreadingInfo;
typedef struct IProcessInitControl IProcessInitControl;
typedef struct IInitializeSpy IInitializeSpy;
extern "C"{
#pragma warning(push)
#pragma warning(disable:4201)
#pragma once
typedef struct _COSERVERINFO
 {
 DWORD dwReserved1;
 LPWSTR pwszName;
 COAUTHINFO *pAuthInfo;
 DWORD dwReserved2;
 } COSERVERINFO;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0000_v0_0_s_ifspec;
typedef IMarshal *LPMARSHAL;
extern "C" const IID IID_IMarshal;
 struct __declspec(uuid("00000003-0000-0000-C000-000000000046")) __declspec(novtable)
 IMarshal : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetUnmarshalClass(
 const IID & riid,
 void *pv,
 DWORD dwDestContext,
 void *pvDestContext,
 DWORD mshlflags,
 CLSID *pCid) = 0;
 virtual HRESULT __stdcall GetMarshalSizeMax(
 const IID & riid,
 void *pv,
 DWORD dwDestContext,
 void *pvDestContext,
 DWORD mshlflags,
 DWORD *pSize) = 0;
 virtual HRESULT __stdcall MarshalInterface(
 IStream *pStm,
 const IID & riid,
 void *pv,
 DWORD dwDestContext,
 void *pvDestContext,
 DWORD mshlflags) = 0;
 virtual HRESULT __stdcall UnmarshalInterface(
 IStream *pStm,
 const IID & riid,
 void **ppv) = 0;
 virtual HRESULT __stdcall ReleaseMarshalData(
 IStream *pStm) = 0;
 virtual HRESULT __stdcall DisconnectObject(
 DWORD dwReserved) = 0;
 };
typedef IMarshal2 *LPMARSHAL2;
extern "C" const IID IID_IMarshal2;
 struct __declspec(uuid("000001cf-0000-0000-C000-000000000046")) __declspec(novtable)
 IMarshal2 : public IMarshal
 {
 public:
 };
typedef IMalloc *LPMALLOC;
extern "C" const IID IID_IMalloc;
 struct __declspec(uuid("00000002-0000-0000-C000-000000000046")) __declspec(novtable)
 IMalloc : public IUnknown
 {
 public:
 virtual void *__stdcall Alloc(
 SIZE_T cb) = 0;
 virtual void *__stdcall Realloc(
 void *pv,
 SIZE_T cb) = 0;
 virtual void __stdcall Free(
 void *pv) = 0;
 virtual SIZE_T __stdcall GetSize(
 void *pv) = 0;
 virtual int __stdcall DidAlloc(
 void *pv) = 0;
 virtual void __stdcall HeapMinimize( void) = 0;
 };
typedef IMallocSpy *LPMALLOCSPY;
extern "C" const IID IID_IMallocSpy;
 struct __declspec(uuid("0000001d-0000-0000-C000-000000000046")) __declspec(novtable)
 IMallocSpy : public IUnknown
 {
 public:
 virtual SIZE_T __stdcall PreAlloc(
 SIZE_T cbRequest) = 0;
 virtual void *__stdcall PostAlloc(
 void *pActual) = 0;
 virtual void *__stdcall PreFree(
 void *pRequest,
 BOOL fSpyed) = 0;
 virtual void __stdcall PostFree(
 BOOL fSpyed) = 0;
 virtual SIZE_T __stdcall PreRealloc(
 void *pRequest,
 SIZE_T cbRequest,
 void **ppNewRequest,
 BOOL fSpyed) = 0;
 virtual void *__stdcall PostRealloc(
 void *pActual,
 BOOL fSpyed) = 0;
 virtual void *__stdcall PreGetSize(
 void *pRequest,
 BOOL fSpyed) = 0;
 virtual SIZE_T __stdcall PostGetSize(
 SIZE_T cbActual,
 BOOL fSpyed) = 0;
 virtual void *__stdcall PreDidAlloc(
 void *pRequest,
 BOOL fSpyed) = 0;
 virtual int __stdcall PostDidAlloc(
 void *pRequest,
 BOOL fSpyed,
 int fActual) = 0;
 virtual void __stdcall PreHeapMinimize( void) = 0;
 virtual void __stdcall PostHeapMinimize( void) = 0;
 };
typedef IStdMarshalInfo *LPSTDMARSHALINFO;
extern "C" const IID IID_IStdMarshalInfo;
 struct __declspec(uuid("00000018-0000-0000-C000-000000000046")) __declspec(novtable)
 IStdMarshalInfo : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetClassForHandler(
 DWORD dwDestContext,
 void *pvDestContext,
 CLSID *pClsid) = 0;
 };
typedef IExternalConnection *LPEXTERNALCONNECTION;
typedef
enum tagEXTCONN
 { EXTCONN_STRONG = 0x1,
 EXTCONN_WEAK = 0x2,
 EXTCONN_CALLABLE = 0x4
 } EXTCONN;
extern "C" const IID IID_IExternalConnection;
 struct __declspec(uuid("00000019-0000-0000-C000-000000000046")) __declspec(novtable)
 IExternalConnection : public IUnknown
 {
 public:
 virtual DWORD __stdcall AddConnection(
 DWORD extconn,
 DWORD reserved) = 0;
 virtual DWORD __stdcall ReleaseConnection(
 DWORD extconn,
 DWORD reserved,
 BOOL fLastReleaseCloses) = 0;
 };
typedef IMultiQI *LPMULTIQI;
typedef struct tagMULTI_QI
 {
 const IID *pIID;
 IUnknown *pItf;
 HRESULT hr;
 } MULTI_QI;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0006_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0006_v0_0_s_ifspec;
extern "C" const IID IID_IMultiQI;
 struct __declspec(uuid("00000020-0000-0000-C000-000000000046")) __declspec(novtable)
 IMultiQI : public IUnknown
 {
 public:
 virtual HRESULT __stdcall QueryMultipleInterfaces(
 ULONG cMQIs,
 MULTI_QI *pMQIs) = 0;
 };
extern "C" const IID IID_AsyncIMultiQI;
 struct __declspec(uuid("000e0020-0000-0000-C000-000000000046")) __declspec(novtable)
 AsyncIMultiQI : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Begin_QueryMultipleInterfaces(
 ULONG cMQIs,
 MULTI_QI *pMQIs) = 0;
 virtual HRESULT __stdcall Finish_QueryMultipleInterfaces(
 MULTI_QI *pMQIs) = 0;
 };
extern "C" const IID IID_IInternalUnknown;
 struct __declspec(uuid("00000021-0000-0000-C000-000000000046")) __declspec(novtable)
 IInternalUnknown : public IUnknown
 {
 public:
 virtual HRESULT __stdcall QueryInternalInterface(
 const IID & riid,
 void **ppv) = 0;
 };
typedef IEnumUnknown *LPENUMUNKNOWN;
extern "C" const IID IID_IEnumUnknown;
 struct __declspec(uuid("00000100-0000-0000-C000-000000000046")) __declspec(novtable)
 IEnumUnknown : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Next(
 ULONG celt,
 IUnknown **rgelt,
 ULONG *pceltFetched) = 0;
 virtual HRESULT __stdcall Skip(
 ULONG celt) = 0;
 virtual HRESULT __stdcall Reset( void) = 0;
 virtual HRESULT __stdcall Clone(
 IEnumUnknown **ppenum) = 0;
 };
 HRESULT __stdcall IEnumUnknown_RemoteNext_Proxy(
 IEnumUnknown * This,
 ULONG celt,
 IUnknown **rgelt,
 ULONG *pceltFetched);
void __stdcall IEnumUnknown_RemoteNext_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IBindCtx *LPBC;
typedef IBindCtx *LPBINDCTX;
 typedef struct tagBIND_OPTS {
 DWORD cbStruct;
 DWORD grfFlags;
 DWORD grfMode;
 DWORD dwTickCountDeadline;
 } BIND_OPTS, * LPBIND_OPTS;
 typedef struct tagBIND_OPTS2 : tagBIND_OPTS {
 DWORD dwTrackFlags;
 DWORD dwClassContext;
 LCID locale;
 COSERVERINFO * pServerInfo;
 } BIND_OPTS2, * LPBIND_OPTS2;
 typedef struct tagBIND_OPTS3 : tagBIND_OPTS2 {
 HWND hwnd;
 } BIND_OPTS3, * LPBIND_OPTS3;
typedef
enum tagBIND_FLAGS
 { BIND_MAYBOTHERUSER = 1,
 BIND_JUSTTESTEXISTENCE = 2
 } BIND_FLAGS;
extern "C" const IID IID_IBindCtx;
 struct __declspec(uuid("0000000e-0000-0000-C000-000000000046")) __declspec(novtable)
 IBindCtx : public IUnknown
 {
 public:
 virtual HRESULT __stdcall RegisterObjectBound(
 IUnknown *punk) = 0;
 virtual HRESULT __stdcall RevokeObjectBound(
 IUnknown *punk) = 0;
 virtual HRESULT __stdcall ReleaseBoundObjects( void) = 0;
 virtual HRESULT __stdcall SetBindOptions(
 BIND_OPTS *pbindopts) = 0;
 virtual HRESULT __stdcall GetBindOptions(
 BIND_OPTS *pbindopts) = 0;
 virtual HRESULT __stdcall GetRunningObjectTable(
 IRunningObjectTable **pprot) = 0;
 virtual HRESULT __stdcall RegisterObjectParam(
 LPOLESTR pszKey,
 IUnknown *punk) = 0;
 virtual HRESULT __stdcall GetObjectParam(
 LPOLESTR pszKey,
 IUnknown **ppunk) = 0;
 virtual HRESULT __stdcall EnumObjectParam(
 IEnumString **ppenum) = 0;
 virtual HRESULT __stdcall RevokeObjectParam(
 LPOLESTR pszKey) = 0;
 };
 HRESULT __stdcall IBindCtx_RemoteSetBindOptions_Proxy(
 IBindCtx * This,
 BIND_OPTS2 *pbindopts);
void __stdcall IBindCtx_RemoteSetBindOptions_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IBindCtx_RemoteGetBindOptions_Proxy(
 IBindCtx * This,
 BIND_OPTS2 *pbindopts);
void __stdcall IBindCtx_RemoteGetBindOptions_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IEnumMoniker *LPENUMMONIKER;
extern "C" const IID IID_IEnumMoniker;
 struct __declspec(uuid("00000102-0000-0000-C000-000000000046")) __declspec(novtable)
 IEnumMoniker : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Next(
 ULONG celt,
 IMoniker **rgelt,
 ULONG *pceltFetched) = 0;
 virtual HRESULT __stdcall Skip(
 ULONG celt) = 0;
 virtual HRESULT __stdcall Reset( void) = 0;
 virtual HRESULT __stdcall Clone(
 IEnumMoniker **ppenum) = 0;
 };
 HRESULT __stdcall IEnumMoniker_RemoteNext_Proxy(
 IEnumMoniker * This,
 ULONG celt,
 IMoniker **rgelt,
 ULONG *pceltFetched);
void __stdcall IEnumMoniker_RemoteNext_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IRunnableObject *LPRUNNABLEOBJECT;
extern "C" const IID IID_IRunnableObject;
 struct __declspec(uuid("00000126-0000-0000-C000-000000000046")) __declspec(novtable)
 IRunnableObject : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetRunningClass(
 LPCLSID lpClsid) = 0;
 virtual HRESULT __stdcall Run(
 LPBINDCTX pbc) = 0;
 virtual BOOL __stdcall IsRunning( void) = 0;
 virtual HRESULT __stdcall LockRunning(
 BOOL fLock,
 BOOL fLastUnlockCloses) = 0;
 virtual HRESULT __stdcall SetContainedObject(
 BOOL fContained) = 0;
 };
 HRESULT __stdcall IRunnableObject_RemoteIsRunning_Proxy(
 IRunnableObject * This);
void __stdcall IRunnableObject_RemoteIsRunning_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IRunningObjectTable *LPRUNNINGOBJECTTABLE;
extern "C" const IID IID_IRunningObjectTable;
 struct __declspec(uuid("00000010-0000-0000-C000-000000000046")) __declspec(novtable)
 IRunningObjectTable : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Register(
 DWORD grfFlags,
 IUnknown *punkObject,
 IMoniker *pmkObjectName,
 DWORD *pdwRegister) = 0;
 virtual HRESULT __stdcall Revoke(
 DWORD dwRegister) = 0;
 virtual HRESULT __stdcall IsRunning(
 IMoniker *pmkObjectName) = 0;
 virtual HRESULT __stdcall GetObjectA(
 IMoniker *pmkObjectName,
 IUnknown **ppunkObject) = 0;
 virtual HRESULT __stdcall NoteChangeTime(
 DWORD dwRegister,
 FILETIME *pfiletime) = 0;
 virtual HRESULT __stdcall GetTimeOfLastChange(
 IMoniker *pmkObjectName,
 FILETIME *pfiletime) = 0;
 virtual HRESULT __stdcall EnumRunning(
 IEnumMoniker **ppenumMoniker) = 0;
 };
typedef IPersist *LPPERSIST;
extern "C" const IID IID_IPersist;
 struct __declspec(uuid("0000010c-0000-0000-C000-000000000046")) __declspec(novtable)
 IPersist : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetClassID(
 CLSID *pClassID) = 0;
 };
typedef IPersistStream *LPPERSISTSTREAM;
extern "C" const IID IID_IPersistStream;
 struct __declspec(uuid("00000109-0000-0000-C000-000000000046")) __declspec(novtable)
 IPersistStream : public IPersist
 {
 public:
 virtual HRESULT __stdcall IsDirty( void) = 0;
 virtual HRESULT __stdcall Load(
 IStream *pStm) = 0;
 virtual HRESULT __stdcall Save(
 IStream *pStm,
 BOOL fClearDirty) = 0;
 virtual HRESULT __stdcall GetSizeMax(
 ULARGE_INTEGER *pcbSize) = 0;
 };
typedef IMoniker *LPMONIKER;
typedef
enum tagMKSYS
 { MKSYS_NONE = 0,
 MKSYS_GENERICCOMPOSITE = 1,
 MKSYS_FILEMONIKER = 2,
 MKSYS_ANTIMONIKER = 3,
 MKSYS_ITEMMONIKER = 4,
 MKSYS_POINTERMONIKER = 5,
 MKSYS_CLASSMONIKER = 7,
 MKSYS_OBJREFMONIKER = 8,
 MKSYS_SESSIONMONIKER = 9,
 MKSYS_LUAMONIKER = 10
 } MKSYS;
typedef
enum tagMKREDUCE
 { MKRREDUCE_ONE = ( 3 << 16 ) ,
 MKRREDUCE_TOUSER = ( 2 << 16 ) ,
 MKRREDUCE_THROUGHUSER = ( 1 << 16 ) ,
 MKRREDUCE_ALL = 0
 } MKRREDUCE;
extern "C" const IID IID_IMoniker;
 struct __declspec(uuid("0000000f-0000-0000-C000-000000000046")) __declspec(novtable)
 IMoniker : public IPersistStream
 {
 public:
 virtual HRESULT __stdcall BindToObject(
 IBindCtx *pbc,
 IMoniker *pmkToLeft,
 const IID & riidResult,
 void **ppvResult) = 0;
 virtual HRESULT __stdcall BindToStorage(
 IBindCtx *pbc,
 IMoniker *pmkToLeft,
 const IID & riid,
 void **ppvObj) = 0;
 virtual HRESULT __stdcall Reduce(
 IBindCtx *pbc,
 DWORD dwReduceHowFar,
 IMoniker **ppmkToLeft,
 IMoniker **ppmkReduced) = 0;
 virtual HRESULT __stdcall ComposeWith(
 IMoniker *pmkRight,
 BOOL fOnlyIfNotGeneric,
 IMoniker **ppmkComposite) = 0;
 virtual HRESULT __stdcall Enum(
 BOOL fForward,
 IEnumMoniker **ppenumMoniker) = 0;
 virtual HRESULT __stdcall IsEqual(
 IMoniker *pmkOtherMoniker) = 0;
 virtual HRESULT __stdcall Hash(
 DWORD *pdwHash) = 0;
 virtual HRESULT __stdcall IsRunning(
 IBindCtx *pbc,
 IMoniker *pmkToLeft,
 IMoniker *pmkNewlyRunning) = 0;
 virtual HRESULT __stdcall GetTimeOfLastChange(
 IBindCtx *pbc,
 IMoniker *pmkToLeft,
 FILETIME *pFileTime) = 0;
 virtual HRESULT __stdcall Inverse(
 IMoniker **ppmk) = 0;
 virtual HRESULT __stdcall CommonPrefixWith(
 IMoniker *pmkOther,
 IMoniker **ppmkPrefix) = 0;
 virtual HRESULT __stdcall RelativePathTo(
 IMoniker *pmkOther,
 IMoniker **ppmkRelPath) = 0;
 virtual HRESULT __stdcall GetDisplayName(
 IBindCtx *pbc,
 IMoniker *pmkToLeft,
 LPOLESTR *ppszDisplayName) = 0;
 virtual HRESULT __stdcall ParseDisplayName(
 IBindCtx *pbc,
 IMoniker *pmkToLeft,
 LPOLESTR pszDisplayName,
 ULONG *pchEaten,
 IMoniker **ppmkOut) = 0;
 virtual HRESULT __stdcall IsSystemMoniker(
 DWORD *pdwMksys) = 0;
 };
 HRESULT __stdcall IMoniker_RemoteBindToObject_Proxy(
 IMoniker * This,
 IBindCtx *pbc,
 IMoniker *pmkToLeft,
 const IID & riidResult,
 IUnknown **ppvResult);
void __stdcall IMoniker_RemoteBindToObject_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IMoniker_RemoteBindToStorage_Proxy(
 IMoniker * This,
 IBindCtx *pbc,
 IMoniker *pmkToLeft,
 const IID & riid,
 IUnknown **ppvObj);
void __stdcall IMoniker_RemoteBindToStorage_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
extern "C" const IID IID_IROTData;
 struct __declspec(uuid("f29f6bc0-5021-11ce-aa15-00006901293f")) __declspec(novtable)
 IROTData : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetComparisonData(
 byte *pbData,
 ULONG cbMax,
 ULONG *pcbData) = 0;
 };
typedef IEnumString *LPENUMSTRING;
extern "C" const IID IID_IEnumString;
 struct __declspec(uuid("00000101-0000-0000-C000-000000000046")) __declspec(novtable)
 IEnumString : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Next(
 ULONG celt,
 LPOLESTR *rgelt,
 ULONG *pceltFetched) = 0;
 virtual HRESULT __stdcall Skip(
 ULONG celt) = 0;
 virtual HRESULT __stdcall Reset( void) = 0;
 virtual HRESULT __stdcall Clone(
 IEnumString **ppenum) = 0;
 };
 HRESULT __stdcall IEnumString_RemoteNext_Proxy(
 IEnumString * This,
 ULONG celt,
 LPOLESTR *rgelt,
 ULONG *pceltFetched);
void __stdcall IEnumString_RemoteNext_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
extern "C" const IID IID_ISequentialStream;
 struct __declspec(uuid("0c733a30-2a1c-11ce-ade5-00aa0044773d")) __declspec(novtable)
 ISequentialStream : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Read(
 void *pv,
 ULONG cb,
 ULONG *pcbRead) = 0;
 virtual HRESULT __stdcall Write(
 const void *pv,
 ULONG cb,
 ULONG *pcbWritten) = 0;
 };
 HRESULT __stdcall ISequentialStream_RemoteRead_Proxy(
 ISequentialStream * This,
 byte *pv,
 ULONG cb,
 ULONG *pcbRead);
void __stdcall ISequentialStream_RemoteRead_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ISequentialStream_RemoteWrite_Proxy(
 ISequentialStream * This,
 const byte *pv,
 ULONG cb,
 ULONG *pcbWritten);
void __stdcall ISequentialStream_RemoteWrite_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IStream *LPSTREAM;
typedef struct tagSTATSTG
 {
 LPOLESTR pwcsName;
 DWORD type;
 ULARGE_INTEGER cbSize;
 FILETIME mtime;
 FILETIME ctime;
 FILETIME atime;
 DWORD grfMode;
 DWORD grfLocksSupported;
 CLSID clsid;
 DWORD grfStateBits;
 DWORD reserved;
 } STATSTG;
typedef
enum tagSTGTY
 { STGTY_STORAGE = 1,
 STGTY_STREAM = 2,
 STGTY_LOCKBYTES = 3,
 STGTY_PROPERTY = 4
 } STGTY;
typedef
enum tagSTREAM_SEEK
 { STREAM_SEEK_SET = 0,
 STREAM_SEEK_CUR = 1,
 STREAM_SEEK_END = 2
 } STREAM_SEEK;
typedef
enum tagLOCKTYPE
 { LOCK_WRITE = 1,
 LOCK_EXCLUSIVE = 2,
 LOCK_ONLYONCE = 4
 } LOCKTYPE;
extern "C" const IID IID_IStream;
 struct __declspec(uuid("0000000c-0000-0000-C000-000000000046")) __declspec(novtable)
 IStream : public ISequentialStream
 {
 public:
 virtual HRESULT __stdcall Seek(
 LARGE_INTEGER dlibMove,
 DWORD dwOrigin,
 ULARGE_INTEGER *plibNewPosition) = 0;
 virtual HRESULT __stdcall SetSize(
 ULARGE_INTEGER libNewSize) = 0;
 virtual HRESULT __stdcall CopyTo(
 IStream *pstm,
 ULARGE_INTEGER cb,
 ULARGE_INTEGER *pcbRead,
 ULARGE_INTEGER *pcbWritten) = 0;
 virtual HRESULT __stdcall Commit(
 DWORD grfCommitFlags) = 0;
 virtual HRESULT __stdcall Revert( void) = 0;
 virtual HRESULT __stdcall LockRegion(
 ULARGE_INTEGER libOffset,
 ULARGE_INTEGER cb,
 DWORD dwLockType) = 0;
 virtual HRESULT __stdcall UnlockRegion(
 ULARGE_INTEGER libOffset,
 ULARGE_INTEGER cb,
 DWORD dwLockType) = 0;
 virtual HRESULT __stdcall Stat(
 STATSTG *pstatstg,
 DWORD grfStatFlag) = 0;
 virtual HRESULT __stdcall Clone(
 IStream **ppstm) = 0;
 };
 HRESULT __stdcall IStream_RemoteSeek_Proxy(
 IStream * This,
 LARGE_INTEGER dlibMove,
 DWORD dwOrigin,
 ULARGE_INTEGER *plibNewPosition);
void __stdcall IStream_RemoteSeek_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IStream_RemoteCopyTo_Proxy(
 IStream * This,
 IStream *pstm,
 ULARGE_INTEGER cb,
 ULARGE_INTEGER *pcbRead,
 ULARGE_INTEGER *pcbWritten);
void __stdcall IStream_RemoteCopyTo_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IEnumSTATSTG *LPENUMSTATSTG;
extern "C" const IID IID_IEnumSTATSTG;
 struct __declspec(uuid("0000000d-0000-0000-C000-000000000046")) __declspec(novtable)
 IEnumSTATSTG : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Next(
 ULONG celt,
 STATSTG *rgelt,
 ULONG *pceltFetched) = 0;
 virtual HRESULT __stdcall Skip(
 ULONG celt) = 0;
 virtual HRESULT __stdcall Reset( void) = 0;
 virtual HRESULT __stdcall Clone(
 IEnumSTATSTG **ppenum) = 0;
 };
 HRESULT __stdcall IEnumSTATSTG_RemoteNext_Proxy(
 IEnumSTATSTG * This,
 ULONG celt,
 STATSTG *rgelt,
 ULONG *pceltFetched);
void __stdcall IEnumSTATSTG_RemoteNext_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IStorage *LPSTORAGE;
typedef struct tagRemSNB
 {
 unsigned long ulCntStr;
 unsigned long ulCntChar;
 OLECHAR rgString[ 1 ];
 } RemSNB;
typedef RemSNB *wireSNB;
typedef OLECHAR **SNB;
extern "C" const IID IID_IStorage;
 struct __declspec(uuid("0000000b-0000-0000-C000-000000000046")) __declspec(novtable)
 IStorage : public IUnknown
 {
 public:
 virtual HRESULT __stdcall CreateStream(
 const OLECHAR *pwcsName,
 DWORD grfMode,
 DWORD reserved1,
 DWORD reserved2,
 IStream **ppstm) = 0;
 virtual HRESULT __stdcall OpenStream(
 const OLECHAR *pwcsName,
 void *reserved1,
 DWORD grfMode,
 DWORD reserved2,
 IStream **ppstm) = 0;
 virtual HRESULT __stdcall CreateStorage(
 const OLECHAR *pwcsName,
 DWORD grfMode,
 DWORD reserved1,
 DWORD reserved2,
 IStorage **ppstg) = 0;
 virtual HRESULT __stdcall OpenStorage(
 const OLECHAR *pwcsName,
 IStorage *pstgPriority,
 DWORD grfMode,
 SNB snbExclude,
 DWORD reserved,
 IStorage **ppstg) = 0;
 virtual HRESULT __stdcall CopyTo(
 DWORD ciidExclude,
 const IID *rgiidExclude,
 SNB snbExclude,
 IStorage *pstgDest) = 0;
 virtual HRESULT __stdcall MoveElementTo(
 const OLECHAR *pwcsName,
 IStorage *pstgDest,
 const OLECHAR *pwcsNewName,
 DWORD grfFlags) = 0;
 virtual HRESULT __stdcall Commit(
 DWORD grfCommitFlags) = 0;
 virtual HRESULT __stdcall Revert( void) = 0;
 virtual HRESULT __stdcall EnumElements(
 DWORD reserved1,
 void *reserved2,
 DWORD reserved3,
 IEnumSTATSTG **ppenum) = 0;
 virtual HRESULT __stdcall DestroyElement(
 const OLECHAR *pwcsName) = 0;
 virtual HRESULT __stdcall RenameElement(
 const OLECHAR *pwcsOldName,
 const OLECHAR *pwcsNewName) = 0;
 virtual HRESULT __stdcall SetElementTimes(
 const OLECHAR *pwcsName,
 const FILETIME *pctime,
 const FILETIME *patime,
 const FILETIME *pmtime) = 0;
 virtual HRESULT __stdcall SetClass(
 const IID & clsid) = 0;
 virtual HRESULT __stdcall SetStateBits(
 DWORD grfStateBits,
 DWORD grfMask) = 0;
 virtual HRESULT __stdcall Stat(
 STATSTG *pstatstg,
 DWORD grfStatFlag) = 0;
 };
 HRESULT __stdcall IStorage_RemoteOpenStream_Proxy(
 IStorage * This,
 const OLECHAR *pwcsName,
 unsigned long cbReserved1,
 byte *reserved1,
 DWORD grfMode,
 DWORD reserved2,
 IStream **ppstm);
void __stdcall IStorage_RemoteOpenStream_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IStorage_RemoteCopyTo_Proxy(
 IStorage * This,
 DWORD ciidExclude,
 const IID *rgiidExclude,
 SNB snbExclude,
 IStorage *pstgDest);
void __stdcall IStorage_RemoteCopyTo_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IStorage_RemoteEnumElements_Proxy(
 IStorage * This,
 DWORD reserved1,
 unsigned long cbReserved2,
 byte *reserved2,
 DWORD reserved3,
 IEnumSTATSTG **ppenum);
void __stdcall IStorage_RemoteEnumElements_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IPersistFile *LPPERSISTFILE;
extern "C" const IID IID_IPersistFile;
 struct __declspec(uuid("0000010b-0000-0000-C000-000000000046")) __declspec(novtable)
 IPersistFile : public IPersist
 {
 public:
 virtual HRESULT __stdcall IsDirty( void) = 0;
 virtual HRESULT __stdcall Load(
 LPCOLESTR pszFileName,
 DWORD dwMode) = 0;
 virtual HRESULT __stdcall Save(
 LPCOLESTR pszFileName,
 BOOL fRemember) = 0;
 virtual HRESULT __stdcall SaveCompleted(
 LPCOLESTR pszFileName) = 0;
 virtual HRESULT __stdcall GetCurFile(
 LPOLESTR *ppszFileName) = 0;
 };
typedef IPersistStorage *LPPERSISTSTORAGE;
extern "C" const IID IID_IPersistStorage;
 struct __declspec(uuid("0000010a-0000-0000-C000-000000000046")) __declspec(novtable)
 IPersistStorage : public IPersist
 {
 public:
 virtual HRESULT __stdcall IsDirty( void) = 0;
 virtual HRESULT __stdcall InitNew(
 IStorage *pStg) = 0;
 virtual HRESULT __stdcall Load(
 IStorage *pStg) = 0;
 virtual HRESULT __stdcall Save(
 IStorage *pStgSave,
 BOOL fSameAsLoad) = 0;
 virtual HRESULT __stdcall SaveCompleted(
 IStorage *pStgNew) = 0;
 virtual HRESULT __stdcall HandsOffStorage( void) = 0;
 };
typedef ILockBytes *LPLOCKBYTES;
extern "C" const IID IID_ILockBytes;
 struct __declspec(uuid("0000000a-0000-0000-C000-000000000046")) __declspec(novtable)
 ILockBytes : public IUnknown
 {
 public:
 virtual HRESULT __stdcall ReadAt(
 ULARGE_INTEGER ulOffset,
 void *pv,
 ULONG cb,
 ULONG *pcbRead) = 0;
 virtual HRESULT __stdcall WriteAt(
 ULARGE_INTEGER ulOffset,
 const void *pv,
 ULONG cb,
 ULONG *pcbWritten) = 0;
 virtual HRESULT __stdcall Flush( void) = 0;
 virtual HRESULT __stdcall SetSize(
 ULARGE_INTEGER cb) = 0;
 virtual HRESULT __stdcall LockRegion(
 ULARGE_INTEGER libOffset,
 ULARGE_INTEGER cb,
 DWORD dwLockType) = 0;
 virtual HRESULT __stdcall UnlockRegion(
 ULARGE_INTEGER libOffset,
 ULARGE_INTEGER cb,
 DWORD dwLockType) = 0;
 virtual HRESULT __stdcall Stat(
 STATSTG *pstatstg,
 DWORD grfStatFlag) = 0;
 };
 HRESULT __stdcall ILockBytes_RemoteReadAt_Proxy(
 ILockBytes * This,
 ULARGE_INTEGER ulOffset,
 byte *pv,
 ULONG cb,
 ULONG *pcbRead);
void __stdcall ILockBytes_RemoteReadAt_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ILockBytes_RemoteWriteAt_Proxy(
 ILockBytes * This,
 ULARGE_INTEGER ulOffset,
 const byte *pv,
 ULONG cb,
 ULONG *pcbWritten);
void __stdcall ILockBytes_RemoteWriteAt_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IEnumFORMATETC *LPENUMFORMATETC;
typedef struct tagDVTARGETDEVICE
 {
 DWORD tdSize;
 WORD tdDriverNameOffset;
 WORD tdDeviceNameOffset;
 WORD tdPortNameOffset;
 WORD tdExtDevmodeOffset;
 BYTE tdData[ 1 ];
 } DVTARGETDEVICE;
typedef CLIPFORMAT *LPCLIPFORMAT;
typedef struct tagFORMATETC
 {
 CLIPFORMAT cfFormat;
 DVTARGETDEVICE *ptd;
 DWORD dwAspect;
 LONG lindex;
 DWORD tymed;
 } FORMATETC;
typedef struct tagFORMATETC *LPFORMATETC;
extern "C" const IID IID_IEnumFORMATETC;
 struct __declspec(uuid("00000103-0000-0000-C000-000000000046")) __declspec(novtable)
 IEnumFORMATETC : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Next(
 ULONG celt,
 FORMATETC *rgelt,
 ULONG *pceltFetched) = 0;
 virtual HRESULT __stdcall Skip(
 ULONG celt) = 0;
 virtual HRESULT __stdcall Reset( void) = 0;
 virtual HRESULT __stdcall Clone(
 IEnumFORMATETC **ppenum) = 0;
 };
 HRESULT __stdcall IEnumFORMATETC_RemoteNext_Proxy(
 IEnumFORMATETC * This,
 ULONG celt,
 FORMATETC *rgelt,
 ULONG *pceltFetched);
void __stdcall IEnumFORMATETC_RemoteNext_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IEnumSTATDATA *LPENUMSTATDATA;
typedef
enum tagADVF
 { ADVF_NODATA = 1,
 ADVF_PRIMEFIRST = 2,
 ADVF_ONLYONCE = 4,
 ADVF_DATAONSTOP = 64,
 ADVFCACHE_NOHANDLER = 8,
 ADVFCACHE_FORCEBUILTIN = 16,
 ADVFCACHE_ONSAVE = 32
 } ADVF;
typedef struct tagSTATDATA
 {
 FORMATETC formatetc;
 DWORD advf;
 IAdviseSink *pAdvSink;
 DWORD dwConnection;
 } STATDATA;
typedef STATDATA *LPSTATDATA;
extern "C" const IID IID_IEnumSTATDATA;
 struct __declspec(uuid("00000105-0000-0000-C000-000000000046")) __declspec(novtable)
 IEnumSTATDATA : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Next(
 ULONG celt,
 STATDATA *rgelt,
 ULONG *pceltFetched) = 0;
 virtual HRESULT __stdcall Skip(
 ULONG celt) = 0;
 virtual HRESULT __stdcall Reset( void) = 0;
 virtual HRESULT __stdcall Clone(
 IEnumSTATDATA **ppenum) = 0;
 };
 HRESULT __stdcall IEnumSTATDATA_RemoteNext_Proxy(
 IEnumSTATDATA * This,
 ULONG celt,
 STATDATA *rgelt,
 ULONG *pceltFetched);
void __stdcall IEnumSTATDATA_RemoteNext_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IRootStorage *LPROOTSTORAGE;
extern "C" const IID IID_IRootStorage;
 struct __declspec(uuid("00000012-0000-0000-C000-000000000046")) __declspec(novtable)
 IRootStorage : public IUnknown
 {
 public:
 virtual HRESULT __stdcall SwitchToFile(
 LPOLESTR pszFile) = 0;
 };
typedef IAdviseSink *LPADVISESINK;
typedef
enum tagTYMED
 { TYMED_HGLOBAL = 1,
 TYMED_FILE = 2,
 TYMED_ISTREAM = 4,
 TYMED_ISTORAGE = 8,
 TYMED_GDI = 16,
 TYMED_MFPICT = 32,
 TYMED_ENHMF = 64,
 TYMED_NULL = 0
 } TYMED;
#pragma warning(push)
#pragma warning(disable:4200)
typedef struct tagRemSTGMEDIUM
 {
 DWORD tymed;
 DWORD dwHandleType;
 unsigned long pData;
 unsigned long pUnkForRelease;
 unsigned long cbData;
 byte data[ 1 ];
 } RemSTGMEDIUM;
#pragma warning(pop)
typedef struct tagSTGMEDIUM
 {
 DWORD tymed;
 union
 {
 HBITMAP hBitmap;
 HMETAFILEPICT hMetaFilePict;
 HENHMETAFILE hEnhMetaFile;
 HGLOBAL hGlobal;
 LPOLESTR lpszFileName;
 IStream *pstm;
 IStorage *pstg;
 } ;
 IUnknown *pUnkForRelease;
 } uSTGMEDIUM;
typedef struct _GDI_OBJECT
 {
 DWORD ObjectType;
 union __MIDL_IAdviseSink_0002
 {
 wireHBITMAP hBitmap;
 wireHPALETTE hPalette;
 wireHGLOBAL hGeneric;
 } u;
 } GDI_OBJECT;
typedef struct _userSTGMEDIUM
 {
 struct _STGMEDIUM_UNION
 {
 DWORD tymed;
 union __MIDL_IAdviseSink_0003
 {
 wireHMETAFILEPICT hMetaFilePict;
 wireHENHMETAFILE hHEnhMetaFile;
 GDI_OBJECT *hGdiHandle;
 wireHGLOBAL hGlobal;
 LPOLESTR lpszFileName;
 BYTE_BLOB *pstm;
 BYTE_BLOB *pstg;
 } u;
 } ;
 IUnknown *pUnkForRelease;
 } userSTGMEDIUM;
typedef userSTGMEDIUM *wireSTGMEDIUM;
typedef uSTGMEDIUM STGMEDIUM;
typedef userSTGMEDIUM *wireASYNC_STGMEDIUM;
typedef STGMEDIUM ASYNC_STGMEDIUM;
typedef STGMEDIUM *LPSTGMEDIUM;
typedef struct _userFLAG_STGMEDIUM
 {
 long ContextFlags;
 long fPassOwnership;
 userSTGMEDIUM Stgmed;
 } userFLAG_STGMEDIUM;
typedef userFLAG_STGMEDIUM *wireFLAG_STGMEDIUM;
typedef struct _FLAG_STGMEDIUM
 {
 long ContextFlags;
 long fPassOwnership;
 STGMEDIUM Stgmed;
 } FLAG_STGMEDIUM;
extern "C" const IID IID_IAdviseSink;
 struct __declspec(uuid("0000010f-0000-0000-C000-000000000046")) __declspec(novtable)
 IAdviseSink : public IUnknown
 {
 public:
 virtual void __stdcall OnDataChange(
 FORMATETC *pFormatetc,
 STGMEDIUM *pStgmed) = 0;
 virtual void __stdcall OnViewChange(
 DWORD dwAspect,
 LONG lindex) = 0;
 virtual void __stdcall OnRename(
 IMoniker *pmk) = 0;
 virtual void __stdcall OnSave( void) = 0;
 virtual void __stdcall OnClose( void) = 0;
 };
 HRESULT __stdcall IAdviseSink_RemoteOnDataChange_Proxy(
 IAdviseSink * This,
 FORMATETC *pFormatetc,
 ASYNC_STGMEDIUM *pStgmed);
void __stdcall IAdviseSink_RemoteOnDataChange_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IAdviseSink_RemoteOnViewChange_Proxy(
 IAdviseSink * This,
 DWORD dwAspect,
 LONG lindex);
void __stdcall IAdviseSink_RemoteOnViewChange_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IAdviseSink_RemoteOnRename_Proxy(
 IAdviseSink * This,
 IMoniker *pmk);
void __stdcall IAdviseSink_RemoteOnRename_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IAdviseSink_RemoteOnSave_Proxy(
 IAdviseSink * This);
void __stdcall IAdviseSink_RemoteOnSave_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IAdviseSink_RemoteOnClose_Proxy(
 IAdviseSink * This);
void __stdcall IAdviseSink_RemoteOnClose_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
extern "C" const IID IID_AsyncIAdviseSink;
 struct __declspec(uuid("00000150-0000-0000-C000-000000000046")) __declspec(novtable)
 AsyncIAdviseSink : public IUnknown
 {
 public:
 virtual void __stdcall Begin_OnDataChange(
 FORMATETC *pFormatetc,
 STGMEDIUM *pStgmed) = 0;
 virtual void __stdcall Finish_OnDataChange( void) = 0;
 virtual void __stdcall Begin_OnViewChange(
 DWORD dwAspect,
 LONG lindex) = 0;
 virtual void __stdcall Finish_OnViewChange( void) = 0;
 virtual void __stdcall Begin_OnRename(
 IMoniker *pmk) = 0;
 virtual void __stdcall Finish_OnRename( void) = 0;
 virtual void __stdcall Begin_OnSave( void) = 0;
 virtual void __stdcall Finish_OnSave( void) = 0;
 virtual void __stdcall Begin_OnClose( void) = 0;
 virtual void __stdcall Finish_OnClose( void) = 0;
 };
 HRESULT __stdcall AsyncIAdviseSink_Begin_RemoteOnDataChange_Proxy(
 AsyncIAdviseSink * This,
 FORMATETC *pFormatetc,
 ASYNC_STGMEDIUM *pStgmed);
void __stdcall AsyncIAdviseSink_Begin_RemoteOnDataChange_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall AsyncIAdviseSink_Finish_RemoteOnDataChange_Proxy(
 AsyncIAdviseSink * This);
void __stdcall AsyncIAdviseSink_Finish_RemoteOnDataChange_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall AsyncIAdviseSink_Begin_RemoteOnViewChange_Proxy(
 AsyncIAdviseSink * This,
 DWORD dwAspect,
 LONG lindex);
void __stdcall AsyncIAdviseSink_Begin_RemoteOnViewChange_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall AsyncIAdviseSink_Finish_RemoteOnViewChange_Proxy(
 AsyncIAdviseSink * This);
void __stdcall AsyncIAdviseSink_Finish_RemoteOnViewChange_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall AsyncIAdviseSink_Begin_RemoteOnRename_Proxy(
 AsyncIAdviseSink * This,
 IMoniker *pmk);
void __stdcall AsyncIAdviseSink_Begin_RemoteOnRename_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall AsyncIAdviseSink_Finish_RemoteOnRename_Proxy(
 AsyncIAdviseSink * This);
void __stdcall AsyncIAdviseSink_Finish_RemoteOnRename_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall AsyncIAdviseSink_Begin_RemoteOnSave_Proxy(
 AsyncIAdviseSink * This);
void __stdcall AsyncIAdviseSink_Begin_RemoteOnSave_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall AsyncIAdviseSink_Finish_RemoteOnSave_Proxy(
 AsyncIAdviseSink * This);
void __stdcall AsyncIAdviseSink_Finish_RemoteOnSave_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall AsyncIAdviseSink_Begin_RemoteOnClose_Proxy(
 AsyncIAdviseSink * This);
void __stdcall AsyncIAdviseSink_Begin_RemoteOnClose_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall AsyncIAdviseSink_Finish_RemoteOnClose_Proxy(
 AsyncIAdviseSink * This);
void __stdcall AsyncIAdviseSink_Finish_RemoteOnClose_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IAdviseSink2 *LPADVISESINK2;
extern "C" const IID IID_IAdviseSink2;
 struct __declspec(uuid("00000125-0000-0000-C000-000000000046")) __declspec(novtable)
 IAdviseSink2 : public IAdviseSink
 {
 public:
 virtual void __stdcall OnLinkSrcChange(
 IMoniker *pmk) = 0;
 };
 HRESULT __stdcall IAdviseSink2_RemoteOnLinkSrcChange_Proxy(
 IAdviseSink2 * This,
 IMoniker *pmk);
void __stdcall IAdviseSink2_RemoteOnLinkSrcChange_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
extern "C" const IID IID_AsyncIAdviseSink2;
 struct __declspec(uuid("00000151-0000-0000-C000-000000000046")) __declspec(novtable)
 AsyncIAdviseSink2 : public AsyncIAdviseSink
 {
 public:
 virtual void __stdcall Begin_OnLinkSrcChange(
 IMoniker *pmk) = 0;
 virtual void __stdcall Finish_OnLinkSrcChange( void) = 0;
 };
 HRESULT __stdcall AsyncIAdviseSink2_Begin_RemoteOnLinkSrcChange_Proxy(
 AsyncIAdviseSink2 * This,
 IMoniker *pmk);
void __stdcall AsyncIAdviseSink2_Begin_RemoteOnLinkSrcChange_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall AsyncIAdviseSink2_Finish_RemoteOnLinkSrcChange_Proxy(
 AsyncIAdviseSink2 * This);
void __stdcall AsyncIAdviseSink2_Finish_RemoteOnLinkSrcChange_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IDataObject *LPDATAOBJECT;
typedef
enum tagDATADIR
 { DATADIR_GET = 1,
 DATADIR_SET = 2
 } DATADIR;
extern "C" const IID IID_IDataObject;
 struct __declspec(uuid("0000010e-0000-0000-C000-000000000046")) __declspec(novtable)
 IDataObject : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetData(
 FORMATETC *pformatetcIn,
 STGMEDIUM *pmedium) = 0;
 virtual HRESULT __stdcall GetDataHere(
 FORMATETC *pformatetc,
 STGMEDIUM *pmedium) = 0;
 virtual HRESULT __stdcall QueryGetData(
 FORMATETC *pformatetc) = 0;
 virtual HRESULT __stdcall GetCanonicalFormatEtc(
 FORMATETC *pformatectIn,
 FORMATETC *pformatetcOut) = 0;
 virtual HRESULT __stdcall SetData(
 FORMATETC *pformatetc,
 STGMEDIUM *pmedium,
 BOOL fRelease) = 0;
 virtual HRESULT __stdcall EnumFormatEtc(
 DWORD dwDirection,
 IEnumFORMATETC **ppenumFormatEtc) = 0;
 virtual HRESULT __stdcall DAdvise(
 FORMATETC *pformatetc,
 DWORD advf,
 IAdviseSink *pAdvSink,
 DWORD *pdwConnection) = 0;
 virtual HRESULT __stdcall DUnadvise(
 DWORD dwConnection) = 0;
 virtual HRESULT __stdcall EnumDAdvise(
 IEnumSTATDATA **ppenumAdvise) = 0;
 };
 HRESULT __stdcall IDataObject_RemoteGetData_Proxy(
 IDataObject * This,
 FORMATETC *pformatetcIn,
 STGMEDIUM *pRemoteMedium);
void __stdcall IDataObject_RemoteGetData_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IDataObject_RemoteGetDataHere_Proxy(
 IDataObject * This,
 FORMATETC *pformatetc,
 STGMEDIUM *pRemoteMedium);
void __stdcall IDataObject_RemoteGetDataHere_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IDataObject_RemoteSetData_Proxy(
 IDataObject * This,
 FORMATETC *pformatetc,
 FLAG_STGMEDIUM *pmedium,
 BOOL fRelease);
void __stdcall IDataObject_RemoteSetData_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IDataAdviseHolder *LPDATAADVISEHOLDER;
extern "C" const IID IID_IDataAdviseHolder;
 struct __declspec(uuid("00000110-0000-0000-C000-000000000046")) __declspec(novtable)
 IDataAdviseHolder : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Advise(
 IDataObject *pDataObject,
 FORMATETC *pFetc,
 DWORD advf,
 IAdviseSink *pAdvise,
 DWORD *pdwConnection) = 0;
 virtual HRESULT __stdcall Unadvise(
 DWORD dwConnection) = 0;
 virtual HRESULT __stdcall EnumAdvise(
 IEnumSTATDATA **ppenumAdvise) = 0;
 virtual HRESULT __stdcall SendOnDataChange(
 IDataObject *pDataObject,
 DWORD dwReserved,
 DWORD advf) = 0;
 };
typedef IMessageFilter *LPMESSAGEFILTER;
typedef
enum tagCALLTYPE
 { CALLTYPE_TOPLEVEL = 1,
 CALLTYPE_NESTED = 2,
 CALLTYPE_ASYNC = 3,
 CALLTYPE_TOPLEVEL_CALLPENDING = 4,
 CALLTYPE_ASYNC_CALLPENDING = 5
 } CALLTYPE;
typedef
enum tagSERVERCALL
 { SERVERCALL_ISHANDLED = 0,
 SERVERCALL_REJECTED = 1,
 SERVERCALL_RETRYLATER = 2
 } SERVERCALL;
typedef
enum tagPENDINGTYPE
 { PENDINGTYPE_TOPLEVEL = 1,
 PENDINGTYPE_NESTED = 2
 } PENDINGTYPE;
typedef
enum tagPENDINGMSG
 { PENDINGMSG_CANCELCALL = 0,
 PENDINGMSG_WAITNOPROCESS = 1,
 PENDINGMSG_WAITDEFPROCESS = 2
 } PENDINGMSG;
typedef struct tagINTERFACEINFO
 {
 IUnknown *pUnk;
 IID iid;
 WORD wMethod;
 } INTERFACEINFO;
typedef struct tagINTERFACEINFO *LPINTERFACEINFO;
extern "C" const IID IID_IMessageFilter;
 struct __declspec(uuid("00000016-0000-0000-C000-000000000046")) __declspec(novtable)
 IMessageFilter : public IUnknown
 {
 public:
 virtual DWORD __stdcall HandleInComingCall(
 DWORD dwCallType,
 HTASK htaskCaller,
 DWORD dwTickCount,
 LPINTERFACEINFO lpInterfaceInfo) = 0;
 virtual DWORD __stdcall RetryRejectedCall(
 HTASK htaskCallee,
 DWORD dwTickCount,
 DWORD dwRejectType) = 0;
 virtual DWORD __stdcall MessagePending(
 HTASK htaskCallee,
 DWORD dwTickCount,
 DWORD dwPendingType) = 0;
 };
typedef unsigned long RPCOLEDATAREP;
typedef struct tagRPCOLEMESSAGE
 {
 void *reserved1;
 RPCOLEDATAREP dataRepresentation;
 void *Buffer;
 ULONG cbBuffer;
 ULONG iMethod;
 void *reserved2[ 5 ];
 ULONG rpcFlags;
 } RPCOLEMESSAGE;
typedef RPCOLEMESSAGE *PRPCOLEMESSAGE;
extern "C" const IID IID_IRpcChannelBuffer;
 struct __declspec(uuid("D5F56B60-593B-101A-B569-08002B2DBF7A")) __declspec(novtable)
 IRpcChannelBuffer : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetBuffer(
 RPCOLEMESSAGE *pMessage,
 const IID & riid) = 0;
 virtual HRESULT __stdcall SendReceive(
 RPCOLEMESSAGE *pMessage,
 ULONG *pStatus) = 0;
 virtual HRESULT __stdcall FreeBuffer(
 RPCOLEMESSAGE *pMessage) = 0;
 virtual HRESULT __stdcall GetDestCtx(
 DWORD *pdwDestContext,
 void **ppvDestContext) = 0;
 virtual HRESULT __stdcall IsConnected( void) = 0;
 };
extern "C" const IID IID_IRpcChannelBuffer2;
 struct __declspec(uuid("594f31d0-7f19-11d0-b194-00a0c90dc8bf")) __declspec(novtable)
 IRpcChannelBuffer2 : public IRpcChannelBuffer
 {
 public:
 virtual HRESULT __stdcall GetProtocolVersion(
 DWORD *pdwVersion) = 0;
 };
extern "C" const IID IID_IAsyncRpcChannelBuffer;
 struct __declspec(uuid("a5029fb6-3c34-11d1-9c99-00c04fb998aa")) __declspec(novtable)
 IAsyncRpcChannelBuffer : public IRpcChannelBuffer2
 {
 public:
 virtual HRESULT __stdcall Send(
 RPCOLEMESSAGE *pMsg,
 ISynchronize *pSync,
 ULONG *pulStatus) = 0;
 virtual HRESULT __stdcall Receive(
 RPCOLEMESSAGE *pMsg,
 ULONG *pulStatus) = 0;
 virtual HRESULT __stdcall GetDestCtxEx(
 RPCOLEMESSAGE *pMsg,
 DWORD *pdwDestContext,
 void **ppvDestContext) = 0;
 };
extern "C" const IID IID_IRpcChannelBuffer3;
 struct __declspec(uuid("25B15600-0115-11d0-BF0D-00AA00B8DFD2")) __declspec(novtable)
 IRpcChannelBuffer3 : public IRpcChannelBuffer2
 {
 public:
 virtual HRESULT __stdcall Send(
 RPCOLEMESSAGE *pMsg,
 ULONG *pulStatus) = 0;
 virtual HRESULT __stdcall Receive(
 RPCOLEMESSAGE *pMsg,
 ULONG ulSize,
 ULONG *pulStatus) = 0;
 virtual HRESULT __stdcall Cancel(
 RPCOLEMESSAGE *pMsg) = 0;
 virtual HRESULT __stdcall GetCallContext(
 RPCOLEMESSAGE *pMsg,
 const IID & riid,
 void **pInterface) = 0;
 virtual HRESULT __stdcall GetDestCtxEx(
 RPCOLEMESSAGE *pMsg,
 DWORD *pdwDestContext,
 void **ppvDestContext) = 0;
 virtual HRESULT __stdcall GetState(
 RPCOLEMESSAGE *pMsg,
 DWORD *pState) = 0;
 virtual HRESULT __stdcall RegisterAsync(
 RPCOLEMESSAGE *pMsg,
 IAsyncManager *pAsyncMgr) = 0;
 };
extern "C" const IID IID_IRpcSyntaxNegotiate;
 struct __declspec(uuid("58a08519-24c8-4935-b482-3fd823333a4f")) __declspec(novtable)
 IRpcSyntaxNegotiate : public IUnknown
 {
 public:
 virtual HRESULT __stdcall NegotiateSyntax(
 RPCOLEMESSAGE *pMsg) = 0;
 };
extern "C" const IID IID_IRpcProxyBuffer;
 struct __declspec(uuid("D5F56A34-593B-101A-B569-08002B2DBF7A")) __declspec(novtable)
 IRpcProxyBuffer : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Connect(
 IRpcChannelBuffer *pRpcChannelBuffer) = 0;
 virtual void __stdcall Disconnect( void) = 0;
 };
extern "C" const IID IID_IRpcStubBuffer;
 struct __declspec(uuid("D5F56AFC-593B-101A-B569-08002B2DBF7A")) __declspec(novtable)
 IRpcStubBuffer : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Connect(
 IUnknown *pUnkServer) = 0;
 virtual void __stdcall Disconnect( void) = 0;
 virtual HRESULT __stdcall Invoke(
 RPCOLEMESSAGE *_prpcmsg,
 IRpcChannelBuffer *_pRpcChannelBuffer) = 0;
 virtual IRpcStubBuffer *__stdcall IsIIDSupported(
 const IID & riid) = 0;
 virtual ULONG __stdcall CountRefs( void) = 0;
 virtual HRESULT __stdcall DebugServerQueryInterface(
 void **ppv) = 0;
 virtual void __stdcall DebugServerRelease(
 void *pv) = 0;
 };
extern "C" const IID IID_IPSFactoryBuffer;
 struct __declspec(uuid("D5F569D0-593B-101A-B569-08002B2DBF7A")) __declspec(novtable)
 IPSFactoryBuffer : public IUnknown
 {
 public:
 virtual HRESULT __stdcall CreateProxy(
 IUnknown *pUnkOuter,
 const IID & riid,
 IRpcProxyBuffer **ppProxy,
 void **ppv) = 0;
 virtual HRESULT __stdcall CreateStub(
 const IID & riid,
 IUnknown *pUnkServer,
 IRpcStubBuffer **ppStub) = 0;
 };
typedef struct SChannelHookCallInfo
 {
 IID iid;
 DWORD cbSize;
 GUID uCausality;
 DWORD dwServerPid;
 DWORD iMethod;
 void *pObject;
 } SChannelHookCallInfo;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0041_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0041_v0_0_s_ifspec;
extern "C" const IID IID_IChannelHook;
 struct __declspec(uuid("1008c4a0-7613-11cf-9af1-0020af6e72f4")) __declspec(novtable)
 IChannelHook : public IUnknown
 {
 public:
 virtual void __stdcall ClientGetSize(
 const GUID & uExtent,
 const IID & riid,
 ULONG *pDataSize) = 0;
 virtual void __stdcall ClientFillBuffer(
 const GUID & uExtent,
 const IID & riid,
 ULONG *pDataSize,
 void *pDataBuffer) = 0;
 virtual void __stdcall ClientNotify(
 const GUID & uExtent,
 const IID & riid,
 ULONG cbDataSize,
 void *pDataBuffer,
 DWORD lDataRep,
 HRESULT hrFault) = 0;
 virtual void __stdcall ServerNotify(
 const GUID & uExtent,
 const IID & riid,
 ULONG cbDataSize,
 void *pDataBuffer,
 DWORD lDataRep) = 0;
 virtual void __stdcall ServerGetSize(
 const GUID & uExtent,
 const IID & riid,
 HRESULT hrFault,
 ULONG *pDataSize) = 0;
 virtual void __stdcall ServerFillBuffer(
 const GUID & uExtent,
 const IID & riid,
 ULONG *pDataSize,
 void *pDataBuffer,
 HRESULT hrFault) = 0;
 };
extern const FMTID FMTID_SummaryInformation;
extern const FMTID FMTID_DocSummaryInformation;
extern const FMTID FMTID_UserDefinedProperties;
extern const FMTID FMTID_DiscardableInformation;
extern const FMTID FMTID_ImageSummaryInformation;
extern const FMTID FMTID_AudioSummaryInformation;
extern const FMTID FMTID_VideoSummaryInformation;
extern const FMTID FMTID_MediaFileSummaryInformation;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0042_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0042_v0_0_s_ifspec;
typedef struct tagSOLE_AUTHENTICATION_SERVICE
 {
 DWORD dwAuthnSvc;
 DWORD dwAuthzSvc;
 OLECHAR *pPrincipalName;
 HRESULT hr;
 } SOLE_AUTHENTICATION_SERVICE;
typedef SOLE_AUTHENTICATION_SERVICE *PSOLE_AUTHENTICATION_SERVICE;
typedef
enum tagEOLE_AUTHENTICATION_CAPABILITIES
 { EOAC_NONE = 0,
 EOAC_MUTUAL_AUTH = 0x1,
 EOAC_STATIC_CLOAKING = 0x20,
 EOAC_DYNAMIC_CLOAKING = 0x40,
 EOAC_ANY_AUTHORITY = 0x80,
 EOAC_MAKE_FULLSIC = 0x100,
 EOAC_DEFAULT = 0x800,
 EOAC_SECURE_REFS = 0x2,
 EOAC_ACCESS_CONTROL = 0x4,
 EOAC_APPID = 0x8,
 EOAC_DYNAMIC = 0x10,
 EOAC_REQUIRE_FULLSIC = 0x200,
 EOAC_AUTO_IMPERSONATE = 0x400,
 EOAC_NO_CUSTOM_MARSHAL = 0x2000,
 EOAC_DISABLE_AAA = 0x1000
 } EOLE_AUTHENTICATION_CAPABILITIES;
typedef struct tagSOLE_AUTHENTICATION_INFO
 {
 DWORD dwAuthnSvc;
 DWORD dwAuthzSvc;
 void *pAuthInfo;
 } SOLE_AUTHENTICATION_INFO;
typedef struct tagSOLE_AUTHENTICATION_INFO *PSOLE_AUTHENTICATION_INFO;
typedef struct tagSOLE_AUTHENTICATION_LIST
 {
 DWORD cAuthInfo;
 SOLE_AUTHENTICATION_INFO *aAuthInfo;
 } SOLE_AUTHENTICATION_LIST;
typedef struct tagSOLE_AUTHENTICATION_LIST *PSOLE_AUTHENTICATION_LIST;
extern "C" const IID IID_IClientSecurity;
 struct __declspec(uuid("0000013D-0000-0000-C000-000000000046")) __declspec(novtable)
 IClientSecurity : public IUnknown
 {
 public:
 virtual HRESULT __stdcall QueryBlanket(
 IUnknown *pProxy,
 DWORD *pAuthnSvc,
 DWORD *pAuthzSvc,
 OLECHAR **pServerPrincName,
 DWORD *pAuthnLevel,
 DWORD *pImpLevel,
 void **pAuthInfo,
 DWORD *pCapabilites) = 0;
 virtual HRESULT __stdcall SetBlanket(
 IUnknown *pProxy,
 DWORD dwAuthnSvc,
 DWORD dwAuthzSvc,
 OLECHAR *pServerPrincName,
 DWORD dwAuthnLevel,
 DWORD dwImpLevel,
 void *pAuthInfo,
 DWORD dwCapabilities) = 0;
 virtual HRESULT __stdcall CopyProxy(
 IUnknown *pProxy,
 IUnknown **ppCopy) = 0;
 };
extern "C" const IID IID_IServerSecurity;
 struct __declspec(uuid("0000013E-0000-0000-C000-000000000046")) __declspec(novtable)
 IServerSecurity : public IUnknown
 {
 public:
 virtual HRESULT __stdcall QueryBlanket(
 DWORD *pAuthnSvc,
 DWORD *pAuthzSvc,
 OLECHAR **pServerPrincName,
 DWORD *pAuthnLevel,
 DWORD *pImpLevel,
 void **pPrivs,
 DWORD *pCapabilities) = 0;
 virtual HRESULT __stdcall ImpersonateClient( void) = 0;
 virtual HRESULT __stdcall RevertToSelf( void) = 0;
 virtual BOOL __stdcall IsImpersonating( void) = 0;
 };
extern "C" const IID IID_IClassActivator;
 struct __declspec(uuid("00000140-0000-0000-C000-000000000046")) __declspec(novtable)
 IClassActivator : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetClassObject(
 const IID & rclsid,
 DWORD dwClassContext,
 LCID locale,
 const IID & riid,
 void **ppv) = 0;
 };
extern "C" const IID IID_IRpcOptions;
 struct __declspec(uuid("00000144-0000-0000-C000-000000000046")) __declspec(novtable)
 IRpcOptions : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Set(
 IUnknown *pPrx,
 DWORD dwProperty,
 ULONG_PTR dwValue) = 0;
 virtual HRESULT __stdcall Query(
 IUnknown *pPrx,
 DWORD dwProperty,
 ULONG_PTR *pdwValue) = 0;
 };
enum __MIDL___MIDL_itf_objidl_0000_0046_0001
 { COMBND_RPCTIMEOUT = 0x1,
 COMBND_SERVER_LOCALITY = 0x2
 } ;
enum __MIDL___MIDL_itf_objidl_0000_0046_0002
 { SERVER_LOCALITY_PROCESS_LOCAL = 0,
 SERVER_LOCALITY_MACHINE_LOCAL = 1,
 SERVER_LOCALITY_REMOTE = 2
 } ;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0046_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0046_v0_0_s_ifspec;
extern "C" const IID IID_IFillLockBytes;
 struct __declspec(uuid("99caf010-415e-11cf-8814-00aa00b569f5")) __declspec(novtable)
 IFillLockBytes : public IUnknown
 {
 public:
 virtual HRESULT __stdcall FillAppend(
 const void *pv,
 ULONG cb,
 ULONG *pcbWritten) = 0;
 virtual HRESULT __stdcall FillAt(
 ULARGE_INTEGER ulOffset,
 const void *pv,
 ULONG cb,
 ULONG *pcbWritten) = 0;
 virtual HRESULT __stdcall SetFillSize(
 ULARGE_INTEGER ulSize) = 0;
 virtual HRESULT __stdcall Terminate(
 BOOL bCanceled) = 0;
 };
 HRESULT __stdcall IFillLockBytes_RemoteFillAppend_Proxy(
 IFillLockBytes * This,
 const byte *pv,
 ULONG cb,
 ULONG *pcbWritten);
void __stdcall IFillLockBytes_RemoteFillAppend_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IFillLockBytes_RemoteFillAt_Proxy(
 IFillLockBytes * This,
 ULARGE_INTEGER ulOffset,
 const byte *pv,
 ULONG cb,
 ULONG *pcbWritten);
void __stdcall IFillLockBytes_RemoteFillAt_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
extern "C" const IID IID_IProgressNotify;
 struct __declspec(uuid("a9d758a0-4617-11cf-95fc-00aa00680db4")) __declspec(novtable)
 IProgressNotify : public IUnknown
 {
 public:
 virtual HRESULT __stdcall OnProgress(
 DWORD dwProgressCurrent,
 DWORD dwProgressMaximum,
 BOOL fAccurate,
 BOOL fOwner) = 0;
 };
typedef struct tagStorageLayout
 {
 DWORD LayoutType;
 OLECHAR *pwcsElementName;
 LARGE_INTEGER cOffset;
 LARGE_INTEGER cBytes;
 } StorageLayout;
extern "C" const IID IID_ILayoutStorage;
 struct __declspec(uuid("0e6d4d90-6738-11cf-9608-00aa00680db4")) __declspec(novtable)
 ILayoutStorage : public IUnknown
 {
 public:
 virtual HRESULT __stdcall LayoutScript(
 StorageLayout *pStorageLayout,
 DWORD nEntries,
 DWORD glfInterleavedFlag) = 0;
 virtual HRESULT __stdcall BeginMonitor( void) = 0;
 virtual HRESULT __stdcall EndMonitor( void) = 0;
 virtual HRESULT __stdcall ReLayoutDocfile(
 OLECHAR *pwcsNewDfName) = 0;
 virtual HRESULT __stdcall ReLayoutDocfileOnILockBytes(
 ILockBytes *pILockBytes) = 0;
 };
extern "C" const IID IID_IBlockingLock;
 struct __declspec(uuid("30f3d47a-6447-11d1-8e3c-00c04fb9386d")) __declspec(novtable)
 IBlockingLock : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Lock(
 DWORD dwTimeout) = 0;
 virtual HRESULT __stdcall Unlock( void) = 0;
 };
extern "C" const IID IID_ITimeAndNoticeControl;
 struct __declspec(uuid("bc0bf6ae-8878-11d1-83e9-00c04fc2c6d4")) __declspec(novtable)
 ITimeAndNoticeControl : public IUnknown
 {
 public:
 virtual HRESULT __stdcall SuppressChanges(
 DWORD res1,
 DWORD res2) = 0;
 };
extern "C" const IID IID_IOplockStorage;
 struct __declspec(uuid("8d19c834-8879-11d1-83e9-00c04fc2c6d4")) __declspec(novtable)
 IOplockStorage : public IUnknown
 {
 public:
 virtual HRESULT __stdcall CreateStorageEx(
 LPCWSTR pwcsName,
 DWORD grfMode,
 DWORD stgfmt,
 DWORD grfAttrs,
 const IID & riid,
 void **ppstgOpen) = 0;
 virtual HRESULT __stdcall OpenStorageEx(
 LPCWSTR pwcsName,
 DWORD grfMode,
 DWORD stgfmt,
 DWORD grfAttrs,
 const IID & riid,
 void **ppstgOpen) = 0;
 };
typedef ISurrogate *LPSURROGATE;
extern "C" const IID IID_ISurrogate;
 struct __declspec(uuid("00000022-0000-0000-C000-000000000046")) __declspec(novtable)
 ISurrogate : public IUnknown
 {
 public:
 virtual HRESULT __stdcall LoadDllServer(
 const IID & Clsid) = 0;
 virtual HRESULT __stdcall FreeSurrogate( void) = 0;
 };
typedef IGlobalInterfaceTable *LPGLOBALINTERFACETABLE;
extern "C" const IID IID_IGlobalInterfaceTable;
 struct __declspec(uuid("00000146-0000-0000-C000-000000000046")) __declspec(novtable)
 IGlobalInterfaceTable : public IUnknown
 {
 public:
 virtual HRESULT __stdcall RegisterInterfaceInGlobal(
 IUnknown *pUnk,
 const IID & riid,
 DWORD *pdwCookie) = 0;
 virtual HRESULT __stdcall RevokeInterfaceFromGlobal(
 DWORD dwCookie) = 0;
 virtual HRESULT __stdcall GetInterfaceFromGlobal(
 DWORD dwCookie,
 const IID & riid,
 void **ppv) = 0;
 };
extern "C" const IID IID_IDirectWriterLock;
 struct __declspec(uuid("0e6d4d92-6738-11cf-9608-00aa00680db4")) __declspec(novtable)
 IDirectWriterLock : public IUnknown
 {
 public:
 virtual HRESULT __stdcall WaitForWriteAccess(
 DWORD dwTimeout) = 0;
 virtual HRESULT __stdcall ReleaseWriteAccess( void) = 0;
 virtual HRESULT __stdcall HaveWriteAccess( void) = 0;
 };
extern "C" const IID IID_ISynchronize;
 struct __declspec(uuid("00000030-0000-0000-C000-000000000046")) __declspec(novtable)
 ISynchronize : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Wait(
 DWORD dwFlags,
 DWORD dwMilliseconds) = 0;
 virtual HRESULT __stdcall Signal( void) = 0;
 virtual HRESULT __stdcall Reset( void) = 0;
 };
extern "C" const IID IID_ISynchronizeHandle;
 struct __declspec(uuid("00000031-0000-0000-C000-000000000046")) __declspec(novtable)
 ISynchronizeHandle : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetHandle(
 HANDLE *ph) = 0;
 };
extern "C" const IID IID_ISynchronizeEvent;
 struct __declspec(uuid("00000032-0000-0000-C000-000000000046")) __declspec(novtable)
 ISynchronizeEvent : public ISynchronizeHandle
 {
 public:
 virtual HRESULT __stdcall SetEventHandle(
 HANDLE *ph) = 0;
 };
extern "C" const IID IID_ISynchronizeContainer;
 struct __declspec(uuid("00000033-0000-0000-C000-000000000046")) __declspec(novtable)
 ISynchronizeContainer : public IUnknown
 {
 public:
 virtual HRESULT __stdcall AddSynchronize(
 ISynchronize *pSync) = 0;
 virtual HRESULT __stdcall WaitMultiple(
 DWORD dwFlags,
 DWORD dwTimeOut,
 ISynchronize **ppSync) = 0;
 };
extern "C" const IID IID_ISynchronizeMutex;
 struct __declspec(uuid("00000025-0000-0000-C000-000000000046")) __declspec(novtable)
 ISynchronizeMutex : public ISynchronize
 {
 public:
 virtual HRESULT __stdcall ReleaseMutex( void) = 0;
 };
typedef ICancelMethodCalls *LPCANCELMETHODCALLS;
extern "C" const IID IID_ICancelMethodCalls;
 struct __declspec(uuid("00000029-0000-0000-C000-000000000046")) __declspec(novtable)
 ICancelMethodCalls : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Cancel(
 ULONG ulSeconds) = 0;
 virtual HRESULT __stdcall TestCancel( void) = 0;
 };
typedef
enum tagDCOM_CALL_STATE
 { DCOM_NONE = 0,
 DCOM_CALL_COMPLETE = 0x1,
 DCOM_CALL_CANCELED = 0x2
 } DCOM_CALL_STATE;
extern "C" const IID IID_IAsyncManager;
 struct __declspec(uuid("0000002A-0000-0000-C000-000000000046")) __declspec(novtable)
 IAsyncManager : public IUnknown
 {
 public:
 virtual HRESULT __stdcall CompleteCall(
 HRESULT Result) = 0;
 virtual HRESULT __stdcall GetCallContext(
 const IID & riid,
 void **pInterface) = 0;
 virtual HRESULT __stdcall GetState(
 ULONG *pulStateFlags) = 0;
 };
extern "C" const IID IID_ICallFactory;
 struct __declspec(uuid("1c733a30-2a1c-11ce-ade5-00aa0044773d")) __declspec(novtable)
 ICallFactory : public IUnknown
 {
 public:
 virtual HRESULT __stdcall CreateCall(
 const IID & riid,
 IUnknown *pCtrlUnk,
 const IID & riid2,
 IUnknown **ppv) = 0;
 };
extern "C" const IID IID_IRpcHelper;
 struct __declspec(uuid("00000149-0000-0000-C000-000000000046")) __declspec(novtable)
 IRpcHelper : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetDCOMProtocolVersion(
 DWORD *pComVersion) = 0;
 virtual HRESULT __stdcall GetIIDFromOBJREF(
 void *pObjRef,
 IID **piid) = 0;
 };
extern "C" const IID IID_IReleaseMarshalBuffers;
 struct __declspec(uuid("eb0cb9e8-7996-11d2-872e-0000f8080859")) __declspec(novtable)
 IReleaseMarshalBuffers : public IUnknown
 {
 public:
 virtual HRESULT __stdcall ReleaseMarshalBuffer(
 RPCOLEMESSAGE *pMsg,
 DWORD dwFlags,
 IUnknown *pChnl) = 0;
 };
extern "C" const IID IID_IWaitMultiple;
 struct __declspec(uuid("0000002B-0000-0000-C000-000000000046")) __declspec(novtable)
 IWaitMultiple : public IUnknown
 {
 public:
 virtual HRESULT __stdcall WaitMultiple(
 DWORD timeout,
 ISynchronize **pSync) = 0;
 virtual HRESULT __stdcall AddSynchronize(
 ISynchronize *pSync) = 0;
 };
extern "C" const IID IID_IUrlMon;
 struct __declspec(uuid("00000026-0000-0000-C000-000000000046")) __declspec(novtable)
 IUrlMon : public IUnknown
 {
 public:
 virtual HRESULT __stdcall AsyncGetClassBits(
 const IID & rclsid,
 LPCWSTR pszTYPE,
 LPCWSTR pszExt,
 DWORD dwFileVersionMS,
 DWORD dwFileVersionLS,
 LPCWSTR pszCodeBase,
 IBindCtx *pbc,
 DWORD dwClassContext,
 const IID & riid,
 DWORD flags) = 0;
 };
extern "C" const IID IID_IForegroundTransfer;
 struct __declspec(uuid("00000145-0000-0000-C000-000000000046")) __declspec(novtable)
 IForegroundTransfer : public IUnknown
 {
 public:
 virtual HRESULT __stdcall AllowForegroundTransfer(
 void *lpvReserved) = 0;
 };
typedef IAddrTrackingControl *LPADDRTRACKINGCONTROL;
extern "C" const IID IID_IAddrTrackingControl;
 struct __declspec(uuid("00000147-0000-0000-C000-000000000046")) __declspec(novtable)
 IAddrTrackingControl : public IUnknown
 {
 public:
 virtual HRESULT __stdcall EnableCOMDynamicAddrTracking( void) = 0;
 virtual HRESULT __stdcall DisableCOMDynamicAddrTracking( void) = 0;
 };
typedef IAddrExclusionControl *LPADDREXCLUSIONCONTROL;
extern "C" const IID IID_IAddrExclusionControl;
 struct __declspec(uuid("00000148-0000-0000-C000-000000000046")) __declspec(novtable)
 IAddrExclusionControl : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetCurrentAddrExclusionList(
 const IID & riid,
 void **ppEnumerator) = 0;
 virtual HRESULT __stdcall UpdateAddrExclusionList(
 IUnknown *pEnumerator) = 0;
 };
extern "C" const IID IID_IPipeByte;
 struct __declspec(uuid("DB2F3ACA-2F86-11d1-8E04-00C04FB9989A")) __declspec(novtable)
 IPipeByte : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Pull(
 BYTE *buf,
 ULONG cRequest,
 ULONG *pcReturned) = 0;
 virtual HRESULT __stdcall Push(
 BYTE *buf,
 ULONG cSent) = 0;
 };
extern "C" const IID IID_AsyncIPipeByte;
 struct __declspec(uuid("DB2F3ACB-2F86-11d1-8E04-00C04FB9989A")) __declspec(novtable)
 AsyncIPipeByte : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Begin_Pull(
 ULONG cRequest) = 0;
 virtual HRESULT __stdcall Finish_Pull(
 BYTE *buf,
 ULONG *pcReturned) = 0;
 virtual HRESULT __stdcall Begin_Push(
 BYTE *buf,
 ULONG cSent) = 0;
 virtual HRESULT __stdcall Finish_Push( void) = 0;
 };
extern "C" const IID IID_IPipeLong;
 struct __declspec(uuid("DB2F3ACC-2F86-11d1-8E04-00C04FB9989A")) __declspec(novtable)
 IPipeLong : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Pull(
 LONG *buf,
 ULONG cRequest,
 ULONG *pcReturned) = 0;
 virtual HRESULT __stdcall Push(
 LONG *buf,
 ULONG cSent) = 0;
 };
extern "C" const IID IID_AsyncIPipeLong;
 struct __declspec(uuid("DB2F3ACD-2F86-11d1-8E04-00C04FB9989A")) __declspec(novtable)
 AsyncIPipeLong : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Begin_Pull(
 ULONG cRequest) = 0;
 virtual HRESULT __stdcall Finish_Pull(
 LONG *buf,
 ULONG *pcReturned) = 0;
 virtual HRESULT __stdcall Begin_Push(
 LONG *buf,
 ULONG cSent) = 0;
 virtual HRESULT __stdcall Finish_Push( void) = 0;
 };
extern "C" const IID IID_IPipeDouble;
 struct __declspec(uuid("DB2F3ACE-2F86-11d1-8E04-00C04FB9989A")) __declspec(novtable)
 IPipeDouble : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Pull(
 DOUBLE *buf,
 ULONG cRequest,
 ULONG *pcReturned) = 0;
 virtual HRESULT __stdcall Push(
 DOUBLE *buf,
 ULONG cSent) = 0;
 };
extern "C" const IID IID_AsyncIPipeDouble;
 struct __declspec(uuid("DB2F3ACF-2F86-11d1-8E04-00C04FB9989A")) __declspec(novtable)
 AsyncIPipeDouble : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Begin_Pull(
 ULONG cRequest) = 0;
 virtual HRESULT __stdcall Finish_Pull(
 DOUBLE *buf,
 ULONG *pcReturned) = 0;
 virtual HRESULT __stdcall Begin_Push(
 DOUBLE *buf,
 ULONG cSent) = 0;
 virtual HRESULT __stdcall Finish_Push( void) = 0;
 };
extern "C" const IID IID_IThumbnailExtractor;
 struct __declspec(uuid("969dc708-5c76-11d1-8d86-0000f804b057")) __declspec(novtable)
 IThumbnailExtractor : public IUnknown
 {
 public:
 virtual HRESULT __stdcall ExtractThumbnail(
 IStorage *pStg,
 ULONG ulLength,
 ULONG ulHeight,
 ULONG *pulOutputLength,
 ULONG *pulOutputHeight,
 HBITMAP *phOutputBitmap) = 0;
 virtual HRESULT __stdcall OnFileUpdated(
 IStorage *pStg) = 0;
 };
extern "C" const IID IID_IDummyHICONIncluder;
 struct __declspec(uuid("947990de-cc28-11d2-a0f7-00805f858fb1")) __declspec(novtable)
 IDummyHICONIncluder : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Dummy(
 HICON h1,
 HDC h2) = 0;
 };
typedef
enum tagApplicationType
 { ServerApplication = 0,
 LibraryApplication = ( ServerApplication + 1 )
 } ApplicationType;
typedef
enum tagShutdownType
 { IdleShutdown = 0,
 ForcedShutdown = ( IdleShutdown + 1 )
 } ShutdownType;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0078_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0078_v0_0_s_ifspec;
extern "C" const IID IID_IProcessLock;
 struct __declspec(uuid("000001d5-0000-0000-C000-000000000046")) __declspec(novtable)
 IProcessLock : public IUnknown
 {
 public:
 virtual ULONG __stdcall AddRefOnProcess( void) = 0;
 virtual ULONG __stdcall ReleaseRefOnProcess( void) = 0;
 };
extern "C" const IID IID_ISurrogateService;
 struct __declspec(uuid("000001d4-0000-0000-C000-000000000046")) __declspec(novtable)
 ISurrogateService : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Init(
 const GUID & rguidProcessID,
 IProcessLock *pProcessLock,
 BOOL *pfApplicationAware) = 0;
 virtual HRESULT __stdcall ApplicationLaunch(
 const GUID & rguidApplID,
 ApplicationType appType) = 0;
 virtual HRESULT __stdcall ApplicationFree(
 const GUID & rguidApplID) = 0;
 virtual HRESULT __stdcall CatalogRefresh(
 ULONG ulReserved) = 0;
 virtual HRESULT __stdcall ProcessShutdown(
 ShutdownType shutdownType) = 0;
 };
typedef
enum _APTTYPE
 { APTTYPE_CURRENT = -1,
 APTTYPE_STA = 0,
 APTTYPE_MTA = 1,
 APTTYPE_NA = 2,
 APTTYPE_MAINSTA = 3
 } APTTYPE;
typedef
enum _THDTYPE
 { THDTYPE_BLOCKMESSAGES = 0,
 THDTYPE_PROCESSMESSAGES = 1
 } THDTYPE;
typedef DWORD APARTMENTID;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0080_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0080_v0_0_s_ifspec;
extern "C" const IID IID_IComThreadingInfo;
 struct __declspec(uuid("000001ce-0000-0000-C000-000000000046")) __declspec(novtable)
 IComThreadingInfo : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetCurrentApartmentType(
 APTTYPE *pAptType) = 0;
 virtual HRESULT __stdcall GetCurrentThreadType(
 THDTYPE *pThreadType) = 0;
 virtual HRESULT __stdcall GetCurrentLogicalThreadId(
 GUID *pguidLogicalThreadId) = 0;
 virtual HRESULT __stdcall SetCurrentLogicalThreadId(
 const GUID & rguid) = 0;
 };
extern "C" const IID IID_IProcessInitControl;
 struct __declspec(uuid("72380d55-8d2b-43a3-8513-2b6ef31434e9")) __declspec(novtable)
 IProcessInitControl : public IUnknown
 {
 public:
 virtual HRESULT __stdcall ResetInitializerTimeout(
 DWORD dwSecondsRemaining) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0082_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0082_v0_0_s_ifspec;
typedef IInitializeSpy *LPINITIALIZESPY;
extern "C" const IID IID_IInitializeSpy;
 struct __declspec(uuid("00000034-0000-0000-C000-000000000046")) __declspec(novtable)
 IInitializeSpy : public IUnknown
 {
 public:
 virtual HRESULT __stdcall PreInitialize(
 DWORD dwCoInit,
 DWORD dwCurThreadAptRefs) = 0;
 virtual HRESULT __stdcall PostInitialize(
 HRESULT hrCoInit,
 DWORD dwCoInit,
 DWORD dwNewThreadAptRefs) = 0;
 virtual HRESULT __stdcall PreUninitialize(
 DWORD dwCurThreadAptRefs) = 0;
 virtual HRESULT __stdcall PostUninitialize(
 DWORD dwNewThreadAptRefs) = 0;
 };
#pragma warning(pop)
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0083_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_objidl_0000_0083_v0_0_s_ifspec;
unsigned long __stdcall ASYNC_STGMEDIUM_UserSize( unsigned long *, unsigned long , ASYNC_STGMEDIUM * );
unsigned char * __stdcall ASYNC_STGMEDIUM_UserMarshal( unsigned long *, unsigned char *, ASYNC_STGMEDIUM * );
unsigned char * __stdcall ASYNC_STGMEDIUM_UserUnmarshal(unsigned long *, unsigned char *, ASYNC_STGMEDIUM * );
void __stdcall ASYNC_STGMEDIUM_UserFree( unsigned long *, ASYNC_STGMEDIUM * );
unsigned long __stdcall CLIPFORMAT_UserSize( unsigned long *, unsigned long , CLIPFORMAT * );
unsigned char * __stdcall CLIPFORMAT_UserMarshal( unsigned long *, unsigned char *, CLIPFORMAT * );
unsigned char * __stdcall CLIPFORMAT_UserUnmarshal(unsigned long *, unsigned char *, CLIPFORMAT * );
void __stdcall CLIPFORMAT_UserFree( unsigned long *, CLIPFORMAT * );
unsigned long __stdcall FLAG_STGMEDIUM_UserSize( unsigned long *, unsigned long , FLAG_STGMEDIUM * );
unsigned char * __stdcall FLAG_STGMEDIUM_UserMarshal( unsigned long *, unsigned char *, FLAG_STGMEDIUM * );
unsigned char * __stdcall FLAG_STGMEDIUM_UserUnmarshal(unsigned long *, unsigned char *, FLAG_STGMEDIUM * );
void __stdcall FLAG_STGMEDIUM_UserFree( unsigned long *, FLAG_STGMEDIUM * );
unsigned long __stdcall HBITMAP_UserSize( unsigned long *, unsigned long , HBITMAP * );
unsigned char * __stdcall HBITMAP_UserMarshal( unsigned long *, unsigned char *, HBITMAP * );
unsigned char * __stdcall HBITMAP_UserUnmarshal(unsigned long *, unsigned char *, HBITMAP * );
void __stdcall HBITMAP_UserFree( unsigned long *, HBITMAP * );
unsigned long __stdcall HDC_UserSize( unsigned long *, unsigned long , HDC * );
unsigned char * __stdcall HDC_UserMarshal( unsigned long *, unsigned char *, HDC * );
unsigned char * __stdcall HDC_UserUnmarshal(unsigned long *, unsigned char *, HDC * );
void __stdcall HDC_UserFree( unsigned long *, HDC * );
unsigned long __stdcall HICON_UserSize( unsigned long *, unsigned long , HICON * );
unsigned char * __stdcall HICON_UserMarshal( unsigned long *, unsigned char *, HICON * );
unsigned char * __stdcall HICON_UserUnmarshal(unsigned long *, unsigned char *, HICON * );
void __stdcall HICON_UserFree( unsigned long *, HICON * );
unsigned long __stdcall SNB_UserSize( unsigned long *, unsigned long , SNB * );
unsigned char * __stdcall SNB_UserMarshal( unsigned long *, unsigned char *, SNB * );
unsigned char * __stdcall SNB_UserUnmarshal(unsigned long *, unsigned char *, SNB * );
void __stdcall SNB_UserFree( unsigned long *, SNB * );
unsigned long __stdcall STGMEDIUM_UserSize( unsigned long *, unsigned long , STGMEDIUM * );
unsigned char * __stdcall STGMEDIUM_UserMarshal( unsigned long *, unsigned char *, STGMEDIUM * );
unsigned char * __stdcall STGMEDIUM_UserUnmarshal(unsigned long *, unsigned char *, STGMEDIUM * );
void __stdcall STGMEDIUM_UserFree( unsigned long *, STGMEDIUM * );
unsigned long __stdcall ASYNC_STGMEDIUM_UserSize64( unsigned long *, unsigned long , ASYNC_STGMEDIUM * );
unsigned char * __stdcall ASYNC_STGMEDIUM_UserMarshal64( unsigned long *, unsigned char *, ASYNC_STGMEDIUM * );
unsigned char * __stdcall ASYNC_STGMEDIUM_UserUnmarshal64(unsigned long *, unsigned char *, ASYNC_STGMEDIUM * );
void __stdcall ASYNC_STGMEDIUM_UserFree64( unsigned long *, ASYNC_STGMEDIUM * );
unsigned long __stdcall CLIPFORMAT_UserSize64( unsigned long *, unsigned long , CLIPFORMAT * );
unsigned char * __stdcall CLIPFORMAT_UserMarshal64( unsigned long *, unsigned char *, CLIPFORMAT * );
unsigned char * __stdcall CLIPFORMAT_UserUnmarshal64(unsigned long *, unsigned char *, CLIPFORMAT * );
void __stdcall CLIPFORMAT_UserFree64( unsigned long *, CLIPFORMAT * );
unsigned long __stdcall FLAG_STGMEDIUM_UserSize64( unsigned long *, unsigned long , FLAG_STGMEDIUM * );
unsigned char * __stdcall FLAG_STGMEDIUM_UserMarshal64( unsigned long *, unsigned char *, FLAG_STGMEDIUM * );
unsigned char * __stdcall FLAG_STGMEDIUM_UserUnmarshal64(unsigned long *, unsigned char *, FLAG_STGMEDIUM * );
void __stdcall FLAG_STGMEDIUM_UserFree64( unsigned long *, FLAG_STGMEDIUM * );
unsigned long __stdcall HBITMAP_UserSize64( unsigned long *, unsigned long , HBITMAP * );
unsigned char * __stdcall HBITMAP_UserMarshal64( unsigned long *, unsigned char *, HBITMAP * );
unsigned char * __stdcall HBITMAP_UserUnmarshal64(unsigned long *, unsigned char *, HBITMAP * );
void __stdcall HBITMAP_UserFree64( unsigned long *, HBITMAP * );
unsigned long __stdcall HDC_UserSize64( unsigned long *, unsigned long , HDC * );
unsigned char * __stdcall HDC_UserMarshal64( unsigned long *, unsigned char *, HDC * );
unsigned char * __stdcall HDC_UserUnmarshal64(unsigned long *, unsigned char *, HDC * );
void __stdcall HDC_UserFree64( unsigned long *, HDC * );
unsigned long __stdcall HICON_UserSize64( unsigned long *, unsigned long , HICON * );
unsigned char * __stdcall HICON_UserMarshal64( unsigned long *, unsigned char *, HICON * );
unsigned char * __stdcall HICON_UserUnmarshal64(unsigned long *, unsigned char *, HICON * );
void __stdcall HICON_UserFree64( unsigned long *, HICON * );
unsigned long __stdcall SNB_UserSize64( unsigned long *, unsigned long , SNB * );
unsigned char * __stdcall SNB_UserMarshal64( unsigned long *, unsigned char *, SNB * );
unsigned char * __stdcall SNB_UserUnmarshal64(unsigned long *, unsigned char *, SNB * );
void __stdcall SNB_UserFree64( unsigned long *, SNB * );
unsigned long __stdcall STGMEDIUM_UserSize64( unsigned long *, unsigned long , STGMEDIUM * );
unsigned char * __stdcall STGMEDIUM_UserMarshal64( unsigned long *, unsigned char *, STGMEDIUM * );
unsigned char * __stdcall STGMEDIUM_UserUnmarshal64(unsigned long *, unsigned char *, STGMEDIUM * );
void __stdcall STGMEDIUM_UserFree64( unsigned long *, STGMEDIUM * );
 HRESULT __stdcall IEnumUnknown_Next_Proxy(
 IEnumUnknown * This,
 ULONG celt,
 IUnknown **rgelt,
 ULONG *pceltFetched);
 HRESULT __stdcall IEnumUnknown_Next_Stub(
 IEnumUnknown * This,
 ULONG celt,
 IUnknown **rgelt,
 ULONG *pceltFetched);
 HRESULT __stdcall IBindCtx_SetBindOptions_Proxy(
 IBindCtx * This,
 BIND_OPTS *pbindopts);
 HRESULT __stdcall IBindCtx_SetBindOptions_Stub(
 IBindCtx * This,
 BIND_OPTS2 *pbindopts);
 HRESULT __stdcall IBindCtx_GetBindOptions_Proxy(
 IBindCtx * This,
 BIND_OPTS *pbindopts);
 HRESULT __stdcall IBindCtx_GetBindOptions_Stub(
 IBindCtx * This,
 BIND_OPTS2 *pbindopts);
 HRESULT __stdcall IEnumMoniker_Next_Proxy(
 IEnumMoniker * This,
 ULONG celt,
 IMoniker **rgelt,
 ULONG *pceltFetched);
 HRESULT __stdcall IEnumMoniker_Next_Stub(
 IEnumMoniker * This,
 ULONG celt,
 IMoniker **rgelt,
 ULONG *pceltFetched);
 BOOL __stdcall IRunnableObject_IsRunning_Proxy(
 IRunnableObject * This);
 HRESULT __stdcall IRunnableObject_IsRunning_Stub(
 IRunnableObject * This);
 HRESULT __stdcall IMoniker_BindToObject_Proxy(
 IMoniker * This,
 IBindCtx *pbc,
 IMoniker *pmkToLeft,
 const IID & riidResult,
 void **ppvResult);
 HRESULT __stdcall IMoniker_BindToObject_Stub(
 IMoniker * This,
 IBindCtx *pbc,
 IMoniker *pmkToLeft,
 const IID & riidResult,
 IUnknown **ppvResult);
 HRESULT __stdcall IMoniker_BindToStorage_Proxy(
 IMoniker * This,
 IBindCtx *pbc,
 IMoniker *pmkToLeft,
 const IID & riid,
 void **ppvObj);
 HRESULT __stdcall IMoniker_BindToStorage_Stub(
 IMoniker * This,
 IBindCtx *pbc,
 IMoniker *pmkToLeft,
 const IID & riid,
 IUnknown **ppvObj);
 HRESULT __stdcall IEnumString_Next_Proxy(
 IEnumString * This,
 ULONG celt,
 LPOLESTR *rgelt,
 ULONG *pceltFetched);
 HRESULT __stdcall IEnumString_Next_Stub(
 IEnumString * This,
 ULONG celt,
 LPOLESTR *rgelt,
 ULONG *pceltFetched);
 HRESULT __stdcall ISequentialStream_Read_Proxy(
 ISequentialStream * This,
 void *pv,
 ULONG cb,
 ULONG *pcbRead);
 HRESULT __stdcall ISequentialStream_Read_Stub(
 ISequentialStream * This,
 byte *pv,
 ULONG cb,
 ULONG *pcbRead);
 HRESULT __stdcall ISequentialStream_Write_Proxy(
 ISequentialStream * This,
 const void *pv,
 ULONG cb,
 ULONG *pcbWritten);
 HRESULT __stdcall ISequentialStream_Write_Stub(
 ISequentialStream * This,
 const byte *pv,
 ULONG cb,
 ULONG *pcbWritten);
 HRESULT __stdcall IStream_Seek_Proxy(
 IStream * This,
 LARGE_INTEGER dlibMove,
 DWORD dwOrigin,
 ULARGE_INTEGER *plibNewPosition);
 HRESULT __stdcall IStream_Seek_Stub(
 IStream * This,
 LARGE_INTEGER dlibMove,
 DWORD dwOrigin,
 ULARGE_INTEGER *plibNewPosition);
 HRESULT __stdcall IStream_CopyTo_Proxy(
 IStream * This,
 IStream *pstm,
 ULARGE_INTEGER cb,
 ULARGE_INTEGER *pcbRead,
 ULARGE_INTEGER *pcbWritten);
 HRESULT __stdcall IStream_CopyTo_Stub(
 IStream * This,
 IStream *pstm,
 ULARGE_INTEGER cb,
 ULARGE_INTEGER *pcbRead,
 ULARGE_INTEGER *pcbWritten);
 HRESULT __stdcall IEnumSTATSTG_Next_Proxy(
 IEnumSTATSTG * This,
 ULONG celt,
 STATSTG *rgelt,
 ULONG *pceltFetched);
 HRESULT __stdcall IEnumSTATSTG_Next_Stub(
 IEnumSTATSTG * This,
 ULONG celt,
 STATSTG *rgelt,
 ULONG *pceltFetched);
 HRESULT __stdcall IStorage_OpenStream_Proxy(
 IStorage * This,
 const OLECHAR *pwcsName,
 void *reserved1,
 DWORD grfMode,
 DWORD reserved2,
 IStream **ppstm);
 HRESULT __stdcall IStorage_OpenStream_Stub(
 IStorage * This,
 const OLECHAR *pwcsName,
 unsigned long cbReserved1,
 byte *reserved1,
 DWORD grfMode,
 DWORD reserved2,
 IStream **ppstm);
 HRESULT __stdcall IStorage_CopyTo_Proxy(
 IStorage * This,
 DWORD ciidExclude,
 const IID *rgiidExclude,
 SNB snbExclude,
 IStorage *pstgDest);
 HRESULT __stdcall IStorage_CopyTo_Stub(
 IStorage * This,
 DWORD ciidExclude,
 const IID *rgiidExclude,
 SNB snbExclude,
 IStorage *pstgDest);
 HRESULT __stdcall IStorage_EnumElements_Proxy(
 IStorage * This,
 DWORD reserved1,
 void *reserved2,
 DWORD reserved3,
 IEnumSTATSTG **ppenum);
 HRESULT __stdcall IStorage_EnumElements_Stub(
 IStorage * This,
 DWORD reserved1,
 unsigned long cbReserved2,
 byte *reserved2,
 DWORD reserved3,
 IEnumSTATSTG **ppenum);
 HRESULT __stdcall ILockBytes_ReadAt_Proxy(
 ILockBytes * This,
 ULARGE_INTEGER ulOffset,
 void *pv,
 ULONG cb,
 ULONG *pcbRead);
 HRESULT __stdcall ILockBytes_ReadAt_Stub(
 ILockBytes * This,
 ULARGE_INTEGER ulOffset,
 byte *pv,
 ULONG cb,
 ULONG *pcbRead);
 HRESULT __stdcall ILockBytes_WriteAt_Proxy(
 ILockBytes * This,
 ULARGE_INTEGER ulOffset,
 const void *pv,
 ULONG cb,
 ULONG *pcbWritten);
 HRESULT __stdcall ILockBytes_WriteAt_Stub(
 ILockBytes * This,
 ULARGE_INTEGER ulOffset,
 const byte *pv,
 ULONG cb,
 ULONG *pcbWritten);
 HRESULT __stdcall IEnumFORMATETC_Next_Proxy(
 IEnumFORMATETC * This,
 ULONG celt,
 FORMATETC *rgelt,
 ULONG *pceltFetched);
 HRESULT __stdcall IEnumFORMATETC_Next_Stub(
 IEnumFORMATETC * This,
 ULONG celt,
 FORMATETC *rgelt,
 ULONG *pceltFetched);
 HRESULT __stdcall IEnumSTATDATA_Next_Proxy(
 IEnumSTATDATA * This,
 ULONG celt,
 STATDATA *rgelt,
 ULONG *pceltFetched);
 HRESULT __stdcall IEnumSTATDATA_Next_Stub(
 IEnumSTATDATA * This,
 ULONG celt,
 STATDATA *rgelt,
 ULONG *pceltFetched);
 void __stdcall IAdviseSink_OnDataChange_Proxy(
 IAdviseSink * This,
 FORMATETC *pFormatetc,
 STGMEDIUM *pStgmed);
 HRESULT __stdcall IAdviseSink_OnDataChange_Stub(
 IAdviseSink * This,
 FORMATETC *pFormatetc,
 ASYNC_STGMEDIUM *pStgmed);
 void __stdcall IAdviseSink_OnViewChange_Proxy(
 IAdviseSink * This,
 DWORD dwAspect,
 LONG lindex);
 HRESULT __stdcall IAdviseSink_OnViewChange_Stub(
 IAdviseSink * This,
 DWORD dwAspect,
 LONG lindex);
 void __stdcall IAdviseSink_OnRename_Proxy(
 IAdviseSink * This,
 IMoniker *pmk);
 HRESULT __stdcall IAdviseSink_OnRename_Stub(
 IAdviseSink * This,
 IMoniker *pmk);
 void __stdcall IAdviseSink_OnSave_Proxy(
 IAdviseSink * This);
 HRESULT __stdcall IAdviseSink_OnSave_Stub(
 IAdviseSink * This);
 void __stdcall IAdviseSink_OnClose_Proxy(
 IAdviseSink * This);
 HRESULT __stdcall IAdviseSink_OnClose_Stub(
 IAdviseSink * This);
 void __stdcall IAdviseSink2_OnLinkSrcChange_Proxy(
 IAdviseSink2 * This,
 IMoniker *pmk);
 HRESULT __stdcall IAdviseSink2_OnLinkSrcChange_Stub(
 IAdviseSink2 * This,
 IMoniker *pmk);
 HRESULT __stdcall IDataObject_GetData_Proxy(
 IDataObject * This,
 FORMATETC *pformatetcIn,
 STGMEDIUM *pmedium);
 HRESULT __stdcall IDataObject_GetData_Stub(
 IDataObject * This,
 FORMATETC *pformatetcIn,
 STGMEDIUM *pRemoteMedium);
 HRESULT __stdcall IDataObject_GetDataHere_Proxy(
 IDataObject * This,
 FORMATETC *pformatetc,
 STGMEDIUM *pmedium);
 HRESULT __stdcall IDataObject_GetDataHere_Stub(
 IDataObject * This,
 FORMATETC *pformatetc,
 STGMEDIUM *pRemoteMedium);
 HRESULT __stdcall IDataObject_SetData_Proxy(
 IDataObject * This,
 FORMATETC *pformatetc,
 STGMEDIUM *pmedium,
 BOOL fRelease);
 HRESULT __stdcall IDataObject_SetData_Stub(
 IDataObject * This,
 FORMATETC *pformatetc,
 FLAG_STGMEDIUM *pmedium,
 BOOL fRelease);
 HRESULT __stdcall IFillLockBytes_FillAppend_Proxy(
 IFillLockBytes * This,
 const void *pv,
 ULONG cb,
 ULONG *pcbWritten);
 HRESULT __stdcall IFillLockBytes_FillAppend_Stub(
 IFillLockBytes * This,
 const byte *pv,
 ULONG cb,
 ULONG *pcbWritten);
 HRESULT __stdcall IFillLockBytes_FillAt_Proxy(
 IFillLockBytes * This,
 ULARGE_INTEGER ulOffset,
 const void *pv,
 ULONG cb,
 ULONG *pcbWritten);
 HRESULT __stdcall IFillLockBytes_FillAt_Stub(
 IFillLockBytes * This,
 ULARGE_INTEGER ulOffset,
 const byte *pv,
 ULONG cb,
 ULONG *pcbWritten);
 void __stdcall AsyncIAdviseSink_Begin_OnDataChange_Proxy(
 AsyncIAdviseSink * This,
 FORMATETC *pFormatetc,
 STGMEDIUM *pStgmed);
 HRESULT __stdcall AsyncIAdviseSink_Begin_OnDataChange_Stub(
 AsyncIAdviseSink * This,
 FORMATETC *pFormatetc,
 ASYNC_STGMEDIUM *pStgmed);
 void __stdcall AsyncIAdviseSink_Finish_OnDataChange_Proxy(
 AsyncIAdviseSink * This);
 HRESULT __stdcall AsyncIAdviseSink_Finish_OnDataChange_Stub(
 AsyncIAdviseSink * This);
 void __stdcall AsyncIAdviseSink_Begin_OnViewChange_Proxy(
 AsyncIAdviseSink * This,
 DWORD dwAspect,
 LONG lindex);
 HRESULT __stdcall AsyncIAdviseSink_Begin_OnViewChange_Stub(
 AsyncIAdviseSink * This,
 DWORD dwAspect,
 LONG lindex);
 void __stdcall AsyncIAdviseSink_Finish_OnViewChange_Proxy(
 AsyncIAdviseSink * This);
 HRESULT __stdcall AsyncIAdviseSink_Finish_OnViewChange_Stub(
 AsyncIAdviseSink * This);
 void __stdcall AsyncIAdviseSink_Begin_OnRename_Proxy(
 AsyncIAdviseSink * This,
 IMoniker *pmk);
 HRESULT __stdcall AsyncIAdviseSink_Begin_OnRename_Stub(
 AsyncIAdviseSink * This,
 IMoniker *pmk);
 void __stdcall AsyncIAdviseSink_Finish_OnRename_Proxy(
 AsyncIAdviseSink * This);
 HRESULT __stdcall AsyncIAdviseSink_Finish_OnRename_Stub(
 AsyncIAdviseSink * This);
 void __stdcall AsyncIAdviseSink_Begin_OnSave_Proxy(
 AsyncIAdviseSink * This);
 HRESULT __stdcall AsyncIAdviseSink_Begin_OnSave_Stub(
 AsyncIAdviseSink * This);
 void __stdcall AsyncIAdviseSink_Finish_OnSave_Proxy(
 AsyncIAdviseSink * This);
 HRESULT __stdcall AsyncIAdviseSink_Finish_OnSave_Stub(
 AsyncIAdviseSink * This);
 void __stdcall AsyncIAdviseSink_Begin_OnClose_Proxy(
 AsyncIAdviseSink * This);
 HRESULT __stdcall AsyncIAdviseSink_Begin_OnClose_Stub(
 AsyncIAdviseSink * This);
 void __stdcall AsyncIAdviseSink_Finish_OnClose_Proxy(
 AsyncIAdviseSink * This);
 HRESULT __stdcall AsyncIAdviseSink_Finish_OnClose_Stub(
 AsyncIAdviseSink * This);
 void __stdcall AsyncIAdviseSink2_Begin_OnLinkSrcChange_Proxy(
 AsyncIAdviseSink2 * This,
 IMoniker *pmk);
 HRESULT __stdcall AsyncIAdviseSink2_Begin_OnLinkSrcChange_Stub(
 AsyncIAdviseSink2 * This,
 IMoniker *pmk);
 void __stdcall AsyncIAdviseSink2_Finish_OnLinkSrcChange_Proxy(
 AsyncIAdviseSink2 * This);
 HRESULT __stdcall AsyncIAdviseSink2_Finish_OnLinkSrcChange_Stub(
 AsyncIAdviseSink2 * This);
}
#pragma once
extern "C" {
extern const IID GUID_NULL;
extern const IID CATID_MARSHALER;
extern const IID IID_IRpcChannel;
extern const IID IID_IRpcStub;
extern const IID IID_IStubManager;
extern const IID IID_IRpcProxy;
extern const IID IID_IProxyManager;
extern const IID IID_IPSFactory;
extern const IID IID_IInternalMoniker;
extern const IID IID_IDfReserved1;
extern const IID IID_IDfReserved2;
extern const IID IID_IDfReserved3;
extern const CLSID CLSID_StdMarshal;
extern const CLSID CLSID_AggStdMarshal;
extern const CLSID CLSID_StdAsyncActManager;
extern const IID IID_IStub;
extern const IID IID_IProxy;
extern const IID IID_IEnumGeneric;
extern const IID IID_IEnumHolder;
extern const IID IID_IEnumCallback;
extern const IID IID_IOleManager;
extern const IID IID_IOlePresObj;
extern const IID IID_IDebug;
extern const IID IID_IDebugStream;
extern const CLSID CLSID_PSGenObject;
extern const CLSID CLSID_PSClientSite;
extern const CLSID CLSID_PSClassObject;
extern const CLSID CLSID_PSInPlaceActive;
extern const CLSID CLSID_PSInPlaceFrame;
extern const CLSID CLSID_PSDragDrop;
extern const CLSID CLSID_PSBindCtx;
extern const CLSID CLSID_PSEnumerators;
extern const CLSID CLSID_StaticMetafile;
extern const CLSID CLSID_StaticDib;
extern const CLSID CID_CDfsVolume;
extern const CLSID CLSID_DCOMAccessControl;
extern const CLSID CLSID_GlobalOptions;
extern const CLSID CLSID_StdGlobalInterfaceTable;
extern const CLSID CLSID_ComBinding;
extern const CLSID CLSID_StdEvent;
extern const CLSID CLSID_ManualResetEvent;
extern const CLSID CLSID_SynchronizeContainer;
extern const CLSID CLSID_AddrControl;
extern const CLSID CLSID_ContextSwitcher;
extern const CLSID CLSID_CCDFormKrnl;
extern const CLSID CLSID_CCDPropertyPage;
extern const CLSID CLSID_CCDFormDialog;
extern const CLSID CLSID_CCDCommandButton;
extern const CLSID CLSID_CCDComboBox;
extern const CLSID CLSID_CCDTextBox;
extern const CLSID CLSID_CCDCheckBox;
extern const CLSID CLSID_CCDLabel;
extern const CLSID CLSID_CCDOptionButton;
extern const CLSID CLSID_CCDListBox;
extern const CLSID CLSID_CCDScrollBar;
extern const CLSID CLSID_CCDGroupBox;
extern const CLSID CLSID_CCDGeneralPropertyPage;
extern const CLSID CLSID_CCDGenericPropertyPage;
extern const CLSID CLSID_CCDFontPropertyPage;
extern const CLSID CLSID_CCDColorPropertyPage;
extern const CLSID CLSID_CCDLabelPropertyPage;
extern const CLSID CLSID_CCDCheckBoxPropertyPage;
extern const CLSID CLSID_CCDTextBoxPropertyPage;
extern const CLSID CLSID_CCDOptionButtonPropertyPage;
extern const CLSID CLSID_CCDListBoxPropertyPage;
extern const CLSID CLSID_CCDCommandButtonPropertyPage;
extern const CLSID CLSID_CCDComboBoxPropertyPage;
extern const CLSID CLSID_CCDScrollBarPropertyPage;
extern const CLSID CLSID_CCDGroupBoxPropertyPage;
extern const CLSID CLSID_CCDXObjectPropertyPage;
extern const CLSID CLSID_CStdPropertyFrame;
extern const CLSID CLSID_CFormPropertyPage;
extern const CLSID CLSID_CGridPropertyPage;
extern const CLSID CLSID_CWSJArticlePage;
extern const CLSID CLSID_CSystemPage;
extern const CLSID CLSID_IdentityUnmarshal;
extern const CLSID CLSID_InProcFreeMarshaler;
extern const CLSID CLSID_Picture_Metafile;
extern const CLSID CLSID_Picture_EnhMetafile;
extern const CLSID CLSID_Picture_Dib;
extern const GUID GUID_TRISTATE;
}
typedef enum tagCOINIT
{
 COINIT_APARTMENTTHREADED = 0x2,
 COINIT_MULTITHREADED = 0x0,
 COINIT_DISABLE_OLE1DDE = 0x4,
 COINIT_SPEED_OVER_MEMORY = 0x8,
} COINIT;
extern "C" __declspec(dllimport) DWORD __stdcall CoBuildVersion( void );
extern "C" __declspec(dllimport) HRESULT __stdcall CoInitialize( LPVOID pvReserved);
extern "C" __declspec(dllimport) void __stdcall CoUninitialize(void);
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetMalloc( DWORD dwMemContext, LPMALLOC * ppMalloc);
extern "C" __declspec(dllimport) DWORD __stdcall CoGetCurrentProcess(void);
extern "C" __declspec(dllimport) HRESULT __stdcall CoRegisterMallocSpy( LPMALLOCSPY pMallocSpy);
extern "C" __declspec(dllimport) HRESULT __stdcall CoRevokeMallocSpy(void);
extern "C" __declspec(dllimport) HRESULT __stdcall CoCreateStandardMalloc( DWORD memctx, IMalloc * * ppMalloc);
extern "C" __declspec(dllimport) HRESULT __stdcall CoInitializeEx( LPVOID pvReserved, DWORD dwCoInit);
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetCallerTID( LPDWORD lpdwTID );
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetCurrentLogicalThreadId( GUID *pguid);
extern "C" __declspec(dllimport) HRESULT __stdcall CoRegisterInitializeSpy( LPINITIALIZESPY pSpy, ULARGE_INTEGER *puliCookie);
extern "C" __declspec(dllimport) HRESULT __stdcall CoRevokeInitializeSpy( ULARGE_INTEGER uliCookie);
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetContextToken( ULONG_PTR* pToken);
typedef enum tagCOMSD
{
 SD_LAUNCHPERMISSIONS = 0,
 SD_ACCESSPERMISSIONS = 1,
 SD_LAUNCHRESTRICTIONS = 2,
 SD_ACCESSRESTRICTIONS = 3
} COMSD;
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetSystemSecurityPermissions(COMSD comSDType, PSECURITY_DESCRIPTOR *ppSD);
typedef struct tagSOleTlsDataPublic
{
 void *pvReserved0[2];
 DWORD dwReserved0[3];
 void *pvReserved1[1];
 DWORD dwReserved1[3];
 void *pvReserved2[4];
 DWORD dwReserved2[1];
 void *pCurrentCtx;
} SOleTlsDataPublic;
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetObjectContext( const IID & riid, LPVOID * ppv);
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetClassObject( const IID & rclsid, DWORD dwClsContext, LPVOID pvReserved,
 const IID & riid, LPVOID * ppv);
extern "C" __declspec(dllimport) HRESULT __stdcall CoRegisterClassObject( const IID & rclsid, LPUNKNOWN pUnk,
 DWORD dwClsContext, DWORD flags, LPDWORD lpdwRegister);
extern "C" __declspec(dllimport) HRESULT __stdcall CoRevokeClassObject( DWORD dwRegister);
extern "C" __declspec(dllimport) HRESULT __stdcall CoResumeClassObjects(void);
extern "C" __declspec(dllimport) HRESULT __stdcall CoSuspendClassObjects(void);
extern "C" __declspec(dllimport) ULONG __stdcall CoAddRefServerProcess(void);
extern "C" __declspec(dllimport) ULONG __stdcall CoReleaseServerProcess(void);
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetPSClsid( const IID & riid, CLSID *pClsid);
extern "C" __declspec(dllimport) HRESULT __stdcall CoRegisterPSClsid( const IID & riid, const IID & rclsid);
extern "C" __declspec(dllimport) HRESULT __stdcall CoRegisterSurrogate( LPSURROGATE pSurrogate);
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetMarshalSizeMax( ULONG *pulSize, const IID & riid, LPUNKNOWN pUnk,
 DWORD dwDestContext, LPVOID pvDestContext, DWORD mshlflags);
extern "C" __declspec(dllimport) HRESULT __stdcall CoMarshalInterface( LPSTREAM pStm, const IID & riid, LPUNKNOWN pUnk,
 DWORD dwDestContext, LPVOID pvDestContext, DWORD mshlflags);
extern "C" __declspec(dllimport) HRESULT __stdcall CoUnmarshalInterface( LPSTREAM pStm, const IID & riid, LPVOID * ppv);
extern "C" __declspec(dllimport) HRESULT __stdcall CoMarshalHresult( LPSTREAM pstm, HRESULT hresult);
extern "C" __declspec(dllimport) HRESULT __stdcall CoUnmarshalHresult( LPSTREAM pstm, HRESULT * phresult);
extern "C" __declspec(dllimport) HRESULT __stdcall CoReleaseMarshalData( LPSTREAM pStm);
extern "C" __declspec(dllimport) HRESULT __stdcall CoDisconnectObject( LPUNKNOWN pUnk, DWORD dwReserved);
extern "C" __declspec(dllimport) HRESULT __stdcall CoLockObjectExternal( LPUNKNOWN pUnk, BOOL fLock, BOOL fLastUnlockReleases);
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetStandardMarshal( const IID & riid, LPUNKNOWN pUnk,
 DWORD dwDestContext, LPVOID pvDestContext, DWORD mshlflags,
 LPMARSHAL * ppMarshal);
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetStdMarshalEx( LPUNKNOWN pUnkOuter, DWORD smexflags,
 LPUNKNOWN * ppUnkInner);
typedef enum tagSTDMSHLFLAGS
{
 SMEXF_SERVER = 0x01,
 SMEXF_HANDLER = 0x02
} STDMSHLFLAGS;
extern "C" __declspec(dllimport) BOOL __stdcall CoIsHandlerConnected( LPUNKNOWN pUnk);
extern "C" __declspec(dllimport) HRESULT __stdcall CoMarshalInterThreadInterfaceInStream( const IID & riid, LPUNKNOWN pUnk,
 LPSTREAM *ppStm);
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetInterfaceAndReleaseStream( LPSTREAM pStm, const IID & iid,
 LPVOID * ppv);
extern "C" __declspec(dllimport) HRESULT __stdcall CoCreateFreeThreadedMarshaler( LPUNKNOWN punkOuter,
 LPUNKNOWN *ppunkMarshal);
extern "C" __declspec(dllimport) HINSTANCE __stdcall CoLoadLibrary( LPOLESTR lpszLibName, BOOL bAutoFree);
extern "C" __declspec(dllimport) void __stdcall CoFreeLibrary( HINSTANCE hInst);
extern "C" __declspec(dllimport) void __stdcall CoFreeAllLibraries(void);
extern "C" __declspec(dllimport) void __stdcall CoFreeUnusedLibraries(void);
extern "C" __declspec(dllimport) void __stdcall CoFreeUnusedLibrariesEx( DWORD dwUnloadDelay, DWORD dwReserved);
extern "C" __declspec(dllimport) HRESULT __stdcall CoDisconnectContext(DWORD dwTimeout);
extern "C" __declspec(dllimport) HRESULT __stdcall CoInitializeSecurity(
 PSECURITY_DESCRIPTOR pSecDesc,
 LONG cAuthSvc,
 SOLE_AUTHENTICATION_SERVICE *asAuthSvc,
 void *pReserved1,
 DWORD dwAuthnLevel,
 DWORD dwImpLevel,
 void *pAuthList,
 DWORD dwCapabilities,
 void *pReserved3 );
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetCallContext( const IID & riid, void **ppInterface );
 extern "C" __declspec(dllimport) HRESULT __stdcall CoQueryProxyBlanket(
 IUnknown *pProxy,
 DWORD *pwAuthnSvc,
 DWORD *pAuthzSvc,
 OLECHAR **pServerPrincName,
 DWORD *pAuthnLevel,
 DWORD *pImpLevel,
 RPC_AUTH_IDENTITY_HANDLE *pAuthInfo,
 DWORD *pCapabilites );
extern "C" __declspec(dllimport) HRESULT __stdcall CoSetProxyBlanket(
 IUnknown *pProxy,
 DWORD dwAuthnSvc,
 DWORD dwAuthzSvc,
 OLECHAR *pServerPrincName,
 DWORD dwAuthnLevel,
 DWORD dwImpLevel,
 RPC_AUTH_IDENTITY_HANDLE pAuthInfo,
 DWORD dwCapabilities );
extern "C" __declspec(dllimport) HRESULT __stdcall CoCopyProxy(
 IUnknown *pProxy,
 IUnknown **ppCopy );
 extern "C" __declspec(dllimport) HRESULT __stdcall CoQueryClientBlanket(
 DWORD *pAuthnSvc,
 DWORD *pAuthzSvc,
 OLECHAR **pServerPrincName,
 DWORD *pAuthnLevel,
 DWORD *pImpLevel,
 RPC_AUTHZ_HANDLE *pPrivs,
 DWORD *pCapabilities );
extern "C" __declspec(dllimport) HRESULT __stdcall CoImpersonateClient(void);
extern "C" __declspec(dllimport) HRESULT __stdcall CoRevertToSelf(void);
extern "C" __declspec(dllimport) HRESULT __stdcall CoQueryAuthenticationServices(
 DWORD *pcAuthSvc,
 SOLE_AUTHENTICATION_SERVICE **asAuthSvc );
extern "C" __declspec(dllimport) HRESULT __stdcall CoSwitchCallContext( IUnknown *pNewObject, IUnknown **ppOldObject );
extern "C" __declspec(dllimport) HRESULT __stdcall CoCreateInstance( const IID & rclsid,
 LPUNKNOWN pUnkOuter,
 DWORD dwClsContext,
 const IID & riid,
 LPVOID * ppv);
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetInstanceFromFile(
 COSERVERINFO * pServerInfo,
 CLSID * pClsid,
 IUnknown * punkOuter,
 DWORD dwClsCtx,
 DWORD grfMode,
 OLECHAR * pwszName,
 DWORD dwCount,
 MULTI_QI * pResults );
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetInstanceFromIStorage(
 COSERVERINFO * pServerInfo,
 CLSID * pClsid,
 IUnknown * punkOuter,
 DWORD dwClsCtx,
 struct IStorage * pstg,
 DWORD dwCount,
 MULTI_QI * pResults );
extern "C" __declspec(dllimport) HRESULT __stdcall CoCreateInstanceEx(
 const IID & Clsid,
 IUnknown * punkOuter,
 DWORD dwClsCtx,
 COSERVERINFO * pServerInfo,
 DWORD dwCount,
 MULTI_QI * pResults );
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetCancelObject( DWORD dwThreadId, const IID & iid, void **ppUnk);
extern "C" __declspec(dllimport) HRESULT __stdcall CoSetCancelObject( IUnknown *pUnk);
extern "C" __declspec(dllimport) HRESULT __stdcall CoCancelCall( DWORD dwThreadId, ULONG ulTimeout);
extern "C" __declspec(dllimport) HRESULT __stdcall CoTestCancel(void);
extern "C" __declspec(dllimport) HRESULT __stdcall CoEnableCallCancellation( LPVOID pReserved);
extern "C" __declspec(dllimport) HRESULT __stdcall CoDisableCallCancellation( LPVOID pReserved);
extern "C" __declspec(dllimport) HRESULT __stdcall CoAllowSetForegroundWindow( IUnknown *pUnk, LPVOID lpvReserved);
extern "C" __declspec(dllimport) HRESULT __stdcall DcomChannelSetHResult( LPVOID pvReserved, ULONG* pulReserved, HRESULT appsHR);
 extern "C" __declspec(dllimport) HRESULT __stdcall StringFromCLSID( const IID & rclsid, LPOLESTR * lplpsz);
extern "C" __declspec(dllimport) HRESULT __stdcall CLSIDFromString( LPOLESTR lpsz, LPCLSID pclsid);
 extern "C" __declspec(dllimport) HRESULT __stdcall StringFromIID( const IID & rclsid, LPOLESTR * lplpsz);
extern "C" __declspec(dllimport) HRESULT __stdcall IIDFromString( LPOLESTR lpsz, LPIID lpiid);
extern "C" __declspec(dllimport) BOOL __stdcall CoIsOle1Class( const IID & rclsid);
 extern "C" __declspec(dllimport) HRESULT __stdcall ProgIDFromCLSID ( const IID & clsid, LPOLESTR * lplpszProgID);
extern "C" __declspec(dllimport) HRESULT __stdcall CLSIDFromProgID ( LPCOLESTR lpszProgID, LPCLSID lpclsid);
extern "C" __declspec(dllimport) HRESULT __stdcall CLSIDFromProgIDEx ( LPCOLESTR lpszProgID, LPCLSID lpclsid);
 extern "C" __declspec(dllimport) int __stdcall StringFromGUID2( const GUID & rguid, LPOLESTR lpsz, int cchMax);
extern "C" __declspec(dllimport) HRESULT __stdcall CoCreateGuid( GUID *pguid);
extern "C" __declspec(dllimport) BOOL __stdcall CoFileTimeToDosDateTime(
 FILETIME * lpFileTime, LPWORD lpDosDate, LPWORD lpDosTime);
extern "C" __declspec(dllimport) BOOL __stdcall CoDosDateTimeToFileTime(
 WORD nDosDate, WORD nDosTime, FILETIME * lpFileTime);
extern "C" __declspec(dllimport) HRESULT __stdcall CoFileTimeNow( FILETIME * lpFileTime );
extern "C" __declspec(dllimport) HRESULT __stdcall CoRegisterMessageFilter( LPMESSAGEFILTER lpMessageFilter,
 LPMESSAGEFILTER * lplpMessageFilter );
extern "C" __declspec(dllimport) HRESULT __stdcall CoRegisterChannelHook( const GUID & ExtensionUuid, IChannelHook *pChannelHook );
extern "C" __declspec(dllimport) HRESULT __stdcall CoWaitForMultipleHandles ( DWORD dwFlags,
 DWORD dwTimeout,
 ULONG cHandles,
 LPHANDLE pHandles,
 LPDWORD lpdwindex);
typedef enum tagCOWAIT_FLAGS
{
 COWAIT_WAITALL = 1,
 COWAIT_ALERTABLE = 2,
 COWAIT_INPUTAVAILABLE = 4
}COWAIT_FLAGS;
extern "C" __declspec(dllimport) HRESULT __stdcall CoInvalidateRemoteMachineBindings( LPOLESTR pszMachineName);
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetTreatAsClass( const IID & clsidOld, LPCLSID pClsidNew);
extern "C" __declspec(dllimport) HRESULT __stdcall CoTreatAsClass( const IID & clsidOld, const IID & clsidNew);
typedef HRESULT (__stdcall * LPFNGETCLASSOBJECT) (const IID &, const IID &, LPVOID *);
typedef HRESULT (__stdcall * LPFNCANUNLOADNOW)(void);
extern "C" HRESULT __stdcall DllGetClassObject( const IID & rclsid, const IID & riid, LPVOID * ppv);
extern "C" HRESULT __stdcall DllCanUnloadNow(void);
extern "C" __declspec(dllimport) LPVOID __stdcall CoTaskMemAlloc( SIZE_T cb);
extern "C" __declspec(dllimport) LPVOID __stdcall CoTaskMemRealloc( LPVOID pv, SIZE_T cb);
extern "C" __declspec(dllimport) void __stdcall CoTaskMemFree( LPVOID pv);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateDataAdviseHolder( LPDATAADVISEHOLDER * ppDAHolder);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateDataCache( LPUNKNOWN pUnkOuter, const IID & rclsid,
 const IID & iid, LPVOID * ppv);
extern "C" __declspec(dllimport) HRESULT __stdcall StgCreateDocfile( const WCHAR* pwcsName,
 DWORD grfMode,
 DWORD reserved,
 IStorage** ppstgOpen);
extern "C" __declspec(dllimport) HRESULT __stdcall StgCreateDocfileOnILockBytes( ILockBytes* plkbyt,
 DWORD grfMode,
 DWORD reserved,
 IStorage** ppstgOpen);
extern "C" __declspec(dllimport) HRESULT __stdcall StgOpenStorage( const WCHAR* pwcsName,
 IStorage* pstgPriority,
 DWORD grfMode,
 SNB snbExclude,
 DWORD reserved,
 IStorage** ppstgOpen);
extern "C" __declspec(dllimport) HRESULT __stdcall StgOpenStorageOnILockBytes( ILockBytes* plkbyt,
 IStorage* pstgPriority,
 DWORD grfMode,
 SNB snbExclude,
 DWORD reserved,
 IStorage** ppstgOpen);
extern "C" __declspec(dllimport) HRESULT __stdcall StgIsStorageFile( const WCHAR* pwcsName);
extern "C" __declspec(dllimport) HRESULT __stdcall StgIsStorageILockBytes( ILockBytes* plkbyt);
extern "C" __declspec(dllimport) HRESULT __stdcall StgSetTimes( const WCHAR* lpszName,
 const FILETIME* pctime,
 const FILETIME* patime,
 const FILETIME* pmtime);
extern "C" __declspec(dllimport) HRESULT __stdcall StgOpenAsyncDocfileOnIFillLockBytes( IFillLockBytes *pflb,
 DWORD grfMode,
 DWORD asyncFlags,
 IStorage** ppstgOpen);
extern "C" __declspec(dllimport) HRESULT __stdcall StgGetIFillLockBytesOnILockBytes( ILockBytes *pilb,
 IFillLockBytes** ppflb);
extern "C" __declspec(dllimport) HRESULT __stdcall StgGetIFillLockBytesOnFile( OLECHAR const *pwcsName,
 IFillLockBytes** ppflb);
extern "C" __declspec(dllimport) HRESULT __stdcall StgOpenLayoutDocfile( OLECHAR const *pwcsDfName,
 DWORD grfMode,
 DWORD reserved,
 IStorage** ppstgOpen);
typedef struct tagSTGOPTIONS
{
 USHORT usVersion;
 USHORT reserved;
 ULONG ulSectorSize;
 const WCHAR *pwcsTemplateFile;
} STGOPTIONS;
extern "C" __declspec(dllimport) HRESULT __stdcall StgCreateStorageEx ( const WCHAR* pwcsName,
 DWORD grfMode,
 DWORD stgfmt,
 DWORD grfAttrs,
 STGOPTIONS* pStgOptions,
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 const IID & riid,
 void** ppObjectOpen);
extern "C" __declspec(dllimport) HRESULT __stdcall StgOpenStorageEx ( const WCHAR* pwcsName,
 DWORD grfMode,
 DWORD stgfmt,
 DWORD grfAttrs,
 STGOPTIONS* pStgOptions,
 PSECURITY_DESCRIPTOR pSecurityDescriptor,
 const IID & riid,
 void** ppObjectOpen);
extern "C" __declspec(dllimport) HRESULT __stdcall BindMoniker( LPMONIKER pmk, DWORD grfOpt, const IID & iidResult, LPVOID * ppvResult);
extern "C" __declspec(dllimport) HRESULT __stdcall CoInstall(
 IBindCtx * pbc,
 DWORD dwFlags,
 uCLSSPEC * pClassSpec,
 QUERYCONTEXT * pQuery,
 LPWSTR pszCodeBase);
extern "C" __declspec(dllimport) HRESULT __stdcall CoGetObject( LPCWSTR pszName, BIND_OPTS *pBindOptions, const IID & riid, void **ppv);
extern "C" __declspec(dllimport) HRESULT __stdcall MkParseDisplayName( LPBC pbc, LPCOLESTR szUserName,
 ULONG * pchEaten, LPMONIKER * ppmk);
extern "C" __declspec(dllimport) HRESULT __stdcall MonikerRelativePathTo( LPMONIKER pmkSrc, LPMONIKER pmkDest, LPMONIKER
 * ppmkRelPath, BOOL dwReserved);
extern "C" __declspec(dllimport) HRESULT __stdcall MonikerCommonPrefixWith( LPMONIKER pmkThis, LPMONIKER pmkOther,
 LPMONIKER * ppmkCommon);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateBindCtx( DWORD reserved, LPBC * ppbc);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateGenericComposite( LPMONIKER pmkFirst, LPMONIKER pmkRest,
 LPMONIKER * ppmkComposite);
extern "C" __declspec(dllimport) HRESULT __stdcall GetClassFile ( LPCOLESTR szFilename, CLSID * pclsid);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateClassMoniker( const IID & rclsid, LPMONIKER * ppmk);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateFileMoniker( LPCOLESTR lpszPathName, LPMONIKER * ppmk);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateItemMoniker( LPCOLESTR lpszDelim, LPCOLESTR lpszItem,
 LPMONIKER * ppmk);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateAntiMoniker( LPMONIKER * ppmk);
extern "C" __declspec(dllimport) HRESULT __stdcall CreatePointerMoniker( LPUNKNOWN punk, LPMONIKER * ppmk);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateObjrefMoniker( LPUNKNOWN punk, LPMONIKER * ppmk);
extern "C" __declspec(dllimport) HRESULT __stdcall GetRunningObjectTable( DWORD reserved, LPRUNNINGOBJECTTABLE * pprot);
#pragma warning( disable: 4049 )
#pragma once
typedef struct IPersistMoniker IPersistMoniker;
typedef struct IMonikerProp IMonikerProp;
typedef struct IBindProtocol IBindProtocol;
typedef struct IBinding IBinding;
typedef struct IBindStatusCallback IBindStatusCallback;
typedef struct IAuthenticate IAuthenticate;
typedef struct IHttpNegotiate IHttpNegotiate;
typedef struct IHttpNegotiate2 IHttpNegotiate2;
typedef struct IWinInetFileStream IWinInetFileStream;
typedef struct IWindowForBindingUI IWindowForBindingUI;
typedef struct ICodeInstall ICodeInstall;
typedef struct IUri IUri;
typedef struct IUriContainer IUriContainer;
typedef struct IUriBuilder IUriBuilder;
typedef struct IUriBuilderFactory IUriBuilderFactory;
typedef struct IWinInetInfo IWinInetInfo;
typedef struct IHttpSecurity IHttpSecurity;
typedef struct IWinInetHttpInfo IWinInetHttpInfo;
typedef struct IWinInetCacheHints IWinInetCacheHints;
typedef struct IWinInetCacheHints2 IWinInetCacheHints2;
typedef struct IBindHost IBindHost;
typedef struct IInternet IInternet;
typedef struct IInternetBindInfo IInternetBindInfo;
typedef struct IInternetProtocolRoot IInternetProtocolRoot;
typedef struct IInternetProtocol IInternetProtocol;
typedef struct IInternetProtocolEx IInternetProtocolEx;
typedef struct IInternetProtocolSink IInternetProtocolSink;
typedef struct IInternetProtocolSinkStackable IInternetProtocolSinkStackable;
typedef struct IInternetSession IInternetSession;
typedef struct IInternetThreadSwitch IInternetThreadSwitch;
typedef struct IInternetPriority IInternetPriority;
typedef struct IInternetProtocolInfo IInternetProtocolInfo;
typedef struct IInternetSecurityMgrSite IInternetSecurityMgrSite;
typedef struct IInternetSecurityManager IInternetSecurityManager;
typedef struct IInternetSecurityManagerEx IInternetSecurityManagerEx;
typedef struct IInternetSecurityManagerEx2 IInternetSecurityManagerEx2;
typedef struct IZoneIdentifier IZoneIdentifier;
typedef struct IInternetHostSecurityManager IInternetHostSecurityManager;
typedef struct IInternetZoneManager IInternetZoneManager;
typedef struct IInternetZoneManagerEx IInternetZoneManagerEx;
typedef struct IInternetZoneManagerEx2 IInternetZoneManagerEx2;
typedef struct ISoftDistExt ISoftDistExt;
typedef struct ICatalogFileInfo ICatalogFileInfo;
typedef struct IDataFilter IDataFilter;
typedef struct IEncodingFilterFactory IEncodingFilterFactory;
typedef struct IWrappedProtocol IWrappedProtocol;
#pragma warning( disable: 4049 )
#pragma once
typedef struct IOleAdviseHolder IOleAdviseHolder;
typedef struct IOleCache IOleCache;
typedef struct IOleCache2 IOleCache2;
typedef struct IOleCacheControl IOleCacheControl;
typedef struct IParseDisplayName IParseDisplayName;
typedef struct IOleContainer IOleContainer;
typedef struct IOleClientSite IOleClientSite;
typedef struct IOleObject IOleObject;
typedef struct IOleWindow IOleWindow;
typedef struct IOleLink IOleLink;
typedef struct IOleItemContainer IOleItemContainer;
typedef struct IOleInPlaceUIWindow IOleInPlaceUIWindow;
typedef struct IOleInPlaceActiveObject IOleInPlaceActiveObject;
typedef struct IOleInPlaceFrame IOleInPlaceFrame;
typedef struct IOleInPlaceObject IOleInPlaceObject;
typedef struct IOleInPlaceSite IOleInPlaceSite;
typedef struct IContinue IContinue;
typedef struct IViewObject IViewObject;
typedef struct IViewObject2 IViewObject2;
typedef struct IDropSource IDropSource;
typedef struct IDropTarget IDropTarget;
typedef struct IDropSourceNotify IDropSourceNotify;
typedef struct IEnumOLEVERB IEnumOLEVERB;
extern "C"{
#pragma once
extern RPC_IF_HANDLE __MIDL_itf_oleidl_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_oleidl_0000_0000_v0_0_s_ifspec;
typedef IOleAdviseHolder *LPOLEADVISEHOLDER;
extern "C" const IID IID_IOleAdviseHolder;
 struct __declspec(uuid("00000111-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleAdviseHolder : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Advise(
 IAdviseSink *pAdvise,
 DWORD *pdwConnection) = 0;
 virtual HRESULT __stdcall Unadvise(
 DWORD dwConnection) = 0;
 virtual HRESULT __stdcall EnumAdvise(
 IEnumSTATDATA **ppenumAdvise) = 0;
 virtual HRESULT __stdcall SendOnRename(
 IMoniker *pmk) = 0;
 virtual HRESULT __stdcall SendOnSave( void) = 0;
 virtual HRESULT __stdcall SendOnClose( void) = 0;
 };
typedef IOleCache *LPOLECACHE;
extern "C" const IID IID_IOleCache;
 struct __declspec(uuid("0000011e-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleCache : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Cache(
 FORMATETC *pformatetc,
 DWORD advf,
 DWORD *pdwConnection) = 0;
 virtual HRESULT __stdcall Uncache(
 DWORD dwConnection) = 0;
 virtual HRESULT __stdcall EnumCache(
 IEnumSTATDATA **ppenumSTATDATA) = 0;
 virtual HRESULT __stdcall InitCache(
 IDataObject *pDataObject) = 0;
 virtual HRESULT __stdcall SetData(
 FORMATETC *pformatetc,
 STGMEDIUM *pmedium,
 BOOL fRelease) = 0;
 };
typedef IOleCache2 *LPOLECACHE2;
typedef
enum tagDISCARDCACHE
 { DISCARDCACHE_SAVEIFDIRTY = 0,
 DISCARDCACHE_NOSAVE = 1
 } DISCARDCACHE;
extern "C" const IID IID_IOleCache2;
 struct __declspec(uuid("00000128-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleCache2 : public IOleCache
 {
 public:
 virtual HRESULT __stdcall UpdateCache(
 LPDATAOBJECT pDataObject,
 DWORD grfUpdf,
 LPVOID pReserved) = 0;
 virtual HRESULT __stdcall DiscardCache(
 DWORD dwDiscardOptions) = 0;
 };
 HRESULT __stdcall IOleCache2_RemoteUpdateCache_Proxy(
 IOleCache2 * This,
 LPDATAOBJECT pDataObject,
 DWORD grfUpdf,
 LONG_PTR pReserved);
void __stdcall IOleCache2_RemoteUpdateCache_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IOleCacheControl *LPOLECACHECONTROL;
extern "C" const IID IID_IOleCacheControl;
 struct __declspec(uuid("00000129-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleCacheControl : public IUnknown
 {
 public:
 virtual HRESULT __stdcall OnRun(
 LPDATAOBJECT pDataObject) = 0;
 virtual HRESULT __stdcall OnStop( void) = 0;
 };
typedef IParseDisplayName *LPPARSEDISPLAYNAME;
extern "C" const IID IID_IParseDisplayName;
 struct __declspec(uuid("0000011a-0000-0000-C000-000000000046")) __declspec(novtable)
 IParseDisplayName : public IUnknown
 {
 public:
 virtual HRESULT __stdcall ParseDisplayName(
 IBindCtx *pbc,
 LPOLESTR pszDisplayName,
 ULONG *pchEaten,
 IMoniker **ppmkOut) = 0;
 };
typedef IOleContainer *LPOLECONTAINER;
extern "C" const IID IID_IOleContainer;
 struct __declspec(uuid("0000011b-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleContainer : public IParseDisplayName
 {
 public:
 virtual HRESULT __stdcall EnumObjects(
 DWORD grfFlags,
 IEnumUnknown **ppenum) = 0;
 virtual HRESULT __stdcall LockContainer(
 BOOL fLock) = 0;
 };
typedef IOleClientSite *LPOLECLIENTSITE;
extern "C" const IID IID_IOleClientSite;
 struct __declspec(uuid("00000118-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleClientSite : public IUnknown
 {
 public:
 virtual HRESULT __stdcall SaveObject( void) = 0;
 virtual HRESULT __stdcall GetMoniker(
 DWORD dwAssign,
 DWORD dwWhichMoniker,
 IMoniker **ppmk) = 0;
 virtual HRESULT __stdcall GetContainer(
 IOleContainer **ppContainer) = 0;
 virtual HRESULT __stdcall ShowObject( void) = 0;
 virtual HRESULT __stdcall OnShowWindow(
 BOOL fShow) = 0;
 virtual HRESULT __stdcall RequestNewObjectLayout( void) = 0;
 };
typedef IOleObject *LPOLEOBJECT;
typedef
enum tagOLEGETMONIKER
 { OLEGETMONIKER_ONLYIFTHERE = 1,
 OLEGETMONIKER_FORCEASSIGN = 2,
 OLEGETMONIKER_UNASSIGN = 3,
 OLEGETMONIKER_TEMPFORUSER = 4
 } OLEGETMONIKER;
typedef
enum tagOLEWHICHMK
 { OLEWHICHMK_CONTAINER = 1,
 OLEWHICHMK_OBJREL = 2,
 OLEWHICHMK_OBJFULL = 3
 } OLEWHICHMK;
typedef
enum tagUSERCLASSTYPE
 { USERCLASSTYPE_FULL = 1,
 USERCLASSTYPE_SHORT = 2,
 USERCLASSTYPE_APPNAME = 3
 } USERCLASSTYPE;
typedef
enum tagOLEMISC
 { OLEMISC_RECOMPOSEONRESIZE = 0x1,
 OLEMISC_ONLYICONIC = 0x2,
 OLEMISC_INSERTNOTREPLACE = 0x4,
 OLEMISC_STATIC = 0x8,
 OLEMISC_CANTLINKINSIDE = 0x10,
 OLEMISC_CANLINKBYOLE1 = 0x20,
 OLEMISC_ISLINKOBJECT = 0x40,
 OLEMISC_INSIDEOUT = 0x80,
 OLEMISC_ACTIVATEWHENVISIBLE = 0x100,
 OLEMISC_RENDERINGISDEVICEINDEPENDENT = 0x200,
 OLEMISC_INVISIBLEATRUNTIME = 0x400,
 OLEMISC_ALWAYSRUN = 0x800,
 OLEMISC_ACTSLIKEBUTTON = 0x1000,
 OLEMISC_ACTSLIKELABEL = 0x2000,
 OLEMISC_NOUIACTIVATE = 0x4000,
 OLEMISC_ALIGNABLE = 0x8000,
 OLEMISC_SIMPLEFRAME = 0x10000,
 OLEMISC_SETCLIENTSITEFIRST = 0x20000,
 OLEMISC_IMEMODE = 0x40000,
 OLEMISC_IGNOREACTIVATEWHENVISIBLE = 0x80000,
 OLEMISC_WANTSTOMENUMERGE = 0x100000,
 OLEMISC_SUPPORTSMULTILEVELUNDO = 0x200000
 } OLEMISC;
typedef
enum tagOLECLOSE
 { OLECLOSE_SAVEIFDIRTY = 0,
 OLECLOSE_NOSAVE = 1,
 OLECLOSE_PROMPTSAVE = 2
 } OLECLOSE;
extern "C" const IID IID_IOleObject;
 struct __declspec(uuid("00000112-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleObject : public IUnknown
 {
 public:
 virtual HRESULT __stdcall SetClientSite(
 IOleClientSite *pClientSite) = 0;
 virtual HRESULT __stdcall GetClientSite(
 IOleClientSite **ppClientSite) = 0;
 virtual HRESULT __stdcall SetHostNames(
 LPCOLESTR szContainerApp,
 LPCOLESTR szContainerObj) = 0;
 virtual HRESULT __stdcall Close(
 DWORD dwSaveOption) = 0;
 virtual HRESULT __stdcall SetMoniker(
 DWORD dwWhichMoniker,
 IMoniker *pmk) = 0;
 virtual HRESULT __stdcall GetMoniker(
 DWORD dwAssign,
 DWORD dwWhichMoniker,
 IMoniker **ppmk) = 0;
 virtual HRESULT __stdcall InitFromData(
 IDataObject *pDataObject,
 BOOL fCreation,
 DWORD dwReserved) = 0;
 virtual HRESULT __stdcall GetClipboardData(
 DWORD dwReserved,
 IDataObject **ppDataObject) = 0;
 virtual HRESULT __stdcall DoVerb(
 LONG iVerb,
 LPMSG lpmsg,
 IOleClientSite *pActiveSite,
 LONG lindex,
 HWND hwndParent,
 LPCRECT lprcPosRect) = 0;
 virtual HRESULT __stdcall EnumVerbs(
 IEnumOLEVERB **ppEnumOleVerb) = 0;
 virtual HRESULT __stdcall Update( void) = 0;
 virtual HRESULT __stdcall IsUpToDate( void) = 0;
 virtual HRESULT __stdcall GetUserClassID(
 CLSID *pClsid) = 0;
 virtual HRESULT __stdcall GetUserType(
 DWORD dwFormOfType,
 LPOLESTR *pszUserType) = 0;
 virtual HRESULT __stdcall SetExtent(
 DWORD dwDrawAspect,
 SIZEL *psizel) = 0;
 virtual HRESULT __stdcall GetExtent(
 DWORD dwDrawAspect,
 SIZEL *psizel) = 0;
 virtual HRESULT __stdcall Advise(
 IAdviseSink *pAdvSink,
 DWORD *pdwConnection) = 0;
 virtual HRESULT __stdcall Unadvise(
 DWORD dwConnection) = 0;
 virtual HRESULT __stdcall EnumAdvise(
 IEnumSTATDATA **ppenumAdvise) = 0;
 virtual HRESULT __stdcall GetMiscStatus(
 DWORD dwAspect,
 DWORD *pdwStatus) = 0;
 virtual HRESULT __stdcall SetColorScheme(
 LOGPALETTE *pLogpal) = 0;
 };
typedef
enum tagOLERENDER
 { OLERENDER_NONE = 0,
 OLERENDER_DRAW = 1,
 OLERENDER_FORMAT = 2,
 OLERENDER_ASIS = 3
 } OLERENDER;
typedef OLERENDER *LPOLERENDER;
typedef struct tagOBJECTDESCRIPTOR
 {
 ULONG cbSize;
 CLSID clsid;
 DWORD dwDrawAspect;
 SIZEL sizel;
 POINTL pointl;
 DWORD dwStatus;
 DWORD dwFullUserTypeName;
 DWORD dwSrcOfCopy;
 } OBJECTDESCRIPTOR;
typedef struct tagOBJECTDESCRIPTOR *POBJECTDESCRIPTOR;
typedef struct tagOBJECTDESCRIPTOR *LPOBJECTDESCRIPTOR;
typedef struct tagOBJECTDESCRIPTOR LINKSRCDESCRIPTOR;
typedef struct tagOBJECTDESCRIPTOR *PLINKSRCDESCRIPTOR;
typedef struct tagOBJECTDESCRIPTOR *LPLINKSRCDESCRIPTOR;
extern RPC_IF_HANDLE IOLETypes_v0_0_c_ifspec;
extern RPC_IF_HANDLE IOLETypes_v0_0_s_ifspec;
typedef IOleWindow *LPOLEWINDOW;
extern "C" const IID IID_IOleWindow;
 struct __declspec(uuid("00000114-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleWindow : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetWindow(
 HWND *phwnd) = 0;
 virtual HRESULT __stdcall ContextSensitiveHelp(
 BOOL fEnterMode) = 0;
 };
typedef IOleLink *LPOLELINK;
typedef
enum tagOLEUPDATE
 { OLEUPDATE_ALWAYS = 1,
 OLEUPDATE_ONCALL = 3
 } OLEUPDATE;
typedef OLEUPDATE *LPOLEUPDATE;
typedef OLEUPDATE *POLEUPDATE;
typedef
enum tagOLELINKBIND
 { OLELINKBIND_EVENIFCLASSDIFF = 1
 } OLELINKBIND;
extern "C" const IID IID_IOleLink;
 struct __declspec(uuid("0000011d-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleLink : public IUnknown
 {
 public:
 virtual HRESULT __stdcall SetUpdateOptions(
 DWORD dwUpdateOpt) = 0;
 virtual HRESULT __stdcall GetUpdateOptions(
 DWORD *pdwUpdateOpt) = 0;
 virtual HRESULT __stdcall SetSourceMoniker(
 IMoniker *pmk,
 const IID & rclsid) = 0;
 virtual HRESULT __stdcall GetSourceMoniker(
 IMoniker **ppmk) = 0;
 virtual HRESULT __stdcall SetSourceDisplayName(
 LPCOLESTR pszStatusText) = 0;
 virtual HRESULT __stdcall GetSourceDisplayName(
 LPOLESTR *ppszDisplayName) = 0;
 virtual HRESULT __stdcall BindToSource(
 DWORD bindflags,
 IBindCtx *pbc) = 0;
 virtual HRESULT __stdcall BindIfRunning( void) = 0;
 virtual HRESULT __stdcall GetBoundSource(
 IUnknown **ppunk) = 0;
 virtual HRESULT __stdcall UnbindSource( void) = 0;
 virtual HRESULT __stdcall Update(
 IBindCtx *pbc) = 0;
 };
typedef IOleItemContainer *LPOLEITEMCONTAINER;
typedef
enum tagBINDSPEED
 { BINDSPEED_INDEFINITE = 1,
 BINDSPEED_MODERATE = 2,
 BINDSPEED_IMMEDIATE = 3
 } BINDSPEED;
typedef
enum tagOLECONTF
 { OLECONTF_EMBEDDINGS = 1,
 OLECONTF_LINKS = 2,
 OLECONTF_OTHERS = 4,
 OLECONTF_ONLYUSER = 8,
 OLECONTF_ONLYIFRUNNING = 16
 } OLECONTF;
extern "C" const IID IID_IOleItemContainer;
 struct __declspec(uuid("0000011c-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleItemContainer : public IOleContainer
 {
 public:
 virtual HRESULT __stdcall GetObjectA(
 LPOLESTR pszItem,
 DWORD dwSpeedNeeded,
 IBindCtx *pbc,
 const IID & riid,
 void **ppvObject) = 0;
 virtual HRESULT __stdcall GetObjectStorage(
 LPOLESTR pszItem,
 IBindCtx *pbc,
 const IID & riid,
 void **ppvStorage) = 0;
 virtual HRESULT __stdcall IsRunning(
 LPOLESTR pszItem) = 0;
 };
typedef IOleInPlaceUIWindow *LPOLEINPLACEUIWINDOW;
typedef RECT BORDERWIDTHS;
typedef LPRECT LPBORDERWIDTHS;
typedef LPCRECT LPCBORDERWIDTHS;
extern "C" const IID IID_IOleInPlaceUIWindow;
 struct __declspec(uuid("00000115-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleInPlaceUIWindow : public IOleWindow
 {
 public:
 virtual HRESULT __stdcall GetBorder(
 LPRECT lprectBorder) = 0;
 virtual HRESULT __stdcall RequestBorderSpace(
 LPCBORDERWIDTHS pborderwidths) = 0;
 virtual HRESULT __stdcall SetBorderSpace(
 LPCBORDERWIDTHS pborderwidths) = 0;
 virtual HRESULT __stdcall SetActiveObject(
 IOleInPlaceActiveObject *pActiveObject,
 LPCOLESTR pszObjName) = 0;
 };
typedef IOleInPlaceActiveObject *LPOLEINPLACEACTIVEOBJECT;
extern "C" const IID IID_IOleInPlaceActiveObject;
 struct __declspec(uuid("00000117-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleInPlaceActiveObject : public IOleWindow
 {
 public:
 virtual HRESULT __stdcall TranslateAcceleratorA(
 LPMSG lpmsg) = 0;
 virtual HRESULT __stdcall OnFrameWindowActivate(
 BOOL fActivate) = 0;
 virtual HRESULT __stdcall OnDocWindowActivate(
 BOOL fActivate) = 0;
 virtual HRESULT __stdcall ResizeBorder(
 LPCRECT prcBorder,
 IOleInPlaceUIWindow *pUIWindow,
 BOOL fFrameWindow) = 0;
 virtual HRESULT __stdcall EnableModeless(
 BOOL fEnable) = 0;
 };
 HRESULT __stdcall IOleInPlaceActiveObject_RemoteTranslateAccelerator_Proxy(
 IOleInPlaceActiveObject * This);
void __stdcall IOleInPlaceActiveObject_RemoteTranslateAccelerator_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IOleInPlaceActiveObject_RemoteResizeBorder_Proxy(
 IOleInPlaceActiveObject * This,
 LPCRECT prcBorder,
 const IID & riid,
 IOleInPlaceUIWindow *pUIWindow,
 BOOL fFrameWindow);
void __stdcall IOleInPlaceActiveObject_RemoteResizeBorder_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IOleInPlaceFrame *LPOLEINPLACEFRAME;
typedef struct tagOIFI
 {
 UINT cb;
 BOOL fMDIApp;
 HWND hwndFrame;
 HACCEL haccel;
 UINT cAccelEntries;
 } OLEINPLACEFRAMEINFO;
typedef struct tagOIFI *LPOLEINPLACEFRAMEINFO;
typedef struct tagOleMenuGroupWidths
 {
 LONG width[ 6 ];
 } OLEMENUGROUPWIDTHS;
typedef struct tagOleMenuGroupWidths *LPOLEMENUGROUPWIDTHS;
typedef HGLOBAL HOLEMENU;
extern "C" const IID IID_IOleInPlaceFrame;
 struct __declspec(uuid("00000116-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleInPlaceFrame : public IOleInPlaceUIWindow
 {
 public:
 virtual HRESULT __stdcall InsertMenus(
 HMENU hmenuShared,
 LPOLEMENUGROUPWIDTHS lpMenuWidths) = 0;
 virtual HRESULT __stdcall SetMenu(
 HMENU hmenuShared,
 HOLEMENU holemenu,
 HWND hwndActiveObject) = 0;
 virtual HRESULT __stdcall RemoveMenus(
 HMENU hmenuShared) = 0;
 virtual HRESULT __stdcall SetStatusText(
 LPCOLESTR pszStatusText) = 0;
 virtual HRESULT __stdcall EnableModeless(
 BOOL fEnable) = 0;
 virtual HRESULT __stdcall TranslateAcceleratorA(
 LPMSG lpmsg,
 WORD wID) = 0;
 };
typedef IOleInPlaceObject *LPOLEINPLACEOBJECT;
extern "C" const IID IID_IOleInPlaceObject;
 struct __declspec(uuid("00000113-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleInPlaceObject : public IOleWindow
 {
 public:
 virtual HRESULT __stdcall InPlaceDeactivate( void) = 0;
 virtual HRESULT __stdcall UIDeactivate( void) = 0;
 virtual HRESULT __stdcall SetObjectRects(
 LPCRECT lprcPosRect,
 LPCRECT lprcClipRect) = 0;
 virtual HRESULT __stdcall ReactivateAndUndo( void) = 0;
 };
typedef IOleInPlaceSite *LPOLEINPLACESITE;
extern "C" const IID IID_IOleInPlaceSite;
 struct __declspec(uuid("00000119-0000-0000-C000-000000000046")) __declspec(novtable)
 IOleInPlaceSite : public IOleWindow
 {
 public:
 virtual HRESULT __stdcall CanInPlaceActivate( void) = 0;
 virtual HRESULT __stdcall OnInPlaceActivate( void) = 0;
 virtual HRESULT __stdcall OnUIActivate( void) = 0;
 virtual HRESULT __stdcall GetWindowContext(
 IOleInPlaceFrame **ppFrame,
 IOleInPlaceUIWindow **ppDoc,
 LPRECT lprcPosRect,
 LPRECT lprcClipRect,
 LPOLEINPLACEFRAMEINFO lpFrameInfo) = 0;
 virtual HRESULT __stdcall Scroll(
 SIZE scrollExtant) = 0;
 virtual HRESULT __stdcall OnUIDeactivate(
 BOOL fUndoable) = 0;
 virtual HRESULT __stdcall OnInPlaceDeactivate( void) = 0;
 virtual HRESULT __stdcall DiscardUndoState( void) = 0;
 virtual HRESULT __stdcall DeactivateAndUndo( void) = 0;
 virtual HRESULT __stdcall OnPosRectChange(
 LPCRECT lprcPosRect) = 0;
 };
extern "C" const IID IID_IContinue;
 struct __declspec(uuid("0000012a-0000-0000-C000-000000000046")) __declspec(novtable)
 IContinue : public IUnknown
 {
 public:
 virtual HRESULT __stdcall FContinue( void) = 0;
 };
typedef IViewObject *LPVIEWOBJECT;
extern "C" const IID IID_IViewObject;
 struct __declspec(uuid("0000010d-0000-0000-C000-000000000046")) __declspec(novtable)
 IViewObject : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Draw(
 DWORD dwDrawAspect,
 LONG lindex,
 void *pvAspect,
 DVTARGETDEVICE *ptd,
 HDC hdcTargetDev,
 HDC hdcDraw,
 LPCRECTL lprcBounds,
 LPCRECTL lprcWBounds,
 BOOL ( __stdcall *pfnContinue )(
 ULONG_PTR dwContinue),
 ULONG_PTR dwContinue) = 0;
 virtual HRESULT __stdcall GetColorSet(
 DWORD dwDrawAspect,
 LONG lindex,
 void *pvAspect,
 DVTARGETDEVICE *ptd,
 HDC hicTargetDev,
 LOGPALETTE **ppColorSet) = 0;
 virtual HRESULT __stdcall Freeze(
 DWORD dwDrawAspect,
 LONG lindex,
 void *pvAspect,
 DWORD *pdwFreeze) = 0;
 virtual HRESULT __stdcall Unfreeze(
 DWORD dwFreeze) = 0;
 virtual HRESULT __stdcall SetAdvise(
 DWORD aspects,
 DWORD advf,
 IAdviseSink *pAdvSink) = 0;
 virtual HRESULT __stdcall GetAdvise(
 DWORD *pAspects,
 DWORD *pAdvf,
 IAdviseSink **ppAdvSink) = 0;
 };
 HRESULT __stdcall IViewObject_RemoteDraw_Proxy(
 IViewObject * This,
 DWORD dwDrawAspect,
 LONG lindex,
 ULONG_PTR pvAspect,
 DVTARGETDEVICE *ptd,
 HDC hdcTargetDev,
 HDC hdcDraw,
 LPCRECTL lprcBounds,
 LPCRECTL lprcWBounds,
 IContinue *pContinue);
void __stdcall IViewObject_RemoteDraw_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IViewObject_RemoteGetColorSet_Proxy(
 IViewObject * This,
 DWORD dwDrawAspect,
 LONG lindex,
 ULONG_PTR pvAspect,
 DVTARGETDEVICE *ptd,
 ULONG_PTR hicTargetDev,
 LOGPALETTE **ppColorSet);
void __stdcall IViewObject_RemoteGetColorSet_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IViewObject_RemoteFreeze_Proxy(
 IViewObject * This,
 DWORD dwDrawAspect,
 LONG lindex,
 ULONG_PTR pvAspect,
 DWORD *pdwFreeze);
void __stdcall IViewObject_RemoteFreeze_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IViewObject_RemoteGetAdvise_Proxy(
 IViewObject * This,
 DWORD *pAspects,
 DWORD *pAdvf,
 IAdviseSink **ppAdvSink);
void __stdcall IViewObject_RemoteGetAdvise_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IViewObject2 *LPVIEWOBJECT2;
extern "C" const IID IID_IViewObject2;
 struct __declspec(uuid("00000127-0000-0000-C000-000000000046")) __declspec(novtable)
 IViewObject2 : public IViewObject
 {
 public:
 virtual HRESULT __stdcall GetExtent(
 DWORD dwDrawAspect,
 LONG lindex,
 DVTARGETDEVICE *ptd,
 LPSIZEL lpsizel) = 0;
 };
typedef IDropSource *LPDROPSOURCE;
extern "C" const IID IID_IDropSource;
 struct __declspec(uuid("00000121-0000-0000-C000-000000000046")) __declspec(novtable)
 IDropSource : public IUnknown
 {
 public:
 virtual HRESULT __stdcall QueryContinueDrag(
 BOOL fEscapePressed,
 DWORD grfKeyState) = 0;
 virtual HRESULT __stdcall GiveFeedback(
 DWORD dwEffect) = 0;
 };
typedef IDropTarget *LPDROPTARGET;
extern "C" const IID IID_IDropTarget;
 struct __declspec(uuid("00000122-0000-0000-C000-000000000046")) __declspec(novtable)
 IDropTarget : public IUnknown
 {
 public:
 virtual HRESULT __stdcall DragEnter(
 IDataObject *pDataObj,
 DWORD grfKeyState,
 POINTL pt,
 DWORD *pdwEffect) = 0;
 virtual HRESULT __stdcall DragOver(
 DWORD grfKeyState,
 POINTL pt,
 DWORD *pdwEffect) = 0;
 virtual HRESULT __stdcall DragLeave( void) = 0;
 virtual HRESULT __stdcall Drop(
 IDataObject *pDataObj,
 DWORD grfKeyState,
 POINTL pt,
 DWORD *pdwEffect) = 0;
 };
extern "C" const IID IID_IDropSourceNotify;
 struct __declspec(uuid("0000012B-0000-0000-C000-000000000046")) __declspec(novtable)
 IDropSourceNotify : public IUnknown
 {
 public:
 virtual HRESULT __stdcall DragEnterTarget(
 HWND hwndTarget) = 0;
 virtual HRESULT __stdcall DragLeaveTarget( void) = 0;
 };
typedef IEnumOLEVERB *LPENUMOLEVERB;
typedef struct tagOLEVERB
 {
 LONG lVerb;
 LPOLESTR lpszVerbName;
 DWORD fuFlags;
 DWORD grfAttribs;
 } OLEVERB;
typedef struct tagOLEVERB *LPOLEVERB;
typedef
enum tagOLEVERBATTRIB
 { OLEVERBATTRIB_NEVERDIRTIES = 1,
 OLEVERBATTRIB_ONCONTAINERMENU = 2
 } OLEVERBATTRIB;
extern "C" const IID IID_IEnumOLEVERB;
 struct __declspec(uuid("00000104-0000-0000-C000-000000000046")) __declspec(novtable)
 IEnumOLEVERB : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Next(
 ULONG celt,
 LPOLEVERB rgelt,
 ULONG *pceltFetched) = 0;
 virtual HRESULT __stdcall Skip(
 ULONG celt) = 0;
 virtual HRESULT __stdcall Reset( void) = 0;
 virtual HRESULT __stdcall Clone(
 IEnumOLEVERB **ppenum) = 0;
 };
 HRESULT __stdcall IEnumOLEVERB_RemoteNext_Proxy(
 IEnumOLEVERB * This,
 ULONG celt,
 LPOLEVERB rgelt,
 ULONG *pceltFetched);
void __stdcall IEnumOLEVERB_RemoteNext_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
unsigned long __stdcall CLIPFORMAT_UserSize( unsigned long *, unsigned long , CLIPFORMAT * );
unsigned char * __stdcall CLIPFORMAT_UserMarshal( unsigned long *, unsigned char *, CLIPFORMAT * );
unsigned char * __stdcall CLIPFORMAT_UserUnmarshal(unsigned long *, unsigned char *, CLIPFORMAT * );
void __stdcall CLIPFORMAT_UserFree( unsigned long *, CLIPFORMAT * );
unsigned long __stdcall HACCEL_UserSize( unsigned long *, unsigned long , HACCEL * );
unsigned char * __stdcall HACCEL_UserMarshal( unsigned long *, unsigned char *, HACCEL * );
unsigned char * __stdcall HACCEL_UserUnmarshal(unsigned long *, unsigned char *, HACCEL * );
void __stdcall HACCEL_UserFree( unsigned long *, HACCEL * );
unsigned long __stdcall HDC_UserSize( unsigned long *, unsigned long , HDC * );
unsigned char * __stdcall HDC_UserMarshal( unsigned long *, unsigned char *, HDC * );
unsigned char * __stdcall HDC_UserUnmarshal(unsigned long *, unsigned char *, HDC * );
void __stdcall HDC_UserFree( unsigned long *, HDC * );
unsigned long __stdcall HGLOBAL_UserSize( unsigned long *, unsigned long , HGLOBAL * );
unsigned char * __stdcall HGLOBAL_UserMarshal( unsigned long *, unsigned char *, HGLOBAL * );
unsigned char * __stdcall HGLOBAL_UserUnmarshal(unsigned long *, unsigned char *, HGLOBAL * );
void __stdcall HGLOBAL_UserFree( unsigned long *, HGLOBAL * );
unsigned long __stdcall HMENU_UserSize( unsigned long *, unsigned long , HMENU * );
unsigned char * __stdcall HMENU_UserMarshal( unsigned long *, unsigned char *, HMENU * );
unsigned char * __stdcall HMENU_UserUnmarshal(unsigned long *, unsigned char *, HMENU * );
void __stdcall HMENU_UserFree( unsigned long *, HMENU * );
unsigned long __stdcall HWND_UserSize( unsigned long *, unsigned long , HWND * );
unsigned char * __stdcall HWND_UserMarshal( unsigned long *, unsigned char *, HWND * );
unsigned char * __stdcall HWND_UserUnmarshal(unsigned long *, unsigned char *, HWND * );
void __stdcall HWND_UserFree( unsigned long *, HWND * );
unsigned long __stdcall STGMEDIUM_UserSize( unsigned long *, unsigned long , STGMEDIUM * );
unsigned char * __stdcall STGMEDIUM_UserMarshal( unsigned long *, unsigned char *, STGMEDIUM * );
unsigned char * __stdcall STGMEDIUM_UserUnmarshal(unsigned long *, unsigned char *, STGMEDIUM * );
void __stdcall STGMEDIUM_UserFree( unsigned long *, STGMEDIUM * );
unsigned long __stdcall CLIPFORMAT_UserSize64( unsigned long *, unsigned long , CLIPFORMAT * );
unsigned char * __stdcall CLIPFORMAT_UserMarshal64( unsigned long *, unsigned char *, CLIPFORMAT * );
unsigned char * __stdcall CLIPFORMAT_UserUnmarshal64(unsigned long *, unsigned char *, CLIPFORMAT * );
void __stdcall CLIPFORMAT_UserFree64( unsigned long *, CLIPFORMAT * );
unsigned long __stdcall HACCEL_UserSize64( unsigned long *, unsigned long , HACCEL * );
unsigned char * __stdcall HACCEL_UserMarshal64( unsigned long *, unsigned char *, HACCEL * );
unsigned char * __stdcall HACCEL_UserUnmarshal64(unsigned long *, unsigned char *, HACCEL * );
void __stdcall HACCEL_UserFree64( unsigned long *, HACCEL * );
unsigned long __stdcall HDC_UserSize64( unsigned long *, unsigned long , HDC * );
unsigned char * __stdcall HDC_UserMarshal64( unsigned long *, unsigned char *, HDC * );
unsigned char * __stdcall HDC_UserUnmarshal64(unsigned long *, unsigned char *, HDC * );
void __stdcall HDC_UserFree64( unsigned long *, HDC * );
unsigned long __stdcall HGLOBAL_UserSize64( unsigned long *, unsigned long , HGLOBAL * );
unsigned char * __stdcall HGLOBAL_UserMarshal64( unsigned long *, unsigned char *, HGLOBAL * );
unsigned char * __stdcall HGLOBAL_UserUnmarshal64(unsigned long *, unsigned char *, HGLOBAL * );
void __stdcall HGLOBAL_UserFree64( unsigned long *, HGLOBAL * );
unsigned long __stdcall HMENU_UserSize64( unsigned long *, unsigned long , HMENU * );
unsigned char * __stdcall HMENU_UserMarshal64( unsigned long *, unsigned char *, HMENU * );
unsigned char * __stdcall HMENU_UserUnmarshal64(unsigned long *, unsigned char *, HMENU * );
void __stdcall HMENU_UserFree64( unsigned long *, HMENU * );
unsigned long __stdcall HWND_UserSize64( unsigned long *, unsigned long , HWND * );
unsigned char * __stdcall HWND_UserMarshal64( unsigned long *, unsigned char *, HWND * );
unsigned char * __stdcall HWND_UserUnmarshal64(unsigned long *, unsigned char *, HWND * );
void __stdcall HWND_UserFree64( unsigned long *, HWND * );
unsigned long __stdcall STGMEDIUM_UserSize64( unsigned long *, unsigned long , STGMEDIUM * );
unsigned char * __stdcall STGMEDIUM_UserMarshal64( unsigned long *, unsigned char *, STGMEDIUM * );
unsigned char * __stdcall STGMEDIUM_UserUnmarshal64(unsigned long *, unsigned char *, STGMEDIUM * );
void __stdcall STGMEDIUM_UserFree64( unsigned long *, STGMEDIUM * );
 HRESULT __stdcall IOleCache2_UpdateCache_Proxy(
 IOleCache2 * This,
 LPDATAOBJECT pDataObject,
 DWORD grfUpdf,
 LPVOID pReserved);
 HRESULT __stdcall IOleCache2_UpdateCache_Stub(
 IOleCache2 * This,
 LPDATAOBJECT pDataObject,
 DWORD grfUpdf,
 LONG_PTR pReserved);
 HRESULT __stdcall IOleInPlaceActiveObject_TranslateAccelerator_Proxy(
 IOleInPlaceActiveObject * This,
 LPMSG lpmsg);
 HRESULT __stdcall IOleInPlaceActiveObject_TranslateAccelerator_Stub(
 IOleInPlaceActiveObject * This);
 HRESULT __stdcall IOleInPlaceActiveObject_ResizeBorder_Proxy(
 IOleInPlaceActiveObject * This,
 LPCRECT prcBorder,
 IOleInPlaceUIWindow *pUIWindow,
 BOOL fFrameWindow);
 HRESULT __stdcall IOleInPlaceActiveObject_ResizeBorder_Stub(
 IOleInPlaceActiveObject * This,
 LPCRECT prcBorder,
 const IID & riid,
 IOleInPlaceUIWindow *pUIWindow,
 BOOL fFrameWindow);
 HRESULT __stdcall IViewObject_Draw_Proxy(
 IViewObject * This,
 DWORD dwDrawAspect,
 LONG lindex,
 void *pvAspect,
 DVTARGETDEVICE *ptd,
 HDC hdcTargetDev,
 HDC hdcDraw,
 LPCRECTL lprcBounds,
 LPCRECTL lprcWBounds,
 BOOL ( __stdcall *pfnContinue )(
 ULONG_PTR dwContinue),
 ULONG_PTR dwContinue);
 HRESULT __stdcall IViewObject_Draw_Stub(
 IViewObject * This,
 DWORD dwDrawAspect,
 LONG lindex,
 ULONG_PTR pvAspect,
 DVTARGETDEVICE *ptd,
 HDC hdcTargetDev,
 HDC hdcDraw,
 LPCRECTL lprcBounds,
 LPCRECTL lprcWBounds,
 IContinue *pContinue);
 HRESULT __stdcall IViewObject_GetColorSet_Proxy(
 IViewObject * This,
 DWORD dwDrawAspect,
 LONG lindex,
 void *pvAspect,
 DVTARGETDEVICE *ptd,
 HDC hicTargetDev,
 LOGPALETTE **ppColorSet);
 HRESULT __stdcall IViewObject_GetColorSet_Stub(
 IViewObject * This,
 DWORD dwDrawAspect,
 LONG lindex,
 ULONG_PTR pvAspect,
 DVTARGETDEVICE *ptd,
 ULONG_PTR hicTargetDev,
 LOGPALETTE **ppColorSet);
 HRESULT __stdcall IViewObject_Freeze_Proxy(
 IViewObject * This,
 DWORD dwDrawAspect,
 LONG lindex,
 void *pvAspect,
 DWORD *pdwFreeze);
 HRESULT __stdcall IViewObject_Freeze_Stub(
 IViewObject * This,
 DWORD dwDrawAspect,
 LONG lindex,
 ULONG_PTR pvAspect,
 DWORD *pdwFreeze);
 HRESULT __stdcall IViewObject_GetAdvise_Proxy(
 IViewObject * This,
 DWORD *pAspects,
 DWORD *pAdvf,
 IAdviseSink **ppAdvSink);
 HRESULT __stdcall IViewObject_GetAdvise_Stub(
 IViewObject * This,
 DWORD *pAspects,
 DWORD *pAdvf,
 IAdviseSink **ppAdvSink);
 HRESULT __stdcall IEnumOLEVERB_Next_Proxy(
 IEnumOLEVERB * This,
 ULONG celt,
 LPOLEVERB rgelt,
 ULONG *pceltFetched);
 HRESULT __stdcall IEnumOLEVERB_Next_Stub(
 IEnumOLEVERB * This,
 ULONG celt,
 LPOLEVERB rgelt,
 ULONG *pceltFetched);
}
#pragma warning( disable: 4049 )
#pragma once
typedef struct IServiceProvider IServiceProvider;
extern "C"{
#pragma comment(lib,"uuid.lib")
extern RPC_IF_HANDLE __MIDL_itf_servprov_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_servprov_0000_0000_v0_0_s_ifspec;
typedef IServiceProvider *LPSERVICEPROVIDER;
 extern "C" const IID IID_IServiceProvider;
 extern "C++"
 {
 struct __declspec(uuid("6d5140c1-7436-11ce-8034-00aa006009fa")) __declspec(novtable)
 IServiceProvider : public IUnknown
 {
 public:
 virtual HRESULT __stdcall QueryService(
 const GUID & guidService,
 const IID & riid,
 void * *ppvObject) = 0;
 template <class Q>
 HRESULT __stdcall QueryService(const GUID & guidService, Q** pp)
 {
 return QueryService(guidService, __uuidof(Q), (void **)pp);
 }
 };
 }
 HRESULT __stdcall IServiceProvider_RemoteQueryService_Proxy(
 IServiceProvider * This,
 const GUID & guidService,
 const IID & riid,
 IUnknown * *ppvObject);
 void __stdcall IServiceProvider_RemoteQueryService_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
extern RPC_IF_HANDLE __MIDL_itf_servprov_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_servprov_0000_0001_v0_0_s_ifspec;
 HRESULT __stdcall IServiceProvider_QueryService_Proxy(
 IServiceProvider * This,
 const GUID & guidService,
 const IID & riid,
 void **ppvObject);
 HRESULT __stdcall IServiceProvider_QueryService_Stub(
 IServiceProvider * This,
 const GUID & guidService,
 const IID & riid,
 IUnknown **ppvObject);
}
#pragma warning( disable: 4049 )
#pragma once
typedef struct IXMLDOMImplementation IXMLDOMImplementation;
typedef struct IXMLDOMNode IXMLDOMNode;
typedef struct IXMLDOMDocumentFragment IXMLDOMDocumentFragment;
typedef struct IXMLDOMDocument IXMLDOMDocument;
typedef struct IXMLDOMNodeList IXMLDOMNodeList;
typedef struct IXMLDOMNamedNodeMap IXMLDOMNamedNodeMap;
typedef struct IXMLDOMCharacterData IXMLDOMCharacterData;
typedef struct IXMLDOMAttribute IXMLDOMAttribute;
typedef struct IXMLDOMElement IXMLDOMElement;
typedef struct IXMLDOMText IXMLDOMText;
typedef struct IXMLDOMComment IXMLDOMComment;
typedef struct IXMLDOMProcessingInstruction IXMLDOMProcessingInstruction;
typedef struct IXMLDOMCDATASection IXMLDOMCDATASection;
typedef struct IXMLDOMDocumentType IXMLDOMDocumentType;
typedef struct IXMLDOMNotation IXMLDOMNotation;
typedef struct IXMLDOMEntity IXMLDOMEntity;
typedef struct IXMLDOMEntityReference IXMLDOMEntityReference;
typedef struct IXMLDOMParseError IXMLDOMParseError;
typedef struct IXTLRuntime IXTLRuntime;
typedef struct XMLDOMDocumentEvents XMLDOMDocumentEvents;
typedef class DOMDocument DOMDocument;
typedef class DOMFreeThreadedDocument DOMFreeThreadedDocument;
typedef struct IXMLHttpRequest IXMLHttpRequest;
typedef class XMLHTTPRequest XMLHTTPRequest;
typedef struct IXMLDSOControl IXMLDSOControl;
typedef class XMLDSOControl XMLDSOControl;
typedef struct IXMLElementCollection IXMLElementCollection;
typedef struct IXMLDocument IXMLDocument;
typedef struct IXMLDocument2 IXMLDocument2;
typedef struct IXMLElement IXMLElement;
typedef struct IXMLElement2 IXMLElement2;
typedef struct IXMLAttribute IXMLAttribute;
typedef struct IXMLError IXMLError;
typedef class XMLDocument XMLDocument;
#pragma warning( disable: 4049 )
#pragma once
typedef struct ICreateTypeInfo ICreateTypeInfo;
typedef struct ICreateTypeInfo2 ICreateTypeInfo2;
typedef struct ICreateTypeLib ICreateTypeLib;
typedef struct ICreateTypeLib2 ICreateTypeLib2;
typedef struct IDispatch IDispatch;
typedef struct IEnumVARIANT IEnumVARIANT;
typedef struct ITypeComp ITypeComp;
typedef struct ITypeInfo ITypeInfo;
typedef struct ITypeInfo2 ITypeInfo2;
typedef struct ITypeLib ITypeLib;
typedef struct ITypeLib2 ITypeLib2;
typedef struct ITypeChangeEvents ITypeChangeEvents;
typedef struct IErrorInfo IErrorInfo;
typedef struct ICreateErrorInfo ICreateErrorInfo;
typedef struct ISupportErrorInfo ISupportErrorInfo;
typedef struct ITypeFactory ITypeFactory;
typedef struct ITypeMarshal ITypeMarshal;
typedef struct IRecordInfo IRecordInfo;
typedef struct IErrorLog IErrorLog;
typedef struct IPropertyBag IPropertyBag;
extern "C"{
#pragma warning(push)
#pragma warning(disable:4201)
#pragma once
extern RPC_IF_HANDLE __MIDL_itf_oaidl_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_oaidl_0000_0000_v0_0_s_ifspec;
typedef CY CURRENCY;
typedef struct tagSAFEARRAYBOUND
 {
 ULONG cElements;
 LONG lLbound;
 } SAFEARRAYBOUND;
typedef struct tagSAFEARRAYBOUND *LPSAFEARRAYBOUND;
typedef struct _wireVARIANT *wireVARIANT;
typedef struct _wireBRECORD *wireBRECORD;
typedef struct _wireSAFEARR_BSTR
 {
 ULONG Size;
 wireBSTR *aBstr;
 } SAFEARR_BSTR;
typedef struct _wireSAFEARR_UNKNOWN
 {
 ULONG Size;
 IUnknown **apUnknown;
 } SAFEARR_UNKNOWN;
typedef struct _wireSAFEARR_DISPATCH
 {
 ULONG Size;
 IDispatch **apDispatch;
 } SAFEARR_DISPATCH;
typedef struct _wireSAFEARR_VARIANT
 {
 ULONG Size;
 wireVARIANT *aVariant;
 } SAFEARR_VARIANT;
typedef struct _wireSAFEARR_BRECORD
 {
 ULONG Size;
 wireBRECORD *aRecord;
 } SAFEARR_BRECORD;
typedef struct _wireSAFEARR_HAVEIID
 {
 ULONG Size;
 IUnknown **apUnknown;
 IID iid;
 } SAFEARR_HAVEIID;
typedef
enum tagSF_TYPE
 { SF_ERROR = VT_ERROR,
 SF_I1 = VT_I1,
 SF_I2 = VT_I2,
 SF_I4 = VT_I4,
 SF_I8 = VT_I8,
 SF_BSTR = VT_BSTR,
 SF_UNKNOWN = VT_UNKNOWN,
 SF_DISPATCH = VT_DISPATCH,
 SF_VARIANT = VT_VARIANT,
 SF_RECORD = VT_RECORD,
 SF_HAVEIID = ( VT_UNKNOWN | VT_RESERVED )
 } SF_TYPE;
typedef struct _wireSAFEARRAY_UNION
 {
 ULONG sfType;
 union __MIDL_IOleAutomationTypes_0001
 {
 SAFEARR_BSTR BstrStr;
 SAFEARR_UNKNOWN UnknownStr;
 SAFEARR_DISPATCH DispatchStr;
 SAFEARR_VARIANT VariantStr;
 SAFEARR_BRECORD RecordStr;
 SAFEARR_HAVEIID HaveIidStr;
 BYTE_SIZEDARR ByteStr;
 WORD_SIZEDARR WordStr;
 DWORD_SIZEDARR LongStr;
 HYPER_SIZEDARR HyperStr;
 } u;
 } SAFEARRAYUNION;
typedef struct _wireSAFEARRAY
 {
 USHORT cDims;
 USHORT fFeatures;
 ULONG cbElements;
 ULONG cLocks;
 SAFEARRAYUNION uArrayStructs;
 SAFEARRAYBOUND rgsabound[ 1 ];
 } *wireSAFEARRAY;
typedef wireSAFEARRAY *wirePSAFEARRAY;
typedef struct tagSAFEARRAY
 {
 USHORT cDims;
 USHORT fFeatures;
 ULONG cbElements;
 ULONG cLocks;
 PVOID pvData;
 SAFEARRAYBOUND rgsabound[ 1 ];
 } SAFEARRAY;
typedef SAFEARRAY *LPSAFEARRAY;
typedef struct tagVARIANT VARIANT;
struct tagVARIANT
 {
 union
 {
 struct
 {
 VARTYPE vt;
 WORD wReserved1;
 WORD wReserved2;
 WORD wReserved3;
 union
 {
 LONGLONG llVal;
 LONG lVal;
 BYTE bVal;
 SHORT iVal;
 FLOAT fltVal;
 DOUBLE dblVal;
 VARIANT_BOOL boolVal;
 SCODE scode;
 CY cyVal;
 DATE date;
 BSTR bstrVal;
 IUnknown *punkVal;
 IDispatch *pdispVal;
 SAFEARRAY *parray;
 BYTE *pbVal;
 SHORT *piVal;
 LONG *plVal;
 LONGLONG *pllVal;
 FLOAT *pfltVal;
 DOUBLE *pdblVal;
 VARIANT_BOOL *pboolVal;
 SCODE *pscode;
 CY *pcyVal;
 DATE *pdate;
 BSTR *pbstrVal;
 IUnknown **ppunkVal;
 IDispatch **ppdispVal;
 SAFEARRAY **pparray;
 VARIANT *pvarVal;
 PVOID byref;
 CHAR cVal;
 USHORT uiVal;
 ULONG ulVal;
 ULONGLONG ullVal;
 INT intVal;
 UINT uintVal;
 DECIMAL *pdecVal;
 CHAR *pcVal;
 USHORT *puiVal;
 ULONG *pulVal;
 ULONGLONG *pullVal;
 INT *pintVal;
 UINT *puintVal;
 struct
 {
 PVOID pvRecord;
 IRecordInfo *pRecInfo;
 } ;
 } ;
 } ;
 DECIMAL decVal;
 } ;
 } ;
typedef VARIANT *LPVARIANT;
typedef VARIANT VARIANTARG;
typedef VARIANT *LPVARIANTARG;
struct _wireBRECORD
 {
 ULONG fFlags;
 ULONG clSize;
 IRecordInfo *pRecInfo;
 byte *pRecord;
 } ;
struct _wireVARIANT
 {
 DWORD clSize;
 DWORD rpcReserved;
 USHORT vt;
 USHORT wReserved1;
 USHORT wReserved2;
 USHORT wReserved3;
 union
 {
 LONGLONG llVal;
 LONG lVal;
 BYTE bVal;
 SHORT iVal;
 FLOAT fltVal;
 DOUBLE dblVal;
 VARIANT_BOOL boolVal;
 SCODE scode;
 CY cyVal;
 DATE date;
 wireBSTR bstrVal;
 IUnknown *punkVal;
 IDispatch *pdispVal;
 wirePSAFEARRAY parray;
 wireBRECORD brecVal;
 BYTE *pbVal;
 SHORT *piVal;
 LONG *plVal;
 LONGLONG *pllVal;
 FLOAT *pfltVal;
 DOUBLE *pdblVal;
 VARIANT_BOOL *pboolVal;
 SCODE *pscode;
 CY *pcyVal;
 DATE *pdate;
 wireBSTR *pbstrVal;
 IUnknown **ppunkVal;
 IDispatch **ppdispVal;
 wirePSAFEARRAY *pparray;
 wireVARIANT *pvarVal;
 CHAR cVal;
 USHORT uiVal;
 ULONG ulVal;
 ULONGLONG ullVal;
 INT intVal;
 UINT uintVal;
 DECIMAL decVal;
 DECIMAL *pdecVal;
 CHAR *pcVal;
 USHORT *puiVal;
 ULONG *pulVal;
 ULONGLONG *pullVal;
 INT *pintVal;
 UINT *puintVal;
 } ;
 } ;
typedef LONG DISPID;
typedef DISPID MEMBERID;
typedef DWORD HREFTYPE;
typedef
enum tagTYPEKIND
 { TKIND_ENUM = 0,
 TKIND_RECORD = ( TKIND_ENUM + 1 ) ,
 TKIND_MODULE = ( TKIND_RECORD + 1 ) ,
 TKIND_INTERFACE = ( TKIND_MODULE + 1 ) ,
 TKIND_DISPATCH = ( TKIND_INTERFACE + 1 ) ,
 TKIND_COCLASS = ( TKIND_DISPATCH + 1 ) ,
 TKIND_ALIAS = ( TKIND_COCLASS + 1 ) ,
 TKIND_UNION = ( TKIND_ALIAS + 1 ) ,
 TKIND_MAX = ( TKIND_UNION + 1 )
 } TYPEKIND;
typedef struct tagTYPEDESC
 {
 union
 {
 struct tagTYPEDESC *lptdesc;
 struct tagARRAYDESC *lpadesc;
 HREFTYPE hreftype;
 } ;
 VARTYPE vt;
 } TYPEDESC;
typedef struct tagARRAYDESC
 {
 TYPEDESC tdescElem;
 USHORT cDims;
 SAFEARRAYBOUND rgbounds[ 1 ];
 } ARRAYDESC;
typedef struct tagPARAMDESCEX
 {
 ULONG cBytes;
 VARIANTARG varDefaultValue;
 } PARAMDESCEX;
typedef struct tagPARAMDESCEX *LPPARAMDESCEX;
typedef struct tagPARAMDESC
 {
 LPPARAMDESCEX pparamdescex;
 USHORT wParamFlags;
 } PARAMDESC;
typedef struct tagPARAMDESC *LPPARAMDESC;
typedef struct tagIDLDESC
 {
 ULONG_PTR dwReserved;
 USHORT wIDLFlags;
 } IDLDESC;
typedef struct tagIDLDESC *LPIDLDESC;
typedef struct tagELEMDESC {
 TYPEDESC tdesc;
 union {
 IDLDESC idldesc;
 PARAMDESC paramdesc;
 };
} ELEMDESC, * LPELEMDESC;
typedef struct tagTYPEATTR
 {
 GUID guid;
 LCID lcid;
 DWORD dwReserved;
 MEMBERID memidConstructor;
 MEMBERID memidDestructor;
 LPOLESTR lpstrSchema;
 ULONG cbSizeInstance;
 TYPEKIND typekind;
 WORD cFuncs;
 WORD cVars;
 WORD cImplTypes;
 WORD cbSizeVft;
 WORD cbAlignment;
 WORD wTypeFlags;
 WORD wMajorVerNum;
 WORD wMinorVerNum;
 TYPEDESC tdescAlias;
 IDLDESC idldescType;
 } TYPEATTR;
typedef struct tagTYPEATTR *LPTYPEATTR;
typedef struct tagDISPPARAMS
 {
 VARIANTARG *rgvarg;
 DISPID *rgdispidNamedArgs;
 UINT cArgs;
 UINT cNamedArgs;
 } DISPPARAMS;
typedef struct tagEXCEPINFO {
 WORD wCode;
 WORD wReserved;
 BSTR bstrSource;
 BSTR bstrDescription;
 BSTR bstrHelpFile;
 DWORD dwHelpContext;
 PVOID pvReserved;
 HRESULT (__stdcall *pfnDeferredFillIn)(struct tagEXCEPINFO *);
 SCODE scode;
} EXCEPINFO, * LPEXCEPINFO;
typedef
enum tagCALLCONV
 { CC_FASTCALL = 0,
 CC_CDECL = 1,
 CC_MSCPASCAL = ( CC_CDECL + 1 ) ,
 CC_PASCAL = CC_MSCPASCAL,
 CC_MACPASCAL = ( CC_PASCAL + 1 ) ,
 CC_STDCALL = ( CC_MACPASCAL + 1 ) ,
 CC_FPFASTCALL = ( CC_STDCALL + 1 ) ,
 CC_SYSCALL = ( CC_FPFASTCALL + 1 ) ,
 CC_MPWCDECL = ( CC_SYSCALL + 1 ) ,
 CC_MPWPASCAL = ( CC_MPWCDECL + 1 ) ,
 CC_MAX = ( CC_MPWPASCAL + 1 )
 } CALLCONV;
typedef
enum tagFUNCKIND
 { FUNC_VIRTUAL = 0,
 FUNC_PUREVIRTUAL = ( FUNC_VIRTUAL + 1 ) ,
 FUNC_NONVIRTUAL = ( FUNC_PUREVIRTUAL + 1 ) ,
 FUNC_STATIC = ( FUNC_NONVIRTUAL + 1 ) ,
 FUNC_DISPATCH = ( FUNC_STATIC + 1 )
 } FUNCKIND;
typedef
enum tagINVOKEKIND
 { INVOKE_FUNC = 1,
 INVOKE_PROPERTYGET = 2,
 INVOKE_PROPERTYPUT = 4,
 INVOKE_PROPERTYPUTREF = 8
 } INVOKEKIND;
typedef struct tagFUNCDESC
 {
 MEMBERID memid;
 SCODE *lprgscode;
 ELEMDESC *lprgelemdescParam;
 FUNCKIND funckind;
 INVOKEKIND invkind;
 CALLCONV callconv;
 SHORT cParams;
 SHORT cParamsOpt;
 SHORT oVft;
 SHORT cScodes;
 ELEMDESC elemdescFunc;
 WORD wFuncFlags;
 } FUNCDESC;
typedef struct tagFUNCDESC *LPFUNCDESC;
typedef
enum tagVARKIND
 { VAR_PERINSTANCE = 0,
 VAR_STATIC = ( VAR_PERINSTANCE + 1 ) ,
 VAR_CONST = ( VAR_STATIC + 1 ) ,
 VAR_DISPATCH = ( VAR_CONST + 1 )
 } VARKIND;
typedef struct tagVARDESC
 {
 MEMBERID memid;
 LPOLESTR lpstrSchema;
 union
 {
 ULONG oInst;
 VARIANT *lpvarValue;
 } ;
 ELEMDESC elemdescVar;
 WORD wVarFlags;
 VARKIND varkind;
 } VARDESC;
typedef struct tagVARDESC *LPVARDESC;
typedef
enum tagTYPEFLAGS
 { TYPEFLAG_FAPPOBJECT = 0x1,
 TYPEFLAG_FCANCREATE = 0x2,
 TYPEFLAG_FLICENSED = 0x4,
 TYPEFLAG_FPREDECLID = 0x8,
 TYPEFLAG_FHIDDEN = 0x10,
 TYPEFLAG_FCONTROL = 0x20,
 TYPEFLAG_FDUAL = 0x40,
 TYPEFLAG_FNONEXTENSIBLE = 0x80,
 TYPEFLAG_FOLEAUTOMATION = 0x100,
 TYPEFLAG_FRESTRICTED = 0x200,
 TYPEFLAG_FAGGREGATABLE = 0x400,
 TYPEFLAG_FREPLACEABLE = 0x800,
 TYPEFLAG_FDISPATCHABLE = 0x1000,
 TYPEFLAG_FREVERSEBIND = 0x2000,
 TYPEFLAG_FPROXY = 0x4000
 } TYPEFLAGS;
typedef
enum tagFUNCFLAGS
 { FUNCFLAG_FRESTRICTED = 0x1,
 FUNCFLAG_FSOURCE = 0x2,
 FUNCFLAG_FBINDABLE = 0x4,
 FUNCFLAG_FREQUESTEDIT = 0x8,
 FUNCFLAG_FDISPLAYBIND = 0x10,
 FUNCFLAG_FDEFAULTBIND = 0x20,
 FUNCFLAG_FHIDDEN = 0x40,
 FUNCFLAG_FUSESGETLASTERROR = 0x80,
 FUNCFLAG_FDEFAULTCOLLELEM = 0x100,
 FUNCFLAG_FUIDEFAULT = 0x200,
 FUNCFLAG_FNONBROWSABLE = 0x400,
 FUNCFLAG_FREPLACEABLE = 0x800,
 FUNCFLAG_FIMMEDIATEBIND = 0x1000
 } FUNCFLAGS;
typedef
enum tagVARFLAGS
 { VARFLAG_FREADONLY = 0x1,
 VARFLAG_FSOURCE = 0x2,
 VARFLAG_FBINDABLE = 0x4,
 VARFLAG_FREQUESTEDIT = 0x8,
 VARFLAG_FDISPLAYBIND = 0x10,
 VARFLAG_FDEFAULTBIND = 0x20,
 VARFLAG_FHIDDEN = 0x40,
 VARFLAG_FRESTRICTED = 0x80,
 VARFLAG_FDEFAULTCOLLELEM = 0x100,
 VARFLAG_FUIDEFAULT = 0x200,
 VARFLAG_FNONBROWSABLE = 0x400,
 VARFLAG_FREPLACEABLE = 0x800,
 VARFLAG_FIMMEDIATEBIND = 0x1000
 } VARFLAGS;
typedef struct tagCLEANLOCALSTORAGE
 {
 IUnknown *pInterface;
 PVOID pStorage;
 DWORD flags;
 } CLEANLOCALSTORAGE;
typedef struct tagCUSTDATAITEM
 {
 GUID guid;
 VARIANTARG varValue;
 } CUSTDATAITEM;
typedef struct tagCUSTDATAITEM *LPCUSTDATAITEM;
typedef struct tagCUSTDATA
 {
 DWORD cCustData;
 LPCUSTDATAITEM prgCustData;
 } CUSTDATA;
typedef struct tagCUSTDATA *LPCUSTDATA;
extern RPC_IF_HANDLE IOleAutomationTypes_v1_0_c_ifspec;
extern RPC_IF_HANDLE IOleAutomationTypes_v1_0_s_ifspec;
typedef ICreateTypeInfo *LPCREATETYPEINFO;
extern "C" const IID IID_ICreateTypeInfo;
 struct __declspec(uuid("00020405-0000-0000-C000-000000000046")) __declspec(novtable)
 ICreateTypeInfo : public IUnknown
 {
 public:
 virtual HRESULT __stdcall SetGuid(
 const GUID & guid) = 0;
 virtual HRESULT __stdcall SetTypeFlags(
 UINT uTypeFlags) = 0;
 virtual HRESULT __stdcall SetDocString(
 LPOLESTR pStrDoc) = 0;
 virtual HRESULT __stdcall SetHelpContext(
 DWORD dwHelpContext) = 0;
 virtual HRESULT __stdcall SetVersion(
 WORD wMajorVerNum,
 WORD wMinorVerNum) = 0;
 virtual HRESULT __stdcall AddRefTypeInfo(
 ITypeInfo *pTInfo,
 HREFTYPE *phRefType) = 0;
 virtual HRESULT __stdcall AddFuncDesc(
 UINT index,
 FUNCDESC *pFuncDesc) = 0;
 virtual HRESULT __stdcall AddImplType(
 UINT index,
 HREFTYPE hRefType) = 0;
 virtual HRESULT __stdcall SetImplTypeFlags(
 UINT index,
 INT implTypeFlags) = 0;
 virtual HRESULT __stdcall SetAlignment(
 WORD cbAlignment) = 0;
 virtual HRESULT __stdcall SetSchema(
 LPOLESTR pStrSchema) = 0;
 virtual HRESULT __stdcall AddVarDesc(
 UINT index,
 VARDESC *pVarDesc) = 0;
 virtual HRESULT __stdcall SetFuncAndParamNames(
 UINT index,
 LPOLESTR *rgszNames,
 UINT cNames) = 0;
 virtual HRESULT __stdcall SetVarName(
 UINT index,
 LPOLESTR szName) = 0;
 virtual HRESULT __stdcall SetTypeDescAlias(
 TYPEDESC *pTDescAlias) = 0;
 virtual HRESULT __stdcall DefineFuncAsDllEntry(
 UINT index,
 LPOLESTR szDllName,
 LPOLESTR szProcName) = 0;
 virtual HRESULT __stdcall SetFuncDocString(
 UINT index,
 LPOLESTR szDocString) = 0;
 virtual HRESULT __stdcall SetVarDocString(
 UINT index,
 LPOLESTR szDocString) = 0;
 virtual HRESULT __stdcall SetFuncHelpContext(
 UINT index,
 DWORD dwHelpContext) = 0;
 virtual HRESULT __stdcall SetVarHelpContext(
 UINT index,
 DWORD dwHelpContext) = 0;
 virtual HRESULT __stdcall SetMops(
 UINT index,
 BSTR bstrMops) = 0;
 virtual HRESULT __stdcall SetTypeIdldesc(
 IDLDESC *pIdlDesc) = 0;
 virtual HRESULT __stdcall LayOut( void) = 0;
 };
typedef ICreateTypeInfo2 *LPCREATETYPEINFO2;
extern "C" const IID IID_ICreateTypeInfo2;
 struct __declspec(uuid("0002040E-0000-0000-C000-000000000046")) __declspec(novtable)
 ICreateTypeInfo2 : public ICreateTypeInfo
 {
 public:
 virtual HRESULT __stdcall DeleteFuncDesc(
 UINT index) = 0;
 virtual HRESULT __stdcall DeleteFuncDescByMemId(
 MEMBERID memid,
 INVOKEKIND invKind) = 0;
 virtual HRESULT __stdcall DeleteVarDesc(
 UINT index) = 0;
 virtual HRESULT __stdcall DeleteVarDescByMemId(
 MEMBERID memid) = 0;
 virtual HRESULT __stdcall DeleteImplType(
 UINT index) = 0;
 virtual HRESULT __stdcall SetCustData(
 const GUID & guid,
 VARIANT *pVarVal) = 0;
 virtual HRESULT __stdcall SetFuncCustData(
 UINT index,
 const GUID & guid,
 VARIANT *pVarVal) = 0;
 virtual HRESULT __stdcall SetParamCustData(
 UINT indexFunc,
 UINT indexParam,
 const GUID & guid,
 VARIANT *pVarVal) = 0;
 virtual HRESULT __stdcall SetVarCustData(
 UINT index,
 const GUID & guid,
 VARIANT *pVarVal) = 0;
 virtual HRESULT __stdcall SetImplTypeCustData(
 UINT index,
 const GUID & guid,
 VARIANT *pVarVal) = 0;
 virtual HRESULT __stdcall SetHelpStringContext(
 ULONG dwHelpStringContext) = 0;
 virtual HRESULT __stdcall SetFuncHelpStringContext(
 UINT index,
 ULONG dwHelpStringContext) = 0;
 virtual HRESULT __stdcall SetVarHelpStringContext(
 UINT index,
 ULONG dwHelpStringContext) = 0;
 virtual HRESULT __stdcall Invalidate( void) = 0;
 virtual HRESULT __stdcall SetName(
 LPOLESTR szName) = 0;
 };
typedef ICreateTypeLib *LPCREATETYPELIB;
extern "C" const IID IID_ICreateTypeLib;
 struct __declspec(uuid("00020406-0000-0000-C000-000000000046")) __declspec(novtable)
 ICreateTypeLib : public IUnknown
 {
 public:
 virtual HRESULT __stdcall CreateTypeInfo(
 LPOLESTR szName,
 TYPEKIND tkind,
 ICreateTypeInfo **ppCTInfo) = 0;
 virtual HRESULT __stdcall SetName(
 LPOLESTR szName) = 0;
 virtual HRESULT __stdcall SetVersion(
 WORD wMajorVerNum,
 WORD wMinorVerNum) = 0;
 virtual HRESULT __stdcall SetGuid(
 const GUID & guid) = 0;
 virtual HRESULT __stdcall SetDocString(
 LPOLESTR szDoc) = 0;
 virtual HRESULT __stdcall SetHelpFileName(
 LPOLESTR szHelpFileName) = 0;
 virtual HRESULT __stdcall SetHelpContext(
 DWORD dwHelpContext) = 0;
 virtual HRESULT __stdcall SetLcid(
 LCID lcid) = 0;
 virtual HRESULT __stdcall SetLibFlags(
 UINT uLibFlags) = 0;
 virtual HRESULT __stdcall SaveAllChanges( void) = 0;
 };
typedef ICreateTypeLib2 *LPCREATETYPELIB2;
extern "C" const IID IID_ICreateTypeLib2;
 struct __declspec(uuid("0002040F-0000-0000-C000-000000000046")) __declspec(novtable)
 ICreateTypeLib2 : public ICreateTypeLib
 {
 public:
 virtual HRESULT __stdcall DeleteTypeInfo(
 LPOLESTR szName) = 0;
 virtual HRESULT __stdcall SetCustData(
 const GUID & guid,
 VARIANT *pVarVal) = 0;
 virtual HRESULT __stdcall SetHelpStringContext(
 ULONG dwHelpStringContext) = 0;
 virtual HRESULT __stdcall SetHelpStringDll(
 LPOLESTR szFileName) = 0;
 };
typedef IDispatch *LPDISPATCH;
extern "C" const IID IID_IDispatch;
 struct __declspec(uuid("00020400-0000-0000-C000-000000000046")) __declspec(novtable)
 IDispatch : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetTypeInfoCount(
 UINT *pctinfo) = 0;
 virtual HRESULT __stdcall GetTypeInfo(
 UINT iTInfo,
 LCID lcid,
 ITypeInfo **ppTInfo) = 0;
 virtual HRESULT __stdcall GetIDsOfNames(
 const IID & riid,
 LPOLESTR *rgszNames,
 UINT cNames,
 LCID lcid,
 DISPID *rgDispId) = 0;
 virtual HRESULT __stdcall Invoke(
 DISPID dispIdMember,
 const IID & riid,
 LCID lcid,
 WORD wFlags,
 DISPPARAMS *pDispParams,
 VARIANT *pVarResult,
 EXCEPINFO *pExcepInfo,
 UINT *puArgErr) = 0;
 };
 HRESULT __stdcall IDispatch_RemoteInvoke_Proxy(
 IDispatch * This,
 DISPID dispIdMember,
 const IID & riid,
 LCID lcid,
 DWORD dwFlags,
 DISPPARAMS *pDispParams,
 VARIANT *pVarResult,
 EXCEPINFO *pExcepInfo,
 UINT *pArgErr,
 UINT cVarRef,
 UINT *rgVarRefIdx,
 VARIANTARG *rgVarRef);
void __stdcall IDispatch_RemoteInvoke_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IEnumVARIANT *LPENUMVARIANT;
extern "C" const IID IID_IEnumVARIANT;
 struct __declspec(uuid("00020404-0000-0000-C000-000000000046")) __declspec(novtable)
 IEnumVARIANT : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Next(
 ULONG celt,
 VARIANT *rgVar,
 ULONG *pCeltFetched) = 0;
 virtual HRESULT __stdcall Skip(
 ULONG celt) = 0;
 virtual HRESULT __stdcall Reset( void) = 0;
 virtual HRESULT __stdcall Clone(
 IEnumVARIANT **ppEnum) = 0;
 };
 HRESULT __stdcall IEnumVARIANT_RemoteNext_Proxy(
 IEnumVARIANT * This,
 ULONG celt,
 VARIANT *rgVar,
 ULONG *pCeltFetched);
void __stdcall IEnumVARIANT_RemoteNext_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef ITypeComp *LPTYPECOMP;
typedef
enum tagDESCKIND
 { DESCKIND_NONE = 0,
 DESCKIND_FUNCDESC = ( DESCKIND_NONE + 1 ) ,
 DESCKIND_VARDESC = ( DESCKIND_FUNCDESC + 1 ) ,
 DESCKIND_TYPECOMP = ( DESCKIND_VARDESC + 1 ) ,
 DESCKIND_IMPLICITAPPOBJ = ( DESCKIND_TYPECOMP + 1 ) ,
 DESCKIND_MAX = ( DESCKIND_IMPLICITAPPOBJ + 1 )
 } DESCKIND;
typedef union tagBINDPTR
 {
 FUNCDESC *lpfuncdesc;
 VARDESC *lpvardesc;
 ITypeComp *lptcomp;
 } BINDPTR;
typedef union tagBINDPTR *LPBINDPTR;
extern "C" const IID IID_ITypeComp;
 struct __declspec(uuid("00020403-0000-0000-C000-000000000046")) __declspec(novtable)
 ITypeComp : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Bind(
 LPOLESTR szName,
 ULONG lHashVal,
 WORD wFlags,
 ITypeInfo **ppTInfo,
 DESCKIND *pDescKind,
 BINDPTR *pBindPtr) = 0;
 virtual HRESULT __stdcall BindType(
 LPOLESTR szName,
 ULONG lHashVal,
 ITypeInfo **ppTInfo,
 ITypeComp **ppTComp) = 0;
 };
 HRESULT __stdcall ITypeComp_RemoteBind_Proxy(
 ITypeComp * This,
 LPOLESTR szName,
 ULONG lHashVal,
 WORD wFlags,
 ITypeInfo **ppTInfo,
 DESCKIND *pDescKind,
 LPFUNCDESC *ppFuncDesc,
 LPVARDESC *ppVarDesc,
 ITypeComp **ppTypeComp,
 CLEANLOCALSTORAGE *pDummy);
void __stdcall ITypeComp_RemoteBind_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeComp_RemoteBindType_Proxy(
 ITypeComp * This,
 LPOLESTR szName,
 ULONG lHashVal,
 ITypeInfo **ppTInfo);
void __stdcall ITypeComp_RemoteBindType_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef ITypeInfo *LPTYPEINFO;
extern "C" const IID IID_ITypeInfo;
 struct __declspec(uuid("00020401-0000-0000-C000-000000000046")) __declspec(novtable)
 ITypeInfo : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetTypeAttr(
 TYPEATTR **ppTypeAttr) = 0;
 virtual HRESULT __stdcall GetTypeComp(
 ITypeComp **ppTComp) = 0;
 virtual HRESULT __stdcall GetFuncDesc(
 UINT index,
 FUNCDESC **ppFuncDesc) = 0;
 virtual HRESULT __stdcall GetVarDesc(
 UINT index,
 VARDESC **ppVarDesc) = 0;
 virtual HRESULT __stdcall GetNames(
 MEMBERID memid,
 BSTR *rgBstrNames,
 UINT cMaxNames,
 UINT *pcNames) = 0;
 virtual HRESULT __stdcall GetRefTypeOfImplType(
 UINT index,
 HREFTYPE *pRefType) = 0;
 virtual HRESULT __stdcall GetImplTypeFlags(
 UINT index,
 INT *pImplTypeFlags) = 0;
 virtual HRESULT __stdcall GetIDsOfNames(
 LPOLESTR *rgszNames,
 UINT cNames,
 MEMBERID *pMemId) = 0;
 virtual HRESULT __stdcall Invoke(
 PVOID pvInstance,
 MEMBERID memid,
 WORD wFlags,
 DISPPARAMS *pDispParams,
 VARIANT *pVarResult,
 EXCEPINFO *pExcepInfo,
 UINT *puArgErr) = 0;
 virtual HRESULT __stdcall GetDocumentation(
 MEMBERID memid,
 BSTR *pBstrName,
 BSTR *pBstrDocString,
 DWORD *pdwHelpContext,
 BSTR *pBstrHelpFile) = 0;
 virtual HRESULT __stdcall GetDllEntry(
 MEMBERID memid,
 INVOKEKIND invKind,
 BSTR *pBstrDllName,
 BSTR *pBstrName,
 WORD *pwOrdinal) = 0;
 virtual HRESULT __stdcall GetRefTypeInfo(
 HREFTYPE hRefType,
 ITypeInfo **ppTInfo) = 0;
 virtual HRESULT __stdcall AddressOfMember(
 MEMBERID memid,
 INVOKEKIND invKind,
 PVOID *ppv) = 0;
 virtual HRESULT __stdcall CreateInstance(
 IUnknown *pUnkOuter,
 const IID & riid,
 PVOID *ppvObj) = 0;
 virtual HRESULT __stdcall GetMops(
 MEMBERID memid,
 BSTR *pBstrMops) = 0;
 virtual HRESULT __stdcall GetContainingTypeLib(
 ITypeLib **ppTLib,
 UINT *pIndex) = 0;
 virtual void __stdcall ReleaseTypeAttr(
 TYPEATTR *pTypeAttr) = 0;
 virtual void __stdcall ReleaseFuncDesc(
 FUNCDESC *pFuncDesc) = 0;
 virtual void __stdcall ReleaseVarDesc(
 VARDESC *pVarDesc) = 0;
 };
 HRESULT __stdcall ITypeInfo_RemoteGetTypeAttr_Proxy(
 ITypeInfo * This,
 LPTYPEATTR *ppTypeAttr,
 CLEANLOCALSTORAGE *pDummy);
void __stdcall ITypeInfo_RemoteGetTypeAttr_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeInfo_RemoteGetFuncDesc_Proxy(
 ITypeInfo * This,
 UINT index,
 LPFUNCDESC *ppFuncDesc,
 CLEANLOCALSTORAGE *pDummy);
void __stdcall ITypeInfo_RemoteGetFuncDesc_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeInfo_RemoteGetVarDesc_Proxy(
 ITypeInfo * This,
 UINT index,
 LPVARDESC *ppVarDesc,
 CLEANLOCALSTORAGE *pDummy);
void __stdcall ITypeInfo_RemoteGetVarDesc_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeInfo_RemoteGetNames_Proxy(
 ITypeInfo * This,
 MEMBERID memid,
 BSTR *rgBstrNames,
 UINT cMaxNames,
 UINT *pcNames);
void __stdcall ITypeInfo_RemoteGetNames_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeInfo_LocalGetIDsOfNames_Proxy(
 ITypeInfo * This);
void __stdcall ITypeInfo_LocalGetIDsOfNames_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeInfo_LocalInvoke_Proxy(
 ITypeInfo * This);
void __stdcall ITypeInfo_LocalInvoke_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeInfo_RemoteGetDocumentation_Proxy(
 ITypeInfo * This,
 MEMBERID memid,
 DWORD refPtrFlags,
 BSTR *pBstrName,
 BSTR *pBstrDocString,
 DWORD *pdwHelpContext,
 BSTR *pBstrHelpFile);
void __stdcall ITypeInfo_RemoteGetDocumentation_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeInfo_RemoteGetDllEntry_Proxy(
 ITypeInfo * This,
 MEMBERID memid,
 INVOKEKIND invKind,
 DWORD refPtrFlags,
 BSTR *pBstrDllName,
 BSTR *pBstrName,
 WORD *pwOrdinal);
void __stdcall ITypeInfo_RemoteGetDllEntry_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeInfo_LocalAddressOfMember_Proxy(
 ITypeInfo * This);
void __stdcall ITypeInfo_LocalAddressOfMember_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeInfo_RemoteCreateInstance_Proxy(
 ITypeInfo * This,
 const IID & riid,
 IUnknown **ppvObj);
void __stdcall ITypeInfo_RemoteCreateInstance_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeInfo_RemoteGetContainingTypeLib_Proxy(
 ITypeInfo * This,
 ITypeLib **ppTLib,
 UINT *pIndex);
void __stdcall ITypeInfo_RemoteGetContainingTypeLib_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeInfo_LocalReleaseTypeAttr_Proxy(
 ITypeInfo * This);
void __stdcall ITypeInfo_LocalReleaseTypeAttr_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeInfo_LocalReleaseFuncDesc_Proxy(
 ITypeInfo * This);
void __stdcall ITypeInfo_LocalReleaseFuncDesc_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeInfo_LocalReleaseVarDesc_Proxy(
 ITypeInfo * This);
void __stdcall ITypeInfo_LocalReleaseVarDesc_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef ITypeInfo2 *LPTYPEINFO2;
extern "C" const IID IID_ITypeInfo2;
 struct __declspec(uuid("00020412-0000-0000-C000-000000000046")) __declspec(novtable)
 ITypeInfo2 : public ITypeInfo
 {
 public:
 virtual HRESULT __stdcall GetTypeKind(
 TYPEKIND *pTypeKind) = 0;
 virtual HRESULT __stdcall GetTypeFlags(
 ULONG *pTypeFlags) = 0;
 virtual HRESULT __stdcall GetFuncIndexOfMemId(
 MEMBERID memid,
 INVOKEKIND invKind,
 UINT *pFuncIndex) = 0;
 virtual HRESULT __stdcall GetVarIndexOfMemId(
 MEMBERID memid,
 UINT *pVarIndex) = 0;
 virtual HRESULT __stdcall GetCustData(
 const GUID & guid,
 VARIANT *pVarVal) = 0;
 virtual HRESULT __stdcall GetFuncCustData(
 UINT index,
 const GUID & guid,
 VARIANT *pVarVal) = 0;
 virtual HRESULT __stdcall GetParamCustData(
 UINT indexFunc,
 UINT indexParam,
 const GUID & guid,
 VARIANT *pVarVal) = 0;
 virtual HRESULT __stdcall GetVarCustData(
 UINT index,
 const GUID & guid,
 VARIANT *pVarVal) = 0;
 virtual HRESULT __stdcall GetImplTypeCustData(
 UINT index,
 const GUID & guid,
 VARIANT *pVarVal) = 0;
 virtual HRESULT __stdcall GetDocumentation2(
 MEMBERID memid,
 LCID lcid,
 BSTR *pbstrHelpString,
 DWORD *pdwHelpStringContext,
 BSTR *pbstrHelpStringDll) = 0;
 virtual HRESULT __stdcall GetAllCustData(
 CUSTDATA *pCustData) = 0;
 virtual HRESULT __stdcall GetAllFuncCustData(
 UINT index,
 CUSTDATA *pCustData) = 0;
 virtual HRESULT __stdcall GetAllParamCustData(
 UINT indexFunc,
 UINT indexParam,
 CUSTDATA *pCustData) = 0;
 virtual HRESULT __stdcall GetAllVarCustData(
 UINT index,
 CUSTDATA *pCustData) = 0;
 virtual HRESULT __stdcall GetAllImplTypeCustData(
 UINT index,
 CUSTDATA *pCustData) = 0;
 };
 HRESULT __stdcall ITypeInfo2_RemoteGetDocumentation2_Proxy(
 ITypeInfo2 * This,
 MEMBERID memid,
 LCID lcid,
 DWORD refPtrFlags,
 BSTR *pbstrHelpString,
 DWORD *pdwHelpStringContext,
 BSTR *pbstrHelpStringDll);
void __stdcall ITypeInfo2_RemoteGetDocumentation2_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef
enum tagSYSKIND
 { SYS_WIN16 = 0,
 SYS_WIN32 = ( SYS_WIN16 + 1 ) ,
 SYS_MAC = ( SYS_WIN32 + 1 ) ,
 SYS_WIN64 = ( SYS_MAC + 1 )
 } SYSKIND;
typedef
enum tagLIBFLAGS
 { LIBFLAG_FRESTRICTED = 0x1,
 LIBFLAG_FCONTROL = 0x2,
 LIBFLAG_FHIDDEN = 0x4,
 LIBFLAG_FHASDISKIMAGE = 0x8
 } LIBFLAGS;
typedef ITypeLib *LPTYPELIB;
typedef struct tagTLIBATTR
 {
 GUID guid;
 LCID lcid;
 SYSKIND syskind;
 WORD wMajorVerNum;
 WORD wMinorVerNum;
 WORD wLibFlags;
 } TLIBATTR;
typedef struct tagTLIBATTR *LPTLIBATTR;
extern "C" const IID IID_ITypeLib;
 struct __declspec(uuid("00020402-0000-0000-C000-000000000046")) __declspec(novtable)
 ITypeLib : public IUnknown
 {
 public:
 virtual UINT __stdcall GetTypeInfoCount( void) = 0;
 virtual HRESULT __stdcall GetTypeInfo(
 UINT index,
 ITypeInfo **ppTInfo) = 0;
 virtual HRESULT __stdcall GetTypeInfoType(
 UINT index,
 TYPEKIND *pTKind) = 0;
 virtual HRESULT __stdcall GetTypeInfoOfGuid(
 const GUID & guid,
 ITypeInfo **ppTinfo) = 0;
 virtual HRESULT __stdcall GetLibAttr(
 TLIBATTR **ppTLibAttr) = 0;
 virtual HRESULT __stdcall GetTypeComp(
 ITypeComp **ppTComp) = 0;
 virtual HRESULT __stdcall GetDocumentation(
 INT index,
 BSTR *pBstrName,
 BSTR *pBstrDocString,
 DWORD *pdwHelpContext,
 BSTR *pBstrHelpFile) = 0;
 virtual HRESULT __stdcall IsName(
 LPOLESTR szNameBuf,
 ULONG lHashVal,
 BOOL *pfName) = 0;
 virtual HRESULT __stdcall FindName(
 LPOLESTR szNameBuf,
 ULONG lHashVal,
 ITypeInfo **ppTInfo,
 MEMBERID *rgMemId,
 USHORT *pcFound) = 0;
 virtual void __stdcall ReleaseTLibAttr(
 TLIBATTR *pTLibAttr) = 0;
 };
 HRESULT __stdcall ITypeLib_RemoteGetTypeInfoCount_Proxy(
 ITypeLib * This,
 UINT *pcTInfo);
void __stdcall ITypeLib_RemoteGetTypeInfoCount_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeLib_RemoteGetLibAttr_Proxy(
 ITypeLib * This,
 LPTLIBATTR *ppTLibAttr,
 CLEANLOCALSTORAGE *pDummy);
void __stdcall ITypeLib_RemoteGetLibAttr_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeLib_RemoteGetDocumentation_Proxy(
 ITypeLib * This,
 INT index,
 DWORD refPtrFlags,
 BSTR *pBstrName,
 BSTR *pBstrDocString,
 DWORD *pdwHelpContext,
 BSTR *pBstrHelpFile);
void __stdcall ITypeLib_RemoteGetDocumentation_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeLib_RemoteIsName_Proxy(
 ITypeLib * This,
 LPOLESTR szNameBuf,
 ULONG lHashVal,
 BOOL *pfName,
 BSTR *pBstrLibName);
void __stdcall ITypeLib_RemoteIsName_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeLib_RemoteFindName_Proxy(
 ITypeLib * This,
 LPOLESTR szNameBuf,
 ULONG lHashVal,
 ITypeInfo **ppTInfo,
 MEMBERID *rgMemId,
 USHORT *pcFound,
 BSTR *pBstrLibName);
void __stdcall ITypeLib_RemoteFindName_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeLib_LocalReleaseTLibAttr_Proxy(
 ITypeLib * This);
void __stdcall ITypeLib_LocalReleaseTLibAttr_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef ITypeLib2 *LPTYPELIB2;
extern "C" const IID IID_ITypeLib2;
 struct __declspec(uuid("00020411-0000-0000-C000-000000000046")) __declspec(novtable)
 ITypeLib2 : public ITypeLib
 {
 public:
 virtual HRESULT __stdcall GetCustData(
 const GUID & guid,
 VARIANT *pVarVal) = 0;
 virtual HRESULT __stdcall GetLibStatistics(
 ULONG *pcUniqueNames,
 ULONG *pcchUniqueNames) = 0;
 virtual HRESULT __stdcall GetDocumentation2(
 INT index,
 LCID lcid,
 BSTR *pbstrHelpString,
 DWORD *pdwHelpStringContext,
 BSTR *pbstrHelpStringDll) = 0;
 virtual HRESULT __stdcall GetAllCustData(
 CUSTDATA *pCustData) = 0;
 };
 HRESULT __stdcall ITypeLib2_RemoteGetLibStatistics_Proxy(
 ITypeLib2 * This,
 ULONG *pcUniqueNames,
 ULONG *pcchUniqueNames);
void __stdcall ITypeLib2_RemoteGetLibStatistics_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall ITypeLib2_RemoteGetDocumentation2_Proxy(
 ITypeLib2 * This,
 INT index,
 LCID lcid,
 DWORD refPtrFlags,
 BSTR *pbstrHelpString,
 DWORD *pdwHelpStringContext,
 BSTR *pbstrHelpStringDll);
void __stdcall ITypeLib2_RemoteGetDocumentation2_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef ITypeChangeEvents *LPTYPECHANGEEVENTS;
typedef
enum tagCHANGEKIND
 { CHANGEKIND_ADDMEMBER = 0,
 CHANGEKIND_DELETEMEMBER = ( CHANGEKIND_ADDMEMBER + 1 ) ,
 CHANGEKIND_SETNAMES = ( CHANGEKIND_DELETEMEMBER + 1 ) ,
 CHANGEKIND_SETDOCUMENTATION = ( CHANGEKIND_SETNAMES + 1 ) ,
 CHANGEKIND_GENERAL = ( CHANGEKIND_SETDOCUMENTATION + 1 ) ,
 CHANGEKIND_INVALIDATE = ( CHANGEKIND_GENERAL + 1 ) ,
 CHANGEKIND_CHANGEFAILED = ( CHANGEKIND_INVALIDATE + 1 ) ,
 CHANGEKIND_MAX = ( CHANGEKIND_CHANGEFAILED + 1 )
 } CHANGEKIND;
extern "C" const IID IID_ITypeChangeEvents;
 struct __declspec(uuid("00020410-0000-0000-C000-000000000046")) __declspec(novtable)
 ITypeChangeEvents : public IUnknown
 {
 public:
 virtual HRESULT __stdcall RequestTypeChange(
 CHANGEKIND changeKind,
 ITypeInfo *pTInfoBefore,
 LPOLESTR pStrName,
 INT *pfCancel) = 0;
 virtual HRESULT __stdcall AfterTypeChange(
 CHANGEKIND changeKind,
 ITypeInfo *pTInfoAfter,
 LPOLESTR pStrName) = 0;
 };
typedef IErrorInfo *LPERRORINFO;
extern "C" const IID IID_IErrorInfo;
 struct __declspec(uuid("1CF2B120-547D-101B-8E65-08002B2BD119")) __declspec(novtable)
 IErrorInfo : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetGUID(
 GUID *pGUID) = 0;
 virtual HRESULT __stdcall GetSource(
 BSTR *pBstrSource) = 0;
 virtual HRESULT __stdcall GetDescription(
 BSTR *pBstrDescription) = 0;
 virtual HRESULT __stdcall GetHelpFile(
 BSTR *pBstrHelpFile) = 0;
 virtual HRESULT __stdcall GetHelpContext(
 DWORD *pdwHelpContext) = 0;
 };
typedef ICreateErrorInfo *LPCREATEERRORINFO;
extern "C" const IID IID_ICreateErrorInfo;
 struct __declspec(uuid("22F03340-547D-101B-8E65-08002B2BD119")) __declspec(novtable)
 ICreateErrorInfo : public IUnknown
 {
 public:
 virtual HRESULT __stdcall SetGUID(
 const GUID & rguid) = 0;
 virtual HRESULT __stdcall SetSource(
 LPOLESTR szSource) = 0;
 virtual HRESULT __stdcall SetDescription(
 LPOLESTR szDescription) = 0;
 virtual HRESULT __stdcall SetHelpFile(
 LPOLESTR szHelpFile) = 0;
 virtual HRESULT __stdcall SetHelpContext(
 DWORD dwHelpContext) = 0;
 };
typedef ISupportErrorInfo *LPSUPPORTERRORINFO;
extern "C" const IID IID_ISupportErrorInfo;
 struct __declspec(uuid("DF0B3D60-548F-101B-8E65-08002B2BD119")) __declspec(novtable)
 ISupportErrorInfo : public IUnknown
 {
 public:
 virtual HRESULT __stdcall InterfaceSupportsErrorInfo(
 const IID & riid) = 0;
 };
extern "C" const IID IID_ITypeFactory;
 struct __declspec(uuid("0000002E-0000-0000-C000-000000000046")) __declspec(novtable)
 ITypeFactory : public IUnknown
 {
 public:
 virtual HRESULT __stdcall CreateFromTypeInfo(
 ITypeInfo *pTypeInfo,
 const IID & riid,
 IUnknown **ppv) = 0;
 };
extern "C" const IID IID_ITypeMarshal;
 struct __declspec(uuid("0000002D-0000-0000-C000-000000000046")) __declspec(novtable)
 ITypeMarshal : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Size(
 PVOID pvType,
 DWORD dwDestContext,
 PVOID pvDestContext,
 ULONG *pSize) = 0;
 virtual HRESULT __stdcall Marshal(
 PVOID pvType,
 DWORD dwDestContext,
 PVOID pvDestContext,
 ULONG cbBufferLength,
 BYTE *pBuffer,
 ULONG *pcbWritten) = 0;
 virtual HRESULT __stdcall Unmarshal(
 PVOID pvType,
 DWORD dwFlags,
 ULONG cbBufferLength,
 BYTE *pBuffer,
 ULONG *pcbRead) = 0;
 virtual HRESULT __stdcall Free(
 PVOID pvType) = 0;
 };
typedef IRecordInfo *LPRECORDINFO;
extern "C" const IID IID_IRecordInfo;
 struct __declspec(uuid("0000002F-0000-0000-C000-000000000046")) __declspec(novtable)
 IRecordInfo : public IUnknown
 {
 public:
 virtual HRESULT __stdcall RecordInit(
 PVOID pvNew) = 0;
 virtual HRESULT __stdcall RecordClear(
 PVOID pvExisting) = 0;
 virtual HRESULT __stdcall RecordCopy(
 PVOID pvExisting,
 PVOID pvNew) = 0;
 virtual HRESULT __stdcall GetGuid(
 GUID *pguid) = 0;
 virtual HRESULT __stdcall GetName(
 BSTR *pbstrName) = 0;
 virtual HRESULT __stdcall GetSize(
 ULONG *pcbSize) = 0;
 virtual HRESULT __stdcall GetTypeInfo(
 ITypeInfo **ppTypeInfo) = 0;
 virtual HRESULT __stdcall GetField(
 PVOID pvData,
 LPCOLESTR szFieldName,
 VARIANT *pvarField) = 0;
 virtual HRESULT __stdcall GetFieldNoCopy(
 PVOID pvData,
 LPCOLESTR szFieldName,
 VARIANT *pvarField,
 PVOID *ppvDataCArray) = 0;
 virtual HRESULT __stdcall PutField(
 ULONG wFlags,
 PVOID pvData,
 LPCOLESTR szFieldName,
 VARIANT *pvarField) = 0;
 virtual HRESULT __stdcall PutFieldNoCopy(
 ULONG wFlags,
 PVOID pvData,
 LPCOLESTR szFieldName,
 VARIANT *pvarField) = 0;
 virtual HRESULT __stdcall GetFieldNames(
 ULONG *pcNames,
 BSTR *rgBstrNames) = 0;
 virtual BOOL __stdcall IsMatchingType(
 IRecordInfo *pRecordInfo) = 0;
 virtual PVOID __stdcall RecordCreate( void) = 0;
 virtual HRESULT __stdcall RecordCreateCopy(
 PVOID pvSource,
 PVOID *ppvDest) = 0;
 virtual HRESULT __stdcall RecordDestroy(
 PVOID pvRecord) = 0;
 };
typedef IErrorLog *LPERRORLOG;
extern "C" const IID IID_IErrorLog;
 struct __declspec(uuid("3127CA40-446E-11CE-8135-00AA004BB851")) __declspec(novtable)
 IErrorLog : public IUnknown
 {
 public:
 virtual HRESULT __stdcall AddError(
 LPCOLESTR pszPropName,
 EXCEPINFO *pExcepInfo) = 0;
 };
typedef IPropertyBag *LPPROPERTYBAG;
extern "C" const IID IID_IPropertyBag;
 struct __declspec(uuid("55272A00-42CB-11CE-8135-00AA004BB851")) __declspec(novtable)
 IPropertyBag : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Read(
 LPCOLESTR pszPropName,
 VARIANT *pVar,
 IErrorLog *pErrorLog) = 0;
 virtual HRESULT __stdcall Write(
 LPCOLESTR pszPropName,
 VARIANT *pVar) = 0;
 };
 HRESULT __stdcall IPropertyBag_RemoteRead_Proxy(
 IPropertyBag * This,
 LPCOLESTR pszPropName,
 VARIANT *pVar,
 IErrorLog *pErrorLog,
 DWORD varType,
 IUnknown *pUnkObj);
void __stdcall IPropertyBag_RemoteRead_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
#pragma warning(pop)
extern RPC_IF_HANDLE __MIDL_itf_oaidl_0000_0021_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_oaidl_0000_0021_v0_0_s_ifspec;
unsigned long __stdcall BSTR_UserSize( unsigned long *, unsigned long , BSTR * );
unsigned char * __stdcall BSTR_UserMarshal( unsigned long *, unsigned char *, BSTR * );
unsigned char * __stdcall BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * );
void __stdcall BSTR_UserFree( unsigned long *, BSTR * );
unsigned long __stdcall CLEANLOCALSTORAGE_UserSize( unsigned long *, unsigned long , CLEANLOCALSTORAGE * );
unsigned char * __stdcall CLEANLOCALSTORAGE_UserMarshal( unsigned long *, unsigned char *, CLEANLOCALSTORAGE * );
unsigned char * __stdcall CLEANLOCALSTORAGE_UserUnmarshal(unsigned long *, unsigned char *, CLEANLOCALSTORAGE * );
void __stdcall CLEANLOCALSTORAGE_UserFree( unsigned long *, CLEANLOCALSTORAGE * );
unsigned long __stdcall VARIANT_UserSize( unsigned long *, unsigned long , VARIANT * );
unsigned char * __stdcall VARIANT_UserMarshal( unsigned long *, unsigned char *, VARIANT * );
unsigned char * __stdcall VARIANT_UserUnmarshal(unsigned long *, unsigned char *, VARIANT * );
void __stdcall VARIANT_UserFree( unsigned long *, VARIANT * );
unsigned long __stdcall BSTR_UserSize64( unsigned long *, unsigned long , BSTR * );
unsigned char * __stdcall BSTR_UserMarshal64( unsigned long *, unsigned char *, BSTR * );
unsigned char * __stdcall BSTR_UserUnmarshal64(unsigned long *, unsigned char *, BSTR * );
void __stdcall BSTR_UserFree64( unsigned long *, BSTR * );
unsigned long __stdcall CLEANLOCALSTORAGE_UserSize64( unsigned long *, unsigned long , CLEANLOCALSTORAGE * );
unsigned char * __stdcall CLEANLOCALSTORAGE_UserMarshal64( unsigned long *, unsigned char *, CLEANLOCALSTORAGE * );
unsigned char * __stdcall CLEANLOCALSTORAGE_UserUnmarshal64(unsigned long *, unsigned char *, CLEANLOCALSTORAGE * );
void __stdcall CLEANLOCALSTORAGE_UserFree64( unsigned long *, CLEANLOCALSTORAGE * );
unsigned long __stdcall VARIANT_UserSize64( unsigned long *, unsigned long , VARIANT * );
unsigned char * __stdcall VARIANT_UserMarshal64( unsigned long *, unsigned char *, VARIANT * );
unsigned char * __stdcall VARIANT_UserUnmarshal64(unsigned long *, unsigned char *, VARIANT * );
void __stdcall VARIANT_UserFree64( unsigned long *, VARIANT * );
 HRESULT __stdcall IDispatch_Invoke_Proxy(
 IDispatch * This,
 DISPID dispIdMember,
 const IID & riid,
 LCID lcid,
 WORD wFlags,
 DISPPARAMS *pDispParams,
 VARIANT *pVarResult,
 EXCEPINFO *pExcepInfo,
 UINT *puArgErr);
 HRESULT __stdcall IDispatch_Invoke_Stub(
 IDispatch * This,
 DISPID dispIdMember,
 const IID & riid,
 LCID lcid,
 DWORD dwFlags,
 DISPPARAMS *pDispParams,
 VARIANT *pVarResult,
 EXCEPINFO *pExcepInfo,
 UINT *pArgErr,
 UINT cVarRef,
 UINT *rgVarRefIdx,
 VARIANTARG *rgVarRef);
 HRESULT __stdcall IEnumVARIANT_Next_Proxy(
 IEnumVARIANT * This,
 ULONG celt,
 VARIANT *rgVar,
 ULONG *pCeltFetched);
 HRESULT __stdcall IEnumVARIANT_Next_Stub(
 IEnumVARIANT * This,
 ULONG celt,
 VARIANT *rgVar,
 ULONG *pCeltFetched);
 HRESULT __stdcall ITypeComp_Bind_Proxy(
 ITypeComp * This,
 LPOLESTR szName,
 ULONG lHashVal,
 WORD wFlags,
 ITypeInfo **ppTInfo,
 DESCKIND *pDescKind,
 BINDPTR *pBindPtr);
 HRESULT __stdcall ITypeComp_Bind_Stub(
 ITypeComp * This,
 LPOLESTR szName,
 ULONG lHashVal,
 WORD wFlags,
 ITypeInfo **ppTInfo,
 DESCKIND *pDescKind,
 LPFUNCDESC *ppFuncDesc,
 LPVARDESC *ppVarDesc,
 ITypeComp **ppTypeComp,
 CLEANLOCALSTORAGE *pDummy);
 HRESULT __stdcall ITypeComp_BindType_Proxy(
 ITypeComp * This,
 LPOLESTR szName,
 ULONG lHashVal,
 ITypeInfo **ppTInfo,
 ITypeComp **ppTComp);
 HRESULT __stdcall ITypeComp_BindType_Stub(
 ITypeComp * This,
 LPOLESTR szName,
 ULONG lHashVal,
 ITypeInfo **ppTInfo);
 HRESULT __stdcall ITypeInfo_GetTypeAttr_Proxy(
 ITypeInfo * This,
 TYPEATTR **ppTypeAttr);
 HRESULT __stdcall ITypeInfo_GetTypeAttr_Stub(
 ITypeInfo * This,
 LPTYPEATTR *ppTypeAttr,
 CLEANLOCALSTORAGE *pDummy);
 HRESULT __stdcall ITypeInfo_GetFuncDesc_Proxy(
 ITypeInfo * This,
 UINT index,
 FUNCDESC **ppFuncDesc);
 HRESULT __stdcall ITypeInfo_GetFuncDesc_Stub(
 ITypeInfo * This,
 UINT index,
 LPFUNCDESC *ppFuncDesc,
 CLEANLOCALSTORAGE *pDummy);
 HRESULT __stdcall ITypeInfo_GetVarDesc_Proxy(
 ITypeInfo * This,
 UINT index,
 VARDESC **ppVarDesc);
 HRESULT __stdcall ITypeInfo_GetVarDesc_Stub(
 ITypeInfo * This,
 UINT index,
 LPVARDESC *ppVarDesc,
 CLEANLOCALSTORAGE *pDummy);
 HRESULT __stdcall ITypeInfo_GetNames_Proxy(
 ITypeInfo * This,
 MEMBERID memid,
 BSTR *rgBstrNames,
 UINT cMaxNames,
 UINT *pcNames);
 HRESULT __stdcall ITypeInfo_GetNames_Stub(
 ITypeInfo * This,
 MEMBERID memid,
 BSTR *rgBstrNames,
 UINT cMaxNames,
 UINT *pcNames);
 HRESULT __stdcall ITypeInfo_GetIDsOfNames_Proxy(
 ITypeInfo * This,
 LPOLESTR *rgszNames,
 UINT cNames,
 MEMBERID *pMemId);
 HRESULT __stdcall ITypeInfo_GetIDsOfNames_Stub(
 ITypeInfo * This);
 HRESULT __stdcall ITypeInfo_Invoke_Proxy(
 ITypeInfo * This,
 PVOID pvInstance,
 MEMBERID memid,
 WORD wFlags,
 DISPPARAMS *pDispParams,
 VARIANT *pVarResult,
 EXCEPINFO *pExcepInfo,
 UINT *puArgErr);
 HRESULT __stdcall ITypeInfo_Invoke_Stub(
 ITypeInfo * This);
 HRESULT __stdcall ITypeInfo_GetDocumentation_Proxy(
 ITypeInfo * This,
 MEMBERID memid,
 BSTR *pBstrName,
 BSTR *pBstrDocString,
 DWORD *pdwHelpContext,
 BSTR *pBstrHelpFile);
 HRESULT __stdcall ITypeInfo_GetDocumentation_Stub(
 ITypeInfo * This,
 MEMBERID memid,
 DWORD refPtrFlags,
 BSTR *pBstrName,
 BSTR *pBstrDocString,
 DWORD *pdwHelpContext,
 BSTR *pBstrHelpFile);
 HRESULT __stdcall ITypeInfo_GetDllEntry_Proxy(
 ITypeInfo * This,
 MEMBERID memid,
 INVOKEKIND invKind,
 BSTR *pBstrDllName,
 BSTR *pBstrName,
 WORD *pwOrdinal);
 HRESULT __stdcall ITypeInfo_GetDllEntry_Stub(
 ITypeInfo * This,
 MEMBERID memid,
 INVOKEKIND invKind,
 DWORD refPtrFlags,
 BSTR *pBstrDllName,
 BSTR *pBstrName,
 WORD *pwOrdinal);
 HRESULT __stdcall ITypeInfo_AddressOfMember_Proxy(
 ITypeInfo * This,
 MEMBERID memid,
 INVOKEKIND invKind,
 PVOID *ppv);
 HRESULT __stdcall ITypeInfo_AddressOfMember_Stub(
 ITypeInfo * This);
 HRESULT __stdcall ITypeInfo_CreateInstance_Proxy(
 ITypeInfo * This,
 IUnknown *pUnkOuter,
 const IID & riid,
 PVOID *ppvObj);
 HRESULT __stdcall ITypeInfo_CreateInstance_Stub(
 ITypeInfo * This,
 const IID & riid,
 IUnknown **ppvObj);
 HRESULT __stdcall ITypeInfo_GetContainingTypeLib_Proxy(
 ITypeInfo * This,
 ITypeLib **ppTLib,
 UINT *pIndex);
 HRESULT __stdcall ITypeInfo_GetContainingTypeLib_Stub(
 ITypeInfo * This,
 ITypeLib **ppTLib,
 UINT *pIndex);
 void __stdcall ITypeInfo_ReleaseTypeAttr_Proxy(
 ITypeInfo * This,
 TYPEATTR *pTypeAttr);
 HRESULT __stdcall ITypeInfo_ReleaseTypeAttr_Stub(
 ITypeInfo * This);
 void __stdcall ITypeInfo_ReleaseFuncDesc_Proxy(
 ITypeInfo * This,
 FUNCDESC *pFuncDesc);
 HRESULT __stdcall ITypeInfo_ReleaseFuncDesc_Stub(
 ITypeInfo * This);
 void __stdcall ITypeInfo_ReleaseVarDesc_Proxy(
 ITypeInfo * This,
 VARDESC *pVarDesc);
 HRESULT __stdcall ITypeInfo_ReleaseVarDesc_Stub(
 ITypeInfo * This);
 HRESULT __stdcall ITypeInfo2_GetDocumentation2_Proxy(
 ITypeInfo2 * This,
 MEMBERID memid,
 LCID lcid,
 BSTR *pbstrHelpString,
 DWORD *pdwHelpStringContext,
 BSTR *pbstrHelpStringDll);
 HRESULT __stdcall ITypeInfo2_GetDocumentation2_Stub(
 ITypeInfo2 * This,
 MEMBERID memid,
 LCID lcid,
 DWORD refPtrFlags,
 BSTR *pbstrHelpString,
 DWORD *pdwHelpStringContext,
 BSTR *pbstrHelpStringDll);
 UINT __stdcall ITypeLib_GetTypeInfoCount_Proxy(
 ITypeLib * This);
 HRESULT __stdcall ITypeLib_GetTypeInfoCount_Stub(
 ITypeLib * This,
 UINT *pcTInfo);
 HRESULT __stdcall ITypeLib_GetLibAttr_Proxy(
 ITypeLib * This,
 TLIBATTR **ppTLibAttr);
 HRESULT __stdcall ITypeLib_GetLibAttr_Stub(
 ITypeLib * This,
 LPTLIBATTR *ppTLibAttr,
 CLEANLOCALSTORAGE *pDummy);
 HRESULT __stdcall ITypeLib_GetDocumentation_Proxy(
 ITypeLib * This,
 INT index,
 BSTR *pBstrName,
 BSTR *pBstrDocString,
 DWORD *pdwHelpContext,
 BSTR *pBstrHelpFile);
 HRESULT __stdcall ITypeLib_GetDocumentation_Stub(
 ITypeLib * This,
 INT index,
 DWORD refPtrFlags,
 BSTR *pBstrName,
 BSTR *pBstrDocString,
 DWORD *pdwHelpContext,
 BSTR *pBstrHelpFile);
 HRESULT __stdcall ITypeLib_IsName_Proxy(
 ITypeLib * This,
 LPOLESTR szNameBuf,
 ULONG lHashVal,
 BOOL *pfName);
 HRESULT __stdcall ITypeLib_IsName_Stub(
 ITypeLib * This,
 LPOLESTR szNameBuf,
 ULONG lHashVal,
 BOOL *pfName,
 BSTR *pBstrLibName);
 HRESULT __stdcall ITypeLib_FindName_Proxy(
 ITypeLib * This,
 LPOLESTR szNameBuf,
 ULONG lHashVal,
 ITypeInfo **ppTInfo,
 MEMBERID *rgMemId,
 USHORT *pcFound);
 HRESULT __stdcall ITypeLib_FindName_Stub(
 ITypeLib * This,
 LPOLESTR szNameBuf,
 ULONG lHashVal,
 ITypeInfo **ppTInfo,
 MEMBERID *rgMemId,
 USHORT *pcFound,
 BSTR *pBstrLibName);
 void __stdcall ITypeLib_ReleaseTLibAttr_Proxy(
 ITypeLib * This,
 TLIBATTR *pTLibAttr);
 HRESULT __stdcall ITypeLib_ReleaseTLibAttr_Stub(
 ITypeLib * This);
 HRESULT __stdcall ITypeLib2_GetLibStatistics_Proxy(
 ITypeLib2 * This,
 ULONG *pcUniqueNames,
 ULONG *pcchUniqueNames);
 HRESULT __stdcall ITypeLib2_GetLibStatistics_Stub(
 ITypeLib2 * This,
 ULONG *pcUniqueNames,
 ULONG *pcchUniqueNames);
 HRESULT __stdcall ITypeLib2_GetDocumentation2_Proxy(
 ITypeLib2 * This,
 INT index,
 LCID lcid,
 BSTR *pbstrHelpString,
 DWORD *pdwHelpStringContext,
 BSTR *pbstrHelpStringDll);
 HRESULT __stdcall ITypeLib2_GetDocumentation2_Stub(
 ITypeLib2 * This,
 INT index,
 LCID lcid,
 DWORD refPtrFlags,
 BSTR *pbstrHelpString,
 DWORD *pdwHelpStringContext,
 BSTR *pbstrHelpStringDll);
 HRESULT __stdcall IPropertyBag_Read_Proxy(
 IPropertyBag * This,
 LPCOLESTR pszPropName,
 VARIANT *pVar,
 IErrorLog *pErrorLog);
 HRESULT __stdcall IPropertyBag_Read_Stub(
 IPropertyBag * This,
 LPCOLESTR pszPropName,
 VARIANT *pVar,
 IErrorLog *pErrorLog,
 DWORD varType,
 IUnknown *pUnkObj);
}
extern "C"{
typedef struct _xml_error
 {
 unsigned int _nLine;
 BSTR _pchBuf;
 unsigned int _cchBuf;
 unsigned int _ich;
 BSTR _pszFound;
 BSTR _pszExpected;
 DWORD _reserved1;
 DWORD _reserved2;
 } XML_ERROR;
extern RPC_IF_HANDLE __MIDL_itf_msxml_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_msxml_0000_0000_v0_0_s_ifspec;
typedef
enum tagDOMNodeType
 { NODE_INVALID = 0,
 NODE_ELEMENT = ( NODE_INVALID + 1 ) ,
 NODE_ATTRIBUTE = ( NODE_ELEMENT + 1 ) ,
 NODE_TEXT = ( NODE_ATTRIBUTE + 1 ) ,
 NODE_CDATA_SECTION = ( NODE_TEXT + 1 ) ,
 NODE_ENTITY_REFERENCE = ( NODE_CDATA_SECTION + 1 ) ,
 NODE_ENTITY = ( NODE_ENTITY_REFERENCE + 1 ) ,
 NODE_PROCESSING_INSTRUCTION = ( NODE_ENTITY + 1 ) ,
 NODE_COMMENT = ( NODE_PROCESSING_INSTRUCTION + 1 ) ,
 NODE_DOCUMENT = ( NODE_COMMENT + 1 ) ,
 NODE_DOCUMENT_TYPE = ( NODE_DOCUMENT + 1 ) ,
 NODE_DOCUMENT_FRAGMENT = ( NODE_DOCUMENT_TYPE + 1 ) ,
 NODE_NOTATION = ( NODE_DOCUMENT_FRAGMENT + 1 )
 } DOMNodeType;
typedef
enum tagXMLEMEM_TYPE
 { XMLELEMTYPE_ELEMENT = 0,
 XMLELEMTYPE_TEXT = ( XMLELEMTYPE_ELEMENT + 1 ) ,
 XMLELEMTYPE_COMMENT = ( XMLELEMTYPE_TEXT + 1 ) ,
 XMLELEMTYPE_DOCUMENT = ( XMLELEMTYPE_COMMENT + 1 ) ,
 XMLELEMTYPE_DTD = ( XMLELEMTYPE_DOCUMENT + 1 ) ,
 XMLELEMTYPE_PI = ( XMLELEMTYPE_DTD + 1 ) ,
 XMLELEMTYPE_OTHER = ( XMLELEMTYPE_PI + 1 )
 } XMLELEM_TYPE;
extern "C" const IID LIBID_MSXML;
extern "C" const IID IID_IXMLDOMImplementation;
 struct __declspec(uuid("2933BF8F-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMImplementation : public IDispatch
 {
 public:
 virtual HRESULT __stdcall hasFeature(
 BSTR feature,
 BSTR version,
 VARIANT_BOOL *hasFeature) = 0;
 };
extern "C" const IID IID_IXMLDOMNode;
 struct __declspec(uuid("2933BF80-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMNode : public IDispatch
 {
 public:
 virtual HRESULT __stdcall get_nodeName(
 BSTR *name) = 0;
 virtual HRESULT __stdcall get_nodeValue(
 VARIANT *value) = 0;
 virtual HRESULT __stdcall put_nodeValue(
 VARIANT value) = 0;
 virtual HRESULT __stdcall get_nodeType(
 DOMNodeType *type) = 0;
 virtual HRESULT __stdcall get_parentNode(
 IXMLDOMNode **parent) = 0;
 virtual HRESULT __stdcall get_childNodes(
 IXMLDOMNodeList **childList) = 0;
 virtual HRESULT __stdcall get_firstChild(
 IXMLDOMNode **firstChild) = 0;
 virtual HRESULT __stdcall get_lastChild(
 IXMLDOMNode **lastChild) = 0;
 virtual HRESULT __stdcall get_previousSibling(
 IXMLDOMNode **previousSibling) = 0;
 virtual HRESULT __stdcall get_nextSibling(
 IXMLDOMNode **nextSibling) = 0;
 virtual HRESULT __stdcall get_attributes(
 IXMLDOMNamedNodeMap **attributeMap) = 0;
 virtual HRESULT __stdcall insertBefore(
 IXMLDOMNode *newChild,
 VARIANT refChild,
 IXMLDOMNode **outNewChild) = 0;
 virtual HRESULT __stdcall replaceChild(
 IXMLDOMNode *newChild,
 IXMLDOMNode *oldChild,
 IXMLDOMNode **outOldChild) = 0;
 virtual HRESULT __stdcall removeChild(
 IXMLDOMNode *childNode,
 IXMLDOMNode **oldChild) = 0;
 virtual HRESULT __stdcall appendChild(
 IXMLDOMNode *newChild,
 IXMLDOMNode **outNewChild) = 0;
 virtual HRESULT __stdcall hasChildNodes(
 VARIANT_BOOL *hasChild) = 0;
 virtual HRESULT __stdcall get_ownerDocument(
 IXMLDOMDocument **DOMDocument) = 0;
 virtual HRESULT __stdcall cloneNode(
 VARIANT_BOOL deep,
 IXMLDOMNode **cloneRoot) = 0;
 virtual HRESULT __stdcall get_nodeTypeString(
 BSTR *nodeType) = 0;
 virtual HRESULT __stdcall get_text(
 BSTR *text) = 0;
 virtual HRESULT __stdcall put_text(
 BSTR text) = 0;
 virtual HRESULT __stdcall get_specified(
 VARIANT_BOOL *isSpecified) = 0;
 virtual HRESULT __stdcall get_definition(
 IXMLDOMNode **definitionNode) = 0;
 virtual HRESULT __stdcall get_nodeTypedValue(
 VARIANT *typedValue) = 0;
 virtual HRESULT __stdcall put_nodeTypedValue(
 VARIANT typedValue) = 0;
 virtual HRESULT __stdcall get_dataType(
 VARIANT *dataTypeName) = 0;
 virtual HRESULT __stdcall put_dataType(
 BSTR dataTypeName) = 0;
 virtual HRESULT __stdcall get_xml(
 BSTR *xmlString) = 0;
 virtual HRESULT __stdcall transformNode(
 IXMLDOMNode *stylesheet,
 BSTR *xmlString) = 0;
 virtual HRESULT __stdcall selectNodes(
 BSTR queryString,
 IXMLDOMNodeList **resultList) = 0;
 virtual HRESULT __stdcall selectSingleNode(
 BSTR queryString,
 IXMLDOMNode **resultNode) = 0;
 virtual HRESULT __stdcall get_parsed(
 VARIANT_BOOL *isParsed) = 0;
 virtual HRESULT __stdcall get_namespaceURI(
 BSTR *namespaceURI) = 0;
 virtual HRESULT __stdcall get_prefix(
 BSTR *prefixString) = 0;
 virtual HRESULT __stdcall get_baseName(
 BSTR *nameString) = 0;
 virtual HRESULT __stdcall transformNodeToObject(
 IXMLDOMNode *stylesheet,
 VARIANT outputObject) = 0;
 };
extern "C" const IID IID_IXMLDOMDocumentFragment;
 struct __declspec(uuid("3efaa413-272f-11d2-836f-0000f87a7782")) __declspec(novtable)
 IXMLDOMDocumentFragment : public IXMLDOMNode
 {
 public:
 };
extern "C" const IID IID_IXMLDOMDocument;
 struct __declspec(uuid("2933BF81-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMDocument : public IXMLDOMNode
 {
 public:
 virtual HRESULT __stdcall get_doctype(
 IXMLDOMDocumentType **documentType) = 0;
 virtual HRESULT __stdcall get_implementation(
 IXMLDOMImplementation **impl) = 0;
 virtual HRESULT __stdcall get_documentElement(
 IXMLDOMElement **DOMElement) = 0;
 virtual HRESULT __stdcall putref_documentElement(
 IXMLDOMElement *DOMElement) = 0;
 virtual HRESULT __stdcall createElement(
 BSTR tagName,
 IXMLDOMElement **element) = 0;
 virtual HRESULT __stdcall createDocumentFragment(
 IXMLDOMDocumentFragment **docFrag) = 0;
 virtual HRESULT __stdcall createTextNode(
 BSTR data,
 IXMLDOMText **text) = 0;
 virtual HRESULT __stdcall createComment(
 BSTR data,
 IXMLDOMComment **comment) = 0;
 virtual HRESULT __stdcall createCDATASection(
 BSTR data,
 IXMLDOMCDATASection **cdata) = 0;
 virtual HRESULT __stdcall createProcessingInstruction(
 BSTR target,
 BSTR data,
 IXMLDOMProcessingInstruction **pi) = 0;
 virtual HRESULT __stdcall createAttribute(
 BSTR name,
 IXMLDOMAttribute **attribute) = 0;
 virtual HRESULT __stdcall createEntityReference(
 BSTR name,
 IXMLDOMEntityReference **entityRef) = 0;
 virtual HRESULT __stdcall getElementsByTagName(
 BSTR tagName,
 IXMLDOMNodeList **resultList) = 0;
 virtual HRESULT __stdcall createNode(
 VARIANT Type,
 BSTR name,
 BSTR namespaceURI,
 IXMLDOMNode **node) = 0;
 virtual HRESULT __stdcall nodeFromID(
 BSTR idString,
 IXMLDOMNode **node) = 0;
 virtual HRESULT __stdcall load(
 VARIANT xmlSource,
 VARIANT_BOOL *isSuccessful) = 0;
 virtual HRESULT __stdcall get_readyState(
 long *value) = 0;
 virtual HRESULT __stdcall get_parseError(
 IXMLDOMParseError **errorObj) = 0;
 virtual HRESULT __stdcall get_url(
 BSTR *urlString) = 0;
 virtual HRESULT __stdcall get_async(
 VARIANT_BOOL *isAsync) = 0;
 virtual HRESULT __stdcall put_async(
 VARIANT_BOOL isAsync) = 0;
 virtual HRESULT __stdcall abort( void) = 0;
 virtual HRESULT __stdcall loadXML(
 BSTR bstrXML,
 VARIANT_BOOL *isSuccessful) = 0;
 virtual HRESULT __stdcall save(
 VARIANT destination) = 0;
 virtual HRESULT __stdcall get_validateOnParse(
 VARIANT_BOOL *isValidating) = 0;
 virtual HRESULT __stdcall put_validateOnParse(
 VARIANT_BOOL isValidating) = 0;
 virtual HRESULT __stdcall get_resolveExternals(
 VARIANT_BOOL *isResolving) = 0;
 virtual HRESULT __stdcall put_resolveExternals(
 VARIANT_BOOL isResolving) = 0;
 virtual HRESULT __stdcall get_preserveWhiteSpace(
 VARIANT_BOOL *isPreserving) = 0;
 virtual HRESULT __stdcall put_preserveWhiteSpace(
 VARIANT_BOOL isPreserving) = 0;
 virtual HRESULT __stdcall put_onreadystatechange(
 VARIANT readystatechangeSink) = 0;
 virtual HRESULT __stdcall put_ondataavailable(
 VARIANT ondataavailableSink) = 0;
 virtual HRESULT __stdcall put_ontransformnode(
 VARIANT ontransformnodeSink) = 0;
 };
extern "C" const IID IID_IXMLDOMNodeList;
 struct __declspec(uuid("2933BF82-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMNodeList : public IDispatch
 {
 public:
 virtual HRESULT __stdcall get_item(
 long index,
 IXMLDOMNode **listItem) = 0;
 virtual HRESULT __stdcall get_length(
 long *listLength) = 0;
 virtual HRESULT __stdcall nextNode(
 IXMLDOMNode **nextItem) = 0;
 virtual HRESULT __stdcall reset( void) = 0;
 virtual HRESULT __stdcall get__newEnum(
 IUnknown **ppUnk) = 0;
 };
extern "C" const IID IID_IXMLDOMNamedNodeMap;
 struct __declspec(uuid("2933BF83-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMNamedNodeMap : public IDispatch
 {
 public:
 virtual HRESULT __stdcall getNamedItem(
 BSTR name,
 IXMLDOMNode **namedItem) = 0;
 virtual HRESULT __stdcall setNamedItem(
 IXMLDOMNode *newItem,
 IXMLDOMNode **nameItem) = 0;
 virtual HRESULT __stdcall removeNamedItem(
 BSTR name,
 IXMLDOMNode **namedItem) = 0;
 virtual HRESULT __stdcall get_item(
 long index,
 IXMLDOMNode **listItem) = 0;
 virtual HRESULT __stdcall get_length(
 long *listLength) = 0;
 virtual HRESULT __stdcall getQualifiedItem(
 BSTR baseName,
 BSTR namespaceURI,
 IXMLDOMNode **qualifiedItem) = 0;
 virtual HRESULT __stdcall removeQualifiedItem(
 BSTR baseName,
 BSTR namespaceURI,
 IXMLDOMNode **qualifiedItem) = 0;
 virtual HRESULT __stdcall nextNode(
 IXMLDOMNode **nextItem) = 0;
 virtual HRESULT __stdcall reset( void) = 0;
 virtual HRESULT __stdcall get__newEnum(
 IUnknown **ppUnk) = 0;
 };
extern "C" const IID IID_IXMLDOMCharacterData;
 struct __declspec(uuid("2933BF84-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMCharacterData : public IXMLDOMNode
 {
 public:
 virtual HRESULT __stdcall get_data(
 BSTR *data) = 0;
 virtual HRESULT __stdcall put_data(
 BSTR data) = 0;
 virtual HRESULT __stdcall get_length(
 long *dataLength) = 0;
 virtual HRESULT __stdcall substringData(
 long offset,
 long count,
 BSTR *data) = 0;
 virtual HRESULT __stdcall appendData(
 BSTR data) = 0;
 virtual HRESULT __stdcall insertData(
 long offset,
 BSTR data) = 0;
 virtual HRESULT __stdcall deleteData(
 long offset,
 long count) = 0;
 virtual HRESULT __stdcall replaceData(
 long offset,
 long count,
 BSTR data) = 0;
 };
extern "C" const IID IID_IXMLDOMAttribute;
 struct __declspec(uuid("2933BF85-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMAttribute : public IXMLDOMNode
 {
 public:
 virtual HRESULT __stdcall get_name(
 BSTR *attributeName) = 0;
 virtual HRESULT __stdcall get_value(
 VARIANT *attributeValue) = 0;
 virtual HRESULT __stdcall put_value(
 VARIANT attributeValue) = 0;
 };
extern "C" const IID IID_IXMLDOMElement;
 struct __declspec(uuid("2933BF86-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMElement : public IXMLDOMNode
 {
 public:
 virtual HRESULT __stdcall get_tagName(
 BSTR *tagName) = 0;
 virtual HRESULT __stdcall getAttribute(
 BSTR name,
 VARIANT *value) = 0;
 virtual HRESULT __stdcall setAttribute(
 BSTR name,
 VARIANT value) = 0;
 virtual HRESULT __stdcall removeAttribute(
 BSTR name) = 0;
 virtual HRESULT __stdcall getAttributeNode(
 BSTR name,
 IXMLDOMAttribute **attributeNode) = 0;
 virtual HRESULT __stdcall setAttributeNode(
 IXMLDOMAttribute *DOMAttribute,
 IXMLDOMAttribute **attributeNode) = 0;
 virtual HRESULT __stdcall removeAttributeNode(
 IXMLDOMAttribute *DOMAttribute,
 IXMLDOMAttribute **attributeNode) = 0;
 virtual HRESULT __stdcall getElementsByTagName(
 BSTR tagName,
 IXMLDOMNodeList **resultList) = 0;
 virtual HRESULT __stdcall normalize( void) = 0;
 };
extern "C" const IID IID_IXMLDOMText;
 struct __declspec(uuid("2933BF87-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMText : public IXMLDOMCharacterData
 {
 public:
 virtual HRESULT __stdcall splitText(
 long offset,
 IXMLDOMText **rightHandTextNode) = 0;
 };
extern "C" const IID IID_IXMLDOMComment;
 struct __declspec(uuid("2933BF88-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMComment : public IXMLDOMCharacterData
 {
 public:
 };
extern "C" const IID IID_IXMLDOMProcessingInstruction;
 struct __declspec(uuid("2933BF89-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMProcessingInstruction : public IXMLDOMNode
 {
 public:
 virtual HRESULT __stdcall get_target(
 BSTR *name) = 0;
 virtual HRESULT __stdcall get_data(
 BSTR *value) = 0;
 virtual HRESULT __stdcall put_data(
 BSTR value) = 0;
 };
extern "C" const IID IID_IXMLDOMCDATASection;
 struct __declspec(uuid("2933BF8A-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMCDATASection : public IXMLDOMText
 {
 public:
 };
extern "C" const IID IID_IXMLDOMDocumentType;
 struct __declspec(uuid("2933BF8B-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMDocumentType : public IXMLDOMNode
 {
 public:
 virtual HRESULT __stdcall get_name(
 BSTR *rootName) = 0;
 virtual HRESULT __stdcall get_entities(
 IXMLDOMNamedNodeMap **entityMap) = 0;
 virtual HRESULT __stdcall get_notations(
 IXMLDOMNamedNodeMap **notationMap) = 0;
 };
extern "C" const IID IID_IXMLDOMNotation;
 struct __declspec(uuid("2933BF8C-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMNotation : public IXMLDOMNode
 {
 public:
 virtual HRESULT __stdcall get_publicId(
 VARIANT *publicID) = 0;
 virtual HRESULT __stdcall get_systemId(
 VARIANT *systemID) = 0;
 };
extern "C" const IID IID_IXMLDOMEntity;
 struct __declspec(uuid("2933BF8D-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMEntity : public IXMLDOMNode
 {
 public:
 virtual HRESULT __stdcall get_publicId(
 VARIANT *publicID) = 0;
 virtual HRESULT __stdcall get_systemId(
 VARIANT *systemID) = 0;
 virtual HRESULT __stdcall get_notationName(
 BSTR *name) = 0;
 };
extern "C" const IID IID_IXMLDOMEntityReference;
 struct __declspec(uuid("2933BF8E-7B36-11d2-B20E-00C04F983E60")) __declspec(novtable)
 IXMLDOMEntityReference : public IXMLDOMNode
 {
 public:
 };
extern "C" const IID IID_IXMLDOMParseError;
 struct __declspec(uuid("3efaa426-272f-11d2-836f-0000f87a7782")) __declspec(novtable)
 IXMLDOMParseError : public IDispatch
 {
 public:
 virtual HRESULT __stdcall get_errorCode(
 long *errorCode) = 0;
 virtual HRESULT __stdcall get_url(
 BSTR *urlString) = 0;
 virtual HRESULT __stdcall get_reason(
 BSTR *reasonString) = 0;
 virtual HRESULT __stdcall get_srcText(
 BSTR *sourceString) = 0;
 virtual HRESULT __stdcall get_line(
 long *lineNumber) = 0;
 virtual HRESULT __stdcall get_linepos(
 long *linePosition) = 0;
 virtual HRESULT __stdcall get_filepos(
 long *filePosition) = 0;
 };
extern "C" const IID IID_IXTLRuntime;
 struct __declspec(uuid("3efaa425-272f-11d2-836f-0000f87a7782")) __declspec(novtable)
 IXTLRuntime : public IXMLDOMNode
 {
 public:
 virtual HRESULT __stdcall uniqueID(
 IXMLDOMNode *pNode,
 long *pID) = 0;
 virtual HRESULT __stdcall depth(
 IXMLDOMNode *pNode,
 long *pDepth) = 0;
 virtual HRESULT __stdcall childNumber(
 IXMLDOMNode *pNode,
 long *pNumber) = 0;
 virtual HRESULT __stdcall ancestorChildNumber(
 BSTR bstrNodeName,
 IXMLDOMNode *pNode,
 long *pNumber) = 0;
 virtual HRESULT __stdcall absoluteChildNumber(
 IXMLDOMNode *pNode,
 long *pNumber) = 0;
 virtual HRESULT __stdcall formatIndex(
 long lIndex,
 BSTR bstrFormat,
 BSTR *pbstrFormattedString) = 0;
 virtual HRESULT __stdcall formatNumber(
 double dblNumber,
 BSTR bstrFormat,
 BSTR *pbstrFormattedString) = 0;
 virtual HRESULT __stdcall formatDate(
 VARIANT varDate,
 BSTR bstrFormat,
 VARIANT varDestLocale,
 BSTR *pbstrFormattedString) = 0;
 virtual HRESULT __stdcall formatTime(
 VARIANT varTime,
 BSTR bstrFormat,
 VARIANT varDestLocale,
 BSTR *pbstrFormattedString) = 0;
 };
extern "C" const IID DIID_XMLDOMDocumentEvents;
 struct __declspec(uuid("3efaa427-272f-11d2-836f-0000f87a7782")) __declspec(novtable)
 XMLDOMDocumentEvents : public IDispatch
 {
 };
extern "C" const CLSID CLSID_DOMDocument;
class __declspec(uuid("2933BF90-7B36-11d2-B20E-00C04F983E60"))
DOMDocument;
extern "C" const CLSID CLSID_DOMFreeThreadedDocument;
class __declspec(uuid("2933BF91-7B36-11d2-B20E-00C04F983E60"))
DOMFreeThreadedDocument;
extern "C" const IID IID_IXMLHttpRequest;
 struct __declspec(uuid("ED8C108D-4349-11D2-91A4-00C04F7969E8")) __declspec(novtable)
 IXMLHttpRequest : public IDispatch
 {
 public:
 virtual HRESULT __stdcall open(
 BSTR bstrMethod,
 BSTR bstrUrl,
 VARIANT varAsync,
 VARIANT bstrUser,
 VARIANT bstrPassword) = 0;
 virtual HRESULT __stdcall setRequestHeader(
 BSTR bstrHeader,
 BSTR bstrValue) = 0;
 virtual HRESULT __stdcall getResponseHeader(
 BSTR bstrHeader,
 BSTR *pbstrValue) = 0;
 virtual HRESULT __stdcall getAllResponseHeaders(
 BSTR *pbstrHeaders) = 0;
 virtual HRESULT __stdcall send(
 VARIANT varBody) = 0;
 virtual HRESULT __stdcall abort( void) = 0;
 virtual HRESULT __stdcall get_status(
 long *plStatus) = 0;
 virtual HRESULT __stdcall get_statusText(
 BSTR *pbstrStatus) = 0;
 virtual HRESULT __stdcall get_responseXML(
 IDispatch **ppBody) = 0;
 virtual HRESULT __stdcall get_responseText(
 BSTR *pbstrBody) = 0;
 virtual HRESULT __stdcall get_responseBody(
 VARIANT *pvarBody) = 0;
 virtual HRESULT __stdcall get_responseStream(
 VARIANT *pvarBody) = 0;
 virtual HRESULT __stdcall get_readyState(
 long *plState) = 0;
 virtual HRESULT __stdcall put_onreadystatechange(
 IDispatch *pReadyStateSink) = 0;
 };
extern "C" const CLSID CLSID_XMLHTTPRequest;
class __declspec(uuid("ED8C108E-4349-11D2-91A4-00C04F7969E8"))
XMLHTTPRequest;
extern "C" const IID IID_IXMLDSOControl;
 struct __declspec(uuid("310afa62-0575-11d2-9ca9-0060b0ec3d39")) __declspec(novtable)
 IXMLDSOControl : public IDispatch
 {
 public:
 virtual HRESULT __stdcall get_XMLDocument(
 IXMLDOMDocument **ppDoc) = 0;
 virtual HRESULT __stdcall put_XMLDocument(
 IXMLDOMDocument *ppDoc) = 0;
 virtual HRESULT __stdcall get_JavaDSOCompatible(
 BOOL *fJavaDSOCompatible) = 0;
 virtual HRESULT __stdcall put_JavaDSOCompatible(
 BOOL fJavaDSOCompatible) = 0;
 virtual HRESULT __stdcall get_readyState(
 long *state) = 0;
 };
extern "C" const CLSID CLSID_XMLDSOControl;
class __declspec(uuid("550dda30-0541-11d2-9ca9-0060b0ec3d39"))
XMLDSOControl;
extern "C" const IID IID_IXMLElementCollection;
 struct __declspec(uuid("65725580-9B5D-11d0-9BFE-00C04FC99C8E")) __declspec(novtable)
 IXMLElementCollection : public IDispatch
 {
 public:
 virtual HRESULT __stdcall put_length(
 long v) = 0;
 virtual HRESULT __stdcall get_length(
 long *p) = 0;
 virtual HRESULT __stdcall get__newEnum(
 IUnknown **ppUnk) = 0;
 virtual HRESULT __stdcall item(
 VARIANT var1,
 VARIANT var2,
 IDispatch **ppDisp) = 0;
 };
extern "C" const IID IID_IXMLDocument;
 struct __declspec(uuid("F52E2B61-18A1-11d1-B105-00805F49916B")) __declspec(novtable)
 IXMLDocument : public IDispatch
 {
 public:
 virtual HRESULT __stdcall get_root(
 IXMLElement **p) = 0;
 virtual HRESULT __stdcall get_fileSize(
 BSTR *p) = 0;
 virtual HRESULT __stdcall get_fileModifiedDate(
 BSTR *p) = 0;
 virtual HRESULT __stdcall get_fileUpdatedDate(
 BSTR *p) = 0;
 virtual HRESULT __stdcall get_URL(
 BSTR *p) = 0;
 virtual HRESULT __stdcall put_URL(
 BSTR p) = 0;
 virtual HRESULT __stdcall get_mimeType(
 BSTR *p) = 0;
 virtual HRESULT __stdcall get_readyState(
 long *pl) = 0;
 virtual HRESULT __stdcall get_charset(
 BSTR *p) = 0;
 virtual HRESULT __stdcall put_charset(
 BSTR p) = 0;
 virtual HRESULT __stdcall get_version(
 BSTR *p) = 0;
 virtual HRESULT __stdcall get_doctype(
 BSTR *p) = 0;
 virtual HRESULT __stdcall get_dtdURL(
 BSTR *p) = 0;
 virtual HRESULT __stdcall createElement(
 VARIANT vType,
 VARIANT var1,
 IXMLElement **ppElem) = 0;
 };
extern "C" const IID IID_IXMLDocument2;
 struct __declspec(uuid("2B8DE2FE-8D2D-11d1-B2FC-00C04FD915A9")) __declspec(novtable)
 IXMLDocument2 : public IDispatch
 {
 public:
 virtual HRESULT __stdcall get_root(
 IXMLElement2 **p) = 0;
 virtual HRESULT __stdcall get_fileSize(
 BSTR *p) = 0;
 virtual HRESULT __stdcall get_fileModifiedDate(
 BSTR *p) = 0;
 virtual HRESULT __stdcall get_fileUpdatedDate(
 BSTR *p) = 0;
 virtual HRESULT __stdcall get_URL(
 BSTR *p) = 0;
 virtual HRESULT __stdcall put_URL(
 BSTR p) = 0;
 virtual HRESULT __stdcall get_mimeType(
 BSTR *p) = 0;
 virtual HRESULT __stdcall get_readyState(
 long *pl) = 0;
 virtual HRESULT __stdcall get_charset(
 BSTR *p) = 0;
 virtual HRESULT __stdcall put_charset(
 BSTR p) = 0;
 virtual HRESULT __stdcall get_version(
 BSTR *p) = 0;
 virtual HRESULT __stdcall get_doctype(
 BSTR *p) = 0;
 virtual HRESULT __stdcall get_dtdURL(
 BSTR *p) = 0;
 virtual HRESULT __stdcall createElement(
 VARIANT vType,
 VARIANT var1,
 IXMLElement2 **ppElem) = 0;
 virtual HRESULT __stdcall get_async(
 VARIANT_BOOL *pf) = 0;
 virtual HRESULT __stdcall put_async(
 VARIANT_BOOL f) = 0;
 };
extern "C" const IID IID_IXMLElement;
 struct __declspec(uuid("3F7F31AC-E15F-11d0-9C25-00C04FC99C8E")) __declspec(novtable)
 IXMLElement : public IDispatch
 {
 public:
 virtual HRESULT __stdcall get_tagName(
 BSTR *p) = 0;
 virtual HRESULT __stdcall put_tagName(
 BSTR p) = 0;
 virtual HRESULT __stdcall get_parent(
 IXMLElement **ppParent) = 0;
 virtual HRESULT __stdcall setAttribute(
 BSTR strPropertyName,
 VARIANT PropertyValue) = 0;
 virtual HRESULT __stdcall getAttribute(
 BSTR strPropertyName,
 VARIANT *PropertyValue) = 0;
 virtual HRESULT __stdcall removeAttribute(
 BSTR strPropertyName) = 0;
 virtual HRESULT __stdcall get_children(
 IXMLElementCollection **pp) = 0;
 virtual HRESULT __stdcall get_type(
 long *plType) = 0;
 virtual HRESULT __stdcall get_text(
 BSTR *p) = 0;
 virtual HRESULT __stdcall put_text(
 BSTR p) = 0;
 virtual HRESULT __stdcall addChild(
 IXMLElement *pChildElem,
 long lIndex,
 long lReserved) = 0;
 virtual HRESULT __stdcall removeChild(
 IXMLElement *pChildElem) = 0;
 };
extern "C" const IID IID_IXMLElement2;
 struct __declspec(uuid("2B8DE2FF-8D2D-11d1-B2FC-00C04FD915A9")) __declspec(novtable)
 IXMLElement2 : public IDispatch
 {
 public:
 virtual HRESULT __stdcall get_tagName(
 BSTR *p) = 0;
 virtual HRESULT __stdcall put_tagName(
 BSTR p) = 0;
 virtual HRESULT __stdcall get_parent(
 IXMLElement2 **ppParent) = 0;
 virtual HRESULT __stdcall setAttribute(
 BSTR strPropertyName,
 VARIANT PropertyValue) = 0;
 virtual HRESULT __stdcall getAttribute(
 BSTR strPropertyName,
 VARIANT *PropertyValue) = 0;
 virtual HRESULT __stdcall removeAttribute(
 BSTR strPropertyName) = 0;
 virtual HRESULT __stdcall get_children(
 IXMLElementCollection **pp) = 0;
 virtual HRESULT __stdcall get_type(
 long *plType) = 0;
 virtual HRESULT __stdcall get_text(
 BSTR *p) = 0;
 virtual HRESULT __stdcall put_text(
 BSTR p) = 0;
 virtual HRESULT __stdcall addChild(
 IXMLElement2 *pChildElem,
 long lIndex,
 long lReserved) = 0;
 virtual HRESULT __stdcall removeChild(
 IXMLElement2 *pChildElem) = 0;
 virtual HRESULT __stdcall get_attributes(
 IXMLElementCollection **pp) = 0;
 };
extern "C" const IID IID_IXMLAttribute;
 struct __declspec(uuid("D4D4A0FC-3B73-11d1-B2B4-00C04FB92596")) __declspec(novtable)
 IXMLAttribute : public IDispatch
 {
 public:
 virtual HRESULT __stdcall get_name(
 BSTR *n) = 0;
 virtual HRESULT __stdcall get_value(
 BSTR *v) = 0;
 };
extern "C" const IID IID_IXMLError;
 struct __declspec(uuid("948C5AD3-C58D-11d0-9C0B-00C04FC99C8E")) __declspec(novtable)
 IXMLError : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetErrorInfo(
 XML_ERROR *pErrorReturn) = 0;
 };
extern "C" const CLSID CLSID_XMLDocument;
class __declspec(uuid("CFC399AF-D876-11d0-9C10-00C04FC99C8E"))
XMLDocument;
}
extern "C"{
#pragma comment(lib,"uuid.lib")
extern "C" const IID CLSID_SBS_StdURLMoniker;
extern "C" const IID CLSID_SBS_HttpProtocol;
extern "C" const IID CLSID_SBS_FtpProtocol;
extern "C" const IID CLSID_SBS_GopherProtocol;
extern "C" const IID CLSID_SBS_HttpSProtocol;
extern "C" const IID CLSID_SBS_FileProtocol;
extern "C" const IID CLSID_SBS_MkProtocol;
extern "C" const IID CLSID_SBS_UrlMkBindCtx;
extern "C" const IID CLSID_SBS_SoftDistExt;
extern "C" const IID CLSID_SBS_StdEncodingFilterFac;
extern "C" const IID CLSID_SBS_DeCompMimeFilter;
extern "C" const IID CLSID_SBS_CdlProtocol;
extern "C" const IID CLSID_SBS_ClassInstallFilter;
extern "C" const IID CLSID_SBS_InternetSecurityManager;
extern "C" const IID CLSID_SBS_InternetZoneManager;
extern "C" const IID IID_IAsyncMoniker;
extern "C" const IID CLSID_StdURLMoniker;
extern "C" const IID CLSID_HttpProtocol;
extern "C" const IID CLSID_FtpProtocol;
extern "C" const IID CLSID_GopherProtocol;
extern "C" const IID CLSID_HttpSProtocol;
extern "C" const IID CLSID_FileProtocol;
extern "C" const IID CLSID_MkProtocol;
extern "C" const IID CLSID_StdURLProtocol;
extern "C" const IID CLSID_UrlMkBindCtx;
extern "C" const IID CLSID_StdEncodingFilterFac;
extern "C" const IID CLSID_DeCompMimeFilter;
extern "C" const IID CLSID_CdlProtocol;
extern "C" const IID CLSID_ClassInstallFilter;
extern "C" const IID IID_IAsyncBindCtx;
extern "C" HRESULT __stdcall CreateURLMoniker(LPMONIKER pMkCtx, LPCWSTR szURL, LPMONIKER * ppmk);
extern "C" HRESULT __stdcall CreateURLMonikerEx(LPMONIKER pMkCtx, LPCWSTR szURL, LPMONIKER * ppmk, DWORD dwFlags);
extern "C" HRESULT __stdcall GetClassURL(LPCWSTR szURL, CLSID *pClsID);
extern "C" HRESULT __stdcall CreateAsyncBindCtx(DWORD reserved, IBindStatusCallback *pBSCb,
 IEnumFORMATETC *pEFetc, IBindCtx **ppBC);
extern "C" HRESULT __stdcall CreateURLMonikerEx2(LPMONIKER pMkCtx, IUri* pUri, LPMONIKER * ppmk, DWORD dwFlags);
extern "C" HRESULT __stdcall CreateAsyncBindCtxEx(IBindCtx *pbc, DWORD dwOptions, IBindStatusCallback *pBSCb, IEnumFORMATETC *pEnum,
 IBindCtx **ppBC, DWORD reserved);
extern "C" HRESULT __stdcall MkParseDisplayNameEx(IBindCtx *pbc, LPCWSTR szDisplayName, ULONG *pchEaten,
 LPMONIKER *ppmk);
extern "C" HRESULT __stdcall RegisterBindStatusCallback(LPBC pBC, IBindStatusCallback *pBSCb,
 IBindStatusCallback** ppBSCBPrev, DWORD dwReserved);
extern "C" HRESULT __stdcall RevokeBindStatusCallback(LPBC pBC, IBindStatusCallback *pBSCb);
extern "C" HRESULT __stdcall GetClassFileOrMime(LPBC pBC, LPCWSTR szFilename, LPVOID pBuffer, DWORD cbSize, LPCWSTR szMime, DWORD dwReserved, CLSID *pclsid);
extern "C" HRESULT __stdcall IsValidURL(LPBC pBC, LPCWSTR szURL, DWORD dwReserved);
extern "C" HRESULT __stdcall CoGetClassObjectFromURL( const IID & rCLASSID,
 LPCWSTR szCODE, DWORD dwFileVersionMS,
 DWORD dwFileVersionLS, LPCWSTR szTYPE,
 LPBINDCTX pBindCtx, DWORD dwClsContext,
 LPVOID pvReserved, const IID & riid, LPVOID * ppv);
extern "C" HRESULT __stdcall FaultInIEFeature( HWND hWnd,
 uCLSSPEC *pClassSpec,
 QUERYCONTEXT *pQuery, DWORD dwFlags);
extern "C" HRESULT __stdcall GetComponentIDFromCLSSPEC( uCLSSPEC *pClassspec,
 LPSTR * ppszComponentID);
extern "C" HRESULT __stdcall IsAsyncMoniker(IMoniker* pmk);
extern "C" HRESULT __stdcall CreateURLBinding(LPCWSTR lpszUrl, IBindCtx *pbc, IBinding **ppBdg);
extern "C" HRESULT __stdcall RegisterMediaTypes(UINT ctypes, const LPCSTR* rgszTypes, CLIPFORMAT* rgcfTypes);
extern "C" HRESULT __stdcall FindMediaType(LPCSTR rgszTypes, CLIPFORMAT* rgcfTypes);
extern "C" HRESULT __stdcall CreateFormatEnumerator( UINT cfmtetc, FORMATETC* rgfmtetc, IEnumFORMATETC** ppenumfmtetc);
extern "C" HRESULT __stdcall RegisterFormatEnumerator(LPBC pBC, IEnumFORMATETC *pEFetc, DWORD reserved);
extern "C" HRESULT __stdcall RevokeFormatEnumerator(LPBC pBC, IEnumFORMATETC *pEFetc);
extern "C" HRESULT __stdcall RegisterMediaTypeClass(LPBC pBC,UINT ctypes, const LPCSTR* rgszTypes, CLSID *rgclsID, DWORD reserved);
extern "C" HRESULT __stdcall FindMediaTypeClass(LPBC pBC, LPCSTR szType, CLSID *pclsID, DWORD reserved);
extern "C" HRESULT __stdcall UrlMkSetSessionOption(DWORD dwOption, LPVOID pBuffer, DWORD dwBufferLength, DWORD dwReserved);
extern "C" HRESULT __stdcall UrlMkGetSessionOption(DWORD dwOption, LPVOID pBuffer, DWORD dwBufferLength, DWORD *pdwBufferLength, DWORD dwReserved);
extern "C" HRESULT __stdcall FindMimeFromData(
 LPBC pBC,
 LPCWSTR pwzUrl,
 LPVOID pBuffer,
 DWORD cbSize,
 LPCWSTR pwzMimeProposed,
 DWORD dwMimeFlags,
 LPWSTR *ppwzMimeOut,
 DWORD dwReserved);
extern "C" HRESULT __stdcall ObtainUserAgentString(
 DWORD dwOption,
 LPSTR pszUAOut,
 DWORD *cbSize);
extern "C" HRESULT __stdcall CompareSecurityIds(BYTE* pbSecurityId1, DWORD dwLen1, BYTE* pbSecurityId2, DWORD dwLen2, DWORD dwReserved);
extern "C" HRESULT __stdcall CompatFlagsFromClsid(CLSID *pclsid, LPDWORD pdwCompatFlags, LPDWORD pdwMiscStatusFlags);
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0000_v0_0_s_ifspec;
typedef IPersistMoniker *LPPERSISTMONIKER;
extern "C" const IID IID_IPersistMoniker;
 struct __declspec(uuid("79eac9c9-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IPersistMoniker : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetClassID(
 CLSID *pClassID) = 0;
 virtual HRESULT __stdcall IsDirty( void) = 0;
 virtual HRESULT __stdcall Load(
 BOOL fFullyAvailable,
 IMoniker *pimkName,
 LPBC pibc,
 DWORD grfMode) = 0;
 virtual HRESULT __stdcall Save(
 IMoniker *pimkName,
 LPBC pbc,
 BOOL fRemember) = 0;
 virtual HRESULT __stdcall SaveCompleted(
 IMoniker *pimkName,
 LPBC pibc) = 0;
 virtual HRESULT __stdcall GetCurMoniker(
 IMoniker **ppimkName) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0001_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0001_v0_0_s_ifspec;
typedef IMonikerProp *LPMONIKERPROP;
typedef
enum __MIDL_IMonikerProp_0001
 { MIMETYPEPROP = 0,
 USE_SRC_URL = 0x1,
 CLASSIDPROP = 0x2,
 TRUSTEDDOWNLOADPROP = 0x3,
 POPUPLEVELPROP = 0x4
 } MONIKERPROPERTY;
extern "C" const IID IID_IMonikerProp;
 struct __declspec(uuid("a5ca5f7f-1847-4d87-9c5b-918509f7511d")) __declspec(novtable)
 IMonikerProp : public IUnknown
 {
 public:
 virtual HRESULT __stdcall PutProperty(
 MONIKERPROPERTY mkp,
 LPCWSTR val) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0002_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0002_v0_0_s_ifspec;
typedef IBindProtocol *LPBINDPROTOCOL;
extern "C" const IID IID_IBindProtocol;
 struct __declspec(uuid("79eac9cd-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IBindProtocol : public IUnknown
 {
 public:
 virtual HRESULT __stdcall CreateBinding(
 LPCWSTR szUrl,
 IBindCtx *pbc,
 IBinding **ppb) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0003_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0003_v0_0_s_ifspec;
typedef IBinding *LPBINDING;
extern "C" const IID IID_IBinding;
 struct __declspec(uuid("79eac9c0-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IBinding : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Abort( void) = 0;
 virtual HRESULT __stdcall Suspend( void) = 0;
 virtual HRESULT __stdcall Resume( void) = 0;
 virtual HRESULT __stdcall SetPriority(
 LONG nPriority) = 0;
 virtual HRESULT __stdcall GetPriority(
 LONG *pnPriority) = 0;
 virtual HRESULT __stdcall GetBindResult(
 CLSID *pclsidProtocol,
 DWORD *pdwResult,
 LPOLESTR *pszResult,
 DWORD *pdwReserved) = 0;
 };
 HRESULT __stdcall IBinding_RemoteGetBindResult_Proxy(
 IBinding * This,
 CLSID *pclsidProtocol,
 DWORD *pdwResult,
 LPOLESTR *pszResult,
 DWORD dwReserved);
void __stdcall IBinding_RemoteGetBindResult_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0004_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0004_v0_0_s_ifspec;
typedef IBindStatusCallback *LPBINDSTATUSCALLBACK;
typedef
enum __MIDL_IBindStatusCallback_0001
 { BINDVERB_GET = 0,
 BINDVERB_POST = 0x1,
 BINDVERB_PUT = 0x2,
 BINDVERB_CUSTOM = 0x3
 } BINDVERB;
typedef
enum __MIDL_IBindStatusCallback_0002
 { BINDINFOF_URLENCODESTGMEDDATA = 0x1,
 BINDINFOF_URLENCODEDEXTRAINFO = 0x2
 } BINDINFOF;
typedef
enum __MIDL_IBindStatusCallback_0003
 { BINDF_ASYNCHRONOUS = 0x1,
 BINDF_ASYNCSTORAGE = 0x2,
 BINDF_NOPROGRESSIVERENDERING = 0x4,
 BINDF_OFFLINEOPERATION = 0x8,
 BINDF_GETNEWESTVERSION = 0x10,
 BINDF_NOWRITECACHE = 0x20,
 BINDF_NEEDFILE = 0x40,
 BINDF_PULLDATA = 0x80,
 BINDF_IGNORESECURITYPROBLEM = 0x100,
 BINDF_RESYNCHRONIZE = 0x200,
 BINDF_HYPERLINK = 0x400,
 BINDF_NO_UI = 0x800,
 BINDF_SILENTOPERATION = 0x1000,
 BINDF_PRAGMA_NO_CACHE = 0x2000,
 BINDF_GETCLASSOBJECT = 0x4000,
 BINDF_RESERVED_1 = 0x8000,
 BINDF_FREE_THREADED = 0x10000,
 BINDF_DIRECT_READ = 0x20000,
 BINDF_FORMS_SUBMIT = 0x40000,
 BINDF_GETFROMCACHE_IF_NET_FAIL = 0x80000,
 BINDF_FROMURLMON = 0x100000,
 BINDF_FWD_BACK = 0x200000,
 BINDF_PREFERDEFAULTHANDLER = 0x400000,
 BINDF_ENFORCERESTRICTED = 0x800000
 } BINDF;
typedef
enum __MIDL_IBindStatusCallback_0004
 { URL_ENCODING_NONE = 0,
 URL_ENCODING_ENABLE_UTF8 = 0x10000000,
 URL_ENCODING_DISABLE_UTF8 = 0x20000000
 } URL_ENCODING;
typedef struct _tagBINDINFO
 {
 ULONG cbSize;
 LPWSTR szExtraInfo;
 STGMEDIUM stgmedData;
 DWORD grfBindInfoF;
 DWORD dwBindVerb;
 LPWSTR szCustomVerb;
 DWORD cbstgmedData;
 DWORD dwOptions;
 DWORD dwOptionsFlags;
 DWORD dwCodePage;
 SECURITY_ATTRIBUTES securityAttributes;
 IID iid;
 IUnknown *pUnk;
 DWORD dwReserved;
 } BINDINFO;
typedef struct _REMSECURITY_ATTRIBUTES
 {
 DWORD nLength;
 DWORD lpSecurityDescriptor;
 BOOL bInheritHandle;
 } REMSECURITY_ATTRIBUTES;
typedef struct _REMSECURITY_ATTRIBUTES *PREMSECURITY_ATTRIBUTES;
typedef struct _REMSECURITY_ATTRIBUTES *LPREMSECURITY_ATTRIBUTES;
typedef struct _tagRemBINDINFO
 {
 ULONG cbSize;
 LPWSTR szExtraInfo;
 DWORD grfBindInfoF;
 DWORD dwBindVerb;
 LPWSTR szCustomVerb;
 DWORD cbstgmedData;
 DWORD dwOptions;
 DWORD dwOptionsFlags;
 DWORD dwCodePage;
 REMSECURITY_ATTRIBUTES securityAttributes;
 IID iid;
 IUnknown *pUnk;
 DWORD dwReserved;
 } RemBINDINFO;
typedef struct tagRemFORMATETC
 {
 DWORD cfFormat;
 DWORD ptd;
 DWORD dwAspect;
 LONG lindex;
 DWORD tymed;
 } RemFORMATETC;
typedef struct tagRemFORMATETC *LPREMFORMATETC;
typedef
enum __MIDL_IBindStatusCallback_0005
 { BINDINFO_OPTIONS_WININETFLAG = 0x10000,
 BINDINFO_OPTIONS_ENABLE_UTF8 = 0x20000,
 BINDINFO_OPTIONS_DISABLE_UTF8 = 0x40000,
 BINDINFO_OPTIONS_USE_IE_ENCODING = 0x80000,
 BINDINFO_OPTIONS_BINDTOOBJECT = 0x100000,
 BINDINFO_OPTIONS_SECURITYOPTOUT = 0x200000,
 BINDINFO_OPTIONS_IGNOREMIMETEXTPLAIN = 0x400000,
 BINDINFO_OPTIONS_USEBINDSTRINGCREDS = 0x800000,
 BINDINFO_OPTIONS_IGNOREHTTPHTTPSREDIRECTS = 0x1000000,
 BINDINFO_OPTIONS_IGNORE_SSLERRORS_ONCE = 0x2000000,
 BINDINFO_WPC_DOWNLOADBLOCKED = 0x8000000,
 BINDINFO_WPC_LOGGING_ENABLED = 0x10000000,
 BINDINFO_OPTIONS_DISABLEAUTOREDIRECTS = 0x40000000,
 BINDINFO_OPTIONS_SHDOCVW_NAVIGATE = 0x80000000
 } BINDINFO_OPTIONS;
typedef
enum __MIDL_IBindStatusCallback_0006
 { BSCF_FIRSTDATANOTIFICATION = 0x1,
 BSCF_INTERMEDIATEDATANOTIFICATION = 0x2,
 BSCF_LASTDATANOTIFICATION = 0x4,
 BSCF_DATAFULLYAVAILABLE = 0x8,
 BSCF_AVAILABLEDATASIZEUNKNOWN = 0x10
 } BSCF;
typedef
enum tagBINDSTATUS
 { BINDSTATUS_FINDINGRESOURCE = 1,
 BINDSTATUS_CONNECTING = ( BINDSTATUS_FINDINGRESOURCE + 1 ) ,
 BINDSTATUS_REDIRECTING = ( BINDSTATUS_CONNECTING + 1 ) ,
 BINDSTATUS_BEGINDOWNLOADDATA = ( BINDSTATUS_REDIRECTING + 1 ) ,
 BINDSTATUS_DOWNLOADINGDATA = ( BINDSTATUS_BEGINDOWNLOADDATA + 1 ) ,
 BINDSTATUS_ENDDOWNLOADDATA = ( BINDSTATUS_DOWNLOADINGDATA + 1 ) ,
 BINDSTATUS_BEGINDOWNLOADCOMPONENTS = ( BINDSTATUS_ENDDOWNLOADDATA + 1 ) ,
 BINDSTATUS_INSTALLINGCOMPONENTS = ( BINDSTATUS_BEGINDOWNLOADCOMPONENTS + 1 ) ,
 BINDSTATUS_ENDDOWNLOADCOMPONENTS = ( BINDSTATUS_INSTALLINGCOMPONENTS + 1 ) ,
 BINDSTATUS_USINGCACHEDCOPY = ( BINDSTATUS_ENDDOWNLOADCOMPONENTS + 1 ) ,
 BINDSTATUS_SENDINGREQUEST = ( BINDSTATUS_USINGCACHEDCOPY + 1 ) ,
 BINDSTATUS_CLASSIDAVAILABLE = ( BINDSTATUS_SENDINGREQUEST + 1 ) ,
 BINDSTATUS_MIMETYPEAVAILABLE = ( BINDSTATUS_CLASSIDAVAILABLE + 1 ) ,
 BINDSTATUS_CACHEFILENAMEAVAILABLE = ( BINDSTATUS_MIMETYPEAVAILABLE + 1 ) ,
 BINDSTATUS_BEGINSYNCOPERATION = ( BINDSTATUS_CACHEFILENAMEAVAILABLE + 1 ) ,
 BINDSTATUS_ENDSYNCOPERATION = ( BINDSTATUS_BEGINSYNCOPERATION + 1 ) ,
 BINDSTATUS_BEGINUPLOADDATA = ( BINDSTATUS_ENDSYNCOPERATION + 1 ) ,
 BINDSTATUS_UPLOADINGDATA = ( BINDSTATUS_BEGINUPLOADDATA + 1 ) ,
 BINDSTATUS_ENDUPLOADDATA = ( BINDSTATUS_UPLOADINGDATA + 1 ) ,
 BINDSTATUS_PROTOCOLCLASSID = ( BINDSTATUS_ENDUPLOADDATA + 1 ) ,
 BINDSTATUS_ENCODING = ( BINDSTATUS_PROTOCOLCLASSID + 1 ) ,
 BINDSTATUS_VERIFIEDMIMETYPEAVAILABLE = ( BINDSTATUS_ENCODING + 1 ) ,
 BINDSTATUS_CLASSINSTALLLOCATION = ( BINDSTATUS_VERIFIEDMIMETYPEAVAILABLE + 1 ) ,
 BINDSTATUS_DECODING = ( BINDSTATUS_CLASSINSTALLLOCATION + 1 ) ,
 BINDSTATUS_LOADINGMIMEHANDLER = ( BINDSTATUS_DECODING + 1 ) ,
 BINDSTATUS_CONTENTDISPOSITIONATTACH = ( BINDSTATUS_LOADINGMIMEHANDLER + 1 ) ,
 BINDSTATUS_FILTERREPORTMIMETYPE = ( BINDSTATUS_CONTENTDISPOSITIONATTACH + 1 ) ,
 BINDSTATUS_CLSIDCANINSTANTIATE = ( BINDSTATUS_FILTERREPORTMIMETYPE + 1 ) ,
 BINDSTATUS_IUNKNOWNAVAILABLE = ( BINDSTATUS_CLSIDCANINSTANTIATE + 1 ) ,
 BINDSTATUS_DIRECTBIND = ( BINDSTATUS_IUNKNOWNAVAILABLE + 1 ) ,
 BINDSTATUS_RAWMIMETYPE = ( BINDSTATUS_DIRECTBIND + 1 ) ,
 BINDSTATUS_PROXYDETECTING = ( BINDSTATUS_RAWMIMETYPE + 1 ) ,
 BINDSTATUS_ACCEPTRANGES = ( BINDSTATUS_PROXYDETECTING + 1 ) ,
 BINDSTATUS_COOKIE_SENT = ( BINDSTATUS_ACCEPTRANGES + 1 ) ,
 BINDSTATUS_COMPACT_POLICY_RECEIVED = ( BINDSTATUS_COOKIE_SENT + 1 ) ,
 BINDSTATUS_COOKIE_SUPPRESSED = ( BINDSTATUS_COMPACT_POLICY_RECEIVED + 1 ) ,
 BINDSTATUS_COOKIE_STATE_UNKNOWN = ( BINDSTATUS_COOKIE_SUPPRESSED + 1 ) ,
 BINDSTATUS_COOKIE_STATE_ACCEPT = ( BINDSTATUS_COOKIE_STATE_UNKNOWN + 1 ) ,
 BINDSTATUS_COOKIE_STATE_REJECT = ( BINDSTATUS_COOKIE_STATE_ACCEPT + 1 ) ,
 BINDSTATUS_COOKIE_STATE_PROMPT = ( BINDSTATUS_COOKIE_STATE_REJECT + 1 ) ,
 BINDSTATUS_COOKIE_STATE_LEASH = ( BINDSTATUS_COOKIE_STATE_PROMPT + 1 ) ,
 BINDSTATUS_COOKIE_STATE_DOWNGRADE = ( BINDSTATUS_COOKIE_STATE_LEASH + 1 ) ,
 BINDSTATUS_POLICY_HREF = ( BINDSTATUS_COOKIE_STATE_DOWNGRADE + 1 ) ,
 BINDSTATUS_P3P_HEADER = ( BINDSTATUS_POLICY_HREF + 1 ) ,
 BINDSTATUS_SESSION_COOKIE_RECEIVED = ( BINDSTATUS_P3P_HEADER + 1 ) ,
 BINDSTATUS_PERSISTENT_COOKIE_RECEIVED = ( BINDSTATUS_SESSION_COOKIE_RECEIVED + 1 ) ,
 BINDSTATUS_SESSION_COOKIES_ALLOWED = ( BINDSTATUS_PERSISTENT_COOKIE_RECEIVED + 1 ) ,
 BINDSTATUS_CACHECONTROL = ( BINDSTATUS_SESSION_COOKIES_ALLOWED + 1 ) ,
 BINDSTATUS_CONTENTDISPOSITIONFILENAME = ( BINDSTATUS_CACHECONTROL + 1 ) ,
 BINDSTATUS_MIMETEXTPLAINMISMATCH = ( BINDSTATUS_CONTENTDISPOSITIONFILENAME + 1 ) ,
 BINDSTATUS_PUBLISHERAVAILABLE = ( BINDSTATUS_MIMETEXTPLAINMISMATCH + 1 ) ,
 BINDSTATUS_DISPLAYNAMEAVAILABLE = ( BINDSTATUS_PUBLISHERAVAILABLE + 1 ) ,
 BINDSTATUS_SSLUX_NAVBLOCKED = ( BINDSTATUS_DISPLAYNAMEAVAILABLE + 1 )
 } BINDSTATUS;
extern "C" const IID IID_IBindStatusCallback;
 struct __declspec(uuid("79eac9c1-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IBindStatusCallback : public IUnknown
 {
 public:
 virtual HRESULT __stdcall OnStartBinding(
 DWORD dwReserved,
 IBinding *pib) = 0;
 virtual HRESULT __stdcall GetPriority(
 LONG *pnPriority) = 0;
 virtual HRESULT __stdcall OnLowResource(
 DWORD reserved) = 0;
 virtual HRESULT __stdcall OnProgress(
 ULONG ulProgress,
 ULONG ulProgressMax,
 ULONG ulStatusCode,
 LPCWSTR szStatusText) = 0;
 virtual HRESULT __stdcall OnStopBinding(
 HRESULT hresult,
 LPCWSTR szError) = 0;
 virtual HRESULT __stdcall GetBindInfo(
 DWORD *grfBINDF,
 BINDINFO *pbindinfo) = 0;
 virtual HRESULT __stdcall OnDataAvailable(
 DWORD grfBSCF,
 DWORD dwSize,
 FORMATETC *pformatetc,
 STGMEDIUM *pstgmed) = 0;
 virtual HRESULT __stdcall OnObjectAvailable(
 const IID & riid,
 IUnknown *punk) = 0;
 };
 HRESULT __stdcall IBindStatusCallback_RemoteGetBindInfo_Proxy(
 IBindStatusCallback * This,
 DWORD *grfBINDF,
 RemBINDINFO *pbindinfo,
 RemSTGMEDIUM *pstgmed);
void __stdcall IBindStatusCallback_RemoteGetBindInfo_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IBindStatusCallback_RemoteOnDataAvailable_Proxy(
 IBindStatusCallback * This,
 DWORD grfBSCF,
 DWORD dwSize,
 RemFORMATETC *pformatetc,
 RemSTGMEDIUM *pstgmed);
void __stdcall IBindStatusCallback_RemoteOnDataAvailable_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0005_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0005_v0_0_s_ifspec;
typedef IAuthenticate *LPAUTHENTICATION;
extern "C" const IID IID_IAuthenticate;
 struct __declspec(uuid("79eac9d0-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IAuthenticate : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Authenticate(
 HWND *phwnd,
 LPWSTR *pszUsername,
 LPWSTR *pszPassword) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0006_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0006_v0_0_s_ifspec;
typedef IHttpNegotiate *LPHTTPNEGOTIATE;
extern "C" const IID IID_IHttpNegotiate;
 struct __declspec(uuid("79eac9d2-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IHttpNegotiate : public IUnknown
 {
 public:
 virtual HRESULT __stdcall BeginningTransaction(
 LPCWSTR szURL,
 LPCWSTR szHeaders,
 DWORD dwReserved,
 LPWSTR *pszAdditionalHeaders) = 0;
 virtual HRESULT __stdcall OnResponse(
 DWORD dwResponseCode,
 LPCWSTR szResponseHeaders,
 LPCWSTR szRequestHeaders,
 LPWSTR *pszAdditionalRequestHeaders) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0007_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0007_v0_0_s_ifspec;
typedef IHttpNegotiate2 *LPHTTPNEGOTIATE2;
extern "C" const IID IID_IHttpNegotiate2;
 struct __declspec(uuid("4F9F9FCB-E0F4-48eb-B7AB-FA2EA9365CB4")) __declspec(novtable)
 IHttpNegotiate2 : public IHttpNegotiate
 {
 public:
 virtual HRESULT __stdcall GetRootSecurityId(
 BYTE *pbSecurityId,
 DWORD *pcbSecurityId,
 DWORD_PTR dwReserved) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0008_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0008_v0_0_s_ifspec;
typedef IWinInetFileStream *LPWININETFILESTREAM;
extern "C" const IID IID_IWinInetFileStream;
 struct __declspec(uuid("F134C4B7-B1F8-4e75-B886-74B90943BECB")) __declspec(novtable)
 IWinInetFileStream : public IUnknown
 {
 public:
 virtual HRESULT __stdcall SetHandleForUnlock(
 DWORD_PTR hWinInetLockHandle,
 DWORD_PTR dwReserved) = 0;
 virtual HRESULT __stdcall SetDeleteFile(
 DWORD_PTR dwReserved) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0009_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0009_v0_0_s_ifspec;
typedef IWindowForBindingUI *LPWINDOWFORBINDINGUI;
extern "C" const IID IID_IWindowForBindingUI;
 struct __declspec(uuid("79eac9d5-bafa-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IWindowForBindingUI : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetWindow(
 const GUID & rguidReason,
 HWND *phwnd) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0010_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0010_v0_0_s_ifspec;
typedef ICodeInstall *LPCODEINSTALL;
typedef
enum __MIDL_ICodeInstall_0001
 { CIP_DISK_FULL = 0,
 CIP_ACCESS_DENIED = ( CIP_DISK_FULL + 1 ) ,
 CIP_NEWER_VERSION_EXISTS = ( CIP_ACCESS_DENIED + 1 ) ,
 CIP_OLDER_VERSION_EXISTS = ( CIP_NEWER_VERSION_EXISTS + 1 ) ,
 CIP_NAME_CONFLICT = ( CIP_OLDER_VERSION_EXISTS + 1 ) ,
 CIP_TRUST_VERIFICATION_COMPONENT_MISSING = ( CIP_NAME_CONFLICT + 1 ) ,
 CIP_EXE_SELF_REGISTERATION_TIMEOUT = ( CIP_TRUST_VERIFICATION_COMPONENT_MISSING + 1 ) ,
 CIP_UNSAFE_TO_ABORT = ( CIP_EXE_SELF_REGISTERATION_TIMEOUT + 1 ) ,
 CIP_NEED_REBOOT = ( CIP_UNSAFE_TO_ABORT + 1 ) ,
 CIP_NEED_REBOOT_UI_PERMISSION = ( CIP_NEED_REBOOT + 1 )
 } CIP_STATUS;
extern "C" const IID IID_ICodeInstall;
 struct __declspec(uuid("79eac9d1-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 ICodeInstall : public IWindowForBindingUI
 {
 public:
 virtual HRESULT __stdcall OnCodeInstallProblem(
 ULONG ulStatusCode,
 LPCWSTR szDestination,
 LPCWSTR szSource,
 DWORD dwReserved) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0011_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0011_v0_0_s_ifspec;
typedef
enum __MIDL_IUri_0001
 { Uri_PROPERTY_ABSOLUTE_URI = 0,
 Uri_PROPERTY_STRING_START = Uri_PROPERTY_ABSOLUTE_URI,
 Uri_PROPERTY_AUTHORITY = ( Uri_PROPERTY_STRING_START + 1 ) ,
 Uri_PROPERTY_DISPLAY_URI = ( Uri_PROPERTY_AUTHORITY + 1 ) ,
 Uri_PROPERTY_DOMAIN = ( Uri_PROPERTY_DISPLAY_URI + 1 ) ,
 Uri_PROPERTY_EXTENSION = ( Uri_PROPERTY_DOMAIN + 1 ) ,
 Uri_PROPERTY_FRAGMENT = ( Uri_PROPERTY_EXTENSION + 1 ) ,
 Uri_PROPERTY_HOST = ( Uri_PROPERTY_FRAGMENT + 1 ) ,
 Uri_PROPERTY_PASSWORD = ( Uri_PROPERTY_HOST + 1 ) ,
 Uri_PROPERTY_PATH = ( Uri_PROPERTY_PASSWORD + 1 ) ,
 Uri_PROPERTY_PATH_AND_QUERY = ( Uri_PROPERTY_PATH + 1 ) ,
 Uri_PROPERTY_QUERY = ( Uri_PROPERTY_PATH_AND_QUERY + 1 ) ,
 Uri_PROPERTY_RAW_URI = ( Uri_PROPERTY_QUERY + 1 ) ,
 Uri_PROPERTY_SCHEME_NAME = ( Uri_PROPERTY_RAW_URI + 1 ) ,
 Uri_PROPERTY_USER_INFO = ( Uri_PROPERTY_SCHEME_NAME + 1 ) ,
 Uri_PROPERTY_USER_NAME = ( Uri_PROPERTY_USER_INFO + 1 ) ,
 Uri_PROPERTY_STRING_LAST = Uri_PROPERTY_USER_NAME,
 Uri_PROPERTY_HOST_TYPE = ( Uri_PROPERTY_STRING_LAST + 1 ) ,
 Uri_PROPERTY_DWORD_START = Uri_PROPERTY_HOST_TYPE,
 Uri_PROPERTY_PORT = ( Uri_PROPERTY_DWORD_START + 1 ) ,
 Uri_PROPERTY_SCHEME = ( Uri_PROPERTY_PORT + 1 ) ,
 Uri_PROPERTY_ZONE = ( Uri_PROPERTY_SCHEME + 1 ) ,
 Uri_PROPERTY_DWORD_LAST = Uri_PROPERTY_ZONE
 } Uri_PROPERTY;
typedef
enum __MIDL_IUri_0002
 { Uri_HOST_UNKNOWN = 0,
 Uri_HOST_DNS = ( Uri_HOST_UNKNOWN + 1 ) ,
 Uri_HOST_IPV4 = ( Uri_HOST_DNS + 1 ) ,
 Uri_HOST_IPV6 = ( Uri_HOST_IPV4 + 1 ) ,
 Uri_HOST_IDN = ( Uri_HOST_IPV6 + 1 )
 } Uri_HOST_TYPE;
extern "C" const IID IID_IUri;
 struct __declspec(uuid("A39EE748-6A27-4817-A6F2-13914BEF5890")) __declspec(novtable)
 IUri : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetPropertyBSTR(
 Uri_PROPERTY uriProp,
 BSTR *pbstrProperty,
 DWORD dwFlags) = 0;
 virtual HRESULT __stdcall GetPropertyLength(
 Uri_PROPERTY uriProp,
 DWORD *pcchProperty,
 DWORD dwFlags) = 0;
 virtual HRESULT __stdcall GetPropertyDWORD(
 Uri_PROPERTY uriProp,
 DWORD *pdwProperty,
 DWORD dwFlags) = 0;
 virtual HRESULT __stdcall HasProperty(
 Uri_PROPERTY uriProp,
 BOOL *pfHasProperty) = 0;
 virtual HRESULT __stdcall GetAbsoluteUri(
 BSTR *pbstrAbsoluteUri) = 0;
 virtual HRESULT __stdcall GetAuthority(
 BSTR *pbstrAuthority) = 0;
 virtual HRESULT __stdcall GetDisplayUri(
 BSTR *pbstrDisplayString) = 0;
 virtual HRESULT __stdcall GetDomain(
 BSTR *pbstrDomain) = 0;
 virtual HRESULT __stdcall GetExtension(
 BSTR *pbstrExtension) = 0;
 virtual HRESULT __stdcall GetFragment(
 BSTR *pbstrFragment) = 0;
 virtual HRESULT __stdcall GetHost(
 BSTR *pbstrHost) = 0;
 virtual HRESULT __stdcall GetPassword(
 BSTR *pbstrPassword) = 0;
 virtual HRESULT __stdcall GetPath(
 BSTR *pbstrPath) = 0;
 virtual HRESULT __stdcall GetPathAndQuery(
 BSTR *pbstrPathAndQuery) = 0;
 virtual HRESULT __stdcall GetQuery(
 BSTR *pbstrQuery) = 0;
 virtual HRESULT __stdcall GetRawUri(
 BSTR *pbstrRawUri) = 0;
 virtual HRESULT __stdcall GetSchemeName(
 BSTR *pbstrSchemeName) = 0;
 virtual HRESULT __stdcall GetUserInfo(
 BSTR *pbstrUserInfo) = 0;
 virtual HRESULT __stdcall GetUserNameA(
 BSTR *pbstrUserName) = 0;
 virtual HRESULT __stdcall GetHostType(
 DWORD *pdwHostType) = 0;
 virtual HRESULT __stdcall GetPort(
 DWORD *pdwPort) = 0;
 virtual HRESULT __stdcall GetScheme(
 DWORD *pdwScheme) = 0;
 virtual HRESULT __stdcall GetZone(
 DWORD *pdwZone) = 0;
 virtual HRESULT __stdcall GetProperties(
 LPDWORD pdwFlags) = 0;
 virtual HRESULT __stdcall IsEqual(
 IUri *pUri,
 BOOL *pfEqual) = 0;
 };
extern "C" HRESULT __stdcall CreateUri( LPCWSTR pwzURI,
 DWORD dwFlags,
 DWORD_PTR dwReserved,
 IUri** ppURI);
extern "C" HRESULT __stdcall CreateUriWithFragment(
 LPCWSTR pwzURI,
 LPCWSTR pwzFragment,
 DWORD dwFlags,
 DWORD_PTR dwReserved,
 IUri** ppURI);
extern "C" HRESULT __stdcall CreateUriFromMultiByteString(
 LPCSTR pszANSIInputUri,
 DWORD dwEncodingFlags,
 DWORD dwCodePage,
 DWORD dwCreateFlags,
 DWORD_PTR dwReserved,
 IUri** ppUri);
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0012_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0012_v0_0_s_ifspec;
extern "C" const IID IID_IUriContainer;
 struct __declspec(uuid("a158a630-ed6f-45fb-b987-f68676f57752")) __declspec(novtable)
 IUriContainer : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetIUri(
 IUri **ppIUri) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0013_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0013_v0_0_s_ifspec;
extern "C" const IID IID_IUriBuilder;
 struct __declspec(uuid("4221B2E1-8955-46c0-BD5B-DE9897565DE7")) __declspec(novtable)
 IUriBuilder : public IUnknown
 {
 public:
 virtual HRESULT __stdcall CreateUriSimple(
 DWORD dwAllowEncodingPropertyMask,
 DWORD_PTR dwReserved,
 IUri **ppIUri) = 0;
 virtual HRESULT __stdcall CreateUri(
 DWORD dwCreateFlags,
 DWORD dwAllowEncodingPropertyMask,
 DWORD_PTR dwReserved,
 IUri **ppIUri) = 0;
 virtual HRESULT __stdcall CreateUriWithFlags(
 DWORD dwCreateFlags,
 DWORD dwUriBuilderFlags,
 DWORD dwAllowEncodingPropertyMask,
 DWORD_PTR dwReserved,
 IUri **ppIUri) = 0;
 virtual HRESULT __stdcall GetIUri(
 IUri **ppIUri) = 0;
 virtual HRESULT __stdcall SetIUri(
 IUri *pIUri) = 0;
 virtual HRESULT __stdcall GetFragment(
 DWORD *pcchFragment,
 LPCWSTR *ppwzFragment) = 0;
 virtual HRESULT __stdcall GetHost(
 DWORD *pcchHost,
 LPCWSTR *ppwzHost) = 0;
 virtual HRESULT __stdcall GetPassword(
 DWORD *pcchPassword,
 LPCWSTR *ppwzPassword) = 0;
 virtual HRESULT __stdcall GetPath(
 DWORD *pcchPath,
 LPCWSTR *ppwzPath) = 0;
 virtual HRESULT __stdcall GetPort(
 BOOL *pfHasPort,
 DWORD *pdwPort) = 0;
 virtual HRESULT __stdcall GetQuery(
 DWORD *pcchQuery,
 LPCWSTR *ppwzQuery) = 0;
 virtual HRESULT __stdcall GetSchemeName(
 DWORD *pcchSchemeName,
 LPCWSTR *ppwzSchemeName) = 0;
 virtual HRESULT __stdcall GetUserNameA(
 DWORD *pcchUserName,
 LPCWSTR *ppwzUserName) = 0;
 virtual HRESULT __stdcall SetFragment(
 LPCWSTR pwzNewValue) = 0;
 virtual HRESULT __stdcall SetHost(
 LPCWSTR pwzNewValue) = 0;
 virtual HRESULT __stdcall SetPassword(
 LPCWSTR pwzNewValue) = 0;
 virtual HRESULT __stdcall SetPath(
 LPCWSTR pwzNewValue) = 0;
 virtual HRESULT __stdcall SetPort(
 BOOL fHasPort,
 DWORD dwNewValue) = 0;
 virtual HRESULT __stdcall SetQuery(
 LPCWSTR pwzNewValue) = 0;
 virtual HRESULT __stdcall SetSchemeName(
 LPCWSTR pwzNewValue) = 0;
 virtual HRESULT __stdcall SetUserName(
 LPCWSTR pwzNewValue) = 0;
 virtual HRESULT __stdcall RemoveProperties(
 DWORD dwPropertyMask) = 0;
 virtual HRESULT __stdcall HasBeenModified(
 BOOL *pfModified) = 0;
 };
extern "C" const IID IID_IUriBuilderFactory;
 struct __declspec(uuid("E982CE48-0B96-440c-BC37-0C869B27A29E")) __declspec(novtable)
 IUriBuilderFactory : public IUnknown
 {
 public:
 virtual HRESULT __stdcall CreateIUriBuilder(
 DWORD dwFlags,
 DWORD_PTR dwReserved,
 IUriBuilder **ppIUriBuilder) = 0;
 virtual HRESULT __stdcall CreateInitializedIUriBuilder(
 DWORD dwFlags,
 DWORD_PTR dwReserved,
 IUriBuilder **ppIUriBuilder) = 0;
 };
extern "C" HRESULT __stdcall CreateIUriBuilder(
 IUri *pIUri,
 DWORD dwFlags,
 DWORD_PTR dwReserved,
 IUriBuilder **ppIUriBuilder
 );
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0015_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0015_v0_0_s_ifspec;
typedef IWinInetInfo *LPWININETINFO;
extern "C" const IID IID_IWinInetInfo;
 struct __declspec(uuid("79eac9d6-bafa-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IWinInetInfo : public IUnknown
 {
 public:
 virtual HRESULT __stdcall QueryOption(
 DWORD dwOption,
 LPVOID pBuffer,
 DWORD *pcbBuf) = 0;
 };
 HRESULT __stdcall IWinInetInfo_RemoteQueryOption_Proxy(
 IWinInetInfo * This,
 DWORD dwOption,
 BYTE *pBuffer,
 DWORD *pcbBuf);
void __stdcall IWinInetInfo_RemoteQueryOption_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0016_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0016_v0_0_s_ifspec;
typedef IHttpSecurity *LPHTTPSECURITY;
extern "C" const IID IID_IHttpSecurity;
 struct __declspec(uuid("79eac9d7-bafa-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IHttpSecurity : public IWindowForBindingUI
 {
 public:
 virtual HRESULT __stdcall OnSecurityProblem(
 DWORD dwProblem) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0017_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0017_v0_0_s_ifspec;
typedef IWinInetHttpInfo *LPWININETHTTPINFO;
extern "C" const IID IID_IWinInetHttpInfo;
 struct __declspec(uuid("79eac9d8-bafa-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IWinInetHttpInfo : public IWinInetInfo
 {
 public:
 virtual HRESULT __stdcall QueryInfo(
 DWORD dwOption,
 LPVOID pBuffer,
 DWORD *pcbBuf,
 DWORD *pdwFlags,
 DWORD *pdwReserved) = 0;
 };
 HRESULT __stdcall IWinInetHttpInfo_RemoteQueryInfo_Proxy(
 IWinInetHttpInfo * This,
 DWORD dwOption,
 BYTE *pBuffer,
 DWORD *pcbBuf,
 DWORD *pdwFlags,
 DWORD *pdwReserved);
void __stdcall IWinInetHttpInfo_RemoteQueryInfo_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0018_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0018_v0_0_s_ifspec;
typedef IWinInetCacheHints *LPWININETCACHEHINTS;
extern "C" const IID IID_IWinInetCacheHints;
 struct __declspec(uuid("DD1EC3B3-8391-4fdb-A9E6-347C3CAAA7DD")) __declspec(novtable)
 IWinInetCacheHints : public IUnknown
 {
 public:
 virtual HRESULT __stdcall SetCacheExtension(
 LPCWSTR pwzExt,
 LPVOID pszCacheFile,
 DWORD *pcbCacheFile,
 DWORD *pdwWinInetError,
 DWORD *pdwReserved) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0019_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0019_v0_0_s_ifspec;
typedef IWinInetCacheHints2 *LPWININETCACHEHINTS2;
extern "C" const IID IID_IWinInetCacheHints2;
 struct __declspec(uuid("7857AEAC-D31F-49bf-884E-DD46DF36780A")) __declspec(novtable)
 IWinInetCacheHints2 : public IWinInetCacheHints
 {
 public:
 virtual HRESULT __stdcall SetCacheExtension2(
 LPCWSTR pwzExt,
 WCHAR *pwzCacheFile,
 DWORD *pcchCacheFile,
 DWORD *pdwWinInetError,
 DWORD *pdwReserved) = 0;
 };
extern "C" const GUID SID_BindHost;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0020_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0020_v0_0_s_ifspec;
typedef IBindHost *LPBINDHOST;
extern "C" const IID IID_IBindHost;
 struct __declspec(uuid("fc4801a1-2ba9-11cf-a229-00aa003d7352")) __declspec(novtable)
 IBindHost : public IUnknown
 {
 public:
 virtual HRESULT __stdcall CreateMoniker(
 LPOLESTR szName,
 IBindCtx *pBC,
 IMoniker **ppmk,
 DWORD dwReserved) = 0;
 virtual HRESULT __stdcall MonikerBindToStorage(
 IMoniker *pMk,
 IBindCtx *pBC,
 IBindStatusCallback *pBSC,
 const IID & riid,
 void **ppvObj) = 0;
 virtual HRESULT __stdcall MonikerBindToObject(
 IMoniker *pMk,
 IBindCtx *pBC,
 IBindStatusCallback *pBSC,
 const IID & riid,
 void **ppvObj) = 0;
 };
 HRESULT __stdcall IBindHost_RemoteMonikerBindToStorage_Proxy(
 IBindHost * This,
 IMoniker *pMk,
 IBindCtx *pBC,
 IBindStatusCallback *pBSC,
 const IID & riid,
 IUnknown **ppvObj);
void __stdcall IBindHost_RemoteMonikerBindToStorage_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
 HRESULT __stdcall IBindHost_RemoteMonikerBindToObject_Proxy(
 IBindHost * This,
 IMoniker *pMk,
 IBindCtx *pBC,
 IBindStatusCallback *pBSC,
 const IID & riid,
 IUnknown **ppvObj);
void __stdcall IBindHost_RemoteMonikerBindToObject_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
struct IBindStatusCallback;
extern "C" HRESULT __stdcall HlinkSimpleNavigateToString(
 LPCWSTR szTarget,
 LPCWSTR szLocation,
 LPCWSTR szTargetFrameName,
 IUnknown *pUnk,
 IBindCtx *pbc,
 IBindStatusCallback *,
 DWORD grfHLNF,
 DWORD dwReserved
);
extern "C" HRESULT __stdcall HlinkSimpleNavigateToMoniker(
 IMoniker *pmkTarget,
 LPCWSTR szLocation,
 LPCWSTR szTargetFrameName,
 IUnknown *pUnk,
 IBindCtx *pbc,
 IBindStatusCallback *,
 DWORD grfHLNF,
 DWORD dwReserved
);
extern "C" HRESULT __stdcall URLOpenStreamA(LPUNKNOWN,LPCSTR,DWORD,LPBINDSTATUSCALLBACK);
extern "C" HRESULT __stdcall URLOpenStreamW(LPUNKNOWN,LPCWSTR,DWORD,LPBINDSTATUSCALLBACK);
extern "C" HRESULT __stdcall URLOpenPullStreamA(LPUNKNOWN,LPCSTR,DWORD,LPBINDSTATUSCALLBACK);
extern "C" HRESULT __stdcall URLOpenPullStreamW(LPUNKNOWN,LPCWSTR,DWORD,LPBINDSTATUSCALLBACK);
extern "C" HRESULT __stdcall URLDownloadToFileA(LPUNKNOWN,LPCSTR,LPCSTR,DWORD,LPBINDSTATUSCALLBACK);
extern "C" HRESULT __stdcall URLDownloadToFileW(LPUNKNOWN,LPCWSTR,LPCWSTR,DWORD,LPBINDSTATUSCALLBACK);
extern "C" HRESULT __stdcall URLDownloadToCacheFileA( LPUNKNOWN, LPCSTR, LPSTR, DWORD cchFileName, DWORD, LPBINDSTATUSCALLBACK);
extern "C" HRESULT __stdcall URLDownloadToCacheFileW( LPUNKNOWN, LPCWSTR, LPWSTR, DWORD cchFileName, DWORD, LPBINDSTATUSCALLBACK);
extern "C" HRESULT __stdcall URLOpenBlockingStreamA(LPUNKNOWN,LPCSTR,LPSTREAM*,DWORD,LPBINDSTATUSCALLBACK);
extern "C" HRESULT __stdcall URLOpenBlockingStreamW(LPUNKNOWN,LPCWSTR,LPSTREAM*,DWORD,LPBINDSTATUSCALLBACK);
extern "C" HRESULT __stdcall HlinkGoBack(IUnknown *pUnk);
extern "C" HRESULT __stdcall HlinkGoForward(IUnknown *pUnk);
extern "C" HRESULT __stdcall HlinkNavigateString(IUnknown *pUnk, LPCWSTR szTarget);
extern "C" HRESULT __stdcall HlinkNavigateMoniker(IUnknown *pUnk, IMoniker *pmkTarget);
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0021_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0021_v0_0_s_ifspec;
typedef IInternet *LPIINTERNET;
extern "C" const IID IID_IInternet;
 struct __declspec(uuid("79eac9e0-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IInternet : public IUnknown
 {
 public:
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0022_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0022_v0_0_s_ifspec;
typedef IInternetBindInfo *LPIINTERNETBINDINFO;
typedef
enum tagBINDSTRING
 { BINDSTRING_HEADERS = 1,
 BINDSTRING_ACCEPT_MIMES = ( BINDSTRING_HEADERS + 1 ) ,
 BINDSTRING_EXTRA_URL = ( BINDSTRING_ACCEPT_MIMES + 1 ) ,
 BINDSTRING_LANGUAGE = ( BINDSTRING_EXTRA_URL + 1 ) ,
 BINDSTRING_USERNAME = ( BINDSTRING_LANGUAGE + 1 ) ,
 BINDSTRING_PASSWORD = ( BINDSTRING_USERNAME + 1 ) ,
 BINDSTRING_UA_PIXELS = ( BINDSTRING_PASSWORD + 1 ) ,
 BINDSTRING_UA_COLOR = ( BINDSTRING_UA_PIXELS + 1 ) ,
 BINDSTRING_OS = ( BINDSTRING_UA_COLOR + 1 ) ,
 BINDSTRING_USER_AGENT = ( BINDSTRING_OS + 1 ) ,
 BINDSTRING_ACCEPT_ENCODINGS = ( BINDSTRING_USER_AGENT + 1 ) ,
 BINDSTRING_POST_COOKIE = ( BINDSTRING_ACCEPT_ENCODINGS + 1 ) ,
 BINDSTRING_POST_DATA_MIME = ( BINDSTRING_POST_COOKIE + 1 ) ,
 BINDSTRING_URL = ( BINDSTRING_POST_DATA_MIME + 1 ) ,
 BINDSTRING_IID = ( BINDSTRING_URL + 1 ) ,
 BINDSTRING_FLAG_BIND_TO_OBJECT = ( BINDSTRING_IID + 1 ) ,
 BINDSTRING_PTR_BIND_CONTEXT = ( BINDSTRING_FLAG_BIND_TO_OBJECT + 1 )
 } BINDSTRING;
extern "C" const IID IID_IInternetBindInfo;
 struct __declspec(uuid("79eac9e1-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IInternetBindInfo : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetBindInfo(
 DWORD *grfBINDF,
 BINDINFO *pbindinfo) = 0;
 virtual HRESULT __stdcall GetBindString(
 ULONG ulStringType,
 LPOLESTR *ppwzStr,
 ULONG cEl,
 ULONG *pcElFetched) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0023_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0023_v0_0_s_ifspec;
typedef IInternetProtocolRoot *LPIINTERNETPROTOCOLROOT;
typedef
enum _tagPI_FLAGS
 { PI_PARSE_URL = 0x1,
 PI_FILTER_MODE = 0x2,
 PI_FORCE_ASYNC = 0x4,
 PI_USE_WORKERTHREAD = 0x8,
 PI_MIMEVERIFICATION = 0x10,
 PI_CLSIDLOOKUP = 0x20,
 PI_DATAPROGRESS = 0x40,
 PI_SYNCHRONOUS = 0x80,
 PI_APARTMENTTHREADED = 0x100,
 PI_CLASSINSTALL = 0x200,
 PI_PASSONBINDCTX = 0x2000,
 PI_NOMIMEHANDLER = 0x8000,
 PI_LOADAPPDIRECT = 0x4000,
 PD_FORCE_SWITCH = 0x10000,
 PI_PREFERDEFAULTHANDLER = 0x20000
 } PI_FLAGS;
typedef struct _tagPROTOCOLDATA
 {
 DWORD grfFlags;
 DWORD dwState;
 LPVOID pData;
 ULONG cbData;
 } PROTOCOLDATA;
typedef struct _tagStartParam
 {
 IID iid;
 IBindCtx *pIBindCtx;
 IUnknown *pItf;
 } StartParam;
extern "C" const IID IID_IInternetProtocolRoot;
 struct __declspec(uuid("79eac9e3-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IInternetProtocolRoot : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Start(
 LPCWSTR szUrl,
 IInternetProtocolSink *pOIProtSink,
 IInternetBindInfo *pOIBindInfo,
 DWORD grfPI,
 HANDLE_PTR dwReserved) = 0;
 virtual HRESULT __stdcall Continue(
 PROTOCOLDATA *pProtocolData) = 0;
 virtual HRESULT __stdcall Abort(
 HRESULT hrReason,
 DWORD dwOptions) = 0;
 virtual HRESULT __stdcall Terminate(
 DWORD dwOptions) = 0;
 virtual HRESULT __stdcall Suspend( void) = 0;
 virtual HRESULT __stdcall Resume( void) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0024_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0024_v0_0_s_ifspec;
typedef IInternetProtocol *LPIINTERNETPROTOCOL;
extern "C" const IID IID_IInternetProtocol;
 struct __declspec(uuid("79eac9e4-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IInternetProtocol : public IInternetProtocolRoot
 {
 public:
 virtual HRESULT __stdcall Read(
 void *pv,
 ULONG cb,
 ULONG *pcbRead) = 0;
 virtual HRESULT __stdcall Seek(
 LARGE_INTEGER dlibMove,
 DWORD dwOrigin,
 ULARGE_INTEGER *plibNewPosition) = 0;
 virtual HRESULT __stdcall LockRequest(
 DWORD dwOptions) = 0;
 virtual HRESULT __stdcall UnlockRequest( void) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0025_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0025_v0_0_s_ifspec;
extern "C" const IID IID_IInternetProtocolEx;
 struct __declspec(uuid("C7A98E66-1010-492c-A1C8-C809E1F75905")) __declspec(novtable)
 IInternetProtocolEx : public IInternetProtocol
 {
 public:
 virtual HRESULT __stdcall StartEx(
 IUri *pUri,
 IInternetProtocolSink *pOIProtSink,
 IInternetBindInfo *pOIBindInfo,
 DWORD grfPI,
 HANDLE_PTR dwReserved) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0026_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0026_v0_0_s_ifspec;
typedef IInternetProtocolSink *LPIINTERNETPROTOCOLSINK;
extern "C" const IID IID_IInternetProtocolSink;
 struct __declspec(uuid("79eac9e5-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IInternetProtocolSink : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Switch(
 PROTOCOLDATA *pProtocolData) = 0;
 virtual HRESULT __stdcall ReportProgress(
 ULONG ulStatusCode,
 LPCWSTR szStatusText) = 0;
 virtual HRESULT __stdcall ReportData(
 DWORD grfBSCF,
 ULONG ulProgress,
 ULONG ulProgressMax) = 0;
 virtual HRESULT __stdcall ReportResult(
 HRESULT hrResult,
 DWORD dwError,
 LPCWSTR szResult) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0027_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0027_v0_0_s_ifspec;
typedef IInternetProtocolSinkStackable *LPIINTERNETPROTOCOLSINKStackable;
extern "C" const IID IID_IInternetProtocolSinkStackable;
 struct __declspec(uuid("79eac9f0-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IInternetProtocolSinkStackable : public IUnknown
 {
 public:
 virtual HRESULT __stdcall SwitchSink(
 IInternetProtocolSink *pOIProtSink) = 0;
 virtual HRESULT __stdcall CommitSwitch( void) = 0;
 virtual HRESULT __stdcall RollbackSwitch( void) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0028_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0028_v0_0_s_ifspec;
typedef IInternetSession *LPIINTERNETSESSION;
typedef
enum _tagOIBDG_FLAGS
 { OIBDG_APARTMENTTHREADED = 0x100,
 OIBDG_DATAONLY = 0x1000
 } OIBDG_FLAGS;
extern "C" const IID IID_IInternetSession;
 struct __declspec(uuid("79eac9e7-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IInternetSession : public IUnknown
 {
 public:
 virtual HRESULT __stdcall RegisterNameSpace(
 IClassFactory *pCF,
 const IID & rclsid,
 LPCWSTR pwzProtocol,
 ULONG cPatterns,
 const LPCWSTR *ppwzPatterns,
 DWORD dwReserved) = 0;
 virtual HRESULT __stdcall UnregisterNameSpace(
 IClassFactory *pCF,
 LPCWSTR pszProtocol) = 0;
 virtual HRESULT __stdcall RegisterMimeFilter(
 IClassFactory *pCF,
 const IID & rclsid,
 LPCWSTR pwzType) = 0;
 virtual HRESULT __stdcall UnregisterMimeFilter(
 IClassFactory *pCF,
 LPCWSTR pwzType) = 0;
 virtual HRESULT __stdcall CreateBinding(
 LPBC pBC,
 LPCWSTR szUrl,
 IUnknown *pUnkOuter,
 IUnknown **ppUnk,
 IInternetProtocol **ppOInetProt,
 DWORD dwOption) = 0;
 virtual HRESULT __stdcall SetSessionOption(
 DWORD dwOption,
 LPVOID pBuffer,
 DWORD dwBufferLength,
 DWORD dwReserved) = 0;
 virtual HRESULT __stdcall GetSessionOption(
 DWORD dwOption,
 LPVOID pBuffer,
 DWORD *pdwBufferLength,
 DWORD dwReserved) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0029_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0029_v0_0_s_ifspec;
typedef IInternetThreadSwitch *LPIINTERNETTHREADSWITCH;
extern "C" const IID IID_IInternetThreadSwitch;
 struct __declspec(uuid("79eac9e8-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IInternetThreadSwitch : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Prepare( void) = 0;
 virtual HRESULT __stdcall Continue( void) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0030_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0030_v0_0_s_ifspec;
typedef IInternetPriority *LPIINTERNETPRIORITY;
extern "C" const IID IID_IInternetPriority;
 struct __declspec(uuid("79eac9eb-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IInternetPriority : public IUnknown
 {
 public:
 virtual HRESULT __stdcall SetPriority(
 LONG nPriority) = 0;
 virtual HRESULT __stdcall GetPriority(
 LONG *pnPriority) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0031_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0031_v0_0_s_ifspec;
typedef IInternetProtocolInfo *LPIINTERNETPROTOCOLINFO;
typedef
enum _tagPARSEACTION
 { PARSE_CANONICALIZE = 1,
 PARSE_FRIENDLY = ( PARSE_CANONICALIZE + 1 ) ,
 PARSE_SECURITY_URL = ( PARSE_FRIENDLY + 1 ) ,
 PARSE_ROOTDOCUMENT = ( PARSE_SECURITY_URL + 1 ) ,
 PARSE_DOCUMENT = ( PARSE_ROOTDOCUMENT + 1 ) ,
 PARSE_ANCHOR = ( PARSE_DOCUMENT + 1 ) ,
 PARSE_ENCODE = ( PARSE_ANCHOR + 1 ) ,
 PARSE_DECODE = ( PARSE_ENCODE + 1 ) ,
 PARSE_PATH_FROM_URL = ( PARSE_DECODE + 1 ) ,
 PARSE_URL_FROM_PATH = ( PARSE_PATH_FROM_URL + 1 ) ,
 PARSE_MIME = ( PARSE_URL_FROM_PATH + 1 ) ,
 PARSE_SERVER = ( PARSE_MIME + 1 ) ,
 PARSE_SCHEMA = ( PARSE_SERVER + 1 ) ,
 PARSE_SITE = ( PARSE_SCHEMA + 1 ) ,
 PARSE_DOMAIN = ( PARSE_SITE + 1 ) ,
 PARSE_LOCATION = ( PARSE_DOMAIN + 1 ) ,
 PARSE_SECURITY_DOMAIN = ( PARSE_LOCATION + 1 ) ,
 PARSE_ESCAPE = ( PARSE_SECURITY_DOMAIN + 1 ) ,
 PARSE_UNESCAPE = ( PARSE_ESCAPE + 1 )
 } PARSEACTION;
typedef
enum _tagPSUACTION
 { PSU_DEFAULT = 1,
 PSU_SECURITY_URL_ONLY = ( PSU_DEFAULT + 1 )
 } PSUACTION;
typedef
enum _tagQUERYOPTION
 { QUERY_EXPIRATION_DATE = 1,
 QUERY_TIME_OF_LAST_CHANGE = ( QUERY_EXPIRATION_DATE + 1 ) ,
 QUERY_CONTENT_ENCODING = ( QUERY_TIME_OF_LAST_CHANGE + 1 ) ,
 QUERY_CONTENT_TYPE = ( QUERY_CONTENT_ENCODING + 1 ) ,
 QUERY_REFRESH = ( QUERY_CONTENT_TYPE + 1 ) ,
 QUERY_RECOMBINE = ( QUERY_REFRESH + 1 ) ,
 QUERY_CAN_NAVIGATE = ( QUERY_RECOMBINE + 1 ) ,
 QUERY_USES_NETWORK = ( QUERY_CAN_NAVIGATE + 1 ) ,
 QUERY_IS_CACHED = ( QUERY_USES_NETWORK + 1 ) ,
 QUERY_IS_INSTALLEDENTRY = ( QUERY_IS_CACHED + 1 ) ,
 QUERY_IS_CACHED_OR_MAPPED = ( QUERY_IS_INSTALLEDENTRY + 1 ) ,
 QUERY_USES_CACHE = ( QUERY_IS_CACHED_OR_MAPPED + 1 ) ,
 QUERY_IS_SECURE = ( QUERY_USES_CACHE + 1 ) ,
 QUERY_IS_SAFE = ( QUERY_IS_SECURE + 1 ) ,
 QUERY_USES_HISTORYFOLDER = ( QUERY_IS_SAFE + 1 )
 } QUERYOPTION;
extern "C" const IID IID_IInternetProtocolInfo;
 struct __declspec(uuid("79eac9ec-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IInternetProtocolInfo : public IUnknown
 {
 public:
 virtual HRESULT __stdcall ParseUrl(
 LPCWSTR pwzUrl,
 PARSEACTION ParseAction,
 DWORD dwParseFlags,
 LPWSTR pwzResult,
 DWORD cchResult,
 DWORD *pcchResult,
 DWORD dwReserved) = 0;
 virtual HRESULT __stdcall CombineUrl(
 LPCWSTR pwzBaseUrl,
 LPCWSTR pwzRelativeUrl,
 DWORD dwCombineFlags,
 LPWSTR pwzResult,
 DWORD cchResult,
 DWORD *pcchResult,
 DWORD dwReserved) = 0;
 virtual HRESULT __stdcall CompareUrl(
 LPCWSTR pwzUrl1,
 LPCWSTR pwzUrl2,
 DWORD dwCompareFlags) = 0;
 virtual HRESULT __stdcall QueryInfo(
 LPCWSTR pwzUrl,
 QUERYOPTION OueryOption,
 DWORD dwQueryFlags,
 LPVOID pBuffer,
 DWORD cbBuffer,
 DWORD *pcbBuf,
 DWORD dwReserved) = 0;
 };
extern "C" HRESULT __stdcall CoInternetParseUrl(
 LPCWSTR pwzUrl,
 PARSEACTION ParseAction,
 DWORD dwFlags,
 LPWSTR pszResult,
 DWORD cchResult,
 DWORD *pcchResult,
 DWORD dwReserved
 );
extern "C" HRESULT __stdcall CoInternetParseIUri(
 IUri *pIUri,
 PARSEACTION ParseAction,
 DWORD dwFlags,
 LPWSTR pwzResult,
 DWORD cchResult,
 DWORD *pcchResult,
 DWORD_PTR dwReserved
 );
extern "C" HRESULT __stdcall CoInternetCombineUrl(
 LPCWSTR pwzBaseUrl,
 LPCWSTR pwzRelativeUrl,
 DWORD dwCombineFlags,
 LPWSTR pszResult,
 DWORD cchResult,
 DWORD *pcchResult,
 DWORD dwReserved
 );
extern "C" HRESULT __stdcall CoInternetCombineUrlEx(
 IUri *pBaseUri,
 LPCWSTR pwzRelativeUrl,
 DWORD dwCombineFlags,
 IUri **ppCombinedUri,
 DWORD_PTR dwReserved
 );
extern "C" HRESULT __stdcall CoInternetCombineIUri (
 IUri *pBaseUri,
 IUri *pRelativeUri,
 DWORD dwCombineFlags,
 IUri **ppCombinedUri,
 DWORD_PTR dwReserved
 );
extern "C" HRESULT __stdcall CoInternetCompareUrl(
 LPCWSTR pwzUrl1,
 LPCWSTR pwzUrl2,
 DWORD dwFlags
 );
extern "C" HRESULT __stdcall CoInternetGetProtocolFlags(
 LPCWSTR pwzUrl,
 DWORD *pdwFlags,
 DWORD dwReserved
 );
extern "C" HRESULT __stdcall CoInternetQueryInfo(
 LPCWSTR pwzUrl,
 QUERYOPTION QueryOptions,
 DWORD dwQueryFlags,
 LPVOID pvBuffer,
 DWORD cbBuffer,
 DWORD *pcbBuffer,
 DWORD dwReserved
 );
extern "C" HRESULT __stdcall CoInternetGetSession(
 DWORD dwSessionMode,
 IInternetSession **ppIInternetSession,
 DWORD dwReserved
 );
extern "C" HRESULT __stdcall CoInternetGetSecurityUrl(
 LPCWSTR pwszUrl,
 LPWSTR *ppwszSecUrl,
 PSUACTION psuAction,
 DWORD dwReserved
 );
extern "C" HRESULT __stdcall AsyncInstallDistributionUnit(
 LPCWSTR szDistUnit,
 LPCWSTR szTYPE,
 LPCWSTR szExt,
 DWORD dwFileVersionMS,
 DWORD dwFileVersionLS,
 LPCWSTR szURL,
 IBindCtx *pbc,
 LPVOID pvReserved,
 DWORD flags
 );
extern "C" HRESULT __stdcall CoInternetGetSecurityUrlEx(
 IUri *pUri,
 IUri **ppSecUri,
 PSUACTION psuAction,
 DWORD_PTR dwReserved
 );
typedef
enum _tagINTERNETFEATURELIST
 { FEATURE_OBJECT_CACHING = 0,
 FEATURE_ZONE_ELEVATION = ( FEATURE_OBJECT_CACHING + 1 ) ,
 FEATURE_MIME_HANDLING = ( FEATURE_ZONE_ELEVATION + 1 ) ,
 FEATURE_MIME_SNIFFING = ( FEATURE_MIME_HANDLING + 1 ) ,
 FEATURE_WINDOW_RESTRICTIONS = ( FEATURE_MIME_SNIFFING + 1 ) ,
 FEATURE_WEBOC_POPUPMANAGEMENT = ( FEATURE_WINDOW_RESTRICTIONS + 1 ) ,
 FEATURE_BEHAVIORS = ( FEATURE_WEBOC_POPUPMANAGEMENT + 1 ) ,
 FEATURE_DISABLE_MK_PROTOCOL = ( FEATURE_BEHAVIORS + 1 ) ,
 FEATURE_LOCALMACHINE_LOCKDOWN = ( FEATURE_DISABLE_MK_PROTOCOL + 1 ) ,
 FEATURE_SECURITYBAND = ( FEATURE_LOCALMACHINE_LOCKDOWN + 1 ) ,
 FEATURE_RESTRICT_ACTIVEXINSTALL = ( FEATURE_SECURITYBAND + 1 ) ,
 FEATURE_VALIDATE_NAVIGATE_URL = ( FEATURE_RESTRICT_ACTIVEXINSTALL + 1 ) ,
 FEATURE_RESTRICT_FILEDOWNLOAD = ( FEATURE_VALIDATE_NAVIGATE_URL + 1 ) ,
 FEATURE_ADDON_MANAGEMENT = ( FEATURE_RESTRICT_FILEDOWNLOAD + 1 ) ,
 FEATURE_PROTOCOL_LOCKDOWN = ( FEATURE_ADDON_MANAGEMENT + 1 ) ,
 FEATURE_HTTP_USERNAME_PASSWORD_DISABLE = ( FEATURE_PROTOCOL_LOCKDOWN + 1 ) ,
 FEATURE_SAFE_BINDTOOBJECT = ( FEATURE_HTTP_USERNAME_PASSWORD_DISABLE + 1 ) ,
 FEATURE_UNC_SAVEDFILECHECK = ( FEATURE_SAFE_BINDTOOBJECT + 1 ) ,
 FEATURE_GET_URL_DOM_FILEPATH_UNENCODED = ( FEATURE_UNC_SAVEDFILECHECK + 1 ) ,
 FEATURE_TABBED_BROWSING = ( FEATURE_GET_URL_DOM_FILEPATH_UNENCODED + 1 ) ,
 FEATURE_SSLUX = ( FEATURE_TABBED_BROWSING + 1 ) ,
 FEATURE_DISABLE_NAVIGATION_SOUNDS = ( FEATURE_SSLUX + 1 ) ,
 FEATURE_DISABLE_LEGACY_COMPRESSION = ( FEATURE_DISABLE_NAVIGATION_SOUNDS + 1 ) ,
 FEATURE_FORCE_ADDR_AND_STATUS = ( FEATURE_DISABLE_LEGACY_COMPRESSION + 1 ) ,
 FEATURE_XMLHTTP = ( FEATURE_FORCE_ADDR_AND_STATUS + 1 ) ,
 FEATURE_DISABLE_TELNET_PROTOCOL = ( FEATURE_XMLHTTP + 1 ) ,
 FEATURE_FEEDS = ( FEATURE_DISABLE_TELNET_PROTOCOL + 1 ) ,
 FEATURE_BLOCK_INPUT_PROMPTS = ( FEATURE_FEEDS + 1 ) ,
 FEATURE_ENTRY_COUNT = ( FEATURE_BLOCK_INPUT_PROMPTS + 1 )
 } INTERNETFEATURELIST;
extern "C" HRESULT __stdcall CoInternetSetFeatureEnabled(
 INTERNETFEATURELIST FeatureEntry,
 DWORD dwFlags,
 BOOL fEnable
 );
extern "C" HRESULT __stdcall CoInternetIsFeatureEnabled(
 INTERNETFEATURELIST FeatureEntry,
 DWORD dwFlags
 );
extern "C" HRESULT __stdcall CoInternetIsFeatureEnabledForUrl(
 INTERNETFEATURELIST FeatureEntry,
 DWORD dwFlags,
 LPCWSTR szURL,
 IInternetSecurityManager *pSecMgr
 );
extern "C" HRESULT __stdcall CoInternetIsFeatureEnabledForIUri(
 INTERNETFEATURELIST FeatureEntry,
 DWORD dwFlags,
 IUri * pIUri,
 IInternetSecurityManagerEx2 *pSecMgr
 );
extern "C" HRESULT __stdcall CoInternetIsFeatureZoneElevationEnabled(
 LPCWSTR szFromURL,
 LPCWSTR szToURL,
 IInternetSecurityManager *pSecMgr,
 DWORD dwFlags
 );
extern "C" HRESULT __stdcall CopyStgMedium(const STGMEDIUM * pcstgmedSrc,
 STGMEDIUM * pstgmedDest);
extern "C" HRESULT __stdcall CopyBindInfo( const BINDINFO * pcbiSrc,
 BINDINFO * pbiDest );
extern "C" void __stdcall ReleaseBindInfo( BINDINFO * pbindinfo );
extern "C" HRESULT __stdcall CoInternetCreateSecurityManager(IServiceProvider *pSP, IInternetSecurityManager **ppSM, DWORD dwReserved);
extern "C" HRESULT __stdcall CoInternetCreateZoneManager(IServiceProvider *pSP, IInternetZoneManager **ppZM, DWORD dwReserved);
extern "C" const IID CLSID_InternetSecurityManager;
extern "C" const IID CLSID_InternetZoneManager;
extern "C" const IID CLSID_PersistentZoneIdentifier;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0032_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0032_v0_0_s_ifspec;
extern "C" const IID IID_IInternetSecurityMgrSite;
 struct __declspec(uuid("79eac9ed-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IInternetSecurityMgrSite : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetWindow(
 HWND *phwnd) = 0;
 virtual HRESULT __stdcall EnableModeless(
 BOOL fEnable) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0033_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0033_v0_0_s_ifspec;
typedef
enum __MIDL_IInternetSecurityManager_0001
 { PUAF_DEFAULT = 0,
 PUAF_NOUI = 0x1,
 PUAF_ISFILE = 0x2,
 PUAF_WARN_IF_DENIED = 0x4,
 PUAF_FORCEUI_FOREGROUND = 0x8,
 PUAF_CHECK_TIFS = 0x10,
 PUAF_DONTCHECKBOXINDIALOG = 0x20,
 PUAF_TRUSTED = 0x40,
 PUAF_ACCEPT_WILDCARD_SCHEME = 0x80,
 PUAF_ENFORCERESTRICTED = 0x100,
 PUAF_NOSAVEDFILECHECK = 0x200,
 PUAF_REQUIRESAVEDFILECHECK = 0x400,
 PUAF_DONT_USE_CACHE = 0x1000,
 PUAF_RESERVED1 = 0x2000,
 PUAF_RESERVED2 = 0x4000,
 PUAF_LMZ_UNLOCKED = 0x10000,
 PUAF_LMZ_LOCKED = 0x20000,
 PUAF_DEFAULTZONEPOL = 0x40000,
 PUAF_NPL_USE_LOCKED_IF_RESTRICTED = 0x80000,
 PUAF_NOUIIFLOCKED = 0x100000,
 PUAF_DRAGPROTOCOLCHECK = 0x200000
 } PUAF;
typedef
enum __MIDL_IInternetSecurityManager_0002
 { PUAFOUT_DEFAULT = 0,
 PUAFOUT_ISLOCKZONEPOLICY = 0x1
 } PUAFOUT;
typedef
enum __MIDL_IInternetSecurityManager_0003
 { SZM_CREATE = 0,
 SZM_DELETE = 0x1
 } SZM_FLAGS;
extern "C" const IID IID_IInternetSecurityManager;
 struct __declspec(uuid("79eac9ee-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IInternetSecurityManager : public IUnknown
 {
 public:
 virtual HRESULT __stdcall SetSecuritySite(
 IInternetSecurityMgrSite *pSite) = 0;
 virtual HRESULT __stdcall GetSecuritySite(
 IInternetSecurityMgrSite **ppSite) = 0;
 virtual HRESULT __stdcall MapUrlToZone(
 LPCWSTR pwszUrl,
 DWORD *pdwZone,
 DWORD dwFlags) = 0;
 virtual HRESULT __stdcall GetSecurityId(
 LPCWSTR pwszUrl,
 BYTE *pbSecurityId,
 DWORD *pcbSecurityId,
 DWORD_PTR dwReserved) = 0;
 virtual HRESULT __stdcall ProcessUrlAction(
 LPCWSTR pwszUrl,
 DWORD dwAction,
 BYTE *pPolicy,
 DWORD cbPolicy,
 BYTE *pContext,
 DWORD cbContext,
 DWORD dwFlags,
 DWORD dwReserved) = 0;
 virtual HRESULT __stdcall QueryCustomPolicy(
 LPCWSTR pwszUrl,
 const GUID & guidKey,
 BYTE **ppPolicy,
 DWORD *pcbPolicy,
 BYTE *pContext,
 DWORD cbContext,
 DWORD dwReserved) = 0;
 virtual HRESULT __stdcall SetZoneMapping(
 DWORD dwZone,
 LPCWSTR lpszPattern,
 DWORD dwFlags) = 0;
 virtual HRESULT __stdcall GetZoneMappings(
 DWORD dwZone,
 IEnumString **ppenumString,
 DWORD dwFlags) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0034_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0034_v0_0_s_ifspec;
extern "C" const IID IID_IInternetSecurityManagerEx;
 struct __declspec(uuid("F164EDF1-CC7C-4f0d-9A94-34222625C393")) __declspec(novtable)
 IInternetSecurityManagerEx : public IInternetSecurityManager
 {
 public:
 virtual HRESULT __stdcall ProcessUrlActionEx(
 LPCWSTR pwszUrl,
 DWORD dwAction,
 BYTE *pPolicy,
 DWORD cbPolicy,
 BYTE *pContext,
 DWORD cbContext,
 DWORD dwFlags,
 DWORD dwReserved,
 DWORD *pdwOutFlags) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0035_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0035_v0_0_s_ifspec;
extern "C" const IID IID_IInternetSecurityManagerEx2;
 struct __declspec(uuid("F1E50292-A795-4117-8E09-2B560A72AC60")) __declspec(novtable)
 IInternetSecurityManagerEx2 : public IInternetSecurityManagerEx
 {
 public:
 virtual HRESULT __stdcall MapUrlToZoneEx2(
 IUri *pUri,
 DWORD *pdwZone,
 DWORD dwFlags,
 LPWSTR *ppwszMappedUrl,
 DWORD *pdwOutFlags) = 0;
 virtual HRESULT __stdcall ProcessUrlActionEx2(
 IUri *pUri,
 DWORD dwAction,
 BYTE *pPolicy,
 DWORD cbPolicy,
 BYTE *pContext,
 DWORD cbContext,
 DWORD dwFlags,
 DWORD_PTR dwReserved,
 DWORD *pdwOutFlags) = 0;
 virtual HRESULT __stdcall GetSecurityIdEx2(
 IUri *pUri,
 BYTE *pbSecurityId,
 DWORD *pcbSecurityId,
 DWORD_PTR dwReserved) = 0;
 virtual HRESULT __stdcall QueryCustomPolicyEx2(
 IUri *pUri,
 const GUID & guidKey,
 BYTE **ppPolicy,
 DWORD *pcbPolicy,
 BYTE *pContext,
 DWORD cbContext,
 DWORD_PTR dwReserved) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0036_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0036_v0_0_s_ifspec;
extern "C" const IID IID_IZoneIdentifier;
 struct __declspec(uuid("cd45f185-1b21-48e2-967b-ead743a8914e")) __declspec(novtable)
 IZoneIdentifier : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetId(
 DWORD *pdwZone) = 0;
 virtual HRESULT __stdcall SetId(
 DWORD dwZone) = 0;
 virtual HRESULT __stdcall Remove( void) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0037_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0037_v0_0_s_ifspec;
extern "C" const IID IID_IInternetHostSecurityManager;
 struct __declspec(uuid("3af280b6-cb3f-11d0-891e-00c04fb6bfc4")) __declspec(novtable)
 IInternetHostSecurityManager : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetSecurityId(
 BYTE *pbSecurityId,
 DWORD *pcbSecurityId,
 DWORD_PTR dwReserved) = 0;
 virtual HRESULT __stdcall ProcessUrlAction(
 DWORD dwAction,
 BYTE *pPolicy,
 DWORD cbPolicy,
 BYTE *pContext,
 DWORD cbContext,
 DWORD dwFlags,
 DWORD dwReserved) = 0;
 virtual HRESULT __stdcall QueryCustomPolicy(
 const GUID & guidKey,
 BYTE **ppPolicy,
 DWORD *pcbPolicy,
 BYTE *pContext,
 DWORD cbContext,
 DWORD dwReserved) = 0;
 };
extern "C" const GUID GUID_CUSTOM_LOCALMACHINEZONEUNLOCKED;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0038_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0038_v0_0_s_ifspec;
typedef IInternetZoneManager *LPURLZONEMANAGER;
typedef
enum tagURLZONE
 { URLZONE_INVALID = -1,
 URLZONE_PREDEFINED_MIN = 0,
 URLZONE_LOCAL_MACHINE = 0,
 URLZONE_INTRANET = ( URLZONE_LOCAL_MACHINE + 1 ) ,
 URLZONE_TRUSTED = ( URLZONE_INTRANET + 1 ) ,
 URLZONE_INTERNET = ( URLZONE_TRUSTED + 1 ) ,
 URLZONE_UNTRUSTED = ( URLZONE_INTERNET + 1 ) ,
 URLZONE_PREDEFINED_MAX = 999,
 URLZONE_USER_MIN = 1000,
 URLZONE_USER_MAX = 10000
 } URLZONE;
typedef
enum tagURLTEMPLATE
 { URLTEMPLATE_CUSTOM = 0,
 URLTEMPLATE_PREDEFINED_MIN = 0x10000,
 URLTEMPLATE_LOW = 0x10000,
 URLTEMPLATE_MEDLOW = 0x10500,
 URLTEMPLATE_MEDIUM = 0x11000,
 URLTEMPLATE_MEDHIGH = 0x11500,
 URLTEMPLATE_HIGH = 0x12000,
 URLTEMPLATE_PREDEFINED_MAX = 0x20000
 } URLTEMPLATE;
enum __MIDL_IInternetZoneManager_0001
 { MAX_ZONE_PATH = 260,
 MAX_ZONE_DESCRIPTION = 200
 } ;
typedef
enum __MIDL_IInternetZoneManager_0002
 { ZAFLAGS_CUSTOM_EDIT = 0x1,
 ZAFLAGS_ADD_SITES = 0x2,
 ZAFLAGS_REQUIRE_VERIFICATION = 0x4,
 ZAFLAGS_INCLUDE_PROXY_OVERRIDE = 0x8,
 ZAFLAGS_INCLUDE_INTRANET_SITES = 0x10,
 ZAFLAGS_NO_UI = 0x20,
 ZAFLAGS_SUPPORTS_VERIFICATION = 0x40,
 ZAFLAGS_UNC_AS_INTRANET = 0x80,
 ZAFLAGS_DETECT_INTRANET = 0x100,
 ZAFLAGS_USE_LOCKED_ZONES = 0x10000,
 ZAFLAGS_VERIFY_TEMPLATE_SETTINGS = 0x20000,
 ZAFLAGS_NO_CACHE = 0x40000
 } ZAFLAGS;
typedef struct _ZONEATTRIBUTES
 {
 ULONG cbSize;
 WCHAR szDisplayName[ 260 ];
 WCHAR szDescription[ 200 ];
 WCHAR szIconPath[ 260 ];
 DWORD dwTemplateMinLevel;
 DWORD dwTemplateRecommended;
 DWORD dwTemplateCurrentLevel;
 DWORD dwFlags;
 } ZONEATTRIBUTES;
typedef struct _ZONEATTRIBUTES *LPZONEATTRIBUTES;
typedef
enum _URLZONEREG
 { URLZONEREG_DEFAULT = 0,
 URLZONEREG_HKLM = ( URLZONEREG_DEFAULT + 1 ) ,
 URLZONEREG_HKCU = ( URLZONEREG_HKLM + 1 )
 } URLZONEREG;
extern "C" const IID IID_IInternetZoneManager;
 struct __declspec(uuid("79eac9ef-baf9-11ce-8c82-00aa004ba90b")) __declspec(novtable)
 IInternetZoneManager : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetZoneAttributes(
 DWORD dwZone,
 ZONEATTRIBUTES *pZoneAttributes) = 0;
 virtual HRESULT __stdcall SetZoneAttributes(
 DWORD dwZone,
 ZONEATTRIBUTES *pZoneAttributes) = 0;
 virtual HRESULT __stdcall GetZoneCustomPolicy(
 DWORD dwZone,
 const GUID & guidKey,
 BYTE **ppPolicy,
 DWORD *pcbPolicy,
 URLZONEREG urlZoneReg) = 0;
 virtual HRESULT __stdcall SetZoneCustomPolicy(
 DWORD dwZone,
 const GUID & guidKey,
 BYTE *pPolicy,
 DWORD cbPolicy,
 URLZONEREG urlZoneReg) = 0;
 virtual HRESULT __stdcall GetZoneActionPolicy(
 DWORD dwZone,
 DWORD dwAction,
 BYTE *pPolicy,
 DWORD cbPolicy,
 URLZONEREG urlZoneReg) = 0;
 virtual HRESULT __stdcall SetZoneActionPolicy(
 DWORD dwZone,
 DWORD dwAction,
 BYTE *pPolicy,
 DWORD cbPolicy,
 URLZONEREG urlZoneReg) = 0;
 virtual HRESULT __stdcall PromptAction(
 DWORD dwAction,
 HWND hwndParent,
 LPCWSTR pwszUrl,
 LPCWSTR pwszText,
 DWORD dwPromptFlags) = 0;
 virtual HRESULT __stdcall LogAction(
 DWORD dwAction,
 LPCWSTR pwszUrl,
 LPCWSTR pwszText,
 DWORD dwLogFlags) = 0;
 virtual HRESULT __stdcall CreateZoneEnumerator(
 DWORD *pdwEnum,
 DWORD *pdwCount,
 DWORD dwFlags) = 0;
 virtual HRESULT __stdcall GetZoneAt(
 DWORD dwEnum,
 DWORD dwIndex,
 DWORD *pdwZone) = 0;
 virtual HRESULT __stdcall DestroyZoneEnumerator(
 DWORD dwEnum) = 0;
 virtual HRESULT __stdcall CopyTemplatePoliciesToZone(
 DWORD dwTemplate,
 DWORD dwZone,
 DWORD dwReserved) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0039_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0039_v0_0_s_ifspec;
extern "C" const IID IID_IInternetZoneManagerEx;
 struct __declspec(uuid("A4C23339-8E06-431e-9BF4-7E711C085648")) __declspec(novtable)
 IInternetZoneManagerEx : public IInternetZoneManager
 {
 public:
 virtual HRESULT __stdcall GetZoneActionPolicyEx(
 DWORD dwZone,
 DWORD dwAction,
 BYTE *pPolicy,
 DWORD cbPolicy,
 URLZONEREG urlZoneReg,
 DWORD dwFlags) = 0;
 virtual HRESULT __stdcall SetZoneActionPolicyEx(
 DWORD dwZone,
 DWORD dwAction,
 BYTE *pPolicy,
 DWORD cbPolicy,
 URLZONEREG urlZoneReg,
 DWORD dwFlags) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0040_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0040_v0_0_s_ifspec;
extern "C" const IID IID_IInternetZoneManagerEx2;
 struct __declspec(uuid("EDC17559-DD5D-4846-8EEF-8BECBA5A4ABF")) __declspec(novtable)
 IInternetZoneManagerEx2 : public IInternetZoneManagerEx
 {
 public:
 virtual HRESULT __stdcall GetZoneAttributesEx(
 DWORD dwZone,
 ZONEATTRIBUTES *pZoneAttributes,
 DWORD dwFlags) = 0;
 virtual HRESULT __stdcall GetZoneSecurityState(
 DWORD dwZoneIndex,
 BOOL fRespectPolicy,
 LPDWORD pdwState,
 BOOL *pfPolicyEncountered) = 0;
 virtual HRESULT __stdcall GetIESecurityState(
 BOOL fRespectPolicy,
 LPDWORD pdwState,
 BOOL *pfPolicyEncountered,
 BOOL fNoCache) = 0;
 virtual HRESULT __stdcall FixUnsecureSettings( void) = 0;
 };
extern "C" const IID CLSID_SoftDistExt;
typedef struct _tagCODEBASEHOLD
 {
 ULONG cbSize;
 LPWSTR szDistUnit;
 LPWSTR szCodeBase;
 DWORD dwVersionMS;
 DWORD dwVersionLS;
 DWORD dwStyle;
 } CODEBASEHOLD;
typedef struct _tagCODEBASEHOLD *LPCODEBASEHOLD;
typedef struct _tagSOFTDISTINFO
 {
 ULONG cbSize;
 DWORD dwFlags;
 DWORD dwAdState;
 LPWSTR szTitle;
 LPWSTR szAbstract;
 LPWSTR szHREF;
 DWORD dwInstalledVersionMS;
 DWORD dwInstalledVersionLS;
 DWORD dwUpdateVersionMS;
 DWORD dwUpdateVersionLS;
 DWORD dwAdvertisedVersionMS;
 DWORD dwAdvertisedVersionLS;
 DWORD dwReserved;
 } SOFTDISTINFO;
typedef struct _tagSOFTDISTINFO *LPSOFTDISTINFO;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0041_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0041_v0_0_s_ifspec;
extern "C" const IID IID_ISoftDistExt;
 struct __declspec(uuid("B15B8DC1-C7E1-11d0-8680-00AA00BDCB71")) __declspec(novtable)
 ISoftDistExt : public IUnknown
 {
 public:
 virtual HRESULT __stdcall ProcessSoftDist(
 LPCWSTR szCDFURL,
 IXMLElement *pSoftDistElement,
 LPSOFTDISTINFO lpsdi) = 0;
 virtual HRESULT __stdcall GetFirstCodeBase(
 LPWSTR *szCodeBase,
 LPDWORD dwMaxSize) = 0;
 virtual HRESULT __stdcall GetNextCodeBase(
 LPWSTR *szCodeBase,
 LPDWORD dwMaxSize) = 0;
 virtual HRESULT __stdcall AsyncInstallDistributionUnit(
 IBindCtx *pbc,
 LPVOID pvReserved,
 DWORD flags,
 LPCODEBASEHOLD lpcbh) = 0;
 };
extern "C" HRESULT __stdcall GetSoftwareUpdateInfo( LPCWSTR szDistUnit, LPSOFTDISTINFO psdi );
extern "C" HRESULT __stdcall SetSoftwareUpdateAdvertisementState( LPCWSTR szDistUnit, DWORD dwAdState, DWORD dwAdvertisedVersionMS, DWORD dwAdvertisedVersionLS );
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0042_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0042_v0_0_s_ifspec;
typedef ICatalogFileInfo *LPCATALOGFILEINFO;
extern "C" const IID IID_ICatalogFileInfo;
 struct __declspec(uuid("711C7600-6B48-11d1-B403-00AA00B92AF1")) __declspec(novtable)
 ICatalogFileInfo : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetCatalogFile(
 LPSTR *ppszCatalogFile) = 0;
 virtual HRESULT __stdcall GetJavaTrust(
 void **ppJavaTrust) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0043_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0043_v0_0_s_ifspec;
typedef IDataFilter *LPDATAFILTER;
extern "C" const IID IID_IDataFilter;
 struct __declspec(uuid("69d14c80-c18e-11d0-a9ce-006097942311")) __declspec(novtable)
 IDataFilter : public IUnknown
 {
 public:
 virtual HRESULT __stdcall DoEncode(
 DWORD dwFlags,
 LONG lInBufferSize,
 BYTE *pbInBuffer,
 LONG lOutBufferSize,
 BYTE *pbOutBuffer,
 LONG lInBytesAvailable,
 LONG *plInBytesRead,
 LONG *plOutBytesWritten,
 DWORD dwReserved) = 0;
 virtual HRESULT __stdcall DoDecode(
 DWORD dwFlags,
 LONG lInBufferSize,
 BYTE *pbInBuffer,
 LONG lOutBufferSize,
 BYTE *pbOutBuffer,
 LONG lInBytesAvailable,
 LONG *plInBytesRead,
 LONG *plOutBytesWritten,
 DWORD dwReserved) = 0;
 virtual HRESULT __stdcall SetEncodingLevel(
 DWORD dwEncLevel) = 0;
 };
typedef struct _tagPROTOCOLFILTERDATA
 {
 DWORD cbSize;
 IInternetProtocolSink *pProtocolSink;
 IInternetProtocol *pProtocol;
 IUnknown *pUnk;
 DWORD dwFilterFlags;
 } PROTOCOLFILTERDATA;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0044_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0044_v0_0_s_ifspec;
typedef IEncodingFilterFactory *LPENCODINGFILTERFACTORY;
typedef struct _tagDATAINFO
 {
 ULONG ulTotalSize;
 ULONG ulavrPacketSize;
 ULONG ulConnectSpeed;
 ULONG ulProcessorSpeed;
 } DATAINFO;
extern "C" const IID IID_IEncodingFilterFactory;
 struct __declspec(uuid("70bdde00-c18e-11d0-a9ce-006097942311")) __declspec(novtable)
 IEncodingFilterFactory : public IUnknown
 {
 public:
 virtual HRESULT __stdcall FindBestFilter(
 LPCWSTR pwzCodeIn,
 LPCWSTR pwzCodeOut,
 DATAINFO info,
 IDataFilter **ppDF) = 0;
 virtual HRESULT __stdcall GetDefaultFilter(
 LPCWSTR pwzCodeIn,
 LPCWSTR pwzCodeOut,
 IDataFilter **ppDF) = 0;
 };
BOOL __stdcall IsLoggingEnabledA( LPCSTR pszUrl);
BOOL __stdcall IsLoggingEnabledW( LPCWSTR pwszUrl);
typedef struct _tagHIT_LOGGING_INFO
 {
 DWORD dwStructSize;
 LPSTR lpszLoggedUrlName;
 SYSTEMTIME StartTime;
 SYSTEMTIME EndTime;
 LPSTR lpszExtendedInfo;
 } HIT_LOGGING_INFO;
typedef struct _tagHIT_LOGGING_INFO *LPHIT_LOGGING_INFO;
BOOL __stdcall WriteHitLogging( LPHIT_LOGGING_INFO lpLogginginfo);
struct CONFIRMSAFETY
 {
 CLSID clsid;
 IUnknown *pUnk;
 DWORD dwFlags;
 } ;
extern "C" const GUID GUID_CUSTOM_CONFIRMOBJECTSAFETY;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0045_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0045_v0_0_s_ifspec;
typedef IWrappedProtocol *LPIWRAPPEDPROTOCOL;
extern "C" const IID IID_IWrappedProtocol;
 struct __declspec(uuid("53c84785-8425-4dc5-971b-e58d9c19f9b6")) __declspec(novtable)
 IWrappedProtocol : public IUnknown
 {
 public:
 virtual HRESULT __stdcall GetWrapperCode(
 LONG *pnCode,
 DWORD_PTR dwReserved) = 0;
 };
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0046_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_urlmon_0000_0046_v0_0_s_ifspec;
unsigned long __stdcall BSTR_UserSize( unsigned long *, unsigned long , BSTR * );
unsigned char * __stdcall BSTR_UserMarshal( unsigned long *, unsigned char *, BSTR * );
unsigned char * __stdcall BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * );
void __stdcall BSTR_UserFree( unsigned long *, BSTR * );
unsigned long __stdcall HWND_UserSize( unsigned long *, unsigned long , HWND * );
unsigned char * __stdcall HWND_UserMarshal( unsigned long *, unsigned char *, HWND * );
unsigned char * __stdcall HWND_UserUnmarshal(unsigned long *, unsigned char *, HWND * );
void __stdcall HWND_UserFree( unsigned long *, HWND * );
unsigned long __stdcall BSTR_UserSize64( unsigned long *, unsigned long , BSTR * );
unsigned char * __stdcall BSTR_UserMarshal64( unsigned long *, unsigned char *, BSTR * );
unsigned char * __stdcall BSTR_UserUnmarshal64(unsigned long *, unsigned char *, BSTR * );
void __stdcall BSTR_UserFree64( unsigned long *, BSTR * );
unsigned long __stdcall HWND_UserSize64( unsigned long *, unsigned long , HWND * );
unsigned char * __stdcall HWND_UserMarshal64( unsigned long *, unsigned char *, HWND * );
unsigned char * __stdcall HWND_UserUnmarshal64(unsigned long *, unsigned char *, HWND * );
void __stdcall HWND_UserFree64( unsigned long *, HWND * );
 HRESULT __stdcall IBinding_GetBindResult_Proxy(
 IBinding * This,
 CLSID *pclsidProtocol,
 DWORD *pdwResult,
 LPOLESTR *pszResult,
 DWORD *pdwReserved);
 HRESULT __stdcall IBinding_GetBindResult_Stub(
 IBinding * This,
 CLSID *pclsidProtocol,
 DWORD *pdwResult,
 LPOLESTR *pszResult,
 DWORD dwReserved);
 HRESULT __stdcall IBindStatusCallback_GetBindInfo_Proxy(
 IBindStatusCallback * This,
 DWORD *grfBINDF,
 BINDINFO *pbindinfo);
 HRESULT __stdcall IBindStatusCallback_GetBindInfo_Stub(
 IBindStatusCallback * This,
 DWORD *grfBINDF,
 RemBINDINFO *pbindinfo,
 RemSTGMEDIUM *pstgmed);
 HRESULT __stdcall IBindStatusCallback_OnDataAvailable_Proxy(
 IBindStatusCallback * This,
 DWORD grfBSCF,
 DWORD dwSize,
 FORMATETC *pformatetc,
 STGMEDIUM *pstgmed);
 HRESULT __stdcall IBindStatusCallback_OnDataAvailable_Stub(
 IBindStatusCallback * This,
 DWORD grfBSCF,
 DWORD dwSize,
 RemFORMATETC *pformatetc,
 RemSTGMEDIUM *pstgmed);
 HRESULT __stdcall IWinInetInfo_QueryOption_Proxy(
 IWinInetInfo * This,
 DWORD dwOption,
 LPVOID pBuffer,
 DWORD *pcbBuf);
 HRESULT __stdcall IWinInetInfo_QueryOption_Stub(
 IWinInetInfo * This,
 DWORD dwOption,
 BYTE *pBuffer,
 DWORD *pcbBuf);
 HRESULT __stdcall IWinInetHttpInfo_QueryInfo_Proxy(
 IWinInetHttpInfo * This,
 DWORD dwOption,
 LPVOID pBuffer,
 DWORD *pcbBuf,
 DWORD *pdwFlags,
 DWORD *pdwReserved);
 HRESULT __stdcall IWinInetHttpInfo_QueryInfo_Stub(
 IWinInetHttpInfo * This,
 DWORD dwOption,
 BYTE *pBuffer,
 DWORD *pcbBuf,
 DWORD *pdwFlags,
 DWORD *pdwReserved);
 HRESULT __stdcall IBindHost_MonikerBindToStorage_Proxy(
 IBindHost * This,
 IMoniker *pMk,
 IBindCtx *pBC,
 IBindStatusCallback *pBSC,
 const IID & riid,
 void **ppvObj);
 HRESULT __stdcall IBindHost_MonikerBindToStorage_Stub(
 IBindHost * This,
 IMoniker *pMk,
 IBindCtx *pBC,
 IBindStatusCallback *pBSC,
 const IID & riid,
 IUnknown **ppvObj);
 HRESULT __stdcall IBindHost_MonikerBindToObject_Proxy(
 IBindHost * This,
 IMoniker *pMk,
 IBindCtx *pBC,
 IBindStatusCallback *pBSC,
 const IID & riid,
 void **ppvObj);
 HRESULT __stdcall IBindHost_MonikerBindToObject_Stub(
 IBindHost * This,
 IMoniker *pMk,
 IBindCtx *pBC,
 IBindStatusCallback *pBSC,
 const IID & riid,
 IUnknown **ppvObj);
}
#pragma warning( disable: 4049 )
#pragma once
typedef struct IPropertyStorage IPropertyStorage;
typedef struct IPropertySetStorage IPropertySetStorage;
typedef struct IEnumSTATPROPSTG IEnumSTATPROPSTG;
typedef struct IEnumSTATPROPSETSTG IEnumSTATPROPSETSTG;
extern "C"{
#pragma warning(push)
#pragma warning(disable:4201)
#pragma warning(disable:4237)
#pragma once
typedef struct tagVersionedStream
 {
 GUID guidVersion;
 IStream *pStream;
 } VERSIONEDSTREAM;
typedef struct tagVersionedStream *LPVERSIONEDSTREAM;
typedef struct tagPROPVARIANT PROPVARIANT;
typedef struct tagCAC
 {
 ULONG cElems;
 CHAR *pElems;
 } CAC;
typedef struct tagCAUB
 {
 ULONG cElems;
 UCHAR *pElems;
 } CAUB;
typedef struct tagCAI
 {
 ULONG cElems;
 SHORT *pElems;
 } CAI;
typedef struct tagCAUI
 {
 ULONG cElems;
 USHORT *pElems;
 } CAUI;
typedef struct tagCAL
 {
 ULONG cElems;
 LONG *pElems;
 } CAL;
typedef struct tagCAUL
 {
 ULONG cElems;
 ULONG *pElems;
 } CAUL;
typedef struct tagCAFLT
 {
 ULONG cElems;
 FLOAT *pElems;
 } CAFLT;
typedef struct tagCADBL
 {
 ULONG cElems;
 DOUBLE *pElems;
 } CADBL;
typedef struct tagCACY
 {
 ULONG cElems;
 CY *pElems;
 } CACY;
typedef struct tagCADATE
 {
 ULONG cElems;
 DATE *pElems;
 } CADATE;
typedef struct tagCABSTR
 {
 ULONG cElems;
 BSTR *pElems;
 } CABSTR;
typedef struct tagCABSTRBLOB
 {
 ULONG cElems;
 BSTRBLOB *pElems;
 } CABSTRBLOB;
typedef struct tagCABOOL
 {
 ULONG cElems;
 VARIANT_BOOL *pElems;
 } CABOOL;
typedef struct tagCASCODE
 {
 ULONG cElems;
 SCODE *pElems;
 } CASCODE;
typedef struct tagCAPROPVARIANT
 {
 ULONG cElems;
 PROPVARIANT *pElems;
 } CAPROPVARIANT;
typedef struct tagCAH
 {
 ULONG cElems;
 LARGE_INTEGER *pElems;
 } CAH;
typedef struct tagCAUH
 {
 ULONG cElems;
 ULARGE_INTEGER *pElems;
 } CAUH;
typedef struct tagCALPSTR
 {
 ULONG cElems;
 LPSTR *pElems;
 } CALPSTR;
typedef struct tagCALPWSTR
 {
 ULONG cElems;
 LPWSTR *pElems;
 } CALPWSTR;
typedef struct tagCAFILETIME
 {
 ULONG cElems;
 FILETIME *pElems;
 } CAFILETIME;
typedef struct tagCACLIPDATA
 {
 ULONG cElems;
 CLIPDATA *pElems;
 } CACLIPDATA;
typedef struct tagCACLSID
 {
 ULONG cElems;
 CLSID *pElems;
 } CACLSID;
typedef WORD PROPVAR_PAD1;
typedef WORD PROPVAR_PAD2;
typedef WORD PROPVAR_PAD3;
struct tagPROPVARIANT {
 union {
struct
 {
 VARTYPE vt;
 PROPVAR_PAD1 wReserved1;
 PROPVAR_PAD2 wReserved2;
 PROPVAR_PAD3 wReserved3;
 union
 {
 CHAR cVal;
 UCHAR bVal;
 SHORT iVal;
 USHORT uiVal;
 LONG lVal;
 ULONG ulVal;
 INT intVal;
 UINT uintVal;
 LARGE_INTEGER hVal;
 ULARGE_INTEGER uhVal;
 FLOAT fltVal;
 DOUBLE dblVal;
 VARIANT_BOOL boolVal;
 SCODE scode;
 CY cyVal;
 DATE date;
 FILETIME filetime;
 CLSID *puuid;
 CLIPDATA *pclipdata;
 BSTR bstrVal;
 BSTRBLOB bstrblobVal;
 BLOB blob;
 LPSTR pszVal;
 LPWSTR pwszVal;
 IUnknown *punkVal;
 IDispatch *pdispVal;
 IStream *pStream;
 IStorage *pStorage;
 LPVERSIONEDSTREAM pVersionedStream;
 LPSAFEARRAY parray;
 CAC cac;
 CAUB caub;
 CAI cai;
 CAUI caui;
 CAL cal;
 CAUL caul;
 CAH cah;
 CAUH cauh;
 CAFLT caflt;
 CADBL cadbl;
 CABOOL cabool;
 CASCODE cascode;
 CACY cacy;
 CADATE cadate;
 CAFILETIME cafiletime;
 CACLSID cauuid;
 CACLIPDATA caclipdata;
 CABSTR cabstr;
 CABSTRBLOB cabstrblob;
 CALPSTR calpstr;
 CALPWSTR calpwstr;
 CAPROPVARIANT capropvar;
 CHAR *pcVal;
 UCHAR *pbVal;
 SHORT *piVal;
 USHORT *puiVal;
 LONG *plVal;
 ULONG *pulVal;
 INT *pintVal;
 UINT *puintVal;
 FLOAT *pfltVal;
 DOUBLE *pdblVal;
 VARIANT_BOOL *pboolVal;
 DECIMAL *pdecVal;
 SCODE *pscode;
 CY *pcyVal;
 DATE *pdate;
 BSTR *pbstrVal;
 IUnknown **ppunkVal;
 IDispatch **ppdispVal;
 LPSAFEARRAY *pparray;
 PROPVARIANT *pvarVal;
 } ;
 } ;
 DECIMAL decVal;
 };
};
typedef struct tagPROPVARIANT * LPPROPVARIANT;
enum PIDMSI_STATUS_VALUE
 { PIDMSI_STATUS_NORMAL = 0,
 PIDMSI_STATUS_NEW = ( PIDMSI_STATUS_NORMAL + 1 ) ,
 PIDMSI_STATUS_PRELIM = ( PIDMSI_STATUS_NEW + 1 ) ,
 PIDMSI_STATUS_DRAFT = ( PIDMSI_STATUS_PRELIM + 1 ) ,
 PIDMSI_STATUS_INPROGRESS = ( PIDMSI_STATUS_DRAFT + 1 ) ,
 PIDMSI_STATUS_EDIT = ( PIDMSI_STATUS_INPROGRESS + 1 ) ,
 PIDMSI_STATUS_REVIEW = ( PIDMSI_STATUS_EDIT + 1 ) ,
 PIDMSI_STATUS_PROOF = ( PIDMSI_STATUS_REVIEW + 1 ) ,
 PIDMSI_STATUS_FINAL = ( PIDMSI_STATUS_PROOF + 1 ) ,
 PIDMSI_STATUS_OTHER = 0x7fff
 } ;
typedef struct tagPROPSPEC
 {
 ULONG ulKind;
 union
 {
 PROPID propid;
 LPOLESTR lpwstr;
 } ;
 } PROPSPEC;
typedef struct tagSTATPROPSTG
 {
 LPOLESTR lpwstrName;
 PROPID propid;
 VARTYPE vt;
 } STATPROPSTG;
typedef struct tagSTATPROPSETSTG
 {
 FMTID fmtid;
 CLSID clsid;
 DWORD grfFlags;
 FILETIME mtime;
 FILETIME ctime;
 FILETIME atime;
 DWORD dwOSVersion;
 } STATPROPSETSTG;
extern RPC_IF_HANDLE __MIDL_itf_propidl_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_propidl_0000_0000_v0_0_s_ifspec;
extern "C" const IID IID_IPropertyStorage;
 struct __declspec(uuid("00000138-0000-0000-C000-000000000046")) __declspec(novtable)
 IPropertyStorage : public IUnknown
 {
 public:
 virtual HRESULT __stdcall ReadMultiple(
 ULONG cpspec,
 const PROPSPEC rgpspec[ ],
 PROPVARIANT rgpropvar[ ]) = 0;
 virtual HRESULT __stdcall WriteMultiple(
 ULONG cpspec,
 const PROPSPEC rgpspec[ ],
 const PROPVARIANT rgpropvar[ ],
 PROPID propidNameFirst) = 0;
 virtual HRESULT __stdcall DeleteMultiple(
 ULONG cpspec,
 const PROPSPEC rgpspec[ ]) = 0;
 virtual HRESULT __stdcall ReadPropertyNames(
 ULONG cpropid,
 const PROPID rgpropid[ ],
 LPOLESTR rglpwstrName[ ]) = 0;
 virtual HRESULT __stdcall WritePropertyNames(
 ULONG cpropid,
 const PROPID rgpropid[ ],
 const LPOLESTR rglpwstrName[ ]) = 0;
 virtual HRESULT __stdcall DeletePropertyNames(
 ULONG cpropid,
 const PROPID rgpropid[ ]) = 0;
 virtual HRESULT __stdcall Commit(
 DWORD grfCommitFlags) = 0;
 virtual HRESULT __stdcall Revert( void) = 0;
 virtual HRESULT __stdcall Enum(
 IEnumSTATPROPSTG **ppenum) = 0;
 virtual HRESULT __stdcall SetTimes(
 const FILETIME *pctime,
 const FILETIME *patime,
 const FILETIME *pmtime) = 0;
 virtual HRESULT __stdcall SetClass(
 const IID & clsid) = 0;
 virtual HRESULT __stdcall Stat(
 STATPROPSETSTG *pstatpsstg) = 0;
 };
typedef IPropertySetStorage *LPPROPERTYSETSTORAGE;
extern "C" const IID IID_IPropertySetStorage;
 struct __declspec(uuid("0000013A-0000-0000-C000-000000000046")) __declspec(novtable)
 IPropertySetStorage : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Create(
 const IID & rfmtid,
 const CLSID *pclsid,
 DWORD grfFlags,
 DWORD grfMode,
 IPropertyStorage **ppprstg) = 0;
 virtual HRESULT __stdcall Open(
 const IID & rfmtid,
 DWORD grfMode,
 IPropertyStorage **ppprstg) = 0;
 virtual HRESULT __stdcall Delete(
 const IID & rfmtid) = 0;
 virtual HRESULT __stdcall Enum(
 IEnumSTATPROPSETSTG **ppenum) = 0;
 };
typedef IEnumSTATPROPSTG *LPENUMSTATPROPSTG;
extern "C" const IID IID_IEnumSTATPROPSTG;
 struct __declspec(uuid("00000139-0000-0000-C000-000000000046")) __declspec(novtable)
 IEnumSTATPROPSTG : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Next(
 ULONG celt,
 STATPROPSTG *rgelt,
 ULONG *pceltFetched) = 0;
 virtual HRESULT __stdcall Skip(
 ULONG celt) = 0;
 virtual HRESULT __stdcall Reset( void) = 0;
 virtual HRESULT __stdcall Clone(
 IEnumSTATPROPSTG **ppenum) = 0;
 };
 HRESULT __stdcall IEnumSTATPROPSTG_RemoteNext_Proxy(
 IEnumSTATPROPSTG * This,
 ULONG celt,
 STATPROPSTG *rgelt,
 ULONG *pceltFetched);
void __stdcall IEnumSTATPROPSTG_RemoteNext_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IEnumSTATPROPSETSTG *LPENUMSTATPROPSETSTG;
extern "C" const IID IID_IEnumSTATPROPSETSTG;
 struct __declspec(uuid("0000013B-0000-0000-C000-000000000046")) __declspec(novtable)
 IEnumSTATPROPSETSTG : public IUnknown
 {
 public:
 virtual HRESULT __stdcall Next(
 ULONG celt,
 STATPROPSETSTG *rgelt,
 ULONG *pceltFetched) = 0;
 virtual HRESULT __stdcall Skip(
 ULONG celt) = 0;
 virtual HRESULT __stdcall Reset( void) = 0;
 virtual HRESULT __stdcall Clone(
 IEnumSTATPROPSETSTG **ppenum) = 0;
 };
 HRESULT __stdcall IEnumSTATPROPSETSTG_RemoteNext_Proxy(
 IEnumSTATPROPSETSTG * This,
 ULONG celt,
 STATPROPSETSTG *rgelt,
 ULONG *pceltFetched);
void __stdcall IEnumSTATPROPSETSTG_RemoteNext_Stub(
 IRpcStubBuffer *This,
 IRpcChannelBuffer *_pRpcChannelBuffer,
 PRPC_MESSAGE _pRpcMessage,
 DWORD *_pdwStubPhase);
typedef IPropertyStorage *LPPROPERTYSTORAGE;
extern "C" __declspec(dllimport) HRESULT __stdcall PropVariantCopy ( PROPVARIANT * pvarDest, const PROPVARIANT * pvarSrc );
extern "C" __declspec(dllimport) HRESULT __stdcall PropVariantClear ( PROPVARIANT * pvar );
extern "C" __declspec(dllimport) HRESULT __stdcall FreePropVariantArray ( ULONG cVariants, PROPVARIANT * rgvars );
inline void PropVariantInit ( PROPVARIANT * pvar )
{
 memset ( pvar, 0, sizeof(PROPVARIANT) );
}
extern "C" __declspec(dllimport) HRESULT __stdcall StgCreatePropStg( IUnknown* pUnk, const IID & fmtid, const CLSID *pclsid, DWORD grfFlags, DWORD dwReserved, IPropertyStorage **ppPropStg );
extern "C" __declspec(dllimport) HRESULT __stdcall StgOpenPropStg( IUnknown* pUnk, const IID & fmtid, DWORD grfFlags, DWORD dwReserved, IPropertyStorage **ppPropStg );
extern "C" __declspec(dllimport) HRESULT __stdcall StgCreatePropSetStg( IStorage *pStorage, DWORD dwReserved, IPropertySetStorage **ppPropSetStg);
 extern "C" __declspec(dllimport) HRESULT __stdcall FmtIdToPropStgName( const FMTID *pfmtid, LPOLESTR oszName );
extern "C" __declspec(dllimport) HRESULT __stdcall PropStgNameToFmtId( const LPOLESTR oszName, FMTID *pfmtid );
typedef struct tagSERIALIZEDPROPERTYVALUE
{
 DWORD dwType;
 BYTE rgb[1];
} SERIALIZEDPROPERTYVALUE;
extern "C" SERIALIZEDPROPERTYVALUE* __stdcall
StgConvertVariantToProperty(
 const PROPVARIANT* pvar,
 USHORT CodePage,
 SERIALIZEDPROPERTYVALUE* pprop,
 ULONG* pcb,
 PROPID pid,
 BOOLEAN fReserved,
 ULONG* pcIndirect);
class PMemoryAllocator;
extern "C" BOOLEAN __stdcall
StgConvertPropertyToVariant(
 const SERIALIZEDPROPERTYVALUE* pprop,
 USHORT CodePage,
 PROPVARIANT* pvar,
 PMemoryAllocator* pma);
#pragma warning(pop)
extern RPC_IF_HANDLE __MIDL_itf_propidl_0000_0004_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_propidl_0000_0004_v0_0_s_ifspec;
unsigned long __stdcall BSTR_UserSize( unsigned long *, unsigned long , BSTR * );
unsigned char * __stdcall BSTR_UserMarshal( unsigned long *, unsigned char *, BSTR * );
unsigned char * __stdcall BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * );
void __stdcall BSTR_UserFree( unsigned long *, BSTR * );
unsigned long __stdcall LPSAFEARRAY_UserSize( unsigned long *, unsigned long , LPSAFEARRAY * );
unsigned char * __stdcall LPSAFEARRAY_UserMarshal( unsigned long *, unsigned char *, LPSAFEARRAY * );
unsigned char * __stdcall LPSAFEARRAY_UserUnmarshal(unsigned long *, unsigned char *, LPSAFEARRAY * );
void __stdcall LPSAFEARRAY_UserFree( unsigned long *, LPSAFEARRAY * );
unsigned long __stdcall BSTR_UserSize64( unsigned long *, unsigned long , BSTR * );
unsigned char * __stdcall BSTR_UserMarshal64( unsigned long *, unsigned char *, BSTR * );
unsigned char * __stdcall BSTR_UserUnmarshal64(unsigned long *, unsigned char *, BSTR * );
void __stdcall BSTR_UserFree64( unsigned long *, BSTR * );
unsigned long __stdcall LPSAFEARRAY_UserSize64( unsigned long *, unsigned long , LPSAFEARRAY * );
unsigned char * __stdcall LPSAFEARRAY_UserMarshal64( unsigned long *, unsigned char *, LPSAFEARRAY * );
unsigned char * __stdcall LPSAFEARRAY_UserUnmarshal64(unsigned long *, unsigned char *, LPSAFEARRAY * );
void __stdcall LPSAFEARRAY_UserFree64( unsigned long *, LPSAFEARRAY * );
 HRESULT __stdcall IEnumSTATPROPSTG_Next_Proxy(
 IEnumSTATPROPSTG * This,
 ULONG celt,
 STATPROPSTG *rgelt,
 ULONG *pceltFetched);
 HRESULT __stdcall IEnumSTATPROPSTG_Next_Stub(
 IEnumSTATPROPSTG * This,
 ULONG celt,
 STATPROPSTG *rgelt,
 ULONG *pceltFetched);
 HRESULT __stdcall IEnumSTATPROPSETSTG_Next_Proxy(
 IEnumSTATPROPSETSTG * This,
 ULONG celt,
 STATPROPSETSTG *rgelt,
 ULONG *pceltFetched);
 HRESULT __stdcall IEnumSTATPROPSETSTG_Next_Stub(
 IEnumSTATPROPSETSTG * This,
 ULONG celt,
 STATPROPSETSTG *rgelt,
 ULONG *pceltFetched);
}
extern "C" __declspec(dllimport) HRESULT __stdcall CreateStdProgressIndicator( HWND hwndParent,
 LPCOLESTR pszTitle,
 IBindStatusCallback * pIbscCaller,
 IBindStatusCallback ** ppIbsc);
#pragma warning(disable:4103)
#pragma pack(pop)
#pragma once
#pragma warning(disable:4103)
#pragma pack(push,8)
extern "C" const IID IID_StdOle;
extern "C" __declspec(dllimport) BSTR __stdcall SysAllocString( const OLECHAR * psz);
extern "C" __declspec(dllimport) INT __stdcall SysReAllocString( BSTR* pbstr, const OLECHAR* psz);
extern "C" __declspec(dllimport) BSTR __stdcall SysAllocStringLen( const OLECHAR * strIn, UINT ui);
 extern "C" __declspec(dllimport) INT __stdcall SysReAllocStringLen( BSTR* pbstr, const OLECHAR* psz, unsigned int len);
extern "C" __declspec(dllimport) void __stdcall SysFreeString( BSTR bstrString);
extern "C" __declspec(dllimport) UINT __stdcall SysStringLen( BSTR);
extern "C" __declspec(dllimport) UINT __stdcall SysStringByteLen( BSTR bstr);
extern "C" __declspec(dllimport) BSTR __stdcall SysAllocStringByteLen( LPCSTR psz, UINT len);
extern "C" __declspec(dllimport) INT __stdcall DosDateTimeToVariantTime( USHORT wDosDate, USHORT wDosTime, DOUBLE * pvtime);
extern "C" __declspec(dllimport) INT __stdcall VariantTimeToDosDateTime( DOUBLE vtime, USHORT * pwDosDate, USHORT * pwDosTime);
extern "C" __declspec(dllimport) INT __stdcall SystemTimeToVariantTime( LPSYSTEMTIME lpSystemTime, DOUBLE *pvtime);
extern "C" __declspec(dllimport) INT __stdcall VariantTimeToSystemTime( DOUBLE vtime, LPSYSTEMTIME lpSystemTime);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayAllocDescriptor( UINT cDims, SAFEARRAY ** ppsaOut);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayAllocDescriptorEx( VARTYPE vt, UINT cDims, SAFEARRAY ** ppsaOut);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayAllocData( SAFEARRAY * psa);
extern "C" __declspec(dllimport) SAFEARRAY * __stdcall SafeArrayCreate( VARTYPE vt, UINT cDims, SAFEARRAYBOUND * rgsabound);
extern "C" __declspec(dllimport) SAFEARRAY * __stdcall SafeArrayCreateEx( VARTYPE vt, UINT cDims, SAFEARRAYBOUND * rgsabound, PVOID pvExtra);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayCopyData( SAFEARRAY *psaSource, SAFEARRAY *psaTarget);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayDestroyDescriptor( SAFEARRAY * psa);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayDestroyData( SAFEARRAY * psa);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayDestroy( SAFEARRAY * psa);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayRedim( SAFEARRAY * psa, SAFEARRAYBOUND * psaboundNew);
extern "C" __declspec(dllimport) UINT __stdcall SafeArrayGetDim( SAFEARRAY * psa);
extern "C" __declspec(dllimport) UINT __stdcall SafeArrayGetElemsize( SAFEARRAY * psa);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayGetUBound( SAFEARRAY * psa, UINT nDim, LONG * plUbound);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayGetLBound( SAFEARRAY * psa, UINT nDim, LONG * plLbound);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayLock( SAFEARRAY * psa);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayUnlock( SAFEARRAY * psa);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayAccessData( SAFEARRAY * psa, void ** ppvData);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayUnaccessData( SAFEARRAY * psa);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayGetElement( SAFEARRAY * psa, LONG * rgIndices, void * pv);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayPutElement( SAFEARRAY * psa, LONG * rgIndices, void * pv);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayCopy( SAFEARRAY * psa, SAFEARRAY ** ppsaOut);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayPtrOfIndex( SAFEARRAY * psa, LONG * rgIndices, void ** ppvData);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArraySetRecordInfo( SAFEARRAY * psa, IRecordInfo * prinfo);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayGetRecordInfo( SAFEARRAY * psa, IRecordInfo ** prinfo);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArraySetIID( SAFEARRAY * psa, const GUID & guid);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayGetIID( SAFEARRAY * psa, GUID * pguid);
extern "C" __declspec(dllimport) HRESULT __stdcall SafeArrayGetVartype( SAFEARRAY * psa, VARTYPE * pvt);
extern "C" __declspec(dllimport) SAFEARRAY * __stdcall SafeArrayCreateVector( VARTYPE vt, LONG lLbound, ULONG cElements);
extern "C" __declspec(dllimport) SAFEARRAY * __stdcall SafeArrayCreateVectorEx( VARTYPE vt, LONG lLbound, ULONG cElements, PVOID pvExtra);
extern "C" __declspec(dllimport) void __stdcall VariantInit( VARIANTARG * pvarg);
extern "C" __declspec(dllimport) HRESULT __stdcall VariantClear( VARIANTARG * pvarg);
extern "C" __declspec(dllimport) HRESULT __stdcall VariantCopy( VARIANTARG * pvargDest, const VARIANTARG * pvargSrc);
extern "C" __declspec(dllimport) HRESULT __stdcall VariantCopyInd( VARIANT * pvarDest, const VARIANTARG * pvargSrc);
extern "C" __declspec(dllimport) HRESULT __stdcall VariantChangeType( VARIANTARG * pvargDest,
 const VARIANTARG * pvarSrc, USHORT wFlags, VARTYPE vt);
extern "C" __declspec(dllimport) HRESULT __stdcall VariantChangeTypeEx( VARIANTARG * pvargDest,
 const VARIANTARG * pvarSrc, LCID lcid, USHORT wFlags, VARTYPE vt);
extern "C" __declspec(dllimport) HRESULT __stdcall VectorFromBstr ( BSTR bstr, SAFEARRAY ** ppsa);
extern "C" __declspec(dllimport) HRESULT __stdcall BstrFromVector ( SAFEARRAY *psa, BSTR *pbstr);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromI2(SHORT sIn, BYTE * pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromI4(LONG lIn, BYTE * pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromI8(LONG64 i64In, BYTE * pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromR4(FLOAT fltIn, BYTE * pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromR8(DOUBLE dblIn, BYTE * pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromCy(CY cyIn, BYTE * pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromDate(DATE dateIn, BYTE * pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromStr( LPCOLESTR strIn, LCID lcid, ULONG dwFlags, BYTE * pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromDisp(IDispatch * pdispIn, LCID lcid, BYTE * pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromBool(VARIANT_BOOL boolIn, BYTE * pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromI1(CHAR cIn, BYTE *pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromUI2(USHORT uiIn, BYTE *pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromUI4(ULONG ulIn, BYTE *pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromUI8(ULONG64 ui64In, BYTE * pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI1FromDec( const DECIMAL *pdecIn, BYTE *pbOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromUI1(BYTE bIn, SHORT * psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromI4(LONG lIn, SHORT * psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromI8(LONG64 i64In, SHORT * psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromR4(FLOAT fltIn, SHORT * psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromR8(DOUBLE dblIn, SHORT * psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromCy(CY cyIn, SHORT * psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromDate(DATE dateIn, SHORT * psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromStr( LPCOLESTR strIn, LCID lcid, ULONG dwFlags, SHORT * psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromDisp(IDispatch * pdispIn, LCID lcid, SHORT * psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromBool(VARIANT_BOOL boolIn, SHORT * psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromI1(CHAR cIn, SHORT *psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromUI2(USHORT uiIn, SHORT *psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromUI4(ULONG ulIn, SHORT *psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromUI8(ULONG64 ui64In, SHORT * psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI2FromDec( const DECIMAL *pdecIn, SHORT *psOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromUI1(BYTE bIn, LONG * plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromI2(SHORT sIn, LONG * plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromI8(LONG64 i64In, LONG * plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromR4(FLOAT fltIn, LONG * plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromR8(DOUBLE dblIn, LONG * plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromCy(CY cyIn, LONG * plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromDate(DATE dateIn, LONG * plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromStr( LPCOLESTR strIn, LCID lcid, ULONG dwFlags, LONG * plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromDisp(IDispatch * pdispIn, LCID lcid, LONG * plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromBool(VARIANT_BOOL boolIn, LONG * plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromI1(CHAR cIn, LONG *plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromUI2(USHORT uiIn, LONG *plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromUI4(ULONG ulIn, LONG *plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromUI8(ULONG64 ui64In, LONG * plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromDec( const DECIMAL *pdecIn, LONG *plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromInt(INT intIn, LONG *plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromUI1(BYTE bIn, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromI2(SHORT sIn, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromI4(LONG lIn, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromR4(FLOAT fltIn, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromR8(DOUBLE dblIn, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromCy( CY cyIn, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromDate(DATE dateIn, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromStr( LPCOLESTR strIn, LCID lcid, unsigned long dwFlags, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromDisp(IDispatch * pdispIn, LCID lcid, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromBool(VARIANT_BOOL boolIn, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromI1(CHAR cIn, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromUI2(USHORT uiIn, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromUI4(ULONG ulIn, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromUI8(ULONG64 ui64In, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromDec( const DECIMAL *pdecIn, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI8FromInt(INT intIn, LONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromUI1(BYTE bIn, FLOAT * pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromI2(SHORT sIn, FLOAT * pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromI4(LONG lIn, FLOAT * pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromI8(LONG64 i64In, FLOAT * pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromR8(DOUBLE dblIn, FLOAT * pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromCy(CY cyIn, FLOAT * pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromDate(DATE dateIn, FLOAT * pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromStr( LPCOLESTR strIn, LCID lcid, ULONG dwFlags, FLOAT *pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromDisp(IDispatch * pdispIn, LCID lcid, FLOAT * pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromBool(VARIANT_BOOL boolIn, FLOAT * pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromI1(CHAR cIn, FLOAT *pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromUI2(USHORT uiIn, FLOAT *pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromUI4(ULONG ulIn, FLOAT *pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromUI8(ULONG64 ui64In, FLOAT * pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR4FromDec( const DECIMAL *pdecIn, FLOAT *pfltOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromUI1(BYTE bIn, DOUBLE * pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromI2(SHORT sIn, DOUBLE * pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromI4(LONG lIn, DOUBLE * pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromI8(LONG64 i64In, DOUBLE * pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromR4(FLOAT fltIn, DOUBLE * pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromCy(CY cyIn, DOUBLE * pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromDate(DATE dateIn, DOUBLE * pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromStr( LPCOLESTR strIn, LCID lcid, ULONG dwFlags, DOUBLE *pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromDisp(IDispatch * pdispIn, LCID lcid, DOUBLE * pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromBool(VARIANT_BOOL boolIn, DOUBLE * pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromI1(CHAR cIn, DOUBLE *pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromUI2(USHORT uiIn, DOUBLE *pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromUI4(ULONG ulIn, DOUBLE *pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromUI8(ULONG64 ui64In, DOUBLE * pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarR8FromDec( const DECIMAL *pdecIn, DOUBLE *pdblOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromUI1(BYTE bIn, DATE * pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromI2(SHORT sIn, DATE * pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromI4(LONG lIn, DATE * pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromI8(LONG64 i64In, DATE * pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromR4(FLOAT fltIn, DATE * pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromR8(DOUBLE dblIn, DATE * pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromCy(CY cyIn, DATE * pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromStr( LPCOLESTR strIn, LCID lcid, ULONG dwFlags, DATE *pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromDisp(IDispatch * pdispIn, LCID lcid, DATE * pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromBool(VARIANT_BOOL boolIn, DATE * pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromI1(CHAR cIn, DATE *pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromUI2(USHORT uiIn, DATE *pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromUI4(ULONG ulIn, DATE *pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromUI8(ULONG64 ui64In, DATE * pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromDec( const DECIMAL *pdecIn, DATE *pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromUI1(BYTE bIn, CY * pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromI2(SHORT sIn, CY * pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromI4(LONG lIn, CY * pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromI8(LONG64 i64In, CY * pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromR4(FLOAT fltIn, CY * pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromR8(DOUBLE dblIn, CY * pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromDate(DATE dateIn, CY * pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromStr( LPCOLESTR strIn, LCID lcid, ULONG dwFlags, CY * pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromDisp( IDispatch * pdispIn, LCID lcid, CY * pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromBool(VARIANT_BOOL boolIn, CY * pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromI1(CHAR cIn, CY *pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromUI2(USHORT uiIn, CY *pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromUI4(ULONG ulIn, CY *pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromUI8(ULONG64 ui64In, CY * pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarCyFromDec( const DECIMAL *pdecIn, CY *pcyOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromUI1(BYTE bVal, LCID lcid, ULONG dwFlags, BSTR * pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromI2(SHORT iVal, LCID lcid, ULONG dwFlags, BSTR * pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromI4(LONG lIn, LCID lcid, ULONG dwFlags, BSTR * pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromI8(LONG64 i64In, LCID lcid, unsigned long dwFlags, BSTR * pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromR4(FLOAT fltIn, LCID lcid, ULONG dwFlags, BSTR * pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromR8(DOUBLE dblIn, LCID lcid, ULONG dwFlags, BSTR * pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromCy(CY cyIn, LCID lcid, ULONG dwFlags, BSTR * pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromDate( DATE dateIn, LCID lcid, ULONG dwFlags, BSTR * pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromDisp(IDispatch * pdispIn, LCID lcid, ULONG dwFlags, BSTR * pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromBool(VARIANT_BOOL boolIn, LCID lcid, ULONG dwFlags, BSTR * pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromI1(CHAR cIn, LCID lcid, ULONG dwFlags, BSTR *pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromUI2(USHORT uiIn, LCID lcid, ULONG dwFlags, BSTR *pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromUI4(ULONG ulIn, LCID lcid, ULONG dwFlags, BSTR *pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromUI8(ULONG64 ui64In, LCID lcid, unsigned long dwFlags, BSTR * pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBstrFromDec( const DECIMAL *pdecIn, LCID lcid, ULONG dwFlags, BSTR *pbstrOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromUI1(BYTE bIn, VARIANT_BOOL * pboolOut);
 extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromI2( SHORT sIn, VARIANT_BOOL * pboolOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromI4(LONG lIn, VARIANT_BOOL * pboolOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromI8(LONG64 i64In, VARIANT_BOOL * pboolOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromR4(FLOAT fltIn, VARIANT_BOOL * pboolOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromR8(DOUBLE dblIn, VARIANT_BOOL * pboolOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromDate(DATE dateIn, VARIANT_BOOL * pboolOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromCy(CY cyIn, VARIANT_BOOL * pboolOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromStr( LPCOLESTR strIn, LCID lcid, ULONG dwFlags, VARIANT_BOOL * pboolOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromDisp(IDispatch * pdispIn, LCID lcid, VARIANT_BOOL * pboolOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromI1(CHAR cIn, VARIANT_BOOL *pboolOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromUI2(USHORT uiIn, VARIANT_BOOL *pboolOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromUI4(ULONG ulIn, VARIANT_BOOL *pboolOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromUI8(ULONG64 i64In, VARIANT_BOOL * pboolOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarBoolFromDec( const DECIMAL *pdecIn, VARIANT_BOOL *pboolOut);
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromUI1(
 BYTE bIn,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromI2(
 SHORT uiIn,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromI4(
 LONG lIn,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromI8(
 LONG64 i64In,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromR4(
 FLOAT fltIn,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromR8(
 DOUBLE dblIn,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromDate(
 DATE dateIn,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromCy(
 CY cyIn,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromStr(
 LPCOLESTR strIn,
 LCID lcid,
 ULONG dwFlags,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromDisp(
 IDispatch *pdispIn,
 LCID lcid,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromBool(
 VARIANT_BOOL boolIn,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromUI2(
 USHORT uiIn,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromUI4(
 ULONG ulIn,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromUI8(
 ULONG64 i64In,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall
VarI1FromDec(
 const DECIMAL *pdecIn,
 CHAR *pcOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromUI1(BYTE bIn, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromI2(SHORT uiIn, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromI4(LONG lIn, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromI8(LONG64 i64In, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromR4(FLOAT fltIn, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromR8(DOUBLE dblIn, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromDate(DATE dateIn, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromCy(CY cyIn, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromStr( LPCOLESTR strIn, LCID lcid, ULONG dwFlags, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromDisp( IDispatch *pdispIn, LCID lcid, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromBool(VARIANT_BOOL boolIn, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromI1(CHAR cIn, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromUI4(ULONG ulIn, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromUI8(ULONG64 i64In, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI2FromDec( const DECIMAL *pdecIn, USHORT *puiOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromUI1(BYTE bIn, ULONG *pulOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromI2( SHORT uiIn, ULONG *pulOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromI4(LONG lIn, ULONG *pulOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromI8(LONG64 i64In, ULONG *plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromR4(FLOAT fltIn, ULONG *pulOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromR8(DOUBLE dblIn, ULONG *pulOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromDate(DATE dateIn, ULONG *pulOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromCy(CY cyIn, ULONG *pulOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromStr( LPCOLESTR strIn, LCID lcid, ULONG dwFlags, ULONG *pulOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromDisp( IDispatch *pdispIn, LCID lcid, ULONG *pulOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromBool(VARIANT_BOOL boolIn, ULONG *pulOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromI1(CHAR cIn, ULONG *pulOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromUI2(USHORT uiIn, ULONG *pulOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromUI8(ULONG64 ui64In, ULONG *plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI4FromDec( const DECIMAL *pdecIn, ULONG *pulOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromUI1(BYTE bIn, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromI2(SHORT sIn, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromI4(LONG lIn, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromI8(LONG64 ui64In, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromR4(FLOAT fltIn, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromR8(DOUBLE dblIn, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromCy(CY cyIn, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromDate(DATE dateIn, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromStr( LPCOLESTR strIn, LCID lcid, unsigned long dwFlags, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromDisp( IDispatch * pdispIn, LCID lcid, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromBool(VARIANT_BOOL boolIn, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromI1(CHAR cIn, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromUI2(USHORT uiIn, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromUI4(ULONG ulIn, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromDec( const DECIMAL *pdecIn, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUI8FromInt(INT intIn, ULONG64 * pi64Out);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromUI1( BYTE bIn, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromI2( SHORT uiIn, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromI4( LONG lIn, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromI8(LONG64 i64In, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromR4( FLOAT fltIn, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromR8( DOUBLE dblIn, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromDate( DATE dateIn, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromCy( CY cyIn, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromStr( LPCOLESTR strIn, LCID lcid, ULONG dwFlags, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromDisp( IDispatch *pdispIn, LCID lcid, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromBool( VARIANT_BOOL boolIn, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromI1( CHAR cIn, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromUI2( USHORT uiIn, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromUI4( ULONG ulIn, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDecFromUI8(ULONG64 ui64In, DECIMAL *pdecOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromI8(LONG64 i64In, LONG *plOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarI4FromUI8(ULONG64 ui64In, LONG *plOut);
typedef struct {
 INT cDig;
 ULONG dwInFlags;
 ULONG dwOutFlags;
 INT cchUsed;
 INT nBaseShift;
 INT nPwr10;
} NUMPARSE;
extern "C" __declspec(dllimport) HRESULT __stdcall VarParseNumFromStr( LPCOLESTR strIn, LCID lcid, ULONG dwFlags,
 NUMPARSE * pnumprs, BYTE * rgbDig);
extern "C" __declspec(dllimport) HRESULT __stdcall VarNumFromParseNum( NUMPARSE * pnumprs, BYTE * rgbDig,
 ULONG dwVtBits, VARIANT * pvar);
extern "C" HRESULT __stdcall VarAdd( LPVARIANT pvarLeft, LPVARIANT pvarRight, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarAnd( LPVARIANT pvarLeft, LPVARIANT pvarRight, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarCat( LPVARIANT pvarLeft, LPVARIANT pvarRight, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarDiv( LPVARIANT pvarLeft, LPVARIANT pvarRight, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarEqv( LPVARIANT pvarLeft, LPVARIANT pvarRight, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarIdiv( LPVARIANT pvarLeft, LPVARIANT pvarRight, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarImp( LPVARIANT pvarLeft, LPVARIANT pvarRight, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarMod( LPVARIANT pvarLeft, LPVARIANT pvarRight, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarMul( LPVARIANT pvarLeft, LPVARIANT pvarRight, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarOr( LPVARIANT pvarLeft, LPVARIANT pvarRight, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarPow( LPVARIANT pvarLeft, LPVARIANT pvarRight, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarSub( LPVARIANT pvarLeft, LPVARIANT pvarRight, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarXor( LPVARIANT pvarLeft, LPVARIANT pvarRight, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarAbs( LPVARIANT pvarIn, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarFix( LPVARIANT pvarIn, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarInt( LPVARIANT pvarIn, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarNeg( LPVARIANT pvarIn, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarNot( LPVARIANT pvarIn, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarRound( LPVARIANT pvarIn, int cDecimals, LPVARIANT pvarResult);
extern "C" HRESULT __stdcall VarCmp( LPVARIANT pvarLeft, LPVARIANT pvarRight, LCID lcid, ULONG dwFlags);
extern "C++" {
__inline
HRESULT
__stdcall
VarCmp(LPVARIANT pvarLeft, LPVARIANT pvarRight, LCID lcid) {
 return VarCmp(pvarLeft, pvarRight, lcid, 0);
}
}
extern "C" HRESULT __stdcall VarDecAdd( LPDECIMAL pdecLeft, LPDECIMAL pdecRight, LPDECIMAL pdecResult);
extern "C" HRESULT __stdcall VarDecDiv( LPDECIMAL pdecLeft, LPDECIMAL pdecRight, LPDECIMAL pdecResult);
extern "C" HRESULT __stdcall VarDecMul( LPDECIMAL pdecLeft, LPDECIMAL pdecRight, LPDECIMAL pdecResult);
extern "C" HRESULT __stdcall VarDecSub( LPDECIMAL pdecLeft, LPDECIMAL pdecRight, LPDECIMAL pdecResult);
extern "C" HRESULT __stdcall VarDecAbs( LPDECIMAL pdecIn, LPDECIMAL pdecResult);
extern "C" HRESULT __stdcall VarDecFix( LPDECIMAL pdecIn, LPDECIMAL pdecResult);
extern "C" HRESULT __stdcall VarDecInt( LPDECIMAL pdecIn, LPDECIMAL pdecResult);
extern "C" HRESULT __stdcall VarDecNeg( LPDECIMAL pdecIn, LPDECIMAL pdecResult);
extern "C" HRESULT __stdcall VarDecRound( LPDECIMAL pdecIn, int cDecimals, LPDECIMAL pdecResult);
extern "C" HRESULT __stdcall VarDecCmp( LPDECIMAL pdecLeft, LPDECIMAL pdecRight);
extern "C" HRESULT __stdcall VarDecCmpR8( LPDECIMAL pdecLeft, double dblRight);
extern "C" HRESULT __stdcall VarCyAdd( CY cyLeft, CY cyRight, LPCY pcyResult);
extern "C" HRESULT __stdcall VarCyMul( CY cyLeft, CY cyRight, LPCY pcyResult);
extern "C" HRESULT __stdcall VarCyMulI4( CY cyLeft, long lRight, LPCY pcyResult);
extern "C" HRESULT __stdcall VarCyMulI8( CY cyLeft, LONG64 lRight, LPCY pcyResult);
extern "C" HRESULT __stdcall VarCySub( CY cyLeft, CY cyRight, LPCY pcyResult);
extern "C" HRESULT __stdcall VarCyAbs( CY cyIn, LPCY pcyResult);
extern "C" HRESULT __stdcall VarCyFix( CY cyIn, LPCY pcyResult);
extern "C" HRESULT __stdcall VarCyInt( CY cyIn, LPCY pcyResult);
extern "C" HRESULT __stdcall VarCyNeg( CY cyIn, LPCY pcyResult);
extern "C" HRESULT __stdcall VarCyRound( CY cyIn, int cDecimals, LPCY pcyResult);
extern "C" HRESULT __stdcall VarCyCmp( CY cyLeft, CY cyRight);
extern "C" HRESULT __stdcall VarCyCmpR8( CY cyLeft, double dblRight);
extern "C" HRESULT __stdcall VarBstrCat( BSTR bstrLeft, BSTR bstrRight, LPBSTR pbstrResult);
extern "C" HRESULT __stdcall VarBstrCmp( BSTR bstrLeft, BSTR bstrRight, LCID lcid, ULONG dwFlags);
extern "C" HRESULT __stdcall VarR8Pow( double dblLeft, double dblRight, double *pdblResult);
extern "C" HRESULT __stdcall VarR4CmpR8( float fltLeft, double dblRight);
extern "C" HRESULT __stdcall VarR8Round( double dblIn, int cDecimals, double *pdblResult);
typedef struct {
 SYSTEMTIME st;
 USHORT wDayOfYear;
} UDATE;
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromUdate( UDATE *pudateIn, ULONG dwFlags, DATE *pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarDateFromUdateEx( UDATE *pudateIn, LCID lcid, ULONG dwFlags, DATE *pdateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall VarUdateFromDate( DATE dateIn, ULONG dwFlags, UDATE *pudateOut);
extern "C" __declspec(dllimport) HRESULT __stdcall GetAltMonthNames(LCID lcid, LPOLESTR * * prgp);
extern "C" __declspec(dllimport) HRESULT __stdcall VarFormat(
 LPVARIANT pvarIn,
 LPOLESTR pstrFormat,
 int iFirstDay,
 int iFirstWeek,
 ULONG dwFlags,
 BSTR *pbstrOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall VarFormatDateTime(
 LPVARIANT pvarIn,
 int iNamedFormat,
 ULONG dwFlags,
 BSTR *pbstrOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall VarFormatNumber(
 LPVARIANT pvarIn,
 int iNumDig,
 int iIncLead,
 int iUseParens,
 int iGroup,
 ULONG dwFlags,
 BSTR *pbstrOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall VarFormatPercent(
 LPVARIANT pvarIn,
 int iNumDig,
 int iIncLead,
 int iUseParens,
 int iGroup,
 ULONG dwFlags,
 BSTR *pbstrOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall VarFormatCurrency(
 LPVARIANT pvarIn,
 int iNumDig,
 int iIncLead,
 int iUseParens,
 int iGroup,
 ULONG dwFlags,
 BSTR *pbstrOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall VarWeekdayName(
 int iWeekday,
 int fAbbrev,
 int iFirstDay,
 ULONG dwFlags,
 BSTR *pbstrOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall VarMonthName(
 int iMonth,
 int fAbbrev,
 ULONG dwFlags,
 BSTR *pbstrOut
 );
extern "C" __declspec(dllimport) HRESULT __stdcall VarFormatFromTokens(
 LPVARIANT pvarIn,
 LPOLESTR pstrFormat,
 LPBYTE pbTokCur,
 ULONG dwFlags,
 BSTR *pbstrOut,
 LCID lcid
 );
extern "C" __declspec(dllimport) HRESULT __stdcall VarTokenizeFormatString(
 LPOLESTR pstrFormat,
 LPBYTE rgbTok,
 int cbTok,
 int iFirstDay,
 int iFirstWeek,
 LCID lcid,
 int *pcbActual
 );
typedef ITypeLib * LPTYPELIB;
typedef LONG DISPID;
typedef DISPID MEMBERID;
typedef ITypeInfo * LPTYPEINFO;
typedef ITypeComp * LPTYPECOMP;
typedef ICreateTypeLib * LPCREATETYPELIB;
typedef ICreateTypeInfo * LPCREATETYPEINFO;
extern "C" __declspec(dllimport) ULONG __stdcall LHashValOfNameSysA(SYSKIND syskind, LCID lcid,
 LPCSTR szName);
extern "C" __declspec(dllimport) ULONG __stdcall
LHashValOfNameSys(SYSKIND syskind, LCID lcid, const OLECHAR * szName);
extern "C" __declspec(dllimport) HRESULT __stdcall LoadTypeLib( LPCOLESTR szFile, ITypeLib ** pptlib);
typedef enum tagREGKIND
{
 REGKIND_DEFAULT,
 REGKIND_REGISTER,
 REGKIND_NONE
} REGKIND;
extern "C" __declspec(dllimport) HRESULT __stdcall LoadTypeLibEx(LPCOLESTR szFile, REGKIND regkind,
 ITypeLib ** pptlib);
extern "C" __declspec(dllimport) HRESULT __stdcall LoadRegTypeLib(const GUID & rguid, WORD wVerMajor, WORD wVerMinor,
 LCID lcid, ITypeLib ** pptlib);
extern "C" __declspec(dllimport) HRESULT __stdcall QueryPathOfRegTypeLib(const GUID & guid, USHORT wMaj, USHORT wMin,
 LCID lcid, LPBSTR lpbstrPathName);
extern "C" __declspec(dllimport) HRESULT __stdcall RegisterTypeLib(ITypeLib * ptlib, LPCOLESTR szFullPath,
 LPCOLESTR szHelpDir);
extern "C" __declspec(dllimport) HRESULT __stdcall UnRegisterTypeLib(const GUID & libID, WORD wVerMajor,
 WORD wVerMinor, LCID lcid, SYSKIND syskind);
extern "C" __declspec(dllimport) HRESULT __stdcall RegisterTypeLibForUser(ITypeLib *ptlib, OLECHAR *szFullPath,
 OLECHAR *szHelpDir);
extern "C" __declspec(dllimport) HRESULT __stdcall UnRegisterTypeLibForUser(
 const GUID & libID,
 WORD wMajorVerNum,
 WORD wMinorVerNum,
 LCID lcid,
 SYSKIND syskind);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateTypeLib(SYSKIND syskind, LPCOLESTR szFile,
 ICreateTypeLib ** ppctlib);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateTypeLib2(SYSKIND syskind, LPCOLESTR szFile,
 ICreateTypeLib2 **ppctlib);
typedef IDispatch * LPDISPATCH;
typedef struct tagPARAMDATA {
 OLECHAR * szName;
 VARTYPE vt;
} PARAMDATA, * LPPARAMDATA;
typedef struct tagMETHODDATA {
 OLECHAR * szName;
 PARAMDATA * ppdata;
 DISPID dispid;
 UINT iMeth;
 CALLCONV cc;
 UINT cArgs;
 WORD wFlags;
 VARTYPE vtReturn;
} METHODDATA, * LPMETHODDATA;
typedef struct tagINTERFACEDATA {
 METHODDATA * pmethdata;
 UINT cMembers;
} INTERFACEDATA, * LPINTERFACEDATA;
extern "C" __declspec(dllimport) HRESULT __stdcall DispGetParam(
 DISPPARAMS * pdispparams,
 UINT position,
 VARTYPE vtTarg,
 VARIANT * pvarResult,
 UINT * puArgErr
 );
 extern "C" __declspec(dllimport) HRESULT __stdcall DispGetIDsOfNames(ITypeInfo * ptinfo, OLECHAR ** rgszNames,
 UINT cNames, DISPID * rgdispid);
extern "C" __declspec(dllimport) HRESULT __stdcall DispInvoke(void * _this, ITypeInfo * ptinfo, DISPID dispidMember,
 WORD wFlags, DISPPARAMS * pparams, VARIANT * pvarResult,
 EXCEPINFO * pexcepinfo, UINT * puArgErr);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateDispTypeInfo(INTERFACEDATA * pidata, LCID lcid,
 ITypeInfo ** pptinfo);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateStdDispatch(IUnknown * punkOuter, void * pvThis,
 ITypeInfo * ptinfo, IUnknown ** ppunkStdDisp);
extern "C" __declspec(dllimport) HRESULT __stdcall DispCallFunc(void * pvInstance, ULONG_PTR oVft, CALLCONV cc,
 VARTYPE vtReturn, UINT cActuals, VARTYPE * prgvt,
 VARIANTARG ** prgpvarg, VARIANT * pvargResult);
extern "C" __declspec(dllimport) HRESULT __stdcall RegisterActiveObject(IUnknown * punk, const IID & rclsid,
 DWORD dwFlags, DWORD * pdwRegister);
extern "C" __declspec(dllimport) HRESULT __stdcall RevokeActiveObject(DWORD dwRegister, void * pvReserved);
extern "C" __declspec(dllimport) HRESULT __stdcall GetActiveObject(const IID & rclsid, void * pvReserved,
 IUnknown ** ppunk);
extern "C" __declspec(dllimport) HRESULT __stdcall SetErrorInfo( ULONG dwReserved, IErrorInfo * perrinfo);
extern "C" __declspec(dllimport) HRESULT __stdcall GetErrorInfo( ULONG dwReserved, IErrorInfo ** pperrinfo);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateErrorInfo( ICreateErrorInfo ** pperrinfo);
extern "C" __declspec(dllimport) HRESULT __stdcall GetRecordInfoFromTypeInfo(ITypeInfo * pTypeInfo,
 IRecordInfo ** ppRecInfo);
extern "C" __declspec(dllimport) HRESULT __stdcall GetRecordInfoFromGuids(const GUID & rGuidTypeLib,
 ULONG uVerMajor, ULONG uVerMinor, LCID lcid,
 const GUID & rGuidTypeInfo, IRecordInfo ** ppRecInfo);
extern "C" __declspec(dllimport) ULONG __stdcall OaBuildVersion(void);
extern "C" __declspec(dllimport) void __stdcall ClearCustData(LPCUSTDATA pCustData);
#pragma warning(disable:4103)
#pragma pack(pop)
extern "C" __declspec(dllimport) HRESULT __stdcall CreateDataAdviseHolder( LPDATAADVISEHOLDER * ppDAHolder);
extern "C" __declspec(dllimport) DWORD __stdcall OleBuildVersion( void );
extern "C" __declspec(dllimport) HRESULT __stdcall ReadClassStg( LPSTORAGE pStg, CLSID * pclsid);
extern "C" __declspec(dllimport) HRESULT __stdcall WriteClassStg( LPSTORAGE pStg, const IID & rclsid);
extern "C" __declspec(dllimport) HRESULT __stdcall ReadClassStm( LPSTREAM pStm, CLSID * pclsid);
extern "C" __declspec(dllimport) HRESULT __stdcall WriteClassStm( LPSTREAM pStm, const IID & rclsid);
 extern "C" __declspec(dllimport) HRESULT __stdcall WriteFmtUserTypeStg ( LPSTORAGE pstg, CLIPFORMAT cf, LPOLESTR lpszUserType);
extern "C" __declspec(dllimport) HRESULT __stdcall ReadFmtUserTypeStg ( LPSTORAGE pstg, CLIPFORMAT * pcf, LPOLESTR * lplpszUserType);
extern "C" __declspec(dllimport) HRESULT __stdcall OleInitialize( LPVOID pvReserved);
extern "C" __declspec(dllimport) void __stdcall OleUninitialize(void);
extern "C" __declspec(dllimport) HRESULT __stdcall OleQueryLinkFromData( LPDATAOBJECT pSrcDataObject);
extern "C" __declspec(dllimport) HRESULT __stdcall OleQueryCreateFromData( LPDATAOBJECT pSrcDataObject);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreate( const IID & rclsid, const IID & riid, DWORD renderopt,
 LPFORMATETC pFormatEtc, LPOLECLIENTSITE pClientSite,
 LPSTORAGE pStg, LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreateEx( const IID & rclsid, const IID & riid, DWORD dwFlags,
 DWORD renderopt, ULONG cFormats, DWORD* rgAdvf,
 LPFORMATETC rgFormatEtc, IAdviseSink * lpAdviseSink,
 DWORD * rgdwConnection, LPOLECLIENTSITE pClientSite,
 LPSTORAGE pStg, LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreateFromData( LPDATAOBJECT pSrcDataObj, const IID & riid,
 DWORD renderopt, LPFORMATETC pFormatEtc,
 LPOLECLIENTSITE pClientSite, LPSTORAGE pStg,
 LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreateFromDataEx( LPDATAOBJECT pSrcDataObj, const IID & riid,
 DWORD dwFlags, DWORD renderopt, ULONG cFormats, DWORD* rgAdvf,
 LPFORMATETC rgFormatEtc, IAdviseSink * lpAdviseSink,
 DWORD * rgdwConnection, LPOLECLIENTSITE pClientSite,
 LPSTORAGE pStg, LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreateLinkFromData( LPDATAOBJECT pSrcDataObj, const IID & riid,
 DWORD renderopt, LPFORMATETC pFormatEtc,
 LPOLECLIENTSITE pClientSite, LPSTORAGE pStg,
 LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreateLinkFromDataEx( LPDATAOBJECT pSrcDataObj, const IID & riid,
 DWORD dwFlags, DWORD renderopt, ULONG cFormats, DWORD* rgAdvf,
 LPFORMATETC rgFormatEtc, IAdviseSink * lpAdviseSink,
 DWORD * rgdwConnection, LPOLECLIENTSITE pClientSite,
 LPSTORAGE pStg, LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreateStaticFromData( LPDATAOBJECT pSrcDataObj, const IID & iid,
 DWORD renderopt, LPFORMATETC pFormatEtc,
 LPOLECLIENTSITE pClientSite, LPSTORAGE pStg,
 LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreateLink( LPMONIKER pmkLinkSrc, const IID & riid,
 DWORD renderopt, LPFORMATETC lpFormatEtc,
 LPOLECLIENTSITE pClientSite, LPSTORAGE pStg, LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreateLinkEx( LPMONIKER pmkLinkSrc, const IID & riid,
 DWORD dwFlags, DWORD renderopt, ULONG cFormats, DWORD* rgAdvf,
 LPFORMATETC rgFormatEtc, IAdviseSink * lpAdviseSink,
 DWORD * rgdwConnection, LPOLECLIENTSITE pClientSite,
 LPSTORAGE pStg, LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreateLinkToFile( LPCOLESTR lpszFileName, const IID & riid,
 DWORD renderopt, LPFORMATETC lpFormatEtc,
 LPOLECLIENTSITE pClientSite, LPSTORAGE pStg, LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreateLinkToFileEx( LPCOLESTR lpszFileName, const IID & riid,
 DWORD dwFlags, DWORD renderopt, ULONG cFormats, DWORD* rgAdvf,
 LPFORMATETC rgFormatEtc, IAdviseSink * lpAdviseSink,
 DWORD * rgdwConnection, LPOLECLIENTSITE pClientSite,
 LPSTORAGE pStg, LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreateFromFile( const IID & rclsid, LPCOLESTR lpszFileName, const IID & riid,
 DWORD renderopt, LPFORMATETC lpFormatEtc,
 LPOLECLIENTSITE pClientSite, LPSTORAGE pStg, LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreateFromFileEx( const IID & rclsid, LPCOLESTR lpszFileName, const IID & riid,
 DWORD dwFlags, DWORD renderopt, ULONG cFormats, DWORD* rgAdvf,
 LPFORMATETC rgFormatEtc, IAdviseSink * lpAdviseSink,
 DWORD * rgdwConnection, LPOLECLIENTSITE pClientSite,
 LPSTORAGE pStg, LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleLoad( LPSTORAGE pStg, const IID & riid, LPOLECLIENTSITE pClientSite,
 LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleSave( LPPERSISTSTORAGE pPS, LPSTORAGE pStg, BOOL fSameAsLoad);
extern "C" __declspec(dllimport) HRESULT __stdcall OleLoadFromStream( LPSTREAM pStm, const IID & iidInterface, LPVOID * ppvObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleSaveToStream( LPPERSISTSTREAM pPStm, LPSTREAM pStm );
extern "C" __declspec(dllimport) HRESULT __stdcall OleSetContainedObject( LPUNKNOWN pUnknown, BOOL fContained);
extern "C" __declspec(dllimport) HRESULT __stdcall OleNoteObjectVisible( LPUNKNOWN pUnknown, BOOL fVisible);
extern "C" __declspec(dllimport) HRESULT __stdcall RegisterDragDrop( HWND hwnd, LPDROPTARGET pDropTarget);
extern "C" __declspec(dllimport) HRESULT __stdcall RevokeDragDrop( HWND hwnd);
extern "C" __declspec(dllimport) HRESULT __stdcall DoDragDrop( LPDATAOBJECT pDataObj, LPDROPSOURCE pDropSource,
 DWORD dwOKEffects, LPDWORD pdwEffect);
extern "C" __declspec(dllimport) HRESULT __stdcall OleSetClipboard( LPDATAOBJECT pDataObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleGetClipboard( LPDATAOBJECT * ppDataObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleFlushClipboard(void);
extern "C" __declspec(dllimport) HRESULT __stdcall OleIsCurrentClipboard( LPDATAOBJECT pDataObj);
extern "C" __declspec(dllimport) HOLEMENU __stdcall OleCreateMenuDescriptor ( HMENU hmenuCombined,
 LPOLEMENUGROUPWIDTHS lpMenuWidths);
extern "C" __declspec(dllimport) HRESULT __stdcall OleSetMenuDescriptor ( HOLEMENU holemenu, HWND hwndFrame,
 HWND hwndActiveObject,
 LPOLEINPLACEFRAME lpFrame,
 LPOLEINPLACEACTIVEOBJECT lpActiveObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleDestroyMenuDescriptor ( HOLEMENU holemenu);
extern "C" __declspec(dllimport) HRESULT __stdcall OleTranslateAccelerator ( LPOLEINPLACEFRAME lpFrame,
 LPOLEINPLACEFRAMEINFO lpFrameInfo, LPMSG lpmsg);
extern "C" __declspec(dllimport) HANDLE __stdcall OleDuplicateData ( HANDLE hSrc, CLIPFORMAT cfFormat,
 UINT uiFlags);
extern "C" __declspec(dllimport) HRESULT __stdcall OleDraw ( LPUNKNOWN pUnknown, DWORD dwAspect, HDC hdcDraw,
 LPCRECT lprcBounds);
extern "C" __declspec(dllimport) HRESULT __stdcall OleRun( LPUNKNOWN pUnknown);
extern "C" __declspec(dllimport) BOOL __stdcall OleIsRunning( LPOLEOBJECT pObject);
extern "C" __declspec(dllimport) HRESULT __stdcall OleLockRunning( LPUNKNOWN pUnknown, BOOL fLock, BOOL fLastUnlockCloses);
extern "C" __declspec(dllimport) void __stdcall ReleaseStgMedium( LPSTGMEDIUM);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateOleAdviseHolder( LPOLEADVISEHOLDER * ppOAHolder);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreateDefaultHandler( const IID & clsid, LPUNKNOWN pUnkOuter,
 const IID & riid, LPVOID * lplpObj);
extern "C" __declspec(dllimport) HRESULT __stdcall OleCreateEmbeddingHelper( const IID & clsid, LPUNKNOWN pUnkOuter,
 DWORD flags, LPCLASSFACTORY pCF,
 const IID & riid, LPVOID * lplpObj);
extern "C" __declspec(dllimport) BOOL __stdcall IsAccelerator( HACCEL hAccel, int cAccelEntries, LPMSG lpMsg,
 WORD * lpwCmd);
extern "C" __declspec(dllimport) HGLOBAL __stdcall OleGetIconOfFile( LPOLESTR lpszPath, BOOL fUseFileAsLabel);
extern "C" __declspec(dllimport) HGLOBAL __stdcall OleGetIconOfClass( const IID & rclsid, LPOLESTR lpszLabel,
 BOOL fUseTypeAsLabel);
extern "C" __declspec(dllimport) HGLOBAL __stdcall OleMetafilePictFromIconAndLabel( HICON hIcon, LPOLESTR lpszLabel,
 LPOLESTR lpszSourceFile, UINT iIconIndex);
 extern "C" __declspec(dllimport) HRESULT __stdcall OleRegGetUserType ( const IID & clsid, DWORD dwFormOfType,
 LPOLESTR * pszUserType);
extern "C" __declspec(dllimport) HRESULT __stdcall OleRegGetMiscStatus ( const IID & clsid, DWORD dwAspect,
 DWORD * pdwStatus);
extern "C" __declspec(dllimport) HRESULT __stdcall OleRegEnumFormatEtc ( const IID & clsid, DWORD dwDirection,
 LPENUMFORMATETC * ppenum);
extern "C" __declspec(dllimport) HRESULT __stdcall OleRegEnumVerbs ( const IID & clsid, LPENUMOLEVERB * ppenum);
typedef struct _OLESTREAM * LPOLESTREAM;
typedef struct _OLESTREAMVTBL
{
 DWORD (__stdcall* Get)(LPOLESTREAM, void *, DWORD);
 DWORD (__stdcall* Put)(LPOLESTREAM, const void *, DWORD);
} OLESTREAMVTBL;
typedef OLESTREAMVTBL * LPOLESTREAMVTBL;
typedef struct _OLESTREAM
{
 LPOLESTREAMVTBL lpstbl;
} OLESTREAM;
extern "C" __declspec(dllimport) HRESULT __stdcall OleConvertOLESTREAMToIStorage
 ( LPOLESTREAM lpolestream,
 LPSTORAGE pstg,
 const DVTARGETDEVICE * ptd);
extern "C" __declspec(dllimport) HRESULT __stdcall OleConvertIStorageToOLESTREAM
 ( LPSTORAGE pstg,
 LPOLESTREAM lpolestream);
extern "C" __declspec(dllimport) HRESULT __stdcall GetHGlobalFromILockBytes ( LPLOCKBYTES plkbyt, HGLOBAL * phglobal);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateILockBytesOnHGlobal ( HGLOBAL hGlobal, BOOL fDeleteOnRelease,
 LPLOCKBYTES * pplkbyt);
extern "C" __declspec(dllimport) HRESULT __stdcall GetHGlobalFromStream ( LPSTREAM pstm, HGLOBAL * phglobal);
extern "C" __declspec(dllimport) HRESULT __stdcall CreateStreamOnHGlobal ( HGLOBAL hGlobal, BOOL fDeleteOnRelease,
 LPSTREAM * ppstm);
extern "C" __declspec(dllimport) HRESULT __stdcall OleDoAutoConvert( LPSTORAGE pStg, LPCLSID pClsidNew);
extern "C" __declspec(dllimport) HRESULT __stdcall OleGetAutoConvert( const IID & clsidOld, LPCLSID pClsidNew);
extern "C" __declspec(dllimport) HRESULT __stdcall OleSetAutoConvert( const IID & clsidOld, const IID & clsidNew);
extern "C" __declspec(dllimport) HRESULT __stdcall GetConvertStg( LPSTORAGE pStg);
extern "C" __declspec(dllimport) HRESULT __stdcall SetConvertStg( LPSTORAGE pStg, BOOL fConvert);
extern "C" __declspec(dllimport) HRESULT __stdcall OleConvertIStorageToOLESTREAMEx
 ( LPSTORAGE pstg,
 CLIPFORMAT cfFormat,
 LONG lWidth,
 LONG lHeight,
 DWORD dwSize,
 LPSTGMEDIUM pmedium,
 LPOLESTREAM polestm);
extern "C" __declspec(dllimport) HRESULT __stdcall OleConvertOLESTREAMToIStorageEx
 ( LPOLESTREAM polestm,
 LPSTORAGE pstg,
 CLIPFORMAT * pcfFormat,
 LONG * plwWidth,
 LONG * plHeight,
 DWORD * pdwSize,
 LPSTGMEDIUM pmedium);
#pragma warning(disable:4103)
#pragma pack(pop)
extern "C" {
struct __declspec(novtable) IAVIStream : public IUnknown
{
 virtual __declspec(nothrow) HRESULT __stdcall QueryInterface ( const IID & riid, LPVOID * ppvObj) = 0;
 virtual __declspec(nothrow) ULONG __stdcall AddRef (void) = 0;
 virtual __declspec(nothrow) ULONG __stdcall Release (void) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall Create ( LPARAM lParam1, LPARAM lParam2) = 0 ;
 virtual __declspec(nothrow) HRESULT __stdcall Info ( AVISTREAMINFOW * psi, LONG lSize) = 0 ;
 virtual __declspec(nothrow) LONG __stdcall FindSample( LONG lPos, LONG lFlags) = 0 ;
 virtual __declspec(nothrow) HRESULT __stdcall ReadFormat ( LONG lPos,
 LPVOID lpFormat, LONG *lpcbFormat) = 0 ;
 virtual __declspec(nothrow) HRESULT __stdcall SetFormat ( LONG lPos,
 LPVOID lpFormat, LONG cbFormat) = 0 ;
 virtual __declspec(nothrow) HRESULT __stdcall Read ( LONG lStart, LONG lSamples,
 LPVOID lpBuffer, LONG cbBuffer,
 LONG * plBytes, LONG * plSamples) = 0 ;
 virtual __declspec(nothrow) HRESULT __stdcall Write ( LONG lStart, LONG lSamples,
 LPVOID lpBuffer, LONG cbBuffer,
 DWORD dwFlags,
 LONG *plSampWritten,
 LONG *plBytesWritten) = 0 ;
 virtual __declspec(nothrow) HRESULT __stdcall Delete ( LONG lStart, LONG lSamples) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall ReadData ( DWORD fcc, LPVOID lp, LONG *lpcb) = 0 ;
 virtual __declspec(nothrow) HRESULT __stdcall WriteData ( DWORD fcc, LPVOID lp, LONG cb) = 0 ;
 virtual __declspec(nothrow) HRESULT __stdcall SetInfo ( AVISTREAMINFOW * lpInfo,
 LONG cbInfo) = 0;
};
typedef IAVIStream * PAVISTREAM;
struct __declspec(novtable) IAVIStreaming : public IUnknown
{
 virtual __declspec(nothrow) HRESULT __stdcall QueryInterface ( const IID & riid, LPVOID * ppvObj) = 0;
 virtual __declspec(nothrow) ULONG __stdcall AddRef (void) = 0;
 virtual __declspec(nothrow) ULONG __stdcall Release (void) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall Begin (
 LONG lStart,
 LONG lEnd,
 LONG lRate) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall End (void) = 0;
};
typedef IAVIStreaming * PAVISTREAMING;
struct __declspec(novtable) IAVIEditStream : public IUnknown
{
 virtual __declspec(nothrow) HRESULT __stdcall QueryInterface ( const IID & riid, LPVOID * ppvObj) = 0;
 virtual __declspec(nothrow) ULONG __stdcall AddRef (void) = 0;
 virtual __declspec(nothrow) ULONG __stdcall Release (void) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall Cut ( LONG *plStart,
 LONG *plLength,
 PAVISTREAM * ppResult) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall Copy ( LONG *plStart,
 LONG *plLength,
 PAVISTREAM * ppResult) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall Paste ( LONG *plPos,
 LONG *plLength,
 PAVISTREAM pstream,
 LONG lStart,
 LONG lEnd) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall Clone ( PAVISTREAM *ppResult) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall SetInfo ( AVISTREAMINFOW * lpInfo,
 LONG cbInfo) = 0;
};
typedef IAVIEditStream * PAVIEDITSTREAM;
struct __declspec(novtable) IAVIPersistFile : public IPersistFile
{
 virtual __declspec(nothrow) HRESULT __stdcall Reserved1(void) = 0;
};
typedef IAVIPersistFile * PAVIPERSISTFILE;
struct __declspec(novtable) IAVIFile : public IUnknown
{
 virtual __declspec(nothrow) HRESULT __stdcall QueryInterface ( const IID & riid, LPVOID * ppvObj) = 0;
 virtual __declspec(nothrow) ULONG __stdcall AddRef (void) = 0;
 virtual __declspec(nothrow) ULONG __stdcall Release (void) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall Info (
 AVIFILEINFOW * pfi,
 LONG lSize) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall GetStream (
 PAVISTREAM * ppStream,
 DWORD fccType,
 LONG lParam) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall CreateStream (
 PAVISTREAM * ppStream,
 AVISTREAMINFOW * psi) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall WriteData (
 DWORD ckid,
 LPVOID lpData,
 LONG cbData) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall ReadData (
 DWORD ckid,
 LPVOID lpData,
 LONG *lpcbData) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall EndRecord (void) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall DeleteStream (
 DWORD fccType,
 LONG lParam) = 0;
};
typedef IAVIFile * PAVIFILE;
struct __declspec(novtable) IGetFrame : public IUnknown
{
 virtual __declspec(nothrow) HRESULT __stdcall QueryInterface ( const IID & riid, LPVOID * ppvObj) = 0;
 virtual __declspec(nothrow) ULONG __stdcall AddRef (void) = 0;
 virtual __declspec(nothrow) ULONG __stdcall Release (void) = 0;
 virtual __declspec(nothrow) LPVOID __stdcall GetFrame ( LONG lPos) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall Begin ( LONG lStart, LONG lEnd, LONG lRate) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall End (void) = 0;
 virtual __declspec(nothrow) HRESULT __stdcall SetFormat ( LPBITMAPINFOHEADER lpbi, LPVOID lpBits, int x, int y, int dx, int dy) = 0;
};
typedef IGetFrame * PGETFRAME;
extern "C" const GUID IID_IAVIFile;
extern "C" const GUID IID_IAVIStream;
extern "C" const GUID IID_IAVIStreaming;
extern "C" const GUID IID_IGetFrame;
extern "C" const GUID IID_IAVIEditStream;
extern "C" const GUID IID_IAVIPersistFile;
extern "C" const GUID CLSID_AVISimpleUnMarshal;
extern "C" const GUID CLSID_AVIFile;
extern "C" void __stdcall AVIFileInit(void);
extern "C" void __stdcall AVIFileExit(void);
extern "C" ULONG __stdcall AVIFileAddRef (PAVIFILE pfile);
extern "C" ULONG __stdcall AVIFileRelease (PAVIFILE pfile);
extern "C" HRESULT __stdcall AVIFileOpenA ( PAVIFILE * ppfile, LPCSTR szFile,
 UINT uMode, LPCLSID lpHandler);
extern "C" HRESULT __stdcall AVIFileOpenW ( PAVIFILE * ppfile, LPCWSTR szFile,
 UINT uMode, LPCLSID lpHandler);
extern "C" HRESULT __stdcall AVIFileInfoW ( PAVIFILE pfile, LPAVIFILEINFOW pfi, LONG lSize);
extern "C" HRESULT __stdcall AVIFileInfoA ( PAVIFILE pfile, LPAVIFILEINFOA pfi, LONG lSize);
extern "C" HRESULT __stdcall AVIFileGetStream ( PAVIFILE pfile, PAVISTREAM * ppavi, DWORD fccType, LONG lParam);
extern "C" HRESULT __stdcall AVIFileCreateStreamW ( PAVIFILE pfile, PAVISTREAM *ppavi, AVISTREAMINFOW * psi);
extern "C" HRESULT __stdcall AVIFileCreateStreamA ( PAVIFILE pfile, PAVISTREAM *ppavi, AVISTREAMINFOA * psi);
extern "C" HRESULT __stdcall AVIFileWriteData ( PAVIFILE pfile,
 DWORD ckid,
 LPVOID lpData,
 LONG cbData);
extern "C" HRESULT __stdcall AVIFileReadData ( PAVIFILE pfile,
 DWORD ckid,
 LPVOID lpData,
 LONG *lpcbData);
extern "C" HRESULT __stdcall AVIFileEndRecord ( PAVIFILE pfile);
extern "C" ULONG __stdcall AVIStreamAddRef (PAVISTREAM pavi);
extern "C" ULONG __stdcall AVIStreamRelease (PAVISTREAM pavi);
extern "C" HRESULT __stdcall AVIStreamInfoW ( PAVISTREAM pavi, LPAVISTREAMINFOW psi, LONG lSize);
extern "C" HRESULT __stdcall AVIStreamInfoA ( PAVISTREAM pavi, LPAVISTREAMINFOA psi, LONG lSize);
extern "C" LONG __stdcall AVIStreamFindSample( PAVISTREAM pavi, LONG lPos, LONG lFlags);
extern "C" HRESULT __stdcall AVIStreamReadFormat ( PAVISTREAM pavi, LONG lPos, LPVOID lpFormat, LONG *lpcbFormat);
extern "C" HRESULT __stdcall AVIStreamSetFormat ( PAVISTREAM pavi, LONG lPos, LPVOID lpFormat, LONG cbFormat);
extern "C" HRESULT __stdcall AVIStreamReadData ( PAVISTREAM pavi, DWORD fcc, LPVOID lp, LONG *lpcb);
extern "C" HRESULT __stdcall AVIStreamWriteData ( PAVISTREAM pavi, DWORD fcc, LPVOID lp, LONG cb);
extern "C" HRESULT __stdcall AVIStreamRead ( PAVISTREAM pavi,
 LONG lStart,
 LONG lSamples,
 LPVOID lpBuffer,
 LONG cbBuffer,
 LONG * plBytes,
 LONG * plSamples);
extern "C" HRESULT __stdcall AVIStreamWrite ( PAVISTREAM pavi,
 LONG lStart, LONG lSamples,
 LPVOID lpBuffer, LONG cbBuffer, DWORD dwFlags,
 LONG *plSampWritten,
 LONG *plBytesWritten);
extern "C" LONG __stdcall AVIStreamStart ( PAVISTREAM pavi);
extern "C" LONG __stdcall AVIStreamLength ( PAVISTREAM pavi);
extern "C" LONG __stdcall AVIStreamTimeToSample ( PAVISTREAM pavi, LONG lTime);
extern "C" LONG __stdcall AVIStreamSampleToTime ( PAVISTREAM pavi, LONG lSample);
extern "C" HRESULT __stdcall AVIStreamBeginStreaming( PAVISTREAM pavi, LONG lStart, LONG lEnd, LONG lRate);
extern "C" HRESULT __stdcall AVIStreamEndStreaming( PAVISTREAM pavi);
extern "C" PGETFRAME __stdcall AVIStreamGetFrameOpen( PAVISTREAM pavi,
 LPBITMAPINFOHEADER lpbiWanted);
extern "C" LPVOID __stdcall AVIStreamGetFrame( PGETFRAME pg, LONG lPos);
extern "C" HRESULT __stdcall AVIStreamGetFrameClose( PGETFRAME pg);
extern "C" HRESULT __stdcall AVIStreamOpenFromFileA( PAVISTREAM *ppavi, LPCSTR szFile,
 DWORD fccType, LONG lParam,
 UINT mode, CLSID *pclsidHandler);
extern "C" HRESULT __stdcall AVIStreamOpenFromFileW( PAVISTREAM *ppavi, LPCWSTR szFile,
 DWORD fccType, LONG lParam,
 UINT mode, CLSID *pclsidHandler);
extern "C" HRESULT __stdcall AVIStreamCreate( PAVISTREAM *ppavi, LONG lParam1, LONG lParam2,
 CLSID *pclsidHandler);
extern "C" HRESULT __stdcall AVIMakeCompressedStream(
 PAVISTREAM * ppsCompressed,
 PAVISTREAM ppsSource,
 AVICOMPRESSOPTIONS * lpOptions,
 CLSID *pclsidHandler);
extern "C" HRESULT AVISaveA (LPCSTR szFile,
 CLSID *pclsidHandler,
 AVISAVECALLBACK lpfnCallback,
 int nStreams,
 PAVISTREAM pfile,
 LPAVICOMPRESSOPTIONS lpOptions,
 ...);
extern "C" HRESULT __stdcall AVISaveVA(LPCSTR szFile,
 CLSID *pclsidHandler,
 AVISAVECALLBACK lpfnCallback,
 int nStreams,
 PAVISTREAM * ppavi,
 LPAVICOMPRESSOPTIONS *plpOptions);
extern "C" HRESULT AVISaveW (LPCWSTR szFile,
 CLSID *pclsidHandler,
 AVISAVECALLBACK lpfnCallback,
 int nStreams,
 PAVISTREAM pfile,
 LPAVICOMPRESSOPTIONS lpOptions,
 ...);
extern "C" HRESULT __stdcall AVISaveVW(LPCWSTR szFile,
 CLSID *pclsidHandler,
 AVISAVECALLBACK lpfnCallback,
 int nStreams,
 PAVISTREAM * ppavi,
 LPAVICOMPRESSOPTIONS *plpOptions);
extern "C" INT_PTR __stdcall AVISaveOptions( HWND hwnd,
 UINT uiFlags,
 int nStreams,
 PAVISTREAM *ppavi,
 LPAVICOMPRESSOPTIONS *plpOptions);
extern "C" HRESULT __stdcall AVISaveOptionsFree(int nStreams,
 LPAVICOMPRESSOPTIONS *plpOptions);
extern "C" HRESULT __stdcall AVIBuildFilterW( LPWSTR lpszFilter, LONG cbFilter, BOOL fSaving);
extern "C" HRESULT __stdcall AVIBuildFilterA( LPSTR lpszFilter, LONG cbFilter, BOOL fSaving);
extern "C" HRESULT __stdcall AVIMakeFileFromStreams( PAVIFILE * ppfile,
 int nStreams,
 PAVISTREAM * papStreams);
extern "C" HRESULT __stdcall AVIMakeStreamFromClipboard(UINT cfFormat, HANDLE hGlobal, PAVISTREAM *ppstream);
extern "C" HRESULT __stdcall AVIPutFileOnClipboard( PAVIFILE pf);
extern "C" HRESULT __stdcall AVIGetFromClipboard( PAVIFILE * lppf);
extern "C" HRESULT __stdcall AVIClearClipboard(void);
extern "C" HRESULT __stdcall CreateEditableStream(
 PAVISTREAM * ppsEditable,
 PAVISTREAM psSource);
extern "C" HRESULT __stdcall EditStreamCut( PAVISTREAM pavi, LONG *plStart, LONG *plLength, PAVISTREAM * ppResult);
extern "C" HRESULT __stdcall EditStreamCopy( PAVISTREAM pavi, LONG *plStart, LONG *plLength, PAVISTREAM * ppResult);
extern "C" HRESULT __stdcall EditStreamPaste( PAVISTREAM pavi, LONG *plPos, LONG *plLength, PAVISTREAM pstream, LONG lStart, LONG lEnd);
extern "C" HRESULT __stdcall EditStreamClone( PAVISTREAM pavi, PAVISTREAM *ppResult);
extern "C" HRESULT __stdcall EditStreamSetNameA( PAVISTREAM pavi, LPCSTR lpszName);
extern "C" HRESULT __stdcall EditStreamSetNameW( PAVISTREAM pavi, LPCWSTR lpszName);
extern "C" HRESULT __stdcall EditStreamSetInfoW( PAVISTREAM pavi, LPAVISTREAMINFOW lpInfo, LONG cbInfo);
extern "C" HRESULT __stdcall EditStreamSetInfoA( PAVISTREAM pavi, LPAVISTREAMINFOA lpInfo, LONG cbInfo);
struct HVIDEO__ { int unused; }; typedef struct HVIDEO__ *HVIDEO;
typedef HVIDEO * LPHVIDEO;
DWORD __stdcall VideoForWindowsVersion(void);
typedef struct videohdr_tag {
 LPBYTE lpData;
 DWORD dwBufferLength;
 DWORD dwBytesUsed;
 DWORD dwTimeCaptured;
 DWORD_PTR dwUser;
 DWORD dwFlags;
 DWORD_PTR dwReserved[4];
} VIDEOHDR, *PVIDEOHDR, * LPVIDEOHDR;
typedef struct channel_caps_tag {
 DWORD dwFlags;
 DWORD dwSrcRectXMod;
 DWORD dwSrcRectYMod;
 DWORD dwSrcRectWidthMod;
 DWORD dwSrcRectHeightMod;
 DWORD dwDstRectXMod;
 DWORD dwDstRectYMod;
 DWORD dwDstRectWidthMod;
 DWORD dwDstRectHeightMod;
} CHANNEL_CAPS, *PCHANNEL_CAPS, * LPCHANNEL_CAPS;
typedef struct tagCapDriverCaps {
 UINT wDeviceIndex;
 BOOL fHasOverlay;
 BOOL fHasDlgVideoSource;
 BOOL fHasDlgVideoFormat;
 BOOL fHasDlgVideoDisplay;
 BOOL fCaptureInitialized;
 BOOL fDriverSuppliesPalettes;
 HANDLE hVideoIn;
 HANDLE hVideoOut;
 HANDLE hVideoExtIn;
 HANDLE hVideoExtOut;
} CAPDRIVERCAPS, *PCAPDRIVERCAPS, *LPCAPDRIVERCAPS;
typedef struct tagCapStatus {
 UINT uiImageWidth;
 UINT uiImageHeight;
 BOOL fLiveWindow;
 BOOL fOverlayWindow;
 BOOL fScale;
 POINT ptScroll;
 BOOL fUsingDefaultPalette;
 BOOL fAudioHardware;
 BOOL fCapFileExists;
 DWORD dwCurrentVideoFrame;
 DWORD dwCurrentVideoFramesDropped;
 DWORD dwCurrentWaveSamples;
 DWORD dwCurrentTimeElapsedMS;
 HPALETTE hPalCurrent;
 BOOL fCapturingNow;
 DWORD dwReturn;
 UINT wNumVideoAllocated;
 UINT wNumAudioAllocated;
} CAPSTATUS, *PCAPSTATUS, *LPCAPSTATUS;
typedef struct tagCaptureParms {
 DWORD dwRequestMicroSecPerFrame;
 BOOL fMakeUserHitOKToCapture;
 UINT wPercentDropForError;
 BOOL fYield;
 DWORD dwIndexSize;
 UINT wChunkGranularity;
 BOOL fUsingDOSMemory;
 UINT wNumVideoRequested;
 BOOL fCaptureAudio;
 UINT wNumAudioRequested;
 UINT vKeyAbort;
 BOOL fAbortLeftMouse;
 BOOL fAbortRightMouse;
 BOOL fLimitEnabled;
 UINT wTimeLimit;
 BOOL fMCIControl;
 BOOL fStepMCIDevice;
 DWORD dwMCIStartTime;
 DWORD dwMCIStopTime;
 BOOL fStepCaptureAt2x;
 UINT wStepCaptureAverageFrames;
 DWORD dwAudioBufferSize;
 BOOL fDisableWriteCache;
 UINT AVStreamMaster;
} CAPTUREPARMS, *PCAPTUREPARMS, *LPCAPTUREPARMS;
typedef struct tagCapInfoChunk {
 FOURCC fccInfoID;
 LPVOID lpData;
 LONG cbData;
} CAPINFOCHUNK, *PCAPINFOCHUNK, *LPCAPINFOCHUNK;
typedef LRESULT (__stdcall* CAPYIELDCALLBACK) ( HWND hWnd);
typedef LRESULT (__stdcall* CAPSTATUSCALLBACKW) ( HWND hWnd, int nID, LPCWSTR lpsz);
typedef LRESULT (__stdcall* CAPERRORCALLBACKW) ( HWND hWnd, int nID, LPCWSTR lpsz);
typedef LRESULT (__stdcall* CAPSTATUSCALLBACKA) ( HWND hWnd, int nID, LPCSTR lpsz);
typedef LRESULT (__stdcall* CAPERRORCALLBACKA) ( HWND hWnd, int nID, LPCSTR lpsz);
typedef LRESULT (__stdcall* CAPVIDEOCALLBACK) ( HWND hWnd, LPVIDEOHDR lpVHdr);
typedef LRESULT (__stdcall* CAPWAVECALLBACK) ( HWND hWnd, LPWAVEHDR lpWHdr);
typedef LRESULT (__stdcall* CAPCONTROLCALLBACK)( HWND hWnd, int nState);
HWND __stdcall capCreateCaptureWindowA (
 LPCSTR lpszWindowName,
 DWORD dwStyle,
 int x, int y, int nWidth, int nHeight,
 HWND hwndParent, int nID);
BOOL __stdcall capGetDriverDescriptionA (UINT wDriverIndex,
 LPSTR lpszName, int cbName,
 LPSTR lpszVer, int cbVer);
HWND __stdcall capCreateCaptureWindowW (
 LPCWSTR lpszWindowName,
 DWORD dwStyle,
 int x, int y, int nWidth, int nHeight,
 HWND hwndParent, int nID);
BOOL __stdcall capGetDriverDescriptionW (UINT wDriverIndex,
 LPWSTR lpszName, int cbName,
 LPWSTR lpszVer, int cbVer);
}
extern "C" {
 BOOL
 __stdcall
 GetOpenFileNamePreviewA(
 LPOPENFILENAMEA lpofn
 );
 BOOL
 __stdcall
 GetSaveFileNamePreviewA(
 LPOPENFILENAMEA lpofn
 );
 BOOL
 __stdcall
 GetOpenFileNamePreviewW(
 LPOPENFILENAMEW lpofn
 );
 BOOL
 __stdcall
 GetSaveFileNamePreviewW(
 LPOPENFILENAMEW lpofn
 );
#pragma warning(disable:4103)
#pragma pack(pop)
}

