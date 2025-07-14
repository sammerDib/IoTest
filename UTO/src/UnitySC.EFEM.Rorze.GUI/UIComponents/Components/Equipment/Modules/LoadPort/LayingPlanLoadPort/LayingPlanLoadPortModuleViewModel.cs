using Agileo.SemiDefinitions;

using UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.LoadPort;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.EFEM.Rorze.GUI.UIComponents.Components.Equipment.Modules.LoadPort.LayingPlanLoadPort
{
    public class LayingPlanLoadPortModuleViewModel : LoadPortModuleViewModel
    {

        private SampleDimension _sampleSize;
        public SampleDimension SampleSize
        {
            get => _sampleSize;
            set
            {
                SetAndRaiseIfChanged(ref _sampleSize, value);
            }
        }

        #region Constructor

        static LayingPlanLoadPortModuleViewModel()
        {
            DataTemplateGenerator.CreateSync(typeof(LayingPlanLoadPortModuleViewModel), typeof(LayingPlanLoadPortModule));
        }

        public LayingPlanLoadPortModuleViewModel(UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort loadPort) : base(loadPort)
        {
            loadPort.CarrierPlaced += LoadPort_CarrierPlaced;
            loadPort.CarrierRemoved += LoadPort_CarrierRemoved;
        }

        #endregion

        #region EventHandler

        private void LoadPort_CarrierPlaced(object sender, UnitySC.Equipment.Abstractions.Material.CarrierEventArgs e)
        {
            SampleSize = e.Carrier.SampleSize;
        }

        private void LoadPort_CarrierRemoved(object sender, UnitySC.Equipment.Abstractions.Material.CarrierEventArgs e)
        {
            SampleSize = SampleDimension.NoDimension;
        }

        #endregion
    }
}
