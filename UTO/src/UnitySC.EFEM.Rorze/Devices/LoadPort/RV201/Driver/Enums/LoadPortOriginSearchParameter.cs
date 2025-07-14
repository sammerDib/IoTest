using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums
{
    public enum LoadPortOriginSearchParameter
    {
        [Description("Performs normal origin search.")]
        Normal = 0,

        [Description("Performs normal origin search, "
                     + "and ends while the carrier is being clamped if the carrier exists.")]
        NormalAndClamp = 1,

        [Description("Performs the self-test.")]
        SelfTest = 9,

        [Description("Performs origin search with no obstacle detection. (The stage does not rotate.)")]
        WithNoObstacleDetection = 10
    }
}
