using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.PostmanCommands
{
    public class InitializeStatusCommand : RV101Command
    {
        #region Constructors

        public static InitializeStatusCommand NewOrder(
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new InitializeStatusCommand(RorzeConstants.CommandTypeAbb.Order, deviceType, deviceId, sender, eqFacade);
        }

        private InitializeStatusCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(commandType, deviceType, deviceId, RorzeConstants.Commands.Initialize, sender, eqFacade)
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

            bool isDone;

            switch (message.DeviceType)
            {
                case RorzeConstants.DeviceTypeAbb.LoadPort:
                    isDone = new RV101LoadPortStatus(message.Data).OperationMode != RorzeLoadPort.Driver.Enums.OperationMode.Initializing;
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
