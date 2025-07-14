using System;
using System.Collections.Generic;

using Agileo.Common.Communication;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice;

using GenericDeviceMessages = UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice.Resources.Messages;

namespace UnitySC.EFEM.Controller.HostInterface.Commands
{
    /// <summary>
    /// Class responsible to handle the initialization command defined in
    /// EFEM Controller Comm Specs 211006.pdf §4.2.1 oINIT
    /// </summary>
    public class InitializeCommand : BaseCommand
    {
        #region Constructors

        public InitializeCommand(
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            ILogger logger,
            EfemEquipmentManager equipmentManager)
            : base(Constants.Commands.Initialize, sender, eqFacade, logger, equipmentManager)
        {
        }

        #endregion Constructors

        #region BaseCommand

        protected override bool TreatOrder(Message message)
        {
            if (App.EfemAppInstance.ControlState == ControlState.Local)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InMaintenanceMode]);
                return true;
            }

            // Check parameters to determine what should actually be done
            // Initialization by unit. If there is no parameter, all units initializations are performed.

            // Check number of parameters
            if (message.CommandParameters.Count > 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            // No parameter, we should initialize the entire EFEM
            if (message.CommandParameters.Count <= 0)
            {
                // Actually check if command can be executed
                if (!EquipmentManager.Controller.CanExecute(nameof(IGenericDevice.Initialize), out CommandContext context,
                    false))
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

                // Note: logs from previous Rorze controller show that only one acknowledge message is sent,
                // although each device send its own init done message
                // This is why we need to register to device's event
                EquipmentManager.Robot.CommandExecutionStateChanged   += Device_CommandExecutionStateChanged;
                EquipmentManager.Aligner.CommandExecutionStateChanged += Device_CommandExecutionStateChanged;

                foreach (var loadPort in EquipmentManager.LoadPorts.Values)
                {
                    loadPort.CommandExecutionStateChanged += Device_CommandExecutionStateChanged;
                }

                // Start command asynchronously, completion event will be sent for each device when done
                EquipmentManager.Controller.InitializeAsync(!GUI.Common.App.Instance.Config.UseWarmInit);

                return true;
            }

            // Check number of parameter's argument
            if (message.CommandParameters[0].Length != 1)
            {
                SendCancellation(message, Constants.Errors[ErrorCode.InvalidNumberOfParameters]);
                return true;
            }

            // Try to get which unit to initialize
            if (message.CommandParameters[0].Length != 1
                || !EnumHelpers.TryParseEnumValue(message.CommandParameters[0][0], out Constants.Unit unit))
            {
                SendCancellation(message, Constants.Errors[ErrorCode.AbnormalFormatOfCommand]);
                return true;
            }

            switch (unit)
            {
                case Constants.Unit.Aligner:
                    Initialize(EquipmentManager.Aligner, unit, message);
                    return true;

                case Constants.Unit.Robot:
                    Initialize(EquipmentManager.Robot, unit, message);
                    return true;

                case Constants.Unit.LP1:
                case Constants.Unit.LP2:
                case Constants.Unit.LP3:
                case Constants.Unit.LP4:
                    // Try to get loadport instance to initialize
                    if (!EquipmentManager.LoadPorts.TryGetValue(Constants.ToLoadPortId(unit), out LoadPort loadPort))
                    {
                        SendCancellation(message, Constants.Errors[ErrorCode.LoadPortDisable]);
                        return true;
                    }

                    Initialize(loadPort, unit, message);
                    return true;

                // ReSharper disable once UnreachableSwitchCaseDueToIntegerAnalysis
                default:
                    return false;
            }
        }

        #endregion BaseCommand

        /// <summary>
        /// Handle the initialization of one device:
        ///  - check that command can be executed and return cancel message if not
        ///  - command the device
        ///  - send end of command message, with error status if needed
        /// </summary>
        /// <param name="device"></param>
        /// <param name="unit"></param>
        /// <param name="message">The received message (that triggers the initialization)</param>
        /// <param name="shouldSendAck"></param>
        /// <returns>The initialization task started on the device; <see langword="null"/> when no command started.</returns>
        private void Initialize(
            GenericDevice device,
            Constants.Unit unit,
            Message message,
            bool shouldSendAck = true)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            bool mustForInit = !GUI.Common.App.Instance.Config.UseWarmInit;

            // Actually check if command can be executed
            if (!device.CanExecute(nameof(IGenericDevice.Initialize), out CommandContext context, mustForInit))
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
                    if (CancelIfIsNotBusyFailed(message, error, unit))
                    {
                        return;
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
                    switch (unit)
                    {
                        case Constants.Unit.Robot:
                            SendCancellation(message, Constants.Errors[ErrorCode.RobotError]);
                            break;

                        case Constants.Unit.LP1:
                        case Constants.Unit.LP2:
                        case Constants.Unit.LP3:
                        case Constants.Unit.LP4:
                            SendCancellation(message, Constants.Errors[ErrorCode.LoadPortError]);
                            break;

                        case Constants.Unit.Aligner:
                            SendCancellation(message, Constants.Errors[ErrorCode.AlignerError]);
                            break;
                    }

                    return;
                }
            }

            // If needed, send acknowledge response (i.e. command ok, will be performed)
            if (shouldSendAck)
            {
                SendAcknowledge(message);
            }

            // Everything ok, performs the command and send completion once done (done asynchronously to not block communication pipe)
            device.InitializeAsync(mustForInit).ContinueWith(antecedent => SendCommandResult(antecedent, (int)unit));
        }

        private void Device_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            if (e.Execution.Context.Command.Name.Equals(nameof(IGenericDevice.Initialize))
                && e.NewState is ExecutionState.Success or ExecutionState.Failed)
            {
                int unit = -1;

                if (sender is Robot robot)
                {
                    unit = (int)Constants.Unit.Robot;
                    robot.CommandExecutionStateChanged -= Device_CommandExecutionStateChanged;
                }
                else if (sender is Aligner aligner)
                {
                    unit = (int)Constants.Unit.Aligner;
                    aligner.CommandExecutionStateChanged -= Device_CommandExecutionStateChanged;
                }
                else if (sender is LoadPort loadPort)
                {
                    unit = (int)Constants.ToLoadPortUnit(loadPort.InstanceId);
                    loadPort.CommandExecutionStateChanged -= Device_CommandExecutionStateChanged;
                }

                SendCommandResult(e.NewState, unit);
            }
        }

        #region Check Preconditions

        private bool CancelIfIsNotBusyFailed(Message message, string error, Constants.Unit unit)
        {
            if (error != null && error.Contains(GenericDeviceMessages.AlreadyBusy))
            {
                switch (unit)
                {
                    case Constants.Unit.Robot:
                        SendCancellation(message, Constants.Errors[ErrorCode.RobotMoving]);
                        break;

                    case Constants.Unit.LP1:
                    case Constants.Unit.LP2:
                    case Constants.Unit.LP3:
                    case Constants.Unit.LP4:
                        SendCancellation(message, Constants.Errors[ErrorCode.LoadPortMoving]);
                        break;

                    case Constants.Unit.Aligner:
                        SendCancellation(message, Constants.Errors[ErrorCode.AlignerMoving]);
                        break;
                }

                return true;
            }

            return false;
        }

        #endregion Check Preconditions
    }
}
