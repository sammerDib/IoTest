using System;
using System.Collections.Generic;
using System.Text;

namespace UnitySC.ADCAS300Like.Common
{
    public enum enumCodageAdaModuleID { Topography = 0, BrightField2D = 1, Darkfield = 2, BrightFieldPattern = 3, PMEdge = 4, Nanotopography = 5, Saturne = 6, BrightField3D = 7, AlignerEdge = 8, LightSpeed = 9 }

    [Flags]
    public enum enumProcessType { MODE_NONE = 0, OCR = 1, ALIGNER_EDGE = 2, FRONTSIDE = 4, BACKSIDE = 8, FLIP_WAFER = 16, BRIGHTFIELD1 = 32, BRIGHTFIELD2 = 64, BRIGHTFIELD3 = 128, BRIGHTFIELD4 = 256, DARKVIEW = 512, PSD = 1024, TOPOGRAPHY = 2048, PM_EDGE = 4096, PM_REVIEW = 8192, PM_LIGHTSPEED = 16384, PM_CENTERING = 32768, ALL_PROCESSES_DISABLED = 65536 };

    public enum EnumProcessStatus { psSTOPPED = 0, psNOTSTARTED = 1, psSTARTED = 2, psCOMPLETE = 3, psERROR = 4, psSKIPPED = 5, psWARNING = 6 }

    public enum enumSideType { NONE = 0, FRONTSIDE, BACKSIDE }

    public enum enumFaceUsed { fuDisabled, fuFlipWafer, fuFrontsideOnly, fuBacksideOnly, fuBothFace }

    public enum WAFER_PROCESS_STATE
    {
        WAFER_UNPROCESSED,
        WAFER_PROCESSING,
        WAFER_PROCESSED,
        WAFER_PROCESSING_ERROR
    };



    public enum enumTypeField { tfString = 0, tfBoolean, tfInteger, tfDouble }
    public enum enumVidType { dtU4 = 0, dtASCII, dtF8, dtList, dtBool }

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


    [Flags]
    public enum enumWaferSizeDedicated { wdNothing = 0, wd3 = 1, wd4 = 2, wd5 = 4, wd6 = 8, wd8 = 16, wd12 = 32 }

    public enum enumSerializationType { stCBaseMessage, stCBaseMessageBrightfield, stNature }

    public enum enumStepSplit { eStepStartSplit = 0, eStepEndSplit = 1 }

    public enum enumCarrierTypeDisplay { ctdNotused = 0, ctdType, ctdThickness }

    public enum enumStatusType { stPresence, stPlaced, stSize }

    [Flags]
    public enum enumSubstrateTypeDefinition { wtdUnknow = 0, wtdFilmFrame = 1, wtdSize75mm = 2, wtdSize100mm = 4, wtdSize150mm = 8, wtdSize200mm = 16, wtdSize300mm = 32 }; // if not FilfmFrame then it is a wafer standard

    public enum enumIOType { tPLCIO, tEFEMSignal }

    public enum enum3State { stFalse, stTrue, stDisabled }

    public enum enumContenairType { ctContainer, ctItems }

    public enum enumPPStatus { ppsNotStarted, ppsInProgress, ppsAborted, ppsComplete }


    public enum AllEventID { eEventAllPostProcess_Complete = 0, eEventAllPostProcess_Failed };

    public enum enumCycleProgramType { cptMRJ, cptFileName }

    public enum VariableResults { vrSUCCESS, vrLIMITCROSSED, vrNOCHANGE, vrFAILURE }

    public enum VarType { I8,I1,I2,I4,F8,F4,U8,U1,U2,U4, A, L,Bo }


    public enum EFEMStationID
    {
        stidUnknown,
        stidLoadPort1,
        stidLoadPort2,
        stidLoadPort3,
        stidLoadPort4,
        stidRobot1,
        stidRobot2,
        stidPrealigner1,
        stidPrealigner2,
        stidToolStation1,
        stidToolStation2,
        stidToolStation3,
        stidToolStation4,
        stidVaCBERobot,
        stidVaCBEEntryLock,
        stidVaCBEParkPos,
        stidVaCBEAnalysisStation
    }
    public enum VSConstants { VS_OK = 0, E_FAIL, E_ACCESSDENIED }
}
