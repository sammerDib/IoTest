using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class CircleCriticalDimensionInput : IANAInputFlow
    {
        public CircleCriticalDimensionInput()
        { }

        public CircleCriticalDimensionInput(ServiceImage image, string objectiveId, CenteredRegionOfInterest roi, Length approximateDiameter, Length diameterTolerance)
        {
            Image = image;
            ObjectiveId = objectiveId;
            RegionOfInterest = roi;
            ApproximateDiameter = approximateDiameter;
            DiameterTolerance = diameterTolerance;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (ObjectiveId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The objective ID is missing.");
            }

            if (Image is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The image is missing.");
            }

            if (ApproximateDiameter is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The approximate diameter is missing.");
            }

            if (DiameterTolerance is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The diameter detection tolerance is missing.");
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public ServiceImage Image { get; set; }

        [DataMember]
        public string ObjectiveId { get; set; }

        [DataMember]
        public CenteredRegionOfInterest RegionOfInterest { get; set; }

        [DataMember]
        public Length ApproximateDiameter{ get; set; }

        [DataMember]
        public Length DiameterTolerance { get; set; }
    }
}
