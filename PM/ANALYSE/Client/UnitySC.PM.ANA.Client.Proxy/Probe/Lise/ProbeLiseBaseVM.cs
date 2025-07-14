using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

using LightningChartLib.WPF.ChartingMVVM;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;
using System.Runtime.InteropServices;

namespace UnitySC.PM.ANA.Client.Proxy.Probe
{
    public static class DispatcherUtil
    {
        //[SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        private static object ExitFrame(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
    }

    public abstract class ProbeLiseBaseVM : ProbeBaseVM
    {
        #region Constructors

        public ProbeLiseBaseVM(IProbeService probeSupervisor, string probeId) : base(probeSupervisor, probeId)
        {
        }

        #endregion Constructors

        #region Events

        public delegate void RawSignalUpdatedHandler(ProbeSignalBase probeRawSignal);

        public event RawSignalUpdatedHandler RawSignalUpdated;

        public delegate void ThicknessMeasureUpdatedHandler(IProbeResult probeResult);

        public event ThicknessMeasureUpdatedHandler ThicknessMeasureUpdated;

        #endregion Events

        #region Fields

        protected ProbeResultsLiseVM OutputParametersLiseVM { get; set; } = null;

        #endregion Fields

        #region Properties

        private bool _isAcquiring = false;

        public bool IsAcquiring
        {
            get
            {
                return _isAcquiring;
            }
            set
            {
                if (_isAcquiring == value)
                {
                    return;
                }
                _isAcquiring = value;
                OnPropertyChanged();
            }
        }

        private ObjectiveConfig _selectedObjective;

        public ObjectiveConfig SelectedObjective
        {
            get
            {
                return _selectedObjective;
            }

            set
            {
                if (_selectedObjective == value)
                {
                    return;
                }

                _selectedObjective = value;
                OnPropertyChanged();
            }
        }

        private bool _settingsChanged = false;

        public bool SettingsChanged
        {
            get => _settingsChanged; set { if (_settingsChanged != value) { _settingsChanged = value; OnPropertyChanged(); } }
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
                _rawAcquisitionPoints = value;
                OnPropertyChanged();
            }
        }

        private SeriesPoint[] _selectedPeaksPoints;

        public SeriesPoint[] SelectedPeaksPoints
        {
            get
            {
                return _selectedPeaksPoints;
            }

            set
            {
                _selectedPeaksPoints = value;
                OnPropertyChanged();
            }
        }

        private SeriesPoint[] _discardedPeaksPoints;

        public SeriesPoint[] DiscarderPeaksPoints
        {
            get
            {
                return _discardedPeaksPoints;
            }

            set
            {
                _discardedPeaksPoints = value;
                OnPropertyChanged();
            }
        }

        private double _saturation = 0;

