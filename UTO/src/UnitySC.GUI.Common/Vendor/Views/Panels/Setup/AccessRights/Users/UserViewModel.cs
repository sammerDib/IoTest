using System;
using System.Windows.Input;

using Agileo.Common.Access;
using Agileo.Common.Access.Users;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AccessRights.Users
{
    public class UserViewModel : Notifier
    {
        /// <summary>
        /// Constructor for existing user
        /// </summary>
        public UserViewModel(User user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            _name = User.Name;
            _password = User.Password;
            _accessLevel = User.AccessLevel;
        }

        /// <summary>
        /// Constructor for new user
        /// </summary>
        public UserViewModel(string name, string password, AccessLevel accessLevel)
        {
            _name = name;
            _password = password;
            _accessLevel = accessLevel;
        }

        #region Properties

        public User User { get; }
        
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (SetAndRaiseIfChanged(ref _name, value))
                {
                    OnPropertyChanged(nameof(HasChanged));
                }
            }
        }

        private AccessLevel _accessLevel;

        public AccessLevel AccessLevel
        {
            get => _accessLevel;
            set
            {
                if (SetAndRaiseIfChanged(ref _accessLevel, value))
                {
                    OnPropertyChanged(nameof(HasChanged));
                }
            }
        }

        private string _password;

        public string Password
        {
            get => _password;
            set
            {
                if (SetAndRaiseIfChanged(ref _password, value))
                {
                    OnPropertyChanged(nameof(HasChanged));
                }
            }
        }

        #endregion Properties

        public virtual bool HasChanged
        {
            get
            {
                // New user case
                if (User == null)
                {
                    return true;
                }

                if (!User.Name.Equals(Name))
                {
                    return true;
                }

                if (User.AccessLevel != AccessLevel)
                {
                    return true;
                }

                if (!string.Equals(User.Password, Password))
                {
                    return true;
                }

                return false;
            }
        }

        private ICommand _resetCommand;

        public ICommand ResetCommand => _resetCommand ??= new DelegateCommand(ResetCommandExecute, ResetCommandCanExecute);

        private bool ResetCommandCanExecute() => User != null && HasChanged;

        protected virtual void ResetCommandExecute()
        {
            SetAndRaiseIfChanged(ref _name, User.Name, nameof(Name));
            SetAndRaiseIfChanged(ref _accessLevel, User.AccessLevel, nameof(AccessLevel));
            SetAndRaiseIfChanged(ref _password, User.Password, nameof(Password));
            OnPropertyChanged(nameof(HasChanged));
        }
    }
}
