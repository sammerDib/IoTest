using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class SetFfuRpmCommand : RorzeCommand
    {
        #region Constructors

        public static SetFfuRpmCommand NewOrder(
            int setPoint,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]>(1) { new[] { setPoint.ToString() } };

            return new SetFfuRpmCommand(parameters, sender, eqFacade, logger);
        }

        private SetFfuRpmCommand(
            List<string[]> commandParameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger) :
            base(Constants.CommandType.Order, Constants.Commands.SetFfuRpm, commandParameters, sender, eqFacade, logger)
        {
        }

        #endregion Constructors

        #region RorzeCommand Overrides

        protected override bool TreatEvent(Message message)
        {
            // If not an "order" command, we have no reason to treat the event
            if (Command.CommandType != Constants.CommandType.Order)
            {
                return false;
            }

            // Expected events have one parameter group of at least one argument
            // Success: eFFUS:1
            // Error:   eFFUS:2_<Level>_<ErrCode>_<Msg>
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length < 1)
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

        #endregion RorzeCommand Overrides
    }
}
