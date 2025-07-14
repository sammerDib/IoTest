using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using ADCConfiguration.Services;

using AdcTools.Widgets;

using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Dto = UnitySC.DataAccess.Dto;

namespace ADCConfiguration.ViewModel.Recipe
{
    public class ImportRecipeViewModel : ViewModelWithMenuBase, INavigationViewModel
    {
        private ADCEngine.Recipe _recipe;
        private ObservableCollection<FileStatusViewModel> _filesStatus;
        private string _recipeName;

        public ObservableCollection<FileStatusViewModel> FilesStatus
        {
            get
            {
                return _filesStatus;
            }
        }


        private string _comment;
        public string Comment
        {
            get => _comment; set { if (_comment != value) { _comment = value; OnPropertyChanged(); } }
        }

        public string DataLoaderTypes
        {
            get => _recipe != null ? string.Join(", ", _recipe.DataLoaderActorTypes.Select(x => x.ToString())) : string.Empty;
        }

        private FileStatusViewModel _recipeStatus;
        public FileStatusViewModel RecipeStatus
        {
            get => _recipeStatus; set { if (_recipeStatus != value) { _recipeStatus = value; OnPropertyChanged(); } }
        }

        private bool _recipeLoaded;
        public bool RecipeLoaded
        {
            get => _recipeLoaded; set { if (_recipeLoaded != value) { _recipeLoaded = value; OnPropertyChanged(); } }
        }


        public ImportRecipeViewModel()
        {
            MenuName = "Import Recipe";
            _filesStatus = new ObservableCollection<FileStatusViewModel>();
            IsBusy = false;
            _recipeStatus = new FileStatusViewModel();
        }

        private string _recipeFilePath;
        public string RecipeFilePath
        {
            get { return _recipeFilePath; }
            set
            {
                _recipeFilePath = value;
                OnPropertyChanged(nameof(RecipeFilePath));
                ImportCommand.NotifyCanExecuteChanged();
                OpenExternalFilesCommand.NotifyCanExecuteChanged();
                if (!String.IsNullOrEmpty(_recipeFilePath))
                    LoadRecipe();
            }
        }

        private string _externalFilesFolder;
        public string ExternalFileFolder
        {
            get { return _externalFilesFolder; }
            set
            {
                _externalFilesFolder = value;
                OnPropertyChanged(nameof(ExternalFileFolder));
                ImportCommand.NotifyCanExecuteChanged();
                OpenExternalFilesCommand.NotifyCanExecuteChanged();
                if (!String.IsNullOrEmpty(_externalFilesFolder))
                    UpdateFileState();
            }
        }

        private void OpenRecipe()
        {
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = "Recipe File(*.adcrcp) | *.adcrcp";

            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                RecipeFilePath = openFileDlg.FileName;
            }
        }

        private void OpenExternalFilesFolder()
        {
            ExternalFileFolder = SelectFolderDialog.ShowDialog(ExternalFileFolder);
        }

