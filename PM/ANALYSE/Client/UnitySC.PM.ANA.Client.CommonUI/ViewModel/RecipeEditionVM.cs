using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.Shared.Helpers;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;
using UnitySC.PM.ANA.Shared;
using UnitySC.PM.Shared;
using System.Text.RegularExpressions;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel
{
    public class RecipeEditionVM : VMSharedBase, IRecipeManager
    {        
        public INavigationManager NavigationManager => ServiceLocator.NavigationManager;
        public GlobalStatusSupervisor GlobalStatusSupervisor => ServiceLocator.GlobalStatusSupervisor;
        public ServiceInvoker<IDbRecipeService> DbRecipeService => ServiceLocator.DbRecipeService;
        private string _recipeName;
        private IMessenger _messenger;

        public RecipeEditionVM()
        {
            // We display the first page
            //NavigationManager.NavigateToNextPage();
            NavigationManager.CurrentPageChanged += NavigationManager_CurrentPageChanged;
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
        }

        private void ConnexionStatusChanged(ConnexionStateForActor connexionStatus)
        {
            if (connexionStatus.Status == ConnexionState.Faulted)
            {
                var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
                if (EditedRecipe?.IsModified ?? false)
                {
                    Application.Current?.Dispatcher.BeginInvoke(new Action(
                        () =>
                        {
                            var res = dialogService.ShowMessageBox("Analyse Process Module server is unreachable, Recipe edition will be closed, do you want to save recipe now ?", "Save recipe", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                            if (res == MessageBoxResult.Yes)
                            {
                                try
                                {
                                    EmergencySaveRecipe();
                                }
                                catch (Exception ex)
                                {
                                    ClassLocator.Default.GetInstance<ILogger>().Error(ex, "Save recipe error");
                                    ServiceLocator.DialogService.ShowException(ex, "Save recipe error");
                                }
                            }
                        }));
                    EditedRecipe.IsModified = false;
                }
                // Call canLeave one time with forceClose = true to release resources
                NavigationManager?.CurrentPage?.CanLeave(null, true);
            }
        }

        private void CreateAllPages(ANARecipeVM editedRecipe)
        {
            var isWaferLessMode = ClassLocator.Default.GetInstance<IClientConfigurationManager>().IsWaferLessMode;
            if (!isWaferLessMode)
                NavigationManager.AllPages.Add(new RecipeAlignmentVM(editedRecipe));
            
            NavigationManager.AllPages.Add(new RecipeWaferMapVM(editedRecipe));
            NavigationManager.AllPages.Add(new RecipeAlignmentMarksVM(editedRecipe));
            NavigationManager.AllPages.Add(new RecipeMeasuresChoiceVM(editedRecipe));
            if (editedRecipe.WaferMap != null)
                NavigationManager.AllPages.Add(new RecipeDiesSelectionVM(editedRecipe));
#if HMI_DEV
            if (editedRecipe.WaferMap is null)
                NavigationManager.AllPages.Add(new RecipeDiesSelectionVM(editedRecipe));
#endif
            NavigationManager.AllPages.Add(new RecipeRunVM(editedRecipe));
        }

        private void NavigationManager_CurrentPageChanged(INavigable newPage, INavigable oldPage)
        {
        }

        public event EventHandler OnEndRecipeEdition;

        public bool CanClose()
        {
            if ((!(NavigationManager.CurrentPage is null)) && (!NavigationManager.CurrentPage.CanLeave(null)))
                return false;
            if (!(EditedRecipe is null) && EditedRecipe.IsModified)
            {
                // We ask the user if he wants to save it
                var result = ServiceLocator.DialogService.ShowMessageBox("The recipe has not been saved. Do you want to save it now ?", "Save Recipe", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (!SaveRecipe())
                        return false;
                }
            }
            EndRecipeEdition();
            return true;
        }

        public void EndRecipeEdition()
        {
            _messenger.Unregister<ConnexionStateForActor>(this);
            OnEndRecipeEdition?.Invoke(this, null);

            // We remove all the pages
            NavigationManager.RemoveAllPages();
            // We reset the referentials

            ServiceLocator.ReferentialSupervisor.DeleteSettings(ReferentialTag.Wafer);
            ServiceLocator.ReferentialSupervisor.DeleteSettings(ReferentialTag.Die);

            ServiceLocator.AxesSupervisor.AxesVM.WaferMap = null;
            ServiceLocator.AxesSupervisor.GoToHome(PM.Shared.Hardware.Service.Interface.Axes.AxisSpeed.Fast);
        }


        private string RemoveWaferLessLine(string comment)
        {
            var pattern = Consts.WaferLessCommentPattern;
            // Define the regex with the multiline option to match each line separately
            Regex regex = new Regex(pattern, RegexOptions.Multiline);

            // Find the first match
            Match match = regex.Match(comment);

            // If a match is found, remove the matched line
            if (match.Success)
            {
                comment = comment.Remove(match.Index, match.Length);

                // Remove the newline character after the match
                if (match.Index < comment.Length && comment[match.Index] == '\n')
                {
                    comment = comment.Remove(match.Index, 1);
                }
                else if (match.Index > 0 && comment[match.Index - 1] == '\r')
                {
                    comment = comment.Remove(match.Index - 1, 1);
                }
            }

            return comment;
        }

        public bool SaveRecipe()
        {
            try
            {
                // Manage Waferless mode save

                var isWaferLessMode = ClassLocator.Default.GetInstance<IClientConfigurationManager>().IsWaferLessMode;

                if (isWaferLessMode)
                {
                    MessageBox.Show("This recipe as been modified in wafer less mode. As it has not been tested on a wafer, results might be wrong", "Recipe Save", MessageBoxButton.OK, MessageBoxImage.Error);
                    EditedRecipe.IsWaferLessModified = true;
                    var waferLessComment = $"{Consts.WaferLessCommentPrefix} {DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")} {ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser?.Name}\n";

                    if (!string.IsNullOrEmpty(EditedRecipe.Comment))
                    { 
                        EditedRecipe.Comment = RemoveWaferLessLine(EditedRecipe.Comment);
                        EditedRecipe.Comment = EditedRecipe.Comment.Insert(0, waferLessComment);
                    }
                    else
                    { 
                        EditedRecipe.Comment =  waferLessComment;
                    }
                }
                else
                {
                    if (EditedRecipe.IsRunExecuted)
                    {
                        EditedRecipe.IsWaferLessModified = false;
                        EditedRecipe.Comment = RemoveWaferLessLine(EditedRecipe.Comment);
                    }
                }

                var mapper = ClassLocator.Default.GetInstance<Mapper>();
                var recipe = mapper.AutoMap.Map<ANARecipe>(EditedRecipe);

                ServiceLocator.ANARecipeSupervisor.SaveRecipe(recipe);
                EditedRecipe.IsModified = false;
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
                    ClassLocator.Default.GetInstance<ILogger>().Error(e, "Save recipe error");
                    ServiceLocator.DialogService.ShowException(ex, "Save recipe error");
                    return false;
                }
            }
        }

        public void EmergencySaveRecipe()
        {
            var incrementVersion = true;
            var mapper = ClassLocator.Default.GetInstance<Mapper>();
            var anaRecipe = mapper.AutoMap.Map<ANARecipe>(EditedRecipe);
            var dbRecipe = mapper.AutoMap.Map<Recipe>(anaRecipe);

            dbRecipe.AddOutput(ResultType.ANALYSE_Thickness);
            var recipeId = DbRecipeService.Invoke(x => x.SetRecipe(dbRecipe, incrementVersion));

            SaveExternalFiles(anaRecipe, recipeId);
        }

        private void SaveExternalFiles(ANARecipe anaRecipe, int recipeId)
        {
            // update external File key
            var externalFiles = SubObjectFinder.GetAllSubObjectOfTypeT<ExternalFileBase>(anaRecipe);
            foreach (var externalFile in externalFiles)
            {
                externalFile.Value.FileNameKey = string.Concat(externalFile.Key, externalFile.Value.FileExtension);
            }

            var userSupervisor = ClassLocator.Default.GetInstance<IUserSupervisor>();
            foreach (var externalFile in externalFiles.Select(x => x.Value))
            {
                var externalFileId = DbRecipeService.Invoke(x => x.SetExternalFile(externalFile, recipeId, userSupervisor.CurrentUser.Id));
            }
        }

        public bool SetEditedRecipe(Guid recipeKey, bool isNewRecipe, bool forceReload = false)
        {
            var recipe = ServiceLocator.ANARecipeSupervisor.GetRecipe(recipeKey, true);

            var mapper = ClassLocator.Default.GetInstance<Mapper>();

            EditedRecipe = mapper.AutoMap.Map<ANARecipeVM>(recipe);
            ServiceLocator.AxesSupervisor.AxesVM.WaferMap = null;
            ServiceLocator.ChuckSupervisor.ChuckVM.SelectWaferCategory(EditedRecipe.Step.Product.WaferCategoryId);

            _recipeName = EditedRecipe.Name;
            CreateAllPages(EditedRecipe);
            EditedRecipe.IsModified = false;

            _messenger.Register<ConnexionStateForActor>(this, (r, m) => ConnexionStatusChanged(m));

            return true;
        }

        public void ExportRecipe(Guid recipeKey, string folderPath)
        {
            var recipe = ServiceLocator.ANARecipeSupervisor.GetRecipe(recipeKey, true);
            var externalFiles = SubObjectFinder.GetAllSubObjectOfTypeT<ExternalFileBase>(recipe);

            //Create a folder to store external files.
            Directory.CreateDirectory(folderPath);

            //Creates a temporary folder to store files
            string tempFolderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            try
            {
                IsBusy = true;
                //Creates a temporary folder
                Directory.CreateDirectory(tempFolderPath);

                //Clean up the file name by replacing invalid characters with an underscore (_).
                string cleanedRecipeName = new PathString(recipe.Name).RemoveInvalidFilePathCharacters("_", false);

                //Copies the .anar file to the temporary folder
                string recipeFilePath = Path.Combine(tempFolderPath, $"{cleanedRecipeName}.anar");
                XML.Serialize(recipe, recipeFilePath);

                //Copies external files to the temporary folder
                foreach (var externalFile in externalFiles.Values)
                {
                    string fileName = Path.Combine(tempFolderPath, externalFile.FileNameKey);
                    externalFile.SaveToFile(fileName);
                }

                //Create the ZIP archive in the destination folder with the .anarx extension
                string zipFilePath = Path.Combine(folderPath, $"{cleanedRecipeName}.anarx");
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }
                ZipFile.CreateFromDirectory(tempFolderPath, zipFilePath);
            }
            catch (Exception ex)
            {
                ClassLocator.Default.GetInstance<ILogger>().Error(ex, "Export recipe error");
                ServiceLocator.DialogService.ShowException(ex, "Export recipe error");
            }
            finally
            {
                //Deletes the temporary folder
                Directory.Delete(tempFolderPath, recursive: true);
                IsBusy = false;
            }
        }

        private string ReplaceInvalidChars(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        public RecipeInfo ImportRecipe(int stepId, int userId, string fileName)
        {
            string tempFolderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolderPath);

            try
            {
                //Extract files from the ZIP into the temporary folder
                ZipFile.ExtractToDirectory(fileName, tempFolderPath);

                //Search for the .anar file in the temporary folder
                string anarFilePath = Directory.GetFiles(tempFolderPath, "*.anar").FirstOrDefault();

                if (anarFilePath == null)
                {
                    throw new InvalidOperationException("Le fichier .anar n'a pas été trouvé dans l'archive ZIP.");
                }

                //Deserialise the recipe from the .anar file

                

                var dbRecipeService = ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();
                
                string newRecipeName = RecipeSharedHelper.FindNewRecipeName(stepId, anarFilePath, ActorType.ANALYSE,dbRecipeService);

                string anaRecipeString = File.ReadAllText(anarFilePath);
                anaRecipeString=ANARecipeHelper.ConvertAnaRecipeIfNeeded(anaRecipeString);

                ANARecipe recipeToImport = XML.DeserializeFromString<ANARecipe>(anaRecipeString);

                //Create a new recipe with the imported information
                var newRecipe = ServiceLocator.ANARecipeSupervisor.CreateRecipe(newRecipeName, stepId, userId);
                recipeToImport.StepId = newRecipe.StepId;
                recipeToImport.UserId = newRecipe.UserId;
                recipeToImport.Created = newRecipe.Created;
                recipeToImport.ActorType = newRecipe.ActorType;
                recipeToImport.CreatorChamberId = newRecipe.CreatorChamberId;
                recipeToImport.Name = newRecipe.Name;
                recipeToImport.Key = newRecipe.Key;
                recipeToImport.FileVersion = ANARecipe.CurrentFileVersion;

                //Copies external files from the temporary folder to the appropriate path
                var externalFiles = SubObjectFinder.GetAllSubObjectOfTypeT<ExternalFileBase>(recipeToImport);

                foreach (var externalFile in externalFiles.Values)
                {
                    string filePath = Path.Combine(tempFolderPath, externalFile.FileNameKey);
                    externalFile.LoadFromFile(filePath);
                }

                //Saves the imported recipe in the database
                ServiceLocator.ANARecipeSupervisor.SaveRecipe(recipeToImport);

                return newRecipe;
            }
            catch (Exception ex)
            {
                ClassLocator.Default.GetInstance<ILogger>().Error(ex, "Import recipe error");
                ServiceLocator.DialogService.ShowException(ex, "Import recipe error");
            }
            finally
            {
                // Deletes the temporary folder
                try
                {
                    // Deletes the temporary folder
                    Directory.Delete(tempFolderPath, recursive: true);
                }
                catch (IOException)
                {
                    // Wait a short while to see if the files are released
                    Thread.Sleep(1000);
                    try
                    {
                        // Try deleting again
                        Directory.Delete(tempFolderPath, recursive: true);
                    }
                    catch (Exception innerEx)
                    {
                        ClassLocator.Default.GetInstance<ILogger>().Error(innerEx, "Error deleting temporary folder");
                    }
                }
            }
            return null;
        }        

        private AutoRelayCommand _doSaveRecipe;

        public AutoRelayCommand DoSaveRecipe
        {
            get
            {
                return _doSaveRecipe ?? (_doSaveRecipe = new AutoRelayCommand(
                    async () =>
                    {
                        IsBusy = true;
                        BusyMessage = "Saving the recipe";
                        Recipe recipe = null;

                        try
                        {
                            await Task.Run(() =>
                            {
                                var pmConfig = GlobalStatusSupervisor.GetConfiguration()?.Result;
                                recipe = DbRecipeService.Invoke(x => x.GetRecipe(pmConfig.Actor, EditedRecipe.Step.Id, EditedRecipe.Name, false, false));
                            });

                            if (recipe != null && _recipeName != EditedRecipe.Name)
                            {
                                ServiceLocator.DialogService.ShowMessageBox("Recipe name must be unique", "Save Recipe", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                await Task.Run(() => SaveRecipe());
                                _recipeName = EditedRecipe.Name;
                            }
                        }
                        catch (Exception ex)
                        {
                            ClassLocator.Default.GetInstance<ILogger>().Error(ex, "Error during saving the recipe");
                            ServiceLocator.DialogService.ShowException(ex, "Error during saving the recipe");
                        }
                        finally
                        {
                            IsBusy = false;
                        }
                        IsBusy = false;
                    },
                    () => { return (!(EditedRecipe is null) && EditedRecipe.IsModified); }
                ));
            }
        }

        private AutoRelayCommand _displayHelp;

        public AutoRelayCommand DisplayHelp
        {
            get
            {
                return _displayHelp ?? (_displayHelp = new AutoRelayCommand(
                    () =>
                    {
                        // Code to execute
                    },
                    () => { return false; }
                ));
            }
        }

        private ANARecipeVM _editedRecipe = null;

        public ANARecipeVM EditedRecipe
        {
            get => _editedRecipe; 
            set 
            { 
                if (_editedRecipe != value) 
                { 
                    if (_editedRecipe !=null)
                    {
                        _editedRecipe.PropertyChanged -= OnEditedRecipePropertyChanged;
                    }
                    _editedRecipe = value; 
                    _editedRecipe.PropertyChanged += OnEditedRecipePropertyChanged;
                    OnPropertyChanged(); 
                } 
            }
        }

        private void OnEditedRecipePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ANARecipeVM.IsModified)) 
            {

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    DoSaveRecipe.NotifyCanExecuteChanged();
                }));
                
            }
        }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private string _busyMessage = "";

        public string BusyMessage
        {
            get => _busyMessage; set { if (_busyMessage != value) { _busyMessage = value; OnPropertyChanged(); } }
        }
    }
}
