using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Client.Proxy.Chiller;
using UnitySC.PM.EME.Client.TestUtils;
using UnitySC.PM.EME.Client.TestUtils.Dispatcher;
using UnitySC.PM.EME.Service.Interface.Chiller;

namespace UnitySC.PM.EME.Client.Test
{
    [TestClass]
    public class ChillerViewModelControlTests
    {
        private IMessenger _messenger;
        private ChillerViewModel _systemUnderTest;
        private FakeChillerSupervisor _fakeChillerSupervisor;

        [TestInitialize]
        public void Setup()
        {
            _messenger = new WeakReferenceMessenger();
            _fakeChillerSupervisor = new FakeChillerSupervisor(_messenger);
            _systemUnderTest = new ChillerViewModel(_fakeChillerSupervisor, new TestDispatcher(), _messenger);
        }
        
        [TestMethod]
        public void ShouldUpdateTemperatureWhenSupervisorRaisedTemperatureEvent()
        {
            _fakeChillerSupervisor.UpdateTemperatureCallback(40);
            Assert.AreEqual(40, _systemUnderTest.Temperature);
        }
        
        [TestMethod]
        public void ShouldUpdateCompressionWhenSupervisorRaisedCompressionEvent()
        {
            _fakeChillerSupervisor.UpdateMaxCompressionSpeedCallback(40);
            Assert.AreEqual(40, _systemUnderTest.Compression);
        }
        
        [TestMethod]
        public void ShouldUpdateFanSpeedWhenSupervisorRaisedFanSpeedEvent()
        {
            _fakeChillerSupervisor.UpdateFanSpeedCallback(0.7);
            Assert.AreEqual(0.7, _systemUnderTest.FanSpeedPercent);
        }
        
        [TestMethod]
        public void ShouldUpdateFanSpeedModeWhenSupervisorRaisedFanSpeedModeEvent()
        {
            _fakeChillerSupervisor.UpdateConstantFanSpeedModeCallback(ConstFanSpeedMode.Disabled);
            Assert.AreEqual(ConstFanSpeedMode.Disabled, _systemUnderTest.ConstFanSpeedMode);
        }
        
        [TestMethod]
        public void ShouldUpdateLeaksWhenSupervisorRaisedLeakEvent()
        {
            _fakeChillerSupervisor.UpdateLeaks(LeakDetection.Yes);
            Assert.AreEqual(LeakDetection.Yes, _systemUnderTest.Leak);
        }
        
        [TestMethod]
        public void ShouldUpdateAlarmsWhenSupervisorRaisedAlarmEvent()
        {
            _fakeChillerSupervisor.UpdateAlarms(AlarmDetection.Raised);
            Assert.AreEqual(AlarmDetection.Raised, _systemUnderTest.Alarms);
        }
        
        [TestMethod]
        public void ShouldUpdateChillerModeWhenSupervisorRaisedChillerModeEvent()
        {
            _fakeChillerSupervisor.UpdateMode(ChillerMode.Remote);
            Assert.AreEqual(ChillerMode.Remote, _systemUnderTest.Mode);
        }
    }
}
