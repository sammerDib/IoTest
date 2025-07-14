using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Thickness
{
    [Serializable]
    [DataContract]
    public class ThicknessResultSettings
    {
        public static string WaferThicknessLayerName = "WaferThickness";

        [DataMember]
        public List<ThicknessLengthSettings> ThicknessLayers { get; set; }

        [DataMember]
        public Length TotalTarget { get; set; } // same for Total and WaferThickness layer if measured

        [DataMember]
        public LengthTolerance TotalTolerance { get; set; }// same for Total and WaferThickness layer if measured

        [DataMember]
        public bool HasWaferThicknesss { get; set; }

        [XmlIgnore]
        public double? TotalNotMeasuredLayers;

        [DataMember]
        public bool HasWarpMeasure { get; set; }

        [DataMember]
        public Length WarpTargetMax { get; set; }

        public void ComputeNotMeasuredLayers()
        {
            var unit = TotalTarget?.Unit ?? LengthUnit.Undefined;
            var layersNotMeasured = ThicknessLayers?.Where(x => x.IsMeasured == false).ToList();
            double totalNotMeasured = 0.0;
            if (!layersNotMeasured.IsNullOrEmpty() && layersNotMeasured.Any())
            {
                totalNotMeasured = layersNotMeasured.Sum(t => t.Target.GetValueAs(unit));
            }
            TotalNotMeasuredLayers = totalNotMeasured;
        }
    }
}
