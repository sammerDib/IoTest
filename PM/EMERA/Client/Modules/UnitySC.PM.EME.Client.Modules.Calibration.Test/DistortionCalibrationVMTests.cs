using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.EME.Client.Modules.Calibration.ViewModel;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.Calibration.Test
{
    [TestClass]
    public class DistortionCalibrationVMTests
    {
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
            _calibrationSupervisor = Mock.Of<ICalibrationService>();
            _mockDialogOwnerService = new Mock<IDialogOwnerService>();
        }
        [TestMethod]
        public void CanLeave_ShouldReturnTrue_WhenHasNotChanged()
        {
            // Arrange
            var distortionCalibrationVM = new DistortionCalibrationVM(_calibrationSupervisor, _mockDialogOwnerService.Object);
            distortionCalibrationVM.HasChanged = false;

            // Act
            bool result = distortionCalibrationVM.CanLeave(null);

            // Assert
            Assert.IsTrue(result, "Expected CanLeave to return true when HasChanged is false.");
        }

        [TestMethod]
        public void CanLeave_ShouldReturnFalse_WhenHasChangedAndUserClicksNo()
        {
            // Arrange
            var distortionCalibrationVM = new DistortionCalibrationVM(_calibrationSupervisor, _mockDialogOwnerService.Object);
            distortionCalibrationVM.HasChanged = true;

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
            bool result = distortionCalibrationVM.CanLeave(null);

            // Assert
            Assert.IsFalse(result, "Expected CanLeave to return false when HasChanged is true, and user declined to leave by clicking No.");
        }       
        [TestMethod]
        public void CanLeave_ShouldReturnTrue_WhenForceCloseIsTrue()
        {
            // Arrange
            var distortionCalibrationVM = new DistortionCalibrationVM(_calibrationSupervisor, _mockDialogOwnerService.Object);  // Arrange
            // Act
            bool result = distortionCalibrationVM.CanLeave(null, forceClose: true);
            // Assert
            Assert.IsTrue(result, "Expected CanLeave to return true when forceClose is true, regardless of HasChanged.");
        }
    }
}
