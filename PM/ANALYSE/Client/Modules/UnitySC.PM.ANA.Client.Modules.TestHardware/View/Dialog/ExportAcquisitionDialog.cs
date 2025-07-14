using System.Windows;
using System.Windows.Controls;
using MvvmDialogs;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.View.Dialog
{
    public class ExportAcquisitionDialog : IWindow
    {

        private readonly ExportAcquisition _dialog;

        public ExportAcquisitionDialog()
        {
            _dialog = new ExportAcquisition();
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
