using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using CommunityToolkit.Mvvm.Messaging;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Shutter
{
    public class ShutterVM : ObservableObject
    {
        private ShutterSupervisor _supervisor;

        public string CustomTxt { get; set; }

        public ShutterVM(ShutterSupervisor supervisor)
        {
            _supervisor = supervisor;

            Messenger.Register<StateChangedMessage>(this, (r, m) => { UpdateState(m.State); });
            Messenger.Register<ShutterIrisPositionChangedMessages>(this, (r, m) => { UpdateShutterIrisPosition(m.ShutterIrisPosition); });

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

        private void UpdateState(string value)
        {
            State = value;
        }

        private void UpdateShutterIrisPosition(string shutterIrisPosition)
        {
            ShutterIrisPosition = shutterIrisPosition;
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

        private string _shutterIrisPosition;
        public string ShutterIrisPosition
        {
            get => _shutterIrisPosition;
            set
            {
                if (_shutterIrisPosition != value)
                {
                    _shutterIrisPosition = value;
                    OnPropertyChanged();
                }
            }
        }

        private AutoRelayCommand _openShutterCommand;
        public AutoRelayCommand OpenShutterCommand
        {
            get
            {
                return _openShutterCommand ?? (_openShutterCommand = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.OpenShutterCommand());
                    }));
            }
        }

        private AutoRelayCommand _closeShutterCommand;
        public AutoRelayCommand CloseShutterCommand
        {
            get
            {
                return _closeShutterCommand ?? (_closeShutterCommand = new AutoRelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.CloseShutterCommand());
                    }));
            }
        }
    }
}
