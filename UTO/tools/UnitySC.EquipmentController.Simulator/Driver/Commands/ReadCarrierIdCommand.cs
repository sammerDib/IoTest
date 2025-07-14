using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EquipmentController.Simulator.Driver.Statuses;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class ReadCarrierIdCommand : LoadPortCommand
    {
        #region Constructors

        public static ReadCarrierIdCommand NewOrder(
            Constants.Port loadPort,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]> { new[] { ((int)loadPort).ToString() } };

            return new ReadCarrierIdCommand(
                Constants.CommandType.Order,
                loadPort,
                parameters,
                sender,
                eqFacade,
                logger);
        }

        private ReadCarrierIdCommand(
            char commandType,
            Constants.Port loadPort,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.Read, loadPort, parameters, sender, eqFacade, logger)
        {
        }
        #endregion

        #region RorzeCommand

        protected override bool TreatEvent(Message message)
        {
            // If not an "order" command, we have no reason to treat the undock event
            if (Command.CommandType != Constants.CommandType.Order)
            {
                return false;
            }

            // Expected events have one parameter group of at least two arguments
            // Success: eREAD:<Port>_1_RFID
            // Error:   eREAD:<Port>_2_<Level>_<ErrCode>_<Msg>
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length < 3)
            {
                return false;
            }

            // Parse arguments and check that received event applies to the LoadPort of this command
            if (!int.TryParse(message.CommandParameters[0][0], out int port)
                || !int.TryParse(message.CommandParameters[0][1], out int result)
                || port != (int)LoadPort)
            {
                return false;
            }

            if (result == 1)
            {
                string carrierId = message.CommandParameters[0][2];
                var status = new CarrierIdStatus((Constants.Port)port, carrierId);
                facade.SendEquipmentEvent((int)EventsToFacade.CarrierIdReceived, new StatusEventArgs<CarrierIdStatus>(status));
            }

            CommandComplete();
            return true;
        }

        #endregion RorzeCommand
    }
}
