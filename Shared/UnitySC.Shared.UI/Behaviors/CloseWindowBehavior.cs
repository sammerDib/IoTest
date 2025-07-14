using System.Windows;

using Microsoft.Xaml.Behaviors;

namespace UnitySC.Shared.UI.Behaviors
{
    // Behavior to close windows from view model
    public class CloseWindowBehavior : Behavior<Window>
    {
        public bool CloseTrigger
        {
            get { return (bool)GetValue(CloseTriggerProperty); }
            set { SetValue(CloseTriggerProperty, value); }
        }

        public static readonly DependencyProperty CloseTriggerProperty =
            DependencyProperty.Register("CloseTrigger", typeof(bool), typeof(CloseWindowBehavior), new PropertyMetadata(false, OnCloseTriggerChanged));

        private static void OnCloseTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as CloseWindowBehavior;

            if (behavior != null)
            {
                behavior.OnCloseTriggerChanged();
            }
        }

        private void OnCloseTriggerChanged()
        {
            // when closetrigger is true, close the window
            if (CloseTrigger)
            {
                AssociatedObject.Close();
            }
        }
    }
}
