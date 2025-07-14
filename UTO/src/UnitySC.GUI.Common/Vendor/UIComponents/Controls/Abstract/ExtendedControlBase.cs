using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Controls.Abstract
{
    public abstract class ExtendedControlBase : Control
    {
        protected bool GetTemplateChild<T>(string childName, out T child) where T : DependencyObject
        {
            child = GetTemplateChild(childName) as T;
            return child != null;
        }
    }
}
