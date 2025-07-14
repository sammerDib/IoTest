using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.PostmanCommands
{
    public class OriginSearchCommand : RV101Command
    {
        #region Constructors


        public static OriginSearchCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new OriginSearchCommand(RorzeConstants.CommandTypeAbb.Order, RorzeConstants.DeviceTypeAbb.LoadPort, deviceId, sender, eqFacade);
        }

        private OriginSearchCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] parameters)
            : base(commandType, deviceType, deviceId, RorzeConstants.Commands.OriginSearch, sender, eqFacade, parameters)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat status messages
            if (message.Name != RorzeConstants.Commands.StatusAcquisition)
            {
                return false;
            }

            // Command is done when hardware has stopped moving
            bool isDone;

            switch (message.DeviceType)
            {
                case RorzeConstants.DeviceTypeAbb.LoadPort:
                    isDone = new RV101LoadPortStatus(message.Data).OperationStatus == RorzeLoadPort.Driver.Enums.OperationStatus.Stop;
                    break;

                default:
                    return false;
            }

            // When order is done, command is complete
            if (Command.CommandType == RorzeConstants.CommandTypeAbb.Order
                && isDone)
            {
                CommandComplete();
                return true;
            }

            return false;
        }

        #endregion RorzeCommand
    }
}
