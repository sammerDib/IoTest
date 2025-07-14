using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.PostmanCommands
{
    public class GpioCommand : RE201Command
    {
        #region Constructors

        public static GpioCommand NewOrder(
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GpioCommand(RorzeConstants.CommandTypeAbb.Order, deviceType, deviceId, sender, eqFacade);
        }

        public static GpioCommand NewEvent(
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GpioCommand(RorzeConstants.CommandTypeAbb.Event, deviceType, deviceId, sender, eqFacade);
        }

        protected GpioCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(commandType, deviceType, deviceId, RorzeConstants.Commands.GpioAcquisition, sender, eqFacade)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            System.EventArgs eventArgs;

            switch (message.DeviceType)
            {
                case RorzeConstants.DeviceTypeAbb.LoadPort:
                    eventArgs = new StatusEventArgs<RE201GpioStatus>(
                        message.DeviceType + message.DeviceId,
                        new RE201GpioStatus(message.Data));
                    break;

                default:
                    return false;
            }

            facade.SendEquipmentEvent((int)EFEMEvents.GpioEventReceived, eventArgs);

            CommandComplete();
            return true;
        }

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat GPIO messages
            if (message.Name != RorzeConstants.Commands.GpioAcquisition)
            {
                return false;
            }

            System.EventArgs eventArgs;
            switch (message.DeviceType)
            {
                case RorzeConstants.DeviceTypeAbb.LoadPort:
                    eventArgs = new StatusEventArgs<RE201GpioStatus>(
                        message.DeviceType + message.DeviceId,
                        new RE201GpioStatus(message.Data));
                    break;

                default:
                    return false;
            }

            facade.SendEquipmentEvent((int)EFEMEvents.GpioEventReceived, eventArgs);

            return true;
        }

        #endregion RorzeCommand
    }
}
