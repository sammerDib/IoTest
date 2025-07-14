using System;
using System.Threading.Tasks;

using Agileo.Common.Localization;
using Agileo.ProcessingFramework.Instructions;

using UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Interface;

namespace UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions
{
    class WaitProcessInstruction : ProcessInstruction, IInstructionAbortable, ISkippableInstruction
    {
        #region Field

        private TaskCompletionSource<bool> _tcs;
        private readonly TimeSpan _delay;

        #endregion

        #region Properties

        public AppProgramProcessor AppProgramProcessor { get; set; }

        #endregion

        #region constructor

        public WaitProcessInstruction(TimeSpan delay)
        {
            _delay = delay;
        }

        #endregion

        public void AbortInstruction()
        {
            IsIntructionAborted = true;
            _tcs.TrySetResult(false);
        }

        public void Skip()
        {
            _tcs.TrySetResult(false);
        }

        public bool IsIntructionAborted { get; private set; }

        #region override

        public override void Execute()
        {
            AppProgramProcessor?.SetWaitInstructionElapsedTimer(true);
            _tcs = new TaskCompletionSource<bool>();
            _tcs.Task.Wait(_delay);
            AppProgramProcessor?.SetWaitInstructionElapsedTimer(false);
        }

        protected override void LocalizeName()
        {
            Name = LocalizationManager.GetString(nameof(ProcessInstructionsResources.WAIT_PROCESS_INSTRUCTION));
        }

        protected override Instruction CloneInstruction()
        {
            return new WaitProcessInstruction(_delay)
            {
                Details = Details,
                ExecutorId = ExecutorId,
                Modifier = Modifier,
                FormattedLabel = FormattedLabel
            };
        }

        #endregion
    }
}
