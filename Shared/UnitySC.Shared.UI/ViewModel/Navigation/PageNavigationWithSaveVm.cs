using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.ViewModel.Header;

namespace UnitySC.Shared.UI.ViewModel.Navigation
{
    public abstract class SaveRefreshPageNavigationVM : PageNavigationVM
    {
        public SaveRefreshPageNavigationVM()
        {
            this.MenuItems.Add(new MenuItemVM()
            {
                ImageResourceKey = "SaveGeometry",
                ExecuteCommand = SaveCommand,
                Tooltip = "Save"
            });

            this.MenuItems.Add(new MenuItemVM()
            {
                ImageResourceKey = "RedoGeometry",
                ExecuteCommand = RefreshCommand,
                Tooltip = "Refresh"
            });
        }

        /// <summary>
        /// Set to true if the pasge has changed
        /// </summary>
        private bool _hasChanged = false;

        public bool HasChanged
        {
            get => _hasChanged; set { if (_hasChanged != value) { _hasChanged = value; OnPropertyChanged(); SaveCommand.NotifyCanExecuteChanged(); } }
        }

        /// <summary>
        /// Save command
        /// </summary>
        private AutoRelayCommand _saveCommand;

        public AutoRelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new AutoRelayCommand(
              () =>
              {
                  Save();
              },
              () => { return HasChanged; }));
            }
        }

        /// <summary>
        /// Refresh command
        /// </summary>
        private AutoRelayCommand _refreshCommand;

        public AutoRelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new AutoRelayCommand(
              () =>
              {
                  Refresh();
              },
              () => { return true; }));
            }
        }

        public abstract void Save();

        public abstract void Refresh();
    }
}
