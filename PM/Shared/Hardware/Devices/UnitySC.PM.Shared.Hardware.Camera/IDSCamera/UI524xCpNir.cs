using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Camera.IDSCamera
{
    public class UI524xCpNir : IDSCameraBase
    {
        protected new UI524xCpNirIDSCameraConfig Config;

        public UI524xCpNir(UI524xCpNirIDSCameraConfig config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
            Config = config;
        }
    }
}
