using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs;

using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel.InsertModule
{
    public class InsertModuleViewModel: ObservableObject, IModalDialogViewModel
    {
        private ServiceInvoker<IDbRecipeService> _dbRecipeService;
        private int _stepId;
        private Guid? _parentRecipeKey;
        private ILogger _logger;
        private List<RecipeInfo> _recipeInfos;
        private List<Guid> _allDataflowRecipes;
        private IDFClientConfiguration _dfClientConfiguration;

        public List<ActorTypeViewModel> ActorTypes { get; private set; }
        public ObservableCollection<RecipeViewModel> Recipes { get; private set; }

        public DataflowRecipeComponent InsertResult { get; private set; }


        private ActorTypeViewModel _selectedActorType;
        public ActorTypeViewModel SelectedActorType
        {
            get => _selectedActorType; 
            set 
            { 
                if (_selectedActorType != value) 
                {
                    _selectedActorType = value; 
                    OnPropertyChanged();
                    UpdateRecipes();
                } 
            }
        }
     

        private void UpdateRecipes()
        {
            Recipes.Clear();
            if (SelectedActorType.ActorType != ActorType.Unknown)
            {
                var newRecipeInfo = new RecipeInfo() { Name = "", ActorType = SelectedActorType.ActorType };
                var newRecipe = new RecipeViewModel(newRecipeInfo) { Text = string.Format("< New  {0}>", SelectedActorType.Text), RecipeState = RecipeState.New };
                Recipes.Add(newRecipe);

                foreach (var recipeInfo in _recipeInfos.Where(x => x.ActorType == SelectedActorType.ActorType))
                {
                    Recipes.Add(new RecipeViewModel(recipeInfo));
                }
            }
            else
            {
                foreach (var recipeInfo in _recipeInfos)
                {
                    Recipes.Add(new RecipeViewModel(recipeInfo));
                }
            }

            

            OnPropertyChanged(nameof(Recipes));
            SelectedRecipe = Recipes.FirstOrDefault();
        }

        private RecipeViewModel _selectedRecipe;
        public RecipeViewModel SelectedRecipe
        {
            get => _selectedRecipe; 
            set
            {
                if (_selectedRecipe != value) 
                {
                    _selectedRecipe = value; 
                    OnPropertyChanged();
                    UpdateRecipeSummary();
                    CloneCommand.NotifyCanExecuteChanged();
                    InsertCommand.NotifyCanExecuteChanged();
                } 
            }
        }

        // View is busy
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        // Close view
        private bool _closeTrigger;
        public bool CloseTrigger
        {
            get => _closeTrigger; set { if (_closeTrigger != value) { _closeTrigger = value; OnPropertyChanged(); } }
        }

        public bool? DialogResult { get; private set; }

        public InsertModuleViewModel(int stepId, Guid? parentRecipeKey, List<Guid> allDataflowRecipes)
        {
            _dbRecipeService = ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();
            _parentRecipeKey = parentRecipeKey;
            _logger = ClassLocator.Default.GetInstance<ILogger<InsertModuleViewModel>>();
            _stepId = stepId;
            _allDataflowRecipes = allDataflowRecipes;
            _dfClientConfiguration = ClassLocator.Default.GetInstance<IDFClientConfiguration>();
            Init();
        }

        private void Init()
        {
            IsBusy = true;
            Task.Factory.StartNew(() =>
            {
                try
                {                    
                    _recipeInfos = _dbRecipeService
                                        .Invoke(x => x.GetCompatibleRecipes(_parentRecipeKey, _stepId, _dfClientConfiguration.ToolKey))
                                        .OrderBy(r => r.Name)
                                        .ToList();
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Recipes = new ObservableCollection<RecipeViewModel>();
                        ActorTypes = new List<ActorTypeViewModel>();
                        ActorTypes.Add(new ActorTypeViewModel() { Text = "< ALL >" });
                        
                        foreach (var actor in _dfClientConfiguration.AvailableModules.Where(x => _parentRecipeKey.HasValue || x.GetCatgory() == ActorCategory.ProcessModule || x.GetCatgory() == ActorCategory.PostProcessing))
                        {
                            ActorTypes.Add(new ActorTypeViewModel() { Text = actor.ToString(), ActorType = actor });
                        }


                        OnPropertyChanged(nameof(ActorTypes));
                        SelectedActorType = ActorTypes.First();
                    });
                }
                catch (Exception ex)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        _logger.Error("GetCompatibleRecipes error", ex);
                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "Impossible to get compatible recipes");
                    });
                }
                finally
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() => { IsBusy = false; });
                }
            });
        }


        private UserControl _currentRecipeSummaryUC;
        public UserControl CurrentRecipeSummaryUC
        {
            get => _currentRecipeSummaryUC; set { if (_currentRecipeSummaryUC != value) { _currentRecipeSummaryUC = value; OnPropertyChanged(); } }
        }

        private void UpdateRecipeSummary()
        {
            if (SelectedRecipe != null && SelectedRecipe.RecipeState != RecipeState.New)
            {
                var recipeSummary = ClassLocator.Default.GetInstance<ExternalUserControls>().GetRecipeSummary(SelectedRecipe.ActorType);
                if (recipeSummary != null)
                {
                    recipeSummary.Init(true);
                    recipeSummary.LoadRecipe(SelectedRecipe.Key);
                }

                CurrentRecipeSummaryUC = recipeSummary as UserControl;
            }
            else
            {
                CurrentRecipeSummaryUC = null;
            }
        }        
        #region Command


        // Insert
        private AutoRelayCommand _insertCommand;
        public AutoRelayCommand InsertCommand
        {
            get
            {
                return _insertCommand ?? (_insertCommand = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      if(SelectedRecipe.Key != Guid.Empty && _allDataflowRecipes.Contains(SelectedRecipe.Key))
                      {
                          _logger.Information($"{SelectedRecipe.Text} already exist in dataflow");
                          ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"{SelectedRecipe.Text} already exist in dataflow","Insert recipe", MessageBoxButton.OK, MessageBoxImage.Warning);
                          return;
                      }

                      if (SelectedRecipe.RecipeState == RecipeState.New)
                      {
                          var recipeEditor = ClassLocator.Default.GetInstance<ExternalUserControls>().GetRecipeEditor(SelectedRecipe.ActorType);
                          recipeEditor.Init(true);
                          var recipeName = SelectedRecipe.ActorType + DateTime.Now.ToString();
                          var userId = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id;
                          var resultInfo = recipeEditor.CreateNewRecipe(recipeName, _stepId, userId);
                          InsertResult = _dbRecipeService.Invoke(x => x.GetLastRecipe(resultInfo.Key, false, false)).ToDataflowRecipeComponent();
                      }
                      else
                      {
                          InsertResult = _dbRecipeService.Invoke(x => x.GetLastRecipe(SelectedRecipe.Key, false, false)).ToDataflowRecipeComponent();
                      }
                      DialogResult = true;
                      CloseTrigger = true;
                  }
                  catch (Exception ex)
                  {
                      _logger.Error("insert recipe error", ex);
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "Insert recipe error");
                  }
              },
              () => { return SelectedRecipe != null && (SelectedRecipe.RecipeState == RecipeState.New || SelectedRecipe.RecipeState == RecipeState.Shared || SelectedRecipe.RecipeState == RecipeState.Local); }));
            }
        }

        // Clone
        private AutoRelayCommand _cloneCommand;
        public AutoRelayCommand CloneCommand
        {
            get
            {
                return _cloneCommand ?? (_cloneCommand = new AutoRelayCommand(
              () =>
              {
                  
                  try
                  {
                      int userId = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id;
                      string finalRecipeName = GenerateClonedRecipeName(SelectedRecipe.Text);
                      var clonedRecipeId = _dbRecipeService.Invoke(x => x.CloneRecipe(SelectedRecipe.Key, finalRecipeName, userId));
                      InsertResult = _dbRecipeService.Invoke(x => x.GetRecipe(clonedRecipeId, false)).ToDataflowRecipeComponent();
                      DialogResult = true;
                      CloseTrigger = true;
                  }
                  catch(Exception ex)
                  {
                      _logger.Error("Clone recipe error", ex);
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "Clone recipe error");
                  }                  
              },
              () => { return SelectedRecipe != null && SelectedRecipe.RecipeState != RecipeState.New; }));
            }
        }

        // Cancel
        private AutoRelayCommand _cancelCommand;
        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () =>
              {
                  DialogResult = false;
                  CloseTrigger = true;
              },
              () => { return true; }));
            }
        }

        #endregion
        public string GenerateClonedRecipeName(string originalName, int maxLength = 50)
        {
            if (string.IsNullOrWhiteSpace(originalName))
                originalName = "Recipe";

            string suffix = "Clone" + DateTime.Now.ToString("yyyyMMddHHmmss");

            int baseNameMaxLength = maxLength - suffix.Length;
            if (baseNameMaxLength < 0) baseNameMaxLength = 0;

            string truncatedName = originalName.Length > baseNameMaxLength
                ? originalName.Substring(0, baseNameMaxLength)
                : originalName;

            return truncatedName + suffix;
        }

    }
}
