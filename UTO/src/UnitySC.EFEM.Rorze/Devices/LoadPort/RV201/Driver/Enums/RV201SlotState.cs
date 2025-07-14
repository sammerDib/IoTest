namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums
{
    /// <summary>
    /// Define the slot states according to it's content for the RV201 device.
    /// </summary>
    public enum RV201SlotState
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
