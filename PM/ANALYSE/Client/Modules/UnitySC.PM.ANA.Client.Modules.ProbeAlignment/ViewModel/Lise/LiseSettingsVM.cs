using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.Shared.Hardware.ClientProxy.Axes;
using UnitySC.PM.Shared.Hardware.ClientProxy.Axes.ViewModel;
using UnitySC.PM.Shared.Hardware.ClientProxy.Laser;
using UnitySC.PM.Shared.Hardware.ClientProxy.Shutter;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Modules.ProbeAlignment.ViewModel.Lise
{
    public class LiseSettingsVM : ObservableObject, IDisposable
    {
        #region Fields

        private string _probeRef = "";
        private string _sliderSelectedPosition = "null";
        private bool _isSliderMoving = false;
        private LaserSupervisor _laserSupervisor;
        private ShutterSupervisor _shutterSupervisor;
        private MotionAxesSupervisor _motionAxesSupervisor;
        private ProbeBaseVM _probeLiseHFVM;

        #endregion

        #region Properties

        public string ProbeRef
        {
            get => _probeRef;
            set => SetProperty(ref _probeRef, value);
        }

        public string SliderSelectedPosition
        {
            get => _sliderSelectedPosition;
            set
            {
                MoveToPosition(value);
                SetProperty(ref _sliderSelectedPosition, value);
            }
        }

        public bool IsSliderMoving
        {
            get => _isSliderMoving;
            set => SetProperty(ref _isSliderMoving, value);
        }

        private LaserSupervisor LaserSupervisor
        {
            get => _laserSupervisor ??
                   (_laserSupervisor = ClassLocator.Default.GetInstance<LaserSupervisor>());
        }

        private MotionAxesSupervisor MotionAxesSupervisor
        {
            get => _motionAxesSupervisor ??
                   (_motionAxesSupervisor = ClassLocator.Default.GetInstance<MotionAxesSupervisor>());
        }

        private ShutterSupervisor ShutterSupervisor
        {
            get => _shutterSupervisor ??
                   (_shutterSupervisor = ClassLocator.Default.GetInstance<ShutterSupervisor>());
        }

        public LaserVM LaserVM => LaserSupervisor.LaserVM;
        public MotionAxesVM MotionAxesVM => MotionAxesSupervisor.MotionAxesVM;
        public ShutterVM ShutterVM => ShutterSupervisor.ShutterVM;

        #endregion

        #region Constructors

        public LiseSettingsVM(ProbeBaseVM probeLiseHFVM)
        {
            _probeLiseHFVM = probeLiseHFVM as ProbeLiseHFVM;
            ProbeRef = probeLiseHFVM.Name;
            Init();
        }

        #endregion

        #region Methods

        private void Init()
        {
            Task.Run(() => MotionAxesSupervisor.TriggerUpdateEvent());
            Task.Run(() => LaserSupervisor.TriggerUpdateEvent());
            Task.Run(() => ShutterSupervisor.TriggerUpdateEvent());
        }

        public void Dispose()
        {
        }

        private async Task MoveToPosition(string lensName)
        {
            IsSliderMoving = true;
            var thorlabsSliderAxisConfig = MotionAxesVM.ConfigurationAxisLinear as ThorlabsSliderAxisConfig;
            int index = thorlabsSliderAxisConfig.NameLenses.IndexOf(lensName);
            var pos = new Length(index, LengthUnit.Millimeter);
            var newPosition = new PMAxisMove("Linear", pos);
            MotionAxesVM.MoveIncremental(newPosition);
            IsSliderMoving = false;
        }

        #endregion
    }
}
