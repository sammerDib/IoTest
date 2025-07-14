using System.Collections.Generic;
using System.Globalization;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.Efem.Resources;
using UnitySC.Equipment.Abstractions.Devices.Robot;

namespace UnitySC.Equipment.Abstractions.Devices.Controller
{
    public class IsLocationServedByRobot : CSharpInterlockBehavior
    {
        public override void Check(Device responsible, CommandContext context)
        {
            // This interlock checks that source/destination device selected for command can be served by robot
            // Interlocks are checked for any command, so "not a robot" can be nominal case (don't add error)
            if (context.Device is not Robot.Robot)
            {
                return;
            }

            List<object> devices = new List<object>();
            switch (context.Command.Name)
            {
                case nameof(IRobot.Pick):
                    devices.Add(context.GetArgument("sourceDevice"));
                    break;

                case nameof(IRobot.Place):
                case nameof(IRobot.GoToLocation):
                case nameof(IRobot.GoToSpecifiedLocation):
                    devices.Add(context.GetArgument("destinationDevice"));
                    break;

                default:
                    // Interlocks are checked for any command, so "command not supported" can be nominal case (don't add error)
                    return;
            }

            // Check that all devices of a command are served by the robot
            // (overkill now, but could be useful in case of Transfer command that takes a source and destination)
            foreach (object device in devices)
            {
                switch (device)
                {
                    case Aligner.Aligner _:
                    case LoadPort.LoadPort _:
                    case ProcessModule.ProcessModule _:
                        break;

                    default:
                        context.AddContextError(string.Format(
                            CultureInfo.InvariantCulture,
                            Messages.DeviceNotServedByRobot,
                            (device as Device)?.Name ?? device.GetType().Name));
                        break;
                }
            }
        }
    }
}
