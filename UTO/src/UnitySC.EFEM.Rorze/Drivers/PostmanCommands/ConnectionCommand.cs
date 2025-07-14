using Agileo.Common.Communication;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    public class ConnectionCommand : RorzeCommand
    {
        public ConnectionCommand(
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(RorzeConstants.CommandTypeAbb.Event, deviceType, deviceId, RorzeConstants.Commands.Connection, sender, eqFacade)
        {
        }

        protected override bool TreatEvent(RorzeMessage message)
        {
            if (!message.Name.Equals(Command.Name))
            {
                return false;
            }

            facade.SendEquipmentEvent((int)EFEMEvents.ConnectedReceived, System.EventArgs.Empty);
            CommandComplete();
            return true;
        }
    }
}
