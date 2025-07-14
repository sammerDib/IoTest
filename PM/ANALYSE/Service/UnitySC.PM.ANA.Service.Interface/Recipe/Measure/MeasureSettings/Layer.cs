using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Colors;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    [DataContract]
    public class Layer
    {
        /// <summary>
        /// If a virtual layer is composed of several real layers, we are in a case where we must group the layers
        /// </summary>

        [DataMember]
        public List<LayerSettings> PhysicalLayers { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public LengthTolerance ThicknessTolerance { get; set; }

        [DataMember]
        public ProbeSettings ProbeSettings { get; set; }

        [DataMember]
        public double RefractiveIndex { get; set; }

        [XmlElement(Type = typeof(XmlColor))]
        [DataMember]
        public Color LayerColor { get; set; }

        /// <summary>
        /// The offset can be applied when creating a group.
        /// <summary>

        [DataMember]
        public Length MultipleLayersOffset { get; set; } = 0.Micrometers();

        /// <summary>
        /// Compute total thickness. All layers are selected and measured by a probe reselected by the user.
        /// <summary>

        [DataMember]
        public bool IsWaferTotalThickness { get; set; }
    }
}
