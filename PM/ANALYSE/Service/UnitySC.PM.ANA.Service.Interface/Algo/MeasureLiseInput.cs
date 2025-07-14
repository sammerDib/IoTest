using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class MeasureLiseInput : IANAInputFlow
    {
        public MeasureLiseInput()
        { }

        public MeasureLiseInput(ThicknessLiseInput liseData)
        {
            LiseData = liseData;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (LiseData is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The lise data is missing.");
            }
            else
            {
                validity.ComposeWith(LiseData.CheckInputValidity());
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public ThicknessLiseInput LiseData { get; set; }
    }
}
