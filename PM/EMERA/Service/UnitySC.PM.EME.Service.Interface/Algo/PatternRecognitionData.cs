using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.ExternalFile;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ServiceImageWithStatistics))]
    public class PatternRecognitionData
    {
        public PatternRecognitionData()
        {
        }
        public PatternRecognitionData(ExternalImage patternReference, RegionOfInterest roi = null, double gamma = double.NaN)
        {
            PatternReference = patternReference;
            Gamma = gamma;
            RegionOfInterest = roi;
        }

        public virtual InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (PatternReference is null)
            {
                validity.IsValid = false;
                validity.Message.Add("The pattern reference is missing.");
            }

            return validity;
        }

        [DataMember]
        public ExternalImage PatternReference { get; set; }

        [DataMember]
        public RegionOfInterest RegionOfInterest { get; set; }

        [DataMember]
        public double Gamma { get; set; }
    }
}
