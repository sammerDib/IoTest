using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Statuses;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.PostmanCommands
{
    public class GetGeneralInputOutputCommand : RorzeCommand
    {
        public static GetGeneralInputOutputCommand NewEvent(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetGeneralInputOutputCommand(RorzeConstants.CommandTypeAbb.Event, deviceId, sender, eqFacade);
        }

        private GetGeneralInputOutputCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.IO,
                deviceId,
                RorzeConstants.Commands.GpioAcquisition,
                sender,
                eqFacade)
        {
        }

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat GPIO messages
            if (message.Name != RorzeConstants.Commands.GpioAcquisition)
            {
                return false;
            }

            var eventArgs = new StatusEventArgs<RC550GeneralIoStatus>(
                message.DeviceType + message.DeviceId,
                new RC550GeneralIoStatus(message.Data));

            facade.SendEquipmentEvent((int)EFEMEvents.GpioEventReceived, eventArgs);

            return true;
        }
    }
}
