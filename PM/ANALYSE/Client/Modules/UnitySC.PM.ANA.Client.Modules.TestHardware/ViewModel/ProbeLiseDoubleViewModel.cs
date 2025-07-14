using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

using UnitySC.PM.ANA.Client.Modules.TestHardware.View.Dialog;
using UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel.Export;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Client.Proxy.Probe.Models;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel
{
    public class ProbeLiseDoubleViewModel : ProbeLiseViewModelBase, IDisposable
    {
        #region Fields

        private IDialogOwnerService _dialogService;
        private ExportConfiguration _exportConfiguration;
        private MeasureConfiguration _measureConfiguration;
        private List<LiseResult> _rawRepeatMeasure = new List<LiseResult>();
        private ExportResultDoubleLise _exportResult;
        private Stopwatch _runningExport;
        private bool _recordOnlyProbeUp = true;
        private bool _recordingInProgress;
        private DualLiseInputParams _inputParametersLise;
        private ProbesSupervisor _probeSupervisor;

        private ProbeLiseBaseVM _probeUsed = null;

        public ProbeLiseBaseVM ProbeUsed
        {
            get => _probeUsed;
            set
            {
                if (_probeUsed != value)
                {
                    _probeUsed = value;
                    OnPropertyChanged(nameof(ProbeUsed));
                }
            }
        }

        #endregion Fields

        #region Constructor

        public ProbeLiseDoubleViewModel(IProbeService probeSupervisor, string probeID)
        {
            _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            _exportResult = new ExportResultDoubleLise();
            _exportConfiguration = new ExportConfiguration();
            _probeSupervisor = probeSupervisor as ProbesSupervisor;
            var dualProbe = _probeSupervisor.Probes.FirstOrDefault(p => p.DeviceID == probeID);

            Probe = dualProbe as ProbeLiseDoubleVM;

            if (!(Probe is null))
            {
                Probe.RawSignalUpdated += ProbeRawSignalUpdated;
                Probe.ThicknessMeasureUpdated += ProbeThicknessMeasureUpdated;
                (Probe as ProbeLiseDoubleVM).ProbeUsedUpdated += UpdateProbeUsed;
            }

            UpdateProbeUsed((Probe as ProbeLiseDoubleVM).IsAcquisitionForProbeUp);
        }

        #endregion Constructor

        private ProbeLiseDoubleVM _probeLiseDouble => Probe as ProbeLiseDoubleVM;

        #region Public Methods

        public MeasureConfiguration MeasureConfiguration
        {
            get
            {
                return _measureConfiguration;
            }
            set
            {
                if (_measureConfiguration == value)
                {
                    return;
                }
                _measureConfiguration = value;
                OnPropertyChanged(nameof(MeasureConfiguration));
            }
        }

        public ExportConfiguration ExportConfiguration
        {
            get
            {
                return _exportConfiguration;
            }
            set
            {
                if (_exportConfiguration == value)
                {
                    return;
                }
                _exportConfiguration = value;
                OnPropertyChanged(nameof(ExportConfiguration));
            }
        }

        public ExportResultDoubleLise ExportResult
        {
            get
            {
                if (_exportResult == null)
                {
                    return new ExportResultDoubleLise();
                }
                return _exportResult;
            }
            set
            {
                if (_exportResult == value)
                {
                    return;
                }
                _exportResult = value;
                OnPropertyChanged(nameof(ExportResult));
            }
        }

        public AutoRelayCommand ExportAcquisition
        {
            get
            {
                return _exportAcquisition
                    ?? (_exportAcquisition = new AutoRelayCommand(

                     async () =>
                     {
                         try
                         {
                             var dialogViewModel = new ExportAcquisitionDialogViewModel();
                             bool? success = _dialogService.ShowCustomDialog<ExportAcquisitionDialog>(dialogViewModel);
                             if (success != true)
                                 return;

                             ExportResult.Date = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
                             _runningExport = Stopwatch.StartNew();
                             ExportConfiguration = dialogViewModel.ExportConfig;
                             ExportConfiguration.IsRunning = true;
                             _inputParametersLise = _probeLiseDouble.GetInputParametersForAcquisition() as DualLiseInputParams;
                             if (_inputParametersLise == null)
                                 return;

                             for (int currentAcquisition = 0; currentAcquisition < ExportConfiguration.NumberOfAcquisition; currentAcquisition++)
                             {
                                 // Double Lise Up
                                 _inputParametersLise.CurrentProbeAcquisition = (_probeLiseDouble.Configuration as ProbeConfigurationLiseDoubleVM).ProbeUp.DeviceID;
                                 _inputParametersLise.CurrentProbeModule = (_probeLiseDouble.Configuration as ProbeConfigurationLiseDoubleVM).ProbeUp.ModulePosition;
                                 _recordingInProgress = true;
                                 var probe = _probeSupervisor.Probes.FirstOrDefault(p => p.DeviceID == _inputParametersLise.CurrentProbeAcquisition);
                                 bool startAcqResult = probe.StartContinuousAcquisition(_inputParametersLise);
                                 if (!startAcqResult)
                                 {
                                     ExportConfiguration.IsRunning = false;
                                     _probeLiseDouble.SettingsChanged = false;
                                     return;
                                 }
                                 //we record one acquisition for lise up and we continue the process
                                 await TaskExt.WaitWhile(() => (ExportResult.YRawAcquisitionProbeUp.Count < (currentAcquisition + 1)) && (ExportResult.SelectedPeaksProbeUp.Count < (currentAcquisition + 1)), 10_000);
                                 probe.StopContinuousAcquisition();

                                 // Double Lise Down
                                 if (ExportConfiguration.IsRunning)
                                 {
                                     _inputParametersLise.CurrentProbeAcquisition = (_probeLiseDouble.Configuration as ProbeConfigurationLiseDoubleVM).ProbeDown.DeviceID;
                                     _inputParametersLise.CurrentProbeModule = (_probeLiseDouble.Configuration as ProbeConfigurationLiseDoubleVM).ProbeDown.ModulePosition;
                                     _recordingInProgress = true;
                                     probe = _probeSupervisor.Probes.FirstOrDefault(p => p.DeviceID == _inputParametersLise.CurrentProbeAcquisition);
                                     startAcqResult = probe.StartContinuousAcquisition(_inputParametersLise);
                                     if (!startAcqResult)
                                     {
                                         ExportConfiguration.IsRunning = false;
                                         _probeLiseDouble.SettingsChanged = false;
                                         return;
                                     }
                                     //we record one acquisition for lise bottom and we continue the process
                                     await TaskExt.WaitWhile(() => (ExportResult.YRawAcquisitionProbeDown.Count < (currentAcquisition + 1)) && (ExportResult.SelectedPeaksProbeDown.Count < (currentAcquisition + 1)), 10_000);
                                     probe.StopContinuousAcquisition();
                                 }
                             }

                             if (ExportConfiguration.IsRunning)
                             {
                                 ProcessingAcquisitionDataForExport();
                             }
                             _probeLiseDouble.SettingsChanged = false;
                         }
                         catch
                         {
                         }
                     },

                    () => _probeLiseDouble.CheckInputParametersValidity() && (Probe.State.Status != DeviceStatus.Busy)));
            }
        }

        public AutoRelayCommand RepeatMeasure
        {
            get
            {
                return _repeatMeasure
                    ?? (_repeatMeasure = new AutoRelayCommand(

                     async () =>
                     {
                         try
                         {
                             var dialogViewModel = new RepeatMeasureDialogViewModel();
                             bool? success = _dialogService.ShowCustomDialog<RepeatMeasureDialog>(dialogViewModel);
                             if (success != true)
                                 return;

                             ExportResult.Date = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
                             _runningExport = Stopwatch.StartNew();
                             MeasureConfiguration = dialogViewModel.MeasureConfig;
                             //MeasureConfiguration.IsRunning = true;
                             Probe.State = new DeviceState(DeviceStatus.Busy);
                             _inputParametersLise = _probeLiseDouble.GetInputParametersForAcquisition() as DualLiseInputParams;
                             if (_inputParametersLise == null)
                                 return;
                             _recordingInProgress = true;
                             Probe.DoMultipleMeasures(_inputParametersLise, MeasureConfiguration.NumberOfMeasure.Value);

                             //we record one acquisition for lise double
                             await TaskExt.WaitWhile(() => _rawRepeatMeasure.Count < MeasureConfiguration.NumberOfMeasure);
                         }
                         catch
                         {
                         }
                     },

                    () => _probeLiseDouble.CheckInputParametersValidity() && (Probe.State.Status != DeviceStatus.Busy)
                    ));
            }
        }

        public AutoRelayCommand StartDoubleAcquisition
        {
            get
            {
                return _startDoubleAcquisition
                    ?? (_startDoubleAcquisition = new AutoRelayCommand(

                   () =>
                   {
                       var inputParams = Probe.GetInputParametersForAcquisition();
                       ProbeUsed.StartContinuousAcquisition(inputParams);
                   },
                    () => Probe.CheckInputParametersValidity() && ProbeUsed.State.Status != DeviceStatus.Busy
                    ));
            }
        }

        public AutoRelayCommand StopDoubleAcquisition
        {
            get
            {
                return _stopDoubleAcquisition
                    ?? (_stopDoubleAcquisition = new AutoRelayCommand(

                   () =>
                   {
                       ProbeUsed.StopContinuousAcquisition();
                       if (ExportConfiguration.IsRunning)
                       {
                           ExportConfiguration.IsRunning = false;
                           ProcessingAcquisitionDataForExport();
                       }
                   },
                    () => ProbeUsed.State.Status == DeviceStatus.Busy || ExportConfiguration.IsRunning
                    ));
            }
        }

        public AutoRelayCommand DoCalibration
        {
            get
            {
                return _doCalibration
                    ?? (_doCalibration = new AutoRelayCommand(

                     () =>
                     {
                         var dialogViewModel = new CalibrateDialogViewModel();
                         bool? success = _dialogService.ShowCustomDialog<CalibrateSettingsDialog>(dialogViewModel);
                         if (success == true)
                         {
                             _probeLiseDouble.IsCalibrated = false;
                             var calibrateConfiguration = dialogViewModel.CalibrateParams;

                             calibrateConfiguration.TopLiseAirgapThreshold = 0.6;
                             calibrateConfiguration.BottomLiseAirgapThreshold = 0.7;
                             calibrateConfiguration.CalibrationMode = 3;
                             var axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
                             var position = (XYZTopZBottomPosition)axesSupervisor.GetCurrentPosition()?.Result;
                             if (position != null)
                             {
                                 calibrateConfiguration.ZTopUsedForCalib = position.ZTop;
                                 calibrateConfiguration.ZBottomUsedForCalib = position.ZBottom;

                                 var inputParametersLise = _probeLiseDouble.GetInputParametersForAcquisition();

                                 _probeLiseDouble.IsCalibrated = Probe.StartCalibration(calibrateConfiguration, inputParametersLise);
                             }
                         }
                     },

                    () => _probeLiseDouble.CheckInputParametersValidity() && (Probe.State.Status != DeviceStatus.Busy)
                    ));
            }
        }

        private void ProbeRawSignalUpdated(ProbeSignalBase probeRawSignal)
        {
            if (!(probeRawSignal is ProbeLiseSignal))
                return;
            if (ExportConfiguration != null && ExportConfiguration.NumberOfAcquisition != null)
                Application.Current?.Dispatcher.BeginInvoke(new Action(() => UpdateProbeRawSignalForExport((ProbeLiseSignal)probeRawSignal)));
        }

        #endregion Public Methods

        #region Private Methods

        private AutoRelayCommand _exportAcquisition;
        private AutoRelayCommand _repeatMeasure;
        private AutoRelayCommand _startDoubleAcquisition;
        private AutoRelayCommand _stopDoubleAcquisition;
        private AutoRelayCommand _doCalibration;

        private void UpdateProbeRawSignalForExport(ProbeLiseSignal rawSignal)
        {
            if (ExportConfiguration.NumberOfAcquisition != 0 && _recordingInProgress)
            {
                if (_recordOnlyProbeUp && ExportResult.YRawAcquisitionProbeUp.Count() != ExportConfiguration.NumberOfAcquisition && ExportResult.SelectedPeaksProbeUp.Count() != ExportConfiguration.NumberOfAcquisition)
                {
                    RecordAcquisitionProbeUpForExport(rawSignal);
                    _recordingInProgress = false;
                    _recordOnlyProbeUp = false;
                }
                else if (!_recordOnlyProbeUp && ExportResult.YRawAcquisitionProbeDown.Count() != ExportConfiguration.NumberOfAcquisition && ExportResult.SelectedPeaksProbeDown.Count() != ExportConfiguration.NumberOfAcquisition)
                {
                    RecordAcquisitionProbeDownForExport(rawSignal);
                    _recordingInProgress = false;
                    _recordOnlyProbeUp = true;
                }
            }
        }

        private void ProcessingAcquisitionDataForExport()
        {
            CreateAndCompleteCSVFileForExport();
            _runningExport.Stop();
            var message = $"Execution time of export : {TimeSpan.FromSeconds(_runningExport.Elapsed.TotalSeconds).ToString("mm'm 'ss's 'fff'ms'")}";
            ExportConfiguration = new ExportConfiguration();
            ExportResult = new ExportResultDoubleLise();
            Application.Current?.Dispatcher.BeginInvoke(new Action(() => _dialogService.ShowMessageBox(message, "Export", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None)));
        }

        private void CreateAndCompleteCSVFileForExport()
        {
            if (ExportConfiguration.ExportSelectedPeaks)
            {
                ExportConfiguration.FileName = $"Export_{ExportResult.Date}_{Probe.DeviceID}_ProbeUp_SelectedPeaks.csv";
                string filePathSelected = ExportConfiguration.FolderName + "\\" + ExportConfiguration.FileName;
                using (var file = new StreamWriter(filePathSelected))
                {
                    file.WriteLine($"Selected Peaks for {(_probeLiseDouble.Configuration as ProbeConfigurationLiseDoubleVM).ProbeUp.DeviceID}");
                    CsvExport.CompleteCsvWithSelectedPoint(file, ExportResult.SelectedPeaksProbeDown);
                }

                ExportConfiguration.FileName = $"Export_{ExportResult.Date}_{Probe.DeviceID}_ProbeDown_SelectedPeaks.csv";
                filePathSelected = ExportConfiguration.FolderName + "\\" + ExportConfiguration.FileName;
                using (var file = new StreamWriter(filePathSelected))
                {
                    file.WriteLine($"Selected Peaks for {(_probeLiseDouble.Configuration as ProbeConfigurationLiseDoubleVM).ProbeDown.DeviceID}");
                    CsvExport.CompleteCsvWithSelectedPoint(file, ExportResult.SelectedPeaksProbeDown);
                }
            }
            if (ExportConfiguration.ExportRawData)
            {
                ExportConfiguration.FileName = $"Export_{ExportResult.Date}_{Probe.DeviceID}_ProbeUp_RawData.csv";
                string filePathRaw = ExportConfiguration.FolderName + "\\" + ExportConfiguration.FileName;
                using (var file = new StreamWriter(filePathRaw))
                {
                    file.WriteLine($"Raw Data for {(_probeLiseDouble.Configuration as ProbeConfigurationLiseDoubleVM).ProbeUp.DeviceID}");
                    CsvExport.CompleteCsvWithRawData(file, ExportResult.XRawAcquisitionProbeUp, ExportResult.YRawAcquisitionProbeUp);
                }

                ExportConfiguration.FileName = $"Export_{ExportResult.Date}_{Probe.DeviceID}_ProbeDown_RawData.csv";
                filePathRaw = ExportConfiguration.FolderName + "\\" + ExportConfiguration.FileName;
                using (var file = new StreamWriter(filePathRaw))
                {
                    file.WriteLine($"Raw Data for {(_probeLiseDouble.Configuration as ProbeConfigurationLiseDoubleVM).ProbeDown.DeviceID}");
                    CsvExport.CompleteCsvWithRawData(file, ExportResult.XRawAcquisitionProbeDown, ExportResult.YRawAcquisitionProbeDown);
                }
            }
        }

        private void RecordAcquisitionProbeDownForExport(ProbeLiseSignal rawSignal)
        {
            if (ExportConfiguration.ExportRawData)
            {
                if (ExportResult.XRawAcquisitionProbeDown.Count == 0 && rawSignal.RawValues.Count > ExportResult.XRawAcquisitionProbeDown.Count)
                {
                    ExportResult.XRawAcquisitionProbeDown.Clear();
                    {
                        for (int i = 0; i < rawSignal.RawValues.Count; i++)
                        {
                            ExportResult.XRawAcquisitionProbeDown.Add(i * (rawSignal.StepX / 1000));
                        }
                    }
                }
                ExportResult.YRawAcquisitionProbeDown.Add(rawSignal.RawValues);
            }

            if (ExportConfiguration.ExportSelectedPeaks)
            {
                ExportResult.SelectedPeaksProbeDown.Add(rawSignal.SelectedPeaks);
            }
        }

        private void RecordAcquisitionProbeUpForExport(ProbeLiseSignal rawSignal)
        {
            if (ExportConfiguration.ExportRawData)
            {
                if (ExportResult.XRawAcquisitionProbeUp.Count == 0 && rawSignal.RawValues.Count > ExportResult.XRawAcquisitionProbeUp.Count)
                {
                    ExportResult.XRawAcquisitionProbeUp.Clear();
                    {
                        for (int i = 0; i < rawSignal.RawValues.Count; i++)
                        {
                            ExportResult.XRawAcquisitionProbeUp.Add(i * (rawSignal.StepX / 1000));
                        }
                    }
                }
                ExportResult.YRawAcquisitionProbeUp.Add(rawSignal.RawValues);
            }

            if (ExportConfiguration.ExportSelectedPeaks)
            {
                ExportResult.SelectedPeaksProbeUp.Add(rawSignal.SelectedPeaks);
            }
        }

        private void ProbeThicknessMeasureUpdated(IProbeResult newMeasure)
        {
            if (newMeasure == null)
            {
                Probe.State = new DeviceState(DeviceStatus.Ready);
                Application.Current?.Dispatcher.BeginInvoke(new Action(() => _dialogService.ShowMessageBox("Measuring failed!", "Export", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None)));
            }

            _rawRepeatMeasure.Add((LiseResult)newMeasure);

            if (_rawRepeatMeasure.Count < MeasureConfiguration.NumberOfMeasure)
                return;

            MeasureConfiguration.FileName = $"MEASURES_{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}_{Probe.DeviceID}.csv";
            string filePathSelected = MeasureConfiguration.FolderName + "\\" + MeasureConfiguration.FileName;
            using (var file = new StreamWriter(filePathSelected))
            {
                Probe.ExportMeasures(file, _rawRepeatMeasure, _probeLiseDouble.Configuration.DeviceID);

                file.WriteLine("");
                Probe.State = new DeviceState(DeviceStatus.Ready);
            }
            _runningExport.Stop();
            var message = $"Execution time of export : {TimeSpan.FromSeconds(_runningExport.Elapsed.TotalSeconds).ToString("mm'm 'ss's 'fff'ms'")}";
            Application.Current?.Dispatcher.BeginInvoke(new Action(() => _dialogService.ShowMessageBox(message, "Export", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None)));
        }

        private void UpdateProbeUsed(bool isProbeUp)
        {
            var probeToUseId = "";
            if (isProbeUp)
            {
                probeToUseId = (Probe.Configuration as ProbeConfigurationLiseDoubleVM).ProbeUp.DeviceID;
            }
            else
            {
                probeToUseId = (Probe.Configuration as ProbeConfigurationLiseDoubleVM).ProbeDown.DeviceID;
            }

            bool acquisitionMustBeRestarted = false;
            if (ProbeUsed != null)
            {
                if (ProbeUsed.State.Status == DeviceStatus.Busy)
                {
                    ProbeUsed.StopContinuousAcquisition();
                    acquisitionMustBeRestarted = true;
                }
            }

            var probe = _probeSupervisor.Probes.FirstOrDefault(p => p.DeviceID == probeToUseId);
            ProbeUsed = probe as ProbeLiseVM;

            if (acquisitionMustBeRestarted)
            {
                var inputParams = Probe.GetInputParametersForAcquisition();
                ProbeUsed.StartContinuousAcquisition(inputParams);
            }
        }

        #endregion Private Methods

        public void Dispose()
        {
            if (!(Probe is null))
            {
                Probe.RawSignalUpdated -= ProbeRawSignalUpdated;
                Probe.ThicknessMeasureUpdated -= ProbeThicknessMeasureUpdated;
                (Probe as ProbeLiseDoubleVM).ProbeUsedUpdated -= UpdateProbeUsed;
            }
        }
    }
}
