using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface;

namespace UnitySC.PM.ANA.Client.Modules.ProbeAlignment.ViewModel.Lise
{
    public class LiseXYAnalysisVM : ObservableObject, IDisposable
    {
        #region Constants

        private const int MATRIX_SIZE = 21;

        #endregion

        #region Fields

        private string _probeRef;
        private bool _isMeasurementRunning = false;
        private AsyncRelayCommand _startCommand;
        private AsyncRelayCommand _stopCommand;
        private ObservableCollection<double> _matrix = new ObservableCollection<double>();
        private ObservableCollection<SolidColorBrush> _matrixColor = new ObservableCollection<SolidColorBrush>();
        private double _matrixAverage = -1;
        private double _matrixRange = -1;
        private double _defocus = -1;
        private double _stdDev = -1;
        private double _modGrad = -1;
        private double _grad45 = -1;
        private double _grad135 = -1;
        private double _stepSize = 1;
        private double _target = 150;
        private ProbeLiseVM _probeLiseVM;

        #endregion

        #region Properties

        public string ProbeRef
        {
            get => _probeRef;
            set => SetProperty(ref _probeRef, value);
        }

        public bool IsMeasurementRunning
        {
            get => _isMeasurementRunning;
            set => SetProperty(ref _isMeasurementRunning, value);
        }

        public AsyncRelayCommand StartCommand
        {
            get => _startCommand ?? (_startCommand = new AsyncRelayCommand(StartMeasurement));
        }

        public AsyncRelayCommand StopCommand
        {
            get => _stopCommand ?? (_stopCommand = new AsyncRelayCommand(StopMeasurement));
        }

        public ObservableCollection<double> Matrix
        {
            get => _matrix;
            set => SetProperty(ref _matrix, value);
        }

        public ObservableCollection<SolidColorBrush> MatrixColor
        {
            get => _matrixColor;
            set => SetProperty(ref _matrixColor, value);
        }

        public double MatrixAverage
        {
            get => _matrixAverage;
            set => SetProperty(ref _matrixAverage, value);
        }

        public double MatrixRange
        {
            get => _matrixRange;
            set => SetProperty(ref _matrixRange, value);
        }

        public double Defocus
        {
            get => _defocus;
            set => SetProperty(ref _defocus, value);
        }

        public double StdDev
        {
            get => _stdDev;
            set => SetProperty(ref _stdDev, value);
        }

        public double ModGrad
        {
            get => _modGrad;
            set => SetProperty(ref _modGrad, value);
        }

        public double Grad45
        {
            get => _grad45;
            set => SetProperty(ref _grad45, value);
        }

        public double Grad135
        {
            get => _grad135;
            set => SetProperty(ref _grad135, value);
        }

        public double StepSize
        {
            get => _stepSize;
            set => SetProperty(ref _stepSize, value);
        }

        #endregion

        #region Constructor

        public LiseXYAnalysisVM(ProbeLiseVM probeLiseVM)
        {
            _probeLiseVM = probeLiseVM;
            ProbeRef = probeLiseVM.Name;

            for (int i = 0; i < MATRIX_SIZE; i++)
            {
                Matrix.Add(150);
                MatrixColor.Add(new SolidColorBrush());
            }
            
            _probeLiseVM.StopContinuousAcquisition();

            Matrix.CollectionChanged += (sender, args) =>
            {
                ComputeData();
                ComputeColors();
            };
        }

        #endregion

        #region Methods

        private async Task StartMeasurement()
        {
            _probeLiseVM.StopContinuousAcquisition();
            _probeLiseVM.RawSignalUpdated += ProbeRawSignalUpdated;
            _probeLiseVM.ThicknessMeasureUpdated += ProbeThicknessMeasureUpdated;


            IsMeasurementRunning = true;
            SimulateValues();
        }

        private async Task StopMeasurement()
        {
            _probeLiseVM.RawSignalUpdated -= ProbeRawSignalUpdated;
            _probeLiseVM.ThicknessMeasureUpdated -= ProbeThicknessMeasureUpdated;

            IsMeasurementRunning = false;
        }

        // TODO ONLY FOR DEV
        private async Task SimulateValues()
        {
            var rand = new Random(123);
            while (IsMeasurementRunning)
            {
                for (int i = 0; i < MATRIX_SIZE; i++)
                {
                    double increment = 10 * rand.NextDouble();
                    if (Matrix[i] < _target)
                    {
                        Matrix[i] += increment;
                    }
                    else
                    {
                        Matrix[i] -= increment;
                    }
                }

                _probeLiseVM.DoSingleAcquisition(null);
                await Task.Delay(500);
            }
        }

        private async Task ComputeData()
        {
            MatrixAverage = Matrix.Average();
            MatrixRange = Matrix.Max() - Matrix.Min();
        }

        private async Task ComputeColors()
        {
            int LinearInterp(int start, int end, double percentage) =>
                start + (int)Math.Round(percentage * (end - start));

            Color ColorInterp(Color start, Color end, double percentage) =>
                Color.FromArgb((byte)LinearInterp(start.A, end.A, percentage),
                    (byte)LinearInterp(start.R, end.R, percentage),
                    (byte)LinearInterp(start.G, end.G, percentage),
                    (byte)LinearInterp(start.B, end.B, percentage));

            _target = Math.Round((500 * StepSize / 3) - 16.0 - 2.0 / 3.0, 4);
            double acceptability = 50;

            var lowColor = (Color)ColorConverter.ConvertFromString("Blue");
            var perfectColor = (Color)ColorConverter.ConvertFromString("Green");
            var highColor = (Color)ColorConverter.ConvertFromString("Red");

            double min = _target - acceptability;
            double max = _target + acceptability;
            double distanceMax = acceptability;

            for (int i = 0; i < MATRIX_SIZE; i++)
            {
                double value = Matrix[i];
                double distance = Math.Abs(value - _target);
                double percentage = Math.Min(Math.Max(distance / distanceMax, 0), 1);
                var valueColor = ColorInterp(perfectColor, value > _target ? highColor : lowColor,
                    percentage);
                Color.FromArgb(255, valueColor.R, valueColor.G, valueColor.B);
                MatrixColor[i] = new SolidColorBrush(valueColor);
            }
        }


        private void ProbeRawSignalUpdated(ProbeSignalBase probeRawSignal)
        {
            var test2 = probeRawSignal.RawValues.ToArray();
            Defocus = test2.Sum();
        }

        private void ProbeThicknessMeasureUpdated(IProbeResult proberesult)
        {
            throw new NotImplementedException();
        }


        public void Dispose()
        {
            StopMeasurement().Wait();
        }

        #endregion
    }
}
