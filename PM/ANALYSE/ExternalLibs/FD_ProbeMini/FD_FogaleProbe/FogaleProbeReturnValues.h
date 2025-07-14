/*
 * $Id: FogaleProbeReturnValues.h 8255 2009-02-16 17:50:50Z S-PETITGRAND $
 */


// Return values (all functions)
// The function has succeeded
#define FP_OK 1
// The function has failed
#define FP_FAIL 0
// The DLL has not been initialized
#define FP_NOINIT -1
// The ProbeID entered is not valid
#define FP_INVALIDPROBE -2
// Precondition failed
#define FP_INVALIDPARAM -3
// The function or parameter ID is not available for the current ProbeID
#define FP_UNAVAILABLE -4
// The DLL is busy in another function
#define FP_BUSY -5
// pointer null
#define INVALID_POINTER	-6
// invalid size buffer
#define INVALID_SIZE_BUFFER	-7

// error associated to simple Lise ed
#define FP_TASK_CREATION_FAILED			(ERROR_LISE_ED + -1)
#define FP_TASK_START_FAILED			(ERROR_LISE_ED + -2)
#define FP_CLOSE_NI_TASK_FAIL			(ERROR_LISE_ED + -3)
#define FP_DESTROY_NI_TASK_FAIL			(ERROR_LISE_ED + -4)
#define	FP_NOT_ENOUGHT_MEAS_TO_AVERAGE	(ERROR_LISE_ED + -5)
#define FP_NOT_ALL_VALUE_FOUND			(ERROR_LISE_ED + -6)
#define FP_GOOD_VALUE_DIFF_OF_AVERAGE	(ERROR_LISE_ED + -7)
#define FP_NO_VALUE_FOUND				(ERROR_LISE_ED + -8)

// error associated to double Lise ed
#define FP_FIRST_ED_INIT_FAILED		(ERROR_DBL_LISE_ED + -1)
#define FP_SECOND_ED_INIT_FAILED	(ERROR_DBL_LISE_ED + -2)
#define FP_DEFINESPL1_FAILED		(ERROR_DBL_LISE_ED + -3)
#define FP_DEFINESPL2_FAILED		(ERROR_DBL_LISE_ED + -4)
#define FP_DEF_MODE_INCORRECT		(ERROR_DBL_LISE_ED + -5)
#define FP_CFG_MEA_ERROR			(ERROR_DBL_LISE_ED + -6)
#define FP_ERROR_PARAM_ID			(ERROR_DBL_LISE_ED + -7)
#define FP_ERROR_CALCULATETH		(ERROR_DBL_LISE_ED + -8)
#define FP_SELECTCHANNEL_FAILED		(ERROR_DBL_LISE_ED + -9)
#define FP_SET_MASTER_DEV_FAIL		(ERROR_DBL_LISE_ED + -10)
#define FP_GETH1_FAILED				(ERROR_DBL_LISE_ED + -11)
#define FP_GETH2_FAILED				(ERROR_DBL_LISE_ED + -12)
#define FP_WRONG_QUALITY			(ERROR_DBL_LISE_ED + -13)
#define FP_CALIB_TH_UP_DOWN_DIFF	(ERROR_DBL_LISE_ED + -14)	
#define FP_FAILED_START_PROBE		(ERROR_DBL_LISE_ED + -15)
#define FP_CALIB_TOTALTH_NEGATIVE	(ERROR_DBL_LISE_ED + -16)	// calibration: Epaisseur totale mesuree inferieure a 0
#define FP_CALIB_UNDER_TOLERANCE	(ERROR_DBL_LISE_ED + -17)	// calibration: Toutes les mesures effectuées sont hors tolerances
#define FP_CALIB_GET_THICK_FAILED	(ERROR_DBL_LISE_ED + -18)	// calibration: On a effecue n GetThickness invalides
#define FP_CALIB_WRONG_PROBE_NUM	(ERROR_DBL_LISE_ED + -19)
#define FP_CALIB_FAILED_DEFINE_SAMPLE	(ERROR_DBL_LISE_ED + -20)
#define FP_CALIB_CANCELED			(ERROR_DBL_LISE_ED + -21)	// Retour d'erreur au cas ou las calibration a été annulé

#define SPIRO_FRAME_NON_COMPLETE	(ERROR_SPIRO + -1)

// error identifiant for each probe
#define ERROR_DBL_LISE_ED	-1000
#define ERROR_LISE_ED		-2000
#define ERROR_CHROM			-3000
#define ERROR_DBL_CHROM		-4000
#define ERROR_LENSCAN		-5000
#define ERROR_SPIRO			-6000