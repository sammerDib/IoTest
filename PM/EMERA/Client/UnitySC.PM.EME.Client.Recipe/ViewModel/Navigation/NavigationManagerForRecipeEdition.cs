using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.EME.Client.Recipe.ViewModel.Navigation
{
    public class NavigationManagerForRecipeEdition : ViewModelBaseExt, INavigationManagerForRecipeEdition
    {
        private INavigable _currentPage;
        public event CurrentPageChangedHandler CurrentPageChanged;
        public ObservableCollection<INavigable> AllPages { get; } = new ObservableCollection<INavigable>();
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

        private void UpdateDisabledPages()
        {
#if !HMI_DEV
            var recipeExecutionViewModel = AllPages.Find(p => p is RecipeExecutionViewModel) as RecipeExecutionViewModel;
            var acquisitionsEditorViewModel = AllPages.Find(p => p is AcquisitionsEditorViewModel) as AcquisitionsEditorViewModel;
            if (!(recipeExecutionViewModel is null) && !(acquisitionsEditorViewModel is null))
                recipeExecutionViewModel.IsEnabled = acquisitionsEditorViewModel.IsValid();
#endif
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

        public void NavigateToNextPage()
        {
            throw new NotImplementedException();
        }

        public void NavigateToPrevPage()
        {
            throw new NotImplementedException();
        }

        public INavigable GetFirstPage()
        {
            return AllPages.First();
        }

        public INavigable GetNextPage()
        {
            throw new NotImplementedException();
        }

        public INavigable GetPrevPage()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
