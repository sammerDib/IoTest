using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Access;
using Agileo.Common.Localization;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AccessRights.Users
{
    public class UserEditorViewModel : NotifyDataError
    {
        #region Fields

        private readonly List<UserViewModel> _userList;
        private readonly IAccessManager _accessManager;

        #endregion Fields

        #region Constructors

        public UserEditorViewModel()
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }

            _isNewPasswordEditionEnabled = true;
            _name = "User";
            _newUserLevel = AccessLevel.Level5;
        }

        public UserEditorViewModel(IAccessManager accessManager, UserViewModel selectedUser, List<UserViewModel> userList)
        {
            Rules.Add(new DelegateRule(nameof(Name), ValidateName));
            Rules.Add(new DelegateRule(nameof(ConfirmedNewPassword), ValidateConfirmedPassword));
            Rules.Add(new DelegateRule(nameof(OldPassword), ValidateOldPassword));

            _accessManager = accessManager;
            UserViewModel = selectedUser;
            _userList = userList;

            IsUserCreation = UserViewModel == null;
            
            Name = UserViewModel?.Name ?? $"User {userList.Count + 2}";
            NewUserLevel = UserViewModel?.AccessLevel ?? AccessLevel.Visibility;
        }
        
        #endregion Constructors

        #region Properties

        public UserViewModel UserViewModel { get; }
        
        public bool IsUserCreation { get; }

        public IEnumerable<AccessLevel> AvailableAccessLevels
        {
            get
            {
                var accessLevels = Enum.GetValues(typeof(AccessLevel)).Cast<AccessLevel>();

                if (IsInDesignMode)
                {
                    return accessLevels;
                }

                return accessLevels.Where(level => level <= _accessManager.CurrentUser.AccessLevel);
            }
        }

        public bool CanEditAccessLevel
        {
            get
            {
                if (IsUserCreation)
                {
                    return true;
                }

                var loggedUserCanEdit = _accessManager.CurrentUser.AccessLevel >= UserViewModel.AccessLevel;

                // Prevent access level from being changed if the user being edited is the highest.
                var anyHigherUser = _userList.Any(user => user.AccessLevel >= UserViewModel.AccessLevel);

                return loggedUserCanEdit && anyHigherUser;
            }
        }

        private string _confirmedNewPassword = string.Empty;
        
        public string ConfirmedNewPassword
        {
            get => _confirmedNewPassword;
            set => SetAndRaiseIfChanged(ref _confirmedNewPassword, value);
        }

        private bool _isNewPasswordEditionEnabled;

        public bool IsNewPasswordEditionEnabled
        {
            get => IsUserCreation || _isNewPasswordEditionEnabled;
            set
            {
                if (SetAndRaiseIfChanged(ref _isNewPasswordEditionEnabled, value) && !value)
                {
                    ClearAllErrors();
                }
            }
        }
        
        private string _newPassword = string.Empty;
        
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (SetAndRaiseIfChanged(ref _newPassword, value) &&
                    !string.IsNullOrEmpty(ConfirmedNewPassword))
                {
                    ApplyRules(nameof(ConfirmedNewPassword));
                }
            }
        }

        private AccessLevel _newUserLevel;
        
        public AccessLevel NewUserLevel
        {
            get => _newUserLevel;
            set => SetAndRaiseIfChanged(ref _newUserLevel, value);
        }

        private string _name = string.Empty;
        
        public string Name
        {
            get => _name;
            set => SetAndRaiseIfChanged(ref _name, value);
        }

        private string _oldPassword = string.Empty;
        
        public string OldPassword
        {
            get => _oldPassword;
            set => SetAndRaiseIfChanged(ref _oldPassword, value);
        }

        #endregion Properties
        
        #region Methods
        
        public bool UserIsValid() => !HasErrors;

        #endregion Methods

        #region Validation

        private string ValidateName()
        {
            var nameChanged = IsUserCreation || !string.Equals(Name, UserViewModel.Name);

            if (nameChanged)
            {
                if (_userList.Any(user => string.Equals(user.Name, Name)))
                {
                    return LocalizationManager.GetString(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_NAME_ALREADY_USED));
                }
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return LocalizationManager.GetString(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_NAME_CANNOT_BE_EMPTY));
                }
            }

            return null;
        }

        private string ValidateConfirmedPassword()
        {
            if (IsUserCreation)
            {
                if (!string.Equals(NewPassword, ConfirmedNewPassword))
                {
                    return LocalizationManager.GetString(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_PASSWORD_CONFIRMATION_FAILED));
                }
            }
            else
            {
                if (IsNewPasswordEditionEnabled && !string.Equals(NewPassword, ConfirmedNewPassword))
                {
                    return LocalizationManager.GetString(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_PASSWORD_CONFIRMATION_FAILED));
                }
            }

            return null;
        }

        private string ValidateOldPassword()
        {
            if (!IsNewPasswordEditionEnabled)
            {
                return null;
            }

            bool isValid;

            if (UserViewModel != null)
            {
                // Edition of an existing user
                if (UserViewModel.User != null)
                {
                    if (!string.Equals(UserViewModel.User.Password, UserViewModel.Password))
                    {
                        // The password has been changed but is not yet applied.
                        // It is therefore not encrypted by the AccessRightsManager and we must therefore compare them as strings.
                        isValid = string.Equals(UserViewModel.Password, _oldPassword);
                    }
                    else
                    {
                        isValid = App.Instance.AccessRights.AccessRightsManager.UserManager.CheckPassword(UserViewModel.User, _oldPassword);
                    }
                }
                else
                {
                    // Edition of a new user but not saved
                    isValid = string.Equals(UserViewModel.Password, _oldPassword);
                }
            }
            else
            {
                // Creation of user
                isValid = true;
            }

            return isValid ? null : LocalizationManager.GetString(nameof(AccessRightsResources.ACCESSRIGHTSPANEL_INCORRECT_PASSWORD));
        }

        #endregion
    }
}
