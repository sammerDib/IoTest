using System;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.Efem.Resources;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.ProcessModule;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Devices.Robot.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.Equipment.Abstractions.Devices.Controller
{
    public class IsLocationAccessedByRobot : CSharpInterlockBehavior
    {
        public override void Check(Device responsible, CommandContext context)
        {
            var robot = context.Device.GetTopDeviceContainer().AllOfType<Robot.Robot>().First();

            if (robot.State == OperatingModes.Idle || robot.CurrentCommandContext == null)
            {
                //No robot command in progress
                if ((robot.UpperArmState == ArmState.Extended ||
                     robot.LowerArmState == ArmState.Extended)
                    && IsLocationAccessed(context.Device, robot.Position))
                {
                    context.AddContextError(Messages.LocationAccessByRobot);
                }
                return;
            }

            if (robot.CurrentCommandContext.Command.Name != nameof(IRobot.Pick)
                && robot.CurrentCommandContext.Command.Name != nameof(IRobot.Place))
            {
                //Robot is not accessing any location
                return;
            }

            var isPick = robot.CurrentCommandContext.Command.Name.Equals(nameof(IRobot.Pick), StringComparison.Ordinal);
            var arg = robot.CurrentCommandContext.GetArgument(isPick ? "sourceDevice" : "destinationDevice");

            if (IsLocationAccessed(context.Device, arg))
            {
                context.AddContextError(Messages.LocationAccessByRobot);
            }
        }

        private bool IsLocationAccessed(Device device, TransferLocation location)
        {
            switch (device)
            {
                case Aligner.Aligner aligner:
                    return aligner.InstanceId == ((int)location - 200);
                case LoadPort.LoadPort loadPort:
                    return loadPort.InstanceId == (int)location;
                case ProcessModule.ProcessModule processModule:
                    return processModule.InstanceId == ((int)location - 300);
                default:
                    return false;
            }
        }

        private bool IsLocationAccessed(Device device, object arg)
        {
            switch (device)
            {
                case Aligner.Aligner when arg is IAligner:
                case LoadPort.LoadPort loadPort when arg is ILoadPort location && location.Name == loadPort.Name:
                case ProcessModule.ProcessModule processModule
                    when arg is IProcessModule pmLocation
                         && pmLocation.Name == processModule.Name:
                    return true;
                default:
                    return false;
            }
        }
    }
}
