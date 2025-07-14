using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using AdcTools.Widgets;

using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Utils.ViewModel;

using Dto = UnitySC.DataAccess.Dto;


namespace ADCConfiguration.ViewModel.Recipe
{
    public class ExportRecipeOptionDetailViewModel : ValidationViewModelBase
    {
        private List<Dto.Recipe> _recipes;

        private bool _exportToolsConfiguration = false;
        public bool ExportToolsConfiguration
        {
            get => _exportToolsConfiguration; set { if (_exportToolsConfiguration != value) { _exportToolsConfiguration = value; OnPropertyChanged(); } }
        }

        private bool _exportExternalFiles = true;
        public bool ExportExternalFiles
        {
            get => _exportExternalFiles; set { if (_exportExternalFiles != value) { _exportExternalFiles = value; OnPropertyChanged(); } }
        }

        private string _outputFolder;
        public string OutputFolder
        {
            get => _outputFolder;
            set
            {
                if (_outputFolder != value)
                {
                    _outputFolder = value; OnPropertyChanged();
                    ExportCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public IEnumerable<int> Versions
        {
            get
            {
                return _recipes != null ? _recipes.Select(x => x.Version) : null;
            }
        }

        private int _selectedVersion;
        public int SelectedVersion
        {
            get => _selectedVersion;
            set
            {
                if (_selectedVersion != value)
                {
                    _selectedVersion = value;
                    OnPropertyChanged();
                    SelectedRecipe = _recipes.SingleOrDefault(x => x.Version == _selectedVersion);
                }
            }
        }


        private Dto.Recipe _selectedRecipe;
        public Dto.Recipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                if (_selectedRecipe != value)
                {
                    _selectedRecipe = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ExternalFiles));
                    OnPropertyChanged(nameof(Created));
                    OnPropertyChanged(nameof(Comment));
                    OnPropertyChanged(nameof(DataLoaderTypes));
                    OnPropertyChanged(nameof(User));
                }
            }
        }

        public string DataLoaderTypes
        {
            get => _selectedRecipe != null ? /*string.Join(", ", _selectedRecipe.DataLoaderTypesList.Select(x => x.ToString())) */ "TODO DATALOADER TYPES": string.Empty;
        }

        public IEnumerable<Dto.RecipeFile> ExternalFiles
        {
            get
            {
                if (_selectedRecipe != null)
                    return _selectedRecipe.RecipeFiles;
                else

                    return null;
            }
        }

        public DateTime? Created
        {
            get
            {
                if (_selectedRecipe != null)
                    return _selectedRecipe.Created;
                else

                    return null;
            }
        }

        public string Comment
        {
            get
            {
                if (_selectedRecipe != null)
                    return _selectedRecipe.Comment;
                else

                    return null;
            }
        }

        public string User
        {
            get
            {
                if (_selectedRecipe != null && _selectedRecipe.User != null)
                    return _selectedRecipe.User.ToString();
                else

                    return null;
            }
        }


        private string _name;
        public string Name
        {
            get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        public ExportRecipeOptionDetailViewModel(string recipeName)
        {
            Name = recipeName;
            OutputFolder = ConfigurationManager.AppSettings["OutputFolder"];
            Init();
        }

        private void Init()
        {
            IsBusy = true;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var recipeProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
                    _recipes = recipeProxy.GetADCRecipes(_name, true);
                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
                    {
                        OnPropertyChanged(nameof(Versions));
                        if (Versions != null)
                            SelectedVersion = Versions.Last();
                        OnPropertyChanged(nameof(SelectedRecipe));
                    }));
                }
                catch (Exception ex)
                {
                    Services.Services.Instance.LogService.LogError("Refresh recipe detail", ex);
                    System.Windows.Application.Current.Dispatcher.Invoke((() => { AdcTools.ExceptionMessageBox.Show("Refresh error: ", ex); }));
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        public override string ToString()
        {
            return _name;
        }

        private void Export()
        {
            IsBusy = true;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Dto.Recipe dtoRecipe = _recipes.SingleOrDefault(x => x.Version == _selectedVersion);
                    // create directory
                    if (!Directory.Exists(_outputFolder))
                        Directory.CreateDirectory(_outputFolder);

                    const string OutputRecipeFolderFormat = "{0}v{1}"; // 0 => RecipeName , 1=> Recipe version
                    string outputRecipeFolder = Path.Combine(_outputFolder, string.Format(OutputRecipeFolderFormat, Path.GetFileNameWithoutExtension(dtoRecipe.Name), dtoRecipe.Version));

                    if (Directory.Exists(outputRecipeFolder))
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke((() =>
                        {
                            if (Services.Services.Instance.PopUpService.ShowConfirme("Confirmation", string.Format("The recipe folder {0} already exist. Do you want to replace it ?", Path.GetFileName(outputRecipeFolder))))
                                Directory.Delete(outputRecipeFolder, true);
                            else
                                return;
                        }));
                    }
#warning ** USP ** Export adc recipe to do 
                    //RecipeService.ExportRecipeAndExternalFiles(dtoRecipe, outputRecipeFolder, _exportExternalFiles, ConfigurationManager.AppSettings["DatabaseConfig.AdditionnalRecipeFiles.ServerDirectory"]);

                 // // TODO Export
                 //   if (ExportToolsConfiguration)
                 //       File.WriteAllText(Path.Combine(outputRecipeFolder, "ADCDatabase.adcdb"), _importExportService.Export());

                    System.Windows.Application.Current.Dispatcher.Invoke((() =>
                    {
                        MessageBox.Show(string.Format("Export successful for recipe {0} ", _name), "Export successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Process.Start(outputRecipeFolder);
                    }));

                }
                catch (Exception ex)
                {
                    Services.Services.Instance.LogService.LogError("Export recipe", ex);
                    System.Windows.Application.Current.Dispatcher.Invoke((() => { AdcTools.ExceptionMessageBox.Show("Export recipe error: ", ex); }));
                }
                finally
                {
                    IsBusy = false;
                }
            });
        }

        private void OpenOutputFolder()
        {
            OutputFolder = SelectFolderDialog.ShowDialog(OutputFolder);
        }

        #region Commands

        private AutoRelayCommand __exportCommand;
        public AutoRelayCommand ExportCommand
        {
            get
            {
                return __exportCommand ?? (__exportCommand = new AutoRelayCommand(
              () =>
              {
                  Export();
              },
              () => { return !string.IsNullOrEmpty(_outputFolder); }));
            }
        }


        private AutoRelayCommand _openOutputFolderCommand;
        public AutoRelayCommand OpenOutputFolderCommand
        {
            get
            {
                return _openOutputFolderCommand ?? (_openOutputFolderCommand = new AutoRelayCommand(
              () =>
              {
                  OpenOutputFolder();
              },
              () => { return true; }));
            }
        }

        #endregion
    }
}
