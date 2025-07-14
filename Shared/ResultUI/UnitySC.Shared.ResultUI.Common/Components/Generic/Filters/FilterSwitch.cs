using System;

namespace UnitySC.Shared.ResultUI.Common.Components.Generic.Filters
{
    public interface IFilterSwitch : IFilter
    {
        bool IsEnabled { get; set; }

        bool IsConstant { get; set; }
    }

    /// <summary>
    /// Generic filter for boolean value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilterSwitch<T> : Filter<T>, IFilterSwitch
    {
        public bool IsConstant { get; set; }

        private readonly Func<T, bool> _predicate;

        public FilterSwitch(string name, Func<T, bool> predicate) : base(name)
        {
            _predicate = predicate;
        }

        #region Overrides of Filter

        public override void Clear()
        {
            if (IsConstant) return;

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

        public override bool Validate(T model) => _appliedValue && _predicate(model);

        public override void UpdatePossibleValues()
        {
        }

        #endregion

        #region Implementation of IFilterSwitch

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        #endregion
    }
}
