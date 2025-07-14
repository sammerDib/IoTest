using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Ionizers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ionizer;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Ionizer
{
    public class SJH108Ionizer : IonizerBase
    {
        private KeyenceIonizerController _controller;

        public SJH108Ionizer(IGlobalStatusServer globalStatusServer, ILogger logger, IonizerConfig config, IonizerController ionizerController) :
            base(globalStatusServer, logger, config)
        {
            _controller = (KeyenceIonizerController)ionizerController;
        }

        public override void Init()
        {
            base.Init();
        }

        public override void OpenAirPneumaticValve()
        {
            _controller.OpenAirPneumaticValve();
        }

        public override void CloseAirPneumaticValve()
        {
            _controller.CloseAirPneumaticValve();
        }

        public override void TriggerUpdateEvent()
        {
            _controller.TriggerUpdateEvent();
        }
    }
}
