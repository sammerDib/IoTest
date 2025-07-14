using System;
using System.Collections.Generic;
using System.Globalization;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.Titles;
using LightningChartLib.WPF.Charting.Views.ViewXY;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.UI.Chart;
using UnitySC.Shared.Tools.Collection;

using Color = System.Drawing.Color;
using Media = System.Windows.Media;

namespace UnitySC.Shared.ResultUI.Common.ViewModel
{
    public abstract class HistogramVMBase : BaseLineChart
    {
        #region Fields

        protected readonly AxisX XAxis;

        protected readonly AxisY YAxis;

        #endregion

        #region Properties

        protected bool ShowLegend
        {
            set
            {
                if (Chart.ViewXY.LegendBoxes[0].Visible == value) return;
                Chart.ViewXY.LegendBoxes[0].Visible = value;
            }
        }

        protected string HistogramTitle
        {
            set
            {
                if (Chart.Title.Text == value) return;
                Chart.Title.Text = value;
            }
        }

        protected string YAxisTitle
        {
            set
            {
                if (YAxis.Title.Text == value) return;
                var axis = CreateAxisYTitle(value);

                axis.DistanceToAxis = (Math.Round(YAxis.Maximum, MidpointRounding.AwayFromZero).ToString(CultureInfo.InvariantCulture).Length * 10) + 10;
                axis.VerticalAlign = YAxisTitleAlignmentVertical.Top;

                YAxis.Title = axis;
            }
        }

        #endregion Properties

        #region Constructor

        protected HistogramVMBase()
        {
            XAxis = new AxisX
            {
                ScrollMode = XAxisScrollMode.None,
                Visible = true,
                CustomTicksEnabled = true,
                ValueType = AxisValueType.Number,
                Minimum = 0,
                AutoFormatLabels = false,
                LabelsAngle = 80,
                Title =
                {
                    Visible = false
                }
            };

            YAxis = new AxisY
            {
                AutoFormatLabels = false,
                Minimum = 0,
                ValueType = AxisValueType.Number,
                Visible = true
            };

            Chart = new LightningChart
            {
                ViewXY =
                {
                    AxisLayout = new AxisLayout
                    {
                        YAxisTitleAutoPlacement = false
                    },
                    BarViewOptions = new BarViewOptions
                    {
                        Stacking = BarsStacking.Stack,
                        Grouping = BarsGrouping.ByLocation,
                        Orientation = BarsOrientation.Vertical,
                        BarSpacing = 0
                    },
                    DropOldSeriesData = false,
                    XAxes = new AxisXList {XAxis},
                    YAxes = new AxisYList {YAxis}
                },
                Title =
                {
                    Shadow = new TextShadow
                    {
                        Style = TextShadowStyle.Off
                    },
                    AllowUserInteraction = false
                }
            };

            SetupChartTheme();

            //Set the legend box
            Chart.ViewXY.LegendBoxes[0].Visible = true;
            Chart.ViewXY.LegendBoxes[0].Position = LegendBoxPositionXY.SegmentRightMarginCenter;
            Chart.ViewXY.LegendBoxes[0].Layout = LegendBoxLayout.VerticalColumnSpan;
            Chart.ViewXY.LegendBoxes[0].HighlightSeriesTitleColor = Media.Colors.MediumPurple;
        }

        #endregion Constructor

        #region Methods

        #region Virtual

        public virtual void UpdateUnits(ResultValueType resultValueType) { }

        public virtual void Generate(List<WaferStatsData> statsData, Dictionary<string, DefectBin> legends)
        {
            ClearData();
        }

        #endregion

        #region Private

        private void ClearData()
        {
            if (Chart.ViewXY.BarSeries.IsNullOrEmpty()) return;
            Chart.ViewXY.BarSeries.Clear();
            XAxis.CustomTicks.Clear();
        }

        #endregion

        #region Protected

        protected void AddCustomXAxisTick(string name, int positionX)
        {
            var tick = new CustomAxisTick
            {
                AxisValue = positionX,
                LabelText = name,
                Style = CustomTickStyle.TickAndGrid,
                Color = Media.Color.FromArgb(35, 0, 0, 255),
                Visible = true
            };
            XAxis.CustomTicks.Add(tick);
        }

        protected void AddCustomXAxisTick(Dictionary<string, DefectBin> legends, string name, int positionX)
        {
            var tick = new CustomAxisTick
            {
                AxisValue = positionX,
                LabelText = legends[name].Label,
                Style = CustomTickStyle.TickAndGrid,
                Color = Media.Color.FromArgb(35, 0, 0, 255)
            };
            XAxis.CustomTicks.Add(tick);
        }

        #endregion

        #region Statics

        protected static BarSeries CreateBarSeries(string name, int color)
        {
            var systemColor = Color.FromArgb(color);
            var barSeries = new BarSeries
            {
                Fill = new Fill
                {
                    Color = Media.Color.FromRgb(systemColor.R, systemColor.G, systemColor.B),
                    GradientFill = GradientFill.Solid

                },
                Title = new SeriesTitle
                {
                    Text = name,
                    Visible = false
                },
                Shadow = new Shadow
                {
                    Visible = false
                },
                IncludeInAutoFit = true,
                BarThickness = 30,
                ShowInLegendBox = true
            };

            return barSeries;
        }

        protected static BarSeriesValue CreateBarSeriesValue(int positionX, double histogramValue)
        {
            var barSeriesValue = new BarSeriesValue
            {
                Location = positionX,
                Value = histogramValue
            };
            return barSeriesValue;
        }

        #endregion

        #endregion Methods
    }
}
