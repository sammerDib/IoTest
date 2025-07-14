using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Devices.Robot.Configuration;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Robot
{
    public abstract class RobotConfigurationSettingsPanel<T> : DeviceSettingsPanel<T>
        where T : RobotConfiguration, new()
    {
        #region Constructors

        static RobotConfigurationSettingsPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(RobotSettingsResources)));
        }

        protected RobotConfigurationSettingsPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            Rules.Add(
                new DelegateRule(
                    nameof(UpperArmConfig),
                    () =>
                    {
                        if (!UpperArmConfig.IsArmEnabled && !LowerArmConfig.IsArmEnabled)
                        {
                            return LocalizationManager.GetString(
                                nameof(RobotSettingsResources.S_SETUP_ROBOT_ONE_ARM_MUST_BE_ENABLED));
                        }

                        return string.Empty;
                    }));

            Rules.Add(
                new DelegateRule(
                    nameof(LowerArmConfig),
                    () =>
                    {
                        if (!UpperArmConfig.IsArmEnabled && !LowerArmConfig.IsArmEnabled)
                        {
                            return LocalizationManager.GetString(
                                nameof(RobotSettingsResources.S_SETUP_ROBOT_ONE_ARM_MUST_BE_ENABLED));
                        }

                        return string.Empty;
                    }));
        }

        #endregion

        #region Properties

        public ArmConfigurationSettingsEditor UpperArmConfig { get; private set; }

        public ArmConfigurationSettingsEditor LowerArmConfig { get; private set; }

        #endregion

        #region Override

        protected override void LoadEquipmentConfig()
        {
            UpperArmConfig = new ArmConfigurationSettingsEditor(ModifiedConfig?.UpperArm);
            LowerArmConfig = new ArmConfigurationSettingsEditor(ModifiedConfig?.LowerArm);

            UpperArmConfig.PropertyChanged += ArmConfig_PropertyChanged;
            LowerArmConfig.PropertyChanged += ArmConfig_PropertyChanged;
        }

        protected override void UndoChanges()
        {
            base.UndoChanges();

            UpperArmConfig = new ArmConfigurationSettingsEditor(ModifiedConfig?.UpperArm);
            LowerArmConfig = new ArmConfigurationSettingsEditor(ModifiedConfig?.LowerArm);
        }

        #endregion

        #region Event Handlers

        private void ArmConfig_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            ApplyRules();
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UpperArmConfig.PropertyChanged -= ArmConfig_PropertyChanged;
                LowerArmConfig.PropertyChanged -= ArmConfig_PropertyChanged;
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
