using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Modules.TestHardware.ViewModel;
using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Client.Proxy.Light;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.TestHardware
{
    public class TestHardwareVM : ObservableObject, IMenuContentViewModel
    {
        private TestCameraViewModel _testCameraViewModel;

        public TestCameraViewModel TestCameraViewModel
        {
            get
            {
                if (_testCameraViewModel == null)
                {
                    var messenger = ClassLocator.Default.GetInstance<IMessenger>();
                    var camera = ClassLocator.Default.GetInstance<CameraBench>();
                    _testCameraViewModel = new TestCameraViewModel(camera, messenger);
                }

                return _testCameraViewModel;
            }
        }

        private FilterWheelViewModel _filterWheelViewModel;

        public FilterWheelViewModel FilterWheelViewModel
        {
            get
            {
                if (_filterWheelViewModel == null)
                {
                    var filterWheelBench = ClassLocator.Default.GetInstance<FilterWheelBench>();
                    _filterWheelViewModel = new FilterWheelViewModel(filterWheelBench);
                }

                return _filterWheelViewModel;
            }
        }

        private PhotoLumMotionAxesViewModel _photoLumMotionAxesViewModel;

        public PhotoLumMotionAxesViewModel PhotoLumMotionAxesViewModel
        {
            get
            {
                _photoLumMotionAxesViewModel = _photoLumMotionAxesViewModel ?? new PhotoLumMotionAxesViewModel(ClassLocator.Default.GetInstance<IMessenger>());
                return _photoLumMotionAxesViewModel;
            }
        }

        private TestLightViewModel _testLightViewModel;

        public TestLightViewModel TestLightViewModel
        {
            get
            {
                var lightBench = ClassLocator.Default.GetInstance<LightBench>();
                var cameraBench = ClassLocator.Default.GetInstance<CameraBench>();
                var messenger = ClassLocator.Default.GetInstance<IMessenger>();
                _testLightViewModel = _testLightViewModel ?? new TestLightViewModel(lightBench, cameraBench, messenger);
                return _testLightViewModel;
            }
        }

        private OverviewChamberVM _overviewChamberVM;

        public OverviewChamberVM OverviewChamberVM
        {
            get
            {
                if (_overviewChamberVM == null)
                {
                    _overviewChamberVM = new OverviewChamberVM();
                }

                return _overviewChamberVM;
            }
        }

        public bool IsEnabled => true;

        private bool _canCloseState = true;

        public bool CanCloseState
        {
            get => _canCloseState;
            set
            {
                if (_canCloseState != value)
                {
                    _canCloseState = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool CanClose()
        {
            if (!CanCloseState)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Can't close");
                return false;
            }

            TestCameraViewModel.Close();
            TestLightViewModel?.Close();
            PhotoLumMotionAxesViewModel.IsActive = false;

            return CanCloseState;
        }

        public void Refresh()
        {
            TestCameraViewModel.Refresh();
            TestLightViewModel?.Refresh();
            PhotoLumMotionAxesViewModel.IsActive = true;
        }
    }
}
