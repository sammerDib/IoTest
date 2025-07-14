using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums;
using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.PostmanCommands
{
    public class ReleaseCarrierCommand : RE201Command
    {
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
            if (Command.CommandType == RorzeConstants.CommandTypeAbb.Order && isDone)
            {
                facade.SendEquipmentEvent(
                    (int)EFEMEvents.ReleaseCarrierCompleted,
                    System.EventArgs.Empty);
                CommandComplete();
                return true;
            }

            return false;
        }

        #endregion RorzeCommand

        #region Constructors

        #region Public

        public static ReleaseCarrierCommand NewOrder(
            byte deviceId,
            ReleaseCarrierOperationMode selectedMode,
            ReleaseCarrierCloseOperation closeOperation,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            //When n=0 & id=0 then parameters can be omitted
            if (selectedMode == ReleaseCarrierOperationMode.ClosesTheCarrier
                && closeOperation == ReleaseCarrierCloseOperation.ClosesTheCarrierToUnclamp)
            {
                return new ReleaseCarrierCommand(
                    RorzeConstants.CommandTypeAbb.Order,
                    deviceId,
                    sender,
                    eqFacade);
            }

            var parameters = new List<string> { ((int)selectedMode).ToString("X1") };

            if (selectedMode == ReleaseCarrierOperationMode.ClosesTheCarrier)
            {
                //If close carrier mode is required, close operation must be specified
                parameters.Add(((int)closeOperation).ToString("X1"));
            }

            return new ReleaseCarrierCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                parameters.ToArray());
        }

        #endregion Public

        #region Private

        private ReleaseCarrierCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.LoadPort,
                deviceId,
                RorzeConstants.Commands.ReleaseCarrier,
                sender,
                eqFacade,
                commandParameters)
        {
        }

        #endregion Private

        #endregion Constructors
    }
}
