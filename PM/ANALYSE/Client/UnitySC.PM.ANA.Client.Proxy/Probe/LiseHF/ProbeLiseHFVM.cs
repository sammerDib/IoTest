using System;
using System.Linq;
using System.Windows;

using LightningChartLib.WPF.ChartingMVVM;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.ProbeLiseHF;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    public class ProbeLiseHFVM : ProbeBaseVM
    {
        #region Constructors

        public ProbeLiseHFVM(IProbeService probeSupervisor, string probeID) : base(probeSupervisor, probeID)
        {
        }

        #endregion Constructors

        #region Events

        public delegate void RawSignalUpdatedHandler(ProbeSignalBase probeRawSignal);

        public event RawSignalUpdatedHandler RawSignalUpdated;

        public delegate void CalibrationTerminatedHandler(ProbeCalibResultsBase probeCalibResults);

        public event CalibrationTerminatedHandler CalibrationTerminated;

        #endregion Events

        #region Properties

        private ProbeInputParametersLiseHFVM _inputParametersLiseHF = null;

        public ProbeInputParametersLiseHFVM InputParametersLiseHF
        {
            get
            {
                if (_inputParametersLiseHF == null)
                {
                    _inputParametersLiseHF = new ProbeInputParametersLiseHFVM();
                    _inputParametersLiseHF.PropertyChanged += InputParametersLiseHF_PropertyChanged;
                }

                return _inputParametersLiseHF;
            }

            set
            {
                if (_inputParametersLiseHF == value)
                {
                    return;
                }

                if (_inputParametersLiseHF != null)
                {
                    InputParametersLiseHF.PropertyChanged -= InputParametersLiseHF_PropertyChanged;
                }

                InputParametersLiseHF.PropertyChanged += InputParametersLiseHF_PropertyChanged;
                _inputParametersLiseHF = value;

                OnPropertyChanged();
            }
        }

        private void InputParametersLiseHF_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (IsContinuousAcquisitionInProgress)
                StartContinuousAcquisition();

            if ((e.PropertyName == nameof(ProbeInputParametersLiseHFVM.IsLowIlluminationPower)) ||
                (e.PropertyName == nameof(ProbeInputParametersLiseHFVM.ObjectiveId)))
            {
                var calibParam = new LiseHFCalibParams();

                IsCalibrated = _probeSupervisor.IsCalibrated(DeviceID, calibParam, InputParametersLiseHF.GetLiseHFInputParams()).Result;

            }
        }

        private SeriesPoint[] _rawAcquisitionPoints;

        public SeriesPoint[] RawAcquisitionPoints
        {
            get
            {
                return _rawAcquisitionPoints;
            }

            set
            {
                bool bNeedUpdateScale = (value != null) && (_rawAcquisitionPoints == null);

                _rawAcquisitionPoints = value;
                OnPropertyChanged();

                // we dont want to update graph zoom scale automatically for now (user should action only)
                // except if no prior data where enter
                if(bNeedUpdateScale)
                    UpdateGraphScale();
            }
        }

        private void UpdateGraphScale()
        {
            var query = RawAcquisitionPoints.GroupBy(i => i.Tag).Select(grp => new
            {
                XMax = grp.Max(t => t.X),
                YMax = grp.Max(t => t.Y),
            });

            XMax = query.Max(r => r.XMax) * (2.0 / 3.0);
            YMax = query.Max(r => r.YMax) * (1.0 / 3.0);

            OnPropertyChanged("XMax");
            OnPropertyChanged("YMax");
        }

        private int _saturationLevel = 0;

        public int SaturationLevel
        {
            get => _saturationLevel; set { if (_saturationLevel != value) { _saturationLevel = value; OnPropertyChanged(); } }
        }

        private double _qualityLevel = 0;

        public double QualityLevel
        {
            get => _qualityLevel; set { if (_qualityLevel != value) { _qualityLevel = value; OnPropertyChanged(); } }
        }

        private double _signalThreshold = 0;

        public double SignalThreshold
        {
            get => _signalThreshold; set { if (_signalThreshold != value) { _signalThreshold = value; OnPropertyChanged(); } }
        }

        private double _signalThresholdPeak = 0;

        public double SignalThresholdPeak
        {
            get => _signalThresholdPeak; set { if (_signalThresholdPeak != value) { _signalThresholdPeak = value;  OnPropertyChanged(); } }
        }
        private double _yMax = 10;

        public double YMax
        {
            get => _yMax; set { if (_yMax != value) { _yMax = value; OnPropertyChanged(); } }
        }

        private double _xMax = 10;

        public double XMax
        {
            get => _xMax; set { if (_xMax != value) { _xMax = value; OnPropertyChanged(); } }
        }

        private ProbeLiseHFCalibResult _calibrationResult;

        public ProbeLiseHFCalibResult CalibrationResult
        {
            get
            {
                return _calibrationResult;
            }

            set
            {
                if (_calibrationResult == value)
                {
                    return;
                }

                _calibrationResult = value;
                OnPropertyChanged();
            }
        }

        private bool _isCalibrated = false;

        public bool IsCalibrated
        {
            get
            {
                return _isCalibrated;
            }

            set
            {
                if (_isCalibrated == value)
                {
                    return;
                }

                _isCalibrated = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region Methods

        public override void SetCalibrationResult(ProbeCalibResultsBase probeCalibrationResults)
        {
            CalibrationResult = probeCalibrationResults as ProbeLiseHFCalibResult;
            if (probeCalibrationResults.Success)
                IsCalibrated = true;
            else
                IsCalibrated = false;

            CalibrationTerminated?.Invoke(probeCalibrationResults);
        }

        public override void SetRawSignal(ProbeSignalBase rawSignal)
        {
            if (!(rawSignal is ProbeLiseHFSignal))
                return;
            Application.Current?.Dispatcher.BeginInvoke(new Action(() => UpdateProbeRawSignal((ProbeLiseHFSignal)rawSignal)));
            RawSignalUpdated?.Invoke(rawSignal);
        }

        private void UpdateProbeRawSignal(ProbeLiseHFSignal rawSignal)
        {
            try
            {
                var rawAcquisitionPoints = new SeriesPoint[rawSignal.RawValues.Count];

                double stepXscaled = (double)(rawSignal.StepX) / 1000.0;
                for (int i = 0; i < rawSignal.RawValues.Count; i++)
                    rawAcquisitionPoints[i] = new SeriesPoint() { X = i * stepXscaled, Y = rawSignal.RawValues[i] };
                RawAcquisitionPoints = rawAcquisitionPoints;

                SaturationLevel = rawSignal.SaturationLevel;

                SignalThreshold = rawSignal.Threshold;

                SignalThresholdPeak = rawSignal.ThresholdPeak;

                QualityLevel = rawSignal.Quality;
            }
            catch (Exception )
            {
                // When the Lise Graph control is unloaded an exception can be generated due to the binding
            }
        }

        public void StartContinuousAcquisition()
        {
            StartContinuousAcquisition(InputParametersLiseHF.GetLiseHFInputParams());
        }

        #endregion Methods
    }
}
