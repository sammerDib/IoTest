using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums
{
    /// <summary>
    /// Define e84 parameter properties.
    /// Format definition:
    ///     - n for one-digit numeral (from '0' to '9')
    /// </summary>
    public enum E84ParameterProperty
    {
        [Description("Motion flag:\n"
                     + "= 0:E84 Communication disable(ES = OFF)\n"
                     + "  1:E84 Communication enable(ES = ON)\n"
                     + "Format = n")]
        OnOffSwitch = 0,

        [Description("Timer TP1\n"
                     + "Format = nnnnnnnn")]
        TimerTP1 = 1,

        [Description("Timer TP2\n"
                     + "Format = nnnnnnnn")]
        TimerTP2 = 2,

        [Description("Timer TP3\n"
                     + "Format = nnnnnnnn")]
        TimerTP3 = 3,

        [Description("Timer TP4\n"
                     + "Format = nnnnnnnn")]
        TimerTP4 = 4,

        [Description("Timer TP5\n"
                     + "Format = nnnnnnnn")]
        TimerTP5 = 5,

        [Description("Timer TP6\n"
                     + "Format = nnnnnnnn")]
        TimerTP6 = 6,

        [Description("Timer TD1 (Not used)\n"
                     + "Format = nnnnnnnn")]
        TimerTD1_NotUsed = 7
    }
}
