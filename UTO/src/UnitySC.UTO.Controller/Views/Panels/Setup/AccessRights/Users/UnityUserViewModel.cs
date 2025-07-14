using Agileo.Common.Access;
using Agileo.Common.Access.Users;

using UnitySC.Shared.Data;

namespace UnitySC.UTO.Controller.Views.Panels.Setup.AccessRights.Users
{
    public class UnityUserViewModel : UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AccessRights.Users.UserViewModel
    {
        /// <summary>
        /// Constructor for existing user
        /// </summary>
        public UnityUserViewModel(User user, UserProfiles initialUserProfile) : base(user)
        {
            _initialUserProfile = initialUserProfile;
            _userProfile = initialUserProfile;
        }

        /// <summary>
        /// Constructor for new user
        /// </summary>
        public UnityUserViewModel(string name, string password, AccessLevel accessLevel, UserProfiles userProfile) : base(name, password, accessLevel)
        {
            _userProfile = userProfile;
        }

        #region Properties

        private readonly UserProfiles _initialUserProfile;

        private UserProfiles _userProfile;

        public UserProfiles UserProfile
        {
            get => _userProfile;
            set => SetAndRaiseIfChanged(ref _userProfile, value);
        }

        #endregion Properties

        public override bool HasChanged
        {
            get
            {
                if (base.HasChanged)
                {
                    return true;
                }

                if (_initialUserProfile != UserProfile)
                {
                    return true;
                }

                return false;
            }
        }

        protected override void ResetCommandExecute()
        {
            SetAndRaiseIfChanged(ref _userProfile, _initialUserProfile, nameof(UserProfile));
            base.ResetCommandExecute();
        }
    }
}
