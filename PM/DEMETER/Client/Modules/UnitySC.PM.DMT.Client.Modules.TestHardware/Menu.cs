using System;
using System.Collections.Generic;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Client.Proxy.Chamber;
using UnitySC.PM.DMT.Client.Proxy.Chuck;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.PM.Shared.Hardware.ClientProxy.Axes;
using UnitySC.PM.Shared.Hardware.ClientProxy.Ffu;
using UnitySC.PM.Shared.Hardware.ClientProxy.Plc;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware
{
    public class Menu : IMenuItem
    {
        private readonly CameraSupervisor _cameraSupervisor;
        private readonly ScreenSupervisor _screenSupervisor;
        private readonly AlgorithmsSupervisor _algorithmsSupervisor;
        private readonly IDialogOwnerService _dialogService;
        private readonly ChuckSupervisor _chuckSupervisor;
        private readonly ChamberSupervisor _chamberSupervisor;
        private readonly PlcSupervisor _plcSupervisor;
        private readonly FfuSupervisor _ffuSupervisor;
        private readonly MotionAxesSupervisor _motionAxesSupervisor;
        private readonly IMessenger _messenger;

        private IMenuContentViewModel _viewModel;
        private UserControl _userControl;
        public string Name => "Test hardware";

        public string Description => "Use to test hardware one by one";

        public string Group => "Test";

        public string ImageResourceKey => "HardwareGeometry";

        public Menu()
        {
            _cameraSupervisor = ClassLocator.Default.GetInstance<CameraSupervisor>();
            _screenSupervisor = ClassLocator.Default.GetInstance<ScreenSupervisor>();
            _algorithmsSupervisor = ClassLocator.Default.GetInstance<AlgorithmsSupervisor>();
            _chuckSupervisor = ClassLocator.Default.GetInstance<ChuckSupervisor>();
            _chamberSupervisor = ClassLocator.Default.GetInstance<ChamberSupervisor>();
            _plcSupervisor = ClassLocator.Default.GetInstance<PlcSupervisor>();
            _motionAxesSupervisor = ClassLocator.Default.GetInstance<MotionAxesSupervisor>();
            _ffuSupervisor = ClassLocator.Default.GetInstance<FfuSupervisor>();
            _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
        }

        public IMenuContentViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = new TestHardwareViewModel(_cameraSupervisor, _screenSupervisor, _algorithmsSupervisor, _chamberSupervisor, _plcSupervisor, _chuckSupervisor, _motionAxesSupervisor, _ffuSupervisor, _dialogService, _messenger);
                }
                return _viewModel;
            }
        }

        public System.Windows.Controls.UserControl UserControl
        {
            get
            {
                if (_userControl == null)
                {
                    _userControl = new TestHardwareView();
                    _userControl.DataContext = ViewModel;
                }
                return _userControl;
            }
        }

        public int Priority => 220;

        public IEnumerable<ApplicationMode> CompatibleWith => new List<ApplicationMode>() { ApplicationMode.Maintenance };

        public IEnumerable<UserRights> RequiredRights => new List<UserRights>() { UserRights.HardwareManagement };

        public void ApplicationModeChange(ApplicationMode newMode)
        {
            // Nothing
        }

        public bool CanClose()
        {
            if (_viewModel == null)
                return true;
            bool canClose = _viewModel.CanClose();

            if (canClose && _viewModel is IDisposable)
                (_viewModel as IDisposable).Dispose();
            return canClose;
        }
    }
}
