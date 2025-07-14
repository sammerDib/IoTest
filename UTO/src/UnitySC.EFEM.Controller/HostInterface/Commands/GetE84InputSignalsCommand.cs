using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.EFEM.Controller.HostInterface.Statuses;
using UnitySC.Equipment.Abstractions;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    public class GetE84InputSignalsCommand : LoadPortCommand
    {
        public static GetE84InputSignalsCommand NewEvent(
            E84InputsStatus status,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
        {
            var parameters = new List<string[]>(1) { status.ToString().Split(',') };
            var message    = new Message(Constants.CommandType.Event, Constants.Commands.GetE84InputSignals, parameters);

            return new GetE84InputSignalsCommand(message, sender, eqFacade, logger, equipmentManager);
        }

        public GetE84InputSignalsCommand(
            Constants.Port loadPortId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.GetE84InputSignals, loadPortId, sender, eqFacade, logger, equipmentManager)
        {
        }

        public GetE84InputSignalsCommand(
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

            var status = new E84InputsStatus(LoadPortId,
                LoadPort.I_VALID,
                LoadPort.I_CS_0,
                LoadPort.I_CS_1,
                LoadPort.I_TR_REQ,
                LoadPort.I_BUSY,
                LoadPort.I_COMPT,
                LoadPort.I_CONT);

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
