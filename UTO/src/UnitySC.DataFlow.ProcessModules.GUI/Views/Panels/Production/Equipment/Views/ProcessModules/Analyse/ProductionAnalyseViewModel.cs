using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Production.Equipment.Views.ProcessModules;

namespace UnitySC.DataFlow.ProcessModules.GUI.Views.Panels.Production.Equipment.Views.ProcessModules.Analyse
{
    public class ProductionAnalyseViewModel : ProductionProcessModuleViewModel<Devices.ProcessModule.Analyse.Analyse>
    {
        #region Constructor

        static ProductionAnalyseViewModel()
        {
            DataTemplateGenerator.CreateSync(typeof(ProductionAnalyseViewModel), typeof(ProductionAnalyseView));
        }

        public ProductionAnalyseViewModel(Devices.ProcessModule.Analyse.Analyse processModule) : base(processModule)
        {

        }

        #endregion
    }
}
