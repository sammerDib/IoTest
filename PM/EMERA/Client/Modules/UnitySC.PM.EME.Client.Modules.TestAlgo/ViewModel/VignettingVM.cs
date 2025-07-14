using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.EME.Client.Proxy.Algo;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.EME.Client.Modules.TestAlgo.ViewModel
{
    public class VignettingVM : AlgoBaseVM
    {
        private readonly AlgoSupervisor _algoSupervisor;
        public IEnumerable<ScanRangeType> ScanRanges { get; private set; }

        public VignettingVM(FilterWheelBench filterWheelBench) : base("Vignetting")
        {
            _algoSupervisor = ClassLocator.Default.GetInstance<AlgoSupervisor>();
            FilterWheelBench = filterWheelBench;
            ScanRanges = Enum.GetValues(typeof(ScanRangeType)).Cast<ScanRangeType>().Where(e => !e.Equals(ScanRangeType.Configured) && !e.Equals(ScanRangeType.AllAxisRange));
            SelectedRange = ScanRangeType.Small;            
        }

        private ScanRangeType _selectedRange;

        public ScanRangeType SelectedRange
        {
            get => _selectedRange;
            set => SetProperty(ref _selectedRange, value);
        }

        private VignettingResult _result;

        public VignettingResult Result
        {
            get => _result;
            set { if (_result != value) { _result = value; OnPropertyChanged(); } }
        }


        private AutoRelayCommand _startVignetting;

        public AutoRelayCommand StartVignetting
        {
            get
            {
                return _startVignetting ?? (_startVignetting = new AutoRelayCommand(
                    () =>
                    {
                        Result = null;
                        IsBusy = true;
                        _algoSupervisor.VignettingChangedEvent += VignettingVM_VignettingChangedEvent;
                        var vignettingInput = new VignettingInput(_selectedRange);
                        _algoSupervisor.StartVignetting(vignettingInput);
                    }));
            }
        }

        private FilterWheelBench _filterWheelBench;

        public FilterWheelBench FilterWheelBench
        {
            get => _filterWheelBench;
            set => SetProperty(ref _filterWheelBench, value);
        }

        private void VignettingVM_VignettingChangedEvent(VignettingResult vignettingResult)
        {
            UpdateResult(vignettingResult);
        }

        private void UpdateResult(VignettingResult vignettingResult)
        {
            Result = vignettingResult;
            if (vignettingResult.Status.IsFinished)
            {
                _algoSupervisor.VignettingChangedEvent -= VignettingVM_VignettingChangedEvent;
                IsBusy = false;
            }            
        }      
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _algoSupervisor.VignettingChangedEvent -= VignettingVM_VignettingChangedEvent;
                Result = null;
            }           
        }       
    }
}
