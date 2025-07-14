using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.Core.KeyGesture;
using UnitySC.GUI.Common.Vendor.UIComponents.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Search
{
    /// <summary>
    /// Interface used by <see cref="SearchBar"/> control.
    /// </summary>
    public interface ISearchEngine : IKeyGestureHandler
    {
        string SearchText { get; set; }

        INotifyCollectionChanged SearchDefinitions { get; }

        bool IsFocused { get; set; }

        bool MatchCase { get; set; }

        event EventHandler ApplySearch;

        #region Methods

        IEnumerable<SearchEngineResult> Apply(string document);

        #endregion
    }

    public sealed class SearchEngineResult
    {
        public int StartOffset { get; set; }

        public int Length { get; set; }

        public Match Data { get; set; }
    }

    /// <summary>
    /// Generic implementation of <see cref="ISearchEngine"/>.
    /// </summary>
    /// <typeparam name="T">Type of model to search</typeparam>
    public class SearchEngine<T> : Notifier, ISearchEngine, IDisposable
    {
        #region Implementation of ISearchEngine

        INotifyCollectionChanged ISearchEngine.SearchDefinitions => SearchDefinitions;

        #endregion

        #region Fields

        private Regex _regex = new(string.Empty, RegexOptions.IgnoreCase);

        private readonly ObservableCollection<SearchDefinition<T>> _observableSearchDefinitions = new();

        #endregion

        #region Properties

        private string _searchText;

        /// <summary>
        /// The regular expression pattern to match.
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetAndRaiseIfChanged(ref _searchText, value);
                UpdateRegex();
            }
        }

        private bool _matchCase;

        public bool MatchCase
        {
            get => _matchCase;
            set
            {
                SetAndRaiseIfChanged(ref _matchCase, value);
                UpdateRegex();
            }
        }

        private bool _isFocused;

        public bool IsFocused
        {
            get => _isFocused;
            set => SetAndRaiseIfChanged(ref _isFocused, value);
        }

        public ReadOnlyObservableCollection<SearchDefinition<T>> SearchDefinitions { get; }

        #endregion

        public SearchEngine()
        {
            SearchDefinitions = new ReadOnlyObservableCollection<SearchDefinition<T>>(_observableSearchDefinitions);
        }

        #region Events

        public event EventHandler ApplySearch;

        protected void OnApplySearch() => ApplySearch?.Invoke(this, EventArgs.Empty);

        public event EventHandler SearchEnded;

        protected void OnSearchEnded() => SearchEnded?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Private methods

        private void UpdateRegex()
        {
            try
            {
                if (string.IsNullOrEmpty(_searchText))
                {
                    _regex = null;
                }
                else
                {
                    _regex = MatchCase ? new Regex(_searchText) : new Regex(_searchText, RegexOptions.IgnoreCase);
                }
            }
            catch (Exception)
            {
                _regex = null;
            }

            OnApplySearch();
        }

        #endregion

        #region Event handlers

        private void OnSearchDefinitionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchDefinition.IsSelected))
            {
                OnApplySearch();
            }
        }

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

        public IEnumerable<SearchEngineResult> Apply(string document)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return Enumerable.Empty<SearchEngineResult>();
            if (_regex == null) return Enumerable.Empty<SearchEngineResult>();

            int length = document.Length;
            var matches = new List<SearchEngineResult>();

            foreach (Match result in _regex.Matches(document))
            {
                int resultEndOffset = result.Length + result.Index;
                if (result.Index < 0 || resultEndOffset > length)
                {
                    continue;
                }

                matches.Add(new SearchEngineResult
                {
                    StartOffset = result.Index,
                    Length = result.Length,
                    Data = result
                });
            }

            return matches;
        }

        public bool Match(T model)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) return true;
            if (_regex == null) return false;

            var activeDefinitions = _observableSearchDefinitions.Where(definition => definition.IsSelected);
            return activeDefinitions.Any(searchDefinition => searchDefinition.Validate(model, _regex));
        }

        #region Search definitions

        public SearchDefinition<T> AddSearchDefinition(LocalizableText displayName, Func<T, string> getComparableStringFunc, bool selectedByDefault = false)
        {
            return AddSearchDefinition((IText)displayName, getComparableStringFunc, selectedByDefault);
        }

        public SearchDefinition<T> AddSearchDefinition(IText displayName, Func<T, string> getComparableStringFunc, bool selectedByDefault = false)
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
            searchDefinition.PropertyChanged += OnSearchDefinitionPropertyChanged;
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
                    foreach (var searchDefinition in _observableSearchDefinitions)
                    {
                        searchDefinition.PropertyChanged -= OnSearchDefinitionPropertyChanged;
                        searchDefinition.Dispose();
                    }
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

        #region Implementation of IKeyGestureHandler

        public bool OnKeyDown(KeyEventArgs keyEventArgs)
        {
            switch (keyEventArgs.Key)
            {
                case Key.Escape:
                case Key.Enter:
                    OnSearchEnded();
                    return true;
            }

            return false;
        }

        #endregion
    }
}
