using System.Collections.ObjectModel;
using System.Linq;
using UnitySC.GUI.Common.Vendor.Helpers;
using Agileo.GUI.Commands;

using UnitySC.UTO.Controller.Views.Panels.Gem.Jobs;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment
{
    public class SlotMapSelectionViewModel : SlotMapViewModel
    {
        static SlotMapSelectionViewModel()
        {
            DataTemplateGenerator.Create(typeof(SlotMapSelectionViewModel), typeof(SlotMapSelectionView));
        }

        public SlotMapSelectionViewModel()
        {
            // Design mode only
        }

        public SlotMapSelectionViewModel(CorrespondingCarrierSlotMapViewModel carrierSlotMap,
            bool isInEditionMode)
        {
            UpdateSlotMap(carrierSlotMap.Carrier, false);

            _selectedSlots = new ObservableCollection<Slot>();
            foreach (var slotIndex in carrierSlotMap.MaterialNameListElement.SlotIds)
            {
                var s = Slots.FirstOrDefault(slot => slot.Index == slotIndex);
                if (s != null)
                {
                    _selectedSlots.Add(s);
                }
            }

            IsInEditionMode = isInEditionMode;
        }

        private ObservableCollection<Slot> _selectedSlots;

        public ObservableCollection<Slot> SelectedSlots
        {
            get => _selectedSlots;
            set => SetAndRaiseIfChanged(ref _selectedSlots, value);
        }

        private bool _isInEditionMode;

        public bool IsInEditionMode
        {
            get => _isInEditionMode;
            set => SetAndRaiseIfChanged(ref _isInEditionMode, value);
        }

        #region Select all

        private DelegateCommand _selectAllCommand;

        public DelegateCommand SelectAllCommand
            => _selectAllCommand ??= new DelegateCommand(SelectAllCommandExecute);

        private void SelectAllCommandExecute()
        {
            SelectedSlots.Clear();
            SelectedSlots.AddRange(Slots);
        }

        #endregion

        #region Deselect all

        private DelegateCommand _deselectAllCommand;

        public DelegateCommand DeselectAllCommand
            => _deselectAllCommand ??=
                new DelegateCommand(DeselectAllCommandExecute);

        private void DeselectAllCommandExecute()
        {
            SelectedSlots.Clear();
        }

        #endregion
    }
}
