using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [KnownType(typeof(XYZTopZBottomPosition))]
    public class BareWaferAlignmentImageInput : IANAInputFlow
    {
        // For serialization
        public BareWaferAlignmentImageInput()
        {
        }

        public BareWaferAlignmentImageInput(WaferDimensionalCharacteristic waferDimensionalCharacteristic, string cameraId, PositionBase position, WaferEdgePositions edgePosition)
        {
            Position = position;
            CameraId = cameraId;
            EdgePosition = edgePosition;
            Wafer = waferDimensionalCharacteristic;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (CameraId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The camera ID is missing.");
            }

            if (Position is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The position is missing.");
            }
            else if (!(Position is XYZTopZBottomPosition))
            {
                validity.IsValid = false;
                validity.Message.Add($"The position must be an XYZTopZBottomPosition.");
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
        public PositionBase Position { get; set; }

        [DataMember]
        public WaferEdgePositions EdgePosition { get; set; }

        [DataMember]
        public WaferDimensionalCharacteristic Wafer { get; set; }
    }
}
