
#define SPG_RELEASECONFIG

#define SPG_SAFECONV __cdecl
#define SPG_CONV __fastcall
#if(_MSC_VER>=1300)
//VS2005
#define SPG_FASTCONV __fastcall
#else
//VC6
#define SPG_FASTCONV __fastcall
#endif
#define SPG_VIDEOCHANGE

