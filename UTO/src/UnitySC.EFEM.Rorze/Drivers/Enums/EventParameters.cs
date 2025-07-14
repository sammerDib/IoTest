namespace UnitySC.EFEM.Rorze.Drivers.Enums
{
    public enum EventTargetParameter
    {
        AllEvents             = 0,
        StatusEvent           = 1,
        PioEvent              = 2,
        StoppingPositionEvent = 3,
        CarrierTypeEvent      = 4,

        // Used only for developer:
        // It is easier for him to conceptualize that he wants to enable/disable a SubstrateIdEvent than a CarrierTypeEvent.
        // According to the RV201, RE201, RR757 and RA420, this parameter value activates the same event "GWID".
        SubstrateIdEvent      = 4
    }

    public enum EventEnableParameter
    {
        Disable = 0,
        Enable  = 1
    }
}
