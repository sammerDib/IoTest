
#ifdef SPG_General_PGLib
#undef SPG_General_USEButtons
#define SPG_General_HIDEButtons
#undef SPG_General_USECarac
#undef SPG_General_USEConsole
#undef SPG_General_USEFilesWindows
#undef SPG_General_USESGRAPH_OPTS
#endif

#ifndef SPG_General_PGLib
#define PGLDisplay void
#define PGLSurface void
#define PGLPrimitive void*
#endif

#ifndef SPG_General_USEButtons
#define B_Lib void*
#ifdef SPG_General_HIDEButtons
#define B_LoadButtonsLib(LB,E,ClearScreen,AutoUpdate,WorkDir,S) 0
#define B_CloseButtonsLib(LB)
#define B_UpdateButtonsLib(LB,MouseX,MouseY,MouseState)
#define B_UpdateButtonsLibFromDoEvents(LB)
#define B_RedrawButtonsLib(LB,EraseBackGround)
#define B_CreateButton(LB,E,CL,PosX,PosY,MODE,NumDigits,VToChange,Increment,ValMin,ValMax) 0
#define B_CreateClickButton(LB,E,PosX,PosY) 0
#define B_CreateCheckButton(LB,E,PosX,PosY) 0
#define B_CreateNumericButton(LB,E,CL,PosX,PosY,NumDigits,VToChange) 0
#define B_CreateVectorButton(LB,E,CL,PosX,PosY,NumDigits,V) 0
#define B_CreateNumericIntButton(LB,E,CL,PosX,PosY,NumDigits,VToChange) 0
#define B_CreateCliquableNumericButton(LB,E,CL,PosX,PosY,NumDigits,VToChange) 0
#define B_CreateCliquableNumericIntButton(LB,E,CL,PosX,PosY,NumDigits,VToChange) 0
#define B_CreateHReglage(LB,E,PosX,PosY,SizeX,VToChange,Increment,VMin,VMax) 0
#define B_CreateHReglageNumeric(LB,E,CL,PosX,PosY,SizeX,NumDigits,VToChange,Increment,VMin,VMax) 0
#define B_CreateHReglageCliquableNumeric(LB,E,CL,PosX,PosY,SizeX,NumDigits,VToChange,Increment,VMin,VMax) 0
#define B_CreateVReglage(LB,E,PosX,PosY,SizeY,VToChange,Increment,VMin,VMax) 0
#define B_CreateVReglageNumeric(LB,E,CL,PosX,PosY,SizeY,NumDigits,VToChange,Increment,VMin,VMax) 0
#define B_CreateVReglageCliquableNumeric(LB,E,CL,PosX,PosY,SizeY,NumDigits,VToChange,Increment,VMin,VMax) 0
#define B_CloseButton(B)
#define B_PrintLabel(BL,NumButton,CL,Label)
#define B_DrawButton(B)
#define B_UpdateButton(B,MouseX,MouseY,MouseState,ShouldFocus)
#define B_EditNumericButton(B)
#define B_EditNumericIntButton(B)
#endif
#endif

#ifndef SPG_General_USECarac
#define C_Lib void*
#define C_LoadCaracLib(LB,E,WorkDir,S) 0
#define C_CloseCaracLib(LB)
#define C_Print(E,PosX,PosY,Ligne,LB,Alignement)
#define C_PrintUni(E,PosX,PosY,Ligne,LB,Alignement,Couleur)
#define C_PrintWithBorder(E,PosX,PosY,Ligne,CL,Alignement,Couleur)
#endif

#ifndef SPG_General_USEGraphics
#define G_Ecran void*
#endif

#ifndef SPG_General_USEFFT
#define SPG_COMPLEX void*
#endif

#ifndef SPG_General_USEConsole
#define SPG_Console void*
#endif

#if((!defined(SPG_General_USENetwork))&&(!defined(SPG_General_USENetworkEmule)))
#define SPG_NET_ADDR void*
#endif

#ifndef SPG_General_USENetwork_Protocol
#define SPG_NET_PROTOCOL void*
#endif

#ifndef SPG_General_USEGlobal
#define MaxProgDir 1024
#define SPG_AddUpdateOnDoEvents(Fct,UsrStruct,Priority)
#define	SPG_KillUpdateOnDoEventsByAddr(A,P)
//efine	SPG_KillUpdateOnDoEventsByAddr(A)
//efine SPG_KillUpdateOnDoEventsByParam(P)
#define SPG_CALLBACK
#define SPG_UnderNetControl 0
#endif

#ifndef SPG_General_USEProgPrincipal
#define DoEvents(F)
#define SPG_WaitMouseRelease()
#define SPG_WaitMouseDown()
#define SPG_BlitAndWaitForClick()
#define SPG_SetCurDirFrom(ResultFile)
#endif

#ifndef SPG_General_USESTEREOVIS
#define SPG_SV_AdaptTextureColor(M,POCT,Pitch,SizeX,SizeY)
#endif

#ifndef SPG_General_USECDCHECK
#define IF_CD_CHECK(IMMED_v_0_31,ThisVal_CA32,Instruction)
#define IF_CD_G_CHECK(IMMED_v_0_31,Instruction)
#define CD_CHECK_EXIT(IMMED_v_0_31,IMMED_x_0_31,ThisVal_CA32)
#define CD_G_CHECK_EXIT(IMMED_v_0_31,IMMED_x_0_31)
#endif

#ifndef SPG_General_USECONFIGFILE
#define CFG_IntParam(CFG, ParamName, Variable, Comment, LoadImmediate, Type, Min, Max)
#define CFG_FloatParam(CFG, ParamName, Variable, Comment, LoadImmediate, Type, Min, Max)
#define CFG_StringParam(CFG, ParamName, Variable, Comment, LoadImmediate, Type)
#endif

#ifndef SPG_General_USEWindow
#define SPG_USERWINDOWCALLBACK void*
#endif

#ifndef SPG_General_USEWindows
#define Sleep(ms) {}
#define SleepEx(ms,a) {}
#define GetCurrentThreadId() 0
#define GetTickCount() 0
long int InterlockedExchange(long int* A, long int B) {	long int T=*A; *A=B; return T;	}
#define GetAsyncKeyState(c) 0
#endif

