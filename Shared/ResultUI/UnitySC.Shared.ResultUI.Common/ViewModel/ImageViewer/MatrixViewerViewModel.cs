using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.ResultUI.Common.Helpers;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer
{
    public class MatrixViewerViewModel : ObservableObject, IDisposable
    {
        public enum MatrixViewerMode
        {
            TwoDimension,
            ThreeDimension
        }

        #region Properties

        public MatrixDefinition Matrix { get; }

        private MatrixViewerMode _mode;

        public MatrixViewerMode Mode
        {
            get { return _mode; }
            set
            {
                if (SetProperty(ref _mode, value) && Mode == MatrixViewerMode.ThreeDimension)
                {
                    if (_areaInterpolationRequered)
                    {
                        ApplyInterpolationArea();
                    }
                }
            }
        }

        private float _minValue;

        public float MinValue
        {
            get => EnableReferenceValue ? _minValue - ReferenceValue : _minValue;
            set
            {
                if (EnableReferenceValue)
                {
                    value += ReferenceValue;
                }
                SetProperty(ref _minValue, value);
                OnMinMaxChanged();
            }
        }

        private float _maxValue;

        public float MaxValue
        {
            get => EnableReferenceValue ? _maxValue - ReferenceValue : _maxValue;
            set
            {
                if (EnableReferenceValue)
                {
                    value += ReferenceValue;
                }
                SetProperty(ref _maxValue, value);
                OnMinMaxChanged();
            }
        }

        private ColorMap _colorMap = ColorMapHelper.ColorMaps.First();

        public ColorMap ColorMap
        {
            get { return _colorMap; }
            set
            {
                SetProperty(ref _colorMap, value);
                OnColorMapChanged();
            }
        }

        private int _rangeCount = 5;

        public int RangeCount
        {
            get { return _rangeCount; }
            set
            {
                SetProperty(ref _rangeCount, value);
                GenerateRanges();
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private List<MatrixRange> _ranges;

        public List<MatrixRange> Ranges
        {
            get { return _ranges; }
            private set { SetProperty(ref _ranges, value); }
        }

        private FloatStatsContainer _statsContainer;

        public FloatStatsContainer StatsContainer
        {
            get { return _statsContainer; }
            private set { SetProperty(ref _statsContainer, value); }
        }

        public FloatStatsContainer EffectiveStatsContainer
        {
            get
            {
                if (StatsContainer == null || !EnableReferenceValue) return StatsContainer;

                float mean = StatsContainer.Mean - ReferenceValue;
                float min = StatsContainer.Min - ReferenceValue;
                float max = StatsContainer.Max - ReferenceValue;
                float stdDev = StatsContainer.StdDev;
                float median = StatsContainer.Median - ReferenceValue;
                return new FloatStatsContainer(mean, min, max, stdDev, median);
            }
        }

        public TwoDimensionsMatrixViewerVM TwoDimensionsMatrixViewer { get; }

        public ThreeDimensionsMatrixViewerVM ThreeDimensionsMatrixViewer { get; }

        public MatrixViewFinderVM MatrixViewFinder { get; }

        private bool _valuePickerEnable;

        public bool ValuePickerEnable
        {
            get { return _valuePickerEnable; }
            set { SetProperty(ref _valuePickerEnable, value); }
        }

        private float _referenceValue;

        public float ReferenceValue
        {
            get { return _referenceValue; }
            set
            {
                SetProperty(ref _referenceValue, value);
                OnReferenceValueChanged();
            }
        }

        private bool _enableReferenceValue;

        public bool EnableReferenceValue
        {
            get { return _enableReferenceValue; }
            set
            {
                SetProperty(ref _enableReferenceValue, value);
                OnReferenceValueChanged();
            }
        }

        public float EffectiveReferenceValue => EnableReferenceValue ? ReferenceValue : 0;

        private bool _excludePureZeroFromStats;

        public bool ExcludePureZeroFromStats
        {
            get { return _excludePureZeroFromStats; }
            set { SetProperty(ref _excludePureZeroFromStats, value); }
        }

        #endregion

        #region Commands

        private ICommand _dynamicCommand;

        public ICommand DynamicCommand => _dynamicCommand ?? (_dynamicCommand = new AutoRelayCommand(DynamicCommandExecute));

        private void DynamicCommandExecute()
        {
            SetProperty(ref _minValue, StatsContainer.Min, nameof(MinValue));
            SetProperty(ref _maxValue, StatsContainer.Max, nameof(MaxValue));
            OnMinMaxChanged();
        }

        private ICommand _medianCommand;

        public ICommand MedianCommand => _medianCommand ?? (_medianCommand = new AutoRelayCommand(MedianCommandExecute));
        private void MedianCommandExecute()
        {
            SetProperty(ref _minValue, StatsContainer.Median - StatsContainer.StdDev, nameof(MinValue));
            SetProperty(ref _maxValue, StatsContainer.Median + StatsContainer.StdDev, nameof(MaxValue));
            OnMinMaxChanged();
        }

        private ICommand _meanCommand;

        public ICommand MeanCommand => _meanCommand ?? (_meanCommand = new AutoRelayCommand(MeanCommandExecute));

        private void MeanCommandExecute()
        {
            SetProperty(ref _minValue, StatsContainer.Mean - StatsContainer.StdDev, nameof(MinValue));
            SetProperty(ref _maxValue, StatsContainer.Mean + StatsContainer.StdDev, nameof(MaxValue));
            OnMinMaxChanged();
        }

        #endregion

        public MatrixViewerViewModel(MatrixDefinition originalMatrix, bool excludeZerofromStat = false)
        {
            Matrix = originalMatrix;

            SetProperty(ref _excludePureZeroFromStats, excludeZerofromStat, nameof(ExcludePureZeroFromStats));

            // Stats
            if (originalMatrix.Values.Length == 0)
            {
                StatsContainer = new FloatStatsContainer(0.0f, -1.0f, 1.0f, 0.05f, 0.0f);
            }
            else
            {
                StatsContainer = FloatStatsContainer.GenerateFromFloatsKeepSortedData(Matrix.Values.ToList(), out List<float> sortedMatrix, _excludePureZeroFromStats);
                Matrix.SortedValues = sortedMatrix.ToArray();
            }
            SetProperty(ref _minValue, StatsContainer.Min, nameof(MinValue));
            SetProperty(ref _maxValue, StatsContainer.Max, nameof(MaxValue));

            // 2D Viewer
            TwoDimensionsMatrixViewer = new TwoDimensionsMatrixViewerVM(Matrix, OnTwoDimensionsMatrixViewerMouseDownFunc);
            TwoDimensionsMatrixViewer.Initialize(_colorMap, _minValue, _maxValue);
            TwoDimensionsMatrixViewer.TwoDimensionsImageViewer.ViewRectChanged += TwoDimensionsImageViewer_ViewRectChanged;

            // 3D Viewer
            ThreeDimensionsMatrixViewer = new ThreeDimensionsMatrixViewerVM(Matrix.Unit, Matrix.PixelXUnit, Matrix.PixelYUnit);
            ThreeDimensionsMatrixViewer.Initialize(_colorMap, _minValue, _maxValue);

            // Thumbnail ViewFinder
            MatrixViewFinder = new MatrixViewFinderVM(originalMatrix, _colorMap, _minValue, _maxValue);
            MatrixViewFinder.MatrixRectChanged += OnRectChanged;
            MatrixViewFinder.MatrixRectChanging += OnRectChanging;

            ColorMap = ColorMapHelper.GetThumbnailColorMap();

            GenerateRanges();

            _areaInterpolationRequered = true;
        }

        #region Private Methods

        private void OnColorMapChanged()
        {
            TwoDimensionsMatrixViewer.UpdateColorMap(_colorMap);
            ThreeDimensionsMatrixViewer.UpdateColorMap(_colorMap);
            MatrixViewFinder.UpdateThumbnail(_colorMap, _minValue, _maxValue);
        }

        private void OnMinMaxChanged()
        {
            TwoDimensionsMatrixViewer.UpdateMinMax(_minValue, _maxValue);
            ThreeDimensionsMatrixViewer.UpdateMinMax(_minValue, _maxValue);
            MatrixViewFinder.UpdateThumbnail(_colorMap, _minValue, _maxValue);
        }

        private void OnReferenceValueChanged()
        {
            if (EnableReferenceValue)
            {
                TwoDimensionsMatrixViewer.SetReferenceValue(ReferenceValue);
                ThreeDimensionsMatrixViewer.SetReferenceValue(ReferenceValue);
            }
            else
            {
                TwoDimensionsMatrixViewer.SetReferenceValue(null);
                ThreeDimensionsMatrixViewer.SetReferenceValue(null);
            }

            OnPropertyChanged(nameof(EffectiveReferenceValue));
            OnPropertyChanged(nameof(MinValue));
            OnPropertyChanged(nameof(MaxValue));
            OnPropertyChanged(nameof(EffectiveStatsContainer));
        }

        private bool OnTwoDimensionsMatrixViewerMouseDownFunc(float value)
        {
            if (!ValuePickerEnable) return false;

            SetProperty(ref _enableReferenceValue, true, nameof(EnableReferenceValue));
            SetProperty(ref _referenceValue, value, nameof(ReferenceValue));
            OnReferenceValueChanged();
            ValuePickerEnable = false;
            return true;
        }

        private void GenerateRanges()
        {
            var ranges = new List<MatrixRange>();
            float range = (StatsContainer.Max - StatsContainer.Min) / (RangeCount + 1);
            float currentRange = StatsContainer.Min + range;

            for (int i = 0; i < RangeCount; i++)
            {
                if (i == 0)
                {
                    ranges.Add(new MatrixRange
                    {
                        Min = float.NegativeInfinity,
                        Max = currentRange - 0.0001f,
                        TotalCount = Matrix.Values.Length
                    });
                }
                else if (i == RangeCount - 1)
                {
                    ranges.Add(new MatrixRange
                    {
                        Min = currentRange,
                        Max = float.PositiveInfinity,
                        TotalCount = Matrix.Values.Length
                    });
                }
                else
                {
                    ranges.Add(new MatrixRange
                    {
                        Min = currentRange,
                        Max = currentRange + range - 0.0001f,
                        TotalCount = Matrix.Values.Length
                    });

                    currentRange += range;
                }
            }

            foreach (float value in Matrix.Values)
            {
                var associatedRange = ranges.LastOrDefault(matrixRange => value >= matrixRange.Min);
                if (associatedRange == null) continue;
                associatedRange.Count++;
            }

            Ranges = ranges;
        }

        private bool _areaInterpolationRequered;
        private bool _preventImageZoomChanged;
        private bool _preventViewFinderRectChanged;

        private void OnRectChanged(object sender, Rect e)
        {
            if (Mode == MatrixViewerMode.ThreeDimension)
            {
                ApplyInterpolationArea();
            }
            else
            {
                _areaInterpolationRequered = true;
            }
        }

        private void OnRectChanging(object sender, Rect e)
        {
            // Prevent loop SetViewRect
            if (_preventImageZoomChanged) return;

            _preventViewFinderRectChanged = true;
            TwoDimensionsMatrixViewer.TwoDimensionsImageViewer.SetViewRect(e);
            _preventViewFinderRectChanged = false;
        }

        /// <summary>
        /// When the area changes, synchronizes the zoom of the 2D view
        /// </summary>
        private void TwoDimensionsImageViewer_ViewRectChanged(object sender, Rect e)
        {
            // Prevent loop SetViewRect
            if (_preventViewFinderRectChanged) return;

            _preventImageZoomChanged = true;
            MatrixViewFinder.SetViewRect(e);
            _preventImageZoomChanged = false;
        }

        private CancellationTokenSource _currenTokenSource;

        /// <summary>
        /// Matrix corresponding to an area of the original matrix according to the Start and End points
        /// </summary>
        private MatrixDefinition _3dMatrix;

        private void ApplyInterpolationArea()
        {
            // Prevents recalculation of the matrix when the user is manipulating its size.
            IsBusy = true;

            _areaInterpolationRequered = false;

            _currenTokenSource?.Cancel();
            _currenTokenSource?.Dispose();

            _currenTokenSource = new CancellationTokenSource();
            var cancellationToken = _currenTokenSource.Token;

            Task.Factory.StartNew(() =>
            {
                // Prevent negative area
                int startX = Math.Min(MatrixViewFinder.StartX, MatrixViewFinder.EndX);
                int startY = Math.Min(MatrixViewFinder.StartY, MatrixViewFinder.EndY);
                int endX = Math.Max(MatrixViewFinder.StartX, MatrixViewFinder.EndX);
                int endY = Math.Max(MatrixViewFinder.StartY, MatrixViewFinder.EndY);

                _3dMatrix = Matrix.ReduceArea(startX, startY, endX, endY, cancellationToken);
                if (cancellationToken.IsCancellationRequested) return;

                _3dMatrix = MatrixDefinitionHelper.Interpolate(_3dMatrix, cancellationToken);
                if (cancellationToken.IsCancellationRequested) return;

                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    if (cancellationToken.IsCancellationRequested) return;
                    On3DMatrixChanged();
                    IsBusy = false;
                });
            }, cancellationToken);
        }

        private void On3DMatrixChanged()
        {
            ThreeDimensionsMatrixViewer.SetData(_3dMatrix);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _currenTokenSource?.Cancel();
            _currenTokenSource?.Dispose();

            TwoDimensionsMatrixViewer.TwoDimensionsImageViewer.ViewRectChanged -= TwoDimensionsImageViewer_ViewRectChanged;

            TwoDimensionsMatrixViewer.Dispose();
            ThreeDimensionsMatrixViewer.Dispose();

            MatrixViewFinder.MatrixRectChanged -= OnRectChanged;
            MatrixViewFinder.MatrixRectChanging -= OnRectChanging;
            MatrixViewFinder.Dispose();
        }

        #endregion
    }
}
