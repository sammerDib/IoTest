using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.Shared.UI.ViewModel.AdvancedGridView
{
    public abstract class SortEngine : ObservableObject
    {
        public abstract SortDefinition CurrentSortDefinition { get; protected set; }

        public void SetCurrentSorting(SortDefinition sort)
        {
            if (CurrentSortDefinition == sort)
            {
                CurrentSortDefinition.RevertDirection();
            }
            else
            {
                CurrentSortDefinition = sort;
            }
        }
    }

    public class SortEngine<T> : SortEngine
    {
        #region Fields

        private readonly List<SortDefinition<T>> _definitions = new List<SortDefinition<T>>();

        #endregion

        #region Properties

        /// <summary>
        /// Get the <see cref="SortDefinition"/> collection as a ReadOnly list.
        /// </summary>
        public IReadOnlyList<SortDefinition<T>> Definitions => _definitions.AsReadOnly();

        #endregion

        #region Override

        private SortDefinition<T> _currentSortDefinition;

        /// <summary>
        /// Get the current <see cref="SortDefinition"/>.
        /// </summary>
        public override SortDefinition CurrentSortDefinition
        {
            get { return _currentSortDefinition; }
            protected set
            {
                if (_currentSortDefinition == value) return;
                foreach (var definition in _definitions)
                {
                    definition.IsActive = false;
                }

                _currentSortDefinition = (SortDefinition<T>)value;
                _currentSortDefinition.IsActive = true;

                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        public SortDefinition<T> AddSortDefinition(Func<T, object> predicate)
        {
            var sortDefinition = new SortDefinition<T>(predicate);
            Add(sortDefinition);
            return sortDefinition;
        }

        public void Add(SortDefinition<T> definition)
        {
            _definitions.Add(definition);
            if (CurrentSortDefinition == null)
            {
                CurrentSortDefinition = definition;
            }
        }

        /// <summary>
        /// Sorts the elements of the specified collection according to of the <see cref="CurrentSortDefinition"/>.
        /// </summary>
        public IEnumerable<T> GetAll(IEnumerable<T> initialCollection)
        {
            return _currentSortDefinition == null ? initialCollection : _currentSortDefinition.Sort(initialCollection);
        }

        #endregion

    }
}
