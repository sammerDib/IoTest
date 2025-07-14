namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums
{
    /// <summary>
    /// Define the slot states according to it's content for the RV101 device.
    /// </summary>
    public enum RV101SlotState
    {
        WaferDoesNotExist            = 0,
        WaferExists                  = 1,
        ThicknessAbnormal_ThickWafer = 2,
        CrossedWafer                 = 3,
        FrontBow                     = 4,
        SeveralWafersInSameSlot      = 7,
        ThicknessAbnormal_ThinWafer  = 8
    }
}
