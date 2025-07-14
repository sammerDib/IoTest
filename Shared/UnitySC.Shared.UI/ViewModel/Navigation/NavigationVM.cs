using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.Shared.UI.ViewModel.Navigation
{
    public abstract class NavigationVM : PageNavigationVM
    {
        /// <summary>
        /// Sub pages
        /// </summary>
        public ObservableCollection<PageNavigationVM> Pages { get; private set; } = new ObservableCollection<PageNavigationVM>();

        /// <summary>
        /// Current page
        /// </summary>
        private PageNavigationVM _currentPage;

        public PageNavigationVM CurrentPage
        {
            get => _currentPage; private set { if (_currentPage != value) { _currentPage = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Go to existing page or new page
        /// </summary>
        /// <param name="page"></param>
        public void Navigate(PageNavigationVM page)
        {
            if (CurrentPage?.PageName == page?.PageName)
                return;

            // Check if the page has changed before navigate
            if (CurrentPage != null && CurrentPage is SaveRefreshPageNavigationVM)
            {
                var pageToCheck = CurrentPage as SaveRefreshPageNavigationVM;
                if (pageToCheck.HasChanged)
                {
                    var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
                    var result = dialogService.ShowMessageBox("Some modifications are not saved and will be lost." + Environment.NewLine + "Do you want to go back anyway ?", "Some modifications are not saved", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result != MessageBoxResult.Yes)
                        return;
                }
            }

            if (CurrentPage != null)
                CurrentPage.Unloading();    // On signale à la page qu'elle va être déchargée

            if (page == null || page == this) // Home
            {
                CurrentPage = null;
                Pages.Clear();
            }
            else if (Pages.Contains(page)) // Existing page
            {
                CurrentPage = page;

                // Remove cuurent and next pages
                for (int i = Pages.Count() - 1; i > Pages.IndexOf(page); i--)
                    Pages.RemoveAt(i);
            }
            else // New page
            {
                Pages.Add(page);
                CurrentPage = page;
            }

            if (CurrentPage != null)
                CurrentPage.Loaded();    // On signale à la page qu'elle vient d'être chargée

            BackCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Go back to previous page
        /// </summary>
        private AutoRelayCommand _backCommand;

        public AutoRelayCommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new AutoRelayCommand(
              () =>
              {
                  if (Pages.Count <= 1) // Go Home
                      Navigate(this);
                  else
                      Navigate(Pages[Pages.IndexOf(CurrentPage) - 1]);
              },
              () => { return (CurrentPage == null || CurrentPage.CanNavigate) && Pages.Any(); }));
            }
        }

        /// <summary>
        /// Navigate to page
        /// </summary>
        private AutoRelayCommand<PageNavigationVM> _navigateCommand;

        public AutoRelayCommand<PageNavigationVM> NavigateCommand
        {
            get
            {
                return _navigateCommand ?? (_navigateCommand = new AutoRelayCommand<PageNavigationVM>(
              (page) =>
              {
                  Navigate(page);
              },
              (page) => { return CurrentPage == null || CurrentPage.CanNavigate; }));
            }
        }
    }
}
