using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.UI.ViewModel.AdvancedGridView
{
    public class DataTableSource<T> : DataTableSourceListFacade<T>
    {
        /// <summary>
        /// Get initial collection used for sorting and filtering before display
        /// </summary>
        protected override List<T> SourceList { get; } = new List<T>();

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
            private set { SetProperty(ref _sourceView, value); }
        }

        /// <summary>
        /// Instantiates a generic implementation of the IDataTableSource
        /// </summary>
        public DataTableSource()
        {
            SortCommand = new AutoRelayCommand<SortDefinition>(SortCommandExecute, SortCommandCanExecute);
        }

        /// <summary>
        /// Applies in order: sort, filter, then search before notifying the view of the new collection.
        /// This method must be explicitly called to refresh the display when the source collection has changed.
        /// </summary>
        public override void UpdateCollection()
        {
            _sortedSource = ApplySort(SourceList);

            SourceView = _sortedSource == null ?
                new ObservableCollection<T>() :
                new ObservableCollection<T>(_sortedSource);
        }

        private IEnumerable<T> _sortedSource;

        /// <summary>
        /// Apply the sort to the source collection.
        /// </summary>
        /// <param name="initialCollection">The collection containing all of the source elements.</param>
        /// <returns>A collection containing all the sorted elements passed as the parameter.</returns>
        protected virtual IEnumerable<T> ApplySort(IEnumerable<T> initialCollection)
        {
            return Sort.GetAll(initialCollection);
        }

        #region Sort Command

        public AutoRelayCommand<SortDefinition> SortCommand { get; }

        private bool SortCommandCanExecute(SortDefinition sort)
        {
            return sort != null && Sort.Definitions.Contains(sort);
        }

        private void SortCommandExecute(SortDefinition propertyName)
        {
            Sort.SetCurrentSorting(propertyName);
            UpdateCollection();
        }

        #endregion
    }
}
