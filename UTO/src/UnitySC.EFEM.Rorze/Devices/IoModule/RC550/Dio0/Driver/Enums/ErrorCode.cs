namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Enums
{
    public enum ErrorCode
    {
        NoError                                      = 0x00,
        ProcessingTimeout                            = 0x01,
        MultiDropCommunicationAbnormal               = 0x02,
        EmergencyStop                                = 0x03,
        InternalSystemError                          = 0x04,
        CommunicationError                           = 0x05,
        FPGAError                                    = 0x0D,
        SettingDataReadingError                      = 0x45,
        FunctionCodeError                            = 0x50,
        ImproperRegisterNumberError                  = 0x51,
        ImproperNumberError                          = 0x52,
        DataSettingError                             = 0x53,
        WritingModeError                             = 0x54,
        WritingErrorWhileMainCircuitVoltageIsLowered = 0x55
    }
}
