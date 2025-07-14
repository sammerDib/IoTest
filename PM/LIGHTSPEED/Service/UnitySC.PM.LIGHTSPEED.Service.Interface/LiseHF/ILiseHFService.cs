using System.ServiceModel;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.LIGHTSPEED.Service.Interface
{
    [ServiceContract(CallbackContract = typeof(ILiseHFServiceCallback))]
    public interface ILiseHFService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> PowerOn();

        [OperationContract]
        Response<VoidResult> PowerOff();

        [OperationContract]
        Response<VoidResult> SetPower(int power);

        [OperationContract]
        Response<VoidResult> AttenuationMoveAbsPosition(ServoPosition position);

        [OperationContract]
        Response<VoidResult> AttenuationHomePosition();

        [OperationContract]
        Response<VoidResult> OpenShutterCommand();

        [OperationContract]
        Response<VoidResult> CloseShutterCommand();

        [OperationContract]
        Response<VoidResult> FastAttenuationMoveAbsPosition(double position);

        [OperationContract]
        Response<VoidResult> InitializeUpdate();

        [OperationContract]
        Response<VoidResult> AttenuationRefresh();
    }
}
