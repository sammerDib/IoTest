using System.ServiceModel;

using UnitySC.Shared.Data;

namespace UnitySC.DataAccess.Service.Interface
{
    [ServiceContract]
    public interface IDBMaintenanceServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void DbOperationProgressChanged(DbOperationProgressMessage progressMessage);
    }
}
