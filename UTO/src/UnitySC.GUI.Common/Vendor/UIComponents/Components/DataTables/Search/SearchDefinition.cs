using System;
using System.Text.RegularExpressions;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables.Search
{
    public class SearchDefinition : Notifier, IDisposable
    {
        public IText DisplayName { get; protected set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetAndRaiseIfChanged(ref _isSelected, value); }
        }

        #region IDisposable
        
        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing && DisplayName is IDisposable disposable)
                {
                    disposable.Dispose();
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
    }

    public class SearchDefinition<T> : SearchDefinition
    {
        public Func<T, string> GetComparableStringFunc { get; }

        public SearchDefinition(LocalizableText displayName, Func<T, string> getComparableStringFunc) : this((IText)displayName, getComparableStringFunc)
        {
        }

        public SearchDefinition(IText displayName, Func<T, string> getComparableStringFunc)
        {
            GetComparableStringFunc = getComparableStringFunc;
            DisplayName = displayName;
        }

        internal bool Validate(T model, Regex regex)
        {
            var comparableSring = GetComparableStringFunc(model);
            if (comparableSring == null)
            {
                return false;
            }
            return regex.IsMatch(comparableSring);
        }
    }
}
