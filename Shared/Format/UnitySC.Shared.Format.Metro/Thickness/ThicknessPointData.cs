using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Linq;

using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.Format.Metro.Warp;

namespace UnitySC.Shared.Format.Metro.Thickness
{
    [KnownType(typeof(WarpPointData))]
    [DataContract]
    public class ThicknessPointData : MeasurePointDataResultBase, IMeasureAirGap
    {
        public ThicknessPointData()
        { }

        [DataMember]
        public List<ThicknessLengthResult> ThicknessLayerResults { get; set; }

        [DataMember]
        public ThicknessLengthResult WaferThicknessResult { get; set; }

        [XmlIgnore]
        [DataMember]
        public Length AirGapUp { get; set; }

        [XmlIgnore]
        [DataMember]
        public Length AirGapDown { get; set; }

        [XmlIgnore]
        [DataMember]
        public Length TotalThickness { get; private set; } //compute by ComputeTotalThickness called by ThicknessResult.FillNonSerializedProperties

        [XmlIgnore]
        [DataMember]
        public MeasureState TotalState { get; set; } = MeasureState.NotMeasured;

        [DataMember]
        public WarpPointData WarpResult { get; set; } // could be null if no warp is requested

        public void ComputeTotalThickness(ThicknessResultSettings settings)
        {
            LengthUnit unit = settings.TotalTarget.Unit;
            double totalMeasured = ThicknessLayerResults.Sum(t => t.Length == null ? 0.0 : t.Length.GetValueAs(unit));
            double totalNotMeasured = settings.TotalNotMeasuredLayers ?? 0.0;
            TotalThickness = new Length(totalMeasured + totalNotMeasured, unit);
        }

        public override string ToString()
        {
            string layerThicknessResultsStr = ThicknessLayerResults != null ? $"{nameof(ThicknessLayerResults)}: {string.Join(", ", ThicknessLayerResults)}" : string.Empty;
            return $"{base.ToString()} Total : {TotalThickness} {layerThicknessResultsStr}";
        }
    }
}
