using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Interface
{
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        Response<List<User>> GetAll(int? toolKey);

        [OperationContract]
        Response<User> GetUserByName(string name, int toolId, bool useArchive = false);

        [OperationContract]
        Response<User> GetUser(int id);

        [OperationContract]
        Response<bool> RenameUser(string oldName, string newName, int toolKey, int loggedUserId);

        [OperationContract]
        Response<VoidResult> RemoveUser(int id, int loggedUserId);

        [Obsolete("ConnectUserFromChamber is deprecated, please use ConnectUserFromToolKey instead.", false)]
        [OperationContract]
        Response<UnifiedUser> ConnectUserFromChamber(string name, UserProfiles profile, int chamberId);

        [OperationContract]
        Response<UnifiedUser> ConnectUserFromToolKey(string name, UserProfiles profile, int toolKey);

        [OperationContract]
        Response<UnifiedUser> ConnectUserFromToolId(string name, UserProfiles profile, int toolId);

        [OperationContract]
        Response<bool> CheckDatabaseVersion();
    }
}
