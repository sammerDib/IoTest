using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Devices.Efem.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Efem
{
    public abstract class EfemConfigurationSettingsPanel<T> : DeviceSettingsPanel<T>
        where T : EfemConfiguration, new()
    {
        #region Constructors

        static EfemConfigurationSettingsPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(EfemSettingsResources)));
        }

        protected EfemConfigurationSettingsPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {

        }

        #endregion

        #region Properties


        public bool LightCurtainSecurityEnabled
        {
            get => ModifiedConfig?.LightCurtainSecurityEnabled ?? false;
            set
            {
                ModifiedConfig.LightCurtainSecurityEnabled = value;
                OnPropertyChanged();
            }
        }

        public LightCurtainWiring LightCurtainWiring
        {
            get => ModifiedConfig?.LightCurtainWiring ?? LightCurtainWiring.NPN;
            set
            {
                ModifiedConfig.LightCurtainWiring = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Override

        protected override bool ChangesNeedRestart => false;

        protected override void LoadEquipmentConfig()
        {
            OnPropertyChanged(nameof(LightCurtainSecurityEnabled));
            OnPropertyChanged(nameof(LightCurtainWiring));
        }

        #endregion
    }
}
