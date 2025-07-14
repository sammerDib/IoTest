using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Production.Equipment.Views.ProcessModules;

namespace UnitySC.DataFlow.ProcessModules.GUI.Views.Panels.Production.Equipment.Views.ProcessModules.Emera
{
    public class ProductionEmeraViewModel : ProductionProcessModuleViewModel<Devices.ProcessModule.Emera.Emera>
    {
        #region Constructor

        static ProductionEmeraViewModel()
        {
            DataTemplateGenerator.CreateSync(typeof(ProductionEmeraViewModel), typeof(ProductionEmeraView));
        }

        public ProductionEmeraViewModel(Devices.ProcessModule.Emera.Emera processModule) : base(processModule)
        {

        }

        #endregion
    }
}
