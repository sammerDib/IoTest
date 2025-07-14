using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Client.Modules.TestHardware.ViewModel;
using UnitySC.PM.DMT.Client.Proxy.Chamber;
using UnitySC.PM.DMT.Client.Proxy.Chuck;
using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.Shared.Hardware.ClientProxy.Axes;
using UnitySC.PM.Shared.Hardware.ClientProxy.Ffu;
using UnitySC.PM.Shared.Hardware.ClientProxy.Plc;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware
{
    public class TestHardwareViewModel : ObservableObject, IMenuContentViewModel
    {
        public bool IsEnabled => true;

        private bool _canCloseState = true;

        public bool CanCloseState
        {
            get => _canCloseState; set { if (_canCloseState != value) { _canCloseState = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<object> TabItems { get; }

        public void Refresh()
        {
            Init();
        }

        public bool CanClose()
        {
            if (CanCloseState)
            {
                _collectionMessages.Filter -= new Predicate<object>(ErrorsOnlyMessagesFilter);
                if (_selectedTab != null)
                {
                    _selectedTab.Hide();
                }
            }
            return CanCloseState;
        }

        private ICollectionView _collectionMessages;
        private readonly List<Message> _messages = new List<Message>();

        public TestHardwareViewModel(CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor, AlgorithmsSupervisor algorithmsSupervisor,
            ChamberSupervisor chamberSupervisor, PlcSupervisor plcSupervisor, ChuckSupervisor chuckSupervisor, MotionAxesSupervisor motionAxesSupervisor, FfuSupervisor ffuSupervisor, IDialogOwnerService dialogService, IMessenger messenger)
        {
            TabItems = new ObservableCollection<object>
            {
                new StageViewModel(motionAxesSupervisor),
                new TestCameraScreenVM(cameraSupervisor, screenSupervisor, algorithmsSupervisor, dialogService, messenger),
                new OverviewChamberVM(chamberSupervisor, chuckSupervisor, plcSupervisor, ffuSupervisor),
                new LuminanceScreensViewModel(Side.Front,screenSupervisor, dialogService)
                // Add more view models as needed
            };
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

        private void Init()
        {
            if (_selectedTab != null)
            {
                _selectedTab.Display();
            }
            _collectionMessages = CollectionViewSource.GetDefaultView(_messages);
            _collectionMessages.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
            _collectionMessages.Filter += new Predicate<object>(ErrorsOnlyMessagesFilter);
        }

        private ITabManager _selectedTab;

        public ITabManager SelectedTab
        {
            get => _selectedTab;

            set
            {
                if (_selectedTab == value)
                {
                    return;
                }

                if (_selectedTab != null)
                {
                    if (!_selectedTab.CanHide())
                    {
                        return;
                    }
                    _selectedTab.Hide();
                }

                _selectedTab = value;
                _selectedTab.Display();

                OnPropertyChanged();
            }
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
