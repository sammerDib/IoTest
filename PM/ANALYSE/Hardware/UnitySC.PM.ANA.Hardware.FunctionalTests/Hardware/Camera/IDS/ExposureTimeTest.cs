using System;

using UnitySC.PM.Shared.Hardware.Camera.IDSCamera;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Camera.IDS
{
    public class ExposureTimeTest : FunctionalTest
    {
        private readonly IDSCameraBase _cameraTop;

        public ExposureTimeTest()
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
            Logger.Information("\n******************\n[ExposureTimeTest] START\n******************\n");

            _cameraTop.SetFrameRate(10);
            _cameraTop.SetExposureTimeMs(40);
            const int expectedMaximumFramerate = 24; // (1_000 / 40) - 1

            Logger.Information($"{_cameraTop.GetSettings()}");

            _cameraTop.SetMaxFramerate();
            double currentFramerate = _cameraTop.GetFrameRate();

            Logger.Information($"{_cameraTop.GetSettings()}");

            if (!currentFramerate.Near(expectedMaximumFramerate, 0.1))
            {
                throw new Exception($"Current framerate ({currentFramerate} fps) is not at its expected maximum value ({expectedMaximumFramerate} fps)");
            }

            Logger.Information("[ExposureTimeTest] END !");
        }
    }
}
