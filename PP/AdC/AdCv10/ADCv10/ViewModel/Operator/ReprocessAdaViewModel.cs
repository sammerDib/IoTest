using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

using AdcTools;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADC.ViewModel.Operator
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ReprocessAdaViewModel : ClosableViewModel, IDisposable
    {
        private ObservableCollection<ReprocessFile> _files = new ObservableCollection<ReprocessFile>();
        private ICollectionView _filesView;
        private FileSystemWatcher _adcFileSystemWatcher = new FileSystemWatcher();
        private string _adaFolder;

        public ReprocessAdaViewModel()
        {
            _adaFolder = ConfigurationManager.AppSettings["AdaFolder"];
            if (!Directory.Exists(_adaFolder))
            {
                AdcTools.AttentionMessageBox.Show("Invalid ada folder path: " + _adaFolder);
                CloseSignal = true;
                return;
            }

            Init();
            _adcFileSystemWatcher.Filter = "*.adc";
            _adcFileSystemWatcher.NotifyFilter = ((System.IO.NotifyFilters)((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.LastWrite)));
            _adcFileSystemWatcher.Created += AdcFileSystemWatcher_Created;
            _adcFileSystemWatcher.Renamed += AdcFileSystemWatcher_Renamed;
            _adcFileSystemWatcher.Path = _adaFolder;
            _adcFileSystemWatcher.EnableRaisingEvents = true;
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

        private void AdcFileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            AdcFileSystemWatcher_Created(sender, e);
        }

        private void AdcFileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                string path = e.FullPath;
                if (path.EndsWith(".adc") && !_files.Any(x => x.FilePath == path))
                    _files.Add(new ReprocessFile(path));
            });
        }

        private void Init()
        {
            _files.Clear();
            _files.AddRange(Directory.EnumerateFiles(_adaFolder).Where(x => x.ToLower().EndsWith("adc")).Select(x => new ReprocessFile(x)));
            Files = CollectionViewSource.GetDefaultView(_files);
            _filesView.Filter = FileFilter;
        }

        private bool FileFilter(object obj)
        {
            ReprocessFile file = (obj as ReprocessFile);
            return (Filter == null || file.FileName.ToLower().Contains(Filter.ToLower()));
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

        private void SelectAll()
        {
            _files.ToList().ForEach(x => x.IsSelected = true);
        }


        private void Reprocess()
        {
            int maxDisplayFile = 10;
            _adcFileSystemWatcher.EnableRaisingEvents = false;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Reprocess started for:");
            int nbFiles = 0;
            foreach (ReprocessFile file in _files.Where(x => x.IsSelected))
            {
                PathString adc = file.FilePath;
                string ada = adc.ChangeExtension(".ada");
                File.Move(adc, ada);
                if (nbFiles == maxDisplayFile)
                {
                    stringBuilder.AppendLine("....");
                }
                else if (nbFiles < maxDisplayFile)
                {
                    stringBuilder.AppendLine(string.Format("-{0}", adc.Filename));
                }

                nbFiles++;
            }
            System.Windows.Forms.MessageBox.Show(stringBuilder.ToString(), "Reprocess started", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            CloseSignal = true;
        }


        public ReprocessFile SelectedItem
        {
            set
            {
                ReprocessCommand.NotifyCanExecuteChanged();
            }
        }

        public void Dispose()
        {
            if (_adcFileSystemWatcher != null)
            {
                _adcFileSystemWatcher.EnableRaisingEvents = false;
                _adcFileSystemWatcher.Created -= AdcFileSystemWatcher_Created;
                _adcFileSystemWatcher.Renamed -= AdcFileSystemWatcher_Renamed;
                _adcFileSystemWatcher.Dispose();
                _adcFileSystemWatcher = null;
            }
        }

        #region commands

        private AutoRelayCommand _selectAllCommand;
        public AutoRelayCommand SelecteAllCommand
        {
            get
            {
                return _selectAllCommand ?? (_selectAllCommand = new AutoRelayCommand(
              () =>
              {
                  SelectAll();
              },
              () => { return true; }));
            }
        }


        private AutoRelayCommand _reprocessCommand;
        public AutoRelayCommand ReprocessCommand
        {
            get
            {
                return _reprocessCommand ?? (_reprocessCommand = new AutoRelayCommand(
              () =>
              {
                  Reprocess();
              },
              () => { return _files.Any(x => x.IsSelected); }));
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

    public class ReprocessFile : ObservableRecipient
    {
        public ReprocessFile(string path)
        {
            PathString pathString = path;
            FileName = pathString.Filename;
            FilePath = pathString;
        }

        private string _fileName;
        public string FileName
        {
            get => _fileName; set { if (_fileName != value) { _fileName = value; OnPropertyChanged(); } }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected; set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        private string _filePath;
        public string FilePath
        {
            get => _filePath; set { if (_filePath != value) { _filePath = value; OnPropertyChanged(); } }
        }
    }
}
