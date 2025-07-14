using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums
{
    public enum RobotOriginSearchParameterN
    {
        [Description("Performs unconditional origin search. (Omitting parameter is allowed.) "
                     + "Origin search is performed without checking the wafer, and wafer retaining is cancelled in the case of \"flg=0\".")]
        Unconditional = 0,

        [Description("Checks the wafer, cancels wafer retaining, and then performs origin search. "
                     + "Wafer detection is performed before performing origin search, "
                     + "and if the wafer is detected, the SYSTEM stops due to the error after cancelling retaining of the wafer.")]
        CheckAndCancelWaferRetaining = 1,

        [Description("Performs origin search with the wafer being retained. "
                     + "Wafer detection is performed before performing origin search, "
                     + "and if the wafer exists, origin search is performed while retaining the wafer."
                     + "The wafer is kept being retained after origin search is completed.")]
        CheckAndRetainWafer = 2,

        [Description("Performs mechanical origin search."
                     + "If the error code \"000C\" occurs, execute this after moving the robot to a safe position "
                     + "in the maintenance mode or teaching pendant mode. "
                     + "Note: The designation of \"flg\" is invalid. ")]
        PerformMechanicalOriginSearch = 3
    }

    public enum RobotOriginSearchParameterFlg
    {
        [Description("Omit parameter when sending the command. Mandatory to use with \"flg=3\".")]
        NotSet = -1,

        [Description("Moves each axis to origin. (Omitting parameter is allowed.)")]
        MoveEachAxisToOrigin = 0,

        [Description(
            "Moves no axis. (The current position is kept. In the case of \"n = 0\", the wafer is kept being retained.) ")]
        MoveNoAxis
    }
}
