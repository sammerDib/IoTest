using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    public class GposCommand : RorzeCommand
    {
        #region Constructors

        public static GposCommand NewOrder(
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GposCommand(RorzeConstants.CommandTypeAbb.Order, deviceType, deviceId, sender, eqFacade);
        }

        public static GposCommand NewEvent(
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GposCommand(RorzeConstants.CommandTypeAbb.Event, deviceType, deviceId, sender, eqFacade);
        }

        protected GposCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
            : base(commandType, deviceType, deviceId, RorzeConstants.SubCommands.PositionAcquisition, sender, eqFacade)
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
                    eventArgs = new StatusEventArgs<RobotGposStatus>(
                        message.DeviceType + message.DeviceId,
                        new RobotGposStatus(message.Data));
                    break;

                case RorzeConstants.DeviceTypeAbb.Aligner:
                    eventArgs = new StatusEventArgs<AlignerGposStatus>(
                        message.DeviceType + message.DeviceId,
                        new AlignerGposStatus(message.Data));
                    break;

                default:
                    return false;
            }

            facade.SendEquipmentEvent((int)EFEMEvents.GposEventReceived, eventArgs);

            CommandComplete();
            return true;
        }

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat GPOS messages
            if (message.Name != RorzeConstants.SubCommands.PositionAcquisition)
            {
                return false;
            }

            System.EventArgs eventArgs;
            switch (message.DeviceType)
            {
                case RorzeConstants.DeviceTypeAbb.Robot:
                    eventArgs = new StatusEventArgs<RobotGposStatus>(
                        message.DeviceType + message.DeviceId,
                        new RobotGposStatus(message.Data));
                    break;

                case RorzeConstants.DeviceTypeAbb.Aligner:
                    eventArgs = new StatusEventArgs<AlignerGposStatus>(
                        message.DeviceType + message.DeviceId,
                        new AlignerGposStatus(message.Data));
                    break;

                default:
                    return false;
            }

            facade.SendEquipmentEvent((int)EFEMEvents.GposEventReceived, eventArgs);

            return true;
        }

        #endregion RorzeCommand
    }
}
