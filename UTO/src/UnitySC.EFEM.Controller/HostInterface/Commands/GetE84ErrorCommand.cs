using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;

using UnitySC.Equipment.Abstractions;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    class GetE84ErrorCommand : LoadPortCommand
    {
        public static GetE84ErrorCommand NewEvent(
            Constants.Port loadPortId,
            Error error,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
        {
            var parameters = new List<string[]>
            {
                new[] { error.Type, ((int)loadPortId).ToString(), error.Code, error.Description }
            };
            var message = new Message(Constants.CommandType.Event, Constants.Commands.GetE84Error, parameters);

            return new GetE84ErrorCommand(message, sender, eqFacade, logger, equipmentManager);
        }

        public GetE84ErrorCommand(
            Message message,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(message, sender, eqFacade, logger, equipmentManager)
        {
        }
    }
}
