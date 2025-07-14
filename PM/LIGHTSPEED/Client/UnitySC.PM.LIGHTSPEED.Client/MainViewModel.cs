using GalaSoft.MvvmLight;

using UnitySC.PM.LIGHTSPEED.Client.CommonUI.ViewModel.Maintenance;
using UnitySC.PM.LIGHTSPEED.Client.Proxy.Acquisition;
using UnitySC.PM.LIGHTSPEED.Client.Proxy.FeedbackLoop;
using UnitySC.PM.LIGHTSPEED.Client.Proxy.RotatorsKitCalibration;
using UnitySC.PM.LIGHTSPEED.Client.Proxy.LiseHF;
using UnitySC.PM.Shared.Hardware.ClientProxy.Global;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.LIGHTSPEED.Client
{
    public class MainViewModel : ViewModelBase
    {
        private GlobalDeviceVM _globalDeviceVM;

        public GlobalDeviceVM GlobalDeviceVM
        {
            get => _globalDeviceVM; set { if (_globalDeviceVM != value) { _globalDeviceVM = value; RaisePropertyChanged(); } }
        }

        private RotatorsKitCalibrationVM _rotatorsKitCalibrationVM;

        public RotatorsKitCalibrationVM RotatorsKitCalibrationVM
        {
            get => _rotatorsKitCalibrationVM;
            set { if (_rotatorsKitCalibrationVM != value) { _rotatorsKitCalibrationVM = value; RaisePropertyChanged(); } }
        }

        private LiseHFVM _liseHFVM;

        public LiseHFVM LiseHFVM
        {
            get => _liseHFVM;
            set { if (_liseHFVM != value) { _liseHFVM = value; RaisePropertyChanged(); } }
        }

        private FeedbackLoopVM _feedbackLoopVM;

        public FeedbackLoopVM FeedbackLoopVM
        {
            get => _feedbackLoopVM;
            set { if (_feedbackLoopVM != value) { _feedbackLoopVM = value; RaisePropertyChanged(); } }
        }

        private DoorSlitVM _doorSlitVM;

        public DoorSlitVM DoorSlitVM
        {
            get => _doorSlitVM;
            set { if (_doorSlitVM != value) { _doorSlitVM = value; RaisePropertyChanged(); } }
        }

        public void Init()
        {
            GlobalDeviceVM = ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalDeviceSupervisor(ActorType.LIGHTSPEED).GlobalDeviceVM;
        }

        private int _selectedTab;

        public int SelectedTab
        {
            get { return _selectedTab; }
            set
            {
                _selectedTab = value;
                SelectedTabChange(_selectedTab);
                RaisePropertyChanged();
            }
        }

        private void SelectedTabChange(int selected)
        {
            switch (selected)
            {
                case 0:
                    GlobalDeviceVM = ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalDeviceSupervisor(ActorType.LIGHTSPEED).GlobalDeviceVM;
                    break;

                case 1:
                    DoorSlitVM = new DoorSlitVM(ClassLocator.Default.GetInstance<DoorSlitSupervisor>());
                    break;

                case 2:
                    FeedbackLoopVM = new FeedbackLoopVM(ClassLocator.Default.GetInstance<FeedbackLoopSupervisor>());
                    break;

                case 3:
                    RotatorsKitCalibrationVM = new RotatorsKitCalibrationVM(ClassLocator.Default.GetInstance<RotatorsKitCalibrationSupervisor>());
                    break;

                case 4:
                    LiseHFVM = new LiseHFVM(ClassLocator.Default.GetInstance<LiseHFSupervisor>());
                    break;
            }
        }
    }
}
