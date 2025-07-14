using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Views.ViewXY;

using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Charts
{
    public class BarChartData
    {
        public int Index { get; set; }

        public string Title { get; set; }

        public double? Min { get; set; }

        public double? Max { get; set; }

        public double Value { get; set; }

        public Color Color { get; set; }
    }

    public class BarsAndLineCharts : BaseLineChart
    {
        #region Fields

        private readonly AxisX _xAxis;

        #endregion Fields

        #region Constructor

        public BarsAndLineCharts(string xAxisTitle)
        {
            #region Axes

            _xAxis = new AxisX
            {
                AutoFormatLabels = false,
                CustomTicksEnabled = true,
                ValueType = AxisValueType.Number,
                Visible = true,
                Title = CreateAxisXTitle(xAxisTitle)
            };
            
            #endregion Axes

            #region Chart

            Chart = new LightningChart
            {
                Options =
                {
                    AllowUserInteraction = true
                },
                ViewXY =
                {
                    DropOldSeriesData = false,
                    AxisLayout =
                    {
                        AxisGridStrips = XYAxisGridStrips.X,
                        YAxesLayout = YAxesLayout.Stacked,
                        AutoAdjustMargins = false,
                    },
                    ZoomPanOptions = new ZoomPanOptions
                    {
                        ViewFitYMarginPixels = 10,
                        PanDirection = PanDirection.Horizontal,
                        WheelZooming = WheelZooming.Horizontal,
                        RectangleZoomMode = RectangleZoomMode.Horizontal,
                    },
                    XAxes = new AxisXList {_xAxis}
                },
                Title =
                {
                    Visible = false
                }
            };

            Chart.ViewXY.LegendBoxes[0].Visible = false;

            SetupChartTheme();

            #endregion Chart
        }
        
        #endregion Contructors

        #region Properties
        
        public double BarWidth { get; set; } = 0.5;

        #endregion Properties

        #region Public Methods
        
        public void SetData(IEnumerable<BarChartData> dataCollection)
        {
            UpdateChart(() =>
            {
                Clear();

                foreach (var data in dataCollection)
                {
                    SetSerieData(data);
                }
            });

            Chart.ViewXY.ZoomToFit();
        }

        #endregion Public Methods

        #region Private Methods

        private void Clear()
        {
            _xAxis.CustomTicks.Clear();
            Chart.ViewXY.YAxes.Clear();
            Chart.ViewXY.PolygonSeries.Clear();
            Chart.ViewXY.PointLineSeries.Clear();
        }

        private void AddSerie(string title, double fontsize = 14.0, string unit = null, double unitfontsize = 10.0)
        {
            // Add the series only if it doesn't exist. 
            if (Chart.ViewXY.PointLineSeries.Any(series => series.Title.Text.Equals(title))) return;

            UpdateChart(() =>
            {
                var yAxis = CreateAxisY(Chart.ViewXY, title, fontsize, unit, unitfontsize);
                Chart.ViewXY.YAxes.Add(yAxis);

                var lineSeries = new PointLineSeries(Chart.ViewXY, _xAxis, yAxis)
                {
                    ShowInLegendBox = false,
                    PointsVisible = true,
                    PointStyle = { Width = 6, Height = 6, Color1 = Colors.Yellow },
                    LineStyle = { Color = Colors.MediumPurple },
                    Highlight = Highlight.None,
                    AssignYAxisIndex = Chart.ViewXY.YAxes.IndexOf(yAxis),
                    Title = { Text = title }
                };
                Chart.ViewXY.PointLineSeries.Add(lineSeries);
            });
        }

        private void SetSerieData(BarChartData data)
        {
            if (data == null) return;

            AddSerie(data.Title);

            if (_xAxis.CustomTicks.All(tick => (int)tick.AxisValue != data.Index))
            {
                var customTick = new CustomAxisTick(_xAxis, data.Index, data.Index.ToString(CultureInfo.InvariantCulture))
                {
                    Style = CustomTickStyle.TickAndGrid,
                    Color = Color.FromArgb(35, 0, 0, 255),
                };

                _xAxis.CustomTicks.Add(customTick);
            }

            int seriesIndex = Chart.ViewXY.PointLineSeries.FindIndex(series => series.Title.Text.Equals(data.Title));
            var point = new SeriesPoint(data.Index, data.Value);
            Chart.ViewXY.PointLineSeries[seriesIndex].AddPoints(new[] { point }, true);
            DrawPolygon(seriesIndex, data.Index, data.Min, data.Max, data.Color);
        }

        private void DrawPolygon(int seriesIndex, int xIndex, double? minimum, double? maximum, Color color)
        {
            if (!minimum.HasValue || !maximum.HasValue) return;

            var yAxis = Chart.ViewXY.YAxes.ElementAt(seriesIndex);

            var polygon = new PolygonSeries(Chart.ViewXY, _xAxis, yAxis)
            {
                AllowUserInteraction = false,
                Fill =
                {
                    Color = color,
                    GradientFill = GradientFill.Solid
                },
                Behind = true,
                Border =
                {
                    Color = Colors.DarkSlateGray,
                    Width = 1
                },
                Points = new[]
                {
                    new PointDouble2D(xIndex - BarWidth / 2, maximum.Value),
                    new PointDouble2D(xIndex + BarWidth / 2, maximum.Value),
                    new PointDouble2D(xIndex + BarWidth / 2, minimum.Value),
                    new PointDouble2D(xIndex - BarWidth / 2, minimum.Value)
                },
                ShowInLegendBox = false
            };

            Chart.ViewXY.PolygonSeries.Add(polygon);
        }

        #endregion Private Methods
    }
}
