using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Core.Step;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    [DataContract]
    public class StepSettings : MeasureSettingsBase
    {
        [XmlIgnore]
        public override MeasureType MeasureType => MeasureType.Step;
        
        [DataMember]
        public XYPosition Point { get; set; }

        [DataMember]
        public Length ScanSize { get; set; }

        [DataMember]
        public Angle ScanOrientation { get; set; }

        [DataMember] 
        public Speed Speed{ get; set; }
        
        [DataMember]
        public string ProbeId { get; set; }

        [DataMember]
        public Length TargetHeight { get; set; }

        [DataMember]
        public LengthTolerance ToleranceHeight { get; set; }

        [DataMember]
        public StepKind StepKind { get; set; }
    }
}
