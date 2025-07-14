/*
 * $Id: FogaleProbeParamID.h 9020 2009-05-14 13:00:30Z S-PETITGRAND $
 */

//C: char* c; FP_GetParam(Probe,c,FPID_C);	//chaine de caractères
//I: int i; FP_GetParam(Probe,&i,FPID_I);	//entier
//D: double d; FP_GetParam(Probe,&d,FPID_D);//double
//E: double d[...]; FP_GetParam(Probe,d,FPID_D);//tableau double
//element i d'une table: FP_GetParam(Probe,x,FPID_X+i);

//identifiants
#define FPID_COMMON			0
#define FPID_MATERIALS		10000
#define FPID_LENSCAN		21100 //(FPID_COMMON+100+2*FPID_MATERIALS)
#define FPID_DBL_LISE_ED	60000 //(FPID_COMMON+100+2*FPID_MATERIALS)
#define FPID_EXT_LISE_ED	70000
#define FPID_SPIRO		80000
#define FPID_RAWCHANNELS	6
#define FPID_STRLEN			256

//#### COMMON PARAMS ####
#define FPID_C_TYPE				(FPID_COMMON	+	1)	//probe type
#define FPID_C_VERSION			(FPID_COMMON	+	2)	//probe module version
#define FPID_I_STATUS			(FPID_COMMON	+	3)	//probe status
#define FPID_C_SERIAL			(FPID_COMMON	+	4)	//probe serial number (string)
#define FPID_D_WORKINGDISTANCE	(FPID_COMMON	+	5)	//probe working distance
#define FPID_D_RANGE			(FPID_COMMON	+	6)	//probe range
#define FPID_I_TOTAL_PROBE_USED	(FPID_COMMON	+	7)	//num max de probe
#define FPID_C_PN				(FPID_COMMON	+	8)	//Part Number

#define FPID_D_CURSOURCEPOWER	(FPID_COMMON	+	10)  //probe source power
#define FPID_D_MINSOURCEPOWER	(FPID_COMMON	+	11)
#define FPID_D_MAXSOURCEPOWER	(FPID_COMMON	+	12)
#define FPID_D_STPSOURCEPOWER	(FPID_COMMON	+	13)

#define FPID_I_STAGEX			(FPID_COMMON	+	14)
#define FPID_I_STAGEY			(FPID_COMMON	+	15)
#define FPID_I_STAGEZ			(FPID_COMMON	+	16)
#define FPID_B_AUTOGAIN			(FPID_COMMON	+	17)
#define FPID_B_AUTOAIRGAPCONFIG	(FPID_COMMON +	18)
#define FPID_I_AVERAGEPARAM		(FPID_COMMON +	19)

#define FPID_C_LOG				(FPID_COMMON	+	20) //log (string) max length 4096 (MaxConsole) Used to retrive the log/error messages and flush the corresponding internal buffer

#define FPID_I_NUMCHANNEL			(FPID_COMMON	+	21) //return the channel count in the raw signal (if more than 1 channel data are interleaved)
#define FPID_C_SAVESIGNAL			(FPID_COMMON	+	22)
#define FPID_D_AMPLITUDEPEAKS		(FPID_COMMON	+	23)

#define FPID_B_LIMITEDTIME_MODE		(FPID_COMMON	+	24)	// Mode de temps limité pour le getThickness
#define FPID_D_SINGLESHOTDURATION	(FPID_COMMON	+	25)	// Durée d'une mesure en single shot
#define FPID_B_TRIGGERED_MODE	(FPID_COMMON	+	26)		// Configurer le Get Thicknesses en mode triggered  
#define FPID_D_COMPARISON_TOLERANCE	(FPID_COMMON	+	27)	// Permet de modifier la tolerance entre les chemins aller/retour
#define FPID_D_MAXIMUM_GAIN         (FPID_COMMON      +     28)   // Permet de modifier le gain Maximum de la sonde
#define FPID_D_WAVE_LENGTH          (FPID_COMMON      +     29)   // Permet de modifier la longeur d'onde du faisceau de la sonde
#define FPID_D_SATURATION_THRESHOLD (FPID_COMMON      +     30)   // Permet de modifier le seuil de saturation de la sonde
#define FPID_C_NI_DEVICE            (FPID_COMMON      +     31)   // Permet de modifier le NI device de la sonde
#define FPID_S_SAVE_CURRENT_PARAMS			(FPID_COMMON      +     32)   // Permet de lancer une sauvegarde des parametres courants avec le bon password
#define FPID_B_GET_PROBE_MEASURING_STATE	(FPID_COMMON    +   33) // Permet de connaitre l'etat de la mesure en continuous
#define FPID_D_PROBE_FREQUENCY				(FPID_COMMON	+   34) // Get de la frequence de la probe
#define FPID_D_AIRGAP_THRESHOLD				(FPID_COMMON	+   35) // définition du seuil sur l'air gap
#define FPID_D_AIRGAP_UPPER_THRESHOLD		(FPID_COMMON	+   36) // définition du seuil sur l'air gap
#define FPID_D_AIRGAP_LOWER_THRESHOLD		(FPID_COMMON	+   37) // définition du seuil sur l'air gap
#define FPID_D_AIRGAP_TH_SWITCH_MODE		(FPID_COMMON	+   38) // définition de l'airgap de switch
#define FPID_D_BASE_LINE					(FPID_COMMON	+	39)	// permet de récupérer la valeur de la ligne de base
#define FPID_D_UPPER_BASE_LINE				(FPID_COMMON	+	40)	// permet de récupérer la valeur de la ligne de base de la sonde du dessus en double
#define FPID_D_LOWER_BASE_LINE				(FPID_COMMON	+	41)	// permet de récupérer la valeur de la ligne de base de la sonde du dessous en double
// Probe standby
#define FPID_B_STANDBY						(FPID_COMMON	+	42)	// Passe la probe en veille (si possible) et sort de veille
#define FPID_D_STANDBY_VALUE				(FPID_COMMON	+	43)	// Recupère la valeur de standby
#define FPID_D_FULLINTENSITY_VALUE			(FPID_COMMON	+	44)	// Récupère la valeur à maximum dans le fichier de parametres

