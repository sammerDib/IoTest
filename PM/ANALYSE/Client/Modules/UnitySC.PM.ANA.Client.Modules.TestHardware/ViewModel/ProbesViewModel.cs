using System.Collections.ObjectModel;
using System.Linq;

using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.UI.ViewModels;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel
{
    public class ProbesViewModel : TabViewModelBase
    {
        private ProbesSupervisor _probesSupervisor;
        private ProbeLiseViewModelBase _selectedProbeVM;
        private AutoRelayCommand _doMeasure;

        public ProbesViewModel()
        {
            Title = "Probes";
        }

        private ObservableCollection<ProbeLiseViewModelBase> _probes;

        public ObservableCollection<ProbeLiseViewModelBase> Probes
        {
            get
            {
                if (_probes == null)
                {
                    _probes = new ObservableCollection<ProbeLiseViewModelBase>();
                    var probesConfig = ProbesSupervisor.GetProbesConfig()?.Result;
                    if (probesConfig != null)
                    {
                        foreach (var config in probesConfig)
                        {
                            var newProbeViewModel = ProbeViewModelsFactory(config);
                            if (newProbeViewModel != null)
                                _probes.Add(newProbeViewModel);
                        }
                    }

                    // We inject the probes list into the supervisor
                    //ProbesSupervisor.Probes = _probes.ToList();
                }

                return _probes;
            }
            set
            {
                if (_probes == value)
                {
                    return;
                }

                _probes = value;
                OnPropertyChanged(nameof(Probes));
            }
        }

        // Factory used to create the viewModels corresponding to the probe configuration
        private ProbeLiseViewModelBase ProbeViewModelsFactory(IProbeConfig probeConfig)
        {
            switch (probeConfig)
            {
                case ProbeLiseConfig _:
                    {
                        var newProbe = new ProbeLiseViewModel(ProbesSupervisor, probeConfig.DeviceID);
                        return newProbe;
                    }

                case ProbeDualLiseConfig _:
                    {
                        var newProbe = new ProbeLiseDoubleViewModel(ProbesSupervisor, probeConfig.DeviceID);
                        return newProbe;
                    }
                // WHY THERE IS NO PROBE LISE HF ?
                default:
                    return null;
            }
        }

        public ProbeLiseViewModelBase SelectedProbe
        {
            get
            {
                if (_selectedProbeVM == null)
                    _selectedProbeVM = _probes.FirstOrDefault();
                return _selectedProbeVM;
            }
            set
            {
                if (_selectedProbeVM == value)
                {
                    return;
                }

                if (SelectedProbe.Probe.State.Status == DeviceStatus.Busy)
                {
                    SelectedProbe.Probe.StopContinuousAcquisition();
                }
                _selectedProbeVM = value;
                OnPropertyChanged(nameof(SelectedProbe));
            }
        }

        public ProbesSupervisor ProbesSupervisor
        {
            get
            {
                if (_probesSupervisor == null)
                {
                    _probesSupervisor = ClassLocator.Default.GetInstance<ProbesSupervisor>();
                }

                return _probesSupervisor;
            }
        }

        public SingleLiseInputParams ProbeInputParametersLise { get; set; }

        public AutoRelayCommand DoMeasure
        {
            get
            {
                return _doMeasure ?? (_doMeasure = new AutoRelayCommand(
                () =>
                {
                    SelectedProbe.Probe.DoMeasure(ProbeInputParametersLise);
                },
                () => true));
            }
        }
    }
}
