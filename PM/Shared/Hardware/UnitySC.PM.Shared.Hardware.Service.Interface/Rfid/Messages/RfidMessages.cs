namespace UnitySC.PM.Shared.Hardware.Service.Interface.Rfid
{
    public class StateMessage
    {
        public DeviceState State;
    }

    public class StatusMessage
    {
        public string Status;
    }

    public class TagMessage
    {
        public string Tag { get; set; }
    }
}
