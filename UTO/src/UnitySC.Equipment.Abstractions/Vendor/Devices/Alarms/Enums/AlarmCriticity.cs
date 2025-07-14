using System.ComponentModel;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Alarms.Enums
{
    public enum AlarmCriticity
    {
        [Description(
            "Alarm is critical, thus when occured, the current command will be aborted and the device will enter maintenance mode.")]
        Critical = 0,

        [Description(
            "Alarm is non-critical, nothing apart from raising the alarm in the interface will be done.")]
        NonCritical = 1,

        [Description(
            "Alarm criticity has not been defnied, thus it will be considererd as critical.")]
        Undefined = 2
    }
}
