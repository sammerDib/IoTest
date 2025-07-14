using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Ffus;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ffu;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Ffu
{
    public class FfuDummy : FfuBase
    {
        private DummyFfuController _controller;
        private Astrofan612FfuConfig _ffuConfig;

        public FfuDummy(IGlobalStatusServer globalStatusServer, ILogger logger, FfuConfig config, FfuController controller) :
            base(globalStatusServer, logger, config, controller)
        {
            _controller = (DummyFfuController)controller;
            _ffuConfig = (Astrofan612FfuConfig)config;
        }

        public override void Init()
        {
            base.Init();
            Logger.Information($"Init Ffu as dummy");
        }

        public override void PowerOn()
        {
            _controller.PowerOn();
        }

        public override void PowerOff()
        {
            _controller.PowerOff();
        }

        public override void SetSpeed(ushort speedPercent)
        {
            _controller.SetSpeed(speedPercent);
        }

        public override void CustomCommand(string custom)
        {
            _controller.CustomCommand(custom);
        }

        public override void TriggerUpdateEvent()
        {
            _controller.TriggerUpdateEvent();
        }

        public override Dictionary<string, ushort> GetDefaultFfuValues()
        {
            var screenDefaultValues = new Dictionary<string, ushort>
            {
                { "NormalRunningSpeed", _ffuConfig.NormalRunningSpeed_percentage },
                { "WarningRaised", (ushort)_ffuConfig.WarningRaised.SpeedUpper_percentage },
                { "AlarmRaised", (ushort)_ffuConfig.AlarmRaised.SpeedUpper_percentage }
            };
            return screenDefaultValues;
        }
    }
}
