using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.Shared.ResultUI.Common.Components.Generic.Search
{
    /// <summary>
    /// Interface used by <see cref="SearchBar"/> control.
    /// </summary>
    public interface ISearchEngine
    {
        string SearchText { get; set; }

        INotifyCollectionChanged SearchDefinitions { get; }
    }

    /// <summary>
    /// Generic implementation of <see cref="ISearchEngine"/>.
    /// </summary>
    /// <typeparam name="T">Type of model to search</typeparam>
    public class SearchEngine<T> : ObservableObject, ISearchEngine, IDisposable
    {
        #region Implementation of ISearchEngine

        INotifyCollectionChanged ISearchEngine.SearchDefinitions => SearchDefinitions;

        #endregion

        #region Properties

        private string _searchText;

        /// <summary>
        /// The regular expression pattern to match.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetProperty(ref _searchText, value);
                try
                {
                    _regex = value != null
                        ? new Regex(_searchText, RegexOptions.IgnoreCase)
                        : null;
                }
                catch (Exception)
                {
                    _regex = null;
                }
                OnApplySearch();
            }
        }

        private Regex _regex = new Regex(string.Empty, RegexOptions.IgnoreCase);

        #endregion

        public ReadOnlyObservableCollection<SearchDefinition<T>> SearchDefinitions { get; }

        private readonly ObservableCollection<SearchDefinition<T>> _observableSearchDefinitions = new ObservableCollection<SearchDefinition<T>>();

        public SearchEngine()
        {
            SearchDefinitions = new ReadOnlyObservableCollection<SearchDefinition<T>>(_observableSearchDefinitions);
        }

        #region Apply event

        public event EventHandler ApplySearch;
        protected void OnApplySearch() => ApplySearch?.Invoke(this, EventArgs.Empty);

        #endregion

        /// <summary>
        /// Get a reduced list of elements after search are applied.
        /// All <see cref="SearchDefinition"/> must have a predicate defined
        /// </summary>
        /// <param name="models">List of all elements to filter</param>
        /// <returns>Filtered list of element</returns>
        public IEnumerable<T> Apply(IEnumerable<T> models)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return models;
            if (_regex == null) return new List<T>();

            var activeDefinitions = _observableSearchDefinitions.Where(definition => definition.IsSelected);
            return models.Where(model => activeDefinitions.Any(searchDefinition => searchDefinition.Validate(model, _regex)));
        }

        public bool Match(T model)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return true;
            if (_regex == null) return false;

            var activeDefinitions = _observableSearchDefinitions.Where(definition => definition.IsSelected);
            return activeDefinitions.Any(searchDefinition => searchDefinition.Validate(model, _regex));
        }

        public SearchDefinition<T> AddSearchDefinition(string displayName, Func<T, string> getComparableStringFunc, bool selectedByDefault = false)
        {
            var searchDefinition = new SearchDefinition<T>(displayName, getComparableStringFunc)
            {
                IsSelected = selectedByDefault
            };
            AddSearchDefinition(searchDefinition);
            return searchDefinition;
        }

        public void AddSearchDefinition(SearchDefinition<T> searchDefinition)
        {
            if (_observableSearchDefinitions.Count == 0)
            {
                searchDefinition.IsSelected = true;
            }
            _observableSearchDefinitions.Add(searchDefinition);
            searchDefinition.PropertyChanged += SearchDefinitionOnPropertyChanged;
        }

        private void SearchDefinitionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchDefinition.IsSelected))
            {
                OnApplySearch();
            }
        }

        #region IDisposable

        public void Dispose()
        {
            foreach (var searchDefinition in _observableSearchDefinitions)
            {
                searchDefinition.PropertyChanged -= SearchDefinitionOnPropertyChanged;
                searchDefinition.Dispose();
            }
        }

        #endregion
    }
}
