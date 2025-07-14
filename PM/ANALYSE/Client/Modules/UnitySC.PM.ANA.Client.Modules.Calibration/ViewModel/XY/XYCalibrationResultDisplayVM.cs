using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using LightningChartLib.WPF.Charting;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using MathNet.Numerics.Providers.LinearAlgebra;

using MvvmDialogs;

using UnitySC.PM.ANA.Service.Core.Referentials.Converters;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.XY
{
    public class XYPoints
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double ShiftX { get; set; }
        public double ShiftY { get; set; }

        public double ShiftXY { get { return XYCalibrationResultDisplayVM.Norm(ShiftX, ShiftY); } }
        public double AngleXY { get { double rad = Math.Atan2(ShiftY, ShiftX); return double.IsNaN(rad) ? rad : 180.0 * rad / Math.PI; } }
    }

    public class XYCalibrationResultDisplayVM : ObservableObject, IModalDialogViewModel, IDisposable
    {
        static public double Norm(double x, double y) { return Math.Sqrt(x * x + y * y); }
        static public Length StagePrecision { get; set; } = new Length(10, LengthUnit.Micrometer);
        static public int StagePrecisionScaleFactor { get; set; } = 5;
        static public int StagePrecisionScaleFactorTest { get; set; } = 5;

        private bool? _dialogResult;

        public XYCalibResultVectorHeatMapVM XYVectorHeatMapVM { get; set; }

        private XYCalibrationData _calibrationData;

        public XYCalibrationData CalibrationData
        {
            get => _calibrationData; set { if (_calibrationData != value) { _calibrationData = value; OnPropertyChanged(); } }
        }

        private bool _isTestCalibration;

        public bool IsTestCalibration
        {
            get => _isTestCalibration; set { if (_isTestCalibration != value) { _isTestCalibration = value; OnPropertyChanged(); } }
        }

        private bool _isValid;

        public bool IsValid
        {
            get => _isValid; set { if (_isValid != value) { _isValid = value; OnPropertyChanged(); } }
        }

        private string _displayLabel;
        public string DisplayLabel
        {
            get => _displayLabel; set { if (_displayLabel != value) { _displayLabel = value; OnPropertyChanged(); } }
        }


        private ObservableCollection<XYPoints> _badPoints;

        public ObservableCollection<XYPoints> BadPoints
        {
            get => _badPoints; set { if (_badPoints != value) { _badPoints = value; OnPropertyChanged(); } }
        }

        private System.Windows.Point _lastLeftMouseDownPosition;

        public XYCalibrationResultDisplayVM(XYCalibrationData calibrationData, XYCalibrationData stagecorrectionApplied = null, string display = null)
        {
            CalibrationData = calibrationData;
            if (calibrationData is XYCalibrationTest calibrationTest)
            {
                IsTestCalibration = true;
                IsValid = calibrationTest.IsValid;
                BadPoints = GetBadPoints(calibrationTest);
                DisplayLabel = display;
            }
            else
            {
                DisplayLabel = $"Stage Shift Corrections";
            }

            if (CalibrationData.Corrections.Count == 0)
            {
                StatsContainerXY = FloatStatsContainer.Empty;
            }
            else
            {
                // Wafer Vector XY
                var query = CalibrationData.Corrections.Select(c => (float)Norm(c.ShiftX.Micrometers, c.ShiftY.Micrometers)).ToList<float>();
                StatsContainerXY = FloatStatsContainer.GenerateFromFloats(query);
            }

            XYVectorHeatMapVM = new XYCalibResultVectorHeatMapVM();
            XYVectorHeatMapVM.Chart.MouseClick += HeatMapChart_MouseClick;
            XYVectorHeatMapVM.Chart.MouseDown += HeatMapChart_MouseDown;
            XYVectorHeatMapVM.StageCorrectionApplied = stagecorrectionApplied;

            if (calibrationData != null)
            {
                var specValueMin = -StagePrecision.Micrometers; // um 
                var specValueMax = +StagePrecision.Micrometers; // um

                if (IsTestCalibration)
                {
                    // need to add a way to obtain stage précision from a stage to another (NST, TMAP etc...)
                    specValueMin *= StagePrecisionScaleFactorTest; // um NST7 précision stage * test factor
                    specValueMax *= StagePrecisionScaleFactorTest; // um NST7 précision stage * test factor
                }
                else
                {
                    specValueMin *= StagePrecisionScaleFactor; // um NST7 x3 précision stage * factor
                    specValueMax *= StagePrecisionScaleFactor; // um NST7 x3 précision stage * factor
                }
                AdvancedSpecMax = specValueMax;
                XYVectorHeatMapVM.Update(calibrationData, specValueMin, specValueMax);
            }
            _displayPoint = new XYPoints();
        }

        private void HeatMapChart_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right || e.ChangedButton == MouseButton.Middle)
                return;

            _lastLeftMouseDownPosition = e.GetPosition(sender as LightningChart);
        }

        private void HeatMapChart_MouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right || e.ChangedButton == MouseButton.Middle)
                return;

            //Find nearest point and show its values 
            var currentChart = sender as LightningChart;
            var position = e.GetPosition(currentChart);

            // if we have move since we have clicked down this means that we are dragging so do not update displa
            if (_lastLeftMouseDownPosition != position)
                return;

            // Get the point last hit by mouse.
            currentChart.ViewXY.XAxes[0].CoordToValue((int)position.X, out double xValue, false);
            currentChart.ViewXY.YAxes[0].CoordToValue((float)position.Y, out double yValue);
            DisplayPoint.X = xValue;
            DisplayPoint.Y = yValue;

            var Shifts = XYCalibrationHelper.ComputeCorrection(xValue, yValue, _calibrationData);
            DisplayPoint.ShiftX = Shifts.Item1.GetValueAs(XYCalibrationData.CorrectionUnit);
            DisplayPoint.ShiftY = Shifts.Item2.GetValueAs(XYCalibrationData.CorrectionUnit);
            DisplaySelectedPoint = true;

            XYVectorHeatMapVM.DrawSelectedPoint(xValue, yValue);

            OnPropertyChanged("DisplayPoint");
        }

        private ObservableCollection<XYPoints> GetBadPoints(XYCalibrationTest calibrationTest)
        {
            var badPoints = new ObservableCollection<XYPoints>();
            foreach (var badPoint in calibrationTest.BadPoints)
            {
                badPoints.Add(new XYPoints { X = badPoint.XTheoricalPosition.Millimeters, Y = badPoint.YTheoricalPosition.Millimeters, ShiftX = badPoint.ShiftX.Micrometers, ShiftY = badPoint.ShiftY.Micrometers });
            }
            return badPoints;
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        public void Dispose()
        {
            if (XYVectorHeatMapVM.Chart != null)
            {
                XYVectorHeatMapVM.Chart.MouseClick -= HeatMapChart_MouseDown;
                XYVectorHeatMapVM.Chart.MouseClick -= HeatMapChart_MouseClick;
            }
            XYVectorHeatMapVM?.Dispose();

            DisplayPoint = null;
        }

        private FloatStatsContainer _statsContainerXY;

        public FloatStatsContainer StatsContainerXY
        {
            get { return _statsContainerXY; }
            private set { if (_statsContainerXY != value) { _statsContainerXY = value; OnPropertyChanged(nameof(StatsWaferXY)); } }
        }
        public FloatStatsContainer StatsWaferXY
        {
            get
            {
                if (StatsContainerXY == null) return StatsContainerXY;
                return new FloatStatsContainer(StatsContainerXY.Mean, StatsContainerXY.Min, StatsContainerXY.Max, StatsContainerXY.StdDev, StatsContainerXY.Median);
            }
        }

        private XYPoints _selectedBadPointsItem;
        public XYPoints SelectedBadPointsItem
        {
            get { return _selectedBadPointsItem; }
            set
            {
                if (_selectedBadPointsItem != value)
                {
                    _selectedBadPointsItem = value;

                    DisplayPoint.X = _selectedBadPointsItem.X;
                    DisplayPoint.Y = _selectedBadPointsItem.Y;
                    DisplayPoint.ShiftX = _selectedBadPointsItem.ShiftX;
                    DisplayPoint.ShiftY = _selectedBadPointsItem.ShiftY;
                    DisplaySelectedPoint = true;

                    XYVectorHeatMapVM.DrawSelectedPoint(_selectedBadPointsItem.X, _selectedBadPointsItem.Y);

                    OnPropertyChanged(nameof(SelectedBadPointsItem));
                    OnPropertyChanged(nameof(DisplayPoint));
                }
            }
        }
        private XYPoints _displayPoint;
        public XYPoints DisplayPoint
        {
            get => _displayPoint; set { if (_displayPoint != value) { _displayPoint = value; OnPropertyChanged(); } }
        }
        private bool _displaySelectedPoint = false;
        public bool DisplaySelectedPoint
        {
            get => _displaySelectedPoint; set { if (_displaySelectedPoint != value) { _displaySelectedPoint = value; OnPropertyChanged(); } }
        }

        #region Advanced
        private double _advancedSpecMax;
        public double AdvancedSpecMax
        {
            get => _advancedSpecMax; set { if (_advancedSpecMax != value) { _advancedSpecMax = value; OnPropertyChanged(); } }
        }

        private bool _displayAdvancedControls = false;
        public bool DisplayAdvancedControls
        {
            get => _displayAdvancedControls; set { if (_displayAdvancedControls != value) { _displayAdvancedControls = value; OnPropertyChanged(); } }
        }


        private AutoRelayCommand _refreshDisplayCommand;
        public AutoRelayCommand RefreshDisplayCommand
        {
            get
            {
                return _refreshDisplayCommand ?? (_refreshDisplayCommand = new AutoRelayCommand(
              () =>
              {
                  XYVectorHeatMapVM.AdvancedSpecMax = AdvancedSpecMax;
                  XYVectorHeatMapVM.RefreshDisplay();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _advMaxCommand;
        public AutoRelayCommand AdvMaxCommand
        {
            get
            {
                return _advMaxCommand ?? (_advMaxCommand = new AutoRelayCommand(
              () =>
              {
                  AdvancedSpecMax = XYVectorHeatMapVM.AdvancedMax;
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _advMeanCommand;
        public AutoRelayCommand AdvMeanCommand
        {
            get
            {
                return _advMeanCommand ?? (_advMeanCommand = new AutoRelayCommand(
              () =>
              {
                  AdvancedSpecMax = XYVectorHeatMapVM.AdvancedMean;
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _toggleAdvancedCommand;
        public AutoRelayCommand ToggleAdvancedCommand
        {
            get
            {
                return _toggleAdvancedCommand ?? (_toggleAdvancedCommand = new AutoRelayCommand(
              () =>
              {
                  DisplayAdvancedControls = !DisplayAdvancedControls;
              },
              () => { return true; }));
            }
        }
        #endregion


    }
}
