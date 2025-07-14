using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Access;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AccessRights.Users;
using UnitySC.Shared.Data;

namespace UnitySC.UTO.Controller.Views.Panels.Setup.AccessRights.Users
{
    public class UnityUserEditorViewModel : UserEditorViewModel
    {
        #region Constructors

        public UnityUserEditorViewModel(IAccessManager accessManager, UnityUserViewModel selectedUser, List<UnityUserViewModel> userList) : base(accessManager, selectedUser, userList.Cast<UserViewModel>().ToList())
        {
            Rules.Add(new DelegateRule(nameof(UserProfile), ValidateUserProfile));

            _userProfile = selectedUser?.UserProfile ?? UserProfiles.Basic;
        }

        #endregion Constructors

        #region Properties

        private UserProfiles _userProfile;

        public UserProfiles UserProfile
        {
            get => _userProfile;
            set => SetAndRaiseIfChanged(ref _userProfile, value);
        }

        #endregion Properties

        #region Validation

        private string ValidateUserProfile() => null;

        #endregion
    }
}
