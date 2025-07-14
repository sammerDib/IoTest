using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class ProbeCalibReference
    {
        public ProbeCalibReference(double thickness, double tolerance, double refractionIndex)
        {
            Thickness = thickness;
            Tolerance = tolerance;
            RefractionIndex = refractionIndex;
        }

        [DataMember]
        public double Thickness { get; set; }

        [DataMember]
        public double Tolerance { get; set; }

        [DataMember]
        public double RefractionIndex { get; set; }
    }
}