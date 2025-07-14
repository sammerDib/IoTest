using System;

namespace UnitySC.Shared.ResultUI.Common.Components.Generic.Filters
{
    public interface IFilterPeriod : IFilter
    {
        DateTime StartDate { get; set; }

        DateTime EndDate { get; set; }

        bool StartDateUsed { get; set; }

        bool EndDateUsed { get; set; }

        TimeSpan StartTime { get; set; }

        TimeSpan EndTime { get; set; }

        bool UseHoursAndMinutes { get; }
    }

    /// <summary>
    /// Generic filter for a temporal duration
    /// </summary>
    /// <typeparam name="T">Type of model to filter</typeparam>
    public class FilterPeriod<T> : Filter<T>, IFilterPeriod
    {
        private readonly Func<T, DateTime?> _predicate;

        public FilterPeriod(string name, Func<T, DateTime?> predicate) : base(name)
        {
            _predicate = predicate;

            StartDate = DateTime.Today.AddDays(-7);
            EndDate = DateTime.Today.AddDays(1);
        }

        #region Use Hours and Minutes

        bool IFilterPeriod.UseHoursAndMinutes => UseHoursAndMinutes;

        private bool _useHoursAndMinutes;

        public bool UseHoursAndMinutes
        {
            get { return _useHoursAndMinutes; }
            set { SetProperty(ref _useHoursAndMinutes, value); }
        }

        #endregion

        #region Selected Dates

        private DateTime _startDate;

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate == value) return;
                _startDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _endDate;

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate == value) return;
                _endDate = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Checkbox Properties

        private bool _startDateUsed;

        public bool StartDateUsed
        {
            get { return _startDateUsed; }
            set
            {
                if (_startDateUsed == value) return;
                _startDateUsed = value;
                OnPropertyChanged();
            }
        }

        private bool _endDateUsed;

        public bool EndDateUsed
        {
            get { return _endDateUsed; }
            set
            {
                if (_endDateUsed == value) return;
                _endDateUsed = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Selected Times

        private TimeSpan _startTime = new TimeSpan(0, 0, 0);

        public TimeSpan StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime == value) return;
                _startTime = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _endTime = new TimeSpan(0, 0, 0);

        public TimeSpan EndTime
        {
            get { return _endTime; }
            set
            {
                if (_endTime == value) return;
                _endTime = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public bool Match(DateTime dateTimeToCompare)
        {
            if (_appliedStartTime.HasValue && DateTime.Compare(dateTimeToCompare, _appliedStartTime.Value) < 0)
            {
                return false;
            }

            if (_appliedEndTime.HasValue && DateTime.Compare(dateTimeToCompare, _appliedEndTime.Value) >= 0)
            {
                return false;
            }

            return true;
        }

        #region Override

        private DateTime? _appliedStartTime;
        private DateTime? _appliedEndTime;

        public override bool Apply()
        {
            if (StartDateUsed)
            {
                _appliedStartTime = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, UseHoursAndMinutes ? StartTime.Hours : 0, UseHoursAndMinutes ? StartTime.Minutes : 0, 0);
            }
            else
            {
                _appliedStartTime = null;
            }

            if (EndDateUsed)
            {
                _appliedEndTime = new DateTime(EndDate.Year, EndDate.Month, UseHoursAndMinutes ? EndDate.Day : EndDate.Day + 1, UseHoursAndMinutes ? EndTime.Hours : 0, UseHoursAndMinutes ? EndTime.Minutes + 1 : 0, 0);
            }
            else
            {
                _appliedEndTime = null;
            }

            return _appliedStartTime.HasValue || _appliedEndTime.HasValue;
        }

        public override bool Validate(T model)
        {
            var dateTime = _predicate(model);
            return dateTime != null && Match(dateTime.Value);
        }

        public override void UpdatePossibleValues()
        {

        }

        public override void Clear()
        {
            StartDateUsed = false;
            EndDateUsed = false;

            _appliedStartTime = null;
            _appliedEndTime = null;
        }

        #endregion
    }
}
