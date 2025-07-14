using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.PostmanCommands
{
    /// <summary>
    /// Responds the detection status of the substrate on the Aligner.
    /// <remarks>
    /// The substrate is judged as " substrate exists" in the case that the substrate is chucked by the
    /// spindle or in the case that the alignment sensor senses the substrate.
    /// </remarks>
    /// </summary>
    public class GetSubstratePresenceCommand : RorzeCommand
    {
        #region Constructors

        public static GetSubstratePresenceCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetSubstratePresenceCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade);
        }

        public static GetSubstratePresenceCommand NewEvent(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetSubstratePresenceCommand(
                RorzeConstants.CommandTypeAbb.Event,
                deviceId,
                sender,
                eqFacade);
        }

        private GetSubstratePresenceCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Aligner,
                deviceId,
                RorzeConstants.Commands.GetSubstratePresence,
                sender,
                eqFacade)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            var args = new StatusEventArgs<AlignerSubstratePresenceStatus>(
                message.DeviceType + message.DeviceId,
                new AlignerSubstratePresenceStatus(message.Data));

            facade.SendEquipmentEvent((int)EFEMEvents.SubstratePresenceReceived, args);

            CommandComplete();
            return true;
        }

        protected override bool TreatEvent(RorzeMessage message)
        {
            if (message.Name != RorzeConstants.Commands.GetSubstratePresence)
            {
                return false;
            }

            var args = new StatusEventArgs<AlignerSubstratePresenceStatus>(
                message.DeviceType + message.DeviceId,
                new AlignerSubstratePresenceStatus(message.Data));

            facade.SendEquipmentEvent((int)EFEMEvents.SubstratePresenceReceived, args);

            return true;
        }

        #endregion RorzeCommand
    }
}
