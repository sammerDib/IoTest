using System.Collections.Generic;

using Agileo.EquipmentModeling;

using UnitsNet;

namespace UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot
{
    public partial class BrooksRobot
    {
        protected virtual void InternalSimulateGetMotionProfiles(Tempomat tempomat)
        {
            MotionProfiles = new List<string>() {"Full","High","Medium","Low","Home", "Jog", "Auto Teach"};
        }

        protected virtual void InternalSimulateSetMotionProfile(string motionProfile, Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromMilliseconds(1500));
            MotionProfile = motionProfile;
        }
    }
}
