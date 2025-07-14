using Agileo.GUI.Components;

using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Communication.Modbus
{
    public class ModbusConfigurationSettingsEditor : Notifier
    {
        #region Constructor

        static ModbusConfigurationSettingsEditor()
        {
            DataTemplateGenerator.Create(typeof(ModbusConfigurationSettingsEditor), typeof(ModbusConfigurationSettingsEditorView));
        }

        public ModbusConfigurationSettingsEditor(
            ModbusConfiguration modbusConfiguration)
        {
            ModbusConfiguration = modbusConfiguration;
        }

        #endregion

        #region Properties

        public ModbusConfiguration ModbusConfiguration { get; }

        public string IPAddress
        {
            get => ModbusConfiguration?.IpAddress ?? string.Empty;
            set
            {
                ModbusConfiguration.IpAddress = value;
                OnPropertyChanged();
            }
        }

        public int TcpPort
        {
            get => ModbusConfiguration?.TcpPort ?? 0;
            set
            {
                ModbusConfiguration.TcpPort = value;
                OnPropertyChanged();
            }
        }

        public bool IsSimulated
        {
            get => ModbusConfiguration?.IsSimulated ?? false;
            set
            {
                ModbusConfiguration.IsSimulated = value;
                OnPropertyChanged();
            }
        }

        public double PollingPeriodInterval
        {
            get => ModbusConfiguration?.PollingPeriodInterval ?? 0;
            set
            {
                ModbusConfiguration.PollingPeriodInterval = value;
                OnPropertyChanged();
            }
        }

        public ushort MaxSpaceBetweenWordsInRange
        {
            get => ModbusConfiguration?.MaxSpaceBetweenWordsInRange ?? 0;
            set
            {
                ModbusConfiguration.MaxSpaceBetweenWordsInRange = value;
                OnPropertyChanged();
            }
        }

        public int ConnectionTimeout
        {
            get => ModbusConfiguration?.ConnectionTimeout ?? 0;
            set
            {
                ModbusConfiguration.ConnectionTimeout = value;
                OnPropertyChanged();
            }
        }

        public int ConnectionRetryDelay
        {
            get => ModbusConfiguration?.ConnectionRetryDelay ?? 0;
            set
            {
                ModbusConfiguration.ConnectionRetryDelay = value;
                OnPropertyChanged();
            }
        }

        public int ConnectionRetryNumber
        {
            get => ModbusConfiguration?.ConnectionRetryNumber ?? 0;
            set
            {
                ModbusConfiguration.ConnectionRetryNumber = value;
                OnPropertyChanged();
            }
        }

        public string TagsConfigurationPath
        {
            get => ModbusConfiguration?.TagsConfigurationPath ?? string.Empty;
            set
            {
                ModbusConfiguration.TagsConfigurationPath = value;
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
