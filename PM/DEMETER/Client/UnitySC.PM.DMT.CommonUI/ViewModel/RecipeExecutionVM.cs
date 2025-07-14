using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using MvvmDialogs.FrameworkDialogs.FolderBrowser;

using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Shared.UI.Message;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.ZoomboxImage;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel.Navigation;

namespace UnitySC.PM.DMT.CommonUI.ViewModel
{
    public class RecipeExecutionVM : PageNavigationVM, IRecipient<ResultMessage>, IRecipient<RecipeMessage>
    {
        //=================================================================
        // Données internes
        //=================================================================

        #region Données internes

        private readonly RecipeSupervisor _recipeSupervisor;
        private readonly IDialogOwnerService _dialogService;

        private string _resultFolderFS;
        private string _resultFolderBS;

        #endregion Données internes

        //=================================================================
        // Constructeurs
        //=================================================================

        #region Constructeurs

        public RecipeExecutionVM(DMTRecipe recipe, RecipeSupervisor recipeSupervisor, IDialogOwnerService dialogService)
        {
            Recipe = recipe;
            _recipeSupervisor = recipeSupervisor;
            _dialogService = dialogService;
            _remoteProductionInfo = null;
        }

        #endregion Constructeurs

        //=================================================================
        // Proprités bindables
        //=================================================================

        #region Proprités bindables

        public override string PageName => "Recipe Execution";

        private RemoteProductionInfo _remoteProductionInfo;

