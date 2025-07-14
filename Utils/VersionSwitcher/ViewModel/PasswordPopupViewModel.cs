using System;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace VersionSwitcher.ViewModel
{
    public class PasswordPopupViewModel : ObservableObject
    {
        public event EventHandler<PasswordCheckEventArgs> OnPasswordIsValid;
        
        //UnitySC
        private const string SECRET_PASSWORD = "6pfFjEoQZfqQtItiAiiNz3udgQPwm9la1n37KsGs82rXUny+59TITgA9BENGMXKlgbl7Awy+5bkId8UI6LN8+g==";

        private bool _isActive;

        private SecureString _password;

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public SecureString Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public PasswordPopupViewModel()
        {
            IsActive = true;
        }
        
        /// <summary>
        /// Compare the provided SecureString to the reference password (SHA512 + Base64)
        /// </summary>
        /// <param name="securePassword">The password to check</param>
        /// <returns>True if it's a match. False otherwise</returns>
        private bool ValidatePassword(SecureString securePassword)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                string password = new NetworkCredential(string.Empty, securePassword).Password;
                byte[] userPasswordBytes = sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                string encodedUserPassword = Convert.ToBase64String(userPasswordBytes);
                return encodedUserPassword == SECRET_PASSWORD;
            }
            
        }
        
        private RelayCommand _submitPasswordCommand;

        public RelayCommand SubmitPasswordCommand
        {
            get
            {
                return _submitPasswordCommand ?? (_submitPasswordCommand = new RelayCommand(
                     () =>
                    {
                        
                        OnPasswordIsValid?.Invoke(this, new PasswordCheckEventArgs(ValidatePassword(Password)));
                    }));    
            }
        }
    }
}
