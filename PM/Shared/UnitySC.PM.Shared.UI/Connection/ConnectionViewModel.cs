using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs;

using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.UI.Connection
{
    public class ConnectionViewModel : ObservableObject, IModalDialogViewModel
    {
        private IUserSupervisor _userSupervisor;
        private ILogger _logger;

        public ConnectionViewModel(IUserSupervisor userSupervisor, ILogger logger)
        {
            _userSupervisor = userSupervisor;
            _logger = logger;
        }

        private string _error = null;

        public string Error
        {
            get => _error; set { if (_error != value) { _error = value; OnPropertyChanged(); } }
        }

        private string _login;

        public string Login
        {
            get => _login;
            set
            {
                if (_login != value)
                {
                    _login = value;
                    OnPropertyChanged();
                    LoginCommand.NotifyCanExecuteChanged();
                    Error = null;
                }
            }
        }

        private string _password;

        public string Password
        {
            get => _password; set { if (_password != value) { _password = value; OnPropertyChanged(); LoginCommand.NotifyCanExecuteChanged(); Error = null; } }
        }

        // Close view
        private bool _closeTrigger;

        public bool CloseTrigger
        {
            get => _closeTrigger; set { if (_closeTrigger != value) { _closeTrigger = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand<PasswordBox> _loginCommand;

        public AutoRelayCommand<PasswordBox> LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new AutoRelayCommand<PasswordBox>(
                passwordBox =>
                {
                    ConnectUser(passwordBox.Password);
                },
                box => { return !string.IsNullOrEmpty(Login); }));
            }
        }

        private AutoRelayCommand _fastLoginCommand;

        public AutoRelayCommand FastLoginCommand
        {
            get
            {
                return _fastLoginCommand ?? (_fastLoginCommand = new AutoRelayCommand(
                () =>
                {
                    Login = "SUPERVISOR";
                    ConnectUser("");
                },
                () => { return true; }));
            }
        }

#pragma warning disable CS1998 // This async method does not have an 'await' operator and will run synchronously.
        //Use the 'await' operator to wait for non-blocking API calls or 'await Task.Run(...)' to perform a job using the processor on a background thread.

        private async void ConnectUser(string password)
        {
            try
            {
                Error = null;
                var user = _userSupervisor.Connect(_login, password);
                if (user == null)
                    Error = "Bad user/password";
                else
                {
                    CloseTrigger = true;
                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during user login");
                Error = "Connection failed";
            }
            finally
            {
                if (!string.IsNullOrEmpty(Error))
                {
                    _password = null;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        private AutoRelayCommand _exitCommand;

        public AutoRelayCommand ExitCommand
        {
            get
            {
                return _exitCommand ?? (_exitCommand = new AutoRelayCommand(
              () =>
              {
                  CloseTrigger = true;
                  DialogResult = false;
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _passwordChangeCommand;

        public AutoRelayCommand PasswordChangeCommand
        {
            get
            {
                return _passwordChangeCommand ?? (_passwordChangeCommand = new AutoRelayCommand(
              () =>
              {
                  Error = null;
              },
              () => { return true; }));
            }
        }

        public bool? DialogResult { get; set; }
    }
}
