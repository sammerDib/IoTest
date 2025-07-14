using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using MvvmDialogs;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.Shared.UI.ViewModel
{

    public class ClearAllMessages
    {

    }

    public class ClearMessage
    {
        public Message MessageToClear { get; set; }
    }

    /// <summary>
    /// ViewModel to display user notification
    /// </summary>
    public class NotifierVM : ObservableObject, IModalDialogViewModel
    {
        public enum NotifierState { Disabled, Information, Warning, Error }

        private const int PopupShowDelay = 2; // Seconds
        private readonly object _messageLock = new object();
        private readonly object _messageLockCurrentState = new object();
        private readonly ObservableCollection<MessageVM> _messages = new ObservableCollection<MessageVM>();
        private readonly System.Windows.Threading.DispatcherTimer _timerPopup;
        private IDialogOwnerService _dialogService;
        private IMessenger _messenger;
    
        public IDialogOwnerService DialogService
        {
            get
            {
                if (_dialogService == null)
                    _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
                return _dialogService;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messenger"></param>
        public NotifierVM(IMessenger messenger)
        {
            _messenger= messenger;
            messenger.Register<Message>(this, (r, m) => AddMessage(m));
            MessagesCV = CollectionViewSource.GetDefaultView(_messages);
            MessagesCV.Filter = MessageFilter;
            _timerPopup = new System.Windows.Threading.DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, PopupShowDelay, 0)
            };
            _timerPopup.Tick += TimerPopup_Tick;
            CurrentState = NotifierState.Information;
        }

        /// <summary>
        /// Add new message
        /// </summary>
        /// <param name="messageService"></param>
        public void AddMessage(Message messageService)
        {
            lock (_messageLock)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var messageVm = new MessageVM(messageService);
                    var lastMessage = _messages.FirstOrDefault();
                    // Hide duplicate message
                    if (lastMessage != null)
                    {
                        if (lastMessage.Content == messageVm.Content)
                            return;
                    }
         
                    _messages.Insert(0, messageVm);
      
                    NotifyMessagesChange();
                    UpdateCurrentState(CurrentState != NotifierState.Disabled);
                    if (CurrentMessage == null && CurrentState != NotifierState.Disabled)
                    {
                        CurrentMessage = messageVm;
                        _timerPopup.Start();
                    }
                }));
            }
        }

        /// <summary>
        /// Nb unread message
        /// </summary>
        public int NbUnreadMessages => _messages.Count();

        /// <summary>
        /// Message collection vieww for filter
        /// </summary>
        private ICollectionView _messagesCV;

        public ICollectionView MessagesCV
        {
            get => _messagesCV; set { if (_messagesCV != value) { _messagesCV = value; OnPropertyChanged(); } }
        }

        private void NotifyMessagesChange()
        {
            OnPropertyChanged(nameof(NbUnreadMessages));
            ClearAllCommand.NotifyCanExecuteChanged();
        }

        private bool MessageFilter(object obj)
        {
            // [FDA] Todo add filter in message level
            return true;
        }

        /// <summary>
        /// Current popup message
        /// </summary>
        private MessageVM _currentMessage;

        public MessageVM CurrentMessage
        {
            get => _currentMessage; set { if (_currentMessage != value) { _currentMessage = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Popup notification
        /// </summary>
        public bool NotificationEnabled
        {
            get => _currentState != NotifierState.Disabled;
            set
            {
                UpdateCurrentState(value);
                if (!value)
                    CurrentMessage = null;
            }
        }

        private void UpdateCurrentState(bool notificationEnabled)
        {
            if (!notificationEnabled)
            {
                CurrentState = NotifierState.Disabled;
            }
            else
            {
                lock (_messageLockCurrentState)
                {
                    if (_messages.Any(x => x.Level == MessageLevel.Fatal || x.Level == MessageLevel.Error))
                        CurrentState = NotifierState.Error;
                    else if (_messages.Any(x => x.Level == MessageLevel.Warning))
                        CurrentState = NotifierState.Warning;
                    else CurrentState = NotifierState.Information;
                }
            }

            OnPropertyChanged(nameof(NotificationEnabled));
        }

        private NotifierState _currentState;

        public NotifierState CurrentState
        {
            get => _currentState; set { if (_currentState != value) { _currentState = value; OnPropertyChanged(); } }
        }

        private void TimerPopup_Tick(object sender, EventArgs e)
        {
            lock (_messageLock)
            {
                _timerPopup.Stop();
                CurrentMessage = null;
            }
        }

        #region Command

        private AutoRelayCommand _clearAllCommand;

        public AutoRelayCommand ClearAllCommand
        {
            get
            {
                return _clearAllCommand ?? (_clearAllCommand = new AutoRelayCommand(
              () =>
              {
                  lock (_messageLock)
                  {
                      _messages.Clear();
                      UpdateCurrentState(CurrentState != NotifierState.Disabled);
                      NotifyMessagesChange();
                      _messenger.Send(new ClearAllMessages());
                  }
              },
              () => { return _messages.Any(); }));
            }
        }

        private AutoRelayCommand _openCommand;

        public AutoRelayCommand OpenCommand
        {
            get
            {
                return _openCommand ?? (_openCommand = new AutoRelayCommand(
              () =>
              {
                  CurrentMessage = null;
                  DialogService.ShowDialog<NotifierWindow>(this);
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand<MessageVM> _removeCommand;

        public AutoRelayCommand<MessageVM> RemoveCommand
        {
            get
            {
                return _removeCommand ?? (_removeCommand = new AutoRelayCommand<MessageVM>(
              (message) =>
              {
                  _messages.Remove(message);
                  NotifyMessagesChange();
                  _messenger.Send(new ClearMessage() { MessageToClear=message.OriginalMessage});
              }));
            }
        }

        public bool? DialogResult => true;

        #endregion Command
    }

    /// <summary>
    /// Message ViewModel
    /// </summary>
    public class MessageVM : ObservableObject
    {
        public readonly Message OriginalMessage;
        public string Content => string.IsNullOrEmpty(OriginalMessage.Source) ? OriginalMessage.UserContent : string.Format("[{0}] {1}", OriginalMessage.Source, OriginalMessage.UserContent);
        public MessageLevel Level => OriginalMessage.Level;
        public DateTime Date { get; private set; }

        public string AdvancedContent { get; private set; }

        public MessageVM(Message message)
        {
            OriginalMessage = message;
            Date = message.Date;
            AdvancedContent = message.AdvancedContent;
        }
    }
}
