/*
 * $Id: LISE_ED_DLL_Config.cpp 8257 2009-02-16 17:52:50Z S-PETITGRAND $
 */

#include <windows.h>
#include <stdio.h>
#include <string.h>
#include <experimental/filesystem>

// ## probe-common headers ##
#include "..\SrcC\SPG.h"
#include "../FD_FogaleProbe/NIDAQmxConfig.h"
// ## probe-common headers ##

// ## probe-specific headers ##
#include "../FD_LISE_General/LISE_Consts.h"
#include "../FD_LISE_General/PeakMatch.h"
#include "../FD_LISE_General/LISE_Struct_Process.h"
#include "../FD_LISE_General/LISE_Struct.h"

// #include "../FD_LISELS/LISE_LSLI_DLL_Internal.h"
// ## probe-specific headers ##

#include "LISE_ED_DLL_UI_Struct.h"
#include "LISE_ED_DLL_Internal.h"
#include "LISE_ED_DLL_Internal.h"
#include "LISE_ED_DLL_Acquisition.h"
#include "LISE_ED_DLL_Config.h"
#include "LISE_ED_DLL_Create.h"
#include "LISE_ED_DLL_General.h"
#include "LISE_ED_DLL_Process.h"
#include "LISE_ED_DLL_Log.h"
#include "LISE_ED_DLL_Reglages.h"



