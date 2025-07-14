using System;
using Microsoft.Win32;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.Result.CommonUI.ViewModel.Search.SettingsPages
{
    public class KlarfSettingsPageViewModel : BaseSettingsPageViewModel
    {
        #region Overrides of BaseSettingsPageViewModel

        public override string PageName => "Klarf";

        #endregion

        public KlarfSettingsPageViewModel(DuplexServiceInvoker<IResultService> resultService) : base(resultService)
        {
        }

        #region Properties

        private DefectBinsVM _defectbinsvm;
        
        public DefectBinsVM CurDefectBinsVM
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
                    }

                    _defectbinsvm = value;
                    OnPropertyChanged();

                    if (_defectbinsvm != null)
                    {
                        _defectbinsvm.OnSaveDefectBins += OnSaveDefectBins;
                        _defectbinsvm.OnExportDefectBins += OnExportDefectBins;
                    }
                }
            }
        }

        private SizeBinsVM _sizebinsvm;

        public SizeBinsVM CurSizeBinsVM
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
                    }

                    _sizebinsvm = value;
                    OnPropertyChanged();

                    if (_sizebinsvm != null)
                    {
                        _sizebinsvm.OnSaveSizeBins += OnSaveSizeBins;
                        _sizebinsvm.OnExportSizeBins += OnExportSizeBins;
                    }
                }
            }
        }

        #endregion

        #region Privates

        private void OnSaveDefectBins(DefectBins defbins)
        {
            bool connectOk = true;

            try
            {
                // update database
                ResultService.Invoke(x => x.RemoteUpdateKlarfDefectBins(defbins));
            }
            catch (Exception)
            {
                var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVm.AddMessage(new Message(MessageLevel.Error, "Connection lost while saving Size bins"));
                // to do perte connexion
                connectOk = false;
            }

            // update klarf display
            if (connectOk)
                ResFactory.GetDisplayFormat(ResultType.ADC_Klarf).UpdateInternalDisplaySettingsPrm(defbins);
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

        private void OnSaveSizeBins(SizeBins szbins)
        {
            bool connectOk = true;

            try
            {
                // update database
                ResultService.Invoke(x => x.RemoteUpdateKlarfSizeBins(szbins));
            }
            catch (Exception)
            {
                var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVm.AddMessage(new Message(MessageLevel.Error, "Connection lost while saving Size bins"));
                // to do perte connexion
                connectOk = false;
            }

            // update klarf display
            if (connectOk)
                ResFactory.GetDisplayFormat(ResultType.ADC_Klarf).UpdateInternalDisplaySettingsPrm(szbins);
        }

        private void OnExportSizeBins(SizeBins szbins)
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
                if (szbins.ExportToXml(filePath))
                    notifierVm.AddMessage(new Message(MessageLevel.Information, "Export Size Bin : " + filePath + " was saved with success"));
                else
                    notifierVm.AddMessage(new Message(MessageLevel.Error, "Unable to Export Size Bin to " + filePath));
            }
        }

        #endregion

        #region Public Methods

        public void RefreshKlarfSettings()
        {
            bool connectOk = true;
            try
            {
                var klarfSettings = ResultService.Invoke(x => x.GetKlarfSettingsFromTables());
                ResFactory.GetDisplayFormat(ResultType.ADC_Klarf).UpdateInternalDisplaySettingsPrm(klarfSettings.RoughBins, klarfSettings.SizeBins);
            }
            catch (Exception)
            {
                var notifierVm = ClassLocator.Default.GetInstance<NotifierVM>();
                notifierVm.AddMessage(new Message(MessageLevel.Error, "Connection lost while Refresh database Klarf Settings"));

                // to do perte connexion
                connectOk = false;
            }

            if (connectOk)
            {
                CurSizeBinsVM = new SizeBinsVM(ResFactory.KlarfDisplay_SizeBins());
                CurDefectBinsVM = new DefectBinsVM(ResFactory.KlarfDisplay_DefectBins());
            }
            else
            {
                CurSizeBinsVM = null;
                CurDefectBinsVM = null;
            }
        }

        #endregion
    }
}
