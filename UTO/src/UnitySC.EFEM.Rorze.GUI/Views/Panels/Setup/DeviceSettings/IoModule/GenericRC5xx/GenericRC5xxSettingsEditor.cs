using System;

using Agileo.Common.Configuration;
using Agileo.Common.Logging;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;
using UnitySC.GUI.Common;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Communication;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.IoModule.GenericRC5xx
{
    public abstract class GenericRC5xxSettingsEditor : BaseEditor<GenericRC5xxConfiguration>, IGenericRC5xxSettingsEditor
    {
        #region Constructors

        static GenericRC5xxSettingsEditor()
        {
            DataTemplateGenerator.Create(typeof(GenericRC5xxSettingsEditor), typeof(GenericRC5xxSettingsEditorView));
        }

        protected GenericRC5xxSettingsEditor()
            : base(null)
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        protected GenericRC5xxSettingsEditor(int instanceId, ILogger logger)
            : base(logger)
        {
            InstanceId = instanceId;
        }

        #endregion

        #region Properties

        public int InstanceId { get; }

        public CommunicationConfigurationSettingsEditor CommunicationConfig { get; protected set; }

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

            return equals;
        }

        public override void UndoChanges()
        {
            base.UndoChanges();

            CommunicationConfig =
                new CommunicationConfigurationSettingsEditor(ModifiedConfig?.CommunicationConfig);
        }
    }

    public class GenericRC5xxSettingsEditor<T> : GenericRC5xxSettingsEditor
        where T : class, IConfigurableDevice<GenericRC5xxConfiguration>
    {
        #region Fields

        private readonly T _device;

        #endregion

        #region Constructors

        public GenericRC5xxSettingsEditor(T device, int instanceId, ILogger logger)
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

    public interface IGenericRC5xxSettingsEditor
    {
        void UndoChanges();

        void SaveConfig();

        bool ConfigurationEqualsCurrent();

        CommunicationConfigurationSettingsEditor CommunicationConfig { get; }
    }
}
