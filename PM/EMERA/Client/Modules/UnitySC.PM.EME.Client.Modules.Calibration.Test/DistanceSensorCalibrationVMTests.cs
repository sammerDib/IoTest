using System.Windows;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.EME.Client.Modules.Calibration.ViewModel;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.Calibration.Test
{
    [TestClass]
    public class DistanceSensorCalibrationVMTests
    {
        private IAlgoSupervisor _fakeAlgoSupervisor;
        private ICalibrationService _calibrationSupervisor;
        private Mock<IDialogOwnerService> _mockDialogOwnerService;

        [TestInitialize]
        public void Setup()
        {
            ClassLocator.ExternalInit(new Container(), true);
            ClassLocator.Default.Register<INavigationManager, NavigationManagerForCalibration>(true);

            _fakeAlgoSupervisor = new FakeAlgoSupervisor();
            _calibrationSupervisor = Mock.Of<ICalibrationService>();
            _mockDialogOwnerService = new Mock<IDialogOwnerService>();
        }

        [TestMethod]
        public void ShouldReturnValueWhenExecuteCalibrationCommand()
        {
            // Given
            var systemUnderTest = new DistanceSensorCalibrationVM(_calibrationSupervisor, _fakeAlgoSupervisor, _mockDialogOwnerService.Object);

            // When
            systemUnderTest.StartDistanceSensorCalibration.Execute(null);

            // Then
            var expectedResult = new DistanceSensorCalibrationResult
            {
                OffsetX = 42.Millimeters(), OffsetY = 42.Millimeters(), Status = new FlowStatus(FlowState.Success)
            };

            Assert.AreEqual(expectedResult.OffsetX, systemUnderTest.Result.OffsetX);
            Assert.AreEqual(expectedResult.OffsetY, systemUnderTest.Result.OffsetY);
            Assert.AreEqual(expectedResult.Status.State, systemUnderTest.Result.Status.State);
        }

        [TestMethod]
        public void SaveDistantSensorCalibration_ShouldStoreDistantSensorCalibrationResult()
        {
            // Given
            var systemUnderTest = new DistanceSensorCalibrationVM(_calibrationSupervisor, _fakeAlgoSupervisor, _mockDialogOwnerService.Object)
            {
                Result = new DistanceSensorCalibrationResult { OffsetX = new Length(42, LengthUnit.Millimeter), OffsetY = new Length(42, LengthUnit.Millimeter) }
            };

            // When
            systemUnderTest.SaveCalibration.Execute(null);

            // Then
            Mock.Get(_calibrationSupervisor)
                .Verify(x => x.SaveCalibration(It.Is<DistanceSensorCalibrationData>(c =>
                    c.OffsetX.Equals(systemUnderTest.Result.OffsetX) && c.OffsetY.Equals(systemUnderTest.Result.OffsetY))));
        }        
        [TestMethod]
        public void CanLeave_ShouldReturnTrue_WhenHasNotChanged()
        {
            // Arrange
            var systemUnderTest = new DistanceSensorCalibrationVM(_calibrationSupervisor, _fakeAlgoSupervisor, _mockDialogOwnerService.Object);
            systemUnderTest.HasChanged = false;

            // Act
            bool result = systemUnderTest.CanLeave(null);

            // Assert
            Assert.IsTrue(result, "Expected CanLeave to return true when HasChanged is false.");
        }

        [TestMethod]
        public void CanLeave_ShouldReturnFalse_WhenHasChangedAndUserClicksNo()
        {
            // Arrange
            var systemUnderTest = new DistanceSensorCalibrationVM(_calibrationSupervisor, _fakeAlgoSupervisor, _mockDialogOwnerService.Object);
            systemUnderTest.HasChanged = true;

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
            bool result = systemUnderTest.CanLeave(null);

            // Assert
            Assert.IsFalse(result, "Expected CanLeave to return false when HasChanged is true, and user declined to leave by clicking No.");
        }
        [TestMethod]
        public void CanLeave_ShouldReturnTrue_WhenForceCloseIsTrue()
        {
            // Arrange
            var systemUnderTest = new DistanceSensorCalibrationVM(_calibrationSupervisor, _fakeAlgoSupervisor, _mockDialogOwnerService.Object);
            // Act
            bool result = systemUnderTest.CanLeave(null, forceClose: true);
            // Assert
            Assert.IsTrue(result, "Expected CanLeave to return true when forceClose is true, regardless of HasChanged.");
        }
    }
}
