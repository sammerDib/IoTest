using Agileo.Drivers;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Drivers.Common;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver
{
    public class RobotCommand : DriverCommand
    {
        public static RobotCommand Initialization { get; } = new(1, nameof(RobotCommands.Initialization));
        public static RobotCommand SetMotionSpeed { get; } = new(2, nameof(SetMotionSpeedCommand));
        public static RobotCommand Clamp { get; } = new(3, nameof(RetainWaferCommand));
        public static RobotCommand UnClamp { get; } = new(4, nameof(ReleaseWaferRetentionCommand));
        public static RobotCommand GoToHome { get; } = new(5, nameof(GoToPosVisitingHomeCommand));
        public static RobotCommand GoToLocation { get; } = new(6, nameof(GoToPosVisitingHomeCommand));
        public static RobotCommand Pick { get; } = new(7, nameof(LoadWaferCommand));
        public static RobotCommand Place { get; } = new(8, nameof(UnloadWaferCommand));
        public static RobotCommand SetDateAndTime { get; } = new(9, nameof(SetDateAndTimeCommand));
        public static RobotCommand GetStatuses { get; } = new(10, nameof(GetStatuses));
        public static RobotCommand InitializeCommunication { get; } = new(11, nameof(InitializeCommunication));
        public static RobotCommand Extend { get; } = new(12, nameof(ExtendRobotArmCommand));
        public static RobotCommand Transfer { get; } = new(13, nameof(TransferWaferCommand));
        public static RobotCommand Swap { get; } = new(14, nameof(ExchangeWaferCommand));
        public static RobotCommand GetLastMapping { get; } = new(15, nameof(MappingPatternAcquisitionCommand));
        public static RobotCommand Map { get; } = new(16, nameof(PerformWaferMappingCommand));
        public static RobotCommand GetVersion { get; } = new(17, nameof(VersionAcquisitionCommand));

        public RobotCommand(ushort id, string name) : base(id, name)
        {
        }
    }
}
