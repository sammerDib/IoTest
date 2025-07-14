using UnitySC.PM.DMT.Hardware.Screen;
using UnitySC.PM.DMT.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Hardware.Manager
{
    public interface IDMTInternalCameraMethods
    {
        USPImageMil CreateMask(Side side, ROI roi, bool ignorePerspectiveCalibration = false);

        /// <summary>
        /// Get next available image from the camera. Next camera image is consider as 
        /// available after at least 1/cameraFramerate second(s).
        /// </summary>
        USPImageMil GrabNextImage(CameraBase camera);

        double SetExposureTime(CameraBase camera, double exposureTimeMs, ScreenBase screen = null, int period = 0);
    }
}
