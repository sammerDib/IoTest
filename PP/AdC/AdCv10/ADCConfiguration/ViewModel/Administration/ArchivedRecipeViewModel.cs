using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using AcquisitionAdcExchange;

using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADCConfiguration.ViewModel.Administration
{
    public class ArchivedRecipeViewModel : ViewModelWithMenuBase, INavigationViewModel
    {
        private ICollectionView _recipesView;
        private ArchivedRecipeDetailViewModel _selectedRecipe;
        private List<string> _recipeTypes = new List<string>();
        private const string AllType = "All Types..";
        public List<string> RecipeTypes => _recipeTypes;
        private List<ArchivedRecipeDetailViewModel> _recipes;

        private eModuleID? _recipeTypeFilter; //TO-DO a remplacé avec des result-type
        private string _selectedRecipeType;
        public string SelectedRecipeType
        {
            get => _selectedRecipeType;
            set
            {
                if (_selectedRecipeType != value)
                {
                    _selectedRecipeType = value;
                    _recipeTypeFilter = _selectedRecipeType != AllType ? (eModuleID?)Enum.Parse(typeof(eModuleID), value) : null;
                    OnPropertyChanged();
                    _recipesView.Refresh();
                }
            }
        }

        public ArchivedRecipeDetailViewModel SelectedRecipe
        {
            get { return _selectedRecipe; }
            set
            {
                _selectedRecipe = value;

                if (_selectedRecipe != null)
                    _selectedRecipe.Init();

                OnPropertyChanged();
            }
        }

        public ICollectionView Recipes
        {
            get { return _recipesView; }
            set
            {
                _recipesView = value;
                OnPropertyChanged();
            }
        }

        private string _filter;
        public string Filter
        {
            get => _filter;
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    OnPropertyChanged();
                    _recipesView.Refresh();
                }
            }
        }

        public ArchivedRecipeViewModel()
        {
            MenuName = "Enable/Disable Recipe";

            CommandMenuItems.Add(new MenuItemViewModel()
            {
                Name = "Refresh",
                Description = "Refresh",
                ExecuteCommand = RefreshCommand,
                ImageResourceKey = "Refresh",
                IconText = "Refresh"
            });

            CommandMenuItems.Add(new MenuItemViewModel()
            {
                Name = "Save",
                Description = "Save",
                ExecuteCommand = SaveCommand,
                ImageResourceKey = "SaveADCImage",
                IconText = "Save"
            });
        }

        private void Init()
        {
            IsBusy = true;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var recipeProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
                    _recipes = recipeProxy.GetADCRecipes(null, false,true).Select(x => new ArchivedRecipeDetailViewModel(x)).ToList();
                    _recipeTypes.Clear();
                    _recipeTypes.Add(AllType);
                    _recipeTypes.AddRange(Enum.GetNames(typeof(eModuleID)));
                    Application.Current.Dispatcher.Invoke((() =>
                    {
                        Recipes = CollectionViewSource.GetDefaultView(_recipes);
                        _recipesView.Filter = RecipeFilter;
                        OnPropertyChanged(nameof(RecipeTypes));
                        SelectedRecipeType = AllType;
                    }));
                }
                catch (Exception ex)
                {
                    Services.Services.Instance.LogService.LogError("Refresh archived recipe", ex);
                    Application.Current.Dispatcher.Invoke((() => { AdcTools.ExceptionMessageBox.Show("Refresh archived error: ", ex); }));
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        private void Save()
        {
            IsBusy = true;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Dictionary<int, bool> recipeIdArchiveState = new Dictionary<int, bool>();

                    // Add all versions of modified recipes to dictionary
                    _recipes.Where(x => x.HasChanged).SelectMany(x => x.AllRecipeVersions).ToList().ForEach(x => recipeIdArchiveState.Add(x.Recipe.Id, x.IsArchived));

                    // Update database
                    var recipeProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
#warning **USP** TODO UpdateArchivedRecipes in DbRecipeServiceProxy
                    //_recipeService.UpdateArhivedRecipes(recipeIdArchiveState, Services.Services.Instance.AuthentificationService.CurrentUser.Id);

                    // Update visual state
                    _recipes.ForEach(x => x.HasChanged = false);
                }
                catch (Exception ex)
                {
                    Services.Services.Instance.LogService.LogError("Save recipe state", ex);
                    Application.Current.Dispatcher.Invoke((() => { AdcTools.ExceptionMessageBox.Show("Save recipe error: ", ex); }));
                    Init();
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        /// <summary>
        /// Filtre sur le nom de la recette et le type 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool RecipeFilter(object obj)
        {
            ArchivedRecipeDetailViewModel recipeVm = (obj as ArchivedRecipeDetailViewModel);
            return (Filter == null || recipeVm.Recipe.Name.ToLower().Contains(Filter.ToLower()));
            // to do : rajouter un filtrer avec les resultypes contenu dans la recettes
              //  && (_recipeTypeFilter == null || recipeVm.Recipe.DataLoaderTypesList.Any(x => x == _recipeTypeFilter));
        }

        public void Refresh()
        {
            Init();
        }

        public bool MustBeSave => _recipes != null && _recipes.Any(x => x.HasChanged);

        #region Commands

        private AutoRelayCommand _refreshCommand = null;
        public AutoRelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new AutoRelayCommand(
              () =>
              {
                  if (MustBeSave)
                  {
                      if (MessageBox.Show("Some recipe state are not saved and will be lost." + Environment.NewLine + "Do you want to refresh anyway ?", "Some recipe are not saved", MessageBoxButton.YesNo) == MessageBoxResult.No)
                          return;
                  }

                  Init();
                  Services.Services.Instance.LogService.LogDebug("Refresh archived recipe");
              },
              () => { return true; }));
            }
        }


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
              () => { return MustBeSave; }));
            }
        }

        #endregion
    }
}
