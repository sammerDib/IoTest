namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums
{
    public enum GetCarrierTypeParameter
    {
        AcquireIdentificationMethodAndCarrierType = 0, // must be omitted when sending the command
        GetIdentificationMethodOnly               = 1,
        GetRealCarrierTypeOnly                    = 2,
        GetInfoPadInput                           = 3
    }
}
