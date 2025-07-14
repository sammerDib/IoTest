using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using AutoMapper.Internal;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Win32;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.Hardware.Service.Interface.Screen;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.ViewModel
{
    public class LuminanceScreensViewModel : ObservableRecipient, ITabManager, IRecipient<LuminanceChange>
    {
        private readonly ScreenSupervisor _screenSupervisor;
        private readonly IDialogOwnerService _dialogService;
        private readonly ObservableCollection<LuminancePointsViewModel> _luminancePointsVmBack = new ObservableCollection<LuminancePointsViewModel>();
        private readonly ObservableCollection<LuminancePointsViewModel> _luminancePointsVmFront = new ObservableCollection<LuminancePointsViewModel>();
        private LuminancePointsViewModel _selectedPoints;

        private Side _currentScreenSide;
        private double _measureCircleSize;
        private Length _sensorDiameter = 80.Millimeters();

        private int _screenHeight;
        private ScreenInfo _screenInfo;
        private int _screenWidth;

        private AutoRelayCommand _nextCommand;
        private AutoRelayCommand _previousCommand;
        private AutoRelayCommand _computeAndSave;

        public LuminanceScreensViewModel(Side screenSide, ScreenSupervisor screenSupervisor, IDialogOwnerService dialogService)
        {
            Messenger.RegisterAll(this);
            _screenSupervisor = screenSupervisor;
            _dialogService = dialogService;
            _currentScreenSide = screenSide;
            _screenInfo = _screenSupervisor.GetScreenInfo(screenSide);
            _screenHeight = _screenInfo.Height;
            _screenWidth = _screenInfo.Width;
            _measureCircleSize = _sensorDiameter.Millimeters / _screenInfo.PixelPitchVertical;
        }

        public string Header { get; } = "Screen Qualification";

        public int NbPointHoriz { get; } = 5;

        public int NbPointVertic { get; } = 3;

        public double CircleStrokeTickness { get; } = 4;

        public ObservableCollection<LuminancePointsViewModel> LuminancePointsVmForCurrentSide { get; } = new ObservableCollection<LuminancePointsViewModel>();

        public Side CurrentScreenSide
        {
            get => _currentScreenSide;
            set
            {
                if (_currentScreenSide != value)
                {
                    if (_currentScreenSide != Side.Unknown)
                    {
                        _screenSupervisor.SetScreenColor(_currentScreenSide, Colors.Black, false);
                    }

                    _currentScreenSide = value;
                    _screenInfo = _screenSupervisor.GetScreenInfo(_currentScreenSide);
                    ScreenHeight = _screenInfo.Height;
                    ScreenWidth = _screenInfo.Width;
                    RefreshLuminancePoints(false);
                    OnPropertyChanged();
                }
            }
        }

        public int ScreenWidth
        {
            get => _screenWidth;
            set
            {
                if (_screenWidth != value)
                {
                    _screenWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ScreenHeight
        {
            get => _screenHeight;
            set
            {
                if (_screenHeight != value)
                {
                    _screenHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MeasureCircleSize
        {
            get => _measureCircleSize;
            set
            {
                if (_measureCircleSize != value)
                {
                    _measureCircleSize = value;
                    RefreshLuminancePoints(true);
                    OnPropertyChanged();
                }
            }
        }

        public LuminancePointsViewModel SelectedPoints
        {
            get => _selectedPoints;
            set
            {
                if (_selectedPoints != value && value != null)
                {
                    _selectedPoints = value;
                    OnPropertyChanged();
                    if (_selectedPoints != null && _selectedPoints.LuminancePoints != null && _selectedPoints.LuminancePoints.Any())
                    {
                        _selectedPoints.SelectedPoint = _selectedPoints.LuminancePoints.First();
                    }

                    var imageToDisplay = _selectedPoints.CreateImages(_screenWidth, _screenHeight, CircleStrokeTickness, MeasureCircleSize);
                    _screenSupervisor.DisplayImage(_currentScreenSide, imageToDisplay);

                    NextCommand.NotifyCanExecuteChanged();
                    PreviousCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public Length SensorDiameter
        {
            get => _sensorDiameter;
            set
            {
                if (_sensorDiameter != value)
                {
                    _sensorDiameter = value;
                    MeasureCircleSize = _sensorDiameter.Millimeters / _screenInfo.PixelPitchVertical;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxSensorDiameter
        {
            get => _screenInfo.Width / NbPointHoriz;
        }

        public AutoRelayCommand NextCommand => _nextCommand ?? (_nextCommand = new AutoRelayCommand(
            () => SelectedPoints = LuminancePointsVmForCurrentSide[LuminancePointsVmForCurrentSide.IndexOf(SelectedPoints) + 1],
            () => LuminancePointsVmForCurrentSide.Any() && LuminancePointsVmForCurrentSide.Last() != SelectedPoints));

        public AutoRelayCommand PreviousCommand => _previousCommand ?? (_previousCommand = new AutoRelayCommand(
            () => SelectedPoints = LuminancePointsVmForCurrentSide[LuminancePointsVmForCurrentSide.IndexOf(SelectedPoints) - 1],
            () => LuminancePointsVmForCurrentSide.Any() && LuminancePointsVmForCurrentSide.First() != SelectedPoints));

        public AutoRelayCommand ComputeAndSave => _computeAndSave ?? (_computeAndSave = new AutoRelayCommand(
            () => ComputeAndSaveScreenQualification(),
            () => LuminancePointsVmForCurrentSide.All(x => x.IsValid)
                        && LuminancePointsVmForCurrentSide.Any(x => x.IsModified)));

        public void Receive(LuminanceChange message)
        {
            Validate();
        }

        public void Display()
        {
            RefreshLuminancePoints(false);
        }

        public bool CanHide()
        {
            return true;
        }

        public void Hide()
        {
            _screenSupervisor.SetScreenColor(Side.Front, Colors.Black);
            _screenSupervisor.SetScreenColor(Side.Back, Colors.Black);
        }

        private void RefreshLuminancePoints(bool forceReload)
        {
            if (forceReload)
            {
                _luminancePointsVmFront.Clear();
                _luminancePointsVmBack.Clear();
            }

            forceReload |= _currentScreenSide == Side.Front && _luminancePointsVmFront.IsEmpty();
            forceReload |= _currentScreenSide == Side.Back && _luminancePointsVmBack.IsEmpty();

            if (forceReload)
            {
                var luminancePointsBlack = new LuminancePointsViewModel();
                luminancePointsBlack.Grayscale = 0;
                luminancePointsBlack.StepName = $"Step 1\n{_currentScreenSide}";
                luminancePointsBlack.Side = _currentScreenSide;

                var luminancePointsWhite = new LuminancePointsViewModel();
                luminancePointsWhite.Grayscale = 255;
                luminancePointsWhite.StepName = $"Step 2\n{_currentScreenSide}";
                luminancePointsWhite.Side = _currentScreenSide;

                int measureNumber = 1;
                foreach (var point in CreatePoints())
                {
                    luminancePointsBlack.LuminancePoints.Add(new LuminancePointViewModel
                    {
                        LeftPosition = point.X - (MeasureCircleSize / 2.0 + CircleStrokeTickness),
                        TopPosition = point.Y - (MeasureCircleSize / 2.0 + CircleStrokeTickness),
                        Point = point,
                        Name = "Measure " + measureNumber
                    });

                    luminancePointsWhite.LuminancePoints.Add(new LuminancePointViewModel
                    {
                        LeftPosition = point.X - (MeasureCircleSize / 2.0 + CircleStrokeTickness),
                        TopPosition = point.Y - (MeasureCircleSize / 2.0 + CircleStrokeTickness),
                        Point = point,
                        Name = "Measure " + measureNumber
                    });
                    measureNumber++;
                }

                if (_currentScreenSide == Side.Front)
                {
                    _luminancePointsVmFront.Clear();
                    _luminancePointsVmFront.Add(luminancePointsBlack);
                    _luminancePointsVmFront.Add(luminancePointsWhite);
                }
                else if (_currentScreenSide == Side.Back)
                {
                    _luminancePointsVmBack.Clear();
                    _luminancePointsVmBack.Add(luminancePointsBlack);
                    _luminancePointsVmBack.Add(luminancePointsWhite);
                }
            }

            LuminancePointsVmForCurrentSide.Clear();
            LuminancePointsVmForCurrentSide.AddRange(_currentScreenSide == Side.Front ? _luminancePointsVmFront : _luminancePointsVmBack);
            SelectedPoints = LuminancePointsVmForCurrentSide.First();
        }

        private List<Point> CreatePoints()
        {
            var points = new List<Point>();
            int stepVertic = _screenInfo.Height / NbPointVertic;
            int stepHoriz = _screenInfo.Width / NbPointHoriz;
            for (int nH = 1; nH <= NbPointHoriz; nH++)
            {
                for (int nV = 1; nV <= NbPointVertic; nV++)
                {
                    var point = new Point
                    {
                        Y = (int)(_screenInfo.Height / 2 + stepVertic * (nV - Math.Floor(NbPointVertic / 2.0) - 1)),
                        X = (int)(_screenInfo.Width / 2 + stepHoriz * (nH - Math.Floor(NbPointHoriz / 2.0) - 1))
                    };
                    points.Add(point);
                }
            }

            return points;
        }

        private void ComputeAndSaveScreenQualification()
        {
            try
            {
                var whitePoints = LuminancePointsVmForCurrentSide.First(p => p.Grayscale == 255).LuminancePoints;
                var blackPoints = LuminancePointsVmForCurrentSide.First(p => p.Grayscale == 0).LuminancePoints;
                if (whitePoints.IsNullOrEmpty() || blackPoints.IsNullOrEmpty())
                {
                    _dialogService.ShowMessageBox($"Cannot compute screen result without points",
                        "Screen Qualification", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                var saveFileDialog = new SaveFileDialog { Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*" };
                if (saveFileDialog.ShowDialog() != true)
                {
                    return;
                }

                ComputeScreenQualificationsValues(whitePoints, out float globalAverageWhite, out float globalErrorWhite,
                    out float centerAverageWhite, out float centerErrorWhite);
                ComputeScreenQualificationsValues(blackPoints, out float globalAverageBlack, out float globalErrorBlack,
                    out float centerAverageBlack, out float centerErrorBlack);
                var result = new ScreenQualificationResult
                {
                    GlobalAverageWhite = globalAverageWhite,
                    GlobalErrorWhite = globalErrorWhite,
                    CenterAverageWhite = centerAverageWhite,
                    CenterErrorWhite = centerErrorWhite,
                    GlobalAverageBlack = globalAverageBlack,
                    GlobalErrorBlack = globalErrorBlack,
                    CenterAverageBlack = centerAverageBlack,
                    CenterErrorBlack = centerErrorBlack,
                };
                result.Serialize(saveFileDialog.FileName);
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessageBox($"Error during screen qualification : {ex.Message}",
                    "Screen Qualification", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            LuminancePointsVmForCurrentSide.ForAll(x => x.IsModified = false);
        }

        private static void ComputeScreenQualificationsValues(List<LuminancePointViewModel> points, out float globalAverage, out float globalError, out float centerAverage, out float centerError)
        {
            var globalLuminances = points.Select(p => p.Luminance);
            globalAverage = globalLuminances.Average() ?? 0f;
            float max = globalLuminances.Max() ?? 0f;
            float min = globalLuminances.Min() ?? 0f;
            globalError = globalAverage != 0 ? (max - min) / globalAverage : 0f;

            // Assume that first point is in the first column and last point is in the last column
            double firstColumnX = points.First().Point.X;
            double lastColumnX = points.Last().Point.X;
            var centerLuminances = points.
                Where(p => p.Point.X > firstColumnX && p.Point.X < lastColumnX).
                Select(a => a.Luminance);
            centerAverage = centerLuminances.Average() ?? 0f;
            float centerMax = centerLuminances.Max() ?? 0f;
            float centerMin = centerLuminances.Min() ?? 0f;
            centerError = centerAverage != 0 ? (centerMax - centerMin) / centerAverage : 0;
        }

        private void Validate()
        {
            _selectedPoints.Validate();
            _selectedPoints.IsModified = true;
            ComputeAndSave.NotifyCanExecuteChanged();
        }

        [Serializable]
        public class ScreenQualificationResult
        {
            public float GlobalAverageWhite;
            public float GlobalErrorWhite;
            public float CenterAverageWhite;
            public float CenterErrorWhite;
            public float GlobalAverageBlack;
            public float GlobalErrorBlack;
            public float CenterAverageBlack;
            public float CenterErrorBlack;
        }
    }
}
