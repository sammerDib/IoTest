using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Alignment
{
    [DataContract]
    public class AutoLightParameters
    {
        [DataMember]
        public bool LightIntensityIsDefinedByUser { get; set; }

        [DataMember]
        public double LightIntensity { get; set; }

        [DataMember]
        public double Exposure { get; set; }

        [DataMember]
        public double MinLightPower { get; set; }

        [DataMember]
        public double MaxLightPower { get; set; }

        [DataMember]
        public double LightPowerStep { get; set; }

        [DataMember]
        public AnaPositionContext AnaPositionContext { get; set; }

        [DataMember]
        public ObjectiveContext ObjectiveContext { get; set; }
    }
}
