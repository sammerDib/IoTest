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

    public class RepeatMeasureDialogViewModel : ObservableObject, IModalDialogViewModel

    {
        #region Fields
        private readonly IDialogOwnerService _dialogService;
        private MeasureConfiguration _measureConfig;
        private bool? _dialogResult;
        #endregion


        #region Private methods
        private AutoRelayCommand _startRepeatMeasure;
        private AutoRelayCommand _chooseFolder;

        private bool AllowRepeatMeasure()
        {

            bool checkNumber = MeasureConfig.NumberOfMeasure > 0;
            bool checkFolderName = !string.IsNullOrEmpty(MeasureConfig.FolderName) && Directory.Exists(MeasureConfig.FolderName);
            return checkNumber && checkFolderName;
        }
        #endregion


        #region Public methods

        public RepeatMeasureDialogViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
                MeasureConfig.NumberOfMeasure = 100;
                MeasureConfig.FolderName = Directory.GetCurrentDirectory();
            }
        }


        public MeasureConfiguration MeasureConfig
        {
            get
            {
                return _measureConfig ?? (_measureConfig = new MeasureConfiguration());
            }
            set
            {
                _measureConfig = value;
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

        public AutoRelayCommand StartRepeatMeasure
        {
            get
            {
                return _startRepeatMeasure
                    ?? (_startRepeatMeasure = new AutoRelayCommand(
                    () =>
                    {
                        DialogResult = true;
                    },
                    AllowRepeatMeasure
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
                            MeasureConfig.FolderName = folderBrowserDialogSettings.SelectedPath;
                        }
                    },
                    () => true
                    ));
            }
        }
        #endregion


    }
}
