using CommunityToolkit.Mvvm.Messaging;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

using UnitySC.PM.EME.Client.Modules.Calibration.ViewModel;
using UnitySC.PM.EME.Client.Proxy.Axes;
using UnitySC.PM.EME.Client.TestUtils;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.EME.Client.Modules.Calibration.Test
{
    [TestClass]
    public class ChuckParallelismCalibrationVMTests
    {
        [TestInitialize]
        public void Setup()
        {
            ClassLocator.ExternalInit(new Container(), true);
            ClassLocator.Default.Register<INavigationManager, NavigationManagerForCalibration>(true);          
        }
        [TestMethod]
        public void CyclingMovementOverX_ShouldMoveXposition()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var axeSupervisor = new FakeMotionAxesSupervisor(messenger);
            var calibrationSupervisor = new FakeCalibrationSupervisor();
            var initialPosition = new XYZPosition() { X = 10, Y = 10, Z = 0 };
            axeSupervisor.SetPosition(initialPosition);

            var axes = new AxesVM(axeSupervisor, null, null, null, null,  messenger);
            var viewModel = new ChuckParallelismCalibrationVM(axes, calibrationSupervisor);

            // When
            viewModel.StartCyclingOverXaxis.Execute(null);
            axeSupervisor.WaitForMovementNumber(2, timeout: 2000);
            viewModel.StopCyclingOverXaxis.Execute(null);

            // Then
            var newPosition = (XYZPosition)axeSupervisor.GetCurrentPosition().Result;
            newPosition.X.Should().BeOneOf(49.0, -49.0);
            newPosition.Y.Should().Be(double.NaN);
            newPosition.Z.Should().Be(double.NaN);
        }

        [TestMethod]
        public void CyclingMovementOverX_ShouldReturnBackAtOppositeWaferPotion()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var axeSupervisor = new FakeMotionAxesSupervisor(messenger);
            var calibrationSupervisor = new FakeCalibrationSupervisor();
            var initialPosition = new XYZPosition() { X = 50, Y = 0, Z = 0 };
            axeSupervisor.SetPosition(initialPosition);

            var axes = new AxesVM(axeSupervisor, null, null, null, null, messenger);
            var viewModel = new ChuckParallelismCalibrationVM(axes, calibrationSupervisor);

            // When
            viewModel.StartCyclingOverXaxis.Execute(null);
            axeSupervisor.WaitForMovementNumber(2, timeout: 2000);
            viewModel.StopCyclingOverXaxis.Execute(null);

            // Then
            var newPosition = (XYZPosition)axeSupervisor.GetCurrentPosition().Result;
            newPosition.X.Should().BeOneOf(49.0, -49.0);
            newPosition.Y.Should().Be(double.NaN);
            newPosition.Z.Should().Be(double.NaN);
        }

        [TestMethod]
        public void CyclingMovementOverY_ShouldMoveYposition()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var axeSupervisor = new FakeMotionAxesSupervisor(messenger);
            var calibrationSupervisor = new FakeCalibrationSupervisor();
            var initialPosition = new XYZPosition() { X = 10, Y = 10, Z = 0 };
            axeSupervisor.SetPosition(initialPosition);

            var axes = new AxesVM(axeSupervisor, null, null, null, null, messenger);
            var viewModel = new ChuckParallelismCalibrationVM(axes, calibrationSupervisor);

            // When
            viewModel.StartCyclingOverYaxis.Execute(null);
            axeSupervisor.WaitForMovementNumber(2, timeout: 2000);
            viewModel.StopCyclingOverYaxis.Execute(null);

            // Then
            var newPosition = (XYZPosition)axeSupervisor.GetCurrentPosition().Result;
            newPosition.X.Should().Be(double.NaN);
            newPosition.Y.Should().BeOneOf(49.0, -49.0);
            newPosition.Z.Should().Be(double.NaN);
        }

        [TestMethod]
        public void CyclingMovementOverY_ShouldReturnBackAtOppositeWaferPotion()
        {
            // Given
            IMessenger messenger = new WeakReferenceMessenger();
            var axeSupervisor = new FakeMotionAxesSupervisor(messenger);
            var calibrationSupervisor = new FakeCalibrationSupervisor();
            var initialPosition = new XYZPosition() { X = 0, Y = 50, Z = 0 };
            axeSupervisor.SetPosition(initialPosition);

            var axes = new AxesVM(axeSupervisor, null, null, null, null, messenger);
            var viewModel = new ChuckParallelismCalibrationVM(axes, calibrationSupervisor);

            // When
            viewModel.StartCyclingOverYaxis.Execute(null);
            axeSupervisor.WaitForMovementNumber(2, timeout: 2000);
            viewModel.StopCyclingOverYaxis.Execute(null);

            // Then
            var newPosition = (XYZPosition)axeSupervisor.GetCurrentPosition().Result;
            newPosition.X.Should().Be(double.NaN);
            newPosition.Y.Should().BeOneOf(49.0, -49.0);
            newPosition.Z.Should().Be(double.NaN);
        }
    }
}
