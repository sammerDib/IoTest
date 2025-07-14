using System;
using System.Threading.Tasks;

using Agileo.Common.Localization;
using Agileo.ProcessingFramework.Instructions;

using UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions;

using UnitySC.GUI.Common.Vendor.ProcessExecution.TaskCompletion;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Interface;

namespace UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions
{
    public class WaitInstruction : ProcessInstruction, IInstructionAbortable, ISkippableInstruction
    {
        #region Field

        private TaskCompletionSource<TaskCompletionInfos> _tcs;
        private readonly TimeSpan _delay;

        #endregion Field

        #region Constructor

        public WaitInstruction(TimeSpan delay)
        {
            _delay = delay;
        }

        #endregion

        #region IInstructionAbortable

        public void AbortInstruction()
        {
            IsIntructionAborted = true;
            _tcs.TrySetResult(TaskCompletionInfos.Aborted);
        }

        public bool IsIntructionAborted { get; private set; }

        #endregion IInstructionAbortable

        #region IInstructionSkippable

        public void Skip()
        {
            _tcs.TrySetResult(TaskCompletionInfos.Stopped);
        }

        #endregion IInstructionSkippable

        #region Override Methods

        public override void Execute()
        {
            _tcs = new TaskCompletionSource<TaskCompletionInfos>();
            _tcs.Task.Wait(_delay);
        }

        protected override Instruction CloneInstruction()
        {
            return new WaitInstruction(_delay)
            {
                ExecutorId = ExecutorId,
                Modifier = Modifier,
                Details = Details,
                FormattedLabel = FormattedLabel
            };
        }

        protected override void LocalizeName()
        {
            Name = LocalizationManager.GetString(nameof(ProcessInstructionsResources.WAIT_PROCESS_INSTRUCTION));
        }

        #endregion Override Methods

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            base.Dispose(disposing);
            if (disposing)
            {
                _tcs?.TrySetResult(TaskCompletionInfos.Aborted);
            }
            _disposedValue = true;
        }

        #endregion IDisposable
    }
}
