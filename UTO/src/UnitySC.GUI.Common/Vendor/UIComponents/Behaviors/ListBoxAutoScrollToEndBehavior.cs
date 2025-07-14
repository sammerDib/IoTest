using System;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Xaml.Behaviors;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    /// <summary>
    /// Allow to automatically scroll to last item in <see cref="ListBox"/>
    /// </summary>
    public class ListBoxAutoScrollToEndBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private static void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is ListBox listBox)) return;
            if (listBox.Items.Count <= 0) return;

            var lastItem = listBox.Items[listBox.Items.Count - 1];
            listBox.Dispatcher?.BeginInvoke((Action)(() =>
            {
                listBox.UpdateLayout();
                listBox.ScrollIntoView(lastItem);
            }));
        }
    }
}
