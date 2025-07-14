namespace UnitySC.PM.Shared.Hardware.Service.Interface.Ionizer
{
    public class StateMessage
    {
        public DeviceState State;
    }

    public class StatusMessage
    {
        public string Status;
    }

    public class AirPneumaticValveMessage
    {
        public bool ValveIsOpened { get; set; }
    }

    public class StaticEliminationInterruptMessage
    {
        public bool IsEnabledStaticEliminationInterrupt { get; set; }
    }

    public class AlarmMessage
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public uint AlarmID { get; set; }
        public bool Triggered { get; set; }

        public override string ToString()
        {
            return $"Name:{Name} Description:{Description} AlarmID:{AlarmID} Triggered:{Triggered}";
        }
    }
}
