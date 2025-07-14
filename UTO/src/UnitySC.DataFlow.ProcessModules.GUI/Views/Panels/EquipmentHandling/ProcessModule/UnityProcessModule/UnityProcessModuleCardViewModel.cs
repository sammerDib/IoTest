using Agileo.Common.Localization;

using UnitySC.GUI.Common.Equipment.DriveableProcessModule;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.DataFlow.ProcessModules.GUI.Views.Panels.EquipmentHandling.ProcessModule.UnityProcessModule
{
    public class UnityProcessModuleCardViewModel
        : DriveableProcessModuleCardViewModel<UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.
            UnityProcessModule.UnityProcessModule>
    {

        static UnityProcessModuleCardViewModel()
        {
            DataTemplateGenerator.Create(typeof(UnityProcessModuleCardViewModel), typeof(UnityProcessModuleCard));

            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(UnityProcessModuleCardResources)));
        }

        public UnityProcessModuleCardViewModel(
            UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule.
                UnityProcessModule processModule)
            : base(processModule)
        {
        }
    }
}
