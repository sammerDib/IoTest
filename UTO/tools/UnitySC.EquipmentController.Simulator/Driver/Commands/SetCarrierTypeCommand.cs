using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class SetCarrierTypeCommand : LoadPortCommand
    {
        #region Constructors

        public static SetCarrierTypeCommand NewOrder(
            Constants.Port loadPort,
            uint carrierType,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]>()
            {
                new[] { ((int)loadPort).ToString(), carrierType.ToString() }
            };

            return new SetCarrierTypeCommand(
                Constants.CommandType.Order,
                loadPort,
                parameters,
                sender,
                eqFacade,
                logger);
        }

        private SetCarrierTypeCommand(
            char commandType,
            Constants.Port loadPort,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.SetCarrierType, loadPort, parameters, sender, eqFacade, logger)
        {
        }

        #endregion

        #region RorzeCommand

        protected override bool TreatEvent(Message message)
        {
            // If not an "order" command, we have no reason to treat the event
            if (Command.CommandType != Constants.CommandType.Order)
            {
                return false;
            }

            // Expected events have one parameter group of at least one argument
            // Success: eSCTY:1
            // Error:   eSCTY:2_<Level>_<ErrCode>_<Msg>
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 2)
            {
                return false;
            }

            // Parse argument to get command status
            if (!int.TryParse(message.CommandParameters[0][1], out int result))
            {
                return false;
            }

            switch (result)
            {
                case (int)Constants.EventResult.Success:
                    // TODO Notify driver about command success
                    break;

                case (int)Constants.EventResult.Error:
                    // TODO Notify driver about command failure
                    break;
            }

            CommandComplete();
            return true;
        }

        #endregion
    }
}
