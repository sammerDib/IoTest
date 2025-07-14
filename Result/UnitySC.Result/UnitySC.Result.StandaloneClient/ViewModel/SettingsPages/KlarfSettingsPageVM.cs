using System;

using Microsoft.Win32;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

using Settings = UnitySC.Result.StandaloneClient.ViewModel.Common.Settings;

namespace UnitySC.Result.StandaloneClient.ViewModel.SettingsPages
{
    public class KlarfSettingsPageVM : BaseSettingsPageVM
    {
        #region Overrides of BaseSettingsPageViewModel

        public override string PageName => "Klarf";

        #endregion

        #region Properties

        private DefectBinsVM _defectbinsvm;
        
        public DefectBinsVM CurrentDefectBinsVM
        {
            get => _defectbinsvm;
            set
            {
                if (_defectbinsvm != value)
                {
                    if (_defectbinsvm != null)
                    {
                        _defectbinsvm.OnSaveDefectBins -= OnSaveDefectBins;
                        _defectbinsvm.OnExportDefectBins -= OnExportDefectBins;
                        _defectbinsvm.OnImportDefectBins -= OnImportDefectBins;
                    }

                    _defectbinsvm = value;
                    OnPropertyChanged();

                    if (_defectbinsvm != null)
                    {
                        _defectbinsvm.OnSaveDefectBins += OnSaveDefectBins;
                        _defectbinsvm.OnExportDefectBins += OnExportDefectBins;
                        _defectbinsvm.OnImportDefectBins += OnImportDefectBins;
                    }
                }
            }
        }

        private SizeBinsVM _sizebinsvm;

        public SizeBinsVM CurrentSizeBinsVM
        {
            get => _sizebinsvm;
            set
            {
                if (_sizebinsvm != value)
                {
                    if (_sizebinsvm != null)
                    {
                        _sizebinsvm.OnSaveSizeBins -= OnSaveSizeBins;
                        _sizebinsvm.OnExportSizeBins -= OnExportSizeBins;
                        _sizebinsvm.OnImportSizeBins -= OnImportSizeBins;
                    }

                    _sizebinsvm = value;
                    OnPropertyChanged();

                    if (_sizebinsvm != null)
                    {
                        _sizebinsvm.OnSaveSizeBins += OnSaveSizeBins;
                        _sizebinsvm.OnExportSizeBins += OnExportSizeBins;
                        _sizebinsvm.OnImportSizeBins += OnImportSizeBins;
                    }
                }
            }
        }

        #endregion

        #region Privates

        #region DefectBins

        private void OnSaveDefectBins(DefectBins defbins)
        {
            var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();
            string filePath = Settings.KlarfRoughBinPath;

            notifierVm.AddMessage(defbins.ExportToXml(filePath)
                ? new Message(MessageLevel.Information, "Defect Bin was saved with success")
                : new Message(MessageLevel.Error, "Unable to save Defect Bin to " + filePath));

            ReloadConfig();

            CurrentDefectBinsVM.BinsSaved();
        }

        private void OnExportDefectBins(DefectBins defbins)
        {
            var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();
            var dialog = new SaveFileDialog
            {
                //dialog.InitialDirectory = ???
                Filter = "Xml Files (*.xml)|*.xml|All Files (*.*)|*.*"
            };

            bool? bOk = dialog.ShowDialog();
            if (!bOk.HasValue)
                return;

            if (bOk.Value)
            {
                string filePath = dialog.FileName;
                if (defbins.ExportToXml(filePath))
                    notifierVm.AddMessage(new Message(MessageLevel.Information, "Export Defect Bin : " + filePath + " was saved with success"));
                else
                    notifierVm.AddMessage(new Message(MessageLevel.Error, "Unbale to Export Defect Bin to " + filePath));
            }
        }

        private void OnImportDefectBins()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "Xml Files (*.xml)|*.xml|All Files (*.*)|*.*"
            };

            bool? fileSelected = openFileDialog.ShowDialog();

            if (fileSelected == null || !fileSelected.Value) return;

