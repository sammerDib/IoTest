using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs.FrameworkDialogs.SaveFile;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.RecipeRun
{
    public class RecipeRunSaveAsVM : ObservableObject, IDisposable
    {
        private ILogger _logger;
        private IDialogOwnerService _dialog;
        private Proxy.Recipe.ANARecipeSupervisor _recipeSupervisor;
        private RecipeRunVM _recipeRunVM;

        public RecipeRunSaveAsVM(RecipeRunVM recipeRunVM)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger>();
            _dialog = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            _recipeSupervisor = ClassLocator.Default.GetInstance<Proxy.Recipe.ANARecipeSupervisor>();
            _recipeRunVM = recipeRunVM;
        }

        #region Properties

        public bool ResultIsAvailable => !string.IsNullOrEmpty(ResultFolderPath);

        private string _resultFolderPath;

        public string ResultFolderPath
        {
            get => _resultFolderPath; set { if (_resultFolderPath != value) { _resultFolderPath = value; OnPropertyChanged(); OnPropertyChanged(nameof(ResultIsAvailable)); } }
        }

        private bool _isSavedToZipFile = true;

        public bool IsSavedToZipFile
        {
            get => _isSavedToZipFile; set { if (_isSavedToZipFile != value) { _isSavedToZipFile = value; OnPropertyChanged(); } }
        }

        private bool _isSavedToDatabase = true;

        public bool IsSavedToDatabase
        {
            get => _isSavedToDatabase; set { if (_isSavedToDatabase != value) { _isSavedToDatabase = value; OnPropertyChanged(); } }
        }

        private string _destZipFileName = string.Empty;

        public string DestZipFileName
        {
            get => _destZipFileName; set { if (_destZipFileName != value) { _destZipFileName = value; OnPropertyChanged(); } }
        }

        private string _lotName = string.Empty;

        public string LotName
        {
            get => _lotName; set { if (_lotName != value) { _lotName = value; OnPropertyChanged(); } }
        }

        private bool _isPopupOpened = false;

        public bool IsPopupOpened
        {
            get => _isPopupOpened; set { if (_isPopupOpened != value) { _isPopupOpened = value; OnPropertyChanged(); } }
        }

        #endregion Properties

        #region RelayCommands

        private AutoRelayCommand _browseZipFile;

        public AutoRelayCommand BrowseZipFile
        {
            get
            {
                return _browseZipFile ?? (_browseZipFile = new AutoRelayCommand(
                () =>
                {
                    var settings = new SaveFileDialogSettings
                    {
                        Title = "Save Zip file",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        Filter = "Zip file (*.zip) | *.zip;",
                        CheckFileExists = false,
                        DefaultExt = "*.zip"
                    };
                    var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();

                    var rep = dialogService.ShowSaveFileDialog(settings);
                    if (rep.HasValue && rep.Value)
                    {
                        DestZipFileName = settings.FileName;
                    }
                    IsPopupOpened = true;
                },
                () => { return true; }
                ));
            }
        }

        private bool CanSaveResults()
        {
            if (!IsSavedToDatabase && !IsSavedToZipFile)
                return false;
            if (IsSavedToZipFile && string.IsNullOrEmpty(DestZipFileName))
                return false;
            if (IsSavedToDatabase && string.IsNullOrEmpty(LotName))
                return false;

            return true;
        }
       
        private AutoRelayCommand _saveResults;

        public AutoRelayCommand SaveResults
        {
            get
            {
                return _saveResults ?? (_saveResults = new AutoRelayCommand(
                    async () =>
                    {
                        if (Directory.Exists(ResultFolderPath))
                        {
                            if (File.Exists(DestZipFileName))
                            {
                                _dialog.ShowMessageBox("The destination file already exists. Please select a new file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            _recipeRunVM.IsBusy = true;

                            try
                            {
                                StringBuilder message = new StringBuilder();
                                if (IsSavedToZipFile)
                                {
                                    _recipeRunVM.BusyMessage = "Saving results to zip file...";

                                    

                                    await Task.Run(() =>
                                    {
                                        ZipFile.CreateFromDirectory(ResultFolderPath, DestZipFileName);
                                    });

                                    message.AppendLine($"Save zip file to {DestZipFileName} successful");
                                }
                                if (IsSavedToDatabase)
                                {
                                    _recipeRunVM.BusyMessage = "Saving results to database...";
                                    await Task.Run(() =>
                                    {
                                        _recipeSupervisor.SaveCurrentResultInProductionDatabase(LotName);
                                    });

                                    message.AppendLine($"Save Lot {LotName} in production database successful");
                                }
                                _logger.Information(message.ToString());
                                _recipeRunVM.IsBusy = false;
                                _dialog.ShowMessageBox(message.ToString(), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            catch (Exception ex)
                            {
                                _recipeRunVM.IsBusy = false;
                                string message = "Failed to save the results";
                                _logger.Error(ex, message);
                                _dialog.ShowMessageBox(message.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            finally
                            {
                                IsPopupOpened = false;
                            }
                        }
                        else
                        {
                            string message = $"Shared result directoy is not accessible: {ResultFolderPath} \n Check ResultFolderPath in PMConfiguration";
                            _logger.Error(message);
                            _dialog.ShowMessageBox(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    },
                    () => { return CanSaveResults(); }
                ));
            }
        }
        #endregion RelayCommands

        public void Dispose()
        {            
            //base.OnDeactivated();
        }

    }
}
