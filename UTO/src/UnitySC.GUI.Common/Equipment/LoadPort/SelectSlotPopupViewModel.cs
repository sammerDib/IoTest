using System.Collections.ObjectModel;

using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Material;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Equipment.LoadPort
{
    public class SelectSlotPopupViewModel : NotifyDataError
    {
        static SelectSlotPopupViewModel()
        {
            DataTemplateGenerator.Create(typeof(SelectSlotPopupViewModel), typeof(SelectSlotPopup));
        }

        public SelectSlotPopupViewModel()
        {
            if (IsInDesignMode)
            {
                MappingTable = new ObservableCollection<IndexedSlotState>();
                for (int i = 0; i < 25; i++)
                {
                    MappingTable.Add(new IndexedSlotState(SlotState.HasWafer, new Substrate("C99S" + i), i + 1));
                }
            }
        }

        public SelectSlotPopupViewModel(ObservableCollection<IndexedSlotState> mappingTable)
            => MappingTable = mappingTable;

        public ObservableCollection<IndexedSlotState> MappingTable { get; }

        private IndexedSlotState _selectedSlot;

        public IndexedSlotState SelectedSlot
        {
            get { return _selectedSlot; }
            set { SetAndRaiseIfChanged(ref _selectedSlot, value); }
        }
    }
}
