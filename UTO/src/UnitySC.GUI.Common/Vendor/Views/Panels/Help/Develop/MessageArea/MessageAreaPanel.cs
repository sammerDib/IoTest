using System.Collections.ObjectModel;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.Popup;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.MessageArea
{
    public class MessageAreaPanel : BusinessPanel
    {
        public MessageAreaPanel() : this("Message Area", IconFactory.PathGeometryFromRessourceKey("MessageIcon"))
        {
        }

        public MessageAreaPanel(string id, IIcon icon = null) : base(id, icon)
        {
        }

        private UserMessage BuildUserMessage()
        {
            var userMessage = new UserMessage(MessageLevel, new InvariantText(Message))
            {
                SecondsDuration = UseDuration ? SecondsDuration : null,
                CanUserCloseMessage = CanUserCloseMessage,
                Icon = UseIcon ? PathIcon.Agileo : null,
            };

            foreach (var messageCommand in MessageCommands)
            {
                userMessage.Commands.Add(new UserMessageCommand(messageCommand.Value)
                {
                    CloseMessageAfterExecute = true
                });
            }

            return userMessage;
        }


        #region Properties

        public UserMessageDisplayer CustomUserMessagesMessages { get; } = new();
        
        private MessageLevel _messageLevel = MessageLevel.Info;

        public MessageLevel MessageLevel
        {
            get => _messageLevel;
            set => SetAndRaiseIfChanged(ref _messageLevel, value);
        }

        private string _message = "Lorem ipsum, nuntius semper magis narrans cum bene indutus est.";

        public string Message
        {
            get => _message;
            set => SetAndRaiseIfChanged(ref _message, value);
        }
        
        private bool _useDuration;

        public bool UseDuration
        {
            get => _useDuration;
            set => SetAndRaiseIfChanged(ref _useDuration, value);
        }

        private int _secondsDuration = 5;

        public int SecondsDuration
        {
            get => _secondsDuration;
            set => SetAndRaiseIfChanged(ref _secondsDuration, value);
        }

        private bool _canUserCloseMessage;

        public bool CanUserCloseMessage
        {
            get => _canUserCloseMessage;
            set => SetAndRaiseIfChanged(ref _canUserCloseMessage, value);
        }

        private bool _useIcon;

        public bool UseIcon
        {
            get => _useIcon;
            set => SetAndRaiseIfChanged(ref _useIcon, value);
        }

        public ObservableCollection<StringWrapper> MessageCommands { get; } = new()
        {
            new StringWrapper("Cancel")
        };

        #endregion Properties

        #region Commands

        private ICommand _setMessageLevelCommand;

        public ICommand SetMessageLevelCommand => _setMessageLevelCommand ??= new DelegateCommand<object>(SetMessageLevelCommandExecute);

        private void SetMessageLevelCommandExecute(object arg)
        {
            if (arg is MessageLevel level)
            {
                MessageLevel = level;
            }
        }

        private ICommand _showOnUserInterfaceCommand;

        public ICommand ShowOnUserInterfaceCommand => _showOnUserInterfaceCommand ??= new DelegateCommand(ShowOnUserInterfaceCommandExecute);

        private void ShowOnUserInterfaceCommandExecute()
        {
            App.Instance.UserInterface.Messages.Show(BuildUserMessage());
        }

        private ICommand _showOnBusinessPanelCommand;

        public ICommand ShowOnBusinessPanelCommand => _showOnBusinessPanelCommand ??= new DelegateCommand(ShowOnBusinessPanelCommandExecute);

        private void ShowOnBusinessPanelCommandExecute()
        {
            Messages.Show(BuildUserMessage());
        }

        private ICommand _showOnCustomAreaCommand;

        public ICommand ShowOnCustomAreaCommand => _showOnCustomAreaCommand ??= new DelegateCommand(ShowOnCustomAreaCommandExecute);

        private void ShowOnCustomAreaCommandExecute()
        {
            CustomUserMessagesMessages.Show(BuildUserMessage());
        }

        private ICommand _clearUserInterfaceMessagesCommand;

        public ICommand ClearUserInterfaceMessagesCommand => _clearUserInterfaceMessagesCommand ??= new DelegateCommand(ClearUserInterfaceMessagesCommandExecute, ClearUserInterfaceMessagesCommandCanExecute);

        private bool ClearUserInterfaceMessagesCommandCanExecute()
        {
            return App.Instance?.UserInterface?.Messages?.DisplayedUserMessage != null;
        }

        private void ClearUserInterfaceMessagesCommandExecute()
        {
            App.Instance.UserInterface.Messages.HideAll();
        }

        private ICommand _clearBusinessPanelMessagesCommand;

        public ICommand ClearBusinessPanelMessagesCommand => _clearBusinessPanelMessagesCommand ??= new DelegateCommand(ClearBusinessPanelMessagesCommandExecute, ClearBusinessPanelMessagesCommandCanExecute);

        private bool ClearBusinessPanelMessagesCommandCanExecute()
        {
            return Messages.DisplayedUserMessage != null;
        }

        private void ClearBusinessPanelMessagesCommandExecute()
        {
            Messages.HideAll();
        }

        private ICommand _clearCustomAreaMessagesCommand;

        public ICommand ClearCustomAreaMessagesCommand => _clearCustomAreaMessagesCommand ??= new DelegateCommand(ClearCustomAreaMessagesCommandExecute, ClearCustomAreaMessagesCommandCanExecute);

        private bool ClearCustomAreaMessagesCommandCanExecute()
        {
            return CustomUserMessagesMessages.DisplayedUserMessage != null;
        }

        private void ClearCustomAreaMessagesCommandExecute()
        {
            CustomUserMessagesMessages.HideAll();
        }

        private ICommand _addCommandCommand;

        public ICommand AddCommandCommand => _addCommandCommand ??= new DelegateCommand(AddCommandCommandExecute);

        private void AddCommandCommandExecute() => MessageCommands.Add(new StringWrapper($"Command {MessageCommands.Count + 1}"));

        private ICommand _removeCommandCommand;

        public ICommand RemoveCommandCommand => _removeCommandCommand ??= new DelegateCommand<StringWrapper>(RemoveCommandCommandExecute);

        private void RemoveCommandCommandExecute(StringWrapper arg) => MessageCommands.Remove(arg);

        #endregion
    }
}
