using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using MvvmDialogs.FrameworkDialogs.OpenFile;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.RecipeExecutor
{
    public class RecipeExecutorVM : ObservableObject, IMenuContentViewModel
    {
        private IDialogOwnerService _dialogService;
        private ANARecipeSupervisor _recipeSupervisor;
        private IMessenger _messenger;

        public RecipeExecutorVM()
        {
            _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            _recipeSupervisor = ClassLocator.Default.GetInstance<ANARecipeSupervisor>();
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private void RecipeSupervisor_MeasurePointResultChangedEvent(UnitySC.Shared.Format.Metro.MeasurePointResult res, string resultFolderPath, DieIndex die)
        {
            string dieInfo = (!(die is null)) ? $"[Die Col: {die.Column} Row: {die.Row} ]" : string.Empty;
            _messenger.Send(new Message(MessageLevel.Information, $"[MeasurePointResult] {dieInfo} {res}"));
        }
        
        private void RecipeSupervisor_RecipeProgressChangedEvent(Service.Interface.Recipe.Execution.RecipeProgress recipeProgress)
        {
            _messenger.Send(new Message(MessageLevel.Information, $"[RecipeProgress] {recipeProgress}"));
            if (recipeProgress.RecipeProgressState == Service.Interface.Recipe.Execution.RecipeProgressState.Success)
            {
                IsBusy = false;
                _messenger.Send(new Message(MessageLevel.Information, $"[RecipeProgress] Ended"));
            }
            else if (recipeProgress.RecipeProgressState == Service.Interface.Recipe.Execution.RecipeProgressState.Error)
            {
                IsBusy = false;
                _messenger.Send(new Message(MessageLevel.Error, $"[RecipeProgress] Error {recipeProgress.Message}"));
            }
        }

        private string _recipePath;

        public string RecipePath
        {
            get => _recipePath;
            set
            {
                if (_recipePath != value)
                {
                    _recipePath = value;
                    StartRecipeCommand.NotifyCanExecuteChanged();
                    OnPropertyChanged();
                }
            }
        }

        public bool IsEnabled => true;

        public List<SpecificPositions> AvailablePositions
        {
            get => new List<SpecificPositions>
            {
                SpecificPositions.PositionChuckCenter,
                SpecificPositions.PositionHome,
                SpecificPositions.PositionManualLoad,
                SpecificPositions.PositionPark
            };
        }

        public SpecificPositions DefaultSpecificPosition
        {
            get => SpecificPositions.PositionChuckCenter;
        }

        public bool CanClose()
        {
            _recipeSupervisor.RecipeProgressChangedEvent -= RecipeSupervisor_RecipeProgressChangedEvent;
            _recipeSupervisor.MeasureResultChangedEvent -= RecipeSupervisor_MeasurePointResultChangedEvent;
            return true;
        }

        public void Refresh()
        {
            _recipeSupervisor.RecipeProgressChangedEvent += RecipeSupervisor_RecipeProgressChangedEvent;
            _recipeSupervisor.MeasureResultChangedEvent += RecipeSupervisor_MeasurePointResultChangedEvent;
        }

        #region Command

        private AutoRelayCommand _openFileCommand;

        public AutoRelayCommand OpenFileCommand
        {
            get
            {
                return _openFileCommand ?? (_openFileCommand = new AutoRelayCommand(
              () =>
              {
                  var settings = new OpenFileDialogSettings
                  {
                      Title = "Open ANALYSE recipe",
                      Filter = "ANALYSE recipe files (*.anarx)|*.anarx",
                  };

                  bool? result = _dialogService.ShowOpenFileDialog(settings);
                  if (result.HasValue && result.Value)
                  {
                      RecipePath = settings.FileName;
                  }
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _startRecipeCommand;

        public AutoRelayCommand StartRecipeCommand
        {
            get
            {
                return _startRecipeCommand ?? (_startRecipeCommand = new AutoRelayCommand(
              () =>
              {
                  ANARecipe recipe;
                  try
                  {
                      IsBusy = true;
                      recipe = XML.Deserialize<ANARecipe>(RecipePath);

                      // Add waferCharacteristic in recipe.Step.Product.WaferCategory
                      recipe.Step = new Step();
                      recipe.Step.Product = new Product();
                      recipe.Step.Product.WaferCategory = new WaferCategory();
                      recipe.Step.Product.WaferCategory.DimentionalCharacteristic = ClassLocator.Default.GetInstance<ChuckSupervisor>().ChuckVM.SelectedWaferCategory.DimentionalCharacteristic;
                      _messenger.Send(new Message(MessageLevel.Information, "Start recipe"));
                      _recipeSupervisor.StartRecipe(recipe);
                  }
                  catch (Exception ex)
                  {
                      IsBusy = false;
                      _dialogService.ShowException(ex, "Start recipe error");
                  }
              },

              () => { return !String.IsNullOrEmpty(RecipePath); }));
            }
        }

        #endregion Command
    }
}
