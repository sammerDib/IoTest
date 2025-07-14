using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Proxy.Test
{
    [TestClass]
    public class UnitTestCamera
    {

        const string cameraId = "1";


        [TestMethod]
        public void GetSettings()
        {
            var cameraSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
            var settings = cameraSupervisor.GetSettings(cameraId)?.Result;

            Assert.IsNotNull(settings);
            Assert.AreEqual(1, settings.Gain);
        }

        [TestMethod]
        public void GetSettingsForIDNotExists()
        {
            var cameraSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
            var settings = cameraSupervisor.GetSettings("NotExists")?.Result;

            Assert.IsNull(settings);
        }

        [TestMethod]
        public void SetSettings()
        {
            var cameraSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
            var settings = cameraSupervisor.GetSettings(cameraId)?.Result;

            Assert.IsNotNull(settings);
            double oldGain = settings.Gain;

            var newParams = new CameraInputParams();
            newParams.Gain = oldGain + 1;

            Assert.IsTrue(cameraSupervisor.SetSettings(cameraId, newParams)?.Result);

            settings = cameraSupervisor.GetSettings(cameraId)?.Result;
            Assert.AreNotEqual(oldGain, settings.Gain);
            Assert.AreEqual(newParams.Gain, settings.Gain);
        }

        [TestMethod]
        public void SetSettingsForIDNotExists()
        {
            var cameraSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
            var newParams = new CameraInputParams();
            newParams.Gain = 9999;

            Assert.IsFalse(cameraSupervisor.SetSettings("NotExists", newParams)?.Result);
        }

        [TestMethod]
        public void GetCameraInfo()
        {
            var cameraSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
            var cameraInfo = cameraSupervisor.GetCameraInfo("1")?.Result;

            Assert.IsNotNull(cameraInfo);
            Assert.AreEqual("DummyIDSCamera", cameraInfo.Model);
            Assert.AreEqual("DummySN", cameraInfo.SerialNumber);
        }

        [TestMethod]
        public void GetCameraInfoForIDNotExists()
        {
            var cameraSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
            var cameraInfo = cameraSupervisor.GetCameraInfo("NotExists")?.Result;
            Assert.IsNull(cameraInfo);
        }

        [TestMethod]
        public void GetSingleGrabImage()
        {
            var cameraSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
            var image = cameraSupervisor.GetSingleGrabImage(cameraId)?.Result;

            Assert.IsNotNull(image);
            Assert.AreEqual(image.DataHeight * image.DataWidth, image.Data.Length);
        }

        [TestMethod]
        public void GetSingleGrabImageForIDNotExists()
        {
            var cameraSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
            var image = cameraSupervisor.GetSingleGrabImage("NotExists")?.Result;

            Assert.IsNull(image);
        }
    }

}
