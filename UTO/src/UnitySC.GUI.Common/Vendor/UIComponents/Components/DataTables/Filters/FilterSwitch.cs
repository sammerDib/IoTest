using System;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters
{
    public interface IFilterSwitch : IFilter
    {
        bool IsEnabled { get; set; }
    }

    /// <summary>
    /// Generic filter for boolean value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilterSwitch<T> : Filter<T>, IFilterSwitch
    {
        private readonly Func<T, bool> _predicate;

        public FilterSwitch(LocalizableText name, Func<T, bool> predicate) : this((IText)name, predicate)
        {
        }

        public FilterSwitch(IText name, Func<T, bool> predicate) : base(name)
        {
            _predicate = predicate;
        }

        #region Overrides of Filter

        public override void Clear()
        {
            IsEnabled = false;
            _appliedValue = false;
        }

        #endregion

        #region Overrides of Filter<T>

        private bool _appliedValue;

        public override bool Apply()
        {
            _appliedValue = IsEnabled;
            return _appliedValue;
        }

        // When the switch is deactivated returns true otherwise returns the value of the predicate.
        public override bool Validate(T model) => !_appliedValue || _predicate(model);

        public override void UpdatePossibleValues()
        {
        }

        #endregion

        #region Implementation of IFilterSwitch

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetAndRaiseIfChanged(ref _isEnabled, value);
        }

        #endregion
    }
}
