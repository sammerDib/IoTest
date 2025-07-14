using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Helpers;
using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.PostmanCommands
{
    public class SetCarrierTypeCommand : RV101Command
    {
        #region Constructors

        public static SetCarrierTypeCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            CarrierType carrierType)
        {
            return new SetCarrierTypeCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade, Converters.ToCarrierTypeCmdParam(carrierType));
        }

        public static SetCarrierTypeCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            string carrierType)
        {
            return new SetCarrierTypeCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                carrierType);
        }

        private SetCarrierTypeCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(commandType, RorzeConstants.DeviceTypeAbb.LoadPort, deviceId, RorzeConstants.Commands.SetCarrierType, sender, eqFacade, commandParameters)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            facade.SendEquipmentEvent((int)EFEMEvents.SetCarrierTypeCompleted, System.EventArgs.Empty);
            CommandComplete();
            return true;
        }

        #endregion RorzeCommand
    }
}
