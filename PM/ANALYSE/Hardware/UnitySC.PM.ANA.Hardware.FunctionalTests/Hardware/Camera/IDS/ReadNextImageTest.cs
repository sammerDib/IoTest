using System.IO;

using UnitySC.PM.Shared.Hardware.Camera.IDSCamera;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests.Hardware.Camera.IDS
{
    public class ReadNextImageTest : FunctionalTest
    {
        public override void Run()
        {
            var cameraTop = (UI324xCpNir)HardwareManager.Cameras["1"];

            var cameraManager = ClassLocator.Default.GetInstance<ICameraManager>();
            var lastImg = cameraManager.GetNextCameraImage(cameraTop);
            var nextImg = cameraManager.GetNextCameraImage(cameraTop);
            Logger.Debug($"lastImg.Timestamp = {lastImg.Timestamp}");
            Logger.Debug($"nextImg.Timestamp = {nextImg.Timestamp}");
            nextImg.Save(Path.Combine("c", "output"));
        }
    }
}
