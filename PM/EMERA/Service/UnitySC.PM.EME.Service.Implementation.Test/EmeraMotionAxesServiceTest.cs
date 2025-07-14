using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Implementation.Test
{
    [TestClass]
    public class EmeraMotionAxesServiceTest : TestWithMockedHardware<EmeraMotionAxesServiceTest>, ITestWithPhotoLumAxes, ITestWithChuck
    {
        private EmeraMotionAxesService _emeMotionAxesService;
        private Mock<ILogger<AxesService>> _logger;
        public Mock<ITestChuck> SimulatedChuck { get; set; }
        Mock<PhotoLumAxes> ITestWithPhotoLumAxes.SimulatedMotionAxes { get; set; }

        [TestInitialize]
        public void SetupTest()
        {
            _logger = new Mock<ILogger<AxesService>>();
            _emeMotionAxesService = new EmeraMotionAxesService(_logger.Object, HardwareManager); 

        }       
        [TestMethod]
        public void GoToEfemLoad_ShouldReturnTrue_WhenSlotConfigExists()
        {
            // Arrange          
            var speed = AxisSpeed.Slow;
            // Act
            var response = _emeMotionAxesService.GoToEfemLoad(200.Millimeters(), speed);
            // Assert
            Assert.IsTrue(response.Result);
        }
        [TestMethod]
        public void GoToManualLoad_ShouldReturnTrue_WhenSlotConfigExists()
        {
            // Arrange          
            var speed = AxisSpeed.Slow;
            // Act
            var response = _emeMotionAxesService.GoToManualLoad(200.Millimeters(), speed);
            // Assert
            Assert.IsTrue(response.Result);
        }
        [TestMethod]
        public void GoToEfemLoad_ShouldReturnFalse_WhenSlotConfigDoesNotExist()
        {
            // Arrange          
            var speed = AxisSpeed.Slow;
            // Act
            var response = _emeMotionAxesService.GoToEfemLoad(700.Millimeters(), speed);
            // Assert
            Assert.IsFalse(response.Result);
            _logger.Verify(l => l.Error("GoToEfemLoad", It.IsAny<Exception>()), Times.Once);
        }
        [TestMethod]
        public void GoToManualLoad_ShouldReturnFalse_WhenSlotConfigDoesNotExist()
        {
            // Arrange          
            var speed = AxisSpeed.Slow;
            // Act
            var response = _emeMotionAxesService.GoToManualLoad(700.Millimeters(), speed);
            // Assert
            Assert.IsFalse(response.Result);
            _logger.Verify(l => l.Error("GoToManualLoad", It.IsAny<Exception>()), Times.Once);
        }
    }
}
