using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Interface
{
    [ServiceContract]
    public interface ILogService
    {
        [OperationContract]
        Response<VoidResult> Connect(Dto.User user);

        [OperationContract]
        Response<VoidResult> Disconnect(Dto.User user);

        [OperationContract]
        [PreserveReferences]
        Response<List<Dto.Log>> GetLogs(int? userId = null, Dto.Log.ActionTypeEnum? action = null, Dto.Log.TableTypeEnum? table = null, DateTime? startDate = null, DateTime? endDate = null, string detailFilter = null);

        [OperationContract]
        Response<bool> CheckDatabaseVersion();
    }
}
