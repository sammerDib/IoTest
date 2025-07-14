
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

#include "../FD_LISEED/LISE_ED_DLL_UI_Struct.h"
#include "../FD_LISEED/LISE_ED_DLL_Internal.h"

// ## probe-specific headers ##
#include "DBL_LISE_ED_DLL_Internal.h"
#include "DBL_LISE_ED_DLL_Log.h"
#include "DBL_LISE_ED_DLL_Config.h"

void ConfigSystemDefault(DBL_LISE_ED& DblLiseEd)
{
	// initialisation de la structure à des params par défaut
	strcpy(DblLiseEd.strTypeDevice,"DBL_LISE_ED");

	DblLiseEd.iStatusProbe = PROBE_NON_INIT;
	DblLiseEd.iCurrentProbe = -1;
	DblLiseEd.iVisibleProbe = -1;
	DblLiseEd.iCalibrationMode = 2;
	DblLiseEd.iNbCalibrationRepeat = 16;

	DblLiseEd.iWaitAfterSwitch = 150;
}

void CreateConfigFromFile(DBL_LISE_ED& DblLiseEd, char* ConfigFile)
{
	// initialisation de la structure par défaut
	ConfigSystemDefault(DblLiseEd);

    if (ConfigFile != 0 && std::experimental::filesystem::exists(ConfigFile))
    {
        strcpy(DblLiseEd.strCfgFilePath, ConfigFile);

        SPG_CONFIGFILE CFG; SPG_ZeroStruct(CFG);
        CFG_Init(CFG, 0, ConfigFile, 1024, 0);

        CFG_StringParam(CFG, "DeviceType", DblLiseEd.strTypeDevice, "Type of device: LISE_ED, CHROM, CHROM_DOUBLE,LISE_ED_DOUBLE", 1);
        CFG_StringParam(CFG, "FirstCfgFile", DblLiseEd.strCfgTileFirstProbe, "config file for the first Lise ED", 1);
        CFG_StringParam(CFG, "SecondCfgFile", DblLiseEd.strCfgTileSecondProbe, "config file for the second Lise ED", 1);
        CFG_StringParam(CFG, "LogCalibrationFile", DblLiseEd.strLogCalibrationFile, "Log for each calibration", 1);

        // paramètres de calibration
        float fTotalTh = (float)DblLiseEd.dTotalThickness;
        CFG_FloatParam(CFG, "TotalThickness", &fTotalTh, "Total thickness value for Lise ED", 1);
        DblLiseEd.dTotalThickness = (double)fTotalTh;

        float fZ = (float)DblLiseEd.ZCalibrationValue;
        CFG_FloatParam(CFG, "ZValueForcalibration", &fZ, "associated Z value for calibration", 1);
        DblLiseEd.ZCalibrationValue = (double)fZ;

        CFG_IntParam(CFG, "WaitAfterSwitch", &DblLiseEd.iWaitAfterSwitch, "Wait time after switch probe", 1);
        CFG_IntParam(CFG, "CalibrationMode", &DblLiseEd.iCalibrationMode, "0, theorical value/ 1, measurement value/ default, measurement value", 1);
        CFG_IntParam(CFG, "CalibrationRepeats", &DblLiseEd.iNbCalibrationRepeat, "1, number of repeatition to process a calibration", 1);

        CFG_Close(CFG, 1);
    }

	// log du temps de switch
	LogDblED(DblLiseEd,PRIO_INFO,"[DBL CONFIG] WaitAfterSwitch = %d ms",DblLiseEd.iWaitAfterSwitch);
}



void UpdateConfigFromHardwareConfig(DBL_LISE_ED& DblLiseEd, DBL_LISE_HCONFIG* HardwareConfigDual)
{
    wcstombs(DblLiseEd.strTypeDevice, HardwareConfigDual->strTypeDevice, wcslen(HardwareConfigDual->strTypeDevice));
}


void SetTotalThicknessInCfgFile(DBL_LISE_ED& DblLiseEd, double CalibValue)
{
	SPG_CONFIGFILE CFG; SPG_ZeroStruct(CFG);
	CFG_Init(CFG,0,DblLiseEd.strCfgFilePath,1024,0);

	float fTotalTh = (float)CalibValue;
	float oldValue = 0.0;
	CFG_FloatParam(CFG,"TotalThickness",&oldValue,"total thickness value for calivbration",1);
	// pas nécessaire puisque fait en amont dans la fonction calibrate
	//DblLiseEd.dTotalThickness = (double)fTotalTh;
	CFG_SetFloatParam(CFG,"TotalThickness",fTotalTh);

	CFG_Close(CFG,1);
}

void SetZValueInCfgFile(DBL_LISE_ED& DblLiseEd, double ZValue)
{
	SPG_CONFIGFILE CFG; SPG_ZeroStruct(CFG);
	CFG_Init(CFG,0,DblLiseEd.strCfgFilePath,1024,0);

	float fZ = (float)ZValue;
	float oldValue = 0.0;
	CFG_FloatParam(CFG,"ZValueForcalibration",&oldValue,"associated Z value for calibration",1);
	// pas nécessaire puisque fait en amont dans la fonction calibrate
	//DblLiseEd.dTotalThickness = (double)fTotalTh;
	CFG_SetFloatParam(CFG,"ZValueForcalibration",fZ);
	
	// on sauvegarde dans la structure la valeur de la dernière calibration
	DblLiseEd.ZCalibrationValue = ZValue;

	CFG_Close(CFG,1);
}