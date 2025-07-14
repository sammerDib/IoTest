using System;
using System.Collections.Generic;
using AdcTools;
using ADCv9.View;
using GalaSoft.MvvmLight;
using System.Linq;


//  Gestion des users
namespace ADCv9.ViewModel
{
    public partial class RecipeViewModel : ViewModelBase
    {
        private List<UserViewModel> _listUsers = new List<UserViewModel>();
		public List<UserViewModel> listUsers
        {
            get { return _listUsers; }
            set
            {
                _listUsers = value;
                RaisePropertyChanged(() => listUsers);
            }
        }


        private UserViewModel _logedUserChangedSelected = null;
        public UserViewModel LogedUserChangedSelected
        {
            get => _logedUserChangedSelected;
            set
            {
                if (_logedUserChangedSelected != value)
                {
                    _logedUserChangedSelected = value;

                    if ((value != null) && (UserService.CurrentUser.Login != value.Login))
                    {
                        if (!LogOn(value))
                        {
                            // l'utilisateur courant n'est pas issu de la meme source que la liste d'utilisateur. à amelioré
                            _logedUserChangedSelected = listUsers.FirstOrDefault(u => u.Login == UserService.CurrentUser.Login);
                            RaisePropertyChanged();
                            return;
                        }
                    }
                    _logedUserChangedSelected = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsConnected
        {
            get {return UserService.IsConnected; }
            set { RaisePropertyChanged(() => IsConnected); }
        }


        private String _userInitial = string.Empty;
        public String UserInitial
        {
            get
            {
                return _userInitial;
            }
            set
            {
                _userInitial = value;
                RaisePropertyChanged(() => UserInitial);
            }
        }

		public bool IsExpertConnected { get { return IsConnected && (UserService?.CurrentUser?.IsExpert).Value; } set { RaisePropertyChanged(() => IsExpertConnected); } }

        public bool IsOperatorConnected { get { return IsConnected && (UserService?.CurrentUser?.IsOperator).Value; } set { RaisePropertyChanged(() => IsOperatorConnected); } }


        public void InitListUsers()
        {
            listUsers = UserService.GetAll();


            if ((listUsers != null) && (listUsers.Count == 0))
            {
                UserViewModel admin = new UserViewModel(UserService);

                // No user registered ->Create an admin user 

                admin.Login = "admin";
                admin.Role = RoleUser.Expert;
                admin.PassWord = SecurityTool.ComputeHash("passWord");

                UserService.SetUser(admin);

                listUsers.Add(admin);
            }
        
        }



        /// <summary>
        /// Used to connect a user and define access to the UI according to its role
        /// </summary>
        /// <param name="selectedUser"></param>
        public bool LogOn(UserViewModel selectedUser = null)
        {

			// In fist, need to close actual recipe ?
			if (CheckSave() == false)
			{
				return false; ;
			}

            bool result = UserService.Logon(selectedUser?.Login);

            return result;
          
        }


		internal void LogOff()
		{
			CloseRecipe();

            UserService.LogOff();


			//User.GetInstance().Disconnect();

			IsConnected = false;
			IsExpertConnected = false;

            UserInitial = "";
		}
        
	}
    
}