        public double Saturation
        {
            get
            {
                return _saturation;
            }
            set
            {
                if (_saturation == value)
                {
                    return;
                }

                _saturation = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region RelayCommands

        private AutoRelayCommand _applySettings;

        public AutoRelayCommand ApplySettings
        {
            get
            {
                return _applySettings
                       ?? (_applySettings = new AutoRelayCommand(
                           () =>
                           {
                               StopContinuousAcquisition();
                               var inputParams = GetInputParametersForAcquisition();
                               StartContinuousAcquisition(inputParams);
                           }, () =>
                           {
                               return CheckInputParametersValidity() && (State.Status == DeviceStatus.Busy);
                           }));
            }
        }

        private AutoRelayCommand _doMeasure;

        public new AutoRelayCommand DoMeasure
        {
            get
            {
                return _doMeasure
                    ?? (_doMeasure = new AutoRelayCommand(
                    () =>
                    {
                        var inputParametersLise = GetInputParametersForAcquisition();
                        if (inputParametersLise != null)
                        {
                            var mapper = ClassLocator.Default.GetInstance<Mapper>();
                            var resultsLise = mapper.AutoMap.Map<LiseResult>(OutputParametersLiseVM);

                            var result = DoMeasure(inputParametersLise);
                            if (result != null)
                            {
                                for (int i = 0; i < (result as IProbeThicknessesResult).LayersThickness.Count; i++)
                                {
                                    Sample.ObservableLayers[i].MeasuredThickness = (result as IProbeThicknessesResult).LayersThickness[i].Thickness.Micrometers;
                                    Sample.ObservableLayers[i].MeasuredQuality = (result as IProbeThicknessesResult).LayersThickness[i].Quality;
                                }
                            }
                        }

                        SettingsChanged = false;
                    }, () =>
                    {
                        return CheckInputParametersValidity() && (State.Status != DeviceStatus.Busy);
                    }));
            }
        }

        private AutoRelayCommand _addSampleLayer;

        public AutoRelayCommand AddSampleLayer
        {
            get
            {
                return _addSampleLayer
                    ?? (_addSampleLayer = new AutoRelayCommand(
                    () =>
                    {
                        // For debug
                        var newSampleLayer = new SampleLayer();
                        newSampleLayer.Thickness = 750.Micrometers();
                        var tolerance = new LengthTolerance(10, LengthToleranceUnit.Micrometer);
                        newSampleLayer.Tolerance = tolerance;
                        newSampleLayer.RefractionIndex = 1.4621;

                        newSampleLayer.PropertyChanged += SampleLayer_PropertyChanged;

                        Sample.ObservableLayers.Add(newSampleLayer);
                    }));
            }
        }

        private AutoRelayCommand<SampleLayer> _deleteSampleLayer;

        public AutoRelayCommand<SampleLayer> DeleteSampleLayer
        {
            get
            {
                return _deleteSampleLayer
                    ?? (_deleteSampleLayer = new AutoRelayCommand<SampleLayer>(
                    (sampleLayer) =>
                    {
                        sampleLayer.PropertyChanged -= SampleLayer_PropertyChanged;
                        Sample.ObservableLayers.Remove(sampleLayer);
                    },
                    (sampleLayer) => sampleLayer != null
                    ));
            }
        }

        private AutoRelayCommand _startContinuousAcquisition;

        public new AutoRelayCommand StartContinuousAcquisition
        {
            get
            {
                return _startContinuousAcquisition
                       ?? (_startContinuousAcquisition = new AutoRelayCommand(
                           () =>
                           {
                               var inputParams = GetInputParametersForAcquisition();
                               StartContinuousAcquisition(inputParams);
                           },
                           () => CheckInputParametersValidity() && (State.Status != DeviceStatus.Busy)
                       )); ;
            }
        }

        private AutoRelayCommand _stopContinuousAcquisition;

        public new AutoRelayCommand StopContinuousAcquisition
        {
            get
            {
                return _stopContinuousAcquisition
                    ?? (_stopContinuousAcquisition = new AutoRelayCommand(
                    () =>
                    {
                        bool stopAckResult = StopContinuousAcquisition();
                    },
                    () => (State.Status == DeviceStatus.Busy)
                    ));
            }
        }

        #endregion RelayCommands

        #region Methods

        public override void SetRawSignal(ProbeSignalBase rawSignal)
        {   
            if (!(rawSignal is ProbeLiseSignal))
                return;
            Application.Current?.Dispatcher.BeginInvoke(new Action(() => UpdateProbeRawSignal((ProbeLiseSignal)rawSignal)));
            RawSignalUpdated?.Invoke(rawSignal);
        }

         public void ExportMeasures(StreamWriter file, List<LiseResult> rawRepeatMeasure, string deviceId)
        {
            string separator = CSVStringBuilder.GetCSVSeparator();

            file.WriteLine($"Measures for {deviceId}");
            foreach (var measure in rawRepeatMeasure)
            {
                var layersCount = measure.LayersThickness.Count * 2;
                var valuesOfSelectedPoint = new string[layersCount];
                foreach (var layer in measure.LayersThickness.Select((value, j) => new
                {
                    j,
                    value
                }))
                {
                    valuesOfSelectedPoint[2 * (layer.j)] = Math.Round(layer.value.Thickness.Nanometers, 4).ToString(CultureInfo.InvariantCulture);
                    valuesOfSelectedPoint[2 * (layer.j) + 1] = Math.Round(layer.value.Quality, 4).ToString(CultureInfo.InvariantCulture);
                }
                string rowOfSelectedPoints = string.Join(separator, valuesOfSelectedPoint);
                file.WriteLine(rowOfSelectedPoints);
            }
            rawRepeatMeasure.Clear();
        }

        protected void InputParametersLiseVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _settingsChanged = true;
        }

        private void SampleLayer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Check if this refraction index is 0
            if ((sender as SampleLayer).RefractionIndex == 0)
            {
                // We check that there is no other one
                foreach (var sampleLayer in Sample.ObservableLayers)
                {
                    if ((sampleLayer != sender) && (sampleLayer.RefractionIndex == 0))
                        sampleLayer.RefractionIndex = 1;
                }
            }
        }

        private void UpdateProbeRawSignal(ProbeLiseSignal rawSignal)
        {
            if (State.Status == DeviceStatus.Busy)
            {
                try
                {
                    var rawAcquisitionPoints= new SeriesPoint[rawSignal.RawValues.Count];
                    double stepXscaled = (double)(rawSignal.StepX) /  1000.0;
                    for (int i = 0; i < rawSignal.RawValues.Count; i++)
                        rawAcquisitionPoints[i] = new SeriesPoint() { X = i * stepXscaled, Y = rawSignal.RawValues[i] };
                    RawAcquisitionPoints = rawAcquisitionPoints;
                    
                    var selectedPeaksPoints = new SeriesPoint[rawSignal.SelectedPeaks.Count];
                    for (int i = 0; i < rawSignal.SelectedPeaks.Count; i++)
                    {
                        selectedPeaksPoints[i] = new SeriesPoint()
                        {
                            X = rawSignal.SelectedPeaks[i].X * stepXscaled,
                            Y = rawSignal.SelectedPeaks[i].Y,
                            PointColor = (Color)ColorConverter.ConvertFromString("#FF00FF00")
                        };
                    }
                    SelectedPeaksPoints = selectedPeaksPoints;

                    var discarderPeaksPoints = new SeriesPoint[rawSignal.DiscardedPeaks.Count];
                    for (int i = 0; i < rawSignal.DiscardedPeaks.Count; i++)
                    {
                        discarderPeaksPoints[i] = new SeriesPoint()
                        {
                            X = rawSignal.DiscardedPeaks[i].X * stepXscaled,
                            Y = rawSignal.DiscardedPeaks[i].Y,
                            PointColor = (Color)ColorConverter.ConvertFromString("#FFFF0000")
                        };
                    }
                    DiscarderPeaksPoints = discarderPeaksPoints;

                    Saturation = rawSignal.SaturationValue;
                }
                catch (Exception)
                {
                    // When the Lise Graph control is unloaded an exception can be generated due to the binding
                }
            }
        }

        // Generates ProbeInputParametersLise from ProbeInputParametersLiseVM
        // ProbeInputParametersLiseVM is used by the HMI Layer
        // ProbeInputParametersLise is used by the hardware Layer
        public abstract ILiseInputParams GetInputParametersForAcquisition();

        public abstract bool CheckInputParametersValidity();

        public abstract Sample Sample { get; }

        #endregion Methods
    }
}
