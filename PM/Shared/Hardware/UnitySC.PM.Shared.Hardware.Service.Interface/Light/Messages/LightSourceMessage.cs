namespace UnitySC.PM.Shared.Hardware.Service.Interface.Light
{
    public class StateMessage
    {
        public DeviceState State;
    }

    public class LightSourceMessage
    {
        public string LightID { get; set; }
        public bool SwitchOn { get; set; }  
        public double Power { get; set; }
        public double Intensity { get; set; }
        public double Temperature { get; set; }
        public string OperatingTime { get; set; }
    }
}
