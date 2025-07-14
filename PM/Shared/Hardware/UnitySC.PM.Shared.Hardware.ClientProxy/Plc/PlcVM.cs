using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Plc
{
    public class PlcVM : ObservableObject
    {
        private PlcSupervisor _supervisor;
        private string _deviceID;

        public string CustomTxt { get; set; }

        public PlcVM(PlcSupervisor supervisor)
        {
            _supervisor = supervisor;

            Messenger.Register<StateChangedMessage>(this, (r, m) => { UpdateState(m.State); });
            Messenger.Register<IdChangedMessage>(this, (r, m) => { UpdateId(m.Id); });
            Messenger.Register<CustomChangedMessage>(this, (r, m) => { UpdateCustom(m.Custom); });
            Messenger.Register<AmsNetIdChangedMessage>(this, (r, m) => { UpdateAmsNetId(m.AmsNetId); });

            Task.Run(() => _supervisor.TriggerUpdateEvent());
        }

        /*public PlcVM(PlcSupervisor supervisor, String deviceID)
        {
            _supervisor = supervisor;
            _deviceID = deviceID;

            Messenger.Register<StateChangedMessage>(this, (r, m) => { UpdateState(m.State); });
            Messenger.Register<IdChangedMessage>(this, (r, m) => { UpdateId(m.Id); });
            Messenger.Register<CustomChangedMessage>(this, (r, m) => { UpdateCustom(m.Custom); });
            Messenger.Register<AmsNetIdChangedMessage>(this, (r, m) => { UpdateAmsNetId(m.AmsNetId); });

            Task.Run(() => _supervisor.TriggerUpdateEvent());
        }*/

        private IMessenger _messenger;

        public IMessenger Messenger
        {
            get
            {
                if (_messenger == null)
                    _messenger = ClassLocator.Default.GetInstance<IMessenger>();
                return _messenger;
            }
        }

        private void UpdateState(string value)
        {
            State = value;
        }

        private void UpdateId(string value)
        {
            GetId = value;
        }

        private void UpdateCustom(string value)
        {
            GetCustom = value;
        }

        private void UpdateAmsNetId(string value)
        {
            GetAmsNetId = value; ;
        }

        private string _state;

        public string State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _getId;

        public string GetId
        {
            get => _getId;
            set
            {
                if (_getId != value)
                {
                    _getId = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _getCustom;

        public string GetCustom
        {
            get => _getCustom;
            set
            {
                if (_getCustom != value)
                {
                    _getCustom = value;

                    OnPropertyChanged();
                }
            }
        }

        private string _getAmsNetId;

        public string GetAmsNetId
        {
            get => _getAmsNetId;
            set
            {
                if (_getAmsNetId != value)
                {
                    _getAmsNetId = value;

                    OnPropertyChanged();
                }
            }
        }

        private RelayCommand _restartCommand;

        public RelayCommand RestartCommand
        {
            get
            {
                return _restartCommand ?? (_restartCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.Restart());
                    }));
            }
        }

        private RelayCommand _rebootCommand;

        public RelayCommand RebootCommand
        {
            get
            {
                return _rebootCommand ?? (_rebootCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.Reboot());
                    }));
            }
        }

        private RelayCommand _customCommand;

        public RelayCommand CustomCommand
        {
            get
            {
                return _customCommand ?? (_customCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.CustomCommand(CustomTxt));
                    }));
            }
        }

        private RelayCommand _smokeDetectorResetAlarmCommand;

        public RelayCommand SmokeDetectorResetAlarmCommand
        {
            get
            {
                return _smokeDetectorResetAlarmCommand ?? (_smokeDetectorResetAlarmCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.SmokeDetectorResetAlarm());
                    }));
            }
        }
    }
}
