using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ADC.Controls
{
    internal class RegexTextBox : TextBox
    {
        public string Regex
        {
            get { return (string)GetValue(RegexProperty); }
            set { SetValue(RegexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Regex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegexProperty =
            DependencyProperty.Register("Regex", typeof(string), typeof(RegexTextBox), new PropertyMetadata(null));

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (Regex != null)
            {
                Regex regex = new Regex(Regex);
                if (!regex.IsMatch(e.Text))
                    e.Handled = true;
            }
            base.OnPreviewTextInput(e);
        }
    }
}
