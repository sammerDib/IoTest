using System.ServiceModel;

namespace UnitySC.DataAccess.Service.Interface
{
    [ServiceContract]
    public interface IResultServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnResultChanged(ResultNotificationMessage msg);

        [OperationContract(IsOneWay = true)]
        void OnResultStatsChanged(ResultStatsNotificationMessage msg);

        [OperationContract(IsOneWay = true)]
        void OnResultStateChanged(ResultStateNotificationMessage msg);
    }
}
