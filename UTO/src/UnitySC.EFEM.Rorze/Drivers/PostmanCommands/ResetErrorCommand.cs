using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers.Enums;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    public class ResetErrorCommand : RorzeCommand
    {
        private readonly bool _mustSendEquipmentEvent;

        #region Constructors

        public static ResetErrorCommand NewOrder(
            string deviceType,
            byte deviceId,
            ResetErrorParameter parameter,
            bool mustSendEquipmentEvent,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new ResetErrorCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceType,
                deviceId,
                mustSendEquipmentEvent,
                sender,
                eqFacade,
                ((int)parameter).ToString());
        }

        private ResetErrorCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            bool mustSendEquipmentEvent,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(commandType, deviceType, deviceId, RorzeConstants.Commands.ResetError, sender, eqFacade, commandParameters)
        {
            _mustSendEquipmentEvent = mustSendEquipmentEvent;
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            if (message.DeviceType.Equals(RorzeConstants.DeviceTypeAbb.LoadPort))
            {
                // We do not wait a status change for the LoadPort according to the RV201 documentation.
                if (_mustSendEquipmentEvent)
                    facade.SendEquipmentEvent((int)EFEMEvents.ResetErrorCompleted, System.EventArgs.Empty);

                CommandComplete();
            }

            return true;
        }

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat status messages
            if (message.Name != RorzeConstants.Commands.StatusAcquisition
                || Command.CommandType != RorzeConstants.CommandTypeAbb.Order)
            {
                return false;
            }

            // Command is done when hardware has stopped processing 
            bool isDone;

            switch (message.DeviceType)
            {
                case RorzeConstants.DeviceTypeAbb.Robot:
                    var robotStatuses = new RobotStatus(message.Data);
                    isDone            = robotStatuses.CommandProcessing == CommandProcessing.Stop;
                    break;

                case RorzeConstants.DeviceTypeAbb.Aligner:
                    var alignerStatuses = new AlignerStatus(message.Data);
                    isDone              = alignerStatuses.CommandProcessing == CommandProcessing.Stop;
                    break;

                // When the device is a LoadPort, we do not wait any status event before completing the command
                // (according to RV201 documentation).
                default:
                    return false;
            }

            // When order is done, command is complete
            if (isDone)
            {
                if (_mustSendEquipmentEvent)
                    facade.SendEquipmentEvent((int)EFEMEvents.ResetErrorCompleted, System.EventArgs.Empty);

                CommandComplete();
                return true;
            }

            return false;
        }

        #endregion RorzeCommand
    }
}
