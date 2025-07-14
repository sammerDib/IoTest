using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

using AcquisitionAdcExchange;

using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Dto = UnitySC.DataAccess.Dto;

namespace ADCConfiguration.ViewModel.Recipe
{
    public class ExportRecipeViewModel : ViewModelWithMenuBase, INavigationViewModel
    {
        private ICollectionView _recipesView;
        private Dto.Recipe _selectedRecipe;
        private List<string> _recipeTypes = new List<string>();
        private const string AllType = "All Types..";
        public List<string> RecipeTypes => _recipeTypes;

        private eModuleID? _recipeTypeFilter;
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

        public Dto.Recipe SelectedRecipe
        {
            get { return _selectedRecipe; }
            set
            {
                _selectedRecipe = value;

                if (_selectedRecipe != null)
                    RecipeDetailVM = new ExportRecipeOptionDetailViewModel(_selectedRecipe.Name);

                OnPropertyChanged();
            }
        }


        private ExportRecipeOptionDetailViewModel _recipeDetailVM;
        public ExportRecipeOptionDetailViewModel RecipeDetailVM
        {
            get => _recipeDetailVM; private set { if (_recipeDetailVM != value) { _recipeDetailVM = value; OnPropertyChanged(); } }
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

        public ExportRecipeViewModel()
        {
            MenuName = "Export Recipe";

            CommandMenuItems.Add(new MenuItemViewModel()
            {
                Name = "Refresh",
                Description = "Refresh",
                ExecuteCommand = RefreshCommand,
                ImageResourceKey = "Refresh",
                IconText = "Refresh"
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
                    var recipes = recipeProxy.GetADCRecipes();
                    _recipeTypes.Clear();
                    _recipeTypes.Add(AllType);
                    _recipeTypes.AddRange(Enum.GetNames(typeof(eModuleID)));
                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
                    {
                        Recipes = CollectionViewSource.GetDefaultView(recipes);
                        _recipesView.Filter = RecipeFilter;
                        SelectedRecipe = recipes.FirstOrDefault();
                        OnPropertyChanged(nameof(RecipeTypes));
                        SelectedRecipeType = AllType;
                    }));
                }
                catch (Exception ex)
                {
                    Services.Services.Instance.LogService.LogError("Refresh recipe", ex);
                    System.Windows.Application.Current.Dispatcher.Invoke((() => { AdcTools.ExceptionMessageBox.Show("Refresh error: ", ex); }));
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
            Dto.Recipe recipe = (obj as Dto.Recipe);
            return (Filter == null || recipe.Name.ToLower().Contains(Filter.ToLower()));
                //&& (_recipeTypeFilter == null || recipe.DataLoaderTypesList.Any(x => x == _recipeTypeFilter));
        }

        public void Refresh()
        {
            Init();
        }

        public bool MustBeSave => false;

        #region Commands

        private AutoRelayCommand _refreshCommand = null;
        public AutoRelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand ?? (_refreshCommand = new AutoRelayCommand(
              () =>
              {
                  Init();
                  Services.Services.Instance.LogService.LogDebug("Refresh export recipe");
              },
              () => { return true; }));
            }
        }

        #endregion
    }
}
