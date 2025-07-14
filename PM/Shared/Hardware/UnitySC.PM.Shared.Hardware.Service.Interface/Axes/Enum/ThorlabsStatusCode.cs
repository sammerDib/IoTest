namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Thorlabs
{
    public enum StatusCode : int
    {
        NoError = 0,
        CommunicationTimeOut,
        MechanicalTimeOut,
        CommandError,
        ValueOutOfRange,
        ModuleIsolated,
        ModuleOutOfIsolation,
        InitializingError,
        ThermalError,
        Busy,
        SensorError,
        MotorError,
        OutOfRange,
        OverCurrentError,
        Reserved
    }
}
