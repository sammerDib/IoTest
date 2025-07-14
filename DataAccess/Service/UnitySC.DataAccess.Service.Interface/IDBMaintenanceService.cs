using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Interface
{
    [ServiceContract(CallbackContract = typeof(IDBMaintenanceServiceCallback))]
    public interface IDBMaintenanceService
    {
        [OperationContract]
        Response<bool> BackupDB();

        [OperationContract]
        Response<List<string>> GetBackupsList();

        [OperationContract]
        Response<bool> RestoreDB(string dbBackupToRestore);

        [OperationContract]
        Response<bool> RepairDB(int userId);

        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();
    }
}
