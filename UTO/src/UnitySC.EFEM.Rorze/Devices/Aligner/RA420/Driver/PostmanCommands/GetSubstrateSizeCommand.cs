using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.PostmanCommands
{
    /// <summary>
    /// Gets the target substrate type of the Aligner.
    /// </summary>
    public class GetSubstrateSizeCommand : RorzeCommand
    {
        #region Constructors

        public static GetSubstrateSizeCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetSubstrateSizeCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade);
        }

        private GetSubstrateSizeCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(commandType, RorzeConstants.DeviceTypeAbb.Aligner, deviceId, RorzeConstants.Commands.GetSubstrateSize, sender, eqFacade)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            var args = new StatusEventArgs<AlignerSubstrateSizeStatus>(
                message.DeviceType + message.DeviceId,
                new AlignerSubstrateSizeStatus(message.Data));
            
            facade.SendEquipmentEvent((int)EFEMEvents.SubstrateSizeReceived, args);

            CommandComplete();
            return true;
        }

        #endregion RorzeCommand
    }
}
