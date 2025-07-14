using System.Collections.Generic;

using Agileo.Common.Configuration;

using UnitySC.Shared.Data;

namespace UnitySC.PM.Shared.UserManager.Service.Implementation
{
    public class UserProfileAssociations : IConfiguration
    {
        public List<UserProfileAssociation> UserProfiles { get; set; } = new List<UserProfileAssociation>();

        #region Implementation of IConfiguration

        public string ValidatedParameters() => UserProfiles == null ? $"{nameof(UserProfiles)} must be defined" : null;

        public string ValidatingParameters() => null;

        #endregion
    }

    public class UserProfileAssociation
    {
        public string UserName { get; set; }

        public UserProfiles UserProfile { get; set; }
    }
}
