using System;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Statuses;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.PostmanCommands
{
    public class GetPressureFromSensorsCommand : RorzeCommand
    {
        #region Constructors

        public static GetPressureFromSensorsCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetPressureFromSensorsCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade);
        }

        public static GetPressureFromSensorsCommand NewEvent(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetPressureFromSensorsCommand(RorzeConstants.CommandTypeAbb.Event, deviceId, sender, eqFacade);
        }

        private GetPressureFromSensorsCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.IO,
                deviceId,
                RorzeConstants.Commands.PressureSensorsValuesAcquisition,
                sender,
                eqFacade,
                commandParameters)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            var result = TreatMessage(message);
            if (result)
            {
                CommandComplete();
            }

            return result;
        }

        protected override bool TreatEvent(RorzeMessage message)
        {
            if (message.Name != RorzeConstants.Commands.PressureSensorsValuesAcquisition)
            {
                return false;
            }

            return TreatMessage(message);
        }

        private bool TreatMessage(RorzeMessage message)
        {
            try
            {
                var pressureSensorsValues = new PressureSensorsValues(message.Data);
                var eventArgs             = new StatusEventArgs<PressureSensorsValues>(
                    message.DeviceType + message.DeviceId,
                    pressureSensorsValues);

                facade.SendEquipmentEvent((int)EFEMEvents.PressureSensorsValuesReceived, eventArgs);
            }
            catch (Exception e)
            {
                Logger.GetLogger(nameof(GetPressureFromSensorsCommand))
                    .Error(e,
                        $"Error occurred while parsing {RorzeConstants.DeviceTypeAbb.IO}{message.DeviceId} signal.");
                return false;
            }

            return true;
        }

        #endregion RorzeCommand
    }
}
