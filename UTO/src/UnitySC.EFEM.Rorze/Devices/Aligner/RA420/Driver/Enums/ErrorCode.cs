namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums
{
    public enum ErrorCode
    {
        None = 0x00,

        // Fatal Errors
        MotorStall                           = 0x01,
        Limit                                = 0x02,
        PositionError                        = 0x03,
        CommandError                         = 0x04,
        CommunicationError                   = 0x05,
        AbnormalSensor                       = 0x06,
        DriverEMSError                       = 0x07,
        SubstrateDroppedError                = 0x08,
        AbnormalDriver                       = 0x0E,
        AbnormalDrivePower                   = 0x0F,
        AbnormalControlPower                 = 0x10,
        AbnormalDriverTemperature            = 0x13,
        DriverFPGAError                      = 0x14,
        MotorWireBroken                      = 0x15,
        MotorOverLoad                        = 0x16,
        MotorMotionStartError                = 0x17,
        AbnormalAlignmentSensor              = 0x18,
        AbnormalExhaustFANState              = 0x19,
        AlignmentSensorDetectsObstacle_Fatal = 0x1A,
        InternalError_AbnormalDeviceDriver   = 0x40,
        InternalError_AbnormalDriverControl  = 0x41,
        InternalError_TaskStartFailed        = 0x42,
        ReadingSettingDataFailed             = 0x45,

        // Light errors
        OriginSearchFailed                   = 0x83,
        ChuckingError                        = 0x84,
        NotchDetectionError                  = 0x90,
        AlignmentSensorDetectsObstacle_Light = 0x91,
        RetryOver_AlignmentFailed            = 0x92
    }
}
