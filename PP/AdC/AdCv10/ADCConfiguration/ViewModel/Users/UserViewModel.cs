using MvvmValidation;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Utils.ViewModel;

using Dto = UnitySC.DataAccess.Dto;

namespace ADCConfiguration.ViewModel
{
    public enum RoleUser
    {
        Expert,
        User
    }

    public class UserViewModel : ViewModelWithMenuBase
    {

        private UserViewModel _userOldValue = null;

        public int Id { get; set; }

        private string _firstName = "";
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; OnPropertyChanged(); }
        }

        private string _lastName = "";
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; OnPropertyChanged(); }
        }

        private string _login = "";
        public string Login
        {
            get { return _login; }
            set { _login = value; OnPropertyChanged(); }

        }

        private string _email = "";
        public string Email
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged(); }

        }

        private string _passWord = "";
        public string PassWord
        {
            get { return _passWord; }
            set
            {

                _passWord = Security.ComputeHash(value);
                //_passWord = value;
                OnPropertyChanged();
            }

        }

        private string _passWordControl = "";
        public string PassWordControl
        {
            get { return _passWordControl; }
            set
            {

                _passWordControl = Security.ComputeHash(value);

                //_passWordControl = value;
                OnPropertyChanged();
            }
        }



        private RoleUser _role = RoleUser.User;
        public RoleUser Role
        {
            get { return _role; }
            set { _role = value; OnPropertyChanged(); }
        }


        private void RaiseIsProperty()
        {
            OnPropertyChanged(nameof(IsExpert));
            OnPropertyChanged(nameof(IsUser));
            OnPropertyChanged(nameof(IsPasswordSet));
        }

        public bool IsExpert { get { return _role == RoleUser.Expert; } }

        public bool IsUser { get { return _role == RoleUser.User; } }

        public bool IsPasswordSet { get { return !IsNew && !string.IsNullOrEmpty(PassWord); } }

        public UserViewModel()
        {
            Validator.AddRequiredRule(() => FirstName, string.Format(ADC.Ressources.Properties.Messages.ErrFieldRequired, nameof(FirstName)));
            Validator.AddRequiredRule(() => LastName, string.Format(ADC.Ressources.Properties.Messages.ErrFieldRequired, nameof(LastName)));
            Validator.AddRequiredRule(() => Login, string.Format(ADC.Ressources.Properties.Messages.ErrFieldRequired, nameof(Login)));
            Validator.AddRequiredRule(() => Email, string.Format(ADC.Ressources.Properties.Messages.ErrFieldRequired, nameof(Email)));
            Validator.AddRequiredRule(() => PassWord, string.Format(ADC.Ressources.Properties.Messages.ErrFieldRequired, nameof(PassWord)));
            Validator.AddRequiredRule(() => Role, string.Format(ADC.Ressources.Properties.Messages.ErrFieldRequired, nameof(Role)));

            Validator.AddRule(nameof(Email),
                () =>
                {
                    const string regexPattern =
                        @"^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,4})$";
                    return RuleResult.Assert(System.Text.RegularExpressions.Regex.IsMatch(Email, regexPattern),
                        ADC.Ressources.Properties.Messages.ErrBadFormatMail);
                });


            Validator.AddRule(nameof(PassWordControl),
                () =>
                {
                    return RuleResult.Assert(IsPasswordSet || (!IsPasswordSet && (PassWord == PassWordControl)), ADC.Ressources.Properties.Messages.ErrOldNewPassWordNoMatch);
                }
                );


        }

        public void Init()
        {
            if (_userOldValue == null)
                _userOldValue = new UserViewModel();

            Services.Services.Instance.MapperService.Mapper.Map(this, _userOldValue);

            RaiseIsProperty();


            _userOldValue.InitEnded();

        }

        public bool Save()
        {
            if (HasErrors) return false;

            IsNew = false;


            //   UserService.SetUser(Services.Services.Instance.MapperService.Mapper.Map<Dto.User>(this),
            //       Services.Services.Instance.AuthentificationService.CurrentUser.Id);

#warning ** USP **  Save Set User to add
            //var address = new ServiceAddress() { Host = "localhost", Port = 2221 }; //ClientConfiguration.GetDataAccessAddress()
            //var userService = new ServiceInvoker<IUserService>("UserService", ClassLocator.Default.GetInstance<SerilogLogger<IUserService>>(), null, address);

            Init();

            this.ChangeProcessed();


            return true;
        }

        public void CancelChange()
        {
            Services.Services.Instance.MapperService.Mapper.Map(_userOldValue, this);

            _userOldValue.ChangeProcessed();
            this.ChangeProcessed();

        }

        private AutoRelayCommand _saveSelectedUserCommand = null;
        public AutoRelayCommand SaveSelectedUserCommand
        {
            get
            {
                return _saveSelectedUserCommand ?? (_saveSelectedUserCommand = new AutoRelayCommand(
              () =>
              {
                  Save();
              },
              () => { return !HasErrors; }));
            }
        }

        private AutoRelayCommand _cancelChangeSelectedUserCommand = null;
        public AutoRelayCommand CancelChangeSelectedUserCommand
        {
            get
            {
                return _cancelChangeSelectedUserCommand ?? (_cancelChangeSelectedUserCommand = new AutoRelayCommand(
              () =>
              {
                  CancelChange();
              },
              () => { return true; }));
            }
        }
    }
}
