namespace UnitySC.PM.Shared.Hardware.ClientProxy.Plc
{
    public class StateChangedMessage
    {
        public string State { get; set; }
    }

    public class IdChangedMessage
    {
        public string Id { get; set; }
    }

    public class CustomChangedMessage
    {
        public string Custom { get; set; }
    }

    public class AmsNetIdChangedMessage
    {
        public string AmsNetId { get; set; }
    }
}
