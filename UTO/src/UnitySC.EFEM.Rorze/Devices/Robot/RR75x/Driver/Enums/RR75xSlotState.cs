namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums
{
    public enum RR75xSlotState
    {
        WaferDoesNotExist = 0,
        WaferExists = 1,
        ThicknessAbnormal_ThickWafer = 2,
        CrossedWafer = 3,
        FrontBow = 4,
        SeveralWafersInSameSlot = 7,
        ThicknessAbnormal_ThinWafer = 8,
        MappingFailure = 9
    }
}
