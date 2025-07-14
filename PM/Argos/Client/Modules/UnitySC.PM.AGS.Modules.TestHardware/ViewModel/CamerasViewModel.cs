using GalaSoft.MvvmLight.CommandWpf;

using UnitySC.PM.AGS.Modules.TestHardware.Dialog;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.AGS.Modules.TestHardware.ViewModel
{
    public class CamerasViewModel : SettingVM
    {
        #region Fields

        private IDialogOwnerService _dialogService;

        #endregion Fields

        #region Propetties

        private double _gain;

        public double Gain
        {
            get
            {
                return _gain;
            }
            set
            {
                if (_gain == value)
                    return;
                _gain = value;
                RaisePropertyChanged(nameof(Gain));
            }
        }

        private double _maxgain;

        public double MaxGain
        {
            get
            {
                return _maxgain;
            }
            set
            {
                if (_maxgain == value)
                    return;
                _maxgain = value;
                RaisePropertyChanged(nameof(MaxGain));
            }
        }

        private double _mingain;

        public double MinGain
        {
            get
            {
                return _mingain;
            }
            set
            {
                if (_mingain == value)
                    return;
                _mingain = value;
                RaisePropertyChanged(nameof(MinGain));
            }
        }

        private double _expostureTime;

        public double ExposureTime
        {
            get
            {
                return _expostureTime;
            }
            set
            {
                if (_expostureTime == value)
                    return;
                _expostureTime = value;
                RaisePropertyChanged(nameof(ExposureTime));
            }
        }

        private double _minexpostureTime;

        public double MinExposureTime
        {
            get
            {
                return _minexpostureTime;
            }
            set
            {
                if (_minexpostureTime == value)
                    return;
                _minexpostureTime = value;
                RaisePropertyChanged(nameof(MinExposureTime));
            }
        }

        private double _maxexpostureTime;

        public double MaxExposureTime
        {
            get
            {
                return _maxexpostureTime;
            }
            set
            {
                if (_maxexpostureTime == value)
                    return;
                _maxexpostureTime = value;
                RaisePropertyChanged(nameof(MaxExposureTime));
            }
        }

        private double _frameRate;

        public double FrameRate
        {
            get
            {
                return _frameRate;
            }
            set
            {
                if (_frameRate == value)
                    return;
                _frameRate = value;
                RaisePropertyChanged(nameof(FrameRate));
            }
        }

        #endregion Propetties

        public CamerasViewModel()
        {
            Header = "Camera";
            IsEnabled = true;
            //_cameraID = cameraConfig.DeviceID;
            // _name = cameraConfig.Name;
            _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
        }

        #region Command

        private RelayCommand _showDetailsCommand;

        public RelayCommand ShowDetailsCommand
        {
            get
            {
                return _showDetailsCommand ?? (_showDetailsCommand = new RelayCommand(
              () =>
              {
                  ShowCameraDetail();
              },
              () => { return true; }));
            }
        }

        #endregion Command

        #region Methode

        public void ShowCameraDetail()
        {
            var dialogViewModel = new CameraDetailsDialogViewModel();
            _dialogService.ShowCustomDialog<CameraDetailsDialog>(dialogViewModel);
        }

        #endregion Methode
    }
}
