using System.Diagnostics;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DebuggerDisplay("X = {X}, Y = {Y}")]
    [DataContract]
    public class ProbePoint
    {
        public ProbePoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }
    }
}