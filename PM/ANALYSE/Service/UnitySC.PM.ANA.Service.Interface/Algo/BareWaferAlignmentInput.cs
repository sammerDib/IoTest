using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class BareWaferAlignmentInput : IANAInputFlow
    {
        public BareWaferAlignmentInput()
        { }

        public BareWaferAlignmentInput(WaferDimensionalCharacteristic waferDimensionalCharacteristic, string cameraId)
        {
            Wafer = waferDimensionalCharacteristic;
            CameraId = cameraId;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (CameraId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The camera ID is missing.");
            }

            if (Wafer is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The wafer characteristic is missing.");
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public string CameraId { get; set; }

        [DataMember]
        public WaferDimensionalCharacteristic Wafer { get; set; }

        // ImageTop Optional
        [DataMember]
        public ServiceImageWithPosition ImageTop { get; set; }

        // ImageRight Optional
        [DataMember]
        public ServiceImageWithPosition ImageRight { get; set; }

        // ImageBottom Optional
        [DataMember]
        public ServiceImageWithPosition ImageBottom { get; set; }

        // ImageLeft Optional
        [DataMember]
        public ServiceImageWithPosition ImageLeft { get; set; }
    }
}
