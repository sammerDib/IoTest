using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [DataContract]
    public class PositionWithPatternRec
    {
        public PositionWithPatternRec()
        {
        }

        public PositionWithPatternRec(XYZPosition position, PatternRecognitionData patternRec)
        {
            Position = position;
            PatternRec = patternRec;
        }

        [DataMember]
        public XYZPosition Position { get; set; }

        [DataMember]
        public PatternRecognitionData PatternRec { get; set; }

        public virtual InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (Position is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The position is missing.");
            }

            if (PatternRec is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The pattern rec is missing.");
            }
            else
            {
                validity.ComposeWith(PatternRec.CheckInputValidity());
            }

            return validity;
        }
    }
}
