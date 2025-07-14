using System;
using System.Threading.Tasks;

using Agileo.ProcessingFramework.Instructions;

using UnitySC.GUI.Common.Vendor.ProcessExecution.TaskCompletion;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Interface;

namespace UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions
{
    public abstract class DelayedInstruction : ProcessInstruction, IInstructionAbortable, ISkippableInstruction
    {
        protected TaskCompletionSource<TaskCompletionInfos> TaskCompletion;

        protected readonly TimeSpan? Delay;

        protected DelayedInstruction(TimeSpan? delay)
        {
            Delay = delay;
        }

        #region Implementation of IInstructionAbortable

        public virtual void AbortInstruction()
        {
            IsIntructionAborted = true;
            TaskCompletion.TrySetResult(TaskCompletionInfos.Aborted);
        }

        public bool IsIntructionAborted { get; protected set; }

        #endregion

        #region Implementation of ISkippableInstruction

        public virtual void Skip()
        {
            TaskCompletion.TrySetResult(TaskCompletionInfos.Stopped);
        }

        #endregion

        #region Overrides of Instruction

        public override void Execute()
        {
            TaskCompletion = new TaskCompletionSource<TaskCompletionInfos>();
            InternalExecute();

            if (Delay.HasValue)
            {
                TaskCompletion.Task.Wait(Delay.Value);
            }
            else
            {
                TaskCompletion.Task.Wait();
            }

            OnExecutionEnded();
        }

        protected virtual void InternalExecute()
        {

        }

        protected virtual void OnExecutionEnded()
        {

        }

        #endregion
    }
}
