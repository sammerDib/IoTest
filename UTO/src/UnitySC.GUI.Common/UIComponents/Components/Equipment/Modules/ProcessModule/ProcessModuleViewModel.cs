using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.ProcessModule
{
    public class ProcessModuleViewModel : SelectableMachineModuleViewModel<UnitySC.Equipment.Abstractions.Devices.ProcessModule.ProcessModule>
    {
        #region Constructor

        static ProcessModuleViewModel()
        {
            DataTemplateGenerator.CreateSync(typeof(ProcessModuleViewModel), typeof(ProcessModule));
        }

        public ProcessModuleViewModel(UnitySC.Equipment.Abstractions.Devices.ProcessModule.ProcessModule module) : base(module)
        {

        }

        #endregion
    }
}
