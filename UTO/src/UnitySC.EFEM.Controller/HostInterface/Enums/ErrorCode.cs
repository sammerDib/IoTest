namespace UnitySC.EFEM.Controller.HostInterface.Enums
{
    /// <summary>
    /// Error types defined in RTI EFEM Protocol v2.26_210503.pdf, table p10
    /// </summary>
    public enum ErrorType
    {
        NoError          = 0,
        EfemError        = 1,
        MaintenanceError = 2
    }

    /// <summary>
    /// Error codes defined in RTI EFEM Protocol v2.26_210503.pdf, table p10
    /// </summary>
    public enum ErrorCode
    {
        Normal                            = 0x0000,
        ArmStateError                     = 0x0001,
        RobotError                        = 0x0002,
        RobotUpperArmHaveWafer            = 0x0003,
        RobotLowerArmHaveWafer            = 0x0004,
        RobotUpperArmNotHaveWafer         = 0x0005,
        RobotLowerArmNotHaveWafer         = 0x0006,
        RobotMoving                       = 0x0007,
        ArmNotAtHomePos                   = 0x0008,
        RobotLoadWaferRetryTimes          = 0x0009,
        AlignerSizeChangeError            = 0x0010,
        OcrCylinderPositionErrorFor12Inch = 0x0011,
        OcrCylinderPositionErrorFor8Inch  = 0x0012, 
        SystemWaferSizeAbnormal           = 0x0013,
        LoadPortDisable                   = 0x0101,
        CarrierNotLoad                    = 0x0102,
        CarrierLoaded                     = 0x0103,
        CarrierNotPresent                 = 0x0104,
        LoadPortDoorClose                 = 0x0105,
        RfidReadFailed                    = 0x0106,
        LoadPortError                     = 0x0107,
        FoupCassetteSlotNotHaveWafer      = 0x0108,
        FoupCassetteSlotHaveWafer         = 0x0109,
        LoadPortMoving                    = 0x0110,
        LoadPortCarrierTypeUndefined      = 0x0111,
        E84TpTimeout                      = 0x0120,
        E84Tp1Timeout                     = 0x0121,
        E84Tp2Timeout                     = 0x0122,
        E84Tp3Timeout                     = 0x0123,
        E84Tp4Timeout                     = 0x0124,
        E84Tp5Timeout                     = 0x0125,
        E84SignalError                    = 0x0126,
        LoadPortStatusError               = 0x0127,
        EfemSwitchToMaintenanceError      = 0x0128,
        LoadPortE84Connect                = 0x0129,
        PleaseSelectRetryOrAbort          = 0x0130,
        FitcE84StatusError                = 0x0131,
        E84Td0Timeout                     = 0x0132,
        E84Cs0Timeout                     = 0x0133,
        FitcE84SignalError1               = 0x0134, // FITC E84 Signal error Wait GO => ON but CS_0, Valid, TR_REQ, Busy, COMPT, anyone => ON
        FitcE84SignalError2               = 0x0135, // FITC E84 Signal error Wait CS_0 => ON but GO => OFF
        FitcE84SignalError3               = 0x0136, // FITC E84 Signal error Wait CS_0 => ON but Valid, TR_REQ, BUSY, COMPT anyone => OFF
        FitcE84SignalError4               = 0x0137, // FITC E84 Signal error Wait VALID => ON but GO, CS_0 anyone => OFF
        FitcE84SignalError5               = 0x0138, // FITC E84 Signal error Wait VALID => ON but TR_REQ, BUSY, COMPT anyone => OFF
        FitcE84SignalError6               = 0x0139, // FITC E84 Signal error Wait TR_REQ => ON but GO, CS_0, VALID anyone => OFF
        FitcE84SignalError7               = 0x0140, // FITC E84 Signal error Wait TR_REQ => ON but BUSY, COMPT anyone => OFF
        FitcE84SignalError8               = 0x0141, // FITC E84 Signal error Wait BUSY => ON but GO, CS_0, VALID, TR_REQ anyone => OFF
        FitcE84SignalError9               = 0x0142, // FITC E84 Signal error Wait BUSY => ON but COMPT => ON
        FitcE84SignalError10              = 0x0143, // FITC E84 Signal error Wait BUSY => OFF, TR_REQ => OFF, COMPT ON but GO, CS_0, VALID anyone => OFF
        FitcE84SignalError11              = 0x0144, // FITC E84 Signal error Wait Valid => OFF, COMPT => OFF, CS_0 => OFF but GO => ON
        FitcE84SignalError12              = 0x0145, // FITC E84 Signal error Wait VALID => OFF, COMPT => OFF, CS_0 => OFF but TR_REQ, BUSY anyone => ON
        FitcE84SignalError13              = 0x0146, // FITC E84 Signal error Wait BUSY => ON, LOADING process, wait PS => ON but GO, VALID, CS_0, TR_REQ_ BUSY anyone => OFF
        FitcE84SignalError14              = 0x0147, // FITC E84 Signal error Wait BUSY => ON, LOADING process, Wait PS => ON but COMPT => ON
        FitcE84SignalError15              = 0x0148, // FITC E84 Signal error Wait BUSY => ON, LOADING process, wait PL => ON but GO, VALID, CS_0, TR_REQ, BUSY anyone => OFF
        FitcE84SignalError16              = 0x0149, // FITC E84 Signal error Wait BUSY => ON, LOADING process, Wait PL => ON but COMPT => ON
        FitcE84SignalError17              = 0x0150, // FITC E84 Signal error Wait BUSY => ON, UNLOADING process, wait PL => OFF but GO, VALID, CS_0, TR_REQ, BUSY anyone => OFF
        FitcE84SignalError18              = 0x0151, // FITC E84 Signal error Wait BUSY => ON, UNLOADING process, wait PL => COMPT => ON
        FitcE84SignalError19              = 0x0152, // FITC E84 Signal error Wait BUSY => ON, UNLOADING process, wait PS => OFF but GO, VALID, CS_0, TR_REQ, BUSY anyone OFF
        FitcE84SignalError20              = 0x0153, // FITC E84 Signal error Wait BUSY => ON, UNLOADING process, wait PL => OFF but COMPT => ON
        FitcE84FoupSensorError1           = 0x0154, // FITC E84 Foup Sensor Error Wait GO => ON but Presence Sensor (PS) or Placement Sensor (PL) Signal Error
        FitcE84FoupSensorError2           = 0x0155, // FITC E84 Foup Sensor Error Wait CS_0 => ON butPS or PL Signal Error
        FitcE84FoupSensorError3           = 0x0156, // FITC E84 Foup Sensor Error Wait Valid ON but PS or PL Signal Error
        FitcE84FoupSensorError4           = 0x0157, // FITC E84 Foup Sensor Error TA1 period but PS or PL Signal Error
        FitcE84FoupSensorError5           = 0x0158, // FITC E84 Foup Sensor Error Wait TR_REQ => ON but PS or PL Signal Error
        FitcE84FoupSensorError6           = 0x0159, // FITC E84 Foup Sensor Error TA2 period but PS or PL Signal Error
        FitcE84FoupSensorError7           = 0x0160, // FITC E84 Foup Sensor Error Wait BUSY => ON but PS or PL Signal Error
        FitcE84FoupSensorError8           = 0x0161, // FITC E84 Foup Sensor Error Loading process, wait PS => ON detected PS => OFF
        FitcE84FoupSensorError9           = 0x0162, // FITC E84 Foup Sensor Error Loading process, wait PL => ON, detected PS => OFF
        FitcE84FoupSensorError10          = 0x0163, // FITC E84 Foup Sensor Error Unloading process, wait PS => OFF detected PL => ON
        FitcE84FoupSensorError11          = 0x0164, // FITC E84 Foup Sensor Error Unloading process, wait PS => OFF detected PL => ON
        FitcE84FoupSensorError12          = 0x0165, // FITC E84 Foup Sensor Error Wait BUSY => OFF, PS or PL Signal Error
        FitcE84FoupSensorError13          = 0x0166, // FITC E84 Foup Sensor Error TA3 period => PS or PL signal Error
        FitcE84FoupSensorError14          = 0x0167, // FITC E84 Foup Sensor Error Wait TR_REQ => OFF but PS or PL SIgnal Error
        FitcE84FoupSensorError15          = 0x0168, // FITC E84 Foup Sensor Error Wait COMPT => ON but PS or PL Signal Error
        FitcE84FoupSensorError16          = 0x0169, // FITC E84 Foup Sensor Error Wait Valid => OFF but PS or PL Signal Error
        FitcE84FoupSensorError17          = 0x0170, // FITC E84 Foup Sensor Error Wait COMPT => OFF but PS or PL Signal Error
        FitcE84FoupSensorError18          = 0x0171, // FITC E84 Foup Sensor Error Wait CS_0 => OFF but PS or PL Signal Error
        FitcE84FoupSensorError19          = 0x0172, // FITC E84 Foup Sensor Error Wait GO => OFF but PS or PL Signal Error
        MclpError                         = 0x0204,
        MclpMovingError                   = 0x0205,
        MclpZaxisMovingError              = 0x0211,
        MclpZaxisUpperLimitError          = 0x0212,
        MclpZaxisUpperLowerError          = 0x0213,
        MclpZaxisDrivingError             = 0x0214,
        MclpZaxisHomingError              = 0x0215,
        MclpZaxisHomingFail               = 0x0216,
        MclpZaxisControllerError          = 0x0217,
        MclpX1axisMovingError             = 0x0221,
        MclpX1axisOriginalLimitError      = 0x0222,
        MclpX1axisterminalLimitError      = 0x0223,
        MclpX1axisDrivingLimitError       = 0x0224,
        MclpX1axisHomingError             = 0x0225,
        MclpX1axisHomingFail              = 0x0226,
        MclpX1axisControllerError         = 0x0227,
        MclpX2axisMovingError             = 0x0231,
        MclpX2axisOriginalLimitError      = 0x0232,
        MclpX2axisterminalLimitError      = 0x0233,
        MclpX2axisDrivingLimitError       = 0x0234,
        MclpX2axisHomingError             = 0x0235,
        MclpX2axisHomingFail              = 0x0236,
        MclpX2axisControllerError         = 0x0237,
        MclpDoorOpenError                 = 0x0268,
        MclpEmoError                      = 0x0269,
        UnknownMclpError                  = 0x0299,
        MclpCommandTimeoutError           = 0x0301,
        MclpCommandError                  = 0x0302,
        MclpCommandAbort                  = 0x0303,
        MclpManualModeError               = 0x0304,
        MclpControllerError               = 0x0305,
        mclpControllerParameterError      = 0x0306,
        mclpAbnormalCommandFormat         = 0x0307,
        mclpAbnormalCommandParameter      = 0x0308,
        MclpAbnormalDio                   = 0x0309,
        MclpNeedToReset                   = 0x0310,
        MclpNotAtHomePosition             = 0x0311,
        MclpTurnError                     = 0x0312,
        MclpAbnormalLoaderBackHome        = 0x0313,
        MclpPointTopointError             = 0x0314,
        MclpMappingError                  = 0x0315,
        MclpOriginSearchFail              = 0x0316,
        MclpServoOff                      = 0x0317,
        MclpAbnormalSpeedSetting          = 0x0318,
        MclpLoaderProjectionSensorError   = 0x0319,
        MclpElevatorProjectionSensorError = 0x0320,
        MclpLoaderLightOff                = 0x0321,
        MclpLoaderPresentSensorError      = 0x0322,
        MclpCassette1PresentError         = 0x0323,
        MclpCassette2PresentError         = 0x0324,
        MclpCassette3PresentError         = 0x0325,
        MclpCassette4PresentError         = 0x0326,
        MclpElevatorSentInPositionError   = 0x0327,
        MclpElevatorSentOutPositionError  = 0x0328,
        MclpRobotPassLinePositionError    = 0x0329,
        MclpNeedToInitialMclp             = 0x0331,
        MclpInterlockTrigger              = 0x0332,
        MclpCassetteProtrusionSensorError = 0x0333,
        MclpFullCassetteError             = 0x0334,
        AlignerDisable                    = 0x0501,
        AlignerError                      = 0x0502,
        WaferOnAligner                    = 0x0503,
        NoWaferOnAligner                  = 0x0504,
        AlignerMoving                     = 0x0505,
        OcrRecipeNotSet                   = 0x0506,
        NoThisCommand                     = 0xFF01,
        AbnormalFormatOfCommand           = 0xFF02,
        HaveNoEndCharacter                = 0xFF03,
        InvalidNumberOfParameters         = 0xFF04,
        AbnormalRangeOfParameter          = 0xFF05,
        UnitNotInitial                    = 0xFF06,
        Moving                            = 0xFF07,
        AbnormalParameterType             = 0xFF08,
        AbnormalSlotNumber                = 0xFF09,
        ReadySignalIsFalse                = 0xFF10,
        WaferStateError                   = 0xFF11,
        ErrorOccurredState                = 0xFF12,
        VacuumError                       = 0xFF13,
        AirError                          = 0xFF14,
        SystemIsNotFinishedInitializing   = 0xFF15,
        AbnormalParameter                 = 0xFF16,
        AbnormalReadOcrSettingValue       = 0xFF17,
        SafetyInterlockIsFalse            = 0xFF18,
        AoiDataError                      = 0xFF19,
        AoiSystemError                    = 0xFF20,
        FfuAbnormal                       = 0xFF21,
        AreaSensorAbnormal                = 0xFF22,
        InitSettingValueAbnormal          = 0xFF23,
        InMaintenanceMode                 = 0x0050
    }
}
