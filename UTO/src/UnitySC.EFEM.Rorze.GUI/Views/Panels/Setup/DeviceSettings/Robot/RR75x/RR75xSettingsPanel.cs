using System.Linq;

using Agileo.Common.Configuration;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Communication;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Robot;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.Robot.RR75x
{
    public class RR75xSettingsPanel
        : RobotConfigurationSettingsPanel<RR75xConfiguration>
    {
        #region Fields

        private readonly EFEM.Rorze.Devices.Robot.RR75x.RR75x _robot;

        #endregion

        #region Constructors

        static RR75xSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(RR75xSettingsPanel), typeof(RR75xSettingsPanelView));
        }

        public RR75xSettingsPanel(
            EFEM.Rorze.Devices.Robot.RR75x.RR75x robot,
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
                        var errors = CommunicationConfig.GetAllErrors().ToList();
                        if (errors.Any())
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
                        var errors = StoppingPositionConfig.GetAllErrors().ToList();
                        if (errors.Any())
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

        #endregion

        #endregion

        #region Override

        protected override IConfigManager GetDeviceConfigManager()
        {
            return _robot.ConfigManager;
        }

        protected override void LoadEquipmentConfig()
        {
            base.LoadEquipmentConfig();

            CommunicationConfig =
                new CommunicationConfigurationSettingsEditor(ModifiedConfig?.CommunicationConfig);
            StoppingPositionConfig = new StoppingPositionSettingsEditor(
                ModifiedConfig?.StoppingPositionPerSampleSize,
                this);
            CommunicationConfig.ErrorsChanged += CommunicationConfig_ErrorsChanged;
            StoppingPositionConfig.ErrorsChanged += StoppingPositionConfig_ErrorsChanged;
        }

        protected override void UndoChanges()
        {
            base.UndoChanges();

            CommunicationConfig.ErrorsChanged -= CommunicationConfig_ErrorsChanged;
            StoppingPositionConfig.ErrorsChanged -= StoppingPositionConfig_ErrorsChanged;

            CommunicationConfig =
                new CommunicationConfigurationSettingsEditor(ModifiedConfig?.CommunicationConfig);
            StoppingPositionConfig = new StoppingPositionSettingsEditor(
                ModifiedConfig?.StoppingPositionPerSampleSize,
                this);

            CommunicationConfig.ErrorsChanged += CommunicationConfig_ErrorsChanged;
            StoppingPositionConfig.ErrorsChanged += StoppingPositionConfig_ErrorsChanged;

            OnPropertyChanged(nameof(CommunicationConfig));
            OnPropertyChanged(nameof(StoppingPositionConfig));
        }

        

        protected override bool ConfigurationEqualsLoaded()
        {
            return ObjectAreEquals(
                       ModifiedConfig.CommunicationConfig,
                       LoadedConfig.CommunicationConfig)
                   && ObjectAreEquals(
                       ModifiedConfig.StoppingPositionPerSampleSizeSerializableData,
                       LoadedConfig.StoppingPositionPerSampleSizeSerializableData);
        }

        protected override bool ConfigurationEqualsCurrent()
        {
            if (!ObjectAreEquals(
                    ModifiedConfig.StoppingPositionPerSampleSizeSerializableData,
                    CurrentConfig.StoppingPositionPerSampleSizeSerializableData))
            {
                return false;
            }

            return base.ConfigurationEqualsCurrent();
        }

        public override bool SaveCommandCanExecute()
        {
            return base.SaveCommandCanExecute()
                   && !CommunicationConfig.HasErrors
                   && !StoppingPositionConfig.HasErrors;
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

        #endregion
    }
}