// Procedure pour faire une configuration de systeme avec des parametres par defaut.
int ConfigSystemDefault(LISE_ED& LiseEd)
{
    // General
    strcpy(LiseEd.Lise.TypeDevice, "LISE_ED");
    strcpy(LiseEd.Lise.SerialNumber, "XXXXXXXXXXXXX");
    LiseEd.Lise.Range = 6000.0;
    LiseEd.Lise.GainMin = 0.0;
    LiseEd.Lise.GainMax = 5.50000;
    LiseEd.Lise.GainStep = 0.100000;
    LiseEd.Lise.AutoGainStep = 0.100000;
    LiseEd.Lise.Frequency = 55;
    LiseEd.Lise.fSourceValue = 1.0;	// Valeur par defaut pour le signal emulé
    LiseEd.Lise.Moyenne = 16;
    LiseEd.Lise.bDebug = false;
    LiseEd.Lise.SettingWindowWidth = 960;
    LiseEd.Lise.SettingWindowHeight = 600;
    LiseEd.Lise.iConfigSensUnique = 0;

    // Calibration
    LiseEd.Lise.dRefWaveLengthNm = 1530.0; // valeur par défaut, a redefinir en fonction des systèmes
    LiseEd.Lise.dStepMicron = LiseEd.Lise.dRefWaveLengthNm / 4.0 / 1000.0;
    LiseEd.Lise.fThresholdSaturation = 6.90000;

    // Emulation
    LiseEd.bLiseEDConnected = true;
    LiseEd.iPicSatureDansSimulation = 2; // Indice compris entre 0 et 7 codé en binaire
    LiseEd.Lise.ReadFileForEmulation = 0;
    strcpy(LiseEd.Lise.FichierReadSignal, "Signal_Emule.txt");
    LiseEd.Lise.fNoiseEmulateSignal = 0.100000;

    // NI board
    char UseDeviceInformation[128] = "manual";
    strcpy(LiseEd.Lise.UseDeviceInformation, UseDeviceInformation);
    strcpy(LiseEd.Lise.NIDevice, "Dev1/");
    strcpy(LiseEd.Lise.SwitchPRecoupTInterne, "Dev1/port0/line0");
    strcpy(LiseEd.Lise.EnableLr, "Dev1/port0/line1");
    strcpy(LiseEd.Lise.Switch0, "Dev1/port0/line2");
    strcpy(LiseEd.Lise.Switch1, "Dev1/port0/line3");
    strcpy(LiseEd.Lise.AlarmLr, "Dev1/port0/line4");
    strcpy(LiseEd.Lise.AlarmSource, "Dev1/port0/line5");
    strcpy(LiseEd.Lise.Trigger, "PFI1");
    strcpy(LiseEd.Lise.SplClk, "PFI9");
    strcpy(LiseEd.Lise.VoieAnalogIn[0], "Dev1/ai0");
    strcpy(LiseEd.Lise.VoieAnalogIn[1], "Dev1/ai2");
    strcpy(LiseEd.Lise.PuissanceREcouplee, "Dev1/ai1");
    strcpy(LiseEd.Lise.ControlSource1, "Dev1/ao0");
    strcpy(LiseEd.Lise.ControlSource2, "Dev1/ao1");
    LiseEd.Lise.FrequencyHz = 1.70000E6;
    LiseEd.Lise.Timeout = 4.00000;
    LiseEd.Lise.ValMin = -10.0000;
    LiseEd.Lise.ValMax = 10.0000;
    LiseEd.Lise.NombredeVoie = 1;
    LiseEd.Lise.iNbSampleMaxBuffer = 100000;
    LiseEd.Lise.DisplayNIError = 1;

    // Signal processing
    LiseEd.Lise.BufferLen = 2000000;
    LiseEd.Lise.PicResultLen = 100000;
    LiseEd.Lise.iNombreEchantillons = (int)(15.0000 / (LiseEd.Lise.dRefWaveLengthNm / 4.0 / 1000.0));    ;
    LiseEd.Lise.PeakWidthMin = 50;
    LiseEd.Lise.PeakWidthMax = 100;
    LiseEd.fNiveauHaut = 9.80000;
    LiseEd.fNiveauBas = -9.80000;
    LiseEd.fTolerance = 0.500000;
    LiseEd.Lise.LigneDeBase = -5.00000;
    LiseEd.Lise.iLissage = 1;
    LiseEd.Lise.fitLen = 63;
    LiseEd.Lise.fitStep = 9;
    LiseEd.Lise.EcartTypiqueFit = 0.500000;
    LiseEd.Lise.ComparisonTolerance = 60.0000;
    LiseEd.Lise.bUseUpdateFirstAirGapDef = true;
    LiseEd.Lise.bUseAirGapAuto = true;

    // Optical reference
    LiseEd.iRefOpt = 1;
    LiseEd.Lise.sample.iRefOpt = LiseEd.iRefOpt; // on met à jour dans sample
    LiseEd.fPositionRefOpt = 500.000;
    LiseEd.fToleranceRefOpt = 200.000;
    LiseEd.bTheoOptRef = false;

    // File save
    LiseEd.Lise.bAllowSavePeaks = false;
    LiseEd.Lise.bAllowSaveThickness = false;
    LiseEd.Lise.FlagSavePeakMeasured = false;
    LiseEd.Lise.bAllowSavePeakMoyenne = false;
    strcpy(LiseEd.Lise.FileNameSavePeaks, "peaks.txt");
    strcpy(LiseEd.Lise.FileNamesaveThickness, "thickness.txt");
    strcpy(LiseEd.Lise.FileNameSavePeakMoyenne, "C:\\Program Files (x86)\\UnitySC\\FPMS V3.4.5.12\\Log\\PeakMoyennePb1.txt");
    LiseEd.Lise.IndiceDecimation = 1;
    LiseEd.Lise.fIntensityThreshold = 2.00000;
    LiseEd.Lise.fQualityThreshold = 1.00000;
    LiseEd.Lise.WritePeaksForCalibration = 0;
    LiseEd.Lise.iConfigMoyenneEpaisseurs = 0;

    // End of configuration file
    LiseEd.iMeasurementWaitTime = 400;

    //Other
    LiseEd.Lise.ResultLen = 500;
    LiseEd.Lise.fThresholAmplitudeAirgap = 0.5f;
    LiseEd.Lise.fThresholdSwithAGAutoAgTh = 0.1f;
    LiseEd.Lise.fAutoAirgapDetectionfactor = 3.0f;
    LiseEd.Lise.bUseAutoAirgapDetectionFactor = false;
    LiseEd.Lise.bSaveSignalBrut = false;
    LiseEd.Lise.bSaveSignalPerChan = false;
    LiseEd.Lise.bAllowSavePeakMoyenne = false;
    LiseEd.Lise.iCounterSignalBrut = 0;
    LiseEd.Lise.iCounterMeasure = 0;
    LiseEd.bEnableStandBy = true;
    LiseEd.Lise.bAllowSaveRawSignal = false;

	return 1;
}
// configuration du systeme par defaut et creation d'un fichier comprenant la config par defaut.
void ConfigInitFromFile(LISE_ED& LiseEd,char* ConfigFile)
{
    // initialisation de la structure par défaut
	ConfigSystemDefault(LiseEd);

    if (ConfigFile != 0 && std::experimental::filesystem::exists(ConfigFile))
    {
        SPG_CONFIGFILE CFG; SPG_ZeroStruct(CFG);
        CFG_Init(CFG, 0, ConfigFile, 1024, 0);

        strcpy(LiseEd.ConfigPath, ConfigFile);

        // General
        CFG_StringParam(CFG, "DeviceType", LiseEd.Lise.TypeDevice, "Type of device: Lise_ED, CHROM, CHROM_DOUBLE", 1);
        if (strcmp(LiseEd.Lise.TypeDevice, "LISE_ED_EXT") == 0)
        {
            //LiseEd.Lise.NombredeVoie = 3;
        }

        CFG_StringParam(CFG, "SerialNumber", LiseEd.Lise.SerialNumber, "Serial number of device", 1);
        CFG_StringParam(CFG, "PartNumber", LiseEd.Lise.PartNumber, "Part number of the device", 1);
        CFG_StringParam(CFG, "HardwareVersion", LiseEd.Lise.HardwareVersion, "Hardware Version of the device", 1);

        CFG_FloatParam(CFG, "ProbeRange", &LiseEd.Lise.Range, "Range of the probe (µm)", 1);
        CFG_FloatParam(CFG, "MinimumGain", &LiseEd.Lise.GainMin, "Minimum gain value", 1);
        CFG_FloatParam(CFG, "MaximumGain", &LiseEd.Lise.GainMax, "Maximum gain value", 1);
        CFG_FloatParam(CFG, "GainStep", &LiseEd.Lise.GainStep, "Gain step", 1);
        CFG_FloatParam(CFG, "AutoGainStep", &LiseEd.Lise.AutoGainStep, "Auto Gain step", 1);
        CFG_IntParam(CFG, "Frequency", &LiseEd.Lise.Frequency, "Measuring frequency (Hz)", 1);
        CFG_FloatParam(CFG, "DefaultGain", &LiseEd.Lise.fSourceValue, "Default gain value", 1);
        CFG_IntParam(CFG, "AverageParam", &LiseEd.Lise.Moyenne, "Number of points used to average the measures", 1);
        int DebugLogInstruction = (int)LiseEd.Lise.bDebug;
        CFG_IntParam(CFG, "DebugInLog", &DebugLogInstruction, "0: no debug information in log file,  1: add all debug information in log file", 1);
        LiseEd.Lise.bDebug = DebugLogInstruction ? true : false;
        CFG_IntParam(CFG, "WindowWidth", &LiseEd.Lise.SettingWindowWidth, "Width of parameter setting window", 1);
        CFG_IntParam(CFG, "WindowHeight", &LiseEd.Lise.SettingWindowHeight, "Height of parameter setting window", 1);
        CFG_IntParam(CFG, "SingleWayMode", &LiseEd.Lise.iConfigSensUnique, "0: use both scanning ways, 1: use single scanning way", 1);
        CFG_IntParam(CFG, "MeasurementWaitTime", &LiseEd.iMeasurementWaitTime, "Time for waiting start of thread measurement and first measurement in millisecond", 1);
        int iEnableStandBy = (int)LiseEd.bEnableStandBy;
        CFG_IntParam(CFG, "EnableStandBy", &iEnableStandBy, "0: stand-by not enabled,  1: stand-by enabled", 1);
        LiseEd.bEnableStandBy = iEnableStandBy ? true : false;

        // Calibration
        double TheoLambda = 1530.0;
        double CalibWavelength = LiseEd.Lise.dRefWaveLengthNm - TheoLambda;
        float CalibWavelengthFile = (float)CalibWavelength;
        CFG_FloatParam(CFG, "CalibWavelength", &CalibWavelengthFile, "Calibrated wavelength of reference laser minus 1530nm (nm)", 1);
        CalibWavelength = (double)CalibWavelengthFile;
        LiseEd.Lise.dRefWaveLengthNm = CalibWavelength + TheoLambda;
        LiseEd.Lise.dStepMicron = LiseEd.Lise.dRefWaveLengthNm / 4.0 / 1000.0;
        CFG_FloatParam(CFG, "SaturThreshold", &LiseEd.Lise.fThresholdSaturation, "Threshold used to detect peak saturation (V)", 1);

        // Emulation
        int EmulationOrConnected = (int)LiseEd.bLiseEDConnected;
        CFG_IntParam(CFG, "LISEEDConnected", &EmulationOrConnected, "0: for LISE ED emulation, 1: if LISE ED connected", 1);
        LiseEd.bLiseEDConnected = EmulationOrConnected ? true : false;
        CFG_IntParam(CFG, "SatPeaksEmul", &LiseEd.iPicSatureDansSimulation, "Number of saturated peaks in emulated signal", 1);
        CFG_IntParam(CFG, "EmulWithFile", &LiseEd.Lise.ReadFileForEmulation, "0: default emulated signal, 1: read emulated signal from a file", 1);
        CFG_StringParam(CFG, "EmulFileName", LiseEd.Lise.FichierReadSignal, "File that contains the emulated signal", 1);
        CFG_FloatParam(CFG, "EmulNoise", &LiseEd.Lise.fNoiseEmulateSignal, "Maximum voltage noise in emulated signal (V)", 1);

        /// NI board
        CFG_StringParam(CFG, "NIDeviceMode", LiseEd.Lise.UseDeviceInformation, "auto: find device automatically, manual: configure name of acquisition board in NIDevice parameter", 1);
        CFG_StringParam(CFG, "NIDevice", LiseEd.Lise.NIDevice, "Name of NI acquisition board", 1);


        // Gestion du mode automatique de détection de la carte NI
#ifdef DEVICECONNECTED
        if (LiseEd.bLiseEDConnected == true)
        {
            int iUseDeviceAuto = strcmp(LiseEd.Lise.UseDeviceInformation, "auto");
            int iUseDeviceManual = strcmp(LiseEd.Lise.UseDeviceInformation, "manual");
            if (iUseDeviceAuto == 0)
            {
                if (LiseEd.Lise.bDebug == true)
                {
                    LogfileF(*LiseEd.Lise.Log, "[LISEED]\t[dev]\t DevName config file%s", LiseEd.Lise.NIDevice);
                }

                char DevNames[1024];
                //char DevNameWithoutSlash[1024];
                DAQmxGetSysDevNames(DevNames, 1024);
                //strcpy(DevNameWithoutSlash,DevNames);

                if (LiseEd.Lise.bDebug == true)
                {
                    LogfileF(*LiseEd.Lise.Log, "[LISEED]\t[dev]\t DevName %s", DevNames);
                }
                //SPG_List(DevNames);
                char* StartOfName = DevNames;
                char* EndOfName = DevNames;
                int match = 0;
                int DeviceFound = 0;

                while (*EndOfName != '\0')
                {
                    while ((*EndOfName != ';') && (*EndOfName != '\0')) { EndOfName++; }
                    if (*EndOfName == ';')
                    {
                        *EndOfName = 0; EndOfName++;
                    }
                    else
                    {
                        //on est arrivé à la fin de la chaine, ne pas incrémenter au delà
                    }

                    char DevType[256];
                    DAQmxGetDevProductType(StartOfName, DevType, 256);

                    if (LiseEd.Lise.bDebug == true)
                    {
                        LogfileF(*LiseEd.Lise.Log, "[LISEED]\t[dev]\t DevType %s", DevType);
                    }
                    //SPG_List2S(StartOfName,DevType);
                    if (stricmp(DevType, "PCI-6110") == 0 || stricmp(DevType, "PCI-6111") == 0)
                    {
                        //if(match==LISE.DeviceNth)
                        {
                            //SPG_List("OK");
                            DeviceFound = 1;

                            if (LiseEd.Lise.bDebug == true)
                            {
                                LogfileF(*LiseEd.Lise.Log, "[LISEED]\t[dev]\t Device Found");
                            }
                            break;
                        }
                        match++;
                    }
                    else
                    {
                        if (LiseEd.Lise.bDebug == true)
                        {
                            LogfileF(*LiseEd.Lise.Log, "[LISEED]\t[dev]\t Warning - One device found, but you use the wrong card NI");
                        }
                    }
                }
                strcat(StartOfName, "/");

                if (LiseEd.Lise.bDebug == true)
                {
                    LogfileF(*LiseEd.Lise.Log, "[LISEED]\t[dev]\t DevName %s", StartOfName);
                }
                strcpy(LiseEd.Lise.NIDevice, StartOfName);
                //strcpy(Lise.NIDevice,DevNames);
                //CHECK(DeviceFound==0,"scxNIDAQOpen: No device matching DevType and DevNth",goto exit_CreateAI);

            }
            else if (iUseDeviceManual == 0)
            {
                // On garde la valeur lue dans le fichier de parametre
                int a = 0;
            }
        }
#endif
        CFG_StringParam(CFG, "SwitchPowAndT", LiseEd.Lise.SwitchPRecoupTInterne, "NIDAQmx line used to switch between coupled power and internal temperature", 1);
        CFG_StringParam(CFG, "EnableDelayLine", LiseEd.Lise.EnableLr, "NIDAQmx line used to enable delay line EnableLr", 1);
        CFG_StringParam(CFG, "SwitchZero", LiseEd.Lise.Switch0, "NIDAQmx line: switch 0", 1);
        CFG_StringParam(CFG, "SwitchOne", LiseEd.Lise.Switch1, "NIDAQmx line: switch 1", 1);
        CFG_StringParam(CFG, "AlarmDelayLine", LiseEd.Lise.AlarmLr, "NIDAQmx line: delay line alarm", 1);
        CFG_StringParam(CFG, "AlarmSourceLine", LiseEd.Lise.AlarmSource, "NIDAQmx line: source alarm", 1);
        CFG_StringParam(CFG, "TriggerLine", LiseEd.Lise.Trigger, "NIDAQmx line: trigger", 1);
        CFG_StringParam(CFG, "SampleClockLine", LiseEd.Lise.SplClk, "NIDAQmx line: sample clock timing. Set to OnboardClock if this line is not connected", 1);
        CFG_StringParam(CFG, "AnalogChannel1", LiseEd.Lise.VoieAnalogIn[0], "NIDAQmx line: analog measure channel 1", 1);
        CFG_StringParam(CFG, "AnalogChannel2", LiseEd.Lise.VoieAnalogIn[1], "NIDAQmx line: analog measure channel 2", 1);
        if (MAXVOIE > 2)
            CFG_StringParam(CFG, "AnalogChannel3", LiseEd.Lise.VoieAnalogIn[2], "NIDAQmx line: analog measure channel 3", 1);
        CFG_StringParam(CFG, "AnalogInCouPow", LiseEd.Lise.PuissanceREcouplee, "NIDAQmx line: coupled power analog input", 1);
        CFG_StringParam(CFG, "AnalogOutS1", LiseEd.Lise.ControlSource1, "NIDAQmx line: source 1", 1);
        CFG_StringParam(CFG, "AnalogOutS2", LiseEd.Lise.ControlSource2, "NIDAQmx line: source 2", 1);
        CFG_FloatParam(CFG, "FreqNoCkLine", &LiseEd.Lise.FrequencyHz, "Acquisition frequency if sample clock line is set to OnboardClock", 1);
        CFG_FloatParam(CFG, "TimeoutAcq", &LiseEd.Lise.Timeout, "Acquisition timeout in s", 1);
        CFG_FloatParam(CFG, "VoltMinAcq", &LiseEd.Lise.ValMin, "Minimum input voltage for data acquisition (V)", 1);
        CFG_FloatParam(CFG, "VoltMaxAcq", &LiseEd.Lise.ValMax, "Maximum input voltage for data acquisition (V)", 1);
        CFG_IntParam(CFG, "NbChannels", &LiseEd.Lise.NombredeVoie, "1: simple detection, 2: double detection", 1);
        CFG_IntParam(CFG, "MaxLengthPeriod", &LiseEd.Lise.iNbSampleMaxBuffer, "Maximum number of samples in a period", 1);
        CFG_IntParam(CFG, "DispNIError", &LiseEd.Lise.DisplayNIError, "0: display NI errors in message box, 1: display NI errors in log file", 1);

        // Signal processing
        CFG_IntParam(CFG, "BufferLength", &LiseEd.Lise.BufferLen, "Length of raw signal ring buffer", 1);
        LiseEd.Lise.BufferLen *= LiseEd.Lise.NombredeVoie;
        CFG_IntParam(CFG, "ResultLength", &LiseEd.Lise.PicResultLen, "Length of peak result ring buffer", 1);
        CFG_IntParam(CFG, "PeriodResLength", &LiseEd.Lise.ResultLen, "Length of period result ring buffer", 1);
        float fMinLengthPulse = (float)LiseEd.Lise.iNombreEchantillons * (float)LiseEd.Lise.dRefWaveLengthNm / 4.0f / 1000.0f;
        CFG_FloatParam(CFG, "MinLengthPulse", &fMinLengthPulse, "Minimum length of synchronisation pulses (µm)", 1);
        LiseEd.Lise.iNombreEchantillons = (int)(fMinLengthPulse / (LiseEd.Lise.dRefWaveLengthNm / 4.0 / 1000.0));
        CFG_IntParam(CFG, "MinWidthPeak", &LiseEd.Lise.PeakWidthMin, "Minimum number of samples to fit a peak", 1);
        CFG_IntParam(CFG, "MaxWidthPeak", &LiseEd.Lise.PeakWidthMax, "Maximum number of samples to fit a peak", 1);
        CFG_FloatParam(CFG, "HighLevelPulse", &LiseEd.fNiveauHaut, "Level of positive synchronization pulse (V)", 1);
        CFG_FloatParam(CFG, "LowLevelPulse", &LiseEd.fNiveauBas, "Level of negative synchronization pulse (V)", 1);
        CFG_FloatParam(CFG, "TolerancePulse", &LiseEd.fTolerance, "Tolerance for pulse detection (V)", 1);
        CFG_FloatParam(CFG, "BaseLine", &LiseEd.Lise.LigneDeBase, "Signal base line value (V)", 1);
        CFG_IntParam(CFG, "RawSmoothing", &LiseEd.Lise.iLissage, "Number of points used to smooth the raw signal", 1);
        CFG_IntParam(CFG, "FitLength", &LiseEd.Lise.fitLen, "Number of points used to fit the peaks", 1);
        CFG_IntParam(CFG, "PkSearchStep", &LiseEd.Lise.fitStep, "Peak search step used to increase speed", 1);
        float ecTypTemp = LiseEd.Lise.EcartTypiqueFit;
        CFG_FloatParam(CFG, "TypErrorFit", &ecTypTemp, "Typical error of the fit", 1);
        LiseEd.Lise.EcartTypiqueFit = (double)ecTypTemp;
        CFG_FloatParam(CFG, "ComparisonTol", &LiseEd.Lise.ComparisonTolerance, "in µm tolerance between peak from go and back", 1);
        CFG_FloatParam(CFG, "fAutoAirgapDetectionfactor", &LiseEd.Lise.fAutoAirgapDetectionfactor, "Mutiplying factor to define the quality of first peak choose for auto airgap auto detection.");
        CFG_FloatParam(CFG, "fThresholdSwithAGAutoAgTh", &LiseEd.Lise.fThresholdSwithAGAutoAgTh, "threshold witch allow to switch between auto airgap detection and detection with amplitude threshold.");
        int bUseAAGFactor = (int)LiseEd.Lise.bUseAutoAirgapDetectionFactor;
        CFG_IntParam(CFG, "iUseAutoAirgapFactor", &bUseAAGFactor, "0: don't use, use auto airgap factor", 1);
        LiseEd.Lise.bUseAutoAirgapDetectionFactor = bUseAAGFactor ? true : false;

        // Optical reference
        CFG_IntParam(CFG, "UseOptRef", &LiseEd.iRefOpt, "0: no optical reference used in computation, 1:use otical reference in computation", 1);
        // on met à jour dans sample
        LiseEd.Lise.sample.iRefOpt = LiseEd.iRefOpt;
        CFG_FloatParam(CFG, "OptRefPos", &LiseEd.fPositionRefOpt, "Position of optical reference peak (µm)", 1);
        CFG_FloatParam(CFG, "OptRefTol", &LiseEd.fToleranceRefOpt, "Tolerance on the position of the optical reference peak (µm)", 1);
        int bTOR = (int)LiseEd.bTheoOptRef;
        CFG_IntParam(CFG, "TheoOptRef", &bTOR, "0: default, use physical optical reference peak, 1: use theoretical reference peak", 1);
        LiseEd.bTheoOptRef = bTOR ? true : false;

        // File save
        int FlagSave = (int)LiseEd.Lise.bAllowSavePeaks;
        CFG_IntParam(CFG, "SavePeaks", &FlagSave, "0: no peaks file save, 1: automatic peaks file save", 1);
        LiseEd.Lise.bAllowSavePeaks = FlagSave ? true : false;
        int FlagSaveThickness = (int)LiseEd.Lise.bAllowSaveThickness;
        CFG_IntParam(CFG, "SaveThickness", &FlagSaveThickness, "0: no thickness file save, 1: automatic thickness file save", 1);
        LiseEd.Lise.bAllowSaveThickness = FlagSave ? true : false;

        int FlagSavePeakMeasure = (int)LiseEd.Lise.FlagSavePeakMeasured;
        CFG_IntParam(CFG, "SavePeakMeasure", &FlagSavePeakMeasure, "0: no peak file save, 1: automatic peak measured file save", 1);
        LiseEd.Lise.FlagSavePeakMeasured = FlagSavePeakMeasure ? true : false;

        // pour sortir le fichier peak moyenne
        int iFlagSavePeakMoyenne = (int)LiseEd.Lise.bAllowSavePeakMoyenne;
        CFG_IntParam(CFG, "SavePeaksAverage", &iFlagSavePeakMoyenne, "0: no peak average file save, 1: automatic peaks average file save", 1);
        LiseEd.Lise.bAllowSavePeakMoyenne = iFlagSavePeakMoyenne ? true : false;

        // pour sortir le buffer brut entrelacé (mode Extended)
        int iFlagSaveRawSignal = (int)LiseEd.Lise.bAllowSaveRawSignal;
        CFG_IntParam(CFG, "SaveRawSignal", &iFlagSaveRawSignal, "0: no raw signal file save, 1: automatic peaks average file save", 1);
        LiseEd.Lise.bAllowSaveRawSignal = iFlagSaveRawSignal ? true : false;

        // pour utiliser la redéfinition d'échantillon par rapport au premier Matching Success
        int iUseUpdateFirstAirGapDef = (int)LiseEd.Lise.bUseUpdateFirstAirGapDef;
        CFG_IntParam(CFG, "UpdateAirGapDef", &iUseUpdateFirstAirGapDef, "0: no use, 1: use update airgap definition after first matching success", 1);
        LiseEd.Lise.bUseUpdateFirstAirGapDef = iUseUpdateFirstAirGapDef ? true : false;

        LogfileF(*LiseEd.Lise.Log, "[LISEED]\t[CFG]\tUse Update AirGap Value: %i", iUseUpdateFirstAirGapDef);

        int iUseUpdateAirGapAuto = (int)LiseEd.Lise.bUseAirGapAuto;
        CFG_IntParam(CFG, "AutoAirGap", &iUseUpdateAirGapAuto, "0: no use, 1: use update airgap auto(use the two first peak detected above Qthreshold to define air gap). Update airgap def is more important than auto airgap. if the both are define, software use update airgape", 1);
        LiseEd.Lise.bUseAirGapAuto = iUseUpdateAirGapAuto ? true : false;

        CFG_StringParam(CFG, "PeaksFile", LiseEd.Lise.FileNameSavePeaks, "name of peaks file", 1);
        CFG_StringParam(CFG, "ThicknessFile", LiseEd.Lise.FileNamesaveThickness, "name of thickness file", 1);
        CFG_StringParam(CFG, "PeakAverageFile", LiseEd.Lise.FileNameSavePeakMoyenne, "name of peak average file", 1);
        CFG_StringParam(CFG, "RawSignalFile", LiseEd.Lise.FileNameRawSignal, ";Path for raw signal", 1);


        CFG_IntParam(CFG, "Decimation", &LiseEd.Lise.IndiceDecimation, "Decimation index in peaks and thickness files", 1);
        CFG_FloatParam(CFG, "MinIntensity", &LiseEd.Lise.fIntensityThreshold, "Intensity threshold to save peaks in file, only for calibration mode", 1);
        CFG_FloatParam(CFG, "MinQuality", &LiseEd.Lise.fQualityThreshold, "Quality threshold to save peaks in file, only for calibration mode", 1);
        CFG_IntParam(CFG, "PeaksForCal", &LiseEd.Lise.WritePeaksForCalibration, "0: default peaks file, 1: special peaks file for calibration", 1);
        CFG_IntParam(CFG, "ThFileMode", &LiseEd.Lise.iConfigMoyenneEpaisseurs, "0: display thickness for both ways in thickness file, 1: average between both ways in thickness file", 1);

        CFG_Close(CFG, 0);

        char StringTemp[128] = "";
        strcat(StringTemp, LiseEd.Lise.NIDevice);
        strcat(StringTemp, LiseEd.Lise.SwitchPRecoupTInterne);
        memcpy(LiseEd.Lise.SwitchPRecoupTInterne, "", 128);
        strcpy(LiseEd.Lise.SwitchPRecoupTInterne, StringTemp);

        memcpy(StringTemp, "", 128);
        strcat(StringTemp, LiseEd.Lise.NIDevice);
        strcat(StringTemp, LiseEd.Lise.EnableLr);
        strcpy(LiseEd.Lise.EnableLr, StringTemp);

        memcpy(StringTemp, "", 128);
        strcat(StringTemp, LiseEd.Lise.NIDevice);
        strcat(StringTemp, LiseEd.Lise.Switch0);
        strcpy(LiseEd.Lise.Switch0, StringTemp);

        memcpy(StringTemp, "", 128);
        strcat(StringTemp, LiseEd.Lise.NIDevice);
        strcat(StringTemp, LiseEd.Lise.Switch1);
        strcpy(LiseEd.Lise.Switch1, StringTemp);

        memcpy(StringTemp, "", 128);
        strcat(StringTemp, LiseEd.Lise.NIDevice);
        strcat(StringTemp, LiseEd.Lise.AlarmLr);
        strcpy(LiseEd.Lise.AlarmLr, StringTemp);

        memcpy(StringTemp, "", 128);
        strcat(StringTemp, LiseEd.Lise.NIDevice);
        strcat(StringTemp, LiseEd.Lise.AlarmSource);
        strcpy(LiseEd.Lise.AlarmSource, StringTemp);

        memcpy(StringTemp, "", 128);
        strcat(StringTemp, LiseEd.Lise.NIDevice);
        strcat(StringTemp, LiseEd.Lise.VoieAnalogIn[0]);
        strcpy(LiseEd.Lise.VoieAnalogIn[0], StringTemp);

        memcpy(StringTemp, "", 128);
        strcat(StringTemp, LiseEd.Lise.NIDevice);
        strcat(StringTemp, LiseEd.Lise.VoieAnalogIn[1]);
        strcpy(LiseEd.Lise.VoieAnalogIn[1], StringTemp);

        if (LiseEd.Lise.NombredeVoie >= 3)
        {
            memcpy(StringTemp, "", 128);
            strcat(StringTemp, LiseEd.Lise.NIDevice);
            strcat(StringTemp, LiseEd.Lise.VoieAnalogIn[2]);
            strcpy(LiseEd.Lise.VoieAnalogIn[2], StringTemp);
        }

        memcpy(StringTemp, "", 128);
        strcat(StringTemp, LiseEd.Lise.NIDevice);
        strcat(StringTemp, LiseEd.Lise.PuissanceREcouplee);
        strcpy(LiseEd.Lise.PuissanceREcouplee, StringTemp);

        memcpy(StringTemp, "", 128);
        strcat(StringTemp, LiseEd.Lise.NIDevice);
        strcat(StringTemp, LiseEd.Lise.ControlSource1);
        strcpy(LiseEd.Lise.ControlSource1, StringTemp);

        memcpy(StringTemp, "", 128);
        strcat(StringTemp, LiseEd.Lise.NIDevice);
        strcat(StringTemp, LiseEd.Lise.ControlSource2);
        strcpy(LiseEd.Lise.ControlSource2, StringTemp);

        if (LiseEd.Lise.bDebug == true)
        {
            LogfileF(*LiseEd.Lise.Log, "[LiseED]\t[dev]\t Use Information %s", LiseEd.Lise.UseDeviceInformation);
            LogfileF(*LiseEd.Lise.Log, "[LiseED]\t[dev]\t DevName config file%s", LiseEd.Lise.NIDevice);
            LogfileF(*LiseEd.Lise.Log, "[LISEED]\t[dev]\t LiseED Connected %i", LiseEd.bLiseEDConnected);
        }
    }

	return;
}


