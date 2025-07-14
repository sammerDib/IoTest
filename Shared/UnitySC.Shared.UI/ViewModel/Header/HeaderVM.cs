using System.Collections.ObjectModel;

namespace UnitySC.Shared.UI.ViewModel.Header
{
    public class HeaderVM : ViewModelBaseExt
    {
        /// <summary>
        /// List of menu items
        /// </summary>
        public ObservableCollection<MenuItemVM> MenuItems { get; private set; } = new ObservableCollection<MenuItemVM>();
    }
}