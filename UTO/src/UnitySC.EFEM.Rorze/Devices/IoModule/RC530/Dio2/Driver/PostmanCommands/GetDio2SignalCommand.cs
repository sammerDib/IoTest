using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Driver.Statuses;
using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Driver.PostmanCommands
{
    public class GetDio2SignalCommand : GetIoSignalCommand
    {
        public static GetIoSignalCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetDio2SignalCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade);
        }

        public static GetIoSignalCommand NewEvent(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetDio2SignalCommand(RorzeConstants.CommandTypeAbb.Event, deviceId, sender, eqFacade);
        }

        public GetDio2SignalCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(commandType, deviceId, sender, eqFacade, commandParameters)
        {
        }

        protected override List<SignalData> GetIoSignals(string messageData)
        {
            return new List<SignalData> { new Dio2SignalData(messageData) };
        }
    }
}
