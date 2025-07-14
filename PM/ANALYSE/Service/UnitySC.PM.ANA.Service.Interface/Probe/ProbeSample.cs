using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class ProbeSample : IProbeSample
    {
        public ProbeSample()
        { }

        public ProbeSample(List<ProbeSampleLayer> layers, string name, string info)
        {
            Layers = layers;
            Name = name;
            Info = info;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Info { get; set; }

        [DataMember]
        public List<ProbeSampleLayer> Layers { get; set; }
    }
}
