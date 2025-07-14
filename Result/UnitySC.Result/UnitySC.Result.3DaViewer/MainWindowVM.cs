using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Microsoft.Win32;

using UnitySC.Shared.Data.FormatFile;
using UnitySC.Shared.Logger;
using UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.ExceptionDialogs;

namespace UnitySC.Result._3DaViewer
{
    public class MainWindowVM : ObservableRecipient
    {
        #region Fields

        private int _fileCount;
        private string _currentDirectory;
        private int _currentFileIndex = -1;

        #endregion

        #region Properties

        private string _currentFileName = "Open file";

        public string CurrentFileName
        {
            get { return _currentFileName; }
            set { SetProperty(ref _currentFileName, value); }
        }

        private MatrixViewerViewModel _matrixViewer;

        public MatrixViewerViewModel MatrixViewer
        {
            get => _matrixViewer;
            private set => SetProperty(ref _matrixViewer, value);
        }

        private string _busyReason;

        public string BusyReason
        {
            get { return _busyReason; }
            set { SetProperty(ref _busyReason, value); }
        }

        private bool _isBusy = true;

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        #endregion

        public MainWindowVM()
        {           
            ClassLocator.Default.GetInstance<ExceptionManager>().Init();

            //Set Deployment Key for Arction components -- à offusquer
            string deploymentKey = "lgCAAMDQWCaC7NkBJABVcGRhdGVhYmxlVGlsbD0yMDI1LTEwLTE1I1JldmlzaW9uPTAI/z8AAAAA/AEy9k+WSPYWl8F5j8DR3/xHV8rmwsD6bgDd0+Kv9Aki7UvzB0KJxrUPqzZmcZ1hEhm6bFr+fezEYuukPWEkI7pybF86LTroOuA934Gci/KuDUrhHiaqtxFeaR30Gcgr25NjTyEpauRATjQ4BFk32TnkLwotmJoCv+HYAJkkvd85VCzS0o5fd4w99JHK3/XtyJSYL8/OCCrqumTQZm5A8s7q95M8AfxmeLTEUjPFJp/k+m0oTPHF4er+PTE/m1R/r1+yL6ZeiCzkuFB5m4vLE1vxa7ZEp0aRQ01Xw+0LPPBusgBj4089eXfVWH3DsnFfDmPrFn63MByaFqpzT/hK4J0EiXGqHRaGz8CCiRVxAO3mAT7DirAypxLrrF+142Z3f3iQnd88mRsFiTN2rqfbZDFmPPaK2j4LwDwqKiaVCOz6ISQpG8W7UOMSZjX1KnMiS+FdQRYJJPuuE0WGRMutSyrNHGawAsMY6J4hOh4hDsJsRgN3onrFG+pCHwFG/fUD154=";
            // Setting Deployment Key for fully bindable chart
            LightningChartLib.WPF.ChartingMVVM.LightningChart.SetDeploymentKey(deploymentKey);
            LightningChartLib.WPF.Charting.LightningChart.SetDeploymentKey(deploymentKey);
        }

        #region Private Methods

        private void Initialize(MatrixDefinition matrix)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                bool lastExcludeZeroFromStats = false; 

                if (MatrixViewer != null)
                {
                    lastExcludeZeroFromStats = MatrixViewer.ExcludePureZeroFromStats;
                    MatrixViewer.PropertyChanged -= MatrixViewer_PropertyChanged;
                    MatrixViewer.Dispose();
                }

                MatrixViewer = new MatrixViewerViewModel(matrix, lastExcludeZeroFromStats);
                MatrixViewer.PropertyChanged += MatrixViewer_PropertyChanged;

