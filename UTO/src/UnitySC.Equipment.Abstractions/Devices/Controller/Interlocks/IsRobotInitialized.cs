using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.Efem;
using UnitySC.Equipment.Abstractions.Devices.Efem.Resources;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.Robot;

namespace UnitySC.Equipment.Abstractions.Devices.Controller
{
    public class IsRobotInitialized : CSharpInterlockBehavior
    {
        public override void Check(Device responsible, CommandContext context)
        {
            // Step 1 => Get Robot device
            var robot = responsible.TryGetDevice<Robot.Robot>();
            if (robot == null)
            {
                // Sometimes it's normal to not have a robot
                // (e.g. aligner is responsible for OCR)
                if (responsible is IEfem)
                {
                    context.AddContextError(Messages.ChildRobotNotFound);
                }

                return;
            }

            // Step 2 => Check the current command
            bool isSafeCommand;
            var commandName = context.Command.Name;
            switch (context.Device)
            {
                case Robot.Robot:
                    isSafeCommand = /*commandName.Equals(nameof(IRobot.GetStatuses))
                                    ||*/ commandName.Equals(nameof(IRobot.SetMotionSpeed))
                                    || commandName.Equals(nameof(IRobot.SetDateAndTime))
                                    || commandName.Equals(nameof(IRobot.StartCommunication))
                                    || commandName.Equals(nameof(IRobot.StopCommunication))
                                    || commandName.Equals(nameof(IRobot.Initialize));
                    break;

                case LoadPort.LoadPort:
                    isSafeCommand = /*commandName.Equals(nameof(ILoadPort.GetStatuses))
                                    ||*/ commandName.Equals(nameof(ILoadPort.SetDateAndTime))
                                    || commandName.Equals(nameof(ILoadPort.StartCommunication))
                                    || commandName.Equals(nameof(ILoadPort.StopCommunication))
                                    || commandName.Equals(nameof(ILoadPort.EnableE84))
                                    || commandName.Equals(nameof(ILoadPort.DisableE84))
                                    || commandName.Equals(nameof(ILoadPort.SetLight));
                    break;

                case Aligner.Aligner:
                    isSafeCommand = /*commandName.Equals(nameof(IAligner.GetStatuses))
                                    ||*/ commandName.Equals(nameof(IAligner.SetDateAndTime))
                                    || commandName.Equals(nameof(IAligner.StartCommunication))
                                    || commandName.Equals(nameof(IAligner.StopCommunication));
                    break;

                default:
                    // Other devices (I/Os, substrate Id reader...) that have no interactions with the robot
                    return;
            }

            // Step 3 => Check condition
            if (!robot.HasBeenInitialized && !isSafeCommand)
            {
                context.AddContextError(Messages.RobotNotInitialized);
            }
        }
    }
}
