using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class EnableOrDisableE84: LoadPortCommand
    {
        /// <summary>
        /// Help to remember if the command was sent to enable or disable E84.
        /// It would be used when receiving acknowledgement and event about command termination
        /// to ensure command has been correctly understood and done by EFEM Controller.
        /// </summary>
        private readonly string _enableParameter;

        #region Constructors

        public static EnableOrDisableE84 NewOrder(
            Constants.Port loadPort,
            bool enable,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            var parameters = new List<string[]>
            {
                new []
                {
                    ((int)loadPort).ToString(),
                    enable ? "1" :  "0"
                }
            };

            return new EnableOrDisableE84(
                Constants.CommandType.Order,
                loadPort,
                parameters,
                sender,
                eqFacade,
                logger);
        }

        private EnableOrDisableE84(
            char commandType,
            Constants.Port loadPort,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.EnableOrDisableE84, loadPort, parameters, sender, eqFacade, logger)
        {
            _enableParameter = parameters[0][1];
        }

        #endregion Constructors

        #region LoadPort Command

        protected override bool TreatAck(Message message)
        {
            // Check parameters to ensure that received ack applies to the LoadPort of this command
            return message.CommandParameters.Count == 1
                   && message.CommandParameters[0].Length == 2
                   && int.TryParse(message.CommandParameters[0][0], out int port)
                   && port == (int)LoadPort
                   && message.CommandParameters[0][1].Equals(_enableParameter);
        }

        protected override bool TreatEvent(Message message)
        {
            // If not an "order" command, we have no reason to treat the undock event
            if (Command.CommandType != Constants.CommandType.Order)
            {
                return false;
            }

            // Expected events have one parameter group of at least two arguments
            // Success: eUNDK:<Port>_<switch>_1
            // Error:   eUNDK:<Port>_<switch>_2_<Level>_<ErrCode>_<Msg>
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length < 3)
            {
                return false;
            }

            // Parse arguments and check that received event applies to the LoadPort of this command
            if (!int.TryParse(message.CommandParameters[0][0], out int port)
                || !message.CommandParameters[0][1].Equals(_enableParameter)
                || !int.TryParse(message.CommandParameters[0][2], out int result)
                || port != (int)LoadPort)
            {
                return false;
            }

            switch (result)
            {
                case (int)Constants.EventResult.Success:
                    // TODO Notify driver about command success
                    break;

                case (int)Constants.EventResult.Error:
                    // TODO Notify driver about command error
                    break;
            }

            CommandComplete();
            return true;
        }

        #endregion LoadPort Command
    }
}
