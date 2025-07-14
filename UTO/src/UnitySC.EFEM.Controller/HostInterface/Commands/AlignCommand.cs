using System.Collections.Generic;
using System.Globalization;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;

using UnitsNet;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the aligner alignment and turn angle command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.4.1 oALGN
    /// </summary>
    public class AlignCommand : AlignerCommand
    {
        public AlignCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.Align, sender, eqFacade, logger, equipmentManager)
        {
        }

        protected override bool TreatOrder(Message message)
        {
            if (App.EfemAppInstance.ControlState == ControlState.Local)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InMaintenanceMode]);
                return true;
            }

            // Check number of parameters
            if (message.CommandParameters.Count != 1
                || message.CommandParameters[0].Length is < 2 or > 3)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Check first parameter validity
            if (!int.TryParse(message.CommandParameters[0][0], out int alignerStation)
                || message.CommandParameters[0][0].Length > 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            if (alignerStation != Constants.Aligner)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            // Check second parameter
            if (!double.TryParse(
                message.CommandParameters[0][1],
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double angle))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Give a default value for when command arrives without third parameter.
            var alignType = AlignType.AlignWaferWithoutCheckingSubO_FlatLocation;

            // Check third parameter (syntax only) if it is present.
            if (message.CommandParameters[0].Length == 3
                && !EnumHelpers.TryParseEnumValue(message.CommandParameters[0][2], out alignType))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Check if command can be executed
            if (!EquipmentManager.Aligner.CanExecute(
                nameof(IAligner.Align),
                out CommandContext context, Angle.FromDegrees(angle), alignType))
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
                    if (CancelIfIsIdleFailed(message, error)
                        || CancelIfIsAngleValidFailed(message, error))
                    {
                        return true;
                    }

                    // Special case for preconditions that can be ignored:
                    // meaning we don't want to send a cancellation message but a command ended in error message
                    // (this is to mimic behavior of previous EFEM Controller)
                    if (IsCommunicatingFailed(error))
                    {
                        shouldBypassCancellation = true;
                    }
                }

                // Send default cancellation code in case we don't know any better
                if (!shouldBypassCancellation)
                {
                    SendCancellation(message, Constants.Errors[ErrorCode.AlignerError]);
                    return true;
                }
            }

            // Send acknowledge response (i.e. command ok, will be performed)
            SendAcknowledge(message);

            // Start command asynchronously and send completion event when done
            EquipmentManager.Aligner
                .AlignAsync(Angle.FromDegrees(angle), alignType)
                .ContinueWith(antecedent => SendCommandResult(antecedent));

            return true;
        }
    }
}
