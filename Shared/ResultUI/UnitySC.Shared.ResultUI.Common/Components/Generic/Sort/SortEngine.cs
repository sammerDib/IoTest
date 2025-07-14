using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.ResultUI.Common.Components.Generic.Sort
{
    public interface ISortEngine
    {
        INotifyCollectionChanged SortDefinitions { get; }

        ICommand SetSortingCommand { get; }
    }

    public class SortEngine<T> : ObservableObject, ISortEngine
    {
        #region Implementation of ISortEngine

        INotifyCollectionChanged ISortEngine.SortDefinitions => SortDefinitions;

        ICommand ISortEngine.SetSortingCommand => SetSortingCommand;

        #endregion

        #region Fields

        private readonly ObservableCollection<SortDefinition> _observableSortDefinitions = new ObservableCollection<SortDefinition>();

        #endregion

        #region Commands

        private ICommand _setSortingCommand;

        public ICommand SetSortingCommand => _setSortingCommand = _setSortingCommand ?? new AutoRelayCommand<SortDefinition>(SetSortingCommandExecute, SetSortingCommandCanExecute);

        private bool SetSortingCommandCanExecute(SortDefinition definition) => definition != null;

        private void SetSortingCommandExecute(SortDefinition definition) => SetCurrentSorting(definition);

        #endregion

        #region Apply Event


        /// <summary>
        /// Event raised when the user applies a modification of the sorting.
        /// </summary>
        public event EventHandler Applied;

        private void OnSortApplied() => Applied?.Invoke(this, EventArgs.Empty);

        #endregion

        public SortEngine()
        {
            SortDefinitions = new ReadOnlyObservableCollection<SortDefinition>(_observableSortDefinitions);
        }
        
        #region Properties

        public ReadOnlyObservableCollection<SortDefinition> SortDefinitions { get; }

        private SortDefinition<T> _currentSortDefinition;

        /// <summary>
        /// Get the current <see cref="SortDefinition"/>.
        /// </summary>
        public SortDefinition CurrentSortDefinition
        {
            get { return _currentSortDefinition; }
            protected set
            {
                if (_currentSortDefinition == value) return;
                foreach (var definition in _observableSortDefinitions)
                {
                    definition.IsActive = false;
                }

                _currentSortDefinition = (SortDefinition<T>)value;
                _currentSortDefinition.IsActive = true;
                OnPropertyChanged();
            }
        }

        public SortDefinition<T> ConstantSortDefinition { get; protected set; }

        /// <summary>
        /// Indexer allowing the view to bind to the <see cref="SortDefinition"/> instance associated with the specified key.
        /// If no <see cref="SortDefinition"/> is found, try to generate a matching sortingDefinition using reflection.
        /// </summary>
        public SortDefinition this[string key] => GetSortDefinition(key);

        #endregion

        #region Private Methods

        private SortDefinition GetSortDefinition(string propertyName)
        {
            return _observableSortDefinitions.SingleOrDefault(definition => definition.PropertyName == propertyName) ?? AddDefaultSortingDefinition(propertyName);
        }

        /// <summary>
        /// Use reflection to instantiate a <see cref="SortDefinition"/>
        /// associated with the property with the same name as the <see cref="sortKey"/> as parameter.
        /// </summary>
        /// <param name="sortKey">Name of the property targeted for sorting</param>
        /// <returns>Generated <see cref="SortDefinition"/> instance</returns>
        private SortDefinition<T> AddDefaultSortingDefinition(string sortKey)
        {
            var members = sortKey.Split('.').ToList();

            var properties = new List<PropertyInfo>();
            var currentType = typeof(T);

            foreach (var member in members)
            {
                var property = currentType.GetProperty(member);
                if (property == null || !property.CanRead)
                {
                    throw new InvalidOperationException($"The member path {sortKey} is not accessible for type {typeof(T).FullName}");
                }
                properties.Add(property);
                currentType = property.PropertyType;
            }

            var definition = new SortDefinition<T>(sortKey, t =>
            {
                object currentObject = t;
                foreach (var propertyInfo in properties)
                {
                    currentObject = propertyInfo.GetValue(currentObject);
                }

                return currentObject;
            });
            Add(definition);
            return definition;
        }

        #endregion

        #region Public Methods

        public void SetCurrentSorting(string propertyName)
        {
            var nextDefinition = GetSortDefinition(propertyName);
            SetCurrentSorting(nextDefinition);
        }

        public void SetCurrentSorting(SortDefinition definition)
        {
            if (CurrentSortDefinition == definition)
            {
                CurrentSortDefinition.RevertDirection();
            }
            else
            {
                CurrentSortDefinition = definition;
            }

            OnSortApplied();
        }

        public void SetCurrentSorting(string propertyName, ListSortDirection direction)
        {
            var nextDefinition = GetSortDefinition(propertyName);
            nextDefinition.Direction = direction;
            CurrentSortDefinition = nextDefinition;
        }

        public SortEngine<T> AddSortDefinition(string propertyName, Func<T, object> predicate)
        {
            var sortDefinition = new SortDefinition<T>(propertyName, predicate);
            return Add(sortDefinition);
        }

        public SortEngine<T> Add(SortDefinition<T> definition)
        {
            _observableSortDefinitions.Add(definition);
            if (CurrentSortDefinition == null)
            {
                CurrentSortDefinition = definition;
            }

            return this;
        }

        /// <summary>
        /// Sorts the elements of the specified collection according to of the <see cref="CurrentSortDefinition"/>.
        /// </summary>
        public IEnumerable<T> GetAll(IEnumerable<T> initialCollection)
        {
            var sortedCollection = _currentSortDefinition == null ? initialCollection : _currentSortDefinition.Sort(initialCollection);
            return ConstantSortDefinition == null ? sortedCollection : ConstantSortDefinition.Sort(sortedCollection);
        }

        /// <summary>
        /// Defines a sort definition which will always be applied first, regardless of the active sort.
        /// </summary>
        public void DefineConstantSortingDefinition(SortDefinition<T> sortDefinition)
        {
            ConstantSortDefinition = sortDefinition;
        }

        #endregion
    }
}
