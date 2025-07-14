using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class WaferMapInput : IANAInputFlow
    {
        public WaferMapInput()
        { }

        public WaferMapInput(PositionWithPatternRec topLeftCorner, PositionWithPatternRec bottomRightCorner, WaferDimensionalCharacteristic waferCharacteristics, Length edgeExclusion, DieDimensionalCharacteristic dieDimensions)
        {
            TopLeftCorner = topLeftCorner;
            BottomRightCorner = bottomRightCorner;
            WaferCharacteristics = waferCharacteristics;
            DieDimensions = dieDimensions;
            EdgeExclusion = edgeExclusion;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (EdgeExclusion is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The edge exclusion is missing.");
            }

            if (DieDimensions is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The die dimensions are missing.");
            }
            else
            {
                if (DieDimensions.DieWidth.Millimeters <= 0)
                {
                    validity.IsValid = false;
                    validity.Message.Add($"The die width must be positive: {DieDimensions.DieWidth.Millimeters} mm.");
                }
                if (DieDimensions.DieHeight.Millimeters <= 0)
                {
                    validity.IsValid = false;
                    validity.Message.Add($"The die height must be positive:{DieDimensions.DieHeight.Millimeters} mm.");
                }
                if (DieDimensions.StreetWidth.Millimeters < 0)
                {
                    validity.IsValid = false;
                    validity.Message.Add($"The street width must be positive or zero: {DieDimensions.StreetWidth.Millimeters} mm.");
                }
                if (DieDimensions.StreetHeight.Millimeters < 0)
                {
                    validity.IsValid = false;
                    validity.Message.Add($"The street height must be positive or zero:{DieDimensions.StreetHeight.Millimeters} mm.");
                }
            }

            if (WaferCharacteristics is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The wafer characteristics are missing.");
            }

            if (TopLeftCorner is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The top left corner data is missing.");
            }
            else
            {
                validity.ComposeWith(TopLeftCorner.CheckInputValidity());
            }

            if (BottomRightCorner is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The bottom right corner data is missing.");
            }
            else
            {
                validity.ComposeWith(BottomRightCorner.CheckInputValidity());
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public WaferDimensionalCharacteristic WaferCharacteristics { get; set; }

        [DataMember]
        public Length EdgeExclusion { get; set; }

        [DataMember]
        public DieDimensionalCharacteristic DieDimensions { get; set; }

        [DataMember]
        public PositionWithPatternRec TopLeftCorner { get; set; }

        [DataMember]
        public PositionWithPatternRec BottomRightCorner { get; set; }
    }
}
