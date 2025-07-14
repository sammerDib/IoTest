using System;

using Agileo.SemiDefinitions;

namespace UnitySC.EFEM.Controller.HostInterface.Converters
{
    public static class RobotArmConverter
    {
        public static RobotArm ToRobotArm(Constants.Arm arm)
        {
            switch (arm)
            {
                case Constants.Arm.Upper:
                    return RobotArm.Arm1;

                case Constants.Arm.Lower:
                    return RobotArm.Arm2;

                default:
                    throw new ArgumentOutOfRangeException(nameof(arm), arm, null);
            }
        }

        public static Constants.Arm ToRobotArm(RobotArm arm)
        {
            switch (arm)
            {
                case RobotArm.Arm1:
                    return Constants.Arm.Upper;

                case RobotArm.Arm2:
                    return Constants.Arm.Lower;

                default:
                    throw new ArgumentOutOfRangeException(nameof(arm), arm, null);
            }
        }
    }
}
