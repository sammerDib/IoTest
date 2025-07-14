using System.Collections.Generic;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.PM.EME.Client.Proxy.KeyboardMouseHook;
using UnitySC.PM.EME.Client.Proxy.Light;
using UnitySC.PM.EME.Client.TestUtils;
using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Client.Test
{
    [TestClass]
    public class LightBenchTests
    {
        private LightBench _lightBench;

        [TestInitialize]
        public void Initialize()
        {
            var mockKeyboardMouseHook = new Mock<IKeyboardMouseHook>();
            var mockLogger = new Mock<ILogger>();

            var messenger = new WeakReferenceMessenger();
            var fakeLightsSupervisor = new FakeLightsSupervisor(messenger);
            var lightConfig = new List<EMELightConfig>
            {
                new EMELightConfig
                {
                     DeviceID = "3", Name = "ddf0deg", ControllerID = "ArduinoLightController"
                },
                new EMELightConfig
                {
                    DeviceID = "4", Name = "ddf90deg", ControllerID = "ArduinoLightController"
                }
            };
            fakeLightsSupervisor.Configs = lightConfig;

            _lightBench = new LightBench(fakeLightsSupervisor, mockKeyboardMouseHook.Object, mockLogger.Object,
                messenger);
        }

        [TestMethod]
        public void Init_ShouldPopulateLightsCollection()
        {
            // Assert
            Assert.AreEqual(_lightBench.Lights.Count, 2);
            Assert.AreEqual("3", _lightBench.Lights[0].DeviceID);
            Assert.AreEqual("ddf0deg", _lightBench.Lights[0].Name);
        }

        [TestMethod]
        public void OnDeactivated_ShouldUnregisterMessengerAndKeyboardMouseHook()
        {
            // Act
            var keyEventArgs = new KeyGlobalEventArgs(Key.F8, true);
            _lightBench.KeyboardMouseHook_KeyEvent(this, keyEventArgs);

            // Assert
            Assert.AreEqual(1, _lightBench.Lights[0].Power);
        }
    }
}
