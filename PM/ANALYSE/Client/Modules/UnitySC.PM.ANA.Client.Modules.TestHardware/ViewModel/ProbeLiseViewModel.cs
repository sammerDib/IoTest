using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.Modules.TestHardware.View.Dialog;
using UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel.Export;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Client.Proxy.Probe.Models;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel
{
    public class ProbeLiseViewModel : ProbeLiseViewModelBase, IDisposable
    {
        #region Constructors

        public ProbeLiseViewModel(IProbeService probeSupervisor, string probeID)
        {
            _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            _exportResult = new ExportResultSimpleLise();
            _exportConfiguration = new ExportConfiguration();
            var probesSupervisor = probeSupervisor as ProbesSupervisor;
            var probe = probesSupervisor.Probes.FirstOrDefault(p => p.DeviceID == probeID);

            Probe = probe as ProbeLiseVM;
            if (!(Probe is null))
            {
                Probe.RawSignalUpdated += ProbeRawSignalUpdated;
                Probe.ThicknessMeasureUpdated += ProbeThicknessMeasureUpdated;
            }
            
        }

        #endregion Constructors

        private ProbeLiseVM _probeLise => Probe as ProbeLiseVM;

        #region Fields

        private ExportConfiguration _exportConfiguration;
        private MeasureConfiguration _measureConfiguration;
        private ExportResultSimpleLise _exportResult;
        private List<LiseResult> _rawRepeatMeasure = new List<LiseResult>();
        private IDialogOwnerService _dialogService;
        private Stopwatch _runningExport;

        #endregion Fields

        #region Private Methods

        private AutoRelayCommand _exportAcquisition;
        private AutoRelayCommand _repeatMeasure;
        private AutoRelayCommand _stopSimpleAcquisition;

        private void UpdateProbeRawSignalForExport(ProbeLiseSignal rawSignal)
        {
            if (ExportConfiguration.NumberOfAcquisition != 0)
            {
                RecordAcquisitionForExport(rawSignal);
                if (ExportResult.YRawAcquisition.Count() == ExportConfiguration.NumberOfAcquisition || ExportResult.SelectedPeaks.Count() == ExportConfiguration.NumberOfAcquisition)
                {
                    Probe.StopContinuousAcquisition();
                    ProcessingAcquisitionDataForExport();
                }
            }
        }

        private void ProcessingAcquisitionDataForExport()
        {
            CreateAndCompleteCSVFileForExport();
            _runningExport.Stop();
            var message = $"Execution time of export : { TimeSpan.FromSeconds(_runningExport.Elapsed.TotalSeconds).ToString("mm'm 'ss's 'fff'ms'")}";
            ExportConfiguration = new ExportConfiguration();
            ExportResult = new ExportResultSimpleLise();
            Application.Current?.Dispatcher.BeginInvoke(new Action(() => _dialogService.ShowMessageBox(message, "Export", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None)));
        }

        private void CreateAndCompleteCSVFileForExport()
        {
            Console.WriteLine("CreateAndCompleteCSVFileForExport");
            if (ExportConfiguration.ExportSelectedPeaks)
            {
                ExportConfiguration.FileName = $"Export_{ExportResult.Date}_{Probe.DeviceID}_SelectedPeaks.csv";
                string filePath = ExportConfiguration.FolderName + "\\" + ExportConfiguration.FileName;
                using (var file = new StreamWriter(filePath))
                {
                    {
                        CsvExport.CompleteCsvWithSelectedPoint(file, ExportResult.SelectedPeaks);
                    }
                }
            }
            if (ExportConfiguration.ExportRawData)
            {
                ExportConfiguration.FileName = $"Export_{ ExportResult.Date}_{Probe.DeviceID}_RawData.csv";
                string filePath = ExportConfiguration.FolderName + "\\" + ExportConfiguration.FileName;
                using (var file = new StreamWriter(filePath))
                {
                    CsvExport.CompleteCsvWithRawData(file, ExportResult.XRawAcquisition, ExportResult.YRawAcquisition);
                }
            }
        }

        private void RecordAcquisitionForExport(ProbeLiseSignal rawSignal)
        {
            if (ExportConfiguration.ExportRawData)
            {
                if (ExportResult.XRawAcquisition.Count == 0 && rawSignal.RawValues.Count > ExportResult.XRawAcquisition.Count)
                {
                    ExportResult.XRawAcquisition.Clear();
                    {
                        for (int i = 0; i < rawSignal.RawValues.Count; i++)
                        {
                            ExportResult.XRawAcquisition.Add(i * (rawSignal.StepX / 1000));
                        }
                    }
                }
                ExportResult.YRawAcquisition.Add(rawSignal.RawValues);
            }

            if (ExportConfiguration.ExportSelectedPeaks)
            {
                ExportResult.SelectedPeaks.Add(rawSignal.SelectedPeaks);
            }
        }

        #endregion Private Methods

        #region Public Methods

        public AutoRelayCommand ExportAcquisition
        {
            get
            {
                return _exportAcquisition
                    ?? (_exportAcquisition = new AutoRelayCommand(
                    () =>
                    {
                        var dialogViewModel = new ExportAcquisitionDialogViewModel();
                        bool? success = _dialogService.ShowCustomDialog<ExportAcquisitionDialog>(dialogViewModel);
                        if (success != true)
                            return;

                        ExportConfiguration = dialogViewModel.ExportConfig;
                        var inputParametersLise = _probeLise.GetInputParametersForAcquisition() as SingleLiseInputParams;
                        if (inputParametersLise == null)
                            return;
                        ExportConfiguration.IsRunning = true;
                        ExportResult.Date = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        _runningExport = System.Diagnostics.Stopwatch.StartNew();
                        bool startAckResult = Probe.StartContinuousAcquisition(inputParametersLise);

                        _probeLise.SettingsChanged = false;
                    },
                    () => _probeLise.CheckInputParametersValidity() && (Probe.State.Status != DeviceStatus.Busy)));
            }
        }

        public AutoRelayCommand RepeatMeasure
        {
            get
            {
                return _repeatMeasure
                    ?? (_repeatMeasure = new AutoRelayCommand(
                    () =>
                    {
                        var dialogViewModel = new RepeatMeasureDialogViewModel();
                        bool? success = _dialogService.ShowCustomDialog<RepeatMeasureDialog>(dialogViewModel);
                        if (success != true)
                            return;
                        _rawRepeatMeasure.Clear();
                        MeasureConfiguration = dialogViewModel.MeasureConfig;
                        var inputParametersLise = _probeLise.GetInputParametersForAcquisition() as SingleLiseInputParams;
                        if (inputParametersLise == null)
                            return;
                        //MeasureConfiguration.IsRunning = true;
                        Probe.State = new DeviceState(DeviceStatus.Busy);
                        ExportResult.Date = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        _runningExport = System.Diagnostics.Stopwatch.StartNew();
                        Probe.DoMultipleMeasures(inputParametersLise, MeasureConfiguration.NumberOfMeasure.GetValueOrDefault(100));

                        _probeLise.SettingsChanged = false;
                    },
                    () => _probeLise.CheckInputParametersValidity() && (Probe.State.Status != DeviceStatus.Busy)
                    ));
            }
        }

        public AutoRelayCommand StopSimpleAcquisition
        {
            get
            {
                return _stopSimpleAcquisition
                    ?? (_stopSimpleAcquisition = new AutoRelayCommand(

                   () =>
                   {
                       Probe.StopContinuousAcquisition();
                       if (ExportConfiguration.IsRunning)
                           ProcessingAcquisitionDataForExport();
                   },
                    () => Probe.State.Status == DeviceStatus.Busy || ExportConfiguration.IsRunning));
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

        public ExportResultSimpleLise ExportResult
        {
            get
            {
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

        private void ProbeRawSignalUpdated(ProbeSignalBase probeRawSignal)
        {
            if (!(probeRawSignal is ProbeLiseSignal))
                return;
            if (ExportConfiguration != null && ExportConfiguration.NumberOfAcquisition != null)
                Application.Current?.Dispatcher.BeginInvoke(new Action(() => UpdateProbeRawSignalForExport((ProbeLiseSignal)probeRawSignal)));
        }

        private void ProbeThicknessMeasureUpdated(IProbeResult newMeasure)
        {
            _rawRepeatMeasure.Add((LiseResult)newMeasure);

            if (_rawRepeatMeasure.Count < MeasureConfiguration.NumberOfMeasure)
                return;

            MeasureConfiguration.FileName = $"MEASURES_{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}_{Probe.DeviceID}.csv";
            string filePathSelected = MeasureConfiguration.FolderName + "\\" + MeasureConfiguration.FileName;
            using (var file = new StreamWriter(filePathSelected))
            {
                Probe.ExportMeasures(file, _rawRepeatMeasure, Probe.DeviceID);
                file.WriteLine("");
                Probe.State = new DeviceState(DeviceStatus.Ready);
            }
            _runningExport.Stop();
            var message = $"Execution time of export : { TimeSpan.FromSeconds(_runningExport.Elapsed.TotalSeconds).ToString("mm'm 'ss's 'fff'ms'")}";
            Application.Current?.Dispatcher.BeginInvoke(new Action(() => _dialogService.ShowMessageBox(message, "Export", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None)));
        }

        public void Dispose()
        {
            if (!(Probe is null))
            {
                Probe.RawSignalUpdated -= ProbeRawSignalUpdated;
                Probe.ThicknessMeasureUpdated -= ProbeThicknessMeasureUpdated;
            }
        }

        #endregion Public Methods
    }
}
