#ifndef SPG_GLOBAL_H
#define SPG_GLOBAL_H


#ifdef SPG_General_USEGlobal

typedef struct
{
	int Etat;
	int Size;

#ifndef SPG_General_PGLib
	int WPosX;
	int WPosY;
	int WSzeX;
	int WSzeY;
	int TotalPosX;
	int TotalPosY;
	int TotalSzeX;
	int TotalSzeY;
	SPG_MouseState Mouse;
#endif

#ifdef DebugList
	int TotalMsg;
#endif

#ifdef DebugMem
	intptr_t StackInitPos;
#endif

#ifdef SPG_General_USETimer
	S_TimerCountType CPUClock;
#ifdef DebugProgPrincipalTimer
	S_TIMER T_Total;
	S_TIMER	T_DoevBlitEcran;
	S_TIMER	T_DoevLock;
#ifdef DebugRenderTimer
	S_TIMER T_SGE_TransformAndRender;
	S_TIMER T_SGE_CalcPointsCoord;
	S_TIMER T_SGE_RefreshTriBuff;
	S_TIMER T_SGE_FinishRender;
	S_TIMER T_SGE_CalcLight;
	S_TIMER T_SGE_G_ClearSky;
	S_TIMER T_SGE_G_Draw;
#endif
#ifdef DebugGraphicsTimer
	S_TIMER T_GraphicsRender;
	S_TIMER T_InlinePrepareSegment;
	S_TIMER T_InlineRenderSegment;
#endif
#ifdef DebugProfilManagerTimer
	S_TIMER T_P_Draw;
	S_TIMER T_P8_Draw;
#endif
#ifdef DebugNetworkTimer
	S_TIMER T_NET_Snd;
	S_TIMER T_NET_Rcv;
#endif
	int TimerUsed;
#endif
#endif

#ifdef SPG_General_USEWLTG_GLOBAL
	WLOGTIME wt;
#endif

	int LT_DOEVENTS;
	int LT_LISTMSGBOX;
	int LT_CLBK;
	int LT_BLIT;
	int LT_PEEKMSG;
	//int LT_GETMSG;
	//int LT_DISPMSG;
	int LT_SLEEP;
	int LT_WNDPROC;
	int LT_NETSEND;
	int LT_NETREAD;
	int LT_RENDER;
	int LT_MEM;
	//int LT_MEMFREE;
	int LT_OSAbnormalTermination;
	int LT_AttemptToChangeOSAbnormalTermination;
	int LT_EditNumericButton;

	DWORD LT_DefaultOSAbnormalTerminationHandler; //callback de fonction


#ifdef SPG_General_USEGLOBALTHREADNAME
	THREAD_DBG_INFOS ThreadDbgInfos;
#endif

#ifdef SPG_General_USEFiles
	char ProgDir[MaxProgDir];
	char ProgName[MaxProgDir];
	char LogDir[MaxProgDir];
	//char ParamsDir[MaxProgDir];
#endif

#ifdef SPG_General_USEFilesWindows
	char CurDir[MaxProgDir];
#endif

	int DoEventsRecursion;
	int NumCallBack;
	Global_CALLBACK CallBack[MaxCallBack];//callback de doevents

//fdef DebugList
	Check_CALLBACK CHECK_Callback[MAXCHECKCALLBACK];//callback de SPG_List
	int SPG_List_Recurse;
	int EnableList; //affichage de messagebox sur erreur de macro 1:always 0:only if no callback defined -1:never
//ndif

#ifdef SPG_General_PGLib
	PGLCommon *common;
	PGLDisplay *display;
#endif

#ifdef SPG_General_USEGraphics
#ifdef SPG_General_USEGEFFECT
	int NumBlur;
	G_Ecran EcranSystem;
	G_Ecran Ecran;
	G_MotionBlur MB;
#else
	G_Ecran Ecran;
#endif
#endif

#ifdef SPG_General_USEAVI
	int AVIFPS;//utilise à l'initialisation de la video (write-only)
	SPG_AVI_STREAM AVISG;//if(AVISG.Etat) AVISG.FPS AVISG.DT (read-only)
#endif

	bool FullScreen;
	bool LockWindow;
	bool Moveable;
	//void* Console;
	int hInstance;
	void* hWndWin;
	int HDCWND;
	void* FullScreenPalette;
	void* OldPalette;
	void* Palette;
	char WClassName[MaxClassName];
	SPG_WINDOWCALLBACK WCB;

#ifdef SPG_General_USENetwork_Protocol
	SPG_NET_PROTOCOL* SNP;
	SPG_NET_ADDR ErrorReport;
	SPG_NET_ADDR ControlSource;
	//SPG_NET_ADDR ScreenView[4];
	S_TIMER NetControlTimer;
	MELINK_SCREEN_SEND_STATE ScreenSendState;
	ULIPS_List ULIST;
#endif

#ifdef DebugMem
	SPG_MEM_STATE MS;
	int StackCheckSet;
	int StackCheckTest;
#endif

#ifdef SPG_General_USECONNEXION
	SCM_INTERFACES SCI;
#endif

#ifdef SPG_General_USECDCHECK
	CRYPTED_ARRAY_32 CD_UID;
#endif

} GlobalType;

extern GlobalType Global;

#endif

#endif 