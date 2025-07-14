using System;
using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.PostmanCommands
{
    public class RetainWaferCommand : RorzeCommand
    {
        private const int MapParameterLength = 5;
        private const int MaxCheckingTime = 60000;
        private readonly bool _mustSendEquipmentEvent;

        #region Constructors

        /// <summary>Creates a new instance of <see cref="RetainWaferCommand" /> class.</summary>
        /// <param name="deviceId">The id of the device.</param>
        /// <param name="retainingOption">The option that provide details about what is to be done. Mandatory.</param>
        /// <param name="sender">Instance that would send the command.</param>
        /// <param name="eqFacade">Instance that would send event to equipment, if necessary.</param>
        /// <param name="mustSendEquipmentEvent">
        /// Indicates if command should send an event to equipment when
        /// ended.
        /// </param>
        /// <param name="map">
        /// Upper arm chucking pattern (Omitting is allowed.). It must have 5 characters.
        /// </param>
        /// <param name="t">
        /// Checking time (3 sec when "t=0"; Omitting is allowed.). Value is valid until
        /// <see cref="MapParameterLength" /> ms.
        /// </param>
        public static RetainWaferCommand NewOrder(
            byte deviceId,
            RetainingOption retainingOption,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            bool mustSendEquipmentEvent,
            string map = null,
            uint? t = null)
        {
            var parameters = new List<string>(3) { ((int)retainingOption).ToString() };

            if (map != null)
            {
                if (map.Length != MapParameterLength)
                {
                    throw new InvalidOperationException(
                        $"{nameof(RetainWaferCommand)} - The given map length is not valid.\n"
                        + $"{map} contains {map.Length} characters. {MapParameterLength} were expected.");
                }

                parameters.Add(map);
            }

            if (t != null)
            {
                if (t > MaxCheckingTime)
                {
                    throw new InvalidOperationException(
                        $"{nameof(RetainWaferCommand)} - The given checking time is too long: {t} > {MaxCheckingTime}");
                }

                parameters.Add(((uint)t).ToString());
            }

            return new RetainWaferCommand(
                mustSendEquipmentEvent,
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                sender,
                eqFacade,
                parameters.ToArray());
        }

        private RetainWaferCommand(
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
                RorzeConstants.Commands.RetainWafer,
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
                        (int)EFEMEvents.RetainWaferCompleted,
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
