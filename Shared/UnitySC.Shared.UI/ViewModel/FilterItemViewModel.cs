using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.Shared.UI.ViewModel
{
    public class FilterItemViewModel<TSource> : ObservableObject
    {
        private readonly TSource _wrappedObject;

        public FilterItemViewModel(TSource wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        public TSource WrappedObject => _wrappedObject;

        public override string ToString()
        {
            return _wrappedObject == null ? "All" : _wrappedObject.ToString();
        }
    }
}