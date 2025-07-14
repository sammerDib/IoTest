using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums;
using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.PostmanCommands
{
    public class SecureCarrierCommand : RV101Command
    {
        #region Constructors

        #region Public

        public static SecureCarrierCommand NewOrder(
            byte deviceId,
            SecureCarrierOperationCharacteristicParameter selectedMode,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            List<string> parameters = new List<string>(2);

            parameters.Add(((int)selectedMode).ToString("X1"));

            return new SecureCarrierCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade, parameters.ToArray());
        }

        #endregion Public

        #region Private

        private SecureCarrierCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(commandType, RorzeConstants.DeviceTypeAbb.LoadPort, deviceId, RorzeConstants.Commands.SecureCarrier, sender, eqFacade, commandParameters)
        {
        }

        #endregion Private

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat status messages
            if (message.Name != RorzeConstants.Commands.StatusAcquisition)
            {
                return false;
            }

            var statuses = new LoadPortStatus(message.Data);

            // Command is done when hardware has stopped moving
            var isDone = statuses.OperationStatus == OperationStatus.Stop;

            // When order is done, command is complete
            if (Command.CommandType == RorzeConstants.CommandTypeAbb.Order
                && isDone)
            {
                facade.SendEquipmentEvent((int)EFEMEvents.SecureCarrierCompleted, System.EventArgs.Empty);
                CommandComplete();
                return true;
            }

            return false;
        }

        #endregion RorzeCommand
    }
}
