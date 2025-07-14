using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.GUI.Components;

using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Material;

namespace UnitySC.GUI.Common.Equipment.LoadPort
{
    public class SimplifiedSlotMapViewModel : Notifier
    {
        public SimplifiedSlotMapViewModel()
        {
            if (IsInDesignMode)
            {
                IndexedSlots = new ObservableCollection<IndexedSlotState>();
                for (var i = 0; i < 25; i++)
                {
                    IndexedSlots.Add(new IndexedSlotState(SlotState.HasWafer, new Substrate("C99S" + i), i + 1));
                }
            }
        }

        #region Properties

        private ObservableCollection<IndexedSlotState> _indexedSlots;

        public ObservableCollection<IndexedSlotState> IndexedSlots
        {
            get => _indexedSlots;
            private set => SetAndRaiseIfChanged(ref _indexedSlots, value);
        }

        #endregion

        #region Methods

        public void UpdateSlotMap(
            IEnumerable<SlotState> slotStates,
            Collection<Substrate> substrate)
        {
            IndexedSlots = new ObservableCollection<IndexedSlotState>(
                slotStates.Select((s, i) => new IndexedSlotState(s, substrate[i], i + 1)).Reverse());
            OnPropertyChanged(nameof(IndexedSlots));
        }

        #endregion
    }
}
