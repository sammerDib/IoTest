using System.Collections.Generic;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Camera;

namespace UnitySC.PM.Shared.Hardware.Common
{
    public interface ICameraManager
    {
        void ImageGrabbed(CameraBase camera, ICameraImage procimage);

        /// <summary>
        /// Get last captured images.
        /// Warning: in case of movement, the returned image may not be the image of current position. <see cref="GetNextCameraImage"/> may be used in such case.
        /// </summary>
        ICameraImage GetLastCameraImage(CameraBase camera);

        /// <summary>
        /// Wait for next image is available, regarding the frame rate, and return it.
        /// </summary>
        ICameraImage GetNextCameraImage(CameraBase camera);

        Dictionary<CameraBase, ICameraImage> LastImages { get; set; }

        Dictionary<CameraBase, long> LastImagesIds { get; set; }

        void Shutdown();
    }
}
