using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UnitySC.GUI.Common.Vendor.Helpers
{
    /// <summary>
    /// Adding Sorting Feature to Observable Collection
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Sorts the specified collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="comparison">The comparison.</param>
        internal static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var comparer = new Comparer<T>(comparison);
            var sorted = collection.OrderBy(x => x, comparer).ToList();
            for (var i = 0; i < sorted.Count; i++) collection.Move(collection.IndexOf(sorted[i]), i);
        }

        private sealed class Comparer<T> : IComparer<T>
        {
            private readonly Comparison<T> _comparison;

            public Comparer(Comparison<T> comparison)
            {
                _comparison = comparison;
            }

            #region IComparer<T> Members

            public int Compare(T x, T y)
            {
                return _comparison.Invoke(x, y);
            }

            #endregion
        }

        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> toAdd)
        {
            foreach (var element in toAdd.ToList())
            {
                collection.Add(element);
            }
        }

        public static void RemoveRange<T>(this ObservableCollection<T> collection, IEnumerable<T> toAdd)
        {
            foreach (var element in toAdd.ToList())
            {
                collection.Remove(element);
            }
        }
    }
}
