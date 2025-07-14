using System.Windows;
using System.Windows.Controls;
using MvvmDialogs;

namespace UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.Measures.CustomPointsManagement
{
    public class InputPresetNameDialog : IWindow
    {

        private readonly InputPresetName _dialog;

        public InputPresetNameDialog()
        {
            _dialog = new InputPresetName();
        }

        object IWindow.DataContext
        {
            get => _dialog.DataContext;
            set => _dialog.DataContext = value;
        }

        bool? IWindow.DialogResult
        {
            get => _dialog.DialogResult;
            set => _dialog.DialogResult = value;
        }

        ContentControl IWindow.Owner
        {
            get => _dialog.Owner;
            set => _dialog.Owner = (Window)value;
        }

        bool? IWindow.ShowDialog()
        {
            return _dialog.ShowDialog();
        }

        void IWindow.Show()
        {
            _dialog.Show();
        }
    }
}
