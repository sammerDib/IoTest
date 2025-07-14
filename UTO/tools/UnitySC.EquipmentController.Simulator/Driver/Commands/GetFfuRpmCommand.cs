using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EquipmentController.Simulator.Driver.EventArgs;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class GetFfuRpmCommand : RorzeCommand
    {
        #region Constructors

        public static GetFfuRpmCommand NewOrder(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]>();

            return new GetFfuRpmCommand(parameters, sender, eqFacade, logger);
        }

        private GetFfuRpmCommand(
            List<string[]> commandParameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger) :
            base(Constants.CommandType.Order, Constants.Commands.GetFfuRpm, commandParameters, sender, eqFacade, logger)
        {
        }

        #endregion Constructors

        #region RorzeCommand Overrides

        protected override bool TreatAck(Message message)
        {
            // If not an "order" command, we have no reason to treat the acknowledgement
            if (Command.CommandType != Constants.CommandType.Order)
            {
                return false;
            }

            // Expected ack have one parameter group of one argument
            // aGFAN:1000
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 1)
            {
                return false;
            }

            // Parse argument to get fan speed
            if (!uint.TryParse(message.CommandParameters[0][0], out uint speedRpm))
            {
                return false;
            }

            facade.SendEquipmentEvent((int)EventsToFacade.FfuSpeedReceived, new FfuSpeedReceivedEventArgs(speedRpm));

            CommandComplete();
            return true;
        }

        #endregion RorzeCommand Overrides
    }
}
