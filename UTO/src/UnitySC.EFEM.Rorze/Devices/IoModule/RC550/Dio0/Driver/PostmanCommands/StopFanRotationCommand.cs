using System;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.PostmanCommands
{
    public class StopFanRotationCommand : RorzeCommand
    {
        public static StopFanRotationCommand NewOrder(
            byte deviceId,
            byte fan,
            bool isGroup,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            var parameters = new[] { $"{(isGroup ? "G" : string.Empty)}{fan}" };

            return new StopFanRotationCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade, parameters);
        }

        private StopFanRotationCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.IO,
                deviceId,
                RorzeConstants.Commands.StopFanRotation,
                sender,
                eqFacade,
                commandParameters)
        {
        }

        protected override bool TreatAck(RorzeMessage message)
        {
            CommandComplete();
            facade.SendEquipmentEvent((int)EFEMEvents.FanRotationSpeedStopped, EventArgs.Empty);

            return base.TreatAck(message);
        }
    }
}
