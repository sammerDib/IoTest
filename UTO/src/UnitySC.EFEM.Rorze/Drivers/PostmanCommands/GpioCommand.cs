using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Statuses;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    /// <summary>
    /// Acquires the input/output signal status.
    /// </summary>
    public class GpioCommand : RorzeCommand
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
                case RorzeConstants.DeviceTypeAbb.Robot:
                    eventArgs = new StatusEventArgs<RobotGpioStatus>(
                        message.DeviceType + message.DeviceId,
                        new RobotGpioStatus(message.Data));
                    break;

                case RorzeConstants.DeviceTypeAbb.Aligner:
                    eventArgs = new StatusEventArgs<AlignerGpioStatus>(
                        message.DeviceType + message.DeviceId,
                        new AlignerGpioStatus(message.Data));
                    break;

                case RorzeConstants.DeviceTypeAbb.IO:
                    eventArgs = new StatusEventArgs<RC550GeneralIoStatus>(
                        message.DeviceType + message.DeviceId,
                        new RC550GeneralIoStatus(message.Data));
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
                case RorzeConstants.DeviceTypeAbb.Robot:
                    eventArgs = new StatusEventArgs<RobotGpioStatus>(
                        message.DeviceType + message.DeviceId,
                        new RobotGpioStatus(message.Data));
                    break;

                case RorzeConstants.DeviceTypeAbb.Aligner:
                    eventArgs = new StatusEventArgs<AlignerGpioStatus>(
                        message.DeviceType + message.DeviceId,
                        new AlignerGpioStatus(message.Data));
                    break;

                case RorzeConstants.DeviceTypeAbb.IO:
                    eventArgs = new StatusEventArgs<RC550GeneralIoStatus>(
                        message.DeviceType + message.DeviceId,
                        new RC550GeneralIoStatus(message.Data));
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
