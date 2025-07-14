using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel
{
    public class CalibrationNavigationManager : ViewModelBaseExt, INavigationManager
    {
        private INavigable _currentPage;
        private ObservableCollection<INavigable> _allPages;

        public event CurrentPageChangedHandler CurrentPageChanged;

        public ObservableCollection<INavigable> AllPages
        {
            get
            {
                if (_allPages == null)
                {
                    _allPages = new ObservableCollection<INavigable>();
                    _allPages.CollectionChanged += _allPages_CollectionChanged;
                }
                return _allPages;
            }
        }

        private void _allPages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateDisabledPages();
        }

        public INavigable CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (!(value is null))
                    NavigateToPage(value);
                UpdateDisabledPages();
            }
        }

        public CalibrationNavigationManager()
        {
        }

        public void NavigateToNextPage()
        {
            UpdateDisabledPages();
            var nextPage = GetNextPage();
            CurrentPage = nextPage;
        }

        public void NavigateToPage(INavigable destPage)
        {
            if (destPage is null || (_currentPage == destPage))
                return;

            UpdateDisabledPages();

            var originalPage = _currentPage;
            bool canLeave = true;
            if (originalPage != null)
            {
                canLeave = originalPage.CanLeave(destPage);
            }

            if (canLeave)
            {
                _currentPage = destPage;
                OnPropertyChanged(nameof(CurrentPage));
                CurrentPageChanged?.Invoke(_currentPage, originalPage);
                Task.Run(async () =>
                {
                    await _currentPage.PrepareToDisplay();
                }
                );
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var prevPage = _currentPage;
                    _currentPage = null;
                    OnPropertyChanged(nameof(CurrentPage));
                    _currentPage = prevPage;
                    OnPropertyChanged(nameof(CurrentPage));
                }
                ));
            }
        }

        public void NavigateToPrevPage()
        {
            var prevPage = GetPrevPage();
            CurrentPage = prevPage;
        }

        public INavigable GetFirstPage()
        {
            if (AllPages.Count > 0)
            {
                return (AllPages[0]);
            }

            return null;
        }

        public INavigable GetNextPage()
        {
            if (_currentPage == null && AllPages.Count > 0)
            {
                return (AllPages[0]);
            }
            var curPageIndex = AllPages.IndexOf(_currentPage);
            var nextPageIndex = curPageIndex + 1;
            // Find next page index
            bool isNextPageFound = false;
            while (!isNextPageFound && nextPageIndex< AllPages.Count)
            {
                if ((AllPages[nextPageIndex] as IWizardNavigationItem).IsEnabled)
                    isNextPageFound = true;
                else
                    nextPageIndex++;
            }
            if (isNextPageFound)
                return (AllPages[nextPageIndex]);

            return (_currentPage);
        }

        public INavigable GetPrevPage()
        {
            if (_currentPage == null && AllPages.Count > 0)
            {
                return (AllPages[0]);
            }

            var curPageIndex = AllPages.IndexOf(_currentPage);
            var prevPageIndex = Math.Max(curPageIndex - 1, 0);
            return AllPages[prevPageIndex];
        }

        public void RemoveAllPages()
        {
            foreach (var page in AllPages)
            {
                if (page is IDisposable disposablePage)
                    disposablePage.Dispose();
            }
            AllPages.Clear();
        }

        public INavigable GetPage(Type pageType)
        {
            return null;
        }

        private AutoRelayCommand _gotoNextPage;

        public AutoRelayCommand GotoNextPage
        {
            get
            {
                return _gotoNextPage ?? (_gotoNextPage = new AutoRelayCommand(
                    () =>
                    {
                        NavigateToNextPage();
                    },
                    () => { return true; }
                ));
            }
        }

        private void UpdateDisabledPages()
        {

        }
    }
}
