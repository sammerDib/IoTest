using System;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Result.CommonUI.ViewModel.LotWafer;
using UnitySC.Result.CommonUI.ViewModel.Search;
using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.ViewModel.Navigation;

namespace UnitySC.Result.CommonUI.ViewModel.Wafers
{
    public class WaferPageVM : PageNavigationVM
    {
        private readonly IMessenger _messenger;

        // propriété de la zone d'affichage de la liste des process modules
        private DisplayViewModel _displayVM;

        public DisplayViewModel DisplayVM
        {
            get => _displayVM;
            set
            {
                if (_displayVM != value) { _displayVM = value; OnPropertyChanged(); }
            }
        }

        public bool IsWaferDetailDisplayed { set; get; }

        public void UpdateSlots(LotWaferSlotVM[] slotsVM)
        {
            int oldSelectSlotIndex = SelectSlotIndex;

            UpdateFromLotSlots(slotsVM);

            /*  Note RTi : wait a bit before remove this commented section to find test case   
                if (oldSelectSlotIndex != -1)
                 {
                     if (slotsVM[oldSelectSlotIndex] == null || !slotsVM[oldSelectSlotIndex].IsResultExist)
                         oldSelectSlotIndex = -1;
                     else
                         SelectSlotIndex = oldSelectSlotIndex;
                 }

                 if (oldSelectSlotIndex == -1) // initial case or if previous slot is not available (select first available slot if exist)
                 {
                     var lotslotvm = slotsVM.FirstOrDefault(x => x.IsResultExist);
                     if (lotslotvm != null && lotslotvm.IsResultExist)
                         SelectSlotIndex = lotslotvm.SlotIndex;
                     else
                         SelectSlotIndex = -1;
                 }   */
        }

        private int _selectSlotIndex;
        public int SelectSlotIndex
        {
            get => _selectSlotIndex;
            set
            {
                SetProperty(ref _selectSlotIndex, value);

                if (IsWaferDetailDisplayed)
                {
                    DisplayVM.WaferDetailSelectionFromWaferPage();
                }

                System.Diagnostics.Debug.WriteLine($"SelectSlotIndex =  {_selectSlotIndex}");
            }
        }

        private WaferSlotVM[] _wslotsVM;

        public WaferSlotVM[] WSlots
        {
            get => _wslotsVM;
            set
            {
                if (_wslotsVM != value)
                {
                    // clean previsou
                    if (_wslotsVM != null)
                    {
                        Array.ForEach(_wslotsVM, a => a.Cleanup());
                    }

                    _wslotsVM = value;
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateFromLotSlots(LotWaferSlotVM[] lotSlotvm)
        {
            if (_wslotsVM == null)
                return;

            if (lotSlotvm == null)
            {
                foreach (var slotvm in _wslotsVM)
                    slotvm.Update();
            }
            else
            {
                foreach (var slotvm in _wslotsVM)
                {
                    var lotvm = lotSlotvm[slotvm.SlotIndex];
                    if (lotvm != null && lotvm.Item != null)
                        slotvm.Update(true, lotvm.Item.Id, lotvm.State);
                    else
                        slotvm.Update();
                }
            }
        }

        private ResultWaferVM _curResultWaferDetailVM;

        public ResultWaferVM CurrentResultWaferVM
        {
            get => _curResultWaferDetailVM; set { if (_curResultWaferDetailVM != value) { _curResultWaferDetailVM = value; OnPropertyChanged(); } }
        }

        public override string PageName => "Wafer detail";

        public WaferPageVM()
        {
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();

            IsWaferDetailDisplayed = false;

            var waferslotvm = new WaferSlotVM[25];
            for (int i = 0; i < waferslotvm.Length; i++)
            {
                waferslotvm[i] = new WaferSlotVM(i + 1, false);
            }
            WSlots = waferslotvm;
        }

        public void UpdateWaferSlotVM(LotWaferSlotVM slotvm)
        {
            if (slotvm != null)
            {
                WSlots[slotvm.SlotIndex].UpdateWaferSlotVM(slotvm);
            }
        }

        public override void Loaded()
        {
            //System.Diagnostics.Debug.WriteLine("~~~~~~~~~ DISPLAY PAGE WAFER DETAIL");
            IsWaferDetailDisplayed = true;
        }

        public override void Unloading()
        {
            //System.Diagnostics.Debug.WriteLine("~~~~~~~~ HIDE PAGE WAFER DETAIL ############");
            IsWaferDetailDisplayed = false;

            DisplayVM.CurrentResultWafer = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

        }
    }
}