#define FPID_I_SETCURRENTPROBE_ONLY (FPID_COMMON +	50)
#define FPID_I_SETVISIBLEPROBE (FPID_COMMON +	51)


#define FPID_I_GETNUMMATERIALS	(FPID_COMMON	+	 99) //total count of material in database (FP_UNAVAILABLE if probe does not have database)

#define FPID_C_GETMATERIALNAME	    (FPID_COMMON	+	100) //char** Name[i]; FP_GetParam(Probe,Name[i],FPID_C_GETMATERIALNAME+i)
#define FPID_C_GETMATERIALNAMEFIRST	(FPID_COMMON	+	100) //char** Name[i]; FP_GetParam(Probe,Name[i],FPID_C_GETMATERIALNAME+i)
#define FPID_C_GETMATERIALNAMELAST	(FPID_COMMON	+	100 + FPID_MATERIALS -1 ) //char** Name[i]; FP_GetParam(Probe,Name[i],FPID_C_GETMATERIALNAME+i)

#define FPID_D_GETMATERIALINDEX		(FPID_COMMON	+	100 + FPID_MATERIALS) // //double* Index[i]; FP_GetParam(Probe,Index+i,FPID_C_GETMATERIALINDEX+i)
#define FPID_D_GETMATERIALINDEXFIRST (FPID_COMMON	+	100 + FPID_MATERIALS)
#define FPID_D_GETMATERIALINDEXLAST	(FPID_COMMON	+	100 + FPID_MATERIALS + FPID_MATERIALS -1 )


//#### DOUBLE LISE ED PARAMS ######
#define FPID_I_SETCURRENTPROBE		(FPID_DBL_LISE_ED + 1)
#define FPID_D_CALIBRATE_TOTAL_TH	(FPID_DBL_LISE_ED + 2)
#define FPID_I_NUM_OF_PROBE			(FPID_DBL_LISE_ED + 3)
#define FPID_I_GETCURRENTPROBE		(FPID_DBL_LISE_ED + 4)
#define FPID_D_ZVALUE_TOTAL_TH		(FPID_DBL_LISE_ED + 5)
#define FPID_D_XVALUE_TOTAL_TH		(FPID_DBL_LISE_ED + 6)
#define FPID_D_YVALUE_TOTAL_TH		(FPID_DBL_LISE_ED + 7)
#define FPID_D_CALIBRATE_LOWER_AG	(FPID_DBL_LISE_ED + 8)
#define FPID_D_CALIBRATE_UPPER_AG	(FPID_DBL_LISE_ED + 9)
#define FPID_D_CALIBRATE_TH_USED	(FPID_DBL_LISE_ED + 10)
#define FPID_I_CLEAR_PEAK_MOYENNE	(FPID_DBL_LISE_ED + 11)
//#define FPID_B_GETAUTOAIRGAPCONFIG	(FPID_DBL_LISE_ED + 12)
#define FPID_I_VISIBLE_PROBE		(FPID_DBL_LISE_ED + 13)
#define FPID_D_CURSOURCEPOWER_UPPER	(FPID_DBL_LISE_ED + 14)
#define FPID_D_CURSOURCEPOWER_LOWER	(FPID_DBL_LISE_ED + 15)
#define FPID_I_AVERAGEPARAM_UPPER	(FPID_DBL_LISE_ED + 16)
#define FPID_I_AVERAGEPARAM_LOWER	(FPID_DBL_LISE_ED + 17)
#define FPID_I_CALIBRATION_MODE		(FPID_DBL_LISE_ED + 18)
#define FPID_D_MAXIMUM_GAIN_UPPER   (FPID_DBL_LISE_ED + 19)		// Permet de modifier le gain Maximum de la sonde du haut
#define FPID_D_MAXIMUM_GAIN_LOWER   (FPID_DBL_LISE_ED + 20)		// Permet de modifier le gain Maximum de la sonde du bas
#define FPID_I_REPEATS_CALIBRATION  (FPID_DBL_LISE_ED + 21)		// Permet de modifier le nombre de repeat de la calibration
#define FPID_B_CANCEL_CALIBRATION  (FPID_DBL_LISE_ED + 22)		// Permet d'annuler la calibration
#define FPID_D_FORCE_CALIBRATE_TOTAL_TH	(FPID_DBL_LISE_ED + 23)

