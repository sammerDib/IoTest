namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums
{
    public enum ErrorCode
    {
        None = 0x00,

        // Significant Errors
        MotorStall                             = 0x01,
        Limit                                  = 0x02,
        CommandError                           = 0x04,
        CommunicationError                     = 0x05,
        DriverAbnormal                         = 0x0E,
        ControlPowerAbnormal                   = 0x20,
        DrivePowerAbnormal                     = 0x21,
        Overheat                               = 0x24,
        OverCurrent                            = 0x25,
        MotorCableAbnormal                     = 0x26,
        OriginSearchError                      = 0x29,
        ErrorCenterAdjustmentStallSensor       = 0x2A,

        // Light errors
        WaferProtrusion                  = 0x9A,
        CarrierImproperlyTaken           = 0x9C,
        CarrierDetectionError            = 0x9F,
        ShutterOpenTimeout               = 0xA0,
        ShutterCloseTimeout              = 0xA1
    }
}