        public RemoteProductionInfo RemoteProductionInfo
        {
            get => _remoteProductionInfo;
            set
            {
                if (_remoteProductionInfo != value)
                {
                    _remoteProductionInfo = value;
                    OnPropertyChanged();
                    StartRecipeCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private string _acqDestFolder;

        public string AcqDestFolder
        {
            get => _acqDestFolder;
            set
            {
                if (_acqDestFolder != value)
                {
                    _acqDestFolder = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsAcqDestFolderValid));
                    StartRecipeCommand.NotifyCanExecuteChanged();
                }
            }
        }


        public bool IsAcqDestFolderValid
        {
            get => (!AcqDestFolder.IsNullOrEmpty()) && Path.IsPathRooted(AcqDestFolder);

        }

        private bool _isRecipeRunning;

        public bool IsRecipeRunning
        {
            get => _isRecipeRunning;
            set
            {
                if (_isRecipeRunning != value)
                {
                    _isRecipeRunning = value;
                    CanNavigate = !_isRecipeRunning;
                    OnPropertyChanged();
                }
            }
        }

        private int _currentAcquisitionStep;

        public int CurrentAcquisitionStep
        {
            get => _currentAcquisitionStep;
            set
            {
                if (_currentAcquisitionStep != value)
                {
                    _currentAcquisitionStep = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalAcquisitionSteps = 1;

        public int TotalAcquisitionSteps
        {
            get => _totalAcquisitionSteps;
            set
            {
                if (_totalAcquisitionSteps != value)
                {
                    _totalAcquisitionSteps = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _currentComputationStep;

        public int CurrentComputationStep
        {
            get => _currentComputationStep;
            set
            {
                if (_currentComputationStep != value)
                {
                    _currentComputationStep = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalComputationSteps = 1;

        public int TotalComputationSteps
        {
            get => _totalComputationSteps;
            set
            {
                if (_totalComputationSteps != value)
                {
                    _totalComputationSteps = value;
                    OnPropertyChanged();
                }
            }
        }

        private static readonly Brush s_defaultProgessBarColor =
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF06B025"));

        private Brush _progessBarColor = s_defaultProgessBarColor;

        public Brush ProgessBarColor
        {
            get => _progessBarColor;
            set
            {
                if (_progessBarColor != value)
                {
                    _progessBarColor = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _acquisitionMessage;

        public string AcquisitionMessage
        {
            get => _acquisitionMessage;
            set
            {
                if (_acquisitionMessage != value)
                {
                    _acquisitionMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _computationMessage;

        public string ComputationMessage
        {
            get => _computationMessage;
            set
            {
                if (_computationMessage != value)
                {
                    _computationMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ImageVM> Results { get; } = new ObservableCollection<ImageVM>();

        #endregion Proprités bindables

        //=================================================================
        // Gestion des messages
        //=================================================================

        #region Gestion des messages

        private void ResultMessageHandler(ResultMessage m)
        {
            Logger.Debug($"RecipeExecutionVM ResultMessageHandler {m.Name} {m.Side} {m.Path}");

            if (m.Name == "folder" && m.Side == Side.Front)
            {
                _resultFolderFS = m.Path;
                return;
            }

            if (m.Name == "folder" && m.Side == Side.Back)
            {
                _resultFolderBS = m.Path;
                return;
            }

            Logger.Debug($"RecipeExecutionVM ResultMessageHandler not folder {m.Name} {m.Path}");
            var resultVM = new ImageVM
            {
                Name = m.Name,
                Path = m.Path,
                Side = m.Side == Side.Front ? "Front Side" : "Back Side",
                ImageType = GetImageType(m.Name)
            };
            Results.Add(resultVM);
        }

        private void RecipeMessageHandler(RecipeMessage m)
        {
            switch (m.Status.State)
            {
                case DMTRecipeState.ExecutionComplete:
                    CurrentAcquisitionStep = TotalAcquisitionSteps;
                    CurrentComputationStep = TotalComputationSteps;
                    IsRecipeRunning = false;
                    UpdateAllCanExecutes();
                    ComputationMessage = m.Status.Message;
                    break;

                case DMTRecipeState.Preparing:
                    AcquisitionMessage = m.Status.Message;
                    ComputationMessage = "";
                    IsRecipeRunning = true;
                    break;

                case DMTRecipeState.Aborted:
                    IsRecipeRunning = false;
                    UpdateAllCanExecutes();
                    switch (m.Status.Step)
                    {
                        case DMTRecipeExecutionStep.Acquisition:
                            CurrentAcquisitionStep = TotalAcquisitionSteps;
                            AcquisitionMessage = "Acquisition aborted";
                            break;

                        case DMTRecipeExecutionStep.Computation:
                            CurrentComputationStep = TotalComputationSteps;
                            ComputationMessage = "Computation aborted";
                            break;

                        default:
                            throw new NotImplementedException();
                    }

                    break;
                case DMTRecipeState.Failed:
                    IsRecipeRunning = false;
                    UpdateAllCanExecutes();
                    switch (m.Status.Step)
                    {
                        case DMTRecipeExecutionStep.Acquisition:
                            CurrentAcquisitionStep = TotalAcquisitionSteps;
                            AcquisitionMessage = "Acquisition failed";
                            break;

                        case DMTRecipeExecutionStep.Computation:
                            CurrentComputationStep = TotalComputationSteps;
                            ComputationMessage = "Computation failed";
                            break;

                        default:
                            throw new NotImplementedException();
                    }

                    break;

                case DMTRecipeState.Executing:
                case DMTRecipeState.AcquisitionComplete:
                    IsRecipeRunning = true;
                    switch (m.Status.Step)
                    {
                        case DMTRecipeExecutionStep.Acquisition:
                            TotalAcquisitionSteps = m.Status.TotalSteps;
                            CurrentAcquisitionStep = m.Status.CurrentStep;
                            AcquisitionMessage = m.Status.Message;
                            break;

                        case DMTRecipeExecutionStep.Computation:
                            TotalComputationSteps = m.Status.TotalSteps;
                            CurrentComputationStep = m.Status.CurrentStep;
                            ComputationMessage = m.Status.Message;
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (m.Status.State == DMTRecipeState.Failed || m.Status.State == DMTRecipeState.Aborted)
                ProgessBarColor = Brushes.Red;
            else
                ProgessBarColor = s_defaultProgessBarColor;
        }

        #endregion Gestion des messages

        //=================================================================
        // Commandes
        //=================================================================

        #region Commandes

        private AutoRelayCommand _selectAcqDestFolderCommand;

        public AutoRelayCommand SelectAcqDestFolderCommand =>
            _selectAcqDestFolderCommand ?? (_selectAcqDestFolderCommand = new AutoRelayCommand(
             () =>
             {
                 var settings = new FolderBrowserDialogSettings
                 {
                     SelectedPath = AcqDestFolder,
                     Description = "Select destination folder for results in engineering mode",
                     ShowNewFolderButton = true
                 };
                 bool? res = _dialogService.ShowFolderBrowserDialog(settings);
                 if (res == true)
                 {
                     AcqDestFolder = settings.SelectedPath;
                 }

             },
             () => { return true; }));

        private AsyncRelayCommand _startRecipeCommand;

        public AsyncRelayCommand StartRecipeCommand => _startRecipeCommand ?? (_startRecipeCommand =
            new AsyncRelayCommand(StartRecipeCommandActionAsync, () => IsAcqDestFolderValid));

        private async Task StartRecipeCommandActionAsync()
        {
            try
            {
                Results.Clear();
                CurrentAcquisitionStep = 0;
                CurrentComputationStep = 0;
                ComputationMessage = null;
                AcquisitionMessage = null;
                _resultFolderFS = null;
                _resultFolderBS = null;

                try
                {
                    if (!Directory.Exists(AcqDestFolder))
                    {
                        try
                        {
                            Directory.CreateDirectory(AcqDestFolder);
                        }
                        catch (Exception)
                        {
                            _dialogService.ShowMessageBox($"Failed to create directory '{AcqDestFolder}'.", "Error",
                                                 MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    await _recipeSupervisor.StartRecipeAsync(Recipe, AcqDestFolder, false);
                }
                catch (Exception ex)
                {
                    // if the destination directory already exists
                    // Do not change the text of the exception as it checked by the client
                    if (ex.Message.Contains("Destination directory is not empty"))
                    {
                        var result = _dialogService
                                .ShowMessageBox($"An output for the Wafer ID '{RemoteProductionInfo.ProcessedMaterial.WaferBaseName}' already exists." +
                                                Environment.NewLine + Environment.NewLine +
                                                "Do you want to overwrite the output ?", "Recipe execution",
                                                MessageBoxButton.YesNo, MessageBoxImage.Exclamation,
                                                MessageBoxResult.No);

                        if (result != MessageBoxResult.Yes)
                        {
                            return;
                        }

                        // Start again the recipe execution but this time the output can be overwritten
                        await _recipeSupervisor.StartRecipeAsync(Recipe, AcqDestFolder, true);
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowException(ex, "Failed to start recipe");
            }
        }

        private AutoRelayCommand _abortCommand;

        public AutoRelayCommand AbortCommand =>
            _abortCommand ?? (_abortCommand = new AutoRelayCommand(() => _recipeSupervisor.Abort(),
                                                                   () => !AcqDestFolder.IsNullOrEmpty()));

        private AutoRelayCommand _openResultFolderCommand;

        public AutoRelayCommand OpenResultFolderCommand =>
            _openResultFolderCommand ?? (_openResultFolderCommand = new AutoRelayCommand(
             () =>
             {
                 try
                 {                    
                   OpenFolderSafe(_resultFolderFS, "result folder front side");
                   OpenFolderSafe(_resultFolderBS, "result folder back side");
                 }
                 catch (Exception ex)
                 {
                     HandleException(ex, _resultFolderFS, _resultFolderBS);                   
                 }
             },
             () => _resultFolderFS != null));

        private AutoRelayCommand _saveResultsCommand;

        public AutoRelayCommand SaveResultsCommand =>
            _saveResultsCommand ?? (_saveResultsCommand = new AutoRelayCommand(SaveResultsCommandAction,
                                                                               () => _resultFolderFS != null));

        private void SaveResultsCommandAction()
        {
            var settings = new FolderBrowserDialogSettings
            {
                Description = "Save execution results",
                ShowNewFolderButton = true
            };
            bool? res = _dialogService.ShowFolderBrowserDialog(settings);
            if (res != true)
            {
                return;
            }

            ShellFileOperation.Copy(_resultFolderFS, Path.Combine(settings.SelectedPath, "Front"));
            if (!_resultFolderBS.IsNullOrEmpty())
            {
                ShellFileOperation.Copy(_resultFolderBS, Path.Combine(settings.SelectedPath, "Back"));
            }
        }

        #endregion Commandes

        //=================================================================
        // Fonctions
        //=================================================================

        #region Fonctions

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

        public static ImageType GetImageType(string name)
        {
            if (name.StartsWith("folder"))
            {
                return ImageType.Data;
            }

            if (name.StartsWith("fringe"))
            {
                return ImageType.Fringe;
            }

            switch (name)
            {
                case "brightfield":
                    return ImageType.Light;

                case "point":
                case "dark":
                case "backlight":
                case "highangledarkfield":
                    return ImageType.Dark;

                default:
                    return ImageType.Data;
            }
        }

        public void Receive(ResultMessage message)
        {
            Application.Current?.Dispatcher?.Invoke(() => ResultMessageHandler(message));
        }

        public void Receive(RecipeMessage message)
        {
            Application.Current?.Dispatcher?.Invoke(() => RecipeMessageHandler(message));
        }

        public DMTRecipe Recipe { get; set; }
        /// <summary>
        /// Tries to open a folder in Windows Explorer.
        /// </summary>
        /// <param name="folderPath">Path to the folder</param>
        /// <param name="folderDescription">Folder description</param>
        /// <returns>True if the folder was opened successfully, False otherwise</returns>
        private bool OpenFolderSafe(string folderPath, string folderDescription)
        {
            if (string.IsNullOrEmpty(folderPath))
                throw new ArgumentException("Folder path is null or empty", nameof(folderPath));

            if (!Directory.Exists(folderPath))
            {                
                var userMessage = $"The {folderDescription} '{folderPath}' was not found. It may have been moved or deleted.";                
                Messenger.Send(new UnitySC.Shared.Tools.Service.Message(MessageLevel.Error, userMessage));
                throw new DirectoryNotFoundException(userMessage);
            }

            try
            {
                Process.Start(folderPath);
                return true;
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to open {folderDescription} '{folderPath}'", ex);
            }
        }
        /// <summary>
        /// Handles exceptions by displaying a user-friendly message and logging details.
        /// </summary>
        /// <param name="ex">The thrown exception</param>
        /// <param name="folderPaths">Paths of the affected folders</param>
        private void HandleException(Exception ex, params string[] folderPaths)
        {
            string userMessage = "An error occurred while trying to open a folder. Please check permissions or verify the path integrity.";            

            Messenger.Send(new UnitySC.Shared.Tools.Service.Message(MessageLevel.Error, userMessage));
            string logMessage = $"Error opening folders.\n" +
               $"Exception: {ex.Message}\n";
            Logger.Error(logMessage);
        }
        #endregion Fonctions
    }
}
