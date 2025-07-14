namespace UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor
{
    public class StateMessage
    {
        public DeviceState State { get; set; }
    }

    public class DistanceMessage
    {
        public double Distance { get; set; }
    }

    public class IdMessage
    {
        public string Id { get; set; }
    }

    public class CustomMessage
    {
        public string Custom { get; set; }
    }
}
