using System.Runtime.Serialization;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    [DataContract]
    public class Layer
    {
        [DataMember]
        public int LayerId { get; set; }

        [DataMember]
        public Length Thickness { get; set; }

        [DataMember]
        public double? RefractiveIndex { get; set; }

        [DataMember]
        public int MaterialId { get; set; }

        public static bool operator ==(Layer layer1, Layer layer2)
        {
            return layer1.LayerId == layer2.LayerId &&
            layer1.Thickness == layer2.Thickness &&
            layer1.RefractiveIndex == layer2.RefractiveIndex;
        }

        public static bool operator !=(Layer layer1, Layer layer2)
        {
            return layer1.LayerId != layer2.LayerId &&
            layer1.Thickness != layer2.Thickness &&
            layer1.RefractiveIndex != layer2.RefractiveIndex;
        }
    }
}
