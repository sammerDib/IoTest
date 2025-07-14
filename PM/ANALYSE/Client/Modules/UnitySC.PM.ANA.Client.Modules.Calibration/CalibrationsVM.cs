using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.Navigation;
using UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel;
using UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.LiseHF;
using UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.Objective;
using UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.XY;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Calibration;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.ANA.Client.Modules.Calibration
{
    public class CalibrationsVM : ObservableObject, IMenuContentViewModel
    {
        public bool IsEnabled => true;

        private INavigationManager _navigationManager;

        public INavigationManager NavigationManager
        {
            get
            {
                if (_navigationManager == null)
                    _navigationManager = new CalibrationNavigationManager();
                return _navigationManager;
            }
            set { _navigationManager = value; }
        }

        private CalibrationSupervisor _calibrationSupervisor;
        private ProbesSupervisor _probesSupervisor;
        private IMessenger _messenger;

        public CalibrationsVM()
        {
            _calibrationSupervisor = ClassLocator.Default.GetInstance<CalibrationSupervisor>();
            _probesSupervisor = ClassLocator.Default.GetInstance<ProbesSupervisor>();
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
        }

        private void ConnexionStatusChanged(ConnexionStateForActor connexionStatus)
        {
            NavigationManager.CurrentPage.CanLeave(null, true);
        }

        public bool CanClose()
        {
            if (NavigationManager.CurrentPage.CanLeave(null))
            {
                _messenger.Unregister<ConnexionStateForActor>(this);
                ServiceLocator.CamerasSupervisor.StopAllStreaming();
                NavigationManager.AllPages.ToList().ForEach(x => { if (x is IDisposable disposablePage) disposablePage.Dispose(); });
                NavigationManager.AllPages.Clear();

                return true;
            }

            return false;
        }

        public void Refresh()
        {
            NavigationManager.AllPages.Add(new ObjectivesCalibrationVM());

            NavigationManager.AllPages.Add(new XYCalibrationVM(ClassLocator.Default.GetInstance<ChuckSupervisor>().ChuckVM.SelectedWaferCategory.DimentionalCharacteristic));

            if (_probesSupervisor.Probes.FirstOrDefault(p => p is ProbeLiseHFVM) != null)
                NavigationManager.AllPages.Add(new LiseHFCalibrationVM());

            NavigationManager.CurrentPage = NavigationManager.AllPages.First();

            foreach (CalibrationVMBase calib in NavigationManager.AllPages)
            {
                calib.Init();
            }
            _messenger.Register<ConnexionStateForActor>(this, (r, m) => ConnexionStatusChanged(m));
        }
    }
}
