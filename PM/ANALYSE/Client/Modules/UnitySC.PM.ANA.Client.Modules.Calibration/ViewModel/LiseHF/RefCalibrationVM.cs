using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.LiseHF
{
    public class RefCalibrationVM: ObservableObject
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

        private double? _standardIntegrationTime = null;

        public double? StandardIntegrationTime
        {
            get => _standardIntegrationTime; set { if (_standardIntegrationTime != value) { _standardIntegrationTime = value; OnPropertyChanged(); } }
        }

        private double? _standardMaxCount = null;

        public double? StandardMaxCount
        {
            get => _standardMaxCount; set { if (_standardMaxCount != value) { _standardMaxCount = value; OnPropertyChanged(); } }
        }

        private double? _lowIllumIntegrationTime = null;

        public double? LowIllumIntegrationTime
        {
            get => _lowIllumIntegrationTime; set { if (_lowIllumIntegrationTime != value) { _lowIllumIntegrationTime = value; OnPropertyChanged(); } }
        }

        private double? _lowIllumMaxCount = null;

        public double? LowIllumMaxCount
        {
            get => _lowIllumMaxCount; set { if (_lowIllumMaxCount != value) { _lowIllumMaxCount = value; OnPropertyChanged(); } }
        }


        private List<double> _standardSignal = null;

        /**
         * Store the raw signal for standard filter
         */
        public List<double> StandardSignal
        {
            get => _standardSignal;
            set
            {
                if (_standardSignal != value)
                {
                    _standardSignal = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private List<double> _lowIllumSignal = null;

        /**
         * Store the raw signal for low illum filter
         */
        public List<double> LowIllumSignal
        {
            get => _lowIllumSignal;
            set
            {
                if (_lowIllumSignal != value)
                {
                    _lowIllumSignal = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<double> _waveSignal = null;

        /**
         * Store the raw signal for low illum filter
         */
        public List<double> WaveSignal
        {
            get => _waveSignal;
            set
            {
                if (_waveSignal != value)
                {
                    _waveSignal = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
