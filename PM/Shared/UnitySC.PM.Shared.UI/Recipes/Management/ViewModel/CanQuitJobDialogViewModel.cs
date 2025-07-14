using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs;

namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel
{
    public class CanQuitJobDialogViewModel : ObservableObject, IModalDialogViewModel
    {
        public CanQuitJobDialogViewModel(string message)
        {
            _message = message;
        }
        private string _message = string.Empty;

        public string Message
        {
            get
            {
                return _message;
            }
            set { if (_message != value) { _message = value; OnPropertyChanged(); } }
        }

        private MessageBoxResult _messageResult = MessageBoxResult.None;
        public MessageBoxResult MessageResult
        {
            get
            {
                return _messageResult;
            }
            set { if (_messageResult != value) { _messageResult = value; OnPropertyChanged(); } }
        }


        private AutoRelayCommand _validateCommand;
        public AutoRelayCommand ValidateCommand
        {
            get
            {
                return _validateCommand ?? (_validateCommand = new AutoRelayCommand(
              () =>
              {
                  MessageResult = MessageBoxResult.Yes;
                  DialogResult = true;

              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _discardCommand;
        public AutoRelayCommand DiscardCommand
        {
            get
            {
                return _discardCommand ?? (_discardCommand = new AutoRelayCommand(
              () =>
              {
                  MessageResult = MessageBoxResult.No;
                  DialogResult = false;
              },
              () => { return true; }));
            }
        }
        private AutoRelayCommand _cancelCommand;
        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () =>
              {
                  MessageResult = MessageBoxResult.Cancel;
                  DialogResult = false;
              },
              () => { return true; }));
            }
        }

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }
    }
}
