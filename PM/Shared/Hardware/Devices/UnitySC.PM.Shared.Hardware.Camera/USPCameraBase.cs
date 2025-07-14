using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Camera
{
    public abstract class USPCameraBase : CameraBase
    {
        protected USPCameraBase(CameraConfigBase config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
        }

        #region IMAGE PROCESSING

        public abstract USPImage SingleGrab();

        #endregion
    }
}
