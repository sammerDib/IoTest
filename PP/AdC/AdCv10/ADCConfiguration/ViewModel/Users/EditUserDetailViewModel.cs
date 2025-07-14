using MvvmValidation;

using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Dto = UnitySC.DataAccess.Dto;

namespace ADCConfiguration.ViewModel.Users
{
    public class EditUserDetailViewModel : ViewModelWithMenuBase
    {
        private Dto.User _user;
        private ValidationHelper _parentValidator;

        public Dto.User DtoUser
        {
            get { return _user; }
        }
        public EditUserDetailViewModel(Dto.User user, ValidationHelper parentValidator)
        {
            _user = user;
            _parentValidator = parentValidator;
            Validator.AddRequiredRule(() => Name, string.Format(ADC.Ressources.Properties.Messages.ErrFieldRequired, nameof(Name)));
        }

        public string Name
        {
            get => _user.Name; set { if (_user.Name != value) { _user.Name = value; OnPropertyChanged(); _parentValidator.Validate("Name"); } }
        }

        private string _login;
        public string Login
        {
            get => _login; set { if (_login != value) { _login = value; OnPropertyChanged(); _parentValidator.Validate("Login"); } }
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName; set { if (_firstName != value) { _firstName = value; OnPropertyChanged(); } }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName; set { if (_lastName != value) { _lastName = value; OnPropertyChanged(); } }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = Security.ComputeHash(value);
                OnPropertyChanged();
            }
        }


        private bool _passwordRequired;
        public bool PasswordRequired
        {
            get => _passwordRequired; set { if (_passwordRequired != value) { _passwordRequired = value; OnPropertyChanged(); } }
        }

        private string _email;
        public string Email
        {
            get => _email; set { if (_email != value) { _email = value; OnPropertyChanged(); } }
        }

        private int _role;
        public RoleUser Role
        {
            get => (RoleUser)_role; set { if ((RoleUser)_role != value) { _role = (int)value; OnPropertyChanged(); } }
        }

        public void NewPassword()
        {
            PasswordRequired = true;
            Password = null;
        }

        public bool IsEnabled
        {
            get { return !_user.IsArchived; }
            set
            {
                _user.IsArchived = !value;
                OnPropertyChanged();
            }
        }


        #region Commands

        private AutoRelayCommand _newPasswordCommand;
        public AutoRelayCommand NewPaswordCommand
        {
            get
            {
                return _newPasswordCommand ?? (_newPasswordCommand = new AutoRelayCommand(
              () =>
              {
                  NewPassword();
              },
              () => { return true; }));
            }
        }

        #endregion
    }
}
