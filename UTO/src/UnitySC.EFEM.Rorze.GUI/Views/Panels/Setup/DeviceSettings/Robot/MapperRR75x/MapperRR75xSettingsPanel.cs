using System.Linq;

using Agileo.Common.Configuration;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x.Configuration;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.Robot.RR75x;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.GUI.Common;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Communication;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Robot;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.Robot.MapperRR75x
{
    public class MapperRR75xSettingsPanel
        : RobotConfigurationSettingsPanel<MapperRR75xConfiguration>
    {
        #region Fields

        private readonly Devices.Robot.MapperRR75x.MapperRR75x _robot;

        #endregion

        #region Constructors
        static MapperRR75xSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(MapperRR75xSettingsPanel), typeof(MapperRR75xSettingsPanelView));
        }

        public MapperRR75xSettingsPanel(
            Devices.Robot.MapperRR75x.MapperRR75x robot,
            string relativeId,
            IIcon icon = null)
            : base(relativeId, icon)
        {
            _robot = robot;

            Rules.Add(
                new DelegateRule(
                    nameof(CommunicationConfig),
                    () =>
                    {
                        var errors = CommunicationConfig?.GetAllErrors().ToList();
                        if (errors != null && errors.Any())
                        {
                            return errors.First();
                        }

                        return string.Empty;
                    }));

            Rules.Add(
                new DelegateRule(
                    nameof(StoppingPositionConfig),
                    () =>
                    {
                        var errors = StoppingPositionConfig?.GetAllErrors().ToList();
                        if (errors != null && errors.Any())
                        {
                            return errors.First();
                        }

                        return string.Empty;
                    }));

            Rules.Add(
                new DelegateRule(
                    nameof(MappingPositionConfig),
                    () =>
                    {
                        var errors = MappingPositionConfig?.GetAllErrors().ToList();
                        if (errors != null && errors.Any())
                        {
                            return errors.First();
                        }

                        return string.Empty;
                    }));
        }

        #endregion

        #region Properties

        #region CommunicationConfiguration

        public CommunicationConfigurationSettingsEditor CommunicationConfig { get; private set; }
        public StoppingPositionSettingsEditor StoppingPositionConfig { get; private set; }
        public MappingPositionSettingsEditor MappingPositionConfig { get; private set; }

        #endregion

        #endregion

        #region Override

        protected override IConfigManager GetDeviceConfigManager()
        {
            return _robot.LoadDeviceConfiguration<MapperRR75xConfiguration>(
                App.Instance.Config.EquipmentConfig.DeviceConfigFolderPath,
                Logger);
        }

        protected override void LoadEquipmentConfig()
        {
            base.LoadEquipmentConfig();

            CommunicationConfig =
                new CommunicationConfigurationSettingsEditor(ModifiedConfig?.CommunicationConfig);
            StoppingPositionConfig = new StoppingPositionSettingsEditor(
                ModifiedConfig?.StoppingPositionPerSampleSize,
                this);

            MappingPositionConfig = new MappingPositionSettingsEditor(
                ModifiedConfig?.MappingPositionPerSampleSize,
                this);
            CommunicationConfig.ErrorsChanged += CommunicationConfig_ErrorsChanged;
            StoppingPositionConfig.ErrorsChanged += StoppingPositionConfig_ErrorsChanged;
            MappingPositionConfig.ErrorsChanged += MappingPositionConfig_ErrorsChanged;
        }

        protected override void UndoChanges()
        {
            base.UndoChanges();

            CommunicationConfig.ErrorsChanged -= CommunicationConfig_ErrorsChanged;
            StoppingPositionConfig.ErrorsChanged -= StoppingPositionConfig_ErrorsChanged;
            MappingPositionConfig.ErrorsChanged -= MappingPositionConfig_ErrorsChanged;

            CommunicationConfig =
                new CommunicationConfigurationSettingsEditor(ModifiedConfig?.CommunicationConfig);
            StoppingPositionConfig = new StoppingPositionSettingsEditor(
                ModifiedConfig?.StoppingPositionPerSampleSize,
                this);

            MappingPositionConfig = new MappingPositionSettingsEditor(
                ModifiedConfig?.MappingPositionPerSampleSize,
                this);

            CommunicationConfig.ErrorsChanged += CommunicationConfig_ErrorsChanged;
            StoppingPositionConfig.ErrorsChanged += StoppingPositionConfig_ErrorsChanged;
            MappingPositionConfig.ErrorsChanged += MappingPositionConfig_ErrorsChanged;

            OnPropertyChanged(nameof(CommunicationConfig));
            OnPropertyChanged(nameof(StoppingPositionConfig));
            OnPropertyChanged(nameof(MappingPositionConfig));
        }

        protected override bool ConfigurationEqualsLoaded()
        {
            return ObjectAreEquals(
                       ModifiedConfig.CommunicationConfig,
                       LoadedConfig.CommunicationConfig)
                   && ObjectAreEquals(
                       ModifiedConfig.StoppingPositionPerSampleSizeSerializableData,
                       LoadedConfig.StoppingPositionPerSampleSizeSerializableData)
                   && ObjectAreEquals(
                       ModifiedConfig.MappingPositionPerSampleSizeSerializableData,
                       LoadedConfig.MappingPositionPerSampleSizeSerializableData);
        }

        protected override bool ConfigurationEqualsCurrent()
        {
            if (!ObjectAreEquals(
                    ModifiedConfig.StoppingPositionPerSampleSizeSerializableData,
                    CurrentConfig.StoppingPositionPerSampleSizeSerializableData))
            {
                return false;
            }

            if (!ObjectAreEquals(
                    ModifiedConfig.MappingPositionPerSampleSizeSerializableData,
                    CurrentConfig.MappingPositionPerSampleSizeSerializableData))
            {
                return false;
            }

            return base.ConfigurationEqualsCurrent();
        }

        public override bool SaveCommandCanExecute()
        {
            return base.SaveCommandCanExecute()
                   && !CommunicationConfig.HasErrors
                   && !StoppingPositionConfig.HasErrors
                   && !MappingPositionConfig.HasErrors;
        }

        #endregion

        #region Event handler

        private void CommunicationConfig_ErrorsChanged(
            object sender,
            System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(CommunicationConfig));
        }

        private void StoppingPositionConfig_ErrorsChanged(
            object sender,
            System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(StoppingPositionConfig));
        }

        private void MappingPositionConfig_ErrorsChanged(
            object sender,
            System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(MappingPositionConfig));
        }

        #endregion
    }
}
