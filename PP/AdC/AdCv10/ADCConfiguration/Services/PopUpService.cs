using System;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADCConfiguration.Services
{
    public class PopUpService
    {
        public bool ShowConfirme(string title, ObservableRecipient viewModel, AutoRelayCommand oKCommmand, AutoRelayCommand cancelCommmand, Func<bool> validate, string oKText, string cancelText, bool warningIsVisible = false)
        {
            Controls.PopupWindow popupWindow = new Controls.PopupWindow();
            popupWindow.Title = title;
            popupWindow.ViewModel = viewModel;
            popupWindow.OKButtonViewModelCommand = oKCommmand;
            popupWindow.CancelButtonViewModelCommand = cancelCommmand;
            popupWindow.OKButtonValidateFunction = validate;
            popupWindow.OKButtonText = oKText;
            popupWindow.CancelButtonText = cancelText;
            popupWindow.WarningVisible = warningIsVisible;

            bool? retour = popupWindow.ShowDialog();

            return retour.Value;
        }

        public bool ShowConfirme(string title, string text, bool warningIsVisible = false)
        {
            return ShowConfirme(title, new Controls.MessageBoxViewModel() { Text = text }, null, null, () => true, "Ok", "Cancel", warningIsVisible);
        }
    }
}
