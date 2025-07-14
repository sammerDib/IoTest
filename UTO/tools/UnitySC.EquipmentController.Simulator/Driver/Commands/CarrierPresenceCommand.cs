using System;
using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EquipmentController.Simulator.Driver.Statuses;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class CarrierPresenceCommand: LoadPortCommand
    {
        public static CarrierPresenceCommand NewEvent(
            Constants.Port loadPort,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            return new CarrierPresenceCommand(
                Constants.CommandType.Event,
                loadPort,
                null,
                sender,
                eqFacade,
                logger);
        }

        private CarrierPresenceCommand(
            char commandType,
            Constants.Port loadPort,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.CarrierPresence, loadPort, parameters, sender, eqFacade, logger)
        {
        }

        // Command is event only. We shall not receive ack as we should not send order.
        protected override bool TreatAck(Message message) => false;

        protected override bool TreatEvent(Message message)
        {
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length != 4
                || !int.TryParse(message.CommandParameters[0][0], out int portInt)
                || portInt != (int)LoadPort
                || !uint.TryParse(message.CommandParameters[0][1], out uint presenceUint)
                || presenceUint > 1
                || !uint.TryParse(message.CommandParameters[0][2], out uint placementUint)
                || placementUint > 1
                || !uint.TryParse(message.CommandParameters[0][3], out uint opPushUint)
                || opPushUint > 1)
                return false;

            var port      = Constants.ToPort(portInt);
            var presence  = Convert.ToBoolean(presenceUint);
            var placement = Convert.ToBoolean(placementUint);
            var opPush    = Convert.ToBoolean(opPushUint);

            var carrierPresenceStatus = new CarrierPresenceStatus(port, presence, placement, opPush);
            var statusEventArgs = new StatusEventArgs<CarrierPresenceStatus>(carrierPresenceStatus);

            facade.SendEquipmentEvent((int)EventsToFacade.CarrierPresenceReceived, statusEventArgs);

            return true;
        }
    }
}
