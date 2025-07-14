using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Proxy.Chuck;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Client.Proxy.Light;
using UnitySC.PM.EME.Client.Recipe.ViewModel.Navigation;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.Shared.Helpers;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.EME.Client.Recipe.ViewModel
{
    public class RecipeEditionVM : ViewModelBaseExt, IRecipeManager
    {
        private readonly ICalibrationService _calibrationService;
        private readonly CameraBench _camera;
        private readonly ChuckVM _chuckViewModel;
        private readonly ServiceInvoker<IDbRecipeService> _dbRecipeService;
        private readonly IDialogOwnerService _dialogService;
        private readonly FilterWheelBench _filterWheelBench;
        private readonly GlobalStatusSupervisor _globalStatusSupervisor;
        private readonly LightBench _lightBench;
        private readonly ILogger _logger;
        private readonly Mapper _mapper = ClassLocator.Default.GetInstance<Mapper>();
        private readonly IMessenger _messenger;
        private readonly IEMERecipeService _recipeSupervisor;
        private readonly IUserSupervisor _userSupervisor;

        private AutoRelayCommand _doSaveRecipe;
        private EMERecipeVM _editedRecipe;

        private bool _isBusy;
        private string _busyMessage = "";
        private bool _isSaveResultsEnabled;
        private string _recipeName;

        public RecipeEditionVM(IEMERecipeService recipeSupervisor,
            ChuckVM chuckViewModel,
            ICalibrationService calibrationService,
            GlobalStatusSupervisor globalStatusSupervisor,
            ServiceInvoker<IDbRecipeService> dbRecipeService,
            CameraBench camera,
            FilterWheelBench filterWheelBench,
            LightBench lightBench,
            IDialogOwnerService dialogService,
            IUserSupervisor userSupervisor,
            ILogger logger,
            IMessenger messenger,
            INavigationManagerForRecipeEdition navigationManager)
        {
            _recipeSupervisor = recipeSupervisor;
            _calibrationService = calibrationService;
            _globalStatusSupervisor = globalStatusSupervisor;
            _chuckViewModel = chuckViewModel;
            _dbRecipeService = dbRecipeService;
            _camera = camera;
            _filterWheelBench = filterWheelBench;
            _lightBench = lightBench;
            _dialogService = dialogService;
            _userSupervisor = userSupervisor;
            _logger = logger;
            _messenger = messenger;
            NavigationManager = navigationManager;
        }

        public INavigationManager NavigationManager { get; }

        public AutoRelayCommand DoSaveRecipe =>
            _doSaveRecipe ?? (_doSaveRecipe = new AutoRelayCommand(
                async () =>
                {
                    IsBusy = true;
                    BusyMessage = "Saving the recipe";
                    DataAccess.Dto.Recipe recipeAlreadyExistingWithSameName = null;
                    try
                    {
                        await Task.Run(() =>
                        {
                            var pmConfig = _globalStatusSupervisor.GetConfiguration().Result;
                            recipeAlreadyExistingWithSameName = _dbRecipeService.Invoke(x =>
                                x.GetRecipe(pmConfig.Actor, EditedRecipe.Step.Id, EditedRecipe.Name, false, false));
                        });

                        if (DoesEditedRecipeNameExist(recipeAlreadyExistingWithSameName))
                        {
                            _dialogService.ShowMessageBox("Recipe name must be unique", "Save Recipe",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            await Task.Run(SaveRecipe);
                            _recipeName = EditedRecipe.Name;
                        }
                    }
                    catch (Exception ex)
                    {
                        ClassLocator.Default.GetInstance<ILogger>().Error(ex, "Error during saving the recipe");
                        _dialogService.ShowException(ex, "Error during saving the recipe");
                    }
                    finally
                    {
                        IsBusy = false;
                        EditedRecipe.IsModified = false;
                    }

                    IsBusy = false;
                },
                () => !(EditedRecipe is null) && EditedRecipe.IsModified));


        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }


        public string BusyMessage
        {
            get => _busyMessage;
            set
            {
                if (_busyMessage != value)
                {
                    _busyMessage = value;
                    OnPropertyChanged();
                }
            }
        }



        public EMERecipeVM EditedRecipe
        {
            get => _editedRecipe;
            set
            {
                if (_editedRecipe != value)
                {
                    if (_editedRecipe != null)
                    {
                        _editedRecipe.PropertyChanged -= OnEditedRecipePropertyChanged;
                    }

                    _editedRecipe = value;
                    _editedRecipe.PropertyChanged += OnEditedRecipePropertyChanged;
                    OnPropertyChanged();
                }
            }
        }

        public bool CanClose()
        {
            if (!(EditedRecipe is null) && EditedRecipe.IsModified)
            {
                // We ask the user if he wants to save it
                var result = _dialogService.ShowMessageBox(
                    "The recipe has not been saved. Do you want to save it now ?", "Save Recipe",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (!SaveRecipe())
                        return false;
                }
            }
            EndRecipeEdition();
            return true;
        }

        public void ExportRecipe(Guid recipeKey, string folderPath)
        {
            try
            {
                var recipe = _recipeSupervisor.GetRecipeFromKey(recipeKey)?.Result;
                if (recipe == null)
                {
                    throw new ArgumentException($"Recipe with the provided key {recipeKey} does not exist.");
                }

                Directory.CreateDirectory(folderPath);
                string cleanedRecipeName = new PathString(recipe.Name).RemoveInvalidFilePathCharacters("_", false);
                string recipeFilePath = Path.Combine(folderPath, $"{cleanedRecipeName}.emer");
                recipe.Serialize(recipeFilePath);
                _logger.Information("Recipe exported successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Export recipe error");
                _dialogService.ShowException(ex, "Export Recipe Error");
            }
        }

        public RecipeInfo ImportRecipe(int stepId, int userId, string emerFilePath)
        {
            try
            {
                var recipeToImport = XML.Deserialize<EMERecipe>(emerFilePath);
                string newRecipeName =
                    RecipeSharedHelper.FindNewRecipeName(stepId, emerFilePath, ActorType.EMERA, _dbRecipeService);
                var newRecipe = _recipeSupervisor.CreateRecipe(newRecipeName, stepId, userId)?.Result;
                if (newRecipe == null)
                {
                    throw new ArgumentException(
                        $"Unable to create a new recipe with the name '{newRecipeName}' for step ID '{stepId}' and user ID '{userId}'.");
                }

                recipeToImport.StepId = newRecipe.StepId;
                recipeToImport.UserId = newRecipe.UserId;
                recipeToImport.Created = newRecipe.Created;
                recipeToImport.ActorType = newRecipe.ActorType;
                recipeToImport.CreatorChamberId = newRecipe.CreatorChamberId;
                recipeToImport.Name = newRecipe.Name;
                recipeToImport.Key = newRecipe.Key;

                _recipeSupervisor.SaveRecipe(recipeToImport, true, _userSupervisor.CurrentUser.Id);
                return newRecipe;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Import recipe error");
                _dialogService.ShowException(ex, "Import recipe error");
                throw;
            }
        }

        public bool SaveRecipe()
        {
            try
            {
                var recipe = _mapper.AutoMap.Map<EMERecipe>(EditedRecipe);

                _recipeSupervisor.SaveRecipe(recipe, true, _userSupervisor.CurrentUser.Id);
                EditedRecipe.IsModified = false;
                UpdateAllCanExecutes();
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    EmergencySaveRecipe();
                    return true;
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Save recipe error");
                    _dialogService.ShowException(ex, "Save recipe error");
                    return false;
                }
            }
        }

        public bool SetEditedRecipe(Guid recipeKey, bool isNewRecipe, bool forceReload = false)
        {
            var calibrations = _calibrationService.GetCalibrations().Result;
            if (calibrations.Count() < _calibrationService.GetNeededCalibrationCount().Result)
            {
                string msg = "Not all calibrations have been found or performed.";
                _logger.Error(msg);
                _messenger.Send(msg);
                throw new Exception(msg);
            }

            var recipe = _recipeSupervisor.GetRecipeFromKey(recipeKey).Result;

            EditedRecipe = _mapper.AutoMap.Map<EMERecipeVM>(recipe);
            _chuckViewModel.SelectWaferCategory(EditedRecipe.Step.Product.WaferCategoryId);

            _recipeName = EditedRecipe.Name;
            CreateAllPages(EditedRecipe);
            EditedRecipe.IsModified = false;
            return true;
        }

        private void OnEditedRecipePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EMERecipeVM.IsModified))
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {

                    DoSaveRecipe.NotifyCanExecuteChanged();
                }));
            }
        }

        private void CreateAllPages(EMERecipeVM editedRecipe)
        {
            NavigationManager.AllPages.Clear();

            NavigationManager.AllPages.Add(new ExecutionSettingsViewModel(editedRecipe));
            NavigationManager.AllPages.Add(new AcquisitionsEditorViewModel(editedRecipe, _camera, _filterWheelBench,
                _lightBench, _dialogService, _logger, _messenger));
            
            NavigationManager.AllPages.Add(new RecipeExecutionViewModel(_messenger, _recipeSupervisor, editedRecipe));

            NavigationManager.NavigateToPage(NavigationManager.GetFirstPage());
        }

        private bool DoesEditedRecipeNameExist(DataAccess.Dto.Recipe recipeAlreadyExistingWithSameName)
        {
            return recipeAlreadyExistingWithSameName != null && _recipeName != EditedRecipe.Name;
        }

        private void EmergencySaveRecipe()
        {
            bool incrementVersion = true;
            var emeRecipe = _mapper.AutoMap.Map<EMERecipe>(EditedRecipe);
            var dbRecipe = _mapper.AutoMap.Map<DataAccess.Dto.Recipe>(emeRecipe);

            dbRecipe.AddOutput(ResultType.NotDefined);
            _dbRecipeService.Invoke(x => x.SetRecipe(dbRecipe, incrementVersion));
        }
        public void EndRecipeEdition()
        {
            NavigationManager.RemoveAllPages();           
        }
    }
}
