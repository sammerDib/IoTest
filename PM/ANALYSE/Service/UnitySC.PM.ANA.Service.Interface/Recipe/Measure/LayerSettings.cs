using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Colors;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    [DataContract]
    public class LayerSettings
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Length Thickness { get; set; }

        [DataMember]
        public double? RefractiveIndex { get; set; }

        [DataMember]
        public string MaterialName { get; set; }

        [XmlElement(Type = typeof(XmlColor))]
        [DataMember]
        public Color LayerColor { get; set; }

        public static bool operator ==(LayerSettings layer1, LayerSettings layer2)
        {
            return layer1.Name == layer2.Name &&
            layer1.Thickness == layer2.Thickness &&
            (layer1.RefractiveIndex == layer2.RefractiveIndex || (layer1.RefractiveIndex.IsNullOrNaN() && layer2.RefractiveIndex.IsNullOrNaN()));
        }

        public static bool operator !=(LayerSettings layer1, LayerSettings layer2)
        {
            return layer1.Name != layer2.Name &&
            layer1.Thickness != layer2.Thickness &&
            layer1.RefractiveIndex != layer2.RefractiveIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is LayerSettings settings &&
                   Name == settings.Name &&
                   EqualityComparer<Length>.Default.Equals(Thickness, settings.Thickness) &&
                   (RefractiveIndex == settings.RefractiveIndex || (RefractiveIndex.IsNullOrNaN() && settings.RefractiveIndex.IsNullOrNaN())) &&
                   MaterialName == settings.MaterialName;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + (Name == null ? 0 : Name.GetHashCode());
            hash = hash * 23 + (Thickness == null ? 0 : Thickness.GetHashCode());
            hash = hash * 23 + (RefractiveIndex == null ? 0 : RefractiveIndex.GetHashCode());
            hash = hash * 23 + (MaterialName == null ? 0 : MaterialName.GetHashCode());

            return hash;
        }
    }
}
