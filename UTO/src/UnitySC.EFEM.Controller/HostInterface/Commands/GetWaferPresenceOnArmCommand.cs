using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Controller.HostInterface.Converters;
using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Robot;

using ErrorCode = UnitySC.EFEM.Controller.HostInterface.Enums.ErrorCode;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the get wafer presence on arm command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.2.9 oWARM
    /// </summary>
    public class GetWaferPresenceOnArmCommand : BaseCommand
    {
        #region Constructors

        public static GetWaferPresenceOnArmCommand NewOrder(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
        {
            return new GetWaferPresenceOnArmCommand(sender, eqFacade, logger, equipmentManager);
        }

        public static GetWaferPresenceOnArmCommand NewEvent(
            Constants.Arm concernedArm,
            ArmHistoryItem upperArmShortHistory,
            ArmHistoryItem lowerArmShortHistory,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
        {
            var parameters = CreateCommandParameters(concernedArm, upperArmShortHistory, lowerArmShortHistory);
            var message    = new Message(Constants.CommandType.Event, Constants.Commands.GetWaferPresenceOnArm, parameters);

            return new GetWaferPresenceOnArmCommand(message, sender, eqFacade, logger, equipmentManager);
        }

        private GetWaferPresenceOnArmCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(
                Constants.Commands.GetWaferPresenceOnArm,
                sender,
                eqFacade,
                logger,
                equipmentManager)
        {
        }

        private GetWaferPresenceOnArmCommand(
            Message message,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(message, sender, eqFacade, logger, equipmentManager)
        {
        }

        #endregion Constructors

        #region BaseCommand

        protected override bool TreatOrder(Message message)
        {
            // Check number of parameters
            if (message.CommandParameters.Count > 0)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Command just aim to retrieve information about a RorzeRobot.
            // No need to send a command to equipment, reading statuses is sufficient.
            Constants.Arm arm = RobotArmConverter.ToRobotArm(EquipmentManager.Robot.LastArmBeingMoved);

            var parameters = CreateCommandParameters(
                arm,
                EquipmentManager.Robot.UpperArmHistory,
                EquipmentManager.Robot.LowerArmHistory);

            // Send acknowledge response with results
            SendAcknowledge(message, parameters);

            return true;
        }

        protected void SendAcknowledge(Message receivedMessage, List<string[]> commandResult)
        {
            SendAcknowledge(new Message(Constants.CommandType.Ack, receivedMessage.CommandName, commandResult));
        }

        #endregion BaseCommand

        #region Private Methods

        private static List<string[]> CreateCommandParameters(
            Constants.Arm concernedArm,
            ArmHistoryItem upperArmShortHistory,
            ArmHistoryItem lowerArmShortHistory)
        {
            var parameters = new List<string[]>(3)
            {
                new string[1], // Last Action
                new string[3], // Upper Arm
                new string[3]  // Lower Arm
            };

            GetWaferPresenceOnArmLastAction lastAction;
            switch (concernedArm)
            {
                case Constants.Arm.Both when upperArmShortHistory.Command.Equals(nameof(IRobot.Pick))
                                             && lowerArmShortHistory.Command.Equals(nameof(IRobot.Place)):
                    lastAction = GetWaferPresenceOnArmLastAction.DualArmPut;
                    break;

                case Constants.Arm.Both:
                    lastAction = GetWaferPresenceOnArmLastAction.NoAction;
                    break;

                case Constants.Arm.Upper when upperArmShortHistory.Command.Equals(nameof(IRobot.Pick)):
                    lastAction = GetWaferPresenceOnArmLastAction.UpperArmGet;
                    break;

                case Constants.Arm.Upper when upperArmShortHistory.Command.Equals(nameof(IRobot.Place)):
                    lastAction = GetWaferPresenceOnArmLastAction.UpperArmPut;
                    break;

                case Constants.Arm.Lower when lowerArmShortHistory.Command.Equals(nameof(IRobot.Pick)):
                    lastAction = GetWaferPresenceOnArmLastAction.LowerArmGet;
                    break;

                case Constants.Arm.Lower when lowerArmShortHistory.Command.Equals(nameof(IRobot.Place)):
                    lastAction = GetWaferPresenceOnArmLastAction.LowerArmPut;
                    break;

                default:
                    lastAction = GetWaferPresenceOnArmLastAction.NoAction;
                    break;
            }

            parameters[0][0] = ((int)lastAction).ToString();
            parameters[1][0] = upperArmShortHistory.HasMaterial ? "1" : "0";
            parameters[2][0] = lowerArmShortHistory.HasMaterial ? "1" : "0";

            // Default TransferLocation used when no HW data is available.
            if (upperArmShortHistory.Location != TransferLocation.DummyPortA)
                parameters[1][1] = ((int)TransferLocationConverter.ToStage(upperArmShortHistory.Location)).ToString();
            else
                parameters[1][1] = "0";

            // Default TransferLocation used when no HW data is available.
            if (lowerArmShortHistory.Location != TransferLocation.DummyPortA)
                parameters[2][1] = ((int)TransferLocationConverter.ToStage(lowerArmShortHistory.Location)).ToString();
            else
                parameters[2][1] = "0";

            parameters[1][2] = upperArmShortHistory.Slot.ToString("00");
            parameters[2][2] = lowerArmShortHistory.Slot.ToString("00");

            return parameters;
        }

        #endregion Private Methods
    }
}
