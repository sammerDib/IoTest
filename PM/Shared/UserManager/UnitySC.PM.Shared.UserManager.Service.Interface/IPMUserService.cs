using System;
using System.ServiceModel;

using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.UserManager.Service.Interface
{
    [ServiceContract]
    public interface IPMUserService
    {
        [OperationContract]
        Response<UnifiedUser> Connect(string user, string password, int chamberKey, int toolKey);
    }
}
