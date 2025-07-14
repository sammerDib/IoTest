namespace UnitySC.Shared.Data.Enum
{
    public enum PowerIlluminationFlow
    {
        Unknown,
        HS,
        HT
    }

    public enum BeamShaperFlow
    {
        Attenuation,
        Polarisation
    }

    public enum WaveplateAnglesPolarisation
    {
        S_Polar,
        P_Polar
    }

    public enum MppcCollector
    {
        Unknown,
        WIDE,
        NARROW
    }

    public enum MppcVoltageStabilityStatus
    {
        UNSTABLE,
        STABLE
    }

    public enum MppcStateModule
    {
        UNRECOVERABLE_ERROR,
        AWAITING_COOLING_TEMP_SATBLE,
        AWAITING_HIGH_VOLTAGE_POWER_SUPPLY_STABLE,
        MODULE_OPERATING_NORMAL
    }

    public enum ECalibrationType { Auto, Custom }

    public enum ServoPosition
    {
        Pos0,
        Pos1,
        Pos2,
        Pos3
    }
}
