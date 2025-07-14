using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UnitySC.Shared.ResultUI.Common.Components.Generic.Filters
{
    public interface IFilterCollection : IFilter
    {
        IList PossibleValues { get; }

        IList SelectedValues { get; }
    }

    /// <summary>
    /// Base class of generic filter with a list of possible values selectable by the user.
    /// </summary>
    /// <typeparam name="TM">Type of model to filter</typeparam>
    public abstract class FilterCollection<TM> : Filter<TM>, IFilterCollection
    {
        public abstract IList PossibleValues { get; protected set; }

        public abstract IList SelectedValues { get; }

        protected FilterCollection(string name) : base(name)
        {
        }
    }

    /// <summary>
    /// Filter with a list of possible values selectable by the user.
    /// </summary>
    /// <typeparam name="TM">Type of model to filter</typeparam>
    /// <typeparam name="TP">Type of the property to filter</typeparam>
    public class FilterCollection<TM, TP> : FilterCollection<TM>
    {
        private readonly Func<IEnumerable<TP>> _getPossibleValues;
        private readonly Func<TM, TP> _getValueFunc;
        private readonly List<TP> _appliedValues = new List<TP>();

        public FilterCollection(string name, Func<IEnumerable<TP>> possibleValues, Func<TM, TP> getValueFunc) : base(name)
        {
            _getValueFunc = getValueFunc;
            _getPossibleValues = possibleValues;
            UpdatePossibleValues();
        }
        
        public readonly ObservableCollection<TP> SelectedItems = new ObservableCollection<TP>();

        #region Override

        private IList _possibleValues;

        public override IList PossibleValues
        {
            get { return _possibleValues; }
            protected set
            {
                _possibleValues = value;
                OnPropertyChanged();
            }
        }

        public sealed override void UpdatePossibleValues()
        {
            var possibleValues = _getPossibleValues();
            PossibleValues = possibleValues != null ? new List<TP>(possibleValues) : null;
        }

        public override IList SelectedValues => SelectedItems;
        
        /// <summary>
        /// Gets if the value can be used on filter application
        /// </summary>
        /// <param name="model">Model to validate</param>
        public override bool Validate(TM model)
        {
            if (_appliedValues.Count == 0) return true;
            var value = _getValueFunc(model);
            return _appliedValues.Contains(value);
        }

        #endregion

        #region Overrides of FilterViewModel
        
        public override void Clear()
        {
            SelectedValues.Clear();
            _appliedValues.Clear();
        }

        public override bool Apply()
        {
            _appliedValues.Clear();
            _appliedValues.AddRange(SelectedItems);
            return _appliedValues.Any();
        }

        #endregion
    }
}
