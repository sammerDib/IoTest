using System.Collections.Generic;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.Drivers;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;

using ErrorCode = UnitySC.EFEM.Controller.HostInterface.Enums.ErrorCode;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the enable or disable E84 command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.6.1 oSE84
    /// </summary>
    public class EnableOrDisableE84Command : LoadPortCommand
    {
        private byte _enableOrDisable;
        private readonly HostDriver _hostDriver;

        public EnableOrDisableE84Command(
            Constants.Port loadPortId,
            IMacroCommandSender sender,
            HostDriver eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.EnableOrDisableE84, loadPortId, sender, eqFacade, logger, equipmentManager)
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

            // Check if order applies to this command's load port
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

            // Command expects '1' for enable or '0' for disable
            if (!byte.TryParse(message.CommandParameters[0][1], out byte enableAsUint)
                || message.CommandParameters[0][1].Length > 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            if (enableAsUint > 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return true;
            }

            _enableOrDisable = enableAsUint;
            int deviceId = (int)Constants.ToLoadPortUnit(LoadPortId);

            // Actually check if command can be executed
            if (!LoadPort.CanExecute(
                enableAsUint == 1 ? nameof(ILoadPort.EnableE84) : nameof(ILoadPort.DisableE84),
                out CommandContext context))
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
                    if (CancelIfIsInServiceFailed(message, error))
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
                    SendCancellation(message, Constants.Errors[ErrorCode.LoadPortError]);
                    return true;
                }
            }

            // Send acknowledge response (i.e. command ok, will be performed)
            SendAcknowledge(message);

            // Start command asynchronously and send completion event when done
            Task.Factory.StartNew(
                () =>
                {
                    if (enableAsUint == 1)
                    {
                        _hostDriver.IsE84EnabledOnLoadPorts[LoadPort.InstanceId] = true;

                        //Activates the E84 flag on the load port
                        LoadPort.EnableE84();

                        //Start the Load or Unload sequence
                        //It activates the HO_AVLB signal
                        if (LoadPort.CarrierPresence == CassettePresence.Absent)
                        {
                            LoadPort.RequestLoad();
                        }
                        else if (LoadPort.CarrierPresence == CassettePresence.Correctly)
                        {
                            LoadPort.RequestUnload();
                        }
                        else
                        {
                            _hostDriver.ForceCarrierPresenceInCaseOfError(LoadPort);
                        }
                    }
                    else
                    {
                        _hostDriver.IsE84EnabledOnLoadPorts[LoadPort.InstanceId] = false;

                        //Stops the current Load/Unload sequence
                        //HO_AVLB signal switches to false
                        LoadPort.DisableE84();

                        if (LoadPort.NeedsInitAfterE84Error())
                        {
                            LoadPort.Initialize(true);
                        }
                    }
                }).ContinueWith(antecedent => SendCommandResult(antecedent, deviceId));

            return true;
        }

        protected override List<string> GetResultArguments(Error error = null)
        {
            // SE84 needs to repeat parameter.
            var res = new List<string> { _enableOrDisable.ToString() };
            res.AddRange(base.GetResultArguments(error));
            return res;
        }
    }
}
