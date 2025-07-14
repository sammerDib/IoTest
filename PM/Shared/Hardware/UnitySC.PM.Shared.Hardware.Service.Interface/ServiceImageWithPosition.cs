using System.Runtime.Serialization;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [DataContract]
    [KnownType(typeof(XYZTopZBottomPosition))]
    public class ServiceImageWithPosition
    {
        public ServiceImageWithPosition()
        {
        }

        public ServiceImageWithPosition(ServiceImage image, PositionBase centerPosition = null) : this()
        {
            Image = image;
            CenterPosition = centerPosition;
        }

        [DataMember]
        public ServiceImage Image;

        [DataMember]
        public PositionBase CenterPosition;
    }
}
