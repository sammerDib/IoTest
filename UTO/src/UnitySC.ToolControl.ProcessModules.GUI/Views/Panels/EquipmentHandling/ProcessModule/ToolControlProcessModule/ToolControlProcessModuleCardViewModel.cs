using Agileo.Common.Localization;

using UnitySC.GUI.Common.Equipment.DriveableProcessModule;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.ToolControl.ProcessModules.GUI.Views.Panels.EquipmentHandling.ProcessModule.
    ToolControlProcessModule
{

    public class ToolControlProcessModuleCardViewModel
        : DriveableProcessModuleCardViewModel<ToolControl.ProcessModules.Devices.ProcessModule.
            ToolControlProcessModule.ToolControlProcessModule>
    {

        static ToolControlProcessModuleCardViewModel()
        {
            DataTemplateGenerator.Create(typeof(ToolControlProcessModuleCardViewModel), typeof(ToolControlProcessModuleCard));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(ToolControlProcessModuleCardResources)));
        }

        public ToolControlProcessModuleCardViewModel(
            ToolControl.ProcessModules.Devices.ProcessModule.ToolControlProcessModule.
                ToolControlProcessModule processModule)
            : base(processModule)
        {
        }
    }
}
