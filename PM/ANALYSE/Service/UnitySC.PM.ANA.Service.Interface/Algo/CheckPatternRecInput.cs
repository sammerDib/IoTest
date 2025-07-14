using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class CheckPatternRecInput : IANAInputFlow
    {
        public CheckPatternRecInput()
        { }

        public CheckPatternRecInput(PositionWithPatternRec positionWithPatternRec, List<XYZTopZBottomPosition> validationPositions, Length tolerance)
        {
            ValidationPositions = validationPositions;
            Tolerance = tolerance;
            PositionWithPatternRec = positionWithPatternRec;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (ValidationPositions is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The validation positions are missing.");
            }
            else if (ValidationPositions.Count == 0)
            {
                validity.IsValid = false;
                validity.Message.Add($"The validation positions list is empty.");
            }

            if (Tolerance is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The tolerance is missing.");
            }

            if (PositionWithPatternRec == null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The position with pattern rec is missing.");
            }
            else
            {
                validity.ComposeWith(PositionWithPatternRec.CheckInputValidity());
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public PositionWithPatternRec PositionWithPatternRec { get; set; }

        [DataMember]
        public List<XYZTopZBottomPosition> ValidationPositions { get; set; }

        [DataMember]
        public Length Tolerance { get; set; }
    }
}
