using System;
using System.ComponentModel;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.DataMonitoring;
using Agileo.DataMonitoring.DataWriter.Chart;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

using Humanizer;

using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.RealTimeAnalysis
{
    /// <summary>
    /// Panel providing real-time data analysis from a <see cref="DataCollectionPlanLibrarian" /> (using
    /// charts).
    /// </summary>
    public class RealTimeAnalysisPanel : BusinessPanel
    {
        #region Constructors

        static RealTimeAnalysisPanel()
        {
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(RealTimeAnalysisPanelResources)));
        }

        /// <inheritdoc />
        /// <summary>Default constructor only used by view in design instance.</summary>
        public RealTimeAnalysisPanel()
            : this("Design Time Constructor", new DataCollectionPlanLibrarian())
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="RealTimeAnalysisPanel" /> class.
        /// </summary>
        /// <param name="relativeId">The relative id.</param>
        /// <param name="dataCollectionPlanLibrarian">
        /// The object responsible to provide data collection plans existing in the system.
        /// </param>
        /// <param name="icon">The icon.</param>
        public RealTimeAnalysisPanel(
            string relativeId,
            DataCollectionPlanLibrarian dataCollectionPlanLibrarian,
            IIcon icon = null)
            : base(relativeId, icon)
        {
            // Create chart views
            ProcessDataVisualizationViewModelTop = new ProcessDataVisualizationViewModel(App.Instance.DataCollectionPlanLibrarian);
            ProcessDataVisualizationViewModelBottom = new ProcessDataVisualizationViewModel(App.Instance.DataCollectionPlanLibrarian);

            var dataCollectionPlans = dataCollectionPlanLibrarian.Plans
                .Where(dcp => dcp.DataWriters.Any(writer => writer is ChartDataWriter))
                .ToList();

            if (dataCollectionPlans.Count > 1)
            {
                ChartInitialization(ProcessDataVisualizationViewModelTop, dataCollectionPlans[0]);
                ChartInitialization(ProcessDataVisualizationViewModelBottom, dataCollectionPlans[1]);
            }

            ProcessDataVisualizationViewModelTop.PropertyChanged += ProcessDataVisualizationViewModelTopOnPropertyChanged;
            ProcessDataVisualizationViewModelBottom.PropertyChanged += ProcessDataVisualizationViewModelBottomOnPropertyChanged;
        }

        #endregion Constructors

        #region Properties

        public LocalizableText TopProcessVisualizationName
            => new(
                nameof(RealTimeAnalysisPanelResources.PROCESS_VISUALIZATION),
                ProcessDataVisualizationViewModelTop.SelectedDcp?.Name
                    .Humanize(LetterCasing.Title));

        /// <summary>The top chart view where to data is analyzed.</summary>
        public ProcessDataVisualizationViewModel ProcessDataVisualizationViewModelTop { get; }

        public LocalizableText BottomProcessVisualizationName
            => new(
                nameof(RealTimeAnalysisPanelResources.PROCESS_VISUALIZATION),
                ProcessDataVisualizationViewModelBottom.SelectedDcp?.Name.Humanize(
                    LetterCasing.Title));

        /// <summary>The bottom chart view where to data is analyzed.</summary>
        public ProcessDataVisualizationViewModel ProcessDataVisualizationViewModelBottom { get; }

        #endregion Properties

        #region Private Methods

        private void ChartInitialization(
            ProcessDataVisualizationViewModel dataVisualization,
            DataCollectionPlan defaultDcpToDisplay)
        {
            if (defaultDcpToDisplay == null)
            {
                return;
            }

            dataVisualization.SelectedDcp = defaultDcpToDisplay;

            // display first three sources
            for (var i = 0; i < defaultDcpToDisplay.DataSources.Count || i > 2; ++i)
            {
                dataVisualization.SelectDataSource(defaultDcpToDisplay.DataSources[i]?.Information);
            }
        }

        #endregion Private Methods

        #region EventHandlers

        private void ProcessDataVisualizationViewModelTopOnPropertyChanged(
            object sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(ProcessDataVisualizationViewModel.SelectedDcp)))
            {
                OnPropertyChanged(nameof(TopProcessVisualizationName));
            }
        }

        private void ProcessDataVisualizationViewModelBottomOnPropertyChanged(
            object sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(ProcessDataVisualizationViewModel.SelectedDcp)))
            {
                OnPropertyChanged(nameof(BottomProcessVisualizationName));
            }
        }

        #endregion EventHandlers

        #region IDisposable

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            ProcessDataVisualizationViewModelBottom.PropertyChanged -= ProcessDataVisualizationViewModelBottomOnPropertyChanged;
            ProcessDataVisualizationViewModelTop.PropertyChanged -= ProcessDataVisualizationViewModelTopOnPropertyChanged;

            if (disposing)
            {
                ProcessDataVisualizationViewModelTop.Dispose();
                ProcessDataVisualizationViewModelBottom.Dispose();
            }

            _disposed = true;

            base.Dispose(disposing);
        }

        ~RealTimeAnalysisPanel()
        {
            Dispose(true);
        }

        #endregion IDisposable
    }
}
