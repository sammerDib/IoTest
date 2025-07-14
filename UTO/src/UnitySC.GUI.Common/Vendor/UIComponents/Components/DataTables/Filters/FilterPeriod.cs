using System;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters
{
    public interface IFilterPeriod : IFilter
    {
        DateTime StartDate { get; set; }

        DateTime EndDate { get; set; }

        bool StartDateUsed { get; set; }

        bool EndDateUsed { get; set; }

        TimeSpan StartTime { get; set; }

        TimeSpan EndTime { get; set; }

        bool UseHoursMinutesSeconds { get; }
    }

    /// <summary>
    /// Generic filter for a temporal duration
    /// </summary>
    /// <typeparam name="T">Type of model to filter</typeparam>
    public class FilterPeriod<T> : Filter<T>, IFilterPeriod
    {
        private readonly Func<T, DateTime?> _getDateTimeFunc;

        public FilterPeriod(LocalizableText name, Func<T, DateTime?> getDateTimeFunc) : this((IText)name, getDateTimeFunc)
        {
        }

        public FilterPeriod(IText name, Func<T, DateTime?> getDateTimeFunc) : base(name)
        {
            _getDateTimeFunc = getDateTimeFunc;

            StartDate = DateTime.Today.AddDays(-7);
            EndDate = DateTime.Today.AddDays(1);
        }

        #region Use Hours and Minutes

        bool IFilterPeriod.UseHoursMinutesSeconds => UseHoursMinutesSeconds;

        private bool _useHoursMinutesSeconds;

        public bool UseHoursMinutesSeconds
        {
            get { return _useHoursMinutesSeconds; }
            set { SetAndRaiseIfChanged(ref _useHoursMinutesSeconds, value); }
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
                OnPropertyChanged(nameof(StartDate));
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
                OnPropertyChanged(nameof(EndDate));
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
                OnPropertyChanged(nameof(StartDateUsed));
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
                OnPropertyChanged(nameof(EndDateUsed));
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
                OnPropertyChanged(nameof(StartTime));
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
                OnPropertyChanged(nameof(EndTime));
            }
        }

        #endregion

        public bool Match(DateTime dateTimeToCompare)
        {
            if (AppliedStartTime.HasValue && DateTime.Compare(dateTimeToCompare, AppliedStartTime.Value) < 0)
            {
                return false;
            }

            if (AppliedEndTime.HasValue && DateTime.Compare(dateTimeToCompare, AppliedEndTime.Value) >= 0)
            {
                return false;
            }

            return true;
        }

        #region Override

        protected DateTime? AppliedStartTime;
        protected DateTime? AppliedEndTime;

        public override bool Apply()
        {
            if (StartDateUsed)
            {
                AppliedStartTime = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, UseHoursMinutesSeconds ? StartTime.Hours : 0, UseHoursMinutesSeconds ? StartTime.Minutes : 0, UseHoursMinutesSeconds ? StartTime.Seconds : 0);
            }
            else
            {
                AppliedStartTime = null;
            }

            if (EndDateUsed)
            {
                AppliedEndTime = new DateTime(EndDate.Year, EndDate.Month, UseHoursMinutesSeconds ? EndDate.Day : EndDate.Day + 1, UseHoursMinutesSeconds ? EndTime.Hours : 0, UseHoursMinutesSeconds ? EndTime.Minutes : 0, UseHoursMinutesSeconds ? EndTime.Seconds : 0);
            }
            else
            {
                AppliedEndTime = null;
            }

            return AppliedStartTime.HasValue || AppliedEndTime.HasValue;
        }

        public override bool Validate(T model)
        {
            var dateTime = _getDateTimeFunc(model);
            return dateTime != null && Match(dateTime.Value);
        }

        public override void UpdatePossibleValues()
        {
        }

        public override void Clear()
        {
            StartDateUsed = false;
            EndDateUsed = false;

            AppliedStartTime = null;
            AppliedEndTime = null;
        }

        #endregion
    }
}
