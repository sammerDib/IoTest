using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    [DataContract]
    public class BowSettings : MeasureSettingsBase, ISummaryProvider
    {
        [XmlIgnore]
        public override MeasureType MeasureType => MeasureType.Bow;

        [XmlIgnore]
        public override bool MeasureStartAtMeasurePoint => true;

        [XmlIgnore]
        [DataMember]
        public override bool IsMeasureWithSubMeasurePoints { get; set; } = true;

        [DataMember]
        public Length BowMax { get; set; }

        [DataMember]
        public Length BowMin { get; set; }

        [DataMember]
        public override List<int> SubMeasurePoints { get; set; }

        [DataMember]
        public ProbeSettings ProbeSettings { get; set; }

        [DataMember]
        public WaferDimensionalCharacteristic WaferCharacteristic { get; set; }

        public MeasureState GetBowMeasureState(Length bowToTest)
        {
            return MeasureStateComputer.GetMeasureState(bowToTest, BowMin, BowMax);
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
