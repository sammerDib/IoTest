using System.Linq;

using CommunityToolkit.Mvvm.Messaging;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Hardware.Light;
using UnitySC.PM.EME.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Hardware.Test
{
    [TestClass]
    public class EmeLightTest
    {
        [TestMethod]
        public void AlphaLight_Should_SwitchOn()
        {
            // Given
            var light = NewLight();

            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            bool isSwitchOn = false;
            messenger.Register<LightSourceMessage>(this, (_, message) => isSwitchOn = message.SwitchOn);

            // When
            light.SwitchOn(true);

            // Then
            isSwitchOn.Should().Be(true);
        }

        [TestMethod]
        public void AlphaLight_Should_SetLightPower()
        {
            // Given
            var light = NewLight();

            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            double actualPower = 0.0;
            messenger.Register<LightSourceMessage>(this, (_, message) => actualPower = message.Power);

            // When
            light.SetPower(42.0);

            // Then
            actualPower.Should().Be(42.0);
        }

        private EMELightBase NewLight()
        {
            var configManager = new FakeConfigurationManager("ALPHA", null, true);
            var mockLoggerFactory= ClassLocator.Default.GetInstance<IHardwareLoggerFactory>();

            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(), mockLoggerFactory, configManager,
                new StubGlobalStatus(), null);
            hardwareManager.Init();
            var light = hardwareManager.EMELights.First().Value;
            return light;
        }
    }
}
