using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Agileo.Common.Localization;
using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.ProcessingFramework.Instructions;

using UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.GUI.Common.Vendor.ProcessExecution.TaskCompletion;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operand;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operators;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Interface;

namespace UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions
{
    class Operation
    {
        private readonly Func<bool> _function;

        public bool Result { get; private set; }

        public Operation(Func<bool> function)
        {
            _function = function;
            Result = false;
        }

        public void PerformOperation()
        {
            Result = _function();
        }
    }

    public class WaitStatusThresholdProcessInstruction : ProcessInstruction, IInstructionAbortable,
        ISkippableInstruction
    {
        #region Declaration

        private readonly WaitStatusThresholdInstruction _waitStatusThresholdInstruction;

        private TaskCompletionSource<TaskCompletionInfos> _tcs;

        private readonly List<Operation> _operations = new();

        private readonly ILogger _logger;

        private readonly Agileo.EquipmentModeling.Equipment _equipment;

        #endregion Declaration

        #region Constructor

        public WaitStatusThresholdProcessInstruction(
            Agileo.EquipmentModeling.Equipment equipment,
            WaitStatusThresholdInstruction recipeInstructions)
        {
            _equipment = equipment;
            _waitStatusThresholdInstruction = recipeInstructions;
            _logger = Agileo.Common.Logging.Logger.GetLogger(
                nameof(WaitStatusThresholdProcessInstruction));
        }

        #endregion Constructor

        #region Events

        private void Dev_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            var dev = sender as Device;

            var lstStatus = _waitStatusThresholdInstruction.Thresholds.OfType<ThresholdOperand>()
                .Where(o => o.DeviceName == dev?.Name && o.StatusName == e.Status.Name)
                .ToList();

            if (lstStatus.Count == 0)
            {
                return;
            }

            if (ControlOperation())
            {
                _tcs?.TrySetResult(TaskCompletionInfos.Executed);
            }
        }

        #endregion Events

        #region Private functions

        private bool ControlOperation()
        {
            foreach (var op in _operations)
            {
                op.PerformOperation();
            }

            int opIndex = 0;
            var isOperationOk = _operations[0].Result;

            foreach (var c in _waitStatusThresholdInstruction.Thresholds.OfType<ThresholdOperator>())
            {
                opIndex++;
                if (opIndex < _operations.Count)
                {
                    switch (c.Operator)
                    {
                        case LogicalOperator.And:
                            isOperationOk &= _operations[opIndex].Result;
                            break;
                        case LogicalOperator.Or:
                            isOperationOk |= _operations[opIndex].Result;
                            break;
                    }
                }
                else
                {
                    throw new ArgumentException("Index of operation incorrect");
                }
            }

            return isOperationOk;
        }

        private bool ControlQuantity(ThresholdOperand threshold)
        {
            var device = _equipment.GetTopDeviceContainer()
                .AllDevices()
                .FirstOrDefault(x => x.Name == threshold.DeviceName);
            switch (threshold)
            {
                case QuantityThresholdOperand quantityThreshold:
                    return CheckValue(device, quantityThreshold);
                case BooleanThresholdOperand booleanThreshold:
                    return CheckValue(device, booleanThreshold);
                case NumericThresholdOperand numericThreshold:
                    return CheckValue(device, numericThreshold);
                case StringThresholdOperand stringThreshold:
                    return CheckValue(device, stringThreshold);
                case EnumerableThresholdOperand enumThreshold:
                    return CheckValue(device, enumThreshold);

                default: return false;
            }
        }

        private bool CheckValue(Device device, QuantityThresholdOperand threshold)
        {
            var statusQuantityValue = (device?.GetStatusValue(threshold.StatusName) as IQuantity)?.Value;
            if (statusQuantityValue == null)
            {
                _logger.Warning(
                    "Quantity can't be define. StatusName {Status} is not of type IQuantity",
                    threshold.StatusName);
                return false;
            }

            var thresholdValue = threshold.Quantity.Value;

            switch (threshold.WaitingOperator)
            {
                case WaitingOperator.GreaterThan:
                    return statusQuantityValue > thresholdValue;
                case WaitingOperator.SmallerThan:
                    return statusQuantityValue < thresholdValue;
                case WaitingOperator.GreaterThanOrEqual:
                    return statusQuantityValue >= thresholdValue;
                case WaitingOperator.SmallerThanOrEqual:
                    return statusQuantityValue <= thresholdValue;
                case WaitingOperator.Equals:
                    return statusQuantityValue == thresholdValue;
                case WaitingOperator.NotEquals:
                    return statusQuantityValue != thresholdValue;
                default:
                    throw new InvalidEnumArgumentException(
                        nameof(threshold.WaitingOperator),
                        (int)threshold.WaitingOperator,
                        typeof(WaitingOperator));
            }
        }

        private bool CheckValue(Device device, BooleanThresholdOperand threshold)
        {
            var statusValue = device.GetStatusValue(threshold.StatusName);
            if (statusValue == null)
            {
                _logger.Warning("StatusName {Status} does not exists", threshold.StatusName);
                return false;
            }

            var statusBoolValue = (bool)statusValue;
            var thresholdValue = threshold.Value;

            return statusBoolValue == thresholdValue;
        }

