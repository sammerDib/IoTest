using System;

using Agileo.Common.Configuration;
using Agileo.Common.Logging;

using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Configuration;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.IoModule.GenericRC5xx;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.GUI.Common;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Communication;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.IoModule.Dio0
{
    public abstract class Dio0SettingsEditor : BaseEditor<Dio0Configuration>, IGenericRC5xxSettingsEditor
    {
        #region Constructors

        static Dio0SettingsEditor()
        {
            DataTemplateGenerator.Create(typeof(Dio0SettingsEditor), typeof(Dio0SettingsEditorView));
        }

        protected Dio0SettingsEditor()
            : base(null)
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        protected Dio0SettingsEditor(int instanceId, ILogger logger)
            : base(logger)
        {
            InstanceId = instanceId;
        }

        #endregion

        #region Properties

        public int InstanceId { get; }

        public CommunicationConfigurationSettingsEditor CommunicationConfig { get; protected set; }

        public bool IsPressureSensorAvailable
        {
            get => ModifiedConfig?.IsPressureSensorAvailable ?? false;
            set
            {
                ModifiedConfig.IsPressureSensorAvailable = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public override bool ConfigurationEqualsCurrent()
        {
            var equals = SetupPanel.ObjectAreEquals(
                ModifiedConfig.CommunicationConfig.IpAddressAsString,
                CurrentConfig.CommunicationConfig.IpAddressAsString);
            equals = equals
                     && SetupPanel.ObjectAreEquals(
                         ModifiedConfig.CommunicationConfig.AnswerTimeout,
                         CurrentConfig.CommunicationConfig.AnswerTimeout);
            equals = equals
                     && SetupPanel.ObjectAreEquals(
                         ModifiedConfig.CommunicationConfig.TcpPort,
                         CurrentConfig.CommunicationConfig.TcpPort);
            equals = equals
                     && SetupPanel.ObjectAreEquals(
                         ModifiedConfig.CommunicationConfig.ConfirmationTimeout,
                         CurrentConfig.CommunicationConfig.ConfirmationTimeout);
            equals = equals
                     && SetupPanel.ObjectAreEquals(
                         ModifiedConfig.CommunicationConfig.InitTimeout,
                         CurrentConfig.CommunicationConfig.InitTimeout);
            equals = equals
                     && SetupPanel.ObjectAreEquals(
                         ModifiedConfig.CommunicationConfig.CommunicatorId,
                         CurrentConfig.CommunicationConfig.CommunicatorId);
            equals = equals
                     && SetupPanel.ObjectAreEquals(
                         ModifiedConfig.CommunicationConfig.MaxNbRetry,
                         CurrentConfig.CommunicationConfig.MaxNbRetry);
            equals = equals
                     && SetupPanel.ObjectAreEquals(
                         ModifiedConfig.CommunicationConfig.ConnectionRetryDelay,
                         CurrentConfig.CommunicationConfig.ConnectionRetryDelay);
            equals = equals
                     && SetupPanel.ObjectAreEquals(
                         ModifiedConfig.CommunicationConfig.ConnectionMode,
                         CurrentConfig.CommunicationConfig.ConnectionMode);
            equals = equals
                     && ModifiedConfig.IsPressureSensorAvailable
                     == CurrentConfig.IsPressureSensorAvailable;

            return equals;
        }

        public override void UndoChanges()
        {
            base.UndoChanges();

            CommunicationConfig =
                new CommunicationConfigurationSettingsEditor(ModifiedConfig?.CommunicationConfig);
        }
    }

    public class Dio0SettingsEditor<T> : Dio0SettingsEditor
        where T : class, IConfigurableDevice<Dio0Configuration>
    {
        #region Fields

        private readonly T _device;

        #endregion

        #region Constructors

        public Dio0SettingsEditor(T device, int instanceId, ILogger logger)
            : base(instanceId, logger)
        {
            _device = device;
        }

        #endregion

        #region Override

        protected override IConfigManager GetConfigManager()
        {
            var configManager = _device.LoadDeviceConfiguration(
                App.Instance.Config.EquipmentConfig.DeviceConfigFolderPath,
                Logger,
                InstanceId);

            CommunicationConfig =
                new CommunicationConfigurationSettingsEditor(
                    configManager?.Modified?.CommunicationConfig);

            return configManager;
        }

        #endregion
    }
}
