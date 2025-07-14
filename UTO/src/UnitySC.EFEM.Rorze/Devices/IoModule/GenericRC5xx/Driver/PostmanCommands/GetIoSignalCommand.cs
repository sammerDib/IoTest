using System;
using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.PostmanCommands
{
    public abstract class GetIoSignalCommand : RorzeCommand
    {
        protected GetIoSignalCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(
                commandType,
                RorzeConstants.DeviceTypeAbb.IO,
                deviceId,
                RorzeConstants.Commands.ExpansionIOSignalAcquisition,
                sender,
                eqFacade,
                commandParameters)
        {
        }

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
            if (message.Name != RorzeConstants.Commands.ExpansionIOSignalAcquisition)
            {
                return false;
            }

            return TreatMessage(message);
        }

        protected abstract List<SignalData> GetIoSignals(string messageData);

        private bool TreatMessage(RorzeMessage message)
        {
            List<SignalData> ioSignals;

            try
            {
                ioSignals = GetIoSignals(message.Data);
            }
            catch (Exception e)
            {
                Logger.GetLogger(nameof(GetIoSignalCommand))
                    .Error(
                        e,
                        $"Error occurred while parsing {RorzeConstants.DeviceTypeAbb.IO}{message.DeviceId} signal.");
                return false;
            }

            foreach (var signalData in ioSignals)
            {
                var eventArgs = new StatusEventArgs<SignalData>(
                    message.DeviceType + message.DeviceId,
                    signalData);
                facade.SendEquipmentEvent((int)EFEMEvents.ExpansionIOSignalReceived, eventArgs);
            }

            return true;
        }
    }
}