void UpdateConfigFromHardwareConfig(LISE_ED& LiseEd, LISE_HCONFIG* HardwareConfig)
{
    //General
    wcstombs(LiseEd.Lise.TypeDevice, HardwareConfig->TypeDevice, 128);
    wcstombs(LiseEd.Lise.SerialNumber, HardwareConfig->SerialNumber, 128);
    LiseEd.Lise.Range = HardwareConfig->ProbeRange;
    LiseEd.Lise.GainMin = HardwareConfig->MinimumGain;
    LiseEd.Lise.GainMax = HardwareConfig->MaximumGain;
    LiseEd.Lise.GainStep = HardwareConfig->GainStep;
    LiseEd.Lise.AutoGainStep = HardwareConfig->AutoGainStep;
    LiseEd.Lise.Frequency = HardwareConfig->Frequency;

    //Calibration
    LiseEd.Lise.dRefWaveLengthNm = 1530.0; // valeur par défaut, a redefinir en fonction des systèmes
    double TheoLambda = 1530.0;
    double CalibWavelength = LiseEd.Lise.dRefWaveLengthNm - TheoLambda;
    float CalibWavelengthFile = (float)CalibWavelength;
    CalibWavelengthFile = HardwareConfig->CalibWavelength;
    CalibWavelength = (double)CalibWavelengthFile;
    LiseEd.Lise.dRefWaveLengthNm = CalibWavelength + TheoLambda;
    LiseEd.Lise.dStepMicron = LiseEd.Lise.dRefWaveLengthNm / 4.0 / 1000.0;
    float fMinLengthPulse = (float)LiseEd.Lise.iNombreEchantillons * (float)LiseEd.Lise.dRefWaveLengthNm / 4.0f / 1000.0f;
    LiseEd.Lise.iNombreEchantillons = (int)(fMinLengthPulse / (LiseEd.Lise.dRefWaveLengthNm / 4.0 / 1000.0));


    //Signal processing
    LiseEd.Lise.ComparisonTolerance = HardwareConfig->ComparisonTol;
}



