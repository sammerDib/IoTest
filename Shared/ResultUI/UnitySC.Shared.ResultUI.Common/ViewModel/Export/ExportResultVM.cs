using System;
using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs.FrameworkDialogs.FolderBrowser;

using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Export
{
    public class ExportResultVM : ObservableObject
    {
        private static string s_lastDirBase = @"C:\Temp\";
        
        #region Constructors

        public ExportResultVM()
        {
            UseZipArchive = true;
            ExportResultData = true;
            ExportResultThumbnails = true;
            ExportSnapshot = true;

            string sBasePath = System.Configuration.ConfigurationManager.AppSettings.Get("ExportBasePath");
            if (sBasePath != null)
                s_lastDirBase = sBasePath;
            
            GenerateNewTargetPath();
        }

        #endregion

        #region Properties

        public ObservableCollection<ExportEntry> AdditionalEntries { get; } = new ObservableCollection<ExportEntry>();

        /// <summary>
        /// Destination Zip filePath or Folder Path 
        /// </summary>
        private string _targetPath;

        public string TargetPath
        {
            get => _targetPath;
            set
            {
                if (SetProperty(ref _targetPath, value))
                {
                    SaveExportCommand.NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        ///  Zip filename or Folder Name 
        /// </summary>
        private string _targetName;

        public string TargetName
        {
            get => _targetName;
            set
            {
                if (SetProperty(ref _targetName, value))
                {
                    SaveExportCommand.NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Export results data in a Zip file (@True) or in a Folder otherwise  
        /// </summary>
        private bool _useZipArchive;

        public bool UseZipArchive
        {
            get => _useZipArchive;
            set => SetProperty(ref _useZipArchive, value);
        }

        /// <summary>
        /// Export results data 
        /// </summary>
        private bool _exportResultData;

        public bool ExportResultData
        {
            get => _exportResultData;
            set
            {
                if (SetProperty(ref _exportResultData, value))
                {
                    SaveExportCommand.NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Export results Thumbnails 
        /// </summary>
        private bool _exportResultThumbnails;

        public bool ExportResultThumbnails
        {
            get => _exportResultThumbnails;
            set
            {
                if (SetProperty(ref _exportResultThumbnails, value))
                {
                    SaveExportCommand.NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Export Snapshot 
        /// </summary>
        private bool _exportSnapshot;

        public bool ExportSnapshot
        {
            get => _exportSnapshot;
            set
            {
                if (SetProperty(ref _exportSnapshot, value))
                {
                    SaveExportCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private bool _isExporting;

        public bool IsExporting
        {
            get => _isExporting;
            set
            {
                if (SetProperty(ref _isExporting, value))
                {
                    SaveExportCommand.NotifyCanExecuteChanged();
                    OpenTargetFolderCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private bool _isStayPopup;

        public bool IsStayPopup
        {
            get => _isStayPopup;
            set => SetProperty(ref _isStayPopup, value);
        }
   
        private bool _displayZipArchive = true;
        public bool DisplayZipArchive
        {
            get => _displayZipArchive; set { if (_displayZipArchive != value) { _displayZipArchive = value; OnPropertyChanged("DisplayZipArchive"); } }
        }

        private bool _displayExportResultData = true;
        public bool DisplayExportResultData
        {
            get => _displayExportResultData; set { if (_displayExportResultData != value) { _displayExportResultData = value; OnPropertyChanged("DisplayExportResultData"); } }
        }

        private bool _displayExportResultThumbnails = true;
        public bool DisplayExportResultThumbnails
        {
            get => _displayExportResultThumbnails; set { if (_displayExportResultThumbnails != value) { _displayExportResultThumbnails = value; OnPropertyChanged("DisplayExportResultThumbnails"); } }
        }
        private bool _displayExportSnapshot = true;
        public bool DisplayExportSnapshot
        {
            get => _displayExportSnapshot; set { if (_displayExportSnapshot != value) { _displayExportSnapshot = value; OnPropertyChanged("DisplayExportSnapshot"); } }
        }




        #endregion

        #region Methods

        public void GenerateNewTargetPath(string suffix = null)
        {
            string sBasePath = s_lastDirBase;
            if (!sBasePath.EndsWith(@"\"))
                sBasePath += @"\";
            TargetPath = sBasePath;

            var dtNow = DateTime.Now;
            string sBaseFile = dtNow.ToString("yyyyMMdd_HHmmss", System.Globalization.CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(suffix))
                sBaseFile += suffix;
            TargetName = sBaseFile;
        }


        public string GetTargetFullPath()
        {
            return System.IO.Path.Combine(TargetPath, TargetName);
        }

        #endregion
        
        #region Commands

        public event Action OnSaveExportCommand;

        private AutoRelayCommand _saveExportCommand;

        public AutoRelayCommand SaveExportCommand => _saveExportCommand ?? (_saveExportCommand = new AutoRelayCommand(SaveExportCommandExecute, SaveExportCommandCanExecute));

        private bool SaveExportCommandCanExecute()
        {
            return !string.IsNullOrWhiteSpace(TargetPath) &&
                   (ExportResultData || ExportResultThumbnails || ExportSnapshot || AdditionalEntries.Any(entry => entry.IsChecked)) &&
                   !IsExporting;
        }

        private void SaveExportCommandExecute()
        {
            if (!string.IsNullOrWhiteSpace(TargetPath))
            {
                s_lastDirBase = TargetPath;
            }

            OnSaveExportCommand?.Invoke();
        }
        
        private AutoRelayCommand _openExportFolderCommand;

        public AutoRelayCommand OpenTargetFolderCommand => _openExportFolderCommand ?? (_openExportFolderCommand = new AutoRelayCommand(OpenTargetFolderCommandExecute, OpenTargetFolderCommandCanExecute));

        private bool OpenTargetFolderCommandCanExecute()
        {
            return !IsExporting;
        }

        private void OpenTargetFolderCommandExecute()
        {
      
            var settings = new FolderBrowserDialogSettings
            {
                Description = "Select Destination Folder",
                SelectedPath = TargetPath,
                ShowNewFolderButton = true,
            };

            var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();

            //Close pop up during folder dialog show
            IsStayPopup = false;

            bool? res = dialogService.ShowFolderBrowserDialog(settings);
            if (res == true)
            {
                if (!string.IsNullOrEmpty(settings.SelectedPath) || !string.IsNullOrWhiteSpace(settings.SelectedPath))
                {
                    TargetPath = settings.SelectedPath;
                    s_lastDirBase = TargetPath;
                }
            }
            
            // re-open pop up with updated (if it has changed) target path
            IsStayPopup = true;
        }

        #endregion
    }
}
