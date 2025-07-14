using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

using Agileo.GUI.Components;

using Castle.Core.Internal;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters
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

        protected FilterCollection(IText name) : base(name)
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

        public FilterCollection(
            LocalizableText name,
            Func<IEnumerable<TP>> possibleValues,
            Func<TM, TP> getValueFunc)
            : this((IText)name, possibleValues, getValueFunc)
        {
        }

        public FilterCollection(
            IText name,
            Func<IEnumerable<TP>> possibleValues,
            Func<TM, TP> getValueFunc)
            : base(name)
        {
            _getValueFunc = getValueFunc;
            _getPossibleValues = possibleValues;
            UpdatePossibleValues();
        }

        #region Properties

        protected List<TP> AppliedValues { get; } = new();

        protected List<Regex> Regexs { get; private set; }

        public ObservableCollection<TP> SelectedItems { get; } = new();

        #endregion

        #region Override

        private IList _possibleValues;

        public override IList PossibleValues
        {
            get => _possibleValues;
            protected set => SetAndRaiseIfChanged(ref _possibleValues, value);
        }

        public sealed override void UpdatePossibleValues()
        {
            var possibleValues = _getPossibleValues();
            PossibleValues = possibleValues != null ? new List<TP>(possibleValues).OrderBy(x => x.ToString()).ToList() : null;
        }

        public override IList SelectedValues => SelectedItems;

        /// <summary>
        /// Gets if the value can be used on filter application
        /// </summary>
        /// <param name="model">Model to validate</param>
        public override bool Validate(TM model)
        {
            if (AppliedValues.Count == 0) return true;
            var value = _getValueFunc(model);
            return Matches(value);
        }

        /// <summary>
        /// Determines if the model passed as a parameter matches at least one of the selected values.
        /// </summary>
        /// <param name="value">Model to test</param>
        protected bool Matches(TP value)
        {
            return !Regexs.IsNullOrEmpty() && value is string str
                ? AppliedValues.Contains(value) || Regexs.Any(r => r.IsMatch(str))
                : AppliedValues.Contains(value);
        }

        #endregion

        #region Overrides of FilterViewModel

        public override void Clear()
        {
            SelectedValues.Clear();
            AppliedValues.Clear();
        }

        public override bool Apply()
        {
            AppliedValues.Clear();
            AppliedValues.AddRange(SelectedItems);

            //We check if the type of TP is string here cause we need to do string manipulations to handle the wildcard '*'
            if (typeof(TP) == typeof(string) && SelectedItems.Any(x => x.ToString().Contains('*')))
            {
                Regexs = new List<Regex>();

                var wildcardValues = SelectedItems.Where(x => x.ToString().Contains('*')).ToList();

                foreach (var splitValue in wildcardValues.Select(
                             wildcardValue => wildcardValue.ToString().Split('*')))
                {
                    Regexs.Add(new Regex($"^{splitValue[0]}.*{splitValue[1]}$"));
                }
            }

            return AppliedValues.Count > 0;
        }

        #endregion
    }
}
