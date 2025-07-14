using System;
using System.Collections;
using System.Linq;
using System.Windows.Input;

using Agileo.Common.Localization;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Collection;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Facades;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Filters;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Search;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables
{
    public class RealTimeDataTableSource<T> : RealtimeDataTableSourceListFacade<T>, IDataTableSource, IDisposable
    {
        #region Implmentation of IGridViewSource

        ICommand IDataTableSource.SortCommand => null;

        ICollection IDataTableSource.SourceView => SourceView;

        FilterEngine IDataTableSource.Filter => Filter;

        ISortEngine IDataTableSource.Sort => null;

        ISearchEngine IDataTableSource.Search => Search;

        #endregion

        public FilterEngine<T> Filter { get; } = new FilterEngine<T>();

        /// <summary>
        /// Allow the view to initiate searches from a string and the descriptive search elements.
        /// </summary>
        public SearchEngine<T> Search { get; } = new SearchEngine<T>();

        public TimedBindingList<T> SourceView { get; } = new TimedBindingList<T>();

        static RealTimeDataTableSource()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(DataTableResources)));
        }

        /// <summary>
        /// Instantiates a generic implementation of the IDataTableSource
        /// </summary>
        public RealTimeDataTableSource()
        {
            Filter.ApplyFilter += ApplyFilterAndSearch;
            Search.ApplySearch += ApplyFilterAndSearch;
        }

        private void ApplyFilterAndSearch(object sender = null, EventArgs e = null)
        {
            lock (LockEnumerator)
            {
                var matchItems = SourceList.Where(item => Filter.Match(item) && Search.Match(item));
                SourceView.Reset(matchItems);
            }
        }

        /// <summary>
        /// Updates the possible values of the filters.
        /// </summary>
        public void UpdateFilterPossibleValues()
        {
            lock (LockEnumerator)
            {
                Filter.UpdatePossibleValue();
            }
        }

        #region Overrides of RealtimeDataTableSourceListFacade<T>

        protected override void OnItemAdded(T item)
        {
            if (Filter.Match(item) && Search.Match(item))
            {
                SourceView.AddInCacheList(item);
            }
        }

        protected override void OnItemRemoved(T item)
        {
            SourceView.RemoveFromCache(item);
        }

        protected override void OnCleared()
        {
            SourceView.ClearAll();
        }

        protected override void OnReset()
        {
            ApplyFilterAndSearch();
        }

        #endregion

        #region IDisposable
        
        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {

                    Filter.ApplyFilter -= ApplyFilterAndSearch;
                    Search.ApplySearch -= ApplyFilterAndSearch;

                    Search?.Dispose();
                    SourceView?.Dispose();
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
