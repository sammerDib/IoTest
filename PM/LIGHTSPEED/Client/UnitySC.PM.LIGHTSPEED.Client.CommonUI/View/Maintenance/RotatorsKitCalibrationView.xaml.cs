using System.Windows.Controls;
using System.Windows.Input;

namespace UnitySC.PM.LIGHTSPEED.Client.CommonUI.View.Maintenance
{
    /// <summary>
    /// Logique d'interaction pour RotatorsKitCalibrationView.xaml
    /// </summary>
    public partial class RotatorsKitCalibrationView : UserControl
    {
        public RotatorsKitCalibrationView()
        {
            InitializeComponent();
        }

        private void OnKeyDownHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var textbox = (TextBox)sender;
                var binding = textbox.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                    binding.UpdateSource();
                //textbox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}
