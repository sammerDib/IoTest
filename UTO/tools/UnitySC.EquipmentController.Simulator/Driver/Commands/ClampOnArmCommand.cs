using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class ClampOnArmCommand: RorzeCommand
    {
        public static ClampOnArmCommand NewOrder(
            Constants.Arm arm,
            bool isReverseOperation,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]>
            {
                new[]
                {
                    ((uint)arm).ToString(), 
                    isReverseOperation ? "0" : "1"
                }
            };

            return new ClampOnArmCommand(parameters, sender, eqFacade, logger);
        }

        private ClampOnArmCommand(
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger):
            base(Constants.CommandType.Order, Constants.Commands.ClampOnArm, parameters, sender, eqFacade, logger)
        {
        }

        protected override bool TreatEvent(Message message)
        {
            // If not an "order" command, we have no reason to treat the event
            if (Command.CommandType != Constants.CommandType.Order)
            {
                return false;
            }

            // Expected events have one parameter group of at least one argument
            // Success: eRBCP:1
            // Error:   eRBCP:2_<Level>_<ErrCode>_<Msg>
            // TMPError:eRBCP:2 TODO: while errors are not managed by EFEM Controller. Must update condition.
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 1)
            {
                return false;
            }

            // Parse argument to get command status
            if (!int.TryParse(message.CommandParameters[0][0], out int result))
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
    }
}
