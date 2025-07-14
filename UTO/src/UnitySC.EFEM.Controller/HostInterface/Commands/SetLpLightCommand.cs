using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the set loadport light command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.3.7 oLPLO
    /// </summary>
    public class SetLpLightCommand : LoadPortCommand
    {
        public SetLpLightCommand(
            Constants.Port loadPortId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.SetLightOnLp, loadPortId, sender, eqFacade, logger, equipmentManager)
        {
        }

        protected override bool TreatOrder(Message message)
        {
            if (base.TreatOrder(message))
            {
                // Message has been treated if treated by base and coming from expected LP
                return !WrongLp;
            }

            // Check number of parameters
            if (message.CommandParameters[0].Length != 4)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            var lightStates = new Dictionary<LoadPortLightRoleType, Agileo.SemiDefinitions.LightState>
            {
                { LoadPortLightRoleType.LoadReady,        Agileo.SemiDefinitions.LightState.Undetermined },
                { LoadPortLightRoleType.UnloadReady,      Agileo.SemiDefinitions.LightState.Undetermined },
                { LoadPortLightRoleType.AccessModeManual, Agileo.SemiDefinitions.LightState.Undetermined }
            };

            // Check parameters validity
            for (int i = 1; i < 4; i++)
            {
                LoadPortLightRoleType light = i switch
                {
                    1 => LoadPortLightRoleType.LoadReady,
                    2 => LoadPortLightRoleType.UnloadReady,
                    3 => LoadPortLightRoleType.AccessModeManual,
                    _ => LoadPortLightRoleType.Undetermined
                };

                var lightParam     = message.CommandParameters[0][i];
                var result         = CheckLightParameterValidity(message, lightParam, out uint lightStateAsUint);
                lightStates[light] = lightStateAsUint == 1
                    ? Agileo.SemiDefinitions.LightState.On
                    : Agileo.SemiDefinitions.LightState.Off;

                // Actually check if command can be executed
                result |= CheckCanExecute(message, light, lightStates[light]);

                if (!result)
                {
                    return true;
                }
            }

            // Send acknowledge response (i.e. command ok, will be performed)
            SendAcknowledge(message);

            // Command each lights
            foreach (var kvp in lightStates)
            {
                if (kvp.Value != Agileo.SemiDefinitions.LightState.Undetermined)
                {
                    LoadPort.SetLightAsync(kvp.Key, kvp.Value);
                }
            }

            return true;
        }

        private bool CheckLightParameterValidity(Message message, string lightParam, out uint lightStateAsUint)
        {
            lightStateAsUint = 0;
            if (lightParam.Length != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return false;
            }

            if (!uint.TryParse(lightParam, out uint tmpLightStateAsUint))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return false;
            }

            if (tmpLightStateAsUint > 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalRangeOfParameter]);
                return false;
            }

            lightStateAsUint = tmpLightStateAsUint;
            return true;
        }

        private bool CheckCanExecute(
            Message message,
            LoadPortLightRoleType role,
            Agileo.SemiDefinitions.LightState lightState)
        {
            if (!LoadPort.CanExecute(nameof(ILoadPort.SetLight), out CommandContext context, role, lightState))
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
                        return false;
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
                    return false;
                }
            }

            return true;
        }
    }
}
