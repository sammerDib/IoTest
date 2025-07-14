using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ADCEngine.View;

using AdcTools;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    // File parameter
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public class FileParameter : ParameterTemplate<ExternalRecipeFile>
    {
        private const string AddFile = "<Add file..>";
        private const string NoFile = "<No file selected>";
        private string _filter;
        private ModuleBase _module;

        public FileParameter(ModuleBase module, string label, string filter = null) :
            base(module, label)
        {
            _module = module;
            _filter = filter;
            _files = new ObservableCollection<string>();
            _value = new ExternalRecipeFile(string.Empty);
            _files.Add(NoFile);
            _files.Add(AddFile);
            Init();
            _selectedFile = NoFile;
        }

        public void Init()
        {
            SynchroniseFiles();
        }

        private void SynchroniseFiles()
        {
            // Récupération des nouveaux fichiers
            List<string> newFiles = new List<string>();
            if (Directory.Exists(_module.Recipe.InputDir))
            {
                List<string> extensions = new List<string>();

                // On récupére les extensions définis dans le filtre
                if (_filter != null)
                    extensions = _filter.Split('|').Where((x, i) => i % 2 != 0).ToList();
                else
                    extensions.Add("*.*");

                // On récupére les fichiers pour chaque d'extenion.
                newFiles.AddRange(extensions.SelectMany(x => Directory.GetFiles(_module.Recipe.InputDir, x).Select(f => Path.GetFileName(f))));
            }

            // Suppression des fichiers inutiles
            for (int i = 2; i < _files.Count; i++)
            {
                if (!newFiles.Contains(_files[i]))
                    _files.RemoveAt(i);
            }

            // Ajout des fichiers manquants
            foreach (string file in newFiles)
            {
                if (!_files.Contains(file))
                    _files.Add(file);
            }
        }


        /// <summary>
        /// Full path du fichier séléctionné
        /// </summary>
        public string FullFilePath
        {
            get
            {
                if (string.IsNullOrEmpty(Value))
                    return null;
                else
                    return Path.Combine(_module.Recipe.InputDir, Value);
            }
        }


        /// <summary>
        ///  List des fichiers définis dans le repertoire de recette + <Add file..> et <No file selected>
        /// </summary>
        private ObservableCollection<string> _files;
        public ObservableCollection<string> Files
        {
            get => _files; set { if (_files != value) { _files = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Fichier séléctionné
        /// </summary>
        private string _selectedFile;
        public string SelectedFile
        {
            get => _selectedFile;
            set
            {
                if (_selectedFile != value)
                {
                    _selectedFile = value;
                    if (_selectedFile == NoFile)
                        Value = string.Empty;
                    else if (_selectedFile != AddFile)
                        Value = _selectedFile;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Ouverture d'un nouveau fichier
        /// </summary>
        private void OpenFile()
        {
            if (string.IsNullOrEmpty(_module.Recipe.InputDir))
            {
                AttentionMessageBox.Show("The recipe must be saved before adding a file");
                SelectedFile = NoFile;
                return;
            }

            System.Windows.Forms.OpenFileDialog openFileDlg = new System.Windows.Forms.OpenFileDialog();
            openFileDlg.Filter = _filter;
            if (openFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePathSource = openFileDlg.FileName;
                string filePathDest = Path.Combine(_module.Recipe.InputDir, Path.GetFileName(filePathSource));
                bool copyFile = false;
                try
                {
                    if (!Directory.Exists(_module.Recipe.InputDir))
                    {
                        Directory.CreateDirectory(_module.Recipe.InputDir);
                    }
                    if (File.Exists(filePathDest))
                    {
                        if (MessageBox.Show("File already exist." + Environment.NewLine + "Do you want to replace it ?", "File already exist", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            copyFile = true;
                        else
                            SelectedFile = NoFile;
                    }
                    else
                    {
                        copyFile = true;
                    }

                    if (copyFile)
                    {
                        if (filePathDest != filePathSource)
                            File.Copy(filePathSource, filePathDest, true);
                        string newFileName = Path.GetFileName(filePathDest);
                        if (!Files.Contains(newFileName))
                            Files.Insert(Files.Count - 1, newFileName);
                        SelectedFile = newFileName;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionMessageBox.Show("File copy error", ex);
                    SelectedFile = NoFile;
                }
            }
            else
            {
                SelectedFile = NoFile;
            }
        }

        protected override bool TryParse(string str)
        {
            if (str == NoFile || str == AddFile)
                str = string.Empty;

            Value = str;
            _selectedFile = _files.SingleOrDefault(x => x == str);

            if (_selectedFile == null)
                _selectedFile = NoFile;

            OnPropertyChanged(nameof(SelectedFile));
            ReportChange();

            return true;
        }

        /// <summary>
        /// Valeur sauvegardé dans la recette
        /// </summary>
        public String String
        {
            get { return Value; }
            set
            {
                Value = value;
                OnPropertyChanged(nameof(String));
                ReportChange();
            }
        }

        public static implicit operator String(FileParameter p)
        {
            return p;
        }


        private AutoRelayCommand _selectionChangedCommand;
        public AutoRelayCommand SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ?? (_selectionChangedCommand = new AutoRelayCommand(
              () =>
              {
                  if (_selectedFile == AddFile)
                  {
                      OpenFile();
                  }
              },
              () => { return true; }));
            }
        }

        private FileParameterExpertView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterUI = new FileParameterExpertView();
                    _parameterUI.DataContext = this;
                }
                return _parameterUI;
            }
        }

        private FileParameterSimplifiedView _parameterSimplifiedUI;
        public override UserControl ParameterSimplifiedUI
        {
            get
            {
                if (_parameterSimplifiedUI == null)
                {
                    _parameterSimplifiedUI = new FileParameterSimplifiedView();
                    _parameterSimplifiedUI.DataContext = this;
                }
                return _parameterSimplifiedUI;
            }
        }
    }
}
