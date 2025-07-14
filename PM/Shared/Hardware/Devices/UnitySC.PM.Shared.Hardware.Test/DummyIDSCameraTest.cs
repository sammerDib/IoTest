using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Test
{
    [TestClass]
    public class DummyIDSCameraTest
    {
        [TestInitialize]
        public void Initialize()
        {
            ClassLocator.ExternalInit(new Container(), true);
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);
        }

        [TestMethod]
        public void ShouldReceiveMessage_WhenAnImageIsGrabbed()
        {
            // GIVEN 
            var camera = new DummyIDSCamera(new CameraConfigBase(), null, new SerilogLogger<DummyIDSCamera>());

            bool messageReceived = false;
            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<CameraMessage>(this, (_, __) => messageReceived = true);

            // WHEN 
            var image = camera.SingleGrab();

            // THEN
            Assert.IsTrue(messageReceived);
            Assert.IsNotNull(image);
            Assert.AreEqual(8, image.ToServiceImage().Depth);
        }
        
        [TestMethod]
        public void Image_ShouldBe16Bits_WhenCameraDepthIsSetTo16()
        {
            // GIVEN 
            var config = new CameraConfigBase { Depth = 16 };
            var camera = new DummyIDSCamera(config, null, new SerilogLogger<DummyIDSCamera>());

            // WHEN 
            var image = camera.SingleGrab();

            // THEN
            Assert.AreEqual(16, image.ToServiceImage().Depth);
        }
    }
}
