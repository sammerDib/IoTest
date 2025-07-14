using System;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.LiseHF
{
    public class SpotCalibrationVM: ObservableObject
    {


        private StepStates _calibrationStatus = StepStates.NotDone;

        public StepStates CalibrationStatus
        {
            get => _calibrationStatus; set { if (_calibrationStatus != value) { _calibrationStatus = value; OnPropertyChanged(); } }
        }


        private string _errorMessage = "";

        public string ErrorMessage
        {
            get => _errorMessage; set { if (_errorMessage != value) { _errorMessage = value; OnPropertyChanged(); } }
        }

        private string _objectiveID ;

        public string ObjectiveID
        {
            get => _objectiveID; set { if (_objectiveID != value) { _objectiveID = value; OnPropertyChanged(); } }
        }
        private string _objectiveName;

        public string ObjectiveName
        {
            get => _objectiveName; set { if (_objectiveName != value) { _objectiveName = value; OnPropertyChanged(); } }
        }

        private bool _isSelected = false;

        public bool IsSelected
        {
            get => _isSelected; set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        private DateTime? _calibrationDate=null;

        public DateTime? CalibrationDate
        {
            get => _calibrationDate; set { if (_calibrationDate != value) { _calibrationDate = value; OnPropertyChanged(); } }
        }

        private double? _xOffsetum = null;

        public double? XOffsetum
        {
            get => _xOffsetum; set { if (_xOffsetum != value) { _xOffsetum = value; OnPropertyChanged(); } }
        }

        private double? _yOffsetum = null;

        public double? YOffsetum
        {
            get => _yOffsetum; set { if (_yOffsetum != value) { _yOffsetum = value; OnPropertyChanged(); } }
        }

        private Length _pixelSizeX = null;
        public Length PixelSizeX
        {
            get => _pixelSizeX; set { if (_pixelSizeX != value) { _pixelSizeX = value; OnPropertyChanged(); } }
        }

        private Length _pixelSizeY = null;
        public Length PixelSizeY
        {
            get => _pixelSizeY; set { if (_pixelSizeY != value) { _pixelSizeY = value; OnPropertyChanged(); } }
        }

        private double? _camExposureTime_ms = null;
        public double? CamExposureTime_ms
        {
            get => _camExposureTime_ms; set { if (_camExposureTime_ms != value) { _camExposureTime_ms = value; OnPropertyChanged(); } }
        }
    }
}
