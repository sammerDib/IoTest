using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.ExternalFile;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ServiceImageWithStatistics))]
    public class PatternRecognitionData
    {
        public PatternRecognitionData()
        {
        }        
        public PatternRecognitionData(ExternalImage patternReference, string cameraId, RegionOfInterest roi = null, double gamma = double.NaN)
        {
            PatternReference = patternReference;
            Gamma = gamma;
            RegionOfInterest = roi;
            CameraId = cameraId;
        }

        public virtual InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (CameraId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The camera ID is missing.");
            }

            if (PatternReference is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The pattern reference is missing.");
            }

            return validity;
        }

        [DataMember]
        public ExternalImage PatternReference { get; set; }       

        [DataMember]
        public RegionOfInterest RegionOfInterest { get; set; }

        [DataMember]
        public double Gamma { get; set; }

        [DataMember]
        public string CameraId { get; set; }
    }
}
