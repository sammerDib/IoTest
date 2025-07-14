using System.Collections.Generic;

namespace AdcTools
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// C'est comme un dictionnaire normal, mais l'exception a un message plus clair.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class CustomExceptionDictionary<TKey, TVal> : Dictionary<TKey, TVal>
    {
        #region Constructors
        public CustomExceptionDictionary(string exceptionKeyName)
        {
            ExceptionKeyName = exceptionKeyName;
        }

        public CustomExceptionDictionary()
        {
        }

        public CustomExceptionDictionary(IDictionary<TKey, TVal> dictionary)
            : base(dictionary)
        {
        }

        public CustomExceptionDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public CustomExceptionDictionary(int capacity)
            : base(capacity)
        {
        }

        public CustomExceptionDictionary(IDictionary<TKey, TVal> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer)
        {
        }

        public CustomExceptionDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }

        #endregion

        /// <summary>
        /// Nom utilisé pour pour les KeyNotFoundException
        /// </summary>
        public string ExceptionKeyName { get; set; } = typeof(TVal).Name;

        public new TVal this[TKey key]
        {
            get
            {
                try
                {
                    return base[key];
                }
                catch (KeyNotFoundException ex)
                {
                    string classname = typeof(TVal).Name;
                    throw new KeyNotFoundException($"{ExceptionKeyName} not found: {key}", ex);
                }
            }

            set { base[key] = value; }
        }
    }
}
