using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace UnitySC.Shared.Tools.Collection
{
    public static class ListExtension
    {
        ///=================================================================
        ///<summary>
        /// Ajoute un item et retourne faux s'il est déjà présent dans la liste
        ///</summary>
        ///=================================================================
        public static bool TryAdd<T>(this IList<T> list, T elem)
        {
            bool alreadyPresent = list.Contains(elem);

            if (alreadyPresent)
            {
                return false;
            }
            else
            {
                list.Add(elem);
                return true;
            }
        }

        ///=================================================================<summary>
        /// Ajoute les éléments de la list2 qui ne sont pas déjà présent dans list
        ///</summary>=================================================================
        public static void UnionWith<T>(this IList<T> list, IEnumerable<T> list2)
        {
            foreach (var t in list2)
                list.TryAdd(t);
        }

        ///=================================================================<summary>
        /// Trie une liste 'list' pour que les éléments soient dans le même
        /// ordre que dans la liste 'order'.
        ///</summary>=================================================================
        public static void SortByList<T>(this List<T> list, List<T> order)
        {
            var ordered = list.OrderBy(b => order.FindIndex(a => a.Equals(b)));
            var orderedList = new List<T>(ordered);
            list.Clear();
            list.AddRange(orderedList);
        }

        //=================================================================
        // Supprime plusieurs éléments d'une liste
        //=================================================================
        public static void RemoveRange<T>(this IList<T> list, IEnumerable<T> remlist)
        {
            foreach (var t in remlist)
                list.Remove(t);
        }

        //=================================================================
        //
        //=================================================================
        public static bool IsEmpty<T>(this IList<T> list)
        {
            bool isEmpty = list.Count == 0;
            return isEmpty;
        }

        //=================================================================
        //
        //=================================================================
        public static void AddRange<T>(this ObservableCollection<T> coll, IEnumerable<T> list)
        {
            foreach (var t in list)
                coll.Add(t);
        }

        public static void RemoveAll<T>(this ObservableCollection<T> collection,
                                                            Func<T, bool> condition)
        {
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (condition(collection[i]))
                {
                    collection.RemoveAt(i);
                }
            }
        }

        public static void AddRange<K, V>(this Dictionary<K, V> dico, Dictionary<K, V> dico2)
        {
            foreach (var kvp in dico2)
                dico.Add(kvp.Key, kvp.Value);
        }

        //=================================================================
        //
        //=================================================================
        public static void Resize<T>(this List<T> list, int newsize)
        {
            int oldsize = list.Count;

            if (newsize < oldsize)
            {
                list.RemoveRange(newsize, oldsize - newsize);
            }
            else if (newsize > oldsize)
            {
                if (list.Capacity < newsize)
                    list.Capacity = newsize;

                for (int i = oldsize; i < newsize; i++)
                    list.Add(default(T));
            }
        }

        //=================================================================
        //
        //=================================================================
        public static void AddAndResize<T>(this List<T> list, int index, T t)
        {
            // Retaille si nécessaire
            //.......................
            if (list.Count <= index)
            {
                if (list.Capacity <= index)
                    list.Capacity = index + 1;

                int oldsize = list.Count;
                for (int i = oldsize; i <= index; i++)
                    list.Add(default(T));
            }

            // Ajoute l'élément
            //.................
            list[index] = t;
        }

        ///=================================================================<summary>
        /// Bouge un item dans une liste
        /// @return l'item qui a bougé, ou null si les positions sont invalides
        ///</summary>=================================================================
        public static int MoveItem<T>(this IList<T> list, int oldpos, int newpos)
        {
            if (oldpos < 0 || list.Count <= oldpos)
                return -1;
            if (newpos < 0 || list.Count <= newpos)
                return -1;

            var t = list[oldpos];
            list.RemoveAt(oldpos);
            list.Insert(newpos, t);

            return newpos;
        }

        ///=================================================================<summary>
        /// Bouge une Row dans la DataTable
        /// @return l'index de la nouvelle Row, ou -1 si les positions sont invalides
        ///</summary>=================================================================
        public static int MoveRow(this DataTable table, int oldpos, int newpos)
        {
            if (oldpos < 0 || table.Rows.Count <= oldpos)
                return -1;
            if (newpos < 0 || table.Rows.Count <= newpos)
                return -1;

            // Clone la Row dans la DataTable
            //...............................
            var Row = table.Rows[oldpos];
            var row2 = table.NewRow();
            row2.ItemArray = Row.ItemArray;

            // Insère le clone
            //................
            table.Rows.RemoveAt(oldpos);
            table.Rows.InsertAt(row2, newpos);

            return newpos;
        }

        ///=================================================================<summary>
        /// Retourne l'élément correspondant à la clé ou bien crée un nouvel élément
        ///</summary>=================================================================
        public static TValue GetOrCreate<TKey, TValue>(this Dictionary<TKey, TValue> dico, TKey key) where TValue : new()
        {
            bool exists = dico.TryGetValue(key, out var val);
            if (!exists)
            {
                val = new TValue();
                dico.Add(key, val);
            }
            return val;
        }

        ///=================================================================<summary>
        /// Même chose que le List.Find()
        ///</summary>=================================================================
        public static T Find<T>(this IEnumerable<T> coll, Predicate<T> cond) where T : class
        {
            foreach (var t in coll)
            {
                if (cond(t))
                    return t;
            }
            return null;
        }

        ///=================================================================
        ///
        ///=================================================================
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            if (list == null)
                return true;
            if (list.Count() == 0)
                return true;
            return false;
        }

        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            if (array == null)
                return true;
            if (array.Length == 0)
                return true;
            return false;
        }

        public static bool IsNullOrEmpty<T>(this string str)
        {
            if (str == null)
                return true;
            if (str.Length == 0)
                return true;
            return false;
        }

        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        ///=================================================================
        ///<summary>
        /// Teste si list1 contient tous les éléments de list2
        ///</summary>
        ///=================================================================
        public static bool ContainsAll<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            foreach (var t in list2)
            {
                if (!list1.Contains(t))
                    return false;
            }
            return true;
        }
    }
}
