using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class GetCarrierCapacityAndSizeCommand : LoadPortCommand
    {
        #region Constructors

        public static GetCarrierCapacityAndSizeCommand NewEvent(
            Constants.Port loadPort,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            return new GetCarrierCapacityAndSizeCommand(
                Constants.CommandType.Event,
                loadPort,
                null,
                sender,
                eqFacade,
                logger);
        }

        private GetCarrierCapacityAndSizeCommand(
            char commandType,
            Constants.Port loadPort,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.GetCarrierCapacityAndSize, loadPort, parameters, sender, eqFacade, logger)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatEvent(Message message)
        {
            // Check parameters to ensure that received event applies to the LoadPort of this command
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length != 3
                || !int.TryParse(message.CommandParameters[0][0], out int port)
                || port != (int)LoadPort)
                return false;

            // For now, we do not have any use of received data about Carrier capacity and size in Host simulator.
            return true;
        }

        #endregion RorzeCommand
    }
}
