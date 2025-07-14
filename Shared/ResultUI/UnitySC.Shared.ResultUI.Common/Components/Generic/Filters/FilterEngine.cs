using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.ResultUI.Common.Components.Generic.Filters
{
    /// <summary>
    /// Base class of Panel to manage a generic collection of filter
    /// </summary>
    public abstract class FilterEngine : ObservableObject
    {
        #region Apply Command

        private ICommand _applyFiltersCommand;

        /// <summary>
        /// Gets the apply filters command
        /// </summary>
        public ICommand ApplyFiltersCommand => _applyFiltersCommand = _applyFiltersCommand ?? new AutoRelayCommand(ApplyFiltersCommandExecute);

        public void ApplyFiltersCommandExecute()
        {
            bool isActive = false;
            foreach (var filter in InternalFilters)
            {
                if (filter.Apply())
                {
                    isActive = true;
                }
            }
            IsApplied = isActive;
            OnApplyFilter();
            IsOpen = false;
        }

        #endregion

        #region Clear Command

        private ICommand _clearFiltersCommand;

        /// <summary>
        /// Gets the clear filters command
        /// </summary>
        public ICommand ClearFiltersCommand => _clearFiltersCommand = _clearFiltersCommand ?? new AutoRelayCommand(ClearFiltersCommandExecute);

        private void ClearFiltersCommandExecute()
        {
            IsApplied = false;
            foreach (var filter in InternalFilters)
            {
                filter.Clear();
            }

            ApplyFiltersCommandExecute();
        }

        #endregion

        #region Properties

        private bool _isOpen;

        /// <summary>
        /// Gets / Sets if the filter panel is open
        /// </summary>
        public bool IsOpen
        {
            get { return _isOpen; }
            set { SetProperty(ref _isOpen, value); }
        }

        private bool _isApplied;

        /// <summary>
        /// Gets if the filters are enabled
        /// </summary>
        public bool IsApplied
        {
            get { return _isApplied; }
            protected set { SetProperty(ref _isApplied, value); }
        }

        protected abstract IEnumerable<Filter> InternalFilters { get; }

        /// <summary>
        /// Get the filter collection.
        /// </summary>
        public abstract INotifyCollectionChanged Collection { get; }

        #endregion

        #region Apply Event

        /// <summary>
        /// Event raised when the user applies a modification of the filters.
        /// </summary>
        public event EventHandler ApplyFilter;

        private void OnApplyFilter() => ApplyFilter?.Invoke(this, EventArgs.Empty);

        #endregion
    }

    /// <summary>
    /// Panel to manage a generic collection of filter for a specified type
    /// </summary>
    /// <typeparam name="T">Type of model to filter</typeparam>
    public class FilterEngine<T> : FilterEngine
    {
        public FilterEngine()
        {
            Collection = new ReadOnlyObservableCollection<Filter<T>>(Filters);
        }

        #region Fields

        public ObservableCollection<Filter<T>> Filters { get; } = new ObservableCollection<Filter<T>>();

        #endregion

        #region Properties

        protected override IEnumerable<Filter> InternalFilters => Filters;

        /// <summary>
        /// Get the <see cref="Filter"/> collection as a ReadOnly list.
        /// </summary>
        public override INotifyCollectionChanged Collection { get; }

        #endregion

        /// <summary>
        /// Get a reduced list of elements after all filters are applied.
        /// All filters must have a predicate defined
        /// </summary>
        /// <param name="models">List of all elements to filter</param>
        /// <returns>Filtered list of element</returns>
        public IEnumerable<T> Apply(IEnumerable<T> models)
        {
            return !IsApplied ? models : models.Where(model => Filters.All(filter => filter.Validate(model)));
        }

        public bool Match(T model)
        {
            return !IsApplied || Filters.All(filter => filter.Validate(model));
        }

        /// <summary>
        /// Update the possible value collection of all filters
        /// </summary>
        public void UpdatePossibleValue()
        {
            foreach (var filter in Filters)
            {
                filter.UpdatePossibleValues();
            }
        }

        /// <summary>
        /// Add a filter to the collection
        /// </summary>
        /// <param name="filter">The filter to add</param>
        public void Add(Filter<T> filter)
        {
            Filters.Add(filter);
        }

        /// <summary>
        /// Generates a filter containing all the values of the specified enum.
        /// </summary>
        /// <typeparam name="TE">Type of the enum</typeparam>
        /// <param name="name">Name to display</param>
        /// <param name="getEnumValue">Get the enum value from the <see cref="T"/> model</param>
        /// <returns>The filter instance</returns>
        public FilterCollection<T, TE> AddEnumFilter<TE>(string name, Func<T, TE> getEnumValue) where TE : Enum
        {
            var filter = new FilterCollection<T, TE>(name, () => Enum.GetValues(typeof(TE)).Cast<TE>(), getEnumValue);
            Add(filter);
            return filter;
        }

        /// <summary>
        /// Generates a filter for a range of values automatically displaying the min & max values contained in the targeted collection.
        /// </summary>
        /// <typeparam name="TE">Type of the property</typeparam>
        /// <param name="name">Name to display</param>
        /// <param name="getValue">Get the value from the <see cref="T"/> model</param>
        /// <param name="getCollectionSource">Get the collection for display min and max values</param>
        /// <returns>The filter instance</returns>
        public FilterRange<T, TE> AddRangeFilter<TE>(string name, Func<T, TE> getValue, Func<ICollection<T>> getCollectionSource) where TE : struct, IComparable
        {
            var filter = new FilterRange<T, TE>(name, getValue,
                () =>
                {
                    var collectionSource = getCollectionSource();
                    var emptyCollection = collectionSource == null || collectionSource.Count == 0;
                    return emptyCollection ? default(TE) : collectionSource.Min(getValue);
                },
                () =>
                {
                    var collectionSource = getCollectionSource();
                    var emptyCollection = collectionSource == null || collectionSource.Count == 0;
                    return emptyCollection ? default(TE) : collectionSource.Max(getValue);
                });
            Add(filter);
            return filter;
        }
    }
}
