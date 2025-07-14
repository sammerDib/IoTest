using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

using UnitySC.PM.AGS.Modules.TestHardware.Interfaces;
using UnitySC.PM.AGS.Modules.TestHardware.ViewModel;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.AGS.Modules.TestHardware
{
    public class TestHardwareViewModel : ViewModelBase, IMenuContentViewModel, IDisposable
    {
        public string Header { get; protected set; }

        public bool IsEnabled => true;

        private bool _canCloseState = true;

        public bool CanCloseState
        {
            get => _canCloseState; set { if (_canCloseState != value) { _canCloseState = value; RaisePropertyChanged(); } }
        }

        public void Refresh()
        {
            Init();
        }

        private ICollectionView _collectionMessages;
        private List<Message> _messages = new List<Message>();

        public TestHardwareViewModel()
        {
            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<GlobalStatus>(this, GlobalStatusChanged);
            Application.Current.Dispatcher.BeginInvoke(new Action(() => Init()));

            _settings = new List<SettingVM>();

            _settings.Add(new StageViewModel());

            // Set Startup Page
            // SelectedViewModel = new StageViewModel();
            CreateAndAddSettings();
            SelectedSetting = _settings.Find(s => s.IsEnabled) as ITabManager;
        }

        private void CreateAndAddSettings()
        {
            Settings.Add(new CamerasViewModel());
            Settings.Add(new CameraPositionViewModel());
            Settings.Add(new ChamberViewModel());
            Settings.Add(new InputOutputViewModel());
        }

        private List<SettingVM> _settings;

        public List<SettingVM> Settings
        {
            get => _settings;
            set
            {
                if (_settings != value)
                {
                    _settings = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ITabManager _selectedSetting = null;

        public ITabManager SelectedSetting
        {
            get
            {
                return _selectedSetting;
            }

            set
            {
                if (_selectedSetting == value)
                {
                    return;
                }

                if (_selectedSetting != null)
                {
                    if (!_selectedSetting.CanHide())
                        return;
                    _selectedSetting.Hide();
                }
                _selectedSetting = value;
                if (_isVisible)
                    _selectedSetting.Display();
                RaisePropertyChanged(nameof(SelectedSetting));
            }
        }

        private bool _isVisible = false;

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                if (_isVisible == value)
                {
                    return;
                }

                _isVisible = value;

                if (_isVisible)
                {
                    if (_selectedSetting != null)
                    {
                        _selectedSetting.Display();
                    }
                }
                else
                {
                    if (_selectedSetting != null)
                    {
                        if (!_selectedSetting.CanHide())
                            return;
                        _selectedSetting.Hide();
                    }
                }

                RaisePropertyChanged(nameof(IsVisible));
            }
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
                RaisePropertyChanged(nameof(MessageToDisplay));
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
                RaisePropertyChanged(nameof(ErrorMessageToDisplay));
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
                RaisePropertyChanged(nameof(InitialisationDone));
            }
        }

        public void Dispose()
        {
            foreach (var setting in Settings)
            {
                if (setting is IDisposable)
                {
                    (setting as IDisposable).Dispose();
                }
            }
        }

        public bool CanClose()
        {
            try
            {
                //Mil.Instance.Free();
            }
            catch (Exception)
            {
            }

            return true;
        }
    }
}
