using AdcTools;
using ADCv9.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADCv9.BusinessServices
{
    public class UserService : BusinessServiceBase
    {

        public UserViewModel CurrentUser { get; private set; }

        public bool IsConnected { get { return CurrentUser != null; } }

        public UserService(GalaSoft.MvvmLight.Messaging.IMessenger messenger ) : base(messenger) {}


        /// <summary>
        /// Recherche un utilisateur en utilisant sa cle (Login)
        /// </summary>
        /// <param name="Login"></param>
        /// <returns></returns>
        public UserViewModel GetUser(string Login)
        {
            User user = new User();

            if (RegistryTool.GetUserInfos(Login, ref user) == true)
            {

                UserViewModel uvm = AutoMapper.Mapper.Map<UserViewModel>(user);
                uvm.InitEnded();
                return uvm;
            }

            return null;

        }


        public List<UserViewModel> GetAll()
        {
            List<User> newListUsers;
            RegistryTool.GetListUsers(out newListUsers);

            List<UserViewModel> ilistDest = AutoMapper.Mapper.Map<List<UserViewModel>>(newListUsers);

            ilistDest.InitEnded();

            return ilistDest;
        }


        public void SetUser(UserViewModel user)
        {
            User setUser = AutoMapper.Mapper.Map<User>(user);
            RegistryTool.SetUserInfo(setUser.Login, setUser);
        }

        public void RemoveUser(UserViewModel user)
        {
            RegistryTool.DeleteUser(user.Login);
        }



        /// <summary>
        /// Controle du password
        /// </summary>
        /// <param name="user">Utilisateur</param>
        /// <param name="password">Password as valider</param>
        /// <returns>Retour true si le Password est bon sinon false.</returns>
        public bool CheckUserPassword(UserViewModel user, string password)
        {
            if (user != null)
            {
                if (SecurityTool.ComputeHash(password).Equals(user.PassWord))
                {
                    return true;
                }
            }
            return false;
        }



        public bool Logon(string login = null)
        {
            UserLoginViewModel ulvm = ViewModelLocator.Instance.UserLoginViewModel;

            if(!string.IsNullOrEmpty(login))
            {
                ulvm.Login = login;
            }

            bool result = Services.Services.Instance.PopUpService.ShowConfirme("Login",
                ulvm,
                ulvm.ConnectionViewCommand,
                null,
                () => ulvm.Connected,
                "Connection",
                "Cancel"
                );

            if (result)
            {
                ChangeUser(ulvm.User);
            }
#if DEBUG
            else
            {
                UserViewModel fakeUser = new UserViewModel(this);
                fakeUser.Login = "nobody";
                fakeUser.Role = ViewModel.RoleUser.Expert;
                ChangeUser(fakeUser);
                result = true;
            }
#endif

            return result;
        }

#if DEBUG

        public bool AutoLogon(string login = null)
        {
            UserViewModel uvm = GetUser(login);
            if (uvm != null)
            {
                ChangeUser(uvm);
                return true;
            }
            else
            {
                return false;
            }
        }

        #endif


        public void LogOff()
        {
            ChangeUser(null);

        }


        private void ChangeUser(UserViewModel newUser)
        {
            UserViewModel OldUser = CurrentUser;
            CurrentUser = newUser;

            Messenger.Send(new Messages.UserChangedMessage() { OldUser = OldUser, NewUser = CurrentUser });
        }

      public bool UsersManagement()
        {
            UserManagerViewModel umvm = ViewModelLocator.Instance.UserManagerViewModel;


            bool result = Services.Services.Instance.PopUpService.ShowConfirme("Users Management",
                umvm,
                null,
                null,
                null,
                null,
                "Close"
                );

            return result;

        }

        public bool UserEdit(UserViewModel user)
        {
            bool result = Services.Services.Instance.PopUpService.ShowConfirme("Users Management",
                user,
                user.SaveSelectedUserCommand,
                user.CancelChangeSelectedUserCommand,
                ()=> !user.HasChanged,
                "Save",
                "Cancel"
                );

            return result;

        }

    }
}
