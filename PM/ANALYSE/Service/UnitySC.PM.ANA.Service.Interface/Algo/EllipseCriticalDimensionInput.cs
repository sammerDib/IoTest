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
    public class EllipseCriticalDimensionInput : IANAInputFlow
    {
        public EllipseCriticalDimensionInput()
        { }

        public EllipseCriticalDimensionInput(ServiceImage image, string objectiveId, CenteredRegionOfInterest roi, Length approximateLength, Length approximateWidth, Length lengthTolerance, Length widthTolerance)
        {
            Image = image;
            ObjectiveId = objectiveId;
            RegionOfInterest = roi;
            ApproximateLength = approximateLength;
            ApproximateWidth = approximateWidth;
            LengthTolerance = lengthTolerance;
            WidthTolerance = widthTolerance;
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

            if (ApproximateLength is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The approximate length is missing.");
            }

            if (ApproximateWidth is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The approximate width is missing.");
            }

            if (LengthTolerance is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The length detection tolerance is missing.");
            }

            if (WidthTolerance is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The width detection tolerance is missing.");
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
        public Length ApproximateLength { get; set; }

        [DataMember]
        public Length ApproximateWidth { get; set; }

        [DataMember]
        public Length LengthTolerance { get; set; }

        [DataMember]
        public Length WidthTolerance { get; set; }
    }
}
