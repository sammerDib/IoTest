/*
 * $Id: FogaleProbeInternal.h 9496 2009-06-30 13:37:11Z m-abet $
 */

//doit etre maintenu en correspondance avec le nom du module compilé
//dans les settings de compilation du projet
#define FP_DLL_NAME "FogaleProbe.dll"

#define ParamsFolder "Params\\"

#define FPMAXSTR 512
#define MAX_NB_PROBES 8

typedef enum
{
	fpLiseED,
	fpLiseEDDouble,
	fpLiseEDExtended,
	fpChrom,
	fpChromDouble,
	fpCCS_PRIMA,
	fpSTIL_DUO,
	fpSPIRO,
	fpLiseLS,
	fpLenScan

} FPROBETYPE;

typedef struct
{
	bool bDefined;
	char sName[FPMAXSTR];	// Nom de l'échantillon
	char sSampleNumber[FPMAXSTR];	// Numéro de lot de l'échantillon
	int iNbThickness;
	double fTheoThickness[MAXMATCHPEAKS];	// Epaisseurs attendues
	double fTolerance[MAXMATCHPEAKS];	// Tolérance sur l'épaisseur attendue
	double fIndex[MAXMATCHPEAKS];	// Indice optique des épaisseurs
	double iType[MAXMATCHPEAKS];	// Intrensité des pics attendus
} SAMPLE;

typedef enum
{
	fpClosed,
	fpStarted,
	fpStopped,
	//fpSettings,
	fpAnyState	//utilisé pour les comparaisons d'état uniquement, ne pas stocker
} FPSTATE;

typedef struct
{
	FPROBETYPE Type;
	char AbsParamsFile[FPMAXSTR];
	union
	{
		int FProbeState;
		LISE_ED LiseEd;
		DBL_LISE_ED DblLiseEd;
		//LISE_LS_LI LiseLsLi;
#ifdef FDE
		CHR Chrom;
		CHRDBL ChromDouble;
#ifdef SPG_General_USESTIL
		STIL_PROBE StilProbe;
#endif
		SPIRO_PROBE SpiroProbe;
#endif
		//LENSCAN LS;
	};

	//Ajouter ultérieurement les pointeurs des fonctions spécifiques
	FPROBE_INIT FProbeInit;
	FPROBE_LAMP_STATE FPProbeLampState;
	FPROBE_CLOSE FProbeClose;
	FPROBE_START FProbeStart;
	FPROBE_STOP FProbeStop;
	FPROBE_DEFINE_SAMPLE FProbeDefineSample;
	FPROBE_GET_THICKNESS FProbeGetThickness;
	FPROBE_GET_THICKNESSES FProbeGetThicknesses;

	FPROBE_START_SINGLESHOT FProbeStartSingleShot;
	FPROBE_START_CONTINUOUS FProbeStartContinuous;

	FPROBE_GETPARAM FProbeGetParam;
	FPROBE_SETPARAM FProbeSetParam;

	FPROBE_OPEN_PARAMS_WINDOW FPOpenSettingsWindow;
	FPROBE_UPDATE_PARAMS_WINDOW FPUpdateSettingsWindow;
	FPROBE_CLOSE_PARAMS_WINDOW FPCloseSettingsWindow;

	FPROBE_GET_SYSTEM_CAPS FPGetSystemCaps;
	FPROBE_SET_STAGE_POSITION_INFO FProbeSetStagePositionInfo;
	FPROBE_GET_RAW_SIGNAL FPGetRawSignal;
	FPROBE_CALIBRATE_DARK FPCalibrateDark;
	FPROBE_CALIBRATE_THICKNESS FPCalibrateThickness;

	FPROBE_DEFINE_SAMPLE_DOUBLE FPDefineSampleDouble;
	FPROBE_GET_SYSTEM_CAPS_DOUBLE FPGetSystemCapsDouble;

	FPSTATE state;
	bool setting;
	
	char sSerialNumber[2][FPMAXSTR];	// tableau de numéro de série à deux dimensions dans le cas du Chrom double

	//HANDLE Mutex;	//< Ajout d'une variable Mutex pour tester la réentrance dans les fonctions !

	bool bReentranceFogaleProbe;	//< Booléen pour ne pas autoriser la réentrance dans FogaleProbe
} FPROBE_STATE;


typedef struct
{
	int DLL_State;
	char AbsModulePath[FPMAXSTR];
	char AbsLogPath[FPMAXSTR];

	int NumProbe;
	int MaxProbe;
	FPROBE_STATE Probe[MAX_NB_PROBES];

	LOGFILE Log;
	/*
	int EnableMsgBox;
	int MsgCount;
	*/

	char sSerialNumberTable[MAX_NB_PROBES][FPMAXSTR];
	int iIndexOnSNTable;
	int iSNDispo;

	char ProductName[FPMAXSTR];	//< Nom du produit fogale
	char VersionDll[FPMAXSTR];	//< Version de la dll, stand alone ou OEM

} FPDLL_STATE;

#define Flag_Log 1
#define Flag_CheckID 2
#define Flag_CheckState 4

//efine DisableReentranceTest

#ifdef DisableReentranceTest

#define FP_ENTER(requieredProbeState)
#define FP_ENTERnolock(requieredProbeState)
#define FP_ENTERnoID()

#define FP_RETURN(r) return r
#define FP_RETURNnolock(r) return r
#define FP_RETURNnoID(r) return r

#else

int FP_Enter(int ProbeID, char* fct, char* crequieredState, FPSTATE requieredState, int Flag);
int FP_Enternolock(int ProbeID, char* fct, char* crequieredState, FPSTATE requieredState, int Flag);

#define FP_ENTER(requieredProbeState) {int r=FP_Enter(ProbeID,__FUNCTION__,#requieredProbeState,requieredProbeState,Flag_Log|Flag_CheckID|Flag_CheckState); if(r!=FP_OK) return r;}
#define FP_ENTERnolock(requieredProbeState) {int r=FP_Enternolock(ProbeID,__FUNCTION__,#requieredProbeState,requieredProbeState,Flag_Log|Flag_CheckID|Flag_CheckState); if(r!=FP_OK) return r;}
#define FP_ENTERnoID() {int r=FP_Enter(0,__FUNCTION__,"",(FPSTATE)0,Flag_Log); if(r!=FP_OK) return r;}

int FP_Return(int ProbeID, char* cReturnValue, int iReturnValue, char* fct, int Line, int Flag);
int FP_Returnnolock(int ProbeID, char* cReturnValue, int iReturnValue, char* fct, int Line, int Flag);

#define FP_RETURN(r) return FP_Return(ProbeID,#r,r,__FUNCTION__,__LINE__,Flag_Log|Flag_CheckID|Flag_CheckState)
#define FP_RETURNnolock(r) return FP_Returnnolock(ProbeID,#r,r,__FUNCTION__,__LINE__,Flag_Log|Flag_CheckID|Flag_CheckState)
#define FP_RETURNnoID(r) return FP_Return(0,#r,r,__FUNCTION__,__LINE__,Flag_Log)

#endif

#define PROBE_CALL(r,fct) if(s.Probe[ProbeID].fct) r=s.Probe[ProbeID].fct(&s.Probe[ProbeID].FProbeState); else r=FP_UNAVAILABLE;
