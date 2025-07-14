using System;

using CommunityToolkit.Mvvm.ComponentModel;

using MvvmDialogs;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Dialogs
{
    public class GenericMvvmDialogViewModel : ObservableObject, IModalDialogViewModel
    {
        public string Title { get; }

        public object ViewModel { get; }

        public GenericMvvmDialogViewModel(string title, object viewModel)
        {
            Title = title;
            ViewModel = viewModel;
        }

        #region Implementation of IModalDialogViewModel

        public bool? DialogResult => true;

        #endregion

        public void OnClosed()
        {
            if (ViewModel is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
