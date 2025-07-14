using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls
{
    public class ChipsSelector : ListBox
    {
        static ChipsSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChipsSelector), new FrameworkPropertyMetadata(typeof(ChipsSelector)));
        }

        /// <summary>Creates or identifies the element used to display the specified item.</summary>
        /// <returns>The element used to display the specified item.</returns>
        protected override DependencyObject GetContainerForItemOverride() => new ChipsItem();
    }
}
