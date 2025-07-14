using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Client.Proxy.Axes;
using UnitySC.PM.EME.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.ClientProxy.Referential;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Proxy.Test
{
    [TestClass]
    public class MotionAxesSupervisorTests
    {
        private AxesVM _axesVM;
        private EmeraMotionAxesSupervisor _axesSupervisor;

        [TestInitialize]
        public void Initialize()
        {
            var logger = new SerilogLogger<IEmeraMotionAxesService>();
            var messenger = new WeakReferenceMessenger();

            _axesSupervisor = new EmeraMotionAxesSupervisor(logger, messenger);
            var referentialSupervisor = new ReferentialSupervisor(new SerilogLogger<ReferentialSupervisor>(), messenger);
            _axesVM = new AxesVM(_axesSupervisor, null, referentialSupervisor,null, logger, messenger);
        }

        [TestMethod]
        public void GetCurrentPosition_ReturnsValidPosition()
        {
            // Arrange
            var expectedPosition = new XYZPosition(new WaferReferential());

            // Act
            var currentPosition = _axesVM.Position;

            // Assert
            Assert.IsNotNull(currentPosition);
            Assert.AreEqual(expectedPosition.X, currentPosition.X);
            Assert.AreEqual(expectedPosition.Y, currentPosition.Y);
            Assert.AreEqual(expectedPosition.Z, currentPosition.Z);
        }

        [TestMethod]
        public void CheckTheAxes()
        {
            string axisX = _axesVM.AxisIDx;
            string axisY = _axesVM.AxisIDy;

            // Assert
            Assert.IsNotNull(axisX);
            Assert.IsNotNull(axisY);
            Assert.AreEqual("X", axisX);
            Assert.AreEqual("Y", axisY);
        }

        [TestMethod]
        public void UpdatePosition_UpdatesPositionCorrectly()
        {
            //Init
            double x = 10.0;
            double y = 11.0;
            double z = 12.0;

            // Arrange
            var newPosition =
                new XYZPosition(new WaferReferential(), x, y,
                    z); // Mock a new position                                 

            // Act
            _axesVM.UpdatePosition(newPosition);

            // Assert
            Assert.AreEqual(x, _axesVM.Position.X);
            Assert.AreEqual(y, _axesVM.Position.Y);
            Assert.AreEqual(z, _axesVM.Position.Z);
        }

        [TestMethod]
        public void MoveX_MovesToNewPositionX()
        {
            // Arrange
            double newPositionX = 20.0; // Set a new position for X-axis            
            // Act
            _axesVM.MoveX.Execute(newPositionX);
            _axesSupervisor.PositionChanged(new XYZPosition(new WaferReferential(), newPositionX, 0.0, 0.0));
            // Assert
            Assert.AreEqual(newPositionX, _axesVM.Position.X);
            Assert.AreEqual(0.0, _axesVM.Position.Y);
            Assert.AreEqual(0.0, _axesVM.Position.Z);
        }

        [TestMethod]
        public void MoveX_MovesToNewPositionY()
        {
            // Arrange
            double newPositionY = 20.0; // Set a new position for X-axis            
            // Act
            _axesVM.MoveY.Execute(newPositionY);
            _axesSupervisor.PositionChanged(new XYZPosition(new WaferReferential(), 0.0, newPositionY, 0.0));
            // Assert
            Assert.AreEqual(newPositionY, _axesVM.Position.Y);
            Assert.AreEqual(0.0, _axesVM.Position.X);
            Assert.AreEqual(0.0, _axesVM.Position.Z);
        }

        [TestMethod]
        public void UpdateStatus_EnablesOrDisablesActionsBasedOnStatus()
        {
            // Arrange
            var axesState = new AxesState { AllAxisEnabled = true, OneAxisIsMoving = false };

            // Act
            _axesVM.UpdateStatus(axesState);

            // Assert
            Assert.AreEqual(axesState.AllAxisEnabled, _axesVM.Status.IsEnabled);
            Assert.AreEqual(axesState.OneAxisIsMoving, _axesVM.Status.IsMoving);
        }

        [TestMethod]
        public void MotionAxesSupervisorIsInitialized()
        {
            var motionAxesSupervisor =
                new EmeraMotionAxesSupervisor(ClassLocator.Default.GetInstance<ILogger<IEmeraMotionAxesService>>(),
                    null);
            Assert.IsNotNull(motionAxesSupervisor);
        }

        [TestMethod]
        public void MotionAxesSupervisorReturnsAxesConfigs()
        {
            var motionAxesSupervisor =
                new EmeraMotionAxesSupervisor(ClassLocator.Default.GetInstance<ILogger<IEmeraMotionAxesService>>(),
                    null);
            var configs = motionAxesSupervisor.GetAxesConfiguration();

            Assert.IsNotNull(configs.Result);
        }
    }
}
