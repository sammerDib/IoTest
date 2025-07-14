using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Reliance;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.ControllersTests.Controllers.Reliance
{
    [TestClass]
    public class RCMAxisMoveTest
    {
        private RelianceAxis _axis;
        private Mock<RelianceAxis> _axisMock;

        [TestInitialize]
        public void Initialize()
        {
            ClassLocator.ExternalInit(new Container(), true);
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
            _axisMock = new Mock<RelianceAxis>(Mock.Of<RelianceAxisConfig>());
            _axis = _axisMock.Object;
        }

        [TestMethod]
        public void GetDestinationAsPulses_returns_integer()
        {
            // Given
            _axis.Config.PositionZero = 0.Millimeters();
            _axis.Config.MotorDirection = 1;
            _axis.Config.PositionMin = 0.Millimeters();
            _axis.Config.PositionMax = 2.Millimeters();

            var move = new RCMAxisMove(_axis, 1.000008.Millimeters());

            // When
            double pulses = move.GetDestinationAsPulses(50_000, 4.Millimeters());

            // Then
            pulses.Should().Be(12_500); // 50_000 / 4
        }

        [TestMethod]
        public void GetSpeedAsPulsesPerSecond_returns_integer()
        {
            // Given
            var move = new RCMAxisMove(_axis, 1.Millimeters(), 1.00000000001.MillimetersPerSecond(), 1.MillimetersPerSecondSquared());

            // When
            double pulsesPerSecond = move.GetSpeedAsPulsesPerSecond(50_000, 4.Millimeters());

            // Then
            pulsesPerSecond.Should().Be(12_500); // 50_000 / 4
        }
    }
}
