using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Drivers.EventArgs;

namespace UnitySC.EFEM.Rorze.Drivers.PostmanCommands
{
    /// <summary>
    /// Sub-command allowing to get data from an hardware device data table.
    /// </summary>
    public class GetDataSubCommand : RorzeSubCommand
    {
        private readonly string[] _parameters;

        public GetDataSubCommand(
            char commandType,
            string deviceType,
            byte deviceId,
            string devicePart,
            IReadOnlyDictionary<string, DevicePart> deviceParts,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] parameters)
            : base(commandType, deviceType, deviceId, devicePart, RorzeConstants.SubCommands.DataAcquisition, false, deviceParts, sender, eqFacade, "", parameters)
        {
            _parameters = parameters;
        }

        protected override bool TreatAck(RorzeMessage message)
        {
            // A sub command need to receive a sub command message
            if (message is not SubCommandMessage subCommandMessage)
                return false;

            var data      = subCommandMessage.Data.Replace(":", string.Empty).Split(',');
            var eventArgs = new GetDeviceDataEventArgs(subCommandMessage.DevicePart, _parameters, data);
            facade.SendEquipmentEvent((int)EFEMEvents.GetDeviceDataCompleted, eventArgs);

            CommandComplete();
            return true;
        }
    }
}
