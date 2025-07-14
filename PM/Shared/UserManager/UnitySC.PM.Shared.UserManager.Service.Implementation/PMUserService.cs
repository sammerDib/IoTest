using System;
using System.IO;
using System.Linq;
using System.ServiceModel;

using Agileo.Common.Access;
using Agileo.Common.Configuration;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.UserManager.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PMUserService : BaseService, IPMUserService
    {
        private readonly PMConfiguration _configuration;
        private readonly ServiceInvoker<IUserService> _dbUserService;

        public PMUserService(ILogger logger, PMConfiguration pmConfiguration) : base(logger, ExceptionType.BuissnessException)
        {
            _configuration = pmConfiguration;
            _dbUserService = new ServiceInvoker<IUserService>("UserService", ClassLocator.Default.GetInstance<SerilogLogger<IUserService>>(), null, ClassLocator.Default.GetInstance<PMConfiguration>().DataAccessAddress);
        }

        public Response<UnifiedUser> Connect(string userName, string password, int chamberKey, int toolKey)
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (TryGetUserProfile(userName, password, out var profile))
                {
                    return _dbUserService.Invoke(x => x.ConnectUserFromToolKey(userName, profile.UserProfile, toolKey));
                }

                return null;
            });
        }

        #region Private methods

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

        #endregion
    }
}
