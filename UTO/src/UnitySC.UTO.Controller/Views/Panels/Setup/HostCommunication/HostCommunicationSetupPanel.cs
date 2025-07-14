using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;

namespace UnitySC.UTO.Controller.Views.Panels.Setup.HostCommunication
{
    public class HostCommunicationSetupPanel : SetupPanel<HostCommunicationConfiguration>
    {
        #region Constructors

        static HostCommunicationSetupPanel()
        {
            DataTemplateGenerator.Create(typeof(HostCommunicationSetupPanel), typeof(HostCommunicationSetupPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(HostCommunicationSetupPanelResources)));
        }

        public HostCommunicationSetupPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
        }

        #endregion

        #region Properties

        public bool EntityBehavior
        {
            get => ModifiedConfig.Secs1.EntityBehaviour == 0;
            set => ModifiedConfig.Secs1.EntityBehaviour = value ? (byte)0 : (byte)1;
        }

        #endregion Properties

        #region Overrides of SetupPanel

        protected override IConfigManager GetConfigManager()
            => new ConfigManager<HostCommunicationConfiguration>(
                new HostCommunicationConfigurationStorage(),
                new DataContractCloneStrategy(),
                new DataContractCompareStrategy<HostCommunicationConfiguration>());

        public override bool UndoCommandCanExecute()
            => base.UndoCommandCanExecute() && App.ControllerInstance.GemController.IsSetupDone;

        public override bool SaveCommandCanExecute()
            => base.SaveCommandCanExecute() && App.ControllerInstance.GemController.IsSetupDone;

        protected override void SaveConfig()
        {
            base.SaveConfig();

            ((ApplicationConfiguration)App.ControllerInstance.ConfigurationManager.Modified).EquipmentIdentityConfig.MDLN =
                ModifiedConfig.MDLN;
            ((ApplicationConfiguration)App.ControllerInstance.ConfigurationManager.Modified).EquipmentIdentityConfig.SOFTREV =
                ModifiedConfig.SOFTREV;

            ((ApplicationConfiguration)App.ControllerInstance.ConfigurationManager.Modified).EquipmentIdentityConfig.EqpSerialNum =
                ModifiedConfig.EqpSerialNum;
            ((ApplicationConfiguration)App.ControllerInstance.ConfigurationManager.Modified).EquipmentIdentityConfig.E30EquipmentSupplier =
                ModifiedConfig.E30EquipmentSupplier;

            if (App.ControllerInstance.ConfigurationManager.IsApplyRequired)
            {
                App.ControllerInstance.ConfigurationManager.Apply(true);
            }
        }

        protected override bool ChangesNeedRestart => true;

        #endregion
    }
}
