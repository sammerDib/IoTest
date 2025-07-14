using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.PostmanCommands
{
    public class HomeCommand : RorzeCommand
    {
        #region Constructors

        public static HomeCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ZAxisMovement zAxisMovement = ZAxisMovement.NoZAxisMove)
        {
            return new HomeCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade, ((int)zAxisMovement).ToString());
        }

        private HomeCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(commandType, RorzeConstants.DeviceTypeAbb.Aligner, deviceId, RorzeConstants.Commands.GoToHome, sender, eqFacade, commandParameters)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat status messages and only when command is an order.
            if (message.Name != RorzeConstants.Commands.StatusAcquisition
                || Command.CommandType != RorzeConstants.CommandTypeAbb.Order)
            {
                return false;
            }

            var statuses = new AlignerStatus(message.Data);

            // Command is done when hardware has stopped moving 
            var isDone = statuses.OperationStatus == OperationStatus.Stop;

            // When order is done, command is complete. Otherwise, command was not addressed explicitly to the current object.
            if (!isDone)
                return false;

            CommandComplete();
            return true;
        }

        #endregion RorzeCommand
    }
}
