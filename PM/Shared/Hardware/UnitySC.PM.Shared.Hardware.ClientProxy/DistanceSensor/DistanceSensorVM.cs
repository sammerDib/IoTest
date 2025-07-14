using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using CommunityToolkit.Mvvm.Messaging;
using UnitySC.Shared.Tools;
using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.DistanceSensor
{
    public class DistanceSensorVM : ObservableObject
    {
        private DistanceSensorSupervisor _supervisor;

        public string CustomTxt { get; set; }
        public string Status { get; set; }

        public DistanceSensorVM(DistanceSensorSupervisor supervisor)
        {
            _supervisor = supervisor;

            Messenger.Register<StateChangedMessage>(this, (r, m) => { UpdateState(m.State); });
            Messenger.Register<DistanceChangedMessages>(this, (r, m) => { UpdateDistance(m.Distance); });
            Messenger.Register<IdChangedMessage>(this, (r, m) => { UpdateId(m.Id); });
            Messenger.Register<CustomChangedMessage>(this, (r, m) => { UpdateCustom(m.Custom); });

            Task.Run(() => _supervisor.TriggerUpdateEvent());
        }

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

        private void UpdateState(DeviceState value)
        {
            State = value.Status.ToString();
        }

        private void UpdateDistance(double distance)
        {
            DistanceSensorHeight = distance;
        }

        private void UpdateId(string value)
        {
            GetId = value;
        }

        private void UpdateCustom(string value)
        {
            GetCustom = value;
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

        private double _distanceSensorHeight;
        public double DistanceSensorHeight
        {
            get => _distanceSensorHeight;
            set
            {
                if (_distanceSensorHeight != value)
                {
                    _distanceSensorHeight = value;
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

        private AutoRelayCommand _customCommand;
        public AutoRelayCommand CustomCommand
        {
            get
            {
                return _customCommand ?? (_customCommand = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.CustomCommand(CustomTxt));
                    }));
            }
        }
    }
}
