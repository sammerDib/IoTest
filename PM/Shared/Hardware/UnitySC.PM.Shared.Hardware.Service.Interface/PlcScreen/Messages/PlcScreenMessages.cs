namespace UnitySC.PM.Shared.Hardware.Service.Interface.PlcScreen
{
    public class StateMessage : SideMessage
    {
        public DeviceState State;
    }

    public class StatusMessage : SideMessage
    {
        public string Status;
    }

    public class PowerStateMessage : SideMessage
    {
        public bool PowerState;
    }

    public class BacklightMessage : SideMessage
    {
        public double Backlight;
    }

    public class BrightnessMessage : SideMessage
    {
        public double Brightness;
    }

    public class ContrastMessage : SideMessage
    {
        public double Contrast;
    }

    public class FanAutoMessage : SideMessage
    {
        public bool FanAuto;
    }

    public class FanStepMessage : SideMessage
    {
        public double FanStep;
    }

    public class FanRpmMessage : SideMessage
    {
        public int FanRpm;
    }

    public class TemperatureMessage : SideMessage
    {
        public double Temperature;
    }

    public class CustomMessage : SideMessage
    {
        public string Custom;
    }
}
