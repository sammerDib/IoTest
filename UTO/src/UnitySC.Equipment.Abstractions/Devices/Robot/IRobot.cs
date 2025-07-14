using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.Robot.Conditions;
using UnitySC.Equipment.Abstractions.Devices.Robot.Enums;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice;
using UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.Conditions;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Conditions;

namespace UnitySC.Equipment.Abstractions.Devices.Robot
{
    [Device(IsAbstract = true)]
    public interface IRobot : IUnityCommunicatingDevice, IMaterialLocationContainer
    {
        #region Statuses

        [Status(Documentation = "Upper arm wafer dimension")]
        SampleDimension UpperArmWaferDimension { get; }

        [Status(Documentation = "Lower arm wafer dimension")]
        SampleDimension LowerArmWaferDimension { get; }

        [Status(Documentation = "Upper arm simplified wafer ID")]
        string UpperArmSimplifiedWaferId { get; }

        [Status(Documentation = "Upper arm simplified wafer ID")]
        string LowerArmSimplifiedWaferId { get; }

        [Status(Documentation = "Upper arm wafer status")]
        WaferStatus UpperArmWaferStatus { get; }

        [Status(Documentation = "Lower arm wafer status")]
        WaferStatus LowerArmWaferStatus { get; }

        [Status(Documentation = "Upper arm wafer presence")]
        WaferPresence UpperArmWaferPresence { get; }

        [Status(Documentation = "Lower arm wafer presence")]
        WaferPresence LowerArmWaferPresence { get; }

        [Status(Documentation = "Upper arm substrate detection error")]
        bool UpperArmSubstrateDetectionError { get; }

        [Status(Documentation = "Lower arm substrate detection error")]
        bool LowerArmSubstrateDetectionError { get; }

        [Status(Documentation = "Upper arm clamp state")]
        bool UpperArmClamped { get; }

        [Status(Documentation = "Lower arm clamp state")]
        bool LowerArmClamped { get; }

        [Status]
        ArmState UpperArmState { get; }

        [Status]
        ArmState LowerArmState { get; }

        [Status]
        TransferLocation Position { get; }

        [Status]
        bool HasBeenInitialized { get; }

        [Status]
        [Unit(RatioUnit.Percent)]
        Ratio Speed { get; }

        #endregion Statuses

        #region Commands

        [Command(Category = "Movement", Timeout=360)]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        void GoToHome();

        [Command(Category = "Movement", Timeout = 360)]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        void GoToLocation(IMaterialLocationContainer destinationDevice);

        [Command(Category = "Movement", Timeout = 360)]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsArmEnabled))]
        void GoToTransferLocation(TransferLocation location, RobotArm arm, byte slot);

        [Command(Category = "Movement", Timeout = 360)]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsArmEnabled))]
        void GoToSpecifiedLocation(
            IMaterialLocationContainer destinationDevice,
            byte destinationSlot,
            RobotArm arm,
            bool isPickUpPosition);

        [Command(Category = "Movement", Timeout = 360)]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsArmEnabled))]
        [Pre(Type = typeof(IsArmReady))]
        [Pre(Type = typeof(IsArmEffectorValid))]
        void Pick(RobotArm arm, IMaterialLocationContainer sourceDevice, byte sourceSlot);

        [Command(Category = "Movement", Timeout = 360)]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsArmEnabled))]
        [Pre(Type = typeof(IsArmReady))]
        [Pre(Type = typeof(IsArmEffectorValid))]
        void Place(RobotArm arm, IMaterialLocationContainer destinationDevice, byte destinationSlot);

        [Command(Category = "Movement", Timeout = 360)]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsArmEnabled))]
        [Pre(Type = typeof(IsArmReady))]
        [Pre(Type = typeof(IsArmEffectorValid))]
        void Transfer(
            RobotArm pickArm,
            IMaterialLocationContainer sourceDevice,
            byte sourceSlot,
            RobotArm placeArm,
            IMaterialLocationContainer destinationDevice,
            byte destinationSlot);

        [Command(Category = "Movement", Timeout = 360)]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsArmEnabled))]
        [Pre(Type = typeof(IsArmReady))]
        [Pre(Type = typeof(IsArmEffectorValid))]
        void Swap(RobotArm pickArm, IMaterialLocationContainer sourceDevice, byte sourceSlot);

        [Command(Category = "Movement", Timeout = 360)]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsArmEnabled))]
        void ExtendArm(RobotArm arm, TransferLocation location, byte slot);

        [Command(Documentation = "Secure material on arm so robot can move without loosing it.")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsArmEnabled))]
        void Clamp(RobotArm arm);

        [Command(Documentation = "Release material on arm so it can be removed by hand.")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsIdle))]
        [Pre(Type = typeof(IsArmEnabled))]
        void Unclamp(RobotArm arm);

        [Command]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsNotBusy))]
        [Pre(Type = typeof(IsSpeedValid))]
        void SetMotionSpeed([Unit(RatioUnit.Percent)] Ratio percentage);

        [Command(Documentation = "Send current date and time to hardware in order to synchronize logs.")]
        [Pre(Type = typeof(IsCommunicating))]
        [Pre(Type = typeof(IsNotBusy))]
        void SetDateAndTime();

        #endregion Commands

        #region Public

        public void CheckSubstrateDetectionError(RobotArm arm, bool reset = false);

        #endregion
    }
}
