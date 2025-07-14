using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers.Enums;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    public class StopCommand : RorzeCommand
    {
        #region Constructors

        public static StopCommand NewOrder(
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade facade)
        {
            return new StopCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceType,
                deviceId,
                sender,
                facade);
        }

        private StopCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade facade)
            : base(
                commandType,
                deviceType,
                deviceId,
                RorzeConstants.Commands.StopMotion,
                sender,
                facade)
        {
        }

        #endregion

        #region RorzeCommand

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat status messages
            if (message.Name != RorzeConstants.Commands.StatusAcquisition)
            {
                return false;
            }

            bool isDone;

            switch (message.DeviceType)
            {
                //TODO Handle robot and aligner case
                case RorzeConstants.DeviceTypeAbb.Robot:
                    isDone = new RobotStatus(message.Data).CommandProcessing
                             == CommandProcessing.Stop
                             && new RobotStatus(message.Data).OperationStatus
                             == OperationStatus.Stop;
                    break;

                case RorzeConstants.DeviceTypeAbb.Aligner:
                    isDone =
                        new AlignerStatus(message.Data).CommandProcessing == CommandProcessing.Stop
                        && new AlignerStatus(message.Data).OperationStatus
                        == Devices.Aligner.RA420.Driver.Enums.OperationStatus.Stop;
                    break;

                case RorzeConstants.DeviceTypeAbb.LoadPort:
                    isDone =
                        new RV201LoadPortStatus(message.Data).CommandProcessing
                        == CommandProcessing.Stop
                        && new RV201LoadPortStatus(message.Data).OperationStatus
                        == Devices.LoadPort.RorzeLoadPort.Driver.Enums.OperationStatus.Stop;
                    break;

                default:
                    return false;
            }

            // When order is done, command is complete
            if (Command.CommandType == RorzeConstants.CommandTypeAbb.Order && isDone)
            {
                CommandComplete();
                return true;
            }

            return false;
        }

        #endregion
    }
}
