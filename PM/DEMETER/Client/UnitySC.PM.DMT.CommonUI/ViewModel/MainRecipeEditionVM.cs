using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.DMT.CommonUI.Message;
using UnitySC.PM.DMT.CommonUI.Proxy;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Service.Interface.RecipeService;
using UnitySC.PM.DMT.Shared.UI;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.Shared.Hardware.ClientProxy.Global;
using UnitySC.PM.Shared.Helpers;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.Dialog.ExceptionDialogs;
using UnitySC.Shared.UI.ViewModel.Navigation;

namespace UnitySC.PM.DMT.CommonUI.ViewModel
{
    public class MainRecipeEditionVM : NavigationVM, IRecipeManager, IRecipient<StageChangedMessage>
    {
        private readonly RecipeSupervisor _recipeSupervisor;
        private readonly AlgorithmsSupervisor _algorithmsSupervisor;
        private readonly CalibrationSupervisor _calibrationSupervisor;
        private readonly SharedSupervisors _sharedSupervisors;
        private readonly IDialogOwnerService _dialogService;
        private readonly Mapper _mapper;

        public override string PageName => "DEMETER";

        private GlobalDeviceVM _globalDeviceVM;

        public GlobalDeviceVM GlobalDeviceVM
        {
            get => _globalDeviceVM; set { if (_globalDeviceVM != value) { _globalDeviceVM = value; OnPropertyChanged(); } }
        }

        public MainRecipeEditionVM(CameraSupervisor cameraSupervisor, RecipeSupervisor recipeSupervisor,
            AlgorithmsSupervisor algorithmsSupervisor, CalibrationSupervisor calibrationSupervisor, SharedSupervisors sharedSupervisors,
            IDialogOwnerService dialogService, Mapper mapper)
        {
            _recipeSupervisor = recipeSupervisor;
            _sharedSupervisors = sharedSupervisors;
            _dialogService = dialogService;
            _algorithmsSupervisor = algorithmsSupervisor;
            _calibrationSupervisor = calibrationSupervisor;
            _mapper = mapper;

            Task.Run(() =>
            {
                BackgroundInit();
            });
        }

        public override void Loaded()
        {
            base.Loaded();
            IsActive = true;
        }

        public override void Unloading()
        {
            base.Unloading();
            IsActive = false;
        }