        /// <summary>
        /// Import de la recette et des fichiers externes
        /// </summary>
        private void Import()
        {
            _recipe.InputDir = string.Empty;
            _recipe.OutputDir = string.Empty;

            bool startImport = true;

            if (_filesStatus.Any(x => x.State == FileState.Missing))
                startImport = Services.Services.Instance.PopUpService.ShowConfirme("Confirmation", "Some externals recipe files are missing. Do you want to import the recipe without these missing files ?");

            if (startImport)
            {
                IsBusy = true;
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Dto.Recipe recipeDto = new Dto.Recipe();
                        recipeDto.Name = _recipeName;
                        recipeDto.RecipeFiles = FileService.UpdateRecipeFile(_filesStatus.ToList(), _recipeName, ExternalFileFolder);
                        recipeDto.CreatorUserId = Services.Services.Instance.AuthentificationService.CurrentUser.Id;
                        recipeDto.XmlContent = File.ReadAllText(RecipeFilePath);
                        recipeDto.Comment = _comment;
                        // TO DO Include enumarable actortype ? or resutl type in recipe...
                        //recipeDto.DataLoaderTypesList = _recipe.DataLoaderActorTypes;

                        // Mise à jour de la base de données

#warning ** USP ** import to do
                        //_recipeService.ImportRecipe(recipeDto);

                        System.Windows.Application.Current.Dispatcher.Invoke((() =>
                        {
                            FilesStatus.Clear();
                            RecipeLoaded = false;
                            IsBusy = false;
                            RecipeFilePath = string.Empty;
                            ExternalFileFolder = string.Empty;
                            MessageBox.Show(string.Format("Import successful for recipe {0} ", _recipeName), "Import successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            _recipe = null;
                            _recipeName = null;
                        }));
                    }
                    catch (Exception ex)
                    {
                        string msg = "Failed to import \"" + _recipeFilePath + "\"";
                        Services.Services.Instance.LogService.LogError(msg, ex);
                        System.Windows.Application.Current.Dispatcher.Invoke((() =>
                        {
                            AdcTools.ExceptionMessageBox.Show(msg, ex);
                        }));
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                });
            }
        }

        /// <summary>
        /// Charge la recette courante depuis la base de données
        /// </summary>
        private void LoadRecipe()
        {
            RecipeLoaded = false;
            IsBusy = true;
            _recipe = new ADCEngine.Recipe();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(_recipeFilePath);
                    _recipeName = Path.GetFileName(_recipeFilePath);
                    _recipe.Load(xmldoc);

                    var recipeProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
                    var recipes = recipeProxy.GetADCRecipes(_recipeName, false, true);

                    // attention ici bug probable en cas de recette nommer identique mais dans un product/step différent
                    int version = 0;
                    if(recipes?.Count > 1)
                        version = recipes[0].Version;

                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
                    {

                        _recipe.SetInputDir(_recipeFilePath);
                        ExternalFileFolder = _recipe.InputDir;
                        _recipeStatus.FileName = _recipeName;
                        _recipeStatus.OldVersion = version;
                        _recipeStatus.State = _recipeStatus.OldVersion == 0 ? FileState.New : FileState.ToUpdate;
                        RecipeLoaded = true;
                        UpdateFileState();
                        OnPropertyChanged(nameof(DataLoaderTypes));
                    }));
                }
                catch (Exception ex)
                {
                    string msg = "Failed to open \"" + _recipeFilePath + "\"";
                    Services.Services.Instance.LogService.LogError(msg, ex);
                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
                    {
                        AdcTools.ExceptionMessageBox.Show(msg, ex);
                    }));
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        /// <summary>
        /// Mise à jour de l'etat de recette et des fichiers externes avant un import
        /// </summary>
        private void UpdateFileState()
        {
            //Dto.Recipe dtoRecipe = _recipeService.GetLastRecipe(_recipeName, true);
            var recipeProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
            var dtoRecipe = recipeProxy.GetADCRecipes(_recipeName, true).OrderByDescending(x => x.Version).FirstOrDefault();

            _filesStatus.Clear();

            if (_recipe == null || _externalFilesFolder == null)
                return;

            List<string> localFiles = new List<string>();
            if (Directory.Exists(_externalFilesFolder))
                localFiles = Directory.GetFiles(_externalFilesFolder).Select(x => Path.GetFileName(x)).ToList();

            foreach (ADCEngine.ExternalRecipeFile recipefile in _recipe.ExternalRecipeFileList)
            {
                FileStatusViewModel fileStatusViewModel = new FileStatusViewModel() { FileName = recipefile };
                Dto.RecipeFile dtoFile = null;
                if (dtoRecipe != null)
                {
                    dtoFile = dtoRecipe.RecipeFiles.SingleOrDefault(x => x.FileName == recipefile.FileName);
                }

                if (dtoFile != null)
                {
                    fileStatusViewModel.OldVersion = dtoFile.Version ?? 0;
                    fileStatusViewModel.State = FileState.ToUpdate;
                    fileStatusViewModel.MD5 = dtoFile.MD5;
                    fileStatusViewModel.UserId = dtoFile.CreatorUserId;
                }
                else
                {
                    fileStatusViewModel.State = FileState.New;
                }

                if (!localFiles.Contains(recipefile))
                    fileStatusViewModel.State = FileState.Missing;

                _filesStatus.Add(fileStatusViewModel);
            }
        }

        public void Refresh()
        {
            // Nothing
        }


        public bool MustBeSave => false;

        #region Commands

        private AutoRelayCommand _openRecipeCommand = null;
        public AutoRelayCommand OpenRecipeCommand
        {
            get
            {
                return _openRecipeCommand ?? (_openRecipeCommand = new AutoRelayCommand(
              () =>
              {
                  OpenRecipe();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _openExternalFilesCommand = null;
        public AutoRelayCommand OpenExternalFilesCommand
        {
            get
            {
                return _openExternalFilesCommand ?? (_openExternalFilesCommand = new AutoRelayCommand(
              () =>
              {
                  OpenExternalFilesFolder();
              },
              () => { return !string.IsNullOrEmpty(_recipeFilePath); }));
            }
        }

        private AutoRelayCommand _importCommand = null;
        public AutoRelayCommand ImportCommand
        {
            get
            {
                return _importCommand ?? (_importCommand = new AutoRelayCommand(
              () =>
              {
                  Import();
              },
              () =>
              {
                  return !string.IsNullOrEmpty(_recipeFilePath) && !string.IsNullOrEmpty(_externalFilesFolder);
              }));
            }
        }
        #endregion
    }
}
