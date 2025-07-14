namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public enum Bool { False = 0, True = 1 }
    public enum OperationResult { Fail = (int)Bool.False, Success = (int)Bool.True }

    public enum CommandLevel { Normal = 0, Admin = 1 }

    // ON = closed-loop ; OFF = open-loop
    public enum ServoMode { Off = (int)Bool.False, On = (int)Bool.True }

    public enum TriggerInState { Off = Bool.False, On = Bool.True }

    public enum TriggerInUsage
    {
        Off = 0,
        WaveGenerator = 1
    }

    public enum TriggerInParam
    {
        TriggerType = 1,
        Polarity = 7
    }

    public enum WaveGeneratorStartingMode
    {
        Off = 0,
        StartNow = 1,
        StartWithDigitalTriggerIn = 2,
        StartNowCumulative = 257,
        StartWithDigitalTriggerInCumulative = 258
    }

    public enum WaveParam { WavePointsCount = 1 }

    public enum TriggerOutParam
    {
        TriggerStep = 1,
        Axis = 2,
        TriggerMode = 3,
        MinThreshold = 4,
        MaxThreshold = 5,
        Polarity = 7,
        StartThreshold = 8,
        StopThreshold = 9
    }

    public enum TriggerInType
    {
        Edge = 0,
        Level = 1
    }

    public enum TriggerInSignal
    {
        Low = 0,
        High = 1
    }

    public enum TriggerOutMode
    {
        PositionDistance = 0,
        OnTarget = 2,
        MinMaxThreshold = 3,
        GeneratorTrigger = 4,
        InMotion = 6
    }

    public enum CommandMode
    {
        Digital = 0,
        Analog = 2
    }

    public enum TriggerPolarity { ActiveWhenLow = 0, ActiveWhenHigh = 1 }

    public static class PIE709Param
    {
        public const uint TriggerInUsage = 0x15000800;
        public const uint WaveTablesCount = 0x1300010A;
        public const uint CommandModeParameter = 0x06000500;
    }
}
