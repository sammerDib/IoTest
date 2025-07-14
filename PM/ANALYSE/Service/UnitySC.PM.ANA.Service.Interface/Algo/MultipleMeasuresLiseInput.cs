using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class MultipleMeasuresLiseInput : IANAInputFlow
    {
        public MultipleMeasuresLiseInput()
        { }

        public MultipleMeasuresLiseInput(ThicknessLiseInput measureLiseData, int nbMeasures)
        {
            NbMeasures = nbMeasures;
            MeasureLise = measureLiseData;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (MeasureLise is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The measure data is missing.");
            }
            else
            {
                validity.ComposeWith(MeasureLise.CheckInputValidity());
            }

            if (NbMeasures <= 0)
            {
                validity.IsValid = false;
                validity.Message.Add($"The measures number must be positive.");
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public int NbMeasures { get; set; }

        [DataMember]
        public ThicknessLiseInput MeasureLise { get; set; }
    }
}
