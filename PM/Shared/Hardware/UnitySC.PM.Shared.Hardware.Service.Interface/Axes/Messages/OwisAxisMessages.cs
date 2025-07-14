
namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Owis
{
    public class StateMessage
    {
        public DeviceState State;
    }

    public class StatusMessage
    {
        public int StatusId;
    }

    public class CheckConnectionMessage
    {
        public string Connection;
    }

    public class PositionCountMessage
    {
        public int PositionCount;
    }

    public class IdMessage
    {
        public string Id;
    }

    public class CustomMessage
    {
        public string Custom;
    }
}
