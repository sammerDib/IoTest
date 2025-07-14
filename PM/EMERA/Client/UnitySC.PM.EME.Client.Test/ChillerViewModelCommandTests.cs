using System;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Client.Proxy.Chiller;
using UnitySC.PM.EME.Client.TestUtils.Dispatcher;
using UnitySC.PM.EME.Service.Interface.Chiller;

namespace UnitySC.PM.EME.Client.Test
{
    [TestClass]
    public class ChillerViewModelCommandTests
    {
        private Mock<IChillerSupervisor> _supervisorMock;
        private ChillerViewModel _systemUnderTest;

        [TestInitialize]
        public void Init()
        {
            _supervisorMock = new Mock<IChillerSupervisor>();
            _systemUnderTest = new ChillerViewModel(_supervisorMock.Object, new TestDispatcher(), new WeakReferenceMessenger());
        }

        [TestMethod]
        public void EnableFanSpeedModeCommand_Should_CallSupervisor()
        {
            _systemUnderTest.EnableFanSpeedModeCommand.Execute(null);
            _supervisorMock.Verify(x => x.SetConstFanSpeedMode(It.IsAny<ConstFanSpeedMode>()), Times.Once);
        }

        [TestMethod]
        public void ResetChiller_Should_CallSupervisor()
        {
            _systemUnderTest.ResetCommand.Execute(null);
            _supervisorMock.Verify(x => x.Reset(), Times.Once);
        }

        [TestMethod]
        public void TurnOffStandaloneModeCommand_Should_CallSupervisor()
        {
            _systemUnderTest.SetStandAloneModeCommand.Execute(null);
            _supervisorMock.Verify(x => x.SetChillerMode(It.IsAny<ChillerMode>()), Times.Once);
        }

        [TestMethod]
        public void ChangeSpeedCommand_Should_CallSupervisorWithSpeedValue()
        {
            _systemUnderTest.FanSpeedPercentSliderValue = 0.7;
            _systemUnderTest.ChangeSpeed.Execute(null);
            _supervisorMock.Verify(
                x => x.SetFanSpeed(It.Is<double>(y => Math.Abs(y - _systemUnderTest.FanSpeedPercentSliderValue) < 0.1)),
                Times.Once);
        }
        
        [TestMethod]
        public void ChangeCompression_Should_CallSupervisorWithCompressionValue()
        {
            _systemUnderTest.CompressionPercentSliderValue = 0.7;
            _systemUnderTest.ChangeCompression.Execute(null);
            _supervisorMock.Verify(
                x => x.SetMaxCompressionSpeed(It.Is<double>(y => Math.Abs(y - _systemUnderTest.CompressionPercentSliderValue) < 0.1)),
                Times.Once);
        }
        
        [TestMethod]
        public void ChangeTemperature_Should_CallSupervisorWithTemperatureValue()
        {
            _systemUnderTest.TemperatureSliderValue = 0.7;
            _systemUnderTest.ChangeTemperature.Execute(null);
            _supervisorMock.Verify(
                x => x.SetTemperature(It.Is<double>(y => Math.Abs(y - _systemUnderTest.TemperatureSliderValue) < 0.1)),
                Times.Once);
        }
    }
}
