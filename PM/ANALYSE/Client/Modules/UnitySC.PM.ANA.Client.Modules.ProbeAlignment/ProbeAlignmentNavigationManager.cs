using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Modules.ProbeAlignment
{
    public class ProbeAlignmentNavigationManager : ViewModelBaseExt, INavigationManager
    {
        #region Fields

        private INavigable _currentPage;
        private ObservableCollection<INavigable> _allPages;
        public event CurrentPageChangedHandler CurrentPageChanged;

        #endregion

        #region Properties

        public ObservableCollection<INavigable> AllPages
        {
            get => _allPages ?? (_allPages = new ObservableCollection<INavigable>());
        }

        public INavigable CurrentPage
        {
            get => _currentPage;
            set => NavigateToPage(value);
        }

        #endregion

        #region Methods

        public void NavigateToNextPage()
        {
            CurrentPage = GetNextPage();
        }

        public void NavigateToPage(INavigable destPage)
        {
            if (destPage is null || (CurrentPage == destPage))
                return;

            var originalPage = CurrentPage;
            bool canLeave = originalPage?.CanLeave(destPage) ?? true;

            if (!canLeave)
            {
                return;
            }

            SetProperty(ref _currentPage, destPage, nameof(CurrentPage));
            CurrentPageChanged?.Invoke(CurrentPage, originalPage);
            Task.Run(async () =>
                {
                    await CurrentPage.PrepareToDisplay();
                }
            );
        }

        public void NavigateToPrevPage()
        {
            CurrentPage = GetPrevPage();
        }

        public INavigable GetFirstPage()
        {
            return AllPages.Count > 0 ? AllPages[0] : null;
        }

        public INavigable GetNextPage()
        {
            if (_currentPage == null && AllPages.Count > 0)
            {
                return (AllPages[0]);
            }

            var curPageIndex = AllPages.IndexOf(_currentPage);
            var nextPageIndex = Math.Min(curPageIndex + 1, AllPages.Count - 1);
            return AllPages[nextPageIndex];
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

        #endregion
    }
}
