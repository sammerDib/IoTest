using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chamber
{
    public class StateMessage
    {
        public DeviceState State { get; set; }
    }

    public class IsInMaintenanceMessage
    {
        public bool IsInMaintenance { get; set; }
    }

    public class ArmNotExtendedMessage
    {
        public bool ArmNotExtended { get; set; }
    }

    public class EfemSlitDoorOpenPositionMessage
    {
        public bool EfemSlitDoorOpenPosition { get; set; }
    }

    public class IsReadyToLoadUnloadMessage
    {
        public bool IsReadyToLoadUnload { get; set; }
    }

    public class SlitDoorPositionMessage
    {
        public SlitDoorPosition SlitDoorPosition { get; set; }
    }

    public class SlitDoorOpenPositionMessage
    {
        public bool SlitDoorOpenPosition { get; set; }
    }

    public class SlitDoorClosePositionMessage
    {
        public bool SlitDoorClosePosition { get; set; }
    }

    public class CdaPneumaticValveMessage
    {
        public bool ValveIsOpened { get; set; }
    }

    public class InterlockMessage
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public uint InterlockID { get; set; }
        public string State { get; set; }

        public override string ToString()
        {
            return $"Name:{Name} Description:{Description} InterlockID:{InterlockID} State:{State}";
        }
    }
}
