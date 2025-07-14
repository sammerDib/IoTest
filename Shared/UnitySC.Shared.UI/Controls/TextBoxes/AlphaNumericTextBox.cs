using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnitySC.Shared.UI.Controls
{
    public class AlphaNumericTextBox : TextBox
    {
        private static readonly Regex s_regex = new Regex("^[a-zA-Z0-9]+$");

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (!s_regex.IsMatch(e.Text))
                e.Handled = true;
            base.OnPreviewTextInput(e);
        }
    }
}
