using System.Collections.Generic;
using System.Linq;

namespace ADCEngine
{
    /// <summary>
    /// Interface utilisé pour la comparaison de la valeur des parametres des modules
    /// </summary>
    public interface IValueComparer
    {
        bool HasSameValue(object obj);
    }

    public static class ValueComparerExtension
    {

        /// <summary>
        /// Compararaison de l'égalité du contenu des listes
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <returns></returns>
        public static bool ValuesEqual<T>(this IList<T> list1, IList<T> list2) where T : IValueComparer
        {
            if (list1 == list2)
                return true;

            if (list1 == null || list2 == null)
                return false;

            if (list1.Count != list2.Count)
                return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (!list1[i].HasSameValue(list2[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Comparaison des clés et valeurs du dictionnaire
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        public static bool DictionaryEqual<TKey, TValue>(this IDictionary<TKey, TValue> dict1, IDictionary<TKey, TValue> dict2) where TValue : IValueComparer
        {
            if (dict1 == dict2)
                return true;

            if (dict1 == null | dict2 == null)
                return false;

            if (dict1.Count != dict2.Count)
                return false;

            return dict1.All(kvp =>
            {
                TValue value2;
                return dict2.TryGetValue(kvp.Key, out value2)
                    && kvp.Value.HasSameValue(value2);
            });
        }
    }
}
