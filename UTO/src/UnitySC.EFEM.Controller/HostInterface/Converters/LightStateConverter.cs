using System;

using UnitySC.EFEM.Controller.HostInterface.Enums;

namespace UnitySC.EFEM.Controller.HostInterface.Converters
{
    public static class LightStateConverter
    {
        public static Agileo.GUI.Services.LightTower.LightState ToGui(LightState lightState)
        {
            switch (lightState)
            {
                case LightState.Off:
                    return Agileo.GUI.Services.LightTower.LightState.Off;

                case LightState.On:
                    return Agileo.GUI.Services.LightTower.LightState.On;

                case LightState.Flashing:
                    return Agileo.GUI.Services.LightTower.LightState.Flashing;

                default:
                    throw new ArgumentOutOfRangeException(nameof(lightState), lightState, null);
            }
        }
    }
}
