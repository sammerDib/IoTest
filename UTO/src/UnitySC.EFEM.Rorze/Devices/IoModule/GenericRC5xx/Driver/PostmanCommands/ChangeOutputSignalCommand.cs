using System;
using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.PostmanCommands
{
    public class ChangeOutputSignalCommand : RorzeCommand
    {
        private readonly bool _mustSendEquipmentEvent;

        public static ChangeOutputSignalCommand NewOrder(
            byte deviceId,
            List<SignalData> dataWithMasks,
            bool mustSendEquipmentEvent,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            if (dataWithMasks == null)
            {
                throw new ArgumentNullException(nameof(dataWithMasks));
            }

            if (dataWithMasks.Count == 0)
            {
                throw new ArgumentException(
                    @$"{nameof(ChangeOutputSignalCommand)} - At least 1 output signal is expected.",
                    nameof(dataWithMasks));
            }

            var parameters =
                new List<string>(dataWithMasks.Count + 1)
                {
                    dataWithMasks[0].IoModuleNo.ToString("D3")
                };

            foreach (var dataWithMask in dataWithMasks)
            {
                var param = dataWithMask.GetSignalOutput();

                // Insert mask in parameter, if any.
                var mask = dataWithMask.GetOutputMask();
                if (!string.IsNullOrEmpty(mask))
                {
                    param += '/' + mask;
                }

                parameters.Add(param);
            }

            return new ChangeOutputSignalCommand(
                RorzeConstants.CommandTypeAbb.Order,
                deviceId,
                mustSendEquipmentEvent,
                sender,
                eqFacade,
                parameters.ToArray());
        }

        private ChangeOutputSignalCommand(
            char commandType,
            byte deviceId,
            bool mustSendEquipmentEvent,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.IO,
                deviceId,
                RorzeConstants.Commands.ChangeOutputSignal,
                sender,
                eqFacade,
                commandParameters)
        {
            _mustSendEquipmentEvent = mustSendEquipmentEvent;
        }

        protected override bool TreatAck(RorzeMessage message)
        {
            if (_mustSendEquipmentEvent)
            {
                facade.SendEquipmentEvent(
                    (int)EFEMEvents.ChangeOutputSignalCompleted,
                    EventArgs.Empty);
            }

            CommandComplete();
            return true;
        }
    }
}
