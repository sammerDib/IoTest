namespace UnitySC.EFEM.Controller.HostInterface.Enums
{
    /*
     * From existing RORZE protocol, different commands define different values for same information
     * This is why we define two kind of "SubstrateType" enum
     */

    // ReSharper disable once InconsistentNaming
    public enum SubstrateTypeGREC
    {
        NormalWafer = 0,
        Frame       = 1
    }

    // ReSharper disable once InconsistentNaming
    public enum SubstrateTypeRDID
    {
        NormalWafer = 1,
        Frame       = 2
    }
}
