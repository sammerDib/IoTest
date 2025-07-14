using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.LightTower;

using ErrorCode = UnitySC.EFEM.Controller.HostInterface.Enums.ErrorCode;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the set buzzer on/off command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.5.1 oBZON
    /// </summary>
    public class SetBuzzerCommand : BaseCommand
    {
        public SetBuzzerCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.SetBuzzer, sender, eqFacade, logger, equipmentManager)
        {
        }

        protected override bool TreatOrder(Message message)
        {
            // Check number of parameters
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Check first parameter (syntax only)
            if (!uint.TryParse(message.CommandParameters[0][0], out uint isBuzzerOnAsUint)
                || message.CommandParameters[0][0].Length > 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Check first parameter (range validity)
            if (isBuzzerOnAsUint > 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            var isBuzzerOn = isBuzzerOnAsUint == 1;
            var lightTower = EquipmentManager.LightTower;

            // Check if command can be executed
            if (!lightTower.CanExecute(
                nameof(ILightTower.DefineBuzzerMode),
                out CommandContext context,
                isBuzzerOn ? BuzzerState.On : BuzzerState.Off))
            {
                bool shouldBypassCancellation = false;
                foreach (var error in context?.Errors ?? new List<string>())
                {
                    // Log the error so we can determine later why Host command is cancelled
                    if (!string.IsNullOrEmpty(error))
                    {
                        Logger.Error(error);
                    }

                    // Check for preconditions that have a dedicated error code
                    /* None for now */

                    // Special case for preconditions that can be ignored:
                    // meaning we don't want to send a cancellation message but a command ended in error message
                    // (this is to mimic behavior of previous EFEM Controller)
                    if (IsCommunicatingFailed(error))
                    {
                        shouldBypassCancellation = true;
                        break;
                    }
                }

                // Send default cancellation code in case we don't know any better
                if (!shouldBypassCancellation)
                {
                    SendCancellation(message, Constants.Errors[ErrorCode.ErrorOccurredState]);
                    return true;
                }
            }

            // Send acknowledge response (i.e. command ok, will be performed)
            SendAcknowledge(message);

            // Start command asynchronously and send completion event when done
            lightTower.DefineBuzzerModeAsync(isBuzzerOn ? BuzzerState.On : BuzzerState.Off)
                .ContinueWith(antecedent => SendCommandResult(antecedent));

            return true;
        }
    }
}
