using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class ProbeSampleLayer
    {
        public ProbeSampleLayer()
        {
        }

        public ProbeSampleLayer(Length thickness, LengthTolerance tolerance, double refractionIndex, bool isMandatory = true, string name = "")
        {
            Thickness = thickness;
            Tolerance = tolerance;
            RefractionIndex = refractionIndex;
            IsMandatory = isMandatory;
            Name = name;
        }

        public ProbeSampleLayer(Length thickness, LengthTolerance tolerance, double refractionIndex, double type) : this(thickness, tolerance, refractionIndex)
        {
            Type = type;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Length Thickness { get; set; }

        //TODO Lenght or lenghtTolerance?
        [DataMember]
        public LengthTolerance Tolerance { get; set; }

        [DataMember]
        public double RefractionIndex { get; set; }

        [DataMember]
        public bool IsMandatory { get; set; }

        [DataMember]
        public double Type { get; set; }
    }
}
