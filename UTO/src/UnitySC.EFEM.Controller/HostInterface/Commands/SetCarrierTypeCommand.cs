using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;

using CommandContext = Agileo.EquipmentModeling.CommandContext;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the set carrier type command defined in
    /// Feature carrier type selection for a TMAP.pdf oSCTY
    /// </summary>
    public class SetCarrierTypeCommand : LoadPortCommand
    {

        public SetCarrierTypeCommand(
            Constants.Port loadPortId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.SetCarrierType, loadPortId, sender, eqFacade, logger, equipmentManager)
        {
        }

        protected override bool TreatOrder(Message message)
        {
            if (App.EfemAppInstance.ControlState == ControlState.Local)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InMaintenanceMode]);
                return true;
            }

            if (base.TreatOrder(message))
            {
                // Message has been treated if treated by base and coming from expected LP
                return !WrongLp;
            }

            // Check number of parameters
            if (message.CommandParameters[0].Length != 2)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Check second parameter (syntax only)
            if (!uint.TryParse(message.CommandParameters[0][1], out uint carrierType))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalParameter]);
                return true;
            }

            // Check second parameter (range validity)
            if (carrierType < 1 || carrierType > 16)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            int deviceId = (int)Constants.ToLoadPortUnit(LoadPortId);

            if (!LoadPort.CanExecute(
                    nameof(ILoadPort.SetCarrierType),
                    out CommandContext context,
                    carrierType))
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
                    if (CancelIfIsInServiceFailed(message, error)
                        || CancelIfIsIdleFailed(message, error))
                    {
                        return true;
                    }

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
                    SendCancellation(message, Constants.Errors[ErrorCode.LoadPortError]);
                    return true;
                }
            }

            // Send acknowledge response (i.e. command ok, will be performed)
            SendAcknowledge(message);

            LoadPort.SetCarrierTypeAsync(carrierType)
                .ContinueWith(antecedent => SendCommandResult(antecedent, deviceId));

            return true;
        }
    }
}
