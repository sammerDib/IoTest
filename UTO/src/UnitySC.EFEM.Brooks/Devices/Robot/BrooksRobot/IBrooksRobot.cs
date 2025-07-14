using Agileo.EquipmentModeling;

using UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot.Conditions;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions;

namespace UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot
{
    // Implement interface IMaterialLocationContainer if the device has material locations
    [Device]
    public interface IBrooksRobot : IRobot
    {
        #region Status

        [Status]
        string MotionProfile { get; }

        #endregion

        #region Commands

        [Command(Category = "Movement", Timeout = 360)]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsNotBusy))]
        void GetMotionProfiles();

        [Command(Category = "Movement", Timeout = 360)]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsNotBusy))]
        [Pre(Type = typeof(IsMotionProfileValid))]
        void SetMotionProfile(string motionProfile);

        #endregion
    }
}
