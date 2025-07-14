using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Shutters;
using UnitySC.PM.Shared.Hardware.Service.Interface.Shutter;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Shutter
{
    public class Sh10pilShutter : ShutterBase
    {
        private Sh10pilShutterController _controller;

        public Sh10pilShutter(IGlobalStatusServer globalStatusServer, ILogger logger, ShutterConfig config, ShutterController shutterController) :
            base(globalStatusServer, logger, config, shutterController)
        {
            _controller = (Sh10pilShutterController)shutterController;
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OpenIris()
        {
            _controller.OpenIris();
        }

        public override void CloseIris()
        {
            _controller.CloseIris();
        }

        public override void TriggerUpdateEvent()
        {
            _controller.TriggerUpdateEvent();
        }
    }
}
