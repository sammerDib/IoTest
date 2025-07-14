using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.EME.Client.Modules.Calibration.ViewModel
{
    public sealed class NavigationManagerForCalibration : ViewModelBaseExt, INavigationManager
    {
        private ObservableCollection<INavigable> _allPages;
        private INavigable _currentPage;

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

        public INavigable CurrentPage
        {
            get => _currentPage;
            set
            {
                if (!(value is null))
                    NavigateToPage(value);
                UpdateDisabledPages();
            }
        }

        public INavigable GetFirstPage()
        {
            if (AllPages.Count > 0)
            {
                return AllPages[0];
            }

            return null;
        }

        public INavigable GetNextPage()
        {
            if (_currentPage == null && AllPages.Count > 0)
            {
                return AllPages[0];
            }

            int curPageIndex = AllPages.IndexOf(_currentPage);
            int nextPageIndex = curPageIndex + 1;
            // Find next page index
            bool isNextPageFound = false;
            while (!isNextPageFound && nextPageIndex < AllPages.Count)
            {
                if ((AllPages[nextPageIndex] as IWizardNavigationItem).IsEnabled)
                    isNextPageFound = true;
                else
                    nextPageIndex++;
            }

            if (isNextPageFound)
                return AllPages[nextPageIndex];

            return _currentPage;
        }

        public INavigable GetPage(Type pageType)
        {
            return AllPages.FirstOrDefault(p => pageType.IsInstanceOfType(p));
        }

        public INavigable GetPrevPage()
        {
            if (_currentPage == null && AllPages.Count > 0)
            {
                return AllPages[0];
            }

            int curPageIndex = AllPages.IndexOf(_currentPage);
            int prevPageIndex = Math.Max(curPageIndex - 1, 0);
            return AllPages[prevPageIndex];
        }

        public void NavigateToNextPage()
        {
            UpdateDisabledPages();
            var nextPage = GetNextPage();
            CurrentPage = nextPage;
        }

        public void NavigateToPage(INavigable destPage)
        {
            if (destPage is null || _currentPage == destPage)
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

        public void RemoveAllPages()
        {
            foreach (var page in AllPages)
            {
                if (page is IDisposable disposablePage)
                    disposablePage.Dispose();
            }

            AllPages.Clear();
        }

        private void _allPages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateDisabledPages();
        }

        private void UpdateDisabledPages()
        {
#if !HMI_DEV
            var pages = new List<IWizardNavigationItem>
            {
                AllPages.Find(p => p is ChuckParallelismCalibrationVM) as IWizardNavigationItem,
                AllPages.Find(p => p is FilterCalibrationVM) as IWizardNavigationItem,
                AllPages.Find(p => p is AxisOrthogonalityCalibrationVM) as IWizardNavigationItem,
                AllPages.Find(p => p is CameraMagnificationCalibrationVM) as IWizardNavigationItem,
                AllPages.Find(p => p is ChuckManagerCalibrationVM) as IWizardNavigationItem,
                AllPages.Find(p => p is DistanceSensorCalibrationVM) as IWizardNavigationItem,
                AllPages.Find(p => p is DistortionCalibrationVM) as IWizardNavigationItem
            };

            bool areAllPreviousValidated = true;

            foreach (var page in pages.Where(page => page != null))
            {
                page.IsEnabled = areAllPreviousValidated;
                areAllPreviousValidated = areAllPreviousValidated && page.IsValidated;
            }
#endif
        }
    }
}
