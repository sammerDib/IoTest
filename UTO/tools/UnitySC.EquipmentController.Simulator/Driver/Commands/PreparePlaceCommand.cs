using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class PreparePlaceCommand : RorzeCommand
    {
        public static PreparePlaceCommand NewOrder(
            Constants.Arm arm, 
            Constants.Stage stage, 
            uint slot,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]>
            {
                new[]
                {
                    ((uint)arm).ToString(), 
                    ((uint)stage).ToString(), 
                    slot.ToString("00")
                }
            };

            return new PreparePlaceCommand(parameters, sender, eqFacade, logger);
        }

        private PreparePlaceCommand(
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger) :
            base(Constants.CommandType.Order, Constants.Commands.PreparePlace, parameters, sender, eqFacade, logger)
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
            // Success: eWPUT:1
            // Error:   eWPUT:2_<Level>_<ErrCode>_<Msg>
            // TMPError:eWPUT:2 TODO: while errors are not managed by EFEM Controller. Must update condition.
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
