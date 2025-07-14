using System;
using System.Threading;
using System.Threading.Tasks;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.Saliences;
using Agileo.ProcessingFramework.Instructions;

using Humanizer;

using UnitySC.GUI.Common.Vendor.ProcessExecution.TaskCompletion;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios;

namespace UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions
{
    public enum UserInteractionCommands
    {
        Continue,
        AbortContinue
    }

    public class UserInteractionProcessInstruction : DelayedInstruction, IUserInterfaceInstruction
    {
        #region Implementation of IUserInterfaceInstruction

        public BusinessPanel TargetedBusinessPanel { get; set; }

        #endregion

        #region Fields

        private readonly string _message;
        private readonly UserInteractionCommands _commands;

        private Popup _popup;

        #endregion

        public UserInteractionProcessInstruction(string message, UserInteractionCommands commands, TimeSpan? delay) : base(delay)
        {
            _message = message;
            _commands = commands;
        }

        #region Overrides of ManipulableInstruction

        public override void Execute()
        {
            if (TargetedBusinessPanel == null) throw new InvalidOperationException($"Business panel not specified to display {nameof(UserInteractionProcessInstruction).Humanize()}.");

            TaskCompletion = new TaskCompletionSource<TaskCompletionInfos>();

            if (Delay.HasValue)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(Delay.Value);
                    TaskCompletion.TrySetResult(TaskCompletionInfos.TimeOutDetected);
                });
            }

            TargetedBusinessPanel?.Saliences.Add(SalienceType.UserAttention);

            _popup = new Popup(new LocalizableText(nameof(ProcessInstructionsResources.USER_INTERACTION_INSTRUCTION_VALIDATION)), new InvariantText(_message));

            if (_commands == UserInteractionCommands.AbortContinue)
            {
                _popup.Commands.Add(new PopupCommand(nameof(ScenarioResources.SCENARIO_ABORT), new DelegateCommand(() =>
                {
                    TaskCompletion.TrySetResult(TaskCompletionInfos.Aborted);
                })));
            }

            _popup.Commands.Add(new PopupCommand(nameof(ScenarioResources.SCENARIO_CONTINUE), new DelegateCommand(() =>
            {
                TaskCompletion.TrySetResult(TaskCompletionInfos.Executed);
            })));

            TargetedBusinessPanel.Popups.Show(_popup);

            TaskCompletion.Task.Wait();

            TargetedBusinessPanel?.Saliences.Remove(SalienceType.UserAttention);

            if (TaskCompletion.Task.Result == TaskCompletionInfos.Aborted)
            {
                AbortByException(new Exception("Execution aborted by the operator"));
            }

            if (TaskCompletion.Task.Result == TaskCompletionInfos.TimeOutDetected)
            {
                AbortByException(new Exception("Interaction not validated by the operator"));
            }
        }

        private void AbortByException(Exception exception)
        {
            OnExecutionEnded();
            IsIntructionAborted = true;
            throw exception;
        }

        #region Overrides of ManipulableInstruction

        protected override void OnExecutionEnded()
        {
            _popup?.Close();
        }

        #endregion

        protected override Instruction CloneInstruction()
        {
            return new UserInteractionProcessInstruction(_message, _commands, Delay)
            {
                Details = Details,
                ExecutorId = ExecutorId,
                Modifier = Modifier,
                FormattedLabel = FormattedLabel
            };
        }

        #endregion

        #region Overrides of ProcessInstruction

        protected override void LocalizeName()
        {
            Name = LocalizationManager.GetString(nameof(ProcessInstructionsResources.USER_INTERACTION_INSTRUCTION));
        }

        #endregion
    }
}
