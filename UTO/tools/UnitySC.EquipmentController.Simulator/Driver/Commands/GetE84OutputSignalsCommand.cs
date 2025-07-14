using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class GetE84OutputSignalsCommand : RorzeCommand
    {
        public static GetE84OutputSignalsCommand NewOrder(
            Constants.Port loadPort,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]> { new[] { ((int)loadPort).ToString() } };

            return new GetE84OutputSignalsCommand(parameters, sender, eqFacade, logger);
        }

        private GetE84OutputSignalsCommand(
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(Constants.CommandType.Order, Constants.Commands.GetE84OutputSignals, parameters, sender, eqFacade, logger)
        {
        }

        protected override bool TreatAck(Message message)
        {
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
            // Success: aE84O:LoadPort_LREQ_UREQ_READY_H0AVBL_ES
            // Error:   oE84T:2_<Level>_<ErrCode>_<Msg>
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 6)
            {
                return false;
            }

            CommandComplete();
            return true;
        }
    }
}
