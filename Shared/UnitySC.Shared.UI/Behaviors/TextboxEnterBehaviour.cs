using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Xaml.Behaviors;

namespace UnitySC.Shared.UI.Behaviors
{
    // Usage example
    // <TextBox>
    //    <i:Interaction.Behaviors>
    //           <behaviors:TextboxEnterBehaviour />
    //    </i:Interaction.Behaviors>
    //</TextBox>

    public class TextboxEnterBehaviour : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += Textbox_KeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyDown -= Textbox_KeyDown;
            base.OnDetaching();
        }

        private void Textbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var textbox = (TextBox)sender;
                var binding = textbox.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                    binding.UpdateSource();
                textbox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}
