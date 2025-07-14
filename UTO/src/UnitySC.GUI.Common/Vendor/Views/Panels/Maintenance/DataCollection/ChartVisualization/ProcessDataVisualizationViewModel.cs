using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Agileo.Common.Localization;
using Agileo.DataMonitoring;
using Agileo.DataMonitoring.DataWriter.Chart;
using Agileo.DataMonitoring.Events;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.LineCharts.Abstractions.Enums;
using Agileo.LineCharts.Abstractions.Model;
using Agileo.LineCharts.OxyPlot.Views;

using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Extensions;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization.Core;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization.Popups;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization
{
    /// <summary>
    /// ViewModel that can be used everywhere to display a chart on any set of <see cref="CollectedData"/>.
    /// It provides tools for chart manipulation and data monitoring.
    /// It can also be used to display all charts of the <see cref="DataCollectionPlanLibrarian"/>.
    /// </summary>
    public class ProcessDataVisualizationViewModel : Notifier, IDisposable
    {
        #region Fields

        private readonly Dictionary<IChart, List<SourceInformation>> _selectedSourcesInfoByChart = new();

        private bool _stopShowingMaxSrcReached;

        #endregion Fields

        #region Constructors

        static ProcessDataVisualizationViewModel()
        {
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(ProcessDataVisualizationResources)));
        }

        /// <summary>
        /// Default constructor only used by view in design instance.
        /// </summary>
        public ProcessDataVisualizationViewModel()
            : this(new DataCollectionPlanLibrarian())
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        /// <summary>
        /// Build the <see cref="ProcessDataVisualizationViewModel"/> from a list of information about the data sources and the time-stamped data.
        /// </summary>
        /// <param name="seriesInformation">A list of information about the data sources.</param>
        /// <param name="collectedData">The time-stamped data.</param>
        public ProcessDataVisualizationViewModel(
            List<SourceInformation> seriesInformation,
            List<CollectedData> collectedData)
        {
            IsFromData = true;

            // Initialize Chart
            var chartModel = new ChartDataWriter("Chart") { XAxisType = AxisType.Automatic };
            chartModel.Initialize(seriesInformation);
            chartModel.Open();
            chartModel.Write(collectedData);
            chartModel.Close();
            SelectedChartModel = chartModel;
            SelectedChart = SelectedChartModel.GetChartViewModel(this);
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="ProcessDataVisualizationViewModel"/> class from a <see cref="DataCollectionPlanLibrarian"/>.
        /// Charts will be available for each <see cref="ChartDataWriter"/> of each <see cref="DataCollectionPlan"/> contained in the given <see cref="Agileo.DataMonitoring.DataCollectionPlanLibrarian"/>,
        /// even if they will be added later.
        /// </summary>
        /// <param name="dcpLibrarian">The librarian containing all <see cref="DataCollectionPlan"/> of the system.</param>
        public ProcessDataVisualizationViewModel(DataCollectionPlanLibrarian dcpLibrarian)
        {
            if (dcpLibrarian == null)
            {
                throw new ArgumentNullException(nameof(dcpLibrarian));
            }

            DataCollectionPlanLibrarian = dcpLibrarian;

            foreach (var dataCollectionPlan in DataCollectionPlanLibrarian.Plans)
            {
                var dataCollectionPlanVm = new DataCollectionPlanChartsNode(dataCollectionPlan);
                _dataCollectionPlanVms.Add(dataCollectionPlanVm);
                dataCollectionPlanVm.PropertyChanged += DataCollectionPlanVmOnPropertyChanged;

                // Consider the DataCollectionPlan only if it contains at least one chart
                if (dataCollectionPlanVm.ContainCharts)
                {
                    DataCollectionPlansWithCharts.Add(dataCollectionPlanVm);
                }
            }

            DataCollectionPlanLibrarian.DataCollectionPlanAdded += DataCollectionPlanLibrarian_DataCollectionPlanAdded;
            DataCollectionPlanLibrarian.DataCollectionPlanRemoved +=
                DataCollectionPlanLibrarian_DataCollectionPlanRemoved;
        }

        private void DataCollectionPlanVmOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals(nameof(DataCollectionPlanChartsNode.ContainCharts)))
            {
                return;
            }

            var dcpVm = sender as DataCollectionPlanChartsNode;

            if (dcpVm == null)
            {
                return;
            }

            if (dcpVm.ContainCharts)
            {
                DataCollectionPlansWithCharts.Add(dcpVm);
            }
            else
            {
                DataCollectionPlansWithCharts.Remove(dcpVm);
            }
        }

        #endregion Constructors

        #region Properties

        private readonly ICollection<DataCollectionPlanChartsNode> _dataCollectionPlanVms =
            new List<DataCollectionPlanChartsNode>();

        public ObservableCollection<DataCollectionPlanChartsNode> DataCollectionPlansWithCharts { get; } = new();

        /// <summary>
        /// Contain all sources classed following their unit abbreviation.
        /// Contain a tree where:
        ///     the first level elements represent the unit abbreviation
        ///     the second level elements represent the source it self
        /// </summary>
        public ObservableCollection<UnitTreeNode> TreeViewSourceElements { get; } = new();

        /// <summary>
        /// Allow to display <see cref="UserMessage"/> into the chart view.
        /// </summary>
        public UserMessageDisplayer Messages { get; } = new();

        /// <summary>
        /// Allow to display <see cref="Popup"/> into the chart view.
        /// </summary>
        public PopupDisplayer Popups { get; } = new();

        private ObservableCollection<DataSourceViewModel> _selectedDataSourcesInformation = new();

        /// <summary>
        /// Contain the <see cref="DataSourceViewModel"/> displayed on the <see cref="SelectedChart"/>.
        /// </summary>
        public ReadOnlyObservableCollection<DataSourceViewModel> SelectedDataSources
            => new(_selectedDataSourcesInformation);

        private ChartDataWriter _selectedChartModel;

        /// <summary>
        /// Get or set the <see cref="SelectedChartModel"/>.
        /// </summary>
        /// <remarks>
        /// Before to set it, make sure the wanted chart is available.
        /// </remarks>
        /// <exception cref="InvalidOperationException">on the set, when the <see cref="SelectedDcp"/> does not contain the given chart.</exception>
        public ChartDataWriter SelectedChartModel
        {
            get => _selectedChartModel;
            set
            {
                if (_selectedChartModel == value)
                {
                    return;
                }

                if (value == null)
                {
                    _selectedChartModel = null;
                    SelectedChart = null;
                }
                else
                {
                    if (!IsFromData)
                    {
                        CheckDcpContainsNewChartModel(value);
                    }

                    _selectedChartModel = value;

                    if (value.IsInitialized)
                    {
                        SelectedChart = value.GetChartViewModel(this);
                    }

                    Application.Current.Dispatcher?.Invoke(BuildTreeViewElementCollection);
                    SelectOldOrDefaultDataSources();
                }

                OnPropertyChanged();
            }
        }

        private IChart _selectedChart;

        /// <summary>
        /// Get or set the chart representation of the <see cref="DataCollectionPlan"/> to be visualized.
        /// </summary>
        public IChart SelectedChart
        {
            get => _selectedChart;
            set
            {
                if (_selectedChart == value)
                {
                    return;
                }

                // Save which DataSources were selected in the previous chart
                if (_selectedChart != null)
                {
                    if (_selectedSourcesInfoByChart.ContainsKey(_selectedChart))
                    {
                        _selectedSourcesInfoByChart[_selectedChart] = new List<SourceInformation>(
                            _selectedDataSourcesInformation.Select(dataSourceVm => dataSourceVm.DataSourceInformation));
                    }
                    else
                    {
                        _selectedSourcesInfoByChart.Add(
                            _selectedChart,
                            new List<SourceInformation>(
                                _selectedDataSourcesInformation.Select(
                                    dataSourceVm => dataSourceVm.DataSourceInformation)));
                    }
                }

                _selectedChart = value;

                OnPropertyChanged();
            }
        }

        private bool _isLegendDisplayed;

        /// <summary>
        /// Get or set the boolean indicating if the legend is displayed or not.
        /// </summary>
        public bool IsLegendDisplayed
        {
            get => _isLegendDisplayed;
            set
            {
                if (_isLegendDisplayed == value)
                {
                    return;
                }

                _isLegendDisplayed = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Get a boolean indicating if the chart visualization has been built directly from data or not.
        /// </summary>
        public bool IsFromData { get; }

        public DataSourceViewModel SelectedSeries
        {
            get => SelectedDataSources.FirstOrDefault(ds => ds.Series.Name == SelectedChart.SelectedSeries?.Name);
            set
            {
                SelectedChart.SelectedSeries = value?.Series;
                OnPropertyChanged(nameof(SelectedSeries));
            }
        }

        #endregion Properties

        #region Methods

        private void BuildTreeViewElementCollection()
        {
            // Nothing to build if there is no data at the chart entry.
            if (SelectedDcp == null && !IsFromData)
            {
                return;
            }

            TreeViewSourceElements.Clear();

            var sources = SelectedChartModel.DataSources;

            foreach (var dataSourceInformation in sources)
            {
                if (TreeViewSourceElements.All(e => e.UnitAbbreviation != dataSourceInformation.SourceUnitAbbreviation))
                {
                    TreeViewSourceElements.Add(new UnitTreeNode(dataSourceInformation.SourceUnitAbbreviation));
                }

                // Construct a DataSourceViewModel with the original DataSource if it is accessible
                if (IsFromData)
                {
                    TreeViewSourceElements
                        .SingleOrDefault(e => e.UnitAbbreviation == dataSourceInformation.SourceUnitAbbreviation)
                        ?.DataSources.Add(
                            new DataSourceViewModel(
                                dataSourceInformation,
                                SelectedChart.GetSeriesByName(dataSourceInformation.SourceName),
                                OnDataSourceCheckedChanged));
                }
                else
                {
                    // Search the real DataSource for the given source attributes
                    BuildTreeViewWithSrcLastValue(dataSourceInformation);
                }
            }
        }

        private void OnDataSourceCheckedChanged()
        {
            if (!App.Instance.Dispatcher.CheckAccess())
            {
                App.Instance.Dispatcher.Invoke(OnDataSourceCheckedChangedInvoked);
            }
            else
            {
                OnDataSourceCheckedChangedInvoked();
            }

            UpdateSeriesVisibility(
                new List<SourceInformation>(_selectedDataSourcesInformation.Select(vm => vm.DataSourceInformation)));
            SelectedChart?.Update();
            OnPropertyChanged(nameof(SelectedDataSources));
            OnPropertyChanged(nameof(SelectedSeries));
        }

        private void OnDataSourceCheckedChangedInvoked()
        {
            if (_selectedDataSourcesInformation == null)
            {
                _selectedDataSourcesInformation = new ObservableCollection<DataSourceViewModel>();
            }
            else
            {
                _selectedDataSourcesInformation.Clear();
            }

            var nSelected = 0;
            foreach (var treeViewElement in TreeViewSourceElements)
            {
                foreach (var dataSource in treeViewElement.DataSources)
                {
                    dataSource.IsEnabled = true;

                    if (!dataSource.IsChecked)
                    {
                        continue;
                    }

                    if (nSelected > 10)
                    {
                        dataSource.IsChecked = false;
                    }
                    else
                    {
                        _selectedDataSourcesInformation.Add(dataSource);
                        ++nSelected;
                    }
                }
            }

            if (nSelected != 10)
            {
                return;
            }

            if (!_stopShowingMaxSrcReached)
            {
                ShowMaxSrcNumberReachedMessage();
            }

            DisableUncheckedDataSources();
        }

        private void DisableUncheckedDataSources()
        {
            foreach (var treeViewElement in TreeViewSourceElements)
            {
                var uncheckedDataSources = treeViewElement.DataSources.Where(c => !c.IsChecked);
                foreach (var dataSourceViewModel in uncheckedDataSources)
                {
                    dataSourceViewModel.IsEnabled = false;
                }
            }
        }

        private void ShowMaxSrcNumberReachedMessage()
        {
            Messages.HideAll();
            var maxSrcNumberMessage = new UserMessage(
                MessageLevel.Info,
                nameof(ProcessDataVisualizationResources.SELECTED_SRCS_FULL));

            maxSrcNumberMessage.Commands.Add(
                new UserMessageCommand(
                    nameof(ProcessDataVisualizationResources.NOT_SHOW_AGAIN),
                    new DelegateCommand(
                        () =>
                        {
                            Messages.Hide(maxSrcNumberMessage);
                            _stopShowingMaxSrcReached = true;
                        })));

            maxSrcNumberMessage.CanUserCloseMessage = true;
            Messages.Show(maxSrcNumberMessage);
        }

        /// <summary>
        /// Update the series visibility, according to the given list of visible sources.
        /// </summary>
        /// <param name="dataSourcesInformation">The list of sources visible on the chart.</param>
        public void UpdateSeriesVisibility(List<SourceInformation> dataSourcesInformation)
        {
            if (SelectedChart == null)
            {
                return;
            }

            foreach (var series in SelectedChart.Series)
            {
                series.IsVisible = dataSourcesInformation.Exists(d => d.SourceName == series.Name);
                series.YAxis.IsVisible =
                    dataSourcesInformation.Exists(d => d.SourceUnitAbbreviation == series.YAxis.Unit);
            }
        }

        /// <summary>
        /// Select a data source to be displayed.
        /// It might be useful to call <see cref="UnselectAllDataSources"/> before a first call to this method (by default, all sources are selected).
        /// <remarks>To ensure performances, it is not allowed to select more than 10 sources.</remarks>
        /// </summary>
        /// <param name="dataSourceInformation">Information about the source to be displayed.</param>
        public void SelectDataSource(SourceInformation dataSourceInformation)
        {
            if (SelectedDataSources?.Count > 10)
            {
                throw new InvalidOperationException(
                    "To ensure performances, only 10 selected data sources for are allowed for analysis.");
            }

            foreach (var treeViewElement in TreeViewSourceElements)
            {
                var dataSources = treeViewElement.DataSources.ToArray();
                var notFound = true;
                for (var i = 0; i < dataSources.Length && notFound; ++i)
                {
                    if (!dataSourceInformation.SourceName.Equals(dataSources[i].DataSourceInformation.SourceName))
                    {
                        continue;
                    }

                    notFound = false;
                    dataSources[i].IsChecked = true;
                }
            }

            OnDataSourceCheckedChanged();
        }

        /// <summary>
        /// Select a list of data sources to be displayed.
        /// It might be useful to call <see cref="UnselectAllDataSources"/> before a first call to this method (by default, all sources are selected.
        /// <remarks>To ensure performances, it is not allowed to select more than 10 sources.</remarks>
        /// </summary>
        /// <param name="dataSourceInformation">Information about the sources to be displayed.</param>
        public void SelectDataSources(List<SourceInformation> dataSourceInformation)
            => dataSourceInformation.ForEach(SelectDataSource);

        /// <summary>
        /// Unselect all <see cref="Agileo.DataMonitoring.IDataSource"/>.
        /// </summary>
        public void UnselectAllDataSources()
        {
            foreach (var treeViewElement in TreeViewSourceElements)
            {
                var dataSources = treeViewElement.DataSources.ToArray();
                foreach (var dataSourceViewModel in dataSources)
                {
                    dataSourceViewModel.IsChecked = false;
                }
            }

            OnDataSourceCheckedChanged();
        }

        /// <summary>
        /// Select the <see cref="Agileo.DataMonitoring.IDataSource"/> that was selected on the previous display of the chart, or all data source if the chart is selected for the first time.
        /// </summary>
        private void SelectOldOrDefaultDataSources()
        {
            // If the chart is already existing, select the same DataSources as before.
            // Otherwise, select some sources by default.
            if (_selectedSourcesInfoByChart.ContainsKey(SelectedChart))
            {
                var oldSelectedSources = _selectedSourcesInfoByChart[SelectedChart];
                foreach (var dataSource in oldSelectedSources)
                {
                    SelectDataSource(dataSource);
                }
            }
            else
            {
                UnselectAllDataSources();
            }

            OnDataSourceCheckedChanged();
        }

        private void DisplaySavingSuccessfully(string filePath, bool withCommand)
        {
            Messages.HideAll();
            var exportingSuccessfullyUserMessage = new UserMessage(
                MessageLevel.Success,
                new LocalizableText(
                    nameof(ProcessDataVisualizationResources.SCREENSHOT_CORRECTLY_SAVED_MESSAGE),
                    filePath));
            if (withCommand)
            {
                exportingSuccessfullyUserMessage.Commands.Add(
                    OpenFileDirectory.GetUserMessageCommand(
                        nameof(ProcessDataVisualizationResources.OPEN_FOLDER_CMD),
                        filePath));
            }

            exportingSuccessfullyUserMessage.CanUserCloseMessage = true;
            exportingSuccessfullyUserMessage.SecondsDuration = 5;
            Messages.Show(exportingSuccessfullyUserMessage);
        }

        #endregion Methods

        #region Commands

        #region EnableLegendDisplayingCommand

        private ICommand _enableLegendDisplayingCommand;

        public ICommand EnableLegendDisplayingCommand
            => _enableLegendDisplayingCommand
               ?? (_enableLegendDisplayingCommand = new DelegateCommand(
                   EnableLegendDisplayingCommandExecute,
                   EnableLegendDisplayingCommandCanExecute));

        private bool EnableLegendDisplayingCommandCanExecute() => !IsLegendDisplayed;

        private void EnableLegendDisplayingCommandExecute() => IsLegendDisplayed = true;

        #endregion EnableLegendDisplayingCommand

        #region DisableLegendDisplayingCommand

        private ICommand _disableLegendDisplayingCommand;

        public ICommand DisableLegendDisplayingCommand
            => _disableLegendDisplayingCommand
               ?? (_disableLegendDisplayingCommand = new DelegateCommand(
                   DisableLegendDisplayingCommandExecute,
                   DisableLegendDisplayingCommandCanExecute));

        private bool DisableLegendDisplayingCommandCanExecute() => IsLegendDisplayed;

        private void DisableLegendDisplayingCommandExecute() => IsLegendDisplayed = false;

        # endregion DisableLegendDisplayingCommand

        #region RecenterCommand

        private ICommand _recenterCommand;

        public ICommand RecenterCommand
            => _recenterCommand
               ?? (_recenterCommand = new DelegateCommand(RecenterCommandExecute, RecenterCommandCanExecute));

        private bool RecenterCommandCanExecute() => SelectedChart != null;

        private void RecenterCommandExecute()
        {
            if (SelectedChart.RecenterCommand.CanExecute(null))
            {
                SelectedChart.RecenterCommand.Execute(null);
            }
        }

        #endregion RecenterCommand

        #region FullScreenCommand

        private ICommand _fullScreenCommand;

        public ICommand FullScreenCommand
            => _fullScreenCommand
               ?? (_fullScreenCommand = new DelegateCommand(FullScreenCommandExecute, () => SelectedChart != null));

        private void FullScreenCommandExecute() => SelectedChart.IsMaximized = !SelectedChart.IsMaximized;

        #endregion RecenterCommand

        #region StartCommand

        private ICommand _startCommand;

        public ICommand StartCommand
            => _startCommand ?? (_startCommand = new DelegateCommand(StartCommandExecute, StartCommandCanExecute));

        private bool StartCommandCanExecute() => SelectedChart != null && SelectedChart.IsPaused && !IsFromData;

        private void StartCommandExecute() => SelectedChart.IsPaused = false;

        #endregion StartCommand

        #region StopCommand

        private ICommand _stopCommand;

        public ICommand StopCommand
            => _stopCommand ?? (_stopCommand = new DelegateCommand(StopCommandExecute, StopCommandCanExecute));

        private bool StopCommandCanExecute() => SelectedChart != null && !SelectedChart.IsPaused && !IsFromData;

        private void StopCommandExecute() => SelectedChart.IsPaused = true;

        #endregion StopCommand

        #region ClearCommand

        private ICommand _clearCommand;

        public ICommand ClearCommand
            => _clearCommand ?? (_clearCommand = new DelegateCommand(ClearCommandExecute, ClearCommandCanExecute));

        private bool ClearCommandCanExecute() => SelectedChart != null && !IsFromData;

        private void ClearCommandExecute()
        {
            var popup = new Popup(
                nameof(ProcessDataVisualizationResources.CONFIRMATION),
                nameof(ProcessDataVisualizationResources.CONFIRM_CLEAR_GRAPH_MSG))
            {
                SeverityLevel = MessageLevel.Warning
            };
            popup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));
            popup.Commands.Add(
                new PopupCommand(
                    nameof(Agileo.GUI.Properties.Resources.S_OK),
                    new DelegateCommand(
                        () =>
                        {
                            SelectedChart.Clear();
                            SelectedChart.Update();
                        })));
            Popups.Show(popup);
        }

        #endregion ClearCommand

        #region MakeCaptureCommand

        private ICommand _makeCaptureCommand;

        public ICommand MakeCaptureCommand
            => _makeCaptureCommand
               ?? (_makeCaptureCommand = new DelegateCommand<FrameworkElement>(
                   MakeCaptureCommandExecute,
                   MakeCaptureCommandCanExecute));

        private bool MakeCaptureCommandCanExecute(FrameworkElement arg) => SelectedChart != null;

        private readonly Dictionary<string, int> _fileNames = new();

        private void MakeCaptureCommandExecute(FrameworkElement arg)
        {
            const string path = @".\DataCollectionPlan\ScreenShotVisualization\";

            var basePath = IsFromData ? "AnalysisView" : SelectedDcp.Name;
            var fileName = basePath + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss");

            string fullPath;
            if (!_fileNames.ContainsKey(fileName))
            {
                _fileNames.Add(fileName, 0);
                fullPath = path + fileName;
            }
            else
            {
                _fileNames[fileName]++;
                fullPath = path + fileName + "(" + _fileNames[fileName] + ")";
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(fullPath);
            }

            var screenShotFilePath = TakeScreenShot((int)arg.ActualHeight, (int)arg.ActualWidth, arg, fullPath, ".png");
            DisplaySavingSuccessfully(screenShotFilePath, true);
        }

        public static string TakeScreenShot(
            int height,
            int width,
            FrameworkElement visual,
            string file,
            string extension)
        {
            // Set another DPI if necessary
            var bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            if (visual is ProcessDataVisualizationView visualModel)
            {
                var chartView = visualModel.GraphView.GetChildren<ChartView>().FirstOrDefault();
                if (chartView != null)
                {
                    bmp.Render(chartView);
                }
            }

            BitmapEncoder encoder;

            switch (extension)
            {
                case ".gif":
                    encoder = new GifBitmapEncoder();
                    break;
                case ".png":
                    encoder = new PngBitmapEncoder();
                    break;
                case ".jpg":
                    encoder = new JpegBitmapEncoder();
                    break;
                default:
                    return string.Empty;
            }

            encoder.Frames.Add(BitmapFrame.Create(bmp));
            var path = file + extension;
            using (Stream stm = File.Create(path))
                encoder.Save(stm);
            return path;
        }

        #endregion MakeCaptureCommand

        #region SetupCommand

        private ICommand _setupCommand;

        public ICommand SetupCommand
            => _setupCommand ?? (_setupCommand = new DelegateCommand(SetupCommandExecute, SetupCommandCanExecute));

        private bool SetupCommandCanExecute() => SelectedChart != null;

        private void SetupCommandExecute()
        {
            if (SelectedChart == null)
            {
                return;
            }

            var setupGraphPopup = new SetupGraphPopup(SelectedChart, !IsFromData);
            var popup = new Popup(nameof(ProcessDataVisualizationResources.SETUP)) { Content = setupGraphPopup };
            popup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_OK)));
            Popups.Show(popup);
        }

        #endregion SetupCommand

        #endregion Commands

        #region DataCollectionPlanSpecificities

        #region DcpProperties

        /// <summary>
        /// The librarian containing all <see cref="DataCollectionPlan"/> which charts could eventually be displayed.
        /// </summary>
        public DataCollectionPlanLibrarian DataCollectionPlanLibrarian { get; }

        private DataCollectionPlan _selectedDcp;

        /// <summary>
        /// Get or set the <see cref="DataCollectionPlan"/> where at least one chart is displayed.
        /// On the set, the first chart of the <see cref="SelectedDcp"/> will be selected.
        /// </summary>
        public DataCollectionPlan SelectedDcp
        {
            get => _selectedDcp;
            set
            {
                if (ReferenceEquals(_selectedDcp, value))
                {
                    return;
                }

                _selectedDcp = value;

                if (_selectedDcp == null)
                {
                    SelectedChartModel = null;
                }
                else
                {
                    SelectedChartModel =
                        (ChartDataWriter)_selectedDcp.DataWriters.FirstOrDefault(writer => writer is ChartDataWriter);
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(TreeViewSourceElements));
            }
        }

        #endregion DcpProperties

        #region DcpEventHandlers

        private void DataCollectionPlanLibrarian_DataCollectionPlanAdded(
            object sender,
            DataCollectionPlanAddedEventArgs e)
        {
            if (SelectedDcp == null)
            {
                SelectedDcp = DataCollectionPlanLibrarian.Plans.FirstOrDefault(
                    dcp => dcp.DataWriters.Any(writer => writer is ChartDataWriter));
            }

            // Create a vm associated with the new dcp and add it to the collection of dcpVms
            var dcpVm = new DataCollectionPlanChartsNode(e.DataCollectionPlan);
            _dataCollectionPlanVms.Add(dcpVm);
            dcpVm.PropertyChanged += DataCollectionPlanVmOnPropertyChanged;
            if (dcpVm.ContainCharts)
            {
                DataCollectionPlansWithCharts.Add(dcpVm);
            }
        }

        private void DataCollectionPlanLibrarian_DataCollectionPlanRemoved(
            object sender,
            DataCollectionPlanRemovedEventArgs e)
        {
            if (!DataCollectionPlanLibrarian.Plans.Contains(SelectedDcp))
            {
                SelectedChartModel = null;
                App.Instance.Dispatcher?.Invoke(() => _selectedDataSourcesInformation.Clear());
                TreeViewSourceElements.Clear();
                SelectedDcp = DataCollectionPlanLibrarian.Plans.FirstOrDefault(
                    dcp => dcp.DataWriters.Any(writer => writer is ChartDataWriter));
            }

            var concernedViewModel = _dataCollectionPlanVms.FirstOrDefault(
                node => ReferenceEquals(node.DataCollectionPlan, e.DataCollectionPlan));
            if (concernedViewModel != null)
            {
                _dataCollectionPlanVms.Remove(concernedViewModel);
                concernedViewModel.PropertyChanged -= DataCollectionPlanVmOnPropertyChanged;
                if (concernedViewModel.ContainCharts)
                {
                    DataCollectionPlansWithCharts.Remove(concernedViewModel);
                }

                concernedViewModel.Dispose();
            }

            foreach (var dataWriter in e.DataCollectionPlan.DataWriters.Where(writer => writer is ChartDataWriter))
            {
                var chartDataWriter = (ChartDataWriter)dataWriter;
                chartDataWriter.RemoveChartViewModel(this);
            }
        }

        #endregion DcpEventHandlers

        #region DcpOtherMethods

        private void CheckDcpContainsNewChartModel(ChartDataWriter value)
        {
            var selectedDcpContainsValue = _selectedDcp.DataWriters.Any(
                writer =>
                {
                    var dataWriter = writer as ChartDataWriter;
                    return dataWriter != null && dataWriter == value;
                });

            if (!selectedDcpContainsValue)
            {
                throw new InvalidOperationException(
                    $"The given {nameof(ChartDataWriter)} is not contained into the {SelectedDcp}");
            }
        }

        // The sources view models will contain the original source to allow accessing the source last value in the view.
        private void BuildTreeViewWithSrcLastValue(SourceInformation dataSourceInformation)
        {
            var dataSource = SelectedDcp?.DataSources.First(
                src => src.Information.SourceName.Equals(dataSourceInformation.SourceName));
            TreeViewSourceElements
                .SingleOrDefault(e => e.UnitAbbreviation == dataSourceInformation.SourceUnitAbbreviation)
                ?.DataSources.Add(
                    new DataSourceViewModel(
                        dataSource,
                        SelectedChart.GetSeriesByName(dataSourceInformation.SourceName),
                        OnDataSourceCheckedChanged));
        }

        #endregion DcpOtherMethods

        #endregion DataCollectionPlanSpecificities

        #region IDisposable

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing && !IsFromData)
                {
                    foreach (var dataCollectionPlanVm in _dataCollectionPlanVms)
                    {
                        foreach (var chartDataWriter in dataCollectionPlanVm.Charts)
                        {
                            chartDataWriter.RemoveChartViewModel(this);
                        }

                        dataCollectionPlanVm.PropertyChanged -= DataCollectionPlanVmOnPropertyChanged;
                        dataCollectionPlanVm.Dispose();
                    }

                    DataCollectionPlanLibrarian.DataCollectionPlanAdded -=
                        DataCollectionPlanLibrarian_DataCollectionPlanAdded;
                    DataCollectionPlanLibrarian.DataCollectionPlanRemoved -=
                        DataCollectionPlanLibrarian_DataCollectionPlanRemoved;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}