        /// <summary>
        /// Fait les initialisation en tâche de fond, en particulier la comm avec le serveur DEMETER
        /// </summary>
        private async void BackgroundInit()
        {
            try
            {
                // Connect to to DEMETER server
                //.........................
                Logger.Information("Connecting to DEMETER server");

                GlobalDeviceVM = _sharedSupervisors.GetGlobalDeviceSupervisor(ActorType.DEMETER).GlobalDeviceVM;

                // Notify UI
                //..........
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    IsBusy = false;
                });
            }
            catch (Exception ex)
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    _dialogService.ShowException(ex, "Failed to connect to DEMETER server: ");
                    Environment.Exit(0);
                });
            }
        }

        public event EventHandler OnEndRecipeEdition;

        #region IRecipeManager Implementation

        public bool SetEditedRecipe(Guid recipeKey, bool isNewRecipe, bool forceReload = false)
        {
            try
            {

                var recipe = _recipeSupervisor.GetLastRecipeWithProductAndStep(recipeKey);
                if (!forceReload)
                {
                    if (recipe is null)
                        return false;

                    if (EditedRecipe != null && recipe.Name == EditedRecipe.Name)
                        return true;

                    if (!CanClose())
                        return false;
                }

                EditedRecipe = _mapper.AutoMap.Map<RecipeEditionVM>(recipe);
                EditedRecipe.IsRecipeModified = false;
                EditedRecipe.IsNewRecipe = isNewRecipe;
                EditedRecipe.Measures.ForEach(measure =>
                {
                    measure.WaferDimensions = recipe.Step?.Product.WaferCategory.DimentionalCharacteristic;
                });
                return true;
            }
            catch (Exception e) when (e.Data.Contains(RecipeLoadImportExceptionDataKeys.LoadCheckErrors) ||
                                      e.Data.Contains(RecipeLoadImportExceptionDataKeys.LoadCheckErrors))
            {
                _dialogService.ShowMessageBox(GetMessageFromRecipeLoadImportException(e), "Select recipe error", MessageBoxButton.OK, MessageBoxImage.Error);
                EditedRecipe = null;
            }
            catch (Exception e)
            {
                _dialogService.ShowException(e, "Select recipe error");
                EditedRecipe = null;
            }
            return false;
        }

        public static string GetMessageFromRecipeLoadImportException(Exception e, bool import = false)
        {
            var checkErrorKey = import
                ? RecipeLoadImportExceptionDataKeys.ImportCheckErrors
                : RecipeLoadImportExceptionDataKeys.LoadCheckErrors;
            var errorKey = import
                ? RecipeLoadImportExceptionDataKeys.ImportErrors
                : RecipeLoadImportExceptionDataKeys.LoadErrors;
            string message = string.Empty;
            if (e.Data.Contains(checkErrorKey) && e.Data[checkErrorKey] is List<RecipeCheckError> checkErrorEnumVales)
            {
                message += GetRecipeLoadingErrorMessage(checkErrorEnumVales, true);
            }

            if (e.Data.Contains(errorKey) && e.Data[errorKey] is List<string> errorMessages)
            {
                if (message != string.Empty)
                {
                    message += "\n";
                }

                message += "Unexpected errors: " + String.Join("\n", errorMessages);
            }

            return message;
        }

        private static string GetRecipeLoadingErrorMessage(List<RecipeCheckError> checkErrors, bool importing = false)
        {
            string errorMessage = importing ? "The selected recipe cannot be imported on this process module. Cause(s):\n" :  "Failed to load the recipe. Cause(s) :\n";
            errorMessage += String.Join(";\n", checkErrors.Select(GetRecipeErrorForIdentifier)) + ".";
            
            return errorMessage;
        }

        public RecipeInfo SaveRecipe()
        {
            if (string.IsNullOrEmpty(EditedRecipe.Name))
            {
                _dialogService.ShowMessageBox("The recipe must have a name", "Save Recipe", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            if (EditedRecipe.IsNewRecipe)
            {
                // We check that the name is available because it must be unique
                var recipesList = _recipeSupervisor.GetRecipeList(EditedRecipe.StepId ?? 0, true);
                if (recipesList.FirstOrDefault(r => r.Name == EditedRecipe.Name) != null)
                {
                    _dialogService.ShowMessageBox($"The recipe name '{EditedRecipe.Name}' is already used.", "Save Recipe", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }

            DMTRecipe recipe = _mapper.AutoMap.Map<DMTRecipe>(EditedRecipe);
            try
            {
                _recipeSupervisor.SaveRecipe(recipe);
            }
            catch (Exception)
            {
                _dialogService.ShowMessageBox($"The recipe could not be saved.", "Save Recipe", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            EditedRecipe.IsRecipeModified = false;
            EditedRecipe.IsNewRecipe = false;
            return recipe;
        }

        public void ExportRecipe(Guid recipeKey, string folderPath)
        {
            var recipe = _recipeSupervisor.ExportRecipe(recipeKey);

            Directory.CreateDirectory(folderPath);
            try
            {
                IsBusy = true;
                string cleanedRecipeFileName = new PathString(recipe.Name).RemoveInvalidFilePathCharacters("_", false) + ".dmtrcp";
                string recipeFullFilePath = Path.Combine(folderPath, cleanedRecipeFileName);
                recipe.Serialize(recipeFullFilePath);              
            }
            catch (Exception e)
            {
                ClassLocator.Default.GetInstance<ILogger<IRecipeManager>>().Error(e, "Error during recipe export");
                _dialogService.ShowException(e, "Export recipe error");
            }
            finally { IsBusy = false; }
        }

        public RecipeInfo ImportRecipe(int stepId, int userId, string fullFilePath)
        {
            string xmlContent = File.ReadAllText(fullFilePath);
            xmlContent = DMTRecipeHelper.UpdateRecipeXmlIfNeeded(xmlContent);
            var recipeToImport = XML.DeserializeFromString<DMTRecipe>(xmlContent);

            var dbRecipeService = ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();
            recipeToImport.Name = RecipeSharedHelper.FindNewRecipeName(stepId, fullFilePath, ActorType.DEMETER, dbRecipeService);
            try
            {
                return _recipeSupervisor.ImportRecipe(recipeToImport, stepId, userId);
            }
            catch (Exception e) when (e.Data.Contains(RecipeLoadImportExceptionDataKeys.ImportCheckErrors) ||
                                      e.Data.Contains(RecipeLoadImportExceptionDataKeys.ImportErrors))
            {
                _dialogService.ShowMessageBox(GetMessageFromRecipeLoadImportException(e, true), "Import recipe error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                _dialogService.ShowException(e, "Import recipe error");
            }

            return null;
        }

        private static string GetRecipeErrorForIdentifier(RecipeCheckError errorIdentifier)
        {
            switch (errorIdentifier)
            {
                case RecipeCheckError.SideIncompatibility:
                    return "- the recipe has measures for a side not available on this process module";
                case RecipeCheckError.NotCompatiblePerspectiveCalibration:
                    return "- the recipe uses a perpective calibration not available on this process module";
                case RecipeCheckError.BrightFieldColorNotCompatible:
                    return "- the recipe uses a bright-field screen color not available on this process module";
                case RecipeCheckError.BrightFieldApplyUniformity:
                    return "- the recipe is applying system uniformity with no automatic exposure computation, which is not possible";
                case RecipeCheckError.NotCompatibleDeflectometryOutputs:
                    return "- the recipe has a defelctometry measure with outputs not available on this process module";
                case RecipeCheckError.NotCompatibleDeflectometryStandardFringe:
                    return "- the recipe has a deflectometry measure using a fringe period not available on this process module";
                case RecipeCheckError.NotCompatibleDeflectometryMultiFringe:
                    return "- the recipe has a deflectometry measure with fringe periods not available on this process module";
                case RecipeCheckError.NotAvailableMeasure:
                    return "- the recipe uses a measure that is not available on this process module";
                case RecipeCheckError.NotAvailableExposureMatchingCalibration:
                    return "- Failed to retrieve the exposure matching coefficient";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void EndRecipeEdition()
        {
            throw new NotImplementedException();
        }

        // a recipe can be saved when it is modified and it has a name
        public bool CanClose()
        {
            if (EditedRecipe != null && EditedRecipe.IsRecipeModified)
            {
                // We ask the user if he wants to save it
                var result = _dialogService.ShowMessageBox("The recipe has not been saved. Do you want to save it now ?", "Save Recipe", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (SaveRecipe() == null)
                        return false;
                }
                else
                {
                    if (EditedRecipe != null)
                        EditedRecipe.EndRecipeEdition();
                    ReloadCurrentEditedRecipe();
                }
            }
            if (EditedRecipe != null)
                EditedRecipe.EndRecipeEdition();
            return true;
        }

        private void ReloadCurrentEditedRecipe()
        {
            if (EditedRecipe != null)
            {
                SetEditedRecipe(EditedRecipe.Key, false, true);
            }
        }

        void IRecipeManager.EndRecipeEdition()
        {
            OnEndRecipeEdition?.Invoke(this, null);
        }

        public void Receive(StageChangedMessage message)
        {
            X = message.NewStage.X;
            Y = message.NewStage.Y;
        }

        #endregion IRecipeManager Implementation

        private bool _isBusy = true;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private double _x;

        public double X
        {
            get => _x; set { if (_x != value) { _x = value; OnPropertyChanged(); } }
        }

        private double _y;

        public double Y
        {
            get => _y; set { if (_y != value) { _y = value; OnPropertyChanged(); } }
        }

        private RecipeEditionVM _editeedRecipe;

        public RecipeEditionVM EditedRecipe
        {
            get => _editeedRecipe; set { if (_editeedRecipe != value) { _editeedRecipe = value; OnPropertyChanged(); } }
        }

        #region Command

        private AutoRelayCommand _subscribeCommand;

        public AutoRelayCommand SubscribeCommand
        {
            get
            {
                return _subscribeCommand ?? (_subscribeCommand = new AutoRelayCommand(
                    () =>
                    {
                        try
                        {
                        }
                        catch (Exception ex)
                        {
                            _dialogService.ShowException(ex, "Subscribe error");
                        }
                    },
                    () => true
                ));
            }
        }

        private AutoRelayCommand _unSubscribeCommand;

        public AutoRelayCommand UnsubscribeCommand
        {
            get
            {
                return _unSubscribeCommand ?? (_unSubscribeCommand = new AutoRelayCommand(
                    () =>
                    {
                        try
                        {
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, "Unsubscribe error");
                            var exceptionVieModel = new ExceptionDialogViewModel("Unsubscribe error: ", ex);
                            _dialogService.ShowDialog(this, exceptionVieModel);
                        }
                    },
                    () => true
                    ));
            }
        }

        #endregion Command
    }
}
