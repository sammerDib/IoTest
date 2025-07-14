using Agileo.Common.Localization;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Production.Equipment.Views.ProcessModules;

namespace UnitySC.DataFlow.ProcessModules.GUI.Views.Panels.Production.Equipment.Views.ProcessModules.Demeter
{
    public class ProductionDemeterViewModel
        : ProductionProcessModuleViewModel<
            UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Demeter.Demeter>
    {
        #region Constructor

        static ProductionDemeterViewModel()
        {
            DataTemplateGenerator.CreateSync(typeof(ProductionDemeterViewModel), typeof(ProductionDemeterView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(ProductionDemeterResource)));
        }

        public ProductionDemeterViewModel(
            UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Demeter.Demeter processModule)
            : base(processModule)
        {

        }

        #endregion
    }
}
