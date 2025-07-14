using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker
{
    public partial class HexColorTextBox
    {
        public static readonly DependencyProperty ShowAlphaProperty =
            DependencyProperty.Register(nameof(ShowAlpha), typeof(bool), typeof(HexColorTextBox),
                new PropertyMetadata(true));

        public bool ShowAlpha
        {
            get => (bool)GetValue(ShowAlphaProperty);
            set => SetValue(ShowAlphaProperty, value);
        }

        public HexColorTextBox()
        {
            InitializeComponent();
        }

        private void ColorToHexConverter_OnShowAlphaChange(object sender, System.EventArgs e)
        {
            TextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
        }
    }
}
