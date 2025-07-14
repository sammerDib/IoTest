using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class SetBuzzerCommand : RorzeCommand
    {
        #region Constructors
        public static SetBuzzerCommand NewOrder(
            BuzzerState isBuzzerOn,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]>
            {
                new[]
                {
                    isBuzzerOn == BuzzerState.On ? "1" : "0"
                }
            };

            return new SetBuzzerCommand(parameters, sender, eqFacade, logger);
        }

        private SetBuzzerCommand(
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger) :
            base(Constants.CommandType.Order, Constants.Commands.SetBuzzer, parameters, sender, eqFacade, logger)
        {
        }

        #endregion

        #region RorzeCommand
        protected override bool TreatAck(Message message)
        {
            // Command is finished on ACK received.
            CommandComplete();
            return true;
        }

        protected override bool TreatEvent(Message message)
        {
            // If not an "order" command, we have no reason to treat the event
            if (Command.CommandType != Constants.CommandType.Order)
            {
                return false;
            }

            // Expected events have one parameter group of at least one argument
            // Success: eBZON:1
            // Error:   eBZON:2_<Level>_<ErrCode>_<Msg>
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

        #endregion RorzeCommand
    }
}
