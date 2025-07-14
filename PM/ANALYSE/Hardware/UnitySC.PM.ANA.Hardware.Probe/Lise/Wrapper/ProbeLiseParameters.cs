using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.ANA.Hardware.Probe.Lise
{
    /// <summary>
    /// Définition des constantes pour FPGetParam/FPSetParam
    /// </summary>
    public enum ProbeLiseParams : int
    {
        //identifiants
        FPID_COMMON = 0,
        FPID_MATERIALS = 10000,
        FPID_LENSCAN = 21100, //(FPID_COMMON+100+2*FPID_MATERIALS),
        FPID_DBL_LISE_ED = 60000, //(FPID_COMMON+100+2*FPID_MATERIALS),
        FPID_EXT_LISE_ED = 70000,
        FPID_SPIRO = 80000,
        FPID_RAWCHANNELS = 6,
        FPID_STRLEN = 256,

        //#### COMMON PARAMS ####,
        FPID_C_TYPE = (FPID_COMMON + 1),    //probe type,
        FPID_C_VERSION = (FPID_COMMON + 2), //probe module version,
        FPID_I_STATUS = (FPID_COMMON + 3),  //probe status,
        FPID_C_SERIAL = (FPID_COMMON + 4),  //probe serial number (string),
        FPID_D_WORKINGDISTANCE = (FPID_COMMON + 5), //probe working distance,
        FPID_D_RANGE = (FPID_COMMON + 6),   //probe range,
        FPID_I_TOTAL_PROBE_USED = (FPID_COMMON + 7),    //num max de probe,
        FPID_C_PN = (FPID_COMMON + 8),  //Part Number

        FPID_D_CURSOURCEPOWER = (FPID_COMMON + 10),  //probe source power,
        FPID_D_MINSOURCEPOWER = (FPID_COMMON + 11),
        FPID_D_MAXSOURCEPOWER = (FPID_COMMON + 12),
        FPID_D_STPSOURCEPOWER = (FPID_COMMON + 13),

        FPID_I_STAGEX = (FPID_COMMON + 14),
        FPID_I_STAGEY = (FPID_COMMON + 15),
        FPID_I_STAGEZ = (FPID_COMMON + 16),
        FPID_B_AUTOGAIN = (FPID_COMMON + 17),
        FPID_B_AUTOAIRGAPCONFIG = (FPID_COMMON + 18),
        FPID_I_AVERAGEPARAM = (FPID_COMMON + 19),

        FPID_C_LOG = (FPID_COMMON + 20), //log (string) max length 4096 (MaxConsole) Used to retrive the log/error messages and flush the corresponding internal buffer

        FPID_I_NUMCHANNEL = (FPID_COMMON + 21), //return the channel count in the raw signal (if more than 1 channel data are interleaved),
        FPID_C_SAVESIGNAL = (FPID_COMMON + 22),
        FPID_D_AMPLITUDEPEAKS = (FPID_COMMON + 23),

        FPID_B_LIMITEDTIME_MODE = (FPID_COMMON + 24),   // Mode de temps limité pour le getThickness

        FPID_D_SINGLESHOTDURATION = (FPID_COMMON + 25), // Durée d'une mesure en single shot,
        FPID_B_TRIGGERED_MODE = (FPID_COMMON + 26), // Configurer le Get Thicknesses en mode triggered  ,
        FPID_D_COMPARISON_TOLERANCE = (FPID_COMMON + 27),   // Permet de modifier la tolerance entre les chemins aller/retour,
        FPID_D_MAXIMUM_GAIN = (FPID_COMMON + 28),   // Permet de modifier le gain Maximum de la sonde,
        FPID_D_WAVE_LENGTH = (FPID_COMMON + 29),    // Permet de modifier la longeur d'onde du faisceau de la sonde,
        FPID_D_SATURATION_THRESHOLD = (FPID_COMMON + 30),   // Permet de modifier le seuil de saturation de la sonde,
        FPID_C_NI_DEVICE = (FPID_COMMON + 31),  // Permet de modifier le NI device de la sonde,
        FPID_S_SAVE_CURRENT_PARAMS = (FPID_COMMON + 32),// Permet de lancer une sauvegarde des parametres courants avec le bon password,
        FPID_B_GET_PROBE_MEASURING_STATE = (FPID_COMMON + 33), // Permet de connaitre l'etat de la mesure en continuous,
        FPID_D_PROBE_FREQUENCY = (FPID_COMMON + 34),// Get de la frequence de la probe,
        FPID_D_AIRGAP_THRESHOLD = (FPID_COMMON + 35),// définition du seuil sur l'air gap,
        FPID_D_AIRGAP_UPPER_THRESHOLD = (FPID_COMMON + 36), // définition du seuil sur l'air gap,
        FPID_D_AIRGAP_LOWER_THRESHOLD = (FPID_COMMON + 37), // définition du seuil sur l'air gap,
        FPID_D_AIRGAP_TH_SWITCH_MODE = (FPID_COMMON + 38), // définition de l'airgap de switch,
        FPID_D_BASE_LINE = (FPID_COMMON + 39),  // permet de récupérer la valeur de la ligne de base,
        FPID_D_UPPER_BASE_LINE = (FPID_COMMON + 40),    // permet de récupérer la valeur de la ligne de base de la sonde du dessus en double,
        FPID_D_LOWER_BASE_LINE = (FPID_COMMON + 41),    // permet de récupérer la valeur de la ligne de base de la sonde du dessous en double
                                                        // Probe standby,
        FPID_B_STANDBY = (FPID_COMMON + 42),    // Passe la probe en veille (si possible) et sort de veille,
        FPID_D_STANDBY_VALUE = (FPID_COMMON + 43),  // Recupère la valeur de standby,
        FPID_D_FULLINTENSITY_VALUE = (FPID_COMMON + 44),    // Récupère la valeur à maximum dans le fichier de parametres

        FPID_I_SETCURRENTPROBE_ONLY = (FPID_COMMON + 50),
        FPID_I_SETVISIBLEPROBE = (FPID_COMMON + 51),

        FPID_I_GETNUMMATERIALS = (FPID_COMMON + 99), //total count of material in database (FP_UNAVAILABLE if probe does not have database)

        FPID_C_GETMATERIALNAME = (FPID_COMMON + 100), //char** Name[i]; FP_GetParam(Probe,Name[i],FPID_C_GETMATERIALNAME+i),
        FPID_C_GETMATERIALNAMEFIRST = (FPID_COMMON + 100), //char** Name[i]; FP_GetParam(Probe,Name[i],FPID_C_GETMATERIALNAME+i),
        FPID_C_GETMATERIALNAMELAST = (FPID_COMMON + 100 + FPID_MATERIALS - 1), //char** Name[i]; FP_GetParam(Probe,Name[i],FPID_C_GETMATERIALNAME+i)

        FPID_D_GETMATERIALINDEX = (FPID_COMMON + 100 + FPID_MATERIALS), // //double* Index[i]; FP_GetParam(Probe,Index+i,FPID_C_GETMATERIALINDEX+i),
        FPID_D_GETMATERIALINDEXFIRST = (FPID_COMMON + 100 + FPID_MATERIALS),
        FPID_D_GETMATERIALINDEXLAST = (FPID_COMMON + 100 + FPID_MATERIALS + FPID_MATERIALS - 1),

        //#### DOUBLE LISE ED PARAMS ######,
        FPID_I_SETCURRENTPROBE = (FPID_DBL_LISE_ED + 1),
        FPID_D_CALIBRATE_TOTAL_TH = (FPID_DBL_LISE_ED + 2),
        FPID_I_NUM_OF_PROBE = (FPID_DBL_LISE_ED + 3),
        FPID_I_GETCURRENTPROBE = (FPID_DBL_LISE_ED + 4),
        FPID_D_ZVALUE_TOTAL_TH = (FPID_DBL_LISE_ED + 5),
        FPID_D_XVALUE_TOTAL_TH = (FPID_DBL_LISE_ED + 6),
        FPID_D_YVALUE_TOTAL_TH = (FPID_DBL_LISE_ED + 7),
        FPID_D_CALIBRATE_LOWER_AG = (FPID_DBL_LISE_ED + 8),
        FPID_D_CALIBRATE_UPPER_AG = (FPID_DBL_LISE_ED + 9),
        FPID_D_CALIBRATE_TH_USED = (FPID_DBL_LISE_ED + 10),
        FPID_I_CLEAR_PEAK_MOYENNE = (FPID_DBL_LISE_ED + 11), //FPID_B_GETAUTOAIRGAPCONFIG = (FPID_DBL_LISE_ED + 12),
        FPID_I_VISIBLE_PROBE = (FPID_DBL_LISE_ED + 13),
        FPID_D_CURSOURCEPOWER_UPPER = (FPID_DBL_LISE_ED + 14),
        FPID_D_CURSOURCEPOWER_LOWER = (FPID_DBL_LISE_ED + 15),
        FPID_I_AVERAGEPARAM_UPPER = (FPID_DBL_LISE_ED + 16),
        FPID_I_AVERAGEPARAM_LOWER = (FPID_DBL_LISE_ED + 17),
        FPID_I_CALIBRATION_MODE = (FPID_DBL_LISE_ED + 18),
        FPID_D_MAXIMUM_GAIN_UPPER = (FPID_DBL_LISE_ED + 19),            // Permet de modifier le gain Maximum de la sonde du haut,
        FPID_D_MAXIMUM_GAIN_LOWER = (FPID_DBL_LISE_ED + 20),            // Permet de modifier le gain Maximum de la sonde du bas,
        FPID_I_REPEATS_CALIBRATION = (FPID_DBL_LISE_ED + 21),               // Permet de modifier le nombre de repeat de la calibration,
        FPID_B_CANCEL_CALIBRATION = (FPID_DBL_LISE_ED + 22),        // Permet d'annuler la calibration
        FPID_D_FORCE_CALIBRATE_TOTAL_TH = (FPID_DBL_LISE_ED + 23),

        //#### DIAMOND PARAMS ######,
        FPID_D_ABE = (FPID_SPIRO + 1),  // Modification du nombre d'ABE dans le boitier,
        FPID_D_LAMPINTENSITY = (FPID_SPIRO + 2),    // Modification de l'intensite de la lampe halogenne,
        FPID_I_FLAGS_VALUE = (FPID_SPIRO + 3),  // Flags,
        FPID_I_SETOPTICALPEN = (FPID_SPIRO + 4),// Optical Pen

        //#### EXTENDED LISE ED PARAMS ######,
        FPID_C_SETABSPATH = (FPID_EXT_LISE_ED + 1),

        //#### LENSCAN-SPECIFIC PARAMS ####,
        FPID_D_COUPLEDPOWER = (FPID_LENSCAN + 1),   //current value of IR coupled power,
        FPID_I_SOURCEALARM = (FPID_LENSCAN + 2),    //source alarm bit

        FPID_D_RAWNOISEBASE = (FPID_LENSCAN + 3), //adjust the detection sensitivity,
        FPID_D_SCANNERLENGTH_UM = (FPID_LENSCAN + 4),  //set a software limit to the scanner length

        FPID_D_TEMPDL = (FPID_LENSCAN + 10),    //delay line temperature (used for the last measurement),
        FPID_D_TEMPEXT = (FPID_LENSCAN + 11),   //ext temperature (meteo option: GET only, no option: SET user defined value),
        FPID_D_HUMIDEXT = (FPID_LENSCAN + 12),  //ext humid (always 50%, not measured),
        FPID_D_PRESSUREEXT = (FPID_LENSCAN + 13), //ext pressure (meteo option: GET only, no option: SET user defined value)

        FPID_D_NDL = (FPID_LENSCAN + 15),   //delay line optical index (at measurement wavelength),
        FPID_I_XSAMPLESCOUNT = (FPID_LENSCAN + 16), //nombre de valeurs de X,
        FPID_E_XSAMPLES = (FPID_LENSCAN + 17),  //axe des abscisses linéarisé de GetRawSignal
        FPID_I_RAWSAMPLES = (FPID_LENSCAN + 18), //nbre d'echantillons,
        FPID_I_RAWCHANNELS = (FPID_LENSCAN + 19), //nbre de channels,
        FPID_E_RAW = (FPID_LENSCAN + 20), //recupere les echantillon d'un channel,
        FPID_E_RAWFIRST = (FPID_LENSCAN + 20),
        FPID_E_RAWLAST = (FPID_LENSCAN + 20 + FPID_RAWCHANNELS - 1),

        FPID_I_SETMUX = (FPID_LENSCAN + 30), // adjust the output multiplexer (Lenscan SW 4)

        FPID_I_CT_ISENABLED = (FPID_LENSCAN + 32),
        FPID_I_CT_GETPOSSTEPS = (FPID_LENSCAN + 33),
        FPID_I_CT_SETPOSSTEPS = (FPID_LENSCAN + 34),
        FPID_D_CT_SETFOCUS = (FPID_LENSCAN + 35),
        FPID_I_CT_RESET = (FPID_LENSCAN + 36),

        FPID_I_AUTOTESTFIRST = (FPID_LENSCAN + 40), //factory/debug loops,
        FPID_I_AUTOTESTPOWER = (FPID_LENSCAN + 41),
        FPID_I_AUTOTESTXY = (FPID_LENSCAN + 42),
        FPID_I_AUTOTESTFOCUS = (FPID_LENSCAN + 43),
        FPID_I_AUTOTESTMAT = (FPID_LENSCAN + 44),
        FPID_I_AUTOTESTSTAGE = (FPID_LENSCAN + 45), FPID_I_AUTOTESTLAST = (FPID_LENSCAN + 46)
    }
}
