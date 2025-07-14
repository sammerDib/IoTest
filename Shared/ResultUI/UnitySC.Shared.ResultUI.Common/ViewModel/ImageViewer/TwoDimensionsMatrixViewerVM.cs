using System;
using System.CodeDom;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.ResultUI.Common.ViewModel.Charts.Histograms;
using UnitySC.Shared.UI.Helper;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer
{
    public class TwoDimensionsMatrixViewerVM : BaseMatrixViewerVM
    {
        #region Fields
        
        private Bitmap _bitmap;

        private float? _referenceValue;

        private readonly Func<float, bool> _onMouseDownFunc;

        #endregion

        #region Properties

        public ImageViewerViewModel TwoDimensionsImageViewer { get; }

        public ColorMapHistogramChartVM HistogramChart { get; }

        public MatrixProfileVM Profile { get; }

        private bool _showProfileChartFlag;

        public bool ShowProfileChartFlag
        {
            get { return _showProfileChartFlag; }
            set { SetProperty(ref _showProfileChartFlag, value); }
        }

        public string Unit => Matrix.Unit;
        
        #endregion

        public TwoDimensionsMatrixViewerVM(MatrixDefinition matrix, Func<float, bool> onMouseDownFunc) : base(matrix)
        {
            _onMouseDownFunc = onMouseDownFunc;
            Profile = new MatrixProfileVM(Matrix.Values, Matrix.Width, Matrix.Height, Matrix.Unit);
            Profile.LineMarkerPositionChanged += Profile_LineMarkerPositionChanged;
            Profile.HorizontalMarkerPositionChanged += Profile_HorizontalMarkerPositionChanged;
            Profile.VerticalMarkerPositionChanged += Profile_VerticalMarkerPositionChanged;

            TwoDimensionsImageViewer = new ImageViewerViewModel(null, Export, Matrix.Extension, Matrix.FileName, true)
            {
                OnMousePosChangedFunc = OnImageViewerMousePosChanged,
                OnMouseDownFunc = OnMouseDownFunc
            };
            TwoDimensionsImageViewer.ProfileDrawn += OnProfileDrawn;
            TwoDimensionsImageViewer.CrossProfileDrawn += OnCrossProfileDrawn;
            TwoDimensionsImageViewer.ProfileCleared += OnProfileCleared;

            HistogramChart = new ColorMapHistogramChartVM($"Value ({Matrix.Unit})")
            {
                MinimumLimit = Min,
                MaximumLimit = Max,
                StepNumber = 100
            };
        }

        private bool OnMouseDownFunc(int x, int y)
        {
            if (_onMouseDownFunc == null) return false;
            float value = GetMeasureFromCoordinate(x, y, Matrix.Values, Matrix.Width);
            return _onMouseDownFunc.Invoke(value);
        }

        #region Public Methods

        public void SetReferenceValue(float? referenceValue)
        {
            _referenceValue = referenceValue;
            Profile.SetReferenceValue(referenceValue);
            
            HistogramChart.SetReferenceValue(referenceValue);

            if (_referenceValue.HasValue)
            {
                HistogramChart.UpdateXAxisTitle($"Relative value ({Matrix.Unit})");
            }
            else
            {
                HistogramChart.UpdateXAxisTitle($"Value ({Matrix.Unit})");
            }
        }
        
        #region Overrides of BaseMatrixViewerVM

        public override void UpdateColorMap(ColorMap colorMap)
        {
            base.UpdateColorMap(colorMap);
            
            GenerateBitmap();
            HistogramChart.UpdateColors(ColorMap);
        }

        public override void UpdateMinMax(float min, float max)
        {
            base.UpdateMinMax(min, max);

            GenerateBitmap();
        }

        public override void Initialize(ColorMap colorMap, float min, float max)
        {
            base.Initialize(colorMap, min, max);

            GenerateBitmap();

            var sortedValues = Matrix.SortedValues;
            if (sortedValues == null)
            {
                if (Matrix.Values.Length <= 1024 * 512)
                    sortedValues = Matrix.Values.ToList().Where(f => !float.IsNaN(f)).OrderBy(d => d).ToArray();
                else
                    sortedValues = Matrix.Values.ToList().AsParallel().Where(f => !float.IsNaN(f)).OrderBy(d => d).ToArray();
            }
            HistogramChart.ResetChart(sortedValues, ColorMap, min, max);
        }

        #endregion

        #endregion

        #region Event Handlers

        private void Profile_LineMarkerPositionChanged(object sender, System.Windows.Point? e)
        {
            TwoDimensionsImageViewer.UpdateLineProfileMarkerPosition(e);
        }

        private void Profile_HorizontalMarkerPositionChanged(object sender, double? e)
        {
            TwoDimensionsImageViewer.UpdateCrossProfileHorizontalMarkerPosition(e);
        }

        private void Profile_VerticalMarkerPositionChanged(object sender, double? e)
        {
            TwoDimensionsImageViewer.UpdateCrossProfileVerticalMarkerPosition(e);
        }

        private void OnCrossProfileDrawn(object sender, CrossProfileDrawnEventArgs e)
        {
            Profile.UpdateCrossProfile(e.Horizontal, e.Vertical);
            ShowProfileChartFlag = !ShowProfileChartFlag;
        }

        private void OnProfileDrawn(object sender, ProfileDrawnEventArgs e)
        {
            Profile.UpdateProfile(e.StartX, e.StartY, e.EndX, e.EndY);
            ShowProfileChartFlag = !ShowProfileChartFlag;
        }

        private void OnProfileCleared(object sender, EventArgs e)
        {
            Profile.Clear();
        }
        
        #endregion

        #region Private Methods

        private void Export(string filePath)
        {
            _bitmap?.Save(filePath);
        }

        private void OnImageViewerMousePosChanged(int? x, int? y)
        {
            if (x.HasValue && y.HasValue && Matrix?.Values != null)
            {
                float value = GetMeasureFromCoordinate(x.Value, y.Value, Matrix.Values, Matrix.Width);

                TwoDimensionsImageViewer.MouseOverXInformation = $"({Matrix.GetRealXAsString(x.Value)})";
                TwoDimensionsImageViewer.MouseOverYInformation = $"({Matrix.GetRealYAsString(y.Value)})";
                TwoDimensionsImageViewer.CurrentValueInformation = _referenceValue.HasValue ? $"(Relative value: {value - _referenceValue.Value:F5} {Matrix.Unit})" : string.Empty;
                TwoDimensionsImageViewer.CurrentValue = $"{value:F5} {Matrix.Unit}";
                return;
            }

            TwoDimensionsImageViewer.MouseOverXInformation = string.Empty;
            TwoDimensionsImageViewer.MouseOverYInformation = string.Empty;
            TwoDimensionsImageViewer.CurrentValueInformation = string.Empty;
            TwoDimensionsImageViewer.CurrentValue = string.Empty;
        }

        private void GenerateBitmap()
        {
            if (Matrix.Width == 0 || Matrix.Height == 0) return;

            var colors = ColorMap.Colors;
            int colorsLength = colors.Length;

            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }

            _bitmap = new Bitmap(Matrix.Width, Matrix.Height);
            var graphics = Graphics.FromImage(_bitmap);
            graphics.Clear(Color.Transparent);

            float range = Max - Min;
            float a = colorsLength / range;
            float b = -Min * colorsLength / range;

            Color GetPixel(int x, int y)
            {
                float value = GetMeasureFromCoordinate(x, y, Matrix.Values, Matrix.Width);
                int colorIndex = ColorMapHelper.GetColorIndexFromValue(value, a, b, colorsLength);
                return colorIndex < 0 ? Color.Transparent : colors[colorIndex];
            }

            ImageHelper.ProcessBitmap(_bitmap, 0, GetPixel);

            Application.Current?.Dispatcher?.Invoke(() =>
            {
                var image = ImageHelper.ConvertToBitmapSource(_bitmap);
                TwoDimensionsImageViewer.SetImage(image);
            });
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            Profile.LineMarkerPositionChanged -= Profile_LineMarkerPositionChanged;
            Profile.HorizontalMarkerPositionChanged -= Profile_HorizontalMarkerPositionChanged;
            Profile.VerticalMarkerPositionChanged -= Profile_VerticalMarkerPositionChanged;
            Profile.Clear();

            TwoDimensionsImageViewer.ProfileDrawn -= OnProfileDrawn;
            TwoDimensionsImageViewer.CrossProfileDrawn -= OnCrossProfileDrawn;
            TwoDimensionsImageViewer.Dispose();

            _bitmap?.Dispose();
        }

        #endregion
    }
}
