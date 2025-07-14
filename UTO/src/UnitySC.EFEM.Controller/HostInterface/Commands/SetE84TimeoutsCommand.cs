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
    public class SetE84TimeoutsCommand : BaseCommand
    {
        public SetE84TimeoutsCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.SetTimeoutsE84, sender, eqFacade, logger, equipmentManager)
        {
        }

        public SetE84TimeoutsCommand(
            Message message,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(message, sender, eqFacade, logger, equipmentManager)
        {
        }

        protected override bool TreatOrder(Message message)
        {
            // Check number of parameters
            if (message.CommandParameters.Count != 1 || message.CommandParameters[0].Length != 5)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Check TP1 (syntax only)
            if (!uint.TryParse(message.CommandParameters[0][0], out uint tp1))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Check TP2 (syntax only)
            if (!uint.TryParse(message.CommandParameters[0][1], out uint tp2))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Check TP3 (syntax only)
            if (!uint.TryParse(message.CommandParameters[0][2], out uint tp3))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Check TP4 (syntax only)
            if (!uint.TryParse(message.CommandParameters[0][3], out uint tp4))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Check TP5 (syntax only)
            if (!uint.TryParse(message.CommandParameters[0][4], out uint tp5))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // Actually check if command can be executed on all LoadPorts
            foreach (var loadPort in EquipmentManager.LoadPorts.Values)
            {
                if (!loadPort.CanExecute(
                        nameof(ILoadPort.SetE84Timeouts),
                        out CommandContext context,
                        tp1, tp2, tp3, tp4, tp5))
                {
                    bool shouldBypassCancellation = false;
                    foreach (var error in context?.Errors ?? new List<string>())
                    {
                        // Log the error so we can determine later why Host command is cancelled
                        if (!string.IsNullOrEmpty(error))
                        {
                            Logger.Error(error);
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
                        SendCancellation(message, Constants.Errors[ErrorCode.ErrorOccurredState]);
                        return false;
                    }
                }
            }

            // Send acknowledge response (i.e. command ok, will be performed)
            //SendAcknowledge(message);

            // Start command asynchronously and send completion event when done
            Task.Factory.StartNew(
                delegate
                {
                    foreach (var loadPort in EquipmentManager.LoadPorts.Values)
                    {
                        loadPort.SetE84Timeouts((int)tp1, (int)tp2, (int)tp3, (int)tp4, (int)tp5);
                    }
                }).ContinueWith(antecedent => SendAcknowledge(message));

            return true;
        }
    }
}
