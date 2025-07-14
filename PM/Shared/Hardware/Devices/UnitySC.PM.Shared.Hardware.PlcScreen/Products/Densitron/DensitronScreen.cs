using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Screens;
using UnitySC.PM.Shared.Hardware.Service.Interface.PlcScreen;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.PlcScreen
{
    public class DensitronScreen : PlcScreenBase
    {
        private DensitronDM430GNScreenController _controller;

        public DensitronScreen(IGlobalStatusServer globalStatusServer, ILogger logger, ScreenConfig config, ScreenController controller) :
            base(globalStatusServer, logger, config, controller)
        {
            _controller = (DensitronDM430GNScreenController)controller;
        }

        public override void Init()
        {
            base.Init();
        }

        public override void PowerOn()
        {
            _controller.PowerOnAsync();
        }

        public override void PowerOff()
        {
            _controller.PowerOffAsync();
        }

        public void SetBacklight(short value_InPercent)
        {
            _controller.SetBacklightAsync(value_InPercent);
        }

        public void SetBrightness(short value_InPercent)
        {
            _controller.SetBrightnessAsync(value_InPercent);
        }

        public void SetContrast(short value_InPercent)
        {
            _controller.SetContrastAsync(value_InPercent);
        }

        public void SetDefaultValue()
        {
            _controller.SetDefaultValueAsync();
        }

        public void SetFanStep(DisplayControlStep step)
        {
            _controller.SetFanStepAsync(step);
        }

        public void SetSharpness(DisplayControlStep step)
        {
            _controller.SetSharpnessAsync(step);
        }

        public void FanAutoOn(bool autOn)
        {
            _controller.FanAutoOn(autOn);
        }

        public void CustomCommand(string cmd)
        {
            _controller.CustomCommand(cmd);
        }

        public override void TriggerUpdateEvent()
        {
            _controller.TriggerUpdateEvent();
        }

        public void RestoreParameters(IOpcMultiParams multiParams)
        {
            _controller.RestoreParameters(multiParams);
        }
    }
}
