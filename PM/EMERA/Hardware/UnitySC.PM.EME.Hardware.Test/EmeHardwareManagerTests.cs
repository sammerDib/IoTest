using CommunityToolkit.Mvvm.Messaging;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.EME.Hardware.Chamber.Dummy;
using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.EME.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Hardware.Test
{
    [TestClass]
    public class EmeHardwareManagerTests
    {
        [TestMethod]
        public void InitializeHardware_FromAlphaConfiguration()
        {
            // Given
            var globalStatus = new StubGlobalStatus();
            var configManager = ClassLocator.Default.GetInstance<IEMEServiceConfigurationManager>();
            var mockLoggerFactory = ClassLocator.Default.GetInstance<IHardwareLoggerFactory>();
            var hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(), mockLoggerFactory, configManager, globalStatus, null);

            // When
            bool isInitialInSuccess = hardwareManager.Init();

            // Then
            globalStatus.GetGlobalState().Should().Be(PMGlobalStates.Free);
            isInitialInSuccess.Should().BeTrue();

            hardwareManager.Cameras.Should().HaveCount(1);
            hardwareManager.EMELights.Should().HaveCount(4);

            hardwareManager.MotionAxes.Should().NotBeNull().And.BeOfType<PhotoLumAxes>();
            hardwareManager.Wheel.Should().NotBeNull().And.BeOfType<FilterWheel.FilterWheel>();
            
            hardwareManager.Chamber.Should().NotBeNull().And.BeOfType<EmeDummyChamber>();
        }
    }
}
