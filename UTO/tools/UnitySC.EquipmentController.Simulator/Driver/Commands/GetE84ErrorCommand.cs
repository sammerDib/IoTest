using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    class GetE84ErrorCommand : LoadPortCommand
    {
        public static GetE84ErrorCommand NewEvent(
            Constants.Port loadPort,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            return new GetE84ErrorCommand(
                Constants.CommandType.Event,
                loadPort,
                null,
                sender,
                eqFacade,
                logger);
        }

        private GetE84ErrorCommand(
            char commandType,
            Constants.Port loadPort,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.GetE84Error, loadPort, parameters, sender, eqFacade, logger)
        {
        }

        protected override bool TreatEvent(Message message)
        {
            // Check parameters to ensure that received event applies to the LoadPort of this command
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length != 4
                || !int.TryParse(message.CommandParameters[0][1], out int port)
                || port != (int)LoadPort)
                return false;

            // For now, we do not have any use of received data about Carrier capacity and size in Host simulator.
            return true;
        }
    }
}