int GetNidaqmxVersion(LISE_ED& LiseEd)
{
#ifdef DEVICECONNECTED
	if(LiseEd.bLiseEDConnected == true)
	{
		uInt32 MajorVersion;
		uInt32 MinorVersion;
		DAQmxGetSysNIDAQMajorVersion(&MajorVersion);
		DAQmxGetSysNIDAQMinorVersion(&MinorVersion);
		if(LiseEd.Lise.bDebug == true)
		{
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\t Nidacq Major Version %i",MajorVersion);
			LogfileF(*LiseEd.Lise.Log,"[LISEED]\t Nidacq Minor Version %i",MinorVersion);
		}
		if(MajorVersion != MAJOR_VERSION)
		{
			if(LiseEd.Lise.bDebug == true)
			{	
				LogfileF(*LiseEd.Lise.Log,"[LISEED]\t Warning - Major Version is different than version used with DLL");
			}
		}
		if(MinorVersion != MINOR_VERSION)
		{
			if(LiseEd.Lise.bDebug == true)
			{
				LogfileF(*LiseEd.Lise.Log,"[LISEED]\t Warning - Minor Version is different than version used with DLL");
			}
		}
		if(MajorVersion != MAJOR_VERSION || MinorVersion != MINOR_VERSION)
		{
			if(LiseEd.Lise.bDebug == true)
			{
				LogfileF(*LiseEd.Lise.Log,"[LISEED]\t Please Install Nidaqmx Version %i.%i to ensure the software works correctly",MAJOR_VERSION,MINOR_VERSION);
			}
			char DisplayMsg[1024];
			sprintf(DisplayMsg,"You don't use the good version of Nidaqmx. It's possible that the software doesn't work correctly. To ensure the good use of software, please install nidaqmx version %i.%i",MAJOR_VERSION,MINOR_VERSION);
			if(Global.EnableList>=1) MessageBox(0,fdwstring(DisplayMsg),L"Nidaqmx Wrong Version",0);
			return STATUS_FAIL;
		}
	}
	else
	{
		return STATUS_OK;
	}
#endif

	return STATUS_OK;
}
