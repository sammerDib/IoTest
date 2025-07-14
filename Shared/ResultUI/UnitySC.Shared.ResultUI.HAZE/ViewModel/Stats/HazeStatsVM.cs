using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;

using LightningChartLib.WPF.ChartingMVVM;
using LightningChartLib.WPF.ChartingMVVM.Axes;
using LightningChartLib.WPF.ChartingMVVM.SeriesXY;
using LightningChartLib.WPF.ChartingMVVM.Views.ViewXY;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.HAZE;
using UnitySC.Shared.ResultUI.Common;
using UnitySC.Shared.ResultUI.Common.Enums;
using UnitySC.Shared.ResultUI.Common.Message;
using UnitySC.Shared.ResultUI.Common.ViewModel;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.ResultUI.HAZE.ViewModel.Stats
{
    public class HazeStatsVM : LotStatsVM
    {
        private readonly IMessenger _messenger;
        
        #region Properties
        
        private List<WaferResultData> _waferResultDatas;

        public List<WaferResultData> WaferResultDatas
        {
            get { return _waferResultDatas; }
            set { SetProperty(ref _waferResultDatas, value); }
        }
        
        private int _selectedHazeType;

        public int SelectedHazeType
        {
            get { return _selectedHazeType; }
            set
            {
                if (SetProperty(ref _selectedHazeType, value))
                {
                    OnSelectedHazeTypeChanged();
                }
            }
        }

        public Func<int, ResultState> IntToResultStateStringFunc { get; }

        public Func<WaferResultData, int, string> WaferResultToMinFunc { get; }

        public Func<WaferResultData, int, string> WaferResultToMaxFunc { get; }

        public Func<WaferResultData, int, string> WaferResultToMeanFunc { get; }

        public Func<WaferResultData, int, string> WaferResultToStdDevFunc { get; }

        #region Chart

        private BarSeriesCollection _barSeriesCollection;

        public BarSeriesCollection BarSeriesCollection
        {
            get { return _barSeriesCollection; }
            set { SetProperty(ref _barSeriesCollection, value); }
        }

        private CustomAxisTickCollection _customAxisXTicks;

        public CustomAxisTickCollection CustomAxisXTicks
        {
            get { return _customAxisXTicks; }
            set { SetProperty(ref _customAxisXTicks, value); }
        }

        private FreeformPointLineSeriesCollection _freeforms;

        public FreeformPointLineSeriesCollection Freeforms
        {
            get { return _freeforms; }
            set { SetProperty(ref _freeforms, value); }
        }

        // Accessor used to manipulate the chart in ViewModel
        public LightningChart Chart { get; set; }

        private bool _resetChartFlag;

        public bool ResetChartFlag
        {
            get { return _resetChartFlag; }
            set { SetProperty(ref _resetChartFlag, value); }
        }

        #endregion

        #endregion

        public void OnChangeSelectedResultFullName(DisplaySelectedResultFullNameMessage msg)
        {
            SelectedResultFullName = msg.SelectedResultFullName;
        }

        public HazeStatsVM()
        {
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();

            LotViews = StatsFactory.GetEnumsWithDescriptions<LotView>();
            LotSelectedView = new KeyValuePair<LotView, string>();

            _messenger.Register<DisplaySelectedResultFullNameMessage>(this, (r, m) => OnChangeSelectedResultFullName(m));

            IntToResultStateStringFunc = i => (ResultState)i;
            
            WaferResultToMinFunc = (data, i) => GetData(data, (HazeType)i, ResultValueType.Min);        
            WaferResultToMaxFunc = (data, i) => GetData(data, (HazeType)i, ResultValueType.Max);        
            WaferResultToMeanFunc = (data, i) => GetData(data, (HazeType)i, ResultValueType.Mean);
            WaferResultToStdDevFunc = (data, i) => GetData(data, (HazeType)i, ResultValueType.StdDev);
        }

        private static string GetData(WaferResultData data, HazeType hazeType, ResultValueType valueType)
        {
            var resultValue = data.ResultItem.ResultItemValues.SingleOrDefault(value => value.Name.Equals(hazeType.ToString()) && value.Type == (int)valueType);
            return resultValue != null ? resultValue.Value.ToString("F5", CultureInfo.InvariantCulture) : "-";
        }

        protected override void OnDeactivated()
        {
            _messenger.Unregister<DisplaySelectedResultFullNameMessage>(this);
            base.OnDeactivated();
        }
        
        public override void UpdateStats(object stats) // to define more accuretly
        {
            if (stats is WaferResultData[] statsArray)
            {
                var waferResultDatas = statsArray.Where(data => data?.ResultItem != null).ToList();
                WaferResultDatas = new List<WaferResultData>(waferResultDatas);
                UpdateChart();
            }
        }

        private void UpdateChart()
        {
            if (WaferResultDatas == null) return;

            Chart?.BeginUpdate();

            CustomAxisXTicks = new CustomAxisTickCollection();
            Freeforms = new FreeformPointLineSeriesCollection();
            BarSeriesCollection = new BarSeriesCollection();

            var values = new List<BarSeriesValue>();

            var barSeries = new BarSeries
            {
                Fill =
                {
                    Color = Color.FromRgb(30, 137, 175),
                    Style = RectFillStyle.ColorOnly,
                    GradientFill = GradientFill.Solid
                },
                BorderWidth = 0,
                BarThickness = 30,
                LabelStyle =
                {
                    Visible = false,
                    Angle = 0,
                    VerticalAlign = BarsTitleVerticalAlign.BarTop
                },
                Shadow =
                {
                    Visible = false
                }
            };

            var hazeType = (HazeType)SelectedHazeType;

            foreach (var waferResultData in WaferResultDatas)
            {
                if (waferResultData.ResultItem == null) continue;

                int positionX = waferResultData.SlotId;

                var matchHazeTypeResult = waferResultData.ResultItem.ResultItemValues.Where(value => value.Name.Equals(hazeType.ToString())).ToList();

                double? min = matchHazeTypeResult.SingleOrDefault(value => value.Type == (int)ResultValueType.Min)?.Value;
                double? max = matchHazeTypeResult.SingleOrDefault(value => value.Type == (int)ResultValueType.Max)?.Value;
                double? mean = matchHazeTypeResult.SingleOrDefault(value => value.Type == (int)ResultValueType.Mean)?.Value;

                if (!min.HasValue || !max.HasValue || !mean.HasValue) continue;

                values.Add(new BarSeriesValue
                {
                    Location = positionX,
                    Value = mean.Value
                });

                CustomAxisXTicks.Add(new CustomAxisTick
                {
                    AxisValue = positionX,
                    Style = CustomTickStyle.TickAndGrid,
                    Color = Color.FromArgb(35, 0, 0, 255),
                    LabelText = positionX.ToString(CultureInfo.InvariantCulture)
                });

                var series = new FreeformPointLineSeries();
                var aPoints = new List<SeriesErrorPoint>();

                // On ajoute le Min, Moy et Max à l'histogram
                series.ErrorBars.ShowYError = true;
                series.ErrorBars.YColor = Color.FromArgb(255, 0, 0, 255);
                series.PointsVisible = true;
                series.ShowInLegendBox = false;
                series.LineVisible = false;
                series.PointCountLimitEnabled = true;
                series.PointsType = PointsType.ErrorPoints;
                series.PointStyle.Width = 6;
                series.PointStyle.Height = 6;

                // Détermination de la longueur des bars
                double dYPlus = max.Value - mean.Value;
                double dYMinus = mean.Value - min.Value;

                var serieErrorPoint = new SeriesErrorPoint
                {
                    X = positionX,
                    Y = mean.Value,
                    ErrorYPlus = dYPlus,
                    ErrorYMinus = dYMinus,
                };
                aPoints.Add(serieErrorPoint);
                series.PointsWithErrors = aPoints.ToArray();
                Freeforms.Add(series);
            }

            barSeries.Values = values.ToArray();

            BarSeriesCollection.Add(barSeries);

            Chart?.EndUpdate();

            ResetChartFlag = !ResetChartFlag;
        }

        public override void SelectLotView(object lotview)
        {
            var lv = (LotView)lotview;
            LotSelectedView = LotViews.FirstOrDefault(x => x.Key == lv);
        }

        private void OnSelectedHazeTypeChanged()
        {
            UpdateChart();
        }
    }
}
