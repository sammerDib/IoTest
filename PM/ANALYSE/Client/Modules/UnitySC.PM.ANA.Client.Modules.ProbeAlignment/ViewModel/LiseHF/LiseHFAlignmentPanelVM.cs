using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.ANA.Client.Modules.ProbeAlignment.ViewModel.LiseHF
{
    public class LiseHFAlignmentPanelVM : ObservableObject, IWizardNavigationItem, IDisposable, INavigable
    {
        #region Fields

        private ProbeAlignmentNavigationManager _navigationManager;
        private ProbesSupervisor _probeSupervisor = ClassLocator.Default.GetInstance<ProbesSupervisor>();
        private ProbeLiseHFVM _probeBase;

        #endregion

        #region Properties

        public ProbeAlignmentNavigationManager NavigationManager
        {
            get => _navigationManager ?? (_navigationManager = new ProbeAlignmentNavigationManager());
        }

        public string Name { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool IsMeasure { get; set; } = true;

        public bool IsValidated { get; set; } = false;

        public List<SpecificPositions> AvailablePositions
        {
            get => new List<SpecificPositions> { SpecificPositions.PositionWaferCenter };
        }

        public SpecificPositions DefaultSpecificPosition
        {
            get => SpecificPositions.PositionWaferCenter;
        }

        #endregion

        #region Constructors

        public LiseHFAlignmentPanelVM(ProbeLiseHFVM probeBase)
        {
            _probeBase = probeBase;
            Name = probeBase.Name;
            var settingsPage = new LiseHFSettingsVM(probeBase);
            var xyAnalysisPage = new LiseHFXYAnalysisVM(probeBase);
            var beamProfilerPage = new LiseHFBeamProfilerVM(probeBase);
            var spectrumCharactPage = new LiseHFSpectrumCharactVM(probeBase);
            NavigationManager.AllPages.Add(settingsPage);
            NavigationManager.AllPages.Add(xyAnalysisPage);
            NavigationManager.AllPages.Add(beamProfilerPage);
            NavigationManager.AllPages.Add(spectrumCharactPage);
            settingsPage.SettingsUpdated += (sender, args) =>
            {
                HandleSettingsChanged(xyAnalysisPage, beamProfilerPage, spectrumCharactPage);
            };
            probeBase.StartContinuousAcquisition();
        }

        private void HandleSettingsChanged(LiseHFXYAnalysisVM liseHfxyAnalysisPage, LiseHFBeamProfilerVM liseHfBeamProfilerPage,
            LiseHFSpectrumCharactVM liseHfSpectrumCharactPage)
        {
            //TODO: Compute new configuration
            object config = null;
            liseHfxyAnalysisPage.UpdateConfig(config);
            liseHfBeamProfilerPage.UpdateConfig(config);
            liseHfSpectrumCharactPage.UpdateConfig(config);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
        }

        public Task PrepareToDisplay()
        {
            _probeSupervisor.CurrentProbe = _probeBase;
            NavigationManager.NavigateToPage(NavigationManager.GetFirstPage());
            OnPropertyChanged(string.Empty);
            return Task.CompletedTask;
        }

        public bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            bool canLeave = true;
            NavigationManager.AllPages.Select(navigable => canLeave &= navigable.CanLeave(nextPage, forceClose));
            Dispose();
            return canLeave;
        }

        #endregion
    }
}
