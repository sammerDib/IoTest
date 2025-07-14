using System;

using Agileo.DataMonitoring;
using Agileo.GUI.Components;
using Agileo.LineCharts.Abstractions.Model;

using UnitySC.GUI.Common.Vendor.Helpers;

using IDataSource = Agileo.DataMonitoring.IDataSource;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization.Core
{
    /// <summary>
    /// ViewModel representing <see cref="Agileo.DataMonitoring.IDataSource"/> and saying if it is selected or not.
    /// </summary>
    public class DataSourceViewModel : Notifier
    {
        #region Fields

        private readonly Action _onCheckedChanged;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Build the view model from the given parameters.
        /// </summary>
        /// <param name="dataSourceInformation">The <see cref="SourceInformation"/> on which the view model is based.</param>
        /// <param name="series">The <see cref="Agileo.LineCharts.Abstractions.Model.ISeries"/> representing this <paramref name="dataSourceInformation"/>.</param>
        /// <param name="onCheckedChanged">The action to perform when the <paramref name="dataSourceInformation"/> selection change.</param>
        public DataSourceViewModel(SourceInformation dataSourceInformation, ISeries series, Action onCheckedChanged)
        {
            _onCheckedChanged = onCheckedChanged;
            DataSourceInformation = dataSourceInformation;
            Series = series;
            Name = HumanizerWithConstants.Humanize(DataSourceInformation.SourceName);
        }

        /// <summary>
        /// Build the view model from the given parameters.
        /// </summary>
        /// <param name="dataSource">The <see cref="Agileo.DataMonitoring.IDataSource"/> on which the view model is based.</param>
        /// <param name="series">The <see cref="ISeries"/> representing this <paramref name="dataSource"/>.</param>
        /// <param name="onCheckedChanged">The action to perform when the <paramref name="dataSource"/> selection change.</param>
        public DataSourceViewModel(IDataSource dataSource, ISeries series, Action onCheckedChanged)
        {
            _onCheckedChanged = onCheckedChanged;
            DataSource = dataSource;
            DataSourceInformation = new SourceInformation(dataSource.Information.SourceName, dataSource.Information.SourceUnitName, dataSource.Information.SourceUnitAbbreviation);
            Series = series;
            Name = HumanizerWithConstants.Humanize(DataSourceInformation.SourceName);
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// The <see cref="SourceInformation"/> on which the view model is based.
        /// </summary>
        public SourceInformation DataSourceInformation { get; }

        /// <summary>
        /// The <see cref="IDataSource"/> on which the view model is based.
        /// </summary>
        public IDataSource DataSource { get; }

        /// <summary>
        /// Get the humanized name of <see cref="DataSourceInformation"/>.
        /// </summary>
        public string Name { get; }

        private bool _isChecked;

        /// <summary>
        /// Get: true if the current <see cref="DataSourceInformation"/> is selected, false otherwise.
        /// Set: Select or unselect the <see cref="DataSourceInformation"/>.
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (_isChecked == value) return;
                _isChecked = value;
                _onCheckedChanged();
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        private bool _isEnabled = true;

        /// <summary>
        /// Get or set the availability of the <see cref="DataSourceInformation"/>.
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled == value) return;
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));

            }
        }

        private ISeries _series;

        /// <summary>
        /// Get or set the <see cref="ISeries"/> associated with <see cref="DataSourceInformation"/>.
        /// </summary>
        public ISeries Series
        {
            get { return _series; }
            set
            {
                if (_series == value) return;
                _series = value;
                OnPropertyChanged(nameof(Series));
            }
        }

        #endregion Properties
    }
}
