using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class ImagePreprocessingInput : IANAInputFlow
    {
        public ImagePreprocessingInput()
        { }

        public ImagePreprocessingInput(string cameraId, XYZTopZBottomPosition position, CenteredRegionOfInterest roi, double gamma)
        {
            CameraId = cameraId;
            Position = position;
            CenteredRegionOfInterest = roi;
            Gamma = gamma;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (CameraId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"CameraId is missing.");
            }

            if (Position is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"Position is missing.");
            }

            if (double.IsNaN(Gamma))
            {
                validity.IsValid = false;
                validity.Message.Add($"Gamma is missing.");
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public string CameraId { get; set; }

        [DataMember]
        public XYZTopZBottomPosition Position { get; set; }

        [DataMember]
        public CenteredRegionOfInterest CenteredRegionOfInterest { get; set; }

        [DataMember]
        public RegionOfInterest RegionOfInterest { get; set; }

        [DataMember]
        public double Gamma { get; set; }
    }
}
