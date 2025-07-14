using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.EME.Client.Proxy.Chamber
{
    public class IsInMaintenanceChangedMessage
    {
        public bool IsInMaintenance { get; set; }
    }

    public class ArmNotExtendedChangedMessage
    {
        public bool ArmNotExtended { get; set; }
    }

    public class InterlockChangedMessage
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public uint InterlockID { get; set; }
        public bool State { get; set; }
    }

    public class SlitDoorPositionChangedMessage
    {
        public SlitDoorPosition SlitDoorPosition { get; set; }
    }
}
