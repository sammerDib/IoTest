using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.Messaging;

using Utils.ViewModel;

namespace ADCConfiguration.ViewModel
{
    public class ViewModelWithMenuBase : ValidationViewModelBase
    {
        public ViewModelWithMenuBase() : base() { }

        public ViewModelWithMenuBase(IMessenger messenger) : base(messenger) { }

        public ObservableCollection<MenuItemViewModel> MenuItems { get; private set; } = new ObservableCollection<MenuItemViewModel>();

        public ObservableCollection<MenuItemViewModel> CommandMenuItems { get; private set; } = new ObservableCollection<MenuItemViewModel>();

        private string _menuName;
        public string MenuName { get { return _menuName; } set { _menuName = value; OnPropertyChanged(nameof(MenuName)); } }

    }
}
