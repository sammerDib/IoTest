using System;
using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Drivers.Enums;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    public class EventCommand : RorzeCommand
    {
        private readonly EventEnableParameter _enableParameter;
        private readonly LinkedList<EventTargetParameter> _remainingEventsToReceive = new();

        #region Constructors

        public static EventCommand NewOrder(
            string deviceType,
            byte deviceId,
            EventTargetParameter selectedEvent,
            EventEnableParameter enable,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new EventCommand(RorzeConstants.CommandTypeAbb.Order, deviceType, deviceId, sender, eqFacade, selectedEvent, enable);
        }

        private EventCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            EventTargetParameter selectedEvent,
            EventEnableParameter enable)
            : base(commandType, deviceType, deviceId, RorzeConstants.Commands.Event, sender, eqFacade, ((int)selectedEvent).ToString(), ((int)enable).ToString())
        {
            _enableParameter = enable;

            switch (selectedEvent)
            {
                case EventTargetParameter.AllEvents:
                    _remainingEventsToReceive.AddLast(EventTargetParameter.StatusEvent);
                    _remainingEventsToReceive.AddLast(EventTargetParameter.PioEvent);
                    _remainingEventsToReceive.AddLast(EventTargetParameter.StoppingPositionEvent);
                    _remainingEventsToReceive.AddLast(EventTargetParameter.CarrierTypeEvent);
                    break;

                case EventTargetParameter.StatusEvent:
                    _remainingEventsToReceive.AddLast(EventTargetParameter.StatusEvent);
                    break;

                case EventTargetParameter.PioEvent:
                    _remainingEventsToReceive.AddLast(EventTargetParameter.PioEvent);
                    break;

                case EventTargetParameter.StoppingPositionEvent:
                    _remainingEventsToReceive.AddLast(EventTargetParameter.StoppingPositionEvent);
                    break;

                case EventTargetParameter.CarrierTypeEvent:
                    _remainingEventsToReceive.AddLast(EventTargetParameter.CarrierTypeEvent);
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
            EventTargetParameter targetParameter;

            switch (message.Name)
            {
                case RorzeConstants.Commands.StatusAcquisition:
                    targetParameter = EventTargetParameter.StatusEvent;
                    break;

                case RorzeConstants.Commands.GpioAcquisition:
                    targetParameter = EventTargetParameter.PioEvent;
                    break;

                case RorzeConstants.Commands.StoppingPositionAcquisition:
                    targetParameter = EventTargetParameter.StoppingPositionEvent;
                    break;

                // CarrierTypeAcquisition == SubstrateIdAcquisition == GWID
                // EventTargetParameter.CarrierTypeEvent == EventTargetParameter.SubstrateIdEvent == 4
                case RorzeConstants.Commands.CarrierTypeAcquisition:
                    targetParameter = EventTargetParameter.CarrierTypeEvent;
                    break;

                default:
                    // Should never happen.
                    targetParameter = EventTargetParameter.AllEvents;
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

            return true;
        }

        #endregion RorzeCommand
    }
}
