using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Interface.FilterWheel
{
    [ServiceContract]
    [ServiceKnownType(typeof(ThorlabsSliderAxisConfig))]
    [ServiceKnownType(typeof(OwisAxisConfig))]
    [ServiceKnownType(typeof(AerotechAxisConfig))]
    [ServiceKnownType(typeof(ParallaxAxisConfig))]
    [ServiceKnownType(typeof(IoAxisConfig))]
    [ServiceKnownType(typeof(CNCAxisConfig))]
    public interface IFilterWheelService
    {
        [OperationContract]
        Response<List<FilterSlot>> GetFilterSlots();

        [OperationContract]
        Response<AxisConfig> GetAxisConfiguration();

        [OperationContract]
        Response<double> GetCurrentPosition();

        [OperationContract]
        Response<VoidResult> GoToPosition(double targetPosition);

        [OperationContract]
        Response<VoidResult> WaitMotionEnd(int timeout);
    }
}