//#### DIAMOND PARAMS ######
#define FPID_D_ABE					(FPID_SPIRO + 1)	// Modification du nombre d'ABE dans le boitier
#define FPID_D_LAMPINTENSITY		(FPID_SPIRO + 2)	// Modification de l'intensite de la lampe halogenne
#define FPID_I_FLAGS_VALUE			(FPID_SPIRO + 3)	// Flags
#define FPID_I_SETOPTICALPEN		(FPID_SPIRO + 4)	// Optical Pen

//#### EXTENDED LISE ED PARAMS ######
#define FPID_C_SETABSPATH			(FPID_EXT_LISE_ED + 1)

//#### LENSCAN-SPECIFIC PARAMS ####
#define FPID_D_COUPLEDPOWER			(FPID_LENSCAN	+	1)	//current value of IR coupled power
#define FPID_I_SOURCEALARM			(FPID_LENSCAN	+	2)	//source alarm bit

#define FPID_D_RAWNOISEBASE			(FPID_LENSCAN	+	3)  //adjust the detection sensitivity
#define FPID_D_SCANNERLENGTH_UM		(FPID_LENSCAN	+	4)  //set a software limit to the scanner length

#define FPID_D_TEMPDL				(FPID_LENSCAN	+	10)	//delay line temperature (used for the last measurement)
#define FPID_D_TEMPEXT				(FPID_LENSCAN	+	11)	//ext temperature (meteo option: GET only, no option: SET user defined value)
#define FPID_D_HUMIDEXT				(FPID_LENSCAN	+	12)	//ext humid (always 50%, not measured)
#define FPID_D_PRESSUREEXT			(FPID_LENSCAN	+	13) //ext pressure (meteo option: GET only, no option: SET user defined value)

#define FPID_D_NDL					(FPID_LENSCAN	+	15)	//delay line optical index (at measurement wavelength)
#define FPID_I_XSAMPLESCOUNT		(FPID_LENSCAN	+	16)	//nombre de valeurs de X
#define FPID_E_XSAMPLES				(FPID_LENSCAN	+	17)	//axe des abscisses linéarisé de GetRawSignal

#define FPID_I_RAWSAMPLES			(FPID_LENSCAN	+	18) //nbre d'echantillons
#define FPID_I_RAWCHANNELS			(FPID_LENSCAN	+	19) //nbre de channels
#define FPID_E_RAW					(FPID_LENSCAN	+	20) //recupere les echantillon d'un channel
#define FPID_E_RAWFIRST				(FPID_LENSCAN	+	20)
#define FPID_E_RAWLAST				(FPID_LENSCAN	+	20 + FPID_RAWCHANNELS - 1)

#define FPID_I_SETMUX				(FPID_LENSCAN	+	30) // adjust the output multiplexer (Lenscan SW 4)

#define FPID_I_CT_ISENABLED			(FPID_LENSCAN	+	32)
#define FPID_I_CT_GETPOSSTEPS		(FPID_LENSCAN	+	33)
#define FPID_I_CT_SETPOSSTEPS		(FPID_LENSCAN	+	34)
#define FPID_D_CT_SETFOCUS			(FPID_LENSCAN	+	35)
#define FPID_I_CT_RESET					(FPID_LENSCAN	+	36)

#define FPID_I_BENCH_ISENABLED			(FPID_LENSCAN	+	42)
//efine FPID_I_BENCH_GETPOSSTEPS		(FPID_LENSCAN	+	43)
//efine FPID_I_BENCH_SETPOSSTEPS		(FPID_LENSCAN	+	44)
#define FPID_D_BENCH_SETPOS		(FPID_LENSCAN	+	45) //utiliser SETPOS pour piloter le banc, passer la position en microns
#define FPID_I_BENCH_RESET			(FPID_LENSCAN	+	46)
#define FPID_D_BENCH_GETPOS			(FPID_LENSCAN	+	47)

#define FPID_I_AUTOTESTFIRST		(FPID_LENSCAN	+	50) //factory/debug loops
#define FPID_I_AUTOTESTPOWER		(FPID_LENSCAN	+	51)
#define FPID_I_AUTOTESTXY			(FPID_LENSCAN	+	52)
#define FPID_I_AUTOTESTFOCUS		(FPID_LENSCAN	+	53)
#define FPID_I_AUTOTESTMAT			(FPID_LENSCAN	+	54)
#define FPID_I_AUTOTESTSTAGE		(FPID_LENSCAN	+	55)
#define FPID_I_AUTOTESTLAST			(FPID_LENSCAN	+	56)
