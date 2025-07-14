using System;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using MvvmDialogs;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.UI.Dialog.ExceptionDialogs
{
    public class ExceptionDialogViewModel : ObservableObject, IModalDialogViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public string DebugInfo { get; set; }

        private bool? _dialogResult;

        public ExceptionDialogViewModel(string message, Exception ex)
        {
            Title = Application.ResourceAssembly.GetName().Name;
            Message = message;
            if (ex == null)
            {
                DebugInfo = message;
            }
            else
            {
                Details = ex.Message;
                DebugInfo = message + "\n" + ex.ToString();
            }
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        private AutoRelayCommand _okCommand;

        public AutoRelayCommand OkCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new AutoRelayCommand(
              () =>
              {
                  DialogResult = true;
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _copyCommand;

        public AutoRelayCommand CopyCommand
        {
            get
            {
                return _copyCommand ?? (_copyCommand = new AutoRelayCommand(
              () =>
              {
                  Clipboard.SetText(DebugInfo);
              },
              () => { return true; }));
            }
        }
    }
}
