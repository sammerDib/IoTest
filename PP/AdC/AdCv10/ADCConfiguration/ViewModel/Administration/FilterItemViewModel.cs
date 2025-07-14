using CommunityToolkit.Mvvm.ComponentModel;

namespace ADCConfiguration.ViewModel.Administration
{
    public class FilterItemViewModel<TSource> : ObservableRecipient
    {
        private TSource _wrappedObject;

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
