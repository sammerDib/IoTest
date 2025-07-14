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
    public class CameraMagnificationCalibrationVMTests
    {
        private FakeAlgoSupervisor _fakeAlgoSupervisor;
        private CalibrationConfiguration _configuration;
        private ICalibrationService _calibrationSupervisor;
        private Mock<IDialogOwnerService> _mockDialogOwnerService;

        [TestInitialize]
        public void Setup()
        {
            ClassLocator.ExternalInit(new Container(), true);
            ClassLocator.Default.Register<INavigationManager, NavigationManagerForCalibration>(true);
            _fakeAlgoSupervisor = new FakeAlgoSupervisor();
            _configuration = new CalibrationConfiguration() { TargetPixelSize = new Length(2, LengthUnit.Micrometer) };
            _calibrationSupervisor = Mock.Of<ICalibrationService>();
            _mockDialogOwnerService = new Mock<IDialogOwnerService>();
        }

        [TestMethod]
        public void StartPixelSizeComputation_ShouldReturnResult()
        {
            //Given
            var cameraMagnification = new CameraMagnificationCalibrationVM(_fakeAlgoSupervisor, _configuration, _calibrationSupervisor, _mockDialogOwnerService.Object);

            //When
            cameraMagnification.MeasurePixelSize.Execute(null);

            //Then
            var expected = new PixelSizeComputationResult()
            {
                PixelSize = 42.Millimeters(),
                Status = new FlowStatus(FlowState.Success)
            };
            Assert.AreEqual(expected.PixelSize, cameraMagnification.Result.PixelSize);
            Assert.AreEqual(expected.Status.State, cameraMagnification.Result.Status.State);
        }

        [TestMethod]
        public void ShouldAbortPixelSizeComputation()
        {
            //Given
            var cameraMagnification = new CameraMagnificationCalibrationVM(_fakeAlgoSupervisor, _configuration, _calibrationSupervisor, _mockDialogOwnerService.Object);

            //When
            cameraMagnification.MeasurePixelSize.Execute(null);
            cameraMagnification.Cancel.Execute(null);

            //Then
            Assert.IsNull(cameraMagnification.Result.PixelSize);
            Assert.AreEqual(FlowState.Canceled, cameraMagnification.Result.Status.State);
        }

        [TestMethod]
        [DataTestMethod]
        [DataRow(2, 2, true)]
        [DataRow(2, 2.02, true)]
        [DataRow(2, 1.98, true)]
        [DataRow(2, 1.979, false)]
        [DataRow(2, 3, false)]
        public void ShouldCompareTargetPixelSize(double targetPixelSize, double resultPixelSize, bool expectedResult)
        {
            //Given
            _configuration = new CalibrationConfiguration() { TargetPixelSize = new Length(targetPixelSize, LengthUnit.Millimeter) };
            var cameraMagnification = new CameraMagnificationCalibrationVM(_fakeAlgoSupervisor, _configuration, _calibrationSupervisor, _mockDialogOwnerService.Object)
            {
                Result = new PixelSizeComputationResult { PixelSize = new Length(resultPixelSize, LengthUnit.Millimeter) }
            };

            //When
            bool result = cameraMagnification.IsPixelSizeCloseToTarget();

            //Then
            Assert.AreEqual(expectedResult, result);
        }


        [TestMethod]
        public void SaveCameraCalibration_ShouldStorePixelSize()
        {
            //Given
            var cameraMagnification = new CameraMagnificationCalibrationVM(_fakeAlgoSupervisor, _configuration, _calibrationSupervisor, _mockDialogOwnerService.Object)
            {
                Result = new PixelSizeComputationResult { PixelSize = new Length(42, LengthUnit.Millimeter) }
            };

            //When
            cameraMagnification.SaveCalibration.Execute(null);

            //Then
            Mock.Get(_calibrationSupervisor)
                .Verify(x => x.SaveCalibration(It.Is<CameraCalibrationData>(c => c.PixelSize.Equals(cameraMagnification.Result.PixelSize))));
        }       
        [TestMethod]
        public void CanLeave_ShouldReturnTrue_WhenHasNotChanged()
        {
            // Arrange
            var cameraMagnification = new CameraMagnificationCalibrationVM(_fakeAlgoSupervisor, _configuration, _calibrationSupervisor, _mockDialogOwnerService.Object);
            cameraMagnification.HasChanged = false;

            // Act
            bool result = cameraMagnification.CanLeave(null);

            // Assert
            Assert.IsTrue(result, "Expected CanLeave to return true when HasChanged is false.");
        }

        [TestMethod]
        public void CanLeave_ShouldReturnFalse_WhenHasChangedAndUserClicksNo()
        {
            // Arrange
            var cameraMagnification = new CameraMagnificationCalibrationVM(_fakeAlgoSupervisor, _configuration, _calibrationSupervisor, _mockDialogOwnerService.Object);
            cameraMagnification.HasChanged = true;

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
            bool result = cameraMagnification.CanLeave(null);

            // Assert
            Assert.IsFalse(result, "Expected CanLeave to return false when HasChanged is true, and user declined to leave by clicking No.");
        }
        [TestMethod]
        public void CanLeave_ShouldReturnTrue_WhenForceCloseIsTrue()
        {
            // Arrange
            var cameraMagnification = new CameraMagnificationCalibrationVM(_fakeAlgoSupervisor, _configuration, _calibrationSupervisor, _mockDialogOwnerService.Object);
            // Act
            bool result = cameraMagnification.CanLeave(null, forceClose: true);
            // Assert
            Assert.IsTrue(result, "Expected CanLeave to return true when forceClose is true, regardless of HasChanged.");
        }
    }
}
