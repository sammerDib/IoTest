using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public abstract class LoadPortCommand : RorzeCommand
    {
        #region Constructors

        protected LoadPortCommand(
            char commandType,
            string commandName,
            Constants.Port loadPort,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, commandName, parameters, sender, eqFacade, logger)
        {
            LoadPort = loadPort;
        }

        #endregion Constructors

        #region Properties

        public Constants.Port LoadPort { get; }

        #endregion Properties

        #region RorzeCommand

        protected override bool TreatAck(Message message)
        {
            // Check parameters to ensure that received ack applies to the LoadPort of this command
            return message.CommandParameters.Count == 1
                   && message.CommandParameters[0].Length == 1
                   && int.TryParse(message.CommandParameters[0][0], out int port)
                   && port == (int)LoadPort;
        }

        protected override bool TreatCancel(Message message)
        {
            // Expected cancel message have one parameter group of four arguments
            // c<CommandName>:<Port>_<Level>_<ErrCode>_<Msg>

            // Check parameters to ensure that received cancel applies to the LoadPort of this command
            if (message.CommandParameters.Count == 1
                && message.CommandParameters[0].Length == 4
                && int.TryParse(message.CommandParameters[0][0], out int port)
                && port == (int)LoadPort)
            {
                // TODO Parse received parameters to build the error (Level -> type / ErrorCode -> Code / Message -> Description)
                var error = new Error();

                facade.SendEquipmentEvent((int)EventsToFacade.CmdCompleteWithError, new ErrorOccurredEventArgs(
                    error,
                    message.CommandName));

                CommandComplete(); // Command cancelled by hardware, we expect nothing else, command is done

                return true;
            }

            return false;
        }

        #endregion RorzeCommand
    }
}
