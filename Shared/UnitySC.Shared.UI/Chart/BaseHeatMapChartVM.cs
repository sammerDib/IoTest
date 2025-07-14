using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.EventMarkers;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Views;
using LightningChartLib.WPF.Charting.Views.ViewXY;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.UI.Chart
{
    public enum HeatMapPaletteType
    {
        TargetTolerance,
        MinMax,
        ZeroToMax,
        SpecValues
    }

    public abstract class BaseHeatMapChartVM : BaseLineChart
    {
        #region Fields

        protected double CurrentMinValue;

        protected double CurrentMaxValue;

        protected double Target;

        protected double Tolerance;

        protected double SpecValueMin;

        protected double SpecValueMax;

        private PointLineSeries _selectedPointSeries;

        #endregion Fields

        #region Properties

        protected IntensityGridSeries HeatMap { get; private set; }

        protected PointLineSeries PointSeries { get; private set; }

        protected LegendBoxXY LegendBox { get; private set; }

        protected int HeatMapSide { get; private set; }

        private HeatMapPaletteType _heatMapPalette = HeatMapPaletteType.MinMax;

        public HeatMapPaletteType HeatMapPalette
        {
            get => _heatMapPalette;
            protected set => SetProperty(ref _heatMapPalette, value);
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        private bool _hasTolerance;

        public bool HasTolerance
        {
            get { return _hasTolerance; }
            protected set { SetProperty(ref _hasTolerance, value); }
        }

        private bool _hasTarget;

        public bool HasTarget
        {
            get { return _hasTarget; }
            protected set { SetProperty(ref _hasTarget, value); }
        }

        #endregion Properties

        protected BaseHeatMapChartVM(int heatmapside)
        {
            HeatMapSide = heatmapside;

            #region Create Chart

            Chart = new LightningChart();

            // Avoids LightningChart crash on NST7. TODO: investigate more the origin.
            //Chart.ChartRenderOptions.DeviceType = RendererDeviceType.SoftwareOnlyD11;

            UpdateChart(() =>
            {
                SetupChartTheme();

                Chart.Title.Visible = false;

                var view = Chart.ViewXY;

                view.ZoomPanOptions.AspectRatioOptions = new AspectRatioOptions
                {
                    AspectRatio = ViewAspectRatio.Manual,
                    ManualAspectRatioWH = 1.0
                };

                var xAxis = view.XAxes[0];
                var yAxis = view.YAxes[0];

                xAxis.Title.Visible = false;
                yAxis.Title.Visible = false;

                view.AxisLayout.AutoAdjustMargins = false;
                var margins = new Thickness(45, 5, 82, 40);
                //var margins = new Thickness(view.Margins.Left, view.Margins.Top, view.Margins.Right + 80, view.Margins.Bottom);
                view.Margins = margins;

                LegendBox = view.LegendBoxes[0];

                LegendBox.Position = LegendBoxPositionXY.RightCenter;
                LegendBox.Layout = LegendBoxLayout.Vertical;
                LegendBox.AllowDragging = false;
                LegendBox.AllowResize = false;
                LegendBox.BorderWidth = 0;
                LegendBox.Fill.Color = Colors.Transparent;
                LegendBox.Fill.Style = RectFillStyle.ColorOnly;
                LegendBox.Fill.GradientFill = GradientFill.Solid;
                LegendBox.ScrollBarVisibility = LegendBoxBase.LegendBoxScrollBarVisibility.None;
                LegendBox.Shadow.Visible = false;
                LegendBox.Highlight = Highlight.None;

                LegendBox.AutoSize = false;
                LegendBox.Width = 80;

                LegendBox.IntensityScales.ScaleSizeDim1 = 200;
                LegendBox.IntensityScales.ScaleSizeDim2 = 20;
                LegendBox.ShowCheckboxes = false;

                view.GraphBackground.Color = Color.FromArgb(155, 255, 255, 255);
                view.GraphBackground.GradientFill = GradientFill.Solid;

                HeatMap = new IntensityGridSeries(view, xAxis, yAxis)
                {
                    WireframeType = SurfaceWireframeType.None,
                    ContourLineType = ContourLineTypeXY.None,
                    Data = new IntensityPoint[HeatMapSide, HeatMapSide],
                    AllowUserInteraction = false,
                    LegendBoxUnits = "µm",
                    Title = { Text = "Depth" }
                };
                view.IntensityGridSeries.Add(HeatMap);

                PointSeries = new PointLineSeries(view, xAxis, yAxis) { ShowInLegendBox = false };
                view.PointLineSeries.Add(PointSeries);
                _selectedPointSeries = new PointLineSeries(view, xAxis, yAxis) { ShowInLegendBox = false };
                view.PointLineSeries.Add(_selectedPointSeries);

                CustomizeChart(view, xAxis, yAxis);

                view.ZoomToFit();
            });

            #endregion Create Chart
        }

        #region Commands

        private ICommand _updatePaletteCommand;

        public ICommand UpdatePaletteCommand => _updatePaletteCommand ?? (_updatePaletteCommand = new AutoRelayCommand<HeatMapPaletteType>(UpdatePaletteCommandExecute, UpdatePaletteCommandCanExecute));

        private bool UpdatePaletteCommandCanExecute(HeatMapPaletteType arg)
        {
            return HeatMapPalette != arg;
        }

        private void UpdatePaletteCommandExecute(HeatMapPaletteType arg)
        {
            HeatMapPalette = arg;
            UpdatePalette();
        }

        #endregion Commands

        protected virtual void CustomizeChart(ViewXY view, AxisX xAxis, AxisY yAxis)
        {
        }

        protected virtual void UpdatePalette()
        {
            var palette = new ValueRangePalette(HeatMap)
            {
                Type = PaletteType.Gradient
            };

            palette.Steps.Clear();

            var minColor = Colors.DodgerBlue;
            var targetColor = Colors.DimGray;
            var maxColor = Colors.Crimson;
            var alternativeMinColor = Color.FromRgb(15, 72, 127);
            var alternativeMaxColor = Color.FromRgb(110, 10, 30);

            switch (HeatMapPalette)
            {
                case HeatMapPaletteType.TargetTolerance:
                    palette.MinValue = Target - Tolerance;
                    palette.Steps.Add(new PaletteStep(palette, minColor, Target - Tolerance));
                    palette.Steps.Add(new PaletteStep(palette, targetColor, Target));
                    palette.Steps.Add(new PaletteStep(palette, maxColor, Target + Tolerance));
                    break;

                case HeatMapPaletteType.MinMax:
                    if (CurrentMaxValue < Target)
                    {
                        palette.MinValue = CurrentMinValue;
                        palette.Steps.Add(new PaletteStep(palette, targetColor, Target));
                        palette.Steps.Add(new PaletteStep(palette, minColor, CurrentMaxValue));
                        palette.Steps.Add(new PaletteStep(palette, alternativeMinColor, CurrentMinValue));
                    }
                    else if (CurrentMinValue > Target)
                    {
                        palette.MinValue = Target;
                        palette.Steps.Add(new PaletteStep(palette, alternativeMaxColor, CurrentMaxValue));
                        palette.Steps.Add(new PaletteStep(palette, maxColor, CurrentMinValue));
                        palette.Steps.Add(new PaletteStep(palette, targetColor, Target));
                    }
                    else
                    {
                        palette.MinValue = CurrentMinValue;
                        palette.Steps.Add(new PaletteStep(palette, minColor, CurrentMinValue));
                        palette.Steps.Add(new PaletteStep(palette, targetColor, Target));
                        palette.Steps.Add(new PaletteStep(palette, maxColor, CurrentMaxValue));
                    }
                    break;

                case HeatMapPaletteType.ZeroToMax:
                    palette.MinValue = 0;
                    palette.Steps.Add(new PaletteStep(palette, Color.FromRgb(131, 131, 131), 0));
                    palette.Steps.Add(new PaletteStep(palette, Color.FromRgb(0, 225, 0), 0.25 * CurrentMaxValue));
                    palette.Steps.Add(new PaletteStep(palette, Color.FromRgb(225, 225, 0), 0.50 * CurrentMaxValue));
                    palette.Steps.Add(new PaletteStep(palette, Color.FromRgb(225, 131, 0), 0.75 * CurrentMaxValue));
                    palette.Steps.Add(new PaletteStep(palette, Color.FromRgb(225, 0, 0), CurrentMaxValue));
                    break;

                case HeatMapPaletteType.SpecValues:
                    palette.MinValue = SpecValueMin;
                    if (double.IsNaN(Target)) // pas de level intermediate
                    {
                        palette.Steps.Add(new PaletteStep(palette, minColor, SpecValueMin));
                        palette.Steps.Add(new PaletteStep(palette, maxColor, SpecValueMax));
                    }
                    else
                    {
                        if (SpecValueMax < Target)
                        {
                            palette.Steps.Add(new PaletteStep(palette, targetColor, Target));
                            palette.Steps.Add(new PaletteStep(palette, minColor, SpecValueMax));
                            palette.Steps.Add(new PaletteStep(palette, alternativeMinColor, SpecValueMin));
                        }
                        else if (SpecValueMin > Target)
                        {
                            palette.MinValue = Target;
                            palette.Steps.Add(new PaletteStep(palette, maxColor, SpecValueMax));
                            palette.Steps.Add(new PaletteStep(palette, alternativeMaxColor, SpecValueMin));
                            palette.Steps.Add(new PaletteStep(palette, targetColor, Target));
                        }
                        else
                        {
                            palette.MinValue = SpecValueMin;
                            palette.Steps.Add(new PaletteStep(palette, minColor, SpecValueMin));
                            palette.Steps.Add(new PaletteStep(palette, targetColor, Target));
                            palette.Steps.Add(new PaletteStep(palette, maxColor, SpecValueMax));
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateChart(() =>
            {
                HeatMap.ValueRangePalette = palette;
            });
        }

        public void DrawSelectedPoint(double? xPosition, double? yPosition)
        {
            SetSelectedPoint(xPosition, yPosition);
        }

        protected void SetSelectedPoint(double? xPosition, double? yPosition)
        {
            UpdateChart(() =>
            {
                _selectedPointSeries.SeriesEventMarkers.Clear();

                if (xPosition.HasValue && yPosition.HasValue)
                {
                    var aimMarker = new SeriesEventMarker(_selectedPointSeries)
                    {
                        HorizontalPosition = SeriesEventMarkerHorizontalPosition.AtXValue,
                        VerticalPosition = SeriesEventMarkerVerticalPosition.AtYValue,
                        XValue = xPosition.Value,
                        YValue = yPosition.Value,
                        Visible = true,
                        Symbol = GetSelectionSymbol(),
                        Label = { Visible = false },
                        AllowDragging = false
                    };

                    _selectedPointSeries.SeriesEventMarkers.Add(aimMarker);
                }

                _selectedPointSeries.InvalidateData();
            });
        }

        protected SeriesEventMarker DrawPointMarker(double x, double y, SeriesMarkerPointShapeStyle symbol)
        {
            var marker = new SeriesEventMarker(PointSeries)
            {
                HorizontalPosition = SeriesEventMarkerHorizontalPosition.AtXValue,
                VerticalPosition = SeriesEventMarkerVerticalPosition.AtYValue,
                XValue = x,
                YValue = y,
                Visible = true,
                Symbol = symbol,
                Label = { Visible = false },
                AllowDragging = false
            };

            marker.MouseClick += OnMarkerClicked;
            PointSeries.SeriesEventMarkers.Add(marker);
            return marker;
        }

        protected virtual void ClearPointMarkers()
        {
            foreach (var marker in PointSeries.SeriesEventMarkers)
            {
                marker.MouseClick -= OnMarkerClicked;
            }

            PointSeries.Clear();
            PointSeries.SeriesEventMarkers.Clear();
        }

        protected abstract void OnMarkerClicked(object sender, MouseEventArgs e);

        public void SetPaletteType(HeatMapPaletteType paletteType)
        {
            HeatMapPalette = paletteType;
        }

        public void SetSpecMinAndMax(double specMin, double specMax)
        {
            SpecValueMax = specMax;
            SpecValueMin = specMin;
        }

        public void SetTargetTolerance(double? target, double? tolerance)
        {
            HasTarget = target.HasValue;
            HasTolerance = tolerance.HasValue;

            Target = target ?? 0;
            Tolerance = tolerance ?? 0;
        }

        public void SetTitle(string title)
        {
            HeatMap.Title.Text = title;
        }

        public void SetUnit(string unit)
        {
            HeatMap.LegendBoxUnits = unit;
        }

        public void SetMinMax(double min, double max)
        {
            CurrentMinValue = min;
            CurrentMaxValue = max;
            UpdatePalette();
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearPointMarkers();
            }
            // Call base class implementation.
            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
