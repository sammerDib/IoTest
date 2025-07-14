using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.PostmanCommands
{
    public class ReleaseWaferRetentionCommand : RorzeCommand
    {
        private readonly bool _mustSendEquipmentEvent;

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="ReleaseWaferRetentionCommand" /> class.
        /// </summary>
        /// <param name="deviceId">The id of the device.</param>
        /// <param name="releaseWaferRetentionParameter">Provide details about what is to be done. Mandatory.</param>
        /// <param name="sender">Instance that would send the command.</param>
        /// <param name="eqFacade">Instance that would send event to equipment, if necessary.</param>
        /// <param name="mustSendEquipmentEvent">
        /// Indicates if command should send an event to equipment when
        /// ended.
        /// </param>
        public static ReleaseWaferRetentionCommand NewOrder(
            byte deviceId,
            ReleaseWaferRetentionParameter releaseWaferRetentionParameter,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            bool mustSendEquipmentEvent)
        {
            return new ReleaseWaferRetentionCommand(
                mustSendEquipmentEvent,
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                ((int)releaseWaferRetentionParameter).ToString());
        }

        private ReleaseWaferRetentionCommand(
            bool mustSendEquipmentEvent,
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Robot,
                deviceId,
                RorzeConstants.Commands.ReleaseWaferRetention,
                sender,
                eqFacade,
                commandParameters)
        {
            _mustSendEquipmentEvent = mustSendEquipmentEvent;
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatEvent(RorzeMessage message)
        {
            // We only treat status messages
            if (message.Name != RorzeConstants.Commands.StatusAcquisition)
            {
                return false;
            }

            var statuses = new RobotStatus(message.Data);

            // Command is done when hardware has stopped moving 
            var isDone = statuses.OperationStatus == OperationStatus.Stop;

            // When order is done, command is complete
            if (Command.CommandType == RorzeConstants.CommandTypeAbb.Order && isDone)
            {
                // We want to send RetainWaferCompleted to equipment only after performing the command action, not when only testing.
                if (_mustSendEquipmentEvent)
                {
                    facade.SendEquipmentEvent(
                        (int)EFEMEvents.ReleaseWaferRetentionCompleted,
                        System.EventArgs.Empty);
                }

                CommandComplete();
                return true;
            }

            return false;
        }

        #endregion RorzeCommand
    }
}
