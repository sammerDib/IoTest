using System.Runtime.Remoting.Contexts;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Context;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class PositionWithPatternRec
    {
        public PositionWithPatternRec()
        {
        }

        public PositionWithPatternRec(XYZTopZBottomPosition position, PatternRecognitionDataWithContext patternRecWithContext) : this(position, patternRecWithContext, patternRecWithContext.Context)
        {
        }

        public PositionWithPatternRec(XYZTopZBottomPosition position, PatternRecognitionData patternRec, ImageAcquisitionContextBase context)
        {
            Position = position;
            PatternRec = patternRec;
            Context = context;
        }

        [DataMember]
        public XYZTopZBottomPosition Position { get; set; }

        [DataMember]
        public PatternRecognitionData PatternRec { get; set; }

        [DataMember]
        public ImageAcquisitionContextBase Context { get; set; }

        public virtual InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (Position is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The position is missing.");
            }

            if (Context is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The context is missing.");
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
