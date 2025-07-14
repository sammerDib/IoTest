using System.Collections.Generic;

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
    public class RCMXYMoveTest
    {
        private Mock<RelianceAxis> _xAxisMock;
        private Mock<RelianceAxis> _yAxisMock;

        [TestInitialize]
        public void Initialize()
        {
            ClassLocator.ExternalInit(new Container(), true);
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            _xAxisMock = new Mock<RelianceAxis>(Mock.Of<RelianceAxisConfig>());
            _xAxisMock.Object.Config.PositionMin = int.MinValue.Millimeters();
            _xAxisMock.Object.Config.PositionMax = int.MaxValue.Millimeters();
            _xAxisMock.Object.AxisConfiguration.MovingDirection = MovingDirection.X;

            _yAxisMock = new Mock<RelianceAxis>(Mock.Of<RelianceAxisConfig>());
            _yAxisMock.Object.Config.PositionMin = int.MinValue.Millimeters();
            _yAxisMock.Object.Config.PositionMax = int.MaxValue.Millimeters();
            _yAxisMock.Object.AxisConfiguration.MovingDirection = MovingDirection.Y;
        }

        [TestMethod]
        public void Distance()
        {
            // Given
            _xAxisMock.Object.CurrentPos = 0.Millimeters();
            _yAxisMock.Object.CurrentPos = 0.Millimeters();

            // When
            var move = new RCMXYMove(
                new List<double> { 1, 0 },
                new List<IAxis> { _xAxisMock.Object, _yAxisMock.Object },
                new List<double> { 1, 1 },
                new List<double> { 1, 1 }
            );

            // Then
            move.X.Distance.Should().Be(1.Millimeters());
            move.Y.Distance.Should().Be(0.Millimeters());
        }

        [TestMethod]
        public void SpeedYIsOverridenForStraightMotion()
        {
            // Given
            _xAxisMock.Object.CurrentPos = 0.Millimeters();
            _yAxisMock.Object.CurrentPos = 0.Millimeters();

            // When
            var move = new RCMXYMove(
                new List<double> { 6, 3 },
                new List<IAxis> { _xAxisMock.Object, _yAxisMock.Object },
                new List<double> { 10, 10 },
                new List<double> { 1, 1 }
            );

            // Then
            move.X.Speed.Should().Be(10.MillimetersPerSecond());
            move.Y.Speed.Should().Be(5.MillimetersPerSecond());
        }

        [TestMethod]
        public void SpeedXIsOverridenForStraightMotion()
        {
            // Given
            _xAxisMock.Object.CurrentPos = 0.Millimeters();
            _yAxisMock.Object.CurrentPos = 0.Millimeters();

            // When
            var move = new RCMXYMove(
                new List<double> { 4, 40 },
                new List<IAxis> { _xAxisMock.Object, _yAxisMock.Object },
                new List<double> { 10, 10 },
                new List<double> { 1, 1 }
            );

            // Then
            move.X.Speed.Should().Be(1.MillimetersPerSecond());
            move.Y.Speed.Should().Be(10.MillimetersPerSecond());
        }
    }
}
