using System.Collections.Generic;
using System.ComponentModel;

namespace DeepLearningSoft48.Services
{
    /// <summary>
    /// Allows for two-way binding of TValue, as opposed to KeyValuePair where it's read-only. 
    /// </summary>
    public class KeyValuePairWrapper<TKey, TValue> : INotifyPropertyChanged
    {
        private KeyValuePair<TKey, TValue> _pair;

        public KeyValuePairWrapper(TKey key, TValue value)
        {
            _pair = new KeyValuePair<TKey, TValue>(key, value);
        }

        public TKey Key
        {
            get { return _pair.Key; }
            set
            {
                _pair = new KeyValuePair<TKey, TValue>(value, _pair.Value);
                OnPropertyChanged(nameof(Key));
            }
        }

        public TValue Value
        {
            get { return _pair.Value; }
            set
            {
                _pair = new KeyValuePair<TKey, TValue>(_pair.Key, value);
                OnPropertyChanged(nameof(Value));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
