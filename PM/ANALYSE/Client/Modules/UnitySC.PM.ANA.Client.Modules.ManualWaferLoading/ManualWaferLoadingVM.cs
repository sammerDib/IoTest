using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.PM.ANA.Client.Modules.ManualWaferLoading.ViewModel;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.ManualWaferLoading
{
    public class ManualWaferLoadingVM : ObservableObject, IMenuContentViewModel
    {
        public bool IsEnabled => true;
        private bool _canCloseState = true;
        private CamerasSupervisor _camerasSupervisor;
        private AxesSupervisor _axesSupervisor;        

        public ManualWaferLoadingVM()
        {
            Load = new LoadVM(this);
            Bwa = new BwaVM();

            _camerasSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();            
            _axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
        }

        public LoadVM Load { get; private set; }
        public BwaVM Bwa { get; private set; }

        public List<SpecificPositions> AvailablePositions
        {
            get => new List<SpecificPositions>
            {
                SpecificPositions.PositionChuckCenter,
                SpecificPositions.PositionHome,
                SpecificPositions.PositionManualLoad,
                SpecificPositions.PositionPark
            };
        }

        public SpecificPositions DefaultSpecificPosition
        {
            get => SpecificPositions.PositionChuckCenter;
        }

        public bool CanCloseState
        {
            get => _canCloseState; set { if (_canCloseState != value) { _canCloseState = value; OnPropertyChanged(); } }
        }

        public void Refresh()
        {
            Init();
        }

        public bool CanClose()
        {
            if (!CanCloseState)
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Can't close");
            else
            {
                _axesSupervisor.AxesVM.IsSpeedLimitedWhenUnclamped = false;
                _camerasSupervisor.StopAllStreaming();
            }

            return CanCloseState;
        }

        private void Init()
        {
            _axesSupervisor.AxesVM.IsSpeedLimitedWhenUnclamped = true;
            _camerasSupervisor.Camera = _camerasSupervisor.GetMainCamera();
            _camerasSupervisor.GetMainCamera()?.StartStreaming();
        }
    }
}
