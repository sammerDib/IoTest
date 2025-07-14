using System.Linq;
using System.Threading;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Result.CommonUI.ViewModel.LotWafer
{
    public class LotWafersVM : ObservableRecipient
    {
        private bool _isLotviewEnabled;

        public bool IsLotViewEnabled
        {
            get => _isLotviewEnabled; set { if (_isLotviewEnabled != value) { _isLotviewEnabled = value; OnPropertyChanged(); } }
        }

        private readonly DuplexServiceInvoker<IResultService> _resultService;

        public LotWafersVM(DuplexServiceInvoker<IResultService> resultservice)
        {
            _resultService = resultservice;
        }

        private LotWaferSlotVM[] _slotsVM;

        public LotWaferSlotVM[] Slots
        {
            get => _slotsVM;
            set
            {
                if (_slotsVM != value)
                {
                    var oldSelectWafer = SelectedWafer;

                    _slotsVM = value;
                    OnPropertyChanged();

                    if (_slotsVM != null)
                    {
                        // Update Scan result priority if needed
                        foreach (var svm in _slotsVM)
                        {
                            if (svm != null && svm.Item != null)
                            {
                                if (svm.Item.InternalState == (int)ResultInternalState.NotProcess && svm.Item.State >= (int)ResultState.Ok)
                                    _resultService.Invoke(x => x.ResultScanRequest(svm.Item.Id, svm.Item.IsAcquisition));
                            }
                        }
                    }

                    // Maj Selected wafer
                    // we kept the same slot id if exist - otherwise select the first one
                    if (oldSelectWafer != null)
                    {
                        if (_slotsVM[oldSelectWafer.SlotIndex] != null)
                            SelectedWafer = _slotsVM[oldSelectWafer.SlotIndex];
                        else
                            SelectedWafer = _slotsVM.Where(x => x.IsResultExist).FirstOrDefault();
                    }

                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    System.GC.Collect();
                }
            }
        }

        public void UpdateSlots(LotWaferSlotVM[] slotsvm)
        {
            var oldSelectWafer = SelectedWafer;

            for (int i = 0; i < slotsvm.Length; i++)
            {
                _slotsVM[i].UpdateSlotVM(slotsvm[i]);
            }

            // Maj Selected wafer
            // Keep the same slot id if exist AND has results for this post process module
            if (oldSelectWafer == null) return;

            if (_slotsVM[oldSelectWafer.SlotIndex] != null && _slotsVM[oldSelectWafer.SlotIndex].IsResultExist)
            {
                SelectedWafer = _slotsVM[oldSelectWafer.SlotIndex];
            }
            else //  select the first one that gets available results
            {
                SelectedWafer = _slotsVM.Where(x => x.IsResultExist).FirstOrDefault();
            }
        }

        public void Init_Async(LotWaferSlotVM[] slotsvm, CancellationToken token)
        {
            for (int i = 0; i < slotsvm.Length; i++)
            {
                _slotsVM[i].SetResult_Async(slotsvm[i].Item);
            }

            _selectdWaferSlot = null;
            if (_slotsVM != null)
            {
                // Update Scan result priority if needed
                foreach (var svm in _slotsVM)
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (svm != null && svm.Item != null)
                    {
                        if (svm.Item.InternalState == (int)ResultInternalState.NotProcess && svm.Item.State >= (int)ResultState.Ok)
                            _resultService.Invoke(x => x.ResultScanRequest(svm.Item.Id, svm.Item.IsAcquisition));
                    }
                }
            }
        }

        public void Refresh_Async()
        {
            // Notify UI
            //..........
            Application.Current?.Dispatcher.Invoke(() =>
            {
                OnPropertyChanged(nameof(Slots));
                OnPropertyChanged(nameof(SelectedWafer));
            });
        }

        private LotWaferSlotVM _selectdWaferSlot;

        public LotWaferSlotVM SelectedWafer
        {
            get => _selectdWaferSlot;
            set
            {
                if (_selectdWaferSlot != value)
                {
                    _selectdWaferSlot = value;
                    OnPropertyChanged();
                }
            }
        }

        public void RefreshThumbnail()
        {
            if (_slotsVM != null)
            {
                // Update Scan result priority if needed
                foreach (var svm in _slotsVM)
                {
                    if (svm != null && svm.Item != null)
                    {
                        if (svm.Item.State >= (int)ResultState.Ok)
                            _resultService.Invoke(x => x.ResultReScanRequest(svm.Item.Id, svm.Item.IsAcquisition));
                    }
                }
            }
        }
    }
}
