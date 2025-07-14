using System;

using Agileo.Common.Communication;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    public class SetDateAndTimeCommand : RorzeCommand
    {
        private readonly bool _mustSendEquipmentEvent;

        #region Constructors

        public static SetDateAndTimeCommand NewOrder(
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            bool mustSendEquipmentEvent)
        {
            var date = DateTime.Now;
            var dateParams = new[]
            {
                date.Year.ToString(),
                date.Month.ToString(),
                date.Day.ToString(),
                date.Hour.ToString(),
                date.Minute.ToString(),
                date.Second.ToString()
            };

            return new SetDateAndTimeCommand(RorzeConstants.CommandTypeAbb.Order, deviceType, deviceId, sender, eqFacade, mustSendEquipmentEvent, dateParams);
        }

        private SetDateAndTimeCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            bool mustSendEquipmentEvent,
            params string[] commandParameters)
            : base(commandType, deviceType, deviceId, RorzeConstants.Commands.SetDateAndTime, sender, eqFacade, commandParameters)
        {
            _mustSendEquipmentEvent = mustSendEquipmentEvent;
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            if (_mustSendEquipmentEvent)
                facade.SendEquipmentEvent((int)EFEMEvents.SetDateTimeCompleted, System.EventArgs.Empty);

            CommandComplete();
            return true;
        }

        #endregion RorzeCommand
    }
}
