using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.DMT.Shared.UI.ViewModel
{
    public class ExposureSettingsVM : ViewModelBaseExt
    {
        protected readonly CameraSupervisor CameraSupervisor;
        
        private double _exposureTimeMs = double.NaN;

        public double ExposureTimeMs
        {
            get => _exposureTimeMs;
            set => SetProperty(ref _exposureTimeMs, value);
        }

        private double _editExposureTime = double.NaN;

        public double EditExposureTime
        {
            get => _editExposureTime;
            set
            {
                if (SetProperty(ref _editExposureTime, value))
                {
                    ExposureTimeStatus = _editExposureTime == _exposureTimeMs ? ExposureTimeStatus.Valid : ExposureTimeStatus.Modified;
                }
            }
        }

        private Side _waferSide;

        public Side WaferSide
        {
            get => _waferSide;
            set => SetProperty(ref _waferSide, value);
        }
        
        private ExposureTimeStatus _exposureTimeStatus = ExposureTimeStatus.InProgress;

        public ExposureTimeStatus ExposureTimeStatus
        {
            get => _exposureTimeStatus;
            set
            {
                if (SetProperty(ref _exposureTimeStatus, value))
                {
                    ApplyExposureSettings.NotifyCanExecuteChanged();
                }
            }
        }

        public ExposureSettingsVM(Side waferSide, CameraSupervisor cameraSupervisor)
        {
            CameraSupervisor = cameraSupervisor;
            WaferSide = waferSide;
        }

        private AutoRelayCommand _applyExposureSettings;
        
        public AutoRelayCommand ApplyExposureSettings
        {
            get
            {
                return _applyExposureSettings ?? (_applyExposureSettings = new AutoRelayCommand(() =>
                {
                    ExposureTimeMs = _editExposureTime;
                    ExposureTimeStatus = ExposureTimeStatus.Valid;
                    CameraSupervisor.SetExposureTime(WaferSide, ExposureTimeMs);
                },
                    () =>  ExposureTimeStatus == ExposureTimeStatus.Modified));
            }
        }
    }
}
