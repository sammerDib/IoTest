using System;
using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Enums;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.Enums;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.PostmanCommands
{
    public class RC550EventCommand : RorzeCommand
    {
        #region FIelds

        private readonly EventEnableParameter _enableParameter;

        private readonly LinkedList<RC550EventTargetParameter> _remainingEventsToReceive = new();

        #endregion FIelds

        #region Constructors

        public static RC550EventCommand NewOrder(
            byte deviceId,
            RC550EventTargetParameter selectedEvent,
            EventEnableParameter enable,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new RC550EventCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade, selectedEvent, enable);
        }

        private RC550EventCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            RC550EventTargetParameter selectedEvent,
            EventEnableParameter enable)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.IO,
                deviceId,
                RorzeConstants.Commands.Event,
                sender,
                eqFacade,
                ((int)selectedEvent).ToString(),
                ((int)enable).ToString())
        {
            _enableParameter = enable;

            switch (selectedEvent)
            {
                case RC550EventTargetParameter.AllEvents:
                case RC550EventTargetParameter.StatusEvent:
                    _remainingEventsToReceive.AddLast(RC550EventTargetParameter.StatusEvent);
                    break;

                case RC550EventTargetParameter.PioEvent:
                    _remainingEventsToReceive.AddLast(RC550EventTargetParameter.PioEvent);
                    break;

                case RC550EventTargetParameter.ExpansionIoSignalEvent:
                    _remainingEventsToReceive.AddLast(RC550EventTargetParameter.ExpansionIoSignalEvent);
                    break;

                case RC550EventTargetParameter.FanRotationalSpeedEvent:
                    _remainingEventsToReceive.AddLast(RC550EventTargetParameter.FanRotationalSpeedEvent);
                    break;

                case RC550EventTargetParameter.PressureSensorValueEvent:
                    _remainingEventsToReceive.AddLast(RC550EventTargetParameter.PressureSensorValueEvent);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(selectedEvent), selectedEvent, null);
            }
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            switch (_enableParameter)
            {
                case EventEnableParameter.Disable:
                    // Events disabled, we don't expect an event message so command is done
                    CommandComplete();
                    return true;

                case EventEnableParameter.Enable:
                    // Events enabled; the enabled event always occurs once immediately after it is enabled
                    // so command will be complete once we receive an event
                    return true;

                default:
                    return false;
            }
        }

        protected override bool TreatEvent(RorzeMessage message)
        {
            // Current instance is of order type (i.e. in a IMacroCommandSubscriber that send messages)
            // Command is done if we received an event that correspond to the one we enabled in the order
            if (Command.CommandType != RorzeConstants.CommandTypeAbb.Order
                || _enableParameter != EventEnableParameter.Enable)
            {
                return false;
            }

            // Check if command was waiting the event that just arrived. If not, event is not to be treated.
            RC550EventTargetParameter targetParameter;

            switch (message.Name)
            {
                case RorzeConstants.Commands.StatusAcquisition:
                    targetParameter = RC550EventTargetParameter.StatusEvent;
                    break;

                case RorzeConstants.Commands.GpioAcquisition:
                    targetParameter = RC550EventTargetParameter.PioEvent;
                    break;

                case RorzeConstants.Commands.ExpansionIOSignalAcquisition:
                    targetParameter = RC550EventTargetParameter.ExpansionIoSignalEvent;
                    break;

                case RorzeConstants.Commands.FanRotationalSpeedAcquisition:
                    targetParameter = RC550EventTargetParameter.FanRotationalSpeedEvent;
                    break;

                case RorzeConstants.Commands.PressureSensorsValuesAcquisition:
                    targetParameter = RC550EventTargetParameter.PressureSensorValueEvent;
                    break;

                default:
                    // Should never happen.
                    targetParameter = RC550EventTargetParameter.AllEvents;
                    break;
            }

            if (!_remainingEventsToReceive.Contains(targetParameter))
            {
                return false;
            }

            // Delete the received event from the expected ones, and consider that command is done if we do not expect any other event.
            _remainingEventsToReceive.Remove(targetParameter);

            if (_remainingEventsToReceive.Count == 0)
            {
                CommandComplete();
            }

            // Current command is not the final recipient for incoming event.
            return false;
        }

        #endregion RorzeCommand
    }
}
