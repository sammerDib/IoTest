using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Media;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.Axes;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Titles;
using LightningChartLib.WPF.Charting.Views.ViewXY;

using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.UI.Chart;

namespace UnitySC.Shared.ResultUI.Metro.ViewModel.Common.RawSignal
{
    public class RawSignalChart : BaseLineChart
    {
        #region Fields

        private readonly AxisX _xAxis;
        protected AxisX GetAxisX() { return _xAxis; }

        private readonly AxisY _yAxis;
        protected AxisY GetAxisY() { return _yAxis; }

        protected bool IsFirstGeneration = true;
        #endregion

        #region Properties

        private string YAxisTitle
        {
            set
            {
                if (_yAxis.Title.Text == value) return;
                var axis = CreateAxisYTitle(value);
                axis.VerticalAlign = YAxisTitleAlignmentVertical.Top;

                _yAxis.Title = axis;
            }
        }

        #endregion Properties

        #region Constructor

        public RawSignalChart(string yAxisTitle)
        {
            _xAxis = new AxisX
            {
                ScrollMode = XAxisScrollMode.None,
                Visible = true,
                ValueType = AxisValueType.Number,
                Minimum = 0,
                AutoFormatLabels = false,
                Title =
                {
                    Visible = false
                }
            };

            _yAxis = new AxisY
            {
                AutoFormatLabels = false,
                Minimum = 0,
                ValueType = AxisValueType.Number,
                Visible = true,
                Title =
                {
                    Visible = false,
                    DistanceToAxis = 20
                },
            };

            Chart = new LightningChart
            {
                ViewXY =
                {
                    AxisLayout = new AxisLayout
                    {
                        YAxisTitleAutoPlacement = false
                    },
                    XAxes = new AxisXList {_xAxis},
                    YAxes = new AxisYList {_yAxis},
                    PointLineSeries = new PointLineSeriesList()
                },
                Title =
                {
                    Shadow = new TextShadow
                    {
                        Style = TextShadowStyle.Off
                    },
                    AllowUserInteraction = false,
                    Text = "",
                    Visible = false
                }
            };

            SetupChartTheme();

            //Set the legend box
            Chart.ViewXY.LegendBoxes[0].Position = LegendBoxPositionXY.SegmentTopLeft;
            Chart.ViewXY.LegendBoxes[0].Layout = LegendBoxLayout.VerticalColumnSpan;
            Chart.ViewXY.LegendBoxes[0].HighlightSeriesTitleColor = Colors.MediumPurple;
            Chart.ViewXY.LegendBoxes[0].Visible = false;
            YAxisTitle = yAxisTitle;

            // DataCursor configuration
            Chart.ViewXY.DataCursor.Visible = true;
            Chart.ViewXY.DataCursor.ShowTrackingPoint = false;
            Chart.ViewXY.DataCursor.ShowResultTable = false;
            Chart.ViewXY.DataCursor.LineStyle.Color = Color.FromArgb(200, 30, 30, 30);
            Chart.ViewXY.DataCursor.LabelFont.Size = 12;
        }

        #endregion

        #region Public Methods

        public void UpdateYAxisTitle(string title)
        {
            UpdateChart(() =>
            {
                YAxisTitle = title;
            });
        }

        public void Generate<T>(ICollection<T> repetaPoints) where T : MeasureScanLine
        {
            UpdateChart(() =>
            {
                Chart.ViewXY.PointLineSeries.Clear();

                var random = new Random(12345);  // use fixed seed to have the same color order from a measure to another

                int repetaIndex = 0;
                foreach (var point in repetaPoints)
                {
                    //temporary solution, generate random color for the repeta raw signal
                    byte r = Convert.ToByte(random.Next(1, 255));
                    byte g = Convert.ToByte(random.Next(1, 255));
                    byte b = Convert.ToByte(random.Next(1, 255));

                    if (point.RawProfileScan?.RawPoints == null) continue;

                    var rawPoints = point.RawProfileScan.RawPoints;
                    var chartPoints = new List<SeriesPoint>();

                    for (int p = 0; p < rawPoints.Count - 1; p++)
                    {
                        chartPoints.Add(new SeriesPoint(rawPoints[p].X, rawPoints[p].Z));
                        chartPoints.Add(new SeriesPoint(rawPoints[p + 1].X, rawPoints[p + 1].Z));
                    }

                    var pointLineSeries = new PointLineSeries(Chart.ViewXY, _xAxis, _yAxis)
                    {
                        Title = new SeriesTitle { Text = $"{repetaIndex + 1}", Color = Color.FromRgb( 10, 10, 10) },
                        LineStyle =
                        {
                            Color = Color.FromRgb(r, g, b)
                        }
                    };

                    Chart.ViewXY.PointLineSeries.Add(pointLineSeries);
                    pointLineSeries.AddPoints(chartPoints.ToArray(), false);

                    repetaIndex++;
                }

                if (IsFirstGeneration)
                {
                    Chart.ViewXY.ZoomToFit();
                    IsFirstGeneration = false;
                }
                Chart.ViewXY.YAxes[0].Title.DistanceToAxis = (Chart.ViewXY.YAxes[0].Maximum.ToString(CultureInfo.InvariantCulture).Length * 2) + 15;
                Chart.ViewXY.LegendBoxes[0].Visible = repetaIndex > 1;
            });
        }

        public void Export(String filePathName)
        {
            string exportFilePath = filePathName;
            if (!exportFilePath.EndsWith(".csv")) exportFilePath += ".csv";

            string directoryName = Path.GetDirectoryName(exportFilePath);
            Directory.CreateDirectory(directoryName);
            var pointSignalSeries = Chart.ViewXY.PointLineSeries;
            var sbCSV = new CSVStringBuilder();

            var titles = pointSignalSeries.Select(pointLineSeries => pointLineSeries.Title.Text).ToList();
            int nbSignalSeries = titles.Count;
            for (int j = 0; j < nbSignalSeries; j++)
            {
                sbCSV.Append(titles[j],"") ;
            }
            sbCSV.AppendLine_NoDelim("");

            var maxlines = pointSignalSeries.Select(pointLineSeries => pointLineSeries.PointCount).ToList().Max();
            for (int i = 0; i < maxlines; i++)
            {
                for (int j = 0; j < nbSignalSeries; j++)
                {
                    var ptserie = pointSignalSeries[j];
                    if (i < ptserie.PointCount)
                        sbCSV.Append(ptserie.Points[i].X.ToString(), ptserie.Points[i].Y.ToString());
                    else
                        sbCSV.Append("","");
                }
                sbCSV.AppendLine_NoDelim("");
            }

            using (var sw = new StreamWriter(exportFilePath))
            {        
               sw.Write(sbCSV.ToString()); 
               sw.Close();
            }
            sbCSV.Clear();
        }

        #endregion
    }
}
