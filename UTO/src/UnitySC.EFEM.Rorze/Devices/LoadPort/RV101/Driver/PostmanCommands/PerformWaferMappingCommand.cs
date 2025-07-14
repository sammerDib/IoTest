using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.PostmanCommands
{
    public class PerformWaferMappingCommand : RV101Command
    {
        #region Constructors

        public static PerformWaferMappingCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new PerformWaferMappingCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade);
        }

        private PerformWaferMappingCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(commandType, RorzeConstants.DeviceTypeAbb.LoadPort, deviceId, RorzeConstants.Commands.PerformWaferMapping, sender, eqFacade)
        {
        }

        #endregion Constructor

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            facade.SendEquipmentEvent((int)EFEMEvents.PerformWaferMappingCompleted, System.EventArgs.Empty);
            CommandComplete();
            return true;
        }

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat status messages
            if (message.Name != RorzeConstants.Commands.StatusAcquisition)
            {
                return false;
            }

            var statuses = new RV101LoadPortStatus(message.Data);

            // Command is done when hardware has stopped moving
            var isDone = statuses.OperationStatus == OperationStatus.Stop;

            // When order is done, command is complete
            if (Command.CommandType == RorzeConstants.CommandTypeAbb.Order
                && isDone)
            {
                facade.SendEquipmentEvent((int)EFEMEvents.PerformWaferMappingCompleted, System.EventArgs.Empty);
                CommandComplete();
                return true;
            }

            return false;
        }

        #endregion RorzeCommand
    }
}
