using System;
using System.Linq;
using System.Threading.Tasks;

using UnitySC.Shared.UI.AutoRelayCommandExt;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.Shared.Hardware.ClientProxy.Axes;
using UnitySC.PM.Shared.Hardware.ClientProxy.Axes.ViewModel;
using UnitySC.PM.Shared.Hardware.ClientProxy.Laser;
using UnitySC.PM.Shared.Hardware.ClientProxy.Shutter;
using UnitySC.PM.Shared.Hardware.ClientProxy.Spectrometer;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel
{
    public class ProbeLiseHFViewModel : ProbeLiseViewModelBase
    {
        //
        // TO DO UTILISER LA PROBE LISE HF
        //

        private LaserSupervisor _laserSupervisor;
        private ShutterSupervisor _shutterSupervisor;
        private MotionAxesSupervisor _motionAxesSupervisor;
        private SpectrometerSupervisor _spectroSupervisor;
        private AutoRelayCommand _doMeasure;

        private string _selectedLensName;

        public LaserVM LaserVM => LaserSupervisor.LaserVM;
        public ShutterVM ShutterVM => ShutterSupervisor.ShutterVM;
        public MotionAxesVM MotionAxesVM => MotionAxesSupervisor.MotionAxesVM;
        public SpectrometerVM SpectroVM => SpectrometerSupervisor.SpectroVM;

        private ThorlabsSliderAxisConfig _thorlabsSliderAxisConfig;

        private int _nbAverage = 8;

        public int NbAverage
        {
            get { return _nbAverage; }
            set { if (_nbAverage != value) { _nbAverage = value; SpectroVM.NbAverage = value; OnPropertyChanged(); }; }
        }

        private double _integrationTimeMs = 10.0;

        public double IntegrationTimeMs
        {
            get { return _integrationTimeMs; }
            set { if (_integrationTimeMs != value) { _integrationTimeMs = value; SpectroVM.IntegrationTimeMs = value; OnPropertyChanged(); }; }
        }

        private bool _isLiseHFAvailable = true;

        public bool IsLiseHFAvailable
        {
            get { return _isLiseHFAvailable; }
            set
            {
                if (_isLiseHFAvailable != value)
                {
                    _isLiseHFAvailable = value;
                    OnPropertyChanged();
                }
            }
        }

        public ProbeLiseHFViewModel()
        {
            var probeSupervisor = ClassLocator.Default.GetInstance<ProbesSupervisor>();
            if (!probeSupervisor.Probes.OfType<ProbeLiseHFVM>().Any()
               || probeSupervisor.Probes.OfType<ProbeLiseHFVM>().Any(p => !p.Configuration.IsEnabled))
            {
                IsLiseHFAvailable = false;
                return;
            }
            _thorlabsSliderAxisConfig = (ThorlabsSliderAxisConfig)MotionAxesVM.ConfigurationAxisLinear;

            Init();
        }

        public void Init()
        {
            Task.Run(() => MotionAxesSupervisor.TriggerUpdateEvent());
            Task.Run(() => LaserSupervisor.TriggerUpdateEvent());
            Task.Run(() => ShutterSupervisor.TriggerUpdateEvent());
        }

        public LaserSupervisor LaserSupervisor
        {
            get
            {
                if (_laserSupervisor == null)
                {
                    _laserSupervisor = ClassLocator.Default.GetInstance<LaserSupervisor>();
                }

                return _laserSupervisor;
            }
        }

        public ShutterSupervisor ShutterSupervisor
        {
            get
            {
                if (_shutterSupervisor == null)
                {
                    _shutterSupervisor = ClassLocator.Default.GetInstance<ShutterSupervisor>();
                }

                return _shutterSupervisor;
            }
        }

        public MotionAxesSupervisor MotionAxesSupervisor
        {
            get
            {
                if (_motionAxesSupervisor == null)
                {
                    _motionAxesSupervisor = ClassLocator.Default.GetInstance<MotionAxesSupervisor>();
                }

                return _motionAxesSupervisor;
            }
        }

        public SpectrometerSupervisor SpectrometerSupervisor
        {
            get
            {
                if (_spectroSupervisor == null)
                {
                    _spectroSupervisor = ClassLocator.Default.GetInstance<SpectrometerSupervisor>();
                }

                return _spectroSupervisor;
            }
        }

        public string SelectedLensName
        {
            get { return _selectedLensName; }
            set
            {
                _selectedLensName = value;

                int index = _thorlabsSliderAxisConfig.NameLenses.IndexOf(_selectedLensName);
                var pos = new Length(index, LengthUnit.Millimeter);
                var newPosition = new PMAxisMove("Linear", pos);
                MotionAxesVM.MoveIncremental(newPosition);

                OnPropertyChanged();
            }
        }

        public AutoRelayCommand DoMeasure
        {
            get
            {
                return _doMeasure ?? (_doMeasure = new AutoRelayCommand(
                async () =>
                {
                    try
                    {
                        await Task.Run(async () => { ShutterSupervisor.OpenShutterCommand(); await Task.Delay(500); });
                        await Task.Run(() => SpectrometerSupervisor.DoMeasure(new SpectrometerParamBase(IntegrationTimeMs, NbAverage)));
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        await Task.Run(() => ShutterSupervisor.CloseShutterCommand());
                    }
                }));
            }
        }
    }
}
