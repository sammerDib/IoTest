
#ifndef DebugList
//dbgCHECK sert pour un ds de debogage
//ne pas utiliser dans une expression complexe,
//encadrer au maximum par ifdef debuglist
#define DbgCHECK(Cond,Msg)
#define DbgCHECKTWO(Cond,Msg,String)
#define DbgCHECKTHREE(Cond,Msg,String1,String2)
#define DbgCHECKV(Cond,Msg,Val)
//test avec affichage d'un message, la sequence d'instructions
//sera toujours executee mais l'affichage du message est controle par debuglist
#define CHECK(Cond,Msg,Ret) if (Cond) {Ret;}
#define CHECKW(Warning,Cond,Msg,Ret) if (Cond) {Ret;}
#define CHECK_ELSE(Cond,Msg,Ret) if (Cond) {Ret;}
//permet d'afficher en plus un nombre
#define CHECKV(Cond,Msg,Val,Ret) if (Cond) {Ret;}
//permet d'afficher en plus une chaine
#define CHECKTWO(Cond,Msg,String,Ret) if (Cond) {Ret;}
#define CHECKWTWO(Warning,Cond,Msg,String,Ret) if (Cond) {Ret;}
#define CHECKTHREE(Cond,Msg,String1,String2,Ret) if (Cond) {Ret;}
#define CHECKWTHREE(Warning,Cond,Msg,String1,String2,Ret) if (Cond) {Ret;}
#define CHECKTWO_ELSE(Cond,Msg,String,Ret) if (Cond) {Ret;}
#define CHECKWTWO_ELSE(Warning,Cond,Msg,String,Ret) if (Cond) {Ret;}
#define CHECKTHREE_ELSE(Cond,Msg,String1,String2,Ret) if (Cond) {Ret;}
#define CHECKWTHREE_ELSE(Warning,Cond,Msg,String1,String2,Ret) if (Cond) {Ret;}
//teste pour un resultat infini
#define CHECKFLOAT(FloatRes,Msg)
//commente les appels de fonction
#define SPG_VerboseCall(FCT) FCT

#define SPG_List_ResetMsg()

#define DebugOutputSizeof(P)

//ragma SPGMSG(__FILE__,__LINE__,"notification messages disabled")

#else

//#pragma SPGMSG(__FILE__,__LINE__,"Error messages enabled")

void SPG_CONV SPG_List_CountMsg();
int SPG_CONV SPG_GetLastWinError(char* Msg, int N);
void SPG_CONV SPG_List(const char* Txt);
void SPG_CONV SPG_List1(const char* Txt,void* Val);
void SPG_CONV SPG_List2S(const char* Txt, const char* Txt1);
void SPG_CONV SPG_List3S(const char* Txt,const char* Txt1, const char* Txt2);
void SPG_CONV SPG_ListSSN(const char* Txt,const char* Txt1, int Val);
void SPG_CONV SPG_List2(const char* Txt,int Val, const char* Txt1,int Val1);
void SPG_CONV SPG_ListSNSS(const char* Txt,int Val,const char* Txt1, const char* Txt2);
void SPG_CONV SPG_ListSNSSS(const char* Txt,int Val,const char* Txt1,const char* Txt2,const char* Txt3);
void SPG_CONV SPG_List3(const char* Txt,int Val,const char* Txt1,int Val1,const char* Txt2,int Val2);

#define SPG_ShouldFail(Txt1,Txt2,RV) {char Msg[512];sprintf(Msg, Txt1 "\n%s\nFaire ECHOUER l'opération ?",Txt2);if (MessageBox(0,Msg,"Debogage",MB_TOPMOST|MB_YESNO)==IDYES) return RV;}
//affichages des messages

//resete l'affichage des messages s'il y a eu trop d'erreurs
#ifdef SPG_General_USEGlobal
#define SPG_List_ResetMsg() Global.TotalMsg=0
#else
#define SPG_List_ResetMsg()
#endif

#define DbgCHECK(Cond,Msg)							if (Cond) SPG_List(CHKSTR(Cond,__FILE__,__FUNCTION__,__LINE__,Msg));

#define DbgCHECKV(Cond,Msg,Val)					if (Cond) SPG_List1(	CHKSTRS(Cond,__FILE__,__FUNCTION__,__LINE__,Msg),Val);
#define DbgCHECKTWO(Cond,Msg,String)		if (Cond) SPG_List2S(	CHKSTRS(Cond,__FILE__,__FUNCTION__,__LINE__,Msg),String);
#define DbgCHECKTHREE(Cond,Msg,str1,str2)  if (Cond) SPG_List3S(	CHKSTRS(Cond,__FILE__,__FUNCTION__,__LINE__,Msg),str1,str2);

