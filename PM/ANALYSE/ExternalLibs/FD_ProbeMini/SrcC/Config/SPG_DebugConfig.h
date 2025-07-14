
#define SPG_DEBUGCONFIG

#define DebugMem
#define HookMem
#define DebugMeca
#define DebugSGRAPH
#define DebugGraphics
#define DebugRender
#define DebugList
#define DebugFloat
#define DebugFloatHard
#define DebugTimer
#define DebugNetwork
#define DebugDBLBUFF
#define SGE_DrawNormales

//ragma message("DebugList")

#ifdef SPG_General_USETimer
#define DebugProfil3DTimer
#define DebugMelinkTimer

#define DebugProgPrincipalTimer//obligatoire pour les timers de global.h
#define DebugRenderTimer
#define DebugGraphicsTimer
#define DebugProfilManagerTimer
#define DebugNetworkTimer
#endif


//#define DebugLogTime
//define DebugBugSearch
//define DebugInterpole	  

//#define BreakHookDebugBreak

#define SPG_SAFECONV
#define SPG_CONV
#define SPG_FASTCONV
