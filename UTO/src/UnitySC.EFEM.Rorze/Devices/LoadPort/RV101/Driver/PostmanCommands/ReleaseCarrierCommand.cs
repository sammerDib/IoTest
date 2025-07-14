using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.PostmanCommands
{
    public class ReleaseCarrierCommand : RV101Command
    {
        #region Constructors

        #region Public

        public static ReleaseCarrierCommand NewOrder(
            byte deviceId,
            ReleaseCarrierOperationMode selectedMode,
            ReleaseCarrierOption unclampOption,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            List<string> parameters = new List<string>(4) { ((int)selectedMode).ToString("X1") };

            // "NotSet" parameter means that the parameter has not to be sent to the real device (not constant number of parameters).
            if (unclampOption == ReleaseCarrierOption.NotSet)
                return new ReleaseCarrierCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade, parameters.ToArray());

            parameters.Add(((int)unclampOption).ToString("X1"));

            return new ReleaseCarrierCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade, parameters.ToArray());
        }

        #endregion Public

        #region Private

        private ReleaseCarrierCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(commandType, RorzeConstants.DeviceTypeAbb.LoadPort, deviceId, RorzeConstants.Commands.ReleaseCarrier,
                sender, eqFacade, commandParameters)
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

            var statuses = new RV101LoadPortStatus(message.Data);

            // Command is done when hardware has stopped moving
            var isDone = statuses.OperationStatus == OperationStatus.Stop;

            // When order is done, command is complete
            if (Command.CommandType == RorzeConstants.CommandTypeAbb.Order
                && isDone)
            {
                facade.SendEquipmentEvent((int)EFEMEvents.ReleaseCarrierCompleted, System.EventArgs.Empty);
                CommandComplete();
                return true;
            }

            return false;
        }

        #endregion RorzeCommand
    }
}
