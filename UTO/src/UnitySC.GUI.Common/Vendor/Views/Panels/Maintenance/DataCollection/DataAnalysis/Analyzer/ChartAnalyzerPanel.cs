using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Agileo.DataMonitoring;
using Agileo.DataMonitoring.DataWriter.File.Csv;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;

using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.DataAnalysis;

using UnitySC.GUI.Common.Vendor.Exceptions;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.DataAnalysis.Analyzer
{
    /// <summary>
    /// View model used to transform a <see cref="FileAnalysisViewModel"/> to a chart visualization.
    /// </summary>
    public class ChartAnalyzerPanel : Notifier
    {
        #region Fields

        private FileAnalysisViewModel _currentAnalyze;

        public delegate void AnalyzeEndedHandler();

        public event AnalyzeEndedHandler OnAnalyzeEnded;

        #endregion

        /// <summary>
        /// Build a new chart analysis view and add specific commands.
        /// </summary>
        public ChartAnalyzerPanel()
        {
            Commands.Add(new BusinessPanelCommand(nameof(DataAnalysisPanelResources.EXIT), ExitCommand, PathIcon.Close));
        }

        #region Properties

        /// <summary>
        /// The <see cref="List{T}"/> of <see cref="BusinessPanelCommand"/> associated with the chart visualization.
        /// </summary>
        public List<BusinessPanelCommand> Commands { get; } = new List<BusinessPanelCommand>();

        private bool _isInAnalyze;

        /// <summary>
        /// True if the chart is displayed, false otherwise.
        /// </summary>
        public bool IsInAnalyze
        {
            get { return _isInAnalyze; }
            set
            {
                if (value == _isInAnalyze) return;
                _isInAnalyze = value;
                OnPropertyChanged(nameof(IsInAnalyze));
            }
        }

        /// <summary>
        /// The chart view to display.
        /// </summary>
        public ProcessDataVisualizationViewModel ProcessDataVisualizationViewModel { get; private set; }

        #endregion

        #region ExitCommand

        private ICommand _exitCommand;

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand != null)
                {
                    return _exitCommand;
                }

                _exitCommand = new DelegateCommand(ExitCommandExecute);

                return _exitCommand;
            }
        }

        private void ExitCommandExecute()
        {
            IsInAnalyze = false;
            OnAnalyzeEnded?.Invoke();
            ProcessDataVisualizationViewModel.Dispose();
        }

        #endregion

        internal void Analyze(FileAnalysisViewModel selectedFileAnalysisViewModel)
        {
            _currentAnalyze = selectedFileAnalysisViewModel
                              ?? throw new ArgumentNullException(nameof(selectedFileAnalysisViewModel));

            var path = _currentAnalyze.FilePath;
            var dataReader = _currentAnalyze.GetDataReader();
            List<SourceInformation> dataSources;

            // For now, only .csv files can be analyzed
            if (dataReader == null
                && string.Equals(_currentAnalyze.Extension, ".CSV", StringComparison.OrdinalIgnoreCase))
            {
                dataReader = new CsvDataReader("yyyy/MM/dd HH:mm:ss.fff");
            }

            // Selected file is not a .csv file
            if (dataReader == null)
            {
                throw new InvalidDataSourceException(
                    $"Selected file ({_currentAnalyze.FileName}) cannot be analyzed because extension ({_currentAnalyze.Extension}) is not supported.");
            }

            try
            {
                // Here, we cannot be sure that the csv file can be read correctly. So TryCatch is needed.
                dataSources = dataReader.GetSourcesInfo(path).ToList();
            }
            catch
            {
                throw new InvalidDataSourceException(
                    $"Selected file ({_currentAnalyze.FileName}) cannot be analyzed because its content is incorrectly formatted.");
            }

            if (dataSources == null || dataSources.Count == 0)
            {
                throw new InvalidDataSourceException(
                    $"Selected file ({_currentAnalyze.FileName}) cannot be analyzed because it contains no data.");
            }

            // Create chart and select the first 10 data sources
            ProcessDataVisualizationViewModel = new ProcessDataVisualizationViewModel(
                dataSources,
                dataReader.ReadData(path).ToList());

            if (dataSources.Count < 10)
            {
                ProcessDataVisualizationViewModel.SelectDataSources(dataSources);
            }
            else
            {
                ProcessDataVisualizationViewModel.SelectDataSources(dataSources.GetRange(0, 10));
            }
        }
    }
}
