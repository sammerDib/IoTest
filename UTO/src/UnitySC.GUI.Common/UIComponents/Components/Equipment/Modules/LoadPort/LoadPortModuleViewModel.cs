using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.LoadPort
{
    public class LoadPortModuleViewModel : SelectableMachineModuleViewModel<UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort>
    {
        private int _selectedSlotNumber;
        public int SelectedSlotNumber
        {
            get => _selectedSlotNumber;
            set
            {
                SetAndRaiseIfChanged(ref _selectedSlotNumber, value);
            }
        }

        #region Constructor

        static LoadPortModuleViewModel()
        {
            DataTemplateGenerator.CreateSync(typeof(LoadPortModuleViewModel), typeof(LoadPortModule));
        }

        public LoadPortModuleViewModel(UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort loadPort) : base(loadPort)
        {

        }

        #endregion

        #region override

        protected override void OnSelectionStateDeselected()
        {
            SelectedSlotNumber = 0;
        }

        #endregion
    }
}
