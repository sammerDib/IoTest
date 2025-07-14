using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace UnitySC.PM.DMT.Service.Interface.Measure.Outputs
{
    [Flags]
    public enum DeflectometryOutput
    {
        [Description("Curvature")] 
        Curvature = 1 << 0,

        [Description("Amplitude")] 
        Amplitude = 1 << 1,

        [Description("Raw images")] 
        RawImages = 1 << 2,

        [Description("Global Topo result")] [XmlEnum("GlobalTopography")]
        GlobalTopo = 1 << 3,

        [Description("Nano Topo result")] [XmlEnum("NanoTopography")]
        NanoTopo = 1 << 4,

        [Description("Low angle dark-field result")]
        LowAngleDarkField = 1 << 5,

        //Pseudo-slope maps
        [Description("Unwrapped Phases")] [XmlEnum("SlopeMap")]
        UnwrappedPhase = 1 << 6
    }
}
