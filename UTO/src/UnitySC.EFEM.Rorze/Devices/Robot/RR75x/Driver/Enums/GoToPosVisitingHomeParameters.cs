namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums
{
    public enum MoveId
    {
        WaferBottomSurface         = 1,
        PositionWhereWaferIsLifted = 2
    }

    public enum Arm_Interpolated
    {
        UpperArm                 = 1,
        LowerArm                 = 2,
        UpperArm_NoInterpolation = 0xA1,
        LowerArm_NoInterpolation = 0xA2
    }
}
