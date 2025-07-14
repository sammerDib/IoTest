namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums
{
    public enum ExtendId
    {
        WaferBottomSurfaceAtLoadPosition = 1,
        PositionWaferLiftedAtLoadPosition = 2,
        WaferBottomSurfaceAtUnloadPosition = 3,
        PositionWaferLiftedAtUnloadPosition = 4,
        PositionWhereTheExternalSensorIsChecked1 = 5,
        PositionWhereTheExternalSensorIsChecked2 = 6,
    }

    public enum ExtendFlag
    {
        PerformsArmOrigin = 0,
        MoveWithoutOrigin = 1,
        MoveOnlyZAxis = 2
    }
}
