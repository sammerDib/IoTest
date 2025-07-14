using System;
using System.Collections.ObjectModel;

using ADCConfiguration.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADCConfiguration.ViewModel
{
    public class MainMenuViewModel : ObservableRecipient, INavigationViewModel
    {
        public ObservableCollection<MenuItemViewModel> Groups { get; } = new ObservableCollection<MenuItemViewModel>();

        public bool MustBeSave => false;

        public MainMenuViewModel(IMessenger messenger)
        {
            LoadMenu();
            messenger.Register<Messages.UserChangedMessage>(this, (r, m) => { LoadMenu(); });
        }

        private void LoadMenu()
        {
            Groups.Clear();
            Groups.Add(new MenuItemViewModel("Recipe",
               //new MenuItemViewModel() { Name = "Import Recipe", Color = MenuColorEnum.Blue01, Description = "Import a recipe and external files", NavigationKey = NavNameEnum.ImportRecipe, ImageResourceKey = "Import" },
               //new MenuItemViewModel() { Name = "Export Recipe", Color = MenuColorEnum.Blue01, Description = "Export a recipe and external files", NavigationKey = NavNameEnum.ExportRecipe, ImageResourceKey = "Export" },
               new MenuItemViewModel() { Name = "Recipe History", Color = MenuColorEnum.Blue01, Description = "Display the recipe history", NavigationKey = NavNameEnum.RecipeHistory, ImageResourceKey = "History", IsEnabled = Services.Services.Instance.AuthentificationService.CurrentUser.IsExpert }
               ));
            Groups.Add(new MenuItemViewModel("Configuration",
                //new MenuItemViewModel() { Name = "Wafer Type", Color = MenuColorEnum.Blue02, Description = "Edit wafer types", NavigationKey = NavNameEnum.WaferTypes, ImageResourceKey = "Wafer" },
                //new MenuItemViewModel() { Name = "Machines configuration", Color = MenuColorEnum.Blue02, Description = "Edit machines configuration", NavigationKey = NavNameEnum.Tool, ImageResourceKey = "Tool" },
                new MenuItemViewModel() { Name = "Vid", Color = MenuColorEnum.Blue02, Description = "Edit variable Ids", NavigationKey = NavNameEnum.Vids, ImageResourceKey = "Vid" }
                //,new MenuItemViewModel() { Name = "Export configuration", Color = MenuColorEnum.Blue02, Description = "Export machines configuration", NavigationKey = NavNameEnum.ExportConfiguration, ImageResourceKey = "Export" }
                ));
            Groups.Add(new MenuItemViewModel("Administration",
                //new MenuItemViewModel() { Name = "Users", Color = MenuColorEnum.Blue03, Description = "User Management", NavigationKey = NavNameEnum.UserManager, ImageResourceKey = "User" },
                //new MenuItemViewModel() { Name = "Enable/Disable recipe", Color = MenuColorEnum.Blue03, Description = "Recipe management", NavigationKey = NavNameEnum.ArchivedRecipe, ImageResourceKey = "Archive" },
              //  new MenuItemViewModel() { Name = "Machines config. history", Color = MenuColorEnum.Blue03, Description = "Display the machine configuration history", NavigationKey = NavNameEnum.MachineConfigurationArchive, ImageResourceKey = "History" },
                new MenuItemViewModel() { Name = "Logs", Color = MenuColorEnum.Blue03, Description = "Consultation of the logs", NavigationKey = NavNameEnum.Logs, ImageResourceKey = "Log" })
            { IsEnabled = Services.Services.Instance.AuthentificationService.CurrentUser.IsExpert });
        }

        public void Refresh()
        {
        }
    }


    public class MenuItemViewModel : ObservableRecipient
    {
        private ObservableCollection<MenuItemViewModel> _menuItems = new ObservableCollection<MenuItemViewModel>();
        public ObservableCollection<MenuItemViewModel> Items { get; }
        public ObservableCollection<MenuItemViewModel> MenuItems { get { return _menuItems; } set { _menuItems = value; OnPropertyChanged(); } }


        private string _name = string.Empty;
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        private string _description = string.Empty;
        public string Description { get { return _description; } set { _description = value; OnPropertyChanged(); } }

        private MenuColorEnum _color = MenuColorEnum.Blue;
        public MenuColorEnum Color { get { return _color; } set { _color = value; OnPropertyChanged(); } }

        private string _imageResourceKey;
        public string ImageResourceKey { get { return _imageResourceKey; } set { _imageResourceKey = value; OnPropertyChanged(); } }

        private string _iconText;
        public string IconText { get { return _iconText; } set { _iconText = value; OnPropertyChanged(); } }


        private string _viewModel = string.Empty;
        public string ViewModel { get { return _viewModel; } set { _viewModel = value; OnPropertyChanged(); } }


        private AutoRelayCommand _executeCommand = null;
        public AutoRelayCommand ExecuteCommand { get { return _executeCommand; } set { _executeCommand = value; } }



        private Services.NavNameEnum _navigationKey = Services.NavNameEnum.MainMenu;
        public NavNameEnum NavigationKey { get => _navigationKey; set => _navigationKey = value; }


        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get => _isEnabled; set { if (_isEnabled != value) { _isEnabled = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _navigateToCommand = null;
        public AutoRelayCommand NavigateToCommand
        {
            get
            {
                return _navigateToCommand ?? (_navigateToCommand = new AutoRelayCommand(() =>
                {

                    if (ExecuteCommand != null)
                    {
                        ExecuteCommand.Execute(null);
                    }
                    else
                    {
                        Services.Services.Instance.NavigationService.NavigateTo(NavigationKey);
                    }


                },
                () =>
                {
                    if (ExecuteCommand != null)
                    {
                        return ExecuteCommand.CanExecute(null);
                    }
                    return true;
                }


                ));

            }
        }


        public MenuItemViewModel() { }

        public MenuItemViewModel(string name, params MenuItemViewModel[] items)
        {
            Name = name;
            Array.ForEach(items, i => MenuItems.Add(i));
        }

    }

    public enum MenuColorEnum
    {
        Red,
        Blue,
        Green,
        Yellow,
        Orange,

        Blue01,
        Blue02,
        Blue03,

    }

}

