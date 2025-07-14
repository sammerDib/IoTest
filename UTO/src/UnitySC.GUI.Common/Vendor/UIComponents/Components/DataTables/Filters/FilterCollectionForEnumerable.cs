using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters
{
    /// <summary>
    /// Filter with a list of possible values selectable by the user.
    /// This filter is applied by comparing the content of a collection of type <see cref="TP"/>.
    /// </summary>
    /// <typeparam name="TM">Type of model to filter</typeparam>
    /// <typeparam name="TP">Generic type of the collection to filter.</typeparam>
    public class FilterCollectionForEnumerable<TM, TP> : FilterCollection<TM, TP>
    {
        public enum FilterCollectionType
        {
            Any,
            All
        }

        private readonly Func<TM, IEnumerable<TP>> _getValuesFunc;

        public FilterCollectionForEnumerable(
            IText name,
            Func<IEnumerable<TP>> possibleValues,
            Func<TM, IEnumerable<TP>> getValuesFunc)
            : base(name, possibleValues, null)
        {
            _getValuesFunc = getValuesFunc;
            UpdatePossibleValues();
        }

        public FilterCollectionForEnumerable(
            LocalizableText name,
            Func<IEnumerable<TP>> possibleValues,
            Func<TM, IEnumerable<TP>> getValuesFunc)
            : this((IText)name, possibleValues, getValuesFunc)
        {
        }

        #region Properties

        public FilterCollectionType FilterType { get; set; } = FilterCollectionType.Any;

        #endregion

        #region Override

        /// <summary>
        /// Gets if the value can be used on filter application
        /// </summary>
        /// <param name="model">Model to validate</param>
        public override bool Validate(TM model)
        {
            if (AppliedValues.Count == 0) return true;
            var values = _getValuesFunc(model);

            if (values == null) return false;

            return FilterType switch
            {
                FilterCollectionType.Any => values.Any(Matches),
                FilterCollectionType.All => values.All(Matches),
                _ => false
            };
        }

        #endregion
    }
}
