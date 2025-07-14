namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    public delegate void StateChangedEventHandler(object sender, StatusChangedEventArgs recipeStatus);

    public interface IDevice
    {
        DeviceFamily Family { get; }

        string Name { get; set; }

        string DeviceID { get; set; }

        DeviceState State { get; set; }

        event StateChangedEventHandler OnStatusChanged;
    }

    public enum DeviceFamily
    {
        Axes,
        Chuck,
        Plc,
        Camera,
        Screen,
        Stage,
        Probe,
        Light,
        Led,
        Wheel,
        DistanceSensor,
        FiberSwitch,
        Laser,
        Mppc,
        OpticalPowermeter,
        Shutter,
        Chamber,
        Controller,
        Spectrometer,
        Ffu,
        Rfid,
        Ionizer,
        Other
    }
}
