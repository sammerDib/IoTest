using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Production.Equipment.Views.ProcessModules;

namespace UnitySC.ToolControl.ProcessModules.GUI.Views.Panels.Production.Equipment.Views.ProcessModules.Wotan
{
    public class ProductionWotanViewModel
        : ProductionProcessModuleViewModel<
            ToolControl.ProcessModules.Devices.ProcessModule.Wotan.Wotan>
    {
        #region Constructor

        static ProductionWotanViewModel()
        {
            DataTemplateGenerator.CreateSync(typeof(ProductionWotanViewModel), typeof(ProductionWotanView));
        }

        public ProductionWotanViewModel(
            ToolControl.ProcessModules.Devices.ProcessModule.Wotan.Wotan processModule)
            : base(processModule)
        {
        }

        #endregion
    }
}
