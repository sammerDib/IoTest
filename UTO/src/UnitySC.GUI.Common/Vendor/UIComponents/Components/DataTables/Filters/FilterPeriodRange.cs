using System;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters
{
    /// <summary>
    /// Generic filter allowing to filter a period which must be strictly contained in another.
    /// </summary>
    /// <typeparam name="T">Type of model to filter</typeparam>
    public class FilterPeriodRange<T> : FilterPeriod<T>
    {
        private readonly Func<T, (DateTime? startTime, DateTime? endTime)> _getPeriodFunc;

        public FilterPeriodRange(
            LocalizableText name,
            Func<T, (DateTime? startTime, DateTime? endTime)> getPeriodFunc)
            : this((IText)name, getPeriodFunc)
        {
        }

        public FilterPeriodRange(
            IText name,
            Func<T, (DateTime? startTime, DateTime? endTime)> getPeriodFunc)
            : base(name, null)
        {
            _getPeriodFunc = getPeriodFunc;
        }

        #region Overrides of FilterPeriod<T>

        public override bool Validate(T model)
        {
            var (startTime, endTime) = _getPeriodFunc(model);
            if (startTime == null || endTime == null) return false;
            return Match(startTime.Value, endTime.Value);
        }

        #endregion

        public bool Match(DateTime startDateTime, DateTime endDateTime)
        {
            if (AppliedStartTime.HasValue && AppliedEndTime.HasValue)
            {
                return startDateTime.Ticks <= AppliedEndTime.Value.Ticks
                       && endDateTime.Ticks >= AppliedStartTime.Value.Ticks;
            }

            if (AppliedStartTime == null && AppliedEndTime.HasValue)
            {
                return startDateTime.Ticks <= AppliedEndTime.Value.Ticks;
            }

            if (AppliedStartTime.HasValue)
            {
                return endDateTime.Ticks >= AppliedStartTime.Value.Ticks;
            }

            return true;
        }
    }
}
