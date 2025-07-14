using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Service.Interface.FilterWheel;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Client.Proxy.Test
{
    [TestClass]
    public class FilterWheelSupervisorTests
    {
        [TestMethod]
        public void FilterWheelConfiguration_ShouldHave_RotiationMovingDirection()
        {
            // Given
            var _filterWheelSupervisor = new FilterWheelSupervisor(new SerilogLogger<IFilterWheelService>(), null);

            // When
            var configuration = _filterWheelSupervisor.GetAxisConfiguration().Result;

            // Then
            configuration.Should().NotBeNull();
            configuration.MovingDirection.Should().Be(MovingDirection.Rotation);
        }

        [TestMethod]
        public void CurrentPosition_ShouldBeEqualTo_TheMovedPosition()
        {
            // Given
            var _filterWheelSupervisor = new FilterWheelSupervisor(new SerilogLogger<IFilterWheelService>(), null);

            // When
            double targetPosition = 33.0;
            _filterWheelSupervisor.GoToPosition(targetPosition);
            _filterWheelSupervisor.WaitMotionEnd(500);
            double position = _filterWheelSupervisor.GetCurrentPosition().Result;

            // Then
            position.Should().Be(targetPosition);
        }
    }
}
