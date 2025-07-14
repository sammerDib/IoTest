using System;

using CommunityToolkit.Mvvm.ComponentModel;

using MvvmDialogs.FrameworkDialogs.FolderBrowser;

using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class ExportPointsVM : ObservableObject
    {
        private static string s_lastDirBase = @"C:\Temp\";
        
        #region Constructors

        public ExportPointsVM()
        {
            string sBasePath = System.Configuration.ConfigurationManager.AppSettings.Get("ExportBasePath");
            if (sBasePath != null)
                s_lastDirBase = sBasePath;
            
            GenerateNewTargetPath();
        }

        #endregion

        #region Properties


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
            return !string.IsNullOrWhiteSpace(TargetPath) && !IsExporting;
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
