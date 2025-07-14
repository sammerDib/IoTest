using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.UI.ViewModel.Navigation;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel
{
    public abstract class AnaNavigationVM : NavigationVM
    {
        public ObservableCollection<PageNavigationVM> AllPages { get; private set; } = new ObservableCollection<PageNavigationVM>();

        public ObservableCollection<PageNavigationVM> NotVisitedPages { get; private set; } = new ObservableCollection<PageNavigationVM>();

        public AnaNavigationVM()
        {
            Pages.CollectionChanged += Pages_CollectionChanged;
        }

        private int _currentPageIndex => (CurrentPage == null) ? 0 : AllPages.IndexOf(CurrentPage);
        public void NavigateToNextPage()
        {
            if (AllPages.Count > _currentPageIndex + 1)
                Navigate(AllPages[_currentPageIndex + 1]);
        }

        private void Pages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotVisitedPages = new ObservableCollection<PageNavigationVM>(AllPages.Except(Pages));
            OnPropertyChanged("NotVisitedPages");
        }

        public bool CanNavigateToNextPage()
        {
            return AllPages.Count > _currentPageIndex + 1;
        }
    }
}
