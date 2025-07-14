using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.EME.Service.Shared.TestUtils.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Hardware.Test
{
    [TestClass]
    public class FilterWheelTest
    {
        [TestMethod]
        public void AlphaFilterWheel_ShouldBe_CorrectlyInitialized()
        {
            // Given
            var configManager = new FakeConfigurationManager("ALPHA", null, true);
            var logFacto = ClassLocator.Default.GetInstance<IHardwareLoggerFactory>();
            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(),logFacto, configManager, new StubGlobalStatus(), null);

            // When
            hardwareManager.Init();

            // Then
            hardwareManager.Wheel.Should().BeOfType(typeof(FilterWheel.FilterWheel));

            var filterWheel = (FilterWheel.FilterWheel)hardwareManager.Wheel;
            filterWheel.Name.Should().NotBeNullOrEmpty();
            filterWheel.DeviceID.Should().NotBeNullOrEmpty();

            filterWheel.Controller.Should().NotBeNull();
            filterWheel.FilterSlots.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public void AlphaFilterWheel_ShouldMove_ToTargetPosition()
        {
            // Given
            var configManager = ClassLocator.Default.GetInstance<IEMEServiceConfigurationManager>();
            var logFacto = ClassLocator.Default.GetInstance<IHardwareLoggerFactory>();

            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(),logFacto, configManager, new StubGlobalStatus(), null);
            hardwareManager.Init();
            var filterWheel = (FilterWheel.FilterWheel)hardwareManager.Wheel;

            // When
            const double targetPosition = 42.0;
            filterWheel.Move(targetPosition);
            double actualPosition = filterWheel.GetCurrentPosition();

            // Then
            actualPosition.Should().Be(targetPosition);
        }
    }
}
