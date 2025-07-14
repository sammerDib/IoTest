using System;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Statuses;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.PostmanCommands
{
    public class GetRotationalFansSpeedCommand : RorzeCommand
    {
        #region Constructors

        public static GetRotationalFansSpeedCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetRotationalFansSpeedCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade);
        }

        public static GetRotationalFansSpeedCommand NewEvent(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetRotationalFansSpeedCommand(RorzeConstants.CommandTypeAbb.Event, deviceId, sender, eqFacade);
        }

        private GetRotationalFansSpeedCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.IO,
                deviceId,
                RorzeConstants.Commands.FanRotationalSpeedAcquisition,
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
            if (message.Name != RorzeConstants.Commands.FanRotationalSpeedAcquisition)
            {
                return false;
            }

            return TreatMessage(message);
        }

        private bool TreatMessage(RorzeMessage message)
        {
            try
            {
                var fansRotationSpeed = new FansRotationSpeed(message.Data);
                var eventArgs         = new StatusEventArgs<FansRotationSpeed>(
                    message.DeviceType + message.DeviceId,
                    fansRotationSpeed);

                facade.SendEquipmentEvent((int)EFEMEvents.FansRotationSpeedReceived, eventArgs);
            }
            catch (Exception e)
            {
                Logger.GetLogger(nameof(GetRotationalFansSpeedCommand))
                    .Error(e,
                        $"Error occurred while parsing {RorzeConstants.DeviceTypeAbb.IO}{message.DeviceId} signal.");
                return false;
            }

            return true;
        }

        #endregion RorzeCommand
    }
}