#define CHECK(Cond,Msg,Ret)							if (Cond) {	SPG_List(		CHKSTR(Cond,__FILE__,__FUNCTION__,__LINE__,Msg));	Ret;	}
#define CHECKTWO(Cond,Msg,str,Ret)				if (Cond) {	SPG_List2S(	CHKSTRS(Cond,__FILE__,__FUNCTION__,__LINE__,Msg),str);	Ret;	}
#define CHECKTHREE(Cond,Msg,str1,str2,Ret)	if (Cond) {	SPG_List3S(	CHKSTRS(Cond,__FILE__,__FUNCTION__,__LINE__,Msg),str1,str2);	Ret;	}

//permet une construction du type CHECK_ELSE(SPG_IsValidPointeur(P)==0,"ERREUR",return) else memset(P,...
#define CHECKV(Cond,Msg,Val,Ret)					if (Cond) {	SPG_List1(	CHKSTRS(Cond,__FILE__,__FUNCTION__,__LINE__,Msg),Val);	Ret;	}
#define CHECKV_ELSE(Cond,Msg,Val,Ret)			if (Cond) {	SPG_List1(	CHKSTRS(Cond,__FILE__,__FUNCTION__,__LINE__,Msg),Val);	Ret;	}

#define CHECKW(Warning,Cond,Msg,Ret)					if (Cond) {if(Warning){	SPG_List(		CHKSTR(Cond,__FILE__,__FUNCTION__,__LINE__,Msg));	};	Ret;	}
#define CHECKWTWO(Warning,Cond,Msg,str,Ret)	if (Cond) {if(Warning){	SPG_List2S(		CHKSTRS(Cond,__FILE__,__FUNCTION__,__LINE__,Msg),str);	};	Ret;	}
#define CHECKWTHREE(Warning,Cond,Msg,str1,str2,Ret)	if (Cond) {if(Warning){	SPG_List3S(	CHKSTRS(Cond,__FILE__,__FUNCTION__,__LINE__,Msg),str1,str2);	};	Ret;	}

#define CHECK_ELSE(Cond,Msg,Ret)							if (Cond) {	SPG_List(		CHKSTR(Cond,__FILE__,__FUNCTION__,__LINE__,Msg)	);	Ret;	}
#define CHECKTWO_ELSE(Cond,Msg,String,Ret)		if (Cond) {	SPG_List2S(	CHKSTRS(Cond,__FILE__,__FUNCTION__,__LINE__,Msg),String);	Ret;	}
#define CHECKTHREE_ELSE(Cond,Msg,str1,str2,Ret)	if (Cond) {	SPG_List3S(	CHKSTRS(Cond,__FILE__,__FUNCTION__,__LINE__,Msg),str1,str2);	Ret;	}

#define DebugOutputSizeof(P) {char strSizeof_##P[256];sprintf(strSizeof_##P,"DebugOutputSizeof : %s(%i)@%s sizeof(%s)=%i\r\n",__FILE__,__LINE__,__FUNCTION__,#P,sizeof(P));OutputDebugString(strSizeof_##P);}

// -------------------------------

#ifdef DebugFloat
#define CHECKFLOAT(FloatRes,Msg) DbgCHECK(_finite(FloatRes)==0,Msg"\nResultat infini\n"#FloatRes)
#else
#define CHECKFLOAT(FloatRes,Msg)
#endif

#ifdef DebugBugSearch
#define SPG_VerboseCall(FCT) {SPG_List(__FILE__"\nAppel à "#FCT);FCT;SPG_List(__FILE__"\nRetour de "#FCT);}
#else
#define SPG_VerboseCall(FCT) FCT
#endif
#endif

#define CHK_ExpandAndConvertToString(x) CHK_ConvertToString(x)
#define CHK_ConvertToString(x) #x
#define CHKSTR(c,f,fc,line,m) "Notification\n" m "\n" #c "\n" f "(" CHK_ExpandAndConvertToString(line) "):" fc //avec commentaire en t^te
#define CHKSTRS(c,f,fc,line,m) "Notification\n" #c "\n" f "(" CHK_ExpandAndConvertToString(line) "):" fc "\n" m //avec commentaire en fin pour concatenation
#define XCPTSTR(f,fc,line,m) m "\n" f "(" CHK_ExpandAndConvertToString(line) "):" fc
#define CHKNOP {__asm nop __asm nop __asm nop __asm nop}

#ifdef DebugFloatHard
#define HardCHECKFLOAT(FloatRes,Msg) CHECKFLOAT(FloatRes,Msg)
#else
#define HardCHECKFLOAT(FloatRes,Msg)
#endif

#define SPG_CHECK CHECK
#define SPG_CHECKTWO CHECKTWO
#define SPG_CHECKTHREE CHECKTHREE

#define SPG_CHECKW CHECKW
#define SPG_CHECKWTWO CHECKWTWO
#define SPG_CHECKWTHREE CHECKWTHREE

#define SPG_CHECKV CHECKV
#define SPG_CHECK_ELSE CHECK_ELSE
#define SPG_CHECKTWO_ELSE CHECKTWO_ELSE
#define SPG_CHECKTHREE_ELSE CHECKTHREE_ELSE
#define SPG_CHECKFLOAT CHECKFLOAT

