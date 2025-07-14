using System;

using Agileo.Common.Access.Users;

using UnitySC.Dataflow.UI.Shared;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.UTO.Controller.Views.Panels.DataFlow.Integration
{
    public class UserSupervisorIntegration : IUserSupervisor
    {
        private readonly ServiceInvoker<IUserService> _dbUserService;
        private string _currentUserName;
        private UserProfiles _currentUserProfile;

        public UserSupervisorIntegration()
        {
            _dbUserService = new ServiceInvoker<IUserService>("UserService",
                ClassLocator.Default.GetInstance<SerilogLogger<IUserService>>(),
                null,
                ClientConfiguration.GetDataAccessAddress());

            GUI.Common.App.Instance.AccessRights.UserLogon += OnAccessRightsUserChanged;
            GUI.Common.App.Instance.AccessRights.UserLogoff += OnAccessRightsUserChanged;
            OnAccessRightsUserChanged();
        }

        private void OnAccessRightsUserChanged(User user = null, UserEventArgs e = null)
        {
            var currentUser = GUI.Common.App.Instance.AccessRights.CurrentUser;

            if (currentUser == null || string.IsNullOrEmpty(currentUser.Name))
            {
                DisconnectLoggedUser();
                return;
            }

            var profileAssociation = App.UtoInstance.UserProfileManager.Current.UserProfiles.
                Find(association => association.UserName.Equals(currentUser.Name));

            _currentUserName = currentUser.Name;
            _currentUserProfile = UserProfiles.Basic;
            if (profileAssociation != null)
            {
                _currentUserProfile = profileAssociation.UserProfile;
            }

            ConnectLoggedUser();
        }

        private void ConnectLoggedUser()
        {
            var toolKey = App.ControllerInstance.ControllerConfig.ToolKey;

            try
            {
                CurrentUser = _dbUserService.Invoke(x =>
                    x.ConnectUserFromToolKey(_currentUserName, _currentUserProfile, toolKey));
            }
            catch (Exception e)
            {
                GUI.Common.App.Instance.Logger.Error(e);
            }

            UserChanged?.Invoke(CurrentUser);
        }

        private void DisconnectLoggedUser()
        {
            CurrentUser = null;
            UserChanged?.Invoke(null);
        }

        public void ChangeName(string oldName, string newName)
        {
            var toolKey = App.ControllerInstance.ControllerConfig.ToolKey;
            var loggedUserId = CurrentUser?.Id ?? -1;
            _dbUserService.Invoke(x => x.RenameUser(oldName, newName, toolKey, loggedUserId));
        }

        #region Implementation of IUserSupervisor

        public UnifiedUser CurrentUser { get; private set; }

        public UnifiedUser Connect(string user, string password) => throw new NotImplementedException("Managed by UTO");

        public void Disconnect() => throw new NotImplementedException("Managed by UTO");

        public event UserChangedEventHandler UserChanged;

        #endregion
    }
}
