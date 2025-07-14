using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.DataAccess.SQL;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class UserService : DataAccesServiceBase, IUserService
    {
        private readonly List<UserRights> _rightForBasicUser = new List<UserRights>() { UserRights.Log, UserRights.RecipeReadonly, UserRights.Status };
        private readonly List<UserRights> _rightForExpertUser = new List<UserRights>() { UserRights.Log, UserRights.RecipeReadonly, UserRights.Status, UserRights.RecipeEdition, UserRights.ChangePMMode, UserRights.ManualOperation };
        private readonly List<UserRights> _rightForMaintenanceUser = new List<UserRights>() { UserRights.Log, UserRights.RecipeReadonly, UserRights.Status, UserRights.Calibration, UserRights.Configuration, UserRights.HardwareManagement, UserRights.ManualOperation, UserRights.ChangePMMode, UserRights.Debug };
        private readonly List<UserRights> _rightForAdmisnitratorUser = new List<UserRights>() { UserRights.Log, UserRights.RecipeReadonly, UserRights.Status, UserRights.Calibration, UserRights.Configuration, UserRights.HardwareManagement, UserRights.ManualOperation, UserRights.ChangePMMode, UserRights.RecipeEdition, UserRights.Debug };
        private readonly Dictionary<UserProfiles, List<UserRights>> _mapProfileRights = new Dictionary<UserProfiles, List<UserRights>>();

        public UserService(ILogger logger) : base(logger)
        {
            _mapProfileRights.Add(UserProfiles.Basic, _rightForBasicUser);
            _mapProfileRights.Add(UserProfiles.Expert, _rightForExpertUser);
            _mapProfileRights.Add(UserProfiles.Maintenance, _rightForMaintenanceUser);
            _mapProfileRights.Add(UserProfiles.Administrator, _rightForAdmisnitratorUser);
        }

        [Obsolete("ConnectUserFromChamber is deprecated, please use ConnectUserFromToolKey instead.", false)]
        public Response<UnifiedUser> ConnectUserFromChamber(string name, UserProfiles profile, int chamberId)
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int toolId = GetToolId(chamberId, unitOfWork);
                    return Connect(unitOfWork, name, profile, toolId);
                }
            });
        }

        public Response<UnifiedUser> ConnectUserFromToolKey(string name, UserProfiles profile, int toolKey)
        {
            //Note rti : 12/06/2023:  @ this moment, chamber key is not completely required, we distinguish user only from tool not from its chamber
            // a user from a tool is allowed to acces alls chamber within this tool
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int toolId = GetToolIdFromToolKey(toolKey, unitOfWork);
                    return Connect(unitOfWork, name, profile, toolId);
                }
            });
        }

        public Response<UnifiedUser> ConnectUserFromToolId(string name, UserProfiles profile, int toolId)
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    return Connect(unitOfWork, name, profile, toolId);
                }
            });
        }

        private UnifiedUser Connect(UnitOfWorkUnity unitOfWork, string name, UserProfiles profile, int toolId)
        {
            var user = unitOfWork.UserRepository.CreateQuery().FirstOrDefault(u => u.Name == name && u.ToolId == toolId);
            if (user != null)
            {
                user.IsArchived = false;
            }
            else
            {
                user = unitOfWork.UserRepository.Add(new User() { IsArchived = false, Name = name, ToolId = toolId });
            }

            DataAccessHelper.LogInDatabase(unitOfWork, user.Id, Dto.Log.ActionTypeEnum.Connect, Dto.Log.TableTypeEnum.User, $"Connect user {user.Name} (Id={user.Id}) in toolId {toolId} ", _logger);
            unitOfWork.Save();
            var unifiedUser = new UnifiedUser
            {
                Rights = new List<UserRights>(),
                Id = user.Id,
                Name = name,
                Profile = profile
            };
            UpdateUserRights(unifiedUser);
            return unifiedUser;
        }

        private void UpdateUserRights(UnifiedUser unifiedUser)
        {
            unifiedUser.Rights = _mapProfileRights[unifiedUser.Profile];
        }

        public Response<List<Dto.User>> GetAll(int? toolKey)
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    if (!toolKey.HasValue)
                    {
                        return Mapper.Map<List<Dto.User>>(unitOfWork.UserRepository.CreateQuery().ToList());
                    }
                    else
                    {
                        int toolId = GetToolIdFromToolKey(toolKey.Value, unitOfWork);
                        return Mapper.Map<List<Dto.User>>(unitOfWork.UserRepository.CreateQuery().Where(x => x.ToolId == toolId).ToList());
                    }
                }
            });
        }

        public Response<Dto.User> GetUser(int id)
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    return Mapper.Map<Dto.User>(unitOfWork.UserRepository.CreateQuery().FirstOrDefault(u => u.Id == id));
                }
            });
        }

        public Response<Dto.User> GetUserByName(string name, int toolKey, bool useArchive = false)
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int toolId = GetToolIdFromToolKey(toolKey, unitOfWork);
                    return Mapper.Map<Dto.User>(unitOfWork.UserRepository.CreateQuery().FirstOrDefault(u => u.Name == name && u.ToolId == toolId && (!useArchive && !u.IsArchived)));
                }
            });
        }

        public Response<bool> RenameUser(string oldName, string newName, int toolKey, int loggedUserId)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    bool renameSuccess = false;
                    int toolId = GetToolIdFromToolKey(toolKey, unitOfWork);
                    var user = unitOfWork.UserRepository.CreateQuery().FirstOrDefault(u => u.Name == oldName && u.ToolId == toolId);
                    if (user != null)
                    {
                        var newuser = unitOfWork.UserRepository.CreateQuery().FirstOrDefault(u => u.Name == newName && u.ToolId == toolId);
                        if (newuser != null)
                        {
                            // user already exists
                            _logger.Error($"Renaming User aborted since a User with same name already exist: ({oldName} to {newName} on tool #{toolKey} cancelled)");
                        }
                        else
                        {
                            user.Name = newName;
                            DataAccessHelper.LogInDatabase(unitOfWork, loggedUserId, Dto.Log.ActionTypeEnum.Edit, Dto.Log.TableTypeEnum.User, $"Rename User from <{oldName}> to <{newName}> (Id={user.Id})",_logger);
                            unitOfWork.Save();
                            renameSuccess = true;
                        }
                    }
                    else
                    {
                        _logger.Error($"Renaming User aborted sinceNo such ToolKey exists: (tool #{toolKey})");
                    }
                    return renameSuccess;
                }
            });
        }

        public Response<VoidResult> RemoveUser(int id, int loggedUserId)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    unitOfWork.UserRepository.RemoveById(id);
                    var user = unitOfWork.UserRepository.CreateQuery().FirstOrDefault(u => u.Id == id);
                    DataAccessHelper.LogInDatabase(unitOfWork, loggedUserId, Dto.Log.ActionTypeEnum.Remove, Dto.Log.TableTypeEnum.User, $"Remove user {user} (Id={user.Id})", _logger);
                    unitOfWork.Save();
                }
            });
        }

        [Obsolete]
        private int GetToolId(int chamberId, UnitOfWorkUnity unitOfWorkUnity)
        {
            var chamber = unitOfWorkUnity.ChamberRepository.CreateQuery(false).FirstOrDefault(x => x.Id == chamberId);
            if (chamber == null)
                throw new InvalidOperationException("Bad chamber Id " + chamberId);
            return chamber.ToolId;
        }

        private int GetToolIdFromToolKey(int toolKey, UnitOfWorkUnity unitOfWorkUnity)
        {
            var tool = unitOfWorkUnity.ToolRepository.CreateQuery(false).FirstOrDefault(x => x.ToolKey == toolKey);
            if (tool == null)
                throw new InvalidOperationException($"No Tool has the ToolKey = {toolKey}");
            return tool.Id;
        }

    
    }
}
