using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Axes
{
    [ServiceContract(CallbackContract = typeof(IMotionAxesServiceCallback))]
    public interface IEmeraMotionAxesService : IMotionAxesService
    {
        [OperationContract]
        Response<VoidResult> GoToPosition(XYZPosition targetPosition, AxisSpeed speed);
        
        [OperationContract]
        Response<bool> GoToEfemLoad(Length waferDiameter, AxisSpeed speed);

        [OperationContract]
        Response<bool> GoToManualLoad(Length waferDiameter, AxisSpeed speed);

    }
}
