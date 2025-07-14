using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.ViewModel;
using UnitySC.PM.Shared;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.Navigation
{
    public class NavigationManager : ViewModelBaseExt, INavigationManager
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

        public NavigationManager()
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
            return (ServiceLocator.NavigationManager.AllPages.FirstOrDefault(p => pageType.IsInstanceOfType(p)));
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
#if !HMI_DEV

            var recipeAlignmentPage = AllPages.Find(p => p is RecipeAlignmentVM) as RecipeWizardStepBaseVM;
            var isWaferLessMode = ClassLocator.Default.GetInstance<IClientConfigurationManager>().IsWaferLessMode;
            var isRecipeAlignmentPageValidatedOrWaferLess = isWaferLessMode ? true : recipeAlignmentPage?.IsValidated == true;
            var recipeWaferMapPage = AllPages.Find(p => p is RecipeWaferMapVM) as RecipeWizardStepBaseVM;
            var recipeAlignmentMarksPage = AllPages.Find(p => p is RecipeAlignmentMarksVM) as RecipeWizardStepBaseVM;
            var recipeMeasuresChoicePage = AllPages.Find(p => p is RecipeMeasuresChoiceVM) as RecipeWizardStepBaseVM;
            var recipeDiesSelectionPage = AllPages.Find(p => p is RecipeDiesSelectionVM) as RecipeWizardStepBaseVM;
            var recipeRunPage = AllPages.Find(p => p is RecipeRunVM) as RecipeWizardStepBaseVM;

            if (!(recipeWaferMapPage is null))
                recipeWaferMapPage.IsEnabled = isRecipeAlignmentPageValidatedOrWaferLess;
            if (!(recipeAlignmentMarksPage is null))
                recipeAlignmentMarksPage.IsEnabled = (isRecipeAlignmentPageValidatedOrWaferLess) && (recipeWaferMapPage?.IsValidated == true);
            if (!(recipeMeasuresChoicePage is null))
                recipeMeasuresChoicePage.IsEnabled = (isRecipeAlignmentPageValidatedOrWaferLess) && (recipeAlignmentMarksPage?.IsValidated == true) && (recipeWaferMapPage?.IsValidated == true);
            foreach (var measurePage in AllPages.Where(p => (p as RecipeWizardStepBaseVM).IsMeasure).ToList())
            {
                (measurePage as RecipeWizardStepBaseVM).IsEnabled = (isRecipeAlignmentPageValidatedOrWaferLess) && (recipeAlignmentMarksPage?.IsValidated == true) && (recipeWaferMapPage?.IsValidated == true) && (recipeMeasuresChoicePage?.IsValidated == true)&& (measurePage as RecipeMeasureVM).IsActive;
            }
            bool areAllMeasuresValidated = !AllPages.Any(p => (p as RecipeWizardStepBaseVM).IsMeasure && (p as RecipeWizardStepBaseVM).IsEnabled && !(p as RecipeWizardStepBaseVM).IsValidated);

            if (!(recipeDiesSelectionPage is null))
                recipeDiesSelectionPage.IsEnabled = (isRecipeAlignmentPageValidatedOrWaferLess) && (recipeMeasuresChoicePage?.IsValidated == true) && areAllMeasuresValidated;

            if (!(recipeRunPage is null))
                recipeRunPage.IsEnabled = (isRecipeAlignmentPageValidatedOrWaferLess) && (recipeMeasuresChoicePage?.IsValidated == true) && areAllMeasuresValidated && (recipeDiesSelectionPage?.IsValidated??true);
#endif
        }
    }
}
