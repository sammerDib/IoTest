using System.Collections.ObjectModel;

using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Devices.LightTower.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.LightTower
{
    public abstract class LightTowerConfigurationSettingsPanel<T> : DeviceSettingsPanel<T> where T : LightTowerConfiguration, new()
    {
        #region Constructors

        protected LightTowerConfigurationSettingsPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(LightTowerSettingsPanelResources)));
        }

        #endregion

        #region Properties

        public ObservableCollection<LightTowerDetails> LightTowerStatus { get; } = new();

        #endregion

        #region Override

        protected override void LoadEquipmentConfig()
        {
            ResetLightTowerStatus();
        }

        protected override void UndoChanges()
        {
            base.UndoChanges();
            DispatcherHelper.DoInUiThreadAsynchronously(ResetLightTowerStatus);
        }

        protected override bool ConfigurationEqualsCurrent()
        {
            return base.ConfigurationEqualsCurrent()
                   && ObjectAreEquals(
                       ModifiedConfig?.LightTowerStatuses,
                       CurrentConfig?.LightTowerStatuses);
        }

        #endregion

        #region Private

        private void ResetLightTowerStatus()
        {
            LightTowerStatus.Clear();

            if (ModifiedConfig == null)
            {
                return;
            }

            foreach (var status in ModifiedConfig.LightTowerStatuses)
            {
                LightTowerStatus.Add(new LightTowerDetails(status));
            }

            OnPropertyChanged(nameof(LightTowerStatus));
        }

        #endregion
    }
}
