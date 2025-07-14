using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class CheckWaferPresenceInput : IANAInputFlow
    {

        public CheckWaferPresenceInput(Length materialDiameter)
        {
            MaterialDiameter = materialDiameter;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);
            if (MaterialDiameter is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The material diameter is missing.");
            }
            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public Length MaterialDiameter { get; set; }
    }
}
