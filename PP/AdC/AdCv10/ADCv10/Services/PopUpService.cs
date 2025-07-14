using System;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADC.Services
{
    public class PopUpService
    {

        public bool ShowConfirme(string title, ObservableRecipient viewModel, AutoRelayCommand oKCommmand, AutoRelayCommand cancelCommmand, Func<bool> validate, string oKText, string cancelText)
        {

            Controls.PopupWindow popupWindow = new Controls.PopupWindow();

            popupWindow.Title = title;
            popupWindow.ViewModel = viewModel;
            popupWindow.OKButtonViewModelCommand = oKCommmand;
            popupWindow.CancelButtonViewModelCommand = cancelCommmand;
            popupWindow.OKButtonValidateFunction = validate;
            popupWindow.OKButtonText = oKText;
            popupWindow.CancelButtonText = cancelText;
            popupWindow.Owner = Application.Current.MainWindow;

            bool? retour = popupWindow.ShowDialog();

            return retour.Value;
        }

        public bool ShowConfirme(string title, string text)
        {
            return ShowConfirme(title, new Controls.MessageBoxViewModel() { Text = text }, null, null, () => true, "Ok", "Cancel");
        }

        public bool ShowConfirmeYesNo(string title, string text)
        {
            return ShowConfirme(title, new Controls.MessageBoxViewModel() { Text = text }, null, null, () => true, "Yes", "No");
        }

        public void ShowDialogWindow(string title, ObservableRecipient viewModel, int? minWidth = 300, int? minHeight = 300, bool sizeToContent = true, int? maxHeight = null, int? maxWidth = null)
        {
            Window win = CreateWindow(title, viewModel, minWidth, minHeight, sizeToContent, maxHeight, maxWidth);
            win.ShowDialog();
        }

        public Window CreateWindow(string title, ObservableRecipient viewModel, int? minWidth = 300, int? minHeight = 300, bool sizeToContent = true, int? maxHeight = null, int? maxWidth = null)
        {
            PopUpWindow win = new PopUpWindow();
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.Title = title;
            win.Content = viewModel;
            win.DataContext = viewModel;
            win.WindowStyle = WindowStyle.ToolWindow;
            win.Owner = Application.Current.MainWindow;
            if (minHeight.HasValue)
                win.MinHeight = minHeight.Value;
            if (minWidth.HasValue)
                win.MinWidth = minWidth.Value;
            if (maxHeight.HasValue)
                win.MaxHeight = minHeight.Value;
            if (maxWidth.HasValue)
                win.MaxWidth = maxWidth.Value;
            if (sizeToContent)
                win.SizeToContent = SizeToContent.WidthAndHeight;
            return win;
        }
    }
}
