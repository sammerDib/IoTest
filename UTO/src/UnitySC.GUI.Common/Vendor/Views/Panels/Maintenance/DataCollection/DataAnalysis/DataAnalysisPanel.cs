using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.Common.Tracing;
using Agileo.DataMonitoring;
using Agileo.DataMonitoring.DataWriter.File;
using Agileo.DataMonitoring.Events;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.DataAnalysis;

using UnitySC.GUI.Common.Vendor.Exceptions;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.DataAnalysis.Analyzer;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.DataAnalysis
{
    /// <summary>
    /// Panel displaying output file and providing file analysis services.
    /// </summary>
    public class DataAnalysisPanel : BusinessPanel
    {
        private readonly DataCollectionPlanLibrarian _dataCollectionPlanLibrarian;
        private readonly UserMessage _unableToAnalyzeMessage =
            new UserMessage(MessageLevel.Error, nameof(DataAnalysisPanelResources.CANT_ANALYSE_FILE));

        #region Constructors

        static DataAnalysisPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(DataAnalysisPanelResources)));
        }

        /// <inheritdoc />
        /// <summary>
        /// Default constructor only used by view in design instance.
        /// </summary>
        public DataAnalysisPanel() : this("Design Time Constructor", new DataCollectionPlanLibrarian())
        {
            if (!IsInDesignMode)
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="DataAnalysisPanel"/> class.
        /// </summary>
        /// <param name="relativeId">The relative id.</param>
        /// <param name="dataCollectionPlanLibrarian">The <see cref="DataCollectionPlanLibrarian"/> to watch to see <see cref="DataCollectionPlan"/> stops.</param>
        /// <param name="icon">The icon.</param>
        public DataAnalysisPanel(string relativeId, DataCollectionPlanLibrarian dataCollectionPlanLibrarian, IIcon icon = null) : base(relativeId, icon)
        {
            _dataCollectionPlanLibrarian = dataCollectionPlanLibrarian ?? throw new ArgumentNullException(nameof(dataCollectionPlanLibrarian));

            Initialize();

            // Initialize sorter
            DataTableSource.Sort.SetCurrentSorting(nameof(FileAnalysisViewModel.RelatedDataCollectionPlanName), ListSortDirection.Descending);

            // Add Commands
            Commands.Add(new BusinessPanelCommand(nameof(DataAnalysisPanelResources.ANALYZE_DATA), AnalyzeDataCommand, PathIcon.Chart));
            Commands.Add(new BusinessPanelCommand(nameof(DataAnalysisPanelResources.OPEN_FILE_CMD), OpenFileCommand, PathIcon.OpenFile));
            Commands.Add(new BusinessPanelCommand(nameof(DataAnalysisPanelResources.OPEN_FOLDER_CMD), OpenFolderCommand, PathIcon.OpenFolder));
        }

        private void LoadDcpFiles()
        {
            DataTableSource.RemoveAll(fileAnalysisViewModel => fileAnalysisViewModel.GetDataReader() == null);

            if (!Directory.Exists(App.Instance.Config.ApplicationPath.DcpStoragePath))
                return;

            var files = Directory.EnumerateFiles(Path.GetFullPath(App.Instance.Config.ApplicationPath.DcpStoragePath), "*.csv");
            foreach (var f in files)
            {
                if (DataTableSource.Any(fileViewModel => fileViewModel.FileName.Equals(Path.GetFileName(f))))
                    continue;

                DataTableSource.Add(new FileAnalysisViewModel(" ", f, null));
            }
        }

        private void Initialize()
        {
            _dataCollectionPlanLibrarian.DataCollectionPlanAdded += _dataCollectionPlanLibrarian_DataCollectionPlanAdded;
            _dataCollectionPlanLibrarian.DataCollectionPlanRemoved += _dataCollectionPlanLibrarian_DataCollectionPlanRemoved;
            _dataCollectionPlanLibrarian.Plans.ToList().ForEach(plan => { plan.Stopped += Dcp_Stopped; });
        }

        private void _dataCollectionPlanLibrarian_DataCollectionPlanAdded(object sender, DataCollectionPlanAddedEventArgs e)
        {
            var dcp = e.DataCollectionPlan;
            if (dcp == null) return;
            dcp.Stopped += Dcp_Stopped;
        }

        private void _dataCollectionPlanLibrarian_DataCollectionPlanRemoved(object sender, DataCollectionPlanRemovedEventArgs e)
        {
            e.DataCollectionPlan.Stopped -= Dcp_Stopped;
        }

        private void Dcp_Stopped(object sender, EventArgs e)
        {
            var dcp = sender as DataCollectionPlan;
            if (dcp == null) return;
            var dcpWriters = dcp.DataWriters.OfType<FileDataWriter>();
            dcpWriters.ToList().ForEach(writer =>
            {
                Application.Current.Dispatcher?.BeginInvoke((Action)delegate
                {
                    // Each FileDataWriter has a corresponding reader in order to be able to read data.
                    // Get the reader here and create a copy because DCP could be removed.
                    var reader = writer.DataReader.Clone() as FileDataReader;
                    DataTableSource.Add(new FileAnalysisViewModel(dcp.Name, writer.OutputFile, reader));
                });
            });
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get the <see cref="SelectedFileAnalysisViewModel"/> <see cref="DataTableSource{T}"/>. Each element contains data about the linked file.
        /// </summary>
        public DataTableSource<FileAnalysisViewModel> DataTableSource { get; } = new DataTableSource<FileAnalysisViewModel>();

        private FileAnalysisViewModel _selectedFileAnalysisViewModel;

        /// <summary>
        /// Get or set the selected <see cref="SelectedFileAnalysisViewModel"/>.
        /// </summary>
        public FileAnalysisViewModel SelectedFileAnalysisViewModel
        {
            get { return _selectedFileAnalysisViewModel; }
            set
            {
                SetAndRaiseIfChanged(ref _selectedFileAnalysisViewModel, value);
                Messages.Hide(_unableToAnalyzeMessage);
            }
        }

        /// <summary>
        /// Link to the view model displaying chart. This chart represent data read by the <see cref="SelectedFileAnalysisViewModel"/>.
        /// </summary>
        public ChartAnalyzerPanel ChartAnalyzer { get; } = new ChartAnalyzerPanel();

        #endregion Properties

        #region Commands

        #region AnalyzeData

        private ICommand _analyzeDataCommand;

        public ICommand AnalyzeDataCommand => _analyzeDataCommand ?? (_analyzeDataCommand = new DelegateCommand(AnalyzeDataExecute, AnalyzeDataCanExecute));

        private bool AnalyzeDataCanExecute()
        {
            return SelectedFileAnalysisViewModel != null;
        }

        private void AnalyzeDataExecute()
        {
            Messages.Hide(_unableToAnalyzeMessage);

            try
            {
                ChartAnalyzer.Analyze(SelectedFileAnalysisViewModel);
                ChartAnalyzer.IsInAnalyze = true;
                UpdateCommandButtonVisibility();
            }
            catch (InvalidDataSourceException e)
            {
                Messages.Show(_unableToAnalyzeMessage);
                App.Instance.Tracer.Trace(nameof(DataAnalysisPanel), TraceLevelType.Error, e.Message, e.ToTraceParam());
            }
        }

        /// <summary>
        /// The chart analysis view will be clipped onto the current panel.
        /// A switch from a view to the other must hide the old view commands and show the new view ones.
        /// </summary>
        private void UpdateCommandButtonVisibility()
        {
            Commands.ToList().ForEach(cmd =>
            {
                if (ChartAnalyzer.Commands.Contains(cmd))
                {
                    cmd.IsVisible = ChartAnalyzer.IsInAnalyze;
                }
                else
                {
                    cmd.IsVisible = !ChartAnalyzer.IsInAnalyze;
                }
            });
        }

        #endregion AnalyzeData

        #region OpenFile

        private ICommand _openFileCommand;

        public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new DelegateCommand(OpenFileExecute, OpenFileCanExecute));

        private bool OpenFileCanExecute()
        {
            return SelectedFileAnalysisViewModel != null;
        }

        private void OpenFileExecute()
        {
            Process.Start(SelectedFileAnalysisViewModel.FilePath);
        }

        #endregion OpenFile

        #region OpenFolder

        private ICommand _openFolderCommand;

        public ICommand OpenFolderCommand => _openFolderCommand ?? (_openFolderCommand = new DelegateCommand(OpenFolderExecute, OpenFolderCanExecute));

        private bool OpenFolderCanExecute()
        {
            return SelectedFileAnalysisViewModel != null;
        }

        private void OpenFolderExecute()
        {
            OpenFileDirectory.ProcessStart(SelectedFileAnalysisViewModel.FilePath);
        }

        #endregion OpenFolder

        #endregion Commands

        #region Override

        /// <inheritdoc />
        public override void OnSetup()
        {
            base.OnSetup();
            ChartAnalyzer.Commands.ForEach(cmd => Commands.Add(cmd));
            ChartAnalyzer.OnAnalyzeEnded += UpdateCommandButtonVisibility;
        }

        /// <inheritdoc />
        public override void OnShow()
        {
            base.OnShow();
            LoadDcpFiles();
            UpdateCommandButtonVisibility();
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                foreach (var dataCollectionPlan in _dataCollectionPlanLibrarian.Plans)
                {
                    dataCollectionPlan.Stopped -= Dcp_Stopped;
                }

                _dataCollectionPlanLibrarian.DataCollectionPlanAdded -= _dataCollectionPlanLibrarian_DataCollectionPlanAdded;
                _dataCollectionPlanLibrarian.DataCollectionPlanRemoved -= _dataCollectionPlanLibrarian_DataCollectionPlanRemoved;
                ChartAnalyzer.OnAnalyzeEnded -= UpdateCommandButtonVisibility;
            }

            base.Dispose(disposing);

            _disposed = true;
        }


        #endregion IDisposable
    }
}
