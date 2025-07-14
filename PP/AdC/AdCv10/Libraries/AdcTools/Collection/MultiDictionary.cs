using System.Collections.Generic;

namespace AdcTools
{
    // A Dictionary that associates several items to a key
    public class MultiDictionary<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {
        public void Add(TKey key, TValue val)
        {
            List<TValue> list;
            if (!TryGetValue(key, out list))
            {
                list = new List<TValue>();
                base.Add(key, list);
            }
            list.Add(val);
        }

        public void Add(TKey key, params TValue[] val)
        {
            List<TValue> list;
            if (!TryGetValue(key, out list))
            {
                list = new List<TValue>();
                base.Add(key, list);
            }
            list.AddRange(val);
        }

        public IEnumerable<TValue> MultiValues
        {
            get
            {
                List<TValue> fulllist = new List<TValue>();
                foreach (List<TValue> sublist in Values)
                    fulllist.AddRange(sublist);
                return fulllist;
            }
        }

        // It would be nice to have a Remove method to remove a single element,
        // but we don't need it in this project.
        // Note that you may use Remove() to remove all the items associated with a key.
        //
        // Also note that you can use the regular TryGetValue() to get the list of value associated with the key.
    }
}
