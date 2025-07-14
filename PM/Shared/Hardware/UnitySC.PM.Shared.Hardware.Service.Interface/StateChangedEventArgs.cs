namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    public class StatusChangedEventArgs
    {
        public StatusChangedEventArgs(DeviceStatus oldStatus, DeviceStatus newStatus)
        {
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }

        public DeviceStatus OldStatus { get; set; }

        public DeviceStatus NewStatus { get; set; }
    }
}