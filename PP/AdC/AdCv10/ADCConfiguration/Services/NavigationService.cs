using System;
using System.Collections.Generic;
using System.Linq;

using ADCConfiguration.ViewModel;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADCConfiguration.Services
{
    public class NavigationService
    {
        private readonly Dictionary<string, Type> _viewModelByKey = new Dictionary<string, Type>();

        private List<NavigationItem> _History = new List<NavigationItem>();

        public NavigationViewModel NavigationViewModel { get; private set; }

        public string CurrentPageKey
        {
            get
            {
                if (_History.Count > 0)
                    return _History.Last().Key;

                return string.Empty;
            }
        }


        public NavigationService()
        {
            NavigationViewModel = new NavigationViewModel(this);
        }


        public void GoBack()
        {
            if (_History.Count > 1)
            {
                if (NavigationViewModel.CurrentViewModel.MustBeSave)
                {
                    if (!Services.Instance.PopUpService.ShowConfirme("Some modifications are not saved", "Some modifications are not saved and will be lost." + Environment.NewLine + "Do you want to go back anyway ?", true))
                        return;
                }

                _History.RemoveAt(_History.Count - 1);
                NavigationViewModel.CurrentViewModel = _History.Last().ViewModelInstance;
            }
        }

        public void NavigateToHome(NavNameEnum key)
        {
            if (NavigationViewModel.CurrentViewModel != null && NavigationViewModel.CurrentViewModel.MustBeSave)
            {
                if (!Services.Instance.PopUpService.ShowConfirme("Some modifications are not saved", "Some modifications are not saved and will be lost." + Environment.NewLine + "Do you want to go back anyway ?", true))
                    return;
            }

            _History.Clear();

            NavigateTo(key.ToString(), null);
        }


        public void NavigateTo(NavNameEnum key)
        {
            NavigateTo(key.ToString(), null);
        }

        public void NavigateTo(string key)
        {
            NavigateTo(key, null);
        }

        public void NavigateTo(string key, object parameter)
        {
            Type vm = null;
            if (_viewModelByKey.TryGetValue(key, out vm))
            {
                INavigationViewModel vmb = (INavigationViewModel)ViewModel.ViewModelLocator.GetInstance(vm);

                // mb.SendParameter(parameter); // a implementer
                AddHistory(new NavigationItem(key, vm, vmb));

                NavigationViewModel.CurrentViewModel = vmb;

                INavigationViewModel navigationVM = vmb as INavigationViewModel;

                if (navigationVM != null)
                    navigationVM.Refresh();
            }
        }


        public void Register<VM>(string key) where VM : INavigationViewModel
        {
            if (!_viewModelByKey.ContainsKey(key))
                _viewModelByKey.Add(key, typeof(VM));

        }

        public void Register<VM>(NavNameEnum key) where VM : INavigationViewModel
        {
            Register<VM>(key.ToString());
        }

        private void AddHistory(NavigationItem navigationItem)
        {
            _History.Add(navigationItem);

            if (_History.Count > 10)
            {
                _History.RemoveAt(0);
            }
        }

    }
    public class NavigationItem
    {
        public string Key { get; private set; }
        public Type ViewModelType { get; private set; }
        public INavigationViewModel ViewModelInstance { get; private set; }

        public NavigationItem(string key, Type viewModelType, INavigationViewModel viewModelInstance)
        {
            Key = key;
            ViewModelType = viewModelType;
            ViewModelInstance = viewModelInstance;
        }
    }


    public class NavigationViewModel : ObservableRecipient
    {

        private NavigationService _navigationService = null;


        internal NavigationViewModel(NavigationService NavigationService)
        {
            _navigationService = NavigationService;
        }

        private INavigationViewModel _currentViewModel = null;
        public INavigationViewModel CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        private AutoRelayCommand _goHomeCommand = null;
        public AutoRelayCommand GoHomeCommand
        {
            get
            {
                return _goHomeCommand ?? (_goHomeCommand = new AutoRelayCommand(() =>
                {
                    _navigationService.NavigateToHome(NavNameEnum.MainMenu);
                }));

            }
        }

        private AutoRelayCommand _goBackCommand = null;
        public AutoRelayCommand GoBackCommand
        {
            get
            {
                return _goBackCommand ?? (_goBackCommand = new AutoRelayCommand(() =>
                {
                    _navigationService.GoBack();
                }));

            }
        }

    }


    public enum NavNameEnum
    {
        MainMenu,
        ImportRecipe,
        ExportRecipe,
        RecipeHistory,
        UserLogin,
        UserManager,
    //    Tool,
   //     ExportConfiguration,
        WaferTypes,
        Vids,
   //     MachineConfigurationArchive,
        Logs,
        ArchivedRecipe
    }
}
