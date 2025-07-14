using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums
{
    public enum ErrorCode
    {
        None = 0x00,

        // Significant Errors
        MotorStall                             = 0x01,
        SensorAbnormal                         = 0x02,
        EmergencyStop                          = 0x03,
        CommandError                           = 0x04,
        CommunicationError                     = 0x05,
        ChuckingSensorAbnormal                 = 0x06,
        Reserved1                              = 0x07,
        ObstacleDetectionSensorError           = 0x08,
        SecondOriginSensorAbnormal             = 0x09,
        MappingSensorAbnormal                  = 0x0A,
        WaferProtrusionDetectingSensorAbnormal = 0x0B,
        DriverAbnormal                         = 0x0E,
        ControlPowerAbnormal                   = 0x20,
        DrivePowerAbnormal                     = 0x21,
        EEPROMAbnormal                         = 0x22,
        ZSearchError                           = 0x23,
        Overheat                               = 0x24,
        OverCurrent                            = 0x25,
        MotorCableAbnormal                     = 0x26,
        MotorStallPositionDeviation            = 0x27,
        MotorStallTimeOver                     = 0x28,

        // Light errors
        ExhaustFanAbnormal               = 0x89,
        FOUPClampRotationDisabled        = 0x92,
        FOUPUnclampRotationDisabled      = 0x93,
        LatchKeyLockDisabled             = 0x94,
        LatchKeyReleaseDisabled          = 0x96,
        MappingSensorPreparationDisabled = 0x97,
        MappingSensorContainingDisabled  = 0x98,
        ChuckingOnDisabled               = 0x99,
        WaferProtrusion                  = 0x9A,
        NoDoorOnFOUP_WithDoorOnFOSB      = 0x9B,
        CarrierImproperlyTaken           = 0x9C,
        FOSBDoorDetection                = 0x9D,
        CarrierImproperlyPlaced          = 0x9E,
        CarrierDetectionError            = 0x9F,
        CoverLockDisabled                = 0xA0,
        CoverUnlockDisabled              = 0xA1,
        TR_REQ_Timeout                   = 0xB0,
        BUSY_ON_Timeout                  = 0xB1,
        CarrierCarryInTimeout            = 0xB2,
        CarrierCarryOutTimeout           = 0xB3,
        BUSY_OFF_Timeout                 = 0xB4,
        Reserved2                        = 0xB5,
        VALID_OFF_Timeout                = 0xB6,
        CONTINUE_Timeout                 = 0xB7,

        [Description("Signal abnormal detected from VALID, CS_0 = ON to TR_REQ=ON")]
        SigAbDetFr_VALID_CS0toTRREQ = 0xB8,

        [Description("Signal abnormal detected from TR_REQ = ON to BUSY = ON")]
        SigAbDetFr_TRREQtoBUSY = 0xB9,

        [Description("Signal abnormal detected from BUSY = ON to Placement = ON")]
        SigAbDetFr_BUSYtoPlacement = 0xBA,

        [Description("Signal abnormal detected from Placement = ON to COMPLETE = ON")]
        SigAbDetFr_PlacementToCOMPLETE = 0xBB,

        [Description("Signal abnormal detected from COMPLETE = ON to VALID = OFF")]
        SigAbDetFr_COMPLETEtoVALIDoff = 0xBC,

        [Description("VALID,CS_0 signal abnormal")]
        VALID_CS0_SignalAbnormal = 0xBF
    }
}
