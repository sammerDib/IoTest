using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.EME.Client.Proxy.Axes;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.EME.Client.Modules.Calibration.ViewModel
{
    public class ChuckParallelismCalibrationVM : CalibrationWizardStepBaseVM
    {
        private readonly AxesVM _axes;

        public ChuckParallelismCalibrationVM(AxesVM axes, ICalibrationService calibrationService)
            : base("Chuck Parallelism", calibrationService)
        {
            _axes = axes;
        }

        public override void Init()
        {
            HasChanged = false;
        }

        public override bool CanCancelChanges() => true;

        public override void CancelChanges()
        {
            Init();
        }

        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            HasChanged = false;
            return true;
        }

        public override bool CanSave()
        {
            return HasChanged;
        }

        public override void Save()
        {
            HasChanged = false;
        }

        public override void Dispose(bool manualDisposing)
        {
        }

        private bool _isCyclingOverXaxis;

        public bool IsCyclingOverXaxis 
        { 
            get => _isCyclingOverXaxis;
            set
            {
                SetProperty(ref _isCyclingOverXaxis, value);
                StartCyclingOverXaxis.NotifyCanExecuteChanged();
                StartCyclingOverYaxis.NotifyCanExecuteChanged();
            }
        }

        private AsyncRelayCommand _startCyclingOverXaxis;

        public IAsyncRelayCommand StartCyclingOverXaxis
        {
            get
            {
                if (_startCyclingOverXaxis == null)
                {
                    _startCyclingOverXaxis = new AsyncRelayCommand(PerformCyclingOverXaxis, CanDoMoveX);
                }

                return _startCyclingOverXaxis;
            }
        }

        private async Task PerformCyclingOverXaxis()
        {

            IsCyclingOverXaxis = true;
            while (IsCyclingOverXaxis)
            {
                double destination = _axes.Position.X < 45 ? 49 : -49;
                await _axes.DoMoveX(destination);
            }
        }

        private RelayCommand _stopCyclingOverXaxis;

        public IRelayCommand StopCyclingOverXaxis
        {
            get
            {
                if (_stopCyclingOverXaxis == null)
                    _stopCyclingOverXaxis = new RelayCommand(PerformStopCyclingOverXaxis);

                return _stopCyclingOverXaxis;
            }
        }

        private void PerformStopCyclingOverXaxis()
        {
            IsCyclingOverXaxis = false;
            _axes.Stop.Execute(null);
        }

        private bool _isCyclingOverYaxis;

        public bool IsCyclingOverYaxis
        {
            get => _isCyclingOverYaxis;
            set
            {
                SetProperty(ref _isCyclingOverYaxis, value);
                StartCyclingOverXaxis.NotifyCanExecuteChanged();
                StartCyclingOverYaxis.NotifyCanExecuteChanged();
            }
        }

        private AsyncRelayCommand _startCyclingOverYaxis;

        public IAsyncRelayCommand StartCyclingOverYaxis
        {
            get
            {
                if (_startCyclingOverYaxis == null)
                {
                    _startCyclingOverYaxis = new AsyncRelayCommand(PerformCyclingOverYaxis, CanDoMoveY);
                }

                return _startCyclingOverYaxis;
            }
        }

        private async Task PerformCyclingOverYaxis()
        {

            IsCyclingOverYaxis = true;
            while (IsCyclingOverYaxis)
            {
                double destination = _axes.Position.Y < 45 ? 49 : -49;
                await _axes.DoMoveY(destination);
            }
        }

        private RelayCommand _stopCyclingOverYaxis;

        public IRelayCommand StopCyclingOverYaxis
        {
            get
            {
                if (_stopCyclingOverYaxis == null)
                    _stopCyclingOverYaxis = new RelayCommand(PerformStopCyclingOverYaxis);

                return _stopCyclingOverYaxis;
            }
        }

        private void PerformStopCyclingOverYaxis()
        {
            IsCyclingOverYaxis = false;
            _axes.Stop.Execute(null);
        }
        private AutoRelayCommand _skipCommand;
        public AutoRelayCommand SkipCommand
        {
            get
            {
                return _skipCommand ?? (_skipCommand = new AutoRelayCommand(
                    () =>
                    {
                        IsValidated = true;
                        NavigationManager.NavigateToNextPage();
                    },
                    () => { return true; }
                ));
            }
        }
        private AutoRelayCommand _validateChoice;
        public AutoRelayCommand ValidateChoice
        {
            get
            {
                return _validateChoice ?? (_validateChoice = new AutoRelayCommand(
                    () =>
                    {
                        IsValidated = true;
                        NavigationManager.NavigateToNextPage();
                    },
                    () => { return true; }
                ));
            }
        }

        private bool CanDoMoveX()
        {
            return !_isCyclingOverYaxis;
        }

        private bool CanDoMoveY()
        {
            return !_isCyclingOverXaxis;
        }
    }
}
