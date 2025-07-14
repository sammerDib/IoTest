using System;
using System.IO;
using System.Linq;

using Agileo.Common.Access;
using Agileo.Common.Configuration;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.PM.Shared.UserManager.Service.Implementation;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace ADC.User
{
    internal class ADCUserSupervisor : IUserSupervisor
    {

        private ILogger _logger;
        private ModuleConfiguration _configuration;
        private ServiceInvoker<IUserService> _userService;
        public UnifiedUser CurrentUser { get; private set; }

        public event UserChangedEventHandler UserChanged;

        public ADCUserSupervisor()
        {
            _logger=ClassLocator.Default.GetInstance<ILogger>();
            _configuration = ClassLocator.Default.GetInstance<ModuleConfiguration>();

            _userService = new ServiceInvoker<IUserService>("UserService", ClassLocator.Default.GetInstance<SerilogLogger<IUserService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress());

        }
        public UnifiedUser Connect(string user, string password)
        {
            _logger.Information("Connect " + user);
            var oldUserName = CurrentUser?.Name;
            CurrentUser = null;
           
            if (TryGetUserProfile(user, password, out var profile))
            {
                CurrentUser=_userService.Invoke(x => x.ConnectUserFromToolKey(user, profile.UserProfile, _configuration.ToolKey));
                
            }

            
            if (oldUserName != CurrentUser?.Name && UserChanged != null)
                UserChanged.Invoke(CurrentUser);
            return CurrentUser;
        }

        private static string GetFullPath(string path)
        {
            // Check if it is a full path
            if (Path.GetFullPath(path) != path)
            {
                var fullPath = ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().ConfigurationFolderPath;
                path = Path.Combine(fullPath, path);
            }

            if (!File.Exists(path))
            {
                throw new Exception("The file used to get user information for TC is missing " + path);
            }

            return path;
        }

        private bool TryGetUserProfile(string userName, string password, out UserProfileAssociation userProfileAssociation)
        {
            string accessRightsFilePath = GetFullPath(_configuration.UtoAccessRightsFilePath);
            string xsdFilePath = GetFullPath(_configuration.UtoAccessRightsXSDFilePath);

            var agileoAccessRights = new AccessManager();
            agileoAccessRights.Setup(accessRightsFilePath, xsdFilePath, AccessRightsLoadingMode.NavigationAndUsers);

            if (!agileoAccessRights.Logon(userName, password))
            {
                userProfileAssociation = null;
                return false;
            }

            string userProfilesFilePath = GetFullPath(_configuration.UtoUserProfilesFilePath);

            var userProfileManager = new ConfigManager<UserProfileAssociations>(
                new XmlDataContractStoreStrategy(userProfilesFilePath),
                new DataContractCloneStrategy(),
                new DataContractCompareStrategy<UserProfileAssociations>());

            if (!userProfileManager.Load())
            {
                userProfileAssociation = null;
                return false;
            }

            var profiles = userProfileManager.Loaded;

            userProfileAssociation = profiles?.UserProfiles?.FirstOrDefault(x => x.UserName.Equals(userName));
            return userProfileAssociation != null;
        }


        public void Disconnect()
        {
            var oldUserName = CurrentUser?.Name;
            CurrentUser = null;
            if (oldUserName != CurrentUser?.Name && UserChanged != null)
                UserChanged.Invoke(CurrentUser);
        }
    }
}
