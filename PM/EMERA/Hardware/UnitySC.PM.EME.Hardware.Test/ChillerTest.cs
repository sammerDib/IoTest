using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Hardware.Chiller;
using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Core.Referentials;
using UnitySC.PM.EME.Service.Interface.Chiller;
using UnitySC.PM.EME.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Hardware.Test
{
    [TestClass]
    public class ChillerTest
    {
        private static bool _isContainerConfigured;
        private static EmeHardwareManager _hardwareManager;
        private FakeConfigurationManager configManager = new FakeConfigurationManager("ALPHA", null, true);

        [TestMethod]
        public void ShouldSendMessageWhenTemperatureIsUpdated()
        {
            //Arrange
            double updatedTemperature = 0;
            var systemUnderTest = CreateChiller();
            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<ChillerTemperatureChangedMessage>(this, (_, message) => updatedTemperature = message.Value);
            
            //Act
            systemUnderTest.SetTemperature(45);
            
            //Assert
            Assert.AreEqual(45, updatedTemperature);
        }
        
        [TestMethod]
        public void ShouldSendMessageWhenFanSpeedIsUpdated()
        {
            //Arrange
            double updatedFanSpeed = 0;
            var systemUnderTest = CreateChiller();
            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<ChillerFanSpeedChangedMessage>(this, (_, message) => updatedFanSpeed = message.Value);
            
            //Act
            systemUnderTest.SetFanSpeed(70);
            
            //Assert
            Assert.AreEqual(70, updatedFanSpeed);
        }
        
        [TestMethod]
        public void ShouldSendMessageWhenMaxCompressionSpeedIsUpdated()
        {
            //Arrange
            double updatedMaxCompressionSpeed = 0;
            var systemUnderTest = CreateChiller();
            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<ChillerMaxCompressionSpeedChangedMessage>(this, (_, message) => updatedMaxCompressionSpeed = message.Value);
            
            //Act
            systemUnderTest.SetMaxCompressionSpeed(70);
            
            //Assert
            Assert.AreEqual(70, updatedMaxCompressionSpeed);
        }
        
        [TestMethod]
        public void ShouldSendMessageWhenFanSpeedModeIsUpdated()
        {
            //Arrange
            var updatedConstantFanSpeedMode = ConstFanSpeedMode.Disabled;
            var systemUnderTest = CreateChiller();
            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<ChillerConstantFanSpeedModeChangedMessage>(this, (_, message) => updatedConstantFanSpeedMode = message.Value);
            
            //Act
            systemUnderTest.SetConstantFanSpeedMode(ConstFanSpeedMode.Enabled);
            
            //Assert
            Assert.AreEqual(ConstFanSpeedMode.Enabled, updatedConstantFanSpeedMode);
        }
        
        [TestMethod]
        public void ShouldSendMessageWhenChillerModeIsUpdated()
        {
            //Arrange
            var updatedChillerMode = ChillerMode.Remote;
            var systemUnderTest = CreateChiller();
            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<ChillerModeChangedMessage>(this, (_, message) => updatedChillerMode = message.Value);
            
            //Act
            systemUnderTest.SetChillerMode(ChillerMode.Standalone);
            
            //Assert
            Assert.AreEqual(ChillerMode.Standalone, updatedChillerMode);
        }
        
        private Chiller.Chiller CreateChiller()
        {
            if (_isContainerConfigured)
            {
                return _hardwareManager.Chiller;
            }
            
            var mockLogger = Mock.Of<IHardwareLogger>();
            var mockLoggerFactory = Mock.Of<IHardwareLoggerFactory>(x => x.CreateHardwareLogger(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()) == mockLogger);

            var referentialManager = new EmeReferentialManager();
            _hardwareManager = new EmeHardwareManager(new SerilogLogger<EmeHardwareManager>(), mockLoggerFactory, configManager, new StubGlobalStatus(), referentialManager);
            _hardwareManager.Init();
            _isContainerConfigured = true;
            return _hardwareManager.Chiller;
        }
    }
}
