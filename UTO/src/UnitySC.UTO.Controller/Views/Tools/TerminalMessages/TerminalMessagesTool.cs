using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Components.Tools;
using Agileo.GUI.Services.Saliences;
using Agileo.Semi.Gem.Abstractions.E30;

using UnitySC.GUI.Common.Vendor;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.UTO.Controller.Views.Tools.TerminalMessages
{
    public class TerminalMessagesTool : Tool
    {
        #region Properties

        public ObservableCollection<TerminalMessageViewModel> Messages { get; } = new ObservableCollection<TerminalMessageViewModel>();

        private string _messageToSend;

        public string MessageToSend
        {
            get { return _messageToSend; }
            set { SetAndRaiseIfChanged(ref _messageToSend, value); }
        }

        private LocalizableText CurrentUserName
        {
            get
            {
                if (IsInDesignMode) return "User";
                var currentUserName = AgilControllerApplication.Current.AccessRights.CurrentUser?.Name;
                return string.IsNullOrEmpty(currentUserName) ? nameof(TerminalMessagesToolResource.TERMINAL_MESSAGE_UNLOGGED_USER) : currentUserName;
            }
        }

        private BusinessPanel FirstAccessibleBusinessPanelWithTool
            => AgilControllerApplication.Current?.UserInterface?.BusinessPanels?.FirstOrDefault(
                businessPanel => businessPanel.AssociatedTools.Contains(this)
                                 && businessPanel.CanNavigateTo);

        #endregion

        #region ctor

        static TerminalMessagesTool()
        {
            DataTemplateGenerator.Create(typeof(TerminalMessagesTool), typeof(TerminalMessagesToolView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(TerminalMessagesToolResource)));
        }

        public TerminalMessagesTool() : base(nameof(TerminalMessagesToolResource.TERMINAL_MESSAGES), PathIcon.Sms)
        {
            DisplayZone = VerticalAlignment.Bottom;
        }

        #endregion

        #region Commands

        private DelegateCommand _sendMessageCommand;

        public DelegateCommand SendMessageCommand => _sendMessageCommand ?? (_sendMessageCommand = new DelegateCommand(SendMessageCommandExecute, SendMessageCommandCanExecute));

        private bool SendMessageCommandCanExecute()
        {
            if (IsInDesignMode) return true;
            if (!App.ControllerInstance.GemController.IsSetupDone) return false;
            return App.ControllerInstance.GemController.TerminalServices != null && !string.IsNullOrWhiteSpace(MessageToSend);
        }

        private void SendMessageCommandExecute()
        {
            // Auto acknowledgment on send message
            if (AcknowledgeCommandCanExecute())
            {
                AcknowledgeCommandExecute();
            }

            App.ControllerInstance.GemController.TerminalServices.SendTerminalMessage(MessageToSend);
            Messages.Add(new TerminalMessageViewModel(TerminalMessageSource.FromEquipment, CurrentUserName, DateTime.Now, MessageToSend));
            MessageToSend = string.Empty;
        }

        private DelegateCommand _acknowledgeCommand;

        public DelegateCommand AcknowledgeCommand => _acknowledgeCommand ?? (_acknowledgeCommand = new DelegateCommand(AcknowledgeCommandExecute, AcknowledgeCommandCanExecute));

        private bool AcknowledgeCommandCanExecute()
        {
            if (IsInDesignMode) return true;
            if (!App.ControllerInstance.GemController.IsSetupDone) return false;
            //Confirm can be executed if _gemTerminal exists and there is some  message not confirmed yet.
            return App.ControllerInstance.GemController.TerminalServices != null && Messages.Any(message => message.Source == TerminalMessageSource.FromHost && !message.Acknowledged);
        }

        private void AcknowledgeCommandExecute()
        {
            foreach (var msg in Messages.Where(message => message.Source == TerminalMessageSource.FromHost))
            {
                msg.Acknowledged = true;
            }

            App.ControllerInstance.GemController.TerminalServices.SendTerminalMessageRecognized();
        }

        #endregion

        #region Handlers

        private void TerminalServicesOnOnMessageReceived(object sender, TerminalMessageReceivedEventArgs e)
        {
            if (Application.Current != null && Thread.CurrentThread != Application.Current.Dispatcher.Thread)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => { TerminalServicesOnOnMessageReceived(sender, e); }));
                return;
            }

            var messageReceived = string.Join(Environment.NewLine, e.Values);

            var messagesToRemove = Messages.Where(message => message.Source == TerminalMessageSource.FromHost && !message.Acknowledged).ToList();
            foreach (var messageToRemove in messagesToRemove)
            {
                messageToRemove.Dispose();
                Messages.Remove(messageToRemove);
            }

            if (!string.IsNullOrEmpty(messageReceived))
            {
                Messages.Add(new TerminalMessageViewModel(
                    TerminalMessageSource.FromHost,
                    new LocalizableText(nameof(TerminalMessagesToolResource.TERMINAL_MESSAGE_HOST_NAME), e.TerminalNumber),
                    DateTime.Now,
                    messageReceived));
            }

            IsOpen = true;

            RefreshSaliences();
        }

        #endregion

        #region Salience Gesture

        private BusinessPanel _userAttentionTargetPanel;
        private int _addedSaliences;

        private void ClearAddedSaliences()
        {
            if (_userAttentionTargetPanel != null && _addedSaliences > 0)
            {
                for (int i = 0; i < _addedSaliences; i++)
                {
                    _userAttentionTargetPanel.Saliences.Remove(SalienceType.UserAttention);
                }
            }

            _userAttentionTargetPanel = null;
            _addedSaliences = 0;
        }

        private void RefreshSaliences()
        {
            ClearAddedSaliences();
            if (!AgilControllerApplication.Current.UserInterface.Navigation.SelectedBusinessPanel.AssociatedTools.Contains(this))
            {
                // It is important to store the businessPanel on which the saliences have been added
                // because the target businessPanel can change in case of user un-logging
                _userAttentionTargetPanel = FirstAccessibleBusinessPanelWithTool;
                if (_userAttentionTargetPanel != null)
                {
                    _addedSaliences = Messages.Count(message => message.Source == TerminalMessageSource.FromHost && !message.Acknowledged);
                    _userAttentionTargetPanel.Saliences.Add(SalienceType.UserAttention, _addedSaliences);
                }
            }
        }

        #endregion

        #region Overrides

        public override void OnSetup()
        {
            base.OnSetup();
            if (App.ControllerInstance.GemController != null && App.ControllerInstance.GemController.IsSetupDone)
            {
                App.ControllerInstance.GemController.TerminalServices.OnMessageReceived += TerminalServicesOnOnMessageReceived;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (App.ControllerInstance.GemController != null)
            {
                App.ControllerInstance.GemController.TerminalServices.OnMessageReceived -= TerminalServicesOnOnMessageReceived;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
