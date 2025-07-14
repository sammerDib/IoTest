using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums;
using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.PostmanCommands
{
    public class HomeCommand : RE201Command
    {
        #region Constructors

        public static HomeCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            uint carrierTypeIndex,
            byte slot)
        {
            var parameters = new List<string>();
            parameters.Add(carrierTypeIndex.ToString());
            parameters.Add(slot.ToString());

            return new HomeCommand(RorzeConstants.CommandTypeAbb.Order, RorzeConstants.DeviceTypeAbb.LoadPort, deviceId,
                sender, eqFacade, parameters.ToArray());
        }

        private HomeCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] parameters)
        : base(commandType, deviceType, deviceId, RorzeConstants.Commands.GoToHome, sender, eqFacade, parameters)
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

            var statuses = new RE201LoadPortStatus(message.Data);

            // Command is done when hardware has stopped moving
            var isDone = statuses.OperationStatus == OperationStatus.Stop;

            // When order is done, command is complete
            if (Command.CommandType == RorzeConstants.CommandTypeAbb.Order
                && isDone)
            {
                facade.SendEquipmentEvent((int)EFEMEvents.GoToHomeCompleted, System.EventArgs.Empty);
                CommandComplete();
                return true;
            }

            return false;
        }

        #endregion
    }
}
