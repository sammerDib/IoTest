using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Charts.Histograms
{
    public class TotalCumulationHistogramVM : HistogramVMBase
    {
        public override void UpdateUnits(ResultValueType resultValueType)
        {
            switch (resultValueType)
            {
                case ResultValueType.Count:
                    YAxisTitle = $"Total Cumulative Count";
                    break;
                case ResultValueType.AreaSize:
                    YAxisTitle = $"Total Cumulative Area Size (µm²)";
                    break;
                default:
                    base.UpdateUnits(resultValueType);
                    break;
            }
        }

        /// <summary>
        /// Genere un histogram avec des barres representant pour chaque classe de defaut, le nombre total de defauts vus sur l'ensemble des slots.
        /// </summary>
        public override void Generate(List<WaferStatsData> statsData, Dictionary<string, DefectBin> legends)
        {
            UpdateChart(() =>
            {
                base.Generate(statsData, legends);

                HistogramTitle = "Lot Total by Defect class";
                ShowLegend = true;

                if (statsData.Count > 0)
                {
                    int positionX = 0;
                    var defectClasses = statsData.OrderBy(x => x.ResultValue.Name)
                        .GroupBy(x => x.ResultValue.Name)
                        .Select(x => (
                            Name: x.Select(y => y.ResultValue.Name).FirstOrDefault(),
                            TotalDefects: x.Sum(y => y.ResultValue.Value)
                        )).ToList();
                    // Determine X  and Y position max
                    YAxis.Maximum = defectClasses.Max(x => x.TotalDefects);
                    XAxis.Maximum = defectClasses.Count + 5;

                    foreach ((string name, double totalDefects) in defectClasses)
                    {
                        positionX++;
                        var barSeries = CreateBarSeries(legends[name].Label, legends[name].Color);
                        barSeries.Values = new[]
                        {
                            CreateBarSeriesValue(positionX, totalDefects)
                        };
                        Chart.ViewXY.BarSeries.Add(barSeries);
                        AddCustomXAxisTick(legends, name, positionX);
                    }
                }
            });
        }
    }
}
