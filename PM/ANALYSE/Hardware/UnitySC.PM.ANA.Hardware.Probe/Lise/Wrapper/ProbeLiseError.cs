using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.ANA.Hardware.Probe.Lise
{
    public enum ProbeLiseError : int
    {
        [Description("The function has succeeded")]
        FP_OK = 1,
        [Description("The function has failed")]
        FP_FAIL = 0,
        /// <summary> The DLL has not been initialized </summary>
        [Description("The DLL has not been initialized")]
        FP_NOINIT = -1,
        /// <summary> The ProbeID entered is not valid </summary>
        FP_INVALIDPROBE = -2,
        /// <summary> Precondition failed </summary>
        FP_INVALIDPARAM = -3,
        [Description("The function or parameter ID is not available for the current ProbeID")]
        FP_UNAVAILABLE = -4,
        /// <summary> The DLL is busy in another function </summary>
        [Description("The DLL is busy in another function")]
        FP_BUSY = -5,
        /// <summary> pointer null </summary>
        INVALID_POINTER = -6,
        /// <summary> invalid size buffer </summary>
        INVALID_SIZE_BUFFER = -7,

        // error associated to simple Lise ed,
        FP_TASK_CREATION_FAILED = (ERROR_LISE_ED + -1),
        FP_TASK_START_FAILED = (ERROR_LISE_ED + -2),
        FP_CLOSE_NI_TASK_FAIL = (ERROR_LISE_ED + -3),
        FP_DESTROY_NI_TASK_FAIL = (ERROR_LISE_ED + -4),
        FP_NOT_ENOUGHT_MEAS_TO_AVERAGE = (ERROR_LISE_ED + -5),
        FP_NOT_ALL_VALUE_FOUND = (ERROR_LISE_ED + -6),
        FP_GOOD_VALUE_DIFF_OF_AVERAGE = (ERROR_LISE_ED + -7),
        FP_NO_VALUE_FOUND = (ERROR_LISE_ED + -8),

        // error associated to double Lise ed,
        FP_FIRST_ED_INIT_FAILED = (ERROR_DBL_LISE_ED + -1),
        FP_SECOND_ED_INIT_FAILED = (ERROR_DBL_LISE_ED + -2),
        FP_DEFINESPL1_FAILED = (ERROR_DBL_LISE_ED + -3),
        FP_DEFINESPL2_FAILED = (ERROR_DBL_LISE_ED + -4),
        FP_DEF_MODE_INCORRECT = (ERROR_DBL_LISE_ED + -5),
        FP_CFG_MEA_ERROR = (ERROR_DBL_LISE_ED + -6),
        FP_ERROR_PARAM_ID = (ERROR_DBL_LISE_ED + -7),
        FP_ERROR_CALCULATETH = (ERROR_DBL_LISE_ED + -8),
        FP_SELECTCHANNEL_FAILED = (ERROR_DBL_LISE_ED + -9),
        FP_SET_MASTER_DEV_FAIL = (ERROR_DBL_LISE_ED + -10),
        FP_GETH1_FAILED = (ERROR_DBL_LISE_ED + -11),
        FP_GETH2_FAILED = (ERROR_DBL_LISE_ED + -12),
        FP_WRONG_QUALITY = (ERROR_DBL_LISE_ED + -13),
        FP_CALIB_TH_UP_DOWN_DIFF = (ERROR_DBL_LISE_ED + -14),
        FP_FAILED_START_PROBE = (ERROR_DBL_LISE_ED + -15),
        /// <summary> calibration: Epaisseur totale mesuree inferieure a 0 </summary>
        FP_CALIB_TOTALTH_NEGATIVE = (ERROR_DBL_LISE_ED + -16),
        /// <summary> calibration: Toutes les mesures effectuées sont hors tolerances </summary>
        FP_CALIB_UNDER_TOLERANCE = (ERROR_DBL_LISE_ED + -17),
        /// <summary> calibration: On a effecue n GetThickness invalides </summary>
        FP_CALIB_GET_THICK_FAILED = (ERROR_DBL_LISE_ED + -18),	
        FP_CALIB_WRONG_PROBE_NUM = (ERROR_DBL_LISE_ED + -19),
        FP_CALIB_FAILED_DEFINE_SAMPLE = (ERROR_DBL_LISE_ED + -20),
        /// <summary> Retour d'erreur au cas ou las calibration a été annulé </summary>
        FP_CALIB_CANCELED = (ERROR_DBL_LISE_ED + -21),	

        SPIRO_FRAME_NON_COMPLETE = (ERROR_SPIRO + -1),

        // error identifiant for each probe,
        ERROR_DBL_LISE_ED = -1000,
        ERROR_LISE_ED = -2000,
        ERROR_CHROM = -3000,
        ERROR_DBL_CHROM = -4000,
        ERROR_LENSCAN = -5000,
        ERROR_SPIRO = -6000
    }
}
