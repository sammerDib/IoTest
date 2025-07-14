using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Data;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    [DataContract]
    public class WarpSettings : MeasureSettingsBase, ISummaryProvider
    {
        [XmlIgnore]
        public override MeasureType MeasureType => MeasureType.Warp;

        [XmlIgnore]
        public override bool MeasureStartAtMeasurePoint => true;

        [XmlIgnore]
        [DataMember]
        public override bool IsMeasureWithSubMeasurePoints { get; set; } = true;

        [DataMember]
        public bool IsSurfaceWarp { get; set; }

        [DataMember]
        public Length WarpMax { get; set; }

        [DataMember]
        public LengthTolerance TotalThicknessTolerance { get; set; } = new LengthTolerance(50.0, LengthToleranceUnit.Micrometer);

        [DataMember]
        public bool IsWaferTransparent { get; set; }

        [DataMember]
        public override List<int> SubMeasurePoints { get; set; }

        [DataMember]
        public Length TheoricalWaferThickness { get; set; }

        [DataMember]
        public List<LayerSettings> PhysicalLayers { get; set; }

        [DataMember]
        public ProbeSettings ProbeSettings { get; set; }

        [DataMember]
        public WaferDimensionalCharacteristic WaferCharacteristic { get; set; }

        public MeasureState GetWarpMeasureState(Length warpToTest)
        {
            return MeasureStateComputer.GetMeasureState(warpToTest, new Length(0d, LengthUnit.Micrometer), WarpMax);
        }

        #region ISummaryProvider

        public AutoFocusType? GetAutoFocusTypeUsed()
        {
            return null;
        }

        public List<string> GetLightsUsed()
        {
            return null;
        }

        public List<string> GetObjectivesUsed()
        {
            var objectivesUsed = new List<string>();
            var probeObjective = (ProbeSettings as SingleLiseSettings)?.ProbeObjectiveContext?.ObjectiveId;
            if (probeObjective != null)
            {
                objectivesUsed.Add(probeObjective);
            }

            return objectivesUsed.Distinct().ToList();
        }

        public List<string> GetProbesUsed()
        {
            var probesUsed = new List<string>
            {
                ProbeSettings?.ProbeId
            };

            return probesUsed;
        }

        #endregion ISummaryProvider
    }
}
