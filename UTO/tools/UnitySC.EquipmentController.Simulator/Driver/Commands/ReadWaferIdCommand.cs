using System;
using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.EquipmentController.Simulator.Driver.Statuses;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class ReadWaferIdCommand : RorzeCommand
    {
        #region Constructors

        public static ReadWaferIdCommand NewOrder(
            SubstrateSide side,
            string frontRecipeName,
            string backRecipeName,
            SubstrateTypeRDID type,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            List<string[]> parameters;

            switch (side)
            {
                case SubstrateSide.Front:
                    parameters = new List<string[]>
                    {
                        new []
                        {
                            Constants.Aligner.ToString(),
                            ((int)side).ToString(),
                            frontRecipeName,
                            ((int)type).ToString(),
                        }
                    };
                    break;
                case SubstrateSide.Back:
                    parameters = new List<string[]>
                    {
                        new []
                        {
                            Constants.Aligner.ToString(),
                            ((int)side).ToString()
                        }
                    };
                    break;
                case SubstrateSide.Both:
                    parameters = new List<string[]>
                    {
                        new []
                        {
                            Constants.Aligner.ToString(),
                            ((int)side).ToString(),
                            frontRecipeName,
                            backRecipeName,
                            ((int)type).ToString(),
                        }
                    };
                    break;
                case SubstrateSide.Frame:
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }

            return new ReadWaferIdCommand(parameters, sender, eqFacade, logger);
        }

        private ReadWaferIdCommand(
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(Constants.CommandType.Order, Constants.Commands.ReadWaferId, parameters, sender, eqFacade, logger)
        {
        }

        #endregion

        #region command

        protected override bool TreatEvent(Message message)
        {
            // If not an "order" command, we have no reason to treat the event
            if (Command.CommandType != Constants.CommandType.Order)
            {
                return false;
            }

            // Expected events have one parameter group of at least one argument
            // Success: eRDID:6_eventResult_waferId
            // Error:   eRDID:2_<Level>_<ErrCode>_<Msg>
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length < 3)
            {
                return false;
            }

            // Parse argument to get command status
            if (!int.TryParse(message.CommandParameters[0][0], out int aligner) ||
                !int.TryParse(message.CommandParameters[0][1], out int result)  ||
                aligner != Constants.Aligner)
            {
                return false;
            }

            switch (result)
            {
                case (int)Constants.EventResult.Success:
                    WaferIdStatus status;
                    status = message.CommandParameters[0].Length == 3 ? 
                        new WaferIdStatus(message.CommandParameters[0][2]) : 
                        new WaferIdStatus(message.CommandParameters[0][2], message.CommandParameters[0][3]);
                    facade.SendEquipmentEvent((int)EventsToFacade.WaferIdReceived, new StatusEventArgs<WaferIdStatus>(status));
                    break;
                case (int)Constants.EventResult.Error:
                    // TODO Notify driver about command failure
                    break;
            }

            CommandComplete();
            return true;
        }

        #endregion
    }
}
