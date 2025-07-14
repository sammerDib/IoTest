using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface.Rfid;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.DMT.Client.Proxy.Chuck
{
    public class ChuckVM : ObservableRecipient, IRecipient<WaferPresenceChangedMessage>
    {
        private readonly ChuckSupervisor _chuckSupervisor;

        public ChuckVM(ChuckSupervisor supervisor)
        {
            _chuckSupervisor = supervisor;
            Messenger.Register<TagChangedMessage>(this, (r, m) => { RfidTag = m.Tag; });

            Init();
        }

        public void Init()
        {
            UpdateTag();

            _chuckSupervisor.GetCurrentState();
            Task.Run(() => _chuckSupervisor.RefreshAllValues());
        }

        private void UpdateTag()
        {
            var tag = GetTag();

            if (tag?.Message == null)
            {
                RfidTag = "";
                RfidSize = "";
            }
            else
            {
                RfidTag = tag.Message;
                RfidSize = tag.Size.Millimeters.ToString();
            }
        }

        private RfidTag GetTag()
        {
            return _chuckSupervisor.GetTag()?.Result;
        }

        private void UpdateChuckWaferPresence(MaterialPresence chuckWaferPresence)
        {
            ChuckWaferPresence = chuckWaferPresence;
        }

        public void Receive(WaferPresenceChangedMessage message)
        {
            UpdateChuckWaferPresence(message.WaferPresence);
        }

        private MaterialPresence _chuckWaferPresence;

        public MaterialPresence ChuckWaferPresence
        {
            get => _chuckWaferPresence;
            set
            {
                if (_chuckWaferPresence != value)
                {
                    _chuckWaferPresence = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _rfidTag;

        public string RfidTag
        {
            get => _rfidTag;
            set
            {
                if (_rfidTag != value)
                {
                    _rfidTag = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _rfidSize;

        public string RfidSize
        {
            get => _rfidSize;
            set
            {
                if (_rfidSize != value)
                {
                    _rfidSize = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _rfidError;

        public string RfidError
        {
            get => _rfidError;
            set
            {
                if (_rfidError != value)
                {
                    _rfidError = value;
                    OnPropertyChanged();
                }
            }
        }

        private AutoRelayCommand _refreshTag;

        public AutoRelayCommand RefreshTag
        {
            get
            {
                return _refreshTag ?? (_refreshTag = new AutoRelayCommand(
                    () =>
                    {
                        UpdateTag();
                    }
                ));
            }
        }
    }
}
