using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class UndockCommand : LoadPortCommand
    {
        #region Constructors

        public static UndockCommand NewOrder(
            Constants.Port loadPort,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]> { new[] { ((int)loadPort).ToString() } };

            return new UndockCommand(
                Constants.CommandType.Order,
                loadPort,
                parameters,
                sender,
                eqFacade,
                logger);
        }

        private UndockCommand(
            char commandType,
            Constants.Port loadPort,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.Undock, loadPort, parameters, sender, eqFacade, logger)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatEvent(Message message)
        {
            // If not an "order" command, we have no reason to treat the undock event
            if (Command.CommandType != Constants.CommandType.Order)
            {
                return false;
            }

            // Expected events have one parameter group of at least two arguments
            // Success: eUNDK:<Port>_1
            // Error:   eUNDK:<Port>_2_<Level>_<ErrCode>_<Msg>
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length < 2)
            {
                return false;
            }

            // Parse arguments and check that received event applies to the LoadPort of this command
            if (!int.TryParse(message.CommandParameters[0][0], out int port)
                || !int.TryParse(message.CommandParameters[0][1], out int result)
                || port != (int)LoadPort)
            {
                return false;
            }

            switch (result)
            {
                case (int)Constants.EventResult.Success:
                    // TODO Notify driver about command success
                    break;

                case (int)Constants.EventResult.Error:
                    // TODO Notify driver about command error
                    break;
            }

            CommandComplete();
            return true;
        }

        #endregion RorzeCommand
    }
}
