using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Ffus;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ffu;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Ffu
{
    public class Astrofan612Ffu : FfuBase
    {
        private Astrofan612FfuController _controller;
        private Astrofan612FfuConfig _ffuConfig;

        public Astrofan612Ffu(IGlobalStatusServer globalStatusServer, ILogger logger, FfuConfig config, FfuController controller) :
            base(globalStatusServer, logger, config, controller)
        {
            _controller = (Astrofan612FfuController)controller;
            _ffuConfig = (Astrofan612FfuConfig)config;
        }

        public override void Init()
        {
            base.Init();
            SetSpeed(_ffuConfig.NormalRunningSpeed_percentage);
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