            string filePath = openFileDialog.FileName;
            try
            {
                var defectBins = DefectBins.ImportFromXml(filePath);
                var defectBinsVM = new DefectBinsVM(defectBins);
                CurrentDefectBinsVM = defectBinsVM;
                CurrentDefectBinsVM.Bin_CollectionChanged(this, null);
            }
            catch (Exception ex)
            {
                var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVm.AddMessage(new Message(MessageLevel.Error, "An error occurred when importing a defect bins file. Make sure the file is a valid defect bins config file. Error : " + ex.Message));
            }
        }

        #endregion

        #region SizeBins

        private void OnSaveSizeBins(SizeBins szbins)
        {
            var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();
            string filePath = Settings.KlarfSizeBinPath;

            notifierVm.AddMessage(szbins.ExportToXml(filePath)
                ? new Message(MessageLevel.Information, "Size Bin was saved with success")
                : new Message(MessageLevel.Error, "Unable to save Size Bin to " + filePath));

            ReloadConfig();

            CurrentSizeBinsVM.BinsSaved();
        }

        private void OnExportSizeBins(SizeBins szbins)
        {
            var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();
            var dialog = new SaveFileDialog
            {
                Filter = "Xml Files (*.xml)|*.xml|All Files (*.*)|*.*"
            };

            bool? bOk = dialog.ShowDialog();
            if (!bOk.HasValue)
                return;

            if (bOk.Value)
            {
                string filePath = dialog.FileName;
                if (szbins.ExportToXml(filePath))
                    notifierVm.AddMessage(new Message(MessageLevel.Information, "Export Size Bin : " + filePath + " was saved with success"));
                else
                    notifierVm.AddMessage(new Message(MessageLevel.Error, "Unable to Export Size Bin to " + filePath));
            }
        }

        private void OnImportSizeBins()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Xml Files (*.xml)|*.xml|All Files (*.*)|*.*"
            };

            bool? fileSelected = openFileDialog.ShowDialog();

            if (fileSelected == null || !fileSelected.Value) return;

            string filePath = openFileDialog.FileName;
            try
            {
                var sizeBins = SizeBins.ImportFromXml(filePath);
                var sizeBinsVM = new SizeBinsVM(sizeBins);
                CurrentSizeBinsVM = sizeBinsVM;
                CurrentSizeBinsVM.Bin_CollectionChanged(this, null);
            }
            catch (Exception ex)
            {
                var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVm.AddMessage(new Message(MessageLevel.Error, "An error occurred when importing a size bins file. Make sure the file is a valid size bin config file. Error : " + ex.Message));
            }

        }

        #endregion

        private void ReloadConfig()
        {
            var klf = Settings.LoadKlarfSettings(Settings.KlarfRoughBinPath, Settings.KlarfSizeBinPath);
            ResFactory.GetDisplayFormat(ResultType.ADC_Klarf).UpdateInternalDisplaySettingsPrm(klf.RoughBins, klf.SizeBins);

            App.Instance.Settings.KlarfSettings = klf;
        }

        #endregion

        #region Public Methods

        public void LoadKlarfSettings()
        {
            try
            {
                var klarfSettings = Settings.LoadKlarfSettings(Settings.KlarfRoughBinPath, Settings.KlarfSizeBinPath);
                ResFactory.GetDisplayFormat(ResultType.ADC_Klarf).UpdateInternalDisplaySettingsPrm(klarfSettings.RoughBins, klarfSettings.SizeBins);

                App.Instance.Settings.KlarfSettings = klarfSettings;
                CurrentSizeBinsVM = new SizeBinsVM(ResFactory.KlarfDisplay_SizeBins());
                CurrentDefectBinsVM = new DefectBinsVM(ResFactory.KlarfDisplay_DefectBins());
            }
            catch (Exception)
            {
                var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVm.AddMessage(new Message(MessageLevel.Error, "No Klarf Settings found"));

                CurrentSizeBinsVM = null;
                CurrentDefectBinsVM = null;
            }
        }

        #endregion
    }
}
