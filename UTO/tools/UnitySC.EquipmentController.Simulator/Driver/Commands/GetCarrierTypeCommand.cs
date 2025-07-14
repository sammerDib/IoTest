using System;
using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EquipmentController.Simulator.Driver.Statuses;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class GetCarrierTypeCommand : LoadPortCommand
    {

        public static GetCarrierTypeCommand NewOrder(
            Constants.Port loadPort,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]> { new[] { ((int)loadPort).ToString() } };

            return new GetCarrierTypeCommand(
                Constants.CommandType.Order,
                loadPort,
                parameters,
                sender,
                eqFacade,
                logger);
        }

        private GetCarrierTypeCommand(
            char commandType,
            Constants.Port loadPort,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.GetCarrierType, loadPort, parameters, sender, eqFacade, logger)
        {
        }

        protected override bool TreatEvent(Message message)
        {
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length != 3)
                return false;

            try
            {
                Enum.TryParse(message.CommandParameters[0][0],out Constants.Port port) ;
                uint.TryParse(message.CommandParameters[0][2], out uint carrierType) ;

                var status = new CarrierTypeStatus(port, carrierType);

                facade.SendEquipmentEvent(
                    (int)EventsToFacade.CarrierTypeReceived,
                    new StatusEventArgs<CarrierTypeStatus>(status));
                CommandComplete();
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(
                    e,
                    $"{nameof(GetCarrierTypeCommand)} - Failed when trying to parse wafer size statuses from equipment. Received status=\"{message.CommandParameters[0][0]}\".");
                return false;
            }
        }
    }
}
