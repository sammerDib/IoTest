using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.ANA.Client.Proxy.Alignment;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.ANA.Client.Modules.ProbeAlignment.ViewModel.LiseHF
{
    public class LiseHFXYAnalysisVM : ObservableObject, IDisposable, INavigable, IWizardNavigationItem
    {
        #region Constants

        private const int MATRIX_SIZE = 21;

        #endregion

        #region Fields

        private string _probeRef;
        private bool _isMeasurementRunning = false;
        private RelayCommand _startCommand;
        private RelayCommand _stopCommand;
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
        private ProbeLiseHFVM _probeLiseHFVM;
        private ProbeAlignmentSupervisor _probeAlignmentSupervisor;

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

        public RelayCommand StartCommand
        {
            get => _startCommand ?? (_startCommand = new RelayCommand(StartMeasurement));
        }

        public RelayCommand StopCommand
        {
            get => _stopCommand ?? (_stopCommand = new RelayCommand(StopMeasurement));
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

        public string Name { get; set; } = "XY Analysis";
        public bool IsEnabled { get; set; } = true;
        public bool IsMeasure { get; set; } = true;
        public bool IsValidated { get; set; } = false;

        #endregion

        #region Constructor

        public LiseHFXYAnalysisVM(ProbeLiseHFVM probeLiseHFVM)
        {
            _probeLiseHFVM = probeLiseHFVM;
            ProbeRef = probeLiseHFVM.Name;

            _probeAlignmentSupervisor = ClassLocator.Default.GetInstance<ProbeAlignmentSupervisor>();

            for (int i = 0; i < MATRIX_SIZE; i++)
            {
                Matrix.Add(150);
                MatrixColor.Add(new SolidColorBrush());
            }

            Matrix.CollectionChanged += (sender, args) =>
            {
                ComputeData().ContinueWith((task) => ComputeColors());
            };
        }

        #endregion

        #region Methods

        private void StartMeasurement()
        {
            _probeLiseHFVM.RawSignalUpdated += ProbeRawSignalUpdated;
            _probeLiseHFVM.StartContinuousAcquisition();

            var input = new AlignmentLiseHFXYAnalysisInput { StepSize = StepSize };
            _probeAlignmentSupervisor.StartXYMeasurement(input);

            IsMeasurementRunning = true;
        }

        private void StopMeasurement()
        {
            _probeLiseHFVM.RawSignalUpdated -= ProbeRawSignalUpdated;
            _probeAlignmentSupervisor.InterruptXYMeasurement();

            IsMeasurementRunning = false;
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
        }

        public void Dispose()
        {
            StopMeasurement();
        }

        public void UpdateConfig(object config)
        {
            //TODO
        }

        #endregion

        public Task PrepareToDisplay()
        {
            return Task.CompletedTask;
        }

        public bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            Dispose();
            return true;
        }
    }
}
