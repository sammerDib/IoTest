using System;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters
{
    public interface IFilterRange : IFilter
    {
        string MinimumValue { get; set; }

        string MaximumValue { get; set; }

        string TheoreticalMinimumValue { get; set; }

        string TheoreticalMaximumValue { get; set; }
    }

    /// <summary>
    /// Filter with a minimum and maximum values.
    /// </summary>
    /// <typeparam name="TM">Type of model to filter</typeparam>
    /// <typeparam name="TP">Type of the property to filter</typeparam>
    public class FilterRange<TM, TP> : Filter<TM>, IFilterRange where TP : struct, IComparable
    {
        private readonly Func<TM, TP> _getValueFunc;
        private readonly Func<TP> _getMinimumValue;
        private readonly Func<TP> _getMaximumValue;

        public FilterRange(IText name, Func<TM, TP> getValueFunc, Func<TP> getMinimumValue = null, Func<TP> getMaximumValue = null) : base(name)
        {
            _getValueFunc = getValueFunc;
            _getMinimumValue = getMinimumValue;
            _getMaximumValue = getMaximumValue;
        }

        #region Overrides of Filter

        public override void Clear()
        {
            MinimumValue = null;
            MaximumValue = null;
            _appliedMinimumValue = null;
            _appliedMaximumValue = null;
        }

        #endregion

        #region Overrides of Filter<T>

        private TP? _appliedMinimumValue;
        private TP? _appliedMaximumValue;

        public override bool Apply()
        {
            _appliedMinimumValue = _minimumValue;
            _appliedMaximumValue = _maximumValue;
            return _minimumValue.HasValue || _maximumValue.HasValue;
        }

        public override bool Validate(TM model)
        {
            if (!_appliedMinimumValue.HasValue && !_appliedMaximumValue.HasValue) return true;

            var value = _getValueFunc(model);

            if (_appliedMinimumValue.HasValue && _appliedMinimumValue.Value.CompareTo(value) > 0)
            {
                return false;
            }
            if (_appliedMaximumValue.HasValue && value.CompareTo(_appliedMaximumValue.Value) > 0)
            {
                return false;
            }

            return true;
        }

        public override void UpdatePossibleValues()
        {
            if (_getMinimumValue != null)
            {
                TheoreticalMinimumValue = _getMinimumValue().ToString();
            }
            if (_getMaximumValue != null)
            {
                TheoreticalMaximumValue = _getMaximumValue().ToString();
            }
        }

        #endregion

        #region Properties

        private string _theoreticalMinimumValue;

        public string TheoreticalMinimumValue
        {
            get { return _theoreticalMinimumValue; }
            set { SetAndRaiseIfChanged(ref _theoreticalMinimumValue, value); }
        }

        private string _theoreticalMaximumValue;

        public string TheoreticalMaximumValue
        {
            get { return _theoreticalMaximumValue; }
            set { SetAndRaiseIfChanged(ref _theoreticalMaximumValue, value); }
        }

        #endregion

        #region Implementation of IFilterRange

        private TP? _minimumValue;

        public string MinimumValue
        {
            get { return _minimumValue.HasValue ? _minimumValue.ToString() : string.Empty; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _minimumValue = null;
                }
                else
                {
                    _minimumValue = (TP)Convert.ChangeType(value, typeof(TP));
                }
                OnPropertyChanged();
            }
        }

        private TP? _maximumValue;

        public string MaximumValue
        {
            get { return _maximumValue.HasValue ? _maximumValue.ToString() : string.Empty; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _maximumValue = null;
                }
                else
                {
                    _maximumValue = (TP)Convert.ChangeType(value, typeof(TP));
                }
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
