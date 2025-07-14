using Agileo.Common.Configuration;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Aligner;
using UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.Communication;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.Aligner.RA420
{
    public class RA420SettingsPanel : AlignerConfigurationSettingsPanel<RA420Configuration>
    {
        #region Fields

        private readonly EFEM.Rorze.Devices.Aligner.RA420.RA420 _aligner;

        #endregion

        #region Constructors
        static RA420SettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(RA420SettingsPanel), typeof(RA420SettingsPanelView));
        }

        public RA420SettingsPanel(EFEM.Rorze.Devices.Aligner.RA420.RA420 aligner, string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            _aligner = aligner;
        }

        #endregion

        #region Properties

        #region CommunicationConfiguration

        public CommunicationConfigurationSettingsEditor CommunicationConfig { get; private set; }

        #endregion

        public DataTableSource<SubstrateInformationsPerPositionsContainer>
            DataTableSubstrateInformations
        {
            get;
        } = new();

        private void UpdateDataTable()
            => DataTableSubstrateInformations.Reset(
                ModifiedConfig.SubstrateInformationsPerPositionsSerializableData);

        #endregion

        #region Override

        protected override IConfigManager GetDeviceConfigManager()
            => _aligner.ConfigManager;

        protected override void LoadEquipmentConfig()
        {
            base.LoadEquipmentConfig();

            CommunicationConfig = new CommunicationConfigurationSettingsEditor(ModifiedConfig?.CommunicationConfig);
            UpdateDataTable();
        }

        protected override void UndoChanges()
        {
            base.UndoChanges();

            CommunicationConfig = new CommunicationConfigurationSettingsEditor(ModifiedConfig?.CommunicationConfig);
            UpdateDataTable();
        }

        public override void OnShow()
        {
            base.OnShow();

            UpdateDataTable();
        }

        protected override bool ConfigurationEqualsCurrent()
        {
            if (ModifiedConfig.SubstrateInformationsPerPositionsSerializableData.Length !=
                    CurrentConfig.SubstrateInformationsPerPositionsSerializableData.Length)
            {
                return false;
            }

            for (int iConfig = 0;
                 iConfig < ModifiedConfig.SubstrateInformationsPerPositionsSerializableData.Length;
                 iConfig++)
            {
                if (ModifiedConfig.SubstrateInformationsPerPositionsSerializableData[iConfig].SubstrateInformations.SubstrateSize
                    != CurrentConfig.SubstrateInformationsPerPositionsSerializableData[iConfig].SubstrateInformations.SubstrateSize
                    || ModifiedConfig.SubstrateInformationsPerPositionsSerializableData[iConfig].SubstrateInformations.MaterialType
                    != CurrentConfig.SubstrateInformationsPerPositionsSerializableData[iConfig].SubstrateInformations.MaterialType)
                {
                    return false;
                }
            }

            return base.ConfigurationEqualsCurrent();
        }

        protected override bool ConfigurationEqualsLoaded()
            => ObjectAreEquals(ModifiedConfig.CommunicationConfig, LoadedConfig.CommunicationConfig);

        public override bool SaveCommandCanExecute()
        {
            return base.SaveCommandCanExecute() && !CommunicationConfig.HasErrors;
        }

        #endregion
    }
}
