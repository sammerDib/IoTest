using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.EME.Service.Interface.Context;
using UnitySC.PM.EME.Service.Interface.Flow;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(EMEContextBase))]
    public class ImagePreprocessingInput : IEMEInputFlow
    {
        public ImagePreprocessingInput()
        { }

        public ImagePreprocessingInput(string cameraId, XYZPosition position, RegionOfInterest roi, double gamma, ServiceImage preAcquiredImage = null)
        {
            CameraId = cameraId;
            Position = position;
            RegionOfInterest = roi;
            Gamma = gamma;
            PreAcquiredImage = preAcquiredImage;
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
        public EMEContextBase InitialContext { get; set; }

        [DataMember]
        public string CameraId { get; set; }

        [DataMember]
        public XYZPosition Position { get; set; }

        [DataMember]
        public CenteredRegionOfInterest CenteredRegionOfInterest { get; set; }

        [DataMember]
        public RegionOfInterest RegionOfInterest { get; set; }

        [DataMember]
        public double Gamma { get; set; }

        [DataMember]
        public ServiceImage PreAcquiredImage { get; set; }
    }
}
