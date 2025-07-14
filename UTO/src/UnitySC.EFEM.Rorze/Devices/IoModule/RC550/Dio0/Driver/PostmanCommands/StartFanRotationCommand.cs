using System;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.PostmanCommands
{
    public class StartFanRotationCommand : RorzeCommand
    {
        public static StartFanRotationCommand NewOrder(
            byte deviceId,
            byte fan,
            bool isGroup,
            uint rev,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            var parameters = new[] { $"{(isGroup ? "G" : string.Empty)}{fan}", rev.ToString() };

            return new StartFanRotationCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade, parameters);
        }

        private StartFanRotationCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.IO,
                deviceId,
                RorzeConstants.Commands.StartFanRotation,
                sender,
                eqFacade,
                commandParameters)
        {
        }

        protected override bool TreatAck(RorzeMessage message)
        {
            CommandComplete();
            facade.SendEquipmentEvent((int)EFEMEvents.FanRotationSpeedStarted, EventArgs.Empty);

            return base.TreatAck(message);
        }
    }
}
