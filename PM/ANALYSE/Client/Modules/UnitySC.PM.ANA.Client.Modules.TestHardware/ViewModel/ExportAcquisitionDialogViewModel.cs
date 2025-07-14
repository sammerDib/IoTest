using System.ComponentModel;
using System.IO;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.FolderBrowser;

using UnitySC.PM.ANA.Client.Proxy.Probe.Models;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel
{

    public class ExportAcquisitionDialogViewModel : ObservableObject, IModalDialogViewModel

    {
        #region Fields
        private readonly IDialogOwnerService _dialogService;
        private ExportConfiguration _exportConfig;
        private bool? _dialogResult;
        #endregion


        #region Private methods
        private AutoRelayCommand _startExportAcquisition;
        private AutoRelayCommand _chooseFolder;

        private bool AllowAcquisitionExport()
        {
            bool checkType = ExportConfig.ExportRawData || ExportConfig.ExportSelectedPeaks;
            bool checkNumber = ExportConfig.NumberOfAcquisition > 0;
            bool checkFolderName = !string.IsNullOrEmpty(ExportConfig.FolderName) && Directory.Exists(ExportConfig.FolderName);
            return checkType && checkNumber && checkFolderName;
        }
        #endregion


        #region Public methods

        public ExportAcquisitionDialogViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
                ExportConfig.NumberOfAcquisition = 20;
                ExportConfig.ExportRawData = true;
                ExportConfig.FolderName = Directory.GetCurrentDirectory();
            }
        }


        public ExportConfiguration ExportConfig
        {
            get
            {
                return _exportConfig ?? (_exportConfig = new ExportConfiguration());
            }
            set
            {
                _exportConfig = value;
            }
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value, nameof(DialogResult));
        }
        private ExportConfiguration _acquisitionToExport;

        public ExportConfiguration AcquisitionToExport
        {
            get
            {
                return _acquisitionToExport;
            }
            set
            {
                if (_acquisitionToExport == value)
                {
                    return;
                }
                _acquisitionToExport = value;
                OnPropertyChanged(nameof(AcquisitionToExport));
            }
        }

        public AutoRelayCommand StartExportAcquisition
        {
            get
            {
                return _startExportAcquisition
                    ?? (_startExportAcquisition = new AutoRelayCommand(
                    () =>
                    {
                        DialogResult = true;
                    },
                    AllowAcquisitionExport
                    ));
            }
        }
        public AutoRelayCommand ChooseFolder
        {
            get
            {
                return _chooseFolder
                    ?? (_chooseFolder = new AutoRelayCommand(
                    () =>
                    {
                        var folderBrowserDialogSettings = new FolderBrowserDialogSettings();
                        var success = _dialogService.ShowFolderBrowserDialog(this, folderBrowserDialogSettings);
                        if (success.Value)
                        {
                            ExportConfig.FolderName = folderBrowserDialogSettings.SelectedPath;
                        }
                    },
                    () => true
                    ));
            }
        }
        #endregion


    }
}
