using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Microsoft.Xaml.Behaviors;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Behaviors
{
    /// <summary>
    /// Allow to automatically scroll to the <see cref="Selector.SelectedItem"/> item in <see cref="ListBox"/>
    /// </summary>
    public class ListBoxAutoScrollToSelectedItemBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }

        private static void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ListBox listBox)) return;
            if (listBox.SelectedItem == null) return;

            var selectedItem = listBox.SelectedItem;
            listBox.Dispatcher?.BeginInvoke((Action)(() =>
            {
                listBox.UpdateLayout();
                listBox.ScrollIntoView(selectedItem);
            }));
        }
    }
}
