using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.UI.ViewModel.Navigation
{
    public class PageMenuItemVM : ObservableObject
    {
        private string _content = null;

        /// <summary>
        /// Menu content. Could be null for image menu without text
        /// </summary>
        public string Content { get { return _content; } set { _content = value; OnPropertyChanged(); } }

        private string _tooltip = string.Empty;

        /// <summary>
        /// Tooltip for the menu
        /// </summary>
        public string Tooltip { get { return _tooltip; } set { _tooltip = value; OnPropertyChanged(); } }

        private string _imageResourceKey;

        /// <summary>
        /// Image resource key definied in Image Dictionary
        /// </summary>
        public string ImageResourceKey { get { return _imageResourceKey; } set { _imageResourceKey = value; OnPropertyChanged(); } }

        /// <summary>
        /// Command to execute on click
        /// </summary>
        public AutoRelayCommand ExecuteCommand { get; set; } = null;

        private bool _isVisible = true;

        /// <summary>
        /// True if the menu is visible
        /// </summary>
        public bool IsVisible { get => _isVisible; set { if (_isVisible != value) { _isVisible = value; OnPropertyChanged(); } } }
    }
}
