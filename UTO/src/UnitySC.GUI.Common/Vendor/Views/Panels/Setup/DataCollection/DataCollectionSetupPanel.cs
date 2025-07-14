using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.DataMonitoring;
using Agileo.DataMonitoring.Configuration;
using Agileo.DataMonitoring.DataWriter.Chart;
using Agileo.DataMonitoring.Events;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using Castle.Core.Internal;

using UnitySC.GUI.Common.Vendor.Configuration.DataCollection;
using UnitySC.GUI.Common.Vendor.Helpers.Colors;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.DataCollection
{
    /// <summary>
    /// Setup panel for the <see cref="DataCollectionPlanLibrarian"/>.
    /// </summary>
    public class DataCollectionSetupPanel : SetupPanel<DataMonitoringConfiguration<DCPConfiguration>>
    {
        #region Fields

        private bool _noConfigExistsOnDisk;

        private bool _areAllMinMaxRangesValid = true;

        private UserMessage _invalidRangeMessage;

        #endregion Fields

        #region Constructors

        static DataCollectionSetupPanel()
        {
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(DataCollectionSetupPanelResources)));
        }

        /// <summary>
        /// Default constructor only used by view in design instance.
        /// </summary>
        public DataCollectionSetupPanel()
            : this("Design Time Constructor", new DataCollectionPlanLibrarian())
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        public DataCollectionSetupPanel(
            string relativeId,
            DataCollectionPlanLibrarian dataCollectionPlanLibrarian,
            IIcon icon = null)
            : base(relativeId, icon)
        {
            if (IsInDesignMode)
            {
                return;
            }

            // DataCollectionPlanLibrarian configuration cannot be null to use this setup panel
            if (dataCollectionPlanLibrarian.Configuration == null)
            {
                throw new ArgumentException(
                    $@"DataCollectionPlanLibrarian.Configuration cannot be null if you want to use {nameof(DataCollectionSetupPanel)}. Please use Setup<T>(ITracer, configurationFilePath) in App.SetupAdditionalComponents() method.",
                    nameof(dataCollectionPlanLibrarian));
            }

            // Initialize parameters
            DataCollectionPlanLibrarian = dataCollectionPlanLibrarian;

            Sort.SetCurrentSorting(nameof(DataCollectionPlan.Name), ListSortDirection.Descending);
            DataCollectionPlans.Reset(DataCollectionPlanLibrarian.Plans);
        }

        #endregion Constructors

        #region CollectionChanged Event Handlers

        private void DataCollectionPlanLibrarian_DataCollectionPlanAdded(
            object sender,
            DataCollectionPlanAddedEventArgs e)
        {
            var dcp = e.DataCollectionPlan;
            if (dcp == null)
            {
                return;
            }

            if (!dcp.IsDynamic)
            {
                DataCollectionPlans.Add(dcp);
            }
        }

        private void DataCollectionPlanLibrarian_DataCollectionPlanRemoved(
            object sender,
            DataCollectionPlanRemovedEventArgs e)
        {
            var dcp = e.DataCollectionPlan;
            if (dcp == null)
            {
                return;
            }

            if (!dcp.IsDynamic)
            {
                DataCollectionPlans.Remove(dcp);
            }
        }

        #endregion CollectionChanged Event Handlers

        #region Properties

        /// <summary>
        /// Get the <see cref="DataCollectionPlanLibrarian"/> containing all <see cref="DataCollectionPlan"/> to be setup.
        /// </summary>
        public DataCollectionPlanLibrarian DataCollectionPlanLibrarian { get; }

        /// <summary>
        /// Get the <see cref="DataCollectionPlan"/> <see cref="DataTableSource{T}"/> associated with the <see cref="DataCollectionPlanLibrarian"/>.
        /// </summary>
        public DataTableSource<DataCollectionPlan> DataCollectionPlans { get; } = new();

        private DataCollectionPlan _selectedDcp;

        /// <summary>
        /// Get or set the selected <see cref="DataCollectionPlan"/>.
        /// </summary>
        public DataCollectionPlan SelectedDcp
        {
            get => _selectedDcp;
            set
            {
                if (!SetAndRaiseIfChanged(ref _selectedDcp, value) || _selectedDcp == null)
                {
                    return;
                }

                var selectedDcpConfig = SelectedDcpConfiguration;
                SelectedDcpAxesMinMaxLog = GenerateAxesMinMaxLogFromConfig(selectedDcpConfig.AxesMinMaxLog);
                SelectedDcpAxesColors = GenerateAxesColorsFromConfig(selectedDcpConfig.SeriesColors);
            }
        }

        public SortEngine<DataCollectionPlan> Sort { get; } = new();

        public DCPConfiguration SelectedDcpConfiguration
        {
            get
            {
                if (SelectedDcp == null)
                {
                    return null;
                }

                var dcpConfig =
                    ModifiedConfig?.DataCollectionPlans.Find(
                        dcp => dcp.RelatedDataCollectionPlanName == SelectedDcp.Name);

                if (dcpConfig == null)
                {
                    dcpConfig = new DCPConfiguration
                    {
                        RelatedDataCollectionPlanName = SelectedDcp.Name,
                        AxesMinMaxLog = ReadOrGenerateAuto(),
                        SeriesColors = ReadOrGenerateColors()
                    };

                    _noConfigExistsOnDisk = true;
                    return dcpConfig;
                }

                if (dcpConfig.SeriesColors.Count == 0)
                {
                    dcpConfig.SeriesColors = ReadOrGenerateColors();
                }

                if (dcpConfig.AxesMinMaxLog.Count == 0)
                {
                    dcpConfig.AxesMinMaxLog = ReadOrGenerateAuto();
                }

                return dcpConfig;
            }
        }

        private ObservableCollection<AxisMinMaxLog> _selectedDcpAxesMinMaxLog;

        public ObservableCollection<AxisMinMaxLog> SelectedDcpAxesMinMaxLog
        {
            get => _selectedDcpAxesMinMaxLog;
            set => SetAndRaiseIfChanged(ref _selectedDcpAxesMinMaxLog, value);
        }

        private ObservableCollection<AxisColor> _selectedDcpAxesColors;

        public ObservableCollection<AxisColor> SelectedDcpAxesColors
        {
            get => _selectedDcpAxesColors;
            set {
                if (SetAndRaiseIfChanged(ref _selectedDcpAxesColors, value))
                {
                    if (value.IsNullOrEmpty())
                    {
                        return;
                    }

                    //Copy the current colors into a new array, used for the saving states
                    var axisColors = Array.Empty<AxisColor>();
                    Array.Resize(ref axisColors, value.Count);
                    value.CopyTo(axisColors, 0);
                    _currentAxisColors = new Collection<AxisColor>(axisColors);
                }
            }
        }

        private Collection<AxisColor> _currentAxisColors;

        #endregion Properties

        #region Methods

        private void DisplayRelatedConfiguration()
        {
            if (SelectedDcpConfiguration == null)
            {
                return;
            }

            SelectedDcpAxesMinMaxLog = GenerateAxesMinMaxLogFromConfig(SelectedDcpConfiguration.AxesMinMaxLog);
            SelectedDcpAxesColors = GenerateAxesColorsFromConfig(SelectedDcpConfiguration.SeriesColors);
        }

        private static Color StringToColor(string color)
            => (Color)(ColorConverter.ConvertFromString(color) ?? Colors.Black);

        private void AxisColor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is not AxisColor axisColor)
            {
                return;
            }

            var relatedAxisConfig =
                SelectedDcpConfiguration.SeriesColors.Find(axis => axis.SourceName == axisColor.AxisName);
            if (relatedAxisConfig == null)
            {
                return;
            }

            relatedAxisConfig.SourceColor = axisColor.Color.ToString();
        }

        private void AxisMinMaxLog_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var axisMinMaxLog = sender as AxisMinMaxLog;
            if (axisMinMaxLog == null)
            {
                return;
            }

            var relatedAxisConfig =
                SelectedDcpConfiguration.AxesMinMaxLog.FirstOrDefault(
                    axis => axis.UnitAbbreviation == axisMinMaxLog.UnitAbbreviation);
            if (relatedAxisConfig == null)
            {
                return;
            }

            // Check for validity if the changed property was the range validity
            if (!axisMinMaxLog.IsRangeValid)
            {
                // Add a new invalid range UserMessage only if it is not already present.
                if (_invalidRangeMessage == null)
                {
                    _invalidRangeMessage =
                        new UserMessage(MessageLevel.Warning, nameof(DataCollectionSetupPanelResources.INVALID_RANGE))
                        {
                            CanUserCloseMessage = false, SecondsDuration = int.MaxValue
                        };
                    Messages.Show(_invalidRangeMessage);
                }

                _areAllMinMaxRangesValid = false;
            }

            // All ranges become valid if all issues are solved.
            else if (!_areAllMinMaxRangesValid)
            {
                CheckRangesValidity();
            }

            relatedAxisConfig.Min = axisMinMaxLog.Min;
            relatedAxisConfig.Max = axisMinMaxLog.Max;
            relatedAxisConfig.IsLogarithmic = axisMinMaxLog.IsLogarithmic;
        }

        private ObservableCollection<AxisColor> GenerateAxesColorsFromConfig(
            List<SourceColorConfiguration> axesColorConfig)
        {
            var list = new ObservableCollection<AxisColor>();
            axesColorConfig.ForEach(
                axisColorConfig =>
                {
                    var axisColor = new AxisColor(
                        axisColorConfig.SourceName,
                        axisColorConfig.QuantityType,
                        axisColorConfig.QuantityAbbreviation,
                        StringToColor(axisColorConfig.SourceColor));
                    axisColor.PropertyChanged += AxisColor_PropertyChanged;
                    list.Add(axisColor);
                });

            return list;
        }

        private ObservableCollection<AxisMinMaxLog> GenerateAxesMinMaxLogFromConfig(
            List<AxisMinMaxLogConfiguration> axesMinMaxLog)
        {
            var list = new ObservableCollection<AxisMinMaxLog>();
            axesMinMaxLog.ForEach(
                axisMinMaxLogConfig =>
                {
                    var axisMinMaxLog = new AxisMinMaxLog(
                        axisMinMaxLogConfig.UnitName,
                        axisMinMaxLogConfig.UnitAbbreviation,
                        axisMinMaxLogConfig.Min,
                        axisMinMaxLogConfig.Max,
                        axisMinMaxLogConfig.IsLogarithmic);
                    axisMinMaxLog.PropertyChanged += AxisMinMaxLog_PropertyChanged;
                    list.Add(axisMinMaxLog);
                });

            return list;
        }

        private List<SourceColorConfiguration> ReadOrGenerateColors(
            List<SourceColorConfiguration> alreadyGeneratedColors = null)
        {
            var colorList = alreadyGeneratedColors ?? new List<SourceColorConfiguration>();

            // Generate values if no chartDataWriter is linked to the SelectedDataCollectionPlan, or read them otherwise.
            var generate = !SelectedDcp.DataWriters.Any(writer => writer is ChartDataWriter);

            if (generate)
            {
                // Do not generate a color for the time data source.
                var colorConfigurations = GenerateColorsForSources(
                    SelectedDcp.DataSources.Except(new[] { SelectedDcp.DataSources.First() })
                        .Select(source => source.Information)
                        .ToList());
                foreach (var sourceColorConfiguration in colorConfigurations)
                {
                    if (!colorList.Any(
                            configuration => configuration.SourceName.Equals(sourceColorConfiguration.SourceName)))
                    {
                        colorList.Add(sourceColorConfiguration);
                    }
                }
            }
            else
            {
                foreach (var dataWriter in SelectedDcp.DataWriters.Where(writer => writer is ChartDataWriter))
                {
                    var chartDataWriter = (ChartDataWriter)dataWriter;

                    // Read colors for each source considered by each ChartDataWriter
                    foreach (var sourceName in chartDataWriter.ColorDictionary.Keys)
                    {
                        if (colorList.Any(source => source.SourceName.Equals(sourceName)))
                        {
                            continue;
                        }

                        var sourceInformation =
                            chartDataWriter.DataSources.First(information => information.SourceName.Equals(sourceName));
                        colorList.Add(
                            new SourceColorConfiguration
                            {
                                SourceName = sourceName,
                                QuantityType = sourceInformation.SourceUnitName,
                                QuantityAbbreviation = sourceInformation.SourceUnitAbbreviation,
                                SourceColor = chartDataWriter.ColorDictionary[sourceName].ToString()
                            });
                    }
                }

                // If the SelectedDataCollectionPlan contains others DataSources, generate for them
                var otherSourceInformation = SelectedDcp.DataSources
                    .Where(
                        source => !colorList.Any(
                            sourceConfig => sourceConfig.SourceName.Equals(source.Information.SourceName)))
                    .Select(source => source.Information);
                colorList.AddRange(GenerateColorsForSources(otherSourceInformation.ToList()));
            }

            return colorList;
        }

        private static List<SourceColorConfiguration> GenerateColorsForSources(
            IReadOnlyCollection<SourceInformation> sourceInformation)
        {
            var colorList = new List<SourceColorConfiguration>();
            var background = Colors.Black;
            App.Instance.Dispatcher.Invoke(
                () =>
                {
                    background = UIComponents.XamlResources.Shared.Brushes.BackgroundBrush.Color;
                });

            var colors = ColorScaleGenerator.GenerateColorsOnBackground(
                sourceInformation.Count,
                background.R,
                background.G,
                background.B);
            var cpt = 0;

            foreach (var src in sourceInformation)
            {
                if (!colorList.Exists(sourceColor => sourceColor.SourceName.Equals(src.SourceName)))
                {
                    colorList.Add(
                        new SourceColorConfiguration
                        {
                            SourceName = src.SourceName,
                            QuantityType = src.SourceUnitName,
                            QuantityAbbreviation = src.SourceUnitAbbreviation,
                            SourceColor = colors[cpt].ToString()
                        });
                }

                ++cpt;
            }

            return colorList;
        }

        private List<AxisMinMaxLogConfiguration> ReadOrGenerateAuto(
            List<AxisMinMaxLogConfiguration> axesMinMaxLog = null)
        {
            var autoMinMaxLogList = axesMinMaxLog ?? new List<AxisMinMaxLogConfiguration>();

            // Generate values if no chartDataWriter is linked to the SelectedDataCollectionPlan, or read them otherwise.
            var generate = !SelectedDcp.DataWriters.Any(writer => writer is ChartDataWriter);

            if (generate)
            {
                var axisConfigurations =
                    GenerateValuesForAxis(SelectedDcp.DataSources.Select(source => source.Information).ToList());
                foreach (var axisMinMaxLogConfiguration in axisConfigurations)
                {
                    if (!autoMinMaxLogList.Any(
                            configuration
                                => configuration.UnitAbbreviation.Equals(axisMinMaxLogConfiguration.UnitAbbreviation)))
                    {
                        autoMinMaxLogList.Add(axisMinMaxLogConfiguration);
                    }
                }
            }
            else
            {
                foreach (var dataWriter in SelectedDcp.DataWriters.Where(writer => writer is ChartDataWriter))
                {
                    var chartDataWriter = (ChartDataWriter)dataWriter;

                    // Read axis min, max and isLogarithmic attributes for axes associated to the sources
                    foreach (var unitAbbreviation in chartDataWriter.UnitMinMaxDictionary.Keys)
                    {
                        if (autoMinMaxLogList.Any(axisConfig => axisConfig.UnitAbbreviation.Equals(unitAbbreviation)))
                        {
                            continue;
                        }

                        var sourceInformation = chartDataWriter.DataSources.First(
                            information => information.SourceUnitAbbreviation.Equals(unitAbbreviation));
                        autoMinMaxLogList.Add(
                            new AxisMinMaxLogConfiguration
                            {
                                UnitName = sourceInformation.SourceUnitName,
                                UnitAbbreviation = sourceInformation.SourceUnitAbbreviation,
                                Min = chartDataWriter.UnitMinMaxDictionary[unitAbbreviation].Item1,
                                Max = chartDataWriter.UnitMinMaxDictionary[unitAbbreviation].Item2,
                                IsLogarithmic = chartDataWriter.ScaleKind[unitAbbreviation]
                            });
                    }

                    // If the SelectedDataCollectionPlan contains others DataSources, generate for them
                    var allSources = SelectedDcp.DataSources.Select(source => source.Information);
                    var otherSourceType = allSources.Where(
                        sourceInformation => !autoMinMaxLogList.Any(
                            configuration
                                => configuration.UnitAbbreviation.Equals(sourceInformation.SourceUnitAbbreviation)));
                    autoMinMaxLogList.AddRange(GenerateValuesForAxis(otherSourceType.ToList()));
                }
            }

            return autoMinMaxLogList;
        }

        private static List<AxisMinMaxLogConfiguration> GenerateValuesForAxis(
            IReadOnlyCollection<SourceInformation> sourceInformation)
        {
            var autoMinMaxLogList = new List<AxisMinMaxLogConfiguration>();

            foreach (var src in sourceInformation)
            {
                if (!autoMinMaxLogList.Exists(axis => axis.UnitAbbreviation.Equals(src.SourceUnitAbbreviation)))
                {
                    // double.NaN represents Auto value on chart
                    autoMinMaxLogList.Add(
                        new AxisMinMaxLogConfiguration
                        {
                            UnitAbbreviation = src.SourceUnitAbbreviation,
                            UnitName = src.SourceUnitName,
                            Min = double.NaN,
                            Max = double.NaN,
                            IsLogarithmic = false
                        });
                }
            }

            return autoMinMaxLogList;
        }

        private void CheckRangesValidity()
        {
            var isValid = SelectedDcpAxesMinMaxLog.All(axisMinMax => axisMinMax.IsRangeValid);

            if (isValid && _invalidRangeMessage != null)
            {
                Messages.Hide(_invalidRangeMessage);
                _invalidRangeMessage = null;
            }

            _areAllMinMaxRangesValid = isValid;
        }

        private void ApplyConfigurationsToPlans()
        {
            if (CurrentConfig == null)
            {
                return;
            }

            foreach (var dcpConfig in CurrentConfig.DataCollectionPlans)
            {
                var relatedDcp = DataCollectionPlanLibrarian.Plans.FirstOrDefault(
                    dcp => dcp.Name.Equals(dcpConfig.RelatedDataCollectionPlanName, StringComparison.Ordinal));

                if (relatedDcp == null)
                {
                    continue;
                }

                ApplyToPlan(relatedDcp, dcpConfig);
            }
        }

        private void ApplyCurrentConfigurationToPlans()
        {
            if (SelectedDcp == null || CurrentConfig == null)
            {
                return;
            }

            var relatedConfig = CurrentConfig.DataCollectionPlans.Find(
                dcpConfig => dcpConfig.RelatedDataCollectionPlanName == SelectedDcp.Name);

            ApplyToPlan(SelectedDcp, relatedConfig);
        }

        private static void ApplyToPlan(DataCollectionPlan plan, DCPConfiguration relatedPlanConfiguration)
        {
            if (relatedPlanConfiguration == null)
            {
                return;
            }

            ApplyToWriters(plan, relatedPlanConfiguration);
        }

        private static void ApplyToWriters(DataCollectionPlan plan, DCPConfiguration relatedPlanConfiguration)
        {
            if (relatedPlanConfiguration == null)
            {
                return;
            }

            ApplyToChartDataWriters(plan.DataWriters.OfType<ChartDataWriter>(), relatedPlanConfiguration);
        }

        private static void ApplyToChartDataWriters(
            IEnumerable<ChartDataWriter> chartWriters,
            DCPConfiguration relatedPlanConfiguration)
        {
            if (relatedPlanConfiguration == null)
            {
                return;
            }

            foreach (var chartDataWriter in chartWriters)
            {
                // Apply color configuration to axes
                ApplyColorConfigurationToCharts(relatedPlanConfiguration, chartDataWriter);

                // Apply min, max and logarithmic configuration to axes
                ApplyUnitMinMaxLogConfigurationToCharts(relatedPlanConfiguration, chartDataWriter);
            }
        }

        private static void ApplyColorConfigurationToCharts(
            DCPConfiguration relatedPlanConfiguration,
            ChartDataWriter chartDataWriter)
            => relatedPlanConfiguration.SeriesColors.ForEach(
                axisColor =>
                {
                    chartDataWriter.ChangeCurveColor(axisColor.SourceName, StringToColor(axisColor.SourceColor));
                });

        private static void ApplyUnitMinMaxLogConfigurationToCharts(
            DCPConfiguration relatedPlanConfiguration,
            ChartDataWriter chartDataWriter)
            => relatedPlanConfiguration.AxesMinMaxLog.ForEach(
                axisMinMaxLogConfig =>
                {
                    chartDataWriter.ChangeYAxisKind(
                        axisMinMaxLogConfig.UnitName,
                        axisMinMaxLogConfig.UnitAbbreviation,
                        axisMinMaxLogConfig.IsLogarithmic);
                    chartDataWriter.UpdateAxisMinMax(
                        axisMinMaxLogConfig.UnitAbbreviation,
                        axisMinMaxLogConfig.Min,
                        axisMinMaxLogConfig.Max);
                });

        #endregion Methods

        #region Override

        public override void OnSetup()
        {
            base.OnSetup();
            DataCollectionPlanLibrarian.DataCollectionPlanAdded += DataCollectionPlanLibrarian_DataCollectionPlanAdded;
            DataCollectionPlanLibrarian.DataCollectionPlanRemoved +=
                DataCollectionPlanLibrarian_DataCollectionPlanRemoved;

            ApplyConfigurationsToPlans();
        }

        protected override IConfigManager GetConfigManager() => DataCollectionPlanLibrarian.Configuration;

        public override bool SaveCommandCanExecute()
            => _areAllMinMaxRangesValid && (_noConfigExistsOnDisk || base.SaveCommandCanExecute());

        protected override void UndoChanges()
        {
            base.UndoChanges();
            DisplayRelatedConfiguration();
        }

        protected override bool ConfigurationEqualsCurrent()
        {
            return ObjectAreEquals(ModifiedConfig, CurrentConfig) || !SelectedDcpAxesColors.All(_currentAxisColors.Contains);
        }

        protected override void SaveConfig()
        {
            if (_noConfigExistsOnDisk)
            {
                ModifiedConfig?.DataCollectionPlans.Add(SelectedDcpConfiguration);
                _noConfigExistsOnDisk = false; // now, one DCPConfig exists on disk
            }

            // apply and save configuration
            ConfigManager.Apply(true);

            ApplyCurrentConfigurationToPlans();
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (IsInDesignMode)
                {
                    foreach (var dataCollectionPlan in DataCollectionPlanLibrarian.Plans)
                    {
                        dataCollectionPlan.Dispose();
                    }
                }

                // Unsubscribe to dataSourceConfigurationView modifications
                SelectedDcpAxesColors?.ToList()
                    .ForEach(axisColorVm => axisColorVm.PropertyChanged -= AxisColor_PropertyChanged);

                // Unsubscribe to axesMinMaxConfigurationView modifications
                SelectedDcpAxesMinMaxLog?.ToList()
                    .ForEach(axisMinMaxVm => axisMinMaxVm.PropertyChanged -= AxisMinMaxLog_PropertyChanged);

                DataCollectionPlanLibrarian.DataCollectionPlanAdded -=
                    DataCollectionPlanLibrarian_DataCollectionPlanAdded;
                DataCollectionPlanLibrarian.DataCollectionPlanRemoved -=
                    DataCollectionPlanLibrarian_DataCollectionPlanRemoved;
            }

            base.Dispose(disposing);

            _disposed = true;
        }

        #endregion IDisposable
    }
}
