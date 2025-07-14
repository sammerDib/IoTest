using System;
using System.Windows;

using UnitySC.PM.Shared.Hardware.Camera.IDSCamera;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Camera.IDS
{
    public class AOITest : FunctionalTest
    {
        private readonly IDSCameraBase _cameraTop;

        public AOITest()
        {
            HardwareManager.Cameras.TryGetValue("1", out var camera);
            if (!(camera is IDSCameraBase))
            {
                throw new Exception($"Camera type is not valid. Expected 'IDSCameraBase' but get '{camera.GetType()}'.");
            }

            _cameraTop = (IDSCameraBase)camera;
        }

        public override void Run()
        {
            Logger.Information("\n******************\n[AOITest] START\n******************\n");
            Logger.Information($"{_cameraTop.GetSettings()}");

            var aoi = new Rect(320, 256, 640, 512);
            _cameraTop.SetAOI(aoi);
            _cameraTop.SetFrameRate(10);
            _cameraTop.SetExposureTimeMs(90);

            Logger.Information($"{_cameraTop.GetSettings()}");

            var image = _cameraTop.SingleGrab().ToServiceImage();
            image.SaveToFile(new PathString("imageAOI.jpg"));

            Logger.Information("[AOITest] END !");
        }
    }
}
