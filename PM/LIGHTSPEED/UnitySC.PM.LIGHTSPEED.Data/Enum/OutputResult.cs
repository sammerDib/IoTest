using System;
using System.ComponentModel;

namespace UnitySC.PM.LIGHTSPEED.Data.Enum
{
    [Flags]
    public enum OutputResult
    {
        [Description("Curvature X")]
        CurvatureX = 1,

        [Description("Curvature Y")]
        CurvatureY = 2,

        [Description("Amplitude X")]
        AmplitudeX = 4,

        [Description("Amplitude Y")]
        AmplitudeY = 8,

        [Description("Raw images")]
        RawImages = 16,

        [Description("Topo result")]
        Topo = 32,

        [Description("Phase result")]
        Phase = 64,

        [Description("DarkPsd result")]
        DarkPsd = 128,
    }
}
