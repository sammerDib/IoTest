using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    public static class TextBoxAssist
    {
        public static readonly DependencyProperty HasClearButtonProperty = DependencyProperty.RegisterAttached(
            "HasClearButton", typeof(bool), typeof(TextBoxAssist), new PropertyMetadata(false));

        public static void SetHasClearButton(DependencyObject element, bool value)
        {
            element.SetValue(HasClearButtonProperty, value);
        }

        public static bool GetHasClearButton(DependencyObject element)
        {
            return (bool)element.GetValue(HasClearButtonProperty);
        }

        #region Clear Command

        public static readonly RoutedCommand ClearCommand = new RoutedCommand();

        public static bool GetHandlesClearCommand(DependencyObject obj) => (bool)obj.GetValue(HandlesClearCommandProperty);

        public static void SetHandlesClearCommand(DependencyObject obj, bool value) => obj.SetValue(HandlesClearCommandProperty, value);

        public static readonly DependencyProperty HandlesClearCommandProperty = DependencyProperty.RegisterAttached("HandlesClearCommand", typeof(bool), typeof(TextBoxAssist), new PropertyMetadata(false, OnHandlesClearCommandChanged));

        private static void OnHandlesClearCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.CommandBindings.Add(new CommandBinding(ClearCommand, OnClearCommand));
                }
                else
                {
                    for (int i = element.CommandBindings.Count - 1; i >= 0; i--)
                    {
                        if (element.CommandBindings[i].Command == ClearCommand)
                        {
                            element.CommandBindings.RemoveAt(i);
                        }
                    }
                }
            }
        }

        private static void OnClearCommand(object sender, ExecutedRoutedEventArgs e)
        {
            switch (e.Source)
            {
                case TextBox textBox:
                    textBox.SetCurrentValue(TextBox.TextProperty, null);
                    break;
            }
            e.Handled = true;
        }

        #endregion
    }
}
