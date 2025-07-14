using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Data;
using LightningChartLib.WPF.Charting;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Charts.Histograms
{
    public class SlotDefectClassesHistogramVM : HistogramVMBase
    {
        public override void UpdateUnits(ResultValueType resultValueType)
        {
            switch (resultValueType)
            {
                case ResultValueType.Count:
                    YAxisTitle = "Count";
                    break;
                case ResultValueType.AreaSize:
                    YAxisTitle = "Area Size (µm²)";
                    break;
                default:
                    YAxisTitle = resultValueType.ToString();
                    break;
            }
        }

        /// <summary>
        /// Generate cumulative defect classes histogram.
        /// </summary>
        public override void Generate(List<WaferStatsData> statsData, Dictionary<string, DefectBin> legends)
        {
            UpdateChart(() =>
            {
                base.Generate(statsData, legends);
                HistogramTitle = "Defect classes by Slot";
                ShowLegend = true;

                if (statsData.Count > 0)
                {
                    // Get defect classes present in lot
                    var defectClasses = statsData.Select(x => x.ResultValue.Name).Distinct().ToList();
                    var slotsIds = statsData.Select(x => x.SlotId).Distinct().ToList();

                    YAxis.Maximum = statsData.GroupBy(x => x.SlotId)
                                        .Select(x => new
                                        {
                                            HistoMax = x.Sum(y => y.ResultValue.Value)
                                        }
                                        )
                                        .Max(y => y.HistoMax);
                    XAxis.Maximum = slotsIds.Count + 5;
                    foreach (string defectClass in defectClasses)
                    {
                        int positionX = 0;
                        var barSeries = CreateBarSeries(legends[defectClass].Label, legends[defectClass].Color);
                        var values = new List<BarSeriesValue>();
                        foreach (int slotId in slotsIds)
                        {
                            positionX++;
                            double nbDefects = statsData.Where(x => x.SlotId == slotId && x.ResultValue.Name == defectClass)
                                                 .Select(x => x.ResultValue.Value)
                                                 .DefaultIfEmpty(0)
                                                 .FirstOrDefault();

                            if (nbDefects != 0)
                                values.Add(CreateBarSeriesValue(positionX, nbDefects));
                        }

                        barSeries.Values = values.ToArray();
                        Chart.ViewXY.BarSeries.Add(barSeries);
                    }

                    // Add X axis labels
                    int labelPosition = 0;
                    foreach (int slotId in slotsIds)
                    {
                        labelPosition++;
                        string id = "S" + slotId;
                        AddCustomXAxisTick(id, labelPosition);
                    }
                }
            });
        }
    }
}
