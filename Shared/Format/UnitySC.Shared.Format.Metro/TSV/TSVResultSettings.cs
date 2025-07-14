using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.Tools.Tolerances;

using System.Runtime.Serialization;

namespace UnitySC.Shared.Format.Metro.TSV
{
    // cf UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings enum TSVShape
    //
    // Circle => LengthTarget == WidthTarget
    // Elipse => LengthTarget != WidthTarget
    // Rectangle => LengthTarget != WidthTarget

    [DataContract]
    public class TSVResultSettings
    {
        [DataMember]
        public Length DepthTarget { get; set; }
        [DataMember]
        public LengthTolerance DepthTolerance { get; set; }
        [DataMember]
        public Length LengthTarget { get; set; }
        [DataMember]
        public LengthTolerance LengthTolerance { get; set; }
        [DataMember]
        public Length WidthTarget { get; set; }
        [DataMember]
        public LengthTolerance WidthTolerance { get; set; }
        [DataMember]
        public TSVShape Shape { get; set; }
    }
}
