using System.Collections.Generic;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.EME.Client.Modules.Calibration.ViewModel;
using UnitySC.PM.EME.Client.Proxy.Chuck;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Client.TestUtils;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.Calibration.Test
{
    [TestClass]
    public class ChuckManagerCalibrationVMTests
    {
        private FilterWheelBench _filterWheelBench;
        private CalibrationConfiguration _configuration;
        private ICalibrationService _calibrationSupervisor;
        private Mock<IDialogOwnerService> _mockDialogOwnerService;
        [TestInitialize]
        public void Setup()
        {
            ClassLocator.ExternalInit(new Container(), true);
            ClassLocator.Default.Register<INavigationManager, NavigationManagerForCalibration>(true);
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
            var messenger = new WeakReferenceMessenger();
            ClassLocator.Default.Register<IMessenger>(() => messenger);            

            var calibrationSupervisor = new FakeCalibrationSupervisor();
            var referentialSupervisor = new FakeReferentialSupervisor();
            var motionAxesSupersivor = new FakeMotionAxesSupervisor(messenger);
            var waferCategories = new List<WaferCategory>()
            {
               new WaferCategory(){Id = 3, Name= "Wafer200", DimentionalCharacteristic = new UnitySC.Shared.Data.WaferDimensionalCharacteristic(){Diameter = 200.Millimeters() } }
            };
            var chuckSupervisor = new FakeChuckSupervisor(messenger, waferCategories);
           var chuckVM = new ChuckVM(chuckSupervisor, calibrationSupervisor, referentialSupervisor, messenger);
           ClassLocator.Default.Register<ChuckVM>(() => chuckVM);

            _configuration = new CalibrationConfiguration() { TargetPixelSize = new Length(2, LengthUnit.Micrometer) };
            _calibrationSupervisor = Mock.Of<ICalibrationService>();
            _mockDialogOwnerService = new Mock<IDialogOwnerService>();
            _filterWheelBench = new FilterWheelBench(new FakeFilterWheelSupervisor(), new FakeCalibrationSupervisor(), null);
        }       
        [TestMethod]
        public void CanLeave_ShouldReturnTrue_WhenHasNotChanged()
        {
            // Arrange            
             var multiSizeChuckManagerVM = new ChuckManagerCalibrationVM(_calibrationSupervisor, _filterWheelBench, _mockDialogOwnerService.Object);
            multiSizeChuckManagerVM.HasChanged = false;

            // Act
            bool result = multiSizeChuckManagerVM.CanLeave(null);

            // Assert
            Assert.IsTrue(result, "Expected CanLeave to return true when HasChanged is false.");
        }

        [TestMethod]
        public void CanLeave_ShouldReturnFalse_WhenHasChangedAndUserClicksNo()
        {
            // Arrange
             var multiSizeChuckManagerVM = new ChuckManagerCalibrationVM(_calibrationSupervisor, _filterWheelBench, _mockDialogOwnerService.Object);
            multiSizeChuckManagerVM.HasChanged = true;

            const string expectedMessage = "You have unsaved changes. Do you really want to leave?";
            const string expectedTitle = "Confirmation";

            _mockDialogOwnerService
                .Setup(d => d.ShowMessageBox(
                    expectedMessage,
                    expectedTitle,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No))
                .Returns(MessageBoxResult.No);

            // Act
            bool result = multiSizeChuckManagerVM.CanLeave(null);

            // Assert
            Assert.IsFalse(result, "Expected CanLeave to return false when HasChanged is true, and user declined to leave by clicking No.");
        }
        [TestMethod]
        public void CanLeave_ShouldReturnTrue_WhenForceCloseIsTrue()
        {
            // Arrange
             var multiSizeChuckManagerVM = new ChuckManagerCalibrationVM(_calibrationSupervisor, _filterWheelBench, _mockDialogOwnerService.Object);
            // Act
            bool result = multiSizeChuckManagerVM.CanLeave(null, forceClose: true);
            // Assert
            Assert.IsTrue(result, "Expected CanLeave to return true when forceClose is true, regardless of HasChanged.");
        }
    }
}
