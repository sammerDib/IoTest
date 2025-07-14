using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.Shared.UI.ViewModel
{
    public class SelectableItemVM<TSource> : ObservableObject
    {
        private bool _isSelected;
        private bool _isAvailable;

        public bool IsSelected
        {
            get => _isSelected; set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        public bool IsAvailable
        {
            get => _isAvailable; set { if (_isAvailable != value) { _isAvailable = value; OnPropertyChanged(); } }
        }
        public TSource WrappedObject { get; set; }
    }
}
