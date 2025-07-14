using System.Collections.Generic;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;
using Agileo.EquipmentModeling;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the read carrier id command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.3.9 oREAD
    /// </summary>
    public class ReadCarrierIdCommand : LoadPortCommand
    {
        public ReadCarrierIdCommand(
            Constants.Port loadPortId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.Read, loadPortId, sender, eqFacade, logger, equipmentManager)
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
            if (message.CommandParameters[0].Length != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            int deviceId = (int)Constants.ToLoadPortUnit(LoadPortId);

            // Actually check if command can be executed
            if (!LoadPort.CanExecute(nameof(ILoadPort.ReadCarrierId), out CommandContext context))
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
                        || CancelIfIsCarrierIdSupportedFailed(message, error)
                        || CancelIfIsCarrierCorrectlyPlacedFailed(message, error))
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

            // Start command asynchronously and send completion event when done
            LoadPort.ReadCarrierIdAsync().ContinueWith(antecedent => SendCommandResult(antecedent, deviceId));

            return true;
        }

        protected override Error GetCommandError(Task completedTask, int deviceId = -1)
        {
            return Constants.Errors[ErrorCode.RfidReadFailed];
        }

        protected override List<string> GetResultArguments(Error error = null)
        {
            // Build command parameter arguments
            var parameterArgs = new List<string>
            {
                // Add result
                ((int)(error == null
                    ? Constants.EventResult.Success
                    : Constants.EventResult.Error)).ToString()
            };

            if (error == null)
            {
                parameterArgs.Add(LoadPort.Carrier.Id);
                return parameterArgs;
            }

            // Add error level (if defined)
            if (!string.IsNullOrWhiteSpace(error.Type))
            {
                parameterArgs.Add(error.Type);
            }

            // Add error code (if defined)
            if (!string.IsNullOrWhiteSpace(error.Code))
            {
                parameterArgs.Add(error.Code);
            }

            // Add error message (if defined)
            if (!string.IsNullOrWhiteSpace(error.Description))
            {
                parameterArgs.Add(error.Description);
            }

            return parameterArgs;
        }
    }
}
