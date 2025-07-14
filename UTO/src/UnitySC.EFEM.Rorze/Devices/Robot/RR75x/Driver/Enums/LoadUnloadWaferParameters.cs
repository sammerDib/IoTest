using System;
using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums
{
    public enum ArmOrBoth_Interpolated
    {
        UpperArm                          = 1,
        LowerArm                          = 2,
        UpperAndLowerArms                 = 3,
        UpperArm_NoInterpolation          = 0xA1,
        LowerArm_NoInterpolation          = 0xA2,
        UpperAndLowerArms_NoInterpolation = 0xA3
    }

    public enum MotionType
    {
        NormalMotion = 0,

        [Description("Stops just after picking up or placing the wafer (the arm being extended).")]
        StopOnWaferLocationChanged = 1,

        TwoStepsOperation = 2
    }

    public enum ExchangeMotionType
    {
        NormalMotion = 0,

        [Description("Stops while the unload arm is being extended (wafer is retained).")]
        StopsWhileUnloadArmExtended = 1,

        [Description("Stops after moving to the wafer take-up position while the unload arm is being extended (wafer is retained).")]
        StopsAfterMovingWaferTakeUpPositionWhileUnloadArmExtended = 2,

        [Description("Stops after cancelling retaining of the wafer while the unload arm is being extended and moving the Z-Axis to the wafer bottom surface height.")]
        StopsAfterCancellingRetainingOfWaferWhileUnloadArmExtendedAndMovingZAxisToWaferBottom = 3,

        [Description("Stops after lifting the wafer using the load arm (operation can be continued by EXCC).")]
        StopsAfterLiftingTheWaferWithLoadArm = 4
    }

    /// <summary>
    /// Possible values for (un)LoadWafer command check option parameters are:
    ///     0 : Presence is not checked by the external sensor. (Omitting is allowed.)
    ///     1 : Checks presence by the external sensor before (un)loading.
    ///         (Presence: Input ON)
    ///     2 : Checks presence by the external sensor after (un)loading.
    ///         (Presence: Input ON)
    ///     3 : Checks presence by the external sensor before/after (un)loading.
    ///         (Presence: Input ON)
    ///     5 : Checks presence by the external sensor before (un)loading.
    ///         (Presence: Input OFF)
    ///     6 : Checks presence by the external sensor after (un)loading.
    ///         (Presence: Input OFF)
    ///     7 : Checks presence by the external sensor before/after (un)loading.
    ///         (Presence: Input OFF)
    /// </summary>
    [Flags]
    public enum CheckOption
    {
        NoCheck                            = 0b000,
        CheckBeforeWaferEmplacementChanged = 0b001,
        CheckAfterWaferEmplacementChanged  = 0b010,
        CheckWithPresenceInputOff          = 0b100
    }
}
