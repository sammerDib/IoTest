using ADCConfiguration.ViewModel;

using CommunityToolkit.Mvvm.Messaging;

namespace ADCConfiguration.Services
{
    public class AuthentificationService
    {
        public IMessenger Messenger { get; private set; }


        private UserViewModel _currentUser = null;

        public UserViewModel CurrentUser { get => _currentUser; set => _currentUser = value; }


        public AuthentificationService(IMessenger messenger)
        {
            Messenger = messenger;
        }


        public bool Logon(string login = null)
        {

            UserLoginViewModel ulvm = new UserLoginViewModel();

            if (!string.IsNullOrEmpty(login))
            {
                ulvm.Login = login;
            }

            //bool result = Services.Instance.PopUpService.ShowConfirme("Login",
            //    ulvm,
            //    ulvm.ConnectionViewCommand,
            //    null,
            //    () => ulvm.Connected,
            //    "Connection",
            //    "Cancel"
            //    );

            //if (result)
            {
                ChangeUser(ulvm.User);
                Services.Instance.LogService.LogInfo("User logon");
            }

            //return result;
            return true;
        }

        private void ChangeUser(UserViewModel newUser)
        {
            UserViewModel OldUser = CurrentUser;
            CurrentUser = newUser;

            Messenger.Send(new Messages.UserChangedMessage() { OldUser = OldUser, NewUser = CurrentUser });
        }

    }
}
