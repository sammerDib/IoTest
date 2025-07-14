using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers.Enums;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    public class PauseCommand : RorzeCommand
    {
        #region Constructors

        public static PauseCommand NewOrder(
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade facade)
        {
            return new PauseCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceType,
                deviceId,
                sender,
                facade);
        }

        private PauseCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade facade)
            : base(
                commandType,
                deviceType,
                deviceId,
                RorzeConstants.Commands.PauseMotion,
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
                case RorzeConstants.DeviceTypeAbb.Robot:
                    isDone =
                        new RobotStatus(message.Data).CommandProcessing == CommandProcessing.Stop
                        && new RobotStatus(message.Data).OperationStatus is OperationStatus.Pause or OperationStatus.Stop;
                    break;

                case RorzeConstants.DeviceTypeAbb.Aligner:
                    isDone =
                        new AlignerStatus(message.Data).CommandProcessing == CommandProcessing.Stop
                        && new AlignerStatus(message.Data).OperationStatus is Devices.Aligner.RA420.Driver.Enums.OperationStatus.Pause or Devices.Aligner.RA420.Driver.Enums.OperationStatus.Stop;
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