                //Forces the propertyChanged of IsBusy because it may be that the value is changed while the subscription was not yet made.
                MatrixViewer_PropertyChanged(this, new PropertyChangedEventArgs(nameof(MatrixViewer.IsBusy)));
            });
        }

        private void MatrixViewer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MatrixViewer.IsBusy))
            {
                if (MatrixViewer.IsBusy)
                {
                    BusyReason = "Calculation of the new display area...";
                }

                IsBusy = MatrixViewer.IsBusy;
            }
        }

        private void Clear()
        {
            Initialize(new MatrixDefinition
            {
                Extension = string.Empty,
                FileName = string.Empty,
                Height = 0,
                Resolution = 1,
                Unit = string.Empty,
                Values = Array.Empty<float>(),
                Width = 0
            });
            IsBusy = false;
        }

        #endregion
        
        #region Public Methods

        public void LoadFile(string[] argsFilePath)
        {
            if (argsFilePath.Length > 0)
            {
                string fileParams = string.Join(" ", argsFilePath);
                string path = Path.GetFullPath(fileParams);

                LoadFile(path);
            }
            else
            {
                IsBusy = false;
            }
        }

        private List<string> EnumerateReadableFilesInDirectory()
        {
            var fileExtFilters = new string[] { "*.3da", "*.bcrf" };
            var files = new List<string>();
            foreach (var extfilter in fileExtFilters)
            {
                files.AddRange(Directory.EnumerateFiles(_currentDirectory, extfilter).ToList());
            }
            files.Sort();
            return files;
        }


        private void LoadFile(string filePath)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    _currentDirectory = Path.GetDirectoryName(filePath);

                    var files = EnumerateReadableFilesInDirectory();

                    _fileCount = files.Count;
                    _currentFileIndex = files.IndexOf(filePath);

                    CurrentFileName = Path.GetFileName(filePath);

                    // Raise command can execute changed
                    CommandManager.InvalidateRequerySuggested();

                    BusyReason = "File processing...";
                    IsBusy = true;

                    var ext = Path.GetExtension(filePath).ToLowerInvariant();
                    if (ext == ".3da")
                    {
                        using (var format3daFile = new MatrixFloatFile(filePath, -1))
                        {
                            var matrix = MatrixDefinition.FromMatrixFloatFile(format3daFile);
                            IsBusy = false;
                            Initialize(matrix);
                        }
                    }
                    else if (ext == ".bcrf")
                    {
                        using (var format3daFile = new MatrixFloatFile())
                        {
                            format3daFile.FromBCRF_File(filePath);
                            var matrix = MatrixDefinition.FromMatrixFloatFile(format3daFile);
                            IsBusy = false;
                            Initialize(matrix);
                        }
                    }
                    else
                    {
                        throw new Exception("This file extension is not handle"); 
                    }

                }
                catch (Exception e)
                {
                    Clear();
                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        ClassLocator.Default.GetInstance<ExceptionManager>().SendReport(e, "Error while loading file.");
                    });
                }
            });
        }

        #endregion

        #region Commands

        private ICommand _openFileCommand;

        public ICommand OpenFileCommand => _openFileCommand ?? (_openFileCommand = new AutoRelayCommand(OpenFileCommandExecute));
        
        private void OpenFileCommandExecute()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                LoadFile(openFileDialog.FileName);
            }
        }

        private ICommand _openPreviousFileCommand;

        public ICommand OpenPreviousFileCommand => _openPreviousFileCommand ?? (_openPreviousFileCommand = new AutoRelayCommand(OpenPreviousFileCommandExecute, OpenPreviousFileCommandCanExecute));

        private bool OpenPreviousFileCommandCanExecute()
        {
            return _currentFileIndex > 1 && _fileCount > 1;
        }

        private void OpenPreviousFileCommandExecute()
        {
            var files = EnumerateReadableFilesInDirectory();      
            string file = files.ElementAt(_currentFileIndex - 1);
            LoadFile(file);
        }

        private ICommand _openNextFileCommand;

        public ICommand OpenNextFileCommand => _openNextFileCommand ?? (_openNextFileCommand = new AutoRelayCommand(OpenNextFileCommandExecute, OpenNextFileCommandCanExecute));

        private bool OpenNextFileCommandCanExecute()
        {
            return _currentFileIndex >= 0 && _currentFileIndex < _fileCount - 1;
        }

        private void OpenNextFileCommandExecute()
        {
            var files = EnumerateReadableFilesInDirectory();
            string file = files.ElementAt(_currentFileIndex + 1);
            LoadFile(file);
        }

        #endregion
    }
}
