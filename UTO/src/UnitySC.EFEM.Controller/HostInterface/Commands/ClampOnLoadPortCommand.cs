using System.Collections.Generic;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;

using ErrorCode = UnitySC.EFEM.Controller.HostInterface.Enums.ErrorCode;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the clamp/unclamp carrier command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.3.6 oLPCP
    /// </summary>
    public class ClampOnLoadPortCommand : LoadPortCommand
    {
        private readonly HostDriver _hostDriver;

        public ClampOnLoadPortCommand(
            Constants.Port loadPortId,
            IMacroCommandSender sender,
            HostDriver eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.ClampOnLp, loadPortId, sender, eqFacade, logger, equipmentManager)
        {
            _hostDriver = eqFacade;
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
            if (!uint.TryParse(message.CommandParameters[0][1], out uint isClampAsUint))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalParameter]);
                return true;
            }

            // Check second parameter (range validity)
            if (isClampAsUint > 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            var isClamp  = isClampAsUint == 1;
            int deviceId = (int)Constants.ToLoadPortUnit(LoadPortId);

            // Actually check if command can be executed
            if (!LoadPort.CanExecute(
                isClamp ? nameof(ILoadPort.Clamp) : nameof(ILoadPort.ReleaseCarrier),
                out CommandContext context)
                && !CanByPassPreconditionOnE84Error(isClamp))
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

            // Start command asynchronously and send completion event when done
            if (isClamp)
            {
                Task.Factory.StartNew(
                    () =>
                    {
                        if (LoadPort.AccessMode == LoadingType.Auto)
                        {
                            _hostDriver.IsE84EnabledOnLoadPorts[LoadPort.InstanceId] = false;
                            LoadPort.DisableE84();
                        }

                        if (LoadPort.NeedsInitAfterE84Error())
                        {
                            LoadPort.Initialize(true);
                        }

                        LoadPort.Clamp();
                    }).ContinueWith(antecedent => SendCommandResult(antecedent, deviceId));
            }
            else
            {
                LoadPort.ReleaseCarrierAsync().ContinueWith(antecedent => SendCommandResult(antecedent, deviceId));
            }

            return true;
        }

        private bool CanByPassPreconditionOnE84Error(bool isClamp)
        {
            if (!isClamp)
            {
                return false;
            }

            if (LoadPort is RV201 rv201lp
                && (rv201lp.ErrorCode == EFEM.Rorze.Devices.LoadPort.RV201.Constants.E84Errors.Tp5Timeout
                    || rv201lp.ErrorCode == EFEM.Rorze.Devices.LoadPort.RV201.Constants.E84Errors.CarrierImproperlyPlaced)
                && LoadPort.CarrierPresence == CassettePresence.Correctly)
            {
                return true;
            }

            return false;
        }
    }
}
