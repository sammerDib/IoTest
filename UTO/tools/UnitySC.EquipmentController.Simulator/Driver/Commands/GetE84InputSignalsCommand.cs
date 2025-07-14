using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class GetE84InputSignalsCommand : RorzeCommand
    {
        public static GetE84InputSignalsCommand NewOrder(
            Constants.Port loadPort,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]> { new[] { ((int)loadPort).ToString() } };

            return new GetE84InputSignalsCommand(parameters, sender, eqFacade, logger);
        }

        private GetE84InputSignalsCommand(
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(Constants.CommandType.Order, Constants.Commands.GetE84InputSignals, parameters, sender, eqFacade, logger)
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
            // Success: aE84I:LoadPort_VALID_CS0_CS1_TRREQ_BUSY_COMPT_CONT
            // Error:   oE84T:2_<Level>_<ErrCode>_<Msg>
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 8)
            {
                return false;
            }

            CommandComplete();
            return true;
        }
    }
}
