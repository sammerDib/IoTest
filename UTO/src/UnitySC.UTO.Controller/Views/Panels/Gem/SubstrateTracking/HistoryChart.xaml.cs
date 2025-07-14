using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Agileo.Semi.Gem300.Abstractions.E90;

using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.UTO.Controller.Views.Panels.Gem.SubstrateTracking.CustomChart;

using Separator = LiveCharts.Wpf.Separator;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.SubstrateTracking
{
    /// <summary>
    /// Interaction logic for HistoryChart.xaml
    /// </summary>
    public partial class HistoryChart : INotifyPropertyChanged
    {
        #region Members

        private AxesCollection _irregularAxis;
        private CartesianMapper<HistoryPoint> _mapper;
        private readonly LinearGradientBrush _gapBrush;

        #endregion

        #region Event

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor

        public HistoryChart()
        {
            InitializeComponent();

            #region Brush
            
            _gapBrush = new LinearGradientBrush();

            _gapBrush.StartPoint = new Point(0.0, -3);
            _gapBrush.EndPoint = new Point(1, 4);

            _gapBrush.GradientStops.Add(
                new GradientStop(LineStroke.Color, 0.0));
            _gapBrush.GradientStops.Add(
                new GradientStop(LineStroke.Color, 0.495));

            _gapBrush.GradientStops.Add(
                new GradientStop(GUI.Common.Vendor.UIComponents.XamlResources.Shared.Brushes.BackgroundBrush.Color, 0.495));
            _gapBrush.GradientStops.Add(
                new GradientStop(GUI.Common.Vendor.UIComponents.XamlResources.Shared.Brushes.BackgroundBrush.Color, 0.505));

            _gapBrush.GradientStops.Add(
                new GradientStop(
                    LineStroke.Color,
                    0.505));
            _gapBrush.GradientStops.Add(
                new GradientStop(
                    LineStroke.Color,
                    1.0));

            #endregion
            
            UpdateAxis(new List<double>(), new List<double>());
            Durations = new VisualElementsCollection();
        }

        #endregion

        #region Properties

        public string[] Labels { get; set; }
        public VisualElementsCollection Durations { get; set; }
        
        public CartesianMapper<HistoryPoint> Mapper
        {
            get => _mapper;
            set
            {
                _mapper = value;
                OnPropertyChanged(nameof(Mapper));
            }
        }

        public AxesCollection IrregularAxis
        {
            get => _irregularAxis;
            set
            {
                _irregularAxis = value;
                OnPropertyChanged(nameof(IsSubstrate));
                OnPropertyChanged(nameof(IrregularAxis));
            }
        }

        public bool IsSubstrate => SubstrateToDisplay is not null || (SubstrateListToDisplay is not null && SubstrateListToDisplay.Count > 0);

        #endregion

        #region DependencyProperty

        public static readonly DependencyProperty SubstrateToDisplayProperty =
            DependencyProperty.Register(
                nameof(SubstrateToDisplay),
                typeof(Substrate),
                typeof(HistoryChart),
                new PropertyMetadata(default(Substrate), PropertyChangedCallback));

        [Category("Main")]
        public Substrate SubstrateToDisplay
        {
            get => (Substrate)GetValue(SubstrateToDisplayProperty);
            set => SetValue(SubstrateToDisplayProperty, value);
        }

        public static readonly DependencyProperty SubstrateListToDisplayProperty =
            DependencyProperty.Register(
                nameof(SubstrateListToDisplay),
                typeof(List<Substrate>),
                typeof(HistoryChart),
                new PropertyMetadata(default(List<Substrate>), PropertyChangedCallback));

        [Category("Main")]
        public List<Substrate> SubstrateListToDisplay
        {
            get => (List<Substrate>)GetValue(SubstrateListToDisplayProperty);
            set => SetValue(SubstrateListToDisplayProperty, value);
        }

        private static void PropertyChangedCallback(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is HistoryChart self )
            {
                if (e.Property.Name.Equals(nameof(SubstrateToDisplay)))
                {
                    if (e.NewValue is Substrate && (self.SubstrateListToDisplay == null || self.SubstrateListToDisplay.Count == 0))
                    {
                        self.Refresh();
                    }
                    else
                    {
                        self.ClearSerie();
                    }
                }
                else if(e.Property.Name.Equals(nameof(SubstrateListToDisplay)))
                {
                    if (e.NewValue is List<Substrate> substrates)
                    {
                        if (substrates.Count > 0)
                        {
                            self.Refreshlist();
                        }
                        if (substrates.Count == 0)
                        {
                            self.ClearSerie();
                        }
                    }
                    else
                    {
                        self.ClearSerie();
                    }
                }
                self.OnPropertyChanged(nameof(IsSubstrate));
            }
        }

        public static readonly DependencyProperty ChartViewProperty = DependencyProperty.Register(
            nameof(ChartView),
            typeof(HistoryChartType),
            typeof(HistoryChart),
            new PropertyMetadata(HistoryChartType.Duration, ChartViewChangedCallback));

        [Category("Main")]
        public HistoryChartType ChartView
        {
            get => (HistoryChartType)GetValue(ChartViewProperty);
            set => SetValue(ChartViewProperty, value);
        }

        private static void ChartViewChangedCallback(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is HistoryChart self
                && e.NewValue is HistoryChartType
                && self.SubstrateToDisplay != null)
            {
                self.Refresh();
            }
        }

        public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(
            nameof(SelectionMode),
            typeof(SelectionMode),
            typeof(HistoryChart),
            new PropertyMetadata(SelectionMode.Single));

        [Category("Main")]
        public SelectionMode SelectionMode
        {
            get => (SelectionMode)GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }

        public static readonly DependencyProperty LineStrokeProperty =
            DependencyProperty.Register(
                nameof(LineStroke),
                typeof(SolidColorBrush),
                typeof(HistoryChart),
                new PropertyMetadata(default(SolidColorBrush)));

        [Category("Main")]
        public SolidColorBrush LineStroke
        {
            get => (SolidColorBrush)GetValue(LineStrokeProperty);
            set => SetValue(LineStrokeProperty, value);
        }

        public static readonly DependencyProperty SeparatorStrokeProperty =
            DependencyProperty.Register(
                nameof(SeparatorStroke),
                typeof(SolidColorBrush),
                typeof(HistoryChart),
                new PropertyMetadata(default(SolidColorBrush)));

        [Category("Main")]
        public SolidColorBrush SeparatorStroke
        {
            get => (SolidColorBrush)GetValue(SeparatorStrokeProperty);
            set => SetValue(SeparatorStrokeProperty, value);
        }

        public static readonly DependencyProperty StrokeListProperty =
            DependencyProperty.Register(
                nameof(StrokeList),
                typeof(List<SolidColorBrush>),
                typeof(HistoryChart),
                new PropertyMetadata(default(List<SolidColorBrush>)));

        [Category("Main")]
        public List<SolidColorBrush> StrokeList
        {
            get => (List<SolidColorBrush>)GetValue(StrokeListProperty);
            set => SetValue(StrokeListProperty, value);
        }

        #endregion

        #region Methods

        private void UpdateSerie(Substrate substrate)
        {
            Mapper = Mappers.Xy<HistoryPoint>().X(item => item.Point.X).Y(item => item.Point.Y);

            ChartValues<HistoryPoint> serieValues = new ChartValues<HistoryPoint>();
            if (HistoryCartesianChart.VisualElements != null
                && HistoryCartesianChart.VisualElements.Count > 0)
            {
                Durations.Clear();
            }
            var values = new List<double>();
            var suppressedSeconds = new List<double>();
            double startSecond;
            double endSecond;
            int locId;

            var labels = GetLocationsLabels(substrate.SubstHistory);

            foreach (var historyItem in substrate.SubstHistory)
            {
                locId = labels.FindIndex(x => x.Equals(historyItem.LocationId));

                startSecond = GetStartSeconds(historyItem);
                endSecond = GetEndSeconds(historyItem);

                var secondDuration = endSecond - startSecond;
                var timeText = GetDisplayedTime(secondDuration);
                Durations.Add(
                    new VisualElement
                    {
                        X = startSecond + (secondDuration / 2),
                        Y = locId,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top,
                        UIElement = new TextBlock()
                        {
                            Text = timeText, Padding = new Thickness(0, 0, 0, 10)
                        }
                    });

                serieValues.Add(new HistoryPoint(new ObservablePoint(startSecond, locId), false));
                values.Add(startSecond);
                suppressedSeconds.Add(0);
                serieValues.Add(new HistoryPoint(new ObservablePoint(endSecond, locId), false));
                values.Add(endSecond);
                suppressedSeconds.Add(0);
            }


            HistoryCartesianChart.Series.Add(new CustomStepLineSeries(Mapper)
            {
                Values = serieValues,
                Stroke = LineStroke,
                AlternativeStroke = LineStroke
            });
            Labels = labels.ToArray();

            OnPropertyChanged(nameof(Labels));
            OnPropertyChanged(nameof(Durations));
            UpdateAxis(values, suppressedSeconds);
        }

        private void UpdateSerieWithGaps(Substrate substrate)
        {
            Mapper = Mappers.Xy<HistoryPoint>()
                .X(item => item.Point.X)
                .Y(item => item.Point.Y)
                .Stroke(
                    item => item.IsCompressed
                        ? _gapBrush
                        : null);

            ChartValues<HistoryPoint> serieValues = new ChartValues<HistoryPoint>();
            if (HistoryCartesianChart.VisualElements != null
                && HistoryCartesianChart.VisualElements.Count > 0)
            {
                Durations.Clear();
            }
            var secondsDurations = new List<double>();
            var suppressedSeconds = new List<double>();
            var values = new List<double>();
            double startSecond;
            double endSecond;
            int locId;

            //Get all durations
            foreach (var historyItem in substrate.SubstHistory)
            {
                startSecond = GetStartSeconds(historyItem);
                endSecond = GetEndSeconds(historyItem);

                secondsDurations.Add(endSecond - startSecond);
            }

            var median = GetMedian(secondsDurations);
            var labels = GetLocationsLabels(substrate.SubstHistory);

            foreach (var historyItem in substrate.SubstHistory)
            {
                double endSuppressedSeconds = 0;

                locId = labels.FindIndex(x => x.Equals(historyItem.LocationId));

                startSecond = GetStartSeconds(historyItem);
                endSecond = GetEndSeconds(historyItem);

                var duration = endSecond - startSecond;
                var isCompressed = false;
                if (duration <= median)
                {
                    if (suppressedSeconds.Count > 0 && suppressedSeconds.Max() > 0)
                    {
                        var max = suppressedSeconds.Max();
                        startSecond -= max;
                        endSecond -= max;
                        endSuppressedSeconds = max;
                    }
                }
                else
                {
                    isCompressed = true;

                    #region StartDate

                    if (suppressedSeconds.Count > 0 && suppressedSeconds.Max() > 0)
                    {
                        startSecond -= suppressedSeconds.Max();
                    }

                    #endregion

                    #region End date

                    endSecond = startSecond + median;
                    if (suppressedSeconds.Count > 0 && suppressedSeconds.Max() != 0)
                    {
                        endSuppressedSeconds = suppressedSeconds.Max();
                    }

                    endSuppressedSeconds += duration - median;

                    #endregion
                }

                #region Display time

                var timeText = GetDisplayedTime(duration);

                //add text with time
                Durations.Add(
                    new VisualElement
                    {
                        X = startSecond + ((endSecond - startSecond) / 2),
                        Y = locId,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top,
                        UIElement = new TextBlock()
                        {
                            Text = timeText, Padding = new Thickness(0, 0, 0, 10)
                        }
                    });

                #endregion

                serieValues.Add(new HistoryPoint(new ObservablePoint(startSecond, locId), false));
                values.Add(startSecond);
                suppressedSeconds.Add(
                    suppressedSeconds.Count > 0
                        ? suppressedSeconds.Max()
                        : 0);
                serieValues.Add(new HistoryPoint(new ObservablePoint(endSecond, locId), isCompressed));
                values.Add(endSecond);
                suppressedSeconds.Add(endSuppressedSeconds);
            }

            HistoryCartesianChart.Series.Add(new CustomStepLineSeries(Mapper)
            {
                Values = serieValues,
                Stroke = LineStroke,
                AlternativeStroke = LineStroke
            });
            Labels = labels.ToArray();

            OnPropertyChanged(nameof(Labels));
            OnPropertyChanged(nameof(Durations));
            UpdateAxis(values, suppressedSeconds);
        }

        private void UpdateSerieUniform(Substrate substrate)
        {
            Mapper = Mappers.Xy<HistoryPoint>().X(item => item.Point.X).Y(item => item.Point.Y);

            ChartValues<HistoryPoint> serieValues = new ChartValues<HistoryPoint>();
            if (HistoryCartesianChart.VisualElements != null
                && HistoryCartesianChart.VisualElements.Count > 0)
            {
                Durations.Clear();
            }
            var suppressedSeconds = new List<double>();
            var values = new List<double>();
            double startSecond;
            double endSecond;
            int locId;
            var standardDuration = 10;

            var labels = GetLocationsLabels(substrate.SubstHistory);

            foreach (var historyItem in substrate.SubstHistory)
            {
                double endSuppressedSeconds = 0;
                double startSuppressedSeconds = 0;

                locId = labels.FindIndex(x => x.Equals(historyItem.LocationId));

                startSecond = GetStartSeconds(historyItem);
                endSecond = GetEndSeconds(historyItem);

                var duration = endSecond - startSecond;

                #region StartDate

                if (suppressedSeconds.Count > 0)
                {
                    startSuppressedSeconds += suppressedSeconds.Last();
                    startSecond -= startSuppressedSeconds;
                }

                #endregion

                #region End date

                endSuppressedSeconds += endSecond - (startSecond + standardDuration);
                endSecond = startSecond + standardDuration;

                #endregion

                #region Display time

                var timeText = GetDisplayedTime(duration);

                //add text with time
                Durations.Add(
                    new VisualElement
                    {
                        X = startSecond + ((endSecond - startSecond) / 2),
                        Y = locId,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top,
                        UIElement = new TextBlock()
                        {
                            Text = timeText, Padding = new Thickness(0, 0, 0, 10)
                        }
                    });

                #endregion

                serieValues.Add(new HistoryPoint(new ObservablePoint(startSecond, locId), false));
                values.Add(startSecond);
                suppressedSeconds.Add(startSuppressedSeconds);
                serieValues.Add(new HistoryPoint(new ObservablePoint(endSecond, locId), false));
                values.Add(endSecond);
                suppressedSeconds.Add(endSuppressedSeconds);
            }

            HistoryCartesianChart.Series.Add(new CustomStepLineSeries(Mapper)
            {
                Values = serieValues,
                Stroke = LineStroke,
                AlternativeStroke = LineStroke
            });
            Labels = labels.ToArray();

            OnPropertyChanged(nameof(Labels));
            OnPropertyChanged(nameof(Durations));
            UpdateAxis(values, suppressedSeconds);
        }

        private void UpdateAxis(List<double> values, List<double> removedSeconds)
        {
            var themeColors = ThemeHelper.GetColors();
            if (values is null || values.Count == 0)
            {
                IrregularAxis = new AxesCollection()
                {
                    new WindowAxis()
                    {
                        MinValue = 0,
                        MaxValue = 1,
                        Windows =
                            new AxisWindowCollection()
                            {
                                new SelectedDataAxis(values, new List<double>())
                            },
                        Separator = new Separator()
                        {
                            StrokeDashArray = new DoubleCollection() {5.0, 2.0},
                            Stroke = new SolidColorBrush(themeColors["ControlActionForeground"])
                        }
                    }
                };
                return;
            }

            var minVal = values.Min();
            for (var i = 0; i < values.Count; i++)
            {
                values[i] -= minVal;
                removedSeconds[i] += minVal;
            }

            for (int i = 0; i < HistoryCartesianChart.Series.Count; i++)
            {
                foreach (var value in HistoryCartesianChart.Series[i].Values)
                {
                    if (value is HistoryPoint historyPoint)
                    {
                        historyPoint.Point.X -= minVal;
                    }
                }
            }
            
            foreach (var duration in Durations)
            {
                duration.X -= minVal;
            }

            var _from = values.Min();
            var _to = values.Max() + 1;

            IrregularAxis = new AxesCollection()
            {
                new WindowAxis()
                {
                    MinValue = _from,
                    MaxValue = _to,
                    LabelsRotation = 45,
                    Windows =
                        new AxisWindowCollection()
                        {
                            new SelectedDataAxis(values, removedSeconds)
                        },
                    Separator = new Separator()
                    {
                        StrokeDashArray = new DoubleCollection() {5.0, 2.0},
                        Stroke = new SolidColorBrush(themeColors["ControlActionForeground"])
                    }
                }
            };
            
            OnPropertyChanged(nameof(SeriesCollection));
            OnPropertyChanged(nameof(Durations));
        }
        
        public static double GetMedian(List<double> sourceNumbers)
        {
            if (sourceNumbers == null || sourceNumbers.Count == 0)
            {
                throw new ArgumentException("Median of empty array not defined.");
            }

            //make sure the list is sorted, but use a new array
            var sortedPNumbers = new double[sourceNumbers.Count];
            sourceNumbers.CopyTo(sortedPNumbers);
            Array.Sort(sortedPNumbers);

            //get the median
            var size = sortedPNumbers.Length;
            var mid = size / 2;
            var median = size % 2 != 0
                ? sortedPNumbers[mid]
                : (sortedPNumbers[mid] + sortedPNumbers[mid - 1]) / 2;
            return median;
        }

        private double GetStartSeconds(SubstrateHistoryItem historyItem)
        {
            return Math.Round(new TimeSpan(historyItem.TimeIn.Ticks).TotalSeconds);
        }

        private double GetEndSeconds(SubstrateHistoryItem historyItem)
        {
            if (historyItem.TimeOut.HasValue)
            {
                return Math.Round(new TimeSpan(historyItem.TimeOut.Value.Ticks).TotalSeconds);
            }

            return Math.Round(new TimeSpan(DateTime.Now.Ticks).TotalSeconds);
        }

        private List<string> GetLocationsLabels(ReadOnlyCollection<SubstrateHistoryItem> history)
        {
            var labels = history.Select(item => item.LocationId).Distinct();
            return labels.OrderBy(c => c.ToUpper()).ToList();
        }

        private List<string> GetLocationsLabels(List<Substrate> substrates)
        {
            List<string> labels = new List<string>();
            foreach (Substrate substrate in substrates)
            {
                labels.AddRange(substrate.SubstHistory.Select(item => item.LocationId)); 
            }

            return labels.Distinct().OrderBy(c => c.ToUpper()).ToList();
        }

        private string GetDisplayedTime(double duration)
        {
            var valueTick = (long)(duration * TimeSpan.TicksPerSecond);
            var time = new TimeSpan(valueTick);
            var hours = $"{time.Hours} {SubstrateTrackingRessources.SUB_TRACK_HOUR} ";
            var min = $"{time.Minutes} {SubstrateTrackingRessources.SUB_TRACK_MIN} ";
            var sec = $"{time.Seconds} {SubstrateTrackingRessources.SUB_TRACK_SECOND}";
            var timeText = "";
            if (time.Hours > 0)
            {
                timeText += hours;
            }

            if (time.Minutes > 0)
            {
                timeText += min;
            }

            if (time.Seconds > 0)
            {
                timeText += sec;
            }

            return timeText;
        }

        private void Refresh()
        {
            if (SubstrateToDisplay != null && SelectionMode == SelectionMode.Single)
            {
                ClearSerie();
                switch (ChartView)
                {
                    case HistoryChartType.Condensed:
                        UpdateSerieWithGaps(SubstrateToDisplay);
                        break;
                    case HistoryChartType.Duration:
                        UpdateSerieUniform(SubstrateToDisplay);
                        break;
                    case HistoryChartType.Time:
                        UpdateSerie(SubstrateToDisplay);
                        break;
                }
            }
        }

        private void Refreshlist()
        {
            Mapper = Mappers.Xy<HistoryPoint>().X(item => item.Point.X).Y(item => item.Point.Y);
            ClearSerie();
            if (HistoryCartesianChart.VisualElements != null
                && HistoryCartesianChart.VisualElements.Count > 0)
            {
                Durations.Clear();
            }
            var labels = GetLocationsLabels(SubstrateListToDisplay);
            
            var values = new List<double>();
            var suppressedSeconds = new List<double>();

            List<ChartValues<HistoryPoint>> stepLineValues = new List<ChartValues<HistoryPoint>>();
            List<CustomStepLineSeries> stepLineSeries = new List<CustomStepLineSeries>();

            foreach (Substrate substrate in SubstrateListToDisplay)
            {
                
                double startSecond;
                double endSecond;
                int locId;
                ChartValues<HistoryPoint> currentValues = new ChartValues<HistoryPoint>();

                foreach (var historyItem in substrate.SubstHistory)
                {
                    locId = labels.FindIndex(x => x.Equals(historyItem.LocationId));

                    startSecond = GetStartSeconds(historyItem);
                    endSecond = GetEndSeconds(historyItem);

                    var secondDuration = endSecond - startSecond;
                    var timeText = GetDisplayedTime(secondDuration);
                    Durations.Add(
                        new VisualElement
                        {
                            X = startSecond + (secondDuration / 2),
                            Y = locId,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Top,
                            UIElement = new TextBlock()
                            {
                                Text = timeText,
                                Padding = new Thickness(0, 0, 0, 10)
                            }
                        });
                    currentValues.Add(new HistoryPoint(new ObservablePoint(startSecond, locId), false));
                    values.Add(startSecond);
                    suppressedSeconds.Add(0);
                    currentValues.Add(new HistoryPoint(new ObservablePoint(endSecond, locId), false));
                    values.Add(endSecond);
                    suppressedSeconds.Add(0);
                }
                stepLineValues.Add(currentValues);
            }

            if (values.Count == 0)
            {
                IrregularAxis = new AxesCollection()
                {
                    new WindowAxis()
                    {
                        MinValue = 0,
                        MaxValue = 1,
                        Windows =
                            new AxisWindowCollection()
                            {
                                new SelectedDataAxis(values, new List<double>())
                            },
                        Separator = new Separator()
                        {
                            StrokeDashArray = new DoubleCollection() {5.0, 2.0},
                            Stroke = SeparatorStroke
                        }
                    }
                };
                return;
            }

            var minVal = values.Min();
            for (var i = 0; i < values.Count; i++)
            {
                values[i] -= minVal;
                suppressedSeconds[i] += minVal;
            }

            foreach (ChartValues<HistoryPoint> chartValues in stepLineValues)
            {
                foreach (HistoryPoint point in chartValues)
                {
                    point.Point.X -= minVal;
                }
                stepLineSeries.Add(new CustomStepLineSeries(Mapper){Values = chartValues});
            }

            foreach (var duration in Durations)
            {
                duration.X -= minVal;
            }

            var _from = values.Min();
            var _to = values.Max() + 1;

            HistoryCartesianChart.Series.AddRange(stepLineSeries);
            Labels = labels.ToArray();

            IrregularAxis = new AxesCollection()
            {
                new WindowAxis()
                {
                    MinValue = _from,
                    MaxValue = _to,
                    LabelsRotation = 45,
                    Windows =
                        new AxisWindowCollection()
                        {
                            new SelectedDataAxis(values, suppressedSeconds)
                        },
                    Separator = new Separator()
                    {
                        StrokeDashArray = new DoubleCollection() {5.0, 2.0},
                        Stroke = SeparatorStroke
                    }
                }
            };
            
            UpdateAxis(values, suppressedSeconds);

            OnPropertyChanged(nameof(Labels));
            OnPropertyChanged(nameof(Durations));
        }

        private void ClearSerie()
        {
            bool delete = true;
            for (int i = 0; i < HistoryCartesianChart.Series.Count; i++)
            {
                if (HistoryCartesianChart.Series[i].Values == null)
                {
                    delete = false;
                    break;
                }
            }

            if (delete)
            {
                HistoryCartesianChart.Series.Clear();
            }
        }

        #endregion

        #region Event Handler

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ResetZoomOnClick(object sender, RoutedEventArgs e)
        {
            if (SubstrateToDisplay != null && SelectionMode == SelectionMode.Single)
            {
                Refresh();
            }
            else if(SubstrateListToDisplay != null && SubstrateListToDisplay.Count>0 && SelectionMode != SelectionMode.Single)
            {
                Refreshlist();
            }
        }

        #endregion

        public class HistoryPoint
        {
            public ObservablePoint Point { get; set; }
            public bool IsCompressed { get; set; }

            public HistoryPoint(ObservablePoint point, bool isCompressed)
            {
                Point = point;
                IsCompressed = isCompressed;
            }
        }
    }
}
