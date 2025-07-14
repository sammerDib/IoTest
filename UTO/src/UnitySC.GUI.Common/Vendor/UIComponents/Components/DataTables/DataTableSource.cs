using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Facades;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Search;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables
{
    public class DataTableSource<T> : DataTableSourceListFacade<T>, IDataTableSource, IDisposable
    {
        #region Implmentation of IGridViewSource

        ICommand IDataTableSource.SortCommand => SortCommand;

        ICollection IDataTableSource.SourceView => SourceView;

        FilterEngine IDataTableSource.Filter => Filter;

        ISortEngine IDataTableSource.Sort => Sort;

        ISearchEngine IDataTableSource.Search => Search;

        #endregion

        /// <summary>
        /// Get initial collection used for sorting and filtering before display
        /// </summary>
        protected override List<T> SourceList { get; } = new List<T>();

        public FilterEngine<T> Filter { get; } = new FilterEngine<T>();

        /// <summary>
        /// Allow to the view to easily reorder elements according to one of the <see cref="SortDefinition"/>.
        /// </summary>
        public SortEngine<T> Sort { get; } = new SortEngine<T>();

        private ObservableCollection<T> _sourceView = new ObservableCollection<T>();

        /// <summary>
        /// Collection of visible items after sorting and filtering
        /// </summary>
        public ObservableCollection<T> SourceView
        {
            get { return _sourceView; }
            protected set { SetAndRaiseIfChanged(ref _sourceView, value); }
        }

        /// <summary>
        /// Allow the view to initiate searches from a string and the descriptive search elements.
        /// </summary>
        public SearchEngine<T> Search { get; } = new SearchEngine<T>();

        static DataTableSource()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(DataTableResources)));
        }

        /// <summary>
        /// Instantiates a generic implementation of the IDataTableSource
        /// </summary>
        public DataTableSource()
        {
            SortCommand = new DelegateCommand<string>(SortCommandExecute, SortCommandCanExecute);
            Filter.ApplyFilter += InternalApplyFilter;
            Search.ApplySearch += InternalApplySearch;
        }

        /// <summary>
        /// Applies in order: sort, filter, then search before notifying the view of the new collection.
        /// This method must be explicitly called to refresh the display when the source collection has changed.
        /// </summary>
        public override void UpdateCollection()
        {
            Filter.UpdatePossibleValue();
            _sortedSource = ApplySort(SourceList);
            InternalApplyFilter();
        }

        private void InternalApplyFilter(object sender = null, EventArgs eventArgs = null)
        {
            _filteredSource = ApplyFilter(_sortedSource);
            InternalApplySearch();
        }

        private void InternalApplySearch(object sender = null, EventArgs e = null)
        {
            SourceView = _filteredSource == null ?
                new ObservableCollection<T>() :
                new ObservableCollection<T>(ApplySearch(_filteredSource));
        }

        private IEnumerable<T> _sortedSource;
        private IEnumerable<T> _filteredSource;
        private bool _disposedValue;

        /// <summary>
        /// Apply the sort to the source collection.
        /// </summary>
        /// <param name="initialCollection">The collection containing all of the source elements.</param>
        /// <returns>A collection containing all the sorted elements passed as the parameter.</returns>
        protected virtual IEnumerable<T> ApplySort(IEnumerable<T> initialCollection)
        {
            return Sort.GetAll(initialCollection);
        }

        /// <summary>
        /// Apply the filter to the sorted collection.
        /// </summary>
        /// <param name="sortedCollection">The sorted collection containing all of the source elements.</param>
        /// <returns>A filtered collection of elements passed as parameters.</returns>
        protected virtual IEnumerable<T> ApplyFilter(IEnumerable<T> sortedCollection)
        {
            return Filter.Apply(sortedCollection);
        }

        /// <summary>
        /// Apply the search to the filtered collection.
        /// </summary>
        /// <param name="filteredCollection">The filtered collection of the source elements.</param>
        /// <returns>A collection containing the elements passed in parameter compatible with the active search.</returns>
        protected virtual IEnumerable<T> ApplySearch(IEnumerable<T> filteredCollection)
        {
            return Search.Apply(filteredCollection);
        }

        #region Sort Command

        public DelegateCommand<string> SortCommand { get; }

        private bool SortCommandCanExecute(string propertyName)
        {
            return Sort.SortDefinitions.Any(definition => definition.PropertyName == propertyName);
        }

        private void SortCommandExecute(string propertyName)
        {
            Sort.SetCurrentSorting(propertyName);
            UpdateCollection();
        }

        #endregion

        #region IDisposable
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {

                    Filter.ApplyFilter -= InternalApplyFilter;
                    Search.ApplySearch -= InternalApplySearch;
                    Search.Dispose();
                }
                _disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
