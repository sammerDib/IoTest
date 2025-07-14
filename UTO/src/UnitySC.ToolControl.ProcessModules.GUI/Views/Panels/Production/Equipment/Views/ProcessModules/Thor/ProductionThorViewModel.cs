using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Production.Equipment.Views.ProcessModules;

namespace UnitySC.ToolControl.ProcessModules.GUI.Views.Panels.Production.Equipment.Views.ProcessModules.Thor
{
    public class ProductionThorViewModel
        : ProductionProcessModuleViewModel<
            ToolControl.ProcessModules.Devices.ProcessModule.Thor.Thor>
    {
        #region Constructor

        static ProductionThorViewModel()
        {
            DataTemplateGenerator.CreateSync(typeof(ProductionThorViewModel), typeof(ProductionThorView));
        }

        public ProductionThorViewModel(
            ToolControl.ProcessModules.Devices.ProcessModule.Thor.Thor processModule)
            : base(processModule)
        {
        }

        #endregion
    }
}
