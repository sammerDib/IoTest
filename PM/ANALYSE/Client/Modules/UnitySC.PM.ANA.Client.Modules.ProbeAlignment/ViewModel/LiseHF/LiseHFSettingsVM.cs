using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.Shared.Hardware.ClientProxy.Axes;
using UnitySC.PM.Shared.Hardware.ClientProxy.Axes.ViewModel;
using UnitySC.PM.Shared.Hardware.ClientProxy.Laser;
using UnitySC.PM.Shared.Hardware.ClientProxy.Shutter;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.ANA.Client.Modules.ProbeAlignment.ViewModel.LiseHF
{
    public class LiseHFSettingsVM : ObservableObject, IDisposable, INavigable, IWizardNavigationItem
    {
        #region Fields

        private string _probeRef = "";
        private string _sliderSelectedPosition = "null";
        private bool _isSliderMoving = false;
        private LaserSupervisor _laserSupervisor;
        private ShutterSupervisor _shutterSupervisor;
        private MotionAxesSupervisor _motionAxesSupervisor;
        private ProbeLiseHFVM _probeLiseHFVM;
        private string _name = "LiseSettings";
        private bool _isEnabled = true;
        private bool _isMeasure = false;
        private bool _isValidated = false;
        private double _qualityThreshold = 0.1;
        private double _tsvDiameter = 5;
        private double _darkPosX;
        private double _darkPosY;
        private double _darkPosZ;
        private AsyncRelayCommand _getDarkCommand;
        private double _refPosX;
        private double _refPosY;
        private double _refPosZ;
        private AsyncRelayCommand _getRefCommand;

        public event EventHandler SettingsUpdated;

        #endregion

        #region Properties

        public string ProbeRef
        {
            get => _probeRef;
            set => SetProperty(ref _probeRef, value);
        }

        public ProbeLiseHFVM Probe
        {
            get => _probeLiseHFVM;
        }

        public ProbeInputParametersLiseHFVM ProbeInputs
        {
            get => Probe.InputParametersLiseHF;
        }

        public string SliderSelectedPosition
        {
            get => _sliderSelectedPosition;
            set
            {
                if (value != _sliderSelectedPosition)
                {
                    MoveToPosition(value);
                }
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
            set => throw new NotImplementedException();
        }

        private MotionAxesSupervisor MotionAxesSupervisor
        {
            get => _motionAxesSupervisor ??
                   (_motionAxesSupervisor = ClassLocator.Default.GetInstance<MotionAxesSupervisor>());
            set => throw new NotImplementedException();
        }

        private ShutterSupervisor ShutterSupervisor
        {
            get => _shutterSupervisor ??
                   (_shutterSupervisor = ClassLocator.Default.GetInstance<ShutterSupervisor>());
            set => throw new NotImplementedException();
        }

        public LaserVM LaserVM
        {
            get => LaserSupervisor.LaserVM;
        }

        public MotionAxesVM MotionAxesVM
        {
            get => MotionAxesSupervisor.MotionAxesVM;
        }

        public ShutterVM ShutterVM
        {
            get => ShutterSupervisor.ShutterVM;
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public bool IsMeasure
        {
            get => _isMeasure;
            set => SetProperty(ref _isMeasure, value);
        }

        public bool IsValidated
        {
            get => _isValidated;
            set => SetProperty(ref _isValidated, value);
        }

        public double QualityThreshold
        {
            get => _qualityThreshold;
            set
            {
                SetProperty(ref _qualityThreshold, value);
                SettingsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public double TSVDiameter
        {
            get => _tsvDiameter;
            set
            {
                SetProperty(ref _tsvDiameter, value);
                SettingsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public double DarkPosX
        {
            get => _darkPosX;
            set
            {
                SetProperty(ref _darkPosX, value);
                SettingsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public double DarkPosY
        {
            get => _darkPosY;
            set
            {
                SetProperty(ref _darkPosY, value);
                SettingsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public double DarkPosZ
        {
            get => _darkPosZ;
            set
            {
                SetProperty(ref _darkPosZ, value);
                SettingsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public AsyncRelayCommand GetDarkAndReferenceCommand
        {
            get => _getDarkCommand ?? (_getDarkCommand = new AsyncRelayCommand(GetDarkAndRef));
        }

        public double RefPosX
        {
            get => _refPosX;
            set
            {
                SetProperty(ref _refPosX, value);
                SettingsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public double RefPosY
        {
            get => _refPosY;
            set
            {
                SetProperty(ref _refPosY, value);
                SettingsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public double RefPosZ
        {
            get => _refPosZ;
            set
            {
                SetProperty(ref _refPosZ, value);
                SettingsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Constructors

        public LiseHFSettingsVM(ProbeLiseHFVM probeLiseHFVM)
        {
            _probeLiseHFVM = probeLiseHFVM;
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


        public Task PrepareToDisplay()
        {
            Init();
            return Task.CompletedTask;
        }

        public bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            Dispose();
            return true;
        }

        private async Task GetDarkAndRef()
        {
            //TODO
        }

        #endregion
    }
}
