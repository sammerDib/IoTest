using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [ServiceKnownType(typeof(XYZTopZBottomPosition))]
    public class BareWaferAlignmentImage : BareWaferAlignmentChangeInfo
    {
        [DataMember]
        public FlowState ImageState { get; set; }

        [DataMember]
        public WaferEdgePositions EdgePosition { get; set; }

        //Position
        [DataMember]
        public PositionBase Position { get; set; }

        //Image
        [DataMember]
        public ServiceImage Image { get; set; }

        //Edge Points in image coordinates
        [DataMember]
        public List<ServicePoint> EdgePoints { get; set; }

        [DataMember]
        public List<(ServicePoint pt1, ServicePoint pt2)> NotchLines { get; set; }
    }
}
