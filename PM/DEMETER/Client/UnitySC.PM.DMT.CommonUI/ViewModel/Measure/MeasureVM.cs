using System.Windows.Media;

using UnitySC.PM.DMT.Service.Interface.AutoExposure;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.Shared.UI.ViewModels;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.UI.Enum;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.Measure
{
    public abstract class MeasureVM : TabViewModelBase
    {
        public abstract HelpTag HelpTag { get; }
        public virtual Fringe Fringe { get; set; }
        public virtual Color Color { get; set; } = Colors.White;

        private Side _side;

        public Side Side
        { get => _side; set { if (_side != value) { _side = value; OnPropertyChanged(); } } }

        private AutoExposureTimeTrigger _autoExposureTimeTrigger;

        public AutoExposureTimeTrigger AutoExposureTimeTrigger
        {
            get => _autoExposureTimeTrigger;
            set
            {
                if (_autoExposureTimeTrigger != value)
                {
                    _autoExposureTimeTrigger = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsTargetSaturationEditable));
                }
            }
        }

        private double _exposureTimeMs;

        public double ExposureTimeMs
        { get => _exposureTimeMs; set { if (_exposureTimeMs != value) { _exposureTimeMs = value; OnPropertyChanged(); } } }

        // Gray level target for auto exposure
        private int _autoExposureTargetSaturation;

        public int AutoExposureTargetSaturation
        {
            get => _autoExposureTargetSaturation;
            set
            {
                if (_autoExposureTargetSaturation != value)
                {
                    _autoExposureTargetSaturation = value;
                    OnPropertyChanged();
                }
            }
        }

        // Gray level tolerance for the auto exposure target
        public int AutoExposureSaturationTolerance { get; set; }

        // Initial Exposure Time for auto exposure
        public double AutoExposureInitialExposureTime { get; set; }

        // Ratio of pixels above the AutoExposureTargetSaturation
        public double AutoExposureRatioSaturated { get; set; }

        public virtual bool IsTargetSaturationEditable { get; set; } = false;

        public WaferDimensionalCharacteristic WaferDimensions { get; set; }

        private RoiVM _roi;

        public RoiVM ROI
        {
            get => _roi;
            set
            {
                if (_roi != value)
                {
                    if (_roi != null)
                    {
                        _roi.PropertyChanged -= OnRoiPropertyChanged;
                    }
                    _roi = value;
                    OnPropertyChanged();
                    if (_roi != null)
                    {
                        _roi.PropertyChanged += OnRoiPropertyChanged;
                    }
                }
            }
        }

        private void OnRoiPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ROI) + "." + e.PropertyName);
        }
    }
}
