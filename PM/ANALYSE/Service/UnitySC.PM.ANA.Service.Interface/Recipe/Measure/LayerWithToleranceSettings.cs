using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Colors;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    [DataContract]
    public class LayerWithToleranceSettings:LayerSettings
    {
 
        [DataMember]
        public Length ThicknessTolerance { get; set; }

        public static bool operator ==(LayerWithToleranceSettings layer1, LayerWithToleranceSettings layer2)
        {
            return layer1.Name == layer2.Name &&
            layer1.Thickness == layer2.Thickness &&
            layer1.RefractiveIndex == layer2.RefractiveIndex;
        }

        public static bool operator !=(LayerWithToleranceSettings layer1, LayerWithToleranceSettings layer2)
        {
            return layer1.Name != layer2.Name &&
            layer1.Thickness != layer2.Thickness &&
            layer1.RefractiveIndex != layer2.RefractiveIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is LayerWithToleranceSettings settings &&
                   Name == settings.Name &&
                   EqualityComparer<Length>.Default.Equals(Thickness, settings.Thickness) &&
                   EqualityComparer<Length>.Default.Equals(ThicknessTolerance, settings.ThicknessTolerance)&&
                   RefractiveIndex == settings.RefractiveIndex &&
                   MaterialName == settings.MaterialName;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + (Name == null ? 0 : Name.GetHashCode());
            hash = hash * 23 + (Thickness == null ? 0 : Thickness.GetHashCode());
            hash = hash * 23 + (ThicknessTolerance == null ? 0 : ThicknessTolerance.GetHashCode());
            hash = hash * 23 + (RefractiveIndex == null ? 0 : RefractiveIndex.GetHashCode());
            hash = hash * 23 + (MaterialName == null ? 0 : MaterialName.GetHashCode());

            return hash;
        }

        public LayerWithToleranceSettings Clone()
        {
            var newLayerSettings=new LayerWithToleranceSettings();
            newLayerSettings.Name = Name;
            newLayerSettings.Thickness = Thickness;
            newLayerSettings.ThicknessTolerance = ThicknessTolerance;
            newLayerSettings.RefractiveIndex = RefractiveIndex;
            newLayerSettings.MaterialName = MaterialName;
            return newLayerSettings;
        }
    }
}
