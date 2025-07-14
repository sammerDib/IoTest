using System;
using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EquipmentController.Simulator.Driver.Statuses;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class GetWaferSizeInLoadPortCommand : LoadPortCommand
    {

        public static GetWaferSizeInLoadPortCommand NewOrder(
            Constants.Port loadPort,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]> { new[] { ((int)loadPort).ToString() } };

            return new GetWaferSizeInLoadPortCommand(
                Constants.CommandType.Order,
                loadPort,
                parameters,
                sender,
                eqFacade,
                logger);
        }

        private GetWaferSizeInLoadPortCommand(
            char commandType,
            Constants.Port loadPort,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.GetWaferSizeInLoadPort, loadPort, parameters, sender, eqFacade, logger)
        {
        }

        protected override bool TreatAck(Message message)
        {
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length != 3)
                return false;

            try
            {
                Enum.TryParse(message.CommandParameters[0][1],out Constants.Port port) ;
                int.TryParse(message.CommandParameters[0][2],out int waferSize) ;


                var status = new WaferSizeStatus(port,waferSize);

                facade.SendEquipmentEvent((int)EventsToFacade.WaferSizeReceived, new StatusEventArgs<WaferSizeStatus>(status));
                CommandComplete();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"{nameof(GetWaferSizeInLoadPortCommand)} - Failed when trying to parse wafer size statuses from equipment. Received status=\"{message.CommandParameters[0][0]}\".");
                return false;
            }
        }
    }
}
