using System;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.GUI.Components;
using Agileo.LineCharts.Abstractions.Enums;
using Agileo.LineCharts.Abstractions.Model;
using Agileo.LineCharts.OxyPlot.Helpers;

using UnitsNet;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.ChartVisualization.Popups
{
    /// <summary>
    /// Provide real-time setup functionalities for the chart.
    /// </summary>
    public class SetupGraphPopup : Notifier
    {
        private readonly IChart _chart;

        /// <summary>
        /// Build the setup popup with the given parameters.
        /// </summary>
        /// <param name="chart">The chart on which change will be done.</param>
        /// <param name="isRealTimeChart">Used to know if there will be time linked attributes to modify or not.</param>
        public SetupGraphPopup(IChart chart, bool isRealTimeChart)
        {
            _chart = chart;
            IsRealTimeChart = isRealTimeChart;
            Axis = new ObservableCollection<IAxis>();
            foreach (var chartSeries in chart.Series)
            {
                if (!chartSeries.IsVisible || Axis.Contains(chartSeries.YAxis)) continue;
                Axis.Add(chartSeries.YAxis);
            }

            var firstSeries = chart.Series.FirstOrDefault();
            if (firstSeries == null) return;
            IsSlidingMode = firstSeries.XAxis.Type == AxisType.Sliding;
            DurationRange =
                Duration.FromHours(TimeSpanToDoubleConverter.DoubleToTimeSpan(firstSeries.XAxis.Range).TotalHours);
        }

        private ObservableCollection<IAxis> _axis;

        /// <summary>
        /// The <see cref="ObservableCollection{T}"/> of <see cref="IAxis"/> to be setup.
        /// </summary>
        public ObservableCollection<IAxis> Axis
        {
            get { return _axis; }
            set
            {
                if (_axis == value) return;
                _axis = value;
                OnPropertyChanged(nameof(Axis));
            }
        }

        private bool _isSlidingMode;

        /// <summary>
        /// Get: true if the sliding mode is activated, false otherwise
        /// Set: Change the mode to SlidingMode or to AutomaticMode.
        ///     SlidingMode: the x axis will automatically be rescaled to match with the sliding window size, and to contain the last added value.
        ///     AutomaticMode: The x axis will automatically be rescaled to fit with the data range.
        /// </summary>
        public bool IsSlidingMode
        {
            get { return _isSlidingMode; }
            set
            {
                if (_isSlidingMode == value) return;
                _isSlidingMode = value;

                var mode = IsSlidingMode ? AxisType.Sliding : AxisType.Automatic;
                ChangeChartMode(mode);

                OnPropertyChanged(nameof(IsSlidingMode));
            }
        }

        private void ChangeChartMode(AxisType mode)
        {
            foreach (var xAxis in _chart.Series.Select(series=>series.XAxis))
            {
                xAxis.Type = mode;

                xAxis.IsRangeActive = mode == AxisType.Sliding;
            }
            _chart.RecenterCommand.Execute(null);
        }

        private Duration _durationRange;

        /// <summary>
        /// Get or set the size of the sliding window, in term of duration.
        /// </summary>
        public Duration DurationRange
        {
            get { return _durationRange; }
            set
            {

                if (_durationRange == value) { return; }

                _durationRange = value;
                ChangeSlidingRange();

                OnPropertyChanged(nameof(DurationRange));
            }
        }

        private void ChangeSlidingRange()
        {
            if (!_chart.Series.Any()) return;

            var newSlidingRange = TimeSpanToDoubleConverter.TimeSpanToDouble(
                _durationRange.CompareTo(Duration.FromDays(TimeSpanToDoubleConverter.MaxNumberOfDaysParSpan)) > 0
                    ? TimeSpan.FromDays(TimeSpanToDoubleConverter.MaxNumberOfDaysParSpan)
                    : _durationRange.ToTimeSpan());

            if (Math.Abs(newSlidingRange - _chart.Series.First().XAxis.Range) < 0) return;

            foreach (var chartSeries in _chart.Series)
            {
                chartSeries.XAxis.Range = newSlidingRange;
            }

            _chart.RecenterCommand.Execute(null);
        }

        /// <summary>
        /// Indicate if the linked evolve in real-time or if it is static.
        /// </summary>
        public bool IsRealTimeChart { get; }
    }
}
