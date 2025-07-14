using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class DieAndStreetSizesInput : IANAInputFlow
    {
        public DieAndStreetSizesInput()
        { }

        public DieAndStreetSizesInput(PositionWithPatternRec topLeftCorner, PositionWithPatternRec bottomRightCorner, WaferDimensionalCharacteristic waferCharacteristics, Length edgeExclusion, AutoFocusSettings autofocusSettings)
        {
            TopLeftCorner = topLeftCorner;
            BottomRightCorner = bottomRightCorner;
            Wafer = waferCharacteristics;
            EdgeExclusion = edgeExclusion;
            AutoFocusSettings = autofocusSettings;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (EdgeExclusion is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The edge exclusion is missing.");
            }

            if (Wafer is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The wafer characteristics is missing.");
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

            if (! (AutoFocusSettings is null))
            {
                validity.ComposeWith(AutoFocusSettings.CheckInputValidity());
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public string CameraID { get; set; }

        [DataMember]
        public PositionWithPatternRec TopLeftCorner { get; set; }

        [DataMember]
        public PositionWithPatternRec BottomRightCorner { get; set; }

        [DataMember]
        public WaferDimensionalCharacteristic Wafer { get; set; }

        [DataMember]
        public Length EdgeExclusion { get; set; }

        [DataMember]
        public AutoFocusSettings AutoFocusSettings { get; set; }
    }
}
