using System;
using System.ComponentModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.ANA.Client.Modules.ProbeAlignment.ViewModel.Lise
{
    public class LiseAlignmentPanelVM : ObservableObject, IWizardNavigationItem, IDisposable, INavigable
    {
        #region Fields

        private ProbesSupervisor _probesSupervisor;
        private LiseSettingsVM _liseSettings;
        private LiseXYAnalysisVM _liseXyAnalysis;
        private LiseBeamProfilerVM _liseBeamProfiler;
        private LiseSpectrumCharactVM _liseSpectrumCharact;
        private bool _isMeasurementRunning = false;
        private ProbesSupervisor _probeSupervisor = ClassLocator.Default.GetInstance<ProbesSupervisor>();

        #endregion

        #region Properties

        public LiseSettingsVM LiseSettings
        {
            get => _liseSettings;
            set => SetProperty(ref _liseSettings, value);
        }

        public LiseXYAnalysisVM LiseXyAnalysis
        {
            get => _liseXyAnalysis;
            set => SetProperty(ref _liseXyAnalysis, value);
        }

        public LiseBeamProfilerVM LiseBeamProfiler
        {
            get => _liseBeamProfiler;
            set => SetProperty(ref _liseBeamProfiler, value);
        }

        public LiseSpectrumCharactVM LiseSpectrumCharact
        {
            get => _liseSpectrumCharact;
            set => SetProperty(ref _liseSpectrumCharact, value);
        }


        public bool IsMeasurementRunning
        {
            get => _isMeasurementRunning;
            set => SetProperty(ref _isMeasurementRunning, value);
        }

        public string Name { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool IsMeasure { get; set; } = true;

        public bool IsValidated { get; set; } = false;

        #endregion

        #region Constructors

        public LiseAlignmentPanelVM(ProbeLiseVM probeBase)
        {
            Name = probeBase.Name;
            LiseSettings = new LiseSettingsVM(probeBase);
            LiseXyAnalysis = new LiseXYAnalysisVM(probeBase);
            LiseBeamProfiler = new LiseBeamProfilerVM(probeBase);
            LiseSpectrumCharact = new LiseSpectrumCharactVM(probeBase);
            LiseXyAnalysis.PropertyChanged += OnPropertyChangedEventHandler();
            LiseBeamProfiler.PropertyChanged += OnPropertyChangedEventHandler();
            LiseSpectrumCharact.PropertyChanged += OnPropertyChangedEventHandler();
        }

        #endregion

        #region Methods

        private PropertyChangedEventHandler OnPropertyChangedEventHandler()
        {
            return (s, e) =>
            {
                IsMeasurementRunning = LiseXyAnalysis.IsMeasurementRunning || LiseBeamProfiler.IsMeasurementRunning ||
                                       LiseSpectrumCharact.IsMeasurementRunning;
            };
        }

        public void Dispose()
        {
            LiseSettings.Dispose();
            LiseXyAnalysis.Dispose();
            LiseBeamProfiler.Dispose();
            LiseSpectrumCharact.Dispose();
        }

        public Task PrepareToDisplay()
        {
            return Task.CompletedTask;
        }

        public bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            Dispose();
            return true;
        }

        #endregion
    }
}
