using System;
using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.PostmanCommands
{
    public class SetMotionSpeedCommand : RorzeCommand
    {
        private readonly bool _mustSendEquipmentEvent;

        /// <summary>
        /// Represent 100% of normal motion speed when in <see cref="Enums.OperationMode.Remote" /> mode, or
        /// 200% of normal motion speed when in <see cref="Enums.OperationMode.Maintenance" />,
        /// <see cref="Enums.OperationMode.Recovery" /> and <see cref="Enums.OperationMode.Teaching" /> modes
        /// </summary>
        private const uint MaxDifferentialValue = 20;

        #region Constructors

        /// <summary>Creates a new instance of <see cref="SetMotionSpeedCommand" /> class.</summary>
        /// <param name="deviceId">The id of the device.</param>
        /// <param name="sender">Instance that would send the command.</param>
        /// <param name="eqFacade">Instance that would send event to equipment, if necessary.</param>
        /// <param name="mustSendEquipmentEvent">
        /// Indicate if an event should be sent to the equipment when the command is done.
        /// </param>
        /// <param name="constantSpeedAccordingModes">
        /// Set whether or not the given speed percentage must be constant according to the current
        /// <see cref="Enums.OperationMode" />.
        /// </param>
        /// <param name="speedAsPercentage">
        /// Give the percentage of normal speed that must be sent to the robot. When
        /// <paramref name="constantSpeedAccordingModes" /> is true, range of validity is from 1 to 100% by
        /// increments of 1%.
        /// <remarks>0% means 100%.</remarks>
        /// When <paramref name="constantSpeedAccordingModes" /> is false, range of validity is from 5 to 100%
        /// by increments of 5%.
        /// <remarks>
        /// 0% means 100%. If the given percentage is not a multiple of 5, it would be rounded the the nearest
        /// inferior valid value. When robot is in <see cref="Enums.OperationMode.Remote" /> mode, physical
        /// robot speed would be the given percentage. When robot is in
        /// <see cref="Enums.OperationMode.Maintenance" />, <see cref="Enums.OperationMode.Recovery" /> or
        /// <see cref="Enums.OperationMode.Teaching" /> modes, physical robot speed would be the double of the
        /// given percentage (up to 200%).
        /// </remarks>
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// when <paramref name="speedAsPercentage" /> is above
        /// 100%.
        /// </exception>
        public static SetMotionSpeedCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            bool mustSendEquipmentEvent,
            bool constantSpeedAccordingModes = true,
            uint speedAsPercentage = 0)
        {
            uint n;

            if (constantSpeedAccordingModes || speedAsPercentage == 0)
            {
                n = 0;
            }
            else
            {
                // n must not be 0 for percent values going from 1 to 4
                if (1 <= speedAsPercentage && speedAsPercentage <= 4)
                {
                    n = 1;
                }

                // Sub-sampling on speedAsPercentage values to be compatible with device command
                else
                {
                    n = speedAsPercentage * MaxDifferentialValue / 100;
                }
            }

            if (speedAsPercentage > 100)
            {
                throw new ArgumentOutOfRangeException(
                    $"{nameof(SetMotionSpeedCommand)} - speedAsPercentage value must not exceed 100. Given value = {speedAsPercentage}.");
            }

            var parameters = new List<string>(2) { n.ToString() };

            if (constantSpeedAccordingModes)
            {
                parameters.Add(speedAsPercentage.ToString());
            }

            return new SetMotionSpeedCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                mustSendEquipmentEvent,
                parameters.ToArray());
        }

        private SetMotionSpeedCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            bool mustSendEquipmentEvent,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.Robot,
                deviceId,
                RorzeConstants.Commands.SetMotionSpeed,
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

            // Command is done when hardware has stopped processing command
            var isDone = statuses.CommandProcessing == CommandProcessing.Stop;

            // When order is done, command is complete
            if (Command.CommandType == RorzeConstants.CommandTypeAbb.Order && isDone)
            {
                if (_mustSendEquipmentEvent)
                {
                    facade.SendEquipmentEvent(
                        (int)EFEMEvents.SetMotionSpeedCompleted,
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
