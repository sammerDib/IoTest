using Agileo.Common.Localization;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Devices.Controller.Activities.WaferFlow.Enum;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;
using UnitySC.UTO.Controller.Configuration;
using UnitySC.UTO.Controller.Remote.Constants;

namespace UnitySC.UTO.Controller.Views.Panels.Setup.DataFlow
{
    public class DataFlowSetupPanel : SetupPanel<ControllerConfiguration>
    {
        public StopConfig StopConfigCurrent { get; set; }
        public StopConfig StopConfigLoaded { get; set; }
        public StopConfig StopConfigModified { get; set; }

        static DataFlowSetupPanel()
        {
            DataTemplateGenerator.Create(typeof(DataFlowSetupPanel), typeof(DataFlowSetupPanelView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(DataFlowPanelResources)));
        }

        public DataFlowSetupPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
        }

        #region Overrides of SetupPanel<ControllerConfiguration>

        protected override bool ConfigurationEqualsLoaded() =>
            ObjectAreEquals(ModifiedConfig?.DataFlowFolderName, LoadedConfig?.DataFlowFolderName) &&
            ObjectAreEquals(ModifiedConfig?.ToolKey, LoadedConfig?.ToolKey) &&
            ObjectAreEquals(ModifiedConfig?.InactivityTimeoutDuration, LoadedConfig?.InactivityTimeoutDuration) &&
            ObjectAreEquals(ModifiedConfig?.ResultReceptionTimeoutDuration, LoadedConfig?.ResultReceptionTimeoutDuration);

        protected override bool ConfigurationEqualsCurrent() =>
            ObjectAreEquals(ModifiedConfig?.DataFlowFolderName, CurrentConfig?.DataFlowFolderName) &&
            ObjectAreEquals(ModifiedConfig?.ToolKey, CurrentConfig?.ToolKey) &&
            ObjectAreEquals(ModifiedConfig?.CarrierPickOrder, CurrentConfig?.CarrierPickOrder) &&
            ObjectAreEquals(ModifiedConfig?.UnloadCarrierAfterAbort, CurrentConfig?.UnloadCarrierAfterAbort) &&
            ObjectAreEquals(ModifiedConfig?.UnloadCarrierBetweenJobs, CurrentConfig?.UnloadCarrierBetweenJobs) &&
            ObjectAreEquals(ModifiedConfig?.DisableParallelControlJob, CurrentConfig?.DisableParallelControlJob) &&
            ObjectAreEquals(ModifiedConfig?.InactivityTimeoutDuration, CurrentConfig?.InactivityTimeoutDuration) &&
            ObjectAreEquals(ModifiedConfig?.JobRecreateAfterInit, CurrentConfig?.JobRecreateAfterInit) &&
            ObjectAreEquals(ModifiedConfig?.StartHotLot, CurrentConfig?.StartHotLot) &&
            ObjectAreEquals(ModifiedConfig?.ResultReceptionTimeoutDuration, CurrentConfig?.ResultReceptionTimeoutDuration) &&
            ObjectAreEquals(StopConfigModified, StopConfigCurrent);

        protected override bool ChangesNeedRestart => true;

        public override void OnSetup()
        {
            base.OnSetup();
            StopConfigModified = StopConfigCurrent = StopConfigLoaded = (StopConfig)App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices
                .GetValueByWellKnownName(ECs.StopConfig)
                .ValueTo<byte>();

            App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices.EquipmentConstantChanged += EquipmentConstantsServices_EquipmentConstantChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices.EquipmentConstantChanged -= EquipmentConstantsServices_EquipmentConstantChanged;
            }
            base.Dispose(disposing);
        }

        protected override void UndoChanges()
        {
            base.UndoChanges();
            StopConfigModified = StopConfigCurrent;
        }

        protected override void SaveConfig()
        {
            base.SaveConfig();

            StopConfigCurrent = StopConfigModified;
            App.ControllerInstance.GemController.E30Std.EquipmentConstantsServices
                .SetValueByWellKnownName(ECs.StopConfig, (byte)StopConfigCurrent);
        }

        #endregion

        #region Event handler

        private void EquipmentConstantsServices_EquipmentConstantChanged(object sender, Agileo.Semi.Gem.Abstractions.E30.VariableEventArgs e)
        {
            if (e.Variable.Name == ECs.StopConfig)
            {
                StopConfigCurrent = StopConfigModified = (StopConfig)e.Variable.ValueTo<byte>();
                OnPropertyChanged(nameof(StopConfigModified));
            }
        }

        #endregion
    }
}
