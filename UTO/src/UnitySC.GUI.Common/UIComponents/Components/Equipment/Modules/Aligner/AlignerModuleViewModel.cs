using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.Aligner
{
    public class AlignerModuleViewModel : SelectableMachineModuleViewModel<UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner>
    {
        #region Constructor

        static AlignerModuleViewModel()
        {
            DataTemplateGenerator.CreateSync(typeof(AlignerModuleViewModel), typeof(AlignerModule));
        }

        public AlignerModuleViewModel(UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner aligner) : base(aligner)
        {
           
        }

        #endregion
    }
}
