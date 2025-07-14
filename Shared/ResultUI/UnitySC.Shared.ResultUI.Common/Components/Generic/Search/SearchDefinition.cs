using System;
using System.Text.RegularExpressions;

using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.Shared.ResultUI.Common.Components.Generic.Search
{
    public class SearchDefinition : ObservableObject, IDisposable
    {
        public string DisplayName { get; protected set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        #region IDisposable

        public void Dispose()
        {
        }

        #endregion
    }

    public class SearchDefinition<T> : SearchDefinition
    {
        public Func<T, string> GetComparableStringFunc { get; }

        public SearchDefinition(string displayName, Func<T, string> getComparableStringFunc)
        {
            GetComparableStringFunc = getComparableStringFunc;
            DisplayName = displayName;
        }

        internal bool Validate(T model, Regex regex)
        {
            string comparableSring = GetComparableStringFunc(model);
            return regex.IsMatch(comparableSring);
        }
    }
}
