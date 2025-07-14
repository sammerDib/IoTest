using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    public class StatusAcquisitionCommand : RorzeCommand
    {
        #region Constructors

        public static StatusAcquisitionCommand NewOrder(
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new StatusAcquisitionCommand(RorzeConstants.CommandTypeAbb.Order, deviceType, deviceId, sender, eqFacade);
        }

        public static StatusAcquisitionCommand NewEvent(
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new StatusAcquisitionCommand(RorzeConstants.CommandTypeAbb.Event, deviceType, deviceId, sender, eqFacade);
        }

        private StatusAcquisitionCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(commandType, deviceType, deviceId, RorzeConstants.Commands.StatusAcquisition, sender, eqFacade)
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
                    eventArgs = new StatusEventArgs<RobotStatus>(
                        message.DeviceType + message.DeviceId,
                        new RobotStatus(message.Data));
                    break;

                case RorzeConstants.DeviceTypeAbb.Aligner:
                    eventArgs = new StatusEventArgs<AlignerStatus>(
                        message.DeviceType + message.DeviceId,
                        new AlignerStatus(message.Data));
                    break;

                case RorzeConstants.DeviceTypeAbb.IO:
                    eventArgs = new StatusEventArgs<IoStatus>(
                        message.DeviceType + message.DeviceId,
                        new IoStatus(message.Data));
                    break;

                default:
                    return false;
            }

            facade.SendEquipmentEvent((int)EFEMEvents.StatusReceived, eventArgs);

            CommandComplete();
            return true;
        }

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat status messages
            if (message.Name != RorzeConstants.Commands.StatusAcquisition)
            {
                return false;
            }

            System.EventArgs eventArgs;
            switch (message.DeviceType)
            {
                case RorzeConstants.DeviceTypeAbb.Robot:
                    eventArgs = new StatusEventArgs<RobotStatus>(
                        message.DeviceType + message.DeviceId,
                        new RobotStatus(message.Data));
                    break;

                case RorzeConstants.DeviceTypeAbb.Aligner:
                    eventArgs = new StatusEventArgs<AlignerStatus>(
                        message.DeviceType + message.DeviceId,
                        new AlignerStatus(message.Data));
                    break;

                case RorzeConstants.DeviceTypeAbb.IO:
                    eventArgs = new StatusEventArgs<IoStatus>(
                        message.DeviceType + message.DeviceId,
                        new IoStatus(message.Data));
                    break;

                default:
                    return false;
            }

            facade.SendEquipmentEvent((int)EFEMEvents.StatusReceived, eventArgs);

            return true;
        }

        #endregion RorzeCommand
    }
}
