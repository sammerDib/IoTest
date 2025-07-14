using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Controls.Camera;
using UnitySC.PM.EME.Client.Modules.Calibration.ViewModel;
using UnitySC.PM.EME.Client.Proxy.Axes;
using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Proxy.Dispatcher;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.Calibration
{
    public sealed class CalibrationsVM : ObservableRecipient, IMenuContentViewModel
    {
        private const string CalibrationConfigurationFileName = "CalibrationConfiguration.xml";
        private readonly CameraBench _cameraBench;
        private readonly FilterWheelBench _filterWheelBench;
        private readonly AxesVM _axes;
        public bool IsEnabled => true;

        public CalibrationsVM()
        {
            _cameraBench = ClassLocator.Default.GetInstance<CameraBench>();
            _filterWheelBench = ClassLocator.Default.GetInstance<FilterWheelBench>();
            _axes = ClassLocator.Default.GetInstance<AxesVM>();
            _navigationManager = ClassLocator.Default.GetInstance<INavigationManager>();            
        }

        private StandardCameraViewModel _standardCameraViewModel;
        public StandardCameraViewModel StandardCameraViewModel
        {
            get
            {
                _standardCameraViewModel = _standardCameraViewModel
                    ?? new StandardCameraViewModel(_cameraBench, ClassLocator.Default.GetInstance<IMessenger>());                
                return _standardCameraViewModel;
            }
        }

        private INavigationManager _navigationManager;
        public INavigationManager NavigationManager
        {
            get => _navigationManager; set => SetProperty(ref _navigationManager, value);
        }
        public bool CanClose()
        {
            if ((!(NavigationManager.CurrentPage is null)) && (!NavigationManager.CurrentPage.CanLeave(null)))
                return false;
            foreach (var calibrationVM in NavigationManager.AllPages)
            {
                (calibrationVM as CalibrationWizardStepBaseVM).Dispose();
            }
            StandardCameraViewModel?.Dispose();            
            EndCalibration();
            return true;
        }

        public void Refresh()
        {
            var clientConfigurationManager = ClassLocator.Default.GetInstance<ClientConfigurationManager>();
            string filePath = Path.Combine(clientConfigurationManager.ConfigurationFolderPath, CalibrationConfigurationFileName);
            var calibrationConfiguration = XML.Deserialize<CalibrationConfiguration>(filePath);
            var calibrationService = ClassLocator.Default.GetInstance<ICalibrationService>();
            var algoSupervisor = ClassLocator.Default.GetInstance<IAlgoSupervisor>();
            var dialogOwnerService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            var filterWheelBench = ClassLocator.Default.GetInstance<FilterWheelBench>();
            var dispatcher = ClassLocator.Default.GetInstance<IDispatcher>();
            NavigationManager.AllPages.Add(new ChuckParallelismCalibrationVM(_axes, calibrationService));
            NavigationManager.AllPages.Add(new FilterCalibrationVM(_filterWheelBench, calibrationService, algoSupervisor, dialogOwnerService, dispatcher));
            NavigationManager.AllPages.Add(new AxisOrthogonalityCalibrationVM(calibrationService, filterWheelBench));
            NavigationManager.AllPages.Add(new CameraMagnificationCalibrationVM(algoSupervisor, calibrationConfiguration, calibrationService, dialogOwnerService));
            NavigationManager.AllPages.Add(new ChuckManagerCalibrationVM(calibrationService, filterWheelBench, dialogOwnerService));
            NavigationManager.AllPages.Add(new DistanceSensorCalibrationVM(calibrationService, algoSupervisor, dialogOwnerService));
            NavigationManager.AllPages.Add(new DistortionCalibrationVM(calibrationService, dialogOwnerService));

            NavigationManager.NavigateToPage(NavigationManager.GetFirstPage());

            foreach (var calibrationVM in NavigationManager.AllPages)
            {
                (calibrationVM as CalibrationWizardStepBaseVM).Init();
            }
            StandardCameraViewModel.Refresh();          
        }
        public void EndCalibration()
        {
            NavigationManager.RemoveAllPages();
        }               
    }
}
