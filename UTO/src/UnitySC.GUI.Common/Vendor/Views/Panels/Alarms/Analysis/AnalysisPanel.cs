using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using Agileo.AlarmModeling;
using Agileo.Common.Localization;
using Agileo.GUI;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;

using UnitySC.GUI.Common.Vendor.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Helpers.Colors;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort;
using UnitySC.GUI.Common.Vendor.Views.Panels.Alarms.Analysis.Core;
using UnitySC.GUI.Common.Vendor.Views.Panels.Alarms.Analysis.Enum;
using UnitySC.GUI.Common.Vendor.Views.Panels.Alarms.Analysis.OxyPlotExtended;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Alarms.Analysis
{
    /// <summary>
    /// Template Model representing the ViewModel (Context) of the panel
    /// </summary>
    public class AnalysisPanel : BusinessPanel
    {
        #region Fields

        private bool _firstShow = true;
        private bool _preventUpdateSource;

        private readonly OxyColor _oxyPlotBackground;

        private readonly List<Color> _fixedColors = new();

        private readonly Dictionary<string, Color> _deviceColors = new();
        private readonly Dictionary<string, AnalysisDeviceOccurrencesWrapper> _deviceOccurrences = new();

        #endregion

        #region Constructors

        static AnalysisPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(AnalysisPanelResources)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisPanel" /> class ONLY FOR the DESIGN MODE.
        /// </summary>
        public AnalysisPanel() : this(null, "Design Time Constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalysisPanel" /> class.
        /// </summary>        
        public AnalysisPanel(IAlarmCenter alarmCenter, string id, IIcon icon = null) : base(id, icon)
        {
            AlarmCenter = alarmCenter;

            Commands.Add(new BusinessPanelCommand(nameof(AnalysisPanelResources.ALARM_ANALYSIS_RELOAD), new DelegateCommand(UpdateSource), IconFactory.PathGeometryFromRessourceKey("RefreshIcon")));
            Commands.Add(new BusinessPanelCommand(nameof(AnalysisPanelResources.ALARM_ANALYSIS_SCREENSHOT), new DelegateCommand(() => TakeScreenShotRequested?.Invoke(this, EventArgs.Empty)), IconFactory.PathGeometryFromRessourceKey("SnapshotIcon")));

            OccurrenceSource.Sort.Add(new SortDefinition<AnalysisOccurrencesWrapper>(nameof(AnalysisOccurrencesWrapper.OccurrenceCount), wrapper => wrapper.OccurrenceCount));
            OccurrenceSource.Sort.Add(new SortDefinition<AnalysisOccurrencesWrapper>(nameof(AnalysisOccurrencesWrapper.OccurrenceDuration), wrapper => wrapper.OccurrenceDuration));

            OccurrenceSource.Search.AddSearchDefinition(nameof(AnalysisPanelResources.ALARM_ANALYSIS_ID), wrapper => wrapper.Alarm.Id.ToString(CultureInfo.InvariantCulture), true);
            OccurrenceSource.Search.AddSearchDefinition(nameof(AnalysisPanelResources.ALARM_ANALYSIS_DESCRIPTION), wrapper => wrapper.Alarm.Description, true);
            
            #region Filters

            SourceFilter = new FilterCollection<AnalysisOccurrencesWrapper, string>(nameof(AnalysisPanelResources.ALARM_ANALYSIS_SOURCE),
                () => _deviceColors.Select(entry => entry.Key),
                alarm => alarm.Alarm.ProviderName);
            
            #endregion

            #region Charts

            _oxyPlotBackground = ToOxyColor(ResourcesHelper.GetSolidColorBrushOrDefault("PanelBackground").Color);
            var oxyPlotForeground = ToOxyColor(ResourcesHelper.GetSolidColorBrushOrDefault("PanelForeground").Color);
            
            SetupPieChartController();

            SourcePieChartModel = new PlotModel
            {
                Background = _oxyPlotBackground,
                TextColor = oxyPlotForeground,
                PlotMargins = new OxyThickness(25)
            };
            OccurrencesPieChartModel = new PlotModel
            {
                Background = _oxyPlotBackground,
                TextColor = oxyPlotForeground,
                PlotMargins = new OxyThickness(25)
            };
            AlarmEvolutionChartModel = new PlotModel
            {
                Background = _oxyPlotBackground,
                TextColor = oxyPlotForeground,
                PlotAreaBorderThickness = new OxyThickness(1, 0, 0, 1)
            };

            AlarmEvolutionChartModel.Legends.Add(new Legend
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.LeftMiddle,
                LegendOrientation = LegendOrientation.Vertical,
                LegendBorderThickness = 0
            });

            #endregion

            #region Fixed colors

            _fixedColors.Add(Color.FromRgb(204, 134, 20));
            _fixedColors.Add(Color.FromRgb(165, 59, 84));
            _fixedColors.Add(Color.FromRgb(85, 140, 140));
            _fixedColors.Add(Color.FromRgb(0, 156, 117));
            _fixedColors.Add(Color.FromRgb(102, 93, 41));
            
            #endregion
        }

        public void TakeScreenShotExecute(FrameworkElement frameworkElement)
        {
            var path = (AgilControllerApplication.Current.ConfigurationManager.Current as ApplicationConfiguration)?.ApplicationPath.AlarmAnalysisCaptureStoragePath;

            if (string.IsNullOrWhiteSpace(path))
            {
                Messages.Show(new UserMessage(MessageLevel.Error, nameof(AnalysisPanelResources.ALARM_ANALYSIS_PATH_NOT_CONFIGURED))
                {
                    CanUserCloseMessage = true,
                    SecondsDuration = 5
                });
                return;
            }

            ScreenshotHelper.MakeCapture("AlarmAnalysis", path, frameworkElement, this);
        }

        #endregion Constructors

        #region Events

        public event EventHandler TakeScreenShotRequested;

        #endregion

        #region Properties

        public IAlarmCenter AlarmCenter { get; }

        public DataTableSource<AnalysisOccurrencesWrapper> OccurrenceSource { get; } = new();

        public FilterCollection<AnalysisOccurrencesWrapper, string> SourceFilter { get; }

        public PlotModel SourcePieChartModel { get; }

        public PlotModel OccurrencesPieChartModel { get; }

        public PlotModel AlarmEvolutionChartModel { get; }

        public PlotController PieChartController { get; } = new();

        #region Filters

        private DateTime _startDate;

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                SetAndRaiseIfChanged(ref _startDate, value);

                if (!_preventUpdateSource)
                {
                    _preventUpdateSource = true;
                    if (EndDate < StartDate)
                    {
                        EndDate = value;
                    }

                    SelectedPeriod = null;

                    _preventUpdateSource = false;
                }

                UpdateSource();
            }
        }

        private DateTime _endDate;

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                SetAndRaiseIfChanged(ref _endDate, value);

                if (!_preventUpdateSource)
                {
                    _preventUpdateSource = true;
                    if (StartDate > EndDate)
                    {
                        StartDate = value;
                    }

                    SelectedPeriod = null;

                    _preventUpdateSource = false;
                }

                UpdateSource();
            }
        }

        public DateTime EndDateIncludingLastDay => EndDate.AddDays(1);

        private Period? _selectedPeriod;

        public Period? SelectedPeriod
        {
            get => _selectedPeriod;
            set
            {
                _preventUpdateSource = true;

                if (value.HasValue)
                {
                    switch (value.Value)
                    {
                        case Period.Today:
                            StartDate = DateTime.Today;
                            EndDate = DateTime.Today;
                            break;
                        case Period.Yesterday:
                            StartDate = DateTime.Today.AddDays(-1);
                            EndDate = DateTime.Today.AddDays(-1);
                            break;
                        case Period.ThisWeek:
                            StartDate = GetStartOfThisWeek(DayOfWeek.Monday);
                            EndDate = DateTime.Today;
                            break;
                        case Period.LastWeek:
                            StartDate = GetStartOfLastWeek(DayOfWeek.Monday);
                            EndDate = StartDate.AddDays(6);
                            break;
                        case Period.Last7Days:
                            StartDate = DateTime.Today.AddDays(-6);
                            EndDate = DateTime.Today;
                            break;
                        case Period.ThisMonth:
                            StartDate = GetStartOfThisMonth();
                            EndDate = DateTime.Today;
                            break;
                        case Period.Last30Days:
                            StartDate = DateTime.Today.AddDays(-29);
                            EndDate = DateTime.Today;
                            break;
                        case Period.LastMonth:
                            StartDate = GetStartOfLastMonth();
                            EndDate = GetLastDayOfLastMonth();
                            break;
                        case Period.Last3Months:
                            StartDate = GetStartOfThisMonth().AddMonths(-2);
                            EndDate = DateTime.Today;
                            break;
                        case Period.Last6Months:
                            StartDate = GetStartOfThisMonth().AddMonths(-5);
                            EndDate = DateTime.Today;
                            break;
                        case Period.ThisYear:
                            StartDate = GetStartOYear();
                            EndDate = DateTime.Today;
                            break;
                        case Period.LastYear:
                            StartDate = new DateTime(DateTime.Today.Year - 1, 1, 1);
                            EndDate = new DateTime(DateTime.Today.Year, 1, 1).AddDays(-1);
                            break;
                        case Period.Last12Months:
                            StartDate = GetStartOfThisMonth().AddMonths(-11);
                            EndDate = DateTime.Today;
                            break;
                    }
                }

                SetAndRaiseIfChanged(ref _selectedPeriod, value);

                _preventUpdateSource = false;

                if (_selectedPeriod != null)
                {
                    UpdateSource();
                }
            }
        }

        #endregion

        private AnalysisOccurrencesWrapper _selectedAlarm;

        public AnalysisOccurrencesWrapper SelectedAlarm
        {
            get => _selectedAlarm;
            set
            {
                SetAndRaiseIfChanged(ref _selectedAlarm, value);
                UpdatePieChartsSelection();
            }
        }
        
        private AlarmAnalysisDisplayMode _displayMode;

        public AlarmAnalysisDisplayMode DisplayMode
        {
            get => _displayMode;
            // ReSharper disable once ValueParameterNotUsed
#pragma warning disable S3237 // "value" parameters should be used
            set
#pragma warning restore S3237 // "value" parameters should be used
            {
                // At the moment, this feature is not available.
                // An evolution of the alarm component is necessary to finalize this functionality.
                // More details in the associated feature in the backlog:
                // https://dev.azure.com/on-agileo/Agileo%20Products/_workitems/edit/8460
                // SetAndRaiseIfChanged(ref _displayMode, value)

                SetAndRaiseIfChanged(ref _displayMode, AlarmAnalysisDisplayMode.Count);
                OnDisplayModeChanged();
            }
        }

        private int _totalCount;

        public int TotalCount
        {
            get => _totalCount;
            set => SetAndRaiseIfChanged(ref _totalCount, value);
        }

        private TimeSpan _totalDuration;

        public TimeSpan TotalDuration
        {
            get => _totalDuration;
            set => SetAndRaiseIfChanged(ref _totalDuration, value);
        }

        private AnalysisDeviceOccurrencesWrapper _mostImpactingDevice;

        public AnalysisDeviceOccurrencesWrapper MostImpactingDevice
        {
            get => _mostImpactingDevice;
            set => SetAndRaiseIfChanged(ref _mostImpactingDevice, value);
        }

        private AlarmAnalysisEvolutionMode _evolutionMode;

        public AlarmAnalysisEvolutionMode EvolutionMode
        {
            get => _evolutionMode;
            set
            {
                SetAndRaiseIfChanged(ref _evolutionMode, value);
                UpdateAlarmEvolutionChart();
            }
        }

        #endregion

        #region Private methods

        #region Handlers

        private void FilteredSourcesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateSource();
        }

        #endregion

        /// <summary>
        /// Iterates through the list of all possible sources to assign them a unique color.
        /// </summary>
        private void BuildDeviceColorDictionary()
        {
            _deviceColors.Clear();

            var devices = AlarmCenter.Repository.GetAlarms().Select(alarm => alarm.ProviderName).Distinct().ToList();
            var colors = _fixedColors.Take(Math.Min(devices.Count, _fixedColors.Count)).ToList();
            var missingColors = devices.Count - _fixedColors.Count;
            if (missingColors > 0)
            {
                if (!ThemeColorGenerator.IsInitialized)
                {
                    ThemeColorGenerator.InitializeWithAppColors();
                }

                colors.AddRange(ThemeColorGenerator.Generate(missingColors));
            }

            var i = 0;
            foreach (var device in devices)
            {
                var color = colors[i];
                _deviceColors.Add(device, color);
                i++;
            }
        }

        private void UpdateSource()
        {
            if (_preventUpdateSource) return;

            // Filters are used as parameters of the alarmCenter
            // So it is necessary to filter the collection before applying the internal logic of the DataTableSource
            var providerNameCollection = SourceFilter.SelectedItems.Any() ? SourceFilter.SelectedItems : null;

            OccurrenceSource.Clear();
            var alarmOccurrences = AlarmCenter.Repository.GetAlarmOccurrences(
                providerNameCollection,
                null,
                StartDate,
                EndDateIncludingLastDay);
            
            var currentDate = DateTime.Now;
            var totalDuration = TimeSpan.Zero;
            var totalCount = 0;

            var list = new Dictionary<int, AnalysisOccurrencesWrapper>();

            foreach (var alarmOccurrence in alarmOccurrences)
            {
                TimeSpan duration;

                if (alarmOccurrence.ClearedTimeStamp.HasValue)
                {
                    duration = alarmOccurrence.ClearedTimeStamp.Value - alarmOccurrence.SetTimeStamp;
                }
                else
                {
                    duration = currentDate - alarmOccurrence.SetTimeStamp;
                }

                if (list.TryGetValue(alarmOccurrence.Alarm.Id, out var wrapper))
                {
                    wrapper.Occurrences.Add(alarmOccurrence);
                    wrapper.OccurrenceDuration += duration;
                }
                else
                {
                    list.Add(
                        alarmOccurrence.Alarm.Id,
                        new AnalysisOccurrencesWrapper(alarmOccurrence.Alarm)
                        {
                            OccurrenceDuration = duration,
                            Occurrences = { alarmOccurrence }
                        });
                }

                totalDuration += duration;
                totalCount++;
            }

            var totalDurationAsSeconds = totalDuration.TotalSeconds;

            foreach (var wrapper in list.Values)
            {
                wrapper.OccurrenceCountRatio = wrapper.OccurrenceCount / (double)totalCount * 100;
                wrapper.OccurrenceDurationRatio = wrapper.OccurrenceDuration.TotalSeconds / totalDurationAsSeconds * 100;
            }

            TotalCount = totalCount;
            TotalDuration = totalDuration;

            OccurrenceSource.AddRange(list.Values);

            // Wrap occurrences in device groups
            _deviceOccurrences.Clear();
            foreach (var wrapper in OccurrenceSource)
            {
                if (_deviceOccurrences.TryGetValue(wrapper.Alarm.ProviderName, out var deviceAlarmWrapper))
                {
                    deviceAlarmWrapper.Alarms.Add(wrapper);
                }
                else
                {
                    _deviceOccurrences.Add(
                        wrapper.Alarm.ProviderName,
                        new AnalysisDeviceOccurrencesWrapper(wrapper.Alarm.ProviderName)
                        {
                            Alarms = new List<AnalysisOccurrencesWrapper> { wrapper },
                        });
                }
            }

            OnDisplayModeChanged();
        }

        private void ApplyColorsToDevicesAndOccurrences()
        {
            var white = OxyColor.FromRgb(255, 255, 255);
            const double interpolationColorLimit = 0.75;

            foreach (var deviceOccurrence in _deviceOccurrences.Values)
            {
                if (!_deviceColors.TryGetValue(deviceOccurrence.DeviceName, out var valueColor))
                {
                    // This shouldn't happen because all of the colors for each ProviderName from the AlarmCenter.Repository were generated in the OnSetup.
                    valueColor = Colors.Magenta;
                }

                deviceOccurrence.Color = valueColor;

                var y = 0;

                switch (DisplayMode)
                {
                    case AlarmAnalysisDisplayMode.Count:
                        foreach (var occurrence in deviceOccurrence.Alarms.OrderByDescending(wrapper => wrapper.OccurrenceCount))
                        {
                            occurrence.Color = OxyColor.Interpolate(
                                ToOxyColor(valueColor),
                                white,
                                y / (double)deviceOccurrence.Alarms.Count * interpolationColorLimit);
                            occurrence.SortedIndex = y;
                            y++;
                        }
                        break;
                    case AlarmAnalysisDisplayMode.Duration:
                        foreach (var occurrence in deviceOccurrence.Alarms.OrderByDescending(wrapper => wrapper.OccurrenceDuration))
                        {
                            occurrence.Color = OxyColor.Interpolate(
                                ToOxyColor(valueColor),
                                white,
                                y / (double)deviceOccurrence.Alarms.Count * interpolationColorLimit);
                            occurrence.SortedIndex = y;
                            y++;
                        }
                        break;
                }
            }
        }

        private void OnDisplayModeChanged()
        {
            switch (DisplayMode)
            {
                case AlarmAnalysisDisplayMode.Count:
                    OccurrenceSource.Sort.SetCurrentSorting(nameof(AnalysisOccurrencesWrapper.OccurrenceCount), ListSortDirection.Descending);
                    break;
                case AlarmAnalysisDisplayMode.Duration:
                    OccurrenceSource.Sort.SetCurrentSorting(nameof(AnalysisOccurrencesWrapper.OccurrenceDuration), ListSortDirection.Descending);
                    break;
            }

            OccurrenceSource.UpdateCollection();

            UpdateIsPartOf80PercentOfImpact();
            ApplyColorsToDevicesAndOccurrences();
            FillMostImpactingDeviceProperties();
            UpdateCharts();
        }

        private void UpdateIsPartOf80PercentOfImpact()
        {
            double totalPercent = 0;
            var i = 1;

            // Workaround to get the list sorted in the same order as the DataTable ignoring filters and searching.
            foreach (var sortedAlarm in OccurrenceSource.Sort.GetAll(OccurrenceSource))
            {
                sortedAlarm.ImpactingPosition = i;
                i++;

                if (totalPercent >= 80)
                {
                    sortedAlarm.IsPartOf80PercentOfImpact = false;
                    continue;
                }

                sortedAlarm.IsPartOf80PercentOfImpact = true;

                switch (DisplayMode)
                {
                    case AlarmAnalysisDisplayMode.Count:
                        totalPercent += sortedAlarm.OccurrenceCountRatio;
                        break;
                    case AlarmAnalysisDisplayMode.Duration:
                        totalPercent += sortedAlarm.OccurrenceDurationRatio;
                        break;
                }
            }
        }

        private void FillMostImpactingDeviceProperties()
        {
            if (_deviceOccurrences.Values.Count == 0)
            {
                MostImpactingDevice = null;
                return;
            }

            switch (DisplayMode)
            {
                case AlarmAnalysisDisplayMode.Count:
                    MostImpactingDevice = _deviceOccurrences.Values.Aggregate(
                        (a, b) => a.TotalOccurrencesCount > b.TotalOccurrencesCount ? a : b
                    );
                    break;
                case AlarmAnalysisDisplayMode.Duration:
                    MostImpactingDevice = _deviceOccurrences.Values.Aggregate(
                        (a, b) => a.TotalOccurrenceDuration > b.TotalOccurrenceDuration ? a : b
                    );
                    break;
                default:
                    return;
            }
        }

        #region Charts

        private void SetupPieChartController()
        {
            var command = new DelegatePlotCommand<OxyMouseDownEventArgs>(
                (view, controller, arg) =>
                {
                    if (arg.HitTestResult?.Element is PieSeries series)
                    {
                        var index = Convert.ToInt32(arg.HitTestResult.Index);
                        var slice = series.Slices.ElementAt(index);

                        if (slice is TaggedPieSlice<AnalysisOccurrencesWrapper> taggedPieSlice)
                        {
                            SelectedAlarm = taggedPieSlice.Tag;
                        }
                    }

                    // Explicit call to the default command being overridden by this behavior.
                    PlotCommands.SnapTrack?.Execute(view, controller, arg);
                });

            var touchCommand = new DelegatePlotCommand<OxyTouchEventArgs>(
                (view, controller, arg) =>
                {
                    var pieSeries = OccurrencesPieChartModel.Series.OfType<PieSeries>().FirstOrDefault();
                    var point = pieSeries?.GetNearestPoint(arg.Position, false);

                    if (point == null || point.Index < 0) return;

                    var index = Convert.ToInt32(point.Index);
                    var slice = pieSeries.Slices.ElementAt(index);

                    if (slice is TaggedPieSlice<AnalysisOccurrencesWrapper> taggedPieSlice)
                    {
                        SelectedAlarm = taggedPieSlice.Tag;
                    }

                    PlotCommands.SnapTrackTouch?.Execute(view, controller, arg);
                });

            PieChartController.BindMouseDown(OxyMouseButton.Left, command);
            PieChartController.BindTouchDown(touchCommand);
        }

        private void UpdateCharts()
        {
            UpdateSourcePieChart();
            UpdateOccurrencesPieChart();
            UpdateAlarmEvolutionChart();
        }

        private void UpdateSourcePieChart()
        {
            SourcePieChartModel.Series.Clear();

            var seriesP1 = new PieSeries
            {
                StrokeThickness = 2.0,
                InsideLabelPosition = 0.8,
                AngleSpan = 360,
                StartAngle = 0,
                Stroke = _oxyPlotBackground,
                TickHorizontalLength = 10,
                TickRadialLength = 20,
                OutsideLabelFormat = "{1} ({2:0} %)",
                InsideLabelFormat = "{0}",
                InsideLabelColor = OxyColor.FromRgb(255, 255, 255),
                ExplodedDistance = 0.15
            };

            if (DisplayMode == AlarmAnalysisDisplayMode.Duration)
            {
                seriesP1.OutsideLabelFormat = "{1}";
            }

            foreach (var deviceAlarms in _deviceOccurrences.Values)
            {
                switch (DisplayMode)
                {
                    case AlarmAnalysisDisplayMode.Count:
                        seriesP1.Slices.Add(new TaggedPieSlice<AnalysisDeviceOccurrencesWrapper>(deviceAlarms.DeviceName, deviceAlarms.TotalOccurrencesCount)
                        {
                            Fill = ToOxyColor(deviceAlarms.Color),
                            Tag = deviceAlarms
                        });
                        break;
                    case AlarmAnalysisDisplayMode.Duration:
                        seriesP1.Slices.Add(new TaggedPieSlice<AnalysisDeviceOccurrencesWrapper>(deviceAlarms.DeviceName, deviceAlarms.TotalOccurrenceDuration.TotalSeconds)
                        {
                            Fill = ToOxyColor(deviceAlarms.Color),
                            Tag = deviceAlarms
                        });
                        break;
                }
            }

            SourcePieChartModel.Series.Add(seriesP1);
            SourcePieChartModel.InvalidatePlot(true);
        }

        private void UpdateOccurrencesPieChart()
        {
            OccurrencesPieChartModel.Series.Clear();

            var pieSeries = new PieSeries
            {
                StrokeThickness = 2.0,
                InsideLabelPosition = 0.8,
                AngleSpan = 360,
                StartAngle = 0,
                Stroke = _oxyPlotBackground,
                TickHorizontalLength = 10,
                TickRadialLength = 10,
                OutsideLabelFormat = "{1} ({2:0} %)",
                InsideLabelFormat = "{0}",
                InsideLabelColor = OxyColor.FromRgb(255, 255, 255),
                ExplodedDistance = 0.15
            };
            
            if (DisplayMode == AlarmAnalysisDisplayMode.Duration)
            {
                pieSeries.OutsideLabelFormat = "{1}";
            }

            foreach (var deviceAlarms in _deviceOccurrences.Values)
            {
                foreach (var alarm in deviceAlarms.Alarms.OrderBy(wrapper => wrapper.SortedIndex))
                {
                    switch (DisplayMode)
                    {
                        case AlarmAnalysisDisplayMode.Count:
                            pieSeries.Slices.Add(new TaggedPieSlice<AnalysisOccurrencesWrapper>(alarm.Alarm.Id.ToString(CultureInfo.InvariantCulture), alarm.OccurrenceCount)
                            {
                                Fill = alarm.Color,
                                Tag = alarm
                            });
                            break;
                        case AlarmAnalysisDisplayMode.Duration:
                            pieSeries.Slices.Add(new TaggedPieSlice<AnalysisOccurrencesWrapper>(alarm.Alarm.Id.ToString(CultureInfo.InvariantCulture), alarm.OccurrenceDuration.TotalSeconds)
                            {
                                Fill = alarm.Color,
                                Tag = alarm
                            });
                            break;
                    }
                }
            }

            OccurrencesPieChartModel.Series.Add(pieSeries);
            OccurrencesPieChartModel.InvalidatePlot(true);
        }

        private enum EvolutionRangeMode
        {
            Month,
            Day,
            Hour
        }

        private static EvolutionRangeMode GetRangeMode(TimeSpan totalDuration)
        {
            if (totalDuration > TimeSpan.FromDays(60))
            {
                return EvolutionRangeMode.Month;
            }

            if (totalDuration <= TimeSpan.FromDays(1))
            {
                return EvolutionRangeMode.Hour;
            }

            return EvolutionRangeMode.Day;
        }

        private void UpdateAlarmEvolutionChart()
        {
            AlarmEvolutionChartModel.Series.Clear();
            AlarmEvolutionChartModel.Axes.Clear();

            var categoryAxis = new CategoryAxis
            {
                Key = "y"
            };

            var deviceSeries = new Dictionary<string, BarSeries>();

            switch (EvolutionMode)
            {
                case AlarmAnalysisEvolutionMode.Source:
                    foreach (var device in _deviceOccurrences.Values)
                    {
                        if (deviceSeries.ContainsKey(device.DeviceName))
                        {
                            continue;
                        }

                        var series = new BarSeries
                        {
                            Title = device.DeviceName,
                            StrokeColor = _oxyPlotBackground,
                            StrokeThickness = 1,
                            XAxisKey = "x",
                            YAxisKey = "y",
                            LabelFormatString = DisplayMode == AlarmAnalysisDisplayMode.Count ? "{0}" : string.Empty,
                            FillColor = ToOxyColor(device.Color)
                        };

                        deviceSeries.Add(device.DeviceName, series);
                        AlarmEvolutionChartModel.Series.Add(series);
                    }
                    break;
                case AlarmAnalysisEvolutionMode.Alarm:
                    foreach (var device in _deviceOccurrences)
                    {
                        foreach (var alarmWrapper in device.Value.Alarms.OrderBy(wrapper => wrapper.SortedIndex))
                        {
                            var alarmId = alarmWrapper.Alarm.Id.ToString(CultureInfo.InvariantCulture);
                            if (deviceSeries.ContainsKey(alarmId))
                            {
                                continue;
                            }

                            var series = new BarSeries
                            {
                                Title = alarmId,
                                StrokeColor = _oxyPlotBackground,
                                StrokeThickness = 2,
                                XAxisKey = "x",
                                YAxisKey = "y",
                                FillColor = alarmWrapper.Color,
                                StackGroup = device.Key,
                                IsStacked = true,
                                LabelFormatString = DisplayMode == AlarmAnalysisDisplayMode.Count ? "{0}" : string.Empty,
                                LabelPlacement = LabelPlacement.Middle,
                                TextColor = OxyColor.FromRgb(255,255,255)
                            };

                            deviceSeries.Add(alarmId, series);
                            AlarmEvolutionChartModel.Series.Add(series);
                        }
                    }
                    break;
                default:
                    return;
            }

            var index = 0;

            var totalDuration = EndDateIncludingLastDay - StartDate;
            var rangeMode = GetRangeMode(totalDuration);
            var showOnlyMondayLabels = rangeMode == EvolutionRangeMode.Day && totalDuration > TimeSpan.FromDays(12);

            foreach (var range in GetRanges(StartDate, EndDateIncludingLastDay, rangeMode))
            {
                switch (rangeMode)
                {
                    case EvolutionRangeMode.Month:
                        categoryAxis.Labels.Add(range.Item1.ToString("MM/yyyy"));
                        break;
                    case EvolutionRangeMode.Day:
                        if (showOnlyMondayLabels && range.Item1.DayOfWeek != DayOfWeek.Monday)
                        {
                            categoryAxis.Labels.Add(string.Empty);
                        }
                        else
                        {
                            categoryAxis.Labels.Add(range.Item1.ToShortDateString());
                        }

                        break;
                    case EvolutionRangeMode.Hour:
                        categoryAxis.Labels.Add(range.Item1.ToString("HH"));
                        break;
                }
                
                switch (EvolutionMode)
                {
                    case AlarmAnalysisEvolutionMode.Source:
                        foreach (var deviceAlarms in _deviceOccurrences.Values)
                        {
                            if (!deviceSeries.TryGetValue(deviceAlarms.DeviceName, out var series))
                            {
                                continue;
                            }

                            var value = DisplayMode switch
                            {
                                AlarmAnalysisDisplayMode.Count => deviceAlarms.GetOccurrenceCount(range.Item1, range.Item2),
                                AlarmAnalysisDisplayMode.Duration => TimeSpanAxis.ToDouble(deviceAlarms.GetOccurrenceDuration(range.Item1, range.Item2)),
                                _ => 0
                            };

                            if (value > 0)
                            {
                                series.Items.Add(new BarItem { Value = value, CategoryIndex = index });
                            }
                        }

                        break;
                    case AlarmAnalysisEvolutionMode.Alarm:
                        foreach (var alarmWrapper in OccurrenceSource)
                        {
                            var alarmId = alarmWrapper.Alarm.Id.ToString(CultureInfo.InvariantCulture);
                            if (!deviceSeries.TryGetValue(alarmId, out var series))
                            {
                                continue;
                            }

                            var value = DisplayMode switch
                            {
                                AlarmAnalysisDisplayMode.Count => alarmWrapper.GetOccurrenceCount(range.Item1, range.Item2),
                                AlarmAnalysisDisplayMode.Duration => TimeSpanAxis.ToDouble(alarmWrapper.GetOccurrenceDuration(range.Item1, range.Item2)),
                                _ => 0
                            };

                            if (value > 0)
                            {
                                series.Items.Add(new BarItem { Value = value, CategoryIndex = index });
                            }
                        }

                        break;
                    default:
                        return;
                }

                index++;
            }
            
            switch (DisplayMode)
            {
                case AlarmAnalysisDisplayMode.Count:
                    var valueAxis = new LinearAxis
                    {
                        MinimumPadding = 0,
                        MaximumPadding = 0.06,
                        AbsoluteMinimum = 0,
                        MinimumMinorStep = 100,
                        MinimumMajorStep = 1,
                        Key = "x"
                    };
                    AlarmEvolutionChartModel.Axes.Add(valueAxis);
                    break;
                case AlarmAnalysisDisplayMode.Duration:
                    var timeSpanAxis = new TimeSpanAxis
                    {
                        AbsoluteMinimum = 0,
                        Key = "x"
                    };
                    AlarmEvolutionChartModel.Axes.Add(timeSpanAxis);
                    break;
                default:
                    return;
            }

            AlarmEvolutionChartModel.Axes.Add(categoryAxis);
            AlarmEvolutionChartModel.InvalidatePlot(true);
        }

        private void UpdatePieChartsSelection()
        {
            UpdateOccurrencesPieChartSelection();
            UpdateSourcePieChartSelection();
        }

        private void UpdateOccurrencesPieChartSelection()
        {
            var series = OccurrencesPieChartModel.Series.OfType<PieSeries>().SingleOrDefault();
            if (series == null) return;
            
            foreach (var taggedPieSlice in series.Slices.OfType<TaggedPieSlice<AnalysisOccurrencesWrapper>>())
            {
                if (SelectedAlarm == null)
                {
                    // Default display
                    taggedPieSlice.Fill = taggedPieSlice.Tag.Color;
                    taggedPieSlice.IsExploded = false;
                }
                else if (ReferenceEquals(taggedPieSlice.Tag, SelectedAlarm))
                {
                    taggedPieSlice.Fill = taggedPieSlice.Tag.Color;
                    taggedPieSlice.IsExploded = true;
                }
                else
                {
                    var oxyColor = taggedPieSlice.Tag.Color;
                    taggedPieSlice.Fill = OxyColor.FromArgb(150, oxyColor.R, oxyColor.G, oxyColor.B);
                    taggedPieSlice.IsExploded = false;
                }
            }

            OccurrencesPieChartModel.InvalidatePlot(false);
        }

        private void UpdateSourcePieChartSelection()
        {
            var series = SourcePieChartModel.Series.OfType<PieSeries>().SingleOrDefault();
            if (series == null) return;

            foreach (var taggedPieSlice in series.Slices.OfType<TaggedPieSlice<AnalysisDeviceOccurrencesWrapper>>())
            {
                if (SelectedAlarm == null)
                {
                    // Default display
                    taggedPieSlice.Fill = ToOxyColor(taggedPieSlice.Tag.Color);
                    taggedPieSlice.IsExploded = false;
                }
                else if (SelectedAlarm.Alarm.ProviderName ==  taggedPieSlice.Tag.DeviceName)
                {
                    taggedPieSlice.Fill = ToOxyColor(taggedPieSlice.Tag.Color);
                    taggedPieSlice.IsExploded = true;
                }
                else
                {
                    var oxyColor = taggedPieSlice.Tag.Color;
                    taggedPieSlice.Fill = OxyColor.FromArgb(150, oxyColor.R, oxyColor.G, oxyColor.B);
                    taggedPieSlice.IsExploded = false;
                }
            }

            SourcePieChartModel.InvalidatePlot(false);
        }

        #endregion

        #region Helpers

        private static IEnumerable<Tuple<DateTime, DateTime>> GetRanges(DateTime from, DateTime thru, EvolutionRangeMode evolutionRangeMode)
        {
            switch (evolutionRangeMode)
            {
                case EvolutionRangeMode.Month:
                    return EachMonth(from, thru);
                case EvolutionRangeMode.Day:
                    return EachDay(from, thru);
                case EvolutionRangeMode.Hour:
                    return EachHour(from, thru);
                default:
                    throw new ArgumentOutOfRangeException(nameof(evolutionRangeMode), evolutionRangeMode, null);
            }
        }

        private static IEnumerable<Tuple<DateTime, DateTime>> EachHour(DateTime from, DateTime thru)
        {
            for (var hour = from.Date; hour.Date < thru.Date; hour = hour.AddHours(1))
            {
                yield return new Tuple<DateTime, DateTime>(hour, hour.AddHours(1));
            }
        }

        private static IEnumerable<Tuple<DateTime, DateTime>> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date < thru.Date; day = day.AddDays(1))
            {
                yield return new Tuple<DateTime, DateTime>(day, day.AddDays(1));
            }
        }

        private static IEnumerable<Tuple<DateTime, DateTime>> EachMonth(DateTime from, DateTime thru)
        {
            List<Tuple<DateTime, DateTime>> months = new List<Tuple<DateTime, DateTime>>();
            var currentStart = from;
            
            while (true)
            {
                DateTime firstDayOfNextMonth = new DateTime(currentStart.AddMonths(1).Year, currentStart.AddMonths(1).Month, 1);
                if (firstDayOfNextMonth >= thru)
                {
                    months.Add(new Tuple<DateTime, DateTime>(currentStart, thru));
                    return months;
                }

                months.Add(new Tuple<DateTime, DateTime>(currentStart, firstDayOfNextMonth));
                currentStart = firstDayOfNextMonth;
            }
        }

        private static OxyColor ToOxyColor(Color color) => OxyColor.FromArgb(color.A, color.R, color.G, color.B);

        /// <summary>
        /// Get the day that started this week.
        /// </summary>
        /// <param name="startDayOfWeek">Witch day start a week (usually monday or sunday).</param>
        /// <returns>The date of the day that started this week.</returns>
        private static DateTime GetStartOfThisWeek(DayOfWeek startDayOfWeek)
        {
            var diff = (7 + (DateTime.Today.DayOfWeek - startDayOfWeek)) % 7;
            return DateTime.Today.AddDays(-1 * diff).Date;
        }

        private static DateTime GetStartOfLastWeek(DayOfWeek startDayOfWeek)
        {
            var diff = (7 + (DateTime.Today.DayOfWeek - startDayOfWeek)) % 7;
            return DateTime.Today.AddDays((-1 * diff) - 7).Date;
        }

        private static DateTime GetStartOfThisMonth() => new(DateTime.Today.Year, DateTime.Today.Month, 1);

        private static DateTime GetStartOfLastMonth() => new(DateTime.Today.Year, DateTime.Today.Month - 1, 1);

        private static DateTime GetLastDayOfLastMonth()
            => new(
                DateTime.Today.Year,
                DateTime.Today.Month - 1,
                DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month - 1));

        private static DateTime GetStartOYear() => new(DateTime.Today.Year, 1, 1);

        #endregion

        #endregion

        #region Overrides

        public override void OnShow()
        {
            base.OnShow();

            if (_firstShow)
            {
                _firstShow = false;
                SelectedPeriod = Period.Last7Days;
            }
            else
            {
                UpdateSource();
            }
        }

        public override void OnSetup()
        {
            base.OnSetup();
            BuildDeviceColorDictionary();
            SourceFilter.UpdatePossibleValues();
            SourceFilter.SelectedItems.CollectionChanged += FilteredSourcesChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SourceFilter.SelectedItems.CollectionChanged -= FilteredSourcesChanged;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
