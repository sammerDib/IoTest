using System.Collections.Generic;
using System.Threading.Tasks;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    public class ResetE84Command : LoadPortCommand
    {
        private readonly HostDriver _hostDriver;

        public ResetE84Command(
            Constants.Port loadPortId,
            IMacroCommandSender sender,
            HostDriver eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.ResetE84, loadPortId, sender, eqFacade, logger, equipmentManager)
        {
            _hostDriver = eqFacade;
        }

        protected override bool TreatOrder(Message message)
        {
            // Check if order applies to this command's load port
            if (base.TreatOrder(message))
            {
                // Message has been treated if treated by base and coming from expected LP
                return !WrongLp;
            }

            // Check number of parameters
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }
            
            int deviceId = (int)Constants.ToLoadPortUnit(LoadPortId);
            bool mustForInit = !GUI.Common.App.Instance.Config.UseWarmInit;

            // Actually check if command can be executed
            if (!LoadPort.CanExecute(
                nameof(ILoadPort.Initialize),
                out CommandContext context,
                mustForInit))
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
            //SendAcknowledge(message);

            // Start command asynchronously and send completion event when done
            Task.Factory.StartNew(
                () =>
                {
                    _hostDriver.IsE84EnabledOnLoadPorts[LoadPort.InstanceId] = false;

                    //Stops the current Load/Unload sequence
                    //HO_AVLB signal switches to false
                    LoadPort.DisableE84();

                    if (LoadPort.NeedsInitAfterE84Error())
                    {
                        LoadPort.Initialize(true);
                    }
                }).ContinueWith(_ => SendAcknowledge(message));

            return true;
        }
    }
}
