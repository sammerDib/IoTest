using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

using AcquisitionAdcExchange;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using Dto = UnitySC.DataAccess.Dto;

namespace ADCConfiguration.ViewModel.Recipe
{
    /// <summary>
    /// Permet de comparer deux recettes
    /// </summary>
    public class RecipeHistoryViewModel : ViewModelWithMenuBase, INavigationViewModel
    {
        private ICollectionView _recipesView;
        private Dto.Recipe _selectedRecipe;
        private List<string> _recipeTypes = new List<string>();
        private const string AllType = "All Types..";
        private const string SplitHorizontalDescription = "Split Horizontal";
        private const string SplitVerticalDescription = "Split Vertical";
        public List<string> RecipeTypes => _recipeTypes;
        private eModuleID? _recipeTypeFilter;
        private string _selectedRecipeType;
        private MenuItemViewModel _splitMenuItem;
        private MenuItemViewModel _synchroMenuItem;
        public RecipeHistoryDetailViewModel RecipeHistoryDetailVM { get; set; }

        public bool MustBeSave => false;

        /// <summary>
        /// Détermine si le panneau de choix de recettes est ouvert
        /// </summary>
        private bool _recipeExplorerIsOpen = true;
        public bool RecipeExplorerIsOpen
        {
            get => _recipeExplorerIsOpen; set { if (_recipeExplorerIsOpen != value) { _recipeExplorerIsOpen = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Type de recette pour la recherche
        /// </summary>
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

        /// <summary>
        /// Recette sélectionnée
        /// </summary>
        public Dto.Recipe SelectedRecipe
        {
            get { return _selectedRecipe; }
            set
            {
                _selectedRecipe = value;
                OnPropertyChanged();
                RecipeHistoryDetailVM.Init(_selectedRecipe?.Name);
            }
        }

        /// <summary>
        /// Liste des recettes filtrées
        /// </summary>
        public ICollectionView Recipes
        {
            get { return _recipesView; }
            set
            {
                _recipesView = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Filtre 
        /// </summary>
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

        public RecipeHistoryViewModel(IMessenger messenger)
        {
            MenuName = "Recipe history";
            RecipeHistoryDetailVM = new RecipeHistoryDetailViewModel(messenger);

            CommandMenuItems.Add(new MenuItemViewModel()
            {
                Name = "Refresh",
                Description = "Refresh",
                ExecuteCommand = RefreshCommand,
                ImageResourceKey = "Refresh",
                IconText = "Refresh"
            });

            _splitMenuItem = new MenuItemViewModel()
            {
                Name = "Split",
                Description = SplitHorizontalDescription,
                ExecuteCommand = SplitCommand,
                ImageResourceKey = "SplitHorizontal",
                IconText = "Split"
            };
            CommandMenuItems.Add(_splitMenuItem);

            _synchroMenuItem = new MenuItemViewModel()
            {
                Name = "Synchro",
                Description = "If Synchro is checked, synchronize base and compare view (Zoom, Position)",
                ExecuteCommand = SynchroCommand,
                ImageResourceKey = "Checked",
                IconText = "Synchro"
            };
            CommandMenuItems.Add(_synchroMenuItem);

            CommandMenuItems.Add(new MenuItemViewModel()
            {
                Name = "Previous",
                Description = "Previous difference",
                ExecuteCommand = RecipeHistoryDetailVM.PreviousCommand,
                ImageResourceKey = "Previous",
                IconText = "Prev."
            });

            CommandMenuItems.Add(new MenuItemViewModel()
            {
                Name = "Next",
                Description = "Next difference",
                ExecuteCommand = RecipeHistoryDetailVM.NextCommand,
                ImageResourceKey = "Next",
                IconText = "Next"
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
                    var recipes = recipeProxy.GetADCRecipes(null, false, false);
                    _recipeTypes.Clear();
                    _recipeTypes.Add(AllType);
                    _recipeTypes.AddRange(Enum.GetNames(typeof(eModuleID)));
                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
                    {
                        Recipes = CollectionViewSource.GetDefaultView(recipes);
                        _recipesView.Filter = RecipeFilter;
                        SelectedRecipe = null;
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
#warning ** USP ** filter avec result type TODO
        }


        public void Refresh()
        {
            Init();
        }

        private void SplitScreen()
        {
            RecipeHistoryDetailVM.SplitHorizontal = !RecipeHistoryDetailVM.SplitHorizontal;

            if (RecipeHistoryDetailVM.SplitHorizontal)
            {
                _splitMenuItem.Description = SplitHorizontalDescription;
                _splitMenuItem.ImageResourceKey = "SplitHorizontal";
            }
            else
            {
                _splitMenuItem.Description = SplitVerticalDescription;
                _splitMenuItem.ImageResourceKey = "SplitVertical";
            }
        }

        private void Synchro()
        {
            RecipeHistoryDetailVM.GraphsAreSynchro = !RecipeHistoryDetailVM.GraphsAreSynchro;
            if (RecipeHistoryDetailVM.GraphsAreSynchro)
            {
                _synchroMenuItem.ImageResourceKey = "Checked";
            }
            else
            {
                _synchroMenuItem.ImageResourceKey = "NotChecked";
            }
        }

        #region commands

        private AutoRelayCommand _commandCollapseRecipeExplorer;
        public AutoRelayCommand CommandCollapseRecipeExplorer
        {
            get
            {
                return _commandCollapseRecipeExplorer ?? (_commandCollapseRecipeExplorer = new AutoRelayCommand(
              () =>
              {
                  RecipeExplorerIsOpen = false;
              },
              () => { return true; }));
            }
        }


        private AutoRelayCommand _commandExpandRecipeExplorer;
        public AutoRelayCommand CommandExpandRecipeExplorer
        {
            get
            {
                return _commandExpandRecipeExplorer ?? (_commandExpandRecipeExplorer = new AutoRelayCommand(
              () =>
              {
                  RecipeExplorerIsOpen = true;
              },
              () => { return true; }));
            }
        }

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


        private AutoRelayCommand _splitCommand;
        public AutoRelayCommand SplitCommand
        {
            get
            {
                return _splitCommand ?? (_splitCommand = new AutoRelayCommand(
              () =>
              {
                  SplitScreen();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _synchroCommand;
        public AutoRelayCommand SynchroCommand
        {
            get
            {
                return _synchroCommand ?? (_synchroCommand = new AutoRelayCommand(
              () =>
              {
                  Synchro();
              },
              () => { return true; }));
            }
        }

        #endregion

    }
}
