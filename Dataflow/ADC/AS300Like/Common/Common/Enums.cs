using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public enum enumScreenSize { ss1280x1024 = 0, ss1920x1080 }

    public enum enumChamberType { ctNoProcessType, ctTopography, ctDarkview, ctBrightField, ctPSD, ctEdge, ctReview, ctLightSpeed, ctPMTool };

    public enum enumToolType { ttNoToolType, ttCentering }

    public enum enumConnection { CONNECT_ADC_FRONT = 0, CONNECT_ADC_BACK = 1, CONNECT_GRAB_FRONT = 2, CONNECT_GRAB_BACK = 3, CONNECT_GRAB_EDGE = 4, CONNECT_GRAB_DARKFIELD = 5, CONNECT_GRAB_BRIGHTFIELD1 = 6, CONNECT_GRAB_BRIGHTFIELD2 = 7, CONNECT_GRAB_BRIGHTFIELD3 = 8, CONNECT_GRAB_BRIGHTFIELD4 = 9, CONNECT_IMG_SERVER = 10, CONNECT_GRAB_REVIEW = 11, CONNECT_NANOTOPO = 12, CONNECT_ADC_EDGE = 13, CONNECT_ADC_BF1 = 14, CONNECT_ADC_BF2 = 15, CONNECT_ADC_BF3 = 16, CONNECT_ADC_BF4 = 17, CONNECT_ADC_DF = 18, CONNECT_PSD = 19, CONNECT_ADC_PSD = 20, CONNECT_PMEDGE = 21, CONNECT_ROBOT = 22, CONNECT_PMLS = 23, CONNECT_MANUALREVIEW = 24, CONNECT_ADC_LS = 25, CONNECT_ADC_BF_COMMON = 26, CONNECT_CENTERING = 27, CONECT_HERMOS=28, COUNT = 29 };

    public enum enumCodageAdaModuleID { Topography = 0, BrightField2D = 1, Darkfield = 2, BrightFieldPattern = 3, PMEdge = 4, Nanotopography = 5, Saturne = 6, BrightField3D = 7, AlignerEdge = 8, LightSpeed = 9 }

    [Flags]
    public enum enumProcessType { MODE_NONE = 0, OCR = 1, ALIGNER_EDGE = 2, FRONTSIDE = 4, BACKSIDE = 8, FLIP_WAFER = 16, BRIGHTFIELD1 = 32, BRIGHTFIELD2 = 64, BRIGHTFIELD3 = 128, BRIGHTFIELD4 = 256, DARKVIEW = 512, PSD = 1024, TOPOGRAPHY = 2048, PM_EDGE = 4096, PM_REVIEW = 8192, PM_LIGHTSPEED = 16384, PM_CENTERING = 32768, ALL_PROCESSES_DISABLED = 65536 };

    public enum EnumProcessStatus { psSTOPPED = 0, psNOTSTARTED = 1, psSTARTED = 2, psCOMPLETE = 3, psERROR = 4, psSKIPPED = 5, psWARNING=6 }

    public enum enumSideType { NONE = 0, FRONTSIDE, BACKSIDE }

    public enum enumFaceUsed { fuDisabled, fuFlipWafer, fuFrontsideOnly, fuBacksideOnly, fuBothFace }

    public enum enumEFEMConstructor { ecRORZE_JPN = 0, ecMECHATRONIC, ecRORZE_JPN_EC200, ecRORZE_TAI, ecAGILEO, ecUnknown }

    public enum enumBFProcessType { stNONE = 0, stWaferIDReading, stProcess2D, stProcess3D };

    public enum enumPSDProcessType { stNONE = 0, stWaferIDReading, stPSD };

    public enum enumToolProcessType { stNONE = 0, stCentering };

    public enum enumPMEdgeProcessType { stNONE = 0, stWaferIDReading, stPMEdge };

    public enum enumReviewProcessType { rptNONE = 0, rptWaferIDReading, rptReview };

    public enum enumAlignerPlace { apRight, apLeft };

    public enum enumReviewProcessMode { enumManual = 0, enumSemiAuto, enumFullAuto }

    [Flags]
    public enum enumOrientationMarkTypeUsed { tUnknown = 0, tNotch = 1, tFlat = 2, tDoubleFlat = 4 }

    public enum WAFER_PROCESS_STATE
    {
        WAFER_UNPROCESSED,
        WAFER_PROCESSING,
        WAFER_PROCESSED,
        WAFER_PROCESSING_ERROR
    };

    public enum PM_COMMAND
    {
        CMD_INITIALIZE = 0,
        CMD_SHUTDOWN,
        CMD_PRELOAD,
        CMD_POSTLOAD,
        CMD_PREUNLOAD,
        CMD_POSTUNLOAD,
        CMD_STARTPROCESS,
        CMD_PROCESS,
        CMD_NOTIFY_PROCESS_COMPLETE,
        CMD_NOTIFY_RESULTSAVAILABLE_COMPLETE,
        CMD_STOPPROCESS,
        CMD_ABORTPROCESS,
        CMD_PAUSEPROCESS,
        CMD_RESUMEPROCESS,
        CMD_VALIDATERECIPE,
        CMD_RECIPEUPDATE,
        CMD_TRIGGEREVENT,
        CMD_NOTIFYGUI,
        CMD_LOGMESSAGE,
        CMD_NOTIFYERROR,
        CMD_STARTDIAGNOSTICS
    };

    public enum FLIP_STATUS
    {
        DO_NOT_FLIP_WAFER,
        WAFER_MUST_BE_FLIPPED,
        WAFER_FLIPPED
    };

    public enum enumCycleDFStep { csNotStarted, csFirstScanning, csFirstScanComplete, csRotationRunning, csRotationComplete, csSecondScanning, csSecondScanComplete, csCycleComplete, csAborted }

    public enum enumTypeField { tfString = 0, tfBoolean, tfInteger, tfDouble }
    public enum enumVidType { dtU4=0, dtASCII, dtF8, dtList, dtBool }

    public enum enumLoading { ldNothing = 0, ldLoadingNotAllowedModuleInError, ldLoadingInProgress, ldLoadingOk, ldLoadingProcessAborted };

    public enum enumUnloading { udNothing = 0, udUnloadingNotAllowedModuleInError, udUnloadingInProgress, udUnloadingOk };

    public enum enumInitialization { izNothing = 0, izInitModuleInError, izNothingInitInProgress, izInitOk };

    public enum enumTableLightsName { cnFrontside, cnBackside, cnEdge, cnDarkview, cnBrightField1, cnBrightField2, cnBrightField3, cnBrightField4, cnPSD, cnPMEdge, cnReview, cnLightSpeed }

    public enum ClientConnection
    {
        NONE = 0,
        CaptureFrontside_OK,
        CaptureFrontside_NC,
        CaptureFrontside_ABS,
        CaptureBackside_OK,
        CaptureBackside_NC,
        CaptureBackside_ABS,
        ModuleEdge_OK,
        ModuleEdge_NC,
        ModuleEdge_ABS,
        ModuleDarkview_OK,
        ModuleDarkview_NC,
        ModuleDarkview_ABS,
        ModuleBrightField1_OK,
        ModuleBrightField1_NC,
        ModuleBrightField1_ABS,
        ModuleBrightField2_OK,
        ModuleBrightField2_NC,
        ModuleBrightField2_ABS,
        ModulePSD_OK,
        ModulePSD_NC,
        ModulePSD_ABS,
        ModulePMEdge_OK,
        ModulePMEdge_NC,
        ModulePMEdge_ABS,
        ADCFrontside_OK,
        ADCFrontside_NC,
        ADCFrontside_ABS,
        ADCBackside_OK,
        ADCBackside_NC,
        ADCBackside_ABS,
        ADCEdge_OK,
        ADCEdge_NC,
        ADCEdge_ABS,
        ADCDarkfield_OK,
        ADCDarkfield_NC,
        ADCDarkfield_ABS,
        ADCBrightField1_OK,
        ADCBrightField1_NC,
        ADCBrightField1_ABS,
        ADCBrightField2_OK,
        ADCBrightField2_NC,
        ADCBrightField2_ABS,
        ADCPSDFront_OK,
        ADCPSDFront_NC,
        ADCPSDFront_ABS,
        ADCPSDBack_OK,
        ADCPSDBack_NC,
        ADCPSDBack_ABS,
        ModuleReview_OK,
        ModuleReview_NC,
        ModuleReview_ABS,
        ModuleLS_OK,
        ModuleLS_NC,
        ModuleLS_ABS,
        ADCLightSpeed_OK,
        ADCLightSpeed_NC,
        ADCLightSpeed_ABS,
    }

    public enum enumErrorReaction { erDoNothing, erCancelCurrentCommand, erStopAllcommands, erStopLoadportCommands, erRetryResume }

    public enum enumEFEMError
    {
        eeNO_ERROR = 0,
        eeUNKNOWN_ERROR = 1,
        eeVACUUMALARM = 3507,
        eeINIT = 10000,
        eeAIR = 10004,
        eeCMD_ERROR = 10006,
        eeMAINTENANCE = 10020,
        eeNOLINK = 10021,
        eeEFEM_ERROR = 10025,
        eeFLIPPER = 10026,
        eeFFUFAN = 10027,
        eeFFUPRESURE=10028,
        eeFFUEFEM = 10029,
        eeTHICKWAF = 10030,
        eeCROSSWAFER = 10031,
        eeBOWWAF = 10032,
        eeDBLWAF = 10033,
        eeTHINWAF = 10034,
        eeFFUPM = 10035,
        eeINTERLOCK = 10038,
        eeCHAMBER1 = 10039,
        eeCHAMBER2 = 10040,
        eeCHAMBER3 = 10041,
        eeCHAMBER4 = 10042,
        eeLOADPORT1 = 10043,
        eeLOADPORT2 = 10044,
        eeLOADPORT3 = 10045,
        eeLOADPORT4 = 10046,
        eeWAFERID_QRCODE_READ = 10047,
        eeJOB_IN_ERROR = 10048,
        eeALIGNMENTFAILED = 10050,
        eeINTERLOCKPM1ERROR = 10052,
        eeINTERLOCKPM2ERROR = 10053,
        eeINTERLOCKPM3ERROR = 10054,
        eeINTERLOCKPM4ERROR = 10055,
        eeALIGNT_CRITICAL_ERROR = 10056,
        eeIonizerPressure = 10057,
        eeIonizerAlarm = 10058,
        eeFramePortCoverOpened = 10059,
        eeTOOL_STOP_ALARM = 21009
    }

    public enum enumUseNotchFlat { nfAlwaysNotch = 0, nfAlwaysFlat, nfAlwaysSubFlat, nfAll }

    public enum CIM87_Callbacks
    {
        CIM87_CB_BIND = 1,
        CIM87_CB_PROCEEDWITHCARRIER,
        CIM87_CB_CANCELBIND,
        CIM87_CB_RESERVEATPORT,
        CIM87_CB_CANCELRESERVEATPORT,
        CIM87_CB_CANCELCARRIER,
        CIM87_CB_CARRIEROUT,
        CIM87_CB_CANCELALLCARRIEROUT,
        CIM87_CB_CANCELCARRIERATPORT,
        CIM87_CB_CANCELCARRIERNOTIFICATION,
        CIM87_CB_CANCELCARRIEROUT,
        CIM87_CB_CARRIERIN,
        CIM87_CB_CARRIERNOTIFICATION,
        CIM87_CB_CHANGEACCESSMODE,
        CIM87_CB_CHANGESERVICESTATUS,
        CIM87_CB_CARRIERRELEASE,
        CIM87_CB_CARRIERTAGREADDATA,
        CIM87_CB_CARRIERTAGWRITEDATA,
        CIM87_CB_CARRIERRECREATE,
        CIM87_CB_CARRIERIDVERIFIED,
        CIM87_CB_ALARMCHANGED,
        CIM87_CB_SLOTMAPVERIFIED,
        CIM87_CB_STATECHANGE,
        CIM87_CB_INSERVICETRANSFERSTATE
    }

    [Flags]
    public enum enumAlignerOptions { aoNothing = 0, aoCentering = 1, aoAlignment = 2, ao1stSensorUsed = 4, ao2ndSensorUsed = 8 }

    [Flags]
    public enum enumWaferSizeDedicated { wdNothing = 0, wd3 = 1, wd4 = 2, wd5 = 4, wd6 = 8, wd8 = 16, wd12 = 32 }

    public enum enumChangeCommand { ccChange1, ccChange2 }

    public enum enumOCReaderType { rtRORZE_DEFAULT = 0, rtRORZE_IOSS, rtMECHATRONIC_IOSS, rtRORZE_TAI }

    public enum enumOCRReadingSide { orsFront, orsBack }

    public enum enumOCRReadingPosition { orpWaferID_Standard, orpQRCode_Middle, orpFilmFrameID_Side, orpMaxCount }

    //WaferIDFront-WaferIDBack-QRCodeFront-QRCodeBack in recipe
    [Flags]
    public enum enumReaderType { rtNone = 0, rtWaferIDFront = 1, rtWaferIDBack = 2, rtQRCodeFront = 4, rtQRCodeBack = 8, rtFilmFrameFront = 16, rtFilmFrameBack = 32 }

    public enum enumCarrierType { crUnknown = 0, crType1 = 1, crType2, crType3, crType4 }

    public enum enumWaferThicknessType { wttUNKNOWN = 0, wttNORMAL, wttTHIN, wttTHICK }

    [Flags]
    public enum enumCarrierTypeItemEnalbed { ieNone = 0, ieSize = 1, ieWaferType = 2 }

    public enum enumOHTMethod { omDefault, omEFEMConstructor }

    public enum enumSerializationType { stCBaseMessage, stCBaseMessageBrightfield, stNature }

    public enum enumStepSplit { eStepStartSplit = 0, eStepEndSplit = 1 }

    public enum enumCarrierTypeDisplay { ctdNotused = 0, ctdType, ctdThickness }

    public enum enumStatusType { stPresence, stPlaced, stSize }

    [Flags]
    public enum enumSysEventCodeIO_RT { secioMaintenance_Switch = 1, secioFFU_Alarm = 2, secioPressure_Vac = 4, secioPressure_Air = 8, secioReserved1 = 16, secioReserved2 = 32, secioIonizer_Air = 64, secioIonizer_Alarm = 128, secioDoorsClosed = 256, secioLightCurtain = 512, secioInterlock = 1024, secioOCRSensor1Alarm = 2048, secioOCRSensor2Alarm = 4096, secioOCRServoOn = 8192 }
    [Flags]
    public enum enumFrmiEventCodeIO_RT { secioUnknown = 1, secioOperatorAccessButton = 2, secioCassettePresence = 4, secioCassettePlacement = 8, secioProtrudeSensor = 16, secioCoverClosed = 32 }

    public enum enumSysEventCodeTable_RT { secbMode_Run0_Maintenance1 = 0, secRobotStatus = 1, secRobotSpeed = 2, secLP1Status = 3, secbLP1Carrier_Present1 = 4, secLP2Status = 5, secbLP2Carrier_Present1 = 6, secLP3Status = 7, secbLP3Carrier_Present1 = 8, secLP4Status = 9, secbLP4Carrier_Present1 = 10, secAlignerStatus = 11, secbAlignerCarrier_Present1 = 12, secDoor_Opened0_Closed1_Status = 13, secVacuum_OFF0_ON1_Status = 14, secAir_OFF0_ON1_Status = 15 }

    public enum enumStationStatus_RT { srtUnknown = 0, srtIdle, srtError, srtMoving, srtLoaded, srtDoorClosed, srtDisable }

    public enum enumAlignerStatus_RT { srtUnknown = 0, srtIdle, srtError, srtMoving, srtDisable }

    public enum enumActionChange { acNoAction = 0, acLowArmUnload, acUpArmUnload, acBothArmUnload, acUnused_Unknown, acLowArmLoad, acUpArmLoad, acBothArmLoad }

    [Flags]
    public enum enumLoadportHwDesign { hdNothing = 0, hdClampUnclamp = 1, hdDockUndock = 2, hdOpenClose = 4, hdDoMapping = 8, hdLightLOADUNLOAD = 16, hdLightCLAMPDOCK=32 }

    public enum enumSubstrateType { stWafer = 0, stWaferFilmFrame, stNoMoreType };

    [Flags]
    public enum enumSubstrateTypeDefinition { wtdUnknow = 0, wtdFilmFrame = 1, wtdSize75mm = 2, wtdSize100mm = 4, wtdSize150mm = 8, wtdSize200mm = 16, wtdSize300mm = 32 }; // if not FilfmFrame then it is a wafer standard

    public enum enumIOType { tPLCIO, tEFEMSignal }

    public enum enum3State { stFalse, stTrue, stDisabled }

    public enum enumContenairType { ctContainer, ctItems}

    public enum enumPPStatus {  ppsNotStarted, ppsInProgress, ppsAborted, ppsComplete}


    public enum AllEventID { eEventAllPostProcess_Complete = 0, eEventAllPostProcess_Failed  };

    public enum enumCycleProgramType { cptMRJ, cptFileName }
}