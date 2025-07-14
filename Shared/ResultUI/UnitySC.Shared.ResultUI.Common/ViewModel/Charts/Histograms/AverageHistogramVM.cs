using System;
using System.Collections.Generic;
using System.Linq;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.SeriesXY;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;

using Media = System.Windows.Media;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Charts.Histograms
{
    public class AverageHistogramVM : HistogramVMBase
    {
        #region Methods

        public override void UpdateUnits(ResultValueType resultValueType)
        {
            switch (resultValueType)
            {
                case ResultValueType.Count:
                    YAxisTitle = "Average Count";
                    break;
                case ResultValueType.AreaSize:
                    YAxisTitle = "Average Area Size (µm²)";
                    break;
                default:
                    base.UpdateUnits(resultValueType);
                    break;
            }
        }

        /// <summary>
        /// Generate lot average histogram.
        /// </summary>
        public override void Generate(List<WaferStatsData> statsData, Dictionary<string, DefectBin> legends)
        {
            UpdateChart(() =>
            {
                base.Generate(statsData, legends);

                Chart.ViewXY.FreeformPointLineSeries.Clear();
                YAxis.Maximum = 10;
                XAxis.Maximum = 3;

                // Histogram title
                HistogramTitle = "Lot Average by Defect class";
                ShowLegend = false;

                if (statsData.Count > 0)
                {
                    var slotsIds = statsData.Select(x => x.SlotId).Distinct().ToList();
                    var data = statsData.GroupBy(x => x.ResultValue.Name)
                                        .Select(x => new AverageHistogramData()
                                        {
                                            DefectClass = x.Select(y => y.ResultValue.Name).FirstOrDefault(),
                                            Max = x.Max(y => y.ResultValue.Value),
                                            Min = x.Min(y => y.ResultValue.Value),
                                            Mean = Math.Round(x.Sum(y => y.ResultValue.Value) / slotsIds.Count, 2),
                                            Sum = Math.Round(x.Sum(y => y.ResultValue.Value), 2)
                                        }
                                                )
                                        .OrderBy(x => x.DefectClass)
                                        .ToList();
                    // Mettre le mininum à zero quand le defaut n'a pas été vu sur tous les slots
                    foreach (var item in data)
                    {
                        if ((Math.Abs(item.Min - item.Max) < 0.001 && Math.Abs(item.Min - item.Mean) > 0.001) || Math.Abs(item.Min - item.Sum) < 0.001)
                            item.Min = 0;
                    }
                    // Histogram max values
                    YAxis.Maximum = data.Max(x => x.Max);
                    XAxis.Maximum = data.Count + 1;

                    int positionX = 0;

                    foreach (var avgHistogramData in data)
                    {
                        positionX++;
                        // Create a bar and add to barseries collection
                        var barSeries = CreateBarSeries(legends[avgHistogramData.DefectClass].Label, legends[avgHistogramData.DefectClass].Color);
                        barSeries.Values = new[]
                        {
                        CreateBarSeriesValue(positionX, avgHistogramData.Mean)
                    };
                        Chart.ViewXY.BarSeries.Add(barSeries);
                        // Create the bar error
                        CreateErrorBar(positionX, avgHistogramData.Max, avgHistogramData.Mean, avgHistogramData.Min);
                        AddCustomXAxisTick(legends, avgHistogramData.DefectClass, positionX);
                    }
                }
            });
        }

        /// <summary>
        /// Create an error bar.
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="max"></param>
        /// <param name="mean"></param>
        /// <param name="min"></param>
        private void CreateErrorBar(int positionX, double max, double mean, double min)
        {
            var series = new FreeformPointLineSeries();//todo
            var aPoints = new List<SeriesErrorPoint>();

            // On ajoute le Min, Moy et Max à l'histogram
            series.ErrorBars.ShowYError = true;
            series.ErrorBars.YColor = Media.Color.FromArgb(255, 0, 0, 255);
            series.PointsVisible = true;
            series.ShowInLegendBox = false;
            series.LineVisible = true;
            series.PointCountLimitEnabled = true;
            series.PointsType = PointsType.ErrorPoints;
            series.PointStyle.Width = 6;
            series.PointStyle.Height = 6;
            series.Visible = true;

            // Détermination de la longueur des bars
            double dYPlus = max - mean;
            double dYMinus = mean - min;

            var serieErrorPoint = new SeriesErrorPoint
            {
                X = positionX,
                Y = mean,
                ErrorYPlus = dYPlus,
                ErrorYMinus = dYMinus,
            };
            aPoints.Add(serieErrorPoint);
            series.PointsWithErrors = aPoints.ToArray();
            Chart.ViewXY.FreeformPointLineSeries.Add(series);
        }

        #endregion Mthods

        //=================================================================
        // Internal classes
        //=================================================================
        private class AverageHistogramData
        {
            public string DefectClass;
            public double Min;
            public double Max;
            public double Mean;
            public double Sum;
        }
    }
}
