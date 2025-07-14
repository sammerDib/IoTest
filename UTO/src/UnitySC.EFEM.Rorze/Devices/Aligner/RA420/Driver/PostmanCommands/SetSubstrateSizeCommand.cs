using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.PostmanCommands
{
    /// <summary>
    /// Sets the target substrate type of the Aligner.
    /// </summary>
    public class SetSubstrateSizeCommand : RorzeCommand
    {
        #region Constructors

        public static SetSubstrateSizeCommand NewOrder(
            byte deviceId,
            byte substrateSizeId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new SetSubstrateSizeCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade, substrateSizeId.ToString());
        }

        private SetSubstrateSizeCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(commandType, RorzeConstants.DeviceTypeAbb.Aligner, deviceId, RorzeConstants.Commands.SetSubstrateSize, sender, eqFacade, commandParameters)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            // Command name and type are already checked in RorzeCommand.
            // SetSubstrateSize does not wait an event to know it's completion.
            // That is why command is done when receiving ack.
            CommandComplete();
            return true;
        }

        #endregion RorzeCommand
    }
}
