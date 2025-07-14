using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware
{
    public class TestHardwareViewModel : ObservableObject, IMenuContentViewModel
    {
        private ControllersSupervisor _controllersSupervisor;
        public bool IsEnabled => true;

        private bool _canCloseState = true;

        public bool CanCloseState
        {
            get => _canCloseState; set { if (_canCloseState != value) { _canCloseState = value; OnPropertyChanged(); } }
        }

        public void Refresh()
        {
            Init();
           _controllersSupervisor.StartIoRefreshTask();
        }

        public bool CanClose()
        {
            if (!CanCloseState)
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Can't close");
            else
            {
                _controllersSupervisor.StopIoRefreshTask();
                ClassLocator.Default.GetInstance<CamerasSupervisor>().StopAllStreaming();
            }

            return CanCloseState;
        }

        private ICollectionView _collectionMessages;
        private List<Message> _messages = new List<Message>();

        public TestHardwareViewModel()
        {
            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<GlobalStatus>(this, (r, m) => GlobalStatusChanged(m));
            _controllersSupervisor = ClassLocator.Default.GetInstance<ControllersSupervisor>();
            _controllersSupervisor.Init();
            Application.Current.Dispatcher.BeginInvoke(new Action(() => Init()));
        }

        private bool ErrorsOnlyMessagesFilter(object message)
        {
            var messageToFilter = message as Message;
            if ((messageToFilter.Level == MessageLevel.Error) || (messageToFilter.Level == MessageLevel.Fatal) || (messageToFilter.Level == MessageLevel.Warning))
                return true;

            return false;
        }

        public ICollectionView Messages
        {
            get { return _collectionMessages; }
        }

        private void GlobalStatusChanged(GlobalStatus globalStatus)
        {
            // If initialization is terminated
            if (globalStatus.CurrentState != PMGlobalStates.Initializing && globalStatus.CurrentState != PMGlobalStates.NotInitialized)
            {
                InitialisationDone = true;
            }
            if (globalStatus.CurrentState == PMGlobalStates.Error)
                ErrorMessageToDisplay = globalStatus?.Messages?.FirstOrDefault()?.UserContent;
            else
                MessageToDisplay = globalStatus?.Messages?.FirstOrDefault()?.UserContent;
        }

        private void Init()
        {
            _collectionMessages = CollectionViewSource.GetDefaultView(_messages);
            _collectionMessages.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
            _collectionMessages.Filter += new Predicate<object>(ErrorsOnlyMessagesFilter);
        }

        private string _messageToDisplay = string.Empty;

        public string MessageToDisplay
        {
            get
            {
                return _messageToDisplay;
            }

            set
            {
                if (_messageToDisplay == value)
                {
                    return;
                }

                _messageToDisplay = value;
                OnPropertyChanged(nameof(MessageToDisplay));
            }
        }

        private string _errorMessageToDisplay = string.Empty;

        public string ErrorMessageToDisplay
        {
            get
            {
                return _errorMessageToDisplay;
            }

            set
            {
                if (_errorMessageToDisplay == value)
                {
                    return;
                }

                _errorMessageToDisplay = value;
                OnPropertyChanged(nameof(ErrorMessageToDisplay));
            }
        }

        private bool _initialisationDone = false;

        public bool InitialisationDone
        {
            get
            {
                return _initialisationDone;
            }

            set
            {
                if (_initialisationDone == value)
                {
                    return;
                }

                _initialisationDone = value;
                OnPropertyChanged(nameof(InitialisationDone));
            }
        }
    }
}
