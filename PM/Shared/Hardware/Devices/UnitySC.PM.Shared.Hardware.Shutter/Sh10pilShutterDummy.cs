using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Shutters;
using UnitySC.PM.Shared.Hardware.Service.Interface.Shutter;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Shutter
{
    public class Sh10pilShutterDummy : ShutterBase
    {
        private Sh10pilShutterDummyController _controller;

        public Sh10pilShutterDummy(IGlobalStatusServer globalStatusServer, ILogger logger, ShutterConfig config, ShutterController shutterController) :
            base(globalStatusServer, logger, config, shutterController)
        {
            _controller = (Sh10pilShutterDummyController)shutterController;
        }

        public override void Init()
        {
            base.Init();
            Logger.Information("Init Shutter as dummy");
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