        private bool CheckValue(Device device, NumericThresholdOperand threshold)
        {
            var statusValue = device?.GetStatusValue(threshold.StatusName);
            if (statusValue == null)
            {
                _logger.Warning("StatusName {Status} does not exists", threshold.StatusName);
                return false;
            }
            var statusDoubleValue = (double)statusValue;
            var thresholdValue = threshold.Value;

            switch (threshold.WaitingOperator)
            {
                case WaitingOperator.GreaterThan:
                    return statusDoubleValue > thresholdValue;
                case WaitingOperator.SmallerThan:
                    return statusDoubleValue < thresholdValue;
                case WaitingOperator.GreaterThanOrEqual:
                    return statusDoubleValue >= thresholdValue;
                case WaitingOperator.SmallerThanOrEqual:
                    return statusDoubleValue <= thresholdValue;
                case WaitingOperator.Equals:
                    return statusDoubleValue == thresholdValue;
                case WaitingOperator.NotEquals:
                    return statusDoubleValue != thresholdValue;
                default:
                    throw new InvalidEnumArgumentException(
                        nameof(threshold.WaitingOperator),
                        (int)threshold.WaitingOperator,
                        typeof(WaitingOperator));
            }
        }

        private bool CheckValue(Device device, StringThresholdOperand threshold)
        {
            var statusValue = device?.GetStatusValue(threshold.StatusName);
            if (statusValue == null)
            {
                _logger.Warning("StatusName {Status} does not exists", threshold.StatusName);
                return false;
            }

            var statusStringValue = statusValue.ToString();
            var thresholdValue = threshold.Value;

            switch (threshold.WaitingOperator)
            {
                case WaitingOperator.NotEquals:
                    return statusStringValue != thresholdValue;
                case WaitingOperator.Equals:
                    return statusStringValue == thresholdValue;
                default:
                    throw new InvalidEnumArgumentException(
                        nameof(threshold.WaitingOperator),
                        (int)threshold.WaitingOperator,
                        typeof(WaitingOperator));
            }
        }

        private bool CheckValue(Device device, EnumerableThresholdOperand threshold)
        {
            var statusValue = device?.GetStatusValue(threshold.StatusName);
            if (statusValue == null)
            {
                _logger.Warning("StatusName {Status} does not exists", threshold.StatusName);
                return false;
            }
            var statusEnumValue = statusValue.ToString();
            var thresholdValue = threshold.Value;

            switch (threshold.WaitingOperator)
            {
                case WaitingOperator.NotEquals:
                    return statusEnumValue != thresholdValue;
                case WaitingOperator.Equals:
                    return statusEnumValue == thresholdValue;
                default:
                    throw new InvalidEnumArgumentException(
                        nameof(threshold.WaitingOperator),
                        (int)threshold.WaitingOperator,
                        typeof(WaitingOperator));
            }
        }

        private void ReleaseStatusChangedEvent()
        {
            foreach (var th in _waitStatusThresholdInstruction.Thresholds.OfType<ThresholdOperand>())
            {
                var dev = _equipment.AllDevices().FirstOrDefault(x => x.Name == th.DeviceName);
                if (dev != null)
                {
                    dev.StatusValueChanged -= Dev_StatusValueChanged;
                }
            }
        }

        #endregion Private functions

        #region Override

        public override void Execute()
        {
            _operations.Clear();

            foreach (var th in _waitStatusThresholdInstruction.Thresholds.OfType<ThresholdOperand>())
            {
                var dev = _equipment.AllDevices().FirstOrDefault(x => x.Name == th.DeviceName);
                if (dev != null)
                {
                    dev.StatusValueChanged += Dev_StatusValueChanged;
                }

                _operations.Add(new Operation(() => ControlQuantity(th)));
            }

            //invoke function for the first time
            if (!ControlOperation()) // && _waitStatusThresholdInstruction.IsTimeoutEnabled)
            {
                //Wait for condition ok
                _tcs = new TaskCompletionSource<TaskCompletionInfos>();
                var timeOut = _waitStatusThresholdInstruction.TimeOut.Milliseconds;
                var wea = new WaitEndOfContractHelper();
                wea.WaitEndOfContract("WaitStatusThreshold",
                    Duration.From(
                        _waitStatusThresholdInstruction.IsTimeoutEnabled ? timeOut : Timeout.Infinite,
                        DurationUnit.Millisecond), _tcs);
            }

            ReleaseStatusChangedEvent();
        }

        protected override Instruction CloneInstruction()
        {
            return new WaitStatusThresholdProcessInstruction(_equipment, _waitStatusThresholdInstruction)
            {
                ExecutorId = ExecutorId,
                Modifier = Modifier,
                Details = Details,
                FormattedLabel = FormattedLabel
            };
        }

        protected override void LocalizeName()
        {
            Name = LocalizationManager.GetString(nameof(ProcessInstructionsResources
                .WAIT_STATUS_THRESHOLD_INSTRUCTION));
        }

        #endregion Override

        #region IInstructionAbortable

        public void AbortInstruction()
        {
            IsIntructionAborted = true;
            _tcs?.TrySetResult(TaskCompletionInfos.Aborted);
        }

        public bool IsIntructionAborted { get; private set; }

        #endregion IInstructionAbortable

        #region IInstructionPausable

        public void Skip()
        {
            _tcs?.TrySetResult(TaskCompletionInfos.Stopped);
        }

        #endregion IInstructionPausable

        #region IDisposable Support

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    _tcs?.TrySetResult(TaskCompletionInfos.Aborted);

                    ReleaseStatusChangedEvent();
                }

                _disposedValue = true;
            }
        }

        #endregion
    }
}
