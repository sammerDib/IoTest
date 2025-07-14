using UnitySC.Shared.Image;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Camera
{
    public class ImageGrabbedMessage
    {
        public string CameraId;

        /// <summary>
        /// L'image recue par WCF
        /// </summary>
        public ServiceImage ServiceImage;
    }
}
