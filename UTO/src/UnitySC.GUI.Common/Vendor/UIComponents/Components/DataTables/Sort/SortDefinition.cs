using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Sort
{
    public abstract class SortDefinition : Notifier
    {
        protected SortDefinition(string propertyName)
        {
            PropertyName = propertyName;
        }
        
        private ListSortDirection _direction = ListSortDirection.Ascending;

        public ListSortDirection Direction
        {
            get { return _direction; }
            set { SetAndRaiseIfChanged(ref _direction, value); }
        }

        public string PropertyName { get; }

        public LocalizableText DisplayName { get; protected set; }

        public abstract IEnumerable Sort(IEnumerable initialCollection);

        public void RevertDirection()
        {
            Direction = Direction == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;
        }

        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set { SetAndRaiseIfChanged(ref _isActive, value); }
        }
    }

    public class SortDefinition<T> : SortDefinition
    {
        private readonly Func<T, object> _predicate;

        public SortDefinition(string propertyName, Func<T, object> predicate) : base(propertyName)
        {
            _predicate = predicate;
        }

        public SortDefinition(LocalizableText localizableText, Func<T, object> predicate) : base(localizableText.Key)
        {
            DisplayName = localizableText;
            _predicate = predicate;
        }

        public override IEnumerable Sort(IEnumerable initialCollection)
        {
            return Sort((IEnumerable<T>)initialCollection);
        }

        public IEnumerable<T> Sort(IEnumerable<T> initialCollection)
        {
            return Direction == ListSortDirection.Ascending
                ? initialCollection.OrderBy(_predicate)
                : initialCollection.OrderByDescending(_predicate);
        }
    }
}
