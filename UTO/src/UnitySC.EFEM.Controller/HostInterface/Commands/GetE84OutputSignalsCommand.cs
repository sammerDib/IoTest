using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.EFEM.Controller.HostInterface.Statuses;
using UnitySC.Equipment.Abstractions;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    public class GetE84OutputSignalsCommand : LoadPortCommand
    {
        public static GetE84OutputSignalsCommand NewEvent(
            E84OutputsStatus status,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
        {
            var parameters = new List<string[]>(1) { status.ToString().Split(',') };
            var message    = new Message(Constants.CommandType.Event, Constants.Commands.GetE84OutputSignals, parameters);

            return new GetE84OutputSignalsCommand(message, sender, eqFacade, logger, equipmentManager);
        }

        public GetE84OutputSignalsCommand(
            Constants.Port loadPortId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.GetE84OutputSignals, loadPortId, sender, eqFacade, logger, equipmentManager)
        {
        }

        public GetE84OutputSignalsCommand(
            Message message,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(message, sender, eqFacade, logger, equipmentManager)
        {
        }

        protected override bool TreatOrder(Message message)
        {
            if (base.TreatOrder(message))
            {
                // Message has been treated if treated by base and coming from expected LP
                return !WrongLp;
            }

            // Check number of parameters
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            var status = new E84OutputsStatus(LoadPortId,
                LoadPort.O_L_REQ,
                LoadPort.O_U_REQ,
                LoadPort.O_READY,
                LoadPort.O_HO_AVBL,
                LoadPort.O_ES);

            // Send acknowledge response with results
            var parameters = new List<string[]>(1) { status.ToString().Split(',') };
            SendAcknowledge(message, parameters);
            return true;
        }

        protected void SendAcknowledge(Message receivedMessage, List<string[]> commandResult)
        {
            SendAcknowledge(new Message(Constants.CommandType.Ack, receivedMessage.CommandName, commandResult));
        }
    }
}
