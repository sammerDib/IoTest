using System;
using System.Threading.Tasks;

using Agileo.Common.Localization;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.ProcessingFramework.Instructions;

using Humanizer;

using UnitySC.GUI.Common.Vendor.ProcessExecution.TaskCompletion;

namespace UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions
{
    public class UserInformationProcessInstruction : DelayedInstruction, IUserInterfaceInstruction
    {
        #region Implementation of IUserInterfaceInstruction

        public BusinessPanel TargetedBusinessPanel { get; set; }

        #endregion

        #region Field

        private readonly string _message;
        private readonly TimeSpan? _autoHideDelay;

        #endregion Field

        #region constructor

        public UserInformationProcessInstruction(string message, TimeSpan? autoHideDelay) : base(null)
        {
            _message = message;
            _autoHideDelay = autoHideDelay;
        }

        #endregion constructor

        #region override

        protected override void InternalExecute()
        {
            if (TargetedBusinessPanel == null) throw new InvalidOperationException($"Business panel not specified to display {nameof(UserInformationProcessInstruction).Humanize()}.");

            TargetedBusinessPanel.Messages.HideAll();

            var userMessage = new UserMessage(MessageLevel.Info, new InvariantText(_message))
            {
                CanUserCloseMessage = true
            };
            TargetedBusinessPanel.Messages.Show(userMessage);

            // Keeps the reference of the host panel and automatically hides the message once the delay has elapsed.
            var targetedBusinessPanel = TargetedBusinessPanel;
            if (_autoHideDelay.HasValue) Task.Delay(_autoHideDelay.Value).ContinueWith(_ => targetedBusinessPanel?.Messages.Hide(userMessage));

            TaskCompletion.TrySetResult(TaskCompletionInfos.Executed);
        }

        protected override void LocalizeName()
        {
            Name = LocalizationManager.GetString(nameof(ProcessInstructionsResources.USER_MESSAGE_PROCESS_INSTRUCTION));
        }

        protected override Instruction CloneInstruction()
        {
            return new UserInformationProcessInstruction(_message, _autoHideDelay)
            {
                Details = Details,
                ExecutorId = ExecutorId,
                Modifier = Modifier,
                FormattedLabel = FormattedLabel
            };
        }

        #endregion override
    }
}
