using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.GUI.Common.Helpers;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Communication
{
    public class CommunicationConfigurationSettingsEditor : NotifyDataError
    {
        #region Constructor

        static CommunicationConfigurationSettingsEditor()
        {
            DataTemplateGenerator.Create(typeof(CommunicationConfigurationSettingsEditor), typeof(CommunicationConfigurationSettingsEditorView));
        }

        public CommunicationConfigurationSettingsEditor(
            CommunicationConfiguration communicationConfig)
        {
            CommunicationConfig = communicationConfig;
            IPAddress = CommunicationConfig.IpAddressAsString;
            Rules.Add(new DelegateRule(nameof(IPAddress), () => IpHelper.ValidateIp(IPAddress)));
            ApplyRules();
        }

        #endregion

        #region Properties

        public CommunicationConfiguration CommunicationConfig { get; }

        public ConnectionMode ConnectionMode
        {
            get => CommunicationConfig?.ConnectionMode ?? ConnectionMode.Client;
            set
            {
                CommunicationConfig.ConnectionMode = value;
                OnPropertyChanged();
            }
        }

        private string _ipAddress;
        public string IPAddress
        {
            get => _ipAddress;
            set
            {
                CommunicationConfig.IpAddressAsString = value;
                SetAndRaiseIfChanged(ref _ipAddress, value);
            }
        }

        public uint TcpPort
        {
            get => CommunicationConfig?.TcpPort ?? 0;
            set
            {
                CommunicationConfig.TcpPort = value;
                OnPropertyChanged();
            }
        }

        public uint AnswerTimeout
        {
            get => CommunicationConfig?.AnswerTimeout ?? 0;
            set
            {
                CommunicationConfig.AnswerTimeout = value;
                OnPropertyChanged();
            }
        }

        public uint ConfirmationTimeout
        {
            get => CommunicationConfig?.ConfirmationTimeout ?? 0;
            set
            {
                CommunicationConfig.ConfirmationTimeout = value;
                OnPropertyChanged();
            }
        }

        public uint InitTimeout
        {
            get => CommunicationConfig?.InitTimeout ?? 0;
            set
            {
                CommunicationConfig.InitTimeout = value;
                OnPropertyChanged();
            }
        }

        public int CommunicatorId
        {
            get => CommunicationConfig?.CommunicatorId ?? 0;
            set
            {
                CommunicationConfig.CommunicatorId = value;
                OnPropertyChanged();
            }
        }

        public int MaxNbRetry
        {
            get => CommunicationConfig?.MaxNbRetry ?? 0;
            set
            {
                CommunicationConfig.MaxNbRetry = value;
                OnPropertyChanged();
            }
        }

        public int ConnectionRetryDelay
        {
            get => CommunicationConfig?.ConnectionRetryDelay ?? 0;
            set
            {
                CommunicationConfig.ConnectionRetryDelay = value;
                OnPropertyChanged();
            }
        }

        public uint AliveBitPeriod
        {
            get => CommunicationConfig?.AliveBitPeriod ?? 0;
            set
            {
                CommunicationConfig.AliveBitPeriod = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
