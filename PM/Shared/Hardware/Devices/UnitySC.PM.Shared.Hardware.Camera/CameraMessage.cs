using UnitySC.Shared.Image;

namespace UnitySC.PM.Shared.Hardware.Camera
{
    public class CameraMessage
    {
        public CameraBase Camera;
        public bool Error;
        public ICameraImage Image;
    }
}
