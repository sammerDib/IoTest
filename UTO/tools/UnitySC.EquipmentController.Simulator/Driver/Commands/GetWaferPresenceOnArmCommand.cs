using System;
using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EFEM.Controller.HostInterface.Enums;

namespace UnitySC.EquipmentController.Simulator.Driver.Commands
{
    public class GetWaferPresenceOnArmCommand: RorzeCommand
    {
        #region Constructors

        public static GetWaferPresenceOnArmCommand NewOrder(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            return new GetWaferPresenceOnArmCommand(Constants.CommandType.Order, null, sender, eqFacade, logger);
        }

        public static GetWaferPresenceOnArmCommand NewEvent(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
        {
            return new GetWaferPresenceOnArmCommand(Constants.CommandType.Event, null, sender, eqFacade, logger);
        }

        public GetWaferPresenceOnArmCommand(
            char commandType,
            List<string[]> parameters,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger)
            : base(commandType, Constants.Commands.GetWaferPresenceOnArm, parameters, sender, eqFacade, logger)
        {
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatAck(Message message)
        {
            // Check parameters to ensure that received event is valid
            if (message.CommandParameters.Count != 3
                || message.CommandParameters[0].Length != 1 // Command type
                || !Enum.TryParse(message.CommandParameters[0][0], out GetWaferPresenceOnArmLastAction lastAction))
            {
                return false;
            }

            try
            {
                ParseArmStateAndHistory(message.CommandParameters[1], out bool isPresentOnUpperArm,
                    out Constants.Stage stageUpperArm, out uint slotUpperArm);
                ParseArmStateAndHistory(message.CommandParameters[1], out bool isPresentOnLowerArm,
                    out Constants.Stage stageLowerArm, out uint slotLowerArm);

                var eventArgs = new ArmHistoryAndWaferPresenceEventArgs(
                    lastAction,
                    isPresentOnUpperArm, stageUpperArm, slotUpperArm,
                    isPresentOnLowerArm, stageLowerArm, slotLowerArm);
                facade.SendEquipmentEvent((int)EventsToFacade.ArmHistoryAndWaferPresenceReceived, eventArgs);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to parse received message about wafer presence on arm and previous arm operation.");
                return false;
            }
            
            CommandComplete();
            return true;
        }

        protected override bool TreatEvent(Message message)
        {
            // Check parameters to ensure that received event is valid
            if (message.CommandParameters.Count != 3
                || message.CommandParameters[0].Length != 1 // Command type
                || !Enum.TryParse(message.CommandParameters[0][0], out GetWaferPresenceOnArmLastAction lastAction))
            {
                return false;
            }

            try
            {
                ParseArmStateAndHistory(message.CommandParameters[1], out bool isPresentOnUpperArm,
                    out Constants.Stage stageUpperArm, out uint slotUpperArm);
                ParseArmStateAndHistory(message.CommandParameters[2], out bool isPresentOnLowerArm,
                    out Constants.Stage stageLowerArm, out uint slotLowerArm);

                var eventArgs = new ArmHistoryAndWaferPresenceEventArgs(
                    lastAction, 
                    isPresentOnUpperArm, stageUpperArm, slotUpperArm, 
                    isPresentOnLowerArm, stageLowerArm, slotLowerArm);
                facade.SendEquipmentEvent((int)EventsToFacade.ArmHistoryAndWaferPresenceReceived, eventArgs);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to parse received message about wafer presence on arm and previous arm operation.");
                return false;
            }

            return true;
        }

        #endregion RorzeCommand

        #region Helpers

        private static void ParseArmStateAndHistory(
            IReadOnlyList<string> description, 
            out bool waferPresenceOnArm, 
            out Constants.Stage previousAccessedStage,
            out uint previousAccessedSlot)
        {
            if (description.Count != 3
                || !uint.TryParse(description[0], out uint waferPresenceOnArmUint)
                || !Enum.TryParse(description[1], out previousAccessedStage)
                || !uint.TryParse(description[2], out previousAccessedSlot))
            {
                throw new InvalidOperationException($"{nameof(GetWaferPresenceOnArmCommand)} - Unable to parse the received message.");
            }

            switch (waferPresenceOnArmUint)
            {
                case 0:
                    waferPresenceOnArm = false;
                    break;
                case 1:
                    waferPresenceOnArm = true;
                    break;
                default:
                    throw new InvalidOperationException($"{nameof(GetWaferPresenceOnArmCommand)} - Unable to parse the received message.");
            }
        }

        #endregion Helpers
    }
}
