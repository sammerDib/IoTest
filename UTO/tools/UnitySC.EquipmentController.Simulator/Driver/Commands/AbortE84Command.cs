using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class AbortE84Command : LoadPortCommand
    {
        #region Constructors

        public static AbortE84Command NewOrder(
            Constants.Port loadPort,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]> { new[] { ((int)loadPort).ToString() } };

            return new AbortE84Command(
                Constants.CommandType.Order,
                loadPort,
                parameters,
                sender,
                eqFacade,
                logger);
        }

        private AbortE84Command(
            char commandType,
            Constants.Port loadPort,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.AbortE84, loadPort, parameters, sender, eqFacade, logger)
        {
        }

        #endregion

        #region LoadPort Command

        protected override bool TreatAck(Message message)
        {
            // If not an "order" command, we have no reason to treat the undock event
            if (Command.CommandType != Constants.CommandType.Order)
            {
                return false;
            }

            // Expected events have one parameter group of at least two arguments
            // Success: eUNDK:<Port>_<switch>_1
            // Error:   eUNDK:<Port>_<switch>_2_<Level>_<ErrCode>_<Msg>
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length < 1)
            {
                return false;
            }

            // Parse arguments and check that received event applies to the LoadPort of this command
            if (!int.TryParse(message.CommandParameters[0][0], out int port)
                || port != (int)LoadPort)
            {
                return false;
            }

            CommandComplete();
            return true;
        }

        #endregion
    }
}
