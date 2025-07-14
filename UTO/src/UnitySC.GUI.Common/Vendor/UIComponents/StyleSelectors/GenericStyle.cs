using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace UnitySC.GUI.Common.Vendor.UIComponents.StyleSelectors
{
    [ContentProperty(nameof(Style))]
    public class GenericStyle : DispatcherObject, INameScope
    {
        private readonly NameScope _nameScope = new NameScope();

        public Type Type { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Style Style { get; set; }

        public void RegisterName(string name, object scopedElement)
        {
            VerifyAccess();
            _nameScope.RegisterName(name, scopedElement);
        }
        public void UnregisterName(string name)
        {
            VerifyAccess();
            _nameScope.UnregisterName(name);
        }
        public object FindName(string name)
        {
            VerifyAccess();
            return _nameScope.FindName(name);
        }
    }
}
