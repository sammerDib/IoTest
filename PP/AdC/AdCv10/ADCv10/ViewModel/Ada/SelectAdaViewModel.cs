using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

using AcquisitionAdcExchange;

using ADC.Model;

using AdcTools;

using MergeContext;

using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADC.ViewModel.Ada
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class SelectAdaViewModel : ClosableViewModel
    {
        private ObservableCollection<string> _files = new ObservableCollection<string>();
        private ICollectionView _filesView;
        private bool _bRun = true;

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private RecipeViewModel _recipeViewModel;

        public SelectAdaViewModel(RecipeViewModel recipeViewModel, bool bRun)
        {
            // Chargement depuis les paramétres utilisateur
            _files.AddRange(Properties.Settings.Default.FavoritesAdas.Cast<string>());
            _recipeViewModel = recipeViewModel;
            Files = CollectionViewSource.GetDefaultView(_files);
            _filesView.Filter = FileFilter;
            _bRun = bRun;
        }

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
                    _filesView.Refresh();
                }
            }
        }

        /// <summary>
        /// Filtre sur le chemin du fichier
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool FileFilter(object obj)
        {
            string file = (obj as string);
            return (Filter == null || file.ToLower().Contains(Filter.ToLower()));
        }

        public string SelectedFile
        {
            get => _recipeViewModel.SelectedAdaFile; set { if (_recipeViewModel.SelectedAdaFile != value) { _recipeViewModel.SelectedAdaFile = value; OnPropertyChanged(); } }
        }

        public ICollectionView Files
        {
            get { return _filesView; }
            set
            {
                _filesView = value;
                OnPropertyChanged();
            }
        }

        private void EditAda()
        {
            try
            {
                if (CheckSelectedFileExist())
                    Process.Start(SelectedFile);
            }
            catch
            {
                AdcTools.AttentionMessageBox.Show("Can not open the program to read the file");
            }

        }

        private bool CheckSelectedFileExist()
        {
            if (File.Exists(SelectedFile))
                return true;
            else
            {
                AdcTools.AttentionMessageBox.Show("The current ada file does not exist");
                return false;
            }
        }

        private void NewAda()
        {
            NewAdaViewModel newAdaVM = new NewAdaViewModel(_recipeViewModel.CurrentFileName);
            Services.Services.Instance.PopUpService.ShowDialogWindow("New Ada", newAdaVM, 500, 400, true);

            // Ajout du nouveau ada
            if (!string.IsNullOrEmpty(newAdaVM.AdaPath) && !_files.Contains(newAdaVM.AdaPath))
            {
                _files.Add(newAdaVM.AdaPath);
                SelectedFile = newAdaVM.AdaPath;
            }

            SaveChangeInUserSettings();
            Filter = null;
        }

        private void DeleteAda()
        {
            _files.Remove(SelectedFile);
            SaveChangeInUserSettings();
            SelectedFile = _files.FirstOrDefault();
        }

        private void ImportAdas()
        {
            string adaFolder = ConfigurationManager.AppSettings["AdaFolder"];
            System.Windows.Forms.OpenFileDialog openFileDlg = new System.Windows.Forms.OpenFileDialog();
            openFileDlg.Filter = "ADA Files (*.ada *.adc)|*.ada;*.adc|All files (*.*)|*.*";
            openFileDlg.InitialDirectory = adaFolder;
            openFileDlg.Multiselect = true;

            if (openFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Ajout des fichiers qui ne sont pas déja présents
                var newFiles = openFileDlg.FileNames.Where(x => !_files.Contains(x));
                if (newFiles.Any())
                {
                    _files.AddRange(newFiles);
                    SelectedFile = _files.First();
                }
            }

            SaveChangeInUserSettings();
            Filter = null;
        }

        private void Run()
        {
            if (CheckSelectedFileExist())
            {
                // Merge
                IsBusy = true;
                bool mergeIsOk = false;
                var task = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        AdaLoader adaloader = new AdaLoader(SelectedFile);

                        if (!adaloader.ContainsRemoteProductionInfo())
                        {
                            var enterProductionInfo = new EnterProductionInfoViewModel();
                            Application.Current.Dispatcher.Invoke((Action)delegate
                            {
                                Services.Services.Instance.PopUpService.ShowDialogWindow("Enter production info", enterProductionInfo, 700, 250, true, 1024);
                            });

                            if (!enterProductionInfo.IsValidated)
                            {
                                IsBusy = false;
                                mergeIsOk = false;
                                return;
                            }
                            // If the ada does not contain all the wafer info we ask them to the user

                            adaloader.CompleteAda(_recipeViewModel.Recipe.Name, _recipeViewModel.Recipe.Key, enterProductionInfo.ProductionInfo);
                        }
                        adaloader.IsOfflineMode = true;
                        adaloader.LoadAda();
                        adaloader.ApplyCurrentRecipe(_recipeViewModel.Recipe);
                        MergeContext.MergeContext mergectx = new MergeContext.MergeContext(_recipeViewModel.Recipe, adaloader.RecipeData);
                        mergectx.Merge();
                        mergectx.SetAcquitionImages(adaloader.AcqImageList);

                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            ServiceRecipe.Instance().MustBeSaved = true;
                            _recipeViewModel.ReplayCommand.NotifyCanExecuteChanged();
                            IsBusy = false;
                            mergeIsOk = true;
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            IsBusy = false;
                            ExceptionMessageBox.Show("Cannot merge", ex);
                        });
                    }
                    if (mergeIsOk)
                    {

                        try
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                          {
                              CloseSignal = true;
                              if (_bRun)
                              {
                                  _recipeViewModel.RunGraph();
                              }
                          });
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                IsBusy = false;
                                ExceptionMessageBox.Show("Run error", ex);
                            });
                        }
                    }
                });
            }
        }

        private bool Merge()
        {
            bool result = false;

            return result;
        }

        private void SaveChangeInUserSettings()
        {
            // Sauvegarde dans les paramétres utilisateur
            Properties.Settings.Default.FavoritesAdas.Clear();
            Properties.Settings.Default.FavoritesAdas.AddRange(_files.ToArray());
            Properties.Settings.Default.Save();
        }

        #region commands

        private AutoRelayCommand _editCommand;
        public AutoRelayCommand EditCommand
        {
            get
            {
                return _editCommand ?? (_editCommand = new AutoRelayCommand(
              () =>
              {
                  EditAda();
              },
              () => { return SelectedFile != null; }));
            }
        }


        private AutoRelayCommand _newCommand;
        public AutoRelayCommand NewCommand
        {
            get
            {
                return _newCommand ?? (_newCommand = new AutoRelayCommand(
              () =>
              {
                  NewAda();
              },
              () => { return true; }));
            }
        }


        private AutoRelayCommand _deleteCommand;
        public AutoRelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new AutoRelayCommand(
              () =>
              {
                  DeleteAda();
              },
              () => { return SelectedFile != null; }));
            }
        }

        private AutoRelayCommand _importCommand;
        public AutoRelayCommand ImportCommand
        {
            get
            {
                return _importCommand ?? (_importCommand = new AutoRelayCommand(
              () =>
              {
                  ImportAdas();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand __runCommand;
        public AutoRelayCommand RunCommand
        {
            get
            {
                return __runCommand ?? (__runCommand = new AutoRelayCommand(
              () =>
              {
                  Run();
              },
              () => { return SelectedFile != null; }));
            }
        }

        private AutoRelayCommand _cancelCommand;
        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () =>
              {
                  CloseSignal = true;
              },
              () => { return true; }));
            }
        }


        #endregion

    }
}
