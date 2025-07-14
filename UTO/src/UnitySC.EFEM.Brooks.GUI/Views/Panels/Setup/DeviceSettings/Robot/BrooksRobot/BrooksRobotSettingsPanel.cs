using System.Linq;

using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Robot;

namespace UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.Robot.BrooksRobot
{
    public class BrooksRobotSettingsPanel
        : RobotConfigurationSettingsPanel<BrooksRobotConfiguration>
    {
        #region Fields

        private readonly Devices.Robot.BrooksRobot.BrooksRobot _robot;

        #endregion

        #region Constructors

        static BrooksRobotSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(BrooksRobotSettingsPanel), typeof(BrooksRobotSettingsPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(BrooksRobotSettingsResource)));
        }

        public BrooksRobotSettingsPanel(
            Devices.Robot.BrooksRobot.BrooksRobot robot,
            string relativeId,
            IIcon icon = null)
            : base(relativeId, icon)
        {
            _robot = robot;

            Rules.Add(
                new DelegateRule(
                    nameof(BrooksPositionConfig),
                    () =>
                    {
                        if (BrooksPositionConfig == null)
                        {
                            return string.Empty;
                        }

                        var errors = BrooksPositionConfig.GetAllErrors().ToList();
                        return errors.Any() ? errors.First() : string.Empty;
                    }));
        }

        #endregion

        #region Properties

        public string BrooksRobotName
        {
            get => ModifiedConfig?.BrooksRobotName ?? string.Empty;
            set
            {
                ModifiedConfig.BrooksRobotName = value;
                OnPropertyChanged();
            }
        }

        public string UpperArmName
        {
            get => ModifiedConfig?.UpperArmName ?? string.Empty;
            set
            {
                ModifiedConfig.UpperArmName = value;
                OnPropertyChanged();
            }
        }

        public string LowerArmName
        {
            get => ModifiedConfig?.LowerArmName ?? string.Empty;
            set
            {
                ModifiedConfig.LowerArmName = value;
                OnPropertyChanged();
            }
        }

        public string UpperEndEffectorName
        {
            get => ModifiedConfig?.UpperEndEffectorName ?? string.Empty;
            set
            {
                ModifiedConfig.UpperEndEffectorName = value;
                OnPropertyChanged();
            }
        }

        public string LowerEndEffectorName
        {
            get => ModifiedConfig?.LowerEndEffectorName ?? string.Empty;
            set
            {
                ModifiedConfig.LowerEndEffectorName = value;
                OnPropertyChanged();
            }
        }

        public string RobotHomeMotionProfile
        {
            get => ModifiedConfig?.RobotHomeMotionProfile ?? string.Empty;
            set
            {
                ModifiedConfig.RobotHomeMotionProfile = value;
                OnPropertyChanged();
            }
        }

        public BrooksPositionSettingsEditor BrooksPositionConfig { get; private set; }

        #endregion

        #region Override

        protected override IConfigManager GetDeviceConfigManager()
        {
            return _robot.ConfigManager;
        }

        protected override void LoadEquipmentConfig()
        {
            base.LoadEquipmentConfig();

            OnPropertyChanged(nameof(BrooksRobotName));
            OnPropertyChanged(nameof(UpperArmName));
            OnPropertyChanged(nameof(LowerArmName));
            OnPropertyChanged(nameof(UpperEndEffectorName));
            OnPropertyChanged(nameof(LowerEndEffectorName));
            OnPropertyChanged(nameof(RobotHomeMotionProfile));

            BrooksPositionConfig = new BrooksPositionSettingsEditor(
                ModifiedConfig?.StoppingPositionPerSampleSize,
                this);

            BrooksPositionConfig.ErrorsChanged += BrooksPositionConfigErrorsChanged;
        }

        protected override void UndoChanges()
        {
            base.UndoChanges();

            BrooksPositionConfig.ErrorsChanged -= BrooksPositionConfigErrorsChanged;


            BrooksPositionConfig = new BrooksPositionSettingsEditor(
                ModifiedConfig?.StoppingPositionPerSampleSize,
                this);


            BrooksPositionConfig.ErrorsChanged += BrooksPositionConfigErrorsChanged;

            OnPropertyChanged(nameof(BrooksPositionConfig));
        }

        

        protected override bool ConfigurationEqualsLoaded()
        {
            return base.ConfigurationEqualsLoaded()
                   && ObjectAreEquals(
                       ModifiedConfig.StoppingPositionPerSampleSizeSerializableData,
                       LoadedConfig.StoppingPositionPerSampleSizeSerializableData);
        }

        protected override bool ConfigurationEqualsCurrent()
        {
            return base.ConfigurationEqualsCurrent()
                   && ObjectAreEquals(ModifiedConfig.StoppingPositionPerSampleSizeSerializableData,
                                        CurrentConfig.StoppingPositionPerSampleSizeSerializableData);
        }

        public override bool SaveCommandCanExecute()
        {
            return base.SaveCommandCanExecute() && !BrooksPositionConfig.HasErrors;
        }

        #endregion

        #region Event handler

        private void BrooksPositionConfigErrorsChanged(
            object sender,
            System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(BrooksPositionConfig));
        }

        #endregion
    }
}
