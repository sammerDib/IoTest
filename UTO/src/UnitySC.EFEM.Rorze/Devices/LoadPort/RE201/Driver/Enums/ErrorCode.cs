namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums
{
    public enum ErrorCode
    {
        None = 0x00,

        // Significant Errors
        MotorStall = 0x01,
        Limit = 0x02,
        CommandError = 0x04,
        CommunicationError = 0x05,
        ObstacleDetected = 0x08,
        DriverAbnormal = 0x0E,
        ControlPowerAbnormal = 0x20,
        DrivePowerAbnormal = 0x21,
        EEPROMAbnormal = 0x22,
        ZSearchError = 0x23,
        Overheat = 0x24,
        OverCurrent = 0x25,
        MotorWireAbnormal = 0x26,
        MotorStallPositionDeviation = 0x27,
        MotorStallOverTimeLimit = 0x28,

        // Light errors
        PodClampDisabled = 0x92,
        PodUnclampDisabled = 0x93,
        WorkProtusionDetection = 0x9A,
        IncorrectTakenCarrier = 0x9C
    }
}
