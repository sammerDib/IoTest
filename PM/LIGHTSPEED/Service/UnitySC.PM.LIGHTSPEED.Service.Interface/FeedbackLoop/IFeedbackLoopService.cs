using System.ServiceModel;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.LIGHTSPEED.Service.Interface
{
    [ServiceContract(CallbackContract = typeof(IFeedbackLoopServiceCallback))]
    public interface IFeedbackLoopService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> InitializeUpdate();

        [OperationContract]
        Response<VoidResult> PowerOn();

        [OperationContract]
        Response<VoidResult> PowerOff();

        [OperationContract]
        Response<VoidResult> SetPower(int power);

        [OperationContract]
        Response<VoidResult> SetCurrent(int current);

        [OperationContract]
        Response<VoidResult> OpenShutterCommand();

        [OperationContract]
        Response<VoidResult> CloseShutterCommand();

        [OperationContract]
        Response<VoidResult> AttenuationHomePosition();

        [OperationContract]
        Response<VoidResult> MoveAbsPosition(double position);
    }
}
