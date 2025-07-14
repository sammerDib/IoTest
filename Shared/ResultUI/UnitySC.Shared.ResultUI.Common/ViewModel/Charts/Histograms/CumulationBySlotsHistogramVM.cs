using System.Collections.Generic;
using System.Linq;

using LightningChartLib.WPF.Charting;
using LightningChartLib.WPF.Charting.SeriesXY;
using LightningChartLib.WPF.Charting.Titles;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;

using Media = System.Windows.Media;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Charts.Histograms
{
    public class CumulationBySlotsHistogramVM : HistogramVMBase
    {
        private Dictionary<int, Media.Color> _dictionarySlotsLegends;

        #region Constructor

        public CumulationBySlotsHistogramVM()
        {
            InitSlotsLegends();
        }

        #endregion Constructor

        #region Methods

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

        public override void Generate(List<WaferStatsData> statsData, Dictionary<string, DefectBin> legends)
        {
            UpdateChart(() =>
            {
                base.Generate(statsData, legends);

                HistogramTitle = "Slots by Defect class";
                ShowLegend = true;

                if (statsData.Count > 0)
                {
                    // Get defect classes present in lot
                    var slotsIds = statsData.Select(x => x.SlotId).Distinct().ToList();
                    var defectClasses = statsData.Select(x => x.ResultValue.Name).Distinct().ToList();

                    YAxis.Maximum = statsData.GroupBy(x => x.ResultValue.Name)
                                           .Select(x => new
                                           {
                                               HistoMax = x.Sum(y => y.ResultValue.Value)
                                           })
                                            .Max(y => y.HistoMax);
                    XAxis.Maximum = defectClasses.Count + 5;

                    foreach (int slotId in slotsIds)
                    {
                        int positionX = 0;
                        var barSeries = InitSlotsBarSeries(slotId);
                        var values = new List<BarSeriesValue>();
                        foreach (string defectClass in defectClasses)
                        {
                            positionX++;
                            double nbDefects = statsData
                                                    .Where(x => x.SlotId == slotId && x.ResultValue.Name == defectClass)
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
                    int lblPosition = 0;
                    foreach (string defClass in defectClasses)
                    {
                        lblPosition++;
                        AddCustomXAxisTick(legends, defClass, lblPosition);
                    }
                }
            });
        }

        /// <summary>
        /// Initialize barseries.
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        private BarSeries InitSlotsBarSeries(int slotId)
        {
            var barSeries = new BarSeries
            {
                Fill = new Fill
                {
                    Color = _dictionarySlotsLegends[slotId],
                    GradientFill = GradientFill.Solid
                },
                Title = new SeriesTitle
                {
                    Text = "S" + slotId,
                    Visible = false
                },
                IncludeInAutoFit = true,
                BarThickness = 30
            };

            return barSeries;
        }

        /// <summary>
        /// Init slots legends colors.
        /// </summary>
        private void InitSlotsLegends()
        {
            // Init dictionary
            _dictionarySlotsLegends = new Dictionary<int, Media.Color>
            {
                // sequential color map
                { 1, Media.Color.FromRgb(153, 015, 015) },
                { 2, Media.Color.FromRgb(178, 044, 044) },
                { 3, Media.Color.FromRgb(204, 081, 081) },
                { 4, Media.Color.FromRgb(229, 126, 126) },
                { 5, Media.Color.FromRgb(255, 178, 178) },

                { 6, Media.Color.FromRgb(153, 084, 015) },
                { 7, Media.Color.FromRgb(178, 111, 044) },
                { 8, Media.Color.FromRgb(204, 142, 081) },
                { 9, Media.Color.FromRgb(229, 177, 126) },
                { 10, Media.Color.FromRgb(255, 216, 178) },

                { 11, Media.Color.FromRgb(107, 153, 015) },
                { 12, Media.Color.FromRgb(133, 178, 044) },
                { 13, Media.Color.FromRgb(163, 204, 081) },
                { 14, Media.Color.FromRgb(195, 229, 126) },
                { 15, Media.Color.FromRgb(229, 255, 178) },

                { 16, Media.Color.FromRgb(015, 107, 153) },
                { 17, Media.Color.FromRgb(044, 133, 178) },
                { 18, Media.Color.FromRgb(081, 163, 204) },
                { 19, Media.Color.FromRgb(126, 195, 229) },
                { 20, Media.Color.FromRgb(178, 229, 255) },

                { 21, Media.Color.FromRgb(098, 015, 153) },
                { 22, Media.Color.FromRgb(066, 044, 178) },
                { 23, Media.Color.FromRgb(101, 081, 204) },
                { 24, Media.Color.FromRgb(143, 126, 229) },
                { 25, Media.Color.FromRgb(191, 178, 255) }
            };
        }

        #endregion Methods
    }
}
